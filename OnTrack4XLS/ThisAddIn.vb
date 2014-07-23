
Imports OnTrack.UI
Imports OnTrack.AddIn
Imports System.Reflection
Imports OnTrack.Database

''' <summary>
''' Definition of the Add-In Object
''' </summary>
''' <remarks></remarks>
Public Class ThisAddIn

    <ormChangeLogEntry(Application:=ConstApplicationExcelAddin, module:="", version:=1, release:=1, patch:=2, changeimplno:=8, _
      description:="On DBExplorer Operation show an waiting cursor and standby message.")> _
   <ormChangeLogEntry(Application:=ConstApplicationExcelAddin, module:="", version:=1, release:=1, patch:=2, changeimplno:=7, _
      description:="Replication will ask before full inbound replication.")> _
   <ormChangeLogEntry(Application:=ConstApplicationExcelAddin, module:="", version:=1, release:=1, patch:=2, changeimplno:=6, _
      description:="MQF Wirzar reworked in stepping for- and backward. Reseting mqf object structures and rebuild.")> _
   <ormChangeLogEntry(Application:=ConstApplicationExcelAddin, module:="", version:=1, release:=1, patch:=2, changeimplno:=5, _
      description:="Bring non-modal forms to front if button is clicked again.")> _
  <ormChangeLogEntry(Application:=ConstApplicationExcelAddin, module:="", version:=1, release:=1, patch:=2, changeimplno:=4, _
      description:="After replication reset the data area to the real range. Delete rows if data area is shrinking.")> _
  <ormChangeLogEntry(Application:=ConstApplicationExcelAddin, module:="", version:=1, release:=1, patch:=2, changeimplno:=3, _
      description:="If in cell editing mode, leave cell if replication or mqf wizard is started.")> _
  <ormChangeLogEntry(Application:=ConstApplicationExcelAddin, module:="", version:=1, release:=1, patch:=2, changeimplno:=2, _
      description:="Close all open forms after shutting down the database")> _
  <ormChangeLogEntry(Application:=ConstApplicationExcelAddin, module:="", version:=1, release:=1, patch:=2, changeimplno:=1, _
      description:="Added the View on Changes in About Box")> _
    Public Const OTAddinCommonsVersion = "V1.R1.P2"

    Public Const ConstApplicationExcelAddin = "Addin4Excel"


    Friend WithEvents _OTDBSession As Session
    Private _CurrentHost As Excel.Workbook
    Private _CurrentDefaultDomainID As String


    Private _ApplicationCompany As String
    Private _ApplicationCopyRight As String
    Private _ApplicationVersion As String
    Private _ApplicationName As String
    Private _ApplicationDescription As String = "OnTrack4XLS is the add-in of the OnTrack Database Suite for Excel. OnTrack provides support for deliverable based scheduling, progress tracking, configuration management in complex project."

    ''' <summary>
    ''' Gets or sets the application copy right.
    ''' </summary>
    ''' <value>The application copy right.</value>
    Public Property ApplicationCopyRight() As String
        Get
            If String.IsNullOrWhiteSpace(_ApplicationCopyRight) Then
                ' Get all Copyright attributes on this assembly
                Dim attributes As Object() = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyCopyrightAttribute), False)
                ' If there aren't any Copyright attributes, return an empty string
                If attributes.Length = 0 Then
                    Return ""
                End If
                ' If there is a Copyright attribute, return its value
                Return (CType(attributes(0), AssemblyCopyrightAttribute)).Copyright
            End If
            Return _ApplicationCopyRight
        End Get
        Set(value As String)
            Me._ApplicationCopyRight = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the application company.
    ''' </summary>
    ''' <value>The application company.</value>
    Public Property ApplicationCompany() As String
        Get
            If String.IsNullOrWhiteSpace(_ApplicationCompany) Then
                ' Get all Company attributes on this assembly
                Dim attributes As Object() = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyCompanyAttribute), False)
                ' If there aren't any Company attributes, return an empty string
                If attributes.Length = 0 Then
                    Return ""
                End If
                ' If there is a Company attribute, return its value
                Return (CType(attributes(0), AssemblyCompanyAttribute)).Company
            End If
            Return Me._ApplicationCompany
        End Get
        Set(value As String)
            Me._ApplicationCompany = Value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the application description.
    ''' </summary>
    ''' <value>The application description.</value>
    Public Property ApplicationDescription() As String
        Get
            If String.IsNullOrWhiteSpace(_ApplicationDescription) Then
                ' Get all Description attributes on this assembly
                Dim attributes As Object() = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyDescriptionAttribute), False)
                ' If there aren't any Description attributes, return an empty string
                If attributes.Length = 0 Then
                    Return ""
                End If
                ' If there is a Description attribute, return its value
                Return (CType(attributes(0), AssemblyDescriptionAttribute)).Description
            End If
            Return Me._ApplicationDescription
        End Get
        Set(value As String)
            Me._ApplicationDescription = Value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the version.
    ''' </summary>
    ''' <value>The version.</value>
    Public Property ApplicationVersion() As String
        Get
            If String.IsNullOrWhiteSpace(_ApplicationVersion) Then Return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
            Return _ApplicationVersion
        End Get
        Set(value As String)
            _ApplicationVersion = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the name of the application.
    ''' </summary>
    ''' <value>The name of the application.</value>
    Public Property ApplicationName() As String
        Get
            If String.IsNullOrWhiteSpace(_ApplicationName) Then
                ' Get all Title attributes on this assembly
                Dim attributes As Object() = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyTitleAttribute), False)
                ' If there is at least one Title attribute
                If attributes.Length > 0 Then
                    ' Select the first one
                    Dim titleAttribute As AssemblyTitleAttribute = CType(attributes(0), AssemblyTitleAttribute)
                    ' If it is not an empty string, return it
                    If titleAttribute.Title <> "" Then
                        Return titleAttribute.Title
                    End If
                End If
            End If

            Return _ApplicationName
        End Get
        Set(value As String)
            _ApplicationName = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the current default domain ID.
    ''' </summary>
    ''' <value>The current default domain ID.</value>
    Public Property CurrentDefaultDomainID() As String
        Get
            Return Me._CurrentDefaultDomainID
        End Get
        Set(value As String)
            Me._CurrentDefaultDomainID = value
        End Set
    End Property

    ''' <summary>
    ''' set the Current Host
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CurrentHost As Object
        Set(host As Object)
            Dim value As Object
            Try

                If Not host Is Nothing Then
                    If _CurrentHost Is Nothing OrElse _CurrentHost.Name <> host.name Then
                        _CurrentHost = host
                        ' add the Filepaths for tooling
                        Try
                            If Not Application.ActiveWorkbook Is Nothing Then
                                value = Application.ActiveWorkbook.Path
                            Else
                                value = ""
                            End If
                        Catch ex As Exception
                            value = ""
                        End Try
                        If value <> "" Then
                            AddConfigFilePath(value)
                        End If
                        ' than a property
                        value = GetHostProperty(ConstCPNConfigFileLocation)
                        If value <> "" Then
                            AddConfigFilePath(value)
                        End If
                        ' first look if we have a parameter 
                        value = modParameterXLS.GetXlsParameterByName(name:=ConstCPNConfigFileLocation, silent:=True)
                        If value <> "" Then
                            AddConfigFilePath(value)
                        End If

                        '''
                        ''' message than we switched the host
                        ''' 
                        Call CoreMessageHandler(message:=" Host Application switched to " & host.name, arg1:=host.path, _
                                                     messagetype:=otCoreMessageType.InternalInfo, subname:="ThisAddin.CurrentHost")

                        ' set the defaultdomainid
                        value = GetXlsParameterByName(name:=constCPNDefaultDomainid, silent:=True)
                        If Not value Is Nothing Then
                            Me.CurrentDefaultDomainID = CStr(value)
                        End If

                        If Not _OTDBSession Is Nothing AndAlso Not _OTDBSession.IsRunning Then
                            '* add the config file path
                            AddConfigFilePath(host.path)
                            '** read the config properties
                            Me.SetConfigProperties()
                        End If
                    End If
                End If
            Catch ex As Exception
                Call CoreMessageHandler(message:=" could not switch host application ", _
                                                     messagetype:=otCoreMessageType.InternalInfo, subname:="ThisAddin.CurrentHost")
            End Try
        End Set
        Get
            Return _CurrentHost
        End Get
    End Property
    ''' <summary>
    ''' set the current host to the current active Workbook
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetCurrentHost()
        Me.CurrentHost = Me.Application.ActiveWorkbook
    End Sub
    ''' <summary>
    ''' Start up this ADDIN for Excel
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ThisAddIn_Startup() Handles Me.Startup

        Dim value As Object

        '** initializes OTDB and sets the current session
        ApplicationName = My.Application.Info.AssemblyName & ConstDelimiter & My.Application.Info.Version.ToString
        _OTDBSession = CurrentSession
        '** register the UI
        UserInterface.RegisterNativeUI(UserInterface.LoginFormName, GetType(UIFormLogin))
        UserInterface.RegisterNativeUI(UserInterface.MessageboxFormName, GetType(OnTrack.UI.UITelerikMessageBox))
        ' check the Tooling PATH
        For Each tooling As Object In Application.AddIns
            If tooling.Name Like ConstDefaultToolingNamePattern And tooling.Installed Then
                AddConfigFilePath(tooling.path)
                Exit For
            End If
        Next

        Call CoreMessageHandler(showmsgbox:=False, message:="On Track Excel AddIn Startup", _
                             noOtdbAvailable:=True, subname:="OTDB.Initialize", messagetype:=otCoreMessageType.InternalInfo)

        '** try to set the current host
        Me.SetCurrentHost()

        '''*** About Data
        AddIn.AboutData.ApplicationName = "OnTrack4XLS - OnTrack Add-in for Excel"
        AddIn.AboutData.Description = Me.ApplicationDescription
        AddIn.AboutData.Version = Me.ApplicationVersion
        AddIn.AboutData.CopyRight = "(C) by sfk engineering services UG"
        AddIn.AboutData.Company = Me.ApplicationCompany
        AddIn.AboutData.ProductName = "OnTrack Database Suite"

        '' changelog
        ot.OnTrackChangeLog.Refresh(GetType(ThisAddIn))
        ot.OnTrackChangeLog.Refresh(GetType(otAddinCommon))
    End Sub

    ''' <summary>
    ''' retrieve the Config parameters of OnTrack and write it to the PropertyBag
    ''' </summary>
    ''' <returns>true if successfull</returns>
    ''' <remarks></remarks>
    Friend Function SetConfigProperties(Optional force As Boolean = False) As Boolean


        Dim value As String
        Dim found As Boolean

        '** do we have a config set name ?! retrieve and use .. but set it in the end
        Dim useConfigSetName As String = GetXlsParameterByName(name:=ConstCPNUseConfigSetName, silent:=True, found:=found)
        If useConfigSetName Is Nothing OrElse useConfigSetName = "" Then
            useConfigSetName = GetHostProperty(ConstCPNUseConfigSetName, silent:=True, found:=found)
        End If

        '* read the configs from file ConstCPNConfigFileName
        value = GetXlsParameterByName(name:=ConstCPNConfigFileName, silent:=True, found:=found)
        If Not value Is Nothing Then
            SetConfigProperty(ConstCPNConfigFileName, weight:=30, value:=value)
        End If
        value = GetHostProperty(ConstCPNConfigFileName, silent:=True, found:=found)
        If Not value Is Nothing Then
            SetConfigProperty(ConstCPNConfigFileName, weight:=20, value:=value)
        End If

        '*** check the local parameters
        '***

        value = GetXlsParameterByName(name:=ConstCPNDBType, silent:=True)
        If Not value Is Nothing Then
            Select Case LCase(value)
                Case ConstCPVDBTypeAccess
                    ' set it
                    SetConfigProperty(name:=ConstCPNDBType, weight:=30, value:=Database.otDBServerType.Access, configsetname:=useConfigSetName)
                    SetConfigProperty(name:=ConstCPNDBUseseek, weight:=30, value:=True, configsetname:=useConfigSetName)
                Case ConstCPVDBTypeSqlServer
                    ' set it
                    SetConfigProperty(name:=ConstCPNDBType, weight:=30, value:=Database.otDBServerType.SQLServer, configsetname:=useConfigSetName)
                    SetConfigProperty(name:=ConstCPNDBUseseek, weight:=30, value:=False, configsetname:=useConfigSetName)
                Case Else
                    Call CoreMessageHandler(showmsgbox:=True, arg1:=value, subname:="ThisAddin.SetConfigProperties", _
                                          message:=" OnTrack database has not a valid databasetype (should be access or sqlserver)", _
                                          break:=False, noOtdbAvailable:=True)
            End Select
        End If
        value = GetHostProperty(ConstCPNDBType, silent:=True)
        If Not value Is Nothing Then
            Select Case LCase(value)
                Case ConstCPVDBTypeAccess
                    ' set it
                    SetConfigProperty(name:=ConstCPNDBType, weight:=20, value:=Database.otDBServerType.Access, configsetname:=useConfigSetName)
                    SetConfigProperty(name:=ConstCPNDBUseseek, weight:=20, value:=True, configsetname:=useConfigSetName)
                Case ConstCPVDBTypeSqlServer
                    ' set it
                    SetConfigProperty(name:=ConstCPNDBType, weight:=20, value:=Database.otDBServerType.SQLServer, configsetname:=useConfigSetName)
                    SetConfigProperty(name:=ConstCPNDBUseseek, weight:=20, value:=False, configsetname:=useConfigSetName)
                Case Else
                    Call CoreMessageHandler(showmsgbox:=True, arg1:=value, subname:="ThisAddin.SetConfigProperties", _
                                          message:=" OnTrack database has not a valid databasetype (should be access or sqlserver)", _
                                          break:=False, noOtdbAvailable:=True)
            End Select
        End If


        ' get the path
        value = GetXlsParameterByName(ConstCPNDBPath, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=ConstCPNDBPath, weight:=30, value:=value, configsetname:=useConfigSetName)
        End If
        value = GetHostProperty(ConstCPNDBPath, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=ConstCPNDBPath, weight:=20, value:=value, configsetname:=useConfigSetName)
        End If


        ' get the Database Name if we have it
        value = GetXlsParameterByName(ConstCPNDBName, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=ConstCPNDBName, weight:=30, value:=value, configsetname:=useConfigSetName)
        End If
        value = GetHostProperty(ConstCPNDBName, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=ConstCPNDBName, weight:=20, value:=value, configsetname:=useConfigSetName)
        End If


        ' get the Database user if we have it
        value = GetXlsParameterByName(ConstCPNDBUser, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=ConstCPNDBUser, weight:=30, value:=value, configsetname:=useConfigSetName)
        End If
        value = GetHostProperty(ConstCPNDBUser, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=ConstCPNDBUser, weight:=20, value:=value, configsetname:=useConfigSetName)
        End If


        ' get the Database password if we have it
        value = GetXlsParameterByName(name:=ConstCPNDBPassword, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=ConstCPNDBPassword, weight:=30, value:=value, configsetname:=useConfigSetName)
        End If
        value = GetHostProperty(ConstCPNDBPassword, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=ConstCPNDBPassword, weight:=20, value:=value, configsetname:=useConfigSetName)
        End If


        ' get the connection string
        value = GetXlsParameterByName(name:=ConstCPNDBConnection, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=ConstCPNDBConnection, weight:=30, value:=value, configsetname:=useConfigSetName)
        End If
        value = GetHostProperty(ConstCPNDBConnection, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=ConstCPNDBConnection, weight:=20, value:=value, configsetname:=useConfigSetName)
        End If

        ' get the driver name
        value = GetXlsParameterByName(name:=ConstCPNDescription, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=ConstCPNDescription, weight:=30, value:=value, configsetname:=useConfigSetName)
        End If
        value = GetHostProperty(ConstCPNDescription, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=ConstCPNDescription, weight:=20, value:=value, configsetname:=useConfigSetName)
        End If

        ' get the driver name
        value = GetXlsParameterByName(name:=ConstCPNDriverName, silent:=True)
        If Not value Is Nothing Then
            Select Case LCase(value)
                Case ConstCPVDriverMSSQL
                    ' set it
                    SetConfigProperty(name:=ConstCPNDriverName, weight:=20, value:=Database.otDbDriverType.ADONETSQL, configsetname:=useConfigSetName)
                    'SetConfigProperty(name:=ConstCPNDBUseseek, weight:=20, value:=True, configsetname:=useConfigSetName)
                Case ConstCPVDriverOleDB
                    ' set it
                    SetConfigProperty(name:=ConstCPNDriverName, weight:=20, value:=Database.otDbDriverType.ADONETOLEDB, configsetname:=useConfigSetName)
                    'SetConfigProperty(name:=ConstCPNDBUseseek, weight:=20, value:=False, configsetname:=useConfigSetName)
                Case Else
                    Call CoreMessageHandler(showmsgbox:=True, arg1:=value, subname:="ThisAddin.SetConfigProperties", _
                                          message:=" OnTrack database has not a valid drivertype", _
                                          break:=False, noOtdbAvailable:=True)
            End Select

        End If
        value = GetHostProperty(ConstCPNDriverName, silent:=True)
        If Not value Is Nothing Then
            Select Case LCase(value)
                Case ConstCPVDriverMSSQL
                    ' set it
                    SetConfigProperty(name:=ConstCPNDriverName, weight:=20, value:=Database.otDbDriverType.ADONETSQL, configsetname:=useConfigSetName)
                    'SetConfigProperty(name:=ConstCPNDBUseseek, weight:=20, value:=True, configsetname:=useConfigSetName)
                Case ConstCPVDriverOleDB
                    ' set it
                    SetConfigProperty(name:=ConstCPNDriverName, weight:=20, value:=Database.otDbDriverType.ADONETOLEDB, configsetname:=useConfigSetName)
                    'SetConfigProperty(name:=ConstCPNDBUseseek, weight:=20, value:=False, configsetname:=useConfigSetName)
                Case Else
                    Call CoreMessageHandler(showmsgbox:=True, arg1:=value, subname:="ThisAddin.SetConfigProperties", _
                                          message:=" OnTrack database has not a valid drivertype", _
                                          break:=False, noOtdbAvailable:=True)
            End Select
        End If
        ' get the UseLogAgent
        value = GetXlsParameterByName(name:=constCPNUseLogAgent, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=constCPNUseLogAgent, weight:=30, value:=CBool(value), configsetname:=useConfigSetName)
        End If
        value = GetHostProperty(constCPNUseLogAgent, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=constCPNUseLogAgent, weight:=20, value:=CBool(value), configsetname:=useConfigSetName)
        End If
        ' set the defaultdomainid
        value = GetXlsParameterByName(name:=constCPNDefaultDomainid, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=constCPNDefaultDomainid, weight:=30, value:=CStr(value), configsetname:=useConfigSetName)
        End If
        value = GetHostProperty(constCPNDefaultDomainid, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=constCPNDefaultDomainid, weight:=20, value:=CStr(value), configsetname:=useConfigSetName)
        End If
        '****
        '**** Finally set the Configset Name after we have created everthing - mus texist
        If useConfigSetName IsNot Nothing Then
            ot.CurrentConfigSetName = useConfigSetName
        End If

        Return True

    End Function
    Private Sub ThisAddIn_Shutdown() Handles Me.Shutdown
        '*** disconnect
        If Not _OTDBSession Is Nothing AndAlso _OTDBSession.ShutDown() Then
            Globals.ThisAddIn.Application.StatusBar = Date.Now & ": Disconnected from OnTrack Database"
        End If
    End Sub

End Class
