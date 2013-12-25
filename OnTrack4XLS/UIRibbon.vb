﻿Imports Microsoft.Office.Tools.Ribbon
Imports OnTrack
Imports OnTrack.AddIn
Imports System.Windows.Forms

Public Class OnTrackRibbon

    Private WithEvents _errorlog As ErrorLog  ' for the Error Log events
    Private WithEvents _logForm As UIFormMessageLog
    Private WithEvents otdbsession As Session
    Private WithEvents _logFormThread As Threading.Thread
    Private WithEvents _SettingForm As New UIFormSetting
    Private WithEvents _BatchForm As New UIFormBatchProcesses


    Private Sub Ribbon_Load(ByVal sender As System.Object, ByVal e As RibbonUIEventArgs) Handles MyBase.Load

        If Globals.ThisAddIn._OTDBSession Is Nothing Then
            ot.ApplicationName = My.Application.Info.AssemblyName & ConstDelimiter & My.Application.Info.Version.ToString
            Globals.ThisAddIn._OTDBSession = ot.CurrentSession
        End If
        otdbsession = Globals.ThisAddIn._OTDBSession
        _errorlog = otdbsession.Errorlog
        Globals.ThisAddIn.SetCurrentHost()

        If Not Globals.ThisAddIn._OTDBSession.IsRunning Then
            Me.WorkspaceCombo.Enabled = False
            Me.DomainCombo.Enabled = False
        Else
            WorkspaceCombo_load()
            DomainCombo_load()
        End If
    End Sub

    Private Sub OnErrorLog(sender As Object, args As otErrorEventArgs) Handles _errorlog.onErrorRaised
        ' show on bar
        If args.Error.messagetype = otCoreMessageType.ApplicationInfo Then
            Globals.ThisAddIn.Application.StatusBar = Date.Now.ToLocalTime & " INFORMATION: " & args.Error.Message
        End If
    End Sub

    Private Sub Login_Click(sender As Object, e As RibbonControlEventArgs) Handles ConnectToggleButton.Click
        Globals.ThisAddIn.SetCurrentHost()
        If Me.ConnectToggleButton.Label = "Connect" Then
            If otdbsession.StartUp(AccessRequest:=otAccessRight.ReadUpdateData) Then
                Globals.ThisAddIn.Application.StatusBar = Date.Now.ToLocalTime & " INFORMATION: user '" & Globals.ThisAddIn._OTDBSession.OTdbUser.Username & _
                    "' successfully connected to OnTrack " & ot.CurrentConnection.DBName & " on " & ot.CurrentConnection.PathOrAddress
            Else
                Me.ConnectToggleButton.Checked = False
            End If

        Else
            '*** disconnect
            If Not otdbsession Is Nothing AndAlso otdbsession.ShutDown() Then
                Globals.ThisAddIn.Application.StatusBar = " Disconnected from OnTrack Database"
            End If

        End If

    End Sub

    '******
    '****** EventHandler for Connection
    Private Sub OnSessionStarted(sender As Object, e As SessionEventArgs) Handles otdbsession.OnStarted
        Me.ConnectToggleButton.Image = Global.OnTrack.Addin.My.Resources.Resources.connect_icon
        Me.ConnectToggleButton.Label = "Disconnect"
        Me.ConnectToggleButton.ScreenTip = "Disconnect from OnTrack Database"
        Me.ConnectToggleButton.Checked = True
        Me.ConnectToggleButton.ShowImage = True
        Me.ConnectToggleButton.SuperTip = "Disconnect " & Username & " from the OnTrack Database on " & DBConnectionString
        Globals.ThisAddIn.Application.StatusBar = Date.Now.ToLocalTime & " INFORMATION: user '" & Globals.ThisAddIn._OTDBSession.OTdbUser.Username & _
                  "' successfully connected to OnTrack " & ot.CurrentConnection.DBName & " on " & ot.CurrentConnection.PathOrAddress
        ' set the settings
        WorkspaceCombo_load()
        DomainCombo_load()
    End Sub
    '******
    '****** EventHandler for DisConnection
    Private Sub OnSessionEnding(sender As Object, e As SessionEventArgs) Handles otdbsession.OnEnding
        Me.ConnectToggleButton.Image = Global.OnTrack.Addin.My.Resources.Resources.disconnect_icon
        Me.ConnectToggleButton.Label = "Connect"
        Me.ConnectToggleButton.ScreenTip = "Connect to OnTrack Database"
        Me.ConnectToggleButton.ShowImage = True
        Me.ConnectToggleButton.Checked = False
        Me.ConnectToggleButton.SuperTip = "Connect to the OnTrack Database"

        Globals.ThisAddIn.Application.StatusBar = " Disconnected from OnTrack Database"
        '
        Me.WorkspaceCombo.Enabled = False
        Me.DomainCombo.Enabled = False
    End Sub
    Private Sub DomainCombo_load()
        If Globals.ThisAddIn._OTDBSession.IsRunning Then
            Dim aDomainList = Domain.All
            Me.DomainCombo.Items.Clear()
            For Each aDomain As Domain In aDomainList
                Dim anItem As Microsoft.Office.Tools.Ribbon.RibbonDropDownItem = Me.Factory.CreateRibbonDropDownItem
                anItem.Label = aDomain.ID
                Me.DomainCombo.Items.Add(anItem)
            Next
            DomainCombo.Enabled = True
            DomainCombo.Text = CurrentSession.CurrentDomainID
            Dim aDefaultDomain As Domain = Domain.Retrieve(CurrentSession.CurrentDomainID)
            If aDefaultDomain IsNot Nothing Then
                DomainCombo.ScreenTip = aDefaultDomain.Description
            Else
                DomainCombo.Enabled = False
            End If
        End If
    End Sub
    Private Sub WorkspaceCombo_load()
        If Globals.thisaddin._OTDBSession.IsRunning Then
            Me.WorkspaceCombo.Items.Clear()
            For Each aWorkspace As Workspace In Workspace.All
                Dim anItem As Microsoft.Office.Tools.Ribbon.RibbonDropDownItem = Me.Factory.CreateRibbonDropDownItem
                anItem.Label = aWorkspace.ID
                Me.WorkspaceCombo.Items.Add(anItem)
            Next
            WorkspaceCombo.Enabled = True
            WorkspaceCombo.Text = CurrentSession.CurrentWorkspaceID
            Dim aDefaultWS As Workspace = Workspace.Retrieve(id:=CurrentSession.CurrentWorkspaceID)
            If aDefaultWS IsNot Nothing Then
                WorkspaceCombo.ScreenTip = aDefaultWS.Description
            Else
                WorkspaceCombo.Enabled = False
            End If
        End If
    End Sub
    Private Sub MQFAdminButton_Click(sender As Object, e As RibbonControlEventArgs) Handles MQFAdminButton.Click
        Globals.ThisAddIn.SetCurrentHost()
        Dim aMQFAdmin As New UIWizardMQFFeed
        Call aMQFAdmin.Show()
    End Sub

    Private Sub AboutButton_Click(sender As Object, e As RibbonControlEventArgs) Handles AboutButton.Click
        Globals.ThisAddIn.SetCurrentHost()
        Dim anAbout As New UIAboutBox
        Call anAbout.ShowDialog()

    End Sub

    Private Sub ReplicateButton_Click(sender As Object, e As RibbonControlEventArgs) Handles ReplicateButton.Click
        Globals.ThisAddIn.SetCurrentHost()
        Dim aReplicationForm As New UIFormReplication
        Call aReplicationForm.Show()
    End Sub

    Private Sub SettingButton_Click(sender As Object, e As RibbonControlEventArgs) Handles SettingButton.Click
        Globals.ThisAddIn.SetCurrentHost()

        _SettingForm.RegisterSetHost(AddressOf SetHostProperty)
        Call _SettingForm.Show()

    End Sub

    Private Sub WorkspaceCombo_TextChanged(sender As Object, e As RibbonControlEventArgs) Handles WorkspaceCombo.TextChanged
        If Globals.thisaddin._OTDBSession.IsRunning Then
            Dim aDefaultWS As New Workspace
            If aDefaultWS.LoadBy(workspaceID:=Me.WorkspaceCombo.Text) Then
                ot.CurrentSession.CurrentWorkspaceID = Me.WorkspaceCombo.Text
                WorkspaceCombo.ScreenTip = aDefaultWS.description
                Globals.ThisAddIn.Application.StatusBar = "default workspaceID for this workbook set to '" & aDefaultWS.ID & "' <" & aDefaultWS.description & "> "
            Else
                WorkspaceCombo.Text = ot.CurrentSession.CurrentWorkspaceID
                Globals.ThisAddIn.Application.StatusBar = "workspaceID was not found in On Track"
            End If
        Else
            WorkspaceCombo.Enabled = False
        End If

    End Sub

    Private Sub DataAreaButton_Click(sender As Object, e As RibbonControlEventArgs) Handles DataAreaButton.Click
        Globals.ThisAddIn.SetCurrentHost()
        Dim aForm As New UIFormWorkDataAreas
        Call aForm.Show()
    End Sub



    Private Sub XConfigButton_Click(sender As Object, e As RibbonControlEventArgs) Handles XConfigButton.Click
        Globals.ThisAddIn.SetCurrentHost()
        Dim aForm As New UIFormWorkXConfig
        Call aForm.Show()
    End Sub


    Private Sub LogButton_Click(sender As Object, e As RibbonControlEventArgs) Handles LogButton.Click
        If _logForm Is Nothing OrElse _logForm.IsDisposed Then
            _logForm = New UIFormMessageLog
            _logForm.Session = Globals.thisaddin._OTDBSession
        End If
        If Me.LogButton.Checked = True Then
            _logFormThread = New Threading.Thread(AddressOf _logForm.ShowDialog)
            '_logFormThread.SetApartmentState(Threading.ApartmentState.STA)
            _logFormThread.Start()
            'Call _logForm.Show()
        Else
            'Call _logForm.Close()
            _logFormThread.Abort()
        End If
    End Sub

   
    Private Sub UnToggleLog(sender As Object, e As EventArgs) Handles _logForm.FormClosed
        Me.LogButton.Checked = False
    End Sub

    Private Sub BatchMenuButton_Click(sender As Object, e As RibbonControlEventArgs) Handles BatchMenuButton.Click
        Globals.ThisAddIn.SetCurrentHost()
        Call _BatchForm.Show()
    End Sub

    Private Sub Button1_Click(sender As Object, e As RibbonControlEventArgs)

    End Sub
End Class