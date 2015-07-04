Option Explicit On

REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** COMMON BUSINESS OBJECT DEFINITION Classes 
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2013-08-08
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************''' <summary>

Imports System.Diagnostics.Debug
Imports System.Text.RegularExpressions
Imports OnTrack.Database
Imports OnTrack.Calendar
Imports OnTrack.Core

Namespace OnTrack.Commons

    ''' <summary>
    ''' Domain Event Arguments
    ''' </summary>
    ''' <remarks></remarks>

    Public Class DomainEventArgs
        Inherits EventArgs

        Private _Session As Session
        Private _Domain As Domain

        Public Sub New(domain As Domain, Optional session As Session = Nothing)
            _Session = session
            _Domain = domain
        End Sub
        ''' <summary>
        ''' Gets or sets the new domain ID.
        ''' </summary>
        ''' <value>The new domain ID.</value>
        Public Property Domain() As Domain
            Get
                Return Me._Domain
            End Get
            Set(value As Domain)
                Me._Domain = value
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
    ''' class to define a list of lookup values
    ''' </summary>
    ''' <remarks>
    ''' 
    ''' Design Principles:
    ''' 
    ''' 1. Value lists are stand-alone and must exist before a value entry can be created.
    ''' 
    ''' 2. value entry are added by creating themselves e.g. ValueEntry.Create(setid:= ...). It will be added automatically to the List
    ''' 
    ''' 3. On loading the set all the value entries will be retrieved as well due to relation.
    ''' 
    ''' </remarks>
    <ormObject(id:=ValueList.ConstObjectID, version:=1, adddomainbehavior:=True, adddeletefieldbehavior:=True, usecache:=True, _
        modulename:=ConstModuleCommons, Title:="Value List", description:="definition of a list of lookup values")> _
    Public Class ValueList
        Inherits ormBusinessObject

        Public Const ConstObjectID = "ValueList"

        ''' <summary>
        ''' Table Definition
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=1, usecache:=True)> Public Const ConstPrimaryTableID = "TBLDEFVALUELISTS"

        '** primary Keys
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty},
            XID:="VL1", title:="List ID", description:="ID of the value list")> Public Const ConstFNListID = "LISTID"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2 _
         , useforeignkey:=otForeignKeyImplementation.NativeDatabase, defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
          XID:="VL3", title:="Description", description:="description of the property section")> Public Const ConstFNDescription = "DESC"


        ''' <summary>
        ''' Mappings
        ''' </summary>
        ''' <remarks></remarks>

        <ormObjectEntryMapping(EntryName:=ConstFNListID)> Private _id As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _description As String = String.Empty

        ''' <summary>
        ''' Relations
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(ValueEntry), cascadeOnDelete:=True, cascadeOnUpdate:=True, _
            fromEntries:={ConstFNListID}, toEntries:={ValueEntry.ConstFNListID})> Public Const ConstRValues = "RELVALUES"

        <ormObjectEntryMapping(RelationName:=ConstRValues, infuseMode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand, _
            keyentries:={ValueEntry.ConstFNValue})> Private WithEvents _valuesCollection As New ormRelationCollection(Of ValueEntry)(Me, {ValueEntry.ConstFNValue})

#Region "Properties"

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
        ReadOnly Property ValueEntries As ormRelationCollection(Of ValueEntry)
            Get
                Return _valuesCollection
            End Get
        End Property

        ''' <summary>
        ''' returns a List of Values (objects)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Values As IList(Of Object)
            Get
                Return _valuesCollection.Select(Function(x) x.Value).ToList()
            End Get
        End Property
#End Region

        ''' <summary>
        ''' retrieve  the property section from store
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(id As String, Optional domainid As String = Nothing) As ValueList
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Return ormBusinessObject.RetrieveDataObject(Of ValueList)(pkArray:={id.ToUpper, domainid.ToUpper}, domainID:=domainid)
        End Function

        ''' <summary>
        ''' creates a persistable property section
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(id As String, Optional domainid As String = Nothing) As ValueList
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Return ormBusinessObject.CreateDataObject(Of ValueList)(pkArray:={id.ToUpper, domainid.ToUpper}, domainID:=domainid, checkUnique:=True)
        End Function


        ''' <summary>
        ''' Handler for the OnAdded event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>

        Private Sub ValuesCollection_OnAdded(sender As Object, e As Database.ormRelationCollection(Of ValueEntry).EventArgs) Handles _valuesCollection.OnAdded
            If Not _valuesCollection.Contains(e.Dataobject) Then
                _valuesCollection.Add(e.Dataobject)
            End If
        End Sub


    End Class


    ''' <summary>
    ''' Value Entry Class for List of Values
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=ValueEntry.ConstObjectID, modulename:=ConstModuleCommons, Version:=1, Description:="lookup value pairs for general use", _
        useCache:=True, adddeletefieldbehavior:=True, addDomainBehavior:=True)> Public Class ValueEntry
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable

        Public Const ConstObjectID = "ValueEntry"
        '** Table Schema
        <ormTableAttribute(Version:=1, usecache:=True)> Public Const ConstPrimaryTableID As String = "tblDefValueEntries"

        '*** Primary Keys
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty},
           XID:="VE2", title:="List", description:="ID of the list of values")> Public Const ConstFNListID = "ID"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, PrimaryKeyOrdinal:=2, _
            XID:="VE3", title:="Value", description:="value entry")> Public Const ConstFNValue = "VALUE"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=3 _
         , useforeignkey:=otForeignKeyImplementation.NativeDatabase, defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        ''' <summary>
        ''' Columns
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, isnullable:=True, _
           XID:="VE4", title:="selector", description:="")> Public Const ConstFNSelector = "selector"

        <ormObjectEntry(Datatype:=otDataType.Long, defaultvalue:=otDataType.Text, dbdefaultvalue:="3", _
          XID:="VE5", title:="datatype", description:="datatype of the  value")> Public Const ConstFNDatatype = "datatype"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, isnullable:=True, _
           XID:="VE10", title:="Description", description:="description of the entry")> Public Const ConstFNDescription = "DESC"

        <ormObjectEntry(Datatype:=otDataType.Long, defaultvalue:=10, dbdefaultvalue:="10", _
         XID:="VE6", title:="Ordinal", description:="ordinal value of the entry")> Public Const ConstFNOrdinal = "ORDINAL"

        ' fields
        <ormObjectEntryMapping(EntryName:=ConstFNDomainID)> Private _DomainID As String
        <ormObjectEntryMapping(EntryName:=ConstFNListID)> Private _ID As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _Description As String
        <ormObjectEntryMapping(EntryName:=ConstFNSelector)> Private _selector As String
        <ormObjectEntryMapping(EntryName:=ConstFNValue)> Private _valuestring As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNOrdinal)> Private _ordinal As Long
        <ormObjectEntryMapping(EntryName:=ConstFNDatatype)> Private _datatype As otDataType = 0

        '' dynamic
        Private _value As Object
        Private _list As ValueList 'cached backlink

#Region "Properties"
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
        ''' Gets the value list
        ''' </summary>
        ''' <value>The list.</value>
        Public ReadOnly Property List() As ValueList
            Get
                If _list Is Nothing Then _list = ValueList.Retrieve(id:=_ID)
                Return Me._list
            End Get

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
                SetValue(ConstFNOrdinal, value)
            End Set
        End Property

        ''' <summary>
        ''' gets the ID of the Domain
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DomainID() As String
            Get
                Return _DomainID
            End Get

        End Property
        ''' <summary>
        ''' gets the ID of the Setting
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ListID() As String
            Get
                Return _ID
            End Get

        End Property
        ''' <summary>
        ''' Description of the setting
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Selector() As String
            Get
                Return _selector
            End Get
            Set(value As String)
                SetValue(ConstFNSelector, value)
            End Set
        End Property
        ''' <summary>
        ''' returns the datatype 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Datatype As otDataType
            Get
                Return _datatype
            End Get
            Set(value As otDataType)
                SetValue(ConstFNDatatype, value)
            End Set
        End Property

        ''' <summary>
        ''' gets or sets the String Presentaton
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ValueString As String
            Get
                Return _valuestring
            End Get
            Set(value As String)
                SetValue(ConstFNValue, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the value of the domain setting
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Value As Object
            Set(value As Object)
                If value IsNot Nothing Then
                    Me.ValueString = value.ToString
                Else
                    Me.ValueString = String.Empty
                End If
            End Set
            Get
                Return _value
            End Get
        End Property

#End Region


        ''' <summary>
        ''' Handles OnCreating 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ValueEntry_OnCreating(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreating
            Dim my As ValueEntry = TryCast(e.DataObject, ValueEntry)

            If my IsNot Nothing Then
                Dim listid As String = e.Record.GetValue(ConstFNListID)
                If listid Is Nothing Then
                    CoreMessageHandler(message:="value list id does not exist", procedure:="valueentry.OnCreating", _
                                       messagetype:=otCoreMessageType.ApplicationError, _
                                       argument:=my.ListID)
                    e.AbortOperation = True
                    Return
                End If
                ''' even if it is early to retrieve the value list and set it (since this might disposed since we have not run through checkuniqueness and cache)
                ''' we need to check on the object here
                _list = ValueList.Retrieve(id:=listid)
                If _list Is Nothing Then
                    CoreMessageHandler(message:="value list does not exist", procedure:="valueentry.OnCreated", _
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
        Public Sub ValueEntry_OnCreated(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreated
            Dim my As ValueEntry = TryCast(e.DataObject, ValueEntry)

            If my IsNot Nothing Then
                If _list Is Nothing Then
                    _list = ValueList.Retrieve(id:=my.ListID)
                    If _list Is Nothing Then
                        CoreMessageHandler(message:="value list does not exist", procedure:="valueentry.OnCreated", _
                                          messagetype:=otCoreMessageType.ApplicationError, _
                                           argument:=my.ListID)
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
        Public Sub ValueEntry_OnInfused(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnInfused
            Dim aVAlue As Object
            Dim my As ValueEntry = TryCast(e.DataObject, ValueEntry)

            Try
                ''' infuse is called on create as well as on retrieve / inject 
                ''' only on the create case we need to add to the properties otherwise
                ''' propertyset will load the property
                ''' or the property will stand alone
                If my IsNot Nothing AndAlso e.Infusemode = otInfuseMode.OnCreate AndAlso _list IsNot Nothing Then
                    _list.ValueEntries.Add(my)
                End If

                ''' get the value converted
                _value = Core.DataType.To(Record.GetValue(ConstFNValue), _datatype)


            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="ValueEntry.Infuse")
            End Try


        End Sub
        ''' <summary>
        ''' Update the record from the properties
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Sub OnFeedRecord(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnFed


            Try
                '** special Handling
                Dim aValue = DirectCast(e.DataObject, ValueEntry).Value

                Select Case DirectCast(e.DataObject, ValueEntry).Datatype

                    Case otDataType.Numeric
                        Call Me.Record.SetValue(ConstFNValue, CStr(aValue))
                    Case otDataType.Text, otDataType.Memo
                        Call Me.Record.SetValue(ConstFNValue, CStr(aValue))
                    Case otDataType.Runtime, otDataType.Formula, otDataType.Binary
                        Call CoreMessageHandler(procedure:="ValueEntry.persist", _
                                              message:="datatype (runtime, formular, binary) not specified how to be persisted", argument:=_datatype)
                    Case otDataType.[Date]
                        If Microsoft.VisualBasic.IsDate(aValue) Then
                            Call Me.Record.SetValue(ConstFNValue, Converter.Date2LocaleShortDateString(CDate(aValue)))
                        Else
                            Call Me.Record.SetValue(ConstFNValue, CStr(aValue))
                        End If
                    Case otDataType.[Long]
                        Call Me.Record.SetValue(ConstFNValue, CStr(aValue))
                    Case otDataType.Timestamp
                        If Microsoft.VisualBasic.IsDate(aValue) Then
                            Call Me.Record.SetValue(ConstFNValue, Converter.DateTime2UniversalDateTimeString(CDate(aValue)))
                        Else
                            Call Me.Record.SetValue(ConstFNValue, CStr(aValue))
                        End If
                    Case otDataType.Bool
                        Call Me.Record.SetValue(ConstFNValue, CStr(aValue))
                    Case Else
                        Call Me.Record.SetValue(ConstFNValue, CStr(aValue))
                        Call CoreMessageHandler(procedure:="ValueEntry.OnFed", _
                                              message:="datatype not specified how to be persisted", argument:=_datatype)
                End Select



            Catch ex As Exception
                Call CoreMessageHandler(procedure:="ValueEntry.OnFed", exception:=ex)
            End Try
        End Sub


        ''' <summary>
        ''' Retrieve the workspaceID Cache Object
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal listID As String, ByVal value As Object, Optional ByVal domainid As String = Nothing, Optional forcereload As Boolean = False) As ValueEntry
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim pkarray() As Object = {UCase(listID), value.ToString, UCase(domainid)}
            Return RetrieveDataObject(Of ValueEntry)(pkArray:=pkarray, forceReload:=forcereload)
        End Function
        ''' <summary>
        ''' Retrieve all value entries by list id in the domain
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function RetrieveByListID(ByVal listID As String, Optional ByVal domainid As String = Nothing, Optional forcereload As Boolean = False) As List(Of ValueEntry)
            Dim aList As List(Of ValueEntry) = ormBusinessObject.AllDataObject(Of ValueEntry)(ID:="allbyListID", domainid:=domainid)
            Return aList
        End Function

        ''' <summary>
        ''' creates a new value entry for listid and value in the domain
        ''' </summary>
        ''' <param name="workspaceID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal listid As String, ByVal value As Object, Optional ByVal domainid As String = Nothing) As ValueEntry
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey() As Object = {UCase(listid), value.ToString, UCase(domainid)}
            Return ormBusinessObject.CreateDataObject(Of ValueEntry)(primarykey, checkUnique:=True)
        End Function

    End Class

    ''' <summary>
    ''' Domain Setting Definition Class
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=DomainSetting.ConstObjectID, modulename:=ConstModuleCommons, adddomainbehavior:=True, description:="properties per domain", _
        Version:=1, useCache:=True)> Public Class DomainSetting
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable

        '** const
        Public Const ConstObjectID = "DomainSetting"
        '** will be cached over domains
        <ormTableAttribute(adddeletefieldbehavior:=False, usecache:=False, Version:=1)> Public Const ConstPrimaryTableID As String = "tblDefDomainSettings"

        <ormObjectEntry(XID:="DMS1", _
            referenceobjectentry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, _
            title:="domain", Description:="domain identifier", defaultvalue:=ConstGlobalDomain, _
            PrimaryKeyOrdinal:=1, _
            isactive:=True, useforeignkey:=otForeignKeyImplementation.ORM)> _
        Const ConstFNDomainID As String = Domain.ConstFNDomainID

        <ormObjectEntry(XID:="DMS2", _
           Datatype:=otDataType.Text, size:=100, PrimaryKeyOrdinal:=2, _
           properties:={ObjectEntryProperty.Keyword}, _
           title:="Setting", description:="ID of the setting per domain")> _
        Const ConstFNSettingID = "id"

        <ormObjectEntry(XID:="DMS3", _
            Datatype:=otDataType.Text, size:=100, title:="Description")> _
        Const ConstFNDescription = "desc"

        <ormObjectEntry(XID:="DMS4", Datatype:=otDataType.Memo, _
           title:="value", description:="value of the domain setting in string presentation")> _
        Const ConstFNValue = "value"

        <ormObjectEntry(XID:="DMS5", Datatype:=otDataType.Long, defaultvalue:=otDataType.Text, _
          title:="datatype", description:="datatype of the domain setting value")> _
        Const ConstFNDatatype = "datatype"

        ' fields
        <ormObjectEntryMapping(EntryName:=ConstFNDomainID)> Private _DomainID As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNSettingID)> Private _ID As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _description As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNValue)> Private _valuestring As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNDatatype)> Private _datatype As otDataType = 0

