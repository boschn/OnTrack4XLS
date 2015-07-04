
REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE TOOLING 4 EXCEL
REM ***********
REM *********** MESSAGE QUEUE FILE MODULE static functions
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
Imports Microsoft.Office.Tools.Excel
Imports Microsoft.Office.Interop.Excel
Imports System.ComponentModel

Imports OnTrack.UI
Imports OnTrack.XChange
Imports OnTrack.Database
Imports OnTrack.Core
Imports OnTrack

''' <summary>
''' Module for Excel message Queue File functions
''' </summary>
''' <remarks></remarks>
Module modXLSMessageQueueFile

    Public Const ConstXLSNullValue As String = "-"

    ' const
    Public Const constMQFClearFieldChar As String = "-"
    Public Const constMQFActionID As String = "mqfxaction"
    Public Const constMQFHeaderidName As String = "mqf_headerid"

    Public Const constMQFConfigName As UShort = 0
    Public Const constMQFConfigCopyOldValues As UShort = 1
    Public Const constMQFConfigFields As UShort = 2
    Public Const constMQFConfigROFields As UShort = 3
    Public Const constMQFConfigDesc As UShort = 4

    Public Const constMQFFieldheader As UShort = 0
    Public Const constMQFfieldname As UShort = 1
    Public Const constMQFFieldColumnWidth As UShort = 2
    Public Const constMQFFieldColumn As UShort = 3

    Public Const constMQFCT_ALL As UShort = 0
    Public Const constMQFCT_Pre As UShort = 1
    Public Const constMQFCT_Post As UShort = 2

    Public Const constMQFDescFieldID As UShort = 0
    Public Const constMQFDescTitle As UShort = 1
    Public Const constMQFDescColumnNo As UShort = 2
    Public Const constMQFReadonly As UShort = 3
    Public Const constMQFObjectName As UShort = 4

    Public Const constMQFOperation_CHANGE As String = "change"
    Public Const constMQFOperation_NOOP As String = "noop"
    Public Const constMQFOperation_ADDAFTER As String = "add-after"
    Public Const constMQFOperation_DELETE As String = "delete"
    Public Const constMQFOperation_FREEZE As String = "freeze"
    Public Const constMQFOperation_REVISION As String = "add-revision"


    '****
    '**** Declare the config definition of the MQF
    '****

    Public Structure MQFConfig

        Public Name As String
        Public desc As String
        Public FieldIDs As String()    'Array of Names
        Public FieldROIDs As Object    'Array of Read-Only Names
        Public copyOldValues As Boolean


    End Structure


    ''' <summary>
    ''' structure for describing Excel Database MQF Columns
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure XLSMQFColumnDescription

        Public ID As String
        Public Title As String
        Public ColumnNo As Integer
        Public READ_ONLY As Boolean
        Public otdbDesc As xlsDBDesc
        Public mqfversion As String
        Public Objectname As String
    End Structure


    ''' <summary>
    ''' structure for describing an MQF File
    ''' </summary>
    ''' <remarks></remarks>

    Public Structure XLSMQFStructure

        Public desc() As XLSMQFColumnDescription
        Public UIDCol As Long

        Public ActionCol As Long
        Public dbrange As Range
        Public processstatuscol As Long
        Public processlogcol As Long
        Public processdatecol As Long
        Public processchangetag As String

        Public requestedBy As String
        Public requestedbyDept As String
        Public requestedOn As Date

        Public Title As String
        Public Requestfor As String

        Public workspaceID As String
        Public XCHANGECONFIG As XChangeConfiguration

    End Structure
    '***
    '*** Message Definition
    '***

    Public Structure MQFMessage
        Public action As String
        Public UID As Long
        Public fieldvalues As Object    ' Array of Values

        'Public status As clsMQFStatus  ' Status of the Message
        Public log As String           ' Message process log
        Public processable As Boolean  ' Message is processabel / should be processed
        Public tobeapproved As Boolean    ' Message has to be approved
        Public isApproved As Boolean   ' Message was approved
        Public processDate As Date            ' Process Date
        Public changetag As String     ' Tag of the Change for field

        Public fieldlog As Object    ' Array of Log messages per Field - same index as values
        Public fieldstatus As Object    ' Array of Stati per Field - same index as values

    End Structure

    '**********
    '**********  getMQFConfigfields : Split fields-ID in the fields()
    '**********

    'Private Function getMQFConfigFields(fields As String) As String()

    '    Dim allfields As Object
    '    Dim Value As Object
    '    Dim multi() As String
    '    Dim i, j As Integer
    '    Dim delimiter As String
    '    Dim theFields() As String
    '    Dim n As Integer


    '    ' check all Fields
    '    allfields = getDBDescIDs()
    '    If Not IsArrayInitialized(allfields) Then
    '        Call CoreMessageHandler(showmsgbox:=True, message:="Abort: Header Field Description / name headerids is not found ", _
    '                               procedure:="modXLSMessageQueueFile.getMQFConfigFields", messagetype:=otCoreMessageType.ApplicationError, break:=False)

    '        Exit Function
    '    End If

    '    delimiter = ","
    '    j = 0

    '    multi = SplitMultiDelims(text:=fields, DelimChars:=delimiter)
    '    If IsArrayInitialized(multi) Then
    '        For i = 1 To UBound(multi)
    '            If InStr(multi(i), delimiter) = 0 Then
    '                Value = Trim(multi(i))
    '                If Value <> "" Then
    '                    ' wildchar
    '                    If InStr(Value, "*") > 0 Or InStr(Value, "?") > 0 Or InStr(Value, "[") > 0 Or _
    '                       InStr(Value, "]") > 0 _
    '                       Then
    '                        For n = 0 To UBound(allfields)
    '                            If allfields(n) Like Value Then
    '                                ReDim Preserve theFields(j)
    '                                theFields(j) = Trim(allfields(n))
    '                                j = j + 1
    '                            End If
    '                        Next n
    '                        ' plain
    '                    Else
    '                        ReDim Preserve theFields(j)
    '                        theFields(j) = Trim(multi(i))
    '                        j = j + 1
    '                    End If
    '                End If
    '            End If
    '        Next i
    '    End If

    '    ' return
    '    If IsArrayInitialized(theFields) Then
    '        getMQFConfigFields = theFields
    '    End If

    'End Function


    '**********
    '**********  getMQFConfigfields : Split fields-ID in the fields()
    '**********

    'Private Function getMQFConfigROFields(fields As String) As String()

    '    Dim allfields As Object
    '    Dim Value As Object
    '    Dim multi() As String
    '    Dim i, j As Integer
    '    Dim delimiter As String
    '    Dim theFields() As String
    '    Dim n As Integer


    '    ' check all Fields
    '    allfields = getDBDescIDs()
    '    If Not IsArrayInitialized(allfields) Then
    '        Call CoreMessageHandler(showmsgbox:=True, message:="Abort: Header Field Description / name headerids is not found ", _
    '                               procedure:="modXLSMessageQueueFile.getMQFConfigROFields", messagetype:=otCoreMessageType.ApplicationError, break:=False)

    '    End If

    '    delimiter = constDelimeter
    '    j = 0

    '    multi = SplitMultiDelims(text:=fields, DelimChars:=delimiter)
    '    If IsArrayInitialized(multi) Then
    '        For i = 1 To UBound(multi)
    '            If InStr(multi(i), delimiter) = 0 Then
    '                Value = Trim(multi(i))
    '                If Value <> "" Then
    '                    ' wildchar
    '                    If InStr(Value, "*") > 0 Or InStr(Value, "?") > 0 Or InStr(Value, "[") > 0 Or _
    '                       InStr(Value, "]") > 0 _
    '                       Then
    '                        For n = 0 To UBound(allfields)
    '                            If allfields(n) Like Value Then
    '                                ReDim Preserve theFields(j)
    '                                theFields(j) = Trim(allfields(n))
    '                                j = j + 1
    '                            End If
    '                        Next n
    '                        ' plain
    '                    Else
    '                        ReDim Preserve theFields(j)
    '                        theFields(j) = Trim(multi(i))
    '                        j = j + 1
    '                    End If
    '                End If
    '            End If
    '        Next i
    '    End If

    '    ' return
    '    If IsArrayInitialized(theFields) Then
    '        getMQFConfigROFields = theFields
    '    End If

    'End Function

    '**********
    '**********  getMQFConfigTable : returns the MQFConfigTable as 2Dim object
    '**********

    'Function getMQFConfigTable(Ctable() As MQFConfig, Optional aName As String = "") As Boolean
    '    Dim config_table As Range
    '    Dim row As Range
    '    'Dim ctable() As object



    '    Dim Value As Object
    '    Dim i As Integer
    '    Dim aCartypes As New clsLEGACYCartypes
    '    Dim found As Boolean

    '    config_table = GetXlsParameterRangeByName("otdb_parameter_mqf_config_table")

    '    found = False

    '    i = -1
    '    For Each row In config_table.Rows
    '        ' found or all
    '        If Trim(UCase(row.Cells(1, constMQFConfigName + 1))) = aName Or aName = "" Then
    '            found = True
    '            i = i + 1
    '            ReDim Preserve Ctable(i)
    '            ' Name
    '            Ctable(i).Name = Trim(UCase(row.Cells(1, constMQFConfigName + 1)))
    '            ' oldValues
    '            If Trim(row.Cells(1, constMQFConfigCopyOldValues + 1)) <> "" Then
    '                Ctable(i).copyOldValues = True
    '            Else
    '                Ctable(i).copyOldValues = False
    '            End If
    '            Ctable(i).FieldIDs = getMQFConfigFields(Trim(row.Cells(1, constMQFConfigFields + 1)))
    '            Ctable(i).FieldROIDs = getMQFConfigROFields(Trim(row.Cells(1, constMQFConfigROFields + 1)))
    '            ' Description
    '            Ctable(i).desc = Trim(row.Cells(1, constMQFConfigDesc + 1))
    '        End If
    '    Next row

    '    getMQFConfigTable = found

    'End Function

    '**********
    '**********  getMQFConfigTable : returns the MQFConfigTable as 2Dim object
    '**********

    'Function getMQFField(aName As String, Optional ColumnType As Integer = constMQFCT_ALL) As Range
    '    Dim DTable As Range
    '    Dim row As Range
    '    'Dim ctable() As object
    '    Dim Value As Object
    '    Dim i As Integer
    '    Dim found As Boolean
    '    Dim result As Range
    '    Dim col As Integer


    '    DTable = GetXlsParameterRangeByName("otdb_parameter_mqf_structure_db_description_table", silent:=True)
    '    If DTable Is Nothing Then
    '        DTable = GetXlsParameterRangeByName("otdb_parameter_mqf_db_description_table", silent:=True)
    '    End If

    '    found = False
    '    i = -1
    '    ' search
    '    For Each row In DTable.Rows
    '        ' found or all
    '        If Trim(UCase(row.Cells(1, constMQFFieldheader + 1))) = UCase(aName) Or aName = "" Then
    '            Value = row.Cells(1, constMQFFieldColumn + 1).Value
    '            If IsNumeric(Value) Then
    '                col = CDec(Value)
    '            Else
    '                col = 0
    '            End If
    '            ' search criteria
    '            If ColumnType = constMQFCT_ALL Or (ColumnType = constMQFCT_Pre And col < 0) _
    '               Or (ColumnType = constMQFCT_Post And col > 0) Then
    '                found = True
    '                i = i + 1
    '                If result Is Nothing Then
    '                    result = row
    '                Else
    '                    result = Globals.ThisAddIn.Application.Union(result, row)
    '                End If
    '            End If
    '        End If
    '    Next row

    '    If found Then
    '        getMQFField = result
    '    Else
    '        getMQFField = Nothing
    '    End If
    'End Function

    '**********
    '**********  getminMQField : returns the smalles column-no
    '**********

    '    Function getminMQFField(ColumnType As Integer) As Integer
    '        Dim DTable As Range
    '        Dim row As Range
    '        'Dim ctable() As object
    '        Dim Value As Object
    '        Dim i As Integer
    '        Dim found As Boolean
    '        Dim result As Integer
    '        Dim col As Integer


    '        DTable = GetXlsParameterRangeByName("otdb_parameter_mqf_structure_db_description_table")
    '        If DTable Is Nothing Then
    '            DTable = GetXlsParameterRangeByName("otdb_parameter_mqf_db_description_table", silent:=True)
    '        End If
    '        found = False
    '        result = 0
    '        ' search
    '        For Each row In DTable.Rows
    '            ' found or all
    '            Value = row.Cells(1, constMQFFieldColumn + 1).Value
    '            If IsNumeric(Value) Then
    '                col = CDec(Value)
    '            Else
    '                col = 0
    '            End If
    '            ' search criteria
    '            If ColumnType = constMQFCT_ALL Or (ColumnType = constMQFCT_Pre And col < 0) _
    '               Or (ColumnType = constMQFCT_Post And col > 0) Then
    '                If col < result Then
    '                    found = True
    '                    result = col
    '                End If
    '            End If
    '        Next row

    '        If found Then
    '            getminMQFField = result
    '        Else
    '            getminMQFField = 0
    '        End If
    '    End Function

    '    '**********
    '    '**********  getmaxMQField : returns the smalles column-no
    '    '**********

    '    Function getmaxMQFField(ColumnType As Integer) As Integer
    '        Dim DTable As Range
    '        Dim row As Range
    '        'Dim ctable() As object
    '        Dim Value As Object
    '        Dim i As Integer
    '        Dim found As Boolean
    '        Dim result As Integer
    '        Dim col As Integer


    '        DTable = GetXlsParameterRangeByName("otdb_parameter_mqf_structure_db_description_table")
    '        If DTable Is Nothing Then
    '            DTable = GetXlsParameterRangeByName("otdb_parameter_mqf_db_description_table", silent:=True)
    '        End If
    '        found = False
    '        result = 0
    '        ' search
    '        For Each row In DTable.Rows
    '            ' found or all
    '            Value = row.Cells(1, constMQFFieldColumn + 1).Value
    '            If IsNumeric(Value) Then
    '                col = CDec(Value)
    '            Else
    '                col = 0
    '            End If
    '            ' search criteria
    '            If ColumnType = constMQFCT_ALL Or (ColumnType = constMQFCT_Pre And col < 0) _
    '               Or (ColumnType = constMQFCT_Post And col > 0) Then
    '                If col > result Then
    '                    found = True
    '                    result = col
    '                End If
    '            End If
    '        Next row

    '        If found Then
    '            getmaxMQFField = result
    '        Else
    '            getmaxMQFField = 0
    '        End If
    '    End Function

    '    Function checkArray(atestArray As Object) As Boolean

    '        On Error GoTo error_handler

    '        If UBound(atestArray) >= 0 Then
    '            checkArray = True
    '            Exit Function
    '        End If

    'error_handler:
    '        checkArray = False
    '        Exit Function
    '    End Function

    '    '********** createXChangeConfigFromIDs: creates a config from an array with IDs, ordinal will be the columns
    '    '**********
    '    Public Sub createXlsDoc9MQFConfig()
    '        'Dim anObjectName As String
    '        'Dim aNewConfig As New XChangeConfiguration
    '        'Dim aColl As Collection
    '        'Dim aSchemaDefTable As New ObjectDefinition
    '        'Dim m As Object
    '        'Dim IDs As Object
    '        'Dim cmds As Object
    '        'Dim flag As Boolean
    '        'Dim aFieldDef As IObjectEntryDefinition
    '        'Dim i As Long

    '        ''*** load the table definition
    '        ''If Not aSchemaDefTable.Inject(Tablename) Then
    '        ''    Call OTDBErrorHandler(argument:=Tablename, Tablename:=Tablename, message:=" Could not load SchemaTableDefinition")
    '        ''    Set createXChangeConfigFromIDs = Nothing
    '        ''    Exit Function
    '        ''End If
    '        ''anObjectName = Tablename
    '        ''If aNewConfig.Inject(ConfigName) Then
    '        ''    aNewConfig.delete
    '        ''End If

    '        ''**
    '        ''** CREATE MQF METHODS
    '        'aNewConfig.Create("mqf_methods")
    '        'Call aNewConfig.AddObjectByName("tblDeliverableTargets")
    '        'Call aNewConfig.AddObjectByName("tblDeliverables")
    '        'IDs = New String() {"uid", "c10", "c6", "t2"}
    '        'cmds = New Integer() {otXChangeCommandType.Read, otXChangeCommandType.Read, otXChangeCommandType.Read, otXChangeCommandType.Update}

    '        'i = 0

    '        'For i = LBound(IDs) To UBound(IDs)
    '        '    ' load ID
    '        '    If Not modHelperVBA.IsEmpty(IDs(i)) Then
    '        '        flag = False
    '        '        ' look into objects first
    '        '        For Each m In aNewConfig.ObjectsByOrderNo
    '        '            If aFieldDef.LoadByID(IDs(i), m.OBJECTNAME) Then
    '        '                Call aNewConfig.AddAttributeByField(objectentry:=aFieldDef, ordinal:=i, xcmd:=cmds(i))
    '        '                flag = True
    '        '                Exit For
    '        '            End If
    '        '        Next m
    '        '        ' if not found look elsewhere -> but take all IDs and aliases !
    '        '        If flag = False Then
    '        '            aColl = aFieldDef.AllByID(IDs(i))
    '        '            For Each m In aColl
    '        '                aFieldDef = m
    '        '                'Call aNewConfig.addObjectByName(aFieldDef.tablename, xcmd:=xcmd) -> by AttributesField
    '        '                Call aNewConfig.AddAttributeByField(objectentry:=aFieldDef, ordinal:=i, xcmd:=cmds(i))
    '        '            Next m
    '        '        End If
    '        '    End If
    '        'Next i

    '        'Call aNewConfig.Persist()
    '        ''Set createXlsDoc9MQFConfig = aNewConfig
    '    End Sub

    '    '**********
    '    '**********  createXlsDoc9MQF -> Create a Template MQF
    '    '**********
    '    '********** FieldIDs () as ColumnIDs of Fields to use
    '    '********** ROFields () (Read-Only Fields)
    '    '********** copyOld as Flag

    '    '********** OPtional Parameters
    '    '********** selectedUIDs() List of UIDs as selection
    '    '********** CloseAfterCreation if true close Workbook
    '    '********** aMQFWorkbook for return of the created workbook
    '    '********** copyValuesFromOldMessages -> TRUE if
    '    '********** MessagesToCopy is filled

    '    Public Function createXlsDoc9MQF(ByVal Filename As String, _
    '                                     FieldIDs() As String, _
    '                                     copyOld As Boolean, _
    '                                     ROFieldIDs() As String, _
    '                                     aMessagesToCopy() As MQFMessage, _
    '                                     theMQFWorkbookToCopy As Excel.Workbook, _
    '                                     copyValuesFromOldMessages As Boolean, _
    '                                     Optional selectedUIDs As Object() = Nothing, _
    '                                     Optional CloseAfterCreation As Boolean = False, _
    '                                     Optional aMQFWorkbook As Excel.Workbook = Nothing _
    '                                     ) As Boolean
    '        Dim FileNamePattern, FileNamePattern2, currWorkbookName, MQFWorkbookName, MQFSheetName As String
    '        Dim filenames As Object
    '        Dim flag, Doc9sheet_flag As Boolean
    '        Dim wb As Excel.Workbook
    '        Dim MQFWS As Excel.Worksheet
    '        Dim Value As Object
    '        Dim template As String
    '        Dim startfolder As String
    '        Dim dbDoc9Range As Range
    '        Dim selection, selectioncol As Range
    '        Dim msgboxrsl As CoreMessageBox.ResultType
    '        Dim MQFWorkbook As Excel.Workbook
    '        Dim MQFWorksheetName As String
    '        Dim MQFsheet_flag As Boolean
    '        Dim headerstartrow As Integer
    '        Dim dbdesc() As xlsDBDesc
    '        Dim fieldname As String
    '        Dim i As Integer
    '        Dim headerids As Excel.Range
    '        Dim headerids_name As String
    '        Dim Prefix As String
    '        'Dim filename As String
    '        Dim startdatarow As Integer
    '        Dim cols() As Integer
    '        Dim row As Excel.Range
    '        Dim j As Integer
    '        Dim preheader As Excel.Range
    '        Dim postheader As Excel.Range
    '        Dim preheadercount As Integer
    '        Dim postheadercount As Integer
    '        Dim operationcolumn As Integer
    '        Dim formatoldvalues As Excel.Range
    '        Dim formatnewvalues As Excel.Range
    '        Dim MaxCol As Integer
    '        Dim operation_column As Integer
    '        Dim uid_column As Integer
    '        Dim uid_column_new As Integer
    '        Dim pn_column As Integer
    '        Dim pn_column_new As Integer
    '        Dim rocols() As Integer
    '        Dim postheaderstart As Integer
    '        Dim rowno As Long
    '        Dim aMQFDbDesc() As XLSMQFColumnDescription
    '        Dim selectfield As Excel.Range

    '        Dim aXChangeConfig As XChangeConfiguration
    '        Dim otdbvalues() As Object
    '        Dim n, m As Integer
    '        Dim templatefiledir As String
    '        Dim otdbvalue_uid_index As Long
    '        Dim otdbvalue_uid As Long
    '        'Dim aProgressBar As New clsUIProgressBarForm


    '        ' Get Selection
    '        'dbDoc9Range = GetdbDoc9Range()
    '        'If dbDoc9Range Is Nothing Then
    '        '    createXlsDoc9MQF = False
    '        '    Exit Function
    '        'End If
    '        ' Blend in all Columns
    '        dbDoc9Range.EntireColumn.Hidden = False
    '        uid_column = getXLSHeaderIDColumn("uid")


    '        ' Any values selection
    '        'selection.Find what:="*", LookIn:=xlValues
    '        If Not IsMissing(selectedUIDs) And IsArray(selectedUIDs) Then
    '            System.Diagnostics.Debug.WriteLine("Using preselected UIDs")
    '        Else

    '            '*** selection
    '            ' selection
    '            Value = getXLSHeaderIDColumn("x1")
    '            selectioncol = dbDoc9Range.Worksheet.Range(dbDoc9Range.Cells(1, Value), dbDoc9Range.Cells(dbDoc9Range.Rows.Count, Value))
    '            ' selection
    '            ReDim selectedUIDs(0)
    '            j = 0
    '            ' select manually the uids
    '            For Each selectfield In selectioncol.Cells
    '                If Not IsEmpty(selectfield.Value) Then
    '                    If IsNumeric(selectfield.EntireRow.Cells(1, uid_column).Value) Then
    '                        ReDim Preserve selectedUIDs(j)
    '                        selectedUIDs(j) = CLng(selectfield.EntireRow.Cells(1, uid_column).Value)
    '                        j = j + 1
    '                    End If
    '                End If
    '            Next selectfield

    '            '* nothing selected
    '            If j = 0 Then
    '                With New CoreMessageBox
    '                    .Message = "ATTENTION !" & vbLf & "No data rows have been selected in the SELECTION Column of the Database. Should ALL rows be written to the Message Queue File ?"
    '                    .Title = " ARE YOU SURE ?"
    '                    .type = CoreMessageBox.MessageType.Question
    '                    .Show()
    '                    msgboxrsl = .result
    '                End With

    '                If msgboxrsl <> CoreMessageBox.ResultType.Yes Then
    '                    Exit Function
    '                Else
    '                    'select all uids
    '                    For Each selectfield In selectioncol.Cells
    '                        If IsNumeric(selectfield.EntireRow.Cells(1, uid_column).Value) Then
    '                            ReDim Preserve selectedUIDs(j)
    '                            selectedUIDs(j) = CLng(selectfield.EntireRow.Cells(1, uid_column).Value)
    '                            j = j + 1
    '                        End If
    '                    Next selectfield
    '                End If

    '            End If


    '            j = 0
    '        End If    '*


    '        ' parameters
    '        startfolder = GetDBParameter("parameter_startfoldernode")
    '        If startfolder <> "" Then
    '            If Mid(startfolder, Len(Value), 1) <> "\" Then startfolder = startfolder & "\"
    '        End If
    '        templatefiledir = GetDBParameter("otdb_parameter_mqf_template_filepath")
    '        If templatefiledir <> "" Then
    '            If Mid(templatefiledir, Len(Value), 1) <> "\" Then templatefiledir = templatefiledir & "\"
    '        End If
    '        template = GetDBParameter("otdb_parameter_mqf_template_file")
    '        'template = startfolder & template
    '        formatoldvalues = GetXlsParameterRangeByName("otdb_parameter_mqf_format_oldvalues")
    '        formatnewvalues = GetXlsParameterRangeByName("otdb_parameter_mqf_format_newvalues")

    '        ' where is template
    '        If FileIO.FileSystem.FileExists(startfolder & templatefiledir & template) Then
    '            template = startfolder & templatefiledir & template
    '        ElseIf FileIO.FileSystem.FileExists(startfolder & template) Then
    '            template = startfolder & template
    '#If ExcelVersion <> "" Then
    '        ElseIf FileIO.FileSystem.FileExists(Globals.ThisAddIn.Application.ActiveWorkbook.Path & "\" & template) Then
    '            template = Globals.ThisAddIn.Application.ActiveWorkbook.Path & "\" & template
    '#End If
    '        Else
    '            Call CoreMessageHandler(showmsgbox:=True, _
    '                                   message:="Abort: The MQF Template '" & template & " ' is not found in the filesystem. Please contact your Administrator" _
    '                                   , procedure:="modXLSMessageQueueFile.createXlsDoc9MQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)
    '            createXlsDoc9MQF = False
    '            Exit Function
    '        End If

    '        '*
    '        '* open
    '        '*
    '        currWorkbookName = Globals.ThisAddIn.Application.ActiveWorkbook.Name

    '        'Open the MQF Template
    '        MQFWorkbook = Globals.ThisAddIn.Application.Globals.ThisAddin.Application.Workbooks.Open(Filename:=template, UpdateLinks:=2, ReadOnly:=True)

    '        MQFWorksheetName = GetXlsParameterByName("otdb_parameter_mqf_templatedata", workbook:=MQFWorkbook, silent:=True)
    '        If MQFWorksheetName = "" Then
    '            MQFWorksheetName = "Data"
    '        End If
    '        'Check if Worksheet there
    '        ' Check if Data Sheet is still there
    '        MQFsheet_flag = False
    '        For Each MQFWS In MQFWorkbook.Sheets
    '            If MQFWS.Name = MQFWorksheetName Then
    '                MQFsheet_flag = True
    '                Exit For
    '            End If
    '        Next MQFWS

    '        ' Error
    '        If MQFsheet_flag = False Then
    '            Call CoreMessageHandler(showmsgbox:=True, message:="Abort: The Worksheet '" & MQFWorksheetName & " ' is not found in the Workbook. Is this a valid Doc9 Message Queue File ? " _
    '                   & MQFWorkbook.Name & "!", procedure:="modXLSMessageQueueFile.createXlsDoc9MQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)

    '            createXlsDoc9MQF = False
    '            Exit Function
    '        End If



    '        'On Error GoTo handleError:

    '        '**
    '        '** Build Header
    '        '**
    '        Value = GetXlsParameterByName("otdb_parameter_mqf_template_headerstartrow", workbook:=MQFWorkbook)
    '        If Not IsNumeric(Value) Then
    '            createXlsDoc9MQF = False
    '            MQFWorkbook.Close(False)
    '            Exit Function
    '        End If
    '        headerstartrow = CInt(Value)
    '        headerids_name = GetXlsParameterByName("parameter_doc9_headerid_name")
    '        headerids = GetXlsParameterRangeByName(headerids_name)
    '        'parameter_doc9_dbdesc_prefix
    '        Prefix = GetXlsParameterByName("parameter_doc9_dbdesc_prefix")

    '        ' error
    '        If headerids Is Nothing Then
    '            Call CoreMessageHandler(showmsgbox:=True, message:="The parameter 'parameter_doc9_headerid_name':" & headerids_name & " is not showing a valid range !" _
    '                  , procedure:="modXLSMessageQueueFile.createXlsDoc9MQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)

    '            createXlsDoc9MQF = False
    '            Exit Function
    '        End If

    '        Globals.ThisAddIn.Application.ScreenUpdating = False

    '        '********
    '        '******** headers
    '        '********

    '        '*** parameters
    '        preheadercount = Math.Abs(getminMQFField(constMQFCT_Pre))
    '        preheader = getMQFField("", constMQFCT_Pre)
    '        postheadercount = getmaxMQFField(constMQFCT_Post)
    '        postheader = getMQFField("", constMQFCT_Post)

    '        ' if preheader is found
    '        If Not preheader Is Nothing Then

    '            '*** preheaders
    '            i = 0
    '            For Each row In preheader
    '                ' ID
    '                MQFWS.Cells(headerstartrow, i + 1).Value = row.Cells(1, constMQFFieldheader + 1).Value
    '                row.Cells(1, constMQFFieldheader + 1).Copy()
    '                MQFWS.Cells(headerstartrow, i + 1).PasteSpecial(Excel.XlPasteType.xlPasteFormats)
    '                ' Title
    '                MQFWS.Cells(headerstartrow + 1, i + 1) = row.Cells(1, constMQFfieldname + 1).Value
    '                row.Cells(1, constMQFfieldname + 1).Copy()
    '                MQFWS.Cells(headerstartrow + 1, i + 1).PasteSpecial(Excel.XlPasteType.xlPasteFormats)
    '                MQFWS.Cells(headerstartrow, i + 1).ColumnWidth = row.Cells(1, constMQFFieldColumnWidth + 1).Value
    '                i = i + 1
    '            Next row

    '        End If

    '        '***
    '        '*** Header of fields
    '        '***
    '        ReDim cols(UBound(FieldIDs))
    '        ReDim rocols(UBound(FieldIDs))    ' same size as FieldsIDs -> Crossreference with Column

    '        ' init
    '        'Call aProgressBar.initialize(UBound(FieldIDs), WindowCaption:="preprocessing MQF ...")
    '        'aProgressBar.showForm()

    '        '******* XCHANGE: create the config of the OTDB XChangeManager
    '        '*******
    '        aXChangeConfig = XChangeManager.CreateXChangeConfigFromIDs(configname:="$$mqftmp", _
    '                                                        xids:=FieldIDs, _
    '                                                        xcmd:=otXChangeCommandType.Read, _
    '                                                        objectids:=New String() {"tblschedules", _
    '                                                                                  "tbldeliverabletargets", _
    '                                                                                  "tbldeliverables", _
    '                                                                                  "tblparts", _
    '                                                                                  "tbldeliverabletracks", _
    '                                                                                  "tblconfigs"})

    '        ReDim Preserve otdbvalues(UBound(FieldIDs))

    '        For i = 0 To UBound(FieldIDs)

    '            fieldname = FieldIDs(i)

    '            '**** XCHANGE
    '            If LCase(fieldname) = "uid" Then otdbvalue_uid_index = i

    '            ' get XLS Database Description -> necessary to get the XLS Column for formatting
    '            If getDBDesc(dbdesc, fieldname) Then
    '                cols(i) = dbdesc(0).ColumnNo    ' column in doc9
    '                ' check for READ_ONLY
    '                For n = 0 To UBound(ROFieldIDs)
    '                    If UCase(fieldname) = UCase(ROFieldIDs(n)) Then
    '                        ' crossreference the new column
    '                        rocols(i) = i + 1 + preheadercount    ' column ro in mqf
    '                    End If
    '                Next n

    '                '***
    '                '*** Copy -> Paste of ID (HEADER FORMATING)
    '                '***
    '                headerids.Cells(1, dbdesc(0).ColumnNo).Copy()
    '                MQFWS.Cells(headerstartrow, i + 1 + preheadercount).PasteSpecial(Excel.XlPasteType.xlPasteFormats)
    '                If MQFWS.Cells(headerstartrow, i + 1 + preheadercount).FormatConditions.count > 0 Then
    '                    For n = 1 To MQFWS.Cells(headerstartrow, i + 1 + preheadercount).FormatConditions.count
    '                        MQFWS.Cells(headerstartrow, i + 1 + preheadercount).FormatConditions(n).delete()
    '                    Next n
    '                End If
    '                MQFWS.Cells(headerstartrow, i + 1 + preheadercount).Value = headerids.Cells(1, dbdesc(0).ColumnNo).text
    '                MQFWS.Cells(headerstartrow, i + 1 + preheadercount).ColumnWidth = _
    '                headerids.Cells(1, dbdesc(0).ColumnNo).ColumnWidth
    '                ' Copy -> Paste Group
    '                'headerids.Cells(2, DBDesc(0).ColumnNo).Copy
    '                'MQFWS.Cells(Headerstartrow + 1, i + 1 + preheadercount).PasteSpecial
    '                'MQFWS.Cells(Headerstartrow + 1, i + 1 + preheadercount).ColumnWidth = headerids.Cells(2, DBDesc(0).ColumnNo).ColumnWidth
    '                ' Copy -> Paste od Title
    '                headerids.Cells(3, dbdesc(0).ColumnNo).Copy()
    '                MQFWS.Cells(headerstartrow + 1, i + 1 + preheadercount).PasteSpecial(Excel.XlPasteType.xlPasteFormats)
    '                If MQFWS.Cells(headerstartrow + 1, i + 1 + preheadercount).FormatConditions.count > 0 Then
    '                    For n = 1 To MQFWS.Cells(headerstartrow + 1, i + 1 + preheadercount).FormatConditions.count
    '                        MQFWS.Cells(headerstartrow + 1, i + 1 + preheadercount).FormatConditions(n).delete()
    '                    Next n
    '                End If
    '                MQFWS.Cells(headerstartrow + 1, i + 1 + preheadercount).Value = headerids.Cells(3, dbdesc(0).ColumnNo).text
    '                MQFWS.Cells(headerstartrow + 1, i + 1 + preheadercount).ColumnWidth = _
    '                headerids.Cells(3, dbdesc(0).ColumnNo).ColumnWidth
    '            Else
    '                Call CoreMessageHandler(showmsgbox:=True, message:="Abort: The Field-ID '" & fieldname & " ' is not found in the OnTrack Database Description." _
    '                       & MQFWorkbook.Name & "!" _
    '                 , procedure:="modXLSMessageQueueFile.createXlsDoc9MQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)

    '                createXlsDoc9MQF = False
    '                Exit Function
    '            End If
    '        Next i

    '        ' if postheader ist found
    '        If Not postheader Is Nothing Then
    '            '*** post-headers
    '            ' i is to be used from above !
    '            i = i + 1
    '            postheaderstart = i
    '            For Each row In postheader.Rows
    '                ' ID
    '                MQFWS.Cells(headerstartrow, i + 1).Value = row.Cells(1, constMQFFieldheader + 1).Value
    '                row.Cells(1, constMQFFieldheader + 1).Copy()
    '                MQFWS.Cells(headerstartrow, i + 1).PasteSpecial(Excel.XlPasteType.xlPasteFormats)
    '                ' Title
    '                MQFWS.Cells(headerstartrow + 1, i + 1) = row.Cells(1, constMQFfieldname + 1).Value
    '                row.Cells(1, constMQFfieldname + 1).Copy()
    '                MQFWS.Cells(headerstartrow + 1, i + 1).PasteSpecial(Excel.XlPasteType.xlPasteFormats)

    '                ' width
    '                MQFWS.Cells(headerstartrow, i + 1).ColumnWidth = row.Cells(1, constMQFFieldColumnWidth + 1).Value
    '                i = i + 1
    '            Next row
    '        End If

    '        ' height of header
    '        MQFWS.Cells(headerstartrow, 1).RowHeight = headerids.Cells(1, 1).RowHeight
    '        MQFWS.Cells(headerstartrow + 1, 1).RowHeight = headerids.Cells(3, 1).RowHeight
    '        ' generate the name
    '        'constMQFHeaderidName
    '        MQFWS.Parent.Names.add(constMQFHeaderidName, _
    '                               RefersTo:=MQFWS.Range(MQFWS.Cells(headerstartrow, 1), MQFWS.Cells(headerstartrow, i)))

    '        ' start data row
    '        startdatarow = headerstartrow + 1 + 1
    '        MaxCol = i
    '        'aProgressBar.closeForm()

    '        '**********
    '        '********** copy data
    '        '**********
    '        ' get the columnno of the action column
    '        Value = getMQFField(constMQFActionID)
    '        If Not Value Is Nothing Then
    '            operation_column = Value.Cells(1, constMQFFieldColumn + 1).Value
    '            If operation_column < 0 Then
    '                operation_column = operation_column + 1 + preheadercount
    '            Else
    '                operation_column = operation_column + 1 + postheadercount
    '            End If
    '        End If

    '        i = 0

    '        '*** create each row
    '        '*** in mqf
    '        ' set the selectioncol to look uid
    '        selectioncol = dbDoc9Range.Worksheet.Range(dbDoc9Range.Cells(1, uid_column), dbDoc9Range.Cells(dbDoc9Range.Rows.Count, uid_column))

    '        ' init
    '        'Call aProgressBar.initialize(UBound(selectedUIDs), WindowCaption:="creating MQF ...")
    '        'aProgressBar.showForm()

    '        For rowno = 0 To UBound(selectedUIDs)
    '            'Call aProgressBar.progress(1, "writing .... #" & rowno)

    '            ' get row
    '            'value = getXLSHeaderIDColumn("uid")
    '            row = FindAll(selectioncol, selectedUIDs(rowno), LookIn:=Excel.XlFindLookIn.xlValues)
    '            '*
    '            If row Is Nothing Then
    '                System.Diagnostics.Debug.WriteLine("Row with UID#" & selectedUIDs(rowno) & " couldnt be found")

    '            Else
    '                'set row =
    '                If copyOld Then
    '                    i = i + 2
    '                Else
    '                    i = i + 1
    '                End If

    '                '******* XCHANGE get the data from the OTDB
    '                '*******
    '                For j = 0 To UBound(FieldIDs)
    '                    otdbvalues(j) = Nothing
    '                Next j
    '                otdbvalues(otdbvalue_uid_index) = selectedUIDs(rowno)
    '                '*** read all data -> will be used in copyOldLine or in READ_ONLY fields of the new line
    '                If XChangeManager.XChangeWithArray(aXChangeConfig, otdbvalues) Then
    '                End If


    '                ' Copy the fields
    '                If copyOld Then
    '                    ' prefill the operation code ->
    '                    MQFWS.Cells(startdatarow + i - 2, operation_column).Value = "noop"
    '                    For j = 0 To UBound(FieldIDs)
    '                        ' copy the old values
    '                        MQFWS.Cells(startdatarow + i - 2, j + 1 + preheadercount).Value = otdbvalues(j)
    '                        'row.EntireRow.Cells(1, cols(j)).text
    '                    Next j
    '                    ' copy format
    '                    'formatoldvalues.Copy
    '                    With MQFWS.Range(MQFWS.Cells(startdatarow + i - 2, 1), MQFWS.Cells(startdatarow + i - 2, MaxCol))    '.PasteSpecial xlPasteFormats
    '                        .Interior.Color = formatoldvalues.Interior.Color
    '                        .Font.Color = formatoldvalues.Font.Color
    '                        .Font.Name = formatoldvalues.Font.Name
    '                        .Font.Size = formatoldvalues.Font.Size
    '                        .Font.Bold = formatoldvalues.Font.Bold
    '                        .Borders.LineStyle = formatoldvalues.Borders.LineStyle
    '                        .Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).Color = formatoldvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).Color
    '                        .Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).Weight = formatoldvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).Weight
    '                        .Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = formatoldvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle
    '                        .Borders.Item(Excel.XlBordersIndex.xlEdgeTop).Color = formatoldvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeTop).Color
    '                        .Borders.Item(Excel.XlBordersIndex.xlEdgeTop).Weight = formatoldvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeTop).Weight
    '                        .Borders.Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = formatoldvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle
    '                        .Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).Color = formatoldvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).Color
    '                        .Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).Weight = formatoldvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).Weight
    '                        .Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle = formatoldvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle
    '                        .Borders.Item(Excel.XlBordersIndex.xlEdgeRight).Color = formatoldvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeRight).Color
    '                        .Borders.Item(Excel.XlBordersIndex.xlEdgeRight).Weight = formatoldvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeRight).Weight
    '                        .Borders.Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = formatoldvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle
    '                    End With

    '                    ' protect
    '                    MQFWS.Range(MQFWS.Cells(startdatarow + i - 2, 1), MQFWS.Cells(startdatarow + i - 2, MaxCol)).Locked = True
    '                End If

    '                ' create new line
    '                ' operations code
    '                With MQFWS.Cells(startdatarow + i - 1, operation_column).Validation
    '                    .purge()
    '                    .add(Type:=Excel.XlDVType.xlValidateList, AlertStyle:=Excel.XlDVAlertStyle.xlValidAlertStop, _
    '                         Operator:=Excel.XlFormatConditionOperator.xlBetween, Formula1:="=parameter_template_action_table")
    '                    .IgnoreBlank = True
    '                    .InCellDropdown = True
    '                    .InputTitle = "Input"
    '                    .ErrorTitle = "Error"
    '                    .InputMessage = "Please enter the Operation Code"
    '                    .ErrorMessage = "Please provide correct Operation Code"
    '                    .ShowInput = True
    '                    .ShowError = True
    '                End With

    '                '** copy the existing value from the messages if existing
    '                '**
    '                If copyValuesFromOldMessages Then
    '                    For m = LBound(aMessagesToCopy) To UBound(aMessagesToCopy)
    '                        If aMessagesToCopy(m).UID = selectedUIDs(rowno) Then
    '                            For j = 0 To UBound(FieldIDs)
    '                                If getMQFDBDesc(aMQFDbDesc, aName:=FieldIDs(j), WORKBOOK:=theMQFWorkbookToCopy) Then

    '                                    ' copy the old values
    '                                    MQFWS.Cells(startdatarow + i - 1, _
    '                                                j + 1 + preheadercount).Value = "'" & _
    '                                                CStr(aMessagesToCopy(m).fieldvalues(aMQFDbDesc(0).ColumnNo - 1))
    '                                End If
    '                            Next j
    '                        End If
    '                    Next m
    '                End If

    '                ' format
    '                'formatnewvalues.Copy
    '                With MQFWS.Range(MQFWS.Cells(startdatarow + i - 1, 1), MQFWS.Cells(startdatarow + i - 1, MaxCol))

    '                    '.PasteSpecial xlPasteFormats
    '                    .Interior.Color = formatnewvalues.Interior.Color
    '                    .Font.Color = formatnewvalues.Font.Color
    '                    .Font.Name = formatnewvalues.Font.Name
    '                    .Font.Size = formatnewvalues.Font.Size
    '                    .Font.Bold = formatnewvalues.Font.Bold
    '                    .Borders.LineStyle = formatnewvalues.Borders.LineStyle
    '                    .Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).Color = formatnewvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).Color
    '                    .Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).Weight = formatnewvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).Weight
    '                    .Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = formatnewvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle
    '                    .Borders.Item(Excel.XlBordersIndex.xlEdgeTop).Color = formatnewvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeTop).Color
    '                    .Borders.Item(Excel.XlBordersIndex.xlEdgeTop).Weight = formatnewvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeTop).Weight
    '                    .Borders.Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = formatnewvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle
    '                    .Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).Color = formatnewvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).Color
    '                    .Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).Weight = formatnewvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).Weight
    '                    .Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle = formatnewvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle
    '                    .Borders.Item(Excel.XlBordersIndex.xlEdgeRight).Color = formatnewvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeRight).Color
    '                    .Borders.Item(Excel.XlBordersIndex.xlEdgeRight).Weight = formatnewvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeRight).Weight
    '                    .Borders.Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = formatnewvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle

    '                    ' not locked
    '                    .Locked = False
    '                    ' bold
    '                    .Font.Bold = True
    '                End With
    '                ' special format
    '                ' fill the copy values
    '                For n = 0 To UBound(rocols)
    '                    ' cross
    '                    If rocols(n) <> 0 Then
    '                        With MQFWS.Cells(startdatarow + i - 1, rocols(n))
    '                            '.Value = row.EntireRow.Cells(1, cols(n)).text
    '                            .Value = otdbvalues(n)
    '                            .Locked = True
    '                            .Interior.Color = constLockedBackground
    '                            .Font.Bold = False
    '                        End With
    '                    End If
    '                    ' special handling MOCKUP STATUS
    '                    '*
    '                    If LCase(FieldIDs(n)) = "bp5" Then
    '                        With MQFWS.Cells(startdatarow + i - 1, n + 1 + preheadercount).Validation
    '                            .purge()

    '                            .add(Type:=Excel.XlDVType.xlValidateList, AlertStyle:=Excel.XlDVAlertStyle.xlValidAlertStop, _
    '                         Operator:=Excel.XlFormatConditionOperator.xlBetween, Formula1:="=parameter_dmu_status")
    '                            .IgnoreBlank = True
    '                            .InCellDropdown = True
    '                            .InputTitle = "Input"
    '                            .ErrorTitle = "Error"
    '                            .InputMessage = "Please enter the DMU status code"
    '                            .ErrorMessage = "Please provide correct DMU status code"
    '                            .ShowInput = True
    '                            .ShowError = True
    '                        End With
    '                    End If
    '                    ' special handling FEM STATUS
    '                    '*
    '                    If LCase(FieldIDs(n)) = "bp21" Then
    '                        With MQFWS.Cells(startdatarow + i - 1, n + 1 + preheadercount).Validation
    '                            .purge()

    '                            .add(Type:=Excel.XlDVType.xlValidateList, AlertStyle:=Excel.XlDVAlertStyle.xlValidAlertStop, _
    '                        Operator:=Excel.XlFormatConditionOperator.xlBetween, Formula1:="=parameter_fem_status")
    '                            .IgnoreBlank = True
    '                            .InCellDropdown = True
    '                            .InputTitle = "Input"
    '                            .ErrorTitle = "Error"
    '                            .InputMessage = "Please enter the FEM status code"
    '                            .ErrorMessage = "Please provide correct FEM status code"
    '                            .ShowInput = True
    '                            .ShowError = True
    '                        End With
    '                    End If
    '                Next n
    '                For n = postheaderstart + 1 To postheaderstart + postheadercount
    '                    With MQFWS.Cells(startdatarow + i - 1, n)
    '                        .Value = ""
    '                        .Locked = True
    '                        .Interior.Color = constProcessBackground
    '                        .Font.Bold = True
    '                    End With
    '                Next n
    '                Globals.ThisAddIn.Application.StatusBar = " copy line " & i & " from OnTrack to message queue file"

    '            End If    ' ROW Not Nothing
    '        Next rowno

    '        '***
    '        '*** save parameters
    '        '***
    '        'set first data row in MQF
    '        flag = SetXlsParameterValueByName(name:="otdb_parameter_mqf_template_datastartrow", value:=startdatarow, workbook:=MQFWorkbook)
    '        'hermes_mqf_createdby
    '        flag = SetXlsParameterValueByName("hermes_mqf_createdby", Globals.ThisAddIn.Application.UserName, workbook:=MQFWorkbook)
    '        flag = SetXlsParameterValueByName("hermes_mqf_createdon", Converter.Date2LocaleShortDateString(Date.Now()), workbook:=MQFWorkbook)
    '        'parameter_doc9_extract_tooling
    '        'flag = SetXlsParameterValueByName("otdb_parameter_mqf_extract_tooling", getDoc9ToolingName(Globals.ThisAddIn.Application.Globals.ThisAddin.Application.Workbooks(currWorkbookName)), workbook:=MQFWorkbook)
    '        flag = SetXlsParameterValueByName("hermes_mqf_doc9used", dbDoc9Range.Worksheet.Parent.Name, workbook:=MQFWorkbook)
    '        flag = SetXlsParameterValueByName("hermes_mqf_doc9usedon", Converter.DateTime2LocaleDateTimeString(Date.Now), workbook:=MQFWorkbook)

    '        'flag = setXLSParameterValueByName("parameter_recent_ICD_change_date", _
    '        'MQFWorkbook.BuiltinDocumentProperties(12).value)
    '        '

    '        flag = SetXlsParameterValueByName("otdb_parameter_mqf_headerid_name", constMQFHeaderidName, workbook:=MQFWorkbook, silent:=True)
    '        ' update the description table
    '        Call updateMQFDescTable(MQFWorkbook, ROFieldIDs)

    '        ' Activate Matrix again

    '        Globals.ThisAddIn.Application.ScreenUpdating = True
    '        'MQFWorkbook.Activate
    '        'MQFWorkbook.Sheets(MQFWorksheetName).Activate
    '        ' autofilter
    '        MQFWS.Range(MQFWS.Cells(headerstartrow + 1, 1), MQFWS.Cells(headerstartrow + 1, MaxCol)).AutoFilter()
    '        ' protect
    '        MQFWorkbook.Sheets(MQFWorksheetName).Protect(password:=constPasswordTemplate, _
    '                                                     DrawingObjects:=False, Contents:=True, Scenarios:=False, _
    '                                                     AllowFormattingCells:=True, AllowFormattingColumns:=True, _
    '                                                     AllowFormattingRows:=True, AllowInsertingColumns:=True, AllowInsertingRows:=True, _
    '                                                     AllowDeletingColumns:=True, AllowDeletingRows:=True, _
    '                                                     AllowFiltering:=True, AllowUsingPivotTables:=True, AllowSorting:=True)
    '        '
    '        MQFWorkbook.Sheets(constParameterSheetName).Protect(password:=constPasswordTemplate, _
    '                                                            DrawingObjects:=False, Contents:=True, Scenarios:=False, _
    '                                                            AllowFormattingCells:=False, AllowFormattingColumns:=False, _
    '                                                            AllowFormattingRows:=False, AllowInsertingColumns:=False, AllowInsertingRows:=False, _
    '                                                            AllowDeletingColumns:=False, AllowDeletingRows:=False, _
    '                                                            AllowFiltering:=False, AllowUsingPivotTables:=False)


    '        '*
    '        'aProgressBar.closeForm()

    '        ' write the template
    '        MQFWorkbook.SaveAs(Filename:=Filename, FileFormat:=Excel.XlFileFormat.xlOpenXMLWorkbookMacroEnabled)
    '        If CloseAfterCreation Then
    '            MQFWorkbook.Close()
    '        Else
    '            MQFWorkbook = MQFWorkbook
    '        End If
    '        ' exit
    '        Globals.ThisAddIn.Application.Globals.ThisAddin.Application.Workbooks(currWorkbookName).Activate()
    '        createXlsDoc9MQF = True
    '        Exit Function

    'handleerror:
    '        Globals.ThisAddIn.Application.Globals.ThisAddin.Application.Workbooks(currWorkbookName).Activate()
    '        createXlsDoc9MQF = False
    '        Exit Function

    '    End Function


    ''' <summary>
    ''' retrieves the Column Descriptions from the XLS MQF File
    ''' </summary>
    ''' <param name="fieldlist"></param>
    ''' <param name="aName"></param>
    ''' <param name="workbook"></param>
    ''' <param name="silent"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMQFDBDesc(ByRef fieldlist() As XLSMQFColumnDescription, _
                                 Optional ByVal aName As String = "", _
                                 Optional workbook As Excel.Workbook = Nothing, _
                                 Optional silent As Boolean = False) As Boolean
        Dim headerids As Range
        Dim headerids_name As String
        Dim DescTable As Range
        Dim row As Range
        Dim i As Integer
        Dim Prefix As String
        Dim mqfversion As String

        If IsMissing(workbook) Then workbook = Globals.ThisAddIn.Application.ActiveWorkbook

        headerids_name = GetXlsParameterByName("otdb_parameter_mqf_headerid_name", workbook:=workbook)
        headerids = GetXlsParameterRangeByName(name:=headerids_name, workbook:=workbook)
        'parameter_doc9_dbdesc_prefix
        Prefix = GetXlsParameterByName("otdb_parameter_mqf_dbdesc_prefix", workbook:=workbook)
        mqfversion = GetXlsParameterByName("hermes_mqf_version", workbook, silent:=True)

        ' error
        If headerids Is Nothing Then
            Call CoreMessageHandler(showmsgbox:=True, message:="The parameter 'otdb_parameter_mqf_headerid_name':" & headerids_name & " is not showing a valid range !" _
                , procedure:="modXLSMessageQueueFile.getMQFDBDesc", messagetype:=otCoreMessageType.ApplicationError, break:=False)
            Return False
        End If

        DescTable = GetXlsParameterRangeByName("otdb_parameter_mqf_structure_db_description_table", workbook:=workbook, silent:=True)
        ' error
        If DescTable Is Nothing Then
            DescTable = GetXlsParameterRangeByName("otdb_parameter_mqf_db_description_table", silent:=True)
            If Not silent Then
                Call CoreMessageHandler(showmsgbox:=True, message:="The parameter 'otdb_parameter_mqf_structure_db_description_table' is not showing a valid range !" _
               , procedure:="modXLSMessageQueueFile.getMQFDBDesc", messagetype:=otCoreMessageType.ApplicationError, break:=False)

            End If
            Return False
        End If

        ' run through the rows
        i = -1
        For Each row In DescTable.Rows
            If Trim(row.Cells(1, xlsDBDescColNo.FieldID + 1).value) Like aName Or aName = "" Then
                i = i + 1
                ReDim Preserve fieldlist(i)
                fieldlist(i).ID = Trim(row.Cells(1, constMQFDescFieldID + 1).value)
                fieldlist(i).Title = Trim(row.Cells(1, constMQFDescTitle + 1).value)
                fieldlist(i).ColumnNo = CInt(row.Cells(1, constMQFDescColumnNo + 1).value)


                If mqfversion = "V_02" Or mqfversion = "V_03" Then
                    If row.Cells(1, constMQFReadonly + 1).value Is Nothing Then
                        fieldlist(i).READ_ONLY = False
                    Else
                        fieldlist(i).READ_ONLY = True
                    End If
                    'FieldList(i).READ_ONLY = Not IsEmpty(row.Cells(1, constMQFReadonly + 1).value)
                End If
                If mqfversion = "V_03" Then
                    fieldlist(i).Objectname = CStr(row.Cells(1, constMQFObjectName + 1).value)
                Else
                    fieldlist(i).Objectname = ""

                End If
                fieldlist(i).mqfversion = mqfversion
            End If
        Next row

        If i >= 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    '********
    '******** update the doc9 MQF DB Structure Description Table
    '********
    '********

    Public Sub UpdateMQFDescTable(WORKBOOK As Excel.Workbook, Optional ROFieldIDs As Object() = Nothing)
        Dim headerids As Range
        Dim headerids_name As String
        Dim DescTable As Range
        Dim cell As Range
        Dim i, j As Integer
        Dim startdesccell As Range
        Dim Prefix As String
        Dim pn As Name
        Dim mqfversion As String




        headerids_name = GetXlsParameterByName("otdb_parameter_mqf_headerid_name", WORKBOOK)
        headerids = GetXlsParameterRangeByName(headerids_name, WORKBOOK)
        'parameter_doc9_dbdesc_prefix
        Prefix = GetXlsParameterByName("otdb_parameter_mqf_dbdesc_prefix", WORKBOOK, silent:=True)
        mqfversion = GetXlsParameterByName("hermes_mqf_version", WORKBOOK, silent:=True)
        If mqfversion = "V_01" Then
            Call SetXlsParameterValueByName("hermes_mqf_version", "V_02", WORKBOOK, silent:=True)
        End If

        ' error
        If headerids Is Nothing Then
            Call CoreMessageHandler(showmsgbox:=True, message:="The parameter 'otdb_parameter_mqf_headerid_name':" & headerids_name & " is not showing a valid range !" _
              , procedure:="modXLSMessageQueueFile.updateMQFDBDescTable", messagetype:=otCoreMessageType.ApplicationError, break:=False)

            Exit Sub
        End If

        DescTable = GetXlsParameterRangeByName("otdb_parameter_mqf_structure_db_description_table", workbook:=WORKBOOK, silent:=True)
        ' error
        If DescTable Is Nothing Then
            DescTable = GetXlsParameterRangeByName("otdb_parameter_mqf_db_description_table", workbook:=WORKBOOK, silent:=True)
            If DescTable Is Nothing Then

                Call CoreMessageHandler(showmsgbox:=True, message:="The parameter 'otdb_parameter_mqf_structure_db_description_table' is not showing a valid range !" _
                , procedure:="modXLSMessageQueueFile.updateMQFDBDescTable", messagetype:=otCoreMessageType.ApplicationError, break:=False)


                Exit Sub
            End If
        End If


        ' upper right corner
        startdesccell = DescTable(1, 1)

        ' run through the headerids
        i = 0
        For Each cell In headerids
            ' insert new row
            If i >= DescTable.Rows.Count Then
                startdesccell.Offset(i, 0).EntireRow.Insert()
            End If
            ' Header ID
            startdesccell.Offset(i, constMQFDescFieldID).Value = cell.Value
            If Trim(cell.Value) = "" Then
                startdesccell.Offset(i, constMQFDescFieldID).Interior.Color = constErrorBackground
            Else
                startdesccell.Offset(i, constMQFDescFieldID).Interior.Color = startdesccell.Offset(0, 0).Interior.Color
            End If
            ' Description
            startdesccell.Offset(i, constMQFDescTitle).Value = cell.Offset(1, 0).Value
            ' Column
            startdesccell.Offset(i, constMQFDescColumnNo).Value = i + 1
            If NameExistsinWorkbook(startdesccell.Worksheet.Parent, Prefix & cell.Value) Then
                pn = DescTable.Worksheet.Parent.Names(Prefix & cell.Value)
                pn.Delete()
            End If
            DescTable.Worksheet.Parent.Names.add( _
                    Name:=Prefix & cell.Value, RefersTo:=startdesccell.Offset(i, 2))
            ' readolny
            If IsArrayInitialized(ROFieldIDs) Then
                For j = LBound(ROFieldIDs) To UBound(ROFieldIDs)
                    If ROFieldIDs(j) = cell.Value Then
                        startdesccell.Offset(i, constMQFReadonly).Value = "Read-only"
                        Exit For
                    Else
                        startdesccell.Offset(i, constMQFReadonly).Value = ""
                    End If
                Next j
            Else
                startdesccell.Offset(i, constMQFReadonly).Value = ""
            End If
            ' inc
            i = i + 1
        Next cell

        DescTable = startdesccell.Worksheet.Range(startdesccell, startdesccell.Offset(i - 1, 2))
        If Not SetXlsParameterValueByName("parameter_mfq_dbdesc_range", DescTable.Address, WORKBOOK) Then System.Diagnostics.Debug.WriteLine("parameter_doc9_dbdesc_range doesnot exist ?!")
        ' delete
        If NameExistsinWorkbook(startdesccell.Worksheet.Parent, "otdb_parameter_mqf_structure_db_description_table") Then
            pn = DescTable.Worksheet.Parent.Names("otdb_parameter_mqf_structure_db_description_table")
            pn.Delete()
        End If
        ' define
        DescTable.Worksheet.Parent.Names.add( _
                Name:="otdb_parameter_mqf_structure_db_description_table", RefersTo:=DescTable)


        Globals.ThisAddIn.Application.StatusBar = " Parameter MFQ Structure Database Description updated"
    End Sub


    ' ***************************************************************************************************
    '  Subroutine to Locate a Doc9-Document Message Queue File in the Globals.ThisAddin.Application
    '
    ' return the filename with path

    Public Function selectNewXLSMQF() As String
        Dim FileNamePattern, FileNamePattern2, currWorkbookName, MQFWorkbookName, MQFSheetName As String
        Dim filenames As Object
        Dim flag, Doc9sheet_flag As Boolean
        Dim MQFWorkbook As Excel.Workbook, wb As Excel.Workbook
        Dim ws As Excel.Worksheet
        Dim Value As Object


        ' get Doc9 File Name
        FileNamePattern = GetDBParameter("otdb_parameter_mqfilename_prefix", silent:=True)
        FileNamePattern = FileNamePattern & Globals.ThisAddIn.Application.UserName & "_" & Format(Date.Now(), "yyyy-mm-dd") & "_" & Format(Date.Now(), "hhmm")

        'Open Dialog for Find
        Value = GetDBParameter("parameter_startfoldernode")
        If Value <> "" And FileIO.FileSystem.FileExists(Value) Then
            If Mid(Value, Len(Value), 1) <> "\" Then Value = Value & "\"
            If Mid(Value, 2, 1) = ":" Then
                ChDrive(Mid(Value, 1, 2))
            End If
            Value = Value & GetDBParameter("otdb_parameter_mqf_output")
            If FileIO.FileSystem.FileExists(Value) Then
                ChDir(Value)
            End If
        End If
        filenames = Globals.ThisAddIn.Application.GetSaveAsFilename(InitialFilename:=FileNamePattern & ".xlsm", _
                                                  Title:="Create a New OnTrack Message Queue File")

        ' User aborted
        If filenames = False Then
            selectNewXLSMQF = ""
            Exit Function
        Else
            ' check with or without .
            If InStrRev(filenames, ".") = 0 Then
                filenames = filenames & ".xlsm"
            Else
                ' exchange
                Value = Mid(filenames, InStrRev(filenames, "."), Len(filenames))
                ' add it to xlsm
                If LCase(Value) <> ".xlsm" And (LCase(Value) Like ".xl*" Or Value = ".") Then
                    filenames = Mid(filenames, 1, InStrRev(filenames, ".") - 1) & ".xlsm"
                End If
            End If
            selectNewXLSMQF = filenames
        End If




    End Function



    ''' <summary>
    ''' preprocess an xls message queue file and builds a message queue structure
    ''' 
    ''' </summary>
    ''' <param name="MQFWorkbook"></param>
    ''' <param name="messagequeue"></param>
    ''' <param name="workerthread"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function BuildXLSMessageQueueObject(ByRef MQFWorkbook As Excel.Workbook, ByRef [messagequeue] As MessageQueue, _
                                     Optional ByRef workerthread As BackgroundWorker = Nothing,
                                     Optional domainid As String = Nothing, _
                                     Optional persist As Boolean = False) As Boolean
        Dim aVAlue As Object
        Dim headerstartrow As Integer
        Dim headerids_name As String
        Dim headerids As Range
        Dim Prefix As String
        Dim cell As Range
        Dim DescTable As Range
        Dim datawsname As String
        Dim dataws As Excel.Worksheet
        Dim theXLSMQFFormat As XLSMQFStructure
        Dim theXLSMQFColumns() As XLSMQFColumnDescription
        Dim startrow As Integer
        Dim foundflag As Boolean

        'Dim mfgStatus As New clsMQFStatus
        Dim changeflag As Boolean
        Dim setro_flag As Boolean
        'Dim newStatus As New clsMQFStatus

        Dim aprocessdate As DateTime
        Dim aXCMD As otXChangeCommandType
        Dim aColumnNo As Long
        Dim theUIDCol As Long
        Dim anObjectName As String
        Dim anID As String
        Dim n As Long
        Dim mqfDBRange As Range
        Dim maximum As Long
        Dim progress As Long = 0
        Dim aStopwatch As New Diagnostics.Stopwatch


        Try
            If Not ot.IsConnected Then
                If Not ot.Startup(accessRequest:=otAccessRight.ReadUpdateData, domainID:=domainid, _
                                   messagetext:="For creating a MQF data structure out of an excel file you need access to Ontrack Database. Please login.") Then
                    CoreMessageHandler(showmsgbox:=True, procedure:="modXLSMessageQueueFile.preProcessXLSMQF", messagetype:=otCoreMessageType.ApplicationError, _
                                        message:="the necessary right to run the operation was not granted by the ontrack session - operation aborted.")
                    Return False
                End If
            Else
                If String.IsNullOrWhiteSpace(domainid) Then domainid = CurrentSession.CurrentDomainID
                If Not ot.CurrentSession.RequireAccessRight(otAccessRight.ReadUpdateData, domainID:=domainid) Then
                    CoreMessageHandler(showmsgbox:=True, procedure:="modXLSMessageQueueFile.preProcessXLSMQF", messagetype:=otCoreMessageType.ApplicationError, _
                                        message:="the necessary right to run the operation was not granted by the ontrack session - operation aborted.")
                    Return False
                Else
                    If CurrentSession.CurrentDomainID <> domainid Then
                        CurrentSession.SwitchToDomain(domainid)
                    End If
                End If
            End If

            ' cache the MQFWOrkbook
            CacheAllWorkbookNames(MQFWorkbook)

            ' start watch
            aStopwatch.Start()

            '''
            ''' Step0: Get all the Parameters from the MQF Excel
            '''
            If [messagequeue] Is Nothing OrElse Not [messagequeue].IsAlive(throwError:=False) Then
                ' check if we have one
                aVAlue = GetXlsParameterByName(name:="otdb_parameter_mqf_tag", workbook:=MQFWorkbook, silent:=True)
                If String.IsNullOrWhiteSpace(aVAlue) Then aVAlue = GetHostProperty("otdb_parameter_mqf_tag", host:=MQFWorkbook, silent:=True)
                If String.IsNullOrWhiteSpace(aVAlue) Then aVAlue = MQFWorkbook.Name & " " & CStr(Now)
                ''' create a message queue in the backend
                [messagequeue] = XChange.MessageQueue.Create(id:=aVAlue, runtimeOnly:=True)
                If [messagequeue] Is Nothing Then [messagequeue] = XChange.MessageQueue.Retrieve(id:=aVAlue)

                If messagequeue Is Nothing Then
                    CoreMessageHandler(showmsgbox:=True, message:="the messagequeue is not creatable in the backend  - operation aborted for '" _
                                  & MQFWorkbook.FullName & "'." & vbLf & ".", _
                                   procedure:="modXLSMessageQueueFile.preProcessXLSMQF", messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If

                ''' clear the queue for cases retrieved and not loaded
                If messagequeue.IsCreated Then messagequeue.Clear()
                ''' set the context identifier
                [messagequeue].ContextIdentifier = aVAlue
            End If

            ''' fill the message queue object
            ''' 
            With messagequeue
                Call SetXlsParameterValueByName("otdb_parameter_mqf_tag", .ID, workbook:=MQFWorkbook, silent:=True)
                Call SetHostProperty("otdb_parameter_mqf_tag", .ID, host:=MQFWorkbook, silent:=True)
                .RequestedBy = GetXlsParameterByName(name:="hermes_mqf_requestedby", workbook:=MQFWorkbook, silent:=True)
                .RequestedByOU = GetXlsParameterByName(name:="hermes_mqf_requestedby_department", workbook:=MQFWorkbook, silent:=True)
                aVAlue = GetXlsParameterByName(name:="hermes_mqf_requested_on", workbook:=MQFWorkbook, silent:=True)
                If Not IsDate(aVAlue) Then
                    .RequestedOn = Date.Now()
                Else
                    .RequestedOn = CDate(aVAlue)
                End If
                .Title = GetXlsParameterByName(name:="hermes_mqf_title", workbook:=MQFWorkbook, silent:=True)
                .Description = GetXlsParameterByName(name:="hermes_mqf_subject", workbook:=MQFWorkbook, silent:=True)
                If String.IsNullOrWhiteSpace(domainid) Then
                    aVAlue = GetXlsParameterByName(name:="hermes_mqf_domainid", workbook:=MQFWorkbook, silent:=True)
                    If aVAlue IsNot Nothing Then
                        .DomainID = aVAlue
                    Else
                        .DomainID = CurrentSession.CurrentDomainID
                    End If
                Else
                    .DomainID = domainid
                End If
            End With

            ''' fill the xls super structure although obsolete
            ''' 
            With theXLSMQFFormat
                .Title = messagequeue.Title
                .requestedbyDept = messagequeue.RequestedByOU
                .requestedBy = messagequeue.RequestedBy
                .requestedOn = messagequeue.RequestedOn
                .Requestfor = messagequeue.Description
            End With

            '''
            ''' check where the headerids are
            ''' 
            aVAlue = modParameterXLS.GetXlsParameterByName("otdb_parameter_mqf_template_headerstartrow", workbook:=MQFWorkbook)
            If Not IsNumeric(aVAlue) Then
                CoreMessageHandler(showmsgbox:=True, message:="the parameter 'otdb_parameter_mqf_template_headerstartrow' was not found in the mqf workbook '" _
                                   & MQFWorkbook.FullName & "'." & vbLf & "Check if the file is an MQF. Operation aborted.", _
                                    procedure:="modXLSMessageQueueFile.preProcessXLSMQF", messagetype:=otCoreMessageType.ApplicationError)
                Return False
            End If

            headerstartrow = CInt(aVAlue)
            headerids_name = GetXlsParameterByName(name:="otdb_parameter_mqf_headerid_name", workbook:=MQFWorkbook)
            headerids = GetXlsParameterRangeByName(headerids_name, workbook:=MQFWorkbook)
            Prefix = GetXlsParameterByName(name:="otdb_parameter_mqf_dbdesc_prefix", workbook:=MQFWorkbook)
            If headerids Is Nothing Then
                Call CoreMessageHandler(showmsgbox:=True, _
                                      message:="The parameter 'otdb_parameter_mqf_headerid_name':" & headerids_name & " is not showing a valid range !" _
                                    , procedure:="modXLSMessageQueueFile.preProcessXLSMQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)

                Return False
            End If

            '''
            ''' Check the description table for the Database Columns
            ''' 
            DescTable = GetXlsParameterRangeByName(name:="otdb_parameter_mqf_structure_db_description_table", workbook:=MQFWorkbook, silent:=True)
            If DescTable Is Nothing Then
                Call CoreMessageHandler(showmsgbox:=True, _
                                     message:="The parameter 'otdb_parameter_mqf_structure_db_description_table' is not showing a valid range ! - Operation aborted" _
                                   , procedure:="modXLSMessageQueueFile.preProcessXLSMQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)

                Return False
            End If

            Dim aMQFDBDescLookup As New Dictionary(Of String, XLSMQFColumnDescription)

            ''' fill all XLS DB Column Descriptions
            If GetMQFDBDesc(theXLSMQFColumns, workbook:=MQFWorkbook) Then
                For Each anMQFDBDescEntry As XLSMQFColumnDescription In theXLSMQFColumns
                    If Not aMQFDBDescLookup.ContainsKey(anMQFDBDescEntry.ID) Then
                        aMQFDBDescLookup.Add(key:=anMQFDBDescEntry.ID, value:=anMQFDBDescEntry)
                    End If
                Next
                ' save it
                theXLSMQFFormat.desc = theXLSMQFColumns
            End If

            Globals.ThisAddIn.Application.EnableEvents = False
            Globals.ThisAddIn.Application.ScreenUpdating = False

            '''
            ''' load or create the xchange configuration if a name is given
            ''' 
            aVAlue = GetXlsParameterByName(name:="otdb_parameter_mqf_xchangeconfigname", workbook:=MQFWorkbook, silent:=True)
            If aVAlue Is Nothing OrElse aVAlue = "" Then aVAlue = [messagequeue].ID

            Dim aXconfig = XChange.XChangeConfiguration.Retrieve(configname:=aVAlue.toupper)
            If aXconfig IsNot Nothing Then
                [messagequeue].XChangeConfig = aXconfig
            Else
                [messagequeue].XChangeConfig = XChange.XChangeConfiguration.Create(configname:=aVAlue.toupper)
                [messagequeue].XChangeConfig.AllowDynamicEntries = True
            End If

            ''' clear the Xchange Config if dynamic
            If [messagequeue].XChangeConfig.AllowDynamicEntries Then messagequeue.XChangeConfig.ClearEntries()

            '*** HACK
            Call [messagequeue].XChangeConfig.AddObjectByName(name:=Deliverables.Deliverable.ConstObjectID, _
                                                              xcmd:=otXChangeCommandType.Update)

            ''' do not persist the xchange configuration since we are only dynamic here
            ''' If messagequeue.XChangeConfig.IsCreated Then [messagequeue].XChangeConfig.Persist()

            '''
            ''' STEP1 : Check the headerids
            '''


            ''' go through the header ids
            Dim i As Integer = 0
            For Each cell In headerids
                If cell.Value <> "" And Not IsError(cell.Value) Then
                    ' resolve the ID
                    anID = CStr(cell.Text)
                    'Diagnostics.Debug.WriteLine(anID)

                    '' try to get a MQFDBDesc in the parameters
                    ''
                    If aMQFDBDescLookup.ContainsKey(anID) Then
                        Dim anMQFDBDescEntry As XLSMQFColumnDescription = aMQFDBDescLookup.Item(key:=anID)
                        aColumnNo = anMQFDBDescEntry.ColumnNo
                        anObjectName = anMQFDBDescEntry.Objectname
                        ' build or reset the ordinal for the config
                        ' preset the operation
                        If anMQFDBDescEntry.READ_ONLY Then
                            aXCMD = otXChangeCommandType.Read
                        Else
                            aXCMD = otXChangeCommandType.Update
                        End If
                        ' if not
                    Else
                        aColumnNo = cell.Column
                        aXCMD = otXChangeCommandType.Read     'assumption
                        anObjectName = ""
                    End If


                    ' special cells
                    If LCase(cell.Value) = "uid" Then    ' 
                        theUIDCol = aColumnNo
                        theXLSMQFFormat.UIDCol = aColumnNo
                        [messagequeue].UIDOrdinal = aColumnNo
                    ElseIf LCase(cell.Value) = "mqfx2" Then
                        [messagequeue].ProcessDateordinal = aColumnNo
                        theXLSMQFFormat.processdatecol = aColumnNo
                    ElseIf LCase(cell.Value) = "mqfx3" Then
                        [messagequeue].ProcessLogordinal = aColumnNo
                        theXLSMQFFormat.processlogcol = aColumnNo
                    ElseIf LCase(cell.Value) = "mqfx4" Then
                        [messagequeue].ProcessStatusordinal = aColumnNo
                        theXLSMQFFormat.processstatuscol = aColumnNo
                    ElseIf LCase(cell.Value) = "mqfxaction" Then
                        [messagequeue].ActionOrdinal = aColumnNo
                        theXLSMQFFormat.ActionCol = aColumnNo
                    End If

                    '''
                    ''' extend the xchange config
                    ''' 
                    If [messagequeue].XChangeConfig.IsLoaded Then
                        Call [messagequeue].XChangeConfig.SetOrdinalForXID(anID, aColumnNo)
                    ElseIf [messagequeue].XChangeConfig.IsCreated Then
                        ' add Attribute by ID
                        Call [messagequeue].XChangeConfig.AddEntryByXID(Xid:=anID, _
                                                                      ordinal:=CLng(aColumnNo), _
                                                                      objectname:=anObjectName, _
                                                                      xcmd:=aXCMD)    ' theMQFFormat.desc(i).ColumnNo
                    Else
                        Call CoreMessageHandler(procedure:="MQF.preProcessXLSMQF", _
                                                message:="xChangeConfig is neither created nor loaded")

                    End If


                    '** increment
                    i = i + 1

                End If

            Next cell

            ' set the READ_ONLY flag to be determined
            setro_flag = False
            ' add change tag
            theXLSMQFFormat.processchangetag = MQFWorkbook.Name

            ' get startrow
            startrow = GetXlsParameterByName("otdb_parameter_mqf_template_datastartrow", workbook:=MQFWorkbook)
            ' assumption where to start ?!
            datawsname = GetXlsParameterByName("otdb_parameter_mqf_templatedata", workbook:=MQFWorkbook, found:=foundflag, silent:=False)
            If Not foundflag Then
                dataws = headerids.Worksheet
            Else
                dataws = MQFWorkbook.Sheets(datawsname)
            End If
            ' the full sheet
            '' go in the uid column from the bottom upwards
            Dim maxrow As Integer = dataws.Cells(dataws.Rows.Count, theUIDCol).End(Excel.XlDirection.xlUp).row
            '' go from the headid row right-most to the left
            Dim MaxCol As Integer = dataws.Cells(headerids.Row, dataws.Columns.Count).End(Excel.XlDirection.xlToLeft).Column
            ' set it
            mqfDBRange = dataws.Range(dataws.Cells(startrow, 1), dataws.Cells(maxrow, MaxCol))
            ' autofilter off
            dataws.AutoFilterMode = False

            ' init worker
            If workerthread IsNot Nothing Then
                workerthread.ReportProgress(0, "")
                maximum = maxrow - startrow + 1
            End If

            '**** through all
            '****
            n = 0

            ' get the attributes
            Dim listOfXEntries As IEnumerable(Of IXChangeConfigEntry) = [messagequeue].XChangeConfig.GetObjectEntries
            Dim aMQFXLSValueRange As Object = CType(mqfDBRange, Excel.Range).Value

            If Not aMQFXLSValueRange.GetType.IsArray OrElse (aMQFXLSValueRange.GetType.IsArray AndAlso aMQFXLSValueRange.GetType.GetArrayRank <> 2) Then
                CoreMessageHandler(message:="No Array Table of MQF Data Input", argument:=mqfDBRange.Address, procedure:="modXLSMessageQueueFile.preProcessXLSMQF", _
                                   messagetype:=otCoreMessageType.ApplicationError)
                Return False
            End If


            '''
            ''' STEP 2:  thorugh 2-dimensional array from MQF and build MQMessages
            '''

            Dim aStopWatch2 As New Diagnostics.Stopwatch
            aStopWatch2.Start()
            Dim aStopWatch3 As New Diagnostics.Stopwatch
            Dim aStopWatch4 As New Diagnostics.Stopwatch

            For rowno As ULong = LBound(aMQFXLSValueRange, 1) To UBound(aMQFXLSValueRange, 1)
                aStopWatch3 = New Diagnostics.Stopwatch
                aStopWatch3.Start()

                changeflag = False
                Dim aMQMessage As MQMessage = [messagequeue].CreateMessage(no:=rowno)

                ' set the msglog identifiers
                aMQMessage.ContextIdentifier = [messagequeue].ID
                aMQMessage.TupleIdentifier = mqfDBRange.Rows(rowno).address

                '** progress
                If workerthread IsNot Nothing Then
                    progress += 1
                    workerthread.ReportProgress((progress / maximum) * 100, "reading row #" & rowno)
                End If

                '** Application Bar
                Globals.ThisAddIn.Application.StatusBar = " reading from " & MQFWorkbook.Name & " row#" & rowno

                '***** ACTION COMMAND
                '*****
                If messagequeue.ActionOrdinal IsNot Nothing Then
                    aVAlue = aMQFXLSValueRange(rowno, [messagequeue].ActionOrdinal)
                    If aVAlue IsNot Nothing Then
                        aMQMessage.Action = aVAlue.ToString.Trim.ToUpper
                        aMQMessage.IsActionProcessable()
                    Else
                        '513;@;MQF;message operation is missing - message not processed;;99;Error;false;|R1|S1|;|XCHANGEENVELOPE|MQMESSAGE|
                        aMQMessage.ObjectMessageLog.Add(513, Nothing, Nothing, Nothing, Nothing, aMQMessage)
                    End If


                End If


                '** already processed -> Process Date
                '**
                If messagequeue.ProcessDateordinal IsNot Nothing Then
                    aVAlue = aMQFXLSValueRange(rowno, [messagequeue].ProcessDateordinal)
                    If IsDate(aVAlue) Then
                        aprocessdate = CDate(aVAlue)
                    Else
                        aprocessdate = ConstNullDate
                    End If
                Else
                    aprocessdate = ConstNullDate
                End If

                '** already processed -> Status
                '**
                If messagequeue.ProcessStatusordinal IsNot Nothing Then
                    aVAlue = aMQFXLSValueRange(rowno, [messagequeue].ProcessStatusordinal)
                    If aVAlue IsNot Nothing Then
                        Dim aStatusItem As Commons.StatusItem = Commons.StatusItem.Retrieve(typeid:=ConstStatusType_MQMessage, code:=aVAlue.ToString)
                        If aStatusItem IsNot Nothing Then
                            If Not aStatusItem.Aborting Then
                                '571;@;MQF;message in row %Tupleidentifier% was already completely processed on %1% - skip processing;;70;Error;false;|Y1|S1|;|XCHANGEENVELOPE|MQMESSAGE|
                                aMQMessage.ObjectMessageLog.Add(571, Nothing, Nothing, Nothing, Nothing, aMQMessage, aprocessdate)
                            Else
                                '572;@;MQF;message in row %Tupleidentifier% was already processed on %1% with errors ;;01;Error;false;|Y1|G2|;|XCHANGEENVELOPE|MQMESSAGE|
                                aMQMessage.ObjectMessageLog.Add(572, Nothing, Nothing, Nothing, Nothing, aMQMessage, aprocessdate)
                            End If
                        End If
                    End If
                End If



                '** phase1 in row : run through all fields of the row to get a full message
                '**
                aColumnNo = 0

                For Each aXChangeEntry As IXChangeConfigEntry In listOfXEntries.Where(Function(x) x.IsXChanged = True).ToList
                    Dim fillSlot As Boolean = True

                    ' get the mapping to the Column
                    If IsNumeric(aXChangeEntry.Ordinal.Value) Then
                        aColumnNo = CLng(aXChangeEntry.Ordinal.Value)
                    Else
                        aColumnNo = aColumnNo + 1
                    End If

                    ' get aValue in the bounds
                    If rowno >= aMQFXLSValueRange.getlowerbound(0) AndAlso rowno <= aMQFXLSValueRange.getupperbound(0) AndAlso _
                        aColumnNo >= aMQFXLSValueRange.getlowerbound(1) AndAlso aColumnNo <= aMQFXLSValueRange.getupperbound(1) Then
                        '*** get the value
                        aVAlue = aMQFXLSValueRange(rowno, aColumnNo)
                        '*** remove any whitespaces which are disturbing
                        If aVAlue IsNot Nothing Then aVAlue = Trim(aVAlue)
                        '*** basic transforms
                        If IsEmpty(aVAlue) Then
                            aVAlue = Nothing
                            fillSlot = True
                        ElseIf IsDate(aVAlue) Then
                            aVAlue = CDate(aVAlue)
                            fillSlot = True
                        ElseIf IsNumeric(aVAlue) Then
                            aVAlue = CDbl(aVAlue)
                            fillSlot = True
                        ElseIf IsError(aVAlue) Then
                            '501;@;MQF;cell value '%2%' of column %1% in %Tupleidentifier% has excel error
                            aMQMessage.ObjectMessageLog.Add(501, Nothing, Nothing, Nothing, Nothing, aMQMessage, aColumnNo, aVAlue)
                            fillSlot = False
                        End If

                    Else
                        ''' we have a xchangeentry with an ordinal (columno) which is not in the range of the line
                        ''' might be added later and not xchanged
                        fillSlot = False
                    End If


                    '**
                    '** store the aValues it
                    ' theMessages(n).fieldvalues(i) = aValue
                    If fillSlot Then
                        aStopWatch4 = New Diagnostics.Stopwatch
                        aStopWatch4.Start()

                        Dim aMQSlot As MQXSlot = aMQMessage.CreateAddedSlot(aColumnNo)

                        aStopWatch4.Stop()
                        'Diagnostics.Debug.WriteLine("> addSlot " & aStopWatch4.ElapsedMilliseconds)

                        ' create a new Member in the Message

                        With aMQSlot
                            If aVAlue Is Nothing Then
                                .IsNull = False
                                .IsEmpty = True
                            ElseIf aVAlue.ToString = constMQFClearFieldChar Then
                                .IsNull = True
                                .IsEmpty = False
                                .Value = Nothing
                            Else
                                .IsNull = False
                                .IsEmpty = False
                                .Value = aVAlue
                            End If

                            .ContextIdentifier = [messagequeue].ID
                            .TupleIdentifier = mqfDBRange.Rows(rowno).address
                            .EntityIdentifier = mqfDBRange.Cells(rowno, aColumnNo).address
                            changeflag = changeflag Or True
                        End With
                    Else
                        ''# slot not filled
                    End If

                Next aXChangeEntry    ' run through fields
                aStopWatch3.Stop()
                Diagnostics.Debug.WriteLine(rowno & ". message line build in " & aStopWatch3.ElapsedMilliseconds & "ms")

                ' reset the processable flag if no change -> ""
                If Not changeflag Then
                    '570;@;MQF;message in row %Tupleidentifier% has no changed values - skip processing;;70;Error;false;|Y1|R1|;|XCHANGEENVELOPE|MQMessage|
                    aMQMessage.ObjectMessageLog.Add(570, Nothing, Nothing, Nothing, Nothing, aMQMessage)
                End If

                ' increase
                n = n + 1
            Next rowno

            aStopWatch2.Stop()
            Globals.ThisAddIn.Application.EnableEvents = True
            Globals.ThisAddIn.Application.ScreenUpdating = True

            aStopwatch.Stop()
            CoreMessageHandler(message:="mqf build from excel file " & MQFWorkbook.Name & " in " & aStopwatch.ElapsedMilliseconds & "ms", _
                                procedure:="modXLSMessageQueueFile.BuildXLSMessageQueueObject", messagetype:=otCoreMessageType.ApplicationInfo)


            ''' save the MQF
            If persist Then
                If workerthread IsNot Nothing Then workerthread.ReportProgress(100, "saving MQF in OnTrack Database ...")
                ' Persist
                [messagequeue].Persist()
                If workerthread IsNot Nothing Then workerthread.ReportProgress(100, "saved MQF in OnTrack Database ...")
            End If

            ''' finish
            Call CoreMessageHandler(message:="message queue built from '" & MQFWorkbook.Name & "'", _
                                    procedure:="modXLSMessageQueueFile.buildMessageQueue", messagetype:=otCoreMessageType.ApplicationInfo)
            'return
            Return True

        Catch ex As Exception
            Globals.ThisAddIn.Application.EnableEvents = True
            Globals.ThisAddIn.Application.ScreenUpdating = True
            CoreMessageHandler(messagetype:=otCoreMessageType.ApplicationException, exception:=ex, procedure:="modXLSMessageQueueFile.preProcessXLSMQF")
            Return False
        End Try

    End Function


    ' ***************************************************************************************************
    '  add a Field Status to the Message
    '
    '  aStatusCode
    '  aLog


    'Public Function GetStatus(theMessage As MQFMessage) As clsMQFStatus

    '    Dim fieldstatus() As Object
    '    Dim aStatus As New clsMQFStatus
    '    Dim i As Integer


    '    If theMessage.status Is Nothing Then
    '        theMessage.status = aStatus
    '    End If


    '    ' if Field Arrays
    '    If IsArrayInitialized(theMessage.fieldstatus) Then
    '        ' lookthrough
    '        For i = 0 To UBound(theMessage.fieldstatus)
    '            If Not IsEmpty(theMessage.fieldstatus(i)) Then
    '                ' if to be approved and approved
    '                If theMessage.fieldstatus(i).code = constStatusCode_forapproval And theMessage.isApproved Then
    '                    System.Diagnostics.Debug.WriteLine("was approved")

    '                    aStatus.code = constStatusCode_processed_ok    ' maybe is approved
    '                Else
    '                    ' else the weight is ok
    '                    If aStatus.weight < theMessage.fieldstatus(i).weight Then
    '                        aStatus = theMessage.fieldstatus(i)
    '                    End If
    '                End If
    '            End If
    '        Next i
    '        If aStatus.weight > theMessage.status.weight Then
    '            getStatus = aStatus
    '        Else
    '            If theMessage.status.code = constStatusCode_forapproval And theMessage.isApproved Then
    '                aStatus.code = constStatusCode_processed_ok
    '                getStatus = aStatus
    '            Else
    '                getStatus = theMessage.status
    '            End If
    '        End If
    '        Exit Function
    '    Else
    '        getStatus = theMessage.status
    '        Exit Function
    '    End If

    'End Function

    ' ***************************************************************************************************
    '  add a Field Status to the Message
    '
    '  aStatusCode
    '  aLog


    'Public Function addFieldStatus(theMessage As MQFMessage, ByVal aFieldindex As Integer, _
    '                               ByVal aStatusCode As String, ByVal aLog As String) As Boolean

    '    Dim fieldstatus() As Object
    '    Dim fieldlog() As Object
    '    Dim newStatus As New clsMQFStatus

    '    ' initialize Arrays
    '    If Not IsArrayInitialized(theMessage.fieldstatus) Then
    '        ReDim fieldstatus(UBound(theMessage.fieldvalues))
    '        theMessage.fieldstatus = fieldstatus
    '    End If
    '    ' initialize Arrays
    '    If Not IsArrayInitialized(theMessage.fieldlog) Then
    '        ReDim fieldlog(UBound(theMessage.fieldvalues))
    '        theMessage.fieldlog = fieldlog
    '    End If

    '    ' add status
    '    newStatus.code = aStatusCode
    '    theMessage.fieldstatus(aFieldindex) = newStatus
    '    ' add log
    '    theMessage.fieldlog(aFieldindex) = addLog(theMessage.fieldlog(aFieldindex), aLog)

    '    addFieldStatus = True

    'End Function

    ' ***************************************************************************************************
    '  checkOnMQFAscendingDates check the Date fields if ascending
    '
    '  theMQFFormat as Messageformat
    '  theMessage as Message

    'Public Function checkOnMQFAscendingDates(theMQFFormat As XLSMQFStructure, theMessage As MQFMessage) As Boolean
    '    Dim Value As Object
    '    Dim MSG As String
    '    Dim date1, date2 As Object
    '    Dim dateids() As String
    '    Dim dateinputids() As String
    '    Dim dateindex() As Integer

    '    Dim datefields As String
    '    Dim i, j, n As Integer


    '    datefields = GetDBParameter("parameter_plausibility_fc_asc_dates")
    '    If datefields = "" Then
    '        checkOnMQFAscendingDates = True
    '        Exit Function
    '    End If

    '    dateinputids = SplitMultiDelims(text:=datefields, DelimChars:=constDelimeter)
    '    n = 1
    '    If IsArrayInitialized(dateinputids) Then

    '        For i = 1 To UBound(dateinputids)
    '            If InStr(dateinputids(i), constDelimeter) = 0 Then
    '                dateinputids(i) = LCase(Trim(dateinputids(i)))
    '                ' order in crossreference dateindex
    '                For j = 0 To UBound(theMQFFormat.desc)
    '                    If LCase(theMQFFormat.desc(j).ID) = dateinputids(i) Then
    '                        ReDim Preserve dateindex(n)
    '                        ReDim Preserve dateids(n)
    '                        dateindex(n) = j
    '                        dateids(n) = theMQFFormat.desc(j).ID
    '                        n = n + 1
    '                        Exit For
    '                    End If
    '                Next j
    '            End If
    '        Next i
    '    Else
    '        checkOnMQFAscendingDates = True
    '        Exit Function
    '    End If

    '    checkOnMQFAscendingDates = True

    '    '**
    '    '** now check on the dates ascending
    '    For i = 1 To UBound(dateindex)
    '        '** check only on Doc9Fields
    '        date1 = theMessage.fieldvalues(dateindex(i))

    '        If IsDate(date1) Then
    '            If i < UBound(dateindex) Then
    '                For j = i + 1 To UBound(dateindex)
    '                    If IsDate(theMessage.fieldvalues(dateindex(j))) Then
    '                        date2 = theMessage.fieldvalues(dateindex(j))
    '                        Exit For
    '                    Else
    '                        date2 = Null()
    '                    End If
    '                Next j
    '            Else
    '                Exit For
    '            End If

    '            If IsDate(date2) Then

    '                '** check the difference in days
    '                Value = DateDiff("d", date1, date2)
    '                If Value > 0 Then
    '                    System.Diagnostics.Debug.WriteLine("checking date " & dateids(i) & " with " & dateids(j) & " : " & Value)
    '                ElseIf Value = 0 Then
    '                    MSG = "Warning: for uid #" & theMessage.UID & " date of field " & dateids(i) & "(" & Format(date1, "dd.mm.yyyy") & ")" & _
    '                          " is the same as date of field " & dateids(j) & "(" & Format(date2, "dd.mm.yyyy") & ")"
    '                    theMessage.log = addLog(theMessage.log, MSG)
    '                    theMessage.status = New clsMQFStatus
    '                    theMessage.status.code = constStatusCode_processed_warnings
    '                    theMessage.processable = theMessage.status.isProcessed And theMessage.processable
    '                    If addFieldStatus(theMessage, i, theMessage.status.code, MSG) Then
    '                    End If
    '                    checkOnMQFAscendingDates = False
    '                Else
    '                    MSG = "Error: for uid #" & theMessage.UID & " date of field " & dateids(i) & "(" & Format(date1, "dd.mm.yyyy") & ")" & _
    '                          " is later as date of field " & dateids(j) & "(" & Format(date2, "dd.mm.yyyy") & ") - forecast milestone have to be ascending !"
    '                    theMessage.log = addLog(theMessage.log, MSG)
    '                    theMessage.status = New clsMQFStatus
    '                    theMessage.status.code = constStatusCode_error
    '                    theMessage.processable = theMessage.status.isProcessed And theMessage.processable
    '                    If addFieldStatus(theMessage, i, theMessage.status.code, MSG) Then
    '                    End If
    '                    checkOnMQFAscendingDates = False
    '                End If
    '            End If

    '            '** set i to next j
    '            i = j - 1
    '        End If
    '    Next i





    'End Function


    '****************************************************************************************************
    ' processXLSMQF
    '
    ' OUTDATED !!

    'Function processXLSMQF(ByRef MQFWorkbook As Excel.Workbook, ByRef MQFObject As MessageQueue) As Boolean
    '    Dim aMQFRowEntry As MQMessage
    '    Dim aMapping As New Dictionary(Of Object, Object)
    '    Dim aMember As MQXSlot
    '    Dim aConfig As XChangeConfiguration
    '    Dim aConfigmember As IXChangeConfigEntry
    '    'Dim aProgressBar As New clsUIProgressBarForm
    '    Dim aDeliverable As New Deliverables.Deliverable
    '    Dim aNewDeliverable As New Deliverables.Deliverable
    '    Dim aValue As Object
    '    Dim aWorkspace As String
    '    Dim aSchedule As New Scheduling.ScheduleEdition
    '    Dim aRefdate As New Date
    '    Dim aNewUID As Long

    '    Dim anUID As Long
    '    Dim aRev As String
    '    Dim i As Long

    '    ' init
    '    'Call aProgressBar.initialize(MQFObject.size, WindowCaption:="processing MQF  ...")
    '    'aProgressBar.showForm()
    '    If Not CurrentSession.IsRunning Then
    '        CurrentSession.StartUp(otAccessRight.ReadUpdateData)
    '    End If
    '    ' save
    '    MQFObject.ProcessedByUsername = CurrentSession.OTdbUser.Username
    '    MQFObject.Processdate = Now

    '    ' step through the RowEntries

    '    For Each aMQFRowEntry In MQFObject.Messages

    '        ' for each Member Check it with the XChangeConfig routines
    '        If aMQFRowEntry.Action = constMQFOperation_CHANGE Then
    '            'Call aMQFRowEntry.RunXChange(MAPPING:=aMapping)
    '            ' get the Result
    '            'Set aMapping = New Dictionary
    '            'For Each aMember In aMQFRowEntry.Members
    '            '    '**
    '            '    Set aConfigmember = MQFObject.XCHANGECONFIG.AttributeByfieldname(aMember.fieldname, tablename:=aMember.OBJECTNAME)
    '            '    If aConfigmember.ISXCHANGED Then
    '            '        If Not aMapping.exists(Key:=aConfigmember.ordinal.value) Then
    '            '            Call aMapping.add(Key:=aConfigmember.ordinal.value, ITEM:=aMember.Value)
    '            '        End If
    '            '    End If

    '            'Next aMember
    '            aMQFRowEntry.ProcessedOn = Now

    '            Call updateRowXlsDoc9(INPUTMAPPING:=aMapping, INPUTXCHANGECONFIG:=MQFObject.XChangeConfig)
    '            '****
    '            '**** ADD REVISION
    '        ElseIf aMQFRowEntry.Action = constMQFOperation_REVISION Then
    '            ' fill the Mapping
    '            aMapping = New Dictionary(Of Object, Object)
    '            'Call aMQFRowEntry.FillMapping(aMapping)
    '            ' get UID
    '            aConfigmember = MQFObject.XChangeConfig.GetEntryByXID(XID:="uid")
    '            If Not aConfigmember Is Nothing Then
    '                If aConfigmember.IsLoaded Or aConfigmember.IsCreated Then
    '                    If aMapping.ContainsKey(key:=aConfigmember.Ordinal.Value) Then
    '                        anUID = aMapping.Item(key:=aConfigmember.Ordinal.Value)
    '                        aDeliverable = Deliverables.Deliverable.Retrieve(uid:=anUID)
    '                        If aDeliverable Is Nothing Then
    '                            '** revision ?!
    '                            aConfigmember = MQFObject.XChangeConfig.GetEntryByXID(XID:="c16")
    '                            If Not aConfigmember Is Nothing Then
    '                                If aConfigmember.IsLoaded Or aConfigmember.IsCreated Then
    '                                    If aMapping.ContainsKey(key:=aConfigmember.Ordinal.Value) Then
    '                                        aRev = aMapping.Item(key:=aConfigmember.Ordinal.Value)
    '                                    Else
    '                                        aRev = ""
    '                                    End If
    '                                Else
    '                                    aRev = ""
    '                                End If
    '                            Else
    '                                aRev = ""
    '                            End If
    '                            '**
    '                            'aNewDeliverable = aDeliverable.AddRevision(newRevision:=aRev, persist:=True)
    '                            If Not aNewDeliverable Is Nothing Then
    '                                ' substitute UID
    '                                aConfigmember = MQFObject.XChangeConfig.GetEntryByXID(XID:="uid")
    '                                Call aMapping.Remove(key:=aConfigmember.Ordinal.Value)
    '                                Call aMapping.Add(key:=aConfigmember.Ordinal.Value, value:=aNewDeliverable.Uid)
    '                                ' substitute REV
    '                                aConfigmember = MQFObject.XChangeConfig.GetEntryByXID(XID:="c16")
    '                                If Not aConfigmember Is Nothing Then
    '                                    If aConfigmember.IsLoaded Or aConfigmember.IsCreated Then
    '                                        If aMapping.ContainsKey(key:=aConfigmember.Ordinal.Value) Then
    '                                            Call aMapping.Remove(key:=aConfigmember.Ordinal.Value)
    '                                        End If
    '                                        Call aMapping.Add(key:=aConfigmember.Ordinal.Value, value:=aNewDeliverable.Revision)
    '                                    End If
    '                                End If
    '                                ' substitute TYPEID or ADD
    '                                aConfigmember = MQFObject.XChangeConfig.GetEntryByXID(XID:="SC14")
    '                                If aConfigmember Is Nothing Then
    '                                    If MQFObject.XChangeConfig.AddEntryByXID(Xid:="SC14") Then
    '                                        aConfigmember = MQFObject.XChangeConfig.GetEntryByXID(XID:="SC14")
    '                                    End If
    '                                End If
    '                                If aConfigmember.IsLoaded Or aConfigmember.IsCreated Then
    '                                    If aMapping.ContainsKey(key:=aConfigmember.Ordinal.Value) Then
    '                                        Call aMapping.Remove(key:=aConfigmember.Ordinal.Value)
    '                                    End If
    '                                    Dim aTrack As Deliverables.Track
    '                                    aTrack = aNewDeliverable.GetTrack
    '                                    If Not aTrack Is Nothing Then
    '                                        Call aMapping.Add(key:=aConfigmember.Ordinal.Value, value:=aTrack.Scheduletype)
    '                                    End If
    '                                    'Call aMapping.Add(key:=aConfigmember.ordinal.value, c:=aNewDeliverable.getTrack.SCHEDULETYPE)
    '                                End If

    '                                '*** runxchange
    '                                'Call aMQFRowEntry.RunXChange(MAPPING:=aMapping)
    '                                aMQFRowEntry.ProcessedOn = Now
    '                                'how to save new uid ?!
    '                                'Call updateRowXlsDoc9(INPUTMAPPING:=aMapping, INPUTXCHANGECONFIG:=MQFObject.XCHANGECONFIG)
    '                            Else
    '                                Call CoreMessageHandler(procedure:="MQF.processXLSMQF", message:="AddRevision failed", _
    '                                                      argument:=aDeliverable.Uid)
    '                            End If
    '                        Else
    '                            Call CoreMessageHandler(procedure:="MQF.processXLSMQF", message:="uid not in mapping", _
    '                                                  argument:=anUID)
    '                        End If
    '                    Else
    '                        Call CoreMessageHandler(procedure:="MQF.processXLSMQF", message:="load of Deliverable failed", _
    '                                              argument:=aConfigmember.Ordinal.Value)
    '                    End If
    '                Else
    '                    Call CoreMessageHandler(procedure:="MQF.processXLSMQF", message:="uid id not in configuration", _
    '                                          argument:="uid")
    '                End If
    '            Else
    '                Call CoreMessageHandler(procedure:="MQF.processXLSMQF", message:="uid id not in configuration", _
    '                                      argument:="uid")
    '            End If

    '            '****
    '            '**** ADD-AFTER
    '            '****
    '        ElseIf aMQFRowEntry.Action = constMQFOperation_ADDAFTER Then
    '            ' fill the Mapping
    '            aMapping = New Dictionary(Of Object, Object)
    '            'Call aMQFRowEntry.FillMapping(aMapping)

    '            ' create -> deliverable type should be in here
    '            aDeliverable = Deliverables.Deliverable.Create()
    '            ' aDeliverable = aDeliverable.CreateFirstRevision() not necessary anymore
    '            If aDeliverable.IsCreated Then
    '                aNewUID = aDeliverable.Uid
    '                ' substitute UID
    '                aConfigmember = MQFObject.XChangeConfig.GetEntryByXID(XID:="uid")
    '                If Not aConfigmember Is Nothing Then
    '                    If aConfigmember.IsLoaded Or aConfigmember.IsCreated Then
    '                        If aMapping.ContainsKey(key:=aConfigmember.Ordinal.Value) Then
    '                            anUID = aMapping.Item(key:=aConfigmember.Ordinal.Value)
    '                            Call aMapping.Remove(key:=aConfigmember.Ordinal.Value)
    '                        Else
    '                            anUID = -1
    '                        End If
    '                    Else
    '                        If MQFObject.XChangeConfig.AddEntryByXID(Xid:="uid") Then
    '                            aConfigmember = MQFObject.XChangeConfig.GetEntryByXID(XID:="uid")
    '                        End If
    '                    End If
    '                Else
    '                    If MQFObject.XChangeConfig.AddEntryByXID(Xid:="uid") Then
    '                        aConfigmember = MQFObject.XChangeConfig.GetEntryByXID(XID:="uid")
    '                    End If
    '                End If

    '                Call aMapping.Add(key:=aConfigmember.Ordinal.Value, value:=aNewUID)


    '                '*** runxchange
    '                'Call aMQFRowEntry.RunXChange(MAPPING:=aMapping)
    '                aMQFRowEntry.ProcessedOn = Now
    '                '*** TODO : ADD TO OUTLINE
    '                System.Diagnostics.Debug.Write("new deliverable added: " & aNewUID & " to be added after uid #" & anUID)
    '            Else
    '                Call CoreMessageHandler(procedure:="MQF.processXLSMQF", message:="new deliverable couldn't be created", _
    '                                      argument:=anUID, break:=False, messagetype:=otCoreMessageType.ApplicationError)
    '            End If


    '            '******
    '            '****** freeze
    '        ElseIf aMQFRowEntry.Action = constMQFOperation_FREEZE Then
    '            aMapping = New Dictionary(Of Object, Object)
    '            'Call aMQFRowEntry.FillMapping(aMapping)
    '            ' get UID
    '            aConfigmember = MQFObject.XChangeConfig.GetEntryByXID(XID:="uid")
    '            If Not aConfigmember Is Nothing Then
    '                If aConfigmember.IsLoaded Or aConfigmember.IsCreated Then
    '                    If aMapping.ContainsKey(key:=aConfigmember.Ordinal.Value) Then
    '                        anUID = aMapping.Item(key:=aConfigmember.Ordinal.Value)
    '                        aDeliverable = Deliverables.Deliverable.Retrieve(uid:=anUID)
    '                        If aDeliverable IsNot Nothing Then
    '                            If Not aDeliverable.IsDeleted Then
    '                                '*** set the workspaceID
    '                                ' REWORK: aValue = MQFObject.XCHANGECONFIG.GetMemberValue(ID:="WS", mapping:=aMapping)
    '                                If IsNull(aValue) Then
    '                                    aWorkspace = CurrentSession.CurrentWorkspaceID
    '                                Else
    '                                    aWorkspace = CStr(aValue)
    '                                End If
    '                                '***get the schedule
    '                                aSchedule = aDeliverable.GetWorkScheduleEdition(workspaceID:=aWorkspace)
    '                                If Not aSchedule Is Nothing Then
    '                                    If aSchedule.IsLoaded Then
    '                                        '*** reference date
    '                                        aRefdate = MQFObject.RequestedOn
    '                                        If aRefdate = constNullDate Then
    '                                            aRefdate = Now
    '                                        End If
    '                                        '*** draw baseline
    '                                        Call aSchedule.DrawBaseline(REFDATE:=aRefdate)
    '                                    End If
    '                                End If
    '                            End If

    '                        End If
    '                    End If
    '                End If
    '            End If
    '            '****
    '            '**** Delete Deliverable
    '        ElseIf aMQFRowEntry.Action = constMQFOperation_DELETE Then
    '            ' fill the Mapping
    '            aMapping = New Dictionary(Of Object, Object)
    '            'Call aMQFRowEntry.FillMapping(aMapping)
    '            ' get UID
    '            aConfigmember = MQFObject.XChangeConfig.GetEntryByXID(XID:="uid")
    '            If Not aConfigmember Is Nothing Then
    '                If aConfigmember.IsLoaded Or aConfigmember.IsCreated Then
    '                    If aMapping.ContainsKey(key:=aConfigmember.Ordinal.Value) Then
    '                        anUID = aMapping.Item(key:=aConfigmember.Ordinal.Value)
    '                        aDeliverable = Deliverables.Deliverable.Retrieve(uid:=anUID)
    '                        If aDeliverable IsNot Nothing Then
    '                            aDeliverable.Delete()

    '                        End If
    '                    End If
    '                End If
    '            End If
    '        End If    ' commands

    '        i = i + 1
    '        'Call aProgressBar.progress(1, Statustext:="updating OnTrack Database message #" & i)
    '    Next

    '    'aProgressBar.showStatus("saving MQF in OnTrack ... ")
    '    MQFObject.Persist()
    '    'aProgressBar.showStatus("saved MQF in OnTrack ... ")

    '    'aProgressBar.closeForm()

    '    'return
    '    processXLSMQF = True

    'End Function

    ''' <summary>
    ''' Postprocess the Excel after MQF Preprocess / Process run - write back the results
    ''' </summary>
    ''' <param name="MQFWorkbook"></param>
    ''' <param name="MQFObject"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function UpdateXLSMQF(ByRef MQFWorkbook As Excel.Workbook, ByRef [messagequeue] As MessageQueue, _
                               Optional ByRef workerthread As BackgroundWorker = Nothing) As Boolean
        Dim aValue As Object
        Dim headerstartrow As Integer
        Dim headerids_name As String
        Dim headerids As Range
        Dim Prefix As String
        Dim DescTable As Range
        Dim datawsname As String
        Dim dataws As Excel.Worksheet
        Dim n As Long
        Dim startrow As Integer
        Dim foundflag As Boolean
        Dim row As Range
        Dim aStatus As Commons.StatusItem
        Dim mqfDBRange As Range
        Dim aMessage As New MQMessage
        Dim MQFWorksheetname As String
        Dim maximum, progress As Integer

        Try
            Globals.ThisAddIn.Application.EnableEvents = False
            Globals.ThisAddIn.Application.ScreenUpdating = False
            ' get Startrow
            aValue = GetXlsParameterByName("otdb_parameter_mqf_template_headerstartrow", workbook:=MQFWorkbook)
            If Not IsNumeric(aValue) Then
                UpdateXLSMQF = False
                'MQFWorkbook.Close (False)
                Exit Function
            End If
            headerstartrow = CInt(aValue)
            headerids_name = GetXlsParameterByName("otdb_parameter_mqf_headerid_name", workbook:=MQFWorkbook)
            headerids = GetXlsParameterRangeByName(headerids_name, workbook:=MQFWorkbook)
            ' error
            If headerids Is Nothing Then
                Call CoreMessageHandler(showmsgbox:=True, _
                                      message:="The parameter 'otdb_parameter_mqf_headerid_name':" & headerids_name & " is not showing a valid range !" _
                                    , procedure:="modXLSMessageQueueFile.postProcessXLSMQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)

                UpdateXLSMQF = False
                Exit Function
            End If

            Prefix = GetXlsParameterByName("otdb_parameter_mqf_dbdesc_prefix", workbook:=MQFWorkbook)
            MQFWorksheetname = GetXlsParameterByName("otdb_parameter_mqf_templatedata", workbook:=MQFWorkbook, silent:=True)
            If MQFWorksheetname <> "" Then
                If MQFWorkbook.Sheets(MQFWorksheetname).ProtectContents Then MQFWorkbook.Sheets(MQFWorksheetname).Unprotect(constPasswordTemplate)
            End If


            DescTable = GetXlsParameterRangeByName("otdb_parameter_mqf_structure_db_description_table", workbook:=MQFWorkbook)
            ' error
            If DescTable Is Nothing Then
                Call CoreMessageHandler(showmsgbox:=True, _
                                     message:="The parameter 'otdb_parameter_mqf_structure_db_description_table' is not showing a valid range !" _
                                   , procedure:="modXLSMessageQueueFile.postProcessXLSMQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)

                UpdateXLSMQF = False
                Exit Function
            End If

            ''' step out in edit mode
            ''' 
            ''' step out in edit mode
            ''' 
            If modXLSHelper.IsEditing() Then
                Globals.ThisAddIn.Application.EnableEvents = True
                Globals.ThisAddIn.Application.ScreenUpdating = True
                Globals.ThisAddIn.Application.SendKeys("{Enter}")
                If modXLSHelper.IsEditing() Then
                    Call CoreMessageHandler(message:="the cell in [" & Globals.ThisAddIn.Application.ActiveWorkbook.Name & "!" & _
                                            CType(Globals.ThisAddIn.Application.ActiveSheet, Excel.Worksheet).Name & "]" & _
                                             CType(Globals.ThisAddIn.Application.ActiveCell, Excel.Range).Address.ToString & " is being edited " & vbLf & " - please leave cell before starting operation", _
                                             procedure:="modXLSMessageQueueFile.postProcessXLSMQF", showmsgbox:=True, messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If
                Globals.ThisAddIn.Application.EnableEvents = False
                Globals.ThisAddIn.Application.ScreenUpdating = False
            End If

            ' get startrow
            startrow = GetXlsParameterByName("otdb_parameter_mqf_template_datastartrow", workbook:=MQFWorkbook)
            datawsname = GetXlsParameterByName("otdb_parameter_mqf_templatedata", workbook:=MQFWorkbook, found:=foundflag, silent:=False)
            If Not foundflag Then
                dataws = headerids.Worksheet
            Else
                dataws = MQFWorkbook.Sheets(datawsname)
            End If
            ' the full sheet
            '' go in the uid column from the bottom upwards
            Dim maxrow As Integer = dataws.Cells(dataws.Rows.Count, messagequeue.UIDOrdinal).End(Excel.XlDirection.xlUp).row
            '' go from the headid row right-most to the left
            Dim MaxCol As Integer = dataws.Cells(headerids.Row, dataws.Columns.Count).End(Excel.XlDirection.xlToLeft).Column
            ' set it
            mqfDBRange = dataws.Range(dataws.Cells(startrow, 1), dataws.Cells(maxrow, MaxCol))
            ' autofilter off
            dataws.AutoFilterMode = False

            ' init
            If workerthread IsNot Nothing Then
                workerthread.ReportProgress(0, "")
                maximum = maxrow - startrow + 1
            End If

            '**** go through message queue
            '****
            n = 1
            For Each row In mqfDBRange.Rows
                ' only if we are really in the same order
                aMessage = [messagequeue].Messages.Item(n)
                '** progress
                If workerthread IsNot Nothing Then
                    progress += 1
                    workerthread.ReportProgress((progress / maximum) * 100, "updating row #" & row.Row)
                End If

                If Not IsError(row.Cells(1, [messagequeue].ActionOrdinal).Value) Then
                    If LCase(aMessage.Action) = LCase(row.Cells(1, [messagequeue].ActionOrdinal).Value) Then  'And _
                        'aMQFRowEntry. = row.Cells(1, theMQFFormat.UIDCol).Value Then

                        Globals.ThisAddIn.Application.StatusBar = " postprocessing MQF " & MQFWorkbook.Name & " updating messages and stati for row#" & row.Row

                        ' timestamp
                        If messagequeue.ProcessDateordinal IsNot Nothing Then
                            If messagequeue.Processdate IsNot Nothing Then
                                row.Cells(1, [messagequeue].ProcessDateordinal) = Converter.DateTime2LocaleDateTimeString([messagequeue].Processdate)
                            Else
                                row.Cells(1, [messagequeue].ProcessDateordinal) = Converter.DateTime2LocaleDateTimeString(DateTime.Now)
                            End If

                        End If

                        aStatus = aMessage.ObjectMessageLog.GetHighesStatusItem
                        If aStatus IsNot Nothing Then
                            ' status code
                            row.Cells(1, [messagequeue].ProcessStatusordinal) = aStatus.Code
                            ' get it
                            If aStatus.FormatBGColor IsNot Nothing Then row.Cells(1, [messagequeue].ProcessStatusordinal).Interior.Color = aStatus.FormatBGColor
                        End If

                        ' message log
                        Dim messages As String = aMessage.ObjectMessageLog.MessageText
                        row.Cells(1, [messagequeue].ProcessLogordinal) = messages
                        row.Cells(1, [messagequeue].ProcessLogordinal).Font.size = 6
                        row.Cells(1, [messagequeue].ProcessLogordinal).Font.Bold = False
                        row.Cells(1, [messagequeue].ProcessLogordinal).WrapText = True

                    Else
                        System.Diagnostics.Debug.WriteLine("Order of postprocess is not matching: ")
                        UpdateXLSMQF = False
                    End If
                End If
                ' increase
                n = n + 1
            Next row

            aValue = SetXlsParameterValueByName("hermes_mqf_processedBy", [messagequeue].ProcessedByUsername, workbook:=MQFWorkbook)
            aValue = SetXlsParameterValueByName("hermes_mqf_processedOn", Converter.Date2LocaleShortDateString([messagequeue].Processdate), workbook:=MQFWorkbook)
            aValue = SetXlsParameterValueByName("hermes_mqf_status", [messagequeue].ProcessStatusCode, workbook:=MQFWorkbook)

            SetXlsParameterValueByName("hermes_mqf_requestedby", [messagequeue].RequestedBy, workbook:=MQFWorkbook, silent:=True)
            SetXlsParameterValueByName("hermes_mqf_requested_on", Converter.Date2LocaleShortDateString([messagequeue].RequestedOn), workbook:=MQFWorkbook, silent:=True)
            SetXlsParameterValueByName("hermes_mqf_requestedby_department", [messagequeue].RequestedByOU, workbook:=MQFWorkbook)

            SetXlsParameterValueByName("hermes_mqf_createdby", [messagequeue].Creator, workbook:=MQFWorkbook, silent:=True)
            SetXlsParameterValueByName("hermes_mqf_createdon", Converter.Date2LocaleShortDateString([messagequeue].CreationDate), workbook:=MQFWorkbook, silent:=True)
            SetXlsParameterValueByName("hermes_mqf_createdby_department", [messagequeue].CreatingOU, workbook:=MQFWorkbook)

            SetXlsParameterValueByName("hermes_mqf_title", [messagequeue].Title, workbook:=MQFWorkbook, silent:=True)
            SetXlsParameterValueByName("hermes_mqf_subject", [messagequeue].Description, workbook:=MQFWorkbook, silent:=True)
            SetXlsParameterValueByName("hermes_mqf_plan_revision", [messagequeue].Planrevision, workbook:=MQFWorkbook, silent:=True)

            SetXlsParameterValueByName("hermes_mqf_approvedBy", [messagequeue].ApprovedBy, workbook:=MQFWorkbook, silent:=True)
            SetXlsParameterValueByName("hermes_mqf_comment", [messagequeue].ProcessComment, workbook:=MQFWorkbook, silent:=True)
            ' autofilter
            'dataws.Range(dataws.Cells(headerstartrow + 1, 1), _
            '             dataws.Cells(headerstartrow + 1, MaxCol)).AutoFilter()

            ' protect again
            MQFWorkbook.Sheets(MQFWorksheetname).Protect(password:=constPasswordTemplate, _
                                                         DrawingObjects:=False, Contents:=True, Scenarios:=False, _
                                                         AllowFormattingCells:=True, AllowFormattingColumns:=True, _
                                                         AllowFormattingRows:=True, AllowInsertingColumns:=True, AllowInsertingRows:=True, _
                                                         AllowDeletingColumns:=True, AllowDeletingRows:=True, _
                                                         AllowFiltering:=True, AllowUsingPivotTables:=True, AllowSorting:=True)

            ' save it
            Globals.ThisAddIn.Application.StatusBar = " MQF " & MQFWorkbook.Name & " was updated after processing"
            If workerthread IsNot Nothing Then workerthread.ReportProgress(100, "saving excel workbook")
            MQFWorkbook.Save()

            Globals.ThisAddIn.Application.EnableEvents = True
            Globals.ThisAddIn.Application.ScreenUpdating = True
            'return
            Return True

        Catch ex As Exception
            Globals.ThisAddIn.Application.EnableEvents = True
            Globals.ThisAddIn.Application.ScreenUpdating = True
            CoreMessageHandler(showmsgbox:=True, exception:=ex, procedure:="modXLSMessageQueueFile.PostProcessXLSMQF")
            Return False
        End Try
    End Function


    ''' <summary>
    ''' Subroutine to Locate and Open a  Message Queue in the Globals.ThisAddin.Application
    '''  runs through some tests to ensure that it is really an MQF
    '''  returns the Workbook
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function LocateAndOpenMQF() As Excel.Workbook
        Dim FileNamePattern, FileNamePattern2, currWorkbookName, MQFWorkbookName, MQFSheetName As String
        Dim filenames As Object
        Dim flag, Doc9sheet_flag As Boolean
        Dim MQFWorkbook As Excel.Workbook, wb As Excel.Workbook
        Dim ws As Excel.Worksheet
        Dim Value As Object

        '** handle no Connection
        If Not CurrentSession.IsRunning Then
            Call CoreMessageHandler(showmsgbox:=True, break:=False, _
                                   message:="No connection to OnTrackDatabase available. Check configuration or availabilty of database with your administrator")
            Return Nothing
        End If

        ' get Doc9 File Name
        FileNamePattern = GetDBParameter("otdb_parameter_mqf_filenamepattern", silent:=True)
        'FileNamePattern2 = getXLSParameterByName("parameter_Doc9filename_search_2")
        'For Each wb In Globals.ThisAddin.Application.Globals.ThisAddin.Application.Workbooks
        ' If (InStr(wb.Name, FileNamePattern) > 0) Or (InStr(wb.Name, FileNamePattern2) > 0) Then
        '     MsgBox "It seems that OnTrack as '" & wb.Name & _
        '     "' is alread opened - please close it and run the procedure again !", Buttons:=vbCritical, Title:="OnTrack Tooling Error"
        '     Exit Sub
        ' End If

        'Next

        'Open Dialog for Doc9 Find
        Value = GetDBParameter("parameter_startfoldernode", silent:=True)
        If Value <> "" And FileIO.FileSystem.FileExists(Value) Then

            If Mid(Value, Len(Value), 1) <> "\" Then Value = Value & "\"

            If Mid(Value, 2, 1) = ":" Then
                ChDrive(Mid(Value, 1, 2))
            End If
            Value = Value & GetDBParameter("otdb_parameter_mqf_inputqueue")
            If FileIO.FileSystem.FileExists(Value) Then
                ChDir(Value)
            End If
        End If

        ' filenames = Globals.ThisAddIn.Application.GetOpenFilename(FileNamePattern, _
        '                                        3, "Find OnTrack MQF to Read-In into Doc9 Database", , False)
        'locate

        filenames = Globals.ThisAddIn.Application.GetOpenFilename(FileNamePattern, _
                                                3, "Select a Message Queue File", "Select", False)

        If Not TypeOf filenames Is String Or String.IsNullOrEmpty(filenames) Then
            Return Nothing
            Exit Function
        End If
        '
        currWorkbookName = Globals.ThisAddIn.Application.ActiveWorkbook.Name
        Dim allWorkbooks As New Collection
        For Each aWb As Excel.Workbook In Globals.ThisAddIn.Application.Workbooks
            Call allWorkbooks.Add(aWb)
        Next


        'Open the OnTrack not read-only-any more
        MQFWorkbook = Globals.ThisAddIn.Application.Workbooks.Open(Filename:=filenames, ReadOnly:=False)
        ' Activate SMBDoc9 again
        If allWorkbooks.Contains(currWorkbookName) Then
            Globals.ThisAddIn.Application.Workbooks(currWorkbookName).Activate()
        End If
        ' close it again if check fails
        If Not checkWorkbookIfMQF(MQFWorkbook) Then
            MQFWorkbook.Close()
            Return Nothing
        End If


        Return MQFWorkbook
    End Function

    '**************
    '************** checkOnMQF : checks if the Workbook is an MQF
    '**************
    Public Function checkWorkbookIfMQF(ByRef MQFWORKBOOK As Excel.Workbook, _
                                Optional ByVal SILENT As Boolean = True) As Boolean
        Dim currWorkbookName As String
        Dim MQFSheetname As String
        Dim ws As Excel.Worksheet

        'Open and check the Doc9
        currWorkbookName = Globals.ThisAddIn.Application.ActiveWorkbook.Name

        MQFSheetname = GetXlsParameterByName("otdb_parameter_mqf_templatedata", MQFWORKBOOK, silent:=True)
        If MQFSheetname = "" Then
            MQFSheetname = GetXlsParameterByName("otdb_parameter_mqf_templatedata", silent:=True)
        End If
        If MQFSheetname = "" Then
            MQFSheetname = GetDBParameter("otdb_parameter_mqf_templatedata", silent:=True)
        End If
        'Check if Worksheet there
        ' Check if Parameters Sheet is still there

        If SheetExistsinWorkbook(MQFWORKBOOK, MQFSheetname) Then
            ws = MQFWORKBOOK.Sheets(MQFSheetname)
        Else
            Call CoreMessageHandler(procedure:="checkOnMQF", message:="Workbook '" & MQFWORKBOOK.Name & "' is not a valid Message Queue File", _
                                    argument:=MQFWORKBOOK.Name, showmsgbox:=Not SILENT, messagetype:=otCoreMessageType.ApplicationInfo)

            ' Error
            If SILENT = False Then
                Call CoreMessageHandler(showmsgbox:=True, _
                                 message:="TThe Worksheet '" & MQFSheetname & " ' is not found in the Workbook. Is this a valid Doc9 Message Queue Workbook ? " _
                               , procedure:="modXLSMessageQueueFile.checkWorkbookIfMQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)
            End If

            Return False
        End If

        Return True
    End Function


End Module
