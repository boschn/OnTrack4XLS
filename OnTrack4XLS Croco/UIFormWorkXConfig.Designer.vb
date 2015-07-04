<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UIFormWorkXConfig
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UIFormWorkXConfig))
        Me.TelerikMetroBlueTheme1 = New Telerik.WinControls.Themes.TelerikMetroBlueTheme()
        Me.StatusStrip = New Telerik.WinControls.UI.RadStatusStrip()
        Me.StatusLabel = New Telerik.WinControls.UI.RadLabelElement()
        Me.CancelButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.SaveButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.CommandPanel = New Telerik.WinControls.UI.RadPanel()
        Me.SpecialsButton = New Telerik.WinControls.UI.RadDropDownButton()
        Me.CreateDoc9ConfigMenuItem = New Telerik.WinControls.UI.RadMenuItem()
        Me.AddButton = New Telerik.WinControls.UI.RadButton()
        Me.RadSplitContainer1 = New Telerik.WinControls.UI.RadSplitContainer()
        Me.SplitPanel1 = New Telerik.WinControls.UI.SplitPanel()
        Me.ListXConfigsGV = New Telerik.WinControls.UI.RadGridView()
        Me.SplitPanel2 = New Telerik.WinControls.UI.SplitPanel()
        Me.RadPanel1 = New Telerik.WinControls.UI.RadPanel()
        Me.OutlineCombo = New Telerik.WinControls.UI.RadMultiColumnComboBox()
        Me.RadLabel5 = New Telerik.WinControls.UI.RadLabel()
        Me.RadLabel1 = New Telerik.WinControls.UI.RadLabel()
        Me.ConfigNameTb = New Telerik.WinControls.UI.RadTextBox()
        Me.DescriptionTB = New Telerik.WinControls.UI.RadTextBox()
        Me.RadLabel2 = New Telerik.WinControls.UI.RadLabel()
        Me.DataIDSplitContainer = New Telerik.WinControls.UI.RadSplitContainer()
        Me.SplitPanel3 = New Telerik.WinControls.UI.SplitPanel()
        Me.XConfigObjectsGView = New Telerik.WinControls.UI.RadGridView()
        Me.RadLabel3 = New Telerik.WinControls.UI.RadLabel()
        Me.SplitPanel4 = New Telerik.WinControls.UI.SplitPanel()
        Me.DynamicIDButton = New Telerik.WinControls.UI.RadToggleButton()
        Me.XConfigIDsGView = New Telerik.WinControls.UI.RadGridView()
        Me.RadLabel4 = New Telerik.WinControls.UI.RadLabel()
        Me.RadSplitContainer2 = New Telerik.WinControls.UI.RadSplitContainer()
        CType(Me.StatusStrip, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CommandPanel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.CommandPanel.SuspendLayout()
        CType(Me.SpecialsButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AddButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadSplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RadSplitContainer1.SuspendLayout()
        CType(Me.SplitPanel1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitPanel1.SuspendLayout()
        CType(Me.ListXConfigsGV, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ListXConfigsGV.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitPanel2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitPanel2.SuspendLayout()
        CType(Me.RadPanel1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RadPanel1.SuspendLayout()
        CType(Me.OutlineCombo, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.OutlineCombo.EditorControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.OutlineCombo.EditorControl.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ConfigNameTb, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DescriptionTB, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataIDSplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.DataIDSplitContainer.SuspendLayout()
        CType(Me.SplitPanel3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitPanel3.SuspendLayout()
        CType(Me.XConfigObjectsGView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XConfigObjectsGView.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitPanel4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitPanel4.SuspendLayout()
        CType(Me.DynamicIDButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XConfigIDsGView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XConfigIDsGView.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadSplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New Telerik.WinControls.RadItem() {Me.StatusLabel, Me.CancelButton, Me.SaveButton})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 453)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(907, 29)
        Me.StatusStrip.TabIndex = 0
        Me.StatusStrip.Text = "RadStatusStrip1"
        Me.StatusStrip.ThemeName = "TelerikMetroBlue"
        '
        'StatusLabel
        '
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusStrip.SetSpring(Me.StatusLabel, True)
        Me.StatusLabel.Text = ""
        Me.StatusLabel.TextWrap = True
        Me.StatusLabel.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'CancelButton
        '
        Me.CancelButton.AccessibleDescription = "Cancel"
        Me.CancelButton.AccessibleName = "Cancel"
        Me.CancelButton.Name = "CancelButton"
        Me.StatusStrip.SetSpring(Me.CancelButton, False)
        Me.CancelButton.Text = "Cancel"
        Me.CancelButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'SaveButton
        '
        Me.SaveButton.AccessibleDescription = "Save"
        Me.SaveButton.AccessibleName = "Save"
        Me.SaveButton.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SaveButton.Name = "SaveButton"
        Me.StatusStrip.SetSpring(Me.SaveButton, False)
        Me.SaveButton.Text = "Save"
        Me.SaveButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'CommandPanel
        '
        Me.CommandPanel.Controls.Add(Me.SpecialsButton)
        Me.CommandPanel.Controls.Add(Me.AddButton)
        Me.CommandPanel.Dock = System.Windows.Forms.DockStyle.Top
        Me.CommandPanel.Location = New System.Drawing.Point(0, 0)
        Me.CommandPanel.Name = "CommandPanel"
        Me.CommandPanel.Size = New System.Drawing.Size(907, 46)
        Me.CommandPanel.TabIndex = 1
        Me.CommandPanel.ThemeName = "TelerikMetroBlue"
        '
        'SpecialsButton
        '
        Me.SpecialsButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SpecialsButton.Image = Global.OnTrack.AddIn.My.Resources.Resources.Actions_system_run_icon32x32
        Me.SpecialsButton.Items.AddRange(New Telerik.WinControls.RadItem() {Me.CreateDoc9ConfigMenuItem})
        Me.SpecialsButton.Location = New System.Drawing.Point(797, 0)
        Me.SpecialsButton.Name = "SpecialsButton"
        Me.SpecialsButton.Size = New System.Drawing.Size(98, 46)
        Me.SpecialsButton.TabIndex = 1
        Me.SpecialsButton.Text = "Specials"
        Me.SpecialsButton.TextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.SpecialsButton.ThemeName = "TelerikMetroBlue"
        '
        'CreateDoc9ConfigMenuItem
        '
        Me.CreateDoc9ConfigMenuItem.AccessibleDescription = "Create DocConfig"
        Me.CreateDoc9ConfigMenuItem.AccessibleName = "Create DocConfig"
        Me.CreateDoc9ConfigMenuItem.DisplayStyle = Telerik.WinControls.DisplayStyle.Text
        Me.CreateDoc9ConfigMenuItem.KeyTip = "D"
        Me.CreateDoc9ConfigMenuItem.Name = "CreateDoc9ConfigMenuItem"
        Me.CreateDoc9ConfigMenuItem.Text = "Create DocConfig"
        Me.CreateDoc9ConfigMenuItem.ToolTipText = "Create or Modify automatically the Config Setting " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "SMB Document 9"
        Me.CreateDoc9ConfigMenuItem.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'AddButton
        '
        Me.AddButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Image
        Me.AddButton.Image = Global.OnTrack.AddIn.My.Resources.Resources.plus
        Me.AddButton.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.AddButton.Location = New System.Drawing.Point(3, 3)
        Me.AddButton.Name = "AddButton"
        Me.AddButton.Size = New System.Drawing.Size(42, 37)
        Me.AddButton.TabIndex = 0
        Me.AddButton.Text = "RadButton1"
        Me.AddButton.ThemeName = "TelerikMetroBlue"
        '
        'RadSplitContainer1
        '
        Me.RadSplitContainer1.Controls.Add(Me.SplitPanel1)
        Me.RadSplitContainer1.Controls.Add(Me.SplitPanel2)
        Me.RadSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RadSplitContainer1.Location = New System.Drawing.Point(0, 46)
        Me.RadSplitContainer1.Name = "RadSplitContainer1"
        '
        '
        '
        Me.RadSplitContainer1.RootElement.MinSize = New System.Drawing.Size(25, 25)
        Me.RadSplitContainer1.Size = New System.Drawing.Size(907, 407)
        Me.RadSplitContainer1.TabIndex = 2
        Me.RadSplitContainer1.TabStop = False
        Me.RadSplitContainer1.Text = "RadSplitContainer1"
        Me.RadSplitContainer1.ThemeName = "TelerikMetroBlue"
        '
        'SplitPanel1
        '
        Me.SplitPanel1.Controls.Add(Me.ListXConfigsGV)
        Me.SplitPanel1.Location = New System.Drawing.Point(0, 0)
        Me.SplitPanel1.Name = "SplitPanel1"
        '
        '
        '
        Me.SplitPanel1.RootElement.MinSize = New System.Drawing.Size(25, 25)
        Me.SplitPanel1.Size = New System.Drawing.Size(163, 407)
        Me.SplitPanel1.SizeInfo.AutoSizeScale = New System.Drawing.SizeF(-0.3198198!, 0.0!)
        Me.SplitPanel1.SizeInfo.SplitterCorrection = New System.Drawing.Size(-234, 0)
        Me.SplitPanel1.TabIndex = 0
        Me.SplitPanel1.TabStop = False
        Me.SplitPanel1.Text = "SplitPanel1"
        Me.SplitPanel1.ThemeName = "TelerikMetroBlue"
        '
        'ListXConfigsGV
        '
        Me.ListXConfigsGV.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListXConfigsGV.Location = New System.Drawing.Point(0, 0)
        '
        'ListXConfigsGV
        '
        Me.ListXConfigsGV.MasterTemplate.AllowAddNewRow = False
        Me.ListXConfigsGV.MasterTemplate.EnableGrouping = False
        Me.ListXConfigsGV.Name = "ListXConfigsGV"
        Me.ListXConfigsGV.ReadOnly = True
        Me.ListXConfigsGV.Size = New System.Drawing.Size(163, 407)
        Me.ListXConfigsGV.TabIndex = 1
        Me.ListXConfigsGV.ThemeName = "TelerikMetroBlue"
        '
        'SplitPanel2
        '
        Me.SplitPanel2.Controls.Add(Me.RadPanel1)
        Me.SplitPanel2.Controls.Add(Me.DataIDSplitContainer)
        Me.SplitPanel2.Location = New System.Drawing.Point(166, 0)
        Me.SplitPanel2.Name = "SplitPanel2"
        '
        '
        '
        Me.SplitPanel2.RootElement.MinSize = New System.Drawing.Size(25, 25)
        Me.SplitPanel2.Size = New System.Drawing.Size(741, 407)
        Me.SplitPanel2.SizeInfo.AutoSizeScale = New System.Drawing.SizeF(0.3198198!, 0.0!)
        Me.SplitPanel2.SizeInfo.SplitterCorrection = New System.Drawing.Size(234, 0)
        Me.SplitPanel2.TabIndex = 1
        Me.SplitPanel2.TabStop = False
        Me.SplitPanel2.Text = "SplitPanel2"
        Me.SplitPanel2.ThemeName = "TelerikMetroBlue"
        '
        'RadPanel1
        '
        Me.RadPanel1.Controls.Add(Me.OutlineCombo)
        Me.RadPanel1.Controls.Add(Me.RadLabel5)
        Me.RadPanel1.Controls.Add(Me.RadLabel1)
        Me.RadPanel1.Controls.Add(Me.ConfigNameTb)
        Me.RadPanel1.Controls.Add(Me.DescriptionTB)
        Me.RadPanel1.Controls.Add(Me.RadLabel2)
        Me.RadPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.RadPanel1.Location = New System.Drawing.Point(0, 0)
        Me.RadPanel1.Name = "RadPanel1"
        Me.RadPanel1.Size = New System.Drawing.Size(741, 82)
        Me.RadPanel1.TabIndex = 8
        '
        'OutlineCombo
        '
        '
        'OutlineCombo.NestedRadGridView
        '
        Me.OutlineCombo.EditorControl.BackColor = System.Drawing.SystemColors.Window
        Me.OutlineCombo.EditorControl.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OutlineCombo.EditorControl.ForeColor = System.Drawing.SystemColors.ControlText
        Me.OutlineCombo.EditorControl.Location = New System.Drawing.Point(0, 0)
        '
        '
        '
        Me.OutlineCombo.EditorControl.MasterTemplate.AllowAddNewRow = False
        Me.OutlineCombo.EditorControl.MasterTemplate.AllowCellContextMenu = False
        Me.OutlineCombo.EditorControl.MasterTemplate.AllowColumnChooser = False
        Me.OutlineCombo.EditorControl.MasterTemplate.EnableGrouping = False
        Me.OutlineCombo.EditorControl.MasterTemplate.ShowFilteringRow = False
        Me.OutlineCombo.EditorControl.Name = "NestedRadGridView"
        Me.OutlineCombo.EditorControl.ReadOnly = True
        Me.OutlineCombo.EditorControl.ShowGroupPanel = False
        Me.OutlineCombo.EditorControl.Size = New System.Drawing.Size(240, 150)
        Me.OutlineCombo.EditorControl.TabIndex = 0
        Me.OutlineCombo.Location = New System.Drawing.Point(524, 14)
        Me.OutlineCombo.Name = "OutlineCombo"
        '
        '
        '
        Me.OutlineCombo.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren
        Me.OutlineCombo.Size = New System.Drawing.Size(192, 21)
        Me.OutlineCombo.TabIndex = 9
        Me.OutlineCombo.TabStop = False
        Me.OutlineCombo.ThemeName = "TelerikMetroBlue"
        '
        'RadLabel5
        '
        Me.RadLabel5.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RadLabel5.Location = New System.Drawing.Point(452, 15)
        Me.RadLabel5.Name = "RadLabel5"
        Me.RadLabel5.Size = New System.Drawing.Size(50, 21)
        Me.RadLabel5.TabIndex = 6
        Me.RadLabel5.Text = "Outline"
        Me.RadLabel5.ThemeName = "ControlDefault"
        '
        'RadLabel1
        '
        Me.RadLabel1.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RadLabel1.Location = New System.Drawing.Point(12, 15)
        Me.RadLabel1.Name = "RadLabel1"
        Me.RadLabel1.Size = New System.Drawing.Size(86, 21)
        Me.RadLabel1.TabIndex = 8
        Me.RadLabel1.Text = "ConfigName"
        Me.RadLabel1.ThemeName = "ControlDefault"
        '
        'ConfigNameTb
        '
        Me.ConfigNameTb.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.ConfigNameTb.Location = New System.Drawing.Point(104, 14)
        Me.ConfigNameTb.Name = "ConfigNameTb"
        Me.ConfigNameTb.Size = New System.Drawing.Size(188, 22)
        Me.ConfigNameTb.TabIndex = 7
        Me.ConfigNameTb.TabStop = False
        Me.ConfigNameTb.ThemeName = "TelerikMetroBlue"
        '
        'DescriptionTB
        '
        Me.DescriptionTB.Location = New System.Drawing.Point(104, 42)
        Me.DescriptionTB.Name = "DescriptionTB"
        Me.DescriptionTB.Size = New System.Drawing.Size(336, 22)
        Me.DescriptionTB.TabIndex = 6
        Me.DescriptionTB.TabStop = False
        Me.DescriptionTB.ThemeName = "TelerikMetroBlue"
        '
        'RadLabel2
        '
        Me.RadLabel2.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RadLabel2.Location = New System.Drawing.Point(12, 42)
        Me.RadLabel2.Name = "RadLabel2"
        Me.RadLabel2.Size = New System.Drawing.Size(74, 21)
        Me.RadLabel2.TabIndex = 5
        Me.RadLabel2.Text = "Description"
        Me.RadLabel2.ThemeName = "ControlDefault"
        '
        'DataIDSplitContainer
        '
        Me.DataIDSplitContainer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataIDSplitContainer.Controls.Add(Me.SplitPanel3)
        Me.DataIDSplitContainer.Controls.Add(Me.SplitPanel4)
        Me.DataIDSplitContainer.Location = New System.Drawing.Point(0, 85)
        Me.DataIDSplitContainer.Name = "DataIDSplitContainer"
        '
        '
        '
        Me.DataIDSplitContainer.RootElement.MinSize = New System.Drawing.Size(25, 25)
        Me.DataIDSplitContainer.Size = New System.Drawing.Size(741, 322)
        Me.DataIDSplitContainer.TabIndex = 6
        Me.DataIDSplitContainer.TabStop = False
        Me.DataIDSplitContainer.Text = "RadSplitContainer3"
        Me.DataIDSplitContainer.ThemeName = "TelerikMetroBlue"
        '
        'SplitPanel3
        '
        Me.SplitPanel3.Controls.Add(Me.XConfigObjectsGView)
        Me.SplitPanel3.Controls.Add(Me.RadLabel3)
        Me.SplitPanel3.Location = New System.Drawing.Point(0, 0)
        Me.SplitPanel3.Name = "SplitPanel3"
        '
        '
        '
        Me.SplitPanel3.RootElement.MinSize = New System.Drawing.Size(25, 25)
        Me.SplitPanel3.Size = New System.Drawing.Size(176, 322)
        Me.SplitPanel3.SizeInfo.AutoSizeScale = New System.Drawing.SizeF(-0.2613636!, 0.0!)
        Me.SplitPanel3.SizeInfo.SplitterCorrection = New System.Drawing.Size(-207, 0)
        Me.SplitPanel3.TabIndex = 0
        Me.SplitPanel3.TabStop = False
        Me.SplitPanel3.Text = "SplitPanel3"
        Me.SplitPanel3.ThemeName = "TelerikMetroBlue"
        '
        'XConfigObjectsGView
        '
        Me.XConfigObjectsGView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.XConfigObjectsGView.Location = New System.Drawing.Point(0, 30)
        '
        'XConfigObjectsGView
        '
        Me.XConfigObjectsGView.MasterTemplate.AllowAddNewRow = False
        Me.XConfigObjectsGView.MasterTemplate.EnableGrouping = False
        Me.XConfigObjectsGView.Name = "XConfigObjectsGView"
        Me.XConfigObjectsGView.ReadOnly = True
        Me.XConfigObjectsGView.Size = New System.Drawing.Size(173, 292)
        Me.XConfigObjectsGView.TabIndex = 3
        Me.XConfigObjectsGView.Text = "RadGridView1"
        Me.XConfigObjectsGView.ThemeName = "TelerikMetroBlue"
        '
        'RadLabel3
        '
        Me.RadLabel3.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold)
        Me.RadLabel3.Location = New System.Drawing.Point(6, 6)
        Me.RadLabel3.Name = "RadLabel3"
        Me.RadLabel3.Size = New System.Drawing.Size(74, 18)
        Me.RadLabel3.TabIndex = 2
        Me.RadLabel3.Text = "Data Objects"
        Me.RadLabel3.ThemeName = "ControlDefault"
        '
        'SplitPanel4
        '
        Me.SplitPanel4.Controls.Add(Me.DynamicIDButton)
        Me.SplitPanel4.Controls.Add(Me.XConfigIDsGView)
        Me.SplitPanel4.Controls.Add(Me.RadLabel4)
        Me.SplitPanel4.Location = New System.Drawing.Point(179, 0)
        Me.SplitPanel4.Name = "SplitPanel4"
        '
        '
        '
        Me.SplitPanel4.RootElement.MinSize = New System.Drawing.Size(25, 25)
        Me.SplitPanel4.Size = New System.Drawing.Size(562, 322)
        Me.SplitPanel4.SizeInfo.AutoSizeScale = New System.Drawing.SizeF(0.2613636!, 0.0!)
        Me.SplitPanel4.SizeInfo.SplitterCorrection = New System.Drawing.Size(207, 0)
        Me.SplitPanel4.TabIndex = 1
        Me.SplitPanel4.TabStop = False
        Me.SplitPanel4.Text = "SplitPanel4"
        Me.SplitPanel4.ThemeName = "TelerikMetroBlue"
        '
        'DynamicIDButton
        '
        Me.DynamicIDButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DynamicIDButton.Location = New System.Drawing.Point(472, 6)
        Me.DynamicIDButton.Name = "DynamicIDButton"
        Me.DynamicIDButton.Size = New System.Drawing.Size(87, 23)
        Me.DynamicIDButton.TabIndex = 5
        Me.DynamicIDButton.Text = "are Dynamic"
        Me.DynamicIDButton.ThemeName = "TelerikMetroBlue"
        '
        'XConfigIDsGView
        '
        Me.XConfigIDsGView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.XConfigIDsGView.Location = New System.Drawing.Point(0, 30)
        '
        'XConfigIDsGView
        '
        Me.XConfigIDsGView.MasterTemplate.AddNewRowPosition = Telerik.WinControls.UI.SystemRowPosition.Bottom
        Me.XConfigIDsGView.MasterTemplate.AllowAddNewRow = False
        Me.XConfigIDsGView.MasterTemplate.EnableGrouping = False
        Me.XConfigIDsGView.Name = "XConfigIDsGView"
        Me.XConfigIDsGView.ReadOnly = True
        Me.XConfigIDsGView.Size = New System.Drawing.Size(562, 292)
        Me.XConfigIDsGView.TabIndex = 4
        Me.XConfigIDsGView.Text = "RadGridView1"
        Me.XConfigIDsGView.ThemeName = "TelerikMetroBlue"
        '
        'RadLabel4
        '
        Me.RadLabel4.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold)
        Me.RadLabel4.Location = New System.Drawing.Point(3, 6)
        Me.RadLabel4.Name = "RadLabel4"
        Me.RadLabel4.Size = New System.Drawing.Size(61, 18)
        Me.RadLabel4.TabIndex = 3
        Me.RadLabel4.Text = "Identifiers"
        Me.RadLabel4.ThemeName = "ControlDefault"
        '
        'RadSplitContainer2
        '
        Me.RadSplitContainer2.Location = New System.Drawing.Point(187, 233)
        Me.RadSplitContainer2.Name = "RadSplitContainer2"
        '
        '
        '
        Me.RadSplitContainer2.RootElement.MinSize = New System.Drawing.Size(25, 25)
        Me.RadSplitContainer2.Size = New System.Drawing.Size(453, 53)
        Me.RadSplitContainer2.TabIndex = 6
        Me.RadSplitContainer2.TabStop = False
        Me.RadSplitContainer2.Text = "RadSplitContainer2"
        Me.RadSplitContainer2.ThemeName = "TelerikMetroBlue"
        '
        'UIFormWorkXConfig
        '
        Me.AcceptButton = Me.SaveButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(907, 482)
        Me.Controls.Add(Me.RadSplitContainer1)
        Me.Controls.Add(Me.CommandPanel)
        Me.Controls.Add(Me.StatusStrip)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "UIFormWorkXConfig"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.Text = "Work with XChange Configurations"
        Me.ThemeName = "TelerikMetroBlue"
        CType(Me.StatusStrip, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CommandPanel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.CommandPanel.ResumeLayout(False)
        CType(Me.SpecialsButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AddButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadSplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RadSplitContainer1.ResumeLayout(False)
        CType(Me.SplitPanel1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitPanel1.ResumeLayout(False)
        CType(Me.ListXConfigsGV.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ListXConfigsGV, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SplitPanel2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitPanel2.ResumeLayout(False)
        CType(Me.RadPanel1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RadPanel1.ResumeLayout(False)
        Me.RadPanel1.PerformLayout()
        CType(Me.OutlineCombo.EditorControl.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.OutlineCombo.EditorControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.OutlineCombo, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ConfigNameTb, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DescriptionTB, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataIDSplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.DataIDSplitContainer.ResumeLayout(False)
        CType(Me.SplitPanel3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitPanel3.ResumeLayout(False)
        Me.SplitPanel3.PerformLayout()
        CType(Me.XConfigObjectsGView.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XConfigObjectsGView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SplitPanel4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitPanel4.ResumeLayout(False)
        Me.SplitPanel4.PerformLayout()
        CType(Me.DynamicIDButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XConfigIDsGView.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XConfigIDsGView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadSplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TelerikMetroBlueTheme1 As Telerik.WinControls.Themes.TelerikMetroBlueTheme
    Friend WithEvents StatusStrip As Telerik.WinControls.UI.RadStatusStrip
    Friend WithEvents StatusLabel As Telerik.WinControls.UI.RadLabelElement
    Friend WithEvents CancelButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents SaveButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents CommandPanel As Telerik.WinControls.UI.RadPanel
    Friend WithEvents AddButton As Telerik.WinControls.UI.RadButton
    Friend WithEvents RadSplitContainer1 As Telerik.WinControls.UI.RadSplitContainer
    Friend WithEvents SplitPanel1 As Telerik.WinControls.UI.SplitPanel
    Friend WithEvents SplitPanel2 As Telerik.WinControls.UI.SplitPanel
    Friend WithEvents RadSplitContainer2 As Telerik.WinControls.UI.RadSplitContainer
    Friend WithEvents XConfigIDsGV As Telerik.WinControls.UI.MasterGridViewTemplate
    Friend WithEvents RadPanel1 As Telerik.WinControls.UI.RadPanel
    Friend WithEvents RadLabel1 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents ConfigNameTb As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents DescriptionTB As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents RadLabel2 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents DataIDSplitContainer As Telerik.WinControls.UI.RadSplitContainer
    Friend WithEvents SplitPanel3 As Telerik.WinControls.UI.SplitPanel
    Friend WithEvents XConfigObjectsGView As Telerik.WinControls.UI.RadGridView
    Friend WithEvents RadLabel3 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents SplitPanel4 As Telerik.WinControls.UI.SplitPanel
    Friend WithEvents RadLabel4 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents XConfigObjectsGV As Telerik.WinControls.UI.MasterGridViewTemplate
    Friend WithEvents XConfigIDsGView As Telerik.WinControls.UI.RadGridView
    Friend WithEvents ListXConfigsGV As Telerik.WinControls.UI.RadGridView
    Friend WithEvents SpecialsButton As Telerik.WinControls.UI.RadDropDownButton
    Friend WithEvents CreateDoc9ConfigMenuItem As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents DynamicIDButton As Telerik.WinControls.UI.RadToggleButton
    Friend WithEvents RadLabel5 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents OutlineCombo As Telerik.WinControls.UI.RadMultiColumnComboBox
End Class

