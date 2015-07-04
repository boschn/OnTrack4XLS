
REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** ORM DATA OBJECT FACTORY CLASSES
REM ***********
REM *********** Version: 2.0
REM *********** Created: 2014-01-31
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
Imports System.Reflection
Imports OnTrack.Commons
Imports OnTrack.rulez
Imports OnTrack.rulez.eXPressionTree
Imports OnTrack.Core
Imports System.Linq.Expressions

Namespace OnTrack.Database

    ''' <summary>
    ''' a singleton Business Object Factory class
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class ormDataObjectProvider
        Implements iormDataObjectProvider

        Protected _IsInitialized As Boolean = False
        Protected WithEvents _repository As ormObjectRepository
        Protected _classdescriptionsByIds As New Dictionary(Of String, ObjectClassDescription)
        Protected _classdescriptionsByTypeFullname As New Dictionary(Of String, ObjectClassDescription)
        ''' <summary>
        ''' constants
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstQryAllObjects As String = "ALL"
        Friend Const ConstPRIMARYKEY As String = "PRIMARYKEY"

#Region "Properties"


        ''' <summary>
        ''' returns the types of this business object factory
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Types As List(Of Type) Implements iormDataObjectProvider.Types
            Get
                If Not Me.IsInitialized AndAlso Not Me.Initialize Then Return New List(Of Type)
                Return _classdescriptionsByIds.Values.Select(Function(x) x.Type).ToList
            End Get
        End Property
        ''' <summary>
        ''' returns the object ids of this factory
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectIDs As List(Of String) Implements iormDataObjectProvider.ObjectIDs
            Get
                If Not Me.IsInitialized AndAlso Not Me.Initialize Then Return New List(Of String)
                Return _classdescriptionsByIds.Keys.ToList
            End Get
        End Property
        ''' <summary>
        ''' sets or gets the repository this Object belongs to
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Repository As ormObjectRepository Implements iormDataObjectProvider.Repository
            Get
                Return _repository
            End Get
            Set(value As ormObjectRepository)
                Initialize(value)
            End Set
        End Property
        ''' <summary>
        ''' returns true if the factory is initialized
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsInitialized As Boolean
            Get
                Return _IsInitialized
            End Get
        End Property
        ''' <summary>
        ''' gets the IDataObjectRepository to which the provider belongs
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DataObjectRepository As iDataObjectRepository Implements iormDataObjectProvider.DataObjectRepository
            Get
                Return Me.Repository
            End Get
        End Property
