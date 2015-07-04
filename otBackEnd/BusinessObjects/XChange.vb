
REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** XChangeManager Business Object Classes for On Track Database Backend Library
REM ***********
REM *********** Version: X.YY
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************
Option Explicit On
Imports System.Collections.Specialized

Imports OnTrack.Database
Imports OnTrack.Deliverables
Imports OnTrack.Commons
Imports OnTrack.Core


Namespace OnTrack.XChange

    ''' <summary>
    ''' XChangeable Interface for exchangeable objects
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iotXChangeable
        ''' <summary>
        ''' runs the XChange 
        ''' </summary>
        ''' <param name="envelope"></param>
        ''' <returns>True if successful</returns>
        ''' <remarks></remarks>
        Function RunXChange(ByRef envelope As XEnvelope, Optional ByRef msglog As BusinessObjectMessageLog = Nothing) As Boolean

        ''' <summary>
        ''' runs the Precheck
        ''' </summary>
        ''' <param name="envelope"></param>
        ''' <returns>True if successful</returns>
        ''' <remarks></remarks>
        Function RunXPreCheck(ByRef envelope As XEnvelope, Optional ByRef msglog As BusinessObjectMessageLog = Nothing) As Boolean

    End Interface
    ''' <summary>
    ''' XChange Commands
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otXChangeCommandType
        Update = 1
        Delete = 2
        CreateUpdate = 3
        Duplicate = 4
        Read = 5
    End Enum
    ''' <summary>
    ''' otXChangeConfigEntryType
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otXChangeConfigEntryType
        [Object] = 1
        ObjectEntry
    End Enum

    ''' <summary>
    ''' Interface for XConfigMembers
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IXChangeConfigEntry
        Inherits iormRelationalPersistable
        Inherits iormInfusable

        ''' <summary>
        ''' returns the Object entryname
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Property ObjectEntryname() As String

        ''' <summary>
        ''' returns the ID of the ConfigMember
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property XID() As String

        ''' <summary>
        ''' returns the name of the Object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Objectname() As String

        ''' <summary>
        ''' returns a List of Aliases
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Aliases() As List(Of String)

        ''' <summary>
        ''' returns the configname of this Member
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Configname() As String

        ''' <summary>
        ''' Has Alias
        ''' </summary>
        ''' <param name="alias"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property HasAlias([alias] As String) As Boolean
        '''' <summary>
        '''' Gets the S is compund entry.
        '''' </summary>
        '''' <value>The S is compund entry.</value>
        'ReadOnly Property IsCompundEntry As Boolean


        ''' <summary>
        ''' gets or sets the Xchange Command
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property XChangeCmd() As otXChangeCommandType

        ''' <summary>
        ''' gets the xchange object object definition
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property xobjectdefinition As ormObjectDefinition

        ''' <summary>
        ''' Primary Key Indexno
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property IDNO() As Long

        ''' <summary>
        ''' gets or sets the Xhanged Flag - value is not xchangend to and from Host Application
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property IsXChanged() As Boolean

        ''' <summary>
        ''' sets the Readonly Flag - value of the OTDB cannot be overwritten
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property IsReadOnly() As Boolean

        ''' <summary>
        ''' gets or sets the Attribute Entry Flag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property IsObjectEntry() As Boolean

        ''' <summary>
        ''' gets True if this is a Compound
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property IsCompound() As Boolean

        ''' <summary>
        ''' gets True if the Attribute is a Field
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property IsColumn() As Boolean

        ''' <summary>
        ''' gets True if this is an Object Entry
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property IsObject() As Boolean

        ''' <summary>
        ''' gets or sets the ordinal
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Ordinal() As Ordinal

        ''' <summary>
        ''' gets or sets the OrderedBy Flag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property IsOrderedBy() As Boolean

        ''' <summary>
        ''' returns the type of the entry
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Type As otXChangeConfigEntryType
        ''' <summary>
        ''' returns the object entry definition 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectEntryDefinition As iormObjectEntryDefinition
    End Interface

    ''' <summary>
    ''' describes an XChange XConfigMember Object
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <ormObject(id:=XChangeObject.ConstObjectID, usecache:=True, adddeletefieldbehavior:=True, adddomainbehavior:=True, _
        Modulename:=ConstModuleXChange, Description:="object definition for X Change configuration entry")> _
    Public Class XChangeObject
        Inherits XChangeConfigAbstractEntry
        Implements IXChangeConfigEntry


        Public Const ConstObjectID As String = "XChangeConfigObject"