#Region "Properties"
        ''' <summary>
        ''' gets the ID of the Domain
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property DomainID() As String
            Get
                DomainID = _DomainID
            End Get

        End Property
        ''' <summary>
        ''' gets the ID of the Setting
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ID() As String
            Get
                ID = _ID
            End Get

        End Property
        ''' <summary>
        ''' Description of the setting
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description() As String
            Get
                Description = _description
            End Get
            Set(value As String)
                _description = value
                Me.IsChanged = True
            End Set
        End Property
        ''' <summary>
        ''' returns the datatype 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Datatype As otDataType
            Set(value As otDataType)
                _datatype = value
            End Set
            Get
                Return _datatype
            End Get
        End Property
        ''' <summary>
        ''' gets or sets the value of the domain setting
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property value As Object
            Set(value As Object)
                If value Is Nothing Then
                    _valuestring = String.Empty
                Else
                    _valuestring = value.ToString
                End If
            End Set
            Get
                Try
                    If String.IsNullOrEmpty(_valuestring) Then Return Core.DataType.GetDefaultValue(_datatype)
                    Return Core.DataType.To(_valuestring, _datatype)
                Catch ex As Exception
                    CoreMessageHandler(exception:=ex, message:="could not convert value to data type " & _datatype, _
                                       argument:=_valuestring, procedure:="DomainSetting.value", messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End Try

            End Get
        End Property
#End Region

        ''' <summary>
        ''' Retrieve the workspaceID Cache Object
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal domainID As String, ByVal id As String, Optional forcereload As Boolean = False) As DomainSetting
            Dim pkarray() As Object = {UCase(domainID), UCase(id)}
            Return RetrieveDataObject(Of DomainSetting)(pkArray:=pkarray, forceReload:=forcereload)
        End Function
        ''' <summary>
        ''' Retrieve the workspaceID Cache Object
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function RetrieveForDomain(domainid As String) As List(Of DomainSetting)
            Dim aList As List(Of DomainSetting) = ormBusinessObject.AllDataObject(Of DomainSetting)(ID:="allforDomain", domainid:=domainid)
            'Additional parameters and where clause is not needed - automatically settings for the current domainid are selected
            'by the algo
            'where:="[" & ConstFNDomainID & "] = @" & ConstFNDomainID, _
            'parameters:={New ormSqlCommandParameter(id:="@" & ConstFNDomainID, columnname:=ConstFNDomainID, tablename:=ConstPrimaryTableID, value:=domainID)}.ToList)
            Return aList
        End Function
        ''' <summary>
        ''' Retrieve the workspaceID Cache Object
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function RetrieveAllofCurrentDomain() As List(Of DomainSetting)
            Dim aList As List(Of DomainSetting) = ormBusinessObject.AllDataObject(Of DomainSetting)(ID:="allbyDomain")
            'Additional parameters and where clause is not needed - automatically settings for the current domainid are selected
            'by the algo
            'where:="[" & ConstFNDomainID & "] = @" & ConstFNDomainID, _
            'parameters:={New ormSqlCommandParameter(id:="@" & ConstFNDomainID, columnname:=ConstFNDomainID, tablename:=ConstPrimaryTableID, value:=domainID)}.ToList)
            Return aList
        End Function

        ''' <summary>
        ''' creates with this object a new persistable Def workspaceID
        ''' </summary>
        ''' <param name="workspaceID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal domainID As String, ByVal id As String) As DomainSetting
            Dim primarykey() As Object = {UCase(domainID), UCase(id)}
            Return ormBusinessObject.CreateDataObject(Of DomainSetting)(primarykey, checkUnique:=False)
        End Function

    End Class

    ''' <summary>
    ''' User Definition Class of an OnTrack User
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=Group.ConstObjectID, description:="group definition for users", _
        modulename:=ConstModuleCommons, Version:=1, usecache:=True, adddomainbehavior:=True, adddeletefieldbehavior:=True, isbootstrap:=False)> _
    Public Class Group
        Inherits ormBusinessObject


        '*** Object ID
        Public Const ConstObjectID = "Group"

        '*** Schema Table
        <ormTableAttribute(version:=1, usecache:=True)> Public Const ConstPrimaryTableID As String = "tblDefGroups"

        '*** Primary Keys
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty},
          XID:="G1", title:="Group", description:="name of the OnTrack user group")> Public Const ConstFNGroupname = "groupname"
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2, _
                       defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        '*** Fields
        <ormObjectEntry(Datatype:=otDataType.Text, size:=255, _
        XID:="G5", title:="description", description:="description of the OnTrack user group")> Public Const ConstFNDescription = "desc"

        <ormObjectEntry(referenceobjectentry:=Workspace.ConstObjectID & "." & Workspace.ConstFNID, isnullable:=True, _
            XID:="G10", title:="Default Workspace", description:="default workspace of the OnTrack user")> Public Const ConstFNDefaultWorkspace = "defws"
        <ormObjectEntry(referenceobjectentry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, isnullable:=True, _
           XID:="G11", title:="Default Domain", description:="default domain of the OnTrack user")> Public Const ConstFNDefaultDomainID = "defdomain"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, _
           XID:="UR1", title:="Alter Schema Right", description:="has user the right to alter the database schema")> _
        Public Const ConstFNAlterSchema = "alterschema"
        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, _
          XID:="UR2", title:="Update Data Right", description:="has user the right to update data (new/change/delete)")> _
        Public Const ConstFNUpdateData = "updatedata"
        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=True, _
          XID:="UR3", title:="Read Data Right", description:="has user the right to read the database data")> Public Const ConstFNReadData = "readdata"
        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, _
          XID:="UR4", title:="No Access", description:="has user no access")> Public Const ConstFNNoAccess = "noright"

        '* Relations
        '* Members
        <ormRelation(cascadeOnDelete:=True, cascadeonUpdate:=True, FromEntries:={ConstFNGroupname}, toEntries:={GroupMember.ConstFNGroupname}, _
            LinkObject:=GetType(GroupMember))> Const ConstRelMembers = "members"
        <ormObjectEntryMapping(Relationname:=ConstRelMembers, infusemode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand)> _
        Private _groupmembers As New List(Of GroupMember)

        'fields
        <ormObjectEntryMapping(EntryName:=ConstFNGroupname)> Private _groupname As String
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _desc As String

        <ormObjectEntryMapping(EntryName:=ConstFNDefaultWorkspace)> Private _DefaultWorkspaceID As String
        <ormObjectEntryMapping(EntryName:=ConstFNDefaultDomainID)> Private _DefaultDomainID As String

        <ormObjectEntryMapping(EntryName:=ConstFNReadData)> Private _hasRead As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNUpdateData)> Private _hasUpdate As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNAlterSchema)> Private _hasAlterSchema As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNNoAccess)> Private _hasNoRights As Boolean


#Region "Properties"



        Public Property Description() As String
            Get
                Description = _desc
            End Get
            Set(ByVal avalue As String)
                SetValue(entryname:=ConstFNDescription, value:=avalue)
            End Set
        End Property

        Public Property DefaultWorkspaceID As String

            Get
                DefaultWorkspaceID = _DefaultWorkspaceID
            End Get
            Set(value As String)
                SetValue(entryname:=ConstFNDefaultWorkspace, value:=value)
            End Set
        End Property

        ReadOnly Property GroupName() As String
            Get
                GroupName = _groupname
            End Get
        End Property
        ''' <summary>
        ''' has no rights at all ?! -> Blocked ?!
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property HasNoRights() As Boolean
            Get
                HasNoRights = _hasNoRights
            End Get
            Set(value As Boolean)
                _hasNoRights = value
                IsChanged = True
            End Set
        End Property
        ''' <summary>
        ''' has right to read
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property HasReadRights() As Boolean
            Get
                HasReadRights = _hasRead
            End Get
            Set(value As Boolean)
                _hasRead = value
                IsChanged = True
            End Set
        End Property
        ''' <summary>
        ''' has right to update and read data
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property HasUpdateRights() As Boolean
            Get
                HasUpdateRights = _hasUpdate
            End Get
            Set(value As Boolean)
                _hasUpdate = value
                IsChanged = True
            End Set
        End Property
        ''' <summary>
        ''' Has Right to update, read and alter schema data
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property HasAlterSchemaRights() As Boolean
            Get
                HasAlterSchemaRights = _hasAlterSchema
            End Get
            Set(value As Boolean)
                _hasAlterSchema = value
                IsChanged = True
            End Set
        End Property

        ''' <summary>
        ''' gets the accessright out of the setting
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AccessRight As otAccessRight
            Get
                '* highes right first
                If Me.HasAlterSchemaRights Then
                    Return otAccessRight.AlterSchema
                ElseIf Me.HasUpdateRights Then
                    Return otAccessRight.ReadUpdateData
                ElseIf Me.HasReadRights Then
                    Return otAccessRight.ReadOnly
                End If

                Return otAccessRight.Prohibited
            End Get
            Set(value As otAccessRight)
                Select Case value
                    Case otAccessRight.AlterSchema
                        Me.HasAlterSchemaRights = True
                        Me.HasUpdateRights = True
                        Me.HasReadRights = True
                        Me.HasNoRights = False
                    Case otAccessRight.ReadUpdateData
                        Me.HasAlterSchemaRights = False
                        Me.HasUpdateRights = True
                        Me.HasReadRights = True
                        Me.HasNoRights = False
                    Case otAccessRight.ReadOnly
                        Me.HasAlterSchemaRights = False
                        Me.HasUpdateRights = False
                        Me.HasReadRights = True
                        Me.HasNoRights = False
                    Case otAccessRight.Prohibited
                        Me.HasAlterSchemaRights = False
                        Me.HasUpdateRights = False
                        Me.HasReadRights = False
                        Me.HasNoRights = True
                    Case Else
                        CoreMessageHandler(message:="access right not implemented", argument:=value, procedure:="Group.AccessRight", messagetype:=otCoreMessageType.InternalError)

                End Select

            End Set
        End Property
#End Region

        ''' <summary>
        ''' Returns a collection of all objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function All() As List(Of Group)
            Return ormBusinessObject.AllDataObject(Of Group)(orderby:=ConstFNGroupname)
        End Function

        ''' <summary>
        ''' Retrieve a User Definition
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal groupname As String, Optional domainid As String = Nothing, Optional forcereload As Boolean = False) As Group
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Return RetrieveDataObject(Of Group)(pkArray:={groupname, domainid}, domainID:=domainid, forceReload:=forcereload)
        End Function

        ''' <summary>
        ''' Create persistency for this object
        ''' </summary>
        ''' <param name="groupname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal groupname As String, Optional domainid As String = Nothing) As Group
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey() As Object = {groupname, domainid}
            Return ormBusinessObject.CreateDataObject(Of Group)(primarykey, domainID:=domainid, checkUnique:=True)
        End Function

    End Class

    ''' <summary>
    ''' Group Member Definition Class of an OnTrack User
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=GroupMember.ConstObjectID, description:="group member definition of n:m relation from user to groups", _
        modulename:=ConstModuleCommons, Version:=1, usecache:=True, isbootstrap:=False, adddomainbehavior:=True, adddeletefieldbehavior:=True)> _
    Public Class GroupMember
        Inherits ormBusinessObject


        '*** Object ID
        Public Const ConstObjectID = "GroupMember"

        <ormTableAttribute(version:=1)> Public Const ConstPrimaryTableID As String = "tblDefGroupMembers"
        <ormIndex(columnname1:=ConstFNUsername, columnname2:=ConstFNDomainID, columnname3:=ConstFNGroupname)> Public Const ConstIndUser As String = "indUser"

        ''' <summary>
        ''' PrimaryKey
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=Group.ConstObjectID & "." & Group.ConstFNGroupname, PrimaryKeyOrdinal:=1, _
          XID:="G1", title:="Group", description:="name of the OnTrack user group")> _
        Public Const ConstFNGroupname = "groupname"
        <ormObjectEntry(referenceObjectEntry:=User.ConstObjectID & "." & User.ConstFNUsername, PrimaryKeyOrdinal:=2, _
          XID:="G1", title:="Group", description:="name of the OnTrack user group", useforeignkey:=otForeignKeyImplementation.NativeDatabase)> _
        Public Const ConstFNUsername = "username"

        ''' <summary>
        ''' Domain
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=3, _
                       defaultvalue:=ConstGlobalDomain, useforeignkey:=otForeignKeyImplementation.None)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        ''' <summary>
        ''' Foreign Key to Group
        ''' </summary>
        ''' <remarks></remarks>
        <ormForeignKey(entrynames:={ConstFNGroupname, ConstFNDomainID}, _
            foreignkeyreferences:={Group.ConstObjectID & "." & Group.ConstFNGroupname, Group.ConstObjectID & "." & Group.ConstFNDomainID}, _
            useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const constFKGroups = "FKGroups"


        '*** Fields


        'mapping
        <ormObjectEntryMapping(EntryName:=ConstFNGroupname)> Private _groupname As String
        <ormObjectEntryMapping(EntryName:=ConstFNUsername)> Private _username As String


#Region "Properties"

        ReadOnly Property GroupName() As String
            Get
                GroupName = _groupname
            End Get
        End Property
        ReadOnly Property Username() As String
            Get
                Username = _username
            End Get
        End Property

#End Region

        ''' <summary>
        ''' Returns a collection of all objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function All() As List(Of Group)
            Return ormBusinessObject.AllDataObject(Of Group)(orderby:=ConstFNGroupname)
        End Function

        ''' <summary>
        ''' Retrieve a User Definition
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal groupname As String, ByVal username As String, Optional ByVal domainid As String = Nothing, Optional forcereload As Boolean = False) As GroupMember
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Return RetrieveDataObject(Of GroupMember)(pkArray:={groupname, username, domainid}, domainID:=domainid, forceReload:=forcereload)
        End Function

        ''' <summary>
        ''' Returns the Groupdefinition
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetGroup() As Group
            If Me.IsAlive(subname:="GetGroup") Then
                Return Group.Retrieve(groupname:=Me.GroupName)
            End If
        End Function
        ''' <summary>
        ''' Returns the Userdefinition
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetUser() As User
            If Me.IsAlive(subname:="GetUser") Then
                Return User.Retrieve(username:=Me.Username)
            End If
        End Function
        ''' <summary>
        ''' Create persistency for this object
        ''' </summary>
        ''' <param name="groupname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal groupname As String, ByVal username As String, Optional ByVal domainid As String = Nothing, Optional runtimeOnly As Boolean = False) As GroupMember
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey() As Object = {groupname, username, domainid}
            Return ormBusinessObject.CreateDataObject(Of GroupMember)(primarykey, domainID:=domainid, checkUnique:=False, runtimeOnly:=runtimeOnly)
        End Function

    End Class

    ''' <summary>
    ''' User Definition Class of an OnTrack User
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=User.ConstObjectID, description:="user definition for OnTrack login users", _
        modulename:=ConstModuleCommons, Version:=1, isbootstrap:=True, usecache:=True, adddeletefieldbehavior:=True)> _
    Public Class User
        Inherits ormBusinessObject
        Implements iormCloneable
        Implements iormInfusable

        '*** Object ID
        Public Const ConstObjectID = "User"
        '*** Schema Table
        <ormTableAttribute(version:=2, usecache:=True)> Public Const ConstPrimaryTableID As String = "tblDefUsers"

        '*** Primary Keys
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Trim}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty},
          XID:="U1", title:="username", description:="name of the OnTrack user")> Public Const ConstFNUsername = "username"

        '*** Fields
        <ormObjectEntry(Datatype:=otDataType.Text, size:=20, properties:={ObjectEntryProperty.Encrypted}, isnullable:=True, _
            validationPropertyStrings:={ObjectValidationProperty.NotEmpty}, _
           XID:="U2", title:="password", description:="password of the OnTrack user")> Public Const ConstFNPassword = "password"

        <ormObjectEntry(referenceobjectentry:=Person.ConstObjectID & "." & Person.constFNID, isnullable:=True, _
         XID:="U4", aliases:={"p1"})> Public Const ConstFNPerson = "person"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=255, _
        XID:="U5", title:="description", description:="description of the OnTrack user")> Public Const ConstFNDescription = "desc"
        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, _
            XID:="U6", title:="is anonymous", description:="is user an anonymous user")> Public Const ConstFNIsAnonymous = "isanon"

        <ormObjectEntry(referenceobjectentry:=Workspace.ConstObjectID & "." & Workspace.ConstFNID, defaultvalue:=ConstGlobalDomain, isnullable:=True, _
            XID:="U10", title:="Default Workspace", description:="default workspace of the OnTrack user")> Public Const ConstFNDefaultWorkspace = "defws"

        <ormObjectEntry(referenceobjectentry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, defaultvalue:=ConstGlobalDomain, isnullable:=True, _
            XID:="U10", title:="Default Domain", description:="default domain of the OnTrack user")> Public Const ConstFNDefaultDomainID = "defdomain"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, _
           XID:="UR1", title:="Alter Schema Right", description:="has user the right to alter the database schema")> _
        Public Const ConstFNAlterSchema = "alterschema"
        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, _
          XID:="UR2", title:="Update Data Right", description:="has user the right to update data (new/change/delete)")> _
        Public Const ConstFNUpdateData = "updatedata"
        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=True, dbdefaultvalue:="1", _
          XID:="UR3", title:="Read Data Right", description:="has user the right to read the database data")> Public Const ConstFNReadData = "readdata"
        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, _
          XID:="UR4", title:="No Access", description:="has user no access")> Public Const ConstFNNoAccess = "noright"

        'overwrite the Domain ID makes no sense
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, _
            defaultvalue:=ConstGlobalDomain, dbdefaultvalue:=ConstGlobalDomain, _
            useforeignkey:=otForeignKeyImplementation.None)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        '** relations
        '* Members
        <ormRelation(cascadeOnDelete:=True, cascadeOnUpdate:=True, cascadeOnCreate:=True, _
            FromEntries:={ConstFNUsername}, toEntries:={GroupMember.ConstFNUsername}, _
            LinkObject:=GetType(GroupMember))> Public Const ConstRelMembers = "members"

        'fields
        <ormObjectEntryMapping(EntryName:=ConstFNUsername)> Private _username As String
        <ormObjectEntryMapping(EntryName:=ConstFNPassword)> Private _password As String
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _desc As String
        <ormObjectEntryMapping(EntryName:=ConstFNPerson)> Private _personID As String

        <ormObjectEntryMapping(EntryName:=ConstFNDefaultWorkspace)> Private _DefaultWorkspace As String
        <ormObjectEntryMapping(EntryName:=ConstFNDefaultDomainID)> Private _DefaultDomainID As String
        <ormObjectEntryMapping(EntryName:=ConstFNIsAnonymous)> Private _isAnonymous As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNReadData)> Private _hasRead As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNUpdateData)> Private _hasUpdate As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNAlterSchema)> Private _hasAlterSchema As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNNoAccess)> Private _hasNoRights As Boolean

        <ormObjectEntryMapping(Relationname:=ConstRelMembers, infusemode:=otInfuseMode.OnDemand)> Private _groupmembers As New List(Of GroupMember)
        ' dynamics
        Private _listtings As New Dictionary(Of String, UserSetting)
        Private _listtingsLoaded As Boolean = False



