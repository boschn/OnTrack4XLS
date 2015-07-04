REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** CORE ORM Driver Classes for On Track Database Backend Library
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2013-08-08
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************''' <summary>

Imports System.Collections
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Data
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Diagnostics.Debug
Imports System.Attribute
Imports System.IO
Imports System.Text.RegularExpressions

Imports OnTrack.UI
Imports System.Reflection
Imports OnTrack.Commons
Imports OnTrack.Core

Namespace OnTrack.Database

    ''' <summary>
    ''' abstract connection implementation
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class ormConnection
        Implements iormConnection

        Private _ID As String
        Protected _Session As Session
        Protected _Connectionstring As String = String.Empty  'the  Connection String
        Protected _Path As String = String.Empty  'where the database is if access
        Protected _Name As String = String.Empty  'name of the database or file
        Protected _Dbuser As String = String.Empty  'User name to use to access the database
        Protected _Dbpassword As String = String.Empty   'password to use to access the database
        Protected _Sequence As ComplexPropertyStore.Sequence = ComplexPropertyStore.Sequence.Primary ' configuration sequence of the connection
        'Protected _OTDBUser As New User    ' OTDB User -> moved to session 
        Protected _AccessLevel As otAccessRight    ' access

        Protected _UILogin As CoreLoginForm
        Protected _cacheUserValidateon As UserValidation
        Protected _OTDBDatabaseDriver As iormRelationalDatabaseDriver
        Protected _useseek As Boolean 'use seek instead of SQL
        Protected _lockObject As New Object ' use lock object for sync locking

        Protected WithEvents _ErrorLog As SessionMessageLog
        Protected WithEvents _configurations As ComplexPropertyStore

        Public Event OnConnection As EventHandler(Of ormConnectionEventArgs) Implements iormConnection.OnConnection
        Public Event OnDisconnection As EventHandler(Of ormConnectionEventArgs) Implements iormConnection.OnDisconnection

        ''' <summary>
        ''' constructor of Connection
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="databasedriver"></param>
        ''' <param name="session"></param>
        ''' <remarks></remarks>
        Public Sub New(id As String, databasedriver As iormRelationalDatabaseDriver, sequence As ComplexPropertyStore.Sequence, Optional session As Session = Nothing)
            _OTDBDatabaseDriver = databasedriver
            _OTDBDatabaseDriver.RegisterConnection(Me)
            If session IsNot Nothing Then Me.Session = session
            _ID = id
            _Sequence = sequence
            _AccessLevel = Nothing
            _UILogin = Nothing
        End Sub
        ''' <summary>
        ''' Gets the ID.
        ''' </summary>
        ''' <value>The ID.</value>
        Public ReadOnly Property ID() As String Implements iormConnection.ID
            Get
                Return _ID
            End Get
        End Property
        ''' <summary>
        ''' Gets the use seek.
        ''' </summary>
        ''' <value>The use seek.</value>
        Public ReadOnly Property Useseek() As Boolean Implements iormConnection.Useseek
            Get
                Return _useseek
            End Get
        End Property
        ''' <summary>
        ''' returns the Sequence of the Database Configuration
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Sequence As ComplexPropertyStore.Sequence
            Get
                Return _Sequence
            End Get
        End Property
        ''' <summary>
        ''' Gets the session.
        ''' </summary>
        ''' <value>The session.</value>
        Public Property Session() As Session Implements iormConnection.Session
            Get
                Return Me._Session
            End Get
            Set(value As Session)
                _Session = value
                If value IsNot Nothing Then
                    _configurations = value.Configurations
                    _ErrorLog = value.Errorlog
                Else
                    _configurations = Nothing
                    _ErrorLog = Nothing
                End If

            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the DatabaseEnvirorment.
        ''' </summary>
        ''' <value>iOTDBDatabaseEnvirorment</value>
        Public Property DatabaseDriver() As iormRelationalDatabaseDriver Implements iormConnection.DatabaseDriver
            Get
                Return _OTDBDatabaseDriver
            End Get
            Friend Set(value As iormRelationalDatabaseDriver)
                _OTDBDatabaseDriver = value
            End Set
        End Property
        ''' <summary>
        ''' returns the native Database name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride ReadOnly Property NativeDatabaseName As String Implements iormConnection.NativeDatabaseName
        ''' <summary>
        ''' returns the native Database name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride ReadOnly Property NativeDatabaseVersion As String Implements iormConnection.NativeDatabaseVersion
        ''' <summary>
        ''' <summary>
        ''' Gets the error log.
        ''' </summary>
        ''' <value>The error log.</value>
        Public ReadOnly Property ErrorLog() As SessionMessageLog Implements iormConnection.ErrorLog
            Get
                If _ErrorLog Is Nothing Then
                    _ErrorLog = New SessionMessageLog(My.Computer.Name & "-" & My.User.Name & "-" & Date.Now.ToUniversalTime)
                End If
                Return _ErrorLog
            End Get
        End Property

        '*******
        '*******
        MustOverride ReadOnly Property IsConnected As Boolean Implements iormConnection.IsConnected

        '*******
        '*******
        MustOverride ReadOnly Property IsInitialized As Boolean Implements iormConnection.IsInitialized

        '*******
        '*******
        Friend MustOverride ReadOnly Property NativeConnection As Object Implements iormConnection.NativeConnection

        ''' <summary>
        ''' Gets or sets the UI login.
        ''' </summary>
        ''' <value>The UI login.</value>
        Public Property UILogin() As CoreLoginForm Implements iormConnection.UILogin
            Get
                If _UILogin Is Nothing Then
                    _UILogin = New CoreLoginForm()
                End If
                Return Me._UILogin
            End Get
            Set(value As CoreLoginForm)
                Me._UILogin = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the access.
        ''' </summary>
        ''' <value>The access.</value>
        Public Property Access() As otAccessRight Implements iormConnection.Access
            Get
                Return Me._AccessLevel
            End Get
            Set(value As otAccessRight)
                Me._AccessLevel = value
            End Set
        End Property


        ''' <summary>
        ''' Gets or sets the dbpassword.
        ''' </summary>
        ''' <value>The dbpassword.</value>
        Public Property Dbpassword() As String Implements iormConnection.Dbpassword
            Get
                Return Me._Dbpassword
            End Get
            Set(value As String)
                Me._Dbpassword = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the dbuser.
        ''' </summary>
        ''' <value>The dbuser.</value>
        Public Property Dbuser() As String Implements iormConnection.Dbuser
            Get
                Return Me._Dbuser
            End Get
            Set(value As String)
                Me._Dbuser = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name.
        ''' </summary>
        ''' <value>The name.</value>
        Public Property DBName() As String Implements iormConnection.DBName
            Get
                Return Me._Name
            End Get
            Set(value As String)
                Me._Name = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the path.
        ''' </summary>
        ''' <value>The path.</value>
        Public Property PathOrAddress() As String Implements iormConnection.PathOrAddress
            Get
                Return Me._Path
            End Get
            Set(value As String)
                Me._Path = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the connectionstring.
        ''' </summary>
        ''' <value>The connectionstring.</value>
        Public Property Connectionstring() As String Implements iormConnection.Connectionstring
            Get
                Return Me._Connectionstring
            End Get
            Set(value As String)
                Me._Connectionstring = value
            End Set
        End Property

        Public Function RaiseOnConnected()
            RaiseEvent OnConnection(Me, New ormConnectionEventArgs(Me))
        End Function
        Public Function RaiseOnDisConnected()
            RaiseEvent OnDisconnection(Me, New ormConnectionEventArgs(Me))
        End Function

        '*****
        '***** reset : reset all the private members for a connection
        Protected Friend Overridable Sub ResetFromConnection()
            '_Connectionstring = String.empty

            '_Path = String.empty
            '_Name = String.empty
            _Dbuser = Nothing
            _Dbpassword = Nothing
            '_OTDBUser = Nothing
            _AccessLevel = Nothing

            '_UILogin = Nothing
        End Sub
        '*****
        '***** disconnect : Disconnects from the Database and cleans up the Enviorment
        Public Overridable Function Disconnect() As Boolean Implements iormConnection.Disconnect
            If Not Me.IsConnected Then
                Return False
            End If
            ' Raise the event -> not working here ?!
            RaiseEvent OnDisconnection(Me, New ormConnectionEventArgs(Me))
            Return True
        End Function

        ''' <summary>
        ''' Event Handler for the Configuration Property Changed Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnConfigPropertyChanged(sender As Object, e As ComplexPropertyStore.EventArgs) Handles _configurations.OnPropertyChanged
            '** do only something if we have run through
            If Me.IsConnected Then
                '** do nothing if we are running
                CoreMessageHandler(message:="current config set name was changed after connection is connected -ignored", procedure:="ormConnection.OnCurrentConfigSetChanged", argument:=e.Setname, messagetype:=otCoreMessageType.InternalError)
            Else
                SetConnectionConfigParameters()

            End If
        End Sub

        ''' <summary>
        ''' retrieve the Config parameters of OnTrack and sets it in the Connection
        ''' </summary>
        ''' <param name="propertyBag">a Dictionary of string, Object</param>
        ''' <returns>true if successfull</returns>
        ''' <remarks></remarks>
        Protected Friend Overridable Function SetConnectionConfigParameters() As Boolean Implements iormConnection.SetConnectionConfigParameters
            Dim connectionstring As String
            Dim Value As Object

            '* useseek
            Value = _configurations.GetProperty(name:=ConstCPNDBUseseek, setname:=_Session.ConfigSetname, sequence:=_Sequence)
            If TypeOf (Value) Is Boolean Then
                _useseek = Value
            ElseIf TypeOf (Value) Is String Then
                If LCase(Trim(Value)) = "true" Then
                    _useseek = True
                Else
                    _useseek = False
                End If

            End If

            ' get the path
            Me.PathOrAddress = _configurations.GetProperty(name:=ConstCPNDBPath, setname:=_Session.ConfigSetname, sequence:=_Sequence)

            ' get the Database Name if we have it
            Me.DBName = _configurations.GetProperty(ConstCPNDBName, setname:=_Session.ConfigSetname, sequence:=_Sequence)

            ' get the Database user if we have it
            Me.Dbuser = _configurations.GetProperty(ConstCPNDBUser, setname:=_Session.ConfigSetname, sequence:=_Sequence)


            ' get the Database password if we have it
            Me.Dbpassword = _configurations.GetProperty(name:=ConstCPNDBPassword, setname:=_Session.ConfigSetname, sequence:=_Sequence)

            ' get the Database password if we have it
            Dim UseMars As String = _configurations.GetProperty(name:=ConstCPNDBSQLServerUseMars, setname:=_Session.ConfigSetname, sequence:=_Sequence)

            ' get the connection string
            connectionstring = _configurations.GetProperty(name:=ConstCPNDBConnection, setname:=_Session.ConfigSetname, sequence:=_Sequence)

            '***
            Call CoreMessageHandler(message:="Config connection parameters :" & Me.ID & vbLf & _
                                        " Drivername : " & Me.DatabaseDriver.Name & vbLf & _
                                        " Useseek : " & _useseek.ToString & vbLf & _
                                        " PathOrAddress :" & Me.PathOrAddress & vbLf & _
                                        " DBUser : " & Me.Dbuser & vbLf & _
                                        " DBPassword : " & Me.Dbpassword & vbLf & _
                                        " connectionsstring :" & connectionstring, _
                                        messagetype:=otCoreMessageType.InternalInfo, procedure:="ormConnection.SetconnectionConfigParameters")
            '** default
            '** we have no connection string than build one
            If String.IsNullOrWhiteSpace(connectionstring) Then
                ' build the connectionstring for access
                If DatabaseDriver.Name = ConstCPVDriverOleDB Then
                    If Mid(_Path, Len(_Path), 1) <> "\" Then _Path &= "\"
                    ''' if access file specified
                    If System.IO.Path.GetExtension(_Path & _Name).ToUpper = "ACCDB" Then
                        If System.IO.File.Exists(_Path & _Name) Then
                            Me.Connectionstring = "Provider=Microsoft.ACE.OLEDB.12.0;" & _
                            "Data Source=" & _Path & _Name & ";"
                            Call CoreMessageHandler(message:="Config connection parameters :" & Me.ID & vbLf & _
                                          " created connectionsstring :" & Me.Connectionstring, _
                                          messagetype:=otCoreMessageType.InternalInfo, procedure:="ormConnection.SetconnectionConfigParameters")
                            Return True
                        Else
                            Call CoreMessageHandler(showmsgbox:=True, argument:=_Path & _Name, procedure:="ormConnection.retrieveConfigParameters", _
                                                  message:=" OnTrack access database " & _Name & " doesnot exist at given location " & _Path, _
                                                  break:=False, noOtdbAvailable:=True)
                            '*** reset
                            Call ResetFromConnection()
                            Return False
                        End If
                    Else
                    End If

                    ' build the connectionstring for SQLServer
                ElseIf DatabaseDriver.Name = ConstCPVDriverSQLServer Then
                    ' set the seek
                    _useseek = False
                    Me.Connectionstring = "Data Source=" & _Path & "; Database=" & _Name & ";User Id=" & _Dbuser & ";Password=" & _Dbpassword & ";"
                    If UseMars IsNot Nothing AndAlso CBool(UseMars) Then
                        Me.Connectionstring &= "MultipleActiveResultSets=True;"
                    End If
                    Call CoreMessageHandler(message:="Config connection parameters :" & Me.ID & vbLf & _
                                      " created connectionsstring :" & Me.Connectionstring, _
                                      messagetype:=otCoreMessageType.InternalInfo, procedure:="ormConnection.SetconnectionConfigParameters")
                    Return True
                Else
                    Call CoreMessageHandler(showmsgbox:=True, argument:=_Connectionstring, procedure:="ormConnection.retrieveConfigParameters", _
                                          message:=" OnTrack database " & _Name & " has not a valid database type.", _
                                          break:=False, noOtdbAvailable:=True)
                    '*** reset
                    Call ResetFromConnection()
                    Return False
                End If
            End If


            Return True

        End Function

        '********
        '******** Connect : Connects to the Database and initialize Enviorement
        '********
        '********

        Public MustOverride Function Connect(Optional ByVal force As Boolean = False, _
        Optional ByVal accessRequest As otAccessRight = otAccessRight.[ReadOnly], _
        Optional ByVal domainid As String = Nothing, _
        Optional ByVal OTDBUsername As String = Nothing, _
        Optional ByVal OTDBPassword As String = Nothing, _
        Optional ByVal exclusive As Boolean = False, _
        Optional ByVal notInitialize As Boolean = False, _
        Optional ByVal doLogin As Boolean = True) As Boolean Implements iormConnection.Connect

        ''' <summary>
        ''' Returns a List of Higher Access Rights then the one selected
        ''' </summary>
        ''' <param name="accessrequest"></param>
        ''' <param name="domain" >Domain to validate for</param>
        ''' <param name="Objects" >list of Obejectnames to validate in the domain</param>
        ''' <returns>eturns false if reverification of User is needed or true if currentAccessLevel includes this new request Level</returns>
        ''' <remarks></remarks>

        Private Function HigherAccessRequest(ByVal accessrequest As otAccessRight) As List(Of String)

            Dim aResult As New List(Of String)

            If accessrequest = otAccessRight.AlterSchema Then
                aResult.Add(otAccessRight.AlterSchema.ToString)
            End If

            If accessrequest = otAccessRight.ReadUpdateData Then
                aResult.Add(otAccessRight.AlterSchema.ToString)
                aResult.Add(otAccessRight.ReadUpdateData.ToString)
            End If

            If accessrequest = otAccessRight.ReadOnly Then
                aResult.Add(otAccessRight.AlterSchema.ToString)
                aResult.Add(otAccessRight.ReadUpdateData.ToString)
                aResult.Add(otAccessRight.ReadOnly.ToString)
            End If

            Return aResult
        End Function

        ''' <summary>
        ''' Validate the Access Request against the current Access Level of the user
        ''' </summary>
        ''' <param name="accessrequest"></param>
        ''' <param name="domain" >Domain to validate for</param>
        ''' <param name="Objects" >list of Obejectnames to validate in the domain</param>
        ''' <returns>eturns false if reverification of User is needed or true if currentAccessLevel includes this new request Level</returns>
        ''' <remarks></remarks>

        Public Function ValidateAccessRequest(accessrequest As otAccessRight, _
                                              Optional domainid As String = Nothing, _
                                              Optional ByRef [Objectnames] As List(Of String) = Nothing) As Boolean Implements iormConnection.ValidateAccessRequest

            '

            If accessrequest = _AccessLevel Then
                Return True
            ElseIf accessrequest = otAccessRight.[ReadOnly] And _
            (_AccessLevel = otAccessRight.ReadUpdateData Or _AccessLevel = otAccessRight.AlterSchema) Then
                Return True
            ElseIf accessrequest = otAccessRight.ReadUpdateData And _AccessLevel = otAccessRight.AlterSchema Then
                Return True
                ' will never be reached !
            ElseIf accessrequest = otAccessRight.AlterSchema And _AccessLevel = otAccessRight.AlterSchema Then
                Return True
            End If

            Return False
        End Function

        ''' <summary>
        ''' verify the user access to OnTrack Database - if necessary start a Login with Loginwindow. Check on user rights.
        ''' </summary>
        ''' <param name="accessRequest">needed User right</param>
        ''' <param name="username">default username to use</param>
        ''' <param name="password">default password to use</param>
        ''' <param name="forceLogin">force a Login window in any case</param>
        ''' <param name="loginOnDemand">do a Login window and reconnect if right is not necessary</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function VerifyUserAccess(accessRequest As otAccessRight, _
                                            Optional ByRef username As String = Nothing, _
                                            Optional ByRef password As String = Nothing, _
                                            Optional ByRef domainid As String = Nothing, _
                                            Optional ByRef [Objectnames] As List(Of String) = Nothing, _
                                            Optional useLoginWindow As Boolean = True, Optional messagetext As String = Nothing) As Boolean Implements iormConnection.VerifyUserAccess
            Dim userValidation As UserValidation
            userValidation.ValidEntry = False

            '****
            '**** no connection -> login
            If Not Me.IsConnected Then

                If String.IsNullOrEmpty(domainid) Then domainid = ConstGlobalDomain
                '*** OTDBUsername supplied

                If useLoginWindow And accessRequest <> ConstDefaultAccessRight Then

                    Me.UILogin.EnableUsername = True
                    Me.UILogin.Username = Nothing
                    Me.UILogin.Password = Nothing

                    'LoginWindow
                    Me.UILogin.Configset = ot.CurrentConfigSetName
                    Me.UILogin.PossibleConfigSets = ot.ConfigSetNamesToSelect
                    'Me.UILogin.Databasedriver = Me.DatabaseDriver
                    Me.UILogin.EnableChangeConfigSet = True
                    If messagetext IsNot Nothing Then Me.UILogin.Messagetext = messagetext

                    Me.UILogin.Domain = domainid
                    Me.UILogin.EnableDomain = False

                    '* reset user validation we have
                    _cacheUserValidateon.ValidEntry = False

                    Me.UILogin.Accessright = accessRequest
                    Me.UILogin.enableAccess = True
                    Me.UILogin.PossibleRights = Me.HigherAccessRequest(accessrequest:=accessRequest)

                    Me.UILogin.Show()

                    username = Me.UILogin.Username
                    password = Me.UILogin.Password
                    accessRequest = Me.UILogin.Accessright

                    userValidation = Me.DatabaseDriver.GetUserValidation(username)
                    ' just check the provided username
                ElseIf username <> String.Empty And password <> String.Empty And accessRequest <> ConstDefaultAccessRight Then
                    userValidation = Me.DatabaseDriver.GetUserValidation(username)
                    '* no username but default accessrequest then look for the anonymous user
                ElseIf accessRequest = ConstDefaultAccessRight Then
                    userValidation = Me.DatabaseDriver.GetUserValidation(username:=String.Empty, selectAnonymous:=True)
                    If userValidation.ValidEntry Then
                        username = userValidation.Username
                        password = userValidation.Password
                    End If
                End If

                ' if user is still nothing -> not verified
                If Not userValidation.ValidEntry Then
                    Call CoreMessageHandler(showmsgbox:=True, _
                                          message:=" Access to OnTrack Database is prohibited - User not found", _
                                          argument:=userValidation.Username, noOtdbAvailable:=True, break:=False)

                    _cacheUserValidateon.ValidEntry = False
                    '*** reset
                    Call ResetFromConnection()
                    Return False
                Else

                    '*** old validation again
                    If _cacheUserValidateon.ValidEntry AndAlso userValidation.Password = _cacheUserValidateon.Password And userValidation.Username = _cacheUserValidateon.Username Then
                        '** do nothing

                        '**** Check Password
                        '****
                    ElseIf userValidation.Password = password Then
                        _cacheUserValidateon = userValidation
                        Call CoreMessageHandler(procedure:="ormConnection.verifyUserAccess", break:=False, message:="User verified successfully *", _
                                              argument:=username, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationInfo)
                    Else
                        Call CoreMessageHandler(procedure:="ormConnection.verifyUserAccess", break:=False, message:="User not verified successfully", _
                                              argument:=username, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationError)
                        _cacheUserValidateon.ValidEntry = False
                        Return False
                    End If

                End If

                '****
                '**** CONNECTION !
            Else
                '** stay in the current domain 
                If String.IsNullOrEmpty(domainid) Then domainid = ot.CurrentSession.CurrentDomainID
                '** validate the current user with the request
                If Me.ValidateAccessRequest(accessrequest:=accessRequest, domainid:=domainid) Then
                    Return True
                    '* change the current user if anonymous
                ElseIf useLoginWindow And ot.CurrentSession.OTdbUser.IsAnonymous Then
                    '** check if new OTDBUsername is valid
                    'LoginWindow
                    Me.UILogin.Domain = domainid
                    Me.UILogin.EnableDomain = False
                    Me.UILogin.PossibleDomains = New List(Of String)
                    Me.UILogin.enableAccess = True
                    Me.UILogin.PossibleRights = HigherAccessRequest(accessRequest)
                    Me.UILogin.Configset = ot.CurrentConfigSetName
                    Me.UILogin.EnableChangeConfigSet = False
                    Me.UILogin.Accessright = accessRequest
                    Me.UILogin.Messagetext = "<html><strong>Welcome !</strong><br />Please change to a valid user and password for authorization of the needed access right.</html>"
                    Me.UILogin.EnableUsername = True
                    Me.UILogin.Username = Nothing
                    Me.UILogin.Password = Nothing
                    Me.UILogin.Show()
                    username = LoginWindow.Username
                    password = LoginWindow.Password
                    userValidation = Me.DatabaseDriver.GetUserValidation(username)
                    '* check password -> relogin on connected -> EventHandler ?!
                    If userValidation.Password = password Then
                        Call CoreMessageHandler(procedure:="ormConnection.verifyUserAccess", break:=False, _
                                                message:="User change verified successfully on domain '" & domainid & "'", _
                           argument:=username, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationInfo)
                        '* set the new access level
                        _AccessLevel = accessRequest

                        '** donot change the user
                        'Dim anOTDBUser As User = User.Retrieve(username:=username)
                        'If anOTDBUser IsNot Nothing Then
                        '    _OTDBUser = anOTDBUser
                        '    Me.Session.UserChangedEvent(_OTDBUser)
                        'Else
                        '    CoreMessageHandler(message:="user definition cannot be loaded", messagetype:=otCoreMessageType.InternalError, _
                        '                        arg1:=username, noOtdbAvailable:=False, subname:="ormConnection.verifyUserAccess")
                        '    Return False

                        'End If

                    Else
                        '** fallback
                        username = CurrentSession.OTdbUser.Username
                        password = CurrentSession.OTdbUser.Password
                        Call CoreMessageHandler(procedure:="ormConnection.verifyUserAccess", break:=False, message:="User couldnot be verified - fallback to user " & username, _
                           argument:=username, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationError, showmsgbox:=True)
                        Return False
                    End If
                    '* the current access level is not for this request
                ElseIf useLoginWindow And Not CurrentSession.OTdbUser.IsAnonymous Then
                    '** check if new OTDBUsername is valid
                    'LoginWindow
                    Me.UILogin.Domain = domainid
                    Me.UILogin.EnableDomain = False
                    Me.UILogin.PossibleDomains = New List(Of String)
                    Me.UILogin.enableAccess = True
                    Me.UILogin.PossibleRights = HigherAccessRequest(accessRequest)
                    Me.UILogin.Configset = ot.CurrentConfigSetName
                    Me.UILogin.EnableChangeConfigSet = False
                    Me.UILogin.Accessright = accessRequest

                    Me.UILogin.Messagetext = "<html><strong>Attention !</strong><br />Please confirm by your password to obtain the access right.</html>"
                    Me.UILogin.EnableUsername = False
                    Me.UILogin.Username = CurrentSession.OTdbUser.Username
                    Me.UILogin.Password = password
                    Me.UILogin.Show()
                    ' return input
                    username = LoginWindow.Username
                    password = LoginWindow.Password
                    userValidation = Me.DatabaseDriver.GetUserValidation(username)
                    '* check password
                    If userValidation.Password = password Then
                        Call CoreMessageHandler(procedure:="ormConnection.verifyUserAccess", break:=False, message:="User change verified successfully (1)", _
                           argument:=username, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationInfo)
                        '* set the new access level
                        _AccessLevel = accessRequest
                    Else
                        '** fallback
                        username = CurrentSession.OTdbUser.Username
                        password = CurrentSession.OTdbUser.Password
                        Call CoreMessageHandler(procedure:="ormConnection.verifyUserAccess", break:=False, message:="User couldnot be verified - fallback to user " & username, _
                           argument:=username, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationError, showmsgbox:=True)
                        Return False
                    End If

                    '*** just check the provided username
                ElseIf username <> String.Empty And password <> String.Empty Then
                    userValidation = Me.DatabaseDriver.GetUserValidation(username)
                End If
            End If

            '**** Check the UserValidation Rights

            '* exclude user
            ' TODO AccessRightProperty.CoverRights(rights:=otAccessRight.AlterSchema, covers:=otAccessRight.ReadOnly)

            If userValidation.HasNoRights Then
                Call CoreMessageHandler(showmsgbox:=True, _
                                      message:=" Access to OnTrack Database is prohibited - User has no rights", _
                                      break:=False, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationError)

                '*** reset
                If Not Me.IsConnected Then
                    ResetFromConnection()
                Else
                    ' Disconnect() -> Do not ! fall back to old user
                End If

                Return False
                '* check on the rights
            ElseIf Not userValidation.HasAlterSchemaRights And accessRequest = otAccessRight.AlterSchema Then
                Call CoreMessageHandler(showmsgbox:=True, _
                                      message:=" Access to OnTrack Database is prohibited - User has no alter schema rights", _
                                      break:=False, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationError)

                '*** reset
                If Not Me.IsConnected Then
                    ResetFromConnection()
                Else
                    ' Disconnect() -> Do not ! fall back to old user
                End If
                Return False
            ElseIf Not userValidation.HasUpdateRights And accessRequest = otAccessRight.ReadUpdateData Then
                Call CoreMessageHandler(showmsgbox:=True, _
                                      message:=" Access to OnTrack Database is prohibited - User has no update rights", _
                                      break:=False, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationError)

                '*** reset
                If Not Me.IsConnected Then
                    ResetFromConnection()
                Else
                    ' Disconnect() -> Do not ! fall back to old user
                End If
                Return False
            ElseIf Not userValidation.HasReadRights And accessRequest = otAccessRight.[ReadOnly] Then
                Call CoreMessageHandler(showmsgbox:=True, _
                                      message:=" Access to OnTrack Database is prohibited - User has no read rights", _
                                      break:=False, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationError)

                '*** reset
                If Not Me.IsConnected Then
                    ResetFromConnection()
                Else
                    ' Disconnect() -> Do not ! fall back to old user
                End If
                Return False
            End If
            '*** return true

            Return True

        End Function
    End Class
    '**************
    '************** ConnectionEventArgs for the ConnectionEvents
    ''' <summary>
    ''' defines the Connection Event Arguments
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ormConnectionEventArgs
        Inherits EventArgs

        Private _Connection As iormConnection
        Private _domain As String

        Public Sub New(newConnection As iormConnection, Optional domain As String = Nothing)
            _Connection = newConnection
            _domain = domain
        End Sub
        ''' <summary>
        ''' Gets or sets the domain.
        ''' </summary>
        ''' <value>The domain.</value>
        Public Property DomainID() As String
            Get
                Return Me._domain
            End Get
            Set(value As String)
                Me._domain = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the error.
        ''' </summary>
        ''' <value>The error.</value>
        Public ReadOnly Property [Connection]() As iormConnection
            Get
                Return _Connection
            End Get
        End Property

    End Class


    ''' <summary>
    ''' describes the current schema in the data base (meta data from the native Database)
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class ormContainerSchema
        Implements iormContainerSchema

        Protected _Connection As iormConnection
        Protected _ContainerID As String
        Protected _nativeDBObjectname As String ' the tablename of the table in the database

        Protected _fieldsDictionary As Dictionary(Of String, Long)    ' crossreference to the Arrays
        Protected _indexDictionary As Dictionary(Of String, ArrayList)    ' crossreference of the Index


        Protected _entrynames() As String    ' Fieldnames in OTDB
        Protected _Primarykeys() As UShort    ' indices for primary keys
        Protected _NoPrimaryKeys As UShort
        Protected _PrimaryKeyIndexName As String
        Protected _DomainIDPrimaryKeyOrdinal As Short = -1 ' cache the Primary Key Ordinal of domainID for domainbehavior


        Protected _IsInitialized As Boolean = False
        Protected _lockObject As New Object ' Lock Object

        ''' <summary>
        ''' constuctor
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <param name="containerId"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef connection As iormConnection, ByVal dbobjectid As String)
            ReDim Preserve _entrynames(0)

            _fieldsDictionary = New Dictionary(Of String, Long)
            _indexDictionary = New Dictionary(Of String, ArrayList)
            _Connection = connection
            _ContainerID = dbobjectid
            _NoPrimaryKeys = 0
            ReDim Preserve _Primarykeys(0 To 0)
        End Sub

        ''' <summary>
        ''' Gets or sets the is initialized. Should be True if the tableschema has a containerId 
        ''' </summary>
        ''' <value>The is initialized.</value>
        Public ReadOnly Property IsInitialized() As Boolean Implements iormContainerSchema.IsInitialized
            Get
                Return Me._IsInitialized
            End Get

        End Property

        ''' <summary>
        ''' resets the  to hold nothing
        ''' </summary>
        ''' <remarks></remarks>
        Protected Overridable Sub Reset()
            Dim nullArray As Object = {}
            _entrynames = nullArray
            _fieldsDictionary.Clear()
            _indexDictionary.Clear()
            _ContainerID = Nothing
            _nativeDBObjectname = Nothing
            _PrimaryKeyIndexName = Nothing
            _Primarykeys = nullArray
            _NoPrimaryKeys = 0
            _DomainIDPrimaryKeyOrdinal = -1
        End Sub

        ''' <summary>
        ''' returns the containerId of the table
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public ReadOnly Property ContainerID() As String Implements iormContainerSchema.ContainerID
            Get
                Return _ContainerID
            End Get
        End Property
        ''' <summary>
        ''' returns the native tablename of this table in the database
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NativeDBObjectname As String Implements iormContainerSchema.NativeDBContainerName
            Get
                If _nativeDBObjectname Is Nothing Then
                    _nativeDBObjectname = _Connection.DatabaseDriver.GetNativeDBObjectName(_ContainerID)
                End If
                Return _nativeDBObjectname
            End Get
        End Property

        ''' <summary>
        ''' Names of the Indices of the table
        ''' </summary>
        ''' <value>List(of String)</value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Indices As List(Of String) Implements iormContainerSchema.Indices
            Get
                Return _indexDictionary.Keys.ToList
            End Get

        End Property
        ''' <summary>
        ''' refresh the table schema
        ''' </summary>
        ''' <param name="reloadForce"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function Refresh(Optional reloadForce As Boolean = False) As Boolean Implements iormContainerSchema.Refresh
        ''' <summary>
        ''' returns the primary Key ordinal (1..n) for the domain ID or less zero if not in primary key
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <summary>
        ''' returns the primary Key ordinal (1..n) for the domain ID or less zero if not in primary key
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDomainIDPKOrdinal() As Integer Implements iormContainerSchema.GetDomainIDPKOrdinal
            If _DomainIDPrimaryKeyOrdinal < 0 Then
                Dim i As Integer = Me.GetEntryOrdinal(index:=Domain.ConstFNDomainID)
                If i < 0 Then
                    Return -1
                Else
                    If Not Me.HasPrimaryEntryName(name:=Domain.ConstFNDomainID.ToUpper) Then
                        Return -1
                    Else
                        For i = 1 To Me.NoPrimaryEntries
                            If Me.GetPrimaryEntrynames(i).ToUpper = Domain.ConstFNDomainID.ToUpper Then
                                _DomainIDPrimaryKeyOrdinal = i
                                Return i
                            End If
                        Next
                        Return -1
                    End If
                End If
            Else
                Return _DomainIDPrimaryKeyOrdinal
            End If

        End Function


        ''' <summary>
        ''' Gets the nullable property.
        ''' </summary>
        ''' <param name="index">The index.</param>
        ''' <returns></returns>
        Public MustOverride Function GetNullable(index As Object) As Boolean Implements iormContainerSchema.GetNullable

        ''' <summary>
        ''' returns the default Value
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function GetDefaultValue(ByVal index As Object) As Object Implements iormContainerSchema.GetDefaultValue

        ''' <summary>
        ''' returns if there is a default Value
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function HasDefaultValue(ByVal index As Object) As Boolean Implements iormContainerSchema.HasDefaultValue


        '**** getIndex returns the ArrayList of Fieldnames for the Index or Nothing
        ''' <summary>
        '''  returns the ArrayList of Fieldnames for the Index or empty array list if not found
        ''' </summary>
        ''' <param name="indexname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIndex(indexname As String) As ArrayList Implements iormContainerSchema.GetIndex


            If Not _indexDictionary.ContainsKey(indexname) Then
                Return New ArrayList
            Else
                Return _indexDictionary.Item(indexname)
            End If

        End Function
        '**** hasIndex returns true if index by Name exists
        ''' <summary>
        ''' returns true if index by Name exists
        ''' </summary>
        ''' <param name="indexname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasIndex(indexname As String) As Boolean Implements iormContainerSchema.HasIndex
            If Not _indexDictionary.ContainsKey(indexname) Then
                Return False
            Else
                Return True
            End If

        End Function
        '**** primaryKeyIndexName
        ''' <summary>
        ''' gets the primarykey name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property PrimaryKeyIndexName As String Implements iormContainerSchema.PrimaryKeyIndexName
            Get
                Throw New NotImplementedException
            End Get
        End Property
        '******* return the no. fields
        '*******
        ''' <summary>
        ''' gets the number of fields
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NoEntries() As Integer Implements iormContainerSchema.NoEntries
            Get
                Return UBound(_entrynames) + 1 'zero bound
            End Get
        End Property
        ''' <summary>
        ''' List of Fieldnames
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property EntryNames As List(Of String) Implements iormContainerSchema.EntryNames
            Get
                Return _entrynames.ToList
            End Get
        End Property


        ''' <summary>
        ''' Get the Fieldordinal (position in record) by Index - can be numeric or the columnname
        ''' </summary>
        ''' <param name="anIndex"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEntryOrdinal(index As Object) As Integer Implements iormContainerSchema.GetEntryOrdinal
            Dim i As ULong

            Try
                If IsNumeric(index) Then
                    If CLng(index) > 0 And CLng(index) <= (_entrynames.GetUpperBound(0) + 1) Then
                        Return CLng(index)
                    Else
                        Call CoreMessageHandler(message:="index of column out of range", _
                                         argument:=index, procedure:="ormContainerSchema.getFieldIndex", messagetype:=otCoreMessageType.InternalError)
                        Return i
                    End If
                ElseIf _fieldsDictionary.ContainsKey(index) Then
                    Return _fieldsDictionary.Item(index)
                ElseIf _fieldsDictionary.ContainsKey(index.toupper) Then
                    Return _fieldsDictionary.Item(index.toupper)

                Else
                    Call CoreMessageHandler(message:="index of column out of range", _
                                          argument:=index, procedure:="ormContainerSchema.getFieldIndex", messagetype:=otCoreMessageType.InternalError)
                    Return -1
                End If

            Catch ex As Exception
                Call CoreMessageHandler(argument:=index, procedure:="ormContainerSchema.getFieldIndex", exception:=ex)
                Return -1
            End Try

        End Function


        ''' <summary>
        ''' get the fieldname by index i - nothing if not in range
        ''' </summary>
        ''' <param name="i"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEntryName(ByVal i As Integer) As String Implements iormContainerSchema.GetEntryName

            If i > 0 And i <= UBound(_entrynames) + 1 Then
                Return _entrynames(i - 1)
            Else
                Call CoreMessageHandler(message:="index of column out of range", argument:=i, containerID:=Me.ContainerID, _
                                      messagetype:=otCoreMessageType.InternalError, procedure:="ormContainerSchema.getFieldName")
                Return Nothing
            End If
        End Function

        '*** check if fieldname by Name exists
        ''' <summary>
        ''' check if entryname exists
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasEntryName(ByVal name As String) As Boolean Implements iormContainerSchema.HasEntryName

            For i = LBound(_entrynames) To UBound(_entrynames)
                If _entrynames(i).ToUpper = name.ToUpper Then
                    Return True
                End If
            Next i

            Return False
        End Function

        ''' <summary>
        ''' List of primary key field names
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property PrimaryEntryNames() As List(Of String) Implements iormContainerSchema.PrimaryEntryNames
            Get
                Dim aList As New List(Of String)
                For i = 1 To Me.NoPrimaryEntries
                    aList.Add(Me.GetPrimaryEntrynames(i))
                Next
                Return aList
            End Get
        End Property

        ''' <summary>
        ''' gets the fieldname of the primary key field by number (1..)
        ''' </summary>
        ''' <param name="i">1..n</param>
        ''' <returnsString></returns>
        ''' <remarks></remarks>
        Public Overridable Function GetPrimaryEntrynames(i As UShort) As String Implements iormContainerSchema.GetPrimaryEntryNames
            Dim aCollection As ArrayList

            If i < 1 Then
                Call CoreMessageHandler(procedure:="ormContainerSchema.getPrimaryKeyFieldName", _
                                      message:="primary Key no : " & i.ToString & " is less then 1", _
                                      argument:=i)
                Return String.Empty
            End If

            Try


                If _indexDictionary.ContainsKey(_PrimaryKeyIndexName) Then
                    aCollection = _indexDictionary.Item(_PrimaryKeyIndexName)
                    If i > aCollection.Count Then
                        Call CoreMessageHandler(procedure:="ormContainerSchema.getPrimaryKeyFieldIndex", _
                                              message:="primary Key no : " & i.ToString & " is out of range ", _
                                              argument:=i)
                        Return String.Empty

                    End If

                    '*** return the item (Name)
                    Return aCollection.Item(i - 1)
                Else
                    Call CoreMessageHandler(procedure:="ormContainerSchema.getPrimaryKeyName", _
                                          message:="Primary Key : " & _PrimaryKeyIndexName & " does not exist !", _
                                          argument:=i)
                    Return String.Empty
                End If


            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=False, procedure:="ormContainerSchema.getPrimaryKeyFieldName", _
                                      containerID:=_ContainerID, exception:=ex)
                Return String.Empty
            End Try

        End Function
        ''' <summary>
        ''' gets the fieldname of the primary key field by number
        ''' </summary>
        ''' <param name="i">1..n</param>
        ''' <returnsString></returns>
        ''' <remarks></remarks>
        Public Overridable Function HasPrimaryEntryName(ByRef name As String) As Boolean Implements iormContainerSchema.HasPrimaryEntryName
            Dim aCollection As ArrayList

            Try

                If _indexDictionary.ContainsKey(_PrimaryKeyIndexName) Then
                    aCollection = _indexDictionary.Item(_PrimaryKeyIndexName)

                    '*** return the item (Name)
                    Return aCollection.Contains(name.ToUpper)
                Else
                    Call CoreMessageHandler(procedure:="ormContainerSchema.hasPrimaryKeyName", _
                                          message:="Primary Key : " & _PrimaryKeyIndexName & " does not exist !")
                    Return Nothing
                End If


            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=False, procedure:="ormContainerSchema.hasPrimaryKeyName", _
                                      containerID:=_ContainerID, exception:=ex)
                Return Nothing
            End Try

        End Function

        ''' <summary>
        ''' gets the field ordinal of the primary Key field by number i. (e.g.returns the ordinal of the primarykey field #2)
        ''' </summary>
        ''' <param name="i">number of primary key field 1..n </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GetOrdinalOfPrimaryEntry(i As UShort) As Integer Implements iormContainerSchema.GetOrdinalOfPrimaryEntry
            Dim aCollection As ArrayList
            Dim aFieldName As String


            If i < 1 Then
                Call CoreMessageHandler(procedure:="ormContainerSchema.getPrimaryKeyFieldIndex", _
                                      message:="primary Key no : " & i.ToString & " is less then 1", _
                                      argument:=i)
                GetOrdinalOfPrimaryEntry = -1
                Exit Function
            End If

            Try
                If _indexDictionary.ContainsKey((_PrimaryKeyIndexName)) Then
                    aCollection = _indexDictionary.Item((_PrimaryKeyIndexName))

                    If i > aCollection.Count Then
                        Call CoreMessageHandler(procedure:="ormContainerSchema.getPrimaryKeyFieldIndex", _
                                              message:="primary Key no : " & i.ToString & " is out of range ", _
                                              argument:=i)
                        GetOrdinalOfPrimaryEntry = -1
                        Exit Function
                    End If

                    aFieldName = aCollection.Item(i - 1)
                    GetOrdinalOfPrimaryEntry = _fieldsDictionary.Item((aFieldName))
                    Exit Function
                Else
                    Call CoreMessageHandler(procedure:="ormContainerSchema.getPrimaryKeyFieldIndex", _
                                          message:="primary Key : " & _PrimaryKeyIndexName & " does not exist !", _
                                          argument:=i)
                    System.Diagnostics.Debug.WriteLine("ormContainerSchema: primary Key : " & _PrimaryKeyIndexName & " does not exist !")
                    GetOrdinalOfPrimaryEntry = -1
                    Exit Function
                End If

            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=False, procedure:="ormContainerSchema.getPrimaryKeyFieldIndex", containerID:=Me.ContainerID, exception:=ex)
                Return -1
            End Try
        End Function

        ''' <summary>
        ''' get the number of primary key fields
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function NoPrimaryEntries() As Integer Implements iormContainerSchema.NoPrimaryEntries
            Dim aCollection As ArrayList

            Try


                If _indexDictionary.ContainsKey(_PrimaryKeyIndexName) Then
                    aCollection = _indexDictionary.Item(_PrimaryKeyIndexName)
                    Return aCollection.Count

                Else
                    Call CoreMessageHandler(procedure:="ormContainerSchema.noPrimaryKeysFields", message:="primary Key : " & _PrimaryKeyIndexName & " does not exist !", _
                                          argument:=_PrimaryKeyIndexName, containerID:=_ContainerID)
                    Return -1

                End If

            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=False, procedure:="ormContainerSchema.noPrimaryKeys", containerID:=_ContainerID, exception:=ex)
                Return -1
            End Try


        End Function

    End Class

End Namespace
