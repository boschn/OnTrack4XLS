

REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** BUSINESS OBJECTs CLASSES: Tracking Classes
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2014-08-08
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************


Option Explicit On
Imports System.Diagnostics
Imports System.Linq
Imports System.Diagnostics.Debug

Imports OnTrack.Database
Imports OnTrack.Commons
Imports OnTrack.Deliverables
Imports OnTrack.Core

Namespace OnTrack.Tracking

    ''' <summary>
    ''' Definition class for Tracking Entries
    ''' </summary>
    ''' <remarks></remarks>
    '''
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleTracking, Version:=2, Release:=0, patch:=0, changeimplno:=1, _
            description:="Introducing Track Log Entry Types")> _
    <ormObject(id:=TrackLogEntryType.ConstObjectID, description:="type definition of an tracking entry.", _
        modulename:=ConstModuleTracking, Version:=1, useCache:=True, adddeletefieldbehavior:=True, addDomainBehavior:=True)> _
    Public Class TrackLogEntryType
        Inherits ormBusinessObject

        Public Const ConstObjectID = "TrackLogEntryType"
        ''' <summary>
        ''' primary Table
        ''' </summary>
        ''' <remarks></remarks>
        <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleRepository, Version:=ConstOTDBSchemaVersion, Release:=0, patch:=0, changeimplno:=1, _
           description:="Introducing new database table " & ConstPrimaryTableID)> _
       <ormTableAttribute(version:=1, usecache:=True)> Public Const ConstPrimaryTableID = "TBLDEFTRACKLOGENTRYTYPES"

        '** indexes
        <ormIndex(columnName1:=ConstFNDomainID, columnname2:=constFNTypeID, columnname3:=ConstFNIsDeleted)> Public Const constIndexDomain = "indDomains"

        ''' <summary>
        ''' keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertystrings:={ObjectValidationProperty.NotEmpty}, _
           title:="Type", description:="type of the TRACK entry", XID:="TRACKT1")> Public Const constFNTypeID = "ID"
        ' switch FK too NOOP since we have a dependency to deliverables
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2, _
            useforeignkey:=otForeignKeyImplementation.ORM, _
            foreignkeyProperties:={ForeignKeyProperty.OnDelete & "(" & ForeignKeyActionProperty.NOOP & ")", _
                                    ForeignKeyProperty.OnUpdate & "(" & ForeignKeyActionProperty.NOOP & ")"})> _
        Public Const ConstFNDomainID = Domain.ConstFNDomainID

        ''' <summary>
        ''' Fields
        ''' </summary>
        ''' <remarks></remarks>
        ''' 

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
            properties:={ObjectEntryProperty.Trim}, validationPropertystrings:={ObjectValidationProperty.NotEmpty}, _
            title:="Classname", description:="classname of the TRACK entry type", XID:="TRACKT2")> Public Const ConstFNClassname = "CLASS"

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
         title:="Description", description:="description of the TRACK entry type", XID:="TRACKT3")> Public Const constFNDescription = "DESC"

        <ormObjectEntry(Datatype:=otDataType.Memo, isnullable:=True, _
            title:="comment", description:="comments of the TRACK entry type", XID:="TRACKT10")> Public Const constFNComment = "CMT"

        <ormObjectEntry(referenceObjectEntry:=Commons.StatusType.ConstObjectID & "." & Commons.StatusType.ConstFNID, _
                        isnullable:=True, _
                        Title:="Status Type", description:="type of the status", _
                        XID:="TRACKT20", isnullable:=True)> Public Const ConstFNLinkStatusType = "STATUSTYPE"

        ''' <summary>
        ''' Foreign Key to Status Type
        ''' </summary>
        ''' <remarks></remarks>
        <ormForeignKey(entrynames:={ConstFNLinkStatusType, ConstFNDomainID}, _
            foreignkeyreferences:={Commons.StatusType.ConstObjectID & "." & Commons.StatusType.ConstFNID, _
                                   Commons.StatusType.ConstObjectID & "." & Commons.StatusType.ConstFNDomainID}, _
            foreignkeyproperties:={ForeignKeyProperty.OnDelete & "(" & ForeignKeyActionProperty.SetDefault & ")", _
                                   ForeignKeyProperty.OnUpdate & "(" & ForeignKeyActionProperty.Cascade & ")"}, _
            useforeignkey:=otForeignKeyImplementation.ORM)> Public Const constFKStatusType = "FKStatusType"

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertystrings:={ObjectValidationProperty.NotEmpty}, _
            title:="OPRule", description:="internal Operation / Method to call on Status Rules", XID:="TRACKT30")> _
        Public Const constFNRuleOperation = "RuleOperation"

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertystrings:={ObjectValidationProperty.NotEmpty}, _
            title:="RulezSet", description:="Rulez Ruleset to call on status rules", XID:="TRACKT31")> Public Const constFNRulezSet = "RuleSet"

        '*** Mapping
        <ormObjectEntryMapping(EntryName:=constFNTypeID)> Private _typeid As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNClassname)> Private _classname As String
        <ormObjectEntryMapping(EntryName:=constFNDescription)> Private _description As String
        <ormObjectEntryMapping(EntryName:=constFNComment)> Private _comment As String
        <ormObjectEntryMapping(EntryName:=ConstFNLinkStatusType)> Private _statustype As String
        <ormObjectEntryMapping(EntryName:=constFNRuleOperation)> Private _ruleOperation As String

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the classname.
        ''' </summary>
        ''' <value>The classname.</value>
        Public Property Classname() As String
            Get
                Return Me._classname
            End Get
            Set(value As String)
                SetValue(ConstFNClassname, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the statustype.
        ''' </summary>
        ''' <value>The statustype.</value>
        Public Property Statustype() As String
            Get
                Return Me._statustype
            End Get
            Set(value As String)
                SetValue(ConstFNLinkStatusType, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the comment.
        ''' </summary>
        ''' <value>The comment.</value>
        Public Property Comment() As String
            Get
                Return Me._comment
            End Get
            Set(value As String)
                SetValue(constFNComment, value)
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
                SetValue(constFNDescription, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the typeid.
        ''' </summary>
        ''' <value>The typeid.</value>
        Public ReadOnly Property Typeid() As String
            Get
                Return Me._typeid
            End Get

        End Property
#End Region

        ''' <summary>
        ''' creates a persistable Track Log Entry Type
        ''' </summary>
        ''' <param name="workspaceID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal typeid As String, Optional ByVal domainid As String = Nothing) As TrackLogEntryType
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey() As Object = {typeid, domainid}
            Return CreateDataObject(Of TrackLogEntryType)(pkArray:=primarykey, domainID:=domainid, checkUnique:=True)
        End Function


        ''' <summary>
        ''' Retrieve a deliverable Type object
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal typeid As String, Optional ByVal domainid As String = Nothing, Optional forcereload As Boolean = False) As TrackLogEntryType
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim pkarray() As Object = {typeid, domainid}
            Return RetrieveDataObject(Of TrackLogEntryType)(pkArray:=pkarray, forceReload:=forcereload)
        End Function

#Region "static routines"
        ''' <summary>
        ''' returns a List(of TrackLogEntryTypes) for the DomainID
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function All(Optional domainid As String = Nothing) As List(Of TrackLogEntryType)
            Return ormBusinessObject.AllDataObject(Of TrackLogEntryType)(domainid:=domainid)
        End Function
#End Region
    End Class

    ''' <summary>
    ''' the TRACK LINK class links a business object to a TRACK Entry on n:m
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' 
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleTracking, Version:=2, Release:=0, patch:=0, changeimplno:=3, _
           description:="Introducing Track Log Entry Link Objects")> _
   <ormObject(id:=TrackLogLink.ConstObjectID, modulename:=ConstModuleTracking, Version:=1, _
       usecache:=True, adddomainbehavior:=False, adddeletefieldbehavior:=True, _
       description:="link definitions between TRACK entries  and other business objects")> _
    Public Class TrackLogLink
        Inherits ormBusinessObject


        Public Const ConstObjectID = "TrackLogLink"

        ''' <summary>
        ''' Table
        ''' </summary>
        ''' <remarks></remarks>
        <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleRepository, Version:=ConstOTDBSchemaVersion, Release:=0, patch:=0, changeimplno:=10, _
              description:="added table " & ConstPrimaryTableID)> _
              <ormTableAttribute(version:=1)> Public Const ConstPrimaryTableID = "TBLTRACKLOGLINKS"

        '** index
        <ormIndex(columnname1:=ConstFNToTrackEntryUid, columnname2:=ConstFNFromObjectID, columnname3:=ConstFNFromUid)> Public Const ConstIndTag = "USED"

        ''' <summary>
        ''' Primary key of the property link object
        ''' FROM an ObjectID, UID, UPDC (KEY)
        ''' TO   an OBJECTID, UID, UPDC
        ''' 
        ''' links a  business objects (deliverable, pars, configcondition (for own use) ) with a TRACK entry
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        ''' 

        <ormObjectEntry(referenceobjectentry:=ormObjectDefinition.ConstObjectID & "." & ormObjectDefinition.ConstFNID, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, _
            validationPropertyStrings:={ObjectValidationProperty.NotEmpty, ObjectValidationProperty.UseLookup}, _
            LookupPropertyStrings:={LookupProperty.UseAttributeValues}, _
            values:={Deliverables.Deliverable.ConstObjectID, Parts.Part.ConstObjectID, Configurables.ConfigItemSelector.ConstObjectID}, _
            dbdefaultvalue:=Deliverable.ConstObjectID, defaultvalue:=Deliverable.ConstObjectID, _
            XID:="TTLINK1", title:="From Object", description:="from object id of the business object")> _
        Public Const ConstFNFromObjectID = "FROMOBJECTID"

        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=2, dbdefaultvalue:="0", lowerrange:=0, _
            XID:="TTLINK2", title:="Linked from UID", description:="from uid of the business object")> _
        Public Const ConstFNFromUid = "FROMUID"

        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=3, dbdefaultvalue:="0", lowerrange:=0, _
            XID:="TTLINK3", title:="Linked from UPDC", description:="from updc of the business object")> _
        Public Const ConstFNFromUpdc = "FROMUPDC"

        <ormObjectEntry(referenceobjectentry:=TrackLogEntry.ConstObjectID & "." & TrackLogEntry.constFNUID, PrimaryKeyOrdinal:=4, _
            useforeignkey:=otForeignKeyImplementation.NativeDatabase, _
            XID:="TTLINK5", title:="Linked to UID", description:="uid link to the track log entry")> _
        Public Const ConstFNToTrackEntryUid = "TOUID"

        ''' <summary>
        ''' Column Definitions
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, dbdefaultvalue:="One2One", defaultvalue:=otLinkType.One2One, _
            XID:="TTLINK10", title:="Link Type", description:="object link type")> Public Const ConstFNTypeID = "TYPEID"

        ' deactivate ForeignKEy we do not have this object in domains
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID,
            useforeignkey:=otForeignKeyImplementation.None)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        ''' <summary>
        ''' Mappings persistable members
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNFromObjectID)> Private _FromObjectID As String
        <ormObjectEntryMapping(EntryName:=ConstFNFromUid)> Private _FromUid As Long
        <ormObjectEntryMapping(EntryName:=ConstFNFromUpdc)> Private _FromUpdc As Long
        <ormObjectEntryMapping(EntryName:=ConstFNToTrackEntryUid)> Private _ToUid As Long

        <ormObjectEntryMapping(EntryName:=ConstFNTypeID)> Private _type As otLinkType

        ''' <summary>
        ''' Relation to PropertyValueLot - will be resolved by event handler on relation manager
        ''' </summary>
        ''' <remarks></remarks>
        '<ormRelation(linkObject:=GetType(ObjectPropertyValueLot), createobjectifnotretrieved:=True, toPrimarykeys:={ConstFNToUid, ConstFNToUpdc}, _
        '             cascadeonCreate:=True, cascadeOnDelete:=True, cascadeOnUpdate:=True)> _
        'Public Const ConstRPropertyValueLot = "RELPROPERTYVALUELOT"

        '<ormEntryMapping(relationName:=ConstRPropertyValueLot, infusemode:=otInfuseMode.OnCreate Or otInfuseMode.OnInject Or otInfuseMode.OnDemand)> _
        'Private _propertyValueLot As ObjectPropertyValueLot

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

#Region "properties"

        ''' <summary>
        ''' Gets or sets the property value lot.
        ''' </summary>
        ''' <value>The property value lot.</value>
        'Public ReadOnly Property PropertyValueLot() As ObjectPropertyValueLot
        '    Get
        '        If Not IsAlive(subname:="PropertyValueLot") Then Return Nothing

        '        If Me.GetRelationStatus(ConstRPropertyValueLot) <> DataObjectRelationMgr.RelationStatus.Loaded Then
        '            Me.InfuseRelation(ConstRPropertyValueLot)
        '        End If
        '        Return Me._propertyValueLot
        '    End Get

        'End Property

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
        Property ToTrackEntryUID() As Long
            Get
                Return _ToUid
            End Get
            Set(value As Long)
                SetValue(ConstFNToTrackEntryUid, value)
            End Set
        End Property


#End Region

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
            'If e.RelationID = ConstRPropertyValueLot Then
            '    Dim aPropertyLot As ObjectPropertyValueLot = ObjectPropertyValueLot.Retrieve(uid:=Me.ToUID, updc:=Me.ToUpdc)
            '    e.RelationObjects.Add(aPropertyLot)
            '    e.Finished = True
            'End If
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
            'If e.RelationID = ConstRPropertyValueLot Then
            'Dim aPropertyLot As ObjectPropertyValueLot = ObjectPropertyValueLot.Create(uid:=Me.ToUID, updc:=Me.ToUpdc)
            'If aPropertyLot Is Nothing Then aPropertyLot = ObjectPropertyValueLot.Retrieve(uid:=Me.ToUID, updc:=Me.ToUpdc)

            '' we have what we need
            'e.RelationObjects.Add(aPropertyLot)
            'e.Finished = True

            'End If
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
        Public Overloads Shared Function Create(fromObjectID As String, fromuid As Long, toTrackEntryUID As Long, _
                                                Optional fromupdc As Long = 0, _
                                                Optional domainid As String = Nothing _
                                                ) As TrackLogLink
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID

            '' set values
            Dim aRecord As New ormRecord
            With aRecord
                .SetValue(ConstFNFromObjectID, fromObjectID)
                .SetValue(ConstFNFromUid, fromuid)
                .SetValue(ConstFNFromUpdc, fromupdc)
                If Not String.IsNullOrEmpty(domainid) Then .SetValue(ConstFNDomainID, domainid)
                .SetValue(ConstFNToTrackEntryUid, toTrackEntryUID)
            End With

            Return ormBusinessObject.CreateDataObject(Of TrackLogLink)(aRecord, checkUnique:=True)
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
        Public Overloads Shared Function Retrieve(fromObjectID As String, fromUid As Long, fromUpdc As Long, toTrackEntryUID As Long, _
                                                  Optional domainid As String = Nothing) As TrackLogLink
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey As Object() = {fromObjectID, fromUid, fromUpdc, toTrackEntryUID}
            Return ormBusinessObject.RetrieveDataObject(Of TrackLogLink)(primarykey)
        End Function
    End Class

    ''' <summary>
    ''' Timeline Class is a Collection of TRACKEntries matching a common Link to a data object
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleTracking, Version:=2, Release:=0, patch:=0, changeimplno:=9, _
          description:="Introducing Timeline dynamic data object")> _
    Public Class Timeline

    End Class

    ''' <summary>
    ''' class for Track Log Entries
    ''' </summary>
    ''' <remarks>
    ''' Design Principles
    ''' 
    ''' </remarks>
    ''' 
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleTracking, Version:=2, Release:=0, patch:=0, changeimplno:=2, _
            description:="Introducing Track Log Entry Objects")> _
    <ormObject(id:=TrackLogEntry.ConstObjectID, version:=1, adddomainbEhavior:=False, adddeletefieldbehavior:=True, usecache:=True, _
        modulename:=ConstModuleTracking, Title:="Track Log Entry", description:="Base class for TRACK Entries")> _
    Public Class TrackLogEntry
        Inherits ormBusinessObject


        Public Const ConstObjectID = "TrackLogEntry"

        ''' <summary>
        ''' Table Definition
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleRepository, Version:=ConstOTDBSchemaVersion, Release:=0, patch:=0, changeimplno:=8, _
       description:="added table " & ConstPrimaryTableID)> _
       <ormTableAttribute(version:=1, usecache:=False)> Public Const ConstPrimaryTableID = "TBLTRACKLOGENTRIES"

        ''' <summary>
        ''' Index
        ''' </summary>
        ''' <remarks></remarks>
        <ormIndex(columnname1:=constFNMatchCode, columnname2:=constFNUID, columnname3:=ConstFNIsDeleted)> Public Const ConstIndexMatchcode = "indmatchcode"
        <ormIndex(columnname1:=constFNCategory, columnname2:=constFNUID, columnname3:=ConstFNIsDeleted)> Public Const ConstIndexcategory = "indcategory"
        <ormIndex(columnname1:=ConstFNFunction, columnname2:=constFNUID, columnname3:=ConstFNIsDeleted)> Public Const ConstIndexFunction = "indFunction"
        <ormIndex(columnname1:=ConstFNTypeID, columnname2:=constFNUID, columnname3:=ConstFNIsDeleted)> Public Const ConstIndexType = "indType"
        <ormIndex(columnname1:=ConstFNValidFrom, columnname2:=ConstFNValiduntil, columnname3:=constFNUID, columnname4:=ConstFNIsDeleted)> Public Const ConstIndexTimeline = "indTimeline"


        ''' <summary>
        ''' primary keys 
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=1, dbdefaultvalue:="0", ContainerID:=ConstPrimaryTableID, _
              XID:="TLOGE1", title:="UID", description:="UID of the track log entry")> Public Const ConstFNUID = "TLUID"


        ''' <summary>
        ''' Entries
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, defaultvalue:=ConstGlobalDomain, ContainerID:=ConstPrimaryTableID, _
          useforeignkey:=otForeignKeyImplementation.None, dbdefaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, ContainerID:=ConstPrimaryTableID, _
            XID:="TLOGE3", title:="Title", description:="title of the entry")> Public Const ConstFNTitle = "TITLE"

        <ormObjectEntry(Datatype:=otDataType.Memo, isnullable:=True, ContainerID:=ConstPrimaryTableID, _
            XID:="TLOGE4", title:="Posting", description:="posting or description of the entry")> Public Const ConstFNPosting = "POSTING"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, isnullable:=True, ContainerID:=ConstPrimaryTableID, _
            XID:="TLOGE5", title:="Author", description:="author of the entry")> Public Const ConstFNAuthor = "AUTHOR"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, isnullable:=True, ContainerID:=ConstPrimaryTableID, _
           XID:="TLOGE6", title:="responsible", description:="responsible person of the entry")> Public Const ConstFNResponsible = "RESP"

        <ormObjectEntry(referenceobjectentry:=OrgUnit.ConstObjectID & "." & OrgUnit.ConstFNID, isnullable:=True, _
           title:="Responsible OrgUnit", description:=" organization unit responsible for the entry", XID:="TLOGE7")> _
        Public Const constFNRespOU = "RESPOU"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, ContainerID:=ConstPrimaryTableID, _
           title:="category", description:="category of the entry", XID:="TLOGE8")> _
        Public Const constFNCategory = "CAT"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, isnullable:=True, ContainerID:=ConstPrimaryTableID, _
            title:="Matchcode", description:="match code of the entry", XID:="TLOGE9")> _
        Public Const constFNMatchCode = "MATCHCODE"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, isnullable:=True, ContainerID:=ConstPrimaryTableID, _
            title:="Function", description:="function of the entry", XID:="TLOGE10")> _
        Public Const ConstFNFunction = "FUNCTION"

        <ormObjectEntry(Datatype:=otDataType.Timestamp, isnullable:=True, ContainerID:=ConstPrimaryTableID, _
            XID:="TLOGE11", title:="valid from", description:="entry is valid from ")> Public Const ConstFNValidFrom = "VALIDFROM"

        <ormObjectEntry(Datatype:=otDataType.Timestamp, isnullable:=True, ContainerID:=ConstPrimaryTableID, _
            XID:="TLOGE12", title:="valid until", description:="entry is valid until ")> Public Const ConstFNValiduntil = "VALIDUNTIL"

        ''' <summary>
        ''' Track Log entry Type
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceobjectentry:=TrackLogEntryType.ConstObjectID & "." & TrackLogEntryType.constFNTypeID, ContainerID:=ConstPrimaryTableID, _
          title:="Type", description:="type of the entry", XID:="TLOGE13", _
          dbdefaultvalue:=ConstDefaultDeliverableType,
          LookupPropertyStrings:={LookupProperty.UseAttributeReference}, validationPropertyStrings:={ObjectValidationProperty.UseLookup} _
                  )> Public Const ConstFNTypeID = "TYPEID"

        ''' <summary>
        ''' Foreign Key to Track Log Type
        ''' </summary>
        ''' <remarks></remarks>
        <ormForeignKey(entrynames:={ConstFNTypeID, ConstFNDomainID}, _
            foreignkeyreferences:={TrackLogEntryType.ConstObjectID & "." & TrackLogEntryType.constFNTypeID, _
                                   TrackLogEntryType.ConstObjectID & "." & TrackLogEntryType.ConstFNDomainID}, _
            foreignkeyproperties:={ForeignKeyProperty.OnDelete & "(" & ForeignKeyActionProperty.SetDefault & ")", _
                                   ForeignKeyProperty.OnUpdate & "(" & ForeignKeyActionProperty.Cascade & ")"}, _
            useforeignkey:=otForeignKeyImplementation.ORM)> Public Const ConstFKType = "FKTypeID"

        ''' <summary>
        ''' Status type
        ''' </summary>
        ''' <remarks></remarks>

        <ormObjectEntry(referenceObjectEntry:=StatusItem.ConstObjectID & "." & StatusItem.constFNType, ContainerID:=ConstPrimaryTableID, _
            isnullable:=True, _
            Title:="Status Type", description:="type of the status", _
            XID:="TLOGE20", isnullable:=True)> Public Const ConstFNStatusType = "STATUSTYPE"


        ''' <summary>
        ''' Foreign Key to Status Type
        ''' </summary>
        ''' <remarks></remarks>
        <ormForeignKey(entrynames:={ConstFNStatusType, ConstFNDomainID}, _
            foreignkeyreferences:={Commons.StatusType.ConstObjectID & "." & Commons.StatusType.ConstFNID, _
                                   Commons.StatusType.ConstObjectID & "." & Commons.StatusType.ConstFNDomainID}, _
            foreignkeyproperties:={ForeignKeyProperty.OnDelete & "(" & ForeignKeyActionProperty.SetDefault & ")", _
                                   ForeignKeyProperty.OnUpdate & "(" & ForeignKeyActionProperty.Cascade & ")"}, _
            useforeignkey:=otForeignKeyImplementation.ORM)> Public Const ConstFKStatusType = "FKStatusType"

        ''' <summary>
        ''' Status
        ''' </summary>
        ''' <remarks></remarks>

        <ormObjectEntry(referenceObjectEntry:=StatusItem.ConstObjectID & "." & StatusItem.constFNCode, _
                       isnullable:=True, ContainerID:=ConstPrimaryTableID, _
                       title:="entry Status", Description:="entry status code", _
                        XID:="TLOGE21")> Public Const ConstFNStatusCode = "STATUS"

        ''' <summary>
        ''' Priority Status Type
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=StatusItem.ConstObjectID & "." & StatusItem.constFNType, ContainerID:=ConstPrimaryTableID, _
                      isnullable:=True, _
                       Title:="Prio Type", description:="type of the prio status", _
                       XID:="TLOGE22", isnullable:=True)> Public Const ConstFNPrioStatusType = "PRIOSTATUSTYPE"

        ''' <summary>
        ''' Foreign Key to Status Type
        ''' </summary>
        ''' <remarks></remarks>
        <ormForeignKey(entrynames:={ConstFNPrioStatusType, ConstFNDomainID}, _
            foreignkeyreferences:={Commons.StatusType.ConstObjectID & "." & Commons.StatusType.ConstFNID, _
                                   Commons.StatusType.ConstObjectID & "." & Commons.StatusType.ConstFNDomainID}, _
            foreignkeyproperties:={ForeignKeyProperty.OnDelete & "(" & ForeignKeyActionProperty.SetDefault & ")", _
                                   ForeignKeyProperty.OnUpdate & "(" & ForeignKeyActionProperty.Cascade & ")"}, _
            useforeignkey:=otForeignKeyImplementation.ORM)> Public Const constFKPrioStatusType = "FKPrioStatusType"


        ''' <summary>
        ''' Priority
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=StatusItem.ConstObjectID & "." & StatusItem.constFNCode, _
                       isnullable:=True, ContainerID:=ConstPrimaryTableID, _
                       title:="Priority", Description:="priority status code", _
                        XID:="TLOGE23")> Public Const ConstFNPriorityCode = "PRIO"
        ''' <summary>
        ''' Mappings
        ''' </summary>
        ''' <remarks></remarks>

        <ormObjectEntryMapping(EntryName:=constFNUID)> Private _uid As Long = 0
        <ormObjectEntryMapping(EntryName:=ConstFNTypeID)> Private _typeID As String

        <ormObjectEntryMapping(EntryName:=ConstFNTitle)> Private _title As String
        <ormObjectEntryMapping(EntryName:=ConstFNPosting)> Private _posting As String
        <ormObjectEntryMapping(EntryName:=ConstFNAuthor)> Private _author As String
        <ormObjectEntryMapping(EntryName:=ConstFNResponsible)> Private _responsible As String
        <ormObjectEntryMapping(EntryName:=constFNRespOU)> Private _responsibleOU As String
        <ormObjectEntryMapping(EntryName:=constFNCategory)> Private _category As String
        <ormObjectEntryMapping(EntryName:=constFNMatchCode)> Private _Matchcode As String
        <ormObjectEntryMapping(EntryName:=ConstFNFunction)> Private _function As String

        <ormObjectEntryMapping(EntryName:=ConstFNPrioStatusType)> Private _priostatustype As String
        <ormObjectEntryMapping(EntryName:=ConstFNPriorityCode)> Private _prioritycode As String

        <ormObjectEntryMapping(EntryName:=ConstFNStatusType)> Private _statustype As String
        <ormObjectEntryMapping(EntryName:=ConstFNStatusCode)> Private _statuscode As String

        <ormObjectEntryMapping(EntryName:=ConstFNValidFrom)> Private _validfrom As DateTime?
        <ormObjectEntryMapping(EntryName:=ConstFNValiduntil)> Private _validuntil As DateTime?


        ''' <summary>
        ''' Relation to  StatusItem
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(StatusItem), fromentries:={ConstFNStatusType, ConstFNStatusCode}, _
            toentries:={StatusItem.constFNType, StatusItem.constFNCode}, _
            cascadeOnCreate:=False, cascadeOnDelete:=False, cascadeOnUpdate:=False)> Public Const ConstRSTatus = "RELSTATUS"

        <ormObjectEntryMapping(RelationName:=ConstRPrioStatus, infuseMode:=otInfuseMode.OnDemand)> Private WithEvents _Status As StatusItem

        ''' <summary>
        ''' Relation to  StatusItem Prio
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(StatusItem), fromentries:={ConstFNPrioStatusType, ConstFNPriorityCode}, _
            toentries:={StatusItem.constFNType, StatusItem.constFNCode}, _
            cascadeOnCreate:=False, cascadeOnDelete:=False, cascadeOnUpdate:=False)> Public Const ConstRPrioStatus = "RELPRIOSTATUS"

        <ormObjectEntryMapping(RelationName:=ConstRPrioStatus, infuseMode:=otInfuseMode.OnDemand)> Private WithEvents _priority As StatusItem

        ''' <summary>
        ''' Relation to Type
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkObject:=GetType(DeliverableType), toprimaryKeys:={ConstFNTypeID, ConstFNDomainID}, _
                     cascadeonCreate:=False, cascadeOnDelete:=False, cascadeOnUpdate:=False)> _
        Public Const ConstRType = "RELType"

        <ormObjectEntryMapping(relationName:=ConstRType, infusemode:=otInfuseMode.OnDemand)> Private _Type As TrackLogEntryType

        ''' <summary>
        ''' Dynamic 
        ''' </summary>
        ''' <remarks></remarks>
        ''' 


