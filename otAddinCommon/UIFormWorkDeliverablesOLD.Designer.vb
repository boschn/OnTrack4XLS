
Imports OnTrack.UI

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UIFormWorkDeliverablesOLD
    Inherits Telerik.WinControls.UI.RadRibbonForm

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UIFormWorkDeliverablesOLD))
        Me.StatusStrip = New Telerik.WinControls.UI.RadStatusStrip()
        Me.StatusLabel = New Telerik.WinControls.UI.RadLabelElement()
        Me.AcceptStripButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.AbortStripButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.CloseButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.SplitContainer = New Telerik.WinControls.UI.RadSplitContainer()
        Me.SplitUpperPanel = New Telerik.WinControls.UI.SplitPanel()
        Me.DeliverablesPageView = New Telerik.WinControls.UI.RadPageView()
        Me.DeliverablesViewPage = New Telerik.WinControls.UI.RadPageViewPage()
        Me.SplitLowerPanel = New Telerik.WinControls.UI.SplitPanel()
        Me.Office2013LightTheme1 = New Telerik.WinControls.Themes.Office2013LightTheme()
        Me.DeliverablesRibbon = New Telerik.WinControls.UI.RadRibbonBar()
        Me.ViewTab = New Telerik.WinControls.UI.RibbonTab()
        Me.ViewSettingGroup = New Telerik.WinControls.UI.RadRibbonBarGroup()
        Me.SaveViewSettingButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.LoadViewSettingsButton = New Telerik.WinControls.UI.RadDropDownButtonElement()
        Me.AddNewButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.EditButton = New Telerik.WinControls.UI.RadToggleButtonElement()
        Me.DeleteButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.AcceptButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.OkButton = New Telerik.WinControls.UI.RadButtonElement()
        CType(Me.StatusStrip, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        CType(Me.SplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer.SuspendLayout()
        CType(Me.SplitUpperPanel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitUpperPanel.SuspendLayout()
        CType(Me.DeliverablesPageView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.DeliverablesPageView.SuspendLayout()
        CType(Me.SplitLowerPanel, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DeliverablesRibbon, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StatusStrip
        '
        resources.ApplyResources(Me.StatusStrip, "StatusStrip")
        Me.StatusStrip.Items.AddRange(New Telerik.WinControls.RadItem() {Me.StatusLabel, Me.AcceptStripButton, Me.AbortStripButton, Me.CloseButton})
        Me.StatusStrip.Name = "StatusStrip"
        '
        '
        '
        Me.StatusStrip.RootElement.AccessibleDescription = resources.GetString("StatusStrip.RootElement.AccessibleDescription")
        Me.StatusStrip.RootElement.AccessibleName = resources.GetString("StatusStrip.RootElement.AccessibleName")
        Me.StatusStrip.RootElement.Alignment = CType(resources.GetObject("StatusStrip.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.StatusStrip.RootElement.AngleTransform = CType(resources.GetObject("StatusStrip.RootElement.AngleTransform"), Single)
        Me.StatusStrip.RootElement.FlipText = CType(resources.GetObject("StatusStrip.RootElement.FlipText"), Boolean)
        Me.StatusStrip.RootElement.Margin = CType(resources.GetObject("StatusStrip.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.StatusStrip.RootElement.Text = resources.GetString("StatusStrip.RootElement.Text")
        Me.StatusStrip.RootElement.TextOrientation = CType(resources.GetObject("StatusStrip.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.StatusStrip.SizingGrip = False
        'Me.StatusStrip.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'StatusLabel
        '
        resources.ApplyResources(Me.StatusLabel, "StatusLabel")
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusStrip.SetSpring(Me.StatusLabel, True)
        Me.StatusLabel.TextWrap = True
        Me.StatusLabel.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'AcceptStripButton
        '
        resources.ApplyResources(Me.AcceptStripButton, "AcceptStripButton")
        Me.AcceptStripButton.Enabled = False
        Me.AcceptStripButton.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AcceptStripButton.Image = Global.OnTrack.UI.My.Resources.Resources.checkmark_16_16
        Me.AcceptStripButton.Name = "AcceptStripButton"
        Me.StatusStrip.SetSpring(Me.AcceptStripButton, False)
        Me.AcceptStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.AcceptStripButton.Visibility = Telerik.WinControls.ElementVisibility.Hidden
        '
        'AbortStripButton
        '
        resources.ApplyResources(Me.AbortStripButton, "AbortStripButton")
        Me.AbortStripButton.Enabled = False
        Me.AbortStripButton.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AbortStripButton.Image = Global.OnTrack.UI.My.Resources.Resources.delete_16_16
        Me.AbortStripButton.Name = "AbortStripButton"
        Me.StatusStrip.SetSpring(Me.AbortStripButton, False)
        Me.AbortStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.AbortStripButton.Visibility = Telerik.WinControls.ElementVisibility.Hidden
        '
        'CloseButton
        '
        resources.ApplyResources(Me.CloseButton, "CloseButton")
        Me.CloseButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Text
        Me.CloseButton.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.CloseButton.Name = "CloseButton"
        Me.StatusStrip.SetSpring(Me.CloseButton, False)
        Me.CloseButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.SplitContainer)
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Name = "Panel1"
        '
        'SplitContainer
        '
        Me.SplitContainer.Controls.Add(Me.SplitUpperPanel)
        Me.SplitContainer.Controls.Add(Me.SplitLowerPanel)
        resources.ApplyResources(Me.SplitContainer, "SplitContainer")
        Me.SplitContainer.Name = "SplitContainer"
        '
        '
        '
        Me.SplitContainer.RootElement.AccessibleDescription = resources.GetString("SplitContainer.RootElement.AccessibleDescription")
        Me.SplitContainer.RootElement.AccessibleName = resources.GetString("SplitContainer.RootElement.AccessibleName")
        Me.SplitContainer.RootElement.Alignment = CType(resources.GetObject("SplitContainer.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.SplitContainer.RootElement.AngleTransform = CType(resources.GetObject("SplitContainer.RootElement.AngleTransform"), Single)
        Me.SplitContainer.RootElement.FlipText = CType(resources.GetObject("SplitContainer.RootElement.FlipText"), Boolean)
        Me.SplitContainer.RootElement.Margin = CType(resources.GetObject("SplitContainer.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.SplitContainer.RootElement.Text = resources.GetString("SplitContainer.RootElement.Text")
        Me.SplitContainer.RootElement.TextOrientation = CType(resources.GetObject("SplitContainer.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.SplitContainer.SplitterWidth = 5
        Me.SplitContainer.TabStop = False
        'Me.SplitContainer.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'SplitUpperPanel
        '
        Me.SplitUpperPanel.Controls.Add(Me.DeliverablesPageView)
        resources.ApplyResources(Me.SplitUpperPanel, "SplitUpperPanel")
        Me.SplitUpperPanel.Name = "SplitUpperPanel"
        '
        '
        '
        Me.SplitUpperPanel.RootElement.AccessibleDescription = resources.GetString("SplitUpperPanel.RootElement.AccessibleDescription")
        Me.SplitUpperPanel.RootElement.AccessibleName = resources.GetString("SplitUpperPanel.RootElement.AccessibleName")
        Me.SplitUpperPanel.RootElement.Alignment = CType(resources.GetObject("SplitUpperPanel.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.SplitUpperPanel.RootElement.AngleTransform = CType(resources.GetObject("SplitUpperPanel.RootElement.AngleTransform"), Single)
        Me.SplitUpperPanel.RootElement.FlipText = CType(resources.GetObject("SplitUpperPanel.RootElement.FlipText"), Boolean)
        Me.SplitUpperPanel.RootElement.Margin = CType(resources.GetObject("SplitUpperPanel.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.SplitUpperPanel.RootElement.MinSize = New System.Drawing.Size(0, 0)
        Me.SplitUpperPanel.RootElement.Text = resources.GetString("SplitUpperPanel.RootElement.Text")
        Me.SplitUpperPanel.RootElement.TextOrientation = CType(resources.GetObject("SplitUpperPanel.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.SplitUpperPanel.TabStop = False
        'Me.SplitUpperPanel.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'DeliverablesPageView
        '
        Me.DeliverablesPageView.Controls.Add(Me.DeliverablesViewPage)
        resources.ApplyResources(Me.DeliverablesPageView, "DeliverablesPageView")
        Me.DeliverablesPageView.Name = "DeliverablesPageView"
        '
        '
        '
        Me.DeliverablesPageView.RootElement.AccessibleDescription = resources.GetString("DeliverablesPageView.RootElement.AccessibleDescription")
        Me.DeliverablesPageView.RootElement.AccessibleName = resources.GetString("DeliverablesPageView.RootElement.AccessibleName")
        Me.DeliverablesPageView.RootElement.Alignment = CType(resources.GetObject("DeliverablesPageView.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.DeliverablesPageView.RootElement.AngleTransform = CType(resources.GetObject("DeliverablesPageView.RootElement.AngleTransform"), Single)
        Me.DeliverablesPageView.RootElement.FlipText = CType(resources.GetObject("DeliverablesPageView.RootElement.FlipText"), Boolean)
        Me.DeliverablesPageView.RootElement.Margin = CType(resources.GetObject("DeliverablesPageView.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.DeliverablesPageView.RootElement.Text = resources.GetString("DeliverablesPageView.RootElement.Text")
        Me.DeliverablesPageView.RootElement.TextOrientation = CType(resources.GetObject("DeliverablesPageView.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.DeliverablesPageView.SelectedPage = Me.DeliverablesViewPage
        'Me.DeliverablesPageView.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        CType(Me.DeliverablesPageView.GetChildAt(0), Telerik.WinControls.UI.RadPageViewStripElement).StripButtons = Telerik.WinControls.UI.StripViewButtons.[Auto]
        '
        'DeliverablesViewPage
        '
        Me.DeliverablesViewPage.ItemSize = New System.Drawing.SizeF(33.0!, 27.0!)
        resources.ApplyResources(Me.DeliverablesViewPage, "DeliverablesViewPage")
        Me.DeliverablesViewPage.Name = "DeliverablesViewPage"
        '
        'SplitLowerPanel
        '
        resources.ApplyResources(Me.SplitLowerPanel, "SplitLowerPanel")
        Me.SplitLowerPanel.Name = "SplitLowerPanel"
        '
        '
        '
        Me.SplitLowerPanel.RootElement.AccessibleDescription = resources.GetString("SplitLowerPanel.RootElement.AccessibleDescription")
        Me.SplitLowerPanel.RootElement.AccessibleName = resources.GetString("SplitLowerPanel.RootElement.AccessibleName")
        Me.SplitLowerPanel.RootElement.Alignment = CType(resources.GetObject("SplitLowerPanel.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.SplitLowerPanel.RootElement.AngleTransform = CType(resources.GetObject("SplitLowerPanel.RootElement.AngleTransform"), Single)
        Me.SplitLowerPanel.RootElement.FlipText = CType(resources.GetObject("SplitLowerPanel.RootElement.FlipText"), Boolean)
        Me.SplitLowerPanel.RootElement.Margin = CType(resources.GetObject("SplitLowerPanel.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.SplitLowerPanel.RootElement.MinSize = New System.Drawing.Size(0, 0)
        Me.SplitLowerPanel.RootElement.Text = resources.GetString("SplitLowerPanel.RootElement.Text")
        Me.SplitLowerPanel.RootElement.TextOrientation = CType(resources.GetObject("SplitLowerPanel.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.SplitLowerPanel.TabStop = False
        'Me.SplitLowerPanel.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'DeliverablesRibbon
        '
        Me.DeliverablesRibbon.CommandTabs.AddRange(New Telerik.WinControls.RadItem() {Me.ViewTab})
        resources.ApplyResources(Me.DeliverablesRibbon, "DeliverablesRibbon")
        Me.DeliverablesRibbon.Name = "DeliverablesRibbon"
        Me.DeliverablesRibbon.QuickAccessToolBarItems.AddRange(New Telerik.WinControls.RadItem() {Me.AddNewButton, Me.EditButton, Me.DeleteButton, Me.AcceptButton})
        '
        '
        '
        Me.DeliverablesRibbon.RootElement.AccessibleDescription = resources.GetString("DeliverablesRibbon.RootElement.AccessibleDescription")
        Me.DeliverablesRibbon.RootElement.AccessibleName = resources.GetString("DeliverablesRibbon.RootElement.AccessibleName")
        Me.DeliverablesRibbon.RootElement.Alignment = CType(resources.GetObject("DeliverablesRibbon.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.DeliverablesRibbon.RootElement.AngleTransform = CType(resources.GetObject("DeliverablesRibbon.RootElement.AngleTransform"), Single)
        Me.DeliverablesRibbon.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren
        Me.DeliverablesRibbon.RootElement.FlipText = CType(resources.GetObject("DeliverablesRibbon.RootElement.FlipText"), Boolean)
        Me.DeliverablesRibbon.RootElement.Margin = CType(resources.GetObject("DeliverablesRibbon.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.DeliverablesRibbon.RootElement.Text = resources.GetString("DeliverablesRibbon.RootElement.Text")
        Me.DeliverablesRibbon.RootElement.TextOrientation = CType(resources.GetObject("DeliverablesRibbon.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.DeliverablesRibbon.ShowHelpButton = True
        Me.DeliverablesRibbon.StartButtonImage = Global.OnTrack.UI.My.Resources.Resources._1420167855_puzzle_basic_blue_24_24
        Me.DeliverablesRibbon.StartMenuWidth = 0
        Me.DeliverablesRibbon.TabStop = False
        'Me.DeliverablesRibbon.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        CType(Me.DeliverablesRibbon.GetChildAt(0), Telerik.WinControls.UI.RadRibbonBarElement).Text = resources.GetString("resource.Text")
        '
        'ViewTab
        '
        resources.ApplyResources(Me.ViewTab, "ViewTab")
        Me.ViewTab.Description = "define the deliverables view"
        Me.ViewTab.IsSelected = True
        Me.ViewTab.Items.AddRange(New Telerik.WinControls.RadItem() {Me.ViewSettingGroup})
        Me.ViewTab.Name = "ViewTab"
        Me.ViewTab.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'ViewSettingGroup
        '
        resources.ApplyResources(Me.ViewSettingGroup, "ViewSettingGroup")
        Me.ViewSettingGroup.CollapsedImage = Global.OnTrack.UI.My.Resources.Resources.save
        Me.ViewSettingGroup.Items.AddRange(New Telerik.WinControls.RadItem() {Me.SaveViewSettingButton, Me.LoadViewSettingsButton})
        Me.ViewSettingGroup.Name = "ViewSettingGroup"
        Me.ViewSettingGroup.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'SaveViewSettingButton
        '
        resources.ApplyResources(Me.SaveViewSettingButton, "SaveViewSettingButton")
        Me.SaveViewSettingButton.Image = Global.OnTrack.UI.My.Resources.Resources._1420257490_MB__save_32_32
        Me.SaveViewSettingButton.ImageAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.SaveViewSettingButton.Name = "SaveViewSettingButton"
        Me.SaveViewSettingButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.SaveViewSettingButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'LoadViewSettingsButton
        '
        resources.ApplyResources(Me.LoadViewSettingsButton, "LoadViewSettingsButton")
        Me.LoadViewSettingsButton.ArrowButtonMinSize = New System.Drawing.Size(12, 12)
        Me.LoadViewSettingsButton.DropDownDirection = Telerik.WinControls.UI.RadDirection.Down
        Me.LoadViewSettingsButton.ExpandArrowButton = False
        Me.LoadViewSettingsButton.Name = "LoadViewSettingsButton"
        Me.LoadViewSettingsButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'AddNewButton
        '
        resources.ApplyResources(Me.AddNewButton, "AddNewButton")
        Me.AddNewButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Image
        Me.AddNewButton.Image = Global.OnTrack.UI.My.Resources.Resources.round_plus_16_16
        Me.AddNewButton.Name = "AddNewButton"
        Me.AddNewButton.SmallImage = Global.OnTrack.UI.My.Resources.Resources.round_plus_16_16
        Me.AddNewButton.StretchHorizontally = False
        Me.AddNewButton.StretchVertically = False
        Me.AddNewButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'EditButton
        '
        resources.ApplyResources(Me.EditButton, "EditButton")
        Me.EditButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Image
        Me.EditButton.Enabled = False
        Me.EditButton.Image = Global.OnTrack.UI.My.Resources.Resources.pencil_16_16
        Me.EditButton.Name = "EditButton"
        Me.EditButton.ReadOnly = False
        Me.EditButton.SmallImage = Global.OnTrack.UI.My.Resources.Resources.pencil_16_16
        Me.EditButton.Visibility = Telerik.WinControls.ElementVisibility.Hidden
        '
        'DeleteButton
        '
        resources.ApplyResources(Me.DeleteButton, "DeleteButton")
        Me.DeleteButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Image
        Me.DeleteButton.Enabled = False
        Me.DeleteButton.Image = Global.OnTrack.UI.My.Resources.Resources.delete_16_16
        Me.DeleteButton.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.DeleteButton.Name = "DeleteButton"
        Me.DeleteButton.SmallImage = Global.OnTrack.UI.My.Resources.Resources.delete_16_16
        Me.DeleteButton.StretchHorizontally = False
        Me.DeleteButton.StretchVertically = False
        Me.DeleteButton.Visibility = Telerik.WinControls.ElementVisibility.Hidden
        '
        'AcceptButton
        '
        resources.ApplyResources(Me.AcceptButton, "AcceptButton")
        Me.AcceptButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Image
        Me.AcceptButton.Image = Global.OnTrack.UI.My.Resources.Resources.checkmark_16_16
        Me.AcceptButton.Name = "AcceptButton"
        Me.AcceptButton.SmallImage = Global.OnTrack.UI.My.Resources.Resources.checkmark_16_16
        Me.AcceptButton.StretchHorizontally = False
        Me.AcceptButton.StretchVertically = False
        Me.AcceptButton.Visibility = Telerik.WinControls.ElementVisibility.Hidden
        '
        'OkButton
        '
        resources.ApplyResources(Me.OkButton, "OkButton")
        Me.OkButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Image
        Me.OkButton.Enabled = False
        Me.OkButton.Image = Global.OnTrack.UI.My.Resources.Resources.pencil_16_16
        Me.OkButton.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.OkButton.Name = "OkButton"
        Me.OkButton.SmallImage = Global.OnTrack.UI.My.Resources.Resources.pencil_16_16
        Me.OkButton.StretchHorizontally = False
        Me.OkButton.StretchVertically = False
        Me.OkButton.Visibility = Telerik.WinControls.ElementVisibility.Hidden
        '
        'UIFormWorkDeliverablesOLD
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.DeliverablesRibbon)
        Me.MainMenuStrip = Nothing
        Me.Name = "UIFormWorkDeliverablesOLD"
        '
        '
        '
        Me.RootElement.AccessibleDescription = resources.GetString("UIFormWorkDeliverablesOLD.RootElement.AccessibleDescription")
        Me.RootElement.AccessibleName = resources.GetString("UIFormWorkDeliverablesOLD.RootElement.AccessibleName")
        Me.RootElement.Alignment = CType(resources.GetObject("UIFormWorkDeliverablesOLD.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.RootElement.AngleTransform = CType(resources.GetObject("UIFormWorkDeliverablesOLD.RootElement.AngleTransform"), Single)
        Me.RootElement.ApplyShapeToControl = True
        Me.RootElement.FlipText = CType(resources.GetObject("UIFormWorkDeliverablesOLD.RootElement.FlipText"), Boolean)
        Me.RootElement.Margin = CType(resources.GetObject("UIFormWorkDeliverablesOLD.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.RootElement.Text = resources.GetString("UIFormWorkDeliverablesOLD.RootElement.Text")
        Me.RootElement.TextOrientation = CType(resources.GetObject("UIFormWorkDeliverablesOLD.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        'Me.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        CType(Me.StatusStrip, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        CType(Me.SplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer.ResumeLayout(False)
        CType(Me.SplitUpperPanel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitUpperPanel.ResumeLayout(False)
        CType(Me.DeliverablesPageView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.DeliverablesPageView.ResumeLayout(False)
        CType(Me.SplitLowerPanel, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DeliverablesRibbon, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DeliverablesRibbon As Telerik.WinControls.UI.RadRibbonBar
    Friend WithEvents StatusStrip As Telerik.WinControls.UI.RadStatusStrip
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Office2013LightTheme1 As Telerik.WinControls.Themes.Office2013LightTheme
    Friend WithEvents SplitContainer As Telerik.WinControls.UI.RadSplitContainer
    Friend WithEvents SplitLowerPanel As Telerik.WinControls.UI.SplitPanel
    Friend WithEvents StatusLabel As Telerik.WinControls.UI.RadLabelElement
    Friend WithEvents CloseButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents ViewTab As Telerik.WinControls.UI.RibbonTab
    Friend WithEvents ViewSettingGroup As Telerik.WinControls.UI.RadRibbonBarGroup
    Friend WithEvents SaveViewSettingButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents LoadViewSettingsButton As Telerik.WinControls.UI.RadDropDownButtonElement
    Friend WithEvents SplitUpperPanel As Telerik.WinControls.UI.SplitPanel
    Friend WithEvents DeliverablesPageView As Telerik.WinControls.UI.RadPageView
    Friend WithEvents DeliverablesViewPage As Telerik.WinControls.UI.RadPageViewPage
    Friend WithEvents DeleteButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents EditButton As Telerik.WinControls.UI.RadToggleButtonElement
    Friend WithEvents OkButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents AcceptButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents AddNewButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents AcceptStripButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents AbortStripButton As Telerik.WinControls.UI.RadButtonElement
    'Friend WithEvents UiControlDataGridView1 As Global.OnTrack.UI.UIControlDataGridView
    'Friend WithEvents DeliverablePanel As Global.OnTrack.UI.UIControlPanelDeliverable

End Class
