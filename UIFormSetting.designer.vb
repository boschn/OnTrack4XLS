<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UIFormSetting
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UIFormSetting))
        Me.Office2013LightTheme1 = New Telerik.WinControls.Themes.Office2013LightTheme()
        Me.RadPropertyGrid = New Telerik.WinControls.UI.RadPropertyGrid()
        Me.RadStatusStrip1 = New Telerik.WinControls.UI.RadStatusStrip()
        Me.StatusLabel = New Telerik.WinControls.UI.RadLabelElement()
        Me.SaveButton = New Telerik.WinControls.UI.RadSplitButtonElement()
        Me.SaveInSessionMenuButton = New Telerik.WinControls.UI.RadMenuItem()
        Me.SaveDocumentMenuButton = New Telerik.WinControls.UI.RadMenuItem()
        Me.SaveConfigFileMenuButton = New Telerik.WinControls.UI.RadMenuItem()
        Me.CancelButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.RadPanel1 = New Telerik.WinControls.UI.RadPanel()
        Me.InitializeDataButton = New Telerik.WinControls.UI.RadButton()
        Me.DropDatabaseButton = New Telerik.WinControls.UI.RadButton()
        Me.ButtonCreateSchema = New Telerik.WinControls.UI.RadButton()
        Me.RadOffice2007ScreenTipElement1 = New Telerik.WinControls.UI.RadOffice2007ScreenTipElement()
        CType(Me.RadPropertyGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadStatusStrip1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadPanel1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RadPanel1.SuspendLayout()
        CType(Me.InitializeDataButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DropDatabaseButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ButtonCreateSchema, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RadPropertyGrid
        '
        Me.RadPropertyGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RadPropertyGrid.HelpBarHeight = 40.0!
        Me.RadPropertyGrid.ItemHeight = 21
        Me.RadPropertyGrid.Location = New System.Drawing.Point(0, 38)
        Me.RadPropertyGrid.Name = "RadPropertyGrid"
        Me.RadPropertyGrid.Size = New System.Drawing.Size(445, 216)
        Me.RadPropertyGrid.SortOrder = System.Windows.Forms.SortOrder.Ascending
        Me.RadPropertyGrid.TabIndex = 0
        Me.RadPropertyGrid.Text = "RadPropertyGrid1"
        Me.RadPropertyGrid.ToolbarVisible = True
        '
        'RadStatusStrip1
        '
        Me.RadStatusStrip1.Items.AddRange(New Telerik.WinControls.RadItem() {Me.StatusLabel, Me.SaveButton, Me.CancelButton})
        Me.RadStatusStrip1.Location = New System.Drawing.Point(0, 266)
        Me.RadStatusStrip1.Name = "RadStatusStrip1"
        Me.RadStatusStrip1.Size = New System.Drawing.Size(442, 26)
        Me.RadStatusStrip1.TabIndex = 1
        Me.RadStatusStrip1.Text = "RadStatusStrip1"
        '
        'StatusLabel
        '
        Me.StatusLabel.Name = "StatusLabel"
        Me.RadStatusStrip1.SetSpring(Me.StatusLabel, True)
        Me.StatusLabel.StretchHorizontally = True
        Me.StatusLabel.StretchVertically = True
        Me.StatusLabel.Text = ""
        Me.StatusLabel.TextWrap = True
        Me.StatusLabel.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'SaveButton
        '
        Me.SaveButton.AccessibleDescription = "Save"
        Me.SaveButton.AccessibleName = "Save"
        Me.SaveButton.ArrowButtonMinSize = New System.Drawing.Size(12, 12)
        Me.SaveButton.DefaultItem = Nothing
        Me.SaveButton.DropDownDirection = Telerik.WinControls.UI.RadDirection.Down
        Me.SaveButton.ExpandArrowButton = False
        Me.SaveButton.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SaveButton.Image = Nothing
        Me.SaveButton.Items.AddRange(New Telerik.WinControls.RadItem() {Me.SaveInSessionMenuButton, Me.SaveDocumentMenuButton, Me.SaveConfigFileMenuButton})
        Me.SaveButton.Name = "SaveButton"
        Me.RadStatusStrip1.SetSpring(Me.SaveButton, False)
        Me.SaveButton.Text = "Save"
        Me.SaveButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'SaveInSessionMenuButton
        '
        Me.SaveInSessionMenuButton.AccessibleDescription = "in CurrentSession"
        Me.SaveInSessionMenuButton.AccessibleName = "in CurrentSession"
        Me.SaveInSessionMenuButton.DescriptionText = "save in current session temporarily"
        Me.SaveInSessionMenuButton.Name = "SaveInSessionMenuButton"
        Me.SaveInSessionMenuButton.Text = "in current Session"
        Me.SaveInSessionMenuButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'SaveDocumentMenuButton
        '
        Me.SaveDocumentMenuButton.AccessibleDescription = "in Document"
        Me.SaveDocumentMenuButton.AccessibleName = "in Document"
        Me.SaveDocumentMenuButton.DescriptionText = "save in document properties"
        Me.SaveDocumentMenuButton.Name = "SaveDocumentMenuButton"
        Me.SaveDocumentMenuButton.Text = "in Document"
        Me.SaveDocumentMenuButton.ToolTipText = "Save configuration properties in document itself"
        Me.SaveDocumentMenuButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'SaveConfigFileMenuButton
        '
        Me.SaveConfigFileMenuButton.AccessibleDescription = "in Config File"
        Me.SaveConfigFileMenuButton.AccessibleName = "in Config File"
        Me.SaveConfigFileMenuButton.DescriptionText = "save configuration in configuration file"
        Me.SaveConfigFileMenuButton.Name = "SaveConfigFileMenuButton"
        Me.SaveConfigFileMenuButton.Text = "in Config File"
        Me.SaveConfigFileMenuButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'CancelButton
        '
        Me.CancelButton.AccessibleDescription = "Cancel"
        Me.CancelButton.AccessibleName = "Cancel"
        Me.CancelButton.Name = "CancelButton"
        Me.RadStatusStrip1.SetSpring(Me.CancelButton, False)
        Me.CancelButton.Text = "Cancel"
        Me.CancelButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'RadPanel1
        '
        Me.RadPanel1.Controls.Add(Me.InitializeDataButton)
        Me.RadPanel1.Controls.Add(Me.DropDatabaseButton)
        Me.RadPanel1.Controls.Add(Me.ButtonCreateSchema)
        Me.RadPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.RadPanel1.Location = New System.Drawing.Point(0, 0)
        Me.RadPanel1.Name = "RadPanel1"
        Me.RadPanel1.Size = New System.Drawing.Size(442, 32)
        Me.RadPanel1.TabIndex = 2
        '
        'InitializeDataButton
        '
        Me.InitializeDataButton.Location = New System.Drawing.Point(118, 5)
        Me.InitializeDataButton.Name = "InitializeDataButton"
        Me.InitializeDataButton.Size = New System.Drawing.Size(110, 24)
        Me.InitializeDataButton.TabIndex = 2
        Me.InitializeDataButton.Text = "Initialize Database"
        '
        'DropDatabaseButton
        '
        Me.DropDatabaseButton.Location = New System.Drawing.Point(234, 5)
        Me.DropDatabaseButton.Name = "DropDatabaseButton"
        Me.DropDatabaseButton.Size = New System.Drawing.Size(110, 24)
        Me.DropDatabaseButton.TabIndex = 1
        Me.DropDatabaseButton.Text = "Drop Database"
        '
        'ButtonCreateSchema
        '
        Me.ButtonCreateSchema.Location = New System.Drawing.Point(3, 5)
        Me.ButtonCreateSchema.Name = "ButtonCreateSchema"
        Me.ButtonCreateSchema.Size = New System.Drawing.Size(109, 24)
        Me.ButtonCreateSchema.TabIndex = 0
        Me.ButtonCreateSchema.Text = "Create Database"
        '
        'RadOffice2007ScreenTipElement1
        '
        Me.RadOffice2007ScreenTipElement1.Description = "Override this property and provide custom screentip template description in Desig" & _
    "nTime."
        Me.RadOffice2007ScreenTipElement1.Name = "RadOffice2007ScreenTipElement1"
        Me.RadOffice2007ScreenTipElement1.TemplateType = Nothing
        Me.RadOffice2007ScreenTipElement1.TipSize = New System.Drawing.Size(210, 50)
        Me.RadOffice2007ScreenTipElement1.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'UIFormSetting
        '
        Me.AcceptButton = Me.SaveInSessionMenuButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(442, 292)
        Me.Controls.Add(Me.RadPanel1)
        Me.Controls.Add(Me.RadStatusStrip1)
        Me.Controls.Add(Me.RadPropertyGrid)
        Me.HelpButton = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(350, 200)
        Me.Name = "UIFormSetting"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.Text = "OnTrack Property Setting"
        CType(Me.RadPropertyGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadStatusStrip1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadPanel1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RadPanel1.ResumeLayout(False)
        CType(Me.InitializeDataButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DropDatabaseButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ButtonCreateSchema, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Office2013LightTheme1 As Telerik.WinControls.Themes.Office2013LightTheme
    Friend WithEvents RadPropertyGrid As Telerik.WinControls.UI.RadPropertyGrid
    Friend WithEvents RadStatusStrip1 As Telerik.WinControls.UI.RadStatusStrip
    Friend WithEvents StatusLabel As Telerik.WinControls.UI.RadLabelElement
    Friend WithEvents CancelButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents SaveButton As Telerik.WinControls.UI.RadSplitButtonElement
    Friend WithEvents SaveDocumentMenuButton As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents RadPanel1 As Telerik.WinControls.UI.RadPanel
    Friend WithEvents ButtonCreateSchema As Telerik.WinControls.UI.RadButton
    Friend WithEvents SaveInSessionMenuButton As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents RadOffice2007ScreenTipElement1 As Telerik.WinControls.UI.RadOffice2007ScreenTipElement
    Friend WithEvents SaveConfigFileMenuButton As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents DropDatabaseButton As Telerik.WinControls.UI.RadButton
    Friend WithEvents InitializeDataButton As Telerik.WinControls.UI.RadButton
End Class

