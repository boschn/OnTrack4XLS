''' <summary>
''' Deliverable Panel for editing a deliverable object
''' </summary>
''' <remarks>
''' </remarks>

Imports Telerik.WinControls.UI
Imports System.Drawing
Imports System.Windows.Forms
Imports OnTrack.Database
Imports OnTrack.Commons
Imports System.ComponentModel
Imports OnTrack.UI


Public Class UIControlDeliverablePanel
    Implements iUIStatusSender

    ''' <summary>
    ''' Elements
    ''' </summary>
    ''' <remarks></remarks>
    Private WithEvents _modeltable As ormModelTable    ''' model
    Private WithEvents _dataobject As iormRelationalPersistable ''' model

    Private WithEvents _statuslabel As RadLabelElement
    Private WithEvents _ProgressPictureBox As System.Windows.Forms.PictureBox
    Private _pwfieldname As String = String.Empty
    Private WithEvents _controller As MVDataObjectController 'controller

   

    Public Event OnIssueMessage(sender As Object, e As UIStatusMessageEventArgs) Implements iUIStatusSender.OnIssueMessage

#Region "Properties"
    ''' <summary>
    ''' Gets or sets the dataobject.
    ''' </summary>
    ''' <value>The dataobject.</value>
    Public Property Dataobject() As iormRelationalPersistable
        Get
            Return _dataobject
        End Get
        Set(value As iormRelationalPersistable)
            _dataobject = Value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the controller.
    ''' </summary>
    ''' <value>The controller.</value>
    Public Property Controller() As MVDataObjectController
        Get
            Return _controller
        End Get
        Set(value As MVDataObjectController)
            _controller = value
            For Each aControl In Me.Controls
                If aControl.GetType Is GetType(UI.UIControlDataEntryBox) Then
                    CType(aControl, UI.UIControlDataEntryBox).Controller = _controller
                Else
                    SetController(Control:=aControl, controller:=value)
                End If
            Next
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the progress picture box.
    ''' </summary>
    ''' <value>The progress picture box.</value>
    Public ReadOnly Property ProgressPictureBox() As PictureBox
        Get
            If _ProgressPictureBox Is Nothing Then
                '
                'ProgressPictureBox
                '
                _ProgressPictureBox = New System.Windows.Forms.PictureBox
                CType(Me._ProgressPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
                With _ProgressPictureBox

                    .Enabled = False
                    .Dock = System.Windows.Forms.DockStyle.Fill
                    .Image = My.Resources.Resources.progress_radar
                    '.Location = New System.Drawing.Point(0, 0)
                    .Name = "ProgressPictureBox"
                    .Size = Me.Size
                    .SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
                    .TabIndex = 0
                    .TabStop = False
                End With
                CType(Me._ProgressPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
            End If
            If Me.Controls.Count = 0 Then Me.Controls.Add(_ProgressPictureBox)
            Return Me._ProgressPictureBox
        End Get

    End Property

    ''' <summary>
    ''' Gets or sets the status.
    ''' </summary>
    ''' <value>The status.</value>
    Public Property Status() As RadLabelElement
        Get
            Return Me._statuslabel
        End Get
        Set(value As RadLabelElement)
            Me._statuslabel = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the modeltable.
    ''' </summary>
    ''' <value>The modeltable.</value>
    Public Property ModelTable As ormModelTable
        Get
            Return Me._modeltable
        End Get
        Set(value As ormModelTable)
            Me._modeltable = value
        End Set
    End Property

    ''' <summary>
    ''' sets the theme name for the embedded page view
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property ThemeName As String
        Get
            Return Me.PageView.ThemeName
        End Get
        Set(value As String)
            Me.PageView.ThemeName = value
        End Set
    End Property

    ''' <summary>
    ''' data source to bind to, bind also to embedded data boxes with same object name
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <BrowsableAttribute(True), Category("Data"), Description("data source of this data entry box")> _
    Public Property DataSource() As Object
        Get
            If _modeltable IsNot Nothing Then Return _modeltable
            Return Nothing
        End Get
        Set(value As Object)
            If value Is Nothing Then
                _modeltable = Nothing
            ElseIf value.GetType Is GetType(ormModelTable) Then
                _modeltable = value
                For Each aControl In Me.Controls
                    If aControl.GetType Is GetType(UI.UIControlDataEntryBox) Then
                        If CType(aControl, UI.UIControlDataEntryBox).ObjectName = _modeltable.DataObjectID Then
                            CType(aControl, UI.UIControlDataEntryBox).DataSource = _modeltable
                        End If
                    Else
                        SetDataSource(Control:=aControl, dataSource:=value)
                    End If
                Next
            ElseIf value.GetType.GetInterfaces.Contains(GetType(iormRelationalPersistable)) Then
                _dataobject = value
                For Each aControl In Me.Controls
                    If aControl.GetType Is GetType(UI.UIControlDataEntryBox) Then
                        If CType(aControl, UI.UIControlDataEntryBox).ObjectName = _dataobject.ObjectID Then
                            CType(aControl, UI.UIControlDataEntryBox).DataSource = _dataobject
                        End If
                    Else
                        SetDataSource(Control:=aControl, dataSource:=value)
                    End If
                Next
            End If

        End Set
    End Property
    ''' <summary>
    ''' type of the data souce
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <BrowsableAttribute(False), Category("Data"), Description("data source type of this data entry box")> _
    Public ReadOnly Property DataSouceType As Type
        Get
            If Me.DataSource IsNot Nothing Then Return Me.DataSource.GetType
            Return Nothing
        End Get

    End Property
#End Region

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    ''' <summary>
    ''' recursivley set all subcoontrols datasource to datasource
    ''' </summary>
    ''' <param name="Control"></param>
    ''' <param name="dataSource"></param>
    ''' <remarks></remarks>
    Private Sub SetDataSource(Control As Control, dataSource As Object)
        For Each aControl In Control.Controls
            If aControl.GetType Is GetType(UI.UIControlDataEntryBox) Then
                CType(aControl, UI.UIControlDataEntryBox).DataSource = dataSource
            Else
                ''' call again if control has own controls
                If aControl.Controls.Count > 0 Then
                    Call SetDataSource(Control:=aControl, dataSource:=dataSource)
                End If
            End If
        Next
    End Sub
    ''' <summary>
    ''' recursivley set all subcontrols controller to controller
    ''' </summary>
    ''' <param name="Control"></param>
    ''' <param name="dataSource"></param>
    ''' <remarks></remarks>
    Private Sub SetController(Control As Control, controller As MVDataObjectController)
        For Each aControl In Control.Controls
            If aControl.GetType Is GetType(UI.UIControlDataEntryBox) Then
                CType(aControl, UI.UIControlDataEntryBox).Controller = controller
            Else
                ''' call again if control has own controls
                If aControl.Controls.Count > 0 Then
                    Call SetController(Control:=aControl, controller:=controller)
                End If
            End If
        Next
    End Sub
    ''' <summary>
    ''' Add a Message to the connected status message
    ''' </summary>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    Private Sub IssueStatusMessage(message As String)
        If _statuslabel IsNot Nothing Then _statuslabel.Text = Date.Now & " : " & message
        RaiseEvent OnIssueMessage(Me, New UIStatusMessageEventArgs(message))
    End Sub

    ''' <summary>
    ''' Create failed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Modeltable_ObjectOpFailed(sender As Object, e As ormModelTable.EventArgs) Handles _modeltable.ObjectCreateFailed, _modeltable.ObjectDeleteFailed
        If Not String.IsNullOrWhiteSpace(e.Message) Then Me.IssueStatusMessage(e.Message)
    End Sub
    ''' <summary>
    ''' Update failed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Modeltable_ObjectUpdateFailed(sender As Object, e As ormModelTable.EventArgs) Handles _modeltable.ObjectUpdateFailed, _modeltable.ObjectPersistFailed
        If e.Msglog.Count > 0 Then Me.IssueStatusMessage(e.Msglog.MessageText)
    End Sub
    ''' <summary>
    ''' Event handler for operation messages
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub OnMessageFromTable(sender As Object, e As ormModelTable.EventArgs) Handles _modeltable.OperationMessage
        IssueStatusMessage(e.Message)
    End Sub


    ''' <summary>
    ''' handle the change data object event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub _controller_OnChangingDataObject(sender As Object, e As MVDataObjectController.EventArgs) Handles _controller.OnChangingDataObject
        Me.DataSource = e.DataObject
    End Sub
End Class
