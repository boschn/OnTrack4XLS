Imports System.ComponentModel
Imports Telerik.WinControls
Imports Telerik.WinControls.UI
Imports System.Data
Imports OnTrack
Imports OnTrack.Database
Imports OnTrack.XChange
Imports OnTrack.AddIn.My
Imports OnTrack.AddIn
Imports OnTrack.Core
''' <summary>
''' Form Class to work with XConfigs
''' </summary>
''' <remarks></remarks>

Public Class UIFormWorkXConfig

    Dim _Connection As ormConnection

    Dim _XconfigList As List(Of XChangeConfiguration)
    Dim _XConfigDataTable As New DataTable
    Dim _XConfigObjectsDataTable As New DataTable
    Dim _xConfigAttributesDataTable As New DataTable

    Public Sub OnLoad(sender As Object, e As EventArgs) Handles Me.Load

        ' get the connection


        If CurrentSession.RequireAccessRight(otAccessRight.[ReadOnly]) Then

            ' get the ConfigList
            _XconfigList = XChangeConfiguration.All
            ' setup of the workspaceID table
            _XConfigDataTable.Columns.Add("Configname", GetType(String))
            _XConfigDataTable.Columns.Add("Description", GetType(String))

            For Each aXconfig In _XconfigList
                _XConfigDataTable.Rows.Add(Trim(aXconfig.Configname), aXconfig.Description)
            Next
            Me.ListXConfigsGV.DataSource = _XConfigDataTable
            Me.ListXConfigsGV.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill
            If _XconfigList.Count > 0 Then
                Me.ListXConfigsGV.SelectedRows.Item(0).IsSelected = True
                LoadDataPanel(0)
            End If
        Else
            Me.StatusLabel.Text = "not connected to the OnTrack database"
        End If


    End Sub
    Public Sub DataPanelOnLoad(sender As Object, e As Telerik.WinControls.UI.GridViewCellEventArgs) Handles ListXConfigsGV.CellClick
        Call LoadDataPanel(e.RowIndex)
    End Sub


    Public Sub LoadDataPanel(ByVal index As UShort)

        ' get the Config
        Dim aXConfig As XChangeConfiguration = _XconfigList.ElementAt(index)

        Me.ConfigNameTb.Text = aXConfig.Configname
        Me.DescriptionTB.Text = aXConfig.Description
        Me.OutlineCombo.Text = aXConfig.OutlineID
        If aXConfig.AllowDynamicEntries Then
            Me.DynamicIDButton.Text = "is dynamic"
            Me.DynamicIDButton.ToggleState = Enumerations.ToggleState.On

        Else
            Me.DynamicIDButton.Text = "not dynamic"
            Me.DynamicIDButton.ToggleState = Enumerations.ToggleState.Off
        End If


        ' fill the attributes
        Dim AttribColl As List(Of XChangeObjectEntry) = aXConfig.GetObjectEntries
        Dim _xConfigAttributesDataTable = New DataTable
        _xConfigAttributesDataTable.Columns.Add("ID", GetType(String))
        _xConfigAttributesDataTable.Columns.Add("fieldname", GetType(String))
        _xConfigAttributesDataTable.Columns.Add("Type", GetType(otDataType))
        _xConfigAttributesDataTable.Columns.Add("ordinal", GetType(Long))
        _xConfigAttributesDataTable.Columns.Add("Title", GetType(String))
        _xConfigAttributesDataTable.Columns.Add("Aliases", GetType(String))

        For Each attrib In AttribColl
            _xConfigAttributesDataTable.Rows.Add(attrib.XID, _
                                                 attrib.ObjectEntryname, _
                                                 attrib.[ObjectEntryDefinition].Datatype, _
                                                 attrib.ordinal, _
                                                 attrib.[ObjectEntryDefinition].Title, _
                                                 String.Join(",", attrib.[ObjectEntryDefinition].Aliases) _
            )

        Next
        Me.XConfigIDsGView.DataSource = _xConfigAttributesDataTable
        Me.XConfigIDsGView.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill

        ' fill the objects
        Dim ObjectsColl = aXConfig.[XChangeObjects]
        Dim _xConfigObjectsDataTable = New DataTable
        _xConfigObjectsDataTable.Columns.Add("Order", GetType(UShort))
        _xConfigObjectsDataTable.Columns.Add("Object name", GetType(String))

        For Each [object] In ObjectsColl
            _xConfigObjectsDataTable.Rows.Add([object].Orderno, _
                                              [object].Objectname)
        Next
        Me.XConfigObjectsGView.DataSource = _xConfigObjectsDataTable
        Me.XConfigObjectsGView.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill

        Me.Refresh()
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelButton.Click
        Me.Close()
    End Sub
    ''' <summary>
    ''' Form Closing Event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="FormClosingArgs"></param>
    ''' <remarks></remarks>
    Private Sub UIFormWorkXConfig_FormClosing(sender As Object, formClosingArgs As System.Windows.Forms.FormClosingEventArgs)
        Dim ds As Windows.Forms.DialogResult = _
            RadMessageBox.Show(Me, "Are you sure?", "Cancel", Windows.Forms.MessageBoxButtons.YesNo, RadMessageIcon.Question)

        If ds <> Windows.Forms.DialogResult.Yes Then
            FormClosingArgs.Cancel = True
        End If

    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub XConfig1MenuItem_Click(sender As Object, e As EventArgs) Handles CreateDoc9ConfigMenuItem.Click


    End Sub

    Private Sub CreateExpediterMenuItem_Click(sender As Object, e As EventArgs) Handles CreateExpediterConfigMenuItem.Click

        'Create the special IDs
        'If createExpediterXConfig(otXChangeCommandType.Read) Then
        '    Me.StatusLabel.Text = MySettings.Default.DefaultExpediterConfigNameDynamic & " successfully created"
        'End If
    End Sub

    Private Sub XConfigIDsGView_Click(sender As Object, e As EventArgs) Handles XConfigIDsGView.Click

    End Sub

    Private Sub CreateDoc18Wpk_Click(sender As Object, e As EventArgs) Handles CreateDoc18Wpkpk.Click
        'Create the special IDs
        If Doc9QuickNDirty.CreateDoc18WkPkConfig() Then
            Me.StatusLabel.Text = " successfully created"
        End If
    End Sub

    Private Sub CreateDoc18ERoadmap_Click(sender As Object, e As EventArgs) Handles CreateDoc18ERoadmap.Click
        If Doc9QuickNDirty.CreateDoc18ERoadmapConfig() Then
            Me.StatusLabel.Text = " successfully created"
        End If
    End Sub

    Private Sub OutlineCombo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles OutlineCombo.SelectedIndexChanged

    End Sub

    ''' <summary>
    ''' New Instance
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AddHandler Me.FormClosing, AddressOf UIFormWorkXConfig_FormClosing
    End Sub
End Class
