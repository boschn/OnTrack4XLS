Option Explicit On
Option Strict On


Imports System.ComponentModel
Imports Telerik.WinControls
Imports Telerik.WinControls.UI
Imports System.Data

Imports OnTrack.Database
Imports OnTrack.XChange

'*********************
'********************* Code-Behind the MQFFeedWizard
Public Class UIWizardMQFFeed

    Private _MQFWorkbookName As String = ""
    Private _MQFObject As MessageQueue
    Private _MQFWorkbook As Excel.Workbook
    Private WithEvents _errorlog As SessionMessageLog 'get reference for event handling of new errors
    Private WithEvents _BuildMQFWorker As New BackgroundWorker
    Private WithEvents _PreProcessWorker As New BackgroundWorker
    Private WithEvents _ProcessWorker As New BackgroundWorker
    Private WithEvents _UpdateXLSWorker As New BackgroundWorker

    Friend WithEvents MQFDataSet As System.Data.DataSet
    Friend WithEvents _datamodel As UIMQFDataModel

    Private Delegate Sub SetProgressCallback([percentage] As Integer, [text] As String)
    Private Delegate Sub SetStatusCallback([text] As String)
    Private Delegate Sub SetAfterWorkCallback()

    Public Sub New()
        ' attach to the errorlog
        _errorlog = ot.Errorlog
        _BuildMQFWorker = New BackgroundWorker
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.MQFWizard.CommandArea.NextButton.Enabled = False

        ' load the current workbooks to the Listbox
        For Each aWb As Excel.Workbook In Globals.ThisAddIn.Application.Workbooks
            Dim aDataItem As New RadListDataItem

            If modXLSMessageQueueFile.checkWorkbookIfMQF(aWb) Then
                aDataItem.Text = aWb.Name
                aDataItem.Enabled = True
            Else
                aDataItem.Text = aWb.Name
                aDataItem.Enabled = False
            End If
            ' add item
            Me.WorkbookList.Items.Add(aDataItem)

            Me.PreProcessButton.ButtonElement.ToolTipText = "Run Preprocess"

        Next

        'MQFDataSet
        '
        Me.MQFViewGrid.MasterTemplate.DataSource = Me.MQFDataSet
        Me.MQFDataSet = New System.Data.DataSet()
        Me._datamodel = New UIMQFDataModel(_MQFObject)
        CType(Me.MQFDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me._datamodel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MQFDataSet.DataSetName = "NewDataSet"
        Me.MQFDataSet.Tables.AddRange(New System.Data.DataTable() {_datamodel})
    End Sub

    '**** 
    '**** WELCOME PAGE -> Select the MQF File
#Region "WelcomePage"

    Private Sub workbookListContextMenuStrip_Opening(sender As Object, e As ComponentModel.CancelEventArgs) Handles workbookListContextMenuStrip.Opening
        'System.Diagnostics.Debug.WriteLine("opening")
    End Sub

    Private Sub WorkbookListContextMenu_loadWorkbook(ByVal sendder As Object, ByVal e As EventArgs) Handles OpenWorkbookButton.Click _
        , workbookListContextMenuStrip.Click

        'Dim _MQFWorkbook As Excel.Workbook
        Dim lastError As SessionMessage

        If Not ot.RequireAccess(otAccessRight.ReadUpdateData) Then
            Me.WelcomeStatusLabel.Text = "No ReadUpdate Connection to the OnTrack Database available"
            Exit Sub
        End If

        '** locate a new MQF
        _MQFWorkbook = modXLSMessageQueueFile.LocateAndOpenMQF()

        If _MQFWorkbook Is Nothing Then
            lastError = GetLastError()
            If Not lastError Is Nothing Then
                Me.WelcomeStatusLabel.Text = lastError.Message
            Else
                Me.WelcomeStatusLabel.Text = "Workbook could not be openend"
            End If

            Exit Sub
        End If

        '** check if already in list
        For Each anItem As RadListDataItem In Me.WorkbookList.Items
            If LCase(anItem.Text) = LCase(_MQFWorkbook.Name) Then
                Me.WelcomeStatusLabel.Text = "Workbook already in List"
                Me.WorkbookList.SelectedItem = anItem
                Exit Sub
            End If
        Next

        ' Add to list
        Dim aNewItem As New RadListDataItem
        aNewItem.Text = _MQFWorkbook.Name
        Me.WorkbookList.Items.Add(aNewItem)
        Me.WorkbookList.SelectedItem = aNewItem

    End Sub

    ''' <summary>
    ''' handles the SelectIndex Workbooklist Changed event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Workbooklist_SelectedIndexChanged(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles WorkbookList.SelectedIndexChanged
        Dim item As RadListDataItem = TryCast(Me.WorkbookList.SelectedItem, RadListDataItem)
        If Not item Is Nothing Then
            _MQFWorkbookName = item.Text
            _MQFWorkbook = Globals.ThisAddIn.Application.Workbooks(_MQFWorkbookName)
            Me.MQFWizard.CommandArea.NextButton.Enabled = True
            Me.MQFSelectedTextbox.Text = item.Text
            Me.WizardPage1.Title = "Pre-Process " & item.Text
            'Me.WizardPage2.Title = "Process " & item.Text

        End If
    End Sub

#End Region
    '**** 
    '**** Preprocess -> Run through Preprocessing of MQF
#Region "Preprocess"

    ''' <summary>
    ''' load the preprocessing Wizard page
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Preprocess_load()
        '** preprocess
        Me.ProcessStatusLabel.Text = "importing excel message queue file ..."
        Me.ProcessStatusStrip.Refresh()

        Me.WizardPage1.Enabled = False
        Me.PreProcessButton.Enabled = False

        'Me.Enabled = False
        Me.Cursor = Windows.Forms.Cursors.WaitCursor

        ' disabled next
        Me.MQFWizard.CommandArea.NextButton.Enabled = False
        Me.PreProcessButton.Visible = True
        Me.PreProcessButton.Enabled = False
        Me.PreProcessButton.ButtonElement.ShowBorder = False
        Me.PreProcessButton.ButtonElement.ToolTipText = "Preprocess the message queue file and check the messages with the data in the database"

        Me.ProcessButton.Visible = True
        Me.ProcessButton.Enabled = False
        Me.ProcessButton.ButtonElement.ShowBorder = False
        Me.ProcessButton.ButtonElement.ToolTipText = "Process the message queue file and feed the messages to the database"

        Me.MQFViewGrid.Enabled = False
        Me.MQFViewGrid.Visible = False

        Me.ProcessPanel.Controls.Add(Me.ProgressPictureBox)
        Me.ProgressPictureBox.Visible = True

        Me.WizardPage1.Title = "Pre-Process :" & _MQFWorkbook.Name
        Me.WizardPage1.Header = "select the preprocess button to check and prepare the messages before feeding"

        Me.Refresh()

        Me._BuildMQFWorker.WorkerReportsProgress = True
        ' run
        _BuildMQFWorker.RunWorkerAsync()
    End Sub


    ''' <summary>
    ''' handles the PreProcess Button Click
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub PreProcessButton_Click(sender As Object, e As EventArgs) Handles PreProcessButton.Click


        '** preprocess
        Me.ProcessStatusLabel.Text = "starting preprocess run ..."
        Me.ProcessStatusStrip.Refresh()

        'Me.WizardPage1.Enabled = False
        Me.PreProcessButton.Enabled = False
        Me.ProcessButton.Enabled = False
        'Me.Enabled = False
        Me.Cursor = Windows.Forms.Cursors.WaitCursor

        '** preprocess
        ' disabled next
        Me.MQFWizard.CommandArea.NextButton.Enabled = False

        Me.MQFViewGrid.Enabled = False
        Me.MQFViewGrid.Visible = False

        Me.ProgressPictureBox.Visible = True

        Me.Refresh()

        Me._PreProcessWorker.WorkerReportsProgress = True
        ' run
        _PreProcessWorker.RunWorkerAsync()

    End Sub
    ' This method demonstrates a pattern for making thread-safe 
    ' calls on a Windows Forms control.  
    ' 
    ' If the calling thread is different from the thread that 
    ' created the control, this method creates a 
    ' Callback and calls itself asynchronously using the 
    ' Invoke method. 
    ' 
    ' If the calling thread is the same as the thread that created 
    ' the  control, the  properties are/is set directly.  
    ''' <summary>
    ''' Set the Progress
    ''' </summary>
    ''' <param name="percentage"></param>
    ''' <param name="text"></param>
    ''' <remarks></remarks>
    Private Sub SetProgress(ByVal [percentage] As Integer, ByVal [text] As String)

        ' InvokeRequired required compares the thread ID of the 
        ' calling thread to the thread ID of the creating thread. 
        ' If these threads are different, it returns true. 
        If Me.ProcessStatusStrip.InvokeRequired Then
            Dim d As New SetProgressCallback(AddressOf SetProgress)
            Me.Invoke(d, New Object() {[percentage], [text]})
        Else
            If percentage > 100 Then percentage = 100
            Me.PreprocessProgressBar.Value1 = [percentage]
            Me.ProcessStatusLabel.Text = [text]
            Me.ProcessStatusStrip.Refresh()
        End If
    End Sub

    ''' <summary>
    ''' AfterWork (Preprocess) CleanUp
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetAfterBuildModelWork()

        ' InvokeRequired required compares the thread ID of the 
        ' calling thread to the thread ID of the creating thread. 
        ' If these threads are different, it returns true. 
        If Me.ProcessStatusStrip.InvokeRequired Then
            Dim d As New SetAfterWorkCallback(AddressOf SetAfterBuildModelWork)
            Me.Invoke(d, New Object() {})
        Else
            Me.Cursor = Windows.Forms.Cursors.Default
            Me.PreProcessButton.Enabled = True
            Me.WizardPage1.Enabled = True
            'Me.Enabled = True
            Me.ProcessStatusStrip.Refresh()
            If _MQFObject IsNot Nothing Then
                _datamodel = New UIMQFDataModel(_MQFObject)
                If _datamodel.Initialize() Then _datamodel.LoadData()

                Me.MQFViewGrid.DataSource = _datamodel
                Me.MQFViewGrid.BestFitColumns()

                Me.MQFViewGrid.Enabled = True
                Me.MQFViewGrid.Visible = True

                Me.ProgressPictureBox.Visible = False

                Me.PreProcessButton.Visible = True
                Me.PreProcessButton.Enabled = True
                Me.PreProcessButton.ButtonElement.ShowBorder = True

                Me.ProcessButton.Visible = True
                Me.ProcessButton.Enabled = False
                Me.ProcessButton.ButtonElement.ShowBorder = False

                Me.ProcessButton.ButtonElement.ToolTipText = "Preprocess and check the data first"

                Me.WizardPage1.Title = "Pre-Process :" & _MQFWorkbook.Name
                Me.WizardPage1.Header = "select the preprocess button to check and prepare the messages before feeding"
                Me.MQFWizard.NextButton.Enabled = False
                Me.Refresh()
            End If
        End If
    End Sub

    ''' <summary>
    ''' AfterWork (Preprocess) CleanUp
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetAfterPreProcessWork()

        ' InvokeRequired required compares the thread ID of the 
        ' calling thread to the thread ID of the creating thread. 
        ' If these threads are different, it returns true. 
        If Me.ProcessStatusStrip.InvokeRequired Then
            Dim d As New SetAfterWorkCallback(AddressOf SetAfterPreProcessWork)
            Me.Invoke(d, New Object() {})
        Else
            Me.Cursor = Windows.Forms.Cursors.Default
            Me.PreProcessButton.Enabled = True
            Me.WizardPage1.Enabled = True
            'Me.Enabled = True
            Me.ProcessStatusStrip.Refresh()
            Me.MQFViewGrid.DataSource = _datamodel
            Me.MQFViewGrid.BestFitColumns()

            Me.MQFViewGrid.Enabled = True
            Me.MQFViewGrid.Visible = True

            Me.ProgressPictureBox.Visible = False

            Me.Refresh()
            ' Add any initialization after the InitializeComponent() call.
            Dim ahighest As Commons.StatusItem = _MQFObject.GetHighestStatusItem

            If Not _MQFObject.Processable Then
                Me.ProcessStatusLabel.Text = "for proceeding press preprocess button to check the data again"

                Me.PreProcessButton.Visible = True
                Me.PreProcessButton.Enabled = True
                Me.PreProcessButton.ButtonElement.ShowBorder = True

                Me.ProcessButton.ButtonElement.ToolTipText = "preprocess and check the data again"

                Me.ProcessButton.Visible = True
                Me.ProcessButton.Enabled = False
                Me.ProcessButton.ButtonElement.ShowBorder = False
                Me.ProcessButton.ButtonElement.ToolTipText = "Process and Feed not available due to preprocess errors"
                Me.MQFWizard.NextButton.Enabled = False
            Else
                Me.ProcessStatusLabel.Text = "press process button to feed data to the database"

                Me.PreProcessButton.Visible = True
                Me.PreProcessButton.Enabled = False
                Me.PreProcessButton.ButtonElement.ShowBorder = False
                Me.ProcessButton.ButtonElement.ToolTipText = "data preprocessed"

                Me.ProcessButton.Visible = True
                Me.ProcessButton.Enabled = True
                Me.ProcessButton.ButtonElement.ShowBorder = True
                Me.ProcessButton.ButtonElement.ToolTipText = "Process and Feed the data to the database"
                Me.WizardPage1.Title = "Process " & _MQFObject.ID
                Me.WizardPage1.Header = "select the process button to feed the messages to the database"
                Me.MQFWizard.NextButton.Enabled = False
            End If

        End If
    End Sub
    ''' <summary>
    ''' Progress EventHandler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ProgressOfWorker(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles _BuildMQFWorker.ProgressChanged, _ProcessWorker.ProgressChanged, _PreProcessWorker.ProgressChanged

        Dim perc As Integer
        Dim text As String

        perc = e.ProgressPercentage
        If Not e.UserState Is Nothing Then
            text = CType(e.UserState, String)
        End If

        Call SetProgress(perc, text)

    End Sub
    ''' <summary>
    '''  End of the Preprocess Eventhandler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub EndOfBuildWorker(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles _BuildMQFWorker.RunWorkerCompleted
        Try
            If TypeOf (e.Result) Is Boolean AndAlso CType(e.Result, Boolean) = True Then
                Me.ProcessStatusLabel.Text = "message queue import succeeded"
            ElseIf TypeOf (e.Result) Is Boolean AndAlso CType(e.Result, Boolean) = False Then
                Me.ProcessStatusLabel.Text = "message queue import failed"
            End If
        Catch ex As Exception
            Diagnostics.Debug.WriteLine("{0} \n {1}", ex.Message, ex.StackTrace)
        End Try


        Call SetAfterBuildModelWork()

    End Sub
    ''' <summary>
    '''  End of the Preprocess Eventhandler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub EndOfPreCHeckWorker(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles _PreProcessWorker.RunWorkerCompleted

        If TypeOf (e.Result) Is Boolean AndAlso CType(e.Result, Boolean) = True Then
            Me.ProcessStatusLabel.Text = "preprocess run succeeded"
        ElseIf TypeOf (e.Result) Is Boolean AndAlso CType(e.Result, Boolean) = False Then
            Me.ProcessStatusLabel.Text = "preprocess run failed"
        End If

        Call SetAfterPreProcessWork()

    End Sub
    ''' <summary>
    ''' Run Preprocess
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RunBuild(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles _BuildMQFWorker.DoWork

        e.Result = modXLSMessageQueueFile.BuildMessageQueueObject(_MQFWorkbook, _MQFObject, _BuildMQFWorker)

    End Sub
    ''' <summary>
    ''' Run Precheck
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RunPreProcess(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles _PreProcessWorker.DoWork
        e.Result = _MQFObject.Precheck(_PreProcessWorker)
    End Sub
#End Region

#Region "Process"
    ''' <summary>
    ''' load the processing Wizard page
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Process_load()
        '** preprocess
        Me.ProcessStatusStrip.Refresh()

        Me.WizardPage1.Enabled = False
        ' Me.PreProcessButton.Enabled = False

        'Me.Enabled = False
        'Me.Cursor = Windows.Forms.Cursors.WaitCursor

        ' disabled next
        Me.MQFWizard.CommandArea.NextButton.Enabled = False
        'Me.Panel4.Controls.Add(ProcessCommandPanel)
        Me.PreProcessButton.Visible = False
        'Me.Panel4.Controls.Add(Me.MQFViewGrid)
        Me.MQFViewGrid.Enabled = False
        Me.MQFViewGrid.Visible = False
        'Me.Panel4.Controls.Add(Me.ProgressPictureBox)
        Me.ProgressPictureBox.Visible = True

        Me.WizardPage1.Title = "Process " & _MQFObject.ID
        Me.WizardPage1.Header = "select the process button to feed the messages to the database"

        Me.Refresh()


    End Sub

    ''' <summary>
    ''' handles the PreProcess Button Click
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ProcessButton_Click(sender As Object, e As EventArgs) Handles ProcessButton.Click


        '** preprocess
        Me.ProcessStatusLabel.Text = "starting processing the data ..."
        Me.ProcessStatusStrip.Refresh()

        'Me.WizardPage1.Enabled = False
        Me.PreProcessButton.Enabled = False
        Me.ProcessButton.Enabled = False

        'Me.Enabled = False
        Me.Cursor = Windows.Forms.Cursors.WaitCursor

        '** preprocess
        ' disabled next
        Me.MQFWizard.CommandArea.NextButton.Enabled = False

        Me.MQFViewGrid.Enabled = False
        Me.MQFViewGrid.Visible = False
        Me.ProgressPictureBox.Visible = True

        Me.Refresh()

        Me._ProcessWorker.WorkerReportsProgress = True
        ' run
        _ProcessWorker.RunWorkerAsync()

    End Sub

    ''' <summary>
    ''' AfterWork (Preprocess) CleanUp
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetAfterProcessWork()

        ' InvokeRequired required compares the thread ID of the 
        ' calling thread to the thread ID of the creating thread. 
        ' If these threads are different, it returns true. 
        If Me.ProcessStatusStrip.InvokeRequired Then
            Dim d As New SetAfterWorkCallback(AddressOf SetAfterProcessWork)
            Me.Invoke(d, New Object() {})
        Else
            Me.Cursor = Windows.Forms.Cursors.Default
            Me.PreProcessButton.Enabled = True
            Me.WizardPage1.Enabled = True
            'Me.Enabled = True
            Me.ProcessStatusStrip.Refresh()

            Me.MQFViewGrid.Enabled = True
            Me.MQFViewGrid.Visible = True

            Me.ProgressPictureBox.Visible = False


            ' Add any initialization after the InitializeComponent() call.
            Dim ahighest As Commons.StatusItem = _MQFObject.GetHighestStatusItem

            If _MQFObject.Processed AndAlso ahighest IsNot Nothing AndAlso Not ahighest.Code Like "G*" Then
                Me.ProcessStatusLabel.Text = "data processed with errors or warnings"

                Me.PreProcessButton.Visible = True
                Me.PreProcessButton.Enabled = False
                Me.PreProcessButton.ButtonElement.ShowBorder = False
                Me.ProcessButton.ButtonElement.ToolTipText = "data preprocessed"

                Me.ProcessButton.Visible = True
                Me.ProcessButton.Enabled = True
                Me.ProcessButton.ButtonElement.ShowBorder = True
                Me.ProcessButton.ButtonElement.ToolTipText = "process again"
                Me.MQFWizard.CommandArea.NextButton.Enabled = True

            ElseIf _MQFObject.Processed Then
                Me.ProcessStatusLabel.Text = "data processed with no warnings nor errors"

                Me.PreProcessButton.Visible = True
                Me.PreProcessButton.Enabled = False
                Me.PreProcessButton.ButtonElement.ShowBorder = False
                Me.ProcessButton.ButtonElement.ToolTipText = "data preprocessed"

                Me.ProcessButton.Visible = True
                Me.ProcessButton.Enabled = False
                Me.ProcessButton.ButtonElement.ShowBorder = False
                Me.ProcessButton.ButtonElement.ToolTipText = "processed"
                Me.MQFWizard.CommandArea.NextButton.Enabled = True
            Else
                Me.ProcessStatusLabel.Text = "data processed with no warnings nor errors"

                Me.PreProcessButton.Visible = True
                Me.PreProcessButton.Enabled = False
                Me.ProcessButton.ButtonElement.ShowBorder = False
                Me.PreProcessButton.ButtonElement.ToolTipText = "data preprocessed"

                Me.ProcessButton.Visible = True
                Me.ProcessButton.Enabled = True
                Me.ProcessButton.ButtonElement.ShowBorder = True
                Me.ProcessButton.ButtonElement.ToolTipText = "process the data again"
                Me.MQFWizard.CommandArea.NextButton.Enabled = False
            End If


            Me.Refresh()
        End If
    End Sub
    ''' <summary>
    ''' Run Precheck
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RunProcessCheck(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles _ProcessWorker.DoWork
        e.Result = _MQFObject.Process(_ProcessWorker)
    End Sub
    ''' <summary>
    '''  End of the Preprocess Eventhandler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub EndOfProcessWorker(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles _ProcessWorker.RunWorkerCompleted

        If TypeOf (e.Result) Is Boolean AndAlso CType(e.Result, Boolean) = True Then
            Me.ProcessStatusLabel.Text = "process run succeeded"
        ElseIf TypeOf (e.Result) Is Boolean AndAlso CType(e.Result, Boolean) = False Then
            Me.ProcessStatusLabel.Text = "process run failed"
        End If

        Call SetAfterProcessWork()

    End Sub
#End Region

#Region "UpdateExcel"
    ''' <summary>
    ''' load the preprocessing Wizard page
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateXLS_load()

        Me.UpdateXLSPanel.Controls.Add(Me.ProcessStatusStrip)
        Me.UpdateXLSPanel.Controls.Add(Me.ProgressPictureBox)

        Me.ProcessStatusLabel.Text = "about to update excel message queue file ..."
        Me.ProcessStatusStrip.Refresh()

        Me.WizardPage1.Title = "Updating Workbook :" & _MQFWorkbook.Name
        Me.WizardPage1.Header = "press the button to run the update to the selected workbook"

        Me.UpdateXLSButton.ButtonElement.ToolTipText = "Update the excel workbook with processing results"
        Me.MQFWizard.CommandArea.NextButton.Enabled = False

        '' load the fields
        Dim foundflag As Boolean
        Dim aValue As Object
        aValue = GetXlsParameterByName(name:="hermes_mqf_createdby", workbook:=_MQFWorkbook, silent:=True, found:=foundflag)
        If foundflag AndAlso aValue.ToString <> "" Then
            Me.XLSCreatedBy.Text = CStr(aValue)
        Else
            Me.XLSCreatedBy.Text = CurrentSession.OTdbUser.PersonName
        End If
        aValue = GetXlsParameterByName(name:="hermes_mqf_createdby_department", workbook:=_MQFWorkbook, silent:=True, found:=foundflag)
        If foundflag Then
            Me.XLSCreatedByDepartment.Text = CStr(aValue)
        Else
            Me.XLSCreatedByDepartment.Text = ""
        End If
        aValue = GetXlsParameterByName(name:="hermes_mqf_createdon", workbook:=_MQFWorkbook, silent:=True, found:=foundflag)
        If foundflag And IsDate(aValue) Then
            Me.XLSCreatedOn.Value = CDate(aValue)
        Else
            Me.XLSCreatedOn.Value = Date.Now
        End If

        aValue = GetXlsParameterByName(name:="hermes_mqf_requestedby", workbook:=_MQFWorkbook, silent:=True, found:=foundflag)
        If foundflag AndAlso aValue.ToString <> "" Then
            Me.XLSRequestedBy.Text = CStr(aValue)
        Else
            Me.XLSRequestedBy.Text = CurrentSession.OTdbUser.PersonName
        End If
        aValue = GetXlsParameterByName(name:="hermes_mqf_requestedby_department", workbook:=_MQFWorkbook, silent:=True, found:=foundflag)
        If foundflag Then
            Me.XLSRequestedByDepartment.Text = CStr(aValue)
        Else
            Me.XLSRequestedByDepartment.Text = ""
        End If
        aValue = GetXlsParameterByName(name:="hermes_mqf_requestedon", workbook:=_MQFWorkbook, silent:=True, found:=foundflag)
        If foundflag And IsDate(aValue) Then
            Me.XLSRequestedOn.Value = CDate(aValue)
        Else
            Me.XLSRequestedOn.Value = Date.Now
        End If

        aValue = GetXlsParameterByName(name:="hermes_mqf_title", workbook:=_MQFWorkbook, silent:=True, found:=foundflag)
        If foundflag AndAlso aValue.ToString <> "" Then
            Me.XlsTitel.Text = CStr(aValue)
        Else
            Me.XlsTitel.Text = "Update"
        End If
        aValue = GetXlsParameterByName(name:="hermes_mqf_subject", workbook:=_MQFWorkbook, silent:=True, found:=foundflag)
        If foundflag AndAlso aValue.ToString <> "" Then
            Me.XLSRequestFor.Text = CStr(aValue)
        Else
            Me.XLSRequestFor.Text = ""
        End If
        aValue = GetXlsParameterByName(name:="hermes_mqf_plan_revision", workbook:=_MQFWorkbook, silent:=True, found:=foundflag)
        If foundflag AndAlso aValue.ToString <> "" Then
            Me.XLSPlanRevision.Text = CStr(aValue)
        Else
            Me.XLSPlanRevision.Text = ""
        End If

        'aValue = GetXlsParameterByName(name:="hermes_mqf_approvedBy", workbook:=_MQFWorkbook, silent:=True, found:=foundflag)
        'If foundflag AndAlso aValue.ToString <> "" Then
        '    Me.XLSRequestedBy.Text = CStr(aValue)
        'Else
        Me.XLSApprovedBy.Text = CurrentSession.OTdbUser.PersonName
        'End If
        'aValue = GetXlsParameterByName(name:="hermes_mqf_processedBy", workbook:=_MQFWorkbook, silent:=True, found:=foundflag)
        'If foundflag AndAlso aValue.ToString <> "" Then
        '    Me.XLSRequestedBy.Text = CStr(aValue)
        'Else
        Me.XLSProcessedBy.Text = CurrentSession.OTdbUser.PersonName
        'End If
        'aValue = GetXlsParameterByName(name:="hermes_mqf_processedOn", workbook:=_MQFWorkbook, silent:=True, found:=foundflag)
        'If foundflag And IsDate(aValue) Then
        '    Me.XLSRequestedOn.Value = CDate(aValue)
        'Else
        If _MQFObject.Processdate.HasValue Then
            Me.XLSProcessedDate.Value = CDate(_MQFObject.Processdate)
        Else
            Me.XLSProcessedDate.Value = Date.Now
        End If
        If _MQFObject.ProcessStatusCode IsNot Nothing Then
            Me.XLSProcessStatus.Text = _MQFObject.ProcessStatusCode
            Me.XLSProcessStatus.TextAlign = Windows.Forms.HorizontalAlignment.Center
            Me.XLSProcessStatus.BackColor = System.Drawing.Color.FromArgb(CInt(_MQFObject.ProcessStatus.FormatBGColor))
            Me.XLSProcessStatus.ForeColor = System.Drawing.Color.FromArgb(CInt(_MQFObject.ProcessStatus.FormatFGColor))
        End If

        'End If
        Me.Refresh()

    End Sub

    ''' <summary>
    ''' handles the PreProcess Button Click
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub UpdateXlsButton_Click(sender As Object, e As EventArgs) Handles UpdateXLSButton.Click

        Me.ProcessStatusLabel.Text = "updating excel message queue file ..."
        Me.ProcessStatusStrip.Refresh()
        'Me.Enabled = False
        Me.Cursor = Windows.Forms.Cursors.WaitCursor

        ' disabled next
        Me.MQFWizard.CommandArea.NextButton.Enabled = False
        Me.ProgressPictureBox.Visible = True

        Me.Refresh()

        ' update data
        With _MQFObject
            .CreationDate = XLSCreatedOn.Value
            .CreatingOU = XLSCreatedByDepartment.Text
            .Creator = XLSCreatedBy.Text
            .RequestedOn = XLSRequestedOn.Value
            .RequestedBy = XLSRequestedBy.Text
            .RequestedByOU = XLSRequestedByDepartment.Text
            .Title = XlsTitel.Text
            .Comment = XLSRequestFor.Text
            .Planrevision = XLSPlanRevision.Text
            .ApprovedBy = XLSApprovedBy.Text
            .ApprovalDate = Date.Now
        End With

        '
        Me._UpdateXLSWorker.WorkerReportsProgress = True
        ' run
        _UpdateXLSWorker.RunWorkerAsync()

    End Sub

    ''' <summary>
    ''' AfterWork (Preprocess) CleanUp
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetAfterUpdateXLSWork()

        ' InvokeRequired required compares the thread ID of the 
        ' calling thread to the thread ID of the creating thread. 
        ' If these threads are different, it returns true. 
        If Me.ProcessStatusStrip.InvokeRequired Then
            Dim d As New SetAfterWorkCallback(AddressOf SetAfterUpdateXLSWork)
            Me.Invoke(d, New Object() {})
        Else
            Me.Cursor = Windows.Forms.Cursors.Default
            Me.UpdateXLSButton.Enabled = False
            Me.WizardPage1.Enabled = True
            Me.ProcessStatusStrip.Refresh()

            Me.ProgressPictureBox.Visible = False
            Me.MQFWizard.CommandArea.NextButton.Enabled = True


            Me.Refresh()
        End If
    End Sub
    ''' <summary>
    ''' Run Precheck
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RunUpdateXLSWorker(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles _UpdateXLSWorker.DoWork
        e.Result = PostProcessXLSMQF(_MQFWorkbook, _MQFObject, workerthread:=_UpdateXLSWorker)
    End Sub


    ''' <summary>
    '''  End of the Preprocess Eventhandler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub EndOfUpdateXLSWorker(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles _UpdateXLSWorker.RunWorkerCompleted

        If TypeOf (e.Result) Is Boolean AndAlso CType(e.Result, Boolean) = True Then
            Me.ProcessStatusLabel.Text = "update run succeeded"
        ElseIf TypeOf (e.Result) Is Boolean AndAlso CType(e.Result, Boolean) = False Then
            Me.ProcessStatusLabel.Text = "update run failed"
        End If

        Call SetAfterUpdateXLSWork()

    End Sub
#End Region
    ''' <summary>
    ''' Handler for DataBinding Complete to add the MQF to each element
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub UIWizardMQFFeed_Event(sender As Object, e As GridViewBindingCompleteEventArgs) Handles MQFViewGrid.DataBindingComplete
        Dim myself As RadGridView = TryCast(sender, RadGridView)

        If myself IsNot Nothing Then
            ''' tag the messages
            For Each aRow As GridViewRowInfo In myself.Rows
                Dim aMessage As MQMessage
                If CType(aRow.Cells(UIMQFDataModel.ConstFNRowType).Value, UIMQFDataModel.internalRowtype) = UIMQFDataModel.internalRowtype.MQMEssage Then
                    Dim aIDNO As Long = CType(aRow.Cells(UIMQFDataModel.ConstFNMessageID).Value, Long)
                    aMessage = _MQFObject.Messages.Item(aIDNO)
                    aRow.Tag = aMessage
                End If
            Next
            ''' hide
            ''' 
            myself.Columns(UIMQFDataModel.ConstFNRowType).IsVisible = False
            myself.Columns(UIMQFDataModel.ConstFNTupleID).IsVisible = False

        End If
    End Sub
    ''' <summary>
    ''' Event Handler for GridView RowFormatting
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub UIWizardMQFFeed_OnGridViewRowFormatEvent(sender As Object, e As RowFormattingEventArgs) Handles MQFViewGrid.RowFormatting

        ''' format the MQMessage Row
        ''' 
        If e.RowElement.RowInfo.Tag IsNot Nothing AndAlso e.RowElement.RowInfo.Tag.GetType Is GetType(MQMessage) Then
            Dim aMQMessage As MQMessage = TryCast(e.RowElement.RowInfo.Tag, MQMessage)
            If aMQMessage.Action Is Nothing OrElse aMQMessage.Action = "" Then
                e.RowElement.ForeColor = System.Drawing.Color.LightGray
                e.RowElement.BackColor = System.Drawing.Color.OldLace
                e.RowElement.GradientStyle = GradientStyles.Solid
                e.RowElement.DrawFill = True
            ElseIf aMQMessage.Action.ToUpper = constMQFOperation_NOOP.ToUpper Then
                e.RowElement.ForeColor = System.Drawing.Color.Gray
                e.RowElement.BackColor = System.Drawing.Color.MintCream
                e.RowElement.GradientStyle = GradientStyles.Solid
                'e.RowElement.BackColor = System.Drawing.Color.LightSteelBlue
                e.RowElement.DrawFill = True

            ElseIf aMQMessage.PrecheckedOn IsNot Nothing And aMQMessage.ProcessedOn Is Nothing Then
                If aMQMessage.Processable Then
                    e.RowElement.ForeColor = System.Drawing.Color.DarkSlateBlue
                    e.RowElement.BackColor = System.Drawing.Color.PaleGreen
                    e.RowElement.DrawFill = True
                Else
                    e.RowElement.ForeColor = System.Drawing.Color.White
                    e.RowElement.BackColor = System.Drawing.Color.LightCoral
                    e.RowElement.DrawFill = True
                End If
            ElseIf aMQMessage.ProcessedOn IsNot Nothing Then
                If aMQMessage.Processed Then
                    e.RowElement.ForeColor = System.Drawing.Color.OldLace
                    e.RowElement.BackColor = System.Drawing.Color.Green
                    e.RowElement.DrawFill = True
                Else
                    e.RowElement.ForeColor = System.Drawing.Color.OldLace
                    e.RowElement.BackColor = System.Drawing.Color.DarkRed
                    e.RowElement.DrawFill = True
                End If
            Else

                'e.RowElement.ResetValue(LightVisualElement.BackColorProperty, ValueResetFlags.Local)
                'e.RowElement.ResetValue(LightVisualElement.ForeColorProperty, ValueResetFlags.Local)
                'e.RowElement.DrawFill = False

            End If
        End If
    End Sub
    ''' <summary>
    ''' Event Handler for GridView RowFormatting
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub UIWizardMQFFeed_OnGridViewCellFormatEvent(sender As Object, e As CellFormattingEventArgs) Handles MQFViewGrid.CellFormatting


        ''' format the MQMessage Row
        ''' 
        If e.Row.Tag IsNot Nothing AndAlso e.Row.Tag.GetType Is GetType(MQMessage) Then
            Dim aMQMessage As MQMessage = TryCast(e.Row.Tag, MQMessage)
            Select Case e.Column.FieldName.ToUpper
                Case UIMQFDataModel.ConstFNMQFOperation.ToUpper, UIMQFDataModel.ConstFNMQFTimestamp.ToUpper, UIMQFDataModel.ConstFNMessageID.ToUpper, UIMQFDataModel.ConstFNTupleID.ToUpper
                    'If aMQMessage.PrecheckedOn IsNot Nothing AndAlso aMQMessage.Processable Then
                    '    e.CellElement.BackColor = System.Drawing.Color.Green
                    '    e.CellElement.ForeColor = System.Drawing.Color.NavajoWhite
                    '    e.CellElement.GradientStyle = GradientStyles.Solid
                    '    e.CellElement.DrawFill = True
                    'End If
                Case UIMQFDataModel.ConstFNMQFStatus.ToUpper
                    Dim aStatus As Commons.StatusItem = aMQMessage.Statusitem
                    If aMQMessage.PrecheckedOn IsNot Nothing AndAlso aStatus IsNot Nothing Then
                        'e.CellElement.BackColor = System.Drawing.Color.FromArgb(CInt(aStatus.FormatBGColor))
                        'e.CellElement.ForeColor = System.Drawing.Color.FromArgb(CInt(aStatus.FormatFGColor))
                        'e.CellElement.GradientStyle = GradientStyles.Solid
                        'e.CellElement.DrawFill = True
                    End If

                Case Else
                    'e.CellElement.ResetValue(LightVisualElement.BackColorProperty, ValueResetFlags.Local)
                    'e.CellElement.ResetValue(LightVisualElement.ForeColorProperty, ValueResetFlags.Local)
                    'e.CellElement.ResetValue(LightVisualElement.GradientStyleProperty, ValueResetFlags.Local)
                    'e.CellElement.DrawFill = False
            End Select

        End If
    End Sub
    '*****
    '***** Cancel / next / previous of the Wizard
    '*****
#Region "Wizard Global Controls"

    '*** setStatus thread safe
    '***
    Private Sub SetStatus(ByVal [text] As String)

        ' InvokeRequired required compares the thread ID of the 
        ' calling thread to the thread ID of the creating thread. 
        ' If these threads are different, it returns true. 

        If Me.MQFWizard.SelectedPage Is Me.MQFWizard.Pages(0) Then
            If Me.WelcomeStatusStrip.InvokeRequired Then
                Dim d As New SetStatusCallback(AddressOf SetStatus)
                Me.Invoke(d, New Object() {[text]})
            Else
                Me.WelcomeStatusLabel.Text = [text]
                Me.WelcomeStatusStrip.Refresh()
            End If
        ElseIf Me.MQFWizard.SelectedPage Is Me.MQFWizard.Pages(1) Then
            If Me.ProcessStatusStrip.InvokeRequired Then
                Dim d As New SetStatusCallback(AddressOf SetStatus)
                Me.Invoke(d, New Object() {[text]})
            Else
                Me.ProcessStatusLabel.Text = [text]
                Me.ProcessStatusStrip.Refresh()
            End If
        End If
    End Sub
    '**** any error will be shown in the status Label
    '****
    Private Sub OTDBERROR_raiseError(ByVal sender As Object, ByVal e As ormErrorEventArgs) Handles _errorlog.onErrorRaised
        Call Me.SetStatus(e.Error.Message)
    End Sub
    '***
    '*** cancel Button
    Private Sub MQFWizard_Cancel(ByVal sender As Object, ByVal e As EventArgs) Handles MQFWizard.Cancel, Me.FormClosing
        RadMessageBox.SetThemeName(Me.MQFWizard.ThemeName)
        Dim ds As Windows.Forms.DialogResult = _
            RadMessageBox.Show(Me, "Are you sure?", "Cancel", Windows.Forms.MessageBoxButtons.YesNo, RadMessageIcon.Question)
        Me.Text = ds.ToString()
        If ds = Windows.Forms.DialogResult.Yes Then

            Me.Dispose()
        Else
            Dim formClosingArgs As System.Windows.Forms.FormClosingEventArgs = TryCast(e, System.Windows.Forms.FormClosingEventArgs)
            If Not formClosingArgs Is Nothing Then
                formClosingArgs.Cancel = True
            End If

        End If

    End Sub
    '**** 
    '**** Next 
    ''' <summary>
    ''' Handles the next Event in the Wizard
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MQFWizard_OnNext(ByVal sender As Object, ByVal e As WizardCancelEventArgs) Handles MQFWizard.Next
        ''' current page is Startpage
        If (Me.MQFWizard.SelectedPage Is Me.MQFWizard.Pages(0)) Then
            Call Preprocess_load()

            ''' current page is preprocess page #1
        ElseIf (Me.MQFWizard.SelectedPage Is Me.MQFWizard.Pages(1)) Then
            Call UpdateXLS_load()

            ''' page is write back to excel 
        ElseIf (Me.MQFWizard.SelectedPage Is Me.MQFWizard.Pages(2)) Then
            'e.Cancel = True
            'Me.MQFWizard.SelectedPage = Me.MQFWizard.Pages(1)
            Me.MQFWizard.FinishButton.Enabled = True
        End If
    End Sub
    '****
    '**** Previous
    Private Sub MQFWizard_OnPrevious(ByVal sender As Object, ByVal e As WizardCancelEventArgs) Handles MQFWizard.Previous

    End Sub

#End Region

   
End Class
