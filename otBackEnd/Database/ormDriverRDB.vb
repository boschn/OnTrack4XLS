
REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** CORE RELATIONAL DRIVER Classes for On Track Database Backend Library
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2013-08-08
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************''' <summary>

Imports System.Collections
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Data
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Diagnostics.Debug
Imports System.Attribute
Imports System.IO
Imports System.Text.RegularExpressions

Imports OnTrack.UI
Imports System.Reflection
Imports OnTrack.Commons
Imports OnTrack.Core
Imports OnTrack.rulez.eXPressionTree
Imports OnTrack.rulez

Namespace OnTrack.Database
    ''' <summary>
    ''' Attribute Class for marking an constant field member in a class as Table name such as
    ''' <otSchemaTable(Version:=1)>Const constTableName = "tblName"
    ''' Version will be saved into clsOTDBDEfSchemaTable
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Field, AllowMultiple:=False, Inherited:=True)> _
    Public Class ormTableAttribute
        Inherits ormContainerAttribute
        Implements iormContainerAttribute


        '** dynamic
        'Private _columns As New Dictionary(Of String, ormColumnAttribute)


        ''' <summary>
        '''  construcotr
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
        End Sub
        ''' <summary>
        ''' Gets or sets the container type
        ''' </summary>
        ''' <value>The is active.</value>
        Public Overrides Property Containertype As otContainerType Implements iormContainerAttribute.ContainerType
            Get
                Return otContainerType.Table
            End Get
            Set(value As otContainerType)
                Throw New NotSupportedException(" do not set a container type here")
            End Set
        End Property
        '''' <summary>
        '''' Add a member - which is a columnattribute
        '''' </summary>
        '''' <param name="member"></param>
        '''' <remarks></remarks>
        '''' <returns></returns>
        'Public Overloads Function AddEntry(member As iormContainerEntryAttribute) As Boolean Implements iormContainerAttribute.AddEntry
        '    Return AddColumn(member)
        'End Function
        '''' <summary>
        '''' Add an entry by TabeColumn
        '''' </summary>
        '''' <param name="entry"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Function AddColumn(entry As ormContainerEntryAttribute) As Boolean
        '    If _columns.ContainsKey(entry.ContainerEntryName.ToUpper) Then
        '        _columns.Remove(entry.ContainerEntryName.ToUpper)
        '    End If
        '    _columns.Add(key:=entry.ContainerEntryName.ToUpper, value:=entry)
        '    If entry.HasValuePrimaryKeyOrdinal Then
        '        If _primaryEntries.ContainsKey(entry.PrimaryEntryOrdinal) Then _primaryEntries.Remove(entry.PrimaryEntryOrdinal)
        '        _primaryEntries.Add(key:=entry.PrimaryEntryOrdinal, value:=entry.ContainerEntryName)
        '    End If
        '    Return True
        'End Function
        '''' <summary>
        '''' Add an entry by TabeColumn
        '''' </summary>
        '''' <param name="entry"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Function UpdateColumn(entry As ormContainerEntryAttribute) As Boolean
        '    If _columns.ContainsKey(entry.ContainerEntryName.ToUpper) Then
        '        _columns.Remove(entry.ContainerEntryName.ToUpper)
        '    End If
        '    _columns.Add(key:=entry.ContainerEntryName.ToUpper, value:=entry)
        '    If entry.HasValuePrimaryKeyOrdinal Then
        '        If _primaryEntries.ContainsKey(entry.PrimaryEntryOrdinal) Then _primaryEntries.Remove(entry.PrimaryEntryOrdinal)
        '        _primaryEntries.Add(key:=entry.PrimaryEntryOrdinal, value:=entry.ContainerEntryName)
        '    Else
        '        If _primaryEntries.Values.Contains(entry.ContainerEntryName) Then
        '            _primaryEntries.Remove(_primaryEntries.First(Function(x) x.Key = entry.ContainerEntryName).Key)
        '        End If
        '    End If
        '    Return True
        'End Function
        '''' <summary>
        '''' Add an entry by iormContainerMember
        '''' </summary>
        '''' <param name="member"></param>
        '''' <remarks></remarks>
        '''' <returns></returns>
        'Public Overloads Function UpdateEntry(member As iormContainerEntryAttribute) As Boolean Implements iormContainerAttribute.UpdateEntry
        '    Return UpdateColumn(member)
        'End Function
        '''' <summary>
        '''' returns an entry by member name or nothing
        '''' </summary>
        '''' <param name="membername"></param>
        '''' <param name="onlyenabled"></param>
        '''' <remarks></remarks>
        '''' <returns></returns>
        'Public Overloads Function GetEntry(membername As String, Optional onlyenabled As Boolean = True) As iormContainerEntryAttribute Implements iormContainerAttribute.GetEntry
        '    Return GetColumn(membername, onlyenabled)
        'End Function


        '''' <summary>
        '''' returns an entry by columnname or nothing
        '''' </summary>
        '''' <param name="columnname"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Function GetColumn(columnname As String, Optional onlyenabled As Boolean = True) As ormContainerEntryAttribute
        '    If _columns.ContainsKey(columnname.ToUpper) Then
        '        Dim anAttribute As ormContainerEntryAttribute = _columns.Item(key:=columnname.ToUpper)
        '        If onlyenabled AndAlso Not anAttribute.Enabled Then Return Nothing
        '        Return anAttribute
        '    Else
        '        Return Nothing
        '    End If
        'End Function
        '''' <summary>
        '''' returns an entry by columnname or nothing
        '''' </summary>
        '''' <param name="columnname"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Overloads Function HasColumn(columnname As String, Optional onlyenabled As Boolean = Nothing) As Boolean Implements iormContainerAttribute.HasEntry
        '    Dim result As Boolean = _columns.ContainsKey(columnname.ToUpper)
        '    If onlyenabled AndAlso result Then
        '        result = _columns.Item(columnname.ToUpper).Enabled
        '    End If
        '    Return result
        'End Function
        '''' <summary>
        '''' remove an entry by columnname 
        '''' </summary>
        '''' <param name="columnname"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Overloads Function RemoveColumn(columnname As String) As Boolean Implements iormContainerAttribute.RemoveEntry
        '    If _columns.ContainsKey(columnname.ToUpper) Then
        '        _columns.Remove(columnname.ToUpper)
        '        If _primaryEntries.Values.Contains(columnname) Then
        '            _primaryEntries.Remove(_primaryEntries.First(Function(x) x.Key = columnname).Key)
        '        End If
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function
        '''' <summary>
        '''' returns a List of all Entries
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks></remarks>
        '''' <value></value>
        'Public Overloads ReadOnly Property MemberAttributes() As IEnumerable(Of iormContainerEntryAttribute) Implements iormContainerAttribute.EntryAttributes
        '    Get
        '        Return ColumnAttributes
        '    End Get
        'End Property
        '''' <summary>
        '''' returns a List of all Entries
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public ReadOnly Property ColumnAttributes As IEnumerable(Of ormContainerEntryAttribute)
        '    Get
        '        Return _columns.Values.Where(Function(x) x.Enabled = True).ToList
        '    End Get
        'End Property


        '''' <summary>
        '''' sets or returns the Names of the PrimaryKey Columns
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Overloads Property PrimaryKeyColumnNames As String() Implements iormContainerAttribute.PrimaryEntryNames
        '    Get
        '        Return _primaryEntries.Values.ToArray
        '    End Get
        '    Set(value As String())
        '        _primaryEntries.Clear()

        '        For i = value.GetLowerBound(0) To value.GetUpperBound(0)
        '            _primaryEntries.Add(key:=i, value:=value(i))
        '        Next

        '    End Set
        'End Property
        '''' <summary>
        '''' returns a List of all Entries
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Overloads ReadOnly Property ColumnNames As IEnumerable(Of String) Implements iormContainerAttribute.EntryNames
        '    Get
        '        Return _columns.Values.Where(Function(x) x.Enabled = True).SelectMany(Function(x) x.ContainerEntryName).ToList
        '    End Get
        'End Property

        ''' <summary>
        ''' Gets or sets the name of the table.
        ''' </summary>
        ''' <value>The name of the table.</value>
        Public Overloads Property TableID() As String Implements iormContainerAttribute.ContainerID
            Get
                Return _ContainerID
            End Get
            Set(value As String)
                _ContainerID = value.ToUpper
            End Set
        End Property
        Public ReadOnly Property HasValueTableID As Boolean Implements iormContainerAttribute.HasValueContainerID
            Get
                Return Not String.IsNullOrWhiteSpace(_ContainerID)
            End Get
        End Property

    End Class

    ''' <summary>
    ''' abstract ORM Driver class for Relational Database Drivers
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class ormRDBDriver
        Implements iormRelationalDatabaseDriver

        Protected _ID As String
        Protected _TableDirectory As New Dictionary(Of String, iormRelationalTableStore)    'Table Directory of TableStored
        Protected _ViewDirectory As New Dictionary(Of String, iormRelationalTableStore)    'view Directory of TableStore
        Protected _TableSchemaDirectory As New Dictionary(Of String, iormContainerSchema)    'Table Directory of container schema
        Protected _ViewSchemaDirectory As New Dictionary(Of String, iormContainerSchema)    'view Directory of container schema
        Protected _setupid As String

        Protected WithEvents _primaryConnection As iormConnection ' primary connection
        Protected WithEvents _session As Session
        Protected _CommandStore As New Dictionary(Of String, iormSqlCommand) ' store of the SqlCommands to handle
        Protected _SelectCommandStore As New Dictionary(Of String, ormSqlSelectCommand) ' store of the SqlSelectCommands to handle
        Protected _lockObject As New Object 'Lock object instead of me

        ''' <summary>
        ''' Const
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstRulePrefix As String = "SELRULE_"
        Public Const ConstDBParameterTableName As String = "_DBPARAMETERS"
        'Public Const ConstOLDParameterTableName As String = "TBLDBPARAMETERS" 'Legacy Parameter Table w/o Application ID
        '** Field names of parameter table
        Public Const ConstFNSetupID = "SETUP"
        Public Const ConstFNID = "ID"
        Public Const ConstFNValue = "VALUE"
        Public Const ConstFNChangedOn = "CHANGEDON"
        Public Const constFNDescription = "DESCRIPTION"
        '* the events
        Public Event RequestBootstrapInstall(sender As Object, e As SessionBootstrapEventArgs) Implements iormOnTrackDriver.RequestBootstrapInstall
#Region "Properties"
        ''' <summary>
        ''' gets or sets the container version for an container id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ContainerVersion(id As String) As Long? Implements iormDatabaseDriver.ContainerVersion
            Get
                Dim aVersion As Object = GetDBParameter(ConstPNBSchemaVersion_ContainerHeader & id, silent:=True)
                If aVersion Is Nothing OrElse Not IsNumeric(aVersion) Then Return Nothing
                Return CULng(aVersion)
            End Get
            Set(value As Long?)
                SetDBParameter(ConstPNBSchemaVersion_ContainerHeader & id, value:=value.ToString, silent:=True)
            End Set
        End Property

        ''' <summary>
        ''' return true if driver is supporting a relational database
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsRelationalDriver As Boolean Implements iormDatabaseDriver.IsRelationalDriver
            Get
                Return True
            End Get
        End Property

        ''' <summary>
        ''' return true if driver is supporting  hosting an OnTrack database
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsOnTrackDriver As Boolean Implements iormDatabaseDriver.IsOnTrackDriver
            Get
                Dim aDriverAttribute = ot.ObjectClassRepository.GetDBDriverAttributes.Where(Function(x) x.Name.ToUpper = Me.Name.ToUpper).FirstOrDefault
                If aDriverAttribute Is Nothing Then Return False
                Return aDriverAttribute.IsOnTrackDriver
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the session.
        ''' </summary>
        ''' <value>The session.</value>
        Public Property Session As Session Implements iormDatabaseDriver.Session
            Get
                Return Me._session
            End Get
            Set(value As Session)
                _session = value
                _primaryConnection.Session = value
            End Set
        End Property

        ''' <summary>
        ''' Returns the Parameter Tablename
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride ReadOnly Property DBParameterTablename As String Implements iormRelationalDatabaseDriver.DBParameterContainerName

        '' <summary>
        ''' Gets the static name of the driver .
        ''' </summary>
        ''' <value>The type.</value>
        Public MustOverride ReadOnly Property Name() As String Implements iormRelationalDatabaseDriver.Name

        '' <summary>
        ''' Gets the native database name 
        ''' </summary>
        ''' <value>The type.</value>
        Public MustOverride ReadOnly Property NativeDatabaseName As String Implements iormRelationalDatabaseDriver.NativeDatabaseName

        '' <summary>
        ''' Gets the native database version 
        ''' </summary>
        ''' <value>The type.</value>
        Public MustOverride ReadOnly Property NativeDatabaseVersion As String Implements iormRelationalDatabaseDriver.NativeDatabaseVersion

        ''' <summary>
        ''' Gets or sets the ID of the driver instance.
        ''' </summary>
        ''' <value>The ID.</value>
        Public Overridable Property ID() As String Implements iormRelationalDatabaseDriver.ID
            Set(value As String)
                _ID = value
            End Set
            Get
                Return _ID
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the table schema directory.
        ''' </summary>
        ''' <value>The table schema directory.</value>
        Public Property TableSchemaDirectory() As Dictionary(Of String, iormContainerSchema)
            Get
                Return Me._TableSchemaDirectory
            End Get
            Set(value As Dictionary(Of String, iormContainerSchema))
                Me._TableSchemaDirectory = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the table directory.
        ''' </summary>
        ''' <value>The table directory.</value>
        Public Property TableDirectory() As Dictionary(Of String, iormRelationalTableStore)
            Get
                Return Me._TableDirectory
            End Get
            Set(value As Dictionary(Of String, iormRelationalTableStore))
                Me._TableDirectory = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the connection.
        ''' </summary>
        ''' <value>The connection.</value>
        Public Overridable ReadOnly Property CurrentConnection() As iormConnection Implements iormRelationalDatabaseDriver.CurrentConnection
            Get
                Return _primaryConnection
            End Get

        End Property
