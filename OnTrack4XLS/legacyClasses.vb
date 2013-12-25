
REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE TOOLING 4 EXCEL
REM ***********
REM *********** LEGACY OBJECT: EXCEL LEGACY CLASSES for Interfaces Status etc.
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2013-08-08
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** TO DO Log:
REM ***********             - get ridd of these here -> exchange against clsOTDBDefStatus
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************

Option Explicit On
Imports System.Diagnostics

Imports System.Diagnostics.Debug

Imports Microsoft.Office.Interop.Excel

Public Class clsIFStatus

    '***********************************************************************
    '***** CLASS status is a representation class of the stati
    '*****
    '***** A Status can be in an ICD or/and in the interface
    '***** although the Stati Definition are the same
    '***** the stati itsel can differ

    ' const name of table holding data
    Const parameter_status_ifc_table = "parameter_status_interface_table"
    Const parameter_icd_status_table = "parameter_status_icd_table"
    Const parameter_icd_x_table = "parameter_status_icd_x_table"

    ' const no of columns in the table to hold status parameter
    Const parameter_status_col_code = 1
    Const parameter_status_col_name = 2
    Const parameter_status_col_oldcode = 3
    Const parameter_status_col_weight = 4
    Const parameter_status_col_freeze = 5
    Const parameter_status_col_desc = 6
    Const parameter_status_col_kpicode = 7
    Const parameter_status_col_format = 8    ' additional

    'Const ifcStatusCode = 1
    'Const ifcStatusDesc = 2
    'Const ifcStatusWeight = 3

    ' possible codes
    Const ifcStatus_accepted = "g1"
    Const ifcStatus_incoporated = "g2"
    Const ifcStatus_pending = "y1"
    Const ifcStatus_superseded = "y2"    ' Superseded
    Const ifcStatusR1 = "r1"
    Const ifcStatusR2 = "r2"
    Const ifcStatusR3 = "r3"
    Const ifcStatusna = "na"
    Const ifcStatusNull = "null"

    ' Holding the Status
    Private scode As String


    '** initialize
    Public Sub New()
        scode = ifcStatusNull

    End Sub
    '** get the code
    Public Property code() As String
        Get
            code = scode
        End Get
        Set(NewCode As String)
            Dim ourRow As Range
            Dim oldcode As String

            oldcode = scode    'save it
            scode = clean(newCode)    ' set the new code

            If Not Verify(newCode) Then
                scode = oldcode    'reset
                'Debug.Print "Code was changed to " & newCode & " which is not in parameter_status_ifc_table"
                Exit Property
            End If
        End Set
    End Property

    '** get the weight
    ReadOnly Property weight() As Long
        Get
            Dim ourRow() As Object

            ourRow = getIfcStatusRow()
            If Not IsArrayInitialized(ourRow) Then
                'Debug.Print "Status not defined"
                weight = -1
                Exit Property
            End If

            ' first column to return
            If IsNumeric(ourRow(parameter_status_col_weight - 1)) Then
                weight = CDec(ourRow(parameter_status_col_weight - 1))
            Else
                weight = -1
            End If
        End Get
    End Property

    '** get the description
    ReadOnly Property description() As String
        Get
            Dim ourRow() As Object

            ourRow = getIfcStatusRow()
            If Not IsArrayInitialized(ourRow) Then
                'Debug.Print "Status not defined"
                description = ""
                Exit Property
            End If

            ' first column to return

            description = ourRow(parameter_status_col_desc - 1)
        End Get

    End Property
    '** get the description
    ReadOnly Property Name() As String
        Get
            Dim ourRow() As Object

            ourRow = getIfcStatusRow()
            If Not IsArrayInitialized(ourRow) Then
                'Debug.Print "Name not defined"
                Name = ""
                Exit Property
            End If

            ' first column to return

            Name = ourRow(parameter_status_col_name - 1)
        End Get
    End Property

    '** get the description
    ReadOnly Property kpicode() As String
        Get
            Dim ourRow As Object

            ourRow = getIfcStatusRow()
            If Not IsArrayInitialized(ourRow) Then
                'Debug.Print "Name not defined"
                kpicode = ""
                Exit Property
            End If

            ' first column to return

            kpicode = ourRow(parameter_status_col_kpicode - 1)
        End Get
    End Property

    '** get the description
    ReadOnly Property oldcode() As String
        Get
            Dim ourRow() As Object

            ourRow = getIfcStatusRow()
            If Not IsArrayInitialized(ourRow) Then
                'Debug.Print "Name not defined"
                oldcode = ""
                Exit Property
            End If

            ' first column to return

            oldcode = ourRow(parameter_status_col_oldcode - 1)
        End Get
    End Property



    '** verify code
    Public Function Verify(ByVal newCode As String, Optional exact As Boolean = False) As Boolean
        Dim ourTable As Object(,)
        Dim i, j As Integer

        Verify = False
        ourTable = getIfcStatusTable()
        If Not IsArrayInitialized(ourTable) Then
            Verify = False
            Exit Function
        End If



        For i = 0 To ifc_status_table_maxrow
            If (ourTable(i, parameter_status_col_code - 1) = newCode And exact) Or _
               ((ourTable(i, parameter_status_col_code - 1) = newCode Or _
                 ourTable(i, parameter_status_col_kpicode - 1) = newCode Or _
                 ourTable(i, parameter_status_col_oldcode - 1) = newCode) And Not exact) Then
                Verify = True
            End If
        Next i


    End Function
    Public Function clean(newCode As String)
        clean = LCase(Trim(newCode))
    End Function

    '** getIfcStatusTable returns the Status Table
    Public Function getIfcStatusTable() As Object

        getIfcStatusTable = getIFStatusTable()

    End Function
    '** getIfcStatusRow getStatus Row of Table
    '**        searchs through all fields of table !
    Public Function getIfcStatusRow() As Object
        Dim ourTable(,) As Object
        Dim ourRow() As Object
        Dim i, j As Integer

        ourTable = getIfcStatusTable()
        If Not IsArrayInitialized(ourTable) Then
            getIfcStatusRow = Nothing
            Exit Function
        End If

        For i = 0 To ifc_status_table_maxrow
            If ourTable(i, parameter_status_col_code - 1) = scode Or _
               ourTable(i, parameter_status_col_kpicode - 1) = scode Or _
               ourTable(i, parameter_status_col_oldcode - 1) = scode Then
                ReDim ourRow(ifc_status_table_maxcol)
                For j = 0 To ifc_status_table_maxcol
                    If j = parameter_status_col_format - 1 Then
                        ourRow(j) = ourTable(i, j)
                    Else
                        ourRow(j) = ourTable(i, j)
                    End If
                Next j
            End If
        Next i


        If Not IsArrayInitialized(ourRow) Then
            getIfcStatusRow = Nothing
            Exit Function
        Else
            getIfcStatusRow = ourRow
        End If
        'Dim i As Integer
        'i = ourRow.row - ourTable.row

        'Set getIfcStatusRow = Range(ourTable.Cells(i + 1, 1), ourTable.Cells(i + 1, 1).offset(0, 6))

        '  Set getIfcStatusRow = ourTable.Worksheet.Range(ourTable.Worksheet.Cells(ourRow.Row, ourTable.Column), _
        '                              ourTable.Worksheet.Cells(ourRow.Row, ourTable.Column + ourTable.Columns.Column + 1))
    End Function
    '** getCodeFormat return Cell for CodeFormat
    Public Function getCodeFormat() As Range
        Dim ourRow() As Object

        ourRow = getIfcStatusRow()
        If Not IsArrayInitialized(ourRow) Then
            'Debug.Print "Status not defined"
            getCodeFormat = Nothing
            Exit Function
        End If

        ' first column to return
        If TypeName(ourRow(parameter_status_col_format - 1)) = "Range" Then
            getCodeFormat = ourRow(parameter_status_col_format - 1)
        Else
            getCodeFormat = Nothing
        End If
    End Function

    '** getCodeColor return ColorCode
    Public Function getCodeColor() As Long
        Dim ourCodeFormat As Range

        ourCodeFormat = getCodeFormat()
        If ourCodeFormat Is Nothing Then
            'Debug.Print "Status not defined"
            getCodeColor = 0
            Exit Function
        End If
        ' first column to return
        getCodeColor = ourCodeFormat.Interior.Color
    End Function

    '** isFreeze returns True if this status is valid as Interface Freeze (positive end of workflow)
    Public Function isFreeze() As Boolean
        Dim ourRow() As Object
        Dim Value As Object


        ourRow = getIfcStatusRow()
        If Not IsArrayInitialized(ourRow) Then
            'Debug.Print "Status not defined"
            isFreeze = False
            Exit Function
        End If

        ' first column to return
        Value = ourRow(parameter_status_col_freeze - 1)
        If IsEmpty(Value) Then
            isFreeze = False
        Else
            isFreeze = True
        End If

    End Function

    '** getCorrespondingIfcStati returns a List of IFC Stati for ICD Stati to use on
    '** returns Nothing if no Status found

    Public Function getCorrespondingIFCStatus() As clsIFStatus
        Dim ICDStatusTable As Range
        Dim aRow As Range
        Dim ifcStatus As New clsIFStatus


        ICDStatusTable = getStatusICDTable()

        For Each aRow In ICDStatusTable.Rows
            If UCase(aRow.Cells(1, 1).Value) = UCase(scode) Then
                ifcStatus.code = aRow.Cells(1, 2)
                getCorrespondingIFCStatus = ifcStatus
                Exit Function
            End If
        Next aRow

        getCorrespondingIFCStatus = Nothing

    End Function
    '** getCorrespondingIfcStati returns a List of IFC Stati for ICD Stati to use on
    '** returns Nothing if no Status found

    Public Function getAllICDStati() As clsIFStatus()
        Dim ICDStatusTable As Range
        Dim aRow As Range
        Dim ifcStatus As New clsIFStatus
        Dim statilist() As clsIFStatus
        Dim i, j As Integer

        ICDStatusTable = getStatusICDTable()
        j = 0

        For Each aRow In ICDStatusTable.Rows
            ReDim Preserve statilist(j)
            statilist(j) = New clsIFStatus
            statilist(j).code = Trim(aRow.Cells(1, 1).Value)
            j = j + 1
        Next aRow

        getAllICDStati = statilist

    End Function

    '** getSuccessorICDStati returns a List of ICD Stati  which can follow this current ICD stati
    '**
    '** return unallocated array if not stati
    Public Function getSuccessorICDStati() As clsIFStatus()
        Dim ICDStatusXTable As Range
        Dim aRow, acell As Range
        Dim Value As Object
        Dim multi() As String
        Dim i, j As Integer
        Dim ifcStatus As New clsIFStatus
        Dim delimiter As String
        Dim statilist() As clsIFStatus


        delimiter = " ,;/|" & vbLf
        j = 0
        ICDStatusXTable = getStatusicdXTable()

        For Each aRow In ICDStatusXTable.Rows
            If UCase(aRow.Cells(1, 1).Value) = UCase(scode) Then
                Value = aRow.Cells(1, 2)
                multi = SplitMultiDelims(text:=Value, DelimChars:=delimiter)
                If IsArrayInitialized(multi) Then
                    For i = 1 To UBound(multi)
                        If InStr(multi(i), delimiter) = 0 Then
                            ReDim Preserve statilist(j)
                            statilist(j) = New clsIFStatus
                            statilist(j).code = Trim(multi(i))
                            j = j + 1
                        End If
                    Next i
                End If
            End If
        Next aRow

        ' return
        If IsArrayInitialized(statilist) Then
            getSuccessorICDStati = statilist
        End If

    End Function

