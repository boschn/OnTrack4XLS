REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** Attributes Declaration
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
Imports OnTrack.Database
Imports OnTrack.Core


Namespace OnTrack.Database
    
    ''' <summary>
    ''' defines a general container attribute interface (container for persisting data objects to)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormContainerAttribute
        Inherits iormContainerDefinition


        ''' <summary>
        ''' Gets or sets the ID of the Attribute
        ''' </summary>
        ''' <value>The ID.</value>
        Property ID() As String

        ''' <summary>
        ''' Gets or sets the object ID.
        ''' </summary>
        ''' <value>The object ID.</value>
        Property ObjectID() As String

        ''' <summary>
        ''' returns an Inenumerale of all foreign key attributes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ForeignkeyAttributes As IEnumerable(Of ormForeignKeyAttribute)
        ''' <summary>
        ''' returns an inenumerable of all index attributes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property IndexAttributes As IEnumerable(Of ormIndexAttribute)
        
        ReadOnly Property HasValueContainerType As Boolean
       

        ''' <summary>
        ''' true if has value UseCache
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property HasValueUseCache As Boolean

        ''' <summary>
        ''' true if there is a CacheProperty
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property HasValueCacheProperties() As Boolean

        ''' <summary>
        ''' returns true if database driver id is set
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property HasValuePrimaryDatabaseDriverID As Boolean

        ''' <summary>
        ''' returns true if the description has a value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property HasValueDescription() As Boolean

        ''' <summary>
        ''' returns true if the primary name is set
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property HasValuePrimaryKey() As Boolean

        ReadOnly Property HasValueObjectID() As Boolean


        ReadOnly Property HasValueContainerID() As Boolean

        ReadOnly Property HasValueAddDomainBehavior() As Boolean


        ReadOnly Property HasValueVersion() As Boolean

        ReadOnly Property HasValueID() As Boolean

        
        ReadOnly Property HasValueDeleteFieldBehavior() As Boolean


        ReadOnly Property HasValueSpareFields() As Boolean

    End Interface
   

    ''' <summary>
    ''' defines the interface for a member of a container (to store the object entry)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormContainerEntryAttribute
        Inherits iormContainerEntryDefinition

        ''' <summary>
        ''' Gets or sets the ID.
        ''' </summary>
        ''' <value>The ID.</value>
        Property ID() As String
        
        ''' <summary>
        ''' true if the ID has a value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property HasValueID() As Boolean

        ReadOnly Property HasValueContainerEntryName() As Boolean

        ReadOnly Property HasValueReferenceObjectEntry() As Boolean

        ''' <summary>
        ''' true if the description has a value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property HasValueDescription() As Boolean

        ReadOnly Property HasValuePosOrdinal() As Boolean

        ReadOnly Property HasValueDBDefaultValue() As Boolean

        ReadOnly Property HasValueContainerID() As Boolean

        ReadOnly Property HasValueDataType() As Boolean

        ReadOnly Property HasValueSize() As Boolean

        ReadOnly Property HasValueParameter() As Boolean

        ReadOnly Property HasValueIsNullable() As Object

        ReadOnly Property HasValueIsUnique() As Object


        ''' <summary>
        ''' returns true if the primary key ordinal has a value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property HasValuePrimaryKeyOrdinal() As Boolean

        ''' <summary>
        ''' returns true if the version has a value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property HasValueVersion() As Boolean


        ReadOnly Property HasValueRelation As Boolean


        ReadOnly Property HasValueForeignKeyProperties As Boolean

        
        ReadOnly Property HasValueForeignKeyReferences As Boolean

        '
        ReadOnly Property HasValueUseForeignKey As Boolean




    End Interface

    ''' <summary>
    ''' defines the ObjectEntryAttribute
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormObjectEntryAttribute
        Inherits iormObjectEntryDefinition, iormContainerEntryAttribute

        ''' <summary>
        ''' True if ObjectEntry has a defined lower value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property HasValueLowerRange() As Boolean

        ''' <summary>
        ''' True if ObjectEntry has a defined upper value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property HasValueUpperRange() As Boolean

        ''' <summary>
        ''' gets the list of possible values
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property HasValuePossibleValues() As Boolean

        ''' <summary>
        ''' returns true if there is a dynamically lookup condition
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property HasValueLookupCondition As Boolean

        ReadOnly Property HasValueValidationProperties As Boolean

        ReadOnly Property HasValueValidateRegExpression As Boolean

        ReadOnly Property HasValueRenderProperties As Boolean

        ReadOnly Property HasValueLookupProperties As Boolean

        ReadOnly Property HasValueAliases As Boolean

        ReadOnly Property HasValueValidate As Boolean

        ReadOnly Property HasValueObjectName As Boolean

        ReadOnly Property HasValueEntryName As Boolean

        ReadOnly Property HasValueXID As Boolean

        ReadOnly Property HasValueIsSpareField As Boolean

        ReadOnly Property HasValueCategory As Boolean

        ReadOnly Property hasValueTitle As Boolean

        ReadOnly Property HasValueIsReadonly As Boolean

        ReadOnly Property HasValueIsActive As Boolean
        ReadOnly Property HasValueObjectEntryProperties As Boolean

        ReadOnly Property HasValueRenderRegExpPattern As Boolean
        ReadOnly Property HasValueRenderRegExprMatch As Boolean

        ReadOnly Property HasValueIsRendering As Boolean

        ReadOnly Property HasValueInnerDatatype As Boolean

    End Interface
End Namespace