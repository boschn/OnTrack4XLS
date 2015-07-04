<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UIFormBatchProcesses
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UIFormBatchProcesses))
        Me.Office2013LightTheme1 = New Telerik.WinControls.Themes.Office2013LightTheme()
        Me.StatusStrip = New Telerik.WinControls.UI.RadStatusStrip()
        Me.StatusLabel = New Telerik.WinControls.UI.RadLabelElement()
        Me.CancelButton = New Telerik.WinControls.UI.RadButtonElement()
        Me.StatusProgress = New Telerik.WinControls.UI.RadProgressBarElement()
        Me.RadPanorama1 = New Telerik.WinControls.UI.RadPanorama()
        Me.TileGroupElement1 = New Telerik.WinControls.UI.TileGroupElement()
        Me.UpdateGaps = New Telerik.WinControls.UI.RadTileElement()
        Me.TileGroupElement2 = New Telerik.WinControls.UI.TileGroupElement()
        Me.buildDependNet = New Telerik.WinControls.UI.RadTileElement()
        Me.BuildCluster = New Telerik.WinControls.UI.RadTileElement()
        Me.CheckDepend = New Telerik.WinControls.UI.RadTileElement()
        Me.RadTileElement1 = New Telerik.WinControls.UI.RadTileElement()
        Me.RadTileElement2 = New Telerik.WinControls.UI.RadTileElement()
        Me.Office2013LightTheme1 = New Telerik.WinControls.Themes.Office2013LightTheme()
        CType(Me.StatusStrip, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadPanorama1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New Telerik.WinControls.RadItem() {Me.StatusLabel, Me.CancelButton, Me.StatusProgress})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 172)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(464, 29)
        Me.StatusStrip.TabIndex = 0
        'Me.StatusStrip.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
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
        Me.CancelButton.Enabled = False
        Me.CancelButton.Name = "CancelButton"
        Me.StatusStrip.SetSpring(Me.CancelButton, False)
        Me.CancelButton.Text = "Cancel"
        Me.CancelButton.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'StatusProgress
        '
        Me.StatusProgress.AutoSize = False
        Me.StatusProgress.Bounds = New System.Drawing.Rectangle(0, 0, 100, 21)
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
        'RadPanorama1
        '
        Me.RadPanorama1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RadPanorama1.Groups.AddRange(New Telerik.WinControls.RadItem() {Me.TileGroupElement1, Me.TileGroupElement2})
        Me.RadPanorama1.Items.AddRange(New Telerik.WinControls.RadItem() {Me.RadTileElement1})
        Me.RadPanorama1.Location = New System.Drawing.Point(0, 0)
        Me.RadPanorama1.Name = "RadPanorama1"
        Me.RadPanorama1.RowsCount = 2
        Me.RadPanorama1.ShowGroups = True
        Me.RadPanorama1.Size = New System.Drawing.Size(464, 172)
        Me.RadPanorama1.TabIndex = 2
        Me.RadPanorama1.Text = "RadPanorama1"
        'Me.RadPanorama1.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'TileGroupElement1
        '
        Me.TileGroupElement1.AccessibleDescription = "Statistics"
        Me.TileGroupElement1.AccessibleName = "Statistics"
        Me.TileGroupElement1.AutoSize = True
        Me.TileGroupElement1.CellSize = New System.Drawing.Size(70, 100)
        Me.TileGroupElement1.Items.AddRange(New Telerik.WinControls.RadItem() {Me.UpdateGaps})
        Me.TileGroupElement1.Name = "TileGroupElement1"
        Me.TileGroupElement1.Text = "Statistics"
        Me.TileGroupElement1.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'UpdateGaps
        '
        Me.UpdateGaps.AccessibleDescription = "UpdateGaps"
        Me.UpdateGaps.AccessibleName = "UpdateGaps"
        Me.UpdateGaps.AutoSize = False
        Me.UpdateGaps.Bounds = New System.Drawing.Rectangle(0, 0, 100, 90)
        Me.UpdateGaps.Name = "UpdateGaps"
        Me.UpdateGaps.Text = "Update Gaps"
        Me.UpdateGaps.TextWrap = True
        Me.UpdateGaps.ToolTipText = "Update all Deliverable Tracks concerning the current gap situation"
        Me.UpdateGaps.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'TileGroupElement2
        '
        Me.TileGroupElement2.AccessibleDescription = "Dependencies"
        Me.TileGroupElement2.AccessibleName = "Dependencies"
        Me.TileGroupElement2.Items.AddRange(New Telerik.WinControls.RadItem() {Me.buildDependNet, Me.BuildCluster, Me.CheckDepend})
        Me.TileGroupElement2.Name = "TileGroupElement2"
        Me.TileGroupElement2.Text = "Dependencies"
        Me.TileGroupElement2.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'buildDependNet
        '
        Me.buildDependNet.AccessibleDescription = "RadTileElement3"
        Me.buildDependNet.AccessibleName = "RadTileElement3"
        Me.buildDependNet.Enabled = False
        Me.buildDependNet.Name = "buildDependNet"
        Me.buildDependNet.Text = "Build Net"
        Me.buildDependNet.TextWrap = True
        Me.buildDependNet.ToolTipText = "Build the dependencies net from all parts"
        Me.buildDependNet.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'BuildCluster
        '
        Me.BuildCluster.AccessibleDescription = "Build Cluster"
        Me.BuildCluster.AccessibleName = "Build Cluster"
        Me.BuildCluster.Column = 1
        Me.BuildCluster.Enabled = False
        Me.BuildCluster.Name = "BuildCluster"
        Me.BuildCluster.Text = "Build Cluster"
        Me.BuildCluster.TextWrap = True
        Me.BuildCluster.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'CheckDepend
        '
        Me.CheckDepend.AccessibleDescription = "Check Net"
        Me.CheckDepend.AccessibleName = "Check Net"
        Me.CheckDepend.Column = 2
        Me.CheckDepend.Enabled = False
        Me.CheckDepend.Name = "CheckDepend"
        Me.CheckDepend.Text = "Check Net"
        Me.CheckDepend.TextWrap = True
        Me.CheckDepend.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'RadTileElement1
        '
        Me.RadTileElement1.AccessibleDescription = "RadTileElement1"
        Me.RadTileElement1.AccessibleName = "RadTileElement1"
        Me.RadTileElement1.Name = "RadTileElement1"
        Me.RadTileElement1.Text = "RadTileElement1"
        Me.RadTileElement1.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'RadTileElement2
        '
        Me.RadTileElement2.AccessibleDescription = "RadTileElement3"
        Me.RadTileElement2.AccessibleName = "RadTileElement3"
        Me.RadTileElement2.Name = "RadTileElement2"
        Me.RadTileElement2.Text = "Build Net"
        Me.RadTileElement2.TextWrap = True
        Me.RadTileElement2.ToolTipText = "Build the dependencies net from all parts"
        Me.RadTileElement2.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'UIFormBatchProcesses
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(464, 201)
        Me.Controls.Add(Me.RadPanorama1)
        Me.Controls.Add(Me.StatusStrip)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "UIFormBatchProcesses"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.Text = "Process in Batch"
        'Me.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        CType(Me.StatusStrip, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadPanorama1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Office2013LightTheme1 As Telerik.WinControls.Themes.Office2013LightTheme
    Friend WithEvents StatusStrip As Telerik.WinControls.UI.RadStatusStrip
    Friend WithEvents StatusLabel As Telerik.WinControls.UI.RadLabelElement
    Friend WithEvents CancelButton As Telerik.WinControls.UI.RadButtonElement
    Friend WithEvents StatusProgress As Telerik.WinControls.UI.RadProgressBarElement
    Friend WithEvents RadPanorama1 As Telerik.WinControls.UI.RadPanorama
    Friend WithEvents TileGroupElement1 As Telerik.WinControls.UI.TileGroupElement
    Friend WithEvents TileGroupElement2 As Telerik.WinControls.UI.TileGroupElement
    Friend WithEvents RadTileElement1 As Telerik.WinControls.UI.RadTileElement
    Friend WithEvents UpdateGaps As Telerik.WinControls.UI.RadTileElement
    Friend WithEvents buildDependNet As Telerik.WinControls.UI.RadTileElement
    Friend WithEvents BuildCluster As Telerik.WinControls.UI.RadTileElement
    Friend WithEvents CheckDepend As Telerik.WinControls.UI.RadTileElement
    Friend WithEvents RadTileElement2 As Telerik.WinControls.UI.RadTileElement

End Class

