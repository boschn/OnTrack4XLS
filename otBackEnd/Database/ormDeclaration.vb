
REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** Object Relationship Model Declaration
REM *********** 
REM *********** Version: 2.00
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
Imports OnTrack.Core

Namespace OnTrack.Database

    ''' <summary>
    ''' Point of Lifecycle to infuse a relation
    ''' </summary>
    ''' <remarks></remarks>

    Public Enum otInfuseMode
        None = 0
        OnInject = 1
        OnCreate = 2
        OnDefault = 8
        OnDemand = 16
        Always = 27 ' Logical AND of everything
    End Enum
    ''' <summary>
    ''' the Foreign Key Implementation layer
    ''' on Native Database layer or ORM (internal)
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otForeignKeyImplementation
        None = 0
        NativeDatabase = 1
        ORM = 3
    End Enum

    ''' <summary>
    ''' Data Types for OnTrack Database Fields -> moved to rulez
    ''' </summary>
    ''' <remarks></remarks>

    '<TypeConverter(GetType(Long))> Public Enum otDataType
    '    Numeric = 1
    '    List = 2
    '    Text = 3
    '    Runtime = 4
    '    Formula = 5
    '    [Date] = 6
    '    [Long] = 7
    '    Timestamp = 8
    '    Bool = 9
    '    Memo = 10
    '    Binary = 11
    '    Time = 12
    'End Enum
    ''' <summary>
    ''' Entry Type
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otObjectEntryType
        ContainerEntry = 1
        CompoundEntry = 2
    End Enum

    ''' <summary>
    ''' defines a data oject repository
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormDataObjectRepository
        Inherits IDataObjectRepository


        Function GetDataObjectProvider(type As Type) As iormDataObjectProvider

        Function GetDataObjectProvider(objectid As String) As iormDataObjectProvider

       
        ReadOnly Property ObjectDefinitions As IEnumerable(Of iormObjectDefinition)

        Function GetObjectEntries(objectname As String) As List(Of iormObjectEntryDefinition)

        Function HasObjectEntry(objectname As String, entryname As String) As Boolean

        Function GetEntriesByXID(xid As String, Optional objectname As String = Nothing) As IList(Of iormObjectEntryDefinition)

        ''' <summary>
        ''' Return an ObjectDefinition
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetObjectDefinition(id As String, Optional runtimeOnly? As Boolean = Nothing) As iormObjectDefinition

        Function GetEntryDefinition(entryname As String, Optional objectname As String = Nothing, Optional runtimeonly? As Boolean = Nothing) As iormObjectEntryDefinition
        ''' <summary>
        ''' returns an ormContainerEntryDefinition
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="containerid"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetContainerEntry(entryname As String, Optional containerid As String = Nothing, Optional runtimeOnly As Boolean? = Nothing) As iormContainerEntryDefinition
        ''' <summary>
        ''' returns a container definition
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetContainerDefinition(id As String) As iormContainerDefinition
    End Interface

    ''' <summary>
    ''' declares an orm objectdefinition
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormObjectDefinition
        Inherits iObjectDefinition

        ''' <summary>
        ''' gets or sets true if the Objects of this type will be cached
        ''' </summary>
        Property UseCache As Boolean

        Property HasDomainBehavior As Boolean

        Property HasDeleteFieldBehavior As Boolean

        Property PrimaryContainerID As String

        Property ContainerIDs As String()

        ReadOnly Property ObjectEntryDefinitions As IList(Of iormObjectEntryDefinition)

        Property RetrieveObjectFromViewID As String

        Function GetKeyEntries() As IList(Of iormObjectEntryDefinition)

        Function PrimaryKeyEntryNames() As String()

        Function GetEntries(Optional onlyActive As Boolean = True) As IList(Of iormObjectEntryDefinition)

        Function GetEntryDefinition(entryname As String) As iormObjectEntryDefinition

        Function HasEntry(entryname As String, Optional isActive As Boolean = True) As Boolean
    End Interface
    ''' <summary>
    ''' declares an orm object entry definition
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormObjectEntryDefinition
        Inherits iObjectEntryDefinition

        ''' <summary>
        ''' returns true if the Entry is mapped to a class member field -> moved to rulez
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Property IsMapped As Boolean


        ''' <summary>
        ''' gets the lower range Value -> moved to rulez
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Property LowerRangeValue() As Long?

       
        ''' <summary>
        ''' gets the upper range Value-> moved to rulez
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Property UpperRangeValue() As Long?


        ''' <summary>
        ''' gets the list of possible values-> moved to rulez
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Property PossibleValues() As List(Of String)

        ''' <summary>
        ''' Gets or sets the description.-> moved to rulez
        ''' </summary>
        ''' <value>The description.</value>
        'Property Description() As String

        ''' <summary>
        ''' sets or gets the object name of the entry-> moved to rulez
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Property Objectname() As String

        ''' <summary>
        ''' sets or gets the XchangeManager ID for the field 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property XID() As String

        ''' <summary>
        ''' returns the name of the entry-> moved to rulez
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Property Entryname() As String

        ''' <summary>
        ''' sets or gets the type otObjectEntryDefinitionType
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Typeid() As otObjectEntryType

        ''' <summary>
        ''' sets or gets true if this field is a spare field
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property IsSpareField() As Boolean

        '''' <summary>
        '''' returns the field data type-> moved to rulez
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Property Datatype() As otDataType
        ''' <summary>
        ''' returns version-> moved to rulez
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Property Version() As Long

        ''' <summary>
        ''' returns a array of aliases
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Aliases() As String()

        ''' <summary>
        ''' returns Title (Column Header)-> moved to rulez
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Property Title() As String

        ''' <summary>
        ''' sets or gets the default value for the object entry-> moved to rulez
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Property DefaultValue As Object

        ''' <summary>
        ''' returns True if the Entry is a Column
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property IsContainer As Boolean

        ''' <summary>
        ''' returns true if the Entry is a Compound entry
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property IsCompound As Boolean

        ''' <summary>
        ''' sets or gets the condition for dynamically looking up values
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property LookupCondition As String

        Property ValidationProperties As List(Of ObjectValidationProperty)

        Property ValidateRegExpression As String

        Property IsValidating As Boolean

        Property RenderProperties As List(Of RenderProperty)

        Property RenderRegExpMatch As String

        Property RenderRegExpPattern As String

        Property IsRendering As Boolean

        Property Properties As List(Of ObjectEntryProperty)

        Property Size As Long?


        ''' <summary>
        ''' gets or sets the Primary key Ordinal of the Object Entry (if set this is part of a key)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Property PrimaryKeyOrdinal As Short

        'Property InnerDatatype As otDataType? -> moved to rulez

        'Property Ordinal As Long -> moved to rulez

        'Property IsReadonly As Boolean -> moved to rulez

        'Property IsActive As Boolean -> moved to rulez

        Property LookupProperties As List(Of LookupProperty)

        Property LookupPropertyStrings As String()

        Property ValidationPropertyStrings As String()

        Property RenderPropertyStrings As String()

        Property PropertyStrings As String()

        ''' <summary>
        ''' gets or sets the category
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Category As String


        ''' <summary>
        ''' set the object entry by the attribute
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function SetByAttribute(attribute As iormObjectEntryAttribute) As Boolean

        ''' <summary>
        ''' handler for the OnSwitchRuntimeOff event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Sub OnswitchRuntimeOff(sender As Object, e As ormDataObjectEventArgs)

        ''' <summary>
        ''' gets the object definition
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetObjectDefinition() As iormObjectDefinition


    End Interface
    ''' <summary>
    ''' defines a general container attribute interface (container for persisting data objects to)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormContainerDefinition

        ''' <summary>
        ''' returns the container Type of the Container Attribute
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property ContainerType As otContainerType
        

        ''' <summary>
        ''' database driver stack
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property DatabaseDriverStack As Stack(Of iormDatabaseDriver)

        ''' <summary>
        ''' remove an index
        ''' </summary>
        ''' <param name="indexname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RemoveIndex(indexname As String) As Boolean
        ''' <summary>
        ''' returns true if the index exists
        ''' </summary>
        ''' <param name="indexname"></param>
        ''' <param name="onlyenabled"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HasIndex(indexname As String, Optional onlyenabled As Boolean = False) As Boolean
        ''' <summary>
        ''' retrieves the index attribute
        ''' </summary>
        ''' <param name="indexname"></param>
        ''' <param name="onlyenabled"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetIndex(indexname As String, Optional onlyenabled As Boolean = True) As ormIndexAttribute
        ''' <summary>
        ''' update the index attribute
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function UpdateIndex(index As ormIndexAttribute) As Boolean
        ''' <summary>
        ''' add index attribute
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function AddIndex(index As ormIndexAttribute) As Boolean
        ''' <summary>
        ''' adds a foreign key attribute
        ''' </summary>
        ''' <param name="entry"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function AddForeignKey(foreignkey As ormForeignKeyAttribute) As Boolean
        ''' <summary>
        ''' retrieves a foreign key attribute
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="enabledonly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetForeignKey(id As String, Optional enabledonly As Boolean = True) As ormForeignKeyAttribute
        ''' <summary>
        ''' removes a foreign key attribute from the container
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RemoveForeignKey(id As String) As Boolean
        ''' <summary>
        ''' returns true if the foreign key attribute exists
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="enabledonly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HasForeignKey(id As String, Optional enabledonly As Boolean = True) As Boolean

        ''' <summary>
        ''' Gets or sets the enabled.
        ''' </summary>
        ''' <value>The enabled.</value>
        Property Enabled() As Boolean

        ''' <summary>
        ''' Gets or sets the cache is active.
        ''' </summary>
        ''' <value>The is active.</value>
        Property UseCache() As Boolean



        ''' <summary>
        ''' Gets or sets the cache select.
        ''' </summary>
        ''' <value>cache.</value>
        Property CacheProperties() As String()

        ''' <summary>
        ''' true if there is a CacheProperty
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        ''' <summary>
        ''' id of the correlated database driver
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property PrimaryDatabaseDriverID As String

        ''' <summary>
        ''' returns true if database driver id is set
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        ''' <summary>
        ''' Add a member
        ''' </summary>
        ''' <param name="entry"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function AddEntry(entry As iormContainerEntryDefinition) As Boolean

        ''' <summary>
        ''' update an entry 
        ''' </summary>
        ''' <param name="entry"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function UpdateEntry(entry As iormContainerEntryDefinition) As Boolean

        ''' <summary>
        ''' returns an entry by entry name or nothing
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetEntry(entryname As String, Optional onlyenabled As Boolean = True) As iormContainerEntryDefinition

        ''' <summary>
        ''' returns true if an entryname exists
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HasEntry(entryname As String, Optional onlyenabled As Boolean = Nothing) As Boolean

        ''' <summary>
        ''' remove an entry by name 
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RemoveEntry(entryname As String) As Boolean

        ''' <summary>
        ''' returns a List of all Entries
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Entries() As IEnumerable(Of iormContainerEntryDefinition)

        ''' <summary>
        ''' sets or returns the Names of the PrimaryKey Columns
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property PrimaryEntryNames() As String()

        ''' <summary>
        ''' returns a List of all Entries
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property EntryNames() As IEnumerable(Of String)

        ''' <summary>
        ''' Gets or sets the description.
        ''' </summary>
        ''' <value>The description.</value>
        Property Description() As String

        ''' <summary>
        ''' returns true if the description has a value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        ''' <summary>
        ''' Gets or sets name of the Primary key 
        ''' </summary>
        ''' <value>The description.</value>
        Property PrimaryKey() As String
        ''' <summary>
        ''' Gets or sets the unique name of the container (such as tables).
        ''' </summary>
        ''' <value>The name of the table.</value>
        Property ContainerID() As String
        ''' <summary>
        ''' Gets or sets the add domain ID flag.
        ''' </summary>
        ''' <value>The add domain ID flag.</value>
        Property HasDomainBehavior() As Boolean

        ''' <summary>
        ''' Gets or sets the version.
        ''' </summary>
        ''' <value>The version.</value>
        Property Version() As Long



        ''' <summary>
        ''' sets or gets the add deletefield flag. This will add a field for deletion the record to the schema.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property HasDeleteFieldBehavior() As Boolean

        ''' <summary>
        ''' sets or gets the add ParameterField flag. 
        ''' This will add extra fields for additional parameters (reserve and spare) to the data object.
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property HasSpareFields() As Boolean


    End Interface
    ''' <summary>
    ''' defines the interface for a member of a container (to store the object entry)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormContainerEntryDefinition

        ''' <summary>
        ''' Gets or sets the enabled.
        ''' </summary>
        ''' <value>The enabled.</value>
        Property Enabled() As Boolean

        ''' <summary>
        ''' Gets or sets the name of the entry
        ''' </summary>
        ''' <value>The name of the Member.</value>
        Property EntryName() As String

        ''' <summary>
        ''' Gets or sets the reference object entry. Has the form [objectname].[entryname] 
        ''' such as Deliverable.constObjectID & "." & deliverable.constFNUID
        ''' </summary>
        ''' <value>The reference object entry.</value>
        Property ReferenceObjectEntry() As String

        ''' <summary>
        ''' Gets or sets the description.
        ''' </summary>
        ''' <value>The description.</value>
        Property Description() As String


        ''' <summary>
        ''' Gets or sets the pos ordinal.
        ''' </summary>
        ''' <value>The pos ordinal.</value>
        Property Posordinal() As Long


        ''' <summary>
        ''' Gets or sets the default value in DB presentation.
        ''' </summary>
        ''' <value>The default value.</value>
        Property DBDefaultValue() As String


        ''' <summary>
        ''' Gets or sets the container ID.
        ''' </summary>
        ''' <value>The table ID.</value>
        Property ContainerID() As String

        ''' <summary>
        ''' Gets or sets the Datatype.
        ''' </summary>
        ''' <value>The typeid.</value>
        Property DataType() As otDataType

        ''' <summary>
        ''' Gets or sets the nested inner Datatype of Datatype list.
        ''' </summary>
        ''' <value>The typeid.</value>
        'Property InnerDataType As otDataType?

        ''' <summary>
        ''' Gets or sets the size.
        ''' </summary>
        ''' <value>The size.</value>
        Property Size() As Long?

        ''' <summary>
        ''' Gets or sets the parameter.
        ''' </summary>
        ''' <value>The parameter.</value>
        Property Parameter() As String

        ''' <summary>
        ''' Gets or sets the is nullable.
        ''' </summary>
        ''' <value>The is nullable.</value>
        Property IsNullable() As Boolean

        ''' <summary>
        ''' Gets or sets the Unique Property.
        ''' </summary>
        ''' <value></value>
        Property IsUnique() As Boolean

        ''' <summary>
        ''' Gets or sets the primary key ordinal.
        ''' </summary>
        ''' <value>The primary key ordinal.</value>
        Property PrimaryKeyOrdinal() As Long


        ''' <summary>
        ''' gets or sets the version counter
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Version() As Long

        ''' <summary>
        ''' get or sets the relation descriptions of this entry by string
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Relation As String()

        ''' <summary>
        ''' sets or gets the ForeignKey properties string representation
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property ForeignKeyProperties As String()
        ''' <summary>
        ''' gets or sets the Foreign Key Property Array
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property ForeignKeyProperty As ForeignKeyProperty()
        ''' <summary>
        ''' gets or sets the foreign key reference
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property ForeignKeyReferences As String()
        ''' <summary>
        ''' gets or sets the UseForeignKey flag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Property UseForeignKey As otForeignKeyImplementation

    End Interface
    ''' <summary>
    ''' interface for a enumeration of data objects or ormResluts against the database
    ''' </summary>
    ''' <remarks>
    ''' design principles
    ''' 1. offer an interface independent on the query language for enumerating data objects or getting result by orm Record
    ''' </remarks>
    Public Interface iormQueriedEnumeration
        Inherits IEnumerable(Of iormRelationalPersistable)

        ''' <summary>
        ''' Event OnLoading is raised when the query execution is started
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnLoading(sender As Object, e As System.EventArgs)

        ''' <summary>
        ''' Event OnLoaded is raised when the query execution has ended
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnLoaded(sender As Object, e As System.EventArgs)

        ''' <summary>
        ''' Event OnAdding is raised when the query result set is extended
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnAdded(sender As Object, e As System.EventArgs)
        ''' <summary>
        ''' Event OnRemoving is raised when the query result set is reduced
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnRemoved(sender As Object, e As System.EventArgs)

        ''' <summary>
        ''' true if the query has run and a result is loaded
        ''' </summary>
        ''' <value></value>
        ReadOnly Property IsLoaded As Boolean

        ''' <summary>
        ''' load the query result by running the query against the database
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Load(Optional domainid As String = Nothing) As Boolean

        ''' <summary>
        ''' returns the primary Object Definition
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetObjectDefinition() As ormObjectDefinition
        ''' <summary>
        ''' remove the data object at position in the query result
        ''' </summary>
        ''' <param name="no"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RemoveObject(no As ULong) As Boolean
        ''' <summary>
        ''' adds a database object to the results of the query
        ''' </summary>
        ''' <param name="no"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function AddObject(dataobject As iormRelationalPersistable, Optional ByRef no As ULong? = Nothing) As Boolean
        ''' <summary>
        ''' returns the primary ClassDescription
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetObjectClassDescription() As ObjectClassDescription
        ''' <summary>
        ''' Gets the id of this queried enumeration.
        ''' </summary>
        ''' <value>The id.</value>
        ReadOnly Property ID As String
        ''' <summary>
        ''' gets or sets all object entry names of the query
        ''' </summary>
        ''' <param name="ordered"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property ObjectEntryNames As IList(Of String)
        ''' <summary>
        ''' Gets or sets the is objects enumerated flag - true if objects are going to be returned otherwise ormRecord could be returned
        ''' </summary>
        ''' <value>The is object enumerated.</value>
        Property AreObjectsEnumerated As Object

        ''' <summary>
        ''' returns a list of iormObjectEntry by name  returned by this Queried Enumeration
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetObjectEntry(name As String) As iormObjectEntryDefinition

        ''' <summary>
        ''' returns a list of iormObjectEntry entries returned by this Queried Enumeration
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetObjectEntries() As IList(Of iormObjectEntryDefinition)

        ''' <summary>
        ''' resets the result but not the query itself
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Reset() As Boolean
        ''' <summary>
        ''' returns the zero-based ormRecord of the qry result
        ''' </summary>
        ''' <param name="no"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetRecord(no As ULong) As ormRecord
        ''' <summary>
        ''' returns an infused object out of the zero-based number or results
        ''' </summary>
        ''' <param name="no"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetObject(no As ULong) As iormDataObject
        ''' <summary>
        ''' returns the size of the result list
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Count As ULong

        ''' <summary>
        ''' gets the value of a query parameter
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetValue(name As String, ByRef value As Object) As Boolean

        ''' <summary>
        ''' sets the value of query parameter
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function SetValue(name As String, value As Object) As Boolean

    End Interface
End Namespace
