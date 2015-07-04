Imports Microsoft.Office.Tools.Ribbon
Imports OnTrack

Imports System.Windows.Forms

Public Class OnTrackRibbon

    Private WithEvents _errorlog As clsOTDBErrorLog  ' for the Error Log events
    Private WithEvents _logForm As UIFormMessageLog
    Private WithEvents otdbsession As clsOTDBSession
    Private WithEvents _logFormThread As Threading.Thread

    Private Sub Ribbon_Load(ByVal sender As System.Object, ByVal e As RibbonUIEventArgs) Handles MyBase.Load

        If Globals.ThisAddIn._OTDBSession Is Nothing Then
            OTDB.ApplicationName = My.Application.Info.AssemblyName & OTDBConst_Delimiter & My.Application.Info.Version.ToString
            Globals.ThisAddIn._OTDBSession = OTDB.CurrentSession
        End If
        otdbsession = Globals.ThisAddIn._OTDBSession
        _errorlog = otdbsession.Errorlog
        Globals.ThisAddIn.SetCurrentHost()

        If Not Globals.thisaddin._OTDBSession.IsRunning Then
            Me.WorkspaceCombo.Enabled = False
        Else
            WorkspaceCombo_load()
        End If
    End Sub

    Private Sub OnErrorLog(sender As Object, args As OTDBErrorEventArgs) Handles _errorlog.onErrorRaised
        ' show on bar
        If args.Error.messagetype = otCoreMessageType.ApplicationInfo Then
            Globals.ThisAddIn.Application.StatusBar = Date.Now.ToLocalTime & " INFORMATION: " & args.Error.Message
        End If
    End Sub

    Private Sub Login_Click(sender As Object, e As RibbonControlEventArgs) Handles ConnectToggleButton.Click
        Globals.ThisAddIn.SetCurrentHost()
        If Me.ConnectToggleButton.Label = "Connect" Then
            If otdbsession.StartUp(AccessRequest:=otAccessRight.otReadUpdateData) Then
                Globals.ThisAddIn.Application.StatusBar = Date.Now.ToLocalTime & " INFORMATION: user '" & Globals.ThisAddIn._OTDBSession.OTdbUser.Username & _
                    "' successfully connected to OnTrack " & OTDB.CurrentConnection.DBName & " on " & OTDB.CurrentConnection.PathOrAddress
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
    Private Sub OnSessionStarted(sender As Object, e As OTDBSessionEventArgs) Handles otdbsession.OnStarted
        Me.ConnectToggleButton.Image = Global.OnTrackTool.My.Resources.Resources.connect_icon
        Me.ConnectToggleButton.Label = "Disconnect"
        Me.ConnectToggleButton.ScreenTip = "Disconnect from OnTrack Database"
        Me.ConnectToggleButton.Checked = True
        Me.ConnectToggleButton.ShowImage = True
        Me.ConnectToggleButton.SuperTip = "Disconnect " & Username & " from the OnTrack Database on " & DBConnectionString
        Globals.ThisAddIn.Application.StatusBar = Date.Now.ToLocalTime & " INFORMATION: user '" & Globals.ThisAddIn._OTDBSession.OTdbUser.Username & _
                  "' successfully connected to OnTrack " & OTDB.CurrentConnection.DBName & " on " & OTDB.CurrentConnection.PathOrAddress
        ' set the settings
        WorkspaceCombo_load()
    End Sub
    '******
    '****** EventHandler for DisConnection
    Private Sub OnSessionEnding(sender As Object, e As OTDBSessionEventArgs) Handles otdbsession.OnEnding
        Me.ConnectToggleButton.Image = Global.OnTrackTool.My.Resources.Resources.disconnect_icon
        Me.ConnectToggleButton.Label = "Connect"
        Me.ConnectToggleButton.ScreenTip = "Connect to OnTrack Database"
        Me.ConnectToggleButton.ShowImage = True
        Me.ConnectToggleButton.Checked = False
        Me.ConnectToggleButton.SuperTip = "Connect to the OnTrack Database"

        Globals.ThisAddIn.Application.StatusBar = " Disconnected from OnTrack Database"
        '
        Me.WorkspaceCombo.Enabled = False
    End Sub

    Private Sub WorkspaceCombo_load()
        If Globals.thisaddin._OTDBSession.IsRunning Then
            Dim aWorkspaceCollection = New clsOTDBDefWorkspace().AllByList
            Me.WorkspaceCombo.Items.Clear()
            For Each aWorkspace As clsOTDBDefWorkspace In aWorkspaceCollection
                Dim anItem As Microsoft.Office.Tools.Ribbon.RibbonDropDownItem = Me.Factory.CreateRibbonDropDownItem
                anItem.Label = aWorkspace.ID
                Me.WorkspaceCombo.Items.Add(anItem)
            Next
            WorkspaceCombo.Enabled = True
            WorkspaceCombo.Text = CurrentSession.DefaultWorkspace
            Dim aDefaultWS As New clsOTDBDefWorkspace
            If aDefaultWS.LoadBy(OTDB.CurrentSession.DefaultWorkspace) Then
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
        Dim aSettingForm As New UIFormSetting
        Call aSettingForm.Show()

    End Sub

    Private Sub WorkspaceCombo_TextChanged(sender As Object, e As RibbonControlEventArgs) Handles WorkspaceCombo.TextChanged
        If Globals.thisaddin._OTDBSession.IsRunning Then
            Dim aDefaultWS As New clsOTDBDefWorkspace
            If aDefaultWS.LoadBy(workspace:=Me.WorkspaceCombo.Text) Then
                OTDB.CurrentSession.DefaultWorkspace = Me.WorkspaceCombo.Text
                WorkspaceCombo.ScreenTip = aDefaultWS.description
                Globals.ThisAddIn.Application.StatusBar = "default workspace for this workbook set to '" & aDefaultWS.ID & "' <" & aDefaultWS.description & "> "
            Else
                WorkspaceCombo.Text = OTDB.CurrentSession.DefaultWorkspace
                Globals.ThisAddIn.Application.StatusBar = "workspace was not found in On Track"
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
End Class
