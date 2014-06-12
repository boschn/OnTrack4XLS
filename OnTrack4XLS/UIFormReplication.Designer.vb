<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UIFormReplication
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UIFormReplication))
        Me.TelerikMetroBlueTheme1 = New Telerik.WinControls.Themes.TelerikMetroBlueTheme()
        Me.CancelButton = New Telerik.WinControls.UI.RadButton()
        Me.ReplicateButton = New Telerik.WinControls.UI.RadDropDownButton()
        Me.FullReplicationMenuItem = New Telerik.WinControls.UI.RadMenuItem()
        Me.EllipseShape1 = New Telerik.WinControls.EllipseShape()
        Me.IncrementalReplicationMenuItem = New Telerik.WinControls.UI.RadMenuItem()
        Me.StatusStrip = New Telerik.WinControls.UI.RadStatusStrip()
        Me.StatusLabel = New Telerik.WinControls.UI.RadLabelElement()
        Me.StatusProgress = New Telerik.WinControls.UI.RadProgressBarElement()
        Me.RadThemeManager1 = New Telerik.WinControls.RadThemeManager()
        Me.OutboundToggleButton = New Telerik.WinControls.UI.RadToggleButton()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.InboundToggleButton = New Telerik.WinControls.UI.RadToggleButton()
        Me.ToggleInOutButton = New Telerik.WinControls.UI.RadToggleButton()
        Me.RadLabel1 = New Telerik.WinControls.UI.RadLabel()
        Me.RadLabel2 = New Telerik.WinControls.UI.RadLabel()
        Me.RadLabel3 = New Telerik.WinControls.UI.RadLabel()
        Me.WorkspaceDropDownList = New Telerik.WinControls.UI.RadMultiColumnComboBox()
        Me.RadMultiColumnComboBox2 = New Telerik.WinControls.UI.RadMultiColumnComboBox()
        Me.RadMultiColumnComboBox1 = New Telerik.WinControls.UI.RadMultiColumnComboBox()
        Me.DataAreaComboBox = New Telerik.WinControls.UI.RadMultiColumnComboBox()
        Me.RadLabel4 = New Telerik.WinControls.UI.RadLabel()
        Me.RadLabel5 = New Telerik.WinControls.UI.RadLabel()
        Me.DomainCombo = New Telerik.WinControls.UI.RadMultiColumnComboBox()
        CType(Me.CancelButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ReplicateButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.StatusStrip, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.OutboundToggleButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.InboundToggleButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ToggleInOutButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WorkspaceDropDownList, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WorkspaceDropDownList.EditorControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WorkspaceDropDownList.EditorControl.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.WorkspaceDropDownList.SuspendLayout()
        CType(Me.RadMultiColumnComboBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadMultiColumnComboBox2.EditorControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadMultiColumnComboBox2.EditorControl.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RadMultiColumnComboBox2.EditorControl.SuspendLayout()
        CType(Me.RadMultiColumnComboBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadMultiColumnComboBox1.EditorControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadMultiColumnComboBox1.EditorControl.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataAreaComboBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataAreaComboBox.EditorControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataAreaComboBox.EditorControl.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DomainCombo, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DomainCombo.EditorControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DomainCombo.EditorControl.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'CancelButton
        '
        Me.CancelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CancelButton.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CancelButton.Location = New System.Drawing.Point(367, 347)
        Me.CancelButton.Name = "CancelButton"
        Me.CancelButton.Size = New System.Drawing.Size(106, 45)
        Me.CancelButton.TabIndex = 1
        Me.CancelButton.Text = "Cancel"
        Me.CancelButton.ThemeName = "TelerikMetroBlue"
        '
        'ReplicateButton
        '
        Me.ReplicateButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ReplicateButton.EnableKeyMap = True
        Me.ReplicateButton.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ReplicateButton.Image = Global.OnTrack.Addin.My.Resources.Resources.save
        Me.ReplicateButton.Items.AddRange(New Telerik.WinControls.RadItem() {Me.FullReplicationMenuItem, Me.IncrementalReplicationMenuItem})
        Me.ReplicateButton.Location = New System.Drawing.Point(213, 347)
        Me.ReplicateButton.Name = "ReplicateButton"
        Me.ReplicateButton.Size = New System.Drawing.Size(148, 45)
        Me.ReplicateButton.TabIndex = 2
        Me.ReplicateButton.Text = "Replicate"
        Me.ReplicateButton.TextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.ReplicateButton.ThemeName = "TelerikMetroBlue"
        '
        'FullReplicationMenuItem
        '
        Me.FullReplicationMenuItem.AccessibleDescription = "Full Replication"
        Me.FullReplicationMenuItem.AccessibleName = "Full Replication"
        Me.FullReplicationMenuItem.DescriptionText = "all rows in the data area"
        Me.FullReplicationMenuItem.Image = Global.OnTrack.Addin.My.Resources.Resources.save
        Me.FullReplicationMenuItem.KeyTip = "ALL ROWS OF DATA AREA"
        Me.FullReplicationMenuItem.Name = "FullReplicationMenuItem"
        Me.FullReplicationMenuItem.Shape = Me.EllipseShape1
        Me.FullReplicationMenuItem.ShowArrow = True
        Me.FullReplicationMenuItem.Tag = Global.OnTrack.Addin.MySettings.Default.ReplicationForm_Full
        Me.FullReplicationMenuItem.Text = "Full Replication"
        Me.FullReplicationMenuItem.ToolTipText = "replicates all rows with the database"
        Me.FullReplicationMenuItem.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'IncrementalReplicationMenuItem
        '
        Me.IncrementalReplicationMenuItem.AccessibleDescription = "RadMenuItem2"
        Me.IncrementalReplicationMenuItem.AccessibleName = "RadMenuItem2"
        Me.IncrementalReplicationMenuItem.DescriptionText = "only changed data"
        Me.IncrementalReplicationMenuItem.Image = Global.OnTrack.Addin.My.Resources.Resources.save
        Me.IncrementalReplicationMenuItem.KeyTip = "ONLY CHANGED ROWS OF DATA AREA"
        Me.IncrementalReplicationMenuItem.Name = "IncrementalReplicationMenuItem"
        Me.IncrementalReplicationMenuItem.Shape = Me.EllipseShape1
        Me.IncrementalReplicationMenuItem.Tag = Global.OnTrack.Addin.MySettings.Default.ReplicationForm_Incremental
        Me.IncrementalReplicationMenuItem.Text = "Incremental Replication"
        Me.IncrementalReplicationMenuItem.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New Telerik.WinControls.RadItem() {Me.StatusLabel, Me.StatusProgress})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 397)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(485, 25)
        Me.StatusStrip.TabIndex = 3
        Me.StatusStrip.Text = "RadStatusStrip1"
        Me.StatusStrip.ThemeName = "TelerikMetroBlue"
        '
        'StatusLabel
        '
        Me.StatusLabel.AccessibleDescription = "StatusLabel"
        Me.StatusLabel.AccessibleName = "StatusLabel"
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusStrip.SetSpring(Me.StatusLabel, True)
        Me.StatusLabel.Text = ""
        Me.StatusLabel.TextWrap = True
        Me.StatusLabel.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'StatusProgress
        '
        Me.StatusProgress.DefaultSize = New System.Drawing.Size(100, 20)
        Me.StatusProgress.Name = "StatusProgress"
        Me.StatusProgress.SeparatorColor1 = System.Drawing.Color.White
        Me.StatusProgress.SeparatorColor2 = System.Drawing.Color.White
        Me.StatusProgress.SeparatorColor3 = System.Drawing.Color.White
        Me.StatusProgress.SeparatorColor4 = System.Drawing.Color.White
        Me.StatusProgress.SeparatorGradientAngle = 0
        Me.StatusProgress.SeparatorGradientPercentage1 = 0.4!
        Me.StatusProgress.SeparatorGradientPercentage2 = 0.6!
        Me.StatusProgress.SeparatorNumberOfColors = 2
        Me.StatusStrip.SetSpring(Me.StatusProgress, False)
        Me.StatusProgress.StepWidth = 14
        Me.StatusProgress.SweepAngle = 90
        Me.StatusProgress.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'OutboundToggleButton
        '
        Me.OutboundToggleButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OutboundToggleButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Image
        Me.OutboundToggleButton.Image = Global.OnTrack.Addin.My.Resources.Resources.excel_replication_outbound_small
        Me.OutboundToggleButton.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.OutboundToggleButton.Location = New System.Drawing.Point(209, 239)
        Me.OutboundToggleButton.Name = "OutboundToggleButton"
        Me.OutboundToggleButton.Size = New System.Drawing.Size(84, 84)
        Me.OutboundToggleButton.TabIndex = 5
        Me.OutboundToggleButton.ThemeName = "TelerikMetroBlue"
        Me.OutboundToggleButton.ToggleState = Telerik.WinControls.Enumerations.ToggleState.[On]
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(0, 12)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(128, 128)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'InboundToggleButton
        '
        Me.InboundToggleButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.InboundToggleButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Image
        Me.InboundToggleButton.Image = Global.OnTrack.Addin.My.Resources.Resources.excel_replication_inbound_small
        Me.InboundToggleButton.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.InboundToggleButton.Location = New System.Drawing.Point(299, 239)
        Me.InboundToggleButton.Name = "InboundToggleButton"
        Me.InboundToggleButton.Size = New System.Drawing.Size(84, 84)
        Me.InboundToggleButton.TabIndex = 6
        Me.InboundToggleButton.ThemeName = "TelerikMetroBlue"
        '
        'ToggleInOutButton
        '
        Me.ToggleInOutButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ToggleInOutButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Image
        Me.ToggleInOutButton.Image = Global.OnTrack.Addin.My.Resources.Resources.excel_replication_full_small
        Me.ToggleInOutButton.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.ToggleInOutButton.Location = New System.Drawing.Point(389, 239)
        Me.ToggleInOutButton.Name = "ToggleInOutButton"
        Me.ToggleInOutButton.Size = New System.Drawing.Size(84, 84)
        Me.ToggleInOutButton.TabIndex = 7
        Me.ToggleInOutButton.ThemeName = "TelerikMetroBlue"
        '
        'RadLabel1
        '
        Me.RadLabel1.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RadLabel1.Location = New System.Drawing.Point(141, 203)
        Me.RadLabel1.Name = "RadLabel1"
        Me.RadLabel1.Size = New System.Drawing.Size(289, 30)
        Me.RadLabel1.TabIndex = 8
        Me.RadLabel1.Text = "Choose a replication direction "
        '
        'RadLabel2
        '
        Me.RadLabel2.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RadLabel2.Location = New System.Drawing.Point(141, 143)
        Me.RadLabel2.Name = "RadLabel2"
        Me.RadLabel2.Size = New System.Drawing.Size(199, 30)
        Me.RadLabel2.TabIndex = 9
        Me.RadLabel2.Text = "Choose a Workspace"
        '
        'RadLabel3
        '
        Me.RadLabel3.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RadLabel3.Location = New System.Drawing.Point(141, 12)
        Me.RadLabel3.Name = "RadLabel3"
        Me.RadLabel3.Size = New System.Drawing.Size(183, 30)
        Me.RadLabel3.TabIndex = 10
        Me.RadLabel3.Text = "Choose a data area"
        '
        'WorkspaceDropDownList
        '
        Me.WorkspaceDropDownList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.WorkspaceDropDownList.AutoFilter = True
        Me.WorkspaceDropDownList.AutoSizeDropDownHeight = True
        Me.WorkspaceDropDownList.AutoSizeDropDownToBestFit = True
        Me.WorkspaceDropDownList.Controls.Add(Me.RadMultiColumnComboBox2)
        Me.WorkspaceDropDownList.DblClickRotate = True
        Me.WorkspaceDropDownList.DropDownSizingMode = Telerik.WinControls.UI.SizingMode.RightBottom
        Me.WorkspaceDropDownList.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList
        '
        'WorkspaceDropDownList.NestedRadGridView
        '
        Me.WorkspaceDropDownList.EditorControl.BackColor = System.Drawing.SystemColors.Window
        Me.WorkspaceDropDownList.EditorControl.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WorkspaceDropDownList.EditorControl.ForeColor = System.Drawing.SystemColors.ControlText
        Me.WorkspaceDropDownList.EditorControl.Location = New System.Drawing.Point(0, 0)
        '
        '
        '
        Me.WorkspaceDropDownList.EditorControl.MasterTemplate.AllowAddNewRow = False
        Me.WorkspaceDropDownList.EditorControl.MasterTemplate.AllowCellContextMenu = False
        Me.WorkspaceDropDownList.EditorControl.MasterTemplate.AllowColumnChooser = False
        Me.WorkspaceDropDownList.EditorControl.MasterTemplate.AllowDeleteRow = False
        Me.WorkspaceDropDownList.EditorControl.MasterTemplate.AllowEditRow = False
        Me.WorkspaceDropDownList.EditorControl.MasterTemplate.EnableFiltering = True
        Me.WorkspaceDropDownList.EditorControl.MasterTemplate.EnableGrouping = False
        Me.WorkspaceDropDownList.EditorControl.MasterTemplate.ShowFilteringRow = False
        Me.WorkspaceDropDownList.EditorControl.Name = "NestedRadGridView"
        Me.WorkspaceDropDownList.EditorControl.ReadOnly = True
        Me.WorkspaceDropDownList.EditorControl.ShowGroupPanel = False
        Me.WorkspaceDropDownList.EditorControl.Size = New System.Drawing.Size(240, 150)
        Me.WorkspaceDropDownList.EditorControl.TabIndex = 0
        Me.WorkspaceDropDownList.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WorkspaceDropDownList.Location = New System.Drawing.Point(346, 166)
        Me.WorkspaceDropDownList.Name = "WorkspaceDropDownList"
        '
        '
        '
        Me.WorkspaceDropDownList.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren
        Me.WorkspaceDropDownList.Size = New System.Drawing.Size(127, 30)
        Me.WorkspaceDropDownList.TabIndex = 3
        Me.WorkspaceDropDownList.TabStop = False
        Me.WorkspaceDropDownList.Text = "Select .."
        Me.WorkspaceDropDownList.ThemeName = "TelerikMetroBlue"
        '
        'RadMultiColumnComboBox2
        '
        Me.RadMultiColumnComboBox2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RadMultiColumnComboBox2.AutoFilter = True
        Me.RadMultiColumnComboBox2.AutoScroll = True
        Me.RadMultiColumnComboBox2.AutoSizeDropDownHeight = True
        Me.RadMultiColumnComboBox2.AutoSizeDropDownToBestFit = True
        Me.RadMultiColumnComboBox2.DblClickRotate = True
        Me.RadMultiColumnComboBox2.DropDownSizingMode = Telerik.WinControls.UI.SizingMode.RightBottom
        Me.RadMultiColumnComboBox2.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList
        '
        'RadMultiColumnComboBox2.NestedRadGridView
        '
        Me.RadMultiColumnComboBox2.EditorControl.BackColor = System.Drawing.SystemColors.Window
        Me.RadMultiColumnComboBox2.EditorControl.Controls.Add(Me.RadMultiColumnComboBox1)
        Me.RadMultiColumnComboBox2.EditorControl.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RadMultiColumnComboBox2.EditorControl.ForeColor = System.Drawing.SystemColors.ControlText
        Me.RadMultiColumnComboBox2.EditorControl.Location = New System.Drawing.Point(-197, -91)
        '
        '
        '
        Me.RadMultiColumnComboBox2.EditorControl.MasterTemplate.AllowAddNewRow = False
        Me.RadMultiColumnComboBox2.EditorControl.MasterTemplate.AllowCellContextMenu = False
        Me.RadMultiColumnComboBox2.EditorControl.MasterTemplate.AllowColumnChooser = False
        Me.RadMultiColumnComboBox2.EditorControl.MasterTemplate.AllowDeleteRow = False
        Me.RadMultiColumnComboBox2.EditorControl.MasterTemplate.AllowEditRow = False
        Me.RadMultiColumnComboBox2.EditorControl.MasterTemplate.EnableFiltering = True
        Me.RadMultiColumnComboBox2.EditorControl.MasterTemplate.EnableGrouping = False
        Me.RadMultiColumnComboBox2.EditorControl.MasterTemplate.ShowFilteringRow = False
        Me.RadMultiColumnComboBox2.EditorControl.Name = "NestedRadGridView"
        Me.RadMultiColumnComboBox2.EditorControl.ReadOnly = True
        Me.RadMultiColumnComboBox2.EditorControl.ShowGroupPanel = False
        Me.RadMultiColumnComboBox2.EditorControl.Size = New System.Drawing.Size(240, 150)
        Me.RadMultiColumnComboBox2.EditorControl.TabIndex = 4
        Me.RadMultiColumnComboBox2.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RadMultiColumnComboBox2.Location = New System.Drawing.Point(68, 75)
        Me.RadMultiColumnComboBox2.Name = "RadMultiColumnComboBox2"
        '
        '
        '
        Me.RadMultiColumnComboBox2.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren
        Me.RadMultiColumnComboBox2.Size = New System.Drawing.Size(168, 31)
        Me.RadMultiColumnComboBox2.TabIndex = 5
        Me.RadMultiColumnComboBox2.TabStop = False
        Me.RadMultiColumnComboBox2.Text = "Select .."
        Me.RadMultiColumnComboBox2.ThemeName = "TelerikMetroBlue"
        '
        'RadMultiColumnComboBox1
        '
        '
        'RadMultiColumnComboBox1.NestedRadGridView
        '
        Me.RadMultiColumnComboBox1.EditorControl.BackColor = System.Drawing.SystemColors.Window
        Me.RadMultiColumnComboBox1.EditorControl.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RadMultiColumnComboBox1.EditorControl.ForeColor = System.Drawing.SystemColors.ControlText
        Me.RadMultiColumnComboBox1.EditorControl.Location = New System.Drawing.Point(0, 0)
        '
        '
        '
        Me.RadMultiColumnComboBox1.EditorControl.MasterTemplate.AllowAddNewRow = False
        Me.RadMultiColumnComboBox1.EditorControl.MasterTemplate.AllowCellContextMenu = False
        Me.RadMultiColumnComboBox1.EditorControl.MasterTemplate.AllowColumnChooser = False
        Me.RadMultiColumnComboBox1.EditorControl.MasterTemplate.EnableGrouping = False
        Me.RadMultiColumnComboBox1.EditorControl.MasterTemplate.ShowFilteringRow = False
        Me.RadMultiColumnComboBox1.EditorControl.Name = "NestedRadGridView"
        Me.RadMultiColumnComboBox1.EditorControl.ReadOnly = True
        Me.RadMultiColumnComboBox1.EditorControl.ShowGroupPanel = False
        Me.RadMultiColumnComboBox1.EditorControl.Size = New System.Drawing.Size(240, 150)
        Me.RadMultiColumnComboBox1.EditorControl.TabIndex = 0
        Me.RadMultiColumnComboBox1.Location = New System.Drawing.Point(0, 0)
        Me.RadMultiColumnComboBox1.Name = "RadMultiColumnComboBox1"
        '
        '
        '
        Me.RadMultiColumnComboBox1.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren
        Me.RadMultiColumnComboBox1.Size = New System.Drawing.Size(106, 20)
        Me.RadMultiColumnComboBox1.TabIndex = 0
        Me.RadMultiColumnComboBox1.TabStop = False
        Me.RadMultiColumnComboBox1.Text = "RadMultiColumnComboBox1"
        '
        'DataAreaComboBox
        '
        Me.DataAreaComboBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataAreaComboBox.AutoScroll = True
        Me.DataAreaComboBox.AutoSizeDropDownHeight = True
        Me.DataAreaComboBox.AutoSizeDropDownToBestFit = True
        Me.DataAreaComboBox.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList
        '
        'DataAreaComboBox.NestedRadGridView
        '
        Me.DataAreaComboBox.EditorControl.BackColor = System.Drawing.SystemColors.Window
        Me.DataAreaComboBox.EditorControl.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DataAreaComboBox.EditorControl.ForeColor = System.Drawing.SystemColors.ControlText
        Me.DataAreaComboBox.EditorControl.Location = New System.Drawing.Point(0, 0)
        '
        '
        '
        Me.DataAreaComboBox.EditorControl.MasterTemplate.AllowAddNewRow = False
        Me.DataAreaComboBox.EditorControl.MasterTemplate.AllowCellContextMenu = False
        Me.DataAreaComboBox.EditorControl.MasterTemplate.AllowColumnChooser = False
        Me.DataAreaComboBox.EditorControl.MasterTemplate.EnableGrouping = False
        Me.DataAreaComboBox.EditorControl.MasterTemplate.ShowFilteringRow = False
        Me.DataAreaComboBox.EditorControl.Name = "NestedRadGridView"
        Me.DataAreaComboBox.EditorControl.ReadOnly = True
        Me.DataAreaComboBox.EditorControl.ShowGroupPanel = False
        Me.DataAreaComboBox.EditorControl.Size = New System.Drawing.Size(240, 150)
        Me.DataAreaComboBox.EditorControl.TabIndex = 0
        Me.DataAreaComboBox.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DataAreaComboBox.Location = New System.Drawing.Point(313, 48)
        Me.DataAreaComboBox.Name = "DataAreaComboBox"
        '
        '
        '
        Me.DataAreaComboBox.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren
        Me.DataAreaComboBox.Size = New System.Drawing.Size(160, 30)
        Me.DataAreaComboBox.TabIndex = 1
        Me.DataAreaComboBox.TabStop = False
        Me.DataAreaComboBox.Text = "Select .."
        Me.DataAreaComboBox.ThemeName = "TelerikMetroBlue"
        '
        'RadLabel4
        '
        Me.RadLabel4.AutoSize = False
        Me.RadLabel4.BackColor = System.Drawing.Color.LightGray
        Me.RadLabel4.Location = New System.Drawing.Point(0, 143)
        Me.RadLabel4.Name = "RadLabel4"
        Me.RadLabel4.Size = New System.Drawing.Size(128, 248)
        Me.RadLabel4.TabIndex = 12
        Me.RadLabel4.Text = "To replicate with the OnTrack database please make the following settings and pre" & _
    "ss ""replicate""."
        '
        'RadLabel5
        '
        Me.RadLabel5.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RadLabel5.Location = New System.Drawing.Point(141, 80)
        Me.RadLabel5.Name = "RadLabel5"
        Me.RadLabel5.Size = New System.Drawing.Size(170, 30)
        Me.RadLabel5.TabIndex = 10
        Me.RadLabel5.Text = "Choose a Domain"
        '
        'DomainCombo
        '
        Me.DomainCombo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DomainCombo.AutoFilter = True
        Me.DomainCombo.AutoSizeDropDownHeight = True
        Me.DomainCombo.AutoSizeDropDownToBestFit = True
        Me.DomainCombo.DblClickRotate = True
        Me.DomainCombo.DropDownSizingMode = Telerik.WinControls.UI.SizingMode.RightBottom
        Me.DomainCombo.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList
        '
        'DomainCombo.NestedRadGridView
        '
        Me.DomainCombo.EditorControl.BackColor = System.Drawing.SystemColors.Window
        Me.DomainCombo.EditorControl.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DomainCombo.EditorControl.ForeColor = System.Drawing.SystemColors.ControlText
        Me.DomainCombo.EditorControl.Location = New System.Drawing.Point(0, 0)
        '
        '
        '
        Me.DomainCombo.EditorControl.MasterTemplate.AllowAddNewRow = False
        Me.DomainCombo.EditorControl.MasterTemplate.AllowCellContextMenu = False
        Me.DomainCombo.EditorControl.MasterTemplate.AllowColumnChooser = False
        Me.DomainCombo.EditorControl.MasterTemplate.EnableFiltering = True
        Me.DomainCombo.EditorControl.MasterTemplate.EnableGrouping = False
        Me.DomainCombo.EditorControl.MasterTemplate.ShowFilteringRow = False
        Me.DomainCombo.EditorControl.Name = "NestedRadGridView"
        Me.DomainCombo.EditorControl.ReadOnly = True
        Me.DomainCombo.EditorControl.ShowGroupPanel = False
        Me.DomainCombo.EditorControl.Size = New System.Drawing.Size(240, 150)
        Me.DomainCombo.EditorControl.TabIndex = 0
        Me.DomainCombo.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DomainCombo.Location = New System.Drawing.Point(346, 110)
        Me.DomainCombo.Name = "DomainCombo"
        Me.DomainCombo.NullText = "Select ..."
        Me.DomainCombo.Size = New System.Drawing.Size(127, 30)
        Me.DomainCombo.TabIndex = 2
        Me.DomainCombo.TabStop = False
        Me.DomainCombo.Text = "Select ..."
        Me.DomainCombo.ThemeName = "TelerikMetroBlue"
        '
        'UIFormReplication
        '
        Me.AcceptButton = Me.IncrementalReplicationMenuItem
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(485, 422)
        Me.Controls.Add(Me.DomainCombo)
        Me.Controls.Add(Me.WorkspaceDropDownList)
        Me.Controls.Add(Me.RadLabel5)
        Me.Controls.Add(Me.RadLabel4)
        Me.Controls.Add(Me.DataAreaComboBox)
        Me.Controls.Add(Me.RadLabel3)
        Me.Controls.Add(Me.RadLabel2)
        Me.Controls.Add(Me.RadLabel1)
        Me.Controls.Add(Me.ToggleInOutButton)
        Me.Controls.Add(Me.InboundToggleButton)
        Me.Controls.Add(Me.OutboundToggleButton)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.ReplicateButton)
        Me.Controls.Add(Me.CancelButton)
        Me.Controls.Add(Me.PictureBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.HelpButton = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "UIFormReplication"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.Text = "Replicate with On Track Database"
        Me.ThemeName = "TelerikMetroBlue"
        CType(Me.CancelButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ReplicateButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.StatusStrip, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.OutboundToggleButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.InboundToggleButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ToggleInOutButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WorkspaceDropDownList.EditorControl.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WorkspaceDropDownList.EditorControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WorkspaceDropDownList, System.ComponentModel.ISupportInitialize).EndInit()
        Me.WorkspaceDropDownList.ResumeLayout(False)
        Me.WorkspaceDropDownList.PerformLayout()
        CType(Me.RadMultiColumnComboBox2.EditorControl.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadMultiColumnComboBox2.EditorControl, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RadMultiColumnComboBox2.EditorControl.ResumeLayout(False)
        CType(Me.RadMultiColumnComboBox2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadMultiColumnComboBox1.EditorControl.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadMultiColumnComboBox1.EditorControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadMultiColumnComboBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataAreaComboBox.EditorControl.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataAreaComboBox.EditorControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataAreaComboBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DomainCombo.EditorControl.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DomainCombo.EditorControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DomainCombo, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TelerikMetroBlueTheme1 As Telerik.WinControls.Themes.TelerikMetroBlueTheme
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents CancelButton As Telerik.WinControls.UI.RadButton
    Friend WithEvents ReplicateButton As Telerik.WinControls.UI.RadDropDownButton
    Friend WithEvents FullReplicationMenuItem As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents EllipseShape1 As Telerik.WinControls.EllipseShape
    Friend WithEvents IncrementalReplicationMenuItem As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents StatusStrip As Telerik.WinControls.UI.RadStatusStrip
    Friend WithEvents StatusLabel As Telerik.WinControls.UI.RadLabelElement
    Friend WithEvents StatusProgress As Telerik.WinControls.UI.RadProgressBarElement
    Friend WithEvents RadThemeManager1 As Telerik.WinControls.RadThemeManager
    Friend WithEvents OutboundToggleButton As Telerik.WinControls.UI.RadToggleButton
    Friend WithEvents InboundToggleButton As Telerik.WinControls.UI.RadToggleButton
    Friend WithEvents ToggleInOutButton As Telerik.WinControls.UI.RadToggleButton
    Friend WithEvents RadLabel1 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadLabel2 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadLabel3 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents WorkspaceDropDownList As Telerik.WinControls.UI.RadMultiColumnComboBox
    Friend WithEvents DataAreaComboBox As Telerik.WinControls.UI.RadMultiColumnComboBox
    Friend WithEvents RadLabel4 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadLabel5 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadMultiColumnComboBox2 As Telerik.WinControls.UI.RadMultiColumnComboBox
    Friend WithEvents RadMultiColumnComboBox1 As Telerik.WinControls.UI.RadMultiColumnComboBox
    Friend WithEvents DomainCombo As Telerik.WinControls.UI.RadMultiColumnComboBox
End Class

