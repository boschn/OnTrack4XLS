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
        Me.Office2013LightTheme1 = New Telerik.WinControls.Themes.Office2013LightTheme()
        Me.StatusStrip = New Telerik.WinControls.UI.RadStatusStrip()
        Me.StatusLabel = New Telerik.WinControls.UI.RadLabelElement()
        Me.CloseButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.ClearToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RefreshToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GridView = New Telerik.WinControls.UI.RadGridView()
        CType(Me.StatusStrip, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.GridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridView.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New Telerik.WinControls.RadItem() {Me.StatusLabel, Me.CloseButton})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 201)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(915, 30)
        Me.StatusStrip.TabIndex = 0
        '
        'StatusLabel
        '
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusStrip.SetSpring(Me.StatusLabel, True)
        Me.StatusLabel.Text = ""
        Me.StatusLabel.TextWrap = True
        Me.StatusLabel.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'CloseButton
        '
        Me.CloseButton.AccessibleDescription = "Close"
        Me.CloseButton.AccessibleName = "Close"
        Me.CloseButton.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CloseButton.Name = "CloseButton"
        Me.StatusStrip.SetSpring(Me.CloseButton, False)
        Me.CloseButton.Text = "Close"
        Me.CloseButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ClearToolStripMenuItem, Me.RefreshToolStripMenuItem, Me.ExportStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
        Me.MenuStrip1.Size = New System.Drawing.Size(915, 24)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'ClearToolStripMenuItem
        '
        Me.ClearToolStripMenuItem.Name = "ClearToolStripMenuItem"
        Me.ClearToolStripMenuItem.Size = New System.Drawing.Size(46, 20)
        Me.ClearToolStripMenuItem.Text = "Clear"
        '
        'RefreshToolStripMenuItem
        '
        Me.RefreshToolStripMenuItem.Name = "RefreshToolStripMenuItem"
        Me.RefreshToolStripMenuItem.Size = New System.Drawing.Size(58, 20)
        Me.RefreshToolStripMenuItem.Text = "Refresh"
        '
        'ExportStripMenuItem
        '
        Me.ExportStripMenuItem.Name = "ExportStripMenuItem"
        Me.ExportStripMenuItem.Size = New System.Drawing.Size(52, 20)
        Me.ExportStripMenuItem.Text = "Export"
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
        Me.GridView.ReadOnly = True
        Me.GridView.Size = New System.Drawing.Size(915, 177)
        Me.GridView.TabIndex = 2
        Me.GridView.Text = "RadGridView1"
        '
        'UIFormMessageLog
        '
        Me.AcceptButton = Me.CloseButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.CloseButton
        Me.ClientSize = New System.Drawing.Size(915, 231)
        Me.Controls.Add(Me.GridView)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.MenuStrip1)
        Me.HelpButton = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MinimumSize = New System.Drawing.Size(600, 200)
        Me.Name = "UIFormMessageLog"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.Text = "Show Message Log"
        CType(Me.StatusStrip, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.GridView.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Office2013LightTheme1 As Telerik.WinControls.Themes.Office2013LightTheme
    Friend WithEvents StatusStrip As Telerik.WinControls.UI.RadStatusStrip
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents ClearToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusLabel As Telerik.WinControls.UI.RadLabelElement
    Friend WithEvents CloseButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents GridView As Telerik.WinControls.UI.RadGridView
    Friend WithEvents RefreshToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExportStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class

