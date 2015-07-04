<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UIForm
    Inherits Telerik.WinControls.UI.RadForm

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
        Me.Office2013LightTheme1 = New Telerik.WinControls.Themes.Office2013LightTheme()
        Me.StatusStrip = New OnTrack.UI.UIControlStatusStrip()
        Me.RadCommandBar1 = New Telerik.WinControls.UI.RadCommandBar()
        Me.CommandBarRowElement1 = New Telerik.WinControls.UI.CommandBarRowElement()
        Me.CommandBarStripElement1 = New Telerik.WinControls.UI.CommandBarStripElement()
        Me.UiControlViewWorkPanel1 = New OnTrack.UI.UIControlViewWorkPanel()
        CType(Me.RadCommandBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StatusStrip
        '
        Me.StatusStrip.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.StatusStrip.Controller = Nothing
        Me.StatusStrip.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.StatusStrip.Location = New System.Drawing.Point(0, 308)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(735, 32)
        Me.StatusStrip.TabIndex = 0
        '
        'RadCommandBar1
        '
        Me.RadCommandBar1.Dock = System.Windows.Forms.DockStyle.Top
        Me.RadCommandBar1.Location = New System.Drawing.Point(0, 0)
        Me.RadCommandBar1.Name = "RadCommandBar1"
        Me.RadCommandBar1.Rows.AddRange(New Telerik.WinControls.UI.CommandBarRowElement() {Me.CommandBarRowElement1})
        Me.RadCommandBar1.Size = New System.Drawing.Size(735, 33)
        Me.RadCommandBar1.TabIndex = 1
        Me.RadCommandBar1.Text = "RadCommandBar1"
        'Me.RadCommandBar1.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'CommandBarRowElement1
        '
        Me.CommandBarRowElement1.MinSize = New System.Drawing.Size(25, 25)
        Me.CommandBarRowElement1.Strips.AddRange(New Telerik.WinControls.UI.CommandBarStripElement() {Me.CommandBarStripElement1})
        '
        'CommandBarStripElement1
        '
        Me.CommandBarStripElement1.DisplayName = "CommandBarStripElement1"
        Me.CommandBarStripElement1.Name = "CommandBarStripElement1"
        '
        'UiControlViewWorkPanel1
        '
        Me.UiControlViewWorkPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UiControlViewWorkPanel1.Location = New System.Drawing.Point(0, 33)
        Me.UiControlViewWorkPanel1.Name = "UiControlViewWorkPanel1"
        Me.UiControlViewWorkPanel1.Size = New System.Drawing.Size(735, 275)
        Me.UiControlViewWorkPanel1.TabIndex = 2
        '
        'UIForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(735, 340)
        Me.Controls.Add(Me.UiControlViewWorkPanel1)
        Me.Controls.Add(Me.RadCommandBar1)
        Me.Controls.Add(Me.StatusStrip)
        Me.Name = "UIForm"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.Text = "UIForm"
        'Me.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        CType(Me.RadCommandBar1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Office2013LightTheme1 As Telerik.WinControls.Themes.Office2013LightTheme
    Friend WithEvents StatusStrip As OnTrack.UI.UIControlStatusStrip
    Friend WithEvents RadCommandBar1 As Telerik.WinControls.UI.RadCommandBar
    Friend WithEvents CommandBarRowElement1 As Telerik.WinControls.UI.CommandBarRowElement
    Friend WithEvents CommandBarStripElement1 As Telerik.WinControls.UI.CommandBarStripElement
    Friend WithEvents UiControlViewWorkPanel1 As OnTrack.UI.UIControlViewWorkPanel
End Class

