

REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** DATA WARE HOUSE CLASSES and Statics Classes
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
Imports System.Diagnostics
Imports System.Linq
Imports System.Diagnostics.Debug

Imports OnTrack
Imports OnTrack.Database

Namespace OnTrack


    Public Class clsOTDBDataWareHouse
        Inherits ormDataObject

        '************************************************************************************
        '***** CLASS clsOTDBDataWareHouse is a general Table for a primitive DataWareHouse
        '*****
        '*****

        Const ourTableName = "tblDataWareHouse"


        Private s_typeid As String
        Private s_snapdate As Date

        Private Const nodims As Integer = 5
        Private s_dims(10) As String

        Private Const novalues As Integer = 10
        Private s_values(10) As Double

        Private s_serializeWithHostApplication As Boolean

        'fields
        Private s_status As String
        Private s_dependfrompartid As String    ' Component ID
        Private s_condition As String
        Private s_comment As String
        Private s_msgno As String

        Private s_parameter_txt1 As String
        Private s_parameter_txt2 As String
        Private s_parameter_txt3 As String
        Private s_parameter_num1 As Double
        Private s_parameter_num2 As Double
        Private s_parameter_num3 As Double
        Private s_parameter_date1 As Date
        Private s_parameter_date2 As Date
        Private s_parameter_date3 As Date
        Private s_parameter_flag1 As Boolean
        Private s_parameter_flag2 As Boolean
        Private s_parameter_flag3 As Boolean
        ReadOnly Property snapdate() As Date
            Get
                snapdate = s_snapdate
            End Get

        End Property

        Public Property TYPEID() As String
            Get
                TYPEID = s_typeid
            End Get
            Set(value As String)

            End Set
        End Property

        Public Property dims(i As Integer) As String
            Get
                Return dims(i)
            End Get
            Set(value As String)
                dims(i) = value
            End Set
        End Property
        Public Property values(i As Integer) As Double
            Get
                Return values(i)
            End Get
            Set(value As Double)
                values(i) = value
            End Set
        End Property

        Public Property parameter_num1() As Double
            Get
                parameter_num1 = s_parameter_num1
            End Get
            Set(value As Double)
                If s_parameter_num1 <> value Then
                    s_parameter_num1 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_num2() As Double
            Get
                parameter_num2 = s_parameter_num2
            End Get
            Set(value As Double)
                If s_parameter_num2 <> value Then
                    s_parameter_num2 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_num3() As Double
            Get
                parameter_num3 = s_parameter_num3
            End Get
            Set(value As Double)
                If s_parameter_num3 <> value Then
                    s_parameter_num3 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_date1() As Date
            Get
                parameter_date1 = s_parameter_date1
            End Get
            Set(value As Date)
                If s_parameter_date1 <> value Then
                    s_parameter_date1 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_date2() As Date
            Get
                parameter_date2 = s_parameter_date2
            End Get
            Set(value As Date)
                If s_parameter_date2 <> value Then
                    s_parameter_date2 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_date3() As Date
            Get
                parameter_date3 = s_parameter_date3
            End Get
            Set(value As Date)
                s_parameter_date3 = value
                Me.IsChanged = True
            End Set
        End Property
        Public Property parameter_flag1() As Boolean
            Get
                parameter_flag1 = s_parameter_flag1
            End Get
            Set(value As Boolean)
                If s_parameter_flag1 <> value Then
                    s_parameter_flag1 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_flag3() As Boolean
            Get
                parameter_flag3 = s_parameter_flag3
            End Get
            Set(value As Boolean)
                If s_parameter_flag3 <> value Then
                    s_parameter_flag3 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_flag2() As Boolean
            Get
                parameter_flag2 = s_parameter_flag2
            End Get
            Set(value As Boolean)
                If s_parameter_flag2 <> value Then
                    s_parameter_flag2 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_txt1() As String
            Get
                parameter_txt1 = s_parameter_txt1
            End Get
            Set(value As String)
                If s_parameter_txt1 <> value Then
                    s_parameter_txt1 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_txt2() As String
            Get
                parameter_txt2 = s_parameter_txt2
            End Get
            Set(value As String)
                If s_parameter_txt2 <> value Then
                    s_parameter_txt2 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_txt3() As String
            Get
                parameter_txt3 = s_parameter_txt3
            End Get
            Set(value As String)
                If s_parameter_txt3 <> value Then
                    s_parameter_txt3 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        '** initialize

        Public Sub New()
            MyBase.New(ourTableName)

        End Sub
        '*** init
        Public Function initialize() As Boolean

            initialize = MyBase.Initialize

            s_parameter_date1 = ot.ConstNullDate
            s_parameter_date2 = ot.ConstNullDate
            s_parameter_date3 = ot.ConstNullDate

            SerializeWithHostApplication = isDefaultSerializeAtHostApplication(ourTableName)
            's_parameter_date1 = ot.ConstNullDate
            's_parameter_date2 = ot.ConstNullDate
            's_parameter_date3 = ot.ConstNullDate

        End Function

