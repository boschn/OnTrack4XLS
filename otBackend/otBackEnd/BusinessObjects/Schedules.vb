
REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** BUSINESS OBJECTs CLASSES: Schedules and Schedule Definitions
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
Imports System.Diagnostics.Debug

Imports OnTrack
Imports OnTrack.Database
Imports OnTrack.Deliverables
Imports OnTrack.XChange
Imports OnTrack.Calendar
Imports OnTrack.Commons
Imports OnTrack.Core

Namespace OnTrack.Scheduling
    ''' <summary>
    ''' enumeration of milestone types
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otMilestoneType
        [Date] = 1
        Status = 2
    End Enum
    ''' <summary>
    ''' Enumeration and other definitions
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otScheduleLinkType
        Deliverable = 1
    End Enum

    ''' <summary>
    ''' milestone definition class
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(version:=1, ID:=MileStoneDefinition.ConstObjectID, Modulename:=ConstModuleScheduling, _
        Description:="definition of milestones for all schedule types", useCache:=True, addDomainBehavior:=True, adddeletefieldbehavior:=True)> _
    Public Class MileStoneDefinition
        Inherits ormBusinessObject


        Public Const ConstObjectID = "MilestoneDefinition"

        ''' <summary>
        ''' Table Definition
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=2, usecache:=True)> Public Const ConstPrimaryTableID As String = "tblDefMilestones"

        ''' <summary>
        ''' primary keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, size:=20, defaultValue:="", PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty}, _
            XID:="bpd1", title:="ID", description:="id of the milestone")> Public Const ConstFNID = "id"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2, _
            useforeignkey:=otForeignKeyImplementation.NativeDatabase)> _
        Public Const ConstFNDomainID = Domain.ConstFNDomainID

        ''' <summary>
        ''' fields
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
           XID:="bpd2", title:="Description", description:="description of the milestone")> Public Const ConstFNDescription = "desc"

        <ormObjectEntry(Datatype:=otDataType.Text, defaultvalue:=otMilestoneType.Date, isnullable:=True, _
           XID:="bpd3", title:="Type", description:="type of the milestone")> Public Const ConstFNType = "typeid"

        <ormObjectEntry(Datatype:=otDataType.Long, defaultvalue:=otDataType.Date, dbdefaultvalue:="6", _
           XID:="bpd4", title:="Datatype", description:="datatype of the milestone")> Public Const ConstFNDatatype = "datatype"

        <ormObjectEntry(referenceobjectentry:=StatusItem.ConstObjectID & "." & StatusItem.constFNType, isnullable:=True, _
          lookuppropertystrings:={LookupProperty.UseAttributeReference}, validationPropertyStrings:={ObjectValidationProperty.UseLookup}, _
          XID:="bpd5", title:="Status Item Type", description:="status item type of the milestone")> Public Const ConstFNStatusType = "status"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0", _
         XID:="bpd6", title:="Forecast", description:="set if milestone is a forecast")> Public Const ConstFNIsForecast = "isforecast"

        <ormObjectEntry(referenceobjectentry:=ConstObjectID & "." & ConstFNID, isnullable:=True, _
             lookuppropertystrings:={LookupProperty.UseAttributeReference}, validationPropertyStrings:={ObjectValidationProperty.UseLookup}, _
             XID:="bpd7", title:="Reference", description:="set if milestone is a reference")> Public Const ConstFNRefMS = "refms"

        <ormObjectEntry(Datatype:=otDataType.Long, defaultvalue:=0, dbdefaultvalue:="0", _
                        XID:="bpd8", title:="Ordinal", Description:="ordinal of the object entry")> Public Const ConstFNordinal As String = "ordinal"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, isnullable:=True, _
                       XID:="bpd9", title:="Title", Description:="Title of the milestone ")> Public Const ConstFNTitle As String = "title"


        <ormObjectEntry(Datatype:=otDataType.List, isnullable:=True, _
                        XID:="bpd10", title:="Attachable ObjectIDs", description:="Object ids to be attached to")> Public Const ConstFNObjectIDs = "objectids"

        '** MAPPING
        <ormObjectEntryMapping(EntryName:=ConstFNID)> Private _id As String = String.Empty  ' id
        <ormObjectEntryMapping(EntryName:=ConstFNTitle)> Private _title As String
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _description As String
        <ormObjectEntryMapping(EntryName:=ConstFNType)> Private _typeid As otMilestoneType
        <ormObjectEntryMapping(EntryName:=ConstFNDatatype)> Private _datatype As otDataType
        <ormObjectEntryMapping(EntryName:=ConstFNRefMS)> Private _refid As String
        <ormObjectEntryMapping(EntryName:=ConstFNIsForecast)> Private _isForecast As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNStatusType)> Private _statustypeid As String
        <ormObjectEntryMapping(EntryName:=ConstFNordinal)> Private _ordinal As Long
        <ormObjectEntryMapping(EntryName:=ConstFNObjectIDs)> Private _objectids As List(Of String)

        ''' <summary>
        ''' dynamic members
        ''' </summary>
        ''' <remarks></remarks>
        ''' 

        ''' relation path for compound data 
        Private _relationpath As String() = {ScheduleLink.ConstObjectID & "." & ScheduleLink.ConstRWorkspaceSchedule, _
                                         WorkspaceSchedule.ConstObjectID & "." & WorkspaceSchedule.ConstRWorkEdition, _
                                         ScheduleEdition.ConstObjectID & "." & ScheduleEdition.ConstRMilestones, _
                                         ScheduleMilestone.ConstObjectID}



#Region "Properties"


        ''' <summary>
        ''' Gets or sets the title.
        ''' </summary>
        ''' <value>The title.</value>
        Public Property Title() As String
            Get
                Return Me._title
            End Get
            Set(value As String)
                Me._title = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the ordinal.
        ''' </summary>
        ''' <value>The ordinal.</value>
        Public Property Ordinal() As Long
            Get
                Return Me._ordinal
            End Get
            Set(value As Long)
                SetValue(ConstFNordinal, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the objectids where milestone might be attached - becomes compounds.
        ''' </summary>
        ''' <value>The objectids.</value>
        Public Property AttachedObjectids() As List(Of String)
            Get
                Return Me._objectids
            End Get
            Set(value As List(Of String))
                Me._objectids = value
            End Set
        End Property

        ''' <summary>
        ''' gets  the ID of the Milestone
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ID() As String
            Get
                Return _id
            End Get

        End Property

        Public Property Datatype() As otDataType
            Get
                Datatype = _datatype
            End Get
            Set(value As otDataType)
                SetValue(ConstFNDatatype, value)
            End Set
        End Property
        ''' <summary>
        ''' sets or gets the type of the milestone
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Typeid() As otMilestoneType
            Get
                Typeid = _typeid
            End Get
            Set(value As otMilestoneType)
                SetValue(ConstFNType, value)
            End Set
        End Property
        ''' <summary>
        ''' returns True if the Milestone is a date
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsDate() As Boolean
            Get
                If _typeid = otMilestoneType.Date Then
                    Return True
                Else
                    Return False
                End If
            End Get
            Set(value As Boolean)
                If value Then
                    Me.Typeid = otMilestoneType.Date
                Else
                    Me.Typeid = otMilestoneType.Status
                End If
            End Set
        End Property

        ''' <summary>
        ''' returns true if the milestone is a status
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsStatus() As Boolean
            Get
                If _typeid = otMilestoneType.Status Then
                    Return True
                Else
                    Return False
                End If
            End Get
            Set(value As Boolean)
                If value Then
                    Me.Typeid = otMilestoneType.Status
                Else
                    Me.Typeid = otMilestoneType.Date
                End If
            End Set
        End Property

        ''' <summary>
        ''' returns true if the milestone is an actual milestone
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsActual() As Boolean
            Get
                Return Not Me.IsForecast
            End Get
            Set(value As Boolean)
                Me.IsForecast = Not value
            End Set
        End Property
        ''' <summary>
        ''' gets or sets true if the milestone is a forecast
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsForecast() As Boolean
            Get
                Return _isForecast
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsForecast, value)
            End Set
        End Property

        ''' <summary>
        ''' sets or gets the description of the milestone
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(value As String)
                SetValue(ConstFNDescription, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the status type id
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property statustypeid() As String
            Get
                Return _statustypeid
            End Get
            Set(value As String)
                SetValue(ConstFNStatusType, value)
            End Set
        End Property
        ''' <summary>
        ''' sets or gets the reference milestone for this milestone (actual <-> forecast)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property referingToID() As String
            Get
                Return _refid
            End Get
            Set(value As String)
                SetValue(ConstFNRefMS, value)
            End Set
        End Property

#End Region

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
                '.Aliases = me.aliases
                .Datatype = Me.Datatype
                ' ordinal calculate an ordinal
                .Ordinal = 100000 + Me.Ordinal
                .IsNullable = True
                .DefaultValue = Nothing
                '.Size = Me.Size
                '.InnerDatatype = Me.InnerDatatype
                '.Version = Me.Version
                .Title = Me.Title
                .Description = Me.Description
                ' addition
                '.LookupCondition = Me.LookupCondition
                '.LookupProperties = Me.LookupProperties
                '.PossibleValues = Me.PossibleValues
                '.LowerRangeValue = Me.LowerRangeValue
                '.UpperRangeValue = Me.HasUpperRangeValue
                '.ValidateRegExpression = Me.ValidateRegExpression
                '.Validationproperties = Me.Validationproperties
                .XID = Me.ID
                '.IsValidating = Me.IsValidating
                '.RenderProperties = Me.RenderProperties
                '.RenderRegExpMatch = Me.RenderRegExpMatch
                '.RenderRegExpPattern = Me.RenderRegExpMatch
                '.IsRendering = Me.IsRendering

                ''' special compound settings
                .CompoundObjectID = ScheduleMilestone.ConstObjectID
                .CompoundValueEntryName = ScheduleMilestone.ConstFNvalue
                .CompoundIDEntryname = ScheduleMilestone.ConstFNID
                .CompoundGetterMethodName = Nothing
                .CompoundSetterMethodName = Nothing
                .CompoundRelationPath = {}

            End With
        End Sub

        ''' <summary>
        ''' the on created handler to set the  datatypes
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>

        Private Sub MileStoneDefinition_OnCreated(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreated
            If Me.IsDate Then
                Me.Datatype = otDataType.Date
            ElseIf Me.IsStatus Then
                Me.Datatype = otDataType.Text
            End If
        End Sub

        ''' <summary>
        ''' handles the default value needed event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub MileStoneDefinition_OnDefaultValuesNeeded(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreateDefaultValuesNeeded

        End Sub

        ''' <summary>
        ''' Creates the Compound Structure for the Milestone Definition
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateCompoundStructure() As Boolean
            Dim result As Boolean = True

            ''' attach the Properties as compounds
            ''' 
            If AttachedObjectids Is Nothing Then Return False

            For Each anObjectID In Me.AttachedObjectids
                Dim anObjectDefinition As ormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=anObjectID)
                If anObjectDefinition IsNot Nothing Then
                    Dim apath As String()
                    ReDim apath(_relationpath.GetUpperBound(0) + 1)
                    ''' set it to the linking objects
                    ''' 
                    If anObjectDefinition.ID = Deliverable.ConstObjectID Then
                        apath(0) = anObjectID & "." & Deliverable.ConstRScheduleLink
                    ElseIf Not String.IsNullOrWhiteSpace(ObjectDefinition.Objectname) Then
                        CoreMessageHandler(message:="other objects for properties to be linked to not implemented", procedure:="MileStoneDefinition.OnPersisted", _
                                            argument:=anObjectDefinition.ID, objectname:=Me.ObjectID)
                    End If

                    Array.ConstrainedCopy(_relationpath, 0, apath, 1, apath.Length - 1)
                    ''' create all the relational path
                    ''' 
                    For i = apath.GetLowerBound(0) To apath.GetUpperBound(0) - 1
                        Dim names As String() = apath(i).ToUpper.Split("."c) ' get the objectname from the canonical form
                        Dim aCompound As ormObjectCompoundEntry = ormObjectCompoundEntry.Create(objectname:=names(0), _
                                                                                     entryname:=Me.ID, domainid:=Me.DomainID, _
                                                                                     runtimeOnly:=Me.RunTimeOnly, checkunique:=True)
                        If aCompound Is Nothing Then aCompound = ormObjectCompoundEntry.Retrieve(objectname:=names(0), entryname:=Me.ID, runtimeOnly:=Me.RunTimeOnly)

                        ''' set the values
                        ''' 
                        SetCompound(compound:=aCompound)
                        Dim relpath As String()
                        ReDim relpath(apath.GetUpperBound(0) - i)
                        Array.ConstrainedCopy(apath, i, relpath, 0, relpath.Length)
                        aCompound.CompoundRelationPath = relpath

                        ''' on WorkspaceSchedule Level we need to go to the setter to enable
                        ''' versioning on the lot if a changed property is needed
                        If names(0) = WorkspaceSchedule.ConstObjectID.ToUpper Then
                            aCompound.CompoundSetterMethodName = WorkspaceSchedule.ConstOPSetMileStoneValue
                            aCompound.CompoundGetterMethodName = WorkspaceSchedule.ConstOPGetMileStoneValue
                            ''' 
                            ''' on the end take the setter / getter operations to resolve
                            ''' 
                        ElseIf names(0) = ScheduleEdition.ConstObjectID.ToUpper Then
                            aCompound.CompoundSetterMethodName = ScheduleEdition.ConstOPSetMileStoneValue
                            aCompound.CompoundGetterMethodName = ScheduleEdition.ConstOPGetMileStoneValue
                        Else
                            aCompound.CompoundGetterMethodName = Nothing
                            aCompound.CompoundSetterMethodName = Nothing
                        End If


                        ''' set it to the linking objects 
                        '''  

                        result = result And aCompound.Persist()

                    Next


                End If
            Next

            Return result
        End Function
        ''' <summary>
        ''' OnPersisted Handler to add the Properties as Compounds to the ObjectIDs
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub MileStoneDefinitoin_OnPersisted(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnPersisted
            Call Me.CreateCompoundStructure()
        End Sub
        ''' <summary>
        ''' Retrieve
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal id As String, Optional domainid As String = Nothing, Optional forcereload As Boolean = False) As MileStoneDefinition
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey() As Object = {id.ToUpper, domainid}
            Return RetrieveDataObject(Of MileStoneDefinition)(pkArray:=primarykey, domainID:=domainid, forceReload:=forcereload)
        End Function

        ''' <summary>
        ''' Return a collection of all def Milestones
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function All(Optional domainid As String = Nothing) As List(Of MileStoneDefinition)
            Return ormBusinessObject.AllDataObject(Of MileStoneDefinition)(domainid:=domainid)
        End Function

        ''' <summary>
        ''' create persistable object with primary key ID
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal ID As String, Optional domainid As String = Nothing) As MileStoneDefinition
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim pkarray() As Object = {ID.ToUpper, domainid}
            Return ormBusinessObject.CreateDataObject(Of MileStoneDefinition)(pkarray, checkUnique:=True, domainID:=domainid)
        End Function

    End Class


    ''' <summary>
    ''' Definition of a  milestone per schedule definition class
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=ScheduleMilestoneDefinition.ConstObjectID, modulename:=ConstModuleScheduling, _
        Version:=1, description:="declaration of milestones specific in a schedule type", _
        addDomainBehavior:=True, adddeletefieldbehavior:=True, useCache:=True)> _
    Public Class ScheduleMilestoneDefinition
        Inherits ormBusinessObject

        Public Const ConstObjectID = "ScheduleMilestoneDefinition"

        ''' <summary>
        ''' table definition
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=2, usecache:=True)> Public Const ConstPrimaryTableID As String = "tblDefScheduleMilestones"
        '*** Index
        <ormIndex(columnname1:=ConstFNType, columnname2:=ConstFNOrdinal)> Public Const ConstIndOrder = "orderby"

        ''' <summary>
        ''' primary keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(XID:="BSD1", referenceobjectentry:=ScheduleDefinition.ConstObjectID & "." & ScheduleDefinition.ConstFNType, _
            PrimaryKeyOrdinal:=1, aliases:={"SCT1"}, title:="schedule type", defaultvalue:="", dbdefaultvalue:="", _
            description:=" type of schedule definition")> Public Const ConstFNType = "scheduletype"

        <ormObjectEntry(XID:="BSD2", referenceobjectentry:=MileStoneDefinition.ConstObjectID & "." & MileStoneDefinition.ConstFNID, _
            PrimaryKeyOrdinal:=2, title:="milestone id", description:=" id of milestone in schedule")> Public Const ConstFNID = "id"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=3, _
            useforeignkey:=otForeignKeyImplementation.NativeDatabase)> _
        Public Const ConstFNDomainID = Domain.ConstFNDomainID

        ''' <summary>
        ''' fields
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(XID:="BSD3", Datatype:=otDataType.Text, isnullable:=True, _
            title:="description", description:="description of milestone in schedule")> Public Const ConstFNDesc = "desc"

        <ormObjectEntry(XID:="BSD4", Datatype:=otDataType.Long, defaultvalue:=1, dbdefaultvalue:="1", _
            title:="ordinal", description:="ordinal of milestone in schedule")> Public Const ConstFNOrdinal = "ordinal"

        <ormObjectEntry(XID:="BSD5", isnullable:=True, _
            referenceobjectentry:=MileStoneDefinition.ConstObjectID & "." & MileStoneDefinition.ConstFNID, isnullable:=True, _
            title:="actual of fc milestone id", description:=" actual id of this milestone in schedule")> Public Const ConstFNActualID = "actualid"

        <ormObjectEntry(XID:="BSD6", Datatype:=otDataType.Bool, dbdefaultvalue:="0", defaultvalue:=False, _
            title:="is forecast", description:=" milestone is forecast in schedule")> Public Const ConstFNIsFC = "isfc"

        <ormObjectEntry(XID:="BSD7", Datatype:=otDataType.Bool, dbdefaultvalue:="0", defaultvalue:=False, _
            title:="is facilitative", description:=" milestone is facilitative in schedule")> Public Const ConstFNIsFacultative = "isfacultative"

        <ormObjectEntry(XID:="BSD8", Datatype:=otDataType.Bool, dbdefaultvalue:="0", defaultvalue:=False, _
            title:="is prohibited", description:=" milestone is prohibited in schedule")> Public Const ConstFNIsProhibited = "isprohibited"

        <ormObjectEntry(XID:="BSD9", Datatype:=otDataType.Bool, dbdefaultvalue:="0", defaultvalue:=False, _
            title:="is mandatory", description:=" milestone is mandatory in schedule")> Public Const ConstFNIsMandatory = "ismandatory"

        <ormObjectEntry(XID:="BSD11", Datatype:=otDataType.Bool, dbdefaultvalue:="0", defaultvalue:=False, _
            title:="is input", description:=" milestone is input deliverable in schedule")> Public Const ConstFNIsINPUT = "isinput"

        <ormObjectEntry(XID:="BSD12", Datatype:=otDataType.Bool, dbdefaultvalue:="0", defaultvalue:=False, _
            title:="is output", description:=" milestone is output deliverable in schedule")> Public Const ConstFNIsOutPut = "isoutput"

        <ormObjectEntry(XID:="BSD13", Datatype:=otDataType.Bool, dbdefaultvalue:="0", defaultvalue:=False, _
            title:="is finish", description:=" milestone is end of schedule")> Public Const ConstFNIsFinish = "isfinish"

        <ormObjectEntry(XID:="BSD14", Datatype:=otDataType.Text, isnullable:=True, _
            title:="default value of milestone", description:=" milestone default value in this schedule")> Public Const ConstFNDefaultValue = "default"

        ''' <summary>
        ''' Mapping
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNType)> Private _scheduletype As String
        <ormObjectEntryMapping(EntryName:=ConstFNID)> Private _id As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNDesc)> Private _description As String
        <ormObjectEntryMapping(EntryName:=ConstFNOrdinal)> Private _Ordinal As Long
        <ormObjectEntryMapping(EntryName:=ConstFNIsFC)> Private _isForecast As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNActualID)> Private _actualid As String

        <ormObjectEntryMapping(EntryName:=ConstFNIsMandatory)> Private _isMandatory As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNIsProhibited)> Private _isProhibited As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNIsFacultative)> Private _isFacultative As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNIsFinish)> Private _isFinish As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNIsINPUT)> Private _isInputDeliverable As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNIsOutPut)> Private _isOutputDeliverable As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNDefaultValue)> Private _defaultValue As String
        ''' <summary>
        ''' dynamic data
        ''' </summary>
        ''' <remarks></remarks>

        Private _scheduledefinition As ScheduleDefinition 'backlink

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the default value in string.
        ''' </summary>
        ''' <value>The default value.</value>
        Public Property DefaultValue() As String
            Get
                Return Me._defaultValue
            End Get
            Set(value As String)
                Me._defaultValue = value
            End Set
        End Property

        ''' <summary>
        ''' gets the schedule type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ScheduleTypeID() As String
            Get
                Return _scheduletype
            End Get
        End Property

        ''' <summary>
        ''' gets the Milestone ID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ID() As String
            Get
                Return _id
            End Get

        End Property

        ''' <summary>
        ''' gets or sets the actual milestone id (counterpart) a forecast milestone
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ActualOfFC() As String
            Get
                Return _actualid
            End Get
            Set(value As String)
                SetValue(ConstFNActualID, value)
            End Set
        End Property
        ''' <summary>
        ''' returns true if actual
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsActual() As Boolean
            Get
                Return Not _isForecast
            End Get
            Set(value As Boolean)
                Me.IsForecast = Not value
            End Set
        End Property

        ''' <summary>
        ''' gets or sets if the milestone is a forecast
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsForecast() As Boolean
            Get
                Return _isForecast
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsFC, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or set the flag for marking the milestone as end milestone
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsFinish() As Boolean
            Get
                Return _isFinish
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsFinish, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the mandatory flag for the milestone
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsMandatory() As Boolean
            Get
                Return _isMandatory
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsMandatory, value)
                If value Then
                    SetValue(ConstFNIsFacultative, False)
                    SetValue(ConstFNIsProhibited, False)
                End If
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the prohibited flag for this milestone in the schedule
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsProhibited() As Boolean
            Get
                Return _isProhibited
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsProhibited, value)
                If value Then
                    SetValue(ConstFNIsFacultative, False)
                    SetValue(ConstFNIsMandatory, False)
                End If
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the faculatative flag for this milestone
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsFacultative() As Boolean
            Get
                Return _isFacultative
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsFacultative, value)
                If value Then
                    SetValue(ConstFNIsMandatory, False)
                    SetValue(ConstFNIsProhibited, False)
                End If
            End Set
        End Property
        ''' <summary>
        ''' gets or set the outdeliverable flag for this milestone in this schedule
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsOutputDeliverable() As Boolean
            Get
                Return _isOutputDeliverable
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsOutPut, value)
            End Set
        End Property

        ''' <summary>
        ''' gets or sets the input deliverable flag for this milestone in this schedule
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsInputDeliverable() As Boolean
            Get
                Return _isInputDeliverable
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsINPUT, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the description
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(value As String)
                SetValue(ConstFNDesc, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the ordinal
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Ordinal() As Long
            Get
                Return _Ordinal
            End Get
            Set(value As Long)
                SetValue(ConstFNOrdinal, value)
            End Set
        End Property
#End Region

        ''' <summary>
        ''' Handles OnCreating 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ScheduleMilestoneDefinition_OnCreating(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreating
            Dim my As ScheduleMilestoneDefinition = TryCast(e.DataObject, ScheduleMilestoneDefinition)

            If my IsNot Nothing Then
                Dim aScheduletypeID As String = e.Record.GetValue(ConstFNType)
                If aScheduletypeID Is Nothing Then
                    CoreMessageHandler(message:="schedule definition does not exist", procedure:="ScheduleMilestoneDefinition.OnCreating", _
                                       messagetype:=otCoreMessageType.ApplicationError, _
                                       argument:=my.ScheduleTypeID)
                    e.AbortOperation = True
                    Return
                End If
                ''' even if it is early to retrieve the set and set it (since this might disposed since we have not run through checkuniqueness and cache)
                ''' we need to check on the object here
                _scheduledefinition = ScheduleDefinition.Retrieve(id:=aScheduletypeID, domainid:=Me.DomainID)
                If _scheduledefinition Is Nothing Then
                    CoreMessageHandler(message:="schedule definition does  does not exist", procedure:="ScheduleMilestoneDefinition.OnCreated", _
                                       messagetype:=otCoreMessageType.ApplicationError, _
                                       argument:=aScheduletypeID)
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
        Public Sub ScheduleMilestoneDefinition_OnCreated(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreated
            Dim my As ScheduleMilestoneDefinition = TryCast(e.DataObject, ScheduleMilestoneDefinition)

            If my IsNot Nothing Then
                If _scheduledefinition Is Nothing Then
                    _scheduledefinition = ScheduleDefinition.Retrieve(id:=Me.ScheduleTypeID, domainid:=Me.DomainID)
                    If _scheduledefinition Is Nothing Then
                        CoreMessageHandler(message:="object propert set doesnot exist", procedure:="ScheduleMilestoneDefinition.OnCreated", _
                                          messagetype:=otCoreMessageType.ApplicationError, _
                                           argument:=Me.ScheduleTypeID)
                        e.AbortOperation = True
                        Return
                    End If
                End If
            End If

        End Sub
        ''' <summary>
        ''' Handles OnCreating and Relation to ConfigSection
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ScheduleMilestoneDefinition_OnInfused(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnInfused
            Dim my As ScheduleMilestoneDefinition = TryCast(e.DataObject, ScheduleMilestoneDefinition)

            ''' infuse is called on create as well as on retrieve / inject 
            ''' only on the create case we need to add to the scheduledefinition otherwise
            ''' scheduledefinition will load the property
            ''' or the property will stand alone
            If my IsNot Nothing AndAlso e.Infusemode = otInfuseMode.OnCreate AndAlso _scheduledefinition IsNot Nothing Then
                _scheduledefinition.Milestones.Add(my)
            End If
        End Sub

        ''' <summary>
        ''' Retrieve
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal scheduletype As String, ByVal ID As String, Optional domainID As String = Nothing, Optional forcereload As Boolean = False) As ScheduleMilestoneDefinition
            If String.IsNullOrEmpty(domainID) Then domainID = CurrentSession.CurrentDomainID
            Return RetrieveDataObject(Of ScheduleMilestoneDefinition)(pkArray:={scheduletype.ToUpper, ID.ToUpper, domainID}, domainID:=domainID, forceReload:=forcereload)
        End Function

        '**** getDefMilestone
        '****
        ''' <summary>
        ''' retrieve related Milestone Definition Object
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMilestoneDefinition() As MileStoneDefinition
            If Not IsAlive(subname:="GetMilestoneDefinition") Then Return Nothing
            Return MileStoneDefinition.Retrieve(id:=Me.ID, domainid:=Me.DomainID)
        End Function


        ''' <summary>
        ''' retrieve a collection of all schedule milestone definition objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function All() As List(Of ScheduleMilestoneDefinition)
            Return ormBusinessObject.AllDataObject(Of ScheduleMilestoneDefinition)()
        End Function

        ''' <summary>
        ''' create the persistable object
        ''' </summary>
        ''' <param name="SCHEDULETYPE"></param>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal scheduletype As String, ByVal ID As String, Optional domainID As String = Nothing) As ScheduleMilestoneDefinition
            If String.IsNullOrEmpty(domainID) Then domainID = CurrentSession.CurrentDomainID
            Dim pkarray() As Object = {scheduletype.ToUpper, ID.ToUpper, domainID}
            Return ormBusinessObject.CreateDataObject(Of ScheduleMilestoneDefinition)(pkarray, domainID:=domainID, checkUnique:=True)
        End Function

    End Class

    ''' <summary>
    ''' schedule definition object
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=ScheduleDefinition.ConstObjectID, modulename:=ConstModuleScheduling, Version:=1, _
        description:="definition of schedules (types)", useCache:=True, adddeletefieldbehavior:=True, addDomainBehavior:=True)> Public Class ScheduleDefinition
        Inherits ormBusinessObject


        Public Const ConstObjectID = "ScheduleDefinition"

        ''' <summary>
        ''' TableDefinition
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=3, usecache:=True)> Public Const ConstPrimaryTableID = "tblDefSchedules"

        ''' <summary>
        ''' keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, title:="ID", size:=50, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertystrings:={ObjectValidationProperty.NotEmpty}, _
            Description:="Unique ID of the schedule type definition", _
            PrimaryKeyOrdinal:=1, xid:="SCT1", aliases:={"bs4"})> Public Const ConstFNType = "scheduletype"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2, _
           useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        ''' <summary>
        ''' fields
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
            title:="description", Description:="description of the schedule definition", _
            xid:="SCT2")> Public Const ConstFNDescription = "desc"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=True, dbdefaultvalue:="1", _
           title:="Auto Publish", Description:="publish automatically after each persist of edition", _
           xid:="SCT3")> Public Const ConstFNAutoPublish = "autopublish"

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
        title:="OPRule", description:="internal Operation / Method to call on Status Rules", XID:="SCT10")> _
        Public Const constFNRuleOperation = "RuleOperation"

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
       title:="RulezSet", description:="Rulez Ruleset to call on status rules", XID:="SCT11")> Public Const constFNRulezSet = "RuleSet"

        ''' <summary>
        ''' Mappings
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNType)> Private _scheduletype As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _description As String
        <ormObjectEntryMapping(EntryName:=ConstFNAutoPublish)> Private _autopublish As Boolean
        ''' <summary>
        ''' Relations
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(ScheduleMilestoneDefinition), cascadeOnDelete:=True, cascadeOnUpdate:=True, _
            fromEntries:={ConstFNType}, toEntries:={ScheduleMilestoneDefinition.ConstFNType})> Public Const ConstRMilestones = "Milestones"

        <ormObjectEntryMapping(RelationName:=ConstRMilestones, infuseMode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand, _
            keyentries:={ScheduleMilestoneDefinition.ConstFNID})> _
        Private WithEvents _milestoneCollection As New ormRelationCollection(Of ScheduleMilestoneDefinition)(Me, {ScheduleMilestoneDefinition.ConstFNID})