#Region "Properties"

        ''' <summary>
        ''' gets or sets the Attribute Entry Flag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property IsObjectEntry() As Boolean Implements IXChangeConfigEntry.IsObjectEntry
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' gets True if the Attribute is a Field
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property IsColumn() As Boolean Implements IXChangeConfigEntry.IsColumn
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' gets True if this is an Object Entry
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property IsObject() As Boolean Implements IXChangeConfigEntry.IsObject
            Get
                Return True
            End Get
        End Property

        ''' <summary>
        ''' gets or sets the Xchange Command
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Property XChangeCmd() As otXChangeCommandType Implements IXChangeConfigEntry.XChangeCmd
            Get
                Return _xcmd
            End Get
            Set(value As otXChangeCommandType)
                MyBase.XChangeCmd = value
                ''' set also all the entries to the same XChangeCmd
                For Each anEntry In Me.XChangeConfig.GetEntriesByObjectName(Me.Objectname)
                    anEntry.XChangeCmd = value
                Next
            End Set
        End Property
#End Region

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase._type = otXChangeConfigEntryType.Object
        End Sub
        ''' <summary>
        ''' creates a persistable XChange member with primary Key
        ''' </summary>
        ''' <param name="configname"></param>
        ''' <param name="indexno"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal configname As String, indexno As Long, _
                                                Optional objectname As String = Nothing, _
                                                Optional xcmd As otXChangeCommandType = otXChangeCommandType.Read,
                                                Optional domainid As String = Nothing, _
                                                Optional runtimeonly As Boolean = False) As XChangeObject
            Dim aRecord As New ormRecord
            With aRecord
                .SetValue(ConstFNXConfigID, configname.ToUpper)
                .SetValue(constFNIDNo, indexno)
                If Not String.IsNullOrWhiteSpace(objectname) Then .SetValue(ConstFNObjectID, objectname.ToUpper)
                If Not String.IsnullorEmpty(domainID) Then .SetValue(ConstFNDomainID, domainid)
                .SetValue(constFNXCMD, xcmd)
                .SetValue(ConstFNTypeid, otXChangeConfigEntryType.Object)
                .SetValue(constFNOrderNo, indexno)
                .SetValue(constFNordinal, indexno)
            End With
            Return ormBusinessObject.CreateDataObject(Of XChangeObject)(aRecord, domainID:=domainid, checkUnique:=True, runtimeOnly:=runtimeonly)
        End Function

        ''' <summary>
        ''' retrieves a persistable XChange Object
        ''' </summary>
        ''' <param name="configname"></param>
        ''' <param name="indexno"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal configname As String, indexno As Long, Optional domainid As String = Nothing, Optional runtimeonly As Boolean = False) As XChangeObject
            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID
            Return ormBusinessObject.RetrieveDataObject(Of XChangeObject)({configname.ToUpper, indexno, domainid}, runtimeOnly:=runtimeonly)
        End Function
    End Class
    ''' <summary>
    ''' describes object entry definition for X Change configuration entry
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=XChangeObjectEntry.ConstObjectID, usecache:=True, adddeletefieldbehavior:=True, adddomainbehavior:=True, _
        Modulename:=ConstModuleXChange, Description:="object entry definition for X Change configuration entry")> _
    Public Class XChangeObjectEntry
        Inherits XChangeConfigAbstractEntry
        Implements IXChangeConfigEntry

        Public Const ConstObjectID As String = "XChangeConfigObjectEntry"

#Region "Properties"


        ''' <summary>
        ''' gets or sets the Attribute Entry Flag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property IsObjectEntry() As Boolean Implements IXChangeConfigEntry.IsObjectEntry
            Get
                Return True
            End Get

        End Property

        ''' <summary>
        ''' gets True if this is an Object Entry
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property IsObject() As Boolean Implements IXChangeConfigEntry.IsObject
            Get
                Return False
            End Get
        End Property
        ''' <summary>
        ''' gets or sets the Dynamic Flag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsDynamicAttribute() As Boolean
            Get
                Return _isDynamicAttribute
            End Get
            Set(value As Boolean)
                SetValue(constFNIsDynamic, value)
            End Set
        End Property
#End Region

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase._type = otXChangeConfigEntryType.ObjectEntry
        End Sub

        ''' <summary>
        ''' creates a persistable XChange Objectentry
        ''' </summary>
        ''' <param name="configname"></param>
        ''' <param name="indexno"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal configname As String, indexno As Long, Optional domainid As String = Nothing, Optional runtimeonly As Boolean = False) As XChangeObjectEntry
            Dim aRecord As New ormRecord
            With aRecord
                .SetValue(ConstFNXConfigID, configname.ToUpper)
                .SetValue(constFNIDNo, indexno)
                .SetValue(ConstFNDomainID, domainid)
                .SetValue(ConstFNTypeid, otXChangeConfigEntryType.ObjectEntry)
            End With
            Return ormBusinessObject.CreateDataObject(Of XChangeObjectEntry)(aRecord, domainID:=domainid, checkUnique:=True, runtimeOnly:=runtimeonly)
        End Function

        ''' <summary>
        ''' retrieves a persistable XChange Object Entry
        ''' </summary>
        ''' <param name="configname"></param>
        ''' <param name="indexno"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal configname As String, indexno As Long, Optional domainid As String = Nothing, Optional runtimeonly As Boolean = False) As XChangeObjectEntry
            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID
            Return ormBusinessObject.RetrieveDataObject(Of XChangeObjectEntry)({configname.ToUpper, indexno, domainid}, domainID:=domainid, runtimeOnly:=runtimeonly)
        End Function
    End Class

    ''' <summary>
    ''' abstract class to describe an XChangeConfiguration EntryMember - an individual item
    ''' </summary>
    ''' <remarks></remarks>

    <ormObject(id:=XChangeConfigAbstractEntry.ConstObjectID, usecache:=True, adddeletefieldbehavior:=True, adddomainbehavior:=True, _
        Modulename:=ConstModuleXChange, Description:="abstract entry definition for X Change configuration")> _
    Public MustInherit Class XChangeConfigAbstractEntry
        Inherits ormBusinessObject
        Implements iormInfusable, iormRelationalPersistable, IXChangeConfigEntry

        Public Const ConstObjectID = "XChangeConfigAbstractEntry"
        ''' <summary>
        ''' Table
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=3, usecache:=True)> Public Const ConstPrimaryTableID = "tblXChangeConfigEntries"

        ''' <summary>
        ''' keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=XChangeConfiguration.constObjectID & "." & XChangeConfiguration.constFNID, PrimaryKeyOrdinal:=1, _
                        title:="XChangeConfigID", description:="name of the eXchange Configuration")> Public Const ConstFNXConfigID = XChangeConfiguration.constFNID

        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=2,
                        title:="Identity Number", description:="unique id in the the eXchange Configuration")> Public Const constFNIDNo = "IDNO"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=3, _
           useforeignkey:=otForeignKeyImplementation.None)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        ''' <summary>
        ''' foreign key
        ''' </summary>
        ''' <remarks></remarks>
        <ormForeignKey(useforeignkey:=otForeignKeyImplementation.NativeDatabase, _
            entrynames:={ConstFNXConfigID, ConstFNDomainID}, _
            foreignkeyreferences:={XChangeConfiguration.constObjectID & "." & XChangeConfiguration.constFNID, _
            XChangeConfiguration.constObjectID & "." & XChangeConfiguration.ConstFNDomainID})> Public Const constFKXConfig = "FK_XCONFIG"

        ''' <summary>
        ''' fields
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=ormObjectDefinition.ConstObjectID & "." & ormObjectDefinition.ConstFNID, _
                        useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const ConstFNObjectID = "objectID"

        <ormObjectEntry(referenceObjectEntry:=ormObjectFieldEntry.ConstObjectID & "." & ormObjectFieldEntry.ConstFNEntryName, _
                       isnullable:=True)> Public Const ConstFNEntryname = "entryname" ' might be null since only object are also members

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True,
                        title:="Description", description:="Description of the member")> Public Const ConstFNDesc = "desc"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, isnullable:=True,
                        properties:={ObjectEntryProperty.Keyword}, _
                        title:="XChange ID", description:="ID  of the Attribute in theObjectDefinition")> Public Const ConstFNXID = "id"

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True,
                        title:="ordinal", description:="ordinal for the Attribute Mapping")> Public Const constFNordinal = "ORDINALVALUE"

        <ormObjectEntry(Datatype:=otDataType.Text, title:="Type", defaultvalue:=otXChangeConfigEntryType.ObjectEntry, isnullable:=True, _
            description:="type of the XChange configuration entry")> Public Const ConstFNTypeid = "typeid"


        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0", _
            title:="Is Entry Read-Only", description:="Set if this entry is read-only - value in OTDB cannot be overwritten")>
        Public Const constFNIsReadonly = "isreadonly"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0", _
            title:="Is ordered", description:="Set if this entry is ordered")>
        Public Const constFNIsOrder = "isorder"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0", _
            title:="Is dynamic attribute", description:="Set if this entry is dynamic")>
        Public Const constFNIsDynamic = "isdynamic"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0", _
            title:="Attribute is not exchanged", description:="Set if this attribute is not exchanged")>
        Public Const constFNIsNotXChanged = "isnotxchg"

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True,
                        properties:={ObjectEntryProperty.Keyword}, _
                        title:="XChange Command", description:="XChangeCommand to run on this")> Public Const constFNXCMD = "xcmd"

        <ormObjectEntry(Datatype:=otDataType.Long, isnullable:=True,
            title:="Order Number", description:="ordinal number in which entriy is processed")>
        Public Const constFNOrderNo = "orderno"


        ''' <summary>
        ''' Mappings
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNXConfigID)> Protected _configname As String = String.empty
        <ormObjectEntryMapping(EntryName:=constFNIDNo)> Protected _idno As Long

        <ormObjectEntryMapping(EntryName:=ConstFNXID)> Protected _xid As String

        <ormObjectEntryMapping(EntryName:=ConstFNObjectID)> Protected _objectname As String
        <ormObjectEntryMapping(EntryName:=ConstFNEntryname)> Protected _entryname As String

        '<otColumnMapping(ColumnName:=ConstFNordinal)> do not since we cannot map it
        Private _ordinal As Ordinal = New Ordinal(0)

        <ormObjectEntryMapping(EntryName:=ConstFNDesc)> Protected _desc As String = String.empty
        <ormObjectEntryMapping(EntryName:=constFNIsNotXChanged)> Protected _isNotXChanged As Boolean
        <ormObjectEntryMapping(EntryName:=constFNIsReadonly)> Protected _isReadOnly As Boolean

        <ormObjectEntryMapping(EntryName:=ConstFNTypeid)> Protected _type As otXChangeConfigEntryType

        <ormObjectEntryMapping(EntryName:=constFNXCMD)> Protected _xcmd As otXChangeCommandType = 0
        <ormObjectEntryMapping(EntryName:=constFNIsOrder)> Protected _isOrdered As Boolean
        <ormObjectEntryMapping(EntryName:=constFNOrderNo)> Protected _orderNo As Long
        <ormObjectEntryMapping(EntryName:=constFNIsDynamic)> Protected _isDynamicAttribute As Boolean

        'dynamic
        Protected _EntryDefinition As iormObjectEntryDefinition
        Protected _ObjectDefinition As ormObjectDefinition
        Protected _XChangeConfig As XChangeConfiguration ' backlink cache


        '** initialize
        Public Sub New()
            Call MyBase.New()

            _EntryDefinition = Nothing
        End Sub

#Region "Properties"


        ''' <summary>
        ''' gets or sets the XChange ID for the Member
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property XID() As String Implements IXChangeConfigEntry.XID
            Get
                Return _xid
            End Get
            Set(value As String)
                SetValue(ConstFNXID, value)
            End Set
        End Property

        ''' <summary>
        ''' gets the entryname of the data object data object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ObjectEntryname() As String Implements IXChangeConfigEntry.ObjectEntryname
            Get
                Return _entryname
            End Get
            Set(value As String)
                SetValue(ConstFNEntryname, value)
            End Set
        End Property

        ''' <summary>
        ''' gets or sets the objectname to which the entry belongs
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Objectname() As String Implements IXChangeConfigEntry.Objectname
            Get
                Return _objectname
            End Get
            Set(value As String)
                SetValue(ConstFNObjectID, value)
            End Set
        End Property

        ''' <summary>
        ''' gets or sets the objectname to which the entry belongs
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Type() As otXChangeConfigEntryType Implements IXChangeConfigEntry.Type
            Get
                Return _type
            End Get
            Set(value As otXChangeConfigEntryType)
                SetValue(ConstFNTypeid, value)
            End Set
        End Property
        ''' <summary>
        ''' gets the configname
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Configname() As String Implements IXChangeConfigEntry.Configname
            Get
                Return _configname
            End Get
            Set(value As String)
                SetValue(ConstFNXConfigID, value)
            End Set
        End Property


        ''' <summary>
        ''' gets the Aliases of the XChange Member
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Aliases() As List(Of String) Implements IXChangeConfigEntry.Aliases
            Get
                If Not Me.[ObjectEntryDefinition] Is Nothing Then
                    Return _EntryDefinition.Aliases.ToList
                Else
                    Return New List(Of String)
                End If
            End Get

        End Property

        ''' <summary>
        ''' gets true if the XChangeMember has the Alias
        ''' </summary>
        ''' <param name="alias"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property HasAlias([alias] As String) As Boolean Implements IXChangeConfigEntry.HasAlias
            Get
                If Me.[ObjectEntryDefinition] IsNot Nothing Then
                    Return Me.[ObjectEntryDefinition].Aliases.Count = 0
                Else
                    Return False
                End If
            End Get
        End Property

        ''' <summary>
        ''' gets or sets the Xchange Command
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property XChangeCmd() As otXChangeCommandType Implements IXChangeConfigEntry.XChangeCmd
            Get
                Return _xcmd
            End Get
            Set(value As otXChangeCommandType)
                SetValue(constFNXCMD, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the domain ID - set it to nothing alwyas the currentdomainId will apply
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Property Domainid As String
            Get
                If String.IsNullOrWhiteSpace(MyBase.DomainID) Then Return CurrentSession.CurrentDomainID
            End Get
            Set(value As String)
                MyBase.DomainID = value
            End Set
        End Property
        ''' <summary>
        ''' gets the ObjectEntry Definition for the XChange Member
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property [ObjectEntryDefinition] As iormObjectEntryDefinition Implements IXChangeConfigEntry.ObjectEntryDefinition
            Get
                If _EntryDefinition Is Nothing AndAlso IsAlive(throwError:=False) Then

                    If _entryname IsNot Nothing And Me.Objectname IsNot Nothing Then
                        _EntryDefinition = CurrentSession.Objects(domainid:=Me.Domainid).GetEntryDefinition(objectname:=Me.Objectname, entryname:=Me.ObjectEntryname)
                    ElseIf Me.Objectname IsNot Nothing And Me.XID IsNot Nothing Then
                        _EntryDefinition = CurrentSession.Objects(domainid:=Me.Domainid).GetEntriesByXID(xid:=_xid, objectname:=Me.Objectname).First
                    Else
                        _EntryDefinition = CurrentSession.Objects(domainid:=Me.Domainid).GetEntriesByXID(xid:=_xid).First
                    End If

                End If

                Return _EntryDefinition
            End Get

        End Property
        ''' <summary>
        ''' return the ObjectDefinition of the associated XObject (not the XObjectEntry - nor the Objectdefinition of the XchangeConfig itself)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Property [XObjectDefinition] As ormObjectDefinition Implements IXChangeConfigEntry.xobjectdefinition
            Get

                If (Me.IsCreated Or Me.IsLoaded) And _ObjectDefinition Is Nothing Then
                    If Not String.IsNullOrWhiteSpace(Me.Objectname) Then
                        Dim aDefinition As iormObjectDefinition = CurrentSession.Objects(domainid:=Me.Domainid).GetObjectDefinition(Me.Objectname)
                        If aDefinition IsNot Nothing Then _ObjectDefinition = aDefinition
                    End If
                End If

                ' return
                Return _ObjectDefinition
            End Get
            Set(value As ormObjectDefinition)
                _ObjectDefinition = value
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the ordinal
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Ordinal() As Ordinal Implements IXChangeConfigEntry.Ordinal
            Get
                Return _ordinal
            End Get
            Set(value As Ordinal)
                SetValue(constFNordinal, value)
                _ordinal = value 'cache
            End Set
        End Property


        ''' <summary>
        ''' Primary Key Indexno
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IDNO() As Long Implements IXChangeConfigEntry.IDNO
            Get
                Return _idno
            End Get
            Set(value As Long)
                Throw New InvalidOperationException("IDNO must not be set by property since it is a primary key")
            End Set
        End Property

        ''' <summary>
        ''' gets or sets the Xhanged Flag - value is not xchangend to and from Host Application
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsXChanged() As Boolean Implements IXChangeConfigEntry.IsXChanged
            Get
                Return Not _isNotXChanged
            End Get
            Set(value As Boolean)
                SetValue(constFNIsNotXChanged, Not value)
            End Set
        End Property

        ''' <summary>
        ''' sets the Readonly Flag - value of the OTDB cannot be overwritten
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsReadOnly() As Boolean Implements IXChangeConfigEntry.IsReadOnly
            Get
                Return _isReadOnly
            End Get
            Set(value As Boolean)
                SetValue(constFNIsReadonly, value)
            End Set
        End Property
        ''' <summary>
        ''' returns true if this entry is an object entry
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property IsObjectEntry() As Boolean Implements IXChangeConfigEntry.IsObjectEntry
            Get
                Return Me.Type = otXChangeConfigEntryType.ObjectEntry
            End Get
        End Property

        ''' <summary>
        ''' gets True if this is a Compound
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property IsCompound() As Boolean Implements IXChangeConfigEntry.IsCompound
            Get
                If Me.Type = otXChangeConfigEntryType.ObjectEntry Then
                    Dim anObjectEntry = Me.ObjectEntryDefinition
                    If anObjectEntry IsNot Nothing Then
                        Return anObjectEntry.IsCompound
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If
            End Get

        End Property
        ''' <summary>
        ''' gets True if the Attribute is a Column
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property IsColumn() As Boolean Implements IXChangeConfigEntry.IsColumn
            Get
                If Me.Type = otXChangeConfigEntryType.ObjectEntry Then
                    Dim anObjectEntry = Me.ObjectEntryDefinition
                    If anObjectEntry IsNot Nothing Then
                        Return anObjectEntry.IsContainer
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If
            End Get
        End Property

        ''' <summary>
        ''' gets True if this is entry is an Object 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property IsObject() As Boolean Implements IXChangeConfigEntry.IsObject
            Get
                Return Me.Type = otXChangeConfigEntryType.Object
            End Get
        End Property

        ''' <summary>
        ''' gets or sets the OrderedBy Flag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsOrderedBy() As Boolean Implements IXChangeConfigEntry.IsOrderedBy
            Get
                Return _isOrdered
            End Get
            Set(value As Boolean)
                SetValue(constFNIsOrder, value)
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the Order ordinal
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Orderno() As Long
            Get
                Return _orderNo
            End Get
            Set(value As Long)
                SetValue(constFNOrderNo, value)
            End Set
        End Property

        ''' <summary>
        ''' returns the the xchange config of this entry
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property XChangeConfig As XChangeConfiguration
            Get
                If _XChangeConfig Is Nothing Then _XChangeConfig = Xchange.XChangeConfiguration.Retrieve(configname:=_configname)
                Return _XChangeConfig
            End Get
        End Property
#End Region

        ''' <summary>
        ''' Increment ordinal
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Incordinal() As Ordinal
            If IsNumeric(_ordinal) Then
                _ordinal = New Ordinal(_ordinal.ToUInt64 + 1)
            ElseIf IsEmpty(_ordinal) Then
                _ordinal = New Ordinal(1)
            Else
                Call CoreMessageHandler(procedure:="XConfigMember.incordinal", message:="ordinal is not numeric")
                Incordinal = Nothing
                Exit Function
            End If
            Incordinal = _ordinal
        End Function

        ''' <summary>
        ''' infuses the XChange member from the record
        ''' </summary>
        ''' <param name="record"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Sub XChangeConfigAbstractEntry_OnInfused(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnInfused
            Dim aValue As Object

            Try
                Dim isnull As Boolean
                aValue = Record.GetValue(constFNordinal, isNull:=isnull)
                If isnull Then
                    _ordinal = New Ordinal(0)
                Else
                    If IsNumeric(aValue) Then
                        _ordinal = New Ordinal(CLng(aValue))
                    Else
                        _ordinal = New Ordinal(CStr(aValue))
                    End If
                End If
                e.Proceed = True
                Exit Sub
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="XChangeConfigAbstractEntry_OnInfused.OnInfused")
                e.AbortOperation = True
            End Try

        End Sub

        ''' <summary>
        ''' infuses the XChange member from the record
        ''' </summary>
        ''' <param name="record"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Sub XChangeConfigAbstractEntry_OnFed(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnFed

            Try
                e.Record.SetValue(constFNordinal, _ordinal.Value.ToString)
                If Orderno = 0 And Me.Ordinal <> New Ordinal(0) And Me.Ordinal.Type = OrdinalType.longType Then
                    Me.Orderno = Me.Ordinal.Value
                    e.Record.SetValue(constFNOrderNo, _ordinal.Value.ToString)
                End If
                e.Result = True
                e.Proceed = True
                Exit Sub
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="XchangeConfigAbstractEntry.XChangeConfigAbstractEntry_OnFed")
                e.AbortOperation = True
            End Try

        End Sub



    End Class

    ''' <summary>
    ''' CLASS XConfig defines how data can be exchanged with the XChange Manager
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <ormChangeLogEntry(application:=ot.ConstAssemblyName, module:=ot.ConstModuleXChange, version:=1, release:=1, patch:=1, changeimplno:=2, _
        description:="resetting the entries")> _
    <ormObject(ID:=XChangeConfiguration.constObjectID, version:=1, usecache:=True, adddomainbehavior:=True, adddeletefieldbehavior:=True, _
        modulename:=ConstModuleXChange, description:="defines how data can be exchanged with the XChange Manager")> _
    Public Class XChangeConfiguration
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable

        'Implements iOTDBXChange
        Public Const constObjectID = "XChangeConfig"

        ''' <summary>
        ''' Table
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(Version:=2, usecache:=True)> Public Const ConstPrimaryTableID = "tblXChangeConfigs"

        ''' <summary>
        ''' Keys
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=1, _
             properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty}, _
             Title:="Name", Description:="Name of XChange Configuration")> Public Const constFNID = "configname"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2, _
           useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        ''' <summary>
        ''' Fields
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True,
             Title:="Description", Description:="Description of XChange Configuration")>
        Public Const constFNDesc = "desc"

        <ormObjectEntry(Datatype:=otDataType.Memo, isnullable:=True,
             Title:="Comments", Description:="Comments")> Public Const constFNTitle = "cmt"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False,
             Title:="IsDynamic", Description:="the XChange Config accepts dynamic addition of XChangeIDs")> Public Const constFNDynamic = "isdynamic"

        <ormObjectEntry(referenceObjectEntry:=XOutline.ConstObjectID & "." & XOutline.ConstFNID, isnullable:=True, _
               Title:="Outline ID", Description:="ID to the associated Outline")> Public Const constFNOutline = "outline"

        ''' <summary>
        ''' Mappings
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=constFNID)> Private _configname As String = String.empty
        <ormObjectEntryMapping(EntryName:=constFNDesc)> Private _description As String
        <ormObjectEntryMapping(EntryName:=constFNDynamic)> Private _DynamicAttributes As Boolean
        <ormObjectEntryMapping(EntryName:=constFNOutline)> Private _outlineid As String

        ''' <summary>
        ''' Relations ! BEWARE HARDCODED Typeids
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        '''
        '*** relation to xconfig object entries
        <ormRelation(linkobject:=GetType(XChangeObjectEntry), cascadeOnDelete:=True, cascadeOnUpdate:=True, _
            fromEntries:={constFNID}, toEntries:={XChangeObjectEntry.ConstFNXConfigID}, _
            linkjoin:=" AND [" & XChangeObjectEntry.ConstFNTypeid & "] ='ObjectEntry'")> _
        Public Const ConstRObjectEntries = "XCHANGEENTRIES"

        <ormObjectEntryMapping(RelationName:=ConstRObjectEntries, infuseMode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand, _
            keyentries:={XChangeObjectEntry.constFNIDNo})> Private WithEvents _ObjectEntryCollection As New ormRelationCollection(Of XChangeObjectEntry)(Me, {XChangeConfigAbstractEntry.constFNIDNo})

        '*** relation xconfig objects
        <ormRelation(linkobject:=GetType(XChangeObject), cascadeOnDelete:=True, cascadeOnUpdate:=True, _
           fromEntries:={constFNID}, toEntries:={XChangeObject.ConstFNXConfigID}, _
           linkjoin:=" AND [" & XChangeObject.ConstFNTypeid & "] ='Object'")> Public Const ConstRObjects = "XCHANGEOBJECTS"

        <ormObjectEntryMapping(RelationName:=ConstRObjects, infuseMode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand, _
            keyentries:={XChangeObject.constFNIDNo})> _
        Private WithEvents _ObjectCollection As New ormRelationCollection(Of XChangeObject)(Me, {XChangeObject.constFNIDNo})

        ''' <summary>
        ''' Relation to Outline
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkObject:=GetType(XOutline), toprimaryKeys:={constFNOutline}, _
                     cascadeonCreate:=False, cascadeOnDelete:=False, cascadeOnUpdate:=False)> _
        Public Const ConstROutline = "RELOutline"

        <ormObjectEntryMapping(relationName:=ConstROutline, infusemode:=otInfuseMode.OnDemand)> Private _outline As XOutline


        ''' <summary>
        '''  dynamic entries
        ''' </summary>
        ''' <remarks></remarks>
        Private _msglog As New BusinessObjectMessageLog
        Private _processedDate As Date = constNullDate

        ' members itself per key:=indexnumber, item:=IXChangeConfigEntry
        'Private _members As New SortedDictionary(Of Long, IXChangeConfigEntry)
        Private _entriesByordinal As New SortedDictionary(Of Ordinal, List(Of IXChangeConfigEntry))

        ' reference object order list to work through members in the row of the exchange
        Private _ObjectDictionary As New Dictionary(Of String, XChangeObject)
        Private _objectsByOrderDirectory As New SortedDictionary(Of Long, XChangeObject)

        ' reference Attributes list to work
        Private _entriesXIDDirectory As New Dictionary(Of String, XChangeObjectEntry)
        Private _entriesByObjectnameDirectory As New Dictionary(Of String, List(Of XChangeObjectEntry))
        Private _entriesXIDList As New Dictionary(Of String, List(Of XChangeObjectEntry)) ' list if IDs are not unique
        Private _aliasDirectory As New Dictionary(Of String, List(Of XChangeObjectEntry))

        ' object ordinalMember -> Members which are driving the ordinal of the complete eXchange
        ' Private _orderByMembers As New Dictionary(Of Object, IXChangeConfigEntry)




