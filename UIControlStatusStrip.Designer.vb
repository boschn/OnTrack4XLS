<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UIControlStatusStrip
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.StatusStrip = New Telerik.WinControls.UI.RadStatusStrip()
        Me.StatusLabel = New Telerik.WinControls.UI.RadLabelElement()
        Me.CloseStripButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.AcceptStripButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.CancelStripButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.ProgressBar = New Telerik.WinControls.UI.RadProgressBarElement()
        Me.ChamferedRectShape1 = New Telerik.WinControls.ChamferedRectShape()
        Me.EllipseShape1 = New Telerik.WinControls.EllipseShape()
        Me.MediaShape1 = New Telerik.WinControls.Tests.MediaShape()
        Me.DonutShape1 = New Telerik.WinControls.Tests.DonutShape()
        Me.TabOffice12Shape1 = New Telerik.WinControls.UI.TabOffice12Shape()
        Me.OfficeShape1 = New Telerik.WinControls.UI.OfficeShape()
        Me.DiamondShape1 = New Telerik.WinControls.UI.DiamondShape()
        Me.TrackBarLThumbShape1 = New Telerik.WinControls.UI.TrackBarLThumbShape()
        Me.TrackBarDThumbShape1 = New Telerik.WinControls.UI.TrackBarDThumbShape()
        CType(Me.StatusStrip, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StatusStrip
        '
        Me.StatusStrip.Dock = System.Windows.Forms.DockStyle.Fill
        Me.StatusStrip.Items.AddRange(New Telerik.WinControls.RadItem() {Me.StatusLabel, Me.ProgressBar, Me.AcceptStripButton, Me.CancelStripButton, Me.CloseStripButton})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 0)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(497, 32)
        Me.StatusStrip.TabIndex = 0
        Me.StatusStrip.Text = "RadStatusStrip1"
        'Me.StatusStrip.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'StatusLabel
        '
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusStrip.SetSpring(Me.StatusLabel, True)
        Me.StatusLabel.Text = ""
        Me.StatusLabel.TextWrap = True
        Me.StatusLabel.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'StripCloseButton
        '
        Me.CloseStripButton.AccessibleDescription = "Close"
        Me.CloseStripButton.AccessibleName = "Close"
        Me.CloseStripButton.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CloseStripButton.Name = "StripCloseButton"
        Me.StatusStrip.SetSpring(Me.CloseStripButton, False)
        Me.CloseStripButton.Text = "Close"
        Me.CloseStripButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'AcceptStripButton
        '
        Me.AcceptStripButton.AccessibleDescription = "Accept"
        Me.AcceptStripButton.AccessibleName = "Accept"
        Me.AcceptStripButton.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AcceptStripButton.Image = Global.OnTrack.UI.My.Resources.Resources.checkmark_16_16
        Me.AcceptStripButton.Name = "AcceptStripButton"
        Me.StatusStrip.SetSpring(Me.AcceptStripButton, False)
        Me.AcceptStripButton.Text = "Accept"
        Me.AcceptStripButton.TextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.AcceptStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.AcceptStripButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'CancelStripButton
        '
        Me.CancelStripButton.AccessibleDescription = "Cancel"
        Me.CancelStripButton.AccessibleName = "Cancel"
        Me.CancelStripButton.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CancelStripButton.Image = Global.OnTrack.UI.My.Resources.Resources.delete_16_16
        Me.CancelStripButton.Name = "CancelStripButton"
        Me.StatusStrip.SetSpring(Me.CancelStripButton, False)
        Me.CancelStripButton.Text = "Cancel"
        Me.CancelStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.CancelStripButton.ToolTipText = "Cancel Operation"
        Me.CancelStripButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'ProgressBar
        '
        Me.ProgressBar.DrawFill = True
        Me.ProgressBar.EnableImageTransparency = True
        Me.ProgressBar.Hatch = True
        Me.ProgressBar.Name = "ProgressBar"
        Me.ProgressBar.Padding = New System.Windows.Forms.Padding(-1)
        Me.ProgressBar.SeparatorColor1 = System.Drawing.Color.White
        Me.ProgressBar.SeparatorColor2 = System.Drawing.Color.White
        Me.ProgressBar.SeparatorColor3 = System.Drawing.Color.White
        Me.ProgressBar.SeparatorColor4 = System.Drawing.Color.White
        Me.ProgressBar.SeparatorGradientAngle = 0
        Me.ProgressBar.SeparatorGradientPercentage1 = 0.4!
        Me.ProgressBar.SeparatorGradientPercentage2 = 0.6!
        Me.ProgressBar.SeparatorNumberOfColors = 2
        Me.ProgressBar.Shape = Nothing
        Me.StatusStrip.SetSpring(Me.ProgressBar, False)
        Me.ProgressBar.StepWidth = 14
        Me.ProgressBar.SweepAngle = 90
        Me.ProgressBar.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'UIControlStatusStrip
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.StatusStrip)
        Me.Name = "UIControlStatusStrip"
        Me.Size = New System.Drawing.Size(497, 32)
        CType(Me.StatusStrip, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatusStrip As Telerik.WinControls.UI.RadStatusStrip
    Friend WithEvents StatusLabel As Telerik.WinControls.UI.RadLabelElement
    Friend WithEvents CloseStripButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents ProgressBar As Telerik.WinControls.UI.RadProgressBarElement
    Friend WithEvents AcceptStripButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents CancelStripButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents ChamferedRectShape1 As Telerik.WinControls.ChamferedRectShape
    Friend WithEvents EllipseShape1 As Telerik.WinControls.EllipseShape
    Friend WithEvents MediaShape1 As Telerik.WinControls.Tests.MediaShape
    Friend WithEvents DonutShape1 As Telerik.WinControls.Tests.DonutShape
    Friend WithEvents TabOffice12Shape1 As Telerik.WinControls.UI.TabOffice12Shape
    Friend WithEvents OfficeShape1 As Telerik.WinControls.UI.OfficeShape
    Friend WithEvents DiamondShape1 As Telerik.WinControls.UI.DiamondShape
    Friend WithEvents TrackBarLThumbShape1 As Telerik.WinControls.UI.TrackBarLThumbShape
    Friend WithEvents TrackBarDThumbShape1 As Telerik.WinControls.UI.TrackBarDThumbShape

End Class