#End Region
        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()

        End Sub
        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="session"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal id As String, ByRef session As Session)
            _ID = id
            _session = session

        End Sub

        ''' <summary>
        ''' checks if SqlCommand is in Store of the driver
        ''' </summary>
        ''' <param name="id">id of the command</param>
        ''' <remarks></remarks>
        ''' <returns>True if successful</returns>
        Public Function HasSqlCommand(id As String) As Boolean Implements iormRelationalDatabaseDriver.HasSqlCommand
            Return _CommandStore.ContainsKey(key:=id)
        End Function

        ''' <summary>
        ''' Store the Command by its ID - replace if existing
        ''' </summary>
        ''' <param name="sqlCommand">a iOTDBSqlCommand</param>
        ''' <remarks></remarks>
        ''' <returns>true if successful</returns>
        Public Function StoreSqlCommand(ByRef sqlCommand As iormSqlCommand) As Boolean Implements iormRelationalDatabaseDriver.StoreSqlCommand
            If _CommandStore.ContainsKey(key:=sqlCommand.ID) Then
                _CommandStore.Remove(key:=sqlCommand.ID)
            End If
            _CommandStore.Add(key:=sqlCommand.ID, value:=sqlCommand)
            Return True
        End Function

        ''' <summary>
        ''' Retrieve the Command from Store
        ''' </summary>
        ''' <param name="id">id of the command</param>
        ''' <remarks></remarks>
        ''' <returns>a iOTDBSqlCommand</returns>
        Public Function RetrieveSqlCommand(id As String) As iormSqlCommand Implements iormRelationalDatabaseDriver.RetrieveSqlCommand
            If _CommandStore.ContainsKey(key:=id) Then
                Return _CommandStore.Item(key:=id)
            End If

            Return Nothing
        End Function
        ''' <summary>
        ''' Creates a Command and store it or gets the current Command
        ''' </summary>
        ''' <param name="id">id of the command</param>
        ''' <returns>a iOTDBSqlCommand</returns>
        ''' <remarks></remarks>
        Public Overridable Function CreateSqlCommand(id As String) As iormSqlCommand Implements iormRelationalDatabaseDriver.CreateSqlCommand
            '* get the ID

            If Me.HasSqlCommand(id) Then
                Return Me.RetrieveSqlCommand(id)
            Else
                Dim aSqlCommand As iormSqlCommand = New ormSqlCommand(id)
                Me.StoreSqlCommand(aSqlCommand)
                Return aSqlCommand
            End If
        End Function
        ''' <summary>
        ''' Creates a Command and store it or gets the current Command
        ''' </summary>
        ''' <param name="id">id of the command</param>
        ''' <returns>a iOTDBSqlCommand</returns>
        ''' <remarks></remarks>
        Public Overridable Function CreateSqlSelectCommand(id As String) As iormSqlCommand Implements iormRelationalDatabaseDriver.CreateSqlSelectCommand
            '* get the ID

            If Me.HasSqlCommand(id) Then
                Return Me.RetrieveSqlCommand(id)
            Else
                Dim aSqlCommand As iormSqlCommand = New ormSqlSelectCommand(id)
                Me.StoreSqlCommand(aSqlCommand)
                Return aSqlCommand
            End If
        End Function
        ''' <summary>
        ''' Register a connection at the Driver to be used
        ''' </summary>
        ''' <param name="connection">a iOTDBConnection</param>
        ''' <returns>true if successful</returns>
        ''' <remarks></remarks>
        Protected Overridable Function RegisterConnection(ByRef connection As iormConnection) As Boolean Implements iormDatabaseDriver.RegisterConnection
            If _primaryConnection Is Nothing Then
                _primaryConnection = connection
                Return True
            Else
                Return False
            End If
        End Function
        ''' <summary>
        ''' Handles the onDisconnect Event of the Driver
        ''' </summary>
        ''' <returns>True if successfull</returns>
        ''' <remarks></remarks>
        Public Function OnDisconnect() As Boolean Handles _primaryConnection.OnDisconnection
            _TableDirectory.Clear()
            _TableSchemaDirectory.Clear()
            Return True
        End Function

        ''' <summary>
        ''' installs the ONTrack Database Schema
        ''' </summary>
        ''' <param name="askBefore"></param>
        ''' <param name="modules"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function InstallOnTrackDatabase(askBefore As Boolean, modules As String()) As Boolean Implements iormOnTrackDriver.InstallOnTrackDatabase

        ''' <summary>
        ''' returns true if an OnTrack Admin User is available in the database
        ''' </summary>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function HasAdminUserValidation(Optional ByRef nativeConnection As Object = Nothing) As Boolean Implements iormOnTrackDriver.HasAdminUserValidation

        ''' <summary>
        ''' Gets or creates the foreign key for a columndefinition
        ''' </summary>
        ''' <param name="nativeTable">The native table.</param>
        ''' <param name="columndefinition">The columndefinition.</param>
        ''' <param name="createOrAlter">The create or alter.</param>
        ''' <param name="connection">The connection.</param>
        ''' <returns></returns>
        Public MustOverride Function GetForeignKeys(nativeTable As Object, foreignkeydefinition As ormForeignKeyDefinition, Optional createOrAlter As Boolean = False, Optional ByRef connection As iormConnection = Nothing) As IEnumerable(Of Object) Implements iormDatabaseDriver.GetForeignKeys

        ''' <summary>
        ''' Creates the global domain.
        ''' </summary>
        ''' <param name="nativeConnection">The native connection.</param>
        ''' <returns></returns>
        Public MustOverride Function CreateGlobalDomain(Optional ByRef nativeConnection As Object = Nothing) As Boolean Implements iormOnTrackDriver.CreateGlobalDomain



        ''' <summary>
        ''' verifyOnTrack
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function VerifyOnTrackDatabase(Optional modules As String() = Nothing, Optional install As Boolean = False, Optional verifySchema As Boolean = False) As Boolean Implements iormOnTrackDriver.VerifyOnTrackDatabase


        ''' <summary>
        ''' create an assigned Native DBParameter to provided name and type
        ''' </summary>
        ''' <param name="parametername"></param>
        ''' <param name="datatype"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function AssignNativeDBParameter(parametername As String, datatype As otDataType, _
                                                              Optional maxsize As Long = 0, _
                                                             Optional value As Object = Nothing) As System.Data.IDbDataParameter Implements iormRelationalDatabaseDriver.AssignNativeDBParameter

        ''' <summary>
        ''' returns the target type for a OTDB FieldType - MAPPING
        ''' </summary>
        ''' <param name="type"></param>
        ''' <remarks></remarks>
        ''' <returns></returns>
        Public MustOverride Function GetTargetTypeFor(type As otDataType) As Long Implements iormDatabaseDriver.GetTargetTypeFor
        '
        ''' <summary>
        '''  converts value to targetType of the native DB Driver
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="targetType"></param>
        ''' <param name="maxsize"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <param name="fieldname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function Convert2DBData(ByVal invalue As Object, ByRef outvalue As Object, _
                                                    targetType As Long, _
                                                    Optional ByVal maxsize As Long = 0, _
                                                   Optional ByRef abostrophNecessary As Boolean = False, _
                                                   Optional ByVal fieldname As String = Nothing, _
                                                   Optional isnullable As Boolean = False,
                                                    Optional defaultvalue As Object = Nothing) As Boolean Implements iormDatabaseDriver.Convert2DBData

        ''' <summary>
        ''' Runs the SQL select command.
        ''' </summary>
        ''' <param name="sqlcommand">The sqlcommand.</param>
        ''' <param name="parametervalues">The parametervalues.</param>
        ''' <param name="nativeConnection">The native connection.</param>
        ''' <returns></returns>
        Public MustOverride Function RunSqlCommand(ByRef sqlcommand As ormSqlCommand, _
                                                   Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing, _
                                                   Optional nativeConnection As Object = Nothing) As Boolean Implements iormRelationalDatabaseDriver.RunSqlCommand


        ''' <summary>
        ''' Convert2s the object data.
        ''' </summary>
        ''' <param name="invalue">The invalue.</param>
        ''' <param name="outvalue">The outvalue.</param>
        ''' <param name="sourceType">Type of the source.</param>
        ''' <param name="isnullable">The isnullable.</param>
        ''' <param name="defaultvalue">The defaultvalue.</param>
        ''' <param name="abostrophNecessary">The abostroph necessary.</param>
        ''' <returns></returns>
        Public MustOverride Function Convert2ObjectData(invalue As Object, _
                                                        ByRef outvalue As Object, _
                                                        sourceType As Long, _
                                                        Optional isnullable As Boolean? = Nothing, _
                                                        Optional defaultvalue As Object = Nothing, _
                                                        Optional ByRef abostrophNecessary As Boolean = False) As Boolean Implements iormDatabaseDriver.Convert2ObjectData


        ''' <summary>
        ''' returns True if data store has the table
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function HasTable(tableid As String, _
                                              Optional ByRef connection As iormConnection = Nothing, _
                                              Optional nativeConnection As Object = Nothing) As Boolean Implements iormRelationalDatabaseDriver.HasTable, iormDatabaseDriver.HasContainerID

        ''' <summary>
        ''' returns True if data store has the table by definition
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function VerifyTableSchema(tabledefinition As ormContainerDefinition, Optional ByRef connection As iormConnection = Nothing, Optional nativeConnection As Object = Nothing) As Boolean Implements iormRelationalDatabaseDriver.VerifyTableSchema

        ''' <summary>
        ''' returns True if data store has the table attribute
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function VerifyTableSchema(tableattribute As ormTableAttribute, Optional ByRef connection As iormConnection = Nothing, Optional nativeConnection As Object = Nothing) As Boolean Implements iormRelationalDatabaseDriver.VerifyContainerSchema

        ''' <summary>
        ''' returns True if data store has the table attribute
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function VerifyContainerSchema(containerdefinition As iormContainerDefinition, Optional ByRef connection As iormConnection = Nothing, Optional nativeConnection As Object = Nothing) As Boolean Implements iormDatabaseDriver.VerifyContainerSchema


        ''' <summary>
        ''' Gets, creates or alters the table.
        ''' </summary>
        ''' <param name="tableid">The ot tableid.</param>
        ''' <param name="createOrAlter">The create on missing.</param>
        ''' <param name="addToSchemaDir">The add to schema dir.</param>
        ''' <param name="connection">The native connection.</param>
        ''' <returns></returns>
        Public MustOverride Function GetTable(tableid As String, _
                        Optional createOrAlter As Boolean = False, _
                        Optional ByRef connection As iormConnection = Nothing, _
                         Optional ByRef nativeTableObject As Object = Nothing) As Object Implements iormRelationalDatabaseDriver.GetTable, iormDatabaseDriver.GetContainerObject

        ''' <summary>
        ''' drops the container version from the database
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DropContainerVersion(id As String) As Boolean Implements iormDatabaseDriver.DropContainerVersion
            ' delete current version also in the DB paramter Table
            DeleteDBParameter(parametername:=ConstPNBSchemaVersion_ContainerHeader & id.ToUpper, silent:=True)
            Return True
        End Function
        ''' <summary>
        ''' drops a table in the database by id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="connection"></param>
        ''' <remarks></remarks>
        ''' <returns></returns>
        Public Overridable Function DropTable(id As String, _
                                               Optional ByRef connection As iormConnection = Nothing) As Boolean Implements iormDatabaseDriver.DropContainerObject, iormRelationalDatabaseDriver.DropTable

            ' delete current version also in the DB paramter Table
            Return DropContainerVersion(id)
        End Function

        ''' <summary>
        ''' returns true if the datastore has the view by viewname
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="connection"></param>
        ''' <param name="nativeConnection"></param>
        ''' <remarks></remarks>
        ''' <returns></returns>
        Public MustOverride Function HasView(viewid As String, Optional ByRef connection As iormConnection = Nothing, Optional nativeConnection As Object = Nothing) As Boolean Implements iormRelationalDatabaseDriver.HasView


        ''' <summary>
        ''' returns or creates a View in the data store
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="sqlselect"></param>
        ''' <param name="createOrAlter"></param>
        ''' <param name="connection"></param>
        ''' <remarks></remarks>
        ''' <returns></returns>
        Public MustOverride Function GetView(viewid As String, Optional sqlselect As String = Nothing, Optional createOrAlter As Boolean = False, Optional ByRef connection As iormConnection = Nothing) As Object Implements iormRelationalDatabaseDriver.GetView


        ''' <summary>
        ''' drops a view by id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="connection"></param>
        ''' <remarks></remarks>
        ''' <returns></returns>
        Public MustOverride Function DropView(id As String, Optional ByRef connection As iormConnection = Nothing) As Boolean Implements iormRelationalDatabaseDriver.DropView


        ''' <summary>
        ''' Gets the index.
        ''' </summary>
        ''' <param name="nativeTABLE">The native TABLE.</param>
        ''' <param name="indexname">The indexname.</param>
        ''' <param name="ColumnNames">The column names.</param>
        ''' <param name="PrimaryKey">The primary key.</param>
        ''' <param name="forceCreation">The force creation.</param>
        ''' <param name="createOrAlter">The create on missing.</param>
        ''' <param name="addToSchemaDir">The add to schema dir.</param>
        ''' <returns></returns>

        Public MustOverride Function GetIndex(ByRef nativeTable As Object, ByRef indexdefinition As ormIndexDefinition, _
                                               Optional ByVal forceCreation As Boolean = False, _
                                               Optional ByVal createOrAlter As Boolean = False, _
                                               Optional ByRef connection As iormConnection = Nothing) As Object Implements iormRelationalDatabaseDriver.GetIndex

        ''' <summary>
        ''' returns True if the column exists in the table 
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="columnname"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function HasColumn(tablename As String, _
                                               columnname As String, _
                                               Optional ByRef connection As iormConnection = Nothing) As Boolean Implements iormRelationalDatabaseDriver.HasColumn, iormDatabaseDriver.HasContainerEntryID

        ''' <summary>
        ''' returns True if the column exists in the table 
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="columnname"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function VerifyColumnSchema(attribute As iormContainerEntryDefinition, _
                                                        Optional ByRef connection As iormConnection = Nothing, _
                                                        Optional silent As Boolean = False) As Boolean Implements iormRelationalDatabaseDriver.VerifyColumnSchema, iormDatabaseDriver.VerifyContainerEntrySchema

        ''' <summary>
        ''' Gets the column.
        ''' </summary>
        ''' <param name="nativeTABLE">The native TABLE.</param>
        ''' <param name="aDBDesc">A DB desc.</param>
        ''' <param name="createOrAlter">The create on missing.</param>
        ''' <param name="addToSchemaDir">The add to schema dir.</param>
        ''' <returns></returns>
        Public MustOverride Function GetColumn(nativeTable As Object, _
                                               columndefinition As iormContainerEntryDefinition, _
                                               Optional createOrAlter As Boolean = False, _
                                               Optional ByRef connection As iormConnection = Nothing) As Object Implements iormRelationalDatabaseDriver.GetColumn, iormDatabaseDriver.GetContainerEntryObject


        ''' <summary>
        ''' Create the User Definition Table
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function CreateDBUserDefTable(Optional ByRef nativeConnection As Object = Nothing) As Boolean Implements iormOnTrackDriver.CreateDBUserDefTable

        ''' <summary>
        ''' create the DB Parameter Table
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function CreateDBParameterTable(Optional ByRef nativeConnection As Object = Nothing) As Boolean Implements iormOnTrackDriver.CreateDBParameterContainer


        ''' <summary>
        ''' drops the DB parameter table - given with setup then just the setup related entries
        ''' if then there is no setup related entries at all -> drop the full table
        ''' </summary>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function DropDBParameterTable(Optional setup As String = Nothing, _
                                                          Optional ByRef nativeConnection As Object = Nothing) As Boolean Implements iormOnTrackDriver.DropDBParameterContainer

        ''' <summary>
        ''' deletes a DB Parameter
        ''' </summary>
        ''' <param name="parametername"></param>
        ''' <param name="nativeConnection"></param>
        ''' <param name="silent"></param>
        ''' <param name="setupID"></param>
        ''' <remarks></remarks>
        ''' <returns></returns>
        Public MustOverride Function DeleteDBParameter(parametername As String, _
                                                       Optional ByRef nativeConnection As Object = Nothing, _
                                                       Optional silent As Boolean = False, _
                                                       Optional setupID As String = Nothing) As Boolean Implements iormOnTrackDriver.DeleteDBParameter


        ''' <summary>
        ''' Sets the DB parameter.
        ''' </summary>
        ''' <param name="Parametername">The parametername.</param>
        ''' <param name="Value">The value.</param>
        ''' <param name="connection">The native connection.</param>
        ''' <param name="UpdateOnly">The update only.</param>
        ''' <param name="silent">The silent.</param>
        ''' <returns></returns>
        Public MustOverride Function SetDBParameter(parametername As String, _
                                                    value As Object, _
                                                    Optional ByRef nativeConnection As Object = Nothing, _
                                                    Optional updateOnly As Boolean = False, _
                                                    Optional silent As Boolean = False, _
                                                    Optional setupID As String = Nothing, _
                                                    Optional description As String = Nothing) As Boolean Implements iormOnTrackDriver.SetDBParameter

        ''' <summary>
        ''' Gets the DB parameter.
        ''' </summary>
        ''' <param name="PARAMETERNAME">The PARAMETERNAME.</param>
        ''' <param name="connection">The native connection.</param>
        ''' <param name="silent">The silent.</param>
        ''' <returns></returns>
        Public MustOverride Function GetDBParameter(parametername As String, _
                                                    Optional ByRef nativeConnection As Object = Nothing, _
                                                    Optional silent As Boolean = False, _
                                                    Optional setupID As String = Nothing) As Object Implements iormOnTrackDriver.GetDBParameter



        ''' <summary>
        ''' validates the User, Passoword, Access Right in the Domain
        ''' </summary>
        ''' <param name="username"></param>
        ''' <param name="password"></param>
        ''' <param name="accessright"></param>
        ''' <param name="domainID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ValidateUser(ByVal username As String, _
                                     ByVal password As String, _
                                     ByVal accessRequest As otAccessRight, _
                                     Optional domainid As String = Nothing) As Boolean Implements iormOnTrackDriver.ValidateUser

            Dim aValidation As UserValidation
            aValidation.ValidEntry = False
            aValidation = GetUserValidation(username:=username)

            If Not aValidation.ValidEntry Then
                Return False
            Else
                ''' if validation has nothing then continiue with any password
                ''' fail if provided password is nothing
                If aValidation.Password IsNot Nothing AndAlso (password Is Nothing OrElse aValidation.Password <> password) Then
                    Return False
                End If

                '** check against the validation
                Dim aAccessProperty As AccessRightProperty

                If aValidation.ValidEntry Then
                    If aValidation.HasAlterSchemaRights Then
                        aAccessProperty = New AccessRightProperty(otAccessRight.AlterSchema)
                    ElseIf aValidation.HasUpdateRights Then
                        aAccessProperty = New AccessRightProperty(otAccessRight.ReadUpdateData)
                    ElseIf aValidation.HasReadRights Then
                        aAccessProperty = New AccessRightProperty(otAccessRight.ReadOnly)
                    Else
                        Return False 'return if no Right in the validation
                    End If
                End If
                ''' ToDo: forbidd access for domains
                ''' 
                ''' check if Rights are covered
                Return aAccessProperty.CoverRights(accessRequest)
            End If

        End Function
        ''' <summary>
        ''' Gets the ontrack user validation object.
        ''' </summary>
        ''' <param name="Username">The username.</param>
        ''' <param name="connection">The native connection.</param>
        ''' <returns></returns>
        Protected Friend MustOverride Function GetUserValidation(username As String, _
                                                                 Optional ByVal selectAnonymous As Boolean = False, _
                                                                 Optional ByRef nativeConnection As Object = Nothing) As UserValidation Implements iormOnTrackDriver.GetUserValidation

        ''' <summary>
        ''' create a tablestore 
        ''' </summary>
        ''' <param name="TableID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function CreateNativeTableStore(ByVal tableID As String, ByVal forceSchemaReload As Boolean) As iormRelationalTableStore
        ''' <summary>
        ''' create a tableschema
        ''' </summary>
        ''' <param name="TableID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function CreateNativeTableSchema(ByVal tableID As String) As iormContainerSchema

        ''' <summary>
        ''' create a native view reader 
        ''' </summary>
        ''' <param name="TableID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function CreateNativeViewReader(ByVal viewID As String, ByVal forceSchemaReload As Boolean) As iormRelationalTableStore
        ''' <summary>
        ''' create native view schema object
        ''' </summary>
        ''' <param name="TableID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function CreateNativeViewSchema(ByVal viewID As String) As iormContainerSchema

        ''' <summary>
        ''' persists the errorlog
        ''' </summary>
        ''' <param name="TableID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function PersistLog(ByRef log As SessionMessageLog) As Boolean Implements iormOnTrackDriver.PersistLog
        ''' <summary>
        ''' Gets the data store which is the tablestore
        ''' </summary>
        ''' <param name="tableID">The tablename.</param>
        ''' <param name="Force">The force.</param>
        ''' <returns></returns>
        Public Function RetrieveContainerStore(ByVal containerid As String, Optional ByVal force As Boolean = False) As iormContainerStore Implements iormDatabaseDriver.RetrieveContainerStore
            Dim aContainerstore As iormContainerStore
            ''' try to get it from the tables
            If _TableDirectory.ContainsKey(containerid.ToUpper) Then
                aContainerstore = Me.GetTableStore(containerid, force)
                If aContainerstore IsNot Nothing Then Return aContainerstore
            End If
            ''' try it from the view
            If _ViewDirectory.ContainsKey(containerid.ToUpper) Then
                aContainerstore = Me.GetViewReader(containerid, force)
                If aContainerstore IsNot Nothing Then Return aContainerstore
            End If

            ''' try to get it as a table first, then as a view
            ''' 
            aContainerstore = Me.GetTableStore(tableID:=containerid.ToUpper)
            If aContainerstore IsNot Nothing Then Return aContainerstore

            aContainerstore = Me.GetViewReader(containerid, force)
            Return aContainerstore
        End Function
        ''' <summary>
        ''' Gets the table store.
        ''' </summary>
        ''' <param name="tableID">The tablename.</param>
        ''' <param name="Force">The force.</param>
        ''' <returns></returns>
        Public Function GetTableStore(ByVal tableID As String, Optional ByVal force As Boolean = False) As iormRelationalTableStore Implements iormRelationalDatabaseDriver.GetTableStore
            'take existing or make new one
            If _TableDirectory.ContainsKey(tableID.ToUpper) And Not force Then
                Return _TableDirectory.Item(tableID.ToUpper)
            Else
                Dim aNewStore As iormRelationalTableStore

                ' reload the existing object on force
                If _TableDirectory.ContainsKey(tableID.ToUpper) Then
                    aNewStore = _TableDirectory.Item(tableID.ToUpper)
                    aNewStore.Refresh(force)
                    Return aNewStore
                End If
                ' assign the Table

                aNewStore = CreateNativeTableStore(tableID.ToUpper, forceSchemaReload:=force)
                If Not aNewStore Is Nothing Then
                    If Not _TableDirectory.ContainsKey(tableID.ToUpper) Then
                        _TableDirectory.Add(key:=tableID.ToUpper, value:=aNewStore)
                    End If
                End If
                ' return
                Return aNewStore

            End If

        End Function

        ''' <summary>
        ''' Gets the table schema.
        ''' </summary>
        ''' <param name="Tablename">The tablename.</param>
        ''' <param name="Force">The force.</param>
        ''' <returns></returns>
        Public Function GetTableSchema(ByVal tableID As String, Optional ByVal force As Boolean = False) As iormContainerSchema Implements iormDatabaseDriver.RetrieveContainerSchema, iormRelationalDatabaseDriver.RetrieveTableSchema

            'take existing or make new one
            If _TableSchemaDirectory.ContainsKey(tableID.ToUpper) And Not force Then
                Return _TableSchemaDirectory.Item(tableID.ToUpper)
            Else
                Dim aNewSchema As iormContainerSchema

                ' delete the existing object
                If _TableSchemaDirectory.ContainsKey(tableID.ToUpper) Then
                    aNewSchema = _TableSchemaDirectory.Item(tableID.ToUpper)
                    SyncLock aNewSchema
                        If force Or Not aNewSchema.IsInitialized Then aNewSchema.Refresh(force)
                    End SyncLock
                    Return aNewSchema
                End If
                ' assign the Table
                aNewSchema = CreateNativeTableSchema(tableID.ToUpper)

                If Not aNewSchema Is Nothing Then
                    SyncLock _lockObject
                        _TableSchemaDirectory.Add(key:=tableID.ToUpper, value:=aNewSchema)
                    End SyncLock

                    If Not aNewSchema.IsInitialized Then
                        SyncLock aNewSchema
                            aNewSchema.Refresh(reloadForce:=force)
                        End SyncLock
                    End If
                End If

                ' return
                Return aNewSchema
            End If

        End Function
        ''' <summary>
        ''' Gets the view reader
        ''' </summary>
        ''' <param name="tableID">The tablename.</param>
        ''' <param name="Force">The force.</param>
        ''' <returns></returns>
        Public Function GetViewReader(ByVal viewID As String, Optional ByVal force As Boolean = False) As iormRelationalTableStore Implements iormRelationalDatabaseDriver.GetViewReader
            'take existing or make new one
            If _ViewDirectory.ContainsKey(viewID.ToUpper) And Not force Then
                Return _ViewDirectory.Item(viewID.ToUpper)
            Else
                Dim aNewStore As iormRelationalTableStore

                ' reload the existing object on force
                If _ViewDirectory.ContainsKey(viewID.ToUpper) Then
                    aNewStore = _ViewDirectory.Item(viewID.ToUpper)
                    aNewStore.Refresh(force)
                    Return aNewStore
                End If
                ' assign the Table

                aNewStore = CreateNativeViewReader(viewID.ToUpper, forceSchemaReload:=force)
                If Not aNewStore Is Nothing Then
                    If Not _ViewDirectory.ContainsKey(viewID.ToUpper) Then
                        _ViewDirectory.Add(key:=viewID.ToUpper, value:=aNewStore)
                    End If
                End If
                ' return
                Return aNewStore

            End If

        End Function
        ''' <summary>
        ''' Gets the view schema
        ''' </summary>
        ''' <param name="Tablename">The tablename.</param>
        ''' <param name="Force">The force.</param>
        ''' <returns></returns>
        Public Function GetViewSchema(ByVal viewID As String, Optional ByVal force As Boolean = False) As iormContainerSchema Implements iormRelationalDatabaseDriver.GetViewSchema

            'take existing or make new one
            If _ViewSchemaDirectory.ContainsKey(viewID.ToUpper) And Not force Then
                Return _ViewSchemaDirectory.Item(viewID.ToUpper)
            Else
                Dim aNewSchema As iormContainerSchema

                ' delete the existing object
                If _ViewSchemaDirectory.ContainsKey(viewID.ToUpper) Then
                    aNewSchema = _ViewSchemaDirectory.Item(viewID.ToUpper)
                    SyncLock aNewSchema
                        If force Or Not aNewSchema.IsInitialized Then aNewSchema.Refresh(force)
                    End SyncLock
                    Return aNewSchema
                End If
                ' assign the Table
                aNewSchema = CreateNativeViewSchema(viewID.ToUpper)

                If Not aNewSchema Is Nothing Then
                    SyncLock _lockObject
                        _ViewSchemaDirectory.Add(key:=viewID.ToUpper, value:=aNewSchema)
                    End SyncLock

                    If Not aNewSchema.IsInitialized Then
                        SyncLock aNewSchema
                            aNewSchema.Refresh(reloadForce:=force)
                        End SyncLock
                    End If
                End If

                ' return
                Return aNewSchema
            End If

        End Function
        ''' <summary>
        ''' Runs the SQL Command
        ''' </summary>
        ''' <param name="sqlcmdstr"></param>
        ''' <param name="parameters"></param>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function RunSqlStatement(ByVal sqlcmdstr As String, Optional ByRef parameters As List(Of ormSqlCommandParameter) = Nothing, _
                                                  Optional silent As Boolean = True, Optional nativeConnection As Object = Nothing) As Boolean _
                                              Implements iormRelationalDatabaseDriver.RunSqlStatement


        ''' <summary>
        ''' Runs the SQL select command.
        ''' </summary>
        ''' <param name="sqlcommand">The sqlcommand.</param>
        ''' <param name="parameters">The parameters.</param>
        ''' <param name="connection">The native connection.</param>
        ''' <returns></returns>
        Public MustOverride Function RunSqlSelectCommand(ByRef sqlcommand As ormSqlSelectCommand, _
                                            Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing, _
                                            Optional nativeConnection As Object = Nothing) As List(Of ormRecord) _
                                        Implements iormRelationalDatabaseDriver.RunSqlSelectCommand

        Public MustOverride Function RunSqlSelectCommand(id As String, _
                                                     Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing, _
                                                     Optional nativeConnection As Object = Nothing) As List(Of ormRecord) _
                                                   Implements iormRelationalDatabaseDriver.RunSqlSelectCommand

        ''' <summary>
        ''' run a selection rule and return the result as ormRecords
        ''' </summary>
        ''' <param name="selectionrule"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function RunSelectionRule(rule As OnTrack.rulez.eXPressionTree.SelectionRule, context As OnTrack.rulez.Context) As IList(Of ormRecord) Implements iormRelationalDatabaseDriver.RetrieveBy
            ''' convert to sql select command
            If _SelectCommandStore.ContainsKey(ConstRulePrefix & rule.ID) Then
                Dim aSelectCommand = rule.ToSQLSelectCommand(id:=ConstRulePrefix & rule.ID)
                ''' get the parameters
                ''' 
                Dim parameters As New Dictionary(Of String, Object)
                For Each aParameter In rule.Parameters.Reverse
                    parameters.Add(key:=aParameter.ID, value:=context.Pop())
                Next
                ' run 
                Return aSelectCommand.RunSelect(parameters)
            Else
                CoreMessageHandler(message:="SQL select command for rule id '" & ConstRulePrefix & rule.ID & "' not found in drivers store", _
                                    messagetype:=otCoreMessageType.InternalError)

            End If
        End Function

        ''' <summary>
        ''' prepares a selection rule
        ''' </summary>
        ''' <param name="rule"></param>
        ''' <param name="resultCode"></param>
        ''' <remarks></remarks>
        ''' <returns></returns>
        Public Overridable Function PrepareSelection(rule As SelectionRule, ByRef resultCode As ICodeBit) As Boolean Implements iormDatabaseDriver.PrepareSelection

            ''' convert to sql select command
            If Not _SelectCommandStore.ContainsKey(ConstRulePrefix & rule.ID) Then
                Dim aSelectCommand = rule.ToSQLSelectCommand(id:=ConstRulePrefix & rule.ID)
                If aSelectCommand IsNot Nothing Then
                    _SelectCommandStore.Add(key:=ConstRulePrefix & rule.ID, value:=aSelectCommand)
                    Return True
                End If

            Else
                CoreMessageHandler(message:="SQL select command for rule id '" & ConstRulePrefix & rule.ID & "' already prepared in drivers store", _
                                    messagetype:=otCoreMessageType.InternalError)

            End If
            Return False
        End Function

        ''' <summary>
        ''' Create a Native IDBCommand (Sql Command)
        ''' </summary>
        ''' <param name="cmd"></param>
        ''' <param name="aNativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function CreateNativeDBCommand(cmd As String, aNativeConnection As System.Data.IDbConnection) As System.Data.IDbCommand Implements iormRelationalDatabaseDriver.CreateNativeDBCommand

        ''' <summary>
        ''' returns the native tablename in the native database
        ''' </summary>
        ''' <param name="tableid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GetNativeDBObjectName(tableid As String) As String Implements iormRelationalDatabaseDriver.GetNativeDBObjectName
            If String.IsNullOrWhiteSpace(Me.Session.CurrentSetupID) OrElse tableid = ConstDBParameterTableName Then
                ' create the native name as simple copy of the tableid
                Return tableid
            Else
                ' create the tablename out of the SetupID "_" tableid
                Return Me.Session.CurrentSetupID & "_" & tableid
            End If
        End Function

        ''' <summary>
        ''' returns the native tablename in the native database
        ''' </summary>
        ''' <param name="tableid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GetNativeIndexname(indexid As String) As String Implements iormRelationalDatabaseDriver.GetNativeIndexName
            If String.IsNullOrWhiteSpace(Me.Session.CurrentSetupID) Then
                ' create the native name as simple copy of the indexid
                Return indexid
            Else
                ' create the indexname out of the SetupID "_" indexid
                Return Me.Session.CurrentSetupID & "_" & indexid
            End If
        End Function

        ''' <summary>
        ''' returns the native view name in the native database
        ''' </summary>
        ''' <param name="tableid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GetNativeViewname(viewid As String) As String Implements iormRelationalDatabaseDriver.GetNativeViewName
            If String.IsNullOrWhiteSpace(Me.Session.CurrentSetupID) Then
                ' create the native name as simple copy of the viewid
                Return viewid
            Else
                ' create the viewname out of the SetupID "_" viewid
                Return Me.Session.CurrentSetupID & "_" & viewid
            End If
        End Function

        ''' <summary>
        ''' returns the native foreign key name in the native database
        ''' </summary>
        ''' <param name="tableid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GetNativeForeignkeyName(foreignkeyid As String) As String Implements iormRelationalDatabaseDriver.GetNativeForeignKeyName
            If String.IsNullOrWhiteSpace(Me.Session.CurrentSetupID) Then
                ' create the native name as simple copy of the viewid
                Return foreignkeyid
            Else
                ' create the foreignkey name out of the SetupID "_" foreignkeyid
                Return Me.Session.CurrentSetupID & "_" & foreignkeyid
            End If
        End Function

        ''' <summary>
        ''' returns a new visitor object for visiting expression trees
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function GetIXPTVisitor() As rulez.eXPressionTree.IVisitor Implements iormDatabaseDriver.GetIXPTVisitor
        ''' <summary>
        ''' returns a new RDBVisitor for visiting expression trees
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function GetIRDBVisitor() As IRDBVisitor Implements iormRelationalDatabaseDriver.GetIRDBVisitor

    End Class


    ''' <summary>
    ''' defines a abstract relational table store for sql tables and views
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class ormDataReader
        Implements iormRelationalTableStore

        Protected _DBObjectID As String ' Id of the database object
        Protected _DataSchema As iormContainerSchema  'Schema (Description) of the Table or DataStore
        Protected _Connection As iormConnection  ' Connection to use to access the Table or Datastore

        Private _PropertyBag As New Dictionary(Of String, Object)

        '*** Tablestore Cache Property names
        ''' <summary>
        ''' Table Property Name "Cache Property"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstTPNCacheProperty = "CacheDataTable"

        ''' <summary>
        ''' Table Property Name for FULL CACHING
        ''' </summary>
        ''' <remarks></remarks>
        Protected Const ConstTPNFullCaching = "FULL"
        ''' <summary>
        ''' constuctor
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <param name="tableID"></param>
        ''' <param name="force"></param>
        ''' <remarks></remarks>
        Protected Sub New(connection As iormConnection, dbobjectid As String, ByVal force As Boolean)
            Call MyBase.New()
            Me.Connection = connection
            Me.ContainerID = dbobjectid
            Me.Refresh(force:=force)
        End Sub
        ''' <summary>
        ''' creates an unique key value. provide primary key array in the form {field1, field2, nothing}. "Nothing" will be increased.
        ''' </summary>
        ''' <param name="pkArray"></param>
        ''' <remarks></remarks>
        ''' <returns>True if successfull new value</returns>
        Public MustOverride Function CreateUniquePkValue(ByRef pkArray() As Object, Optional tag As String = Nothing) As Boolean Implements iormRelationalTableStore.CreateUniquePkValue


        ''' <summary>
        ''' Refresh
        ''' </summary>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function Refresh(Optional ByVal force As Boolean = False) As Boolean Implements iormRelationalTableStore.Refresh

        ''' <summary>
        ''' returns the native Database Object Name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NativeDBObjectname As String Implements iormRelationalTableStore.NativeDBObjectname
            Get
                '**
                If Not Me.ContainerSchema.IsInitialized Then
                    Return Nothing
                End If
                Return _DataSchema.NativeDBContainerName
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the database object ID.
        ''' </summary>
        ''' <value>The table ID.</value>
        Public Property ContainerID As String Implements iormRelationalTableStore.ContainerID
            Get
                Return Me._DBObjectID
            End Get
            Protected Set(value As String)
                Me._DBObjectID = value.ToUpper
            End Set
        End Property

        ''' <summary>
        ''' Gets the records by SQL command.
        ''' </summary>
        ''' <param name="sqlcommand">The sqlcommand.</param>
        ''' <param name="parameters">The parameters.</param>
        ''' <returns></returns>
        Public MustOverride Function GetRecordsBySqlCommand(ByRef sqlcommand As ormSqlSelectCommand, Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing) As List(Of ormRecord) Implements iormRelationalTableStore.GetRecordsBySqlCommand


        ''' <summary>
        ''' Gets or sets the connection.
        ''' </summary>
        ''' <value>The connection.</value>
        Public Overridable Property Connection() As iormConnection Implements iormRelationalTableStore.Connection
            Get
                Return _Connection
            End Get
            Friend Set(value As iormConnection)
                _Connection = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the DB table schema.
        ''' </summary>
        ''' <value>The DB table schema.</value>
        Public Overridable Property ContainerSchema() As iormContainerSchema Implements iormRelationalTableStore.ContainerSchema
            Get
                Return _DataSchema
            End Get
            Friend Set(value As iormContainerSchema)
                _DataSchema = value
            End Set
        End Property
        ''' <summary>
        ''' sets a Property to the TableStore
        ''' </summary>
        ''' <param name="Name">Name of the Property</param>
        ''' <param name="Object">ObjectValue</param>
        ''' <returns>returns True if succesfull</returns>
        ''' <remarks></remarks>
        Public Function SetProperty(ByVal name As String, ByVal value As Object) As Boolean Implements iormRelationalTableStore.SetProperty
            If _PropertyBag.ContainsKey(name) Then
                _PropertyBag.Remove(name)
            End If
            _PropertyBag.Add(name, value)
            Return True
        End Function
        ''' <summary>
        ''' Gets the Property of a Tablestore
        ''' </summary>
        ''' <param name="name">name of property</param>
        ''' <returns>object of the property</returns>
        ''' <remarks></remarks>
        Public Function GetProperty(ByVal name As String) As Object Implements iormRelationalTableStore.GetProperty
            If _PropertyBag.ContainsKey(name) Then
                Return _PropertyBag.Item(name)
            End If
            Return Nothing
        End Function
        ''' <summary>
        ''' has Tablestore the named property
        ''' </summary>
        ''' <param name="name">name of property</param>
        ''' <returns>return true</returns>
        ''' <remarks></remarks>
        Public Function HasProperty(ByVal name As String) As Boolean Implements iormRelationalTableStore.HasProperty
            Return _PropertyBag.ContainsKey(name)
        End Function
        ''' <summary>
        ''' Dels the record by primary key.
        ''' </summary>
        ''' <param name="aKeyArr">A key arr.</param>
        ''' <param name="silent">The silent.</param>
        ''' <returns></returns>
        Public Overridable Function DeleteRecordByPrimaryKey(ByRef pkArray() As Object, Optional silent As Boolean = False) As Boolean Implements iormRelationalTableStore.DeleteRecordByPrimaryKey
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function
        ''' <summary>
        ''' Runs the SQL command.
        ''' </summary>
        ''' <param name="command">The command.</param>
        ''' <param name="parameters">The parameters.</param>
        ''' <returns></returns>
        '''   
        Public Overridable Function RunSqlCommand(ByRef command As ormSqlCommand, _
                                                  Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing) As Boolean _
            Implements iormRelationalTableStore.RunSqlCommand
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function
        ''' <summary>
        ''' Gets the record by primary key.
        ''' </summary>
        ''' <param name="aKeyArr">A key arr.</param>
        ''' <param name="silent">The silent.</param>
        ''' <returns></returns>
        Public Overridable Function GetRecordByPrimaryKey(ByRef pkArray() As Object, Optional silent As Boolean = False) As ormRecord Implements iormRelationalTableStore.GetRecordByPrimaryKey
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function

        ''' <summary>
        ''' Gets the records by SQL.
        ''' </summary>
        ''' <param name="wherestr">The wherestr.</param>
        ''' <param name="fullsqlstr">The fullsqlstr.</param>
        ''' <param name="innerjoin">The innerjoin.</param>
        ''' <param name="orderby">The orderby.</param>
        ''' <param name="silent">The silent.</param>
        ''' <returns></returns>
        Public Overridable Function GetRecordsBySql(wherestr As String, Optional fullsqlstr As String = Nothing, _
                                                     Optional innerjoin As String = Nothing, Optional orderby As String = Nothing, _
                                                     Optional silent As Boolean = False, Optional ByRef parameters As List(Of ormSqlCommandParameter) = Nothing) As List(Of ormRecord) Implements iormRelationalTableStore.GetRecordsBySql
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' returns records by selection rule
        ''' </summary>
        ''' <param name="rule"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GetRecordsBySelectionRule(rule As rulez.eXPressionTree.SelectionRule) As IEnumerable(Of ormRecord) Implements iormContainerStore.GetRecordsBySelectionRule

        End Function
        ''' <summary>
        ''' Is Linq in this TableStore available
        ''' </summary>
        ''' <value>True if available</value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property IsLinqAvailable As Boolean Implements iormRelationalTableStore.IsLinqAvailable
            Get
                Return False
            End Get
        End Property
        ''' <summary>
        ''' gets a List of ormRecords by SQLCommand
        ''' </summary>
        ''' <param name="id">ID of the Command to store</param>
        ''' <param name="wherestr"></param>
        ''' <param name="fullsqlstr"></param>
        ''' <param name="innerjoin"></param>
        ''' <param name="orderby"></param>
        ''' <param name="silent"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GetRecordsbySQlCommand(id As String, Optional wherestr As String = Nothing, Optional fullsqlstr As String = Nothing, _
                                               Optional innerjoin As String = Nothing, Optional orderby As String = Nothing, Optional silent As Boolean = False, _
                                               Optional ByRef parameters As List(Of ormSqlCommandParameter) = Nothing) As List(Of ormRecord) _
                                           Implements iormRelationalTableStore.GetRecordsBySqlCommand
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function
        ''' <summary>
        ''' Gets the index of the records by.
        ''' </summary>
        ''' <param name="indexname">The indexname.</param>
        ''' <param name="aKeyArr">A key arr.</param>
        ''' <param name="silent">The silent.</param>
        ''' <returns></returns>
        Public Overridable Function GetRecordsByIndex(indexname As String, ByRef keysArray As Object(), Optional silent As Boolean = False) As List(Of ormRecord) Implements iormRelationalTableStore.GetRecordsByIndex
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function

        ''' <summary>
        ''' Infuses the record.
        ''' </summary>
        ''' <param name="aNewEnt">A new ent.</param>
        ''' <param name="aRecordSet">A record set.</param>
        ''' <param name="silent">The silent.</param>
        ''' <returns></returns>
        Public Overridable Function InfuseRecord(ByRef newRecord As ormRecord, ByRef RowObject As Object, Optional ByVal silent As Boolean = False, Optional CreateNewRecord As Boolean = False) As Boolean Implements iormRelationalTableStore.InfuseRecord
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function

        ''' <summary>
        ''' Persists the record.
        ''' </summary>
        ''' <param name="aRecord">A record.</param>
        ''' <param name="aTimestamp">A timestamp.</param>
        ''' <param name="silent">The silent.</param>
        ''' <returns></returns>
        Public Overridable Function PersistRecord(ByRef record As ormRecord, Optional timestamp As DateTime = ot.ConstNullDate, Optional ByVal silent As Boolean = False) As Boolean Implements iormRelationalTableStore.PersistRecord
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function

        ''' <summary>
        ''' Runs the SQL command.
        ''' </summary>
        ''' <param name="sqlcmdstr">The SQLCMDSTR.</param>
        ''' <param name="silent">The silent.</param>
        ''' <returns></returns>
        Public Overridable Function RunSQLStatement(sqlcmdstr As String, _
                                                    Optional ByRef parameters As List(Of ormSqlCommandParameter) = Nothing, _
                                                    Optional silent As Boolean = True) As Boolean _
            Implements iormRelationalTableStore.RunSqlStatement
            Throw New NotImplementedException()
        End Function

        ''' <summary>
        ''' converts an object value to column data
        ''' </summary>
        ''' <param name="invalue"></param>
        ''' <param name="outvalue"></param>
        ''' <param name="targetType"></param>
        ''' <param name="maxsize"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <param name="fieldname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function Convert2ContainerData(ByVal invalue As Object, ByRef outvalue As Object, _
                                                    targetType As Long, _
                                                    Optional ByVal maxsize As Long = 0, _
                                                   Optional ByRef abostrophNecessary As Boolean = False, _
                                                   Optional ByVal fieldname As String = Nothing, _
                                                    Optional isnullable As Boolean? = Nothing, _
                                                    Optional defaultvalue As Object = Nothing _
                                                ) As Boolean Implements iormRelationalTableStore.Convert2ContainerData


        ''' <summary>
        ''' Convert2s the column data.
        ''' </summary>
        ''' <param name="anIndex">An index.</param>
        ''' <param name="aVAlue">A V alue.</param>
        ''' <param name="abostrophNecessary">The abostroph necessary.</param>
        ''' <returns></returns>
        Public Overridable Function Convert2ContainerData(index As Object, ByVal invalue As Object, ByRef outvalue As Object, _
                                                       Optional ByRef abostrophNecessary As Boolean = False, _
                                                       Optional isnullable As Boolean? = Nothing, _
                                                    Optional defaultvalue As Object = Nothing _
                                                ) As Boolean Implements iormRelationalTableStore.Convert2ContainerData
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function

        ''' <summary>
        ''' Convert2s the object data.
        ''' </summary>
        ''' <param name="anIndex">An index.</param>
        ''' <param name="aVAlue">A V alue.</param>
        ''' <param name="abostrophNecessary">The abostroph necessary.</param>
        ''' <returns></returns>
        Public Overridable Function Convert2ObjectData(index As Object, _
                                                       ByVal invalue As Object, _
                                                       ByRef outvalue As Object, _
                                                       Optional isnullable As Boolean? = Nothing, _
                                                        Optional defaultvalue As Object = Nothing, _
                                                       Optional ByRef abostrophNecessary As Boolean = False) As Boolean Implements iormRelationalTableStore.Convert2ObjectData
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function
        ''' <summary>
        ''' checks if SqlCommand is in Store of the driver
        ''' </summary>
        ''' <param name="id">id of the command</param>
        ''' <returns>True if successful</returns>
        ''' <remarks></remarks>
        Public Overridable Function HasSqlCommand(id As String) As Boolean Implements iormRelationalTableStore.HasSqlCommand
            Throw New NotImplementedException()
        End Function
        ''' <summary>
        ''' Store the Command by its ID - replace if existing
        ''' </summary>
        ''' <param name="sqlCommand">a iOTDBSqlCommand</param>
        ''' <returns>true if successfull</returns>
        ''' <remarks></remarks>
        Public Overridable Function StoreSqlCommand(ByRef sqlCommand As iormSqlCommand) As Boolean Implements iormRelationalTableStore.StoreSqlCommand
            sqlCommand.ID = Me.GetSqlCommandID(sqlCommand.ID)

            Dim anExistingSqlCommand As iormSqlCommand
            If Me.Connection.DatabaseDriver.HasSqlCommand(sqlCommand.ID) Then
                anExistingSqlCommand = Me.Connection.DatabaseDriver.RetrieveSqlCommand(sqlCommand.ID)
                If anExistingSqlCommand.BuildVersion > sqlCommand.BuildVersion Then
                    Call CoreMessageHandler(messagetype:=otCoreMessageType.InternalWarning, procedure:="ormDataStore.StoreSQLCommand", argument:=sqlCommand.ID, _
                                           message:=" SqlCommand in Store has higher buildversion as the one to save ?! - not saved")
                    Return False
                End If
            End If

            Me.Connection.DatabaseDriver.StoreSqlCommand(sqlCommand)
            Return True
        End Function
        ''' <summary>
        ''' Retrieve the Command from Store
        ''' </summary>
        ''' <param name="id">id of the command</param>
        ''' <returns>a iOTDBSqlCommand</returns>
        ''' <remarks></remarks>
        Public Overridable Function RetrieveSqlCommand(id As String) As iormSqlCommand Implements iormRelationalTableStore.RetrieveSqlCommand
            '* get the ID
            id = Me.GetSqlCommandID(id)
            If Me.Connection.DatabaseDriver.HasSqlCommand(id) Then
                Return Me.Connection.DatabaseDriver.RetrieveSqlCommand(id)
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' Creates a Command and store it or gets the current Command
        ''' </summary>
        ''' <param name="id">id of the command</param>
        ''' <returns>a iOTDBSqlCommand</returns>
        ''' <remarks></remarks>
        Public Overridable Function CreateSqlCommand(id As String) As iormSqlCommand Implements iormRelationalTableStore.CreateSqlCommand
            '* get the ID
            id = Me.GetSqlCommandID(id)
            If Me.Connection.DatabaseDriver.HasSqlCommand(id) Then
                Return Me.Connection.DatabaseDriver.RetrieveSqlCommand(id)
            Else
                Dim aSqlCommand As iormSqlCommand = New ormSqlCommand(id)
                Me.Connection.DatabaseDriver.StoreSqlCommand(aSqlCommand)
                Return aSqlCommand
            End If
        End Function
        ''' <summary>
        ''' Creates a Command and store it or gets the current Command
        ''' </summary>
        ''' <param name="id">id of the command</param>
        ''' <returns>a iOTDBSqlCommand</returns>
        ''' <remarks></remarks>
        Public Overridable Function CreateSqlSelectCommand(id As String, Optional addMe As Boolean = True, Optional addAllFields As Boolean = True) As iormSqlCommand Implements iormRelationalTableStore.CreateSqlSelectCommand
            '* get the ID
            id = Me.GetSqlCommandID(id)
            If Me.Connection.DatabaseDriver.HasSqlCommand(id) Then
                Return Me.Connection.DatabaseDriver.RetrieveSqlCommand(id)
            Else
                Dim aSqlCommand As iormSqlCommand = New ormSqlSelectCommand(id)
                Me.Connection.DatabaseDriver.StoreSqlCommand(aSqlCommand)
                If addMe Then
                    DirectCast(aSqlCommand, ormSqlSelectCommand).AddTable(tableid:=Me.ContainerID, addAllFields:=addAllFields)
                End If
                Return aSqlCommand
            End If
        End Function
        ''' <summary>
        ''' returns a ID for this Tablestore. Add the name of the table in front of the ID
        ''' </summary>
        ''' <param name="id">SqlcommandID</param>
        ''' <returns>the id</returns>
        ''' <remarks></remarks>
        Public Function GetSqlCommandID(id As String) As String
            If Not id.ToLower.Contains((LCase(Me.ContainerID & "."))) Then
                Return Me.ContainerID & "." & id
            Else
                Return id
            End If
        End Function
    End Class

    ''' <summary>
    ''' TopLevel abstract ViewReader Class
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class ormViewReader
        Inherits ormDataReader
        Implements iormRelationalTableStore


        ''' <summary>
        ''' constuctor
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <param name="tableID"></param>
        ''' <param name="force"></param>
        ''' <remarks></remarks>
        Protected Sub New(connection As iormConnection, viewid As String, ByVal force As Boolean)
            Call MyBase.New(connection:=connection, dbobjectid:=viewid, force:=force)
        End Sub


        ''' <summary>
        ''' Refresh
        ''' </summary>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Refresh(Optional ByVal force As Boolean = False) As Boolean Implements iormRelationalTableStore.Refresh
            ''' TODO: on Connection Refresh
            '** 
            If Not Connection Is Nothing AndAlso (Connection.IsConnected OrElse Connection.Session.IsBootstrappingInstallationRequested) Then

                ''** all cache properties for tables used in starting up will be determined
                ''** by schema
                'If CurrentSession.IsStartingUp Then
                '    Dim aTable = ot.GetSchemaTableAttribute(Me.ViewID)
                '    If aTable IsNot Nothing Then
                '        If aTable.HasValueUseCache AndAlso aTable.UseCache Then
                '            If Not aTable.HasValueCacheProperties Then
                '                Me.SetProperty(ConstTPNCacheProperty, ConstTPNFullCaching)
                '            Else
                '                '** set properties
                '                Dim ext As String = String.empty
                '                Dim i As Integer = 0
                '                For Each aproperty In aTable.CacheProperties
                '                    Me.SetProperty(ConstTPNCacheProperty & ext, aproperty)
                '                    ext = i.ToString
                '                    i += 1
                '                Next

                '            End If
                '        End If

                '    End If
                '    '** set the cache property if running from the object definitions
                'ElseIf CurrentSession.IsRunning Then
                '    Dim aTable = CurrentSession.Objects.GetTable(tablename:=Me.ViewID)
                '    If aTable IsNot Nothing Then
                '        If aTable.UseCache And aTable.CacheProperties.Count = 0 Then
                '            Me.SetProperty(ConstTPNCacheProperty, ConstTPNFullCaching)
                '        Else
                '            '** set properties
                '            Dim ext As String = String.empty
                '            Dim i As Integer = 0
                '            For Each aproperty In aTable.CacheProperties
                '                Me.SetProperty(ConstTPNCacheProperty & ext, aproperty)
                '                ext = i.ToString
                '                i += 1
                '            Next

                '        End If
                '    End If
                'End If

                '** create and assign the table schema
                If Me.ViewSchema Is Nothing OrElse force Then Me._DataSchema = Connection.DatabaseDriver.GetViewSchema(Me.ViewID, force:=force)
                If ViewSchema Is Nothing OrElse Not ViewSchema.IsInitialized Then
                    Call CoreMessageHandler(break:=True, message:=" Schema for TableID '" & Me.ViewID & "' couldnot be loaded", containerID:=Me.ViewID, _
                                          messagetype:=otCoreMessageType.InternalError, procedure:="ormViewReader.Refresh")
                    Return False
                End If
            End If
        End Function

        ''' <summary>
        ''' returns the native Tablename of this store from the schema
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NativeViewName As String Implements iormRelationalTableStore.NativeDBObjectname
            Get
                '**
                If Not Me.ContainerSchema.IsInitialized Then
                    Return Nothing
                End If
                Return Me.ContainerSchema.NativeDBContainerName
            End Get
        End Property

        ''' <summary>
        ''' return the associated Tableschema
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ViewSchema As iormContainerSchema
            Get
                Return Me.ContainerSchema
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the view ID.
        ''' </summary>
        ''' 
        ''' <value>The view ID.</value>
        Public Property ViewID As String Implements iormRelationalTableStore.ContainerID
            Get
                Return MyBase.ContainerID
            End Get
            Protected Set(value As String)
                MyBase.ContainerID = value.ToUpper
            End Set
        End Property


    End Class


    ''' <summary>
    ''' TopLevel OTDB Tablestore implementation base class
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class ormTableStore
        Inherits ormDataReader
        Implements iormRelationalTableStore

        ''' <summary>
        ''' Table Property Name "Cache Update Instant"
        ''' </summary>
        ''' <remarks></remarks>
        Public Const ConstTPNCacheUpdateInstant = "CacheDataTableUpdateImmediatly"

        ''' <summary>
        ''' constuctor
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <param name="tableID"></param>
        ''' <param name="force"></param>
        ''' <remarks></remarks>
        Protected Sub New(connection As iormConnection, tableID As String, ByVal force As Boolean)
            Call MyBase.New(connection:=connection, dbobjectid:=tableID, force:=force)
        End Sub
        ''' <summary>
        ''' creates an unique key value. provide primary key array in the form {field1, field2, nothing}. "Nothing" will be increased.
        ''' </summary>
        ''' <param name="pkArray"></param>
        ''' <remarks></remarks>
        ''' <returns>True if successfull new value</returns>
        Public Overrides Function CreateUniquePkValue(ByRef pkArray() As Object, Optional tag As String = Nothing) As Boolean Implements iormRelationalTableStore.CreateUniquePkValue

            '**
            If Not Me.ContainerSchema.IsInitialized Then
                Return False
            End If

            '** redim 
            ReDim Preserve pkArray(Me.ContainerSchema.NoPrimaryEntries() - 1)
            Dim anIndex As UShort = 0
            Dim keyfieldname As String

            Try
                ' get
                Dim aStore As iormRelationalTableStore = GetPrimaryTableStore(Me.TableID)
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(id:="CreateUniquePkValue" & tag, addMe:=True, addAllFields:=False)

                '** prepare the command if necessary

                ''' this command lives from the first call !! -> all elements in pkArray not fixed will be regarded as elements to be fixed
                If Not aCommand.IsPrepared Then
                    '* retrieve the maximum field
                    For Each pkvalue In pkArray
                        If pkvalue Is Nothing Then
                            keyfieldname = ContainerSchema.GetPrimaryEntryNames(anIndex + 1)
                            Exit For
                        End If
                        anIndex += 1
                    Next
                    '*
                    aCommand.select = "max( [" & keyfieldname & "] )"
                    If anIndex > 0 Then
                        For j = 0 To anIndex - 1 ' an index points to the keyfieldname, parameter is the rest
                            If j > 0 Then aCommand.Where &= " AND "
                            aCommand.Where &= "[" & ContainerSchema.GetPrimaryEntryNames(j + 1) & "] = @" & ContainerSchema.GetPrimaryEntryNames(j + 1)
                            aCommand.AddParameter(New ormSqlCommandParameter(ID:="@" & ContainerSchema.GetPrimaryEntryNames(j + 1), _
                                                                                 columnname:=ContainerSchema.GetPrimaryEntryNames(j + 1), tableid:=Me.TableID))
                        Next
                    End If
                    aCommand.Prepare()
                End If

                '* retrieve the maximum field -> and sets the index
                anIndex = 0
                For Each pkvalue In pkArray
                    If Not pkvalue Is Nothing Then
                        aCommand.SetParameterValue(ID:="@" & ContainerSchema.GetPrimaryEntryNames(anIndex + 1), value:=pkvalue)
                    Else
                        Exit For
                    End If
                    anIndex += 1
                Next
                '** run the Command
                Dim theRecords As List(Of ormRecord) = aCommand.RunSelect

                '*** increments ! -> need to be incrementable
                If theRecords.Count > 0 Then
                    ' returns always one field Max !
                    If Not DBNull.Value.Equals(theRecords.Item(0).GetValue(1)) AndAlso IsNumeric(theRecords.Item(0).GetValue(1)) Then
                        pkArray(anIndex) = CLng(theRecords.Item(0).GetValue(1)) + 1
                        Return True
                    Else
                        pkArray(anIndex) = CLng(1)
                        Return True
                    End If

                Else
                    pkArray(anIndex) = CLng(1)
                    Return True
                End If

            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=False, exception:=ex, procedure:="ormTableStore.CreateUniquePkValue")
                Return False
            End Try


        End Function

        ''' <summary>
        ''' Refresh
        ''' </summary>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Refresh(Optional ByVal force As Boolean = False) As Boolean Implements iormRelationalTableStore.Refresh
            ''' TODO: on Connection Refresh
            '** 
            If Connection IsNot Nothing AndAlso (Connection.IsConnected OrElse Connection.Session.IsBootstrappingInstallationRequested) Then

                '** all cache properties for tables used in starting up will be determined
                '** by schema
                If CurrentSession.IsStartingUp Then
                    Dim aTable = ot.GetSchemaTableAttribute(Me.TableID)
                    If aTable IsNot Nothing Then
                        If aTable.HasValueUseCache AndAlso aTable.UseCache Then
                            If Not aTable.HasValueCacheProperties Then
                                Me.SetProperty(ConstTPNCacheProperty, ConstTPNFullCaching)
                            Else
                                '** set properties
                                Dim ext As String = String.Empty
                                Dim i As Integer = 0
                                For Each aproperty In aTable.CacheProperties
                                    Me.SetProperty(ConstTPNCacheProperty & ext, aproperty)
                                    ext = i.ToString
                                    i += 1
                                Next

                            End If
                        End If

                    End If
                    '** set the cache property if running from the object definitions
                ElseIf CurrentSession.IsRunning Then
                    Dim aTable = CurrentSession.Objects.GetContainerDefinition(id:=Me.TableID)
                    If aTable IsNot Nothing Then
                        If aTable.UseCache And aTable.CacheProperties.Count = 0 Then
                            Me.SetProperty(ConstTPNCacheProperty, ConstTPNFullCaching)
                        Else
                            '** set properties
                            Dim ext As String = String.Empty
                            Dim i As Integer = 0
                            For Each aproperty In aTable.CacheProperties
                                Me.SetProperty(ConstTPNCacheProperty & ext, aproperty)
                                ext = i.ToString
                                i += 1
                            Next

                        End If
                    End If
                End If

                '** create and assign the table schema
                If Me.TableSchema Is Nothing OrElse force Then Me._DataSchema = Connection.DatabaseDriver.RetrieveContainerSchema(Me.TableID, force:=force)
                If TableSchema Is Nothing OrElse Not TableSchema.IsInitialized Then
                    Call CoreMessageHandler(break:=True, message:=" Schema for TableID '" & Me.TableID & "' couldnot be loaded", containerID:=Me.TableID, _
                                          messagetype:=otCoreMessageType.InternalError, procedure:="ormTableStore.Refresh")
                    Return False
                End If
            End If
        End Function

        ''' <summary>
        ''' returns the native Tablename of this store from the schema
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NativeViewName As String Implements iormRelationalTableStore.NativeDBObjectname
            Get
                '**
                If Not Me.ContainerSchema.IsInitialized Then
                    Return Nothing
                End If
                Return Me.ContainerSchema.NativeDBContainerName
            End Get
        End Property

        ''' <summary>
        ''' return the associated Tableschema
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TableSchema As iormContainerSchema
            Get
                Return Me.ContainerSchema
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the table ID.
        ''' </summary>
        ''' 
        ''' <value>The table ID.</value>
        Public Property TableID As String Implements iormRelationalTableStore.ContainerID
            Get
                Return MyBase.ContainerID
            End Get
            Protected Set(value As String)
                MyBase.ContainerID = value.ToUpper
            End Set
        End Property


    End Class

    ''' <summary>
    ''' describes the schema independent of the base database
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class ormViewSchema
        Inherits ormContainerSchema
        Implements iormRelationalSchema

        ''' <summary>
        ''' List of Tables a View relies on
        ''' </summary>
        ''' <remarks></remarks>
        Protected _tableschemas As List(Of iormContainerSchema)

        ''' <summary>
        ''' constuctor
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <param name="tableID"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef connection As iormConnection, ByVal viewid As String)
            MyBase.New(connection:=connection, dbobjectid:=viewid)
        End Sub


        ''' <summary>
        ''' returns the tableid of the table
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public ReadOnly Property ViewID() As String Implements iormContainerSchema.ContainerID
            Get
                Return MyBase.ContainerID
            End Get
        End Property
        ''' <summary>
        ''' returns the native tablename of this table in the database
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NativeViewname As String Implements iormContainerSchema.NativeDBContainerName
            Get
                Return MyBase.NativeDBObjectname
            End Get
        End Property
        ''' <summary>
        ''' Assign a native DB parameters and return
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <param name="parametername"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function AssignNativeDBParameter(columnname As String, Optional parametername As String = Nothing) As System.Data.IDbDataParameter Implements iormRelationalSchema.AssignNativeDBParameter
    End Class
    ''' <summary>
    ''' describes the schema independent of the base database
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class ormTableSchema
        Inherits ormContainerSchema
        Implements iormRelationalSchema


        ''' <summary>
        ''' constuctor
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <param name="tableID"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef connection As iormConnection, ByVal tableID As String)
            MyBase.New(connection:=connection, dbobjectid:=tableID)
        End Sub


        ''' <summary>
        ''' resets the TableSchema to hold nothing
        ''' </summary>
        ''' <remarks></remarks>
        Protected Overridable Sub Reset()
            MyBase.Reset()
        End Sub

        ''' <summary>
        ''' returns the tableid of the table
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public ReadOnly Property TableID() As String Implements iormContainerSchema.ContainerID
            Get
                Return MyBase.ContainerID
            End Get
        End Property
        ''' <summary>
        ''' returns the native tablename of this table in the database
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NativeTablename As String Implements iormContainerSchema.NativeDBContainerName
            Get
                Return MyBase.NativeDBObjectname
            End Get
        End Property



        '**** primaryKeyIndexName
        ''' <summary>
        ''' gets the primarykey name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property PrimaryKeyIndexName As String Implements iormContainerSchema.PrimaryKeyIndexName
            Get
                PrimaryKeyIndexName = _PrimaryKeyIndexName
            End Get
        End Property

        ''' <summary>
        ''' Assign a native DB parameters and return
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <param name="parametername"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function AssignNativeDBParameter(columnname As String, Optional parametername As String = Nothing) As System.Data.IDbDataParameter Implements iormRelationalSchema.AssignNativeDBParameter

    End Class

End Namespace