#Region "properties"
        ''' <summary>
        ''' Gets or sets the autopublish flag.
        ''' </summary>
        ''' <value>The autopublish.</value>
        Public Property Autopublish() As Boolean
            Get
                Return Me._autopublish
            End Get
            Set(value As Boolean)
                SetValue(ConstFNAutoPublish, value)
            End Set
        End Property

        ''' <summary>
        ''' gets the schedule Type ID 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        ReadOnly Property ID As String
            Get
                Return _scheduletype
            End Get
        End Property
        ''' <summary>
        ''' gets or sets the description
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description As String
            Get
                Return _description
            End Get
            Set(value As String)
                SetValue(ConstFNDescription, value)
            End Set
        End Property
        ''' <summary>
        ''' gets the Floag for no active Schedule
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property isNoSchedule() As Boolean
            Get
                If _milestoneCollection.Count > 0 Then
                    isNoSchedule = False
                Else
                    isNoSchedule = True
                End If
            End Get
        End Property
        ''' <summary>
        ''' returns the number of milestones
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NoMembers() As Long
            Get
                Return _milestoneCollection.Count
            End Get
        End Property

        ''' <summary>
        ''' returns the Milestones of this schedule definition
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Milestones As iormRelationalCollection(Of ScheduleMilestoneDefinition)
            Get
                Return _milestoneCollection
            End Get
        End Property
        ''' <summary>
        ''' gets the Milestones ordered by their ordinal
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property MilestonesOrderedByOrdinal As ICollection(Of ScheduleMilestoneDefinition)
            Get
                Return _milestoneCollection.OrderBy(Function(x) x.Ordinal)
            End Get
        End Property
#End Region

        ''' <summary>
        ''' returns the actual milestone which finishes the schedule
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetActualFinishID() As String()
            Return _milestoneCollection.Where(Function(x) x.IsActual = True And x.IsFinish = True).Select(Function(x) x.ID).ToArray
        End Function

        ''' <summary>
        ''' returns the actual milestone which finishes the schedule
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFCFinishID(Optional ofActualID As String = Nothing) As String()
            If ofActualID Is Nothing Then
                Return _milestoneCollection.Where(Function(x) x.IsActual = False And x.IsFinish = True).Select(Function(x) x.ID).ToArray
            Else
                Dim aList As New List(Of String)

                For Each aMilstone In _milestoneCollection
                    If Not aMilstone.IsActual AndAlso aMilstone.IsFinish AndAlso aMilstone.ActualOfFC = ofActualID Then
                        aList.Add(aMilstone.ID)
                    End If
                Next
                Return aList.ToArray
            End If

        End Function
        ''' <summary>
        ''' returns the maximum ordinal
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMaximumMilestoneOrdinal() As Long
            If _milestoneCollection.Count > 0 Then Return _milestoneCollection.Select(Function(x) x.Ordinal).Max
            Return 0
        End Function

        ''' <summary>
        ''' legacy function
        ''' </summary>
        ''' <param name="AliasID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMilestoneIDByAlias(AliasID As String) As String
            ' legacy code:
            '''

            ' load aliases
            'If aTableEntry.LoadBy(objectname:=aSchedule.TableID, entryname:=anEntry.ID) Then
            '    For Each m In aTableEntry.Aliases
            '        If _aliases.ContainsKey(key:=LCase(m)) Then
            '            Call _aliases.Remove(key:=LCase(m))
            '        End If
            '        Call _aliases.Add(key:=LCase(m), value:=anEntry.ID)
            '    Next m
            'End If

            Dim anObjectDefinition As ormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(ScheduleEdition.ConstObjectID)
            If anObjectDefinition IsNot Nothing Then
                For Each anEntry In anObjectDefinition.GetEntries
                    ''' return the XID = Milestone ID of the Compund
                    If anEntry.IsCompound AndAlso anEntry.Aliases.Contains(AliasID) Then Return anEntry.XID
                Next
            End If

            Return String.Empty
        End Function
        ''' <summary>
        ''' Retrieve
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal id As String, Optional domainid As String = Nothing, Optional forcereload As Boolean = False) As ScheduleDefinition
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Return RetrieveDataObject(Of ScheduleDefinition)(pkArray:={id.ToUpper, domainid}, domainID:=domainid, forceReload:=forcereload)
        End Function

        ''' <summary>
        ''' create the data object by primary key
        ''' </summary>
        ''' <param name="SCHEDULETYPE"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal id As String, Optional domainid As String = Nothing) As ScheduleDefinition
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim pkarray() As Object = {id.ToUpper, domainid}
            Return ormBusinessObject.CreateDataObject(Of ScheduleDefinition)(pkarray, domainID:=domainid, checkUnique:=True)
        End Function

    End Class

    ''' <summary>
    ''' schedule class
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(Version:=1, ID:=ScheduleEdition.ConstObjectID, modulename:=ConstModuleScheduling, _
        addDomainBehavior:=False, usecache:=True, AddDeleteFieldBehavior:=True, _
        Title:="Schedule", Description:="schedules for business objects")> _
    Public Class ScheduleEdition
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable
        Implements iormCloneable(Of ScheduleEdition)

        Public Const ConstObjectID = "ScheduleEdition"

        ''' <summary>
        ''' TableDefinition
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(Version:=2)> Public Const ConstPrimaryTableID = "TBLSCHEDULEEDITIONS"
        '** Indexes
        <ormIndex(columnname1:=ConstFNWorkspaceID, columnname2:=ConstFNUid, columnname3:=ConstFNVersion)> Public Const ConstIndexWS = "WORKSPACEID"
        <ormIndex(columnname1:=ConstFNUid, columnname2:=ConstFNVersion, columnname3:=ConstFNIsDeleted)> Public Const ConstIndexUID = "UIDINDEX"

        ''' <summary>
        ''' Keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Long, title:="unique ID", Description:="Unique ID of the schedule", _
            lowerrange:=0, _
            PrimaryKeyOrdinal:=1, XID:="SC2", aliases:={"SUID"})> Public Const ConstFNUid = "uid"
        <ormObjectEntry(Datatype:=otDataType.Long, title:="update count", Description:="Update count of the schedule", _
            lowerrange:=0, _
           PrimaryKeyOrdinal:=2, XID:="SC3", aliases:={"BS3"})> Public Const ConstFNVersion = "VERSION"


        ''' <summary>
        ''' fields
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Long, _
            title:="forecast count", Description:="number of forecast udates of this schedule" _
          )> Public Const ConstFNfcupdc = "fcupdc"

        <ormObjectEntry(referenceObjectEntry:=Workspace.ConstObjectID & "." & Workspace.ConstFNID, _
            Description:="workspaceID ID of the schedule", useforeignkey:=otForeignKeyImplementation.NativeDatabase, _
             foreignkeyProperties:={ForeignKeyProperty.OnDelete & "(" & ForeignKeyActionProperty.NOOP & ")", _
                                   ForeignKeyProperty.OnUpdate & "(" & ForeignKeyActionProperty.NOOP & ")"})> Public Const ConstFNWorkspaceID = Workspace.ConstFNID

        ' deactivate ForeignKEy we do not have this object in domains
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, _
            useforeignkey:=otForeignKeyImplementation.None)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, isnullable:=True, _
            title:="revision", Description:="revision of the schedule", _
            XID:="SC5")> Public Const ConstFNPlanRev = "plrev"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0", _
            title:="is frozen", Description:="schedule is frozen flag", _
            XID:="SC6")> Public Const ConstFNisfrozen = "isfrozen"

        <ormObjectEntry(referenceObjectEntry:=StatusItem.ConstObjectID & "." & StatusItem.constFNCode, _
            isnullable:=True, _
            title:="lifecycle status", Description:="lifecycle status of the schedule", _
            XID:="SC7")> Public Const ConstFNlcstatus = "lcstatus"

        <ormObjectEntry(referenceObjectEntry:=StatusItem.ConstObjectID & "." & StatusItem.constFNCode, _
            isnullable:=True, _
            title:="process status", Description:="process status of the schedule", _
            XID:="SC8")> Public Const ConstFNpstatus = "pstatus"

        <ormObjectEntry(Datatype:=otDataType.Timestamp, isnullable:=True, _
            title:="check timestamp", Description:="timestamp of check status of the schedule", _
            XID:="SC9")> Public Const ConstFNCheckedOn = "checkedon"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, isnullable:=True, _
            title:="planner", Description:="responsible planner of the schedule", _
            XID:="SC10")> Public Const ConstFNPlanner = "resp"

        <ormObjectEntry(Datatype:=otDataType.Memo, isnullable:=True, _
            title:="comment", Description:="comment of the schedule", _
            XID:="SC12", Defaultvalue:="", parameter:="")> Public Const ConstFNComment = "cmt"

        <ormObjectEntry(Datatype:=otDataType.Timestamp, isnullable:=True, _
            title:="last fc update", Description:="last forecast change of the schedule", _
            XID:="SC13")> Public Const ConstFNFCupdatedOn = "fcupdon"

        <ormObjectEntry(referenceObjectEntry:=Scheduling.ScheduleDefinition.ConstObjectID & "." & Scheduling.ScheduleDefinition.ConstFNType, _
            title:="type", Description:="type of the schedule", _
            XID:="SC14", aliases:={"BS4"}, isnullable:=True)> Public Const ConstFNTypeid = "typeid"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0", _
            title:="baseline flag", Description:="flag if the schedule is a baseline", _
            XID:="SC15")> Public Const ConstFNIsBaseline = "isbaseline"

        <ormObjectEntry(Datatype:=otDataType.Date, isnullable:=True, _
            title:="baseline date", Description:="date of the baseline creation", _
            XID:="SC16")> Public Const ConstFNBlDate = "bldate"

        <ormObjectEntry(Datatype:=otDataType.Long, isnullable:=True, _
            title:="baseline updc", Description:="updc of the last baseline of this schedule", _
            XID:="SC17")> Public Const ConstFNBlUpdc = "blupdc"

        <ormObjectEntry(Datatype:=otDataType.Numeric, isnullable:=True, _
            title:="required capacity", Description:="required capacity of this schedule", _
            XID:="SC20", aliases:={"WBS2"})> Public Const ConstFNRequCap = "requ"

        <ormObjectEntry(Datatype:=otDataType.Numeric, isnullable:=True, _
            title:="used capacity", Description:="used capacity of this schedule", _
            XID:="SC21", aliases:={"WBS3"}, Defaultvalue:="0")> Public Const ConstFNUsedCap = "used"

        <ormObjectEntry(Datatype:=otDataType.Date, isnullable:=True, _
            title:="used capacity reference date", Description:="used capacity reference date of this schedule", _
            XID:="SC22", aliases:={"WBS4"})> Public Const ConstFNUsedCapRef = "ufdt"



        ''' <summary>
        ''' Mappings
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNUid)> Private _uid As Long = 0
        <ormObjectEntryMapping(EntryName:=ConstFNVersion)> Private _updc As Long = 0
        <ormObjectEntryMapping(EntryName:=ConstFNfcupdc)> Private _fcupdc As Long    ' update count of just fc
        <ormObjectEntryMapping(EntryName:=ConstFNPlanRev)> Private _plrev As String
        <ormObjectEntryMapping(EntryName:=ConstFNPlanner)> Private _planner As String
        <ormObjectEntryMapping(EntryName:=ConstFNisfrozen)> Private _isFrozen As Boolean = False
        <ormObjectEntryMapping(EntryName:=ConstFNpstatus)> Private _ProcessStatusCode As String
        <ormObjectEntryMapping(EntryName:=ConstFNlcstatus)> Private _lfcstatuscode As String
        <ormObjectEntryMapping(EntryName:=ConstFNCheckedOn)> Private _checkedOn As Date?
        <ormObjectEntryMapping(EntryName:=ConstFNFCupdatedOn)> Private _fcUpdatedOn As Date?
        <ormObjectEntryMapping(EntryName:=ConstFNIsBaseline)> Private _isBaseline As Boolean = False
        <ormObjectEntryMapping(EntryName:=ConstFNBlDate)> Private _baselineDate As Date?
        <ormObjectEntryMapping(EntryName:=ConstFNBlUpdc)> Private _baselineUPDC As Long?

        <ormObjectEntryMapping(EntryName:=ConstFNWorkspaceID)> Private _workspace As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNTypeid)> Private _typeid As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNRequCap)> Private _requ As Double?
        <ormObjectEntryMapping(EntryName:=ConstFNUsedCap)> Private _used As Double?
        <ormObjectEntryMapping(EntryName:=ConstFNUsedCapRef)> Private _ufdt As Date?
        <ormObjectEntryMapping(EntryName:=ConstFNComment)> Private _comment As String

        ''' <summary>
        ''' Relation to schedule milestones
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(ScheduleMilestone), cascadeOnCreate:=True, cascadeOnDelete:=True, cascadeOnUpdate:=True, _
            fromEntries:={ConstFNUid}, toEntries:={ScheduleMilestone.ConstFNUid})> Public Const ConstRMilestones = "RELMILESTONES"

        <ormObjectEntryMapping(RelationName:=ConstRMilestones, infuseMode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand, _
            keyentries:={ScheduleMilestone.ConstFNID})> Private WithEvents _milestoneCollection As New ormRelationCollection(Of ScheduleMilestone)(Me, {ScheduleMilestone.ConstFNID})


        ''' <summary>
        ''' Relation to schedule definition
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(ScheduleDefinition), toprimarykeys:={ConstFNTypeid}, _
            cascadeOnCreate:=True, cascadeOnDelete:=False, cascadeOnUpdate:=False)> Public Const ConstRScheduleDefinition = "RELSCHEDULEDEFINITION"

        <ormObjectEntryMapping(RelationName:=ConstRScheduleDefinition, infuseMode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand)> Private WithEvents _scheduleDefinition As ScheduleDefinition

        ''' <summary>
        ''' Relation to LifeCycle StatusItem
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(StatusItem), fromentries:={ConstFNlcstatus}, _
            toentries:={StatusItem.constFNCode}, linkjoin:=" AND [" & StatusItem.constFNType & "] = '" & ConstStatusType_ScheduleLifecycle & "'", _
            cascadeOnCreate:=False, cascadeOnDelete:=False, cascadeOnUpdate:=False)> Public Const ConstRLifeCycleSatus = "RELLFCLSTATUS"

        <ormObjectEntryMapping(RelationName:=ConstRLifeCycleSatus, infuseMode:=otInfuseMode.OnDemand)> Private WithEvents _lifecylcestatus As StatusItem

        ''' <summary>
        ''' Relation to Process StatusItem
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(StatusItem), fromentries:={ConstFNlcstatus}, _
            toentries:={StatusItem.constFNCode}, linkjoin:=" AND [" & StatusItem.constFNType & "] = '" & ConstStatusType_ScheduleProcess & "'", _
            cascadeOnCreate:=False, cascadeOnDelete:=False, cascadeOnUpdate:=False)> Public Const ConstRProcessSatus = "RELPROCSTATUS"

        <ormObjectEntryMapping(RelationName:=ConstRProcessSatus, infuseMode:=otInfuseMode.OnDemand)> Private WithEvents _processstatus As StatusItem



        ' components itself per key:=id, item:=clsOTDBXScheduleMilestone
        'Private s_members As New Dictionary(Of String, ScheduleMilestone)
        Private _originalMilestoneValues As New Dictionary(Of String, Object)   'orgmembers -> original members before any change

        ' dynamic
        Private _haveMilestonesChanged As Boolean
        Private _isForeCastChanged As Boolean
        'Private s_milestones As New Dictionary -> superseded with members
        Private _loadedFromHost As Boolean
        Private _savedToHost As Boolean

        ''' <summary>
        ''' Define the constants for accessing the compounds
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstOPGetMileStoneValue = "GETMILESTONEVALUE"
        Public Const ConstOPSetMileStoneValue = "SETMILESTONEVALUE"

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            Call MyBase.New()
            AddHandler Me.OnPersisted, AddressOf Track.Track_OnPersisted

        End Sub
