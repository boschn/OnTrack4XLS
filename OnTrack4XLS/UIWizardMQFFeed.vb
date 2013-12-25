
Imports System.ComponentModel
Imports Telerik.WinControls
Imports Telerik.WinControls.UI
Imports System.Data

Imports OnTrack
Imports OnTrack.XChange

'*********************
'********************* Code-Behind the MQFFeedWizard
Public Class UIWizardMQFFeed

    Private _MQFWorkbookName As String = ""
    Private _MQFObject As New clsOTDBMessageQueue
    Private _MQFWorkbook As Excel.Workbook
    Private WithEvents _errorlog As ErrorLog 'get reference for event handling of new errors
    Private WithEvents _preprocessWorker As BackgroundWorker

    Friend WithEvents MQFDataSet As System.Data.DataSet
    Friend WithEvents _datamodel As UIMQFDataModel

    Private Delegate Sub SetProgressCallback([percentage] As Integer, [text] As String)
    Private Delegate Sub SetStatusCallback([text] As String)
    Private Delegate Sub SetAfterWorkCallback()

    Public Sub New()
        ' attach to the errorlog
        _errorlog = ot.Errorlog
        _preprocessWorker = New BackgroundWorker
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.RadWizard.CommandArea.NextButton.Enabled = False

        ' load the current workbooks to the Listbox
        For Each aWb As Excel.Workbook In Globals.ThisAddIn.Application.Workbooks
            Dim aDataItem As New RadListDataItem

            If modMQF.checkWorkbookIfMQF(aWb) Then
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
        Me.PreProcessRadViewGrid.MasterTemplate.DataSource = Me.MQFDataSet
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
        Dim lastError As CoreError

        If Not isConnected Then
            Dim aCon As Database.ormConnection = ot.CurrentConnection(otAccessRight.ReadUpdateData)
            If aCon Is Nothing Then
                lastError = GetLastError()
                If Not lastError Is Nothing Then
                    Me.WelcomeStatusLabel.Text = lastError.Message
                Else
                    Me.WelcomeStatusLabel.Text = "No Connection to the OnTrack Database available"
                End If
                Exit Sub ' Exit here if no connection available
            End If
        End If

        '** locate a new MQF
        _MQFWorkbook = modMQF.LocateAndOpenMQF()

        If _MQFWorkbook Is Nothing Then
            lastError = GetLastError()
            If Not lastError Is Nothing Then
                Me.WelcomeStatusLabel.Text = lastError.Message
            Else
                Me.WelcomeStatusLabel.Text = "Workbook couldnot be openend"
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

    '**** selection 
    '****
    Private Sub Workbooklist_ValueChanged(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) _
        Handles WorkbookList.SelectedValueChanged, WorkbookList.SelectedIndexChanged
        Dim item As RadListDataItem = TryCast(Me.WorkbookList.SelectedItem, RadListDataItem)
        If Not item Is Nothing Then
            _MQFWorkbookName = item.Text
            _MQFWorkbook = Globals.ThisAddIn.Application.Workbooks(_MQFWorkbookName)
            Me.RadWizard.CommandArea.NextButton.Enabled = True
            Me.MQFSelectedTextbox.Text = item.Text
            Me.WizardPage1.Title = "Preprocess " & item.Text
            Me.WizardPage2.Title = "Process " & item.Text

        End If
    End Sub

#End Region
    '**** 
    '**** Preprocess -> Run through Preprocessing of MQF
