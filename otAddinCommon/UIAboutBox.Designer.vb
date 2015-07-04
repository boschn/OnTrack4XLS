Imports Microsoft.VisualBasic
Imports System

Partial Public Class UIAboutBox
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (Not components Is Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UIAboutBox))
        Me.Office2013LightTheme1 = New Telerik.WinControls.Themes.Office2013LightTheme()
        Me.PageView = New Telerik.WinControls.UI.RadPageView()
        Me.RadPageViewPage1 = New Telerik.WinControls.UI.RadPageViewPage()
        Me.tableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.logoPictureBox = New System.Windows.Forms.PictureBox()
        Me.radLabelProductName = New Telerik.WinControls.UI.RadLabel()
        Me.radLabelVersion = New Telerik.WinControls.UI.RadLabel()
        Me.radLabelCopyright = New Telerik.WinControls.UI.RadLabel()
        Me.radLabelCompanyName = New Telerik.WinControls.UI.RadLabel()
        Me.radTextBoxDescription = New Telerik.WinControls.UI.RadTextBox()
        Me.RadPageViewPage2 = New Telerik.WinControls.UI.RadPageViewPage()
        Me.GVChangeLog = New Telerik.WinControls.UI.RadGridView()
        Me.okRadButton = New Telerik.WinControls.UI.RadButton()
        Me.RadStatusStrip1 = New Telerik.WinControls.UI.RadStatusStrip()
        CType(Me.PageView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PageView.SuspendLayout()
        Me.RadPageViewPage1.SuspendLayout()
        Me.tableLayoutPanel.SuspendLayout()
        CType(Me.logoPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.radLabelProductName, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.radLabelVersion, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.radLabelCopyright, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.radLabelCompanyName, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.radTextBoxDescription, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RadPageViewPage2.SuspendLayout()
        CType(Me.GVChangeLog, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GVChangeLog.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.okRadButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadStatusStrip1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RadStatusStrip1.SuspendLayout()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PageView
        '
        Me.PageView.Controls.Add(Me.RadPageViewPage1)
        Me.PageView.Controls.Add(Me.RadPageViewPage2)
        Me.PageView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PageView.Location = New System.Drawing.Point(9, 9)
        Me.PageView.Name = "PageView"
        Me.PageView.SelectedPage = Me.RadPageViewPage1
        Me.PageView.Size = New System.Drawing.Size(618, 321)
        Me.PageView.TabIndex = 1
        '
        'RadPageViewPage1
        '
        Me.RadPageViewPage1.Controls.Add(Me.tableLayoutPanel)
        Me.RadPageViewPage1.ItemSize = New System.Drawing.SizeF(47.0!, 28.0!)
        Me.RadPageViewPage1.Location = New System.Drawing.Point(10, 37)
        Me.RadPageViewPage1.Name = "RadPageViewPage1"
        Me.RadPageViewPage1.Size = New System.Drawing.Size(597, 273)
        Me.RadPageViewPage1.Text = "About"
        '
        'tableLayoutPanel
        '
        Me.tableLayoutPanel.BackColor = System.Drawing.Color.Transparent
        Me.tableLayoutPanel.ColumnCount = 2
        Me.tableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.0!))
        Me.tableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.0!))
        Me.tableLayoutPanel.Controls.Add(Me.logoPictureBox, 0, 0)
        Me.tableLayoutPanel.Controls.Add(Me.radLabelProductName, 1, 0)
        Me.tableLayoutPanel.Controls.Add(Me.radLabelVersion, 1, 1)
        Me.tableLayoutPanel.Controls.Add(Me.radLabelCopyright, 1, 2)
        Me.tableLayoutPanel.Controls.Add(Me.radLabelCompanyName, 1, 3)
        Me.tableLayoutPanel.Controls.Add(Me.radTextBoxDescription, 1, 4)
        Me.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tableLayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.tableLayoutPanel.Name = "tableLayoutPanel"
        Me.tableLayoutPanel.RowCount = 6
        Me.tableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        Me.tableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        Me.tableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        Me.tableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.62271!))
        Me.tableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.08425!))
        Me.tableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        Me.tableLayoutPanel.Size = New System.Drawing.Size(597, 273)
        Me.tableLayoutPanel.TabIndex = 1
        '
        'logoPictureBox
        '
        Me.logoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.logoPictureBox.Image = CType(resources.GetObject("logoPictureBox.Image"), System.Drawing.Image)
        Me.logoPictureBox.Location = New System.Drawing.Point(3, 3)
        Me.logoPictureBox.Name = "logoPictureBox"
        Me.tableLayoutPanel.SetRowSpan(Me.logoPictureBox, 6)
        Me.logoPictureBox.Size = New System.Drawing.Size(191, 267)
        Me.logoPictureBox.TabIndex = 12
        Me.logoPictureBox.TabStop = False
        '
        'radLabelProductName
        '
        Me.radLabelProductName.Dock = System.Windows.Forms.DockStyle.Fill
        Me.radLabelProductName.Location = New System.Drawing.Point(203, 0)
        Me.radLabelProductName.Margin = New System.Windows.Forms.Padding(6, 0, 3, 0)
        Me.radLabelProductName.MaximumSize = New System.Drawing.Size(0, 17)
        Me.radLabelProductName.Name = "radLabelProductName"
        '
        '
        '
        Me.radLabelProductName.RootElement.MaxSize = New System.Drawing.Size(0, 17)
        Me.radLabelProductName.Size = New System.Drawing.Size(78, 17)
        Me.radLabelProductName.TabIndex = 19
        Me.radLabelProductName.Text = "Product Name"
        Me.radLabelProductName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft
        '
        'radLabelVersion
        '
        Me.radLabelVersion.Dock = System.Windows.Forms.DockStyle.Fill
        Me.radLabelVersion.Location = New System.Drawing.Point(203, 27)
        Me.radLabelVersion.Margin = New System.Windows.Forms.Padding(6, 0, 3, 0)
        Me.radLabelVersion.MaximumSize = New System.Drawing.Size(0, 17)
        Me.radLabelVersion.Name = "radLabelVersion"
        '
        '
        '
        Me.radLabelVersion.RootElement.MaxSize = New System.Drawing.Size(0, 17)
        Me.radLabelVersion.Size = New System.Drawing.Size(44, 17)
        Me.radLabelVersion.TabIndex = 0
        Me.radLabelVersion.Text = "Version"
        Me.radLabelVersion.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft
        '
        'radLabelCopyright
        '
        Me.radLabelCopyright.Dock = System.Windows.Forms.DockStyle.Fill
        Me.radLabelCopyright.Location = New System.Drawing.Point(203, 54)
        Me.radLabelCopyright.Margin = New System.Windows.Forms.Padding(6, 0, 3, 0)
        Me.radLabelCopyright.MaximumSize = New System.Drawing.Size(0, 17)
        Me.radLabelCopyright.Name = "radLabelCopyright"
        '
        '
        '
        Me.radLabelCopyright.RootElement.MaxSize = New System.Drawing.Size(0, 17)
        Me.radLabelCopyright.Size = New System.Drawing.Size(56, 17)
        Me.radLabelCopyright.TabIndex = 21
        Me.radLabelCopyright.Text = "Copyright"
        Me.radLabelCopyright.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft
        '
        'radLabelCompanyName
        '
        Me.radLabelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill
        Me.radLabelCompanyName.Location = New System.Drawing.Point(203, 81)
        Me.radLabelCompanyName.Margin = New System.Windows.Forms.Padding(6, 0, 3, 0)
        Me.radLabelCompanyName.MaximumSize = New System.Drawing.Size(0, 17)
        Me.radLabelCompanyName.Name = "radLabelCompanyName"
        '
        '
        '
        Me.radLabelCompanyName.RootElement.MaxSize = New System.Drawing.Size(0, 17)
        Me.radLabelCompanyName.Size = New System.Drawing.Size(87, 17)
        Me.radLabelCompanyName.TabIndex = 22
        Me.radLabelCompanyName.Text = "Company Name"
        Me.radLabelCompanyName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft
        '
        'radTextBoxDescription
        '
        Me.radTextBoxDescription.AutoSize = False
        Me.radTextBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill
        Me.radTextBoxDescription.Location = New System.Drawing.Point(203, 113)
        Me.radTextBoxDescription.Margin = New System.Windows.Forms.Padding(6, 3, 3, 3)
        Me.radTextBoxDescription.Multiline = True
        Me.radTextBoxDescription.Name = "radTextBoxDescription"
        Me.radTextBoxDescription.ReadOnly = True
        Me.radTextBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.radTextBoxDescription.Size = New System.Drawing.Size(391, 128)
        Me.radTextBoxDescription.TabIndex = 23
        Me.radTextBoxDescription.TabStop = False
        Me.radTextBoxDescription.Text = "Description"
        '
        'RadPageViewPage2
        '
        Me.RadPageViewPage2.Controls.Add(Me.GVChangeLog)
        Me.RadPageViewPage2.ItemSize = New System.Drawing.SizeF(76.0!, 28.0!)
        Me.RadPageViewPage2.Location = New System.Drawing.Point(10, 37)
        Me.RadPageViewPage2.Name = "RadPageViewPage2"
        Me.RadPageViewPage2.Size = New System.Drawing.Size(597, 273)
        Me.RadPageViewPage2.Text = "Change Log"
        '
        'GVChangeLog
        '
        Me.GVChangeLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GVChangeLog.Location = New System.Drawing.Point(0, 0)
        '
        '
        '
        Me.GVChangeLog.MasterTemplate.AllowAddNewRow = False
        Me.GVChangeLog.MasterTemplate.AllowColumnReorder = False
        Me.GVChangeLog.Name = "GVChangeLog"
        Me.GVChangeLog.ReadOnly = True
        Me.GVChangeLog.ShowGroupPanel = False
        Me.GVChangeLog.Size = New System.Drawing.Size(597, 273)
        Me.GVChangeLog.TabIndex = 0
        Me.GVChangeLog.Text = "RadGridView1"
        '
        'okRadButton
        '
        Me.okRadButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.okRadButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.okRadButton.Location = New System.Drawing.Point(520, 2)
        Me.okRadButton.Name = "okRadButton"
        Me.okRadButton.Size = New System.Drawing.Size(75, 21)
        Me.okRadButton.TabIndex = 24
        Me.okRadButton.Text = "&OK"
        '
        'RadStatusStrip1
        '
        Me.RadStatusStrip1.Controls.Add(Me.okRadButton)
        Me.RadStatusStrip1.Location = New System.Drawing.Point(9, 330)
        Me.RadStatusStrip1.Name = "RadStatusStrip1"
        Me.RadStatusStrip1.Size = New System.Drawing.Size(618, 24)
        Me.RadStatusStrip1.TabIndex = 2
        Me.RadStatusStrip1.Text = "RadStatusStrip1"
        '
        'UIAboutBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(636, 363)
        Me.Controls.Add(Me.PageView)
        Me.Controls.Add(Me.RadStatusStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(585, 310)
        Me.Name = "UIAboutBox"
        Me.Padding = New System.Windows.Forms.Padding(9)
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "About OnTrack"
        CType(Me.PageView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PageView.ResumeLayout(False)
        Me.RadPageViewPage1.ResumeLayout(False)
        Me.tableLayoutPanel.ResumeLayout(False)
        Me.tableLayoutPanel.PerformLayout()
        CType(Me.logoPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.radLabelProductName, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.radLabelVersion, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.radLabelCopyright, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.radLabelCompanyName, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.radTextBoxDescription, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RadPageViewPage2.ResumeLayout(False)
        CType(Me.GVChangeLog.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GVChangeLog, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.okRadButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadStatusStrip1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RadStatusStrip1.ResumeLayout(False)
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Friend WithEvents Office2013LightTheme1 As Telerik.WinControls.Themes.Office2013LightTheme
    Friend WithEvents PageView As Telerik.WinControls.UI.RadPageView
    Friend WithEvents RadPageViewPage1 As Telerik.WinControls.UI.RadPageViewPage
    Private WithEvents tableLayoutPanel As System.Windows.Forms.TableLayoutPanel
    Private WithEvents logoPictureBox As System.Windows.Forms.PictureBox
    Private WithEvents radLabelProductName As Telerik.WinControls.UI.RadLabel
    Private WithEvents radLabelVersion As Telerik.WinControls.UI.RadLabel
    Private WithEvents radLabelCopyright As Telerik.WinControls.UI.RadLabel
    Private WithEvents radLabelCompanyName As Telerik.WinControls.UI.RadLabel
    Private WithEvents radTextBoxDescription As Telerik.WinControls.UI.RadTextBox
    Private WithEvents okRadButton As Telerik.WinControls.UI.RadButton
    Friend WithEvents RadPageViewPage2 As Telerik.WinControls.UI.RadPageViewPage
    Friend WithEvents RadStatusStrip1 As Telerik.WinControls.UI.RadStatusStrip
    Friend WithEvents GVChangeLog As Telerik.WinControls.UI.RadGridView
End Class
