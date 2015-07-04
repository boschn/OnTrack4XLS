REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** Object Relationship Model Driver Declaration
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
Imports OnTrack.rulez
Imports OnTrack.rulez.eXPressionTree


Namespace OnTrack.Database

    Public Enum otContainerType
        Table = 1
        EmbeddedObject = 2
    End Enum
    ''' <summary>
    ''' type of database server (OLEDB might be access or sql server)
    ''' </summary>
    ''' <remarks></remarks>
    'Public Enum otDBServerType
    '    Access = 1
    '    SQLServer = 2
    'End Enum


    ''' <summary>
    ''' defines the interface for a primary database (which can host the onTrack Database as primary) 
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormOnTrackDriver
        Inherits iormDatabaseDriver

        ''' <summary>
        ''' creates the UserDefinition Table
        ''' </summary>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function CreateDBUserDefTable(Optional ByRef nativeConnection As Object = Nothing) As Boolean



        ''' <summary>
        ''' validates the User against the Database with a accessrequest
        ''' </summary>
        ''' <param name="username"></param>
        ''' <param name="password"></param>
        ''' <param name="accessRequest"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ValidateUser(username As String, _
                              password As String, _
                              accessRequest As otAccessRight, _
                              Optional domainid As String = Nothing) As Boolean

        ''' <summary>
        ''' creates a global domain in the domain objects
        ''' </summary>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function CreateGlobalDomain(Optional ByRef nativeConnection As Object = Nothing) As Boolean

        ''' <summary>
        ''' returns true if an Admin User Validation exists in the database (e.g. a otdb with admin rights exists)
        ''' </summary>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HasAdminUserValidation(Optional ByRef nativeConnection As Object = Nothing) As Boolean

        ''' <summary>
        ''' Install the OnTrackDatabase
        ''' </summary>
        ''' <param name="askBefore"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function InstallOnTrackDatabase(askBefore As Boolean, modules As String()) As Boolean

        ''' <summary>
        ''' event triggered by the request for a bootstrap installation
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event RequestBootstrapInstall(sender As Object, e As SessionBootstrapEventArgs)

        ''' <summary>
        ''' Persist the Session or ErrorLog
        ''' </summary>
        ''' <param name="log"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function PersistLog(ByRef log As OnTrack.Core.SessionMessageLog) As Boolean

        ''' <summary>
        ''' verify OnTrack if Data Objects are there and up to date
        ''' </summary>
        ''' <returns>true if OnTrack is ok</returns>
        ''' <remarks></remarks>
        Function VerifyOnTrackDatabase(Optional modules As String() = Nothing, _
                                       Optional install As Boolean = False, _
                                       Optional verifySchema As Boolean = False) As Boolean
        ''' <summary>
        ''' gets a user validation structure from the DB
        ''' </summary>
        ''' <param name="username"></param>
        ''' <param name="selectAnonymous"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetUserValidation(ByVal username As String, _
                                   Optional ByVal selectAnonymous As Boolean = False, _
                                   Optional ByRef nativeConnection As Object = Nothing) As UserValidation

    End Interface


    ''' <summary>
    ''' defines a abstract database driver
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormDatabaseDriver
        ''' <summary>
        ''' returns true if driver is supporting OnTrack Hosting as primary database
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property IsOnTrackDriver As Boolean

        ''' <summary>
        ''' returns true if driver is supporting a relational database
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property IsRelationalDriver As Boolean

        ''' <summary>
        ''' returns the default target type for a OTDB FieldType - MAPPING
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetTargetTypeFor(type As otDataType) As Long

        ''' <summary>
        ''' convert a value to column data type
        ''' </summary>
        ''' <param name="value">value</param>
        ''' <param name="targetType">target data type of the native driver</param>
        ''' <param name="maxsize">optional max size of string / text</param>
        ''' <param name="abostrophNecessary">optional true if abostrop in sql necessary</param>
        ''' <param name="columnname">optional columnname to use on error handling</param>
        ''' <returns>the converted object</returns>
        ''' <remarks></remarks>
        Function Convert2DBData(ByVal invalue As Object, ByRef outvalue As Object, targetType As Long, _
                                Optional ByVal maxsize As Long = 0, _
                                Optional ByRef abostrophNecessary As Boolean = False, _
                                Optional ByVal columnname As String = Nothing, _
                                Optional isnullable As Boolean = False, _
                                Optional defaultvalue As Object = Nothing) As Boolean
        ''' <summary>
        ''' convert an otdb data object to a native .net object data type
        ''' </summary>
        ''' <param name="invalue"></param>
        ''' <param name="outvalue"></param>
        ''' <param name="sourceType"></param>
        ''' <param name="isnullable"></param>
        ''' <param name="defaultvalue"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Convert2ObjectData(invalue As Object, _
                                    ByRef outvalue As Object, _
                                    sourceType As Long, _
                                    Optional isnullable As Boolean? = Nothing, _
                                    Optional defaultvalue As Object = Nothing, _
                                    Optional ByRef abostrophNecessary As Boolean = False) As Boolean

        ''' <summary>
        ''' gets or sets the session
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Session As Session

        ''' <summary>
        ''' gets or sets the id of the database driver instance
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property ID As String

        ''' <summary>
        ''' gets the static name of the database driver 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Name As String

        ''' <summary>
        ''' gets the name of the native database product 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property NativeDatabaseName As String

        ''' <summary>
        ''' get the version of the native database product 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property NativeDatabaseVersion As String

        ''' <summary>
        ''' gets a connection object to the relational database
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property CurrentConnection As iormConnection



        ''' <summary>
        ''' registers a connection at the relational Database Driver
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RegisterConnection(ByRef connection As iormConnection) As Boolean

        ''' <summary>
        ''' gets or sets the container version
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property ContainerVersion(id As String) As Long?

        ''' <summary>
        ''' deletes the version of the container id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function DropContainerVersion(id As String) As Boolean

        ''' <summary>
        ''' returns the Parameter Table name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property DBParameterContainerName As String

        ''' <summary>
        ''' creates the DB parameter table
        ''' </summary>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function CreateDBParameterContainer(Optional ByRef nativeConnection As Object = Nothing) As Boolean

        ''' <summary>
        ''' drops the DB parameter table - given with setup then just the setup related entries
        ''' if then there is no setup related entries at all -> drop the full table
        ''' </summary>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function DropDBParameterContainer(Optional setupid As String = Nothing, Optional ByRef nativeConnection As Object = Nothing) As Boolean

        ''' <summary>
        ''' deletes a DB Parameter
        ''' </summary>
        ''' <param name="parametername"></param>
        ''' <param name="nativeConnection"></param>
        ''' <param name="silent"></param>
        ''' <param name="setupID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function DeleteDBParameter(parametername As String,
                                    Optional ByRef nativeConnection As Object = Nothing, _
                                    Optional silent As Boolean = False, _
                                    Optional setupID As String = Nothing) As Boolean
        ''' <summary>
        ''' sets a db parameter
        ''' </summary>
        ''' <param name="parametername"></param>
        ''' <param name="value"></param>
        ''' <param name="nativeConnection"></param>
        ''' <param name="updateOnly"></param>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function SetDBParameter(ByVal parametername As String, _
                                ByVal value As Object, _
                                Optional ByRef nativeConnection As Object = Nothing, _
                                Optional ByVal updateOnly As Boolean = False, _
                                Optional ByVal silent As Boolean = False, _
                                Optional setupID As String = Nothing, _
                                Optional description As String = Nothing) As Boolean

        ''' <summary>
        ''' returns a DB parameter value
        ''' </summary>
        ''' <param name="parametername"></param>
        ''' <param name="nativeConnection"></param>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetDBParameter(parametername As String, _
                                Optional ByRef nativeConnection As Object = Nothing, _
                                Optional silent As Boolean = False, _
                                Optional setupID As String = Nothing) As Object



        ''' <summary>
        ''' returns a container store for a container id
        ''' </summary>
        ''' <param name="tableID"></param>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RetrieveContainerStore(ByVal containerid As String, Optional ByVal force As Boolean = False) As iormContainerStore


        ''' <summary>
        ''' returns the native name of the foreign key id in the database
        ''' </summary>
        ''' <param name="foreignkeyid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetNativeForeignKeyName(foreignkeyid As String) As String

        ''' <summary>
        ''' returns the native name of the index in the database
        ''' </summary>
        ''' <param name="indexid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetNativeIndexName(indexid As String) As String

        ''' <summary>
        ''' returns an id in a native object name of the database
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetNativeDBObjectName(id As String) As String

        ''' <summary>
        ''' returns or creates foreign keys in the database
        ''' </summary>
        ''' <param name="nativeTable"></param>
        ''' <param name="columndefinition"></param>
        ''' <param name="createOrAlter"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetForeignKeys(nativeContainerObject As Object, _
                                foreignkeydefinition As ormForeignKeyDefinition, _
                                Optional createOrAlter As Boolean = False, _
                                Optional ByRef connection As iormConnection = Nothing) As IEnumerable(Of Object)



        ''' <summary>
        ''' creates or retrieves an index out of a indexdefinition
        ''' </summary>
        ''' <param name="nativeTable"></param>
        ''' <param name="indexdefinition"></param>
        ''' <param name="forceCreation"></param>
        ''' <param name="createOrAlter"></param>
        ''' <param name="connection"></param>
        ''' <returns>native index object</returns>
        ''' <remarks></remarks>
        Function GetIndex(ByRef nativeTable As Object, _
                          ByRef indexdefinition As ormIndexDefinition,
                          Optional forceCreation As Boolean = False, _
                          Optional createOrAlter As Boolean = False, _
                          Optional ByRef connection As iormConnection = Nothing) As Object


        ''' <summary>
        ''' returns true if the database has the table
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HasContainerID(ByVal containerID As String, _
                          Optional ByRef connection As iormConnection = Nothing, _
                          Optional nativeConnection As Object = Nothing) As Boolean

        ''' <summary>
        ''' returns a TableSchema Object
        ''' </summary>
        ''' <param name="tableID"></param>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RetrieveContainerSchema(ByVal containerID As String, Optional ByVal force As Boolean = False) As iormContainerSchema


        ''' <summary>
        ''' verifies the tabledefinition with the existing table definition in the data base
        ''' returns true if both are the same
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function VerifyContainerSchema(containerDefinition As iormContainerDefinition, _
                                   Optional ByRef connection As iormConnection = Nothing, _
                                   Optional nativeConnection As Object = Nothing) As Boolean

        ''' <summary>
        ''' alters or creates a Container object in the data base
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="createOrAlter"></param>
        ''' <param name="addToSchemaDir"></param>
        ''' <param name="nativeConnection"></param>
        ''' <param name="tableNativeObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetContainerObject(ByVal id As String, _
                Optional ByVal createOrAlter As Boolean = False, _
                Optional ByRef connection As iormConnection = Nothing, _
                Optional ByRef nativeContainerObject As Object = Nothing) As Object

        ''' <summary>
        ''' drops a table in the database by id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function DropContainerObject(ByVal id As String, Optional ByRef connection As iormConnection = Nothing) As Boolean

        ''' <summary>
        ''' returns true if the data store has the columnname in the table
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="columnname"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HasContainerEntryID(containerID As String, _
                                     entryID As String, _
                                    Optional ByRef connection As iormConnection = Nothing) As Boolean
        ''' <summary>
        ''' returns true if the data store has the column definition in the table
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="columnname"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function VerifyContainerEntrySchema(containerEntryDefinition As iormContainerEntryDefinition, _
                                    Optional ByRef connection As iormConnection = Nothing, _
                                    Optional silent As Boolean = False) As Boolean


        ''' <summary>
        ''' returns or creates a column in the data store
        ''' </summary>
        ''' <param name="nativeTable"></param>
        ''' <param name="aDBDesc"></param>
        ''' <param name="createOrAlter"></param>
        ''' <param name="addToSchemaDir"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetContainerEntryObject(nativeContainerObject As Object, _
                                         containerEntryDefinition As iormContainerEntryDefinition, _
                           Optional ByVal createOrAlter As Boolean = False, _
                           Optional ByRef connection As iormConnection = Nothing) As Object

        ''' <summary>
        ''' returns a new Visitor Object for the DataBase
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetIXPTVisitor() As IVisitor

        ''' <summary>
        ''' retrieve records by running a selection rule
        ''' </summary>
        ''' <param name="selectionrule"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RetrieveBy(selectionrule As OnTrack.rulez.eXPressionTree.SelectionRule, context As Context) As IList(Of ormRecord)
        ''' <summary>
        ''' prepares a selection rule
        ''' </summary>
        ''' <param name="rule"></param>
        ''' <param name="resultCode"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function PrepareSelection(rule As SelectionRule, ByRef resultCode As rulez.ICodeBit) As Boolean
    End Interface

    ''' <summary>
    ''' defines a relational primary onTrack database driver
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormRelationalDatabaseDriver
        Inherits iormOnTrackDriver

        ''' <summary>
        ''' returns a data view schema by id
        ''' </summary>
        ''' <param name="viewID"></param>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetViewSchema(viewID As String, Optional force As Boolean = False) As iormContainerSchema

        ''' <summary>
        ''' returns a data view reader
        ''' </summary>
        ''' <param name="viewID"></param>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetViewReader(viewID As String, Optional force As Boolean = False) As iormRelationalTableStore



        ''' <summary>
        ''' returns the native name of the view id in the relational database
        ''' </summary>
        ''' <param name="viewid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetNativeViewName(viewid As String) As String

        ''' <summary>
        ''' create or reuse and return a ORM SQL Command by ID
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function CreateSqlCommand(id As String) As iormSqlCommand

        ''' <summary>
        ''' create or reuse and return a ormSqlSelectCommand by ID
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function CreateSqlSelectCommand(id As String) As iormSqlCommand

        ''' <summary>
        ''' run a sql command 
        ''' </summary>
        ''' <param name="sqlcommand"></param>
        ''' <param name="parametervalues"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RunSqlCommand(ByRef sqlcommand As ormSqlCommand, _
                               Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing, _
                               Optional nativeConnection As Object = Nothing) As Boolean

        ''' <summary>
        ''' returns true if the datastore has the view by viewname
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HasView(ByVal name As String, _
                         Optional ByRef connection As iormConnection = Nothing, _
                         Optional nativeConnection As Object = Nothing) As Boolean

        ''' <summary>
        ''' returns or creates a Table in the data store
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="createOrAlter"></param>
        ''' <param name="addToSchemaDir"></param>
        ''' <param name="nativeConnection"></param>
        ''' <param name="tableNativeObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetView(ByVal name As String, Optional sqlselect As String = Nothing, _
                Optional ByVal createOrAlter As Boolean = False, _
                Optional ByRef connection As iormConnection = Nothing) As Object

        ''' <summary>
        ''' drops a view by id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function DropView(ByVal id As String, _
                          Optional ByRef connection As iormConnection = Nothing) As Boolean

        ''' <summary>
        ''' returns true if the data store has the columnname in the table
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="columnname"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HasTable(tableid As String, Optional ByRef connection As iormConnection = Nothing, Optional nativeConnection As Object = Nothing) As Boolean

        ''' <summary>
        ''' verifies the tabledefinition with the existing table definition in the data base
        ''' returns true if both are the same
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function VerifyTableSchema(tabledefinition As ormContainerDefinition, _
                                   Optional ByRef connection As iormConnection = Nothing, _
                                   Optional nativeConnection As Object = Nothing) As Boolean

        ''' <summary>
        ''' returns a TableSchema Object
        ''' </summary>
        ''' <param name="tableID"></param>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RetrieveTableSchema(ByVal tableID As String, Optional ByVal force As Boolean = False) As iormContainerSchema


        ''' <summary>
        ''' verifies the tabledefinition with the existing table definition in the data base
        ''' returns true if both are the same
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function VerifyContainerSchema(containerAttribute As ormTableAttribute, _
                                   Optional ByRef connection As iormConnection = Nothing, _
                                   Optional nativeConnection As Object = Nothing) As Boolean

        ''' <summary>
        ''' alters or creates a Container object in the data base
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="createOrAlter"></param>
        ''' <param name="addToSchemaDir"></param>
        ''' <param name="nativeConnection"></param>
        ''' <param name="tableNativeObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetTable(ByVal tableid As String, _
                Optional ByVal createOrAlter As Boolean = False, _
                Optional ByRef connection As iormConnection = Nothing, _
                Optional ByRef nativeContainerObject As Object = Nothing) As Object

        ''' <summary>
        ''' drops a table in the database by id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function DropTable(ByVal tableid As String, Optional ByRef connection As iormConnection = Nothing) As Boolean

        ''' <summary>
        ''' returns true if the data store has the columnname in the table
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="columnname"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HasColumn(tableID As String, _
                           columnname As String, _
                           Optional ByRef connection As iormConnection = Nothing) As Boolean

        ''' <summary>
        ''' returns true if the data store has the column table attribute in the table
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="columnname"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function VerifyColumnSchema(attribute As iormContainerEntryDefinition, _
                                    Optional ByRef connection As iormConnection = Nothing, _
                                    Optional silent As Boolean = False) As Boolean

        ''' <summary>
        ''' returns or creates a column in the data store
        ''' </summary>
        ''' <param name="nativeTable"></param>
        ''' <param name="aDBDesc"></param>
        ''' <param name="createOrAlter"></param>
        ''' <param name="addToSchemaDir"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetColumn(nativeTable As Object, columndefinition As iormContainerEntryDefinition, _
                           Optional ByVal createOrAlter As Boolean = False, _
                           Optional ByRef connection As iormConnection = Nothing) As Object




        ''' <summary>
        ''' returns a Tablestore Object
        ''' </summary>
        ''' <param name="tableID"></param>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetTableStore(ByVal tableID As String, Optional ByVal force As Boolean = False) As iormRelationalTableStore


        ''' <summary>
        ''' runs a sql statement against the database
        ''' </summary>
        ''' <param name="sqlcmdstr"></param>
        ''' <param name="parameters"></param>
        ''' <param name="silent"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RunSqlStatement(ByVal sqlcmdstr As String, _
                                 Optional ByRef parameters As List(Of ormSqlCommandParameter) = Nothing, _
                                 Optional silent As Boolean = True, _
                                 Optional nativeConnection As Object = Nothing) As Boolean

        ''' <summary>
        ''' run a Select Command and return the List of Records
        ''' </summary>
        ''' <param name="sqlcommand">a clsOTDBSqlSelectCommand</param>
        ''' <param name="parameters">optional list of Parameters for the values</param>
        ''' <param name="nativeConnection">optional native Connection</param>
        ''' <returns>list of clsOTDBRecords</returns>
        ''' <remarks></remarks>
        Function RunSqlSelectCommand(ByRef sqlcommand As ormSqlSelectCommand, _
                                    Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing, _
                                    Optional nativeConnection As Object = Nothing) As List(Of ormRecord)

        Function RunSqlSelectCommand(id As String, _
                                        Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing, _
                                        Optional nativeConnection As Object = Nothing) As List(Of ormRecord)
        ''' <summary>
        ''' checks if SqlCommand is in Store of the driver
        ''' </summary>
        ''' <param name="id">id of the command</param>
        ''' <returns>True if successful</returns>
        ''' <remarks></remarks>
        Function HasSqlCommand(id As String) As Boolean
        ''' <summary>
        ''' Store the Command by its ID - replace if existing
        ''' </summary>
        ''' <param name="sqlCommand">a iOTDBSqlCommand</param>
        ''' <returns>true if successfull</returns>
        ''' <remarks></remarks>
        Function StoreSqlCommand(ByRef sqlCommand As iormSqlCommand) As Boolean
        ''' <summary>
        ''' Retrieve the Command from Store
        ''' </summary>
        ''' <param name="id">id of the command</param>
        ''' <returns>a iOTDBSqlCommand</returns>
        ''' <remarks></remarks>
        Function RetrieveSqlCommand(id As String) As iormSqlCommand
        ''' <summary>
        ''' Creates a native DB Command
        ''' </summary>
        ''' <param name="p1">Command name</param>
        ''' <param name="aNativeConnection"></param>
        ''' <returns>a idbcommand</returns>
        ''' <remarks></remarks>
        Function CreateNativeDBCommand(p1 As String, nativeConnection As Data.IDbConnection) As Data.IDbCommand
        ''' <summary>
        ''' creates and assigns a native DB Paramter by otdb datatype
        ''' </summary>
        ''' <param name="parametername"></param>
        ''' <param name="datatype"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function AssignNativeDBParameter(parametername As String, _
                                         datatype As otDataType, _
                                         Optional maxsize As Long = 0, _
                                         Optional value As Object = Nothing) As System.Data.IDbDataParameter



        ''' <summary>
        ''' returns a new Visitor Object for the DataBase
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetIRDBVisitor() As IRDBVisitor
    End Interface

    ''' <summary>
    ''' defines an relational XPT Visitor
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IRDBVisitor
        Inherits IVisitor

        ''' <summary>
        ''' gets the parameters of the sql query
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Parameters As List(Of ormSqlCommandParameter)
           
    ''' <summary>
    ''' gets a list of table ids
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
        ReadOnly Property TableIDs As List(Of String)

        ''' <summary>
        ''' returns the result
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Result() As String

        Function [Select]() As String

    End Interface
        ''' <summary>
        ''' a abstract store for persisting objects of the same type from / to an record object
        ''' </summary>
        ''' <remarks></remarks>
    Public Interface iormContainerStore
        ''' <summary>
        ''' returns the native database object name for the store
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property NativeDBObjectname As String

        ''' <summary>
        ''' sets or gets the connection to the database
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Connection As iormConnection
        ''' <summary>
        ''' sets or gets the schema class for this tablestore
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property ContainerSchema As iormContainerSchema
        ''' <summary>
        ''' set or gets the ID (name) of the table
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property ContainerID As String
        ''' <summary>
        ''' returns true if the tablestore supports Linq
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property IsLinqAvailable As Boolean
        ''' <summary>
        ''' returns a new unique key value
        ''' </summary>
        ''' <param name="pkArray">sets or fills this array</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function CreateUniquePkValue(ByRef pkArray() As Object, Optional tag As String = Nothing) As Boolean
        ''' <summary>
        ''' Refresh the data of the tablestore
        ''' </summary>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Refresh(Optional ByVal force As Boolean = False) As Boolean
        ''' <summary>
        ''' deletes the data record in the native container by primary key array
        ''' </summary>
        ''' <param name="aKeyArr"></param>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function DeleteRecordByPrimaryKey(ByRef pkArray() As Object, Optional silent As Boolean = False) As Boolean

        ''' <summary>
        ''' retrieves a clsOTDBRecord by primary key arrary
        ''' </summary>
        ''' <param name="aKeyArr"></param>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetRecordByPrimaryKey(ByRef pkArray() As Object, Optional silent As Boolean = False) As ormRecord

        ''' <summary>
        ''' returns a collection of clsotdbrecord by an named index / view and keys Array in the datastore
        ''' </summary>
        ''' <param name="indexname"></param>
        ''' <param name="keyArray"></param>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetRecordsByIndex(indexname As String, _
                                   ByRef keyArray() As Object,
                                   Optional silent As Boolean = False) As List(Of ormRecord)

        ''' <summary>
        ''' return a ist of records by a selection rule
        ''' </summary>
        ''' <param name="aSelection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetRecordsBySelectionRule(rule As SelectionRule) As IEnumerable(Of ormRecord)

        ''' <summary>
        ''' infuses a record from the ContainerStore by native Object
        ''' </summary>
        ''' <param name="newRecord"></param>
        ''' <param name="rowObject"></param>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function InfuseRecord(ByRef newRecord As ormRecord, _
                              ByRef rowObject As Object,
                              Optional ByVal silent As Boolean = False,
                              Optional createNewRecord As Boolean = False) As Boolean

        ''' <summary>
        ''' persists a record in the Container
        ''' </summary>
        ''' <param name="record"></param>
        ''' <param name="timestamp"></param>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function PersistRecord(ByRef record As ormRecord,
                               Optional ByVal timestamp As Date = ot.ConstNullDate,
                               Optional ByVal silent As Boolean = False) As Boolean

        ''' <summary>
        ''' convert a value to container data type
        ''' </summary>
        ''' <param name="value">value</param>
        ''' <param name="targetType">target data type of the native driver</param>
        ''' <param name="maxsize">optional max size of string / text</param>
        ''' <param name="abostrophNecessary">optional true if abostrop in sql necessary</param>
        ''' <param name="columnname">optional columnname to use on error handling</param>
        ''' <returns>the converted object</returns>
        ''' <remarks></remarks>
        Function Convert2ContainerData(ByVal invalue As Object, ByRef outvalue As Object, _
                                    targetType As Long, _
                                    Optional ByVal maxsize As Long = 0, _
                                    Optional ByRef abostrophNecessary As Boolean = False, _
                                    Optional ByVal columnname As String = Nothing, _
                                    Optional isnullable As Boolean? = Nothing, _
                                    Optional defaultvalue As Object = Nothing) As Boolean
        ''' <summary>
        ''' convert a value to data type of the container entry data type
        ''' </summary>
        ''' <param name="index">column name</param>
        ''' <param name="value">value </param>
        ''' <param name="abostrophNecessary">true if abostrop in sql necessary</param>
        ''' <returns>converted value</returns>
        ''' <remarks></remarks>
        Function Convert2ContainerData(ByVal index As Object, _
                                    ByVal invalue As Object, ByRef outvalue As Object, _
                                    Optional ByRef abostrophNecessary As Boolean = False, _
                                    Optional isnullable As Boolean? = Nothing, _
                                    Optional defaultvalue As Object = Nothing) As Boolean

        ''' <summary>
        ''' convert container entry data to .net object
        ''' </summary>
        ''' <param name="index"></param>
        ''' <param name="value"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Convert2ObjectData(ByVal index As Object, _
                                    ByVal invalue As Object, ByRef outvalue As Object, _
                                    Optional isnullable As Boolean? = Nothing, _
                                    Optional defaultvalue As Object = Nothing, _
                                    Optional ByRef abostrophNecessary As Boolean = False) As Boolean

        ''' <summary>
        ''' returns true if the data store has the named property
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HasProperty(ByVal name As String) As Boolean
        ''' <summary>
        ''' returns the Property by name
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetProperty(ByVal name As String) As Object
        ''' <summary>
        ''' sets the property by name for the tablestore
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function SetProperty(ByVal name As String, ByVal value As Object) As Boolean



    End Interface

    ''' <summary>
    ''' defines an interface for persistency classes which are able to persist clsOTDBRecord 
    ''' through an iotdbconnection object
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormRelationalTableStore
        Inherits iormContainerStore

        ''' <summary>
        ''' get the records by using a predefined orm sql command
        ''' </summary>
        ''' <param name="sqlcommand"></param>
        ''' <param name="parametervalues"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetRecordsBySqlCommand(ByRef sqlcommand As ormSqlSelectCommand, _
                                        Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing) As List(Of ormRecord)


        ''' <summary>
        ''' returns records by building a sql command
        ''' </summary>
        ''' <param name="wherestr"></param>
        ''' <param name="fullsqlstr"></param>
        ''' <param name="innerjoin"></param>
        ''' <param name="orderby"></param>
        ''' <param name="silent"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Function GetRecordsBySql(ByVal wherestr As String,
                                 Optional ByVal fullsqlstr As String = Nothing,
                                 Optional ByVal innerjoin As String = Nothing, _
                                Optional ByVal orderby As String = Nothing,
                                Optional ByVal silent As Boolean = False, _
                                Optional ByRef parameters As List(Of ormSqlCommandParameter) = Nothing) As List(Of ormRecord)

        ''' <summary>
        ''' retrieves a collection of records by retrieving or creating a sql command from the data store
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <param name="wherestr"></param>
        ''' <param name="fullsqlstr"></param>
        ''' <param name="innerjoin"></param>
        ''' <param name="orderby"></param>
        ''' <param name="silent"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetRecordsBySqlCommand(ByVal id As String, _
                                    Optional ByVal wherestr As String = Nothing, _
                                    Optional ByVal fullsqlstr As String = Nothing, _
                                    Optional ByVal innerjoin As String = Nothing, _
                                    Optional ByVal orderby As String = Nothing, _
                                    Optional ByVal silent As Boolean = False, _
                                    Optional ByRef parameters As List(Of ormSqlCommandParameter) = Nothing) As List(Of ormRecord)




        ''' <summary>
        ''' runs a plain sql statement
        ''' </summary>
        ''' <param name="sqlcmdstr"></param>
        ''' <param name="parameters"></param>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RunSqlStatement(ByVal sqlcmdstr As String, Optional ByRef parameters As List(Of ormSqlCommandParameter) = Nothing, Optional silent As Boolean = True) As Boolean

        ''' <summary>
        ''' runs a sql command 
        ''' </summary>
        ''' <param name="command"></param>
        ''' <param name="parametervalues"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RunSqlCommand(ByRef command As ormSqlCommand, Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing) As Boolean

        ''' <summary>
        ''' checks if SqlCommand is in Store of the driver
        ''' </summary>
        ''' <param name="id">id of the command</param>
        ''' <returns>True if successful</returns>
        ''' <remarks></remarks>
        Function HasSqlCommand(id As String) As Boolean
        ''' <summary>
        ''' Store the Command by its ID - replace if existing
        ''' </summary>
        ''' <param name="sqlCommand">a iOTDBSqlCommand</param>
        ''' <returns>true if successfull</returns>
        ''' <remarks></remarks>
        Function StoreSqlCommand(ByRef sqlCommand As iormSqlCommand) As Boolean
        ''' <summary>
        ''' Retrieve the Command from Store
        ''' </summary>
        ''' <param name="id">id of the command</param>
        ''' <returns>a iOTDBSqlCommand</returns>
        ''' <remarks></remarks>
        Function RetrieveSqlCommand(id As String) As iormSqlCommand
        ''' <summary>
        ''' Retrieve the Command from Store or create new command
        ''' </summary>
        ''' <param name="id">id of the command</param>
        ''' <returns>a iOTDBSqlCommand</returns>
        ''' <remarks></remarks>
        Function CreateSqlCommand(id As String) As iormSqlCommand
        ''' <summary>
        ''' Retrieve the Command from Store or create a new Select Command
        ''' </summary>
        ''' <param name="id">id of the command</param>
        ''' <returns>a iOTDBSqlCommand</returns>
        ''' <remarks></remarks>
        Function CreateSqlSelectCommand(id As String, Optional addMe As Boolean = True, Optional addAllFields As Boolean = True) As iormSqlCommand
    End Interface



    ''' <summary>
    ''' sql command types
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otSQLCommandTypes
        [SELECT] = 1
        UPDATE
        INSERT
        DELETE
    End Enum

    ''' <summary>
    ''' a sql command description
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormSqlCommand

        Function HasParameter(ID As String) As Boolean

        Property ID As String
        ReadOnly Property TableIDs As List(Of String)
        ReadOnly Property [Type] As otSQLCommandTypes
        Property CustomerSqlStatement As String
        ReadOnly Property BuildVersion As UShort
        ReadOnly Property SqlText As String
        Property NativeCommand As System.Data.IDbCommand

        ReadOnly Property Parameters As List(Of ormSqlCommandParameter)

        Function AddParameter(parameter As ormSqlCommandParameter) As Boolean
        Function SetParameterValue(ID As String, value As Object) As Boolean
        Function GetParameterValue(ID As String) As Object
        Function Prepare() As Boolean

    End Interface


    ''' <summary>
    ''' interface for a native container schema for a data store
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormContainerSchema

        ''' <summary>
        ''' returns the native database object name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property NativeDBContainerName As String

        ''' <summary>
        ''' returns true if the name or index number of column is nullable
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetNullable(index As Object) As Boolean

        ''' <summary>
        ''' associated table id of the schema
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ContainerID As String
        ''' <summary>
        ''' True if Schema is read and initialized
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property IsInitialized() As Boolean
        ''' <summary>
        ''' all Indices's as list
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Indices As List(Of String)

        ''' <summary>
        ''' returns the primary key names
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property PrimaryEntryNames As List(Of String)

        ''' <summary>
        ''' refresh loads the schema
        ''' </summary>
        ''' <param name="reloadForce"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Refresh(Optional reloadForce As Boolean = False) As Boolean
        ''' <summary>
        ''' gets the name of the primary key
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property PrimaryKeyIndexName As String
        ''' <summary>
        ''' gets the columnname ordinals in the schema
        ''' </summary>
        ''' <param name="anIndex"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetEntryOrdinal(index As Object) As Integer

        '**** return columnnames as Collection
        '****
        ''' <summary>
        ''' all columnnames in the schema as List
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property EntryNames() As List(Of String)

        '** return columnname by index 
        '** Nothing if out of range
        ''' <summary>
        ''' return the columnname by ordinal
        ''' </summary>
        ''' <param name="i"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetEntryName(ByVal i As Integer) As String
        ''' <summary>
        ''' true if the columnname exists in the primary key
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HasPrimaryEntryName(ByRef name As String) As Boolean
        '*** check if columnname by Name exists
        ''' <summary>
        ''' true if the columnname exists in the schema
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HasEntryName(ByVal name As String) As Boolean

        ''' <summary>
        ''' returns the ordinal number of the domainID in the primary key array - less zero if not in the primary key
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetDomainIDPKOrdinal() As Integer

        ''' <summary>
        ''' returns the Default Value for a columnname
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetDefaultValue(ByVal index As Object) As Object

        ''' <summary>
        ''' returns the if there is a Default Value for a columnname
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HasDefaultValue(ByVal index As Object) As Boolean

        '**** get the Primary Key columnname by Index i
        '***  returns nothing if there is none
        ''' <summary>
        ''' get the Primary Key columnname by Index i.returns nothing if there is none
        ''' </summary>
        ''' <param name="i"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetPrimaryEntryNames(i As UShort) As String

        '**** get the Primary Key columnname no by field index i
        '***  returns -1 if there is none
        ''' <summary>
        '''  get the Primary Key columnname no by field index i.  returns -1 if there is none
        ''' </summary>
        ''' <param name="i"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetOrdinalOfPrimaryEntry(i As UShort) As Integer

        '******* return the noPrimaryKeys
        ''' <summary>
        ''' the number of fields in the primary key
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function NoPrimaryEntries() As Integer

        ''' <summary>
        ''' the number of fields
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property NoEntries() As Integer
        ''' <summary>
        ''' gets an Index by name
        ''' </summary>
        ''' <param name="indexname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetIndex(indexname As String) As ArrayList
        ''' <summary>
        ''' True if index exists
        ''' </summary>
        ''' <param name="indexname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function HasIndex(indexname As String) As Boolean


    End Interface

    ''' <summary>
    ''' describes a container schema for relational stores
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormRelationalSchema
        Inherits iormContainerSchema

        ''' <summary>
        ''' Assign a native DB parameters and return
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <param name="parametername"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function AssignNativeDBParameter(columnname As String, Optional parametername As String = Nothing) As System.Data.IDbDataParameter

    End Interface

    ''' <summary>
    ''' describes a connection
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormConnection

        ''' <summary>
        ''' returns the native database name (product)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property NativeDatabaseName As String
        ''' <summary>
        ''' returns the native database version
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property NativeDatabaseVersion As String


        '******** Connect : Connects to the Database and initialize Environment
        ''' <summary>
        ''' connects to the native database
        ''' </summary>
        ''' <param name="FORCE"></param>
        ''' <param name="access"></param>
        ''' <param name="domainid"></param>
        ''' <param name="OTDBUsername"></param>
        ''' <param name="OTDBPassword"></param>
        ''' <param name="exclusive"></param>
        ''' <param name="notInitialize"></param>
        ''' <param name="doLogin"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Connect(Optional ByVal FORCE As Boolean = False, _
                        Optional ByVal access As otAccessRight = otAccessRight.[ReadOnly], _
                         Optional ByVal domainid As String = Nothing, _
                        Optional ByVal OTDBUsername As String = Nothing, _
                        Optional ByVal OTDBPassword As String = Nothing, _
                        Optional ByVal exclusive As Boolean = False, _
                        Optional ByVal notInitialize As Boolean = False, _
                        Optional ByVal doLogin As Boolean = True) As Boolean

        '**** ID of the Connection
        ReadOnly Property ID As String

        '**** useSeek Property
        ReadOnly Property Useseek As Boolean

        '*** ErrorLog
        ReadOnly Property [ErrorLog] As SessionMessageLog

        ''' <summary>
        ''' returns true if connected
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property IsConnected As Boolean

        ''' <summary>
        ''' returns true if connection is initialized
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property IsInitialized As Boolean

        ''' <summary>
        ''' gets or sets the Session of the Connection
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Session As Session

        ''' <summary>
        ''' Gets or sets the UI login.
        ''' </summary>
        ''' <value>The UI login.</value>
        Property UILogin As UI.CoreLoginForm

        ''' <summary>
        ''' Gets or sets the access.
        ''' </summary>
        ''' <value>The access.</value>
        Property Access As otAccessRight


        ''' <summary>
        ''' Gets or sets the dbpassword.
        ''' </summary>
        ''' <value>The dbpassword.</value>
        Property Dbpassword As String

        ''' <summary>
        ''' Gets or sets the dbuser.
        ''' </summary>
        ''' <value>The dbuser.</value>
        Property Dbuser As String

        ''' <summary>
        ''' Gets or sets the name of the database or file.
        ''' </summary>
        ''' <value>The name.</value>
        Property DBName As String

        ''' <summary>
        ''' Gets or sets the path.
        ''' </summary>
        ''' <value>The path.</value>
        Property PathOrAddress As String

        ''' <summary>
        ''' Gets or sets the connectionstring.
        ''' </summary>
        ''' <value>The connectionstring.</value>
        Property Connectionstring As String

        ''' <summary>
        ''' Gets or sets the databasetype.
        ''' </summary>
        ''' <value>OnTrackDatabaseServer</value>
        'Property Databasetype As otDBServerType

        ''' <summary>
        ''' Gets or sets the DatabaseEnvirorment.
        ''' </summary>
        ''' <value>iOTDBDatabaseEnvirorment</value>
        Property DatabaseDriver As iormRelationalDatabaseDriver
        ''' <summary>
        ''' Gets the NativeConnection.
        ''' </summary>
        ''' <value>Object</value>

        ReadOnly Property NativeConnection As Object
        ''' <summary>
        ''' disconnect from the database
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Disconnect() As Boolean

        ''' <summary>
        ''' set a connection configuration parameter
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function SetConnectionConfigParameters() As Boolean

        ''' <summary>
        ''' vallidate the access request in the optional domain and the optional object name
        ''' </summary>
        ''' <param name="accessRequest"></param>
        ''' <param name="domainid"></param>
        ''' <param name="Objectnames"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ValidateAccessRequest(accessRequest As otAccessRight, _
                                       Optional domainid As String = Nothing, _
                                        Optional ByRef [Objectnames] As List(Of String) = Nothing) As Boolean

        ''' <summary>
        ''' verify the access request of the optional given username / password with the database. 
        ''' Optional use a login window use a optional message text
        ''' </summary>
        ''' <param name="accessRequest"></param>
        ''' <param name="username"></param>
        ''' <param name="password"></param>
        ''' <param name="domainid"></param>
        ''' <param name="Objectnames"></param>
        ''' <param name="useLoginWindow"></param>
        ''' <param name="messagetext"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function VerifyUserAccess(accessRequest As otAccessRight, _
        Optional ByRef username As String = Nothing, _
        Optional ByRef password As String = Nothing, _
        Optional ByRef domainid As String = Nothing, _
        Optional ByRef [Objectnames] As List(Of String) = Nothing, _
        Optional useLoginWindow As Boolean = True, Optional messagetext As String = Nothing) As Boolean

        '*** Events
        Event OnConnection As EventHandler(Of ormConnectionEventArgs)
        Event OnDisconnection As EventHandler(Of ormConnectionEventArgs)

    End Interface


End Namespace