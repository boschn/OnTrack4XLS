
REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** BUSINESS OBJECTs: Extensible Properties Classes 
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2013-08-08
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** TO DO Log:
REM ***********             -
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************

Option Explicit On
Imports System.Diagnostics
Imports System.Linq
Imports System.Diagnostics.Debug

Imports OnTrack
Imports OnTrack.Database
Imports OnTrack.XChange
Imports OnTrack.Scheduling
Imports OnTrack.Deliverables
Imports OnTrack.Commons
Imports System.ComponentModel
Imports OnTrack.Core

Namespace OnTrack.ObjectProperties

    ''' <summary>
    ''' class to define an current or alive set of properties 
    ''' </summary>
    ''' <remarks>
    ''' 
    ''' Design Principles:
    ''' 
    ''' 1. Property sets are stand-alone and must exist before a property can be created.
    ''' 
    ''' 2. Properties are added by creating themselves e.g. Property.Create(setid:= ...). It will be added automatically to the set
    ''' 
    ''' 3. On loading the set all the properties will be retrieved as well due to relation.
    ''' 
    ''' </remarks>
    <ormObject(id:=ObjectPropertyCurrentSet.ConstObjectID, version:=1, adddomainbehavior:=True, adddeletefieldbehavior:=True, usecache:=True, _
        modulename:=ConstModuleProperties, Title:="Property Set", description:="definition of a set of properties attachable to bussiness object")> _
    Public Class ObjectPropertyCurrentSet
        Inherits ormBusinessObject

        ''' <summary>
        ''' constants
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstObjectID As String = "PropertyCurrentSet"
        Public Const ConstOpPublish As String = "Publish"
        ''' <summary>
        ''' Table Definition
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=1, usecache:=True)> Public Const ConstPrimaryTableID = "TBLDEFOBJPROPERTYCURRSETS"

        '** primary Keys
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty},
            XID:="OPCS1", title:="Set ID", description:="ID of the property set")> Public Const ConstFNSetID = "SETID"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2, _
            useforeignkey:=otForeignKeyImplementation.NativeDatabase, defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID


        ''' <summary>
        ''' field members
        ''' </summary>
        ''' <remarks></remarks>

        <ormObjectEntry(Datatype:=otDataType.Long, isnullable:=True, _
        XID:="OPCS2", title:="Alive Updc", description:="update count of the alive property set properties")> Public Shadows Const constFNAliveUpdc = "ALIVEUPDC"

        <ormObjectEntry(Datatype:=otDataType.Long, isnullable:=True, _
            XID:="OPCS3", title:="Work Updc", description:="update count of the working property set properties")> Public Shadows Const constFNWorkUpdc = "WORKUPDC"

        <ormObjectEntry(Datatype:=otDataType.List, isnullable:=True, _
         XID:="OPCS5", title:="Business Objects", description:="applicable business objects for this set")> Public Const ConstFNObjects = "OBJECTS"

        <ormObjectEntry(Datatype:=otDataType.Long, defaultvalue:=1, dbdefaultvalue:="1", _
                        XID:="OPCS6", title:="Ordinal", Description:="ordinal of the set")> Public Const ConstFNordinal As String = "ORDINAL"

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
        XID:="OPCS10", title:="Description", description:="description of the property set")> Public Const ConstFNDescription = "DESC"


        ''' <summary>
        ''' Mappings
        ''' </summary>
        ''' <remarks></remarks>

        <ormObjectEntryMapping(EntryName:=ConstFNSetID)> Private _id As String = String.Empty
        <ormObjectEntryMapping(EntryName:=constFNAliveUpdc)> Private _aliveupdc As Long?
        <ormObjectEntryMapping(EntryName:=constFNWorkUpdc)> Private _Workupdc As Long?
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _Description As String
        <ormObjectEntryMapping(EntryName:=ConstFNObjects)> Private _objectids As New List(Of String)
        <ormObjectEntryMapping(EntryName:=ConstFNordinal)> Private _ordinal As Long?
        ''' <summary>
        ''' Relations
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(ObjectPropertySet), cascadeOnCreate:=True, cascadeOnDelete:=True, cascadeOnUpdate:=False, _
            fromEntries:={ConstFNSetID}, toEntries:={ObjectProperty.ConstFNSetID})> Public Const ConstRProperties = "RELSETS"

        <ormObjectEntryMapping(RelationName:=ConstRProperties, infuseMode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand Or otInfuseMode.OnCreate, _
            keyentries:={ObjectPropertySet.ConstFNVersion})> Private WithEvents _setCollection As New ormRelationNewableCollection(Of ObjectPropertySet)(Me, {ObjectPropertySet.ConstFNVersion})

        ''' <summary>
        '''  Work Set
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(ObjectPropertySet), cascadeOnCreate:=False, cascadeOnDelete:=True, cascadeOnUpdate:=True, _
           toPrimaryKeys:={ConstFNSetID, constFNWorkUpdc})> Public Const ConstRWorkSet = "RELWORKSET"

        <ormObjectEntryMapping(RelationName:=ConstRWorkSet, infuseMode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand _
            )> Private WithEvents _workset As ObjectPropertySet

        ''' <summary>
        '''  Alive Set
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(ObjectPropertySet), cascadeOnCreate:=False, cascadeOnDelete:=True, cascadeOnUpdate:=True, _
           toPrimaryKeys:={ConstFNSetID, constFNAliveUpdc})> Public Const ConstRAliveSet = "RELALIVESET"

        <ormObjectEntryMapping(RelationName:=ConstRAliveSet, infuseMode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand _
           )> Private WithEvents _aliveset As ObjectPropertySet


        ''' <summary>
        ''' Dynamic Members
        ''' </summary>
        ''' <remarks></remarks>