End Class

Public Class clsFCLFCStatus
    '***********************************************************************
    '***** CLASS Forecast LifeCycle status
    '*****


    ' const name of table holding data
    Const parameter_status_FCLFC_table = "parameter_fc_lifecycle_status"

    ' const no of columns in the table to hold status parameter
    Const parameter_status_col_code = 1
    Const parameter_status_col_name = 2
    Const parameter_status_col_alive = 3
    Const parameter_status_col_weight = 4
    Const parameter_status_col_desc = 5
    Const parameter_status_col_kpi = 6    ' special -> take format of this cell

    ' possible codes
    Const fclfcStatus_fullproc = constStatusCode_processed_infos
    Const fclfcStatus_corrproc = constStatusCode_processed_ok
    Const fclfcStatus_skipped = constStatusCode_skipped
    Const fclfcStatus_error = constStatusCode_error
    Const fclfcStatus_null = "null"


    ' Holding the Status
    Private scode As String


    '** initialize
    Public Sub New()
        scode = fclfcStatus_null

    End Sub
    '** get the code
    Public Property code() As String
        Get
            code = scode
        End Get
        Set(NewCode As String)
            Dim ourRow As Range
            Dim oldcode As String

            oldcode = scode    'save it
            scode = clean(newCode)    ' set the new code

            If Not Verify(newCode) Then
                scode = oldcode    'reset
                'Debug.Print "Code was changed to " & newCode & " which is not in parameter_status_FCLFC_table"
                Exit Property
            End If
        End Set
    End Property


    '** get the weight
    ReadOnly Property weight() As Long
        Get
            Dim ourRow() As Object

            ourRow = getFCLFCStatusRow()
            If Not IsArrayInitialized(ourRow) Then
                'Debug.Print "Status not defined"
                weight = -1
                Exit Property
            End If

            ' first column to return
            If IsNumeric(ourRow(parameter_status_col_weight - 1)) Then
                weight = CDec(ourRow(parameter_status_col_weight - 1))
            Else
                weight = -1
            End If

        End Get
    End Property


    '** get the description
    ReadOnly Property description() As String
        Get
            Dim ourRow() As Object

            ourRow = getFCLFCStatusRow()
            If Not IsArrayInitialized(ourRow) Then
                'Debug.Print "Status not defined"
                description = ""
                Exit Property
            End If

            ' first column to return

            description = ourRow(parameter_status_col_desc - 1)
        End Get
    End Property


    ReadOnly Property Name() As String
        Get
            Dim ourRow() As Object

            ourRow = getFCLFCStatusRow()
            If Not IsArrayInitialized(ourRow) Then
                'Debug.Print "Name not defined"
                Name = ""
                Exit Property
            End If

            ' first column to return

            Name = ourRow(parameter_status_col_name - 1)
        End Get
    End Property

    '** isProcessed returns True if this status is an endstatus
    Public Function isalive() As Boolean
        Dim ourRow() As Object
        Dim Value As Object


        ourRow = getFCLFCStatusRow()
        If Not IsArrayInitialized(ourRow) Then
            'Debug.Print "FCLFCStatus not defined"
            isalive = False
            Exit Function
        End If

        ' first column to return
        Value = ourRow(parameter_status_col_alive - 1)
        If IsEmpty(Value) Then
            isalive = False
        Else
            isalive = True
        End If

    End Function
    '** verify code
    Public Function Verify(ByVal newCode As String, Optional exact As Boolean = True) As Boolean
        Dim ourTable(,) As Object
        Dim i, j As Integer

        Verify = False
        ourTable = getFCLFCStatusTable()
        If Not IsArrayInitialized(ourTable) Then
            Verify = False
            Exit Function
        End If

        For i = 0 To fclfc_status_table_maxrow
            If (LCase(ourTable(i, parameter_status_col_code - 1)) = LCase(newCode)) Then
                Verify = True
                Exit Function
            End If
        Next i


    End Function
    Public Function clean(newCode As String)
        clean = LCase(Trim(newCode))
    End Function

    '** getFCLFCStatusTable returns the Status Table
    Public Function getFCLFCStatusTable() As Object

        getFCLFCStatusTable = getFCLFCStatusTable()

    End Function
    '** getFCLFCStatusRow getStatus Row of Table
    '**        searchs through all fields of table !
    Public Function getFCLFCStatusRow() As Object
        Dim ourTable(,) As Object
        Dim ourRow() As Object
        Dim i, j As Integer

        ourTable = getFCLFCStatusTable()
        If Not IsArrayInitialized(ourTable) Then
            getFCLFCStatusRow = Nothing
            Exit Function
        End If

        For i = 0 To fclfc_status_table_maxrow
            If ourTable(i, parameter_status_col_code - 1) = scode Then
                ReDim ourRow(fclfc_status_table_maxcol)
                For j = 0 To fclfc_status_table_maxcol
                    If j = parameter_status_col_kpi - 1 Then
                        ourRow(j) = ourTable(i, j)
                    Else
                        ourRow(j) = ourTable(i, j)
                    End If
                Next j
            End If
        Next i


        If Not IsArrayInitialized(ourRow) Then
            getFCLFCStatusRow = Nothing
            Exit Function
        Else
            getFCLFCStatusRow = ourRow
        End If

    End Function
    '** getCodeFormat return Cell for CodeFormat
    Public Function getCodeFormat() As Range
        Dim ourRow() As Object

        ourRow = getFCLFCStatusRow()
        If Not IsArrayInitialized(ourRow) Then
            'Debug.Print "Status not defined"
            getCodeFormat = Nothing
            Exit Function
        End If

        ' first column to return
        If TypeName(ourRow(parameter_status_col_kpi - 1)) = "Range" Then
            getCodeFormat = ourRow(parameter_status_col_kpi - 1)
        Else
            getCodeFormat = Nothing
        End If
    End Function

    '** getCodeColor return ColorCode
    Public Function getCodeColor() As Long
        Dim ourCodeFormat As Range

        ourCodeFormat = getCodeFormat()
        If ourCodeFormat Is Nothing Then
            'Debug.Print "Status not defined"
            getCodeColor = 0
            Exit Function
        End If
        ' first column to return
        getCodeColor = ourCodeFormat.Interior.Color
    End Function

    '** getFontColor return ColorCode
    Public Function getFontColor() As Long
        Dim ourCodeFormat As Range

        ourCodeFormat = getCodeFormat()
        If ourCodeFormat Is Nothing Then
            'Debug.Print "Status not defined"
            getFontColor = 0
            Exit Function
        End If
        ' first column to return
        getFontColor = ourCodeFormat.Font.Color
    End Function