#Region "Properties"
        ''' <summary>
        ''' gets the UID of the schedule
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        ReadOnly Property Uid() As Long
            Get
                Return _uid
            End Get
        End Property
        ''' <summary>
        ''' gets or sets the comment for this schedule
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Comment() As String
            Get
                Return _comment
            End Get
            Set(value As String)
                SetValue(ConstFNComment, value)
            End Set
        End Property

        ''' <summary>
        ''' gets or sets the workspace for the schedule
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property WorkspaceID() As String
            Get
                Return _workspace

            End Get
            Set(value As String)
                SetValue(ConstFNWorkspaceID, value)
                ''' change the workspace also for all milestones !
                For Each aMilestone In _milestoneCollection
                    aMilestone.WorkspaceID = value
                Next
            End Set
        End Property
        ''' <summary>
        ''' gets the number of milestones in the schedule
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property NoMilestones() As Long
            Get
                Return _milestoneCollection.Count
            End Get
        End Property
        ''' <summary>
        ''' returns true if the the forecast is changed since Inject / last persist
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property IsForecastChanged() As Boolean
            Get
                Return _isForeCastChanged
            End Get
        End Property
        ''' <summary>
        ''' returns the type id of the schedule type of this schedule
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Typeid() As String
            Get
                Typeid = _typeid
            End Get
            Set(value As String)
                Dim defschedule As ScheduleDefinition
                ' set the internal defschedule link
                If LCase(_typeid) <> LCase(value) Then

                    defschedule = ScheduleDefinition.Retrieve(id:=value, domainid:=Me.DomainID)
                    If defschedule Is Nothing Then
                        Call CoreMessageHandler(message:="TypeID has not schedule defined", procedure:="Schedule.typeID", _
                                              argument:=value)
                    Else
                        SetValue(ConstFNTypeid, defschedule.ID)
                        ' load the milestones
                        If Not LoadMilestones(scheduletypeid:=defschedule.ID) Then
                            Call CoreMessageHandler(message:="Milestones of TypeID couldnot loaded", _
                                                  procedure:="Schedule.typeID let", _
                                                  argument:=value)
                        End If
                    End If

                End If

            End Set
        End Property
        ''' <summary>
        ''' retrieve the related Schedule Definition object
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ScheduleDefinition() As ScheduleDefinition
            Get
                If Not Me.IsAlive(subname:="ScheduleDefinition") Then Return Nothing

                InfuseRelation(ConstRScheduleDefinition)
                Return _scheduleDefinition
            End Get
        End Property

        ''' <summary>
        ''' get the loaded from Host Application flag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LoadedFromHost() As Boolean
            Get
                LoadedFromHost = _loadedFromHost
            End Get
            Set(value As Boolean)
                _loadedFromHost = value
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the required capaciyty
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RequiredCapacity() As Double?
            Get
                Return _requ
            End Get
            Set(value As Double?)
                SetValue(ConstFNRequCap, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the used capacity
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property UsedCapacity() As Double?
            Get
                Return _used
            End Get
            Set(value As Double?)
                SetValue(ConstFNUsedCap, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the used capacity reference date
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property UsedCapacityRefDate() As Date?
            Get
                Return _ufdt
            End Get
            Set(value As Date?)
                SetValue(ConstFNUsedCapRef, value)
            End Set
        End Property

        ''' <summary>
        ''' gets or sets the planning revision
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Revision() As String
            Get
                Return _plrev
            End Get
            Set(value As String)
                SetValue(ConstFNPlanRev, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the planner
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Planner() As String
            Get
                Return _planner
            End Get
            Set(value As String)
                SetValue(ConstFNPlanner, value)
            End Set
        End Property
        ''' <summary>
        ''' getrs or sets the process status code
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ProcessStatusCode() As String
            Get
                Return _ProcessStatusCode
            End Get
            Set(value As String)
                SetValue(ConstFNpstatus, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the process status item of the schedule
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ProcessStatus() As StatusItem
            Get
                If _processstatus Is Nothing OrElse _processstatus.Code <> _ProcessStatusCode Then InfuseRelation(ConstRLifeCycleSatus)
                Return _processstatus
            End Get
            Set(value As StatusItem)
                If value IsNot Nothing AndAlso value.TypeID = ConstStatusType_ScheduleProcess Then
                    Me.ProcessStatusCode = value.Code
                    _processstatus = value
                Else
                    Me.ProcessStatusCode = Nothing
                    _processstatus = Nothing
                End If
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the lifecycle status code of the schedule
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LifeCycleStatusCode() As String
            Get
                Return _lfcstatuscode
            End Get
            Set(value As String)
                SetValue(ConstFNlcstatus, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the lifecycle status of the schedule
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LifeCycleStatus() As StatusItem
            Get
                If _lifecylcestatus Is Nothing OrElse _lifecylcestatus.Code <> _lfcstatuscode Then InfuseRelation(ConstRLifeCycleSatus)
                Return _lifecylcestatus
            End Get
            Set(value As StatusItem)

                If value IsNot Nothing AndAlso value.TypeID = ConstStatusType_ScheduleLifecycle Then
                    Me.LifeCycleStatusCode = value.Code
                    _lifecylcestatus = value
                Else
                    Me.LifeCycleStatusCode = Nothing
                    _lifecylcestatus = Nothing
                End If
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the isfrozen flag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsFrozen() As Boolean
            Get
                Return _isFrozen
            End Get
            Set(value As Boolean)
                SetValue(ConstFNisfrozen, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the Baseline flag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsBaseline() As Boolean
            Get
                Return _isBaseline
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsBaseline, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the status checked date
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property StatusCheckedOn() As Date?
            Get
                Return _checkedOn
            End Get
            Set(value As Date?)
                SetValue(ConstFNCreatedOn, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the baseline reference date
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property BaselineRefDate() As Date?
            Get
                Return _baselineDate
            End Get
            Set(value As Date?)
                SetValue(ConstFNBlDate, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the baseline updc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property BaselineUPDC() As Long?
            Get
                Return _baselineUPDC
            End Get
            Set(value As Long?)
                SetValue(ConstFNBlUpdc, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the last forecast update date
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LastForecastUpdate() As Date?
            Get
                Return _fcUpdatedOn
            End Get
            Set(value As Date?)
                SetValue(ConstFNFCupdatedOn, value)
            End Set
        End Property
        ''' <summary>
        ''' gets the forecast update count
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property FCupdc() As Long
            Get
                Return _fcupdc
            End Get

        End Property
        ''' <summary>
        ''' gets the updc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Updc() As Long
            Get
                Return _updc
            End Get

        End Property


        ''' <summary>
        ''' true if a milestone was changed after last load / persist / publish
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property HaveMileStonesChanged() As Boolean
            Get
                Return _haveMilestonesChanged
            End Get
        End Property
        ''' <summary>
        ''' gets the Milestones of this schedule
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Milestones As iormRelationalCollection(Of ScheduleMilestone)
            Get
                Return _milestoneCollection
            End Get
        End Property
#End Region
        ''' <summary>
        ''' operation to Access the Milestone's Value
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ormObjectOperationMethod(operationname:=ConstOPGetMileStoneValue, tag:=ormObjectCompoundEntry.ConstCompoundGetter, _
            parameterEntries:={ormObjectCompoundEntry.ConstFNEntryName, ormObjectCompoundEntry.ConstFNValues})> _
        Public Function RetrieveMilestoneValue(id As String, ByRef value As Object) As Boolean
            If Not IsAlive(subname:="GetMilestoneValue") Then Return Nothing

            ''' return
            If Me.HasMilestone(id, hasData:=False) Then
                value = GetMilestoneValue(id)
                Return True
            Else
                Return False
            End If

        End Function



        ''' <summary>
        ''' retrieve the related Schedule Milestone Definition Object
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetScheduleMilestoneDefinition(ByVal ID As String) As ScheduleMilestoneDefinition
            Dim aDefScheduleMS As ScheduleMilestoneDefinition = ScheduleMilestoneDefinition.Retrieve(scheduletype:=_typeid, ID:=ID)
            If aDefScheduleMS Is Nothing Then
                Call CoreMessageHandler(message:="schedule milestone definition doesn't exist", _
                                      procedure:="Schedule.getDefScheduleMilestone", _
                                      argument:=_typeid & "-" & ID)
                aDefScheduleMS = Nothing
            End If

            Return aDefScheduleMS
        End Function


        '*** increment the updc version
        Public Function Incfcupdc() As Long
            _fcupdc = _fcupdc + 1
            Incfcupdc = _fcupdc
            Me.IsChanged = True
        End Function
        '****** getUniqueTag
        Public Function getUniqueTag()
            getUniqueTag = ConstDelimiter & ConstPrimaryTableID & ConstDelimiter & _uid & ConstDelimiter & _updc & ConstDelimiter
        End Function

        ''' <summary>
        ''' event handler for persisting this schedule
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub Request_Perist(sender As Object, e As ormDataObjectEventArgs)
            If Me.IsCreated OrElse Me.IsChanged Then Me.Persist(e.Timestamp)
        End Sub
        ''' <summary>
        ''' milestone returns the Milestone Value as object or Null if not exists
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <param name="ORIGINAL"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMilestoneValue(ByVal ID As String, Optional ORIGINAL As Boolean = False) As Object
            Dim aMember As New ScheduleMilestone
            Dim aDefSchedule As ScheduleDefinition = Me.ScheduleDefinition
            Dim aRealID As String
            ID = ID.ToUpper

            If Not IsAlive(subname:="GetMilestoneValue") Then Return Nothing

            ' check aliases
            If aDefSchedule Is Nothing Then
                Call CoreMessageHandler(message:="DefSchedule is not valid", argument:=Me.Typeid, procedure:="Schedule.getMilestone")
                Return Nothing
            End If

            aRealID = aDefSchedule.GetMilestoneIDByAlias(AliasID:=ID)
            If aRealID = String.Empty Then aRealID = ID
            '

            ' return not original
            If _milestoneCollection.ContainsKey({aRealID}) Then
                aMember = _milestoneCollection.Item({aRealID})
                If Not ORIGINAL Then
                    Return aMember.Value
                Else
                    Return _originalMilestoneValues.Item(aRealID)
                End If
            End If
            'If s_members.ContainsKey(key:=LCase(aRealID)) Then
            '    aMember = s_members.Item(key:=LCase(aRealID))
            '    If Not ORIGINAL Then
            '        Return aMember.Value
            '    ElseIf s_orgMSvalues.ContainsKey(LCase(aRealID)) Then
            '        Return s_orgMSvalues.Item(LCase(aRealID))
            '    Else
            '        Return Nothing
            '    End If

            'Else
            '    Return Nothing
            'End If

            Return Nothing
        End Function
        ''' <summary>
        ''' milestone returns the Milestone Value as object or Null if not exists
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <param name="ORIGINAL"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMilestone(ByVal ID As String) As ScheduleMilestone
            Dim aMember As New ScheduleMilestone
            Dim aDefSchedule As ScheduleDefinition = Me.ScheduleDefinition
            Dim aRealID As String
            ID = ID.ToUpper
            If Not IsAlive(subname:="getMilestone") Then Return Nothing

            If aDefSchedule Is Nothing Then
                Call CoreMessageHandler(message:="DefSchedule is not valid", argument:=Me.Typeid, procedure:="Schedule.getMilestone")
                Return Nothing
            End If

            aRealID = aDefSchedule.GetMilestoneIDByAlias(AliasID:=ID)
            If aRealID = String.Empty Then aRealID = ID


            ' return not original
            ' return not original
            If _milestoneCollection.ContainsKey({aRealID}) Then
                Return _milestoneCollection.Item({aRealID})
            End If

            'If s_members.ContainsKey(key:=LCase(aRealID)) Then
            '    aMember = s_members.Item(key:=LCase(aRealID))
            '    Return aMember
            'Else
            '    Return Nothing
            'End If

            Return Nothing
        End Function
        '******* setMilestone ID to Value
        '*******
        ''' <summary>
        ''' setMilestone ID to Value
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <param name="Value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ormObjectOperationMethod(operationname:=ConstOPSetMileStoneValue, tag:=ormObjectCompoundEntry.ConstCompoundSetter, _
            parameterEntries:={ormObjectCompoundEntry.ConstFNEntryName, ormObjectCompoundEntry.ConstFNValues})> _
        Public Function SetMilestoneValue(ByVal id As String, ByVal value As Object, Optional setNull As Boolean = False) As Boolean
            Dim aMember As New ScheduleMilestone
            Dim isMemberchanged As Boolean
            Dim aDefSchedule As ScheduleDefinition = Me.ScheduleDefinition
            Dim aRealID As String
            id = id.ToUpper

            If Not IsAlive(subname:="SetMilestoneValue") Then Return False

            ' check aliases
            If aDefSchedule Is Nothing Then
                CoreMessageHandler(message:="schedule definition was not found", argument:=Me.Typeid, procedure:="Schedule.SetMilestoneVlue")
                Return False
            End If

            ''' load milestones
            ''' 
            LoadMilestones(scheduletypeid:=Me.Typeid)

            aRealID = aDefSchedule.GetMilestoneIDByAlias(AliasID:=id)
            If aRealID = String.Empty Then aRealID = id


            If _milestoneCollection.ContainsKey({aRealID}) Then
                aMember = _milestoneCollection.Item({aRealID})
            Else
                aMember = ScheduleMilestone.Create(UID:=Me.Uid, updc:=Me.Updc, ID:=aRealID, domainid:=Me.DomainID, workspaceid:=Me.WorkspaceID)
                If aMember Is Nothing Then aMember = ScheduleMilestone.Retrieve(UID:=Me.Uid, updc:=Me.Updc, ID:=aRealID)
                If aMember Is Nothing Then
                    Call CoreMessageHandler(argument:=id, procedure:="Schedule.setMilestone", containerID:=ConstPrimaryTableID, _
                                          message:="ID doesnot exist in Milestone Entries")
                    Return False
                End If

            End If

            isMemberchanged = False


            ' if the Member is only a Cache ?!
            If aMember.IsCacheNoSave Then
                Call CoreMessageHandler(message:="setMilestone to cached Item", procedure:="Schedule.setMilestone", messagetype:=otCoreMessageType.ApplicationError, _
                                      argument:=LCase(id) & ":" & CStr(value))
                Return False
            End If

            ' convert it
            If (aMember.Datatype = otDataType.[Date] Or aMember.Datatype = otDataType.Timestamp) Then
                If IsDate(value) And Not setNull Then
                    If aMember.Value <> CDate(value) Then
                        aMember.Value = CDate(value)
                        isMemberchanged = True
                    End If
                ElseIf setNull Then
                    If aMember.Value IsNot Nothing OrElse aMember.Value <> ConstNullDate Then
                        aMember.Value = Nothing
                        isMemberchanged = True
                    End If
                ElseIf value Is Nothing Then
                    If aMember.Value IsNot Nothing Then
                        aMember.Value = Nothing
                        isMemberchanged = True
                    End If
                Else
                    Call CoreMessageHandler(message:="milestone of date cannot set to", procedure:="Schedule.setMilestone", _
                                                         argument:=LCase(id) & ":" & CStr(value), messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If

            ElseIf aMember.Datatype = otDataType.Numeric Then
                If IsNumeric(value) And Not setNull Then
                    If aMember.Value <> CDbl(value) Then
                        aMember.Value = CDbl(value)
                        isMemberchanged = True
                    End If
                ElseIf setNull Then
                    If aMember.Value <> 0 Then
                        aMember.Value = 0
                        isMemberchanged = True
                    End If
                ElseIf value Is Nothing Then
                    If aMember.Value IsNot Nothing Then
                        aMember.Value = Nothing
                        isMemberchanged = True
                    End If
                Else
                    Call CoreMessageHandler(message:="milestone of numeric cannot set to", procedure:="Schedule.setMilestone", _
                                                        argument:=LCase(id) & ":" & CStr(value), messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If

            ElseIf aMember.Datatype = otDataType.[Long] Then
                If IsNumeric(value) And Not setNull Then
                    If aMember.Value <> CLng(value) Then
                        aMember.Value = CLng(value)
                        isMemberchanged = True
                    End If
                ElseIf setNull Then
                    If aMember.Value <> 0 Then
                        aMember.Value = 0
                        isMemberchanged = True
                    End If
                ElseIf value Is Nothing Then
                    If aMember.Value IsNot Nothing Then
                        aMember.Value = Nothing
                        isMemberchanged = True
                    End If
                Else
                    Call CoreMessageHandler(message:="milestone of long cannot set to", procedure:="Schedule.setMilestone", _
                                                        argument:=LCase(id) & ":" & CStr(value), messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If

            ElseIf aMember.Datatype = otDataType.Bool Then
                If Not setNull Then
                    If aMember.Value <> CBool(value) Then
                        aMember.Value = CBool(value)
                        isMemberchanged = True
                    End If
                ElseIf setNull Then
                    If aMember.Value <> False Then
                        aMember.Value = False
                        isMemberchanged = True
                    End If
                ElseIf value Is Nothing Then
                    If aMember.Value IsNot Nothing Then
                        aMember.Value = Nothing
                        isMemberchanged = True
                    End If
                Else
                    Call CoreMessageHandler(message:="milestone of bool cannot set to", procedure:="Schedule.setMilestone", _
                                                        argument:=LCase(id) & ":" & CStr(value), messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If

            Else
                If Not setNull Then
                    If aMember.Value <> CStr(value) Then
                        aMember.Value = CStr(value)
                        isMemberchanged = True
                    End If
                ElseIf setNull Then
                    If String.IsNullOrEmpty(aMember.Value) Then
                        aMember.Value = CStr(value)
                        isMemberchanged = True
                    End If
                ElseIf value Is Nothing Then
                    If aMember.Value IsNot Nothing Then
                        aMember.Value = Nothing
                        isMemberchanged = True
                    End If
                Else
                    Call CoreMessageHandler(message:="milestone of string cannot set to", procedure:="Schedule.setMilestone", _
                                                        argument:=LCase(id) & ":" & CStr(value), messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If

            End If


            ' save it to dictionary
            ' get Member
            If isMemberchanged Then
                'Call s_members.add(Key:=LCase(aRealID), Item:=aMember) -> should be ok since referenced
                _haveMilestonesChanged = True
                aMember.WorkspaceID = Me.WorkspaceID
                If aMember.IsForecast Then
                    _isForeCastChanged = True
                End If
                Return True
            Else
                Return True
            End If

            Return False

        End Function

        '******** moveMilestone
        '********
        ''' <summary>
        ''' move the milestone in date range
        ''' </summary>
        ''' <param name="noDays"></param>
        ''' <param name="MSID"></param>
        ''' <param name="considerWorkingDays"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MoveMilestone(ByVal noDays As Long, _
                        ByVal MSID As String, _
                        Optional considerWorkingDays As Boolean = True) As Boolean
            Dim aScheduleMSDef As New ScheduleMilestoneDefinition
            Dim aScheduleMSDefColl As New List(Of ScheduleMilestoneDefinition)
            Dim aCE As New CalendarEntry
            Dim flag As Boolean
            Dim aDate As Object
            Dim actDate As Object

            If Not Me.IsAlive(subname:="MoveMilestone") Then Return False
            If Me.IsFinished Then Return False
            aScheduleMSDef = ScheduleMilestoneDefinition.Retrieve(scheduletype:=Me.Typeid, ID:=MSID, domainID:=Me.DomainID)
            If aScheduleMSDef Is Nothing Then Return False

            ' if we have a forecast -> need to look for the actual
            If aScheduleMSDef.IsForecast Then
                aScheduleMSDefColl = GetDefScheduleMSbyOrder(justDates:=True)
                If aScheduleMSDefColl Is Nothing Or aScheduleMSDefColl.Count = 0 Then Return False
                flag = False
                For Each aScheduleMSDef In aScheduleMSDefColl
                    If (Not String.IsNullOrWhiteSpace(MSID) AndAlso aScheduleMSDef.ActualOfFC = LCase(MSID)) AndAlso Not String.IsNullOrEmpty(aScheduleMSDef.ActualOfFC) Then
                        flag = True
                        Exit For
                    End If
                Next aScheduleMSDef
                If Not flag Then Return False
            End If
            'actual found -> checkit
            actDate = Me.GetMilestoneValue(aScheduleMSDef.ID)
            aDate = Me.GetMilestoneValue(aScheduleMSDef.ActualOfFC)
            If IsDate(aDate) AndAlso aDate <> constNullDate AndAlso _
                ((IsDate(actDate) AndAlso actDate = constNullDate) OrElse actDate Is Nothing) AndAlso _
                Not String.IsNullOrEmpty(aScheduleMSDef.ActualOfFC) AndAlso Not String.IsNullOrEmpty(aScheduleMSDef.ID) Then
                ' only if there is this milestone
                aCE.Timestamp = aDate
                aDate = aCE.AddDay(noDays, considerAvailibilty:=considerWorkingDays, calendarname:=CurrentSession.DefaultCalendarName)
                Call Me.SetMilestoneValue(aScheduleMSDef.ActualOfFC, aDate)
                '*******
                '******* we need to check ascending condition !!

                Return True
            End If

            Return False
        End Function

        '******** moveSchedule
        '********
        ''' <summary>
        ''' move the full schedule in date range
        ''' </summary>
        ''' <param name="noDays"></param>
        ''' <param name="startMS"></param>
        ''' <param name="considerWorkingDays"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MoveSchedule(ByVal noDays As Long, _
         ByVal startMS As String, _
        Optional considerWorkingDays As Boolean = True) As Boolean
            Dim aScheduleMSDefColl As New List(Of ScheduleMilestoneDefinition)
            Dim aScheduleMSDef As New ScheduleMilestoneDefinition
            Dim aCE As New CalendarEntry
            Dim started As Boolean
            Dim aDate As Object
            Dim actDate As Object

            If Not Me.IsLoaded And Not Me.IsCreated Then
                MoveSchedule = False
                Exit Function
            End If

            If Me.IsFinished Then
                MoveSchedule = False
                Exit Function
            End If

            aScheduleMSDefColl = GetDefScheduleMSbyOrder(justDates:=True)
            If aScheduleMSDefColl Is Nothing Or aScheduleMSDefColl.Count = 0 Then
                MoveSchedule = False
                Exit Function
            End If

            started = False
            ' go through the milestones in order and move them if they are not actual
            For Each aScheduleMSDef In aScheduleMSDefColl
                If aScheduleMSDef.ID = LCase(startMS) Or (aScheduleMSDef.ActualOfFC = LCase(startMS) And aScheduleMSDef.ActualOfFC <> String.Empty) Or startMS = String.Empty Then
                    started = True
                End If
                If Not aScheduleMSDef.IsForecast And started Then
                    ' no actual found -> calculate on the fc
                    actDate = Me.GetMilestoneValue(aScheduleMSDef.ID)
                    aDate = Me.GetMilestoneValue(aScheduleMSDef.ActualOfFC)
                    If aDate <> constNullDate And IsDate(aDate) And _
                    actDate = constNullDate And IsDate(actDate) And aScheduleMSDef.ActualOfFC <> String.Empty And aScheduleMSDef.ID <> String.Empty Then
                        ' only if there is this milestone
                        aCE.Timestamp = aDate
                        aDate = aCE.AddDay(noDays, considerAvailibilty:=considerWorkingDays, calendarname:=CurrentSession.DefaultCalendarName)
                        Call Me.SetMilestoneValue(aScheduleMSDef.ActualOfFC, aDate)
                    End If
                End If
            Next aScheduleMSDef
            ' move it
            MoveSchedule = True
        End Function
        '******** getDefScheduleMSbyOrder returns a Collection of ScheduleMilestones by Orderno
        '********
        ''' <summary>
        ''' getDefScheduleMSbyOrder returns a Collection of ScheduleMilestones by Orderno
        ''' </summary>
        ''' <param name="justDates"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDefScheduleMSbyOrder(Optional justDates As Boolean = True) As List(Of ScheduleMilestoneDefinition)
            Dim aScheduleDef As New ScheduleDefinition
            Dim atypeid As String
            Dim aDeliverableTrack As New Track
            Dim aCollection As New List(Of ScheduleMilestoneDefinition)
            Dim aMSDefCollection As New Collection
            Dim aScheduleMSDef As New ScheduleMilestoneDefinition
            Dim aMilestoneDef As New MileStoneDefinition

            If Not Me.IsLoaded And Not Me.IsCreated Then
                GetDefScheduleMSbyOrder = Nothing
                Exit Function
            End If

            If Me.Typeid = String.Empty Then
                atypeid = CurrentSession.DefaultScheduleTypeID
            Else
                atypeid = Me.Typeid
            End If

            aScheduleDef = ScheduleDefinition.Retrieve(id:=atypeid, domainid:=Me.DomainID)
            If aScheduleDef Is Nothing Then
                Call CoreMessageHandler(procedure:="Schedule.getDefScheduleMSbyOrder", message:=" scheduletype of '" & atypeid & "' not found", argument:=atypeid)
                Return Nothing
            Else
                aMSDefCollection = aScheduleDef.Milestones     ' should be in the order
                If aMSDefCollection Is Nothing Or aMSDefCollection.Count = 0 Then
                    GetDefScheduleMSbyOrder = Nothing
                    Exit Function
                End If
                ' go through
                For Each aScheduleMSDef In aMSDefCollection
                    aMilestoneDef = MileStoneDefinition.Retrieve(id:=aScheduleMSDef.ID, domainid:=Me.DomainID)
                    If aMilestoneDef IsNot Nothing Then
                        If (aMilestoneDef.Datatype = otMilestoneType.Status And Not justDates) Or justDates Then
                            Call aCollection.Add(item:=aScheduleMSDef)
                        End If
                    Else
                        Call CoreMessageHandler(procedure:="Schedule.getDefScheduleMSbyOrder", message:=" milestone with id '" & aScheduleMSDef.ID & "' not found", argument:=atypeid)
                    End If

                Next aScheduleMSDef
            End If
            ' return value
            GetDefScheduleMSbyOrder = aCollection
        End Function

        ''' <summary>
        ''' return a collection of all schedule editions of a schedule uid
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function AllByUID(UID As Long) As ormRelationCollection(Of ScheduleEdition)
            Dim aCollection As ormRelationCollection(Of ScheduleEdition) = New ormRelationCollection(Of ScheduleEdition)(Nothing, keyentrynames:={ConstFNVersion})
            Dim aRecordCollection As List(Of ormRecord)
            Dim aStore As iormRelationalTableStore
            Dim aRecord As ormRecord

            Try
                aStore = GetPrimaryTableStore(ConstPrimaryTableID)
                Dim pkarray() As Object = {UID}
                aRecordCollection = aStore.GetRecordsByIndex(ConstIndexUID, pkarray, True)

                If Not aRecordCollection Is Nothing Then
                    For Each aRecord In aRecordCollection
                        Dim aNewSchedule As New ScheduleEdition
                        If InfuseDataObject(record:=aRecord, dataobject:=aNewSchedule) Then
                            aCollection.Add(item:=aNewSchedule)
                        End If
                    Next aRecord
                End If
                Return aCollection
            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="Schedule.AllByUID")
                Return aCollection
            End Try

        End Function

        ''' <summary>
        ''' load all Milestones as Members -> look for Actuals access
        ''' </summary>
        ''' <param name="scheduletypeid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function LoadMilestones(ByVal scheduletypeid As String) As Boolean
            If Not IsAlive(subname:="LoadMilestones") Then Return False

            ''' load the milestones
            If Not InfuseRelation(id:=ConstRMilestones) Then
                CoreMessageHandler(message:="could not load and infuse the milestones for this schedule #" & _uid & "." & _updc, _
                                    messagetype:=otCoreMessageType.InternalError, argument:=Me.Typeid)
            End If
            Return True
        End Function

        ''' <summary>
        ''' handles the defaults needed event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ScheduleEdition_OnDefaultValuesNeeded(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreateDefaultValuesNeeded

            If Not e.Record.HasIndex(ConstFNWorkspaceID) OrElse String.IsNullOrWhiteSpace(e.Record.GetValue(ConstFNWorkspaceID)) Then
                e.Record.SetValue(ConstFNWorkspaceID, CurrentSession.CurrentWorkspaceID)
            End If
            If Not e.Record.HasIndex(ConstFNDomainID) OrElse String.IsNullOrWhiteSpace(e.Record.GetValue(ConstFNDomainID)) Then
                e.Record.SetValue(ConstFNDomainID, CurrentSession.CurrentDomainID)
            End If
        End Sub
        ''' <summary>
        ''' event handler for relation loaded
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub Schedule_OnRelationLoad(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnRelationLoad

            If e.RelationIDs.Contains(ConstRMilestones.ToUpper) Then
                Dim CurrenWorkspace As Workspace = Workspace.Retrieve(Me.WorkspaceID)
                Dim aCurrSCHEDULE As New WorkspaceSchedule
                Dim anUpdc As Long
                Dim isCache As Boolean
                Dim aWSID As String
                Dim meme = TryCast(e.DataObject, ScheduleEdition)

                If meme Is Nothing Then
                    CoreMessageHandler(message:="data object could not be cast to Schedule", procedure:="Schedule_OnRelationload", messagetype:=otCoreMessageType.InternalError)
                    Exit Sub
                End If
                Dim aScheduleDefinition As ScheduleDefinition = ScheduleDefinition.Retrieve(id:=meme.Typeid, domainid:=meme.DomainID)
                If aScheduleDefinition Is Nothing Then
                    CoreMessageHandler(message:="schedule definition could not be retrieved", argument:=meme.Typeid, _
                                       procedure:="Schedule_OnRelationload", messagetype:=otCoreMessageType.InternalError)
                    Exit Sub
                End If

                Dim aCollection As iormRelationalCollection(Of ScheduleMilestoneDefinition) = aScheduleDefinition.Milestones

                ''' reworked to load really the actuals from the actual workspace
                ''' disable the milestones which are not set int his schedule
                For Each aMilestone In meme.Milestones

                    If Not aCollection.ContainsKey({aMilestone.ID}) Then
                        aMilestone.IsEnabled = False
                    Else
                        Dim aScheduleMSDef As ScheduleMilestoneDefinition = aCollection.Item({aMilestone.ID})
                        Dim aMSDef As MileStoneDefinition = MileStoneDefinition.Retrieve(aScheduleMSDef.ID, domainid:=Me.DomainID)

                        If Not aScheduleMSDef.IsProhibited AndAlso aMSDef IsNot Nothing Then
                            isCache = False
                            ' check if actuals are kept in this workspaceID
                            If Not CurrenWorkspace.HasActuals And aScheduleMSDef.IsActual Then
                                anUpdc = 0
                                isCache = True    ' find or not we are true
                                ' search for the next wspace in stack with actuals
                                For Each aWSID In CurrenWorkspace.ACTRelyingOn
                                    Dim aWS As Workspace = Workspace.Retrieve(aWSID)
                                    If Not aWS Is Nothing Then
                                        If aWS.HasActuals Then
                                            ' load the current
                                            aCurrSCHEDULE = WorkspaceSchedule.RetrieveUnique(UID:=_uid, workspaceID:=aWSID)
                                            If aCurrSCHEDULE IsNot Nothing Then
                                                anUpdc = aCurrSCHEDULE.AliveEditionUpdc
                                                Exit For
                                            End If
                                        End If
                                    End If
                                Next
                                '** load the actual milestone
                                Dim anotherMilestone As ScheduleMilestone = ScheduleMilestone.Retrieve(UID:=_uid, updc:=anUpdc, ID:=aScheduleMSDef.ID)
                                aMilestone.IsCacheNoSave = True
                                aMilestone.Value = anotherMilestone.Value

                            End If    ' actuals

                        Else
                            aMilestone.IsEnabled = False
                        End If
                    End If
                Next

            End If

        End Sub
        ''' <summary>
        ''' handles the OnInfused Event - load the milestones
        ''' </summary>
        ''' <param name="aRecord"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Sub OnInfused(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnInfused

            Try
                _haveMilestonesChanged = False
            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="Schedule.Infuse")
            End Try

        End Sub

        ''' <summary>
        ''' Event Handler for the Added Milestones
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub Schedule_OnMilestoneAdded(sender As Object, e As ormRelationCollection(Of ScheduleMilestone).EventArgs) Handles _milestoneCollection.OnAdded
            If Not IsAlive(subname:="Schedule_ONAdded") Then
                e.Cancel = True
                Exit Sub
            End If

            '** save original values
            If Not _originalMilestoneValues.ContainsKey(key:=e.Dataobject.ID) Then
                _originalMilestoneValues.Add(key:=e.Dataobject.ID, value:=e.Dataobject.Value)
            End If
        End Sub


        ''' <summary>
        ''' loads an schedule from store
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <param name="VERSION"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal UID As Long, ByVal updc As Long) As ScheduleEdition
            Return RetrieveDataObject(Of ScheduleEdition)(pkArray:={UID, updc})
        End Function

        ''' <summary>
        ''' creates all the default milestones for this schedule dependend on the schedule type
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CreateDefaultMilestones() As Boolean
            Dim CurrenWorkspace As Workspace = Workspace.Retrieve(Me.WorkspaceID)
            Dim aCurrSCHEDULE As New WorkspaceSchedule
            Dim anUpdc As Long
            Dim isCache As Boolean
            Dim aWSID As String

            If _typeid Is Nothing OrElse _typeid = String.Empty Then
                CoreMessageHandler(message:="schedule type of this schedule is not set - can not create default milestones", _
                                    argument:=_uid, messagetype:=otCoreMessageType.ApplicationError, procedure:="Schedule.CreateDefaultMilestones")
                Return False
            End If

            Dim aSchedule As ScheduleDefinition = ScheduleDefinition.Retrieve(id:=Me.Typeid, domainid:=Me.DomainID)
            If aSchedule Is Nothing Then
                CoreMessageHandler(message:="schedule type of this schedule could not be retrieved - can not create default milestones", _
                                   argument:=_typeid, messagetype:=otCoreMessageType.ApplicationError, procedure:="Schedule.CreateDefaultMilestones")
                Return False
            End If

            Dim aCollection As iormRelationalCollection(Of ScheduleMilestoneDefinition) = aSchedule.Milestones
            ''' switch off all not used in the schedule
            ''' 
            For Each aMilestone In Me.Milestones
                If Not aCollection.ContainsKey(aMilestone.ID) Then
                    aMilestone.IsEnabled = False
                End If
            Next

            ''' check the ones in the definition - create them or overload the actuals if necessary
            ''' 
            For Each aScheduleMSDef In aCollection
                ' get the milestone definition
                Dim aMSDef As MileStoneDefinition = MileStoneDefinition.Retrieve(aScheduleMSDef.ID, domainid:=Me.DomainID)

                If Not aScheduleMSDef.IsProhibited AndAlso aMSDef IsNot Nothing Then
                    isCache = False
                    ' check if actuals are kept in this workspaceID
                    If Not CurrenWorkspace.HasActuals AndAlso aScheduleMSDef.IsActual Then
                        anUpdc = 0
                        isCache = True    ' find or not we are true
                        ' search for the next wspace in stack with actuals
                        For Each aWSID In CurrenWorkspace.ACTRelyingOn
                            Dim aWS As Workspace = Workspace.Retrieve(aWSID)
                            If Not aWS Is Nothing Then
                                If aWS.HasActuals Then
                                    ' load the current
                                    aCurrSCHEDULE = WorkspaceSchedule.RetrieveUnique(UID:=_uid, workspaceID:=aWSID)
                                    If aCurrSCHEDULE IsNot Nothing Then
                                        anUpdc = aCurrSCHEDULE.AliveEditionUpdc
                                        Exit For
                                    End If
                                End If
                            End If
                        Next
                    Else
                        anUpdc = Me.Updc
                        isCache = False
                    End If    ' actuals

                    '** load the milestone
                    Dim aMilestone As ScheduleMilestone
                    If Not isCache Then
                        '' create
                        aMilestone = ScheduleMilestone.Create(UID:=_uid, updc:=anUpdc, ID:=aScheduleMSDef.ID, domainid:=Me.DomainID, workspaceid:=Me.WorkspaceID)
                        '' retrieve
                        If aMilestone Is Nothing Then
                            aMilestone = ScheduleMilestone.Retrieve(UID:=_uid, updc:=anUpdc, ID:=aScheduleMSDef.ID)
                        End If

                    Else
                        '' retrieve
                        aMilestone = ScheduleMilestone.Retrieve(UID:=_uid, updc:=anUpdc, ID:=aScheduleMSDef.ID)
                    End If

                    If aMilestone IsNot Nothing Then
                        ' iscache must be kept
                        aMilestone.IsCacheNoSave = isCache
                        '** include
                        _milestoneCollection.Add(aMilestone)
                    Else
                        CoreMessageHandler(message:="Milestone for uid #" & _uid & " from definition '" & aScheduleMSDef.ScheduleTypeID & "' could not be created or retrieved", _
                                           argument:=aScheduleMSDef.ID, containerID:=ConstPrimaryTableID, procedure:="Schedule.CreateDEfaultMilestones", _
                                            messagetype:=otCoreMessageType.ApplicationError)
                    End If

                End If

            Next aScheduleMSDef

            Return True
        End Function
        ''' <summary>
        ''' handles the OnCreated Event to create also all the milestones dependend on the schedule type
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub Schedule_OnCreated(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnCreated
            ''' create the Milestones
            Call Me.CreateDefaultMilestones()
        End Sub
        ''' <summary>
        ''' Property Change Handler
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub Schedule_OnEntryChanged(sender As Object, e As ormDataObjectEntryEventArgs) Handles MyBase.OnEntryChanged
            If e.ObjectEntryName = ConstFNTypeid Then
                CreateDefaultMilestones()
            End If
        End Sub
        ''' <summary>
        ''' handles the OnCreated Event to create also all the milestones dependend on the schedule type
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub Schedule_OnCreating(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnCreating
            Dim anUpdc As Long? = e.Record.GetValue(ConstFNVersion)
            Dim anuid As Long? = e.Record.GetValue(ConstFNUid)
            Dim aWorkspaceID As String = e.Record.GetValue(ConstFNWorkspaceID)

            '* new updc key ?!
            If Not anUpdc.HasValue OrElse anUpdc = 0 Then
                If Not Me.GetMaxUpdc(uid:=anuid, max:=anUpdc.Value, workspaceID:=aWorkspaceID) Then
                    Call CoreMessageHandler(message:=" primary key values could not be created - cannot create object", _
                                            procedure:="Schedule.create", containerID:=ObjectPrimaryTableID, messagetype:=otCoreMessageType.InternalError)
                    e.AbortOperation = True
                    Exit Sub
                End If
                '* increase
                anUpdc += 1
                e.Record.SetValue(ConstFNVersion, anUpdc)
            End If
        End Sub
        ''' <summary>
        ''' create a persistable schedule edition
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <param name=ConstFNVersion></param>
        ''' <param name="workspaceID"></param>
        ''' <param name="SCHEDULETYPEID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal uid As Long, _
                                Optional ByVal updc As Long = 0, _
                                Optional ByVal workspaceID As String = Nothing, _
                                Optional ByVal scheduletypeid As String = Nothing, _
                                Optional ByVal domainid As String = Nothing) As ScheduleEdition


            If String.IsNullOrEmpty(workspaceID) Then workspaceID = CurrentSession.CurrentWorkspaceID
            If String.IsNullOrWhiteSpace(scheduletypeid) Then scheduletypeid = CurrentSession.DefaultScheduleTypeID
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim aRecord As New ormRecord
            With aRecord
                .SetValue(ConstFNUid, uid)
                .SetValue(ConstFNVersion, updc)
                If Not String.IsNullOrEmpty(workspaceID) Then .SetValue(ConstFNWorkspaceID, workspaceID)
                If Not String.IsNullOrWhiteSpace(scheduletypeid) Then .SetValue(ConstFNTypeid, scheduletypeid)
                If Not String.IsNullOrEmpty(domainid) Then .SetValue(ConstFNDomainID, domainid)
            End With

            Return ormBusinessObject.CreateDataObject(Of ScheduleEdition)(aRecord, checkUnique:=True, domainID:=domainid)

        End Function



        '******* existsMilestone: checks if the Milestone by ID exists and is Of Type
        '*******
        ''' <summary>
        ''' if the milestone by id exists
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <param name="MSTYPEID"></param>
        ''' <param name="HASDATA"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasMilestone(ByVal ID As String, _
                                     Optional ByVal mstypeid As otMilestoneType = 0, _
                                     Optional ByVal hasData As Boolean = True) As Boolean

            If Not Me.IsAlive("HasMilestone") Then Return False

            Dim aVAlue As Object
            Dim aDefSchedule As ScheduleDefinition = Me.ScheduleDefinition
            Dim aRealID As String = String.Empty
            Dim aScheduleMilestone As ScheduleMilestone

            ID = ID.ToUpper
            aRealID = aDefSchedule.GetMilestoneIDByAlias(AliasID:=ID)
            If aRealID = String.Empty Then aRealID = ID
            Dim aDefMilestone As MileStoneDefinition = MileStoneDefinition.Retrieve(id:=aRealID, domainid:=Me.DomainID)

            ' check aliases
            If aDefSchedule Is Nothing Then
                Call CoreMessageHandler(message:="DefSchedule is not valid", argument:=Me.Typeid, procedure:="Schedule.getMilestone")
                Return False
            End If

            ''' load milestones
            ''' 
            LoadMilestones(Me.Typeid)

            ' get the DefSchedule Milestone
            ' if mstypeid is missing
            If mstypeid = 0 And aDefMilestone IsNot Nothing Then
                mstypeid = aDefMilestone.Typeid
            End If

            ' if milestone exists in members
            If _milestoneCollection.ContainsKey({aRealID}) Then
                aScheduleMilestone = _milestoneCollection.Item({aRealID})
                aVAlue = aScheduleMilestone.Value

                Select Case mstypeid

                    ' check date
                    Case otMilestoneType.[Date]
                        If IsDate(aVAlue) Then
                            If hasData And aVAlue <> constNullDate Then
                                HasMilestone = True
                            ElseIf Not hasData Then
                                HasMilestone = True
                            Else
                                HasMilestone = False
                            End If
                        ElseIf Not hasData Then
                            HasMilestone = True
                        Else
                            HasMilestone = False
                        End If
                        '
                        ' check status
                    Case otMilestoneType.Status
                        If Trim(CStr(aVAlue)) <> String.Empty And hasData Then
                            HasMilestone = True

                        ElseIf Trim(CStr(aVAlue)) = String.Empty And hasData Then
                            HasMilestone = False
                        ElseIf Not hasData Then
                            HasMilestone = True
                        Else
                            HasMilestone = True
                        End If
                End Select

            Else
                HasMilestone = False
                Exit Function
            End If

        End Function
        ''' <summary>
        ''' has the Milestone date data ?!
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasMilestoneDate(ByVal ID As String) As Boolean
            Return Me.HasMilestone(ID:=ID, mstypeid:=otMilestoneType.[Date], hasData:=True)
        End Function
        ''' <summary>
        ''' returns true if the milestone has no data or does not exist (optional ifNotExists)
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <param name="ifNotExists"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsMilestoneValueMissing(ByVal ID As String, Optional ByVal ifNotExists As Boolean = True) As Boolean
            ' check milestone on data 
            If Not Me.HasMilestone(ID:=ID, hasData:=True) Then
                Return ifNotExists
            Else
                Return False ' false = not missing value
            End If

        End Function
        ''' <summary>
        ''' returns the finish forecast date of the schedule
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FinishOn() As Date?
            If Not Me.IsAlive("FinishedOn") Then Return Nothing
            Dim aList As String() = Me.ScheduleDefinition.GetFCFinishID

            '' search for the actual of the planned end
            '' if this has data (a date) then it must be an end
            For Each anID In aList
                If Me.HasMilestone(ID:=anID, hasData:=False) Then
                    Dim aMilestone As ScheduleMilestoneDefinition = Me.ScheduleDefinition.Milestones.Item(anID)
                    Dim aMDef As MileStoneDefinition = aMilestone.GetMilestoneDefinition()
                    If aMilestone IsNot Nothing AndAlso aMDef IsNot Nothing Then
                        Dim avalue As Object = Me.GetMilestoneValue(anID)
                        If aMDef.IsDate Then
                            Return avalue
                        Else
                            ''' here we should check on some status
                            ''' 
                            Throw New NotImplementedException("Finishing on Status is not implemented")
                        End If
                    ElseIf aMilestone Is Nothing Then
                        CoreMessageHandler(message:="milestone schedule definition could not be retrieved", argument:=anID, messagetype:=otCoreMessageType.ApplicationError, _
                                            procedure:="ScheduleEdition.FinishOn", objectname:=Me.ObjectID)
                    ElseIf aMDef Is Nothing Then
                        CoreMessageHandler(message:="milestone definition could not be retrieved", argument:=aMilestone.ActualOfFC, messagetype:=otCoreMessageType.ApplicationError, _
                                            procedure:="ScheduleEdition.FinishOn", objectname:=Me.ObjectID)
                    End If
                End If
            Next

            Return Nothing
        End Function
        ''' <summary>
        ''' returns the actual finished date of the schedule
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FinishedOn() As Date?
            If Not Me.IsAlive("FinishedOn") Then Return Nothing
            Dim aList As String() = Me.ScheduleDefinition.GetFCFinishID

            '' search for the actual of the planned end
            '' if this has data (a date) then it must be an end
            For Each anID In aList
                If Me.HasMilestone(ID:=anID, hasData:=False) Then
                    Dim aMilestone As ScheduleMilestoneDefinition = Me.ScheduleDefinition.Milestones.Item(anID)
                    Dim aMDef As MileStoneDefinition = aMilestone.GetMilestoneDefinition()
                    If aMilestone IsNot Nothing AndAlso aMDef IsNot Nothing AndAlso aMilestone.IsForecast Then
                        Dim avalue As Object = Me.GetMilestoneValue(aMilestone.ActualOfFC())
                        If aMDef.IsDate Then
                            Return avalue
                        Else
                            ''' here we should check on some status
                            ''' 
                            Throw New NotImplementedException("Finishing on Status is not implemented")
                        End If
                    ElseIf aMilestone Is Nothing Then
                        CoreMessageHandler(message:="milestone schedule definition could not be retrieved", argument:=anID, messagetype:=otCoreMessageType.ApplicationError, _
                                            procedure:="ScheduleEdition.FinishedOn", objectname:=Me.ObjectID)
                    ElseIf aMDef Is Nothing Then
                        CoreMessageHandler(message:="milestone definition could not be retrieved", argument:=aMilestone.ActualOfFC, messagetype:=otCoreMessageType.ApplicationError, _
                                            procedure:="ScheduleEdition.FinishedOn", objectname:=Me.ObjectID)
                    End If
                End If
            Next

            Return Nothing
        End Function
        ''' <summary>
        ''' is the schedule finished
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsFinished() As Boolean
            If Not Me.IsAlive("IsFinished") Then Return False
            Dim aList As String() = Me.ScheduleDefinition.GetFCFinishID

            '' search for the actual of the planned end
            '' if this has data (a date) then it must be an end
            For Each anID In aList
                If Me.HasMilestone(ID:=anID, hasData:=False) Then
                    Dim aMilestone As ScheduleMilestoneDefinition = Me.ScheduleDefinition.Milestones.Item(anID)
                    Dim aMDef As MileStoneDefinition = aMilestone.GetMilestoneDefinition()
                    If aMilestone IsNot Nothing AndAlso aMDef IsNot Nothing Then
                        Dim avalue As Object = Me.GetMilestoneValue(aMilestone.ActualOfFC())
                        If aMDef.IsDate Then
                            If avalue IsNot Nothing Then Return True
                        Else
                            ''' here we should check on some status
                            ''' 
                            Throw New NotImplementedException("Finishing on Status is not implemented")
                        End If
                    ElseIf aMilestone Is Nothing Then
                        CoreMessageHandler(message:="milestone schedule definition could not be retrieved", argument:=anID, messagetype:=otCoreMessageType.ApplicationError, _
                                            procedure:="ScheduleEdition.IsFinished", objectname:=Me.ObjectID)
                    ElseIf aMDef Is Nothing Then
                        CoreMessageHandler(message:="milestone definition could not be retrieved", argument:=aMilestone.ActualOfFC, messagetype:=otCoreMessageType.ApplicationError, _
                                            procedure:="ScheduleEdition.IsFinished", objectname:=Me.ObjectID)
                    End If
                End If
            Next

            Return False
        End Function

        '******* returns a TimeInterval for Task
        '*******
        ''' <summary>
        ''' LEGACY HACK ! timeinterval for the task
        ''' </summary>
        ''' <param name="TaskTypeID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTimeInterval(TaskTypeID As String) As clsHELPERTimeInterval
            Dim aVAlue As Object
            Dim aTimeInterval As New clsHELPERTimeInterval

            If Not Me.IsLoaded And Not Me.IsCreated Then
                GetTimeInterval = Nothing
                Exit Function
            End If

            Select Case LCase(TaskTypeID)

                ' return the Developement Task
                Case "development"

                    ' determine the end
                    ' HACK !
                    ' CASE 1 we are not ended and have a FC End
                    If Me.IsMilestoneValueMissing("bp4") And Not Me.IsMilestoneValueMissing("bp3") Then
                        aTimeInterval.endcmt = "bp3"
                        aTimeInterval.isActEnd = False
                        ' CASE 2 we are ended and have not a FC end
                    ElseIf Not Me.IsMilestoneValueMissing("bp4") And Me.IsMilestoneValueMissing("bp3") Then
                        aTimeInterval.endcmt = "bp4"
                        aTimeInterval.isActEnd = True
                        ' CASE 3 we have no FAP ends but a PDM Entry or Approval ?!
                        ' we have both ends -> which to take ?!
                    ElseIf Not Me.IsMilestoneValueMissing("bp3") And Not Me.IsMilestoneValueMissing("bp4") Then
                        aTimeInterval.endcmt = "bp4"
                        aTimeInterval.isActEnd = True
                    ElseIf Me.IsMilestoneValueMissing("bp3") And Me.IsMilestoneValueMissing("bp4") And _
                    (Not Me.IsMilestoneValueMissing("bp7") Or Not Me.IsMilestoneValueMissing("bp8") Or Not Me.IsMilestoneValueMissing("bp9") Or Not Me.IsMilestoneValueMissing("bp10")) Then
                        If Not Me.IsMilestoneValueMissing("bp7") Then
                            aTimeInterval.endcmt = "bp7"
                            aTimeInterval.isActEnd = False
                        ElseIf Not Me.IsMilestoneValueMissing("bp8") Then
                            aTimeInterval.endcmt = "bp8"
                            aTimeInterval.isActEnd = True
                        ElseIf Not Me.IsMilestoneValueMissing("bp9") Then
                            aTimeInterval.endcmt = "bp9"
                            aTimeInterval.isActEnd = False
                        ElseIf Not Me.IsMilestoneValueMissing("bp10") Then
                            aTimeInterval.endcmt = "bp10"
                            aTimeInterval.isActEnd = True
                        Else
                            aTimeInterval.endcmt = String.Empty
                        End If

                        ' CASE 4 we have no end atall
                    Else
                        aTimeInterval.endcmt = "no end could be found ?!"
                    End If
                    ' set the end
                    aVAlue = Me.GetMilestoneValue(aTimeInterval.endcmt)
                    If IsDate(aVAlue) And aVAlue <> constNullDate Then
                        aTimeInterval.enddate = CDate(aVAlue)
                    Else
                        aTimeInterval.enddate = constNullDate
                    End If

                    ' determine the start
                    ' CASE 1 we are not started and have a FC Start
                    If Me.IsMilestoneValueMissing("bp12") And Not Me.IsMilestoneValueMissing("bp11") Then
                        aTimeInterval.startcmt = "bp11"
                        aTimeInterval.isActStart = False
                        ' CASE 2 we are started and have not a FC Start
                    ElseIf Not Me.IsMilestoneValueMissing("bp12") And Me.IsMilestoneValueMissing("bp11") Then
                        aTimeInterval.startcmt = "bp12"
                        aTimeInterval.isActStart = True
                        ' CASE 3 we have no starts but a FC Freeze
                    ElseIf Me.IsMilestoneValueMissing("bp12") And Me.IsMilestoneValueMissing("bp11") And Not Me.IsMilestoneValueMissing("bp1") Then
                        aTimeInterval.startcmt = "bp1"
                        aTimeInterval.isActStart = False
                        ' CASE 4 we are started -> what to take ??
                    ElseIf Not Me.IsMilestoneValueMissing("bp11") And Not Me.IsMilestoneValueMissing("bp12") Then
                        aVAlue = DateDiff("d", Me.GetMilestoneValue("bp12"), aTimeInterval.enddate)
                        ' still time fo the proposed end -> take it
                        If aVAlue >= 0 Then
                            aTimeInterval.startcmt = "bp12"
                            aTimeInterval.isActStart = True
                        Else
                            'the actual start is later than the end (actual or fc)
                            ' take the fc start if it fits
                            aVAlue = DateDiff("d", Me.GetMilestoneValue("bp11"), aTimeInterval.enddate)
                            ' still time fo the proposed end -> take it
                            If aVAlue >= 0 Then
                                aTimeInterval.startcmt = "bp11"
                                aTimeInterval.isActStart = False
                            Else
                                ' the start is later than the end ?!
                                aTimeInterval.startcmt = "start of bp11,bp12 is later than the end of" & aTimeInterval.endcmt
                            End If
                        End If
                    End If


                    aVAlue = Me.GetMilestoneValue(aTimeInterval.startcmt)
                    If IsDate(aVAlue) And aVAlue <> constNullDate Then
                        aTimeInterval.startdate = CDate(aVAlue)
                    Else
                        ' error no  valid date in schedule
                    End If

                Case Else
                    System.Diagnostics.Debug.WriteLine("mismatch in getTimeInterval")
            End Select

            GetTimeInterval = aTimeInterval
        End Function

        '**** drawBaseline: creates out of this Schedule a new Baseline and updates all other Objects
        '****
        ''' <summary>
        ''' creates out of this Schedule a new Baseline and updates all other Objects
        ''' </summary>
        ''' <param name="MSGLOG"></param>
        ''' <param name="REFDATE"></param>
        ''' <param name="TIMESTAMP"></param>
        ''' <param name="ForceSerializeToOTDB"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DrawBaseline(Optional ByRef MSGLOG As BusinessObjectMessageLog = Nothing, _
                                     Optional ByVal REFDATE As Date = Nothing, _
                                     Optional ByVal TIMESTAMP As Date = Nothing, _
                                     Optional ByVal ForceSerializeToOTDB As Boolean = False) As Boolean

            Dim aTrack As New Track
            Dim allSchedules As New Collection
            Dim allTracks As New Collection
            Dim anEdition As New ScheduleEdition

            '* init
            If Not Me.IsInitialized Then
                If Not Me.Initialize() Then
                    DrawBaseline = False
                    Exit Function
                End If
            End If
            If Not IsCreated And Not IsLoaded Then
                DrawBaseline = False
                Exit Function
            End If

            If IsMissing(REFDATE) Then
                REFDATE = Now
            End If

            'if we have a baseline
            If Me.IsBaseline Then
                Call CoreMessageHandler(message:=" Schedule for uid #" & Me.Uid & " is already baselined with this updc #" & Me.Updc, _
                                      procedure:="Schedule.drawBaseline", argument:=Me.Uid & "." & Me.Updc, break:=False)
                DrawBaseline = True
                Exit Function
            End If

            '** set it
            Me.IsBaseline = True
            '** add BaseLineDate
            Me.BaselineRefDate = REFDATE
            Me.BaselineUPDC = Me.Updc
            Me.IsFrozen = True
            ' add plan version
            If Me.IsFrozen Then
                ' set the revision
                If Me.Revision = String.Empty Then
                    Me.Revision = ConstFirstPlanRevision
                Else
                    Me.Revision = Me.IncreaseRevison(MajorFlag:=True, MinorFlag:=False)
                End If
            End If
            '*** persist
            If Not Me.Persist(timestamp:=TIMESTAMP) Then
                DrawBaseline = False
                Exit Function
            End If


            '** go through all schedules (also me) in the same workspaceID
            '**
            For Each anEdition In AllByUID(Me.Uid)
                If anEdition.WorkspaceID = Me.WorkspaceID And anEdition.CreatedOn >= Me.CreatedOn And _
                   anEdition.FCupdc >= Me.FCupdc Then
                    '** freeze it if the schedule was not frozen through al later baseline
                    '**
                    ' freeze again ?!
                    If anEdition.IsFrozen = True Then
                        Call CoreMessageHandler(message:=" Schedule was baselined again at a later point of time", _
                                              procedure:="Schedule.drawBaseline", argument:=Me.Uid & "." & Me.Updc, break:=False)

                    End If
                    If anEdition.Updc <> Me.Updc Then
                        anEdition.IsFrozen = True
                        anEdition.BaselineUPDC = Me.Updc
                        anEdition.Revision = Me.Revision
                        anEdition.BaselineRefDate = Me.BaselineRefDate
                        anEdition.Persist()
                    End If
                    'update the Tracks associated with this schedule (moving targets)
                    allTracks = aTrack.AllByDeliverable(Me.Uid, scheduleUPDC:=anEdition.Updc)
                    For Each aTrack In allTracks
                        If Not aTrack.UpdateTracking(persist:=True, checkGAP:=True) Then
                            Debug.Assert(False)
                        End If
                    Next aTrack

                End If
            Next anEdition

            DrawBaseline = True
        End Function


        '******** Increase the Revision in Form VXX.YY
        '********
        ''' <summary>
        ''' increase revision in Form VXX.YY
        ''' </summary>
        ''' <param name="MajorFlag"></param>
        ''' <param name="MinorFlag"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IncreaseRevison(MajorFlag As Boolean, MinorFlag As Boolean) As String
            Dim i, j, k As Integer
            Dim minor As Integer
            Dim major As Integer
            Dim aValue As Object

            If Not IsLoaded And Not IsCreated Then
                IncreaseRevison = String.Empty
                Exit Function
            End If


            If Me.Revision <> String.Empty And UCase(Me.Revision) Like "V*.*" Then
                aValue = Mid(Me.Revision, _
                             InStr(UCase(Me.Revision), "V") + 1, _
                             InStr(Me.Revision, ".") - InStr(UCase(Me.Revision), "V"))
                If IsNumeric(aValue) Then
                    major = CInt(aValue)

                    aValue = (Mid(Me.Revision, InStr(Me.Revision, ".") + 1))
                    If IsNumeric(aValue) Then
                        minor = CInt(aValue)
                    Else
                        minor = 0
                    End If

                    If MajorFlag Then
                        major = major + 1
                        minor = 0
                    ElseIf MinorFlag Then
                        minor = minor + 1
                    End If

                    Me.Revision = "V" & major & "." & minor
                End If
            ElseIf Me.Revision <> String.Empty And UCase(Me.Revision) Like "V*" Then
                aValue = Mid(Me.Revision, _
                             InStr(UCase(Me.Revision), "V") + 1, _
                             Len(Me.Revision) - InStr(UCase(Me.Revision), "V"))
                If IsNumeric(aValue) Then
                    major = CInt(aValue)
                    minor = 0
                    If MajorFlag Then
                        major = major + 1
                        minor = 0
                    ElseIf MinorFlag Then
                        minor = minor + 1
                    End If

                    Me.Revision = "V" & major & "." & minor
                End If

            ElseIf Me.Revision = String.Empty Then
                Me.Revision = ConstFirstPlanRevision
            Else
                Call CoreMessageHandler(message:=("me.revision " & Me.Revision & " not increasable since not in VXX.YY"), argument:=Me.Revision, _
                                      procedure:="Schedule.increaserevision", break:=False)
                Return Me.Revision
            End If
            ' exit
            IncreaseRevison = Me.Revision

        End Function

        ''' <summary>
        ''' Handles the ObjectMessage Added Event and sets the status here
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ScheduleEdtion_OnMessageAdded(sender As Object, e As BusinessObjectMessageLog.EventArgs) Handles _ObjectMessageLog.OnObjectMessageAdded
            Dim theItems As IList(Of StatusItem) = e.Message.StatusItems
            If theItems IsNot Nothing AndAlso theItems.Count > 0 Then
                For Each anItem In theItems
                    ''' sets the lifecycle status to the highest 
                    ''' 
                    If anItem.TypeID = ConstStatusType_ScheduleLifecycle Then
                        If Me.LifeCycleStatus Is Nothing OrElse anItem.Weight > Me.LifeCycleStatus.Weight Then
                            Me.LifeCycleStatus = anItem
                        End If
                    End If
                    ''' sets the process status to the highest 
                    ''' 
                    If anItem.TypeID = ConstStatusType_ScheduleProcess Then
                        If Me.ProcessStatus Is Nothing OrElse anItem.Weight > Me.ProcessStatus.Weight Then
                            Me.ProcessStatus = anItem
                        End If
                    End If
                Next
            End If
        End Sub
        ''' <summary>
        ''' checks the schedule edition on the lifecycle status
        ''' </summary>
        ''' <param name="msglog"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CheckScheduleLifeCycle(Optional msglog As BusinessObjectMessageLog = Nothing) As otValidationResultType
            If msglog Is Nothing Then msglog = Me.ObjectMessageLog
            If Not Me.IsAlive("CheckScheduleLifeCycle") Then Return otValidationResultType.FailedNoProceed

            ''' clear log
            ''' 
            For Each message In msglog.ToList
                If message.StatusItems(statustype:=ConstStatusType_ScheduleLifecycle).Count > 0 Then
                    message.Delete() ' remove old messages from list and delete
                End If
            Next


            Dim aScheduleDefinition As ScheduleDefinition = Me.ScheduleDefinition
            If aScheduleDefinition Is Nothing Then
                msglog.Add(2101, Nothing, Nothing, Nothing, Nothing, Me, Me.Uid, Me.Updc)
                Return otValidationResultType.FailedNoProceed
            End If

            ''' check if we have a finishing milestone
            ''' 
            For Each anID As String In aScheduleDefinition.GetFCFinishID
                If Not Me.HasMilestoneDate(anID) Then
                    msglog.Add(2100, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, anID)
                End If
            Next

            '''
            ''' Check the milestones 
            '''
            For Each aMilestone In Me.Milestones

                '''
                ''' is it a finishing milestone ?!
                ''' 
                If aMilestone.IsActual AndAlso aMilestone.IsFinishingMilestone Then
                    ''' is the schedule finished ? - checks are obsolete
                    ''' 
                    If aMilestone.Value IsNot Nothing AndAlso aMilestone.IsValid Then
                        msglog.Add(2211, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, aMilestone.ID, CType(aMilestone.Value, Date))
                    End If
                End If
                '''
                ''' check on what is mandatory / prohibited / facultative
                ''' 

                Dim aMSDef As ScheduleMilestoneDefinition = aMilestone.ScheduleMilestoneDefinition
                If aMSDef IsNot Nothing Then
                    If aMilestone.Value IsNot Nothing AndAlso aMSDef.IsFacultative Then
                        If aMilestone.IsActual Then
                            msglog.Add(2105, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, aMilestone.ID)
                        Else
                            msglog.Add(2108, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, aMilestone.ID)
                        End If

                    End If
                    If aMilestone.Value Is Nothing AndAlso aMSDef.IsMandatory Then
                        If aMilestone.IsActual Then
                            ' doesnot make sense actual and mandatory means that if this is overdue it cannot be neglected
                            ' for lfcl this means the schedule has not started but actual milestones in the past which are null

                            'msglog.Add(2104, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, aMilestone.ID)
                        Else
                            msglog.Add(2107, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, aMilestone.ID)
                        End If

                    End If
                    If aMilestone.Value IsNot Nothing AndAlso aMSDef.IsProhibited Then
                        If aMilestone.IsActual Then
                            msglog.Add(2103, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, aMilestone.ID, aMilestone.Value)
                        Else
                            msglog.Add(2106, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, aMilestone.ID, aMilestone.Value)
                        End If

                    End If
                End If

                '''
                ''' date milestone must have a date (comment on absence days)
                ''' 
                If aMilestone.IsDate AndAlso aMilestone.Value IsNot Nothing Then
                    If IsDate(aMilestone.Value) Then
                        If Not CalendarEntry.IsAvailableOn(refdate:=CDate(aMilestone.Value), name:=CurrentSession.DefaultCalendarName) Then
                            ''' not available ?!
                            msglog.Add(2210, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, aMilestone.ID, _
                                       CDate(aMilestone.Value), CurrentSession.DefaultCalendarName)
                        End If
                    Else
                        ''' not a date ?!
                        msglog.Add(2102, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, aMilestone.ID, aMilestone.Value)
                    End If

                End If
            Next


            ''' final status
            ''' 
            If Me.LifeCycleStatus Is Nothing Then
                msglog.Add(2200, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID)
                Return otValidationResultType.Succeeded
            ElseIf Not Me.LifeCycleStatus.Aborting Then
                msglog.Add(2201, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID)
                Return otValidationResultType.WarningProceed
            Else
                Return otValidationResultType.FailedButProceed
            End If
        End Function

        ''' <summary>
        ''' checks the schedule edition on the lifecycle status
        ''' </summary>
        ''' <param name="msglog"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CheckScheduleProcessStatus(Optional msglog As BusinessObjectMessageLog = Nothing) As otValidationResultType
            If msglog Is Nothing Then msglog = Me.ObjectMessageLog
            If Not Me.IsAlive("CheckScheduleProcessStatus") Then Return otValidationResultType.FailedNoProceed

            ''' clear log
            ''' 
            For Each message In msglog.ToList 'make list to avoid operation error while removing
                If message.StatusItems(statustype:=ConstStatusType_ScheduleProcess).Count > 0 Then
                    message.Delete() ' remove old messages from list and delete
                End If
            Next
            Dim aScheduleDefinition As ScheduleDefinition = Me.ScheduleDefinition
            If aScheduleDefinition Is Nothing Then
                msglog.Add(2101, Nothing, Nothing, Nothing, Nothing, Me, Me.Uid, Me.Updc)
                Return otValidationResultType.FailedNoProceed
            End If

            '''
            ''' Check the milestones 
            '''
            For Each aMilestone In Me.Milestones

                '''
                ''' is it a finishing milestone ?!
                ''' 
                If aMilestone.IsActual AndAlso aMilestone.Value IsNot Nothing AndAlso _
                    aMilestone.IsFinishingMilestone AndAlso aMilestone.IsDate AndAlso aMilestone.IsValid Then
                    '''
                    ''' check on finishing
                    ''' 
                    Dim afinishdate As Date = CType(aMilestone.Value, Date)
                    Dim anFCID As String() = ScheduleDefinition.GetFCFinishID(ofActualID:=aMilestone.ID)
                    Dim anForecast As ScheduleMilestone
                    Dim aFinishFCDate As Date
                    If anFCID IsNot Nothing AndAlso anFCID.Count > 0 Then
                        anForecast = Me.Milestones.Item(anFCID.First)
                        aFinishFCDate = CDate(anForecast.Value)
                    Else
                        aFinishFCDate = Nothing
                    End If

                    '''
                    ''' finished
                    ''' 
                    If aMilestone.Value IsNot Nothing Then
                        Dim span As Integer = DateDiff("d", aFinishFCDate, afinishdate)
                        If span >= -30 AndAlso span <= 30 Then
                            msglog.Add(2901, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, span)
                        ElseIf span > 30 Then
                            msglog.Add(2903, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, span)
                        ElseIf span < -30 Then
                            msglog.Add(2902, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, span)
                        End If
                    Else
                        '''
                        ''' not fininished
                        ''' 
                        Dim span As Integer = DateDiff("d", aFinishFCDate, Date.Now)
                        If span > CurrentSession.TodayLatency + 30 Then
                            msglog.Add(2610, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, aFinishFCDate, span)
                        ElseIf span > CurrentSession.TodayLatency Then
                            msglog.Add(2611, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, aFinishFCDate, span)
                        ElseIf span < CurrentSession.TodayLatency Then
                            msglog.Add(2612, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, aFinishFCDate, -span)
                        Else
                            msglog.Add(2613, Nothing, Nothing, Nothing, Nothing, Me, aScheduleDefinition.ID, aFinishFCDate, -span)
                        End If
                    End If

                End If

            Next

            Return otValidationResultType.Succeeded

        End Function

        ''' <summary>
        ''' checks and sets the new validation status
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CheckScheduleStatus(Optional msglog As BusinessObjectMessageLog = Nothing) As otValidationResultType
            Dim result As otValidationResultType
            Dim status As StatusItem
            If msglog Is Nothing Then msglog = Me.ObjectMessageLog
            '''
            ''' Check Lifecycle
            ''' 
            result = Me.CheckScheduleLifeCycle(msglog:=msglog)
            status = msglog.GetHighestMessageHighestStatusItem(statustype:=ConstStatusType_ScheduleLifecycle)
            Me.LifeCycleStatus = status
            If result = otValidationResultType.FailedNoProceed Then Return result
            ' do not take the abort from status -> strange external controlled condition because this is also used 
            ' for persisting validation !!

            'If status IsNot Nothing AndAlso (status.Aborting OrElse result = otValidationResultType.FailedNoProceed) Then
            '    Return otValidationResultType.FailedNoProceed
            'End If

            '''
            ''' Check the Process Status
            ''' 
            result = Me.CheckScheduleProcessStatus(msglog:=msglog)
            status = msglog.GetHighestMessageHighestStatusItem(statustype:=ConstStatusType_ScheduleProcess)
            Me.ProcessStatus = status
            ' do not take the abort from status -> strange external controlled condition because this is also used 
            ' for persisting validation !!

            'If status IsNot Nothing AndAlso (status.Aborting OrElse result = otValidationResultType.FailedNoProceed) Then
            '    Return otValidationResultType.FailedNoProceed
            'End If

            Return result
        End Function
        ''' <summary>
        ''' Validated Event Handler for the Object itself
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ScheduleEdition_OnValidated(sender As Object, e As ormDataObjectValidationEventArgs) Handles Me.OnValidated
            Dim msglog As BusinessObjectMessageLog
            ''' run the schedule check
            '''
            If e.Msglog IsNot Nothing Then
                msglog = Me.ObjectMessageLog
            Else
                msglog = e.Msglog
            End If

            e.ValidationResult = Me.CheckScheduleStatus(msglog)

        End Sub


        ''' <summary>
        ''' Feeding Event 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub Schedule_OnFeeding(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnFeeding
            ' set last forecast update
            If Me.IsForecastChanged Then
                If e.Timestamp IsNot Nothing Then
                    Me.LastForecastUpdate = e.Timestamp
                Else
                    Me.LastForecastUpdate = Date.Now
                End If
            End If
        End Sub

        ''' <summary>
        ''' onPersisted Handler for reseting
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ScheduleEdition_OnPersisted(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnPersisted
            _isForeCastChanged = False
            _haveMilestonesChanged = False
        End Sub

        ''' <summary>
        ''' clones an object
        ''' </summary>
        ''' <param name="pkarray"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Clone(pkarray() As Object, Optional runtimeOnly As Boolean? = Nothing) As ScheduleEdition Implements iormCloneable(Of OnTrack.Scheduling.ScheduleEdition).Clone

            Dim aNewRecord As ormRecord
            Dim aMember As ScheduleMilestone
            Dim aCloneMember As ScheduleMilestone
            Dim aNewUID As Long = pkarray(0)
            Dim aNewUPDC As Long? = pkarray(1)
            If Not IsAlive(subname:="Clone") Then Return Nothing

            Try
                ''' for sure load
                LoadMilestones(scheduletypeid:=Me.Typeid)
                If Not Feed() Then
                    CoreMessageHandler(message:="object could not feed while cloning", procedure:="ScheduleEdition.Clone", argument:=Converter.Array2StringList(pkarray), _
                                        messagetype:=otCoreMessageType.InternalError, objectname:=Me.ObjectID)
                    Return Nothing
                End If

                '*** key ?
                If Not aNewUPDC.HasValue OrElse aNewUPDC = 0 Then
                    If Not Me.GetMaxUpdc(uid:=aNewUID, max:=aNewUPDC, workspaceID:=Me.WorkspaceID) Then
                        Call CoreMessageHandler(message:="cannot create unique primary key values - abort clone", procedure:="ScheduleEdition.clone", argument:=pkarray, _
                                                     containerID:=ObjectPrimaryTableID, messagetype:=otCoreMessageType.InternalError)
                        Return Nothing
                    End If
                    aNewUPDC += 1
                End If
                '*** now we copy the object
                Dim aNewObject As ScheduleEdition = MyBase.Clone(Of ScheduleEdition)({Uid, aNewUPDC})
                If Not aNewObject Is Nothing Then
                    ' actually here it we should clone all members too !
                    For Each aMember In _milestoneCollection
                        Call aNewObject.SetMilestoneValue(id:=aMember.ID, value:=aMember.Value)
                    Next
                    Return aNewObject
                End If

                Return Nothing

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="ScheduleEdition.Clone", exception:=ex)
                Return Nothing
            End Try
        End Function
        ''' <summary>
        ''' clone the object and its members
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <param name="VERSION"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Clone(Optional ByVal updc As Long = 0) As ScheduleEdition
            Dim pkArray() As Object = {Me.Uid, updc}
            Return Me.Clone(pkarray:=pkArray)
        End Function

        ''' <summary>
        ''' clone this schedule to a specific workspaceID by id
        ''' </summary>
        ''' <param name="workspaceID"></param>
        ''' <param name="UID"></param>
        ''' <param name="VERSION"></param>
        ''' <param name="setCurrSchedule"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CloneToWorkspace(ByVal workspaceID As String, _
                                        ByVal UID As Long, _
                                        Optional ByVal updc As Long = 0, _
                                        Optional ByVal setCurrSchedule As Boolean = False) As Boolean

            Dim aNewObject As ScheduleEdition
            Dim newRecord As ormRecord
            Dim aWorkspace As Workspace
            Dim aCurrSCHEDULE As WorkspaceSchedule
            Dim newUPDC As Long

            If Not IsAlive(subname:="CloneToWorkspace") Then Return False

            '**
            aWorkspace = Workspace.Retrieve(id:=workspaceID)
            If aWorkspace Is Nothing Then
                Call CoreMessageHandler(argument:=workspaceID, procedure:="Schedule.cloneToWorkspace", message:="couldn't load workspace")
                Return False
            End If

            ' get the new updc
            If Me.GetMaxUpdc(uid:=UID, max:=newUPDC, workspaceID:=workspaceID) Then
                If newUPDC = 0 Then
                    newUPDC = aWorkspace.MinScheduleUPDC
                Else
                    newUPDC = newUPDC + 1
                End If
            End If

            '** clone
            aNewObject = Me.Clone(updc:=newUPDC)
            If aNewObject Is Nothing Then
                Call CoreMessageHandler(argument:=workspaceID, procedure:="Schedule.cloneToWorkspace", _
                                      message:="couldn't clone schedule (" & Me.Uid & "," & Me.Updc & ") to new updc(" & newUPDC)
                CloneToWorkspace = False
                Exit Function
            End If

            '** set the workspaceID !
            aNewObject.WorkspaceID = workspaceID
            CloneToWorkspace = aNewObject.Persist

            ' set the currschedule
            If setCurrSchedule Then
                aCurrSCHEDULE = WorkspaceSchedule.Retrieve(UID:=Me.Uid, workspaceID:=workspaceID)
                If aCurrSCHEDULE Is Nothing Then
                    aCurrSCHEDULE = WorkspaceSchedule.Create(UID:=Me.Uid, workspaceID:=workspaceID)
                End If
                aCurrSCHEDULE.AliveEditionUpdc = newUPDC
                'aCurrSchedule.rev = Me.v
                CloneToWorkspace = aCurrSCHEDULE.Persist
            End If
        End Function
        ''' <summary>
        ''' retrieve maximum update count from the datastore
        ''' </summary>
        ''' <param name="max">the max to be set</param>
        ''' <param name="workspaceID">optional workspaceID</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetMaxUpdc(ByVal uid As Long, ByRef max As Long, Optional ByVal workspaceID As String = Nothing) As Boolean
            Dim aWorkspaceDef As New Workspace
            Dim mymax As Long
            If String.IsNullOrEmpty(workspaceID) Then workspaceID = CurrentSession.CurrentWorkspaceID
            Dim pkarray() As Object = {workspaceID}

            Try
                ' get
                Dim aStore As iormRelationalTableStore = GetPrimaryTableStore(ConstPrimaryTableID)
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(id:="getmaxupdc", addMe:=True, addAllFields:=False)

                '** prepare the command if necessary
                If Not aCommand.IsPrepared Then
                    aCommand.select = "max(" & ConstFNVersion & ")"
                    aCommand.Where = ConstFNUid & "=@uid and " & ConstFNWorkspaceID & "=@wspace"
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@uid", ColumnName:=ConstFNUid, tableid:=ConstPrimaryTableID))
                    aCommand.AddParameter(New ormSqlCommandParameter(id:="@wspace", ColumnName:=ConstFNWorkspaceID, tableid:=ConstPrimaryTableID))
                    aCommand.Prepare()
                End If
                aCommand.SetParameterValue(ID:="@uid", value:=uid)
                aCommand.SetParameterValue(ID:="@wspace", value:=workspaceID)

                '** run the Command
                Dim theRecords As List(Of ormRecord) = aCommand.RunSelect
                aWorkspaceDef = Workspace.Retrieve(id:=workspaceID)

                If theRecords.Count > 0 Then
                    If Not IsNull(theRecords.Item(0).GetValue(1)) And IsNumeric(theRecords.Item(0).GetValue(1)) Then
                        mymax = CLng(theRecords.Item(0).GetValue(1))
                        If Not aWorkspaceDef Is Nothing Then
                            If mymax >= (aWorkspaceDef.MaxScheduleUPDC - 10) Then
                                Call CoreMessageHandler(showmsgbox:=True, message:="Number range for workspaceID ends", _
                                                      argument:=workspaceID, messagetype:=otCoreMessageType.ApplicationWarning)
                            End If
                        End If
                    Else
                        If aWorkspaceDef IsNot Nothing Then
                            mymax = aWorkspaceDef.MinScheduleUPDC
                        Else
                            GetMaxUpdc = False
                        End If

                    End If
                    GetMaxUpdc = True

                Else
                    If aWorkspaceDef IsNot Nothing Then
                        mymax = aWorkspaceDef.MinScheduleUPDC
                    Else
                        GetMaxUpdc = False
                    End If
                End If
                If GetMaxUpdc Then
                    max = mymax
                End If
                Return GetMaxUpdc
            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=False, exception:=ex, procedure:="Schedule.getMaxUPDC")
                Return False
            End Try
        End Function


    End Class

    ''' <summary>
    ''' Schedule Milestone Class (runtime data of a schedule)
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=ScheduleMilestone.ConstObjectID, modulename:=ConstModuleScheduling, Version:=1, adddeletefieldbehavior:=True, usecache:=True, _
        description:="milestone data for schedules")> Public Class ScheduleMilestone
        Inherits ormBusinessObject
        Implements iormRelationalPersistable
        Implements iormCloneable(Of ScheduleMilestone)
        Implements iormInfusable

        Public Const ConstObjectID = "ScheduleMilestone"

        ''' <summary>
        ''' Table
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=2)> Public Const ConstPrimaryTableID = "tblScheduleMilestones"

        '** Index
        <ormIndex(columnname1:=ConstFNUid, columnname2:=ConstFNVersion)> Public constIndexCompound = ConstDefaultCompoundIndexName

        ''' <summary>
        ''' Primary KEys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=ScheduleEdition.ConstObjectID & "." & ScheduleEdition.ConstFNUid, _
             PrimaryKeyOrdinal:=1, XID:="MST1", aliases:={"SUID"})> Public Const ConstFNUid = ScheduleEdition.ConstFNUid

        <ormObjectEntry(referenceObjectEntry:=ScheduleEdition.ConstObjectID & "." & ScheduleEdition.ConstFNVersion, _
           PrimaryKeyOrdinal:=2, XID:="MST2")> _
        Public Const ConstFNVersion = ScheduleEdition.ConstFNVersion
        '** link together
        <ormForeignKey(entrynames:={ConstFNUid, ConstFNVersion}, foreignkeyreferences:={ScheduleEdition.ConstObjectID & "." & ScheduleEdition.ConstFNUid, _
                ScheduleEdition.ConstObjectID & "." & ScheduleEdition.ConstFNVersion}, useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const constFKSchedule = "fkschedules"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, defaultvalue:="", _
            title:="milestone id", Description:="id of the milestone", _
          PrimaryKeyOrdinal:=3, XID:="MST3")> Public Const ConstFNID = "id"

        ''' <summary>
        ''' Fields
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
           title:="value", Description:="text presentation of the milestone value", XID:="MST4")> Public Const ConstFNvalue = "value"

        <ormObjectEntry(Datatype:=otDataType.Date, isnullable:=True, _
          title:="value", Description:="date presentation of the milestone value", XID:="MST5")> Public Const ConstFNvaluedate = "valuedate"

        <ormObjectEntry(Datatype:=otDataType.Numeric, isnullable:=True, _
                 title:="value", Description:="numeric presentation of the milestone value", XID:="MST6")> Public Const ConstFNvaluenumeric = "valuenumeric"

        <ormObjectEntry(Datatype:=otDataType.Bool, isnullable:=True, _
        title:="value", Description:="bool presentation of the milestone value", XID:="MST7")> Public Const ConstFNvaluebool = "valuebool"

        <ormObjectEntry(Datatype:=otDataType.Long, isnullable:=True, _
        title:="value", Description:="long presentation of the milestone value", XID:="MST8")> Public Const ConstFNvaluelong = "valuelong"

        <ormObjectEntry(Datatype:=otDataType.Long, defaultvalue:=otDataType.Date, dbdefaultvalue:="6", _
        title:="datatype", Description:="datatype of the milestone value", XID:="MST10")> Public Const ConstFNDatatype = "datatype"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0", _
        title:="is a forecast", Description:="true if the milestone is a forecast", XID:="MST11")> Public Const ConstFNIsForecast = "isforecast"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0", _
        title:="is a status", Description:="true if the milestone is a status", XID:="MST12")> Public Const ConstFNIsStatus = "isstatus"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0", _
       title:="is valid", Description:="true if the milestone is valid", XID:="MST16")> Public Const ConstFNIsValid = "ISVALID"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=True, dbdefaultvalue:="1", _
        title:="is enabled", Description:="true if the milestone is enabled", XID:="MST13")> Public Const ConstFNIsEnabled = "isenabled"

        <ormObjectEntry(Datatype:=otDataType.Text, defaultvalue:=otMilestoneType.Date, _
           title:="Type", description:="type of the milestone", XID:="MST14")> Public Const ConstFNType = "typeid"

        <ormObjectEntry(referenceObjectEntry:=Workspace.ConstObjectID & "." & Workspace.ConstFNID, _
             Description:="workspaceID ID of the schedule", useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const ConstFNWorkspace = Workspace.ConstFNID

        ' deactivate ForeignKEy we do not have this object in domains
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, _
            useforeignkey:=otForeignKeyImplementation.None)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
                     title:="comment", Description:="comment", XID:="MST14")> Public Const ConstFNcmt = "cmt"


        ' fields

        <ormObjectEntryMapping(EntryName:=ConstFNUid)> Private _uid As Long
        <ormObjectEntryMapping(EntryName:=ConstFNVersion)> Private _updc As Long
        <ormObjectEntryMapping(EntryName:=ConstFNID)> Private _id As String = String.Empty

        <ormObjectEntryMapping(entryname:=ConstFNvalue)> Private _valuestring As String
        <ormObjectEntryMapping(EntryName:=ConstFNDatatype)> Private _datatype As otDataType
        <ormObjectEntryMapping(EntryName:=ConstFNcmt)> Private _cmt As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNWorkspace)> Private _workspaceID As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNIsStatus)> Private _isStatus As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNIsEnabled)> Private _isEnabled As Boolean = True
        <ormObjectEntryMapping(EntryName:=ConstFNIsValid)> Private _isvalid As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNType)> Private _typeid As otMilestoneType

        'Private s_isActual As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNIsForecast)> Private _isForecast As Boolean

        'dynamic
        Private _loadedFromHost As Boolean
        Private _savedToHost As Boolean
        Private _isCacheNoSave As Boolean    ' if set this is not saved since taken from another updc
        Private _msglog As New BusinessObjectMessageLog
        Private _scheduleedition As ScheduleEdition
        Private _schedulemilestonedefinition As ScheduleMilestoneDefinition
        Private _value As Object

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the isvalid.
        ''' </summary>
        ''' <value>The isvalid.</value>
        Public Property IsValid() As Boolean
            Get
                Return Me._isvalid
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsValid, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the type id of the milestone type.
        ''' </summary>
        ''' <value>The typeid.</value>
        Public Property Typeid() As otMilestoneType
            Get
                Return Me._typeid
            End Get
            Set(value As otMilestoneType)
                Me._typeid = value
            End Set
        End Property

        ''' <summary>
        ''' returns true if this is a cache and will not be saved
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsCacheNoSave() As Boolean
            Get
                IsCacheNoSave = _isCacheNoSave
            End Get
            Set(value As Boolean)
                If value Then
                    _isCacheNoSave = True
                Else
                    _isCacheNoSave = False
                End If
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the scheduleedition.
        ''' </summary>
        ''' <value>The scheduleedition.</value>
        Public ReadOnly Property ScheduleEdition() As ScheduleEdition
            Get
                If Not Me.IsAlive("ScheduleEdition") Then Return Nothing

                If _scheduleedition Is Nothing Then _scheduleedition = Scheduling.ScheduleEdition.Retrieve(UID:=Me.UID, updc:=Me.Updc)
                Return Me._scheduleedition
            End Get

        End Property

        ''' <summary>
        ''' retrieves the schedule milestone definition for this milestone.
        ''' </summary>
        ''' <value>schedule milestone edition.</value>
        Public ReadOnly Property ScheduleMilestoneDefinition() As ScheduleMilestoneDefinition
            Get
                If Not Me.IsAlive("ScheduleMilestoneDefinition") Then Return Nothing
                If Me.ScheduleEdition Is Nothing Then Return Nothing
                If _schedulemilestonedefinition Is Nothing Then
                    _schedulemilestonedefinition = _
                        Scheduling.ScheduleMilestoneDefinition.Retrieve(Me.ScheduleEdition.Typeid, ID:=Me.ID, domainID:=Me.DomainID)
                End If
                Return Me._schedulemilestonedefinition
            End Get

        End Property
        ''' <summary>
        ''' returns true if this milestone is a finishing milestone of the schedule
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsFinishingMilestone As Boolean
            Get
                If Me.ScheduleMilestoneDefinition Is Nothing Then Return False
                Return Me.ScheduleMilestoneDefinition.IsFinish
            End Get
        End Property

        ''' <summary>
        ''' get the uid 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property UID() As Long
            Get

                UID = _uid
            End Get

        End Property
        ''' <summary>
        ''' get the updc of the schedule
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Updc() As Long
            Get
                Updc = _updc
            End Get

        End Property
        ''' <summary>
        ''' gets the ID of the Milestone
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ID() As String
            Get
                Return _id
            End Get

        End Property
        ''' <summary>
        ''' gets or sets the Workspace ID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property WorkspaceID() As String
            Get
                Return _workspaceID
            End Get
            Set(value As String)
                SetValue(ConstFNWorkspace, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Value() As Object
            Get
                Return _value
            End Get
            Set(ByVal value As Object)
                _value = value
                SetValue(ConstFNvalue, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the datatype
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Datatype() As otDataType
            Get
                Return _datatype
            End Get
            Set(value As otDataType)
                SetValue(ConstFNDatatype, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the comment
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Comment() As String
            Get
                Return _cmt
            End Get
            Set(value As String)
                SetValue(ConstFNcmt, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the forecast flag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsForecast() As Boolean
            Get
                Return _isForecast
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsForecast, value)
            End Set
        End Property
        ''' <summary>
        ''' returns true if the milestone is a actual
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsActual() As Boolean
            Get
                Return Not _isForecast
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsForecast, Not value)
            End Set
        End Property
        ''' <summary>
        ''' returns true if the milestone is a status
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsStatus() As Boolean
            Get
                Return _typeid = otMilestoneType.Status
            End Get

        End Property

        ''' <summary>
        ''' returns true if the milestone is a status
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsDate() As Boolean
            Get
                Return _typeid = otMilestoneType.Date
            End Get

        End Property
        ''' <summary>
        ''' returns true if the milestone is enabled
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
        ''' Initialize the data object
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Initialize() As Boolean
            _workspaceID = CurrentSession.CurrentWorkspaceID
            Return MyBase.Initialize()
        End Function

        ''' <summary>
        ''' Handles the default values needed event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ScheduleMilestone_OnDefaultValuesNeeded(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreateDefaultValuesNeeded



            Dim anID As String = e.Record.GetValue(ConstFNID)
            Dim anUPDC As Long? = e.Record.GetValue(ConstFNVersion)
            Dim anUID As Long? = e.Record.GetValue(ConstFNUid)

            ''' set the default values needed
            ''' 
            If anID IsNot Nothing AndAlso anUPDC.HasValue AndAlso anUID.HasValue Then
                Dim anEdition = Scheduling.ScheduleEdition.Retrieve(UID:=anUID, updc:=anUPDC)
                Dim aMilestoneDef = Scheduling.MileStoneDefinition.Retrieve(id:=anID, domainid:=Me.DomainID)
                Dim aScheduleMilestoneDef = Scheduling.ScheduleMilestoneDefinition.Retrieve(scheduletype:=anEdition.Typeid, ID:=anID, domainID:=Me.DomainID)

                If Not e.Record.HasIndex(ConstFNDatatype) OrElse e.Record.GetValue(ConstFNDatatype) Is Nothing OrElse e.Record.GetValue(ConstFNDatatype) = 0 Then
                    e.Record.SetValue(ConstFNDatatype, aMilestoneDef.Datatype)
                End If

                If Not e.Record.HasIndex(ConstFNvalue) OrElse e.Record.GetValue(ConstFNvalue) Is Nothing Then
                    e.Record.SetValue(ConstFNDatatype, aScheduleMilestoneDef.DefaultValue)
                End If

                If Not e.Record.HasIndex(ConstFNIsForecast) OrElse e.Record.GetValue(ConstFNIsForecast) Is Nothing Then
                    e.Record.SetValue(ConstFNIsForecast, aMilestoneDef.IsForecast)
                End If

                If Not e.Record.HasIndex(ConstFNType) OrElse e.Record.GetValue(ConstFNType) Is Nothing Then
                    e.Record.SetValue(ConstFNType, aMilestoneDef.Typeid)
                End If

                If Not e.Record.HasIndex(ConstFNWorkspace) OrElse e.Record.GetValue(ConstFNWorkspace) Is Nothing Then
                    e.Record.SetValue(ConstFNWorkspace, anEdition.WorkspaceID)
                End If
            End If

        End Sub

        ''' <summary>
        ''' Infuse the data object by record
        ''' </summary>
        ''' <param name="aRecord"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Sub OnInfused(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnInfused
            Dim aVAlue As Object


            Try
                ''' get the value converted
                ''' select on Datatype -> different fields
                Select Case _datatype

                    Case otDataType.Numeric
                        _value = Core.DataType.To(Record.GetValue(ConstFNvaluenumeric), _datatype)
                    Case otDataType.[Date], otDataType.Timestamp
                        _value = Core.DataType.To(Record.GetValue(ConstFNvaluedate), _datatype)
                    Case otDataType.[Long]
                        _value = Core.DataType.To(Record.GetValue(ConstFNvaluelong), _datatype)
                    Case otDataType.Bool
                        _value = Core.DataType.To(Record.GetValue(ConstFNvaluebool), _datatype)
                    Case Else
                        _value = Core.DataType.To(Record.GetValue(ConstFNvalue), _datatype)
                End Select


            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="ScheduleMilestone.Infuse")
            End Try


        End Sub

        ''' <summary>
        ''' Load and infuse the schedule milestone from the data store by primary key
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <param name="VERSION"></param>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal UID As Long, ByVal updc As Long, ByVal ID As String) As ScheduleMilestone
            Dim pkarray() As Object = {UID, updc, ID}
            Return ormBusinessObject.RetrieveDataObject(Of ScheduleMilestone)(pkArray:=pkarray)
        End Function

        ''' <summary>
        ''' Update the record from the properties
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Sub OnFeedRecord(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnFed


            Try
                '** special Handling
                Call Me.Record.SetValue(ConstFNvaluedate, Nothing)
                Call Me.Record.SetValue(ConstFNvaluenumeric, Nothing)
                Call Me.Record.SetValue(ConstFNvaluelong, Nothing)
                Call Me.Record.SetValue(ConstFNvaluebool, Nothing)

                Dim aValue = DirectCast(e.DataObject, ScheduleMilestone).Value

                Select Case DirectCast(e.DataObject, ScheduleMilestone).Datatype

                    Case otDataType.Numeric
                        If IsNumeric(aValue) Then Call Me.Record.SetValue(ConstFNvaluenumeric, CDbl(aValue))
                        Call Me.Record.SetValue(ConstFNvalue, CStr(aValue))
                    Case otDataType.Text, otDataType.Memo
                        Call Me.Record.SetValue(ConstFNvalue, CStr(aValue))
                    Case otDataType.Runtime, otDataType.Formula, otDataType.Binary
                        Call CoreMessageHandler(procedure:="ScheduleMilestone.persist", _
                                              message:="datatype (runtime, formular, binary) not specified how to be persisted", msglog:=_msglog, argument:=_datatype)
                    Case otDataType.[Date]
                        If Microsoft.VisualBasic.IsDate(aValue) Then
                            Call Me.Record.SetValue(ConstFNvaluedate, CDate(aValue))
                            Call Me.Record.SetValue(ConstFNvalue, Converter.Date2LocaleShortDateString(aValue))
                        Else
                            Call Me.Record.SetValue(ConstFNvalue, CStr(aValue))
                        End If
                    Case otDataType.[Long]
                        If IsNumeric(aValue) Then Call Me.Record.SetValue(ConstFNvaluelong, CLng(aValue))
                        Call Me.Record.SetValue(ConstFNvalue, CStr(aValue))
                    Case otDataType.Timestamp
                        If Microsoft.VisualBasic.IsDate(aValue) Then
                            Call Me.Record.SetValue(ConstFNvaluedate, CDate(aValue))
                            Call Me.Record.SetValue(ConstFNvalue, Converter.DateTime2UniversalDateTimeString(aValue))
                        Else
                            Call Me.Record.SetValue(ConstFNvalue, CStr(aValue))
                        End If
                    Case otDataType.Bool
                        If aValue = String.Empty Or IsEmpty(aValue) Or IsNull(aValue) Or aValue Is Nothing Then
                            Call Me.Record.SetValue(ConstFNvaluebool, False)
                        ElseIf aValue = True Or aValue = False Then
                            Call Me.Record.SetValue(ConstFNvaluedate, CBool(aValue))
                        Else
                            Call Me.Record.SetValue(ConstFNvaluedate, True)
                        End If
                        Call Me.Record.SetValue(ConstFNvalue, CStr(aValue))
                    Case Else
                        Call Me.Record.SetValue(ConstFNvalue, CStr(aValue))
                        Call CoreMessageHandler(procedure:="ScheduleMilestone.persist", _
                                              message:="datatype not specified how to be persisted", msglog:=_msglog, argument:=_datatype)
                End Select



            Catch ex As Exception
                Call CoreMessageHandler(procedure:="ScheduleMilestone.UpdateRecord", exception:=ex)
            End Try
        End Sub

        ''' <summary>
        ''' handler for the serialize with Host Application 
        ''' </summary>
        ''' <param name="aTimestamp"></param>
        ''' <param name="forceSerializeToOTDB"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Sub ScheduleMilestone_OnPersist(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnPersisting

        End Sub

        ''' <summary>
        ''' Validating Object Handler
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ScheduleMilestone_OValidated(sender As Object, e As ormDataObjectValidationEventArgs) Handles Me.OnValidated
            '''
            ''' set true
            ''' 
            If e.ValidationResult <> otValidationResultType.FailedNoProceed Then
                Me.IsValid = True
            End If
        End Sub

        ''' <summary>
        ''' Validated Entry Event Handler 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ScheduleMilestone_OnEntryValidating(sender As Object, e As ormDataObjectEntryValidationEventArgs) Handles Me.OnEntryValidating

            ''' validating the Entry
            ''' 
            If (e.ObjectEntryName = ConstFNvalue OrElse e.ObjectEntryName = ConstFNvaluedate) Then
                If e.Value IsNot Nothing Then
                    Dim aDef As ScheduleMilestoneDefinition = Me.ScheduleMilestoneDefinition
                    ''' prohibited
                    ''' 
                    If aDef IsNot Nothing AndAlso aDef.IsProhibited Then
                        e.Msglog.Add(2302, Nothing, Nothing, Nothing, Nothing, Me, Me.UID, Me.Updc, Me.ID, e.Value, aDef.ScheduleTypeID)
                        e.Value = Nothing
                        e.Result = True
                        e.ValidationResult = otValidationResultType.FailedButProceed
                        Return
                    End If
                End If
            ElseIf (e.ObjectEntryName = ConstFNvalue) AndAlso Me.IsStatus Then
                '''
                ''' should validate on the status item
                ''' 
                ''' Throw New NotImplementedException("StatusItem Validation")
            End If
        End Sub

        ''' <summary>
        ''' Validated Entry Event Handler 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ScheduleMilestone_OnEntryValidated(sender As Object, e As ormDataObjectEntryValidationEventArgs) Handles Me.OnEntryValidated

            ''' validating the Entry
            ''' 
            If (e.ObjectEntryName = ConstFNvalue OrElse e.ObjectEntryName = ConstFNvaluedate) AndAlso Me.IsDate Then

                If e.Value IsNot Nothing AndAlso Not Microsoft.VisualBasic.IsDate(e.Value) Then
                    e.Msglog.Add(2300, Nothing, Nothing, Nothing, Nothing, Me, Me.UID, Me.Updc, Me.ID)
                    e.ValidationResult = otValidationResultType.FailedNoProceed
                    Return
                ElseIf e.Value IsNot Nothing And Microsoft.VisualBasic.IsDate(e.Value) Then
                    Dim aDef As ScheduleMilestoneDefinition = Me.ScheduleMilestoneDefinition
                    ''' prohibited
                    ''' 
                    If aDef.IsProhibited Then
                        e.Msglog.Add(2303, Nothing, Nothing, Nothing, Nothing, Me, Me.UID, Me.Updc, Me.ID, e.Value, aDef.ScheduleTypeID)
                        e.ValidationResult = otValidationResultType.FailedNoProceed
                        Return
                    End If
                    ''' not in calendar
                    ''' 
                    If Not CalendarEntry.HasDate(refDate:=CDate(e.Value)) Then
                        e.Msglog.Add(2301, Nothing, Nothing, Nothing, Nothing, Me, Me.UID, Me.Updc, Me.ID, e.Value)
                        e.ValidationResult = otValidationResultType.FailedNoProceed
                        Return
                    Else
                        If Not CalendarEntry.IsAvailableOn(refdate:=CDate(e.Value), name:=CurrentSession.DefaultCalendarName) Then
                            ''' not available ?!
                            e.Msglog.Add(2210, Nothing, Nothing, Nothing, Nothing, aDef.ScheduleTypeID, Me.ID, _
                                       CDate(e.Value), CurrentSession.DefaultCalendarName)
                        End If
                    End If
                End If
            ElseIf (e.ObjectEntryName = ConstFNvalue) AndAlso Me.IsStatus Then
                '''
                ''' should validate on the status item
                ''' 
                Throw New NotImplementedException("StatusItem Validation")
            End If
        End Sub

        ''' <summary>
        ''' create a persistable schedule milestone by primary key
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <param name="VERSION"></param>
        ''' <param name="ID"></param>
        ''' <param name="FORCE"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal UID As Long, ByVal updc As Long, ByVal ID As String, _
                                                Optional domainid As String = Nothing, _
                                                Optional workspaceid As String = Nothing) As ScheduleMilestone
            Dim pkarray() As Object = {UID, updc, ID}
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            If String.IsNullOrEmpty(workspaceid) Then workspaceid = CurrentSession.CurrentWorkspaceID
            Dim aRecord As New ormRecord
            With aRecord
                .SetValue(ConstFNUid, UID)
                .SetValue(ConstFNVersion, updc)
                .SetValue(ConstFNID, ID)
                .SetValue(ConstFNDomainID, domainid) ' add this for the milestone definition and we are not under domainbehavior
                .SetValue(ConstFNWorkspace, workspaceid)
            End With
            Return ormBusinessObject.CreateDataObject(Of ScheduleMilestone)(aRecord, checkUnique:=True, domainID:=domainid)
        End Function

        ''' <summary>
        ''' Clone the schedule milestone by new primary key
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <param name="VERSION"></param>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Clone(ByVal UID As Long, ByVal updc As Long, ByVal ID As String) As ScheduleMilestone
            Dim pkarray() As Object = {UID, updc, ID}
            Return Me.Clone(pkarray)
        End Function
        ''' <summary>
        ''' clone the data object by primary key array
        ''' </summary>
        ''' <param name="pkArray"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Clone(pkArray() As Object, Optional runtimeOnly As Boolean? = Nothing) As ScheduleMilestone Implements iormCloneable(Of ScheduleMilestone).Clone
            Dim aNewObject As ScheduleMilestone

            If Not IsLoaded And Not IsCreated Then
                Return Nothing
            End If
            '* init
            If Not Me.IsInitialized Then
                If Not Me.Initialize() Then
                    Clone = Nothing
                    Exit Function
                End If
            End If

            'update our Record
            If Not Me.Feed() Then
                Clone = Nothing
                Exit Function
            End If

            aNewObject = MyBase.Clone(Of ScheduleMilestone)(pkArray)
            If Not aNewObject Is Nothing Then
                ' overwrite the primary keys
                Call aNewObject.Record.SetValue(ConstFNUid, UID)
                Call aNewObject.Record.SetValue(ConstFNVersion, Updc)
                Call aNewObject.Record.SetValue(ConstFNID, ID)
                Return aNewObject
            End If

            Return Nothing
        End Function


    End Class

    '    SELECT      dbo.TBLSCHEDULELINKS.FROMOBJECTID , dbo.TBLSCHEDULELINKS.FROMUID , dbo.TBLSCHEDULELINKS.fromms,
    '            dbo.TBLWORKSPACESCHEDULES .wspace, dbo.TBLSCHEDULEEDITIONS.UID, dbo.TBLSCHEDULEEDITIONS.UPDC, 
    '            t1.VALUE AS BP9, t2.value as BP10

    'FROM            dbo.TBLSCHEDULEEDITIONS
    '                   INNER JOIN
    '                         dbo.TBLSCHEDULEMILESTONES as t1 ON dbo.TBLSCHEDULEEDITIONS.UID = t1.UID AND 
    '                         dbo.TBLSCHEDULEEDITIONS.UPDC = t1.UPDC and t1.ID ='BP9'
    '					 INNER JOIN
    '                         dbo.TBLSCHEDULEMILESTONES as t2 ON dbo.TBLSCHEDULEEDITIONS.UID = t2.UID AND 
    '                        t2.UPDC = dbo.TBLSCHEDULEEDITIONS.UPDC and t2.ID ='BP10'	
    '					inner join 
    '		    			dbo.TBLWORKSPACESCHEDULES on dbo.TBLWORKSPACESCHEDULES.uid = dbo.TBLSCHEDULEEDITIONS .uid and dbo.TBLWORKSPACESCHEDULES.workupdc = dbo.TBLSCHEDULEEDITIONS.updc
    '					inner join
    '			    		dbo.TBLSCHEDULELINKS on dbo.TBLSCHEDULELINKS.TOUID = dbo.TBLSCHEDULEEDITIONS.uid and dbo.TBLSCHEDULELINKS.TOOBJECTID ='ScheduleEdition' and dbo.TBLSCHEDULELINKS .toms =''
    ''' <summary>
    ''' the current schedule class links the current schedule updc to a scheduled object 
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=ScheduleLink.ConstObjectID, modulename:=ConstModuleScheduling, Version:=1, adddeletefieldbehavior:=True, usecache:=True, _
        description:="link definitions between schedules and other business objects")> _
    Public Class ScheduleLink
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable

        Public Const ConstObjectID = "ScheduleLink"

        '** Schema Table
        <ormTableAttribute(version:=1)> Public Const ConstPrimaryTableID = "tblScheduleLinks"

        '** index
        <ormIndex(columnnames:={ConstFNToObjectID, ConstFNToUID, CONSTFNToMilestoneID, ConstFNFromObjectID, ConstFNFromUID, ConstFNFromMilestoneID})> Public Const ConstIndTo = "USED"

        ''' <summary>
        ''' Primary key of the schedule link object
        ''' FROM an ObjectID, UID, MS (String.empty if null)
        ''' TO   an ScheduleUID, MS
        ''' 
        ''' links a deliverable or other business objects with a schedule
        ''' also capable of linking schedules to schedules or milestones of schedules to schedules and
        ''' sustaining multiple links from an object to schedules
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' Design principles:
        ''' 
        ''' 1. a schedule link links a major business object (with uid) to a schedule object or a schedule object to a schedule object
        ''' 
        ''' 2. a schedule link has as relation all the workspace schedules
        ''' </remarks>

        ''' from Section
        ''' 
        <ormObjectEntry(referenceobjectentry:=ormObjectDefinition.ConstObjectID & "." & ormObjectDefinition.ConstFNID, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, _
            validationPropertyStrings:={ObjectValidationProperty.NotEmpty, ObjectValidationProperty.UseLookup}, _
            LookupPropertyStrings:={LookupProperty.UseAttributeValues}, _
            values:={Deliverable.ConstObjectID}, dbdefaultvalue:=Deliverable.ConstObjectID, defaultvalue:=Deliverable.ConstObjectID, _
            XID:="SL1", title:="Linked From Object", description:="object link from the scheduled object")> _
        Public Const ConstFNFromObjectID = "FROMOBJECTID"

        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=2, dbdefaultvalue:="0", lowerrange:=0, _
            XID:="SL2", title:="Linked from UID", description:="uid link from the scheduled object")> Public Const ConstFNFromUID = "FROMUID"

        <ormObjectEntry(referenceobjectentry:=MileStoneDefinition.ConstObjectID & "." & MileStoneDefinition.ConstFNID, PrimaryKeyOrdinal:=3, _
            dbdefaultValue:="", properties:={ObjectEntryProperty.Keyword}, _
            XID:="SL3", title:="Linked from Milestone", description:="uid link from the scheduled object milestone")> Public Const ConstFNFromMilestoneID = "FROMMS"

        ''' <summary>
        ''' fields
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceobjectentry:=ormObjectDefinition.ConstObjectID & "." & ormObjectDefinition.ConstFNID, _
             properties:={ObjectEntryProperty.Keyword}, isnullable:=True, _
             validationPropertyStrings:={ObjectValidationProperty.NotEmpty, ObjectValidationProperty.UseLookup}, _
             LookupPropertyStrings:={LookupProperty.UseAttributeValues}, _
            values:={Scheduling.ScheduleEdition.ConstObjectID}, _
            dbdefaultvalue:=Scheduling.ScheduleEdition.ConstObjectID, defaultvalue:=Scheduling.ScheduleEdition.ConstObjectID, _
            XID:="SL4", title:="Linked to Object", description:="object link to the scheduled object")> _
        Public Const ConstFNToObjectID = "ToObjectID"

        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=4, lowerrange:=0, _
            XID:="SL5", title:="Linked to UID", description:="uid link to the scheduled object")> Public Const ConstFNToUID = "TOUID"

        <ormObjectEntry(referenceobjectentry:=MileStoneDefinition.ConstObjectID & "." & MileStoneDefinition.ConstFNID, _
            PrimaryKeyOrdinal:=5, _
             properties:={ObjectEntryProperty.Keyword}, _
            XID:="SL6", title:="Linked to Milestone", description:="uid link to the scheduled object milestone")> Public Const CONSTFNToMilestoneID = "TOMS"

        ' deactivate ForeignKEy we do not have this object in domains
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, _
            useforeignkey:=otForeignKeyImplementation.None)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, dbdefaultvalue:="Deliverable", defaultvalue:=otScheduleLinkType.Deliverable, _
            XID:="SL7", title:="Link Type", description:="object link type")> Public Const ConstFNTypeID = "typeid"

        ''' <summary>
        ''' Mapping
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNFromObjectID)> Private _fromObjectID As String
        <ormObjectEntryMapping(EntryName:=ConstFNFromUID)> Private _fromUID As Long
        <ormObjectEntryMapping(EntryName:=ConstFNFromMilestoneID)> Private _FromMilestone As String
        <ormObjectEntryMapping(EntryName:=ConstFNToObjectID)> Private _ToObjectID As String
        <ormObjectEntryMapping(EntryName:=ConstFNToUID)> Private _ToUID As Long
        <ormObjectEntryMapping(EntryName:=CONSTFNToMilestoneID)> Private _ToMilestone As String
        <ormObjectEntryMapping(EntryName:=ConstFNTypeID)> Private _type As otScheduleLinkType

        ''' <summary>
        ''' Relation to WorkspaceSchedules per Workspace (this is not complete since workspaces are resolved by retrieved)
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkObject:=GetType(WorkspaceSchedule), fromEntries:={ConstFNToUID}, toEntries:={Scheduling.WorkspaceSchedule.ConstFNUID}, _
                     cascadeonCreate:=True, cascadeOnDelete:=True, cascadeOnUpdate:=False)> _
        Public Const ConstRWorkspaceSchedules = "RELWorkspaceScheduleS"

        <ormObjectEntryMapping(relationName:=ConstRWorkspaceSchedules, infusemode:=otInfuseMode.OnInject Or otInfuseMode.OnCreate Or otInfuseMode.OnDemand)> _
        Private _cscheduleCollection As ormRelationCollection(Of WorkspaceSchedule) = New ormRelationCollection(Of WorkspaceSchedule)(Me, keyentrynames:={WorkspaceSchedule.ConstFNWorkspaceID})

        ''' <summary>
        ''' Relation to WorkspaceSchedule on Compound Path - infused by event
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkObject:=GetType(WorkspaceSchedule), createobjectifnotretrieved:=True, _
                     cascadeonCreate:=True, cascadeOnDelete:=True, cascadeOnUpdate:=True)> _
        Public Const ConstRWorkspaceSchedule = "RELWorkspaceSchedule"

        <ormObjectEntryMapping(relationName:=ConstRWorkspaceSchedule, infusemode:=otInfuseMode.OnCreate Or otInfuseMode.OnInject Or otInfuseMode.OnDemand)> Private _WorkspaceSchedule As WorkspaceSchedule



#Region "properties"

        ''' <summary>
        ''' Gets or sets the type.
        ''' </summary>
        ''' <value>The type.</value>
        Public Property Type() As otScheduleLinkType
            Get
                Return Me._type
            End Get
            Set(value As otScheduleLinkType)
                Me._type = value
            End Set
        End Property

        ''' <summary>
        ''' gets the linking Object ID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FromObjectID() As String
            Get
                Return _fromObjectID

            End Get

        End Property
        ''' <summary>
        ''' gets linking Object UID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FromUID() As Long
            Get
                Return _fromUID
            End Get

        End Property
        ''' <summary>
        ''' gets the linking Milestone or String.empty if none
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FromMilestone() As String
            Get
                Return _FromMilestone
            End Get

        End Property
        ''' <summary>
        ''' gets or sets the linked Object ID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ToObjectID() As String

            Get
                Return _ToObjectID
            End Get
            Set(value As String)
                SetValue(ConstFNToObjectID, value)
            End Set

        End Property
        ''' <summary>
        ''' gets or sets the linking object UID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ToUid() As Long
            Get
                Return _ToUID
            End Get

        End Property
        ''' <summary>
        ''' gets or sets the linking Milestone or String.empty if not applicable
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ToMilestoneID() As String
            Get
                Return _ToMilestone
            End Get

        End Property

        ''' <summary>
        ''' retrieves the linked Schedule
        ''' </summary>
        ''' <param name="workspaceid"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property WorkspaceSchedule(Optional workspaceid As String = Nothing) As WorkspaceSchedule
            Get
                If workspaceid Is Nothing Then workspaceid = CurrentSession.CurrentWorkspaceID
                Return Scheduling.WorkspaceSchedule.Retrieve(UID:=Me.ToUid, workspaceID:=workspaceid)
            End Get
        End Property

#End Region

        ''' <summary>
        ''' Event Handler for OnWorkspace Changed from the Session
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ScheduleLink_OnWorkspaceChanged(sender As Object, e As SessionEventArgs)
            InfuseRelation(ConstRWorkspaceSchedule)
        End Sub

        ''' <summary>
        ''' event handler for related schedule persist
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ScheduleLink_WorkspaceSchedulePersist(sender As Object, e As ormDataObjectEventArgs)
            ''' Persist me too -> leads to recursion was thought that a workspaceschedule is an individual object
            ''' but Xchange works here through the deliverable view therefore also save in that direction and not backwards
            ''' 
            '''' If Me.IsCreated OrElse Me.IsChanged Then Me.Persist(e.Timestamp)
        End Sub

        Private Sub ScheduleLink_OnPersisting(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnPersisting

        End Sub
        ''' <summary>
        ''' Event Handler for the RelationRetrieveNeeded event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ScheduleLink_OnRelationRetrieveNeeded(sender As Object, e As ormDataObjectRelationEventArgs) Handles Me.OnRelationRetrieveNeeded
            If Not Me.IsAlive(subname:="WorkspaceSchedule_OnRelationRetrieveNeeded") Then Return
            ''' check on PropertyValueLot
            ''' 
            If e.RelationID.ToUpper = ConstRWorkspaceSchedule.ToUpper Then

                Dim aWorkspaceID As String = CurrentSession.CurrentWorkspaceID
                '' get the workspace for the active workspace (only ! - no way yet tor resolve for another workspace)
                ''
                Dim aWorkspaceSchedule As WorkspaceSchedule = Scheduling.WorkspaceSchedule.Retrieve(UID:=Me.ToUid, workspaceID:=aWorkspaceID)
                If aWorkspaceSchedule IsNot Nothing Then
                    AddHandler aWorkspaceSchedule.OnPersisted, AddressOf ScheduleLink_WorkspaceSchedulePersist
                    AddHandler CurrentSession.OnWorkspaceChanged, AddressOf ScheduleLink_OnWorkspaceChanged
                    e.RelationObjects.Add(aWorkspaceSchedule)
                End If

                e.Finished = True

            End If
        End Sub
        ''' <summary>
        ''' Event Handler for the RelationRetrieveNeeded event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ScheduleLink_OnRelationCreateNeeded(sender As Object, e As ormDataObjectRelationEventArgs) Handles Me.OnRelationCreateNeeded
            If Not Me.IsAlive(subname:="WorkspaceSchedule_OnRelationCreateNeeded") Then Return
            ''' check on PropertyValueLot
            ''' 
            If e.RelationID = ConstRWorkspaceSchedule Then

                Dim aWorkspaceID As String = CurrentSession.CurrentWorkspaceID
                '' get the workspace for the active workspace -- no way yet to provide an argument
                ''
                Dim aWorkspaceSchedule As WorkspaceSchedule = Scheduling.WorkspaceSchedule.Retrieve(UID:=Me.ToUid, workspaceID:=aWorkspaceID)
                If aWorkspaceSchedule Is Nothing Then aWorkspaceSchedule = Scheduling.WorkspaceSchedule.Create(UID:=Me.ToUid, workspaceID:=aWorkspaceID)
                AddHandler aWorkspaceSchedule.OnPersisted, AddressOf ScheduleLink_WorkspaceSchedulePersist
                AddHandler CurrentSession.OnWorkspaceChanged, AddressOf ScheduleLink_OnWorkspaceChanged
                e.RelationObjects.Add(aWorkspaceSchedule)
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
        Public Shared Function Create(fromObjectID As String, fromuid As Long, toScheduleUid As Long, _
                                      Optional fromMilestone As String = "", _
                                      Optional toMilestone As String = "") As ScheduleLink
            Dim primarykey As Object() = {fromObjectID, fromuid, fromMilestone, toScheduleUid, toMilestone}
            Return ormBusinessObject.CreateDataObject(Of ScheduleLink)(primarykey, checkUnique:=True)
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
        Public Shared Function Retrieve(fromObjectID As String, fromuid As Long, toScheduleUid As Long, _
                                        Optional fromMilestone As String = "", Optional toMilestone As String = "") As ScheduleLink
            Dim primarykey As Object() = {fromObjectID, fromuid, fromMilestone.ToUpper, toScheduleUid, toMilestone.ToUpper}
            Return ormBusinessObject.RetrieveDataObject(Of ScheduleLink)(primarykey)
        End Function

        ''' <summary>
        ''' retrieve a persitable link object for deliverables
        ''' </summary>
        ''' <param name="fromid"></param>
        ''' <param name="fromuid"></param>
        ''' <param name="toid"></param>
        ''' <param name="touid"></param>
        ''' <param name="frommilestone"></param>
        ''' <param name="tomilestone"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RetrieveDeliverableLinkFrom(deliverableUID As Long) As ScheduleLink
            Dim aResult As ScheduleLink

            Try
                Dim aStore As iormRelationalTableStore = GetPrimaryTableStore(ConstPrimaryTableID)
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(id:="deliverable", addAllFields:=True)
                If Not aCommand.IsPrepared Then
                    aCommand.Where = "[" & ConstFNFromObjectID & "] = @fromobjectid AND [" & ConstFNFromUID & "] = @fromuid AND [" & ConstFNFromMilestoneID & "] = @fromms"
                    aCommand.Where &= " AND [" & ConstFNToObjectID & "] = @toobjectid "
                    aCommand.Where &= " AND [" & ConstFNIsDeleted & "] = @deleted "
                    'aCommand.Where &= " AND ([" & ConstFNDomainID & "] = @domainID OR [" & ConstFNDomainID & "] = @globalID)"
                    aCommand.OrderBy = "[" & ConstFNToUID & "] asc"
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@fromobjectid", ColumnName:=ConstFNFromObjectID, tableid:=ConstPrimaryTableID))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@fromuid", ColumnName:=ConstFNFromUID, tableid:=ConstPrimaryTableID))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@fromms", ColumnName:=ConstFNFromMilestoneID, tableid:=ConstPrimaryTableID))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@toobjectid", ColumnName:=ConstFNToObjectID, tableid:=ConstPrimaryTableID))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@deleted", ColumnName:=ConstFNIsDeleted, tableid:=ConstPrimaryTableID))

                    'aCommand.AddParameter(New ormSqlCommandParameter(ID:="@domainID", ColumnName:=ConstFNDomainID, tableid:=ConstPrimaryTableID))
                    'aCommand.AddParameter(New ormSqlCommandParameter(ID:="@globalID", ColumnName:=ConstFNDomainID, tableid:=ConstPrimaryTableID))
                    aCommand.Prepare()
                End If

                aCommand.SetParameterValue(ID:="@fromobjectid", value:=Deliverable.ConstObjectID)
                aCommand.SetParameterValue(ID:="@fromuid", value:=deliverableUID)
                aCommand.SetParameterValue(ID:="@fromms", value:=String.Empty)
                aCommand.SetParameterValue(ID:="@toobjectid", value:=ScheduleEdition.ConstObjectID)
                aCommand.SetParameterValue(ID:="@deleted", value:=False)

                Dim aRecordCollection = aCommand.RunSelect

                For Each aRecord As ormRecord In aRecordCollection
                    Dim aNewObject As New ScheduleLink
                    If InfuseDataObject(record:=aRecord, dataobject:=aNewObject) Then
                        Return aNewObject
                    End If
                Next

                Return Nothing


            Catch ex As Exception

                Call CoreMessageHandler(exception:=ex, procedure:="ScheduleLink.RetrieveDeliverableLink")
                Return Nothing

            End Try

        End Function
        ''' <summary>
        ''' retrieve a persitable link object for deliverables
        ''' </summary>
        ''' <param name="fromid"></param>
        ''' <param name="fromuid"></param>
        ''' <param name="toid"></param>
        ''' <param name="touid"></param>
        ''' <param name="frommilestone"></param>
        ''' <param name="tomilestone"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RetrieveDeliverableLinkTo(scheduleUID As Long) As ScheduleLink

            Try
                Dim aStore As iormRelationalTableStore = GetPrimaryTableStore(ConstPrimaryTableID)
                Dim pkarray As Object() = {ScheduleEdition.ConstObjectID, scheduleUID, String.Empty}
                Dim aRecordCollection As List(Of ormRecord) = aStore.GetRecordsByIndex(indexname:=ConstIndTo, keyArray:=pkarray)

                For Each aRecord As ormRecord In aRecordCollection
                    Dim aNewObject As New ScheduleLink
                    If InfuseDataObject(record:=aRecord, dataobject:=aNewObject) Then
                        Return aNewObject
                    End If
                Next

                Return Nothing


            Catch ex As Exception

                Call CoreMessageHandler(exception:=ex, procedure:="ScheduleLink.RetrieveDeliverableLinkTo")
                Return Nothing

            End Try

        End Function
    End Class


    ''' <summary>
    ''' the workspace schedule class links schedule uid to a schedule edition in a given workspace
    ''' </summary>
    ''' <remarks>
    ''' design principles:
    ''' 
    ''' 1. the workspace schedule links per workspace the schedule uid with a updc edition
    ''' 
    ''' 2. the workspace schedule also publish a schedule edition (freeze) and increases the edition number
    ''' 
    ''' 3. the workspace schedule differentiates between active Schedule edition(frozen) and working schedule editions
    ''' 
    ''' </remarks>
    <ormObject(id:=WorkspaceSchedule.ConstObjectID, modulename:=ConstModuleScheduling, Version:=1, _
        adddeletefieldbehavior:=True, adddomainbehavior:=False, usecache:=True, _
        description:="linking object to the schedule edition per workspace")> _
    Public Class WorkspaceSchedule
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable

        ''' <summary>
        ''' Object ID
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstObjectID = "WorkspaceSchedule"

        ''' <summary>
        ''' Table Definition
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=2)> Public Const ConstPrimaryTableID = "TBLWORKSPACESCHEDULES"

        ''' <summary>
        ''' Index
        ''' </summary>
        ''' <remarks></remarks>
        <ormIndex(columnname1:=ConstFNUID, columnname2:=ConstFNWorkspaceID)> Public Const ConstIndTag = "UIDs"

        ''' <summary>
        ''' Primary Key Columns
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=Workspace.ConstObjectID & "." & Workspace.ConstFNID, PrimaryKeyOrdinal:=1, _
            useforeignkey:=otForeignKeyImplementation.NativeDatabase, _
            validationPropertyStrings:={ObjectValidationProperty.UseLookup}, lookupPropertyStrings:={LookupProperty.UseAttributeReference})> _
        Public Const ConstFNWorkspaceID = Workspace.ConstFNID

        <ormObjectEntry(referenceobjectentry:=ScheduleEdition.ConstObjectID & "." & ScheduleEdition.ConstFNUid, PrimaryKeyOrdinal:=2)> _
        Public Const ConstFNUID = ScheduleEdition.ConstFNUid



        ''' <summary>
        ''' Column Definition
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceobjectentry:=ScheduleEdition.ConstObjectID & "." & ScheduleEdition.ConstFNVersion, isnullable:=True, _
            title:="Alive Edition", description:="the alive schedule edition update counter" _
            )> Public Const ConstFNAliveUPDC = "aliveupdc"


        <ormObjectEntry(referenceobjectentry:=ScheduleEdition.ConstObjectID & "." & ScheduleEdition.ConstFNVersion, isnullable:=True, _
           title:="Working Edition", description:="the working schedule edition update counter" _
           )> Public Const ConstFNWorkUPDC = "workupdc"

        <ormObjectEntry(Datatype:=otDataType.Bool, XID:="CS5", title:="Is Active", defaultvalue:=True, dbdefaultvalue:="1", description:="set if active")> _
        Public Const ConstFNIsActive = "isactive"

        ' change FK Action since we have the workspace as FK (leads also to domians)
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, _
            useforeignkey:=otForeignKeyImplementation.None)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        <ormObjectEntry(referenceObjectEntry:=ScheduleDefinition.ConstObjectID & "." & ScheduleDefinition.ConstFNType, _
          title:="type", Description:="type of the schedule", XID:="SC14", aliases:={"BS4"}, isnullable:=True)> Public Const ConstFNTypeid = "typeid"

        ''' <summary>
        ''' Column Mappings
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNWorkspaceID)> Private _workspaceID As String
        <ormObjectEntryMapping(EntryName:=ConstFNUID)> Private _uid As Long
        <ormObjectEntryMapping(EntryName:=ConstFNTypeid)> Private _typeid As String
        <ormObjectEntryMapping(EntryName:=ConstFNAliveUPDC)> Private _AliveUpdc As Long?
        <ormObjectEntryMapping(EntryName:=ConstFNWorkUPDC)> Private _WorkUpdc As Long?
        <ormObjectEntryMapping(EntryName:=ConstFNIsActive)> Private _isActive As Boolean = True

        ''' <summary>
        ''' Relation to schedule definition
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(ScheduleDefinition), toprimarykeys:={ConstFNTypeid}, _
            cascadeOnCreate:=False, cascadeOnDelete:=False, cascadeOnUpdate:=False)> Public Const ConstRScheduleDefinition = "RELSCHEDULEDEFINITION"

        <ormObjectEntryMapping(RelationName:=ConstRScheduleDefinition, infuseMode:=otInfuseMode.OnDemand)> Private WithEvents _scheduleDefinition As ScheduleDefinition

        ''' <summary>
        ''' Relation to alive Schedule edition - will be resolved by events
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkObject:=GetType(Workspace), ToPrimaryKeys:={ConstFNWorkspaceID}, _
                     cascadeonCreate:=False, cascadeOnDelete:=False, cascadeOnUpdate:=False)> _
        Public Const ConstRWorkspace = "REL_Workspace"

        <ormObjectEntryMapping(relationName:=ConstRWorkspace, infusemode:=otInfuseMode.OnCreate Or otInfuseMode.OnDemand Or otInfuseMode.OnInject)> _
        Private _workspace As Workspace

        ''' <summary>
        ''' Relation to alive Schedule edition - will be resolved by events
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkObject:=GetType(ScheduleEdition), ToPrimaryKeys:={ConstFNUID, ConstFNAliveUPDC}, _
                     cascadeonCreate:=True, cascadeOnDelete:=True, cascadeOnUpdate:=True)> _
        Public Const ConstRAliveEdition = "REL_ALIVEEDITION"

        <ormObjectEntryMapping(relationName:=ConstRAliveEdition, infusemode:=otInfuseMode.OnCreate Or otInfuseMode.OnDemand Or otInfuseMode.OnInject)> _
        Private _aliveedition As ScheduleEdition

        ''' <summary>
        ''' Relation to alive Schedule edition - will be resolved by events
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkObject:=GetType(ScheduleEdition), createObjectifnotretrieved:=True, _
                     ToPrimaryKeys:={ConstFNUID, ConstFNWorkUPDC}, _
                     cascadeonCreate:=True, cascadeOnDelete:=True, cascadeOnUpdate:=True)> _
        Public Const ConstRWorkEdition = "REL_WorkEDITION"

        <ormObjectEntryMapping(relationName:=ConstRWorkEdition, infusemode:=otInfuseMode.OnCreate Or otInfuseMode.OnDemand Or otInfuseMode.OnInject)> _
        Private _workingedition As ScheduleEdition

        ''' <summary>
        ''' Define the constants for accessing the compounds
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstOPGetMileStoneValue = "GETMILESTONEVALUE"
        Public Const ConstOPSetMileStoneValue = "SETMILESTONEVALUE"

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
            AddHandler CurrentSession.OnWorkspaceChanged, AddressOf Me.WorkspaceSchedule_OnWorkspaceChanged
        End Sub


