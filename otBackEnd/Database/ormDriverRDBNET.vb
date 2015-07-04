REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** Driver Wrapper for ADO.NET Base Classes for On Track Database Backend Library
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2013-08-08
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************''' <summary>


Option Explicit On
Imports System.Collections
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Diagnostics.Debug
Imports System.Data
Imports OTDB
Imports System.Text.RegularExpressions
Imports OnTrack
Imports OnTrack.UI
Imports OnTrack.Commons
Imports OnTrack.Core
Imports System.Data.SqlClient
Imports System.Data.OleDb

Namespace OnTrack.Database


    ''' <summary>
    ''' Extension Module to the ormRecord for loading from a ADO.NET Data structure to the record
    ''' </summary>
    ''' <remarks></remarks>
    Module ormRecordExtension

        ''' <summary>
        ''' load a record into this record from the datareader
        ''' </summary>
        ''' <param name="datareader"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <System.Runtime.CompilerServices.Extension> _
        Public Function LoadFrom(ByRef this As ormRecord, ByRef datareader As IDataReader, Optional InSync As Boolean = False) As Boolean
            Dim result As Boolean = True

            Try
                ''' if tableset then only check which fields are in the datareader
                ''' 
                this.IsLoaded = True ' important

                If this.IsBound Then

                    ''' read it at once if only one tablestore
                    ''' 
                    If InSync Then
                        datareader.GetValues(this.ValuesArray)
                        result = True
                    Else
                        ''' go through each tablestore
                        For n = 0 To this.ContainerStores.Length - 1
                            For j = 1 To this.ContainerStores(n).ContainerSchema.NoEntries
                                Dim found As Integer = -1
                                Dim aColumnname As String = this.ContainerStores(n).ContainerSchema.GetEntryName(j)
                                For i = 0 To datareader.FieldCount - 1
                                    If datareader.GetName(i) = aColumnname Then
                                        ''' uuuh slow
                                        ''' 
                                        found = i
                                        Exit For
                                    End If
                                Next
                                If found >= 0 Then
                                    Dim aValue As Object
                                    Dim index As Integer = this.ZeroBasedIndexOf(this.ContainerStores(n).ContainerID & "." & aColumnname) + 1
                                    If index >= 0 Then
                                        If this.ContainerStores(n).Convert2ObjectData(index:=j, invalue:=datareader.Item(found), outvalue:=aValue) Then
                                            If Not this.SetValue(index, aValue) Then
                                                CoreMessageHandler(message:="set value failed", argument:=aValue, containerEntryName:=aColumnname, containerID:=this.ContainerIDS(n), procedure:="ormRecord.LoadFrom")
                                                result = False
                                            Else
                                                result = result And True
                                            End If
                                        Else
                                            CoreMessageHandler(message:="data conversion failed", argument:=datareader.Item(aColumnname), containerEntryName:=aColumnname, _
                                                               containerID:=this.ContainerIDS(n), procedure:="ormRecord.LoadFrom")
                                            result = False
                                        End If
                                    Else
                                        CoreMessageHandler(message:="index in record failed - canonical name doesnot exist ?", _
                                                           argument:=datareader.Item(aColumnname), containerEntryName:=aColumnname, containerID:=this.ContainerIDS(n), procedure:="ormRecord.LoadFrom")
                                        result = False
                                    End If

                                Else
                                    CoreMessageHandler(message:="column from table not in datareader - record uncomplete", containerEntryName:=aColumnname, _
                                                       containerID:=this.ContainerIDS(n), procedure:="ormRecord.LoadFrom(IDataReader)")
                                    result = False
                                End If
                            Next j
                        Next

                    End If

                    Return result
                Else
                    ''' take all the values from datareader and move it 
                    ''' 
                    For j = 0 To datareader.FieldCount - 1
                        Dim aName As String = datareader.GetName(j)
                        If aName = String.Empty Then aName = "column" & j.ToString
                        Dim aValue As Object = datareader.Item(j)

                        ''' how to convert ?!
                        ''' we have already system type

                        If Not this.SetValue(aName.ToString, aValue) Then
                            CoreMessageHandler(message:="could not set value from data reader", argument:=aValue, _
                                                messagetype:=otCoreMessageType.InternalError, procedure:="ormRecord.LoadFrom(IDataReader)")
                            result = False
                        Else
                            result = result And True
                        End If
                    Next

                    Return result
                End If


            Catch ex As Exception
                Call CoreMessageHandler(procedure:="ormRecord.LoadFrom(IDataReader)", exception:=ex, message:="Exception", _
                                      argument:=this.ContainerIDS)
                Return False
            End Try

        End Function
        ''' <summary>
        ''' load a record into this record from the datareader
        ''' </summary>
        ''' <param name="datareader"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <System.Runtime.CompilerServices.Extension> _
        Public Function LoadFrom(ByRef this As ormRecord, ByRef datarow As DataRow) As Boolean
            Dim result As Boolean = True
            Try
                ''' if tableset then only check which fields are in the datareader
                ''' 
                this.IsLoaded = True ' important

                If this.IsBound Then
                    Dim flagColumnNameCheck As Boolean = False
                    If this.ContainerStores.Length > 1 OrElse datarow.Table.TableName.ToUpper <> this.ContainerStores(0).ContainerSchema.ContainerID.ToUpper Then
                        flagColumnNameCheck = True
                    End If

                    ''' run through
                    For n = 0 To this.ContainerStores.Length - 1
                        For j = 1 To this.ContainerStores(n).ContainerSchema.NoEntries
                            Dim aColumnname As String = this.ContainerStores(n).ContainerSchema.GetEntryName(j)
                            If datarow.Table.Columns.Contains(aColumnname) Then
                                Dim aValue As Object = datarow.Item(aColumnname)
                                If flagColumnNameCheck AndAlso this.ZeroBasedIndexOf(this.ContainerStores(n).ContainerID & "." & aColumnname) < 0 Then
                                    CoreMessageHandler(message:="column doesnot exist in record ?!", argument:=datarow.Item(aColumnname), _
                                                        containerEntryName:=aColumnname, containerID:=datarow.Table.TableName, procedure:="ormRecord.LoadFrom(Datarow)")
                                    '''convert and set the value
                                ElseIf this.ContainerStores(n).Convert2ObjectData(index:=j, invalue:=datarow.Item(aColumnname), outvalue:=aValue) Then
                                    If Not this.SetValue(j, aValue) Then
                                        CoreMessageHandler(message:="could not set value from data reader", argument:=aValue, _
                                                           containerEntryName:=aColumnname, containerID:=datarow.Table.TableName, procedure:="ormRecord.LoadFrom(Datarow)")
                                        result = False
                                    Else
                                        result = result And True
                                    End If
                                Else
                                    CoreMessageHandler(message:="could not convert value from data reader", argument:=datarow.Item(aColumnname), _
                                                       containerEntryName:=aColumnname, containerID:=datarow.Table.TableName, procedure:="ormRecord.LoadFrom(Datarow)")
                                    result = False
                                End If

                            Else
                                CoreMessageHandler(message:="column from table not in datareader - record uncomplete", containerEntryName:=aColumnname, _
                                                   containerID:=datarow.Table.TableName, procedure:="ormRecord.LoadFrom(Datarow)")
                                result = False
                            End If
                        Next j
                    Next


                    Return result
                Else
                    ''' take all the values from datareader and move it 
                    ''' 
                    For j = 0 To datarow.Table.Columns.Count - 1
                        Dim aColumnname As String = datarow.Table.Columns.Item(j).ColumnName
                        Dim aValue As Object = datarow.Item(j)

                        ''' how to convert ?!
                        ''' 
                        ''' datarow has system types !!
                        ''' Dim Outvalue = CTypeDynamic (avalue, atype)
                        '''
                        If Not this.SetValue(datarow.Table.TableName.ToUpper & "." & aColumnname.ToUpper, aValue) Then
                            CoreMessageHandler(message:="could not set value from data reader", argument:=aValue, _
                                               containerEntryName:=aColumnname, containerID:=datarow.Table.TableName, procedure:="ormRecord.LoadFrom(Datarow)")
                            result = False
                        Else
                            result = True
                        End If
                    Next

                    Return result
                End If

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="ormRecord.LoadFrom(Datarow)", exception:=ex, message:="Exception", containerID:=datarow.Table.TableName)
                Return False
            End Try

        End Function
    End Module

    ''' <summary>
    ''' describes a ado.net column
    ''' </summary>
    ''' <remarks></remarks>
    Public Class adonetColumnDescription
        Private _Description As String
        Private _ColumnName As String
        Private _IsNullable As Boolean
        Private _Ordinal As UShort
        Private _CharacterMaxLength As Nullable(Of Int64)
        Private _HasDefault As Boolean
        Private _Default As String
        Private _DataType As Long
        Private _Catalog As String
        Private _NumericPrecision As Nullable(Of Int64)
        Private _NumericScale As Nullable(Of Int64)
        Private _DateTimePrecision As Nullable(Of Int64)
        Private _CharachterOctetLength As Nullable(Of Int64)

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ColumnDescription" /> class.
        ''' </summary>
        ''' <param name="characterMaxLength">Length of the character max.</param>
        Public Sub New()

        End Sub

#Region "Properties"


        ''' <summary>
        ''' Gets or sets the length of the charachter octet.
        ''' </summary>
        ''' <value>The length of the charachter octet.</value>
        Public Property CharachterOctetLength() As Nullable(Of Int64)
            Get
                Return Me._CharachterOctetLength
            End Get
            Set(value As Nullable(Of Int64))
                Me._CharachterOctetLength = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the date time precision.
        ''' </summary>
        ''' <value>The date time precision.</value>
        Public Property DateTimePrecision() As Nullable(Of Int64)
            Get
                Return Me._DateTimePrecision
            End Get
            Set(value As Nullable(Of Int64))
                Me._DateTimePrecision = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the numeric scale.
        ''' </summary>
        ''' <value>The numeric scale.</value>
        Public Property NumericScale() As Nullable(Of Int64)
            Get
                Return Me._NumericScale
            End Get
            Set(value As Nullable(Of Int64))
                Me._NumericScale = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the numeric precision.
        ''' </summary>
        ''' <value>The numeric precision.</value>
        Public Property NumericPrecision() As Nullable(Of Int64)
            Get
                Return Me._NumericPrecision
            End Get
            Set(value As Nullable(Of Int64))
                Me._NumericPrecision = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the catalog.
        ''' </summary>
        ''' <value>The catalog.</value>
        Public Property Catalog() As String
            Get
                Return Me._Catalog
            End Get
            Set(value As String)
                Me._Catalog = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the type of the data.
        ''' </summary>
        ''' <value>The type of the data.</value>
        Public Overridable Property DataType() As Long
            Get
                Return Me._DataType
            End Get
            Set(value As Long)
                Me._DataType = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the default.
        ''' </summary>
        ''' <value>The default.</value>
        Public Property [Default]() As String
            Get
                Return Me._Default
            End Get
            Set(value As String)
                Me._Default = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the has default.
        ''' </summary>
        ''' <value>The has default.</value>
        Public Property HasDefault() As Boolean
            Get
                Return Me._HasDefault
            End Get
            Set(value As Boolean)
                Me._HasDefault = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the length of the character max.
        ''' </summary>
        ''' <value>The length of the character max.</value>
        Public Property CharacterMaxLength() As Nullable(Of Int64)
            Get
                Return Me._CharacterMaxLength
            End Get
            Set(value As Nullable(Of Int64))
                Me._CharacterMaxLength = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the ordinal.
        ''' </summary>
        ''' <value>The ordinal.</value>
        Public Property Ordinal() As UShort
            Get
                Return Me._Ordinal
            End Get
            Set(value As UShort)
                Me._Ordinal = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the is nullable.
        ''' </summary>
        ''' <value>The is nullable.</value>
        Public Property IsNullable() As Boolean
            Get
                Return Me._IsNullable
            End Get
            Set(value As Boolean)
                Me._IsNullable = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the column.
        ''' </summary>
        ''' <value>The name of the column.</value>
        Public Property ColumnName() As String
            Get
                Return Me._ColumnName
            End Get
            Set(value As String)
                Me._ColumnName = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the description.
        ''' </summary>
        ''' <value>The description.</value>
        Public Property Description() As String
            Get
                Return Me._Description
            End Get
            Set(value As String)
                Me._Description = value
            End Set
        End Property
#End Region
    End Class


    ''' <summary>
    ''' abstract ado.net relational database driver
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class AdoNetRDBDriver
        Inherits ormRDBDriver
        Implements iormRelationalDatabaseDriver

        Protected _currentUserValidation As New UserValidation
        'Protected Friend Shadows WithEvents _primaryConnection As iOTDBConnection '-> in clsOTDBDriver
        Protected _OnTrackDataSet As DataSet

        Protected _ParametersTableAdapter As System.Data.IDbDataAdapter
        Protected _ParametersTable As DataTable = Nothing 'initialize must assign this - important to determine if parameters will be written to cache or to table
        Protected _parametersTableName As String = ConstDBParameterTableName

        Protected _IsInitialized As Boolean = False
        Protected _ErrorLogPersistCommand As IDbCommand = Nothing
        Protected _ErrorLogPersistTableschema As iormContainerSchema = Nothing

        Protected _BootStrapParameterCache As New Dictionary(Of String, Object) ' during bootstrap use this 

        Protected _isInstalling As Boolean = False ' flag to see if we are in Install-Mode
        Protected _lock As New Object 'lockObject for driver instance
        Shadows Event RequestBootstrapInstall(sender As Object, e As SessionBootstrapEventArgs) Implements iormRelationalDatabaseDriver.RequestBootstrapInstall

        ''' <summary>
        ''' construcotr
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            Call MyBase.New()
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ID">an ID for this driver</param>
        ''' <remarks></remarks>
        Public Sub New(ID As String, ByRef session As Session)
            Call MyBase.New(ID, session)
        End Sub

        ''' <summary>
        ''' Gets the on track data set.
        ''' </summary>
        ''' <value>The on track data set.</value>
        Public ReadOnly Property OnTrackDataSet() As DataSet
            Get
                Return Me._OnTrackDataSet
            End Get
        End Property

        ''' <summary>
        ''' returns True if driver is initialized.
        ''' </summary>
        ''' <value></value>
        Public Property IsInitialized() As Boolean
            Get
                Return Me._IsInitialized
            End Get
            Protected Friend Set(value As Boolean)
                _IsInitialized = value
            End Set
        End Property

        ''' <summary>
        ''' gets the native connection
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Overloads Property NativeConnection As IDbConnection

        ''' <summary>
        ''' Returns the Parameter Tablename
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property DBParameterTablename As String Implements iormRelationalDatabaseDriver.DBParameterContainerName
            Get
                Return ConstDBParameterTableName
            End Get
        End Property

        '' <summary>
        ''' Gets the native database name 
        ''' </summary>
        ''' <value>The type.</value>
        Public Overrides ReadOnly Property NativeDatabaseName As String Implements iormRelationalDatabaseDriver.NativeDatabaseName
            Get
                If Me.CurrentConnection Is Nothing Then
                    Call CoreMessageHandler(procedure:="adonetrdbdriver.NativeDatabaseName", message:="No current Connection to the Database", _
                                          messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If

                Return CurrentConnection.NativeDatabaseName
            End Get
        End Property

        '' <summary>
        ''' Gets the native database version 
        ''' </summary>
        ''' <value>The type.</value>
        Public Overrides ReadOnly Property NativeDatabaseVersion As String Implements iormRelationalDatabaseDriver.NativeDatabaseVersion
            Get
                If Me.CurrentConnection Is Nothing Then
                    Call CoreMessageHandler(procedure:="adonetrdbdriver.NativeDatabaseName", message:="No current Connection to the Database", _
                                          messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If

                Return CurrentConnection.NativeDatabaseVersion
            End Get
        End Property
        ''' <summary>
        ''' initialize driver
        ''' </summary>
        ''' <param name="Force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overridable Function Initialize(Optional Force As Boolean = False) As Boolean

            If Me.IsInitialized And Not Force Then
                Return True
            End If
            Return False
        End Function
        ''' <summary>
        ''' Start of Bootstrap of the session
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnStartofBootstrap(sender As Object, e As SessionEventArgs) Handles _session.StartOfBootStrapInstallation

        End Sub
        ''' <summary>
        ''' handle the end of bootstrap
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnEndOfBootstrap(sender As Object, e As SessionEventArgs) Handles _session.EndOfBootStrapInstallation
            If Not e.AbortOperation Then
                Initialize(Force:=True) ' reinitialize and save
            Else
                Reset()
            End If

        End Sub
        ''' <summary>
        ''' reset the Driver
        ''' </summary>
        ''' <returns>true if successfull</returns>
        ''' <remarks></remarks>
        Protected Friend Overridable Function Reset() As Boolean
            Try

                _OnTrackDataSet = Nothing
                _ParametersTable = Nothing
                _ParametersTableAdapter = Nothing
                _BootStrapParameterCache.Clear()

                Me.IsInitialized = False
                Return True
            Catch ex As Exception
                Me.IsInitialized = False
                Call CoreMessageHandler(procedure:="adonetDBDriver.reset", message:="couldnot reset database driver", _
                                      exception:=ex)
                Me.IsInitialized = False
                Return True
            End Try
        End Function
        '******
        '****** EventHandler for Connection
        Protected Friend Overridable Sub Connection_onConnection(sender As Object, e As ormConnectionEventArgs) Handles _primaryConnection.OnConnection
            Call Me.Initialize()
        End Sub

        '******
        '****** EventHandler for DisConnection
        Protected Friend Overridable Sub Connection_onDisConnection(sender As Object, e As ormConnectionEventArgs) Handles _primaryConnection.OnDisconnection
            Call Me.Reset()
        End Sub

        ''' <summary>
        ''' returns True if data store has the tablename
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function VerifyTableSchema(tabledefinition As ormContainerDefinition, Optional ByRef connection As iormConnection = Nothing, Optional nativeConnection As Object = Nothing) As Boolean
            Dim result As Boolean = True
            If Not tabledefinition.IsAlive(subname:="adonetDBDriver.hastable") Then Return False
            '** check if we have the table ?!
            If Not Me.HasTable(tableid:=tabledefinition.ID, connection:=connection, nativeConnection:=nativeConnection) Then
                CoreMessageHandler(message:="table schema does not exist in database", containerID:=tabledefinition.ID, _
                                    procedure:="adonetDBDriver.verifytableSchema", messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            '** check each column
            For Each aColumndefinition In tabledefinition.ContainerEntries
                result = result And Me.VerifyColumnSchema(aColumndefinition)
            Next

            If Not result Then
                CoreMessageHandler(message:="table schema in database differs from definition", containerID:=tabledefinition.ID, _
                                    procedure:="adonetDBDriver.verifytableSchema", messagetype:=otCoreMessageType.InternalError)
            End If
            Return result
        End Function
        ''' <summary>
        ''' verify the container schema
        ''' </summary>
        ''' <param name="containerAttribute"></param>
        ''' <param name="connection"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function VerifyContainerSchema(containerAttribute As iormContainerDefinition, Optional ByRef connection As iormConnection = Nothing, Optional nativeConnection As Object = Nothing) As Boolean
            If containerAttribute.GetType Is GetType(ormTableAttribute) Then
                Return VerifyTableSchema(CType(containerAttribute, ormTableAttribute), connection:=connection, nativeConnection:=nativeConnection)
            Else
                CoreMessageHandler(message:="wrong type of container attribute passed to verify", procedure:="adonetDBDriver.verifyContainerSchema", _
                                    messagetype:=otCoreMessageType.InternalError, argument:=containerAttribute.GetType.Name)
                Return False
            End If
        End Function


        ''' <summary>
        ''' returns True if data store has the table described by the table attribute
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function VerifyTableSchema(tableattribute As ormTableAttribute, Optional ByRef connection As iormConnection = Nothing, Optional nativeConnection As Object = Nothing) As Boolean
            Dim result As Boolean = True

            '** check if we have the table ?!
            If Not Me.HasTable(tableid:=tableattribute.TableID, connection:=connection, nativeConnection:=nativeConnection) Then
                CoreMessageHandler(message:="table schema does not exist in database", containerID:=tableattribute.TableID, _
                                    procedure:="adonetDBDriver.verifytableSchema", messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            '** check each column
            For Each aColumn In tableattribute.EntryAttributes
                result = result And Me.VerifyColumnSchema(aColumn)
            Next
            If Not result Then
                CoreMessageHandler(message:="table schema in database differs from table attributes", containerID:=tableattribute.TableID, _
                                    procedure:="adonetDBDriver.verifytableSchema", messagetype:=otCoreMessageType.InternalError)
            End If
            Return True
        End Function
        ''' <summary>
        ''' returns True if data store has the table name
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function HasTable(tableid As String, Optional ByRef connection As iormConnection = Nothing, Optional nativeConnection As Object = Nothing) As Boolean
            Throw New NotImplementedException()
        End Function

        ''' <summary>
        ''' Gets the table.
        ''' </summary>
        ''' <param name="tablename">The tablename.</param>
        ''' <param name="createOnMissing">The create on missing.</param>
        ''' <param name="addToSchemaDir">The add to schema dir.</param>
        ''' <param name="NativeConnection">The native connection.</param>
        ''' <returns></returns>
        Public Overrides Function GetTable(tableid As String, _
                                           Optional createOrAlter As Boolean = False, _
                                           Optional ByRef connection As iormConnection = Nothing, _
                                            Optional ByRef nativeTableObject As Object = Nothing) As Object
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function


        ''' <summary>
        ''' drops a table in the database by id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function DropTable(ByVal id As String, Optional ByRef connection As iormConnection = Nothing) As Boolean Implements iormDatabaseDriver.DropContainerObject
            Dim nativename As String = Me.GetNativeDBObjectName(id)
            '** drop table
            Me.RunSqlStatement("DROP TABLE " & nativename)
            If Not Me.HasTable(id) Then
                Return DropContainerVersion(id)
            End If
            Return False
        End Function


        ''' <summary>
        ''' drops a table in the database by id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function DropView(ByVal id As String, Optional ByRef connection As iormConnection = Nothing) As Boolean Implements iormRelationalDatabaseDriver.DropView
            Dim nativename As String = Me.GetNativeViewname(id)
            '** drop table
            Me.RunSqlStatement("DROP VIEW " & nativename)
            Return Not Me.HasView(id)
        End Function
        ''' <summary>
        ''' gets or creates a native index object out of a indexdefinition
        ''' </summary>
        ''' <param name="nativeTable"></param>
        ''' <param name="indexdefinition"></param>
        ''' <param name="forceCreation"></param>
        ''' <param name="createOrAlter"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Overrides Function GetIndex(ByRef nativeTable As Object, ByRef indexdefinition As ormIndexDefinition, _
          Optional ByVal forceCreation As Boolean = False, _
          Optional ByVal createOrAlter As Boolean = False, _
           Optional ByRef connection As iormConnection = Nothing) As Object Implements iormRelationalDatabaseDriver.GetIndex
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function
        ''' <summary>
        ''' returns True if tablename has the column
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="columnname"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function HasColumn(tablename As String, columnname As String, Optional ByRef connection As iormConnection = Nothing) As Boolean
            Throw New NotImplementedException()
        End Function

        ''' <summary>
        ''' returns True if tablename has the column
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="columnname"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function VerifyColumnSchema(columnattribute As iormContainerEntryDefinition, Optional ByRef connection As iormConnection = Nothing, Optional silent As Boolean = False) As Boolean
            Throw New NotImplementedException()
        End Function
        ''' <summary>
        ''' Gets the column.
        ''' </summary>
        ''' <param name="nativeTABLE">The native TABLE.</param>
        ''' <param name="aDBDesc">A DB desc.</param>
        ''' <param name="createOnMissing">The create on missing.</param>
        ''' <param name="addToSchemaDir">The add to schema dir.</param>
        ''' <returns></returns>
        Public Overrides Function GetColumn(nativeTable As Object, columndefinition As iormContainerEntryDefinition, Optional createOrAlter As Boolean = False, Optional ByRef connection As iormConnection = Nothing) As Object Implements iormRelationalDatabaseDriver.GetColumn
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function

        ''' <summary>
        ''' Gets or creates the foreign key for a columndefinition
        ''' </summary>
        ''' <param name="nativeTable">The native table.</param>
        ''' <param name="columndefinition">The columndefinition.</param>
        ''' <param name="createOrAlter">The create or alter.</param>
        ''' <param name="connection">The connection.</param>
        ''' <returns></returns>
        Public Overrides Function GetForeignKeys(nativeTable As Object, foreignkeydefinition As ormForeignKeyDefinition, Optional createOrAlter As Boolean = False, Optional ByRef connection As iormConnection = Nothing) As IEnumerable(Of Object) Implements iormRelationalDatabaseDriver.GetForeignKeys
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function
        ''' <summary>
        ''' returns the target type for a OTDB FieldType - MAPPING
        ''' </summary>
        ''' <param name="type"></param>
        ''' <remarks></remarks>
        ''' <returns></returns>
        Public Overrides Function GetTargetTypeFor(type As otDataType) As Long Implements iormRelationalDatabaseDriver.GetTargetTypeFor
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function

        ''' <summary>
        ''' Gets the DB parameter.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function CreateDBParameterTable(Optional ByRef nativeConnection As Object = Nothing) As Boolean Implements iormRelationalDatabaseDriver.CreateDBParameterContainer
            Dim anativeConnection As IDbConnection
            Dim aTablename As String = ConstDBParameterTableName 'this table has no prefix

            '*** get the native Connection 
            If nativeConnection Is Nothing Then
                If Not Me.CurrentConnection Is Nothing Then
                    anativeConnection = DirectCast(Me.CurrentConnection, adonetConnection).NativeInternalConnection
                    If anativeConnection Is Nothing Then
                        Call CoreMessageHandler(procedure:="adonetDBDriver.CreateDBParameterTable", message:="Native internal Connection not available")
                        Return False
                    End If
                Else
                    Call CoreMessageHandler(procedure:="adonetDBDriver.CreateDBParameterTable", message:="Connection not available")
                    Return False
                End If
            Else
                anativeConnection = nativeConnection
            End If

            '*** create
            If Not Me.HasTable(aTablename) Then
                Me.RunSqlStatement(String.Format("CREATE TABLE {0} " & _
                                  "( [{1}] nvarchar(255) not null,[{2}] nvarchar(255) not null, [{3}] nvarchar(255) null, [{4}] datetime  null,	[{5}] nvarchar(255) null " & _
                                  " CONSTRAINT [{0}_primaryKey] PRIMARY KEY ([{1}], [{2}] ))", _
                                  aTablename, ConstFNSetupID, ConstFNID, ConstFNValue, ConstFNChangedOn, constFNDescription), _
                                  nativeConnection:=nativeConnection)
                'Me.RunSQLCommand("create unique index primaryKey on " & ConstParameterTableName & "(ID);", nativeConnection:=nativeConnection)
            End If

            ' reinitialize
            Me.Initialize(Force:=True)

            Return True
        End Function

        ''' <summary>
        ''' drops the DB parameter table - given with setup then just the setup related entries
        ''' if then there is no setup related entries at all -> drop the full table
        ''' </summary>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function DropDBParameterTable(Optional setupid As String = Nothing, Optional ByRef nativeConnection As Object = Nothing) As Boolean Implements iormRelationalDatabaseDriver.DropDBParameterContainer
            Dim anativeConnection As IDbConnection
            Dim aTablename As String = ConstDBParameterTableName 'this table has no prefix


            '*** get the native Connection 
            If nativeConnection Is Nothing Then
                If Not Me.CurrentConnection Is Nothing Then
                    anativeConnection = DirectCast(Me.CurrentConnection, adonetConnection).NativeInternalConnection
                    If anativeConnection Is Nothing Then
                        Call CoreMessageHandler(procedure:="adonetDBDriver.DropDBParameterTable", message:="Native internal Connection not available")
                        Return False
                    End If
                Else
                    Call CoreMessageHandler(procedure:="adonetDBDriver.DropDBParameterTable", message:="Connection not available")
                    Return False
                End If
            Else
                anativeConnection = nativeConnection
            End If

            '*** create
            If Not Me.HasTable(aTablename) Then
                Call CoreMessageHandler(procedure:="adonetDBDriver.DropDBParameterTable", message:="table does not exist")
                Return False
            End If

            ''' if setup is not nothing then delete just the rows with it
            ''' 
            Dim droptable As Boolean = True 'drop the table
            Dim aDataReader As IDataReader

            If setupid IsNot Nothing Then
                If _ParametersTable IsNot Nothing Then
                    Dim dataRows = _ParametersTable.AsEnumerable.Where(Function(x) x.Field(Of String)(columnName:=ConstFNSetupID).Trim = setupid)

                    ''' delete the db Parameter
                    For Each aRow In dataRows.ToList
                        Me.DeleteDBParameter(parametername:=aRow.Item(ConstFNID), setupID:=setupid)
                    Next
                    ''' anything left ?!
                    ''' 
                    If _ParametersTable.Rows.Count > 0 Then
                        Return True
                    Else
                        droptable = True
                    End If
                Else
                    Try
                        Dim aDeleteStr As String = String.Format("DELETE FROM [{0}] WHERE [{1}]='{2}'", DBParameterTablename, ConstFNSetupID, setupid)
                        Me.RunSqlStatement(aDeleteStr, nativeConnection:=nativeConnection)
                        Dim aSelectStr As String = String.Format("SELECT [{0}] FROM [{1}]", ConstFNID, DBParameterTablename)
                        Dim aCommand As IDbCommand = Me.CreateNativeDBCommand(aSelectStr, anativeConnection)
                        aDataReader = aCommand.ExecuteReader
                        If aDataReader.Read Then
                            Return True
                        Else
                            droptable = True
                        End If

                        aDataReader.Close()
                    Catch ex As Exception
                        If aDataReader IsNot Nothing Then aDataReader.Close()
                        CoreMessageHandler(exception:=ex, procedure:="adonetDBDriver.DropParameterTable")
                        Return False
                    End Try

                End If

            End If

            ''' drop table
            If droptable Then
                CurrentOTDBDriver.DropContainerObject(id:=DBParameterTablename)
                If Not CurrentOTDBDriver.HasContainerID(DBParameterTablename) Then
                    CoreMessageHandler(message:="OTDB PARAMETER TABLE  " & CurrentOTDBDriver.DBParameterContainerName & " DROPPED", containerID:=CurrentOTDBDriver.DBParameterContainerName, _
                                       messagetype:=otCoreMessageType.ApplicationInfo, procedure:="Installation.DropDatabase")
                End If
            End If

            ' reinitialize
            Me.Initialize(Force:=True)

            Return True
        End Function

        ''' <summary>
        ''' Gets the DB parameter.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function CreateDBUserDefTable(Optional ByRef nativeConnection As Object = Nothing) As Boolean _
            Implements iormRelationalDatabaseDriver.CreateDBUserDefTable
            Dim anativeConnection As IDbConnection

            Try
                '*** get the native Connection 
                If nativeConnection Is Nothing Then
                    If Not Me.CurrentConnection Is Nothing Then
                        anativeConnection = DirectCast(Me.CurrentConnection, adonetConnection).NativeInternalConnection
                        If anativeConnection Is Nothing Then
                            Call CoreMessageHandler(procedure:="adonetDBDriver.CreateDBUserDefTable", message:="Native internal Connection not available")
                            Return Nothing
                        End If
                    Else
                        Call CoreMessageHandler(procedure:="adonetDBDriver.CreateDBUserDefTable", message:="Connection not available")
                        Return Nothing
                    End If
                Else
                    anativeConnection = nativeConnection
                End If

                '*** create
                If Not Me.HasTable(User.ConstPrimaryTableID) Then
                    Me.RunSqlStatement(User.GetCreateSqlString, nativeConnection:=nativeConnection)
                End If
                Dim anInsertStr As String = User.GetInsertInitalUserSQLString(username:="admin", password:="axs2ontrack", desc:="Administrator", _
                                                                              group:="admins", defaultworkspace:=String.Empty, person:=String.Empty)
                Me.RunSqlStatement(anInsertStr, nativeConnection:=nativeConnection)

                With New UI.CoreMessageBox
                    .type = UI.CoreMessageBox.MessageType.Info
                    .Message = "An administrator user 'Admin' with password 'axs2ontrack' was created. Please change the password as soon as possible"
                    .buttons = UI.CoreMessageBox.ButtonType.OK
                    .Show()
                End With
                Call CoreMessageHandler(message:="An administrator user 'Admin' with password 'axs2ontrack' was created. Please change the password as soon as possible", _
                                        procedure:="adonetDBDriver.CreateDBUserDefTable", messagetype:=otCoreMessageType.InternalInfo)
                Return True


            Catch ex As SqlException
                Call CoreMessageHandler(exception:=ex, procedure:="adonetDBDriver.CreateDBUserDefTable", messagetype:=otCoreMessageType.InternalException)
                Return False
            Catch ex As OleDb.OleDbException
                Call CoreMessageHandler(exception:=ex, procedure:="adonetDBDriver.CreateDBUserDefTable", messagetype:=otCoreMessageType.InternalException)
                Return False
            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="adonetDBDriver.CreateDBUserDefTable", messagetype:=otCoreMessageType.InternalException)
                Return False
            End Try

            Return True
        End Function

        ''' <summary>
        ''' Install the schema of Ontrack Database
        ''' </summary>
        ''' <param name="askBefore"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function InstallOnTrackDatabase(askBefore As Boolean, modules As String()) As Boolean Implements iormRelationalDatabaseDriver.InstallOnTrackDatabase
            Dim result As OnTrack.UI.CoreMessageBox.ResultType

            '** check
            If _isInstalling Then Return False



            '** ask
            If askBefore Then
                With New CoreMessageBox
                    .Title = "IMPORTANT QUESTION"
                    .Message = "The OnTrack database detected missing installation data for database setup >" & CurrentSetupID & "< using configuration > " & CurrentConfigSetName & "<." & vbLf & _
                        "Should the database schema be (re) created ? This means that all data might be lost ..." & vbLf & _
                        "If this is a repair or upgrade of the schema - an Administrator Account might be necessary for this operation."
                    .buttons = CoreMessageBox.ButtonType.YesNo
                    .Show()
                    result = .result
                End With
            Else
                result = CoreMessageBox.ResultType.Yes
            End If

            '** check rights
            If Me.CurrentConnection.IsConnected OrElse Me.HasAdminUserValidation() Then
                If Not Me.CurrentConnection.VerifyUserAccess(accessRequest:=otAccessRight.AlterSchema, useLoginWindow:=True, messagetext:="Please enter an administrator account to continue schema upgrade") Then
                    CoreMessageHandler(message:="User access for alter or repair the database schema could NOT be granted - installation aborted", messagetype:=otCoreMessageType.InternalInfo, _
                                        procedure:="adonetDBDriver.InstallOnTrackDatabase")
                    Return False
                End If
            End If

            '*** create
            '***
            If result = CoreMessageBox.ResultType.Yes Then
                _isInstalling = True
                '** send message to the session
                RaiseEvent RequestBootstrapInstall(Me, New SessionBootstrapEventArgs(install:=False, modules:=modules, AskBefore:=False))
                '***
                '*** create the database
                Call Installation.CreateDatabase(modules) ' startups also a session and login
                '** sets the total schema version parameter
                _isInstalling = False
                Return True
            End If

        End Function

        ''' <summary>
        ''' Checks if the most important objects are here
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function VerifyOnTrackDatabase(Optional modules As String() = Nothing,
                                                        Optional install As Boolean = False,
                                                        Optional verifySchema As Boolean = False) As Boolean Implements iormRelationalDatabaseDriver.VerifyOnTrackDatabase
            Dim result As Boolean = True
            Dim hasParameterTable As Boolean = False
            Dim aValue As String = Nothing
            Dim aVersion As Long?

            If Not Me.HasTable(tableid:=_parametersTableName, connection:=Me.CurrentConnection) Then
                result = result And False
                CoreMessageHandler(message:="Database table " & _parametersTableName & " missing in database ", noOtdbAvailable:=True, _
                                   messagetype:=otCoreMessageType.InternalError, argument:=Me._primaryConnection.Connectionstring, containerID:=_parametersTableName)
            Else
                'CoreMessageHandler(message:="Database table " & _parametersTableName & " exists in database ", noOtdbAvailable:=True, _
                '                    messagetype:=otCoreMessageType.InternalInfo, arg1:=Me._primaryConnection.Connectionstring, tablename:=_parametersTableName)
                result = result And True
                hasParameterTable = True
            End If

            '** check overall schema version
            If hasParameterTable Then
                aValue = GetDBParameter(ConstPNBSchemaVersion, silent:=True)
                If aValue Is Nothing OrElse Not IsNumeric(aValue) Then
                    result = result And False
                ElseIf ot.SchemaVersion < Convert.ToUInt64(aValue) Then
                    CoreMessageHandler(showmsgbox:=True, message:="Verifying the OnTrack Database failed. The Tooling schema version of # " & ot.SchemaVersion & _
                                       " is less than the database schema version of #" & aValue & " - Session could not start up", _
                                       messagetype:=otCoreMessageType.InternalError, procedure:="Session.Startup")
                    Return False
                ElseIf ot.SchemaVersion > Convert.ToUInt64(aValue) Then
                    result = result And False
                End If
            End If

            '** BOOTSTRAP TABLE CHECKING
            '**
            For Each anObjectClassDescription In ot.GetBootStrapObjectClassDescriptions
                For Each aTablename In anObjectClassDescription.Tablenames

                    If Not Me.HasTable(aTablename) Then
                        CoreMessageHandler(message:="Database table " & aTablename & " missing in database ", noOtdbAvailable:=True, _
                                           messagetype:=otCoreMessageType.InternalError, argument:=Me._primaryConnection.Connectionstring, containerID:=aTablename)
                        result = result And False
                    Else

                        If hasParameterTable Then aVersion = ContainerVersion(aTablename.ToUpper)
                        If aVersion Is Nothing Then
                            CoreMessageHandler(message:="Database table " & aTablename & " has no version in database parameters - schema will be recreated", noOtdbAvailable:=True, _
                                      messagetype:=otCoreMessageType.InternalError, containerID:=aTablename)

                            result = result And False
                        Else
                            Dim anAttribute = ot.GetSchemaTableAttribute(aTablename)
                            If anAttribute.Version <> aVersion Then
                                CoreMessageHandler(message:="Database table " & aTablename & " has different version in database parameters ( " & aVersion & " ) than in SchemaAttribute", noOtdbAvailable:=True, _
                                      messagetype:=otCoreMessageType.InternalError, argument:=anAttribute.Version, containerID:=aTablename)

                                result = result And False
                            Else
                                result = result And True
                            End If
                        End If

                        '*** check additionally the schema
                        If verifySchema Then
                            '** build an ObjectDefinition out of the attributes
                            Dim anTableAttribute = ot.GetSchemaTableAttribute(aTablename)
                            If anTableAttribute IsNot Nothing Then
                                result = result And Me.VerifyTableSchema(anTableAttribute)
                            End If
                            '** check on the table definition
                            If Not result Then
                                CoreMessageHandler(message:="Database table " & aTablename & " exists in database but has different same schema", noOtdbAvailable:=True, _
                                             messagetype:=otCoreMessageType.InternalInfo, argument:=Me._primaryConnection.Connectionstring, containerID:=aTablename)
                            End If
                        End If

                    End If
                Next

            Next

            '**** Check the modules
            '****
            If result AndAlso modules IsNot Nothing Then
                For Each modulename In modules
                    '**Module Checking
                    '**
                    For Each anObjectClassDescription In GetObjectClassDescriptionsForModule(modulename)
                        For Each aTablename In anObjectClassDescription.Tablenames

                            If Not Me.HasTable(aTablename) Then
                                CoreMessageHandler(message:="Database table " & aTablename & " missing in database module " & modulename, noOtdbAvailable:=True, _
                                                   messagetype:=otCoreMessageType.InternalError, argument:=Me._primaryConnection.Connectionstring, containerID:=aTablename)
                                result = result And False
                            Else

                                If hasParameterTable Then aVersion = Me.ContainerVersion(aTablename.ToUpper)
                                If aVersion Is Nothing Then
                                    CoreMessageHandler(message:="Database table " & aTablename & " for module " & modulename & " has no version in database parameters", noOtdbAvailable:=True, _
                                              messagetype:=otCoreMessageType.InternalError, containerID:=aTablename)

                                    result = result And False
                                Else
                                    Dim anAttribute = ot.GetSchemaTableAttribute(aTablename)
                                    If anAttribute.Version <> aVersion Then
                                        CoreMessageHandler(message:="Database table " & aTablename & " for module " & modulename & " has different version in database parameters ( " & aValue & " ) than in SchemaAttribute", noOtdbAvailable:=True, _
                                              messagetype:=otCoreMessageType.InternalError, argument:=anAttribute.Version, containerID:=aTablename)

                                        result = result And False
                                    Else
                                        result = result And True
                                    End If
                                End If

                                '*** check additionally the schema
                                If verifySchema Then
                                    '** build an ObjectDefinition out of the attributes
                                    Dim anTableAttribute = ot.GetSchemaTableAttribute(aTablename)
                                    If anTableAttribute IsNot Nothing Then
                                        result = result And Me.VerifyTableSchema(anTableAttribute)
                                    End If
                                    '** check on the table definition
                                    If Not result Then
                                        CoreMessageHandler(message:="Database table " & aTablename & " for module " & modulename & " exists in database but has different same schema", noOtdbAvailable:=True, _
                                                     messagetype:=otCoreMessageType.InternalInfo, argument:=Me._primaryConnection.Connectionstring, containerID:=aTablename)
                                    End If
                                End If

                            End If
                        Next

                    Next

                Next
            End If

            '*** Raise request to bootstrap install
            Dim args = New SessionBootstrapEventArgs(install:=install, modules:=modules, AskBefore:=True)
            If Not result And install Then RaiseRequestBootstrapInstall(Me, args)
            If install Then result = args.InstallationResult
            Return result
        End Function
        ''' <summary>
        '''  raise the RequestBootStrapInstall Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Protected Overridable Sub RaiseRequestBootstrapInstall(sender As Object, ByRef e As EventArgs)
            RaiseEvent RequestBootstrapInstall(sender, e)

        End Sub

        ''' <summary>
        ''' creates the entry for the global domain in bootstrapping
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function CreateGlobalDomain(Optional ByRef nativeConnection As Object = Nothing) As Boolean Implements iormRelationalDatabaseDriver.CreateGlobalDomain
            Dim anativeConnection As IDbConnection

            Try
                '*** get the native Connection 
                If nativeConnection Is Nothing Then
                    If Not Me.CurrentConnection Is Nothing Then
                        anativeConnection = DirectCast(Me.CurrentConnection, adonetConnection).NativeInternalConnection
                        If anativeConnection Is Nothing Then
                            Call CoreMessageHandler(procedure:="adonetDBDriver.CreateGlobalDomain", message:="Native internal Connection not available", messagetype:=otCoreMessageType.InternalError)
                            Return False
                        End If
                    Else
                        Call CoreMessageHandler(procedure:="adonetDBDriver.CreateGlobalDomain", message:="Connection not available", messagetype:=otCoreMessageType.InternalError)
                        Return False
                    End If
                Else
                    anativeConnection = nativeConnection
                End If

                '*** check
                If Me.HasTable(Domain.ConstPrimaryTableID) Then
                    Dim cmdstr As String

                    cmdstr = "SELECT {0} FROM [{1}] WHERE {0} = '{2}' "
                    cmdstr = String.Format(cmdstr, Domain.ConstFNDomainID, Me.GetNativeDBObjectName(Domain.ConstPrimaryTableID), ConstGlobalDomain)

                    Dim aCommand As IDbCommand = Me.CreateNativeDBCommand(cmdstr, anativeConnection)
                    Dim aDataReader As IDataReader = aCommand.ExecuteReader

                    If aDataReader.Read Then
                        aDataReader.Close()
                        Return True
                    Else
                        aDataReader.Close()
                        cmdstr = Domain.GetInsertGlobalDomainSQLString(domainid:=ConstGlobalDomain, description:="global domain", mindeliverableuid:=0, maxdeliverableuid:=100000)
                        Dim result = RunSqlStatement(sqlcmdstr:=cmdstr, nativeConnection:=anativeConnection)
                        Return result
                    End If
                Else
                    Call CoreMessageHandler(procedure:="adonetDBDriver.CreateGlobalDomain", message:="table for domain object doesnot exist", _
                                            containerID:=Domain.ConstPrimaryTableID, objectname:=Domain.ConstObjectID, messagetype:=otCoreMessageType.InternalError)

                    Return False
                End If

            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="adonetDBDriver.CreateGlobalDomain", messagetype:=otCoreMessageType.InternalException)
                Return False
            End Try
        End Function
        ''' <summary>
        ''' returns true if there is a Admin User in the user definition of this database
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function HasAdminUserValidation(Optional ByRef nativeConnection As Object = Nothing) As Boolean Implements iormRelationalDatabaseDriver.HasAdminUserValidation
            Dim anativeConnection As IDbConnection

            Try
                '*** get the native Connection 
                If nativeConnection Is Nothing Then
                    If Not Me.CurrentConnection Is Nothing Then
                        anativeConnection = DirectCast(Me.CurrentConnection, adonetConnection).NativeInternalConnection
                        If anativeConnection Is Nothing Then
                            Call CoreMessageHandler(procedure:="adonetDBDriver.HasAdminUserValidation", message:="Native internal Connection not available", messagetype:=otCoreMessageType.InternalError)
                            Return False
                        End If
                    Else
                        Call CoreMessageHandler(procedure:="adonetDBDriver.HasAdminUserValidation", message:="Connection not available", messagetype:=otCoreMessageType.InternalError)
                        Return False
                    End If
                Else
                    anativeConnection = nativeConnection
                End If

                '*** check
                If Me.HasTable(User.ConstPrimaryTableID) Then
                    Dim cmdstr As String
                    If Me.Name = ConstCPVDriverSQLServer Then
                        cmdstr = "SELECT {0}, {1}, {2}, {3} , {4}, {5}, {6} FROM [{7}] WHERE {2} = 1 "
                    Else
                        cmdstr = "SELECT {0}, {1}, {2}, {3} , {4}, {5}, {6} FROM [{7}] WHERE {2} <> 0 "
                        'Else
                        '    CoreMessageHandler(message:="unknown database driver type - implementation missing", procedure:="adonetDBDriver.HasAdminUserValidation", messagetype:=otCoreMessageType.InternalError)
                        '    Return False
                    End If
                    cmdstr = String.Format(cmdstr, User.ConstFNPassword, User.ConstFNUsername, User.ConstFNAlterSchema, User.ConstFNIsAnonymous, User.ConstFNReadData, User.ConstFNUpdateData, User.ConstFNNoAccess, _
                                                         GetNativeDBObjectName(User.ConstPrimaryTableID))

                    Dim aCommand As IDbCommand = Me.CreateNativeDBCommand(cmdstr, anativeConnection)
                    Dim aDataReader As IDataReader = aCommand.ExecuteReader


                    If aDataReader.Read Then
                        aDataReader.Close()
                        Return True
                    End If

                    aDataReader.Close()
                    Return False
                Else
                    Return False
                End If

            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="adonetDBDriver.HasAdminUserValidation", messagetype:=otCoreMessageType.InternalException)
                Return False
            End Try
        End Function
        ''' <summary>
        ''' Gets the def user validation structure from the database.
        ''' </summary>
        ''' <param name="Username">The username.</param>
        ''' <param name="SelectAnonymous"></param>
        ''' <param name="nativeConnection">The native connection.</param>
        ''' <returns></returns>
        Protected Friend Overrides Function GetUserValidation(username As String, _
                                                              Optional selectAnonymous As Boolean = False, _
                                                    Optional ByRef nativeConnection As Object = Nothing) As UserValidation
            Dim anUser As New User
            Dim aCollection As New Collection
            Dim aNativeConnection As IDbConnection
            Dim cmdstr As String


            '*** get the native Connection 
            If nativeConnection Is Nothing Then
                If _primaryConnection IsNot Nothing Then
                    aNativeConnection = DirectCast(_primaryConnection, adonetConnection).NativeInternalConnection
                    If aNativeConnection Is Nothing Then
                        Call CoreMessageHandler(procedure:="adonetDBDriver.getUserValidation", message:="Native internal Connection not available")
                        Return Nothing
                    End If
                Else
                    Call CoreMessageHandler(procedure:="adonetDBDriver.getUserValidation", message:="Connection not available")
                    Return Nothing
                End If
            Else
                aNativeConnection = nativeConnection
            End If


            Try
                '** init driver
                If Not Me.IsInitialized Then
                    Me.Initialize()
                End If

                '** on multiple enquiries
                If _currentUserValidation.ValidEntry AndAlso username = _currentUserValidation.Username Then
                    Return _currentUserValidation
                End If

                If Not Me.HasTable(User.ConstPrimaryTableID) Then
                    If Not Me.VerifyOnTrackDatabase(install:=False) Then
                        Call CoreMessageHandler(procedure:="adonetDBDriver.getUserValidation", message:="Database is not installed - Setup of schema failed")
                        Return Nothing
                    End If
                End If

                '** if no anonymous -> check valid username
                If Not selectAnonymous AndAlso String.IsNullOrWhiteSpace(username) Then
                    Call CoreMessageHandler(procedure:="adonetDBDriver.getUserValidation", message:="for none-anonymous authentication provide a valid non-whitechar username")
                    Return Nothing
                End If

                '** select the validation
                If Not selectAnonymous Then
                    cmdstr = "select * from [" & GetNativeDBObjectName(User.ConstPrimaryTableID) & "] where " & User.ConstFNUsername & " ='" & username & "'"
                Else
                    If Me.Name = ConstCPVDriverSQLServer Then
                        cmdstr = "select * from [" & GetNativeDBObjectName(User.ConstPrimaryTableID) & "] where  " & User.ConstFNIsAnonymous & " <>0 order by " & User.ConstFNUsername & " desc"
                    Else
                        cmdstr = "select * from [" & GetNativeDBObjectName(User.ConstPrimaryTableID) & "] where  " & User.ConstFNIsAnonymous & " <> false order by " & User.ConstFNUsername & " desc"
                        'Else
                        '    Call CoreMessageHandler(message:="DriverType is not implemented", procedure:="adonetDBDriver.GetUserValidation", messagetype:=otCoreMessageType.InternalError)
                        '    Return Nothing
                    End If
                End If


                Dim aCommand As IDbCommand = Me.CreateNativeDBCommand(cmdstr, aNativeConnection)
                Dim aDataReader As IDataReader = aCommand.ExecuteReader

                If aDataReader.Read Then
                    Try
                        Dim aValue As Object
                        _currentUserValidation.Password = Nothing
                        aValue = aDataReader(User.ConstFNPassword)
                        If Not IsDBNull(aValue) Then _currentUserValidation.Password = CStr(aValue)

                        _currentUserValidation.Username = Nothing
                        aValue = aDataReader(User.ConstFNUsername)
                        If Not IsDBNull(aValue) Then _currentUserValidation.Username = CStr(aValue)

                        _currentUserValidation.IsAnonymous = aDataReader(User.ConstFNIsAnonymous)
                        _currentUserValidation.HasAlterSchemaRights = aDataReader(User.ConstFNAlterSchema)
                        _currentUserValidation.HasReadRights = aDataReader(User.ConstFNReadData)
                        _currentUserValidation.HasUpdateRights = aDataReader(User.ConstFNUpdateData)
                        _currentUserValidation.HasNoRights = aDataReader(User.ConstFNNoAccess)
                        _currentUserValidation.ValidEntry = True

                    Catch ex As Exception
                        Call CoreMessageHandler(exception:=ex, procedure:="adonetDBDriver.getUserValidation", message:="Couldn't read User Validation", _
                                              break:=False, noOtdbAvailable:=True)
                        _currentUserValidation.ValidEntry = False
                        aDataReader.Close()
                        Return _currentUserValidation

                    End Try

                    ' return successfull
                    aDataReader.Close()
                    Return _currentUserValidation

                End If

                aDataReader.Close()
                ' return
                _currentUserValidation.ValidEntry = False
                Return _currentUserValidation

            Catch ex As OleDbException
                Call CoreMessageHandler(showmsgbox:=True, message:="OLEDB Database not available", procedure:="adonetDBDriver.getUserValidation", exception:=ex)

                Return Nothing

            Catch ex As SqlException
                Call CoreMessageHandler(showmsgbox:=True, message:="SQL Server Database not available", procedure:="adonetDBDriver.getUserValidation", exception:=ex)

                Return Nothing


                ' Handle the error
            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=True, procedure:="adonetDBDriver.getUserValidation", exception:=ex)

                Return Nothing

            End Try

        End Function

        ''' <summary>
        ''' run Sql Select Command by ID
        ''' </summary>
        ''' <param name="id">the ID of the stored SQLCommand</param>
        ''' <param name="parameters">optional a list of parameters for the values</param>
        ''' <param name="nativeConnection">optional a nativeConnection</param>
        ''' <returns>a list of clsotdbRecords</returns>
        ''' <remarks></remarks>
        Public Overrides Function RunSqlSelectCommand(id As String, _
                                                       Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing, _
                                                      Optional nativeConnection As Object = Nothing) As List(Of ormRecord) _
                                                    Implements iormRelationalDatabaseDriver.RunSqlSelectCommand
            Try
                Dim aSqlCommand As iormSqlCommand


                '*** bookkeeping on commands
                If Me.HasSqlCommand(id) Then
                    aSqlCommand = Me.RetrieveSqlCommand(id)
                    Return Me.RunSqlSelectCommand(sqlcommand:=aSqlCommand, parametervalues:=parametervalues, nativeConnection:=nativeConnection)
                Else
                    Call CoreMessageHandler(message:="SQL command with this ID is not in store", procedure:="adonetDBDriver.RunSqlSelectCommand", _
                                          messagetype:=otCoreMessageType.InternalError, argument:=id)
                    Return New List(Of ormRecord)
                End If
            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, message:="Exception", procedure:="adonetDBDriver.RunSqlSelectCommand", _
                                          messagetype:=otCoreMessageType.InternalError, argument:=id)
                Return New List(Of ormRecord)
            End Try
        End Function
        ''' <summary>
        ''' runs a Sql Select Command and returns a List of Records
        ''' </summary>
        ''' <param name="sqlcommand">a clsOTDBSqlSelectCommand</param>
        ''' <param name="parameters"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function RunSqlSelectCommand(ByRef sqlcommand As ormSqlSelectCommand, _
                                           Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing, _
                                           Optional nativeConnection As Object = Nothing) As List(Of ormRecord) _
                                       Implements iormRelationalDatabaseDriver.RunSqlSelectCommand


            Dim cvtvalue As Object
            '*** Execute and get Results
            Dim aDataReader As IDataReader
            Dim theResults As New List(Of ormRecord)
            Dim atableid As String = Nothing

            Try
                '****
                '**** CHECK HERE IF WE CAN TAKE A CACHED DATATABLE FOR THE SQL SELECT
                '****
                If sqlcommand.TableIDs.Count = 1 Then
                    atableid = sqlcommand.TableIDs.First
                    Dim aTablestore = Me.GetTableStore(sqlcommand.TableIDs.First)
                    If aTablestore.HasProperty(ormTableStore.ConstTPNCacheProperty) And sqlcommand.AllFieldsAdded Then
                        '*** BRANCH OUT
                        Return RunSqlSelectCommandCached(sqlcommand:=sqlcommand, parametervalues:=parametervalues, nativeConnection:=nativeConnection)
                    End If
                End If

                '**** NORMAL PROCEDURE RUNS AGAINST DATABASE
                '****
                If Not sqlcommand.IsPrepared Then
                    If Not sqlcommand.Prepare Then
                        Call CoreMessageHandler(message:="SqlCommand couldn't be prepared", argument:=sqlcommand.ID, _
                                               procedure:="adonetDBDriver.runsqlselectCommand", messagetype:=otCoreMessageType.InternalError)
                        Return New List(Of ormRecord)
                    End If
                End If

                Dim aNativeCommand As IDbCommand
                aNativeCommand = sqlcommand.NativeCommand

                '***  Assign the values
                '** initial values
                For Each aParameter In sqlcommand.Parameters
                    If Not aParameter.NotColumn AndAlso Not String.IsNullOrEmpty(aParameter.ColumnName) AndAlso Not String.IsNullOrEmpty(aParameter.TableID) Then
                        Dim aTablestore As iormRelationalTableStore = Me.GetTableStore(aParameter.TableID)
                        If aTablestore.Convert2ContainerData(aParameter.ColumnName, invalue:=aParameter.Value, outvalue:=cvtvalue) Then
                            aNativeCommand.Parameters(aParameter.ID).value = cvtvalue
                        Else
                            CoreMessageHandler(message:=" parameter value could not be converted", argument:=aParameter.Value, containerEntryName:=aParameter.ColumnName, containerID:=aParameter.TableID, _
                                                procedure:="adonetdbdriver.RunSqlSelectCommand", messagetype:=otCoreMessageType.InternalError)
                        End If
                    Else
                        If Convert2DBData(invalue:=aParameter.Value, outvalue:=cvtvalue, targetType:=GetTargetTypeFor(aParameter.Datatype)) Then
                            aNativeCommand.Parameters(aParameter.ID).value = cvtvalue
                        Else
                            CoreMessageHandler(message:=" parameter value could not be converted", argument:=aParameter.Value, _
                                                procedure:="adonetdbdriver.RunSqlSelectCommand", messagetype:=otCoreMessageType.InternalError)
                        End If
                    End If

                Next
                '** Input Parameters 
                If Not parametervalues Is Nothing Then
                    ' overwrite the initial values
                    For Each kvp As KeyValuePair(Of String, Object) In parametervalues
                        If aNativeCommand.Parameters.Contains(kvp.Key) Then
                            Dim aParameter = sqlcommand.Parameters.Find(Function(x) x.ID = kvp.Key)

                            If Not aParameter.NotColumn AndAlso Not String.IsNullOrEmpty(aParameter.ColumnName) AndAlso Not String.IsNullOrEmpty(aParameter.TableID) Then
                                Dim aTablestore As iormRelationalTableStore = Me.GetTableStore(aParameter.TableID)
                                If aTablestore.Convert2ContainerData(aParameter.ColumnName, invalue:=kvp.Value, outvalue:=cvtvalue) Then
                                    aNativeCommand.Parameters(aParameter.ID).value = cvtvalue
                                Else
                                    CoreMessageHandler(message:=" parameter value could not be converted", argument:=kvp.Value, containerEntryName:=aParameter.ColumnName, containerID:=aParameter.TableID, _
                                                        procedure:="adonetdbdriver.RunSqlSelectCommand", messagetype:=otCoreMessageType.InternalError)
                                End If
                            Else
                                If Convert2DBData(invalue:=kvp.Value, outvalue:=cvtvalue, targetType:=GetTargetTypeFor(aParameter.Datatype)) Then
                                    aNativeCommand.Parameters(aParameter.ID).value = cvtvalue
                                Else
                                    CoreMessageHandler(message:=" parameter value could not be converted", argument:=kvp.Value, _
                                                        procedure:="adonetdbdriver.RunSqlSelectCommand", messagetype:=otCoreMessageType.InternalError)
                                End If

                            End If

                        End If
                    Next
                End If

                '*** check if we have only on table -> to infuse this is necessary
                If sqlcommand.TableIDs.Count = 1 Then
                    atableid = sqlcommand.TableIDs(0)
                End If

                aDataReader = aNativeCommand.ExecuteReader
                Dim aRecord As ormRecord
                While aDataReader.Read
                    If sqlcommand.TableIDs.Count = 1 And sqlcommand.AllFieldsAdded Then
                        aRecord = New ormRecord(containerID:=sqlcommand.TableIDs.First, dbdriver:=Me)
                    Else
                        aRecord = New ormRecord() 'free flow record
                    End If
                    If aRecord.LoadFrom(aDataReader) Then
                        theResults.Add(aRecord)
                    End If

                End While

                aDataReader.Close()
                Return theResults

            Catch ex As OleDb.OleDbException
                Call CoreMessageHandler(exception:=ex, procedure:="adonetDBDriver.runSqlSelectCommand", argument:=sqlcommand.SqlText, messagetype:=otCoreMessageType.InternalException)
                If Not aDataReader Is Nothing Then aDataReader.Close()
                Return New List(Of ormRecord)
            Catch ex As SqlException
                Call CoreMessageHandler(exception:=ex, procedure:="adonetDBDriver.runSqlSelectCommand", argument:=sqlcommand.SqlText, messagetype:=otCoreMessageType.InternalException)
                If Not aDataReader Is Nothing Then aDataReader.Close()
                Return New List(Of ormRecord)
            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="adonetDBDriver.runSqlSelectCommand", argument:=sqlcommand.SqlText, messagetype:=otCoreMessageType.InternalException)
                If Not aDataReader Is Nothing Then aDataReader.Close()
                Return New List(Of ormRecord)
            End Try


        End Function

        ''' <summary>
        ''' runs a Sql  Command with parameters
        ''' </summary>
        ''' <param name="sqlcommand">a clsOTDBSqlSelectCommand</param>
        ''' <param name="parameters"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function RunSqlCommand(ByRef sqlcommand As ormSqlCommand, _
                                           Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing, _
                                           Optional nativeConnection As Object = Nothing) As Boolean _
                                            Implements iormRelationalDatabaseDriver.RunSqlCommand


            Dim cvtvalue As Object
            '*** Execute and get Results
            Dim aDataReader As IDataReader
            Dim theResults As New List(Of ormRecord)
            Dim atableid As String = Nothing

            Try


                '**** NORMAL PROCEDURE RUNS AGAINST DATABASE
                '****
                If Not sqlcommand.IsPrepared Then
                    If Not sqlcommand.Prepare Then
                        Call CoreMessageHandler(message:="SqlCommand couldn't be prepared", argument:=sqlcommand.ID, _
                                               procedure:="adonetDBDriver.runsqlCommand", messagetype:=otCoreMessageType.InternalError)
                        Return False
                    End If
                End If

                '****
                '**** CHECK HERE IF WE CAN TAKE A CACHED DATATABLE FOR THE SQL SELECT
                '****
                If sqlcommand.TableIDs.Count = 1 Then
                    atableid = sqlcommand.TableIDs.First
                    Dim aTablestore = Me.GetTableStore(sqlcommand.TableIDs.First)
                    If aTablestore.HasProperty(ormTableStore.ConstTPNCacheProperty) Then
                        '*** BRANCH OUT BUT NOT RETURN !
                        '' RunSqlCommandCached(sqlcommand:=sqlcommand, parametervalues:=parametervalues, nativeConnection:=nativeConnection)
                        ' DATATABLE Doesnot accept general SQL Statements
                        ' this means that we have to recache at the end
                        'Debug.WriteLine("recache it")
                    End If
                End If

                Dim aNativeCommand As IDbCommand
                aNativeCommand = sqlcommand.NativeCommand

                '***  Assign the values
                '** initial values
                For Each aParameter In sqlcommand.Parameters
                    If Not aParameter.NotColumn AndAlso Not String.IsNullOrEmpty(aParameter.ColumnName) AndAlso Not String.IsNullOrEmpty(aParameter.TableID) Then
                        Dim aTablestore As iormRelationalTableStore = Me.GetTableStore(aParameter.TableID)
                        If aTablestore.Convert2ContainerData(aParameter.ColumnName, invalue:=aParameter.Value, outvalue:=cvtvalue) Then
                            aNativeCommand.Parameters(aParameter.ID).value = cvtvalue
                        Else
                            CoreMessageHandler(message:=" parameter value could not be converted", argument:=aParameter.Value, containerEntryName:=aParameter.ColumnName, containerID:=aParameter.TableID, _
                                                procedure:="adonetdbdriver.RunSqlCommand", messagetype:=otCoreMessageType.InternalError)
                        End If
                    Else
                        If Convert2DBData(invalue:=aParameter.Value, outvalue:=cvtvalue, targetType:=GetTargetTypeFor(aParameter.Datatype)) Then
                            aNativeCommand.Parameters(aParameter.ID).value = cvtvalue
                        Else
                            CoreMessageHandler(message:=" parameter value could not be converted", argument:=aParameter.Value, _
                                                procedure:="adonetdbdriver.RunSqlCommand", messagetype:=otCoreMessageType.InternalError)
                        End If
                    End If

                Next
                '** Input Parameters 
                If Not parametervalues Is Nothing Then
                    ' overwrite the initial values
                    For Each kvp As KeyValuePair(Of String, Object) In parametervalues
                        If aNativeCommand.Parameters.Contains(kvp.Key) Then
                            Dim aParameter = sqlcommand.Parameters.Find(Function(x) x.ID = kvp.Key)

                            If Not aParameter.NotColumn AndAlso Not String.IsNullOrEmpty(aParameter.ColumnName) AndAlso Not String.IsNullOrEmpty(aParameter.TableID) Then
                                Dim aTablestore As iormRelationalTableStore = Me.GetTableStore(aParameter.TableID)
                                If aTablestore.Convert2ContainerData(aParameter.ColumnName, invalue:=kvp.Value, outvalue:=cvtvalue) Then
                                    aNativeCommand.Parameters(aParameter.ID).value = cvtvalue
                                Else
                                    CoreMessageHandler(message:=" parameter value could not be converted", argument:=kvp.Value, containerEntryName:=aParameter.ColumnName, containerID:=aParameter.TableID, _
                                                        procedure:="adonetdbdriver.RunSqlCommand", messagetype:=otCoreMessageType.InternalError)
                                End If
                            Else
                                If Convert2DBData(invalue:=kvp.Value, outvalue:=cvtvalue, targetType:=GetTargetTypeFor(aParameter.Datatype)) Then
                                    aNativeCommand.Parameters(aParameter.ID).value = cvtvalue
                                Else
                                    CoreMessageHandler(message:=" parameter value could not be converted", argument:=kvp.Value, _
                                                        procedure:="adonetdbdriver.RunSqlCommand", messagetype:=otCoreMessageType.InternalError)
                                End If

                            End If

                        End If
                    Next
                End If

                '''
                ''' execute
                Dim result As Integer = aNativeCommand.ExecuteNonQuery()

                '****
                '**** CHECK HERE IF WE CAN TAKE A CACHED DATATABLE FOR THE SQL SELECT
                '****
                If sqlcommand.TableIDs.Count = 1 Then
                    atableid = sqlcommand.TableIDs.First
                    Dim aTablestore As iormRelationalTableStore = Me.GetTableStore(sqlcommand.TableIDs.First)
                    If aTablestore.HasProperty(ormTableStore.ConstTPNCacheProperty) Then
                        '*** BRANCH OUT BUT NOT RETURN !
                        '' RunSqlCommandCached(sqlcommand:=sqlcommand, parametervalues:=parametervalues, nativeConnection:=nativeConnection)
                        ' DATATABLE Doesnot accept general SQL Statements
                        ' this means that we have to recache at the end
                        DirectCast(aTablestore, adonetTableStore).InitializeCache(force:=True)
                    End If
                End If

                Return True

            Catch ex As OleDb.OleDbException
                Call CoreMessageHandler(exception:=ex, procedure:="adonetDBDriver.runSqlCommand", argument:=sqlcommand.SqlText, messagetype:=otCoreMessageType.InternalException)
                If Not aDataReader Is Nothing Then aDataReader.Close()
                Return False
            Catch ex As SqlException
                Call CoreMessageHandler(exception:=ex, procedure:="adonetDBDriver.runSqlCommand", argument:=sqlcommand.SqlText, messagetype:=otCoreMessageType.InternalException)
                If Not aDataReader Is Nothing Then aDataReader.Close()
                Return False
            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="adonetDBDriver.runSqlCommand", argument:=sqlcommand.SqlText, messagetype:=otCoreMessageType.InternalException)
                If Not aDataReader Is Nothing Then aDataReader.Close()
                Return False
            End Try


        End Function

        ''' <summary>
        ''' runs a Sql Select Command and returns a List of Records
        ''' </summary>
        ''' <param name="sqlcommand">a clsOTDBSqlSelectCommand</param>
        ''' <param name="parameters"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RunSqlSelectCommandCached(ByRef sqlcommand As ormSqlSelectCommand, _
                                           Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing, _
                                           Optional nativeConnection As Object = Nothing) As List(Of ormRecord)



            Dim acvtvalue As Object
            '*** Execute and get Results
            Dim aDataReader As IDataReader
            Dim theResults As New List(Of ormRecord)
            Dim atableid As String = Nothing
            Dim abostrophnecessary As Boolean
            Dim wherestr As String = sqlcommand.Where
            Dim aTablestore As iormRelationalTableStore

            Try


                If sqlcommand.TableIDs.Count > 1 Then
                    Call CoreMessageHandler(message:="SqlCommand cannot run against multiple datatables", argument:=sqlcommand.ID, _
                                               procedure:="adonetDBDriver.runsqlselectCommand", messagetype:=otCoreMessageType.InternalError)
                    Return theResults
                Else
                    atableid = sqlcommand.TableIDs.First
                    aTablestore = Me.GetTableStore(sqlcommand.TableIDs.First)
                    If aTablestore.HasProperty(ormTableStore.ConstTPNCacheProperty) Then
                        If Not DirectCast(aTablestore, adonetTableStore).IsCacheInitialized Then
                            DirectCast(aTablestore, adonetTableStore).InitializeCache()
                        End If
                    Else
                        Call CoreMessageHandler(message:="tablestore is not set to caching of table", argument:=sqlcommand.ID, containerID:=aTablestore.ContainerID, _
                                              procedure:="adonetDBDriver.runsqlselectCommand", messagetype:=otCoreMessageType.InternalError)
                        Return theResults
                    End If
                End If

                '** prepare
                If Not sqlcommand.IsPrepared Then
                    If Not sqlcommand.Prepare Then
                        Call CoreMessageHandler(message:="SqlCommand couldn't be prepared", argument:=sqlcommand.ID, _
                                               procedure:="adonetDBDriver.runsqlselectCommand", messagetype:=otCoreMessageType.InternalError)
                        Return theResults
                    End If
                End If

                '***  Assign the values
                '** initial values
                For i = 0 To sqlcommand.Parameters.Count - 1
                    Dim aParameter As ormSqlCommandParameter = sqlcommand.Parameters(i)
                    Dim aParameterValue As Object
                    If parametervalues.Count > i Then
                        aParameterValue = parametervalues.ElementAt(i).Value
                    Else
                        aParameterValue = aParameter.Value
                    End If

                    If Not aParameter.NotColumn AndAlso (Not String.IsNullOrEmpty(aParameter.ColumnName) AndAlso Not String.IsNullOrEmpty(aParameter.TableID)) Then
                        If aTablestore.Convert2ContainerData(aParameter.ColumnName, invalue:=aParameterValue, outvalue:=acvtvalue, abostrophNecessary:=abostrophnecessary) Then
                            ' and build wherestring for cache
                            If abostrophnecessary Then
                                wherestr = wherestr.Replace(aParameter.ID, "'" & acvtvalue.ToString & "'")
                            Else
                                wherestr = wherestr.Replace(aParameter.ID, acvtvalue.ToString)
                            End If
                        Else
                            CoreMessageHandler(message:=" parameter value could not be converted", argument:=aParameter.Value, containerEntryName:=aParameter.ColumnName, containerID:=aParameter.TableID, _
                                                procedure:="adonetdbdriver.RunSqlSelectCommand", messagetype:=otCoreMessageType.InternalError)
                        End If
                    Else
                        If Convert2DBData(invalue:=aParameterValue, outvalue:=acvtvalue, targetType:=GetTargetTypeFor(aParameter.Datatype)) Then
                            ' and build wherestring for cache
                            If abostrophnecessary Then
                                wherestr = wherestr.Replace(aParameter.ID, "'" & acvtvalue.ToString & "'")
                            Else
                                wherestr = wherestr.Replace(aParameter.ID, acvtvalue.ToString)
                            End If
                        Else
                            CoreMessageHandler(message:=" parameter value could not be converted", argument:=aParameter.Value, _
                                                procedure:="adonetdbdriver.RunSqlSelectCommand", messagetype:=otCoreMessageType.InternalError)
                        End If
                    End If

                Next


                Dim dataRows() As DataRow = DirectCast(aTablestore, adonetTableStore).CacheDataTable.Select(wherestr)
                Dim aNewRecord As ormRecord ' free style do not set to a table
                ' not found
                If dataRows.GetLength(0) = 0 Then
                    Return theResults
                Else
                    For Each row In dataRows
                        ''' infuse a record
                        ''' 
                        If sqlcommand.TableIDs.Count = 1 And sqlcommand.AllFieldsAdded Then
                            aNewRecord = New ormRecord(containerID:=sqlcommand.TableIDs.First, dbdriver:=Me)
                        Else
                            aNewRecord = New ormRecord() 'free flow record
                        End If

                        If aNewRecord.LoadFrom(row) Then
                            theResults.Add(item:=aNewRecord)
                        Else
                            Call CoreMessageHandler(procedure:="adonetDBDriver.RunSqlSelectCommand", message:="couldnot infuse a record", _
                                                  argument:=aNewRecord, containerID:=atableid, break:=False)
                        End If

                        'If DirectCast(aTablestore, adonetTableStore).InfuseRecord(aNewRecord, row, CreateNewrecord:=True) Then
                        '    theResults.Add(item:=aNewRecord)
                        'Else
                        '    Call CoreMessageHandler(subname:="adonetDBDriver.RunSqlSelectCommand", message:="couldnot infuse a record", _
                        '                          arg1:=aNewRecord, tablename:=atableid, break:=False)
                        'End If
                    Next
                End If
                Return theResults

            Catch ex As OleDb.OleDbException
                Call CoreMessageHandler(exception:=ex, procedure:="adonetDBDriver.RunSqlSelectCommand", argument:=sqlcommand.SqlText, messagetype:=otCoreMessageType.InternalException)
                If Not aDataReader Is Nothing Then aDataReader.Close()
                Return New List(Of ormRecord)
            Catch ex As SqlException
                Call CoreMessageHandler(exception:=ex, procedure:="adonetDBDriver.runSqlSelectCommand", argument:=sqlcommand.SqlText, messagetype:=otCoreMessageType.InternalException)
                If Not aDataReader Is Nothing Then aDataReader.Close()
                Return New List(Of ormRecord)
            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="adonetDBDriver.runSqlSelectCommand", argument:=sqlcommand.SqlText, messagetype:=otCoreMessageType.InternalException)
                If Not aDataReader Is Nothing Then aDataReader.Close()
                Return New List(Of ormRecord)
            End Try


        End Function
        ''' <summary>
        ''' persists the errorlog
        ''' </summary>
        ''' <param name="TableID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function PersistLog(ByRef log As SessionMessageLog) As Boolean Implements iormRelationalDatabaseDriver.PersistLog


            '** we need a valid connection also nativeInternal could work also
            If _primaryConnection Is Nothing OrElse Not Me._primaryConnection.IsConnected Then
                Return False
            End If

            Try
                If DirectCast(_primaryConnection, adonetConnection).IsNativeInternalLocked Then
                    Return False
                End If

                '** build the command
                If _ErrorLogPersistCommand Is Nothing Then
                    '* get the schema
                    _ErrorLogPersistTableschema = Me.GetTableSchema(SessionMessage.ConstPrimaryTableID)
                    If _ErrorLogPersistTableschema Is Nothing OrElse Not _ErrorLogPersistTableschema.IsInitialized Then
                        Return False
                    End If

                    '** we need just the insert
                    _ErrorLogPersistCommand = DirectCast(_ErrorLogPersistTableschema, adonetTableSchema). _
                        BuildCommand(_ErrorLogPersistTableschema.PrimaryKeyIndexName, _
                                     adonetTableSchema.CommandType.InsertType, _
                                     nativeconnection:=DirectCast(_primaryConnection, adonetConnection).NativeInternalConnection)
                    '** take it on the internal 
                    If _ErrorLogPersistCommand Is Nothing Then
                        'DirectCast(_primaryConnection, adonetConnection).IsNativeInternalLocked = False
                        Return False
                    End If
                End If

                If _ErrorLogPersistCommand.Connection.State = ConnectionState.Open Then
                    PersistLog = False
                    Dim anError As SessionMessage
                    Do
                        anError = log.Retain
                        If anError IsNot Nothing AndAlso Not anError.Processed Then
                            'get all fields -> update
                            For Each fieldname As String In _ErrorLogPersistTableschema.EntryNames
                                ' assign values
                                If Not String.IsNullOrEmpty(fieldname) Then
                                    With _ErrorLogPersistCommand.Parameters.Item("@" & fieldname)
                                        '** set the value of parameter
                                        Select Case fieldname
                                            Case SessionMessage.ConstFNTag
                                                If String.IsNullOrWhiteSpace(anError.Tag) Then
                                                    .value = CurrentSession.Errorlog.Tag
                                                Else
                                                    .Value = anError.Tag
                                                End If
                                            Case SessionMessage.ConstFNno
                                                .value = anError.Entryno
                                            Case SessionMessage.ConstFNmessage
                                                .value = anError.Message
                                            Case SessionMessage.ConstFNtimestamp
                                                .value = anError.Timestamp
                                            Case SessionMessage.ConstFNID
                                                .value = String.Empty
                                            Case SessionMessage.ConstFNsubname
                                                .value = anError.Subname
                                            Case SessionMessage.ConstFNtype
                                                .value = anError.messagetype
                                            Case SessionMessage.ConstFNtablename
                                                .value = anError.Tablename
                                            Case SessionMessage.ConstFNStack
                                                .value = anError.StackTrace
                                            Case SessionMessage.ConstFNColumn
                                                .value = anError.Columnname
                                            Case SessionMessage.ConstFNarg
                                                .value = anError.Arguments
                                            Case SessionMessage.ConstFNUpdatedOn, SessionMessage.ConstFNCreatedOn
                                                .value = Date.Now
                                            Case SessionMessage.ConstFNIsDeleted
                                                .value = False
                                            Case SessionMessage.ConstFNDeletedOn
                                                .value = ConstNullDate
                                            Case SessionMessage.ConstFNUsername
                                                .value = anError.Username
                                            Case SessionMessage.ConstFNObjectname
                                                .value = anError.Objectname
                                            Case SessionMessage.ConstFNObjectentry
                                                .value = anError.ObjectEntry
                                            Case SessionMessage.ConstFNObjectTag
                                                .value = anError.Objecttag
                                            Case SessionMessage.ConstFNDomainID
                                                .value = anError.Domainid
                                            Case Else
                                                .value = DBNull.Value
                                        End Select

                                        If .value Is Nothing Then
                                            .value = DBNull.Value
                                        End If
                                    End With
                                End If
                            Next

                            SyncLock _ErrorLogPersistCommand.Connection
                                If _ErrorLogPersistCommand.ExecuteNonQuery() > 0 Then
                                    anError.Processed = True
                                    PersistLog = PersistLog And True
                                End If
                            End SyncLock

                        End If
                    Loop Until anError Is Nothing

                    'DirectCast(_primaryConnection, adonetConnection).IsNativeInternalLocked = False
                    Return PersistLog
                End If

            Catch ex As Exception
                Console.WriteLine(Date.Now & ": could not flush error log to database")
                'DirectCast(_primaryConnection, adonetConnection).IsNativeInternalLocked = False
                Return False
            End Try

        End Function

        Private Sub adonetDBDriver_RequestBootstrapInstall(sender As Object, e As SessionBootstrapEventArgs) Handles Me.RequestBootstrapInstall

        End Sub
    End Class



    '**************
    '************** ConnectionEventArgs for the ConnectionEvents

    Public Class InternalConnectionEventArgs
        Inherits EventArgs

        Private _Connection As iormConnection
        Private _NativeConnection As IDbConnection

        Public Sub New(newConnection As iormConnection, nativeConnection As IDbConnection)
            _Connection = newConnection
        End Sub
        ''' <summary>
        ''' Gets the native connection.
        ''' </summary>
        ''' <value>The native connection.</value>
        Public ReadOnly Property NativeConnection() As IDbConnection
            Get
                Return Me._NativeConnection
            End Get
        End Property

        ''' <summary>
        ''' Gets the error.
        ''' </summary>
        ''' <value>The error.</value>
        Public ReadOnly Property [Connection]() As iormConnection
            Get
                Return _Connection
            End Get
        End Property

    End Class

    '************************************************************************************
    '***** CLASS adonetConnection describes the Connection description to OnTrack
    '*****        based on ADO.NET  Driver
    '*****

    Public MustInherit Class adonetConnection
        Inherits ormConnection
        Implements iormConnection

        Protected Friend _IsConnected As Boolean = False

        Protected Friend _nativeConnection As IDbConnection
        Protected Friend _nativeinternalConnection As IDbConnection
        Private _IsNativeInternalLocked As Boolean = False

        ' Private _ADOXcatalog As ADOX.Catalog
        'Private _ADOError As ADODB.Error
        Protected Friend Shadows _useseek As Boolean = False 'use seek instead of SQL

        Protected Friend Shadows WithEvents _ErrorLog As New SessionMessageLog(My.Computer.Name & "-" & My.User.Name & "-" & Date.Now.ToUniversalTime)

        Public Shadows Event OnConnection As EventHandler(Of ormConnectionEventArgs) Implements iormConnection.OnConnection
        Public Shadows Event OnDisconnection As EventHandler(Of ormConnectionEventArgs) Implements iormConnection.OnDisconnection
        Public Event OnInternalConnected As EventHandler(Of InternalConnectionEventArgs)

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="DatabaseDriver"></param>
        ''' <param name="session"></param>
        ''' <param name="sequence"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal id As String, ByRef DatabaseDriver As iormRelationalDatabaseDriver, sequence As ComplexPropertyStore.Sequence, Optional ByRef session As Session = Nothing)
            MyBase.New(id, DatabaseDriver, sequence, session:=session)
            _useseek = False
            _nativeConnection = Nothing
            _nativeinternalConnection = Nothing
        End Sub
        '*****
        '***** finalize 
        Protected Overrides Sub Finalize()
            MyBase.Finalize()
            '*** close
            Try
                If Not _nativeConnection Is Nothing AndAlso _nativeConnection.State = ConnectionState.Open Then
                    _nativeConnection.Close()
                End If
            Catch ex As Exception
                'Call CoreMessageHandler(exception:=ex, subname:="adonetConnection.finalize", messagetype:=otCoreMessageType.InternalException _
                '                       )

            End Try

            '*** close
            Try
                If Not _nativeinternalConnection Is Nothing AndAlso _nativeinternalConnection.State = ConnectionState.Open Then
                    _nativeinternalConnection.Close()
                End If
            Catch ex As Exception
                'Call CoreMessageHandler(exception:=ex, subname:="adonetConnection.finalize", messagetype:=otCoreMessageType.InternalException _
                '                       )
            End Try

        End Sub
        Public Shadows Function RaiseOnConnected()
            RaiseEvent OnConnection(Me, New ormConnectionEventArgs(Me))
        End Function
        Public Shadows Function RaiseOnDisConnected()
            RaiseEvent OnDisconnection(Me, New ormConnectionEventArgs(Me))
        End Function
        ''' Gets the is initialized.
        ''' </summary>
        ''' <value>The is initialized.</value>
        Overrides ReadOnly Property isInitialized() As Boolean
            Get
                If _nativeConnection Is Nothing Then
                    Return False
                Else
                    Return True
                End If
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the is native internal locked.
        ''' </summary>
        ''' <value>The is native internal locked.</value>
        Public Property IsNativeInternalLocked() As Boolean
            Get
                Return Me._IsNativeInternalLocked
            End Get
            Set(value As Boolean)
                Me._IsNativeInternalLocked = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the native connection.
        ''' </summary>
        ''' <value>The native connection.</value>
        Friend Overrides ReadOnly Property NativeConnection() As Object
            Get
                If _nativeConnection Is Nothing AndAlso Not _Session.IsBootstrappingInstallationRequested Then
                    Return Nothing
                ElseIf _nativeConnection IsNot Nothing AndAlso _nativeConnection.State <> ConnectionState.Open AndAlso Not _Session.IsBootstrappingInstallationRequested Then
                    Throw New ormNoConnectionException(message:="connection to database lost - state is not open", subname:="adonetConnection.NativeConnection", path:=Me.PathOrAddress)
                ElseIf _Session.IsBootstrappingInstallationRequested AndAlso _nativeinternalConnection IsNot Nothing Then
                    Return _nativeinternalConnection
                Else
                    Return Me._nativeConnection
                End If
            End Get
        End Property
        ''' <summary>
        ''' Gets the is connected.
        ''' </summary>
        ''' <value>The is connected.</value>
        Public Overrides ReadOnly Property IsConnected() As Boolean
            Get
                Return _IsConnected
            End Get

        End Property


        ''' <summary>
        ''' Disconnects this instance of the connection with raising events
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function Disconnect() As Boolean

            If Not MyBase.Disconnect() Then
                Return False
            End If


            ' Raise the event
            RaiseEvent OnDisconnection(Me, New ormConnectionEventArgs(Me))

            '***
            If Not _nativeConnection Is Nothing AndAlso _nativeConnection.State <> ConnectionState.Open Then
                '** close
                _nativeConnection.Close()
            End If

            '*** reset
            Call ResetFromConnection()
            '***
            Call CoreMessageHandler(showmsgbox:=False, message:=" Connection disconnected ", _
                                  break:=True, noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationInfo, _
                                  procedure:="Session.Disconnect")

            '** close also the internal connection
            If Not _nativeinternalConnection Is Nothing AndAlso _nativeinternalConnection.State <> ConnectionState.Closed Then
                _nativeinternalConnection.Close()
                _nativeinternalConnection = Nothing
            End If

            Return True
        End Function


        ''' <summary>
        ''' gets the native internal connection
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overridable ReadOnly Property NativeInternalConnection As IDbConnection
            Get
                If _nativeinternalConnection Is Nothing OrElse _nativeinternalConnection.State <> ConnectionState.Open Then
                    Try
                        '**** retrieve ConfigParameters
                        If Not Me.SetConnectionConfigParameters() Then
                            Call CoreMessageHandler(showmsgbox:=True, message:="Configuration Parameters could not be retrieved from a data source", _
                                                  procedure:="adonetConnection.Connect")
                            Return Nothing
                        End If
                        ' connect 
                        _nativeinternalConnection = CreateNewNativeConnection()
                        _nativeinternalConnection.ConnectionString = Me.Connectionstring
                        _nativeinternalConnection.Open()
                        ' check if state is open
                        If _nativeinternalConnection.State = ConnectionState.Open Then
                            RaiseEvent OnInternalConnected(Me, New InternalConnectionEventArgs(newConnection:=Me, nativeConnection:=_nativeinternalConnection))
                            Return _nativeinternalConnection
                        Else
                            Call CoreMessageHandler(showmsgbox:=False, message:="internal connection couldnot be established", messagetype:=otCoreMessageType.InternalError,
                                                  procedure:="adonetConnection.NativeInternalConnection")

                            Throw New ormNoConnectionException(message:="internal connection couldnot be established", subname:="adonetConnection.NativeInternalConnection", path:=Me.PathOrAddress)
                            Return Nothing
                        End If
                    Catch ex As SqlException

                        Call CoreMessageHandler(showmsgbox:=True, message:="internal connection to database could not be established", messagetype:=otCoreMessageType.InternalError, _
                                              procedure:="adonetConnection.NativeInternalConnection", exception:=ex)
                        Throw New ormNoConnectionException(message:="internal connection couldnot be established", exception:=ex, subname:="adonetConnection.NativeInternalConnection", path:=Me.PathOrAddress)

                        Return Nothing

                    Catch ex As Exception
                        Call CoreMessageHandler(showmsgbox:=True, message:="internal connection couldnot be established", messagetype:=otCoreMessageType.InternalError, _
                                              procedure:="adonetConnection.NativeInternalConnection", exception:=ex)
                        Throw New ormNoConnectionException(message:="internal connection couldnot be established", exception:=ex, subname:="adonetConnection.NativeInternalConnection", path:=Me.PathOrAddress)


                        Return Nothing
                    Finally
                        ' Return Nothing
                    End Try


                Else
                    Return Me._nativeinternalConnection
                End If
            End Get
        End Property


        '*****
        '***** reset : reset all the private members for a connection
        Protected Friend Overrides Sub ResetFromConnection()
            Call MyBase.ResetFromConnection()
            '** close the native Connection
            If Not _nativeConnection Is Nothing AndAlso _nativeConnection.State <> ConnectionState.Open Then
                _nativeConnection.Close()
            End If
            'If Not _nativeinternalConnection Is Nothing AndAlso _nativeinternalConnection.State <> ObjectStateEnum.adStateClosed Then
            '_nativeinternalConnection.Close()
            'End If
            _IsConnected = False
            _nativeConnection = Nothing

            '_nativeinternalConnection = Nothing

        End Sub

        ''' <summary>
        ''' create a new native Connection (not connected)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function CreateNewNativeConnection() As IDbConnection


        ''' <summary>
        ''' Connects the specified FORCE.
        ''' </summary>
        ''' <param name="FORCE">The FORCE.</param>
        ''' <param name="AccessRequest">The access request.</param>
        ''' <param name="OTDBUsername">The OTDB username.</param>
        ''' <param name="OTDBPassword">The OTDB password.</param>
        ''' <param name="exclusive">The exclusive.</param>
        ''' <param name="notInitialize">The not initialize.</param>
        ''' <returns></returns>
        Public Overrides Function Connect(Optional FORCE As Boolean = False, _
                                            Optional accessRequest As otAccessRight = otAccessRight.[ReadOnly], _
                                            Optional domainid As String = Nothing, _
                                            Optional OTDBUsername As String = Nothing, _
                                            Optional OTDBPassword As String = Nothing, _
                                            Optional exclusive As Boolean = False, _
                                            Optional notInitialize As Boolean = False, _
                                            Optional doLogin As Boolean = True) As Boolean

            ' return if connection is there
            If Not _nativeConnection Is Nothing And Not FORCE Then
                ' stay in the connection if we donot need another state -> Validate the Request
                ' if there is a connection and we have no need for higher access -> return
                If _nativeConnection.State = ConnectionState.Open And Me.ValidateAccessRequest(accessrequest:=accessRequest) Then
                    ' initialize the parameter values of the OTDB
                    Call Initialize(force:=False)
                    Return True

                ElseIf _nativeConnection.State <> ConnectionState.Closed Then
                    _nativeConnection.Close()
                Else
                    'Set otdb_connection = Nothing
                    ' reset
                    System.Diagnostics.Debug.WriteLine("reseting")
                End If
            End If

            '*** check On Track and just the kernel
            If Not Me.DatabaseDriver.VerifyOnTrackDatabase(install:=False) Then
                Call CoreMessageHandler(showmsgbox:=True, message:="OnTrack Database Verify failed - use 'create Schema' in Setting form to re-install it", _
                                     procedure:="adonetConnection.Connect", noOtdbAvailable:=True, messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            '**** retrieve ConfigParameters
            If Not Me.SetConnectionConfigParameters() Then
                Call CoreMessageHandler(showmsgbox:=True, message:="Configuration Parameters couldnot be retrieved from a data source", _
                                      procedure:="adonetConnection.Connect", noOtdbAvailable:=True, messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            '*** verify the User
            If Not _Session.ValidateUser(accessRequest:=accessRequest, username:=OTDBUsername, _
                                           password:=OTDBPassword, domainID:=domainid) Then
                Call CoreMessageHandler(showmsgbox:=True, message:="Connect not possible - user could not be validated", argument:=OTDBUsername, _
                                    procedure:="adonetConnection.Connect", noOtdbAvailable:=True, messagetype:=otCoreMessageType.ApplicationError)
                If Me.IsConnected Then
                    Me.Disconnect()
                End If
                Return False
            End If


            '*** we are connected =!
            If Me.IsConnected Then
                Me.Disconnect()
            End If
            '*** create the connection
            _nativeConnection = CreateNewNativeConnection()

            Try
                If Me.Connectionstring = Nothing Then
                    Call CoreMessageHandler(messagetype:=otCoreMessageType.InternalError, message:="Connection String to Database is empty", _
                                           procedure:="adonetConnection.Connect", argument:=Me.Connectionstring)
                    ResetFromConnection()
                    Return False
                End If
                ' set dbpassword
                _nativeConnection.ConnectionString = Me.Connectionstring

                ' open again
                _nativeConnection.Open()
                ' check if state is open
                If _nativeConnection.State = ConnectionState.Open Then
                    ' set the Access Request
                    _AccessLevel = accessRequest
                    _IsConnected = True ' even with no valid User Defintion we are Connection (otherwise we cannot load)
                    _OTDBDatabaseDriver.SetDBParameter("lastLogin_user", OTDBUsername)
                    _OTDBDatabaseDriver.SetDBParameter("lastLogin_timestamp", Date.Now.ToString)
                    _Dbuser = OTDBUsername
                    _Dbpassword = OTDBPassword


                    ' raise Connected Event
                    RaiseEvent OnConnection(Me, New ormConnectionEventArgs(Me, domainid))
                    ' return true
                    Return True
                End If

            Catch ex As System.Data.DataException
                Call CoreMessageHandler(showmsgbox:=True, message:="internal connection to database could not be established" & vbLf, _
                                      procedure:="adonetConnection.Connect", exception:=ex)
                If Not _nativeConnection Is Nothing AndAlso _nativeConnection.State <> ConnectionState.Closed Then
                    _nativeConnection.Close()
                End If
                '*** reset
                Call ResetFromConnection()
                Return False

            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=True, procedure:="adonetConnection.Connect", exception:=ex, _
                                      argument:=_Connectionstring, noOtdbAvailable:=True, break:=False)
                If Not _nativeConnection Is Nothing AndAlso _nativeConnection.State <> ConnectionState.Closed Then
                    _nativeConnection.Close()
                End If
                '*** reset
                Call ResetFromConnection()
                Return False
            End Try

        End Function


    End Class



    '************************************************************************************
    '***** CLASS adonetTableSchema  CLASS describes the schema per table of the database itself
    '*****        based on ADO.NET OLEDB Driver
    '*****

    ''' <summary>
    ''' CLASS describes the schema per table of the database itself
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class adonetTableSchema
        Inherits ormTableSchema
        Implements iormContainerSchema

        '** own CommandKey

        Enum CommandType
            SelectType
            UpdateType
            DeleteType
            InsertType
        End Enum

        Structure CommandKey
            Public IndexName As String
            Public CommandType As CommandType
            Public Sub New(name As String, type As CommandType)
                IndexName = name
                CommandType = type
            End Sub
        End Structure

        '***** internal variables
        '*****

        Protected _ColumnsTable As DataTable
        Protected _IndexTable As DataTable
        Protected _Columns() As adonetColumnDescription

        '**** CommandStore
        Protected _CommandStore As New Dictionary(Of CommandKey, IDbCommand)


        ''' <summary>
        ''' Initializes a new instance of the <see cref="adonetTableSchema" /> class.
        ''' </summary>
        ''' <param name="connection">The connection.</param>
        ''' <param name="tableID">The table ID.</param>
        Public Sub New(ByRef connection As iormConnection, tableID As String)
            MyBase.New(connection, tableID)
        End Sub

        Protected Overrides Sub Finalize()
            _CommandStore = Nothing
            _Connection = Nothing
            _ColumnsTable = Nothing
            _IndexTable = Nothing
        End Sub
        ''' <summary>
        ''' resets the TableSchema
        ''' </summary>
        ''' <remarks></remarks>
        Protected Overrides Sub reset()
            Call MyBase.Reset()
            _CommandStore.Clear()
            _ColumnsTable.Clear()
            _IsInitialized = False
            _IndexTable.Clear()
            _Columns = Nothing

        End Sub

        ''' <summary>
        ''' returns a Default Value for a fieldname
        ''' </summary>
        ''' <param name="fieldname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GetDefaultValue(index As Object) As Object Implements iormContainerSchema.GetDefaultValue
            Dim i As Integer = Me.GetEntryOrdinal(index:=index)
            Dim result As Object
            Dim aDesc As adonetColumnDescription

            If i >= 0 Then
                aDesc = Me.GetColumnDescription(i)
                If aDesc IsNot Nothing AndAlso aDesc.HasDefault Then
                    Dim aTablestore As iormRelationalTableStore = _Connection.DatabaseDriver.GetTableStore(Me.TableID)
                    If aTablestore.Convert2ObjectData(i, invalue:=aDesc.Default, outvalue:=result, isnullable:=False, defaultvalue:=aDesc.Default) Then
                        Return result
                    Else
                        Return Nothing
                    End If
                End If
            End If

            Return Nothing

        End Function
        ''' <summary>
        ''' returns the nullable property for a fieldname
        ''' </summary>
        ''' <param name="fieldname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GetNullable(index As Object) As Boolean Implements iormContainerSchema.GetNullable
            Dim i As Integer = Me.GetEntryOrdinal(index:=index)
            Dim result As Object
            Dim aDesc As adonetColumnDescription

            If i >= 0 Then
                aDesc = Me.GetColumnDescription(i)
                Return aDesc.IsNullable
            End If

            Return False

        End Function
        ''' <summary>
        ''' returns true if default value exists for fieldname by index
        ''' </summary>
        ''' <param name="fieldname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function HasDefaultValue(index As Object) As Boolean Implements iormContainerSchema.HasDefaultValue
            Dim i As Integer = Me.GetEntryOrdinal(index:=index)
            Dim aDesc As adonetColumnDescription

            If i >= 0 Then
                aDesc = Me.GetColumnDescription(i)
                If aDesc IsNot Nothing Then
                    Return aDesc.HasDefault
                End If
            End If

            Return False

        End Function
        ''' <summary>
        ''' get the ColumnDescription of Field 
        ''' </summary>
        ''' <param name="Index">Index no</param>
        ''' <returns>ColumnDescription</returns>
        ''' <remarks>Returns Nothing on range bound exception</remarks>
        Public Function GetColumnDescription(index As UShort) As adonetColumnDescription
            If index > 0 And index <= _Columns.Length Then
                Return _Columns(index - 1)
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' return a Command
        ''' </summary>
        ''' <param name="indexname"></param>
        ''' <param name="commandtype"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCommand(ByVal indexname As String, ByVal commandtype As CommandType) As IDbCommand

            If Not _indexDictionary.ContainsKey(indexname) Then
                Call CoreMessageHandler(procedure:="adonetTableSchema.getCommand", message:="indexname not in IndexDictionary", _
                                      argument:=indexname, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If
            '** return
            Dim aKey = New CommandKey(indexname, commandtype)
            If _CommandStore.ContainsKey(aKey) Then
                Return _CommandStore.Item(aKey)
            Else
                Return Nothing
            End If

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function CreateNativeDBParameter() As IDbDataParameter
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function CreateNativeDBCommand() As IDbCommand
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function IsNativeDBTypeOfVar(type As Object) As Boolean


        ''' <summary>
        ''' buildcommand builds per Indexname and commandtype the Command and prepare it
        ''' </summary>
        ''' <param name="commandtype">type of adonetTableSchema.commandtype</param>
        ''' <param name="indexname">name of the index</param>
        ''' <returns>the IDBCommand </returns>
        ''' <remarks></remarks>
        Protected Friend Function BuildCommand(ByVal indexname As String, _
                                               ByVal commandtype As CommandType, _
                                               Optional ByRef nativeconnection As IDbConnection = Nothing) As IDbCommand

            ' set the IndxColumns
            Dim aColumnCollection As ArrayList
            Dim theIndexColumns() As Object
            Dim commandstr As String
            Dim aParameter As IDataParameter

            Try

                '' do not use initialized since buildcommand is part of initialized
                '' 
                If Me.NoEntries = 0 Then
                    Call CoreMessageHandler(procedure:="adonetTableSchema.buildcommand", message:="table schema is not initialized - does it exist ?", _
                                          argument:=indexname, messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If

                Dim aCommand As IDbCommand = CreateNativeDBCommand()

                If nativeconnection Is Nothing And _Connection.NativeConnection IsNot Nothing Then
                    nativeconnection = DirectCast(Me._Connection.NativeConnection, IDbConnection)
                    '** build on internal
                ElseIf _Connection.NativeConnection Is Nothing And _Connection.Session.IsBootstrappingInstallationRequested Then
                    nativeconnection = DirectCast(DirectCast(_Connection, adonetConnection).NativeInternalConnection, IDbConnection)
                    'CoreMessageHandler(message:="note: native internal connection used to build and drive commands during bootstrap", arg1:=_Connection.ID, _
                    '                   subname:="adonetTableSchema.buildCommand", _
                    '                  messagetype:=otCoreMessageType.InternalInfo, tablename:=me.tableid)
                ElseIf nativeconnection Is Nothing Then
                    CoreMessageHandler(message:="no internal connection in the connection", argument:=_Connection.ID, procedure:="adonetTableSchema.buildCommand", _
                                       messagetype:=otCoreMessageType.InternalError, containerID:=Me.TableID)
                    Return Nothing
                End If


                '*****
                '***** BUILD THE DIFFERENT COMMANDS
                '*****
                Select Case (commandtype)


                    '*********
                    '********* SELECT
                    '*********
                    Case adonetTableSchema.CommandType.SelectType
                        ' set the IndxColumns
                        If Not _indexDictionary.ContainsKey(indexname) Then
                            Call CoreMessageHandler(procedure:="adonetTableSchema.buildcommand", message:="indexname not in IndexDictionary", _
                                                  argument:=indexname, messagetype:=otCoreMessageType.InternalError)
                            Return Nothing
                        Else
                            aColumnCollection = _indexDictionary.Item(key:=indexname)
                            theIndexColumns = aColumnCollection.ToArray
                        End If
                        commandstr = "SELECT "
                        For i = 0 To _entrynames.GetUpperBound(0)
                            commandstr &= String.Format("[{0}].[{1}]", Me.NativeTablename, _entrynames(i))
                            If i <> _entrynames.GetUpperBound(0) Then
                                commandstr &= " , "
                            Else
                                commandstr &= " "
                            End If
                        Next
                        commandstr &= "FROM [" & Me.NativeTablename & "]"
                        '**
                        '** where
                        commandstr &= " WHERE "
                        For i = 0 To theIndexColumns.GetUpperBound(0)
                            If i > _entrynames.GetLowerBound(0) Then
                                commandstr &= " AND "
                            End If
                            commandstr &= String.Format("[{0}].[{1}] = @{1}", Me.NativeTablename, theIndexColumns(i))

                        Next

                        '** Add the Parameters
                        '**
                        aCommand.CommandText = commandstr
                        aCommand.Connection = nativeconnection
                        aCommand.CommandType = Data.CommandType.Text
                        For i = 0 To theIndexColumns.GetUpperBound(0)
                            aParameter = AssignNativeDBParameter(theIndexColumns(i))
                            If Not aParameter Is Nothing Then aCommand.Parameters.Add(aParameter)
                        Next
                        SyncLock aCommand.Connection
                            aCommand.Prepare()
                        End SyncLock
                        Return aCommand

                        '*********
                        '********* INSERT
                        '*********
                    Case adonetTableSchema.CommandType.InsertType

                        commandstr = "INSERT INTO [" & Me.NativeTablename & "] ( "
                        For i = 0 To _entrynames.GetUpperBound(0)
                            commandstr &= "[" & _entrynames(i) & "]"
                            If i <> _entrynames.GetUpperBound(0) Then
                                commandstr &= " , "
                            Else
                                commandstr &= " "
                            End If
                        Next
                        commandstr &= ") "
                        '**
                        '** where
                        commandstr &= " VALUES( "
                        For i = 0 To _entrynames.GetUpperBound(0)
                            commandstr &= "@" & _entrynames(i)
                            If i <> _entrynames.GetUpperBound(0) Then
                                commandstr &= " , "
                            Else
                                commandstr &= " "
                            End If
                        Next
                        commandstr &= ")"

                        '** Add the Parameters
                        '**
                        aCommand.CommandText = commandstr
                        aCommand.Connection = nativeconnection
                        aCommand.CommandType = Data.CommandType.Text
                        For i = 0 To _entrynames.GetUpperBound(0)
                            aParameter = AssignNativeDBParameter(_entrynames(i))
                            If Not aParameter Is Nothing Then aCommand.Parameters.Add(aParameter)
                        Next
                        SyncLock aCommand.Connection
                            aCommand.Prepare()
                        End SyncLock
                        Return aCommand

                        '*********
                        '********* UPDATE
                        '*********
                    Case adonetTableSchema.CommandType.UpdateType
                        ' set the IndxColumns
                        If Not _indexDictionary.ContainsKey(indexname) Then
                            Call CoreMessageHandler(procedure:="adonetTableSchema.buildcommand", message:="index name not in IndexDictionary", _
                                                  argument:=indexname, messagetype:=otCoreMessageType.InternalError)
                            Return Nothing
                        Else
                            aColumnCollection = _indexDictionary.Item(key:=indexname)
                            theIndexColumns = aColumnCollection.ToArray
                        End If
                        commandstr = "UPDATE [" & Me.NativeTablename & "]"
                        commandstr &= " SET "
                        Dim first As Boolean = True
                        For i = 0 To _entrynames.GetUpperBound(0)
                            '* do not include primary keys
                            If Not MyBase.HasPrimaryEntryName(_entrynames(i)) Then
                                If Not first Then
                                    commandstr &= ", "
                                End If
                                commandstr &= String.Format("[{0}] = @{0}", _entrynames(i))
                                first = False
                            End If

                        Next
                        '**
                        '** where
                        commandstr &= " WHERE "
                        For i = 0 To theIndexColumns.GetUpperBound(0)
                            If i > _entrynames.GetLowerBound(0) Then
                                commandstr &= " AND "
                            End If
                            commandstr &= String.Format("[{0}].[{1}] = @{1}", Me.NativeTablename, theIndexColumns(i))
                        Next

                        '** Add the Parameters
                        '**
                        aCommand.CommandText = commandstr
                        aCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord
                        aCommand.Connection = nativeconnection
                        aCommand.CommandType = Data.CommandType.Text

                        '** UPDATE FIELDS
                        '**
                        For i = 0 To _entrynames.GetUpperBound(0)
                            If Not MyBase.HasPrimaryEntryName(_entrynames(i)) Then
                                aParameter = AssignNativeDBParameter(_entrynames(i))
                                If Not aParameter Is Nothing Then aCommand.Parameters.Add(aParameter)
                            End If
                        Next
                        '***
                        '*** WHERE CLAUSE
                        For i = 0 To theIndexColumns.GetUpperBound(0)
                            aParameter = AssignNativeDBParameter(theIndexColumns(i))
                            If Not aParameter Is Nothing Then aCommand.Parameters.Add(aParameter)
                        Next
                        SyncLock aCommand.Connection
                            aCommand.Prepare()
                        End SyncLock

                        Return aCommand
                        '*********
                        '********* DELETE
                        '*********
                    Case adonetTableSchema.CommandType.DeleteType
                        ' set the IndxColumns
                        If Not _indexDictionary.ContainsKey(indexname) Then
                            Call CoreMessageHandler(procedure:="adonetTableSchema.buildcommand", message:="indexname not in IndexDictionary", _
                                                  argument:=indexname, messagetype:=otCoreMessageType.InternalError)
                            Return Nothing
                        Else
                            aColumnCollection = _indexDictionary.Item(key:=indexname)
                            theIndexColumns = aColumnCollection.ToArray
                        End If
                        commandstr = "DELETE FROM [" & Me.NativeTablename & "]"

                        '**
                        '** where
                        commandstr &= " WHERE "
                        For i = 0 To theIndexColumns.GetUpperBound(0)
                            If i > _entrynames.GetLowerBound(0) Then
                                commandstr &= " AND "
                            End If
                            commandstr &= String.Format("[{0}].[{1}] = @{1}", Me.NativeTablename, theIndexColumns(i))
                        Next

                        '** Add the Parameters
                        '**
                        aCommand.CommandText = commandstr
                        aCommand.Connection = nativeconnection
                        aCommand.CommandType = Data.CommandType.Text
                        For i = 0 To theIndexColumns.GetUpperBound(0)
                            aParameter = AssignNativeDBParameter(theIndexColumns(i))
                            If Not aParameter Is Nothing Then aCommand.Parameters.Add(aParameter)
                        Next
                        SyncLock aCommand.Connection
                            aCommand.Prepare()
                        End SyncLock
                        Return aCommand
                    Case Else
                        Call CoreMessageHandler(procedure:="adonetTableSchema.buildcommand", message:="commandtype not recognized or implemented", _
                                              argument:=commandtype, messagetype:=otCoreMessageType.InternalError)
                        Return Nothing
                End Select

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="adonetTableSchema.buildcommand", message:="exception for " & indexname, _
                                      argument:=commandtype.ToString & ":" & commandstr, messagetype:=otCoreMessageType.InternalError, exception:=ex)
                Return Nothing
            End Try
        End Function


    End Class

    ''' <summary>
    '''  describes the schema per view of the database itself
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class adonetViewSchema
        Inherits ormViewSchema
        Implements iormContainerSchema

        ''' <summary>
        ''' CommandType
        ''' </summary>
        ''' <remarks></remarks>

        Enum CommandType
            SelectType
        End Enum

        Structure CommandKey
            Public IndexName As String
            Public CommandType As CommandType
            Public Sub New(name As String, type As CommandType)
                IndexName = name
                CommandType = type
            End Sub
        End Structure

        '***** internal variables
        '*****

        Protected _ColumnsTable As DataTable
        Protected _IndexTable As DataTable
        Protected _Columns() As adonetColumnDescription

        '**** CommandStore
        Protected _CommandStore As New Dictionary(Of CommandKey, IDbCommand)


        ''' <summary>
        ''' Initializes a new instance of the <see cref="adonetTableSchema" /> class.
        ''' </summary>
        ''' <param name="connection">The connection.</param>
        ''' <param name="tableID">The table ID.</param>
        Public Sub New(ByRef connection As iormConnection, tableID As String)
            MyBase.New(connection, tableID)
        End Sub

        Protected Overrides Sub Finalize()
            _CommandStore = Nothing
            _Connection = Nothing
            _ColumnsTable = Nothing
            _IndexTable = Nothing
        End Sub
        ''' <summary>
        ''' resets the TableSchema
        ''' </summary>
        ''' <remarks></remarks>
        Protected Overrides Sub reset()
            Call MyBase.Reset()
            _CommandStore.Clear()
            _ColumnsTable.Clear()
            _IsInitialized = False
            _IndexTable.Clear()
            _Columns = Nothing

        End Sub

        ''' <summary>
        ''' returns a Default Value for a fieldname
        ''' </summary>
        ''' <param name="fieldname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GetDefaultValue(index As Object) As Object Implements iormContainerSchema.GetDefaultValue
            Dim i As Integer = Me.GetEntryOrdinal(index:=index)
            Dim result As Object
            Dim aDesc As adonetColumnDescription

            If i >= 0 Then
                aDesc = Me.GetColumnDescription(i)
                If aDesc IsNot Nothing AndAlso aDesc.HasDefault Then
                    Dim aViewReader As iormRelationalTableStore = _Connection.DatabaseDriver.GetViewReader(Me.ViewID)
                    If aViewReader.Convert2ObjectData(i, invalue:=aDesc.Default, outvalue:=result, isnullable:=False, defaultvalue:=aDesc.Default) Then
                        Return result
                    Else
                        Return Nothing
                    End If
                End If
            End If

            Return Nothing

        End Function
        ''' <summary>
        ''' returns the nullable property for a fieldname
        ''' </summary>
        ''' <param name="fieldname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GetNullable(index As Object) As Boolean Implements iormContainerSchema.GetNullable
            Dim i As Integer = Me.GetEntryOrdinal(index:=index)
            Dim aDesc As adonetColumnDescription

            If i >= 0 Then
                aDesc = Me.GetColumnDescription(i)
                Return aDesc.IsNullable
            End If

            Return False

        End Function
        ''' <summary>
        ''' returns true if default value exists for fieldname by index
        ''' </summary>
        ''' <param name="fieldname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function HasDefaultValue(index As Object) As Boolean Implements iormContainerSchema.HasDefaultValue
            Dim i As Integer = Me.GetEntryOrdinal(index:=index)
            Dim aDesc As adonetColumnDescription

            If i >= 0 Then
                aDesc = Me.GetColumnDescription(i)
                If aDesc IsNot Nothing Then
                    Return aDesc.HasDefault
                End If
            End If

            Return False

        End Function
        ''' <summary>
        ''' get the ColumnDescription of Field 
        ''' </summary>
        ''' <param name="Index">Index no</param>
        ''' <returns>ColumnDescription</returns>
        ''' <remarks>Returns Nothing on range bound exception</remarks>
        Public Function GetColumnDescription(index As UShort) As adonetColumnDescription
            If index > 0 And index <= _Columns.Length Then
                Return _Columns(index - 1)
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' return a Command
        ''' </summary>
        ''' <param name="indexname"></param>
        ''' <param name="commandtype"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCommand(ByVal indexname As String, ByVal commandtype As CommandType) As IDbCommand

            If Not _indexDictionary.ContainsKey(indexname) Then
                Call CoreMessageHandler(procedure:="adonetViewSchema.getCommand", message:="indexname not in IndexDictionary", _
                                      argument:=indexname, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If
            '** return
            Dim aKey = New CommandKey(indexname, commandtype)
            If _CommandStore.ContainsKey(aKey) Then
                Return _CommandStore.Item(aKey)
            Else
                Return Nothing
            End If

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function CreateNativeDBParameter() As IDbDataParameter
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function CreateNativeDBCommand() As IDbCommand
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function IsNativeDBTypeOfVar(type As Object) As Boolean


        ''' <summary>
        ''' buildcommand builds per Indexname and commandtype the Command and prepare it
        ''' </summary>
        ''' <param name="commandtype">type of adonetTableSchema.commandtype</param>
        ''' <param name="indexname">name of the index</param>
        ''' <returns>the IDBCommand </returns>
        ''' <remarks></remarks>
        Protected Friend Function BuildCommand(ByVal indexname As String, _
                                               ByVal commandtype As CommandType, _
                                               Optional ByRef nativeconnection As IDbConnection = Nothing) As IDbCommand

            ' set the IndxColumns
            Dim aColumnCollection As ArrayList
            Dim theIndexColumns() As Object
            Dim commandstr As String
            Dim aParameter As IDataParameter

            Try

                '' do not use initialized since buildcommand is part of initialized
                '' 
                If Me.NoEntries = 0 Then
                    Call CoreMessageHandler(procedure:="adonetViewSchema.buildcommand", message:="table schema is not initialized - does it exist ?", _
                                          argument:=indexname, messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If

                Dim aCommand As IDbCommand = CreateNativeDBCommand()

                If nativeconnection Is Nothing And _Connection.NativeConnection IsNot Nothing Then
                    nativeconnection = DirectCast(Me._Connection.NativeConnection, IDbConnection)
                    '** build on internal
                ElseIf _Connection.NativeConnection Is Nothing And _Connection.Session.IsBootstrappingInstallationRequested Then
                    nativeconnection = DirectCast(DirectCast(_Connection, adonetConnection).NativeInternalConnection, IDbConnection)
                    'CoreMessageHandler(message:="note: native internal connection used to build and drive commands during bootstrap", arg1:=_Connection.ID, _
                    '                   subname:="adonetTableSchema.buildCommand", _
                    '                  messagetype:=otCoreMessageType.InternalInfo, tablename:=me.tableid)
                ElseIf nativeconnection Is Nothing Then
                    CoreMessageHandler(message:="no internal connection in the connection", argument:=_Connection.ID, procedure:="adonetViewSchema.buildCommand", _
                                       messagetype:=otCoreMessageType.InternalError, containerID:=Me.ViewID)
                    Return Nothing
                End If


                '*****
                '***** BUILD THE DIFFERENT COMMANDS
                '*****
                Select Case (commandtype)


                    '*********
                    '********* SELECT
                    '*********
                    Case commandtype.SelectType
                        ' set the IndxColumns
                        If Not _indexDictionary.ContainsKey(indexname) Then
                            Call CoreMessageHandler(procedure:="adonetViewSchema.buildcommand", message:="indexname not in IndexDictionary", _
                                                  argument:=indexname, messagetype:=otCoreMessageType.InternalError)
                            Return Nothing
                        Else
                            aColumnCollection = _indexDictionary.Item(key:=indexname)
                            theIndexColumns = aColumnCollection.ToArray
                        End If
                        commandstr = "SELECT "
                        For i = 0 To _entrynames.GetUpperBound(0)
                            commandstr &= String.Format("[{0}].[{1}]", Me.NativeViewname, _entrynames(i))
                            If i <> _entrynames.GetUpperBound(0) Then
                                commandstr &= " , "
                            Else
                                commandstr &= " "
                            End If
                        Next
                        commandstr &= "FROM [" & Me.NativeViewname & "]"
                        '**
                        '** where
                        commandstr &= " WHERE "
                        For i = 0 To theIndexColumns.GetUpperBound(0)
                            If i > _entrynames.GetLowerBound(0) Then
                                commandstr &= " AND "
                            End If
                            commandstr &= String.Format("[{0}].[{1}] = @{1}", Me.NativeViewname, theIndexColumns(i))

                        Next

                        '** Add the Parameters
                        '**
                        aCommand.CommandText = commandstr
                        aCommand.Connection = nativeconnection
                        aCommand.CommandType = Data.CommandType.Text
                        For i = 0 To theIndexColumns.GetUpperBound(0)
                            aParameter = AssignNativeDBParameter(theIndexColumns(i))
                            If Not aParameter Is Nothing Then aCommand.Parameters.Add(aParameter)
                        Next
                        SyncLock aCommand.Connection
                            aCommand.Prepare()
                        End SyncLock
                        Return aCommand


                    Case Else
                        Call CoreMessageHandler(procedure:="adonetViewSchema.buildcommand", message:="commandtype not recognized or implemented", _
                                              argument:=commandtype, messagetype:=otCoreMessageType.InternalError)
                        Return Nothing
                End Select

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="adonetViewSchema.buildcommand", message:="exception for " & indexname, _
                                      argument:=commandtype.ToString & ":" & commandstr, messagetype:=otCoreMessageType.InternalError, exception:=ex)
                Return Nothing
            End Try
        End Function


    End Class



    ''' <summary>
    '''  describes an abstract table store on the ORM level
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class adonetViewReader
        Inherits ormViewReader
        Implements iormRelationalTableStore


        Protected Friend _cacheTable As DataTable  ' DataTable to cache it
        Protected Friend _cacheViews As New Dictionary(Of String, DataView) ' Dictionary for Dataview per Index
        Protected Friend _cacheAdapter As Data.IDbDataAdapter



        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <param name="TableID"></param>
        ''' <param name="forceSchemaReload"></param>
        ''' <remarks></remarks>

        Public Sub New(connection As iormConnection, viewID As String, ByVal forceSchemaReload As Boolean)
            Call MyBase.New(Connection:=connection, viewid:=viewID, force:=forceSchemaReload)
        End Sub

        ''' <summary>
        ''' gets the current cache Table
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CacheDataTable As DataTable
            Get
                Return _cacheTable
            End Get
        End Property
        ''' <summary>
        ''' gets an enumerable of the cached views (indices)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CacheDataViews As IEnumerable(Of DataView)
            Get
                Return _cacheViews
            End Get
        End Property
        ''' converts a Object from OTDB VB.NET Data to ColumnData in the Database
        ''' </summary>
        ''' <param name="index"></param>
        ''' <param name="value"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <returns>the converted object</returns>
        ''' <remarks></remarks>
        Public Overloads Function Convert2ContainerData(ByVal index As Object, _
                                                     ByVal invalue As Object, _
                                                     ByRef outvalue As Object, _
                                                    Optional ByRef abostrophNecessary As Boolean = False, _
                                                    Optional isnullable As Boolean? = Nothing, _
                                                    Optional defaultvalue As Object = Nothing) As Boolean Implements iormRelationalTableStore.Convert2ContainerData
            Dim aSchema As adonetTableSchema = Me.ContainerSchema
            Dim aDBColumn As adonetColumnDescription
            Dim result As Object
            Dim fieldno As Integer

            result = Nothing
            ' check if schema is initialized
            If Not Me.ContainerSchema.IsInitialized Then
                Call CoreMessageHandler(procedure:="adonetViewReader.convert2ColumnData", messagetype:=otCoreMessageType.InternalError, _
                                      message:="tableschema couldnot be initialized - loaded to fail ?", containerID:=Me.ViewID)
                Return False
            End If


            Try

                fieldno = aSchema.GetEntryOrdinal(index)
                If fieldno < 0 Then
                    Call CoreMessageHandler(procedure:="adonetViewReader.cvt2ColumnData", _
                                          message:="iOTDBTableStore " & Me.ViewID & " anIndex for " & index & " not found", _
                                          containerID:=Me.ViewID, argument:=index, messagetype:=otCoreMessageType.InternalError)
                    Return False

                Else
                    aDBColumn = aSchema.GetColumnDescription(fieldno)
                End If
                If Not isnullable.HasValue Then
                    isnullable = aDBColumn.IsNullable
                End If
                If defaultvalue Is Nothing And aDBColumn.HasDefault Then
                    defaultvalue = aDBColumn.Default
                End If

                abostrophNecessary = False

                '***
                '*** convert
                Return Connection.DatabaseDriver.Convert2DBData(invalue:=invalue, outvalue:=outvalue, _
                                                                targetType:=aDBColumn.DataType, _
                                                                maxsize:=aDBColumn.CharacterMaxLength, _
                                                                  abostrophNecessary:=abostrophNecessary,
                                                                  isnullable:=isnullable, defaultvalue:=defaultvalue, _
                                                                  columnname:=aDBColumn.ColumnName)


            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=False, procedure:="adonetViewReader.cvt2ColumnData", messagetype:=otCoreMessageType.InternalError, _
                                      containerID:=Me.ViewID, entryname:=aDBColumn.ColumnName, exception:=ex, argument:=index & ": '" & invalue & "'")
                Return Nothing

            End Try


        End Function

        ''' <summary>
        ''' converts data to object data
        ''' </summary>
        ''' <param name="index"></param>
        ''' <param name="invalue"></param>
        ''' <param name="outvalue"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function Convert2ObjectData(ByVal index As Object, _
                                                        ByVal invalue As Object, _
                                                        ByRef outvalue As Object, _
                                                        Optional isnullable As Boolean? = Nothing, _
                                                        Optional defaultvalue As Object = Nothing, _
                                                        Optional ByRef abostrophNecessary As Boolean = False) As Boolean

        ''' <summary>
        ''' if Cache is Initialized and running 
        ''' </summary>
        ''' <returns>return true</returns>
        ''' <remarks></remarks>
        Public Function IsCacheInitialized() As Boolean
            If _cacheAdapter Is Nothing OrElse _cacheTable Is Nothing Then
                Return False
            Else
                Return True
            End If
        End Function
        ''' <summary>
        ''' Initialize Cache 
        ''' </summary>
        ''' <returns>true if successfull </returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function InitializeCache(Optional ByVal force As Boolean = False) As Boolean

        ''' <summary>
        ''' specific Command
        ''' </summary>
        ''' <param name="commandstr"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function CreateNativeDBCommand(ByVal commandstr As String, ByRef nativeConnection As IDbConnection) As IDbCommand


        ''' <summary>
        ''' GetRecordbyPrimaryKey returns a clsOTDBRecord object by the Primarykey from the Database
        ''' </summary>
        ''' <param name="primaryKeyArray">PrimaryKey Array</param>
        ''' <param name="silent"></param>
        ''' <returns>returns Nothing if not found otherwise a clsOTDBRecord</returns>
        ''' <remarks></remarks>
        Public Overrides Function GetRecordByPrimaryKey(ByRef primaryKeyArray() As Object, Optional silent As Boolean = False) As ormRecord _
        Implements iormRelationalTableStore.GetRecordByPrimaryKey
            'Dim aConnection As IDbConnection
            Dim aSqlSelectCommand As IDbCommand
            Dim j As Integer
            Dim afieldname As String
            Dim aValue As Object
            Dim wherestr As String = Nothing
            Dim abostrophNecessary As Boolean
            Dim aCvtValue As Object
            Dim aDataReader As IDataReader

            If Not IsArray(primaryKeyArray) Then
                Call CoreMessageHandler(procedure:="adonetViewReader.getRecordByPrimaryKey", message:="Empty Key Array")
                WriteLine("uups - no Array as primaryKey")
                Return Nothing
            ElseIf primaryKeyArray.GetUpperBound(0) < (Me.ContainerSchema.NoPrimaryEntries - 1) Then
                Call CoreMessageHandler(procedure:="adonetViewReader.getRecordByPrimaryKey", message:="Size of Primary Key Array less than the number of primary keys", _
                                      argument:=Me.ViewID, messagetype:=otCoreMessageType.InternalError)
                Return Nothing

            End If

            ' Connection
            Try
                If Not Me.Connection.IsConnected AndAlso Not Me.Connection.Session.IsBootstrappingInstallationRequested Then
                    Call CoreMessageHandler(procedure:="adonetViewReader.getRecordsByPrimaryKey", message:="Connection is not available")
                    Return Nothing
                End If

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="adonetViewReader.getRecordsByPrimaryKey", exception:=ex)
                Return Nothing
            End Try

            ''' check if schema is initialized
            ''' 
            If Not Me.ContainerSchema.IsInitialized Then
                Call CoreMessageHandler(procedure:="adonetViewReader.getRecordsByPrimaryKey", messagetype:=otCoreMessageType.InternalError, _
                                      message:="tableschema couldnot be initialized - loaded to fail ?", containerID:=Me.ViewID)
                Return Nothing
            End If

            '*** check on Property Cached
            If Me.HasProperty(ConstTPNCacheProperty) Then
                If Not Me.IsCacheInitialized Then
                    Me.InitializeCache()
                End If
            End If

            '* get PrimaryKeys and their value -> build the criteria
            '*
            aSqlSelectCommand = TryCast(Me.ContainerSchema, adonetTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, adonetTableSchema.CommandType.SelectType)
            If aSqlSelectCommand Is Nothing Then
                Call CoreMessageHandler(procedure:="adonetViewReader.getRecordByPrimaryKey", message:="Select Command is not in Store", _
                                      argument:=Me.ContainerSchema.PrimaryKeyIndexName, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If

            SyncLock aSqlSelectCommand.Connection

                Try

                    For j = 0 To (Me.ContainerSchema.NoPrimaryEntries - 1)

                        ' value of key
                        aValue = primaryKeyArray(j)
                        afieldname = Me.ContainerSchema.GetPrimaryEntryNames(j + 1)
                        If Not String.IsNullOrEmpty(afieldname) Then
                            If j <> 0 Then
                                wherestr &= String.Format(" AND [{0}]", afieldname)
                            Else
                                wherestr &= String.Format(" [{0}]", afieldname)
                            End If
                            If Convert2ContainerData(afieldname, invalue:=aValue, outvalue:=aCvtValue, abostrophNecessary:=abostrophNecessary) Then
                                ' build parameter
                                aSqlSelectCommand.Parameters(j).Value = aCvtValue
                                ' and build wherestring for cache
                                If abostrophNecessary Then
                                    wherestr &= " = '" & aCvtValue.ToString & "'"
                                Else
                                    wherestr &= " = " & aCvtValue.ToString
                                End If
                            Else
                                Call CoreMessageHandler(procedure:="adonetViewReader.getRecordByPrimaryKey", message:="Value for primary key couldnot be converted to ColumnData", _
                                                      argument:=aValue, messagetype:=otCoreMessageType.InternalError, entryname:=afieldname, containerID:=Me.ViewID)
                                Return Nothing
                            End If

                        End If

                    Next j

                Catch ex As Exception
                    Call CoreMessageHandler(procedure:="adonetViewReader.getRecordByPrimaryKey", message:="Exception", exception:=ex)
                    Return Nothing
                End Try


                '**** read
                Try
                    '*** check on Property Cached
                    If Me.HasProperty(ConstTPNCacheProperty) AndAlso Me.IsCacheInitialized Then
                        Dim dataRows() As DataRow = _cacheTable.Select(wherestr)

                        ' not found
                        If dataRows.GetLength(0) = 0 Then
                            Return Nothing
                        Else
                            '** Factory a new clsOTDBRecord
                            '**
                            Dim aNewRecord As New ormRecord(containerID:=Me.ViewID, dbdriver:=Me.Connection.DatabaseDriver, runtimeOnly:=False)
                            If aNewRecord.LoadFrom(dataRows(0)) Then
                                Return aNewRecord
                            Else
                                Return Nothing
                            End If
                            'If InfuseRecord(record:=aNewEnt, dataobject:=dataRows(0), CreateNewrecord:=True) Then
                            '    Return aNewEnt
                            'Else

                            '    Return Nothing
                            'End If
                        End If
                    Else
                        ''' run the datareader
                        ''' 
                        aDataReader = aSqlSelectCommand.ExecuteReader
                        If aDataReader.Read Then
                            '** Factory a new clsOTDBRecord
                            '**
                            Dim aNewRecord As New ormRecord(containerID:=Me.ViewID, dbdriver:=Me.Connection.DatabaseDriver, runtimeOnly:=False)
                            If aNewRecord.LoadFrom(aDataReader, InSync:=True) Then
                                aDataReader.Close()
                                Return aNewRecord
                            Else
                                aDataReader.Close()
                                Return Nothing
                            End If
                            'Dim aNewEnt As New ormRecord
                            'If InfuseRecord(aNewEnt, aDataReader, CreateNewrecord:=True) Then
                            '    aDataReader.Close()
                            '    Return aNewEnt
                            'Else
                            '    aDataReader.Close()
                            '    Return Nothing
                            'End If
                        Else
                            aDataReader.Close()
                            Return Nothing
                        End If


                    End If


                Catch ex As Exception
                    Call CoreMessageHandler(showmsgbox:=silent, procedure:="adonetViewReader.getRecordByPrimaryKey", _
                                          containerID:=Me.ViewID, argument:=primaryKeyArray, exception:=ex)
                    If aDataReader IsNot Nothing Then aDataReader.Close()
                    Return Nothing
                End Try

            End SyncLock

        End Function

        '****** getRecords by Index
        '******
        Public Overrides Function GetRecordsByIndex(indexname As String, ByRef keyArray() As Object, Optional silent As Boolean = False) As List(Of ormRecord) _
        Implements iormRelationalTableStore.GetRecordsByIndex
            Dim aSqlSelectCommand As IDbCommand
            Dim j As Integer
            Dim fieldname As String
            Dim aValue As Object
            Dim anIndexColumnList As ArrayList
            Dim abostrophNecessary As Boolean
            Dim aCvtValue As Object
            Dim wherestr As String = Nothing
            Dim aNewRecord As ormRecord
            Dim aCollection As New List(Of ormRecord)
            Dim aDataReader As IDataReader

            ' check if schema is initialized
            If Not Me.ContainerSchema.IsInitialized Then
                Call CoreMessageHandler(procedure:="adonetTableStore.DelRecordsByPrimaryKey", messagetype:=otCoreMessageType.InternalError, _
                                      message:="tableschema couldnot be initialized - loaded to fail ?", containerID:=Me.ViewID)
                Return Nothing
            End If

            '* get Index and their value -> build the criteria
            '*
            If Me.ContainerSchema.HasIndex(indexname) Then

                anIndexColumnList = Me.ContainerSchema.GetIndex(indexname)
            ElseIf Me.ContainerSchema.HasIndex(String.Format("{0}_{1}", Me.ViewID, indexname)) Then
                indexname = String.Format("{0}_{1}", Me.ViewID, indexname)
                anIndexColumnList = Me.ContainerSchema.GetIndex(indexname)
            Else
                Call CoreMessageHandler(procedure:="clsADOStore.getRecordsByIndex", argument:=indexname, _
                                      message:="Index does not exists for Table " & Me.ViewID, messagetype:=otCoreMessageType.InternalError, _
                                      containerID:=Me.ViewID)
                Return Nothing
            End If

            If Not IsArray(keyArray) Then
                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByIndex", message:="Empty Key Array", _
                                      messagetype:=otCoreMessageType.InternalError, _
                                      containerID:=Me.ViewID)
                WriteLine("uups - no Array as primaryKey")
                Return Nothing
            ElseIf keyArray.GetUpperBound(0) > (anIndexColumnList.Count - 1) Then
                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByIndex", message:="Size of Primary Key Array less than the number of primary keys", _
                                      argument:=Me.ViewID, messagetype:=otCoreMessageType.InternalError)
                Return Nothing

            End If

            ' Connection
            Try
                If Not Me.Connection.IsConnected AndAlso Not Me.Connection.Session.IsBootstrappingInstallationRequested Then
                    Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByIndex", message:="Connection is not available")
                    Return Nothing
                End If

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByIndex", exception:=ex)
                Return Nothing
            End Try

            '*** check on Property Cached
            If Me.HasProperty(ConstTPNCacheProperty) Then
                If Not Me.IsCacheInitialized Then
                    Me.InitializeCache()
                End If
            End If

            '* get PrimaryKeys and their value -> build the criteria
            '*
            aSqlSelectCommand = TryCast(Me.ContainerSchema, adonetTableSchema).GetCommand(indexname, adonetTableSchema.CommandType.SelectType)
            If aSqlSelectCommand Is Nothing Then
                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByIndex", message:="Select Command is not in Store", _
                                      argument:=Me.ContainerSchema.PrimaryKeyIndexName, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If
            SyncLock aSqlSelectCommand.Connection

                Try

                    For j = 0 To (anIndexColumnList.Count - 1)

                        ' reflect part keys
                        If j <= keyArray.GetUpperBound(0) Then

                            ''' build the statement out of it
                            aValue = keyArray(j)
                            fieldname = anIndexColumnList.Item(j)
                            If String.IsNullOrEmpty(fieldname) Then
                                If j <> 0 Then
                                    wherestr &= String.Format(" AND [{0}]", fieldname)
                                Else
                                    wherestr &= "[" & fieldname & "]"
                                End If
                                If Me.Convert2ContainerData(fieldname, invalue:=aValue, outvalue:=aCvtValue, abostrophNecessary:=abostrophNecessary) Then
                                    ' set parameter
                                    aSqlSelectCommand.Parameters(j).Value = aCvtValue
                                    ' and build wherestring for cache
                                    If abostrophNecessary Then
                                        wherestr &= " = '" & aCvtValue.ToString & "'"
                                    Else
                                        wherestr &= " = " & aCvtValue.ToString
                                    End If
                                Else
                                    Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByIndex", message:="Value for primary key couldnot be converted to ColumnData", _
                                                          argument:=aValue, messagetype:=otCoreMessageType.InternalError, entryname:=fieldname, containerID:=Me.ViewID)
                                    Return Nothing

                                End If

                            End If
                        End If

                    Next j

                Catch ex As Exception
                    Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByIndex", message:="Exception", exception:=ex)
                    Return New List(Of ormRecord)
                End Try

                ''' read section
                ''' 
                Try
                    ''' try to read on the cache table if we have it
                    ''' 
                    If Me.HasProperty(ConstTPNCacheProperty) AndAlso Me.IsCacheInitialized Then
                        Dim dataRows() As DataRow
                        If _cacheViews.ContainsKey(key:=indexname) Then
                            Dim aDataView = _cacheViews.Item(key:=indexname)

                            dataRows = aDataView.Table.Select(wherestr)
                        Else
                            dataRows = _cacheTable.Select(wherestr)
                        End If

                        ' not found
                        If dataRows.GetLength(0) = 0 Then
                            Return aCollection
                        Else
                            For Each row In dataRows
                                aNewRecord = New ormRecord(containerID:=Me.ViewID, dbdriver:=Me.Connection.DatabaseDriver, runtimeOnly:=False)
                                If aNewRecord.LoadFrom(row) Then
                                    aCollection.Add(item:=aNewRecord)
                                Else
                                    Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsBySQL", message:="couldnot infuse a record", _
                                                         argument:=aNewRecord, containerID:=Me.ViewID, break:=False)
                                End If

                            Next
                        End If
                    Else
                        ''' read from the data reader
                        ''' 
                        aDataReader = aSqlSelectCommand.ExecuteReader

                        Do While aDataReader.Read
                            aNewRecord = New ormRecord(containerID:=Me.ViewID, dbdriver:=Me.Connection.DatabaseDriver, runtimeOnly:=False)
                            If aNewRecord.LoadFrom(aDataReader, InSync:=True) Then
                                aCollection.Add(item:=aNewRecord)
                            Else
                                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsBySQL", message:="couldnot infuse a record", _
                                                     argument:=aNewRecord, containerID:=Me.ViewID, break:=False)
                            End If
                            ''** Factory a new clsOTDBRecord
                            'aNewRecord = New ormRecord
                            'If InfuseRecord(aNewRecord, aDataReader, CreateNewrecord:=True) Then
                            '    aCollection.Add(item:=aNewRecord)
                            'Else
                            '    Call CoreMessageHandler(subname:="adonetTableStore.getRecordsBySQL", message:="couldnot infuse a record", _
                            '                          arg1:=aNewRecord, tablename:=Me.TableID, break:=False)
                            'End If

                        Loop

                        aDataReader.Close()

                    End If

                    Return aCollection
                    '*****
                    '***** Error Handling
                    '*****
                Catch ex As Exception
                    Call CoreMessageHandler(showmsgbox:=silent, procedure:="adonetTableStore.getRecordsByIndex", _
                                          containerID:=Me.ViewID, argument:=keyArray, exception:=ex)
                    If aDataReader IsNot Nothing Then aDataReader.Close()

                    Return New List(Of ormRecord)
                End Try

            End SyncLock

        End Function

        ''' <summary>
        ''' Update a Datatable with the adapter
        ''' </summary>
        ''' <param name="datatable"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function UpdateDBDataTable(ByRef dataadapter As IDbDataAdapter, ByRef datatable As DataTable) As Integer

        '****** runs a SQLCommand
        '******
        Public Overrides Function RunSqlStatement(ByVal sqlcmdstr As String, Optional ByRef parameters As List(Of ormSqlCommandParameter) = Nothing, Optional silent As Boolean = True) As Boolean _
        Implements iormRelationalTableStore.RunSqlStatement

            Return Me.Connection.DatabaseDriver.RunSqlStatement(sqlcmdstr:=sqlcmdstr, parameters:=parameters, silent:=silent)

        End Function
        '****** returns the Collection of Records by SQL
        '******
        Public Overrides Function GetRecordsBySql(ByVal wherestr As String, _
                                    Optional ByVal fullsqlstr As String = Nothing, _
                                    Optional ByVal innerjoin As String = Nothing, _
                                    Optional ByVal orderby As String = Nothing, _
                                    Optional ByVal silent As Boolean = False, _
                                    Optional ByRef parameters As List(Of ormSqlCommandParameter) = Nothing) As List(Of ormRecord) Implements iormRelationalTableStore.GetRecordsBySql

            Dim aConnection As IDbConnection
            Dim i As Integer
            Dim cmdstr As String
            Dim aCollection As New List(Of ormRecord)
            Dim aNewRecord As ormRecord
            Dim fieldstr As String

            ' Connection
            Try
                If Me.Connection.IsConnected OrElse Me.Connection.Session.IsBootstrappingInstallationRequested Then
                    aConnection = DirectCast(Me.Connection.NativeConnection, IDbConnection)
                    If aConnection Is Nothing And Me.Connection.Session.IsBootstrappingInstallationRequested Then
                        aConnection = DirectCast(DirectCast(Me.Connection, adonetConnection).NativeInternalConnection, IDbConnection)
                    Else
                        CoreMessageHandler(message:="No Internal connection available", procedure:="adnoetTablestore.getrecordsbysql", _
                                            messagetype:=otCoreMessageType.InternalError)
                    End If
                Else
                    Call CoreMessageHandler(procedure:="adonetViewReader.getRecordsBySQL", message:="Connection is not available", messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="adonetViewReader.getRecordsBySQL", exception:=ex)
                Return Nothing
            End Try

            ' check if schema is initialized
            If Not Me.ContainerSchema.IsInitialized Then
                Call CoreMessageHandler(procedure:="adonetViewReader.getRecordBySQL", messagetype:=otCoreMessageType.InternalError, _
                                      message:="tableschema couldnot be initialized - loaded to fail ?", containerID:=Me.ViewID)
                Return Nothing
            End If

            '*** check on Property Cached
            If Me.HasProperty(ConstTPNCacheProperty) Then
                If Not Me.IsCacheInitialized Then
                    Me.InitializeCache()
                End If
            End If


            If String.IsNullOrWhiteSpace(fullsqlstr) Then cmdstr = fullsqlstr

            i = 0
            fieldstr = String.Empty
            For Each field As String In Me.ContainerSchema.EntryNames
                If i = 0 Then
                    fieldstr = "[" & Me.NativeDBObjectname & "].[" & field & "]"
                    i += 1
                Else
                    fieldstr &= " , [" & Me.NativeDBObjectname & "].[" & field & "]"
                End If
            Next

            ' Select
            If String.IsNullOrWhiteSpace(innerjoin) Then
                cmdstr = String.Format("SELECT * FROM [{0}] WHERE {1}", Me.NativeDBObjectname, wherestr)
            Else
                cmdstr = "SELECT " & fieldstr & " FROM [" & Me.NativeDBObjectname & "] " & innerjoin & " WHERE " & wherestr
            End If

            If Not String.IsNullOrWhiteSpace(orderby) Then cmdstr = cmdstr & " ORDER BY " & orderby

            Try
                '*** check on Property Cached
                If Me.HasProperty(ConstTPNCacheProperty) AndAlso Me.IsCacheInitialized Then
                    Dim dataRows() As DataRow = _cacheTable.Select(wherestr)

                    ' not found
                    If dataRows.GetLength(0) = 0 Then
                        Return aCollection
                    Else
                        For Each row In dataRows
                            ''' infuse the records
                            aNewRecord = New ormRecord(containerID:=Me.ViewID, dbdriver:=Me.Connection.DatabaseDriver)
                            If aNewRecord.LoadFrom(row) Then
                                aCollection.Add(item:=aNewRecord)
                            Else
                                Call CoreMessageHandler(procedure:="adonetViewReader.getRecordsBySQL", message:="couldnot infuse a record", _
                                                      argument:=aNewRecord, containerID:=Me.ViewID, break:=False)
                            End If
                            'If InfuseRecord(aNewRecord, row, CreateNewrecord:=True) Then
                            '    aCollection.Add(item:=aNewRecord)
                            'Else
                            '    Call CoreMessageHandler(subname:="adonetViewReader.getRecordsBySQL", message:="couldnot infuse a record", _
                            '                          arg1:=aNewRecord, tablename:=Me.TableID, break:=False)
                            'End If
                        Next
                    End If
                Else
                    Dim aSqlCommand As IDbCommand = CreateNativeDBCommand(cmdstr, aConnection)
                    Dim aDataReader As IDataReader
                    SyncLock aSqlCommand.Connection
                        ' read
                        aDataReader = aSqlCommand.ExecuteReader
                        Do While aDataReader.Read
                            ''' infuse the records
                            aNewRecord = New ormRecord(containerID:=Me.ViewID, dbdriver:=Me.Connection.DatabaseDriver)
                            If aNewRecord.LoadFrom(aDataReader) Then
                                aCollection.Add(item:=aNewRecord)
                            Else
                                Call CoreMessageHandler(procedure:="adonetViewReader.getRecordsBySQL", message:="couldnot infuse a record", _
                                                      argument:=aNewRecord, containerID:=Me.ViewID, break:=False)
                            End If
                            ''** Factory a new clsOTDBRecord
                            'aNewRecord = New ormRecord
                            'If InfuseRecord(aNewRecord, aDataReader, CreateNewrecord:=True) Then
                            '    aCollection.Add(item:=aNewRecord)
                            'Else
                            '    Call CoreMessageHandler(subname:="adonetViewReader.getRecordsBySQL", message:="couldnot infuse a record", _
                            '                          arg1:=aNewRecord, tablename:=Me.TableID, break:=False)
                            'End If

                        Loop

                        ' close
                        aDataReader.Close()

                    End SyncLock
                End If



                ' return
                If aCollection.Count > 0 Then
                    GetRecordsBySql = aCollection
                Else
                    GetRecordsBySql = Nothing
                End If

                Exit Function

                '******** error handling
            Catch ex As Exception

                Call CoreMessageHandler(showmsgbox:=silent, procedure:="adonetViewReader.getRecordsBySQL", containerID:=Me.ViewID, _
                                      argument:="Where :" & wherestr & " inner join: " & innerjoin & " full: " & fullsqlstr, _
                                      exception:=ex)

                Return New List(Of ormRecord)
            End Try



        End Function
        ''' <summary>
        ''' returns a collection of records selected by this helper command which creates an SqlCommand with an ID or reuse one
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
        Public Overrides Function GetRecordsBySqlCommand(ByVal ID As String, _
                                    Optional ByVal wherestr As String = Nothing, _
                                    Optional ByVal fullsqlstr As String = Nothing, _
                                    Optional ByVal innerjoin As String = Nothing, _
                                    Optional ByVal orderby As String = Nothing, _
                                    Optional ByVal silent As Boolean = False, _
                                    Optional ByRef parameters As List(Of ormSqlCommandParameter) = Nothing) As List(Of ormRecord) _
                                Implements iormRelationalTableStore.GetRecordsBySqlCommand


            Dim aCollection As New List(Of ormRecord)
            Dim aParameterValues As New Dictionary(Of String, Object)
            Dim aCommand As ormSqlSelectCommand

            '*** check on Property Cached
            If Me.HasProperty(ConstTPNCacheProperty) Then
                If Not Me.IsCacheInitialized Then
                    Me.InitializeCache()
                End If
            End If

            ' check if schema is initialized
            If Not Me.ContainerSchema.IsInitialized Then
                Call CoreMessageHandler(procedure:="adonetViewReader.GetRecordBySQLCommand", messagetype:=otCoreMessageType.InternalError, _
                                      message:="table schema could not be initialized - loaded to fail ?", containerID:=Me.ViewID)
                Return Nothing
            End If

            Try
                ' get
                aCommand = Me.CreateSqlSelectCommand(ID)
                SyncLock aCommand
                    If Not aCommand.IsPrepared Then
                        aCommand.AddTable(Me.ViewID, addAllFields:=True)
                        aCommand.Where = wherestr
                        aCommand.InnerJoin = innerjoin
                        aCommand.OrderBy = orderby
                        'If fullsqlstr <> String.empty then aCommand.SqlText = fullsqlstr 
                        If parameters IsNot Nothing Then
                            For Each aParameter In parameters
                                aCommand.AddParameter(aParameter)
                                aParameterValues.Add(aParameter.ID, aParameter.Value)
                            Next
                        End If

                        If Not aCommand.Prepare Then
                            Call CoreMessageHandler(message:="couldnot prepare command", procedure:="adonetViewReader.getRecordsBySQLCommand", _
                                                   messagetype:=otCoreMessageType.InternalError, argument:=aCommand.SqlText)
                            Return New List(Of ormRecord)
                        End If
                    End If


                    '*** check on Property Cached
                    '***
                    If Me.HasProperty(ConstTPNCacheProperty) AndAlso Me.IsCacheInitialized Then
                        Dim aDataview = _cacheTable.AsDataView
                        If Not String.IsNullOrWhiteSpace(orderby) Then aDataview.Sort = aCommand.OrderBy

                        If Not String.IsNullOrWhiteSpace(aCommand.Where) Then
                            Dim wherestatement As String = aCommand.Where
                            wherestatement = wherestatement.Replace("[", " ").Replace("]", " ")
                            If wherestatement.Contains(".") Then
                                '** strip off all the table namings
                                wherestatement = Regex.Replace(wherestatement, "\S*\.", String.Empty)
                            End If
                            '** replace the values
                            If aCommand.Parameters IsNot Nothing Then
                                For Each aParameter In aCommand.Parameters
                                    If aParameter.Datatype <> otDataType.Memo And aParameter.Datatype <> otDataType.Text And aParameter.Datatype <> otDataType.List Then
                                        wherestatement = wherestatement.Replace(aParameter.ID, aParameter.Value)
                                    Else
                                        wherestatement = wherestatement.Replace(aParameter.ID, "'" & aParameter.Value & "'")
                                    End If
                                Next
                            End If

                            aDataview.RowFilter = wherestatement
                        End If

                        Dim dataRows() As DataRow = aDataview.ToTable.Select()

                        ' not found
                        If dataRows.GetLength(0) = 0 Then
                            Return aCollection
                        Else
                            For Each row In dataRows
                                ''' infuse the records
                                Dim aNewRecord As New ormRecord(containerID:=Me.ViewID, dbdriver:=Me.Connection.DatabaseDriver)
                                If aNewRecord.LoadFrom(row) Then
                                    aCollection.Add(item:=aNewRecord)
                                Else
                                    Call CoreMessageHandler(procedure:="adonetViewReader.getRecordsBySQLCommand", message:="couldnot infuse a record", _
                                                          argument:=aNewRecord, containerID:=Me.ViewID, break:=False)
                                End If
                                'Dim aNewEnt = New ormRecord
                                'If InfuseRecord(aNewEnt, row, CreateNewrecord:=True) Then
                                '    aCollection.Add(item:=aNewEnt)
                                'Else
                                '    Call CoreMessageHandler(subname:="adonetViewReader.getRecordsBySQLCommand", message:="couldnot infuse a record", _
                                '                          arg1:=aNewEnt, tablename:=Me.TableID, break:=False)
                                'End If
                            Next
                        End If

                        Return aCollection
                    Else
                        ' replace parametervalues out of the paramter -> might be different to the prepared one
                        If parameters IsNot Nothing Then
                            For Each aParameter In parameters
                                If Not aParameterValues.ContainsKey(key:=aParameter.ID) Then
                                    aParameterValues.Add(aParameter.ID, aParameter.Value)
                                End If
                            Next
                        End If
                        '** NOCACHE
                        '** run the Command
                        Dim theRecords As List(Of ormRecord) = _
                            Me.Connection.DatabaseDriver.RunSqlSelectCommand(aCommand, parametervalues:=aParameterValues)

                        Return theRecords
                    End If
                End SyncLock
                '******** error handling
            Catch ex As Exception

                Call CoreMessageHandler(showmsgbox:=silent, procedure:="adonetViewReader.getRecordsBySQLCommand", containerID:=Me.ViewID, _
                                      argument:="Where :" & wherestr & " inner join: " & innerjoin & " full: " & fullsqlstr, _
                                      exception:=ex)

                Return New List(Of ormRecord)
            End Try



        End Function
        ''' <summary>
        ''' return a collection of records for a sqlcommand on a table
        ''' </summary>
        ''' <param name="sqlcommand"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Overrides Function GetRecordsBySqlCommand(ByRef sqlcommand As ormSqlSelectCommand, _
                                                         Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing) As List(Of ormRecord) _
                                                        Implements iormRelationalTableStore.GetRecordsBySqlCommand

            Return Me.Connection.DatabaseDriver.RunSqlSelectCommand(sqlcommand:=sqlcommand, parametervalues:=parametervalues)
        End Function
        ''' <summary>
        ''' infuse a Record with the Help of the Datareader Object
        ''' </summary>
        ''' <param name="record">clsOTDBRecord</param>
        ''' <param name="DataReader">an open Datareader which has just the data</param>
        ''' <param name="silent">no messages</param>
        ''' <returns>True if successfull and read</returns>
        ''' <remarks></remarks>
        Public Overrides Function InfuseRecord(ByRef record As ormRecord, ByRef dataobject As Object, _
        Optional ByVal silent As Boolean = False, Optional CreateNewrecord As Boolean = False) As Boolean _
        Implements iormRelationalTableStore.InfuseRecord
            Dim aDBColumn As adonetColumnDescription
            Dim cvtvalue, Value As Object
            Dim j As Integer
            Dim abostrophNecessary As Boolean
            Dim ordinal As Nullable(Of Integer)
            Dim aDatareader As IDataReader = Nothing
            Dim aRow As DataRow = Nothing

            ' check if schema is initialized
            If Not Me.ContainerSchema.IsInitialized Then
                Call CoreMessageHandler(procedure:="adonetViewReader.InfuseRecord", messagetype:=otCoreMessageType.InternalError, _
                                      message:="tableschema couldnot be initialized - loaded to fail ?", containerID:=Me.ViewID)
                Return Nothing
            End If

            Try
                If GetType(IDataReader).IsAssignableFrom(dataobject.GetType) AndAlso Not dataobject.GetType.IsAbstract Then
                    aDatareader = DirectCast(dataobject, IDataReader)

                ElseIf dataobject.GetType() = GetType(DataRow) Then
                    aRow = DirectCast(dataobject, DataRow)
                Else
                    Call CoreMessageHandler(procedure:="adonetViewReader.infuseRecord", message:="Data object has no known type", _
                                          argument:=dataobject.GetType.ToString)
                    Return False

                End If
            Catch ex As Exception
                Call CoreMessageHandler(procedure:="adonetViewReader.infuseRecord", exception:=ex, message:="Exception", _
                                      argument:=dataobject.GetType.ToString)
                Return False
            End Try
            Try

                '** Factory a new clsOTDBRecord
                '**
                ''' if record is not supplied take a bound record
                If record Is Nothing OrElse CreateNewrecord Then record = New ormRecord(containerID:=Me.ViewID, dbdriver:=Me.Connection.DatabaseDriver)
                record.IsLoaded = True ' definitely loaded ! not created

                For j = 1 To Me.ViewSchema.NoEntries
                    ' get fields
                    aDBColumn = DirectCast(Me.ContainerSchema, adonetTableSchema).GetColumnDescription(j)
                    If aDBColumn IsNot Nothing Then
                        Try
                            If Not aDatareader Is Nothing Then
                                ordinal = aDatareader.GetOrdinal(aDBColumn.ColumnName)
                            End If
                        Catch ex As System.IndexOutOfRangeException
                            Try
                                ordinal = aDatareader.GetOrdinal(String.Format("{0}.{1}", Me.ViewID, aDBColumn.ColumnName))
                            Catch ex2 As Exception
                                Call CoreMessageHandler(exception:=ex2, message:="Exception", procedure:="adonetViewReader.infuseRecord", _
                                                      argument:=Me.ViewID, messagetype:=otCoreMessageType.InternalError)
                            Finally
                                ordinal = Nothing
                            End Try
                        End Try

                        If aDatareader IsNot Nothing Then
                            If ordinal IsNot Nothing AndAlso ordinal >= 0 Then
                                Value = aDatareader.GetValue(ordinal)
                                If Me.Convert2ObjectData(j, invalue:=Value, outvalue:=cvtvalue, abostrophNecessary:=abostrophNecessary) Then
                                    Call record.SetValue(j, cvtvalue)
                                Else
                                    Call CoreMessageHandler(procedure:="adonetViewReader.infuseRecord", message:="could not convert db value", argument:=Value, _
                                                      containerEntryName:=aDBColumn.ColumnName, containerID:=Me.ViewID, messagetype:=otCoreMessageType.InternalError)
                                End If
                            Else
                                Call CoreMessageHandler(procedure:="adonetViewReader.infuseRecord", message:="ordinal missing - Field not in DataReader", _
                                                      entryname:=aDBColumn.ColumnName, containerID:=Me.ViewID, messagetype:=otCoreMessageType.InternalError)
                            End If
                        Else
                            '** aRow
                            Value = aRow.Item(j - 1)
                            If Me.Convert2ObjectData(j, invalue:=Value, outvalue:=cvtvalue, abostrophNecessary:=abostrophNecessary) Then
                                Call record.SetValue(j, cvtvalue)
                            Else
                                Call CoreMessageHandler(procedure:="adonetViewReader.infuseRecord", message:="could not convert db value", argument:=Value, _
                                                  containerEntryName:=aDBColumn.ColumnName, containerID:=Me.ViewID, messagetype:=otCoreMessageType.InternalError)
                            End If

                        End If
                    Else
                        Call CoreMessageHandler(procedure:="adonetViewReader.infuseRecord", message:="DBColumn missing - Field not in DataReader", _
                                              argument:=j, containerID:=Me.ViewID, messagetype:=otCoreMessageType.InternalError)
                    End If
                Next j

                Return True

            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="adonetViewReader.infuseRecord")
                Return False
            End Try
        End Function

    End Class

    ''' <summary>
    '''  describes an abstract table store on the ado.net level
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class adonetTableStore
        Inherits ormTableStore
        Implements iormRelationalTableStore


        Protected Friend _cacheTable As DataTable  ' DataTable to cache it
        Protected Friend _cacheViews As New Dictionary(Of String, DataView) ' Dictionary for Dataview per Index
        Protected Friend _cacheAdapter As Data.IDbDataAdapter

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <param name="TableID"></param>
        ''' <param name="forceSchemaReload"></param>
        ''' <remarks></remarks>

        Public Sub New(connection As iormConnection, tableid As String, ByVal forceSchemaReload As Boolean)
            Call MyBase.New(Connection:=connection, tableID:=tableid, force:=forceSchemaReload)
        End Sub

        ''' <summary>
        ''' gets the current cache Table
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CacheDataTable As DataTable
            Get
                Return _cacheTable
            End Get
        End Property
        ''' <summary>
        ''' gets an enumerable of the cached views (indices)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CacheDataViews As IEnumerable(Of DataView)
            Get
                Return _cacheViews
            End Get
        End Property
        ''' converts a Object from OTDB VB.NET Data to ColumnData in the Database
        ''' </summary>
        ''' <param name="index"></param>
        ''' <param name="value"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <returns>the converted object</returns>
        ''' <remarks></remarks>
        Public Overloads Function Convert2ContainerData(ByVal index As Object, _
                                                     ByVal invalue As Object, _
                                                     ByRef outvalue As Object, _
                                                    Optional ByRef abostrophNecessary As Boolean = False, _
                                                    Optional isnullable As Boolean? = Nothing, _
                                                    Optional defaultvalue As Object = Nothing) As Boolean Implements iormRelationalTableStore.Convert2ContainerData
            Dim aSchema As adonetTableSchema = Me.ContainerSchema
            Dim aDBColumn As adonetColumnDescription
            Dim result As Object
            Dim fieldno As Integer

            result = Nothing
            ' check if schema is initialized
            If Not Me.ContainerSchema.IsInitialized Then
                Call CoreMessageHandler(procedure:="adonetTableStore.convert2ColumnData", messagetype:=otCoreMessageType.InternalError, _
                                      message:="tableschema couldnot be initialized - loaded to fail ?", containerID:=Me.TableID)
                Return False
            End If


            Try

                fieldno = aSchema.GetEntryOrdinal(index)
                If fieldno < 0 Then
                    Call CoreMessageHandler(procedure:="adonetTableStore.cvt2ColumnData", _
                                          message:="iOTDBTableStore " & Me.TableID & " anIndex for " & index & " not found", _
                                          containerID:=Me.TableID, argument:=index, messagetype:=otCoreMessageType.InternalError)
                    Return False

                Else
                    aDBColumn = aSchema.GetColumnDescription(fieldno)
                End If
                If Not isnullable.HasValue Then
                    isnullable = aDBColumn.IsNullable
                End If
                If defaultvalue Is Nothing And aDBColumn.HasDefault Then
                    defaultvalue = aDBColumn.Default
                End If

                abostrophNecessary = False

                '***
                '*** convert
                Return Connection.DatabaseDriver.Convert2DBData(invalue:=invalue, outvalue:=outvalue, _
                                                                targetType:=aDBColumn.DataType, _
                                                                maxsize:=aDBColumn.CharacterMaxLength, _
                                                                  abostrophNecessary:=abostrophNecessary,
                                                                  isnullable:=isnullable, defaultvalue:=defaultvalue, _
                                                                  columnname:=aDBColumn.ColumnName)


            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=False, procedure:="adonetTableStore.cvt2ColumnData", messagetype:=otCoreMessageType.InternalError, _
                                      containerID:=Me.TableID, entryname:=aDBColumn.ColumnName, exception:=ex, argument:=index & ": '" & invalue & "'")
                Return Nothing

            End Try


        End Function

        ''' <summary>
        ''' converts data to object data
        ''' </summary>
        ''' <param name="index"></param>
        ''' <param name="invalue"></param>
        ''' <param name="outvalue"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function Convert2ObjectData(ByVal index As Object, _
                                                        ByVal invalue As Object, _
                                                        ByRef outvalue As Object, _
                                                        Optional isnullable As Boolean? = Nothing, _
                                                        Optional defaultvalue As Object = Nothing, _
                                                        Optional ByRef abostrophNecessary As Boolean = False) As Boolean

        ''' <summary>
        ''' if Cache is Initialized and running 
        ''' </summary>
        ''' <returns>return true</returns>
        ''' <remarks></remarks>
        Public Function IsCacheInitialized() As Boolean
            If _cacheAdapter Is Nothing OrElse _cacheTable Is Nothing Then
                Return False
            Else
                Return True
            End If
        End Function
        ''' <summary>
        ''' Initialize Cache 
        ''' </summary>
        ''' <returns>true if successfull </returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function InitializeCache(Optional ByVal force As Boolean = False) As Boolean

        ''' <summary>
        ''' specific Command
        ''' </summary>
        ''' <param name="commandstr"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function CreateNativeDBCommand(ByVal commandstr As String, ByRef nativeConnection As IDbConnection) As IDbCommand

        ''' <summary>
        ''' deletes a Record in the database by Primary key
        ''' </summary>
        ''' <param name="primaryKeyArray">Array of Objects as Primary Key</param>
        ''' <param name="silent"></param>
        ''' <returns>true if successfull </returns>
        ''' <remarks></remarks>
        Public Overrides Function DeleteRecordByPrimaryKey(ByRef primaryKeyArray() As Object, Optional silent As Boolean = False) As Boolean _
        Implements iormRelationalTableStore.DeleteRecordByPrimaryKey
            Dim aSQLDeleteCommand As IDbCommand

            Dim j As Integer
            Dim fieldname As String = Nothing
            Dim aValue As Object
            Dim wherestr As String = Nothing
            Dim abostrophNecessary As Boolean
            Dim acvtvalue As Object

            ' check if schema is initialized
            If Not Me.ContainerSchema.IsInitialized Then
                Call CoreMessageHandler(procedure:="adonetTableStore.DelRecordByPrimaryKey", messagetype:=otCoreMessageType.InternalError, _
                                      message:="tableschema couldnot be initialized - loaded to fail ?", containerID:=Me.TableID)
                Return False
            End If


            If Not IsArray(primaryKeyArray) Then
                Call CoreMessageHandler(procedure:="adonetTableStore.delRecordByPrimaryKey", message:="Empty Key Array")
                WriteLine("uups - no Array as primaryKey")
                Return False
            ElseIf primaryKeyArray.GetUpperBound(0) > (Me.ContainerSchema.NoPrimaryEntries - 1) Then
                Call CoreMessageHandler(procedure:="adonetTableStore.delRecordByPrimaryKey", message:="Size of Primary Key Array less than the number of primary keys", _
                                      argument:=Me.TableID, messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            ' Connection
            Try
                If Not Me.Connection.IsConnected AndAlso Not Me.Connection.Session.IsBootstrappingInstallationRequested Then
                    Call CoreMessageHandler(procedure:="adonetTableStore.delRecordByPrimaryKey", message:="Connection is not available", messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="adonetTableStore.delRecordByPrimaryKey", exception:=ex)
                Return False
            End Try

            '*** check on Property Cached
            If Me.HasProperty(ConstTPNCacheProperty) Then
                If Not Me.IsCacheInitialized Then
                    Me.InitializeCache()
                End If
            End If

            '* get PrimaryKeys and their value -> build the criteria
            '*
            aSQLDeleteCommand = TryCast(Me.ContainerSchema, adonetTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, _
                                                                                               adonetTableSchema.CommandType.DeleteType)
            If aSQLDeleteCommand Is Nothing Then
                Call CoreMessageHandler(procedure:="adonetTableStore.delRecordByPrimaryKey", message:="DeleteCommand is not in Store", _
                                      argument:=Me.ContainerSchema.PrimaryKeyIndexName, messagetype:=otCoreMessageType.InternalError)
                Return False
            End If
            SyncLock aSQLDeleteCommand.Connection

                Try


                    For j = 0 To (Me.ContainerSchema.NoPrimaryEntries - 1)

                        ' value of key
                        aValue = primaryKeyArray(j)
                        fieldname = Me.ContainerSchema.GetPrimaryEntryNames(j + 1)
                        If j <> 0 Then
                            wherestr &= String.Format(" AND [{0}]", fieldname)
                        Else
                            wherestr &= String.Format(" [{0}]", fieldname)
                        End If
                        If Not String.IsNullOrEmpty(fieldname) Then
                            If Me.Convert2ContainerData(fieldname, invalue:=aValue, outvalue:=acvtvalue, abostrophNecessary:=abostrophNecessary) Then
                                aSQLDeleteCommand.Parameters(j).Value = acvtvalue
                                If abostrophNecessary Then
                                    wherestr &= " = '" & acvtvalue.ToString & "'"
                                Else
                                    wherestr &= " = " & acvtvalue.ToString
                                End If
                            Else
                                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordByPrimaryKey", message:="Value for primary key couldnot be converted to ColumnData", _
                                                      argument:=aValue, messagetype:=otCoreMessageType.InternalError, entryname:=fieldname, containerID:=Me.TableID)
                                Return Nothing
                            End If

                        End If

                    Next j

                Catch ex As Exception
                    Call CoreMessageHandler(procedure:="adonetTableStore.delRecordByPrimaryKey", message:="Exception", exception:=ex)
                    Return False
                End Try

                ' find it
                Try
                    '*** check on Property Cached
                    If Me.HasProperty(ConstTPNCacheProperty) AndAlso Me.IsCacheInitialized Then

                        Dim dataRows() As DataRow = _cacheTable.Select(wherestr)
                        SyncLock dataRows
                            ' not found
                            If dataRows.GetLength(0) = 0 Then
                                DeleteRecordByPrimaryKey = False
                            Else
                                dataRows(0).Delete()
                                DeleteRecordByPrimaryKey = True
                            End If
                        End SyncLock
                        '* InstantUpdate not implemented

                        If UpdateDBDataTable(_cacheAdapter, _cacheTable) > 0 Then
                            DeleteRecordByPrimaryKey = True
                        Else
                            DeleteRecordByPrimaryKey = False
                        End If

                        If False Then
                            If Me.HasProperty(ConstTPNCacheUpdateInstant) Then
                                If UpdateDBDataTable(_cacheAdapter, _cacheTable) > 0 Then
                                    DeleteRecordByPrimaryKey = True
                                Else
                                    DeleteRecordByPrimaryKey = False
                                End If
                            Else
                                CoreMessageHandler(message:="not implemented")
                            End If
                        End If

                    Else
                        If aSQLDeleteCommand.ExecuteNonQuery > 0 Then
                            DeleteRecordByPrimaryKey = True
                        Else
                            DeleteRecordByPrimaryKey = False
                        End If

                    End If

                    Return DeleteRecordByPrimaryKey


                Catch ex As Exception
                    Call CoreMessageHandler(showmsgbox:=silent, procedure:="adonetTableStore.delRecordByPrimaryKeys", _
                                          containerID:=Me.TableID, entryname:=fieldname, exception:=ex)
                    Return False
                End Try

            End SyncLock

        End Function

        ''' <summary>
        ''' GetRecordbyPrimaryKey returns a clsOTDBRecord object by the Primarykey from the Database
        ''' </summary>
        ''' <param name="primaryKeyArray">PrimaryKey Array</param>
        ''' <param name="silent"></param>
        ''' <returns>returns Nothing if not found otherwise a clsOTDBRecord</returns>
        ''' <remarks></remarks>
        Public Overrides Function GetRecordByPrimaryKey(ByRef primaryKeyArray() As Object, Optional silent As Boolean = False) As ormRecord _
        Implements iormRelationalTableStore.GetRecordByPrimaryKey
            'Dim aConnection As IDbConnection
            Dim aSqlSelectCommand As IDbCommand
            Dim j As Integer
            Dim afieldname As String
            Dim aValue As Object
            Dim wherestr As String = Nothing
            Dim abostrophNecessary As Boolean
            Dim aCvtValue As Object
            Dim aDataReader As IDataReader

            If Not IsArray(primaryKeyArray) Then
                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByPrimaryKey", message:="Empty Key Array")
                WriteLine("uups - no Array as primaryKey")
                Return Nothing
            ElseIf primaryKeyArray.GetUpperBound(0) < (Me.ContainerSchema.NoPrimaryEntries - 1) Then
                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByPrimaryKey", message:="Size of Primary Key Array less than the number of primary keys", _
                                      argument:=Me.TableID, messagetype:=otCoreMessageType.InternalError)
                Return Nothing

            End If

            ' Connection
            Try
                If Not Me.Connection.IsConnected AndAlso Not Me.Connection.Session.IsBootstrappingInstallationRequested Then
                    Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByPrimaryKey", message:="Connection is not available")
                    Return Nothing
                End If

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByPrimaryKey", exception:=ex)
                Return Nothing
            End Try

            ''' check if schema is initialized
            ''' 
            If Not Me.ContainerSchema.IsInitialized Then
                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByPrimaryKey", messagetype:=otCoreMessageType.InternalError, _
                                      message:="tableschema couldnot be initialized - loaded to fail ?", containerID:=Me.TableID)
                Return Nothing
            End If

            '*** check on Property Cached
            If Me.HasProperty(ConstTPNCacheProperty) Then
                If Not Me.IsCacheInitialized Then
                    Me.InitializeCache()
                End If
            End If

            '* get PrimaryKeys and their value -> build the criteria
            '*
            aSqlSelectCommand = TryCast(Me.ContainerSchema, adonetTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, adonetTableSchema.CommandType.SelectType)
            If aSqlSelectCommand Is Nothing Then
                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordByPrimaryKey", message:="Select Command is not in Store", _
                                      argument:=Me.ContainerSchema.PrimaryKeyIndexName, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If

            SyncLock aSqlSelectCommand.Connection

                Try

                    For j = 0 To (Me.ContainerSchema.NoPrimaryEntries - 1)

                        ' value of key
                        aValue = primaryKeyArray(j)
                        afieldname = Me.ContainerSchema.GetPrimaryEntryNames(j + 1)
                        If j <> 0 Then
                            wherestr &= String.Format(" AND [{0}]", afieldname)
                        Else
                            wherestr &= String.Format(" [{0}]", afieldname)
                        End If
                        If Not String.IsNullOrEmpty(afieldname) Then
                            If Convert2ContainerData(afieldname, invalue:=aValue, outvalue:=aCvtValue, abostrophNecessary:=abostrophNecessary) Then
                                ' build parameter
                                aSqlSelectCommand.Parameters(j).Value = aCvtValue
                                ' and build wherestring for cache
                                If abostrophNecessary Then
                                    wherestr &= " = '" & aCvtValue.ToString & "'"
                                Else
                                    wherestr &= " = " & aCvtValue.ToString
                                End If
                            Else
                                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordByPrimaryKey", message:="Value for primary key couldnot be converted to ColumnData", _
                                                      argument:=aValue, messagetype:=otCoreMessageType.InternalError, entryname:=afieldname, containerID:=Me.TableID)
                                Return Nothing
                            End If

                        End If

                    Next j

                Catch ex As Exception
                    Call CoreMessageHandler(procedure:="adonetTableStore.getRecordByPrimaryKey", message:="Exception", exception:=ex)
                    Return Nothing
                End Try


                '**** read
                Try
                    '*** check on Property Cached
                    If Me.HasProperty(ConstTPNCacheProperty) AndAlso Me.IsCacheInitialized Then
                        Dim dataRows() As DataRow = _cacheTable.Select(wherestr)

                        ' not found
                        If dataRows.GetLength(0) = 0 Then
                            Return Nothing
                        Else
                            '** Factory a new clsOTDBRecord
                            '**
                            Dim aNewRecord As New ormRecord(containerID:=Me.TableID, dbdriver:=Me.Connection.DatabaseDriver, runtimeOnly:=False)
                            If aNewRecord.LoadFrom(dataRows(0)) Then
                                Return aNewRecord
                            Else
                                Return Nothing
                            End If
                            'If InfuseRecord(record:=aNewEnt, dataobject:=dataRows(0), CreateNewrecord:=True) Then
                            '    Return aNewEnt
                            'Else

                            '    Return Nothing
                            'End If
                        End If
                    Else
                        ''' run the datareader
                        ''' 
                        aDataReader = aSqlSelectCommand.ExecuteReader
                        If aDataReader.Read Then
                            '** Factory a new clsOTDBRecord
                            '**
                            Dim aNewRecord As New ormRecord(containerID:=Me.TableID, dbdriver:=Me.Connection.DatabaseDriver, runtimeOnly:=False)
                            If aNewRecord.LoadFrom(aDataReader, InSync:=True) Then
                                aDataReader.Close()
                                Return aNewRecord
                            Else
                                aDataReader.Close()
                                Return Nothing
                            End If
                            'Dim aNewEnt As New ormRecord
                            'If InfuseRecord(aNewEnt, aDataReader, CreateNewrecord:=True) Then
                            '    aDataReader.Close()
                            '    Return aNewEnt
                            'Else
                            '    aDataReader.Close()
                            '    Return Nothing
                            'End If
                        Else
                            aDataReader.Close()
                            Return Nothing
                        End If


                    End If


                Catch ex As Exception
                    Call CoreMessageHandler(showmsgbox:=silent, procedure:="adonetTableStore.getRecordByPrimaryKey", _
                                          containerID:=Me.TableID, argument:=primaryKeyArray, exception:=ex)
                    If aDataReader IsNot Nothing Then aDataReader.Close()
                    Return Nothing
                End Try

            End SyncLock

        End Function

        '****** getRecords by Index
        '******
        Public Overrides Function GetRecordsByIndex(indexname As String, ByRef keyArray() As Object, Optional silent As Boolean = False) As List(Of ormRecord) _
        Implements iormRelationalTableStore.GetRecordsByIndex
            Dim aSqlSelectCommand As IDbCommand
            Dim j As Integer
            Dim fieldname As String
            Dim aValue As Object
            Dim anIndexColumnList As ArrayList
            Dim abostrophNecessary As Boolean
            Dim aCvtValue As Object
            Dim wherestr As String = Nothing
            Dim aNewRecord As ormRecord
            Dim aCollection As New List(Of ormRecord)
            Dim aDataReader As IDataReader

            ' check if schema is initialized
            If Not Me.ContainerSchema.IsInitialized Then
                Call CoreMessageHandler(procedure:="adonetTableStore.DelRecordsByPrimaryKey", messagetype:=otCoreMessageType.InternalError, _
                                      message:="tableschema couldnot be initialized - loaded to fail ?", containerID:=Me.TableID)
                Return Nothing
            End If

            '* get Index and their value -> build the criteria
            '*
            If Me.ContainerSchema.HasIndex(indexname) Then

                anIndexColumnList = Me.ContainerSchema.GetIndex(indexname)
            ElseIf Me.ContainerSchema.HasIndex(String.Format("{0}_{1}", Me.TableID, indexname)) Then
                indexname = String.Format("{0}_{1}", Me.TableID, indexname)
                anIndexColumnList = Me.ContainerSchema.GetIndex(indexname)
            Else
                Call CoreMessageHandler(procedure:="clsADOStore.getRecordsByIndex", argument:=indexname, _
                                      message:="Index does not exists for Table " & Me.TableID, messagetype:=otCoreMessageType.InternalError, _
                                      containerID:=Me.TableID)
                Return Nothing
            End If

            If Not IsArray(keyArray) Then
                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByIndex", message:="Empty Key Array", _
                                      messagetype:=otCoreMessageType.InternalError, _
                                      containerID:=Me.TableID)
                WriteLine("uups - no Array as primaryKey")
                Return Nothing
            ElseIf keyArray.GetUpperBound(0) > (anIndexColumnList.Count - 1) Then
                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByIndex", message:="Size of Primary Key Array less than the number of primary keys", _
                                      argument:=Me.TableID, messagetype:=otCoreMessageType.InternalError)
                Return Nothing

            End If

            ' Connection
            Try
                If Not Me.Connection.IsConnected AndAlso Not Me.Connection.Session.IsBootstrappingInstallationRequested Then
                    Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByIndex", message:="Connection is not available")
                    Return Nothing
                End If

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByIndex", exception:=ex)
                Return Nothing
            End Try

            '*** check on Property Cached
            If Me.HasProperty(ConstTPNCacheProperty) Then
                If Not Me.IsCacheInitialized Then
                    Me.InitializeCache()
                End If
            End If

            '* get PrimaryKeys and their value -> build the criteria
            '*
            aSqlSelectCommand = TryCast(Me.ContainerSchema, adonetTableSchema).GetCommand(indexname, adonetTableSchema.CommandType.SelectType)
            If aSqlSelectCommand Is Nothing Then
                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByIndex", message:="Select Command is not in Store", _
                                      argument:=Me.ContainerSchema.PrimaryKeyIndexName, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If
            SyncLock aSqlSelectCommand.Connection

                Try

                    For j = 0 To (anIndexColumnList.Count - 1)

                        ' reflect part keys
                        If j <= keyArray.GetUpperBound(0) Then

                            ''' build the statement out of it
                            aValue = keyArray(j)
                            fieldname = anIndexColumnList.Item(j)
                            If j <> 0 Then
                                wherestr &= String.Format(" AND [{0}]", fieldname)
                            Else
                                wherestr &= "[" & fieldname & "]"
                            End If
                            If Not String.IsNullOrEmpty(fieldname) Then
                                If Me.Convert2ContainerData(fieldname, invalue:=aValue, outvalue:=aCvtValue, abostrophNecessary:=abostrophNecessary) Then
                                    ' set parameter
                                    aSqlSelectCommand.Parameters(j).Value = aCvtValue
                                    ' and build wherestring for cache
                                    If abostrophNecessary Then
                                        wherestr &= " = '" & aCvtValue.ToString & "'"
                                    Else
                                        wherestr &= " = " & aCvtValue.ToString
                                    End If
                                Else
                                    Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByIndex", message:="Value for primary key couldnot be converted to ColumnData", _
                                                          argument:=aValue, messagetype:=otCoreMessageType.InternalError, entryname:=fieldname, containerID:=Me.TableID)
                                    Return Nothing

                                End If

                            End If
                        End If

                    Next j

                Catch ex As Exception
                    Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsByIndex", message:="Exception", exception:=ex)
                    Return New List(Of ormRecord)
                End Try

                ''' read section
                ''' 
                Try
                    ''' try to read on the cache table if we have it
                    ''' 
                    If Me.HasProperty(ConstTPNCacheProperty) AndAlso Me.IsCacheInitialized Then
                        Dim dataRows() As DataRow
                        If _cacheViews.ContainsKey(key:=indexname) Then
                            Dim aDataView = _cacheViews.Item(key:=indexname)

                            dataRows = aDataView.Table.Select(wherestr)
                        Else
                            dataRows = _cacheTable.Select(wherestr)
                        End If

                        ' not found
                        If dataRows.GetLength(0) = 0 Then
                            Return aCollection
                        Else
                            For Each row In dataRows
                                aNewRecord = New ormRecord(containerID:=Me.TableID, dbdriver:=Me.Connection.DatabaseDriver, runtimeOnly:=False)
                                If aNewRecord.LoadFrom(row) Then
                                    aCollection.Add(item:=aNewRecord)
                                Else
                                    Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsBySQL", message:="couldnot infuse a record", _
                                                         argument:=aNewRecord, containerID:=Me.TableID, break:=False)
                                End If

                            Next
                        End If
                    Else
                        ''' read from the data reader
                        ''' 
                        aDataReader = aSqlSelectCommand.ExecuteReader

                        Do While aDataReader.Read
                            aNewRecord = New ormRecord(containerID:=Me.TableID, dbdriver:=Me.Connection.DatabaseDriver, runtimeOnly:=False)
                            If aNewRecord.LoadFrom(aDataReader, InSync:=True) Then
                                aCollection.Add(item:=aNewRecord)
                            Else
                                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsBySQL", message:="couldnot infuse a record", _
                                                     argument:=aNewRecord, containerID:=Me.TableID, break:=False)
                            End If
                            ''** Factory a new clsOTDBRecord
                            'aNewRecord = New ormRecord
                            'If InfuseRecord(aNewRecord, aDataReader, CreateNewrecord:=True) Then
                            '    aCollection.Add(item:=aNewRecord)
                            'Else
                            '    Call CoreMessageHandler(subname:="adonetTableStore.getRecordsBySQL", message:="couldnot infuse a record", _
                            '                          arg1:=aNewRecord, tablename:=Me.TableID, break:=False)
                            'End If

                        Loop

                        aDataReader.Close()

                    End If

                    Return aCollection
                    '*****
                    '***** Error Handling
                    '*****
                Catch ex As Exception
                    Call CoreMessageHandler(showmsgbox:=silent, procedure:="adonetTableStore.getRecordsByIndex", _
                                          containerID:=Me.TableID, argument:=keyArray, exception:=ex)
                    If aDataReader IsNot Nothing Then aDataReader.Close()

                    Return New List(Of ormRecord)
                End Try

            End SyncLock

        End Function

        ''' <summary>
        ''' Update a Datatable with the adapter
        ''' </summary>
        ''' <param name="datatable"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function UpdateDBDataTable(ByRef dataadapter As IDbDataAdapter, ByRef datatable As DataTable) As Integer

        '****** runs a SQLCommand
        '******
        Public Overrides Function RunSqlStatement(ByVal sqlcmdstr As String, Optional ByRef parameters As List(Of ormSqlCommandParameter) = Nothing, Optional silent As Boolean = True) As Boolean _
        Implements iormRelationalTableStore.RunSqlStatement

            Return Me.Connection.DatabaseDriver.RunSqlStatement(sqlcmdstr:=sqlcmdstr, parameters:=parameters, silent:=silent)

        End Function
        '****** returns the Collection of Records by SQL
        '******
        Public Overrides Function GetRecordsBySql(ByVal wherestr As String, _
        Optional ByVal fullsqlstr As String = Nothing, _
        Optional ByVal innerjoin As String = Nothing, _
        Optional ByVal orderby As String = Nothing, _
        Optional ByVal silent As Boolean = False, _
        Optional ByRef parameters As List(Of ormSqlCommandParameter) = Nothing) As List(Of ormRecord) Implements iormRelationalTableStore.GetRecordsBySql

            Dim aConnection As IDbConnection
            Dim i As Integer
            Dim cmdstr As String
            Dim aCollection As New List(Of ormRecord)
            Dim aNewRecord As ormRecord
            Dim fieldstr As String

            ' Connection
            Try
                If Me.Connection.IsConnected OrElse Me.Connection.Session.IsBootstrappingInstallationRequested Then
                    aConnection = DirectCast(Me.Connection.NativeConnection, IDbConnection)
                    If aConnection Is Nothing And Me.Connection.Session.IsBootstrappingInstallationRequested Then
                        aConnection = DirectCast(DirectCast(Me.Connection, adonetConnection).NativeInternalConnection, IDbConnection)
                    Else
                        CoreMessageHandler(message:="No Internal connection available", procedure:="adnoetTablestore.getrecordsbysql", _
                                            messagetype:=otCoreMessageType.InternalError)
                    End If
                Else
                    Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsBySQL", message:="Connection is not available", messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsBySQL", exception:=ex)
                Return Nothing
            End Try

            ' check if schema is initialized
            If Not Me.ContainerSchema.IsInitialized Then
                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordBySQL", messagetype:=otCoreMessageType.InternalError, _
                                      message:="tableschema couldnot be initialized - loaded to fail ?", containerID:=Me.TableID)
                Return Nothing
            End If

            '*** check on Property Cached
            If Me.HasProperty(ConstTPNCacheProperty) Then
                If Not Me.IsCacheInitialized Then
                    Me.InitializeCache()
                End If
            End If


            If Not String.IsNullOrWhiteSpace(fullsqlstr) Then
                cmdstr = fullsqlstr
            Else

                i = 0
                fieldstr = String.Empty
                For Each field As String In Me.ContainerSchema.EntryNames
                    If i = 0 Then
                        fieldstr = "[" & Me.NativeDBObjectname & "].[" & field & "]"
                        i += 1
                    Else
                        fieldstr &= " , [" & Me.NativeDBObjectname & "].[" & field & "]"
                    End If
                Next

                ' Select
                If String.IsNullOrWhiteSpace(innerjoin) Then
                    cmdstr = String.Format("SELECT * FROM [{0}] WHERE {1}", Me.NativeDBObjectname, wherestr)
                Else
                    cmdstr = "SELECT " & fieldstr & " FROM [" & Me.NativeDBObjectname & "] " & innerjoin & " WHERE " & wherestr
                End If

                If Not String.IsNullOrWhiteSpace(orderby) Then
                    cmdstr = cmdstr & " ORDER BY " & orderby
                End If
            End If

            Try
                '*** check on Property Cached
                If Me.HasProperty(ConstTPNCacheProperty) AndAlso Me.IsCacheInitialized Then
                    Dim dataRows() As DataRow = _cacheTable.Select(wherestr)

                    ' not found
                    If dataRows.GetLength(0) = 0 Then
                        Return aCollection
                    Else
                        For Each row In dataRows
                            ''' infuse the records
                            aNewRecord = New ormRecord(containerID:=Me.TableID, dbdriver:=Me.Connection.DatabaseDriver)
                            If aNewRecord.LoadFrom(row) Then
                                aCollection.Add(item:=aNewRecord)
                            Else
                                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsBySQL", message:="couldnot infuse a record", _
                                                      argument:=aNewRecord, containerID:=Me.TableID, break:=False)
                            End If
                            'If InfuseRecord(aNewRecord, row, CreateNewrecord:=True) Then
                            '    aCollection.Add(item:=aNewRecord)
                            'Else
                            '    Call CoreMessageHandler(subname:="adonetTableStore.getRecordsBySQL", message:="couldnot infuse a record", _
                            '                          arg1:=aNewRecord, tablename:=Me.TableID, break:=False)
                            'End If
                        Next
                    End If
                Else
                    Dim aSqlCommand As IDbCommand = CreateNativeDBCommand(cmdstr, aConnection)
                    Dim aDataReader As IDataReader
                    SyncLock aSqlCommand.Connection
                        ' read
                        aDataReader = aSqlCommand.ExecuteReader
                        Do While aDataReader.Read
                            ''' infuse the records
                            aNewRecord = New ormRecord(containerID:=Me.TableID, dbdriver:=Me.Connection.DatabaseDriver)
                            If aNewRecord.LoadFrom(aDataReader) Then
                                aCollection.Add(item:=aNewRecord)
                            Else
                                Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsBySQL", message:="couldnot infuse a record", _
                                                      argument:=aNewRecord, containerID:=Me.TableID, break:=False)
                            End If
                            ''** Factory a new clsOTDBRecord
                            'aNewRecord = New ormRecord
                            'If InfuseRecord(aNewRecord, aDataReader, CreateNewrecord:=True) Then
                            '    aCollection.Add(item:=aNewRecord)
                            'Else
                            '    Call CoreMessageHandler(subname:="adonetTableStore.getRecordsBySQL", message:="couldnot infuse a record", _
                            '                          arg1:=aNewRecord, tablename:=Me.TableID, break:=False)
                            'End If

                        Loop

                        ' close
                        aDataReader.Close()

                    End SyncLock
                End If



                ' return
                If aCollection.Count > 0 Then
                    GetRecordsBySql = aCollection
                Else
                    GetRecordsBySql = Nothing
                End If

                Exit Function

                '******** error handling
            Catch ex As Exception

                Call CoreMessageHandler(showmsgbox:=silent, procedure:="adonetTableStore.getRecordsBySQL", containerID:=Me.TableID, _
                                      argument:="Where :" & wherestr & " inner join: " & innerjoin & " full: " & fullsqlstr, _
                                      exception:=ex)

                Return New List(Of ormRecord)
            End Try



        End Function
        ''' <summary>
        ''' returns a collection of records selected by this helper command which creates an SqlCommand with an ID or reuse one
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
        Public Overrides Function GetRecordsBySqlCommand(ByVal ID As String, _
                                    Optional ByVal wherestr As String = Nothing, _
                                    Optional ByVal fullsqlstr As String = Nothing, _
                                    Optional ByVal innerjoin As String = Nothing, _
                                    Optional ByVal orderby As String = Nothing, _
                                    Optional ByVal silent As Boolean = False, _
                                    Optional ByRef parameters As List(Of ormSqlCommandParameter) = Nothing) As List(Of ormRecord) _
                                Implements iormRelationalTableStore.GetRecordsBySqlCommand


            Dim aCollection As New List(Of ormRecord)
            Dim aParameterValues As New Dictionary(Of String, Object)
            Dim aCommand As ormSqlSelectCommand

            '*** check on Property Cached
            If Me.HasProperty(ConstTPNCacheProperty) Then
                If Not Me.IsCacheInitialized Then
                    Me.InitializeCache()
                End If
            End If

            ' check if schema is initialized
            If Not Me.ContainerSchema.IsInitialized Then
                Call CoreMessageHandler(procedure:="adonetTableStore.GetRecordBySQLCommand", messagetype:=otCoreMessageType.InternalError, _
                                      message:="table schema could not be initialized - loaded to fail ?", containerID:=Me.TableID)
                Return Nothing
            End If

            Try
                ' get
                aCommand = Me.CreateSqlSelectCommand(ID)
                SyncLock aCommand
                    If Not aCommand.IsPrepared Then
                        aCommand.AddTable(Me.TableID, addAllFields:=True)
                        aCommand.Where = wherestr
                        aCommand.InnerJoin = innerjoin
                        aCommand.OrderBy = orderby
                        'If fullsqlstr <> String.empty then aCommand.SqlText = fullsqlstr 
                        If parameters IsNot Nothing Then
                            For Each aParameter In parameters
                                aCommand.AddParameter(aParameter)
                                aParameterValues.Add(aParameter.ID, aParameter.Value)
                            Next
                        End If

                        If Not aCommand.Prepare Then
                            Call CoreMessageHandler(message:="couldnot prepare command", procedure:="adonetTableStore.getRecordsBySQLCommand", _
                                                   messagetype:=otCoreMessageType.InternalError, argument:=aCommand.SqlText)
                            Return New List(Of ormRecord)
                        End If
                    End If


                    '*** check on Property Cached
                    '***
                    If Me.HasProperty(ConstTPNCacheProperty) AndAlso Me.IsCacheInitialized Then
                        Dim aDataview = _cacheTable.AsDataView
                        If Not String.IsNullOrWhiteSpace(aCommand.OrderBy) Then aDataview.Sort = aCommand.OrderBy
                        If Not String.IsNullOrWhiteSpace(aCommand.Where) Then
                            Dim wherestatement As String = aCommand.Where
                            wherestatement = wherestatement.Replace("[", " ").Replace("]", " ")
                            If wherestatement.Contains(".") Then
                                '** strip off all the table namings
                                wherestatement = Regex.Replace(wherestatement, "\S*\.", String.Empty)
                            End If
                            '** replace the values
                            If aCommand.Parameters IsNot Nothing Then
                                For Each aParameter In aCommand.Parameters
                                    If aParameter.Datatype <> otDataType.Memo And aParameter.Datatype <> otDataType.Text And aParameter.Datatype <> otDataType.List Then
                                        wherestatement = wherestatement.Replace(aParameter.ID, aParameter.Value)
                                    Else
                                        wherestatement = wherestatement.Replace(aParameter.ID, "'" & aParameter.Value & "'")
                                    End If
                                Next
                            End If

                            aDataview.RowFilter = wherestatement
                        End If

                        Dim dataRows() As DataRow = aDataview.ToTable.Select()

                        ' not found
                        If dataRows.GetLength(0) = 0 Then
                            Return aCollection
                        Else
                            For Each row In dataRows
                                ''' infuse the records
                                Dim aNewRecord As New ormRecord(containerID:=Me.TableID, dbdriver:=Me.Connection.DatabaseDriver)
                                If aNewRecord.LoadFrom(row) Then
                                    aCollection.Add(item:=aNewRecord)
                                Else
                                    Call CoreMessageHandler(procedure:="adonetTableStore.getRecordsBySQLCommand", message:="couldnot infuse a record", _
                                                          argument:=aNewRecord, containerID:=Me.TableID, break:=False)
                                End If
                                'Dim aNewEnt = New ormRecord
                                'If InfuseRecord(aNewEnt, row, CreateNewrecord:=True) Then
                                '    aCollection.Add(item:=aNewEnt)
                                'Else
                                '    Call CoreMessageHandler(subname:="adonetTableStore.getRecordsBySQLCommand", message:="couldnot infuse a record", _
                                '                          arg1:=aNewEnt, tablename:=Me.TableID, break:=False)
                                'End If
                            Next
                        End If

                        Return aCollection
                    Else
                        ' replace parametervalues out of the paramter -> might be different to the prepared one
                        If parameters IsNot Nothing Then
                            For Each aParameter In parameters
                                If Not aParameterValues.ContainsKey(key:=aParameter.ID) Then
                                    aParameterValues.Add(aParameter.ID, aParameter.Value)
                                End If
                            Next
                        End If
                        '** NOCACHE
                        '** run the Command
                        Dim theRecords As List(Of ormRecord) = _
                            Me.Connection.DatabaseDriver.RunSqlSelectCommand(aCommand, parametervalues:=aParameterValues)

                        Return theRecords
                    End If
                End SyncLock
                '******** error handling
            Catch ex As Exception

                Call CoreMessageHandler(showmsgbox:=silent, procedure:="adonetTableStore.getRecordsBySQLCommand", containerID:=Me.TableID, _
                                      argument:="Where :" & wherestr & " inner join: " & innerjoin & " full: " & fullsqlstr, _
                                      exception:=ex)

                Return New List(Of ormRecord)
            End Try



        End Function
        ''' <summary>
        ''' return a collection of records for a sqlcommand on a table
        ''' </summary>
        ''' <param name="sqlcommand"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Overrides Function GetRecordsBySqlCommand(ByRef sqlcommand As ormSqlSelectCommand, _
                                                         Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing) As List(Of ormRecord) _
                                                        Implements iormRelationalTableStore.GetRecordsBySqlCommand

            Return Me.Connection.DatabaseDriver.RunSqlSelectCommand(sqlcommand:=sqlcommand, parametervalues:=parametervalues)
        End Function
        ''' <summary>
        ''' infuse a Record with the Help of the Datareader Object
        ''' </summary>
        ''' <param name="record">clsOTDBRecord</param>
        ''' <param name="DataReader">an open Datareader which has just the data</param>
        ''' <param name="silent">no messages</param>
        ''' <returns>True if successfull and read</returns>
        ''' <remarks></remarks>
        Public Overrides Function InfuseRecord(ByRef record As ormRecord, ByRef dataobject As Object, _
        Optional ByVal silent As Boolean = False, Optional CreateNewrecord As Boolean = False) As Boolean _
        Implements iormRelationalTableStore.InfuseRecord
            Dim aDBColumn As adonetColumnDescription
            Dim cvtvalue, Value As Object
            Dim j As Integer
            Dim abostrophNecessary As Boolean
            Dim ordinal As Nullable(Of Integer)
            Dim aDatareader As IDataReader = Nothing
            Dim aRow As DataRow = Nothing

            ' check if schema is initialized
            If Not Me.ContainerSchema.IsInitialized Then
                Call CoreMessageHandler(procedure:="adonetTableStore.InfuseRecord", messagetype:=otCoreMessageType.InternalError, _
                                      message:="tableschema couldnot be initialized - loaded to fail ?", containerID:=Me.TableID)
                Return Nothing
            End If

            Try
                If GetType(IDataReader).IsAssignableFrom(dataobject.GetType) AndAlso Not dataobject.GetType.IsAbstract Then
                    aDatareader = DirectCast(dataobject, IDataReader)

                ElseIf dataobject.GetType() = GetType(DataRow) Then
                    aRow = DirectCast(dataobject, DataRow)
                Else
                    Call CoreMessageHandler(procedure:="adonetTableStore.infuseRecord", message:="Data object has no known type", _
                                          argument:=dataobject.GetType.ToString)
                    Return False

                End If
            Catch ex As Exception
                Call CoreMessageHandler(procedure:="adonetTableStore.infuseRecord", exception:=ex, message:="Exception", _
                                      argument:=dataobject.GetType.ToString)
                Return False
            End Try
            Try

                '** Factory a new clsOTDBRecord
                '**
                ''' if record is not supplied take a bound record
                If record Is Nothing OrElse CreateNewrecord Then record = New ormRecord(containerID:=Me.TableID, dbdriver:=Me.Connection.DatabaseDriver)
                record.IsLoaded = True ' definitely loaded ! not created

                For j = 1 To Me.ContainerSchema.NoEntries
                    ' get fields
                    aDBColumn = DirectCast(Me.ContainerSchema, adonetTableSchema).GetColumnDescription(j)
                    If aDBColumn IsNot Nothing Then
                        Try
                            If Not aDatareader Is Nothing Then
                                ordinal = aDatareader.GetOrdinal(aDBColumn.ColumnName)
                            End If
                        Catch ex As System.IndexOutOfRangeException
                            Try
                                ordinal = aDatareader.GetOrdinal(String.Format("{0}.{1}", Me.TableID, aDBColumn.ColumnName))
                            Catch ex2 As Exception
                                Call CoreMessageHandler(exception:=ex2, message:="Exception", procedure:="adonetTableStore.infuseRecord", _
                                                      argument:=Me.TableID, messagetype:=otCoreMessageType.InternalError)
                            Finally
                                ordinal = Nothing
                            End Try
                        End Try

                        If aDatareader IsNot Nothing Then
                            If ordinal IsNot Nothing AndAlso ordinal >= 0 Then
                                Value = aDatareader.GetValue(ordinal)
                                If Me.Convert2ObjectData(j, invalue:=Value, outvalue:=cvtvalue, abostrophNecessary:=abostrophNecessary) Then
                                    Call record.SetValue(j, cvtvalue)
                                Else
                                    Call CoreMessageHandler(procedure:="adonetTableStore.infuseRecord", message:="could not convert db value", argument:=Value, _
                                                      containerEntryName:=aDBColumn.ColumnName, containerID:=Me.TableID, messagetype:=otCoreMessageType.InternalError)
                                End If
                            Else
                                Call CoreMessageHandler(procedure:="adonetTableStore.infuseRecord", message:="ordinal missing - Field not in DataReader", _
                                                      entryname:=aDBColumn.ColumnName, containerID:=Me.TableID, messagetype:=otCoreMessageType.InternalError)
                            End If
                        Else
                            '** aRow
                            Value = aRow.Item(j - 1)
                            If Me.Convert2ObjectData(j, invalue:=Value, outvalue:=cvtvalue, abostrophNecessary:=abostrophNecessary) Then
                                Call record.SetValue(j, cvtvalue)
                            Else
                                Call CoreMessageHandler(procedure:="adonetTableStore.infuseRecord", message:="could not convert db value", argument:=Value, _
                                                  containerEntryName:=aDBColumn.ColumnName, containerID:=Me.TableID, messagetype:=otCoreMessageType.InternalError)
                            End If

                        End If
                    Else
                        Call CoreMessageHandler(procedure:="adonetTableStore.infuseRecord", message:="DBColumn missing - Field not in DataReader", _
                                              argument:=j, containerID:=Me.TableID, messagetype:=otCoreMessageType.InternalError)
                    End If
                Next j

                Return True

            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="adonetTableStore.infuseRecord")
                Return False
            End Try
        End Function

        ''' <summary>
        ''' persists aRecord to the database if aRecord is created or loaded
        ''' </summary>
        ''' <param name="record">clsOTDBRecord</param>
        ''' <param name="timestamp">the Timestamp to be used for the ChangedOn or CreatedOn</param>
        ''' <param name="silent">no messages</param>
        ''' <returns>true if successfull and written, false if error or no changes</returns>
        ''' <remarks></remarks>
        Public Function PersistCache(ByRef record As ormRecord, _
                                     Optional ByVal timestamp As Date = ot.ConstNullDate, _
                                     Optional ByVal silent As Boolean = False) As Boolean

            Dim fieldname As String
            Dim aCVTValue, aValue As Object
            Dim j As Integer
            Dim abostrophNecessary As Boolean
            Dim wherestr As String = Nothing
            Dim changedRecord As Boolean
            Dim dataRows() As DataRow

            ' timestamp
            If timestamp = ConstNullDate Then
                timestamp = Date.Now
            End If

            ' Connection

            Try
                If Not Me.Connection.IsConnected AndAlso Not Me.Connection.Session.IsBootstrappingInstallationRequested Then
                    Call CoreMessageHandler(procedure:="adonetTableStore.PersistCache", message:="Connection is not available")
                    Return Nothing
                End If

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="adonetTableStore.PersistCache", exception:=ex)
                Return Nothing
            End Try

            '*** check on Property Cached

            If Not Me.IsCacheInitialized Then
                Me.InitializeCache()
            End If

            '*** Try to persist

            Try
                '*** Check if not Status
                If record.IsUnknown OrElse (Not record.IsCreated And Not record.IsLoaded) Then
                    If Not record.CheckStatus Then
                        Return False
                    End If
                End If

                '*** Check which Command to use
                If record.IsLoaded Then

                    'build wherestring
                    For j = 0 To (Me.ContainerSchema.NoPrimaryEntries - 1)
                        ' value of key
                        fieldname = Me.ContainerSchema.GetPrimaryEntryNames(j + 1)
                        If j <> 0 Then
                            wherestr &= String.Format(" AND [{0}]", fieldname)
                        Else
                            wherestr &= String.Format("[{0}]", fieldname)
                        End If
                        aValue = record.GetValue(fieldname)
                        If Not String.IsNullOrEmpty(fieldname) Then
                            If Me.Convert2ContainerData(fieldname, invalue:=aValue, outvalue:=aCVTValue, abostrophNecessary:=abostrophNecessary) Then
                                If abostrophNecessary Then
                                    wherestr &= " = '" & aCVTValue.ToString & "'"
                                Else
                                    wherestr &= " = " & aCVTValue.ToString
                                End If
                            Else
                                Call CoreMessageHandler(procedure:="adonetTableStore.PersistCache", message:="Value for primary key could not be converted to ColumnData", _
                                                      argument:=aValue, messagetype:=otCoreMessageType.InternalError, entryname:=fieldname, containerID:=Me.TableID)
                                Return False
                            End If

                        End If

                    Next j

                    ' load
                    dataRows = _cacheTable.Select(wherestr)

                    If dataRows.Length = 0 Then
                        Call CoreMessageHandler(procedure:="adonetTableStore.persistCache", message:="Datarow to update not found", containerID:=Me.TableID)
                        Return False
                    End If


                ElseIf record.IsCreated Then
                    ReDim dataRows(0)
                    dataRows(0) = _cacheTable.NewRow
                    'set all primary keys
                    For j = 1 To Me.ContainerSchema.NoEntries
                        ' get fields
                        fieldname = Me.ContainerSchema.GetEntryName(j)
                        If Me.ContainerSchema.HasPrimaryEntryName(fieldname) Then
                            aValue = record.GetValue(fieldname)
                            If Convert2ContainerData(j, invalue:=aValue, outvalue:=aCVTValue, abostrophNecessary:=abostrophNecessary) Then
                                dataRows(0).Item(fieldname) = aCVTValue
                            Else
                                Call CoreMessageHandler(procedure:="adonetTableStore.persistCache", argument:=aValue, containerEntryName:=fieldname, _
                                                      message:="object primary key value could not be converted to column data", messagetype:=otCoreMessageType.InternalError, _
                                                      containerID:=Me.TableID)
                            End If
                        End If


                    Next j

                Else


                    Call CoreMessageHandler(procedure:="adonetTableStore.persistCache", argument:=Me.ContainerSchema.PrimaryKeyIndexName, _
                                          message:="record is nor loaded or created", messagetype:=otCoreMessageType.InternalError, _
                                          containerID:=Me.TableID)
                    Return False
                End If



                'get all fields
                For j = 1 To Me.ContainerSchema.NoEntries
                    ' get fields
                    fieldname = Me.ContainerSchema.GetEntryName(j)

                    If Not Me.ContainerSchema.HasPrimaryEntryName(fieldname) Then
                        If Not String.IsNullOrEmpty(fieldname) AndAlso fieldname <> ConstFNUpdatedOn AndAlso fieldname <> ConstFNCreatedOn Then
                            aValue = record.GetValue(fieldname)
                            If Me.Convert2ContainerData(j, invalue:=aValue, outvalue:=aCVTValue, abostrophNecessary:=abostrophNecessary) Then
                                dataRows(0).Item(fieldname) = aCVTValue
                                changedRecord = True
                            Else
                                Call CoreMessageHandler(procedure:="adonetTableStore.persistCache", argument:=aValue, containerEntryName:=fieldname, _
                                                      message:="object value could not be converted to column data", messagetype:=otCoreMessageType.InternalError, _
                                                      containerID:=Me.TableID)
                            End If

                        End If
                    End If
                Next j
                ' Update the record
                If changedRecord Then

                    '**** UpdateTimeStamp
                    If Me.ContainerSchema.GetEntryOrdinal(ConstFNUpdatedOn) > 0 Then
                        'rst.Fields(OTDBConst_UpdateOn).Value = aTimestamp
                        dataRows(0).Item(ConstFNUpdatedOn) = timestamp
                    End If

                    '*** Create Timestamp
                    If Me.ContainerSchema.GetEntryOrdinal(ConstFNCreatedOn) > 0 And record.IsCreated Then
                        dataRows(0).Item(ConstFNCreatedOn) = timestamp
                    ElseIf Me.ContainerSchema.GetEntryOrdinal(ConstFNCreatedOn) > 0 And Not record.IsCreated Then
                        If Not DBNull.Value.Equals(record.GetValue(ConstFNCreatedOn)) And Not record.GetValue(ConstFNCreatedOn) Is Nothing Then
                            dataRows(0).Item(ConstFNCreatedOn) = record.GetValue(ConstFNCreatedOn)    'keep the value
                        ElseIf Me.ContainerSchema.GetEntryOrdinal(ConstFNUpdatedOn) > 0 AndAlso _
                        Not DBNull.Value.Equals(record.GetValue(ConstFNUpdatedOn)) _
                        AndAlso Not record.GetValue(ConstFNUpdatedOn) Is Nothing Then
                            dataRows(0).Item(ConstFNCreatedOn) = record.GetValue(ConstFNUpdatedOn)    'keep the value
                        Else
                            dataRows(0).Item(ConstFNCreatedOn) = timestamp
                        End If
                    End If


                End If



                '** Run Command
                If changedRecord Then
                    '* add the record
                    If record.IsCreated Then
                        _cacheTable.Rows.Add(dataRows(0))
                        PersistCache = True
                    End If
                    ' save to the database not only the cache
                    ' synclock on connection of update (should be the same as insertCommand)
                    SyncLock _cacheAdapter.UpdateCommand.Connection
                        If Me.IsCacheInitialized Then
                            If UpdateDBDataTable(_cacheAdapter, _cacheTable) > 0 Then
                                PersistCache = True
                            End If
                        Else
                            CoreMessageHandler(message:="persist to an uninitialized cache ?!", procedure:="adonetTableStore.PersistCache", _
                                                messagetype:=otCoreMessageType.InternalError, containerID:=Me.TableID, argument:=dataRows.ToString)
                        End If

                    End SyncLock

                    If False Then
                        If Me.HasProperty(ConstTPNCacheUpdateInstant) AndAlso Me.IsCacheInitialized Then
                            SyncLock _cacheAdapter.UpdateCommand.Connection
                                If UpdateDBDataTable(_cacheAdapter, _cacheTable) > 0 Then
                                    PersistCache = True
                                End If
                            End SyncLock
                        ElseIf Not Me.HasProperty(ConstTPNCacheUpdateInstant) Then
                            CoreMessageHandler(message:="Perist later is not implemented", procedure:="adonetTableStore.PersistCache", _
                                              messagetype:=otCoreMessageType.InternalError, containerID:=Me.TableID, argument:=dataRows.ToString)
                        ElseIf Not Me.IsCacheInitialized Then
                            CoreMessageHandler(message:="persist to an uninitialized cache ?!", procedure:="adonetTableStore.PersistCache", _
                                                messagetype:=otCoreMessageType.InternalError, containerID:=Me.TableID, argument:=dataRows.ToString)
                        End If
                    End If
                    Return PersistCache
                Else
                    Return True
                End If



            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=silent, procedure:="adonetTableStore.PersistCache", exception:=ex, containerID:=Me.TableID)
                Return False
            End Try



        End Function
        ''' <summary>
        ''' persists aRecord to the database if aRecord is created or loaded
        ''' </summary>
        ''' <param name="aRecord">clsOTDBRecord</param>
        ''' <param name="aTimestamp">the Timestamp to be used for the ChangedOn or CreatedOn</param>
        ''' <param name="silent">no messages</param>
        ''' <returns>true if successfull and written, false if error or no changes</returns>
        ''' <remarks></remarks>
        Public Overrides Function PersistRecord(ByRef record As ormRecord, _
                                                Optional timestamp As Date = ot.ConstNullDate, _
                                                Optional ByVal silent As Boolean = False) As Boolean _
        Implements iormRelationalTableStore.PersistRecord

            ' check if schema is initialized
            If Not Me.ContainerSchema.IsInitialized Then
                Call CoreMessageHandler(procedure:="adonetTableStore.PersistRecord", messagetype:=otCoreMessageType.InternalError, _
                                      message:="tableschema couldnot be initialized - loaded to fail ?", containerID:=Me.TableID)
                Return False
            End If

            '*** check on Property Cached
            If (Me.HasProperty(ConstTPNCacheProperty) AndAlso Me.IsCacheInitialized) OrElse _
                (Me.HasProperty(ConstTPNCacheProperty) AndAlso Not Me.IsCacheInitialized AndAlso Me.InitializeCache) Then
                Return PersistCache(record, timestamp, silent)
            Else
                Return PersistDirect(record, timestamp, silent)
            End If
        End Function
        ''' <summary>
        ''' persists aRecord to the Cache if aRecord is created or loaded
        ''' </summary>
        ''' <param name="aRecord">clsOTDBRecord</param>
        ''' <param name="aTimestamp">the Timestamp to be used for the ChangedOn or CreatedOn</param>
        ''' <param name="silent">no messages</param>
        ''' <returns>true if successfull and written, false if error or no changes</returns>
        ''' <remarks></remarks>
        Public Function PersistDirect(ByRef record As ormRecord, _
                                      Optional ByVal timestamp As Date = ot.ConstNullDate, _
                                      Optional ByVal silent As Boolean = False) As Boolean


            Dim fieldname As String
            Dim aCVTValue, aValue As Object
            Dim j As Integer
            Dim abostrophNecessary As Boolean
            Dim changedRecord As Boolean
            Dim persistCommand As IDbCommand

            ' timestamp
            If timestamp = ConstNullDate Then
                timestamp = Date.Now
            End If

            ' Connection

            Try
                If Not Me.Connection.IsConnected AndAlso Not Me.Connection.Session.IsBootstrappingInstallationRequested Then
                    Call CoreMessageHandler(procedure:="adonetTableStore.PersistDirect", message:="Connection is not available")
                    Return Nothing
                End If

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="adonetTableStore.PersistDirect", exception:=ex)
                Return Nothing
            End Try

            '*** Try to persist

            Try
                '*** Check if not Status
                If (Not record.IsCreated And Not record.IsLoaded) OrElse record.IsUnknown Then
                    If Not record.CheckStatus Then
                        Return False
                    End If
                End If

                '*** Check which Command to use
                '****
                '**** UPDATE
                If record.IsLoaded Then
                    persistCommand = TryCast(Me.ContainerSchema, adonetTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, _
                                                                                                adonetTableSchema.CommandType.UpdateType)
                    If persistCommand Is Nothing Then
                        Call CoreMessageHandler(procedure:="adonetTableStore.PersistDirect", argument:=Me.ContainerSchema.PrimaryKeyIndexName, _
                                              message:="Update Command is not in store", messagetype:=otCoreMessageType.InternalError, containerID:=Me.TableID)
                        Return False
                    End If
                ElseIf record.IsCreated Then
                    persistCommand = TryCast(Me.ContainerSchema, adonetTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, _
                                                                                                adonetTableSchema.CommandType.InsertType)
                    If persistCommand Is Nothing Then
                        Call CoreMessageHandler(procedure:="adonetTableStore.PersistDirect", argument:=Me.ContainerSchema.PrimaryKeyIndexName, _
                                              message:="Update Command is not in store", messagetype:=otCoreMessageType.InternalError, containerID:=Me.TableID)
                        Return False
                    End If

                End If

                '*** lock the command and generate the parameters
                SyncLock persistCommand.Connection
                    '**** UPDATE
                    If record.IsLoaded Then

                        'get all fields -> update
                        For j = 1 To Me.ContainerSchema.NoEntries
                            ' get fields
                            fieldname = Me.ContainerSchema.GetEntryName(j)

                            If Not Me.ContainerSchema.HasPrimaryEntryName(fieldname) Then
                                If Not String.IsNullOrEmpty(fieldname) AndAlso fieldname <> ConstFNUpdatedOn AndAlso fieldname <> ConstFNCreatedOn Then
                                    aValue = record.GetValue(fieldname)
                                    If Me.Convert2ContainerData(fieldname, invalue:=aValue, _
                                                             outvalue:=aCVTValue, _
                                                             abostrophNecessary:=abostrophNecessary) Then
                                        persistCommand.Parameters.Item("@" & fieldname).Value = aCVTValue
                                        changedRecord = True
                                    Else
                                        CoreMessageHandler(message:="parameter value could not be converted", argument:=aValue, containerEntryName:=fieldname, containerID:=Me.ContainerSchema.ContainerID, _
                                                         procedure:="adonetTableStore.PersistDirect", messagetype:=otCoreMessageType.InternalWarning)
                                    End If
                                End If
                            End If
                        Next j
                        '*** set the primary key
                        For j = 0 To (Me.ContainerSchema.NoPrimaryEntries - 1)
                            ' value of key
                            fieldname = Me.ContainerSchema.GetPrimaryEntryNames(j + 1)
                            If Not String.IsNullOrEmpty(fieldname) Then
                                aValue = record.GetValue(fieldname)
                                If Me.Convert2ContainerData(fieldname, _
                                                         invalue:=aValue, _
                                                         outvalue:=aCVTValue, _
                                                         abostrophNecessary:=abostrophNecessary) Then
                                    persistCommand.Parameters.Item("@" & fieldname).Value = aCVTValue
                                Else
                                    CoreMessageHandler(message:="primary key parameter value could not be converted", argument:=aValue, containerEntryName:=fieldname, containerID:=Me.ContainerSchema.ContainerID, _
                                                     procedure:="adonetTableStore.PersistDirect", messagetype:=otCoreMessageType.InternalWarning)
                                End If

                            End If

                        Next j

                        '*****
                        '***** CREATE INSERT
                    ElseIf record.IsCreated Then
                        'get all fields -> update
                        For j = 1 To Me.ContainerSchema.NoEntries
                            ' get fields
                            fieldname = Me.ContainerSchema.GetEntryName(j)
                            If Not String.IsNullOrEmpty(fieldname) AndAlso fieldname <> ConstFNUpdatedOn AndAlso fieldname <> ConstFNCreatedOn Then
                                aValue = record.GetValue(fieldname)
                                If Me.Convert2ContainerData(j, invalue:=aValue, _
                                                         outvalue:=aCVTValue, _
                                                         abostrophNecessary:=abostrophNecessary) Then
                                    persistCommand.Parameters.Item("@" & fieldname).Value = aCVTValue
                                    changedRecord = True
                                Else
                                    CoreMessageHandler(message:="insert parameter value could not be converted", argument:=aValue, containerEntryName:=fieldname, containerID:=Me.ContainerSchema.ContainerID, _
                                                procedure:="adonetTableStore.PersistDirect", messagetype:=otCoreMessageType.InternalWarning)
                                End If
                            End If

                        Next j
                    Else

                        Call CoreMessageHandler(procedure:="adonetTableStore.PersistDirect", argument:=Me.ContainerSchema.PrimaryKeyIndexName, _
                                              message:="record is nor loaded or created", messagetype:=otCoreMessageType.InternalError, _
                                              containerID:=Me.TableID)
                        Return False
                    End If


                    ' Update the record
                    If changedRecord Then

                        '**** UpdateTimeStamp
                        If Me.ContainerSchema.GetEntryOrdinal(ConstFNUpdatedOn) > 0 Then
                            'rst.Fields(OTDBConst_UpdateOn).Value = aTimestamp
                            persistCommand.Parameters.Item("@" & ConstFNUpdatedOn).Value = timestamp
                        End If

                        '*** Create Timestamp
                        If Me.ContainerSchema.GetEntryOrdinal(ConstFNCreatedOn) > 0 And record.IsCreated Then
                            persistCommand.Parameters.Item("@" & ConstFNCreatedOn).Value = timestamp
                        ElseIf Me.ContainerSchema.GetEntryOrdinal(ConstFNCreatedOn) > 0 And Not record.IsCreated Then
                            If Not DBNull.Value.Equals(record.GetValue(ConstFNCreatedOn)) AndAlso Not record.GetValue(ConstFNCreatedOn) Is Nothing _
                                AndAlso record.GetValue(ConstFNCreatedOn) <> ConstNullDate Then
                                persistCommand.Parameters.Item("@" & ConstFNCreatedOn).Value = record.GetValue(ConstFNCreatedOn)    'keep the value
                            ElseIf Me.ContainerSchema.GetEntryOrdinal(ConstFNUpdatedOn) > 0 AndAlso Not DBNull.Value.Equals(record.GetValue(ConstFNUpdatedOn)) _
                                AndAlso Not record.GetValue(ConstFNUpdatedOn) Is Nothing AndAlso record.GetValue(ConstFNUpdatedOn) <> ConstNullDate Then
                                persistCommand.Parameters.Item("@" & ConstFNCreatedOn).Value = record.GetValue(ConstFNUpdatedOn)    'keep the value
                            Else
                                persistCommand.Parameters.Item("@" & ConstFNCreatedOn).Value = timestamp
                            End If
                        End If

                        '*** really update now
                        persistCommand.ExecuteNonQuery()
                        Return True
                    Else
                        Return True 'always true if no error
                    End If

                End SyncLock

                Exit Function
            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=silent, exception:=ex, argument:=persistCommand.CommandText, procedure:="adonetTableStore.PersistDirect", containerID:=Me.TableID, _
                                      messagetype:=otCoreMessageType.InternalException)
                Return False
            End Try



        End Function
    End Class
End Namespace