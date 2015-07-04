Imports System.Windows.Forms
Imports Telerik.WinControls

''' <summary>
''' Status Strip Control for Messages, Operation Buttons
''' </summary>
''' <remarks>
''' Functional Design Principle
''' 1. A Status Strip at the bottom Dock Position of a form
''' 2. Possibility to show messages to the user
''' 3. Show to the User a Progressbar for longtime Operations 
''' 4. Allow the User to Accept or Cancel a Operation
''' 5. Allow the User to Close a Form</remarks>
Public Class UIControlStatusStrip

    ''' <summary>
    ''' Messages Class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Messages
        Private _message As String
        Private _timestamp As DateTime

        Public Sub New(message As String, timestamp As DateTime)
            _message = message
            _timestamp = timestamp
        End Sub
        ''' <summary>
        ''' Gets or sets the timestamp.
        ''' </summary>
        ''' <value>The timestamp.</value>
        Public Property Timestamp() As DateTime
            Get
                Return _timestamp
            End Get
            Set(value As DateTime)
                _timestamp = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the message.
        ''' </summary>
        ''' <value>The message.</value>
        Public Property Message() As String
            Get
                Return _message
            End Get
            Set(value As String)
                _message = value
            End Set
        End Property

    End Class
    ''' <summary>
    ''' Internal 
    ''' </summary>
    ''' <remarks></remarks>
    Private WithEvents _controller As MVDataObjectController
    Private _messages As New List(Of Messages)

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
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.ProgressBar.Visibility = Telerik.WinControls.ElementVisibility.Hidden
        Me.ProgressBar.Enabled = False

        Me.AcceptStripButton.Enabled = False
        Me.AcceptStripButton.Visibility = Telerik.WinControls.ElementVisibility.Hidden

        Me.CancelStripButton.Enabled = False
        Me.CancelStripButton.Visibility = Telerik.WinControls.ElementVisibility.Hidden

    End Sub

    ''' <summary>
    ''' Event Parent Changed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub UIControlStatusStrip_ParentChanged(sender As Object, e As EventArgs) Handles Me.ParentChanged
        If Me.Parent.GetType.IsSubclassOf(GetType(Windows.Forms.Form)) Then
            Me.CloseStripButton.Enabled = True
            Me.CloseStripButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
            AddHandler Me.Parent.ControlAdded, AddressOf UIControlStatusStrip_NewParentControl
        End If
    End Sub
    ''' <summary>
    ''' New ParentControl
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub UIControlStatusStrip_NewParentControl(sender As Object, e As ControlEventArgs)
        If e.Control IsNot Me Then RegisterControls(e.Control) ' register us
    End Sub
    ''' <summary>
    ''' register all controls which implements UIStatusSender of the parents with this status strip
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RegisterStatusControl()
        If Me.Parent IsNot Nothing Then
            RegisterControls(Me.Parent)
        End If
    End Sub
    ''' <summary>
    ''' recursively register this Status Control to all sub controls which implement status sender interface of the argument 
    ''' </summary>
    ''' <param name="Control"></param>
    ''' <param name="dataSource"></param>
    ''' <remarks></remarks>
    Public Sub RegisterControls(Control As Windows.Forms.Control)
        If Control IsNot Me Then
            If Control.GetType.GetInterfaces.Where(Function(x) x.Name = GetType(iUIStatusSender).Name).FirstOrDefault IsNot Nothing Then
                AddHandler CType(Control, UI.iUIStatusSender).OnIssueMessage, AddressOf IssueMessage
            End If

            ''' call again if control has own controls
            If Control.Controls.Count > 0 Then
                For Each acontrol In Control.Controls
                    Call RegisterControls(Control:=acontrol)
                Next
            End If
        End If
    End Sub

    ''' <summary>
    ''' recursivley set all subcontrols controller to controller
    ''' </summary>
    ''' <param name="Control"></param>
    ''' <param name="dataSource"></param>
    ''' <remarks></remarks>
    Private Sub SetController(Control As Windows.Forms.Control, controller As MVDataObjectController)
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
    ''' Add a Message to the Message List and display
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub IssueMessage(sender As Object, e As UIStatusMessageEventArgs)
        Me.IssueMessage(New Messages(timestamp:=e.timestamp, message:=e.Message))
    End Sub
    ''' <summary>
    ''' Add a Message to the Message List and display
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub IssueMessage(message As String, Optional timestamp As DateTime? = Nothing)
        Me.IssueMessage(New Messages(timestamp:=timestamp, message:=message))
    End Sub
    ''' <summary>
    ''' Add a Message to the Message List and display
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub IssueMessage(message As UIControlStatusStrip.Messages)
        _messages.Add(message)
        Me.StatusLabel.Text = message.Timestamp & " " & message.Message
    End Sub


    Private Sub _controller_OnChangingToCreate(sender As Object, e As MVDataObjectController.EventArgs) Handles _controller.OnChangingToCreate
        'Me.AcceptButton.Visibility = ElementVisibility.Visible
        'Me.AcceptButton.Enabled = True

        'Me.DeleteButton.Visibility = ElementVisibility.Visible
        'Me.DeleteButton.Enabled = True
        'Me.DeleteButton.ToolTipText = "Abort Changes"

        'Me.EditButton.Visibility = ElementVisibility.Hidden
        'Me.EditButton.Enabled = False

        'Me.AddNewButton.Visibility = ElementVisibility.Hidden
        'Me.AddNewButton.Enabled = False

        ''' in Strip
        Me.AcceptStripButton.Visibility = ElementVisibility.Visible
        Me.AcceptStripButton.Enabled = True
        Me.CancelStripButton.Visibility = ElementVisibility.Visible
        Me.CancelStripButton.Enabled = True
    End Sub

    Private Sub _controller_OnChangingToDelete(sender As Object, e As MVDataObjectController.EventArgs) Handles _controller.OnChangingToDelete
        ''' from Update abort changes go to read
        If Me.Controller.State = MVDataObjectController.CRUDState.Update Then
            Me.Controller.State = MVDataObjectController.CRUDState.Read
        Else
            ''' delete object

            'Me.AcceptButton.Visibility = ElementVisibility.Hidden
            'Me.AcceptButton.Enabled = False

            'Me.EditButton.Visibility = ElementVisibility.Hidden
            'Me.EditButton.Enabled = False
        End If
    End Sub

    Private Sub _controller_OnChangingToRead(sender As Object, e As MVDataObjectController.EventArgs) Handles _controller.OnChangingToRead
        'Me.AcceptButton.Visibility = ElementVisibility.Hidden
        'Me.AcceptButton.Enabled = False

        'Me.DeleteButton.Visibility = ElementVisibility.Visible
        'Me.DeleteButton.Enabled = True
        'Me.DeleteButton.ToolTipText = "Delete"

        'Me.EditButton.Visibility = ElementVisibility.Visible
        'Me.EditButton.Enabled = True

        'Me.AddNewButton.Visibility = ElementVisibility.Visible
        'Me.AddNewButton.Enabled = True

        ''' in Strip
        Me.AcceptStripButton.Visibility = ElementVisibility.Hidden
        Me.AcceptStripButton.Enabled = False
        Me.CancelStripButton.Visibility = ElementVisibility.Hidden
        Me.CancelStripButton.Enabled = False
    End Sub

    Private Sub _controller_OnChangingToUpdate(sender As Object, e As MVDataObjectController.EventArgs) Handles _controller.OnChangingToUpdate
        'Me.AcceptButton.Visibility = ElementVisibility.Visible
        'Me.AcceptButton.Enabled = True

        'Me.DeleteButton.Visibility = ElementVisibility.Visible
        'Me.DeleteButton.Enabled = True
        'Me.DeleteButton.ToolTipText = "Abort Changes"

        'Me.EditButton.Visibility = ElementVisibility.Hidden
        'Me.EditButton.Enabled = False

        'Me.AddNewButton.Visibility = ElementVisibility.Hidden
        'Me.AddNewButton.Enabled = False

        ''' in Strip
        Me.AcceptStripButton.Visibility = ElementVisibility.Visible
        Me.AcceptStripButton.Enabled = True

        Me.CancelStripButton.Visibility = ElementVisibility.Visible
        Me.CancelStripButton.Enabled = True
        Me.CancelStripButton.ToolTipText = "Abort Changes"
    End Sub

    ''' Close Button Handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub UIFormWorkDeliverables_CloseClicked(sender As Object, e As EventArgs) Handles CloseStripButton.Click
        If Me.Parent.GetType.IsSubclassOf(GetType(Windows.Forms.Form)) Then
            RemoveHandler Me.Parent.ControlAdded, AddressOf UIControlStatusStrip_NewParentControl
            CType(Me.Parent, Form).Close()
        End If

    End Sub
    Private Sub AcceptButton_Click(sender As Object, e As EventArgs) Handles AcceptStripButton.Click
        If Me.Controller IsNot Nothing Then
            Me.Controller.State = MVDataObjectController.CRUDState.Read
        End If
    End Sub
End Class
