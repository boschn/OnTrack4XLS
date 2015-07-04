
REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE TOOLING 4 EXCEL
REM ***********
REM *********** LEGACY MODULE FOR general Helper functions
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
Imports Microsoft.Office.Interop.Excel

Module modXLSHelper
    ''' <summary>
    ''' returns True if User is CellEditing
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' That is, you check the Interactive property of the Excel.Application object and set it to False. 
    ''' If this causes an exception, it means the user is editing a cell. If there’s no exception, you 
    ''' restore the Interactive. This is required because this MSDN article describes that property as follows:
    ''' True if Microsoft Excel is in interactive mode; this property is usually True. 
    ''' If you set the this property to False, Microsoft Excel will block all input from the keyboard and mouse
    '''  (except input to dialog boxes that are displayed by your code). Read/write Boolean.
    ''' Blocking user input will prevent the user from interfering with the macro as it moves or activates Microsoft Excel objects.
    ''' If you set this property to False, don’t forget to set it back to True. 
    ''' Microsoft Excel won’t automatically set this property back to True when your macro stops running.
    ''' </remarks>
    Function IsEditing() As Boolean
        If Globals.ThisAddIn.Application.Interactive = False Then Return False
        Try
            Globals.ThisAddIn.Application.Interactive = False
            Globals.ThisAddIn.Application.Interactive = True
        Catch
            Return True
        End Try
        Return False
    End Function


    '************************************************************************
    ' addLog : adds a Message to a LogField with vbLF if necessary
    '
    '

    'Function addLog(oldmessage As Object, newMessage As String)

    '    If oldmessage = "" Then
    '        oldmessage = newMessage
    '    ElseIf newMessage <> "" Then
    '        oldmessage = oldmessage & vbLf & newMessage
    '    End If

    '    addLog = oldmessage

    'End Function

    ''********* createPrecode helper to create a Precode out of a PartID in the FORM 3HXX-YYYYYY-000 to 3.HXX
    ''*********
    'Public Function createPrecode(ByVal aPartID As String) As String
    '    createPrecode = Mid(aPartID, 1, 1) & "." & UCase(Mid(aPartID, 2, 3)) & "-"
    'End Function


    '********* createAssycode helper to create a Precode out of a PartID in the FORM 3HXX-YYYYYY-000 to 3.HXX
    '*********
    'Public Function createAssycode(ByVal aPartID As String) As String
    '    createAssycode = Mid(aPartID, 6, 2) & "." & Mid(aPartID, 8, 2) & "." & Mid(aPartID, 10, 2) & ":" & Mid(aPartID, 13, 3)
    'End Function

    '************************************************************************
    ' getDoc9ToolingName  : returns the Name of the Doc#9 Tooling
    '
    '
    ' returns a String

'    Function getDoc9ToolingName(Optional WORKBOOK As Excel.Workbook = Nothing) As String
'        Dim named As Range
'        Dim pn As Name
'        Dim wb As Workbook

'        ' get or set the global doc9
'        If Not IsMissing(WORKBOOK) Then
'            If SetGlobalDoc9(WORKBOOK) Then
'                wb = GetGlobalDoc9()
'            End If
'        End If

'        'exists the name
'        If NameExistsinWorkbook(wb, constDoc9ToolingParameterName) Then
'            pn = wb.Names(constDoc9ToolingParameterName)
'            If pn.ValidWorkbookParameter Then
'                named = pn.RefersToRange
'            End If
'        End If

'        If Not named Is Nothing Then
'            getDoc9ToolingName = named.Value
'        Else
'            getDoc9ToolingName = ""
'        End If

'    End Function

'    '************
'    '************ Doc9isSet returns TRUE if the Global Doc9 ist set
'    '************
'    Function GlobalDoc9isSet() As Boolean
'        On Error GoTo errorhandle
'        If ourSMBDoc9 Is Nothing Then
'            GlobalDoc9isSet = False
'            Exit Function
'        Else
'            If ourSMBDoc9.Name <> "" Then
'                GlobalDoc9isSet = True
'                On Error GoTo 0
'                Exit Function
'            Else
'                GlobalDoc9isSet = False
'                Exit Function
'            End If
'        End If
'errorhandle:
'        GlobalDoc9isSet = False
'        On Error GoTo 0
'    End Function

'    '************
'    '************ SetGlobalDoc9 return true if the Workbook aWorkbook ist set to Global Doc9 or if this exists
'    '************
'    Function SetGlobalDoc9(ByVal aWorkbook As Workbook) As Boolean
'        On Error GoTo errorhandle
'        If GlobalDoc9isSet() Then
'            On Error GoTo 0
'            SetGlobalDoc9 = True
'            Exit Function
'        Else
'            If SheetExistsinWorkbook(aWorkbook, constDoc9StructureSheetName) And _
'               NameExistsinWorkbook(aWorkbook, constdbdoc9structureName) Then
'                ourSMBDoc9 = aWorkbook
'                SetGlobalDoc9 = True
'                On Error GoTo 0
'                Exit Function
'            Else
'                SetGlobalDoc9 = True = False
'                On Error GoTo 0
'                Exit Function
'            End If
'        End If
'errorhandle:
'        SetGlobalDoc9 = False
'        On Error GoTo 0
'    End Function

    '****** getMessageLogTable
    '******
    '****** get the cache of the 2 dimensional ifc status Table

    'Function getMessageLogTable() As Object
    'cache the Range
    'If getMessageLogTable Is Nothing Then
    '    getMessageLogTable = getXLSParameterRangeByName("parameter_messagelog_table")
    'End If

    'If Not ArrayIsInitializedV(mqf_status_table) Then
    '  ReDim mqf_status_table(parameter_status_mqf_message_table.Rows.Count - 1, parameter_status_mqf_message_table.Columns.Count)
    '  Dim i, j As Integer
    '  For i = 0 To parameter_status_mqf_message_table.Rows.Count - 1
    '       For j = 0 To parameter_status_mqf_message_table.Columns.Count - 1
    '          mqf_status_table(i, j) = parameter_status_mqf_message_table(i + 1, j + 1).value
    '       Next j
    '       ' get Range of the format as last one
    '       Set mqf_status_table(i, const_parameter_mqf_status_col_format - 1) = _
    '         parameter_status_mqf_message_table.Cells(i + 1, const_parameter_mqf_status_col_format) ' hardcoded
    '  Next i

    '  mqf_status_table_maxcol = parameter_status_mqf_message_table.Columns.Count ' - 1 Additional one
    '  mqf_status_table_maxrow = parameter_status_mqf_message_table.Rows.Count - 1
    ' End If

    'getMFQStatusTable = mqf_status_table
    'End Function

    Function Union2(ParamArray Ranges() As Object) As Range
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' Union2
        ' A Union operation that accepts parameters that are Nothing.
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim n As Long
        Dim RR As Range
        For n = LBound(Ranges) To UBound(Ranges)

            If Not Ranges(n) Is Nothing Then
                If TypeOf Ranges(n) Is Excel.Range Then
                    If Not RR Is Nothing Then
                        RR = Globals.ThisAddIn.Application.Union(RR, Ranges(n))
                    Else
                        RR = Ranges(n)
                    End If
                End If
            End If

        Next n
        Union2 = RR
    End Function

    Function ProperUnion(ParamArray Ranges() As Object) As Range
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' ProperUnion
        ' This provides Union functionality without duplicating
        ' cells when ranges overlap. Requires the Union2 function.
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim ResR As Range
        Dim n As Long
        Dim r As Range

        If Not Ranges(LBound(Ranges)) Is Nothing Then
            ResR = Ranges(LBound(Ranges))
        End If
        For n = LBound(Ranges) + 1 To UBound(Ranges)
            If Not Ranges(n) Is Nothing Then
                For Each r In Ranges(n).Cells
                    If Globals.ThisAddIn.Application.Intersect(ResR, r) Is Nothing Then
                        ResR = Union2(ResR, r)
                    End If
                Next r
            End If
        Next n
        ProperUnion = ResR

    End Function

    '****** getMFQStatusTable
    '******
    '****** get the cache of the 2 dimensional ifc status Table

    'Function getMFQStatusTable() As Object
    '    'cache the Range
    '    If parameter_status_mqf_message_table Is Nothing Then
    '        parameter_status_mqf_message_table = GetXlsParameterRangeByName("parameter_mqf_message_status_table")
    '    End If

    '    If Not IsArrayInitialized(mqf_status_table) Then
    '        ReDim mqf_status_table(parameter_status_mqf_message_table.Rows.Count - 1, parameter_status_mqf_message_table.Columns.Count)
    '        Dim i, j As Integer
    '        For i = 0 To parameter_status_mqf_message_table.Rows.Count - 1
    '            For j = 0 To parameter_status_mqf_message_table.Columns.Count - 1
    '                mqf_status_table(i, j) = parameter_status_mqf_message_table(i + 1, j + 1).Value
    '            Next j
    '            ' get Range of the format as last one
    '            mqf_status_table(i, const_parameter_mqf_status_col_format - 1) = _
    '            parameter_status_mqf_message_table.Cells(i + 1, const_parameter_mqf_status_col_format)    ' hardcoded
    '        Next i

    '        mqf_status_table_maxcol = parameter_status_mqf_message_table.Columns.Count    ' - 1 Additional one
    '        mqf_status_table_maxrow = parameter_status_mqf_message_table.Rows.Count - 1
    '    End If

    '    getMFQStatusTable = mqf_status_table
    'End Function


    '****** get parameterstatusTable
    '******
    '****** get the cache of the 2 dimensional ifc status Table

    'Function getIFStatusTable() As Object
    '    'cache the Range
    '    If parameter_status_ifc_table Is Nothing Then
    '        parameter_status_ifc_table = GetXlsParameterRangeByName("parameter_status_interface_table")
    '    End If
    '    If Not IsArrayInitialized(ifc_status_table) Then
    '        ReDim ifc_status_table(parameter_status_ifc_table.Rows.Count - 1, parameter_status_ifc_table.Columns.Count)
    '        Dim i, j As Integer
    '        For i = 0 To parameter_status_ifc_table.Rows.Count - 1
    '            For j = 0 To parameter_status_ifc_table.Columns.Count - 1
    '                ifc_status_table(i, j) = parameter_status_ifc_table(i + 1, j + 1).Value
    '            Next j
    '            ' get Range of the format as last one
    '            ifc_status_table(i, const_parameter_IF_status_col_format - 1) = _
    '            parameter_status_ifc_table.Cells(i + 1, const_parameter_IF_status_col_code)
    '        Next i
    '        ifc_status_table_maxcol = parameter_status_ifc_table.Columns.Count    ' - 1 Additional one
    '        ifc_status_table_maxrow = parameter_status_ifc_table.Rows.Count - 1
    '    End If

    '    getIFStatusTable = ifc_status_table
    'End Function
    '****** get parameterstatusICDTable
    '******

    'Function getStatusICDTable() As Range
    '    'cache the Range
    '    If parameter_status_icd_table Is Nothing Then
    '        parameter_status_icd_table = GetXlsParameterRangeByName("parameter_status_icd_table")
    '    End If
    '    getStatusICDTable = parameter_status_icd_table
    'End Function
    '****** get parameterstatusXTable
    '******

    'Function getStatusicdXTable() As Range
    '    'cache the Range
    '    If parameter_status_icd_x_table Is Nothing Then
    '        parameter_status_icd_x_table = GetXlsParameterRangeByName("parameter_status_icd_x_table")
    '    End If
    '    getStatusicdXTable = parameter_status_icd_x_table
    'End Function
    '****** getFCLFCStatusTable:
    '******
    '****** get the cache of the 2 dimensional Forecast Lifecycle status Table

    'Function getFCLFCStatusTable() As Object
    '    'cache the Range
    '    If parameter_status_FCLFC_table Is Nothing Then
    '        parameter_status_FCLFC_table = GetXlsParameterRangeByName("parameter_fc_lifecycle_status")
    '    End If

    '    If Not IsArrayInitialized(fclfc_status_table) Then
    '        ReDim fclfc_status_table(parameter_status_FCLFC_table.Rows.Count - 1, parameter_status_FCLFC_table.Columns.Count)
    '        Dim i, j As Integer
    '        For i = 0 To parameter_status_FCLFC_table.Rows.Count - 1
    '            For j = 0 To parameter_status_FCLFC_table.Columns.Count - 1
    '                fclfc_status_table(i, j) = parameter_status_FCLFC_table(i + 1, j + 1).Value
    '            Next j
    '            ' get Range of the format as last one
    '            fclfc_status_table(i, const_parameter_fclfc_status_col_kpi - 1) = _
    '            parameter_status_FCLFC_table.Cells(i + 1, const_parameter_fclfc_status_col_kpi)    ' hardcoded
    '        Next i

    '        fclfc_status_table_maxcol = parameter_status_FCLFC_table.Columns.Count    ' - 1 Additional one
    '        fclfc_status_table_maxrow = parameter_status_FCLFC_table.Rows.Count - 1
    '    End If

    '    getFCLFCStatusTable = fclfc_status_table
    'End Function

    '****** getProcessStatusTable:
    '******
    '****** get the cache of the 2 dimensional Forecast Lifecycle status Table

    'Function getProcessStatusTable() As Object
    '    'cache the Range
    '    If parameter_status_Process_table Is Nothing Then
    '        parameter_status_Process_table = GetXlsParameterRangeByName("parameter_process_status")
    '    End If

    '    If Not IsArrayInitialized(Process_status_table) Then
    '        ReDim Process_status_table(parameter_status_Process_table.Rows.Count - 1, parameter_status_Process_table.Columns.Count)
    '        Dim i, j As Integer
    '        For i = 0 To parameter_status_Process_table.Rows.Count - 1
    '            For j = 0 To parameter_status_Process_table.Columns.Count - 1
    '                Process_status_table(i, j) = parameter_status_Process_table(i + 1, j + 1).Value
    '            Next j
    '            ' get Range of the format as last one
    '            Process_status_table(i, const_parameter_process_Status_col_kpi - 1) = _
    '            parameter_status_Process_table.Cells(i + 1, const_parameter_process_Status_col_kpi)    ' hardcoded
    '        Next i

    '        Process_status_table_maxcol = parameter_status_Process_table.Columns.Count    ' - 1 Additional one
    '        Process_status_table_maxrow = parameter_status_Process_table.Rows.Count - 1
    '    End If

    '    getProcessStatusTable = Process_status_table
    'End Function

    '****** getDMUStatusTable:
    '******
    '****** get the cache of the 2 dimensional DMU status Table

    'Function getDMUStatusTable() As Object
    '    'cache the Range
    '    If parameter_status_DMU_table Is Nothing Then
    '        parameter_status_DMU_table = getXLSParameterRangeByName("parameter_dmu_status")
    '    End If

    '    If Not IsArrayInitialized(DMU_status_table) Then
    '        ReDim DMU_status_table(parameter_status_DMU_table.Rows.count - 1, parameter_status_DMU_table.Columns.count)
    '        Dim i, j As Integer
    '        For i = 0 To parameter_status_DMU_table.Rows.count - 1
    '            For j = 0 To parameter_status_DMU_table.Columns.count - 1
    '                DMU_status_table(i, j) = parameter_status_DMU_table(i + 1, j + 1).Value
    '            Next j
    '            ' get Range of the format as last one
    '            DMU_status_table(i, const_parameter_DMU_Status_col_kpi - 1) = _
    '            parameter_status_DMU_table.Cells(i + 1, const_parameter_DMU_Status_col_kpi)    ' hardcoded
    '        Next i

    '        DMU_status_table_maxcol = parameter_status_DMU_table.Columns.count    ' - 1 Additional one
    '        DMU_status_table_maxrow = parameter_status_DMU_table.Rows.count - 1
    '    End If

    '    getDMUStatusTable = DMU_status_table
    'End Function

    '****** getFEMStatusTable:
    '******
    '****** get the cache of the 2 dimensional FEM status Table

    'Function getFEMStatusTable() As Object
    '    'cache the Range
    '    If parameter_status_FEM_table Is Nothing Then
    '        parameter_status_FEM_table = getXLSParameterRangeByName("parameter_FEM_status")
    '    End If

    '    If Not IsArrayInitialized(FEM_status_table) Then
    '        ReDim FEM_status_table(parameter_status_FEM_table.Rows.count - 1, parameter_status_FEM_table.Columns.count)
    '        Dim i, j As Integer
    '        For i = 0 To parameter_status_FEM_table.Rows.count - 1
    '            For j = 0 To parameter_status_FEM_table.Columns.count - 1
    '                FEM_status_table(i, j) = parameter_status_FEM_table(i + 1, j + 1).Value
    '            Next j
    '            ' get Range of the format as last one
    '            FEM_status_table(i, const_parameter_FEM_Status_col_kpi - 1) = _
    '            parameter_status_FEM_table.Cells(i + 1, const_parameter_FEM_Status_col_kpi)    ' hardcoded
    '        Next i

    '        FEM_status_table_maxcol = parameter_status_FEM_table.Columns.count    ' - 1 Additional one
    '        FEM_status_table_maxrow = parameter_status_FEM_table.Rows.count - 1
    '    End If

    '    getFEMStatusTable = FEM_status_table
    'End Function
    '************
    '************ GetGlobalDoc9 return the Doc9 Workbook or Nothing if not set
    '************
'    Function GetGlobalDoc9() As Workbook
'        On Error GoTo errorhandle
'        If GlobalDoc9isSet() Then
'            On Error GoTo 0
'            GetGlobalDoc9 = ourSMBDoc9
'            Exit Function
'        Else
'            GetGlobalDoc9 = Nothing
'            On Error GoTo 0
'        End If
'errorhandle:
'        GetGlobalDoc9 = Nothing
'        On Error GoTo 0
'    End Function
    '************
    '************ SheetExistsinWorkbook return true if the Sheet <aName> exists in the Workbook aWorkbook
    '************
    Function SheetExistsinWorkbook(ByVal aWorkbook As Workbook, ByVal aName As String) As Boolean

        Try
            If Not aWorkbook.Sheets(aName) Is Nothing Then
                Return True
            End If
        Catch ex As System.Runtime.InteropServices.COMException
            SheetExistsinWorkbook = False

        End Try

        Return False
    End Function

    ''' <summary>
    ''' CacheDictionary for the named ranges
    ''' </summary>
    ''' <remarks></remarks>
    Public namedRangesCacheTable As Dictionary(Of String, String) = New Dictionary(Of String, String)

    ''' <summary>
    ''' returns True if a given Name exists in the Notebook
    ''' </summary>
    ''' <param name="aWorkbook"></param>
    ''' <param name="aName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function NameExistsinWorkbook(ByVal aWorkbook As Workbook, ByVal aName As String) As Boolean
        Try
            For Each name As Microsoft.Office.Interop.Excel.Name In aWorkbook.Names
                If LCase(name.Name) = LCase(aName) Then
                    Return True
                End If
            Next
            Return False
        Catch ex As System.Runtime.InteropServices.COMException
            Return False
        End Try
        Return False
    End Function
    ''' <summary>
    ''' Cache all Property Names in a workbook
    ''' </summary>
    ''' <param name="WORKBOOK"></param>
    ''' <remarks></remarks>
    Sub CacheAllWorkbookNames(ByVal [workbook] As Excel.Workbook)
        Dim i As UShort
        For Each aName As Microsoft.Office.Interop.Excel.Name In [workbook].Names

            If namedRangesCacheTable.ContainsKey(Globals.ThisAddIn.Application.Name & "." & [workbook].Name & "." & aName.Name) Then

            ElseIf CType(workbook.Names(i), Name).ValidWorkbookParameter Then
                namedRangesCacheTable.Add(key:=Globals.ThisAddIn.Application.Name & "." & [workbook].Name & "." & aName.Name, _
                                 value:=CType(workbook.Names(i), Name).RefersToRange.Address)
            End If

            i += 1
        Next
    End Sub

    '************
    '************ NameExistsinWorkbook return true if the Name <aName> exists in the Workbook aWorkbook
    '************
    Function NameInWorkbook(ByVal aWorkbook As Workbook, ByVal aName As String) As Microsoft.Office.Interop.Excel.Name
        Try
            For Each name As Microsoft.Office.Interop.Excel.Name In aWorkbook.Names
                If LCase(name.Name) = LCase(aName) Then
                    Return name
                End If
            Next

            Return Nothing
        Catch ex As Exception

        End Try
        Return Nothing
    End Function
    '************
    '************ NameExistsinApplication return true if the Name <aName> exists in the Globals.ThisAddin.Application
    '************
    Function NameinApplication(ByVal aName As String) As Microsoft.Office.Interop.Excel.Name
        Try
            If Not Globals.ThisAddIn.Application.Names.Item(aName) Is Nothing Then
                Return Globals.ThisAddIn.Application.Names.Item(aName)
            End If
            Return Nothing
        Catch ex As System.Runtime.InteropServices.COMException
            For Each name As Microsoft.Office.Interop.Excel.Name In Globals.ThisAddIn.Application.Names

                If LCase(name.Name) = LCase(aName) Then

                    Return name
                End If
            Next
            Return Nothing
        End Try

        Return Nothing
    End Function
    '************
    '************ NameExistsinApplication return true if the Name <aName> exists in the Globals.ThisAddin.Application
    '************
    Function NameExistsinApplication(ByVal aName As String) As Boolean
        Try
            For Each name As Microsoft.Office.Interop.Excel.Name In Globals.ThisAddIn.Application.Names
                If LCase(name.Name) = LCase(aName) Then
                    Return True
                End If
            Next
            Return False

        Catch ex As System.Runtime.InteropServices.COMException
            Return False
        End Try

        Return False
    End Function
    '************
    '************ NameExistsinWorksheet return true if the Name <aName> exists in the Worksheet aWorksheet
    '************
    Function NameExistsinWorksheet(ByVal aWorksheet As Worksheet, ByVal aName As String) As Boolean

        Try
            For Each name As String In aWorksheet.Names
                If LCase(name) = LCase(aName) Then
                    Return True
                End If
            Next

            Return False

        Catch ex As System.Runtime.InteropServices.COMException


            Return False
        End Try
        Return False
    End Function
    '************
    '************ NameExistsinWorksheet return true if the Name <aName> exists in the Worksheet aWorksheet
    '************
    Function NameinWorksheet(ByVal aWorksheet As Worksheet, ByVal aName As String) As Microsoft.Office.Interop.Excel.Name
        Try
            For Each name As Microsoft.Office.Interop.Excel.Name In aWorksheet.Names
                If LCase(name.Name) = LCase(aName) Then
                    Return name
                End If
            Next
            Return Nothing
        Catch ex As System.Runtime.InteropServices.COMException
            Return Nothing
        End Try

        Return Nothing
    End Function

    '************************************************************************
    ' getParameterByNameasArray : returns Parameter by Name in the order XLS, Workbook, Worksheet
    '                             Fetches errors like Worksheet is missing or ParameterName not Defined
    ' Optional aWorkbook to look in
    ' Optional found to indicate if parameter exists -> return value
    ' silent if true than do not issue error
    ' returns a Variant()

    'Function getParameterFieldArray(ByVal Name As String, _
    '                                Optional aWorkbook As Excel.Workbook = Nothing, _
    '                                Optional found As Boolean = False, _
    '                                Optional silent As Boolean = False) As Object

    '    Dim multi() As String
    '    Dim Value As Object
    '    Dim n, i, j As Integer
    '    Dim dbdesc() As xlsDBDesc
    '    Dim dateids() As String

    '    ' parameters
    '    If Not getDBDesc(dbdesc) Then
    '        getParameterFieldArray = Nothing
    '        Exit Function
    '    End If

    '    Value = GetXlsParameterByName(Name, aWorkbook, found, silent)
    '    If Value = "" Then
    '        getParameterFieldArray = Nothing
    '        Exit Function
    '    End If

    '    ' check through
    '    multi = SplitMultiDelims(text:=Value, DelimChars:=constDelimeter)
    '    n = 0
    '    If IsArrayInitialized(multi) Then

    '        For i = 1 To UBound(multi)
    '            If InStr(multi(i), constDelimeter) = 0 Then
    '                multi(i) = LCase(Trim(multi(i)))
    '                ' order in crossreference dateindex
    '                For j = 0 To UBound(dbdesc)
    '                    If LCase(dbdesc(j).ID) = multi(i) Then
    '                        'ReDim Preserve dateindex(n)
    '                        ReDim Preserve dateids(n)
    '                        'dateindex(n) = j
    '                        dateids(n) = dbdesc(j).ID
    '                        n = n + 1
    '                        Exit For
    '                    End If
    '                Next j
    '            End If
    '        Next i
    '    Else
    '        getParameterFieldArray = Nothing
    '        Exit Function
    '    End If

    '    getParameterFieldArray = dateids
    'End Function

    '************************************************************************
    ' getDoc9FieldArray : returns a list of valid IDs from a String
    '                             Fetches errors like Worksheet is missing or ParameterName not Defined

    'Function getDoc9FieldArray(ByVal Value As String) As Object

    '    Dim multi() As String
    '    'Dim value As Variant
    '    Dim n, i, j As Integer
    '    Dim dbdesc() As xlsDBDesc
    '    Dim dateids() As String
    '    Dim emptya() As String


    '    ' parameters
    '    If Not getDBDesc(dbdesc) Then
    '        getDoc9FieldArray = emptya
    '        Exit Function
    '    End If

    '    If Value = "" Then
    '        getDoc9FieldArray = emptya
    '        Exit Function
    '    End If

    '    ' check through
    '    multi = SplitMultiDelims(text:=Value, DelimChars:=constDelimeter)
    '    n = 0
    '    If IsArrayInitialized(multi) Then

    '        For i = 1 To UBound(multi)
    '            If InStr(multi(i), constDelimeter) = 0 Then
    '                multi(i) = LCase(Trim(multi(i)))
    '                ' order in crossreference dateindex
    '                For j = 0 To UBound(dbdesc)
    '                    If LCase(dbdesc(j).ID) = multi(i) Then
    '                        'ReDim Preserve dateindex(n)
    '                        ReDim Preserve dateids(n)
    '                        'dateindex(n) = j
    '                        dateids(n) = dbdesc(j).ID
    '                        n = n + 1
    '                        Exit For
    '                    End If
    '                Next j
    '            End If
    '        Next i
    '    Else
    '        getDoc9FieldArray = emptya
    '        Exit Function
    '    End If

    '    getDoc9FieldArray = dateids
    'End Function



    '************************************************************************
    '    getXLSColumn is a Helper function to get the Column No by Integer from the Parameter in 'A' Notation
    '
    ' ParameterName is the Name of the Column Parameter
    ' db is the Range of the Database to reference
    '
    ' return the Column or -1 if there is none
    '
    Function getXLSColumn(ByVal ParameterName As String, ByVal db As Range, Optional aWorkbook As Excel.Workbook = Nothing) As Integer
        Dim Value As String

        Value = GetXlsParameterByName(ParameterName, aWorkbook)
        Try



            If Value <> "" And Not db Is Nothing Then
                getXLSColumn = db.Range(Value & "1").Column
                Exit Function
            End If
        Catch ex As Exception

            getXLSColumn = -1
        End Try



    End Function



    '*********
    '********* cvtCar returns boolean if value is set to something indicating this is meant to be a cartype set
    '*********
    '********* value as Variant
    '*********
    'Public Function cvtCar(Value As Object) As Boolean
    '    If Len(Value) > 0 And Len(Globals.ThisAddIn.Application.WorksheetFunction.Trim(Value)) = 0 Then
    '        Value = Globals.ThisAddIn.Application.WorksheetFunction.Trim(Value)
    '        'Debug.Print "value is whitechar"
    '    End If
    '    cvtCar = Not Value = ""
    'End Function

    '*********
    '********* pasteFormat from Source Range to Target Range
    '*********
    '*********
    '*********
    Public Sub pasteFormat(source As Range, TARGET As Range)
        source.Copy()
        TARGET.PasteSpecial(Paste:=Excel.XlPasteType.xlPasteFormats, Operation:=Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone, SkipBlanks:=False, Transpose:=False)
        Globals.ThisAddIn.Application.CutCopyMode = False

    End Sub

    '*********
    '********* pasteValue from Source Range to Target Range
    '*********
    '*********
    '*********
    Public Sub pasteValue(Value As Object, format As Range, TARGET As Range)
        pasteFormat(format, TARGET)
        TARGET.Value = Value
    End Sub

    '*********
    '********* pasteFormat from Source Range to Target Range
    '*********
    '*********
    '*********
    Public Sub copyComment(ByVal source As Range, ByVal TARGET As Range)
        Dim sComment As COMMENT
        Dim tcomment As COMMENT

        If source.Comment Is Nothing Then
            Exit Sub
        End If

        ' no comment in Target
        If Not TARGET.Comment Is Nothing Then
            TARGET.ClearComments()
        End If

        ' Add Text
        TARGET.AddComment(source.Comment.Text)
        With TARGET.Comment
            .Shape.Height = source.Comment.Shape.Height
            .Shape.Width = source.Comment.Shape.Width
            .Shape.TextFrame.AutoSize = source.Comment.Shape.TextFrame.AutoSize
        End With
    End Sub



    Function FindAll(searchrange As Range, _
                     FindWhat As Object, _
                     Optional LookIn As XlFindLookIn = Excel.XlFindLookIn.xlValues, _
                     Optional LookAt As XlLookAt = Excel.XlLookAt.xlWhole, _
                     Optional SearchOrder As XlSearchOrder = Excel.XlSearchOrder.xlByRows, _
                     Optional MatchCase As Boolean = False, _
                     Optional BeginsWith As String = vbNullString, _
                     Optional EndsWith As String = vbNullString, _
                     Optional BeginEndCompare As CompareMethod = CompareMethod.Text) As Range
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' FindAll
        ' This searches the range specified by SearchRange and returns a Range object
        ' that contains all the cells in which FindWhat was found. The search parameters to
        ' this function have the same meaning and effect as they do with the
        ' Range.Find method. If the value was not found, the function return Nothing. If
        ' BeginsWith is not an empty string, only those cells that begin with BeginWith
        ' are included in the result. If EndsWith is not an empty string, only those cells
        ' that end with EndsWith are included in the result. Note that if a cell contains
        ' a single word that matches either BeginsWith or EndsWith, it is included in the
        ' result.  If BeginsWith or EndsWith is not an empty string, the LookAt parameter
        ' is automatically changed to xlPart. The tests for BeginsWith and EndsWith may be
        ' case-sensitive by setting BeginEndCompare to vbBinaryCompare. For case-insensitive
        ' comparisons, set BeginEndCompare to vbTextCompare. If this parameter is omitted,
        ' it defaults to vbTextCompare. The comparisons for BeginsWith and EndsWith are
        ' in an OR relationship. That is, if both BeginsWith and EndsWith are provided,
        ' a match if found if the text begins with BeginsWith OR the text ends with EndsWith.
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Dim FoundCell As Range
        Dim FirstFound As Range
        Dim LastCell As Range
        Dim ResultRange As Range
        Dim XLookAt As XlLookAt
        Dim Include As Boolean
        Dim CompMode As CompareMethod
        Dim area As Range
        Dim maxrow As Long
        Dim MaxCol As Long
        Dim BeginB As Boolean
        Dim EndB As Boolean


        CompMode = BeginEndCompare
        If BeginsWith <> vbNullString Or EndsWith <> vbNullString Then
            XLookAt = Excel.XlLookAt.xlPart
        Else
            XLookAt = LookAt
        End If

        ' this loop in Areas is to find the last cell
        ' of all the areas. That is, the cell whose row
        ' and column are greater than or equal to any cell
        ' in any Area.

        For Each area In searchrange.Areas
            With area
                If .Cells(.Cells.Count).row > maxrow Then
                    maxrow = .Cells(.Cells.Count).row
                End If
                If .Cells(.Cells.Count).Column > MaxCol Then
                    MaxCol = .Cells(.Cells.Count).Column
                End If
            End With
        Next area
        LastCell = searchrange.Worksheet.Cells(maxrow, MaxCol)

        On Error GoTo 0
        FoundCell = searchrange.Find(What:=FindWhat, _
                                         After:=LastCell, _
                                         LookIn:=LookIn, _
                                         LookAt:=XLookAt, _
                                         SearchOrder:=SearchOrder, _
                                         MatchCase:=MatchCase)

        If Not FoundCell Is Nothing Then
            FirstFound = FoundCell
            Do Until False    ' Loop forever. We'll "Exit Do" when necessary.
                Include = False
                If BeginsWith = vbNullString And EndsWith = vbNullString Then
                    Include = True
                Else
                    If BeginsWith <> vbNullString Then
                        If StrComp(Left(FoundCell.Text, Len(BeginsWith)), BeginsWith, BeginEndCompare) = 0 Then
                            Include = True
                        End If
                    End If
                    If EndsWith <> vbNullString Then
                        If StrComp(Right(FoundCell.Text, Len(EndsWith)), EndsWith, BeginEndCompare) = 0 Then
                            Include = True
                        End If
                    End If
                End If
                If Include = True Then
                    If ResultRange Is Nothing Then
                        ResultRange = FoundCell
                    Else
                        ResultRange = Globals.ThisAddIn.Application.Union(ResultRange, FoundCell)
                    End If
                End If
                FoundCell = searchrange.FindNext(After:=FoundCell)
                If (FoundCell Is Nothing) Then
                    Exit Do
                End If
                If (FoundCell.Address = FirstFound.Address) Then
                    Exit Do
                End If

            Loop
        End If

        FindAll = ResultRange

    End Function

    '*******************************************************************************
    '********** Returns the Row in an Range which is lower or equal as the key
    '**********
    '********** -1 if every value is greater than

    Function getRowLE(anArea As Range, aKey As Object) As Long
        Dim anArray() As Object
        Dim Value As Object
        Dim i As Long
        Dim k As Integer
        Dim c As Integer
        Dim row As Long


        i = 0
        For Each Value In anArea.Cells
            ReDim Preserve anArray(i)
            anArray(i) = Value & ":" & Value.row
            i = i + 1
        Next Value

        Array.Sort(anArray)

        ' find LL than KEY
        i = getLL(anArray, aKey, 0, UBound(anArray))

        If i < 0 Then
            getRowLE = -1
        Else
            Value = anArray(i)
            k = InStr(Value, ":")
            row = CLng(Mid(Value, k + 1, Len(Value)))
            ' return
            getRowLE = row
        End If

    End Function

    '******* returns -1 -> beginning

    Function getLL(anArray() As Object, aKey As Object, lb As Long, ub As Long) As Long
        Dim Value As Object
        Dim i As Long
        Dim k As Integer
        Dim c1, c2, c As Integer

        ' exit recursion
        If (ub - lb) <= 1 Then
            k = InStr(anArray(lb), ":")
            Value = Mid(anArray(lb), 1, k - 1)
            c1 = StrComp(Value, aKey)
            k = InStr(anArray(ub), ":")
            Value = Mid(anArray(ub), 1, k - 1)
            c2 = StrComp(Value, aKey)

            ' greater than lower bound -> return lower bound
            If c1 < 0 Then
                getLL = lb
                Exit Function
            ElseIf c2 < 0 Then
                getLL = ub
                Exit Function
            Else
                getLL = lb + 1
                'Debug.Print "debug getll :" & c1 & " - " & c2
                Exit Function
            End If
            ' search over half
        Else
            ' get bound test the against upper bound
            i = ub
            Value = anArray(i)
            k = InStr(Value, ":")
            c = StrComp(Mid(Value, 1, k - 1), aKey)

            If c <= 0 Then
                getLL = ub
                Exit Function
            End If

            ' get bound test the against lower bound
            i = lb
            Value = anArray(i)
            k = InStr(Value, ":")
            c = StrComp(Mid(Value, 1, k - 1), aKey)

            If c >= 0 Then
                getLL = lb - 1    ' -1 if lb = 0
                Exit Function
            End If

            ' get bound test the middle
            i = lb + (ub - lb) / 2
            Value = anArray(i)
            k = InStr(Value, ":")
            c = StrComp(Mid(Value, 1, k - 1), aKey)

            If c < 0 Then
                getLL = getLL(anArray, aKey, i, ub)
                Exit Function
            ElseIf c > 0 Then
                getLL = getLL(anArray, aKey, lb, i)
                Exit Function
            Else
                getLL = lb - 1
            End If

        End If

    End Function


    '***
    '*** convertColLetter2Number
    '***

    Function convertColLetter2Number(ColumnLetter As String) As Long
        Dim i As Integer
        Dim c As String
        Dim n As Long

        ColumnLetter = UCase(ColumnLetter)

        For i = 0 To Len(ColumnLetter) - 1
            c = Mid(ColumnLetter, Len(ColumnLetter) - i, 1)
            n = n + (Asc(c) - 64) * Globals.ThisAddIn.Application.WorksheetFunction.Power(26, i)
        Next i

        convertColLetter2Number = n
    End Function


End Module
