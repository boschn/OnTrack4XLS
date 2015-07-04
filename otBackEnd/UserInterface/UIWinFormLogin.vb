
Imports OnTrack
Imports OnTrack.UI
Imports OnTrack.Core

Public Class UIWinFormLogin
    Implements iUINativeFormLogin

    Private _ourUILogin As CoreLoginForm

    Friend WithEvents ButtonOk As System.Windows.Forms.Button
    Friend WithEvents CancelButton As System.Windows.Forms.Button


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        TbUsername.Enabled = True
    End Sub
    ''' <summary>
    ''' Gets or sets the username enabled.
    ''' </summary>
    ''' <value>The username enabled.</value>
    Public Property UsernameEnabled() As Boolean Implements iUINativeFormLogin.UsernameEnabled
        Get
            Return TbUsername.Enabled
        End Get
        Set(value As Boolean)
            TbUsername.Enabled = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the message.
    ''' </summary>
    ''' <value>The message.</value>
    Public Property Domain() As String Implements iUINativeFormLogin.Domain
        Get
            Return tbDomain.Text
        End Get
        Set(value As String)
            tbDomain.Text = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the domain list.
    ''' </summary>
    ''' <value>The domain list.</value>
    Public Property DomainList() As List(Of String) Implements iUINativeFormLogin.DomainList
        Get
            ' TODO: Implement this property getter
            Throw New NotImplementedException()
        End Get
        Set(value As List(Of String))
            ' TODO: Implement this property setter
            Throw New NotImplementedException()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the domain change enables.
    ''' </summary>
    ''' <value>The domain change enables.</value>
    Public Property DomainChangeEnables() As Boolean Implements iUINativeFormLogin.DomainChangeEnables
        Get
            ' TODO: Implement this property getter
            Throw New NotImplementedException()
        End Get
        Set(value As Boolean)
            ' TODO: Implement this property setter
            Throw New NotImplementedException()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the message.
    ''' </summary>
    ''' <value>The message.</value>
    Public Property Message() As String Implements iUINativeFormLogin.Message
        Get
            Return RBMessage.Text
        End Get
        Set(value As String)
            RBMessage.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Connect with OTDB counterpart
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    ''' Property OTDBParent As iOTDBAbstractUIForm ' for call back to the OTDB UI Form
    ''' <remarks></remarks>
    ''' <returns></returns>
    Public Function ShowOTDBForm() As Object Implements iUINativeForm.ShowOTDBForm
        Me.ShowDialog()
    End Function

    ''' <summary>
    ''' Close the Form
    ''' </summary>
    ''' <remarks></remarks>
    ''' <returns></returns>
    Public Function CloseOTDBForm() As Object Implements iUINativeForm.CloseOTDBForm
        Me.Close()
    End Function

    ''' <summary>
    ''' Refresh the Form
    ''' </summary>
    ''' <remarks></remarks>
    ''' <returns></returns>
    Public Function RefreshOTDBForm() As Object Implements iUINativeForm.RefreshOTDBForm
        Me.Refresh()
    End Function

    ''' <summary>
    ''' Gets or sets the config set.
    ''' </summary>
    ''' <value>The config set.</value>
    Public Property ConfigSet() As String Implements iUINativeFormLogin.ConfigSet
        Get
            ' TODO: Implement this property getter
            Throw New NotImplementedException()
        End Get
        Set(value As String)
            ' TODO: Implement this property setter
            Throw New NotImplementedException()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the config set list.
    ''' </summary>
    ''' <value>The config set list.</value>
    Public Property ConfigSetList() As List(Of String) Implements iUINativeFormLogin.ConfigSetList
        Get
            ' TODO: Implement this property getter
            Throw New NotImplementedException()
        End Get
        Set(value As List(Of String))
            ' TODO: Implement this property setter
            Throw New NotImplementedException()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the config set enabled.
    ''' </summary>
    ''' <value>The config set enabled.</value>
    Public Property ConfigSetEnabled() As Boolean Implements iUINativeFormLogin.ConfigSetEnabled
        Get
            ' TODO: Implement this property getter
            Throw New NotImplementedException()
        End Get
        Set(value As Boolean)
            ' TODO: Implement this property setter
            Throw New NotImplementedException()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the username.
    ''' </summary>
    ''' <value>The username.</value>
    Public Property Username() As String Implements iUINativeFormLogin.Username
        Get
            Return TbUsername.Text
        End Get
        Set(value As String)
            TbUsername.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the password.
    ''' </summary>
    ''' <value>The password.</value>
    Public Property Password() As String Implements iUINativeFormLogin.Password
        Get
            Return TBPassword.Text
        End Get
        Set(value As String)
            TBPassword.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the right.
    ''' <summary>
    ''' Gets or sets the right change enabled.
    ''' </summary>
    ''' <value>The right change enabled.</value>
    Public Property RightChangeEnabled() As String Implements iUINativeFormLogin.RightChangeEnabled
        Get
            ' TODO: Implement this property getter
            Throw New NotImplementedException()
        End Get
        Set(value As String)
            ' TODO: Implement this property setter
            Throw New NotImplementedException()
        End Set
    End Property

    ''' </summary>
    ''' <value>The right.</value>
    Public Property Right() As String Implements iUINativeFormLogin.Right
        Get
            Return Me.TBRight.Text
        End Get
        Set(value As String)
            Me.TBRight.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the rights list.
    ''' </summary>
    ''' <value>The rights list.</value>
    Public Property RightsList() As List(Of String) Implements iUINativeFormLogin.RightsList
        Get
            ' TODO: Implement this property getter
            Throw New NotImplementedException()
        End Get
        Set(value As List(Of String))
            ' TODO: Implement this property setter
            Throw New NotImplementedException()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the status text.
    ''' </summary>
    ''' <value>The status text.</value>
    Public Property StatusText() As String Implements iUINativeFormLogin.StatusText
        Get
            Return Me.StatusLabel.Text
        End Get
        Set(value As String)
            Me.StatusLabel.Text = value
        End Set
    End Property



    Private Sub CBLogin_Click(sender As Object, e As EventArgs) Handles CBLogin.Click
        If Not Me.OtdbShadow Is Nothing Then
            _ourUILogin.Username = Me.TbUsername.Text
            _ourUILogin.Password = Me.TBPassword.Text

            If _ourUILogin.Verify Then
                Me.Hide()
            Else
                Me.StatusLabel.Text = _ourUILogin.Statustext
                Me.StatusLabel.BackColor = Drawing.Color.DarkRed
                Me.StatusLabel.ForeColor = Drawing.Color.White
            End If
        End If
    End Sub

    Private Sub CbQuit_Click(sender As Object, e As EventArgs) Handles CBquit.Click
        If Not _ourUILogin Is Nothing Then
            DirectCast(Me.OtdbShadow, CoreLoginForm).Ok = False
        End If
        Me.Dispose()
    End Sub

    Public Property OtdbShadow As iOTDBUIAbstractForm Implements iUINativeForm.OtdbShadow
        Set(value As iOTDBUIAbstractForm)
            _ourUILogin = value
        End Set
        Get
            Return _ourUILogin
        End Get
    End Property

    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UIWinFormLogin))
        Me.StatusStrip = New System.Windows.Forms.StatusStrip()
        Me.StatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.CBLogin = New System.Windows.Forms.Button()
        Me.CBquit = New System.Windows.Forms.Button()
        Me.TbUsername = New System.Windows.Forms.TextBox()
        Me.TBPassword = New System.Windows.Forms.TextBox()
        Me.TBRight = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.CancelButton = New System.Windows.Forms.Button()
        Me.ButtonOk = New System.Windows.Forms.Button()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.RBMessage = New System.Windows.Forms.RichTextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.tbDomain = New System.Windows.Forms.TextBox()
        Me.StatusStrip.SuspendLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusLabel})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 332)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(331, 22)
        Me.StatusStrip.TabIndex = 0
        Me.StatusStrip.Text = "StatusStrip1"
        '
        'StatusLabel
        '
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusLabel.Size = New System.Drawing.Size(0, 17)
        '
        'CBLogin
        '
        Me.CBLogin.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CBLogin.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CBLogin.Image = CType(resources.GetObject("CBLogin.Image"), System.Drawing.Image)
        Me.CBLogin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.CBLogin.Location = New System.Drawing.Point(149, 288)
        Me.CBLogin.Name = "CBLogin"
        Me.CBLogin.Size = New System.Drawing.Size(90, 41)
        Me.CBLogin.TabIndex = 3
        Me.CBLogin.Text = "Login"
        Me.CBLogin.UseVisualStyleBackColor = True
        '
        'CBquit
        '
        Me.CBquit.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CBquit.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CBquit.Location = New System.Drawing.Point(245, 288)
        Me.CBquit.Name = "CBquit"
        Me.CBquit.Size = New System.Drawing.Size(74, 41)
        Me.CBquit.TabIndex = 4
        Me.CBquit.Text = "Cancel"
        Me.CBquit.UseVisualStyleBackColor = True
        '
        'TbUsername
        '
        Me.TbUsername.AcceptsReturn = True
        Me.TbUsername.AcceptsTab = True
        Me.TbUsername.AllowDrop = True
        Me.TbUsername.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TbUsername.Location = New System.Drawing.Point(150, 174)
        Me.TbUsername.Name = "TbUsername"
        Me.TbUsername.Size = New System.Drawing.Size(169, 24)
        Me.TbUsername.TabIndex = 1
        '
        'TBPassword
        '
        Me.TBPassword.AllowDrop = True
        Me.TBPassword.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TBPassword.Location = New System.Drawing.Point(150, 204)
        Me.TBPassword.Name = "TBPassword"
        Me.TBPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TBPassword.ShortcutsEnabled = False
        Me.TBPassword.Size = New System.Drawing.Size(169, 24)
        Me.TBPassword.TabIndex = 2
        '
        'TBRight
        '
        Me.TBRight.Enabled = False
        Me.TBRight.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TBRight.Location = New System.Drawing.Point(149, 115)
        Me.TBRight.Name = "TBRight"
        Me.TBRight.Size = New System.Drawing.Size(169, 24)
        Me.TBRight.TabIndex = 5
        Me.TBRight.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(13, 115)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(95, 18)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Access Right"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(11, 177)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(85, 18)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Username"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(11, 207)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(83, 18)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Password"
        '
        'CancelButton
        '
        Me.CancelButton.Location = New System.Drawing.Point(0, 0)
        Me.CancelButton.Name = "CancelButton"
        Me.CancelButton.Size = New System.Drawing.Size(75, 23)
        Me.CancelButton.TabIndex = 0
        '
        'ButtonOk
        '
        Me.ButtonOk.Location = New System.Drawing.Point(0, 0)
        Me.ButtonOk.Name = "ButtonOk"
        Me.ButtonOk.Size = New System.Drawing.Size(75, 23)
        Me.ButtonOk.TabIndex = 0
        '
        'PictureBox2
        '
        Me.PictureBox2.Image = CType(resources.GetObject("PictureBox2.Image"), System.Drawing.Image)
        Me.PictureBox2.Location = New System.Drawing.Point(12, 12)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(128, 91)
        Me.PictureBox2.TabIndex = 10
        Me.PictureBox2.TabStop = False
        '
        'RBMessage
        '
        Me.RBMessage.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RBMessage.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.RBMessage.Enabled = False
        Me.RBMessage.Location = New System.Drawing.Point(150, 14)
        Me.RBMessage.Name = "RBMessage"
        Me.RBMessage.Size = New System.Drawing.Size(167, 88)
        Me.RBMessage.TabIndex = 11
        Me.RBMessage.Text = String.Empty
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(13, 144)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(60, 18)
        Me.Label4.TabIndex = 13
        Me.Label4.Text = "Domain"
        '
        'tbDomain
        '
        Me.tbDomain.Enabled = False
        Me.tbDomain.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbDomain.Location = New System.Drawing.Point(149, 144)
        Me.tbDomain.Name = "tbDomain"
        Me.tbDomain.Size = New System.Drawing.Size(169, 24)
        Me.tbDomain.TabIndex = 12
        Me.tbDomain.TabStop = False
        '
        'UIWinFormLogin
        '
        Me.AcceptButton = Me.CBLogin
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(331, 354)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.tbDomain)
        Me.Controls.Add(Me.RBMessage)
        Me.Controls.Add(Me.PictureBox2)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TBRight)
        Me.Controls.Add(Me.TBPassword)
        Me.Controls.Add(Me.TbUsername)
        Me.Controls.Add(Me.CBquit)
        Me.Controls.Add(Me.CBLogin)
        Me.Controls.Add(Me.StatusStrip)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "UIWinFormLogin"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Login to OnTrack Database"
        Me.TopMost = True
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
End Class