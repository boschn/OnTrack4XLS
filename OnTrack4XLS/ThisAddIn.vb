
Imports OnTrack.UI
Imports OnTrack.addin


Public Class ThisAddIn

    Friend WithEvents _OTDBSession As Session
    Private _CurrentHost As Excel.Workbook

    Public Property CurrentHost
        Set(host)
            Dim value As Object

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

                    Call CoreMessageHandler(message:=" Host Application switched to " & host.name, arg1:=host.path, _
                                                 messagetype:=otCoreMessageType.InternalInfo, subname:="ThisAddin.CurrentHost")
                    If Not _OTDBSession Is Nothing AndAlso Not _OTDBSession.IsRunning Then
                        '* add the config file path
                        AddConfigFilePath(host.path)
                        '** read the config properties
                        Me.SetConfigProperties()
                    End If
                End If
            End If
        End Set
        Get
            Return _CurrentHost
        End Get
    End Property
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
        OTDBUI.RegisterNativeUI(OTDBUI.LoginFormName, GetType(UIFormLogin))
        OTDBUI.RegisterNativeUI(OTDBUI.MessageboxFormName, GetType(OnTrack.UI.UITelerikMessageBox))
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

    End Sub

    ''' <summary>
    ''' retrieve the Config parameters of OnTrack and write it to the PropertyBag
    ''' </summary>
    ''' <returns>true if successfull</returns>
    ''' <remarks></remarks>
    Friend Function SetConfigProperties(Optional force As Boolean = False) As Boolean


        Dim value As String

        Dim found As Boolean
        Dim useConfigSetName As String = GetXlsParameterByName(name:=ConstCPNUseConfigSetName, silent:=True, found:=found)
        If useConfigSetName Is Nothing OrElse useConfigSetName = "" Then
            useConfigSetName = GetHostProperty(ConstCPNUseConfigSetName, silent:=True, found:=found)
        End If

        If useConfigSetName IsNot Nothing Then
            ot.CurrentConfigSetName = useConfigSetName
        Else
            useConfigSetName = ot.CurrentConfigSetName
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
        ' get the driver name
        value = GetXlsParameterByName(name:=constCPNUseLogAgent, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=constCPNUseLogAgent, weight:=30, value:=CBool(value), configsetname:=useConfigSetName)
        End If
        value = GetHostProperty(constCPNUseLogAgent, silent:=True)
        If Not value Is Nothing Then
            SetConfigProperty(name:=constCPNUseLogAgent, weight:=20, value:=CBool(value), configsetname:=useConfigSetName)
        End If



        Return True

    End Function
    Private Sub ThisAddIn_Shutdown() Handles Me.Shutdown
        '*** disconnect
        If Not _OTDBSession Is Nothing AndAlso _OTDBSession.ShutDown() Then
            Globals.ThisAddIn.Application.StatusBar = Date.Now.ToLocalTime & ": Disconnected from OnTrack Database"
        End If
    End Sub

End Class