End Class
Public Class clsMQFStatus

    '***********************************************************************
    '***** CLASS status is a representation class of the message
    '*****

    ' const name of table holding data
    Const parameter_status_mqf_message_table = "parameter_mqf_message_status_table"


    ' const no of columns in the table to hold status parameter
    Const parameter_status_col_code = 1
    Const parameter_status_col_name = 2
    Const parameter_status_col_processed = 3
    Const parameter_status_col_weight = 4
    Const parameter_status_col_desc = 5
    Const parameter_status_col_format = 6    ' special -> take format of this cell



    ' possible codes
    Const fclfcStatus_fullproc = constStatusCode_processed_infos
    Const fclfcStatus_corrproc = constStatusCode_processed_ok
    Const fclfcStatus_skipped = constStatusCode_skipped
    Const fclfcStatus_error = constStatusCode_error
    Const fclfcStatus_forapproval = constStatusCode_forapproval
    Const fclfcStatus_approvalrejected = constStatusCode_approvalrejected

    Const fclfcStatus_null = "null"

    ' Holding the Status
    Private scode As String


    '** initialize
    Public Sub New()
        scode = fclfcStatus_null

    End Sub
    '** get the code
    Public Property code() As String
        Get
            code = scode
        End Get
        Set(newCode As String)
            Dim ourRow As Range
            Dim oldcode As String

            oldcode = scode    'save it
            scode = clean(newCode)    ' set the new code

            If Not Verify(newCode) Then
                scode = oldcode    'reset
                'Debug.Print "Code was changed to " & newCode & " which is not in parameter_status_mqf_message_table"
                Exit Property
            End If
        End Set
    End Property


    '** get the weight
    ReadOnly Property weight() As Long
        Get
            Dim ourRow() As Object

            ourRow = getMQFStatusRow()
            If Not IsArrayInitialized(ourRow) Then
                'Debug.Print "Status not defined"
                weight = -1
                Exit Property
            End If

            ' first column to return
            If IsNumeric(ourRow(parameter_status_col_weight - 1)) Then
                weight = CDec(ourRow(parameter_status_col_weight - 1))
            Else
                weight = -1
            End If
        End Get

    End Property


    '** get the description
    ReadOnly Property description() As String
        Get
            Dim ourRow() As Object

            ourRow = getMQFStatusRow()
            If Not IsArrayInitialized(ourRow) Then
                'Debug.Print "Status not defined"
                description = ""
                Exit Property
            End If

            ' first column to return

            description = ourRow(parameter_status_col_desc - 1)
        End Get

    End Property
    '** get the description
    ReadOnly Property Name() As String
        Get
            Dim ourRow() As Object

            ourRow = getMQFStatusRow()
            If Not IsArrayInitialized(ourRow) Then
                'Debug.Print "Name not defined"
                Name = ""
                Exit Property
            End If

            ' first column to return

            Name = ourRow(parameter_status_col_name - 1)
        End Get
    End Property

    '** isProcessed returns True if this status is an endstatus
    Public Function isProcessed() As Boolean
        Dim ourRow() As Object
        Dim Value As Object


        ourRow = getMQFStatusRow()
        If Not IsArrayInitialized(ourRow) Then
            'Debug.Print "MQFStatus not defined"
            isProcessed = False
            Exit Function
        End If

        ' first column to return
        Value = ourRow(parameter_status_col_processed - 1)
        If IsEmpty(Value) Then
            isProcessed = False
        Else
            isProcessed = True
        End If

    End Function
    '** verify code
    Public Function Verify(ByVal newCode As String, Optional exact As Boolean = True) As Boolean
        Dim ourTable(,) As Object
        Dim i, j As Integer

        Verify = False
        ourTable = getMQFStatusTable()
        If Not IsArrayInitialized(ourTable) Then
            Verify = False
            Exit Function
        End If

        For i = 0 To mqf_status_table_maxrow
            If (LCase(ourTable(i, parameter_status_col_code - 1)) = LCase(newCode)) Then
                Verify = True
                Exit Function
            End If
        Next i


    End Function
    Public Function clean(newCode As String)
        clean = LCase(Trim(newCode))
    End Function


    '** getMQFStatusTable returns the Status Table
    Public Function getMQFStatusTable() As Object

        getMQFStatusTable = getMFQStatusTable()

    End Function
    '** getMQFStatusRow getStatus Row of Table
    '**        searchs through all fields of table !
    Public Function getMQFStatusRow() As Object
        Dim ourTable(,) As Object
        Dim ourRow() As Object
        Dim i, j As Integer

        ourTable = getMQFStatusTable()
        If Not IsArrayInitialized(ourTable) Then
            getMQFStatusRow = Nothing
            Exit Function
        End If

        For i = 0 To mqf_status_table_maxrow
            If ourTable(i, parameter_status_col_code - 1) = scode Then
                ReDim ourRow(mqf_status_table_maxcol)
                For j = 0 To mqf_status_table_maxcol
                    If j = parameter_status_col_format - 1 Then
                        ourRow(j) = ourTable(i, j)
                    Else
                        ourRow(j) = ourTable(i, j)
                    End If
                Next j
            End If
        Next i


        If Not IsArrayInitialized(ourRow) Then
            getMQFStatusRow = Nothing
            Exit Function
        Else
            getMQFStatusRow = ourRow
        End If

    End Function
    '** getCodeFormat return Cell for CodeFormat
    Public Function getCodeFormat() As Range
        Dim ourRow() As Object

        ourRow = getMQFStatusRow()
        If Not IsArrayInitialized(ourRow) Then
            'Debug.Print "Status not defined"
            getCodeFormat = Nothing
            Exit Function
        End If

        ' first column to return
        If TypeName(ourRow(parameter_status_col_format - 1)) = "Range" Then
            getCodeFormat = ourRow(parameter_status_col_format - 1)
        Else
            getCodeFormat = Nothing
        End If
    End Function

    '** getCodeColor return ColorCode
    Public Function getCodeColor() As Long
        Dim ourCodeFormat As Range

        ourCodeFormat = getCodeFormat()
        If ourCodeFormat Is Nothing Then
            'Debug.Print "Status not defined"
            getCodeColor = 0
            Exit Function
        End If
        ' first column to return
        getCodeColor = ourCodeFormat.Interior.Color
    End Function


End Class