#Region "Properties"

        Public Property Description() As String
            Get
                Description = _desc
            End Get
            Set(ByVal avalue As String)
                SetValue(entryname:=ConstFNDescription, value:=avalue)
            End Set
        End Property
        ''' <summary>
        ''' returns a list of groups
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property GroupNames() As IEnumerable(Of String)
            Get
                '* infuse the groupmembers
                If (_groupmembers Is Nothing OrElse _groupmembers.Count = 0) And Not CurrentSession.IsBootstrappingInstallationRequested Then
                    MyBase.InfuseRelation(id:=ConstRelMembers)
                End If
                Dim alist As New List(Of String)
                For Each member In _groupmembers
                    If member.IsAlive AndAlso Not alist.Contains(member.GroupName) Then alist.Add(member.GroupName)
                Next
                Return alist
            End Get
            Set(ByVal value As IEnumerable(Of String))
                For Each groupname In value
                    If _groupmembers.FindIndex(Function(x)
                                                   Return x.GroupName = groupname
                                               End Function) < 0 Then
                        Dim aGroupMember As GroupMember = _
                            GroupMember.Create(groupname:=groupname, username:=Me.Username, runtimeOnly:=CurrentSession.IsBootstrappingInstallationRequested)
                        If aGroupMember IsNot Nothing Then _groupmembers.Add(aGroupMember)
                    End If
                Next

            End Set
        End Property
        ''' <summary>
        ''' set or return the default workspace
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DefaultWorkspaceID As String

            Get
                DefaultWorkspaceID = _DefaultWorkspace
            End Get
            Set(value As String)
                SetValue(entryname:=ConstFNDefaultWorkspace, value:=value)
            End Set
        End Property
        ''' <summary>
        ''' set or return the default workspace
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DefaultDomainID As String

            Get
                DefaultDomainID = _DefaultDomainID
            End Get
            Set(value As String)
                SetValue(entryname:=ConstFNDefaultDomainID, value:=value)
            End Set
        End Property
        ''' <summary>
        ''' Password
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Password() As String
            Get
                Password = _password
            End Get
            Set(value As String)
                SetValue(entryname:=ConstFNPassword, value:=value)
            End Set
        End Property
        ''' <summary>
        ''' gets or set the person id
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PersonName() As String
            Get
                PersonName = _personID
            End Get
            Set(value As String)
                SetValue(entryname:=ConstFNPerson, value:=value)
            End Set
        End Property
        ''' <summary>
        ''' gets the ontrack username
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Username() As String
            Get
                Username = _username
            End Get
        End Property
        ''' <summary>
        ''' has no rights at all ?! -> Blocked ?!
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property HasNoRights() As Boolean
            Get
                HasNoRights = _hasNoRights
            End Get
            Set(value As Boolean)
                SetValue(entryname:=ConstFNNoAccess, value:=value)
            End Set
        End Property
        ''' <summary>
        ''' has right to read
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property HasReadRights() As Boolean
            Get
                HasReadRights = _hasRead
            End Get
            Set(value As Boolean)
                SetValue(entryname:=ConstFNReadData, value:=value)
            End Set
        End Property
        ''' <summary>
        ''' has right to update and read data
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property HasUpdateRights() As Boolean
            Get
                HasUpdateRights = _hasUpdate
            End Get
            Set(value As Boolean)
                SetValue(entryname:=ConstFNUpdateData, value:=value)
            End Set
        End Property
        ''' <summary>
        ''' Has Right to update, read and alter schema data
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property HasAlterSchemaRights() As Boolean
            Get
                HasAlterSchemaRights = _hasAlterSchema
            End Get
            Set(value As Boolean)
                SetValue(entryname:=ConstFNAlterSchema, value:=value)
            End Set
        End Property
        ''' <summary>
        ''' is anonymous user
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsAnonymous() As Boolean
            Get
                Return _isAnonymous
            End Get
            Set(value As Boolean)
                SetValue(entryname:=ConstFNIsAnonymous, value:=value)
            End Set
        End Property

        ''' <summary>
        ''' gets the accessright out of the setting
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AccessRight As otAccessRight
            Get
                '* highes right first
                If Me.HasAlterSchemaRights Then
                    Return otAccessRight.AlterSchema
                ElseIf Me.HasUpdateRights Then
                    Return otAccessRight.ReadUpdateData
                ElseIf Me.HasReadRights Then
                    Return otAccessRight.ReadOnly
                End If

                Return otAccessRight.Prohibited
            End Get
            Set(value As otAccessRight)
                Select Case value
                    Case otAccessRight.AlterSchema
                        Me.HasAlterSchemaRights = True
                        Me.HasUpdateRights = True
                        Me.HasReadRights = True
                        Me.HasNoRights = False
                    Case otAccessRight.ReadUpdateData
                        Me.HasAlterSchemaRights = False
                        Me.HasUpdateRights = True
                        Me.HasReadRights = True
                        Me.HasNoRights = False
                    Case otAccessRight.ReadOnly
                        Me.HasAlterSchemaRights = False
                        Me.HasUpdateRights = False
                        Me.HasReadRights = True
                        Me.HasNoRights = False
                    Case otAccessRight.Prohibited
                        Me.HasAlterSchemaRights = False
                        Me.HasUpdateRights = False
                        Me.HasReadRights = False
                        Me.HasNoRights = True
                    Case Else
                        CoreMessageHandler(message:="access right not implemented", argument:=value, procedure:="User.AccessRight", messagetype:=otCoreMessageType.InternalError)

                End Select

            End Set
        End Property
