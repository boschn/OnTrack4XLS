REM ***********************************************************************************************************************************************
REM *********** BUSINESS OBJECTs: DELIVERABLE LINKS Classes for On Track Database Backend Library
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2013-08-08
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************
Option Explicit On

Imports System.Collections.Generic

Imports OnTrack
Imports OnTrack.Database
Imports OnTrack.Parts
Imports OnTrack.IFM
Imports OnTrack.Scheduling
Imports OnTrack.XChange
Imports OnTrack.Calendar
Imports OnTrack.Commons
Imports OnTrack.ObjectProperties
Imports OnTrack.Core

Namespace OnTrack.Deliverables

    ''' <summary>
    ''' Definition class for LinkTypes
    ''' </summary>
    ''' <remarks></remarks>
    '''     
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleDeliverables, Version:=2, Release:=0, patch:=0, changeimplno:=2, _
            description:="Introducing Deliverable Link types")> _
    <ormObject(id:=LinkType.ConstObjectID, description:="type definition of a deliverable link. Defines default setting and some general logic.", _
        modulename:=ConstModuleDeliverables, Version:=1, useCache:=True, adddeletefieldbehavior:=True, addDomainBehavior:=True)> _
    Public Class LinkType
        Inherits ormBusinessObject

        Public Const ConstObjectID = "LinkType"
        '** Table
        <ormTableAttribute(version:=1, usecache:=True)> Public Const ConstPrimaryTableID = "tblDefDeliverableLinkTypes"

        '** indexes
        <ormIndex(columnName1:=ConstFNDomainID, columnname2:=constFNTypeID, columnname3:=ConstFNIsDeleted)> Public Const constIndexDomain = "indDomains"

        ''' <summary>
        ''' keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertystrings:={ObjectValidationProperty.NotEmpty}, _
           title:="Type", description:="type of the deliverable link", XID:="DLVLT1")> Public Const constFNTypeID = "ID"

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
         title:="Description", description:="description of the deliverable link type", XID:="DLVTL3")> Public Const constFNDescription = "DESC"

        <ormObjectEntry(Datatype:=otDataType.Memo, isnullable:=True, _
            title:="comment", description:="comments of the deliverable link type", XID:="DLVLT10")> Public Const constFNComment = "CMT"

        <ormObjectEntry(referenceObjectEntry:=Commons.StatusType.ConstObjectID & "." & Commons.StatusType.ConstFNID, _
            useforeignkey:=otForeignKeyImplementation.ORM, _
            foreignkeyProperties:={ForeignKeyProperty.OnDelete & "(" & ForeignKeyActionProperty.SetNull & ")", _
                                   ForeignKeyProperty.OnUpdate & "(" & ForeignKeyActionProperty.Cascade & ")"}, _
            isnullable:=True, _
            Title:="Status Type", description:="type of the status", _
            XID:="DLVLT20", isnullable:=True)> Public Const ConstFNLinkStatusType = "STATUSTYPE"

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertystrings:={ObjectValidationProperty.NotEmpty}, _
            title:="OPRule", description:="internal Operation / Method to call on Status Rules", XID:="DLVTL30")> _
        Public Const constFNRuleOperation = "RuleOperation"

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertystrings:={ObjectValidationProperty.NotEmpty}, _
            title:="RulezSet", description:="Rulez Ruleset to call on status rules", XID:="DLVTL31")> Public Const constFNRulezSet = "RuleSet"

        '*** Mapping
        <ormObjectEntryMapping(EntryName:=constFNTypeID)> Private _typeid As String = String.Empty
        <ormObjectEntryMapping(EntryName:=constFNDescription)> Private _description As String
        <ormObjectEntryMapping(EntryName:=constFNComment)> Private _comment As String
        <ormObjectEntryMapping(EntryName:=ConstFNLinkStatusType)> Private _statustype As String
        <ormObjectEntryMapping(EntryName:=constFNRuleOperation)> Private _ruleOperation As String

#Region "Properties"

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
        ''' creates with this object a new persistable Def workspaceID
        ''' </summary>
        ''' <param name="workspaceID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal typeid As String, Optional ByVal domainid As String = Nothing) As LinkType
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey() As Object = {typeid, domainid}
            Return CreateDataObject(Of LinkType)(pkArray:=primarykey, domainID:=domainid, checkUnique:=True)
        End Function


        ''' <summary>
        ''' Retrieve a deliverable Type object
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal typeid As String, Optional ByVal domainid As String = Nothing, Optional forcereload As Boolean = False) As LinkType
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim pkarray() As Object = {typeid, domainid}
            Return RetrieveDataObject(Of LinkType)(pkArray:=pkarray, forceReload:=forcereload)
        End Function

#Region "static routines"
        ''' <summary>
        ''' returns a List(of Delivertype) for the DomainID
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function All(Optional domainid As String = Nothing) As List(Of LinkType)
            Return ormBusinessObject.AllDataObject(Of LinkType)(domainid:=domainid)
        End Function