'        '**** infuese the object by a OTDBRecord
'        '****
'        Public Overrides Function Infuse(ByRef record As ormRecord) As Boolean

'            Dim i As Integer
'            Dim aVAlue As Object

'            '* init
'            If Not Me.IsInitialized Then
'                If Not Me.initialize() Then
'                    Infuse = False
'                    Exit Function
'                End If
'            End If

'            On Error GoTo errorhandle

'            '*** overload it from the Application Container
'            '***
'            'If Me.serializeWithHostApplication Then
'            '    If overloadFromHostApplication(me.record) Then
'            '        s_loadedFromHost = True
'            '    End If
'            'End If


'            s_typeid = CStr(Me.Record.GetValue("typeid"))
'            s_snapdate = CDate(Me.Record.GetValue("snapdate"))

'            s_dims(1) = CStr(Me.Record.GetValue("dim1"))
'            s_dims(2) = CStr(Me.Record.GetValue("dim2"))
'            s_dims(3) = CStr(Me.Record.GetValue("dim3"))
'            s_dims(4) = CStr(Me.Record.GetValue("dim4"))
'            s_dims(5) = CStr(Me.Record.GetValue("dim5"))
'            s_dims(6) = CStr(Me.Record.GetValue("dim6"))
'            s_dims(7) = CStr(Me.Record.GetValue("dim7"))
'            s_dims(8) = CStr(Me.Record.GetValue("dim8"))
'            s_dims(9) = CStr(Me.Record.GetValue("dim9"))
'            s_dims(10) = CStr(Me.Record.GetValue("dim10"))

'            s_values(1) = CDbl(Me.Record.GetValue("value1"))
'            s_values(2) = CDbl(Me.Record.GetValue("value2"))
'            s_values(3) = CDbl(Me.Record.GetValue("value3"))
'            s_values(4) = CDbl(Me.Record.GetValue("value4"))
'            s_values(5) = CDbl(Me.Record.GetValue("value5"))
'            s_values(6) = CDbl(Me.Record.GetValue("value6"))
'            s_values(7) = CDbl(Me.Record.GetValue("value7"))
'            s_values(8) = CDbl(Me.Record.GetValue("value8"))
'            s_values(9) = CDbl(Me.Record.GetValue("value9"))
'            s_values(10) = CDbl(Me.Record.GetValue("value10"))

'            s_parameter_txt1 = CStr(Me.Record.GetValue("param_txt1"))
'            s_parameter_txt2 = CStr(Me.Record.GetValue("param_txt2"))
'            s_parameter_txt3 = CStr(Me.Record.GetValue("param_txt3"))
'            s_parameter_num1 = CDbl(Me.Record.GetValue("param_num1"))
'            s_parameter_num2 = CDbl(Me.Record.GetValue("param_num2"))
'            s_parameter_num3 = CDbl(Me.Record.GetValue("param_num3"))
'            s_parameter_date1 = CDate(Me.Record.GetValue("param_date1"))
'            s_parameter_date2 = CDate(Me.Record.GetValue("param_date2"))
'            s_parameter_date3 = CDate(Me.Record.GetValue("param_date3"))
'            s_parameter_flag1 = CBool(Me.Record.GetValue("param_flag1"))
'            s_parameter_flag2 = CBool(Me.Record.GetValue("param_flag2"))
'            s_parameter_flag3 = CBool(Me.Record.GetValue("param_flag3"))



