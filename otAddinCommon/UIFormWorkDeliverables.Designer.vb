<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UIFormWorkDeliverables
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UIFormWorkDeliverables))
        Me.StatusStrip = New OnTrack.UI.UIControlStatusStrip()
        Me.CommandBar = New Telerik.WinControls.UI.RadCommandBar()
        Me.CommandBarRowElement1 = New Telerik.WinControls.UI.CommandBarRowElement()
        Me.CommandBarStripElement1 = New Telerik.WinControls.UI.CommandBarStripElement()
        Me.AddNewButton = New Telerik.WinControls.UI.CommandBarButton()
        Me.WorkPanel = New OnTrack.UI.UIControlViewWorkPanel()
        Me.CommandBarStripElement2 = New Telerik.WinControls.UI.CommandBarStripElement()
        CType(Me.CommandBar, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StatusStrip
        '
        Me.StatusStrip.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.StatusStrip.Controller = Nothing
        Me.StatusStrip.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.StatusStrip.Location = New System.Drawing.Point(0, 444)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(986, 32)
        Me.StatusStrip.TabIndex = 0
        '
        'CommandBar
        '
        Me.CommandBar.Dock = System.Windows.Forms.DockStyle.Top
        Me.CommandBar.Location = New System.Drawing.Point(0, 0)
        Me.CommandBar.Name = "CommandBar"
        Me.CommandBar.Rows.AddRange(New Telerik.WinControls.UI.CommandBarRowElement() {Me.CommandBarRowElement1})
        Me.CommandBar.Size = New System.Drawing.Size(986, 62)
        Me.CommandBar.TabIndex = 1
        Me.CommandBar.Text = "CommandBar"
        'Me.CommandBar.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'CommandBarRowElement1
        '
        Me.CommandBarRowElement1.DisabledTextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault
        Me.CommandBarRowElement1.MinSize = New System.Drawing.Size(25, 25)
        Me.CommandBarRowElement1.Strips.AddRange(New Telerik.WinControls.UI.CommandBarStripElement() {Me.CommandBarStripElement1})
        Me.CommandBarRowElement1.Text = ""
        Me.CommandBarRowElement1.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault
        '
        'CommandBarStripElement1
        '
        Me.CommandBarStripElement1.DisplayName = "CommandBarStripElement1"
        Me.CommandBarStripElement1.Items.AddRange(New Telerik.WinControls.UI.RadCommandBarBaseItem() {Me.AddNewButton})
        Me.CommandBarStripElement1.Name = "CommandBarStripElement1"
        '
        'AddNewButton
        '
        Me.AddNewButton.DisplayName = "AddNewButton"
        Me.AddNewButton.Image = Global.OnTrack.UI.My.Resources.Resources.plus
        Me.AddNewButton.Name = "AddNewButton"
        Me.AddNewButton.Text = ""
        Me.AddNewButton.ToolTipText = "Add a new deliverable"
        Me.AddNewButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'WorkPanel
        '
        Me.WorkPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WorkPanel.Location = New System.Drawing.Point(0, 62)
        Me.WorkPanel.Name = "WorkPanel"
        Me.WorkPanel.Size = New System.Drawing.Size(986, 382)
        Me.WorkPanel.TabIndex = 2
        '
        'CommandBarStripElement2
        '
        Me.CommandBarStripElement2.DisplayName = "CommandBarStripElement2"
        Me.CommandBarStripElement2.Name = "CommandBarStripElement2"
        '
        'UIFormWorkDeliverables
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(986, 476)
        Me.Controls.Add(Me.WorkPanel)
        Me.Controls.Add(Me.CommandBar)
        Me.Controls.Add(Me.StatusStrip)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "UIFormWorkDeliverables"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.Text = "Work with Deliverables"
        'Me.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        CType(Me.CommandBar, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatusStrip As OnTrack.UI.UIControlStatusStrip
    Friend WithEvents CommandBar As Telerik.WinControls.UI.RadCommandBar
    Friend WithEvents CommandBarRowElement1 As Telerik.WinControls.UI.CommandBarRowElement
    Friend WithEvents CommandBarStripElement1 As Telerik.WinControls.UI.CommandBarStripElement
    Friend WithEvents WorkPanel As OnTrack.UI.UIControlViewWorkPanel
    Friend WithEvents AddNewButton As Telerik.WinControls.UI.CommandBarButton
    Friend WithEvents CommandBarStripElement2 As Telerik.WinControls.UI.CommandBarStripElement
End Class

