Imports Telerik.WinControls.Data
Imports System.ComponentModel
Imports System.Data
Imports Telerik.WinControls.UI
Imports Telerik.WinControls.UI.Export
Imports OnTrack.Database
Imports OnTrack.Core

''' <summary>
'''  Form for MessageLog
''' </summary>
''' <remarks></remarks>

Public Class UIFormMessageLog

    Private WithEvents _messageLog As New System.Data.DataTable("messageLog")
    Private WithEvents _mydataset As New System.Data.DataSet
    '** links to Ontrack
    Private WithEvents _session As Session
    Private WithEvents _otdblog As SessionMessageLog
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        _mydataset.Tables.Add(_messageLog)
        _messageLog.Columns.Add("#", GetType(Long))
        _messageLog.Columns.Add("Type", GetType(otCoreMessageType))
        _messageLog.Columns.Add("Message", GetType(String))
        _messageLog.Columns.Add("Timestamp", GetType(DateTime))

        _messageLog.Columns.Add("Argument", GetType(String))
        _messageLog.Columns.Add("Routine", GetType(String))
        _messageLog.Columns.Add("Object", GetType(String))
        _messageLog.Columns.Add("Entry", GetType(String))
        _messageLog.Columns.Add("Table", GetType(String))
        _messageLog.Columns.Add("Columnname", GetType(String))
        _messageLog.Columns.Add("ObjectTag", GetType(String))
        _messageLog.Columns.Add("Username", GetType(String))
        _messageLog.Columns.Add("Stacktrace", GetType(String))
        '_messageLog.Columns.Add("Exception", GetType(String))
        GridView.EnableSorting = True
        Dim descriptor As New SortDescriptor()
        descriptor.PropertyName = "#"
        descriptor.Direction = ListSortDirection.Descending

        Me.GridView.MasterTemplate.SortDescriptors.Add(descriptor)
        GridView.MasterTemplate.DataSource = _messageLog
        'Me.GridView.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill

        Me.GridView.Columns("#").MaxWidth = 40
        Me.GridView.Columns("#").AllowResize = False
        Me.GridView.Columns("Type").Width = 100
        Me.GridView.Columns("Type").AllowResize = False

        Me.GridView.Columns("Message").MinWidth = 500

        Me.GridView.Columns("Timestamp").Width = 120
        Me.GridView.Columns("Routine").Width = 120
        Me.GridView.Columns("Argument").Width = 120
        Me.GridView.Columns("Object").Width = 120
        Me.GridView.Columns("Entry").Width = 120
        Me.GridView.Columns("Table").Width = 120
        Me.GridView.Columns("Columnname").Width = 120

        Me.GridView.Columns("Stacktrace").Width = 400

        'Me.GridView.MasterGridViewTemplate.AutoGenerateColumns = True
        'Me.GridView.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill
    End Sub
    ''' <summary>
    ''' Gets or sets the session.
    ''' </summary>
    ''' <value>The session.</value>
    Public Property Session() As Session
        Get
            Return Me._session
        End Get
        Set(value As Session)
            Me._session = value
            _otdblog = _session.Errorlog
            _messageLog.Clear()
            '** initial fill
            For Each [error] As SessionMessage In _otdblog
                With [error]
                    _messageLog.Rows.Add(.Entryno, .messagetype, .Message, .Timestamp, .Arguments, .Subname, .Objectname, .ObjectEntry, .Tablename, .Columnname, .Objecttag, _
                                         .Username, .StackTrace)
                End With
            Next
        End Set
    End Property
    ''' <summary>
    ''' On Session start
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnConnected(sender As Object, e As EventArgs) Handles _session.OnStarted
        _otdblog = _session.Errorlog
        _messageLog.Clear()
        '** initial fill
        For Each [error] As SessionMessage In _otdblog
            With [error]
                _messageLog.Rows.Add(.Entryno, .messagetype, .Message, .Timestamp, .Arguments, .Subname, .Objectname, .ObjectEntry, .Tablename, .Columnname, .Objecttag, _
                                         .Username, .StackTrace)
            End With
        Next
    End Sub
    ''' <summary>
    ''' OnError Raised
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub AddErrorEvent(sender As Object, e As ormErrorEventArgs) Handles _otdblog.onErrorRaised
        With e.Error
            _messageLog.Rows.Add(.Entryno, .messagetype, .Message, .Timestamp, .Arguments, .Subname, .Objectname, .ObjectEntry, .Tablename, .Columnname, .Objecttag, _
                                         .Username, .StackTrace)
        End With
    End Sub
    ''' <summary>
    '''  cleear log
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ClearToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearToolStripMenuItem.Click
        _messageLog.Clear()
        Me.Refresh()
    End Sub
    ''' <summary>
    ''' Close
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CloseButton_Click(sender As Object, e As EventArgs) Handles CloseButton.Click
        Me.Close()
    End Sub

    ''' <summary>
    ''' On Load
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MessageLogForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If ot.CurrentSession.IsInitialized Then
            _otdblog = ot.Errorlog
            _messageLog.Clear()
            '** initial fill
            For Each [error] As SessionMessage In _otdblog
                With [error]
                    _messageLog.Rows.Add(.Entryno, .messagetype, .Message, .Timestamp, .Arguments, .Subname, .Objectname, .ObjectEntry, .Tablename, .Columnname, .Objecttag, _
                                         .Username, .StackTrace)
                End With
            Next
        End If
    End Sub

    ''' <summary>
    ''' Refresh
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RefreshToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RefreshToolStripMenuItem.Click
        Me.GridView.Refresh()
    End Sub

    ''' <summary>
    ''' Export
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExportStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportStripMenuItem.Click
        Dim saveDialog As New Windows.Forms.SaveFileDialog()
        saveDialog.FileName = "OnTrackSessionLog_" & Format(DateTime.Now, "yyyy-MM-ddTHHMMss")
        saveDialog.DefaultExt = ".xlsx"
        saveDialog.Filter = "Excel|*.xlsx"
        Dim dialogResult As Windows.Forms.DialogResult = saveDialog.ShowDialog()
        If dialogResult = System.Windows.Forms.DialogResult.OK Then
            Dim exporter As ExportToExcelML = New ExportToExcelML(Me.GridView)
            exporter.ExportVisualSettings = True
            exporter.SheetName = "Log"
            exporter.RunExport(saveDialog.FileName)
            Me.StatusLabel.Text = "file saved to " & saveDialog.FileName
        End If
    End Sub
End Class