#End Region
        ''' <summary>
        ''' returns a SQL String to insert the Admin User in the table -> bootstrap
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetInsertInitalUserSQLString(username As String, person As String, password As String, desc As String, group As String, defaultworkspace As String) As String

            Dim aSqlString As String = String.Format("INSERT INTO [{0}] ", CurrentSession.OTDBDriver.GetNativeDBObjectName(ConstPrimaryTableID))
            'aSqlString &= "( [username], person, [password], [desc],  defws, isanon, alterschema, readdata, updatedata, noright, UpdatedOn, CreatedOn)"
            aSqlString &= String.Format("( [{0}], [{1}], [{2}], [{3}],  [{4}], [{5}], [{6}], [{7}], {8}, [{9}], [{10}], [{11}], [{12}])", _
                                         ConstFNUsername, ConstFNPerson, ConstFNPassword, ConstFNDescription, ConstFNDefaultWorkspace, ConstFNDefaultDomainID, _
                                         ConstFNIsAnonymous, ConstFNAlterSchema, ConstFNReadData, ConstFNUpdateData, ConstFNNoAccess, ConstFNCreatedOn, ConstFNUpdatedOn)
            aSqlString &= String.Format("VALUES ('{0}','{1}', '{2}', '{3}',  '{4}','{5}', 0, 1,1,1,0, '{6}','{7}' )", _
                                        username, person, password, desc, defaultworkspace, _
                                        ConstGlobalDomain, Date.Now.ToString("yyyyMMdd hh:mm:ss"), Date.Now.ToString("yyyyMMdd hh:mm:ss"))
            Return aSqlString

        End Function

        ''' <summary>
        ''' returns a SQL String to create the table on bootstrapping
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetCreateSqlString() As String

            Dim aSqlString As String = String.Format("CREATE TABLE [{0}] ", CurrentSession.OTDBDriver.GetNativeDBObjectName(ConstPrimaryTableID))
            aSqlString &= String.Format("( [{0}] nvarchar(50) not null, [{1}] nvarchar(50)  null, [{2}] nvarchar(50)  null, ", _
                                        ConstFNUsername, ConstFNPassword, ConstFNPerson)
            aSqlString &= String.Format("[{0}] nvarchar(max)  null default , [{1}] nvarchar(max)  null default, [{2}] bit not null default 0, [{3}] bit not null default 0, [{4}] bit not null default 0, [{5}] bit not null default 0, ", _
                                        ConstFNDefaultWorkspace, ConstFNDefaultDomainID, ConstFNAlterSchema, ConstFNUpdateData, ConstFNReadData, ConstFNNoAccess)
            aSqlString &= String.Format(" [{0}] nvarchar(max)  null default , [{1}] DATETIME not null , [{2}] Datetime not null , " & _
                                                "CONSTRAINT [{3}_primarykey] PRIMARY KEY NONCLUSTERED ([{3}] Asc) ", _
                                                ConstFNDescription, ConstFNUpdatedOn, ConstFNCreatedOn, ConstFNUsername, ConstPrimaryTableID)
            aSqlString &= "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY];"

            Return aSqlString
            '*** LEGACY working
            'Return "create table " & Me.TableID & _
            '                 " ( username nvarchar(50) not null, [password] nvarchar(50) not null, [person] nvarchar(50) not null, [group] nvarchar(50) not null, " & _
            '                 "defws nvarchar(max) not null default '', " & _
            '                 "isanon bit not null default 0, alterschema bit not null default 0, updatedata bit not null default 0, noright bit not null default 0," & _
            '                 "readdata bit not null default 1," & _
            '                 " [desc] nvarchar(max) not null default '', UpdatedOn DATETIME not null , CreatedOn Datetime not null , " & _
            '                 "CONSTRAINT [tblDefUsers_primarykey] PRIMARY KEY NONCLUSTERED ([username] Asc) " & _
            '                 "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" & _
            '                 ") ON [PRIMARY];"
        End Function
        ''' <summary>
        ''' Returns a collection of all objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function All() As List(Of User)
            Return ormBusinessObject.AllDataObject(Of User)(orderby:=ConstFNUsername)
        End Function

        '****** getAnonymous: "static" function to return the first Anonymous user
        '******
        ''' <summary>
        ''' returns the anonyous user ( first descending username)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetAnonymous() As OnTrack.Commons.User
            Dim aObjectCollection As List(Of User)
            If CurrentSession.OTDBDriver.Name = ConstCPVDriverSQLServer Then
                aObjectCollection = ormBusinessObject.AllDataObject(Of User)(orderby:=ConstFNUsername, where:=ConstFNIsAnonymous & "=1")
            Else
                aObjectCollection = ormBusinessObject.AllDataObject(Of User)(orderby:=ConstFNUsername, where:=ConstFNIsAnonymous & "=true")
            End If

            If aObjectCollection.Count = 0 Then
                Return Nothing
            Else
                Return aObjectCollection.Item(1)
            End If

        End Function

        ''' <summary>
        ''' Retrieve a User Definition
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal username As String, Optional forcereload As Boolean = False) As User
            Return RetrieveDataObject(Of User)(pkArray:={username}, forceReload:=forcereload)
        End Function
        ''' <summary>
        ''' Returns a list of groupdefinition this belongs to
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetGroups() As List(Of Group)
            If Not Me.IsAlive(subname:="getgroup") Then Return New List(Of Group)
            Dim alist As New List(Of Group)
            '* infuse the groupmembers
            If (_groupmembers Is Nothing OrElse _groupmembers.Count = 0) And Not CurrentSession.IsBootstrappingInstallationRequested Then
                MyBase.InfuseRelation(id:=ConstRelMembers)
            End If
            '' add all the group definitions
            For Each member In _groupmembers
                If alist.FindIndex(Function(x)
                                       Return x.GroupName = member.GroupName
                                   End Function) < 0 Then
                    Dim aGroup As Group = member.GetGroup
                    If aGroup IsNot Nothing Then alist.Add(aGroup)
                End If
            Next
            Return alist
        End Function
        ''' <summary>
        ''' returns true if the setting exists
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasSetting(id As String) As Boolean
            LoadSettings() ' load since we might no have it during bootstrap
            Return _listtings.ContainsKey(key:=id)
        End Function
        ''' <summary>
        ''' returns the setting valid in the domain
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSetting(id As String) As Object
            LoadSettings()
            If Me.HasSetting(id:=id) Then
                Return _listtings.Item(key:=id)
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' sets the setting valid in the domain
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetSetting(id As String, datatype As otDataType, value As Object) As Boolean
            Dim aSetting As New UserSetting
            LoadSettings()
            If Me.HasSetting(id:=id) Then
                aSetting = Me.GetSetting(id:=id)
            Else
                aSetting = UserSetting.Create(username:=Me.Username, id:=id)
                If aSetting Is Nothing Then aSetting = UserSetting.Retrieve(Username:=Me.Username, id:=id)
            End If

            If aSetting Is Nothing OrElse Not (aSetting.IsLoaded Or aSetting.IsCreated) Then
                Return False
            End If
            aSetting.Datatype = datatype
            aSetting.Value = value

            If Not Me.HasSetting(id:=id) Then _listtings.Add(key:=id, value:=aSetting)
            Return True
        End Function
        ''' <summary>
        ''' Load the settings to the settings dictionary
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadSettings(Optional force As Boolean = False) As Boolean

            If _listtingsLoaded And Not force Then Return True

            Dim aListDomain As List(Of UserSetting) = UserSetting.RetrieveByUsername(Username:=Me.Username)

            '** overwrite
            For Each aSetting In aListDomain
                If _listtings.ContainsKey(key:=aSetting.ID) Then
                    _listtings.Remove(key:=aSetting.ID)
                End If
                _listtings.Add(key:=aSetting.ID, value:=aSetting)
            Next

            _listtingsLoaded = False
            Return True
        End Function

        ''' <summary>
        ''' create the persistency schema with use of database driver
        ''' ATTENTION ! This can only be called if database is set up
        ''' user createSql function otherwise
        ''' </summary>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateSchema(Optional silent As Boolean = True) As Boolean
            Return ormBusinessObject.CreateDataObjectSchema(Of User)(silent:=silent)
        End Function

        ''' <summary>
        ''' Create persistency for this object
        ''' </summary>
        ''' <param name="username"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal username As String) As User
            Dim primarykey() As Object = {username}
            Return ormBusinessObject.CreateDataObject(Of User)(primarykey, checkUnique:=True)
        End Function

    End Class
    ''' <summary>
    ''' User Setting Definition Class
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=UserSetting.ConstObjectID, description:="properties per user", _
        modulename:=ConstModuleCommons, Version:=1, useCache:=True)> Public Class UserSetting
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable

        Public Const ConstObjectID = "UserSetting"
        '** Table Schema
        <ormTableAttribute(adddeletefieldbehavior:=True, Version:=1)> Public Const ConstPrimaryTableID As String = "tblDefUserSettings"

        '** Primary Key
        <ormObjectEntry(XID:="US1", referenceobjectentry:=User.ConstObjectID & "." & User.ConstFNUsername, PrimaryKeyOrdinal:=1)> _
        Const ConstFNUsername As String = User.ConstFNUsername

        '** Fields
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, PrimaryKeyOrdinal:=2, _
                    validationPropertyStrings:={ObjectValidationProperty.NotEmpty}, _
           XID:="US2", title:="Setting", description:="ID of the setting per user")> Const ConstFNSettingID = "id"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, _
            XID:="US3", title:="Description")> Const ConstFNDescription = "desc"

        <ormObjectEntry(Datatype:=otDataType.Memo, _
           XID:="US4", title:="value", description:="value of the user setting in string presentation")> Const ConstFNValue = "value"

        <ormObjectEntry(Datatype:=otDataType.Long, defaultvalue:=otDataType.Text, _
          XID:="US5", title:="datatype", description:="data type of the user setting value")> Const ConstFNDatatype = "datatype"

        ' fields
        <ormObjectEntryMapping(EntryName:=ConstFNUsername)> Private _Username As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNSettingID)> Private _ID As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _description As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNValue)> Private _valuestring As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNDatatype)> Private _datatype As otDataType = 0


#Region "Properties"
        ''' <summary>
        ''' gets the ID of the Domain
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Username() As String
            Get
                Username = _Username
            End Get

        End Property
        ''' <summary>
        ''' gets the ID of the Setting
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ID() As String
            Get
                ID = _ID
            End Get

        End Property
        ''' <summary>
        ''' Description of the setting
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description() As String
            Get
                Description = _description
            End Get
            Set(value As String)
                _description = value
                Me.IsChanged = True
            End Set
        End Property
        ''' <summary>
        ''' returns the datatype 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Datatype As otDataType
            Set(value As otDataType)
                _datatype = value
            End Set
            Get
                Return _datatype
            End Get
        End Property
        ''' <summary>
        ''' gets or sets the value of the domain setting
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Value As Object
            Set(value As Object)
                If value Is Nothing Then
                    _valuestring = String.Empty
                Else
                    _valuestring = value.ToString
                End If
            End Set
            Get
                Try
                    If String.IsNullOrEmpty(_valuestring) Then Return Core.DataType.GetDefaultValue(_datatype)
                    Return Core.DataType.To(_valuestring, _datatype)
                Catch ex As Exception
                    CoreMessageHandler(exception:=ex, message:="could not convert value to data type " & _datatype, _
                                       argument:=_valuestring, procedure:="DomainSetting.value", messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End Try

            End Get
        End Property
#End Region



        ''' <summary>
        ''' Retrieve the workspaceID Cache Object
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal Username As String, ByVal id As String, Optional forcereload As Boolean = False) As UserSetting
            Dim pkarray() As Object = {UCase(Username), UCase(id)}
            Return RetrieveDataObject(Of UserSetting)(pkArray:=pkarray, forceReload:=forcereload)
        End Function
        ''' <summary>
        ''' Retrieve the workspaceID Cache Object
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function RetrieveByUsername(ByVal Username As String, Optional forcereload As Boolean = False) As List(Of UserSetting)
            Dim aParameterslist As New List(Of ormSqlCommandParameter)
            aParameterslist.Add(New ormSqlCommandParameter(ID:="@Username", columnname:=ConstFNUsername, value:=Username))

            Dim aList As List(Of UserSetting) = ormBusinessObject.AllDataObject(Of UserSetting)(ID:="allby" & Username, where:=ConstFNUsername & "= @Username", _
                                                                                      parameters:=aParameterslist)
            Return aList
        End Function

        ''' <summary>
        ''' creates with this object a new persistable Def workspaceID
        ''' </summary>
        ''' <param name="workspaceID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal username As String, ByVal id As String) As UserSetting
            Dim primarykey() As Object = {UCase(username), UCase(id)}
            Return ormBusinessObject.CreateDataObject(Of UserSetting)(primarykey, checkUnique:=False)

        End Function

    End Class


    ''' <summary>
    ''' the person definition class
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=Person.ConstObjectID, modulename:=ConstModuleCommons, description:="person definition", _
        usecache:=True, Version:=1, addDomainBehavior:=True, adddeletefieldbehavior:=True)> Public Class Person
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable

        '** Object ID
        Public Const ConstObjectID = "Person"
        '** Table
        <ormTableAttribute(version:=2, usecache:=True)> Public Const ConstPrimaryTableID As String = "tblDefPersons"

        '** primary keys
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty},
            XID:="P1", title:="ID", description:="ID of the person")> Public Const constFNID = "id"
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2 _
         , useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        '** fields
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, _
          XID:="P2", title:="First Name", description:="first name of the person")> Public Const constFNFirstName = "firstname"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=255, _
         XID:="P3", title:="Middle Names", description:="mid names of the person")> Public Const constFNMidNames = "midnames"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, _
         XID:="P4", title:="Sir Name", description:="sir name of the person")> Public Const constFNSirName = "sirname"
        <ormObjectEntry(Datatype:=otDataType.Memo, _
           XID:="P5", title:="Description", description:="description of the person")> Public Const constFNDescription = "desc"
        <ormObjectEntry(Datatype:=otDataType.Bool, _
           XID:="P6", title:="Role", description:="set if the person is a role")> Public Const ConstFNIsRole = "isrole"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, _
        XID:="P13", title:="Company Name", description:="name of the persons company")> Public Const constFNCompany = "company"
        <ormObjectEntry(referenceObjectEntry:=ConstObjectID & "." & constFNID, XID:="P7", Title:="superior ID", description:="ID of the superior manager")> _
        Public Const ConstFNManager = "superid"
        <ormObjectEntry(referenceObjectEntry:=OrgUnit.ConstObjectID & "." & OrgUnit.ConstFNID, _
            XID:="P8")> Public Const ConstFNOrgUnit = "orgunit"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=255, _
          XID:="P9", title:="eMail", description:="eMail Address of the person")> Public Const constFNeMail = "email"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, _
         XID:="P10", title:="phone", description:="phone of the person")> Public Const constFNPhone = "phone"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, _
         XID:="P11", title:="phone", description:="mobile of the person")> Public Const constFNMobile = "mobile"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, _
         XID:="P12", title:="phone", description:="fax of the person")> Public Const constFNFax = "fax"

        ' field mapping
        <ormObjectEntryMapping(EntryName:=constFNID)> Private _id As String = String.Empty
        <ormObjectEntryMapping(EntryName:=constFNFirstName)> Private _firstname As String = String.Empty
        <ormObjectEntryMapping(EntryName:=constFNMidNames)> Private _midnames As String = String.Empty
        <ormObjectEntryMapping(EntryName:=constFNSirName)> Private _sirname As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNIsRole)> Private _isrole As Boolean = False
        <ormObjectEntryMapping(EntryName:=constFNDescription)> Private _description As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNManager)> Private _managerid As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNOrgUnit)> Private _orgunitID As String = String.Empty
        <ormObjectEntryMapping(EntryName:=constFNCompany)> Private _companyID As String = String.Empty
        <ormObjectEntryMapping(EntryName:=constFNeMail)> Private _emailaddy As String = String.Empty
        <ormObjectEntryMapping(EntryName:=constFNPhone)> Private _phone As String = String.Empty
        <ormObjectEntryMapping(EntryName:=constFNMobile)> Private _mobile As String = String.Empty
        <ormObjectEntryMapping(EntryName:=constFNFax)> Private _fax As String = String.Empty


#Region "Properties"
        ''' <summary>
        ''' returns the ID of the Person
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ID() As String
            Get
                ID = _id
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the firstname.
        ''' </summary>
        ''' <value>The firstname.</value>
        Public Property Firstname() As String
            Get
                Return Me._firstname
            End Get
            Set(value As String)
                If _firstname.ToLower <> value.ToLower Then
                    Dim pattern As String = "\b(\w|['-])+\b"
                    ' With lambda support:
                    Dim result As String = Regex.Replace(value.ToLower, pattern, _
                        Function(m) m.Value(0).ToString().ToUpper() & m.Value.Substring(1))
                    Me._firstname = result
                    IsChanged = True
                End If
            End Set
        End Property
        ''' <summary>
        ''' sets or gets the midnames
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Midnames() As String()
            Get
                Return Core.DataType.ToArray(_midnames)
            End Get
            Set(avalue As String())
                If Not Array.Equals(avalue, _midnames) Then
                    Dim pattern As String = "\b(\w|['-])+\b"
                    ' With lambda support:
                    Dim result As String = Regex.Replace(LCase(Core.DataType.ToString(avalue)), pattern, _
                        Function(m) m.Value(0).ToString().ToUpper() & m.Value.Substring(1))
                    _midnames = result
                    IsChanged = True
                End If
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the Sirname.
        ''' </summary>
        ''' <value>The sirname.</value>
        Public Property Sirname() As String
            Get
                Return Me._sirname
            End Get
            Set(value As String)
                If _sirname.ToLower <> value.ToLower Then
                    Dim pattern As String = "\b(\w|['-])+\b"
                    ' With lambda support:
                    Dim result As String = Regex.Replace(value.ToLower, pattern, _
                        Function(m) m.Value(0).ToString().ToUpper() & m.Value.Substring(1))
                    _sirname = result
                    IsChanged = True
                End If
            End Set
        End Property
        ''' <summary>
        ''' returns the description of the person
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description() As String
            Get
                Description = _description
            End Get
            Set(value As String)
                _description = value
                IsChanged = True
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the role flag
        ''' </summary>
        ''' <value></value>
        Public Property IsRole() As Boolean
            Get
                Return Me._isrole
            End Get
            Set(value As Boolean)
                If _isrole <> value Then
                    Me._isrole = value
                    IsChanged = True
                End If
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the company ID.
        ''' </summary>
        ''' <value>The company name.</value>
        Public Property Company() As String
            Get
                Return Me._companyID
            End Get
            Set(value As String)
                If _companyID.ToLower <> value.ToLower Then
                    Me._companyID = value
                    IsChanged = True
                End If
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the ManagerID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ManagerID() As String
            Get
                ManagerID = _managerid
            End Get
            Set(value As String)
                If ManagerID.ToLower <> value.ToLower Then
                    _managerid = value
                    IsChanged = True
                End If
            End Set
        End Property
        ''' <summary>
        ''' sets or gets the Organization Unit ID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Property OrgUnitID() As String
            Get
                OrgUnitID = _orgunitID
            End Get
            Set(value As String)
                If _orgunitID.ToLower <> value.ToLower Then
                    _orgunitID = value
                    IsChanged = True
                End If
            End Set
        End Property
        ''' <summary>
        ''' sets or gets the Organization Unit 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Property OrgUnit() As OrgUnit
            Get
                Return OrgUnit.Retrieve(id:=_orgunitID)
            End Get
            Set(value As OrgUnit)
                If _orgunitID.ToLower <> value.ID.ToLower Then
                    _orgunitID = value.ID
                    IsChanged = True
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the email address 
        ''' </summary>
        ''' <value>The company name.</value>
        Public Property eMail() As String
            Get
                Return Me._emailaddy
            End Get
            Set(value As String)
                If _emailaddy.ToLower <> value.ToLower Then
                    Me._emailaddy = LCase(Trim(value))
                    IsChanged = True
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the Phone number 
        ''' </summary>
        ''' <value>The company name.</value>
        Public Property Phone() As String
            Get
                Return Me._phone
            End Get
            Set(value As String)
                If _phone.ToLower <> value.ToLower Then
                    Me._phone = LCase(Trim(value))
                    IsChanged = True
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the email address 
        ''' </summary>
        ''' <value>The company name.</value>
        Public Property Fax() As String
            Get
                Return Me._fax
            End Get
            Set(value As String)
                If _fax.ToLower <> value.ToLower Then
                    Me._fax = LCase(Trim(value))
                    IsChanged = True
                End If
            End Set
        End Property
#End Region
        ''' <summary>
        ''' loads the persistence object with ID from the parameters
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal firstname As String, ByVal midnames As String(), ByVal sirname As String, Optional domainid As String = Nothing) As Person
            Return Retrieve(id:=BuildID(firstname:=firstname, midnames:=midnames, sirname:=sirname), domainid:=domainid)
        End Function
        ''' <summary>
        ''' Retrieve
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal id As String, Optional domainid As String = Nothing, Optional forcereload As Boolean = False) As Person
            Dim primarykey() As Object = {id, domainid}
            Return RetrieveDataObject(Of Person)(pkArray:=primarykey, domainID:=domainid, forceReload:=forcereload)
        End Function

        ''' <summary>
        ''' returns a collection of all Person Definition Objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function All(Optional domainid As String = Nothing) As List(Of Person)
            Return ormBusinessObject.AllDataObject(Of Person)(domainid:=domainid)
        End Function

        ''' <summary>
        ''' build the ID string out of the names
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function BuildID(ByVal firstname As String, ByVal sirname As String, Optional ByVal midnames As String() = Nothing) As String
            Dim pattern As String = "\b(\w|['-])+\b"
            Dim midnamesS As String = String.Empty
            ' With lambda support:
            firstname = Regex.Replace(firstname.ToLower, pattern, Function(m) m.Value(0).ToString().ToUpper() & m.Value.Substring(1))
            sirname = Regex.Replace(firstname.ToLower, pattern, Function(m) m.Value(0).ToString().ToUpper() & m.Value.Substring(1))
            If midnames IsNot Nothing Then midnamesS = Regex.Replace(LCase(Core.DataType.ToString(midnames)), pattern, Function(m) m.Value(0).ToString().ToUpper() & m.Value.Substring(1))

            If midnamesS <> String.Empty Then
                Return sirname & ", " & firstname & " (" & midnamesS & ")"
            Else
                Return sirname & ", " & firstname
            End If
        End Function
        ''' <summary>
        ''' Creates the persistence object
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal id As String, Optional domainid As String = Nothing) As Person
            Dim primarykey() As Object = {id, domainid}
            ' set the primaryKey
            Return ormBusinessObject.CreateDataObject(Of Person)(primarykey, domainID:=domainid, checkUnique:=True)
        End Function
        ''' <summary>
        ''' Creates the persistence object with ID from the parameters
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal firstname As String, ByVal sirname As String, Optional ByVal midnames As String() = Nothing, Optional domainid As String = Nothing) As Person
            Return Create(id:=BuildID(firstname:=firstname, midnames:=midnames, sirname:=sirname), domainid:=domainid)
        End Function
    End Class


    ''' <summary>
    ''' Object Message Definition Class - bound messages to a buisiness object
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(ID:=ObjectMessageType.ConstObjectID, Modulename:=ConstModuleCommons, _
        usecache:=True, adddomainbehavior:=True, adddeletefieldbehavior:=True, _
        Description:="message definitions for object messages")> _
    Public Class ObjectMessageType
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable

        Public Const ConstObjectID = "OBJECTMESSAGETYPE"
        ''' <summary>
        ''' Table
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=1, usecache:=True, addDomainBehavior:=True)> Public Const ConstPrimaryTableID As String = "TBLDEFOBJECTMESSAGES"

        ''' <summary>
        ''' Primary Keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=1, _
           XID:="omd1", title:="UID", description:="unique identifier of the object message")> Public Const ConstFNUID = "UID"
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2 _
        , useforeignkey:=otForeignKeyImplementation.NativeDatabase, defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        ''' <summary>
        ''' Columns
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, isnullable:=True, _
          XID:="omd2", title:="Area", description:="area of the object message")> Public Const constFNArea = "AREA"
        <ormObjectEntry(Datatype:=otDataType.Numeric, isnullable:=True, _
        XID:="omd3", title:="Weight", description:="weight of the object message")> Public Const constFNWeight = "WEIGHT"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=20, dbdefaultvalue:="Info", defaultvalue:=otObjectMessageType.Info, _
        XID:="omd4", title:="Type", description:="type of the object message")> Public Const constFNType = "TYPEID"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=1024, _
        XID:="omd5", title:="Text", description:="message text of the object message")> Public Const constFNText = "MESSAGE"
        <ormObjectEntry(Datatype:=otDataType.Memo, isnullable:=True,
        XID:="omd6", title:="Description", description:="additional description and help text of the object message")> Public Const constFNDescription = "DESCRIPTION"
        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0", _
        XID:="omd7", title:="Persisted", description:="true if the messages of this type will be persisted in the log")> Public Const constFNIsPersisted = "ISPERSISTED"

        <ormObjectEntry(Datatype:=otDataType.List, _
        XID:="omd10", isnullable:=True, title:="Status Types ", description:="resulting status types of the object message")> Public Const constFNSStatusTypes = "statustypes"
        <ormObjectEntry(Datatype:=otDataType.List, _
        XID:="omd20", isnullable:=True, title:="Status Codes ", description:="resulting status codes of the object message")> Public Const constFNSStatusCodes = "statusCodes"

        ''' <summary>
        ''' column field mapping
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNUID)> Private _uid As Long
        <ormObjectEntryMapping(EntryName:=constFNWeight)> Private _weight As Double?
        <ormObjectEntryMapping(EntryName:=constFNArea)> Private _area As String
        <ormObjectEntryMapping(EntryName:=constFNType)> Private _type As otObjectMessageType
        <ormObjectEntryMapping(EntryName:=constFNText)> Private _message As String
        <ormObjectEntryMapping(EntryName:=constFNDescription)> Private _desc As String
        <ormObjectEntryMapping(EntryName:=constFNIsPersisted)> Private _IsPersisted As Boolean
        <ormObjectEntryMapping(EntryName:=constFNSStatusTypes)> Private _statustypes As String()
        <ormObjectEntryMapping(EntryName:=constFNSStatusCodes)> Private _statuscodes As String()

        ''' dynamic
        ''' 
        Private _StatusCodeDictionary As New Dictionary(Of String, String)

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the is persisted flag for the messages of this type.
        ''' </summary>
        ''' <value>The is persisted.</value>
        Public Property IsPersisted() As Boolean
            Get
                Return Me._IsPersisted
            End Get
            Set(value As Boolean)
                SetValue(constFNIsPersisted, value)
            End Set
        End Property

        ''' <summary>
        ''' get the UID of the message
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ID() As Long
            Get
                Return _uid
            End Get
        End Property

        ''' <summary>
        ''' sets or gets the message text 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Message As String
            Get
                Message = _message
            End Get
            Set(value As String)
                SetValue(constFNText, value)
            End Set
        End Property

        ''' <summary>
        ''' get or sets the weight of the message from 0 ... 100
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Property Weight As Double?
            Get
                Return _weight
            End Get
            Set(avalue As Double?)
                SetValue(constFNWeight, value:=avalue)
            End Set
        End Property

        ''' <summary>
        ''' set or gets the type of the message
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Property [Type] As otObjectMessageType
            Get
                Return _type
            End Get
            Set(avalue As otObjectMessageType)
                SetValue(constFNType, avalue)
            End Set
        End Property

        ''' <summary>
        ''' sets or gets the message area category
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Area() As String
            Get
                Return _area
            End Get
            Set(ByVal avalue As String)
                SetValue(constFNArea, avalue)
            End Set
        End Property

        ''' <summary>
        ''' returns a IList of StatusItems of this MessageType
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property StatusItems(Optional domainid As String = Nothing, Optional statustype As String = Nothing) As IList(Of StatusItem)
            Get
                Dim aList As New List(Of StatusItem)
                If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
                For Each aPair In _StatusCodeDictionary
                    Dim aCode As String = aPair.Value
                    Dim aType As String = aPair.Key
                    If statustype Is Nothing OrElse statustype.ToUpper = aType.ToUpper Then
                        Dim aStatusItem As StatusItem = StatusItem.Retrieve(typeid:=aType, code:=aCode, domainid:=domainid)
                        If aStatusItem IsNot Nothing Then
                            aList.Add(aStatusItem)
                        End If
                    End If
                Next
                Return aList
            End Get
        End Property

        ''' <summary>
        ''' sets or gets the Status code of a status type item
        ''' </summary>
        ''' <param name="typeid"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property StatusCodeOf(ByVal typeid As String) As String
            Get
                If Not Me.IsAlive("GetStatusCodeOf") Then Return Nothing

                If _StatusCodeDictionary.ContainsKey(key:=typeid.ToUpper) Then
                    Return _StatusCodeDictionary.Item(key:=typeid.ToUpper)
                End If
                Return Nothing
            End Get
            Set(value As String)
                If Not Me.IsAlive("GetStatusCodeOf") Then Return

                If _StatusCodeDictionary.ContainsKey(key:=typeid.ToUpper) Then
                    _StatusCodeDictionary.Remove(key:=typeid.ToUpper)
                End If
                _StatusCodeDictionary.Add(key:=typeid.ToUpper, value:=value)
            End Set
        End Property

        ''' <summary>
        ''' returns a List of statustypes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property StatusTypes As IList(Of String)
            Get
                Return _StatusCodeDictionary.Keys.ToList
            End Get
        End Property
#End Region




        ''' <summary>
        ''' returns a Object Log Message Definition Object from the data store
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Retrieve(ByVal uid As Long, Optional domainid As String = Nothing) As ObjectMessageType
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey() As Object = {uid, domainid}
            Return ormBusinessObject.RetrieveDataObject(Of ObjectMessageType)(pkArray:=primarykey)
        End Function


        ''' <summary>
        ''' return all Log Message Definitions
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function All(Optional domainid As String = Nothing) As List(Of ObjectMessageType)
            Return ormBusinessObject.AllDataObject(Of ObjectMessageType)(domainid:=domainid)
        End Function


        ''' <summary>
        ''' Create a persistable Log Message
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal uid As Long, Optional ByVal domainid As String = Nothing) As ObjectMessageType
            Dim primarykey() As Object = {uid}
            ' set the primaryKey
            Return ormBusinessObject.CreateDataObject(Of ObjectMessageType)(primarykey, domainID:=domainid, checkUnique:=True)
        End Function


        ''' <summary>
        ''' handler for the record feed event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectMessageType_OnFed(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnFed
            '''
            ''' convert the dictionary to the key-value string arrays
            ''' 
            Dim codes As String = e.Record.GetValue(constFNSStatusCodes)
            Dim types As String = e.Record.GetValue(constFNSStatusTypes)
            e.Record.SetValue(constFNSStatusCodes, Core.DataType.ToString(_StatusCodeDictionary.Values.ToArray))
            e.Record.SetValue(constFNSStatusTypes, Core.DataType.ToString(_StatusCodeDictionary.Keys.ToArray))
        End Sub

        ''' <summary>
        ''' On Infused Handler
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectMessageType_OnInfused(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnInfused

            ''' get all the status types and set the codes
            ''' 
            If _statuscodes IsNot Nothing AndAlso _statustypes IsNot Nothing AndAlso _statuscodes.Count <> _statustypes.Count Then
                CoreMessageHandler(message:="statustypes and statuscodes differ in length ?!", messagetype:=otCoreMessageType.ApplicationError, _
                                   procedure:="ObjectMessageType.OnInfused")
            ElseIf (_statuscodes Is Nothing AndAlso _statustypes IsNot Nothing) OrElse (_statuscodes IsNot Nothing AndAlso _statustypes Is Nothing) Then
                CoreMessageHandler(message:="statustypes and statuscodes differ in length ?!", messagetype:=otCoreMessageType.ApplicationError, _
                                  procedure:="ObjectMessageType.OnInfused")
            ElseIf _statuscodes IsNot Nothing AndAlso _statustypes IsNot Nothing Then
                For I = 0 To _statustypes.GetUpperBound(0)
                    If I <= _statuscodes.GetUpperBound(0) Then
                        If _StatusCodeDictionary.ContainsKey(_statustypes(I)) Then
                            _StatusCodeDictionary.Remove(key:=_statustypes(I))
                        End If
                        _StatusCodeDictionary.Add(key:=_statustypes(I), value:=_statuscodes(I))
                    End If
                Next
            End If

        End Sub

        ''' <summary>
        ''' on property change
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectMessageType_PropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs) Handles Me.PropertyChanged
            If e.PropertyName = constFNSStatusCodes OrElse e.PropertyName = constFNSStatusTypes Then
                ''' only rebuild if possible
                If _statuscodes IsNot Nothing AndAlso _statustypes IsNot Nothing AndAlso _statuscodes.Count = _statustypes.Count Then
                    '* rebuild the dictionary
                    For I = 0 To _statustypes.GetUpperBound(0)
                        If I <= _statuscodes.GetUpperBound(0) Then
                            If _StatusCodeDictionary.ContainsKey(_statustypes(I)) Then
                                _StatusCodeDictionary.Remove(key:=_statustypes(I))
                            End If
                            _StatusCodeDictionary.Add(key:=_statustypes(I), value:=_statuscodes(I))
                        End If
                    Next
                End If
            End If
        End Sub
    End Class

    ''' <summary>
    ''' Status Type Class 
    ''' </summary>
    ''' <remarks>
    '''  Design Principle:
    ''' 
    ''' 1. Create or Add or Update Items by StatusType Object
    ''' </remarks>
    ''' 
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleCommons, Version:=2, Release:=0, patch:=0, changeimplno:=3, _
         description:="Introducing Status Type object")> _
   <ormObject(id:=StatusType.ConstObjectID, description:="status type definitions", _
       modulename:=ConstModuleCommons, Version:=1, usecache:=True, adddomainbehavior:=True, adddeletefieldbehavior:=True, isbootstrap:=False)> _
    Public Class StatusType
        Inherits ormBusinessObject

        '*** Object ID
        Public Const ConstObjectID = "StatusType"

        ''' <summary>
        ''' Table Definition
        ''' </summary>
        ''' <remarks></remarks>
        <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleRepository, Version:=ConstOTDBSchemaVersion, Release:=0, patch:=0, changeimplno:=6, _
        description:="added table " & ConstPrimaryTableID)> _
        <ormTableAttribute(version:=1, usecache:=True)> Public Const ConstPrimaryTableID As String = "TBLDEFSTATUSTYPES"

        ''' <summary>
        ''' Primary Keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty},
          XID:="ST1", title:="Status Type", description:="name of the status type")> Public Const ConstFNID = "ID"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2, _
                       defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        ''' <summary>
        ''' Entries
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, size:=255, _
        XID:="ST10", title:="description", description:="description of the status type")> Public Const ConstFNDescription = "DESC"



        '* Relations
        '* Members
        <ormRelation(cascadeOnDelete:=True, cascadeonUpdate:=True, FromEntries:={ConstFNID}, toEntries:={StatusItem.constFNType}, _
            LinkObject:=GetType(StatusItem))> Const ConstRelItems = "ITEMS"

        <ormObjectEntryMapping(Relationname:=ConstRelItems, infusemode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand, _
        keyentries:={StatusItem.constFNType})> Private WithEvents _items As New ormRelationCollection(Of StatusItem)(Me, {StatusItem.constFNType})

        'fields
        <ormObjectEntryMapping(EntryName:=ConstFNID)> Private _id As String
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _desc As String