#End Region
    End Class

    ''' <summary>
    ''' describes a temporal deliverable link to a deliverable from other deliverables (inbound or pointing to the deliverable)
    ''' </summary>
    ''' <remarks>
    ''' Design Requirements
    ''' 
    ''' 1. A Current Link points to the current link to be used in the alive (not changeable but active) and work (changeable not active) manner
    ''' 2. Links are pointing from to
    ''' 3. A Current Link has the LINK UID as primary key and the from / to by additional indices
    ''' 4. If a CurrentLink is deleted or updated by LUID then update/delete the Links too
    ''' </remarks>
    ''' 
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleDeliverables, Version:=2, Release:=0, patch:=0, changeimplno:=1, _
            description:="Introducing Deliverable Links")> _
    <ormObject(id:=CurrentLink.ConstObjectID, description:="describes a current link from other objects", _
        modulename:=ConstModuleDeliverables, Version:=1, useCache:=True, adddeletefieldbehavior:=True)> _
    Public Class CurrentLink
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable


        Public Const ConstObjectID = "CURRENTLINK"
        '** Schema Table
        <ormTableAttribute(Version:=1)> Public Const ConstPrimaryTableID = "TBLCURRDLVLINKS"

        ''' <summary>
        ''' Index
        ''' </summary>
        ''' <remarks></remarks>
        <ormIndex(columnname1:=ConstFNTOUID, columnName2:=constFNLinkTypeID, columnname3:=ConstFNLinkUid)> Public Const constIndexFrom = "indfrom"
        <ormIndex(columnName1:=constFNLinkTypeID, columnname2:=ConstFNTOUID, columnname3:=ConstFNLinkUid)> Public Const constIndexTypeto = "indtypeto"

        ''' <summary>
        ''' Primary Keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(datatype:=otDataType.Long, defaultvalue:=0, _
                        lowerrange:=0, PrimaryKeyOrdinal:=1, XID:="DLVCL1", title:="link uid", description:="uid of the deliverable link")> _
        Public Const ConstFNLinkUid = "LUID"

        ''' <summary>
        ''' columns
        ''' </summary>
        ''' <remarks></remarks>
        '''  
        <ormObjectEntry(referenceObjectEntry:=Deliverable.ConstObjectID & "." & Deliverable.ConstFNDLVUID, _
                      xid:="DLVLC2", useforeignkey:=otForeignKeyImplementation.ORM)> Public Const ConstFNFROMUID = "FROMDLVUID"

        <ormObjectEntry(referenceObjectEntry:=Deliverable.ConstObjectID & "." & Deliverable.ConstFNDLVUID, _
                        xid:="DLVCL3", useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const ConstFNTOUID = "TODLVUID"

        <ormObjectEntry(referenceobjectentry:=LinkType.ConstObjectID & "." & LinkType.constFNTypeID, _
            title:="Type", description:="type of the deliverable link", XID:="DLVCL4", _
            LookupPropertyStrings:={LookupProperty.UseAttributeReference}, validationPropertyStrings:={ObjectValidationProperty.UseLookup} _
         )> Public Const constFNLinkTypeID = "TYPEID"

        ''' <summary>
        ''' Foreign Key to Link Type
        ''' </summary>
        ''' <remarks></remarks>
        <ormForeignKey(entrynames:={constFNLinkTypeID, ConstFNDomainID}, _
            foreignkeyreferences:={LinkType.ConstObjectID & "." & LinkType.constFNTypeID, _
                                   LinkType.ConstObjectID & "." & LinkType.ConstFNDomainID}, _
            foreignkeyproperties:={ForeignKeyProperty.OnDelete & "(" & ForeignKeyActionProperty.SetDefault & ")", _
                                   ForeignKeyProperty.OnUpdate & "(" & ForeignKeyActionProperty.Cascade & ")"}, _
            useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const constFKLinkType = "FKLinkType"


        <ormObjectEntry(Datatype:=otDataType.Long, isnullable:=True, _
        title:="working counter", description:="update number of the working target", XID:="DLVCL10")> Public Const ConstFNWorkUPDC = "workupdc"

        <ormObjectEntry(Datatype:=otDataType.Long, isnullable:=True, _
         title:="Alive Counter", description:="update number of the alive target", XID:="DLVCL11")> Public Const ConstFNAliveUPDC = "aliveupdc"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0", _
          title:="is dynamic", description:="is the link dynamically created", XID:="DLVCL12")> Public Const ConstFNIsDynamic = "ISDYNAMIC"


        ' change FK Action since we have the workspace as FK (leads also to domians)
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, _
              useforeignkey:=otForeignKeyImplementation.None)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        '** mappings
        <ormObjectEntryMapping(EntryName:=ConstFNLinkUid)> Private _linkuid As Long
        <ormObjectEntryMapping(EntryName:=ConstFNTOUID)> Private _touid As Long
        <ormObjectEntryMapping(EntryName:=ConstFNFROMUID)> Private _fromuid As Long
        <ormObjectEntryMapping(EntryName:=constFNLinkTypeID)> Private _typeid As String

        <ormObjectEntryMapping(EntryName:=ConstFNWorkUPDC)> Private _workupdc As Long?
        <ormObjectEntryMapping(EntryName:=ConstFNAliveUPDC)> Private _aliveupdc As Long?
        <ormObjectEntryMapping(EntryName:=ConstFNIsDynamic)> Private _isdynamic As Boolean


        ''' <summary>
        ''' Define the constants for accessing the compounds
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstOPGetLink = "GETLINK"
        Public Const ConstOPSetLink = "SETLINK"

        ''' <summary>
        ''' Relation to alive Link - will be resolved by events
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkObject:=GetType(Link), ToPrimaryKeys:={ConstFNLinkUid, ConstFNAliveUPDC}, _
                     cascadeonCreate:=True, cascadeOnDelete:=True, cascadeOnUpdate:=True)> _
        Public Const ConstRAliveLink = "REL_ALIVELINK"

        <ormObjectEntryMapping(relationName:=ConstRAliveLink, infusemode:=otInfuseMode.OnCreate Or otInfuseMode.OnDemand Or otInfuseMode.OnInject)> _
        Private _alivelink As Link

        ''' <summary>
        ''' Relation to working Link - will be resolved by events
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkObject:=GetType(Link), createObjectifnotretrieved:=True, _
                    ToPrimaryKeys:={ConstFNLinkUid, ConstFNWorkUPDC}, _
                     cascadeonCreate:=True, cascadeOnDelete:=True, cascadeOnUpdate:=True)> _
        Public Const ConstRWorkLink = "REL_WORKLINK"

        <ormObjectEntryMapping(relationName:=ConstRWorkLink, infusemode:=otInfuseMode.OnCreate Or otInfuseMode.OnDemand Or otInfuseMode.OnInject)> _
        Private _workinglink As Link

        '' <summary>
        ''' Relation to Type
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkObject:=GetType(DeliverableType), toprimaryKeys:={constFNLinkTypeID, ConstFNDomainID}, _
                     cascadeonCreate:=False, cascadeOnDelete:=False, cascadeOnUpdate:=False)> _
        Public Const ConstRType = "RELType"

        <ormObjectEntryMapping(relationName:=ConstRType, infusemode:=otInfuseMode.OnDemand)> Private _Type As LinkType
        ''' <summary>
        ''' dynamic data
        ''' </summary>
        ''' <remarks></remarks>


