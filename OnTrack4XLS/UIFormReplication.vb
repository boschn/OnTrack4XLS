Imports System.ComponentModel
Imports Telerik.WinControls
Imports Telerik.WinControls.UI
Imports System.Data

Imports OnTrack
Imports OnTrack.XChange
Imports OnTrack.Database
Imports OnTrack.Commons

Public Class UIFormReplication

    Private _workspaceList As List(Of Workspace)
    Private _workspaceTable As New DataTable
    Private _dataAreaList As List(Of XLSDataArea)
    Private _DataAreaTable As New DataTable
    Private _domainList As IList(Of Domain)
    Private _domainTable As New DataTable
    Private _workspaceID As String
    Private _domainID As String

    Private WithEvents _replicateWorker As BackgroundWorker
    Private Delegate Sub SetProgressCallback([percentage] As Integer, [text] As String)
    Private Delegate Sub SetStatusCallback([text] As String)
    Private Delegate Sub SetAfterWorkCallback()

    Private _dataarea As XLSDataArea

    Enum ReplicationMode
        Inbound
        Outbound
        InAndOut
    End Enum

    Enum ReplicationType
        Full
        Incremental
    End Enum

    Private _replicationType As ReplicationType

    Private WithEvents _errorlog As SessionMessageLog 'get reference for event handling of new errors


    Friend WithEvents MQFDataSet As System.Data.DataSet
    Friend WithEvents _datamodel As UIMQFDataModel

    Private _replicationMode As ReplicationMode
    Private _DefaultRangeAdresses As New List(Of String)
    Private _DefaultXConfigNames As New List(Of String)
    Private _DefaultHeaderIDs As New List(Of String)

    Public Sub New()
        ' attach to the errorlog
        _errorlog = ot.Errorlog
        _replicateWorker = New BackgroundWorker

        ' This call is required by the designer.
        InitializeComponent()

        ' set the defaultdomainid
        Dim foundflag As Boolean
        Dim value = GetXlsParameterByName(name:=constCPNDefaultDomainid, workbook:=Globals.ThisAddIn.Application.ActiveWorkbook, found:=foundflag, silent:=True)
        If foundflag AndAlso value IsNot Nothing Then
            _domainID = CStr(value)
        ElseIf Not String.IsNullOrWhiteSpace(Globals.ThisAddIn.CurrentDefaultDomainID) Then
            _domainID = Globals.ThisAddIn.CurrentDefaultDomainID
        Else
            _domainID = Nothing

        End If

        ' Add any initialization after the InitializeComponent() call.
        If Not ot.RequireAccess(accessRequest:=otAccessRight.[ReadOnly], domainID:=_domainID) Then
            Me.StatusLabel.Text = "no access to database"
            Return
        End If
        If String.IsNullOrWhiteSpace(_domainID) Then _domainID = CurrentSession.CurrentDomainID
        '** fill the domain List

        _domainList = Domain.All
        ' setup of the workspaceID table
        _domainTable.Columns.Add("DomainID", GetType(String))
        _domainTable.Columns.Add("Description", GetType(String))
        _domainTable.Columns.Add("is Global", GetType(Boolean))
        Dim i As Integer
        Dim found As Integer

        For Each aDomain As Domain In _domainList
            If Not aDomain.IsDeleted Then
                _domainTable.Rows.Add(Trim(aDomain.ID), aDomain.Description, aDomain.IsGlobal)
                If _domainID.ToUpper = aDomain.ID.ToUpper Then
                    found = i
                End If
                i += 1
            End If
        Next
        Me.DomainCombo.DataSource = _domainTable
        Me.DomainCombo.SelectedIndex = found

        '** fill the workspaceID List
        Me.WorkspaceDropDownList.Text = CurrentSession.CurrentWorkspaceID
        _workspaceList = Workspace.All
        ' setup of the workspaceID table
        _workspaceTable.Columns.Add("workspaceID", GetType(String))
        _workspaceTable.Columns.Add("Description", GetType(String))
        _workspaceTable.Columns.Add("is Base", GetType(Boolean))
        _workspaceTable.Columns.Add("has Actuals", GetType(Boolean))
        For Each aWorkspace As Workspace In _workspaceList
            If Not aWorkspace.IsDeleted Then
                _workspaceTable.Rows.Add(Trim(aWorkspace.ID), aWorkspace.Description, aWorkspace.IsBasespace, aWorkspace.HasActuals)
            End If
        Next
        Me.WorkspaceDropDownList.DataSource = _workspaceTable

        '** fill the workspaceID List

        'Dim aXConfig As New XChangeConfiguration
        'Dim aXConfigList As New List(Of XChangeConfiguration)
        'aXConfigList = aXConfig.AllByList
        ' setup of the workspaceID table
        XLSXChangeMgr.AttachWorkbook(Globals.ThisAddIn.Application.ActiveWorkbook)
        _DataAreaTable.Columns.Add("Name", GetType(String))
        _DataAreaTable.Columns.Add("XConfigName", GetType(String))
        _DataAreaTable.Columns.Add("Range", GetType(String))

        _dataAreaList = XLSXChangeMgr.getDataAreas(Globals.ThisAddIn.Application.ActiveWorkbook)
        If _dataAreaList.Count > 0 Then
            For Each aDataArea As XLSDataArea In _dataAreaList
                _DataAreaTable.Rows.Add(aDataArea.Name, aDataArea.XConfigName, aDataArea.DataRangeAddress)
            Next
            Me.DataAreaComboBox.DataSource = _DataAreaTable
            Me.DataAreaComboBox.Text = _DataAreaTable.Rows(0).Item(0)
        Else
            Me.DataAreaComboBox.Enabled = False
            Me.StatusLabel.Text = "no data areas found in workbook"
        End If

        'For Each aXConfig In aXConfigList
        'If Not aXConfig.IsDeleted Then
        '_DataAreaTable.Rows.Add(Trim(aXConfig.CONFIGNAME), aXConfig.description)
        'End If
        'Next
        'Me.ConfigDropDownList.DataSource = _DataAreaTable
        'Me.ConfigDropDownList.Text = _DataAreaTable.Rows(0).Item(0)
        'Set the Inbound
        Me.OutboundToggleButton.ToggleState = Enumerations.ToggleState.On

        Me.Refresh()


    End Sub

    '*****
    '***** On Load
    Protected Sub OnLoad(sender As Object, e As EventArgs) Handles Me.Load
        'MyBase.OnLoad(e)
        ' check which properties we have

    End Sub

    '******** Disposing
    '********
    Protected Overloads Sub Disposing(sender As Object, e As EventArgs) Handles Me.Disposed
        'If Not Me.RefEdit Is Nothing Then
        'Me.RefEdit.ExcelConnector = Nothing
        'Me.RefEdit.Dispose()
        'Me.RefEdit = Nothing
        'GC.Collect()
        'End If
    End Sub
    ''' <summary>
    ''' ReplicationMenuItem
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ReplicationMenuItem_Click(sender As Object, e As EventArgs) Handles FullReplicationMenuItem.Click, IncrementalReplicationMenuItem.Click
        If _dataarea Is Nothing Then
        Else
            Dim aMenuItem As RadMenuItem = DirectCast(sender, RadMenuItem)
            If aMenuItem.Tag = MySettings.Default.ReplicationForm_Full Then
                _replicationType = ReplicationType.Full
            Else
                _replicationType = ReplicationType.Incremental
            End If
            '** preprocess
            Me.StatusProgress.Text = "starting replication run ..."
            Me.StatusStrip.Refresh()
            Me.ReplicateButton.Enabled = False

            Me.DataAreaComboBox.Enabled = False
            Me.WorkspaceDropDownList.Enabled = False
            Me.DomainCombo.Enabled = False
            Me.ToggleInOutButton.Enabled = False
            Me.InboundToggleButton.Enabled = False
            Me.OutboundToggleButton.Enabled = False
            'Me.Enabled = False
            Me.Cursor = Windows.Forms.Cursors.WaitCursor

            _replicateWorker.WorkerReportsProgress = True
            _replicateWorker.WorkerSupportsCancellation = True
            ' run
            _replicateWorker.RunWorkerAsync()

        End If
    End Sub

    ''' <summary>
    ''' RunFullReplication
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RunReplication(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles _replicateWorker.DoWork

        Dim aXCMD As otXChangeCommandType = otXChangeCommandType.Read
        If Me._replicationMode = ReplicationMode.Outbound Then
            aXCMD = otXChangeCommandType.Read
        ElseIf Me._replicationMode = ReplicationMode.Inbound Then
            aXCMD = otXChangeCommandType.CreateUpdate
        ElseIf Me._replicationMode = ReplicationMode.InAndOut Then
            aXCMD = otXChangeCommandType.CreateUpdate
        End If

        Dim fullReplication As Boolean
        If _replicationType = ReplicationType.Full Then
            fullReplication = True
        Else
            fullReplication = False
        End If
        e.Result = XLSXChangeMgr.ReplicateDataArea(dataarea:=_dataarea, domainid:=_domainID, fullReplication:=fullReplication, xcmd:=aXCMD, _
                                           workspaceID:=Me.WorkspaceDropDownList.Text, workerthread:=_replicateWorker)

        CoreMessageHandler(message:="replication to " & _dataarea.DataRangeAddress & " completed", messagetype:=otCoreMessageType.ApplicationInfo)

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
        If Me.StatusStrip.InvokeRequired Then
            Dim d As New SetProgressCallback(AddressOf SetProgress)
            Me.Invoke(d, New Object() {[percentage], [text]})
        Else
            Me.StatusProgress.Value1 = [percentage]
            Me.StatusLabel.Text = [text]
            Me.StatusStrip.Refresh()
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
        If Me.StatusStrip.InvokeRequired Then
            Dim d As New SetAfterWorkCallback(AddressOf SetAfterWork)
            Me.Invoke(d, New Object() {})
        Else
            Me.Cursor = Windows.Forms.Cursors.Default
            Me.ReplicateButton.Enabled = True
            Me.DataAreaComboBox.Enabled = True
            Me.WorkspaceDropDownList.Enabled = True
            Me.DomainCombo.Enabled = True
            Me.ToggleInOutButton.Enabled = True
            Me.InboundToggleButton.Enabled = True
            Me.OutboundToggleButton.Enabled = True
            Me.CancelButton.Text = "Finish"
            'Me.Enabled = True
            Me.StatusStrip.Refresh()
        End If


    End Sub
    ''' <summary>
    ''' Progress EventHandler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ReplicationProgress(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles _replicateWorker.ProgressChanged

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
    Private Sub EndOfReplication(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) _
        Handles _replicateWorker.RunWorkerCompleted

        If TypeOf (e.Result) Is Boolean AndAlso e.Result = True Then
            Me.StatusLabel.Text = "replication finished sucessfully"
        ElseIf TypeOf (e.Result) Is Boolean AndAlso e.Result = False Then
            Me.StatusLabel.Text = "replication finished with no success"
        End If

        Call SetAfterWork()

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    ''' <remarks></remarks>
    Private Sub OutboundToggleButton_ToggleStateChanged(sender As Object, args As Telerik.WinControls.UI.StateChangedEventArgs) Handles OutboundToggleButton.ToggleStateChanged

        If args.ToggleState = Enumerations.ToggleState.On Then
            Me._replicationMode = ReplicationMode.Outbound

            Me.ToggleInOutButton.ToggleState = Telerik.WinControls.Enumerations.ToggleState.Off
            Me.InboundToggleButton.ToggleState = Telerik.WinControls.Enumerations.ToggleState.Off
        ElseIf Me._replicationMode = ReplicationMode.Outbound Then
            ' don't switch off
            DirectCast(sender, RadToggleButton).ToggleState = Telerik.WinControls.Enumerations.ToggleState.On
        End If
        Me.Refresh()
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    ''' <remarks></remarks>
    Private Sub InboundToggleButton_ToggleStateChanged(sender As Object, args As Telerik.WinControls.UI.StateChangedEventArgs) Handles InboundToggleButton.ToggleStateChanged


        If args.ToggleState = Enumerations.ToggleState.On Then
            Me._replicationMode = ReplicationMode.Inbound

            Me.OutboundToggleButton.ToggleState = Telerik.WinControls.Enumerations.ToggleState.Off
            Me.ToggleInOutButton.ToggleState = Telerik.WinControls.Enumerations.ToggleState.Off
        ElseIf Me._replicationMode = ReplicationMode.Inbound Then
            ' don't switch off
            DirectCast(sender, RadToggleButton).ToggleState = Telerik.WinControls.Enumerations.ToggleState.On
        End If
        Me.Refresh()
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    ''' <remarks></remarks>
    Private Sub ToggleInOutButton_ToggleStateChanged(sender As Object, args As Telerik.WinControls.UI.StateChangedEventArgs) Handles ToggleInOutButton.ToggleStateChanged

        If args.ToggleState = Enumerations.ToggleState.On Then
            Me._replicationMode = ReplicationMode.InAndOut
            Me.OutboundToggleButton.ToggleState = Telerik.WinControls.Enumerations.ToggleState.Off
            Me.InboundToggleButton.ToggleState = Telerik.WinControls.Enumerations.ToggleState.Off
        ElseIf Me._replicationMode = ReplicationMode.InAndOut Then
            ' don't switch off
            DirectCast(sender, RadToggleButton).ToggleState = Telerik.WinControls.Enumerations.ToggleState.On
        End If
        Me.Refresh()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub WorkspaceDropDownList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles WorkspaceDropDownList.TextChanged
        Dim found As Boolean
        '*** check workspaces
        Dim workspaceID As String
        workspaceID = Me.WorkspaceDropDownList.Text
        found = False
        If String.IsNullOrWhiteSpace(workspaceID) AndAlso _workspaceList Is Nothing Then
            For Each aWorkspace As Workspace In _workspaceList
                If LCase(workspaceID) = LCase(aWorkspace.ID) Then
                    found = True
                    Exit For
                End If
            Next

            If Not found Then
                Me.StatusLabel.Text = "workspaceID '" & workspaceID & "' not defined"
            End If
        End If
    End Sub
    ''' <summary>
    ''' validate Entry
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function Validate() As Boolean
        Dim found As Boolean
        '*** check workspaces
        Dim workspaceID As String
        workspaceID = WorkspaceDropDownList.Text
        found = False
        For Each aWorkspace As Workspace In _workspaceList
            If LCase(workspaceID) = LCase(aWorkspace.ID) Then
                found = True
                Exit For
            End If
        Next

        If Not found Then
            Validate = False
            Me.StatusLabel.Text = "workspaceID '" & workspaceID & "' not defined"
        End If
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelButton.Click, Me.FormClosing
        If _replicateWorker.IsBusy Then _replicateWorker.CancelAsync()
        Dim FormClosingArgs As System.Windows.Forms.FormClosingEventArgs = TryCast(e, System.Windows.Forms.FormClosingEventArgs)
        If FormClosingArgs Is Nothing Then

            Me.Dispose()

        Else
            FormClosingArgs.Cancel = True
        End If


    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub AddConfig_Click(sender As Object, e As EventArgs)
        Dim aXconfig As XChangeConfiguration = modQuicknDirty.getXlsDoc9Xconfig
        Dim aRange As Microsoft.Office.Interop.Excel.Range = modQuicknDirty.GetdbDoc9Range()

        'Me.ConfigDropDownList.Text = aXconfig.CONFIGNAME

        'define the dataarea
        _dataarea = New XLSDataArea("doc9", aXconfig)
        _dataarea.DataRange = aRange
        _dataarea.SelectionID = "X2"
        _dataarea.HeaderIDRange = GetXlsParameterRangeByName( _
                            name:="doc9_headerid", workbook:=Globals.ThisAddIn.Application.ActiveWorkbook)

        'Me.RefEdit.Address = "'" & aRange.Parent.Name & "'!" & aRange.Address(External:=False)

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DataAreaComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DataAreaComboBox.SelectedIndexChanged
        Dim name As String = Me.DataAreaComboBox.Text
        Dim i As Integer = _dataAreaList.FindIndex(Function(d As XLSDataArea) (d.Name = name))
        If i >= 0 Then
            _dataarea = _dataAreaList.ElementAt(i)
        Else
            Me.StatusLabel.Text = "Data Area not found in workbook"
        End If


    End Sub
    ''' <summary>
    ''' handler for selected index changed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DomainCombo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DomainCombo.SelectedIndexChanged
        _domainID = Me.DomainCombo.Text
        Me.StatusLabel.Text = "Domain changed to " & _domainID
    End Sub
End Class
