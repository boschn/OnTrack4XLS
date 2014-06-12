Imports Microsoft.Office.Tools.Ribbon
Imports OnTrack.Database
Imports OnTrack.Addin
Imports System.Windows.Forms
Imports OnTrack.Commons

Public Class OnTrackRibbon

    Private WithEvents _errorlog As SessionMessageLog  ' for the Error Log events
    Private WithEvents _logForm As UIFormMessageLog
    Private WithEvents otdbsession As Session
    Private WithEvents _logFormThread As Threading.Thread
    Private WithEvents _SettingForm As New UIFormSetting
    Private WithEvents _BatchForm As New UIFormBatchProcesses
    Private WithEvents _replicationForm As UIFormReplication
    Private WithEvents _MQFWizard As UIWizardMQFFeed

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

    ''' <summary>
    ''' OnError Handler for Applicationinfo Output
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    ''' <remarks></remarks>
    Private Sub OnErrorLog(sender As Object, args As ormErrorEventArgs) Handles _errorlog.onErrorRaised
        ' show on bar
        If args.Error.messagetype = otCoreMessageType.ApplicationInfo Then
            Globals.ThisAddIn.Application.StatusBar = Date.Now & " INFORMATION: " & args.Error.Message
        End If
    End Sub

    Private Sub Login_Click(sender As Object, e As RibbonControlEventArgs) Handles ConnectToggleButton.Click
        Globals.ThisAddIn.SetCurrentHost()
        If Me.ConnectToggleButton.Label = "Connect" Then
            Dim domainid As String = Globals.ThisAddIn.CurrentDefaultDomainID
            If otdbsession.StartUp(AccessRequest:=otAccessRight.ReadUpdateData, domainID:=domainid) Then
                Globals.ThisAddIn.Application.StatusBar = Date.Now & " INFORMATION: user '" & Globals.ThisAddIn._OTDBSession.Username & _
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
        Globals.ThisAddIn.Application.StatusBar = Date.Now & " INFORMATION: user '" & Globals.ThisAddIn._OTDBSession.OTdbUser.Username & _
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

    ''' <summary>
    ''' Handle the Domain Change Event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnDomainChanged(sender As Object, e As SessionEventArgs) Handles otdbsession.OnDomainChanged
        DomainCombo.Text = e.Session.CurrentDomainID
    End Sub
    ''' <summary>
    ''' Handle the Domain Change Event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnWorkspaceChanged(sender As Object, e As SessionEventArgs) Handles otdbsession.OnWorkspaceChanged
        WorkspaceCombo.Text = e.Session.CurrentWorkspaceID
    End Sub
    ''' <summary>
    ''' Sub for loading the Combo
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DomainCombo_load()
        If Globals.ThisAddIn._OTDBSession.IsRunning Then
            Me.DomainCombo.Items.Clear()
            For Each aDomain As Domain In Domain.All
                Dim anItem As Microsoft.Office.Tools.Ribbon.RibbonDropDownItem = Me.Factory.CreateRibbonDropDownItem
                anItem.Label = aDomain.ID
                anItem.ScreenTip = aDomain.Description
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

    ''' <summary>
    ''' Event for DomainComboClicked
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnTrackRibbon_OnDomainComboClick(sender As Object, e As RibbonControlEventArgs) Handles DomainCombo.TextChanged
        'Dim aCombo As RibbonComboBox = CType(e.Control, RibbonComboBox)
        If DomainCombo.Text <> CurrentSession.CurrentDomainID Then
            Telerik.WinControls.RadMessageBox.SetThemeName("TelerikMetroBlue")
            Dim aresult As DialogResult = Telerik.WinControls.RadMessageBox.Show(text:="Change current domain to " & DomainCombo.Text, _
                                                          caption:="Please confirm", _
                                                          icon:=Telerik.WinControls.RadMessageIcon.Question, _
                                                          buttons:=MessageBoxButtons.YesNo, _
                                                          defaultButton:=MessageBoxDefaultButton.Button2)
            If aresult = DialogResult.Yes OrElse aresult = DialogResult.OK Then
                CurrentSession.SwitchToDomain(DomainCombo.Text)
            End If
        End If
    End Sub
    ''' <summary>
    ''' load the Combo Box for Workspaces
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub WorkspaceCombo_load()
        If Globals.ThisAddIn._OTDBSession.IsRunning Then
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
        If _MQFWizard Is Nothing OrElse _MQFWizard.IsDisposed Then _MQFWizard = New UIWizardMQFFeed
        Call _MQFWizard.Show()
    End Sub

    Private Sub AboutButton_Click(sender As Object, e As RibbonControlEventArgs) Handles AboutButton.Click
        Globals.ThisAddIn.SetCurrentHost()
        Dim anAbout As New UIAboutBox
        Call anAbout.ShowDialog()

    End Sub

    Private Sub ReplicateButton_Click(sender As Object, e As RibbonControlEventArgs) Handles ReplicateButton.Click
        Globals.ThisAddIn.SetCurrentHost()
        If _replicationForm Is Nothing OrElse _replicationForm.IsDisposed Then _replicationForm = New UIFormReplication
        Call _replicationForm.Show()
    End Sub

    Private Sub SettingButton_Click(sender As Object, e As RibbonControlEventArgs) Handles SettingButton.Click
        Globals.ThisAddIn.SetCurrentHost()

        _SettingForm.RegisterSetHost(AddressOf SetHostProperty)
        Call _SettingForm.Show()

    End Sub

    ''' <summary>
    ''' Event Handler for Workspace Changed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub WorkspaceCombo_TextChanged(sender As Object, e As RibbonControlEventArgs) Handles WorkspaceCombo.TextChanged
        If Globals.ThisAddIn._OTDBSession.IsRunning Then
            Dim aDefaultWS As Workspace = Workspace.Retrieve(id:=Me.WorkspaceCombo.Text)
            If aDefaultWS IsNot Nothing Then
                ot.CurrentSession.CurrentWorkspaceID = Me.WorkspaceCombo.Text
                WorkspaceCombo.ScreenTip = aDefaultWS.Description
                Globals.ThisAddIn.Application.StatusBar = "default workspaceID for this workbook set to '" & aDefaultWS.ID & "' <" & aDefaultWS.Description & "> "
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
            _logForm.Session = Globals.ThisAddIn._OTDBSession
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


    Private Sub ObjectExplorerButton_Click(sender As Object, e As RibbonControlEventArgs) Handles ObjectExplorerButton.Click
        Globals.ThisAddIn.SetCurrentHost()
        Dim aForm As New UIFormDBExplorer
        Call aForm.Show()
    End Sub
End Class
