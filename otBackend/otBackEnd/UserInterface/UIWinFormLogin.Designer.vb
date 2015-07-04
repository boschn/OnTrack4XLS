<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UIWinFormLogin
    Inherits System.Windows.Forms.Form

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
    Private Sub InitializeallComponent()
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

        Me.StatusStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusLabel})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 290)
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
        Me.CBLogin.Location = New System.Drawing.Point(149, 246)
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
        Me.CBquit.Location = New System.Drawing.Point(245, 246)
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
        Me.TbUsername.Location = New System.Drawing.Point(149, 145)
        Me.TbUsername.Name = "TbUsername"
        Me.TbUsername.Size = New System.Drawing.Size(169, 24)
        Me.TbUsername.TabIndex = 1
        '
        'TBPassword
        '
        Me.TBPassword.AllowDrop = True
        Me.TBPassword.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TBPassword.Location = New System.Drawing.Point(150, 175)
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
        Me.Label2.Location = New System.Drawing.Point(13, 148)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(85, 18)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Username"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(13, 178)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(83, 18)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Password"
       
        '
        'clsUIWinFormLoginV2
        '
        Me.AcceptButton = Me.CBLogin
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.CBquit
        Me.ClientSize = New System.Drawing.Size(331, 312)
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
        Me.Name = "clsUIWinFormLoginV2"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Login to OnTrack Database"
        Me.TopMost = True
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()

        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents StatusLabel As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents CBLogin As System.Windows.Forms.Button
    Friend WithEvents CBquit As System.Windows.Forms.Button
    Friend WithEvents TbUsername As System.Windows.Forms.TextBox
    Friend WithEvents TBPassword As System.Windows.Forms.TextBox
    Friend WithEvents TBRight As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
    Friend WithEvents RBMessage As System.Windows.Forms.RichTextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents tbDomain As System.Windows.Forms.TextBox
End Class