#Region "Properties"
        ''' <summary>
        ''' retrieves a type object of this Deliverable link
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Type As LinkType
            Get
                If Not Me.IsAlive(subname:="Type") Then Return Nothing
                Me.InfuseRelation(ConstRType)
                Return _Type

            End Get
        End Property

        ''' <summary>
        ''' Gets the link object
        ''' </summary>
        ''' <value>The target.</value>
        Public ReadOnly Property Link As Link
            Get
                If Me.WorkingLinkUpdc.HasValue Then
                    Return Me.WorkingLink
                ElseIf Me.AliveLinkUpdc.HasValue Then
                    Return Me.AliveLink
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the working link object
        ''' </summary>
        ''' <value>The target.</value>
        Public ReadOnly Property WorkingLink As Link
            Get
                If GetRelationStatus(ConstRWorkLink) = ormRelationManager.RelationStatus.Unloaded Then InfuseRelation(ConstRWorkLink)
                Return Me._workinglink
            End Get
        End Property


        ''' <summary>
        ''' Gets the alive target object
        ''' </summary>
        ''' <value>The target.</value>
        Public ReadOnly Property AliveLink() As Link
            Get
                If GetRelationStatus(ConstRAliveLink) = ormRelationManager.RelationStatus.Unloaded Then InfuseRelation(ConstRAliveLink)
                Return Me._alivelink
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the isdynamic.
        ''' </summary>
        ''' <value>The isdynamic.</value>
        Public Property Isdynamic() As Boolean
            Get
                Return Me._isdynamic
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsDynamic, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the fromuid.
        ''' </summary>
        ''' <value>The fromuid.</value>
        Public Property Fromuid() As Long
            Get
                Return Me._fromuid
            End Get
            Set(value As Long)
                SetValue(ConstFNFROMUID, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the aliveupdc.
        ''' </summary>
        ''' <value>The aliveupdc.</value>
        Public Property AliveLinkUpdc() As Long?
            Get
                Return Me._aliveupdc
            End Get
            Set(value As Long?)
                SetValue(ConstFNAliveUPDC, value)
                Me._aliveupdc = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the workupdc.
        ''' </summary>
        ''' <value>The workupdc.</value>
        Public Property WorkingLinkUpdc() As Long?
            Get
                Return Me._workupdc
            End Get
            Set(value As Long?)
                Me._workupdc = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the typeid.
        ''' </summary>
        ''' <value>The typeid.</value>
        Public Property Typeid() As String
            Get
                Return Me._typeid
            End Get
            Set(value As String)
                SetValue(constFNLinkTypeID, value)
            End Set
        End Property

        ''' <summary>
        ''' returns the TO UID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ToUID() As Long
            Get
                Return _touid
            End Get
        End Property
        ''' <summary>
        ''' returns the From UID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property LinkUID() As Long
            Get
                Return _linkuid
            End Get
        End Property


#End Region

        ''' <summary>
        ''' handles the relationCreateNeeded Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub Link_OnRelationCreateNeeded(sender As Object, e As ormDataObjectRelationEventArgs) Handles Me.OnRelationCreateNeeded
            If Not Me.IsAlive(subname:="Link_OnRelationCreateNeeded") Then Return

            If e.RelationID = ConstRWorkLink Then
                ''' always gives the current workspace
                Dim aLink As Link
                If Me.WorkingLinkUpdc.HasValue AndAlso Me.WorkingLinkUpdc <> 0 Then
                    aLink = Deliverables.Link.Retrieve(uid:=Me.LinkUID, updc:=Me.WorkingLinkUpdc)
                Else
                    aLink = Deliverables.Link.Create(uid:=Me.LinkUID, fromUID:=Me.Fromuid, toUID:=Me.ToUID)
                End If
                If aLink IsNot Nothing Then
                    Me.WorkingLinkUpdc = aLink.LinkUPDC

                    e.RelationObjects.Add(aLink)
                    e.Finished = True
                End If

            End If

        End Sub
        ''' <summary>
        ''' retrieve  the configuration from store
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(linkUID As Long) As CurrentLink
            Return ormBusinessObject.RetrieveDataObject(Of CurrentLink)(pkArray:={linkUID})
        End Function

        ''' <summary>
        ''' handler for onCreating Event - generates unique primary key values
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub CurrentLink_OnCreating(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnCreating
            Dim uid As Long? = e.Record.GetValue(ConstFNLinkUid)
            Dim tag As String
            If Not uid.HasValue OrElse uid = 0 Then
                tag = ConstFNLinkUid
                uid = Nothing
            End If
            Dim primarykey As Object() = {uid}
            If uid Is Nothing Then
                If e.DataObject.ObjectPrimaryContainerStore.CreateUniquePkValue(pkArray:=primarykey, tag:=tag) Then
                    e.Record.SetValue(ConstFNLinkUid, primarykey(0))
                    e.Result = True
                    e.Proceed = True
                Else
                    CoreMessageHandler(message:="primary keys couldnot be created ?!", procedure:="CurrentLink.OnCreate", messagetype:=otCoreMessageType.InternalError)
                End If
            End If
            '''
            ''' TODO: Check double entries of fromUID -> toUID Pairs
            ''' 
        End Sub
        ''' <summary>
        ''' creates a persistable Link
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(Optional linkUID As Long = 0, _
                                                Optional linkType As String = Nothing, _
                                                Optional fromUID As Long? = Nothing, _
                                                Optional toUID As Long? = Nothing) As CurrentLink
            Dim aRecord As New ormRecord
            With aRecord
                .SetValue(ConstFNLinkUid, linkUID)
                If Not String.IsNullOrWhiteSpace(linkType) Then .SetValue(constFNLinkTypeID, linkType)
                If toUID.HasValue Then .SetValue(ConstFNTOUID, toUID.Value)
                If fromUID.HasValue Then .SetValue(ConstFNFROMUID, fromUID.Value)
            End With
            Return ormBusinessObject.CreateDataObject(Of CurrentLink)(aRecord, checkUnique:=True)
        End Function



        ''' <summary>
        ''' publish is a persist with history and baseline integrated functions. It sets the working edition as the alive edition
        ''' </summary>
        ''' <param name="msglog"></param>
        ''' <param name="timestamp"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Publish(Optional ByRef msglog As BusinessObjectMessageLog = Nothing, _
                                Optional ByVal timestamp As Date? = Nothing) As Boolean
            Dim isProcessable As Boolean = True

            Dim aWorklingLink As Link = _workinglink

            '* init
            If Not Me.IsAlive(subname:="Publish") Then Return False


            ' TIMESTAMP
            If timestamp Is Nothing Then timestamp = Date.Now


            '** if any of the milestones is changed
            '**
            isProcessable = True

            '** condition
            If aWorklingLink IsNot Nothing AndAlso aWorklingLink.IsChanged Then

                If isProcessable Then
                    Dim publishflag As Boolean = True
                    ''' do some checkings here
                    ''' 

                    ''' change over THE working schedule to alive scheudle
                    '''
                    If publishflag Then
                        Me.AliveLinkUpdc = aWorklingLink.LinkUPDC
                        _alivelink = aWorklingLink

                        Me.WorkingLinkUpdc = Nothing
                        '' cannot generate an new updc on a created edition (getmax will not work on unpersisted objects)
                        If _alivelink.IsCreated Then
                            _workinglink = aWorklingLink.Clone(uid:=_alivelink.LinkUID, updc:=_alivelink.LinkUPDC + 1)
                        Else
                            _workinglink = aWorklingLink.Clone()
                        End If
                        '* should be cloned but to make sure
                        Me.WorkingLink.DomainID = aWorklingLink.DomainID
                        '** link
                        Me.WorkingLinkUpdc = _workinglink.LinkUPDC
                    End If


                    ''' save the workspace schedule itself and the
                    ''' related objects
                    Return MyBase.Persist(timestamp)

                Else
                    Throw New NotImplementedException("CurrentLink.Publish not processable")

                End If
            ElseIf Me.IsAlive("Publish") Then
                '**** save without Milestone checking
                Return MyBase.Persist(timestamp:=timestamp)
            Else
                '** nothing changed
                '***
                Return False
            End If

            Return True
        End Function



        ''' <summary>
        ''' Persist with checking on publish
        ''' </summary>
        ''' <param name="timestamp"></param>
        ''' <param name="doFeedRecord"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Persist(Optional timestamp As DateTime? = Nothing, Optional doFeedRecord As Boolean = True) As Boolean Implements iormRelationalPersistable.Persist
            If Not Me.IsAlive("Persist") Then Return False
            If Not timestamp.HasValue OrElse timestamp = constNullDate Then timestamp = DateTime.Now
            Dim autopublish As Boolean = True

            If autopublish Then
                Return Publish(timestamp:=timestamp)
            Else
                Return MyBase.Persist(timestamp:=timestamp, doFeedRecord:=doFeedRecord)
            End If
        End Function


    End Class

    ''' <summary>
    ''' describes a deliverable link to a deliverable from other deliverables (inbound or pointing to the deliverable)
    ''' </summary>
    ''' <remarks>
    ''' Design Requirements
    ''' 
    ''' 1. A Link can be described by a describing Deliverable and be therefore dynamic
    ''' 2. A Link has an own unique UID (as primary key) to attach properties to it
    ''' 3. A Link has an UPDC to make it temporal -> see currentlink object for the active link in time
    ''' 4. A Link has a status on its own
    ''' 5. A Link has a type and therefore multiple links bettween the same deliverable different in types might exists
    ''' 6. If a CurrentLink is deleted or updated by LUID then update/delete the Links too
    ''' </remarks>
    ''' 
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleDeliverables, Version:=2, Release:=0, patch:=0, changeimplno:=4, _
           description:="Introducing Deliverable link objects")> _
   <ormObject(id:=Link.ConstObjectID, description:="describes a link from other deliverables", _
       modulename:=ConstModuleDeliverables, Version:=1, useCache:=True, adddeletefieldbehavior:=True)> _
    Public Class Link
        Inherits ormBusinessObject
        Implements iormCloneable(Of Link)


        Public Const ConstObjectID = "DELIVERABLELINK"

        ''' <summary>
        ''' Table
        ''' </summary>
        ''' <remarks></remarks>
        <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleRepository, Version:=ConstOTDBSchemaVersion, Release:=0, patch:=0, changeimplno:=7, _
          description:="added table " & ConstPrimaryTableID)> _
       <ormTableAttribute(Version:=1)> Public Const ConstPrimaryTableID = "TBLDELIVERABLELINKS"

        ''' <summary>
        ''' Indices
        ''' </summary>
        ''' <remarks></remarks>
        <ormIndex(columnname1:=ConstFNFROMUID, columnname2:=ConstFNTOUID, columnName3:=constFNLinkTypeID)> Public Const constIndexFrom = "indfrom"
        <ormIndex(columnName1:=constFNLinkTypeID, columnname2:=ConstFNTOUID, columnname3:=ConstFNFROMUID)> Public Const constIndexTypeto = "indtypeto"
        <ormIndex(columnName1:=constFNLinkTypeID, columnname2:=ConstFNFROMUID, columnname3:=ConstFNTOUID)> Public Const constIndexTypeFrom = "indtypefrom"
        <ormIndex(columnname1:=ConstFNDESCUID, columnname2:=ConstFNTOUID, columnname3:=ConstFNFROMUID, columnName4:=constFNLinkTypeID)> Public Const constIndexDescUID = "indDescUID"


        ''' <summary>
        ''' Primary Keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=CurrentLink.ConstObjectID & "." & CurrentLink.ConstFNLinkUid, PrimaryKeyOrdinal:=1, _
          lowerrange:=0, XID:="DLVL1")> Public Const ConstFNLinkUid = "LUID"

        <ormObjectEntry(Datatype:=otDataType.Long, title:="update count", Description:="Update count of the link", PrimaryKeyOrdinal:=2, _
            lowerrange:=0, XID:="DLVL2")> Public Const ConstFnLinkUpdc = "VERSION"

        ''' <summary>
        ''' Foreign Key to Current Link
        ''' </summary>
        ''' <remarks></remarks>
        <ormForeignKey(entrynames:={ConstFNLinkUid}, _
            foreignkeyreferences:={Deliverables.CurrentLink.ConstObjectID & "." & Deliverables.CurrentLink.ConstFNLinkUid}, _
            useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const ConstFKCurrentLink = "FKCurrentLink"

        ''' <summary>
        ''' other Columns
        ''' </summary>
        ''' <remarks></remarks>


        <ormObjectEntry(referenceObjectEntry:=Deliverable.ConstObjectID & "." & Deliverable.ConstFNDLVUID, _
                      xid:="DLVL3", useforeignkey:=otForeignKeyImplementation.ORM)> Public Const ConstFNFROMUID = CurrentLink.ConstFNFROMUID

        <ormObjectEntry(referenceObjectEntry:=Deliverable.ConstObjectID & "." & Deliverable.ConstFNDLVUID, _
                        xid:="DLVL4", useforeignkey:=otForeignKeyImplementation.ORM)> Public Const ConstFNTOUID = CurrentLink.ConstFNTOUID

        ''' <summary>
        ''' Type
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceobjectentry:=LinkType.ConstObjectID & "." & LinkType.constFNTypeID, _
            title:="Type", description:="type of the deliverable link", XID:="DLVL5", _
            LookupPropertyStrings:={LookupProperty.UseAttributeReference}, validationPropertyStrings:={ObjectValidationProperty.UseLookup}) _
           > Public Const constFNLinkTypeID = CurrentLink.constFNLinkTypeID

        ''' <summary>
        ''' Foreign Key to Link Type
        ''' </summary>
        ''' <remarks></remarks>
        <ormForeignKey(entrynames:={constFNLinkTypeID, ConstFNDomainID}, _
            foreignkeyreferences:={LinkType.ConstObjectID & "." & LinkType.constFNTypeID, _
                                   LinkType.ConstObjectID & "." & LinkType.ConstFNDomainID}, _
            foreignkeyproperties:={ForeignKeyProperty.OnDelete & "(" & ForeignKeyActionProperty.SetDefault & ")", _
                                   ForeignKeyProperty.OnUpdate & "(" & ForeignKeyActionProperty.Cascade & ")"}, _
            useforeignkey:=otForeignKeyImplementation.ORM)> Public Const constFKLinkType = "FKLinkType"

        <ormObjectEntry(referenceObjectEntry:=Deliverable.ConstObjectID & "." & Deliverable.ConstFNDLVUID, isnullable:=True, _
                        Title:="describing deliverable", description:="link is described by this deliverable by uid", _
                       xid:="DLVL11", useforeignkey:=otForeignKeyImplementation.ORM)> Public Const ConstFNDESCUID = "DESCDLVUID"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=True, dbdefaultvalue:="1", _
            title:="is enabled", description:="is the link active", XID:="DLVL12")> Public Const ConstFNIsEnabled = "ISENABLED"


        <ormObjectEntry(Datatype:=otDataType.Timestamp, isnullable:=True, _
            XID:="DLVL13", title:="valid from", description:="link is valid from ")> Public Const ConstFNValidFrom = "VALIDFROM"

        <ormObjectEntry(Datatype:=otDataType.Timestamp, isnullable:=True, _
            XID:="DLVL14", title:="valid until", description:="link is valid until ")> Public Const ConstFNValiduntil = "VALIDUNTIL"

        ''' <summary>
        ''' Link Status
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=Commons.StatusType.ConstObjectID & "." & Commons.StatusType.ConstFNID, _
                        isnullable:=True, _
                        Title:="Status Type", description:="type of the status", _
                        XID:="DLVL20", isnullable:=True)> Public Const ConstFNLinkStatusType = "STATUSTYPE"

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

        ''' <summary>
        ''' Status Code
        ''' </summary>
        ''' <remarks></remarks>

        <ormObjectEntry(referenceObjectEntry:=StatusItem.ConstObjectID & "." & StatusItem.constFNCode, _
                       isnullable:=True, _
                       title:="Link Status", Description:="Link Status", _
                        XID:="DLVL21")> Public Const ConstFNLinkStatus = "STATUS"

        <ormObjectEntry(Datatype:=otDataType.Timestamp, _
                        title:="Link Status Checked", Description:="timestamp of last link check status run", _
                        XID:="DLVL22", isnullable:=True)> Public Const ConstFNLinkCheckTimestamp = "STATUSCHECKEDON"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0", _
            title:="Status outdated", description:="true if the status must be rechecked due to changes", XID:="DLVL23")> Public Const ConstFNLinkCheckStatus = "CHECKSTATUS"

        ' change FK Action since we have the workspace as FK (leads also to domians)
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, _
              dbdefaultvalue:=ConstGlobalDomain, defaultvalue:=ConstGlobalDomain, _
              useforeignkey:=otForeignKeyImplementation.None)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        ''' <summary>
        ''' Mappings
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNLinkUid)> Private _linkuid As Long
        <ormObjectEntryMapping(EntryName:=ConstFnLinkUpdc)> Private _updc As Long
        <ormObjectEntryMapping(EntryName:=ConstFNTOUID)> Private _touid As Long
        <ormObjectEntryMapping(EntryName:=ConstFNFROMUID)> Private _fromuid As Long
        <ormObjectEntryMapping(EntryName:=constFNLinkTypeID)> Private _typeid As String

        <ormObjectEntryMapping(EntryName:=ConstFNDESCUID)> Private _descuid As Long?
        <ormObjectEntryMapping(EntryName:=ConstFNIsEnabled)> Private _isEnabled As Boolean = True 'explicitly set to be active in the beginning !
        <ormObjectEntryMapping(EntryName:=ConstFNValidFrom)> Private _validfrom As DateTime?
        <ormObjectEntryMapping(EntryName:=ConstFNValiduntil)> Private _validuntil As DateTime?

        <ormObjectEntryMapping(EntryName:=ConstFNLinkStatusType)> Private _linkstatustype As String
        <ormObjectEntryMapping(EntryName:=ConstFNLinkStatus)> Private _linkstatusCode As String
        <ormObjectEntryMapping(EntryName:=ConstFNLinkCheckTimestamp)> Private _linkcheckedon As DateTime?
        <ormObjectEntryMapping(EntryName:=ConstFNLinkCheckStatus)> Private _linkstatusoutdated As Boolean?

        ''' <summary>
        ''' Relation to Link StatusItem
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(StatusItem), fromentries:={ConstFNLinkStatusType, ConstFNLinkStatus}, _
            toentries:={StatusItem.constFNType, StatusItem.constFNCode}, _
            cascadeOnCreate:=False, cascadeOnDelete:=False, cascadeOnUpdate:=False)> Public Const ConstRLinkSTatus = "RELLinkSTATUS"

        <ormObjectEntryMapping(RelationName:=ConstRLinkSTatus, infuseMode:=otInfuseMode.OnDemand)> Private WithEvents _Linkstatus As StatusItem

        '' <summary>
        ''' Relation to Type
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkObject:=GetType(DeliverableType), toprimaryKeys:={constFNLinkTypeID, ConstFNDomainID}, _
                     cascadeonCreate:=False, cascadeOnDelete:=False, cascadeOnUpdate:=False)> _
        Public Const ConstRType = "RELType"

        <ormObjectEntryMapping(relationName:=ConstRType, infusemode:=otInfuseMode.OnDemand)> Private _Type As LinkType
        ''' <summary>
        ''' dynamic data
        ''' </summary>
        ''' <remarks></remarks>


#Region "Properties"

        ''' <summary>
        ''' retrieves a type object of this Deliverable link
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Type As LinkType
            Get
                If Not Me.IsAlive(subname:="Type") Then Return Nothing
                Me.InfuseRelation(ConstRType)
                Return _Type

            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the linkuid.
        ''' </summary>
        ''' <value>The linkuid.</value>
        Public ReadOnly Property LinkUID() As Long
            Get
                Return Me._linkuid
            End Get

        End Property

        ''' <summary>
        ''' Gets or sets the updc.
        ''' </summary>
        ''' <value>The updc.</value>
        Public ReadOnly Property LinkUPDC() As Long
            Get
                Return Me._updc
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the check status outdated flag.
        ''' </summary>
        ''' <value>The checkstatus.</value>
        Public Property LinkStatusOutDated() As Boolean?
            Get
                Return Me._linkstatusoutdated
            End Get
            Set(value As Boolean?)
                SetValue(ConstFNLinkCheckStatus, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the link Status Timestamp.
        ''' </summary>
        ''' <value>The Link Status Timestamp.</value>
        Public Property LinkStatusTimestamp() As DateTime?
            Get
                Return Me._linkcheckedon
            End Get
            Set(value As DateTime?)
                SetValue(ConstFNLinkCheckTimestamp, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the link status item 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LinkStatus() As StatusItem
            Get
                If _Linkstatus Is Nothing OrElse _Linkstatus.Code <> _linkstatusCode OrElse _linkstatustype <> _Linkstatus.Code Then InfuseRelation(ConstRLinkSTatus)
                Return _Linkstatus
            End Get
            Set(value As StatusItem)
                If value IsNot Nothing Then
                    Me.LinkStatusType = value.TypeID
                    Me.LinkStatusCode = value.Code
                    _Linkstatus = value
                Else
                    Me.LinkStatusCode = Nothing
                    Me.LinkStatusType = Nothing
                    _Linkstatus = Nothing
                End If
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the Link Status Code.
        ''' </summary>
        ''' <value>The Link Status.</value>
        Public Property LinkStatusCode() As String
            Get
                Return Me._linkstatusCode
            End Get
            Set(value As String)
                SetValue(ConstFNLinkStatus, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the Type of the Link Status.
        ''' </summary>
        ''' <value>The syncstatustype.</value>
        Public Property LinkStatusType() As String
            Get
                Return Me._linkstatustype
            End Get
            Set(value As String)
                SetValue(ConstFNLinkStatusType, value)
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
        ''' Gets or sets the UID of the describing deliverable of the link.
        ''' </summary>
        ''' <value>The descuid.</value>
        Public Property DescribedByUID() As Long?
            Get
                Return Me._descuid
            End Get
            Set(value As Long?)
                SetValue(ConstFNDESCUID, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the typeid.
        ''' </summary>
        ''' <value>The typeid.</value>
        Public Property Typeid() As String
            Get
                Return Me._typeid
            End Get
            Set(value As String)
                SetValue(constFNLinkTypeID, value)
            End Set
        End Property

        ''' <summary>
        ''' returns the TO UID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ToUID() As Long
            Get
                Return _touid
            End Get
            Set(value As Long)
                SetValue(ConstFNTOUID, value)
            End Set

        End Property
        ''' <summary>
        ''' returns the From UID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property FromUID() As Long
            Get
                Return _fromuid
            End Get
            Set(value As Long)
                SetValue(ConstFNFROMUID, value)
            End Set
        End Property

        ''' <summary>
        ''' gets or sets the active flag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsEnabled() As Boolean
            Get
                Return _isEnabled
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsEnabled, value)
            End Set
        End Property