#Region "Properties"

        ''' <summary>
        ''' gets or sets the  priority 
        ''' </summary>
        ''' <value></value>
        ''' <returns>statusitem</returns>
        ''' <remarks></remarks>
        Public Property Priority() As StatusItem
            Get
                If _priority Is Nothing OrElse _priority.Code <> _prioritycode OrElse _priority.TypeID <> _priostatustype Then InfuseRelation(ConstRPrioStatus)
                Return _priority
            End Get
            Set(value As StatusItem)
                If value IsNot Nothing Then
                    Me.Priostatustype = value.TypeID
                    Me.Prioritycode = value.Code
                    _priority = value
                Else
                    Me.Prioritycode = Nothing
                    Me.Priostatustype = Nothing
                    _priority = Nothing
                End If
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the prioritycode.
        ''' </summary>
        ''' <value>The prioritycode.</value>
        Public Property Prioritycode() As String
            Get
                Return Me._prioritycode
            End Get
            Set(value As String)
                Me._prioritycode = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the priostatustype.
        ''' </summary>
        ''' <value>The priostatustype.</value>
        Public Property Priostatustype() As String
            Get
                Return Me._priostatustype
            End Get
            Set(value As String)
                Me._priostatustype = value
            End Set
        End Property

        ''' <summary>
        ''' retrieves a type object of this entry
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Type As TrackLogEntryType
            Get
                If Not Me.IsAlive(subname:="type") Then Return Nothing
                Me.InfuseRelation(ConstRType)
                Return _Type

            End Get
        End Property
        ''' <summary>
        ''' gets or sets the  status item 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Status() As StatusItem
            Get
                If _Status Is Nothing OrElse _Status.Code <> _statuscode OrElse _statustype <> _Status.Code Then InfuseRelation(ConstRPrioStatus)
                Return _Status
            End Get
            Set(value As StatusItem)
                If value IsNot Nothing Then
                    Me.Statustype = value.TypeID
                    Me.Statuscode = value.Code
                    _Status = value
                Else
                    Me.Statuscode = Nothing
                    Me.Statustype = Nothing
                    _Status = Nothing
                End If
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the statuscode.
        ''' </summary>
        ''' <value>The statuscode.</value>
        Public Property Statuscode() As String
            Get
                Return Me._statuscode
            End Get
            Set(value As String)
                SetValue(ConstFNStatusCode, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the statustype.
        ''' </summary>
        ''' <value>The statustype.</value>
        Public Property Statustype() As String
            Get
                Return Me._statustype
            End Get
            Set(value As String)
                SetValue(ConstFNStatusType, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the function.
        ''' </summary>
        ''' <value>The function.</value>
        Public Property [Function]() As String
            Get
                Return Me._function
            End Get
            Set(value As String)
                SetValue(ConstFNFunction, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the matchcode.
        ''' </summary>
        ''' <value>The matchcode.</value>
        Public Property Matchcode() As String
            Get
                Return Me._Matchcode
            End Get
            Set(value As String)
                SetValue(constFNMatchCode, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the category.
        ''' </summary>
        ''' <value>The category.</value>
        Public Property Category() As String
            Get
                Return Me._category
            End Get
            Set(value As String)
                SetValue(constFNCategory, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the responsible OU.
        ''' </summary>
        ''' <value>The responsible OU.</value>
        Public Property ResponsibleOU() As String
            Get
                Return Me._responsibleOU
            End Get
            Set(value As String)
                SetValue(constFNRespOU, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the responsible.
        ''' </summary>
        ''' <value>The responsible.</value>
        Public Property Responsible() As String
            Get
                Return Me._responsible
            End Get
            Set(value As String)
                SetValue(ConstFNResponsible, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the author.
        ''' </summary>
        ''' <value>The author.</value>
        Public Property Author() As String
            Get
                Return Me._author
            End Get
            Set(value As String)
                SetValue(ConstFNAuthor, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the posting.
        ''' </summary>
        ''' <value>The posting.</value>
        Public Property Posting() As String
            Get
                Return Me._posting
            End Get
            Set(value As String)
                SetValue(ConstFNPosting, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the title.
        ''' </summary>
        ''' <value>The title.</value>
        Public Property Title() As String
            Get
                Return Me._title
            End Get
            Set(value As String)
                SetValue(ConstFNTitle, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the type ID.
        ''' </summary>
        ''' <value>The type ID.</value>
        Public Property TypeID() As String
            Get
                Return Me._typeID
            End Get
            Set(value As String)
                SetValue(ConstFNTypeID, value)
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


#End Region

        ''' <summary>
        ''' retrieve  the configuration from store
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(uid As Long) As TrackLogEntry
            Return ormBusinessObject.RetrieveDataObject(Of TrackLogEntry)(pkArray:={uid})
        End Function

        ''' <summary>
        ''' handler for onCreating Event - generates unique primary key values
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub TRACKLogEntry_OnCreating(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnCreating
            Dim uid As Long? = e.Record.GetValue(constFNUID)
            Dim tag As String
            If Not uid.HasValue OrElse uid = 0 Then
                tag = constFNUID
                uid = Nothing
            End If
            Dim primarykey As Object() = {uid}
            If uid Is Nothing Then
                If e.DataObject.ObjectPrimaryContainerStore.CreateUniquePkValue(pkArray:=primarykey, tag:=tag) Then
                    e.Record.SetValue(ConstFNUID, primarykey(0))
                    e.Result = True
                    e.Proceed = True
                Else
                    CoreMessageHandler(message:="primary keys could not be created ?!", procedure:="TrackLogEntry.OnCreate", messagetype:=otCoreMessageType.InternalError)
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
        Public Overloads Shared Function Create(Optional uid As Long = 0) As TrackLogEntry
            Return ormBusinessObject.CreateDataObject(Of TrackLogEntry)(pkArray:={uid}, checkUnique:=True)
        End Function

        ''' <summary>
        ''' clones an value lot to a new updc
        ''' </summary>
        ''' <param name="pkarray"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Clone(Optional uid As Long? = Nothing) As TrackLogEntry
            If Not IsAlive(subname:="Clone") Then Return Nothing

            Dim primarykey As Object()
            primarykey = {uid}
            Return MyBase.Clone(Of TrackLogEntry)(primarykey)
        End Function

    End Class

    ''' <summary>
    '''Action Items
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleTracking, Version:=2, Release:=0, patch:=0, changeimplno:=8, _
          description:="Introducing Track Log Action Object")> _
    <ormObject(id:=ActionItem.ConstObjectID, version:=1, adddeletefieldbehavior:=True, usecache:=True, _
        primaryContainerID:=TrackLogEntry.ConstPrimaryTableID, buildRetrieveView:=True, RetrieveObjectFroMViewID:="VWACTIONITEMS", _
        modulename:=ConstModuleTracking, Description:="action items" _
        )> Public Class ActionItem
        Inherits TrackLogEntry

        ''' <summary>
        ''' Object ID
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstObjectID = "TLActionItem"

        ''' <summary>
        ''' Additional Table
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleRepository, Version:=ConstOTDBSchemaVersion, Release:=0, patch:=0, changeimplno:=12, _
        description:="added secondary table " & ConstActionTableID)> _
        <ormTableAttribute(version:=1)> Public Const ConstActionTableID = "TBLTRACKLOGACTIONS"

        ''' <summary>
        ''' Primary Keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=1, ContainerID:=ConstActionTableID, _
            XID:="ACTI1", title:="ID", description:="ID of the action item" _
           )> Public Const ConstFNID = "ID"

        ''' <summary>
        ''' Link the secondary Table to the primary via foreign key
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceobjectentry:=TrackLogEntry.ConstObjectID & "." & TrackLogEntry.ConstFNUID, ContainerID:=ConstActionTableID, _
                        foreignkeyProperties:={ForeignKeyProperty.PrimaryTableLink}, useforeignkey:=otForeignKeyImplementation.NativeDatabase _
                      )> Public Const ConstFNLINK = "UID"

        ''' <summary>
        ''' Create an action item
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(Optional id As Long? = Nothing, Optional uid As Long? = Nothing) As ActionItem
            Dim aRecord As New ormRecord
            With aRecord
                .SetValue(ConstActionTableID & "." & ConstFNID, id)
                .SetValue(ConstPrimaryTableID & "." & ConstFNUID, uid)
            End With
            Return ormBusinessObject.CreateDataObject(Of ActionItem)(record:=aRecord, checkUnique:=True)
        End Function

        ''' <summary>
        ''' Retrieve a Action Item by ActionItemID
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(id As Long) As ActionItem
            Dim aKey As New ormDatabaseKey(containerID:=ConstActionTableID, keyvalues:={id})
            Return ormBusinessObject.RetrieveDataObject(Of ActionItem)(key:=aKey)
        End Function


        ''' <summary>
        ''' Retrieve a Action Item by Track Log Entry UID
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RetrieveByEntryUID(uid As Long) As ActionItem
            Dim aKey As New ormDatabaseKey(containerID:=ConstPrimaryTableID, keyvalues:={uid})
            Return ormBusinessObject.RetrieveDataObject(Of ActionItem)(key:=aKey)
        End Function
    End Class

    ''' <summary>
    ''' Change Revision Items
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleTracking, Version:=2, Release:=0, patch:=0, changeimplno:=5, _
          description:="Introducing Track Log Change Entry  Objects")> _
   <ormObject(id:=ChangeRevisionItem.ConstObjectID, version:=1, adddeletefieldbehavior:=True, usecache:=True, _
       primaryContainerID:=TrackLogEntry.ConstPrimaryTableID, buildRetrieveView:=True, RetrieveObjectFroMViewID:="VWCHANGEREVISIONITEMS", _
       modulename:=ConstModuleTracking, Description:="change revision items" _
       )> Public Class ChangeRevisionItem
        Inherits TrackLogEntry

        ''' <summary>
        ''' Object ID
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstObjectID = "TLChangeRevisionItem"

        ''' <summary>
        ''' Additional Table
        ''' </summary>
        ''' <remarks></remarks>
        ''' 

        <ormTableAttribute(version:=1, id:="TBLCRITEMS")> Public Const ConstChangeRevisionTableID = "TBLTRACKLOGCHANGEREVISIONS"

        ''' <summary>
        ''' Primary Keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=1, ContainerID:=ConstChangeRevisionTableID, _
            XID:="CR1", category:="Primary Key", title:="UID", description:="UID of the change revision item" _
           )> Public Const ConstFNUID = "UID"

        ''' <summary>
        ''' Link the secondary Table to the primary via foreign key
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceobjectentry:=TrackLogEntry.ConstObjectID & "." & TrackLogEntry.ConstFNUID, ContainerID:=ConstChangeRevisionTableID, _
                        foreignkeyProperties:={ForeignKeyProperty.PrimaryTableLink}, useforeignkey:=otForeignKeyImplementation.NativeDatabase _
                      )> Public Const ConstFNLINK = "UID"
    End Class

    ''' <summary>
    ''' Verification Items
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleTracking, Version:=2, Release:=0, patch:=0, changeimplno:=4, _
          description:="Introducing Track Log Verify Entry  Objects")> _
   <ormObject(id:=VerifyItem.ConstObjectID, version:=1, adddeletefieldbehavior:=True, usecache:=True, _
       primaryContainerID:=TrackLogEntry.ConstPrimaryTableID, buildRetrieveView:=True, RetrieveObjectFroMViewID:="VWVERIFYITEMS", _
       modulename:=ConstModuleTracking, Description:="input specification verification items" _
       )> Public Class VerifyItem
        Inherits TrackLogEntry

        ''' <summary>
        ''' Object ID
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstObjectID = "TLVerifyItem"

        ''' <summary>
        ''' Additional Table
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleRepository, Version:=ConstOTDBSchemaVersion, Release:=0, patch:=0, changeimplno:=10, _
            description:="added secondary table " & ConstVerifyTableID)> _
        <ormTableAttribute(version:=1, id:="TBLVERIFYITEMS")> Public Const ConstVerifyTableID = "TBLTRACKLOGVERIFIERS"

        ''' <summary>
        ''' Primary Keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=1, ContainerID:=ConstVerifyTableID, _
            XID:="VERI1", title:="UID", description:="UID of the verification item" _
           )> Public Const ConstFNUID = "UID"

        ''' <summary>
        ''' Link the secondary Table to the primary via foreign key
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceobjectentry:=TrackLogEntry.ConstObjectID & "." & TrackLogEntry.ConstFNUID, ContainerID:=ConstVerifyTableID, _
                        foreignkeyProperties:={ForeignKeyProperty.PrimaryTableLink}, useforeignkey:=otForeignKeyImplementation.NativeDatabase _
                      )> Public Const ConstFNLINK = "UID"
    End Class
    ''' <summary>
    ''' Risk'n Oppertunities Items
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleTracking, Version:=2, Release:=0, patch:=0, changeimplno:=8, _
          description:="Introducing Risk'n Opportunity Object")> _
    <ormObject(id:=RNOItem.ConstObjectID, version:=1, adddeletefieldbehavior:=True, usecache:=True, _
        primaryContainerID:=TrackLogEntry.ConstPrimaryTableID, buildRetrieveView:=True, RetrieveObjectFroMViewID:="VWRISKNOPPITEMS", _
        modulename:=ConstModuleTracking, Description:="risk and opportunity items" _
        )> Public Class RNOItem
        Inherits TrackLogEntry

        ''' <summary>
        ''' ObjectID
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstObjectID = "TLRiskNOppItem"

        ''' <summary>
        ''' additional Table
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleRepository, Version:=ConstOTDBSchemaVersion, Release:=0, patch:=0, changeimplno:=13, _
       description:="added secondary table " & ConstRNOTableID)> _
       <ormTableAttribute(version:=1, id:="TBLRNOITEMS")> Public Const ConstRNOTableID = "TBLTRACKLOGRNOS"

        ''' <summary>
        ''' Primary Keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=1, ContainerID:=ConstRNOTableID, _
            XID:="RNOI1", title:="ID", description:="ID of the RNO item", _
            properties:={ObjectEntryProperty.Keyword}, validationPropertystrings:={ObjectValidationProperty.NotEmpty} _
           )> Public Const ConstFNID = "ID"

        ''' <summary>
        ''' Link the secondary Table to the primary via foreign key
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceobjectentry:=TrackLogEntry.ConstObjectID & "." & TrackLogEntry.ConstFNUID, ContainerID:=ConstRNOTableID, _
                        foreignkeyProperties:={ForeignKeyProperty.PrimaryTableLink}, useforeignkey:=otForeignKeyImplementation.NativeDatabase _
                      )> Public Const ConstFNLINK = "UID"
    End Class

    ''' <summary>
    ''' Track List of business objects Class 
    ''' </summary>
    ''' <remarks>
    '''  Design Principle:
    ''' 
    ''' 1. Create or Add or Update Items by TrackList Object
    ''' </remarks>
    ''' 
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleTracking, Version:=2, Release:=0, patch:=0, changeimplno:=6, _
          description:="Introducing Track List Object")> _
    <ormObject(id:=TrackList.ConstObjectID, description:="lists of trackable business objects", _
        modulename:=ConstModuleCommons, Version:=1, usecache:=True, adddomainbehavior:=True, adddeletefieldbehavior:=True, isbootstrap:=False)> _
    Public Class TrackList
        Inherits ormBusinessObject


        ''' <summary>
        ''' Object ID
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstObjectID = "TRACKLIST"

        ''' <summary>
        ''' primary Table
        ''' </summary>
        ''' <remarks></remarks>
        <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleRepository, Version:=ConstOTDBSchemaVersion, Release:=0, patch:=0, changeimplno:=9, _
        description:="added table " & ConstPrimaryTableID)> _
        <ormTableAttribute(version:=1, usecache:=True)> Public Const ConstPrimaryTableID As String = "TBLTRACKLISTS"

        ''' <summary>
        ''' Primary Key
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty},
          XID:="TL1", title:="ID of the list", description:="name of the status type")> Public Const ConstFNID = "ID"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2, _
                       defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        '*** Fields
        <ormObjectEntry(Datatype:=otDataType.Text, size:=255, _
        XID:="TL10", title:="description", description:="description of the status type")> Public Const ConstFNDescription = "DESC"



        '* Relations
        '* Members
        <ormRelation(cascadeOnDelete:=True, cascadeonUpdate:=True, FromEntries:={ConstFNID}, toEntries:={TrackListItem.constFNID}, _
            LinkObject:=GetType(TrackListItem))> Const ConstRelItems = "ITEMS"

        <ormObjectEntryMapping(Relationname:=ConstRelItems, infusemode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand, _
        keyentries:={TrackListItem.constFNID})> Private WithEvents _items As New ormRelationCollection(Of TrackListItem)(Me, {TrackListItem.constFNID})

        'fields
        <ormObjectEntryMapping(EntryName:=ConstFNID)> Private _id As String
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _desc As String




#Region "Properties"

        ''' <summary>
        ''' returns the ID / name of the tracking list
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ID As String
            Get
                Return _id
            End Get
        End Property

        ''' <summary>
        ''' returns the description of the tracking list
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description() As String
            Get
                Description = _desc
            End Get
            Set(ByVal avalue As String)
                SetValue(entryname:=ConstFNDescription, value:=avalue)
            End Set
        End Property

        ''' <summary>
        ''' returns the collection of items 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Items As ormRelationCollection(Of TrackListItem)
            Get
                Return _items
            End Get
        End Property

#End Region

        ''' <summary>
        ''' Handler for the OnAdded event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>

        Private Sub ValuesCollection_OnAdded(sender As Object, e As Database.ormRelationCollection(Of TrackListItem).EventArgs) Handles _items.OnAdded
            If Not _items.Contains(e.Dataobject) Then
                _items.Add(e.Dataobject)
            End If
        End Sub
        ''' <summary>
        ''' Returns a collection of all objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function All() As List(Of Group)
            Return ormBusinessObject.AllDataObject(Of Group)(orderby:=ConstFNID)
        End Function

        ''' <summary>
        ''' Retrieve a User Definition
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal id As String, Optional domainid As String = Nothing, Optional forcereload As Boolean = False) As TrackList
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Return RetrieveDataObject(Of TrackList)(pkArray:={id, domainid}, domainID:=domainid, forceReload:=forcereload)
        End Function

        ''' <summary>
        ''' Create persistency for this object
        ''' </summary>
        ''' <param name="groupname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal id As String, Optional domainid As String = Nothing) As TrackList
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey() As Object = {id, domainid}
            Return ormBusinessObject.CreateDataObject(Of TrackList)(primarykey, domainID:=domainid, checkUnique:=True)
        End Function

    End Class
    ''' <summary>
    ''' List of Tracking Items
    ''' </summary>
    ''' <remarks>
    ''' Design Principle:
    ''' 
    ''' 1. Create or Add or Update Items by TrackList Object
    ''' </remarks>
    ''' 
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleTracking, Version:=2, Release:=0, patch:=0, changeimplno:=5, _
          description:="Modify Track List Item Objects -> belong to Track List Object")> _
    <ormObject(id:=TrackListItem.ConstObjectID, version:=1, adddeletefieldbehavior:=True, adddomainbehavior:=True, usecache:=True, _
        modulename:=ConstModuleTracking, Description:="member of tracking lists" _
        )> Public Class TrackListItem
        Inherits ormBusinessObject

        ''' <summary>
        ''' ObjectID
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstObjectID = "TrackListItem"

        ''' <summary>
        ''' Table
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=1)> Public Const ConstPrimaryTableID = "TBLTRACKLISTITEMS"

        ''' <summary>
        ''' Index
        ''' </summary>
        ''' <remarks></remarks>
        <ormIndex(columnname1:=constFNID, columnname2:=constFNOrdinal)> Public Const constIndexOrder = "orderby"
        <ormIndex(columnname1:=constFNID)> Public Const constIndexList = "lists"

        ''' <summary>
        ''' primary keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(XID:="TLI1", title:="List ID", description:="name of the tracking item list", _
            referenceObjectEntry:=TrackList.ConstObjectID & "." & TrackList.ConstFNID, _
            PrimaryKeyOrdinal:=1)> Public Const constFNID = "listid"

        <ormObjectEntry(XID:="TLI2", title:="List Pos", description:="entry number in the tracking item list", _
            lowerrange:=0, _
            Datatype:=otDataType.Long, PrimaryKeyOrdinal:=2)> Public Const constFNPos = "posno"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=3 _
       , useforeignkey:=otForeignKeyImplementation.NativeDatabase, defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        ''' <summary>
        ''' Foreign Key to Status Type keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormForeignKey(entrynames:={constFNID, ConstFNDomainID}, _
            foreignkeyreferences:={TrackList.ConstObjectID & "." & TrackList.ConstFNID, _
                                   TrackList.ConstObjectID & "." & TrackList.ConstFNDomainID}, _
            useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const constFKID = "FKID"

        ''' <summary>
        ''' Object Entries
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectentry:=Parts.Part.ConstObjectID & "." & Parts.Part.ConstFNPartID, _
            XID:="TLI3", description:="part id of the item to be tracked", isnullable:=True, _
           isnullable:=True, useforeignkey:=otForeignKeyImplementation.ORM)> Public Const constFNPartid = Parts.Part.ConstFNPartID

        <ormObjectEntry(XID:="TLI4", title:="order", defaultvalue:=0, dbdefaultvalue:="0", description:="ordinal in the list to be sorted", _
           Datatype:=otDataType.Long)> Public Const constFNOrdinal = "order"

        <ormObjectEntry(XID:="TLI5", title:="matchcode", description:="matchcode for items", isnullable:=True, _
           Datatype:=otDataType.Text, size:=100)> Public Const constFNMatchCode = "MATCHCODE"

        <ormObjectEntry(referenceObjectentry:=Deliverables.Deliverable.ConstObjectID & "." & Deliverables.Deliverable.ConstFNDLVUID, _
                XID:="TLI7", description:="UID of the deliverable to be tracked", isnullable:=True, _
          isnullable:=True, useforeignkey:=otForeignKeyImplementation.ORM)> Public Const constFNDLVUID = Deliverables.Deliverable.ConstFNDLVUID

        <ormObjectEntry(XID:="TLI6", title:="Comments", description:="comment for the item", isnullable:=True, _
         Datatype:=otDataType.Memo)> Public Const constFNComment = "cmt"



        ''' <summary>
        ''' Mappings
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=constFNID)> Private _listid As String = String.Empty
        <ormObjectEntryMapping(EntryName:=constFNPos)> Private _posno As Long
        <ormObjectEntryMapping(EntryName:=constFNPartid)> Private _pnid As String
        <ormObjectEntryMapping(EntryName:=constFNOrdinal)> Private _ordinal As Long
        <ormObjectEntryMapping(EntryName:=constFNComment)> Private _cmt As String
        <ormObjectEntryMapping(EntryName:=constFNMatchCode)> Private _matchcode As String = String.Empty
        <ormObjectEntryMapping(EntryName:=constFNDLVUID)> Private _dlvuid As Long?

        Private _TrackList As TrackList 'cached backlink
#Region "Properties"
        ''' <summary>
        ''' gets the id of the tracking list
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Listid() As String
            Get
                Return _listid
            End Get

        End Property
        ''' <summary>
        ''' gets the position number in the list
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Posno() As Long
            Get
                Return _posno
            End Get

        End Property

        ''' <summary>
        ''' gets or set the part id to be tracked - might be null / nothing
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PartID() As String
            Get
                Return _pnid
            End Get
            Set(value As String)
                SetValue(constFNPartid, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets some comments and textfield
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Comment() As String
            Get
                Return _cmt
            End Get
            Set(value As String)
                SetValue(constFNComment, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the matchcode
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Matchcode() As String
            Get
                Return _matchcode
            End Get
            Set(value As String)
                SetValue(constFNMatchCode, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the ordinal in the list
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Ordinal() As Long
            Get
                Return _ordinal

            End Get
            Set(value As Long)
                SetValue(constFNOrdinal, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the deliverable uid to be tracked - might be nothing / nullable
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DlvUid() As Long?
            Get
                Return _dlvuid
            End Get
            Set(value As Long?)
                SetValue(constFNDLVUID, value)
            End Set
        End Property

#End Region
        ''' <summary>
        ''' Handles OnCreating 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub TrackListItem_OnCreating(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreating
            Dim my As ValueEntry = TryCast(e.DataObject, ValueEntry)

            If my IsNot Nothing Then
                Dim listid As String = e.Record.GetValue(constFNID)
                If listid Is Nothing Then
                    CoreMessageHandler(message:="Track list id does not exist", procedure:="TrackListItem.OnCreating", _
                                       messagetype:=otCoreMessageType.ApplicationError, _
                                       argument:=my.ListID)
                    e.AbortOperation = True
                    Return
                End If
                ''' even if it is early to retrieve the value list and set it (since this might disposed since we have not run through checkuniqueness and cache)
                ''' we need to check on the object here
                _TrackList = TrackList.Retrieve(id:=listid)
                If _TrackList Is Nothing Then
                    CoreMessageHandler(message:="Track list id  does not exist", procedure:="TrackListItem.OnCreated", _
                                       messagetype:=otCoreMessageType.ApplicationError, _
                                       argument:=listid)
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
        Public Sub TrackListItem_OnCreated(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreated
            Dim my As StatusItem = TryCast(e.DataObject, StatusItem)

            If my IsNot Nothing Then
                If _TrackList Is Nothing Then
                    _TrackList = TrackList.Retrieve(id:=my.TypeID)
                    If _TrackList Is Nothing Then
                        CoreMessageHandler(message:="Track list id  does not exist", procedure:="TrackListItem.OnCreated", _
                                          messagetype:=otCoreMessageType.ApplicationError, _
                                           argument:=my.TypeID)
                        e.AbortOperation = True
                        Return
                    End If
                End If
            End If

        End Sub


        ''' <summary>
        ''' Infuse the data object by record
        ''' </summary>
        ''' <param name="aRecord"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Sub TrackListItem_OnInfused(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnInfused
            Dim my As TrackListItem = TryCast(e.DataObject, TrackListItem)

            Try
                ''' infuse is called on create as well as on retrieve / inject 
                ''' only on the create case we need to add to the TrackListItem otherwise
                ''' TrackList will load the item
                ''' or the TrackListItem will stand alone
                If my IsNot Nothing AndAlso e.Infusemode = otInfuseMode.OnCreate AndAlso _TrackList IsNot Nothing Then
                    _TrackList.Items.Add(my)
                End If

            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="TrackListItem.Infuse")
            End Try


        End Sub
        ''' <summary>
        ''' Retrieve a trackitem from the data store
        ''' </summary>
        ''' <param name="listid"></param>
        ''' <param name="posno"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Retrieve(ByVal listid As String, ByVal posno As Long) As TrackListItem
            Dim primarykey() As Object = {listid, posno}
            Return ormBusinessObject.RetrieveDataObject(Of TrackListItem)(primarykey)
        End Function


        ''' <summary>
        ''' create a persistable track list item
        ''' </summary>
        ''' <param name="listid"></param>
        ''' <param name="posno"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal listid As String, ByVal posno As Long) As TrackListItem
            Dim primarykey() As Object = {listid, posno}
            Return ormBusinessObject.CreateDataObject(Of TrackListItem)(primarykey, checkUnique:=True)
        End Function

        ''' <summary>
        ''' get the items by list
        ''' </summary>
        ''' <param name="listid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTrackItemsList(listid As String) As Collection
            Dim aTable As iormRelationalTableStore
            Dim aRecordCollection As List(Of ormRecord)
            Dim aCollection As New Collection
            Dim primarykey() As Object = {listid}
            ' set the primaryKey
            aTable = GetPrimaryTableStore(ConstPrimaryTableID)
            aRecordCollection = aTable.GetRecordsByIndex(indexname:=constIndexOrder, keyArray:=primarykey)

            If Not aRecordCollection Is Nothing AndAlso aRecordCollection.Count > 0 Then
                ' records read
                For Each aRecord In aRecordCollection
                    Dim anEntry As New TrackListItem
                    If InfuseDataObject(record:=aRecord, dataobject:=anEntry) Then
                        aCollection.Add(Item:=anEntry)
                    End If
                Next aRecord
            End If
            Return aCollection

        End Function

        ''' <summary>
        ''' retrieve a collection of all Items
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function All() As List(Of TrackListItem)
            Return ormBusinessObject.AllDataObject(Of TrackListItem)(ID:="all")
        End Function

    End Class
End Namespace


