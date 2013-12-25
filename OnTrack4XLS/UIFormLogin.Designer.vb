<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UIFormLogin
    Inherits Telerik.WinControls.UI.ShapedForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UIFormLogin))
        Me.TelerikMetroBlueTheme1 = New Telerik.WinControls.Themes.TelerikMetroBlueTheme()
        Me.TbUsername = New Telerik.WinControls.UI.RadTextBox()
        Me.TbPassword = New Telerik.WinControls.UI.RadTextBox()
        Me.TbRight = New Telerik.WinControls.UI.RadTextBox()
        Me.RadLabel1 = New Telerik.WinControls.UI.RadLabel()
        Me.RadLabel2 = New Telerik.WinControls.UI.RadLabel()
        Me.RadLabel3 = New Telerik.WinControls.UI.RadLabel()
        Me.WelcomeLabel = New Telerik.WinControls.UI.RadLabel()
        Me.Status = New Telerik.WinControls.UI.RadLabel()
        Me.LoginButton = New Telerik.WinControls.UI.RadButton()
        Me.CancelButton = New Telerik.WinControls.UI.RadButton()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.RadTitleBar = New Telerik.WinControls.UI.RadTitleBar()
        CType(Me.TbUsername, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TbPassword, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TbRight, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WelcomeLabel, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Status, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LoginButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CancelButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadTitleBar, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TbUsername
        '
        Me.TbUsername.AcceptsReturn = True
        Me.TbUsername.AcceptsTab = True
        Me.TbUsername.AllowDrop = True
        Me.TbUsername.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.TbUsername.Location = New System.Drawing.Point(97, 164)
        Me.TbUsername.Name = "TbUsername"
        Me.TbUsername.Size = New System.Drawing.Size(172, 24)
        Me.TbUsername.TabIndex = 1
        Me.TbUsername.TabStop = False
        Me.TbUsername.ThemeName = "TelerikMetroBlue"
        '
        'TbPassword
        '
        Me.TbPassword.AcceptsReturn = True
        Me.TbPassword.AcceptsTab = True
        Me.TbPassword.AccessibleName = "Password"
        Me.TbPassword.AllowDrop = True
        Me.TbPassword.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.TbPassword.Location = New System.Drawing.Point(97, 194)
        Me.TbPassword.Name = "TbPassword"
        Me.TbPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TbPassword.Size = New System.Drawing.Size(172, 24)
        Me.TbPassword.TabIndex = 2
        Me.TbPassword.TabStop = False
        Me.TbPassword.ThemeName = "TelerikMetroBlue"
        '
        'TbRight
        '
        Me.TbRight.Enabled = False
        Me.TbRight.Location = New System.Drawing.Point(97, 136)
        Me.TbRight.Name = "TbRight"
        Me.TbRight.Size = New System.Drawing.Size(172, 22)
        Me.TbRight.TabIndex = 10
        Me.TbRight.TabStop = False
        Me.TbRight.ThemeName = "TelerikMetroBlue"
        '
        'RadLabel1
        '
        Me.RadLabel1.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.RadLabel1.Location = New System.Drawing.Point(12, 136)
        Me.RadLabel1.Name = "RadLabel1"
        Me.RadLabel1.Size = New System.Drawing.Size(83, 21)
        Me.RadLabel1.TabIndex = 6
        Me.RadLabel1.Text = "Access Right"
        Me.RadLabel1.ThemeName = "ControlDefault"
        '
        'RadLabel2
        '
        Me.RadLabel2.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.RadLabel2.Location = New System.Drawing.Point(29, 164)
        Me.RadLabel2.Name = "RadLabel2"
        Me.RadLabel2.Size = New System.Drawing.Size(68, 21)
        Me.RadLabel2.TabIndex = 7
        Me.RadLabel2.Text = "Username"
        Me.RadLabel2.ThemeName = "ControlDefault"
        '
        'RadLabel3
        '
        Me.RadLabel3.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.RadLabel3.Location = New System.Drawing.Point(29, 194)
        Me.RadLabel3.Name = "RadLabel3"
        Me.RadLabel3.Size = New System.Drawing.Size(64, 21)
        Me.RadLabel3.TabIndex = 8
        Me.RadLabel3.Text = "Password"
        Me.RadLabel3.ThemeName = "ControlDefault"
        '
        'WelcomeLabel
        '
        Me.WelcomeLabel.AutoSize = False
        Me.WelcomeLabel.Location = New System.Drawing.Point(128, 45)
        Me.WelcomeLabel.Name = "WelcomeLabel"
        Me.WelcomeLabel.Size = New System.Drawing.Size(170, 72)
        Me.WelcomeLabel.TabIndex = 10
        Me.WelcomeLabel.Text = "<html><strong>Welcome !</strong><br />Please enter your Username and Password to " & _
    "obtain access to the OnTrack Database.</html>"
        Me.WelcomeLabel.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        CType(Me.WelcomeLabel.GetChildAt(0), Telerik.WinControls.UI.RadLabelElement).TextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        CType(Me.WelcomeLabel.GetChildAt(0), Telerik.WinControls.UI.RadLabelElement).Text = "<html><strong>Welcome !</strong><br />Please enter your Username and Password to " & _
    "obtain access to the OnTrack Database.</html>"
        CType(Me.WelcomeLabel.GetChildAt(0).GetChildAt(2), Telerik.WinControls.Layouts.ImageAndTextLayoutPanel).TextAlignment = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Status
        '
        Me.Status.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Status.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Status.Location = New System.Drawing.Point(0, 348)
        Me.Status.Name = "Status"
        Me.Status.Size = New System.Drawing.Size(2, 2)
        Me.Status.TabIndex = 11
        '
        'LoginButton
        '
        Me.LoginButton.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LoginButton.Image = Global.OnTrackTool.My.Resources.Resources.connect_icon
        Me.LoginButton.Location = New System.Drawing.Point(97, 246)
        Me.LoginButton.Name = "LoginButton"
        Me.LoginButton.Size = New System.Drawing.Size(77, 45)
        Me.LoginButton.TabIndex = 3
        Me.LoginButton.Text = "Login"
        Me.LoginButton.ThemeName = "TelerikMetroBlue"
        '
        'CancelButton
        '
        Me.CancelButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Text
        Me.CancelButton.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CancelButton.Image = Global.OnTrackTool.My.Resources.Resources.connect_icon
        Me.CancelButton.Location = New System.Drawing.Point(192, 246)
        Me.CancelButton.Name = "CancelButton"
        Me.CancelButton.Size = New System.Drawing.Size(77, 45)
        Me.CancelButton.TabIndex = 4
        Me.CancelButton.Text = "Cancel"
        Me.CancelButton.ThemeName = "TelerikMetroBlue"
        '
        'PictureBox2
        '
        Me.PictureBox2.Image = Global.OnTrackTool.My.Resources.Resources.fasttrack
        Me.PictureBox2.Location = New System.Drawing.Point(12, 45)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(110, 72)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.PictureBox2.TabIndex = 5
        Me.PictureBox2.TabStop = False
        '
        'RadTitleBar
        '
        Me.RadTitleBar.AllowResize = False
        Me.RadTitleBar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RadTitleBar.Location = New System.Drawing.Point(12, 4)
        Me.RadTitleBar.Name = "RadTitleBar"
        Me.RadTitleBar.Size = New System.Drawing.Size(296, 35)
        Me.RadTitleBar.TabIndex = 12
        Me.RadTitleBar.TabStop = False
        Me.RadTitleBar.Text = "Add-In Login to the On Track Database"
        Me.RadTitleBar.ThemeName = "TelerikMetroBlue"
        '
        'UIFormLogin
        '
        Me.AcceptButton = Me.LoginButton
        Me.AllowResize = False
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.BorderColor = System.Drawing.Color.BurlyWood
        Me.ClientSize = New System.Drawing.Size(320, 350)
        Me.ControlBox = False
        Me.Controls.Add(Me.RadTitleBar)
        Me.Controls.Add(Me.Status)
        Me.Controls.Add(Me.CancelButton)
        Me.Controls.Add(Me.LoginButton)
        Me.Controls.Add(Me.WelcomeLabel)
        Me.Controls.Add(Me.RadLabel3)
        Me.Controls.Add(Me.RadLabel2)
        Me.Controls.Add(Me.RadLabel1)
        Me.Controls.Add(Me.PictureBox2)
        Me.Controls.Add(Me.TbRight)
        Me.Controls.Add(Me.TbPassword)
        Me.Controls.Add(Me.TbUsername)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(320, 350)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(320, 350)
        Me.Name = "UIFormLogin"
        Me.Opacity = 0.95R
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Add-In Login to the On Track Database"
        Me.ThemeName = "TelerikMetroBlue"
        Me.TopMost = True
        CType(Me.TbUsername, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TbPassword, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TbRight, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WelcomeLabel, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Status, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LoginButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CancelButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadTitleBar, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents RoundRectShapeForm As Telerik.WinControls.RoundRectShape
    Friend WithEvents TelerikMetroBlueTheme1 As Telerik.WinControls.Themes.TelerikMetroBlueTheme
    Friend WithEvents TbUsername As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents TbPassword As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents TbRight As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
    Friend WithEvents RadLabel1 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadLabel2 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadLabel3 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents WelcomeLabel As Telerik.WinControls.UI.RadLabel
    Friend WithEvents EllipseShape1 As Telerik.WinControls.EllipseShape
    Friend WithEvents OfficeShape1 As Telerik.WinControls.UI.OfficeShape

    Friend WithEvents LoginButton As Telerik.WinControls.UI.RadButton
    Friend WithEvents CancelButton As Telerik.WinControls.UI.RadButton
    Friend WithEvents Status As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadTitleBar As Telerik.WinControls.UI.RadTitleBar
End Class

