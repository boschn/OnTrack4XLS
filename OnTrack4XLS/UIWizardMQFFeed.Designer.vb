<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UIWizardMQFFeed
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UIWizardMQFFeed))
        Me.TelerikMetroBlueTheme1 = New Telerik.WinControls.Themes.TelerikMetroBlueTheme()
        Me.WorkbookListContextmenu = New Telerik.WinControls.UI.RadContextMenu(Me.components)
        Me.RadMenuItem1 = New Telerik.WinControls.UI.RadMenuItem()
        Me.workbookListContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RadWizard = New Telerik.WinControls.UI.RadWizard()
        Me.WizardCompletionPage1 = New Telerik.WinControls.UI.WizardCompletionPage()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.MQFSelectedTextbox = New Telerik.WinControls.UI.RadTextBox()
        Me.WelcomeStatusStrip = New Telerik.WinControls.UI.RadStatusStrip()
        Me.WelcomeStatusLabel = New Telerik.WinControls.UI.RadLabelElement()
        Me.OpenWorkbookButton = New Telerik.WinControls.UI.RadButton()
        Me.WorkbookList = New Telerik.WinControls.UI.RadListControl()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.RadTextBox1 = New Telerik.WinControls.UI.RadTextBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.PreprocessStatusStrip = New Telerik.WinControls.UI.RadStatusStrip()
        Me.PreprocessStatusLabel = New Telerik.WinControls.UI.RadLabelElement()
        Me.PreprocessProgressBar = New Telerik.WinControls.UI.RadProgressBarElement()
        Me.OfficeShape1 = New Telerik.WinControls.UI.OfficeShape()
        Me.ProcessCommandPanel = New Telerik.WinControls.UI.RadPanel()
        Me.RadButton1 = New Telerik.WinControls.UI.RadButton()
        Me.PreProcessButton = New Telerik.WinControls.UI.RadButton()
        Me.PreProcessRadViewGrid = New Telerik.WinControls.UI.RadGridView()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.WizardWelcomePage1 = New Telerik.WinControls.UI.WizardWelcomePage()
        Me.WizardPage1 = New Telerik.WinControls.UI.WizardPage()
        Me.WizardPage2 = New Telerik.WinControls.UI.WizardPage()
        Me.WizardPage3 = New Telerik.WinControls.UI.WizardPage()
        Me.workbookListContextMenuStrip.SuspendLayout()
        CType(Me.RadWizard, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RadWizard.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.MQFSelectedTextbox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WelcomeStatusStrip, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.OpenWorkbookButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WorkbookList, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadTextBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        CType(Me.PreprocessStatusStrip, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ProcessCommandPanel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ProcessCommandPanel.SuspendLayout()
        CType(Me.RadButton1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PreProcessButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PreProcessRadViewGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PreProcessRadViewGrid.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'WorkbookListContextmenu
        '
        Me.WorkbookListContextmenu.Items.AddRange(New Telerik.WinControls.RadItem() {Me.RadMenuItem1})
        Me.WorkbookListContextmenu.ThemeName = "TelerikMetroBlue"
        '
        'RadMenuItem1
        '
        Me.RadMenuItem1.AccessibleDescription = "Open new MQF"
        Me.RadMenuItem1.AccessibleName = "Open new MQF"
        Me.RadMenuItem1.DescriptionText = "Open an additional Excel Workbook"
        Me.RadMenuItem1.Image = Global.OnTrack.Addin.My.Resources.Resources.bt_add
        Me.RadMenuItem1.Name = "RadMenuItem1"
        Me.RadMenuItem1.Text = "Open new MQF"
        Me.RadMenuItem1.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'workbookListContextMenuStrip
        '
        Me.workbookListContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem})
        Me.workbookListContextMenuStrip.Name = "workbookListContextMenuStrip"
        Me.workbookListContextMenuStrip.Size = New System.Drawing.Size(155, 26)
        Me.workbookListContextMenuStrip.Text = "Load Workbook"
        '
        'ToolStripMenuItem
        '
        Me.ToolStripMenuItem.Image = Global.OnTrack.Addin.My.Resources.Resources.bt_add
        Me.ToolStripMenuItem.Name = "ToolStripMenuItem"
        Me.ToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.ToolStripMenuItem.Text = "Add Workbook"
        '
        'RadWizard
        '
        Me.RadWizard.CompletionImage = Global.OnTrack.Addin.My.Resources.Resources.fasttrack
        Me.RadWizard.CompletionPage = Me.WizardCompletionPage1
        Me.RadWizard.Controls.Add(Me.Panel1)
        Me.RadWizard.Controls.Add(Me.Panel2)
        Me.RadWizard.Controls.Add(Me.Panel3)
        Me.RadWizard.Controls.Add(Me.Panel4)
        Me.RadWizard.Controls.Add(Me.Panel5)
        Me.RadWizard.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RadWizard.EnableKeyMap = True
        Me.RadWizard.Location = New System.Drawing.Point(0, 0)
        Me.RadWizard.Mode = Telerik.WinControls.UI.WizardMode.Wizard97
        Me.RadWizard.Name = "RadWizard"
        Me.RadWizard.PageHeaderIcon = CType(resources.GetObject("RadWizard.PageHeaderIcon"), System.Drawing.Image)
        Me.RadWizard.Pages.Add(Me.WizardWelcomePage1)
        Me.RadWizard.Pages.Add(Me.WizardPage1)
        Me.RadWizard.Pages.Add(Me.WizardPage2)
        Me.RadWizard.Pages.Add(Me.WizardPage3)
        Me.RadWizard.Pages.Add(Me.WizardCompletionPage1)
        Me.RadWizard.Size = New System.Drawing.Size(709, 473)
        Me.RadWizard.TabIndex = 0
        Me.RadWizard.Text = "RadWizard1"
        Me.RadWizard.ThemeName = "TelerikMetroBlue"
        Me.RadWizard.WelcomeImage = CType(resources.GetObject("RadWizard.WelcomeImage"), System.Drawing.Image)
        Me.RadWizard.WelcomePage = Me.WizardWelcomePage1
        '
        'WizardCompletionPage1
        '
        Me.WizardCompletionPage1.CompletionImage = CType(resources.GetObject("WizardCompletionPage1.CompletionImage"), System.Drawing.Image)
        Me.WizardCompletionPage1.ContentArea = Me.Panel3
        Me.WizardCompletionPage1.Header = ""
        Me.WizardCompletionPage1.Icon = CType(resources.GetObject("WizardCompletionPage1.Icon"), System.Drawing.Image)
        Me.WizardCompletionPage1.Image = Global.OnTrack.Addin.My.Resources.Resources.MessageQueueTube
        Me.WizardCompletionPage1.ImageAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.WizardCompletionPage1.Name = "WizardCompletionPage1"
        Me.WizardCompletionPage1.Title = "Finished"
        Me.WizardCompletionPage1.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.White
        Me.Panel3.Location = New System.Drawing.Point(130, 69)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(381, 265)
        Me.Panel3.TabIndex = 2
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.White
        Me.Panel1.Controls.Add(Me.PictureBox1)
        Me.Panel1.Controls.Add(Me.TextBox2)
        Me.Panel1.Controls.Add(Me.MQFSelectedTextbox)
        Me.Panel1.Controls.Add(Me.WelcomeStatusStrip)
        Me.Panel1.Controls.Add(Me.OpenWorkbookButton)
        Me.Panel1.Controls.Add(Me.WorkbookList)
        Me.Panel1.Controls.Add(Me.TextBox1)
        Me.Panel1.Controls.Add(Me.RadTextBox1)
        Me.Panel1.Location = New System.Drawing.Point(112, 94)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(597, 331)
        Me.Panel1.TabIndex = 0
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.OnTrack.Addin.My.Resources.Resources.files
        Me.PictureBox1.Location = New System.Drawing.Point(3, 234)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(48, 48)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.PictureBox1.TabIndex = 5
        Me.PictureBox1.TabStop = False
        '
        'TextBox2
        '
        Me.TextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox2.Location = New System.Drawing.Point(57, 234)
        Me.TextBox2.Multiline = True
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(460, 14)
        Me.TextBox2.TabIndex = 4
        Me.TextBox2.Text = "Selected File"
        '
        'MQFSelectedTextbox
        '
        Me.MQFSelectedTextbox.Enabled = False
        Me.MQFSelectedTextbox.EnableTheming = False
        Me.MQFSelectedTextbox.Location = New System.Drawing.Point(57, 254)
        Me.MQFSelectedTextbox.Name = "MQFSelectedTextbox"
        '
        '
        '
        Me.MQFSelectedTextbox.RootElement.Enabled = False
        Me.MQFSelectedTextbox.RootElement.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
        Me.MQFSelectedTextbox.RootElement.Text = ""
        Me.MQFSelectedTextbox.Size = New System.Drawing.Size(460, 28)
        Me.MQFSelectedTextbox.TabIndex = 1
        Me.MQFSelectedTextbox.TabStop = False
        Me.MQFSelectedTextbox.ThemeName = "TelerikMetroBlue"
        CType(Me.MQFSelectedTextbox.GetChildAt(0), Telerik.WinControls.UI.RadTextBoxElement).TextOrientation = System.Windows.Forms.Orientation.Horizontal
        CType(Me.MQFSelectedTextbox.GetChildAt(0), Telerik.WinControls.UI.RadTextBoxElement).Text = ""
        CType(Me.MQFSelectedTextbox.GetChildAt(0), Telerik.WinControls.UI.RadTextBoxElement).Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        '
        'WelcomeStatusStrip
        '
        Me.WelcomeStatusStrip.AutoSize = True
        Me.WelcomeStatusStrip.Items.AddRange(New Telerik.WinControls.RadItem() {Me.WelcomeStatusLabel})
        Me.WelcomeStatusStrip.LayoutStyle = Telerik.WinControls.UI.RadStatusBarLayoutStyle.Stack
        Me.WelcomeStatusStrip.Location = New System.Drawing.Point(0, 306)
        Me.WelcomeStatusStrip.Name = "WelcomeStatusStrip"
        Me.WelcomeStatusStrip.Size = New System.Drawing.Size(597, 25)
        Me.WelcomeStatusStrip.TabIndex = 3
        Me.WelcomeStatusStrip.Text = "RadStatusStrip1"
        Me.WelcomeStatusStrip.ThemeName = "TelerikMetroBlue"
        '
        'WelcomeStatusLabel
        '
        Me.WelcomeStatusLabel.AccessibleDescription = "WelcomeStatusLabel"
        Me.WelcomeStatusLabel.AccessibleName = "WelcomeStatusLabel"
        Me.WelcomeStatusLabel.Alignment = System.Drawing.ContentAlignment.TopLeft
        Me.WelcomeStatusLabel.Name = "WelcomeStatusLabel"
        Me.WelcomeStatusStrip.SetSpring(Me.WelcomeStatusLabel, True)
        Me.WelcomeStatusLabel.Text = ""
        Me.WelcomeStatusLabel.TextWrap = True
        Me.WelcomeStatusLabel.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'OpenWorkbookButton
        '
        Me.OpenWorkbookButton.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.OpenWorkbookButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Image
        Me.OpenWorkbookButton.EnableKeyMap = True
        Me.OpenWorkbookButton.Image = Global.OnTrack.Addin.My.Resources.Resources.bt_add
        Me.OpenWorkbookButton.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.OpenWorkbookButton.Location = New System.Drawing.Point(529, 93)
        Me.OpenWorkbookButton.Name = "OpenWorkbookButton"
        Me.OpenWorkbookButton.Size = New System.Drawing.Size(56, 60)
        Me.OpenWorkbookButton.TabIndex = 2
        Me.OpenWorkbookButton.ThemeName = "TelerikMetroBlue"
        '
        'WorkbookList
        '
        Me.WorkbookList.AllowDrop = True
        Me.WorkbookList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.WorkbookList.AutoScroll = True
        Me.WorkbookList.CaseSensitiveSort = True
        Me.WorkbookList.ContextMenuStrip = Me.workbookListContextMenuStrip
        Me.WorkbookList.ItemHeight = 18
        Me.WorkbookList.Location = New System.Drawing.Point(30, 93)
        Me.WorkbookList.Name = "WorkbookList"
        Me.WorkbookList.Size = New System.Drawing.Size(487, 124)
        Me.WorkbookList.SortStyle = Telerik.WinControls.Enumerations.SortStyle.Ascending
        Me.WorkbookList.TabIndex = 1
        Me.WorkbookList.Text = "open Workbooks"
        Me.WorkbookList.ThemeName = "TelerikMetroBlue"
        '
        'TextBox1
        '
        Me.TextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox1.Location = New System.Drawing.Point(30, 74)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(487, 22)
        Me.TextBox1.TabIndex = 1
        Me.TextBox1.Text = "Please select the file to be fed to the database either already open in Excel or " & _
    "to be loaded"
        '
        'RadTextBox1
        '
        Me.RadTextBox1.Enabled = False
        Me.RadTextBox1.EnableTheming = False
        Me.RadTextBox1.Location = New System.Drawing.Point(26, 16)
        Me.RadTextBox1.Name = "RadTextBox1"
        '
        '
        '
        Me.RadTextBox1.RootElement.Enabled = False
        Me.RadTextBox1.RootElement.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
        Me.RadTextBox1.RootElement.Text = ""
        Me.RadTextBox1.Size = New System.Drawing.Size(559, 28)
        Me.RadTextBox1.TabIndex = 0
        Me.RadTextBox1.TabStop = False
        Me.RadTextBox1.Text = "Welcome to the Message Queue File Wizard"
        Me.RadTextBox1.ThemeName = "TelerikMetroBlue"
        CType(Me.RadTextBox1.GetChildAt(0), Telerik.WinControls.UI.RadTextBoxElement).TextOrientation = System.Windows.Forms.Orientation.Horizontal
        CType(Me.RadTextBox1.GetChildAt(0), Telerik.WinControls.UI.RadTextBoxElement).Text = "Welcome to the Message Queue File Wizard"
        CType(Me.RadTextBox1.GetChildAt(0), Telerik.WinControls.UI.RadTextBoxElement).Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.White
        Me.Panel2.Controls.Add(Me.PreprocessStatusStrip)
        Me.Panel2.Controls.Add(Me.ProcessCommandPanel)
        Me.Panel2.Controls.Add(Me.PreProcessRadViewGrid)
        Me.Panel2.Location = New System.Drawing.Point(0, 94)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(709, 331)
        Me.Panel2.TabIndex = 1
        '
        'PreprocessStatusStrip
        '
        Me.PreprocessStatusStrip.AutoSize = True
        Me.PreprocessStatusStrip.Items.AddRange(New Telerik.WinControls.RadItem() {Me.PreprocessStatusLabel, Me.PreprocessProgressBar})
        Me.PreprocessStatusStrip.LayoutStyle = Telerik.WinControls.UI.RadStatusBarLayoutStyle.Stack
        Me.PreprocessStatusStrip.Location = New System.Drawing.Point(0, 330)
        Me.PreprocessStatusStrip.Name = "PreprocessStatusStrip"
        Me.PreprocessStatusStrip.Size = New System.Drawing.Size(709, 1)
        Me.PreprocessStatusStrip.TabIndex = 2
        Me.PreprocessStatusStrip.Text = "RadStatusStrip2"
        Me.PreprocessStatusStrip.ThemeName = "TelerikMetroBlue"
        '
        'PreprocessStatusLabel
        '
        Me.PreprocessStatusLabel.Name = "PreprocessStatusLabel"
        Me.PreprocessStatusStrip.SetSpring(Me.PreprocessStatusLabel, True)
        Me.PreprocessStatusLabel.Text = ""
        Me.PreprocessStatusLabel.TextWrap = True
        Me.PreprocessStatusLabel.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'PreprocessProgressBar
        '
        Me.PreprocessProgressBar.AccessibleDescription = "0 %"
        Me.PreprocessProgressBar.AccessibleName = "0 %"
        Me.PreprocessProgressBar.DefaultSize = New System.Drawing.Size(100, 20)
        Me.PreprocessProgressBar.Name = "PreprocessProgressBar"
        Me.PreprocessProgressBar.SeparatorColor1 = System.Drawing.Color.White
        Me.PreprocessProgressBar.SeparatorColor2 = System.Drawing.Color.White
        Me.PreprocessProgressBar.SeparatorColor3 = System.Drawing.Color.White
        Me.PreprocessProgressBar.SeparatorColor4 = System.Drawing.Color.White
        Me.PreprocessProgressBar.SeparatorGradientAngle = 0
        Me.PreprocessProgressBar.SeparatorGradientPercentage1 = 0.4!
        Me.PreprocessProgressBar.SeparatorGradientPercentage2 = 0.6!
        Me.PreprocessProgressBar.SeparatorNumberOfColors = 2
        Me.PreprocessProgressBar.Shape = Me.OfficeShape1
        Me.PreprocessProgressBar.ShowProgressIndicators = True
        Me.PreprocessProgressBar.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias
        Me.PreprocessStatusStrip.SetSpring(Me.PreprocessProgressBar, False)
        Me.PreprocessProgressBar.StepWidth = 14
        Me.PreprocessProgressBar.SweepAngle = 90
        Me.PreprocessProgressBar.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'OfficeShape1
        '
        Me.OfficeShape1.RoundedBottom = True
        '
        'ProcessCommandPanel
        '
        Me.ProcessCommandPanel.Controls.Add(Me.RadButton1)
        Me.ProcessCommandPanel.Controls.Add(Me.PreProcessButton)
        Me.ProcessCommandPanel.Dock = System.Windows.Forms.DockStyle.Top
        Me.ProcessCommandPanel.Location = New System.Drawing.Point(0, 0)
        Me.ProcessCommandPanel.Name = "ProcessCommandPanel"
        Me.ProcessCommandPanel.Size = New System.Drawing.Size(709, 72)
        Me.ProcessCommandPanel.TabIndex = 1
        Me.ProcessCommandPanel.ThemeName = "TelerikMetroBlue"
        '
        'RadButton1
        '
        Me.RadButton1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RadButton1.Image = Global.OnTrack.Addin.My.Resources.Resources.options
        Me.RadButton1.Location = New System.Drawing.Point(630, 3)
        Me.RadButton1.Name = "RadButton1"
        Me.RadButton1.Size = New System.Drawing.Size(67, 66)
        Me.RadButton1.TabIndex = 2
        Me.RadButton1.ThemeName = "TelerikMetroBlue"
        '
        'PreProcessButton
        '
        Me.PreProcessButton.AutoSize = True
        Me.PreProcessButton.DisplayStyle = Telerik.WinControls.DisplayStyle.Image
        Me.PreProcessButton.Image = Global.OnTrack.Addin.My.Resources.Resources.bt_play
        Me.PreProcessButton.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.PreProcessButton.Location = New System.Drawing.Point(12, 3)
        Me.PreProcessButton.Name = "PreProcessButton"
        Me.PreProcessButton.Size = New System.Drawing.Size(52, 52)
        Me.PreProcessButton.TabIndex = 1
        Me.PreProcessButton.Text = "Go"
        Me.PreProcessButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.PreProcessButton.ThemeName = "TelerikMetroBlue"
        '
        'PreProcessRadViewGrid
        '
        Me.PreProcessRadViewGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PreProcessRadViewGrid.Location = New System.Drawing.Point(0, 0)
        Me.PreProcessRadViewGrid.Name = "PreProcessRadViewGrid"
        Me.PreProcessRadViewGrid.Size = New System.Drawing.Size(709, 331)
        Me.PreProcessRadViewGrid.TabIndex = 0
        Me.PreProcessRadViewGrid.Text = "preprocessed Messages "
        Me.PreProcessRadViewGrid.ThemeName = "TelerikMetroBlue"
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.White
        Me.Panel4.Location = New System.Drawing.Point(0, 94)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(709, 331)
        Me.Panel4.TabIndex = 3
        '
        'Panel5
        '
        Me.Panel5.BackColor = System.Drawing.Color.White
        Me.Panel5.Location = New System.Drawing.Point(0, 94)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(511, 240)
        Me.Panel5.TabIndex = 4
        '
        'WizardWelcomePage1
        '
        Me.WizardWelcomePage1.ContentArea = Me.Panel1
        Me.WizardWelcomePage1.CustomizePageHeader = True
        Me.WizardWelcomePage1.Header = "Provide what to feed to the database"
        Me.WizardWelcomePage1.Icon = CType(resources.GetObject("WizardWelcomePage1.Icon"), System.Drawing.Image)
        Me.WizardWelcomePage1.Image = CType(resources.GetObject("WizardWelcomePage1.Image"), System.Drawing.Image)
        Me.WizardWelcomePage1.Name = "WizardWelcomePage1"
        Me.WizardWelcomePage1.Text = ""
        Me.WizardWelcomePage1.Title = "Select the message queue files"
        Me.WizardWelcomePage1.ToolTipText = "select the message queue files to be fed into the database"
        Me.WizardWelcomePage1.Visibility = Telerik.WinControls.ElementVisibility.Visible
        Me.WizardWelcomePage1.WelcomeImage = CType(resources.GetObject("WizardWelcomePage1.WelcomeImage"), System.Drawing.Image)
        '
        'WizardPage1
        '
        Me.WizardPage1.ContentArea = Me.Panel2
        Me.WizardPage1.Header = "Preprocess the data to be fed to the database"
        Me.WizardPage1.Icon = CType(resources.GetObject("WizardPage1.Icon"), System.Drawing.Image)
        Me.WizardPage1.Name = "WizardPage1"
        Me.WizardPage1.Title = "Preprocess"
        Me.WizardPage1.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'WizardPage2
        '
        Me.WizardPage2.ContentArea = Me.Panel4
        Me.WizardPage2.Header = "process the fed data to the database"
        Me.WizardPage2.Name = "WizardPage2"
        Me.WizardPage2.Title = "Process"
        Me.WizardPage2.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'WizardPage3
        '
        Me.WizardPage3.ContentArea = Me.Panel5
        Me.WizardPage3.Header = "update the excel data in the loaded excel replicas"
        Me.WizardPage3.Name = "WizardPage3"
        Me.WizardPage3.Title = "Update Excel"
        Me.WizardPage3.Visibility = Telerik.WinControls.ElementVisibility.Visible
        '
        'MQFFeedWizard
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(709, 473)
        Me.Controls.Add(Me.RadWizard)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "MQFFeedWizard"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "OnTrack Message Queue File Wizard"
        Me.ThemeName = "TelerikMetroBlue"
        Me.workbookListContextMenuStrip.ResumeLayout(False)
        CType(Me.RadWizard, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RadWizard.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.MQFSelectedTextbox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WelcomeStatusStrip, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.OpenWorkbookButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WorkbookList, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadTextBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        CType(Me.PreprocessStatusStrip, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ProcessCommandPanel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ProcessCommandPanel.ResumeLayout(False)
        Me.ProcessCommandPanel.PerformLayout()
        CType(Me.RadButton1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PreProcessButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PreProcessRadViewGrid.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PreProcessRadViewGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents RadWizard As Telerik.WinControls.UI.RadWizard
    Friend WithEvents WizardCompletionPage1 As Telerik.WinControls.UI.WizardCompletionPage
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents WizardWelcomePage1 As Telerik.WinControls.UI.WizardWelcomePage
    Friend WithEvents WizardPage1 As Telerik.WinControls.UI.WizardPage
    Friend WithEvents TelerikMetroBlueTheme1 As Telerik.WinControls.Themes.TelerikMetroBlueTheme
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents WizardPage2 As Telerik.WinControls.UI.WizardPage
    Friend WithEvents WizardPage3 As Telerik.WinControls.UI.WizardPage
    Friend WithEvents WorkbookList As Telerik.WinControls.UI.RadListControl
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents RadTextBox1 As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents WorkbookListContextmenu As Telerik.WinControls.UI.RadContextMenu
    Friend WithEvents RadMenuItem1 As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents workbookListContextMenuStrip As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenWorkbookButton As Telerik.WinControls.UI.RadButton
    Friend WithEvents WelcomeStatusStrip As Telerik.WinControls.UI.RadStatusStrip
    Friend WithEvents WelcomeStatusLabel As Telerik.WinControls.UI.RadLabelElement
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
    Friend WithEvents MQFSelectedTextbox As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents PreProcessRadViewGrid As Telerik.WinControls.UI.RadGridView
    Friend WithEvents PreprocessStatusStrip As Telerik.WinControls.UI.RadStatusStrip
    Friend WithEvents ProcessCommandPanel As Telerik.WinControls.UI.RadPanel
    Friend WithEvents PreProcessButton As Telerik.WinControls.UI.RadButton
    Friend WithEvents RadButton1 As Telerik.WinControls.UI.RadButton
    Friend WithEvents PreprocessStatusLabel As Telerik.WinControls.UI.RadLabelElement
    Friend WithEvents PreprocessProgressBar As Telerik.WinControls.UI.RadProgressBarElement
    Friend WithEvents OfficeShape1 As Telerik.WinControls.UI.OfficeShape

End Class

