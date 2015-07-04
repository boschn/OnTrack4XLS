REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** BUSINESS OBJECTs: Bill-Of-Material Classes On Track Database Backend Library
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
Imports System.Data.OleDb


Imports OnTrack
Imports OnTrack.Database

Namespace OnTrack.Parts


    Public Class clsOTDBBOM
        Inherits ormDataObject

        '************************************************************************************
        '***** CLASS clsOTDBBOM is the object for a OTDBRecord (which is the datastore)
        '*****
        '*****

        Const ourTableName = "tbleBOM"

        ' key
        Private s_pnid As String
        ' components itself per key:=posno, item:=cmid
        Private s_cmids As New Dictionary(Of Long, clsOTDBBOMMember)

        '** initialize
        Public Sub New()
            Call MyBase.New(ourTableName)

        End Sub

        ReadOnly Property PARTID()
            Get
                PARTID = s_pnid
            End Get

        End Property

        ReadOnly Property NoComponents() As Long
            Get
                NoComponents = s_cmids.Count - 1
            End Get

        End Property

        Public Function getMaxPosNo() As Long
            Dim keys() As Object
            Dim i As Integer
            Dim max As Long

            If NoComponents >= 0 Then
                For Each pos As Long In s_cmids.Keys
                    If pos > max Then max = pos
                Next
                'keys = s_cmids.Keys
                'For i = LBound(keys) To UBound(keys)
                'If keys(i) > max Then max = keys(i)
                'Next i
                getMaxPosNo = max
            Else
                getMaxPosNo = 0
            End If
        End Function
        '*** add a Component by cls OTDB
        '***
        Public Function addPartID(aPNID As String, aQty As Double) As Boolean
            Dim flag As Boolean
            Dim existEntry As New clsOTDBBOMMember
            Dim anEntry As New clsOTDBBOMMember
            Dim m As Object
            Dim posno As Long

            ' empty
            If Not me.isloaded And Not Me.IsCreated Then
                addPartID = False
                Exit Function
            End If
            ' check Members
            For Each kvp As KeyValuePair(Of Long, clsOTDBBOMMember) In s_cmids
                existEntry = kvp.Value
                ' check
                If LCase(existEntry.cmid) = LCase(anEntry.cmid) Then
                    addPartID = False
                    Exit Function
                End If
            Next
            ' create new Member
            anEntry = New clsOTDBBOMMember
            posno = Me.getMaxPosNo + 1
            If Not anEntry.create(s_pnid, Me.getMaxPosNo + 1, cmid:=aPNID, qty:=aQty) Then
                Call anEntry.Inject(s_pnid, posno)
            End If
            anEntry.cmid = aPNID
            anEntry.qty = aQty

            ' add the component
            addPartID = Me.addComponent(anEntry)

        End Function

        '*** add a Component by cls OTDB
        '***
        Public Function addComponent(anEntry As clsOTDBBOMMember) As Boolean
            Dim flag As Boolean
            Dim existEntry As New clsOTDBBOMMember
            Dim m As Object

            ' empty
            If Not me.isloaded And Not Me.IsCreated Then
                addComponent = False
                Exit Function
            End If

            ' remove and overwrite
            If s_cmids.ContainsKey(key:=anEntry.posno) Then
                Call s_cmids.Remove(key:=anEntry.posno)
            End If
            ' add entry
            s_cmids.Add(key:=anEntry.posno, value:=anEntry)

            '
            addComponent = True

        End Function

        '**** delete
        '****
        Public Function delete() As Boolean
            Dim anEntry As New clsOTDBBOMMember
            Dim initialEntry As New clsOTDBBOMMember
            Dim m As Object

            If Not Me.IsCreated And Not me.isloaded Then
                delete = False
                Exit Function
            End If

            ' delete each entry
            For Each kvp As KeyValuePair(Of Long, clsOTDBBOMMember) In s_cmids
                anEntry = kvp.Value

                anEntry.Delete()
            Next

            ' reset it
            s_cmids = New Dictionary(Of Long, clsOTDBBOMMember)
            If Not anEntry.create(AssyID:=Me.PARTID, posno:=0, cmid:=String.empty, qty:=0) Then
                Call anEntry.Inject(AssyID:=Me.PARTID, posno:=0)
                anEntry.cmid = String.empty
                anEntry.qty = 0
            End If
            s_cmids.Add(key:=0, value:=anEntry)

            'me.iscreated = True
            _IsDeleted = True
            Me.Unload()

        End Function

        '**** Posno
        '****
        Public Function posno() As Object

            If Not Me.IsCreated And Not me.isloaded Then
                posno = Nothing
                Exit Function
            End If

            ' delete each entry
            posno = s_cmids.Keys


        End Function
        '**** Members returns a Collection of Members
        '****
        Public Function Members() As Collection
            Dim anEntry As New clsOTDBBOMMember
            Dim aCollection As New Collection
            Dim m As Object

            If Not Me.IsCreated And Not me.isloaded Then
                Members = Nothing
                Exit Function
            End If

            ' delete each entry
            For Each kvp As KeyValuePair(Of Long, clsOTDBBOMMember) In s_cmids
                anEntry = kvp.Value
                If anEntry.posno <> 0 Then
                    aCollection.Add(anEntry)
                End If
            Next

            Members = aCollection
        End Function
        '**** infuese the object by a OTDBRecord
        '****
        Public Function infuse(ByRef aRecord As ormRecord) As Boolean
            ' not implemented
            infuse = False
        End Function

        '**** Inject : load the object by the PrimaryKeys
        '****
        Public Function Inject(ByVal pnid As String) As Boolean
            Dim aTable As iormDataStore
            Dim aRecordCollection As List(Of ormRecord)
            Dim aRecord As ormRecord
            Dim cmid As String
            Dim posno As Long
            Dim qty As Double
            Dim anEntry As New clsOTDBBOMMember

            Dim wherestr As String
            'Dim PKArry(1 To 1) As Variant

            ' set the primaryKey

            aTable = GetTableStore(ourTableName)
            aRecordCollection = aTable.GetRecordsBySql(wherestr:="assyid = '" & pnid & "'")
            'Set aRecordCollection = aTable.getRecordsByIndex(aTable.primaryKeyIndexName, Key, True)

            If aRecordCollection Is Nothing Then
                Me.Unload()
                Inject = False
                Exit Function
            Else
                s_pnid = pnid
                '
                ' records read
                For Each aRecord In aRecordCollection
                    posno = aRecord.GetValue("posno")
                    cmid = aRecord.GetValue("cmid")
                    qty = aRecord.GetValue("qty")
                    ' add the Entry as Component
                    anEntry = New clsOTDBBOMMember
                    If anEntry.infuse(aRecord) Then
                        If Not Me.addComponent(anEntry) Then
                        End If
                    End If
                Next aRecord
                '
                'me.isloaded = True
                Inject = True
                Exit Function
            End If