#Region "Properties"

        ''' <summary>
        ''' returns the ID / name of the status type
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
        ''' returns the description of the type
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
        ReadOnly Property Items As ormRelationCollection(Of StatusItem)
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

        Private Sub ValuesCollection_OnAdded(sender As Object, e As Database.ormRelationCollection(Of StatusItem).EventArgs) Handles _items.OnAdded
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
        Public Overloads Shared Function Retrieve(ByVal id As String, Optional domainid As String = Nothing, Optional forcereload As Boolean = False) As StatusType
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Return RetrieveDataObject(Of StatusType)(pkArray:={id, domainid}, domainID:=domainid, forceReload:=forcereload)
        End Function

        ''' <summary>
        ''' Create persistency for this object
        ''' </summary>
        ''' <param name="groupname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal id As String, Optional domainid As String = Nothing) As StatusType
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey() As Object = {id, domainid}
            Return ormBusinessObject.CreateDataObject(Of StatusType)(primarykey, domainID:=domainid, checkUnique:=True)
        End Function

    End Class

    ''' <summary>
    ''' Status ITEM Class for Stati in Object Messages
    ''' </summary>
    ''' <remarks>
    '''  Design Principle:
    ''' 
    ''' 1. Create or Add or Update Items by StatusType Object
    ''' 2. Retrieve direct a status
    ''' </remarks>
    ''' 
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleCommons, Version:=2, Release:=0, patch:=0, changeimplno:=2, _
        description:="Modify Status Item object to be a member of the Status Type object")> _
   <ormObject(id:=StatusItem.ConstObjectID, description:="status item description for object messages and others", _
       modulename:=ConstModuleCommons, Version:=1, usecache:=True, addDomainBehavior:=True, adddeletefieldbehavior:=True)> _
    Public Class StatusItem
        Inherits ormBusinessObject
        Implements iormRelationalPersistable
        Implements iormInfusable

        ''' <summary>
        ''' Object ID
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstObjectID = "StatusItem"

        ''' <summary>
        ''' Table
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleRepository, Version:=ConstOTDBSchemaVersion, Release:=0, patch:=0, changeimplno:=5, _
        description:="added foreign keys for table " & ConstPrimaryTableID)> _
        <ormTableAttribute(version:=2, usecache:=True)> Public Const ConstPrimaryTableID As String = "tblDefStatusItems"

        ''' <summary>
        ''' Primary Keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=StatusType.ConstObjectID & "." & StatusType.ConstFNID, PrimaryKeyOrdinal:=1, _
                        XID:="SI1", title:="Type", description:="type id of the status")> Public Const constFNType = "typeid"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=2, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty},
           XID:="SI2", title:="Code", description:="code id of the status")> Public Const constFNCode = "code"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=3 _
         , useforeignkey:=otForeignKeyImplementation.NativeDatabase, defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        ''' <summary>
        ''' Foreign Key to Status Type keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormForeignKey(entrynames:={constFNType, ConstFNDomainID}, _
            foreignkeyreferences:={Commons.StatusType.ConstObjectID & "." & Commons.StatusType.ConstFNID, _
                                   Commons.StatusType.ConstObjectID & "." & Commons.StatusType.ConstFNDomainID}, _
            useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const constFKType = "FKType"

        '* fields
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, _
           XID:="si3", title:="Title", description:="name of the status")> Public Const ConstFNTitle = "Title"
        <ormObjectEntry(Datatype:=otDataType.Memo, _
          XID:="si4", title:="Description", description:="description of the status")> Public Const constFNDescription = "desc"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, isnullable:=True, _
          XID:="si5", title:="KPICode", description:="KPI code of the status")> Public Const constFNKPICode = "kpicode"
        <ormObjectEntry(Datatype:=otDataType.Numeric, _
          XID:="si6", title:="Weight", description:="weight of the status")> Public Const constFNWeight = "weight"
        <ormObjectEntry(Datatype:=otDataType.Bool, _
          XID:="si11", title:="Start", description:="set if the status is an start status")> Public Const constFNIsStartStatus = "isstart"
        <ormObjectEntry(Datatype:=otDataType.Bool, _
          XID:="si12", title:="Intermediate", description:="set if the status is an intermediate status")> Public Const constFNIsEndStatus = "isend"
        <ormObjectEntry(Datatype:=otDataType.Bool, _
         XID:="si13", title:="End", description:="set if the status is an end status")> Public Const constFNIsIntermediateStatus = "isimed"

        <ormObjectEntry(Datatype:=otDataType.Bool, _
         XID:="si14", title:="Abort Operation", description:="set if the status will abort the ongoing-operation")> Public Const constFNAbort = "ABORT"

        <ormObjectEntry(Datatype:=otDataType.Long, isnullable:=True, _
          XID:="si21", title:="Foreground", description:="RGB foreground color code")> Public Const ConstFNFGColor = "fgcolor"
        <ormObjectEntry(Datatype:=otDataType.Long, isnullable:=True, _
          XID:="si22", title:="Background", description:="RGB background color code")> Public Const ConstFNBGColor = "bgcolor"
        <ormObjectEntry(Datatype:=otDataType.Long, isnullable:=True, _
          XID:="si23", title:="KPI Foreground", description:="RGB foreground kpi color code")> Public Const ConstFNKPIFGColor = "kpifgcolor"
        <ormObjectEntry(Datatype:=otDataType.Long, isnullable:=True, _
          XID:="si24", title:="KPI Background", description:="RGB background kpi color code")> Public Const ConstFNKPIBGColor = "kpibgcolor"


        '* mappings
        <ormObjectEntryMapping(EntryName:=constFNType)> Private _type As String = String.Empty  ' Status Type
        <ormObjectEntryMapping(EntryName:=constFNCode)> Private _code As String = String.Empty  ' code

        <ormObjectEntryMapping(EntryName:=ConstFNTitle)> Private _title As String
        <ormObjectEntryMapping(EntryName:=constFNDescription)> Private _description As String
        <ormObjectEntryMapping(EntryName:=constFNKPICode)> Private _kpicode As String
        <ormObjectEntryMapping(EntryName:=constFNWeight)> Private _weight As Double?
        <ormObjectEntryMapping(EntryName:=ConstFNFGColor)> Private _fgcolor As Long?
        <ormObjectEntryMapping(EntryName:=ConstFNBGColor)> Private _bgcolor As Long?
        <ormObjectEntryMapping(EntryName:=ConstFNKPIFGColor)> Private _kpifgcolor As Long?
        <ormObjectEntryMapping(EntryName:=ConstFNKPIBGColor)> Private _kpibgcolor As Long?
        <ormObjectEntryMapping(EntryName:=constFNIsEndStatus)> Private _endStatus As Boolean
        <ormObjectEntryMapping(EntryName:=constFNIsStartStatus)> Private _startStatus As Boolean
        <ormObjectEntryMapping(EntryName:=constFNAbort)> Private _Aborting As Boolean
        <ormObjectEntryMapping(EntryName:=constFNIsIntermediateStatus)> Private _intermediateStatus As Boolean

        Private _statusType As StatusType 'cached backlink

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the aborting.
        ''' </summary>
        ''' <value>The aborting.</value>
        Public Property Aborting() As Boolean
            Get
                Return Me._Aborting
            End Get
            Set(value As Boolean)
                Me._Aborting = value
            End Set
        End Property

        ''' <summary>
        ''' gets the typeid of the status item
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property TypeID() As String
            Get
                Return _type
            End Get

        End Property
        ''' <summary>
        ''' gets the code of the status type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Code() As String
            Get
                Return _code
            End Get

        End Property
        ''' <summary>
        ''' gets or sets the description of the status item 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(value As String)
                SetValue(constFNDescription, value)
            End Set
        End Property

        ''' <summary>
        ''' sets or gets the Title of the Status Item
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Title() As String
            Get
                Return _title
            End Get
            Set(value As String)
                SetValue(ConstFNTitle, value)
            End Set
        End Property

        ''' <summary>
        ''' sets or gets the KPI Code (statistic code)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property KPICode() As String
            Get
                Return _kpicode
            End Get
            Set(value As String)
                SetValue(constFNKPICode, value)
            End Set
        End Property

        ''' <summary>
        ''' sets or gets the weight of the status item
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Weight() As Double?
            Get
                Return _weight
            End Get
            Set(value As Double?)
                SetValue(constFNWeight, value)
            End Set
        End Property

        ''' <summary>
        ''' sets the start business process flag of this status
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsStartStatus() As Boolean
            Get
                Return _startStatus
            End Get
            Set(value As Boolean)
                SetValue(constFNIsStartStatus, value)
            End Set
        End Property

        ''' <summary>
        ''' sets or gets the intermediate business process status flag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Property IsIntermediateStatus() As Boolean
            Get
                Return _intermediateStatus
            End Get
            Set(value As Boolean)
                SetValue(constFNIsIntermediateStatus, value)
            End Set
        End Property
        ''' <summary>
        ''' sets or gets the buisness process flag for end status
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Property IsEndStatus() As Boolean
            Get
                Return _endStatus
            End Get
            Set(value As Boolean)
                SetValue(constFNIsEndStatus, value)
            End Set
        End Property

        ''' <summary>
        ''' sets or gets the Background Colour for rendering the status
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property FormatBGColorHex() As Long?
            Get
                Return _bgcolor
            End Get
            Set(value As Long?)
                SetValue(ConstFNBGColor, value)
            End Set
        End Property

        ''' <summary>
        ''' sets or gets the Background Color for rendering the status
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property FormatBGColor() As System.Drawing.Color?
            Get
                If Not Me.FormatBGColorHex.HasValue Then Return Nothing

                Return Converter.RGB2Color(Me.FormatBGColorHex.Value)
            End Get
            Set(value As System.Drawing.Color?)
                Me.FormatBGColorHex = Converter.Color2RGB(value)
            End Set
        End Property

        ''' <summary>
        ''' sets or gets the KPI Background Colour for rendering the status
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Property FormatkpiBGColorHex() As Long?
            Get

                Return _kpibgcolor
            End Get
            Set(value As Long?)
                SetValue(ConstFNKPIBGColor, value)
            End Set
        End Property

        ''' <summary>
        ''' sets or gets the KPI Background Color for rendering the status
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property FormatKPIBGColor() As System.Drawing.Color?
            Get
                If Not Me.FormatkpiBGColorHex.HasValue Then Return Nothing
                Return Converter.RGB2Color(Me.FormatkpiBGColorHex.Value)
            End Get
            Set(value As System.Drawing.Color?)
                Me.FormatkpiBGColorHex = Converter.Color2RGB(value)
            End Set
        End Property
        ''' <summary>
        ''' sets or gets the foreground color for rendering the status
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property FormatFGColorHex() As Long?
            Get
                Return _fgcolor
            End Get
            Set(value As Long?)
                SetValue(ConstFNFGColor, value)
            End Set
        End Property
        ''' <summary>
        ''' sets or gets the Foreground Color for rendering the status
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property FormatFGColor() As System.Drawing.Color?
            Get
                If Not Me.FormatFGColorHex.HasValue Then Return Nothing
                Return Converter.RGB2Color(Me.FormatFGColorHex.Value)
            End Get
            Set(value As System.Drawing.Color?)
                Me.FormatFGColorHex = Converter.Color2RGB(value)
            End Set
        End Property
        ''' <summary>
        ''' sets or gets the kpi foreground color for rendering the status
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property FormatKPIGFColorHex() As Long?
            Get
                Return _kpifgcolor
            End Get
            Set(value As Long?)
                SetValue(ConstFNKPIFGColor, value)
            End Set
        End Property
        ''' <summary>
        ''' sets or gets the Foreground Color for rendering the status
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property FormatKPIFGColor() As System.Drawing.Color?
            Get
                If Not Me.FormatKPIGFColorHex.HasValue Then Return Nothing
                Return Converter.RGB2Color(Me.FormatKPIGFColorHex.Value)
            End Get
            Set(value As System.Drawing.Color?)
                Me.FormatKPIGFColorHex = Converter.Color2RGB(value)
            End Set
        End Property
#End Region

        ''' <summary>
        ''' Handles OnCreating 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ValueEntry_OnCreating(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreating
            Dim my As ValueEntry = TryCast(e.DataObject, ValueEntry)

            If my IsNot Nothing Then
                Dim listid As String = e.Record.GetValue(constFNType)
                If listid Is Nothing Then
                    CoreMessageHandler(message:="Status Type id does not exist", procedure:="StatusItem.OnCreating", _
                                       messagetype:=otCoreMessageType.ApplicationError, _
                                       argument:=my.ListID)
                    e.AbortOperation = True
                    Return
                End If
                ''' even if it is early to retrieve the value list and set it (since this might disposed since we have not run through checkuniqueness and cache)
                ''' we need to check on the object here
                _statusType = StatusType.Retrieve(id:=listid)
                If _statusType Is Nothing Then
                    CoreMessageHandler(message:="Status Type does not exist", procedure:="StatusItem.OnCreated", _
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
        Public Sub ValueEntry_OnCreated(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreated
            Dim my As StatusItem = TryCast(e.DataObject, StatusItem)

            If my IsNot Nothing Then
                If _statusType Is Nothing Then
                    _statusType = StatusType.Retrieve(id:=my.TypeID)
                    If _statusType Is Nothing Then
                        CoreMessageHandler(message:="type id does not exist", procedure:="StatusItem.OnCreated", _
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
        Public Sub ValueEntry_OnInfused(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnInfused
            Dim my As StatusItem = TryCast(e.DataObject, StatusItem)

            Try
                ''' infuse is called on create as well as on retrieve / inject 
                ''' only on the create case we need to add to the status item otherwise
                ''' statustype will load the items
                ''' or the item will stand alone
                If my IsNot Nothing AndAlso e.Infusemode = otInfuseMode.OnCreate AndAlso _statusType IsNot Nothing Then
                    _statusType.Items.Add(my)
                End If

            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="StatusItem.Infuse")
            End Try


        End Sub
        ''' <summary>
        ''' Retrieve from datastore
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve([typeid] As String, code As String, Optional domainid As String = Nothing, Optional forcereload As Boolean = False) As StatusItem
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim pkarry() As Object = {typeid.ToUpper, code.ToUpper, domainid}
            Return RetrieveDataObject(Of StatusItem)(pkArray:=pkarry, domainID:=domainid, forceReload:=forcereload)
        End Function


        ''' <summary>
        ''' create a persistable object 
        ''' </summary>
        ''' <param name="typeid"></param>
        ''' <param name="code"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal typeid As String, ByVal code As String, Optional ByVal domainid As String = Nothing) As StatusItem
            ' set the primaryKey
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey() As Object = {typeid.ToUpper, code.ToUpper, domainid}
            Return ormBusinessObject.CreateDataObject(Of StatusItem)(primarykey, domainID:=domainid, checkUnique:=True)
        End Function

    End Class

    ''' <summary>
    ''' Workspace Definition Class
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=Workspace.ConstObjectID, adddomainBehavior:=False, adddeletefieldbehavior:=True, usecache:=True, _
        modulename:=ConstModuleCommons, description:="workspace definition for vertical grouping in scheduling", _
        Version:=1, useCache:=True)> Public Class Workspace
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable

        Public Const ConstObjectID = "Workspace"

        '** Table Schema
        <ormTableAttribute(Version:=2, usecache:=True)> Public Const ConstPrimaryTableID As String = "tblDefWorkspaces"

        '** primary Keys
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertystrings:={ObjectValidationProperty.NotEmpty}, _
            XID:="WS", title:="Workspace", Description:="workspaceID identifier")> Public Const ConstFNID As String = "wspace"


        '** Fields
        <ormObjectEntry(Datatype:=otDataType.Text, size:=255, isnullable:=True, _
            XID:="WS1", title:="Description")> Public Const ConstFNDescription = "desc"

        <ormObjectEntry(Datatype:=otDataType.List, innerDatatype:=otDataType.Text, _
            XID:="WS2", title:="forecast lookup order", description:="Forecasts milestones are lookup in this order. Must include this workspaceID ID.")> _
        Public Const ConstFNFCRelyOn = "fcrelyOn"

        <ormObjectEntry(Datatype:=otDataType.List, innerDatatype:=otDataType.Text, _
            XID:="WS3", title:="actual lookup order", description:="Actual milestones are looked up in this order. Must include this workspaceID ID")> _
        Public Const ConstFNActRelyOn = "actrelyOn"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0", _
            XID:="WS4", title:="Base", description:="if set this workspaceID is a base workspaceID")> Public Const ConstFNIsBase = "isbase"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=True, dbdefaultvalue:="1", _
              XID:="WS5", title:="has actuals", description:="if set this workspaceID has actual milestones") _
               > Public Const ConstFNHasAct = "hasact"

        <ormObjectEntry(Datatype:=otDataType.List, innerDatatype:=otDataType.Text, isnullable:=True,
          XID:="WS6", title:="accesslist", description:="Accesslist")> Public Const ConstFNAccesslist = "acclist"

        <ormObjectEntry(Datatype:=otDataType.[Long], defaultValue:=0, isnullable:=True, _
              XID:="WS7", title:="min schedule updc", description:="Minimum update counter for schedules of this workspaceID") _
               > Public Const ConstFNMinScheduleUPC = "minsupdc"

        <ormObjectEntry(Datatype:=otDataType.[Long], defaultValue:=9999, isnullable:=True, _
              XID:="WS8", title:="max schedule updc", description:="Maximum update counter for schedules of this workspaceID") _
               > Public Const ConstFNMaxScheduleUPC = "maxsupdc"

        <ormObjectEntry(Datatype:=otDataType.[Long], defaultValue:=0, isnullable:=True, _
              XID:="WS9", title:="min target updc", description:="Minimum update counter for targets of this workspaceID") _
               > Public Const ConstFNMinTargetUPDC = "mintupdc"

        <ormObjectEntry(Datatype:=otDataType.[Long], defaultValue:=9999, isnullable:=True, _
              XID:="WS10", title:="max target updc", description:="Minimum update counter for target of this workspaceID") _
               > Public Const ConstFNMaxTargetUPDC = "maxtupdc"

        ''' <summary>
        '''  deactivate foreign keys to domain since this would end up in a mess - all the schedule and deliverable object would be
        ''' in domains then also
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID,
            useforeignkey:=otForeignKeyImplementation.None)> Public Shadows Const ConstFNDomainID = Domain.ConstFNDomainID


        ' fields
        <ormObjectEntryMapping(EntryName:=ConstFNID)> Private _ID As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _description As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNIsBase)> Private _isBasespace As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNHasAct)> Private _hasActuals As Boolean
        <ormObjectEntryMapping(EntryName:=ConstFNFCRelyOn)> Private _fcrelyingOn As String()
        <ormObjectEntryMapping(EntryName:=ConstFNActRelyOn)> Private _actrelyingOn As String()
        <ormObjectEntryMapping(EntryName:=ConstFNAccesslist)> Private _accesslistID As String()

        <ormObjectEntryMapping(EntryName:=ConstFNMinScheduleUPC)> Private _min_schedule_updc As Long
        <ormObjectEntryMapping(EntryName:=ConstFNMaxScheduleUPC)> Private _max_schedule_updc As Long
        <ormObjectEntryMapping(EntryName:=ConstFNMinTargetUPDC)> Private _min_target_updc As Long
        <ormObjectEntryMapping(EntryName:=ConstFNMaxTargetUPDC)> Private _max_target_updc As Long

        ' dynamics
        Private _fc_wspace_stack As New List(Of String)
        Private _act_wspace_stack As New List(Of String)


#Region "Properties"
        ''' <summary>
        ''' Gets or sets the domain ID.
        ''' </summary>
        ''' <value>The domain ID.</value>
        Public Property DomainID() As String
            Get
                Return Me._domainID
            End Get
            Set(value As String)
                SetValue(ConstFNDomainID, value)
            End Set
        End Property

        ''' <summary>
        ''' get the ID of the Workspace
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        <ormPropertyMappingAttribute(ID:="ID", fieldname:=ConstFNID, tablename:=ConstPrimaryTableID)> ReadOnly Property ID() As String
            Get
                Return _ID
            End Get

        End Property

        ''' <summary>
        ''' gets or sets the description of the workspace
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
        ''' returns true if the workspace is a basespace - basic workspace where a schedule must reside !
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Property IsBasespace() As Boolean
            Get
                IsBasespace = _isBasespace
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsBase, value)
            End Set
        End Property

        ''' <summary>
        ''' returns true if the workspace has actuals of milestones
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property HasActuals() As Boolean
            Get
                Return _hasActuals
            End Get
            Set(value As Boolean)
                SetValue(ConstFNHasAct, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the forecast milestone workspaces in order 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Property FCRelyingOn() As String()
            Get
                Return _fcrelyingOn
            End Get
            Set(value As String())
                SetValue(ConstFNFCRelyOn, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the actuals milestone workspace order
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Property ACTRelyingOn() As String()
            Get

                Return _actrelyingOn
            End Get
            Set(value As String())
                SetValue(ConstFNActRelyOn, value)

            End Set
        End Property

        ''' <summary>
        ''' gets or set the access list
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AccesslistIDs() As String()
            Get
                Return _accesslistID
            End Get
            Set(value As String())
                SetValue(ConstFNAccesslist, value)
            End Set
        End Property
        ''' <summary>
        ''' get or set the minimum schedule updc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MinScheduleUPDC() As Long
            Get
                Return _min_schedule_updc
            End Get
            Set(value As Long)
                SetValue(ConstFNMinScheduleUPC, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the maximum schedule updc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MaxScheduleUPDC() As Long
            Get
                Return _max_schedule_updc
            End Get
            Set(value As Long)
                SetValue(ConstFNMaxScheduleUPC, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the minimum target updc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MinTargetUPDC() As Long
            Get
                Return _min_target_updc
            End Get
            Set(value As Long)
                SetValue(ConstFNMinTargetUPDC, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the maximum target updc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MaxTargetUPDC() As Long
            Get
                Return _max_target_updc
            End Get
            Set(value As Long)
                SetValue(ConstFNMaxTargetUPDC, value)
            End Set
        End Property

#End Region

        ''' <summary>
        ''' returns the first workspace in workspace stack which has actual milestones
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFirstBase(Optional domainid As String = Nothing) As Workspace
            If Not Me.IsAlive(subname:="GetFirstActual") Then Return Nothing
            If domainid Is Nothing Then domainid = CurrentSession.CurrentDomainID

            For Each anId In Me.FCRelyingOn
                Dim aWorkspace = Workspace.Retrieve(id:=anId, domainid:=domainid)
                If aWorkspace.IsBasespace Then Return aWorkspace
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' returns the first workspace in workspace stack which has actual milestones
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFirstActual(Optional domainid As String = Nothing) As Workspace
            If Not Me.IsAlive(subname:="GetFirstActual") Then Return Nothing
            If domainid Is Nothing Then domainid = CurrentSession.CurrentDomainID

            For Each anId In Me.ACTRelyingOn
                Dim aWorkspace = Workspace.Retrieve(id:=anId, domainid:=domainid)
                If aWorkspace.HasActuals Then Return aWorkspace
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' Retrieve the workspaceID Cache Object
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal id As String, Optional domainid As String = Nothing, Optional forcereload As Boolean = False) As Workspace
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim pkarray() As Object = {UCase(id)}
            Return RetrieveDataObject(Of Workspace)(pkArray:=pkarray, domainID:=domainid, forceReload:=forcereload)
        End Function


        ''' <summary>
        ''' creates with this object a new persistable Def workspaceID
        ''' </summary>
        ''' <param name="workspaceID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal workspaceID As String, Optional ByVal domainid As String = Nothing) As Workspace
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey() As Object = {workspaceID.ToUpper}
            Return ormBusinessObject.CreateDataObject(Of Workspace)(pkArray:=primarykey, domainID:=domainid, checkUnique:=True)
        End Function


        ''' <summary>
        ''' returns a List(of clsotdbDefWorkspace) of all workspaceID Definitions
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function All() As List(Of Workspace)
            Return ormBusinessObject.AllDataObject(Of Workspace)()
        End Function

    End Class

    ''' <summary>
    ''' Domain Definition Class
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(version:=1, id:=Domain.ConstObjectID, description:="domain definition for horizontal grouping of objects", _
        modulename:=ConstModuleCommons, isbootstrap:=True, useCache:=True)> Public Class Domain
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable


        '** const
        Public Const ConstObjectID = "Domain"
        <ormTableAttribute(Version:=1, usecache:=True)> Public Const ConstPrimaryTableID As String = "tblDefDomains"

        '** key
        <ormObjectEntry(XID:="DM1", _
            Datatype:=otDataType.Text, size:=50, Properties:={ObjectEntryProperty.Keyword}, _
            validationPropertyStrings:={ObjectValidationProperty.NotEmpty}, _
            title:="Domain", Description:="domain identifier", _
            PrimaryKeyOrdinal:=1, isnullable:=False, useforeignkey:=otForeignKeyImplementation.None)> Public Const ConstFNDomainID As String = "DOMAINID"

        '** fields
        <ormObjectEntry(XID:="DM2", _
            Datatype:=otDataType.Text, size:=100, _
            title:="Description", Description:="description of the domain")> Public Const ConstFNDescription = "desc"

        <ormObjectEntry(XID:="DM3", _
            Datatype:=otDataType.Bool, title:="Global", description:="if set this domain is the global domain") _
             > Public Const ConstFNIsGlobal = "isglobal"

        <ormObjectEntry(XID:="DM10", _
              Datatype:=otDataType.[Long], defaultValue:=0, dbdefaultvalue:="0", _
              title:="min deliverable uid", description:="Minimum deliverable uid for domain")> Public Const ConstFNMinDeliverableUID = "mindlvuid"

        <ormObjectEntry(XID:="DM11", _
              Datatype:=otDataType.[Long], defaultValue:=999999, dbdefaultvalue:="999999", _
              title:="max deliverable uid", description:="Maximum deliverable uid for domain")> Public Const ConstFNMaxDeliverableUID = "maxdlvuid"


        ' field mappings
        <ormObjectEntryMapping(EntryName:=ConstFNDomainID)> Private _domainID As String
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _description As String
        <ormObjectEntryMapping(EntryName:=ConstFNIsGlobal)> Private _isGlobal As Boolean

        <ormObjectEntryMapping(EntryName:=ConstFNMinDeliverableUID)> Private _min_deliverable_uid As Long
        <ormObjectEntryMapping(EntryName:=ConstFNMaxDeliverableUID)> Private _max_deliverable_uid As Long

        ' inherited from dataobject -> disabled
        '<ormEntryMapping(EntryName:=ConstFNDomainID, enabled:=False)> Protected _domainID As String = ConstGlobalDomain

        ' dynamics
        Private _listtings As New Dictionary(Of String, DomainSetting)
        Public Event OnInitialize As EventHandler(Of DomainEventArgs)
        Public Event OnReset As EventHandler(Of DomainEventArgs)

        Private _SessionDir As New Dictionary(Of String, Session)



#Region "Properties"
        ''' <summary>
        ''' returns the ID of this domain
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ormPropertyMappingAttribute(ID:="ID", fieldname:=ConstFNDomainID, tablename:=ConstPrimaryTableID)> ReadOnly Property ID() As String
            Get
                ID = _domainID
            End Get

        End Property
        ''' <summary>
        ''' gets and sets the description text of the domain
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description() As String
            Get
                Description = _description
            End Get
            Set(value As String)
            End Set
        End Property
        ''' <summary>
        ''' gets and set the Global Flag of the domain
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsGlobal() As Boolean
            Get
                IsGlobal = _isGlobal
            End Get
            Set(value As Boolean)
                SetValue(entryname:=ConstFNIsGlobal, value:=value)
            End Set
        End Property
        ''' <summary>
        ''' gets or set the minimum deliverable UID for this domain
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MinDeliverableUID() As Long
            Get
                MinDeliverableUID = _min_deliverable_uid
            End Get
            Set(value As Long)
                SetValue(entryname:=ConstFNMinDeliverableUID, value:=value)
            End Set
        End Property
        ''' <summary>
        ''' gets or set the maximum Deliverable UID for this domain
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MaxDeliverableUID() As Long
            Get
                MaxDeliverableUID = _max_deliverable_uid
            End Get
            Set(value As Long)
                SetValue(entryname:=ConstFNMaxDeliverableUID, value:=value)
            End Set
        End Property
        ''' <summary>
        ''' gets a list of domain settings
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Settings() As List(Of DomainSetting)
            Get
                Return _listtings.Values.ToList
            End Get
        End Property
#End Region

        ''' <summary>
        ''' returns a SQL String to insert the Gloobal Domain in the table -> bootstrap
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetInsertGlobalDomainSQLString(domainid As String, description As String, mindeliverableuid As Long, maxdeliverableuid As Long) As String

            Dim aSqlString As String = String.Format("INSERT INTO [{0}] ", CurrentSession.OTDBDriver.GetNativeDBObjectName(ConstPrimaryTableID))
            aSqlString &= String.Format("( [{0}], [{1}], [{2}], [{3}],  [{4}], [{5}], [{6}])", _
                                         ConstFNDomainID, ConstFNDescription, ConstFNIsGlobal, ConstFNMinDeliverableUID, ConstFNMaxDeliverableUID, _
                                         ConstFNCreatedOn, ConstFNUpdatedOn)

            aSqlString &= String.Format("VALUES ('{0}','{1}', {2}, {3}, {4},'{5}', '{6}' )", _
                                        domainid, description, 1, mindeliverableuid, maxdeliverableuid, _
                                         Date.Now.ToString("yyyy-MM-ddThh:mm:ss"), Date.Now.ToString("yyyy-MM-ddThh:mm:ss"))



            Return aSqlString

        End Function
        ''' <summary>
        ''' handles the session start event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnSessionEnd(sender As Object, e As SessionEventArgs)
            If _SessionDir.ContainsKey(e.Session.SessionID) Then
                _SessionDir.Remove(e.Session.SessionID)
            End If

        End Sub
        ''' <summary>
        ''' handles the session end event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>

        Public Sub OnSessionStart(sender As Object, e As SessionEventArgs)

        End Sub
        ''' <summary>
        ''' Register a Session a the Domain
        ''' </summary>
        ''' <param name="session"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RegisterSession(session As Session) As Boolean
            If _SessionDir.ContainsKey(session.SessionID) Then
                _SessionDir.Remove(session.SessionID)
            End If
            _SessionDir.Add(session.SessionID, session)
            AddHandler session.OnStarted, AddressOf OnSessionStart
            AddHandler session.OnEnding, AddressOf OnSessionEnd

        End Function


        Public Shared Function GlobalDomain() As Domain
            Return Retrieve(id:=ConstGlobalDomain)
        End Function
        ''' <summary>
        ''' Retrieve the workspaceID Cache Object
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal id As String, Optional dbdriver As iormRelationalDatabaseDriver = Nothing, Optional runtimeOnly As Boolean = False, Optional forcereload As Boolean = False) As Domain
            Dim pkarray() As Object = {UCase(id)}
            Return RetrieveDataObject(Of Domain)(pkArray:=pkarray, dbdriver:=dbdriver, runtimeOnly:=runtimeOnly, forceReload:=forcereload)
        End Function

        ''' <summary>
        ''' returns true if the setting exists
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasSetting(id As String) As Boolean
            Return _listtings.ContainsKey(key:=id.ToUpper)
        End Function
        ''' <summary>
        ''' returns the setting valid in the domain
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSetting(id As String) As DomainSetting
            If Me.HasSetting(id:=id) Then
                Return _listtings.Item(key:=id.ToUpper)
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' sets (add or overwrites) the setting valid in the domain
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetSetting(id As String, datatype As otDataType, value As Object, Optional description As String = Nothing) As Boolean
            Dim aSetting As New DomainSetting
            If Me.HasSetting(id:=id) Then
                aSetting = Me.GetSetting(id:=id)
            Else
                aSetting = DomainSetting.Create(domainID:=Me.ID, id:=id)
                If aSetting Is Nothing Then aSetting = DomainSetting.Retrieve(domainID:=Me.ID, id:=id)
            End If

            If aSetting Is Nothing OrElse Not aSetting.IsAlive(throwError:=False) Then
                Return False
            End If
            aSetting.Datatype = datatype
            aSetting.value = value
            aSetting.Description = description

            If Not Me.HasSetting(id:=id) Then _listtings.Add(key:=id.ToUpper, value:=aSetting)
            Return True
        End Function
        ''' <summary>
        ''' Load the settings to the settings dictionary
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadSettings() As Boolean
            Dim aListDomain As New List(Of DomainSetting)
            If ConstGlobalDomain <> Me.ID Then aListDomain = DomainSetting.RetrieveForDomain(domainid:=Me.ID)
            Dim aListGlobal As List(Of DomainSetting) = DomainSetting.RetrieveForDomain(domainid:=ConstGlobalDomain)

            '** first for the global
            For Each aSetting In aListGlobal
                If _listtings.ContainsKey(key:=aSetting.ID) Then
                    _listtings.Remove(key:=aSetting.ID)
                End If
                _listtings.Add(key:=aSetting.ID, value:=aSetting)
            Next

            '** overwrite
            For Each aSetting In aListDomain
                If _listtings.ContainsKey(key:=aSetting.ID) Then
                    _listtings.Remove(key:=aSetting.ID)
                End If
                _listtings.Add(key:=aSetting.ID, value:=aSetting)
            Next
            Return True
        End Function
        ''' <summary>
        ''' Persist the data object
        ''' </summary>
        ''' <param name="TIMESTAMP"></param>
        ''' <param name="ForceSerializeToOTDB"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Sub OnPersist(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnPersisted
            Try
                Dim myself = TryCast(e.DataObject, Domain)
                For Each aSetting In myself.Settings
                    aSetting.Persist()
                Next

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="Domain.OnPersisted", exception:=ex)
            End Try
        End Sub

        ''' <summary>
        ''' infuse the domain  by a record and load the settings
        ''' </summary>
        ''' <param name="aRecord"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Sub OnInfused(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnInfused

            Try

                If Not LoadSettings() Then
                End If

            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="Domain.Infuse")
            End Try
        End Sub
        ''' <summary>
        ''' create the objects persistence schema
        ''' </summary>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateSchema(Optional silent As Boolean = True) As Boolean
            Return ormBusinessObject.CreateDataObjectSchema(Of Domain)(silent:=silent)
        End Function

        ''' <summary>
        ''' creates with this object a new persistable Def workspaceID
        ''' </summary>
        ''' <param name="workspaceID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal ID As String, Optional runtimeonly As Boolean = False) As Domain
            Dim primarykey() As Object = {ID.ToUpper}
            Return ormBusinessObject.CreateDataObject(Of Domain)(pkArray:=primarykey, runtimeOnly:=runtimeonly, checkUnique:=Not runtimeonly)
        End Function

#Region "static routines"
        ''' <summary>
        ''' returns a List(of Domain) of all workspaceID Definitions
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function All() As List(Of Domain)
            Return ormBusinessObject.AllDataObject(Of Domain)()
        End Function
#End Region
    End Class
    '************************************************************************************
    '***** CLASS clsOTDBDefOrgUnit describes additional database schema information
    '*****
    ''' <summary>
    ''' Organization Unit Definition Class
    ''' </summary>
    ''' <remarks></remarks>

    <ormObject(id:=OrgUnit.ConstObjectID, modulename:=ConstModuleCommons, description:="recursive organization unit for a group of persons", _
        Version:=1, useCache:=True, adddeletefieldbehavior:=True, addDomainBehavior:=True)> Public Class OrgUnit
        Inherits ormBusinessObject
        Implements iormRelationalPersistable
        Implements iormInfusable

        '**
        Public Const ConstObjectID = "OrgUnit"
        '** Table
        <ormTableAttribute(version:=2, usecache:=True)> Public Const ConstPrimaryTableID As String = "tblDefOrgUnits"

        '** primary Keys
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty},
            XID:="OU1", title:="OrgUnit", description:="ID of the organization unit")> Public Const ConstFNID = "id"
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2 _
         , useforeignkey:=otForeignKeyImplementation.NativeDatabase, defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        '** fields
        <ormObjectEntry(Datatype:=otDataType.Text, size:=255, _
           XID:="OU2", title:="Description", description:="description of the organization unit")> Public Const ConstFNDescription = "desc"
        <ormObjectEntry(referenceObjectEntry:=Person.ConstObjectID & "." & Person.constFNID, _
           XID:="OU3", title:="Manager", description:="manager of the organization unit")> Public Const ConstFNManager = "manager"
        <ormObjectEntry(referenceObjectEntry:=Site.ConstObjectiD & "." & Site.constFNId, _
          XID:="OU4", title:="Site", description:="ID of the site organization unit")> Public Const ConstFNSite = "site"
        <ormObjectEntry(referenceObjectEntry:=ConstObjectID & "." & ConstFNID, _
          XID:="OU5", title:="Superior", description:="superior ID of the  organization unit")> Public Const ConstFNSuperior = "superior"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, _
         XID:="OU6", title:="Function", description:="default function ID of the  organization unit")> Public Const ConstFNFunction = "funct"

        ' field mapping
        <ormObjectEntryMapping(EntryName:=ConstFNID)> Private _id As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _description As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNManager)> Private _manager As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNSite)> Private _siteid As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNSuperior)> Private _superiorOUID As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNFunction)> Private _functionid As String = String.Empty



