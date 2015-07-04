<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UIControlDeliverablePanel
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
        Me.PageView = New Telerik.WinControls.UI.RadPageView()
        Me.CoreDataPage = New Telerik.WinControls.UI.RadPageViewPage()
        Me.UiControlDataEntryBox1 = New OnTrack.UI.UIControlDataEntryBox()
        Me.IDEntry = New OnTrack.UI.UIControlDataEntryBox()
        Me.UIDEntry = New OnTrack.UI.UIControlDataEntryBox()
        Me.DESCEntry = New OnTrack.UI.UIControlDataEntryBox()
        Me.DescriptionPage = New Telerik.WinControls.UI.RadPageViewPage()
        Me.SchedulePage = New Telerik.WinControls.UI.RadPageViewPage()
        Me.Office2013LightTheme1 = New Telerik.WinControls.Themes.Office2013LightTheme()
        Me.RichTextBox = New Telerik.WinControls.RichTextBox.RadRichTextBox()
        CType(Me.PageView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PageView.SuspendLayout()
        Me.CoreDataPage.SuspendLayout()
        Me.DescriptionPage.SuspendLayout()
        CType(Me.RichTextBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PageView
        '
        Me.PageView.Controls.Add(Me.CoreDataPage)
        Me.PageView.Controls.Add(Me.DescriptionPage)
        Me.PageView.Controls.Add(Me.SchedulePage)
        Me.PageView.DefaultPage = Me.CoreDataPage
        Me.PageView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PageView.Location = New System.Drawing.Point(0, 0)
        Me.PageView.Name = "PageView"
        Me.PageView.SelectedPage = Me.DescriptionPage
        Me.PageView.Size = New System.Drawing.Size(675, 295)
        Me.PageView.TabIndex = 0
        Me.PageView.Text = "PageView"
        'Me.PageView.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'CoreDataPage
        '
        Me.CoreDataPage.AutoScroll = True
        Me.CoreDataPage.Controls.Add(Me.UiControlDataEntryBox1)
        Me.CoreDataPage.Controls.Add(Me.IDEntry)
        Me.CoreDataPage.Controls.Add(Me.UIDEntry)
        Me.CoreDataPage.Controls.Add(Me.DESCEntry)
        Me.CoreDataPage.ItemSize = New System.Drawing.SizeF(99.0!, 27.0!)
        Me.CoreDataPage.Location = New System.Drawing.Point(5, 31)
        Me.CoreDataPage.Name = "CoreDataPage"
        Me.CoreDataPage.Size = New System.Drawing.Size(665, 259)
        Me.CoreDataPage.Text = "Data Properties"
        '
        'UiControlDataEntryBox1
        '
        Me.UiControlDataEntryBox1.BackColor = System.Drawing.Color.Transparent
        Me.UiControlDataEntryBox1.Controller = Nothing
        Me.UiControlDataEntryBox1.DataSource = Nothing
        Me.UiControlDataEntryBox1.Descriptionsize = CType(0, Short)
        Me.UiControlDataEntryBox1.Entrysize = CType(6, Short)
        Me.UiControlDataEntryBox1.Labelsize = CType(6, Short)
        Me.UiControlDataEntryBox1.Location = New System.Drawing.Point(464, 3)
        Me.UiControlDataEntryBox1.Name = "UiControlDataEntryBox1"
        Me.UiControlDataEntryBox1.ObjectEntryName = "DREV"
        Me.UiControlDataEntryBox1.ObjectName = "DELIVERABLE"
        Me.UiControlDataEntryBox1.Size = New System.Drawing.Size(115, 22)
        Me.UiControlDataEntryBox1.TabIndex = 3
        '
        'IDEntry
        '
        Me.IDEntry.BackColor = System.Drawing.Color.Transparent
        Me.IDEntry.Controller = Nothing
        Me.IDEntry.DataSource = Nothing
        Me.IDEntry.Descriptionsize = CType(5, Short)
        Me.IDEntry.Entrysize = CType(15, Short)
        Me.IDEntry.Labelsize = CType(5, Short)
        Me.IDEntry.Location = New System.Drawing.Point(225, 3)
        Me.IDEntry.Name = "IDEntry"
        Me.IDEntry.ObjectEntryName = "ID"
        Me.IDEntry.ObjectName = "DELIVERABLE"
        Me.IDEntry.Size = New System.Drawing.Size(233, 22)
        Me.IDEntry.TabIndex = 2
        '
        'UIDEntry
        '
        Me.UIDEntry.BackColor = System.Drawing.Color.Transparent
        Me.UIDEntry.Controller = Nothing
        Me.UIDEntry.DataSource = Nothing
        Me.UIDEntry.Descriptionsize = CType(0, Short)
        Me.UIDEntry.Entrysize = CType(6, Short)
        Me.UIDEntry.Labelsize = CType(15, Short)
        Me.UIDEntry.Location = New System.Drawing.Point(3, 3)
        Me.UIDEntry.Name = "UIDEntry"
        Me.UIDEntry.ObjectEntryName = "DLVUID"
        Me.UIDEntry.ObjectName = "DELIVERABLE"
        Me.UIDEntry.Size = New System.Drawing.Size(187, 22)
        Me.UIDEntry.TabIndex = 1
        '
        'DESCEntry
        '
        Me.DESCEntry.BackColor = System.Drawing.Color.Transparent
        Me.DESCEntry.Controller = Nothing
        Me.DESCEntry.DataSource = Nothing
        Me.DESCEntry.Descriptionsize = CType(3, Short)
        Me.DESCEntry.Entrysize = CType(50, Short)
        Me.DESCEntry.Labelsize = CType(15, Short)
        Me.DESCEntry.Location = New System.Drawing.Point(3, 29)
        Me.DESCEntry.Name = "DESCEntry"
        Me.DESCEntry.ObjectEntryName = "DESC"
        Me.DESCEntry.ObjectName = "DELIVERABLE"
        Me.DESCEntry.Size = New System.Drawing.Size(605, 22)
        Me.DESCEntry.TabIndex = 0
        '
        'DescriptionPage
        '
        Me.DescriptionPage.AutoScroll = True
        Me.DescriptionPage.Controls.Add(Me.RichTextBox)
        Me.DescriptionPage.ItemSize = New System.Drawing.SizeF(77.0!, 27.0!)
        Me.DescriptionPage.Location = New System.Drawing.Point(5, 31)
        Me.DescriptionPage.Name = "DescriptionPage"
        Me.DescriptionPage.Size = New System.Drawing.Size(665, 259)
        Me.DescriptionPage.Text = "Description"
        '
        'SchedulePage
        '
        Me.SchedulePage.ItemSize = New System.Drawing.SizeF(64.0!, 27.0!)
        Me.SchedulePage.Location = New System.Drawing.Point(5, 31)
        Me.SchedulePage.Name = "SchedulePage"
        Me.SchedulePage.Size = New System.Drawing.Size(665, 259)
        Me.SchedulePage.Text = "Schedule"
        '
        'RichTextBox
        '
        Me.RichTextBox.AllowDrop = True
        Me.RichTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RichTextBox.HyperlinkToolTipFormatString = Nothing
        Me.RichTextBox.Location = New System.Drawing.Point(0, 0)
        Me.RichTextBox.Name = "RichTextBox"
        Me.RichTextBox.Size = New System.Drawing.Size(665, 259)
        Me.RichTextBox.TabIndex = 0
        'Me.RichTextBox.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'UIControlDeliverablePanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.PageView)
        Me.Name = "UIControlDeliverablePanel"
        Me.Size = New System.Drawing.Size(675, 295)
        CType(Me.PageView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PageView.ResumeLayout(False)
        Me.CoreDataPage.ResumeLayout(False)
        Me.DescriptionPage.ResumeLayout(False)
        CType(Me.RichTextBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PageView As Telerik.WinControls.UI.RadPageView
    Friend WithEvents CoreDataPage As Telerik.WinControls.UI.RadPageViewPage
    Friend WithEvents DESCEntry As UIControlDataEntryBox
    Friend WithEvents DescriptionPage As Telerik.WinControls.UI.RadPageViewPage
    Friend WithEvents SchedulePage As Telerik.WinControls.UI.RadPageViewPage
    Friend WithEvents Office2013LightTheme1 As Telerik.WinControls.Themes.Office2013LightTheme
    Friend WithEvents UIDEntry As UIControlDataEntryBox
    Friend WithEvents IDEntry As OnTrack.UI.UIControlDataEntryBox
    Friend WithEvents UiControlDataEntryBox1 As OnTrack.UI.UIControlDataEntryBox
    Friend WithEvents RichTextBox As Telerik.WinControls.RichTextBox.RadRichTextBox

End Class