#Region "Preprocess"
    Private Sub Preprocess_load()

    End Sub


    '**** Preprocess-Event
    '****
    Private Sub PreProcessButton_Click(sender As Object, e As EventArgs) Handles PreProcessButton.Click


        '** preprocess
        Me.PreprocessStatusLabel.Text = "starting preprocess run ..."
        Me.PreprocessStatusStrip.Refresh()

        'Me.WizardPage1.Enabled = False
        Me.PreProcessButton.Enabled = False
        'Me.Enabled = False
        Me.Cursor = Windows.Forms.Cursors.WaitCursor
        Me._preprocessWorker.WorkerReportsProgress = True
        ' run
        _preprocessWorker.RunWorkerAsync()


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
        If Me.PreprocessStatusStrip.InvokeRequired Then
            Dim d As New SetProgressCallback(AddressOf SetProgress)
            Me.Invoke(d, New Object() {[percentage], [text]})
        Else
            Me.PreprocessProgressBar.Value1 = [percentage]
            Me.PreprocessStatusLabel.Text = [text]
            Me.PreprocessStatusStrip.Refresh()
        End If
    End Sub

    ''' <summary>
    ''' AfterWork (Preprocess) CleanUp
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetAfterWork()

        ' InvokeRequired required compares the thread ID of the 
        ' calling thread to the thread ID of the creating thread. 
        ' If these threads are different, it returns true. 
        If Me.PreprocessStatusStrip.InvokeRequired Then
            Dim d As New SetAfterWorkCallback(AddressOf SetAfterWork)
            Me.Invoke(d, New Object() {})
        Else
            Me.Cursor = Windows.Forms.Cursors.Default
            Me.PreProcessButton.Enabled = True
            'Me.WizardPage1.Enabled = True
            'Me.Enabled = True
            Me.PreprocessStatusStrip.Refresh()
        End If
    End Sub
    ''' <summary>
    ''' Progress EventHandler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ProgressofPreprocess(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles _preprocessWorker.ProgressChanged

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
    Private Sub EndOfPreprocess(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) _
        Handles _preprocessWorker.RunWorkerCompleted

        If TypeOf (e.Result) Is Boolean AndAlso e.Result = True Then
            Me.PreprocessStatusLabel.Text = "preprocess finished sucessfully"
        ElseIf TypeOf (e.Result) Is Boolean AndAlso e.Result = False Then
            Me.PreprocessStatusLabel.Text = "preprocess finished with no success"
        End If

        Call SetAfterWork()

    End Sub

    ''' <summary>
    ''' Run Preprocess
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RunPreprocess(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles _preprocessWorker.DoWork

        e.Result = modMQF.preProcessXLSMQF(_MQFWorkbook, _MQFObject, _preprocessWorker)

    End Sub

#End Region
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

        If Me.RadWizard.SelectedPage Is Me.RadWizard.Pages(0) Then
            If Me.WelcomeStatusStrip.InvokeRequired Then
                Dim d As New SetStatusCallback(AddressOf SetStatus)
                Me.Invoke(d, New Object() {[text]})
            Else
                Me.WelcomeStatusLabel.Text = [text]
                Me.WelcomeStatusStrip.Refresh()
            End If
        ElseIf Me.RadWizard.SelectedPage Is Me.RadWizard.Pages(1) Then
            If Me.PreprocessStatusStrip.InvokeRequired Then
                Dim d As New SetStatusCallback(AddressOf SetStatus)
                Me.Invoke(d, New Object() {[text]})
            Else
                Me.PreprocessStatusLabel.Text = [text]
                Me.PreprocessStatusStrip.Refresh()
            End If
        End If
    End Sub
    '**** any error will be shown in the status Label
    '****
    Private Sub OTDBERROR_raiseError(ByVal sender As Object, ByVal e As otErrorEventArgs) Handles _errorlog.onErrorRaised
        Call Me.SetStatus(e.Error.Message)
    End Sub
    '***
    '*** cancel Button
    Private Sub RadWizard_Cancel(ByVal sender As Object, ByVal e As EventArgs) Handles RadWizard.Cancel, Me.FormClosing
        RadMessageBox.SetThemeName(Me.RadWizard.ThemeName)
        Dim ds As Windows.Forms.DialogResult = _
            RadMessageBox.Show(Me, "Are you sure?", "Cancel", Windows.Forms.MessageBoxButtons.YesNo, RadMessageIcon.Question)
        Me.Text = ds.ToString()
        If ds = Windows.Forms.DialogResult.Yes Then
            Me.Dispose()
        Else
            Dim formClosingArgs As System.Windows.Forms.FormClosingEventArgs = TryCast(e, System.Windows.Forms.FormClosingEventArgs)
            If Not FormClosingArgs Is Nothing Then
                formClosingArgs.Cancel = True
            End If

        End If

    End Sub
    '**** 
    '**** Next 
    Private Sub radwizard_next(ByVal sender As Object, ByVal e As WizardCancelEventArgs) Handles RadWizard.Next
        If (Me.RadWizard.SelectedPage Is Me.RadWizard.Pages(0)) Then
            Call Preprocess_load()


        ElseIf (Me.RadWizard.SelectedPage Is Me.RadWizard.Pages(1)) Then

            e.Cancel = True
            Me.RadWizard.SelectedPage = Me.RadWizard.Pages(0)
        End If
    End Sub
    '****
    '**** Previous
    Private Sub radwizard_previous(ByVal sender As Object, ByVal e As WizardCancelEventArgs) Handles RadWizard.Previous

    End Sub

#End Region


    Private Sub Workbooklist_ValueChanged(sender As Object, e As EventArgs) Handles WorkbookList.SelectedValueChanged, WorkbookList.SelectedIndexChanged

    End Sub

    Private Sub PreProcessRadViewGrid_Click(sender As Object, e As EventArgs) Handles PreProcessRadViewGrid.Click

    End Sub
End Class