#Region "Properties"
        ReadOnly Property ID() As String
            Get
                ID = _id
            End Get

        End Property

        Public Property Description() As String
            Get
                Description = _description
            End Get
            Set(value As String)
                _description = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property Manager() As String
            Get
                Manager = _manager
            End Get
            Set(value As String)
                _manager = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property Siteid() As String
            Get
                Siteid = _siteid
            End Get
            Set(value As String)
                _siteid = value
                Me.IsChanged = True
            End Set
        End Property


        Public Property SuperiorOUID() As String
            Get
                SuperiorOUID = _superiorOUID
            End Get
            Set(value As String)
                _superiorOUID = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property Functionid() As String
            Get
                Functionid = _functionid
            End Get
            Set(value As String)
                _functionid = value
                Me.IsChanged = True
            End Set
        End Property
#End Region


        ''' <summary>
        ''' Retrieve 
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal id As String, Optional domainid As String = Nothing, Optional forcereload As Boolean = False) As OrgUnit
            Return RetrieveDataObject(Of OrgUnit)(pkArray:={domainid, id}, domainID:=domainid, forceReload:=forcereload)
        End Function


        ''' <summary>
        ''' returns a collection of all objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function All() As List(Of OrgUnit)
            Return ormBusinessObject.AllDataObject(Of OrgUnit)()
        End Function
        '**** create : create a new Object with primary keys
        '****
        Public Shared Function Create(ByVal id As String, Optional domainid As String = Nothing) As OrgUnit
            Dim primarykey() As Object = {id, domainid}
            ' set the primaryKey
            Return CreateDataObject(Of OrgUnit)(primarykey, domainID:=domainid, checkUnique:=True)
        End Function

    End Class


    ''' <summary>
    ''' Site Definition Class
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=Site.ConstObjectiD, description:="site (geographic units) definition for organization units", modulename:=ConstModuleCommons, _
        Version:=1, useCache:=True, addDomainBehavior:=True, adddeletefieldbehavior:=True)> Public Class Site
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable

        '** ObjectID
        Public Const ConstObjectiD = "Site"
        '** Table
        <ormTableAttribute(version:=2, useCache:=True)> Public Const ConstPrimaryTableID As String = "tblDefOUSites"

        '** keys
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=1, _
            XID:="OUS1", title:="Site ID", description:="id of the site")> Public Const constFNId = "id"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2 _
         , useforeignkey:=otForeignKeyImplementation.NativeDatabase, defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID


        '** fields
        <ormObjectEntry(referenceObjecTEntry:=CalendarEntry.ConstObjectID & "." & CalendarEntry.constFNName, _
            XID:="OUS2", title:="CalendarName", description:="name of the calendar valid for this site")> Public Const ConstFNCalendarID = "calendar"

        <ormObjectEntry(Datatype:=otDataType.Memo, XID:="OUS10", title:="Description", description:="description of the site")> Public Const constFNDescription = "desc"
        ' field mapping
        <ormObjectEntryMapping(EntryName:=constFNId)> Private _iD As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNCalendarID)> Private _CalendarID As String = String.Empty
        <ormObjectEntryMapping(EntryName:=constFNDescription)> Private _description As String = String.Empty


