Imports Telerik.WinControls.Data
Imports System.ComponentModel
Imports System.Data
Imports Telerik.WinControls.UI

Imports OnTrack

Public Class UIFormMessageLog

    Private WithEvents _messageLog As New System.Data.DataTable("messageLog")
    Private WithEvents _mydataset As New System.Data.DataSet
    Private WithEvents _session As clsOTDBSession
    Private WithEvents _otdblog As clsOTDBErrorLog
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
        _messageLog.Columns.Add("Sub or Function", GetType(String))
        _messageLog.Columns.Add("Table", GetType(String))
        _messageLog.Columns.Add("Entry", GetType(String))
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

        Me.GridView.Columns("Message").MinWidth = 300

        Me.GridView.Columns("Timestamp").Width = 120
        Me.GridView.Columns("Sub or Function").Width = 120
        Me.GridView.Columns("Argument").Width = 120
        Me.GridView.Columns("Table").Width = 120
        Me.GridView.Columns("Entry").Width = 120
        'Me.GridView.MasterGridViewTemplate.AutoGenerateColumns = True
        'Me.GridView.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill
    End Sub
    ''' <summary>
    ''' Gets or sets the session.
    ''' </summary>
    ''' <value>The session.</value>
    Public Property Session() As clsOTDBSession
        Get
            Return Me._session
        End Get
        Set(value As clsOTDBSession)
            Me._session = value
            _otdblog = _session.Errorlog
            _messageLog.Clear()
            '** initial fill
            For Each [error] As clsOTDBError In _otdblog
                With [error]
                    _messageLog.Rows.Add(.Entryno, .messagetype, .Message, .Timestamp, .Arguments, .Subname, .Tablename, .EntryName)
                End With
            Next
        End Set
    End Property

    Private Sub OnConnected(sender As Object, e As EventArgs) Handles _session.OnStarted
        _otdblog = _session.Errorlog
        _messageLog.Clear()
        '** initial fill
        For Each [error] As clsOTDBError In _otdblog
            With [error]
                _messageLog.Rows.Add(.Entryno, .messagetype, .Message, .Timestamp, .Arguments, .Subname, .Tablename, .EntryName)
            End With
        Next
    End Sub

    Private Sub AddErrorEvent(sender As Object, e As OTDBErrorEventArgs) Handles _otdblog.onErrorRaised
        With e.Error
            _messageLog.Rows.Add(.Entryno, .messagetype, .Message, .Timestamp, .Arguments, .Subname, .Tablename, .EntryName)
        End With
    End Sub

    Private Sub ClearToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearToolStripMenuItem.Click
        _messageLog.Clear()
    End Sub

    Private Sub CloseButton_Click(sender As Object, e As EventArgs) Handles CloseButton.Click
        Me.Close()
    End Sub

    Private Sub GridView_Click(sender As Object, e As EventArgs) Handles GridView.Click

    End Sub

    Private Sub MessageLogForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If OTDB.CurrentSession.IsInitialized Then
            _otdblog = Errorlog

            '** initial fill
            For Each [error] As clsOTDBError In _otdblog
                With [error]
                    _messageLog.Rows.Add(.Entryno, .messagetype, .Message, .Timestamp, .Arguments, .Subname, .Tablename, .EntryName)
                End With
            Next
        End If
    End Sub
End Class