#Region "Properties"

        ''' <summary>
        ''' Gets or sets the ordinal.
        ''' </summary>
        ''' <value>The ordinal.</value>
        Public Property Ordinal() As Long?
            Get
                Return Me._ordinal
            End Get
            Set(value As Long?)
                SetValue(ConstFNordinal, value)
            End Set
        End Property

        '' <summary>
        ''' Gets or sets the attached object ids where this object propert set fits.
        ''' </summary>
        ''' <value>The properties.</value>
        Public Property AttachedObjectIDs() As List(Of String)
            Get
                Return Me._objectids
            End Get
            Set(value As List(Of String))
                SetValue(ConstFNObjects, value)
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the description.
        ''' </summary>
        ''' <value>The description.</value>
        Public Property Description() As String
            Get
                Return Me._Description
            End Get
            Set(value As String)
                SetValue(ConstFNDescription, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the alive property set.
        ''' </summary>
        ''' <value>The workset.</value>
        Public Property AliveSet() As ObjectPropertySet
            Get
                Return Me._aliveset
            End Get
            Private Set(value As ObjectPropertySet)
                Me._aliveset = value
                Me.AliveUpdc = value.Updc
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the work property set.
        ''' </summary>
        ''' <value>The workset.</value>
        Public Property Workset() As ObjectPropertySet
            Get
                If _workset Is Nothing Then
                    Me.Workset = _setCollection.AddCreate(domainid:=Me.DomainID, runtimeOnly:=Me.RunTimeOnly)
                End If
                Return Me._workset
            End Get
            Private Set(value As ObjectPropertySet)
                Me._workset = value
                Me.Workupdc = _workset.Updc
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the workupdc.
        ''' </summary>
        ''' <value>The workupdc.</value>
        Public Property Workupdc() As Long?
            Get
                If Not _Workupdc.HasValue Then
                    Return Me.Workset.Updc
                End If
                Return Me._Workupdc
            End Get
            Private Set(value As Long?)
                SetValue(constFNWorkUpdc, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the alive updc of the set.
        ''' </summary>
        ''' <value>The updc.</value>
        Public Property AliveUpdc() As Long?
            Get
                If Not _aliveupdc.HasValue Then
                    If _aliveset Is Nothing AndAlso _workset IsNot Nothing Then
                        Me.Publish()
                    End If
                End If

                Return Me._aliveupdc
            End Get
            Private Set(value As Long?)
                SetValue(constFNAliveUpdc, value)
            End Set
        End Property

        ''' <summary>
        ''' returns the ID of the set
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ID()
            Get
                Return _id
            End Get

        End Property

        ''' <summary>
        ''' returns the collection of Property sets
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Sets As ormRelationCollection(Of ObjectPropertySet)
            Get
                Return _setCollection
            End Get
        End Property

#End Region

        ''' <summary>
        ''' retrieve  the current property set from store
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(id As String, Optional domainid As String = Nothing) As ObjectPropertyCurrentSet
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Return ormBusinessObject.RetrieveDataObject(Of ObjectPropertyCurrentSet)(pkArray:={id.ToUpper, domainid.ToUpper}, domainID:=domainid)
        End Function

        ''' <summary>
        ''' creates a persistable current property set
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(id As String, Optional domainid As String = Nothing) As ObjectPropertyCurrentSet
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Return ormBusinessObject.CreateDataObject(Of ObjectPropertyCurrentSet)(pkArray:={id.ToUpper, domainid.ToUpper}, domainID:=domainid, checkUnique:=True)
        End Function

        ''' <summary>
        ''' Publish and persist the working PropertySet to the alive Set
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ormObjectOperationMethod(Description:="Publish the working property set", title:="Publish", TransactionID:=ConstOpPublish, _
            UIvisible:=True)> _
        Public Function Publish(Optional workerthread As BackgroundWorker = Nothing, _
                                Optional ByRef msglog As BusinessObjectMessageLog = Nothing, _
                               Optional ByVal timestamp As Date? = Nothing) As Boolean
            Dim IsPublishable As Boolean = True
            Dim aValidationResult As otValidationResultType
            Dim aWorkingSet = Me.Workset

            If Not RunTimeOnly AndAlso _
                   Not CurrentSession.ValidateAccessRights(accessrequest:=otAccessRight.ReadUpdateData, domainid:=DomainID, _
                                                                objecttransactions:={Me.ObjectID & "." & ConstOPCreate}) Then
                If Not CurrentSession.RequestUserAccess(accessRequest:=otAccessRight.ReadUpdateData, loginOnFailed:=True, _
                                                         messagetext:="Please provide another user to authorize requested operation", _
                                                        domainid:=DomainID, objecttransactions:={Me.ObjectID & "." & ConstOPCreate}) Then
                    Call CoreMessageHandler(message:="data object operation cannot be executed - permission denied to user", _
                                            objectname:=Me.ObjectID, argument:=ConstOpPublish, _
                                            messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If
            End If

            '* init
            If Not Me.IsAlive(subname:="Publish") Then Return False


            ' TIMESTAMP
            If timestamp Is Nothing Then timestamp = Date.Now


            '** if any of the milestones is changed
            '**
            IsPublishable = True

            '** condition
            If aWorkingSet IsNot Nothing Then

                '''
                ''' Validate the Working Edition
                ''' 
                If msglog Is Nothing Then msglog = aWorkingSet.ObjectMessageLog
                '* not implemented yet
                'aValidationResult = aWorkingSet.Validate(msglog)
                'If aValidationResult = otValidationResultType.FailedNoProceed Then
                '    IsPublishable = False
                'Else
                '    IsPublishable = True
                'End If

                ''' do we need to have some transformation while an edition is alive and now comes up the next one ?
                ''' should be included here
                ''' 

                ''' publish the new edition (working edition) since it is statisfying the validation and checking
                ''' the working edition will become the alive edition
                ''' and a copy of the working edition will be there as new working edition
                ''' 
                If IsPublishable Then
                    If Me.AliveSet IsNot Nothing Then
                        Me.AliveSet.ValidUntil = Date.Now
                        Me.AliveSet.Persist(timestamp)
                    End If

                    '' cannot generate an new updc on a just created object 
                    ''(getmax will not work on unpersisted objects)
                    If Me.AliveSet IsNot Nothing AndAlso Me.AliveSet.IsCreated Then
                        _workset = aWorkingSet.Clone(_aliveset.Updc + 1)
                    Else
                        _workset = aWorkingSet.Clone()
                    End If
                    '** set new working edition
                    ''' here take over the working edition to the alive edition
                    aWorkingSet.ValidFrom = Date.Now
                    Me.Workset = _workset
                    Me.AliveSet = aWorkingSet
                    ''' save the workspace schedule itself and the
                    ''' related objects
                    IsPublishable = MyBase.Persist(timestamp)

                    ''' update the former sets here
                    ''' 
                    AliveSet.UpdateLots(workerthread:=workerthread)

                    ''' build the view here
                    ''' 
                    CreatePropertyValueView(set:=AliveSet)

                    ''' create the compounds of the properties here
                    ''' 
                    For Each aProperty In Me.AliveSet.Properties
                        aProperty.CreateCompoundStructure()
                    Next
                Else
                    '''
                    ''' no publish possible - not even a persist (will fail on the same conditions)
                    ''' 
                End If

            ElseIf Me.IsChanged Or Me.IsCreated Then

                '**** save without Milestone checking
                IsPublishable = MyBase.Persist(timestamp:=timestamp)

            Else
                '** nothing changed
                '***
                Publish = False
                Exit Function
            End If

            Return IsPublishable
        End Function
        '    SELECT      TBLOBJPROPERTYLINKS.FROMOBJECTID, TBLOBJPROPERTYLINKS.fromuid, tblobjpropertylinks.FROMUPDC ,
        '		    TBLOBJPROPERTYLINKS.TOUID, TBLOBJPROPERTYLINKS.toupdc, LOT.PUID, LOT.UPDC, P1.VALUE as '0.0.2.0',P2.value AS '0.0.3.0' , P3.VALUE AS '0.1.0.0', P4.VALUE AS '0.1.3.0', 
        '              P5.VALUE AS '0.1.6.0', P6.VALUE AS '0.2.0.0',  P7.VALUE AS '0.2.3.0', P8.VALUE AS '0.2.6.0', P9.VALUE AS '0.3.0.0',
        '			   P10.VALUE AS '0.4.0.0',  P11.VALUE AS '0.5.0.0',  P12.VALUE AS '0.6.0.0',  P13.VALUE AS '1.0.0.0'
        'FROM            ontrack.dbo.TBLOBJPROPERTYVALUELOTS AS LOT 
        ' INNER JOIN               ontrack.dbo.TBLOBJPROPERTYVALUES AS P1 ON LOT.PUID = P1.PUID AND LOT.UPDC = P1.UPDC AND P1.PROPERTYID = '0.0.2.0'
        ' INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P2 ON LOT.PUID = P2.PUID AND LOT.UPDC = P2.UPDC AND P2.PROPERTYID = '0.0.3.0'
        ' INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P3 ON LOT.PUID = P3.PUID AND LOT.UPDC = P3.UPDC AND P3.PROPERTYID = '0.1.0.0'
        'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P4 ON LOT.PUID = P4.PUID AND LOT.UPDC = P4.UPDC AND P4.PROPERTYID = '0.1.3.0'
        'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P5 ON LOT.PUID = P5.PUID AND LOT.UPDC = P5.UPDC AND P5.PROPERTYID = '0.1.6.0'
        'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P6 ON LOT.PUID = P6.PUID AND LOT.UPDC = P6.UPDC AND P6.PROPERTYID = '0.2.0.0'
        'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P7 ON LOT.PUID = P7.PUID AND LOT.UPDC = P7.UPDC AND P7.PROPERTYID = '0.2.3.0'
        'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P8 ON LOT.PUID = P8.PUID AND LOT.UPDC = P8.UPDC AND P8.PROPERTYID = '0.2.6.0'
        'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P9 ON LOT.PUID = P9.PUID AND LOT.UPDC = P9.UPDC AND P9.PROPERTYID = '0.3.0.0'
        'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P10 ON LOT.PUID = P10.PUID AND LOT.UPDC = P10.UPDC AND P10.PROPERTYID = '0.4.0.0'
        'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P11 ON LOT.PUID = P11.PUID AND LOT.UPDC = P11.UPDC AND P11.PROPERTYID = '0.5.0.0'
        'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P12 ON LOT.PUID = P12.PUID AND LOT.UPDC = P12.UPDC AND P12.PROPERTYID = '0.6.0.0'
        'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P13 ON LOT.PUID = P13.PUID AND LOT.UPDC = P13.UPDC AND P13.PROPERTYID = '1.0.0.0'
        'inner join	ontrack.dbo.TBLOBJPROPERTYLINKS on lot.puid = TBLOBJPROPERTYLINKS.touid 
        ''' <summary>
        ''' Create an SQLView for the PropertyValueLot
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreatePropertyValueView([set] As ObjectPropertySet) As Boolean
            Dim aDBDriver As iormRelationalDatabaseDriver = ot.CurrentOTDBDriver
            Dim viewnames As String = "VWPROPERTYVALUELOT_" & [set].ID & "_" & [set].DomainID & "_V" & [set].Updc
            Dim sqlselectcmd As New Text.StringBuilder("SELECT ")
            Try
                '    SELECT      TBLOBJPROPERTYLINKS.FROMOBJECTID, TBLOBJPROPERTYLINKS.fromuid, tblobjpropertylinks.FROMUPDC ,
                '		    TBLOBJPROPERTYLINKS.TOUID, TBLOBJPROPERTYLINKS.toupdc, LOT.PUID, LOT.UPDC,

                ' objectPropertyLink
                sqlselectcmd.AppendFormat(" [{0}].[{1}]", ot.CurrentSession.OTDBDriver.GetNativeDBObjectName(ObjectPropertyLink.ConstPrimaryTableID), ObjectPropertyLink.ConstFNFromObjectID)
                sqlselectcmd.AppendFormat(",[{0}].[{1}]", ot.CurrentSession.OTDBDriver.GetNativeDBObjectName(ObjectPropertyLink.ConstPrimaryTableID), ObjectPropertyLink.ConstFNFromUid)
                sqlselectcmd.AppendFormat(",[{0}].[{1}]", ot.CurrentSession.OTDBDriver.GetNativeDBObjectName(ObjectPropertyLink.ConstPrimaryTableID), ObjectPropertyLink.ConstFNFromUpdc)
                sqlselectcmd.AppendFormat(",[{0}].[{1}]", ot.CurrentSession.OTDBDriver.GetNativeDBObjectName(ObjectPropertyLink.ConstPrimaryTableID), ObjectPropertyLink.ConstFNToUid)
                sqlselectcmd.AppendFormat(",[{0}].[{1}]", ot.CurrentSession.OTDBDriver.GetNativeDBObjectName(ObjectPropertyLink.ConstPrimaryTableID), ObjectPropertyLink.ConstFNToUpdc)
                ' ObjectPropertyValueLot
                sqlselectcmd.AppendFormat(",LOT.[{0}]", ObjectPropertyValueLot.ConstFNSets)
                sqlselectcmd.AppendFormat(",LOT.[{0}]", ObjectPropertyValueLot.ConstFNSetUPDCs)
                sqlselectcmd.AppendFormat(",LOT.[{0}]", ObjectPropertyValueLot.ConstFNValidFrom)
                sqlselectcmd.AppendFormat(",LOT.[{0}]", ObjectPropertyValueLot.ConstFNValiduntil)

                Dim i As Integer = 1
                ' P1.VALUE as '0.0.2.0', P2.value AS '0.0.3.0' , P3.VALUE AS '0.1.0.0', P4.VALUE AS '0.1.3.0', 
                For Each aProperty In [set].Properties
                    sqlselectcmd.AppendFormat(",P{0}.{1} as '{2}'", i, ObjectPropertyValue.ConstFNValue, aProperty.ID)
                    i += 1
                Next

                '** updated
                sqlselectcmd.AppendFormat(",LOT.[{0}]", ObjectPropertyValueLot.ConstFNUpdatedOn)
                'FROM   ontrack.dbo.TBLOBJPROPERTYVALUELOTS AS LOT 
                sqlselectcmd.AppendLine(" FROM  [" & ot.CurrentSession.OTDBDriver.GetNativeDBObjectName(ObjectPropertyValueLot.ConstPrimaryTableID) & "] AS LOT ")
                'inner join	ontrack.dbo.TBLOBJPROPERTYLINKS on lot.puid = TBLOBJPROPERTYLINKS.touid 
                sqlselectcmd.AppendFormat(" INNER JOIN [{0}] ON LOT.[{1}] = {0}.[{2}] AND lot.[{3}] = {0}.[{4}]", _
                                          ot.CurrentSession.OTDBDriver.GetNativeDBObjectName(ObjectPropertyLink.ConstPrimaryTableID), ObjectPropertyValueLot.constFNUID, ObjectPropertyLink.ConstFNToUid, _
                                          ObjectPropertyValueLot.ConstFNVersion, ObjectPropertyLink.ConstFNToUpdc)

                i = 1
                'INNER JOIN	ontrack.dbo.TBLOBJPROPERTYVALUES AS P13 ON LOT.PUID = P13.PUID AND LOT.UPDC = P13.UPDC AND P13.PROPERTYID = '1.0.0.0'

                For Each aProperty In [set].Properties
                    sqlselectcmd.AppendFormat(" LEFT OUTER JOIN [{0}] AS P{1} ON LOT.{2} = P{1}.{3} AND LOT.{4} = P{1}.{5} AND P{1}.{6} = '{7}'" _
                                              , ot.CurrentSession.OTDBDriver.GetNativeDBObjectName(ObjectPropertyValue.ConstPrimaryTableID), _
                                              i, _
                                              ObjectPropertyValueLot.constFNUID, _
                                              ObjectPropertyValue.constFNUID, _
                                              ObjectPropertyValueLot.ConstFNVersion, _
                                              ObjectPropertyValue.ConstFNVersion, _
                                              ObjectPropertyValue.ConstFNPropertyID, _
                                              aProperty.ID)
                    i += 1
                Next

                ''' create
                ''' 
                Return aDBDriver.GetView(createOrAlter:=True, name:=viewnames, sqlselect:=sqlselectcmd.ToString) Is Nothing

            Catch ex As Exception
                CoreMessageHandler("failed to build view on property set", argument:=sqlselectcmd.ToString, exception:=ex, _
                                    procedure:="ObjectPropertyCurrentSet.CreatePropertyValueView")
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Handler for the OnAdded event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>

        Private Sub SetCollection_OnAdded(sender As Object, e As Database.ormRelationCollection(Of ObjectPropertySet).EventArgs) Handles _setCollection.OnAdded

            '''  add some event handling
            ''' 
            AddHandler e.Dataobject.OnCreated, AddressOf Me.CurrentPropertySet_OnCreatedProperty
            AddHandler e.Dataobject.OnDeleted, AddressOf Me.CurrentPropertySet_OnDeletedProperty
            AddHandler e.Dataobject.OnPersisted, AddressOf Me.CurrentPropertySet_OnPersistedProperty
        End Sub

        ''' <summary>
        ''' Event Handler for ObjectProperty OnPersisted Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub CurrentPropertySet_OnPersistedProperty(sender As Object, e As ormDataObjectEventArgs)
            ' cascading on update in the creation process will also lead to save the initial current sets BUT
            ' beware this might lead to an recursion loop if the save is comming from the CurrentSet (persisted)
            'If TryCast(e.DataObject, ObjectPropertySet).PropertiesChanged Then
            '   Me.Publish()
            'End If
        End Sub

        ''' <summary>
        ''' Event Handler for ObjectProperty OnCreated Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub CurrentPropertySet_OnCreatedProperty(sender As Object, e As ormDataObjectEventArgs)

        End Sub
        ''' <summary>
        ''' Event Handler for ObjectProperty OnDeleted Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub CurrentPropertySet_OnDeletedProperty(sender As Object, e As ormDataObjectEventArgs)
            Throw New NotImplementedException("Deleting of PropertySet not implemented")
        End Sub

        ''' <summary>
        ''' Handler for OnCreated Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectPropertySet_OnCreated(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreated

            ''' check if there is an PropertySet - set the working set
            ''' 
            If _setCollection.Count = 0 Then
                Dim aSet As ObjectPropertySet = ObjectPropertySet.Create(id:=Me.ID, domainid:=Me.DomainID)
                If aSet Is Nothing Then aSet = ObjectPropertySet.Retrieve(id:=Me.ID, updc:=1, domainid:=Me.DomainID)
                Me.Workset = aSet
                _setCollection.Add(aSet)
            Else
                Me.Workset = _setCollection.First
            End If
            '** defaults will be set as the entries are changed here 

        End Sub



        ''' <summary>
        ''' OnNew EventHandler of the PropertySet Key
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub SetCollection_OnRequestKeys(sender As Object, e As Database.ormRelationNewableCollection(Of ObjectPropertySet).EventArgs) Handles _setCollection.RequestKeys
            e.Keys = {Me.ID, Nothing} 'nothing means create new Unique key
            e.Cancel = False
        End Sub

        ''' <summary>
        ''' OnEntryChanged Event to set the defaults for all the sets
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectPropertyCurrentSet_OnEntryChanged(sender As Object, e As ormDataObjectEntryEventArgs) Handles Me.OnEntryChanged
            If e.ObjectEntryName = ConstFNDescription OrElse e.ObjectEntryName = ConstFNObjects OrElse e.ObjectEntryName = ConstFNordinal Then
                For Each aPropertySet In Me.Sets
                    If e.ObjectEntryName = ConstFNDescription Then aPropertySet.Description = Me.Description
                    If e.ObjectEntryName = ConstFNordinal Then aPropertySet.Ordinal = Me.Ordinal
                    If e.ObjectEntryName = ConstFNObjects Then aPropertySet.AttachedObjectIDs = Me.AttachedObjectIDs
                Next
            End If
        End Sub
    End Class

    ''' <summary>
    ''' class to define a set of properties attachable to other business objects
    ''' </summary>
    ''' <remarks>
    ''' 
    ''' Design Principles:
    ''' 
    ''' 1. Property sets are stand-alone and must exist before a property can be created.
    ''' 
    ''' 2. Properties are added by creating themselves e.g. Property.Create(setid:= ...). It will be added automatically to the set
    ''' 
    ''' 3. On loading the set all the properties will be retrieved as well due to relation.
    ''' 
    ''' </remarks>
    <ormObject(id:=ObjectPropertySet.ConstObjectID, version:=1, adddomainbehavior:=True, adddeletefieldbehavior:=True, usecache:=True, _
        modulename:=ConstModuleProperties, Title:="Property Set", description:="definition of a set of properties attachable to bussiness object")> _
    Public Class ObjectPropertySet
        Inherits ormBusinessObject

        Public Const ConstObjectID = "PropertySet"

        ''' <summary>
        ''' Table Definition
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=1, usecache:=True)> Public Const ConstPrimaryTableID = "TBLDEFOBJPROPERTYSETS"

        '** primary Keys
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty},
            XID:="OPS1", title:="Set ID", description:="ID of the property set")> Public Const ConstFNSetID = "SETID"

        <ormObjectEntry(Datatype:=otDataType.Long, dbdefaultvalue:="1", defaultvalue:=1, PrimaryKeyOrdinal:=2, _
         XID:="OPS2", title:="Update Count of Property Set", description:="update count of the property set properties")> Public Shadows Const ConstFNVersion = "VERSION"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=3 _
         , useforeignkey:=otForeignKeyImplementation.NativeDatabase, defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
          XID:="OPS3", title:="Description", description:="description of the property set")> Public Const ConstFNDescription = "DESC"

        <ormObjectEntry(Datatype:=otDataType.List, isnullable:=True, _
          XID:="OPS4", title:="Properties", description:="properties of the object property set")> Public Const ConstFNProperties = "PROPERTIES"

        <ormObjectEntry(Datatype:=otDataType.List, isnullable:=True, _
         XID:="OPS5", title:="Business Objects", description:="applicable business objects for this set")> Public Const ConstFNObjects = "OBJECTS"

        <ormObjectEntry(Datatype:=otDataType.Long, defaultvalue:=1, dbdefaultvalue:="1", _
                        XID:="OPS6", title:="Ordinal", Description:="ordinal of the set")> Public Const ConstFNordinal As String = "ORDINAL"

        <ormObjectEntry(Datatype:=otDataType.Timestamp, isnullable:=True, _
          XID:="OPS11", title:="Valid from", description:="timestamp the set is valid from on")> Public Shadows Const ConstFNValidFrom = "VALIDFROM"

        <ormObjectEntry(Datatype:=otDataType.Timestamp, isnullable:=True, _
           XID:="OPS12", title:="Valid Until", description:="timestamp the set is valid until")> Public Shadows Const ConstFNValidTo = "VALIDTO"

        <ormObjectEntry(Datatype:=otDataType.Timestamp, isnullable:=True, _
           XID:="OPS13", title:="Last Property Change", description:="timestamp of the last property change")> Public Shadows Const constFNPropChanged = "PCHANGEDON"


        ''' <summary>
        ''' Mappings
        ''' </summary>
        ''' <remarks></remarks>

        <ormObjectEntryMapping(EntryName:=ConstFNSetID)> Private _id As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNVersion)> Private _updc As Long = 1
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _description As String
        <ormObjectEntryMapping(EntryName:=ConstFNProperties)> Private _propertyids As New List(Of String)
        <ormObjectEntryMapping(EntryName:=ConstFNObjects)> Private _objectids As New List(Of String)
        <ormObjectEntryMapping(EntryName:=ConstFNordinal)> Private _ordinal As Long?

        <ormObjectEntryMapping(EntryName:=constFNPropChanged)> Private _lastPropertyChangedOn As DateTime?
        <ormObjectEntryMapping(EntryName:=ConstFNValidFrom)> Private _validFrom As DateTime?
        <ormObjectEntryMapping(EntryName:=ConstFNValidTo)> Private _validTo As DateTime?

        ''' <summary>
        ''' Relations
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(ObjectProperty), cascadeOnDelete:=True, cascadeOnUpdate:=True, _
            fromEntries:={ConstFNSetID, ConstFNVersion}, toEntries:={ObjectProperty.ConstFNSetID, ObjectProperty.ConstFNSetUPDC})> Public Const ConstRProperties = "PROPERTIES"

        <ormObjectEntryMapping(RelationName:=ConstRProperties, infuseMode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand, _
            keyentries:={ObjectProperty.ConstFNPropertyID})> Private WithEvents _propertiesCollection As New ormRelationCollection(Of ObjectProperty)(Me, {ObjectProperty.ConstFNPropertyID})


        ''' <summary>
        ''' Dynamic Members
        ''' </summary>
        ''' <remarks></remarks>

        Private _propertiesChanged As Boolean = False ' true if properties are added or deleted
        Private _currentset As ObjectPropertyCurrentSet ' backlink

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the properties changed.
        ''' </summary>
        ''' <value>The properties changed.</value>
        Public Property PropertiesChanged() As Boolean
            Get
                Return Me._propertiesChanged
            End Get
            Private Set(value As Boolean)
                Me._propertiesChanged = value
            End Set
        End Property

        ''' <summary>
        ''' returns the PropertyCurrentSet of this PropertySet
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property PropertyCurrentSet As ObjectPropertyCurrentSet
            Get
                If Not Me.IsAlive("PropertyCurrentSet") Then Return Nothing

                If _currentset Is Nothing Then
                    _currentset = ObjectPropertyCurrentSet.Retrieve(id:=Me.ID, domainid:=Me.DomainID)
                End If

                Return _currentset
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the valid until timestamp.
        ''' </summary>
        ''' <value>The valid to.</value>
        Public Property [ValidUntil]() As DateTime?
            Get
                Return Me._validTo
            End Get
            Set(value As DateTime?)
                SetValue(ConstFNValidTo, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the valid from timestamp.
        ''' </summary>
        ''' <value>The valid from.</value>
        Public Property ValidFrom() As DateTime?
            Get
                Return Me._validFrom
            End Get
            Set(value As DateTime?)
                SetValue(ConstFNValidFrom, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the last property changed on.
        ''' </summary>
        ''' <value>The last property changed on.</value>
        Public Property LastPropertyChangedOn() As DateTime?
            Get
                Return Me._lastPropertyChangedOn
            End Get
            Private Set(value As DateTime?)
                SetValue(constFNPropChanged, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the updc.
        ''' </summary>
        ''' <value>The updc.</value>
        Public ReadOnly Property Updc() As Long
            Get
                Return Me._updc
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the ordinal.
        ''' </summary>
        ''' <value>The ordinal.</value>
        Public Property Ordinal() As Long?
            Get
                Return Me._ordinal
            End Get
            Set(value As Long?)
                SetValue(ConstFNordinal, value)
            End Set
        End Property

        '' <summary>
        ''' Gets or sets the attached object ids where this object propert set fits.
        ''' </summary>
        ''' <value>The properties.</value>
        Public Property AttachedObjectIDs() As List(Of String)
            Get
                Return Me._objectids
            End Get
            Set(value As List(Of String))
                SetValue(ConstFNObjects, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the properties ids.
        ''' </summary>
        ''' <value>The properties.</value>
        Public ReadOnly Property PropertyIDs() As List(Of String)
            Get
                Return Me._propertyids
            End Get
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
                SetValue(ConstFNDescription, value)
            End Set
        End Property

        ''' <summary>
        ''' returns the ID of the configuration
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ID()
            Get
                Return _id
            End Get

        End Property

        ''' <summary>
        ''' returns the collection of Properties in this set
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Properties As ormRelationCollection(Of ObjectProperty)
            Get
                Return _propertiesCollection
            End Get
        End Property

#End Region

        ''' <summary>
        ''' Update and initialize the linked value lots of  former versions of the set to this one
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateLots(Optional workerthread As BackgroundWorker = Nothing, _
                                   Optional timestamp As DateTime? = Nothing) As Boolean
            '** get all lots which have this set
            Dim result As Boolean = True
            If Not timestamp.HasValue Then timestamp = DateTime.Now
            Dim aList As List(Of ObjectPropertyLink) = ObjectPropertyLink.AllBySet(setid:=Me.ID)
            Dim i As Long
            Dim max As Long = aList.Count

            For Each aLink In aList
                '** get the links to increase the valuelot version
                result = result And aLink.UpdateValueLot2Set(setid:=Me.ID, setupdc:=Me.Updc, timestamp:=timestamp, domainid:=Me.DomainID)
                If workerthread IsNot Nothing Then
                    workerthread.ReportProgress(i / max, "updating value lots")
                End If
            Next

            Return result
        End Function
        ''' <summary>
        ''' clone the object and its members
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <param name="VERSION"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Clone(Optional ByVal updc As Long? = Nothing) As ObjectPropertySet
            Dim pkArray() As Object = {Me.ID, updc}
            Dim aClone = MyBase.Clone(Of ObjectPropertySet)(newpkarray:=pkArray, runtimeOnly:=Me.RunTimeOnly)
            For Each aProperty As ObjectProperty In Me.Properties
                aProperty.Clone(aClone.ID, aClone.Updc, aProperty.ID, aProperty.DomainID)
                'aClone.Properties.Add(aPropertyClone) adds by event
            Next
            Return aClone
        End Function
        ''' <summary>
        ''' retrieve  the property set from store
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(id As String, updc As Long, Optional domainid As String = Nothing) As ObjectPropertySet
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Return ormBusinessObject.RetrieveDataObject(Of ObjectPropertySet)(pkArray:={id.ToUpper, updc, domainid.ToUpper}, domainID:=domainid)
        End Function

        ''' <summary>
        ''' creates a persistable property set
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(id As String, Optional updc As Long? = Nothing, Optional domainid As String = Nothing) As ObjectPropertySet
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Return ormBusinessObject.CreateDataObject(Of ObjectPropertySet)(pkArray:={id.ToUpper, updc, domainid.ToUpper}, domainID:=domainid, checkUnique:=True)
        End Function


        ''' <summary>
        ''' Handler for the OnAdded event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>

        Private Sub PropertiesCollection_OnAdded(sender As Object, e As Database.ormRelationCollection(Of ObjectProperty).EventArgs) Handles _propertiesCollection.OnAdded
            If Not _propertyids.Contains(e.Dataobject.ID) Then
                _propertyids.Add(e.Dataobject.ID)
            End If
            '''  add some event handling
            ''' 
            AddHandler e.Dataobject.OnCreated, AddressOf Me.PropertySet_OnCreatedProperty
            AddHandler e.Dataobject.OnDeleted, AddressOf Me.PropertySet_OnDeletedProperty
        End Sub


        ''' <summary>
        ''' Event Handler for ObjectProperty OnCreated Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub PropertySet_OnCreatedProperty(sender As Object, e As ormDataObjectEventArgs)
            Me._propertiesChanged = True
        End Sub
        ''' <summary>
        ''' Event Handler for ObjectProperty OnDeleted Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub PropertySet_OnDeletedProperty(sender As Object, e As ormDataObjectEventArgs)
            Me.Properties.Remove(e.DataObject)
            Me._propertiesChanged = True
        End Sub

        ''' <summary>
        ''' OnCreated Handler
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectPropertySet_OnCreated(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreated

            ''' create also aCurrentSet if it doesnot exist
            Dim aCurrentSet As ObjectPropertyCurrentSet = ObjectPropertyCurrentSet.Retrieve(id:=Me.ID, domainid:=Me.DomainID)
            If aCurrentSet Is Nothing Then
                aCurrentSet = ObjectPropertyCurrentSet.Create(id:=Me.ID, domainid:=Me.DomainID)
                aCurrentSet.Sets.Add(Me)
            End If
        End Sub

        ''' <summary>
        ''' Handler for OnCreating Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectPropertySet_OnCreating(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreating
            Dim anUpdc As Long? = e.Record.GetValue(ConstFNVersion)
            Dim aSetid As String = e.Record.GetValue(ConstFNSetID)
            '* new uid
            If Not anUpdc.HasValue OrElse anUpdc = 0 Then
                anUpdc = Nothing 'reset to norhing
                Dim primarykey As Object() = {aSetid, anUpdc}
                If e.DataObject.ObjectPrimaryContainerStore.CreateUniquePkValue(pkArray:=primarykey) Then
                    e.Record.SetValue(ConstFNVersion, primarykey(1)) ' to be created
                    e.Result = True
                    e.Proceed = True
                Else
                    CoreMessageHandler(message:="primary keys could not be created ?!", procedure:="ObjectPropertySet.ObjectPropertySet_OnCreating", _
                                       messagetype:=otCoreMessageType.InternalError)
                End If

            End If
        End Sub
        ''' <summary>
        ''' OnFed Handler for some updating
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectPropertySet_OnFed(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnFed
            If Me._propertiesChanged Then
                Me.LastPropertyChangedOn = Date.Now
                Me._propertiesChanged = False
            End If
        End Sub
    End Class

    ''' <summary>
    ''' class for ObjectProperty Extended properties (special settings for ObjectProperty)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ObjectPropertyExtProperty
        Inherits AbstractPropertyFunction(Of otObjectPropertyExtProperty)
        Public Const CopyInitialValueFrom = "COPYINITIALVALUEFROM"

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="propertystring"></param>
        ''' <remarks></remarks>
        Public Sub New(propertystring As String)
            MyBase.New(propertystring:=propertystring)
        End Sub
        ''' <summary>
        ''' validates the property
        ''' </summary>
        ''' <param name="property"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Validate([property] As ObjectPropertyExtProperty) As Boolean
            Try
                Select Case [property].Enum
                    Case otObjectPropertyExtProperty.CopyInitialValueFrom
                        '''
                        ''' optional arguemnt of foreign key
                        ''' 
                        If [property].Arguments IsNot Nothing AndAlso [property].Arguments.Count > 1 Then
                            CoreMessageHandler(message:="first argument is property id - more arguments specified as required ", argument:=[property].ToString, _
                                           procedure:="ObjectPropertyExtProperty.Validate", messagetype:=otCoreMessageType.InternalError)
                            Return False
                        End If

                    Case Else
                        Return True
                End Select

                Return True
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectPropertyExtProperty.Validate")
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Apply the Property function to a value
        ''' </summary>
        ''' <param name="in"></param>
        ''' <param name="out"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Apply(ByRef value As Object, valuelot As ObjectPropertyValueLot, Optional msglog As BusinessObjectMessageLog = Nothing) As Boolean

            Try
                ''' check on empty arrays or list
                ''' 
                If _property = otObjectPropertyExtProperty.CopyInitialValueFrom Then
                    Dim avalue As Object
                    If valuelot.GetPropertyValue(id:=_arguments(0), value:=avalue) Then
                        value = avalue
                        Return True
                    Else
                        Return False
                    End If
                End If

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectPropertyExtProperty.Apply")
                Return True
            End Try

        End Function
        ''' <summary>
        ''' returns the enumeration value
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToEnum() As otObjectPropertyExtProperty
            Return AbstractPropertyFunction(Of otObjectPropertyExtProperty).ToEnum(_property)
        End Function
    End Class
    ''' <summary>
    ''' Enumeration of the validation properties
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otObjectPropertyExtProperty
        <Description(ObjectPropertyExtProperty.CopyInitialValueFrom)> CopyInitialValueFrom

    End Enum

    ''' <summary>
    ''' class to define a configuration entity as member of a configuration attachable to other business objects
    ''' </summary>
    ''' <remarks>
    ''' 
    ''' Design principles:
    ''' 
    ''' 1. Properties can be created by Created -> will be added to the set by the property itself. If set doesnot exist also the property will not create
    ''' 2. Class inherits allEntries from ObjectCompoundEntry -> added to ConstPrimaryTableID (new Table)
    ''' 3. the Class Property PropertySet is the cached backlink to the Set ( will not be loaded on infuse -> creates loops)
    ''' 
    ''' </remarks>
    <ormObject(id:=ObjectProperty.ConstObjectID, version:=1, adddomainbehavior:=True, adddeletefieldbehavior:=True, usecache:=True, _
        modulename:=ConstModuleProperties, Title:="property definition", description:="definition of a property attachable to business objects")> _
    Public Class ObjectProperty
        Inherits ormObjectCompoundEntry

        Public Const ConstObjectID = "OBJECTPROPERTY"

        ''' <summary>
        ''' Table Definition
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=1, usecache:=True)> Public Const ConstPrimaryTableID = "TBLDEFOBJPROPERTY"

        ''' <summary>
        ''' Index 
        ''' </summary>
        ''' <remarks></remarks>
        <ormIndex(tableid:=ConstPrimaryTableID, columnname1:=ConstFNPropertyID, columnname2:=ConstFNSetID, columnname3:=ConstFNIsDeleted)> Public Const ConstINProperty = "INDEXPROPERTYIDS"
        <ormIndex(tableid:=ConstPrimaryTableID, columnname1:=ConstFNObjectName, columnname2:=ConstFNType, columnname3:=ConstFNIsDeleted, columnname4:=ConstFNEntryName, enabled:=False)> Public Const constINDtypes = "indexTypes"

        ''' <summary>
        ''' primary keys
        ''' </summary>
        ''' <remarks></remarks>

        <ormObjectEntry(referenceObjectEntry:=ObjectPropertySet.ConstObjectID & "." & ObjectPropertySet.ConstFNSetID, PrimaryKeyOrdinal:=1, _
            lookupPropertyStrings:={LookupProperty.UseForeignKey & "(" & constFKSet & ")"}, _
            validationPropertyStrings:={ObjectValidationProperty.NotEmpty, ObjectValidationProperty.UseLookup})> Public Const ConstFNSetID = ObjectPropertySet.ConstFNSetID

        <ormObjectEntry(referenceObjectEntry:=ObjectPropertySet.ConstObjectID & "." & ObjectPropertySet.ConstFNVersion, PrimaryKeyOrdinal:=2, _
            XID:="OPR2")> Public Shadows Const ConstFNSetUPDC = "SETUPDC"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=3, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty},
            XID:="OPR3", title:="Name", description:="ID of the property")> Public Const ConstFNPropertyID = "PROPERTYID"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=4 _
         , useforeignkey:=otForeignKeyImplementation.None, defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        <ormForeignKey(entrynames:={ConstFNSetID, ConstFNSetUPDC, ConstFNDomainID}, _
            foreignkeyreferences:={ObjectPropertySet.ConstObjectID & "." & ObjectPropertySet.ConstFNSetID, _
                ObjectPropertySet.ConstObjectID & "." & ObjectPropertySet.ConstFNVersion, _
                ObjectPropertySet.ConstObjectID & "." & ObjectPropertySet.ConstFNDomainID}, _
            useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const constFKSet = "FK_ObjPropertySet"

        ''' <summary>
        ''' other fields
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.List, isnullable:=True, _
          XID:="OPR4", title:="Extended Properties", description:="internal properties of the object property")> Public Shadows Const ConstFNExtProperties = "EXTPROPERTIES"


        ''' <summary>
        ''' Shadows with own XID
        ''' </summary>
        ''' <remarks></remarks>
        ''' 


        ''' <summary>
        ''' disabled the inherited fields
        ''' </summary>
        ''' <remarks> 
        ''' this is only disabled if the value is exactly the same as inherited, since
        ''' the field value is taken as id/entryname of the entry and stored but the name of the constant is only used
        ''' for inheritage
        ''' </remarks>
        <ormObjectEntry(enabled:=False)> Public Const ConstFNObjectName As String = ormAbstractEntryDefinition.ConstFNObjectID
        <ormObjectEntry(enabled:=False)> Public Const ConstFNEntryName As String = ormAbstractEntryDefinition.ConstFNEntryName

        ''' <summary>
        ''' Mappings
        ''' </summary>
        ''' <remarks></remarks>

        <ormObjectEntryMapping(EntryName:=ConstFNSetID)> Private _setid As String = String.Empty
        <ormObjectEntryMapping(entryname:=ConstFNPropertyID)> Private _ID As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNExtProperties)> Private _extpropertiesStrings As String()
        <ormObjectEntryMapping(EntryName:=ConstFNSetUPDC)> Private _setupdc As Long?

        ''' <summary>
        '''  further dynamic 
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        Private _relationpath As String() = {ObjectPropertyLink.ConstObjectID & "." & ObjectPropertyLink.ConstRPropertyValueLot, _
                                         ObjectPropertyValueLot.ConstObjectID & "." & ObjectPropertyValueLot.ConstRValues, _
                                         ObjectPropertyValue.ConstObjectID}
        Private _set As ObjectPropertySet 'cached

        '** disable some of the inherited columns
        ''' <summary>
        ''' Disabled
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(isactive:=False)> Public Const ConstFNFinalObjectID As String = "ctblname"
        <ormObjectEntry(isactive:=False)> Public Const ConstFNCompoundRelation As String = "crelation"
        <ormObjectEntry(isactive:=False)> Public Const ConstFNCompoundIDEntryname As String = "cidfield"
        <ormObjectEntry(isactive:=False)> Public Const ConstFNCompoundValueEntryName As String = "cvalfield"
        <ormObjectEntry(isactive:=False)> Public Const ConstFNCompoundSetter As String = "CSETTER"
        <ormObjectEntry(isactive:=False)> Public Const ConstFNCompoundGetter As String = "CGETTER"
        <ormObjectEntry(isactive:=False)> Public Const ConstFNCompoundValidator As String = "CVALIDATE"

        ''' <summary>
        ''' Dynamic members
        ''' </summary>
        ''' <remarks></remarks>

        Private _extendedProperties As New List(Of ObjectPropertyExtProperty)
        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
            MyBase.DeRegisterHandler() ' deregister the derived abstractentry handlers !
            AddHandler ormBusinessObject.OnCreating, AddressOf ObjectProperty_OnCreating
            AddHandler ormBusinessObject.OnCreated, AddressOf ObjectProperty_OnCreated
            AddHandler ormBusinessObject.OnInfused, AddressOf ObjectProperty_OnInfused
            AddHandler ormBusinessObject.OnEntryChanged, AddressOf AbstractEntryDefinition_OnEntryChanged '' keep this one
        End Sub

#Region "Properties"
        ''' <summary>
        ''' Gets or sets the Updc of the set.
        ''' </summary>
        ''' <value>The setupdc.</value>
        Public Property Setupdc() As Long?
            Get
                Return Me._setupdc
            End Get
            Set(value As Long?)
                SetValue(ConstFNSetUPDC, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets the entity ID.
        ''' </summary>
        ''' <value>The entity.</value>
        Public ReadOnly Property ID() As String
            Get
                Return Me._ID
            End Get
        End Property

        ''' <summary>
        ''' returns the ID of the set
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SetID As String
            Get
                Return _setid
            End Get
        End Property
        ''' <summary>
        ''' returns the property set
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property PropertySet As ObjectPropertySet
            Get

                If _set Is Nothing Then
                    _set = ObjectPropertySet.Retrieve(id:=_setid, updc:=Me.Setupdc.Value, domainid:=Me.DomainID)
                    If _set Is Nothing Then
                        CoreMessageHandler(message:="object property set does not exist", procedure:="ObjectProperty.PropertySet", _
                                           messagetype:=otCoreMessageType.ApplicationError, _
                                           argument:=_setid)
                        Return Nothing
                    End If
                End If
                Return _set
            End Get
        End Property


        ''' <summary>
        ''' Gets or sets the properties of the object property definition.
        ''' </summary>
        ''' <value>The properties.</value>
        Public Property ExtendedPropertyStrings() As String()
            Get
                Return Me._extpropertiesStrings
            End Get
            Set(value As String())
                Me._extpropertiesStrings = value
            End Set
        End Property

        ''' <summary>
        ''' gets or sets the validation properties
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ExtendedProperties As List(Of ObjectPropertyExtProperty)
            Get
                Return _extendedProperties
            End Get
            Set(value As List(Of ObjectPropertyExtProperty))
                Dim aPropertyString As New List(Of String)
                For Each aP In value
                    aPropertyString.Add(aP.ToString)
                Next
                If SetValue(entryname:=ConstFNExtProperties, value:=aPropertyString.ToArray) Then
                    _extendedProperties = value
                End If
            End Set
        End Property


#End Region

        ''' <summary>
        ''' clone the object 
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <param name="VERSION"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Clone(setid As String, setupdc As Long, ID As String, Optional domainid As String = Nothing) As ObjectProperty
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim pkarray As Object() = {setid.ToUpper, setupdc, ID.ToUpper, domainid}
            Return MyBase.Clone(Of ObjectProperty)(newpkarray:=pkarray, runtimeOnly:=Me.RunTimeOnly)
        End Function

        ''' <summary>
        ''' Handles OnCreating 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ObjectProperty_OnCreating(sender As Object, e As ormDataObjectEventArgs)
            Dim my As ObjectProperty = TryCast(e.DataObject, ObjectProperty)

            If my IsNot Nothing Then
                Dim setid As String = e.Record.GetValue(ConstFNSetID)
                If setid Is Nothing Then
                    CoreMessageHandler(message:="object property set is not set in object property creating", procedure:="ObjectProperty.OnCreating", _
                                       messagetype:=otCoreMessageType.ApplicationError, _
                                       argument:=my.SetID)
                    e.AbortOperation = True
                    Return
                End If
                ''' even if it is early to retrieve the set and set it (since this might disposed since we have not run through checkuniqueness and cache)
                ''' we need to check on the object here

                Dim setupdc As Long? = e.Record.GetValue(ConstFNSetUPDC)
                Dim domainid As String = e.Record.GetValue(ConstFNDomainID)
                If domainid Is Nothing Then domainid = Me.DomainID
                If Not setupdc.HasValue Then
                    Dim aCurrentSet As ObjectPropertyCurrentSet = ObjectPropertyCurrentSet.Retrieve(id:=setid, domainid:=domainid)
                    If aCurrentSet IsNot Nothing Then
                        setupdc = aCurrentSet.Workupdc
                        e.Record.SetValue(ConstFNSetUPDC, setupdc)
                    Else
                        CoreMessageHandler(message:="object property current set does not exist in the database", procedure:="ObjectProperty.OnCreated", _
                                      messagetype:=otCoreMessageType.ApplicationError, _
                                      argument:=setid)
                        e.AbortOperation = True
                    End If
                End If

                _set = ObjectPropertySet.Retrieve(id:=setid, updc:=setupdc, domainid:=domainid)
                If _set Is Nothing Then
                    CoreMessageHandler(message:="object property set doesn ot exist", procedure:="ObjectProperty.OnCreated", _
                                       messagetype:=otCoreMessageType.ApplicationError, _
                                       argument:=setid)
                    e.AbortOperation = True
                    Return
                End If
            End If
        End Sub

        ''' <summary>
        ''' Handles OnCreated and Relation to ConfigSet
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ObjectProperty_OnCreated(sender As Object, e As ormDataObjectEventArgs)
            Dim my As ObjectProperty = TryCast(e.DataObject, ObjectProperty)

            If my IsNot Nothing Then
                If Me.PropertySet Is Nothing Then
                    CoreMessageHandler(message:="object propert set doesnot exist", procedure:="ObjectProperty.OnCreated", _
                                      messagetype:=otCoreMessageType.ApplicationError, _
                                       argument:=my.SetID)
                    e.AbortOperation = True
                    Return
                End If
            End If

        End Sub
        ''' <summary>
        ''' Handles OnCreating and Relation to Configset
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ObjectProperty_OnInfused(sender As Object, e As ormDataObjectEventArgs)
            Dim my As ObjectProperty = TryCast(e.DataObject, ObjectProperty)

            ''' infuse is called on create as well as on retrieve / inject 
            ''' only on the create case we need to add to the properties otherwise
            ''' propertyset will load the property
            ''' or the property will stand alone
            If my IsNot Nothing AndAlso e.Infusemode = otInfuseMode.OnCreate Then
                If Me.PropertySet Is Nothing Then
                    CoreMessageHandler(message:="object propert set doesnot exist", procedure:="ObjectProperty.OnCreated", _
                                      messagetype:=otCoreMessageType.ApplicationError, _
                                       argument:=my.SetID)
                    e.AbortOperation = True
                    Return
                Else
                    Me.PropertySet.Properties.Add(my)
                End If
            End If
            ''** the property list in Object presentation

            If _extpropertiesStrings IsNot Nothing Then
                Dim aList As New List(Of ObjectPropertyExtProperty)
                For Each propstring In _extpropertiesStrings
                    Try
                        Dim aProperty As ObjectPropertyExtProperty = New ObjectPropertyExtProperty(propstring)
                        aList.Add(aProperty)
                    Catch ex As Exception
                        Call CoreMessageHandler(procedure:="ObjetcProperty_OnInfused", exception:=ex)
                    End Try
                Next
                _extendedProperties = aList ' assign
            End If

        End Sub
        ''' <summary>
        ''' set the values of a compound from a property
        ''' </summary>
        ''' <param name="compound"></param>
        ''' <param name="property"></param>
        ''' <remarks></remarks>
        Private Sub SetCompound(compound As ormObjectCompoundEntry)
            ''' set the values
            ''' 
            With compound
                '' type and field

                .Aliases = Me.Aliases
                .Datatype = Me.Datatype
                .IsNullable = Me.IsNullable
                .DefaultValue = Me.DefaultValue
                .Size = Me.Size
                .InnerDatatype = Me.InnerDatatype
                .Version = Me.Version
                .Title = Me.Title
                .PropertyStrings = .PropertyStrings
                .Description = Me.Description
                ' ordinal calculate an ordinal
                .Ordinal = 1000 + (Me.PropertySet.Ordinal - 1) * 100 + Me.Ordinal
                ' addition
                .LookupCondition = Me.LookupCondition
                .LookupPropertyStrings = Me.LookupPropertyStrings
                .PossibleValues = Me.PossibleValues
                .LowerRangeValue = Me.LowerRangeValue
                .UpperRangeValue = Me.UpperRangeValue
                .ValidateRegExpression = Me.ValidateRegExpression
                .ValidationPropertyStrings = Me.ValidationPropertyStrings
                .XID = Me.XID
                If .XID Is Nothing Then .XID = Me.SetID & "." & Me.ID
                .IsValidating = Me.IsValidating
                .RenderPropertyStrings = Me._renderPropertyStrings
                .RenderRegExpMatch = Me.RenderRegExpMatch
                .RenderRegExpPattern = Me.RenderRegExpMatch
                .IsRendering = Me.IsRendering

                ''' special compound settings
                .CompoundObjectID = ObjectPropertyValue.ConstObjectID
                .CompoundValueEntryName = ObjectPropertyValue.ConstFNValue
                .CompoundIDEntryname = ObjectPropertyValue.ConstFNPropertyID
                .CompoundSetterMethodName = Nothing
                .CompoundGetterMethodName = Nothing
                .CompoundRelationPath = {}

            End With
        End Sub

        ''' <summary>
        ''' set the default values for the create event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectProperty_OnCreateDefaultValuesNeeded(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreateDefaultValuesNeeded
            If Not e.Record.HasIndex(ConstFNType) Then e.Record.SetValue(ConstFNType, otObjectEntryType.CompoundEntry.ToString)
            If Not e.Record.HasIndex(ConstFNSetUPDC) Then
                If _set Is Nothing Then
                    Dim setid As String = e.Record.GetValue(ConstFNSetID)
                    If setid Is Nothing Then
                        CoreMessageHandler(message:="object propert set doesnot exist", procedure:="ObjectProperty.OnCreateDefaultValuesNeeded", _
                                           messagetype:=otCoreMessageType.ApplicationError, _
                                           argument:=setid)
                        e.AbortOperation = True
                        Return
                    End If
                    ''' even if it is early to retrieve the set and set it (since this might disposed since we have not run through checkuniqueness and cache)
                    ''' we need to check on the object here
                    Dim setupdc As Long? = e.Record.GetValue(ConstFNSetUPDC)
                    If Not setupdc.HasValue Then
                        Dim aCurrentSetid As ObjectPropertyCurrentSet = ObjectPropertyCurrentSet.Retrieve(id:=setid)
                        If aCurrentSetid IsNot Nothing Then
                            setupdc = aCurrentSetid.AliveUpdc
                        Else
                            CoreMessageHandler(message:="object propert current set does not exist", procedure:="ObjectProperty.OnCreated", _
                                          messagetype:=otCoreMessageType.ApplicationError, _
                                          argument:=setid)
                            e.AbortOperation = True
                        End If
                    End If
                    _set = ObjectPropertySet.Retrieve(id:=setid, updc:=setupdc, domainid:=Me.DomainID)
                    If _set Is Nothing Then
                        CoreMessageHandler(message:="object propert set doesnot exist", procedure:="ObjectProperty.OnCreateDefaultValuesNeeded", _
                                           messagetype:=otCoreMessageType.ApplicationError, _
                                           argument:=setid)
                        e.AbortOperation = True
                        Return
                    End If
                End If
            End If
            ''' finally set the updc of the set in the record
            e.Record.SetValue(ConstFNSetUPDC, _set.Updc)
        End Sub

        ''' <summary>
        ''' Delete the compound structure of this Property
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteCompoundStructure() As Boolean

            ''' attach the Properties as compounds
            ''' 
            For Each anObjectID In Me.PropertySet.AttachedObjectIDs
                Dim anObjectDefinition As iormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=anObjectID)
                If anObjectDefinition IsNot Nothing Then
                    Dim apath As String()
                    ReDim apath(_relationpath.GetUpperBound(0) + 1)
                    apath(0) = anObjectID
                    Array.ConstrainedCopy(_relationpath, 0, apath, 1, apath.Length - 1)
                    ''' delete all the relational path compounds
                    ''' 
                    For i = apath.GetLowerBound(0) To apath.GetUpperBound(0) - 1
                        Dim aCompound As ormObjectCompoundEntry = ormObjectCompoundEntry.Retrieve(apath(i), Me.ID, domainID:=Me.DomainID, runtimeOnly:=Me.RunTimeOnly)
                        If aCompound IsNot Nothing Then aCompound.Delete()
                    Next

                End If
            Next

            Return True
        End Function
        ''' <summary>
        ''' Create the Compound on each layer of the property
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateCompoundStructure() As Boolean
            ''' attach the Properties as compounds
            ''' 
            Dim aSet = Me.PropertySet
            If aSet Is Nothing Then
                CoreMessageHandler(message:="object propert set doesnot exist", procedure:="ObjectProperty.OnPersisted", _
                                   messagetype:=otCoreMessageType.ApplicationError, _
                                   argument:=Me.SetID)
                Return False
            End If

            '''
            ''' build the structure
            ''' 
            For Each anObjectID In aSet.AttachedObjectIDs
                Dim anObjectDefinition As ormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=anObjectID)
                If anObjectDefinition IsNot Nothing Then
                    Dim apath As String()
                    ReDim apath(_relationpath.GetUpperBound(0) + 1)
                    ''' set it to the linking objects
                    ''' 
                    If anObjectDefinition.ID = Deliverable.ConstObjectID Then
                        apath(0) = anObjectID & "." & Deliverable.ConstRPropertyLink
                    ElseIf Not String.IsNullOrEmpty(anObjectDefinition.ID) Then
                        CoreMessageHandler(message:="other objects for properties to be linked to not implemented", procedure:="ObjectPropertySet.OnPersisted", _
                                            argument:=anObjectDefinition.ID, objectname:=Me.ObjectID)
                    End If

                    Array.ConstrainedCopy(_relationpath, 0, apath, 1, apath.Length - 1)

                    ''' create all the relational path
                    ''' 
                    For i = apath.GetLowerBound(0) To apath.GetUpperBound(0) - 1
                        Dim names As String() = Shuffle.NameSplitter(apath(i)) ' get the objectname from the canonical form
                        Dim aCompound As ormObjectCompoundEntry = ormObjectCompoundEntry.Create(objectname:=names(0), _
                                                                                     entryname:=Me.ID, domainid:=Me.DomainID, _
                                                                                     runtimeOnly:=Me.RunTimeOnly, checkunique:=True)
                        If aCompound Is Nothing Then aCompound = ormObjectCompoundEntry.Retrieve(objectname:=names(0), _
                                                                                     entryname:=Me.ID, domainID:=Me.DomainID, runtimeOnly:=Me.RunTimeOnly)

                        ''' set the values
                        ''' 
                        SetCompound(compound:=aCompound)
                        Dim relpath As String()
                        ReDim relpath(apath.GetUpperBound(0) - i)
                        Array.ConstrainedCopy(apath, i, relpath, 0, relpath.Length)
                        aCompound.CompoundRelationPath = relpath

                        ''' on ObjectPropertyvLink Level we need to go to the setter to enable
                        ''' versioning on the lot if a changed property is needed
                        If names(0) = ObjectPropertyLink.ConstObjectID.ToUpper Then
                            aCompound.CompoundSetterMethodName = ObjectPropertyLink.ConstOPSetCompoundValue
                            ''' 
                            ''' on the end take the setter / getter operations to resolve
                            ''' 
                        ElseIf names(0) = ObjectPropertyValueLot.ConstObjectID.ToUpper Then
                            aCompound.CompoundSetterMethodName = ObjectPropertyValueLot.ConstOPSetCompoundValue
                            aCompound.CompoundGetterMethodName = ObjectPropertyValueLot.ConstOPGetCompoundValue
                        End If
                        ''' set it to the linking objects
                        ''' 

                        aCompound.Persist()
                    Next
                End If
            Next

            Return True
        End Function

        ''' <summary>
        ''' OnPersisted Handler to add the Properties as Compounds to the ObjectIDs
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectProperty_OnPersisted(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnPersisted
            Dim aSet = Me.PropertySet
            If aSet Is Nothing Then
                CoreMessageHandler(message:="object propert set doesnot exist", procedure:="ObjectProperty.OnPersisted", _
                                   messagetype:=otCoreMessageType.ApplicationError, _
                                   argument:=Me.SetID)
                e.AbortOperation = True
                Return
            End If
        End Sub
        ''' <summary>
        ''' OnDeleted Handler to add the Properties as Compounds to the ObjectIDs
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectProperty_OnDeleted(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnDeleted
            DeleteCompoundStructure()
        End Sub
        ''' <summary>
        ''' create a persistable ObjectProperty
        ''' </summary>
        ''' <param name="set"></param>
        ''' <param name="Entity"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(setid As String, setupdc As Long, ID As String, Optional domainid As String = Nothing) As ObjectProperty
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey As Object() = {setid.ToUpper, setupdc, ID.ToUpper, domainid}
            Return ormBusinessObject.CreateDataObject(Of ObjectProperty)(pkArray:=primarykey, domainID:=domainid, checkUnique:=True)
        End Function

        ''' <summary>
        ''' create a persistable ObjectProperty
        ''' </summary>
        ''' <param name="set"></param>
        ''' <param name="Entity"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(setid As String, setupdc As Long, ID As String, Optional domainid As String = Nothing) As ObjectProperty
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey As Object() = {setid.ToUpper, setupdc, ID.ToUpper, domainid}
            Return ormBusinessObject.RetrieveDataObject(Of ObjectProperty)(pkArray:=primarykey)
        End Function

        ''' <summary>
        ''' set default value event for object entry
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectProperty_OnDefaultValueNeeded(sender As Object, e As ormDataObjectEntryEventArgs) Handles Me.OnDefaultValueNeeded
            If e.ObjectEntryName = ConstFNType Then
                e.Value = otObjectEntryType.CompoundEntry
                e.Result = True
            End If
        End Sub


    End Class

    '    SELECT      TBLOBJPROPERTYLINKS.FROMOBJECTID, TBLOBJPROPERTYLINKS.fromuid, tblobjpropertylinks.FROMUPDC ,
    '		    TBLOBJPROPERTYLINKS.TOUID, TBLOBJPROPERTYLINKS.toupdc, LOT.PUID, LOT.UPDC, P1.VALUE as '0.0.2.0',P2.value AS '0.0.3.0' , P3.VALUE AS '0.1.0.0', P4.VALUE AS '0.1.3.0', 
    '              P5.VALUE AS '0.1.6.0', P6.VALUE AS '0.2.0.0',  P7.VALUE AS '0.2.3.0', P8.VALUE AS '0.2.6.0', P9.VALUE AS '0.3.0.0',
    '			   P10.VALUE AS '0.4.0.0',  P11.VALUE AS '0.5.0.0',  P12.VALUE AS '0.6.0.0',  P13.VALUE AS '1.0.0.0'
    'FROM            ontrack.dbo.TBLOBJPROPERTYVALUELOTS AS LOT 
    ' INNER JOIN               ontrack.dbo.TBLOBJPROPERTYVALUES AS P1 ON LOT.PUID = P1.PUID AND LOT.UPDC = P1.UPDC AND P1.PROPERTYID = '0.0.2.0'
    ' INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P2 ON LOT.PUID = P2.PUID AND LOT.UPDC = P2.UPDC AND P2.PROPERTYID = '0.0.3.0'
    ' INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P3 ON LOT.PUID = P3.PUID AND LOT.UPDC = P3.UPDC AND P3.PROPERTYID = '0.1.0.0'
    'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P4 ON LOT.PUID = P4.PUID AND LOT.UPDC = P4.UPDC AND P4.PROPERTYID = '0.1.3.0'
    'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P5 ON LOT.PUID = P5.PUID AND LOT.UPDC = P5.UPDC AND P5.PROPERTYID = '0.1.6.0'
    'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P6 ON LOT.PUID = P6.PUID AND LOT.UPDC = P6.UPDC AND P6.PROPERTYID = '0.2.0.0'
    'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P7 ON LOT.PUID = P7.PUID AND LOT.UPDC = P7.UPDC AND P7.PROPERTYID = '0.2.3.0'
    'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P8 ON LOT.PUID = P8.PUID AND LOT.UPDC = P8.UPDC AND P8.PROPERTYID = '0.2.6.0'
    'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P9 ON LOT.PUID = P9.PUID AND LOT.UPDC = P9.UPDC AND P9.PROPERTYID = '0.3.0.0'
    'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P10 ON LOT.PUID = P10.PUID AND LOT.UPDC = P10.UPDC AND P10.PROPERTYID = '0.4.0.0'
    'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P11 ON LOT.PUID = P11.PUID AND LOT.UPDC = P11.UPDC AND P11.PROPERTYID = '0.5.0.0'
    'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P12 ON LOT.PUID = P12.PUID AND LOT.UPDC = P12.UPDC AND P12.PROPERTYID = '0.6.0.0'
    'INNER JOIN				ontrack.dbo.TBLOBJPROPERTYVALUES AS P13 ON LOT.PUID = P13.PUID AND LOT.UPDC = P13.UPDC AND P13.PROPERTYID = '1.0.0.0'
    'inner join	ontrack.dbo.TBLOBJPROPERTYLINKS on lot.puid = TBLOBJPROPERTYLINKS.touid 

    ''' <summary>
    ''' the Property LINK class links a busines object to a value collection
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=ObjectPropertyLink.ConstObjectID, modulename:=ConstModuleProperties, Version:=1, _
        usecache:=True, adddomainbehavior:=True, adddeletefieldbehavior:=True, _
        description:="link definitions between properties via value collection and other business objects")> _
    Public Class ObjectPropertyLink
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable

        Public Const ConstObjectID = "PropertyLink"

        '** Schema Table
        <ormTableAttribute(version:=1)> Public Const ConstPrimaryTableID = "TBLOBJPROPERTYLINKS"

        '** index
        <ormIndex(columnname1:=ConstFNToUid, columnname2:=ConstFNFromObjectID, columnname3:=ConstFNFromUid)> Public Const ConstIndTag = "used"

        ''' <summary>
        ''' Primary key of the property link object
        ''' FROM an ObjectID, UID, UPDC (KEY)
        ''' TO   an OBJECTID, UID, UPDC
        ''' 
        ''' links a  business objects (deliverable, pars, configcondition (for own use) ) with a property set
        ''' also capable of linking schedules to schedules or milestones of schedules to schedules
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        ''' 

        <ormObjectEntry(referenceobjectentry:=ormObjectDefinition.ConstObjectID & "." & ormObjectDefinition.ConstFNID, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, _
            validationPropertyStrings:={ObjectValidationProperty.NotEmpty, ObjectValidationProperty.UseLookup}, _
            LookupPropertyStrings:={LookupProperty.UseAttributeValues}, _
            values:={Deliverable.ConstObjectID, Parts.Part.ConstObjectID, Configurables.ConfigItemSelector.ConstObjectID}, _
            dbdefaultvalue:=Deliverable.ConstObjectID, defaultvalue:=Deliverable.ConstObjectID, _
            XID:="OPL1", title:="From Object", description:="from object id of the business object")> _
        Public Const ConstFNFromObjectID = "FROMOBJECTID"

        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=2, dbdefaultvalue:="0", lowerrange:=0, _
            XID:="OPL2", title:="Linked from UID", description:="from uid of the business object")> _
        Public Const ConstFNFromUid = "FROMUID"

        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=3, dbdefaultvalue:="0", lowerrange:=0, _
            XID:="OPL3", title:="Linked from UPDC", description:="from uid of the business object")> _
        Public Const ConstFNFromUpdc = "FROMUPDC"

        ' deactivate ForeignKEy we do not have this object in domains
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=4, _
            useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const ConstFNDomainID = Domain.ConstFNDomainID


        ''' <summary>
        ''' Column Definitions
        ''' </summary>
        ''' <remarks></remarks>

        <ormObjectEntry(Datatype:=otDataType.Long, dbdefaultvalue:="0", lowerrange:=0, _
            XID:="OPL5", title:="Linked to UID", description:="uid link to the property value lot object")> _
        Public Const ConstFNToUid = "TOUID"

        <ormObjectEntry(Datatype:=otDataType.Long, isnullable:=True, lowerrange:=0, _
            XID:="OPL6", title:="Linked to UPDC", description:="updc link to the property value lot object")> _
        Public Const ConstFNToUpdc = "TOUPDC"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, dbdefaultvalue:="One2One", defaultvalue:=otLinkType.One2One, _
            XID:="OPL10", title:="Linke Type", description:="object link type")> Public Const ConstFNTypeID = "typeid"

        ''' <summary>
        ''' Mappings persistable members
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNFromObjectID)> Private _FromObjectID As String
        <ormObjectEntryMapping(EntryName:=ConstFNFromUid)> Private _FromUid As Long
        <ormObjectEntryMapping(EntryName:=ConstFNFromUpdc)> Private _FromUpdc As Long

        <ormObjectEntryMapping(EntryName:=ConstFNToUid)> Private _ToUid As Long
        <ormObjectEntryMapping(EntryName:=ConstFNToUpdc)> Private _ToUpdc As Long
        <ormObjectEntryMapping(EntryName:=ConstFNTypeID)> Private _type As otLinkType

        ''' <summary>
        ''' Relation to PropertyValueLot - will be resolved by event handler on relation manager
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkObject:=GetType(ObjectPropertyValueLot), createobjectifnotretrieved:=True, toPrimarykeys:={ConstFNToUid, ConstFNToUpdc}, _
                     cascadeonCreate:=True, cascadeOnDelete:=True, cascadeOnUpdate:=True)> _
        Public Const ConstRPropertyValueLot = "RELPROPERTYVALUELOT"

        <ormObjectEntryMapping(relationName:=ConstRPropertyValueLot, infusemode:=otInfuseMode.OnCreate Or otInfuseMode.OnInject Or otInfuseMode.OnDemand)> _
        Private _propertyValueLot As ObjectPropertyValueLot

        ''' <summary>
        ''' Define the constants for accessing the compounds
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstOPGetCompoundValue = "GETPROPERTYVALUE"
        Public Const ConstOPSetCompoundValue = "SETPROPERTYVALUE"

        ''' <summary>
        ''' dynamic data
        ''' </summary>
        ''' <remarks></remarks>
        Private _prevVersionLots As New List(Of ObjectPropertyValueLot) 'list of previous versions we we issue a version change

#Region "properties"

        ''' <summary>
        ''' Gets or sets the property value lot.
        ''' </summary>
        ''' <value>The property value lot.</value>
        Public ReadOnly Property PropertyValueLot() As ObjectPropertyValueLot
            Get
                If Not IsAlive(subname:="PropertyValueLot") Then Return Nothing

                If Me.GetRelationStatus(ConstRPropertyValueLot) <> ormRelationManager.RelationStatus.Loaded Then
                    Me.InfuseRelation(ConstRPropertyValueLot)
                End If
                Return Me._propertyValueLot
            End Get

        End Property

        ''' <summary>
        ''' Gets or sets the type.
        ''' </summary>
        ''' <value>The type.</value>
        Public Property Type() As otLinkType
            Get
                Return Me._type
            End Get
            Set(value As otLinkType)
                Me._type = value
            End Set
        End Property

        ''' <summary>
        ''' gets the object id of the linking object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property FromObjectID() As String
            Get
                Return _FromObjectID
            End Get

        End Property
        ''' <summary>
        ''' gets the UID of the linking object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property FromUID() As Long
            Get
                Return _FromUid
            End Get

        End Property
        ''' <summary>
        ''' gets the Updc of the linking object - returns zero if not applicable
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property FromUPDC() As Long
            Get
                Return _FromUpdc
            End Get

        End Property

        ''' <summary>
        ''' gets or sets the UID of the linked object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property ToUID() As Long
            Get
                Return _ToUid
            End Get
            Set(value As Long)
                SetValue(ConstFNToUid, value)
            End Set
        End Property

        ''' <summary>
        ''' gets or sets the Updc of the linked object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property ToUpdc() As Long?
            Get
                Return _ToUpdc
            End Get
            Set(value As Long?)
                SetValue(ConstFNToUpdc, value)
            End Set
        End Property
#End Region

        ''' <summary>
        ''' return a list of all ValueLots having the setid
        ''' </summary>
        ''' <param name="setid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function AllByValueLot(uid As Long, updc As Long) As IList(Of ObjectPropertyLink)
            Dim aStore As iormRelationalTableStore = ot.GetPrimaryTableStore(ConstPrimaryTableID)
            Dim aCollection As New List(Of ObjectPropertyLink)
            Try
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(id:="AllByValueLot", addAllFields:=True)
                If Not aCommand.IsPrepared Then
                    aCommand.Where = "[" & ConstFNIsDeleted & "] = @deleted AND [" & ConstFNToUid & "] = @touid AND [" & ConstFNToUpdc & "] = @toupdc "

                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@deleted", ColumnName:=ConstFNIsDeleted, tableid:=ConstPrimaryTableID))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@touid", columnname:=ConstFNToUid, tableid:=ConstPrimaryTableID))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@toupdc", columnname:=ConstFNToUpdc, tableid:=ConstPrimaryTableID))
                    aCommand.Prepare()
                End If
                aCommand.SetParameterValue(ID:="@deleted", value:=False)
                aCommand.SetParameterValue(ID:="@touid", value:=uid)
                aCommand.SetParameterValue(ID:="@toupdc", value:=updc)

                Dim aRecordCollection = aCommand.RunSelect

                For Each aRecord As ormRecord In aRecordCollection
                    Dim anewObject As New ObjectPropertyLink
                    If InfuseDataObject(record:=aRecord, dataobject:=anewObject) Then
                        aCollection.Add(item:=anewObject)
                    End If
                Next

                Return aCollection


            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="ObjectPropertyLink.allbyLot")
                Return aCollection

            End Try
        End Function
        ''' <summary>
        ''' return a list of all ValueLots having the setid
        ''' </summary>
        ''' <param name="setid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function AllBySet(setid As String) As IList(Of ObjectPropertyLink)
            Dim aStore As iormRelationalTableStore = ot.GetPrimaryTableStore(ConstPrimaryTableID)
            Dim aCollection As New List(Of ObjectPropertyLink)
            Try

                For Each aLink In ObjectPropertyLink.AllDataObject(Of ObjectPropertyLink)()
                    If aLink.PropertyValueLot IsNot Nothing AndAlso aLink.PropertyValueLot.PropertySetIDs.Contains(setid.ToUpper) Then
                        aCollection.Add(aLink)
                    End If
                Next

                Return aCollection

            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="ObjectPropertyLink.AllBySet")
                Return aCollection

            End Try
        End Function
        ''' <summary>
        ''' Update the link to the setid and setupdc
        ''' </summary>
        ''' <param name="setid"></param>
        ''' <param name="setupdc"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateValueLot2Set(setid As String, setupdc As Long, _
                                           Optional timestamp As DateTime? = Nothing, _
                                           Optional domainid As String = Nothing) As Boolean
            If Not IsAlive(subname:="UpdateSetUpdc") Then Return False
            '**
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID

            '** clone
            Dim aNewLot As ObjectPropertyValueLot = Me.PropertyValueLot.Clone(uid:=Me.ToUID)
            If aNewLot.UpdateSet(setid:=setid, setupdc:=setupdc, domainid:=domainid) Then

                Me.ToUID = aNewLot.UID
                Me.ToUpdc = aNewLot.UPDC ' set new one
                Me.PropertyValueLot.ValidUntil = Date.Now
                _prevVersionLots.Add(_propertyValueLot)
                aNewLot.Validfrom = Date.Now
                aNewLot.ValidUntil = Nothing
                _propertyValueLot = aNewLot
                _propertyValueLot.DomainID = domainid
                aNewLot.Persist(timestamp)
                Me.Persist(timestamp)

                Return True
            End If
            Return False
        End Function
        ''' <summary>
        ''' operation to set a PropertyValue - here we must change to next version (updc) of the 
        ''' </summary>
        ''' <param name="id">the property</param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ormObjectOperationMethod(operationname:=ConstOPSetCompoundValue, tag:=ormObjectCompoundEntry.ConstCompoundSetter, _
            parameterEntries:={ormObjectCompoundEntry.ConstFNEntryName, ormObjectCompoundEntry.ConstFNValues, Domain.ConstFNDomainID})> _
        Public Function SetPropertyValue(id As String, value As Object, Optional domainid As String = Nothing) As Boolean
            If Not IsAlive(subname:="SetPropertyValue") Then Return False

            ''' get the relation
            ''' 
            If Me.PropertyValueLot Is Nothing Then
                Return False
            End If

            '**
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            '''
            ''' check if the new Property value is different then old one
            ''' 
            If Not Me.PropertyValueLot.EqualsValue(id, value) Then
                ''' we need change the version of the properyvaluelot if we have not done so (then it is created)
                ''' 
                If Not Me.PropertyValueLot.IsCreated Then
                    Dim aNewLot As ObjectPropertyValueLot = Me.PropertyValueLot.Clone(uid:=Me.ToUID)
                    Me.ToUID = aNewLot.UID
                    Me.ToUpdc = aNewLot.UPDC ' set new one
                    Me.PropertyValueLot.ValidUntil = Date.Now
                    _prevVersionLots.Add(_propertyValueLot)
                    aNewLot.Validfrom = Date.Now
                    aNewLot.ValidUntil = Nothing
                    _propertyValueLot = aNewLot
                    _propertyValueLot.DomainID = domainid
                End If

                Return _propertyValueLot.SetValue(entryname:=id, value:=value)
            Else
                ''' nothing to do
                ''' 
                Return True
            End If

        End Function
        ''' <summary>
        ''' handles the onPersisted Event to save the previous versions
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectPropertyLink_OnPersisted(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnPersisted
            For Each aLot In _prevVersionLots
                aLot.Persist()
            Next
        End Sub
        ''' <summary>
        ''' Event Handler for the RelationRetrieveNeeded event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectPropertyLink_OnRelationRetrieveNeeded(sender As Object, e As ormDataObjectRelationEventArgs) Handles Me.OnRelationRetrieveNeeded
            If Not Me.IsAlive(subname:="ObjectPropertyLink_OnRelationRetrieveNeeded") Then Return
            ''' check on PropertyValueLot
            ''' 
            If e.RelationID = ConstRPropertyValueLot Then
                Dim aPropertyLot As ObjectPropertyValueLot = ObjectPropertyValueLot.Retrieve(uid:=Me.ToUID, updc:=Me.ToUpdc)
                e.RelationObjects.Add(aPropertyLot)
                e.Finished = True
            End If
        End Sub
        ''' <summary>
        ''' Event Handler for the RelationRetrieveNeeded event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectPropertyLink_OnRelationCreateNeeded(sender As Object, e As ormDataObjectRelationEventArgs) Handles Me.OnRelationCreateNeeded
            If Not Me.IsAlive(subname:="Deliverable_OnRelationCreateNeeded") Then Return
            ''' check on PropertyValueLot
            ''' 
            If e.RelationID = ConstRPropertyValueLot Then
                Dim aPropertyLot As ObjectPropertyValueLot = ObjectPropertyValueLot.Create(uid:=Me.ToUID, updc:=Me.ToUpdc)
                If aPropertyLot Is Nothing Then aPropertyLot = ObjectPropertyValueLot.Retrieve(uid:=Me.ToUID, updc:=Me.ToUpdc)

                ' we have what we need
                e.RelationObjects.Add(aPropertyLot)
                e.Finished = True

            End If
        End Sub
        ''' <summary>
        ''' create a persitable link object
        ''' </summary>
        ''' <param name="fromid"></param>
        ''' <param name="fromuid"></param>
        ''' <param name="toid"></param>
        ''' <param name="touid"></param>
        ''' <param name="frommilestone"></param>
        ''' <param name="tomilestone"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(fromObjectID As String, _
                                                fromuid As Long, _
                                                Optional fromupdc As Long = 0, _
                                                Optional domainid As String = Nothing, _
                                                Optional toUID As Long? = Nothing, _
                                                Optional toUpdc As Long? = Nothing) As ObjectPropertyLink
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey As Object() = {fromObjectID, fromuid, fromupdc, domainid}

            '' set values
            Dim aRecord As New ormRecord
            With aRecord
                .SetValue(ConstFNFromObjectID, fromObjectID)
                .SetValue(ConstFNFromUid, fromuid)
                .SetValue(ConstFNFromUpdc, fromupdc)
                .SetValue(ConstFNDomainID, domainid)
                '.SetValue(ConstFNToObjectID, ObjectPropertyValueLot.ConstObjectID)
                .SetValue(ConstFNToUid, toUID)
                .SetValue(ConstFNToUpdc, toUpdc)
            End With

            Return ormBusinessObject.CreateDataObject(Of ObjectPropertyLink)(aRecord, checkUnique:=True)
        End Function

        ''' <summary>
        ''' retrieve a persitable link object
        ''' </summary>
        ''' <param name="fromid"></param>
        ''' <param name="fromuid"></param>
        ''' <param name="toid"></param>
        ''' <param name="touid"></param>
        ''' <param name="frommilestone"></param>
        ''' <param name="tomilestone"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(fromObjectID As String, fromUid As Long, fromUpdc As Long, Optional domainid As String = Nothing) As ObjectPropertyLink
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey As Object() = {fromObjectID, fromUid, fromUpdc, domainid}
            Return ormBusinessObject.RetrieveDataObject(Of ObjectPropertyLink)(primarykey)
        End Function
    End Class

    ''' <summary>
    ''' class for a lot or set of object properties values  attached to other business objects
    ''' </summary>
    ''' <remarks>
    ''' Design Principles
    ''' 
    ''' 1. The Lot takes care of the values by the SetPropertyValue, GetPropertyValue Routine
    ''' 
    ''' 2. The Lot loads or creates with the AddSet Function all the Properties in its collection.
    ''' 
    ''' 3. setPropertyValue also issues an AddSet with new Sets to be assigned values to
    ''' </remarks>
    <ormObject(id:=ObjectPropertyValueLot.ConstObjectID, version:=1, adddomainbehavior:=False, adddeletefieldbehavior:=True, usecache:=True, _
        modulename:=ConstModuleProperties, Title:="Property Value Lot", description:="Lot of properties values attached to bussiness object")> _
    Public Class ObjectPropertyValueLot
        Inherits ormBusinessObject


        Public Const ConstObjectID = "PropertyValueLot"

        ''' <summary>
        ''' Table Definition
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=1, usecache:=False)> Public Const ConstPrimaryTableID = "TBLOBJPROPERTYVALUELOTS"

        ''' <summary>
        ''' primary keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=1, dbdefaultvalue:="0", _
              XID:="PLOT1", title:="Lot UID", description:="UID of the property value lot")> Public Const constFNUID = "PUID"

        <ormObjectEntry(Datatype:=otDataType.Long, dbdefaultvalue:="0", PrimaryKeyOrdinal:=2, _
            title:="update count", Description:="Update count of the property value lot", XID:="PLOT2")> Public Const ConstFNVersion = "VERSION"

        ''' <summary>
        ''' fields
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, defaultvalue:=ConstGlobalDomain, _
          useforeignkey:=otForeignKeyImplementation.None, dbdefaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
          XID:="PLOT3", title:="Description", description:="description of the property value lot")> Public Const ConstFNDescription = "DESC"

        <ormObjectEntry(Datatype:=otDataType.List, _
         lookupPropertyStrings:={LookupProperty.UseObjectEntry & "(" & ObjectPropertySet.ConstObjectID & "." & ObjectPropertySet.ConstFNSetID & ")"}, validationPropertyStrings:={ObjectValidationProperty.UseLookup}, _
         XID:="PLOT4", title:="Property Sets", description:="applicable property sets for this lot")> Public Const ConstFNSets = "SETS"

        <ormObjectEntry(Datatype:=otDataType.Timestamp, isnullable:=True, _
        XID:="PLOT11", title:="valid from", description:="property set is valid from ")> Public Const ConstFNValidFrom = "validfrom"

        <ormObjectEntry(Datatype:=otDataType.Timestamp, isnullable:=True, _
       XID:="PLOT12", title:="valid until", description:="property set is valid until ")> Public Const ConstFNValiduntil = "validuntil"

        <ormObjectEntry(Datatype:=otDataType.List, isnullable:=True, _
        XID:="PLOT15", title:="PropertySet UpdateCounter", description:="property set updatecounter of last values ")> Public Const ConstFNSetUPDCs = "SETUPDCS"


        ''' <summary>
        ''' Mappings
        ''' </summary>
        ''' <remarks></remarks>

        <ormObjectEntryMapping(EntryName:=constFNUID)> Private _uid As Long = 0
        <ormObjectEntryMapping(EntryName:=ConstFNVersion)> Private _updc As Long = 0
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _description As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNSets)> Private _setids As String() = {}
        <ormObjectEntryMapping(EntryName:=ConstFNValidFrom)> Private _validfrom As DateTime?
        <ormObjectEntryMapping(EntryName:=ConstFNValiduntil)> Private _validuntil As DateTime?
        <ormObjectEntryMapping(EntryName:=ConstFNSetUPDCs)> Private _setsupdc As String() = {}

        ''' <summary>
        ''' Relations of values
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(ObjectPropertyValue), cascadeOnDelete:=True, cascadeOnUpdate:=True, _
            fromEntries:={constFNUID, ConstFNVersion}, toEntries:={ObjectPropertyValue.constFNUID, ObjectPropertyValue.ConstFNVersion})> Public Const ConstRValues = "RELVALUES"

        <ormObjectEntryMapping(RelationName:=ConstRValues, infuseMode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand, _
            keyentries:={ObjectPropertyValue.ConstFNSetID, ObjectPropertyValue.ConstFNPropertyID})> _
        Private WithEvents _valuesCollection As New ormRelationCollection(Of ObjectPropertyValue)(Me, {ObjectPropertyValue.ConstFNSetID, ObjectPropertyValue.ConstFNPropertyID})

        ''' <summary>
        ''' Define the constants for accessing the compounds
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstOPGetCompoundValue = "GETPROPERTYVALUE"
        Public Const ConstOPSetCompoundValue = "SETPROPERTYVALUE"

        ''' <summary>
        ''' dynamic members
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        Private _changedPropertyValues As New Dictionary(Of String, ObjectPropertyValue)
        Private _propertysets As New List(Of ObjectPropertySet)

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the UPDC of the Sets as Array of Long.
        ''' </summary>
        ''' <value>The setsupdc.</value>
        Public Property Setsupdc() As String()
            Get
                Return Me._setsupdc
            End Get
            Set(value As String())
                SetValue(ConstFNSetUPDCs, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the validto date.
        ''' </summary>
        ''' <value>The validto.</value>
        Public Property ValidUntil() As DateTime?
            Get
                Return Me._validuntil
            End Get
            Set(value As DateTime?)
                SetValue(ConstFNValiduntil, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the validfrom.
        ''' </summary>
        ''' <value>The validfrom.</value>
        Public Property Validfrom() As DateTime?
            Get
                Return Me._validfrom
            End Get
            Set(value As DateTime?)
                SetValue(ConstFNValidFrom, value)
            End Set
        End Property

        '' <summary>
        ''' Gets or sets the set id s.
        ''' </summary>
        ''' <value>The properties.</value>
        Public Property PropertySetIDs() As String()
            Get
                Return Me._setids
            End Get
            Set(value As String())
                SetValue(ConstFNSets, value)
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
                SetValue(ConstFNDescription, value)
            End Set
        End Property

        ''' <summary>
        ''' returns the UID of the configuration set
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property UID() As Long
            Get
                Return _uid
            End Get
        End Property

        ''' <summary>
        ''' returns the UID of the configuration set
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property UPDC() As Long
            Get
                Return _updc
            End Get
        End Property
        ''' <summary>
        ''' returns the Entities of this set
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Values As ormRelationCollection(Of ObjectPropertyValue)
            Get
                Return _valuesCollection
            End Get
        End Property

        ''' <summary>
        ''' gets the property sets of this value lot
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property PropertySets As IList(Of ObjectPropertySet)
            Get
                Return _propertysets
            End Get
        End Property