#End Region

        Public Event OnCreated(sender As Object, e As ormDataObjectEventArgs) Implements iormDataObjectProvider.OnCreated
        Public Event OnCreating(sender As Object, e As ormDataObjectEventArgs) Implements iormDataObjectProvider.OnCreating
        Public Event OnOverloaded(sender As Object, e As ormDataObjectOverloadedEventArgs) Implements iormDataObjectProvider.OnOverloaded
        Public Event OnRetrieved(sender As Object, e As ormDataObjectEventArgs) Implements iormDataObjectProvider.OnRetrieved
        Public Event OnRetrieving(sender As Object, e As ormDataObjectEventArgs) Implements iormDataObjectProvider.OnRetrieving

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="Session"></param>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
        End Sub
        ''' <summary>
        ''' Creates an instance of an data object by type
        ''' </summary>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Public Function CreateDataObjectInstance(type As Type) As iormDataObject
        '    Return ot.ObjectClassRepository.CreateInstance(type:=type)
        'End Function
        ''' <summary>
        ''' Creates an instance of an data object by type
        ''' </summary>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateDataObjectInstance(type As Type) As iDataObject Implements iDataObjectProvider.NewDataObject
            Return ot.ObjectClassRepository.CreateInstance(type:=type)
        End Function
        ''' <summary>
        ''' Initialize
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overridable Function Initialize(Optional repository As ormObjectRepository = Nothing) As Boolean
            If repository Is Nothing Then
                repository = ot.CurrentSession.Objects
            End If
            ' deregister
            If repository IsNot Nothing Then
                If _repository IsNot Nothing Then
                    RemoveHandler _repository.OnObjectDefinitionLoaded, AddressOf Me._repository_OnObjectDefinitionLoaded
                    _classdescriptionsByIds.Clear()
                End If
                'register this one here
                _repository = repository
                If repository.IsInitialized Then
                    For Each anObjectID In repository.ObjectDefinitions.Select(Function(x) x.ID).ToList
                        RegisterObjectID(anObjectID)
                    Next
                End If
                AddHandler _repository.OnObjectDefinitionLoaded, AddressOf _repository_OnObjectDefinitionLoaded
            End If

            Return True
        End Function
        ''' <summary>
        ''' returns true if the ObjectID is handled by this factory
        ''' </summary>
        ''' <param name="objectid"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasObjectID(objectid As String) As Boolean Implements iormDataObjectProvider.HasObjectID
            If Not Me.IsInitialized AndAlso Not Me.Initialize Then Return False
            If _classdescriptionsByIds.ContainsKey(objectid.ToUpper) Then Return True
            Return False
        End Function
        ''' <summary>
        ''' returns true if the type is handled by this factory
        ''' </summary>
        ''' <param name="objectid"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasType(type As System.Type) As Boolean Implements iormDataObjectProvider.HasType
            If Not Me.IsInitialized AndAlso Not Me.Initialize Then Return False
            If _classdescriptionsByTypeFullname.ContainsKey(type.FullName.ToUpper) Then Return True
            Return False
        End Function
        ''' <summary>
        ''' handle the OnObjectDefinition Loaded in the repository event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub _repository_OnObjectDefinitionLoaded(sender As Object, e As ormObjectRepository.EventArgs) Handles _repository.OnObjectDefinitionLoaded
            RegisterObjectID(e.Objectname)
        End Sub
        ''' <summary>
        ''' register the object id at the factory to enable processing
        ''' </summary>
        ''' <param name="objectid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RegisterObjectID(objectid As String) As Boolean Implements iormDataObjectProvider.RegisterObjectID
            If _repository IsNot Nothing Then
                Dim aClassDescription As ObjectClassDescription = ot.GetObjectClassDescriptionByID(id:=objectid)
                If aClassDescription IsNot Nothing Then
                    If GetType(ormBusinessObject).IsAssignableFrom(aClassDescription.Type) Then
                        If Not _classdescriptionsByIds.ContainsKey(aClassDescription.ID.ToUpper) Then
                            _classdescriptionsByIds.Add(key:=aClassDescription.ID.ToUpper, value:=aClassDescription)
                        End If
                        If Not _classdescriptionsByTypeFullname.ContainsKey(aClassDescription.Type.FullName.ToUpper) Then
                            _classdescriptionsByTypeFullname.Add(key:=aClassDescription.Type.FullName.ToUpper, value:=aClassDescription)
                        End If
                        Return True
                    Else
                        Throw New ormException(ormException.Types.WrongDataObjectProvider, arguments:={objectid, Me.GetType.FullName})
                    End If
                End If
            End If
            Return False
        End Function
        ''' <summary>
        ''' extract out of a record a Primary Key array
        ''' </summary>
        ''' <param name="record"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Shared Function ExtractObjectPrimaryKey(record As ormRecord, objectID As String,
                                                        Optional runtimeOnly As Boolean = False) As ormDatabaseKey
            Dim thePrimaryKeyEntryNames As String()
            Dim pkarray As Object()
            Dim anObjectDefinition = CurrentSession.Objects.GetObjectDefinition(objectID)

            '* keynames of the object

            thePrimaryKeyEntryNames = anObjectDefinition.PrimaryKeyEntryNames
            If thePrimaryKeyEntryNames.Count = 0 Then
                CoreMessageHandler(message:="objectdefinition has not primary keys", objectname:=anObjectDefinition.Objectname, _
                               procedure:="ormDataObjectFactory.ExtractPrimaryKey", messagetype:=otCoreMessageType.InternalWarning)
                Return Nothing
            End If

            '* extract
            ReDim pkarray(thePrimaryKeyEntryNames.Length - 1)
            Dim i As UShort = 0
            For Each anEntry In anObjectDefinition.GetKeyEntries
                If record.HasIndex(DirectCast(anEntry, ormObjectFieldEntry).ContainerEntryName) Then
                    pkarray(i) = record.GetValue(index:=DirectCast(anEntry, ormObjectFieldEntry).ContainerEntryName)
                    i += 1
                End If
            Next


            Return New ormDatabaseKey(objectid:=objectID, keyvalues:=pkarray)
        End Function
        ''' <summary>
        ''' Creates an instance of an iormdataobject
        ''' </summary>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function NewOrmDataObject(type As Type) As iormDataObject Implements iormDataObjectProvider.NewOrmDataObject
            Return ot.ObjectClassRepository.CreateInstance(type:=type)
        End Function
        ''' <summary>
        ''' Create a DataObject by objectid
        ''' </summary>
        ''' <param name="objectid"></param>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Create(objectid As String, key As iKey) As iDataObject Implements iDataObjectProvider.Create
            Dim aType As System.Type = CurrentSession.Objects.GetObjectType(objectname:=objectid)
            If aType Is Nothing Then
                Throw New ormException(ormException.Types.NoObjectIDFound, arguments:={objectid})
                Return Nothing
            End If
            Return Me.Create(type:=aType, primarykey:=TryCast(key, ormDatabaseKey))
        End Function
        ''' <summary>
        ''' returns a created business object of certain type
        ''' </summary>
        ''' <param name="primarykey"></param>
        ''' <param name="type"></param>
        ''' <param name="domainID"></param>
        ''' <param name="checkUnique"></param>
        ''' <param name="runTimeonly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Create(primarykey As ormDatabaseKey, type As Type, _
                               Optional domainID As String = Nothing, _
                               Optional checkUnique As Boolean? = Nothing, _
                               Optional runTimeonly As Boolean? = Nothing) As iormDataObject Implements iormDataObjectProvider.Create

            If Not Me.IsInitialized AndAlso Not Me.Initialize Then
                CoreMessageHandler(message:="could not initiliaze factory", procedure:="ormDataObjectFactory.Create", _
                                   argument:=type.FullName, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If

            If Not Me.HasType(type) Then
                Throw New ormException(ormException.Types.WrongDataObjectProvider, arguments:={type.FullName, Me.GetType.FullName})
                Return Nothing
            End If
            ''' a new data object
            Dim aDataobject As iormPersistable = TryCast(Me.CreateDataObjectInstance(type), iormPersistable)
            If aDataobject Is Nothing Then
                CoreMessageHandler(message:="type is not implementing iormPersistable", procedure:="ormDataObjectFactory.Create", _
                                  argument:=type.FullName, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If
            ''' defautl values
            If String.IsNullOrEmpty(domainID) Then domainID = CurrentSession.CurrentDomainID
            If Not checkUnique.HasValue Then checkUnique = True
            If Not runTimeonly.HasValue Then runTimeonly = False
            ''' Substitute the DomainID if necessary
            If String.IsNullOrEmpty(domainID) Then domainID = CurrentSession.CurrentDomainID

            ''' fix the primary key
            ''' 
            primarykey.ChecknFix(domainid:=domainID, runtimeOnly:=runTimeonly)

            '** fire event
            Dim ourEventArgs As New ormDataObjectEventArgs([object]:=aDataobject, _
                                                           record:=aDataobject.Record, _
                                                          key:=primarykey, _
                                                           usecache:=aDataobject.ObjectUsesCache, _
                                                           runtimeonly:=runTimeonly)
            RaiseEvent OnCreating(Nothing, ourEventArgs)
            If ourEventArgs.AbortOperation Then
                If ourEventArgs.Result Then
                    Return ourEventArgs.DataObject
                Else
                    Return Nothing
                End If
            ElseIf ourEventArgs.Result Then
                primarykey = ourEventArgs.Key
            End If

            If aDataobject.Create(primarykey, domainID:=domainID, runTimeonly:=runTimeonly, checkUnique:=checkUnique) Then
                '** fire event
                ourEventArgs = New ormDataObjectEventArgs([object]:=aDataobject, _
                                                               record:=aDataobject.Record, _
                                                              key:=primarykey, _
                                                               usecache:=aDataobject.ObjectUsesCache, _
                                                               runtimeonly:=runTimeonly)
                RaiseEvent OnCreated(Nothing, ourEventArgs)
                If ourEventArgs.AbortOperation Then
                    If ourEventArgs.Result Then
                        Return ourEventArgs.DataObject
                    Else
                        Return Nothing
                    End If
                End If
                Return aDataobject
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' create an instance of a certain object type with a record (and containing keys and default values)
        ''' </summary>
        ''' <param name="record"></param>
        ''' <param name="type"></param>
        ''' <param name="domainID"></param>
        ''' <param name="checkUnique"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Create(ByRef record As ormRecord, type As Type, Optional domainID As String = Nothing, Optional checkUnique As Boolean? = Nothing, Optional runtimeOnly As Boolean? = Nothing) As iormDataObject Implements iormDataObjectProvider.Create
            If Not Me.IsInitialized AndAlso Not Me.Initialize Then
                CoreMessageHandler(message:="could not initiliaze factory", procedure:="ormDataObjectFactory.Create", _
                                   argument:=type.FullName, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If

            If Not Me.HasType(type) Then
                Throw New ormException(ormException.Types.WrongDataObjectProvider, arguments:={type.FullName, Me.GetType.FullName})
                Return Nothing
            End If
            Dim aDataobject As iormPersistable = TryCast(Me.CreateDataObjectInstance(type), iormPersistable)
            If aDataobject Is Nothing Then
                CoreMessageHandler(message:="type is not implementing iormPersistable", procedure:="ormDataObjectFactory.Create", _
                                 argument:=type.FullName, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If
            ''' defautl values
            If String.IsNullOrEmpty(domainID) Then domainID = CurrentSession.CurrentDomainID
            If Not checkUnique.HasValue Then checkUnique = True
            If Not runtimeOnly.HasValue Then runtimeOnly = False
            ''' Get the Primary key
            Dim aPrimaryKey As ormDatabaseKey = ExtractObjectPrimaryKey(record:=record, objectID:=aDataobject.ObjectID, runtimeOnly:=runtimeOnly)
            ''' Substitute the DomainID if necessary
            If String.IsNullOrEmpty(domainID) Then domainID = CurrentSession.CurrentDomainID

            ''' fix primary key
            ''' 
            aPrimaryKey.ChecknFix(domainid:=domainID, runtimeOnly:=runtimeOnly)

            '** fire event
            Dim ourEventArgs As New ormDataObjectEventArgs([object]:=aDataobject, _
                                                           record:=record, _
                                                           key:=aPrimaryKey, _
                                                           usecache:=aDataobject.ObjectUsesCache, _
                                                           runtimeonly:=runtimeOnly)
            RaiseEvent OnCreating(Nothing, ourEventArgs)
            If ourEventArgs.AbortOperation Then
                If ourEventArgs.Result Then
                    Return ourEventArgs.DataObject
                Else
                    Return Nothing
                End If
            Else
                record = ourEventArgs.Record
            End If

            If aDataobject.Create(record, domainID:=domainID, runtimeOnly:=runtimeOnly, checkUnique:=checkUnique) Then
                '** fire event
                ourEventArgs = New ormDataObjectEventArgs([object]:=aDataobject, _
                                                               record:=record, _
                                                               key:=ExtractObjectPrimaryKey(record:=record, objectID:=aDataobject.ObjectID, runtimeOnly:=runtimeOnly), _
                                                               usecache:=aDataobject.ObjectUsesCache, _
                                                               runtimeonly:=runtimeOnly)
                RaiseEvent OnCreated(Nothing, ourEventArgs)
                If ourEventArgs.AbortOperation Then
                    If ourEventArgs.Result Then
                        Return ourEventArgs.DataObject
                    Else
                        Return Nothing
                    End If
                End If
            Else
                Return Nothing
            End If

            Return aDataobject
        End Function
        ''' <summary>
        ''' retrieves dataobject by selection rule
        ''' </summary>
        ''' <param name="rule"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Retrieve(rule As SelectionRule) As IEnumerable(Of iDataObject) Implements iDataObjectProvider.Retrieve
            Throw New NotImplementedException("Retrieve by rule is not implemented")
        End Function
        ''' <summary>
        ''' retrieves a dataobject from the store
        ''' </summary>
        ''' <param name="objectid"></param>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Retrieve(objectid As String, key As iKey) As iDataObject Implements iDataObjectProvider.Retrieve
            Dim aType As System.Type = CurrentSession.Objects.GetObjectType(objectname:=objectid)
            If aType Is Nothing Then
                Throw New ormException(ormException.Types.NoObjectIDFound, arguments:={objectid})
                Return Nothing
            End If
            Return Me.Retrieve(type:=aType, key:=TryCast(key, ormDatabaseKey))
        End Function
        ''' <summary>
        ''' retrieve a data object
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="type"></param>
        ''' <param name="domainID"></param>
        ''' <param name="forceReload"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Retrieve(key As ormDatabaseKey, type As Type, _
                                             Optional domainID As String = Nothing, _
                                             Optional forceReload As Boolean? = Nothing, _
                                             Optional runtimeOnly As Boolean? = Nothing) As iormDataObject Implements iormDataObjectProvider.Retrieve

            If Not Me.IsInitialized AndAlso Not Me.Initialize Then
                CoreMessageHandler(message:="could not initiliaze factory", procedure:="ormDataObjectFactory.Retrieve", _
                                   argument:=type.FullName, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If

            If Not Me.HasType(type) Then
                Throw New ormException(ormException.Types.WrongDataObjectProvider, arguments:={type.FullName, Me.GetType.FullName})
            End If
            Dim useCache As Boolean = True
            If String.IsNullOrEmpty(domainID) Then domainID = CurrentSession.CurrentDomainID
            If Not runtimeOnly.HasValue Then runtimeOnly = False
            If Not forceReload.HasValue Then forceReload = False
            Dim anObject As iormPersistable = TryCast(Me.NewOrmDataObject(type), iormPersistable)
            If anObject Is Nothing Then
                CoreMessageHandler(message:="type is not implementing iormPersistable", procedure:="ormDataObjectFactory.Retrieve", _
                                   argument:=type.FullName, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If
            Dim dbdriver As iormDatabaseDriver

            '** is a session running ?!
            If Not runtimeOnly AndAlso Not CurrentSession.IsRunning AndAlso Not CurrentSession.IsStartingUp Then
                Call CoreMessageHandler(message:="data object cannot be retrieved - start session to database first", _
                                        objectname:=anObject.ObjectID, _
                                        procedure:="ormDataObjectFactory.Retrieve", messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If

            '** use Cache ?!
            useCache = anObject.ObjectUsesCache
            Dim hasDomainBehavior As Boolean = anObject.ObjectHasDomainBehavior
            Dim globalKey As ormDatabaseKey
            ''' fix primary key
            ''' 
            key.ChecknFix(domainid:=domainID, runtimeOnly:=runtimeOnly)

            ''' check if we have key
            ''' 
            If key.Count = 0 Then
                Call CoreMessageHandler(message:="data object cannot be retrieved - no primary key and also no record for keys provided", _
                                       objectname:=anObject.ObjectID, username:=CurrentSession.CurrentUsername, _
                                       procedure:="ormDataObjectFactory.Retrieve", messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If

            '* fire event
            Dim ourEventArgs As New ormDataObjectEventArgs(anObject, domainID:=domainID, domainBehavior:=hasDomainBehavior, key:=key, usecache:=useCache)
            RaiseEvent OnRetrieving(Nothing, ourEventArgs)
            If ourEventArgs.AbortOperation Then
                If ourEventArgs.Result Then
                    Return ourEventArgs.DataObject
                Else
                    Return Nothing
                End If

                '*** we have a result yes to use the dataobject supplied
            ElseIf ourEventArgs.Result Then
                anObject = ourEventArgs.DataObject
                useCache = False ' switch off cache

                ''' no positive result from the events
                ''' check if we take the substitute domainID
            ElseIf Not ourEventArgs.Result Then
                If hasDomainBehavior AndAlso domainID <> ConstGlobalDomain Then
                    '* Domain Behavior - is global cached but it might be that we are missing the domain related one if one has been created
                    '* after load of the object - since not in cache
                    globalKey = key.Clone
                    globalKey.SubstituteDomainID(domainid:=ConstGlobalDomain, runtimeOnly:=runtimeOnly)
                    '* fire event again
                    ourEventArgs = New ormDataObjectEventArgs(anObject, domainID:=domainID, domainBehavior:=hasDomainBehavior, key:=globalKey)
                    RaiseEvent OnRetrieving(Nothing, ourEventArgs)
                    If ourEventArgs.AbortOperation Then
                        If ourEventArgs.Result Then
                            Return ourEventArgs.DataObject
                        Else
                            Return Nothing
                        End If
                    ElseIf ourEventArgs.Result Then
                        '** retrieved by success
                        anObject = ourEventArgs.DataObject
                        useCache = False ' switch off cache
                    Else
                        anObject = Nothing
                    End If
                Else
                    anObject = Nothing ' load it
                End If
            Else
                anObject = Nothing ' load it
            End If

            '* load object if not runtime only
            If (anObject Is Nothing OrElse forceReload) And Not runtimeOnly Then
                '* create the data object
                anObject = TryCast(Me.CreateDataObjectInstance(type), iormPersistable)
                If anObject Is Nothing Then
                    Call CoreMessageHandler(message:="data object of type '" & type.FullName & "' is not implementing iormPersistable - retrieve aborted", _
                                      objectname:=anObject.ObjectID, username:=CurrentSession.CurrentUsername, _
                                      procedure:="ormDataObjectFactory.Retrieve", messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If

                '* domain substitution if not injectable ?!
                If Not anObject.Inject(key:=key, domainid:=domainID, dbdriver:=dbdriver) Then
                    ' try to get overloaded domain object
                    If hasDomainBehavior AndAlso domainID <> ConstGlobalDomain Then
                        '* on domain behavior ? -> reload from  the global domain
                        If globalKey Is Nothing Then
                            globalKey = key.Clone
                            globalKey.SubstituteDomainID(domainid:=ConstGlobalDomain, substituteOnlyNothingDomain:=False, runtimeOnly:=runtimeOnly)
                        End If
                        ''' add it to cache by event overloaded
                        If anObject.Inject(key:=globalKey, domainid:=ConstGlobalDomain, dbdriver:=dbdriver) Then
                            RaiseEvent OnOverloaded(Nothing, _
                                                          New ormDataObjectOverloadedEventArgs(globalPrimaryKey:=globalKey, domainPrimaryKey:=key, dataobject:=anObject))
                        Else
                            ' reset
                            anObject = Nothing
                        End If
                    Else
                        ' reset
                        anObject = Nothing
                    End If
                End If

                '* fire event if successful
                If anObject IsNot Nothing Then
                    ourEventArgs = New ormDataObjectEventArgs(anObject, record:=anObject.Record, key:=key, usecache:=useCache)
                    '** fire event
                    RaiseEvent OnRetrieved(Nothing, ourEventArgs)
                    If ourEventArgs.AbortOperation Then
                        If ourEventArgs.Result Then
                            Return ourEventArgs.DataObject
                        Else
                            Return Nothing
                        End If
                    End If
                End If
            End If

            Return anObject

        End Function
        ''' <summary>
        ''' retrieve all dataobject
        ''' </summary>
        ''' <param name="objectid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function RetrieveAll(objectid As String) As IEnumerable(Of iDataObject) Implements iDataObjectProvider.RetrieveAll
            Dim aType As System.Type = CurrentSession.Objects.GetObjectType(objectname:=objectid)
            If aType Is Nothing Then
                Throw New ormException(ormException.Types.NoObjectIDFound, arguments:={objectid})
                Return Nothing
            End If
            Return RetrieveAll(aType)
        End Function
        ''' <summary>
        ''' retrieve all business objects regardless of selection criteria
        ''' </summary>
        ''' <param name="type"></param>
        ''' <param name="domainID"></param>
        ''' <param name="forceReload"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function RetrieveAll(type As Type, _
                                    Optional key As ormDatabaseKey = Nothing, _
                                    Optional domainID As String = Nothing, _
                                    Optional deleted As Boolean = False, _
                                    Optional forceReload As Boolean? = Nothing, _
                                    Optional runtimeOnly As Boolean? = Nothing) As IEnumerable(Of iormDataObject) Implements iormDataObjectProvider.RetrieveAll
            Throw New NotImplementedException("ormDataObjectFactory.RetrieveALL")
        End Function
        ''' <summary>
        ''' add the behavior statements to the selection expression
        ''' </summary>
        ''' <param name="rule"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overridable Function AddBehavior(rule As SelectionRule, Optional context As Context = Nothing) As Boolean

            Try

                ''' check which objects have a predefined behavior
                ''' 
                For Each anObjectID In rule.Result.Objectnames

                    Dim anObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=anObjectID)
                    If anObjectDefinition Is Nothing Then
                        Throw New ormException(ormException.Types.TypeNotFound, arguments:={anObjectID})
                    Else
                        ''' add the domain behavior
                        ''' 
                        If anObjectDefinition.HasDomainBehavior Then
                            ''' add the domain
                            Dim aDomain As rulez.eXPressionTree.Variable = rule.Parameters.Where(Function(x) x.ID = "@" & Domain.ConstFNDomainID).FirstOrDefault
                            If aDomain Is Nothing Then
                                aDomain = rule.AddNewParameter("@" & Domain.ConstFNDomainID, otDataType.Text)
                                ''' extend the selection expression
                                Dim aDataObjectEntry As DataObjectEntrySymbol = New DataObjectEntrySymbol(objectid:=anObjectID, entryname:=Domain.ConstFNDomainID)
                                Dim addSelection = LogicalExpression.ORELSE(LogicalExpression.EQ(aDataObjectEntry, aDomain), LogicalExpression.EQ(aDataObjectEntry, New Literal(ConstGlobalDomain, otDataType.Text)))
                                If rule.Selection.Nodes.Count = 0 Then
                                    rule.Selection = addSelection
                                Else
                                    rule.Selection = rule.Selection.ANDALSO(addSelection)
                                End If
                            End If
                            ''' add it to context
                            If context IsNot Nothing AndAlso Not context.Itemnames.Contains("@" & Domain.ConstFNDomainID) Then
                                context.AddItem(id:="@" & Domain.ConstFNDomainID, value:=CurrentSession.CurrentDomainID)
                            End If

                        End If
                        ''' add the delete behavior
                        ''' 
                        If anObjectDefinition.HasDeleteFieldBehavior Then
                            ''' add the parameter
                            Dim aDelete As rulez.eXPressionTree.Variable = rule.Parameters.Where(Function(x) x.ID = "@" & ormDataObject.ConstFNIsDeleted).FirstOrDefault
                            If aDelete Is Nothing Then
                                aDelete = rule.AddNewParameter("@" & ormDataObject.ConstFNIsDeleted, otDataType.Bool)
                                ''' extend selection rule
                                Dim aDataObjectEntry As DataObjectEntrySymbol = New DataObjectEntrySymbol(objectid:=anObjectID, entryname:=Domain.ConstFNIsDeleted)
                                If rule.Selection.Nodes.Count = 0 Then
                                    rule.Selection = LogicalExpression.EQ(aDataObjectEntry, aDelete)
                                Else
                                    rule.Selection = rule.Selection.ANDALSO(LogicalExpression.EQ(aDataObjectEntry, aDelete))
                                End If
                            End If
                            ''' add it to context
                            If context IsNot Nothing AndAlso Not context.Itemnames.Contains("@" & Domain.ConstFNIsDeleted) Then
                                context.AddItem(id:="@" & ormDataObject.ConstFNIsDeleted, value:=False)
                            End If
                        End If
                    End If

                Next

                Return True

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ormDataObjectProvider.AddBehavior")
                Return False
            End Try

        End Function
        ''' <summary>
        ''' retrieves data objects of a type by a list of keys supplied by ormRecords. Provide the targettype and optional the objectid, the primaryKeyIndex is the 
        ''' index of the primary key im the record. if this nothing or empty the index is determined from the first record.
        ''' The Domainbehavior can be specified - if nothing then it will be determined from the repository.
        ''' The Objects will be retrieved and not infused.
        ''' </summary>
        ''' <param name="list"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function RetrieveFrom(recordList As List(Of ormRecord), _
                                  targetType As System.Type, _
                                  ByRef primaryKeyIndex As UShort(), _
                                  Optional ByRef objectid As String = Nothing, _
                                  Optional ByRef domainbehavior As Boolean? = Nothing, _
                                  Optional ByRef primaryKeyNames As List(Of String) = Nothing) As List(Of iDataObject)
            Dim aTargetObjectDefinition As ormObjectDefinition
            Dim resultList As New List(Of iDataObject)
            ''' objectid 
            If String.IsNullOrEmpty(objectid) Then
                objectid = CurrentSession.Objects.GetObjectname(targetType)
            End If
            ''' determine the indexes
            If primaryKeyIndex Is Nothing OrElse primaryKeyIndex.Length = 0 Then
                If primaryKeyNames Is Nothing Then
                    If aTargetObjectDefinition Is Nothing Then aTargetObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=objectid)
                    primaryKeyNames = aTargetObjectDefinition.PrimaryKeyEntryNames.ToList()
                End If
                '' get the indexes
                If primaryKeyNames IsNot Nothing AndAlso primaryKeyNames.Count > 0 Then
                    ReDim primaryKeyIndex(primaryKeyNames.Count)
                    For I = 0 To primaryKeyNames.Count
                        primaryKeyIndex(I) = recordList.First.ZeroBasedIndexOf(primaryKeyNames.Item(I))
                    Next
                End If
            End If
            ''' if we do not have the domain behavior
            If Not domainbehavior.HasValue Then
                If aTargetObjectDefinition Is Nothing Then aTargetObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=objectid)
                domainbehavior = aTargetObjectDefinition.HasDomainBehavior
            End If

            ''' depending if we have domain behavior or not
            ''' 
            If domainbehavior And CurrentSession.CurrentDomainID <> ConstGlobalDomain Then
                Dim interimList As New Dictionary(Of String, ormRecord)
                For Each aRecord As ormRecord In recordList
                    '** build pk key
                    Dim pk As String = String.Empty
                    For Each acolumnname In primaryKeyNames
                        If acolumnname <> Commons.Domain.ConstFNDomainID Then pk &= aRecord.GetValue(index:=acolumnname).ToString & ConstDelimiter
                    Next
                    If interimList.ContainsKey(pk) Then
                        Dim anotherRecord = interimList.Item(pk)
                        If anotherRecord.GetValue(Commons.Domain.ConstFNDomainID).ToString = ConstGlobalDomain Then
                            interimList.Remove(pk)
                            interimList.Add(key:=pk, value:=aRecord)
                        End If
                    Else
                        interimList.Add(key:=pk, value:=aRecord)
                    End If
                Next
                ''' take each from the interim list
                ''' 
                Dim theKeyvalues As Object()
                ReDim theKeyvalues(primaryKeyNames.Count)

                For Each aRecord As ormRecord In interimList.Values
                    For i = 0 To theKeyvalues.GetUpperBound(0)
                        theKeyvalues(i) = aRecord.GetValue(primaryKeyIndex(i))
                    Next
                    Dim aKey As New ormDatabaseKey(objectid:=objectid, keyvalues:=theKeyvalues)
                    Dim anObject As iDataObject = Me.Retrieve(objectid:=objectid, key:=aKey)
                    If anObject IsNot Nothing Then resultList.Add(anObject)
                Next
            Else
                ''' take each and retrieve
                ''' 
                Dim theKeyvalues As Object()
                ReDim theKeyvalues(primaryKeyNames.Count)
                For Each aRecord As ormRecord In recordList
                    For i = 0 To theKeyvalues.GetUpperBound(0)
                        theKeyvalues(i) = aRecord.GetValue(primaryKeyIndex(i))
                    Next
                    Dim aKey As New ormDatabaseKey(objectid:=objectid, keyvalues:=theKeyvalues)
                    Dim anObject As iDataObject = Me.Retrieve(objectid:=objectid, key:=aKey)
                    If anObject IsNot Nothing Then resultList.Add(anObject)
                Next

            End If
            Return resultList
        End Function
        ''' <summary>
        ''' Prepares a selection rule to be run
        ''' </summary>
        ''' <param name="rule"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function PrepareSelection(rule As SelectionRule, ByRef resultCode As rulez.ICodeBit) As Boolean Implements iormDataObjectProvider.PrepareSelection
            resultCode = New CodeBit()
            Dim anObjectID As String
            Dim anObjectDefinition As iormObjectDefinition

            '''
            ''' check on the domain / 
            ''' check the data base drivers
            ''' 
            Dim aList As New List(Of iormDatabaseDriver)
            Dim aKeyName As String
            ''' collect all the drivers
            For Each aName In rule.ResultingObjectnames
                anObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=aName)
                If anObjectDefinition Is Nothing Then
                    Throw New ormException(ormException.Types.NoObjectIDFound, arguments:={aName})
                End If
                ''' check which way we are accessing (by PrimaryKey, Indice or query)
                ''' 
                aKeyName = Me.CheckKeyName(rule, aName)

                ''' collect the database drivers
                ''' 
                For Each anContainerID In anObjectDefinition.ContainerIDs
                    For Each aDatabaseDriver In CurrentSession.GetDatabaseDrivers(containerID:=anContainerID)
                        If Not aList.Contains(aDatabaseDriver) Then aList.Add(aDatabaseDriver)
                    Next
                Next
            Next

            ''' check on the the drivers
            '''
            If aList.Count = 0 Then
                ''' no driver
                Return False
            ElseIf aList.Count > 1 Then
                ''' more than 1 
                ''' 
                Throw New NotImplementedException("more than 1 db driver necessary to prepare selection rule")
                Return False
            End If

            ''' add the behavior to the selection rule
            ''' 
            If Not AddBehavior(rule:=rule) Then Return False

            ''' build the selection by retrieve by the primary key
            ''' 
            If rule.ResultingObjectnames.Count = 1 AndAlso aKeyName = ConstPRIMARYKEY Then
                anObjectID = rule.ResultingObjectnames.FirstOrDefault
                Dim i As UInteger = rule.Parameters.Count
                ''' assign the code piece
                ''' 
                resultCode.Code = Function(context) As Boolean
                                      Dim aKey As New ormDatabaseKey(objectid:=anObjectID, keyvalues:=context.PopParameters(i))
                                      Dim resultList As New List(Of iDataObject)
                                      resultList.Add(Me.Retrieve(key:=aKey, objectid:=anObjectID))
                                      context.Push(resultList)
                                      Return True
                                  End Function

                ''' build it from a secondary primary key
                ''' 
            ElseIf rule.ResultingObjectnames.Count = 1 AndAlso aKeyName IsNot Nothing Then
                Throw New NotImplementedException("preparing selections with secondary primary keys not supported")

                ''' build it from selection query
                ''' 
            ElseIf rule.ResultingObjectnames.Count = 1 AndAlso aKeyName Is Nothing Then
                anObjectID = rule.ResultingObjectnames.FirstOrDefault
                Dim i As UInteger = rule.Parameters.Count
                Dim keyindexes As UShort()
                ''' prepare through the database driver
                ''' 
                If aList.First.PrepareSelection(rule, resultCode:=resultCode) Then
                    ''' assign the code piece
                    ''' 
                    resultCode.Code = Function(context) As Boolean
                                          Dim theRecords As List(Of ormRecord) = aList.First.RetrieveBy(rule, context)

                                          Dim resultList As List(Of iDataObject) = Me.RetrieveFrom(theRecords, _
                                                                                             targetType:=anObjectDefinition.ObjectType, _
                                                                                             primaryKeyIndex:=keyindexes, _
                                                                                             objectid:=anObjectDefinition.Objectname,
                                                                                             domainbehavior:=anObjectDefinition.HasDomainBehavior, _
                                                                                             primaryKeyNames:=anObjectDefinition.PrimaryKeyEntryNames.ToList())
                                          context.Push(resultList)
                                          Return True
                                      End Function
                Else
                    ''' prepare Selection was not successfull
                    ''' 
                    CoreMessageHandler(message:="Preparing a selection rule '" & rule.ID & "' by a database driver '" & aList.First.ID & "' failed", argument:=aList.First.ID, _
                                        procedure:="ormDataObjectProvider.PrepareSelection", messagetype:=otCoreMessageType.InternalError, objectname:=anObjectID)
                    Return False
                End If

            End If

            Return True

        End Function
        ''' <summary>
        ''' retrieve by relation
        ''' </summary>
        ''' <param name="relationattribute"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function RetrieveByRelation(relationattribute As ormRelationAttribute, sourceobject As iormDataObject) As List(Of iormDataObject) Implements iormDataObjectProvider.RetrieveByRelation
            Dim dbdriver As iormDatabaseDriver
            Dim theObjectList As New List(Of iormDataObject)
            Dim theKeyvalues As New List(Of Object)
            ''' source
            Dim aSourceObjectDescriptor As ObjectClassDescription = ot.GetObjectClassDescription(sourceobject.ObjectID)
            If aSourceObjectDescriptor Is Nothing Then
                Throw New ormException(ormException.Types.NoObjectClassDescription, arguments:={sourceobject.ObjectID, Me.GetType.FullName})
                Return New List(Of iormDataObject)
            End If
            ''' target
            Dim aTargetObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=relationattribute.LinkObjectID)
            If aTargetObjectDefinition Is Nothing Then
                Throw New ormException(ormException.Types.TypeNotFound, arguments:={relationattribute.LinkObject.FullName})
            End If
            Dim aTargetType As System.Type = relationattribute.LinkObject

            ''' get target primary db driver
            If dbdriver Is Nothing Then dbdriver = CurrentSession.GetPrimaryDatabaseDriver(containerID:=aTargetObjectDefinition.PrimaryContainerID)
            If dbdriver Is Nothing Then
                Throw New ormException(ormException.Types.NoDatabaseDriver, arguments:={relationattribute.LinkObjectID, aTargetObjectDefinition.PrimaryContainerID})
            ElseIf Not dbdriver.IsRelationalDriver Then
                Throw New ormException(ormException.Types.WrongDriverType, arguments:={aTargetObjectDefinition.PrimaryContainerID, "relational"})
            End If
            Try

                ''' request a Rule and build it
                ''' 
                Dim aSelection As SelectionRule = CurrentSession.RulezEngine.GetSelectionRule(sourceobject.ObjectID & "_" & relationattribute.Name)
                Dim aContext As New Context(CurrentSession.RulezEngine)
                ''' retrieve the key values
                theKeyvalues = Reflector.GetContainerEntryValues(dataobject:=sourceobject, entrynames:=relationattribute.FromEntries)

                ''' build the selection rule
                If aSelection.RuleState <> otRuleState.generatedCode Then

                    '''' work on the query
                    '''' 

                    '** get the keys althoug determining if TOEntries are by Primarykey is a bit obsolete
                    If Not relationattribute.HasValueFromEntries OrElse Not relationattribute.HasValueToEntries Then
                        CoreMessageHandler(message:="relation attribute has nor fromEntries or ToEntries set", _
                                            argument:=relationattribute.Name, objectname:=sourceobject.ObjectID, _
                                             procedure:="ormDataObjectFactory.RetrieveByRelation", messagetype:=otCoreMessageType.InternalError)
                        Return theObjectList
                    ElseIf relationattribute.ToEntries.Count > relationattribute.FromEntries.Count Then
                        CoreMessageHandler(message:="relation attribute has nor mot ToEntries than FromEntries set", _
                                            argument:=relationattribute.Name, objectname:=sourceobject.ObjectID, _
                                             procedure:="ormDataObjectFactory.RetrieveByRelation", messagetype:=otCoreMessageType.InternalError)
                        Return theObjectList
                    End If

                    'If Not aTargetType.GetInterfaces.Contains(GetType(iormRelationalPersistable)) And Not aTargetType.GetInterfaces.Contains(GetType(iormInfusable)) Then
                    '    CoreMessageHandler(message:="target type has neither iormperistable nor iorminfusable interface", _
                    '                       argument:=relationattribute.Name, objectname:=sourceobject.ObjectID, _
                    '                        procedure:="ormDataObjectFactory.RetrieveByRelation", messagetype:=otCoreMessageType.InternalError)
                    '    Return theObjectList
                    'End If
                    ''***

                    ''' define the selection rule result set
                    ''' 
                    aSelection.Result = New ResultList({New DataObjectSymbol(id:=relationattribute.LinkObjectID)})

                    ''' define the selection rule parameters
                    ''' 
                    For i = 0 To relationattribute.FromEntries.Count - 1
                        Dim aName As String = relationattribute.FromEntries(i)
                        Dim aType As otDataType = CurrentSession.Objects.GetEntryDefinition(entryname:=aName, objectname:=relationattribute.LinkObjectID).Datatype
                        aSelection.AddNewParameter(aName, aType)
                    Next

                    ''' add the selection compare condition
                    ''' 
                    For i = 0 To relationattribute.ToEntries.Count - 1
                        With aSelection
                            Dim aDataObjectEntry As DataObjectEntrySymbol = New DataObjectEntrySymbol(objectid:=relationattribute.LinkObjectID, entryname:=relationattribute.ToEntries(i))
                            If i = 0 Then
                                .Selection = LogicalExpression.EQ(aDataObjectEntry, aSelection.Parameters(i))
                            Else
                                .Selection = .Selection.ANDALSO(LogicalExpression.EQ(aDataObjectEntry, aSelection.Parameters(i)))
                            End If
                        End With
                    Next

                    ''' add the behaviors
                    ''' 
                    If Not AddBehavior(aSelection, context:=aContext) Then Return theObjectList

                    ''' generate whatever is necessary to run
                    ''' 
                    If Not CurrentSession.RulezEngine.Generate(aSelection) Then
                        CoreMessageHandler(message:="Generating a selection rule '" & aSelection.ID & "' failed", _
                                           argument:=relationattribute.Name, objectname:=sourceobject.ObjectID, _
                                            messagetype:=otCoreMessageType.InternalError, _
                                           procedure:="ormDataObjectFactory.RetrieveByRelation")
                        Return theObjectList
                    End If
                End If

                ''' run the rule
                ''' 
                Dim aList As IEnumerable(Of iDataObject) = CurrentSession.RulezEngine.RunSelectionRule(aSelection.ID, theKeyvalues)
                ''' return the result list converted to iormDataObject
                ''' 
                theObjectList = aList.Select(Function(x) CType(x, iormDataObject))
                Return theObjectList

            Catch ex As RulezException
                CoreMessageHandler(message:=ex.Message, exception:=ex, messagetype:=otCoreMessageType.InternalException, _
                                    argument:=relationattribute.Name, objectname:=sourceobject.ObjectID, _
                                    procedure:="ormDataObjectFactory.RetrieveByRelation")
                Return theObjectList
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, _
                                    argument:=relationattribute.Name, objectname:=sourceobject.ObjectID, _
                                     procedure:="ormDataObjectFactory.RetrieveByRelation")
                Return theObjectList
            End Try




           
        End Function

        ''' <summary>
        ''' persists a dataobject
        ''' </summary>
        ''' <param name="dataobject"></param>
        ''' <param name="timestamp"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Persist(dataobject As iDataObject, Optional timestamp As Date? = Nothing) As Boolean Implements iormDataObjectProvider.Persist
            Return TryCast(dataobject, iormPersistable).Persist(timestamp)
        End Function
        ''' <summary>
        ''' deletes a dataobject
        ''' </summary>
        ''' <param name="dataobject"></param>
        ''' <param name="timestamp"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Delete(dataobject As iDataObject, Optional timestamp As Date? = Nothing) As Boolean Implements iormDataObjectProvider.Delete
            Return TryCast(dataobject, iormPersistable).Delete(timestamp)
        End Function
        ''' <summary>
        ''' persists a dataobject
        ''' </summary>
        ''' <param name="dataobject"></param>
        ''' <param name="timestamp"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Undelete(dataobject As iDataObject) As Boolean Implements iormDataObjectProvider.UnDelete
            Return TryCast(dataobject, iormPersistable).UnDelete()
        End Function
        ''' <summary>
        ''' persists a dataobject
        ''' </summary>
        ''' <param name="dataobject"></param>
        ''' <param name="timestamp"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Clone(dataobject As iDataObject, Optional key As iKey = Nothing) As iDataObject Implements iormDataObjectProvider.Clone
            Return TryCast(TryCast(dataobject, iormCloneable).Clone(key.Values), iDataObject)
        End Function

        ''' <summary>
        ''' check if the rule consists of a known key of the object. Returns the key / index name or nothing if not
        ''' </summary>
        ''' <param name="rule"></param>
        ''' <param name="aName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CheckKeyName(rule As SelectionRule, objectid As String) As String
            Dim anObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=objectid)
            If anObjectDefinition Is Nothing Then
                Throw New ormException(ormException.Types.NoObjectIDFound, arguments:={objectid})
            End If

            Dim aVisitor As New Visitor(Of Object)
            ''' define the Event
            Dim anEventHandler As Visitor(Of Object).Eventhandler = Function(o, e)
                                                                        ''' if an logical expression but no AND -> NO KEY SELECTION ! put false
                                                                        If e.CurrentNode.GetType Is GetType(LogicalExpression) Then
                                                                            If CType(e.CurrentNode, LogicalExpression).Operator.TokenID.ToInt <> Token.AND _
                                                                                AndAlso CType(e.CurrentNode, LogicalExpression).Operator.TokenID.ToInt <> Token.ANDALSO Then
                                                                                e.Stack.Push(False)
                                                                            End If

                                                                            '' push the id on the stack
                                                                        ElseIf e.CurrentNode.GetType Is GetType(DataObjectEntrySymbol) Then
                                                                            e.Stack.Push(CType(e.CurrentNode, DataObjectEntrySymbol).ID)
                                                                        End If
                                                                    End Function

            AddHandler aVisitor.VisitingExpression, anEventHandler
            ''' run the visitor
            aVisitor.Visit(rule.Selection)
            ''' get the unique list or give up if any other
            Dim aList As New List(Of String)
            For Each anObject In aVisitor.Stack.ToList()
                If anObject.GetType Is GetType(Boolean) AndAlso CBool(anObject) = False Then
                    Return Nothing
                ElseIf anObject.GetType() Is GetType(String) AndAlso Not aList.Contains(CStr(anObject)) Then
                    aList.Add(CStr(anObject))
                End If
            Next

            ''' now we have a unique list
            ''' 

            ''' check primary key
            ''' 
            Dim result As Boolean = True
            For Each anEntryname In anObjectDefinition.PrimaryKeyEntryNames
                If Not aList.Contains(anEntryname) Then
                    result = False
                    Exit For
                End If
            Next
            If result Then Return ConstPRIMARYKEY

            Return Nothing
        End Function

    End Class
End Namespace
