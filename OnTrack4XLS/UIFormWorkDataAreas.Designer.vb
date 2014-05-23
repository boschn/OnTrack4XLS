<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UIFormWorkDataAreas
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UIFormWorkDataAreas))
        Me.TelerikMetroBlueTheme1 = New Telerik.WinControls.Themes.TelerikMetroBlueTheme()
        Me.RadStatusStrip1 = New Telerik.WinControls.UI.RadStatusStrip()
        Me.RadLabelElement1 = New Telerik.WinControls.UI.RadLabelElement()
        Me.CancelButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.SaveButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.RadSplitContainer1 = New Telerik.WinControls.UI.RadSplitContainer()
        Me.SplitPanel1 = New Telerik.WinControls.UI.SplitPanel()
        Me.DataAreaListControl = New Telerik.WinControls.UI.RadListControl()
        Me.SplitPanel2 = New Telerik.WinControls.UI.SplitPanel()
        Me.DataAreaPropertyGrid = New Telerik.WinControls.UI.RadPropertyGrid()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.AddDataAreaButton = New Telerik.WinControls.UI.RadButton()
        CType(Me.RadStatusStrip1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadSplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RadSplitContainer1.SuspendLayout()
        CType(Me.SplitPanel1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitPanel1.SuspendLayout()
        CType(Me.DataAreaListControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitPanel2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitPanel2.SuspendLayout()
        CType(Me.DataAreaPropertyGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        CType(Me.AddDataAreaButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RadStatusStrip1
        '
        Me.RadStatusStrip1.Items.AddRange(New Telerik.WinControls.RadItem() {Me.RadLabelElement1, Me.CancelButton, Me.SaveButton})
        Me.RadStatusStrip1.Location = New System.Drawing.Point(0, 375)
        Me.RadStatusStrip1.Name = "RadStatusStrip1"
        Me.RadStatusStrip1.Size = New System.Drawing.Size(485, 29)
        Me.RadStatusStrip1.TabIndex = 0
        Me.RadStatusStrip1.Text = "RadStatusStrip1"
        Me.RadStatusStrip1.ThemeName = "TelerikMetroBlue"
        '
        'RadLabelElement1
        '
        Me.RadLabelElement1.Name = "RadLabelElement1"
        Me.RadStatusStrip1.SetSpring(Me.RadLabelElement1, True)
        Me.RadLabelElement1.Text = ""
        Me.RadLabelElement1.TextWrap = True
        Me.RadLabelElement1.Visibility = Telerik.WinControls.ElementVisibility.Visible
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
        'SaveButton
        '
        Me.SaveButton.AccessibleDescription = "Save"
        Me.SaveButton.AccessibleName = "Save"
        Me.SaveButton.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.SaveButton.Name = "SaveButton"
        Me.RadStatusStrip1.SetSpring(Me.SaveButton, False)
        Me.SaveButton.Text = "Save"
        Me.SaveButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'RadSplitContainer1
        '
        Me.RadSplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RadSplitContainer1.Controls.Add(Me.SplitPanel1)
        Me.RadSplitContainer1.Controls.Add(Me.SplitPanel2)
        Me.RadSplitContainer1.Location = New System.Drawing.Point(0, 36)
        Me.RadSplitContainer1.Name = "RadSplitContainer1"
        '
        '
        '
        Me.RadSplitContainer1.RootElement.MinSize = New System.Drawing.Size(25, 25)
        Me.RadSplitContainer1.Size = New System.Drawing.Size(485, 339)
        Me.RadSplitContainer1.TabIndex = 2
        Me.RadSplitContainer1.TabStop = False
        Me.RadSplitContainer1.Text = "RadSplitContainer1"
        Me.RadSplitContainer1.ThemeName = "TelerikMetroBlue"
        '
        'SplitPanel1
        '
        Me.SplitPanel1.Controls.Add(Me.DataAreaListControl)
        Me.SplitPanel1.Location = New System.Drawing.Point(0, 0)
        Me.SplitPanel1.Name = "SplitPanel1"
        '
        '
        '
        Me.SplitPanel1.RootElement.MinSize = New System.Drawing.Size(25, 25)
        Me.SplitPanel1.Size = New System.Drawing.Size(139, 339)
        Me.SplitPanel1.SizeInfo.AutoSizeScale = New System.Drawing.SizeF(-0.2116182!, 0.0!)
        Me.SplitPanel1.SizeInfo.SplitterCorrection = New System.Drawing.Size(-102, 0)
        Me.SplitPanel1.TabIndex = 0
        Me.SplitPanel1.TabStop = False
        Me.SplitPanel1.Text = "SplitPanel1"
        Me.SplitPanel1.ThemeName = "TelerikMetroBlue"
        '
        'DataAreaListControl
        '
        Me.DataAreaListControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataAreaListControl.Location = New System.Drawing.Point(0, 0)
        Me.DataAreaListControl.Name = "DataAreaListControl"
        Me.DataAreaListControl.Size = New System.Drawing.Size(139, 339)
        Me.DataAreaListControl.TabIndex = 0
        Me.DataAreaListControl.Text = "RadListControl1"
        Me.DataAreaListControl.ThemeName = "TelerikMetroBlue"
        '
        'SplitPanel2
        '
        Me.SplitPanel2.Controls.Add(Me.DataAreaPropertyGrid)
        Me.SplitPanel2.Location = New System.Drawing.Point(142, 0)
        Me.SplitPanel2.Name = "SplitPanel2"
        '
        '
        '
        Me.SplitPanel2.RootElement.MinSize = New System.Drawing.Size(25, 25)
        Me.SplitPanel2.Size = New System.Drawing.Size(343, 339)
        Me.SplitPanel2.SizeInfo.AutoSizeScale = New System.Drawing.SizeF(0.2116182!, 0.0!)
        Me.SplitPanel2.SizeInfo.SplitterCorrection = New System.Drawing.Size(102, 0)
        Me.SplitPanel2.TabIndex = 1
        Me.SplitPanel2.TabStop = False
        Me.SplitPanel2.Text = "SplitPanel2"
        Me.SplitPanel2.ThemeName = "TelerikMetroBlue"
        '
        'DataAreaPropertyGrid
        '
        Me.DataAreaPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataAreaPropertyGrid.ItemHeight = 21
        Me.DataAreaPropertyGrid.Location = New System.Drawing.Point(0, 0)
        Me.DataAreaPropertyGrid.Name = "DataAreaPropertyGrid"
        Me.DataAreaPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.CategorizedAlphabetical
        Me.DataAreaPropertyGrid.Size = New System.Drawing.Size(343, 339)
        Me.DataAreaPropertyGrid.SortOrder = System.Windows.Forms.SortOrder.Ascending
        Me.DataAreaPropertyGrid.TabIndex = 2
        Me.DataAreaPropertyGrid.ThemeName = "TelerikMetroBlue"
        Me.DataAreaPropertyGrid.ToolbarVisible = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.AddDataAreaButton)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(485, 33)
        Me.Panel1.TabIndex = 3
        '
        'AddDataAreaButton
        '
        Me.AddDataAreaButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Image
        Me.AddDataAreaButton.Image = Global.OnTrack.Addin.My.Resources.Resources.plus
        Me.AddDataAreaButton.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.AddDataAreaButton.Location = New System.Drawing.Point(3, 0)
        Me.AddDataAreaButton.Name = "AddDataAreaButton"
        Me.AddDataAreaButton.Size = New System.Drawing.Size(37, 33)
        Me.AddDataAreaButton.TabIndex = 0
        Me.AddDataAreaButton.ThemeName = "TelerikMetroBlue"
        '
        'UIFormWorkDataAreas
        '
        Me.AcceptButton = Me.SaveButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(485, 404)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.RadSplitContainer1)
        Me.Controls.Add(Me.RadStatusStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "UIFormWorkDataAreas"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.Text = "Work with DataAreas"
        Me.ThemeName = "TelerikMetroBlue"
        CType(Me.RadStatusStrip1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadSplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RadSplitContainer1.ResumeLayout(False)
        CType(Me.SplitPanel1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitPanel1.ResumeLayout(False)
        CType(Me.DataAreaListControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SplitPanel2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitPanel2.ResumeLayout(False)
        CType(Me.DataAreaPropertyGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        CType(Me.AddDataAreaButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TelerikMetroBlueTheme1 As Telerik.WinControls.Themes.TelerikMetroBlueTheme
    Friend WithEvents RadStatusStrip1 As Telerik.WinControls.UI.RadStatusStrip
    Friend WithEvents RadLabelElement1 As Telerik.WinControls.UI.RadLabelElement
    Friend WithEvents CancelButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents SaveButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents RadSplitContainer1 As Telerik.WinControls.UI.RadSplitContainer
    Friend WithEvents SplitPanel1 As Telerik.WinControls.UI.SplitPanel
    Friend WithEvents DataAreaListControl As Telerik.WinControls.UI.RadListControl
    Friend WithEvents SplitPanel2 As Telerik.WinControls.UI.SplitPanel
    Friend WithEvents DataAreaPropertyGrid As Telerik.WinControls.UI.RadPropertyGrid
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents AddDataAreaButton As Telerik.WinControls.UI.RadButton
End Class

