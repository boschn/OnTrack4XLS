REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** embedded Object
REM *********** 
REM *********** Version: 2.0
REM *********** Created: 2015-04-13
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2015
REM ***********************************************************************************************************************************************
Option Explicit On
Imports System.Collections
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Diagnostics.Debug
Imports OnTrack.Core

Namespace OnTrack.Database

    ''' <summary>
    ''' embedded object class
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class persistableDataObject
        Inherits ormRelationalInfusable
        Implements iormPersistable

        ''' <summary>
        ''' Static Events
        ''' </summary>
        ''' <param name="persistableDataObject"></param>
        ''' <param name="ourEventArgs"></param>
        ''' <remarks></remarks>
        Public Shared Event ClassOnPersisting(persistableDataObject As iormPersistable, ourEventArgs As ormDataObjectEventArgs)
        Public Shared Event ClassOnPersisted(persistableDataObject As iormPersistable, ourEventArgs As ormDataObjectEventArgs)
        Public Shared Event ClassOnDeleting(persistableDataObject As iormPersistable, ourEventArgs As ormDataObjectEventArgs)
        Public Shared Event ClassOnCheckingUniqueness(persistableDataObject As iormPersistable, ourEventArgs As ormDataObjectEventArgs)
        Public Shared Event ClassOnDeleted(persistableDataObject As iormPersistable, ourEventArgs As ormDataObjectEventArgs)
        Public Shared Event ClassOnUnDeleted(persistableDataObject As iormPersistable, ourEventArgs As ormDataObjectEventArgs)
        Public Shared Event ClassOnOverloaded(persistableDataObject As iormPersistable, ourEventArgs As ormDataObjectOverloadedEventArgs)


        ''' <summary>
        ''' Events
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Event OnInjecting As iormPersistable.OnInjectingEventHandler Implements iormPersistable.OnInjecting

        ''' <summary>
        ''' Occurs when [on injected].
        ''' </summary>
        Public Event OnInjected As iormPersistable.OnInjectedEventHandler Implements iormPersistable.OnInjected

        ''' <summary>
        ''' Occurs when [on persisting].
        ''' </summary>
        Public Event OnPersisting As iormPersistable.OnPersistingEventHandler Implements iormPersistable.OnPersisting

        ''' <summary>
        ''' Occurs when [on persisted].
        ''' </summary>
        Public Event OnPersisted As iormPersistable.OnPersistedEventHandler Implements iormPersistable.OnPersisted

        ''' <summary>
        ''' Occurs when [on un deleting].
        ''' </summary>
        Public Event OnUnDeleting As iormPersistable.OnUnDeletingEventHandler Implements iormPersistable.OnUnDeleting

        ''' <summary>
        ''' Occurs when [on un deleted].
        ''' </summary>
        Public Event OnUnDeleted As iormPersistable.OnUnDeletedEventHandler Implements iormPersistable.OnUnDeleted

        ''' <summary>
        ''' Occurs when [on deleting].
        ''' </summary>
        Public Event OnDeleting As iormPersistable.OnDeletingEventHandler Implements iormPersistable.OnDeleting

        ''' <summary>
        ''' Occurs when [on deleted].
        ''' </summary>
        Public Event OnDeleted As iormPersistable.OnDeletedEventHandler Implements iormPersistable.OnDeleted

        ''' <summary>
        ''' Occurs when [on creating].
        ''' </summary>
        Public Event OnCreating As iormPersistable.OnCreatingEventHandler Implements iormPersistable.OnCreating

        ''' <summary>
        ''' Occurs when [on created].
        ''' </summary>
        Public Event OnCreated As iormPersistable.OnCreatedEventHandler Implements iormPersistable.OnCreated

        ''' <summary>
        ''' triggered if the create operation needs default values
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Event OnCreateDefaultValuesNeeded As iormPersistable.OnCreateDefaultValuesNeededEventHandler Implements iormPersistable.OnCreateDefaultValuesNeeded

        ''' <summary>
        ''' occurs onvalidationneeded
        ''' </summary>
        ''' <param name="persistableDataObject"></param>
        ''' <param name="validationEventArgs"></param>
        ''' <remarks></remarks>
        Public Event OnValidationNeeded(persistableDataObject As iormPersistable, validationEventArgs As ormDataObjectValidationEventArgs) Implements iormPersistable.OnValidationNeeded


        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub New(Optional runtimeonly As Boolean = False, Optional objectID As String = Nothing)
            MyBase.New(runtimeonly:=runtimeonly, objectID:=objectID)
        End Sub

        ''' <summary>
        ''' clean up with the object
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Finialize()
            MyBase.Finalize()
        End Sub

        
        ''' <summary>
        ''' create a dataobject from a type
        ''' </summary>
        ''' <param name="pkArray"></param>
        ''' <param name="type"></param>
        ''' <param name="domainID"></param>
        ''' <param name="checkUnique"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateDataObject(primarykey As ormDatabaseKey, type As System.Type, _
                                  Optional domainID As String = Nothing,
                                  Optional checkUnique As Boolean? = Nothing, _
                                  Optional runtimeOnly As Boolean? = Nothing) As iormPersistable
            Dim aFactory As iormDataObjectProvider = CurrentSession.Objects(domainid:=domainID).GetDataObjectProvider(type)
            ''' get the data object from the factory
            Return aFactory.Create(primarykey:=primarykey, type:=type, domainID:=domainID, checkUnique:=checkUnique, runTimeonly:=runtimeOnly)
        End Function
        ''' <summary>
        ''' Create a Dataobject of Type T with a primary key with values in order of the array
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="pkArray"></param>
        ''' <param name="domainID"></param>
        ''' <param name="checkUnique"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateDataObject(Of T As {iormPersistable, New}) _
                           (ByRef pkArray() As Object,
                            Optional domainID As String = Nothing,
                            Optional checkUnique As Boolean? = Nothing, _
                            Optional runtimeOnly As Boolean? = Nothing) As iormPersistable
            
            Dim aFactory As iormDataObjectProvider = CurrentSession.Objects(domainid:=domainID).GetDataObjectProvider(GetType(T))
            Dim aPrimaryKey As New ormDatabaseKey(objectid:=CurrentSession.Objects(domainid:=domainID).GetObjectname(GetType(T)), _
                                                  keyvalues:=pkArray)
            ''' get the data object from the factory
            Return aFactory.Create(primarykey:=aPrimaryKey, type:=GetType(T), domainID:=domainID, checkUnique:=checkUnique, runTimeonly:=runtimeOnly)
        End Function
        ''' <summary>
        ''' create a persistable dataobject of type T 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="pkArray"></param>
        ''' <param name="checkUnique"></param>
        ''' <returns>the iotdbdataobject or nothing (if checkUnique)</returns>
        ''' <remarks></remarks>

        Public Shared Function CreateDataObject(Of T As {iormInfusable, iormPersistable, New}) _
                            (ByRef primarykey As ormDatabaseKey,
                             Optional domainID As String = Nothing,
                             Optional checkUnique As Boolean? = Nothing, _
                             Optional runtimeOnly As Boolean? = Nothing) As iormPersistable

            Dim aFactory As iormDataObjectProvider = CurrentSession.Objects(domainid:=domainID).GetDataObjectProvider(GetType(T))

            ''' get the data object from the factory
            Return aFactory.Create(primarykey:=primarykey, type:=GetType(T), domainID:=domainID, checkUnique:=checkUnique, runTimeonly:=runtimeOnly)
        End Function
        ''' <summary>
        ''' create a persistable dataobject of type T out of data of a record
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="pkArray"></param>
        ''' <param name="checkUnique"></param>
        ''' <returns>iormRelationalPersistable</returns>
        ''' <remarks></remarks>
        Public Shared Function CreateDataObject(Of T As {iormPersistable, New}) _
                            (ByRef record As ormRecord,
                             Optional domainID As String = Nothing,
                             Optional checkUnique As Boolean? = Nothing, _
                             Optional runtimeOnly As Boolean? = Nothing) As iormPersistable
            Dim aFactory As iormDataObjectProvider = CurrentSession.Objects(domainid:=domainID).GetDataObjectProvider(GetType(T))
            ''' get the data object from the factory
            Return aFactory.Create(record:=record, type:=GetType(T), domainID:=domainID, checkUnique:=checkUnique, runtimeOnly:=runtimeOnly)
        End Function
        ''' <summary>
        ''' helper for checking the uniqueness during creation
        ''' </summary>
        ''' <param name="pkarray"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function CheckUniqueness(primarykey As ormDatabaseKey, record As ormRecord, Optional runtimeOnly As Boolean = False) As Boolean

            '*** Check on Not Runtime
            If Not runtimeOnly OrElse Me.ObjectUsesCache Then
                Dim aRecord As ormRecord
                '* fire Event and check uniqueness in cache if we have one
                Dim ourEventArgs = New ormDataObjectEventArgs(Me, record:=record, key:=primarykey, usecache:=Me.ObjectUsesCache, runtimeOnly:=runtimeOnly)
                RaiseEvent ClassOnCheckingUniqueness(Me, ourEventArgs)

                '* skip
                If ourEventArgs.Proceed AndAlso Not runtimeOnly Then
                    ' Check
                    Dim aStore As iormContainerStore = Me.ObjectPrimaryContainerStore
                    aRecord = aStore.GetRecordByPrimaryKey(primarykey.Values)

                    '* not found
                    If aRecord IsNot Nothing Then
                        If Me.ObjectHasDeletePerFlagBehavior Then
                            If aRecord.HasIndex(ConstFNIsDeleted) Then
                                If CBool(aRecord.GetValue(ConstFNIsDeleted)) Then
                                    CoreMessageHandler(message:="deleted (per flag) object found - use undelete instead of create", messagetype:=otCoreMessageType.ApplicationWarning, _
                                                        argument:=primarykey.Values, containerID:=Me.ObjectPrimaryContainerID)
                                    Return False
                                End If
                            End If
                        Else
                            Return False
                        End If

                    Else
                        '** use the result to check record on uniqueness
                        record.IsCreated = True
                        Return True ' unqiue
                    End If

                    Return False ' not unique
                Else
                    Return ourEventArgs.Proceed
                End If

            Else

                Return True ' if runTimeOnly only the Cache could be checked
            End If

        End Function


        ''' <summary>
        ''' generic function to create a data object by  a record
        ''' </summary>
        ''' <param name="pkArray"></param>
        ''' <param name="domainID" > optional domain ID for domain behavior</param>
        ''' <param name="dataobject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Create(ByRef record As ormRecord, _
                                              Optional domainID As String = Nothing, _
                                              Optional checkUnique As Boolean? = Nothing, _
                                              Optional runtimeOnly As Boolean? = Nothing) As Boolean Implements iormPersistable.Create

            ''' defautl values
            If String.IsNullOrEmpty(domainID) Then domainID = CurrentSession.CurrentDomainID
            If Not checkUnique.HasValue Then checkUnique = True
            If Not runtimeOnly.HasValue Then runtimeOnly = False
            '** is a session running ?!
            If Not runtimeOnly AndAlso Not CurrentSession.IsRunning AndAlso Not CurrentSession.IsStartingUp Then
                Call CoreMessageHandler(message:="data object cannot be created - start session to database first", _
                                           objectname:=Me.ObjectID, _
                                           messagetype:=otCoreMessageType.ApplicationError)
                Return False
            End If

            '** initialize
            If Not Me.IsInitialized AndAlso Not Me.Initialize(runtimeOnly:=runtimeOnly) Then
                Call CoreMessageHandler(message:="dataobject can not be initialized", containerID:=_primaryContainerID, argument:=record.ToString, _
                                        procedure:="PersistableDataObject.create", messagetype:=otCoreMessageType.InternalError)

                Return False
            End If
            '** is the object loaded -> no reinit
            If Me.IsLoaded Then
                Call CoreMessageHandler(message:="data object cannot be created if it has state loaded", objectname:=Me.ObjectID, containerID:=_primaryContainerID, argument:=record.ToString, _
                                        procedure:="PersistableDataObject.create", messagetype:=otCoreMessageType.InternalError)
                Return False
            End If


            '**
            Dim aPrimaryKey As ormDatabaseKey

            '** domainid
            If String.IsNullOrEmpty(domainID) Then domainID = ConstGlobalDomain

            '** fire event
            Dim ourEventArgs As New ormDataObjectEventArgs(record:=record, object:=Me, infuseMode:=otInfuseMode.OnCreate, _
                                                           usecache:=Me.ObjectUsesCache, runtimeonly:=runtimeOnly)
            RaiseEvent OnCreating(Me, ourEventArgs)
            If ourEventArgs.AbortOperation Then
                If ourEventArgs.Result Then record = ourEventArgs.Record
                Return ourEventArgs.Proceed
            ElseIf ourEventArgs.Result Then
                record = ourEventArgs.Record
            End If

            '** leave the primary key extraction here after

            '* extract the primary key
            aPrimaryKey = _record.ToPrimaryKey(objectID:=Me.ObjectID, runtimeOnly:=runtimeOnly)
            '** check for domainBehavior
            aPrimaryKey.ChecknFix(domainid:=domainID, runtimeOnly:=runtimeOnly)

            '** keys must be set in the object itself
            '** create
            _UniquenessInStoreWasChecked = Not runtimeOnly And checkUnique ' remember
            If checkUnique AndAlso Not CheckUniqueness(primarykey:=aPrimaryKey, record:=record, runtimeOnly:=runtimeOnly) Then
                Return False '* not unique
            End If

            '** set on the runtime Only Flag
            If runtimeOnly Then SwitchRuntimeON()

            '''
            ''' raise the Default Values Needed Event
            ''' 
            RaiseEvent OnCreateDefaultValuesNeeded(Me, ourEventArgs)
            If ourEventArgs.Result Then
                record = ourEventArgs.Record
            End If
            ''' set default values
            If Me.ObjectHasDomainBehavior Then
                If Not record.HasIndex(ConstFNDomainID) OrElse String.IsNullOrWhiteSpace(record.GetValue(ConstFNDomainID)) Then
                    record.SetValue(ConstFNDomainID, domainID)
                End If
            End If


            ''' set the record (and merge with property assignement)
            ''' 
            If _record Is Nothing Then
                _record = record
            Else
                _record.Merge(record)
            End If

            ''' infuse what we have in the record
            ''' 
            Dim aDataobject = Me

            If Not InfuseDataObject(record:=record, dataobject:=aDataobject, mode:=otInfuseMode.OnCreate) Then
                CoreMessageHandler(message:="InfuseDataobject failed", messagetype:=otCoreMessageType.InternalError, procedure:="PersistableDataObject.Create")
                If aDataobject.Guid <> Me.Guid Then
                    CoreMessageHandler(message:="data object was substituted in instance create function during infuse ?!", messagetype:=otCoreMessageType.InternalWarning, _
                        procedure:="PersistableDataObject.Create")
                End If
            End If

            '** set status
            _domainID = domainID
            _isCreated = True
            _IsDeleted = False
            Me.SetUnloaded()
            _IsChanged = False

            '* fire Event
            ourEventArgs = New ormDataObjectEventArgs(Me, record:=Me.Record, _
                                                      key:=aPrimaryKey, _
                                                      usecache:=Me.ObjectUsesCache, _
                                                      infuseMode:=otInfuseMode.OnCreate, _
                                                      runtimeonly:=runtimeOnly)
            RaiseEvent OnCreated(Me, ourEventArgs)

            Return ourEventArgs.Proceed

        End Function


        ''' <summary>
        ''' <summary>
        ''' injects a new instance a dataobject and infuses it
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="pkArray"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Shared Function InjectDataObject(ByRef primarykey As ormDatabaseKey, type As System.Type, _
                                                                     Optional domainid As String = Nothing, _
                                                                     Optional dbdriver As iormDatabaseDriver = Nothing) As iormPersistable
            Dim aFactory As iormDataObjectProvider = CurrentSession.Objects(domainid:=domainid).GetDataObjectProvider(type)
            Dim aDataObject As iormPersistable = TryCast(aFactory.NewOrmDataObject(type), iormPersistable)

            If aDataObject.Inject(primarykey, domainid:=domainid, dbdriver:=dbdriver) Then
                Return aDataObject
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' injects a new  iormpersistable DataObject by Type
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="pkArray"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Shared Function InjectDataObject(Of T As {iormInfusable, iormPersistable, New})(ByRef primarykey As ormDatabaseKey, _
                                                                                               Optional domainid As String = Nothing, _
                                                                                                Optional dbdriver As iormDatabaseDriver = Nothing) As T
            Return InjectDataObject(primarykey:=primarykey, type:=GetType(T), domainid:=domainid, dbdriver:=dbdriver)
        End Function

        ''' <summary>
        ''' injects retrieving records from the datastores and infuses the object from the inside out
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overridable Function Inject(ByRef primarykey As ormDatabaseKey, _
                                           Optional domainid As String = Nothing, _
                                           Optional dbdriver As iormDatabaseDriver = Nothing, _
                                           Optional loadDeleted As Boolean = False) As Boolean Implements iormPersistable.Inject
            Dim aRecord As ormRecord
            Dim aStore As iormContainerStore
            Dim ourEventArgs As ormDataObjectEventArgs
            Dim anewDataobject As iormPersistable = Me

            '* init
            If Not Me.IsInitialized AndAlso Not Me.Initialize Then
                Return False
            End If

            Try
                _RunTimeOnly = False

                ''' fix the primary key
                ''' 
                primarykey.ChecknFix(domainid:=domainid, runtimeOnly:=RunTimeOnly)

                '** fire event
                ourEventArgs = New ormDataObjectEventArgs(Me, record:=aRecord, key:=primarykey, infusemode:=otInfuseMode.OnInject, runtimeOnly:=Me.RunTimeOnly)
                ourEventArgs.UseCache = Me.ObjectUsesCache
                RaiseEvent OnInjecting(Me, ourEventArgs)
                If ourEventArgs.AbortOperation Then
                    If ourEventArgs.Result Then
                        Me.Record = ourEventArgs.Record
                    End If
                    '** reset the infuse mode
                    Return ourEventArgs.Result
                ElseIf ourEventArgs.Result Then
                    primarykey = ourEventArgs.Key
                    aRecord = ourEventArgs.Record
                End If

                ''' How to Inject
                '''
                Dim UseView As Boolean = False
                Dim retrieveViewID As String = Me.ObjectDefinition.RetrieveObjectFromViewID
                If Not String.IsNullOrWhiteSpace(retrieveViewID) Then UseView = True
                
                ''' check how many tables to inject from -> get the record
                ''' 
                If Me.ObjectContainerIDs.Count = 1 AndAlso Not UseView Then
                    If dbdriver Is Nothing Then dbdriver = Me.ObjectPrimaryDatabaseDriver
                    aStore = dbdriver.RetrieveContainerStore(Me.ObjectPrimaryContainerID)

                    ''' the primary table is always loaded with the pkarray
                    ''' 
                    aRecord = aStore.GetRecordByPrimaryKey(primarykey.Values)

                ElseIf Me.ObjectContainerIDs.Count > 1 Then

                    ''' check if injecting from a view
                    If UseView AndAlso dbdriver.GetType.GetInterfaces.Contains(GetType(iormRelationalDatabaseDriver)) Then
                        aStore = CType(dbdriver, iormRelationalDatabaseDriver).GetViewReader(CType(Me.ObjectDefinition, ormObjectDefinition).RetrieveObjectFromViewID)
                        aRecord = aStore.GetRecordByPrimaryKey(primarykey.Values)

                    Else
                        ''' not implemented -> load from multiple tables
                        ''' 
                        Throw New NotImplementedException("not implemented to load from multiple containers")

                    End If
                End If

                '* still nothing ?!

                If aRecord Is Nothing Then
                    Me.SetUnloaded()
                    Return False
                Else
                    '* what about deleted objects
                    If Me.ObjectHasDeletePerFlagBehavior Then
                        If aRecord.HasIndex(ConstFNIsDeleted) Then
                            If CBool(aRecord.GetValue(ConstFNIsDeleted)) Then
                                _IsDeleted = True
                                '* load only on deleted
                                If Not loadDeleted Then
                                    Me.SetUnloaded()
                                    _isCreated = False
                                    Return False
                                End If
                            Else
                                _IsDeleted = False
                            End If
                        Else
                            CoreMessageHandler(message:="object has delete per flag behavior but no flag", messagetype:=otCoreMessageType.InternalError, _
                                                procedure:="PersistableDataObject.Inject", containerID:=Me.ObjectPrimaryContainerID, entryname:=ConstFNIsDeleted)
                            _IsDeleted = False
                        End If
                    Else
                        _IsDeleted = False
                    End If

                    ''' INFUSE THE OBJECT (partially) from the record
                    ''' 

                    If InfuseDataObject(record:=aRecord, dataobject:=anewDataobject, mode:=otInfuseMode.OnInject) Then
                        If Me.Guid <> anewDataobject.GUID Then
                            CoreMessageHandler(message:="object was substituted during infuse", messagetype:=otCoreMessageType.InternalError, _
                                                procedure:="PersistableDataObject.Inject", containerID:=Me.ObjectPrimaryContainerID, objectname:=Me.ObjectID)
                            Return False
                        End If

                        '** set all tables to be loaded
                        ''' Array.ForEach(Of Boolean)(_tableisloaded, Function(x) x = True) -> in .infuse method
                        '** set the primary keys
                        ''' _primarykey = primarykey -> in .infuse method
                    Else
                        CoreMessageHandler(message:="unable to inject a new data object from record", messagetype:=otCoreMessageType.InternalError, _
                                            procedure:="PersistableDataObject.Inject", containerID:=Me.ObjectPrimaryContainerID, objectname:=Me.ObjectID)
                        Return False
                    End If


                End If


                '** fire event
                ourEventArgs = New ormDataObjectEventArgs(anewDataobject, record:=Me.Record, key:=primarykey, infuseMode:=otInfuseMode.OnInject, runtimeOnly:=Me.RunTimeOnly)
                ourEventArgs.Proceed = Me.IsLoaded
                ourEventArgs.UseCache = Me.ObjectUsesCache
                RaiseEvent OnInjected(Me, ourEventArgs)

                If ourEventArgs.Proceed Then
                    _isCreated = False
                    _IsChanged = False
                End If
                '** return
                Return Me.IsLoaded
            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="PersistableDataObject.Inject", argument:=primarykey, containerID:=_primaryContainerID)
                Return False
            End Try


        End Function

        ''' <summary>
        ''' Undelete the data object
        ''' </summary>
        ''' <returns>True if successful</returns>
        ''' <remarks></remarks>
        Public Function Undelete() As Boolean Implements iormPersistable.UnDelete
            If Not Me.IsInitialized AndAlso Not Me.Initialize Then
                Return False
            End If

            '* fire event
            Dim ourEventArgs As New ormDataObjectEventArgs(Me, record:=Me.Record, key:=Me.ObjectPrimaryKey, runtimeOnly:=Me.RunTimeOnly)
            RaiseEvent OnUnDeleting(Me, ourEventArgs)
            If ourEventArgs.AbortOperation Then
                Return ourEventArgs.Result
            End If

            '* undelete if possible
            Dim aObjectDefinition As ormObjectDefinition = Me.ObjectDefinition
            If aObjectDefinition IsNot Nothing AndAlso aObjectDefinition.HasDeleteFieldBehavior Then
                _IsDeleted = False
                _deletedOn = Nothing
                '* fire event
                ourEventArgs = New ormDataObjectEventArgs(Me, record:=Me.Record, _
                                                          key:=_record.ToPrimaryKey(objectID:=Me.ObjectID, runtimeOnly:=Me.RunTimeOnly), _
                                                           runtimeOnly:=Me.RunTimeOnly, usecache:=Me.ObjectUsesCache)
                ourEventArgs.Result = True
                ourEventArgs.Proceed = True
                RaiseEvent OnUnDeleted(Me, ourEventArgs)
                'If ourEventArgs.AbortOperation Then
                '    Return ourEventArgs.Result
                'End If
                RaiseEvent ClassOnUnDeleted(Me, ourEventArgs)
                If ourEventArgs.Result Then
                    CoreMessageHandler(message:="data object undeleted", procedure:="persistableDataObject.undelete", messagetype:=otCoreMessageType.InternalInfo, _
                                        containerID:=Me.ObjectPrimaryContainerID)
                    Return True
                Else
                    CoreMessageHandler(message:="data object cannot be undeleted by event - delete per flag behavior not set", procedure:="persistableDataObject.undelete", messagetype:=otCoreMessageType.InternalInfo, _
                                     containerID:=Me.ObjectPrimaryContainerID)
                    Return False
                End If

            Else
                CoreMessageHandler(message:="data object cannot be undeleted - delete per flag behavior not set", procedure:="persistableDataObject.undelete", messagetype:=otCoreMessageType.InternalInfo, _
                                     containerID:=Me.ObjectPrimaryContainerID)
                Return False
            End If


        End Function

        ''' <summary>
        ''' Delete the object and its persistancy
        ''' </summary>
        ''' <returns>True if successfull</returns>
        ''' <remarks></remarks>
        Public Overridable Function Delete(Optional timestamp As DateTime? = Nothing) As Boolean Implements iormPersistable.Delete

            '** initialize -> no error if not alive
            If Not Me.IsAlive(throwError:=False) Then Return False
            If Not timestamp.HasValue OrElse timestamp = constNullDate Then timestamp = DateTime.Now

            '** Fire Event
            Dim ourEventArgs As New ormDataObjectEventArgs(Me, record:=Me.Record, key:=Me.ObjectPrimaryKey, _
                                                           usecache:=Me.ObjectUsesCache, runtimeOnly:=Me.RunTimeOnly, timestamp:=timestamp)
            RaiseEvent ClassOnDeleting(Me, ourEventArgs)
            RaiseEvent OnDeleting(Me, ourEventArgs)
            If ourEventArgs.AbortOperation Then
                Return ourEventArgs.Result
            End If

            '*** cascade the operation through the related members
            Dim result As Boolean = Me.CascadeRelations(cascadeDelete:=True)

            If result Then
                '** determine how to delete
                Dim aObjectDefinition As ormObjectDefinition = Me.ObjectDefinition
                '** per flag
                If aObjectDefinition IsNot Nothing AndAlso aObjectDefinition.HasDeleteFieldBehavior Then
                    _IsDeleted = True
                    _deletedOn = timestamp
                    Feed()
                    '** save only on the record level
                    If Me.IsLoaded AndAlso Not Me.RunTimeOnly Then _IsDeleted = _record.Persist(timestamp)
                Else
                    'delete the  object itself
                    If Not Me.RunTimeOnly AndAlso Me.IsLoaded Then _IsDeleted = _record.Delete()
                    If _IsDeleted Then
                        Me.SetUnloaded()
                        _deletedOn = timestamp
                    End If

                End If

                '** fire Event
                ourEventArgs.Result = _IsDeleted
                RaiseEvent OnDeleted(Me, ourEventArgs)
                RaiseEvent ClassOnDeleted(Me, ourEventArgs)
                Return _IsDeleted
            Else
                CoreMessageHandler("object could not delete  cascaded objected", procedure:="PersistableDataObject.Delete", objectname:=Me.ObjectID, _
                                   argument:=Converter.Array2StringList(Me.ObjectPrimaryKeyValues))
                Return False
            End If

        End Function
        ''' <summary>
        ''' Persist the object to the datastore
        ''' </summary>
        ''' <param name="timestamp"></param>
        ''' <returns>True if successfull</returns>
        ''' <remarks></remarks>
        Public Overridable Function Persist(Optional timestamp As DateTime? = Nothing, Optional doFeedRecord As Boolean = True) As Boolean Implements iormPersistable.Persist

            '* init
            If Not Me.IsInitialized AndAlso Not Me.Initialize() Then Return False
            '** must be alive from data store
            If Not IsAlive(subname:="Persist") Then Return False
            If Not timestamp.HasValue OrElse timestamp = constNullDate Then timestamp = DateTime.Now

            '''
            ''' object on runtime -> no save
            ''' 
            If Me.RunTimeOnly Then
                CoreMessageHandler(message:="object on runtime could not be persisted", messagetype:=otCoreMessageType.InternalWarning, _
                                 procedure:="PersistableDataObject.Persist", dataobject:=Me)
                Return False
            End If

            '''
            ''' record must be alive
            ''' 
            If Not Me.Record.Alive Then
                CoreMessageHandler(message:="record is not alive in data object - cannot persist", messagetype:=otCoreMessageType.InternalError, _
                                   procedure:="PersistableDataObject.Persist", objectname:=Me.ObjectID, containerID:=Me.ObjectPrimaryContainerID)
                Return False
            End If
            '**
            Try
                '* if object was deleted an its now repersisted
                Dim isdeleted As Boolean = _IsDeleted
                _IsDeleted = False

                '** fire event
                Dim ourEventArgs As New ormDataObjectEventArgs(Me, record:=Me.Record, key:=Me.ObjectPrimaryKey, _
                                                               timestamp:=timestamp, usecache:=Me.ObjectUsesCache, domainID:=DomainID, _
                                                               domainBehavior:=Me.ObjectHasDomainBehavior, runtimeOnly:=Me.RunTimeOnly)
                RaiseEvent ClassOnPersisting(Me, ourEventArgs)
                If ourEventArgs.AbortOperation Then
                    Return False
                Else
                    _record = ourEventArgs.Record
                End If

                '** fire event
                RaiseEvent OnPersisting(Me, ourEventArgs)
                If ourEventArgs.AbortOperation Then
                    Return False
                Else
                    _record = ourEventArgs.Record
                End If

                '''
                ''' Validate the object 
                ''' 
                Dim validationEventArgs As New ormDataObjectValidationEventArgs(Me, domainid:=DomainID, timestamp:=timestamp)
                RaiseEvent OnValidationNeeded(Me, validationEventArgs)

                If validationEventArgs.ValidationResult = otValidationResultType.FailedNoProceed Then
                    ''' Failed ?!
                    ''' 
                    CoreMessageHandler(message:="persist operation rejected due to failing validation", messagetype:=otCoreMessageType.ApplicationWarning, _
                                        procedure:="PersistableDataObject.Persist", argument:=Converter.Array2StringList(Me.ObjectPrimaryKeyValues), _
                                        objectname:=Me.ObjectID)

                    ''' return
                    Return False
                End If


                '** feed record
                If doFeedRecord Then Feed()

                '''
                ''' persist the data object through the record
                ''' 
                If Not Me.Record.Persist(timestamp) Then
                    CoreMessageHandler("data object could not persist", dataobject:=Me, procedure:="PersistableDataObject.Persist", messagetype:=otCoreMessageType.InternalError)
                    Persist = False
                Else
                    ''' set it loaded
                    For Each aContainerID In Me.Record.ContainerIDS
                        Me.Setloaded(aContainerID)
                    Next
                    '''
                    ''' cascade the operation through the related members
                    ''' 
                    If Not Me.CascadeRelations(cascadeUpdate:=True, timestamp:=timestamp, uniquenesswaschecked:=_UniquenessInStoreWasChecked) Then
                        Persist = False
                    Else
                        Persist = True
                    End If
                End If

                '** set flags -> we are persisted anyway even if the events might demand to abort
                '''
                If Persist Then
                    _isCreated = False
                    _IsChanged = False
                    'Me.Setloaded() -> above
                    _IsDeleted = False
                Else
                    _IsDeleted = isdeleted
                End If


                '** fire event
                ourEventArgs = New ormDataObjectEventArgs(Me, record:=Me.Record, key:=Me.ObjectPrimaryKey, _
                                                               timestamp:=timestamp, usecache:=Me.ObjectUsesCache, domainID:=DomainID, _
                                                               domainBehavior:=Me.ObjectHasDomainBehavior, runtimeOnly:=Me.RunTimeOnly)
                RaiseEvent OnPersisted(Me, ourEventArgs)
                Persist = ourEventArgs.Proceed And Persist

                RaiseEvent ClassOnPersisted(Me, ourEventArgs)
                Persist = ourEventArgs.Proceed And Persist

                Return Persist

            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception", exception:=ex, procedure:="PersistableDataObject.Persist")
                Return False
            End Try
        End Function

        ''' <summary>
        ''' generic function to create  a persistable by primary key
        ''' </summary>
        ''' <param name="pkArray"></param>
        ''' <param name="domainID" > optional domain ID for domain behavior</param>
        ''' <param name="dataobject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Create(primarykey As ormDatabaseKey, _
                                              Optional domainID As String = Nothing, _
                                              Optional checkUnique As Boolean? = Nothing, _
                                              Optional runtimeOnly As Boolean? = Nothing) As Boolean Implements iormPersistable.Create
            ''' defautl values
            If String.IsNullOrEmpty(domainID) Then domainID = CurrentSession.CurrentDomainID
            If Not checkUnique.HasValue Then checkUnique = True
            If Not runtimeOnly.HasValue Then runtimeOnly = False

            '*** add the primary keys
            '** is a session running ?!
            If Not runtimeOnly AndAlso Not CurrentSession.IsRunning AndAlso Not CurrentSession.IsStartingUp Then
                Call CoreMessageHandler(message:="data object cannot be created - start session to database first", _
                                          procedure:="PersistableDataObject.create", objectname:=Me.ObjectID, _
                                           messagetype:=otCoreMessageType.ApplicationError)
                Return False
            End If

            '** initialize
            If Not Me.IsInitialized AndAlso Not Me.Initialize(runtimeOnly:=runtimeOnly) Then
                Call CoreMessageHandler(message:="data object can not be initialized", containerID:=_primaryContainerID, argument:=Record.ToString, _
                                        procedure:="PersistableDataObject.create", messagetype:=otCoreMessageType.InternalError)

                Return False
            End If

            '** set default
            If String.IsNullOrEmpty(domainID) Then domainID = ConstGlobalDomain

            '** copy the primary keys
            primarykey.ToRecord(record:=Me.Record, objectclassdescription:=Me.ObjectClassDescription, domainid:=domainID, runtimeOnly:=runtimeOnly)

            ''' run the create with this record
            ''' 
            Return Create(record:=Me.Record, domainID:=domainID, checkUnique:=checkUnique, runtimeOnly:=runtimeOnly)
        End Function


        ''' <summary>
        ''' Retrieve a data object from the cache or load it
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="pkArray"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function RetrieveDataObject(Of T As {iormInfusable, ormDataObject, iormPersistable, New}) _
            (pkArray() As Object, _
             Optional domainID As String = Nothing, _
             Optional dbdriver As iormDatabaseDriver = Nothing, _
             Optional forceReload As Boolean? = Nothing, _
             Optional runtimeOnly As Boolean? = Nothing) As T
            Return RetrieveDataObject(pkArray:=pkArray, type:=GetType(T), domainID:=domainID, dbdriver:=dbdriver, forceReload:=forceReload, runtimeOnly:=runtimeOnly)
        End Function
        ''' <summary>
        ''' Retrieve a data object from the cache or load it
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="pkArray"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function RetrieveDataObject(Of T As {iormInfusable, ormDataObject, iormPersistable, New}) _
            (key As ormDatabaseKey, _
             Optional domainID As String = Nothing, _
             Optional dbdriver As iormDatabaseDriver = Nothing, _
             Optional forceReload As Boolean? = Nothing, _
             Optional runtimeOnly As Boolean? = Nothing) As T
            Return RetrieveDataObject(key:=key, type:=GetType(T), domainID:=domainID, dbdriver:=dbdriver, forceReload:=forceReload, runtimeOnly:=runtimeOnly)
        End Function

        ''' <summary>
        ''' Retrieve a data object from the cache or load it - use an array of values which are supposed to be the primary key of the object
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="pkArray"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Overloads Shared Function RetrieveDataObject(pkArray() As Object, type As System.Type, _
                 Optional domainID As String = Nothing, _
                 Optional dbdriver As iormDatabaseDriver = Nothing, _
                 Optional forceReload As Boolean? = Nothing, _
                 Optional runtimeOnly As Boolean? = Nothing) As iormPersistable

            ''' get the primarykey of the object out of a record and might be primarykey of a secondary table
            ''' 

            Dim aFactory As iormDataObjectProvider = CurrentSession.Objects(domainid:=domainID).GetDataObjectProvider(type)
            Dim aPrimaryKey = New ormDatabaseKey(objectid:=CurrentSession.Objects(domainid:=domainID).GetObjectname(type), keyvalues:=pkArray)
            ''' get the data object from the factory
            Return aFactory.Retrieve(primarykey:=aPrimaryKey, type:=type, domainID:=domainID, forceReload:=forceReload, runtimeOnly:=runtimeOnly)
        End Function

        ''' <summary>
        ''' Retrieve a data object from the cache or load it
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="pkArray"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        '<ormObjectOperationMethod(Description:="Retrieve a Data Object by primary keys from store)", _
        '    OperationName:="GeneralRetrieveBy PrimaryKeys", Tag:=ObjectClassDescription.ConstMTRetrieve, TransactionID:=ConstOPRetrieve)> _
        Public Overloads Shared Function RetrieveDataObject(key As ormDatabaseKey, type As System.Type, _
                                                             Optional domainID As String = Nothing, _
                                                             Optional dbdriver As iormDatabaseDriver = Nothing, _
                                                             Optional forceReload As Boolean? = Nothing, _
                                                             Optional runtimeOnly As Boolean? = Nothing) As iormPersistable

            Dim aFactory As iormDataObjectProvider = CurrentSession.Objects(domainid:=domainID).GetDataObjectProvider(type)
            
            ''' get the data object from the factory
            Return aFactory.Retrieve(primarykey:=key, type:=type, domainID:=domainID, forceReload:=forceReload, runtimeOnly:=runtimeOnly)
        End Function


        ''' <summary>
        ''' Static Function ALL returns a Collection of all objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function AllDataObject(Of T As {iormPersistable, New})(Optional key As ormDatabaseKey = Nothing, _
                                                                             Optional ID As String = "All", _
                                                                             Optional domainid As String = Nothing) _
                                                                              As List(Of T)
            Dim aFactory As iormDataObjectProvider = CurrentSession.Objects(domainid:=domainid).GetDataObjectProvider(GetType(T))
            ''' get the data object from the factory
            Return aFactory.RetrieveAll(key:=key, type:=GetType(T), domainID:=domainid)

        End Function

    End Class
End Namespace