#Region "properties"


        ''' <summary>
        ''' Gets or sets the schedule typeid.
        ''' </summary>
        ''' <value>The typeid.</value>
        Public Property ScheduleTypeID() As String
            Get
                Return Me._typeid
            End Get
            Set(value As String)
                SetValue(ConstFNTypeid, _typeid)
            End Set
        End Property

        ''' <summary>
        ''' retrieve the related Schedule Definition object
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ScheduleDefinition() As ScheduleDefinition
            Get
                If Not Me.IsAlive(subname:="ScheduleDefinition") Then Return Nothing

                InfuseRelation(ConstRScheduleDefinition)
                Return _scheduleDefinition
            End Get
        End Property
        ''' <summary>
        ''' sets or gets the workspace ID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property WorkspaceID() As String
            Get
                Return _workspaceID
            End Get
            Set(value As String)
                SetValue(ConstFNWorkspaceID, value)
            End Set
        End Property
        ''' <summary>
        ''' returns the Workspace object of this Schedule
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Workspace As Workspace
            Get
                If Me.GetRelationStatus(ConstRWorkspace) = ormRelationManager.RelationStatus.Unloaded Then InfuseRelation(ConstRWorkspace)
                Return _workspace
            End Get
        End Property
        ''' <summary>
        ''' gets the schedule UID
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
        ''' true if the current schedule updc is active / enabled
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsActive() As Boolean
            Get
                Return _isActive
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsActive, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the current alive (running) Schedule edition Update Counter
        ''' returns nothing if not set (null)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AliveEditionUpdc() As Long?
            Get
                Return _AliveUpdc
            End Get
            Set(value As Long?)
                SetValue(ConstFNAliveUPDC, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the current (working - changeable) Schedule edition Update Counter
        ''' returns nothing if not set (null)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property WorkingEditionUpdc() As Long?
            Get
                Return _WorkUpdc
            End Get
            Set(value As Long?)
                SetValue(ConstFNWorkUPDC, value)
            End Set
        End Property
        ''' <summary>
        ''' gets the working schedule edition object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property WorkingEdition As ScheduleEdition
            Get
                If Me.GetRelationStatus(ConstRWorkEdition) = ormRelationManager.RelationStatus.Unloaded Then InfuseRelation(ConstRWorkEdition)
                Return _workingedition
            End Get
        End Property
        ''' <summary>
        ''' gets the alive schedule edition object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>C
        Public ReadOnly Property AliveEdition As ScheduleEdition
            Get
                If Me.GetRelationStatus(ConstRAliveEdition) = ormRelationManager.RelationStatus.Unloaded Then InfuseRelation(ConstRAliveEdition)
                Return _aliveedition
            End Get
        End Property
#End Region


        ''' <summary>
        ''' operation to Access the Milestone's Value
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ormObjectOperationMethod(operationname:=ConstOPGetMileStoneValue, tag:=ormObjectCompoundEntry.ConstCompoundGetter, _
            parameterEntries:={ormObjectCompoundEntry.ConstFNEntryName, ormObjectCompoundEntry.ConstFNValues})> _
        Public Function GetMilestoneValue(id As String, ByRef value As Object) As Boolean
            If Not IsAlive(subname:="GetMilestoneValue") Then Return Nothing

            If _workingedition IsNot Nothing Then
                Return _workingedition.RetrieveMilestoneValue(id:=id, value:=value)
            ElseIf _aliveedition IsNot Nothing Then
                Return _aliveedition.RetrieveMilestoneValue(id:=id, value:=value)
            End If

            Return False
        End Function

        ''' <summary>
        ''' operation to Access the Milestone's Value
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ormObjectOperationMethod(operationname:=ConstOPSetMileStoneValue, tag:=ormObjectCompoundEntry.ConstCompoundSetter, _
            parameterEntries:={ormObjectCompoundEntry.ConstFNEntryName, ormObjectCompoundEntry.ConstFNValues})> _
        Public Function SetMilestoneValue(id As String, value As Object) As Boolean
            If Not IsAlive(subname:="SetMilestoneValue") Then Return Nothing

            If _workingedition Is Nothing Then

                If _aliveedition IsNot Nothing Then
                    _workingedition = _aliveedition.Clone()

                End If
            ElseIf _workingedition IsNot Nothing Then
                Return _workingedition.SetMilestoneValue(id:=id, value:=value)
            End If

            Return False
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
            Dim IsPublishable As Boolean = True
            Dim aValidationResult As otValidationResultType
            Dim aWorkingEdition = Me.WorkingEdition

            '* init
            If Not Me.IsAlive(subname:="Publish") Then Return False


            ' TIMESTAMP
            If timestamp Is Nothing Then timestamp = Date.Now


            '** if any of the milestones is changed
            '**
            IsPublishable = True

            '** condition
            If aWorkingEdition IsNot Nothing AndAlso aWorkingEdition.HaveMileStonesChanged Then

                '''
                ''' Validate the Working Edition
                ''' 
                If msglog Is Nothing Then msglog = aWorkingEdition.ObjectMessageLog
                aValidationResult = aWorkingEdition.CheckScheduleStatus(msglog)
                If aValidationResult = otValidationResultType.FailedNoProceed Then
                    IsPublishable = False
                Else
                    IsPublishable = True
                End If

                ''' do we need to have some transformation while an edition is alive and now comes up the next one ?
                ''' should be included here
                ''' 

                ''' publish the new edition (working edition) since it is statisfying the validation and checking
                ''' the working edition will become the alive edition
                ''' and a copy of the working edition will be there as new working edition
                ''' 
                If IsPublishable Then
                    If aWorkingEdition.IsForecastChanged Then
                        aWorkingEdition.Incfcupdc()
                        aWorkingEdition.LastForecastUpdate = timestamp
                        '** right-move of new Schedule if we are frozen
                        If Me.AliveEdition IsNot Nothing AndAlso Me.AliveEdition.IsFrozen Then
                            Dim aNewDate As Date?
                            Dim anOldDate As Date?
                            For Each anID In aWorkingEdition.ScheduleDefinition.GetActualFinishID
                                aNewDate = aWorkingEdition.GetMilestoneValue(anID)
                                anOldDate = aWorkingEdition.GetMilestoneValue(anID, ORIGINAL:=True) ' 
                                If aNewDate.HasValue And anOldDate.HasValue Then
                                    If DateDiff("d", anOldDate, aNewDate) >= 0 Then
                                        '** Now we should approve ??!
                                        '** at least we increase the revision count
                                        aWorkingEdition.Revision = aWorkingEdition.IncreaseRevison(MajorFlag:=False, MinorFlag:=True)
                                    End If
                                End If
                            Next
                        Else
                            aWorkingEdition.Revision = "V1.0"
                        End If
                    End If

                    ''' here take over the working edition to the alive edition
                    Me.AliveEditionUpdc = aWorkingEdition.Updc
                    _aliveedition = aWorkingEdition
                    _aliveedition.IsFrozen = True ''' freeze it
                    Me.WorkingEditionUpdc = Nothing
                    '' cannot generate an new updc on a created edition (getmax will not work on unpersisted objects)
                    If _aliveedition.IsCreated Then
                        _workingedition = aWorkingEdition.Clone(_aliveedition.Updc + 1)
                    Else
                        _workingedition = aWorkingEdition.Clone()
                    End If
                    '** set new working edition
                    Me.WorkingEditionUpdc = _workingedition.Updc
                    _workingedition.IsFrozen = False

                    ''' save the workspace schedule itself and the
                    ''' related objects
                    Return MyBase.Persist(timestamp)
                Else
                    '''
                    ''' no publish possible but persist
                    ''' 
                    Return MyBase.Persist(timestamp:=timestamp)
                End If

            ElseIf Me.IsAlive("Publish") Then

                '**** save without Milestone checking
                Return MyBase.Persist(timestamp:=timestamp)

            Else
                '** nothing changed
                '***
                Return False
            End If

            Return False
        End Function



        ''' <summary>
        ''' Persist with checking on publish
        ''' </summary>
        ''' <param name="timestamp"></param>
        ''' <param name="doFeedRecord"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Persist(Optional timestamp As DateTime? = Nothing, Optional doFeedRecord As Boolean = True) As Boolean Implements iormRelationalPersistable.Persist
            If Me.ScheduleDefinition.Autopublish Then
                Return Publish(timestamp:=timestamp)
            Else
                Return MyBase.Persist(timestamp:=timestamp, doFeedRecord:=doFeedRecord)
            End If
        End Function

        ''' <summary>
        ''' set the default values needed
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub WorkspaceSchedule_OnDefaultValuesNeeded(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreateDefaultValuesNeeded
            If Not e.Record.HasIndex(ConstFNIsActive) OrElse e.Record.GetValue(ConstFNIsActive) Is Nothing Then
                e.Record.SetValue(ConstFNIsActive, True)
            End If

        End Sub
        ''' <summary>
        ''' Event Handler for the RelationRetrieveNeeded event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub WorkspaceSchedule_OnRelationRetrieveNeeded(sender As Object, e As ormDataObjectRelationEventArgs) Handles Me.OnRelationRetrieveNeeded
            If Not Me.IsAlive(subname:="WorkspaceSchedule_OnRelationRetrieveNeeded") Then Return
            ''' check on PropertyValueLot
            ''' 
            If e.RelationID.ToUpper = ConstRAliveEdition.ToUpper Then
                If Me.AliveEditionUpdc IsNot Nothing Then
                    Dim aSchedule As ScheduleEdition = ScheduleEdition.Retrieve(UID:=Me.UID, updc:=Me.AliveEditionUpdc)
                    If aSchedule IsNot Nothing Then
                        e.RelationObjects.Add(aSchedule)
                    End If
                End If

                e.Finished = True
            ElseIf e.RelationID.ToUpper = ConstRWorkEdition.ToUpper Then
                If Me.WorkingEditionUpdc IsNot Nothing Then
                    Dim aSchedule As ScheduleEdition = ScheduleEdition.Retrieve(UID:=Me.UID, updc:=Me.WorkingEditionUpdc)
                    If aSchedule IsNot Nothing Then
                        e.RelationObjects.Add(aSchedule)
                    End If
                End If
                e.Finished = True
            End If
        End Sub
        ''' <summary>
        ''' Event Handler for the RelationRetrieveNeeded event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub WorkspaceSchedule_OnRelationCreateNeeded(sender As Object, e As ormDataObjectRelationEventArgs) Handles Me.OnRelationCreateNeeded
            If Not Me.IsAlive(subname:="WorkspaceSchedule_OnRelationCreateNeeded") Then Return
            ''' check on Workspace Relation
            ''' 
            If e.RelationID = ConstRWorkEdition Then
                Dim aSchedule As ScheduleEdition
                ''' try to create a new working version out of an existing alive schedule
                ''' 
                If Me.AliveEditionUpdc IsNot Nothing Then
                    Dim aAliveSchedule As ScheduleEdition = ScheduleEdition.Retrieve(UID:=Me.UID, updc:=Me.AliveEditionUpdc)
                    If aAliveSchedule IsNot Nothing Then
                        aSchedule = aAliveSchedule.Clone()
                        aSchedule.IsFrozen = False
                    End If
                End If
                '' create
                If aSchedule Is Nothing Then
                    aSchedule = ScheduleEdition.Create(uid:=Me.UID, workspaceID:=Me.WorkspaceID, domainid:=Me.DomainID, scheduletypeid:=Me.ScheduleTypeID)
                    '' try to get a missed one
                    If aSchedule Is Nothing Then
                        Dim aWorkspace As Commons.Workspace = Me.Workspace
                        If aWorkspace IsNot Nothing Then
                            aSchedule = ScheduleEdition.Retrieve(UID:=Me.UID, updc:=aWorkspace.MinScheduleUPDC + 1)
                        End If
                    End If
                End If
                If aSchedule IsNot Nothing Then
                    Me.WorkingEditionUpdc = aSchedule.Updc
                    e.RelationObjects.Add(aSchedule)
                    e.Finished = True
                End If
            End If


        End Sub


        ''' <summary>
        ''' returns a list of workspaceschedule of a given uid
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function AllByUID(UID As Long) As List(Of WorkspaceSchedule)
            Dim aCollection As New List(Of WorkspaceSchedule)
            Dim aRECORDCollection As List(Of ormRecord)
            Dim aTable As iormRelationalTableStore
            Dim Key(0) As Object
            Dim aRECORD As ormRecord
            ' set the primaryKey

            Key(0) = UID

            Try
                aTable = GetPrimaryTableStore(ConstPrimaryTableID)
                aRECORDCollection = aTable.GetRecordsBySql(wherestr:="[" & ConstFNUID & "] = " & CStr(UID))

                For Each aRECORD In aRECORDCollection
                    Dim aNewcurSchedule As New WorkspaceSchedule
                    If InfuseDataObject(record:=aRECORD, dataobject:=aNewcurSchedule) Then
                        aCollection.Add(item:=aNewcurSchedule)
                    End If
                Next
                Return aCollection

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="WorkspaceSchedule.AllbyUID")
            End Try


        End Function



        ''' <summary>
        ''' retrieves a a current schedule object for the workspace id 
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <param name="workspaceID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ormObjectOperationMethod(id:=ConstOPRetrieve, parameterEntries:={ConstFNUID, ConstFNWorkspaceID})> _
        Public Shared Function Retrieve(ByVal UID As Long, Optional ByVal workspaceID As String = Nothing) As WorkspaceSchedule
            ' if no workspaceID -> Default workspaceID
            If String.IsNullOrEmpty(workspaceID) Then workspaceID = CurrentSession.CurrentWorkspaceID
            Dim aWSObj As Workspace = Workspace.Retrieve(id:=workspaceID)
            '*
            If aWSObj Is Nothing Then
                Call CoreMessageHandler(message:="Can't load workspaceID definition", procedure:="WorkspaceSchedule.Retrieve", _
                                      argument:=workspaceID, messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If

            ' check now the stack
            For Each aWorkspaceID In aWSObj.FCRelyingOn
                ' check if in workspaceID any data -> fall back to default (should be base)
                Dim primarykey As Object() = {aWorkspaceID, UID}
                Dim aCurrSchedule As WorkspaceSchedule = ormBusinessObject.RetrieveDataObject(Of WorkspaceSchedule)(pkArray:=primarykey)
                If aCurrSchedule IsNot Nothing AndAlso aCurrSchedule.IsActive Then
                    Return aCurrSchedule
                End If
            Next

            Return Nothing
        End Function


        '' <summary>
        ''' load the object by the PrimaryKeys unique and do not overload from other workspaces
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <param name="workspaceID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RetrieveUnique(ByVal UID As Long, Optional ByVal workspaceID As String = Nothing) As WorkspaceSchedule
            If String.IsNullOrEmpty(workspaceID) Then workspaceID = CurrentSession.CurrentWorkspaceID
            Dim pkarry() As Object = {workspaceID, UID}
            Return ormBusinessObject.RetrieveDataObject(Of WorkspaceSchedule)(pkArray:=pkarry)
        End Function

        '**** getthe TrackDef
        '****
        Public Function GetDeliverableTrack() As Track
            Throw New NotImplementedException
            'Dim aTrackDef As New Track
            'Dim aCurrTarget As New CurrentTarget

            'If IsLoaded Then
            '    '-> UID= ME.UID
            '    If Not aCurrTarget.Inject(uid:=Me.UID, workspaceID:=Me.WorkspaceID) Then
            '        aCurrTarget.UPDC = 0
            '    End If
            '    If aTrackDef.Inject(deliverableUID:=Me.UID, _
            '                        scheduleUID:=Me.UID, _
            '                        scheduleUPDC:=Me.UPDC, _
            '                        targetUPDC:=aCurrTarget.UPDC) Then
            '        GetDeliverableTrack = aTrackDef
            '    End If
            'End If

            'GetDeliverableTrack = Nothing
        End Function

        ''' <summary>
        ''' create the persistable WorkspaceSchedule object
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <param name="workspaceID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(Optional ByVal UID As Long = 0, _
                                      Optional ByVal workspaceID As String = Nothing, _
                                      Optional domainid As String = Nothing, _
                                      Optional scheduletypeid As String = Nothing) As WorkspaceSchedule
            If String.IsNullOrEmpty(workspaceID) Then workspaceID = CurrentSession.CurrentWorkspaceID
            If String.IsNullOrWhiteSpace(scheduletypeid) Then scheduletypeid = CurrentSession.DefaultScheduleTypeID
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim aRecord As New ormRecord
            With aRecord
                .SetValue(ConstFNUID, UID)
                If Not String.IsNullOrEmpty(workspaceID) Then .SetValue(ConstFNWorkspaceID, workspaceID)
                .SetValue(ConstFNTypeid, scheduletypeid)
                .SetValue(ConstFNDomainID, domainid)
            End With
            Return ormBusinessObject.CreateDataObject(Of WorkspaceSchedule)(aRecord, checkUnique:=True)
        End Function

        ''' <summary>
        ''' handles the OnCreating Event to generate an new UID if necessary
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub WorkspaceSchedule_OnCreating(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreating
            Dim anUid As Long? = e.Record.GetValue(ConstFNUID)
            Dim aWorkspaceID As String = e.Record.GetValue(ConstFNWorkspaceID)
            Dim aScheduleTypeID As String = e.Record.GetValue(ConstFNTypeid)

            '* new uid
            If Not anUid.HasValue OrElse anUid = 0 Then
                anUid = Nothing 'reset to norhing
                Dim primarykey As Object() = {aWorkspaceID, anUid}
                If e.DataObject.ObjectPrimaryContainerStore.CreateUniquePkValue(pkArray:=primarykey) Then
                    e.Record.SetValue(ConstFNUID, primarykey(1)) ' to be created
                    e.Result = True
                    e.Proceed = True
                Else
                    CoreMessageHandler(message:="primary keys couldnot be created ?!", procedure:="WorkspaceSchedule.WorkspaceSchedule_OnCreating", _
                                       messagetype:=otCoreMessageType.InternalError)
                End If

            End If


        End Sub

        ''' <summary>
        ''' handles the OnCreated Event to generate a edition if necessary
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub WorkspaceSchedule_OnCreated(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreated

            ''' successfull until here
            If e.DataObject IsNot Nothing Then
                Dim aScheduleEdition As ScheduleEdition
                Dim aCollection = ScheduleEdition.AllByUID(Me.UID) 'test if we have schedules of this UID
                If aCollection.Count > 0 Then

                    ''' there are editions but is not clear
                    ''' to which this Workspace Schedule belongs ?!
                    ''' 
                    For Each anEdition In aCollection.OrderByDescending(Function(x) x.Updc)
                        If anEdition.WorkspaceID = Me.WorkspaceID Then
                            If _workingedition IsNot Nothing AndAlso Not anEdition.IsFrozen Then
                                Me.WorkingEditionUpdc = anEdition.Updc
                                _workingedition = anEdition ' set direct
                            ElseIf _aliveedition IsNot Nothing AndAlso anEdition.IsFrozen Then
                                Me.AliveEditionUpdc = anEdition.Updc
                                _aliveedition = anEdition ' set direct
                            End If

                        End If
                    Next
                End If
                ''' Here it should also be checked if the workspace ID is a base and has actuals
                ''' 
                If Not Me.Workspace.IsBasespace OrElse Not Me.Workspace.HasActuals Then
                    '' create the base and actual !
                    Dim aWorkspace As Workspace = Me.Workspace.GetFirstActual
                    Dim aBaseSchedule = ScheduleEdition.Create(uid:=Me.UID, workspaceID:=aWorkspace.ID, domainid:=Me.DomainID, scheduletypeid:=Me.ScheduleTypeID)
                    AddHandler Me.OnPersisted, AddressOf aBaseSchedule.Request_Perist
                End If

                '' was created we need to create a schedule edition anyway
                '' this will be an empty one (recursive cloneing from workspace stack should be implemented later)
                If _workingedition Is Nothing Then
                    If _aliveedition Is Nothing Then
                        aScheduleEdition = ScheduleEdition.Create(uid:=Me.UID, workspaceID:=Me.WorkspaceID, domainid:=Me.DomainID, scheduletypeid:=Me.ScheduleTypeID)
                    Else
                        '' clone the last alive edition for the working edition
                        aScheduleEdition = _aliveedition.Clone()
                    End If

                    Me.WorkingEditionUpdc = aScheduleEdition.Updc
                    Me.IsActive = True
                    _workingedition = aScheduleEdition ' set direct the relation
                End If


            End If



        End Sub

        ''' <summary>
        ''' Event Handler for Workspace Change
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub WorkspaceSchedule_OnWorkspaceChanged(sender As Object, e As SessionEventArgs)
            Throw New NotImplementedException("Workspace Schedule Event Reaction on OnWorkspaceChanged to be implemented")
        End Sub


    End Class

End Namespace