#Region "Properties"
        ''' <summary>
        ''' ID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ID() As String
            Get
                ID = _iD
            End Get

        End Property
        ''' <summary>
        ''' Description
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description() As String
            Get
                Description = _description
            End Get
            Set(value As String)
                _description = value
                Me.IsChanged = True
            End Set
        End Property
#End Region

        ''' <summary>
        ''' Retrieve
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal id As String, Optional domainid As String = Nothing, Optional forcereload As Boolean = False) As Site
            Return RetrieveDataObject(Of Site)(pkArray:={UCase(id), domainid}, domainID:=domainid, forceReload:=forcereload)
        End Function


        ''' <summary>
        ''' returns a collection of all objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function All(Optional domainid As String = Nothing) As List(Of Site)
            Return ormBusinessObject.AllDataObject(Of Site)(domainid:=domainid)
        End Function
        '**** create : create a new Object with primary keys
        ''' <summary>
        ''' creates a persistable site object
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal id As String, Optional domainid As String = Nothing) As Site
            Dim primarykey() As Object = {id, domainid}
            ' set the primaryKey
            Return ormBusinessObject.CreateDataObject(Of Site)(primarykey, domainID:=domainid, checkUnique:=True)
        End Function

    End Class


    ''' <summary>
    ''' Change Log Entry for OnTrack Changes
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <ormObject(id:=OnTrackChangeLogEntry.ConstObjectiD, description:="Entry of the OnTrack Change Log", modulename:=ConstModuleCommons, _
        Version:=1, useCache:=True, addDomainBehavior:=False, adddeletefieldbehavior:=False)> Public Class OnTrackChangeLogEntry
        Inherits ormBusinessObject


        '** ObjectID
        Public Const ConstObjectiD = "OTChangeLogEntry"
        '** Table
        <ormTableAttribute(version:=3, useCache:=True)> Public Const ConstPrimaryTableID As String = "TBLCHANGELOGENTRIES"

        '** keys
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=1, defaultvalue:="OTBACKEND", _
            XID:="CLE1", title:="Application", description:="Name of the OnTrack application")> Public Const ConstFNApplication = "APPLICATION"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=2, defaultvalue:=ot.ConstModuleCommons, _
            XID:="CLE2", title:="Module", description:="Name of the OnTrack application module")> Public Const ConstFNModule = "MODULE"

        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=3, defaultvalue:=1, _
            XID:="CLE3", title:="Version", description:="Version number of the OnTrack application release")> Public Const ConstFNVersion = "VERSION"

        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=4, defaultvalue:=1, _
           XID:="CLE4", title:="Release", description:="Number of the application release")> Public Const ConstFNRelease = "RELEASE"

        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=5, defaultvalue:=0, _
            XID:="CLE5", title:="Patch", description:="Patch number of the OnTrack application version")> Public Const ConstFNPatch = "PATCH"

        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=6, defaultvalue:=1, _
            XID:="CLE6", title:="Implementation No", description:="implementation number of the OnTrack application change")> Public Const ConstFNImplNo = "NO"

        ''' <summary>
        ''' Column Members
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(datatype:=otDataType.Date, isnullable:=True, _
            XID:="CLE10", title:="Date of the release change", description:="date of the change")> Public Const ConstFNReleaseDate = "RELEASEDATE"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, isnullable:=True,
         useforeignkey:=otForeignKeyImplementation.None, defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        <ormObjectEntry(datatype:=otDataType.Text, size:=100, isnullable:=True, _
           XID:="CLE11", title:="CRID", description:="ID of the change request")> Public Const ConstFNCRID = "CRID"

        <ormObjectEntry(Datatype:=otDataType.Memo, isnullable:=True, _
            XID:="CLE20", title:="Description", description:="description of the change implementation")> Public Const ConstFNDescription = "DESCRIPTION"


        ' field mapping
        <ormObjectEntryMapping(EntryName:=ConstFNApplication)> Private _Application As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNModule)> Private _Module As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNRelease)> Private _Release As Long
        <ormObjectEntryMapping(EntryName:=ConstFNVersion)> Private _Version As Long
        <ormObjectEntryMapping(EntryName:=ConstFNPatch)> Private _Patch As Long
        <ormObjectEntryMapping(EntryName:=ConstFNImplNo)> Private _ChangeImplementationNo As Long

        <ormObjectEntryMapping(EntryName:=ConstFNCRID)> Private _changerequestID As String
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _description As String
        <ormObjectEntryMapping(EntryName:=ConstFNReleaseDate)> Private _releasedate As Date?

        ''' <summary>
        '''  constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
        End Sub
        Public Sub New(application As String, [module] As String, version As Long, release As Long, patch As Long, changeimplno As Long, description As String)
            MyBase.New()
            _Application = application
            _Module = [module]
            _Version = version
            _Release = release
            _Patch = patch
            _ChangeImplementationNo = changeimplno
            _description = description
        End Sub

