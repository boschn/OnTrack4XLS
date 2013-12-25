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
        Me.TelerikMetroBlueTheme1 = New Telerik.WinControls.Themes.TelerikMetroBlueTheme()
        Me.RadPropertyGrid = New Telerik.WinControls.UI.RadPropertyGrid()
        Me.RadStatusStrip1 = New Telerik.WinControls.UI.RadStatusStrip()
        Me.StatusLabel = New Telerik.WinControls.UI.RadLabelElement()
        Me.CreateSchemaButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.SaveButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.CancelButton = New Telerik.WinControls.UI.RadButtonElement()
        CType(Me.RadPropertyGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadStatusStrip1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RadPropertyGrid
        '
        Me.RadPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RadPropertyGrid.ItemHeight = 21
        Me.RadPropertyGrid.Location = New System.Drawing.Point(0, 0)
        Me.RadPropertyGrid.Name = "RadPropertyGrid"
        Me.RadPropertyGrid.Size = New System.Drawing.Size(308, 292)
        Me.RadPropertyGrid.TabIndex = 0
        Me.RadPropertyGrid.Text = "RadPropertyGrid1"
        Me.RadPropertyGrid.ThemeName = "TelerikMetroBlue"
        Me.RadPropertyGrid.ToolbarVisible = True
        '
        'RadStatusStrip1
        '
        Me.RadStatusStrip1.AutoSize = True
        Me.RadStatusStrip1.Items.AddRange(New Telerik.WinControls.RadItem() {Me.StatusLabel, Me.CreateSchemaButton, Me.SaveButton, Me.CancelButton})
        Me.RadStatusStrip1.LayoutStyle = Telerik.WinControls.UI.RadStatusBarLayoutStyle.Stack
        Me.RadStatusStrip1.Location = New System.Drawing.Point(0, 263)
        Me.RadStatusStrip1.Name = "RadStatusStrip1"
        Me.RadStatusStrip1.Size = New System.Drawing.Size(308, 29)
        Me.RadStatusStrip1.TabIndex = 1
        Me.RadStatusStrip1.Text = "RadStatusStrip1"
        Me.RadStatusStrip1.ThemeName = "TelerikMetroBlue"
        '
        'StatusLabel
        '
        Me.StatusLabel.Name = "StatusLabel"
        Me.RadStatusStrip1.SetSpring(Me.StatusLabel, True)
        Me.StatusLabel.Text = ""
        Me.StatusLabel.TextWrap = True
        Me.StatusLabel.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'CreateSchemaButton
        '
        Me.CreateSchemaButton.AccessibleDescription = "Create Schema"
        Me.CreateSchemaButton.AccessibleName = "Create Schema"
        Me.CreateSchemaButton.Name = "CreateSchemaButton"
        Me.RadStatusStrip1.SetSpring(Me.CreateSchemaButton, False)
        Me.CreateSchemaButton.Text = "Create Schema"
        Me.CreateSchemaButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'SaveButton
        '
        Me.SaveButton.AccessibleDescription = "Save"
        Me.SaveButton.AccessibleName = "Save"
        Me.SaveButton.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.SaveButton.Name = "SaveButton"
        Me.RadStatusStrip1.SetSpring(Me.SaveButton, False)
        Me.SaveButton.Text = "Save"
        Me.SaveButton.ToolTipText = "Save properties"
        Me.SaveButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
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
        'UIFormSetting
        '
        Me.AcceptButton = Me.SaveButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(308, 292)
        Me.Controls.Add(Me.RadStatusStrip1)
        Me.Controls.Add(Me.RadPropertyGrid)
        Me.HelpButton = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "UIFormSetting"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.Text = "OnTrack Property Setting"
        Me.ThemeName = "TelerikMetroBlue"
        CType(Me.RadPropertyGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadStatusStrip1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TelerikMetroBlueTheme1 As Telerik.WinControls.Themes.TelerikMetroBlueTheme
    Friend WithEvents RadPropertyGrid As Telerik.WinControls.UI.RadPropertyGrid
    Friend WithEvents RadStatusStrip1 As Telerik.WinControls.UI.RadStatusStrip
    Friend WithEvents StatusLabel As Telerik.WinControls.UI.RadLabelElement
    Friend WithEvents SaveButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents CancelButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents CreateSchemaButton As Telerik.WinControls.UI.RadButtonElement
End Class

