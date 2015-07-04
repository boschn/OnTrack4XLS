Imports OnTrack
Imports OnTrack.UI
Imports OnTrack.Database
Imports OnTrack.Core



Public Class UIFormLogin
    Implements iUINativeFormLogin

    Private myshadow As CoreLoginForm

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
    ''' <summary>
    ''' Gets or sets the message.
    ''' </summary>
    ''' <value>The message.</value>
    Public Property Domain() As String Implements iUINativeFormLogin.Domain
        Get
            Return Me.CbDomain.Text
        End Get
        Set(value As String)
            Me.CbDomain.Text = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the domain list.
    ''' </summary>
    ''' <value>The domain list.</value>
    Public Property DomainList() As List(Of String) Implements iUINativeFormLogin.DomainList
        Get
            Return CbDomain.DataSource
        End Get
        Set(value As List(Of String))
            CbDomain.DataSource = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the domain change enables.
    ''' </summary>
    ''' <value>The domain change enables.</value>
    Public Property DomainChangeEnables() As Boolean Implements iUINativeFormLogin.DomainChangeEnables
        Get
            Return CbDomain.Enabled
        End Get
        Set(value As Boolean)
            CbDomain.Enabled = value
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
            Me.CbAccess.Text = value
        End Set
        Get
            Return CbAccess.Text
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the rights list.
    ''' </summary>
    ''' <value>The rights list.</value>
    Public Property RightsList() As List(Of String) Implements iUINativeFormLogin.RightsList
        Get
            Return CbAccess.DataSource
        End Get
        Set(value As List(Of String))
            CbAccess.DataSource = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the right change enabled.
    ''' </summary>
    ''' <value>The right change enabled.</value>
    Public Property RightChangeEnabled() As String Implements iUINativeFormLogin.RightChangeEnabled
        Get
            Return CbAccess.Enabled
        End Get
        Set(value As String)
            CbAccess.Enabled = value

        End Set
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

    ''' <summary>
    ''' Gets or sets the config set.
    ''' </summary>
    ''' <value>The config set.</value>
    Public Property ConfigSet() As String Implements iUINativeFormLogin.ConfigSet
        Get
            Return CbConfigSet.Text
        End Get
        Set(value As String)
            CbConfigSet.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the config set list.
    ''' </summary>
    ''' <value>The config set list.</value>
    Public Property ConfigSetList() As List(Of String) Implements iUINativeFormLogin.ConfigSetList
        Get
            Return CbConfigSet.DataSource
        End Get
        Set(value As List(Of String))
            CbConfigSet.DataSource = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the config set enabled.
    ''' </summary>
    ''' <value>The config set enabled.</value>
    Public Property ConfigSetEnabled() As Boolean Implements iUINativeFormLogin.ConfigSetEnabled
        Get
            Return CbConfigSet.Enabled
        End Get
        Set(value As Boolean)
            CbConfigSet.Enabled = value
        End Set
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
            DirectCast(Me.OtdbShadow, CoreLoginForm).Ok = False
        End If
        Me.Close()
    End Sub

    Private Sub LoginButton_Click(sender As Object, e As EventArgs) Handles LoginButton.Click
        If Me.OtdbShadow IsNot Nothing Then
            myshadow.Username = Me.TbUsername.Text
            myshadow.Password = Me.TbPassword.Text
            myshadow.Domain = Me.Domain
            myshadow.Configset = Me.ConfigSet
            Select Case Me.Righttext
                Case otAccessRight.AlterSchema.ToString
                    myshadow.Accessright = otAccessRight.AlterSchema
                Case otAccessRight.ReadUpdateData.ToString
                    myshadow.Accessright = otAccessRight.ReadUpdateData
                Case otAccessRight.ReadOnly.ToString
                    myshadow.Accessright = otAccessRight.ReadOnly
                Case otAccessRight.Prohibited.ToString
                    myshadow.Accessright = otAccessRight.Prohibited
            End Select

            If myshadow.Verify() Then
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
            Me.ConfigSetEnabled = myshadow.EnableChangeConfigSet
            RemoveHandler CbConfigSet.SelectedIndexChanged, AddressOf Me.CbConfigSet_SelectedIndexChanged
            Me.ConfigSetList = myshadow.PossibleConfigSets
            Me.ConfigSet = myshadow.Configset
            '** add handler here otherwise while adding the possible configs also the configset in myshadow will be set anew
            AddHandler CbConfigSet.SelectedIndexChanged, AddressOf Me.CbConfigSet_SelectedIndexChanged

            Me.CbConfigSet.AutoCompleteDataSource = myshadow.PossibleConfigSets
            Me.CbConfigSet.AutoCompleteMode = Windows.Forms.AutoCompleteMode.Append

            Me.RightsList = myshadow.PossibleRights
            Me.RightChangeEnabled = myshadow.enableAccess
            Me.Righttext = myshadow.Accessright.ToString
            Me.CbAccess.AutoCompleteDataSource = myshadow.PossibleRights
            Me.CbAccess.AutoCompleteMode = Windows.Forms.AutoCompleteMode.Append

            Me.Domain = myshadow.Domain
            Me.DomainChangeEnables = myshadow.EnableDomain
            Me.DomainList = myshadow.PossibleDomains
            If Me.DomainList Is Nothing OrElse Me.DomainList.Count = 0 Then
                Me.CbDomain.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDown
            Else
                Me.CbDomain.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList
            End If

            Me.CbDomain.AutoCompleteDataSource = myshadow.PossibleDomains
            Me.CbDomain.AutoCompleteMode = Windows.Forms.AutoCompleteMode.Append

            Me.Username = myshadow.Username
            Me.Password = myshadow.Password
            Me.UsernameEnabled = myshadow.EnableUsername


            If Me.myshadow.EnableUsername Then
                Me.TbUsername.Focus()
            Else
                Me.TbUsername.Enabled = False
                Me.TbPassword.Focus()

            End If

            If Me.myshadow.Messagetext = String.Empty Then
                Me.Message = "<html><strong>Welcome !</strong><br />Please enter your Username and Password to obtain access to the OnTrack Database.</html>"
            Else
                Me.Message = Me.myshadow.Messagetext
            End If
        End If

    End Sub
    ''' <summary>
    ''' handler for the change in the configset
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CbConfigSet_SelectedIndexChanged(sender As Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs)
        Dim aList As List(Of String) = CbConfigSet.DataSource
        If e.Position >= 0 And e.Position < aList.Count Then
            myshadow.Configset = aList.Item(e.Position)
        End If
    End Sub
End Class
