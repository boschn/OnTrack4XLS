REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** CORE Classes for On Track Database Backend Library
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2013-08-08
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************''' <summary>
Option Explicit On
Imports System.Collections
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Diagnostics.Debug
Imports System.Text.RegularExpressions
Imports System.Collections.Concurrent

Imports System.IO
Imports System.Threading

Imports OnTrack.Database
Imports OnTrack.Rulez
Imports OnTrack.Rulez.eXPressionTree
Imports System.Reflection
Imports OnTrack.Commons
Imports OnTrack.Core

Namespace OnTrack

    ''' <summary>
    ''' Session Class holds all the Session based Data for On Track Database
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Session
        Implements Core.iDataObjectEngine

        Private _SessionID As String

        '******  PARAMETERS
        Private _DependencySynchroMinOverlap As Integer  '= 7
        Private _DefaultWorkspace As String    '= String.empty
        Private _DefaultCalendarName As String = ConstDefaultCalendarName 'needed for Installation Calendar Setup
        Private _TodayLatency As Integer
        Private _DefaultScheduleTypeID As String = String.Empty
        Private _DefaultDeliverableTypeID As String = String.Empty
        Private _AutoPublishTarget As Boolean = False
        Private _DeliverableUniqueEntries As String()
        Private _DeliverableOnCloningCloneAlso As String()
        Private _DeliverableOnCloningResetEntries As String()

        '*** their names to be stored under
        Public Const ConstCPDependencySynchroMinOverlap = "DependencySynchroMinOverlap"
        Public Const ConstCPDefaultWorkspace = "DefaultWorkspace"
        Public Const ConstCPDefaultCalendarName = "DefaultCalendarName"
        Public Const ConstCPDefaultTodayLatency = "DefaultTodayLatency"
        Public Const ConstCDefaultScheduleTypeID = "DefaultScheduleTypeID"
        Public Const ConstCPDefaultDeliverableTypeID = "DefaultDeliverableTypeID"
        Public Const ConstCPAutoPublishTarget = "AutoPublishTarget"
        Public Const ConstCPDeliverableUniqueEntries = "DeliverableUniqueEntries"
        Public Const ConstCPDeliverableOnCloningCloneAlso = "DeliverableOnCloningCloneAlso"
        Public Const ConstCPDeliverableOnCloningResetEntries = "DeliverableOnCloningResetEntries"

        '*** SESSION
        Private _OTDBUser As Commons.User
        Private _Username As String = String.Empty
        Private _errorLog As SessionMessageLog
        Private _logagent As SessionAgent
        Private _UseConfigSetName As String = String.Empty
        Private _CurrentDomainID As String = ConstGlobalDomain
        Private _loadDomainReqeusted As Boolean = False
        Private _CurrentWorkspaceID As String = String.Empty
        Private _setupID As String = String.Empty
        Private _cacheRepositoryActiveDomain As ormObjectRepository 'cache
        Private _cacheDBDriverStacks As New Dictionary(Of String, Stack(Of iormDatabaseDriver))
        Private _cachePrimaryDBDriver As New Dictionary(Of String, iormDatabaseDriver)
        Private _rulezEngine As rulez.Engine = OnTrack.Rules.Engine()

        ' initialized Flag
        Private _IsInitialized As Boolean = False
        Private _IsStartupRunning As Boolean = False
        Private _IsRunning As Boolean = False
        Private _IsDomainSwitching As Boolean = False
        Private _IsBootstrappingInstallRequested As Boolean = False ' BootstrappingInstall ?
        Private _IsInstallationRunning As Boolean = False ' actual Installallation running ?

        ' the environments
        Private WithEvents _primaryDBDriver As iormRelationalDatabaseDriver
        Private WithEvents _primaryConnection As iormConnection
        Private WithEvents _configurations As ComplexPropertyStore
        Private WithEvents _databasedrivers As New Dictionary(Of String, iormDatabaseDriver) '' the registered database drivers

        ''' current settings
        Private _CurrentDomain As Domain         '' current domain object
        Private _UILogin As UI.CoreLoginForm     '' Login Form to be used
        Private _AccessLevel As otAccessRight    '' current access level 

        Private _DomainRepositories As New Dictionary(Of String, ormObjectRepository) '' the object repository per domain
        Private _ObjectPermissionCache As New Dictionary(Of String, Boolean) '' cache of business object permission
        Private _ValueListCache As New Dictionary(Of String, ValueList) '' value list cache
        Private _DataObjectCaches As New Dictionary(Of String, ObjectCacheManager) ''' caches of the data object cachemanager


        'shadow Reference for Events
        ' our Events
        Public Event OnDomainChanging As EventHandler(Of SessionEventArgs)
        Public Event OnDomainChanged As EventHandler(Of SessionEventArgs)
        Public Event OnWorkspaceChanging As EventHandler(Of SessionEventArgs)
        Public Event OnWorkspaceChanged As EventHandler(Of SessionEventArgs)
        Public Event OnStarted As EventHandler(Of SessionEventArgs)
        Public Event OnEnding As EventHandler(Of SessionEventArgs)
        Public Event OnConfigSetChange As EventHandler(Of SessionEventArgs)
        Public Event ObjectDefinitionChanged As EventHandler(Of ormObjectDefinition.EventArgs)
        Public Event StartOfBootStrapInstallation As EventHandler(Of SessionEventArgs)
        Public Event EndOfBootStrapInstallation As EventHandler(Of SessionEventArgs)



        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="SessionID"> unqiue ID of the Session</param>
        ''' <remarks></remarks>
        Public Sub New(configurations As ComplexPropertyStore, Optional id As String = Nothing)
            '* ID
            If Not String.IsNullOrWhiteSpace(id) Then
                id = UCase(id)
            ElseIf Not String.IsNullOrWhiteSpace(ot.ApplicationName) Then
                id = ot.ApplicationName
            Else
                id = ot.AssemblyName
            End If
            '* session
            _SessionID = ConstDelimiter & Date.Now.ToString("s") & ConstDelimiter & My.Computer.Name & ConstDelimiter _
            & Environment.UserName & ConstDelimiter & id & ConstDelimiter & ot.ApplicationVersion.ToString & ConstDelimiter
            '* init
            _errorLog = New SessionMessageLog(_SessionID)
            _logagent = New SessionAgent(Me)

            '** register the configuration
            If configurations IsNot Nothing Then
                _UseConfigSetName = configurations.CurrentSet
                _configurations = configurations
            End If
        End Sub

        ''' <summary>
        ''' Finalize
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Finalize()
            _primaryDBDriver = Nothing
            _primaryConnection = Nothing
            _logagent = Nothing
            _UILogin = Nothing
            _DomainRepositories = Nothing
            _DataObjectCaches.Clear()
        End Sub

