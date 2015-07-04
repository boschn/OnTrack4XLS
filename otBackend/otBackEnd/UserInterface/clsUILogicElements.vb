REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** User Interface - Logical persistable Elements
REM *********** 
REM *********** Version: 2.0
REM *********** Created: 2015-02-13
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
Imports OnTrack.Database
Imports OnTrack.Core


Namespace OnTrack.UI

    ''' <summary>
    ''' Type of Expression
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otExpressionType
        ObjectEntry
    End Enum
    ''' <summary>
    ''' status of the User interface
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otUIStatus
        Disabled = 0
        Enabled = 1
        Hidden = 4
        Visible = 5
    End Enum
    ''' <summary>
    ''' persistable View Element
    ''' </summary>
    ''' <remarks></remarks>

    <ormObject(version:=1, id:=ViewElementColumn.ConstObjectID, description:="persistable UI view column element", _
       modulename:=ConstModuleUIElements, isbootstrap:=False, useCache:=True, adddomainbehavior:=True)> _
    Public Class ViewElementColumn
        Inherits persistableDataObject
        Implements iormEmbeddedObject

        ''' <summary>
        ''' object ID
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstObjectID = "UIViewElementColumn"

        ''' <summary>
        ''' define the container of the class
        ''' </summary>
        ''' <remarks></remarks>
        <ormEmbeddedContainer(version:=1, adddomainbehavior:=True, PrimaryDatabaseDriverName:=ConstCPVDriverEmbeddedName, _
            embeddedin:=ViewElement.ConstObjectID & "." & ViewElement.ConstFNViewColumns, _
            serializeas:=otSerializeFormat.XML)> Public Const ConstContainerID As String = ViewElement.ConstObjectID & "_" & ViewElement.ConstFNViewColumns

        ''' <summary>
        ''' Primary Key: First key must be the host key
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(category:="Key", referenceObjectEntry:=ViewElement.ConstObjectID & "." & ViewElement.ConstFNViewID, PrimaryKeyOrdinal:=1 _
                        )> Public Const ConstFNViewID = "VIEWID"

        <ormObjectEntry(category:="Key", referenceObjectEntry:=Commons.Domain.ConstObjectID & "." & Commons.Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2 _
                       )> Public Const ConstFNDomainID = Commons.Domain.ConstFNDomainID

        <ormObjectEntry(category:="Key", datatype:=otDataType.Long, PrimaryKeyOrdinal:=3, _
                        title:="UID", description:="id of the column")> Public Const ConstFNColumnID = "UID"

        ''' <summary>
        ''' type of the expression in the column
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(category:="Properties", datatype:=otDataType.Text, size:=15, defaultvalue:=otExpressionType.ObjectEntry,
                     title:="Expression Type", description:="expression type of the column")> Public Const ConstFNExpressionType = "EXPRESSIONTYPE"

        ''' <summary>
        ''' expression of the column
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(category:="Properties", datatype:=otDataType.Text, isnullable:=True,
                     title:="Expression", description:="expression of the column")> Public Const ConstFNExpression = "EXPRESSION"

        ''' <summary>
        ''' Column size
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(category:="Properties", datatype:=otDataType.Long, isnullable:=True,
                      title:="Size", description:="size of the column in characters")> Public Const ConstFNSize = "SIZE"

        ''' <summary>
        ''' Column Header
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(category:="Properties", datatype:=otDataType.Text, size:=50, isnullable:=True,
                      title:="Title", description:="header text of the column")> Public Const ConstFNTitle = "TITLE"

        ''' <summary>
        ''' Column Category
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(category:="Properties", datatype:=otDataType.Text, size:=50, isnullable:=True,
                      title:="Category", description:="category of the column")> Public Const ConstFNCategory = "CATEGORY"

        ''' <summary>
        ''' Column Description
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(category:="Properties", datatype:=otDataType.Text, size:=150, isnullable:=True,
                      title:="Description", description:="description text of the column")> Public Const ConstFNDescription = "DESCRIPTION"

        ''' <summary>
        ''' Column Status
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(category:="Properties", datatype:=otDataType.Text, size:=15, isnullable:=True,
                      title:="Status", description:="status of the column")> Public Const ConstFNStatus = "UISTATUS"

        ''' <summary>
        ''' ORDINAL
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(category:="Properties", datatype:=otDataType.Long, isnullable:=True, _
                      title:="Ordinal", description:="ordinal of the column")> Public Const ConstFNOrdinal = "ORDINAL"

        ''' <summary>
        ''' isReadonly
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(category:="Properties", datatype:=otDataType.Bool, defaultvalue:=False, _
                      title:="Is Readonly", description:="readonly flag")> Public Const ConstFNIsReadonly = "IsReadonly"

        ''' <summary>
        ''' Mapping
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNViewID)> Private _viewid As String
        <ormObjectEntryMapping(EntryName:=ConstFNDomainID)> Private _domainid As String
        <ormObjectEntryMapping(EntryName:=ConstFNColumnID)> Private _id As Long
        <ormObjectEntryMapping(EntryName:=ConstFNExpressionType)> Private _expressiontype As otExpressionType
        <ormObjectEntryMapping(EntryName:=ConstFNExpression)> Private _expression As String
        <ormObjectEntryMapping(EntryName:=ConstFNCategory)> Private _category As String
        <ormObjectEntryMapping(EntryName:=ConstFNTitle)> Private _title As String
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _description As String
        <ormObjectEntryMapping(EntryName:=ConstFNSize)> Private _size As Long?
        <ormObjectEntryMapping(EntryName:=ConstFNStatus)> Private _status As otUIStatus?
        <ormObjectEntryMapping(EntryName:=ConstFNOrdinal)> Private _ordinal As Long?
        <ormObjectEntryMapping(EntryName:=ConstFNIsReadonly)> Private _isreadonly As Boolean

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the isreadonly.
        ''' </summary>
        ''' <value>The isreadonly.</value>
        Public Property Isreadonly() As Boolean
            Get
                Return _isreadonly
            End Get
            Set(value As Boolean)
                SetValue(ConstFNIsReadonly, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the ordinal.
        ''' </summary>
        ''' <value>The ordinal.</value>
        Public Property Ordinal() As Long?
            Get
                Return _ordinal
            End Get
            Set(value As Long?)
                SetValue(ConstFNOrdinal, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the status.
        ''' </summary>
        ''' <value>The status.</value>
        Public Property Status() As otUIStatus?
            Get
                Return _status
            End Get
            Set(value As otUIStatus?)
                SetValue(ConstFNStatus, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the size.
        ''' </summary>
        ''' <value>The size.</value>
        Public Property Size() As Long?
            Get
                Return _size
            End Get
            Set(value As Long?)
                SetValue(ConstFNSize, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the description.
        ''' </summary>
        ''' <value>The description.</value>
        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(value As String)
                SetValue(ConstFNDescription, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the title.
        ''' </summary>
        ''' <value>The title.</value>
        Public Property Title() As String
            Get
                Return _title
            End Get
            Set(value As String)
                SetValue(ConstFNTitle, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the category.
        ''' </summary>
        ''' <value>The category.</value>
        Public Property Category() As String
            Get
                Return _category
            End Get
            Set(value As String)
                SetValue(ConstFNCategory, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the expression.
        ''' </summary>
        ''' <value>The expression.</value>
        Public Property Expression() As String
            Get
                Return _expression
            End Get
            Set(value As String)
                SetValue(ConstFNExpression, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the expressiontype.
        ''' </summary>
        ''' <value>The expressiontype.</value>
        Public Property Expressiontype() As otExpressionType
            Get
                Return _expressiontype
            End Get
            Set(value As otExpressionType)
                SetValue(ConstFNExpressionType, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the id.
        ''' </summary>
        ''' <value>The id.</value>
        Public ReadOnly Property Id() As Long
            Get
                Return _id
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the domainid.
        ''' </summary>
        ''' <value>The domainid.</value>
        Public ReadOnly Property Domainid() As String
            Get
                Return _domainid
            End Get

        End Property

        ''' <summary>
        ''' Gets the viewid.
        ''' </summary>
        ''' <value>The viewid.</value>
        Public ReadOnly Property Viewid() As String
            Get
                Return _viewid
            End Get
        End Property
        ''' <summary>
        ''' returns true if the UI is visible
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsVisible As Boolean
            Get
                If Me.Status.HasValue Then Return Me.Status And otUIStatus.Visible
                Return False
            End Get
        End Property
        ''' <summary>
        ''' returns true if the UI is enabled
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsEnabled As Boolean
            Get
                If Me.Status.HasValue Then Return Me.Status And otUIStatus.Enabled
                Return False
            End Get
        End Property
#End Region

#Region "PersistenceHandling"

        ''' <summary>
        ''' Retrieve
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal viewid As String, ByVal id As Long, Optional domainid As String = Nothing, Optional forcereload As Boolean = False) As ViewElementColumn
            Dim primarykey() As Object = {viewid, domainid, id}
            Return persistableDataObject.RetrieveDataObject(Of ViewElementColumn)(pkArray:=primarykey, domainID:=domainid, forceReload:=forcereload)
        End Function

        ''' <summary>
        ''' returns a collection of all Person Definition Objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function All(viewid As String, Optional domainid As String = Nothing) As List(Of ViewElementColumn)
            Dim aKey = New ormDatabaseKey(objectid:=ConstObjectID)
            aKey.Item(ConstFNViewID) = viewid
            aKey.Item(ConstFNDomainID) = domainid
            Return persistableDataObject.AllDataObject(Of ViewElementColumn)(aKey, domainid:=domainid)
        End Function


        ''' <summary>
        ''' Creates the persistence object
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal viewid As String, id As Long, Optional domainid As String = Nothing) As ViewElementColumn
            Dim primarykey() As Object = {viewid, domainid, id}
            ' set the primaryKey
            Return persistableDataObject.CreateDataObject(Of ViewElementColumn)(primarykey, domainID:=domainid, checkUnique:=True)
        End Function

#End Region

    End Class
    ''' <summary>
    ''' persistable View Element
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(version:=1, id:=ViewElement.ConstObjectID, description:="persistable UI view element", _
       modulename:=ConstModuleUIElements, isbootstrap:=False, useCache:=True, adddomainbehavior:=True)> _
    Public Class ViewElement
        Inherits ormBusinessObject

        ''' <summary>
        ''' object ID
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstObjectID = "UIViewElement"

        ''' <summary>
        ''' define the container of the class
        ''' </summary>
        ''' <remarks></remarks>
        <ormTable(version:=1, adddomainbehavior:=True)> Public Const ConstPrimaryTableID As String = "TBLUIVIEWELEMENTS"

        ''' <summary>
        ''' primary keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(category:="Key", datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=1, _
                        properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty}, _
                        description:="ID of the View")> Public Const ConstFNViewID = "VIEWID"

        <ormObjectEntry(category:="Key", referenceObjectEntry:=Commons.Domain.ConstObjectID & "." & Commons.Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2 _
                      )> Public Const ConstFNDomainID = Commons.Domain.ConstFNDomainID



        ''' <summary>
        ''' Column Header
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(category:="Properties", datatype:=otDataType.Text, size:=100, isnullable:=True,
                      title:="Title", description:="header text of the view")> Public Const ConstFNTitle = "TITLE"

        ''' <summary>
        ''' Column Category
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(category:="Properties", datatype:=otDataType.Text, size:=50, isnullable:=True,
                      title:="Category", description:="category of the view")> Public Const ConstFNCategory = "CATEGORY"

        ''' <summary>
        ''' Column Description
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(category:="Properties", datatype:=otDataType.Text, size:=255, isnullable:=True,
                      title:="Description", description:="description text of the view")> Public Const ConstFNDescription = "DESCRIPTION"


        ''' <summary>
        ''' hosting element for the column definitions
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(datatype:=otDataType.Memo, _
                        description:="columns of the View")> Public Const ConstFNViewColumns = "XMLVIEWCOLUMNS"

        ''' <summary>
        ''' UI Status
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(category:="Properties", datatype:=otDataType.Text, size:=15, isnullable:=True,
                      title:="Status", description:="ui status of the view")> Public Const ConstFNStatus = "UISTATUS"

        ''' <summary>
        ''' Relation to Tracks - will be resolved via event handling
        ''' </summary>
        ''' <remarks>
        ''' track object is not finished add createobjectifnotretrieved:=True again if Track can build itself from otherobjects
        ''' </remarks>
        <ormRelation(linkObject:=GetType(ViewElementColumn), _
                     fromEntries:={ConstFNViewID, ConstFNDomainID}, toEntries:={ConstFNViewID, ConstFNDomainID}, _
                     cascadeonCreate:=True, cascadeOnDelete:=True, cascadeOnUpdate:=False)> _
        Public Const ConstRColumns = "RELViewElementColumns"

        <ormObjectEntryMapping(relationName:=ConstRColumns, infusemode:=otInfuseMode.OnDemand)> _
        Private WithEvents _ColumnCollection As ormRelationCollection(Of ViewElementColumn) = _
        New ormRelationCollection(Of ViewElementColumn)(container:=Me, keyentrynames:={ViewElementColumn.ConstFNViewID, ViewElementColumn.ConstFNDomainID, ViewElementColumn.ConstFNColumnID})


        ''' <summary>
        ''' Mapping
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNViewID)> Private _viewid As String
        <ormObjectEntryMapping(EntryName:=ConstFNDomainID)> Private _domainid As String
        <ormObjectEntryMapping(EntryName:=ConstFNViewColumns)> Private _viewcolumnsxml As String
        <ormObjectEntryMapping(EntryName:=ConstFNStatus)> Private _status As otUIStatus?
        <ormObjectEntryMapping(EntryName:=ConstFNCategory)> Private _category As String
        <ormObjectEntryMapping(EntryName:=ConstFNTitle)> Private _title As String
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _description As String

#Region "Properties"
        ''' <summary>
        ''' returns the Columns of this view
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Columns As iormRelationalCollection(Of ViewElementColumn)
            Get
                Return _ColumnCollection
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the description.
        ''' </summary>
        ''' <value>The description.</value>
        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(value As String)
                SetValue(ConstFNDescription, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the title.
        ''' </summary>
        ''' <value>The title.</value>
        Public Property Title() As String
            Get
                Return _title
            End Get
            Set(value As String)
                SetValue(ConstFNTitle, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the category.
        ''' </summary>
        ''' <value>The category.</value>
        Public Property Category() As String
            Get
                Return _category
            End Get
            Set(value As String)
                SetValue(ConstFNCategory, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the status.
        ''' </summary>
        ''' <value>The status.</value>
        Public Property Status() As otUIStatus?
            Get
                Return _status
            End Get
            Set(value As otUIStatus?)
                SetValue(ConstFNStatus, value)
            End Set
        End Property
        ''' <summary>
        ''' Gets the domainid.
        ''' </summary>
        ''' <value>The domainid.</value>
        Public ReadOnly Property Domainid() As String
            Get
                Return _domainid
            End Get
        End Property

        ''' <summary>
        ''' Gets the viewid.
        ''' </summary>
        ''' <value>The viewid.</value>
        Public ReadOnly Property Viewid() As String
            Get
                Return _viewid
            End Get
        End Property
        ''' <summary>
        ''' returns true if the UI is visible
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsVisible As Boolean
            Get
                If Me.Status.HasValue Then Return Me.Status And otUIStatus.Visible
                Return False
            End Get
        End Property
        ''' <summary>
        ''' returns true if the UI is enabled
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsEnabled As Boolean
            Get
                If Me.Status.HasValue Then Return Me.Status And otUIStatus.Enabled
                Return False
            End Get
        End Property
#End Region


        ''' <summary>
        ''' Retrieve a persisted view element
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal viewid As String, Optional domainid As String = Nothing, Optional forcereload As Boolean = False) As ViewElement
            Dim primarykey() As Object = {viewid, domainid}
            Return RetrieveDataObject(Of ViewElement)(pkArray:=primarykey, domainID:=domainid, forceReload:=forcereload)
        End Function

        ''' <summary>
        ''' create a persistable view element
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal viewid As String, Optional domainid As String = Nothing) As ViewElement
            Dim primarykey() As Object = {viewid, domainid}
            ' set the primaryKey
            Return CreateDataObject(Of ViewElement)(primarykey, domainID:=domainid, checkUnique:=True)
        End Function
        ''' <summary>
        ''' Initialize a view element by referencing to an object id and fill it will all settings of that object
        ''' </summary>
        ''' <param name="objectid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InitializeView(objectid As String) As Boolean
            If Not Me.IsAlive("InitializeView") Then Return False

            Dim anObjectDefinition As iormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=objectid)
            If anObjectDefinition Is Nothing Then
                CoreMessageHandler(message:="object id is not found in the repository", objectname:=objectid, procedure:="ViewElement.InitializeView", messagetype:=otCoreMessageType.ApplicationError)
                Return False
            End If

            ''' save all the entries
            ''' 
            Dim i As UShort = 1
            For Each anEntry In anObjectDefinition.GetEntries
                Dim aColumn As ViewElementColumn = Me.Columns.Where(Function(x) x.Id = i).FirstOrDefault
                If aColumn Is Nothing Then aColumn = ViewElementColumn.Create(Me.Viewid, id:=i)
                With aColumn
                    .Title = anEntry.Title
                    .Description = anEntry.Description
                    .Ordinal = anEntry.Ordinal
                    .Expressiontype = otExpressionType.ObjectEntry
                    .Expressiontype = anObjectDefinition.Objectname & "." & anEntry.Entryname
                    .Category = anEntry.Category
                    .Status = otUIStatus.Visible
                    If Not anEntry.IsActive Then .Status = otUIStatus.Disabled
                    .Isreadonly = anEntry.IsReadonly
                End With
                Me.Columns.Add(aColumn)
                i += 1
            Next

            Return True
        End Function

        ''' <summary>
        ''' Event Handler for the Added Columns
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ViewElement_ColumnCollectionOnAdded(sender As Object, e As ormRelationCollection(Of ViewElementColumn).EventArgs) Handles _ColumnCollection.OnAdded
            If Not IsAlive(subname:="ViewElement_ColumnCollectionOnAdded") Then
                e.Cancel = True
                Exit Sub
            End If
        End Sub
    End Class

    ''' <summary>
    ''' persistable PanelElelemnt
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PanelElement

    End Class

End Namespace