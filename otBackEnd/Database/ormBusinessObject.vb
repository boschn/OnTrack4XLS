
REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** ORM Business Object Class - heavy weight relational business object
REM ***********
REM *********** Version: X.YY
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
    ''' abstract base class for all business objects based on relational persistence
    ''' handles the data operations with an embedded record
    ''' raises all data events
    ''' </summary>
    ''' <remarks>
    ''' functional Design principles
    ''' 1. derived from infusable
    ''' 2. own features : SpareField Flag
    ''' 3. rights on operations - who is allowed
    ''' 4. persists relational in multiple tables
    ''' 5. tracks a message log
    ''' 6. allows validation of entry members
    ''' 7. allows cloneing
    ''' 8. implements CRUD operations such as Create, Retrieve, Update, Delete
    ''' </remarks>
    Partial Public MustInherit Class ormBusinessObject
        Inherits persistableDataObject
        Implements System.ComponentModel.INotifyPropertyChanged
        Implements iormRelationalPersistable
        Implements iormCloneable
        Implements iormValidatable
        Implements iormQueriable
        Implements iormLoggable
        Implements IDisposable

        ''' <summary>
        ''' important objects to drive data object behavior
        ''' </summary>
        ''' <remarks></remarks>

        Private WithEvents _validator As ObjectValidator          ' valitator to validate

        ''' <summary>
        ''' identifier for ormLoggable
        ''' </summary>
        ''' <remarks></remarks>
        Protected _contextidentifier As String
        Protected _tupleidentifier As String
        Protected _entityidentifier As String

        ''' <summary>
        ''' Spare member entries
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, size:=255, spareFieldTag:=True, posordinal:=1101, _
        title:="text field #1", description:="available spare text field #1")> Public Const ConstFNSpareText1 = "SPARETEXT1"
        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, size:=255, spareFieldTag:=True, posordinal:=1102, _
        title:="text field #2", description:="available spare text field #2")> Public Const ConstFNSpareText2 = "SPARETEXT2"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=255, isnullable:=True, spareFieldTag:=True, posordinal:=1103, _
        title:="text field #3", description:="available spare text field #3")> Public Const ConstFNSpareText3 = "SPARETEXT3"
        <ormObjectEntry(Datatype:=otDataType.Numeric, isnullable:=True, spareFieldTag:=True, posordinal:=1201, _
        title:="numeric field #1", description:="available spare numeric field #1")> Public Const ConstFNSpareNumeric1 = "SPARENUMERIC1"
        <ormObjectEntry(Datatype:=otDataType.Numeric, isnullable:=True, spareFieldTag:=True, posordinal:=1202, _
        title:="numeric field #2", description:="available spare numeric field #2")> Public Const ConstFNSpareNumeric2 = "SPARENUMERIC2"
        <ormObjectEntry(Datatype:=otDataType.Numeric, isnullable:=True, spareFieldTag:=True, posordinal:=1203, _
        title:="numeric field #3", description:="available spare numeric field #3")> Public Const ConstFNSpareNumeric3 = "SPARENUMERIC3"
        <ormObjectEntry(Datatype:=otDataType.Date, isnullable:=True, spareFieldTag:=True, posordinal:=1301, _
        title:="date field #1", description:="available spare date field #1")> Public Const ConstFNSpareDate1 = "SPAREDATE1"
        <ormObjectEntry(Datatype:=otDataType.Date, isnullable:=True, spareFieldTag:=True, posordinal:=1302, _
        title:="date field #2", description:="available spare date field #2")> Public Const ConstFNSpareDate2 = "SPAREDATE2"
        <ormObjectEntry(Datatype:=otDataType.Date, isnullable:=True, spareFieldTag:=True, posordinal:=1303, _
        title:="date field #3", description:="available spare date field #3")> Public Const ConstFNSpareDate3 = "SPAREDATE3"
        <ormObjectEntry(Datatype:=otDataType.Bool, isnullable:=True, spareFieldTag:=True, posordinal:=1401, _
        title:="flag field #1", description:="available spare flag field #1")> Public Const ConstFNSpareFlag1 = "SPAREFLAG1"
        <ormObjectEntry(Datatype:=otDataType.Bool, isnullable:=True, spareFieldTag:=True, posordinal:=1402, _
        title:="flag field #2", description:="available spare flag field #2")> Public Const ConstFNSpareFlag2 = "SPAREFLAG2"
        <ormObjectEntry(Datatype:=otDataType.Bool, isnullable:=True, spareFieldTag:=True, posordinal:=1403, _
        title:="flag field #3", description:="available spare flag field #3")> Public Const ConstFNSpareFlag3 = "SPAREFLAG3"

        ''' <summary>
        ''' MSG LOG TAG
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=BusinessObjectMessage.ConstObjectID & "." & BusinessObjectMessage.ConstFNTag, isnullable:=True)> _
        Public Const ConstFNMSGLOGTAG = BusinessObjectMessage.ConstFNTag



        ''' <summary>
        ''' ColumnMapping
        ''' </summary>
        ''' <remarks></remarks>
        '** Spare Fields
        <ormObjectEntryMapping(EntryName:=ConstFNSpareText1)> Protected _spare_txt1 As String
        <ormObjectEntryMapping(EntryName:=ConstFNSpareText2)> Protected _spare_txt2 As String
        <ormObjectEntryMapping(EntryName:=ConstFNSpareText3)> Protected _spare_txt3 As String
        <ormObjectEntryMapping(EntryName:=ConstFNSpareNumeric1)> Protected _spare_num1 As Nullable(Of Double)
        <ormObjectEntryMapping(EntryName:=ConstFNSpareNumeric2)> Protected _spare_num2 As Nullable(Of Double)
        <ormObjectEntryMapping(EntryName:=ConstFNSpareNumeric3)> Protected _spare_num3 As Nullable(Of Double)
        <ormObjectEntryMapping(EntryName:=ConstFNSpareDate1)> Protected _spare_date1 As Nullable(Of Date)
        <ormObjectEntryMapping(EntryName:=ConstFNSpareDate2)> Protected _spare_date2 As Nullable(Of Date)
        <ormObjectEntryMapping(EntryName:=ConstFNSpareDate3)> Protected _spare_date3 As Nullable(Of Date)
        <ormObjectEntryMapping(EntryName:=ConstFNSpareFlag1)> Protected _spare_flag1 As Nullable(Of Boolean)
        <ormObjectEntryMapping(EntryName:=ConstFNSpareFlag2)> Protected _spare_flag2 As Nullable(Of Boolean)
        <ormObjectEntryMapping(EntryName:=ConstFNSpareFlag3)> Protected _spare_flag3 As Nullable(Of Boolean)

        ''' <summary>
        ''' message log tag for the business object
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNMSGLOGTAG)> Protected _msglogtag As String

        '''
        ''' Transactions DEFAULTS
        ''' 
        <ormObjectTransactionAttribute(DefaultAllowPermission:=True, PermissionRules:={ObjectPermissionRuleProperty.DBAccess & "(" & AccessRightProperty.ConstARReadonly & ", true, true)"}, _
                        Description:="create an instance of persist able data object")> Public Const ConstOPCreate = "Create"
        <ormObjectTransactionAttribute(DefaultAllowPermission:=True, PermissionRules:={ObjectPermissionRuleProperty.DBAccess & "(" & AccessRightProperty.ConstARReadonly & ", true, true)"}, _
                       Description:="retrieve a data object")> Public Const ConstOPRetrieve = "Retrieve"
        <ormObjectTransactionAttribute(DefaultAllowPermission:=True, PermissionRules:={ObjectPermissionRuleProperty.DBAccess & "(" & AccessRightProperty.ConstARReadUpdate & ", true, true)"}, _
                       Description:="delete a data object")> Public Const ConstOPDelete = "Delete"
        <ormObjectTransactionAttribute(DefaultAllowPermission:=True, PermissionRules:={ObjectPermissionRuleProperty.DBAccess & "(" & AccessRightProperty.ConstARReadonly & ", true, true)"}, _
                       Description:="inject a data object")> Public Const ConstOPInject = "Inject"
        <ormObjectTransactionAttribute(DefaultAllowPermission:=True, PermissionRules:={ObjectPermissionRuleProperty.DBAccess & "(" & AccessRightProperty.ConstARReadUpdate & ", true, true)"}, _
                       Description:="perist a data object")> Public Const ConstOPPersist = "Persist"


        ''' Queries
        ''' 
        <ormObjectQuery(Description:="All Objects", where:="")> Public Const ConstQRYAll = "All"


        ''' <summary>
        ''' Operation Constants
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstOPGetObjectMessages = "GetObjectMessages"

        ''' <summary>
        ''' Relation to Message Log
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkObject:=GetType(BusinessObjectMessage), retrieveOperation:=ConstOPGetObjectMessages, _
                     cascadeonCreate:=False, cascadeOnDelete:=False, cascadeOnUpdate:=False)> _
        Public Const ConstRMessageLog = "RelObjectMessage"

        <ormObjectEntryMapping(relationName:=ConstRMessageLog, infusemode:=otInfuseMode.OnDemand)> Protected WithEvents _ObjectMessageLog As BusinessObjectMessageLog '  MessageLog

        ''' <summary>
        ''' Events
        ''' </summary>
        ''' <remarks></remarks>
        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

        Public Event OnCloning(sender As Object, e As ormDataObjectCloneEventArgs) Implements iormCloneable.OnCloning
        Public Event OnCloned(sender As Object, e As ormDataObjectCloneEventArgs) Implements iormCloneable.OnCloned
        Public Shared Event ClassOnCloning(sender As Object, e As ormDataObjectCloneEventArgs)
        Public Shared Event ClassOnCloned(sender As Object, e As ormDataObjectCloneEventArgs)

        '* Validation Events
        Public Event OnEntryValidating(sender As Object, e As ormDataObjectEntryEventArgs) Implements iormValidatable.OnEntryValidating
        Public Event OnEntryValidated(sender As Object, e As ormDataObjectEntryEventArgs) Implements iormValidatable.OnEntryValidated
        Public Event OnValidating(sender As Object, e As ormDataObjectEventArgs) Implements iormValidatable.OnValidating
        Public Event OnValidated(sender As Object, e As ormDataObjectEventArgs) Implements iormValidatable.OnValidated

        '** ObjectMessage Added to Log
        Public Event OnObjectMessageAdded(sender As Object, e As BusinessObjectMessageLog.EventArgs)

