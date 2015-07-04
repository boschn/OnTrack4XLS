REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** Driver Wrapper for ADO.NET MS SQL Classes for On Track Database Backend Library
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
Imports System.Collections
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Diagnostics.Debug
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports Microsoft.SqlServer.Management.Smo
Imports Microsoft.SqlServer.Management.Common
Imports System.Text


Imports OnTrack
Imports OnTrack.Core
Imports OnTrack.rulez.eXPressionTree
Imports OnTrack.rulez

Namespace OnTrack.Database

    ''' <summary>
    ''' SQL Server OnTrack Database Driver
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <ormDatabaseDriver(autoinstance:=False, Name:=ConstCPVDriverSQLServer, isontrackdriver:=True, Version:=2)> _
    Public Class mssqlDBDriver
        Inherits AdoNetRDBDriver
        Implements iormRelationalDatabaseDriver

        Protected Shadows WithEvents _primaryConnection As mssqlConnection '-> in clsOTDBDriver
        Private Shadows _ParametersTableAdapter As New SqlDataAdapter
        Shadows Event RequestBootstrapInstall(sender As Object, e As SessionBootstrapEventArgs) Implements iormRelationalDatabaseDriver.RequestBootstrapInstall

        Private _internallock As New Object 'internal lock
        Private _parameterlock As New Object 'internal lock

        ''' <summary>
        ''' construcotr
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            Call MyBase.New()
            If Me._primaryConnection Is Nothing Then
                _primaryConnection = New mssqlConnection(id:="primary", DatabaseDriver:=Me, session:=Session, sequence:=ComplexPropertyStore.Sequence.Primary)
            End If
        End Sub
        ''' <summary>
        ''' 
        ''' 
        ''' Constructor
        ''' </summary>
        ''' <param name="ID">an ID for this driver</param>
        ''' <remarks></remarks>
        Public Sub New(ID As String, ByRef session As Session)
            Call MyBase.New(ID, session)
            If Me._primaryConnection Is Nothing Then
                _primaryConnection = New mssqlConnection(id:="primary", DatabaseDriver:=Me, session:=session, sequence:=ComplexPropertyStore.Sequence.Primary)
            End If
        End Sub


        ''' <summary>
        ''' NativeConnection
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads ReadOnly Property NativeConnection() As SqlConnection
            Get
                Return DirectCast(_primaryConnection.NativeConnection, SqlConnection)
            End Get

        End Property
        ''' <summary>
        ''' build Adapter for parameter table
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function BuildParameterAdapter()

            With _ParametersTableAdapter


                .SelectCommand.Prepare()

                ' Create the commands.
                '**** INSERT
                .InsertCommand = New SqlCommand( _
                                String.Format("INSERT INTO  [{0}] ([{1}], [{2}] , [{3}] , [{4}], [{5}])  VALUES (@SetupID, @ID , @Value , @changedOn , @description) ", _
                                              GetNativeDBObjectName(_parametersTableName), ConstFNSetupID, ConstFNID, ConstFNValue, ConstFNChangedOn, constFNDescription))
                '' Create the parameters.
                .InsertCommand.Parameters.Add("@SetupID", SqlDbType.Char, 50, ConstFNSetupID)
                .InsertCommand.Parameters.Add("@ID", SqlDbType.Char, 50, ConstFNID)
                .InsertCommand.Parameters.Add("@Value", SqlDbType.VarChar, 250, ConstFNValue)
                .InsertCommand.CommandType = CommandType.Text
                ' handling of the timestamp
                '.InsertCommand.Parameters.Add("@changedOn", SqlDbType.VarChar, 250, ConstFNChangedOn)
                .InsertCommand.Parameters.Add(parameterName:="@changedOn", sqlDbType:=SqlDbType.DateTime).SourceColumn = ConstFNChangedOn
                .InsertCommand.Parameters(parameterName:="@changedOn").IsNullable = True

                .InsertCommand.Parameters.Add("@description", SqlDbType.VarChar, 250, constFNDescription)
                .InsertCommand.Connection = DirectCast(_primaryConnection.NativeInternalConnection, SqlConnection)
                .InsertCommand.Prepare()


                '**** UPDATE
                .UpdateCommand = New SqlCommand( _
                String.Format("UPDATE [{0}] SET [{1}] = @value , [{2}] = @changedOn , [{3}] = @description WHERE [{4}] = @ID AND [{5}] = @SETUPID", _
                              GetNativeDBObjectName(_parametersTableName), ConstFNValue, ConstFNChangedOn, constFNDescription, ConstFNID, ConstFNSetupID))
                '' Create the parameters.
                .UpdateCommand.Parameters.Add("@value", SqlDbType.VarChar, 250, ConstFNValue)
                ' strange enough sqldbdate is not working on some sqlservers
                ' handling of the timestamp
                .UpdateCommand.Parameters.Add(parameterName:="@changedOn", sqlDbType:=SqlDbType.DateTime).SourceColumn = ConstFNChangedOn
                '.UpdateCommand.Parameters.Add("@changedOn", SqlDbType.VarChar, 250, ConstFNChangedOn)
                .UpdateCommand.Parameters(parameterName:="@changedOn").IsNullable = True

                .UpdateCommand.Parameters.Add("@description", SqlDbType.VarChar, 250, constFNDescription)
                .UpdateCommand.Parameters.Add("@ID", SqlDbType.Char, 50, ConstFNID).SourceVersion = DataRowVersion.Original
                .UpdateCommand.Parameters.Add("@SETUPID", SqlDbType.Char, 50, ConstFNSetupID).SourceVersion = DataRowVersion.Original

                .UpdateCommand.Connection = DirectCast(_primaryConnection.NativeInternalConnection, SqlConnection)
                .UpdateCommand.Prepare()


                '***** DELETE
                .DeleteCommand = New SqlCommand(String.Format("DELETE FROM [{0}] where [{1}] = @id AND [{2}] = @SETUPID", GetNativeDBObjectName(_parametersTableName), ConstFNID, ConstFNSetupID))
                .DeleteCommand.Parameters.Add("@ID", SqlDbType.Char, 50, ConstFNID).SourceVersion = DataRowVersion.Original
                .DeleteCommand.Parameters.Add("@SETUPID", SqlDbType.Char, 50, ConstFNSetupID).SourceVersion = DataRowVersion.Original
                .DeleteCommand.Connection = DirectCast(_primaryConnection.NativeInternalConnection, SqlConnection)
                .DeleteCommand.Prepare()

            End With

        End Function
        ''' <summary>
        ''' initialize driver
        ''' </summary>
        ''' <param name="Force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function Initialize(Optional force As Boolean = False) As Boolean

            If Me.IsInitialized And Not force Then
                Return True
            End If

            Try
                Call MyBase.Initialize(Force:=force)

                ' we have no Connection ?!
                If _primaryConnection Is Nothing Then
                    _primaryConnection = New mssqlConnection("primary", Me, ComplexPropertyStore.Sequence.Primary, session:=_session)
                End If

                '*** do we have the Table ?! - donot do this in bootstrapping since we are running in recursion then
                If Not Me.HasTable(_parametersTableName) And Not _session.IsBootstrappingInstallationRequested Then
                    If Not VerifyOnTrackDatabase(install:=False) Then
                        '* now in bootstrap ?!
                        If _session.IsBootstrappingInstallationRequested Then
                            CoreMessageHandler(message:="verifying the database failed moved to bootstrapping - caching parameters meanwhile", procedure:="mssqlDBDriver.Initialize", _
                                          messagetype:=otCoreMessageType.InternalWarning, argument:=Me.ID)
                            Me.IsInitialized = True
                            Return True
                        Else
                            CoreMessageHandler(message:="verifying the database failed - failed to initialize driver", procedure:="mssqlDBDriver.Initialize", _
                                              messagetype:=otCoreMessageType.InternalError, argument:=Me.ID)
                            Me.IsInitialized = False
                            Return False
                        End If
                    End If
                End If

                '*** end of bootstrapping conditions reinitialize automatically
                '*** verifyOnTrackDatabase might set bootstrapping mode
                If Not _session.IsBootstrappingInstallationRequested OrElse force Then
                    '*** set the DataTable
                    If _OnTrackDataSet Is Nothing Then _OnTrackDataSet = New DataSet(Me.ID & Date.Now.ToString)

                    '** create adapaters
                    If Me.HasTable(_parametersTableName) Then
                        ' the command
                        Dim aDBCommand = New SqlCommand()
                        aDBCommand.CommandText = String.Format("select [{0}],[{1}],[{2}],[{3}],[{4}] from [{5}] ", _
                                                                ConstFNSetupID, ConstFNID, ConstFNValue, ConstFNChangedOn, constFNDescription, GetNativeDBObjectName(_parametersTableName))
                        aDBCommand.Connection = DirectCast(_primaryConnection.NativeInternalConnection, SqlConnection)
                        ' fill with adapter
                        _ParametersTableAdapter = New SqlDataAdapter()
                        _ParametersTableAdapter.SelectCommand = aDBCommand
                        _ParametersTableAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey

                        SyncLock DirectCast(_primaryConnection.NativeInternalConnection, SqlConnection)
                            _ParametersTableAdapter.FillSchema(_OnTrackDataSet, SchemaType.Source)
                            _ParametersTableAdapter.Fill(_OnTrackDataSet, _parametersTableName)
                        End SyncLock

                        ' build Commands
                        Call BuildParameterAdapter()
                        ' set the Table
                        _ParametersTable = _OnTrackDataSet.Tables(_parametersTableName)

                        '** save the cache
                        If _BootStrapParameterCache.Count > 0 Then
                            For Each kvp As KeyValuePair(Of String, Object) In _BootStrapParameterCache
                                SetDBParameter(parametername:=kvp.Key, value:=kvp.Value, silent:=True)
                            Next
                            _BootStrapParameterCache.Clear()
                        End If
                    Else
                        '** important to recognize where to write data
                        _ParametersTable = Nothing
                    End If

                End If


                Me.IsInitialized = True
                Return True
            Catch ex As Exception
                Me.IsInitialized = False
                Call CoreMessageHandler(procedure:="mssqlDBDriver.OnConnection", message:="couldnot Initialize Driver", _
                                      exception:=ex)
                Me.IsInitialized = False
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Gets the name of the driver.
        ''' </summary>
        ''' <value>The type.</value>
        Public Overrides ReadOnly Property Name() As String
            Get
                Return ConstCPVDriverSQLServer
            End Get
        End Property
        ''' <summary>
        ''' create a new TableStore for this Driver
        ''' </summary>
        ''' <param name="TableID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function CreateNativeTableStore(ByVal TableID As String, ByVal forceSchemaReload As Boolean) As iormRelationalTableStore
            Return New mssqlTableStore(Me.CurrentConnection, TableID, forceSchemaReload)
        End Function
        ''' <summary>
        ''' create a new TableSchema for this Driver
        ''' </summary>
        ''' <param name="TableID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function CreateNativeTableSchema(ByVal TableID As String) As iormContainerSchema
            Return New mssqlTableSchema(Me.CurrentConnection, TableID)
        End Function
        ''' <summary>
        ''' create a new TableStore for this Driver
        ''' </summary>
        ''' <param name="TableID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function CreateNativeViewReader(ByVal viewID As String, ByVal forceSchemaReload As Boolean) As iormRelationalTableStore
            Return New mssqlViewReader(Me.CurrentConnection, viewID, forceSchemaReload)
        End Function
        ''' <summary>
        ''' create a new TableSchema for this Driver
        ''' </summary>
        ''' <param name="TableID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function CreateNativeViewSchema(ByVal viewID As String) As iormContainerSchema
            Return New mssqlViewSchema(Me.CurrentConnection, viewID)
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function CreateNativeDBCommand(commandstr As String, nativeConnection As IDbConnection) As IDbCommand Implements iormRelationalDatabaseDriver.CreateNativeDBCommand
            Return New SqlCommand(commandstr, nativeConnection)
        End Function
        ''' <summary>
        '''  raise the RequestBootStrapInstall Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Protected Overrides Sub RaiseRequestBootstrapInstall(sender As Object, ByRef e As EventArgs)
            RaiseEvent RequestBootstrapInstall(sender, e)
        End Sub
        ''' <summary>
        ''' returns a object from sourcetype of the column to Host interpretation (.net)
        ''' </summary>
        ''' <param name="invalue"></param>
        ''' <param name="outvalue"></param>
        ''' <param name="sourceType"></param>
        ''' <param name="isnullable"></param>
        ''' <param name="defaultvalue"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Convert2ObjectData(ByVal invalue As Object, _
                                                     ByRef outvalue As Object, _
                                                     sourceType As Long, _
                                                     Optional isnullable As Boolean? = Nothing, _
                                                     Optional defaultvalue As Object = Nothing, _
                                                     Optional ByRef abostrophNecessary As Boolean = False) As Boolean Implements iormRelationalDatabaseDriver.Convert2ObjectData

            Dim result As Object = Nothing


            Try


                abostrophNecessary = False

                '*
                '*


                If sourceType = SqlDataType.BigInt Or sourceType = SqlDataType.Int _
                    Or sourceType = SqlDataType.SmallInt Or sourceType = SqlDataType.TinyInt Then

                    If defaultvalue Is Nothing Then defaultvalue = Convert.ToInt64(0)
                    If isnullable Then
                        result = New Nullable(Of Long)
                    Else
                        result = New Long
                    End If

                    If isnullable AndAlso (Not IsNumeric(invalue) OrElse invalue Is Nothing OrElse _
                                               DBNull.Value.Equals(invalue) OrElse String.IsNullOrWhiteSpace(invalue)) Then
                        result = New Nullable(Of Long)
                    ElseIf Not isnullable AndAlso (Not IsNumeric(invalue) OrElse invalue Is Nothing OrElse _
                                               DBNull.Value.Equals(invalue) OrElse String.IsNullOrWhiteSpace(invalue)) Then
                        result = Convert.ToInt64(defaultvalue)
                    ElseIf IsNumeric(invalue) Then
                        result = Convert.ToInt64(invalue)
                    Else
                        Call CoreMessageHandler(procedure:="mssqlDBDriver.convert2ObjectData", messagetype:=otCoreMessageType.InternalError, _
                                              message:="OTDB data '" & invalue & "' is not convertible to Integer", _
                                              argument:=sourceType)
                        Return False
                    End If

                ElseIf sourceType = SqlDataType.Char Or sourceType = SqlDataType.NText _
                     Or sourceType = SqlDataType.VarChar Or sourceType = SqlDataType.Text _
                      Or sourceType = SqlDataType.NVarChar Or sourceType = SqlDataType.VarCharMax _
                      Or sourceType = SqlDataType.NVarCharMax Then
                    abostrophNecessary = True
                    If defaultvalue Is Nothing Then defaultvalue = Convert.ToString(String.Empty)

                    If isnullable AndAlso (invalue Is Nothing OrElse DBNull.Value.Equals(invalue) OrElse _
                                          String.IsNullOrWhiteSpace(invalue)) Then
                        result = Nothing
                    ElseIf Not isnullable AndAlso (invalue Is Nothing OrElse DBNull.Value.Equals(invalue) OrElse _
                                          String.IsNullOrWhiteSpace(invalue)) Then
                        result = Convert.ToString(defaultvalue)
                    Else
                        result = Convert.ToString(invalue)
                    End If

                ElseIf sourceType = SqlDataType.Date Or sourceType = SqlDataType.SmallDateTime Or sourceType = SqlDataType.Time _
                Or sourceType = SqlDataType.Timestamp Or sourceType = SqlDataType.DateTime Or sourceType = SqlDataType.DateTime2 _
                Or sourceType = SqlDataType.DateTimeOffset Then
                    If defaultvalue Is Nothing Then defaultvalue = Convert.ToDateTime(ConstNullDate)
                    If isnullable Then
                        result = New Nullable(Of DateTime)
                    Else
                        result = New DateTime
                    End If

                    If isnullable AndAlso (Not IsDate(invalue) OrElse invalue Is Nothing OrElse DBNull.Value.Equals(invalue) _
                                            OrElse String.IsNullOrWhiteSpace(invalue)) Then
                        result = New Nullable(Of DateTime)
                    ElseIf (Not IsDate(invalue) Or invalue Is Nothing Or DBNull.Value.Equals(invalue) Or IsError(invalue)) OrElse String.IsNullOrWhiteSpace(invalue) Then
                        result = Convert.ToDateTime(defaultvalue)
                    ElseIf IsDate(invalue) Then
                        result = Convert.ToDateTime(invalue)
                    Else
                        Call CoreMessageHandler(procedure:="mssqlDBDriver.convert2ObjectData", messagetype:=otCoreMessageType.InternalError, _
                                              message:="OTDB data '" & invalue & "' is not convertible to Date", _
                                              argument:=sourceType)
                        Return False
                    End If

                ElseIf sourceType = SqlDataType.Float Or sourceType = SqlDataType.Decimal _
               Or sourceType = SqlDataType.Real Then
                    If defaultvalue Is Nothing Then defaultvalue = Convert.ToDouble(0)
                    If isnullable Then
                        result = New Nullable(Of Double)
                    Else
                        result = New Double
                    End If

                    If isnullable AndAlso (Not IsNumeric(invalue) OrElse invalue Is Nothing OrElse _
                        DBNull.Value.Equals(invalue) OrElse String.IsNullOrWhiteSpace(invalue)) Then
                        result = New Nullable(Of Double)
                    ElseIf isnullable AndAlso (Not IsNumeric(invalue) OrElse invalue Is Nothing OrElse _
                        DBNull.Value.Equals(invalue) OrElse String.IsNullOrWhiteSpace(invalue)) Then
                        result = Convert.ToDouble(defaultvalue)
                    ElseIf IsNumeric(invalue) Then
                        result = Convert.ToDouble(invalue)
                    Else
                        Call CoreMessageHandler(procedure:="mssqlDBDriver.convert2ObjectData", messagetype:=otCoreMessageType.InternalError, _
                                              message:="OTDB data '" & invalue & "' is not convertible to Double", _
                                              argument:=sourceType)
                        Return False
                    End If

                ElseIf sourceType = SqlDataType.Bit Then
                    If defaultvalue Is Nothing Then defaultvalue = Convert.ToBoolean(False)
                    If isnullable Then
                        result = New Nullable(Of Boolean)
                    Else
                        result = New Boolean
                    End If

                    If isnullable AndAlso (invalue Is Nothing OrElse DBNull.Value.Equals(invalue) _
                                               OrElse invalue = False) OrElse String.IsNullOrWhiteSpace(invalue) Then
                        result = New Nullable(Of Boolean)
                    ElseIf Not isnullable AndAlso (invalue Is Nothing OrElse DBNull.Value.Equals(invalue) _
                                               OrElse invalue = False) OrElse String.IsNullOrWhiteSpace(invalue) Then
                        result = Convert.ToBoolean(False)
                    Else
                        result = True
                    End If

                End If

                ' return
                outvalue = result
                Return True
            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=False, procedure:="mssqlmssqlDBDriver.convert2ObjData", _
                                      argument:=sourceType, exception:=ex, _
                                      messagetype:=otCoreMessageType.InternalError)
                Return False
            End Try

        End Function
        ''' <summary>
        ''' converts data to a specific native database type
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="targetType"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Convert2DBData(ByVal invalue As Object, ByRef outvalue As Object, _
                                                     targetType As Long, _
                                                     Optional ByVal maxsize As Long = 0, _
                                                    Optional ByRef abostrophNecessary As Boolean = False, _
                                                    Optional ByVal columnname As String = Nothing, _
                                                    Optional isnullable As Boolean = False, _
                                                    Optional defaultvalue As Object = Nothing) As Boolean Implements iormRelationalDatabaseDriver.Convert2DBData
            Dim result As Object = Nothing
            Try

                '*** array conversion should not occure on this level
                If IsArray(invalue) Then
                    invalue = Core.DataType.ToString(invalue)
                End If

                If targetType = SqlDataType.BigInt OrElse targetType = SqlDataType.Int _
                OrElse targetType = SqlDataType.SmallInt OrElse targetType = SqlDataType.TinyInt Then

                    If defaultvalue Is Nothing Then defaultvalue = 0

                    If isnullable AndAlso (invalue Is Nothing OrElse String.IsNullOrWhiteSpace(invalue.ToString) _
                        OrElse DBNull.Value.Equals(invalue)) Then
                        result = DBNull.Value
                    ElseIf Not isnullable AndAlso (invalue Is Nothing OrElse String.IsNullOrWhiteSpace(invalue.ToString) _
                        OrElse IsError(invalue) OrElse DBNull.Value.Equals(invalue)) Then
                        result = Convert.ToInt64(defaultvalue)
                    ElseIf IsNumeric(invalue) Then
                        result = Convert.ToInt64(invalue)
                    Else
                        Call CoreMessageHandler(procedure:="mssqlDBDriver.cvt2ColumnData", entryname:=columnname, _
                                              message:="OTDB data " & invalue & " is not convertible to Long", _
                                              argument:=invalue, messagetype:=otCoreMessageType.InternalError)
                        outvalue = Nothing
                        Return False
                    End If

                ElseIf targetType = SqlDataType.Char OrElse targetType = SqlDataType.NText _
                    OrElse targetType = SqlDataType.VarChar OrElse targetType = SqlDataType.Text _
                     OrElse targetType = SqlDataType.NVarChar OrElse targetType = SqlDataType.VarCharMax _
                     OrElse targetType = SqlDataType.NVarCharMax Then

                    abostrophNecessary = True
                    If defaultvalue Is Nothing Then defaultvalue = String.Empty

                    If isnullable AndAlso (invalue Is Nothing OrElse DBNull.Value.Equals(invalue)) Then
                        result = DBNull.Value
                    ElseIf Not isnullable AndAlso (invalue Is Nothing OrElse String.IsNullOrWhiteSpace(invalue) OrElse _
                                               DBNull.Value.Equals(invalue)) Then
                        result = Convert.ToString(defaultvalue)
                    Else
                        If maxsize < Len(CStr(invalue)) And maxsize > 1 Then
                            result = Mid(Convert.ToString(invalue), 1, maxsize - 1)
                        Else
                            result = Convert.ToString(invalue)
                        End If
                    End If

                ElseIf targetType = SqlDataType.Date OrElse targetType = SqlDataType.SmallDateTime OrElse targetType = SqlDataType.Time _
                OrElse targetType = SqlDataType.Timestamp OrElse targetType = SqlDataType.DateTime OrElse targetType = SqlDataType.DateTime2 _
                OrElse targetType = SqlDataType.DateTimeOffset Then

                    If defaultvalue Is Nothing Then defaultvalue = ConstNullDate

                    If isnullable AndAlso (invalue Is Nothing OrElse String.IsNullOrWhiteSpace(invalue.ToString) OrElse _
                         DBNull.Value.Equals(invalue)) Then
                        result = DBNull.Value
                    ElseIf Not isnullable AndAlso (invalue Is Nothing OrElse String.IsNullOrWhiteSpace(invalue.ToString) OrElse _
                         DBNull.Value.Equals(invalue)) Then
                        result = Convert.ToDateTime(defaultvalue)
                    ElseIf IsDate(invalue) Then
                        result = Convert.ToDateTime(invalue)
                    ElseIf invalue.GetType = GetType(TimeSpan) Then
                        result = invalue
                    Else
                        Call CoreMessageHandler(procedure:="mssqlDBDriver.cvt2ColumnData", entryname:=columnname, _
                                              message:="OTDB data " & invalue & " is not convertible to Date", _
                                              argument:=invalue, messagetype:=otCoreMessageType.InternalError)
                        outvalue = Nothing
                        Return False
                    End If

                ElseIf targetType = SqlDataType.Float OrElse targetType = SqlDataType.Decimal _
                OrElse targetType = SqlDataType.Real Then

                    If defaultvalue Is Nothing Then defaultvalue = Convert.ToDouble(0)

                    If isnullable AndAlso (invalue Is Nothing OrElse String.IsNullOrWhiteSpace(invalue.ToString) _
                        OrElse DBNull.Value.Equals(invalue)) Then
                        result = DBNull.Value
                    ElseIf Not isnullable AndAlso (invalue Is Nothing OrElse String.IsNullOrWhiteSpace(invalue.ToString) _
                        OrElse DBNull.Value.Equals(invalue)) Then
                        result = defaultvalue
                    ElseIf IsNumeric(invalue) Then
                        result = Convert.ToDouble(invalue)
                    Else
                        Call CoreMessageHandler(procedure:="mssqlDBDriver.cvt2ColumnData", entryname:=columnname, _
                                              message:="OTDB data " & invalue & " is not convertible to Double", _
                                              argument:=targetType, messagetype:=otCoreMessageType.InternalError)
                        outvalue = Nothing
                        Return False
                    End If

                ElseIf targetType = SqlDataType.Bit Then

                    If defaultvalue Is Nothing Then defaultvalue = Convert.ToBoolean(False)

                    If isnullable AndAlso (invalue Is Nothing OrElse DBNull.Value.Equals(invalue) OrElse String.IsNullOrWhiteSpace(invalue.ToString) _
                         OrElse (IsNumeric(invalue) AndAlso invalue = 0)) Then
                        result = DBNull.Value
                    ElseIf Not isnullable AndAlso (invalue Is Nothing OrElse DBNull.Value.Equals(invalue) OrElse String.IsNullOrWhiteSpace(invalue.ToString) _
                         OrElse (IsNumeric(invalue) AndAlso invalue = 0)) Then
                        result = defaultvalue
                    ElseIf TypeOf (invalue) Is Boolean Then
                        result = Convert.ToBoolean(invalue)
                    Else
                        result = True
                    End If

                End If

                ' return
                outvalue = result
                Return True

            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception", procedure:="mssqlDBDriver.convert2ColumnData(Object, long ..", _
                                       exception:=ex, messagetype:=otCoreMessageType.InternalException)
                outvalue = Nothing
                Return False
            End Try

        End Function
        ''' <summary>
        ''' returns the target type for a OTDB FieldType - MAPPING
        ''' </summary>
        ''' <param name="type"></param>
        ''' <remarks></remarks>
        ''' <returns></returns>
        Public Overrides Function GetTargetTypeFor(type As otDataType) As Long Implements iormRelationalDatabaseDriver.GetTargetTypeFor

            Try
                '** returns SQLDataType which is SMO DataType and not SQLDbtype for ADONET !

                Select Case type
                    Case otDataType.Binary
                        Return SqlDataType.Binary
                    Case otDataType.Bool
                        Return SqlDataType.Bit
                    Case otDataType.[Date]
                        Return SqlDataType.Date
                    Case otDataType.[Time]
                        Return SqlDataType.Time
                    Case otDataType.List
                        Return SqlDataType.NVarChar
                    Case otDataType.[Long]
                        Return SqlDataType.BigInt
                    Case otDataType.Memo
                        Return SqlDataType.NVarChar
                    Case otDataType.Numeric
                        Return SqlDataType.Decimal
                    Case otDataType.Timestamp
                        Return SqlDataType.DateTime
                    Case otDataType.Text
                        Return SqlDataType.NVarChar
                    Case Else

                        Call CoreMessageHandler(procedure:="mssqlDBDriver.GetTargetTypefor", message:="Type not defined",
                                       messagetype:=otCoreMessageType.InternalException)
                End Select

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="mssqlDBDriver.GetTargetTypefor", message:="Exception", exception:=ex, _
                                       messagetype:=otCoreMessageType.InternalException)
                Return 0
            End Try

        End Function
        ''' <summary>
        ''' create an assigned Native DBParameter to provided name and type
        ''' </summary>
        ''' <param name="parametername">name of parameter</param>
        ''' <param name="datatype">otdb datatype</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function AssignNativeDBParameter(parametername As String, _
                                                          datatype As otDataType, _
                                                           Optional maxsize As Long = 0, _
                                                          Optional value As Object = Nothing) As System.Data.IDbDataParameter _
                                                      Implements iormRelationalDatabaseDriver.AssignNativeDBParameter


            Try
                Dim aParameter As New SqlParameter()

                aParameter.ParameterName = parametername

                Select Case datatype
                    Case otDataType.Binary
                        aParameter.SqlDbType = SqlDbType.Binary
                    Case otDataType.Bool
                        aParameter.SqlDbType = SqlDbType.Bit
                    Case otDataType.[Date]
                        aParameter.SqlDbType = SqlDbType.Date
                    Case otDataType.[Time]
                        aParameter.SqlDbType = SqlDbType.Time
                    Case otDataType.List
                        aParameter.SqlDbType = SqlDbType.NVarChar
                    Case otDataType.[Long]
                        aParameter.SqlDbType = SqlDbType.BigInt
                    Case otDataType.Memo
                        aParameter.SqlDbType = SqlDbType.NVarChar
                    Case otDataType.Numeric
                        aParameter.SqlDbType = SqlDbType.Decimal
                    Case otDataType.Timestamp
                        aParameter.SqlDbType = SqlDbType.DateTime
                    Case otDataType.Text
                        aParameter.SqlDbType = SqlDbType.NVarChar
                    Case Else

                        Call CoreMessageHandler(procedure:="mssqlDBDriver.AssignNativeDBParameter", message:="Type not defined",
                                       messagetype:=otCoreMessageType.InternalException)
                End Select

                Select Case datatype
                    Case otDataType.Bool
                        aParameter.SqlValue = False
                    Case otDataType.[Date]
                        aParameter.SqlValue = ConstNullDate
                    Case otDataType.[Time]
                        If maxsize = 0 Then aParameter.Size = 7
                        aParameter.SqlValue = ot.ConstNullTime
                    Case otDataType.List
                        If maxsize = 0 Then aParameter.Size = ConstDBDriverMaxTextSize
                        aParameter.SqlValue = String.Empty
                    Case otDataType.[Long]
                        aParameter.SqlValue = 0
                    Case otDataType.Memo
                        If maxsize = 0 Then aParameter.Size = constDBDriverMaxMemoSize
                        aParameter.SqlValue = String.Empty
                    Case otDataType.Numeric
                        aParameter.SqlValue = 0
                    Case otDataType.Timestamp
                        aParameter.SqlValue = ConstNullDate
                    Case otDataType.Text
                        If maxsize = 0 Then aParameter.Size = ConstDBDriverMaxTextSize
                        aParameter.SqlValue = String.Empty

                End Select
                If Not value Is Nothing Then
                    aParameter.SqlValue = value
                End If
                Return aParameter
            Catch ex As Exception
                Call CoreMessageHandler(procedure:="mssqlDBDriver.assignDBParameter", message:="Exception", exception:=ex, _
                                       messagetype:=otCoreMessageType.InternalException)
                Return Nothing
            End Try

        End Function


        ''' <summary>
        ''' returns true if the datastore has the view by viewname
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="connection"></param>
        ''' <param name="nativeConnection"></param>
        ''' <remarks></remarks>
        ''' <returns></returns>
        Public Overrides Function HasView(viewid As String, Optional ByRef connection As iormConnection = Nothing, Optional nativeConnection As Object = Nothing) As Boolean Implements iormRelationalDatabaseDriver.HasView
            Dim myconnection As mssqlConnection
            Dim smoconnection As ServerConnection
            Dim database As Microsoft.SqlServer.Management.Smo.Database
            Dim myNativeConnection As SqlConnection
            Dim nativeViewName As String = GetNativeViewname(viewid)

            '* if already loaded
            If _TableDirectory.ContainsKey(key:=nativeViewName) Then Return True

            If connection Is Nothing Then
                myconnection = _primaryConnection
            Else
                myconnection = connection
            End If
            ' ** important ! Access to native Internal Connection opens up the internal connection and also the SMO Driver
            ' **
            If nativeConnection Is Nothing And myconnection IsNot Nothing Then
                myNativeConnection = TryCast(myconnection.NativeInternalConnection, SqlConnection)
            Else
                myNativeConnection = TryCast(nativeConnection, SqlConnection)
            End If

            '** return if no connection (and no exception)

            If myNativeConnection Is Nothing Then
                CoreMessageHandler(message:="no connection established", procedure:="mssqldbdriver.hastable", messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            '*** check on rights - avoid recursion if we are looking for the User Table
            '** makes no sense since we are checkin before installation if we need to install
            'If Not CurrentSession.IsBootstrappingInstallation AndAlso tableID <> User.ConstPrimaryTableID Then
            '    If Not _currentUserValidation.ValidEntry AndAlso Not _currentUserValidation.HasReadRights Then
            '        If Not myconnection.VerifyUserAccess(accessRequest:=otAccessRight.[ReadOnly], loginOnFailed:=True) Then
            '            Call CoreMessageHandler(showmsgbox:=True, subname:="mssqlDBDriver.HasTable", tablename:=tableID, _
            '                                  message:="No right to read schema of database", messagetype:=otCoreMessageType.ApplicationError)
            '            Return False
            '        End If
            '    End If
            'End If


            Try
                SyncLock _internallock
                    smoconnection = myconnection.SMOConnection ' will be setup during internal connection
                    database = myconnection.Database

                    If smoconnection Is Nothing OrElse database Is Nothing Then
                        Call CoreMessageHandler(message:="SMO is not initialized", argument:=nativeViewName, _
                                              procedure:="mssqlDBDriver.hasTable", messagetype:=otCoreMessageType.InternalError)
                        Return Nothing
                    End If

                    database.Views.Refresh()
                    Dim existsOnServer As Boolean = database.Views.Contains(name:=nativeViewName)
                    Return existsOnServer
                End SyncLock


            Catch smoex As SmoException

                Dim sb As New StringBuilder
                sb.AppendLine("This is an SMO Exception")
                'Display the SMO exception message.
                sb.AppendLine(smoex.Message)
                'Display the sequence of non-SMO exceptions that caused the SMO exception.
                Dim ex As Exception
                ex = smoex.InnerException
                If ex Is Nothing Then
                Else
                    Do While ex.InnerException IsNot (Nothing)
                        sb.AppendLine(ex.InnerException.Message)
                        ex = ex.InnerException
                    Loop
                End If

                Call CoreMessageHandler(message:=sb.ToString, exception:=ex, argument:=nativeViewName, _
                                      procedure:="mssqlDBDriver.HasView", messagetype:=otCoreMessageType.InternalError)
                Return False
            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception", exception:=ex, argument:=nativeViewName, _
                                      procedure:="mssqlDBDriver.HasView", messagetype:=otCoreMessageType.InternalError)
                Return False
            End Try

        End Function


        ''' <summary>
        ''' returns or creates a view in the data store
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="sqlselect"></param>
        ''' <param name="createOrAlter"></param>
        ''' <param name="connection"></param>
        ''' <remarks></remarks>
        ''' <returns></returns>
        Public Overrides Function GetView(viewid As String, _
                                          Optional sqlselect As String = Nothing, _
                                          Optional createOrAlter As Boolean = False, _
                                          Optional ByRef connection As iormConnection = Nothing) As Object Implements iormRelationalDatabaseDriver.GetView
            Dim aView As Microsoft.SqlServer.Management.Smo.View
            Dim smoconnection As ServerConnection
            Dim database As Microsoft.SqlServer.Management.Smo.Database
            Dim localCreated As Boolean = False
            Dim myconnection As mssqlConnection
            Dim nativeViewName As String = GetNativeViewname(viewid)

            If connection Is Nothing Then
                myconnection = _primaryConnection
            Else
                myconnection = connection
            End If
            ' ** important ! Access to native Internal Connection opens up the internal connection and also the SMO Driver
            ' **
            If myconnection Is Nothing Or myconnection.NativeInternalConnection Is Nothing Then
                Call CoreMessageHandler(message:="internal connection connection is nothing - no table can be retrieved", procedure:="mssqlDBDriver.GetView", _
                                            messagetype:=otCoreMessageType.InternalError, argument:=nativeViewName)
                Return Nothing
            End If

            '*** check on rights
            If createOrAlter And Not CurrentSession.IsBootstrappingInstallationRequested Then
                If Not myconnection.VerifyUserAccess(accessRequest:=otAccessRight.AlterSchema, useLoginWindow:=True) Then
                    Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.GetView", argument:=nativeViewName, _
                                          message:="No right to alter schema of database", messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If
            End If



            Try
                SyncLock _internallock

                    smoconnection = myconnection.SMOConnection
                    database = myconnection.Database()

                    If smoconnection Is Nothing OrElse database Is Nothing Then
                        Call CoreMessageHandler(message:="SMO is not initialized", argument:=nativeViewName, _
                                              procedure:="mssqlDBDriver.GetView", messagetype:=otCoreMessageType.InternalError)
                        Return Nothing
                    End If

                    database.Views.Refresh()
                    Dim existsOnServer As Boolean = database.Views.Contains(name:=nativeViewName)

                    '*** No CreateAlter -> return the Object
                    '*** CreatorAlter but the Object exists and was localCreated (means no object transmitted for change)
                    '*** return the refreshed

                    If Not createOrAlter AndAlso existsOnServer Then
                        If Not aView Is Nothing Then aView.Refresh()
                        Return aView

                        '** doesnot Exist 
                    ElseIf (Not createOrAlter AndAlso Not existsOnServer) Then
                        Call CoreMessageHandler(procedure:="mssqlDBDriver.GetView", message:="View does not exist", messagetype:=otCoreMessageType.InternalWarning, _
                                               break:=False, argument:=nativeViewName)
                        Return Nothing
                    End If

                    '** create the View
                    '**
                    If createOrAlter Then
                        ''' drop the view
                        If existsOnServer Then
                            database.Views.Item(name:=nativeViewName).Drop()
                        End If
                        '                        View myview = new View(myNewDatabase, "My_SMO_View");
                        'myview.TextHeader = "CREATE VIEW [My_SMO_View] AS";
                        'myview.TextBody = "SELECT ID, NAME FROM MyFirstSMOTable"; 
                        'myview.Create();
                        aView = New Microsoft.SqlServer.Management.Smo.View(database, nativeViewName.ToUpper)
                        aView.TextMode = True
                        aView.TextHeader = "CREATE VIEW [" & nativeViewName.ToUpper & "] AS "
                        aView.TextBody = sqlselect
                        'aView.isschemabound = True

                        aView.Create()
                        Return aView
                    Else
                        Call CoreMessageHandler(procedure:="mssqlDBDriver.GetView", argument:=nativeViewName, _
                                              message:="View was not found in database", messagetype:=otCoreMessageType.ApplicationWarning)
                        Return Nothing
                    End If
                End SyncLock

                Return aView

            Catch smoex As SmoException

                Dim sb As New StringBuilder
                sb.AppendLine("This is an SMO Exception")
                'Display the SMO exception message.
                sb.AppendLine(smoex.Message)
                'Display the sequence of non-SMO exceptions that caused the SMO exception.
                Dim ex As Exception
                ex = smoex.InnerException
                If ex Is Nothing Then
                Else
                    Do While ex.InnerException IsNot (Nothing)
                        sb.AppendLine(ex.InnerException.Message)
                        ex = ex.InnerException
                    Loop
                End If

                Call CoreMessageHandler(message:=sb.ToString, exception:=ex, argument:=nativeViewName, _
                                      procedure:="mssqlDBDriver.getView", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception", exception:=ex, argument:=nativeViewName, _
                                      procedure:="mssqlDBDriver.getView", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End Try

        End Function

        ''' <summary>
        ''' True if table ID exists in data store
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function HasTable(tableid As String, _
                                           Optional ByRef connection As iormConnection = Nothing, _
                                           Optional nativeConnection As Object = Nothing) As Boolean

            Dim myconnection As mssqlConnection
            Dim smoconnection As ServerConnection
            Dim database As Microsoft.SqlServer.Management.Smo.Database
            Dim myNativeConnection As SqlConnection
            Dim path As String
            Dim nativeTablename As String = GetNativeDBObjectName(tableid)


            '* if already loaded
            If _TableDirectory.ContainsKey(key:=nativeTablename) Then Return True

            If connection Is Nothing Then
                myconnection = _primaryConnection
            Else
                myconnection = connection
            End If
            ' ** important ! Access to native Internal Connection opens up the internal connection and also the SMO Driver
            ' **
            If nativeConnection Is Nothing And myconnection IsNot Nothing Then
                myNativeConnection = TryCast(myconnection.NativeInternalConnection, SqlConnection)
            Else
                myNativeConnection = TryCast(nativeConnection, SqlConnection)
            End If

            '** return if no connection (and no exception)

            If myNativeConnection Is Nothing Then
                CoreMessageHandler(message:="no connection established", procedure:="mssqldbdriver.hastable", messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            '*** check on rights - avoid recursion if we are looking for the User Table
            '** makes no sense since we are checkin before installation if we need to install
            'If Not CurrentSession.IsBootstrappingInstallation AndAlso tableID <> User.ConstPrimaryTableID Then
            '    If Not _currentUserValidation.ValidEntry AndAlso Not _currentUserValidation.HasReadRights Then
            '        If Not myconnection.VerifyUserAccess(accessRequest:=otAccessRight.[ReadOnly], loginOnFailed:=True) Then
            '            Call CoreMessageHandler(showmsgbox:=True, subname:="mssqlDBDriver.HasTable", tablename:=tableID, _
            '                                  message:="No right to read schema of database", messagetype:=otCoreMessageType.ApplicationError)
            '            Return False
            '        End If
            '    End If
            'End If


            Try
                SyncLock _internallock
                    smoconnection = myconnection.SMOConnection ' will be setup during internal connection
                    database = myconnection.Database

                    If smoconnection Is Nothing OrElse database Is Nothing Then
                        Call CoreMessageHandler(message:="SMO is not initialized", containerID:=tableid, _
                                              procedure:="mssqlDBDriver.hasTable", messagetype:=otCoreMessageType.InternalError)
                        Return Nothing
                    End If

                    database.Tables.Refresh()
                    Dim existsOnServer As Boolean = database.Tables.Contains(name:=nativeTablename)
                    Return existsOnServer
                End SyncLock


            Catch smoex As SmoException

                Dim sb As New StringBuilder
                sb.AppendLine("This is an SMO Exception")
                'Display the SMO exception message.
                sb.AppendLine(smoex.Message)
                'Display the sequence of non-SMO exceptions that caused the SMO exception.
                Dim ex As Exception
                ex = smoex.InnerException
                If ex Is Nothing Then
                Else
                    Do While ex.InnerException IsNot (Nothing)
                        sb.AppendLine(ex.InnerException.Message)
                        ex = ex.InnerException
                    Loop
                End If

                Call CoreMessageHandler(message:=sb.ToString, exception:=ex, containerID:=tableid, _
                                      procedure:="mssqlDBDriver.hasTable", messagetype:=otCoreMessageType.InternalError)
                Return False
            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception", exception:=ex, containerID:=tableid, _
                                      procedure:="mssqlDBDriver.hasTable", messagetype:=otCoreMessageType.InternalError)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Gets the table object.
        ''' </summary>
        ''' <param name="tablename">The tablename.</param>
        ''' <param name="createOrAlter">The create on missing.</param>
        ''' <param name="addToSchemaDir">The add to schema dir.</param>
        ''' <param name="NativeConnection">The native connection.</param>
        ''' <returns></returns>
        Public Overrides Function GetTable(tableID As String, _
                                           Optional createOrAlter As Boolean = False, _
                                           Optional ByRef connection As iormConnection = Nothing, _
                                           Optional ByRef nativeTableObject As Object = Nothing) As Object

            Dim aTable As Table
            Dim smoconnection As ServerConnection
            Dim database As Microsoft.SqlServer.Management.Smo.Database
            Dim localCreated As Boolean = False
            Dim myconnection As mssqlConnection
            Dim nativeTablename As String = GetNativeDBObjectName(tableID)

            If connection Is Nothing Then
                myconnection = _primaryConnection
            Else
                myconnection = connection
            End If
            ' ** important ! Access to native Internal Connection opens up the internal connection and also the SMO Driver
            ' **
            If myconnection Is Nothing Or myconnection.NativeInternalConnection Is Nothing Then
                Call CoreMessageHandler(message:="internal connection connection is nothing - no table can be retrieved", procedure:="mssqlDBDriver.GetTable", _
                                            messagetype:=otCoreMessageType.InternalError, containerID:=tableID, argument:=nativeTablename)
                Return Nothing
            End If

            '*** check on rights
            If createOrAlter And Not CurrentSession.IsBootstrappingInstallationRequested Then
                If Not myconnection.VerifyUserAccess(accessRequest:=otAccessRight.AlterSchema, useLoginWindow:=True) Then
                    Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.GetTable", containerID:=tableID, argument:=nativeTablename, _
                                          message:="No right to alter schema of database", messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If
            End If



            Try
                SyncLock _internallock
                    smoconnection = myconnection.SMOConnection
                    database = myconnection.Database

                    If smoconnection Is Nothing OrElse database Is Nothing Then
                        Call CoreMessageHandler(message:="SMO is not initialized", containerID:=tableID, _
                                              procedure:="mssqlDBDriver.getTable", messagetype:=otCoreMessageType.InternalError)
                        Return Nothing
                    End If

                    database.Tables.Refresh()
                    Dim existsOnServer As Boolean = database.Tables.Contains(name:=nativeTablename)

                    '*** Exists and nothing supplied -> get it
                    If existsOnServer And (nativeTableObject Is Nothing OrElse nativeTableObject.GetType <> GetType(Table)) Then
                        aTable = database.Tables(nativeTablename)
                        aTable.Refresh()
                        Return aTable

                        '*** Doesnot Exist, create and nothing supplied -> createLocal Object
                    ElseIf Not existsOnServer And createOrAlter And (nativeTableObject Is Nothing OrElse nativeTableObject.GetType <> GetType(Table)) Then
                        aTable = New Table(database, name:=nativeTablename)
                        localCreated = True
                    Else
                        aTable = nativeTableObject
                    End If

                    '*** No CreateAlter -> return the Object
                    '*** CreatorAlter but the Object exists and was localCreated (means no object transmitted for change)
                    '*** return the refreshed

                    If (Not createOrAlter Or localCreated) AndAlso existsOnServer Then
                        If Not aTable Is Nothing Then aTable.Refresh()
                        Return aTable

                        '** doesnot Exist 
                    ElseIf (Not createOrAlter And Not existsOnServer) Then
                        Call CoreMessageHandler(procedure:="mssqlDBDriver.gettable", message:="Table does not exist", messagetype:=otCoreMessageType.InternalWarning, _
                                               break:=False, containerID:=tableID, argument:=nativeTablename)
                        Return Nothing
                    End If

                    '** create the table
                    '**
                    If createOrAlter Then
                        If Not localCreated And Not myconnection.Database.Tables.Contains(name:=nativeTablename) Then
                            aTable.Create()
                        ElseIf myconnection.Database.Tables.Contains(name:=nativeTablename) Then
                            aTable.Alter()
                        End If

                        Return aTable
                    Else
                        Call CoreMessageHandler(procedure:="mssqlDBDriver.getTable", containerID:=nativeTablename, _
                                              message:="Table was not found in database", messagetype:=otCoreMessageType.ApplicationWarning)
                        Return Nothing
                    End If
                End SyncLock

                Return aTable

            Catch smoex As SmoException

                Dim sb As New StringBuilder
                sb.AppendLine("This is an SMO Exception")
                'Display the SMO exception message.
                sb.AppendLine(smoex.Message)
                'Display the sequence of non-SMO exceptions that caused the SMO exception.
                Dim ex As Exception
                ex = smoex.InnerException
                If ex Is Nothing Then
                Else
                    Do While ex.InnerException IsNot (Nothing)
                        sb.AppendLine(ex.InnerException.Message)
                        ex = ex.InnerException
                    Loop
                End If

                Call CoreMessageHandler(message:=sb.ToString, exception:=ex, containerID:=tableID, _
                                      procedure:="mssqlDBDriver.getTable", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception", exception:=ex, containerID:=tableID, _
                                      procedure:="mssqlDBDriver.getTable", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End Try

        End Function


        ''' <summary>
        ''' Gets the index.
        ''' </summary>
        ''' <param name="nativeTABLE">The native TABLE.</param>
        ''' <param name="indexname">The indexname.</param>
        ''' <param name="ColumnNames">The column names.</param>
        ''' <param name="PrimaryKey">The primary key.</param>
        ''' <param name="forceCreation">The force creation.</param>
        ''' <param name="createOrAlter">The create on missing.</param>
        ''' <returns></returns>
        Public Overrides Function GetIndex(ByRef nativeTable As Object, _
                                           ByRef indexdefinition As ormIndexDefinition, _
                                            Optional ByVal forceCreation As Boolean = False, _
                                            Optional ByVal createOrAlter As Boolean = False, _
                                             Optional ByRef connection As iormConnection = Nothing) As Object Implements iormRelationalDatabaseDriver.GetIndex


            Dim aTable As Object 'keep it as object

            Dim myconnection As mssqlConnection
            Dim existingIndex As Boolean = False
            Dim indexnotchanged As Boolean = False
            Dim aIndexColumn As IndexedColumn
            Dim existPrimaryName As String = String.Empty
            Dim anIndex As Index
            Dim i As UShort = 0
            Dim nativeIndexname As String = GetNativeIndexname(indexdefinition.Tablename & "_" & indexdefinition.Name)

            If connection Is Nothing Then
                myconnection = _primaryConnection
            Else
                myconnection = connection
            End If
            '*** object
            If Not nativeTable.GetType = GetType(Table) AndAlso Not nativeTable.GetType = GetType(View) Then
                Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.getIndex", _
                                             message:="No SMO Table or View Object given to function", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            ElseIf nativeTable.GetType = GetType(Table) Then
                aTable = DirectCast(nativeTable, Table)
            ElseIf nativeTable.GetType = GetType(View) Then
                aTable = DirectCast(nativeTable, View)
            End If
            ' ** important ! Access to native Internal Connection opens up the internal connection and also the SMO Driver
            ' **
            If myconnection Is Nothing Or myconnection.NativeInternalConnection Is Nothing Then
                Call CoreMessageHandler(message:="internal connection connection is nothing - no table can be retrieved", procedure:="mssqlDBDriver.getIndex", _
                                            messagetype:=otCoreMessageType.InternalError, argument:=indexdefinition.Name, containerID:=aTable.Name)
                Return Nothing
            End If

            '*** check on rights
            If createOrAlter And Not CurrentSession.IsBootstrappingInstallationRequested Then
                If Not myconnection.VerifyUserAccess(otAccessRight.AlterSchema) Then
                    Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.getIndex", argument:=indexdefinition.Name, containerID:=aTable.Name, _
                                          message:="No right to alter schema of database", messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If
            End If


            Try

                SyncLock _internallock
                    '**
                    If aTable.Indexes.Count = 0 Then aTable.Refresh()

                    ' save the primary name
                    For Each index As Index In aTable.Indexes
                        If LCase(index.Name) = LCase(nativeIndexname) OrElse _
                            index.Name.ToLower = indexdefinition.NativeIndexname.ToLower OrElse _
                            LCase(index.Name) = LCase(aTable.Name & "_" & nativeIndexname) Then

                            existingIndex = True
                            anIndex = index
                        End If
                        If index.IndexKeyType = IndexKeyType.DriPrimaryKey Then
                            existPrimaryName = index.Name
                            If indexdefinition.Name = String.Empty Then
                                indexdefinition.NativeIndexname = nativeIndexname
                                existingIndex = True
                                anIndex = index
                            End If
                        End If
                    Next

                    '** check on changes
                    If (aTable.Indexes.Contains(name:=LCase(nativeIndexname)) OrElse _
                        aTable.Indexes.Contains(name:=indexdefinition.NativeIndexname) OrElse _
                        aTable.Indexes.Contains(name:=LCase(aTable.Name & "_" & nativeIndexname))) _
                        And Not forceCreation Then

                        If aTable.Indexes.Contains(name:=LCase(nativeIndexname)) Then
                            anIndex = aTable.Indexes(name:=LCase(nativeIndexname))
                        ElseIf aTable.Indexes.Contains(name:=indexdefinition.NativeIndexname) Then
                            anIndex = aTable.Indexes(name:=indexdefinition.NativeIndexname)
                        Else
                            anIndex = aTable.Indexes(name:=LCase(aTable.Name & "_" & nativeIndexname))
                        End If
                        ' check all Members
                        If Not forceCreation And existingIndex Then
                            i = 0
                            For Each columnName As String In indexdefinition.Columnnames
                                ' check
                                If anIndex.IndexedColumns.Count - 1 < i Then
                                    indexnotchanged = True
                                    Exit For
                                ElseIf columnName IsNot Nothing AndAlso columnName <> String.Empty Then

                                    ' not equal
                                    aIndexColumn = anIndex.IndexedColumns(i)
                                    If LCase(aIndexColumn.Name) <> LCase(columnName) Then
                                        indexnotchanged = False
                                        Exit For
                                    Else
                                        indexnotchanged = True
                                    End If
                                End If
                                ' exit
                                If Not indexnotchanged Then
                                    Exit For
                                End If
                                i = i + 1
                            Next columnName
                            ' return
                            If indexnotchanged Then
                                Return anIndex
                            End If
                        End If

                        '** exit
                    ElseIf Not createOrAlter Then

                        Call CoreMessageHandler(message:="index does not exist", procedure:="mssqlDBDriver.getIndex", argument:=indexdefinition.Name, _
                                               containerID:=aTable.Name, messagetype:=otCoreMessageType.InternalError)

                        Return Nothing

                    End If

                    '** create
                    myconnection.IsNativeInternalLocked = True

                    ' if we have another Primary
                    If indexdefinition.IsPrimary And LCase(nativeIndexname) <> LCase(existPrimaryName) And existPrimaryName <> String.Empty Then
                        'indexdefinition.Name is found and not the same ?!
                        Call CoreMessageHandler(message:="indexdefinition.Name of table " & aTable.Name & " is " & anIndex.Name & " and not " & indexdefinition.Name & " - getOTDBIndex aborted", _
                                              messagetype:=otCoreMessageType.InternalError, procedure:="mssqlDBDriver.getIndex", argument:=indexdefinition.Name, containerID:=indexdefinition.Tablename)
                        Return Nothing
                        ' create primary key
                    ElseIf indexdefinition.IsPrimary And String.IsNullOrWhiteSpace(existPrimaryName) Then
                        'create primary
                        If String.IsNullOrWhiteSpace(indexdefinition.NativeIndexname) Then
                            indexdefinition.NativeIndexname = nativeIndexname
                        End If

                        anIndex = New Index(parent:=aTable, name:=indexdefinition.NativeIndexname)
                        anIndex.IndexKeyType = IndexKeyType.DriPrimaryKey
                        anIndex.IndexType = IndexType.NonClusteredIndex
                        anIndex.IgnoreDuplicateKeys = False
                        anIndex.IsUnique = True

                        '** extend indexdefinition.isprimary
                    ElseIf indexdefinition.IsPrimary And LCase(nativeIndexname) = LCase(existPrimaryName) Then
                        '* DROP !
                        anIndex.Drop()

                        '* create
                        If String.IsNullOrWhiteSpace(indexdefinition.NativeIndexname) Then
                            indexdefinition.NativeIndexname = nativeIndexname
                        End If

                        anIndex = New Index(parent:=aTable, name:=indexdefinition.NativeIndexname)
                        anIndex.IndexKeyType = IndexKeyType.DriPrimaryKey
                        anIndex.IndexType = IndexType.NonClusteredIndex
                        anIndex.IgnoreDuplicateKeys = False
                        anIndex.IsUnique = True
                        'anIndex.Recreate()

                        '** extend Index -> Drop
                    ElseIf Not indexdefinition.IsPrimary And existingIndex Then
                        anIndex.Drop()
                        If String.IsNullOrWhiteSpace(indexdefinition.NativeIndexname) Then
                            indexdefinition.NativeIndexname = nativeIndexname
                        End If

                        anIndex = New Index(parent:=aTable, name:=indexdefinition.NativeIndexname)
                        anIndex.IndexKeyType = IndexKeyType.None
                        anIndex.IgnoreDuplicateKeys = Not indexdefinition.IsUnique
                        anIndex.IsUnique = indexdefinition.IsUnique

                        '** create filtered index if one of columns is nullable
                        If indexdefinition.IsUnique Then
                            For Each columnName As String In indexdefinition.Columnnames
                                Dim filterstr As String = String.Empty
                                If aTable.Columns.Contains(columnName) AndAlso aTable.Columns.Item(columnName).Nullable Then
                                    If filterstr <> String.Empty Then filterstr &= " AND "
                                    filterstr &= columnName & " is not null "
                                End If
                                If filterstr <> String.Empty Then
                                    anIndex.FilterDefinition = filterstr
                                End If
                            Next
                        End If
                        '** create new
                    ElseIf Not indexdefinition.IsPrimary And Not existingIndex Then
                        If String.IsNullOrWhiteSpace(indexdefinition.NativeIndexname) Then
                            indexdefinition.NativeIndexname = nativeIndexname
                        End If

                        anIndex = New Index(parent:=aTable, name:=indexdefinition.NativeIndexname)
                        anIndex.IndexKeyType = IndexKeyType.None
                        anIndex.IgnoreDuplicateKeys = Not indexdefinition.IsUnique
                        anIndex.IsUnique = indexdefinition.IsUnique
                        '** create filtered index if one of columns is nullable
                        If indexdefinition.IsUnique Then
                            For Each columnName As String In indexdefinition.Columnnames
                                Dim filterstr As String = String.Empty
                                If aTable.Columns.Contains(columnName) AndAlso aTable.Columns.Item(columnName).Nullable Then
                                    If filterstr <> String.Empty Then filterstr &= " AND "
                                    filterstr &= columnName & " is not null "
                                End If
                                If filterstr <> String.Empty Then
                                    anIndex.FilterDefinition = filterstr
                                End If
                            Next
                        End If
                    End If


                    ' check on keys & indexes
                    For Each aColumnname As String In indexdefinition.Columnnames
                        Dim indexColumn As IndexedColumn = New IndexedColumn(anIndex, aColumnname)
                        anIndex.IndexedColumns.Add(indexColumn)
                    Next

                    ' attach the Index
                    If Not anIndex Is Nothing Then
                        anIndex.Create()
                        Return anIndex
                    Else
                        Return Nothing
                    End If

                End SyncLock


            Catch smoex As SmoException

                Dim sb As New StringBuilder
                sb.AppendLine("This is an SMO Exception")
                'Display the SMO exception message.
                sb.AppendLine(smoex.Message)
                'Display the sequence of non-SMO exceptions that caused the SMO exception.
                Dim ex As Exception
                ex = smoex.InnerException
                If ex Is Nothing Then
                Else
                    Do While ex.InnerException IsNot (Nothing)
                        sb.AppendLine(ex.InnerException.Message)
                        ex = ex.InnerException
                    Loop
                End If

                Call CoreMessageHandler(message:=sb.ToString, exception:=ex, containerID:=aTable.Name, argument:=indexdefinition.Name, _
                                      procedure:="mssqlDBDriver.GetIndex", messagetype:=otCoreMessageType.InternalError)
                myconnection.IsNativeInternalLocked = False
                Return Nothing

            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.GetIndex", argument:=indexdefinition.Name, containerID:=aTable.Name, _
                                           message:="Exception", exception:=ex, messagetype:=otCoreMessageType.InternalError)
                myconnection.IsNativeInternalLocked = False
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' returns True if table Id has columnname in datastore
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="columnname"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function HasColumn(tableID As String, columnname As String, Optional ByRef connection As iormConnection = Nothing) As Boolean
            Dim aTable As Table
            Dim smoconnection As ServerConnection
            Dim database As Microsoft.SqlServer.Management.Smo.Database
            Dim myconnection As mssqlConnection
            Dim nativeTablename As String = GetNativeDBObjectName(tableID)

            If connection Is Nothing Then
                myconnection = _primaryConnection
            Else
                myconnection = connection
            End If

            ' ** important ! Access to native Internal Connection opens up the internal connection and also the SMO Driver
            ' **
            If myconnection Is Nothing Or myconnection.NativeInternalConnection Is Nothing Then
                Call CoreMessageHandler(message:="internal connection connection is nothing - no table can be retrieved", procedure:="mssqlDBDriver.HasColumn", _
                                            messagetype:=otCoreMessageType.InternalError, containerID:=tableID, argument:=columnname)
                Return Nothing
            End If

            '*** check on rights
            '*** makes no sense
            'If Not CurrentSession.IsBootstrappingInstallation Then
            '    If Not myconnection.VerifyUserAccess(accessRequest:=otAccessRight.ReadOnly, loginOnFailed:=True) Then
            '        Call CoreMessageHandler(showmsgbox:=True, subname:="mssqlDBDriver.hasColumn", _
            '                              message:="No right to read schema of database", messagetype:=otCoreMessageType.ApplicationError)
            '        Return Nothing
            '    End If
            'End If



            Try
                myconnection.IsNativeInternalLocked = True
                SyncLock _internallock

                    smoconnection = myconnection.SMOConnection
                    database = myconnection.Database

                    If smoconnection Is Nothing OrElse database Is Nothing Then
                        Call CoreMessageHandler(message:="SMO is not initialized", containerID:=tableID, argument:=nativeTablename, _
                                              procedure:="mssqlDBDriver.hasTable", messagetype:=otCoreMessageType.InternalError)
                        Return Nothing
                    Else
                        myconnection.IsNativeInternalLocked = True
                    End If

                    database.Tables.Refresh()
                    Dim existsOnServer As Boolean = database.Tables.Contains(name:=nativeTablename)
                    If Not existsOnServer Then
                        Return False
                    End If
                    aTable = database.Tables.Item(nativeTablename)


                    '**
                    If aTable.Columns.Count = 0 Then aTable.Refresh()

                    If aTable.Columns.Contains(name:=columnname) Then
                        Return True
                    Else
                        Return False
                    End If

                End SyncLock


            Catch smoex As SmoException

                Dim sb As New StringBuilder
                sb.AppendLine("This is an SMO Exception")
                'Display the SMO exception message.
                sb.AppendLine(smoex.Message)
                'Display the sequence of non-SMO exceptions that caused the SMO exception.
                Dim ex As Exception
                ex = smoex.InnerException
                If ex Is Nothing Then
                Else
                    Do While ex.InnerException IsNot (Nothing)
                        sb.AppendLine(ex.InnerException.Message)
                        ex = ex.InnerException
                    Loop
                End If

                Call CoreMessageHandler(message:=sb.ToString, exception:=ex, entryname:=columnname, argument:=nativeTablename, containerID:=tableID, _
                                      procedure:="mssqlDBDriver.hasColumn", messagetype:=otCoreMessageType.InternalError)
                ' rturn and do not change !
                myconnection.IsNativeInternalLocked = False
                Return Nothing

            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.hasColumn", entryname:=columnname, argument:=nativeTablename, containerID:=tableID, _
                                           message:="Exception", exception:=ex, messagetype:=otCoreMessageType.InternalError)
                ' rturn and do not change !
                myconnection.IsNativeInternalLocked = False
                Return Nothing
            End Try
        End Function
        '''' <summary>
        '''' returns True if table Id has column name in data store
        '''' </summary>
        '''' <param name="tablename"></param>
        '''' <param name="columnname"></param>
        '''' <param name="nativeConnection"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Overrides Function VerifyColumnSchema(columndefinition As iormContainerEntryDefinition, Optional ByRef connection As iormConnection = Nothing, Optional silent As Boolean = False) As Boolean
        '    Dim aTable As Table
        '    Dim smoconnection As ServerConnection
        '    Dim database As Microsoft.SqlServer.Management.Smo.Database
        '    Dim myconnection As mssqlConnection
        '    Dim tableid As String = columndefinition.ContainerID
        '    Dim columnname As String = columndefinition.EntryName
        '    Dim nativetablename As String = GetNativeDBObjectName(tableid)

        '    If connection Is Nothing Then
        '        myconnection = _primaryConnection
        '    Else
        '        myconnection = connection
        '    End If

        '    ' ** important ! Access to native Internal Connection opens up the internal connection and also the SMO Driver
        '    ' **
        '    If myconnection Is Nothing Or myconnection.NativeInternalConnection Is Nothing Then
        '        Call CoreMessageHandler(message:="internal connection connection is nothing - no table can be retrieved", procedure:="mssqlDBDriver.HasColumn", _
        '                                    messagetype:=otCoreMessageType.InternalError, containerID:=tableid, argument:=nativetablename & "." & columnname)
        '        Return Nothing
        '    End If

        '    '*** check on rights
        '    '** do not session since we might checking this to get bootstrapping status before session is started
        '    If Not CurrentSession.IsBootstrappingInstallationRequested AndAlso Not myconnection.VerifyUserAccess(accessRequest:=otAccessRight.[ReadOnly], useLoginWindow:=True) Then
        '        Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.hasColumn", _
        '                                  message:="No right to read schema of database", messagetype:=otCoreMessageType.ApplicationError)
        '        Return Nothing
        '    End If



        '    Try
        '        myconnection.IsNativeInternalLocked = True
        '        SyncLock _internallock

        '            smoconnection = myconnection.SMOConnection
        '            database = myconnection.Database

        '            If smoconnection Is Nothing OrElse database Is Nothing Then
        '                Call CoreMessageHandler(message:="SMO is not initialized", containerID:=tableid, argument:=nativetablename, _
        '                                      procedure:="mssqlDBDriver.hasTable", messagetype:=otCoreMessageType.InternalError)
        '                Return Nothing
        '            Else
        '                myconnection.IsNativeInternalLocked = True
        '            End If

        '            database.Tables.Refresh()
        '            Dim existsOnServer As Boolean = database.Tables.Contains(name:=nativetablename)
        '            If Not existsOnServer Then
        '                Return False
        '            End If
        '            aTable = database.Tables.Item(nativetablename)


        '            '**
        '            If aTable.Columns.Count = 0 Then aTable.Refresh()

        '            '** check name
        '            If aTable.Columns.Contains(name:=columnname) Then
        '                Dim column = aTable.Columns(columnname)
        '                '** set standard sizes or other specials
        '                Select Case columndefinition.Datatype
        '                    Case otDataType.[Long]
        '                        If column.DataType.SqlDataType <> SqlDataType.BigInt Then
        '                            If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be Long", argument:=columndefinition.Datatype, _
        '                                           containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
        '                            Return False
        '                        End If

        '                    Case otDataType.Numeric
        '                        If column.DataType.SqlDataType <> SqlDataType.Real Then
        '                            If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be REAL", argument:=columndefinition.Datatype, _
        '                                         containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
        '                            Return False
        '                        End If
        '                    Case otDataType.List, otDataType.Text
        '                        If column.DataType.SqlDataType <> SqlDataType.NVarChar Then
        '                            If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be NVARCHAR", argument:=columndefinition.Datatype, _
        '                                        containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
        '                            Return False
        '                        End If
        '                        If columndefinition.Size > 0 Then
        '                            If column.DataType.MaximumLength < columndefinition.Size Then
        '                                If Not silent Then CoreMessageHandler(message:="verifying table column: column maximum length differs", argument:=columndefinition.Size, _
        '                                       containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)

        '                                Return False
        '                            End If
        '                        End If
        '                    Case otDataType.Memo
        '                        If column.DataType.SqlDataType <> SqlDataType.NVarCharMax Then
        '                            If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be NVARCHARMAX", argument:=columndefinition.Datatype, _
        '                                      containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
        '                            Return False
        '                        End If
        '                    Case otDataType.Binary
        '                        If column.DataType.SqlDataType <> SqlDataType.VarBinaryMax Then
        '                            If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be VARBINARYMAX", argument:=columndefinition.Datatype, _
        '                                     containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
        '                            Return False
        '                        End If
        '                    Case otDataType.[Date]
        '                        If column.DataType.SqlDataType <> SqlDataType.Date Then
        '                            If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be DATE", argument:=columndefinition.Datatype, _
        '                                    containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
        '                            Return False
        '                        End If
        '                    Case otDataType.Time
        '                        If column.DataType.SqlDataType <> SqlDataType.Time Then
        '                            If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be TIME", argument:=columndefinition.Datatype, _
        '                                    containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
        '                            Return False
        '                        End If
        '                    Case otDataType.Timestamp
        '                        If column.DataType.SqlDataType <> SqlDataType.DateTime Then
        '                            If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be DATETIME", argument:=columndefinition.Datatype, _
        '                                    containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
        '                            Return False
        '                        End If
        '                    Case otDataType.Bool
        '                        If column.DataType.SqlDataType <> SqlDataType.Bit Then
        '                            If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be BIT", argument:=columndefinition.Datatype, _
        '                                    containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
        '                            Return False
        '                        End If
        '                End Select


        '                Return True
        '            Else
        '                If Not silent Then CoreMessageHandler(message:="verifying table column: column does not exist in database ", _
        '                                          containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)


        '                Return False
        '            End If

        '        End SyncLock


        '    Catch smoex As SmoException

        '        Dim sb As New StringBuilder
        '        sb.AppendLine("This is an SMO Exception")
        '        'Display the SMO exception message.
        '        sb.AppendLine(smoex.Message)
        '        'Display the sequence of non-SMO exceptions that caused the SMO exception.
        '        Dim ex As Exception
        '        ex = smoex.InnerException
        '        If ex Is Nothing Then
        '        Else
        '            Do While ex.InnerException IsNot (Nothing)
        '                sb.AppendLine(ex.InnerException.Message)
        '                ex = ex.InnerException
        '            Loop
        '        End If

        '        Call CoreMessageHandler(message:=sb.ToString, exception:=ex, entryname:=columnname, containerID:=tableid, _
        '                              procedure:="mssqlDBDriver.hasColumn", messagetype:=otCoreMessageType.InternalError)
        '        ' rturn and do not change !
        '        myconnection.IsNativeInternalLocked = False
        '        Return Nothing

        '    Catch ex As Exception
        '        Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.hasColumn", entryname:=columnname, containerID:=tableid, _
        '                                   message:="Exception", exception:=ex, messagetype:=otCoreMessageType.InternalError)
        '        ' rturn and do not change !
        '        myconnection.IsNativeInternalLocked = False
        '        Return Nothing
        '    End Try
        'End Function
        ''' <summary>
        ''' returns True if table Id has columnname in datastore
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="columnname"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function VerifyColumnSchema(columndefinition As iormContainerEntryDefinition, Optional ByRef connection As iormConnection = Nothing, Optional silent As Boolean = False) As Boolean
            Dim aTable As Table
            Dim smoconnection As ServerConnection
            Dim database As Microsoft.SqlServer.Management.Smo.Database
            Dim myconnection As mssqlConnection
            Dim tableid As String = columndefinition.ContainerID
            Dim nativeTablename As String = GetNativeDBObjectName(tableid)
            Dim columnname As String = columndefinition.EntryName

            If connection Is Nothing Then
                myconnection = _primaryConnection
            Else
                myconnection = connection
            End If

            ' ** important ! Access to native Internal Connection opens up the internal connection and also the SMO Driver
            ' **
            If myconnection Is Nothing Or myconnection.NativeInternalConnection Is Nothing Then
                Call CoreMessageHandler(message:="internal connection connection is nothing - no table can be retrieved", procedure:="mssqlDBDriver.verifyColumnSchema", _
                                            messagetype:=otCoreMessageType.InternalError, containerID:=tableid, argument:=columnname)
                Return Nothing
            End If

            '*** check on rights
            '** do not session since we might checking this to get bootstrapping status before session is started
            If Not CurrentSession.IsBootstrappingInstallationRequested AndAlso Not myconnection.VerifyUserAccess(accessRequest:=otAccessRight.[ReadOnly], useLoginWindow:=True) Then
                Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.verifyColumnSchema", _
                                      message:="No right to read schema of database", messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If



            Try
                myconnection.IsNativeInternalLocked = True
                SyncLock _internallock

                    smoconnection = myconnection.SMOConnection
                    database = myconnection.Database

                    If smoconnection Is Nothing OrElse database Is Nothing Then
                        Call CoreMessageHandler(message:="SMO is not initialized", containerID:=tableid, argument:=nativeTablename, _
                                              procedure:="mssqlDBDriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
                        Return Nothing
                    Else
                        myconnection.IsNativeInternalLocked = True
                    End If

                    database.Tables.Refresh()
                    Dim existsOnServer As Boolean = database.Tables.Contains(name:=nativeTablename)
                    If Not existsOnServer Then
                        Return False
                    End If
                    aTable = database.Tables.Item(nativeTablename)


                    '**
                    If aTable.Columns.Count = 0 Then aTable.Refresh()

                    '** check name
                    If aTable.Columns.Contains(name:=columnname) Then
                        Dim column = aTable.Columns(columnname)
                        If columndefinition.DataType >= 0 Then
                            '** set standard sizes or other specials
                            Select Case columndefinition.DataType
                                Case otDataType.[Long]
                                    If column.DataType.SqlDataType <> SqlDataType.BigInt Then
                                        If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be Long", argument:=columndefinition.DataType, _
                                                       containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
                                        Return False
                                    End If

                                Case otDataType.Numeric
                                    If column.DataType.SqlDataType <> SqlDataType.Real Then
                                        If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be REAL", argument:=columndefinition.DataType, _
                                                     containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
                                        Return False
                                    End If
                                Case otDataType.List, otDataType.Text
                                    If column.DataType.SqlDataType <> SqlDataType.NVarChar Then
                                        If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be NVARCHAR", argument:=columndefinition.DataType, _
                                                    containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
                                        Return False
                                    End If
                                    If columndefinition.Size > 0 Then
                                        If column.DataType.MaximumLength < columndefinition.Size Then
                                            If Not silent Then CoreMessageHandler(message:="verifying table column: column maximum length differs", argument:=columndefinition.Size, _
                                                   containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)

                                            Return False
                                        End If
                                    End If
                                Case otDataType.Memo
                                    If column.DataType.SqlDataType <> SqlDataType.NVarCharMax Then
                                        If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be NVARCHARMAX", argument:=columndefinition.DataType, _
                                                  containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
                                        Return False
                                    End If
                                Case otDataType.Binary
                                    If column.DataType.SqlDataType <> SqlDataType.VarBinaryMax Then
                                        If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be VARBINARYMAX", argument:=columndefinition.DataType, _
                                                 containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
                                        Return False
                                    End If
                                Case otDataType.[Date]
                                    If column.DataType.SqlDataType <> SqlDataType.Date Then
                                        If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be DATE", argument:=columndefinition.DataType, _
                                                containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
                                        Return False
                                    End If
                                Case otDataType.Time
                                    If column.DataType.SqlDataType <> SqlDataType.Time Then
                                        If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be TIME", argument:=columndefinition.DataType, _
                                                containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
                                        Return False
                                    End If
                                Case otDataType.Timestamp
                                    If column.DataType.SqlDataType <> SqlDataType.DateTime Then
                                        If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be DATETIME", argument:=columndefinition.DataType, _
                                                containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
                                        Return False
                                    End If
                                Case otDataType.Bool
                                    If column.DataType.SqlDataType <> SqlDataType.Bit Then
                                        If Not silent Then CoreMessageHandler(message:="verifying table column: column data type differs - should be BIT", argument:=columndefinition.DataType, _
                                                containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
                                        Return False
                                    End If
                                Case otDataType.Runtime
                                Case otDataType.Formula
                                    If Not silent Then Call CoreMessageHandler(procedure:="mssqlDBDriver.verifyColumnSchema", containerID:=aTable.Name, argument:=columndefinition.EntryName, _
                                                           message:="runtime, formular not supported as fieldtypes", messagetype:=otCoreMessageType.InternalError)

                            End Select

                        End If

                        Return True
                    Else
                        If Not silent Then CoreMessageHandler(message:="verifying table column: column does not exist in database ", _
                                                containerID:=tableid, containerEntryName:=columnname, procedure:="mssqldbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)


                        Return False
                    End If

                End SyncLock


            Catch smoex As SmoException

                Dim sb As New StringBuilder
                sb.AppendLine("This is an SMO Exception")
                'Display the SMO exception message.
                sb.AppendLine(smoex.Message)
                'Display the sequence of non-SMO exceptions that caused the SMO exception.
                Dim ex As Exception
                ex = smoex.InnerException
                If ex Is Nothing Then
                Else
                    Do While ex.InnerException IsNot (Nothing)
                        sb.AppendLine(ex.InnerException.Message)
                        ex = ex.InnerException
                    Loop
                End If

                Call CoreMessageHandler(message:=sb.ToString, exception:=ex, entryname:=columnname, containerID:=tableid, _
                                      procedure:="mssqlDBDriver.hasColumn", messagetype:=otCoreMessageType.InternalError)
                ' rturn and do not change !
                myconnection.IsNativeInternalLocked = False
                Return Nothing

            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.hasColumn", entryname:=columnname, containerID:=tableid, _
                                           message:="Exception", exception:=ex, messagetype:=otCoreMessageType.InternalError)
                ' rturn and do not change !
                myconnection.IsNativeInternalLocked = False
                Return Nothing
            End Try
        End Function
        ''' <summary>
        ''' Gets the column.
        ''' </summary>
        ''' <param name="nativeTABLE">The native TABLE.</param>
        ''' <param name="aDBDesc">A DB desc.</param>
        ''' <param name="createOrAlter">The create on missing.</param>
        ''' <param name="addToSchemaDir">The add to schema dir.</param>
        ''' <returns></returns>

        Public Overrides Function GetColumn(nativeTable As Object, columndefinition As iormContainerEntryDefinition, _
                                            Optional createOrAlter As Boolean = False, _
                                            Optional ByRef connection As iormConnection = Nothing) As Object Implements iormRelationalDatabaseDriver.GetColumn
            Dim myconnection As mssqlConnection

            If connection Is Nothing Then
                myconnection = _primaryConnection
            Else
                myconnection = connection
            End If
            ' ** important ! Access to native Internal Connection opens up the internal connection and also the SMO Driver
            ' **
            If myconnection Is Nothing Or myconnection.NativeInternalConnection Is Nothing Then
                Call CoreMessageHandler(message:="internal connection connection is nothing - no table can be retrieved", procedure:="mssqlDBDriver.GetColumn", _
                                            messagetype:=otCoreMessageType.InternalError, containerEntryName:=columndefinition.EntryName, containerID:=columndefinition.ContainerID)
                Return Nothing
            End If

            '*** check on rights
            If createOrAlter And Not CurrentSession.IsBootstrappingInstallationRequested Then
                If Not myconnection.VerifyUserAccess(accessRequest:=otAccessRight.AlterSchema, useLoginWindow:=True) Then
                    Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.GetColumn", containerEntryName:=columndefinition.EntryName, containerID:=columndefinition.ContainerID, _
                                          message:="No right to alter schema of database", messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If
            End If


            '*** object
            If Not nativeTable.GetType = GetType(Table) Then
                Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.GetColumn", containerEntryName:=columndefinition.EntryName, containerID:=columndefinition.ContainerID, _
                                             message:="No SMO TableObject given to function", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If

            Dim aTable As Table = DirectCast(nativeTable, Table)
            Dim newColumn As Column
            Dim aDatatype As New Microsoft.SqlServer.Management.Smo.DataType()
            Dim addColumn As Boolean = False

            Try

                SyncLock _internallock

                    '**
                    If aTable.Columns.Count = 0 Then aTable.Refresh()

                    If aTable.Columns.Contains(name:=columndefinition.EntryName) And Not createOrAlter Then
                        Return aTable.Columns(Name:=columndefinition.EntryName)
                    ElseIf Not aTable.Columns.Contains(name:=columndefinition.EntryName) And Not createOrAlter Then
                        Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.GetColumn", argument:=columndefinition.EntryName, containerID:=aTable.Name, _
                                                    message:="Column does not exist", messagetype:=otCoreMessageType.InternalError)
                        Return Nothing
                    Else



                        '**
                        '** create normal database column
                        '**
                        If aTable.Columns.Contains(name:=columndefinition.EntryName) Then
                            newColumn = aTable.Columns(Name:=columndefinition.EntryName)
                            aDatatype = newColumn.DataType
                        Else
                            newColumn = New Column(parent:=aTable, name:=columndefinition.EntryName)
                            aDatatype = New Microsoft.SqlServer.Management.Smo.DataType()
                            addColumn = True
                        End If


                        'aDatatype.SqlDataType = GetTargetTypeFor(columndefinition.Datatype) is not working since we have SMO sqldatatype here and not 
                        'isqltype

                        '** set standard sizes or other specials
                        Select Case columndefinition.Datatype
                            Case otDataType.[Long]
                                aDatatype.SqlDataType = SqlDataType.BigInt
                            Case otDataType.Numeric
                                aDatatype.SqlDataType = SqlDataType.Real

                            Case otDataType.List, otDataType.Text
                                aDatatype.SqlDataType = SqlDataType.NVarChar
                                If columndefinition.Size.HasValue Then
                                    aDatatype.MaximumLength = columndefinition.Size
                                Else
                                    aDatatype.MaximumLength = ConstDBDriverMaxTextSize
                                End If
                            Case otDataType.Memo
                                aDatatype.SqlDataType = SqlDataType.NVarCharMax
                            Case otDataType.Binary
                                aDatatype.SqlDataType = SqlDataType.VarBinaryMax
                            Case otDataType.[Date]
                                aDatatype.SqlDataType = SqlDataType.Date
                            Case otDataType.Time
                                aDatatype.SqlDataType = SqlDataType.Time
                                'aDatatype.MaximumLength = 7
                            Case otDataType.Timestamp
                                aDatatype.SqlDataType = SqlDataType.DateTime
                            Case otDataType.Bool
                                aDatatype.SqlDataType = SqlDataType.Bit
                            Case otDataType.Runtime
                            Case otDataType.Formula
                                Call CoreMessageHandler(procedure:="mssqlDBDriver.getColumn", containerID:=aTable.Name, argument:=columndefinition.EntryName, _
                                                       message:="runtime, formular not supported as fieldtypes", messagetype:=otCoreMessageType.InternalError)
                            Case Else
                                Call CoreMessageHandler(procedure:="mssqlDBDriver.getColumn", containerID:=aTable.Name, argument:=columndefinition.EntryName, _
                                                      message:="datatype not implemented", messagetype:=otCoreMessageType.InternalError)
                        End Select
                        newColumn.DataType = aDatatype
                        ' default value
                        If columndefinition.DBDefaultValue IsNot Nothing Then
                            If newColumn.DefaultConstraint IsNot Nothing Then newColumn.DefaultConstraint.Drop()
                            '** create a constraint name - setup specific
                            Dim aConstraintName As String
                            If String.IsNullOrWhiteSpace(ot.CurrentSetupID) Then
                                aConstraintName = "DEFAULT_" & nativeTable.name & "_" & columndefinition.EntryName
                            Else
                                aConstraintName = ot.CurrentSetupID & "_" & "DEFAULT_" & nativeTable.name & "_" & columndefinition.EntryName
                            End If

                            '** set the constraint
                            If columndefinition.DataType = otDataType.Time Then
                                newColumn.AddDefaultConstraint(aConstraintName).Text = _
                                    "'" & CDate(columndefinition.DBDefaultValue).ToString("HH:mm:ss") & "'"
                            ElseIf columndefinition.DataType = otDataType.Date Then
                                newColumn.AddDefaultConstraint(aConstraintName).Text = _
                                "'" & CDate(columndefinition.DBDefaultValue).ToString("yyyy-MM-dd") & "T00:00:00Z'"
                            ElseIf columndefinition.DataType = otDataType.Timestamp Then
                                newColumn.AddDefaultConstraint(aConstraintName).Text = _
                                    "'" & (Convert.ToDateTime(columndefinition.DBDefaultValue).ToString("yyyy-MM-ddTHH:mm:ssZ")) & "'"
                            ElseIf columndefinition.DataType = otDataType.Bool Then
                                If columndefinition.DBDefaultValue Then
                                    newColumn.AddDefaultConstraint(aConstraintName).Text = "1"
                                Else
                                    newColumn.AddDefaultConstraint(aConstraintName).Text = "0"
                                End If
                            ElseIf columndefinition.DataType = otDataType.Text OrElse columndefinition.DataType = otDataType.List Then
                                newColumn.AddDefaultConstraint(aConstraintName).Text = "'" & columndefinition.DBDefaultValue & "'"
                            ElseIf columndefinition.DataType = otDataType.Long OrElse columndefinition.DataType = otDataType.Numeric Then
                                newColumn.AddDefaultConstraint(aConstraintName).Text = columndefinition.DBDefaultValue.ToString
                            ElseIf Not String.IsNullOrEmpty(columndefinition.DBDefaultValue) Then
                                newColumn.AddDefaultConstraint(aConstraintName).Text = columndefinition.DBDefaultValue
                            End If

                        End If

                        ' per default Nullable
                        If aTable.State = SqlSmoState.Creating Then
                            newColumn.Nullable = columndefinition.IsNullable
                            ' SQL Server throws error if not nullable or default value on change
                        ElseIf columndefinition.DBDefaultValue Is Nothing Then
                            newColumn.Nullable = True
                        End If

                        '** enfore uniqueness
                        If columndefinition.IsUnique Then
                            newColumn.Identity = True
                        End If

                        '** extended Properties
                        newColumn.ExtendedProperties.Refresh()
                        If newColumn.ExtendedProperties.Contains("MS_Description") Then
                            newColumn.ExtendedProperties("MS_Description").Value = columndefinition.Description
                        Else
                            Dim newEP As ExtendedProperty = New ExtendedProperty(parent:=newColumn, name:="MS_Description", propertyValue:=columndefinition.Description)
                            newColumn.ExtendedProperties.Add(newEP)
                            'newEP.Create() -> doesnot work
                        End If

                        '** add it
                        If addColumn Then aTable.Columns.Add(newColumn)
                        '** unique ?

                        '*** return new column
                        Return newColumn

                    End If


                End SyncLock


            Catch smoex As SmoException

                Dim sb As New StringBuilder
                sb.AppendLine("This is an SMO Exception")
                'Display the SMO exception message.
                sb.AppendLine(smoex.Message)
                'Display the sequence of non-SMO exceptions that caused the SMO exception.
                Dim ex As Exception
                ex = smoex.InnerException
                If ex Is Nothing Then
                Else
                    Do While ex.InnerException IsNot (Nothing)
                        sb.AppendLine(ex.InnerException.Message)
                        ex = ex.InnerException
                    Loop
                End If

                Call CoreMessageHandler(message:=sb.ToString, exception:=ex, containerEntryName:=columndefinition.EntryName, containerID:=columndefinition.ContainerID, _
                                      procedure:="mssqlDBDriver.GetColumn", messagetype:=otCoreMessageType.InternalError)
                Return Nothing

            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.GetColumn", containerEntryName:=columndefinition.EntryName, containerID:=columndefinition.ContainerID, _
                                           message:="Exception", exception:=ex, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End Try
        End Function
        ''' <summary>
        ''' Gets the foreign keys.
        ''' </summary>
        ''' <param name="nativeTABLE">The native TABLE.</param>
        ''' <param name="aDBDesc">A DB desc.</param>
        ''' <param name="createOrAlter">The create on missing.</param>
        ''' <param name="addToSchemaDir">The add to schema dir.</param>
        ''' <returns></returns>

        Public Overrides Function GetForeignKeys(nativeTable As Object, foreignkeydefinition As ormForeignKeyDefinition, _
                                            Optional createOrAlter As Boolean = False, _
                                            Optional ByRef connection As iormConnection = Nothing) As IEnumerable(Of Object) Implements iormRelationalDatabaseDriver.GetForeignKeys

            Dim myconnection As mssqlConnection

            If connection Is Nothing Then
                myconnection = _primaryConnection
            Else
                myconnection = connection
            End If
            ' ** important ! Access to native Internal Connection opens up the internal connection and also the SMO Driver
            ' **
            If myconnection Is Nothing Or myconnection.NativeInternalConnection Is Nothing Then
                Call CoreMessageHandler(message:="internal connection connection is nothing - no table can be retrieved", procedure:="mssqlDBDriver.GetColumn", _
                                            messagetype:=otCoreMessageType.InternalError, containerEntryName:=foreignkeydefinition.Id, containerID:=foreignkeydefinition.Tablename)
                Return Nothing
            End If

            '*** check on rights
            If createOrAlter And Not CurrentSession.IsBootstrappingInstallationRequested Then
                If Not myconnection.VerifyUserAccess(accessRequest:=otAccessRight.AlterSchema, useLoginWindow:=True) Then
                    Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.getForeignKey", containerEntryName:=foreignkeydefinition.Id, containerID:=foreignkeydefinition.Tablename, _
                                          message:="No right to alter schema of database", messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If
            End If


            '*** object
            If Not nativeTable.GetType = GetType(Table) Then
                Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.getForeignKey", containerEntryName:=foreignkeydefinition.Id, containerID:=foreignkeydefinition.Tablename, _
                                             message:="No SMO TableObject given to function", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If

            Dim aTable As Table = DirectCast(nativeTable, Table)

            Try

                SyncLock _internallock

                    '**
                    If aTable.Columns.Count = 0 Then aTable.Refresh()

                    Dim i As UShort
                    Dim resultkeys As New List(Of ForeignKey)
                    Dim alterflag As Boolean
                    '*** just return 
                    If Not createOrAlter Then

                        For Each aForeignkey As ForeignKey In aTable.ForeignKeys
                            For Each aColumnname In foreignkeydefinition.ColumnNames
                                If aForeignkey.Columns.Contains(aColumnname) Then
                                    resultkeys.Add(aForeignkey)
                                    Exit For
                                End If
                            Next
                        Next

                        Return resultkeys

                        '***
                        '*** Drop all Foreign Key usages if not nativeDatabase
                    ElseIf createOrAlter And (foreignkeydefinition.UseForeignKey <> otForeignKeyImplementation.NativeDatabase) Then
                        CoreMessageHandler(message:="Foreign Key usage is not 'native database' - drop all existing foreign keys", containerID:=aTable.Name, _
                                           argument:=foreignkeydefinition.Id, _
                                               procedure:="mssqlDBDriver.getForeignKey", messagetype:=otCoreMessageType.InternalWarning)

                        '** delete all existing key
                        alterflag = False
                        Dim aList As New List(Of ForeignKey)
                        For Each aexistingkey As ForeignKey In aTable.ForeignKeys
                            aList.Add(aexistingkey)
                            alterflag = True
                        Next
                        If alterflag Then
                            For Each existingkey In aList
                                existingkey.Drop()
                            Next
                        End If
                        Return resultkeys


                        '**
                        '** create foreign key
                        '**
                    ElseIf createOrAlter AndAlso foreignkeydefinition.UseForeignKey And otForeignKeyImplementation.NativeDatabase Then
                        Dim theColumnnames As String()
                        Dim theFKColumnnames As String()
                        Dim theFKTablenames As String()
                        Dim fkproperties As List(Of ForeignKeyProperty)
                        Dim aForeignKeyName As String = GetNativeForeignkeyName(foreignkeydefinition.Id)
                        Dim fkerror As Boolean = False

                        If foreignkeydefinition Is Nothing OrElse foreignkeydefinition.ForeignKeyReferences Is Nothing _
                            OrElse foreignkeydefinition.ForeignKeyReferences.Count = 0 Then
                            CoreMessageHandler(message:="Foreign Key Reference of column definition is not set - drop all existing foreign keys", containerID:=aTable.Name, _
                                               argument:=foreignkeydefinition.Id, _
                                                procedure:="mssqlDBDriver.getForeignKey", messagetype:=otCoreMessageType.InternalError)

                            '** delete all existing key
                            alterflag = False
                            Dim aList As New List(Of ForeignKey)
                            For Each aexistingkey As ForeignKey In aTable.ForeignKeys
                                aList.Add(aexistingkey)
                                alterflag = True
                            Next
                            If alterflag Then
                                For Each existingkey In aList
                                    existingkey.Drop()
                                Next
                            End If
                            fkerror = True
                        Else
                            '** check count
                            If foreignkeydefinition.ForeignKeyReferences.Count <> foreignkeydefinition.ColumnNames.Count Then
                                CoreMessageHandler(message:="number of foreign Key references is different then the number of columnnames ", _
                                                      containerID:=aTable.Name, argument:=foreignkeydefinition.Id, _
                                                       procedure:="mssqlDBDriver.getForeignKey", messagetype:=otCoreMessageType.InternalError)
                                Return Nothing
                                fkerror = True
                            End If



                            '** do bookeeping for new foreign keys for this table
                            '**
                            Dim no As UShort = foreignkeydefinition.ForeignKeyReferences.Count
                            ReDim theColumnnames(no - 1)
                            ReDim theFKColumnnames(no - 1)
                            ReDim theFKTablenames(no - 1)
                            Dim anTableColumnAttribute As ormContainerEntryAttribute
                            i = 0

                            For i = 0 To no - 1
                                Dim afkreference As String = foreignkeydefinition.ForeignKeyReferences(i)
                                '** complete reference
                                If Not afkreference.Contains("."c) And Not afkreference.Contains(ConstDelimiter) Then
                                    CoreMessageHandler(message:="Foreign Key Reference of column definition has no object name part divided by '.'", _
                                                       containerID:=aTable.Name, containerEntryName:=foreignkeydefinition.Id, _
                                                        argument:=afkreference, procedure:="mssqlDBDriver.getForeignKey", messagetype:=otCoreMessageType.InternalError)
                                    fkerror = True
                                Else
                                    Dim names = Shuffle.NameSplitter(afkreference)
                                    theFKTablenames(i) = names(0).Clone
                                    theFKColumnnames(i) = names(1).Clone
                                    If fkproperties Is Nothing AndAlso foreignkeydefinition.ForeignKeyProperty IsNot Nothing Then fkproperties = foreignkeydefinition.ForeignKeyProperty
                                    names = Shuffle.NameSplitter(foreignkeydefinition.ColumnNames(i))
                                    If names.Count > 0 Then
                                        theColumnnames(i) = names(1).Clone
                                    Else
                                        theColumnnames(i) = names(0).Clone
                                    End If

                                    '** resolve the reference - must be loaded previously
                                    Dim anColumnEntry = CurrentSession.Objects.GetContainerEntry(entryname:=theFKColumnnames(i), containerid:=theFKTablenames(i), runtimeOnly:=foreignkeydefinition.RunTimeOnly)
                                    If anColumnEntry Is Nothing Then
                                        anTableColumnAttribute = ot.GetSchemaTableColumnAttribute(columnname:=theFKColumnnames(i), tableid:=theFKTablenames(i))
                                        If anTableColumnAttribute Is Nothing Then
                                            CoreMessageHandler(message:="Foreign Key Reference of column definition was not found in the object repository - foreign key not set", _
                                                               containerID:=aTable.Name, containerEntryName:=theFKColumnnames(i), _
                                                               argument:=afkreference, procedure:="mssqlDBDriver.getForeignKey", _
                                                               messagetype:=otCoreMessageType.InternalError)
                                            fkerror = True
                                        Else
                                            If anTableColumnAttribute.HasValueContainerEntryName Then theFKColumnnames(i) = anTableColumnAttribute.ContainerEntryName
                                            If anTableColumnAttribute.HasValueContainerID Then theFKTablenames(i) = anTableColumnAttribute.ContainerID
                                            If fkproperties Is Nothing AndAlso anTableColumnAttribute.HasValueForeignKeyProperties Then fkproperties = anTableColumnAttribute.ForeignKeyProperty.ToList
                                        End If
                                    End If

                                End If

                            Next
                        End If

                        '*** create keys
                        '***
                        If Not fkerror Then
                            Dim aforeignkey As ForeignKey
                            Dim uniquetables = theFKTablenames.Distinct.ToArray
                            i = 0
                            For Each aFKTablename In uniquetables

                                '** delete existing key
                                If aTable.ForeignKeys.Contains(aForeignKeyName & "_" & i) Then
                                    aTable.ForeignKeys.Item(aForeignKeyName & "_" & i).Drop()
                                End If

                                '** rebuild
                                aforeignkey = New ForeignKey(aTable, aForeignKeyName & "_" & i)
                                'Add columns as the foreign key column.
                                For i = 0 To theFKColumnnames.Count - 1
                                    If theFKTablenames(i) = aFKTablename Then
                                        If theColumnnames(i) IsNot Nothing AndAlso theFKColumnnames(i) IsNot Nothing Then
                                            Dim fkColumn As ForeignKeyColumn
                                            fkColumn = New ForeignKeyColumn(aforeignkey, theColumnnames(i), theFKColumnnames(i))
                                            aforeignkey.Columns.Add(fkColumn)
                                        End If
                                    End If
                                Next
                                'Set the referenced table and schema.
                                aforeignkey.ReferencedTable = Me.GetNativeDBObjectName(aFKTablename)
                                aforeignkey.IsEnabled = True

                                'foreignkey.ReferencedTableSchema 
                                If fkproperties IsNot Nothing Then
                                    For Each [aProperty] In fkproperties
                                        If aProperty.Enum = otForeignKeyProperty.OnUpdate Then
                                            Select Case aProperty.ActionProperty.Enum
                                                Case otForeignKeyAction.Cascade
                                                    aforeignkey.UpdateAction = ForeignKeyAction.Cascade
                                                Case otForeignKeyAction.SetDefault
                                                    aforeignkey.UpdateAction = ForeignKeyAction.SetDefault
                                                Case otForeignKeyAction.SetNull
                                                    aforeignkey.UpdateAction = ForeignKeyAction.SetNull
                                                Case otForeignKeyAction.Restrict
                                                    CoreMessageHandler(message:="Restricted foreign key action OnUpdate is not implemented in MS-SQLServer", messagetype:=otCoreMessageType.InternalError, _
                                                                       procedure:="mssqlDBDriver.getForeignKey")
                                                Case otForeignKeyAction.Noop
                                                    aforeignkey.UpdateAction = ForeignKeyAction.NoAction
                                                Case Else
                                                    CoreMessageHandler(message:="Restricted foreign key action OnUpdate not implemented ", argument:=aProperty.ActionProperty.ToString, messagetype:=otCoreMessageType.InternalError, _
                                                                      procedure:="mssqlDBDriver.getForeignKey")
                                            End Select
                                        ElseIf aProperty.Enum = otForeignKeyProperty.OnDelete Then
                                            Select Case aProperty.ActionProperty.Enum
                                                Case otForeignKeyAction.Cascade
                                                    aforeignkey.DeleteAction = ForeignKeyAction.Cascade
                                                Case otForeignKeyAction.SetDefault
                                                    aforeignkey.DeleteAction = ForeignKeyAction.SetDefault
                                                Case otForeignKeyAction.SetNull
                                                    aforeignkey.DeleteAction = ForeignKeyAction.SetNull
                                                Case otForeignKeyAction.Restrict
                                                    CoreMessageHandler(message:="Restricted foreign key action for OnDelete is not implemented in MS-SQLServer", messagetype:=otCoreMessageType.InternalError, _
                                                                       procedure:="mssqlDBDriver.getForeignKey")
                                                Case otForeignKeyAction.Noop
                                                    aforeignkey.DeleteAction = ForeignKeyAction.NoAction
                                                Case Else
                                                    CoreMessageHandler(message:="Restricted foreign key action for OnDelete not implemented ", argument:=aProperty.ActionProperty.ToString, messagetype:=otCoreMessageType.InternalError, _
                                                                      procedure:="mssqlDBDriver.getForeignKey")
                                            End Select

                                        End If
                                    Next
                                End If
                                'Create the foreign key on the instance of SQL Server.
                                aforeignkey.Create()
                                resultkeys.Add(aforeignkey)
                            Next

                        End If

                        Return resultkeys
                    End If


                End SyncLock


            Catch smoex As SmoException

                Dim sb As New StringBuilder
                sb.AppendLine("This is an SMO Exception")
                'Display the SMO exception message.
                sb.AppendLine(smoex.Message)
                'Display the sequence of non-SMO exceptions that caused the SMO exception.
                Dim ex As Exception
                ex = smoex.InnerException
                If ex Is Nothing Then
                Else
                    Do While ex.InnerException IsNot (Nothing)
                        sb.AppendLine(ex.InnerException.Message)
                        ex = ex.InnerException
                    Loop
                End If

                Call CoreMessageHandler(message:=sb.ToString, exception:=ex, argument:=foreignkeydefinition.Id, containerID:=foreignkeydefinition.Tablename, _
                                      procedure:="mssqlDBDriver.Getforeignkeys", messagetype:=otCoreMessageType.InternalError)
                Return Nothing

            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=True, procedure:="mssqlDBDriver.GetColumn", argument:=foreignkeydefinition.Id, containerID:=foreignkeydefinition.Tablename, _
                                           message:="Exception", exception:=ex, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End Try
        End Function

       
       
        ''' <summary>
        ''' Runs the SQL Command
        ''' </summary>
        ''' <param name="sqlcmdstr"></param>
        ''' <param name="parameters"></param>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function RunSqlStatement(ByVal sqlcmdstr As String, _
                                                  Optional ByRef parameters As List(Of ormSqlCommandParameter) = Nothing, _
                                                  Optional silent As Boolean = True, Optional nativeConnection As Object = Nothing) As Boolean _
        Implements iormRelationalDatabaseDriver.RunSqlStatement
            Dim anativeConnection As SqlConnection
            '*** get the native Connection 
            If nativeConnection Is Nothing Then
                If Not Me.CurrentConnection Is Nothing Then
                    anativeConnection = DirectCast(Me.CurrentConnection, adonetConnection).NativeInternalConnection
                    If anativeConnection Is Nothing Then
                        Call CoreMessageHandler(procedure:="mssqlDBDriver.runSQLCommand", message:="Native internal Connection not available")
                        Return Nothing
                    End If
                Else
                    Call CoreMessageHandler(procedure:="mssqlDBDriver.runSQLCommand", message:="Connection not available")
                    Return Nothing
                End If
            Else
                anativeConnection = nativeConnection
            End If
            Try
                SyncLock anativeConnection
                    Dim aSQLCommand As New SqlCommand
                    aSQLCommand.Connection = anativeConnection
                    aSQLCommand.CommandText = sqlcmdstr
                    aSQLCommand.CommandType = CommandType.Text
                    aSQLCommand.Prepare()

                    If aSQLCommand.ExecuteNonQuery() > 0 Then

                        Return True
                    Else
                        ''' return false if command return 
                        If sqlcmdstr.ToUpper.Contains("DELETE") OrElse sqlcmdstr.ToUpper.Contains("UPDATE") OrElse sqlcmdstr.ToUpper.Contains("INSERT") Then
                            Return False
                        End If
                        '** return true since it didnot failed
                        Return True
                    End If

                End SyncLock

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="mssqlDBDriver.runSQLCommand", argument:=sqlcmdstr, exception:=ex)
                Return False
            End Try

        End Function
        ''' <summary>
        ''' deletes a db parameter
        ''' </summary>
        ''' <param name="parametername"></param>
        ''' <param name="nativeConnection"></param>
        ''' <param name="silent"></param>
        ''' <param name="setupID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function DeleteDBParameter(parametername As String, _
                                               Optional ByRef nativeConnection As Object = Nothing, _
                                              Optional silent As Boolean = False, _
                                              Optional setupID As String = Nothing) As Boolean Implements iormRelationalDatabaseDriver.DeleteDBParameter


            '*** get the native Connection 
            If nativeConnection Is Nothing Then
                If Not Me.CurrentConnection Is Nothing Then
                    nativeConnection = DirectCast(_primaryConnection, mssqlConnection).NativeInternalConnection
                    If nativeConnection Is Nothing Then
                        If Not silent Then CoreMessageHandler(procedure:="mssqlDBDriver.DeleteDBParameter", _
                                              message:="Native Internal Connection not available")
                        Return False
                    End If
                Else
                    If Not silent Then CoreMessageHandler(procedure:="mssqlDBDriver.DeleteDBParameter", _
                                          message:="Connection not available")
                    Return False
                End If

            End If


            '** init driver
            If Not Me.IsInitialized AndAlso Not Me.Initialize() Then
                If Not silent Then Call CoreMessageHandler(procedure:="mssqlDBDriver.DeleteDBParameter", messagetype:=otCoreMessageType.InternalError, _
                                          message:="couldnot initialize database driver")
                Return False
            End If

            If setupID Is Nothing Then setupID = Me.Session.CurrentSetupID

            Try
                '** on Bootstrapping in the cache
                '** but bootstrapping mode is not sufficnt
                If _BootStrapParameterCache IsNot Nothing AndAlso _ParametersTable Is Nothing Then
                    If _BootStrapParameterCache.ContainsKey(key:=parametername) Then
                        Return _BootStrapParameterCache.Remove(key:=parametername)
                    End If

                    '*** delete from parameters by sql
                    Dim aDeleteText As New Text.StringBuilder
                    aDeleteText.AppendFormat("DELETE FROM [{0}] WHERE [{1}] = '{2}' AND [{3}] = '{4}'", _
                                             Me.DBParameterTablename, Me.ConstFNSetupID, CurrentSetupID, Me.ConstFNID, parametername)

                    If Me.RunSqlStatement(aDeleteText.ToString) Then
                        CoreMessageHandler(message:="IN SETUP >" & CurrentSetupID & "< DROPPED FROM OTDB PARAMETER TABLE  " & CurrentOTDBDriver.DBParameterContainerName, containerID:=CurrentOTDBDriver.DBParameterContainerName, _
                                              messagetype:=otCoreMessageType.ApplicationInfo, procedure:="adonetDBDriver.DropTable")
                    End If
                Else

                    '*** to the table
                    SyncLock _parameterlock

                        ''' strange enough there might be trailing whitechars
                        Dim dataRows = _ParametersTable.AsEnumerable.Where(Function(x) x.Field(Of String)(ConstFNSetupID).Trim.ToUpper = setupID.Trim.ToUpper _
                                                                               AndAlso x.Field(Of String)(ConstFNID).Trim.ToUpper = parametername.Trim.ToUpper)
                        For Each aRow In dataRows
                            aRow.Delete()
                        Next

                        '* save only if not in bootstrapping
                        Dim i = _ParametersTableAdapter.Update(_ParametersTable)
                        If i > 0 Then
                            SyncLock _ParametersTableAdapter.SelectCommand.Connection
                                _ParametersTable.AcceptChanges()
                            End SyncLock
                            Return True
                        Else
                            Return False
                        End If

                    End SyncLock
                End If
            Catch ex As Exception
                ' Handle the error

                If Not silent Then CoreMessageHandler(showmsgbox:=Not silent, procedure:="mssqlDBDriver.DeleteDBParameter", _
                                      exception:=ex, containerID:=_parametersTableName, entryname:=parametername)
                Return False
            End Try


        End Function
        ''' <summary>
        ''' Sets the DB parameter.
        ''' </summary>
        ''' <param name="Parametername">The parametername.</param>
        ''' <param name="Value">The value.</param>
        ''' <param name="NativeConnection">The native connection.</param>
        ''' <param name="UpdateOnly">The update only.</param>
        ''' <param name="silent">The silent.</param>
        ''' <returns></returns>
        Public Overrides Function SetDBParameter(parametername As String, [value] As Object, _
                                                 Optional ByRef nativeConnection As Object = Nothing, _
                                                Optional updateOnly As Boolean = False, _
                                                Optional silent As Boolean = False, _
                                                Optional setupID As String = Nothing, _
                                                Optional description As String = Nothing) As Boolean


            Dim dataRows As IEnumerable(Of DataRow)
            Dim insertFlag As Boolean = False

            '*** get the native Connection 
            If nativeConnection Is Nothing Then
                If Not Me.CurrentConnection Is Nothing Then
                    nativeConnection = DirectCast(_primaryConnection, mssqlConnection).NativeInternalConnection
                    If nativeConnection Is Nothing Then
                        Call CoreMessageHandler(procedure:="mssqlDBDriver.setDBParameter", _
                                              message:="Native Internal Connection not available")
                        Return False
                    End If
                Else
                    Call CoreMessageHandler(procedure:="mssqlDBDriver.setDBParameter", _
                                          message:="Connection not available")
                    Return False
                End If

            End If


            '** init driver
            If Not Me.IsInitialized AndAlso Not Me.Initialize() Then
                Call CoreMessageHandler(procedure:="mssqlDBDriver.setDBParameter", messagetype:=otCoreMessageType.InternalError, _
                                          message:="couldnot initialize database driver")
                Return False
            End If

            If setupID Is Nothing Then setupID = Me.Session.CurrentSetupID

            Try
                '** on Bootstrapping in the cache
                '** but bootstrapping mode is not sufficnt
                If _BootStrapParameterCache IsNot Nothing AndAlso _ParametersTable Is Nothing Then
                    If _BootStrapParameterCache.ContainsKey(key:=parametername) Then
                        _BootStrapParameterCache.Remove(key:=parametername)
                    End If
                    _BootStrapParameterCache.Add(key:=parametername, value:=value)
                    Return True

                Else

                    '*** to the table
                    SyncLock _parameterlock

                        ''' strange enough there might be trailing whitechars
                        dataRows = _ParametersTable.AsEnumerable.Where(Function(x) x.Field(Of String)(ConstFNSetupID).Trim.ToUpper = setupID.Trim.ToUpper _
                                                                               AndAlso x.Field(Of String)(ConstFNID).Trim.ToUpper = parametername.Trim.ToUpper)

                        ' not found
                        If dataRows.Count = 0 Then
                            If updateOnly And silent Then
                                Return False
                            ElseIf updateOnly And Not silent Then
                                Call CoreMessageHandler(showmsgbox:=True, _
                                                      message:="The Parameter '" & parametername & "' was not found in the OTDB Table " & ConstDBParameterTableName, procedure:="mssqlDBDriver.setdbparameter", messagetype:=otCoreMessageType.ApplicationError)
                                Return False
                            ElseIf Not updateOnly Then
                                Dim aFirst As DataRow = _ParametersTable.NewRow
                                dataRows = dataRows.Concat({aFirst})
                                dataRows.First.Item(constFNDescription) = DBNull.Value
                                dataRows.First.Item(ConstFNSetupID) = setupID.Trim
                                dataRows.First.Item(ConstFNID) = parametername.Trim
                                insertFlag = True
                            End If
                        End If

                        ' value
                        'dataRows(0).BeginEdit()

                        If String.IsNullOrEmpty([value]) Then
                            dataRows.First.Item(ConstFNValue) = DBNull.Value
                        Else
                            dataRows.First.Item(ConstFNValue) = [value].ToString.Trim
                        End If
                        dataRows.First.Item(ConstFNChangedOn) = DateTime.Now
                        If String.IsNullOrEmpty(description) Then
                            dataRows.First.Item(constFNDescription) = DBNull.Value
                        Else
                            dataRows.First.Item(constFNDescription) = description.Trim
                        End If

                        'dataRows(0).EndEdit()

                        '* add to table
                        If insertFlag Then
                            _ParametersTable.Rows.Add(dataRows(0))
                        End If

                        '* save only if not in bootstrapping
                        Dim i = _ParametersTableAdapter.Update(_ParametersTable)
                        If i > 0 Then
                            SyncLock _ParametersTableAdapter.SelectCommand.Connection
                                _ParametersTable.AcceptChanges()
                            End SyncLock
                            Return True
                        Else
                            Return False
                        End If

                    End SyncLock
                End If
            Catch ex As Exception
                ' Handle the error

                Call CoreMessageHandler(showmsgbox:=Not silent, procedure:="mssqlDBDriver.setDBParameter", _
                                      exception:=ex, containerID:=_parametersTableName, entryname:=parametername)
                Return False
            End Try


        End Function

        ''' <summary>
        ''' Gets the DB parameter.
        ''' </summary>
        ''' <param name="PARAMETERNAME">The PARAMETERNAME.</param>
        ''' <param name="nativeConnection">The native connection.</param>
        ''' <param name="silent">The silent.</param>
        ''' <returns></returns>
        Public Overrides Function GetDBParameter(parametername As String, _
                                                 Optional ByRef nativeConnection As Object = Nothing, _
                                                 Optional silent As Boolean = False, _
                                                 Optional SetupID As String = Nothing) As Object
            Dim dataRows As IEnumerable(Of DataRow)

            '*** get the native Connection 
            If nativeConnection Is Nothing Then
                If Not Me.CurrentConnection Is Nothing Then
                    nativeConnection = _primaryConnection.NativeInternalConnection
                    If nativeConnection Is Nothing Then
                        Call CoreMessageHandler(procedure:="mssqlDBDriver.getDBParameter", message:="Native internal Connection not available")
                        Return Nothing
                    End If
                Else
                    Call CoreMessageHandler(procedure:="mssqlDBDriver.getDBParameter", message:="Connection not available")
                    Return Nothing
                End If
            End If


            Try
                '** init driver
                If Not Me.IsInitialized AndAlso Not Initialize() Then
                    Call CoreMessageHandler(procedure:="mssqlDBDriver.getDBParameter", containerID:=ConstDBParameterTableName, _
                                       message:="couldnot initialize database driver", argument:=Me.ID, entryname:=parametername)
                    Return Nothing
                End If
                If SetupID Is Nothing Then SetupID = Me.Session.CurrentSetupID
                '** on Bootstrapping out of the cache
                '** but bootstrapping mode is not sufficnt
                If _BootStrapParameterCache IsNot Nothing AndAlso _ParametersTable Is Nothing Then
                    If _BootStrapParameterCache.ContainsKey(key:=parametername) Then
                        Return _BootStrapParameterCache.Item(key:=parametername)
                    Else
                        Return Nothing
                    End If
                Else
                    SyncLock _parameterlock

                        ''' strange enough there might be trailing whitechars
                        dataRows = _ParametersTable.AsEnumerable.Where(Function(x) x.Field(Of String)(ConstFNSetupID).Trim.ToUpper = SetupID.Trim.ToUpper _
                                                                        AndAlso x.Field(Of String)(ConstFNID).Trim.ToUpper = parametername.Trim.ToUpper)

                        ' not found
                        If dataRows.Count = 0 Then
                            If silent Then
                                Return Nothing
                            ElseIf Not silent Then
                                Call CoreMessageHandler(showmsgbox:=True, _
                                                      message:="The Parameter '" & parametername & "' was not found in the OTDB Table " & ConstDBParameterTableName, procedure:="mssqlDBDriver.setdbparameter", messagetype:=otCoreMessageType.ApplicationError)
                                Return Nothing

                            End If
                        End If

                        ' value
                        Return dataRows.First.Item(ConstFNValue)

                    End SyncLock
                End If
                ' Handle the error
            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=silent, procedure:="mssqlDBDriver.getDBParameter", containerID:=ConstDBParameterTableName, _
                                      exception:=ex, entryname:=parametername)
                Return Nothing
            End Try

        End Function


        ''' <summary>
        ''' EventHandler for onConnect
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Protected Friend Sub Connection_onConnection(sender As Object, e As ormConnectionEventArgs) Handles _primaryConnection.OnConnection
            Call Me.Initialize()
        End Sub

        ''' <summary>
        ''' EventHandler for onDisConnect
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Protected Friend Sub Connection_onDisConnection(sender As Object, e As ormConnectionEventArgs) Handles _primaryConnection.OnDisconnection
            Call Me.Reset()
        End Sub

        ''' <summary>
        ''' returns a new visitor object for building expression trees
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GetIXPTVisitor() As rulez.eXPressionTree.IVisitor Implements iormDatabaseDriver.GetIXPTVisitor
            Return Me.GetIRDBVisitor()
        End Function

        ''' <summary>
        ''' returns a new visitor object for building expression trees
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GetIRDBVisitor() As IRDBVisitor Implements iormRelationalDatabaseDriver.GetIRDBVisitor
            Return New mssqlXPTVisitor()
        End Function
    End Class


    ''' <summary>
    ''' SQL Server OnTrack Database Connection Class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class mssqlConnection
        Inherits adonetConnection
        Implements iormConnection

        'Protected Friend Shadows _nativeConnection As SqlConnection
        'Protected Friend Shadows _nativeinternalConnection As SqlConnection

        '** SMO Objects
        Protected _SMOConnection As Microsoft.SqlServer.Management.Common.ServerConnection
        Protected _Server As Microsoft.SqlServer.Management.Smo.Server
        Protected _Database As Microsoft.SqlServer.Management.Smo.Database

        Public Shadows Event OnConnection As EventHandler(Of ormConnectionEventArgs) Implements iormConnection.OnConnection
        Public Shadows Event OnDisconnection As EventHandler(Of ormConnectionEventArgs) Implements iormConnection.OnDisconnection

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="databaseDriver"></param>
        ''' <param name="session"></param>
        ''' <param name="sequence"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal id As String, ByRef databaseDriver As iormRelationalDatabaseDriver, sequence As ComplexPropertyStore.Sequence, Optional ByRef session As Session = Nothing)
            MyBase.New(id, databaseDriver, sequence, session:=session)

        End Sub

        ''' <summary>
        ''' returns the native Database name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property NativeDatabaseName As String Implements iormConnection.NativeDatabaseName
            Get
                If _Server IsNot Nothing Then
                    Return _Server.Product.ToString
                End If
                Return Nothing
            End Get
        End Property
        ''' <summary>
        ''' returns the native Database version
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property NativeDatabaseVersion As String Implements iormConnection.NativeDatabaseVersion
            Get
                If _Server IsNot Nothing Then
                    Return _Server.VersionString
                End If
                Return Nothing
            End Get
        End Property
        ''' <summary>
        ''' Gets the SMO connection.
        ''' </summary>
        ''' <value>The SMO connection.</value>
        Public ReadOnly Property SMOConnection() As Microsoft.SqlServer.Management.Common.ServerConnection
            Get
                Return Me._SMOConnection
            End Get
        End Property
        ''' <summary>
        ''' Gets the server.
        ''' </summary>
        ''' <value>The server.</value>
        Public ReadOnly Property Server() As Microsoft.SqlServer.Management.Smo.Server
            Get
                Return Me._Server
            End Get
        End Property
        ''' <summary>
        ''' Gets the database.
        ''' </summary>
        ''' <value>The database.</value>
        Public ReadOnly Property Database() As Microsoft.SqlServer.Management.Smo.Database
            Get
                Return Me._Database
            End Get
        End Property
        ''' <summary>
        ''' Returns True if we have sqlserver permission to receive notificcations
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CanRequestNotifications() As Boolean
            Try
                Dim perm As SqlClientPermission = New SqlClientPermission(Security.Permissions.PermissionState.Unrestricted)
                perm.Demand()
                Return True
            Catch ex As Exception
                Return False
            End Try

        End Function
        ''' <summary>
        ''' create a smo server connection and returns it. Sets also the scripting optimization and the default fields to load
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function CreateSMOConnection(connection As IDbConnection) As ServerConnection
            Dim aSMOConnection As ServerConnection
            aSMOConnection = New Microsoft.SqlServer.Management.Common.ServerConnection()
            aSMOConnection.ServerInstance = DirectCast(connection, SqlConnection).DataSource
            aSMOConnection.SqlExecutionModes = SqlExecutionModes.ExecuteSql
            aSMOConnection.AutoDisconnectMode = AutoDisconnectMode.NoAutoDisconnect

            If Not aSMOConnection Is Nothing Then
                _Server = New Server(aSMOConnection)
                _Server.ConnectionContext.LoginSecure = False
                _Server.ConnectionContext.Login = Me._Dbuser
                _Server.ConnectionContext.Password = Me._Dbpassword
                _Server.Refresh()
                ' get the database
                If _Server.Databases.Contains(DirectCast(_nativeinternalConnection, SqlConnection).Database) Then
                    _Database = _Server.Databases(DirectCast(_nativeinternalConnection, SqlConnection).Database)
                Else
                    Call CoreMessageHandler(showmsgbox:=True, message:="Database " & Me.DBName & " is not existing on server " & _Server.Name, _
                                           messagetype:=otCoreMessageType.InternalError, noOtdbAvailable:=True, procedure:="mssqlConnection.CreateSMOConnection")
                    _Database = Nothing

                End If
                '** set what to load
                Dim scriptingOptions As ScriptingOptions = New ScriptingOptions()
                scriptingOptions.ExtendedProperties = True
                scriptingOptions.Indexes = True
                scriptingOptions.DriAllKeys = True
                scriptingOptions.DriForeignKeys = True

                _Database.PrefetchObjects(GetType(Table), scriptingOptions)

                _Server.SetDefaultInitFields(GetType(Table), {"CreateDate"})
                _Server.SetDefaultInitFields(GetType(Index), {"IndexKeyType"})
                _Server.SetDefaultInitFields(GetType(Column), {"Nullable", "ID", "Default", "DataType"})

                Return aSMOConnection
            Else
                Call CoreMessageHandler(message:="SMO Object for Database " & Me.DBName & " is not existing for server " & _Server.Name, break:=False, _
                                           messagetype:=otCoreMessageType.InternalError, noOtdbAvailable:=True, procedure:="mssqlConnection.CreateSMOConnection")
                Return Nothing
            End If
        End Function

        Private Sub mssqlConnection_OnDisconnection(sender As Object, e As ormConnectionEventArgs) Handles Me.OnDisconnection

        End Sub
        ''' <summary>
        ''' Event Handler onInternalConnection
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="arguments"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Sub OnInternalConnection(sender As Object, arguments As InternalConnectionEventArgs) Handles MyBase.OnInternalConnected
            If _SMOConnection Is Nothing Then
                _SMOConnection = CreateSMOConnection(_nativeinternalConnection)
            End If
            If _SMOConnection Is Nothing Then
                Call CoreMessageHandler(message:="SMO Object for Database " & Me.DBName & " is not existing for server " & _Server.Name, break:=False, _
                                           messagetype:=otCoreMessageType.InternalError, noOtdbAvailable:=True, procedure:="mssqlConnection.OnInternalConnection")
            End If
        End Sub
        ''' <summary>
        ''' Raise the OnConnected Event
        ''' </summary>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shadows Sub MssqlConnection_OnConnected(sender As Object, e As ormConnectionEventArgs) Handles MyBase.OnConnection
            RaiseEvent OnConnection(sender, e)
        End Sub
        Public Shadows Sub MssqlConnection_RaiseOnDisConnected(sender As Object, e As ormConnectionEventArgs) Handles MyBase.OnDisconnection
            RaiseEvent OnDisconnection(sender, e)
            _Server = Nothing
            _Database = Nothing
            _SMOConnection.ForceDisconnected()
            _SMOConnection = Nothing
        End Sub

        ''' <summary>
        ''' Gets or sets the connection.
        ''' </summary>
        ''' <value>The connection.</value>
        Public Property SqlConnection() As SqlConnection
            Get
                If _nativeConnection Is Nothing AndAlso _nativeConnection.State <> ConnectionState.Closed Then
                    Return Nothing
                Else
                    Return DirectCast(Me.NativeConnection, SqlConnection)
                End If

            End Get
            Protected Friend Set(value As SqlConnection)
                Me._nativeConnection = value
            End Set
        End Property


        ''' <summary>
        ''' create a new SQLConnection
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function CreateNewNativeConnection() As IDbConnection

            Return New SqlConnection()
        End Function


    End Class

    ''' <summary>
    ''' CLASS describes the schema per table of the database itself
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class mssqlAbstractSchema
        Inherits adonetTableSchema
        Implements iormContainerSchema


        ''' <summary>
        ''' Initializes a new instance of the <see cref="mssqlTableSchema" /> class.
        ''' </summary>
        ''' <param name="connection">The connection.</param>
        ''' <param name="tableID">The table ID.</param>
        Public Sub New(ByRef connection As iormConnection, tableID As String)
            MyBase.New(connection, tableID)

        End Sub


        '***** internal variables
        '*****



    End Class
    ''' <summary>
    ''' CLASS describes the schema per table of the database itself
    ''' </summary>
    ''' <remarks></remarks>
    Public Class mssqlTableSchema
        Inherits adonetTableSchema
        Implements iormRelationalSchema

        ''' <summary>
        ''' Initializes a new instance of the <see cref="mssqlTableSchema" /> class.
        ''' </summary>
        ''' <param name="connection">The connection.</param>
        ''' <param name="tableID">The table ID.</param>
        Public Sub New(ByRef connection As iormConnection, tableID As String)
            MyBase.New(connection, tableID)
        End Sub

        ''' <summary>
        ''' returns a native DBParameter from the Driver
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function CreateNativeDBParameter() As IDbDataParameter
            Return New SqlParameter()
        End Function
        ''' <summary>
        ''' returns a native DBCommonad
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function CreateNativeDBCommand() As IDbCommand
            Return New SqlCommand()
        End Function
        ''' <summary>
        ''' returns true if the type is used by the driver
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function IsNativeDBTypeOfVar(type As Object) As Boolean
            Dim datatype As SqlDataType = type

            If datatype = SqlDataType.NVarChar Or datatype = SqlDataType.NText Or datatype = SqlDataType.VarChar _
             Or datatype = SqlDataType.VarChar Or datatype = SqlDataType.Binary Or datatype = SqlDataType.Variant _
             Or datatype = SqlDataType.NVarCharMax Or datatype = SqlDataType.VarCharMax Or datatype = SqlDataType.NChar _
             Or datatype = SqlDataType.VarBinary Or datatype = SqlDataType.Text Then
                Return True
            Else
                Return False
            End If
        End Function
        ''' <summary>
        ''' Create aDBParameter
        ''' </summary>
        ''' <param name="columnname">name of the Column</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function AssignNativeDBParameter(columnname As String, _
                                                          Optional parametername As String = Nothing) As System.Data.IDbDataParameter Implements iormRelationalSchema.AssignNativeDBParameter
            Dim aDBColumnDescription As adonetColumnDescription = GetColumnDescription(Me.GetEntryOrdinal(columnname))
            Dim aParameter As SqlParameter

            If Not aDBColumnDescription Is Nothing Then

                aParameter = CreateNativeDBParameter()

                If String.IsNullOrWhiteSpace(parametername) Then
                    aParameter.ParameterName = "@" & columnname
                Else
                    If parametername.First = "@" Then
                        aParameter.ParameterName = parametername
                    Else
                        aParameter.ParameterName = "@" & parametername
                    End If
                End If
                'aParameter.SqlDbType = aDBColumnDescription.DataType
                Select Case aDBColumnDescription.DataType
                    Case SqlDataType.BigInt
                        aParameter.SqlDbType = SqlDbType.BigInt
                    Case SqlDataType.SmallInt
                        aParameter.SqlDbType = SqlDbType.SmallInt
                    Case SqlDataType.Int
                        aParameter.SqlDbType = SqlDbType.Int
                    Case SqlDataType.NVarChar, SqlDataType.NVarCharMax, SqlDataType.NChar, SqlDataType.NText, SqlDataType.VarChar, SqlDataType.VarCharMax, SqlDataType.Text
                        aParameter.SqlDbType = SqlDbType.NVarChar
                    Case SqlDataType.Bit
                        aParameter.SqlDbType = SqlDbType.Bit
                    Case SqlDataType.Numeric, SqlDataType.Real, SqlDataType.Float
                        aParameter.SqlDbType = SqlDbType.Float
                    Case SqlDataType.Money
                        aParameter.SqlDbType = SqlDbType.Money
                    Case SqlDataType.SmallMoney
                        aParameter.SqlDbType = SqlDbType.SmallMoney
                    Case SqlDataType.DateTime
                        aParameter.SqlDbType = SqlDbType.DateTime
                    Case SqlDataType.DateTime2
                        aParameter.SqlDbType = SqlDbType.DateTime2
                    Case SqlDataType.Date
                        aParameter.SqlDbType = SqlDbType.Date
                    Case SqlDataType.SmallDateTime
                        aParameter.SqlDbType = SqlDbType.SmallDateTime
                    Case SqlDataType.Time
                        aParameter.SqlDbType = SqlDbType.Time
                        aParameter.Size = 7
                    Case Else
                        Call CoreMessageHandler(procedure:="mssqlTableSchema.AssignNativeDBParameter", break:=False, message:="SqlDatatype not handled", _
                                               messagetype:=otCoreMessageType.ApplicationError)
                        aParameter.SqlDbType = SqlDbType.Variant
                End Select

                aParameter.SourceColumn = columnname

                '** set the length
                If IsNativeDBTypeOfVar(aDBColumnDescription.DataType) Then
                    If aDBColumnDescription.CharacterMaxLength = 0 Then
                        aParameter.Size = constDBDriverMaxMemoSize
                    Else
                        aParameter.Size = aDBColumnDescription.CharacterMaxLength
                    End If

                Else
                    If aDBColumnDescription.CharacterMaxLength <> 0 Then
                        ' aParameter.Size = aDBColumnDescription.CharacterMaxLength
                    End If
                    ' aParameter.Size = 0
                End If
                Return aParameter
            Else
                Call CoreMessageHandler(procedure:="mssqlTableSchema.buildParameter", message:="ColumnDescription couldn't be loaded", _
                                                     argument:=columnname, containerID:=Me.TableID, messagetype:=otCoreMessageType.InternalError)
                Return Nothing

            End If
        End Function

        ''' <summary>
        ''' Fills the schema for table.
        ''' </summary>
        ''' <param name="aTableName">Name of a table.</param>
        ''' <param name="reloadForce">The reload force.</param>
        ''' <returns></returns>
        Public Overrides Function Refresh(Optional reloadForce As Boolean = False) As Boolean
            Dim no As UShort
            Dim index As Integer
            Dim aColumnCollection As ArrayList
            Dim myConnection As mssqlConnection = DirectCast(Me._Connection, mssqlConnection)
            Dim aCon As SqlConnection = DirectCast(myConnection.NativeInternalConnection, SqlConnection)


            ' not working 
            If myConnection.Database Is Nothing OrElse Not myConnection.SMOConnection.IsOpen Then
                Call CoreMessageHandler(procedure:="mssqlTableSchema.refresh", _
                                     message:="SMO Connection is not open", _
                                     containerID:=TableID, messagetype:=otCoreMessageType.InternalError)
                _IsInitialized = False
                Return False
            End If
            ' return if no TableID
            If String.IsNullOrWhiteSpace(Me.TableID) Then
                Call CoreMessageHandler(procedure:="mssqlTableSchema.refresh", _
                                      message:="Nothing Tablename to set to", _
                                      containerID:=TableID, messagetype:=otCoreMessageType.InternalError)
                _IsInitialized = False
                Return False
            End If
            '

            Refresh = True

            'Diagnostics.Debug.WriteLine("refreshing " & Me.TableID & "->" & Me.NativeTablename)

            Try

                SyncLock DirectCast(myConnection, adonetConnection).NativeInternalConnection

                    myConnection.IsNativeInternalLocked = True
                    Dim aTable As Table = New Table(myConnection.Database, name:=NativeTablename)
                    If aTable Is Nothing Then
                        Call CoreMessageHandler(procedure:="mssqlTableSchema.refresh", message:="Table could not be loaded from SMO", _
                                         containerID:=TableID, argument:=Me.NativeTablename, messagetype:=otCoreMessageType.InternalError)
                        myConnection.IsNativeInternalLocked = False
                        _IsInitialized = False
                        Return False
                    End If



                    '** reload the Table
                    '**
                    aTable.Refresh()
                    myConnection.IsNativeInternalLocked = False
                    If False Then
                        Call CoreMessageHandler(procedure:="mssqlTableSchema.refresh", message:="Table couldnot initialized from SMO", _
                                         containerID:=TableID, messagetype:=otCoreMessageType.InternalError)
                        _IsInitialized = False
                        Return False
                    End If

                    no = aTable.Columns.Count
                    If no = 0 Then
                        Call CoreMessageHandler(procedure:="mssqlTableSchema.refresh", _
                                                        message:="Table couldnot initialized from SMO - does it exist ????", _
                                                        containerID:=TableID, messagetype:=otCoreMessageType.InternalError)
                        _IsInitialized = False
                        Return False
                    Else
                        ReDim _entrynames(no - 1)
                        ReDim _Columns(no - 1)
                    End If

                    ' set the Dictionaries if reload
                    _fieldsDictionary = New Dictionary(Of String, Long)
                    _indexDictionary = New Dictionary(Of String, ArrayList)
                    aColumnCollection = New ArrayList
                    _NoPrimaryKeys = 0
                    Dim i As UShort = 0

                    '*
                    myConnection.IsNativeInternalLocked = True

                    '* each column
                    For Each aColumn As Column In aTable.Columns

                        '*
                        _entrynames(i) = aColumn.Name.Clone
                        '* set the description
                        _Columns(i) = New adonetColumnDescription
                        With _Columns(i)
                            .ColumnName = aColumn.Name.ToUpper

                            '* time penalty to heavy for refreshing
                            ' If Not aColumn.ExtendedProperties.Contains("MS_Description") Then aColumn.ExtendedProperties.Refresh()
                            'If aColumn.ExtendedProperties.Contains("MS_Description") Then
                            '.Description = aColumn.ExtendedProperties("MS_Description").Value
                            'Else
                            .Description = String.Empty
                            'End If
                            If aColumn.Default <> String.Empty Then
                                .HasDefault = True
                            Else
                                .HasDefault = False
                            End If
                            'If aColumn.DataType.MaximumLength Is Nothing Then
                            .CharacterMaxLength = aColumn.DataType.MaximumLength
                            'End If
                            .IsNullable = aColumn.Nullable
                            .DataType = aColumn.DataType.SqlDataType
                            .Ordinal = aColumn.ID
                            .Default = aColumn.Default.Clone
                            .Catalog = aColumn.DefaultSchema.Clone
                            '.DateTimePrecision = aColumn.DataType.DateTimePrecision
                            .NumericPrecision = aColumn.DataType.NumericPrecision
                            .NumericScale = aColumn.DataType.NumericScale
                            .CharachterOctetLength = aColumn.DataType.MaximumLength
                        End With
                        ' remove if existing
                        If _fieldsDictionary.ContainsKey(_entrynames(i)) Then
                            _fieldsDictionary.Remove(_entrynames(i))
                        End If
                        ' add
                        _fieldsDictionary.Add(key:=_entrynames(i), value:=i + 1) 'store no field 1... not the array index

                        '* inc
                        i += 1
                    Next

                    '** Crossreference the Indices
                    For Each anIndex As Index In aTable.Indexes
                        'anIndex.Refresh()

                        ' new
                        aColumnCollection = New ArrayList

                        For Each aColumn In anIndex.IndexedColumns

                            ' indx no
                            index = _fieldsDictionary.Item(aColumn.name.toupper)
                            '
                            '** check if primaryKey
                            'fill old primary Key structure
                            If anIndex.IndexKeyType = IndexKeyType.DriPrimaryKey Then
                                _PrimaryKeyIndexName = anIndex.Name.ToUpper
                                _NoPrimaryKeys = _NoPrimaryKeys + 1
                                ReDim Preserve _Primarykeys(0 To _NoPrimaryKeys - 1)
                                _Primarykeys(_NoPrimaryKeys - 1) = index - 1 ' set to the array 0...ubound
                            End If

                            aColumnCollection.Add(aColumn.name.toupper)

                        Next

                        '** store final

                        If _indexDictionary.ContainsKey(anIndex.Name.ToUpper) Then
                            _indexDictionary.Remove(key:=anIndex.Name.ToUpper)
                        End If
                        _indexDictionary.Add(key:=anIndex.Name.ToUpper, value:=aColumnCollection)
                    Next

                    myConnection.IsNativeInternalLocked = False

                    '**** read each Index
                    '****
                    Dim anIndexName As String = String.Empty

                    '**** build the commands
                    '****
                    Dim enumValues As Array = System.[Enum].GetValues(GetType(CommandType))
                    For Each anIndexName In _indexDictionary.Keys
                        Dim aNewCommand As SqlCommand
                        For Each aCommandType In enumValues
                            Dim aNewKey = New CommandKey(anIndexName, aCommandType)
                            aNewCommand = BuildCommand(anIndexName, aCommandType)
                            If Not aNewCommand Is Nothing Then
                                If _CommandStore.ContainsKey(aNewKey) Then
                                    _CommandStore.Remove(aNewKey)
                                End If
                                _CommandStore.Add(aNewKey, aNewCommand)
                            End If
                        Next


                    Next

                    _IsInitialized = True

                End SyncLock



                Return True

            Catch ex As Exception
                myConnection.IsNativeInternalLocked = False
                Call CoreMessageHandler(showmsgbox:=False, procedure:="mssqlTableSchema.refresh", containerID:=Me.TableID, _
                                      argument:=reloadForce, exception:=ex)

                _IsInitialized = False
                Return False
            End Try

        End Function

    End Class

    ''' <summary>
    ''' CLASS describes the schema per table of the database itself
    ''' </summary>
    ''' <remarks></remarks>
    Public Class mssqlViewSchema
        Inherits adonetViewSchema
        Implements iormRelationalSchema

        ''' <summary>
        ''' Initializes a new instance of the <see cref="mssqlTableSchema" /> class.
        ''' </summary>
        ''' <param name="connection">The connection.</param>
        ''' <param name="tableID">The table ID.</param>
        Public Sub New(ByRef connection As iormConnection, viewID As String)
            MyBase.New(connection, viewID)
        End Sub

        ''' <summary>
        ''' returns a native DBParameter from the Driver
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function CreateNativeDBParameter() As IDbDataParameter
            Return New SqlParameter()
        End Function
        ''' <summary>
        ''' returns a native DBCommonad
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function CreateNativeDBCommand() As IDbCommand
            Return New SqlCommand()
        End Function
        ''' <summary>
        ''' returns true if the type is used by the driver
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function IsNativeDBTypeOfVar(type As Object) As Boolean
            Dim datatype As SqlDataType = type

            If datatype = SqlDataType.NVarChar Or datatype = SqlDataType.NText Or datatype = SqlDataType.VarChar _
             Or datatype = SqlDataType.VarChar Or datatype = SqlDataType.Binary Or datatype = SqlDataType.Variant _
             Or datatype = SqlDataType.NVarCharMax Or datatype = SqlDataType.VarCharMax Or datatype = SqlDataType.NChar _
             Or datatype = SqlDataType.VarBinary Or datatype = SqlDataType.Text Then
                Return True
            Else
                Return False
            End If
        End Function
        ''' <summary>
        ''' Create aDBParameter
        ''' </summary>
        ''' <param name="columnname">name of the Column</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function AssignNativeDBParameter(ColumnName As String, _
                                                          Optional parametername As String = Nothing) As System.Data.IDbDataParameter Implements iormRelationalSchema.AssignNativeDBParameter
            Dim aDBColumnDescription As adonetColumnDescription = GetColumnDescription(Me.GetEntryOrdinal(ColumnName))
            Dim aParameter As SqlParameter

            If Not aDBColumnDescription Is Nothing Then

                aParameter = CreateNativeDBParameter()

                If String.IsNullOrWhiteSpace(parametername) Then
                    aParameter.ParameterName = "@" & ColumnName
                Else
                    If parametername.First = "@" Then
                        aParameter.ParameterName = parametername
                    Else
                        aParameter.ParameterName = "@" & parametername
                    End If
                End If
                'aParameter.SqlDbType = aDBColumnDescription.DataType
                Select Case aDBColumnDescription.DataType
                    Case SqlDataType.BigInt
                        aParameter.SqlDbType = SqlDbType.BigInt
                    Case SqlDataType.SmallInt
                        aParameter.SqlDbType = SqlDbType.SmallInt
                    Case SqlDataType.Int
                        aParameter.SqlDbType = SqlDbType.Int
                    Case SqlDataType.NVarChar, SqlDataType.NVarCharMax, SqlDataType.NChar, SqlDataType.NText, SqlDataType.VarChar, SqlDataType.VarCharMax, SqlDataType.Text
                        aParameter.SqlDbType = SqlDbType.NVarChar
                    Case SqlDataType.Bit
                        aParameter.SqlDbType = SqlDbType.Bit
                    Case SqlDataType.Numeric, SqlDataType.Real, SqlDataType.Float
                        aParameter.SqlDbType = SqlDbType.Float
                    Case SqlDataType.Money
                        aParameter.SqlDbType = SqlDbType.Money
                    Case SqlDataType.SmallMoney
                        aParameter.SqlDbType = SqlDbType.SmallMoney
                    Case SqlDataType.DateTime
                        aParameter.SqlDbType = SqlDbType.DateTime
                    Case SqlDataType.DateTime2
                        aParameter.SqlDbType = SqlDbType.DateTime2
                    Case SqlDataType.Date
                        aParameter.SqlDbType = SqlDbType.Date
                    Case SqlDataType.SmallDateTime
                        aParameter.SqlDbType = SqlDbType.SmallDateTime
                    Case SqlDataType.Time
                        aParameter.SqlDbType = SqlDbType.Time
                        aParameter.Size = 7
                    Case Else
                        Call CoreMessageHandler(procedure:="mssqlViewSchema.AssignNativeDBParameter", break:=False, message:="SqlDatatype not handled", _
                                               messagetype:=otCoreMessageType.ApplicationError)
                        aParameter.SqlDbType = SqlDbType.Variant
                End Select

                aParameter.SourceColumn = ColumnName

                '** set the length
                If IsNativeDBTypeOfVar(aDBColumnDescription.DataType) Then
                    If aDBColumnDescription.CharacterMaxLength = 0 Then
                        aParameter.Size = constDBDriverMaxMemoSize
                    Else
                        aParameter.Size = aDBColumnDescription.CharacterMaxLength
                    End If

                Else
                    If aDBColumnDescription.CharacterMaxLength <> 0 Then
                        ' aParameter.Size = aDBColumnDescription.CharacterMaxLength
                    End If
                    ' aParameter.Size = 0
                End If
                Return aParameter
            Else
                Call CoreMessageHandler(procedure:="mssqlViewSchema.buildParameter", message:="ColumnDescription couldn't be loaded", _
                                                     argument:=ColumnName, containerID:=Me.ViewID, messagetype:=otCoreMessageType.InternalError)
                Return Nothing

            End If
        End Function
        ''' <summary>
        ''' Fills the schema for view.
        ''' </summary>
        ''' <param name="reloadForce">The reload force.</param>
        ''' <returns></returns>
        Public Overrides Function Refresh(Optional reloadForce As Boolean = False) As Boolean
            Dim no As UShort
            Dim index As Integer
            Dim aColumnCollection As ArrayList
            Dim myConnection As mssqlConnection = DirectCast(Me._Connection, mssqlConnection)
            Dim aCon As SqlConnection = DirectCast(myConnection.NativeInternalConnection, SqlConnection)


            ' not working 
            If myConnection.Database Is Nothing OrElse Not myConnection.SMOConnection.IsOpen Then
                Call CoreMessageHandler(procedure:="mssqlViewSchema.refresh", _
                                     message:="SMO Connection is not open", _
                                     containerID:=Me.ViewID, messagetype:=otCoreMessageType.InternalError)
                _IsInitialized = False
                Return False
            End If
            ' return if no viewid
            If String.IsNullOrWhiteSpace(Me.ViewID) Then
                Call CoreMessageHandler(procedure:="mssqlViewSchema.refresh", _
                                      message:="No viewname to set to", _
                                      containerID:=Me.ViewID, messagetype:=otCoreMessageType.InternalError)
                _IsInitialized = False
                Return False
            End If
            '

            Refresh = True

            'Diagnostics.Debug.WriteLine("refreshing " & Me.TableID & "->" & Me.NativeTablename)

            Try

                SyncLock DirectCast(myConnection, adonetConnection).NativeInternalConnection

                    myConnection.IsNativeInternalLocked = True
                    Dim aView As View = New View(myConnection.Database, name:=NativeViewname)
                    If aView Is Nothing Then
                        Call CoreMessageHandler(procedure:="mssqlViewSchema.refresh", message:="Table could not be loaded from SMO", _
                                         containerID:=Me.ViewID, argument:=Me.NativeViewname, messagetype:=otCoreMessageType.InternalError)
                        myConnection.IsNativeInternalLocked = False
                        _IsInitialized = False
                        Return False
                    End If



                    '** reload the Table
                    '**
                    aView.Refresh()
                    myConnection.IsNativeInternalLocked = False
                    If False Then
                        Call CoreMessageHandler(procedure:="mssqlViewSchema.refresh", message:="Table couldnot initialized from SMO", _
                                         containerID:=Me.ViewID, messagetype:=otCoreMessageType.InternalError)
                        _IsInitialized = False
                        Return False
                    End If

                    no = aView.Columns.Count
                    If no = 0 Then
                        Call CoreMessageHandler(procedure:="mssqlViewSchema.refresh", _
                                                        message:="Table couldnot initialized from SMO - does it exist ????", _
                                                        containerID:=Me.ViewID, messagetype:=otCoreMessageType.InternalError)
                        _IsInitialized = False
                        Return False
                    Else
                        ReDim _entrynames(no - 1)
                        ReDim _Columns(no - 1)
                    End If

                    ' set the Dictionaries if reload
                    _fieldsDictionary = New Dictionary(Of String, Long)
                    _indexDictionary = New Dictionary(Of String, ArrayList)
                    aColumnCollection = New ArrayList
                    '_NoPrimaryKeys = 0
                    Dim i As UShort = 0

                    '*
                    myConnection.IsNativeInternalLocked = True

                    '* each column
                    For Each aColumn As Column In aView.Columns

                        '*
                        _entrynames(i) = aColumn.Name.Clone
                        '* set the description
                        _Columns(i) = New adonetColumnDescription
                        With _Columns(i)
                            .ColumnName = aColumn.Name.ToUpper

                            '* time penalty to heavy for refreshing
                            ' If Not aColumn.ExtendedProperties.Contains("MS_Description") Then aColumn.ExtendedProperties.Refresh()
                            'If aColumn.ExtendedProperties.Contains("MS_Description") Then
                            '.Description = aColumn.ExtendedProperties("MS_Description").Value
                            'Else
                            .Description = String.Empty
                            'End If
                            If aColumn.Default <> String.Empty Then
                                .HasDefault = True
                            Else
                                .HasDefault = False
                            End If
                            'If aColumn.DataType.MaximumLength Is Nothing Then
                            .CharacterMaxLength = aColumn.DataType.MaximumLength
                            'End If
                            .IsNullable = aColumn.Nullable
                            .DataType = aColumn.DataType.SqlDataType
                            .Ordinal = aColumn.ID
                            .Default = aColumn.Default.Clone
                            .Catalog = aColumn.DefaultSchema.Clone
                            '.DateTimePrecision = aColumn.DataType.DateTimePrecision
                            .NumericPrecision = aColumn.DataType.NumericPrecision
                            .NumericScale = aColumn.DataType.NumericScale
                            .CharachterOctetLength = aColumn.DataType.MaximumLength
                        End With
                        ' remove if existing
                        If _fieldsDictionary.ContainsKey(_entrynames(i)) Then
                            _fieldsDictionary.Remove(_entrynames(i))
                        End If
                        ' add
                        _fieldsDictionary.Add(key:=_entrynames(i), value:=i + 1) 'store no field 1... not the array index

                        '* inc
                        i += 1
                    Next

                    ''' TODO: get all the tables which are based on this view
                    ''' 
                    'UrnCollection col = new UrnCollection(); 
                    'foreach (Table table in database.Tables) {
                    '    if(table.Name == "Employees")
                    '       col.Add(table.Urn); 
                    '}
                    'DependencyTree tree = sp.DiscoverDependencies(col, DependencyType.Children); 
                    'DependencyWalker walker = new DependencyWalker(server); 
                    'DependencyCollection depends = walker.WalkDependencies(tree); 
                    '//Iterate over each table in DB in dependent order... 
                    'foreach (DependencyCollectionNode dcn in depends)
                    Dim urncol As New UrnCollection
                    urncol.Add(aView.Urn)
                    Dim aWalker As New DependencyWalker(server:=myConnection.Database.Parent)
                    Dim aDepTree As DependencyTree = aWalker.DiscoverDependencies(urncol, True)
                    Dim aDepColl As DependencyCollection = aWalker.WalkDependencies(aDepTree)

                    For Each aNode As DependencyCollectionNode In aDepColl
                        Dim anObject As SqlSmoObject = myConnection.Database.Parent.GetSmoObject(aNode.Urn)
                        'If anObject.Properties.
                    Next

                    '** Crossreference the Indices
                    For Each anIndex As Index In aView.Indexes
                        'anIndex.Refresh()

                        ' new
                        aColumnCollection = New ArrayList

                        For Each aColumn In anIndex.IndexedColumns

                            ' indx no
                            index = _fieldsDictionary.Item(aColumn.name.toupper)
                            '
                            '** check if primaryKey
                            'fill old primary Key structure
                            If anIndex.IndexKeyType = IndexKeyType.DriPrimaryKey Then
                                '_PrimaryKeyIndexName = anIndex.Name.ToUpper
                                '_NoPrimaryKeys = _NoPrimaryKeys + 1
                                'ReDim Preserve _Primarykeys(0 To _NoPrimaryKeys - 1)
                                '_Primarykeys(_NoPrimaryKeys - 1) = index - 1 ' set to the array 0...ubound
                            End If

                            aColumnCollection.Add(aColumn.name.toupper)

                        Next

                        '** store final

                        If _indexDictionary.ContainsKey(anIndex.Name.ToUpper) Then
                            _indexDictionary.Remove(key:=anIndex.Name.ToUpper)
                        End If
                        _indexDictionary.Add(key:=anIndex.Name.ToUpper, value:=aColumnCollection)
                    Next

                    myConnection.IsNativeInternalLocked = False

                    '**** read each Index
                    '****
                    Dim anIndexName As String = String.Empty

                    '**** build the commands
                    '****
                    Dim enumValues As Array = System.[Enum].GetValues(GetType(CommandType))
                    For Each anIndexName In _indexDictionary.Keys
                        Dim aNewCommand As SqlCommand
                        For Each aCommandType In enumValues
                            Dim aNewKey = New CommandKey(anIndexName, aCommandType)
                            aNewCommand = BuildCommand(anIndexName, aCommandType)
                            If Not aNewCommand Is Nothing Then
                                If _CommandStore.ContainsKey(aNewKey) Then
                                    _CommandStore.Remove(aNewKey)
                                End If
                                _CommandStore.Add(aNewKey, aNewCommand)
                            End If
                        Next


                    Next

                    _IsInitialized = True

                End SyncLock



                Return True

            Catch ex As Exception
                myConnection.IsNativeInternalLocked = False
                Call CoreMessageHandler(showmsgbox:=False, procedure:="mssqlViewSchema.refresh", containerID:=Me.ViewID, _
                                      argument:=reloadForce, exception:=ex)

                _IsInitialized = False
                Return False
            End Try

        End Function

    End Class

    ''' <summary>
    '''  describes the per View reference and Helper Class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class mssqlViewReader
        Inherits adonetViewReader
        Implements iormRelationalTableStore

        Dim WithEvents _dependency As SqlDependency ''' dependency object for the associated table

        'Protected Friend Shadows _cacheAdapter As sqlDataAdapter


        ''' <summary>
        ''' Initializes a new instance of the <see cref="mssqlViewReader" /> class.
        ''' </summary>
        ''' <param name="connection">The connection.</param>
        ''' <param name="viewID">The view ID.</param>
        ''' <param name="forceSchemaReload">The force schema reload.</param>
        Public Sub New(connection As iormConnection, viewID As String, forceSchemaReload As Boolean)
            MyBase.New(connection, viewID, forceSchemaReload)

        End Sub

        ''' <summary>
        ''' creates an unique key value. provide primary key array in the form {field1, field2, nothing}. "Nothing" will be increased.
        ''' </summary>
        ''' <param name="pkArray"></param>
        ''' <param name="tag"></param>
        ''' <remarks></remarks>
        ''' <returns>True if successfull new value</returns>
        Public Overrides Function CreateUniquePkValue(ByRef pkArray As Object(), Optional tag As String = Nothing) As Boolean
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function

        ''' <summary>
        ''' is Linq Available
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property IsLinqAvailable As Boolean Implements iormRelationalTableStore.IsLinqAvailable
            Get
                Return False
            End Get
        End Property
        ''' <summary>
        ''' create the specific native Command
        ''' </summary>
        ''' <param name="commandstr"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function CreateNativeDBCommand(commandstr As String, ByRef nativeConnection As IDbConnection) As IDbCommand
            Return New SqlCommand(cmdText:=commandstr, connection:=nativeConnection)
        End Function

        ''' <summary>
        ''' converts data to a specific type
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="targetType"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Convert2ContainerData(ByVal invalue As Object, ByRef outvalue As Object, _
                                                     targetType As Long, _
                                                     Optional ByVal maxsize As Long = 0, _
                                                    Optional ByRef abostrophNecessary As Boolean = False, _
                                                    Optional ByVal columnname As String = Nothing, _
                                                    Optional isnullable As Boolean? = Nothing, _
                                                    Optional defaultvalue As Object = Nothing) As Boolean _
                                                Implements iormRelationalTableStore.Convert2ContainerData

            If Not isnullable.HasValue And Not String.IsNullOrWhiteSpace(columnname) Then
                isnullable = Me.ContainerSchema.GetNullable(columnname)
            Else
                isnullable = False
            End If
            If defaultvalue Is Nothing And Not String.IsNullOrWhiteSpace(columnname) Then
                defaultvalue = Me.ContainerSchema.GetDefaultValue(columnname)
            End If
            '** return
            Return Me.Connection.DatabaseDriver.Convert2DBData(invalue:=invalue, outvalue:=outvalue, _
                                                               targetType:=targetType, maxsize:=maxsize, abostrophNecessary:=abostrophNecessary, _
                                                             columnname:=columnname, isnullable:=isnullable, defaultvalue:=defaultvalue)
        End Function


        '*********
        '********* cvt2ObjData returns a object from the Datatype of the column to XLS nterpretation
        '*********
        ''' <summary>
        ''' returns a object from the Datatype of the column to Host interpretation
        ''' </summary>
        ''' <param name="index">index as object (name or index 1..n)</param>
        ''' <param name="value">value to convert</param>
        ''' <param name="abostrophNecessary">True if necessary</param>
        ''' <returns>convered value </returns>
        ''' <remarks></remarks>
        Public Overrides Function Convert2ObjectData(ByVal index As Object, _
                                                     ByVal invalue As Object, _
                                                     ByRef outvalue As Object, _
                                                     Optional isnullable As Boolean? = Nothing, _
                                                     Optional defaultvalue As Object = Nothing, _
                                                     Optional ByRef abostrophNecessary As Boolean = False) As Boolean Implements iormRelationalTableStore.Convert2ObjectData
            Dim aSchema As mssqlViewSchema = Me.ContainerSchema
            Dim aDBColumn As adonetColumnDescription
            Dim result As Object = Nothing
            Dim fieldno As Integer


            Try

                fieldno = aSchema.GetEntryOrdinal(index)
                If fieldno < 0 Then
                    Call CoreMessageHandler(procedure:="mssqlViewReader.cvt2ColumnData", messagetype:=otCoreMessageType.InternalError, _
                                          message:="mssqlViewReader " & Me.ViewID & " anIndex for " & index & " not found", _
                                          containerID:=Me.ViewID, argument:=index)
                    System.Diagnostics.Debug.WriteLine("mssqlViewReader " & Me.ViewID & " anIndex for " & index & " not found")

                    Return False
                Else
                    aDBColumn = aSchema.GetColumnDescription(fieldno)
                End If

                If Not isnullable.HasValue Then
                    isnullable = Me.ContainerSchema.GetNullable(index)
                End If
                If defaultvalue = Nothing Then
                    defaultvalue = Me.ContainerSchema.GetDefaultValue(index)
                End If
                abostrophNecessary = False

                '** return
                Return Me.Connection.DatabaseDriver.Convert2ObjectData(invalue:=invalue, outvalue:=outvalue, _
                                                                   sourceType:=aDBColumn.DataType, abostrophNecessary:=abostrophNecessary, _
                                                                 isnullable:=isnullable, defaultvalue:=defaultvalue)

            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=False, procedure:="mssqlViewReader.convert2ObjectData", _
                                      argument:=aDBColumn.DataType, containerID:=Me.ViewID, entryname:=aDBColumn.ColumnName, exception:=ex, _
                                      messagetype:=otCoreMessageType.InternalError)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' handle the changes on the underlaying database model
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub mssqlViewREader_OnChangeNotification(sender As Object, e As SqlNotificationEventArgs)
            ''' not implemented
            ''' 
            Debug.WriteLine("uuuh")
        End Sub

        ''' <summary>
        ''' Initialize Cache 
        ''' </summary>
        ''' <returns>true if successfull </returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function InitializeCache(Optional ByVal force As Boolean = False) As Boolean

            Dim aCommand As SqlCommand
            Dim aDataSet As DataSet

            Try
                '** initialize
                If Not Me.IsCacheInitialized Or force Then
                    ''' TODO: build a view out of the loaded tables
                    ''' 


                    '** if the connection is during bootstrapping installation not available
                    Dim anativeConnection As SqlConnection = DirectCast(Me.Connection.NativeConnection, SqlConnection)
                    If anativeConnection Is Nothing OrElse _
                        (Not anativeConnection.State = ConnectionState.Open AndAlso DirectCast(Me.Connection, mssqlConnection).NativeInternalConnection.State = ConnectionState.Open) Then
                        anativeConnection = DirectCast(Me.Connection, mssqlConnection).NativeInternalConnection
                    End If
                    ' set theAdapter
                    _cacheAdapter = New SqlDataAdapter
                    MyBase._cacheAdapter = _cacheAdapter
                    _cacheAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
                    aDataSet = DirectCast(Me.Connection.DatabaseDriver, mssqlDBDriver).OnTrackDataSet
                    ' Select Command
                    aCommand = DirectCast(Me.ContainerSchema, mssqlTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, _
                                                                                          adonetTableSchema.CommandType.SelectType)
                    If Not aCommand Is Nothing Then
                        ' create cache with select on all but no where -> aCommand holds where on the primary keys
                        Dim selectstr As String = "SELECT "
                        For i = 1 To Me.ContainerSchema.NoEntries
                            selectstr &= "[" & Me.ContainerSchema.GetEntryName(i) & "]"
                            If i < Me.ContainerSchema.NoEntries Then
                                selectstr &= ","
                            End If
                        Next
                        selectstr &= " FROM [" & Me.NativeDBObjectname & "] "
                        _cacheAdapter.SelectCommand = New SqlCommand(selectstr)
                        _cacheAdapter.SelectCommand.CommandType = CommandType.Text
                        SyncLock anativeConnection
                            _cacheAdapter.SelectCommand.Connection = anativeConnection
                            _cacheAdapter.FillSchema(aDataSet, SchemaType.Source)
                            DirectCast(_cacheAdapter, SqlDataAdapter).Fill(aDataSet, Me.ViewID)
                        End SyncLock

                        '''
                        ''' register the callback event handlers for getting changes from the database
                        ''' 
                        If DirectCast(Me.Connection, mssqlConnection).CanRequestNotifications Then
                            Dim aDepCommand As SqlCommand = TryCast(_cacheAdapter.SelectCommand, SqlCommand)
                            aDepCommand.Notification = Nothing
                            _dependency = New SqlDependency(aDepCommand)

                            AddHandler _dependency.OnChange, AddressOf mssqlViewREader_OnChangeNotification
                        End If

                        ' set the Table
                        _cacheTable = aDataSet.Tables(Me.ViewID)
                        If _cacheTable Is Nothing Then
                            CoreMessageHandler(message:="Cache Table couldnot be read from database", _
                                                argument:=selectstr, procedure:="mssqlViewReader.InitializeCache", messagetype:=otCoreMessageType.InternalError)
                            Return False
                        End If

                        ' set the nulls
                        For Each aColumn As Data.DataColumn In _cacheTable.Columns
                            aColumn.AllowDBNull = Me.ContainerSchema.GetNullable(aColumn.ColumnName)
                        Next

                        ' Build DataViews per Index
                        For Each indexName As String In Me.ContainerSchema.Indices
                            Dim aDataview As DataView

                            If _cacheViews.ContainsKey(key:=indexName) Then
                                aDataview = _cacheViews.Item(key:=indexName)
                            Else
                                aDataview = New DataView(_cacheTable)
                            End If

                            Dim fieldlist As String = String.Empty
                            For Each columnname In Me.ContainerSchema.GetIndex(indexName)
                                If String.IsNullOrWhiteSpace(fieldlist) Then
                                    fieldlist &= columnname
                                Else
                                    fieldlist &= "," & columnname
                                End If
                            Next
                            aDataview.Sort = fieldlist
                            If Not _cacheViews.ContainsKey(key:=indexName) Then
                                _cacheViews.Add(key:=indexName, value:=aDataview)
                            End If
                        Next


                    End If

                    ''' build also  the adapter commands to update the View if any of the tables are being updated
                    ''' 


                    ' Delete Command
                    aCommand = DirectCast(Me.ContainerSchema, mssqlTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, _
                                                                                          adonetTableSchema.CommandType.DeleteType)
                    If Not aCommand Is Nothing Then
                        _cacheAdapter.DeleteCommand = aCommand
                    End If

                    ' Insert Command
                    aCommand = DirectCast(Me.ContainerSchema, mssqlTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, _
                                                                                          adonetTableSchema.CommandType.InsertType)
                    If Not aCommand Is Nothing Then
                        _cacheAdapter.InsertCommand = aCommand
                    End If
                    ' Update Command
                    aCommand = DirectCast(Me.ContainerSchema, mssqlTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, _
                                                                                          adonetTableSchema.CommandType.UpdateType)
                    If Not aCommand Is Nothing Then
                        _cacheAdapter.UpdateCommand = aCommand
                    End If
                    '** return true
                    Return True
                Else
                    Return False
                End If



            Catch ex As Exception
                Call CoreMessageHandler(procedure:="mssqlViewReader.initializeCache", exception:=ex, message:="Exception", _
                                      messagetype:=otCoreMessageType.InternalError, containerID:=Me.ViewID)
                Return False
            End Try

        End Function
        ''' <summary>
        ''' update the cache Datatable
        ''' </summary>
        ''' <param name="datatable"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function UpdateDBDataTable(ByRef dataadapter As IDbDataAdapter, ByRef datatable As DataTable) As Integer
            Try
                Return DirectCast(dataadapter, SqlDataAdapter).Update(datatable)
            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception occured", procedure:="mssqlViewReader.UpdateDBDataTable", exception:=ex, _
                                        messagetype:=otCoreMessageType.InternalError, containerID:=Me.ViewID)
                Return 0
            End Try

        End Function
    End Class


    '************************************************************************************
    '***** CLASS mssqlTableStore describes the per Table reference and Helper Class
    '*****                    ORM Mapping Class and Table Access Workhorse
    '*****
    ''' <summary>
    '''  describes the per Table reference and Helper Class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class mssqlTableStore
        Inherits adonetTableStore
        Implements iormRelationalTableStore

        Dim WithEvents _dependency As SqlDependency ''' dependency object for the associated table

        'Protected Friend Shadows _cacheAdapter As sqlDataAdapter

        '** initialize
        Public Sub New(connection As iormConnection, tableID As String, ByVal forceSchemaReload As Boolean)
            Call MyBase.New(Connection:=connection, tableID:=tableID, forceSchemaReload:=forceSchemaReload)
        End Sub


        ''' <summary>
        ''' is Linq Available
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property IsLinqAvailable As Boolean Implements iormRelationalTableStore.IsLinqAvailable
            Get
                Return False
            End Get
        End Property
        ''' <summary>
        ''' create the specific native Command
        ''' </summary>
        ''' <param name="commandstr"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function CreateNativeDBCommand(commandstr As String, ByRef nativeConnection As IDbConnection) As IDbCommand
            Return New SqlCommand(cmdText:=commandstr, connection:=nativeConnection)
        End Function

        ''' <summary>
        ''' converts data to a specific type
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="targetType"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Convert2ContainerData(ByVal invalue As Object, ByRef outvalue As Object, _
                                                     targetType As Long, _
                                                     Optional ByVal maxsize As Long = 0, _
                                                    Optional ByRef abostrophNecessary As Boolean = False, _
                                                    Optional ByVal columnname As String = Nothing, _
                                                    Optional isnullable As Boolean? = Nothing, _
                                                    Optional defaultvalue As Object = Nothing) As Boolean _
                                                Implements iormRelationalTableStore.Convert2ContainerData

            If Not isnullable.HasValue And Not String.IsNullOrWhiteSpace(columnname) Then
                isnullable = Me.ContainerSchema.GetNullable(columnname)
            Else
                isnullable = False
            End If
            If defaultvalue Is Nothing And Not String.IsNullOrWhiteSpace(columnname) Then
                defaultvalue = Me.ContainerSchema.GetDefaultValue(columnname)
            End If
            '** return
            Return Me.Connection.DatabaseDriver.Convert2DBData(invalue:=invalue, outvalue:=outvalue, _
                                                               targetType:=targetType, maxsize:=maxsize, abostrophNecessary:=abostrophNecessary, _
                                                             columnname:=columnname, isnullable:=isnullable, defaultvalue:=defaultvalue)
        End Function


        '*********
        '********* cvt2ObjData returns a object from the Datatype of the column to XLS nterpretation
        '*********
        ''' <summary>
        ''' returns a object from the Datatype of the column to Host interpretation
        ''' </summary>
        ''' <param name="index">index as object (name or index 1..n)</param>
        ''' <param name="value">value to convert</param>
        ''' <param name="abostrophNecessary">True if necessary</param>
        ''' <returns>convered value </returns>
        ''' <remarks></remarks>
        Public Overrides Function Convert2ObjectData(ByVal index As Object, _
                                                     ByVal invalue As Object, _
                                                     ByRef outvalue As Object, _
                                                     Optional isnullable As Boolean? = Nothing, _
                                                     Optional defaultvalue As Object = Nothing, _
                                                     Optional ByRef abostrophNecessary As Boolean = False) As Boolean Implements iormRelationalTableStore.Convert2ObjectData
            Dim aSchema As mssqlTableSchema = Me.ContainerSchema
            Dim aDBColumn As adonetColumnDescription
            Dim result As Object = Nothing
            Dim fieldno As Integer


            Try

                fieldno = aSchema.GetEntryOrdinal(index)
                If fieldno < 0 Then
                    Call CoreMessageHandler(procedure:="mssqlTableStore.cvt2ColumnData", messagetype:=otCoreMessageType.InternalError, _
                                          message:="mssqlTableStore " & Me.TableID & " anIndex for " & index & " not found", _
                                          containerID:=Me.TableID, argument:=index)
                    System.Diagnostics.Debug.WriteLine("mssqlTableStore " & Me.TableID & " anIndex for " & index & " not found")

                    Return False
                Else
                    aDBColumn = aSchema.GetColumnDescription(fieldno)
                End If

                If Not isnullable.HasValue Then
                    isnullable = Me.ContainerSchema.GetNullable(index)
                End If
                If defaultvalue = Nothing Then
                    defaultvalue = Me.ContainerSchema.GetDefaultValue(index)
                End If
                abostrophNecessary = False

                '** return
                Return Me.Connection.DatabaseDriver.Convert2ObjectData(invalue:=invalue, outvalue:=outvalue, _
                                                                   sourceType:=aDBColumn.DataType, abostrophNecessary:=abostrophNecessary, _
                                                                 isnullable:=isnullable, defaultvalue:=defaultvalue)

            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=False, procedure:="mssqlTableStore.convert2ObjectData", _
                                      argument:=aDBColumn.DataType, containerID:=Me.TableID, entryname:=aDBColumn.ColumnName, exception:=ex, _
                                      messagetype:=otCoreMessageType.InternalError)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' handle the changes on the underlaying database model
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub mssqlTableStore_OnChangeNotification(sender As Object, e As SqlNotificationEventArgs)
            ''' not implemented
            ''' 
            Debug.WriteLine("uuuh")
        End Sub

        ''' <summary>
        ''' Initialize Cache 
        ''' </summary>
        ''' <returns>true if successfull </returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function InitializeCache(Optional ByVal force As Boolean = False) As Boolean

            Dim aCommand As SqlCommand
            Dim aDataSet As DataSet

            Try
                '** initialize
                If Not Me.IsCacheInitialized Or force Then
                    '** if the connection is during bootstrapping installation not available
                    Dim anativeConnection As SqlConnection = DirectCast(Me.Connection.NativeConnection, SqlConnection)
                    If anativeConnection Is Nothing OrElse _
                        (Not anativeConnection.State = ConnectionState.Open AndAlso DirectCast(Me.Connection, mssqlConnection).NativeInternalConnection.State = ConnectionState.Open) Then
                        anativeConnection = DirectCast(Me.Connection, mssqlConnection).NativeInternalConnection
                    End If
                    ' set theAdapter
                    _cacheAdapter = New SqlDataAdapter
                    MyBase._cacheAdapter = _cacheAdapter
                    _cacheAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
                    aDataSet = DirectCast(Me.Connection.DatabaseDriver, mssqlDBDriver).OnTrackDataSet
                    ' Select Command
                    aCommand = DirectCast(Me.ContainerSchema, mssqlTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, _
                                                                                          adonetTableSchema.CommandType.SelectType)
                    If Not aCommand Is Nothing Then
                        ' create cache with select on all but no where -> aCommand holds where on the primary keys
                        Dim selectstr As String = "SELECT "
                        For i = 1 To Me.ContainerSchema.NoEntries
                            selectstr &= "[" & Me.ContainerSchema.GetEntryName(i) & "]"
                            If i < Me.ContainerSchema.NoEntries Then
                                selectstr &= ","
                            End If
                        Next
                        selectstr &= " FROM [" & Me.NativeDBObjectname & "] "
                        _cacheAdapter.SelectCommand = New SqlCommand(selectstr)
                        _cacheAdapter.SelectCommand.CommandType = CommandType.Text
                        SyncLock anativeConnection
                            _cacheAdapter.SelectCommand.Connection = anativeConnection
                            _cacheAdapter.FillSchema(aDataSet, SchemaType.Source)
                            DirectCast(_cacheAdapter, SqlDataAdapter).Fill(aDataSet, Me.TableID)
                        End SyncLock

                        '''
                        ''' register the callback event handlers for getting changes from the database
                        ''' 
                        If DirectCast(Me.Connection, mssqlConnection).CanRequestNotifications Then
                            Dim aDepCommand As SqlCommand = TryCast(_cacheAdapter.SelectCommand, SqlCommand)
                            aDepCommand.Notification = Nothing
                            _dependency = New SqlDependency(aDepCommand)

                            AddHandler _dependency.OnChange, AddressOf mssqlTableStore_OnChangeNotification
                        End If

                        ' set the Table
                        _cacheTable = aDataSet.Tables(Me.TableID)
                        If _cacheTable Is Nothing Then
                            CoreMessageHandler(message:="Cache Table couldnot be read from database", _
                                                argument:=selectstr, procedure:="mssqlTableStore.InitializeCache", messagetype:=otCoreMessageType.InternalError)
                            Return False
                        End If

                        ' set the nulls
                        For Each aColumn As Data.DataColumn In _cacheTable.Columns
                            aColumn.AllowDBNull = Me.ContainerSchema.GetNullable(aColumn.ColumnName)
                        Next

                        ' Build DataViews per Index
                        For Each indexName As String In Me.ContainerSchema.Indices
                            Dim aDataview As DataView

                            If _cacheViews.ContainsKey(key:=indexName) Then
                                aDataview = _cacheViews.Item(key:=indexName)
                            Else
                                aDataview = New DataView(_cacheTable)
                            End If

                            Dim fieldlist As String = String.Empty
                            For Each columnname In Me.ContainerSchema.GetIndex(indexName)
                                If String.IsNullOrWhiteSpace(fieldlist) Then
                                    fieldlist &= columnname
                                Else
                                    fieldlist &= "," & columnname
                                End If
                            Next
                            aDataview.Sort = fieldlist
                            If Not _cacheViews.ContainsKey(key:=indexName) Then
                                _cacheViews.Add(key:=indexName, value:=aDataview)
                            End If
                        Next


                    End If

                    ' Delete Command
                    aCommand = DirectCast(Me.ContainerSchema, mssqlTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, _
                                                                                          adonetTableSchema.CommandType.DeleteType)
                    If Not aCommand Is Nothing Then
                        _cacheAdapter.DeleteCommand = aCommand
                    End If

                    ' Insert Command
                    aCommand = DirectCast(Me.ContainerSchema, mssqlTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, _
                                                                                          adonetTableSchema.CommandType.InsertType)
                    If Not aCommand Is Nothing Then
                        _cacheAdapter.InsertCommand = aCommand
                    End If
                    ' Update Command
                    aCommand = DirectCast(Me.ContainerSchema, mssqlTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, _
                                                                                          adonetTableSchema.CommandType.UpdateType)
                    If Not aCommand Is Nothing Then
                        _cacheAdapter.UpdateCommand = aCommand
                    End If

                    '** return true
                    Return True
                Else
                    Return False
                End If



            Catch ex As Exception
                Call CoreMessageHandler(procedure:="mssqlTableStore.initializeCache", exception:=ex, message:="Exception", _
                                      messagetype:=otCoreMessageType.InternalError, containerID:=Me.TableID)
                Return False
            End Try

        End Function
        ''' <summary>
        ''' update the cache Datatable
        ''' </summary>
        ''' <param name="datatable"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function UpdateDBDataTable(ByRef dataadapter As IDbDataAdapter, ByRef datatable As DataTable) As Integer
            Try
                Return DirectCast(dataadapter, SqlDataAdapter).Update(datatable)
            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception occured", procedure:="mssqlTableStore.UpdateDBDataTable", exception:=ex, _
                                       messagetype:=otCoreMessageType.InternalError, containerID:=Me.TableID)
                Return 0
            End Try

        End Function
    End Class

    ''' <summary>
    ''' visitor for selection rules to build an ms sql statement out of the selection expression
    ''' </summary>
    ''' <remarks></remarks>
    Public Class mssqlXPTVisitor
        Inherits rulez.eXPressionTree.Visitor(Of String)
        Implements IRDBVisitor

        ''' <summary>
        ''' parameters
        ''' </summary>
        ''' <remarks></remarks>
        Private _parameters As New List(Of ormSqlCommandParameter)
        ''' <summary>
        ''' list of tableids
        ''' </summary>
        ''' <remarks></remarks>
        Private _tableids As New List(Of String)
        ''' <summary>
        ''' list of results
        ''' </summary>
        ''' <remarks></remarks>
        Private _results As New List(Of String)
        ''' <summary>
        ''' return the result of the visitor
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Result() As String Implements IRDBVisitor.Result
            If Me.Stack.Count > 1 Then Return Me.Stack.Pop
            Return String.Empty
        End Function

        ''' <summary>
        ''' gets the parameters of the sql query
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Parameters As List(Of ormSqlCommandParameter) Implements IRDBVisitor.Parameters
            Get
                Return _parameters
            End Get
        End Property
        ''' <summary>
        ''' gets a list of table ids
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TableIDs As List(Of String) Implements IRDBVisitor.TableIDs
            Get
                Return _tableids
            End Get
        End Property
        ''' <summary>
        ''' returns the select sql string
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function [Select]() As String Implements IRDBVisitor.Select
            Dim result As String = String.Empty
            For Each aResult In _results
                If String.IsNullOrEmpty(result) Then
                    result = aResult
                Else
                    result &= ", " & aResult
                End If
            Next
            Return result
        End Function
        ''' <summary>
        ''' Visiting selection rule
        ''' </summary>
        ''' <param name="o"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnSelectionRuleVisiting(o As Object, e As VisitorEventArgs(Of String)) Handles Me.VisitingSelectionRule
            Dim aRule As SelectionRule = e.CurrentNode

            ''' add the results for full object add the keys
            For Each aResult As Result In aRule.Result
                If aResult.Embedded.NodeTokenType = otXPTNodeType.DataObjectSymbol Then
                    If aResult.Embedded.GetType() Is GetType(DataObjectSymbol) Then
                        Dim aDataObjectSymbol As DataObjectSymbol = aResult.Embedded
                        Dim aObjectDefinition As ormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=aDataObjectSymbol.ObjectID)
                        If aObjectDefinition IsNot Nothing Then
                            For Each aKeyName In aObjectDefinition.PrimaryKeyEntryNames
                                Dim anEntry As ormObjectFieldEntry = aObjectDefinition.GetEntryDefinition(aKeyName)
                                _results.Add(anEntry.ContainerID & ".[" & anEntry.ContainerEntryName & "]")
                            Next
                        End If
                    Else
                        Dim aDataObjectEntrySymbol As DataObjectEntrySymbol = aResult.Embedded
                        Dim aObjectDefinition As ormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=aDataObjectEntrySymbol.ObjectID)
                        If aObjectDefinition IsNot Nothing Then
                            Dim anEntry As ormObjectFieldEntry = aObjectDefinition.GetEntryDefinition(aDataObjectEntrySymbol.Entryname)
                            _results.Add(anEntry.ContainerID & ".[" & anEntry.ContainerEntryName & "]")
                        End If
                    End If
                End If
            Next
        End Sub
        ''' <summary>
        ''' Visiting a data object symbol
        ''' </summary>
        ''' <param name="o"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnDataObjectVisiting(o As Object, e As VisitorEventArgs(Of String)) Handles Me.VisitingDataObjectSymbol
            Dim result As String = String.Empty
            Dim aNode As DataObjectEntrySymbol = e.CurrentNode
            Dim anObjectDefinition As iormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=aNode.ObjectID)

            If anObjectDefinition IsNot Nothing Then
                Dim anObjectEntryDefinition As iormObjectEntryDefinition = anObjectDefinition.GetEntryDefinition(entryname:=aNode.Entryname)

                ''' build the symbol by referencing the containerID = tableid and the entryname
                ''' of an  ormObjectContainerEntry which is the 
                If anObjectEntryDefinition IsNot Nothing AndAlso anObjectEntryDefinition.IsContainer Then
                    Dim aTableid As String = DirectCast(anObjectEntryDefinition, ormObjectFieldEntry).ContainerEntryDefinition.ContainerID
                    '' store the tableid
                    If Not _tableids.Contains(aTableid) Then _tableids.Add(aTableid)
                    '' build the symbol expression
                    result = aTableid & ".[" & DirectCast(anObjectEntryDefinition, ormObjectFieldEntry).ContainerEntryDefinition.EntryName & "]"

                ElseIf anObjectEntryDefinition IsNot Nothing Then
                    Throw New NotImplementedException("building a sql where expression on a compound entry is not implemented")
                Else
                    CoreMessageHandler(message:="object entry definition was not found in repository", procedure:="mssqlXPTVisitor.VisitingDataObjectSymbol", _
                                        messagetype:=otCoreMessageType.InternalError, objectname:=aNode.ObjectID, entryname:=aNode.Entryname)
                End If

            Else
                CoreMessageHandler(message:="object  definition was not found in repository", procedure:="mssqlXPTVisitor.VisitingDataObjectSymbol", _
                               messagetype:=otCoreMessageType.InternalError, objectname:=aNode.ObjectID, entryname:=aNode.Entryname)

            End If

            ''' put it on stack
            e.Stack.Push(result)
        End Sub

        ''' <summary>
        ''' Visiting a Variable
        ''' </summary>
        ''' <param name="o"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnVariableVisiting(o As Object, e As VisitorEventArgs(Of String)) Handles Me.VisitingVariable
            Dim result As String = String.Empty
            Dim aNode As Variable = e.CurrentNode

            ''' name it
            result = "@" & aNode.ID.ToUpper

            '' store the variable as sql command parameter
            Dim aParameter As New ormSqlCommandParameter(aNode.ID.ToUpper, DataType:=aNode.Type, notColumn:=True)
            _parameters.Add(aParameter)

            ''' put it on stack
            e.Stack.Push(result)
        End Sub
        ''' <summary>
        ''' Visiting a Literal
        ''' </summary>
        ''' <param name="o"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnLiteralVisiting(o As Object, e As VisitorEventArgs(Of String)) Handles Me.VisitingLiteral
            Dim aNode As Literal = TryCast(e.CurrentNode, Literal)
            Dim result As String = String.Empty

            ''' convert the literal
            If aNode.Value IsNot Nothing Then
                Select Case aNode.Datatype
                    Case otDataType.Memo, otDataType.Text
                        result = "'" & aNode.Value.ToString() & "'"
                    Case otDataType.Time
                        result = "CAST('" & aNode.Value.ToString("HH:mm:ss") & "' AS TIME)"
                    Case otDataType.Timestamp
                        result = "CAST('" & aNode.Value.ToString("yyyy-MM-ddTHH:mm:ss") & "' AS DATETIME)"
                    Case otDataType.Date
                        result = "CAST('" & aNode.Value.ToString("yyyy-MM-dd") & "' AS DATE)"
                    Case otDataType.Void
                        result = " NULL "
                    Case otDataType.Bool
                        ''' convert the value
                        If Core.DataType.ToBool(aNode.Value) = True Then
                            result = "1"
                        Else
                            result = "0"
                        End If

                    Case otDataType.Formula, otDataType.Binary, otDataType.Runtime, otDataType.Money
                        CoreMessageHandler(message:="cannot convert literal for ontrack datatypes formula, binary, runtime, money", _
                                            procedure:="mssqlXPTVisitorEvents.VisitingLiteral", messagetype:=otCoreMessageType.InternalError)

                    Case Else
                        result = aNode.Value.ToString()
                End Select
            Else
                result = " NULL "
            End If

            ''' on stack
            e.Stack.Push(result)
        End Sub
        ''' <summary>
        ''' Handle the Expression Visiting Event
        ''' </summary>
        ''' <param name="o"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnLogicalExpressionVisited(o As Object, e As VisitorEventArgs(Of String)) Handles Me.VisitedLogicalExpression
            Dim rightresult As String
            Dim leftresult As String

            Dim node As LogicalExpression = TryCast(e.CurrentNode, LogicalExpression)
            If node IsNot Nothing Then
                If node.Operator.Arguments >= 2 Then
                    rightresult = e.Stack.Pop
                    ''' include on the right side parenthesises '()'
                    If node.RightOperand.NodeTokenType = otXPTNodeType.LogicalExpression Then
                        If node.Operator.Priority <> DirectCast(node.RightOperand, LogicalExpression).Operator.Priority Then
                            rightresult = "( " & rightresult & " )"
                        End If
                    ElseIf node.RightOperand.NodeTokenType = otXPTNodeType.OperationExpression Then
                        rightresult = "( " & rightresult & " )"
                    End If
                End If

                If node.Operator.Arguments >= 1 Then
                    leftresult = e.Stack.Pop
                    ''' include on the left side parenthesises '()'
                    If node.LeftOperand.NodeTokenType = otXPTNodeType.LogicalExpression Then
                        If node.Operator.Priority <> DirectCast(node.LeftOperand, LogicalExpression).Operator.Priority Then
                            leftresult = "( " & leftresult & " )"
                        End If
                    ElseIf node.LeftOperand.NodeTokenType = otXPTNodeType.OperationExpression Then
                        leftresult = "( " & leftresult & " )"
                    End If
                End If

                ''' create the sql expression for the subexpressions
                Dim result As String = String.Empty
                Select Case (node.Operator.TokenID.ToInt)

                    Case Token.AND, Token.ANDALSO
                        result = leftresult & " AND " & rightresult
                    Case Token.OR, Token.ORELSE
                        result = leftresult & " OR " & rightresult
                        '''

                    Case Token.EQ
                        ''' if the right operand is a literal with nothing -> Check on NULL
                        If node.RightOperand IsNot Nothing AndAlso _
                            node.RightOperand.NodeTokenType = otXPTNodeType.Literal AndAlso DirectCast(node.RightOperand, Literal).Value Is Nothing Then
                            result = leftresult & " IS NULL"
                        Else
                            result = leftresult & " = " & rightresult
                        End If

                    Case Token.NEQ
                        ''' if the right operand is a literal with nothing -> Check on NULL
                        If node.RightOperand IsNot Nothing AndAlso _
                            node.RightOperand.NodeTokenType = otXPTNodeType.Literal AndAlso DirectCast(node.RightOperand, Literal).Value Is Nothing Then
                            result = leftresult & " IS NOT NULL"
                        Else
                            result = leftresult & " <> " & rightresult
                        End If

                    Case Token.GT
                        result = leftresult & " > " & rightresult
                    Case Token.GE
                        result = leftresult & " >= " & rightresult
                    Case Token.LE
                        result = leftresult & " <= " & rightresult
                    Case Token.LT
                        result = leftresult & " < " & rightresult
                    Case Token.NOT
                        result = "NOT " & leftresult
                    Case Token.POS
                        result = " TRUE "
                    Case Else
                        CoreMessageHandler(message:="operator for token " & node.Operator.TokenID.ToString() & " is not defined", messagetype:=otCoreMessageType.InternalError, _
                                            procedure:=" mssqlXPTVisitor.VisitedLogicalExpression")
                End Select

                ''' push the result on stack
                e.Stack.Push(result)
            End If
        End Sub
        ''' <summary>
        ''' Handle the Operation Expression Visiting Event
        ''' </summary>
        ''' <param name="o"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnOperationExpressionVisited(o As Object, e As VisitorEventArgs(Of String)) Handles Me.VisitedOperationExpression
            Dim rightresult As String
            Dim leftresult As String

            Dim node As OperationExpression = TryCast(e.CurrentNode, OperationExpression)
            If node IsNot Nothing Then
                If node.Operator.Arguments >= 2 Then
                    rightresult = e.Stack.Pop
                    ''' include on the right side parenthesises '()'
                    If node.RightOperand.NodeTokenType = otXPTNodeType.LogicalExpression Then
                        If node.Operator.Priority <> DirectCast(node.RightOperand, OperationExpression).Operator.Priority Then
                            rightresult = "( " & rightresult & " )"
                        End If
                    ElseIf node.RightOperand.NodeTokenType = otXPTNodeType.OperationExpression Then
                        rightresult = "( " & rightresult & " )"
                    End If
                End If

                If node.Operator.Arguments >= 1 Then
                    leftresult = e.Stack.Pop
                    ''' include on the left side parenthesises '()'
                    If node.LeftOperand.NodeTokenType = otXPTNodeType.LogicalExpression Then
                        If node.Operator.Priority <> DirectCast(node.LeftOperand, OperationExpression).Operator.Priority Then
                            leftresult = "( " & leftresult & " )"
                        End If
                    ElseIf node.LeftOperand.NodeTokenType = otXPTNodeType.OperationExpression Then
                        leftresult = "( " & leftresult & " )"
                    End If
                End If

                ''' create the sql expression for the subexpressions
                Dim result As String = String.Empty
                Select Case (node.Operator.TokenID.ToInt)

                    Case Token.PLUS
                        result = leftresult & " + " & rightresult
                    Case Token.MINUS
                        result = leftresult & " - " & rightresult
                    Case Token.MULT
                        result = leftresult & " * " & rightresult
                    Case Token.DIV
                        result = leftresult & " / " & rightresult
                    Case Token.CONCAT
                        result = "CONCAT(" & leftresult & "," & rightresult & ")"
                    Case Else
                        CoreMessageHandler(message:="operator for token " & node.Operator.TokenID.ToString() & " is not defined", messagetype:=otCoreMessageType.InternalError, _
                                            procedure:=" mssqlXPTVisitor.VisitedOperationExpression")
                End Select

                ''' push the result on stack
                e.Stack.Push(result)
            End If
        End Sub
        ''' <summary>
        ''' Handle the Operation Expression Visiting Event
        ''' </summary>
        ''' <param name="o"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnFunctionCallVisited(o As Object, e As VisitorEventArgs(Of String)) Handles Me.VisitedFunctionCall

            Dim aNode As FunctionCall = TryCast(e.CurrentNode, FunctionCall)
            If aNode IsNot Nothing Then

                ''' create the sql expression for the subexpressions
                Dim result As String = String.Empty
                Select Case (aNode.Function.TokenID.ToInt)
                    Case Else
                        CoreMessageHandler(message:="Function for token " & aNode.Function.TokenID.ToString() & " is not defined", messagetype:=otCoreMessageType.InternalError, _
                                            procedure:=" mssqlXPTVisitor.VisitedFunctionCall")
                End Select

                ''' push the result on stack
                e.Stack.Push(result)
            End If
        End Sub
    End Class
End Namespace
