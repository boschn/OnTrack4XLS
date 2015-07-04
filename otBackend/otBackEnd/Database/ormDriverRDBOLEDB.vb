REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** Driver Wrapper for ADO.NET OLEDB Classes for On Track Database Back end Library
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
Imports System.Data.OleDb
Imports System.Linq
Imports System.Text.RegularExpressions
Imports OnTrack.Core
Imports OnTrack.rulez.eXPressionTree
Imports OnTrack.rulez



Namespace OnTrack.Database


    ''' <summary>
    ''' oleDBDriver is the database driver for ADO.NET OLEDB drivers
    ''' </summary>
    ''' <remarks></remarks>
    <ormDatabaseDriver(autoinstance:=False, Name:=ConstCPVDriverOleDB, isontrackdriver:=True, Version:=2)> Public Class oleDBDriver
        Inherits AdoNetRDBDriver
        Implements iormRelationalDatabaseDriver

        Protected Friend Shadows WithEvents _primaryConnection As oledbConnection '-> in clsOTDBDriver
        Private Shadows _ParametersTableAdapter As New OleDbDataAdapter
        Shadows Event RequestBootstrapInstall(sender As Object, e As SessionBootstrapEventArgs) Implements iormRelationalDatabaseDriver.RequestBootstrapInstall

        Public Sub New()
            Call MyBase.New()
            If Me._primaryConnection Is Nothing Then
                _primaryConnection = New oledbConnection(id:="primary", DatabaseDriver:=Me, session:=Session, sequence:=ComplexPropertyStore.Sequence.Primary)
            End If
        End Sub
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ID">an ID for this driver</param>
        ''' <remarks></remarks>
        Public Sub New(id As String, ByRef session As Session)
            Call MyBase.New(id, session)
            If Me._primaryConnection Is Nothing Then
                _primaryConnection = New oledbConnection(id:="primary", DatabaseDriver:=Me, session:=session, sequence:=ComplexPropertyStore.Sequence.Primary)
            End If
        End Sub


        ''' <summary>
        ''' NativeConnection
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads ReadOnly Property NativeConnection() As OleDb.OleDbConnection
            Get
                Return DirectCast(_primaryConnection.NativeConnection, System.Data.OleDb.OleDbConnection)
            End Get

        End Property

        ''' <summary>
        ''' builds the adapter for the parameters table
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function BuildParameterAdapter()

            With _ParametersTableAdapter


                .SelectCommand.Prepare()

                ' Create the commands.
                '**** INSERT
                .InsertCommand = New OleDbCommand( _
                    String.Format("INSERT INTO  [{0}] ([{1}], [{2}] , [{3}] , [{4}], [{5}])  VALUES (?,?,?,?,?) ", _
                    GetNativeDBObjectName(_parametersTableName), ConstFNSetupID, ConstFNID, ConstFNValue, ConstFNChangedOn, constFNDescription))
                ' Create the parameters.
                .InsertCommand.Parameters.Add("@SETUPID", OleDbType.Char, 50, ConstFNSetupID)
                .InsertCommand.Parameters.Add("@ID", OleDbType.Char, 50, ConstFNID)
                .InsertCommand.Parameters.Add("@Value", OleDbType.VarChar, 250, ConstFNValue)
                .InsertCommand.Parameters.Add("@changedOn", OleDbType.VarChar, 50, ConstFNChangedOn)
                .InsertCommand.Parameters.Add("@description", OleDbType.VarChar, 250, constFNDescription)
                .InsertCommand.Connection = DirectCast(_primaryConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection)
                .InsertCommand.Prepare()


                '**** UPDATE
                .UpdateCommand = New OleDbCommand( _
                    String.Format("UPDATE [{0}] SET [{1}] = ? , [{2}] = ? , [{3}] = ? WHERE [{4}] = ? AND [{5}] = ?", _
                              GetNativeDBObjectName(_parametersTableName), ConstFNValue, ConstFNChangedOn, constFNDescription, ConstFNID, ConstFNSetupID))
                ' Create the parameters.
                .UpdateCommand.Parameters.Add("@Value", OleDbType.VarChar, 250, ConstFNValue)
                .UpdateCommand.Parameters.Add("@changedOn", OleDbType.VarChar, 50, ConstFNChangedOn)
                .UpdateCommand.Parameters.Add("@description", OleDbType.VarChar, 250, constFNDescription)
                .UpdateCommand.Parameters.Add("@ID", OleDbType.Char, 50, ConstFNID).SourceVersion = DataRowVersion.Original
                .UpdateCommand.Parameters.Add("@setupID", OleDbType.Char, 50, ConstFNSetupID).SourceVersion = DataRowVersion.Original
                .UpdateCommand.Connection = DirectCast(_primaryConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection)
                .UpdateCommand.Prepare()


                '***** DELETE
                .DeleteCommand = New OleDbCommand(String.Format("DELETE FROM [{0}] where [{1}] = @id AND [{2}] = @SETUPID", GetNativeDBObjectName(_parametersTableName), ConstFNID, ConstFNSetupID))
                .DeleteCommand.Parameters.Add("@ID", OleDbType.Char, 50, "ID").SourceVersion = DataRowVersion.Original
                .DeleteCommand.Parameters.Add("@setupID", OleDbType.Char, 50, ConstFNSetupID).SourceVersion = DataRowVersion.Original
                .DeleteCommand.Connection = DirectCast(_primaryConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection)
                .DeleteCommand.Prepare()

            End With

        End Function
        '***
        '*** Initialize Driver
        ''' <summary>
        ''' Initialize the driver
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
                    _primaryConnection = New oledbConnection("primary", Me, ComplexPropertyStore.Sequence.Primary, session:=_session)
                End If

                '*** do we have the Table ?! - donot do this in bootstrapping since we are running in recursion then
                If Not Me.HasTable(_parametersTableName) And Not _session.IsBootstrappingInstallationRequested Then
                    If Not VerifyOnTrackDatabase(install:=False) Then
                        '* now in bootstrap ?!
                        If _session.IsBootstrappingInstallationRequested Then
                            CoreMessageHandler(message:="verifying the database failed moved to bootstrapping - caching parameters meanwhile", _
                                               procedure:="oleDBDriver.Initialize", _
                                          messagetype:=otCoreMessageType.InternalWarning, argument:=Me.ID)
                            Me.IsInitialized = True
                            Return True
                        Else
                            CoreMessageHandler(message:="verifying the database failed - failed to initialize driver", procedure:="oleDBDriver.Initialize", _
                                              messagetype:=otCoreMessageType.InternalError, argument:=Me.ID)
                            Me.IsInitialized = False
                            Return False
                        End If
                    End If
                End If

                '*** end of bootstrapping conditions reinitialize automatically
                '*** might be that we are now in bootstrapping
                If Not _session.IsBootstrappingInstallationRequested OrElse force Then
                    '*** set the DataTable
                    If _OnTrackDataSet Is Nothing Then _OnTrackDataSet = New DataSet(Me.ID & Date.Now.ToString)

                    '** create adapaters
                    If Me.HasTable(_parametersTableName) Then
                        ' the command
                        Dim aDBCommand = New OleDbCommand()
                        aDBCommand.CommandText = String.Format("select [{0}],[{1}],[{2}],[{3}],[{4}] from [{5}] ", _
                                                             ConstFNSetupID, ConstFNID, ConstFNValue, ConstFNChangedOn, constFNDescription, GetNativeDBObjectName(_parametersTableName))

                        aDBCommand.Connection = DirectCast(_primaryConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection)
                        ' fill with adapter
                        _ParametersTableAdapter = New OleDbDataAdapter()
                        _ParametersTableAdapter.SelectCommand = aDBCommand
                        _ParametersTableAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
                        _ParametersTableAdapter.FillSchema(_OnTrackDataSet, SchemaType.Source)
                        _ParametersTableAdapter.Fill(_OnTrackDataSet, _parametersTableName)
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
                Call CoreMessageHandler(procedure:="oleDBDriver.OnConnection", message:="couldnot Initialize Driver", _
                                      exception:=ex)
                Me.IsInitialized = False
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Gets the driver name
        ''' </summary>
        ''' <value>The type.</value>
        Public Overrides ReadOnly Property Name() As String Implements iormDatabaseDriver.Name
            Get
                Return ConstCPVDriverOleDB
            End Get
        End Property

        '' <summary>
        ''' Gets the native database name 
        ''' </summary>
        ''' <value>The type.</value>
        Public Overrides ReadOnly Property NativeDatabaseName As String Implements iormRelationalDatabaseDriver.NativeDatabaseName
            Get
                Dim myNativeConnection As OleDb.OleDbConnection = TryCast(NativeConnection, OleDb.OleDbConnection)

                If myNativeConnection Is Nothing Then
                    Call CoreMessageHandler(procedure:="oleDBDriver.NativeDatabaseName", message:="No current Connection to the Database", _
                                          messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If

                If myNativeConnection.State <> ConnectionState.Closed Then Return myNativeConnection.Database

                Return Nothing

            End Get
        End Property

        '' <summary>
        ''' Gets the native database version 
        ''' </summary>
        ''' <value>The type.</value>
        Public Overrides ReadOnly Property NativeDatabaseVersion As String Implements iormRelationalDatabaseDriver.NativeDatabaseVersion
            Get
                Dim myNativeConnection As OleDb.OleDbConnection = TryCast(NativeConnection, OleDb.OleDbConnection)

                If myNativeConnection Is Nothing Then
                    Call CoreMessageHandler(procedure:="oleDBDriver.NativeDatabaseVersion", message:="No current Connection to the Database", _
                                          messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If

                If myNativeConnection.State <> ConnectionState.Closed Then Return myNativeConnection.ServerVersion

                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' create a new TableStore for this Driver
        ''' </summary>
        ''' <param name="TableID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function CreateNativeTableStore(ByVal TableID As String, ByVal forceSchemaReload As Boolean) As iormRelationalTableStore
            Return New oledbTableStore(Me.CurrentConnection, TableID, forceSchemaReload)
        End Function
        ''' <summary>
        ''' create a new TableSchema for this Driver
        ''' </summary>
        ''' <param name="TableID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function CreateNativeTableSchema(ByVal TableID As String) As iormContainerSchema
            Return New oledbTableSchema(Me.CurrentConnection, TableID)
        End Function
        ''' create a new TableStore for this Driver
        ''' </summary>
        ''' <param name="TableID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function CreateNativeViewReader(ByVal viewID As String, ByVal forceSchemaReload As Boolean) As iormRelationalTableStore
            Throw New NotImplementedException
        End Function
        ''' <summary>
        ''' create a new TableSchema for this Driver
        ''' </summary>
        ''' <param name="TableID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function CreateNativeViewSchema(ByVal viewID As String) As iormContainerSchema
            Throw New NotImplementedException
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function CreateNativeDBCommand(commandstr As String, nativeConnection As IDbConnection) As IDbCommand Implements iormRelationalDatabaseDriver.CreateNativeDBCommand
            Return New OleDbCommand(commandstr, nativeConnection)
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
        ''' returns the target type for a OTDB FieldType - MAPPING
        ''' </summary>
        ''' <param name="type"></param>
        ''' <remarks></remarks>
        ''' <returns></returns>
        Public Overrides Function GetTargetTypeFor(type As otDataType) As Long Implements iormRelationalDatabaseDriver.GetTargetTypeFor

            Try
                Select Case type
                    Case otDataType.Binary
                        Return OleDbType.Binary
                    Case otDataType.Bool
                        Return OleDbType.Boolean
                    Case otDataType.[Date]
                        Return OleDbType.Date
                    Case otDataType.Time
                        Return OleDbType.DBTime
                    Case otDataType.List
                        Return OleDbType.WChar
                    Case otDataType.[Long]
                        Return OleDbType.Integer
                    Case otDataType.Memo
                        Return OleDbType.WChar
                    Case otDataType.Numeric
                        Return OleDbType.Double
                    Case otDataType.Timestamp
                        Return OleDbType.Date
                    Case otDataType.Text
                        Return OleDbType.WChar
                    Case Else

                        Call CoreMessageHandler(procedure:="oleDBDriver.GetTargetTypefor", message:="Type not defined",
                                       messagetype:=otCoreMessageType.InternalException)
                End Select

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="oleDBDriver.GetTargetTypefor", message:="Exception", exception:=ex, _
                                       messagetype:=otCoreMessageType.InternalException)
                Return 0
            End Try

        End Function
        ''' <summary>
        ''' returns a object from the Data type of the column to Host interpretation
        ''' </summary>
        ''' <param name="index">index as object (name or index 1..n)</param>
        ''' <param name="value">value to convert</param>
        ''' <param name="abostrophNecessary">True if necessary</param>
        ''' <returns>converted value </returns>
        ''' <remarks></remarks>
        Public Overrides Function Convert2ObjectData(ByVal invalue As Object, _
                                                     ByRef outvalue As Object, _
                                                     sourcetype As Long, _
                                                     Optional isnullable As Boolean? = Nothing, _
                                                     Optional defaultvalue As Object = Nothing, _
                                                     Optional ByRef abostrophNecessary As Boolean = False) As Boolean Implements iormRelationalDatabaseDriver.Convert2ObjectData

            Dim result As Object


            Try



                If sourcetype = OleDbType.BigInt OrElse sourcetype = OleDbType.Integer _
                OrElse sourcetype = OleDbType.SmallInt OrElse sourcetype = OleDbType.TinyInt _
                OrElse sourcetype = OleDbType.UnsignedBigInt OrElse sourcetype = OleDbType.UnsignedInt _
                OrElse sourcetype = OleDbType.UnsignedSmallInt OrElse sourcetype = OleDbType.UnsignedTinyInt _
                OrElse sourcetype = OleDbType.SmallInt OrElse sourcetype = OleDbType.TinyInt Then
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
                        Call CoreMessageHandler(procedure:="oleDBDriver.Convert2ObjectData", messagetype:=otCoreMessageType.InternalError, _
                                              message:="OTDB data '" & invalue & "' is not convertible to Integer", _
                                              argument:=sourcetype)
                        Return False
                    End If


                ElseIf sourcetype = OleDbType.Char OrElse sourcetype = OleDbType.BSTR OrElse sourcetype = OleDbType.LongVarChar _
                OrElse sourcetype = OleDbType.LongVarWChar OrElse sourcetype = OleDbType.VarChar OrElse sourcetype = OleDbType.VarWChar _
                OrElse sourcetype = OleDbType.WChar Then
                    abostrophNecessary = True
                    If defaultvalue Is Nothing Then defaultvalue = String.Empty

                    If isnullable AndAlso (invalue Is Nothing OrElse DBNull.Value.Equals(invalue) OrElse _
                                          String.IsNullOrWhiteSpace(invalue)) Then
                        result = Nothing
                    ElseIf Not isnullable AndAlso (invalue Is Nothing OrElse DBNull.Value.Equals(invalue) OrElse _
                                          String.IsNullOrWhiteSpace(invalue)) Then
                        result = Convert.ToString(defaultvalue)
                    Else
                        result = Convert.ToString(invalue)
                    End If

                ElseIf sourcetype = OleDbType.Date OrElse sourcetype = OleDbType.DBDate OrElse sourcetype = OleDbType.DBTime _
                OrElse sourcetype = OleDbType.DBTimeStamp Then
                    If defaultvalue Is Nothing Then defaultvalue = Convert.ToDateTime(ConstNullDate)
                    If isnullable Then
                        result = New Nullable(Of DateTime)
                    Else
                        result = New DateTime
                    End If

                    If isnullable AndAlso (Not IsDate(invalue) OrElse invalue Is Nothing OrElse DBNull.Value.Equals(invalue) _
                                            OrElse String.IsNullOrWhiteSpace(invalue)) Then
                        result = New Nullable(Of DateTime)
                    ElseIf (Not IsDate(invalue) OrElse invalue Is Nothing OrElse DBNull.Value.Equals(invalue) OrElse IsError(invalue)) OrElse String.IsNullOrWhiteSpace(invalue) Then
                        result = Convert.ToDateTime(defaultvalue)
                    ElseIf IsDate(invalue) Then
                        result = Convert.ToDateTime(invalue)
                    Else
                        Call CoreMessageHandler(procedure:="oleDBDriver.Convert2ObjectData", messagetype:=otCoreMessageType.InternalError, _
                                            message:="OTDB data '" & invalue & "' is not convertible to Date", _
                                            argument:=sourcetype)
                        Return False
                    End If

                ElseIf sourcetype = OleDbType.Double OrElse sourcetype = OleDbType.Decimal _
                OrElse sourcetype = OleDbType.Single OrElse sourcetype = OleDbType.Numeric Then
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
                        Call CoreMessageHandler(procedure:="oleDBDriver.Convert2ObjectData", messagetype:=otCoreMessageType.InternalError, _
                                             message:="OTDB data '" & invalue & "' is not convertible to Double", _
                                             argument:=sourcetype)
                        Return False
                    End If


                ElseIf sourcetype = OleDbType.Boolean Then
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
                Call CoreMessageHandler(showmsgbox:=False, procedure:="oledbTableStore.convert2ObjectData", _
                                      argument:=sourcetype, exception:=ex, _
                                      messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End Try

        End Function

        ''' <summary>
        ''' converts data to a specific type
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

                ''' convert an array object to a string
                If IsArray(invalue) Then
                    invalue = Core.DataType.ToString(invalue)
                End If


                If targetType = OleDbType.BigInt OrElse targetType = OleDbType.Integer _
                      OrElse targetType = OleDbType.SmallInt OrElse targetType = OleDbType.TinyInt _
                      OrElse targetType = OleDbType.UnsignedBigInt OrElse targetType = OleDbType.UnsignedInt _
                      OrElse targetType = OleDbType.UnsignedSmallInt OrElse targetType = OleDbType.UnsignedTinyInt _
                      OrElse targetType = OleDbType.SmallInt OrElse targetType = OleDbType.TinyInt Then

                    If defaultvalue Is Nothing Then defaultvalue = 0

                    If isnullable AndAlso (invalue Is Nothing OrElse String.IsNullOrWhiteSpace(invalue.ToString) _
                        OrElse DBNull.Value.Equals(invalue)) Then
                        result = DBNull.Value
                    ElseIf Not isnullable AndAlso (invalue Is Nothing OrElse String.IsNullOrWhiteSpace(invalue.ToString) _
                        OrElse IsError(invalue) OrElse DBNull.Value.Equals(invalue)) Then
                        result = Convert.ToUInt64(defaultvalue)
                    ElseIf IsNumeric(invalue) Then
                        result = Convert.ToUInt64(invalue)
                    Else
                        Call CoreMessageHandler(procedure:="oledbTableStore.cvt2ColumnData", entryname:=columnname, _
                                              message:="OTDB data " & invalue & " is not convertible to Integer", _
                                              argument:=invalue, messagetype:=otCoreMessageType.InternalError)
                        System.Diagnostics.Debug.WriteLine("OTDB data " & invalue & " is not convertible to Integer")
                        outvalue = Nothing
                        Return False

                    End If

                ElseIf targetType = OleDbType.Char OrElse targetType = OleDbType.BSTR OrElse targetType = OleDbType.LongVarChar _
                OrElse targetType = OleDbType.LongVarWChar OrElse targetType = OleDbType.VarChar OrElse targetType = OleDbType.VarWChar _
                OrElse targetType = OleDbType.WChar Then

                    abostrophNecessary = True
                    If defaultvalue Is Nothing Then defaultvalue = String.Empty

                    If isnullable AndAlso (invalue Is Nothing OrElse DBNull.Value.Equals(invalue)) Then
                        result = DBNull.Value

                    ElseIf Not isnullable AndAlso (invalue Is Nothing OrElse String.IsNullOrWhiteSpace(invalue) OrElse _
                                               DBNull.Value.Equals(invalue)) Then
                        result = Convert.ToString(defaultvalue)
                    Else
                        If maxsize < Len(CStr(invalue)) And maxsize > 1 Then
                            result = Mid(Convert.ToString(invalue), 0, maxsize - 1)
                        Else
                            result = Convert.ToString(invalue)
                        End If
                    End If

                ElseIf targetType = OleDbType.Date OrElse targetType = OleDbType.DBDate OrElse targetType = OleDbType.DBTime _
                OrElse targetType = OleDbType.DBTimeStamp Then
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
                        Call CoreMessageHandler(procedure:="oledbTableStore.cvt2ColumnData", entryname:=columnname, _
                                             message:="OTDB data " & invalue & " is not convertible to Date", _
                                             argument:=invalue, messagetype:=otCoreMessageType.InternalError)
                        outvalue = Nothing
                        Return False
                    End If

                ElseIf targetType = OleDbType.Double OrElse targetType = OleDbType.Decimal _
                OrElse targetType = OleDbType.Single OrElse targetType = OleDbType.Numeric Then

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
                        Call CoreMessageHandler(procedure:="oledbTableStore.cvt2ColumnData", entryname:=columnname, _
                                              message:="OTDB data " & invalue & " is not convertible to Double", _
                                              argument:=targetType, messagetype:=otCoreMessageType.InternalError)

                        outvalue = Nothing
                        Return False
                    End If

                ElseIf targetType = OleDbType.Boolean Then

                    If defaultvalue Is Nothing Then defaultvalue = Convert.ToBoolean(False)

                    If isnullable AndAlso (invalue Is Nothing OrElse DBNull.Value.Equals(invalue) OrElse String.IsNullOrWhiteSpace(invalue.ToString) _
                         OrElse (IsNumeric(invalue) AndAlso invalue = 0)) Then
                        result = DBNull.Value
                    ElseIf isnullable AndAlso (invalue Is Nothing OrElse DBNull.Value.Equals(invalue) OrElse String.IsNullOrWhiteSpace(invalue.ToString) _
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
                Call CoreMessageHandler(message:="Exception", procedure:="oledbTableStore.convert2ColumnData(Object, long ..", _
                                       exception:=ex, messagetype:=otCoreMessageType.InternalException)
                Return Nothing
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
                Dim aParameter As New OleDbParameter()

                aParameter.ParameterName = parametername
                aParameter.OleDbType = GetTargetTypeFor(datatype)
                Select Case datatype

                    Case otDataType.Bool
                        aParameter.Value = False
                    Case otDataType.[Date]
                        aParameter.Value = ConstNullDate
                    Case otDataType.Time
                        aParameter.Value = ot.ConstNullTime
                    Case otDataType.List
                        If maxsize = 0 Then aParameter.Size = ConstDBDriverMaxTextSize
                        aParameter.Value = String.Empty
                    Case otDataType.[Long]
                        aParameter.Value = 0
                    Case otDataType.Memo
                        If maxsize = 0 Then aParameter.Size = constDBDriverMaxMemoSize
                        aParameter.Value = String.Empty
                    Case otDataType.Numeric
                        aParameter.Value = 0
                    Case otDataType.Timestamp
                        aParameter.Value = ConstNullDate
                    Case otDataType.Text
                        If maxsize = 0 Then aParameter.Size = ConstDBDriverMaxTextSize
                        aParameter.Value = String.Empty

                End Select
                If Not value Is Nothing Then
                    aParameter.Value = value
                End If
                Return aParameter
            Catch ex As Exception
                Call CoreMessageHandler(procedure:="oleDBDriver.assignDBParameter", message:="Exception", exception:=ex, _
                                       messagetype:=otCoreMessageType.InternalException)
                Return Nothing
            End Try

        End Function

        ''' <summary>
        ''' returns True if the tablename exists in the datastore
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function HasTable(tableid As String, _
                                           Optional ByRef connection As iormConnection = Nothing, _
                                           Optional nativeConnection As Object = Nothing) As Boolean
            Dim myConnection As OnTrack.Database.oledbConnection
            Dim aTable As DataTable
            Dim myNativeConnection As OleDb.OleDbConnection

            '* if already loaded
            If _TableDirectory.ContainsKey(key:=tableid) Then Return True

            If connection Is Nothing Then
                myConnection = _primaryConnection
            Else
                myConnection = connection
            End If

            If nativeConnection Is Nothing Then
                myNativeConnection = TryCast(myConnection.NativeInternalConnection, OleDb.OleDbConnection)
            Else
                myNativeConnection = TryCast(nativeConnection, OleDb.OleDbConnection)
            End If

            If myConnection Is Nothing OrElse myConnection.NativeInternalConnection Is Nothing Then
                Call CoreMessageHandler(procedure:="oleDBDriver.HasTable", message:="No current Connection to the Database", _
                                      messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If
            If myNativeConnection Is Nothing Then
                Call CoreMessageHandler(procedure:="oleDBDriver.HasTable", message:="No current internal Connection to the Database", _
                                      messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If
            '*** check on rights we cannot check on the User Table -> recursion
            '* do not check -> makes no sense since we are checking the database status before we are installing
            'If Not CurrentSession.IsBootstrappingInstallation AndAlso tableid <> User.ConstPrimaryTableID Then
            '    If Not _currentUserValidation.ValidEntry AndAlso Not _currentUserValidation.HasReadRights Then
            '        If Not myConnection.VerifyUserAccess(accessRequest:=otAccessRight.[ReadOnly], loginOnFailed:=True) Then
            '            Call CoreMessageHandler(showmsgbox:=True, subname:="oleDBDriver.HasTable", _
            '                                  message:="No right to read schema of database", messagetype:=otCoreMessageType.ApplicationError)
            '            Return Nothing
            '        End If
            '    End If
            'End If


            Try
                Dim restrictionsTable() As String = {Nothing, Nothing, tableid}
                aTable = myNativeConnection.GetSchema("COLUMNS", restrictionsTable)

                If aTable.Rows.Count = 0 Then
                    Return False
                Else
                    Return True
                End If

            Catch ex As OleDb.OleDbException
                Call CoreMessageHandler(message:="Exception", exception:=ex, containerID:=tableid, _
                                      procedure:="oleDBDriver.hasTable", messagetype:=otCoreMessageType.InternalError)
                Return Nothing

            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception", exception:=ex, containerID:=tableid, _
                                      procedure:="oleDBDriver.hasTable", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End Try

        End Function

        ''' <summary>
        ''' Gets the table.
        ''' </summary>
        ''' <param name="tablename">The tablename.</param>
        ''' <param name="createOnMissing">The create on missing.</param>
        ''' <param name="NativeConnection">The native connection.</param>
        ''' <returns></returns>
        Public Overrides Function GetTable(tableid As String, _
                                           Optional createOrAlter As Boolean = False, _
                                           Optional ByRef connection As iormConnection = Nothing, _
                                            Optional ByRef nativeTableObject As Object = Nothing) As Object

            Dim myConnection As oledbConnection
            Dim aTable As DataTable
            Dim aStatement As String = String.Empty
            Dim nativeTablename As String = GetNativeDBObjectName(tableid)

            If connection Is Nothing Then
                myConnection = _primaryConnection
            Else
                myConnection = connection
            End If

            If myConnection Is Nothing Then
                Call CoreMessageHandler(procedure:="oleDBDriver.GetTable", containerID:=tableid, message:="No current Connection to the Database", _
                                      messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If
            '*** check on rights
            If createOrAlter And Not CurrentSession.IsBootstrappingInstallationRequested Then
                If Not myConnection.VerifyUserAccess(accessRequest:=otAccessRight.AlterSchema, useLoginWindow:=True) Then
                    Call CoreMessageHandler(showmsgbox:=True, procedure:="oleDBDriver.GetTable", containerID:=tableid, _
                                          message:="No right to alter schema of database", messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If
            End If


            Try
                Dim restrictionsTable() As String = {Nothing, Nothing, nativeTablename}
                aTable = DirectCast(myConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection).GetSchema("COLUMNS", restrictionsTable)


                '** create the table
                '**
                If aTable.Rows.Count = 0 And createOrAlter Then

                    aStatement = "CREATE TABLE " & nativeTablename & " ( tttemp  bit )"
                    Me.RunSqlStatement(aStatement, _
                                       nativeConnection:=DirectCast(myConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection))

                    aTable = DirectCast(myConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection).GetSchema("COLUMNS", restrictionsTable)

                    Return aTable

                ElseIf aTable.Rows.Count > 0 Then
                    'Dim columnRow As System.Data.DataRow
                    '** select
                    Dim columnsList = From columnRow In aTable.AsEnumerable _
                                      Select TableName = columnRow.Field(Of String)("TABLE_NAME"), _
                                      [ColumnName] = columnRow.Field(Of String)("COLUMN_NAME") _
                                      Where [ColumnName] = "tttemp"



                    If columnsList.Count > 0 Then
                        Me.RunSqlStatement(sqlcmdstr:="ALTER TABLE [" & nativeTablename & "] DROP [tttemp]")
                    End If
                    Return aTable
                Else
                    Call CoreMessageHandler(procedure:="oleDBDriver.getTable", containerID:=tableid, _
                                          message:="Table was not found in database", messagetype:=otCoreMessageType.ApplicationWarning)
                    Return Nothing
                End If

            Catch ex As OleDb.OleDbException
                Call CoreMessageHandler(message:="Exception", exception:=ex, containerID:=tableid, _
                                      procedure:="oleDBDriver.getTable", messagetype:=otCoreMessageType.InternalError)
                Return Nothing

            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception", exception:=ex, containerID:=tableid, _
                                      procedure:="oleDBDriver.getTable", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End Try

        End Function

        ''' <summary>
        ''' returns a native index object or creates / alters it
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
            Dim aTable As DataTable = TryCast(nativeTable, DataTable)
            Dim myconnection As oledbConnection
            Dim nativeTablename As String = String.Empty
            Dim nativeIndexname As String

            '** no object ?!
            If aTable Is Nothing Then
                Return Nothing
            End If
            If connection Is Nothing Then
                myconnection = _primaryConnection
            Else
                myconnection = connection
            End If

            If myconnection Is Nothing Then
                Call CoreMessageHandler(procedure:="oleDBDriver.GetIndex", argument:=indexdefinition.Name, _
                                      message:="No current Connection to the Database", messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
                '** Schema and User Creation are for free !
            End If
            '*** check on rights
            If createOrAlter And Not CurrentSession.IsBootstrappingInstallationRequested Then
                If Not myconnection.VerifyUserAccess(accessRequest:=otAccessRight.AlterSchema, useLoginWindow:=True) Then
                    Call CoreMessageHandler(showmsgbox:=True, procedure:="oleDBDriver.GetIndex", argument:=indexdefinition.Name, _
                                          message:="No right to alter schema of database", messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If
            End If



            Dim newindexname As String = indexdefinition.NativeIndexname
            Dim aStatement As String = String.Empty
            Dim anIndexTable As DataTable
            Dim existingIndex As Boolean = False
            Dim indexnotchanged As Boolean = False
            Dim existingprimaryName As String = String.Empty
            Dim existingIndexName As String = String.Empty
            Dim isprimaryKey As Boolean = False
            Dim i As UShort = 0

            Try
                '** awkwar get the tableid
                Dim tableidList = From columnRow In aTable.AsEnumerable _
                     Select TableName = columnRow.Field(Of String)("TABLE_NAME"), _
                     Columnordinal = columnRow.Field(Of Int64)("ORDINAL_POSITION"), _
                     DataType = columnRow.Field(Of OleDbType)("DATA_TYPE"), _
                     [ColumnName] = columnRow.Field(Of String)("COLUMN_NAME"), _
                     Description = columnRow.Field(Of String)("DESCRIPTION"), _
                     CharacterMaxLength = columnRow.Field(Of Nullable(Of Int64))("CHARACTER_MAXIMUM_LENGTH"), _
                     IsNullable = columnRow.Field(Of Nullable(Of Boolean))("IS_NULLABLE")

                If tableidList.Count = 0 Then
                    Call CoreMessageHandler(message:="atableid couldn't be retrieved from nativetable object", procedure:="oleDBDriver.getIndex", _
                                                 containerID:=nativeTablename, argument:=indexdefinition.Name, messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                Else
                    nativeTablename = tableidList(0).TableName
                End If
                '** read indixes
                Dim restrictionsIndex() As String = {Nothing, Nothing, Nothing, Nothing, nativeTablename}
                anIndexTable = DirectCast(myconnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection).GetSchema("INDEXES", restrictionsIndex)

                Dim columnsIndexList = From indexRow In anIndexTable.AsEnumerable _
                                        Select TableName = indexRow.Field(Of String)("TABLE_NAME"), _
                                        theIndexName = indexRow.Field(Of String)("INDEX_NAME"), _
                                        Columnordinal = indexRow.Field(Of Int64)("ORDINAL_POSITION"), _
                                        ColumnName = indexRow.Field(Of String)("COLUMN_NAME"), _
                                        IndexisPrimaryKey = indexRow.Field(Of Boolean)("PRIMARY_KEY") _
                                        Where [ColumnName] <> String.Empty And TableName <> String.Empty And Columnordinal > 0 And (theIndexName = newindexname Or theIndexName = nativeTablename & "_" & newindexname) _
                                        Order By TableName, newindexname, Columnordinal, ColumnName

                Dim primaryIndexList = From indexRow In anIndexTable.AsEnumerable _
                                        Select TableName = indexRow.Field(Of String)("TABLE_NAME"), _
                                        theIndexName = indexRow.Field(Of String)("INDEX_NAME"), _
                                        Columnordinal = indexRow.Field(Of Int64)("ORDINAL_POSITION"), _
                                        ColumnName = indexRow.Field(Of String)("COLUMN_NAME"), _
                                        IndexisPrimaryKey = indexRow.Field(Of Boolean)("PRIMARY_KEY") _
                                        Where [ColumnName] <> String.Empty And TableName <> String.Empty And Columnordinal > 0 And IndexisPrimaryKey = True _
                                        Order By TableName, newindexname, Columnordinal, ColumnName

                If primaryIndexList.Count > 0 Then
                    existingprimaryName = primaryIndexList(0).theIndexName
                End If

                If columnsIndexList.Count = 0 And Not createOrAlter Then
                    Return Nothing
                ElseIf columnsIndexList.Count = 0 And createOrAlter Then
                    existingIndex = False
                    indexnotchanged = False
                ElseIf Not forceCreation Then
                    i = 0
                    ' get an list
                    Dim anIndexColumnsList As New List(Of String)
                    For Each anIndex In columnsIndexList
                        anIndexColumnsList.Add(anIndex.ColumnName)
                        If anIndex.IndexisPrimaryKey Then
                            isprimaryKey = True
                        End If
                        existingIndexName = anIndex.theIndexName
                    Next
                    ' go through
                    If anIndexColumnsList.Count = indexdefinition.Columnnames.Count Then
                        For Each columnName As String In indexdefinition.Columnnames
                            If LCase(anIndexColumnsList.Item(i)) <> LCase(columnName) Then
                                indexnotchanged = False
                                Exit For
                            Else
                                indexnotchanged = True
                            End If

                            ' exit
                            If Not indexnotchanged Then
                                Exit For
                            End If
                            i = i + 1
                        Next columnName
                    Else
                        indexnotchanged = False ' different number of columnnames
                    End If
                    '** check if primary is different
                    If indexdefinition.IsPrimary <> isprimaryKey Or forceCreation Then
                        indexnotchanged = False
                    End If
                    ' return
                    If indexnotchanged Then
                        Return columnsIndexList
                    End If
                End If


                '** drop existing

                If (isprimaryKey Or indexdefinition.IsPrimary) And existingprimaryName <> String.Empty Then
                    aStatement = " ALTER TABLE " & nativeTablename & " DROP CONSTRAINT [" & existingprimaryName & "]"
                    Me.RunSqlStatement(aStatement)
                ElseIf existingIndex Then
                    aStatement = " DROP INDEX " & existingIndex
                    Me.RunSqlStatement(aStatement)
                End If

                ''' buiöd
                If String.IsNullOrWhiteSpace(newindexname) Then
                    newindexname = nativeTablename & "_" & GetNativeIndexname(indexdefinition.Name)
                    indexdefinition.NativeIndexname = newindexname
                End If

                '*** build new
                If indexdefinition.IsPrimary Then
                    aStatement = " ALTER TABLE [" & nativeTablename & "] ADD CONSTRAINT [" & indexdefinition.NativeIndexname & "] PRIMARY KEY ("
                    Dim comma As Boolean = False
                    For Each name As String In indexdefinition.Columnnames
                        If comma Then aStatement &= ","
                        aStatement &= "[" & name & "]"
                        comma = True
                    Next
                    aStatement &= ")"
                    Me.RunSqlStatement(aStatement)
                Else
                    Dim UniqueStr As String = String.Empty
                    If indexdefinition.IsUnique Then UniqueStr = "UNIQUE"
                    aStatement = " CREATE " & UniqueStr & " INDEX [" & indexdefinition.NativeIndexname & "] ON [" & nativeTablename & "] ("
                    Dim comma As Boolean = False
                    For Each name As String In indexdefinition.Columnnames
                        If comma Then aStatement &= ","
                        aStatement &= "[" & name & "]"
                        comma = True
                    Next
                    aStatement &= ")"
                    Me.RunSqlStatement(aStatement)
                End If

                '** read indixes

                anIndexTable = DirectCast(myconnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection).GetSchema("INDEXES", restrictionsIndex)

                Dim columnsResultIndexList = From indexRow In anIndexTable.AsEnumerable Select TableName = indexRow.Field(Of String)("TABLE_NAME"), _
                                             theIndexName = indexRow.Field(Of String)("INDEX_NAME"), _
                                             Columnordinal = indexRow.Field(Of Int64)("ORDINAL_POSITION"), _
                                             ColumnName = indexRow.Field(Of String)("COLUMN_NAME"), _
                                            IndexisPrimaryKey = indexRow.Field(Of Boolean)("PRIMARY_KEY") _
                                            Where [ColumnName] <> String.Empty And TableName <> String.Empty And Columnordinal > 0 And (theIndexName = newindexname) _
                                            Order By TableName, newindexname, Columnordinal, ColumnName

                If columnsResultIndexList.Count > 0 Then
                    Return columnsResultIndexList
                Else
                    Call CoreMessageHandler(message:="creation of index failed", argument:=indexdefinition.Name, _
                                                 procedure:="oleDBDriver.getIndex", containerID:=nativeTablename, _
                                                 messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If

            Catch ex As OleDb.OleDbException
                Call CoreMessageHandler(showmsgbox:=True, procedure:="oleDBDriver.GetIndex", argument:=aStatement, containerID:=nativeTablename, _
                                           message:="Exception", exception:=ex, messagetype:=otCoreMessageType.InternalError)
                myconnection.IsNativeInternalLocked = False
                Return Nothing

            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=True, procedure:="oleDBDriver.GetIndex", argument:=indexdefinition.Name, containerID:=nativeTablename, _
                                           message:="Exception", exception:=ex, messagetype:=otCoreMessageType.InternalError)
                myconnection.IsNativeInternalLocked = False
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' returns True if the table id has the Column
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="columnname"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Public Overrides Function VerifyColumnSchema(columndefinition As iormContainerEntryDefinition, Optional ByRef connection As iormConnection = Nothing, Optional silent As Boolean = False) As Boolean
        '    Dim myConnection As oledbConnection
        '    Dim aTable As DataTable
        '    Dim tableid As String = columndefinition.ContainerID
        '    Dim nativetablename As String = GetNativeDBObjectName(tableid)
        '    Dim columnname As String = columndefinition.EntryName

        '    If connection Is Nothing Then
        '        myConnection = _primaryConnection
        '    Else
        '        myConnection = connection
        '    End If

        '    '** do not session since we might checking this to get bootstrapping status before session is started
        '    If Not CurrentSession.IsBootstrappingInstallationRequested AndAlso Not myConnection.VerifyUserAccess(accessRequest:=otAccessRight.[ReadOnly], useLoginWindow:=True) Then
        '        Call CoreMessageHandler(showmsgbox:=True, procedure:="oleDBDriver.verifyColumnSchema", containerID:=tableid, _
        '                              message:="No right to read schema of database", messagetype:=otCoreMessageType.ApplicationError)
        '        Return Nothing
        '    End If
        '    Try
        '        Dim restrictionsTable() As String = {Nothing, Nothing, nativetablename}
        '        aTable = DirectCast(myConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection).GetSchema("COLUMNS", restrictionsTable)


        '        '** select
        '        Dim columnsResultList = From columnRow In aTable.AsEnumerable _
        '                       Select TableName = columnRow.Field(Of String)("TABLE_NAME"), _
        '                       Columnordinal = columnRow.Field(Of Int64)("ORDINAL_POSITION"), _
        '                       DataType = columnRow.Field(Of OleDbType)("DATA_TYPE"), _
        '                       [name] = columnRow.Field(Of String)("COLUMN_NAME"), _
        '                       Description = columnRow.Field(Of String)("DESCRIPTION"), _
        '                       CharacterMaxLength = columnRow.Field(Of Nullable(Of Int64))("CHARACTER_MAXIMUM_LENGTH"), _
        '                       IsNullable = columnRow.Field(Of Nullable(Of Boolean))("IS_NULLABLE") _
        '                       Where [name] = columndefinition.EntryName

        '        If columnsResultList.Count = 0 Then
        '            If Not silent Then
        '                CoreMessageHandler(message:="verifying table column: column does not exist in database ", _
        '                                              containerID:=tableid, containerEntryName:=columnname, procedure:="oledbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)

        '            End If
        '            Return False
        '        Else
        '            '** what to check
        '            For Each column In columnsResultList
        '                '** check on datatype
        '                If column.DataType <> GetTargetTypeFor(columndefinition.Datatype) Then
        '                    If Not silent Then
        '                        CoreMessageHandler(message:="verifying table column: column data type in database differs from column definition", argument:=columndefinition.Datatype, _
        '                                                containerID:=tableid, containerEntryName:=columnname, procedure:="oledbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
        '                    End If
        '                    Return False
        '                End If
        '                '** check on size
        '                If column.DataType = OleDbType.VarChar OrElse column.DataType = OleDbType.LongVarChar OrElse _
        '                    column.DataType = OleDbType.LongVarWChar OrElse column.DataType = OleDbType.VarWChar Then
        '                    If columndefinition.Size > column.CharacterMaxLength Then
        '                        If Not silent Then
        '                            CoreMessageHandler(message:="verifying table column: column size in database differs from column definition", argument:=columndefinition.Size, _
        '                                                containerID:=tableid, containerEntryName:=columnname, procedure:="oledbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
        '                        End If
        '                        Return False
        '                    End If
        '                End If

        '            Next
        '            Return True
        '        End If

        '    Catch ex As OleDb.OleDbException
        '        Call CoreMessageHandler(message:="Exception", exception:=ex, containerID:=tableid, _
        '                              procedure:="oleDBDriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
        '        Return Nothing

        '    Catch ex As Exception
        '        Call CoreMessageHandler(message:="Exception", exception:=ex, containerID:=tableid, _
        '                              procedure:="oleDBDriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
        '        Return Nothing
        '    End Try


        'End Function
        ''' <summary>
        ''' returns True if the table id has the Column
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="columnname"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function VerifyColumnSchema(columnDefinition As iormContainerEntryDefinition, Optional ByRef connection As iormConnection = Nothing, Optional silent As Boolean = False) As Boolean
            Dim myConnection As oledbConnection
            Dim aTable As DataTable
            Dim tableid As String = columnDefinition.ContainerID
            Dim nativetablename As String = GetNativeDBObjectName(tableid)
            Dim columnname As String = columnDefinition.EntryName

            If connection Is Nothing Then
                myConnection = _primaryConnection
            Else
                myConnection = connection
            End If

            '** do not session since we might checking this to get bootstrapping status before session is started
            If Not CurrentSession.IsBootstrappingInstallationRequested AndAlso Not myConnection.VerifyUserAccess(accessRequest:=otAccessRight.[ReadOnly], useLoginWindow:=True) Then
                Call CoreMessageHandler(showmsgbox:=True, procedure:="oleDBDriver.HasTable", containerID:=tableid, _
                                                 message:="No right to read schema of database", messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If
            Try
                Dim restrictionsTable() As String = {Nothing, Nothing, nativetablename}
                aTable = DirectCast(myConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection).GetSchema("COLUMNS", restrictionsTable)


                '** select
                Dim columnsResultList = From columnRow In aTable.AsEnumerable _
                               Select TableName = columnRow.Field(Of String)("TABLE_NAME"), _
                               Columnordinal = columnRow.Field(Of Int64)("ORDINAL_POSITION"), _
                               DataType = columnRow.Field(Of OleDbType)("DATA_TYPE"), _
                               [name] = columnRow.Field(Of String)("COLUMN_NAME"), _
                               Description = columnRow.Field(Of String)("DESCRIPTION"), _
                               CharacterMaxLength = columnRow.Field(Of Nullable(Of Int64))("CHARACTER_MAXIMUM_LENGTH"), _
                               IsNullable = columnRow.Field(Of Nullable(Of Boolean))("IS_NULLABLE") _
                               Where [name] = columnname

                If columnsResultList.Count = 0 Then
                    If Not silent Then
                        CoreMessageHandler(message:="verifying table column: column doesnot exist in database ", _
                                                      containerID:=tableid, containerEntryName:=columnname, procedure:="oledbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)
                    End If
                    Return False
                Else
                    '** what to check
                    For Each column In columnsResultList
                        '** check on datatype
                        If columnDefinition.DataType >= 0 AndAlso column.DataType <> GetTargetTypeFor(columnDefinition.DataType) Then
                            If Not silent Then
                                CoreMessageHandler(message:="verifying table column: column data type in database differs from column attribute", argument:=columnDefinition.DataType, _
                                                      containerID:=tableid, containerEntryName:=columnname, procedure:="oledbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)

                            End If
                            Return False
                        End If
                        '** check on size
                        If column.DataType = OleDbType.VarChar OrElse column.DataType = OleDbType.LongVarChar OrElse _
                            column.DataType = OleDbType.LongVarWChar OrElse column.DataType = OleDbType.VarWChar Then
                            If columnDefinition.Size > column.CharacterMaxLength Then
                                If Not silent Then
                                    CoreMessageHandler(message:="verifying table column: column size in database differs from column attribute", argument:=columnDefinition.Size, _
                                                       containerID:=tableid, containerEntryName:=columnname, procedure:="oledbdriver.verifyColumnSchema", messagetype:=otCoreMessageType.InternalError)

                                End If

                                Return False
                            End If
                        End If

                    Next
                    Return True
                End If

            Catch ex As OleDb.OleDbException
                Call CoreMessageHandler(message:="Exception", exception:=ex, containerID:=tableid, _
                                      procedure:="oleDBDriver.hasColumn", messagetype:=otCoreMessageType.InternalError)
                Return Nothing

            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception", exception:=ex, containerID:=tableid, _
                                      procedure:="oleDBDriver.hasColumn", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End Try


        End Function
        ''' <summary>
        ''' returns True if the table id has the Column
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="columnname"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function HasColumn(tableid As String, columnname As String, Optional ByRef connection As iormConnection = Nothing) As Boolean
            Dim myConnection As oledbConnection
            Dim aTable As DataTable
            Dim nativetablename As String = GetNativeDBObjectName(tableid)

            If connection Is Nothing Then
                myConnection = _primaryConnection
            Else
                myConnection = connection
            End If

            '* doesnot make any sense
            'If Not myConnection.VerifyUserAccess(otAccessRight.[ReadOnly], loginOnFailed:=True) And Not CurrentSession.IsBootstrappingInstallation Then
            '    Call CoreMessageHandler(showmsgbox:=True, subname:="oleDBDriver.HasTable", tablename:=tableid, _
            '                          message:="No right to read schema of database", messagetype:=otCoreMessageType.ApplicationError)
            '    Return Nothing
            'End If

            Try
                Dim restrictionsTable() As String = {Nothing, Nothing, nativetablename}
                aTable = DirectCast(myConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection).GetSchema("COLUMNS", restrictionsTable)

                '** select
                Dim columnsResultList = From columnRow In aTable.AsEnumerable _
                                       Select TableName = columnRow.Field(Of String)("TABLE_NAME"), _
                                       cname = columnRow.Field(Of String)("COLUMN_NAME") _
                                       Where cname = columnname

                If columnsResultList.Count = 0 Then
                    Return False
                Else
                    Return True
                End If

            Catch ex As OleDb.OleDbException
                Call CoreMessageHandler(message:="Exception", exception:=ex, containerID:=tableid, _
                                      procedure:="oleDBDriver.hasColumn", messagetype:=otCoreMessageType.InternalError)
                Return Nothing

            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception", exception:=ex, containerID:=tableid, _
                                      procedure:="oleDBDriver.hasColumn", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End Try


        End Function
        ''' <summary>
        ''' Gets the column.
        ''' </summary>
        ''' <param name="nativeTABLE">The native TABLE.</param>
        ''' <param name="aDBDesc">A DB desc.</param>
        ''' <param name="createOnMissing">The create on missing.</param>
        ''' <param name="addToSchemaDir">The add to schema dir.</param>
        ''' <returns></returns>
        Public Overrides Function GetColumn(nativeTable As Object, columndefinition As iormContainerEntryDefinition, _
                                            Optional createOrAlter As Boolean = False, _
                                            Optional ByRef connection As iormConnection = Nothing) As Object Implements iormRelationalDatabaseDriver.GetColumn

            Dim aTable As DataTable = TryCast(nativeTable, DataTable)
            Dim nativeTablename As String = String.Empty
            Dim myConnection As oledbConnection
            Dim aStatement As String = String.Empty

            '** no object ?!
            If aTable Is Nothing Then
                Call CoreMessageHandler(procedure:="oleDBDriver.GetColumn", message:="native table parameter to function is nothing",
                                        messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If

            If connection Is Nothing Then
                myConnection = _primaryConnection
            Else
                myConnection = connection
            End If

            If myConnection Is Nothing Then
                Call CoreMessageHandler(procedure:="oleDBDriver.GetColumn", message:="No current Connection to the Database", _
                                      messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If
            '*** check on rights
            If createOrAlter And Not CurrentSession.IsBootstrappingInstallationRequested Then
                If Not myConnection.VerifyUserAccess(accessRequest:=otAccessRight.AlterSchema, useLoginWindow:=True) Then
                    Call CoreMessageHandler(showmsgbox:=True, procedure:="oleDBDriver.GetColumn", _
                                          message:="No right to alter schema of database", messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If
            End If


            Try
                '** select
                Dim columnsList = From columnRow In aTable.AsEnumerable _
                               Select TableName = columnRow.Field(Of String)("TABLE_NAME"), _
                               Columnordinal = columnRow.Field(Of Int64)("ORDINAL_POSITION"), _
                               DataType = columnRow.Field(Of OleDbType)("DATA_TYPE"), _
                               [name] = columnRow.Field(Of String)("COLUMN_NAME"), _
                               Description = columnRow.Field(Of String)("DESCRIPTION"), _
                               CharacterMaxLength = columnRow.Field(Of Nullable(Of Int64))("CHARACTER_MAXIMUM_LENGTH"), _
                               IsNullable = columnRow.Field(Of Nullable(Of Boolean))("IS_NULLABLE") _
                               Where [name] = columndefinition.EntryName

                If columnsList.Count > 0 And Not createOrAlter Then
                    Return columnsList
                Else

                    '** create the column
                    '**

                    Dim tableidList = From columnRow In aTable.AsEnumerable _
                                          Select TableName = columnRow.Field(Of String)("TABLE_NAME"), _
                                          Columnordinal = columnRow.Field(Of Int64)("ORDINAL_POSITION"), _
                                          DataType = columnRow.Field(Of OleDbType)("DATA_TYPE"), _
                                          [name] = columnRow.Field(Of String)("COLUMN_NAME"), _
                                          Description = columnRow.Field(Of String)("DESCRIPTION"), _
                                          CharacterMaxLength = columnRow.Field(Of Nullable(Of Int64))("CHARACTER_MAXIMUM_LENGTH"), _
                                          IsNullable = columnRow.Field(Of Nullable(Of Boolean))("IS_NULLABLE")

                    If tableidList.Count = 0 Then
                        Call CoreMessageHandler(message:="atableid couldn't be retrieved from nativetable object", procedure:="oleDBDriver.getColumn", _
                                                     containerID:=nativeTablename, entryname:=columndefinition.EntryName, messagetype:=otCoreMessageType.InternalError)
                        Return Nothing
                    Else
                        nativeTablename = tableidList(0).TableName
                    End If

                    aStatement = "ALTER TABLE " & nativeTablename
                    If columnsList.Count = 0 Then
                        aStatement &= " ADD COLUMN "
                    ElseIf Me.NativeDatabaseName = ConstCPNAccessName Then
                        aStatement &= " ALTER COLUMN "
                    Else
                        aStatement &= " MODIFY COLUMN "
                    End If
                    aStatement &= "[" & columndefinition.EntryName & "] "

                    Select Case columndefinition.Datatype
                        Case otDataType.Bool
                            aStatement &= " BIT "
                        Case otDataType.Binary
                            aStatement &= " BINARY VARYING"
                        Case otDataType.Date
                            aStatement &= " DATE "
                        Case otDataType.Long
                            If Me.NativeDatabaseName = ConstCPNAccessName Then
                                aStatement &= " INTEGER "
                            Else
                                aStatement &= " BIG INT "
                            End If

                        Case otDataType.Memo
                            If Me.NativeDatabaseName = ConstCPNAccessName Then
                                aStatement &= " MEMO "
                            Else
                                aStatement &= " NVARCHAR(" & constDBDriverMaxMemoSize.ToString & ")"
                            End If
                        Case otDataType.Numeric
                            aStatement &= " FLOAT "
                        Case otDataType.Text, otDataType.List
                            aStatement &= " NVARCHAR("
                            If Not columndefinition.Size.HasValue Then
                                aStatement &= ConstDBDriverMaxTextSize.ToString
                                aStatement &= ")"
                            ElseIf columndefinition.Size <= 255 Then
                                aStatement &= columndefinition.Size.ToString
                                aStatement &= ")"
                            ElseIf Me.NativeDatabaseName = ConstCPNAccessName And columndefinition.Size > 255 Then
                                aStatement &= " MEMO "
                            End If

                        Case otDataType.Timestamp
                            aStatement &= " TIMESTAMP "
                        Case otDataType.Time
                            aStatement &= " TIME "
                        Case Else
                            Call CoreMessageHandler(message:="Datatype is not implemented", containerID:=nativeTablename, entryname:=columndefinition.EntryName, _
                                                         procedure:="oleDBDriver.getColumn", argument:=columndefinition.Datatype.ToString, _
                                                         messagetype:=otCoreMessageType.InternalError)
                            Return Nothing
                    End Select

                    If columndefinition.IsNullable Then
                        aStatement &= " NULL "
                    Else
                        aStatement &= " NOT NULL "
                    End If

                    If columndefinition.DBDefaultValue IsNot Nothing Then
                        '** to be implemented
                        '     aStatement &= " DEFAULT '" & columndefinition.DefaultValueString & "'" not working mus be differentiate to string sql presenttion of data
                    End If
                    '** Run it
                    Me.RunSqlStatement(aStatement, _
                                       nativeConnection:=DirectCast(myConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection))

                    '** add uniqueness
                    If columndefinition.IsUnique Then
                        aStatement = "ALTER TABLE [" & nativeTablename & "] ADD CONSTRAINT " & "C_" & nativeTablename & "_" & columndefinition.EntryName & " UNIQUE (" & columndefinition.EntryName & ")"
                        '** Run it
                        Me.RunSqlStatement(aStatement, _
                                           nativeConnection:=DirectCast(myConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection))
                    End If

                    '** get the result
                    Dim restrictionsTable() As String = {Nothing, Nothing, nativeTablename}
                    aTable = DirectCast(myConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection).GetSchema("COLUMNS", restrictionsTable)
                    '** select
                    Dim columnsResultList = From columnRow In aTable.AsEnumerable _
                                           Select TableName = columnRow.Field(Of String)("TABLE_NAME"), _
                                           Columnordinal = columnRow.Field(Of Int64)("ORDINAL_POSITION"), _
                                           DataType = columnRow.Field(Of OleDbType)("DATA_TYPE"), _
                                           [name] = columnRow.Field(Of String)("COLUMN_NAME"), _
                                           Description = columnRow.Field(Of String)("DESCRIPTION"), _
                                           CharacterMaxLength = columnRow.Field(Of Nullable(Of Int64))("CHARACTER_MAXIMUM_LENGTH"), _
                                           IsNullable = columnRow.Field(Of Nullable(Of Boolean))("IS_NULLABLE") _
                                           Where [name] = columndefinition.EntryName



                    If columnsResultList.Count > 0 Then
                        Return columnsResultList
                    Else
                        Call CoreMessageHandler(message:="Add Column failed", procedure:="oleDBDriver", _
                                                    containerID:=nativeTablename, entryname:=columndefinition.EntryName, _
                                                    messagetype:=otCoreMessageType.InternalError)
                        Return Nothing
                    End If


                End If

            Catch ex As OleDb.OleDbException
                Call CoreMessageHandler(message:="Exception", exception:=ex, argument:=aStatement, containerID:=nativeTablename, entryname:=columndefinition.EntryName, _
                                     procedure:="oleDBDriver.getColumn", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception", exception:=ex, containerID:=nativeTablename, entryname:=columndefinition.EntryName, _
                                      procedure:="oleDBDriver.getColumn", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
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
            Dim dataRows() As DataRow
            Dim insertFlag As Boolean = False

            '*** get the native Connection 
            If nativeConnection Is Nothing Then
                If Not Me.CurrentConnection Is Nothing Then
                    nativeConnection = DirectCast(_primaryConnection, OnTrack.Database.oledbConnection).NativeInternalConnection
                    If nativeConnection Is Nothing Then
                        Call CoreMessageHandler(procedure:="oleDBDriver.DeleteDBParameter", _
                                              message:="Native Internal Connection not available")
                        Return False
                    End If
                Else
                    Call CoreMessageHandler(procedure:="oleDBDriver.DeleteDBParameter", _
                                          message:="Connection not available")
                    Return False
                End If

            End If

            '** init driver
            If Not Me.IsInitialized Then Me.Initialize()

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
                                             Me.DBParameterTablename, ormRDBDriver.ConstFNSetupID, CurrentSetupID, ormRDBDriver.ConstFNID, parametername)

                    If Me.RunSqlStatement(aDeleteText.ToString) Then
                        CoreMessageHandler(message:="IN SETUP >" & CurrentSetupID & "< DROPPED FROM OTDB PARAMETER TABLE  " & CurrentOTDBDriver.DBParameterContainerName, containerID:=CurrentOTDBDriver.DBParameterContainerName, _
                                              messagetype:=otCoreMessageType.ApplicationInfo, procedure:="adonetDBDriver.DropTable")
                    End If
                Else

                    '*** to the table
                    Dim aSelectStr As String = String.Format("[{0}]='{1}' AND [{2}]='{3}'", _
                                                                 ConstFNSetupID, setupID, ConstFNID, parametername)
                    dataRows = _ParametersTable.Select(aSelectStr)

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

                End If

            Catch ex As Exception
                ' Handle the error

                Call CoreMessageHandler(showmsgbox:=silent, procedure:="oleDBDriver.DeleteDBParameter", _
                                      containerID:=_parametersTableName, entryname:=parametername)
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
        Public Overrides Function SetDBParameter(parametername As String, _
                                                value As Object, _
                                                Optional ByRef nativeConnection As Object = Nothing, _
                                                Optional updateOnly As Boolean = False, _
                                                Optional silent As Boolean = False, _
                                                Optional SetupID As String = Nothing, _
                                                Optional description As String = Nothing) As Boolean

            Dim dataRows() As DataRow
            Dim insertFlag As Boolean = False

            '*** get the native Connection 
            If nativeConnection Is Nothing Then
                If Not Me.CurrentConnection Is Nothing Then
                    nativeConnection = DirectCast(_primaryConnection, OnTrack.Database.oledbConnection).NativeInternalConnection
                    If nativeConnection Is Nothing Then
                        Call CoreMessageHandler(procedure:="oleDBDriver.setDBParameter", _
                                              message:="Native Internal Connection not available")
                        Return False
                    End If
                Else
                    Call CoreMessageHandler(procedure:="oleDBDriver.setDBParameter", _
                                          message:="Connection not available")
                    Return False
                End If

            End If

            '** init driver
            If Not Me.IsInitialized Then Me.Initialize()

            If SetupID Is Nothing Then SetupID = Me.Session.CurrentSetupID

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
                    '** diretc in the table
                    Dim aSelectStr As String = String.Format("[{0}]='{1}' AND [{2}]='{3}'", ConstFNSetupID, SetupID, ConstFNID, parametername)
                    dataRows = _ParametersTable.Select(aSelectStr)

                    ' not found
                    If dataRows.GetLength(0) = 0 Then
                        If updateOnly And silent Then
                            SetDBParameter = False
                            Exit Function
                        ElseIf updateOnly And Not silent Then
                            Call CoreMessageHandler(showmsgbox:=True, _
                                                  message:="The Parameter '" & parametername & "' was not found in the Table " & ConstDBParameterTableName, procedure:="oleDBDriver.setdbparameter", messagetype:=otCoreMessageType.ApplicationError)
                            Return False
                        ElseIf Not updateOnly Then
                            ReDim dataRows(0)
                            dataRows(0) = _ParametersTable.NewRow
                            dataRows(0)(constFNDescription) = String.Empty

                            insertFlag = True
                        End If
                    End If

                    ' value
                    'dataRows(0).BeginEdit()
                    dataRows(0)(ConstFNSetupID) = SetupID.Trim
                    dataRows(0)(ConstFNID) = parametername.Trim
                    dataRows(0)(ConstFNValue) = CStr(value).Trim
                    dataRows(0)(ConstFNChangedOn) = Date.Now().ToString.Trim
                    dataRows(0)(constFNDescription) = description.Trim
                    'dataRows(0).EndEdit()

                    '* add to table
                    If insertFlag Then
                        _ParametersTable.Rows.Add(dataRows(0))
                    End If

                    '*
                    Dim i = _ParametersTableAdapter.Update(_ParametersTable)
                    If i > 0 Then
                        _ParametersTable.AcceptChanges()
                        Return True
                    Else
                        Return False
                    End If
                End If

            Catch ex As Exception
                ' Handle the error

                Call CoreMessageHandler(showmsgbox:=silent, procedure:="oleDBDriver.setDBParameter", _
                                      containerID:=_parametersTableName, entryname:=parametername)
                SetDBParameter = False
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

            Dim dataRows() As DataRow

            '*** get the native Connection 
            If nativeConnection Is Nothing Then
                If Not Me.CurrentConnection Is Nothing Then
                    nativeConnection = _primaryConnection.NativeInternalConnection
                    If nativeConnection Is Nothing Then
                        Call CoreMessageHandler(procedure:="oleDBDriver.getDBParameter", message:="Native internal Connection not available")
                        Return Nothing
                    End If
                Else
                    Call CoreMessageHandler(procedure:="oleDBDriver.getDBParameter", message:="Connection not available")
                    Return Nothing
                End If
            End If

            Try
                '** init driver
                If Not Me.IsInitialized Then Me.Initialize()
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
                    '** select row
                    Dim aSelectStr As String = String.Format("[{0}]='{1}' AND [{2}]='{3}'", ConstFNSetupID, SetupID, ConstFNID, parametername)
                    dataRows = _ParametersTable.Select(aSelectStr)
                    ' not found
                    If dataRows.GetLength(0) = 0 Then
                        If silent Then
                            Return Nothing
                        ElseIf Not silent Then
                            Call CoreMessageHandler(showmsgbox:=True, _
                                                  message:="The Parameter '" & parametername & "' was not found in the OTDB Table " & ConstDBParameterTableName, procedure:="oleDBDriver.setdbparameter", messagetype:=otCoreMessageType.ApplicationError)
                            Return Nothing

                        End If
                    End If

                    ' value
                    Return dataRows(0)(ConstFNValue)
                End If


                ' Handle the error
            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=silent, procedure:="oleDBDriver.getDBParameter", containerID:=ConstDBParameterTableName, _
                                      exception:=ex, entryname:=parametername)
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
        Public Overrides Function RunSqlStatement(ByVal sqlcmdstr As String, Optional ByRef parameters As List(Of ormSqlCommandParameter) = Nothing, _
                                                  Optional silent As Boolean = True, Optional nativeConnection As Object = Nothing) As Boolean _
        Implements iormRelationalDatabaseDriver.RunSqlStatement
            Dim anativeConnection As System.Data.OleDb.OleDbConnection
            '*** get the native Connection 
            If nativeConnection Is Nothing Then
                If Not Me.CurrentConnection Is Nothing Then
                    anativeConnection = DirectCast(Me.CurrentConnection, adonetConnection).NativeInternalConnection
                    If anativeConnection Is Nothing Then
                        Call CoreMessageHandler(procedure:="oleDBDriver.runSQLCommand", message:="Native internal Connection not available")
                        Return Nothing
                    End If
                Else
                    Call CoreMessageHandler(procedure:="oleDBDriver.runSQLCommand", message:="Connection not available")
                    Return Nothing
                End If
            Else
                anativeConnection = nativeConnection
            End If
            Try
                Dim aSQLCommand As New OleDbCommand
                aSQLCommand.Connection = anativeConnection
                aSQLCommand.CommandText = sqlcmdstr

                If aSQLCommand.ExecuteNonQuery() > 0 Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                Call CoreMessageHandler(procedure:="oleDBDriver.runSQLCommand", exception:=ex, argument:=sqlcmdstr)
                Return False
            End Try

        End Function
        ''' returns or creates a view in the data store
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="sqlselect"></param>
        ''' <param name="createOrAlter"></param>
        ''' <param name="connection"></param>
        ''' <remarks></remarks>
        ''' <returns></returns>
        Public Overrides Function GetView(viewid As String, Optional sqlselect As String = Nothing, Optional createOrAlter As Boolean = False, Optional ByRef connection As iormConnection = Nothing) As Object

            Dim myConnection As oledbConnection
            Dim theViews As DataTable
            Dim nativeViewname As String = GetNativeViewname(viewid)
            Dim aStatement As String = String.Empty

            If connection Is Nothing Then
                myConnection = _primaryConnection
            Else
                myConnection = connection
            End If

            If myConnection Is Nothing Then
                Call CoreMessageHandler(procedure:="oleDBDriver.GetView", argument:=nativeViewname, message:="No current Connection to the Database", _
                                      messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If
            '*** check on rights
            If createOrAlter And Not CurrentSession.IsBootstrappingInstallationRequested Then
                If Not myConnection.VerifyUserAccess(accessRequest:=otAccessRight.AlterSchema, useLoginWindow:=True) Then
                    Call CoreMessageHandler(showmsgbox:=True, procedure:="oleDBDriver.GetView", argument:=nativeViewname, _
                                          message:="No right to alter schema of database", messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If
            End If


            Try
                Dim restrictionsTable() As String = {Nothing, Nothing, nativeViewname}
                theViews = DirectCast(myConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection).GetSchema("VIEWS", restrictionsTable)


                '** create the table
                '**
                If theViews.Rows.Count > 0 Then

                    If createOrAlter Then
                        aStatement = "DROP VIEW [" & nativeViewname & "]"
                        Me.RunSqlStatement(aStatement, _
                                           nativeConnection:=DirectCast(myConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection))

                    Else
                        Return theViews
                    End If


                End If

                If theViews.Rows.Count = 0 And createOrAlter Then

                    aStatement = "CREATE VIEW [" & nativeViewname & "] AS " & sqlselect
                    Me.RunSqlStatement(aStatement, _
                                       nativeConnection:=DirectCast(myConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection))

                    theViews = DirectCast(myConnection.NativeInternalConnection, System.Data.OleDb.OleDbConnection).GetSchema("COLUMNS", restrictionsTable)

                    Return theViews


                End If

            Catch ex As OleDb.OleDbException
                Call CoreMessageHandler(message:="Exception", exception:=ex, argument:=nativeViewname, _
                                      procedure:="oleDBDriver.GetView", messagetype:=otCoreMessageType.InternalError)
                Return Nothing

            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception", exception:=ex, argument:=nativeViewname, _
                                      procedure:="oleDBDriver.GetView", messagetype:=otCoreMessageType.InternalError)
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
        Public Overrides Function HasView(viewid As String, Optional ByRef connection As iormConnection = Nothing, Optional nativeConnection As Object = Nothing) As Boolean
            Dim myConnection As OnTrack.Database.oledbConnection
            Dim aTable As DataTable
            Dim myNativeConnection As OleDb.OleDbConnection
            Dim nativeViewname As String = GetNativeViewname(viewid)

            If connection Is Nothing Then
                myConnection = _primaryConnection
            Else
                myConnection = connection
            End If

            If nativeConnection Is Nothing Then
                myNativeConnection = TryCast(myConnection.NativeInternalConnection, OleDb.OleDbConnection)
            Else
                myNativeConnection = TryCast(nativeConnection, OleDb.OleDbConnection)
            End If

            If myConnection Is Nothing OrElse myConnection.NativeInternalConnection Is Nothing Then
                Call CoreMessageHandler(procedure:="oleDBDriver.HasView", message:="No current Connection to the Database", _
                                      messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If
            If myNativeConnection Is Nothing Then
                Call CoreMessageHandler(procedure:="oleDBDriver.HasView", message:="No current internal Connection to the Database", _
                                      messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If
            '*** check on rights we cannot check on the User Table -> recursion
            '* do not check -> makes no sense since we are checking the database status before we are installing
            'If Not CurrentSession.IsBootstrappingInstallation AndAlso name <> User.Constname Then
            '    If Not _currentUserValidation.ValidEntry AndAlso Not _currentUserValidation.HasReadRights Then
            '        If Not myConnection.VerifyUserAccess(accessRequest:=otAccessRight.[ReadOnly], loginOnFailed:=True) Then
            '            Call CoreMessageHandler(showmsgbox:=True, subname:="oleDBDriver.HasView", _
            '                                  message:="No right to read schema of database", messagetype:=otCoreMessageType.ApplicationError)
            '            Return Nothing
            '        End If
            '    End If
            'End If


            Try
                Dim restrictionsTable() As String = {Nothing, Nothing, nativeViewname}
                aTable = myNativeConnection.GetSchema("VIEWS", restrictionsTable)

                If aTable.Rows.Count = 0 Then
                    Return False
                Else
                    Return True
                End If

            Catch ex As OleDb.OleDbException
                Call CoreMessageHandler(message:="Exception", exception:=ex, argument:=nativeViewname, _
                                      procedure:="oleDBDriver.HasView", messagetype:=otCoreMessageType.InternalError)
                Return Nothing

            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception", exception:=ex, argument:=nativeViewname, _
                                      procedure:="oleDBDriver.HasView", messagetype:=otCoreMessageType.InternalError)
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
            Return GetIRDBVisitor()
        End Function

        ''' <summary>
        ''' returns a new visitor object for building expression trees
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GetIRDBVisitor() As IRDBVisitor Implements iormRelationalDatabaseDriver.GetIRDBVisitor
            Return New oleDBXPTVisitor()
        End Function
    End Class


    ''' <summary>
    ''' OLE DB OnTrack Database Connection Class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class oledbConnection
        Inherits adonetConnection
        Implements iormConnection

        'Protected Friend Shadows _nativeConnection As OleDbConnection
        'Protected Friend Shadows _nativeinternalConnection As OleDbConnection

        Public Shadows Event OnConnection As EventHandler(Of ormConnectionEventArgs) Implements iormConnection.OnConnection
        Public Shadows Event OnDisconnection As EventHandler(Of ormConnectionEventArgs) Implements iormConnection.OnDisconnection

        Public Sub New(ByVal id As String, ByRef databaseDriver As iormRelationalDatabaseDriver, sequence As ComplexPropertyStore.Sequence, Optional ByRef session As Session = Nothing)
            MyBase.New(id, databaseDriver, sequence, session:=session)
        End Sub
        ''' <summary>
        ''' Propagate Connected Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OLEDBConnection_OnConnected(sender As Object, e As ormConnectionEventArgs) Handles MyBase.OnConnection
            RaiseEvent OnConnection(sender, e)
        End Sub
        ''' <summary>
        ''' propagates the disconnected event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shadows Function OLEDBConnection_DisConnected(sender As Object, e As ormConnectionEventArgs) Handles MyBase.OnDisconnection
            RaiseEvent OnDisconnection(sender, e)
        End Function
        ''' <summary>
        ''' gets the native connection
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Overrides ReadOnly Property NativeConnection() As Object
            Get
                Return _nativeConnection
            End Get
        End Property
        ''' <summary>
        ''' returns the native Database name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property NativeDatabaseName As String Implements iormConnection.NativeDatabaseName
            Get
                If OledbConnection IsNot Nothing Then
                    Return OledbConnection.DataSource.ToString
                End If
                Return Nothing
            End Get
        End Property
        ''' <summary>
        ''' returns the native Database name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property NativeDatabaseVersion As String Implements iormConnection.NativeDatabaseVersion
            Get
                If OledbConnection IsNot Nothing Then
                    Return OledbConnection.ServerVersion.ToString
                End If
                Return Nothing
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the connection.
        ''' </summary>
        ''' <value>The connection.</value>
        Public Property OledbConnection() As OleDb.OleDbConnection
            Get
                If _nativeConnection Is Nothing AndAlso _nativeConnection.State <> ConnectionState.Closed Then
                    Return Nothing
                Else
                    Dim otdbcn As oledbConnection
                    Return DirectCast(Me.NativeConnection, System.Data.OleDb.OleDbConnection)
                End If

            End Get
            Protected Friend Set(value As OleDb.OleDbConnection)
                Me._nativeConnection = value
            End Set
        End Property


        ''' <summary>
        ''' create a new SQLConnection
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function CreateNewNativeConnection() As IDbConnection
            Return New System.Data.OleDb.OleDbConnection()
        End Function

    End Class


    ''' <summary>
    ''' CLASS describes the schema per table of the database itself
    ''' </summary>
    ''' <remarks></remarks>
    Public Class oledbTableSchema
        Inherits adonetTableSchema
        Implements iormRelationalSchema


        ''' Initializes a new instance of the <see cref="oledbTableSchema" /> class.
        ''' </summary>
        ''' <param name="connection">The connection.</param>
        ''' <param name="tableID">The table ID.</param>
        Public Sub New(ByRef connection As iormConnection, tableID As String)
            MyBase.New(connection, tableID)

        End Sub

        Protected Friend Overrides Function createNativeDBParameter() As IDbDataParameter
            Return New OleDbParameter()
        End Function
        Protected Friend Overrides Function createNativeDBCommand() As IDbCommand
            Return New OleDbCommand()
        End Function
        Protected Friend Overrides Function isNativeDBTypeOfVar(type As Object) As Boolean
            Dim datatype As OleDbType = type

            If datatype = OleDbType.LongVarChar Or datatype = OleDbType.LongVarWChar _
             Or datatype = OleDbType.VarChar Or datatype = OleDbType.VarWChar _
             Or datatype = OleDbType.WChar Or datatype = OleDbType.BSTR _
             Or datatype = OleDbType.Binary Or datatype = OleDbType.Variant _
             Or datatype = OleDbType.LongVarBinary Or datatype = OleDbType.VarBinary Then
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
                                                           Optional parametername As String = Nothing) As IDbDataParameter Implements iormRelationalSchema.AssignNativeDBParameter
            Dim aDBColumnDescription As adonetColumnDescription = GetColumnDescription(Me.GetEntryOrdinal(columnname))
            Dim aParameter As OleDbParameter

            If Not aDBColumnDescription Is Nothing Then

                aParameter = createNativeDBParameter()
                If parametername = String.Empty Then
                    aParameter.ParameterName = "@" & columnname
                Else
                    If parametername.First = "@" Then
                        aParameter.ParameterName = parametername
                    Else
                        aParameter.ParameterName = "@" & parametername
                    End If

                End If

                aParameter.OleDbType = aDBColumnDescription.DataType
                aParameter.SourceColumn = columnname

                '** set the length
                If isNativeDBTypeOfVar(aDBColumnDescription.DataType) Then
                    If aDBColumnDescription.CharacterMaxLength = 0 Then
                        aParameter.Size = constDBDriverMaxMemoSize
                    Else
                        aParameter.Size = aDBColumnDescription.CharacterMaxLength
                    End If

                Else
                    If aDBColumnDescription.CharacterMaxLength <> 0 Then
                        aParameter.Size = aDBColumnDescription.CharacterMaxLength
                    End If
                    aParameter.Size = 0
                End If
                Return aParameter
            Else
                Call CoreMessageHandler(procedure:="oleDBTableSchema.buildParameter", message:="ColumnDescription couldn't be loaded", _
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
            Dim aColumnName As String = String.Empty
            Dim aCon As System.Data.OleDb.OleDbConnection = DirectCast(DirectCast(_Connection, oledbConnection).NativeInternalConnection, System.Data.OleDb.OleDbConnection)


            ' return if no TableID
            If Me.TableID = String.Empty Then
                Call CoreMessageHandler(procedure:="oleDBTableSchema.refresh", _
                                      message:="Nothing table name to set to", _
                                      containerID:=TableID)
                _IsInitialized = False
                Return False
            End If
            '


            Refresh = True

            Try
                SyncLock DirectCast(_Connection, oledbConnection).NativeInternalConnection

                    ' set the SchemaTable
                    Dim restrictionsTable() As String = {Nothing, Nothing, Me.TableID}
                    _ColumnsTable = aCon.GetSchema("COLUMNS", restrictionsTable)
                    Dim columnsList = From columnRow In _ColumnsTable.AsEnumerable _
                                Select TableName = columnRow.Field(Of String)("TABLE_NAME"), _
                                Columnordinal = columnRow.Field(Of Int64)("ORDINAL_POSITION"), _
                                DataType = columnRow.Field(Of OleDbType)("DATA_TYPE"), _
                                [ColumnName] = columnRow.Field(Of String)("COLUMN_NAME"), _
                                Description = columnRow.Field(Of String)("DESCRIPTION"), _
                                CharacterMaxLength = columnRow.Field(Of Nullable(Of Int64))("CHARACTER_MAXIMUM_LENGTH"), _
                                IsNullable = columnRow.Field(Of Nullable(Of Boolean))("IS_NULLABLE") _
                                Where [ColumnName] <> String.Empty Order By TableName, Columnordinal

                    no = columnsList.Count()

                    Dim columnsList1 = From columnRow In _ColumnsTable.AsEnumerable _
                                        Select TableName = columnRow.Field(Of String)("TABLE_NAME"), _
                                        [ColumnName] = columnRow.Field(Of String)("COLUMN_NAME"), _
                                        Columnordinal = columnRow.Field(Of Int64)("ORDINAL_POSITION"), _
                                        DataType = columnRow.Field(Of OleDbType)("DATA_TYPE"), _
                                        IsNullable = columnRow.Field(Of Boolean)("IS_NULLABLE"), _
                                        HasDefault = columnRow.Field(Of Boolean)("COLUMN_HASDEFAULT"), _
                                        [Default] = columnRow.Field(Of String)("COLUMN_DEFAULT"), _
                                        CharacterMaxLength = columnRow.Field(Of Nullable(Of Int64))("CHARACTER_MAXIMUM_LENGTH"), _
                                        CharacterOctetLength = columnRow.Field(Of Nullable(Of Int64))("CHARACTER_OCTET_LENGTH"), _
                                        Description = columnRow.Field(Of String)("DESCRIPTION"), _
                                        NumericPrecision = columnRow.Field(Of Nullable(Of Int64))("NUMERIC_PRECISION"), _
                                        NumericScale = columnRow.Field(Of Nullable(Of Int64))("NUMERIC_SCALE"), _
                                        DateTimePrecision = columnRow.Field(Of Nullable(Of Int64))("DATETIME_PRECISION"), _
                                        Catalog = columnRow.Field(Of String)("TABLE_CATALOG") _
                                        Where [ColumnName] <> String.Empty And TableName <> String.Empty And Columnordinal > 0 _
                                        Order By TableName, Columnordinal


                    '** read indixes
                    Dim restrictionsIndex() As String = {Nothing, Nothing, Nothing, Nothing, Me.TableID}
                    ' get the Index Table
                    _IndexTable = aCon.GetSchema("INDEXES", restrictionsIndex)

                    Dim columnsIndexList = From indexRow In _IndexTable.AsEnumerable _
                                                Select TableName = indexRow.Field(Of String)("TABLE_NAME"), _
                                                IndexName = indexRow.Field(Of String)("INDEX_NAME"), _
                                                Columnordinal = indexRow.Field(Of Int64)("ORDINAL_POSITION"), _
                                                ColumnName = indexRow.Field(Of String)("COLUMN_NAME"), _
                                                isPrimaryKey = indexRow.Field(Of Boolean)("PRIMARY_KEY") _
                                                Where [ColumnName] <> String.Empty And TableName <> String.Empty And Columnordinal > 0 _
                                                Order By TableName, IndexName, Columnordinal, ColumnName


                    no = columnsList.Count

                    If no = 0 Then
                        Call CoreMessageHandler(procedure:="oleDBTableSchema.Refresh", containerID:=Me.TableID, _
                                              messagetype:=otCoreMessageType.InternalError, message:="table has no fields - does it exist ?")
                        _IsInitialized = False
                        Return False
                    End If

                    ReDim _entrynames(no - 1)
                    ReDim _Columns(no - 1)

                    ' set the Dictionaries if reload
                    _fieldsDictionary = New Dictionary(Of String, Long)
                    _indexDictionary = New Dictionary(Of String, ArrayList)
                    aColumnCollection = New ArrayList
                    _NoPrimaryKeys = 0

                    '**** read all the column / columnnames
                    '****
                    Dim i As UShort = 0
                    For Each row In columnsList

                        '*
                        If row.ColumnName.Contains(".") Then
                            aColumnName = UCase(row.ColumnName.Substring(row.ColumnName.IndexOf(".") + 1, row.ColumnName.Length - row.ColumnName.IndexOf(".") + 1))
                        Else
                            aColumnName = UCase(row.ColumnName)
                        End If
                        '*
                        _entrynames(i) = aColumnName.ToUpper
                        '* set the description
                        _Columns(i) = New adonetColumnDescription
                        With _Columns(i)
                            .ColumnName = aColumnName.ToUpper
                            .Description = row.Description
                            '.HasDefault = row.HasDefault
                            .CharacterMaxLength = row.CharacterMaxLength
                            If Not row.CharacterMaxLength Is Nothing Then
                                .CharacterMaxLength = CLng(row.CharacterMaxLength)
                            Else
                                .CharacterMaxLength = 0
                            End If
                            .IsNullable = row.IsNullable
                            .DataType = row.DataType
                            .Ordinal = row.Columnordinal
                            .Default = Nothing
                            .HasDefault = False
                            '.Catalog = row.Catalog
                            '.DateTimePrecision = row.DateTimePrecision
                            '.NumericPrecision = row.NumericPrecision
                            '.NumericScale = row.NumericScale
                            '.CharachterOctetLength = row.CharacterOctetLength
                        End With

                        ' remove if existing
                        If _fieldsDictionary.ContainsKey(aColumnName.ToUpper) Then
                            _fieldsDictionary.Remove(aColumnName.ToUpper)
                        End If
                        ' add
                        _fieldsDictionary.Add(key:=aColumnName.ToUpper, value:=i + 1) 'store no field 1... not the array index

                        '* 
                        i = i + 1
                    Next



                    '**** read each Index
                    '****
                    Dim anIndexName As String = String.Empty
                    For Each row In columnsIndexList

                        If row.ColumnName.Contains(".") Then
                            aColumnName = UCase(row.ColumnName.Substring(row.ColumnName.IndexOf(".") + 1, row.ColumnName.Length))
                        Else
                            aColumnName = UCase(row.ColumnName)
                        End If

                        If row.IndexName.ToUpper <> anIndexName.ToUpper Then
                            '** store
                            If anIndexName <> String.Empty Then
                                If _indexDictionary.ContainsKey(anIndexName.ToUpper) Then
                                    _indexDictionary.Remove(key:=anIndexName.ToUpper)
                                End If
                                _indexDictionary.Add(key:=anIndexName.ToUpper, value:=aColumnCollection)
                            End If
                            ' new
                            anIndexName = row.IndexName.ToUpper
                            aColumnCollection = New ArrayList
                        End If
                        '** Add To List
                        aColumnCollection.Add(aColumnName.ToUpper)

                        ' indx no
                        index = _fieldsDictionary.Item(aColumnName.ToUpper)
                        '
                        '** check if primaryKey
                        'fill old primary Key structure
                        If row.isPrimaryKey Then
                            _PrimaryKeyIndexName = row.IndexName.ToUpper
                            _NoPrimaryKeys = _NoPrimaryKeys + 1
                            ReDim Preserve _Primarykeys(0 To _NoPrimaryKeys - 1)
                            _Primarykeys(_NoPrimaryKeys - 1) = index - 1 ' set to the array 0...ubound
                        End If

                        If Not _fieldsDictionary.ContainsKey(aColumnName.ToUpper) Then
                            Call CoreMessageHandler(procedure:="oleDBTableSchema.refresh", _
                                                  message:="oleDBTableSchema : column " & row.ColumnName & " not in dictionary ?!", _
                                                  containerID:=TableID, entryname:=row.ColumnName)

                            Return False
                        End If

                    Next
                    '** store final
                    If anIndexName <> String.Empty Then
                        If _indexDictionary.ContainsKey(anIndexName.ToUpper) Then
                            _indexDictionary.Remove(key:=anIndexName.ToUpper)
                        End If
                        _indexDictionary.Add(key:=anIndexName.ToUpper, value:=aColumnCollection)
                    End If

                    '**** build the commands
                    '****
                    Dim enumValues As Array = System.[Enum].GetValues(GetType(CommandType))
                    For Each anIndexName In _indexDictionary.Keys
                        Dim aNewCommand As OleDbCommand
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
                Call CoreMessageHandler(showmsgbox:=False, procedure:="oleDBTableSchema.refresh", containerID:=Me.TableID, _
                                      argument:=reloadForce, exception:=ex)
                _IsInitialized = False
                Return False
            End Try

        End Function

    End Class

    ''' <summary>
    ''' describes the ORM Mapping Function per Table for OLE DB
    ''' </summary>
    ''' <remarks></remarks>
    Public Class oledbTableStore
        Inherits adonetTableStore
        Implements iormRelationalTableStore

        'Protected Friend Shadows _cacheAdapter As OleDbDataAdapter

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
        Protected Friend Overrides Function createNativeDBCommand(commandstr As String, ByRef nativeConnection As IDbConnection) As IDbCommand
            Return New OleDbCommand(cmdText:=commandstr, connection:=nativeConnection)
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
                                                    Optional isnullable? As Boolean = Nothing, _
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
            Return Connection.DatabaseDriver.Convert2DBData(invalue:=invalue, outvalue:=outvalue, _
                                                            targetType:=targetType, maxsize:=maxsize, abostrophNecessary:=abostrophNecessary, _
                                       columnname:=columnname, isnullable:=isnullable, defaultvalue:=defaultvalue)
        End Function


        '*********
        '********* cvt2ObjData returns a object from the Datatype of the column to XLS nterpretation
        '*********
        ''' <summary>
        ''' returns a object from the Data type of the column to Host interpretation
        ''' </summary>
        ''' <param name="index">index as object (name or index 1..n)</param>
        ''' <param name="value">value to convert</param>
        ''' <param name="abostrophNecessary">True if necessary</param>
        ''' <returns>converted value </returns>
        ''' <remarks></remarks>
        Public Overrides Function Convert2ObjectData(ByVal index As Object, _
                                                     ByVal invalue As Object, _
                                                     ByRef outvalue As Object, _
                                                     Optional isnullable As Boolean? = Nothing, _
                                                     Optional defaultvalue As Object = Nothing, _
                                                     Optional ByRef abostrophNecessary As Boolean = False) As Boolean Implements iormRelationalTableStore.Convert2ObjectData
            Dim aSchema As oledbTableSchema = Me.ContainerSchema
            Dim aDBColumn As adonetColumnDescription
            Dim result As Object
            Dim fieldno As Integer

            result = Nothing

            Try


                fieldno = aSchema.GetEntryOrdinal(index)
                If fieldno < 0 Then
                    Call CoreMessageHandler(procedure:="oledbTableStore.cvt2ColumnData", messagetype:=otCoreMessageType.InternalError, _
                                          message:="iOTDBTableStore " & Me.TableID & " anIndex for " & index & " not found", _
                                          containerID:=Me.TableID, argument:=index)
                    System.Diagnostics.Debug.WriteLine("iOTDBTableStore " & Me.TableID & " anIndex for " & index & " not found")

                    Return False
                Else
                    aDBColumn = aSchema.GetColumnDescription(fieldno)
                End If
                abostrophNecessary = False
                If Not isnullable.HasValue Then
                    isnullable = Me.ContainerSchema.GetNullable(index)
                End If
                If defaultvalue = Nothing Then
                    defaultvalue = Me.ContainerSchema.GetDefaultValue(index)
                End If
                '** return
                Return Me.Connection.DatabaseDriver.Convert2ObjectData(invalue:=invalue, outvalue:=outvalue, _
                                                                   sourceType:=aDBColumn.DataType, abostrophNecessary:=abostrophNecessary, _
                                                                   isnullable:=isnullable, defaultvalue:=defaultvalue)

            Catch ex As Exception
                Call CoreMessageHandler(showmsgbox:=False, procedure:="oledbTablestore.convert2ObjectData", _
                                      argument:=aDBColumn.DataType, containerID:=Me.TableID, entryname:=aDBColumn.ColumnName, exception:=ex, _
                                      messagetype:=otCoreMessageType.InternalError)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Initialize Cache 
        ''' </summary>
        ''' <returns>true if successful </returns>
        ''' <remarks></remarks>
        Protected Friend Overrides Function InitializeCache(Optional ByVal force As Boolean = False) As Boolean

            Dim aCommand As OleDbCommand
            Dim aDataSet As DataSet

            Try
                '** initialize
                If Not Me.IsCacheInitialized Or force Then
                    ' set theAdapter
                    _cacheAdapter = New OleDbDataAdapter
                    _cacheAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
                    aDataSet = DirectCast(Me.Connection.DatabaseDriver, oleDBDriver).OnTrackDataSet
                    ' Select Command
                    aCommand = DirectCast(Me.ContainerSchema, oledbTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, _
                                                                                          oledbTableSchema.CommandType.SelectType)
                    If Not aCommand Is Nothing Then
                        Dim selectstr As String = "SELECT "
                        For i = 1 To Me.ContainerSchema.NoEntries
                            selectstr &= "[" & Me.ContainerSchema.GetEntryName(i) & "]"
                            If i < Me.ContainerSchema.NoEntries Then
                                selectstr &= ","
                            End If
                        Next
                        selectstr &= " FROM [" & Me.NativeDBObjectname & "]"
                        _cacheAdapter.SelectCommand = New OleDbCommand(selectstr)
                        _cacheAdapter.SelectCommand.CommandType = CommandType.Text
                        _cacheAdapter.SelectCommand.Connection = DirectCast(Me.Connection.NativeConnection, System.Data.OleDb.OleDbConnection)
                        SyncLock Me.Connection.NativeConnection
                            _cacheAdapter.FillSchema(aDataSet, SchemaType.Source)
                            DirectCast(_cacheAdapter, System.Data.OleDb.OleDbDataAdapter).Fill(aDataSet, Me.TableID)
                        End SyncLock

                        ' set the Table
                        _cacheTable = aDataSet.Tables(Me.TableID)
                        If _cacheTable Is Nothing Then
                            Debug.Assert(False)
                        End If

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
                                If fieldlist = String.Empty Then
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
                    aCommand = DirectCast(Me.ContainerSchema, oledbTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, _
                                                                                          oledbTableSchema.CommandType.DeleteType)
                    If Not aCommand Is Nothing Then
                        _cacheAdapter.DeleteCommand = aCommand
                    End If

                    ' Insert Command
                    aCommand = DirectCast(Me.ContainerSchema, oledbTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, _
                                                                                          oledbTableSchema.CommandType.InsertType)
                    If Not aCommand Is Nothing Then
                        _cacheAdapter.InsertCommand = aCommand
                    End If
                    ' Update Command
                    aCommand = DirectCast(Me.ContainerSchema, oledbTableSchema).GetCommand(Me.ContainerSchema.PrimaryKeyIndexName, _
                                                                                          oledbTableSchema.CommandType.UpdateType)
                    If Not aCommand Is Nothing Then
                        _cacheAdapter.UpdateCommand = aCommand
                    End If

                    '** return true
                    Return True
                Else
                    Return False
                End If



            Catch ex As Exception
                Call CoreMessageHandler(procedure:="oledbTableStore.initializeCache", exception:=ex, message:="Exception", _
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
                Return DirectCast(dataadapter, System.Data.OleDb.OleDbDataAdapter).Update(datatable)

            Catch ex As OleDb.OleDbException
                Call CoreMessageHandler(message:="Exception occured", procedure:="oledbTableStore.UpdateDBDataTable", exception:=ex, _
                                    messagetype:=otCoreMessageType.InternalError, containerID:=Me.TableID)
                Return Nothing
            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception occured", procedure:="oledbTableStore.UpdateDBDataTable", exception:=ex, _
                                       messagetype:=otCoreMessageType.InternalError, containerID:=Me.TableID)
                Return 0
            End Try

        End Function
    End Class
    ''' <summary>
    ''' visitor for selection rules to build an oleDB sql statement out of the selection expression
    ''' </summary>
    ''' <remarks></remarks>
    Public Class oleDBXPTVisitor
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
                        ''' TODO
                        ''' 
                        Throw New NotImplementedException("time conversion")
                        result = "CAST('" & aNode.Value.ToString("HH:mm:ss") & "' AS TIME)"
                    Case otDataType.Timestamp
                        ''' TODO
                        ''' 
                        Throw New NotImplementedException("timestamp conversion")
                        result = "CAST('" & aNode.Value.ToString("yyyy-MM-ddTHH:mm:ss") & "' AS DATETIME)"
                    Case otDataType.Date
                        Throw New NotImplementedException("date conversion")
                        result = "CAST('" & aNode.Value.ToString("yyyy-MM-dd") & "' AS DATE)"
                    Case otDataType.Void
                        result = " NULL "
                    Case otDataType.Bool
                        ''' convert the value
                        If Core.DataType.ToBool(aNode.Value) = True Then
                            result = "TRUE"
                        Else
                            result = "FALSE"
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
End Namespace ''' <summary>