#Region "Properties"
        ''' <summary>
        ''' Gets or sets the change implementation no.
        ''' </summary>
        ''' <value>The change implementation no.</value>
        Public Property ChangeImplementationNo() As Long
            Get
                Return Me._ChangeImplementationNo
            End Get
            Set(value As Long)
                SetValue(ConstFNImplNo, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the releasedate.
        ''' </summary>
        ''' <value>The releasedate.</value>
        Public Property Releasedate() As DateTime?
            Get
                Return Me._releasedate
            End Get
            Set(value As DateTime?)
                SetValue(ConstFNReleaseDate, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the changerequest ID.
        ''' </summary>
        ''' <value>The changerequest ID.</value>
        Public Property ChangerequestID() As String
            Get
                Return Me._changerequestID
            End Get
            Set(value As String)
                SetValue(ConstFNCRID, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the patch.
        ''' </summary>
        ''' <value>The patch.</value>
        Public Property Patch() As Long
            Get
                Return Me._Patch
            End Get
            Set(value As Long)
                SetValue(ConstFNPatch, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the version.
        ''' </summary>
        ''' <value>The version.</value>
        Public Property Version() As Long
            Get
                Return Me._Version
            End Get
            Set(value As Long)
                SetValue(ConstFNVersion, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the release.
        ''' </summary>
        ''' <value>The release.</value>
        Public Property Release() As Long
            Get
                Return Me._Release
            End Get
            Set(value As Long)
                SetValue(ConstFNRelease, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the module.
        ''' </summary>
        ''' <value>The module.</value>
        Public ReadOnly Property [Module]() As String
            Get
                Return Me._Module
            End Get
        End Property

        ''' <summary>
        ''' gets the name of the application
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Application() As String
            Get
                Return _Application
            End Get

        End Property
        ''' <summary>
        ''' Description
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description() As String
            Get
                Description = _description
            End Get
            Set(value As String)
                SetValue(ConstFNDescription, value)
            End Set
        End Property
        ''' <summary>
        ''' returns the Versioning String
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Versioning() As String
            Get
                Return New OnTrackChangeLog.Versioning(Me.Version, Me.Release, Me.Patch).ToString
            End Get
        End Property
#End Region

        ''' <summary>
        ''' Retrieve
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal application As String, ByVal [module] As String, version As Long, release As Long, patch As Long, changeimplno As Long _
                                                  ) As OnTrackChangeLogEntry
            Return RetrieveDataObject(Of OnTrackChangeLogEntry)(pkArray:={UCase(application), UCase([module]), release, version, patch, changeimplno})
        End Function


        ''' <summary>
        ''' returns a collection of all objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function All() As List(Of OnTrackChangeLogEntry)
            Return ormBusinessObject.AllDataObject(Of OnTrackChangeLogEntry)()
        End Function
        '**** create : create a new Object with primary keys
        ''' <summary>
        ''' creates a persistable site object
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal application As String, ByVal [module] As String, version As Long, release As Long, patch As Long?, changeimplno As Long, _
                                                  Optional runtimeOnly As Boolean = True) As OnTrackChangeLogEntry
            If Not patch.HasValue Then patch = 0
            Dim primarykey() As Object = {UCase(application), UCase([module]), version, release, patch, changeimplno}
            ' set the primaryKey
            Return ormBusinessObject.CreateDataObject(Of OnTrackChangeLogEntry)(primarykey, checkUnique:=True, runtimeOnly:=runtimeOnly)
        End Function

        ''' <summary>
        ''' creates a persistable object
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Create(Optional ByVal application As String = Nothing, _
                               Optional ByVal [module] As String = Nothing,
                               Optional version As Long? = Nothing, _
                               Optional release As Long? = Nothing,
                                         Optional patch As Long? = Nothing, _
                                         Optional changeimplno As Long? = Nothing, _
                                                  Optional runtimeOnly As Boolean = True) As Boolean
            If Not patch.HasValue Then patch = 0
            Dim aRecord As New ormRecord
            Me.Feed(aRecord)
            With aRecord
                If application IsNot Nothing Then .SetValue(ConstFNApplication, application.ToUpper)
                If [module] IsNot Nothing Then .SetValue(ConstFNModule, [module].ToUpper)
                If version.HasValue Then .SetValue(ConstFNVersion, version)
                If release.HasValue Then .SetValue(ConstFNRelease, release)
                If patch.HasValue Then .SetValue(ConstFNPatch, patch)
                If changeimplno.HasValue Then .SetValue(ConstFNImplNo, changeimplno)
            End With

            ' set the primaryKey
            Return MyBase.Create(aRecord, checkUnique:=True, runtimeOnly:=runtimeOnly)
        End Function

    End Class


End Namespace