#Region "Properties"


        ''' <summary>
        ''' Gets or sets the S outlineid.
        ''' </summary>
        ''' <value>The S outlineid.</value>
        Public Property OutlineID() As String
            Get
                Return Me._outlineid
            End Get
            Set(value As String)
                SetValue(constFNOutline, value)
            End Set
        End Property

        ''' <summary>
        ''' returns the outline object for this xchangeconfiguration
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Outline As XOutline
            Get
                If _outlineid Is Nothing Then Return Nothing
                If Me.GetRelationStatus(ConstROutline) = ormRelationManager.RelationStatus.Unloaded Then InfuseRelation(ConstROutline)
                Return _outline
            End Get
        End Property


        ''' <summary>
        ''' Gets or sets the dynamic attributes.
        ''' </summary>
        ''' <value>The S dynamic attributes.</value>
        Public Property AllowDynamicEntries() As Boolean
            Get
                Return Me._DynamicAttributes
            End Get
            Set(value As Boolean)
                SetValue(constFNDynamic, value)
            End Set
        End Property


        ''' <summary>
        ''' gets name of configuration
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Configname()
            Get
                Configname = _configname
            End Get

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
                SetValue(constFNDesc, value)
            End Set
        End Property
        ''' <summary>
        ''' sets the dynamic processed date
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ProcessedDate() As Date
            Get
                ProcessedDate = _processedDate
            End Get
            Set(value As Date)
                _processedDate = value
            End Set
        End Property

        ''' <summary>
        ''' get the number of objects
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property NoObjects() As Long
            Get
                Return _ObjectCollection.Count
            End Get
        End Property
        ''' <summary>
        ''' get the number of entries
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property NoObjectEntries() As Long
            Get
                Return _ObjectEntryCollection.Count
            End Get
        End Property