#End Region

        ''' <summary>
        ''' retrieve  the configuration from store
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(uid As Long, updc As Long) As Link
            Return ormBusinessObject.RetrieveDataObject(Of Link)(pkArray:={uid, updc})
        End Function

        ''' <summary>
        ''' handler for onCreating Event - generates unique primary key values
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub Link_OnCreating(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnCreating
            Dim uid As Long? = e.Record.GetValue(ConstFNLinkUid)
            Dim updc As Long? = e.Record.GetValue(ConstFnLinkUpdc)
            Dim tag As String
            If Not uid.HasValue OrElse uid = 0 Then
                tag = ConstFNLinkUid
                uid = Nothing
                updc = 1
            ElseIf Not updc.HasValue OrElse updc = 0 Then
                tag = ConstFnLinkUpdc
                updc = Nothing
            End If
            Dim primarykey As Object() = {uid, updc}
            If uid Is Nothing Then
                If e.DataObject.ObjectPrimaryContainerStore.CreateUniquePkValue(pkArray:=primarykey, tag:=tag) Then
                    e.Record.SetValue(ConstFNLinkUid, primarykey(0))
                    e.Record.SetValue(ConstFnLinkUpdc, primarykey(1))
                    e.Result = True
                    e.Proceed = True
                Else
                    CoreMessageHandler(message:="primary keys couldnot be created ?!", procedure:="Link.OnCreate", messagetype:=otCoreMessageType.InternalError)
                End If
            End If

        End Sub
        ''' <summary>
        ''' creates a persistable Link
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(Optional uid As Long = 0, _
                                                Optional updc As Long = 0, _
                                                Optional linktype As String = Nothing,
                                                Optional toUID As Long? = Nothing, _
                                                Optional fromUID As Long? = Nothing) As Link
            Dim aRecord As New ormRecord
            With aRecord
                .SetValue(ConstFNLinkUid, uid)
                .SetValue(ConstFnLinkUpdc, updc)
                If Not String.IsNullOrWhiteSpace(linktype) Then .SetValue(constFNLinkTypeID, linktype)
                If toUID.HasValue Then .SetValue(ConstFNTOUID, toUID.Value)
                If fromUID.HasValue Then .SetValue(ConstFNFROMUID, fromUID.Value)
            End With
            Return ormBusinessObject.CreateDataObject(Of Link)(aRecord, checkUnique:=True)
        End Function

        ''' <summary>
        ''' clone the object with the new primary key
        ''' </summary>
        ''' <param name="pkarray">primary key array</param>
        ''' <remarks></remarks>
        ''' <returns>the new cloned object or nothing</returns>
        Public Overloads Function Clone(pkarray() As Object, Optional runtimeOnly As Boolean? = Nothing) As Link Implements iormCloneable(Of Link).Clone
            If Not MyBase.Feed() Then
                Return Nothing
            End If

            If pkarray.Length = 0 OrElse pkarray(0) Is Nothing OrElse pkarray(0) = 0 Then
                Call CoreMessageHandler(message:="Deliverable UID cannot be 0 or Nothing or primary key array not set for clone - must be set", argument:=pkarray, _
                                        procedure:="Link.Clone", messagetype:=otCoreMessageType.InternalError, containerID:=ObjectPrimaryTableID)
                Return Nothing
            End If
            If pkarray.Length = 1 OrElse pkarray(1) Is Nothing OrElse pkarray(0) = 0 Then
                If Not Me.ObjectPrimaryTableStore.CreateUniquePkValue(pkarray) Then
                    Call CoreMessageHandler(message:="failed to create an unique primary key value", argument:=pkarray, _
                                            procedure:="Link.Clone", messagetype:=otCoreMessageType.InternalError, containerID:=ObjectPrimaryTableID)
                    Return Nothing
                End If
            End If
            '**
            Return MyBase.Clone(Of Link)(pkarray)
        End Function

        ''' <summary>
        ''' clone the loaded or created dataobject object
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <param name="VERSION"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Clone(Optional ByVal uid? As Long = Nothing, Optional ByVal updc? As Long = Nothing) As Link
            If Not uid.HasValue Then uid = Me.LinkUID
            Dim pkarray() As Object = {uid, updc}
            Return Me.Clone(pkarray)
        End Function

        ''' <summary>
        ''' set default values
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub Link_OnDefaultValueNeeded(sender As Object, e As ormDataObjectEntryEventArgs) Handles Me.OnDefaultValueNeeded
            Select Case e.ObjectEntryName
                Case ConstFNValidFrom
                    If e.Value Is Nothing Then e.Value = DateTime.Now
                    e.Result = True
            End Select
        End Sub
    End Class
End Namespace