'            Infuse = True
'            me.isloaded = True
'            Exit Function

'errorhandle:
'            Infuse = False


'        End Function

        '**** Inject : load the object by the PrimaryKeys
        '****
        Public Function Inject(ByVal TYPEID As String, ByVal snapdate As Date, ByRef dims() As Object) As Boolean
            Dim aTable As iormDataStore
            Dim pkarry(nodims + 2)
            Dim aRecord As ormRecord
            Dim i As Integer

            '* init
            If Not Me.IsInitialized Then
                If Not Me.initialize() Then
                    Inject = False
                    Exit Function
                End If
            End If

            ' set the primaryKey
            pkarry(1) = TYPEID
            pkarry(2) = snapdate
            For i = 1 To nodims
                If i <= UBound(dims) Then pkarry(2 + i) = dims(i)
            Next i

            aTable = OnTrack.ot.GetTableStore(ourTableName)
            aRecord = aTable.GetRecordByPrimaryKey(pkarry)

            If aRecord Is Nothing Then
                Me.Unload()
                Inject = Me.IsLoaded
                Exit Function
            Else
                Me.Record = aRecord
                'me.isloaded = Me.Infuse(Me.Record)
                Inject = Me.IsLoaded
                Exit Function
            End If

error_handler:
            Inject = True
            Exit Function
        End Function

        '********** static createSchema
        '**********
        Public Function createSchema(Optional silent As Boolean = True) As Boolean

'            Dim PrimaryColumnNames As New Collection
'            Dim aFieldDesc As New ormFieldDescription
'            Dim aTable As New ObjectDefinition

'            With aTable
'                .Create(ourTableName)

'                aFieldDesc.Tablename = ourTableName
'                aFieldDesc.ID = String.empty
'                aFieldDesc.Parameter = String.empty


'                ' typeid
'                aFieldDesc.Datatype = otFieldDataType.Text
'                aFieldDesc.Title = "type of entry"
'                aFieldDesc.ColumnName = "typeid"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                PrimaryColumnNames.Add(aFieldDesc.ColumnName)

'                ' Snapdate
'                aFieldDesc.Datatype = otFieldDataType.[Date]
'                aFieldDesc.Title = "snapdate"
'                aFieldDesc.ColumnName = "snapdate"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                PrimaryColumnNames.Add(aFieldDesc.ColumnName)

