Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UIControlDataEntryBox
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UIControlDataEntryBox))
        Me.Label = New Telerik.WinControls.UI.RadLabel()
        Me.ContextMenu = New Telerik.WinControls.UI.RadContextMenu(Me.components)
        Me.Textbox = New Telerik.WinControls.UI.RadTextBox()
        Me.EntryDescription = New Telerik.WinControls.UI.RadLabel()
        Me.Panel = New Telerik.WinControls.UI.RadPanel()

        Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)

        CType(Me.Label, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Textbox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.EntryDescription, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Panel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label
        '
        resources.ApplyResources(Me.Label, "Label")
        Me.Label.Name = "Label"
        'Me.Label.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'Textbox
        '
        resources.ApplyResources(Me.Textbox, "Textbox")
        Me.Textbox.Name = "Textbox"
        'Me.Textbox.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'EntryDescription
        '
        resources.ApplyResources(Me.EntryDescription, "EntryDescription")
        Me.EntryDescription.CausesValidation = False
        Me.EntryDescription.Name = "EntryDescription"
        'Me.EntryDescription.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
        '
        'Panel
        '
        resources.ApplyResources(Me.Panel, "Panel")
        Me.Panel.Name = "Panel"
        '
        'UIControlDataEntryBox
        '
        Me.Controls.Add(Me.Label)
        Me.Controls.Add(Me.EntryDescription)
        resources.ApplyResources(Me, "$this")
        CType(Me.Label, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Textbox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.EntryDescription, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Panel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label As Telerik.WinControls.UI.RadLabel
    Friend WithEvents ContextMenu As Telerik.WinControls.UI.RadContextMenu
    Friend WithEvents EntryDescription As Telerik.WinControls.UI.RadLabel
    Private WithEvents Textbox As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents Panel As Telerik.WinControls.UI.RadPanel

End Class