error_handler:
            Me.Unload()
            Inject = True
            Exit Function
        End Function

        '**** persist
        '****

        Public Function persist(Optional ByVal TIMESTAMP As Date = Nothing) As Boolean
            Dim anEntry As Object
            Dim aTimestamp As Date

            If Not Me.IsInitialized Then
                If Not Me.initialize() Then
                    persist = False
                    Exit Function
                End If
            End If
            If Not IsLoaded And Not IsCreated Then
                persist = False
                Exit Function
            End If


            ' set Timestamp
            If TIMESTAMP = Nothing Then
                TIMESTAMP = Date.Now()
            End If

            ' delete each entry
            For Each kvp As KeyValuePair(Of Long, clsOTDBBOMMember) In s_cmids
                anEntry = kvp.Value
                anEntry.PERSIST(TIMESTAMP)
            Next

            persist = True

            Exit Function

errorhandle:

            persist = False

        End Function

        '**** create : create a new Object with primary keys
        '****
        Public Function create(ByVal pnid As String) As Boolean
            Dim anEntry As New clsOTDBBOMMember

            If IsLoaded Then
                create = False
                Exit Function
            End If

            ' set the primaryKey
            s_pnid = pnid
            s_cmids = New Dictionary(Of Long, clsOTDBBOMMember)
            ' abort create if exists
            If Not anEntry.create(AssyID:=pnid, posno:=0) Then
                create = False
                Exit Function
            End If
            s_cmids.Add(key:=0, value:=anEntry)

            'me.iscreated = True
            create = Me.IsCreated

        End Function

    End Class

    Public Class clsOTDBBOMMember
        Inherits ormDataObject
        '************************************************************************************
        '***** CLASS clsOTDBBOMMember is a helper for the BOM Members
        '*****
        '*****

        Private s_assyid As String    ' Assy ID
        Private s_cmid As String    ' Component ID
        Private s_posno As Long
        Private s_qty As Double

        Const ourTableName = "tbleBOM"

        ReadOnly Property AssyID() As String
            Get
                AssyID = s_assyid

            End Get
        End Property

        ReadOnly Property posno() As Long
            Get
                posno = s_posno
            End Get

        End Property

        Public Property cmid() As String
            Get
                cmid = s_cmid
            End Get
            Set(value As String)
                s_cmid = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property qty() As Double
            Get
                qty = s_qty
            End Get
            Set(value As Double)
                s_qty = value
                Me.IsChanged = True
            End Set
        End Property

        '** initialize

        Public Sub New()
            MyBase.New(ourTableName)
        End Sub

        '**** infuese the object by a OTDBRecord
        '****
        Public Function infuse(ByRef aRecord As ormRecord) As Boolean

            '* init
            If Not Me.IsInitialized Then
                If Not Me.Initialize() Then
                    infuse = False
                    Exit Function
                End If
            End If


            On Error GoTo errorhandle

            Me.Record = aRecord

            s_assyid = CStr(aRecord.GetValue("assyid"))
            s_cmid = CStr(aRecord.GetValue("cmid"))
            s_qty = CLng(aRecord.GetValue("qty"))
            s_posno = CLng(aRecord.GetValue("posno"))
            _updatedOn = CDate(aRecord.GetValue(ConstFNUpdatedOn))
            _createdOn = CDate(aRecord.GetValue(ConstFNCreatedOn))

            infuse = True
            'me.isloaded = True
            Exit Function