'                ' dimension 1
'                aFieldDesc.Datatype = otFieldDataType.Text
'                aFieldDesc.Title = "dimension 1"
'                aFieldDesc.ColumnName = "dim1"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                PrimaryColumnNames.Add(aFieldDesc.ColumnName)
'                ' dimension 2
'                aFieldDesc.Datatype = otFieldDataType.Text
'                aFieldDesc.Title = "dimension 2"
'                aFieldDesc.ColumnName = "dim2"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                PrimaryColumnNames.Add(aFieldDesc.ColumnName)
'                ' dimension 3
'                aFieldDesc.Datatype = otFieldDataType.Text
'                aFieldDesc.Title = "dimension 3"
'                aFieldDesc.ColumnName = "dim3"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                PrimaryColumnNames.Add(aFieldDesc.ColumnName)
'                ' dimension 4
'                aFieldDesc.Datatype = otFieldDataType.Text
'                aFieldDesc.Title = "dimension 4"
'                aFieldDesc.ColumnName = "dim4"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                PrimaryColumnNames.Add(aFieldDesc.ColumnName)
'                ' dimension 5
'                aFieldDesc.Datatype = otFieldDataType.Text
'                aFieldDesc.Title = "dimension 5"
'                aFieldDesc.ColumnName = "dim5"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                PrimaryColumnNames.Add(aFieldDesc.ColumnName)
'                ' dimension 6
'                aFieldDesc.Datatype = otFieldDataType.Text
'                aFieldDesc.Title = "dimension 6"
'                aFieldDesc.ColumnName = "dim6"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                'PrimaryColumnNames.add aFieldDesc.Name
'                ' dimension 7
'                aFieldDesc.Datatype = otFieldDataType.Text
'                aFieldDesc.Title = "dimension 7"
'                aFieldDesc.ColumnName = "dim7"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                'PrimaryColumnNames.add aFieldDesc.Name
'                ' dimension 8
'                aFieldDesc.Datatype = otFieldDataType.Text
'                aFieldDesc.Title = "dimension 8"
'                aFieldDesc.ColumnName = "dim8"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                'PrimaryColumnNames.add aFieldDesc.Name
'                ' dimension 9
'                aFieldDesc.Datatype = otFieldDataType.Text
'                aFieldDesc.Title = "dimension 9"
'                aFieldDesc.ColumnName = "dim9"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                'PrimaryColumnNames.add aFieldDesc.Name
'                ' dimension 10
'                aFieldDesc.Datatype = otFieldDataType.Text
'                aFieldDesc.Title = "dimension 10"
'                aFieldDesc.ColumnName = "dim10"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                'PrimaryColumnNames.add aFieldDesc.Name


'                ' Value 1
'                aFieldDesc.Datatype = otFieldDataType.Numeric
'                aFieldDesc.Title = "value #1"
'                aFieldDesc.ColumnName = "value1"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' Value 2
'                aFieldDesc.Datatype = otFieldDataType.Numeric
'                aFieldDesc.Title = "value #2"
'                aFieldDesc.ColumnName = "value2"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' Value 3
'                aFieldDesc.Datatype = otFieldDataType.Numeric
'                aFieldDesc.Title = "value #3"
'                aFieldDesc.ColumnName = "value3"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' Value 4
'                aFieldDesc.Datatype = otFieldDataType.Numeric
'                aFieldDesc.Title = "value #4"
'                aFieldDesc.ColumnName = "value4"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' Value 5
'                aFieldDesc.Datatype = otFieldDataType.Numeric
'                aFieldDesc.Title = "value #5"
'                aFieldDesc.ColumnName = "value5"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' Value 6
'                aFieldDesc.Datatype = otFieldDataType.Numeric
'                aFieldDesc.Title = "value #6"
'                aFieldDesc.ColumnName = "value6"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' Value 7
'                aFieldDesc.Datatype = otFieldDataType.Numeric
'                aFieldDesc.Title = "value #7"
'                aFieldDesc.ColumnName = "value7"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' Value 8
'                aFieldDesc.Datatype = otFieldDataType.Numeric
'                aFieldDesc.Title = "value #8"
'                aFieldDesc.ColumnName = "value8"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' Value 9
'                aFieldDesc.Datatype = otFieldDataType.Numeric
'                aFieldDesc.Title = "value #9"
'                aFieldDesc.ColumnName = "value9"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' Value 10
'                aFieldDesc.Datatype = otFieldDataType.Numeric
'                aFieldDesc.Title = "value #10"
'                aFieldDesc.ColumnName = "value10"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)