#Region "Properties"
        ''' <summary>
        ''' Gets the rulez engine.
        ''' </summary>
        ''' <value>The rulez engine.</value>
        Public ReadOnly Property RulezEngine() As rulez.Engine
            Get
                Return _rulezEngine
            End Get
        End Property

        ''' <summary>
        ''' returns the Current Database Setup ID (for tables, views and other data)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CurrentSetupID As String
            Get
                If _setupID IsNot Nothing Then 'AndAlso (IsConnected OrElse IsStartingUp OrElse IsInstallationRunning)  -> if dropping no condition is hold
                    Return _setupID
                Else
                    Return String.Empty
                End If

            End Get
        End Property

        ''' <summary>
        ''' Gets or sets an array of entry names of the deliverable object which should be reseted on cloning
        ''' </summary>
        ''' <value>The deliverable on clone reset entries.</value>
        Public Property DeliverableOnCloningResetEntries() As String()
            Get
                Return Me._DeliverableOnCloningResetEntries
            End Get
            Set(value As String())
                Me._DeliverableOnCloningResetEntries = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets an array of object ids which should be cloned also if clonening the deliverable object
        ''' </summary>
        ''' <value>The deliverable on cloning clone also objects.</value>
        Public Property DeliverableOnCloningCloneAlso() As String()
            Get
                Return Me._DeliverableOnCloningCloneAlso
            End Get
            Set(value As String())
                Me._DeliverableOnCloningCloneAlso = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the unique entries per deliverables as array of strings. These Entries will be checked if
        ''' creating or cloneing deliverables
        ''' </summary>
        ''' <value>The deliverable unique entires.</value>
        Public Property DeliverableUniqueEntries() As String()
            Get
                Return Me._DeliverableUniqueEntries
            End Get
            Set(value As String())
                Me._DeliverableUniqueEntries = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the domain ID (if setting then the domains will be switched).
        ''' </summary>
        ''' <value>The domain.</value>
        Public Property CurrentDomainID() As String
            Get
                Return Me._CurrentDomainID
            End Get
            Private Set(value As String)
                _CurrentDomainID = value
            End Set
        End Property
        ''' <summary>
        ''' Gets  the domain.
        ''' </summary>
        ''' <value>The domain.</value>
        Public ReadOnly Property CurrentDomain() As Domain
            Get
                Return Me._CurrentDomain
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the default deliverable type ID.
        ''' </summary>
        ''' <value>The default deliverable type ID.</value>
        Public Property DefaultDeliverableTypeID() As String
            Get
                Return Me._DefaultDeliverableTypeID
            End Get
            Set(value As String)
                Me._DefaultDeliverableTypeID = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the auto publish target.
        ''' </summary>
        ''' <value>The auto publish target.</value>
        Public Property AutoPublishTarget() As Boolean
            Get
                Return Me._AutoPublishTarget
            End Get
            Set(value As Boolean)
                Me._AutoPublishTarget = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the access level.
        ''' </summary>
        ''' <value>The access level.</value>
        Public Property AccessLevel() As otAccessRight
            Get
                Return Me._AccessLevel
            End Get
            Set(value As otAccessRight)
                Me._AccessLevel = value
            End Set
        End Property
        ''' <summary>
        ''' returns true if for the domain a runtime object repository is available
        ''' </summary>
        ''' <param name="domainid"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsRuntimeRepositoryAvailable(Optional domainid As String = Nothing) As Boolean
            Get
                ''' get status on availability
                If (_DomainRepositories Is Nothing OrElse _DomainRepositories.Count = 0) _
                    AndAlso Not _DomainRepositories.ContainsKey(key:=ConstGlobalDomain) Then
                    Return False
                ElseIf (domainid IsNot Nothing AndAlso domainid <> ConstGlobalDomain) _
                    AndAlso Not _DomainRepositories.ContainsKey(key:=domainid) Then
                    Return False
                End If
                ''' define from the status 
                'If Not Me.IsRunning AndAlso Not Me.IsStartingUp AndAlso _
                '    Not Me.IsInstallationRunning AndAlso Not Me.IsBootstrappingInstallationRequested Then
                '    Return False
                'End If

                Return True
            End Get
        End Property
        ''' <summary>
        ''' gets an IDataObjectRepository
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IDataObjectRepository As iDataObjectRepository Implements iDataObjectEngine.Objects
            Get
                Return Me.Objects
            End Get
        End Property
       

        ''' <summary>
        ''' Gets an iormDataObjectRepository for an optional domainid
        ''' if session has no runtime repository then return object class repository
        ''' </summary>
        ''' <value>The Objects.</value>
        Public ReadOnly Property Objects(Optional domainid As String = Nothing) As iormDataObjectRepository
            Get
                If String.IsNullOrWhiteSpace(domainid) Then domainid = ConstGlobalDomain

                If Not Me.IsRuntimeRepositoryAvailable Then
                    If String.IsNullOrEmpty(domainid) Then
                        ''' return the class repository
                        Return ot.ObjectClassRepository
                    Else
                        ''' throw error
                        Throw New ormException(ormException.Types.NoRepositoryAvailable, arguments:={domainid})
                    End If
                End If

                ''' check if there any repository
                ''' 
                If _DomainRepositories.Count = 0 Then
                    Throw New ormException(ormException.Types.SessionNotInitialized)
                End If

                '** retrieve cache
                If _cacheRepositoryActiveDomain Is Nothing Then
                    ''' if domain switching then  use global domain repository untill domain is fully switched
                    If Me.IsDomainSwitching AndAlso _DomainRepositories.ContainsKey(key:=ConstGlobalDomain) Then
                        _cacheRepositoryActiveDomain = _DomainRepositories.Item(key:=ConstGlobalDomain)
                    ElseIf _DomainRepositories.ContainsKey(key:=domainid) Then
                        _cacheRepositoryActiveDomain = _DomainRepositories.Item(key:=domainid)
                    End If
                End If

                ''' never return nothing
                ''' 
                If _cacheRepositoryActiveDomain Is Nothing Then
                    Throw New ormException(ormException.Types.NoRepositoryAvailable, arguments:={domainid})
                End If

                Return _cacheRepositoryActiveDomain
            End Get

        End Property
        ''' <summary>
        ''' gets the object provider object for a specific object id
        ''' </summary>
        ''' <param name="objectid"></param>
        ''' <param name="domainid"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DataObjectProvider(objectid As String, Optional domainid As String = Nothing) As iormDataObjectProvider
            Get
                If Me.IsRuntimeRepositoryAvailable(domainid:=domainid) Then
                    Return Me.Objects(domainid:=domainid).GetDataObjectProvider(objectid)
                End If
                Return Nothing
            End Get
        End Property
        ''' <summary>
        ''' gets the object provider object for a specific object id
        ''' </summary>
        ''' <param name="objectid"></param>
        ''' <param name="domainid"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DataObjectProvider(type As System.Type, Optional domainid As String = Nothing) As iormDataObjectProvider
            Get
                If Me.IsRuntimeRepositoryAvailable(domainid:=domainid) Then
                    Return Me.Objects(domainid:=domainid).GetDataObjectProvider(type)
                End If
                Return Nothing
            End Get
        End Property
        '' <summary>
        ''' gets the object providers
        ''' </summary>
        ''' <param name="objectid"></param>
        ''' <param name="domainid"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DataObjectProviders(Optional domainid As String = Nothing) As List(Of iormDataObjectProvider)
            Get
                Return Me.Objects(domainid:=domainid).DataObjectProviders
            End Get
        End Property
        ''' <summary>
        ''' returns a list of all cached Valuelists names
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ValueListIDs As IList(Of String)
            Get
                Return _ValueListCache.Keys.ToList
            End Get
        End Property
        ''' <summary>
        ''' returns a list of all cached Valuelists
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ValueLists As IList(Of ValueList)
            Get
                Return _ValueListCache.Values.ToList
            End Get
        End Property

        ''' <summary>
        ''' returns a list of all cached Valuelists
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ValueList(name As String) As ValueList
            Get
                If _ValueListCache.ContainsKey(name.ToUpper) Then
                    Return _ValueListCache.Item(name.ToUpper)
                Else
                    Dim aVL As ValueList = Commons.ValueList.Retrieve(id:=name, domainid:=Me.CurrentDomainID)
                    If aVL IsNot Nothing Then
                        _ValueListCache.Add(key:=name.ToUpper, value:=aVL)
                        Return aVL
                    End If
                End If
                Return Nothing
            End Get
        End Property
        ''' <summary>
        ''' returns the Values of a ValueList
        ''' </summary>
        ''' <param name="name"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ValueListValues(name As String) As IList(Of Object)
            Get
                Dim aVL As ValueList = Me.ValueList(name:=name)
                If aVL IsNot Nothing Then
                    Return aVL.Values
                Else
                    Return New List(Of Object)
                End If
            End Get
        End Property
        ''' <summary>
        ''' Gets the user name.
        ''' </summary>
        ''' <value>The user name.</value>
        Public ReadOnly Property CurrentUsername() As String
            Get
                Return Me._Username
            End Get
        End Property
        ''' <summary>
        '''  returns if session is running
        ''' </summary>
        ''' <value>The is running.</value>
        Public Property IsRunning() As Boolean
            Get
                Return Me._IsRunning
            End Get
            Private Set(value As Boolean)
                _IsRunning = value
            End Set
        End Property

        ''' Gets the O TDB user.
        ''' </summary>
        ''' <value>The O TDB user.</value>
        Public ReadOnly Property OTdbUser() As User
            Get
                Return Me._OTDBUser
            End Get
        End Property
        ''' <summary>
        ''' Gets the configurations ComplexPropertyStore.
        ''' </summary>
        ''' <value>The configurations.</value>
        Public ReadOnly Property Configurations() As ComplexPropertyStore
            Get
                Return Me._configurations
            End Get
        End Property
        ''' <summary>
        ''' returns the setname to be used to connect to the databased
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConfigSetname As String
            Get
                Return _configurations.CurrentSet
            End Get
            Set(value As String)
                If _UseConfigSetName <> value Then
                    If Not Me.IsRunning Then
                        _configurations.CurrentSet = value ' raises event
                    Else
                        CoreMessageHandler(message:="a running session can not be set to another config set name", argument:=value, messagetype:=otCoreMessageType.ApplicationError, procedure:="Sesion.setname")
                    End If
                End If
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the today latency.
        ''' </summary>
        ''' <value>The today latency.</value>
        Public Property TodayLatency() As Integer
            Get
                Return Me._TodayLatency
            End Get
            Set(value As Integer)
                Me._TodayLatency = value
            End Set
        End Property

        ''' <summary>
        ''' set or gets the DefaultScheduleTypeID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DefaultScheduleTypeID As String
            Get
                Return _DefaultScheduleTypeID
            End Get
            Set(ByVal value As String)
                _DefaultScheduleTypeID = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the default name of the calendar.
        ''' </summary>
        ''' <value>The default name of the calendar.</value>
        Public Property DefaultCalendarName() As String
            Get
                Return Me._DefaultCalendarName
            End Get
            Set(value As String)
                Me._DefaultCalendarName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the default workspaceID.
        ''' </summary>
        ''' <value>The default workspaceID.</value>
        Public Property DefaultWorkspaceID() As String
            Get
                Return Me._DefaultWorkspace
            End Get
            Set(value As String)
                Me._DefaultWorkspace = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the is bootstrapping mode.
        ''' </summary>
        ''' <value>The is bootstrapping installation.</value>
        Public Property IsBootstrappingInstallationRequested() As Boolean
            Get
                Return Me._IsBootstrappingInstallRequested
            End Get
            Private Set(value As Boolean)
                Me._IsBootstrappingInstallRequested = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the is installation Mode
        ''' </summary>
        ''' <value>The is bootstrapping installation.</value>
        Public Property IsInstallationRunning() As Boolean
            Get
                Return Me._IsInstallationRunning
            End Get
            Private Set(value As Boolean)
                Me._IsInstallationRunning = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the is statup Mode
        ''' </summary>
        ''' <value></value>
        Public Property IsStartingUp() As Boolean
            Get
                Return Me._IsStartupRunning
            End Get
            Private Set(value As Boolean)
                Me._IsStartupRunning = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the is statup Mode
        ''' </summary>
        ''' <value></value>
        Public Property IsDomainSwitching() As Boolean
            Get
                Return Me._IsDomainSwitching
            End Get
            Private Set(value As Boolean)
                Me._IsDomainSwitching = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the default workspaceID.
        ''' </summary>
        ''' <value>The default workspaceID.</value>
        Public Property CurrentWorkspaceID() As String
            Get
                Return Me._CurrentWorkspaceID
            End Get
            Set(value As String)
                If value <> _CurrentWorkspaceID Then
                    Dim e As SessionEventArgs = New SessionEventArgs(session:=Me, newWorkspaceid:=value)
                    RaiseEvent OnWorkspaceChanging(sender:=Me, e:=e)
                    If e.AbortOperation Then Return
                    Me._CurrentWorkspaceID = value
                    RaiseEvent OnWorkspaceChanging(sender:=Me, e:=e)
                End If
            End Set
        End Property
        ''' <summary>
        ''' the errorlog of the session
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Errorlog As SessionMessageLog
            Get
                Return _errorLog
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the dependency synchro min overlap.
        ''' </summary>
        ''' <value>The dependency synchro min overlap.</value>
        Public Property DependencySynchroMinOverlap() As Integer
            Get
                Return Me._DependencySynchroMinOverlap
            End Get
            Set(value As Integer)
                Me._DependencySynchroMinOverlap = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the UI login.
        ''' </summary>
        ''' <value>The UI login.</value>
        Public Property UILogin() As UI.CoreLoginForm
            Get
                If _UILogin Is Nothing Then
                    _UILogin = New UI.CoreLoginForm()
                End If
                Return Me._UILogin
            End Get
            Set(value As UI.CoreLoginForm)
                Me._UILogin = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the is initialized.
        ''' </summary>
        ''' <value>The is initialized.</value>
        Public Property IsInitialized() As Boolean
            Get
                Return Me._IsInitialized
            End Get
            Private Set(value As Boolean)
                Me._IsInitialized = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the current OnTrack DB driver.
        ''' </summary>
        ''' <value>The primary DB driver.</value>
        Public Property OTDBDriver() As iormOnTrackDriver
            Get
                If Me.IsInitialized OrElse Me.Initialize Then
                    Return Me._primaryDBDriver
                Else
                    Return Nothing
                End If
            End Get
            Protected Set(value As iormOnTrackDriver)
                Me._primaryDBDriver = value
                Me._primaryConnection = value.CurrentConnection
                If Not Me.HasDatabaseDriver(id:=value.ID) Then Me.RegisterDatabaseDriver(value)
                Me.IsInitialized = True
            End Set
        End Property
        ''' <summary>
        ''' Gets the session ID.
        ''' </summary>
        ''' <value>The session ID.</value>
        Public ReadOnly Property SessionID() As String Implements iDataObjectEngine.ID
            Get
                Return Me._SessionID
            End Get
        End Property
