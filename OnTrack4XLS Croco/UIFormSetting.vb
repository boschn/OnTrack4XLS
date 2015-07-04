Imports System.ComponentModel
Imports Telerik.WinControls
Imports Telerik.WinControls.UI
Imports System.Data
Imports OnTrack

Public Class UIFormSetting
    Private store As RadPropertyStore

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.store = Me.CreatePropertyStore()
        Me.RadPropertyGrid.SelectedObject = Me.store
    End Sub

    Private Function CreatePropertyStore() As RadPropertyStore
        Dim myStore As RadPropertyStore = New RadPropertyStore()

        Dim databasetype As PropertyStoreItem = _
            New PropertyStoreItem(GetType(Database.otDBServerType), "Server Type", GetConfigProperty(OTDBConst_Parameter_OTDB_Type))
        myStore.Add(databasetype)
        Dim dataBaseName As PropertyStoreItem = _
            New PropertyStoreItem(GetType(String), "Name of the Database", GetConfigProperty(OTDBConst_Parameter_OTDB_DBNAME))
        myStore.Add(DataBaseName)
        Dim dataBasePath As PropertyStoreItem = _
           New PropertyStoreItem(GetType(String), "Path of the Database", GetConfigProperty(OTDBconst_Parameter_OTDB_DBPATH))
        myStore.Add(DataBasePath)
        Dim dataBaseUser As PropertyStoreItem = _
         New PropertyStoreItem(GetType(String), "Database User", GetConfigProperty(OTDBConst_Parameter_OTDB_DBUSER))
        myStore.Add(DataBaseUser)
        Dim dataBasePassword As PropertyStoreItem = _
        New PropertyStoreItem(GetType(String), "Database Password", GetConfigProperty(OTDBConst_Parameter_OTDB_DBPASSWORD))

        myStore.Add(dataBasePassword)

        Return myStore
    End Function

    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelButton.Click, Me.FormClosing
        RadMessageBox.SetThemeName(Me.ThemeName)
        Dim ds As Windows.Forms.DialogResult = _
            RadMessageBox.Show(Me, "Are you sure?", "Cancel", Windows.Forms.MessageBoxButtons.YesNo, RadMessageIcon.Question)

        If ds = Windows.Forms.DialogResult.Yes Then
            'Me.Disposing(sender, e)
            Me.Close()
        Else
            Dim FormClosingArgs As System.Windows.Forms.FormClosingEventArgs = TryCast(e, System.Windows.Forms.FormClosingEventArgs)
            If Not FormClosingArgs Is Nothing Then
                FormClosingArgs.Cancel = True
            End If
            Exit Sub
        End If

    End Sub


    Private Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        Me.StatusLabel.Text = "properties saved"
        Me.Refresh()
        Threading.Thread.Sleep(500)
        Me.Dispose()
    End Sub

    Private Sub CreateSchemaButton_Click(sender As Object, e As EventArgs) Handles CreateSchemaButton.Click
        If CurrentSession.RequireAccessRight(otAccessRight.otAlterSchema) Then
            Call modCreateDB.createDatabase()
        Else
            With New clsCoreUIMessageBox
                .Message = "Couldn't access or require rights to change the schema of the OnTrack Database"
                .type = clsCoreUIMessageBox.MessageType.Error
                .Show()
            End With
        End If
    End Sub

    Private Sub UIFormSetting_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