'                ' parameter_txt 1
'                aFieldDesc.Datatype = otFieldDataType.Text
'                aFieldDesc.Title = "parameter_txt 1 of condition"
'                aFieldDesc.ColumnName = "param_txt1"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' parameter_txt 2
'                aFieldDesc.Datatype = otFieldDataType.Text
'                aFieldDesc.Title = "parameter_txt 2 of condition"
'                aFieldDesc.ColumnName = "param_txt2"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' parameter_txt 3
'                aFieldDesc.Datatype = otFieldDataType.Text
'                aFieldDesc.Title = "parameter_txt 3 of condition"
'                aFieldDesc.ColumnName = "param_txt3"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' parameter_num 1
'                aFieldDesc.Datatype = otFieldDataType.Numeric
'                aFieldDesc.Title = "parameter numeric 1 of condition"
'                aFieldDesc.ColumnName = "param_num1"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' parameter_num 2
'                aFieldDesc.Datatype = otFieldDataType.Numeric
'                aFieldDesc.Title = "parameter numeric 2 of condition"
'                aFieldDesc.ColumnName = "param_num2"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' parameter_num 3
'                aFieldDesc.Datatype = otFieldDataType.Numeric
'                aFieldDesc.Title = "parameter numeric 3 of condition"
'                aFieldDesc.ColumnName = "param_num3"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)

'                ' parameter_date 1
'                aFieldDesc.Datatype = otFieldDataType.[Date]
'                aFieldDesc.Title = "parameter date 1 of condition"
'                aFieldDesc.ColumnName = "param_date1"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)

'                ' parameter_date 2
'                aFieldDesc.Datatype = otFieldDataType.[Date]
'                aFieldDesc.Title = "parameter date 2 of condition"
'                aFieldDesc.ColumnName = "param_date2"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' parameter_date 3
'                aFieldDesc.Datatype = otFieldDataType.[Date]
'                aFieldDesc.Title = "parameter date 3 of condition"
'                aFieldDesc.ColumnName = "param_date3"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)

'                ' parameter_flag 1
'                aFieldDesc.Datatype = otFieldDataType.Bool
'                aFieldDesc.Title = "parameter flag 1 of condition"
'                aFieldDesc.ColumnName = "param_flag1"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)

'                ' parameter_flag 2
'                aFieldDesc.Datatype = otFieldDataType.Bool
'                aFieldDesc.Title = "parameter flag 2 of condition"
'                aFieldDesc.ColumnName = "param_flag2"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' parameter_flag 3
'                aFieldDesc.Datatype = otFieldDataType.Bool
'                aFieldDesc.Title = "parameter flag 3 of condition"
'                aFieldDesc.ColumnName = "param_flag3"
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)


'                '***
'                '*** TIMESTAMP
'                '****
'                aFieldDesc.Datatype = otFieldDataType.Timestamp
'                aFieldDesc.Title = "last Update"
'                aFieldDesc.ColumnName = ot.ConstFNUpdatedOn
'                aFieldDesc.ID = String.empty
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)

'                aFieldDesc.Datatype = otFieldDataType.Timestamp
'                aFieldDesc.Title = "creation Date"
'                aFieldDesc.ColumnName = ot.ConstFNCreatedOn
'                aFieldDesc.ID = String.empty
'                Call .AddFieldDesc(fielddesc:=aFieldDesc)
'                ' Index
'                Call .AddIndex("PrimaryKey", PrimaryColumnNames, isprimarykey:=True)


'                ' persist
'                .Persist()
'                ' change the database
'                .CreateObjectSchema()
'            End With

'            ' reset the Table description
'            If Not Me.Record.SetTable(ourTableName, forceReload:=True) Then
'                Call ot.CoreMessageHandler(subname:="clsDependency.createSchema", tablename:=ourTableName, _
'                                      message:="Error while setTable in createSchema")
'            End If

'            '
'            createSchema = True
'            Exit Function


