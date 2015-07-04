
REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** CORE ORM SQL Helper Classes for On Track Database Backend Library
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
Imports OnTrack.rulez
Imports OnTrack.rulez.eXPressionTree
Imports System.Text

Namespace OnTrack.Database

    ''' <summary>
    ''' an neutral SQL Command
    ''' </summary>
    ''' <remarks></remarks>

    Public Class ormSqlCommand
        Implements iormSqlCommand

        Private _ID As String = String.Empty  ' an Unique ID to store
        Protected _parameters As New Dictionary(Of String, ormSqlCommandParameter)
        Protected _parametervalues As New Dictionary(Of String, Object)

        Protected _type As otSQLCommandTypes
        Protected _SqlStatement As String = String.Empty
        Protected _SqlText As String = String.Empty ' the build SQL Text

        Protected _databaseDriver As iormRelationalDatabaseDriver
        Protected _tablestores As New Dictionary(Of String, iormRelationalTableStore)
        Protected _buildTextRequired As Boolean = True
        Protected _buildVersion As UShort = 0
        Protected _nativeCommand As System.Data.IDbCommand
        Protected _Prepared As Boolean = False

        Public Sub New(ID As String, Optional databasedriver As iormRelationalDatabaseDriver = Nothing)
            _ID = ID
            _databaseDriver = databasedriver
        End Sub

        Public Property ID As String Implements iormSqlCommand.ID
            Get
                Return _ID
            End Get
            Set(value As String)
                _ID = ID
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the database driver.
        ''' </summary>
        ''' <value>The database driver.</value>
        Public Property DatabaseDriver() As iormRelationalDatabaseDriver
            Get
                Return Me._databaseDriver
            End Get
            Set(value As iormRelationalDatabaseDriver)
                Me._databaseDriver = value
            End Set
        End Property
        ''' <summary>
        ''' returns the build version
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property BuildVersion As UShort Implements iormSqlCommand.BuildVersion
            Get
                Return _buildVersion
            End Get
        End Property
        ''' <summary>
        ''' returns a copy of the parameters list
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public ReadOnly Property Parameters As List(Of ormSqlCommandParameter) Implements iormSqlCommand.Parameters
            Get
                Return _parameters.Values.ToList
            End Get

        End Property
        ''' <summary>
        ''' set the Native Command
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property NativeCommand As System.Data.IDbCommand Implements iormSqlCommand.NativeCommand
            Set(value As System.Data.IDbCommand)
                _nativeCommand = value
            End Set
            Get
                Return _nativeCommand
            End Get
        End Property
        ''' <summary>
        ''' returns the build SQL Statement
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property SqlText As String Implements iormSqlCommand.SqlText
            Get
                If Not String.IsNullOrWhiteSpace(_SqlText) OrElse Me.BuildTextRequired Then
                    If Me.BuildTextRequired Then Call BuildSqlText()
                    Return _SqlText
                Else
                    Return _SqlStatement
                End If

            End Get
        End Property
        Public Property CustomerSqlStatement As String Implements iormSqlCommand.CustomerSqlStatement
            Get
                Return _SqlStatement
            End Get
            Set(value As String)
                _SqlStatement = value
                _SqlText = value
                Me.BuildTextRequired = False
            End Set
        End Property

        ''' <summary>
        ''' returns a copy of the table list
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TableIDs As List(Of String) Implements iormSqlCommand.TableIDs
            Get
                Return _tablestores.Keys.ToList()
            End Get

        End Property
        ''' <summary>
        ''' Type of the Sql Command -> Select, Delete etc.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property [Type] As otSQLCommandTypes Implements iormSqlCommand.Type
            Get
                Return _type
            End Get
        End Property
        ''' <summary>
        ''' True if the SQL Statement has to be build, false if it has been build
        ''' </summary>
        ''' <value>True</value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property BuildTextRequired As Boolean
            Set(value As Boolean)
                _buildTextRequired = value
            End Set
            Get
                Return _buildTextRequired
            End Get
        End Property
        ''' <summary>
        ''' True if the Native sql command is prepared
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsPrepared As Boolean
            Get
                Return _Prepared
            End Get
        End Property
        ''' <summary>
        ''' add a Parameter for the command
        ''' </summary>
        ''' <param name="parameter">a new Parameter</param>
        ''' <returns>true if successful</returns>
        ''' <remarks></remarks>
        Public Function AddParameter(parameter As ormSqlCommandParameter) As Boolean Implements iormSqlCommand.AddParameter

            '**
            '** some checking

            '** PARAMETER ID
            If String.IsNullOrWhiteSpace(parameter.ID) AndAlso String.IsNullOrWhiteSpace(parameter.ColumnName) AndAlso Not parameter.NotColumn Then
                Call CoreMessageHandler(procedure:="ormSqlCommand.AddParameter", argument:=Me.ID, message:=" id not set in parameter for sql command", messagetype:=otCoreMessageType.InternalError)
                Return False
            ElseIf String.IsNullOrWhiteSpace(parameter.ID) AndAlso Not String.IsNullOrWhiteSpace(parameter.ColumnName) AndAlso Not parameter.NotColumn Then
                parameter.ID = "@" & parameter.ColumnName
            ElseIf Not String.IsNullOrWhiteSpace(parameter.ID) Then
                parameter.ID = Regex.Replace(parameter.ID, "\s", String.Empty) ' no white chars allowed
            End If

            '** TABLENAME
            If Not parameter.NotColumn Then
                If Me.TableIDs.Count = 0 Then
                    Call CoreMessageHandler(procedure:="ormSqlCommand.AddParameter", argument:=Me.ID, _
                                          message:="no tablename  set in parameter for sql command", _
                                          messagetype:=otCoreMessageType.InternalError)
                    Return False
                ElseIf String.IsNullOrWhiteSpace(parameter.TableID) AndAlso Not String.IsNullOrWhiteSpace(Me.TableIDs(0)) Then
                    parameter.TableID = Me.TableIDs(0)
                    Call CoreMessageHandler(procedure:="ormSqlCommand.AddParameter", argument:=Me.ID, _
                                          message:=" tablename not set in parameter for sql command - first table used", _
                                          messagetype:=otCoreMessageType.InternalWarning, containerID:=Me.TableIDs(0))

                ElseIf String.IsNullOrWhiteSpace(parameter.TableID) AndAlso String.IsNullOrWhiteSpace(Me.TableIDs(0)) Then
                    Call CoreMessageHandler(procedure:="ormSqlCommand.AddParameter", argument:=Me.ID, _
                                          message:=" tablename not set in parameter for sql command - no default table", _
                                         messagetype:=otCoreMessageType.InternalError)

                    Return False
                End If
            End If

            '** fieldnames
            If String.IsNullOrWhiteSpace(parameter.ColumnName) AndAlso String.IsNullOrWhiteSpace(parameter.ID) Then
                Call CoreMessageHandler(procedure:="ormSqlCommand.AddParameter", argument:=Me.ID, _
                                      message:=" fieldname not set in parameter for sql command", _
                                      messagetype:=otCoreMessageType.InternalError)
                Return False
            ElseIf Not String.IsNullOrWhiteSpace(parameter.ColumnName) AndAlso String.IsNullOrWhiteSpace(parameter.ID) AndAlso Not parameter.NotColumn Then
                Call CoreMessageHandler(procedure:="ormSqlCommand.AddParameter", argument:=Me.ID, _
                                     message:=" fieldname not set in parameter for sql command - use ID without @", _
                                     messagetype:=otCoreMessageType.InternalWarning, containerID:=parameter.TableID, entryname:=parameter.ID)
                If parameter.ID.First = "@" Then
                    parameter.ColumnName = parameter.ID.Substring(2)
                Else
                    parameter.ColumnName = parameter.ID
                End If
            End If

            '** table name ?!
            If String.IsNullOrWhiteSpace(parameter.TableID) AndAlso Not parameter.NotColumn Then
                Call CoreMessageHandler(procedure:="ormSqlCommand.AddParameter", containerID:=parameter.TableID, _
                                      message:="table name is blank", argument:=parameter.ID)
                Return False
            End If
            If Not parameter.NotColumn AndAlso Not String.IsNullOrWhiteSpace(parameter.TableID) AndAlso Not GetPrimaryTableStore(parameter.TableID).ContainerSchema.IsInitialized Then
                Call CoreMessageHandler(procedure:="ormSqlCommand.AddParameter", containerID:=parameter.TableID, _
                                       message:="couldnot initialize table schema")
                Return False
            End If

            If Not parameter.NotColumn AndAlso Not Me._tablestores.ContainsKey(parameter.TableID) Then
                Call CoreMessageHandler(procedure:="ormSqlCommand.AddParameter", argument:=Me.ID, entryname:=parameter.ID, _
                                      message:=" tablename of parameter is not used in sql command", _
                                  messagetype:=otCoreMessageType.InternalError, containerID:=parameter.TableID)
                Return False
            ElseIf Not parameter.NotColumn AndAlso Not Me._tablestores.Item(key:=parameter.TableID).ContainerSchema.HasEntryName(parameter.ColumnName) Then
                Call CoreMessageHandler(procedure:="ormSqlCommand.AddParameter", argument:=Me.ID, entryname:=parameter.ColumnName, _
                                     message:=" fieldname of parameter is not used in table schema", _
                                 messagetype:=otCoreMessageType.InternalError, containerID:=parameter.TableID)
                Return False

            End If


            ''' datatype
            If parameter.NotColumn And parameter.Datatype = 0 Then
                Call CoreMessageHandler(procedure:="ormSqlCommand.AddParameter", _
                                      argument:=Me.ID, message:=" datatype not set in parameter for sql command", _
                                      messagetype:=otCoreMessageType.InternalError)
                Return False
                ''' datatype lookup
            ElseIf Not parameter.NotColumn AndAlso parameter.Datatype = 0 Then

                ''' look up internally first
                ''' 
                Dim anAttribute As ormContainerEntryAttribute = ot.GetSchemaTableColumnAttribute(tableid:=parameter.TableID, columnname:=parameter.ColumnName)
                If anAttribute IsNot Nothing AndAlso anAttribute.HasValueDataType Then
                    parameter.Datatype = anAttribute.DataType
                End If
                ''' datatype still not resolved
                If parameter.Datatype = 0 Then
                    Dim aSchemaEntry As iormContainerEntryDefinition = CurrentSession.Objects.GetContainerEntry(entryname:=parameter.ColumnName, containerid:=parameter.TableID)
                    If aSchemaEntry IsNot Nothing Then parameter.Datatype = aSchemaEntry.Datatype

                End If
            End If

            '** add the paramter
            If _parameters.ContainsKey(key:=parameter.ID) Then
                _parameters.Remove(key:=parameter.ID)
            End If
            _parameters.Add(key:=parameter.ID, value:=parameter)
            Return True
        End Function

        ''' <summary>
        ''' Add Table 
        ''' </summary>
        ''' <param name="tableid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function AddTable(tableid As String) As Boolean
            Dim aTablestore As iormRelationalTableStore
            tableid = tableid.ToUpper
            If Me._databaseDriver Is Nothing Then
                aTablestore = GetPrimaryTableStore(tableid:=tableid)
                If aTablestore Is Nothing Then
                    Call CoreMessageHandler(message:="Tablestore couldnot be retrieved", containerID:=tableid, procedure:="clsOTDBSelectCommand.ADDTable", _
                                          messagetype:=otCoreMessageType.InternalError)
                    Return False
                Else
                    Me.DatabaseDriver = aTablestore.Connection.DatabaseDriver
                End If
            Else
                aTablestore = _databaseDriver.GetTableStore(tableID:=tableid)
            End If


            If aTablestore Is Nothing Then
                Call CoreMessageHandler(message:="Tablestore couldnot be retrieved", containerID:=tableid, procedure:="clsOTDBSelectCommand.ADDTable", _
                                      messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            If Not _tablestores.ContainsKey(key:=tableid) Then
                _tablestores.Add(key:=tableid, value:=aTablestore)
            End If

            Return True
        End Function
        ''' Sets the parameter value.
        ''' </summary>
        ''' <param name="name">The name of the parameter.</param>
        ''' <param name="value">The value of the object</param>
        ''' <returns></returns>
        Public Function SetParameterValue(ID As String, [value] As Object) As Boolean Implements iormSqlCommand.SetParameterValue
            If Not _parameters.ContainsKey(key:=ID) Then
                Call CoreMessageHandler(message:="Parameter ID not in Command", argument:=Me.ID, entryname:=ID, procedure:="ormSqlCommand.SetParameterValue", _
                                      messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            ID = Regex.Replace(ID, "\s", String.Empty) ' no white chars allowed
            If _parametervalues.ContainsKey(key:=ID) Then
                _parametervalues.Remove(key:=ID)
            End If

            _parametervalues.Add(key:=ID, value:=[value])

            Return True
        End Function
        ''' <summary>
        ''' returns True if the Command has the parameter
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasParameter(ID As String) As Boolean Implements iormSqlCommand.HasParameter
            ID = Regex.Replace(ID, "\s", String.Empty) ' no white chars allowed
            If Not _parameters.ContainsKey(key:=ID) Then
                Return False
            Else
                Return True
            End If
        End Function
        ''' Sets the parameter value.
        ''' </summary>
        ''' <param name="name">The name of the parameter.</param>
        ''' <param name="value">The value of the object</param>
        ''' <returns></returns>
        Public Function GetParameterValue(ID As String) As Object Implements iormSqlCommand.GetParameterValue
            ID = Regex.Replace(ID, "\s", String.Empty) ' no white chars allowed
            If Not _parameters.ContainsKey(key:=ID) Then
                Call CoreMessageHandler(message:="Parameter ID not in Command", argument:=Me.ID, entryname:=ID, procedure:="ormSqlCommand.SetParameterValue", _
                                      messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If

            If _parametervalues.ContainsKey(key:=ID) Then
                Return _parametervalues.Item(key:=ID)
            Else
                Dim aParameter As ormSqlCommandParameter = _parameters.Item(key:=ID)
                Return aParameter.Value
            End If

        End Function
        ''' <summary>
        ''' builds the SQL text for the Command
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function BuildSqlText() As String
            IncBuildVersion()
            _SqlText = _SqlStatement ' simple
            Return _SqlText
        End Function
        ''' <summary>
        ''' prepares the command. returns true if successfull
        ''' </summary>
        ''' <returns>True if successfull</returns>
        ''' <remarks></remarks>
        Public Overridable Function Prepare() As Boolean Implements iormSqlCommand.Prepare
            Dim aNativeConnection As System.Data.IDbConnection
            Dim aNativeCommand As System.Data.IDbCommand
            Dim cvtvalue As Object
            Dim aTablestore As iormRelationalTableStore

            If Me.DatabaseDriver Is Nothing And ot.IsConnected Then
                Me.DatabaseDriver = CurrentOTDBDriver
                aNativeConnection = CurrentOTDBDriver.CurrentConnection.NativeConnection
            ElseIf Me.DatabaseDriver Is Nothing Then
                Call CoreMessageHandler(procedure:="ormSqlCommand.Prepare", argument:=Me.ID, message:="database driver missing", _
                                            messagetype:=otCoreMessageType.InternalError)
                Return False
            ElseIf Me.DatabaseDriver.CurrentConnection Is Nothing Then
                Call CoreMessageHandler(procedure:="ormSqlCommand.Prepare", argument:=Me.ID, message:="driver is not connected or connection is missing", _
                                            messagetype:=otCoreMessageType.InternalError)
                Return False
            Else
                aNativeConnection = DatabaseDriver.CurrentConnection.NativeConnection
            End If

            Try
                Dim aSqlText As String
                '** Build the Sql String
                If Me.BuildTextRequired Then
                    aSqlText = Me.BuildSqlText()
                Else
                    aSqlText = Me.SqlText
                End If
                '**
                If String.IsNullOrWhiteSpace(aSqlText) Then
                    Call CoreMessageHandler(message:="No SQL statement could be build", argument:=Me.ID, _
                                           procedure:="ormSqlCommand.Prepare", _
                                           messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If
                'DatabaseDriver.StoreSqlCommand(me)
                aNativeCommand = _databaseDriver.CreateNativeDBCommand(aSqlText, aNativeConnection)
                Me.NativeCommand = aNativeCommand
                '** prepare
                aNativeCommand.CommandText = aSqlText
                If aNativeCommand.Connection Is Nothing Then
                    aNativeCommand.Connection = aNativeConnection
                End If

                aNativeCommand.CommandType = Data.CommandType.Text
                '** add Parameter
                For Each aParameter In Me.Parameters
                    '** add Column Parameter

                    If Not aParameter.NotColumn And aParameter.TableID <> String.Empty And aParameter.ColumnName <> String.Empty Then
                        aTablestore = _databaseDriver.GetTableStore(aParameter.TableID)
                        If Not aTablestore.ContainerSchema.IsInitialized Then
                            Call CoreMessageHandler(procedure:="ormSqlCommand.Prepare", containerID:=aParameter.TableID, _
                                                   message:="couldnot initialize table schema")
                            Return False
                        End If
                        Dim aNativeParameter As System.Data.IDbDataParameter = _
                            TryCast(aTablestore.ContainerSchema, iormRelationalSchema).AssignNativeDBParameter(columnname:=aParameter.ColumnName, parametername:=aParameter.ID)
                        If Not aParameter Is Nothing Then aNativeCommand.Parameters.Add(aNativeParameter)
                    ElseIf aParameter.NotColumn Then
                        Dim aNativeParameter As System.Data.IDbDataParameter = _
                           _databaseDriver.AssignNativeDBParameter(parametername:=aParameter.ID, datatype:=aParameter.Datatype)
                        If Not aParameter Is Nothing Then aNativeCommand.Parameters.Add(aNativeParameter)
                    Else
                        Call CoreMessageHandler(procedure:="ormSqlCommand.Prepare", argument:=aParameter.ID, message:="Tablename missing", _
                                              entryname:=aParameter.ColumnName, messagetype:=otCoreMessageType.InternalError)
                    End If
                Next
                '** prepare the native
                aNativeCommand.Prepare()
                Me._Prepared = True
                '** initial values
                aTablestore = Nothing ' reset
                For Each aParameter In Me.Parameters
                    If aParameter.ColumnName <> String.Empty And aParameter.TableID <> String.Empty Then
                        If aTablestore Is Nothing OrElse aTablestore.ContainerID <> aParameter.TableID Then
                            aTablestore = _databaseDriver.GetTableStore(aParameter.TableID)
                        End If
                        If Not aTablestore.Convert2ContainerData(aParameter.ColumnName, invalue:=aParameter.Value, outvalue:=cvtvalue) Then
                            Call CoreMessageHandler(message:="parameter value could not be converted", containerEntryName:=aParameter.ColumnName, _
                                                    entryname:=aParameter.ID, argument:=aParameter.Value, messagetype:=otCoreMessageType.InternalError, _
                                                    procedure:="ormSqlCommand.Prepare")
                        End If
                    Else
                        cvtvalue = aParameter.Value
                    End If
                    If aNativeCommand.Parameters.Contains(aParameter.ID) Then
                        aNativeCommand.Parameters(aParameter.ID).value = cvtvalue
                    Else
                        Call CoreMessageHandler(message:="Parameter ID is not in native sql command", entryname:=aParameter.ID, argument:=Me.ID, _
                                               messagetype:=otCoreMessageType.InternalError, procedure:="ormSqlCommand.Prepare")

                    End If

                Next

                Return True

            Catch ex As OleDb.OleDbException
                Me._Prepared = False
                Call CoreMessageHandler(procedure:="ormSqlCommand.Prepare", message:="Exception", argument:=Me.ID, _
                                       exception:=ex, messagetype:=otCoreMessageType.InternalException)
                Return False
            Catch ex As Exception
                Me._Prepared = False
                Call CoreMessageHandler(procedure:="ormSqlCommand.Prepare", message:="Exception", argument:=Me.ID, _
                                       exception:=ex, messagetype:=otCoreMessageType.InternalException)
                Return False
            End Try




        End Function
        ''' <summary>
        ''' increase the buildVersion
        ''' </summary>
        ''' <returns>the new build version</returns>
        ''' <remarks></remarks>
        Protected Function IncBuildVersion() As UShort
            Return (_buildVersion = _buildVersion + 1)
        End Function
        ''' <summary>
        ''' Run the Sql Select Statement and returns a List of ormRecords
        ''' </summary>
        ''' <param name="parameters">parameters of value</param>
        ''' <param name="connection">a optional native connection</param>
        ''' <returns>list of ormRecords (might be empty)</returns>
        ''' <remarks></remarks>
        Public Function Run(Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing, _
                                           Optional nativeConnection As Object = Nothing) As Boolean
            '** set the parameters value to current command parameters value 
            '** if not specified
            Dim aParametervalues As Dictionary(Of String, Object)
            If parametervalues Is Nothing Then
                aParametervalues = _parametervalues
            Else
                aParametervalues = parametervalues
            End If

            ''' if we are running on one table only with all fields
            ''' then use the tablestore select with type checking

            ''' else run against the database driver
            ''' 
            '*** run it 
            If Me.IsPrepared Then
                Return Me.DatabaseDriver.RunSqlCommand(sqlcommand:=Me, parametervalues:=aParametervalues, nativeConnection:=nativeConnection)
            Else
                If Me.Prepare() Then
                    Return Me.DatabaseDriver.RunSqlCommand(sqlcommand:=Me, parametervalues:=aParametervalues, nativeConnection:=nativeConnection)
                Else
                    Call CoreMessageHandler(procedure:="clsOTDBSqlSelectCommand.run", message:="Command is not prepared", argument:=Me.ID, _
                                                     messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If
            End If

        End Function
    End Class



    '*******************************************************************************************
    '***** CLASS clsOTDBStoreParameter  defines a Parameter for SQL Commands
    '*****
    ''' <summary>
    ''' Parameter definition for a SQL Command
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ormSqlCommandParameter

        Private _ID As String = String.Empty
        Private _NotColumn As Boolean = False
        Private _tablename As String = Nothing
        Private _columname As String = Nothing
        Private _datatype As otDataType = 0
        Private _value As Object

        ''' <summary>
        ''' constructor for a Sql Command parameter
        ''' </summary>
        ''' <param name="ID">the ID in the sql statement</param>
        ''' <param name="datatype">datatype </param>
        ''' <param name="fieldname">fieldname </param>
        ''' <param name="tablename">tablename</param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ID As String, _
                       Optional datatype As otDataType = 0, _
                       Optional columnname As String = Nothing, _
                       Optional tableid As String = Nothing, _
                       Optional value As Object = Nothing, _
                       Optional notColumn As Boolean = False)
            _ID = Regex.Replace(ID, "\s", String.Empty) ' no white chars allowed
            _datatype = datatype
            If Not String.IsNullOrWhiteSpace(columnname) Then _columname = columnname.ToUpper
            If Not String.IsNullOrWhiteSpace(tableid) Then _tablename = tableid.ToUpper
            If Not value Is Nothing Then _value = value
            _NotColumn = notColumn
        End Sub
        ''' <summary>
        ''' Gets or sets the not column.
        ''' </summary>
        ''' <value>The not column.</value>
        Public Property NotColumn() As Boolean
            Get
                Return Me._NotColumn
            End Get
            Set(value As Boolean)
                Me._NotColumn = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value.
        ''' </summary>
        ''' <value>The value.</value>
        Public Property Value() As Object
            Get
                Return Me._value
            End Get
            Set(value As Object)
                Me._value = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the datatype.
        ''' </summary>
        ''' <value>The datatype.</value>
        Public Property Datatype() As otDataType
            Get
                Return Me._datatype
            End Get
            Set(value As otDataType)
                Me._datatype = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the fieldname.
        ''' </summary>
        ''' <value>The fieldname.</value>
        Public Property ColumnName() As String
            Get
                Return Me._columname
            End Get
            Set(value As String)
                Me._columname = value.ToUpper
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the fieldname.
        ''' </summary>
        ''' <value>The fieldname.</value>
        Public Property TableID() As String
            Get
                Return Me._tablename
            End Get
            Set(value As String)
                Me._tablename = value.ToUpper
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the ID.
        ''' </summary>
        ''' <value>The name.</value>
        Public Property ID() As String
            Get
                Return Me._ID
            End Get
            Set(value As String)
                Me._ID = Regex.Replace(ID, "\s", String.Empty) ' no white chars allowed
            End Set
        End Property

    End Class

    '************************************************************************************
    '*****  CLASS clsOTDBSelectCommand 
    '***** 
    '*****
    Public Enum ormSelectResultFieldType
        TableField
        InLineFunction
    End Enum
    ''' <summary>
    '''  a flexible Select Command
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ormSqlSelectCommand
        Inherits ormSqlCommand
        Implements iormSqlCommand

        'Private _tablestores As New Dictionary(Of String, iOTDBTableStore) 'store the used Tablestores
        Private _fields As New Dictionary(Of String, ormResultField)

        Private _select As String = String.Empty
        Private _innerjoin As String = String.Empty
        Private _orderby As String = String.Empty
        Private _where As String = String.Empty
        Private _AllFieldsAdded As Boolean



        ''' <summary>
        ''' Class for Storing the select result fields per Table(store)
        ''' </summary>
        ''' <remarks></remarks>
        Public Class ormResultField
            Implements IHashCodeProvider

            Private _myCommand As ormSqlSelectCommand ' Backreference
            Private _name As String
            Private _tablestore As iormRelationalTableStore
            Private _type As ormSelectResultFieldType


            ''' <summary>
            ''' constructs a new Result field for command
            ''' </summary>
            ''' <param name="aCommand"></param>
            ''' <remarks></remarks>
            Public Sub New(command As ormSqlSelectCommand)
                _myCommand = command
            End Sub
            ''' <summary>
            ''' constructs a new resultfield for command 
            ''' </summary>
            ''' <param name="aCommand"></param>
            ''' <param name="tableid"></param>
            ''' <param name="fieldname"></param>
            ''' <remarks></remarks>
            Public Sub New(command As ormSqlSelectCommand, tableid As String, fieldname As String)
                _myCommand = command
                Me.[TableID] = tableid
                _name = fieldname
            End Sub
            ''' <summary>
            ''' Gets or sets the name.
            ''' </summary>
            ''' <value>The name.</value>
            Public Property Name() As String
                Get
                    Return Me._name
                End Get
                Set(value As String)
                    Me._name = value
                End Set
            End Property
            ''' <summary>
            ''' Gets or sets the name.
            ''' </summary>
            ''' <value>The name.</value>
            Public Property [Type]() As ormSelectResultFieldType
                Get
                    Return Me._type
                End Get
                Set(value As ormSelectResultFieldType)
                    Me._type = value
                End Set
            End Property
            ''' <summary>
            ''' Gets or sets the Tablestore used
            ''' </summary>
            ''' <value>The name.</value>
            Public Property [Tablestore]() As iormRelationalTableStore
                Get
                    Return Me._tablestore
                End Get
                Set(value As iormRelationalTableStore)
                    Me._tablestore = value
                    If _myCommand.DatabaseDriver Is Nothing Then
                        _myCommand.DatabaseDriver = value.Connection.DatabaseDriver
                    End If
                End Set
            End Property

            ''' <summary>
            ''' returns the nativetablename if a tablestore is set
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public ReadOnly Property [NativeTablename] As String
                Get
                    If _tablestore IsNot Nothing Then
                        Return _tablestore.NativeDBObjectname
                    End If
                    Return String.Empty
                End Get
            End Property
            ''' <summary>
            ''' Gets or sets the Tablestore / Tablename.
            ''' </summary>
            ''' <value>The name.</value>
            Public Property [TableID]() As String
                Get
                    If _tablestore Is Nothing Then
                        Return String.Empty
                    Else
                        Return _tablestore.ContainerID
                    End If

                End Get
                Set(value As String)
                    Dim aTablestore As iormRelationalTableStore
                    '** set it to current connection 
                    If Not _myCommand.DatabaseDriver Is Nothing Then
                        _myCommand.DatabaseDriver = ot.CurrentConnection.DatabaseDriver
                    End If
                    ' retrieve the tablestore
                    If Not _myCommand._tablestores.ContainsKey(key:=value) Then
                        ' add it
                        aTablestore = Me._myCommand.DatabaseDriver.GetTableStore(tableID:=value)
                        If aTablestore IsNot Nothing Then
                            _myCommand._tablestores.Add(key:=aTablestore.ContainerID, value:=aTablestore)
                        End If
                    Else
                        aTablestore = _myCommand._tablestores.Item(value)
                    End If
                    _tablestore = aTablestore ' set it
                End Set
            End Property


            ''' <summary>
            ''' Returns a hash code for the specified object.
            ''' </summary>
            ''' <param name="obj">The <see cref="T:System.Object" /> for which a hash code is
            ''' to be returned.</param>
            ''' <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj" />
            ''' is a reference type and <paramref name="obj" /> is null. </exception>
            ''' <returns>A hash code for the specified object.</returns>
            Public Function GetHashCode(obj As Object) As Integer Implements IHashCodeProvider.GetHashCode
                Return (Me.[TableID] & _name).GetHashCode
            End Function

        End Class

        ''' <summary>
        ''' Constructor of the OTDB Select command
        ''' </summary>
        ''' <param name="ID">the unique ID to store it</param>
        ''' <remarks></remarks>
        Public Sub New(ID As String)
            Call MyBase.New(ID:=ID)
            _type = otSQLCommandTypes.SELECT
        End Sub
        ''' <summary>
        ''' Gets the completefor object.
        ''' </summary>
        ''' <value>The completefor object.</value>
        Public ReadOnly Property AllFieldsAdded() As Boolean
            Get
                Return Me._AllFieldsAdded
            End Get
        End Property
        ''' <summary>
        ''' sets or gets the innerjoin 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property InnerJoin As String
            Get
                Return _innerjoin
            End Get
            Set(value As String)
                _innerjoin = value
                Me.BuildTextRequired = True
            End Set
        End Property
        ''' <summary>
        '''  sets the select part of an Sql Select without SELECT Keyword
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property [select] As String
            Get
                Return _select
            End Get
            Set(value As String)
                _select = value
                Me.BuildTextRequired = True

            End Set
        End Property
        ''' <summary>
        ''' set or gets the orderby string
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property OrderBy As String
            Get
                Return _orderby
            End Get
            Set(value As String)
                _orderby = value
                Me.BuildTextRequired = True
            End Set
        End Property
        ''' <summary>
        ''' sets or gets the wherestr
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Where As String
            Get
                Return _where
            End Get
            Set(value As String)

                _where = value
                Me.BuildTextRequired = True
            End Set
        End Property

        ''' <summary>
        ''' Add Table with fields to the Resultfields
        ''' </summary>
        ''' <param name="tableid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddTable(tableid As String, addAllFields As Boolean, Optional addFieldnames As List(Of String) = Nothing) As Boolean
            Dim aTablestore As iormRelationalTableStore
            tableid = tableid.ToUpper
            If Me._databaseDriver Is Nothing Then
                aTablestore = GetPrimaryTableStore(tableid:=tableid)
                If aTablestore Is Nothing Then
                    Call CoreMessageHandler(message:="Tablestore couldnot be retrieved", containerID:=tableid, procedure:="clsOTDBSelectCommand.ADDTable", _
                                          messagetype:=otCoreMessageType.InternalError)
                    Return False
                Else
                    Me.DatabaseDriver = aTablestore.Connection.DatabaseDriver
                End If
            Else
                aTablestore = _databaseDriver.GetTableStore(tableID:=tableid)
            End If


            If aTablestore Is Nothing Then
                Call CoreMessageHandler(message:="Tablestore couldnot be retrieved", containerID:=tableid, procedure:="clsOTDBSelectCommand.ADDTable", _
                                      messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            If Not _tablestores.ContainsKey(key:=tableid) Then
                _tablestores.Add(key:=tableid, value:=aTablestore)
            End If

            '*** include all fields
            If addAllFields Then
                For Each aFieldname As String In aTablestore.ContainerSchema.EntryNames
                    If Not _fields.ContainsKey(key:=tableid & "." & aFieldname.ToUpper) Then
                        _fields.Add(key:=tableid & "." & aFieldname.ToUpper, value:=New ormResultField(Me, tableid:=tableid, fieldname:=aFieldname.ToUpper))
                    End If
                Next
                _AllFieldsAdded = True
            End If

            '** include specific fields
            If Not addFieldnames Is Nothing Then
                For Each aFieldname As String In addFieldnames
                    If Not _fields.ContainsKey(key:=tableid & "." & aFieldname.ToUpper) Then
                        _fields.Add(key:=tableid & "." & aFieldname, value:=New ormResultField(Me, tableid:=tableid, fieldname:=aFieldname.ToUpper))
                    End If
                Next
            End If

            Return True
        End Function
        ''' <summary>
        ''' builds the SQL text for the Command
        ''' </summary>
        ''' <returns>True if successfull </returns>
        ''' <remarks></remarks>
        Public Overrides Function BuildSqlText() As String
            Me._SqlText = "SELECT "
            Dim aTableList As New List(Of String)
            Dim first As Boolean = True

            '** fill tables first 
            For Each atableid In _tablestores.Keys
                'Dim aTablename = kvp.Key
                If Not aTableList.Contains(atableid) Then
                    aTableList.Add(atableid)
                End If
            Next

            '*** build the result list
            If String.IsNullOrWhiteSpace(_select) Then
                first = True
                '*
                For Each aResultField In _fields.Values
                    Dim aTablename = aResultField.[TableID]
                    If Not String.IsNullOrWhiteSpace(aTablename) Then
                        If Not aTableList.Contains(aTablename) Then aTableList.Add(aTablename)

                        If Not first Then Me._SqlText &= ","
                        Me._SqlText &= "[" & aResultField.NativeTablename & "].[" & aResultField.Name & "] "
                    Else
                        Me._SqlText &= "[" & aResultField.Name & "] "
                    End If

                    first = False
                Next

                If aTableList.Count = 0 Then
                    Call CoreMessageHandler(message:="no table and no fields in sql statement", procedure:="clsOTDBSqlSelectCommand.BuildSqlText", _
                                           argument:=Me.ID, messagetype:=otCoreMessageType.InternalError)
                    Me.BuildTextRequired = True
                    Return String.Empty
                End If
            Else
                ''' TODO: add the additional parameter sql text
                ''' and keep allfieldsadded
                Me._SqlText &= _select
                If _AllFieldsAdded Then _AllFieldsAdded = False ' reset the allfieldsadded in any case
            End If

            '*** build the tables
            first = True
            Me._SqlText &= " FROM "
            For Each aTableID In aTableList

                '** if innerjoin has the tablename
                If String.IsNullOrWhiteSpace(_innerjoin) OrElse _
                    (Not String.IsNullOrWhiteSpace(_innerjoin) AndAlso Not _innerjoin.ToUpper.Contains(aTableID)) Then
                    If Not first Then
                        Me._SqlText &= ","
                    End If
                    Me._SqlText &= "[" & Me.DatabaseDriver.GetNativeDBObjectName(aTableID) & "] AS \"" & aTableID & " \ ""
                    first = False
                End If
            Next

            '*** innerjoin
            If Not String.IsNullOrWhiteSpace(_innerjoin) Then
                If Not _innerjoin.ToLower.Contains("join") Then
                    Me._SqlText &= " INNER JOIN "
                End If
                _SqlText &= _innerjoin
            End If

            '*** where 
            If _where <> String.Empty Then
                If Not _where.ToLower.Contains("where") Then
                    Me._SqlText &= " WHERE "
                End If
                _SqlText &= _where
            End If

            '*** order by 
            If _orderby <> String.Empty Then
                If Not _where.ToLower.Contains("order by") Then
                    Me._SqlText &= " ORDER BY "
                End If
                Me._SqlText &= _orderby
            End If

            '*
            IncBuildVersion()
            Me.BuildTextRequired = False
            '*
            Return Me._SqlText
        End Function
        ''' <summary>
        ''' Run the Sql Select Statement and returns a List of ormRecords
        ''' </summary>
        ''' <param name="parameters">parameters of value</param>
        ''' <param name="connection">a optional native connection</param>
        ''' <returns>list of ormRecords (might be empty)</returns>
        ''' <remarks></remarks>
        Public Function RunSelect(Optional ByRef parametervalues As Dictionary(Of String, Object) = Nothing, _
                                  Optional nativeConnection As Object = Nothing) As List(Of ormRecord)
            '** set the parameters value to current command parameters value 
            '** if not specified
            Dim aParametervalues As Dictionary(Of String, Object)
            If parametervalues Is Nothing Then
                aParametervalues = _parametervalues
            Else
                aParametervalues = parametervalues
            End If

            ''' if we are running on one table only with all fields
            ''' then use the tablestore select with type checking
            If _tablestores.Count = 1 And _AllFieldsAdded Then
                Dim aStore As iormRelationalTableStore = _tablestores.Values.First
                '*** run it
                If Me.IsPrepared Then
                    Return aStore.GetRecordsBySqlCommand(sqlcommand:=Me, parametervalues:=aParametervalues)
                Else
                    If Me.Prepare() Then
                        Return aStore.GetRecordsBySqlCommand(sqlcommand:=Me, parametervalues:=aParametervalues)
                    Else
                        Call CoreMessageHandler(procedure:="clsOTDBSqlSelectCommand.runSelect", message:="Command is not prepared", argument:=Me.ID, _
                                                         messagetype:=otCoreMessageType.InternalError)
                        Return New List(Of ormRecord)
                    End If
                End If
            Else
                ''' else run against the database driver
                ''' 
                '*** run it
                If Me.IsPrepared Then
                    Return Me.DatabaseDriver.RunSqlSelectCommand(sqlcommand:=Me, parametervalues:=aParametervalues, nativeConnection:=nativeConnection)
                Else
                    If Me.Prepare() Then
                        Return Me.DatabaseDriver.RunSqlSelectCommand(sqlcommand:=Me, parametervalues:=aParametervalues, nativeConnection:=nativeConnection)
                    Else
                        Call CoreMessageHandler(procedure:="clsOTDBSqlSelectCommand.runSelect", message:="Command is not prepared", argument:=Me.ID, _
                                                         messagetype:=otCoreMessageType.InternalError)
                        Return New List(Of ormRecord)
                    End If
                End If
            End If

        End Function
    End Class

    ''' <summary>
    ''' an extension for selection rules to convert them to sql select comands
    ''' </summary>
    ''' <remarks></remarks>
    Module SQLRulezExtension

        ''' <summary>
        ''' builds a sql statement string out of a selection rule
        ''' </summary>
        ''' <param name="rulez"></param>
        ''' <remarks></remarks>
        <Extension()> _
        Public Function ToSQL(rule As SelectionRule, Optional dbdriver As iormDatabaseDriver = Nothing, Optional deletebehavior As Boolean? = Nothing, Optional domainbehavior As Boolean? = Nothing) As String
            Dim aSqlText As String
            Dim aVisitor As IVisitor = dbdriver.GetIXPTVisitor()

            ''' run the visitor on the expression tree
            ''' 
            aVisitor.Visit(rule)
            aSqlText = aVisitor.Result

            '        ' build the key part
            '        For i = 0 To arelationAttribute.ToEntries.Count - 1
            '            If i > 0 Then wherekey &= " AND "
            '            '** if where is run against select of datatable the tablename is creating an error
            '            wherekey &= "[" & arelationAttribute.ToEntries(i) & "] = @" & arelationAttribute.ToEntries(i)
            '        Next
            '        aCommand.Where = wherekey
            '        If arelationAttribute.HasValueLinkJOin Then
            '            aCommand.Where &= " " & arelationAttribute.LinkJoin
            '        End If
            '        '** additional behavior
            '        If deletebehavior Then aCommand.Where &= " AND " & FNDeleted & " = @deleted "
            '        If domainBehavior Then aCommand.Where &= " AND ([" & FNDomainID & "] = @domainID OR [" & FNDomainID & "] = @globalID)"

            '        '** parameters
            '        For i = 0 To arelationAttribute.ToEntries.Count - 1
            '            aCommand.AddParameter(New ormSqlCommandParameter(ID:="@" & arelationAttribute.ToEntries(i), columnname:=arelationAttribute.ToEntries(i), _
            '                                                             tableid:=ToTableID))
            '        Next
            '        If deletebehavior Then aCommand.AddParameter(New ormSqlCommandParameter(ID:="@deleted", ColumnName:=FNDeleted, tableid:=ToTableID))
            '        If domainBehavior Then aCommand.AddParameter(New ormSqlCommandParameter(ID:="@domainID", ColumnName:=FNDomainID, tableid:=ToTableID))
            '        If domainBehavior Then aCommand.AddParameter(New ormSqlCommandParameter(ID:="@globalID", ColumnName:=FNDomainID, tableid:=ToTableID))
            '        aCommand.Prepare()
            '    End If
            '    '** parameters
            '    For i = 0 To arelationAttribute.ToEntries.Count - 1
            '        aCommand.SetParameterValue(ID:="@" & arelationAttribute.ToEntries(i), value:=theKeyvalues(i))
            '    Next
            '    '** set the values
            '    If aCommand.HasParameter(ID:="@deleted") Then aCommand.SetParameterValue(ID:="@deleted", value:=False)
            '    If aCommand.HasParameter(ID:="@domainID") Then aCommand.SetParameterValue(ID:="@domainID", value:=domainID)
            '    If aCommand.HasParameter(ID:="@globalID") Then aCommand.SetParameterValue(ID:="@globalID", value:=ConstGlobalDomain)

            Return aSqlText.ToString
        End Function
        ''' <summary>
        ''' builds a sql select command out of a selection rule
        ''' </summary>
        ''' <param name="rulez"></param>
        ''' <remarks></remarks>
        <Extension()> _
        Public Function ToSQLSelectCommand(rule As SelectionRule, Optional id As String = Nothing, Optional domainid As String = Nothing) As ormSqlSelectCommand
            ''' defaults
            If id Is Nothing Then id = New Guid().ToString
            If domainid Is Nothing Then domainid = CurrentSession.CurrentDomainID

            Dim dbdriver As iormDatabaseDriver
            Dim domainbehavior As Boolean
            Dim deletebehavior As Boolean
            Dim aCommand As ormSqlSelectCommand
            Dim containerIDs As String()
            Dim parameterColumnNames As String()
            Dim primaryContainerID As String
            Dim anObjectId As String = rule.ResultingObjectnames.FirstOrDefault

            ''' Hack
            ''' 
            If (rule.ResultingObjectnames.Count > 1) Then
                Throw New NotImplementedException("creating sql select commands with more than on objects is not implemented yet")

            End If
            Try
                ''' get some information

              
                    Dim anObjectDefinition As iormObjectDefinition = ot.CurrentSession.Objects(domainid).GetObjectDefinition(id:=anObjectId)
                    If anObjectDefinition Is Nothing Then
                        CoreMessageHandler(message:="object not found by object id", objectname:=anObjectId, procedure:="SQLRulezExtension.ToSQLSelectcommand")
                        Return Nothing
                    End If
                    domainbehavior = anObjectDefinition.HasDomainBehavior
                    deletebehavior = anObjectDefinition.HasDeleteFieldBehavior
                    primaryContainerID = anObjectDefinition.PrimaryContainerID
                    If dbdriver Is Nothing Then dbdriver = CurrentSession.GetPrimaryDatabaseDriver(containerID:=primaryContainerID)
                    containerIDs = anObjectDefinition.ContainerIDs

                ''' no db relational driver ?
                If dbdriver Is Nothing OrElse Not dbdriver.IsRelationalDriver Then
                    CoreMessageHandler(message:="could not retrieve relational database driver for object", objectname:=anObjectId, _
                                       procedure:="SQLRulezExtension.ToSQLSelectCommand", messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If

                '** get a Store
                Dim aStore As iormRelationalTableStore = CType(dbdriver, iormRelationalDatabaseDriver).GetTableStore(primaryContainerID)
                aCommand = aStore.CreateSqlSelectCommand(id:=id, addAllFields:=False)
                If Not aCommand.IsPrepared Then
                    ''' retrieve the visitor from the dbdriver
                    ''' 
                    Dim aVisitor As IRDBVisitor = CType(dbdriver, iormRelationalDatabaseDriver).GetIRDBVisitor()
                    ''' run the visitor on the expression tree
                    ''' 
                    aVisitor.Visit(rule)
                    ''' add parameters
                    ''' 
                    For Each aParameter In aVisitor.Parameters
                        aCommand.AddParameter(aParameter)
                    Next
                    ''' extract the sql statement
                    ''' 
                    aCommand.select = aVisitor.Select
                    aCommand.Where = aVisitor.Result
                    ''' prepare the command
                    ''' 
                    aCommand.Prepare()
                End If

                ''' return the command
                Return aCommand

            Catch ex As RulezException
                CoreMessageHandler(message:=ex.Message, argument:=id, procedure:="SQLRulezExtension.ToSQLSelectCommand", exception:=ex)
                Return Nothing
            Catch ex As Exception
                CoreMessageHandler(message:=ex.Message, argument:=id, procedure:="ASTTree.ToSQLSelectCommand", exception:=ex)
                Return Nothing
            End Try


        End Function
    End Module
End Namespace