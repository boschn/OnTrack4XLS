Imports System.ComponentModel
Imports OnTrack.Database
Imports Telerik.WinControls
Imports Telerik.WinControls.UI
Imports System.Data
Imports OnTrack
Imports OnTrack.AddIn
Imports OnTrack.Core

Imports System
Imports System.Linq
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports System.Windows.Forms

Public Class UIFormSetting
    Private Const constConfigurationName As String = "1ConfigurationName"
    Private Const constConfigDescription As String = "2description"
    Private Const constConfigFileName As String = "3ConfigFileName"
    Private Const constConfigFileLocation As String = "4ConfigFileLocation"
    Private Const constDriverName As String = "DriverName" ''' HACK: Must be same name as the Property


    Private WithEvents _propertyStore As RadPropertyStore
    Private isChanged As Boolean = False
    Public Delegate Function SetHostProperty(ByVal name As String, _
                                          ByVal value As Object, _
                                          ByRef host As Object, _
                                          silent As Boolean) As Boolean
    Private SetHostPropertyDelegate As SetHostProperty
    Private ConfigSetnames As New Dictionary(Of String, ConfigSetModel)


    ''' <summary>
    ''' ModelConverter for the ConfigSet 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ConfigSetModelConverter
        Inherits ExpandableObjectConverter

        Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object, destinationType As Type) As Object
            Return MyBase.ConvertTo(context, culture, value, destinationType)
            If (destinationType = GetType(String)) And TypeOf value Is ConfigSetModel Then
                Dim aValue As ConfigSetModel = TryCast(value, ConfigSetModel)
                If aValue IsNot Nothing Then
                    Return String.Format("[{0}]:{1}", aValue.Database, aValue.DBPath)
                Else
                    Return String.empty
                End If
            Else
                Return MyBase.ConvertTo(context, culture, value, destinationType)
            End If
        End Function

        Public Overrides Function CanConvertFrom(context As ITypeDescriptorContext, sourceType As Type) As Boolean

            Return False

            If sourceType = GetType(String) Then
                Return True
            End If
            Return MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overrides Function ConvertFrom(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object) As Object
            If TypeOf value Is String Then
                Debug.Assert(False)

            End If

            Return MyBase.ConvertFrom(context, culture, value)

        End Function
        Public Overrides Function GetProperties(context As ITypeDescriptorContext, value As Object, attributes() As Attribute) As PropertyDescriptorCollection
            'Return MyBase.GetProperties(context, value, attributes)
            'Return TypeDescriptor.GetProperties(GetType(ConfigSetModel), attributes).Sort({"Name", "DBType", "ConfigSetname", "Sequence", "Path", "DbUser", "DbPassword"})
            Dim aList As PropertyDescriptorCollection = TypeDescriptor.GetProperties(GetType(ConfigSetModel), attributes).Sort()
            Return aList

        End Function

        Public Overrides Function GetPropertiesSupported(context As ITypeDescriptorContext) As Boolean
            Return True
        End Function
    End Class

    ''' <summary>
    ''' ConfigSetModel for nested Properties per ConfigSetModel
    ''' </summary>
    ''' <remarks></remarks>
    <TypeConverter(GetType(ConfigSetModelConverter))> Public Class ConfigSetModel
        Private _configsetName As String = String.Empty
        Private _name As String = String.Empty
        Private _path As String = String.empty
        Private _dbuser As String = String.empty
        Private _dbpassword As String = String.empty
        Private _sequence As ComplexPropertyStore.Sequence = ComplexPropertyStore.Sequence.primary
        Private _description As String = String.empty
        Private _ConnectionString As String = String.Empty
        Private _logagent As Boolean = False
        Private _usemars As Boolean = True
        Private _SetupID As String = String.Empty
        Private _setupDescription As String = String.Empty
        Private _drivername As String = String.Empty
        Private _driverid As String = String.Empty

        ''' <summary>
        ''' Gets or sets the usemars.
        ''' </summary>
        ''' <value>The usemars.</value>
        '''  
        <DisplayName("Use MARS (Sql-Server)")> _
        <Category("Database")> <Browsable(True)> _
        Public Property Usemars() As Boolean
            Get
                Return Me._usemars
            End Get
            Set(value As Boolean)
                Me._usemars = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the logagent.
        ''' </summary>
        ''' <value>The logagent.</value>
        <DisplayName("Start LogAgent")> _
        <Category("Database")> <Browsable(True)> _
     <Description("start loggin agent to submit log messages asynchronous to database")> _
        Public Property Logagent() As Boolean
            Get
                Return Me._logagent
            End Get
            Set(value As Boolean)
                Me._logagent = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the connection string.
        ''' </summary>
        ''' <value>The connection string.</value>

        <DisplayName("Connection String")> _
        <Category("Database")> <Browsable(True)> _
     <Description("connection string to be used to connect to database - will be build if empty")> _
        Public Property ConnectionString() As String
            Get
                Return Me._ConnectionString
            End Get
            Set(value As String)
                Me._ConnectionString = value
            End Set
        End Property

        <DisplayName("Config Set Name")> _
        <Category("Database")> <Browsable(True)> _
     <Description("name of the configuration set")> _
        Public Property ConfigSetname As String
            Set(value As String)
                If value Is Nothing Then value = String.Empty
                _configsetName = value
            End Set
            Get
                Return _configsetName
            End Get
        End Property
        <DisplayName("Config Set Description")> _
         <Category("Database")> <Browsable(True)> _
         <Description("description of the configuration set")> _
        Public Property ConfigSetNDescription As String
            Set(value As String)
                If value Is Nothing Then value = String.empty
                _description = value
            End Set
            Get
                Return _description
            End Get
        End Property
        <DisplayName("Config Set Sequence")> _
        <Category("Database")> <Browsable(True)> _
        <Description("sequence of the configuration")> _
        Public Property Sequence As ComplexPropertyStore.Sequence
            Set(value As ComplexPropertyStore.Sequence)
                _sequence = value
            End Set
            Get
                Return _sequence
            End Get
        End Property

        ''' <summary>
        ''' gets or sets the driver name 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Must be same Name as the constDrivername !</remarks>
        <DisplayName("Database Driver Name")> _
       <Category("Database")> _
       <Browsable(True)> _
       <Description("name of the database")> _
        Public Property DriverName As String
            Set(value As String)
                'If value Is Nothing Then value = 0
                _drivername = value
            End Set
            Get
                Return _drivername
            End Get
        End Property
        ''' <summary>
        ''' gets or sets the driver name 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Must be same Name as the constDrivername !</remarks>
        <DisplayName("Database Driver Instance ID")> _
       <Category("Database")> _
       <Browsable(True)> _
       <Description("id of the database driver instance")> _
        Public Property DriverID As String
            Set(value As String)
                _driverid = value
            End Set
            Get
                Return _driverid
            End Get
        End Property
        <DisplayName("Database Name")> _
        <Category("Database")> <Browsable(True)> _
        <Description("name of the database in connection string")> _
        Public Property Database As String
            Set(value As String)
                If value Is Nothing Then value = String.empty
                _name = value
            End Set
            Get
                Return _name
            End Get
        End Property


        <DisplayName("Database Path")> _
       <Category("Database")> <Browsable(True)> _
       <Description("name of the database path or host address in connection string")> _
        Public Property DBPath As String
            Set(value As String)
                If value Is Nothing Then value = String.empty
                _path = value
            End Set
            Get
                Return _path
            End Get
        End Property
        ''' <summary>
        ''' gets or sets the name of the db user
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DisplayName("Db User")> _
        <Category("Database")> <Browsable(True)> _
        <Description("name of the database user in connection string")> _
        Public Property DbUser As String
            Set(value As String)
                If value Is Nothing Then value = String.empty
                _dbuser = value
            End Set
            Get
                Return _dbuser
            End Get
        End Property
        ''' <summary>
        ''' gets or sets the db user password
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DisplayName("Db User Password")> _
        <Category("Database")> _
        <Browsable(True)> _
        <PasswordPropertyText(True)> _
        <Description("password of the database user in connection string")> _
        Public Property DbUserPassword As String
            Set(value As String)
                If value Is Nothing Then value = String.empty
                _dbpassword = value
            End Set
            Get
                Return _dbpassword
            End Get
        End Property

        <DisplayName("Setup ID")> _
        <Category("Database")> <Browsable(True)> _
        <Description("ID of the setup to be used as object prefix in the database")> _
        Public Property SetupID As String
            Set(value As String)
                If value Is Nothing Then value = String.Empty
                _SetupID = value
            End Set
            Get
                Return _SetupID
            End Get
        End Property
        <DisplayName("Setup Description")> _
        <Category("Database")> <Browsable(True)> _
        <Description("Description of the database setup")> _
        Public Property SetupDescription As String
            Set(value As String)
                If value Is Nothing Then value = String.Empty
                _setupDescription = value
            End Set
            Get
                Return _setupDescription
            End Get
        End Property
        ''' <summary>
        ''' gets the string presentation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Return String.Format("{0}:({1}/{2},{3},{4},{5},{6},{7})", _configsetName, _drivername, _name, _path, _dbuser, _configsetName, _sequence.ToString, _usemars.ToString)
        End Function
    End Class
    ''' <summary>
    ''' Instance
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        AddHandler Me.FormClosing, AddressOf UIFormSetting_FormClosing
    End Sub
    ''' <summary>
    ''' Register the SetHostPropertyFunction
    ''' </summary>
    ''' <param name="delegate"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RegisterSetHost(ByVal [delegate] As SetHostProperty) As Boolean
        Me.SetHostPropertyDelegate = [delegate]
    End Function

    ''' <summary>
    ''' Change Handler for the Properties
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub OnChange(sender As Object, e As PropertyStoreItemValueChangedEventArgs) Handles _propertyStore.ItemValueChanged
        isChanged = True

        If e.Item.PropertyName = constConfigFileLocation Then
            '** TODO: change the name of the SetModel
            Dim path As String = CStr(e.Item.Value)
            Dim configfilename As String = _propertyStore.Item(constConfigFileName).Value.ToString
            Dim changed As Boolean = False

            '** path has a filename
            If System.IO.File.Exists(path) Then
                configfilename = System.IO.Path.GetFileName(path)
                changed = True
            ElseIf Not System.IO.Directory.Exists(path) Then
                Me.StatusLabel.Text = "no directory exists"
                Exit Sub
            End If

            If Mid(path, Len(path), 1) <> "\" Then path = path & "\"
            If File.Exists(path & configfilename) Then
                'reload
                ot.AddConfigFilePath(path)
                If changed Then
                    ot.CurrentConfigFileName = configfilename
                End If

                '* reinitialize OTDB
                ot.Initialize(force:=True)
                Me.UpdatePropertyStore(_propertyStore)
                CoreMessageHandler(message:="configuration file in " & path & configfilename & " found and added to OnTrack Configuration", _
                                   procedure:="UIFormSetting.PropertyStore.ItemValueChanged", messagetype:=otCoreMessageType.ApplicationInfo)
                Me.StatusLabel.Text = "configuration file found and added to OnTrack Configuration"
                Me.Refresh()
            Else
                Me.StatusLabel.Text = "no configuration file found "
            End If
        ElseIf e.Item.PropertyName = constConfigFileName Then
            '** TODO: change the name of the SetModel
            Dim configfilename As String = CStr(e.Item.Value)
            Dim path As String = _propertyStore.Item(constConfigFileLocation).Value.ToString
            If Mid(path, Len(path), 1) <> "\" Then path = path & "\"

            '** path has a filename
            If Not System.IO.Directory.Exists(path) Then
                Me.StatusLabel.Text = "no directory found "
            ElseIf Not System.IO.File.Exists(path & configfilename) Then
                Me.StatusLabel.Text = "no file with that filename found "
            End If

        End If
        'SetConfigProperty(e.Item.PropertyName, weight:=50, value:=e.Item.Value)

    End Sub
    ''' <summary>
    ''' event handler for the Editor Required event of the Property Grid
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadPropertyGrid_EditorRequired(ByVal sender As Object, ByVal e As PropertyGridEditorRequiredEventArgs) Handles RadPropertyGrid.EditorRequired
        If e.Item.Name = constConfigurationName Then
            e.EditorType = GetType(PropertyGridDropDownListEditor)
        ElseIf StrComp(e.Item.Name, constDriverName) = 0 Then
            e.EditorType = GetType(PropertyGridDropDownListEditor)
        ElseIf e.Item.Name = constConfigFileLocation Then
            e.EditorType = GetType(PropertyGridBrowseEditor)
        End If
    End Sub

    ''' <summary>
    ''' Event Handler for the FileEditor Changed Event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FileEditor_Changed(ByVal sender As Object, ByVal e As System.EventArgs)

        Debug.Print(DirectCast(sender, RadBrowseEditorElement).Value)

        Dim configfilename As String = String.empty
        Dim configfilelocation As String = String.Empty
        If Not String.IsNullOrWhiteSpace(DirectCast(sender, RadBrowseEditorElement).Value) Then

            Dim newValue As String = DirectCast(sender, RadBrowseEditorElement).Value.ToString
            Dim configfile As PropertyStoreItem = _propertyStore.Item(constConfigFileName)
            If configfile IsNot Nothing Then
                configfilename = configfile.Value.ToString
            End If
            Dim configlocation As PropertyStoreItem = _propertyStore.Item(constConfigFileLocation)
            If configlocation IsNot Nothing Then
                configfilelocation = configlocation.Value.ToString
            End If

            If System.IO.Path.GetFileName(newValue) <> String.Empty Then
                _propertyStore.Item(constConfigFileName).Value = System.IO.Path.GetFileName(newValue)
            End If
            If System.IO.Path.GetDirectoryName(newValue) <> String.Empty Then
                _propertyStore.Item(constConfigFileLocation).Value = System.IO.Path.GetDirectoryName(newValue)
            End If

            Me.Refresh()
        End If
    End Sub

    ''' <summary>
    ''' Event Handler of the EditorInitialized Event of the Property Grid
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadPropertyGrid_EditorInitalized(ByVal sender As Object, ByVal e As PropertyGridItemEditorInitializedEventArgs) Handles RadPropertyGrid.EditorInitialized

        If e.Item.Name = constConfigurationName Then
            Dim editor As PropertyGridDropDownListEditor = DirectCast(e.Editor, PropertyGridDropDownListEditor)
            Dim editorElement As BaseDropDownListEditorElement = DirectCast(editor.EditorElement, BaseDropDownListEditorElement)
            'editorElement.DropDownStyle = RadDropDownStyle.DropDown

            editorElement.DataSource = ot.ConfigSetNames.FindAll(Function(x) x <> ConstGlobalConfigSetName)
            editorElement.SelectedValue = ot.CurrentConfigSetName
        ElseIf StrComp(e.Item.Name, constDriverName) = 0 Then
            Dim editor As PropertyGridDropDownListEditor = DirectCast(e.Editor, PropertyGridDropDownListEditor)
            Dim editorElement As BaseDropDownListEditorElement = DirectCast(editor.EditorElement, BaseDropDownListEditorElement)
            'editorElement.DropDownStyle = RadDropDownStyle.DropDown

            editorElement.DataSource = ot.ObjectClassRepository.GetDBDriverAttributes.Where(Function(x) x.IsOnTrackDriver).Select(Function(x) x.Name).ToList
            If ot.CurrentOTDBDriver IsNot Nothing Then
                editorElement.SelectedValue = ot.CurrentOTDBDriver.Name
            ElseIf HasConfigProperty(constDriverName) Then
                editorElement.SelectedValue = GetConfigProperty(constDriverName)
            End If

        ElseIf e.Item.Name = constConfigFileLocation Then

            Dim editor As PropertyGridBrowseEditor = TryCast(e.Editor, PropertyGridBrowseEditor) 'New PropertyGridBrowseEditor()
            Dim element As RadBrowseEditorElement = TryCast(editor.EditorElement, RadBrowseEditorElement)
            element.DialogType = BrowseEditorDialogType.OpenFileDialog
            'e.Editor = editor

            Dim openDialog As OpenFileDialog = DirectCast(element.Dialog, OpenFileDialog)
            'openDialog.Filter = "OnTrack Config Files (*.ini)"
            openDialog.CheckFileExists = True
            openDialog.CheckPathExists = True
            openDialog.Multiselect = False
            openDialog.Title = "select OnTrack configuration file"

            AddHandler element.ValueChanged, AddressOf FileEditor_Changed
        End If
    End Sub
    ''' <summary>
    ''' creates the Property Store for the configuration ITems
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdatePropertyStore(Optional store As RadPropertyStore = Nothing) As RadPropertyStore
        Dim myStore As RadPropertyStore = New RadPropertyStore()
        Dim aPropertyName As String = String.empty
        If store IsNot Nothing Then
            myStore = store
        End If

        '** reread
        ot.RetrieveConfigProperties(force:=False)
        '** check
        Dim currentconfig As PropertyStoreItem = myStore.Item(constConfigurationName)
        If currentconfig Is Nothing Then
            currentconfig = New PropertyStoreItem(GetType(String), constConfigurationName, ot.CurrentConfigSetName)
            myStore.Add(currentconfig)
        End If
        If currentconfig.Value IsNot Nothing AndAlso currentconfig.Value <> ot.CurrentConfigSetName Then
            currentconfig.Value = ot.CurrentConfigSetName
        End If
        currentconfig.Value = ot.CurrentConfigSetName
        currentconfig.Description = "current configuration set to be used"
        currentconfig.Label = "current configuration set"

        If ot.CurrentSession.IsRunning Then currentconfig.ReadOnly = True

        Dim description As PropertyStoreItem = myStore.Item(constConfigDescription)
        If description Is Nothing Then
            description = New PropertyStoreItem(GetType(String), constConfigDescription, GetConfigProperty(ConstCPNDescription))
            myStore.Add(description)
        End If
        If description.Value IsNot Nothing AndAlso description.Value <> GetConfigProperty(ConstCPNDescription) Then
            description.Value = GetConfigProperty(ConstCPNDescription)
        End If
        description.Value = GetConfigProperty(ConstCPNDescription)
        description.Description = "description of the current configuration set"
        description.Label = "current configuration description"
        If ot.CurrentSession.IsRunning Then description.ReadOnly = True

        Dim configfilename As PropertyStoreItem = myStore.Item(constConfigFileName)
        If configfilename Is Nothing Then
            configfilename = New PropertyStoreItem(GetType(String), constConfigFileName, GetConfigProperty(ConstCPNConfigFileName))
            myStore.Add(configfilename)
        End If
        If configfilename.Value IsNot Nothing AndAlso configfilename.Value <> GetConfigProperty(ConstCPNConfigFileName) Then
            configfilename.Value = GetConfigProperty(ConstCPNConfigFileName)
        End If
        configfilename.Description = "name of the current configuration file"
        configfilename.Label = "configuration file name"
        If ot.CurrentSession.IsRunning Then configfilename.ReadOnly = True

        Dim configlocation As PropertyStoreItem = myStore.Item(constConfigFileLocation)
        If configlocation Is Nothing Then
            configlocation = New PropertyStoreItem(GetType(String), constConfigFileLocation, ot.UsedConfigFileLocation)
            myStore.Add(configlocation)
        End If
        If configlocation.Value IsNot Nothing AndAlso configlocation.Value <> ot.UsedConfigFileLocation Then
            configlocation.Value = ot.UsedConfigFileLocation
        End If
        configlocation.Description = "location of the current configuration file"
        configlocation.Label = "configuration file location"
        If ot.CurrentSession.IsRunning Then configlocation.ReadOnly = True

        '*** remove all config sets
        Dim aList As New List(Of String)
        ConfigSetnames.Clear()
        For Each aProperty In myStore
            If aProperty.PropertyName Like "&*" Then
                aList.Add(aProperty.PropertyName)
            End If
        Next
        For Each aName In aList
            myStore.Remove(aName)
            'Dim aValue As String = aProperty.PropertyName
            'If aValue.Contains(ConstDelimiter) Then
            '    Dim aName As String = aValue.Split(ConstDelimiter).ElementAt(1)
            '    If Not ot.ConfigSetNames.Contains(aName) Then
            '        myStore.Remove(aName)
            '    ElseIf Not ot.HasConfigSetProperty(ConstCPNDBType, configsetname:=aName, sequence:=ComplexPropertyStore.Sequence.primary) Then
            '        myStore.Remove(aName)
            '    ElseIf Not ot.HasConfigSetProperty(ConstCPNDBType, configsetname:=aName, sequence:=ComplexPropertyStore.Sequence.secondary) Then
            '        myStore.Remove(aName)
            '    End If
            'End If
        Next

        '*** add configsets
        Dim i As UShort = 1
        For Each aConfigSetName In ot.ConfigSetNamesToSelect
            For Each aSequence As ComplexPropertyStore.Sequence In [Enum].GetValues(GetType(ComplexPropertyStore.Sequence))
                If ot.HasConfigSetName(configsetname:=aConfigSetName, sequence:=aSequence) Then
                    Dim aConfigSetModel As New ConfigSetModel
                    With aConfigSetModel
                        .ConfigSetname = aConfigSetName
                        .Sequence = aSequence
                        .DriverName = GetConfigProperty(ConstCPNDriverName, configsetname:=aConfigSetName, sequence:=aSequence)
                        .DriverID = GetConfigProperty(ConstCPNDriverID, configsetname:=aConfigSetName, sequence:=aSequence)
                        .DbUser = GetConfigProperty(ConstCPNDBUser, configsetname:=aConfigSetName, sequence:=aSequence)
                        .DbUserPassword = GetConfigProperty(ConstCPNDBPassword, configsetname:=aConfigSetName, sequence:=aSequence)
                        .Database = GetConfigProperty(ConstCPNDBName, configsetname:=aConfigSetName, sequence:=aSequence)
                        .DBPath = GetConfigProperty(ConstCPNDBPath, configsetname:=aConfigSetName, sequence:=aSequence)
                        .ConfigSetNDescription = GetConfigProperty(ConstCPNDescription, configsetname:=aConfigSetName, sequence:=aSequence)
                        .ConnectionString = GetConfigProperty(ConstCPNDBConnection, configsetname:=aConfigSetName, sequence:=aSequence)
                        .Logagent = GetConfigProperty(constCPNUseLogAgent, configsetname:=aConfigSetName, sequence:=aSequence)
                        .Usemars = GetConfigProperty(ConstCPNDBSQLServerUseMars, configsetname:=aConfigSetName, sequence:=aSequence)
                        .SetupDescription = GetConfigProperty(ConstCPNSetupDescription, configsetname:=aConfigSetName, sequence:=aSequence)
                        .SetupID = GetConfigProperty(ConstCPNSetupID, configsetname:=aConfigSetName, sequence:=aSequence)
                    End With

                    aPropertyName = "&" & i & ConstDelimiter & "ConfigurationSet" & ConstDelimiter & aSequence.ToString
                    If ConfigSetnames.ContainsKey(aPropertyName) Then
                        ConfigSetnames.Remove(aPropertyName)
                    End If
                    ConfigSetnames.Add(key:=aPropertyName, value:=aConfigSetModel)

                    Dim configset As PropertyStoreItem = myStore.Item(aPropertyName)
                    If configset Is Nothing Then
                        configset = New PropertyStoreItem(GetType(UIFormSetting.ConfigSetModel), aPropertyName, aConfigSetModel)
                        '** add only if there is a sequence
                        If Not String.IsNullOrEmpty(aConfigSetModel.DriverName) AndAlso Not String.IsNullOrEmpty(aConfigSetModel.Database) Then
                            configset.Label = "DB Configuration #" & i & ":" & aSequence.ToString
                            myStore.Add(configset)
                        End If
                    End If
                    configset.Value = aConfigSetModel
                    configset.Description = GetConfigProperty(ConstCPNDescription, configsetname:=aConfigSetName)
                    configset.Label = "DB Configuration #" & i & ":" & aSequence.ToString
                End If
            Next
            i += 1 ' next configuration (primary & secondary belong together)
        Next

        Return myStore
    End Function

    '''
    ''' <summary>
    ''' Cancel Button Handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelButton.Click
        Me.Close()
    End Sub
    ''' <summary>
    ''' FormClosing Event Handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="formClosingArgs"></param>
    ''' <remarks></remarks>
    Private Sub UIFormSetting_FormClosing(sender As Object, formClosingArgs As System.Windows.Forms.FormClosingEventArgs)
        Dim ds As Windows.Forms.DialogResult
        If isChanged Then
            'RadMessageBox.SetThemeName(Me.ThemeName)
            ds = RadMessageBox.Show(Me, "Are you sure?", "Cancel", Windows.Forms.MessageBoxButtons.YesNo, RadMessageIcon.Question)
        Else
            ds = Windows.Forms.DialogResult.Yes
        End If
        formClosingArgs.Cancel = True
        If ds = Windows.Forms.DialogResult.Yes Then
            Me.Hide()
        End If
    End Sub
    ''' <summary>
    ''' Create Schema Handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>


    Private Sub ButtonCreateSchema_Click(sender As Object, e As EventArgs) Handles ButtonCreateSchema.Click

        If ot.RequireAccess(otAccessRight.AlterSchema) Then
            Global.OnTrack.Database.Installation.CreateDatabase(ot.InstalledModules)
        Else
            ot.CoreMessageHandler(message:="couldn't acquire the necessary rights to continue this operation", _
                                         messagetype:=otCoreMessageType.ApplicationError, procedure:="UIFormSetting.CreateSchemaButton")

        End If
    End Sub
    ''' <summary>
    ''' Onload of Form handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub UIFormSetting_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated

        Me.RadPropertyGrid.SelectedObject = Me._propertyStore
        isChanged = False
    End Sub

    ''' <summary>
    ''' Event handler for save in Document
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub SaveInDocument_Click(sender As Object, e As EventArgs) Handles SaveDocumentMenuButton.Click
        Try
            Dim configsetname As PropertyStoreItem = _propertyStore.Item(constConfigurationName)
            If configsetname IsNot Nothing AndAlso configsetname.Value IsNot Nothing AndAlso LCase(CStr(configsetname.Value)) <> LCase(ot.CurrentConfigSetName) Then
                ot.CurrentConfigSetName = configsetname.Value.ToString
                SetHostPropertyDelegate(name:=ConstCPNUseConfigSetName, value:=configsetname.Value, host:=Nothing, silent:=False)
                ot.CoreMessageHandler(message:="Current database configuration changed to " & CStr(configsetname.Value), _
                                      messagetype:=otCoreMessageType.ApplicationInfo, procedure:="UIFormSetting.SaveInDocument")
            End If

            Dim description As PropertyStoreItem = _propertyStore.Item(constConfigDescription)
            If description IsNot Nothing Then
                SetConfigProperty(ot.ConstCPNDescription, weight:=50, value:=description.Value)
                SetHostPropertyDelegate(name:=ot.ConstCPNDescription, value:=description.Value, host:=Nothing, silent:=False)
            End If

            Dim configfilename As PropertyStoreItem = _propertyStore.Item(ConstCPNConfigFileName)
            If configfilename IsNot Nothing Then
                SetConfigProperty(ot.ConstCPNConfigFileName, weight:=50, value:=configfilename.Value)
                SetHostPropertyDelegate(name:=ot.ConstCPNConfigFileName, value:=configfilename.Value, host:=Nothing, silent:=False)
            End If

            Dim configfilelocation As PropertyStoreItem = _propertyStore.Item(constConfigFileLocation)
            If configfilelocation IsNot Nothing Then
                ot.AddConfigFilePath(configfilelocation.Value)
                SetHostPropertyDelegate(name:=ot.ConstCPNConfigFileLocation, value:=configfilelocation.Value, host:=Nothing, silent:=False)
            End If

            For Each aProperty In _propertyStore
                Dim aValue As String = aProperty.PropertyName

                If aValue.Split(ConstDelimiter).Length >= 2 Then
                    Dim aName As String = aValue.Split(ConstDelimiter).ElementAt(1)
                    If LCase(aName) = "configurationset" Then
                        Dim aConfigSetModel As ConfigSetModel = TryCast(aProperty.Value, ConfigSetModel)
                        If aConfigSetModel IsNot Nothing _
                        AndAlso aConfigSetModel.ConfigSetname = configsetname.Value.ToString _
                        And aConfigSetModel.Sequence = ComplexPropertyStore.Sequence.Primary Then
                            With aConfigSetModel
                                If .Database <> String.empty Then
                                    SetHostPropertyDelegate(name:=ConstCPNDBName, value:=.Database, host:=Nothing, silent:=False)
                                    SetConfigProperty(ConstCPNDBName, weight:=50, value:=.Database, configsetname:=configsetname.Value.ToString, sequence:=ComplexPropertyStore.Sequence.Primary)
                                End If

                                If .DBPath <> String.empty Then
                                    SetHostPropertyDelegate(name:=ConstCPNDBPath, value:=.DBPath, host:=Nothing, silent:=False)
                                    SetConfigProperty(ConstCPNDBPath, weight:=50, value:=.DBPath, configsetname:=configsetname.Value.ToString, sequence:=ComplexPropertyStore.Sequence.Primary)
                                End If

                                If .DbUser <> String.Empty Then
                                    SetHostPropertyDelegate(name:=ConstCPNDBUser, value:=.DbUser, host:=Nothing, silent:=False)
                                    SetConfigProperty(ConstCPNDBUser, weight:=50, value:=.DbUser, configsetname:=configsetname.Value.ToString, sequence:=ComplexPropertyStore.Sequence.Primary)
                                End If

                                If .DbUserPassword <> String.empty Then
                                    SetHostPropertyDelegate(name:=ConstCPNDBPassword, value:=.DbUserPassword, host:=Nothing, silent:=False)
                                    SetConfigProperty(ConstCPNDBPassword, weight:=50, value:=.DbUserPassword, configsetname:=configsetname.Value.ToString, sequence:=ComplexPropertyStore.Sequence.Primary)
                                End If

                                If .ConfigSetNDescription <> String.empty Then
                                    SetHostPropertyDelegate(name:=ConstCPNDescription, value:=.ConfigSetNDescription, host:=Nothing, silent:=False)
                                    SetConfigProperty(name:=ConstCPNDescription, weight:=50, value:=.ConfigSetNDescription, configsetname:=configsetname.Value.ToString, sequence:=ComplexPropertyStore.Sequence.Primary)
                                End If

                                If Not String.IsNullOrEmpty(.DriverName) Then
                                    SetHostPropertyDelegate(name:=ConstCPNDriverName, value:=.DriverName, host:=Nothing, silent:=False)
                                    SetConfigProperty(name:=ConstCPNDriverName, weight:=50, value:=.DriverName, configsetname:=configsetname.Value.ToString, sequence:=ComplexPropertyStore.Sequence.Primary)
                                End If
                                If Not String.IsNullOrEmpty(.DriverID) Then
                                    SetHostPropertyDelegate(name:=ConstCPNDriverID, value:=.DriverName, host:=Nothing, silent:=False)
                                    SetConfigProperty(name:=ConstCPNDriverID, weight:=50, value:=.DriverName, configsetname:=configsetname.Value.ToString, sequence:=ComplexPropertyStore.Sequence.Primary)
                                End If
                                If .ConnectionString <> String.empty Then
                                    SetHostPropertyDelegate(name:=ConstCPNDBConnection, value:=.ConnectionString, host:=Nothing, silent:=False)
                                    SetConfigProperty(name:=ConstCPNDBConnection, weight:=50, value:=.ConnectionString, configsetname:=configsetname.Value.ToString, sequence:=ComplexPropertyStore.Sequence.Primary)
                                End If

                                If .SetupID <> String.empty Then
                                    SetHostPropertyDelegate(name:=ConstCPNSetupID, value:=.SetupID, host:=Nothing, silent:=False)
                                    SetConfigProperty(name:=ConstCPNSetupID, weight:=50, value:=.SetupID, configsetname:=configsetname.Value.ToString, sequence:=ComplexPropertyStore.Sequence.Primary)
                                End If

                                If .SetupDescription <> String.empty Then
                                    SetHostPropertyDelegate(name:=ConstCPNSetupDescription, value:=.SetupDescription, host:=Nothing, silent:=False)
                                    SetConfigProperty(name:=ConstCPNSetupDescription, weight:=50, value:=.SetupDescription, configsetname:=configsetname.Value.ToString, sequence:=ComplexPropertyStore.Sequence.Primary)
                                End If
                                SetHostPropertyDelegate(name:=constCPNUseLogAgent, value:=.Logagent, host:=Nothing, silent:=False)
                                SetConfigProperty(name:=constCPNUseLogAgent, weight:=50, value:=.Logagent, configsetname:=configsetname.Value.ToString, sequence:=ComplexPropertyStore.Sequence.Primary)

                            End With
                        End If
                    End If
                End If
            Next


            ot.CoreMessageHandler(message:="OnTrack configuration properties saved in document properties", messagetype:=otCoreMessageType.ApplicationInfo, _
                                   procedure:="UIFormSetting.saveInDocument")
            isChanged = False
            Me.Refresh()
            Me.Hide()
        Catch ex As Exception
            ot.CoreMessageHandler(exception:=ex, procedure:="UIFormSetting.SaveInDocument", showmsgbox:=True)
        End Try

    End Sub
    ''' <summary>
    ''' handles the inSession save
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub SaveInSessionButton_Click(sender As Object, e As EventArgs) Handles SaveInSessionMenuButton.Click
        Try
            If ot.CurrentSession.IsRunning Then
                ot.CoreMessageHandler(showmsgbox:=True, message:="Current database configuration cannot be changed." & vbLf & _
                                      "since a session is running. Please disconnect from session and try again", _
                                       messagetype:=otCoreMessageType.ApplicationError, procedure:="UIFormSetting.SaveInSession")
                Return
            End If


            '** change the config set name
            Dim configsetname As PropertyStoreItem = _propertyStore.Item(constConfigurationName)
            If configsetname IsNot Nothing AndAlso configsetname.Value IsNot Nothing AndAlso LCase(CStr(configsetname.Value)) <> LCase(ot.CurrentConfigSetName) Then
                ot.CurrentConfigSetName = configsetname.Value
                ot.CoreMessageHandler(message:="Current database configuration changed to " & CStr(configsetname.Value), _
                                      messagetype:=otCoreMessageType.ApplicationInfo, procedure:="UIFormSetting.SaveInSession")
            End If

            Dim description As PropertyStoreItem = _propertyStore.Item(constConfigDescription)
            If description IsNot Nothing Then SetConfigProperty(ot.ConstCPNDescription, weight:=50, value:=description.Value)

            Dim configfilename As PropertyStoreItem = _propertyStore.Item(ConstCPNConfigFileName)
            If configfilename IsNot Nothing Then ot.CurrentConfigFileName = configfilename.Value

            Dim configfilelocation As PropertyStoreItem = _propertyStore.Item(constConfigFileLocation)
            If configfilelocation IsNot Nothing Then ot.AddConfigFilePath(configfilelocation.Value)

            '** set the current configuration set
            Dim aSequence As ComplexPropertyStore.Sequence
            For Each aProperty In _propertyStore
                Dim aValue As String = aProperty.PropertyName
                If aValue.Split(ConstDelimiter).Length >= 2 Then
                    Dim aName As String = aValue.Split(ConstDelimiter).ElementAt(1)
                    If LCase(aName) = "configurationset" Then
                        Dim aConfigSetModel As ConfigSetModel = TryCast(aProperty.Value, ConfigSetModel)
                        If aConfigSetModel IsNot Nothing _
                            AndAlso aConfigSetModel.ConfigSetname = configsetname.Value.ToString Then
                            aSequence = aConfigSetModel.Sequence
                            With aConfigSetModel
                                If .SetupID <> String.empty Then SetConfigProperty(ConstCPNSetupID, weight:=50, value:=.SetupID, configsetname:=configsetname.Value.ToString, sequence:=aSequence)
                                If .SetupDescription <> String.empty Then SetConfigProperty(ConstCPNSetupDescription, weight:=50, value:=.SetupDescription, configsetname:=configsetname.Value.ToString, sequence:=aSequence)
                                If .Database <> String.empty Then SetConfigProperty(ConstCPNDBName, weight:=50, value:=.Database, configsetname:=configsetname.Value.ToString, sequence:=aSequence)
                                If .DBPath <> String.empty Then SetConfigProperty(ConstCPNDBPath, weight:=50, value:=.DBPath, configsetname:=configsetname.Value.ToString, sequence:=aSequence)
                                If .DbUser <> String.Empty Then SetConfigProperty(ConstCPNDBUser, weight:=50, value:=.DbUser, configsetname:=configsetname.Value.ToString, sequence:=aSequence)
                                If .DbUserPassword <> String.empty Then SetConfigProperty(ConstCPNDBPassword, weight:=50, value:=.DbUserPassword, configsetname:=configsetname.Value.ToString, sequence:=aSequence)
                                If .ConfigSetNDescription <> String.empty Then SetConfigProperty(name:=ConstCPNDescription, weight:=50, value:=.ConfigSetNDescription, configsetname:=configsetname.Value.ToString, sequence:=aSequence)
                                If Not String.IsNullOrEmpty(.DriverName) Then SetConfigProperty(name:=ConstCPNDriverName, weight:=50, value:=.DriverName, configsetname:=configsetname.Value.ToString, sequence:=aSequence)
                                If Not String.IsNullOrEmpty(.DriverID) Then SetConfigProperty(name:=ConstCPNDriverID, weight:=50, value:=.DriverID, configsetname:=configsetname.Value.ToString, sequence:=aSequence)
                                If .ConnectionString <> String.Empty Then SetConfigProperty(name:=ConstCPNDBConnection, weight:=50, value:=.ConnectionString, configsetname:=configsetname.Value.ToString, sequence:=aSequence)
                                SetConfigProperty(name:=constCPNUseLogAgent, weight:=50, value:=.Logagent, configsetname:=configsetname.Value.ToString, sequence:=aSequence)
                            End With
                        End If

                    End If
                End If
            Next


            ot.CoreMessageHandler(message:="OnTrack configuration properties saved in session", messagetype:=otCoreMessageType.ApplicationInfo, _
                                   procedure:="UIFormSetting.SaveInSession")
            isChanged = False
            Me.Refresh()
            Me.Close()
        Catch ex As Exception
            ot.CoreMessageHandler(exception:=ex, procedure:="UIFormSetting.SaveInSession", showmsgbox:=True)
        End Try
    End Sub
    ''' <summary>
    ''' handles the InConfigFileSave
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>

    Private Sub SaveInConfigFileButton_Click(sender As Object, e As EventArgs) Handles SaveConfigFileMenuButton.Click
        Try
            Dim configfilename_prop As PropertyStoreItem = _propertyStore.Item(constConfigFileName)
            If configfilename_prop Is Nothing Then configfilename_prop = New PropertyStoreItem(GetType(String), constConfigFileName, ConstDefaultConfigFileName)
            Dim configfilelocation_prop As PropertyStoreItem = _propertyStore.Item(constConfigFileLocation)
            'If configfilelocation IsNot Nothing Then ot.AddConfigFilePath(configfilelocation.Value)
            Dim configfilefullname As String = String.Empty

            ''' check if there is a file
            ''' 
            ' check the configfilepath first
            If Not String.IsNullOrWhiteSpace(configfilelocation_prop.Value) Then
                If Mid(configfilelocation_prop.Value, Len(configfilelocation_prop.Value), 1) <> "\" Then configfilelocation_prop.Value = configfilelocation_prop.Value & "\"

                If Directory.Exists(configfilelocation_prop.Value) Then
                    If File.Exists(configfilelocation_prop.Value & configfilename_prop.Value) Then
                        ''' check name of backup
                        Dim i As UInt16
                        For i = 1 To UInt16.MaxValue
                            configfilefullname = configfilelocation_prop.Value & configfilename_prop.Value & "." & Strings.Format(i, "00000")
                            If Not File.Exists(configfilefullname) Then Exit For
                        Next
                        File.Copy(configfilelocation_prop.Value & configfilename_prop.Value, configfilefullname)
                        If Not File.Exists(configfilefullname) Then
                            ot.CoreMessageHandler(message:="Unable to save copy '" & configfilefullname & "' of config file '" & configfilename_prop.Value & "' !" & vbLf & _
                                         "Sure you have rights for the path ?!", messagetype:=otCoreMessageType.ApplicationError, showmsgbox:=True, procedure:="UIFormSetting.SaveInSession")
                            Return
                        End If

                    End If
                    '' set to the new name
                    configfilefullname = configfilelocation_prop.Value & configfilename_prop.Value
                Else
                    ot.CoreMessageHandler(message:="Path '" & configfilelocation_prop.Value & "' to save config file '" & configfilename_prop.Value & "' doesnot exists !" & vbLf & _
                                          "Please provide a valid file path", messagetype:=otCoreMessageType.ApplicationError, showmsgbox:=True, procedure:="UIFormSetting.SaveInSession")
                    Return
                End If

            End If


            ''' start to write
            ''' 
            Dim aWriter = New StreamWriter(configfilefullname, append:=False)

            aWriter.WriteLine("; OnTrack Database Tooling Config File")
            aWriter.WriteLine("; (C) by sfk engineering services UG 2014")
            aWriter.WriteLine(";")
            aWriter.WriteLine("; assembly name:" & My.Application.Info.AssemblyName & " version:" & My.Application.Info.Version.ToString)
            aWriter.WriteLine("; in directory path:" & My.Application.Info.DirectoryPath)
            aWriter.WriteLine(";")
            aWriter.WriteLine("; saved config file from user '" & Environment.UserName & "' on " & Converter.DateTime2UniversalDateTimeString(DateTime.Now))
            aWriter.WriteLine(";")


            ''' get the global setting
            Dim currentconfigsetname As PropertyStoreItem = _propertyStore.Item(constConfigurationName)

            Dim currentdescription As PropertyStoreItem = _propertyStore.Item(constConfigDescription)
            If currentconfigsetname IsNot Nothing AndAlso currentconfigsetname.Value IsNot Nothing Then
                aWriter.WriteLine(";  global settings ")
                aWriter.WriteLine(ConstCPNUseConfigSetName & "=" & currentconfigsetname.Value)
                aWriter.WriteLine(constCPNDefaultDomainid & "=" & ConstGlobalDomain)
            End If

            '** save all sets
            Dim aSequence As ComplexPropertyStore.Sequence
            For Each aProperty In _propertyStore
                '' extract
                Dim aValue As String = aProperty.PropertyName
                If aValue.Split(ConstDelimiter).Length >= 2 Then
                    Dim aName As String = aValue.Split(ConstDelimiter).ElementAt(1)
                    If LCase(aName) = "configurationset" Then
                        Dim aConfigSetModel As ConfigSetModel = TryCast(aProperty.Value, ConfigSetModel)
                        If aConfigSetModel IsNot Nothing Then
                            aSequence = aConfigSetModel.Sequence
                            aWriter.WriteLine("; CONFIGSET " & aConfigSetModel.ConfigSetname)
                            With aConfigSetModel
                                If .ConfigSetname = ConstGlobalConfigSetName Then
                                    aWriter.WriteLine(";  global set ")
                                    aWriter.WriteLine(ConstCPNUseConfigSetName & "=" & currentconfigsetname.Value)
                                    aWriter.WriteLine(constCPNDefaultDomainid & "=" & ConstGlobalDomain)
                                Else
                                    aWriter.WriteLine("[" & .ConfigSetname & ":" & .Sequence.ToString & "]")
                                End If

                                ''' save properties
                                If Not String.IsNullOrWhiteSpace(.Database) Then aWriter.WriteLine(ConstCPNDBName & "=" & .Database)
                                If Not String.IsNullOrWhiteSpace(.DbUser) Then aWriter.WriteLine(ConstCPNDBUser & "=" & .DbUser)
                                If Not String.IsNullOrWhiteSpace(.SetupDescription) Then aWriter.WriteLine(ConstCPNSetupDescription & "=" & .SetupDescription)
                                If Not String.IsNullOrWhiteSpace(.SetupID) Then aWriter.WriteLine(ConstCPNSetupID & "=" & .SetupID)
                                If Not String.IsNullOrWhiteSpace(.DbUserPassword) Then aWriter.WriteLine(ConstCPNDBPassword & "=" & .DbUserPassword)
                                If Not String.IsNullOrWhiteSpace(.ConfigSetNDescription) Then aWriter.WriteLine(ConstCPNDescription & "=" & .ConfigSetNDescription)
                                If Not String.IsNullOrEmpty(.DriverName) Then aWriter.WriteLine(ConstCPNDriverName & "=" & .DriverName.ToString)
                                If Not String.IsNullOrEmpty(.DriverID) Then aWriter.WriteLine(ConstCPNDriverID & "=" & .DriverID.ToString)
                                If Not String.IsNullOrWhiteSpace(.ConnectionString) Then aWriter.WriteLine(ConstCPNDBConnection & "=" & .ConnectionString)
                                If Not String.IsNullOrWhiteSpace(.DBPath) Then aWriter.WriteLine(ConstCPNDBPath & "=" & .DBPath)
                                aWriter.WriteLine(constCPNUseLogAgent & "=" & .Logagent.ToString)
                                aWriter.WriteLine(ConstCPNDBSQLServerUseMars & "=" & .Usemars.ToString)

                            End With

                        End If

                    End If
                End If
            Next

            aWriter.Flush()
            aWriter.Close()

            ot.CoreMessageHandler(message:="OnTrack configuration properties saved in config file " & configfilefullname & vbLf _
                                  & "Restart Office or Add-In to use them", showmsgbox:=True, messagetype:=otCoreMessageType.ApplicationInfo, _
                                   procedure:="UIFormSetting.SaveConfigFile")
            isChanged = False
            Me.Refresh()
            Me.Close()
        Catch ex As Exception
            ot.CoreMessageHandler(exception:=ex, procedure:="UIFormSetting.SaveConfigFile", showmsgbox:=True)
        End Try
    End Sub
    ''' <summary>
    ''' handles the Form OnLoad Event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub UIFormSetting_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Add any initialization after the InitializeComponent() call.
        _propertyStore = Me.UpdatePropertyStore()
    End Sub

    ''' <summary>
    ''' Click the button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DropDatabaseButton_Click(sender As Object, e As EventArgs) Handles DropDatabaseButton.Click
            If Installation.DropDatabase() Then
                Me.StatusLabel.Text = "database dropped"
            End If
    End Sub

    ''' <summary>
    ''' Initialize the Database Data
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub InitializeDataButton_Click(sender As Object, e As EventArgs) Handles InitializeDataButton.Click
        If ot.CurrentSession.RequireAccessRight (accessrequest:=otAccessRight.AlterSchema ) Then
            Dim aBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
            aBrowserDialog.Description = "Select the directory which contains initial data (folder will be searched recursively for .csv files)"
            ' Do not allow the user to create New files via the FolderBrowserDialog.
            aBrowserDialog.ShowNewFolderButton = False

            ' Default lookup path
            Dim uri As System.Uri = New System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)
            Dim valueInitialPath As String = ConstInitialDataFolder & "\" & CurrentSession.CurrentSetupID
            Dim searchpath As String = String.Empty
            If System.IO.Directory.Exists(valueInitialPath) Then
                searchpath = valueInitialPath
            ElseIf System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(uri.LocalPath) & "\Resources\" & valueInitialPath) Then
                searchpath = System.IO.Path.GetDirectoryName(uri.LocalPath) & "\Resources\" & valueInitialPath
            ElseIf System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(uri.LocalPath) & "\Resources\" & ConstInitialDataFolder) Then
                searchpath = System.IO.Path.GetDirectoryName(uri.LocalPath) & "\Resources\" & ConstInitialDataFolder
            Else

                searchpath = System.IO.Path.GetDirectoryName(uri.LocalPath) & "\Resources"
            End If



            ''' try to feed from My.Application path, then from Executing Assembly Path 
            ''' 

            ''' 
            aBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop
            If Not String.IsNullOrWhiteSpace(searchpath) Then
                aBrowserDialog.SelectedPath = searchpath
            End If

            Dim result = aBrowserDialog.ShowDialog()

            If result = Windows.Forms.DialogResult.OK OrElse result = Windows.Forms.DialogResult.Yes Then
                Dim path As String = aBrowserDialog.SelectedPath
                If Installation.InitializeData(setupid:=CurrentSetupID, searchpath:=path) Then
                    ' feed initial data
                    Me.StatusLabel.Text = "database data initialized"
                Else
                    Me.StatusLabel.Text = "database data initialization failed - see message log"
                End If
            Else
                Me.StatusLabel.Text = "initialize database data aborted"
            End If

        End If
    End Sub
End Class
