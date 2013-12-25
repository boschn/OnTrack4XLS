Imports System.ComponentModel
Imports Telerik.WinControls
Imports Telerik.WinControls.UI
Imports System.Data
Imports OnTrack
Imports OnTrack.Database
Imports OnTrack.XChange

Public Class UIFormWorkXConfig

    Dim _XconfigList As List(Of XConfig)
    Dim _XConfigDataTable As New DataTable
    Dim _XConfigObjectsDataTable As New DataTable
    Dim _xConfigAttributesDataTable As New DataTable

    Public Sub MeOnLoad(sender As Object, e As EventArgs) Handles Me.Load

        ' get the connection


        If CurrentSession.RequireAccessRight(otAccessRight.[ReadOnly]) Then

            ' get the ConfigList
            _XconfigList = XConfig.All
            ' setup of the workspaceID table
            _XConfigDataTable.Columns.Add("Configname", GetType(String))
            _XConfigDataTable.Columns.Add("Description", GetType(String))

            For Each aXconfig As XConfig In _XconfigList
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
        Dim aXConfig As XConfig = _XconfigList.ElementAt(index)

        Me.ConfigNameTb.Text = aXConfig.CONFIGNAME
        Me.DescriptionTB.Text = aXConfig.description
        Me.OutlineCombo.Text = aXConfig.outlineID
        If aXConfig.AllowDynamicAttributes Then
            Me.DynamicIDButton.Text = "is dynamic"
            Me.DynamicIDButton.ToggleState = Enumerations.ToggleState.On

        Else
            Me.DynamicIDButton.Text = "not dynamic"
            Me.DynamicIDButton.ToggleState = Enumerations.ToggleState.Off
        End If


        ' fill the attributes
        Dim attribColl As IEnumerable(Of XConfigAttributeEntry) = aXConfig.Attributes
        Dim xConfigAttributesDataTable As DataTable = New DataTable
        xConfigAttributesDataTable.Columns.Add("ID", GetType(String))
        xConfigAttributesDataTable.Columns.Add("fieldname", GetType(String))
        xConfigAttributesDataTable.Columns.Add("Type", GetType(otFieldDataType))
        xConfigAttributesDataTable.Columns.Add("ordinal", GetType(Long))
        xConfigAttributesDataTable.Columns.Add("Title", GetType(String))
        xConfigAttributesDataTable.Columns.Add("Aliases", GetType(String))

        For Each attrib As XConfigAttributeEntry In attribColl
            xConfigAttributesDataTable.Rows.Add(attrib.ID, _
                                                 attrib.Entryname, _
                                                 attrib.ObjectEntryDefinition.Datatype, _
                                                 attrib.ordinal.Value, _
                                                 attrib.ObjectEntryDefinition.Title, _
                                                 String.Join(",", attrib.ObjectEntryDefinition.Aliases) _
            )

        Next
        Me.XConfigIDsGView.DataSource = xConfigAttributesDataTable
        Me.XConfigIDsGView.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill

        ' fill the objects
        Dim ObjectsColl As List(Of XConfigObjectEntry) = aXConfig.Objects
        Dim xConfigObjectsDataTable = New DataTable
        xConfigObjectsDataTable.Columns.Add("Order", GetType(UShort))
        xConfigObjectsDataTable.Columns.Add("Object name", GetType(String))

        For Each [object] As XConfigObjectEntry In ObjectsColl
            xConfigObjectsDataTable.Rows.Add([object].Orderno, _
                                              [object].Objectname)
        Next
        Me.XConfigObjectsGView.DataSource = xConfigObjectsDataTable
        Me.XConfigObjectsGView.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill

        Me.Refresh()
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelButton.Click, Me.FormClosing
        RadMessageBox.SetThemeName(Me.ThemeName)
        Dim ds As Windows.Forms.DialogResult = _
            RadMessageBox.Show(Me, "Are you sure?", "Cancel", Windows.Forms.MessageBoxButtons.YesNo, RadMessageIcon.Question)

        If ds = Windows.Forms.DialogResult.Yes Then
            'Me.Disposing(sender, e)
            Me.Dispose()
        Else
            Dim FormClosingArgs As System.Windows.Forms.FormClosingEventArgs = TryCast(e, System.Windows.Forms.FormClosingEventArgs)
            If Not FormClosingArgs Is Nothing Then
                FormClosingArgs.Cancel = True
            End If
            Exit Sub
        End If
    End Sub

    Private Sub XConfig1MenuItem_Click(sender As Object, e As EventArgs) Handles CreateDoc9ConfigMenuItem.Click

        'Create the special IDs
        If modDoc9.createDoc9XConfig(otXChangeCommandType.Read) Then
            Me.StatusLabel.Text = MySettings.Default.DefaultDoc9ConfigNameDynamic & " successfully created"
        End If
    End Sub

    Private Sub CreateExpediterMenuItem_Click(sender As Object, e As EventArgs) Handles CreateExpediterConfigMenuItem.Click

        'Create the special IDs
        If modDoc9.createExpediterXConfig(otXChangeCommandType.Read) Then
            Me.StatusLabel.Text = MySettings.Default.DefaultExpediterConfigNameDynamic & " successfully created"
        End If
    End Sub

    Private Sub XConfigIDsGView_Click(sender As Object, e As EventArgs) Handles XConfigIDsGView.Click

    End Sub
End Class
