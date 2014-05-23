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
        Me.ConnectToggleButton = Me.Factory.CreateRibbonToggleButton
        Me.AboutButton = Me.Factory.CreateRibbonButton
        Me.SettingButton = Me.Factory.CreateRibbonButton
        Me.LogButton = Me.Factory.CreateRibbonToggleButton
        Me.ObjectExplorerButton = Me.Factory.CreateRibbonButton
        Me.SettingGroup = Me.Factory.CreateRibbonGroup
        Me.WorkspaceCombo = Me.Factory.CreateRibbonComboBox
        Me.DomainCombo = Me.Factory.CreateRibbonComboBox
        Me.MQFGroup = Me.Factory.CreateRibbonGroup
        Me.DataAreaButton = Me.Factory.CreateRibbonButton
        Me.XConfigButton = Me.Factory.CreateRibbonButton
        Me.MQFAdminButton = Me.Factory.CreateRibbonButton
        Me.ReplicateButton = Me.Factory.CreateRibbonButton
        Me.OperationsGroup = Me.Factory.CreateRibbonGroup
        Me.BatchMenuButton = Me.Factory.CreateRibbonButton
        Me.OnTrackRibbonTab.SuspendLayout()
        Me.OnTrack.SuspendLayout()
        Me.Setting.SuspendLayout()
        Me.SettingGroup.SuspendLayout()
        Me.MQFGroup.SuspendLayout()
        Me.OperationsGroup.SuspendLayout()
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
        Me.OnTrack.Groups.Add(Me.OperationsGroup)
        Me.OnTrack.Label = "OnTrack"
        Me.OnTrack.Name = "OnTrack"
        '
        'Setting
        '
        Me.Setting.Items.Add(Me.ConnectToggleButton)
        Me.Setting.Items.Add(Me.AboutButton)
        Me.Setting.Items.Add(Me.SettingButton)
        Me.Setting.Items.Add(Me.LogButton)
        Me.Setting.Items.Add(Me.ObjectExplorerButton)
        Me.Setting.Label = "OnTrack"
        Me.Setting.Name = "Setting"
        '
        'ConnectToggleButton
        '
        Me.ConnectToggleButton.Description = "Connect to the OnTrack Database"
        Me.ConnectToggleButton.Image = Global.OnTrack.Addin.My.Resources.Resources.disconnect_icon
        Me.ConnectToggleButton.Label = "Connect"
        Me.ConnectToggleButton.Name = "ConnectToggleButton"
        Me.ConnectToggleButton.ShowImage = True
        '
        'AboutButton
        '
        Me.AboutButton.Image = Global.OnTrack.Addin.My.Resources.Resources.cert
        Me.AboutButton.Label = "About"
        Me.AboutButton.Name = "AboutButton"
        Me.AboutButton.ShowImage = True
        '
        'SettingButton
        '
        Me.SettingButton.Image = Global.OnTrack.Addin.My.Resources.Resources.wrench_plus
        Me.SettingButton.Label = "Setting"
        Me.SettingButton.Name = "SettingButton"
        Me.SettingButton.ScreenTip = "OnTrack Property Setting"
        Me.SettingButton.ShowImage = True
        '
        'LogButton
        '
        Me.LogButton.Description = "Show Message Log Window"
        Me.LogButton.Image = Global.OnTrack.Addin.My.Resources.Resources.twitter_2
        Me.LogButton.Label = "MessageLog"
        Me.LogButton.Name = "LogButton"
        Me.LogButton.ShowImage = True
        '
        'ObjectExplorerButton
        '
        Me.ObjectExplorerButton.Description = "Work with OnTrack Object Data"
        Me.ObjectExplorerButton.Image = Global.OnTrack.Addin.My.Resources.Resources.rail_metro_24_2x
        Me.ObjectExplorerButton.Label = "Explorer"
        Me.ObjectExplorerButton.Name = "ObjectExplorerButton"
        Me.ObjectExplorerButton.ScreenTip = "Work with OnTrack Object Data"
        Me.ObjectExplorerButton.ShowImage = True
        Me.ObjectExplorerButton.SuperTip = "Work with OnTrack Object Data "
        '
        'SettingGroup
        '
        Me.SettingGroup.Items.Add(Me.WorkspaceCombo)
        Me.SettingGroup.Items.Add(Me.DomainCombo)
        Me.SettingGroup.Label = "Settings"
        Me.SettingGroup.Name = "SettingGroup"
        '
        'WorkspaceCombo
        '
        Me.WorkspaceCombo.Label = "Current Workspace"
        Me.WorkspaceCombo.Name = "WorkspaceCombo"
        Me.WorkspaceCombo.ScreenTip = "Set the Default workspaceID for this Workbook"
        Me.WorkspaceCombo.Text = Nothing
        '
        'DomainCombo
        '
        Me.DomainCombo.Label = "Current Domain"
        Me.DomainCombo.Name = "DomainCombo"
        Me.DomainCombo.Text = Nothing
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
        'DataAreaButton
        '
        Me.DataAreaButton.Image = Global.OnTrack.Addin.My.Resources.Resources.box
        Me.DataAreaButton.Label = "DataArea"
        Me.DataAreaButton.Name = "DataAreaButton"
        Me.DataAreaButton.ScreenTip = "Work with DataAreas in this Workbook"
        Me.DataAreaButton.ShowImage = True
        '
        'XConfigButton
        '
        Me.XConfigButton.Description = "Work with XChange Configuration"
        Me.XConfigButton.Image = Global.OnTrack.Addin.My.Resources.Resources.setting_config
        Me.XConfigButton.Label = "Config"
        Me.XConfigButton.Name = "XConfigButton"
        Me.XConfigButton.ScreenTip = "Work with XChange Configuration"
        Me.XConfigButton.ShowImage = True
        '
        'MQFAdminButton
        '
        Me.MQFAdminButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge
        Me.MQFAdminButton.Image = Global.OnTrack.Addin.My.Resources.Resources.MessageQueueTube
        Me.MQFAdminButton.Label = "MQF"
        Me.MQFAdminButton.Name = "MQFAdminButton"
        Me.MQFAdminButton.OfficeImageId = "AccessRelinkLists"
        Me.MQFAdminButton.ScreenTip = "Call the MQF Administrator Tool"
        Me.MQFAdminButton.ShowImage = True
        '
        'ReplicateButton
        '
        Me.ReplicateButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge
        Me.ReplicateButton.Image = Global.OnTrack.Addin.My.Resources.Resources.excel_replication
        Me.ReplicateButton.Label = "Replicate"
        Me.ReplicateButton.Name = "ReplicateButton"
        Me.ReplicateButton.ShowImage = True
        '
        'OperationsGroup
        '
        Me.OperationsGroup.Items.Add(Me.BatchMenuButton)
        Me.OperationsGroup.Label = "Data Operations"
        Me.OperationsGroup.Name = "OperationsGroup"
        '
        'BatchMenuButton
        '
        Me.BatchMenuButton.Label = "Batch"
        Me.BatchMenuButton.Name = "BatchMenuButton"
        Me.BatchMenuButton.OfficeImageId = "ClientQueriesMenu"
        Me.BatchMenuButton.ScreenTip = "Runs batch processed operations"
        Me.BatchMenuButton.ShowImage = True
        Me.BatchMenuButton.SuperTip = "Runs batch processed operations "
        '
        'OnTrackRibbon
        '
        Me.Name = "OnTrackRibbon"
        Me.RibbonType = "Microsoft.Excel.Workbook"
        Me.Tabs.Add(Me.OnTrackRibbonTab)
        Me.Tabs.Add(Me.OnTrack)
        Me.OnTrackRibbonTab.ResumeLayout(false)
        Me.OnTrackRibbonTab.PerformLayout
        Me.OnTrack.ResumeLayout(false)
        Me.OnTrack.PerformLayout
        Me.Setting.ResumeLayout(false)
        Me.Setting.PerformLayout
        Me.SettingGroup.ResumeLayout(false)
        Me.SettingGroup.PerformLayout
        Me.MQFGroup.ResumeLayout(false)
        Me.MQFGroup.PerformLayout
        Me.OperationsGroup.ResumeLayout(false)
        Me.OperationsGroup.PerformLayout

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
    Friend WithEvents OperationsGroup As Microsoft.Office.Tools.Ribbon.RibbonGroup
    Friend WithEvents BatchMenuButton As Microsoft.Office.Tools.Ribbon.RibbonButton
    Friend WithEvents DomainCombo As Microsoft.Office.Tools.Ribbon.RibbonComboBox
    Friend WithEvents ObjectExplorerButton As Microsoft.Office.Tools.Ribbon.RibbonButton
End Class

Partial Class ThisRibbonCollection

    <System.Diagnostics.DebuggerNonUserCode()> 
    Friend ReadOnly Property Ribbon1() As OnTrackRibbon
        Get
            Return Me.GetRibbon(Of OnTrackRibbon)()
        End Get
    End Property
End Class
