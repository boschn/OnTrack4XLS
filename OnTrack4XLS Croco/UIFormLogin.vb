Imports OnTrack


Public Class UIFormLogin
    Implements iUINativeFormLogin

    Private myshadow As clsCoreUILogin

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ' nothing on the shadow

    End Sub
    ''' <summary>
    ''' Gets or sets the username enabled.
    ''' </summary>
    ''' <value>The username enabled.</value>
    Public Property UsernameEnabled() As Boolean Implements iUINativeFormLogin.UsernameEnabled
        Get
            Return Me.TbUsername.Enabled
        End Get
        Set(value As Boolean)
            Me.TbUsername.Enabled = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the message.
    ''' </summary>
    ''' <value>The message.</value>
    Public Property Message() As String Implements iUINativeFormLogin.Message
        Get
            Return Me.WelcomeLabel.Text
        End Get
        Set(value As String)
            Me.WelcomeLabel.Text = value
        End Set
    End Property

    Public Function CloseOTDBForm() As Object Implements iUINativeForm.CloseOTDBForm
        Me.Close()

    End Function

    Public Property OtdbShadow As iOTDBUIAbstractForm Implements iUINativeForm.OtdbShadow
        Set(value As iOTDBUIAbstractForm)
            myshadow = value
        End Set
        Get
            Return myshadow
        End Get
    End Property

    Public Function RefreshOTDBForm() As Object Implements iUINativeForm.RefreshOTDBForm
        Me.Refresh()
    End Function

    Public Function ShowOTDBForm() As Object Implements iUINativeForm.ShowOTDBForm
        Me.ShowDialog()
    End Function

    Public Property Password As String Implements iUINativeFormLogin.Password
        Set(value As String)
            Me.TbPassword.Text = value
        End Set
        Get
            Return TbPassword.Text
        End Get
    End Property

    Public Property Righttext As String Implements iUINativeFormLogin.Right
        Set(value As String)
            Me.TbRight.Text = value
        End Set
        Get
            Return TbRight.Text
        End Get
    End Property

    Public Property StatusText As String Implements iUINativeFormLogin.StatusText
        Set(value As String)
            Me.Status.Text = value
            Me.Refresh()
        End Set
        Get
            Return Status.Text
        End Get
    End Property

    Public Property Username As String Implements iUINativeFormLogin.Username
        Set(value As String)
            Me.TbUsername.Text = value
        End Set
        Get
            Return Me.TbUsername.Text
        End Get
    End Property

    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelButton.Click
        If Not myshadow Is Nothing Then
            DirectCast(Me.OtdbShadow, clsCoreUILogin).Ok = False
        End If
        Me.Close()
    End Sub

    Private Sub LoginButton_Click(sender As Object, e As EventArgs) Handles LoginButton.Click
        If Not Me.OtdbShadow Is Nothing Then
            myshadow.Username = Me.TbUsername.Text
            myshadow.Password = Me.TbPassword.Text

            If myshadow.Verify Then
                Me.Close()
            Else
                Me.Status.Text = myshadow.Statustext
                Me.Status.BackColor = Drawing.Color.DarkRed
                Me.Status.ForeColor = Drawing.Color.White
                Me.Refresh()
            End If
        End If
    End Sub

    ''' <summary>
    ''' set onLoad everything dependend on the shadow
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TelerikLoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If myshadow Is Nothing Then
        Else
            If Me.myshadow.EnableUsername Then
                Me.TbUsername.Focus()
            Else
                Me.TbUsername.Enabled = False
                Me.TbPassword.Focus()

            End If

            If Me.myshadow.Messagetext = "" Then
                Me.Message = "<html><strong>Welcome !</strong><br />Please enter your Username and Password to obtain access to the OnTrack Database.</html>"
            Else
                Me.Message = Me.myshadow.Messagetext
            End If
        End If

    End Sub
End Class
