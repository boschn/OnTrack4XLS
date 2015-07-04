<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UIControlViewWorkPanel
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.VSplitContainer = New Telerik.WinControls.UI.RadSplitContainer()
        Me.RightPanel = New Telerik.WinControls.UI.SplitPanel()
        Me.HSplitContainer = New Telerik.WinControls.UI.RadSplitContainer()
        Me.LeftPanel = New Telerik.WinControls.UI.SplitPanel()
        Me.UpperRightPanel = New Telerik.WinControls.UI.SplitPanel()
        Me.LowerRightPanel = New Telerik.WinControls.UI.SplitPanel()
        CType(Me.VSplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.VSplitContainer.SuspendLayout()
        CType(Me.RightPanel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RightPanel.SuspendLayout()
        CType(Me.HSplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.HSplitContainer.SuspendLayout()
        CType(Me.LeftPanel, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.UpperRightPanel, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LowerRightPanel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'VSplitContainer
        '
        Me.VSplitContainer.Controls.Add(Me.LeftPanel)
        Me.VSplitContainer.Controls.Add(Me.RightPanel)
        Me.VSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.VSplitContainer.Location = New System.Drawing.Point(0, 0)
        Me.VSplitContainer.Name = "VSplitContainer"
        '
        '
        '
        Me.VSplitContainer.RootElement.MinSize = New System.Drawing.Size(25, 25)
        Me.VSplitContainer.Size = New System.Drawing.Size(947, 406)
        Me.VSplitContainer.SplitterWidth = 5
        Me.VSplitContainer.TabIndex = 1
        Me.VSplitContainer.TabStop = False
        'Me.VSplitContainer.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'RightPanel
        '
        Me.RightPanel.Controls.Add(Me.HSplitContainer)
        Me.RightPanel.Location = New System.Drawing.Point(187, 0)
        Me.RightPanel.Name = "RightPanel"
        '
        '
        '
        Me.RightPanel.RootElement.MinSize = New System.Drawing.Size(25, 25)
        Me.RightPanel.Size = New System.Drawing.Size(760, 406)
        Me.RightPanel.SizeInfo.AutoSizeScale = New System.Drawing.SizeF(0.306794!, 0.0!)
        Me.RightPanel.SizeInfo.SplitterCorrection = New System.Drawing.Size(289, 0)
        Me.RightPanel.TabIndex = 1
        Me.RightPanel.TabStop = False
        Me.RightPanel.Text = "RightPanel"
        'Me.RightPanel.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'HSplitContainer
        '
        Me.HSplitContainer.Controls.Add(Me.UpperRightPanel)
        Me.HSplitContainer.Controls.Add(Me.LowerRightPanel)
        Me.HSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.HSplitContainer.Location = New System.Drawing.Point(0, 0)
        Me.HSplitContainer.Name = "HSplitContainer"
        Me.HSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        '
        '
        Me.HSplitContainer.RootElement.MinSize = New System.Drawing.Size(25, 25)
        Me.HSplitContainer.Size = New System.Drawing.Size(760, 406)
        Me.HSplitContainer.SplitterWidth = 5
        Me.HSplitContainer.TabIndex = 0
        Me.HSplitContainer.TabStop = False
        Me.HSplitContainer.Text = "HSplitContainer"
        'Me.HSplitContainer.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'LeftPanel
        '
        Me.LeftPanel.Location = New System.Drawing.Point(0, 0)
        Me.LeftPanel.Name = "LeftPanel"
        '
        '
        '
        Me.LeftPanel.RootElement.MinSize = New System.Drawing.Size(25, 25)
        Me.LeftPanel.Size = New System.Drawing.Size(182, 406)
        Me.LeftPanel.SizeInfo.AutoSizeScale = New System.Drawing.SizeF(-0.306794!, 0.0!)
        Me.LeftPanel.SizeInfo.SplitterCorrection = New System.Drawing.Size(-289, 0)
        Me.LeftPanel.TabIndex = 0
        Me.LeftPanel.TabStop = False
        Me.LeftPanel.Text = "LeftPanel"
        'Me.LeftPanel.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'UpperRightPanel
        '
        Me.UpperRightPanel.Location = New System.Drawing.Point(0, 0)
        Me.UpperRightPanel.Name = "UpperRightPanel"
        '
        '
        '
        Me.UpperRightPanel.RootElement.MinSize = New System.Drawing.Size(25, 25)
        Me.UpperRightPanel.Size = New System.Drawing.Size(760, 200)
        Me.UpperRightPanel.TabIndex = 0
        Me.UpperRightPanel.TabStop = False
        Me.UpperRightPanel.Text = "UpperPanel"
        'Me.UpperRightPanel.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'LowerRightPanel
        '
        Me.LowerRightPanel.Location = New System.Drawing.Point(0, 205)
        Me.LowerRightPanel.Name = "LowerRightPanel"
        '
        '
        '
        Me.LowerRightPanel.RootElement.MinSize = New System.Drawing.Size(25, 25)
        Me.LowerRightPanel.Size = New System.Drawing.Size(760, 201)
        Me.LowerRightPanel.TabIndex = 1
        Me.LowerRightPanel.TabStop = False
        Me.LowerRightPanel.Text = "LowerRightPanel"
        'Me.LowerRightPanel.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'UIControlViewWorkPanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.VSplitContainer)
        Me.Name = "UIControlViewWorkPanel"
        Me.Size = New System.Drawing.Size(947, 406)
        CType(Me.VSplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.VSplitContainer.ResumeLayout(False)
        CType(Me.RightPanel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RightPanel.ResumeLayout(False)
        CType(Me.HSplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.HSplitContainer.ResumeLayout(False)
        CType(Me.LeftPanel, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.UpperRightPanel, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LowerRightPanel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents VSplitContainer As Telerik.WinControls.UI.RadSplitContainer
    Friend WithEvents LeftPanel As Telerik.WinControls.UI.SplitPanel
    Friend WithEvents RightPanel As Telerik.WinControls.UI.SplitPanel
    Friend WithEvents HSplitContainer As Telerik.WinControls.UI.RadSplitContainer
    Friend WithEvents UpperRightPanel As Telerik.WinControls.UI.SplitPanel
    Friend WithEvents LowerRightPanel As Telerik.WinControls.UI.SplitPanel

End Class
