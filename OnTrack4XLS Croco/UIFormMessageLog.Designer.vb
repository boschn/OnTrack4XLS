<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UIFormMessageLog
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
        Dim SortDescriptor1 As Telerik.WinControls.Data.SortDescriptor = New Telerik.WinControls.Data.SortDescriptor()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UIFormMessageLog))
        Me.TelerikMetroBlueTheme1 = New Telerik.WinControls.Themes.TelerikMetroBlueTheme()
        Me.StatusStrip = New Telerik.WinControls.UI.RadStatusStrip()
        Me.Label = New Telerik.WinControls.UI.RadLabelElement()
        Me.CloseButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.ClearToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GridView = New Telerik.WinControls.UI.RadGridView()
        CType(Me.StatusStrip,System.ComponentModel.ISupportInitialize).BeginInit
        Me.MenuStrip1.SuspendLayout
        CType(Me.GridView,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.GridView.MasterTemplate,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me,System.ComponentModel.ISupportInitialize).BeginInit
        Me.SuspendLayout
        '
        'StatusStrip
        '
        Me.StatusStrip.AutoSize = true
        Me.StatusStrip.Items.AddRange(New Telerik.WinControls.RadItem() {Me.Label, Me.CloseButton})
        Me.StatusStrip.LayoutStyle = Telerik.WinControls.UI.RadStatusBarLayoutStyle.Stack
        Me.StatusStrip.Location = New System.Drawing.Point(0, 135)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(634, 32)
        Me.StatusStrip.TabIndex = 0
        Me.StatusStrip.ThemeName = "TelerikMetroBlue"
        '
        'Label
        '
        Me.Label.Name = "Label"
        Me.StatusStrip.SetSpring(Me.Label, true)
        Me.Label.Text = ""
        Me.Label.TextWrap = true
        Me.Label.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'CloseButton
        '
        Me.CloseButton.AccessibleDescription = "Close"
        Me.CloseButton.AccessibleName = "Close"
        Me.CloseButton.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.CloseButton.Name = "CloseButton"
        Me.StatusStrip.SetSpring(Me.CloseButton, false)
        Me.CloseButton.Text = "Close"
        Me.CloseButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ClearToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
        Me.MenuStrip1.Size = New System.Drawing.Size(634, 24)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'ClearToolStripMenuItem
        '
        Me.ClearToolStripMenuItem.Name = "ClearToolStripMenuItem"
        Me.ClearToolStripMenuItem.Size = New System.Drawing.Size(46, 20)
        Me.ClearToolStripMenuItem.Text = "Clear"
        '
        'GridView
        '
        Me.GridView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GridView.EnableFastScrolling = True
        Me.GridView.Location = New System.Drawing.Point(0, 24)
        '
        'GridView
        '
        Me.GridView.MasterTemplate.AllowAddNewRow = False
        Me.GridView.MasterTemplate.AllowColumnChooser = False
        Me.GridView.MasterTemplate.AllowDeleteRow = False
        Me.GridView.MasterTemplate.AllowEditRow = False
        Me.GridView.MasterTemplate.EnableAlternatingRowColor = True
        Me.GridView.MasterTemplate.EnableFiltering = True
        Me.GridView.MasterTemplate.EnableGrouping = False
        Me.GridView.MasterTemplate.ShowFilteringRow = False
        SortDescriptor1.PropertyName = "no"
        Me.GridView.MasterTemplate.SortDescriptors.AddRange(New Telerik.WinControls.Data.SortDescriptor() {SortDescriptor1})
        Me.GridView.Name = "GridView"
        Me.GridView.ReadOnly = true
        Me.GridView.Size = New System.Drawing.Size(634, 111)
        Me.GridView.TabIndex = 2
        Me.GridView.Text = "RadGridView1"
        Me.GridView.ThemeName = "TelerikMetroBlue"
        '
        'UIFormMessageLog
        '
        Me.AcceptButton = Me.CloseButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.CloseButton
        Me.ClientSize = New System.Drawing.Size(634, 167)
        Me.Controls.Add(Me.GridView)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.MenuStrip1)
        Me.HelpButton = true
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MinimumSize = New System.Drawing.Size(600, 200)
        Me.Name = "UIFormMessageLog"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = true
        Me.Text = "Show Message Log"
        Me.ThemeName = "TelerikMetroBlue"
        CType(Me.StatusStrip,System.ComponentModel.ISupportInitialize).EndInit
        Me.MenuStrip1.ResumeLayout(false)
        Me.MenuStrip1.PerformLayout
        CType(Me.GridView.MasterTemplate,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.GridView,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me,System.ComponentModel.ISupportInitialize).EndInit
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents TelerikMetroBlueTheme1 As Telerik.WinControls.Themes.TelerikMetroBlueTheme
    Friend WithEvents StatusStrip As Telerik.WinControls.UI.RadStatusStrip
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents ClearToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Label As Telerik.WinControls.UI.RadLabelElement
    Friend WithEvents CloseButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents GridView As Telerik.WinControls.UI.RadGridView
End Class

