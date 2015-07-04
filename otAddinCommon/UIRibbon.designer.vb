Partial Class OnTrackRibbon
    Inherits Microsoft.Office.Tools.Ribbon.RibbonBase

    <System.Diagnostics.DebuggerNonUserCode()> 
    Public Sub New(ByVal container As System.ComponentModel.IContainer)
        MyClass.New()

        'Required for Windows.Forms Class Composition Designer support
        If (container IsNot Nothing) Then
            container.Add(Me)
        End If

    End Sub

    <System.Diagnostics.DebuggerNonUserCode()> 
    Public Sub New()
        MyBase.New(Globals.Factory.GetRibbonFactory())

        'This call is required by the Component Designer.
        InitializeComponent()

    End Sub

    'Component overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> 
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> 
    Private Sub InitializeComponent()
        Dim RibbonDialogLauncherImpl1 As Microsoft.Office.Tools.Ribbon.RibbonDialogLauncher = Me.Factory.CreateRibbonDialogLauncher
        Me.OnTrackRibbonTab = Me.Factory.CreateRibbonTab
        Me.OnTrack = Me.Factory.CreateRibbonTab
        Me.Setting = Me.Factory.CreateRibbonGroup
        Me.SettingGroup = Me.Factory.CreateRibbonGroup
        Me.WorkspaceCombo = Me.Factory.CreateRibbonComboBox
        Me.MQFGroup = Me.Factory.CreateRibbonGroup
        Me.ConnectToggleButton = Me.Factory.CreateRibbonToggleButton
        Me.AboutButton = Me.Factory.CreateRibbonButton
        Me.SettingButton = Me.Factory.CreateRibbonButton
        Me.DataAreaButton = Me.Factory.CreateRibbonButton
        Me.XConfigButton = Me.Factory.CreateRibbonButton
        Me.MQFAdminButton = Me.Factory.CreateRibbonButton
        Me.ReplicateButton = Me.Factory.CreateRibbonButton
        Me.LogButton = Me.Factory.CreateRibbonToggleButton
        Me.OnTrackRibbonTab.SuspendLayout()
        Me.OnTrack.SuspendLayout()
        Me.Setting.SuspendLayout()
        Me.SettingGroup.SuspendLayout()
        Me.MQFGroup.SuspendLayout()
        '
        'OnTrackRibbonTab
        '
        Me.OnTrackRibbonTab.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office
        Me.OnTrackRibbonTab.Label = "TabAddIns"
        Me.OnTrackRibbonTab.Name = "OnTrackRibbonTab"
        '
        'OnTrack
        '
        Me.OnTrack.Groups.Add(Me.Setting)
        Me.OnTrack.Groups.Add(Me.SettingGroup)
        Me.OnTrack.Groups.Add(Me.MQFGroup)
        Me.OnTrack.Label = "OnTrack"
        Me.OnTrack.Name = "OnTrack"
        '
        'Setting
        '
        Me.Setting.Items.Add(Me.ConnectToggleButton)
        Me.Setting.Items.Add(Me.AboutButton)
        Me.Setting.Items.Add(Me.SettingButton)
        Me.Setting.Items.Add(Me.LogButton)
        Me.Setting.Label = "OnTrack"
        Me.Setting.Name = "Setting"
        '
        'SettingGroup
        '
        Me.SettingGroup.Items.Add(Me.WorkspaceCombo)
        Me.SettingGroup.Label = "Settings"
        Me.SettingGroup.Name = "SettingGroup"
        '
        'WorkspaceCombo
        '
        Me.WorkspaceCombo.Label = "Workspace"
        Me.WorkspaceCombo.Name = "WorkspaceCombo"
        Me.WorkspaceCombo.ScreenTip = "Set the Default Workspace for this Workbook"
        Me.WorkspaceCombo.Text = Nothing
        '
        'MQFGroup
        '
        Me.MQFGroup.DialogLauncher = RibbonDialogLauncherImpl1
        Me.MQFGroup.Items.Add(Me.DataAreaButton)
        Me.MQFGroup.Items.Add(Me.XConfigButton)
        Me.MQFGroup.Items.Add(Me.MQFAdminButton)
        Me.MQFGroup.Items.Add(Me.ReplicateButton)
        Me.MQFGroup.Label = "Feeding Data"
        Me.MQFGroup.Name = "MQFGroup"
        '
        'ConnectToggleButton
        '
        Me.ConnectToggleButton.Description = "Connect to the OnTrack Database"
        Me.ConnectToggleButton.Image = Global.OnTrackTool.My.Resources.Resources.disconnect_icon
        Me.ConnectToggleButton.Label = "Connect"
        Me.ConnectToggleButton.Name = "ConnectToggleButton"
        Me.ConnectToggleButton.ShowImage = True
        '
        'AboutButton
        '
        Me.AboutButton.Image = Global.OnTrackTool.My.Resources.Resources.fasttrack
        Me.AboutButton.Label = "About"
        Me.AboutButton.Name = "AboutButton"
        Me.AboutButton.ShowImage = True
        '
        'SettingButton
        '
        Me.SettingButton.Image = Global.OnTrackTool.My.Resources.Resources.Actions_configure_toolbars_icon
        Me.SettingButton.Label = "Setting"
        Me.SettingButton.Name = "SettingButton"
        Me.SettingButton.ScreenTip = "OnTrack Property Setting"
        Me.SettingButton.ShowImage = True
        '
        'DataAreaButton
        '
        Me.DataAreaButton.Image = Global.OnTrackTool.My.Resources.Resources.box
        Me.DataAreaButton.Label = "DataArea"
        Me.DataAreaButton.Name = "DataAreaButton"
        Me.DataAreaButton.ScreenTip = "Work with DataAreas in this Workbook"
        Me.DataAreaButton.ShowImage = True
        '
        'XConfigButton
        '
        Me.XConfigButton.Description = "Work with XChange Configuration"
        Me.XConfigButton.Image = Global.OnTrackTool.My.Resources.Resources.setting_config
        Me.XConfigButton.Label = "Config"
        Me.XConfigButton.Name = "XConfigButton"
        Me.XConfigButton.ScreenTip = "Work with XChange Configuration"
        Me.XConfigButton.ShowImage = True
        '
        'MQFAdminButton
        '
        Me.MQFAdminButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge
        Me.MQFAdminButton.Image = Global.OnTrackTool.My.Resources.Resources.MessageQueueTube
        Me.MQFAdminButton.Label = "XChange"
        Me.MQFAdminButton.Name = "MQFAdminButton"
        Me.MQFAdminButton.OfficeImageId = "AccessRelinkLists"
        Me.MQFAdminButton.ScreenTip = "Call the MQF Administrator Tool"
        Me.MQFAdminButton.ShowImage = True
        '
        'ReplicateButton
        '
        Me.ReplicateButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge
        Me.ReplicateButton.Image = Global.OnTrackTool.My.Resources.Resources.excel_replication
        Me.ReplicateButton.Label = "Replicate"
        Me.ReplicateButton.Name = "ReplicateButton"
        Me.ReplicateButton.ShowImage = True
        '
        'LogButton
        '
        Me.LogButton.Description = "Show Message Log Window"
        Me.LogButton.Image = Global.OnTrackTool.My.Resources.Resources.setting_config
        Me.LogButton.Label = "MessageLog"
        Me.LogButton.Name = "LogButton"
        Me.LogButton.ShowImage = True
        '
        'OnTrackRibbon
        '
        Me.Name = "OnTrackRibbon"
        Me.RibbonType = "Microsoft.Excel.Workbook"
        Me.Tabs.Add(Me.OnTrackRibbonTab)
        Me.Tabs.Add(Me.OnTrack)
        Me.OnTrackRibbonTab.ResumeLayout(False)
        Me.OnTrackRibbonTab.PerformLayout()
        Me.OnTrack.ResumeLayout(False)
        Me.OnTrack.PerformLayout()
        Me.Setting.ResumeLayout(False)
        Me.Setting.PerformLayout()
        Me.SettingGroup.ResumeLayout(False)
        Me.SettingGroup.PerformLayout()
        Me.MQFGroup.ResumeLayout(False)
        Me.MQFGroup.PerformLayout()

    End Sub

    Friend WithEvents OnTrackRibbonTab As Microsoft.Office.Tools.Ribbon.RibbonTab
    Friend WithEvents OnTrack As Microsoft.Office.Tools.Ribbon.RibbonTab
    Friend WithEvents Setting As Microsoft.Office.Tools.Ribbon.RibbonGroup
    Friend WithEvents MQFGroup As Microsoft.Office.Tools.Ribbon.RibbonGroup
    Friend WithEvents ConnectToggleButton As Microsoft.Office.Tools.Ribbon.RibbonToggleButton
    Friend WithEvents AboutButton As Microsoft.Office.Tools.Ribbon.RibbonButton
    Friend WithEvents ReplicateButton As Microsoft.Office.Tools.Ribbon.RibbonButton
    Friend WithEvents SettingButton As Microsoft.Office.Tools.Ribbon.RibbonButton
    Friend WithEvents SettingGroup As Microsoft.Office.Tools.Ribbon.RibbonGroup
    Friend WithEvents WorkspaceCombo As Microsoft.Office.Tools.Ribbon.RibbonComboBox
    Friend WithEvents DataAreaButton As Microsoft.Office.Tools.Ribbon.RibbonButton
    Friend WithEvents MQFAdminButton As Microsoft.Office.Tools.Ribbon.RibbonButton
    Friend WithEvents XConfigButton As Microsoft.Office.Tools.Ribbon.RibbonButton
    Friend WithEvents LogButton As Microsoft.Office.Tools.Ribbon.RibbonToggleButton
End Class

Partial Class ThisRibbonCollection

    <System.Diagnostics.DebuggerNonUserCode()> 
    Friend ReadOnly Property Ribbon1() As OnTrackRibbon
        Get
            Return Me.GetRibbon(Of OnTrackRibbon)()
        End Get
    End Property
End Class
