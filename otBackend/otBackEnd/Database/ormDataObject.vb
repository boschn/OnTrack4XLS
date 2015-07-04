
REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** ORM DATA OBJECT CLASS
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
Imports OnTrack.Core

Namespace OnTrack.Database
    ''' <summary>
    ''' abstract class for persistable OnTrack Data Objects
    ''' </summary>
    ''' <remarks>
    ''' Functional Design Principles
    ''' 1. a data object has a life cycle of initialized, created, loaded, deleted (must be set by derived classes)
    ''' 2. a data object is record bound
    ''' 3. a data object instance has a guid
    ''' 4. a data object has a domain id (might be overwritten by derived classes)
    ''' 5. a data object (derived class) has a object id and a class description
    ''' 6. a data object might be running in runtimeOnly mode (not persistable) -> mode might be also changed -> event raised
    ''' </remarks>
    Public MustInherit Class ormDataObject
        Implements iormDataObject
        Implements IDisposable

        ''' <summary>
        ''' guid as identity
        ''' </summary>
        ''' <remarks></remarks>
        Private _guid As Guid = Guid.NewGuid

        ''' <summary>
        ''' the Record
        ''' </summary>
        ''' <remarks></remarks>
        Protected WithEvents _record As ormRecord                   ' record to save persistency

        ''' <summary>
        ''' runtime only flag
        ''' </summary>
        ''' <remarks></remarks>
        Protected _RunTimeOnly As Boolean = False     'if Object is only kept in Memory (no persist, no Record according to table, no DBDriver necessary, no checkuniqueness)
        ''' <summary>
        ''' cache of the use cache property
        ''' </summary>
        ''' <remarks></remarks>
        Protected _useCache As Nullable(Of Boolean) 'cache variable of the ObjectDefinition.UseCache Property
        ''' <summary>
        ''' cache of the class description
        ''' </summary>
        ''' <remarks></remarks>
        Protected WithEvents _classDescription As ObjectClassDescription
        ''' <summary>
        ''' cache of the objectdefinition
        ''' </summary>
        ''' <remarks></remarks>
        Protected WithEvents _objectdefinition As ormObjectDefinition
        ''' <summary>
        ''' cache of the primary key
        ''' </summary>
        ''' <remarks></remarks>
        Protected _primarykey As ormDatabaseKey

        ''' <summary>
        ''' cache of primary databasedriver
        ''' </summary>
        ''' <remarks></remarks>
        Protected WithEvents _primarydatabasedriver As iormDatabaseDriver

        ''' <summary>
        ''' cache of the primarycontainterid
        ''' </summary>
        ''' <remarks></remarks>
        Protected _primaryContainerID As String
        ''' <summary>
        ''' tables for storing the record in 
        ''' </summary>
        ''' <remarks></remarks>
        Protected _containerids As String() = {}
        Protected _containerIsLoaded As Boolean() 'true if loaded
        ''' <summary>
        ''' IsInitialized flag
        ''' </summary>
        ''' <remarks></remarks>
        Protected _IsInitialized As Boolean = False 'true if initialized all internal members to run a persistable data object
        ''' <summary>
        ''' liefetime status and valiables
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        Protected _isloaded As Boolean = False
        Protected _isCreated As Boolean = False   'true if created by .CreateXXX Functions
        Protected _IsChanged As Boolean = False  'true if has changed and persisted is needed to retrieve the object as it is now
        <ormObjectEntryMapping(EntryName:=ConstFNIsDeleted)> Protected _IsDeleted As Boolean = False

        ''' <summary>
        ''' Timestamps
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNUpdatedOn)> Protected _updatedOn As Nullable(Of DateTime)
        <ormObjectEntryMapping(EntryName:=ConstFNCreatedOn)> Protected _createdOn As Nullable(Of DateTime)
        <ormObjectEntryMapping(EntryName:=ConstFNDeletedOn)> Protected _deletedOn As Nullable(Of DateTime)
        Protected _changedOn As Nullable(Of DateTime) 'Internal Timestamp which is used if an entry is changed

        ''' <summary>
        ''' Domain ID
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, _
           title:="Domain", description:="domain of the business Object", _
           defaultvalue:=ConstGlobalDomain, dbdefaultvalue:=ConstGlobalDomain, _
           useforeignkey:=otForeignKeyImplementation.NativeDatabase, _
           posordinal:=1000, _
           foreignkeyProperties:={ForeignKeyProperty.OnDelete & "(" & ForeignKeyActionProperty.Cascade & ")", _
                                   ForeignKeyProperty.OnUpdate & "(" & ForeignKeyActionProperty.Cascade & ")"})> _
        Public Const ConstFNDomainID = Domain.ConstFNDomainID

        <ormObjectEntryMapping(EntryName:=ConstFNDomainID)> Protected _domainID As String = ConstGlobalDomain

        <ormObjectEntry(Datatype:=otDataType.Bool, isnullable:=True, posordinal:=1001, _
          title:="Ignore Domain", description:="flag if the domainValue is to be ignored -> look in global")> _
        Public Const ConstFNIsDomainIgnored As String = "domainignore"

        <ormObjectEntryMapping(EntryName:=ConstFNIsDomainIgnored)> Protected _DomainIsIgnored As Boolean = False

        ''' <summary>
        ''' Member Entries to drive lifecycle
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Timestamp, isnullable:=True, isreadonly:=True, posordinal:=9901, _
           title:="Updated On", Description:="last update time stamp in the data store")> Public Const ConstFNUpdatedOn As String = ot.ConstFNUpdatedOn

        <ormObjectEntry(Datatype:=otDataType.Timestamp, isnullable:=True, isreadonly:=True, posordinal:=9902, _
            title:="Created On", Description:="creation time stamp in the data store")> Public Const ConstFNCreatedOn As String = ot.ConstFNCreatedOn

        <ormObjectEntry(Datatype:=otDataType.Timestamp, isnullable:=True, isreadonly:=True, posordinal:=9903, _
            title:="Deleted On", Description:="time stamp when the deletion flag was set")> Public Const ConstFNDeletedOn As String = ot.ConstFNDeletedOn

        '** Deleted flag
        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0", posordinal:=9904, _
            title:="Deleted", description:="flag if the entry in the data stored is regarded as deleted depends on the deleteflagbehavior")> _
        Public Const ConstFNIsDeleted As String = ot.ConstFNIsDeleted

        ''' <summary>
        ''' Events
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Event OnSwitchRuntimeOff(sender As Object, e As ormDataObjectEventArgs)
        Public Event OnSwitchRuntimeOn(sender As Object, e As ormDataObjectEventArgs)
        Public Event OnInitializing(sender As Object, e As ormDataObjectEventArgs)
        Public Event OnInitialized(sender As Object, e As ormDataObjectEventArgs)