errorhandle:
            infuse = False


        End Function

        '**** Inject : load the object by the PrimaryKeys
        '****
        Public Function Inject(ByVal AssyID As String, ByVal posno As Long) As Boolean
            Dim aTable As iormDataStore
            Dim pkarry(2) As Object
            Dim aRecord As ormRecord

            ' set the primaryKey
            pkarry(0) = AssyID
            pkarry(1) = posno

            aTable = GetTableStore(ourTableName)
            aRecord = aTable.GetRecordByPrimaryKey(pkarry)

            If aRecord Is Nothing Then
                Me.Unload()
                Inject = Me.IsLoaded
                Exit Function
            Else
                Me.Record = aRecord
                'me.isloaded = Me.infuse(Me.Record)
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



            '            Dim UsedKeyColumnNames As New Collection
            '            Dim aFieldDesc As New ormFieldDescription
            '            Dim PrimaryColumnNames As New Collection
            '            Dim WorkspaceColumnNames As New Collection
            '            Dim aTable As New ObjectDefinition
            '            Dim aTableEntry As New ObjectEntryDefinition


            '            aFieldDesc.ID = String.empty
            '            aFieldDesc.Parameter = String.empty
            '            aFieldDesc.Relation = New String() {}
            '            aFieldDesc.Aliases = New String() {}
            '            aFieldDesc.Tablename = ourTableName


            '            aTable = New ObjectDefinition
            '            aTable.Create(ourTableName)

            '            '******
            '            '****** Fields

            '            With aTable


            '                On Error GoTo error_handle


            '                '*** TaskUID
            '                '****
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "Assembly part-id"
            '                aFieldDesc.ID = String.empty
            '                aFieldDesc.Parameter = String.empty
            '                aFieldDesc.ColumnName = "assyid"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                PrimaryColumnNames.Add(aFieldDesc.ColumnName)

            '                'Position
            '                aFieldDesc.Datatype = otFieldDataType.[Long]
            '                aFieldDesc.Title = "posno"
            '                aFieldDesc.ColumnName = "posno"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                PrimaryColumnNames.Add(aFieldDesc.ColumnName)

            '                'component id
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "component part-id"
            '                aFieldDesc.ColumnName = "cmid"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                UsedKeyColumnNames.Add(aFieldDesc.ColumnName)

            '                ' number
            '                aFieldDesc.Datatype = otFieldDataType.Numeric
            '                aFieldDesc.Title = "quantity"
            '                aFieldDesc.ColumnName = "qty"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)


            '                '***
            '                '*** TIMESTAMP
            '                '****
            '                aFieldDesc.Datatype = otFieldDataType.Timestamp
            '                aFieldDesc.Title = "last Update"
            '                aFieldDesc.ColumnName = ConstFNUpdatedOn
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                aFieldDesc.Datatype = otFieldDataType.Timestamp
            '                aFieldDesc.Title = "creation Date"
            '                aFieldDesc.ColumnName = ConstFNCreatedOn
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' Index
            '                Call .AddIndex("PrimaryKey", PrimaryColumnNames, isprimarykey:=True)
            '                Call .AddIndex("UsedByKey", UsedKeyColumnNames, isprimarykey:=False)
            '                ' persist
            '                .Persist()
            '                ' change the database
            '                .CreateObjectSchema()
            '            End With

            '            ' reset the Table description
            '            If Not Me.Record.SetTable(ourTableName, forceReload:=True) Then
            '                Call CoreMessageHandler(subname:="clsOTDBBOM.createSchema", tablename:=ourTableName, _
            '                                      message:="Error while setTable in createSchema")
            '            End If



            '            '
            '            createSchema = True
            '            Exit Function

            '            ' Handle the error
            'error_handle:
            '            Call CoreMessageHandler(subname:="clsOTDBBOM.createSchema", tablename:=ourTableName)
            '            createSchema = False
        End Function

        '**** persist
        '****


        Public Function Persist(Optional timestamp As Date = ot.ConstNullDate) As Boolean

            '* init
            If Not Me.IsInitialized Then
                If Not Me.Initialize() Then
                    Persist = False
                    Exit Function
                End If
            End If

            'On Error GoTo errorhandle
            Call Me.Record.SetValue("assyid", s_assyid)
            Call Me.Record.SetValue("posno", s_posno)
            Call Me.Record.SetValue("cmid", s_cmid)
            Call Me.Record.SetValue("qty", s_qty)

            'Call me.record.setValue(OTDBConst_UpdateOn, (Date & " " & Time)) not necessary

            Persist = Me.Record.Persist(timestamp)

            Exit Function

errorhandle:

            Persist = False

        End Function

        '**** create : create a new Object with primary keys
        '****
        Public Function create(ByVal AssyID As String, ByVal posno As Long, _
        Optional ByVal cmid As String = String.empty, Optional qty As Double = 0) As Boolean
            Dim aTable As iormDataStore
            Dim pkarry(2) As Object
            Dim aRecord As ormRecord

            If IsLoaded Then
                create = False
                Exit Function
            End If

            ' Check
            ' set the primaryKey
            pkarry(0) = AssyID
            pkarry(1) = posno
            'PKArry(3) = cmid
            aTable = GetTableStore(ourTableName)
            aRecord = aTable.GetRecordByPrimaryKey(pkarry)

            If Not aRecord Is Nothing Then
                create = False
                'Call OTDBErrorHandler(tablename:=ourTableName, entryname:="assyid, posno", _
                'subname:="clsOTDBBOMMember.create", message:=" double key as should be unique", arg1:=AssyID & posno)
                Exit Function
            End If

            ' set the primaryKey
            s_assyid = AssyID
            s_posno = posno
            s_cmid = cmid
            s_qty = qty

            'me.iscreated = True
            create = Me.IsCreated

        End Function
    End Class
End Namespace