#End Region


        ''' <summary>
        '''  get the maximal ordinal of exchange object entry as long if it is numeric
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMaxordinalNo() As Long
            If Not IsAlive(subname:="GetmaxordinalNo") Then Return 0
            Return _entriesByordinal.Keys.Select(Function(x) CLng(x.Value)).Max()
        End Function

        ''' <summary>
        ''' returns the maximal index number of a xchange entry
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMaxIDNO() As Long

            If _ObjectEntryCollection.Count > 0 AndAlso _ObjectCollection.Count > 0 Then
                Dim i As ULong = Me.ObjectEntryIDNos.Max
                Dim j As ULong = Me.ObjectIDNos.Max
                If i > j Then Return i
                Return j
            ElseIf _ObjectEntryCollection.Count > 0 AndAlso _ObjectCollection.Count = 0 Then
                Return Me.ObjectEntryIDNos.Max
            ElseIf _ObjectEntryCollection.Count = 0 AndAlso _ObjectCollection.Count > 0 Then
                Return Me.ObjectIDNos.Max
            Else
                Return 0
            End If

        End Function


        ''' <summary>
        ''' gets the highest XCommand Ranking
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetHighestXCmd() As otXChangeCommandType

            Dim aHighestXcmd As otXChangeCommandType

            aHighestXcmd = 0

            Dim listofObjects As List(Of XChangeObject) = Me.[XChangeobjects]
            If listofObjects.Count = 0 Then
                Return 0
            End If

            For Each aChangeMember As XChangeObject In listofObjects
                'aChangeMember = m
                Select Case aChangeMember.XChangeCmd
                    Case otXChangeCommandType.Read
                        If aHighestXcmd = 0 Then
                            aHighestXcmd = aChangeMember.XChangeCmd
                        Else
                            'aHighestXcmd = aChangeMember.xChangeCmd
                        End If

                    Case otXChangeCommandType.Update
                        If aHighestXcmd = 0 Or aHighestXcmd = otXChangeCommandType.Read Then
                            aHighestXcmd = aChangeMember.XChangeCmd
                        Else
                            'aHighestXcmd = aChangeMember.xChangeCmd
                        End If
                    Case otXChangeCommandType.CreateUpdate
                        If aHighestXcmd = 0 Or aHighestXcmd = otXChangeCommandType.Read Or aHighestXcmd = otXChangeCommandType.CreateUpdate Then
                            aHighestXcmd = aChangeMember.XChangeCmd
                        Else
                            'aHighestXcmd = aChangeMember.xChangeCmd

                        End If
                End Select
            Next

            Return aHighestXcmd
        End Function

        '*** get the highest need XCMD to run the attributes XCMD
        '***
        Public Function GetHighestObjectXCmd(ByVal objectname As String) As otXChangeCommandType

            Dim aHighestXcmd As otXChangeCommandType

            aHighestXcmd = 0

            Dim listofAttributes As List(Of XChangeObjectEntry) = Me.GetObjectEntries(objectname:=objectname)
            If listofAttributes.Count = 0 Then
                Return 0
            End If

            For Each aChangeMember As XChangeObjectEntry In listofAttributes
                'aChangeMember = m
                Select Case aChangeMember.XChangeCmd
                    Case otXChangeCommandType.Delete
                        If aHighestXcmd = 0 Then
                            aHighestXcmd = aChangeMember.XChangeCmd
                        Else
                            'aHighestXcmd = aChangeMember.xChangeCmd
                        End If

                    Case otXChangeCommandType.Read
                        If aHighestXcmd = 0 Or aHighestXcmd = otXChangeCommandType.Delete Then
                            aHighestXcmd = aChangeMember.XChangeCmd
                        Else
                            'aHighestXcmd = aChangeMember.xChangeCmd
                        End If

                    Case otXChangeCommandType.Update
                        If aHighestXcmd = 0 Or aHighestXcmd = otXChangeCommandType.Read Or aHighestXcmd = otXChangeCommandType.Delete Then
                            aHighestXcmd = aChangeMember.XChangeCmd
                        Else
                            'aHighestXcmd = aChangeMember.xChangeCmd
                        End If
                    Case otXChangeCommandType.CreateUpdate
                        If aHighestXcmd = 0 Or aHighestXcmd = otXChangeCommandType.Delete Or aHighestXcmd = otXChangeCommandType.Read _
                            Or aHighestXcmd = otXChangeCommandType.CreateUpdate Then
                            aHighestXcmd = aChangeMember.XChangeCmd
                        Else
                            'aHighestXcmd = aChangeMember.xChangeCmd

                        End If
                End Select
            Next

            Return aHighestXcmd
        End Function
        '*** set the ordinal for a given ID
        ''' <summary>
        ''' sets the ordinal for an ID
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <param name="ordinal"></param>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Function SetOrdinalForXID(ByVal XID As String, ByVal ordinal As Object, _
                                         Optional ByVal objectname As String = Nothing) As Boolean
            Dim anEntry As New XChangeObjectEntry
            ' Nothing
            If Not IsAlive("setOrdinalForXid") Then Return False


            ' get the entry
            anEntry = Me.GetEntryByXID(XID, objectname)
            If anEntry Is Nothing And Not Me.AllowDynamicEntries Then
                Return False
            ElseIf anEntry Is Nothing And Me.AllowDynamicEntries Then
                Return Me.AddEntryByXID(Xid:=XID, ordinal:=ordinal, objectname:=objectname)
            ElseIf Not anEntry.IsAlive(throwError:=False) Then
                Return False
            End If

            If Not TypeOf ordinal Is OnTrack.Database.Ordinal Then
                ordinal = New Ordinal(ordinal)
            End If
            anEntry.Ordinal = ordinal
            AddOrdinalReference(anEntry)
            Return True
        End Function
        '*** set objectXCmd set the maximum XCMD
        ''' <summary>
        ''' set the object xchange command
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="xchangecommand"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetObjectXCmd(ByVal name As String,
                                      ByVal xchangecommand As otXChangeCommandType) As Boolean
            Dim aMember As New XChangeObject

            ' Nothing
            If Not Me.IsLoaded And Not Me.IsCreated Then
                SetObjectXCmd = False
                Exit Function
            End If

            ' return if exists
            If Not _ObjectDictionary.ContainsKey(key:=name) Then
                SetObjectXCmd = False
                Exit Function
            Else
                aMember = _ObjectDictionary.Item(key:=name)
                ' depending what the current object xcmd, set it to "max" operation
                Select Case aMember.XChangeCmd

                    Case otXChangeCommandType.Update
                        If xchangecommand <> otXChangeCommandType.Read Then
                            aMember.XChangeCmd = xchangecommand
                        End If
                    Case otXChangeCommandType.Delete
                        ' keep it
                    Case otXChangeCommandType.CreateUpdate
                        If xchangecommand <> otXChangeCommandType.Read And xchangecommand <> otXChangeCommandType.Update Then
                            aMember.XChangeCmd = xchangecommand
                        End If
                    Case otXChangeCommandType.Duplicate
                        ' keep it
                    Case otXChangeCommandType.Read
                        aMember.XChangeCmd = xchangecommand
                End Select

            End If
            SetObjectXCmd = True
        End Function

        ''' <summary>
        ''' refresh all ObjectLoads
        ''' </summary>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RefreshObjects(Optional domainid As String = Nothing) As Boolean
            If Not Me.IsAlive("RefreshObects") Then Return False

            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID
            For Each anXObject As XChangeObject In _ObjectCollection
                CurrentSession.Objects(domainid:=domainid).GetObjectDefinition(anXObject.Objectname)
            Next
            Return True
        End Function

        '*** add an Object by Name
        '***
        ''' <summary>
        ''' Adds an object to exchange by name and orderno
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="orderno"></param>
        ''' <param name="xcmd"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddObjectByName(ByVal name As String,
                                        Optional domainid As String = Nothing, _
                                        Optional ByVal orderno As Long = 0,
                                        Optional ByVal xcmd As otXChangeCommandType = 0) As Boolean

            Dim aXchangeObject As New XChangeObject
            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID
            Dim anObjectDef As ormObjectDefinition = CurrentSession.Objects(domainid:=domainid).GetObjectDefinition(name)
            name = name.ToUpper
            If xcmd = 0 Then xcmd = otXChangeCommandType.Read

            ' Nothing
            If Not Me.IsAlive(subname:="AddObjectByName") Then
                Return False
            End If

            ' return if exists
            If _ObjectDictionary.ContainsKey(key:=name) Then
                If xcmd = 0 Then
                    aXchangeObject = _ObjectDictionary.Item(key:=name)
                    xcmd = aXchangeObject.XChangeCmd
                End If
                Call SetObjectXCmd(name:=name, xchangecommand:=xcmd)
                Return False
            End If

            ' load
            If anObjectDef Is Nothing Then
                CoreMessageHandler(message:="Object couldnot be retrieved", procedure:="XChangeConfiguration.AddObjectByname", messagetype:=otCoreMessageType.InternalError, _
                                    argument:=name, objectname:=Me.ObjectID)
                Return False
            End If

            ' add 
            aXchangeObject = XChangeObject.Create(Me.Configname, Me.GetMaxIDNO + 1, objectname:=name, xcmd:=xcmd, domainid:=domainid, runtimeonly:=Me.RunTimeOnly)
            If aXchangeObject IsNot Nothing Then
                _ObjectCollection.Add(aXchangeObject)
                Return True
            End If

            Return False

        End Function

        ''' <summary>
        ''' Adds an xchange entry by object- and entryname
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="ordinal"></param>
        ''' <param name="OBJECTNAME"></param>
        ''' <param name="ISXCHANGED"></param>
        ''' <param name="XCMD"></param>
        ''' <param name="READONLY"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddEntryByObjectEntry(ByRef entryname As String,
                                             ByVal objectname As String,
                                                Optional ByVal ordinal As Object = Nothing,
                                                Optional ByVal isXChanged As Boolean = True,
                                                Optional ByVal xcmd As otXChangeCommandType = 0,
                                                Optional ByVal [readonly] As Boolean = False, _
                                            Optional domainid As String = Nothing) As Boolean

            ' Nothing
            If Not IsAlive("AddEntryByObjectEntry") Then Return False
            Dim anObjectEntry As iormObjectEntryDefinition = CurrentSession.Objects.GetEntryDefinition(objectname:=objectname, entryname:=entryname)
            entryname = entryname.ToUpper
            objectname = objectname.ToUpper
            If xcmd = 0 Then xcmd = otXChangeCommandType.Read


            If Not anObjectEntry Is Nothing Then
                Return Me.AddEntryByObjectEntry(objectentry:=anObjectEntry, objectname:=objectname, domainid:=domainid, ordinal:=ordinal, isxchanged:=isXChanged, xcmd:=xcmd, [readonly]:=[readonly])
            Else
                Call CoreMessageHandler(message:="field entry not found", argument:=objectname & "." & entryname, messagetype:=otCoreMessageType.InternalError,
                                         procedure:="XChangeConfiguration.addAttributeByField")

                Return False
            End If

        End Function
        ''' <summary>
        ''' adds an xchange entry by the objectentry from the repository
        ''' </summary>
        ''' <param name="objectentry"></param>
        ''' <param name="ordinal"></param>
        ''' <param name="objectname"></param>
        ''' <param name="isxchanged"></param>
        ''' <param name="xcmd"></param>
        ''' <param name="readonly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddEntryByObjectEntry(ByRef objectentry As iormObjectEntryDefinition,
                                        Optional ByVal ordinal As Object = Nothing,
                                        Optional ByVal objectname As String = Nothing,
                                        Optional ByVal isxchanged As Boolean = True,
                                        Optional ByVal xcmd As otXChangeCommandType = 0,
                                        Optional ByVal [readonly] As Boolean = False, _
                                        Optional domainid As String = Nothing) As Boolean
            Dim anEntry As XChangeObjectEntry
            Dim aVAlue As Object
            Dim aXchangeObject As XChangeObject
            If Not String.IsNullOrWhiteSpace(objectname) Then objectname = objectname.ToUpper
            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID

            ' isalive
            If Not Me.IsAlive(subname:="AddEntryByObjectEntry") Then Return False

            ' if ordinal is missing -> create one
            If ordinal Is Nothing Then
                For Each [alias] In objectentry.Aliases
                    'could be more than one Attribute by Alias
                    anEntry = Me.GetEntryByXID(XID:=[alias])
                    If anEntry IsNot Nothing Then
                        If anEntry.IsLoaded Or anEntry.IsCreated Then
                            ordinal = anEntry.Ordinal
                            Exit For
                        End If
                    End If
                Next
            End If
            If ordinal Is Nothing Then
                aVAlue = Me.GetMaxordinalNo
                If aVAlue < constXCHCreateordinal - 1 Then
                    ordinal = New Ordinal(constXCHCreateordinal)
                Else
                    ordinal = New Ordinal(aVAlue + 1)
                End If

            End If

            '*** Add the Object if necessary
            If String.IsNullOrWhiteSpace(objectname) Then
                aXchangeObject = Me.GetObjectByName(objectentry.Objectname)
                If aXchangeObject Is Nothing Then
                    If Me.AddObjectByName(name:=objectentry.Objectname, xcmd:=xcmd) Then
                        aXchangeObject = Me.GetObjectByName(objectentry.Objectname)
                    End If
                End If
            Else
                aXchangeObject = Me.GetObjectByName(objectname)
                If aXchangeObject Is Nothing Then
                    If Me.AddObjectByName(name:=objectname, xcmd:=xcmd) Then
                        aXchangeObject = Me.GetObjectByName(objectname)
                    End If
                End If
            End If

            '** add a default command -> might be also 0 if object was added with entry
            If xcmd = 0 Then xcmd = aXchangeObject.XChangeCmd
            If xcmd = 0 Then xcmd = otXChangeCommandType.Read

            ' add the component
            anEntry = XChangeObjectEntry.Create(Me.Configname, Me.GetMaxIDNO + 1, domainid:=domainid)
            If anEntry IsNot Nothing Then
                anEntry.XID = objectentry.XID
                If Not TypeOf ordinal Is OnTrack.Database.Ordinal Then
                    ordinal = New Ordinal(ordinal)
                End If

                anEntry.Ordinal = ordinal ' create an ordinal 
                anEntry.ObjectEntryname = objectentry.Entryname
                anEntry.IsXChanged = isxchanged
                anEntry.IsReadOnly = [readonly]
                anEntry.Domainid = domainid
                anEntry.Objectname = aXchangeObject.Objectname
                anEntry.XChangeCmd = xcmd
                ' add the Object too
                _ObjectEntryCollection.Add(anEntry)
                Return True
            Else
                CoreMessageHandler(message:="Warning! Entry couldnot be created in XChangeConfiguration", procedure:="XChangeConfiguration.AddEntryByObjectEntry", argument:=Me.Configname, _
                                   messagetype:=otCoreMessageType.ApplicationWarning)
            End If

            Return False


        End Function
        ''' <summary>
        ''' Adds an Entry  by its XChange-ID
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="ordinal"></param>
        ''' <param name="objectname"></param>
        ''' <param name="isXChanged"></param>
        ''' <param name="xcmd"></param>
        ''' <param name="readonly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddEntryByXID(ByVal Xid As String,
                                            Optional ByVal ordinal As Object = Nothing,
                                            Optional ByVal objectname As String = Nothing,
                                            Optional ByVal isXChanged As Boolean = True,
                                            Optional ByVal xcmd As otXChangeCommandType = Nothing,
                                            Optional ByVal [readonly] As Boolean = False, _
                                            Optional domainid As String = Nothing) As Boolean


            AddEntryByXID = False
            If objectname IsNot Nothing Then objectname = objectname.ToUpper
            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID
            Xid = Xid.ToUpper

            ' isalive
            If Not Me.IsAlive(subname:="AddEntryByXID") Then Return False

            '*** no objectname -> get all IDs in objects
            If String.IsNullOrWhiteSpace(objectname) Then
                ''' make sure that the objects needed are really loaded in anything else than the currentdomain
                Dim anEntrylist As List(Of iormObjectEntryDefinition) = CurrentSession.Objects(domainid:=domainid).GetEntriesByXID(xid:=Xid)
                For Each anEntry In anEntrylist.ToArray 'make sure that the list is not changing (clone it) - maybe we are adding entries
                    '** compare to objects in order
                    If Me.NoObjects > 0 Then
                        Dim aList As List(Of XChangeObject) = Me.ObjectsByOrderNo
                        For Each anObjectEntry As XChangeObject In aList.ToArray 'make sure that the list is not changing (clone it) - maybe we are adding entries
                            If anEntry.Objectname = anObjectEntry.Objectname Then
                                Return AddEntryByObjectEntry(objectentry:=anEntry, ordinal:=ordinal,
                                                                  isxchanged:=isXChanged,
                                                                  objectname:=anEntry.Objectname,
                                                                  domainid:=domainid, _
                                                                  xcmd:=xcmd, readonly:=[readonly])
                            End If
                        Next
                        ' simply add

                    Else
                        Return AddEntryByObjectEntry(objectentry:=anEntry, ordinal:=ordinal,
                                                          isxchanged:=isXChanged, domainid:=domainid, _
                                                          objectname:=anEntry.Objectname, xcmd:=xcmd, readonly:=[readonly])
                    End If

                Next

            Else
                Dim aList As List(Of iormObjectEntryDefinition) = CurrentSession.Objects.GetEntriesByXID(xid:=Xid)
                For Each entry In aList.ToArray 'make sure that the list is not changing (clone it) - maybe we are adding entries
                    If objectname = entry.Objectname Then
                        Return AddEntryByObjectEntry(objectentry:=entry, ordinal:=ordinal,
                                                          isxchanged:=isXChanged,
                                                          objectname:=entry.Objectname,
                                                           domainid:=domainid, _
                                                          xcmd:=xcmd, readonly:=[readonly])
                    End If
                Next


            End If

            ' return
            Return False


        End Function
        ''' <summary>
        ''' returns True if an Objectname with an ID exists
        ''' </summary>
        ''' <param name="objectname"></param>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Exists(Optional ByVal objectname As String = Nothing, _
                               Optional ByVal XID As String = Nothing) As Boolean
            Dim flag As Boolean
            If Not String.IsNullOrWhiteSpace(objectname) Then objectname = objectname.ToUpper
            If Not String.IsNullOrWhiteSpace(XID) Then XID = XID.ToUpper

            ' Nothing
            If Not Me.IsLoaded And Not Me.IsCreated Then
                Exists = False
                Exit Function
            End If

            ' missing arguments
            If String.IsNullOrWhiteSpace(objectname) Then
                Call CoreMessageHandler(procedure:="XChangeConfiguration.exists", message:="objectname was not set", _
                                        messagetype:=otCoreMessageType.InternalError)
                Exists = False
                Exit Function
            End If
            ' missing arguments
            If String.IsNullOrWhiteSpace(objectname) AndAlso String.IsNullOrWhiteSpace(XID) Then
                Call CoreMessageHandler(procedure:="XChangeConfiguration.exists", message:="set either objectname or attributename - not both", _
                                        messagetype:=otCoreMessageType.InternalError)
                Exists = False
                Exit Function
            End If

            '+ check
            If Not String.IsNullOrWhiteSpace(XID) AndAlso String.IsNullOrWhiteSpace(XID) Then
                If _ObjectCollection.ContainsKey(key:=objectname) Then
                    Exists = True
                Else
                    Exists = False
                End If
                Exit Function
            Else
                If _entriesXIDDirectory.ContainsKey(key:=XID) Then

                    Exists = True
                Else
                    Exists = False
                End If
                Exit Function
            End If
        End Function

        ''' <summary>
        ''' Add an Entry by ID
        ''' </summary>
        ''' <param name="entry"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AddXIDReference(ByRef entry As XChangeObjectEntry) As Boolean
            Dim entries As List(Of XChangeObjectEntry)

            If _entriesXIDList.ContainsKey(key:=UCase(entry.XID)) Then
                entries = _entriesXIDList.Item(UCase(entry.XID))
            Else

                entries = New List(Of XChangeObjectEntry)
                _entriesXIDList.Add(UCase(entry.XID), entries)
            End If
            If entries.Contains(entry) Then entries.Remove(entry)
            entries.Add(entry)

            Return True
        End Function
        ''' <summary>
        ''' Add ordinal to Reference Structures
        ''' </summary>
        ''' <param name="member"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AddOrdinalReference(ByRef entry As IXChangeConfigEntry) As Boolean
            Dim entries As List(Of IXChangeConfigEntry)
            '** sorted
            If _entriesByordinal.ContainsKey(key:=entry.Ordinal) Then
                entries = _entriesByordinal.Item(entry.Ordinal)
            Else
                entries = New List(Of IXChangeConfigEntry)
                _entriesByordinal.Add(entry.Ordinal, entries)
            End If

            If entries.Contains(entry) Then entries.Remove(entry)
            entries.Add(entry)

            Return True
        End Function
        ''' <summary>
        ''' Add an Entry
        ''' </summary>
        ''' <param name="entry"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AddObjectReference(ByRef entry As XChangeObjectEntry) As Boolean
            Dim entries As List(Of XChangeObjectEntry)

            If _entriesByObjectnameDirectory.ContainsKey(key:=entry.Objectname) Then
                entries = _entriesByObjectnameDirectory.Item(entry.Objectname)
            Else
                entries = New List(Of XChangeObjectEntry)
                _entriesByObjectnameDirectory.Add(entry.Objectname, entries)
            End If

            entries.Add(entry)

            Return True
        End Function
        ''' <summary>
        ''' Add an Entry by ID
        ''' </summary>
        ''' <param name="entry"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AddAliasReference(ByRef Entry As XChangeObjectEntry) As Boolean
            Dim entries As List(Of XChangeObjectEntry)

            For Each [alias] As String In Entry.Aliases

                If _aliasDirectory.ContainsKey(key:=UCase([alias])) Then
                    entries = _aliasDirectory.Item(key:=UCase([alias]))
                Else
                    entries = New List(Of XChangeObjectEntry)
                    _aliasDirectory.Add(key:=UCase([alias]), value:=entries)
                End If
                If entries.Contains(Entry) Then entries.Remove(Entry)
                entries.Add(Entry)
            Next

            Return True
        End Function
        ''' <summary>
        ''' Event Handler for on Removed
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub XChangeConfiguration_OnRemovedEntry(sender As Object, e As ormRelationCollection(Of XChangeObjectEntry).EventArgs) Handles _ObjectEntryCollection.OnRemoved
            Dim anEntry = e.Dataobject
            Dim anObjectEntry As XChangeObject

            Throw New NotImplementedException

        End Sub

        ''' <summary>
        ''' Event handler for the Added Entry in the Entries Collection
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub XChangeConfiguration_OnAddEntry(sender As Object, e As ormRelationCollection(Of XChangeObjectEntry).EventArgs) Handles _ObjectEntryCollection.OnAdded
            Dim anEntry As XChangeObjectEntry = e.Dataobject

            ' check on the Object of the Attribute
            If Not _ObjectDictionary.ContainsKey(key:=anEntry.Objectname.ToUpper) Then
                Me.AddObjectByName(anEntry.Objectname.ToUpper)
            End If

            ' add the Attribute
            If _entriesXIDDirectory.ContainsKey(key:=anEntry.XID) Then
                Call _entriesXIDDirectory.Remove(key:=anEntry.XID)
            End If

            Call _entriesXIDDirectory.Add(key:=anEntry.XID, value:=anEntry)
            '** references
            AddXIDReference(anEntry) '-> List references if multipe
            AddObjectReference(anEntry)
            AddAliasReference(anEntry)
            AddOrdinalReference(anEntry)



        End Sub
        ''' <summary>
        ''' Event handler for the Added Entry in the Entries Collection
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub XChangeConfiguration_OnAddEntry(sender As Object, e As ormRelationCollection(Of XChangeObject).EventArgs) Handles _ObjectCollection.OnAdded
            Dim anXchangeObject As XChangeObject = e.Dataobject
            If anXchangeObject Is Nothing Then
                CoreMessageHandler(message:="anEntry is not an ObjectEntry", messagetype:=otCoreMessageType.InternalError,
                                    procedure:="XConfig.Addmember")
                Return
            End If

            If _ObjectDictionary.ContainsKey(key:=anXchangeObject.Objectname) Then
                Call _ObjectDictionary.Remove(key:=anXchangeObject.Objectname)
            End If
            Call _ObjectDictionary.Add(key:=anXchangeObject.Objectname, value:=anXchangeObject)
            '**
            If _objectsByOrderDirectory.ContainsKey(key:=anXchangeObject.Orderno) Then
                Call _objectsByOrderDirectory.Remove(key:=anXchangeObject.Orderno)
            End If
            Call _objectsByOrderDirectory.Add(key:=anXchangeObject.Orderno, value:=anXchangeObject)

        End Sub

        ''' <summary>
        ''' Event Handler for on Removed
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub XChangeConfiguration_OnRemovedEntry(sender As Object, e As ormRelationCollection(Of XChangeObject).EventArgs) Handles _ObjectCollection.OnRemoved
            Dim anEntry = e.Dataobject


            Throw New NotImplementedException

        End Sub
        ''' <summary>
        ''' Add XChangeMember
        ''' </summary>
        ''' <param name="anEntry"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddEntry(anEntry As XChangeObjectEntry) As Boolean
            ' remove and overwrite
            If _ObjectEntryCollection.Contains(anEntry) Then
                Call _ObjectEntryCollection.Remove(anEntry)
            End If

            ' add Member Entry
            _ObjectEntryCollection.Add(anEntry)
            Return True
        End Function

        ''' <summary>
        ''' reset all entry definitions
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ClearEntries() As Boolean
            Return Reset(justentries:=True)
        End Function
        ''' <summary>
        ''' resets the object
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Reset(Optional justentries As Boolean = False) As Boolean
            ''' if not just the entries
            If Not justentries Then
                _ObjectCollection.Clear()
                _ObjectDictionary.Clear()
                _objectsByOrderDirectory.Clear()
            End If

            '*** reset the entries
            _entriesXIDDirectory.Clear()
            _entriesByObjectnameDirectory.Clear()
            _entriesXIDList.Clear()
            _aliasDirectory.Clear()
            _entriesByordinal.Clear()
        End Function


        ''' <summary>
        ''' retrieves an Object by its name or nothing
        ''' </summary>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectByName(ByVal objectname As String) As XChangeObject

            If _ObjectDictionary.ContainsKey(objectname.ToUpper) Then
                Return _ObjectDictionary.Item(key:=objectname.ToUpper)
            Else
                Return Nothing
            End If

        End Function
        ''' <summary>
        ''' returns the xchange object entry id's
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectEntryIDNos() As IEnumerable(Of Long)
            Get
                Dim alist As New List(Of Long)
                For Each akey In _ObjectEntryCollection.Keys
                    alist.Add(akey(0))
                Next
                Return alist
            End Get
        End Property
        ''' <summary>
        ''' returns the xchange object id's
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectIDNos() As IEnumerable(Of Long)
            Get

                Dim alist As New List(Of Long)
                For Each akey In _ObjectCollection.Keys
                    alist.Add(akey(0))
                Next
                Return alist
            End Get
        End Property


        ''' <summary>
        ''' returns a list of xchangeobjects in ordinal order
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectsByOrderNo() As IList(Of XChangeObject)
            Get
                Return _objectsByOrderDirectory.Values.ToList
            End Get
        End Property

        ''' <summary>
        ''' returns a list of xchange object names in ordinal order
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectnamesByOrderNo() As IList(Of String)
            Get
                Return _objectsByOrderDirectory.Select(Function(x) x.Value.Objectname).ToList
            End Get
        End Property

        ''' <summary>
        ''' retrieves a List of Attributes per Objectname
        ''' </summary>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEntriesByObjectName(ByVal objectname As String) As IList(Of XChangeObjectEntry)

            If _entriesByObjectnameDirectory.ContainsKey(objectname) Then
                Return _entriesByObjectnameDirectory.Item(key:=objectname)
            Else
                Return New List(Of XChangeObjectEntry)
            End If


        End Function

        ''' <summary>
        ''' gets an list of ordered (by ordinal) XChange ObjectEntries
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property OrderedXChangeObjectEntries() As IList(Of XChangeObjectEntry)
            Get
                Return _entriesByordinal.ToList
            End Get
        End Property
        ''' <summary>
        ''' gets an relational collection of xchange obejct entries
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property XChangeObjectEntries() As iormRelationalCollection(Of XChangeObjectEntry)
            Get
                Return _ObjectEntryCollection
            End Get
        End Property

        ''' <summary>
        ''' gets an relational collection of the xchange objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property XChangeobjects() As iormRelationalCollection(Of XChangeObject)
            Get
                Return _ObjectCollection
            End Get
        End Property

        ''' <summary>
        ''' returns an attribute by its entryname and objectname
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEntryByObjectEntryName(ByVal entryname As String,
                                                    Optional ByVal objectname As String = Nothing) As XChangeObjectEntry

            Dim anEntry As XChangeObjectEntry
            If Not IsAlive(subname:="GetEntryByObjectEntryName") Then Return Nothing
            If Not String.IsNullOrWhiteSpace(objectname) Then objectname = objectname.ToUpper
            entryname = entryname.ToUpper

            Dim alist As List(Of XChangeObjectEntry)
            If Not String.IsNullOrWhiteSpace(objectname) Then
                '* might be we have the object but no fields
                If _entriesByObjectnameDirectory.ContainsKey(key:=objectname) Then
                    alist = _entriesByObjectnameDirectory.Item(key:=objectname)
                    anEntry = alist.Find(Function(m As XChangeObjectEntry)
                                             Return m.ObjectEntryname = entryname
                                         End Function)

                    If Not anEntry Is Nothing Then
                        Return anEntry
                    End If
                End If

            Else
                For Each objectdef In _objectsByOrderDirectory.Values
                    If _entriesByObjectnameDirectory.ContainsKey(key:=objectdef.Objectname) Then
                        alist = _entriesByObjectnameDirectory(key:=objectdef.Objectname)

                        anEntry = alist.Find(Function(m As XChangeObjectEntry)
                                                 Return m.ObjectEntryname = entryname
                                             End Function)

                        If Not anEntry Is Nothing Then
                            Return anEntry
                        End If
                    End If
                Next
            End If

            '** search also by ID and consequent by ALIAS
            If Not String.IsNullOrWhiteSpace(objectname) Then
                Dim anObjectEntry As iormObjectEntryDefinition = CurrentSession.Objects.GetEntryDefinition(objectname:=objectname, entryname:=entryname)
                If Not anObjectEntry Is Nothing AndAlso anObjectEntry.XID IsNot Nothing Then
                    anEntry = Me.GetEntryByXID(XID:=anObjectEntry.XID, objectname:=objectname)
                    If Not anEntry Is Nothing Then
                        Return anEntry
                    End If
                End If
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' returns an XChange ConfigEnry by idno or nothing if not exists
        ''' </summary>
        ''' <param name="idno"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEntry(ByVal idno As Long) As IXChangeConfigEntry
            If Not Me.IsAlive("GetEntry") Then Return Nothing
            If _ObjectEntryCollection.ContainsKey(key:={idno}) Then
                Return _ObjectEntryCollection.Item(key:={idno})
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' returns an Attribute in the XChange Config by its XChange ID or Alias
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEntryByXID(ByVal XID As String, _
                                        Optional ByVal objectname As String = Nothing) As XChangeObjectEntry

            Dim aCollection As IEnumerable
            Dim names As String() = Shuffle.NameSplitter(XID.ToUpper)
            If names.Count = 0 OrElse Not String.IsNullOrWhiteSpace(objectname) Then
                XID = names.First
                objectname = objectname.ToUpper
            ElseIf names.Count > 1 Then
                XID = names.Last
                objectname = names.First
            Else
                ' case we have a canonical xid
                XID = names.First
                If Not String.IsNullOrWhiteSpace(objectname) Then objectname = objectname.ToUpper
            End If


            If Not Me.IsAlive(subname:="GetEntryByXID") Then
                Return Nothing
            End If


            If _entriesXIDList.ContainsKey(XID) Then
                aCollection = _entriesXIDList.Item(XID)
                For Each entry As XChangeObjectEntry In aCollection
                    If Not String.IsNullOrWhiteSpace(objectname) AndAlso entry.Objectname = objectname Then
                        Return entry
                    ElseIf String.IsNullOrWhiteSpace(objectname) Then
                        Return entry
                    End If
                Next
                '** special case it was one xid and no objectname
            ElseIf _entriesXIDList.ContainsKey(objectname & "." & XID) Then
                Return _entriesXIDList.Item(objectname & "." & XID).First
            End If

            '** look into aliases 
            '**
            '* check if ID is an ID already in the xconfig
            GetEntryByXID = GetEntrybyAlias(XID, objectname)
            If GetEntryByXID Is Nothing Then
                '* check all Objects coming through with this ID
                For Each anObjectEntry In CurrentSession.Objects.GetEntriesByXID(xid:=XID)
                    '** check on all the XConfig Objects
                    For Each anObjectMember In Me.ObjectsByOrderNo
                        '* if ID is included as Alias Name
                        GetEntryByXID = GetEntrybyAlias(alias:=anObjectEntry.XID, objectname:=anObjectMember.Objectname)
                        '** or the aliases are included in this XConfig
                        If GetEntryByXID Is Nothing Then
                            For Each aliasID In anObjectEntry.Aliases
                                GetEntryByXID = GetEntrybyAlias(alias:=aliasID, objectname:=anObjectMember.Objectname)
                                '* found
                                If Not GetEntryByXID Is Nothing Then
                                    Exit For
                                End If
                            Next

                        End If
                        '* found
                        If Not GetEntryByXID Is Nothing Then
                            Exit For
                        End If
                    Next
                    '* found
                    If Not GetEntryByXID Is Nothing Then
                        Exit For
                    End If
                Next

            End If
            Return GetEntryByXID
        End Function
        ''' <summary>
        ''' returns a List of XConfigMembers per ordinal
        ''' </summary>
        ''' <param name="ordinal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEntriesByMappingOrdinal(ByVal ordinal As Ordinal) As List(Of IXChangeConfigEntry)

            If Not Me.IsCreated And Not Me.IsLoaded Then
                Return New List(Of IXChangeConfigEntry)
            End If

            If _entriesByordinal.ContainsKey(ordinal) Then
                Return _entriesByordinal.Item(ordinal)
            Else
                Return New List(Of IXChangeConfigEntry)
            End If

        End Function
        ''' <summary>
        ''' returns an Attribute by its XChange Alias ID
        ''' </summary>
        ''' <param name="alias"></param>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEntrybyAlias(ByVal [alias] As String,
                                        Optional ByVal objectname As String = Nothing) As XChangeObjectEntry

            Dim aCollection As IEnumerable
            If Not String.IsNullOrWhiteSpace(objectname) Then objectname = objectname.ToUpper

            If Not Me.IsCreated And Not Me.IsLoaded Then
                GetEntrybyAlias = Nothing
                Exit Function
            End If

            If _aliasDirectory.ContainsKey(UCase([alias])) Then

                aCollection = _aliasDirectory.Item(UCase([alias]))
                For Each entry As XChangeObjectEntry In aCollection
                    If Not String.IsNullOrWhiteSpace(objectname) AndAlso entry.Objectname = objectname Then
                        Return entry
                    ElseIf String.IsNullOrWhiteSpace(objectname) Then
                        Return entry
                    End If
                Next

            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' Returns an ienumerable of all entries (optional just by an objectname)
        ''' </summary>
        ''' <param name="objectname">optional objectname</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectEntries(Optional objectname As String = Nothing) As IEnumerable(Of XChangeObjectEntry)
            If Not IsAlive(subname:="GetObjectEntries") Then Return New List(Of XChangeObjectEntry)

            If Not String.IsNullOrWhiteSpace(objectname) Then
                Return GetEntriesByObjectName(objectname)
            Else
                Return _entriesXIDDirectory.Values.ToList
            End If

        End Function
        ''' <summary>
        ''' Loads a XChange Configuration from Store
        ''' </summary>
        ''' <param name="configname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal configname As String, _
                                                  Optional domainid As String = Nothing, _
                                                  Optional runtimeonly As Boolean = False) As XChangeConfiguration

            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey() As Object = {configname.ToUpper, domainid}
            Return ormBusinessObject.RetrieveDataObject(Of XChangeConfiguration)(primarykey, runtimeOnly:=runtimeonly)
        End Function


        ''' <summary>
        ''' creates a persistable object with primary key
        ''' </summary>
        ''' <param name="configname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal configname As String, Optional domainid As String = Nothing, Optional runtimeonly As Boolean = False) As XChangeConfiguration
            Dim primarykey() As Object = {configname.ToUpper, domainid}
            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID
            Return ormBusinessObject.CreateDataObject(Of XChangeConfiguration)(primarykey, checkUnique:=True)
        End Function


        ''' <summary>
        ''' retrieves a List of all XConfigs
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function All() As List(Of XChangeConfiguration)
            Return ormBusinessObject.AllDataObject(Of XChangeConfiguration)()
        End Function
    End Class

    ''' <summary>
    ''' describes a XChange Outline data structure
    ''' </summary>
    ''' <remarks></remarks>
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleXChange, Version:=1, Release:=1, patch:=3, changeimplno:=1, _
         description:="Bug Fix in outline generation. Outline items will be created or retrieved on rundynamic.")> _
    <ormObject(ID:=XOutline.ConstObjectID, version:=1, usecache:=True, adddeletefieldbehavior:=True, adddomainbehavior:=True, _
        modulename:=ConstModuleXChange, description:="describes a XChange Outline data structure")> _
    Public Class XOutline
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable
        Implements IEnumerable(Of XOutlineItem)

        Public Const ConstObjectID = "XOUTLINE"

        <ormTableAttribute(Version:=1)> Public Const ConstPrimaryTableID = "tblXOutlines"

        ''' <summary>
        ''' Keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(XID:="otl1", PrimaryKeyOrdinal:=1, Datatype:=otDataType.Text, size:=50,
                    properties:={ObjectEntryProperty.Keyword}, _
                description:="identifier of the outline", Title:="ID")> Public Const ConstFNID = "ID"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2, _
                        useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const ConstFNDomainID = Domain.ConstFNDomainID
        ''' <summary>
        '''  Fields
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(XID:="OTL2", Datatype:=otDataType.Text, isnullable:=True, _
                description:="description of the outline", Title:="description")> Public Const constFNdesc = "DESC"


        <ormObjectEntry(Datatype:=otDataType.List, isnullable:=True, _
                        XID:="OTL5", title:="Business Objects", description:="applicable business objects for this outline")> Public Const ConstFNObjects = "OBJECTS"

        <ormObjectEntry(XID:="OTL10", Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0",
                       description:="True if deliverable revisions are added dynamically", Title:="DynamicRevision")> Public Const constFNDynamicAddRevisions = "ADDREV"
        <ormObjectEntry(XID:="OTL11", Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0",
                        description:="True if items are generated automatically by order", Title:="Dynamic")> Public Const constFNDynamic = "DYNAMIC"

        <ormObjectEntry(XID:="OTL12", Datatype:=otDataType.Text, isnullable:=True, _
               description:="order by clause of the dynamic item", Title:="Orderby")> Public Const constFNOrderBy = "OrderBy"

        ''' <summary>
        ''' Column Mappings
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNID)> Private _id As String = String.empty
        <ormObjectEntryMapping(EntryName:=constFNdesc)> Private _desc As String
        <ormObjectEntryMapping(EntryName:=ConstFNObjects)> Private _Objects As String()
        <ormObjectEntryMapping(EntryName:=constFNDynamicAddRevisions)> Private _DynamicAddRevisions As Boolean
        <ormObjectEntryMapping(EntryName:=constFNDynamic)> Private _DynamiBehaviour As Boolean
        <ormObjectEntryMapping(EntryName:=constFNOrderBy)> Private _OderByClause As String

        ''' <summary>
        ''' Relations
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(XOutlineItem), cascadeOnDelete:=True, cascadeOnUpdate:=True, _
            fromEntries:={ConstFNID}, toEntries:={XOutlineItem.constFNID})> Public Const ConstRItems = "RELITEMS"

        <ormObjectEntryMapping(RelationName:=ConstRItems, infuseMode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand, _
            keyentries:={XOutlineItem.ConstFNordinals})> Private WithEvents _itemCollection As New ormRelationCollection(Of XOutlineItem)(Me, {XOutlineItem.ConstFNordinals})


        ''' <summary>
        ''' runtime Elements
        ''' </summary>
        ''' <remarks></remarks>

        Private _dynamicCollection As New List(Of XOutlineItem)

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the oder by clause.
        ''' </summary>
        ''' <value>The oder by clause.</value>
        Public Property OrderByClause As String
            Get
                Return Me._OderByClause
            End Get
            Set(value As String)
                SetValue(constFNOrderBy, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the dynami behaviour.
        ''' </summary>
        ''' <value>The dynamic behaviour.</value>
        Public Property DynamicBehaviour As Boolean
            Get
                Return Me._DynamiBehaviour
            End Get
            Set(value As Boolean)
                SetValue(constFNDynamic, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the objects.
        ''' </summary>
        ''' <value>The objects.</value>
        Public Property Objects As String()
            Get
                Return Me._Objects
            End Get
            Set(value As String())
                SetValue(ConstFNObjects, value)
            End Set
        End Property

        ''' <summary>
        ''' Returns the Collection of OutlineItems in this
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Items As iormRelationalCollection(Of XOutlineItem)
            Get
                Return _itemCollection
            End Get
        End Property



        ''' <summary>
        ''' Gets or sets the desc.
        ''' </summary>
        ''' <value>The desc.</value>
        Public Property Description() As String
            Get
                Return Me._desc
            End Get
            Set(value As String)
                SetValue(constFNdesc, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the dynamic add revisions.
        ''' </summary>
        ''' <value>The dynamic add revisions.</value>
        Public Property DynamicAddRevisions() As Boolean
            Get
                Return Me._DynamicAddRevisions
            End Get
            Set(value As Boolean)
                SetValue(constFNDynamicAddRevisions, value)
            End Set
        End Property

        ''' <summary>
        ''' gets the ID of the Outline
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ID()
            Get
                Return _id
            End Get

        End Property
        ''' <summary>
        ''' gets the number outline items in the outline
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Count() As Long
            Get
                Return _itemCollection.Count
            End Get

        End Property
#End Region



        ''' <summary>
        ''' Add an Item
        ''' </summary>
        ''' <param name="anEntry"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddItem(item As XOutlineItem) As Boolean
            If Not Me.IsAlive("AddItem") Then Return False


            ' remove and overwrite
            If _itemCollection.ContainsKey(key:=item.ordinal) Then
                Call _itemCollection.Remove(item)
            End If
            ' add entry
            _itemCollection.Add(item)

            '
            Return True

        End Function


        ''' <summary>
        ''' ordinals of the components
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Ordinals() As IList(Of DataValueTuple)
            Return _itemCollection.Keys
        End Function


        ''' <summary>
        ''' create an persistable outline
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Retrieve(ByVal id As String, Optional domainID As String = Nothing) As XOutline
            If String.IsnullorEmpty(domainID) Then domainID = CurrentSession.CurrentDomainID
            Return ormBusinessObject.RetrieveDataObject(Of XOutline)(pkArray:={id.ToUpper, domainID})
        End Function

        ''' <summary>
        ''' create an persistable outline
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal id As String, Optional domainID As String = Nothing) As XOutline
            If String.IsnullorEmpty(domainID) Then domainID = CurrentSession.CurrentDomainID
            Return ormBusinessObject.CreateDataObject(Of XOutline)(pkArray:={id.ToUpper, domainID}, domainID:=domainID, checkUnique:=True)
        End Function

        '*****
        '***** CleanUpRevisions (if dynamic revision than throw out all the revisions)
        Public Function CleanUpRevision() As Boolean

            Dim aDeliverable As New Deliverable
            Dim aFirstRevision As New Deliverable
            Dim deletedColl As New Collection


            If Not CurrentSession.RequireAccessRight(accessRequest:=otAccessRight.ReadUpdateData) Then
                Call CoreMessageHandler(procedure:="clsOTDBXOutline.cleanupRevision", message:="Read Update not granted",
                                       messagetype:=otCoreMessageType.ApplicationError)
                Return False
            End If
            If Not Me.IsLoaded And Not Me.IsCreated Then
                Return False
            ElseIf Not Me.DynamicAddRevisions Then
                Return False
            End If

            '*** go through all items in Outline and delete the NON-Firstrevisions 
            '*** without checking if the first revisions are in the outline

            For Each item As XOutlineItem In _itemCollection
                Dim keys As List(Of XOutlineItem.OutlineKey) = item.keys

                '** look for Deliverable UID
                For Each key In keys
                    If key.ID.ToLower = "uid" Or key.ID.ToLower = "sc2" Then
                        aFirstRevision = Deliverable.Retrieve(uid:=CLng(key.Value))
                        If aFirstRevision IsNot Nothing Then
                            If Not aFirstRevision.IsFirstRevision Or aFirstRevision.IsDeleted Then
                                deletedColl.Add(Item:=item)
                                Call item.Delete()
                            End If
                        End If
                    End If
                Next

            Next

            For Each item As XOutlineItem In deletedColl
                _itemCollection.Remove(item)
            Next

            Call CoreMessageHandler(message:="outline cleaned from revisions", procedure:="clsOTDBXoutline.cleanuprevision",
                                         argument:=Me.ID, messagetype:=otCoreMessageType.ApplicationInfo)
            Return True

        End Function

        ''' <summary>
        ''' add the items dynamically
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Private Function RunDynamic(Optional domainid As String = Nothing) As Boolean
            If Not IsAlive("RunDynamic") Then Return False
            If String.IsnullorEmpty(domainID) Then domainid = Me.DomainID

            If Not Me.DynamicBehaviour Then Return False
            If Me.Objects Is Nothing OrElse Me.Objects.Count = 0 Then
                CoreMessageHandler(message:="For dynamic behavior the objects must be set", messagetype:=otCoreMessageType.ApplicationError, _
                                   procedure:="XOutline.RunDynamic", argument:=Me.ID)
                Return False
            End If
            Dim anObjectname As String = Me.Objects.First
            If anObjectname.ToUpper <> Deliverable.ConstObjectID.ToUpper Then
                CoreMessageHandler(message:="dynamic behavior only supported for deliverables", messagetype:=otCoreMessageType.ApplicationError, _
                                   procedure:="XOutline.RunDynamic", argument:=Me.ID, objectname:=anObjectname)
                Return False
            End If
            If String.IsNullOrWhiteSpace(Me.OrderByClause) Then
                CoreMessageHandler(message:="For dynamic behavior the order by clause must be set", messagetype:=otCoreMessageType.ApplicationError, _
                                  procedure:="XOutline.RunDynamic", argument:=Me.ID)
                Return False
            End If

            Try
                Dim anobjectdefinition As ormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=anObjectname)
                If anobjectdefinition Is Nothing Then
                    CoreMessageHandler(message:="For dynamic behavior the objects must be set with correct name", messagetype:=otCoreMessageType.ApplicationError, _
                                 procedure:="XOutline.RunDynamic", argument:=Me.ID, objectname:=anObjectname)
                    Return False
                End If
                Dim aStore As iormRelationalTableStore = GetPrimaryTableStore(anobjectdefinition.Tablenames.First)
                Dim cached = aStore.GetProperty(ormTableStore.ConstTPNCacheProperty)
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand("_Outline_" & Me.ID & "_RunDynamic")

                '** prepare the command if necessary
                If Not aCommand.IsPrepared Then

                    aCommand.AddTable(anobjectdefinition.Tablenames.First, addAllFields:=False)

                    '** select
                    aCommand.select = Deliverable.ConstFNDLVUID

                    '** where condition
                    aCommand.Where = "[" & Deliverable.ConstFNIsDeleted & "] = @isdeleted"
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@isdeleted", columnname:=ConstFNIsDeleted))
                    aCommand.Where &= String.Format(" AND ([{0}]=@domain or [{0}]=@globaldomainid)", {Deliverable.ConstFNDomain})
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@domain", notColumn:=True, datatype:=otDataType.Text))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@globaldomainid", notColumn:=True, datatype:=otDataType.Text))

                    aCommand.OrderBy = Me.OrderByClause
                    aCommand.Prepare()
                End If

                ' set Parameter
                aCommand.SetParameterValue("@isdeleted", False)
                aCommand.SetParameterValue("@domain", domainid)
                aCommand.SetParameterValue("@globaldomainid", ConstGlobalDomain)

                '** run the Command
                Dim theRecords As List(Of ormRecord) = aCommand.RunSelect
                Dim myordinal As Long = 10
                _dynamicCollection.Clear()
                Dim anUIDEntry As iormObjectEntryDefinition = CurrentSession.Objects.GetObjectDefinition(id:=Deliverable.ConstObjectID).GetEntryDefinition(entryname:=Deliverable.ConstFNDLVUID)
                If theRecords.Count >= 0 Then
                    For Each aRecord As ormRecord In theRecords
                        Dim aLngValue As Long = CLng(aRecord.GetValue(1))
                        Dim anItem As XOutlineItem = XOutlineItem.Retrieve(Me.ID, ordinal:=myordinal) 'maybe the item is in cache
                        If anItem Is Nothing Then anItem = XOutlineItem.Create(Me.ID, ordinal:=myordinal, uid:=aLngValue)
                        If anItem IsNot Nothing Then
                            anItem.keys.Add(New XOutlineItem.OutlineKey(otDataType.Long, ID:=anUIDEntry.XID, value:=aLngValue))
                            _dynamicCollection.Add(anItem)
                            myordinal += 10
                        End If
                    Next aRecord
                End If
                Return True
            Catch ex As Exception
                Call CoreMessageHandler(procedure:="Xoutline.RunDynamic", exception:=ex, messagetype:=otCoreMessageType.InternalError)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' processes the dynamic collection with revisions
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub RunDynamicRevision()
            Dim aDeliverable As New Deliverable
            Dim aFirstRevision As New Deliverable
            Dim returnCollection As New List(Of XOutlineItem)

            If Not IsAlive("RunDynamicRevision") Then Return
            If Not Me.DynamicAddRevisions Then Return

            '*** go through all items in Outline and delete the NON-Firstrevisions 
            '*** without checking if the first revisions are in the outline

            For Each item As XOutlineItem In _itemCollection
                Dim keys As List(Of XOutlineItem.OutlineKey) = item.keys

                '** look for Deliverable UID
                If item.IsText Or item.IsGroup Then
                    returnCollection.Add(item)
                Else
                    For Each key In keys
                        If key.ID.ToLower = "uid" Or key.ID.ToLower = "sc2" Then
                            aFirstRevision = Deliverable.Retrieve(uid:=CLng(key.Value))
                            If Me.DynamicAddRevisions AndAlso aFirstRevision IsNot Nothing Then
                                If aFirstRevision.IsFirstRevision And Not aFirstRevision.IsDeleted Then
                                    ' add all revisions inclusive the follow ups
                                    For Each uid As Long In Deliverable.AllRevisionUIDsBy(aFirstRevision.Uid)
                                        Dim newKey As New XOutlineItem.OutlineKey(otDataType.[Long], "uid", uid)
                                        Dim newKeylist As New List(Of XOutlineItem.OutlineKey)
                                        newKeylist.Add(newKey)
                                        Dim newOI As New XOutlineItem
                                        newOI.Create(ID:=item.OutlineID, level:=item.Level, ordinal:=item.ordinal)
                                        newOI.keys = newKeylist
                                        newOI.Level = item.Level
                                        newOI.Text = item.Text

                                        returnCollection.Add(newOI)
                                    Next
                                End If
                            Else
                                returnCollection.Add(item)
                            End If
                        Else
                            returnCollection.Add(item)
                        End If
                    Next
                End If


            Next
        End Sub
        Public Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Return Me.GetEnumerator()
        End Function
        ''' <summary>
        ''' returns an enumerator
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEnumerator() As IEnumerator(Of XOutlineItem) Implements IEnumerable(Of XOutlineItem).GetEnumerator

            If Me.DynamicBehaviour Then
                Me.RunDynamic()
                Return _dynamicCollection.GetEnumerator
            ElseIf Me.DynamicAddRevisions Then
                Me.RunDynamicRevision()
                Return _dynamicCollection.GetEnumerator
            End If

            Return _itemCollection.GetEnumerator
        End Function

    End Class

    ''' <summary>
    ''' OutlineItem of an Outline
    ''' </summary>
    ''' <remarks></remarks>

    <ormObject(ID:=XOutlineItem.constObjectID, version:=1, usecache:=True, adddeletefieldbehavior:=True, adddomainbehavior:=True, _
        modulename:=ConstModuleXChange, description:="describes a XChange Outline Item")> _
    Public Class XOutlineItem
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable




        ''' <summary>
        ''' OutlineKey Class as subclass of outline item to make it flexible
        ''' </summary>
        ''' <remarks></remarks>
        Public Class OutlineKey
            Private _Value As Object
            Private _ID As String
            Private [_Type] As otDataType

            Public Sub New(ByVal [Type] As otDataType, ByVal ID As String, ByVal value As Object)
                _Value = value
                _ID = ID
                _Type = [Type]
            End Sub
            ''' <summary>
            ''' Gets the type.
            ''' </summary>
            ''' <value>The type.</value>
            Public ReadOnly Property Type() As otDataType
                Get
                    Return Me.[_Type]
                End Get
            End Property

            ''' <summary>
            ''' Gets the ID.
            ''' </summary>
            ''' <value>The ID.</value>
            Public ReadOnly Property ID() As String
                Get
                    Return Me._ID
                End Get
            End Property

            ''' <summary>
            ''' Gets the value.
            ''' </summary>
            ''' <value>The value.</value>
            Public ReadOnly Property Value() As Object
                Get
                    Return Me._Value
                End Get
            End Property

        End Class

        Public Const constObjectID = "XOutlineItem"

        ''' <summary>
        ''' Table
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=1)> Public Const ConstPrimaryTableID = "tblXOutlineItems"

        ''' <summary>
        ''' indices
        ''' </summary>
        ''' <remarks></remarks>
        <ormIndex(columnname1:=constFNID, columnname2:=ConstFNordinall)> Public Const constIndexLongOutline = "longOutline"
        <ormIndex(columnname1:=ConstFNUid, columnname2:="id", columnname3:=ConstFNordinals)> Public Const constIndexUsedOutline = "UsedOutline"

        ''' <summary>
        ''' keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(XID:="otl1", PrimaryKeyOrdinal:=1, referenceObjectEntry:=XOutline.constobjectid & "." & XOutline.constFNID, _
            title:="Outline ID", description:="identifier of the outline")> Public Const constFNID = XOutline.constFNID

        <ormObjectEntry(XID:="otli3", PrimaryKeyOrdinal:=2, Datatype:=otDataType.Text, size:=255,
         title:="ordinals", description:="ordinal as string of the outline item")> Public Const ConstFNordinals = "ordials"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=3, _
            useforeignkey:=otForeignKeyImplementation.None)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        ''' <summary>
        ''' foreign key
        ''' </summary>
        ''' <remarks></remarks>
        <ormForeignKey(useforeignkey:=otForeignKeyImplementation.NativeDatabase, _
            entrynames:={constFNID, ConstFNDomainID}, _
            foreignkeyreferences:={XOutline.constobjectid & "." & XOutline.constFNID, _
            XOutline.constobjectid & "." & XOutline.ConstFNDomainID})> Public Const constFKXOUTLINE = "FK_XOUTLINE"

        ''' <summary>
        ''' fields
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(XID:="otli2", Datatype:=otDataType.Long,
           title:="ordinal", description:="ordinal as long of the outline")> Public Const ConstFNordinall = "ordiall"

        <ormObjectEntry(XID:="dlvuid", referenceobjectentry:=Deliverable.ConstObjectID & "." & Deliverable.ConstFNDLVUID, _
        isnullable:=True, useforeignkey:=otForeignKeyImplementation.NativeDatabase,
         title:="deliverable uid", description:="uid of the deliverable")> Public Const ConstFNUid = Deliverable.ConstFNDLVUID

        <ormObjectEntry(XID:="otli4", Datatype:=otDataType.Long, defaultvalue:=1,
          title:="identlevel", description:="identlevel as string of the outline")> Public Const ConstFNIdent = "level"

        <ormObjectEntry(XID:="otli10", Datatype:=otDataType.List, innerDatatype:=otDataType.Text,
         title:="Types", description:="types the outline key")> Public Const ConstFNTypes = "types"

        <ormObjectEntry(XID:="otli11", Datatype:=otDataType.List, innerDatatype:=otDataType.Text,
         title:="IDs", description:="ids the outline key")> Public Const ConstFNIDs = "ids"

        <ormObjectEntry(XID:="otli12", Datatype:=otDataType.List, innerDatatype:=otDataType.Text,
        title:="Values", description:="values the outline key")> Public Const ConstFNValues = "values"

        <ormObjectEntry(XID:="otli13", Datatype:=otDataType.Bool, defaultvalue:=False,
        title:="Grouping Item", description:="check if this an grouping item")> Public Const ConstFNisgroup = "isgrouped"

        <ormObjectEntry(XID:="otli14", Datatype:=otDataType.Bool, defaultvalue:=False,
       title:="Text Item", description:="check if this an text item")> Public Const ConstFNisText = "istext"

        <ormObjectEntry(XID:="otli14", Datatype:=otDataType.Text, isnullable:=True,
       title:="Text", description:="Text if a text item")> Public Const ConstFNText = "text"

        <ormObjectEntryMapping(EntryName:=constFNID)> Private _id As String = String.empty   ' ID of the outline

        Private _keys As New List(Of OutlineKey)    'keys and values
        Private _ordinal As Ordinal ' extramapping

        <ormObjectEntryMapping(EntryName:=ConstFNIdent)> Private _level As Long = 0
        <ormObjectEntryMapping(EntryName:=ConstFNisgroup)> Private _isGroup As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNisText)> Private _isText As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNText)> Private _text As String
        <ormObjectEntryMapping(EntryName:=ConstFNUid)> Private _deliverableUID As Long?
#Region "properties"

        ''' <summary>
        ''' Gets or sets the deliverable uid.
        ''' </summary>
        ''' <value>The deliverable uid.</value>
        Public Property DeliverableUid() As Long?
            Get
                Return Me._deliverableUID
            End Get
            Set(value As Long?)
                Me._deliverableUID = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text.
        ''' </summary>
        ''' <value>The text.</value>
        Public Property Text() As String
            Get
                Return Me._text
            End Get
            Set(value As String)
                Me._text = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the is text.
        ''' </summary>
        ''' <value>The is text.</value>
        Public Property IsText() As Boolean
            Get
                Return Me._isText
            End Get
            Set(value As Boolean)
                Me._isText = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the is group.
        ''' </summary>
        ''' <value>The is group.</value>
        Public Property IsGroup() As Boolean
            Get
                Return Me._isGroup
            End Get
            Set(value As Boolean)
                Me._isGroup = value
            End Set
        End Property

        ReadOnly Property OutlineID() As String
            Get
                OutlineID = _id

            End Get
        End Property

        ReadOnly Property ordinal() As Ordinal
            Get
                ordinal = _ordinal
            End Get

        End Property

        Public Property keys() As List(Of OutlineKey)
            Get
                keys = _keys
            End Get
            Set(value As List(Of OutlineKey))
                _keys = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property Level() As UShort
            Get

                Level = _level
            End Get
            Set(value As UShort)
                _level = value
                Me.IsChanged = True
            End Set
        End Property


#End Region


        ''' <summary>
        ''' infuses the data object by record
        ''' </summary>
        ''' <param name="record"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Sub XOutlineItem_OnInfused(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnInfused

            Dim aType As otDataType
            Dim aValue As Object


            '***
            Try
                aValue = e.Record.GetValue(ConstFNordinals)

                If IsNumeric(aValue) Then
                    _ordinal = New Ordinal(CLng(Record.GetValue(ConstFNordinall)))
                Else
                    _ordinal = New Ordinal(CStr(Record.GetValue(ConstFNordinall)))
                End If

                ' get the keys and values
                Dim ids As String() = e.Record.GetValue(ConstFNIDs)
                'Dim ids As String()
                'If idstr <> String.empty AndAlso Not IsNull(idstr) Then
                '    ids = SplitMultbyChar(idstr, ConstDelimiter)
                'Else
                '    ids = {}
                'End If
                Dim valuestr As String() = e.Record.GetValue(ConstFNValues)
                Dim values As String()
                If IsArrayInitialized(valuestr) Then
                    values = valuestr
                Else
                    values = {}
                End If
                Dim typestr As String() = e.Record.GetValue(ConstFNTypes)
                Dim types As String()
                If IsArrayInitialized(typestr) Then
                    types = typestr
                Else
                    types = {}
                End If

                For i = 0 To ids.Length - 1
                    If i < types.Length Then
                        Try
                            Select Case CLng(types(i))
                                Case CLng(otDataType.Bool)
                                    aType = otDataType.Bool
                                    aValue = CBool(values(i))
                                Case CLng(otDataType.[Date]), CLng(otDataType.[Timestamp]), CLng(otDataType.Time)
                                    aType = otDataType.[Date]
                                    aValue = CDate(values(i))
                                Case CLng(otDataType.Text)
                                    aType = otDataType.Text
                                    aValue = values(i)
                                Case CLng(otDataType.[Long])
                                    aType = otDataType.[Long]
                                    aValue = CLng(values(i))
                                Case Else
                                    Call CoreMessageHandler(procedure:="XOutlineItem.infuse", messagetype:=otCoreMessageType.InternalError,
                                                            message:="Outline datatypes couldnot be determined ", argument:=types(i))
                                    e.AbortOperation = True
                                    Exit Sub
                            End Select

                        Catch ex As Exception
                            Call CoreMessageHandler(exception:=ex, procedure:="XOutlineItem.infuse",
                                                    messagetype:=otCoreMessageType.InternalError, message:="Outline keys couldnot be filled ")
                            e.AbortOperation = True
                            Exit Sub
                        End Try

                        '**
                        _keys.Add(New OutlineKey(aType, ids(i), aValue))
                    End If
                Next
                e.Proceed = True
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="XOutlineItem.Infuse")
                SetUnloaded()
                e.AbortOperation = True
            End Try

        End Sub
        ''' <summary>
        ''' handles the feed
        ''' </summary>
        ''' <param name="record"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Sub XOutlineItem_OnFed(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnFed

            '***
            Try

                '** own feed record
                If _ordinal.Type = OrdinalType.longType Then
                    Call Me.Record.SetValue(ConstFNordinall, _ordinal.Value)
                Else
                    Call Me.Record.SetValue(ConstFNordinall, 0)
                End If

                '***
                Dim idstr As String = ConstDelimiter
                Dim valuestr As String = ConstDelimiter
                Dim typestr As String = ConstDelimiter

                For Each key As OutlineKey In _keys
                    idstr &= key.ID & ConstDelimiter
                    If key.ID.ToLower = "uid" Then
                        Me.Record.SetValue(ConstFNUid, CLng(key.Value))
                    End If
                    typestr &= CLng(key.Type) & ConstDelimiter
                    valuestr &= CStr(key.Value) & ConstDelimiter
                Next

                If idstr = ConstDelimiter Then idstr = String.empty
                If valuestr = ConstDelimiter Then valuestr = String.empty
                If typestr = ConstDelimiter Then typestr = String.empty

                Call Me.Record.SetValue(ConstFNIDs, UCase(idstr))
                Call Me.Record.SetValue(ConstFNValues, valuestr)
                Call Me.Record.SetValue(ConstFNTypes, LCase(typestr))
                e.Proceed = True

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="XOutlineItem.OnFed")
                e.AbortOperation = True
            End Try

        End Sub
        ''' <summary>
        ''' retrieves a sorted list of items by uid
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function AllByID(ByVal id As String) As SortedList(Of Ordinal, XOutlineItem)
            Dim aCollection As New SortedList(Of Ordinal, XOutlineItem)
            Dim aRecordCollection As New List(Of ormRecord)
            Dim aTable As iormRelationalTableStore
            Dim aRecord As ormRecord
            Dim anEntry As New XOutlineItem


            Try
                aTable = ot.GetPrimaryTableStore(ConstPrimaryTableID)
                Dim aCommand As ormSqlSelectCommand = aTable.CreateSqlSelectCommand(id:="AllByID")
                If Not aCommand.IsPrepared Then
                    aCommand.OrderBy = "[" & ConstPrimaryTableID & "." & ConstFNordinall & "] asc"
                    aCommand.Where = "[" & constFNID & "] = @ID"
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@ID", ColumnName:=constFNID, tableid:=ConstPrimaryTableID))
                    aCommand.Prepare()
                End If

                aCommand.SetParameterValue(ID:="@ID", value:=id)
                aRecordCollection = aCommand.RunSelect

                If aRecordCollection.Count > 0 Then
                    ' records read
                    For Each aRecord In aRecordCollection
                        ' add the Entry as Component
                        anEntry = New XOutlineItem
                        If InfuseDataObject(record:=aRecord, dataobject:=anEntry) Then
                            aCollection.Add(value:=anEntry, key:=anEntry.ordinal)
                        End If
                    Next aRecord

                End If
                Return aCollection
            Catch ex As Exception
                Call CoreMessageHandler(procedure:="XOutlineItem.allByID", argument:=id,
                                        exception:=ex, objectname:=ConstPrimaryTableID)
                Return aCollection
            End Try


        End Function

        ''' <summary>
        ''' retrieves the data object from the data store
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="ordinal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Retrieve(ByVal id As String, ByVal ordinal As String) As XOutlineItem
            Return Retrieve(id, New Ordinal(ordinal))
        End Function
        Public Shared Function Retrieve(ByVal id As String, ByVal ordinal As Long) As XOutlineItem
            Return Retrieve(id, New Ordinal(ordinal))
        End Function
        Public Shared Function Retrieve(ByVal id As String, ByVal ordinal As Ordinal) As XOutlineItem
            Dim pkarry() As Object = {id, ordinal.ToString}
            Return ormBusinessObject.RetrieveDataObject(Of XOutlineItem)(pkarry)
        End Function

        ''' <summary>
        ''' create a new outline item in the persistable data store
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <param name="ordinal"></param>
        ''' <param name="uid"></param>
        ''' <param name="level"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal ID As String, ByVal ordinal As String,
                                     Optional uid As Long? = Nothing, _
                                     Optional level As UShort? = Nothing, _
                                     Optional runtimeonly As Boolean = False) As XOutlineItem
            Return Create(ID, New Ordinal(ordinal), uid, level, runtimeonly)
        End Function
        Public Shared Function Create(ByVal ID As String, ByVal ordinal As Long, _
                                      Optional uid As Long? = Nothing, _
                                      Optional level As UShort? = Nothing, _
                                      Optional runtimeonly As Boolean = False) As XOutlineItem
            Return Create(ID, New Ordinal(ordinal), uid, level, runtimeonly)
        End Function
        Public Shared Function Create(ByVal ID As String, ByVal ordinal As Ordinal, _
                                      Optional uid As Long? = Nothing, _
                                      Optional level As UShort? = 0, _
                                      Optional runtimeonly As Boolean = False) As XOutlineItem
            Dim aRecord As New ormRecord
            With aRecord
                .SetValue(constFNID, ID.ToUpper)
                .SetValue(ConstFNordinals, ordinal.ToString)
                If uid.HasValue Then .SetValue(ConstFNUid, uid)
                If level.HasValue Then .SetValue(ConstFNIdent, level)
            End With
            Return ormBusinessObject.CreateDataObject(Of XOutlineItem)(aRecord, checkUnique:=True, runtimeOnly:=runtimeonly)
        End Function
    End Class
End Namespace