#Region "Properties"
        ''' <summary>
        ''' Gets or sets the domain ID.
        ''' </summary>
        ''' <value>The domain ID.</value>
        Public Overridable Property DomainID() As String Implements iormDataObject.DomainID
            Get
                If Me.ObjectHasDomainBehavior Then
                    Return Me._domainID
                Else
                    Return CurrentSession.CurrentDomainID
                End If
            End Get
            Set(value As String)
                _domainID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the GUID for the Object.
        ''' </summary>
        ''' <value>T</value>
        Public ReadOnly Property Guid() As Guid Implements iormDataObject.GUID
            Get
                Return Me._guid
            End Get
        End Property
        ''' <summary>
        ''' True if a memory data object
        ''' </summary>
        ''' <value>The run time only.</value>
        Public ReadOnly Property RunTimeOnly() As Boolean Implements iormDataObject.RuntimeOnly
            Get
                Return Me._RunTimeOnly
            End Get
        End Property

        ''' <summary>
        ''' returns the object definition associated with this data object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectDefinition As iormObjectDefinition Implements iormDataObject.ObjectDefinition
            Get
                If _objectdefinition Is Nothing Then
                    _objectdefinition = CurrentSession.Objects.GetObjectDefinition(id:=Me.ObjectID)
                End If
                Return _objectdefinition
            End Get
        End Property
        ''' <summary>
        ''' gets the IObjectDefinition
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IObjectDefinition As iObjectDefinition Implements iDataObject.IObjectDefinition
            Get
                Return TryCast(Me.ObjectDefinition, iObjectDefinition)
            End Get
        End Property

        ''' <summary>
        ''' returns the object class description associated with this data object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectClassDescription As ObjectClassDescription Implements iormDataObject.ObjectClassDescription
            Get
                If _classDescription Is Nothing Then
                    _classDescription = ot.GetObjectClassDescription(Me.GetType)
                End If
                Return _classDescription
            End Get

        End Property

        ''' <summary>
        ''' return true if the data object is initialized
        ''' </summary>
        ''' <value>The PS is initialized.</value>
        Public Overridable ReadOnly Property IsInitialized() As Boolean Implements iormDataObject.IsInitialized
            Get
                Return Me._IsInitialized
            End Get
        End Property
        ''' <summary>
        ''' returns the ObjectID of the Class of this instance
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectID() As String Implements iormDataObject.ObjectID
            Get

                If Me.ObjectClassDescription IsNot Nothing Then
                    Return Me.ObjectClassDescription.ID
                Else
                    CoreMessageHandler("object id for orm data object class could not be found", argument:=Me.GetType.Name, _
                                        procedure:="ormDataObejct.ObjectID", messagetype:=otCoreMessageType.InternalError)
                End If
                Return Nothing
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the isDeleted.
        ''' </summary>
        ''' <value>The isDeleted.</value>
        Public Overridable ReadOnly Property IsDeleted() As Boolean Implements iormDataObject.IsDeleted
            Get
                Return Me._IsDeleted
            End Get

        End Property

        ''' <summary>
        ''' returns true if object has domain behavior
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectHasDomainBehavior As Boolean Implements iormDataObject.ObjectHasDomainBehavior
            Get
                ' do not initialize
                'If Not _IsInitialized AndAlso Not Initialize() Then
                '    CoreMessageHandler(message:="could not initialize object", subname:="ormDataObject.HasDomainBehavior")
                '    Return False
                'End If

                '** to avoid recursion loops for bootstrapping objects during 
                '** startup of session check these out and look into description
                If CurrentSession.IsBootstrappingInstallationRequested _
                    OrElse ot.GetBootStrapObjectClassIDs.Contains(Me.ObjectID) Then
                    Dim anObjectDecsription As ObjectClassDescription = ot.GetObjectClassDescriptionByID(Me.ObjectID)
                    If anObjectDecsription IsNot Nothing Then
                        Return anObjectDecsription.ObjectAttribute.AddDomainBehavior
                    Else
                        Return False
                    End If
                Else
                    Dim aObjectDefinition As ormObjectDefinition = Me.ObjectDefinition
                    If aObjectDefinition IsNot Nothing Then Return aObjectDefinition.HasDomainBehavior
                    Return False
                End If

            End Get

        End Property
        ''' <summary>
        ''' returns true if object is cached
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectUsesCache As Boolean Implements iormDataObject.ObjectUsesCache
            Get
                ' do not initialize
                'If Not _IsInitialized AndAlso Not Initialize() Then
                '    CoreMessageHandler(message:="could not initialize object", subname:="ormDataObject.UseCache")
                '    Return False
                'End If
                If _useCache.HasValue Then
                    Return _useCache
                Else
                    '** to avoid recursion loops for bootstrapping objects during 
                    '** startup of session check these out and look into description
                    If CurrentSession.IsBootstrappingInstallationRequested _
                        OrElse ot.GetBootStrapObjectClassIDs.Contains(Me.ObjectID) Then
                        Dim anObjectDecsription As ObjectClassDescription = ot.GetObjectClassDescriptionByID(Me.ObjectID)
                        If anObjectDecsription IsNot Nothing AndAlso anObjectDecsription.ObjectAttribute.HasValueUseCache Then
                            _useCache = anObjectDecsription.ObjectAttribute.UseCache
                        Else
                            _useCache = False
                        End If
                    Else
                        Dim aObjectDefinition As ormObjectDefinition = Me.ObjectDefinition
                        If aObjectDefinition IsNot Nothing Then Return aObjectDefinition.UseCache
                        _useCache = False
                    End If

                    Return _useCache
                End If

            End Get

        End Property
        ''' <summary>
        ''' returns true if object has delete per flag behavior
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectHasDeletePerFlagBehavior As Boolean Implements iormDataObject.ObjectHasDeletePerFlagBehavior
            Get
                ' do not initialize
                'If Not _IsInitialized AndAlso Not Initialize() Then
                '    CoreMessageHandler(message:="could not initialize object", subname:="ormDataObject.HasDeletePerFlagBehavior")
                '    Return False
                'End If
                '** avoid loops while starting up with bootstraps or during installation
                If CurrentSession.IsBootstrappingInstallationRequested OrElse ot.GetBootStrapObjectClassIDs.Contains(Me.ObjectID) Then
                    Dim anObjectDescription As ObjectClassDescription = ot.GetObjectClassDescriptionByID(id:=Me.ObjectID)
                    If anObjectDescription IsNot Nothing Then
                        Return anObjectDescription.ObjectAttribute.AddDeleteFieldBehavior
                    Else
                        Return False
                    End If
                Else
                    Dim aObjectDefinition As ormObjectDefinition = Me.ObjectDefinition
                    '** per flag
                    If aObjectDefinition IsNot Nothing Then Return aObjectDefinition.HasDeleteFieldBehavior
                End If

            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the changed property
        ''' </summary>
        ''' <value>The PS is changed.</value>
        Public Overridable Property IsChanged() As Boolean Implements iormDataObject.IsChanged
            Get
                Return Me._IsChanged
            End Get
            Protected Friend Set(value As Boolean)
                Me._IsChanged = value
                _changedOn = DateTime.Now
            End Set
        End Property
        ''' <summary>
        ''' Gets the changed property time stamp
        ''' </summary>
        ''' <value>The PS is changed.</value>
        Public ReadOnly Property ChangedOn() As DateTime? Implements iormDataObject.ChangedOn
            Get
                Return _changedOn
            End Get
        End Property
        ''' <summary>
        ''' True if the data object is loaded
        ''' </summary>
        ''' <value>.</value>
        Public Overridable ReadOnly Property IsLoaded() As Boolean Implements iormDataObject.IsLoaded
            Get
                If _containerIsLoaded IsNot Nothing AndAlso _containerIsLoaded.Length > 0 Then 'do not use alive since this might be recursive
                    For Each aFlag In _containerIsLoaded
                        If Not aFlag Then Return False
                    Next
                    Return True
                End If
                Return False
            End Get
        End Property
        ''' <summary>
        ''' returns the Object Tag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectTag() As String
            Get

                Return ConstDelimiter & Me.ObjectID.ToUpper & Core.DataType.ToString(Me.ObjectPrimaryKeyValues)
            End Get
        End Property


        ''' <summary>
        '''  returns True if the Object was Instanced by Create
        ''' </summary>
        ''' <value>The PS is created.</value>
        Public ReadOnly Property IsCreated() As Boolean Implements iormDataObject.IsCreated
            Get
                Return _isCreated
            End Get
        End Property

        ''' <summary>
        ''' returns the record
        ''' </summary>
        ''' <value>The record.</value>
        Public Property Record() As ormRecord Implements iormDataObject.Record
            Get
                Return Me._record
            End Get
            Set(value As ormRecord)
                If _record Is Nothing Then
                    Me._record = value
                Else
                    _record.Merge(value)
                End If
            End Set
        End Property
        ''' <summary>
        ''' returns an array of the primarykey entry names
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectPrimaryKeyEntrynames As String()
            Get
                Return Me.ObjectPrimaryKey.EntryNames
            End Get
        End Property

        ''' <summary>
        ''' returns the primaryKeyvalues
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property ObjectPrimaryKeyValues As Object() Implements iormDataObject.ObjectPrimaryKeyValues
            Get
                Return Me.ObjectPrimaryKey.Values
            End Get
        End Property
        ''' <summary>
        ''' returns the primary key
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property ObjectPrimaryKey As ormDatabaseKey Implements iormDataObject.ObjectPrimaryKey
            Get
                Return _primarykey
            End Get
        End Property
        ''' <summary>
        ''' returns the primary key as IKey
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property IPrimaryKey As iKey Implements iDataObject.PrimaryKey
            Get
                Return TryCast(Me.ObjectPrimaryKey, iKey)
            End Get
        End Property
        ''' <summary>
        ''' gets the Creation date in the persistence store
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property CreatedOn() As DateTime? Implements iormDataObject.CreatedOn
            Get
                Return _createdOn
            End Get
        End Property
        ''' <summary>
        ''' gets the last update date in the persistence store
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property UpdatedOn() As DateTime? Implements iormDataObject.UpdatedOn
            Get
                Return _updatedOn
            End Get
        End Property
        ''' <summary>
        ''' gets the deletion date in the persistence store
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property DeletedOn As DateTime? Implements iDataObject.DeletedOn
            Get
                Return _deletedOn
            End Get
        End Property

        ''' <summary>
        ''' returns the Version number of the Attribute set Persistance Version
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="name"></param>
        ''' <param name="dataobject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectClassVersion(dataobject As iormDataObject, Optional name As String = Nothing) As Long Implements iormDataObject.GetObjectClassVersion
            Dim aFieldList As System.Reflection.FieldInfo()

            Try
                '***
                '*** collect all the attributes first
                '***
                aFieldList = (dataobject.GetType).GetFields(Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic Or _
                                                  Reflection.BindingFlags.Public Or Reflection.BindingFlags.Static Or _
                                                  Reflection.BindingFlags.FlattenHierarchy)
                '** look into each Const Type (Fields)
                For Each aFieldInfo As System.Reflection.FieldInfo In aFieldList

                    If aFieldInfo.MemberType = Reflection.MemberTypes.Field Then
                        '** Attribtes
                        For Each anAttribute As System.Attribute In Attribute.GetCustomAttributes(aFieldInfo)
                            '** TABLE
                            If anAttribute.GetType().Equals(GetType(ormTableAttribute)) AndAlso String.IsNullOrWhiteSpace(name) Then
                                '** Schema Definition
                                Return (DirectCast(anAttribute, ormTableAttribute).Version)

                                '** FIELD COLUMN
                            ElseIf anAttribute.GetType().Equals(GetType(iormObjectEntryDefinition)) AndAlso Not String.IsNullOrWhiteSpace(name) Then
                                If name.ToLower = LCase(CStr(aFieldInfo.GetValue(dataobject))) Then
                                    Return DirectCast(anAttribute, iormObjectEntryDefinition).Version
                                End If

                                '** INDEX
                            ElseIf anAttribute.GetType().Equals(GetType(ormIndexAttribute)) Then
                                If name.ToLower = LCase(CStr(aFieldInfo.GetValue(dataobject))) Then
                                    Return DirectCast(anAttribute, ormIndexAttribute).Version
                                End If

                            End If

                        Next
                    End If
                Next

                Return 0

            Catch ex As Exception

                Call CoreMessageHandler(procedure:="ormDataObject.GetVersion(of T)", exception:=ex)
                Return 0

            End Try
        End Function

        ''' <summary>
        ''' gets underlying primary container ID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectPrimaryContainerID() As String Implements iormDataObject.ObjectPrimaryContainerID
            Get
                If String.IsNullOrWhiteSpace(_primaryContainerID) Then
                    _primaryContainerID = Me.ObjectClassDescription.ObjectAttribute.PrimaryContainerID
                End If

                Return _primaryContainerID
            End Get
        End Property
        ''' <summary>
        ''' returns the Containerts
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectContainerIDs As String() Implements iormDataObject.ObjectContainerIDs
            Get
                ''' to avoid loops get the description here
                If _containerids.Length = 0 Then
                    Dim anObjectDescription As ObjectClassDescription = Me.ObjectClassDescription
                    If anObjectDescription IsNot Nothing Then _containerids = anObjectDescription.ObjectAttribute.ContainerIDs
                    ReDim Preserve _containerIsLoaded(_containerids.GetUpperBound(0))
                End If

                Return _containerids
            End Get
        End Property
        ''' <summary>
        '''  gets the primary database driver of the underlying container
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectPrimaryDatabaseDriver As iormDatabaseDriver Implements iormDataObject.ObjectPrimaryDatabaseDriver
            Get
                If _primarydatabasedriver Is Nothing AndAlso Me.ObjectPrimaryContainerStore IsNot Nothing Then
                    _primarydatabasedriver = ot.CurrentSession.GetPrimaryDatabaseDriver(containerID:=Me.ObjectPrimaryContainerID)
                End If
                Return _primarydatabasedriver

            End Get
        End Property
        ''' <summary>
        '''  gets the  database driver stack of the underlying container
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectDatabaseDrivers As Stack(Of iormDatabaseDriver) Implements iormDataObject.ObjectDatabaseDrivers
            Get
                Return ot.CurrentSession.GetDatabaseDrivers(Me.ObjectPrimaryContainerID)
            End Get
        End Property
        ''' <summary>
        ''' Gets the primary container store.
        ''' </summary>
        ''' <value>The table store.</value>
        Public ReadOnly Property ObjectPrimaryContainerStore() As iormContainerStore Implements iormDataObject.ObjectPrimaryContainerStore
            Get
                If _record IsNot Nothing AndAlso _record.Alive AndAlso _record.ContainerStores IsNot Nothing AndAlso _record.ContainerStores.Count > 0 Then
                    Return _record.RetrieveContainerStore(Me.ObjectPrimaryContainerID)
                    ''' assume about the store to choose
                ElseIf Not Me.RunTimeOnly AndAlso Not String.IsNullOrEmpty(Me.ObjectPrimaryContainerID) Then
                    Dim aDatabaseDriver As iormDatabaseDriver = ot.CurrentSession.GetPrimaryDatabaseDriver(containerID:=Me.ObjectPrimaryContainerID)
                    If aDatabaseDriver IsNot Nothing Then Return aDatabaseDriver.RetrieveContainerStore(containerid:=Me.ObjectPrimaryContainerID)
                End If

                Return Nothing
            End Get
        End Property

#End Region


        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub New(Optional runtimeonly As Boolean = False, Optional objectID As String = Nothing)
            _IsInitialized = False
            _RunTimeOnly = runtimeonly
            If Not String.IsNullOrWhiteSpace(objectID) Then
                _classDescription = ot.GetObjectClassDescriptionByID(id:=objectID)
                If _classDescription Is Nothing Then
                    _classDescription = ot.GetObjectClassDescription(Me.GetType)
                End If
            End If
        End Sub
        ''' <summary>
        ''' clean up with the object
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Finialize()
            _IsInitialized = False
            _record = Nothing
        End Sub
        ''' <summary>
        ''' Performs application-defined tasks associated with freeing, releasing,
        ''' or resetting unmanaged resources.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            Finalize()
        End Sub
        ''' <summary>
        ''' Helper for Adding Handlers to SwitchRuntimeOff Event
        ''' </summary>
        ''' <param name="handler"></param>
        ''' <remarks></remarks>
        Public Sub AddSwitchRuntimeOffhandler(handler As [Delegate])
            AddHandler Me.OnSwitchRuntimeOff, handler
        End Sub

        ''' <summary>
        ''' set value to the entry
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function SetValue(entryname As String, ByVal value As Object) As Boolean Implements iormDataObject.SetValue
        ''' <summary>
        ''' get the value from an entry
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function GetValue(entryname As String) As Object Implements iormDataObject.GetValue
        ''' <summary>
        ''' Switch off the Runtime Mode
        ''' </summary>
        ''' <remarks></remarks>
        Public Function SwitchRuntimeOff() As Boolean
            If _RunTimeOnly Then
                Dim ourEventArgs As New ormDataObjectEventArgs(Me)
                ourEventArgs.Proceed = True
                ourEventArgs.Result = True
                RaiseEvent OnSwitchRuntimeOff(Me, ourEventArgs)
                '** no
                If Not ourEventArgs.Proceed Then Return ourEventArgs.Result
                '** proceed
                _RunTimeOnly = Not Me.Initialize(runtimeOnly:=False)
                Return Not _RunTimeOnly
            End If
            Return True
        End Function
        ''' <summary>
        ''' set the dataobject to Runtime
        ''' </summary>
        ''' <remarks></remarks>
        Protected Function SwitchRuntimeON() As Boolean
            If Not _RunTimeOnly Then
                Dim ourEventArgs As New ormDataObjectEventArgs(Me)
                ourEventArgs.Proceed = True
                ourEventArgs.Result = True
                RaiseEvent OnSwitchRuntimeOn(Me, ourEventArgs)
                '** no
                If Not ourEventArgs.Proceed Then Return ourEventArgs.Result
                _RunTimeOnly = True
            End If

        End Function
        ''' <summary>
        ''' copy the Primary key to the record
        ''' </summary>
        ''' <param name="pkArray"></param>
        ''' <remarks></remarks>
        Protected Function CopyPrimaryKeyToRecord(ByRef primarykey As ormDatabaseKey, ByRef record As ormRecord,
                                                Optional domainid As String = Nothing, _
                                                Optional runtimeOnly As Boolean = False) As Boolean

            Return primarykey.ToRecord(record:=record, objectclassdescription:=Me.ObjectClassDescription, domainid:=domainid, runtimeOnly:=runtimeOnly)
        End Function

        ''' <summary>
        ''' checks if the data object is alive
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsAlive(Optional subname As String = Nothing, Optional throwError As Boolean = True) As Boolean Implements iormDataObject.IsAlive
            If Not Me.IsLoaded And Not Me.IsCreated Then
                DetermineLiveStatus()
                '** check again
                If Not Me.IsLoaded And Not Me.IsCreated Then
                    If throwError Then
                        If String.IsNullOrWhiteSpace(subname) Then subname = "ormDataObject.checkalive"
                        If Not subname.Contains("."c) Then subname = Me.GetType.Name & "." & subname

                        CoreMessageHandler(message:="object is not alive but operation requested", objectname:=Me.GetType.Name, _
                                           procedure:=subname, messagetype:=otCoreMessageType.InternalError)
                    End If
                    Return False
                End If
            End If

            ''' success
            Return True
        End Function
        ''' <summary>
        ''' initialize the data object
        ''' </summary>
        ''' <returns>True if successfull</returns>
        ''' <remarks></remarks>
        Public Overridable Function Initialize(Optional runtimeOnly As Boolean = False) As Boolean Implements iormDataObject.Initialize


            '** is a session running ?!
            If Not runtimeOnly AndAlso Not CurrentSession.IsRunning AndAlso Not CurrentSession.IsStartingUp Then
                Call CoreMessageHandler(message:="data object cannot be initialized - start session to database first", _
                                           objectname:=Me.ObjectID, procedure:="ormDataobject.initialize", _
                                           messagetype:=otCoreMessageType.ApplicationError)
                Return False
            End If

            ''' set the runtime flag 
            _RunTimeOnly = runtimeOnly



            ''' fire event
            ''' 
            Dim ourEventArgs As New ormDataObjectEventArgs(Me, usecache:=Me.ObjectUsesCache, runtimeOnly:=runtimeOnly)
            RaiseEvent OnInitializing(Me, ourEventArgs)
            If ourEventArgs.AbortOperation Then
                Return False
            End If

            ''' set the properties which are not initializing by themselves -> or by the event
            ''' 
            ''' set the status of the containers if not infused already
            ''' 
            ''' the _objectcontainerids are set by the call to Me.ObjectContainerIDS
            If _containerIsLoaded Is Nothing OrElse _containerIsLoaded.Length = 0 Then
                ReDim Preserve _containerIsLoaded(Me.ObjectContainerIDs.GetUpperBound(0))
                '** set all tables to be unloaded
                ' Array.ForEach(Of Boolean)(_tableisloaded, Function(x) x = False) -> do not overwrite true
            End If
            ''' get new  record if necessary
            ''' STILL we rely on One Table for the Record
            If _record Is Nothing Then
                _record = New ormRecord(Me.ObjectContainerIDs, dbdriver:=Me.ObjectPrimaryDatabaseDriver, runtimeOnly:=runtimeOnly)
                'now we are not runtime only anymore -> set also the table and let's have a fixed structure
            ElseIf Not Me.RunTimeOnly AndAlso Not _record.IsBound Then
                _record.SetContainers(Me.ObjectContainerIDs, dbdriver:=Me.ObjectPrimaryDatabaseDriver)
            End If
            ''' set the return value
            Initialize = True

            ''' run on checks
            If Not _record.IsBound AndAlso Not Me.RunTimeOnly Then
                Call CoreMessageHandler(procedure:="ormDataObject.Initialize", message:="record is not set to container store", _
                                        messagetype:=otCoreMessageType.InternalError, containerID:=Me.Record.ContainerIDS.FirstOrDefault, noOtdbAvailable:=True)
                Initialize = False
            End If

            '*** check on connected status if not on runtime
            If Not Me.RunTimeOnly Then
                If _record.ContainerStores IsNot Nothing Then
                    For Each aTablestore In _record.ContainerStores
                        If Not aTablestore Is Nothing AndAlso Not aTablestore.Connection Is Nothing Then
                            If Not aTablestore.Connection.IsConnected AndAlso Not aTablestore.Connection.Session.IsBootstrappingInstallationRequested Then
                                Call CoreMessageHandler(procedure:="ormDataObject.Initialize", message:="TableStore is not connected to database / no connection available", _
                                                        messagetype:=otCoreMessageType.InternalError, noOtdbAvailable:=True)
                                Initialize = False
                            End If
                        End If
                    Next
                Else
                    Call CoreMessageHandler(procedure:="ormDataObject.Initialize", message:="TableStore is nothing in record", _
                                                       messagetype:=otCoreMessageType.InternalError, noOtdbAvailable:=True)
                    Initialize = False
                End If

            End If

            '* default values
            _IsDeleted = False

            '** fire event
            ourEventArgs = New ormDataObjectEventArgs(Me, record:=Me.Record, usecache:=Me.ObjectUsesCache, runtimeOnly:=runtimeOnly)
            ourEventArgs.Proceed = Initialize
            RaiseEvent OnInitialized(Me, ourEventArgs)
            '** set initialized
            _IsInitialized = ourEventArgs.Proceed
            Return Initialize
        End Function
        ''' <summary>
        ''' set one or all container or partial a container to be unloaded
        ''' </summary>
        ''' <param name="tableid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function SetUnloaded(Optional tableid As String = Nothing) As Boolean
            If _containerIsLoaded.Count = 0 Then
                Return False
            ElseIf tableid Is Nothing AndAlso Me.IsLoaded Then
                For i As UShort = _containerIsLoaded.GetLowerBound(0) To _containerIsLoaded.GetUpperBound(0)
                    _containerIsLoaded(i) = False
                Next
            ElseIf tableid IsNot Nothing Then
                For i As UShort = _containerIsLoaded.GetLowerBound(0) To _containerIsLoaded.GetUpperBound(0)
                    If Me.ObjectContainerIDs(i) = tableid Then _containerIsLoaded(i) = False
                Next
            End If
            Return True
        End Function
        ''' <summary>
        ''' set one or all containers or partial a container to be loaded
        ''' </summary>
        ''' <param name="tableid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Sub Setloaded(Optional containerID As String = Nothing)
            If _containerIsLoaded.Count = 0 Then
                Return
            ElseIf containerID Is Nothing Then
                For i As UShort = _containerIsLoaded.GetLowerBound(0) To _containerIsLoaded.GetUpperBound(0)
                    _containerIsLoaded(i) = True
                Next
                _isloaded = True
            ElseIf containerID IsNot Nothing Then
                For i As UShort = _containerIsLoaded.GetLowerBound(0) To _containerIsLoaded.GetUpperBound(0)
                    If Me.ObjectContainerIDs(i) = containerID Then _containerIsLoaded(i) = True
                Next
            End If
        End Sub
        ''' <summary>
        ''' sets the Livecycle status of this object if created or loaded
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DetermineLiveStatus() As Boolean Implements iormDataObject.DetermineLifeStatus
            ''' check the record again -> if infused by a record by sql selectment if have nor created not loaded
            If Me.IsInitialized Then
                '** check on the records
                _isCreated = Me.Record.IsCreated
                If Me.Record.IsLoaded Then
                    For Each atableid In Me.Record.ContainerIDS
                        Me.Setloaded(atableid)
                    Next
                End If
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' finalize
        ''' </summary>
        ''' <remarks></remarks>
        Protected Overrides Sub Finalize()
            MyBase.Finalize()
            _primarydatabasedriver = Nothing
        End Sub
    End Class
End Namespace