#End Region


        ''' <summary>
        ''' Event Handler for the Current ConfigurationSet Changed Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnCurrentConfigSetChanged(sender As Object, e As ComplexPropertyStore.EventArgs) Handles _configurations.OnCurrentSetChanged
            '** do only something if we have run through
            If Me.IsRunning Then
                '** do nothing if we are running
                CoreMessageHandler(message:="current config set name was changed after session is running -ignored", procedure:="OnCurrentConfigSetChanged", argument:=e.Setname, messagetype:=otCoreMessageType.InternalError)
            Else
                ''' create or get the Database Driver
                _primaryDBDriver = CreateOnTrackDBDriverInstance(session:=Me)
                If _primaryDBDriver IsNot Nothing Then
                    '** set the connection for events
                    _primaryConnection = _primaryDBDriver.CurrentConnection
                    If _primaryConnection Is Nothing Then
                        CoreMessageHandler(message:="The database connection could not be set - initialization of session aborted ", _
                                           noOtdbAvailable:=True, procedure:="Session.OnCurrentConfigSetChange", _
                                           messagetype:=otCoreMessageType.InternalInfo)
                    End If
                End If

            End If

        End Sub
        ''' <summary>
        ''' Event Handler for the Configuration Property Changed Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnConfigPropertyChanged(sender As Object, e As ComplexPropertyStore.EventArgs) Handles _configurations.OnPropertyChanged
            '** do only something if we have run through
            If Me.IsRunning Then
                '** do nothing if we are running
                CoreMessageHandler(message:="current config set name was changed after session is running -ignored", procedure:="OnCurrentConfigSetChanged", argument:=e.Setname, messagetype:=otCoreMessageType.InternalError)
            Else
                If Me.IsInitialized Then
                    ''' propagate the change shoud be running automatically 
                End If
            End If
        End Sub
        ''' <summary>
        ''' retrieves the primary database drivers for a specific container id
        ''' </summary>
        ''' <param name="containerID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Function GetPrimaryDatabaseDriver(containerID As String) As iormDatabaseDriver
            ''' use cache
            If _cachePrimaryDBDriver.ContainsKey(containerID.ToUpper) Then Return _cachePrimaryDBDriver.Item(containerID.ToUpper)

            ''' retrieve
            Dim aContainerDefinition = Me.Objects.GetContainerDefinition(id:=containerID)
            Dim aDatabaseDriver As iormDatabaseDriver = Me.RetrieveDatabaseDriver(aContainerDefinition.PrimaryDatabaseDriverID)
            If aDatabaseDriver IsNot Nothing Then
                _cachePrimaryDBDriver.Add(key:=containerID.ToUpper, value:=Me.RetrieveDatabaseDriver(aContainerDefinition.PrimaryDatabaseDriverID))
                Return aDatabaseDriver
            End If

            ''' error
            ''' 
            Throw New ormException(ormException.Types.NoPrimaryDatabaseDriverFound, arguments:={containerID})

        End Function
        ''' <summary>
        ''' retrieves the database drivers for a specific container id
        ''' </summary>
        ''' <param name="containerID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Function GetDatabaseDrivers(containerID As String) As Stack(Of iormDatabaseDriver)

            If _cacheDBDriverStacks.ContainsKey(containerID.ToUpper) Then Return _cacheDBDriverStacks.Item(containerID.ToUpper)
            ''' build and cache
            Dim aContainerDefinition = Me.Objects.GetContainerDefinition(id:=containerID)
            Dim aStack As New Stack(Of iormDatabaseDriver)
            For Each aDatabaseID In aContainerDefinition.DatabaseDriverStack
                Dim aDriver As iormDatabaseDriver = Me.RetrieveDatabaseDriver(id:=aDatabaseID)
                If aDriver IsNot Nothing Then aStack.Push(aDriver)
            Next
            _cacheDBDriverStacks.Add(key:=containerID.ToUpper, value:=aStack)
            Return aStack

        End Function
        ''' <summary>
        ''' returns the OnTrack database driver (OnTrack Environment Database) for a session
        ''' </summary>
        ''' <param name="configsetname"></param>
        ''' <param name="session"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Function CreateOnTrackDBDriverInstance(Optional session As Session = Nothing) As iormOnTrackDriver
            Dim avalue As Object

            If session Is Nothing Then session = ot.CurrentSession
            If _primaryDBDriver IsNot Nothing Then Me.DeRegisterDatabaseDriver(_primaryDBDriver)

            ''' check the value of ConstCPNDrivername which must be matching to a driver attribute
            ''' 
            avalue = _configurations.GetProperty(name:=ConstCPNDriverName, setname:=session.ConfigSetname)
            If String.IsNullOrWhiteSpace(avalue) Then
                Call CoreMessageHandler(showmsgbox:=True, message:="Initialization of database driver failed. Declaration of Database Driver Name " & ConstCPNDriverName & " is missing in the config set '" & session.ConfigSetname & "'.", _
                                       noOtdbAvailable:=True, argument:=avalue, procedure:="Session.CreateOnTrackDatabaseDriver", messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If

            ''' returns a driver by the name in the configuration
            ''' 
            Dim aDBDriverAttribute As ormDatabaseDriverAttribute = ot.ObjectClassRepository.GetDBDriverAttributes.Where(Function(x) x.Name.ToUpper = avalue.toupper).FirstOrDefault
            If aDBDriverAttribute IsNot Nothing Then
                ''' create an instance
                Dim aDriver As iormDatabaseDriver = Activator.CreateInstance(aDBDriverAttribute.Type)
                ''' check against the instance if OnTrackDriver
                If aDriver.IsOnTrackDriver Then
                    ''' an Instance ID
                    Dim anID As String = _configurations.GetProperty(name:=ConstCPNDriverID, setname:=session.ConfigSetname)
                    If Not String.IsNullOrWhiteSpace(anID) Then
                        aDriver.ID = anID
                    Else
                        aDriver.ID = New Guid().ToString
                    End If
                    ''' set the session
                    aDriver.Session = Me
                    '' setting to the ontrack driver must be handled from the caller
                    '' here we only register the driver
                    Me.RegisterDatabaseDriver(aDriver)
                    Return aDriver
                Else
                    Call CoreMessageHandler(showmsgbox:=True, message:="Initialization of database driver failed. Type of Database Driver Name '" & avalue.toupper & "' is not a primary OnTrack Database Driver.", _
                                                     noOtdbAvailable:=True, argument:=avalue, procedure:="Session.CreateOnTrackDatabaseDriver", messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If

            Else
                Call CoreMessageHandler(showmsgbox:=True, message:="Initialization of database driver failed. Database Driver Name '" & avalue.toupper & "' is invalid or not implemented.", _
                                                      noOtdbAvailable:=True, argument:=avalue, procedure:="Session.CreateOnTrackDatabaseDriver", messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If

        End Function
        ''' <summary>
        ''' returns a registered database driver
        ''' </summary>
        ''' <param name="databasedriver"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RetrieveDatabaseDriver(id As String) As iormDatabaseDriver
            If _databasedrivers.ContainsKey(key:=id.ToUpper) Then
                Return _databasedrivers.Item(key:=id.ToUpper)
            ElseIf String.Compare(id, ConstDefaultPrimaryDBDriver, ignoreCase:=True) = 0 Then
                Return Me.OTDBDriver
            End If
            Return Nothing
        End Function
        ''' <summary>
        ''' register a database driver at the session
        ''' </summary>
        ''' <param name="databasedriver"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeRegisterDatabaseDriver(databasedriver As iormDatabaseDriver) As Boolean
            If _databasedrivers.ContainsKey(key:=databasedriver.ID) Then
                _databasedrivers.Remove(key:=databasedriver.ID)
                Return True
            End If
            CoreMessageHandler(message:="could not de-register database driver at session - not registered", argument:=databasedriver.ID, procedure:="Session.RegisterDatabaseDriver", messagetype:=otCoreMessageType.InternalWarning)
            Return True
        End Function
        ''' <summary>
        ''' register a database driver at the session
        ''' </summary>
        ''' <param name="databasedriver"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RegisterDatabaseDriver(databasedriver As iormDatabaseDriver) As Boolean
            If databasedriver IsNot Nothing AndAlso databasedriver.ID IsNot Nothing AndAlso Not _databasedrivers.ContainsKey(key:=databasedriver.ID) Then
                _databasedrivers.Add(key:=databasedriver.ID, value:=databasedriver)
                Return True
            End If
            Return False
        End Function
        ''' <summary>
        ''' register a database driver at the session
        ''' </summary>
        ''' <param name="databasedriver"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasDatabaseDriver(id As String) As Boolean
            If Not _databasedrivers.ContainsKey(key:=id.ToUpper) Then Return True
            Return False
        End Function

        ''' <summary>
        ''' registers all drivers
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AutoRegisterDatabaseDrivers() As Boolean
            Try
                ''' check the class type attributes
                '''
                For Each aDriverAttribute As ormDatabaseDriverAttribute In ot.ObjectClassRepository.GetDBDriverAttributes()
                    ''' Object Attribute
                    ''' 
                    If aDriverAttribute.HasValueAutoInstance AndAlso aDriverAttribute.AutoInstance Then
                        Dim aDriver As iormDatabaseDriver = Activator.CreateInstance(aDriverAttribute.Type)
                        Me.RegisterDatabaseDriver(aDriver)
                    End If
                Next

                Return True
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="Session.AutoRegisterDatabaseDriver")
                Return False
            End Try


        End Function
        ''' <summary>
        ''' Initialize the Session 
        ''' </summary>
        ''' <param name="DBDriver">DBDriver to be provided</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Initialize(Optional useConfigsetName As String = Nothing) As Boolean
            '
            Try
                If Me.IsInitialized Then Return True

                '*** Retrieve Config Properties and set the Bag
                If Not ot.RetrieveConfigProperties() Then
                    Call CoreMessageHandler(showmsgbox:=True, message:="config properties couldnot be retrieved - Initialized failed. ", _
                                            noOtdbAvailable:=True, procedure:="Session.Initialize", messagetype:=otCoreMessageType.InternalError)
                    Return False
                Else
                    Call CoreMessageHandler(showmsgbox:=False, message:="config properties could be retrieved", _
                                            noOtdbAvailable:=True, procedure:="Session.Initialize", messagetype:=otCoreMessageType.InternalInfo)
                End If

                ' set the configuration set to be used
                If String.IsNullOrWhiteSpace(useConfigsetName) Then
                    '** get the default - trigger change event
                    If _configurations.CurrentSet IsNot Nothing Then
                        useConfigsetName = _configurations.CurrentSet
                    Else
                        useConfigsetName = _configurations.GetProperty(name:=ConstCPNUseConfigSetName, setname:=ConstGlobalConfigSetName)
                    End If

                ElseIf Not _configurations.HasSet(useConfigsetName) Then
                    Call CoreMessageHandler(message:="config properties set could not be retrieved from config set properties - Initialized failed.", _
                                           noOtdbAvailable:=True, procedure:="Session.Initialize", messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If
                '** set a specific - trigger change event
                _configurations.CurrentSet = useConfigsetName
                '** set the initial Setup ID
                _setupID = ot.GetConfigProperty(ConstCPNSetupID)
                '** autoregister database drivers
                If Not Me.AutoRegisterDatabaseDrivers() Then
                    Call CoreMessageHandler(message:="failure in auto registering database drivers.", _
                                            noOtdbAvailable:=True, procedure:="Session.Initialize", messagetype:=otCoreMessageType.InternalError)

                End If

                '** here we should have a database driver and a connection by event handling
                '** and reading the properties if not something is wrong
                If _primaryDBDriver Is Nothing OrElse _primaryConnection Is Nothing Then
                    Call CoreMessageHandler(showmsgbox:=True, message:="config properties are invalid - Session to Ontrack failed to initialize. ", _
                                           noOtdbAvailable:=True, procedure:="Session.Initialize", messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If

                ''' create Object Cache
                If _DataObjectCaches.Count = 0 Then _DataObjectCaches.Add(key:=ConstGlobalDomain, value:=New ObjectCacheManager(Me, ConstGlobalDomain))
                ot.ObjectClassRepository.RegisterCacheManager(_DataObjectCaches.First.Value)
                ''' start caching
                _DataObjectCaches.First.Value.Start()

                '** create ObjectStore
                Dim aRepository As New ormObjectRepository(Me, ConstGlobalDomain)
                ''' register the cache at the repository
                aRepository.RegisterCache(_DataObjectCaches.First.Value)
                _DomainRepositories.Clear()
                _DomainRepositories.Add(key:=ConstGlobalDomain, value:=aRepository)

                _CurrentDomainID = ConstGlobalDomain
                _loadDomainReqeusted = True
                _CurrentDomain = Nothing

                ''' register at Engine
                _rulezEngine.AddDataEngine(Me)

                '** fine 
                Call CoreMessageHandler(message:="The Session '" & Me.SessionID & "' is initialized ", _
                                        noOtdbAvailable:=True, procedure:="Session.Initialize", _
                                        messagetype:=otCoreMessageType.InternalInfo)

                _IsInitialized = True
                Return Me.IsInitialized

            Catch ex As Exception

                Call CoreMessageHandler(exception:=ex, noOtdbAvailable:=True, procedure:="Session.Initialize")
                Return False
            End Try



        End Function
        ''' <summary>
        ''' EventHandler for BootstrapInstall requested by primaryDBDriver
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub OnRequestBootstrapInstall(sender As Object, e As SessionBootstrapEventArgs) Handles _primaryDBDriver.RequestBootstrapInstall
            If Not _IsInitialized AndAlso Not Initialize() Then Return

            If Not _IsBootstrappingInstallRequested Then
                If _primaryDBDriver IsNot Nothing Then
                    _IsBootstrappingInstallRequested = True
                    RaiseEvent StartOfBootStrapInstallation(Me, New SessionEventArgs(Me))
                    Call CoreMessageHandler(procedure:="Session.OnRequestBootstrapInstall", message:="bootstrapping mode started", _
                                               argument:=Me.SessionID, messagetype:=otCoreMessageType.InternalInfo)
                End If
            End If

            If Not _IsInstallationRunning AndAlso e.Install Then
                Call CoreMessageHandler(procedure:="Session.OnRequestBootstrapInstall", message:="bootstrapping installation started", _
                                                argument:=Me.SessionID, messagetype:=otCoreMessageType.InternalInfo)
                '** issue an installation
                e.InstallationResult = _primaryDBDriver.InstallOnTrackDatabase(askBefore:=e.AskBefore, modules:=e.Modules)
            End If
        End Sub
        ''' <summary>
        ''' initialize on Connection Event
        ''' </summary>
        ''' <value>The session ID.</value>

        Private Sub OnConnecting(sender As Object, e As ormConnectionEventArgs) Handles _primaryConnection.OnConnection
            Me.StartUpSessionEnviorment(force:=True, domainid:=e.DomainID)
        End Sub

        ''' <summary>
        ''' initialize on Connection Event
        ''' </summary>
        ''' <value>The session ID.</value>

        Private Sub OnDisConnecting(sender As Object, e As ormConnectionEventArgs) Handles _primaryConnection.OnDisconnection
            Me.ShutDownSessionEnviorment()
        End Sub
        ''' <summary>
        ''' Install the Ontrack database
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InstallOnTrackDatabase(Optional sequence As ComplexPropertyStore.Sequence = ComplexPropertyStore.Sequence.Primary) As Boolean
            '** lazy initialize
            If Not Me.IsInitialized AndAlso Not Me.Initialize() Then
                CoreMessageHandler(procedure:="Session.InstallOnTrackDatabase", message:="failed to initialize session", _
                                        argument:=Me.SessionID, messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            '** install
            If sequence = sequence.Primary Then
                '** set domainid to global without switching
                _CurrentDomainID = ot.ConstGlobalDomain
                '** go into global
                If _primaryDBDriver.InstallOnTrackDatabase(askBefore:=True, modules:={}) Then
                    Return True
                Else
                    CoreMessageHandler(procedure:="Session.InstallOnTrackDatabase", message:="installation failed", _
                                        argument:=Me.SessionID, messagetype:=otCoreMessageType.InternalError)
                End If
            Else
                CoreMessageHandler(procedure:="Session.InstallOnTrackDatabase", message:="other sequences not implemented", _
                                        argument:=Me.SessionID, messagetype:=otCoreMessageType.InternalError)
                Return False
            End If


        End Function
        ''' <summary>
        ''' Abort the Starting up if possible
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RequestToAbortStartingUp() As Boolean
            _IsStartupRunning = False
            Return Not _IsStartupRunning
        End Function
        ''' <summary>
        ''' requests and checks if an end of bootstrap is possible 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RequestEndofBootstrap() As Boolean
            '** lazy initialize
            If Not Me.IsInitialized AndAlso Not Me.Initialize() Then
                Call CoreMessageHandler(procedure:="Session.RequestEndofBootstrap", message:="failed to initialize session", _
                                        argument:=Me.SessionID, messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            If Me.IsBootstrappingInstallationRequested Then
                '** check should not only be on existence also on the columns
                If Not OTDBDriver.VerifyOnTrackDatabase Then
                    '** raise event
                    RaiseEvent EndOfBootStrapInstallation(Me, New SessionEventArgs(Me, abortOperation:=True))
                    Call CoreMessageHandler(procedure:="Session.RequestEndofBootstrap", message:="bootstrapping aborted - verify failed", _
                                        argument:=Me.SessionID, messagetype:=otCoreMessageType.InternalInfo)
                    Me.IsBootstrappingInstallationRequested = False
                    Me.IsInstallationRunning = False
                    Return False ' return false to indicate that state is not ok
                Else
                    '** raise event
                    RaiseEvent EndOfBootStrapInstallation(Me, New SessionEventArgs(Me))
                    Call CoreMessageHandler(procedure:="Session.RequestEndofBootstrap", message:="bootstrapping ended", _
                                        argument:=Me.SessionID, messagetype:=otCoreMessageType.InternalInfo)
                    Me.IsBootstrappingInstallationRequested = False
                    Me.IsInstallationRunning = False
                    Return True
                End If
            Else
                Return True
            End If
        End Function
        ''' <summary>
        ''' requires from OTDB the Access Rights - starts a session if not running otherwise just validates
        ''' </summary>
        ''' <param name="AccessRequest">otAccessRight</param>
        ''' <returns>True if successfull</returns>
        ''' <remarks></remarks>
        Public Function RequireAccessRight(accessRequest As otAccessRight, _
                                            Optional domainID As String = Nothing, _
                                            Optional reLogin As Boolean = True) As Boolean
            Dim anUsername As String
            '** lazy initialize
            If Not Me.IsInitialized AndAlso Not Me.Initialize() Then
                Call CoreMessageHandler(procedure:="Session.RequireAccessRight", message:="failed to initialize session", _
                                        argument:=Me.SessionID, messagetype:=otCoreMessageType.InternalError)
                Return False
            End If
            '* take the OTDBDriver
            If _primaryDBDriver Is Nothing Then
                Me.OTDBDriver = CurrentOTDBDriver
            End If

            '* how to check and wha to do

            If Me.IsRunning Then
                If String.IsNullOrEmpty(domainID) Then domainID = Me.CurrentDomainID
                anUsername = Me.OTdbUser.Username

                Return Me.RequestUserAccess(accessRequest:=accessRequest, username:=anUsername, domainid:=domainID, loginOnFailed:=reLogin)
            Else
                If String.IsNullOrEmpty(domainID) Then domainID = ConstGlobalDomain

                If Me.StartUp(AccessRequest:=accessRequest, domainID:=domainID) Then
                    Return Me.ValidateAccessRights(accessrequest:=accessRequest, domainid:=domainID)
                Else
                    CoreMessageHandler(message:="failed to startup a session", procedure:="Session.RequireAccessRight", messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If
            End If

        End Function
        ''' <summary>
        ''' Raises the Event ObjectChagedDefinition
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub RaiseObjectChangedDefinitionEvent(sender As Object, e As ormObjectDefinition.EventArgs)
            If _DomainRepositories.ContainsKey(key:=_CurrentDomainID) Then
                _DomainRepositories.Item(key:=_CurrentDomainID).OnObjectDefinitionChanged(sender, e)
            End If
        End Sub
        ''' <summary>
        ''' Raises the Event RaiseChangeConfigSet
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub RaiseChangeConfigSetEvent(setname As String)
            RaiseEvent OnConfigSetChange(Me, New SessionEventArgs(session:=Me, newConfigSetName:=setname))

        End Sub

        ''' <summary>
        ''' Validate the User against the Database with the accessRight
        ''' </summary>
        ''' <param name="username"></param>
        ''' <param name="password"></param>
        ''' <param name="accessRequest"></param>
        ''' <param name="domainID"></param>
        ''' <param name="databasedriver"></param>
        ''' <param name="uservalidation"></param>
        ''' <param name="messagetext"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ValidateUser(ByVal username As String, ByVal password As String, ByVal accessRequest As otAccessRight, ByVal domainID As String, _
                                      Optional databasedriver As iormRelationalDatabaseDriver = Nothing, _
                                      Optional uservalidation As UserValidation = Nothing, _
                                      Optional messagetext As String = Nothing) As Boolean

            If databasedriver Is Nothing Then databasedriver = _primaryDBDriver
            If databasedriver Is Nothing Then
                CoreMessageHandler(message:="database driver is not available ", procedure:="Session.ValidateUser", messagetype:=otCoreMessageType.InternalError)
                Return False
            End If
            Return databasedriver.ValidateUser(username:=username, password:=password, accessRequest:=accessRequest)
        End Function

        ''' <summary>
        ''' Validate the Access Request against the current OnTrack DB Access Level of the user and the objects operations
        ''' (database driver and connection are checking just the access level)
        ''' </summary>
        ''' <param name="accessrequest"></param>
        ''' <param name="domain" >Domain to validate for</param>
        ''' <param name="Objects" >list of Obejectnames to validate in the domain</param>
        ''' <returns>eturns false if reverification of User is needed or true if currentAccessLevel includes this new request Level</returns>
        ''' <remarks></remarks>

        Public Function ValidateAccessRights(accessrequest As otAccessRight, _
                                                Optional domainid As String = Nothing, _
                                                Optional ByRef objecttransactions As String() = Nothing) As Boolean
            Dim result As Boolean = False

            '** during startup we might not have a otdbuser
            If Me.IsStartingUp AndAlso (_OTDBUser Is Nothing OrElse Not _OTDBUser.IsAlive) Then
                Return True
            ElseIf _OTDBUser Is Nothing OrElse Not _OTDBUser.IsAlive Then
                CoreMessageHandler(message:="no otdb user is loaded into the session -failed to validate accessrights", messagetype:=otCoreMessageType.InternalError, _
                                                  procedure:="Session.validateAccessRights")
                Return False
            End If

            '** check on the ontrackdatabase request
            result = AccessRightProperty.CoverRights(rights:=_AccessLevel, covers:=accessrequest)
            If Not result Then Return result

            'exit 
            If objecttransactions Is Nothing OrElse objecttransactions.Count = 0 OrElse Me.IsBootstrappingInstallationRequested Then Return result

            '** check all objecttransactions if level iss sufficent
            For Each opname In objecttransactions
                '** check cache
                If _ObjectPermissionCache.ContainsKey(opname.ToUpper) Then
                    result = result And True
                Else
                    Dim anObjectname As String
                    Dim anTransactionname As String
                    Shuffle.NameSplitter(opname, anObjectname, anTransactionname)
                    If anObjectname Is Nothing OrElse anObjectname = String.Empty Then
                        CoreMessageHandler(message:="ObjectID is missing in operation name", argument:=opname, procedure:="Session.validateOTDBAccessLevel", messagetype:=otCoreMessageType.InternalError)
                        result = result And False
                    ElseIf anTransactionname Is Nothing OrElse anTransactionname = String.Empty Then
                        CoreMessageHandler(message:="Operation Name is missing in operation name", argument:=opname, procedure:="Session.validateOTDBAccessLevel", messagetype:=otCoreMessageType.InternalError)
                        result = result And False
                    Else
                        Dim aObjectDefinition = Me.Objects.GetObjectDefinition(id:=anObjectname, runtimeOnly:=Me.IsBootstrappingInstallationRequested)
                        If aObjectDefinition Is Nothing And Not Me.IsBootstrappingInstallationRequested Then
                            CoreMessageHandler(message:="Object is missing in object repository", argument:=opname, procedure:="Session.validateOTDBAccessLevel", messagetype:=otCoreMessageType.InternalError)
                            result = result And False
                        Else
                            '** get the ObjectDefinition's effective permissions
                            result = result And CType(aObjectDefinition, ormObjectDefinition).GetEffectivePermission(user:=_OTDBUser, domainid:=domainid, transactionname:=anTransactionname)
                            '** put it in cache
                            If _ObjectPermissionCache.ContainsKey(opname.ToUpper) Then
                                _ObjectPermissionCache.Remove(opname.ToUpper)
                            Else
                                _ObjectPermissionCache.Add(key:=opname.ToUpper, value:=result)
                            End If
                        End If

                    End If
                End If


            Next

            Return result
        End Function

        ''' <summary>
        ''' request the user access to OnTrack Database (running or not) - if necessary start a Login with Loginwindow. Check on user rights.
        ''' </summary>
        ''' <param name="accessRequest">needed User right</param>
        ''' <param name="username">default username to use</param>
        ''' <param name="password">default password to use</param>
        ''' <param name="forceLogin">force a Login window in any case</param>
        ''' <param name="loginOnDemand">do a Login window and reconnect if right is not necessary</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RequestUserAccess(accessRequest As otAccessRight, _
                                            Optional ByRef username As String = Nothing, _
                                            Optional ByRef password As String = Nothing, _
                                            Optional ByRef domainid As String = Nothing, _
                                            Optional ByRef [objecttransactions] As String() = Nothing, _
                                            Optional loginOnDisConnected As Boolean = False, _
                                            Optional loginOnFailed As Boolean = False, _
                                            Optional messagetext As String = Nothing) As Boolean

            Dim userValidation As UserValidation
            userValidation.ValidEntry = False


            '****
            '**** rights during bootstrapping
            '****


            If Me.IsBootstrappingInstallationRequested Then

                Return True
                '****
                '**** no connection -> login
                '****

            ElseIf Not Me.IsRunning Then

                ''' todo: check if validation is obtainable -> user table there or something
                ''' 

                '*** OTDBUsername supplied

                If loginOnDisConnected And accessRequest <> ConstDefaultAccessRight Then
                    If Me.OTdbUser IsNot Nothing AndAlso Me.OTdbUser.IsAnonymous Then
                        Me.UILogin.EnableUsername = True
                        Me.UILogin.Username = Nothing
                        Me.UILogin.Password = Nothing
                    End If
                    'LoginWindow
                    Me.UILogin.Configset = ot.CurrentConfigSetName
                    Me.UILogin.PossibleConfigSets = ot.ConfigSetNamesToSelect
                    Me.UILogin.EnableChangeConfigSet = True
                    If Not String.IsNullOrWhiteSpace(messagetext) Then Me.UILogin.Messagetext = messagetext
                    If String.IsNullOrEmpty(domainid) Then
                        domainid = ConstGlobalDomain
                        Me.UILogin.Domain = domainid
                        Me.UILogin.EnableDomain = True
                    Else
                        '** enable domainchange
                        Me.UILogin.Domain = domainid
                        Me.UILogin.EnableDomain = False
                    End If

                    'Me.UILogin.Session = Me

                    Me.UILogin.Accessright = accessRequest
                    Me.UILogin.enableAccess = True
                    Me.UILogin.PossibleRights = AccessRightProperty.GetHigherAccessRequests(accessrequest:=accessRequest)

                    Me.UILogin.Show()

                    If Not Me.UILogin.Ok Then
                        CoreMessageHandler(message:="login aborted by user", procedure:="Session.verifyuserAccess", messagetype:=otCoreMessageType.ApplicationInfo)
                        Return False
                    Else
                        username = Me.UILogin.Username
                        password = Me.UILogin.Password
                        accessRequest = Me.UILogin.Accessright
                        '** change the currentConfigSet
                        If UILogin.Configset <> _UseConfigSetName Then
                            _UseConfigSetName = UILogin.Configset
                        End If
                        If Me.CurrentDomainID <> Me.UILogin.Domain Then
                            SwitchToDomain(Me.UILogin.Domain)
                        End If
                        '* validate
                        userValidation = _primaryDBDriver.GetUserValidation(username)
                    End If

                    ' just check the provided username
                ElseIf Not String.IsNullOrWhiteSpace(username) Then
                    If Not String.IsNullOrEmpty(domainid) Then domainid = ConstGlobalDomain
                    userValidation = _primaryDBDriver.GetUserValidation(username)
                    If userValidation.ValidEntry AndAlso password = String.Empty Then
                        password = userValidation.Password
                    End If
                    '* no username but default accessrequest then look for the anonymous user
                ElseIf accessRequest = ConstDefaultAccessRight Then
                    If String.IsNullOrEmpty(domainid) Then domainid = ConstGlobalDomain
                    userValidation = _primaryDBDriver.GetUserValidation(username:=String.Empty, selectAnonymous:=True)
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

                    '*** reset
                    Call ShutDown()
                    Return False
                Else
                    '**** Check Password
                    '****
                    If String.IsNullOrEmpty(domainid) Then domainid = ConstGlobalDomain
                    If _primaryDBDriver.ValidateUser(accessRequest:=accessRequest, username:=username, password:=password, domainid:=domainid) Then
                        Call CoreMessageHandler(procedure:="Session.verifyUserAccess", break:=False, message:="User verified successfully", _
                                                argument:=username, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationInfo)
                    Else
                        Call CoreMessageHandler(procedure:="Session.verifyUserAccess", break:=False, message:="User not verified successfully", _
                                                argument:=username, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationError)
                        Return False
                    End If

                End If

                '****
                '**** CONNECTION on CONNECTED !
            Else
                '** stay in the current domain 
                If String.IsNullOrEmpty(domainid) Then domainid = ot.CurrentSession.CurrentDomainID

                '** validate the current user with the request if it is failing then
                '** do check again
                If Me.ValidateAccessRights(accessrequest:=accessRequest, domainid:=domainid, objecttransactions:=[objecttransactions]) Then
                    Return True
                    '* change the current user if anonymous
                    '*
                ElseIf loginOnFailed And OTdbUser.IsAnonymous Then
                    '** check if new OTDBUsername is valid
                    'LoginWindow
                    ' enable domain
                    If Not String.IsNullOrEmpty(domainid) Then
                        Me.UILogin.Domain = domainid
                        Me.UILogin.EnableDomain = False
                    Else
                        '** enable domain change
                        domainid = ConstGlobalDomain
                        Me.UILogin.Domain = domainid
                        Me.UILogin.PossibleDomains = Domain.All.Select(Function(x) x.ID).ToList
                        Me.UILogin.EnableDomain = True
                    End If

                    Me.UILogin.enableAccess = True
                    Me.UILogin.PossibleRights = AccessRightProperty.GetHigherAccessRequests(accessRequest)
                    Me.UILogin.Configset = ot.CurrentConfigSetName
                    Me.UILogin.EnableChangeConfigSet = False
                    Me.UILogin.Accessright = accessRequest
                    If Not String.IsNullOrWhiteSpace(messagetext) Then
                        Me.UILogin.Messagetext = messagetext
                    Else
                        Me.UILogin.Messagetext = "<html><strong>Welcome !</strong><br />Please change to a valid user and password for the needed access right.</html>"
                    End If
                    Me.UILogin.EnableUsername = True
                    Me.UILogin.Username = Nothing
                    Me.UILogin.Password = Nothing
                    'Me.UILogin.Session = Me

                    Me.UILogin.Show()

                    If Not Me.UILogin.Ok Then
                        Call CoreMessageHandler(procedure:="Session.verifyUserAccess", break:=False, _
                                                message:="login aborted by user - fall back to user " & username, _
                                                argument:=username, messagetype:=otCoreMessageType.ApplicationInfo)
                        Return False
                    End If


                    username = UILogin.Username
                    password = UILogin.Password

                    userValidation = _primaryDBDriver.GetUserValidation(username)

                    '* check validation -> relogin on connected -> EventHandler ?!
                    '* or abortion of the login window
                    If _primaryDBDriver.ValidateUser(accessRequest:=accessRequest, username:=username, password:=password, domainid:=domainid) Then
                        Call CoreMessageHandler(procedure:="Session.verifyUserAccess", break:=False, _
                                                message:="User change verified successfully on domain '" & domainid & "'", _
                                                argument:=username, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationInfo)
                        If Me.CurrentDomainID <> Me.UILogin.Domain Then
                            SwitchToDomain(Me.UILogin.Domain)
                        End If

                        '* set the new access level
                        _AccessLevel = accessRequest
                        Dim anOTDBUser As User = User.Retrieve(username:=username)
                        If anOTDBUser IsNot Nothing Then
                            _OTDBUser = anOTDBUser
                            Me.UserChangedEvent(_OTDBUser)
                        Else
                            CoreMessageHandler(message:="user definition cannot be loaded", messagetype:=otCoreMessageType.InternalError, _
                                               argument:=username, noOtdbAvailable:=False, procedure:="Session.verifyUserAccess")
                            username = _OTDBUser.Username
                            password = _OTDBUser.Password
                            Return False
                        End If

                    Else
                        '** fall back
                        username = _OTDBUser.Username
                        password = _OTDBUser.Password

                        Call CoreMessageHandler(procedure:="Session.verifyUserAccess", break:=False, message:="User couldnot be verified - fallback to user " & username, _
                                                argument:=username, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationError, showmsgbox:=True)


                        Return False
                    End If


                    '* the current access level is not for this request
                    '*
                ElseIf loginOnFailed And Not Me.OTdbUser.IsAnonymous Then
                    '** check if new OTDBUsername is valid
                    'LoginWindow
                    Me.UILogin.Domain = domainid
                    Me.UILogin.EnableDomain = False
                    Me.UILogin.PossibleDomains = New List(Of String)
                    Me.UILogin.enableAccess = True
                    Me.UILogin.PossibleRights = AccessRightProperty.GetHigherAccessRequests(accessRequest)
                    Me.UILogin.Configset = ot.CurrentConfigSetName
                    Me.UILogin.EnableChangeConfigSet = False
                    Me.UILogin.Accessright = accessRequest
                    If messagetext <> String.Empty Then
                        Me.UILogin.Messagetext = messagetext
                    Else
                        Me.UILogin.Messagetext = "<html><strong>Attention !</strong><br />Please confirm by your password to obtain the access right.</html>"
                    End If
                    Me.UILogin.EnableUsername = False
                    Me.UILogin.Username = Me.OTdbUser.Username
                    Me.UILogin.Password = password
                    'Me.UILogin.Session = Me

                    Me.UILogin.Show()
                    If Not Me.UILogin.Ok Then
                        Call CoreMessageHandler(procedure:="Session.verifyUserAccess", break:=False, _
                                                message:="login aborted by user - fall back to user " & username, _
                                                argument:=username, messagetype:=otCoreMessageType.ApplicationInfo)
                        Return False
                    End If
                    ' return input
                    username = UILogin.Username
                    password = UILogin.Password
                    If Me.CurrentDomainID <> Me.UILogin.Domain Then
                        SwitchToDomain(Me.UILogin.Domain)
                    End If
                    If Me.CurrentDomainID <> Me.UILogin.Domain Then
                        SwitchToDomain(Me.UILogin.Domain)
                    End If
                    userValidation = _primaryDBDriver.GetUserValidation(username)
                    '* check password
                    If _primaryDBDriver.ValidateUser(accessRequest:=accessRequest, username:=username, password:=password, domainid:=domainid) Then
                        '** not again
                        'Call CoreMessageHandler(subname:="Session.verifyUserAccess", break:=False, message:="User change verified successfully", _
                        '                        arg1:=username, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationInfo)
                        '* set the new access level
                        _AccessLevel = accessRequest
                    Else
                        '** fallback
                        username = _OTDBUser.Username
                        password = _OTDBUser.Password
                        Call CoreMessageHandler(procedure:="Session.verifyUserAccess", break:=False, message:="User couldnot be verified - fallback to user " & username, _
                                                argument:=username, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationError, showmsgbox:=True)
                        Return False
                    End If

                    '*** just check the provided username
                ElseIf Not String.IsNullOrWhiteSpace(username) AndAlso Not String.IsNullOrWhiteSpace(password) Then
                    userValidation = _primaryDBDriver.GetUserValidation(username)
                End If
            End If

            '**** Check the UserValidation Rights

            '* exclude user
            If userValidation.HasNoRights Then
                Call CoreMessageHandler(showmsgbox:=True, _
                                        message:=" Access to OnTrack Database is prohibited - User has no rights", _
                                        break:=False, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationError)

                '*** shutdown 
                If Not Me.IsRunning Then
                    ShutDown()
                Else
                    ' Disconnect() -> Do not ! fall back to old user
                End If

                Return False
                '* check on the rights
            ElseIf Not userValidation.HasAlterSchemaRights And accessRequest = otAccessRight.AlterSchema Then
                Call CoreMessageHandler(showmsgbox:=True, _
                                        message:=" Access to OnTrack Database is prohibited - User has no alter schema rights", _
                                        break:=False, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationError)

                '*** shutdown 
                If Not Me.IsRunning Then
                    ShutDown()
                Else
                    ' Disconnect() -> Do not ! fall back to old user
                End If
                Return False
            ElseIf Not userValidation.HasUpdateRights And accessRequest = otAccessRight.ReadUpdateData Then
                Call CoreMessageHandler(showmsgbox:=True, _
                                        message:=" Access to OnTrack Database is prohibited - User has no update rights", _
                                        break:=False, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationError)

                '*** shutdown 
                If Not Me.IsRunning Then
                    ShutDown()
                Else
                    ' Disconnect() -> Do not ! fall back to old user
                End If
                Return False
            ElseIf Not userValidation.HasReadRights And accessRequest = otAccessRight.[ReadOnly] Then
                Call CoreMessageHandler(showmsgbox:=True, _
                                        message:=" Access to OnTrack Database is prohibited - User has no read rights", _
                                        break:=False, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationError)

                '*** shutdown 
                If Not Me.IsRunning Then
                    ShutDown()
                Else
                    ' Disconnect() -> Do not ! fall back to old user
                End If
                Return False
            End If
            '*** return true

            Return True

        End Function


        ''' <summary>
        ''' Initiate/Start a new Session or do nothing if a Session is already initiated
        ''' </summary>
        ''' <param name="OTDBUsername"></param>
        ''' <param name="OTDBPasswort"></param>
        ''' <param name="AccessRequest"></param>
        ''' <returns>True if successfull False else</returns>
        ''' <remarks></remarks>
        Public Function StartUp(AccessRequest As otAccessRight, _
                                Optional useconfigsetname As String = Nothing, _
                            Optional domainID As String = Nothing, _
                            Optional OTDBUsername As String = Nothing, _
                            Optional OTDBPassword As String = Nothing, _
                            Optional installIfNecessary As Boolean? = Nothing, _
                            Optional ByVal messagetext As String = Nothing) As Boolean
            Dim aValue As Object
            Dim result As Boolean

            Try
                If Me.IsRunning OrElse Me.IsStartingUp Then
                    CoreMessageHandler(message:="Session is already running or starting up - further startups not possible", argument:=Me.SessionID, procedure:="Session.Startup", messagetype:=otCoreMessageType.InternalInfo)
                    Return False
                End If

                '** default is install on startup
                If Not installIfNecessary.HasValue Then installIfNecessary = True
                If String.IsNullOrEmpty(domainID) Then domainID = _CurrentDomainID

                '** set statup
                Me.IsStartingUp = True

                ' set the config setname
                If Not String.IsNullOrWhiteSpace(useconfigsetname) AndAlso ot.HasConfigSetName(useconfigsetname, ComplexPropertyStore.Sequence.Primary) Then
                    _UseConfigSetName = useconfigsetname
                End If
                ' set the application ID from the current config set
                If ot.HasConfigProperty(ConstCPNSetupID, configsetname:=ot.CurrentConfigSetName) Then
                    _setupID = ot.GetConfigProperty(ConstCPNSetupID)
                End If

                If String.IsNullOrWhiteSpace(_setupID) Then
                    _setupID = ConstDefaultSetupID
                End If

                Call CoreMessageHandler(procedure:="Session.Startup", message:="setup id for the session set to '" & _setupID & "'", _
                                           argument:=_SessionID, messagetype:=otCoreMessageType.InternalInfo)

                '** lazy initialize
                If Not Me.IsInitialized AndAlso Not Me.Initialize() Then
                    Call CoreMessageHandler(procedure:="Session.Startup", message:="failed to initialize session", _
                                            argument:=Me.SessionID, messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If

                '* take the OTDBDriver
                If _primaryDBDriver Is Nothing Then
                    CoreMessageHandler(message:="primary database driver is not set", messagetype:=otCoreMessageType.InternalError, _
                                       procedure:="Session.Startup")
                    '** reset
                    If IsBootstrappingInstallationRequested Then Me.RequestEndofBootstrap()
                    Me.IsRunning = False
                    Me.IsStartingUp = False
                    Return False
                End If

                '** set domain without switching since it is not running
                '**
                If String.IsNullOrEmpty(domainID) Then
                    If ot.HasConfigSetProperty(constCPNDefaultDomainid) Then
                        domainID = CStr(ot.GetConfigProperty(constCPNDefaultDomainid)).ToUpper
                        If Not String.IsNullOrEmpty(domainID) Then
                            Me.CurrentDomainID = domainID
                        Else
                            Me.CurrentDomainID = ConstGlobalDomain
                        End If
                    ElseIf ot.HasConfigSetProperty(constCPNDefaultDomainid, configsetname:=ConstGlobalConfigSetName) Then
                        domainID = CStr(ot.GetConfigProperty(constCPNDefaultDomainid, configsetname:=ConstGlobalConfigSetName)).ToUpper
                        If Not String.IsNullOrEmpty(domainID) Then
                            Me.CurrentDomainID = domainID
                        Else
                            Me.CurrentDomainID = ConstGlobalDomain
                        End If

                    Else
                        Me.CurrentDomainID = ConstGlobalDomain ' set the current domain (_domainID)
                    End If
                End If

                '*** get the Schema Version
                aValue = _primaryDBDriver.GetDBParameter(ConstPNBSchemaVersion, setupID:=_setupID, silent:=True)
                If aValue Is Nothing OrElse Not IsNumeric(aValue) Then
                    result = _primaryDBDriver.VerifyOnTrackDatabase(install:=installIfNecessary, modules:=ot.InstalledModules, verifySchema:=False)
                ElseIf ot.SchemaVersion < Convert.ToUInt64(aValue) Then
                    CoreMessageHandler(showmsgbox:=True, message:="Verifying the OnTrack Database failed. The Tooling schema version of # " & ot.SchemaVersion & _
                                       " is less than the database schema version of #" & aValue & " - Session could not start up", _
                                       messagetype:=otCoreMessageType.InternalError, procedure:="Session.Startup")
                    '** reset
                    If IsBootstrappingInstallationRequested Then Me.RequestEndofBootstrap()
                    Me.IsRunning = False
                    Me.IsStartingUp = False
                    Return False
                ElseIf ot.SchemaVersion > Convert.ToUInt64(aValue) Then
                    result = _primaryDBDriver.VerifyOnTrackDatabase(install:=installIfNecessary, modules:=ot.InstalledModules, verifySchema:=False)
                ElseIf Not _primaryDBDriver.VerifyOnTrackDatabase(install:=False, modules:={ot.ConstModuleRepository}, verifySchema:=False) Then
                    ''' if repository failed check and install all modules again
                    ''' 
                    result = _primaryDBDriver.VerifyOnTrackDatabase(install:=installIfNecessary, modules:=ot.InstalledModules, verifySchema:=False)
                Else
                    '** check also the bootstrap version
                    aValue = _primaryDBDriver.GetDBParameter(ConstPNBootStrapSchemaChecksum, setupID:=_setupID, silent:=True)
                    If aValue Is Nothing OrElse Not IsNumeric(aValue) OrElse ot.GetBootStrapSchemaChecksum <> Convert.ToUInt64(aValue) Then
                        result = _primaryDBDriver.VerifyOnTrackDatabase(install:=installIfNecessary, modules:=ot.InstalledModules, verifySchema:=False)
                    Else
                        result = True
                    End If
                End If


                ''' the starting up aborted
                ''' 
                If Not Me.IsStartingUp Then
                    CoreMessageHandler(message:="Startup of Session was aborted", _
                                       messagetype:=otCoreMessageType.InternalInfo, procedure:="Session.Startup")
                    '** reset
                    If IsBootstrappingInstallationRequested Then Me.RequestEndofBootstrap()
                    Me.IsRunning = False
                    Me.IsStartingUp = False
                    Return False
                End If

                ''' the installation failed
                If Not result And installIfNecessary Then
                    CoreMessageHandler(showmsgbox:=True, message:="Verifying and Installing the OnTrack Database failed - Session could not start up", _
                                       messagetype:=otCoreMessageType.InternalError, procedure:="Session.Startup")
                    '** reset
                    If IsBootstrappingInstallationRequested Then Me.RequestEndofBootstrap()
                    Me.IsRunning = False
                    Me.IsStartingUp = False
                    Return False
                ElseIf Not installIfNecessary And Not result Then
                    CoreMessageHandler(showmsgbox:=True, message:="Verifying  the OnTrack Database failed - Session will be started anyway on demand", _
                                                      messagetype:=otCoreMessageType.InternalWarning, procedure:="Session.Startup")
                End If

                ''' default messagetext
                If messagetext Is Nothing Then
                    messagetext = "Please provide valid username and password to logon."
                End If
                '** request access
                If RequestUserAccess(accessRequest:=AccessRequest, _
                                     username:=OTDBUsername, _
                                    password:=OTDBPassword, _
                                    domainid:=domainID, _
                                    loginOnDisConnected:=True, _
                                    loginOnFailed:=True, _
                                    messagetext:=messagetext.Clone) Then
                    '** the starting up aborted
                    If Not Me.IsStartingUp Then
                        CoreMessageHandler(message:="Startup of Session was aborted", _
                                           messagetype:=otCoreMessageType.InternalInfo, procedure:="Session.Startup")
                        '** reset
                        If IsBootstrappingInstallationRequested Then Me.RequestEndofBootstrap()
                        Me.IsRunning = False
                        Me.IsStartingUp = False
                        Return False
                    End If

                    ''' Connect - if we return we are not starting up again we have started
                    '''
                    If Not _primaryConnection.Connect(FORCE:=True, _
                                                      access:=AccessRequest, _
                                                      domainid:=domainID, _
                                                      OTDBUsername:=OTDBUsername, _
                                                      OTDBPassword:=OTDBPassword, _
                                                      doLogin:=True) Then

                        ''' start up message
                        CoreMessageHandler(message:="Could not connect to OnTrack Database though primary connection", argument:=_primaryConnection.ID, _
                                                      messagetype:=otCoreMessageType.InternalError, procedure:="Session.Startup")
                        '** reset
                        If IsBootstrappingInstallationRequested Then Me.RequestEndofBootstrap()
                        Me.IsRunning = False
                        Me.IsStartingUp = False
                        Return False
                    End If

                    '** Initialize through events
                Else
                    CoreMessageHandler(message:="user could not be verified - abort to start up a session", messagetype:=otCoreMessageType.InternalInfo, argument:=OTDBUsername, _
                                       procedure:="Session.Startup")
                    '** reset
                    If IsBootstrappingInstallationRequested Then Me.RequestEndofBootstrap()
                    Me.IsRunning = False
                    Me.IsStartingUp = False
                    Return False
                End If

                Return True

            Catch ex As ormNoConnectionException
                Return False
            Catch ex As ormException
                CoreMessageHandler(exception:=ex, procedure:="Session.Startup")
                Return False
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="Session.Startup")
                Return False

            End Try

        End Function
        ''' <summary>
        ''' Initiate closeDown this Session and the Connection to OnTrack Database
        ''' </summary>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ShutDown(Optional force As Boolean = False) As Boolean

            '***
            Call CoreMessageHandler(showmsgbox:=False, message:="Session Shutdown", argument:=_SessionID, _
                                    break:=True, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationInfo, _
                                    procedure:="Session.ShutDown")

            '*** shut down the primary connection
            If Not _primaryConnection Is Nothing AndAlso _primaryConnection.IsConnected Then
                _primaryConnection.Disconnect()
                ' Call Me.ShutDownSessionEnviorment()  -> Event Driven
            Else
                Call Me.ShutDownSessionEnviorment()
            End If

            'reset
            _IsRunning = False
            _CurrentDomain = Nothing
            _CurrentDomainID = String.Empty
            _CurrentWorkspaceID = String.Empty
            _AccessLevel = 0
            _Username = String.Empty
            _IsInitialized = False
            For Each anObjectstore In _DomainRepositories.Values
                'anObjectstore.reset()
            Next
            _DomainRepositories.Clear()
            _errorLog.Clear()
            Return True
        End Function

        ''' <summary>
        ''' sets the current Domain
        ''' </summary>
        ''' <param name="newDomainID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SwitchToDomain(newDomainID As String) As Boolean
            Dim newDomain As Domain
            Dim aRepository As ormObjectRepository = _DomainRepositories.First.Value
            Try
                '* return if not running -> me.running might be false but connection is there since
                '* we are coming here during startup
                If _primaryDBDriver Is Nothing OrElse _primaryConnection Is Nothing _
                OrElse (_primaryConnection IsNot Nothing And Not _primaryConnection.IsConnected) Then
                    _CurrentDomainID = newDomainID
                    _loadDomainReqeusted = True
                    Return True
                End If

                '* no change or domain is set but not loaded
                If (Not String.IsNullOrWhiteSpace(_CurrentDomainID) AndAlso newDomainID = _CurrentDomainID AndAlso Not _loadDomainReqeusted) Then
                    Return True
                End If

                ' repository for constglobaldomain is create in session initalize
                'If Not _DomainObjectsDir.ContainsKey(key:=ConstGlobalDomain) Then
                '    Dim aStore = New ObjectRepository(Me)
                '    _DomainObjectsDir.Add(key:=ConstGlobalDomain, value:=aStore)
                '    aStore.RegisterCache(_ObjectCache)
                'End If

                If newDomainID <> ConstGlobalDomain Then
                    aRepository = _DomainRepositories.Item(key:=ConstGlobalDomain)
                    If Not aRepository.IsInitialized Then
                        ''' we need a initialized repository for global domain before we can switch
                        ''' to a different custom domain
                        ''' initialization is done via event domainchanged
                        ''' best ist to run recursive switch to domain
                        'Me.SwitchToDomain(ConstGlobalDomain)
                    End If
                End If

                '' set the session status for domain switching / changing
                '' set it here since Domain.retieve will access the Repository and fall back to Global might necessary
                ''
                Me.IsDomainSwitching = True

                'get the new domain object direct from the factory to prevent any startup of enviorment
                Dim aKey As New ormDatabaseKey({newDomainID})
                newDomain = TryCast(aRepository.GetProvider(objectid:=Domain.ConstObjectID).Retrieve(primarykey:=aKey, type:=GetType(Domain), runtimeOnly:=Me.IsBootstrappingInstallationRequested), Domain)
                Dim saveDomain As Boolean = False

                '** check on bootstrapping 
                If newDomain Is Nothing And Not Me.IsBootstrappingInstallationRequested Then
                    CoreMessageHandler(message:="domain does not exist - falling back to global domain", _
                                       argument:=newDomainID, procedure:="Session.SetDomain", messagetype:=otCoreMessageType.ApplicationError)
                    newDomain = TryCast(aRepository.GetProvider(objectid:=Domain.ConstObjectID).Retrieve(primarykey:=New ormDatabaseKey({ConstGlobalDomain}), type:=GetType(Domain), runtimeOnly:=Me.IsBootstrappingInstallationRequested), Domain)
                    If newDomain Is Nothing Then
                        CoreMessageHandler(message:="global domain does not exist", argument:=ConstGlobalDomain, procedure:="Session.SetDomain", messagetype:=otCoreMessageType.InternalError)
                        Return False
                    End If

                ElseIf newDomain Is Nothing And Me.IsBootstrappingInstallationRequested Then
                    '** bootstrapping database install
                    newDomainID = ConstGlobalDomain
                    'newDomain = New Domain()
                    'newDomain.Create(domainID:=newDomainID)
                    Me._CurrentDomain = Nothing
                    Me._CurrentDomainID = newDomainID
                    _loadDomainReqeusted = True
                    RaiseEvent OnDomainChanging(Me, New SessionEventArgs(Me, Nothing))
                    Me.IsDomainSwitching = False
                    Return True
                Else

                    '** we have a domain
                    newDomain.RegisterSession(Me)

                    '** add new Repository
                    If Not _DomainRepositories.ContainsKey(key:=newDomainID) Then
                        Dim anewRepository = New ormObjectRepository(Me, newDomainID)
                        If Not _DataObjectCaches.ContainsKey(key:=newDomainID) Then
                            _DataObjectCaches.Add(key:=newDomainID, value:=New ObjectCacheManager(Me, newDomainID))
                        End If
                        _DomainRepositories.Add(key:=newDomainID, value:=anewRepository)
                        anewRepository.RegisterCache(_DataObjectCaches.Item(key:=newDomainID))
                        _DataObjectCaches.Item(key:=newDomainID).Start()
                    End If

                    '* reset cache
                    _ObjectPermissionCache.Clear()
                    _ValueListCache.Clear()

                    '** raise event
                    RaiseEvent OnDomainChanging(Me, New SessionEventArgs(Me, newDomain))

                    '*** read the Domain Settings
                    '***

                    If newDomain.HasSetting(id:=ConstCPDependencySynchroMinOverlap) Then
                        Me.DependencySynchroMinOverlap = newDomain.GetSetting(id:=ConstCPDependencySynchroMinOverlap).value
                    Else
                        Me.DependencySynchroMinOverlap = 7
                    End If

                    If newDomain.HasSetting(id:=ConstCPDefaultWorkspace) Then
                        Me.DefaultWorkspaceID = newDomain.GetSetting(id:=ConstCPDefaultWorkspace).value
                        _CurrentWorkspaceID = _DefaultWorkspace
                    Else
                        Me.DefaultWorkspaceID = String.Empty
                    End If

                    If newDomain.HasSetting(id:=ConstCPDefaultCalendarName) Then
                        Me.DefaultCalendarName = newDomain.GetSetting(id:=ConstCPDefaultCalendarName).value
                    Else
                        Me.DefaultCalendarName = "default"
                    End If

                    If newDomain.HasSetting(id:=ConstCPDefaultTodayLatency) Then
                        Me.TodayLatency = newDomain.GetSetting(id:=ConstCPDefaultTodayLatency).value
                    Else
                        Me.TodayLatency = -14
                    End If

                    If newDomain.HasSetting(id:=ConstCDefaultScheduleTypeID) Then
                        Me.DefaultScheduleTypeID = newDomain.GetSetting(id:=ConstCDefaultScheduleTypeID).value
                    Else
                        Me.DefaultScheduleTypeID = String.Empty

                    End If

                    If newDomain.HasSetting(id:=ConstCPDefaultDeliverableTypeID) Then
                        Me.DefaultDeliverableTypeID = newDomain.GetSetting(id:=ConstCPDefaultDeliverableTypeID).value
                    Else
                        Me.DefaultDeliverableTypeID = String.Empty
                    End If

                    If newDomain.HasSetting(id:=ConstCPAutoPublishTarget) Then
                        Me.AutoPublishTarget = newDomain.GetSetting(id:=ConstCPAutoPublishTarget).value
                    Else
                        Me.AutoPublishTarget = False
                    End If

                    If newDomain.HasSetting(id:=ConstCPDeliverableOnCloningCloneAlso) Then
                        Me.DeliverableOnCloningCloneAlso = Core.DataType.ToArray(newDomain.GetSetting(id:=ConstCPDeliverableOnCloningCloneAlso).value)
                    Else
                        Me.DeliverableOnCloningCloneAlso = {}
                    End If

                    If newDomain.HasSetting(id:=ConstCPDeliverableUniqueEntries) Then
                        Me.DeliverableUniqueEntries = Core.DataType.ToArray(newDomain.GetSetting(id:=ConstCPDeliverableUniqueEntries).value)
                    Else
                        Me.DeliverableUniqueEntries = {}
                    End If

                    If newDomain.HasSetting(id:=ConstCPDeliverableOnCloningResetEntries) Then
                        Me.DeliverableOnCloningResetEntries = Core.DataType.ToArray(newDomain.GetSetting(id:=ConstCPDeliverableOnCloningResetEntries).value)
                    Else
                        Me.DeliverableOnCloningResetEntries = {}
                    End If
                End If


                Me._CurrentDomain = newDomain
                Me._CurrentDomainID = newDomainID
                _loadDomainReqeusted = False

                ''' rause the domain changed event
                RaiseEvent OnDomainChanged(Me, New SessionEventArgs(Me))
                CoreMessageHandler(message:="Domain switched to '" & newDomainID & "' - " & newDomain.Description, _
                                    procedure:="Session.SwitchToDomain", dataobject:=newDomain, messagetype:=otCoreMessageType.ApplicationInfo)
                Me.IsDomainSwitching = False
                Return True

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="Session.SwitchToDomain")
                _loadDomainReqeusted = False
                Me.IsDomainSwitching = False
                Return False
            End Try

        End Function
        ''' <summary>
        ''' Initialize and set all Parameters
        ''' </summary>
        ''' <param name="FORCE"></param>
        ''' <returns>true if successful</returns>
        ''' <remarks></remarks>
        Private Function StartUpSessionEnviorment(Optional ByVal force As Boolean = False, Optional domainid As String = Nothing) As Boolean
            Dim aValue As Object

            Try

                If Not IsRunning Or force Then


                    '** start the Agent
                    If Not _logagent Is Nothing Then
                        aValue = ot.GetConfigProperty(constCPNUseLogAgent)
                        If CBool(aValue) Then
                            _logagent.Start()
                            '***
                            Call CoreMessageHandler(showmsgbox:=False, message:=" LogAgent for Session started ", argument:=_SessionID, _
                                                    break:=True, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationInfo, _
                                                    procedure:="Session.startupSesssionEnviorment")
                        Else
                            '***
                            Call CoreMessageHandler(showmsgbox:=False, message:=" LogAgent for Session not used by configuration ", argument:=_SessionID, _
                                                    break:=True, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationInfo, _
                                                    procedure:="Session.startupSesssionEnviorment")
                        End If

                    End If
                    '** check driver
                    If _primaryDBDriver Is Nothing Or Not IsInitialized Then
                        '***
                        Call CoreMessageHandler(showmsgbox:=False, message:=" Session cannot initiated no DBDriver set ", _
                                                break:=True, noOtdbAvailable:=True, messagetype:=otCoreMessageType.InternalError, _
                                                procedure:="Session.startupSesssionEnviorment")
                        Me.IsStartingUp = False
                        IsRunning = False
                        Return False
                    End If

                    '''
                    ''' load domain before retrieving any data
                    ''' 
                    If String.IsNullOrEmpty(domainid) Then domainid = Me.CurrentDomainID
                    '* set it here that we are really loading in SetDomain and not only 
                    '* assigning _DomainID (if no connection is available)
                    If SwitchToDomain(newDomainID:=domainid) Then
                        Call CoreMessageHandler(message:="Session Domain set to '" & domainid & "' - " & CurrentSession.CurrentDomain.Description, _
                                                messagetype:=otCoreMessageType.ApplicationInfo, _
                                                procedure:="Session.startupSesssionEnviorment")
                    End If

                    '''
                    ''' load the user
                    ''' 
                    _Username = _primaryDBDriver.CurrentConnection.Dbuser
                    _OTDBUser = User.Retrieve(username:=_primaryDBDriver.CurrentConnection.Dbuser)
                    If Not _OTDBUser Is Nothing AndAlso _OTDBUser.IsLoaded Then
                        _Username = _OTDBUser.Username
                        _AccessLevel = _OTDBUser.AccessRight
                    Else
                        Call CoreMessageHandler(showmsgbox:=True, message:=" Session could not initiate - user could not be retrieved from database", _
                                               break:=False, argument:=_primaryDBDriver.CurrentConnection.Dbuser, noOtdbAvailable:=True, messagetype:=otCoreMessageType.InternalError, _
                                               procedure:="Session.startupSesssionEnviorment")
                        IsRunning = False
                        Me.IsStartingUp = False
                        Return False
                    End If


                    '** the starting up aborted
                    If Not Me.IsStartingUp Then
                        CoreMessageHandler(message:="Startup of Session was aborted", _
                                           messagetype:=otCoreMessageType.InternalInfo, procedure:="Session.StartupSessionEnviorment")
                        '** reset
                        If IsBootstrappingInstallationRequested Then Me.RequestEndofBootstrap()
                        Me.IsRunning = False
                        Me.IsStartingUp = False
                        Return False
                    End If

                    '*** get the Schema Version
                    aValue = _primaryDBDriver.GetDBParameter(ConstPNBootStrapSchemaChecksum, silent:=True)
                    If aValue Is Nothing OrElse Not IsNumeric(aValue) Then
                        _primaryDBDriver.VerifyOnTrackDatabase()
                    ElseIf ot.GetBootStrapSchemaChecksum <> Convert.ToUInt64(aValue) Then
                        _primaryDBDriver.VerifyOnTrackDatabase()
                    End If
                    '** the starting up aborted
                    If Not Me.IsStartingUp Then
                        CoreMessageHandler(message:="Startup of Session was aborted", _
                                           messagetype:=otCoreMessageType.InternalInfo, procedure:="Session.StartupSessionEnviorment")
                        '** reset
                        If IsBootstrappingInstallationRequested Then Me.RequestEndofBootstrap()
                        Me.IsRunning = False
                        Me.IsStartingUp = False
                        Return False
                    End If
                    '*** Initialize the Repository
                    If Me.Objects.GetType().IsAssignableFrom(GetType(ormObjectRepository)) Then TryCast(Me.Objects, ormObjectRepository).Initialize(force:=False)
                    '*** set started
                    Me.IsStartingUp = False
                    IsRunning = True
                    '*** we are started
                    RaiseEvent OnStarted(Me, New SessionEventArgs(Me))

                End If
                Return IsRunning

            Catch ex As ormNoConnectionException
                Me.IsRunning = False
                Me.IsStartingUp = False
                Return False

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="Session.StartupSessionEnviorment")
                Me.IsRunning = False
                Me.IsStartingUp = False
                Return False
            End Try

        End Function

        ''' <summary>
        ''' reset the Session or close it down
        ''' </summary>
        ''' <param name="FORCE">true if to do it even not initialized</param>
        ''' <returns>True if successfully reseted</returns>
        ''' <remarks></remarks>
        Private Function ShutDownSessionEnviorment(Optional ByVal force As Boolean = False) As Boolean
            Dim aValue As Object

            If Not Me.IsInitialized OrElse Not Me.IsRunning Then
                Return False
            End If

            '*** we are ending
            RaiseEvent OnEnding(Me, New SessionEventArgs(Me))


            '** stop the Agent
            If Not _logagent Is Nothing Then
                _logagent.Stop()
                aValue = ot.GetConfigProperty(constCPNUseLogAgent)
                If CBool(aValue) Then
                    '***
                    Call CoreMessageHandler(showmsgbox:=False, message:="LogAgent for Session stopped ", argument:=_SessionID, _
                                            break:=True, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationInfo, _
                                            procedure:="Session.shutdownSessionEviorment")
                Else
                    '***
                    Call CoreMessageHandler(showmsgbox:=False, message:=" LogAgent for Session not used by configuration but stopped anyway ", argument:=_SessionID, _
                                            break:=True, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationInfo, _
                                            procedure:="Session.startupSesssionEnviorment")
                End If

            End If
            '*** Parameters
            '***
            _DataObjectCaches.Clear()
            _ObjectPermissionCache.Clear()
            _DomainRepositories.Clear()
            _OTDBUser = Nothing
            IsRunning = False
            Call CoreMessageHandler(showmsgbox:=False, message:="Session ended ", argument:=_SessionID, _
                                    break:=True, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationInfo, _
                                    procedure:="Session.shutdownSessionEviorment")
            '** flush the log
            Me.OTDBDriver.PersistLog(Me.Errorlog)
            Return True

        End Function

        ''' <summary>
        ''' changes the session user to a new object
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub UserChangedEvent(newuser As User)
            _OTDBUser = newuser
            _Username = _OTDBUser.Username
            _ObjectPermissionCache.Clear()
        End Sub

        ''' <summary>
        ''' handler for domain switched
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub Session_OnDomainChanged(sender As Object, e As SessionEventArgs) Handles Me.OnDomainChanged
            ' reset
            _cacheRepositoryActiveDomain = Nothing
        End Sub

        ''' <summary>
        ''' generate a rule
        ''' </summary>
        ''' <param name="rule"></param>
        ''' <returns></returns>
        Public Function GenerateRule(rule As IRule, ByRef code As rulez.ICodeBit) As Boolean Implements iDataObjectEngine.Generate
            ''' check the rule type to generate
            If Not rule.GetType().IsAssignableFrom(GetType(SelectionRule)) Then
                Throw New ormException(ormException.Types.WrongRule, arguments:={rule.GetType.FullName, GetType(SelectionRule).FullName})
            End If

            ''' check the objectnames
            ''' 
            Dim aList As New List(Of iormDataObjectProvider)
            For Each aName In CType(rule, SelectionRule).ResultingObjectnames
                Dim aProvider = Me.Objects.GetDataObjectProvider(aName)
                If aProvider IsNot Nothing AndAlso aList.Contains(aProvider) Then aList.Add(aProvider)
            Next

            ''' prepare via the first
            If aList.Count > 0 Then Return aList.First.PrepareSelection(CType(rule, SelectionRule), code)
            Return False
        End Function

        ''' <summary>
        ''' run a rule by id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="context"></param>
        ''' <returns></returns>
        Public Function Run(id As String, context As Context) As Boolean Implements iDataObjectEngine.Run
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function
    End Class

    ''' <summary>
    ''' Session Event Arguments
    ''' </summary>
    ''' <remarks></remarks>

    Public Class SessionEventArgs
        Inherits EventArgs

        Private _Session As Session
        Private _NewDomain As Domain
        Private _newConfigSetName As String
        Private _newWorkspaceID As String

        Private _Cancel As Boolean

        Public Sub New(Session As Session, Optional newDomain As Domain = Nothing, Optional abortOperation As Boolean? = Nothing, Optional newWorkspaceID As String = Nothing, Optional newConfigsetName As String = Nothing)
            _Session = Session
            _NewDomain = newDomain
            _newWorkspaceID = newWorkspaceID
            If abortOperation.HasValue Then _Cancel = abortOperation
            If newConfigsetName IsNot Nothing Then _newConfigSetName = newConfigsetName
        End Sub
        ''' <summary>
        ''' Gets the abort operation.
        ''' </summary>
        ''' <value>The abort operation.</value>
        Public ReadOnly Property AbortOperation() As Boolean
            Get
                Return Me._Cancel
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the new domain ID.
        ''' </summary>
        ''' <value>The new domain ID.</value>
        Public Property NewDomain() As Domain
            Get
                Return Me._NewDomain
            End Get
            Set(value As Domain)
                Me._NewDomain = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the error.
        ''' </summary>
        ''' <value>The error.</value>
        Public ReadOnly Property [Session]() As Session
            Get
                Return _Session
            End Get
        End Property

    End Class

    ''' <summary>
    '''  Session Agent Class
    ''' </summary>
    ''' <remarks></remarks>

    Public Class SessionAgent
        Private _workerTimer As TimerCallback  'Workerthread
        Private _autoEvent As New AutoResetEvent(False)
        Private _threadTimer As System.Threading.Timer
        Private _session As Session
        Private _workinprogress As Boolean = False
        Private _stopped As Boolean = False

        Public Sub New(session As Session)
            _session = session
        End Sub
        ''' <summary>
        ''' Worker Sub 
        ''' </summary>
        ''' <param name="stateInfo"></param>
        ''' <remarks></remarks>
        Private Sub Worker(stateInfo As Object)
            If _session.IsRunning Then
                If Not _workinprogress AndAlso Not _stopped Then
                    _workinprogress = True
                    _session.OTDBDriver.PersistLog(_session.Errorlog)
                    _workinprogress = False
                End If
            End If
        End Sub
        ''' <summary>
        ''' Start the Agent
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Start()
            Initialize()
        End Sub
        ''' <summary>
        ''' Stop the the Agent
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub [Stop]()
            _stopped = True
            If Not _threadTimer Is Nothing Then
                ' When autoEvent signals, change the period to every  
                ' 1/2 second.
                _autoEvent.WaitOne(500, False)
                _threadTimer.Change(New TimeSpan(0), New TimeSpan(0, 0, 0, 250))

                ' When autoEvent signals the second time, dispose of  
                ' the timer.
                _autoEvent.WaitOne(500, False)
                _threadTimer.Dispose()
                Console.WriteLine(vbCrLf & "Destroying timer.")
                _threadTimer = Nothing
            End If
        End Sub
        Private Sub Initialize()
            If _threadTimer Is Nothing Then
                _workerTimer = AddressOf Me.Worker
                Dim delayTime As New TimeSpan(0, 0, 0, 50)
                Dim intervalTime As New TimeSpan(0, 0, 60)
                ' Create a timer that signals the delegate to invoke  
                ' CheckStatus after one second, and every 1/4 second  
                ' thereafter.
                Console.WriteLine("{0} Creating timer." & vbCrLf, _
                                  DateTime.Now.ToString("h:mm:ss.fff"))
                _threadTimer = New System.Threading.Timer(AddressOf Worker, _autoEvent, delayTime, intervalTime)

            End If

        End Sub

    End Class

End Namespace