'            ' Handle the error
'error_handle:
'            Call ot.CoreMessageHandler(subname:="clsOTDBBOM.createSchema", tablename:=ourTableName)
'            createSchema = False
        End Function

        '**** persist
        '****

        Public Function Persist(Optional timestamp As Date = ot.ConstNullDate) As Boolean

            '* init
            If Not Me.IsInitialized Then
                If Not Me.initialize() Then
                    Persist = False
                    Exit Function
                End If
            End If
            'On Error GoTo errorhandle
            Call Me.Record.SetValue("typeid", s_typeid)
            Call Me.Record.SetValue("snapdate", s_snapdate)

            Call Me.Record.SetValue("dim1", s_dims(1))
            Call Me.Record.SetValue("dim2", s_dims(2))
            Call Me.Record.SetValue("dim3", s_dims(3))
            Call Me.Record.SetValue("dim4", s_dims(4))
            Call Me.Record.SetValue("dim5", s_dims(5))
            Call Me.Record.SetValue("dim6", s_dims(6))
            Call Me.Record.SetValue("dim7", s_dims(7))
            Call Me.Record.SetValue("dim8", s_dims(8))
            Call Me.Record.SetValue("dim9", s_dims(9))
            Call Me.Record.SetValue("dim10", s_dims(10))

            Call Me.Record.SetValue("value1", s_values(1))
            Call Me.Record.SetValue("value2", s_values(2))
            Call Me.Record.SetValue("value3", s_values(3))
            Call Me.Record.SetValue("value4", s_values(4))
            Call Me.Record.SetValue("value5", s_values(5))
            Call Me.Record.SetValue("value6", s_values(6))
            Call Me.Record.SetValue("value7", s_values(7))
            Call Me.Record.SetValue("value8", s_values(8))
            Call Me.Record.SetValue("value9", s_values(9))
            Call Me.Record.SetValue("value10", s_values(10))


            Call Me.Record.SetValue("param_txt1", s_parameter_txt1)
            Call Me.Record.SetValue("param_txt2", s_parameter_txt2)
            Call Me.Record.SetValue("param_txt3", s_parameter_txt3)
            Call Me.Record.SetValue("param_date1", s_parameter_date1)
            Call Me.Record.SetValue("param_date2", s_parameter_date2)
            Call Me.Record.SetValue("param_date3", s_parameter_date3)
            Call Me.Record.SetValue("param_num1", s_parameter_num1)
            Call Me.Record.SetValue("param_num2", s_parameter_num2)
            Call Me.Record.SetValue("param_num3", s_parameter_num3)
            Call Me.Record.SetValue("param_flag1", s_parameter_flag1)
            Call Me.Record.SetValue("param_flag2", s_parameter_flag2)
            Call Me.Record.SetValue("param_flag3", s_parameter_flag3)

            ' overwrite
            'If Me.serializeWithHostApplication Then
            '    If overwriteToHostApplication(me.record) Then
            '        s_savedToHost = True
            '    End If
            'Else
            'Call me.record.setValue(OTDBConst_UpdateOn, (Date & " " & Time)) not necessary

            Persist = Me.Record.Persist(timestamp)

            'End If
            Exit Function

errorhandle:

            Persist = False

        End Function

        '**** create : create a new Object with primary keys
        '****
        Public Function create(ByVal TYPEID As String, ByVal snapdate As Date, ByRef dims() As Object) As Boolean
            Dim aTable As iormDataStore
            Dim pkarry(nodims + 2)
            Dim aRecord As ormRecord
            Dim i As Integer

            If IsLoaded Then
                create = False
                Exit Function
            End If
            '* init
            If Not Me.IsInitialized Then
                If Not Me.initialize() Then
                    create = False
                    Exit Function
                End If
            End If

            ' Check
            ' set the primaryKey
            pkarry(1) = TYPEID
            pkarry(2) = snapdate
            For i = 1 To nodims
                If i <= UBound(dims) Then pkarry(2 + i) = dims(i)
            Next i

            'PKArry(3) = dependfrompartid
            aTable = Me.PrimaryTableStore
            aRecord = aTable.GetRecordByPrimaryKey(pkarry)

            If Not aRecord Is Nothing Then
                create = False
                'Call OTDBErrorHandler(tablename:=ourTableName, entryname:="partid, posno", _
                'subname:="clsOTDBBOMMember.create", message:=" double key as should be unique", arg1:=partid & posno)
                Exit Function
            End If

            ' set the primaryKey
            s_typeid = TYPEID
            s_snapdate = snapdate
            For i = 1 To UBound(dims)
                s_dims(i) = dims(i)
            Next i

            'me.iscreated = True
            create = Me.IsCreated

        End Function

    End Class

End Namespace