#End Region

        ''' <summary>
        ''' return a list of all ValueLots having the setid
        ''' </summary>
        ''' <param name="setid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function AllBySet(setid As String) As IList(Of ObjectPropertyValueLot)
            Dim aStore As iormRelationalTableStore = ot.GetPrimaryTableStore(ObjectPropertyValueLot.ConstPrimaryTableID)
            Dim aCollection As New List(Of ObjectPropertyValueLot)
            Try
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(id:="AllBySet", addAllFields:=True)
                If Not aCommand.IsPrepared Then
                    aCommand.Where = "[" & ConstFNIsDeleted & "] = @deleted AND [" & ConstFNSets & "] like @sets"

                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@deleted", ColumnName:=ConstFNIsDeleted, tableid:=ConstPrimaryTableID))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@sets", notColumn:=True, datatype:=otDataType.Text))
                    aCommand.Prepare()
                End If
                aCommand.SetParameterValue(ID:="@deleted", value:=False)
                aCommand.SetParameterValue(ID:="@sets", value:="%" & ConstDelimiter & setid & ConstDelimiter & "%")

                Dim aRecordCollection = aCommand.RunSelect

                For Each aRecord As ormRecord In aRecordCollection
                    Dim anewObject As New ObjectPropertyValueLot
                    If InfuseDataObject(record:=aRecord, dataobject:=anewObject) Then
                        aCollection.Add(item:=anewObject)
                    End If
                Next

                Return aCollection


            Catch ex As Exception

                Call CoreMessageHandler(exception:=ex, procedure:="ObjectPropertyValueLot.AllBySet")
                Return aCollection

            End Try
        End Function

        ''' <summary>
        ''' operation to Access the Compound's Value
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ormObjectOperationMethod(operationname:=ConstOPGetCompoundValue, tag:=ormObjectCompoundEntry.ConstCompoundGetter, _
            parameterEntries:={ormObjectCompoundEntry.ConstFNEntryName, ormObjectCompoundEntry.ConstFNValues, Domain.ConstFNDomainID})> _
        Public Function GetPropertyValue(id As String, ByRef value As Object, Optional domainid As String = Nothing) As Boolean
            If Not IsAlive(subname:="GetPropertyValue") Then Return Nothing
            Dim propertyID As String = id.ToUpper
            Dim aPropertySet As ObjectPropertySet
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID

            ''' the id should be in a canonical form
            ''' 
            Dim names = Shuffle.NameSplitter(id)

            ''' if we have a set then check 
            If names.Count = 1 Then
                If _setids.Count = 0 Then
                    ''' we could look up if this is unqiue
                    ''' 

                    CoreMessageHandler(message:="lot as no property set attached to it - value cannot be retrieved", messagetype:=otCoreMessageType.ApplicationError, _
                                   argument:=id, objectname:=Me.ObjectID, procedure:="ObjectPropertyValueLot.GetPropertyValue")
                    Return False


                ElseIf _setids.Count = 1 AndAlso Me.PropertySets.First.PropertyIDs.Contains(id.ToUpper) Then
                    ReDim names(1)
                    names(0) = _setids(0)
                    aPropertySet = Me.PropertySets.First
                    names(1) = id.ToUpper
                Else
                    CoreMessageHandler(message:="property to be added does not exist in this set", messagetype:=otCoreMessageType.ApplicationError, _
                                      argument:=id, objectname:=Me.ObjectID, procedure:="ObjectPropertyValueLot.GetPropertyValue")
                    ''' not found not in
                    Return False
                End If

                ''' extend the properties by this set
            ElseIf names.Count > 1 Then
                ''' do we have the id
                aPropertySet = Me.PropertySets.Where(Function(x) x.ID = names(0) And x.PropertyIDs.Contains(names(1))).FirstOrDefault

                If aPropertySet Is Nothing Then
                    ''' we have in the property sets a property with this as full name
                    aPropertySet = Me.PropertySets.Where(Function(x) x.PropertyIDs.Contains(id.ToUpper)).FirstOrDefault

                    If aPropertySet IsNot Nothing Then
                        names(0) = aPropertySet.ID
                        names(1) = id.ToUpper
                    Else
                        Dim aCurrentSet = ObjectPropertyCurrentSet.Retrieve(id:=names(0), domainid:=domainid)
                        If aCurrentSet IsNot Nothing Then
                            aPropertySet = ObjectPropertySet.Retrieve(id:=names(0), updc:=aCurrentSet.AliveUpdc, domainid:=domainid)
                            If Not aPropertySet.PropertyIDs.Contains(names(1)) Then
                                aPropertySet = Nothing ' reset not found -> now we could try to find it in different versions of the set or in different sets
                                CoreMessageHandler(message:="property does not exist in any set of the lot and the set was also not found in store", messagetype:=otCoreMessageType.ApplicationError, _
                                          argument:=id, objectname:=Me.ObjectID, procedure:="ObjectPropertyValueLot.GetPropertyValue")
                                Return False
                            End If
                        End If
                    End If
                End If
            End If

            ''' load the relation if needed
            ''' 
            If Me.GetRelationStatus(ConstRValues) = ormRelationManager.RelationStatus.Unloaded Then InfuseRelation(ConstRValues)

            '''
            ''' check if the set has the id - otherwise extend if we have no value
            ''' 
            If aPropertySet Is Nothing Then
                CoreMessageHandler(message:="property to be retrieved does not exist in this set '" & names(0) & "'", messagetype:=otCoreMessageType.ApplicationError, _
                                   argument:=id, objectname:=Me.ObjectID, procedure:="ObjectPropertyValueLot.GetPropertyValue")
                ''' not found not in
                Return False
            ElseIf Not _valuesCollection.ContainsKey(key:={aPropertySet.ID, names(1)}) Then
                ''' if we have no value but the property is in the set - we need to add it
                Me.AddPropertyValue(setid:=names(0), propertyid:=names(1))
            End If


            ''' return the value
            If _valuesCollection.ContainsKey(key:={aPropertySet.ID, names(1)}) Then
                value = _valuesCollection.Item(key:={aPropertySet.ID, names(1)}).GetValue(ObjectPropertyValue.ConstFNValue)
                Return True
            Else
                value = Nothing
                Return False
            End If

        End Function

        ''' <summary>
        ''' operation to set a PropertyValue
        ''' </summary>
        ''' <param name="id">the property</param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ormObjectOperationMethod(operationname:=ConstOPSetCompoundValue, tag:=ormObjectCompoundEntry.ConstCompoundSetter, _
            parameterEntries:={ormObjectCompoundEntry.ConstFNEntryName, ormObjectCompoundEntry.ConstFNValues, Domain.ConstFNDomainID})> _
        Public Function SetPropertyValue(id As String, value As Object, Optional domainid As String = Nothing) As Boolean
            If Not IsAlive(subname:="SetPropertyValue") Then Return Nothing
            Dim aPropertySet As ObjectPropertySet
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID

            ''' the id should be in a canonical form
            ''' 
            Dim names = Shuffle.NameSplitter(id)
            ''' if we have a set then check 
            If names.Count = 1 Then
                If _setids.Count = 0 Then
                    ''' we could look up if this is unqiue
                    ''' 

                    CoreMessageHandler(message:="lot as no property set attached to it - value cannot be retrieved", messagetype:=otCoreMessageType.ApplicationError, _
                                   argument:=id, objectname:=Me.ObjectID, procedure:="ObjectPropertyValueLot.GetPropertyValue")
                    Return False


                ElseIf _setids.Count = 1 AndAlso Me.PropertySets.First.PropertyIDs.Contains(id.ToUpper) Then
                    ReDim names(1)
                    names(0) = _setids(0)
                    aPropertySet = Me.PropertySets.FirstOrDefault
                    names(1) = id.ToUpper
                Else
                    CoreMessageHandler(message:="property to be added does not exist in this set", messagetype:=otCoreMessageType.ApplicationError, _
                                      argument:=id, objectname:=Me.ObjectID, procedure:="ObjectPropertyValueLot.GetPropertyValue")
                    ''' not found not in
                    Return False
                End If

                ''' extend the properties by this set
            ElseIf names.Count > 1 Then
                ''' do we have the id
                aPropertySet = Me.PropertySets.Where(Function(x) x.ID = names(0) And x.PropertyIDs.Contains(names(1))).FirstOrDefault

                If aPropertySet Is Nothing Then
                    ''' we have in the property sets a property with this as full name
                    aPropertySet = Me.PropertySets.Where(Function(x) x.PropertyIDs.Contains(id.ToUpper)).FirstOrDefault

                    If aPropertySet IsNot Nothing Then
                        names(0) = aPropertySet.ID
                        names(1) = id.ToUpper
                    Else
                        Dim aCurrentSet = ObjectPropertyCurrentSet.Retrieve(id:=names(0), domainid:=domainid)
                        If aCurrentSet IsNot Nothing Then
                            aPropertySet = ObjectPropertySet.Retrieve(id:=names(0), updc:=aCurrentSet.AliveUpdc, domainid:=domainid)
                            If Not aPropertySet.PropertyIDs.Contains(names(1)) Then
                                aPropertySet = Nothing ' reset not found -> now we could try to find it in different versions of the set or in different sets
                                CoreMessageHandler(message:="property does not exist in any set of the lot and the set was also not found in store", messagetype:=otCoreMessageType.ApplicationError, _
                                          argument:=id, objectname:=Me.ObjectID, procedure:="ObjectPropertyValueLot.GetPropertyValue")
                                Return False
                            Else
                                ''' add the set
                                If Not Me.AddSet(names(0)) Then Return False
                            End If
                        End If
                    End If
                End If
            End If


            If Me.GetRelationStatus(ConstRValues) = ormRelationManager.RelationStatus.Unloaded Then InfuseRelation(ConstRValues)

            '''
            ''' check if the set has the id - otherwise extend if we have no value
            ''' 
            If aPropertySet Is Nothing Then
                CoreMessageHandler(message:="property to be set does not exist in this set '" & names(0) & "'", messagetype:=otCoreMessageType.ApplicationError, _
                                   argument:=id, objectname:=Me.ObjectID, procedure:="ObjectPropertyValueLot.GetPropertyValue")
                ''' not found not in
                Return False
            ElseIf Not _valuesCollection.ContainsKey(key:={aPropertySet.ID, names(1)}) Then
                ''' if we have no value but the property is in the set - we need to add it
                Me.AddPropertyValue(setid:=names(0), propertyid:=names(1))
            End If


            ''' 
            ''' set the value
            If names.Count > 1 AndAlso _valuesCollection.ContainsKey(key:={aPropertySet.ID, names(1)}) Then
                ''' check if something is now different
                ''' 
                Dim aPropertyvalue As ObjectPropertyValue = _valuesCollection.Item(key:={aPropertySet.ID, names(1)})

                ''' on success
                If aPropertyvalue.SetValue(ObjectPropertyValue.ConstFNValue, value) Then

                End If

                Return True


            Else
                CoreMessageHandler(message:="property to be added doesnot exist in this set", messagetype:=otCoreMessageType.ApplicationError, _
                                      argument:=id, objectname:=Me.ObjectID, procedure:="ObjectPropertyValueLot.SetPropertyValue")
                ''' not found not in
                Return False
            End If

        End Function

        ''' <summary>
        ''' Update the lot with the set
        ''' </summary>
        ''' <param name="setid"></param>
        ''' <param name="setupdc"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateSet(setid As String, Optional setupdc As Long? = Nothing, Optional domainid As String = Nothing) As Boolean
            If Not IsAlive(subname:="UpdateSetUpdc") Then Return False
            Dim result As Boolean = True
            '**
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID

            setid = setid.ToUpper
            '' if not attached
            If Not Me.PropertySetIDs.Contains(setid) Then
                Return False
            End If

            If Not setupdc.HasValue Then
                Dim anCurrentSet As ObjectPropertyCurrentSet = ObjectPropertyCurrentSet.Retrieve(id:=setid, domainid:=domainid)
                setupdc = anCurrentSet.AliveUpdc
            End If

            Dim aPropertySet = ObjectPropertySet.Retrieve(id:=setid, updc:=setupdc, domainid:=domainid)
            If aPropertySet Is Nothing Then
                CoreMessageHandler(message:="property set to be updated to does not exist", messagetype:=otCoreMessageType.ApplicationError, _
                                    argument:=setid, objectname:=Me.ObjectID, procedure:="ObjectPropertyValueLot.UpdateSet")
                Return False
            End If

            '' exchange
            Dim aSet As ObjectPropertySet = Me.PropertySets.Where(Function(x) x.ID = aPropertySet.ID AndAlso x.Updc <> aPropertySet.Updc).FirstOrDefault
            If aSet IsNot Nothing Then
                Me.PropertySets.Remove(aSet)
                Me.PropertySets.Add(aPropertySet)
                '** exchange the updc
                _setsupdc(Array.IndexOf(_setids, aSet.ID)) = aPropertySet.Updc.ToString
                '' Check on deleted elements
                For Each aProperty In aSet.Properties
                    Dim stillProperty As ObjectProperty = aPropertySet.Properties.Where(Function(x) x.ID = aProperty.ID).FirstOrDefault
                    If stillProperty Is Nothing Then
                        Dim avalue As ObjectPropertyValue = Me.Values.Where(Function(x) x.SetID = aProperty.SetID AndAlso x.PropertyID = aProperty.ID).FirstOrDefault
                        If avalue IsNot Nothing Then
                            '' delete the property value
                            avalue.Delete()
                        End If
                    End If

                Next
            End If

            '''
            ''' Add All or increase the reference the values
            For Each aProperty In aPropertySet.Properties
                Dim avalue As ObjectPropertyValue = Me.Values.Where(Function(x) x.SetID = aProperty.SetID AndAlso x.PropertyID = aProperty.ID).FirstOrDefault

                ''' update the existing value
                If avalue IsNot Nothing AndAlso avalue.Setupdc <> aProperty.Setupdc Then
                    ' simply set the setupdc (other changes are not reflected)
                    avalue.Setupdc = setupdc
                    result = result And True
                ElseIf avalue Is Nothing Then
                    ''' Addd the propertyvalue
                    result = result And Me.AddPropertyValue(setid:=setid, setupdc:=setupdc, propertyid:=aProperty.ID)
                End If

            Next

            ''' set the vcalid from
            If Not Me.Validfrom.HasValue Then Me.Validfrom = Date.Now

            Return result
        End Function


        ''' <summary>
        ''' Add a PropertySet to this lot and creates / retrieves all the values with default values
        ''' 
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddSet(setid As String, _
                               Optional updc As Long? = Nothing, _
                               Optional domainid As String = Nothing) As Boolean
            If Not IsAlive(subname:="AddSet") Then Return Nothing
            setid = setid.ToUpper
            If Not updc.HasValue Then
                Dim anCurrentSet As ObjectPropertyCurrentSet = ObjectPropertyCurrentSet.Retrieve(id:=setid, domainid:=domainid)
                updc = anCurrentSet.AliveUpdc
            End If
            Dim aPropertySet = ObjectPropertySet.Retrieve(id:=setid, updc:=updc, domainid:=domainid)
            If aPropertySet Is Nothing Then
                CoreMessageHandler(message:="property set to be added doesnot exist", messagetype:=otCoreMessageType.ApplicationError, _
                                    argument:=setid, objectname:=Me.ObjectID, procedure:="ObjectPropertyValueLot.AddSet")
                Return False
            End If

            '''
            ''' add the id -> done by event handling
            'If Not _setids.Contains(id) Then
            '    ReDim Preserve _setids(_setids.GetUpperBound(0) + 1)
            '    _setids(_setids.GetUpperBound(0)) = id
            'End If

            '''
            ''' Add All the values
            For Each aProperty In aPropertySet.Properties
                If Not Me.Values.ContainsKey({setid, aPropertySet.Updc, aProperty.ID}) Then
                    Me.AddPropertyValue(setid:=setid, setupdc:=updc, propertyid:=aProperty.ID)
                End If
            Next

            ''' set the vcalid from
            If Not Me.Validfrom.HasValue Then Me.Validfrom = Date.Now

            Return True
        End Function


        ''' <summary>
        ''' Add APropertyValue to the Lot
        ''' </summary>
        ''' <param name="setid"></param>
        ''' <param name="propertyid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AddPropertyValue(setid As String, propertyid As String, _
                                          Optional setupdc As Long? = Nothing, _
                                          Optional addSet As Boolean = True, _
                                          Optional value As Object = Nothing) As Boolean

            If Not setupdc.HasValue Then
                Dim anCurrentSet As ObjectPropertyCurrentSet = ObjectPropertyCurrentSet.Retrieve(id:=setid, domainid:=DomainID)
                setupdc = anCurrentSet.AliveUpdc
            End If

            '** add the set -> recursion loop due to setting default values
            'If (Not Me.PropertySetIDs.Contains(setid) And addSet) Then
            '    Me.AddSet(setid, updc:=setupdc)
            'End If

            ''' add the value
            If Me.PropertySetIDs.Contains(setid) OrElse addSet Then
                '' fix001 - 2014-11-17
                Dim aPropertyValue = ObjectPropertyValue.Create(uid:=Me.UID, updc:=Me.UPDC, setupdc:=setupdc, setid:=setid, propertyid:=propertyid)
                ''' apply the Extended Property CopyInitialValueFrom
                If aPropertyValue IsNot Nothing Then
                    Dim aPropertySet = Me.PropertySets.Where(Function(x) x.ID = setid.ToUpper).FirstOrDefault
                    If aPropertySet IsNot Nothing Then
                        Dim aProperty = aPropertySet.Properties.Item(propertyid)
                        If aProperty IsNot Nothing AndAlso aProperty.ExtendedProperties.Count > 0 Then
                            Dim aValue As Object
                            Dim anExtProp As ObjectPropertyExtProperty = aProperty.ExtendedProperties.Where(Function(x) x.Enum = otObjectPropertyExtProperty.CopyInitialValueFrom).FirstOrDefault
                            If anExtProp IsNot Nothing Then
                                If anExtProp.Apply(aValue, Me) Then
                                    value = aValue
                                End If
                            End If
                        End If
                    End If
                Else
                    aPropertyValue = ObjectPropertyValue.Retrieve(Me.UID, updc:=Me.UPDC, setid:=setid, propertyid:=propertyid)
                End If

                ' set the value
                If value IsNot Nothing Then aPropertyValue.ValueString = CStr(value)

                '**
                If aPropertyValue IsNot Nothing Then Me.Values.Add(aPropertyValue) ' set is added by the Add Event on the relationalcollection
                Return True
            End If

            CoreMessageHandler("set '" & setid & "' is not attached to this property value lot", argument:=Converter.Array2StringList(Me.ObjectPrimaryKeyValues), _
                               objectname:=Me.ObjectID, messagetype:=otCoreMessageType.InternalError, procedure:="AddPropertyValue")
            Return False
        End Function
        ''' <summary>
        ''' retrieve  the configuration from store
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(uid As Long, updc As Long, Optional domainid As String = Nothing) As ObjectPropertyValueLot
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Return ormBusinessObject.RetrieveDataObject(Of ObjectPropertyValueLot)(pkArray:={uid, updc}, domainID:=domainid)
        End Function

        ''' <summary>
        ''' handler for onCreating Event - generates unique primary key values
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub PropertySet_OnCreating(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnCreating
            Dim uid As Long? = e.Record.GetValue(constFNUID)
            Dim updc As Long? = e.Record.GetValue(ConstFNVersion)
            Dim tag As String
            If Not uid.HasValue OrElse uid = 0 Then
                tag = constFNUID
                uid = Nothing
                updc = 1
            ElseIf Not updc.HasValue OrElse updc = 0 Then
                updc = Nothing
                tag = ConstFNVersion
            End If
            Dim primarykey As Object() = {uid, updc}
            If uid Is Nothing OrElse updc Is Nothing Then
                If e.DataObject.ObjectPrimaryContainerStore.CreateUniquePkValue(pkArray:=primarykey, tag:=tag) Then
                    e.Record.SetValue(constFNUID, primarykey(0))
                    e.Record.SetValue(ConstFNVersion, primarykey(1))
                    e.Result = True
                    e.Proceed = True
                Else
                    CoreMessageHandler(message:="primary keys couldnot be created ?!", procedure:="ConfigSet.OnCreate", messagetype:=otCoreMessageType.InternalError)
                End If
            End If

        End Sub
        ''' <summary>
        ''' creates a persistable configuration
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(Optional uid As Long = 0, Optional updc As Long = 0, Optional domainid As String = Nothing) As ObjectPropertyValueLot
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Return ormBusinessObject.CreateDataObject(Of ObjectPropertyValueLot)(pkArray:={uid, updc}, domainID:=domainid, checkUnique:=True)
        End Function

        ''' <summary>
        ''' Handler for added PropertyValues
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ValuesCollection_OnAdded(sender As Object, e As Database.ormRelationCollection(Of ObjectPropertyValue).EventArgs) Handles _valuesCollection.OnAdded
            '''
            ''' add the id
            ''' 
            Dim aPropertyValue As ObjectPropertyValue = e.Dataobject
            If aPropertyValue Is Nothing Then
                CoreMessageHandler(message:="something different than ObjectPropertyValue added to valuescollection", procedure:="_ValuesCollection_OnAdded", _
                                   argument:=e.Dataobject.ObjectID, objectname:=Me.ObjectID, messagetype:=otCoreMessageType.InternalError)
                Return
            End If
            If Not _setids.Contains(aPropertyValue.SetID) Then
                ReDim Preserve _setids(_setids.GetUpperBound(0) + 1)
                ReDim Preserve _setsupdc(_setsupdc.GetUpperBound(0) + 1)
                _setids(_setids.GetUpperBound(0)) = aPropertyValue.SetID
                _setsupdc(_setsupdc.GetUpperBound(0)) = aPropertyValue.Setupdc
                _propertysets.Add(aPropertyValue.PropertySet)
            End If
            ' register PropertyChange
            AddHandler aPropertyValue.PropertyChanged, AddressOf Me.ObjectPropertyValueLot_PropertyValueChanged
        End Sub
        ''' <summary>
        ''' Handler for added PropertyValues
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ValuesCollection_OnRemoved(sender As Object, e As Database.ormRelationCollection(Of ObjectPropertyValue).EventArgs) Handles _valuesCollection.OnRemoved
            '''
            ''' add the id
            ''' 
            Dim aPropertyValue As ObjectPropertyValue = e.Dataobject
            If aPropertyValue Is Nothing Then
                CoreMessageHandler(message:="something different than ObjectPropertyValue added to valuescollection", procedure:="_ValuesCollection_OnAdded", _
                                   argument:=e.Dataobject.ObjectID, objectname:=Me.ObjectID, messagetype:=otCoreMessageType.InternalError)
                Return
            End If


            If _valuesCollection.ToList.Where(Function(x) x.SetID = aPropertyValue.SetID).FirstOrDefault Is Nothing Then
                Dim newids As String()
                Dim newsupdc As String()
                ReDim Preserve newids(_setids.GetUpperBound(0) - 1)
                ReDim Preserve newsupdc(_setsupdc.GetUpperBound(0) - 1)
                Dim j As Integer
                For i = 0 To _setids.GetUpperBound(0)
                    If _setids(i) <> aPropertyValue.SetID Then
                        newids(j) = _setids(i)
                        newsupdc(j) = _setsupdc(i)
                        j += 1
                    End If
                Next
                '* set
                _setids = newids
                _setsupdc = newsupdc
                _propertysets.Remove(aPropertyValue.PropertySet)
            End If

            ' register PropertyChange
            RemoveHandler aPropertyValue.PropertyChanged, AddressOf Me.ObjectPropertyValueLot_PropertyValueChanged
        End Sub
        ''' <summary>
        ''' Handler for ValueChange of PropertyValue
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectPropertyValueLot_PropertyValueChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs)
            If e.PropertyName = ObjectPropertyValue.ConstFNValue Then
                ''' 
                ''' 
            End If
        End Sub

        ''' <summary>
        ''' clones an value lot to a new updc
        ''' </summary>
        ''' <param name="pkarray"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Clone(Optional uid As Long? = Nothing, Optional updc As Long? = Nothing) As ObjectPropertyValueLot
            If Not IsAlive(subname:="Clone") Then Return Nothing

            Dim primarykey As Object()
            primarykey = {uid, updc}
            Return MyBase.Clone(Of ObjectPropertyValueLot)(primarykey)
        End Function

        ''' <summary>
        ''' Clone Handler to clone the related objects as well
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ObjectPropertyValueLot_OnCloned(sender As Object, e As ormDataObjectCloneEventArgs) Handles Me.OnCloned
            If Not e.AbortOperation Then
                Dim aNewObject As ObjectPropertyValueLot = TryCast(e.NewObject, ObjectPropertyValueLot)
                ' now clone the Members (Milestones)
                For Each aPropertyValue In _valuesCollection
                    aNewObject.Values.Add(aPropertyValue.Clone(uid:=aNewObject.UID, updc:=aNewObject.UPDC, setid:=aPropertyValue.SetID, propertyid:=aPropertyValue.PropertyID))
                Next
            End If

        End Sub

        ''' <summary>
        ''' Event Handler for OnRelationLoad. Check if all Properties of the set are included - if not (added or deleted) than add or drop values
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>

        Private Sub ObjectPropertyValueLot_OnRelationLoad(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnRelationLoad

            If e.RelationIDs.Contains(ConstRValues) And e.Infusemode = otInfuseMode.OnInject Then

                ''' check if we have all Properties of the sets also in the values - if not add it 
                ''' 

                For Each aPropertySetName As String In Me.PropertySetIDs
                    Dim anupdc As Long = CLng(Me.Setsupdc(Array.IndexOf(Me.PropertySetIDs, aPropertySetName)))
                    Dim aPropertyset As ObjectPropertySet = ObjectPropertySet.Retrieve(aPropertySetName, anupdc, domainid:=Me.DomainID)
                    If aPropertyset Is Nothing Then
                        CoreMessageHandler("property set could not be retrieved", dataobject:=Me, argument:=aPropertySetName, domainid:=Me.DomainID, _
                                           procedure:="ObjectPropertyValueLot.OnRelationLoad", messagetype:=otCoreMessageType.InternalWarning)
                    Else
                        For Each aProperty As ObjectProperty In aPropertyset.Properties
                            If Not Me.Values.ContainsKey(aProperty.ID) Then
                                Me.AddPropertyValue(setid:=aPropertySetName, propertyid:=aProperty.ID)
                            End If
                        Next
                    End If

                Next

                ''' counter check do we have a value which is not in a set (or deleted)
                ''' 
                For Each aPropertyValue In Me.Values
                    Dim aPropertyset As ObjectPropertySet = ObjectPropertySet.Retrieve(aPropertyValue.SetID, aPropertyValue.Setupdc, domainid:=Me.DomainID)
                    If aPropertyset IsNot Nothing AndAlso Not aPropertyset.Properties.ContainsKey(aPropertyValue.PropertyID) Then
                        aPropertyValue.Delete() 'delete it
                    End If
                Next
            End If
        End Sub


        ''' <summary>
        ''' load the PropertySets into dynamic internal structure
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function LoadPropertySets() As Boolean
            Dim i As Integer
            For Each aSetname In Me.PropertySetIDs
                If _setsupdc IsNot Nothing AndAlso _setsupdc.GetUpperBound(0) <= i Then
                    If IsNumeric(_setsupdc(i)) Then
                        Dim foundSet = _propertysets.Where(Function(x) x.ID = aSetname).FirstOrDefault()
                        If foundSet Is Nothing Then
                            Dim anUpdc = CLng(_setsupdc(i))
                            Dim aSet As ObjectPropertySet = ObjectPropertySet.Retrieve(id:=aSetname, updc:=anUpdc)
                            If aSet IsNot Nothing Then
                                _propertysets.Add(aSet)
                            End If
                        End If

                        i += 1
                    Else
                        CoreMessageHandler("updc for property set is not stored", argument:=_setsupdc, _
                                       objectname:=Me.ObjectID, entryname:=ConstFNSetUPDCs)
                    End If

                Else
                    CoreMessageHandler("updc for property set is not stored", argument:=Converter.Array2StringList(Me.ObjectPrimaryKeyValues), _
                                       objectname:=Me.ObjectID, entryname:=ConstFNSetUPDCs)
                End If
            Next
            Return True
        End Function
        ''' <summary>
        ''' Handler for the OnInfused Event 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectPropertyValueLot_OnInfused(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnColumnsInfused
            LoadPropertySets()
        End Sub


    End Class


    ''' <summary>
    ''' class for config properties of entities attached to other business objects
    ''' </summary>
    ''' <remarks>
    ''' 
    ''' Design Principles:
    ''' 
    ''' 1. Values should be never created by Create - go over the Value Lot instead. Values are not added automatically to the Lot.
    ''' 
    ''' 2. Values should be never retrieved alone - go over the lot instead.
    ''' 
    ''' </remarks>

    <ormObject(id:=ObjectPropertyValue.ConstObjectID, version:=1, adddomainbehavior:=False, adddeletefieldbehavior:=True, usecache:=True, _
        modulename:=ConstModuleProperties, Title:="Property Value", description:="values of object properties attached to bussiness object")> _
    Public Class ObjectPropertyValue
        Inherits ormBusinessObject
        Implements iormCloneable(Of ObjectPropertyValue)

        Public Const ConstObjectID = "PropertyValue"

        ''' <summary>
        ''' Table Definition
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=1, usecache:=False)> Public Const ConstPrimaryTableID = "TBLOBJPROPERTYVALUES"

        ''' <summary>
        ''' Primary KEys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(ReferenceObjectEntry:=ObjectPropertyValueLot.ConstObjectID & "." & ObjectPropertyValueLot.constFNUID, PrimaryKeyOrdinal:=1, _
              XID:="PV1", lookupPropertyStrings:={LookupProperty.UseForeignKey & "(" & constFKValues & ")"}, validationPropertyStrings:={ObjectValidationProperty.UseLookup})> _
        Public Const constFNUID = ObjectPropertyValueLot.constFNUID

        <ormObjectEntry(ReferenceObjectEntry:=ObjectPropertyValueLot.ConstObjectID & "." & ObjectPropertyValueLot.ConstFNVersion, PrimaryKeyOrdinal:=2, _
             XID:="PV2", lookupPropertyStrings:={LookupProperty.UseForeignKey & "(" & constFKValues & ")"}, validationPropertyStrings:={ObjectValidationProperty.UseLookup})> _
        Public Const ConstFNVersion = ObjectPropertyValueLot.ConstFNVersion

        <ormObjectEntry(ReferenceObjectEntry:=ObjectProperty.ConstObjectID & "." & ObjectProperty.ConstFNSetID, PrimaryKeyOrdinal:=3, _
            XID:="PV3")> _
        Public Const ConstFNSetID = ObjectProperty.ConstFNSetID

        <ormObjectEntry(ReferenceObjectEntry:=ObjectProperty.ConstObjectID & "." & ObjectProperty.ConstFNPropertyID, PrimaryKeyOrdinal:=4, _
            XID:="PV4")> _
        Public Const ConstFNPropertyID = ObjectProperty.ConstFNPropertyID

        <ormObjectEntry(ReferenceObjectEntry:=ObjectProperty.ConstObjectID & "." & ObjectProperty.ConstFNVersion, _
            XID:="PV5", title:="PropertySet UpdateCounter", description:="property set updatecounters ")> Public Const ConstFNSetUPDC = "SETUPDC"



        ''' <summary>
        '''  Fields
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
          XID:="PV10", title:="Value", description:="Value in string representation")> Public Const ConstFNValue = "VALUE"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, _
          useforeignkey:=otForeignKeyImplementation.None, defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        <ormObjectEntry(defaultvalue:=otDataType.Text, Datatype:=otDataType.Long, _
                              title:="Datatype", Description:="OTDB field data type")> Public Const ConstFNDatatype As String = "datatype"

        ''' <summary>
        ''' Foreign Keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormForeignKey(entrynames:={constFNUID, ConstFNVersion}, _
           foreignkeyreferences:={ObjectPropertyValueLot.ConstObjectID & "." & ObjectPropertyValueLot.constFNUID, _
                                  ObjectPropertyValueLot.ConstObjectID & "." & ObjectPropertyValueLot.ConstFNVersion}, _
           useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const constFKValues = "FK_PropertyValue_Lot"


        ''' <summary>
        ''' Mappings
        ''' </summary>
        ''' <remarks></remarks>

        <ormObjectEntryMapping(EntryName:=constFNUID)> Private _uid As Long = 0
        <ormObjectEntryMapping(EntryName:=ConstFNVersion)> Private _updc As Long = 0
        <ormObjectEntryMapping(EntryName:=ConstFNSetID)> Private _SetID As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNPropertyID)> Private _propertyID As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNValue)> Private _value As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNDatatype)> Private _datatype As otDataType
        <ormObjectEntryMapping(EntryName:=ConstFNSetUPDC)> Private _setupdc As Long
        ''' <summary>
        ''' Relations
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(ObjectProperty), cascadeOnDelete:=False, cascadeOnUpdate:=False, _
                     toprimarykeys:={ConstFNSetID, ConstFNSetUPDC, ConstFNPropertyID})> Public Const ConstRProperty = "RELObjectProperty"

        <ormObjectEntryMapping(RelationName:=ConstRProperty, infuseMode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand)> Private WithEvents _propertyDefinition As ObjectProperty

        ''' <summary>
        ''' dynamic member
        ''' </summary>
        ''' <remarks></remarks>



#Region "Properties"

        ''' <summary>
        ''' returns the PropertySet this PropertyValue Belongs to
        ''' </summary>
        ''' <param name="domainid"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property PropertySet(Optional domainid As String = Nothing) As ObjectPropertySet
            Get
                If Me.Property IsNot Nothing Then
                    Return Me.Property.PropertySet
                Else
                    Return Nothing
                End If

            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the setupdc.
        ''' </summary>
        ''' <value>The setupdc.</value>
        Public Property Setupdc() As Long?
            Get
                Return Me._setupdc
            End Get
            Set(value As Long?)
                SetValue(ConstFNSetUPDC, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the datatype of the property.
        ''' </summary>
        ''' <value>The datatype.</value>
        Public ReadOnly Property Datatype() As otDataType
            Get
                Return [Property].Datatype
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the entity.
        ''' </summary>
        ''' <value>The entity.</value>
        Public ReadOnly Property [Property]() As ObjectProperty
            Get
                If Not IsAlive(subname:="[Property]") Then Return Nothing
                InfuseRelation(ConstRProperty)
                Return Me._propertyDefinition
            End Get
        End Property

        ''' <summary>
        ''' returns the UID of the configuration set
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property UID()
            Get
                Return _uid
            End Get
        End Property

        ''' <summary>
        ''' returns the UPDC of the configuration set
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property UPDC()
            Get
                Return _updc
            End Get
        End Property
        '' <summary>
        ''' Gets or sets the Property id.
        ''' </summary>
        ''' <value>The properties.</value>
        Public ReadOnly Property PropertyID() As String
            Get
                Return Me._propertyID
            End Get
        End Property
        '' <summary>
        ''' Gets or sets the set id.
        ''' </summary>
        ''' <value>The properties.</value>
        Public ReadOnly Property SetID() As String
            Get
                Return Me._SetID
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the value in string presenation.
        ''' </summary>
        ''' <value>The description.</value>
        Public Property ValueString() As String
            Get
                Return Me._value
            End Get
            Set(value As String)
                SetValue(ConstFNValue, value)
            End Set
        End Property


#End Region



        ''' <summary>
        ''' retrieve  the configuration set value from store
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(uid As Long, updc As Long, setid As String, propertyid As String, Optional domainid As String = Nothing) As ObjectPropertyValue
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Return ormBusinessObject.RetrieveDataObject(Of ObjectPropertyValue)(pkArray:={uid, updc, setid, propertyid, domainid}, domainID:=domainid)
        End Function


        ''' <summary>
        ''' creates a persistable property value collection value
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(uid As Long, updc As Long, setid As String, propertyid As String, _
                                                Optional setupdc As Long? = Nothing,
                                                Optional domainid As String = Nothing) As ObjectPropertyValue
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim arecord As New ormRecord
            With arecord
                .SetValue(constFNUID, uid)
                .SetValue(ConstFNVersion, updc)
                .SetValue(ConstFNSetID, setid)
                .SetValue(ConstFNPropertyID, propertyid)
                .SetValue(ConstFNDomainID, domainid)
                If setupdc.HasValue Then .SetValue(ConstFNSetUPDC, setupdc.Value)
            End With
            Return ormBusinessObject.CreateDataObject(Of ObjectPropertyValue)(arecord, domainID:=domainid, checkUnique:=True)
        End Function

        ''' <summary>
        ''' onCreating Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectPropertyValue_OnCreating(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreating
            ''' check of the property exist
            ''' 
            Dim setid As String = e.Record.GetValue(ConstFNSetID)
            Dim propertyid As String = e.Record.GetValue(ConstFNPropertyID)
            Dim domainid As String = e.Record.GetValue(ConstFNDomainID)
            Dim setupdc As Long? = e.Record.GetValue(ConstFNSetUPDC)

            If setid IsNot Nothing AndAlso propertyid IsNot Nothing Then
                If Not setupdc.HasValue Then
                    Dim aCurrentSet As ObjectPropertyCurrentSet = ObjectPropertyCurrentSet.Retrieve(id:=setid, domainid:=domainid)
                    If aCurrentSet Is Nothing Then
                        CoreMessageHandler(message:="property set does not exist", argument:=setid, messagetype:=otCoreMessageType.ApplicationError, objectname:=ConstObjectID, _
                                      procedure:="ObjectPropertyValue.OnCreating")
                        e.AbortOperation = True
                    Else
                        setupdc = aCurrentSet.AliveUpdc
                        e.Record.SetValue(ConstFNSetUPDC, setupdc.Value)
                    End If

                End If
                ''' to early to set the link but has to be checked anyway
                _propertyDefinition = ObjectProperty.Retrieve(setid:=setid, setupdc:=setupdc, ID:=propertyid)
                If _propertyDefinition Is Nothing Then
                    CoreMessageHandler(message:="property does not exist", argument:=setid & "." & propertyid, messagetype:=otCoreMessageType.ApplicationError, objectname:=ConstObjectID, _
                                       procedure:="ObjectPropertyValue.OnCreating")
                    e.AbortOperation = True
                Else
                    ''' set this too
                    _datatype = _propertyDefinition.Datatype
                    _value = _propertyDefinition.DefaultValue
                    _setupdc = _propertyDefinition.Setupdc
                End If
            End If
        End Sub

        ''' <summary>
        ''' clone the object with the new primary key
        ''' </summary>
        ''' <param name="pkarray">primary key array</param>
        ''' <remarks></remarks>
        ''' <returns>the new cloned object or nothing</returns>
        Public Function Clone(uid As Long, updc As Long, setid As String, propertyid As String, Optional domainid As String = Nothing) As ObjectPropertyValue
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Return Clone(pkarray:={uid, updc, setid, propertyid, domainid})
        End Function
        ''' <summary>
        ''' clone the object with the new primary key
        ''' </summary>
        ''' <param name="pkarray">primary key array</param>
        ''' <remarks></remarks>
        ''' <returns>the new cloned object or nothing</returns>
        Public Function Clone(pkarray As Object(), Optional runtimeOnly As Boolean? = Nothing) As ObjectPropertyValue Implements iormCloneable(Of ObjectPropertyValue).Clone
            Return MyBase.Clone(Of ObjectPropertyValue)(newpkarray:=pkarray)
        End Function
    End Class

End Namespace