#Region "Properties"

        ''' <summary>
        ''' Sets the flag for ignoring the domainentry (delete on domain level)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsDomainIgnored As Boolean
            Get
                Return _DomainIsIgnored
            End Get
            Set(value As Boolean)
                SetValue(entryname:=ConstFNIsDomainIgnored, value:=value)
            End Set
        End Property
        ''' <summary>
        ''' Gets the table store.
        ''' </summary>
        ''' <value>The table store.</value>
        Public ReadOnly Property ObjectPrimaryTableStore() As iormRelationalTableStore Implements iormRelationalPersistable.ObjectPrimaryTableStore
            Get
                If _record IsNot Nothing AndAlso _record.Alive AndAlso _record.ContainerStores IsNot Nothing AndAlso _record.ContainerStores.Count > 0 Then
                    Return TryCast(_record.RetrieveContainerStore(Me.ObjectPrimaryTableID), iormRelationalTableStore)
                    ''' assume about the tablestore to choose
                ElseIf Not Me.RunTimeOnly AndAlso Not String.IsNullOrEmpty(Me.ObjectPrimaryTableID) Then
                    If _primarydatabasedriver IsNot Nothing Then Return _primarydatabasedriver.RetrieveContainerStore(containerid:=Me.ObjectPrimaryTableID)
                    Return ot.GetPrimaryTableStore(tableid:=Me.ObjectPrimaryTableID)
                End If

                Return Nothing

            End Get
        End Property

        ''' <summary>
        ''' sets or gets the context identifier
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Property ContextIdentifier() As String Implements iormLoggable.ContextIdentifier
            Get
                Return _contextidentifier
            End Get
            Set(value As String)
                _contextidentifier = value
            End Set
        End Property
        ''' <summary>
        ''' sets or gets the context identifier
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Property TupleIdentifier() As String Implements iormLoggable.TupleIdentifier
            Get
                Return _tupleidentifier
            End Get
            Set(value As String)
                _tupleidentifier = value
            End Set
        End Property
        ''' <summary>
        ''' sets or gets the context identifier
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Property EntityIdentifier() As String Implements iormLoggable.EntityIdentifier
            Get
                Return _entityidentifier
            End Get
            Set(value As String)
                _entityidentifier = value
            End Set
        End Property
        ''' <summary>
        ''' returns the object message log for this data object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ObjectMessageLog As BusinessObjectMessageLog Implements iormLoggable.BusinessObjectMessageLog
            Get
                ''' ObjectMessageLog wil always return something (except for errors while infuse)
                ''' since also there might be messages before the object comes alive
                ''' Infuse will merge the loaded into the current ones
                ''' 
                If _ObjectMessageLog Is Nothing Then
                    If Not Me.RunTimeOnly Then
                        If Me.IsAlive(throwError:=False) AndAlso GetRelationStatus(ConstRMessageLog) = ormRelationManager.RelationStatus.Unloaded Then InfuseRelation(ConstRMessageLog)
                        If _ObjectMessageLog Is Nothing Then _ObjectMessageLog = New BusinessObjectMessageLog(Me) ' if nothing is loaded because nothing there
                    Else
                        _ObjectMessageLog = New BusinessObjectMessageLog(Me)
                    End If
                End If

                Return _ObjectMessageLog

            End Get
            Set(value As BusinessObjectMessageLog)
                'Throw New InvalidOperationException("setting the Object message log is not allowed")
            End Set
        End Property
        ''' <summary>
        ''' returns the tableschema associated with this data object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectTableSchema() As iormContainerSchema
            Get
                If Me.ObjectPrimaryTableStore IsNot Nothing Then
                    Return Me.ObjectPrimaryTableStore.ContainerSchema
                Else
                    Return Nothing
                End If

            End Get
        End Property


        ''' <summary>
        '''  gets the DBDriver for the data object to use (real or the default dbdriver)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectPrimaryRelationalDatabaseDriver As iormRelationalDatabaseDriver Implements iormRelationalPersistable.ObjectPrimaryRelationalDatabaseDriver
            Get
                Return TryCast(Me.ObjectPrimaryDatabaseDriver, iormRelationalDatabaseDriver)
            End Get
        End Property



        ''' <summary>
        ''' gets the associated tableids of this object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectTableIDs As String() Implements iormRelationalPersistable.ObjectTableIDs
            Get
                Return Me.ObjectContainerIDs
            End Get
        End Property

        ''' <summary>
        ''' gets the TableID of the primary Table for this dataobject object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectPrimaryTableID() As String Implements iormRelationalPersistable.ObjectPrimaryTableID
            Get
                Return Me.ObjectPrimaryContainerID
            End Get
        End Property


        ''' <summary>
        ''' sets or gets the message log tag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ObjectMessageLogTag() As String
            Get
                Return _msglogtag
            End Get
            Set(value As String)
                SetValue(ConstFNMSGLOGTAG, value)
            End Set
        End Property


        ''' <summary>
        ''' gets or sets the additional spare parameter num1
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SpareFieldNumeric1 As Double?
            Get
                Return _spare_num1
            End Get
            Set(value As Double?)
                SetValue(ConstFNSpareNumeric1, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the additional spare parameter num2
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SpareFieldNumeric2 As Double?
            Get
                Return _spare_num2
            End Get
            Set(value As Double?)
                SetValue(ConstFNSpareNumeric2, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the additional spare parameter num3
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Property SpareFieldNumeric3 As Double?
            Get
                Return _spare_num3
            End Get
            Set(value As Double?)
                SetValue(ConstFNSpareNumeric3, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the additional spare parameter date1
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SpareFieldDate1 As Date?
            Get
                Return _spare_date1
            End Get
            Set(value As Date?)
                SetValue(ConstFNSpareDate1, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the additional spare parameter date2
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SpareFieldDate2 As Date?
            Get
                Return _spare_date2
            End Get
            Set(value As Date?)
                SetValue(ConstFNSpareDate2, value)
            End Set
        End Property

        ''' <summary>
        ''' gets or sets the additional spare parameter date3
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SpareFieldDate3 As Date?
            Get
                Return _spare_date3
            End Get
            Set(value As Date?)
                SetValue(ConstFNSpareDate3, value)
            End Set
        End Property

        ''' <summary>
        ''' gets or sets the additional spare parameter flag1
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SpareFieldFlag1 As Boolean?
            Get
                Return _spare_flag1
            End Get
            Set(value As Boolean?)
                SetValue(ConstFNSpareFlag1, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the additional spare parameter flag3
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SpareFieldFlag3 As Boolean?
            Get
                Return _spare_flag3
            End Get
            Set(value As Boolean?)
                SetValue(ConstFNSpareFlag3, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the additional spare parameter flag2
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SpareFieldFlag2 As Boolean?
            Get
                Return _spare_flag2
            End Get
            Set(value As Boolean?)
                SetValue(ConstFNSpareFlag2, value)
            End Set
        End Property

        ''' <summary>
        '''  gets or sets the additional spare parameter
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SpareFieldText1 As String
            Get
                Return _spare_txt1
            End Get
            Set(value As String)
                SetValue(ConstFNSpareText1, value)
            End Set
        End Property
        ''' <summary>
        '''  gets or sets the additional spare parameter
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SpareFieldText2 As String
            Get
                Return _spare_txt2
            End Get
            Set(value As String)
                SetValue(ConstFNSpareText2, value)
            End Set
        End Property
        ''' <summary>
        '''  gets or sets the additional spare parameter
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SpareFieldText3 As String
            Get
                Return _spare_txt3
            End Get
            Set(value As String)
                SetValue(ConstFNSpareText3, value)
            End Set
        End Property

#End Region

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
            _ObjectMessageLog = Nothing
        End Sub


        ''' <summary>
        ''' operation to load the object messages into the local container
        ''' </summary>
        ''' <param name="id">the property</param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ormObjectOperationMethod(operationname:=ConstOPGetObjectMessages)> Public Function LoadObjectMessages() As iormRelationalCollection(Of BusinessObjectMessage)
            If Not IsAlive(subname:="LoadObjectMessages") Then Return New ormRelationCollection(Of BusinessObjectMessage)(Nothing, keyentrynames:={BusinessObjectMessage.ConstFNNo})

            ''' assign the messagelog
            If _ObjectMessageLog Is Nothing Then _ObjectMessageLog = New BusinessObjectMessageLog(Me)

            ''' load the existing log and merge it into the current one
            ''' 
            If Not Me.RunTimeOnly Then
                Dim aRetrieveLog As BusinessObjectMessageLog = ObjectMessageLog.Retrieve(Me.ObjectTag)
                For Each aMessage In aRetrieveLog
                    If _ObjectMessageLog.ContainsKey(key:=aMessage.No) Then
                        aMessage.No = _ObjectMessageLog.Max(Function(x) x.No) + 1
                    End If
                    _ObjectMessageLog.Add(aMessage)
                Next
            End If

            Return _ObjectMessageLog
        End Function


        ''' <summary>
        ''' injects retrieving records from the datastores and infuses the object from the inside out
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Inject(ByRef primarykey As ormDatabaseKey, _
                                           Optional domainid As String = Nothing, _
                                           Optional dbdriver As iormDatabaseDriver = Nothing, _
                                           Optional loadDeleted As Boolean = False) As Boolean Implements iormPersistable.Inject

            '** check on the operation right for this object
            If Not RunTimeOnly AndAlso Not CurrentSession.IsStartingUp AndAlso Not ot.GetBootStrapObjectClassIDs.Contains(Me.ObjectID) _
                AndAlso Not CurrentSession.ValidateAccessRights(accessrequest:=otAccessRight.ReadOnly, domainid:=domainid, _
                                                                objecttransactions:={Me.ObjectID & "." & ConstOPInject}) Then
                If Not CurrentSession.RequestUserAccess(accessRequest:=otAccessRight.ReadOnly, _
                                                        domainid:=domainid, _
                                                        username:=CurrentSession.CurrentUsername, _
                                                         messagetext:="Please provide another user to authorize requested operation", _
                                                        objecttransactions:={Me.ObjectID & "." & ConstOPInject}) Then
                    Call CoreMessageHandler(message:="data object cannot be injected - permission denied to user", _
                                            objectname:=Me.ObjectID, argument:=ConstOPInject, username:=CurrentSession.CurrentUsername, _
                                            procedure:="ormBusinessObject.Inject", messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If
            End If

            Return MyBase.Inject(primarykey:=primarykey, domainid:=domainid, dbdriver:=dbdriver, loadDeleted:=loadDeleted)
        End Function

        ''' <summary>
        ''' Persist the object to the datastore
        ''' </summary>
        ''' <param name="timestamp"></param>
        ''' <returns>True if successfull</returns>
        ''' <remarks></remarks>
        Public Overridable Function Persist(Optional timestamp As DateTime? = Nothing, Optional doFeedRecord As Boolean = True) As Boolean Implements iormRelationalPersistable.Persist

            '** check on the operation right for this object
            If Not CurrentSession.IsStartingUp AndAlso _
                Not CurrentSession.ValidateAccessRights(accessrequest:=otAccessRight.ReadUpdateData, objecttransactions:={Me.ObjectID & "." & ConstOPPersist}) Then
                '** authorize
                If Not CurrentSession.RequestUserAccess(accessRequest:=otAccessRight.ReadUpdateData, _
                                                    messagetext:="Please provide another user to authorize requested operation", _
                                                    username:=CurrentSession.CurrentUsername, loginOnFailed:=True, _
                                                    objecttransactions:={Me.ObjectID & "." & ConstOPPersist}) Then
                    Call CoreMessageHandler(message:="data object cannot be persisted - permission denied to user", _
                                            objectname:=Me.ObjectID, argument:=ConstOPPersist, username:=CurrentSession.CurrentUsername, _
                                            procedure:="ormBusinessObject.Persist", messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If
            End If

            Return MyBase.Persist(timestamp:=timestamp, doFeedRecord:=doFeedRecord)
        End Function

        ''' <summary>
        ''' shared create the schema for this object by reflection
        ''' </summary>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateDataObjectSchema(Of T)(Optional silent As Boolean = False, Optional dbdriver As iormRelationalDatabaseDriver = Nothing) As Boolean
            '** check on Bootstrapping conditions
            Dim aClassDescription = ot.GetObjectClassDescription(GetType(T))
            If dbdriver Is Nothing Then dbdriver = CurrentOTDBDriver
            If aClassDescription.ObjectAttribute.IsBootstrap And Not CurrentSession.IsBootstrappingInstallationRequested Then
                dbdriver.VerifyOnTrackDatabase() 'check if a bootstrap needs to be issued
            End If
            Dim anObjectDefinition = ot.CurrentSession.Objects.GetObjectDefinition(aClassDescription.ObjectAttribute.ID)
            If anObjectDefinition IsNot Nothing AndAlso anObjectDefinition.GetType.IsAssignableFrom(GetType(ormObjectDefinition)) Then
                Return CType(anObjectDefinition, ormObjectDefinition).CreateObjectSchema(silent:=silent)
            Else
                Throw New NotImplementedException("creating schema through iormObjectDefinition not implemented")
            End If
            Return False

        End Function

        ''' <summary>
        ''' clone a dataobject with a new pkarray. return nothing if fails
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="cloneobject"></param>
        ''' <param name="newpkarray"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CloneDataObject(Of T As {iormRelationalPersistable, iormCloneable, New})(cloneobject As iormCloneable(Of T), newpkarray As Object()) As T
            Return cloneobject.Clone(newpkarray)
        End Function

        ''' <summary>
        ''' this method must be overritten
        ''' </summary>
        ''' <param name="newpkarray"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function CloneObject(newpkarray As Object(), Optional runtimeOnly As Boolean? = Nothing) As Object Implements iormCloneable.Clone
            ''' by intention
            Throw New NotImplementedException(message:="use derived version instead")
        End Function

        ''' <summary>
        ''' cloe the object with an primary key array
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="newpkarray"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Clone(Of T As {iormPersistable, iormInfusable, Class, New})(newpkarray As Object(), _
                                                                                                   Optional runtimeOnly As Boolean? = Nothing) As T
            Dim aPrimarykey As New ormDatabaseKey(objectid:=Me.ObjectID, keyvalues:=newpkarray)
            Return Me.Clone(Of T)(newprimarykey:=aPrimarykey, runtimeOnly:=runtimeOnly)
        End Function
        ''' <summary>
        ''' clone the object with the new primary key
        ''' </summary>
        ''' <param name="pkarray">primary key array</param>
        ''' <remarks></remarks>
        ''' <returns>the new cloned object or nothing</returns>
        Public Overloads Function Clone(Of T As {iormPersistable, iormInfusable, Class, New})(newprimarykey As ormDatabaseKey, _
                                                                                             Optional runtimeOnly As Boolean? = Nothing) As T
            '
            '*** now we copy the object
            Dim aNewObject As New T
            Dim newRecord As New ormRecord
            If Not runtimeOnly.HasValue Then runtimeOnly = Me.RunTimeOnly

            '**
            If Not Me.IsAlive(subname:="clone") Then Return Nothing


            '* fire class event
            Dim ourEventArgs As New ormDataObjectCloneEventArgs(newObject:=TryCast(aNewObject, ormBusinessObject), oldObject:=Me)
            ourEventArgs.UseCache = Me.ObjectUsesCache
            RaiseEvent ClassOnCloning(Me, ourEventArgs)
            If ourEventArgs.AbortOperation Then
                If ourEventArgs.Result Then
                    If ourEventArgs.DataObject IsNot Nothing Then
                        Return TryCast(ourEventArgs.DataObject, T)
                    Else
                        CoreMessageHandler(message:="ClassOnCloning: cannot convert persistable to class", argument:=GetType(T).Name, procedure:="ormBusinessObject.Clone(of T)", _
                                           messagetype:=otCoreMessageType.InternalError)
                        Return Nothing
                    End If
                Else
                    Return Nothing
                End If
            End If

            '* fire object event
            RaiseEvent OnCloning(Me, ourEventArgs)
            If ourEventArgs.AbortOperation Then
                If ourEventArgs.Result AndAlso ourEventArgs.DataObject IsNot Nothing Then
                    Return TryCast(ourEventArgs.DataObject, T)
                Else
                    Return Nothing
                End If
            End If

            ' set it
            If Not runtimeOnly Then newRecord.SetContainer(Me.ObjectPrimaryTableID)

            ' go through the table and overwrite the Record if the rights are there
            For Each entryname In Me.Record.Entrynames
                If entryname <> ConstFNCreatedOn And entryname <> ConstFNUpdatedOn _
                    And entryname <> ConstFNIsDeleted And entryname <> ConstFNDeletedOn _
                    And entryname <> ConstFNIsDomainIgnored Then

                    Call newRecord.SetValue(entryname, Me.Record.GetValue(entryname))
                End If
            Next entryname

            ''' copy the new primary keys
            Me.CopyPrimaryKeyToRecord(newprimarykey, newRecord, runtimeOnly:=Me.RunTimeOnly)

            ''' create the new object with the record
            ''' 
            If Not aNewObject.Create(record:=newRecord, checkUnique:=True) Then
                Call CoreMessageHandler(message:="object new keys are not unique - clone aborted", argument:=newprimarykey, containerID:=_primaryContainerID, _
                                       messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If

            '** Fire Event
            ourEventArgs = New ormDataObjectCloneEventArgs(newObject:=TryCast(aNewObject, ormBusinessObject), oldObject:=Me)

            RaiseEvent OnCloned(Me, ourEventArgs)
            If ourEventArgs.AbortOperation Then
                If ourEventArgs.Result AndAlso ourEventArgs.DataObject IsNot Nothing Then
                    Return TryCast(ourEventArgs.DataObject, T)
                Else
                    Return Nothing
                End If
            End If

            '** Fire class Event
            RaiseEvent ClassOnCloned(Me, ourEventArgs)
            If ourEventArgs.AbortOperation Then
                If ourEventArgs.Result AndAlso ourEventArgs.DataObject IsNot Nothing Then
                    Return TryCast(ourEventArgs.DataObject, T)
                Else
                    Return Nothing
                End If
            End If

            ''' return
            ''' 
            Return aNewObject
        End Function

        ''' <summary>
        ''' Delete the object and its persistancy
        ''' </summary>
        ''' <returns>True if successfull</returns>
        ''' <remarks></remarks>
        Public Overridable Function Delete(Optional timestamp As DateTime? = Nothing) As Boolean Implements iormRelationalPersistable.Delete

            '** check on the operation right for this object
            If Not RunTimeOnly AndAlso Not CurrentSession.ValidateAccessRights(accessrequest:=otAccessRight.ReadUpdateData, _
                                                                               domainid:=DomainID, _
                                                                                objecttransactions:={Me.ObjectID & "." & ConstOPDelete}) Then

                If Not CurrentSession.RequestUserAccess(accessRequest:=otAccessRight.ReadOnly, username:=CurrentSession.CurrentUsername, _
                                                        domainid:=DomainID, loginOnFailed:=True, _
                                                         messagetext:="Please provide another user to authorize requested operation", _
                                                         objecttransactions:={Me.ObjectID & "." & ConstOPDelete}) Then
                    Call CoreMessageHandler(message:="data object cannot be deleted - permission denied to user", _
                                            objectname:=Me.ObjectID, argument:=ConstOPDelete, username:=CurrentSession.CurrentUsername, _
                                            procedure:="ormBusinessObject.Delete", messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If
            End If

            Return MyBase.Delete(timestamp:=timestamp)

        End Function

#Region "EventHandling"

        ''' <summary>
        ''' handles the validation needed event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ormBusinessObject_onValidationNeeded(sender As Object, e As ormDataObjectValidationEventArgs) Handles MyBase.OnValidationNeeded
            ''' run validation
            ''' 
            Dim aMsglog As BusinessObjectMessageLog = e.Msglog
            If e.Msglog Is Nothing Then aMsglog = Me.ObjectMessageLog
            e.ValidationResult = Me.Validate(msglog:=aMsglog)
            ''' throw result to message log
            If e.ValidationResult = otValidationResultType.FailedNoProceed Then
                ''' Failed ?!
                ''' 
                CoreMessageHandler(message:="persist operation rejected due to failing validation", messagetype:=otCoreMessageType.ApplicationWarning, _
                                    procedure:="ormBusinessObject.OnValidationNeeded", argument:=Converter.Array2StringList(Me.ObjectPrimaryKeyValues), _
                                    objectname:=Me.ObjectID, msglog:=aMsglog)


            End If
        End Sub
        ''' <summary>
        ''' event handler for onPersisted Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ormBusinessObject_onPersisted(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnPersisted
            ''' persist the object messages
            ''' 
            If _ObjectMessageLog IsNot Nothing AndAlso _ObjectMessageLog.Count > 0 Then
                For Each aMessage In _ObjectMessageLog
                    If Not aMessage.RunTimeOnly AndAlso aMessage.IsPersisted Then
                        aMessage.Persist(timestamp:=e.Timestamp)
                    End If
                Next
            End If
        End Sub
        ''' <summary>
        ''' handler for the OnInfused Event 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ormBusinessObject_OnInfused(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnInfused

            '** set all containers to be loaded
            Setloaded()
        End Sub

        ''' <summary>
        ''' Handler cascaded the OnObjectMessageAdded Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ormDataObject_OnObjectMessageAdded(sender As Object, e As BusinessObjectMessageLog.EventArgs) Handles _ObjectMessageLog.OnObjectMessageAdded
            RaiseEvent OnObjectMessageAdded(sender:=sender, e:=e)
        End Sub

        ''' <summary>
        ''' raises the PropetfyChanged Event
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Sub RaiseObjectEntryChanged(entryname As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(entryname))
        End Sub

        ''' <summary>
        ''' Event Handler for defaultValues
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ormDataObject_OnDefaultValuesNeeded(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreateDefaultValuesNeeded
            Dim result As Boolean = True

            '** set the default values of the object
            If Not CurrentSession.IsBootstrappingInstallationRequested AndAlso Not CurrentSession.IsStartingUp Then
                For Each anEntry In e.DataObject.ObjectDefinition.GetEntries
                    ' only the columns
                    If anEntry.IsContainer Then
                        Dim anColumnEntry As ormObjectFieldEntry = TryCast(anEntry, ormObjectFieldEntry)
                        If anColumnEntry IsNot Nothing And Not e.Record.HasIndex(anColumnEntry.ContainerID & "." & anColumnEntry.ContainerEntryName) Then
                            '' if a default value is neded is decided in the defaultvalue property
                            '' it might be nothing if nullable is true
                            result = result And e.Record.SetValue(anColumnEntry.ContainerID & "." & anColumnEntry.ContainerEntryName, value:=anColumnEntry.Defaultvalue)
                        End If
                    End If
                Next
            Else
                ''' during bootstrapping install or starting up just take the class description values
                ''' 
                For Each anEntry In Me.ObjectClassDescription.ObjectEntryAttributes
                    ' only the columns
                    If anEntry.EntryType = otObjectEntryType.ContainerEntry And Not e.Record.HasIndex(anEntry.ContainerID & "." & anEntry.ContainerEntryName) Then
                        If anEntry.HasValueDefaultValue Then
                            result = result And e.Record.SetValue(anEntry.ContainerID & "." & anEntry.ContainerEntryName, value:=Core.DataType.To(anEntry.DefaultValue, anEntry.Datatype))
                        ElseIf Not anEntry.HasValueIsNullable OrElse (anEntry.HasValueIsNullable AndAlso Not anEntry.IsNullable) Then
                            result = result And e.Record.SetValue(anEntry.ContainerID & "." & anEntry.ContainerEntryName, value:=Core.DataType.GetDefaultValue(anEntry.Datatype))
                        End If
                    End If
                Next
            End If


            e.Result = result
            e.Proceed = True
        End Sub

        ''' <summary>
        ''' Event Handler for initializing
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ormBusinessObject_OnInitializing(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnInitializing
            ''' this already done in the Initialize Method of the ormDataObject class
            ''' 
            'If _containerIsLoaded Is Nothing OrElse _containerIsLoaded.Length = 0 Then
            '    ReDim Preserve _containerIsLoaded(Me.ObjectTableIDs.GetUpperBound(0))
            '    '** set all tables to be unloaded
            '    ' Array.ForEach(Of Boolean)(_tableisloaded, Function(x) x = False) -> do not overwrite true
            'End If
            '''' get new  record if necessary
            '''' STILL we rely on One Table for the Record
            'If _record Is Nothing Then
            '    _record = New ormRecord(Me.ObjectTableIDs, dbdriver:=Me.ObjectPrimaryRelationalDatabaseDriver, runtimeOnly:=RunTimeOnly)
            '    'now we are not runtime only anymore -> set also the table and let's have a fixed structure
            'ElseIf Not Me.RunTimeOnly Then
            '    _record.SetContainers(Me.ObjectTableIDs, dbdriver:=_primarydatabasedriver)
            'End If
        End Sub

        ''' <summary>
        ''' Handler for ObjectEntryValidation
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ormBusinessObject_EntryValidationNeeded(sender As Object, e As ormDataObjectEntryValidationEventArgs) Handles Me.EntryValidationNeeded
            If e.Msglog Is Nothing Then e.Msglog = Me.ObjectMessageLog
            e.Result = Validate(entryname:=e.ObjectEntryName, value:=e.Value, msglog:=e.Msglog)
        End Sub
        ''' <summary>
        ''' Handles the OnEntryChanged Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ormBusinessObject_OnEntryChanged(sender As Object, e As ormDataObjectEntryEventArgs) Handles Me.OnEntryChanged
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(e.ObjectEntryName))
        End Sub

#End Region


    End Class

    ''' <summary>
    ''' definition class for the permission rules on a data object
    ''' </summary>
    ''' <remarks></remarks>

    <ormObject(id:=BusinessObjectPermission.ConstObjectID, modulename:=ConstModuleRepository, description:="permission rules for object access", _
        version:=1, isbootstrap:=True, usecache:=True)> _
    Public Class BusinessObjectPermission
        Inherits ormBusinessObject

        Public Const ConstObjectID = "ObjectPermissionRule"

        <ormTableAttribute(version:=1, usecache:=True, adddomainbehavior:=True, adddeletefieldbehavior:=True)> Public Const ConstPrimaryTableID = "tblObjectPermissions"


        '** Primary key
        <ormObjectEntry(referenceObjectEntry:=ormObjectDefinition.ConstObjectID & "." & ormObjectDefinition.ConstFNID, PrimaryKeyOrdinal:=1 _
                       )> Public Const ConstFNObjectname = ormAbstractEntryDefinition.ConstFNObjectID

        <ormObjectEntry(referenceObjectEntry:=ormObjectFieldEntry.ConstObjectID & "." & ormObjectFieldEntry.ConstFNEntryName, PrimaryKeyOrdinal:=2 _
                        )> Public Const ConstFNEntryname = ormAbstractEntryDefinition.ConstFNEntryName

        <ormObjectEntry(Datatype:=otDataType.Text, size:=150, PrimaryKeyOrdinal:=3, _
                        properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty}, _
                        title:="Operation", description:="business object operation")> Public Const ConstFNOperation = "operation"

        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=4, defaultvalue:=10, _
                        title:="Rule Order", description:="ordinal of the rule")> Public Const ConstFNRuleordinal = "order"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=5, _
                       useforeignkey:=otForeignKeyImplementation.None, defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        '** build foreign key
        ' proplematic
        '<ormForeignKey(entrynames:={ConstFNObjectname, ConstFNEntryname, ConstFNDomainID}, _
        '    foreignkeyreferences:={ObjectColumnEntry.ConstObjectID & "." & ObjectColumnEntry.ConstFNObjectName, _
        '                           ObjectColumnEntry.ConstObjectID & "." & ObjectColumnEntry.ConstFNEntryName, _
        '                           ObjectColumnEntry.ConstObjectID & "." & ObjectColumnEntry.ConstFNDomainID}, _
        '                       useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const constFKprimary = "fkpermission"


        <ormForeignKeyAttribute(entrynames:={ConstFNObjectname}, _
                             foreignkeyreferences:={ormObjectDefinition.ConstObjectID & "." & ormObjectDefinition.ConstFNID}, _
                             useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const constFKprimary = "fkpermission"
        '** Fields

        <ormObjectEntry(Datatype:=otDataType.Text, size:=255, _
            title:="RuleType", description:="rule condition")> Public Const ConstFNRuleType = "typeid"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=255, _
            title:="Rule", description:="rule condition")> Public Const ConstFNRule = "rule"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, _
            title:="Allow Operation", description:="if condition andalso true allow Operation orelse if condition then disallow")> _
        Public Const ConstFNAllow = "allow"
        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, _
            title:="Exit Operation", description:="if condition andalso exittrue then stop rule processing")> _
        Public Const ConstFNExitTrue = "exitontrue"
        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, _
            title:="Exit Operation", description:="if not condition andalso exitfalse then stop rule processing")> _
        Public Const ConstFNExitFalse = "exitonfalse"
        <ormObjectEntry(Datatype:=otDataType.Memo, _
            title:="Description", description:="description of the permission rule")> Public Const ConstFNdesc = "desc"
        <ormObjectEntry(defaultvalue:=0, Datatype:=otDataType.[Long], _
            title:="Version", Description:="version counter of updating")> Public Const ConstFNVersion As String = "VERSION"

        '*** Mappings
        <ormObjectEntryMapping(entryname:=ConstFNObjectname)> Private _objectname As String = String.Empty
        <ormObjectEntryMapping(entryname:=ConstFNEntryname)> Private _entryname As String = String.Empty
        <ormObjectEntryMapping(entryname:=ConstFNOperation)> Private _operation As String = String.Empty
        <ormObjectEntryMapping(entryname:=ConstFNDomainID)> Private _domainID As String = String.Empty
        <ormObjectEntryMapping(entryname:=ConstFNRuleordinal)> Private _order As Long = 0
        <ormObjectEntryMapping(entryname:=ConstFNRuleType)> Private _ruletype As String = String.Empty
        <ormObjectEntryMapping(entryname:=ConstFNRule)> Private _rule As String = String.Empty
        <ormObjectEntryMapping(entryname:=ConstFNAllow)> Private _allow As Boolean
        <ormObjectEntryMapping(entryname:=ConstFNExitTrue)> Private _exitOnTrue As Boolean
        <ormObjectEntryMapping(entryname:=ConstFNExitFalse)> Private _exitOnFalse As Boolean
        <ormObjectEntryMapping(entryname:=ConstFNdesc)> Private _description As String = String.Empty
        <ormObjectEntryMapping(entryname:=ConstFNVersion)> Private _version As ULong = 0

        '*** dynmaic
        Private _permissionruleProperty As ObjectPermissionRuleProperty

#Region "Properties"
        ''' <summary>
        ''' Gets or sets the version.
        ''' </summary>
        ''' <value>The version.</value>
        Public Property Version() As ULong
            Get
                Return Me._version
            End Get
            Set(value As ULong)
                SetValue(entryname:=ConstFNVersion, value:=value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the description.
        ''' </summary>
        ''' <value>The description.</value>
        Public Property Description() As String
            Get
                Return Me._description
            End Get
            Set(value As String)
                SetValue(entryname:=ConstFNdesc, value:=value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the exit.
        ''' </summary>
        ''' <value>The exit.</value>
        Public Property [ExitOnFalse]() As Boolean
            Get
                Return Me._exitOnFalse
            End Get
            Set(value As Boolean)
                SetValue(entryname:=ConstFNExitFalse, value:=value)
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the exit.
        ''' </summary>
        ''' <value>The exit.</value>
        Public Property [ExitOnTrue]() As Boolean
            Get
                Return Me._exitOnTrue
            End Get
            Set(value As Boolean)
                SetValue(entryname:=ConstFNExitTrue, value:=value)
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the allow.
        ''' </summary>
        ''' <value>The allow.</value>
        Public Property Allow() As Boolean
            Get
                Return Me._allow
            End Get
            Set(value As Boolean)
                SetValue(entryname:=ConstFNAllow, value:=value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the rule.
        ''' </summary>
        ''' <value>The rule.</value>
        Public Property Rule() As String
            Get
                Return Me._rule
            End Get
            Set(value As String)
                SetValue(entryname:=ConstFNRule, value:=value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the ruletype.
        ''' </summary>
        ''' <value>The ruletype.</value>
        Public Property Ruletype() As String
            Get
                Return Me._ruletype
            End Get
            Set(value As String)
                SetValue(entryname:=ConstFNRuleType, value:=value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the order.
        ''' </summary>
        ''' <value>The order.</value>
        Public ReadOnly Property Order() As Long
            Get
                Return Me._order
            End Get
        End Property

        ''' <summary>
        ''' Gets the domain ID.
        ''' </summary>
        ''' <value>The domain ID.</value>
        Public ReadOnly Property DomainID() As String
            Get
                Return Me._domainID
            End Get
        End Property

        ''' <summary>
        ''' Gets the operation.
        ''' </summary>
        ''' <value>The operation.</value>
        Public ReadOnly Property Operation() As String
            Get
                Return Me._operation
            End Get
        End Property

        ''' <summary>
        ''' Gets the entryname.
        ''' </summary>
        ''' <value>The entryname.</value>
        Public ReadOnly Property Entryname() As String
            Get
                Return Me._entryname
            End Get
        End Property

        ''' <summary>
        ''' Gets the objectname.
        ''' </summary>
        ''' <value>The objectname.</value>
        Public ReadOnly Property Objectname() As String
            Get
                Return Me._objectname
            End Get
        End Property
        ''' <summary>
        ''' set or gets the RuleProperty
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RuleProperty As ObjectPermissionRuleProperty
            Set(value As ObjectPermissionRuleProperty)
                If _permissionruleProperty Is Nothing OrElse _permissionruleProperty.ToString = value.ToString Then
                    Me.Ruletype = "PROPERTY"
                    Me.ExitOnTrue = value.ExitOnTrue
                    Me.ExitOnFalse = value.ExitOnFalse
                    _permissionruleProperty = value
                    Me.IsChanged = True
                End If
            End Set
            Get
                Return _permissionruleProperty
            End Get
        End Property

#End Region

        ''' <summary>
        ''' Handler for the SwitchRuntimeOFF Event after Bootstrapping
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnSwitchRuntimeOff(sender As Object, e As ormDataObjectEventArgs)
            '** also switch runtime off the column definition via event Handler
            e.Result = Me.SwitchRuntimeOff()
            If Not e.Result Then e.AbortOperation = True
        End Sub

        ''' <summary>
        ''' returns a List of  Permissions for an objectname for the active domainID
        ''' </summary>
        ''' <param name="objectdefinition"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ByObjectName(objectname As String, Optional domainid As String = Nothing) As List(Of BusinessObjectPermission)
            Dim aCollection As New List(Of BusinessObjectPermission)
            Dim aRecordCollection As List(Of ormRecord)
            Dim aStore As iormRelationalTableStore
            '** set the domain
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID

            Try
                aStore = ot.GetPrimaryTableStore(ConstPrimaryTableID)
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(id:="all", addAllFields:=True)
                If Not aCommand.IsPrepared Then
                    aCommand.Where = "[" & ConstFNIsDeleted & "] = @deleted "
                    aCommand.Where &= " AND ([" & ConstFNDomainID & "] = @domainID OR [" & ConstFNDomainID & "] = @globalID)"
                    aCommand.Where &= " AND [" & ConstFNObjectname & "] = @objectname AND [" & ConstFNEntryname & "] = ''"

                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@deleted", ColumnName:=ConstFNIsDeleted, tableid:=ConstPrimaryTableID))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@domainID", ColumnName:=ConstFNDomainID, tableid:=ConstPrimaryTableID))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@globalID", ColumnName:=ConstFNDomainID, tableid:=ConstPrimaryTableID))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@objectname", ColumnName:=ConstFNObjectname, tableid:=ConstPrimaryTableID))
                    aCommand.Prepare()
                End If

                aCommand.SetParameterValue(ID:="@deleted", value:=False)
                aCommand.SetParameterValue(ID:="@domainID", value:=domainid)
                aCommand.SetParameterValue(ID:="@globalID", value:=ConstGlobalDomain)
                aCommand.SetParameterValue(ID:="@objectname", value:=objectname.ToUpper)

                aRecordCollection = aCommand.RunSelect
                Dim instantDir As New Dictionary(Of String, BusinessObjectPermission)

                For Each aRecord As ormRecord In aRecordCollection
                    Dim aPermission As New BusinessObjectPermission
                    If InfuseDataObject(record:=aRecord, dataobject:=aPermission) Then
                        '** add only the domain asked or if nothing in there
                        Dim key As String = aPermission.Objectname & ConstDelimiter & aPermission.Entryname & ConstDelimiter & aPermission.Operation & ConstDelimiter & aPermission.Order.ToString
                        If instantDir.ContainsKey(key) And aPermission.DomainID = domainid Then
                            instantDir.Remove(key:=key)
                            instantDir.Add(key:=key, value:=aPermission)
                        ElseIf Not instantDir.ContainsKey(key) Then
                            instantDir.Add(key:=key, value:=aPermission)
                        End If
                    End If

                Next

                '** transfer the active entries
                For Each apermission In instantDir.Values
                    aCollection.Add(item:=apermission)
                Next

                Return aCollection


            Catch ex As Exception

                Call CoreMessageHandler(exception:=ex, procedure:="ObjectPermission.ByObjectname")
                Return aCollection

            End Try

        End Function


        ''' <summary>
        ''' creates a ObjectPermission
        ''' </summary>
        ''' <param name="objectname"></param>
        ''' <param name="order"></param>
        ''' <param name="operationname"></param>
        ''' <param name="entryname"></param>
        ''' <param name="domainID"></param>
        ''' <param name="checkUnique"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Shared Function Create(objectname As String, order As Long, _
                                         Optional operationname As String = "", _
                                         Optional entryname As String = "", _
                                         Optional domainid As String = Nothing, _
                                         Optional checkUnique As Boolean = True, _
                                            Optional runtimeOnly As Boolean = False) As BusinessObjectPermission
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim pkarray As Object() = {objectname.ToUpper, entryname.ToUpper, operationname.ToUpper, order, domainid}
            Return ormBusinessObject.CreateDataObject(Of BusinessObjectPermission)(pkArray:=pkarray, domainID:=domainid, checkUnique:=checkUnique, runtimeOnly:=runtimeOnly)
        End Function

        ''' <summary>
        ''' retrieves a ObjectPermission
        ''' </summary>
        ''' <param name="objectname"></param>
        ''' <param name="order"></param>
        ''' <param name="operationname"></param>
        ''' <param name="entryname"></param>
        ''' <param name="domainID"></param>
        ''' <param name="checkUnique"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Shared Function Retrieve(objectname As String, order As Long, _
                                           Optional operationname As String = "", _
                                           Optional entryname As String = "", _
                                           Optional domainid As String = Nothing, _
                                            Optional dbdriver As iormRelationalDatabaseDriver = Nothing, _
                                            Optional runtimeOnly As Boolean = False) As BusinessObjectPermission
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim pkarray As Object() = {objectname.ToUpper, entryname.ToUpper, operationname.ToUpper, order, domainid}
            Return ormBusinessObject.RetrieveDataObject(Of BusinessObjectPermission)(pkArray:=pkarray, domainID:=domainid, dbdriver:=dbdriver, runtimeOnly:=runtimeOnly)
        End Function

        ''' <summary>
        ''' creates the persistency schema
        ''' </summary>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateSchema(Optional silent As Boolean = False) As Boolean
            Return ormBusinessObject.CreateDataObjectSchema(Of BusinessObjectPermission)(silent:=silent)
        End Function
        ''' <summary>
        ''' Handler for the RecordFed Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnFeeding(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnFeeding
            Try
                If _permissionruleProperty IsNot Nothing Then
                    Me.Ruletype = "PROPERTY"
                    Me.Rule = _permissionruleProperty.ToString
                End If

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectPermission.OnInfused", messagetype:=otCoreMessageType.InternalError)
            End Try
        End Sub

        ''' <summary>
        ''' Handler for the OnInfused Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnInfused(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnInfused
            Try
                If Me.Ruletype = "PROPERTY" Then Me._permissionruleProperty = New ObjectPermissionRuleProperty(Me.Rule)
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectPermission.OnInfused", messagetype:=otCoreMessageType.InternalError)
            End Try
        End Sub


        ''' <summary>
        ''' applies the current permission rule on the current user and returns the result
        ''' </summary>
        ''' <param name="user"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CheckFor([user] As User, ByRef [exit] As Boolean, Optional domainid As String = Nothing) As Boolean
            If Not Me.IsAlive(subname:="CheckFor") Then Return False
            Dim result As Boolean
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID

            Try

                '** evaluate the rules
                Select Case _permissionruleProperty.[Enum]
                    '*** check on user rights
                    '*** and on the user's group rights
                    Case otObjectPermissionRuleProperty.DBAccess
                        If _permissionruleProperty.Validate Then
                            Dim accessright = New AccessRightProperty(_permissionruleProperty.Arguments(0).ToString)
                            result = AccessRightProperty.CoverRights(rights:=user.AccessRight, covers:=accessright.[Enum])
                            If Not result Then
                                For Each groupname In user.GroupNames
                                    Dim aGroup As Commons.Group = Commons.Group.Retrieve(groupname:=groupname)
                                    If aGroup IsNot Nothing Then
                                        result = AccessRightProperty.CoverRights(rights:=aGroup.AccessRight, covers:=accessright.[Enum])
                                    Else
                                        CoreMessageHandler(message:="Groupname not found", argument:=_permissionruleProperty.ToString, _
                                                procedure:="ObjectPermission.CheckFor", objectname:=Me.Objectname, messagetype:=otCoreMessageType.InternalError)
                                        '* do not set  a result
                                    End If
                                Next
                            End If

                        Else
                            result = False 'wrong value -> false
                        End If

                        '*** check on membership
                    Case otObjectPermissionRuleProperty.Group
                        If _permissionruleProperty.Validate Then
                            Dim groupname As String = _permissionruleProperty.Arguments(0).ToString
                            If user.GroupNames.Contains(groupname) Then
                                result = True
                            Else
                                result = False
                            End If
                        Else
                            result = False 'wrong value -> false
                        End If

                        '** compare the individual member
                    Case otObjectPermissionRuleProperty.User
                        If _permissionruleProperty.Validate Then
                            Dim username As String = _permissionruleProperty.Arguments(0).ToString
                            If user.Username.ToUpper = username.ToUpper Then
                                result = True
                            Else
                                result = False
                            End If
                        Else
                            result = False 'wrong value -> false
                        End If
                    Case Else
                        CoreMessageHandler(message:="ObjectPermissionRuleProperty not implemented", argument:=_permissionruleProperty.ToString, _
                                            procedure:="ObjectPermission.CheckFor", objectname:=Me.Objectname, messagetype:=otCoreMessageType.InternalError)
                        result = False 'wrong value -> false

                End Select
                '* exit flag
                If (result AndAlso ExitOnTrue) OrElse (Not result AndAlso _exitOnFalse) Then
                    [exit] = True
                End If
                Return result

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectPermission.Checkfor")
                Return False
            End Try


        End Function
    End Class



End Namespace
