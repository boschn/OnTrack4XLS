
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


Module modMQF

    ' ***************************************************************************************************
    '   Module for message Queue File functions
    '
    '   Author: B.Schneider
    '   created: 2012-07-13
    '
    '   change-log:
    ' ***************************************************************************************************

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
        Public FieldIDs As Object    'Array of Names
        Public FieldROIDs As Object    'Array of Read-Only Names
        Public copyOldValues As Boolean


    End Structure

    '****
    '**** MessageQueue File Database Description
    '****
    Public Structure MQFDBDesc

        Public ID As String
        Public TITLE As String
        Public ColumnNo As Integer
        Public READ_ONLY As Boolean
        Public Doc9Desc As xlsDBDesc
        Public mqfversion As String
        Public OBJECTNAME As String
    End Structure

    '****
    '**** MessageFormat of a MQF Message
    '****

    Public Structure MessageFormat

        Public desc() As MQFDBDesc
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

        Public TITLE As String
        Public Requestfor As String

        Public workspaceID As String
        Public XCHANGECONFIG As clsOTDBXChangeConfig

    End Structure
    '***
    '*** Message Definition
    '***

    Public Structure MQFMessage
        Public action As String
        Public UID As Long
        Public fieldvalues As Object    ' Array of Values

        Public status As clsMQFStatus  ' Status of the Message
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

    Private Function getMQFConfigFields(fields As String) As String()

        Dim allfields As Object
        Dim Value As Object
        Dim multi() As String
        Dim i, j As Integer
        Dim delimiter As String
        Dim theFields() As String
        Dim n As Integer


        ' check all Fields
        allfields = getDBDescIDs()
        If Not IsArrayInitialized(allfields) Then
            Call CoreMessageHandler(showmsgbox:=True, message:="Abort: Header Field Description / name headerids is not found ", _
                                   subname:="modMQF.getMQFConfigFields", messagetype:=otCoreMessageType.ApplicationError, break:=False)

            Exit Function
        End If

        delimiter = ","
        j = 0

        multi = SplitMultiDelims(text:=fields, DelimChars:=delimiter)
        If IsArrayInitialized(multi) Then
            For i = 1 To UBound(multi)
                If InStr(multi(i), delimiter) = 0 Then
                    Value = Trim(multi(i))
                    If Value <> "" Then
                        ' wildchar
                        If InStr(Value, "*") > 0 Or InStr(Value, "?") > 0 Or InStr(Value, "[") > 0 Or _
                           InStr(Value, "]") > 0 _
                           Then
                            For n = 0 To UBound(allfields)
                                If allfields(n) Like Value Then
                                    ReDim Preserve theFields(j)
                                    theFields(j) = Trim(allfields(n))
                                    j = j + 1
                                End If
                            Next n
                            ' plain
                        Else
                            ReDim Preserve theFields(j)
                            theFields(j) = Trim(multi(i))
                            j = j + 1
                        End If
                    End If
                End If
            Next i
        End If

        ' return
        If IsArrayInitialized(theFields) Then
            getMQFConfigFields = theFields
        End If

    End Function


    '**********
    '**********  getMQFConfigfields : Split fields-ID in the fields()
    '**********

    Private Function getMQFConfigROFields(fields As String) As String()

        Dim allfields As Object
        Dim Value As Object
        Dim multi() As String
        Dim i, j As Integer
        Dim delimiter As String
        Dim theFields() As String
        Dim n As Integer


        ' check all Fields
        allfields = getDBDescIDs()
        If Not IsArrayInitialized(allfields) Then
            Call CoreMessageHandler(showmsgbox:=True, message:="Abort: Header Field Description / name headerids is not found ", _
                                   subname:="modMQF.getMQFConfigROFields", messagetype:=otCoreMessageType.ApplicationError, break:=False)

        End If

        delimiter = constDelimeter
        j = 0

        multi = SplitMultiDelims(text:=fields, DelimChars:=delimiter)
        If IsArrayInitialized(multi) Then
            For i = 1 To UBound(multi)
                If InStr(multi(i), delimiter) = 0 Then
                    Value = Trim(multi(i))
                    If Value <> "" Then
                        ' wildchar
                        If InStr(Value, "*") > 0 Or InStr(Value, "?") > 0 Or InStr(Value, "[") > 0 Or _
                           InStr(Value, "]") > 0 _
                           Then
                            For n = 0 To UBound(allfields)
                                If allfields(n) Like Value Then
                                    ReDim Preserve theFields(j)
                                    theFields(j) = Trim(allfields(n))
                                    j = j + 1
                                End If
                            Next n
                            ' plain
                        Else
                            ReDim Preserve theFields(j)
                            theFields(j) = Trim(multi(i))
                            j = j + 1
                        End If
                    End If
                End If
            Next i
        End If

        ' return
        If IsArrayInitialized(theFields) Then
            getMQFConfigROFields = theFields
        End If

    End Function

    '**********
    '**********  getMQFConfigTable : returns the MQFConfigTable as 2Dim object
    '**********

    Function getMQFConfigTable(Ctable() As MQFConfig, Optional aName As String = "") As Boolean
        Dim config_table As Range
        Dim row As Range
        'Dim ctable() As object



        Dim Value As Object
        Dim i As Integer
        Dim aCartypes As New clsCartypes
        Dim found As Boolean

        config_table = GetXlsParameterRangeByName("parameter_mqf_config_table")

        found = False

        i = -1
        For Each row In config_table.Rows
            ' found or all
            If Trim(UCase(row.Cells(1, constMQFConfigName + 1))) = aName Or aName = "" Then
                found = True
                i = i + 1
                ReDim Preserve Ctable(i)
                ' Name
                Ctable(i).Name = Trim(UCase(row.Cells(1, constMQFConfigName + 1)))
                ' oldValues
                If Trim(row.Cells(1, constMQFConfigCopyOldValues + 1)) <> "" Then
                    Ctable(i).copyOldValues = True
                Else
                    Ctable(i).copyOldValues = False
                End If
                Ctable(i).FieldIDs = getMQFConfigFields(Trim(row.Cells(1, constMQFConfigFields + 1)))
                Ctable(i).FieldROIDs = getMQFConfigROFields(Trim(row.Cells(1, constMQFConfigROFields + 1)))
                ' Description
                Ctable(i).desc = Trim(row.Cells(1, constMQFConfigDesc + 1))
            End If
        Next row

        getMQFConfigTable = found

    End Function

    '**********
    '**********  getMQFConfigTable : returns the MQFConfigTable as 2Dim object
    '**********

    Function getMQFField(aName As String, Optional ColumnType As Integer = constMQFCT_ALL) As Range
        Dim DTable As Range
        Dim row As Range
        'Dim ctable() As object
        Dim Value As Object
        Dim i As Integer
        Dim found As Boolean
        Dim result As Range
        Dim col As Integer


        DTable = GetXlsParameterRangeByName("parameter_mqf_structure_db_description_table")

        found = False
        i = -1
        ' search
        For Each row In DTable.Rows
            ' found or all
            If Trim(UCase(row.Cells(1, constMQFFieldheader + 1))) = UCase(aName) Or aName = "" Then
                Value = row.Cells(1, constMQFFieldColumn + 1).Value
                If IsNumeric(Value) Then
                    col = CDec(Value)
                Else
                    col = 0
                End If
                ' search criteria
                If ColumnType = constMQFCT_ALL Or (ColumnType = constMQFCT_Pre And col < 0) _
                   Or (ColumnType = constMQFCT_Post And col > 0) Then
                    found = True
                    i = i + 1
                    If result Is Nothing Then
                        result = row
                    Else
                        result = Globals.ThisAddIn.Application.Union(result, row)
                    End If
                End If
            End If
        Next row

        If found Then
            getMQFField = result
        Else
            getMQFField = Nothing
        End If
    End Function

    '**********
    '**********  getminMQField : returns the smalles column-no
    '**********

    Function getminMQFField(ColumnType As Integer) As Integer
        Dim DTable As Range
        Dim row As Range
        'Dim ctable() As object
        Dim Value As Object
        Dim i As Integer
        Dim found As Boolean
        Dim result As Integer
        Dim col As Integer


        DTable = GetXlsParameterRangeByName("parameter_mqf_structure_db_description_table")

        found = False
        result = 0
        ' search
        For Each row In DTable.Rows
            ' found or all
            Value = row.Cells(1, constMQFFieldColumn + 1).Value
            If IsNumeric(Value) Then
                col = CDec(Value)
            Else
                col = 0
            End If
            ' search criteria
            If ColumnType = constMQFCT_ALL Or (ColumnType = constMQFCT_Pre And col < 0) _
               Or (ColumnType = constMQFCT_Post And col > 0) Then
                If col < result Then
                    found = True
                    result = col
                End If
            End If
        Next row

        If found Then
            getminMQFField = result
        Else
            getminMQFField = 0
        End If
    End Function

    '**********
    '**********  getmaxMQField : returns the smalles column-no
    '**********

    Function getmaxMQFField(ColumnType As Integer) As Integer
        Dim DTable As Range
        Dim row As Range
        'Dim ctable() As object
        Dim Value As Object
        Dim i As Integer
        Dim found As Boolean
        Dim result As Integer
        Dim col As Integer


        DTable = GetXlsParameterRangeByName("parameter_mqf_structure_db_description_table")

        found = False
        result = 0
        ' search
        For Each row In DTable.Rows
            ' found or all
            Value = row.Cells(1, constMQFFieldColumn + 1).Value
            If IsNumeric(Value) Then
                col = CDec(Value)
            Else
                col = 0
            End If
            ' search criteria
            If ColumnType = constMQFCT_ALL Or (ColumnType = constMQFCT_Pre And col < 0) _
               Or (ColumnType = constMQFCT_Post And col > 0) Then
                If col > result Then
                    found = True
                    result = col
                End If
            End If
        Next row

        If found Then
            getmaxMQFField = result
        Else
            getmaxMQFField = 0
        End If
    End Function

    Function checkArray(atestArray As Object) As Boolean

        On Error GoTo error_handler

        If UBound(atestArray) >= 0 Then
            checkArray = True
            Exit Function
        End If

error_handler:
        checkArray = False
        Exit Function
    End Function

    '********** createXChangeConfigFromIDs: creates a config from an array with IDs, ordinal will be the columns
    '**********
    Public Sub createXlsDoc9MQFConfig()
        Dim anObjectName As String
        Dim aNewConfig As New clsOTDBXChangeConfig
        Dim aColl As Collection
        Dim aSchemaDefTable As New ObjectDefinition
        Dim m As Object
        Dim IDs As Object
        Dim cmds As Object
        Dim flag As Boolean
        Dim aFieldDef As New ObjectEntryDefinition
        Dim i As Long

        '*** load the table definition
        'If Not aSchemaDefTable.loadBy(Tablename) Then
        '    Call OTDBErrorHandler(arg1:=Tablename, Tablename:=Tablename, message:=" Could not load SchemaTableDefinition")
        '    Set createXChangeConfigFromIDs = Nothing
        '    Exit Function
        'End If
        'anObjectName = Tablename
        'If aNewConfig.loadBy(ConfigName) Then
        '    aNewConfig.delete
        'End If

        '**
        '** CREATE MQF METHODS
        aNewConfig.Create("mqf_methods")
        Call aNewConfig.AddObjectByName("tblDeliverableTargets")
        Call aNewConfig.AddObjectByName("tblDeliverables")
        IDs = New String() {"uid", "c10", "c6", "t2"}
        cmds = New Integer() {otXChangeCommandType.Read, otXChangeCommandType.Read, otXChangeCommandType.Read, otXChangeCommandType.Update}

        i = 0

        For i = LBound(IDs) To UBound(IDs)
            ' load ID
            If Not modHelperVBA.IsEmpty(IDs(i)) Then
                flag = False
                ' look into objects first
                For Each m In aNewConfig.ObjectsByOrderNo
                    If aFieldDef.LoadByID(IDs(i), m.OBJECTNAME) Then
                        Call aNewConfig.AddAttributeByField(objectentry:=aFieldDef, ordinal:=i, xcmd:=cmds(i))
                        flag = True
                        Exit For
                    End If
                Next m
                ' if not found look elsewhere -> but take all IDs and aliases !
                If flag = False Then
                    aColl = aFieldDef.AllByID(IDs(i))
                    For Each m In aColl
                        aFieldDef = m
                        'Call aNewConfig.addObjectByName(aFieldDef.tablename, xcmd:=xcmd) -> by AttributesField
                        Call aNewConfig.AddAttributeByField(objectentry:=aFieldDef, ordinal:=i, xcmd:=cmds(i))
                    Next m
                End If
            End If
        Next i

        Call aNewConfig.Persist()
        'Set createXlsDoc9MQFConfig = aNewConfig
    End Sub

    '**********
    '**********  createXlsDoc9MQF -> Create a Template MQF
    '**********
    '********** FieldIDs () as ColumnIDs of Fields to use
    '********** ROFields () (Read-Only Fields)
    '********** copyOld as Flag

    '********** OPtional Parameters
    '********** selectedUIDs() List of UIDs as selection
    '********** CloseAfterCreation if true close Workbook
    '********** aMQFWorkbook for return of the created workbook
    '********** copyValuesFromOldMessages -> TRUE if
    '********** MessagesToCopy is filled

    Public Function createXlsDoc9MQF(ByVal Filename As String, _
                                     FieldIDs() As String, _
                                     copyOld As Boolean, _
                                     ROFieldIDs() As String, _
                                     aMessagesToCopy() As MQFMessage, _
                                     theMQFWorkbookToCopy As Excel.Workbook, _
                                     copyValuesFromOldMessages As Boolean, _
                                     Optional selectedUIDs As Object() = Nothing, _
                                     Optional CloseAfterCreation As Boolean = False, _
                                     Optional aMQFWorkbook As Excel.Workbook = Nothing _
                                     ) As Boolean
        Dim FileNamePattern, FileNamePattern2, currWorkbookName, MQFWorkbookName, MQFSheetName As String
        Dim filenames As Object
        Dim flag, Doc9sheet_flag As Boolean
        Dim wb As Excel.Workbook
        Dim MQFWS As Excel.Worksheet
        Dim Value As Object
        Dim template As String
        Dim startfolder As String
        Dim dbDoc9Range As Range
        Dim selection, selectioncol As Range
        Dim msgboxrsl As clsCoreUIMessageBox.ResultType
        Dim MQFWorkbook As Excel.Workbook
        Dim MQFWorksheetName As String
        Dim MQFsheet_flag As Boolean
        Dim headerstartrow As Integer
        Dim dbdesc() As xlsDBDesc
        Dim fieldname As String
        Dim i As Integer
        Dim headerids As Excel.Range
        Dim headerids_name As String
        Dim Prefix As String
        'Dim filename As String
        Dim startdatarow As Integer
        Dim cols() As Integer
        Dim row As Excel.Range
        Dim j As Integer
        Dim preheader As Excel.Range
        Dim postheader As Excel.Range
        Dim preheadercount As Integer
        Dim postheadercount As Integer
        Dim operationcolumn As Integer
        Dim formatoldvalues As Excel.Range
        Dim formatnewvalues As Excel.Range
        Dim MaxCol As Integer
        Dim operation_column As Integer
        Dim uid_column As Integer
        Dim uid_column_new As Integer
        Dim pn_column As Integer
        Dim pn_column_new As Integer
        Dim rocols() As Integer
        Dim postheaderstart As Integer
        Dim rowno As Long
        Dim aMQFDbDesc() As MQFDBDesc
        Dim selectfield As Excel.Range

        Dim aXChangeConfig As clsOTDBXChangeConfig
        Dim otdbvalues() As Object
        Dim n, m As Integer
        Dim templatefiledir As String
        Dim otdbvalue_uid_index As Long
        Dim otdbvalue_uid As Long
        'Dim aProgressBar As New clsUIProgressBarForm


        ' Get Selection
        dbDoc9Range = getdbDoc9Range()
        If dbDoc9Range Is Nothing Then
            createXlsDoc9MQF = False
            Exit Function
        End If
        ' Blend in all Columns
        dbDoc9Range.EntireColumn.Hidden = False
        uid_column = getXLSHeaderIDColumn("uid")


        ' Any values selection
        'selection.Find what:="*", LookIn:=xlValues
        If Not IsMissing(selectedUIDs) And IsArray(selectedUIDs) Then
            System.Diagnostics.Debug.WriteLine("Using preselected UIDs")
        Else

            '*** selection
            ' selection
            Value = getXLSHeaderIDColumn("x1")
            selectioncol = dbDoc9Range.Worksheet.Range(dbDoc9Range.Cells(1, Value), dbDoc9Range.Cells(dbDoc9Range.Rows.count, Value))
            ' selection
            ReDim selectedUIDs(0)
            j = 0
            ' select manually the uids
            For Each selectfield In selectioncol.Cells
                If Not IsEmpty(selectfield.Value) Then
                    If IsNumeric(selectfield.EntireRow.Cells(1, uid_column).Value) Then
                        ReDim Preserve selectedUIDs(j)
                        selectedUIDs(j) = CLng(selectfield.EntireRow.Cells(1, uid_column).Value)
                        j = j + 1
                    End If
                End If
            Next selectfield

            '* nothing selected
            If j = 0 Then
                With New clsCoreUIMessageBox
                    .Message = "ATTENTION !" & vbLf & "No data rows have been selected in the SELECTION Column of the Database. Should ALL rows be written to the Message Queue File ?"
                    .Title = " ARE YOU SURE ?"
                    .type = clsCoreUIMessageBox.MessageType.Question
                    .Show()
                    msgboxrsl = .result
                End With

                If msgboxrsl <> clsCoreUIMessageBox.ResultType.Yes Then
                    Exit Function
                Else
                    'select all uids
                    For Each selectfield In selectioncol.Cells
                        If IsNumeric(selectfield.EntireRow.Cells(1, uid_column).Value) Then
                            ReDim Preserve selectedUIDs(j)
                            selectedUIDs(j) = CLng(selectfield.EntireRow.Cells(1, uid_column).Value)
                            j = j + 1
                        End If
                    Next selectfield
                End If

            End If


            j = 0
        End If    '*


        ' parameters
        startfolder = GetDBParameter("parameter_startfoldernode")
        If startfolder <> "" Then
            If Mid(startfolder, Len(Value), 1) <> "\" Then startfolder = startfolder & "\"
        End If
        templatefiledir = GetDBParameter("parameter_mqf_template_filepath")
        If templatefiledir <> "" Then
            If Mid(templatefiledir, Len(Value), 1) <> "\" Then templatefiledir = templatefiledir & "\"
        End If
        template = GetDBParameter("parameter_mqf_template_file")
        'template = startfolder & template
        formatoldvalues = GetXlsParameterRangeByName("parameter_mqf_format_oldvalues")
        formatnewvalues = GetXlsParameterRangeByName("parameter_mqf_format_newvalues")

        ' where is template
        If FileIO.FileSystem.FileExists(startfolder & templatefiledir & template) Then
            template = startfolder & templatefiledir & template
        ElseIf FileIO.FileSystem.FileExists(startfolder & template) Then
            template = startfolder & template
#If ExcelVersion <> "" Then
        ElseIf FileIO.FileSystem.FileExists(Globals.ThisAddIn.Application.ActiveWorkbook.Path & "\" & template) Then
            template = Globals.ThisAddIn.Application.ActiveWorkbook.Path & "\" & template
#End If
        Else
            Call CoreMessageHandler(showmsgbox:=True, _
                                   message:="Abort: The MQF Template '" & template & " ' is not found in the filesystem. Please contact your Administrator" _
                                   , subname:="modMQF.createXlsDoc9MQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)
            createXlsDoc9MQF = False
            Exit Function
        End If

        '*
        '* open
        '*
        currWorkbookName = Globals.ThisAddIn.Application.ActiveWorkbook.Name

        'Open the MQF Template
        MQFWorkbook = Globals.ThisAddIn.Application.Globals.ThisAddin.Application.Workbooks.Open(Filename:=template, UpdateLinks:=2, ReadOnly:=True)

        MQFWorksheetName = GetXlsParameterByName("parameter_mqf_templatedata", workbook:=MQFWorkbook, silent:=True)
        If MQFWorksheetName = "" Then
            MQFWorksheetName = "Data"
        End If
        'Check if Worksheet there
        ' Check if Data Sheet is still there
        MQFsheet_flag = False
        For Each MQFWS In MQFWorkbook.Sheets
            If MQFWS.Name = MQFWorksheetName Then
                MQFsheet_flag = True
                Exit For
            End If
        Next MQFWS

        ' Error
        If MQFsheet_flag = False Then
            Call CoreMessageHandler(showmsgbox:=True, message:="Abort: The Worksheet '" & MQFWorksheetName & " ' is not found in the Workbook. Is this a valid Doc9 Message Queue File ? " _
                   & MQFWorkbook.Name & "!", subname:="modMQF.createXlsDoc9MQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)

            createXlsDoc9MQF = False
            Exit Function
        End If



        'On Error GoTo handleError:

        '**
        '** Build Header
        '**
        Value = GetXlsParameterByName("parameter_mqf_template_headerstartrow", workbook:=MQFWorkbook)
        If Not IsNumeric(Value) Then
            createXlsDoc9MQF = False
            MQFWorkbook.Close(False)
            Exit Function
        End If
        headerstartrow = CInt(Value)
        headerids_name = GetXlsParameterByName("parameter_doc9_headerid_name")
        headerids = GetXlsParameterRangeByName(headerids_name)
        'parameter_doc9_dbdesc_prefix
        Prefix = GetXlsParameterByName("parameter_doc9_dbdesc_prefix")

        ' error
        If headerids Is Nothing Then
            Call CoreMessageHandler(showmsgbox:=True, message:="The parameter 'parameter_doc9_headerid_name':" & headerids_name & " is not showing a valid range !" _
                  , subname:="modMQF.createXlsDoc9MQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)

            createXlsDoc9MQF = False
            Exit Function
        End If

        Globals.ThisAddIn.Application.ScreenUpdating = False

        '********
        '******** headers
        '********

        '*** parameters
        preheadercount = Math.Abs(getminMQFField(constMQFCT_Pre))
        preheader = getMQFField("", constMQFCT_Pre)
        postheadercount = getmaxMQFField(constMQFCT_Post)
        postheader = getMQFField("", constMQFCT_Post)

        ' if preheader is found
        If Not preheader Is Nothing Then

            '*** preheaders
            i = 0
            For Each row In preheader
                ' ID
                MQFWS.Cells(headerstartrow, i + 1).Value = row.Cells(1, constMQFFieldheader + 1).Value
                row.Cells(1, constMQFFieldheader + 1).Copy()
                MQFWS.Cells(headerstartrow, i + 1).PasteSpecial(Excel.XlPasteType.xlPasteFormats)
                ' Title
                MQFWS.Cells(headerstartrow + 1, i + 1) = row.Cells(1, constMQFfieldname + 1).Value
                row.Cells(1, constMQFfieldname + 1).Copy()
                MQFWS.Cells(headerstartrow + 1, i + 1).PasteSpecial(Excel.XlPasteType.xlPasteFormats)
                MQFWS.Cells(headerstartrow, i + 1).ColumnWidth = row.Cells(1, constMQFFieldColumnWidth + 1).Value
                i = i + 1
            Next row

        End If

        '***
        '*** Header of fields
        '***
        ReDim cols(UBound(FieldIDs))
        ReDim rocols(UBound(FieldIDs))    ' same size as FieldsIDs -> Crossreference with Column

        ' init
        'Call aProgressBar.initialize(UBound(FieldIDs), WindowCaption:="preprocessing MQF ...")
        'aProgressBar.showForm()

        '******* XCHANGE: create the config of the OTDB XChangeManager
        '*******
        aXChangeConfig = XChangeManager.createXChangeConfigFromIDs(CONFIGNAME:="$$mqftmp", _
                                                        IDs:=FieldIDs, _
                                                        XCMD:=otXChangeCommandType.Read, _
                                                        OBJECTNAMES:=New String() {"tblschedules", _
                                                                                  "tbldeliverabletargets", _
                                                                                  "tbldeliverables", _
                                                                                  "tblparts", _
                                                                                  "tbldeliverabletracks", _
                                                                                  "tblconfigs"})

        ReDim Preserve otdbvalues(UBound(FieldIDs))

        For i = 0 To UBound(FieldIDs)

            fieldname = FieldIDs(i)

            '**** XCHANGE
            If LCase(fieldname) = "uid" Then otdbvalue_uid_index = i

            ' get XLS Database Description -> necessary to get the XLS Column for formatting
            If getDBDesc(dbdesc, fieldname) Then
                cols(i) = dbdesc(0).ColumnNo    ' column in doc9
                ' check for READ_ONLY
                For n = 0 To UBound(ROFieldIDs)
                    If UCase(fieldname) = UCase(ROFieldIDs(n)) Then
                        ' crossreference the new column
                        rocols(i) = i + 1 + preheadercount    ' column ro in mqf
                    End If
                Next n

                '***
                '*** Copy -> Paste of ID (HEADER FORMATING)
                '***
                headerids.Cells(1, dbdesc(0).ColumnNo).Copy()
                MQFWS.Cells(headerstartrow, i + 1 + preheadercount).PasteSpecial(Excel.XlPasteType.xlPasteFormats)
                If MQFWS.Cells(headerstartrow, i + 1 + preheadercount).FormatConditions.count > 0 Then
                    For n = 1 To MQFWS.Cells(headerstartrow, i + 1 + preheadercount).FormatConditions.count
                        MQFWS.Cells(headerstartrow, i + 1 + preheadercount).FormatConditions(n).delete()
                    Next n
                End If
                MQFWS.Cells(headerstartrow, i + 1 + preheadercount).Value = headerids.Cells(1, dbdesc(0).ColumnNo).text
                MQFWS.Cells(headerstartrow, i + 1 + preheadercount).ColumnWidth = _
                headerids.Cells(1, dbdesc(0).ColumnNo).ColumnWidth
                ' Copy -> Paste Group
                'headerids.Cells(2, DBDesc(0).ColumnNo).Copy
                'MQFWS.Cells(Headerstartrow + 1, i + 1 + preheadercount).PasteSpecial
                'MQFWS.Cells(Headerstartrow + 1, i + 1 + preheadercount).ColumnWidth = headerids.Cells(2, DBDesc(0).ColumnNo).ColumnWidth
                ' Copy -> Paste od Title
                headerids.Cells(3, dbdesc(0).ColumnNo).Copy()
                MQFWS.Cells(headerstartrow + 1, i + 1 + preheadercount).PasteSpecial(Excel.XlPasteType.xlPasteFormats)
                If MQFWS.Cells(headerstartrow + 1, i + 1 + preheadercount).FormatConditions.count > 0 Then
                    For n = 1 To MQFWS.Cells(headerstartrow + 1, i + 1 + preheadercount).FormatConditions.count
                        MQFWS.Cells(headerstartrow + 1, i + 1 + preheadercount).FormatConditions(n).delete()
                    Next n
                End If
                MQFWS.Cells(headerstartrow + 1, i + 1 + preheadercount).Value = headerids.Cells(3, dbdesc(0).ColumnNo).text
                MQFWS.Cells(headerstartrow + 1, i + 1 + preheadercount).ColumnWidth = _
                headerids.Cells(3, dbdesc(0).ColumnNo).ColumnWidth
            Else
                Call CoreMessageHandler(showmsgbox:=True, message:="Abort: The Field-ID '" & fieldname & " ' is not found in the OnTrack Database Description." _
                       & MQFWorkbook.Name & "!" _
                 , subname:="modMQF.createXlsDoc9MQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)

                createXlsDoc9MQF = False
                Exit Function
            End If
        Next i

        ' if postheader ist found
        If Not postheader Is Nothing Then
            '*** post-headers
            ' i is to be used from above !
            i = i + 1
            postheaderstart = i
            For Each row In postheader.Rows
                ' ID
                MQFWS.Cells(headerstartrow, i + 1).Value = row.Cells(1, constMQFFieldheader + 1).Value
                row.Cells(1, constMQFFieldheader + 1).Copy()
                MQFWS.Cells(headerstartrow, i + 1).PasteSpecial(Excel.XlPasteType.xlPasteFormats)
                ' Title
                MQFWS.Cells(headerstartrow + 1, i + 1) = row.Cells(1, constMQFfieldname + 1).Value
                row.Cells(1, constMQFfieldname + 1).Copy()
                MQFWS.Cells(headerstartrow + 1, i + 1).PasteSpecial(Excel.XlPasteType.xlPasteFormats)

                ' width
                MQFWS.Cells(headerstartrow, i + 1).ColumnWidth = row.Cells(1, constMQFFieldColumnWidth + 1).Value
                i = i + 1
            Next row
        End If

        ' height of header
        MQFWS.Cells(headerstartrow, 1).RowHeight = headerids.Cells(1, 1).RowHeight
        MQFWS.Cells(headerstartrow + 1, 1).RowHeight = headerids.Cells(3, 1).RowHeight
        ' generate the name
        'constMQFHeaderidName
        MQFWS.Parent.Names.add(constMQFHeaderidName, _
                               RefersTo:=MQFWS.Range(MQFWS.Cells(headerstartrow, 1), MQFWS.Cells(headerstartrow, i)))

        ' start data row
        startdatarow = headerstartrow + 1 + 1
        MaxCol = i
        'aProgressBar.closeForm()

        '**********
        '********** copy data
        '**********
        ' get the columnno of the action column
        Value = getMQFField(constMQFActionID)
        If Not Value Is Nothing Then
            operation_column = Value.Cells(1, constMQFFieldColumn + 1).Value
            If operation_column < 0 Then
                operation_column = operation_column + 1 + preheadercount
            Else
                operation_column = operation_column + 1 + postheadercount
            End If
        End If

        i = 0

        '*** create each row
        '*** in mqf
        ' set the selectioncol to look uid
        selectioncol = dbDoc9Range.Worksheet.Range(dbDoc9Range.Cells(1, uid_column), dbDoc9Range.Cells(dbDoc9Range.Rows.count, uid_column))

        ' init
        'Call aProgressBar.initialize(UBound(selectedUIDs), WindowCaption:="creating MQF ...")
        'aProgressBar.showForm()

        For rowno = 0 To UBound(selectedUIDs)
            'Call aProgressBar.progress(1, "writing .... #" & rowno)

            ' get row
            'value = getXLSHeaderIDColumn("uid")
            row = FindAll(selectioncol, selectedUIDs(rowno), LookIn:=Excel.XlFindLookIn.xlValues)
            '*
            If row Is Nothing Then
                System.Diagnostics.Debug.WriteLine("Row with UID#" & selectedUIDs(rowno) & " couldnt be found")

            Else
                'set row =
                If copyOld Then
                    i = i + 2
                Else
                    i = i + 1
                End If

                '******* XCHANGE get the data from the OTDB
                '*******
                For j = 0 To UBound(FieldIDs)
                    otdbvalues(j) = Nothing
                Next j
                otdbvalues(otdbvalue_uid_index) = selectedUIDs(rowno)
                '*** read all data -> will be used in copyOldLine or in READ_ONLY fields of the new line
                If XChangeManager.XChangeWithArray(aXChangeConfig, otdbvalues) Then
                End If


                ' Copy the fields
                If copyOld Then
                    ' prefill the operation code ->
                    MQFWS.Cells(startdatarow + i - 2, operation_column).Value = "noop"
                    For j = 0 To UBound(FieldIDs)
                        ' copy the old values
                        MQFWS.Cells(startdatarow + i - 2, j + 1 + preheadercount).Value = otdbvalues(j)
                        'row.EntireRow.Cells(1, cols(j)).text
                    Next j
                    ' copy format
                    'formatoldvalues.Copy
                    With MQFWS.Range(MQFWS.Cells(startdatarow + i - 2, 1), MQFWS.Cells(startdatarow + i - 2, MaxCol))    '.PasteSpecial xlPasteFormats
                        .Interior.Color = formatoldvalues.Interior.Color
                        .Font.Color = formatoldvalues.Font.Color
                        .Font.Name = formatoldvalues.Font.Name
                        .Font.Size = formatoldvalues.Font.size
                        .Font.Bold = formatoldvalues.Font.Bold
                        .Borders.LineStyle = formatoldvalues.Borders.LineStyle
                        .Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).Color = formatoldvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).Color
                        .Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).weight = formatoldvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeBottom).weight
                        .Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = formatoldvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeBottom).LineStyle
                        .Borders.Item(Excel.XlBordersIndex.xlEdgeTop).Color = formatoldvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeTop).Color
                        .Borders.Item(Excel.XlBordersIndex.xlEdgeTop).weight = formatoldvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeTop).weight
                        .Borders.Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = formatoldvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeTop).LineStyle
                        .Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).Color = formatoldvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeLeft).Color
                        .Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).weight = formatoldvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeLeft).weight
                        .Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle = formatoldvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeLeft).LineStyle
                        .Borders.Item(Excel.XlBordersIndex.xlEdgeRight).Color = formatoldvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeRight).Color
                        .Borders.Item(Excel.XlBordersIndex.xlEdgeRight).weight = formatoldvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeRight).weight
                        .Borders.Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = formatoldvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeRight).LineStyle
                    End With

                    ' protect
                    MQFWS.Range(MQFWS.Cells(startdatarow + i - 2, 1), MQFWS.Cells(startdatarow + i - 2, MaxCol)).Locked = True
                End If

                ' create new line
                ' operations code
                With MQFWS.Cells(startdatarow + i - 1, operation_column).Validation
                    .purge()
                    .add(Type:=Excel.XlDVType.xlValidateList, AlertStyle:=Excel.XlDVAlertStyle.xlValidAlertStop, _
                         Operator:=Excel.XlFormatConditionOperator.xlBetween, Formula1:="=parameter_template_action_table")
                    .IgnoreBlank = True
                    .InCellDropdown = True
                    .InputTitle = "Input"
                    .ErrorTitle = "Error"
                    .InputMessage = "Please enter the Operation Code"
                    .ErrorMessage = "Please provide correct Operation Code"
                    .ShowInput = True
                    .ShowError = True
                End With

                '** copy the existing value from the messages if existing
                '**
                If copyValuesFromOldMessages Then
                    For m = LBound(aMessagesToCopy) To UBound(aMessagesToCopy)
                        If aMessagesToCopy(m).UID = selectedUIDs(rowno) Then
                            For j = 0 To UBound(FieldIDs)
                                If getMQFDBDesc(aMQFDbDesc, aName:=FieldIDs(j), WORKBOOK:=theMQFWorkbookToCopy) Then

                                    ' copy the old values
                                    MQFWS.Cells(startdatarow + i - 1, _
                                                j + 1 + preheadercount).Value = "'" & _
                                                CStr(aMessagesToCopy(m).fieldvalues(aMQFDbDesc(0).ColumnNo - 1))
                                End If
                            Next j
                        End If
                    Next m
                End If

                ' format
                'formatnewvalues.Copy
                With MQFWS.Range(MQFWS.Cells(startdatarow + i - 1, 1), MQFWS.Cells(startdatarow + i - 1, MaxCol))

                    '.PasteSpecial xlPasteFormats
                    .Interior.Color = formatnewvalues.Interior.Color
                    .Font.Color = formatnewvalues.Font.Color
                    .Font.Name = formatnewvalues.Font.Name
                    .Font.Size = formatnewvalues.Font.size
                    .Font.Bold = formatnewvalues.Font.Bold
                    .Borders.LineStyle = formatnewvalues.Borders.LineStyle
                    .Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).Color = formatnewvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).Color
                    .Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).weight = formatnewvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeBottom).weight
                    .Borders.Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = formatnewvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeBottom).LineStyle
                    .Borders.Item(Excel.XlBordersIndex.xlEdgeTop).Color = formatnewvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeTop).Color
                    .Borders.Item(Excel.XlBordersIndex.xlEdgeTop).weight = formatnewvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeTop).weight
                    .Borders.Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = formatnewvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeTop).LineStyle
                    .Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).Color = formatnewvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeLeft).Color
                    .Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).weight = formatnewvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeLeft).weight
                    .Borders.Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle = formatnewvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeLeft).LineStyle
                    .Borders.Item(Excel.XlBordersIndex.xlEdgeRight).Color = formatnewvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeRight).Color
                    .Borders.Item(Excel.XlBordersIndex.xlEdgeRight).weight = formatnewvalues.Borders.ITEM(Excel.XlBordersIndex.xlEdgeRight).weight
                    .Borders.Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = formatnewvalues.Borders.Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle

                    ' not locked
                    .Locked = False
                    ' bold
                    .Font.Bold = True
                End With
                ' special format
                ' fill the copy values
                For n = 0 To UBound(rocols)
                    ' cross
                    If rocols(n) <> 0 Then
                        With MQFWS.Cells(startdatarow + i - 1, rocols(n))
                            '.Value = row.EntireRow.Cells(1, cols(n)).text
                            .Value = otdbvalues(n)
                            .Locked = True
                            .Interior.Color = constLockedBackground
                            .Font.Bold = False
                        End With
                    End If
                    ' special handling MOCKUP STATUS
                    '*
                    If LCase(FieldIDs(n)) = "bp5" Then
                        With MQFWS.Cells(startdatarow + i - 1, n + 1 + preheadercount).Validation
                            .purge()

                            .add(Type:=Excel.XlDVType.xlValidateList, AlertStyle:=Excel.XlDVAlertStyle.xlValidAlertStop, _
                         Operator:=Excel.XlFormatConditionOperator.xlBetween, Formula1:="=parameter_dmu_status")
                            .IgnoreBlank = True
                            .InCellDropdown = True
                            .InputTitle = "Input"
                            .ErrorTitle = "Error"
                            .InputMessage = "Please enter the DMU status code"
                            .ErrorMessage = "Please provide correct DMU status code"
                            .ShowInput = True
                            .ShowError = True
                        End With
                    End If
                    ' special handling FEM STATUS
                    '*
                    If LCase(FieldIDs(n)) = "bp21" Then
                        With MQFWS.Cells(startdatarow + i - 1, n + 1 + preheadercount).Validation
                            .purge()

                            .add(Type:=Excel.XlDVType.xlValidateList, AlertStyle:=Excel.XlDVAlertStyle.xlValidAlertStop, _
                        Operator:=Excel.XlFormatConditionOperator.xlBetween, Formula1:="=parameter_fem_status")
                            .IgnoreBlank = True
                            .InCellDropdown = True
                            .InputTitle = "Input"
                            .ErrorTitle = "Error"
                            .InputMessage = "Please enter the FEM status code"
                            .ErrorMessage = "Please provide correct FEM status code"
                            .ShowInput = True
                            .ShowError = True
                        End With
                    End If
                Next n
                For n = postheaderstart + 1 To postheaderstart + postheadercount
                    With MQFWS.Cells(startdatarow + i - 1, n)
                        .Value = ""
                        .Locked = True
                        .Interior.Color = constProcessBackground
                        .Font.Bold = True
                    End With
                Next n
                Globals.ThisAddIn.Application.StatusBar = " copy line " & i & " from OnTrack to message queue file"

            End If    ' ROW Not Nothing
        Next rowno

        '***
        '*** save parameters
        '***
        'set first data row in MQF
        flag = SetXlsParameterValueByName(name:="parameter_mqf_template_datastartrow", value:=startdatarow, workbook:=MQFWorkbook)
        'doc9_mqf_createdby
        flag = SetXlsParameterValueByName("doc9_mqf_createdby", Globals.ThisAddIn.Application.UserName, workbook:=MQFWorkbook)
        flag = SetXlsParameterValueByName("doc9_mqf_createdon", Format(Date.Now(), "dd.mm.yyyy"), workbook:=MQFWorkbook)
        'parameter_doc9_extract_tooling
        flag = SetXlsParameterValueByName("parameter_mqf_extract_tooling", getDoc9ToolingName(Globals.ThisAddIn.Application.Globals.ThisAddin.Application.Workbooks(currWorkbookName)), workbook:=MQFWorkbook)
        flag = SetXlsParameterValueByName("doc9_mqf_doc9used", dbDoc9Range.Worksheet.Parent.Name, workbook:=MQFWorkbook)
        flag = SetXlsParameterValueByName("doc9_mqf_doc9usedon", Format(Date.Now(), "dd.mm.yyyy") & " " & Format(Date.Now(), "hh:mm"), workbook:=MQFWorkbook)

        'flag = setXLSParameterValueByName("parameter_recent_ICD_change_date", _
        'MQFWorkbook.BuiltinDocumentProperties(12).value)
        '

        flag = SetXlsParameterValueByName("parameter_mqf_headerid_name", constMQFHeaderidName, workbook:=MQFWorkbook, silent:=True)
        ' update the description table
        Call updateMQFDescTable(MQFWorkbook, ROFieldIDs)

        ' Activate Matrix again

        Globals.ThisAddIn.Application.ScreenUpdating = True
        'MQFWorkbook.Activate
        'MQFWorkbook.Sheets(MQFWorksheetName).Activate
        ' autofilter
        MQFWS.Range(MQFWS.Cells(headerstartrow + 1, 1), MQFWS.Cells(headerstartrow + 1, MaxCol)).AutoFilter()
        ' protect
        MQFWorkbook.Sheets(MQFWorksheetName).Protect(password:=constPasswordTemplate, _
                                                     DrawingObjects:=False, Contents:=True, Scenarios:=False, _
                                                     AllowFormattingCells:=True, AllowFormattingColumns:=True, _
                                                     AllowFormattingRows:=True, AllowInsertingColumns:=True, AllowInsertingRows:=True, _
                                                     AllowDeletingColumns:=True, AllowDeletingRows:=True, _
                                                     AllowFiltering:=True, AllowUsingPivotTables:=True, AllowSorting:=True)
        '
        MQFWorkbook.Sheets(constParameterSheetName).Protect(password:=constPasswordTemplate, _
                                                            DrawingObjects:=False, Contents:=True, Scenarios:=False, _
                                                            AllowFormattingCells:=False, AllowFormattingColumns:=False, _
                                                            AllowFormattingRows:=False, AllowInsertingColumns:=False, AllowInsertingRows:=False, _
                                                            AllowDeletingColumns:=False, AllowDeletingRows:=False, _
                                                            AllowFiltering:=False, AllowUsingPivotTables:=False)


        '*
        'aProgressBar.closeForm()

        ' write the template
        MQFWorkbook.SaveAs(Filename:=Filename, FileFormat:=Excel.XlFileFormat.xlOpenXMLWorkbookMacroEnabled)
        If CloseAfterCreation Then
            MQFWorkbook.Close()
        Else
            MQFWorkbook = MQFWorkbook
        End If
        ' exit
        Globals.ThisAddIn.Application.Globals.ThisAddin.Application.Workbooks(currWorkbookName).Activate()
        createXlsDoc9MQF = True
        Exit Function

handleerror:
        Globals.ThisAddIn.Application.Globals.ThisAddin.Application.Workbooks(currWorkbookName).Activate()
        createXlsDoc9MQF = False
        Exit Function

    End Function

    '********
    '******** getMQFDBDesc() returns the MQFDBDesc of the MQF or search for aName
    '********
    '********

    Public Function getMQFDBDesc(ByRef FieldList() As MQFDBDesc, _
                                 Optional ByVal aName As String = "", _
                                 Optional WORKBOOK As Excel.Workbook = Nothing, _
                                 Optional silent As Boolean = False) As Boolean
        Dim headerids As Range
        Dim headerids_name As String
        Dim DescTable As Range
        Dim row As Range
        Dim i As Integer
        Dim startdesccell As Range
        Dim Prefix As String
        Dim mqfversion As String
        'Dim fieldlist() As xlsDBDesc

        If IsMissing(WORKBOOK) Then
            WORKBOOK = GetGlobalDoc9()
        End If

        headerids_name = GetXlsParameterByName("parameter_mqf_headerid_name", workbook:=WORKBOOK)
        headerids = GetXlsParameterRangeByName(name:=headerids_name, workbook:=WORKBOOK)
        'parameter_doc9_dbdesc_prefix
        Prefix = GetXlsParameterByName("parameter_mqf_dbdesc_prefix", workbook:=WORKBOOK)
        mqfversion = GetXlsParameterByName("doc9_mqf_version", WORKBOOK, silent:=True)

        ' error
        If headerids Is Nothing Then
            Call CoreMessageHandler(showmsgbox:=True, message:="The parameter 'parameter_mqf_headerid_name':" & headerids_name & " is not showing a valid range !" _
                , subname:="modMQF.getMQFDBDesc", messagetype:=otCoreMessageType.ApplicationError, break:=False)

            getMQFDBDesc = False
            Exit Function
        End If

        DescTable = GetXlsParameterRangeByName("parameter_mqf_structure_db_description_table", workbook:=WORKBOOK)
        ' error
        If DescTable Is Nothing Then
            If Not silent Then
                Call CoreMessageHandler(showmsgbox:=True, message:="The parameter 'parameter_mqf_structure_db_description_table' is not showing a valid range !" _
               , subname:="modMQF.getMQFDBDesc", messagetype:=otCoreMessageType.ApplicationError, break:=False)

            End If
            getMQFDBDesc = False
            Exit Function
        End If

        ' upper right corner
        'Set startdesccell = desctable(1, 1)

        ' run through the rows
        i = -1
        For Each row In DescTable.Rows
            If Trim(row.Cells(1, xlsDBDescColNo.FieldID + 1).value) Like aName Or aName = "" Then
                i = i + 1
                ReDim Preserve FieldList(i)
                FieldList(i).ID = Trim(row.Cells(1, constMQFDescFieldID + 1).value)
                FieldList(i).TITLE = Trim(row.Cells(1, constMQFDescTitle + 1).value)
                FieldList(i).ColumnNo = CInt(row.Cells(1, constMQFDescColumnNo + 1).value)


                If mqfversion = "V_02" Or mqfversion = "V_03" Then
                    If row.Cells(1, constMQFReadonly + 1).value Is Nothing Then
                        FieldList(i).READ_ONLY = False
                    Else
                        FieldList(i).READ_ONLY = True
                    End If
                    'FieldList(i).READ_ONLY = Not IsEmpty(row.Cells(1, constMQFReadonly + 1).value)
                End If
                If mqfversion = "V_03" Then
                    FieldList(i).OBJECTNAME = CStr(row.Cells(1, constMQFObjectName + 1).value)
                Else
                    FieldList(i).OBJECTNAME = ""

                End If
                FieldList(i).mqfversion = mqfversion
            End If
        Next row

        If i >= 0 Then
            getMQFDBDesc = True
        Else
            getMQFDBDesc = False
        End If

    End Function

    '********
    '******** update the doc9 MQF DB Structure Description Table
    '********
    '********

    Public Sub updateMQFDescTable(WORKBOOK As Excel.Workbook, Optional ROFieldIDs As Object() = Nothing)
        Dim headerids As Range
        Dim headerids_name As String
        Dim DescTable As Range
        Dim cell As Range
        Dim i, j As Integer
        Dim startdesccell As Range
        Dim Prefix As String
        Dim pn As Name
        Dim mqfversion As String




        headerids_name = GetXlsParameterByName("parameter_mqf_headerid_name", WORKBOOK)
        headerids = GetXlsParameterRangeByName(headerids_name, WORKBOOK)
        'parameter_doc9_dbdesc_prefix
        Prefix = GetXlsParameterByName("parameter_mqf_dbdesc_prefix", WORKBOOK, silent:=True)
        mqfversion = GetXlsParameterByName("doc9_mqf_version", WORKBOOK, silent:=True)
        If mqfversion = "V_01" Then
            Call SetXlsParameterValueByName("doc9_mqf_version", "V_02", WORKBOOK, silent:=True)
        End If

        ' error
        If headerids Is Nothing Then
            Call CoreMessageHandler(showmsgbox:=True, message:="The parameter 'parameter_mqf_headerid_name':" & headerids_name & " is not showing a valid range !" _
              , subname:="modMQF.updateMQFDBDescTable", messagetype:=otCoreMessageType.ApplicationError, break:=False)

            Exit Sub
        End If

        DescTable = GetXlsParameterRangeByName("parameter_mqf_structure_db_description_table", WORKBOOK)
        ' error
        If DescTable Is Nothing Then
            Call CoreMessageHandler(showmsgbox:=True, message:="The parameter 'parameter_mqf_structure_db_description_table' is not showing a valid range !" _
            , subname:="modMQF.updateMQFDBDescTable", messagetype:=otCoreMessageType.ApplicationError, break:=False)


            Exit Sub
        End If

        ' upper right corner
        startdesccell = DescTable(1, 1)

        ' run through the headerids
        i = 0
        For Each cell In headerids
            ' insert new row
            If i >= DescTable.Rows.count Then
                startdesccell.offset(i, 0).EntireRow.Insert()
            End If
            ' Header ID
            startdesccell.Offset(i, constMQFDescFieldID).Value = cell.Value
            If Trim(cell.Value) = "" Then
                startdesccell.offset(i, constMQFDescFieldID).Interior.Color = constErrorBackground
            Else
                startdesccell.offset(i, constMQFDescFieldID).Interior.Color = startdesccell.offset(0, 0).Interior.Color
            End If
            ' Description
            startdesccell.offset(i, constMQFDescTitle).Value = cell.offset(1, 0).Value
            ' Column
            startdesccell.offset(i, constMQFDescColumnNo).Value = i + 1
            If NameExistsinWorkbook(startdesccell.Worksheet.Parent, Prefix & cell.Value) Then
                pn = DescTable.Worksheet.Parent.Names(Prefix & cell.Value)
                pn.Delete()
            End If
            DescTable.Worksheet.Parent.Names.add( _
                    Name:=Prefix & cell.Value, RefersTo:=startdesccell.offset(i, 2))
            ' readolny
            If IsArrayInitialized(ROFieldIDs) Then
                For j = LBound(ROFieldIDs) To UBound(ROFieldIDs)
                    If ROFieldIDs(j) = cell.Value Then
                        startdesccell.offset(i, constMQFReadonly).Value = "Read-only"
                        Exit For
                    Else
                        startdesccell.offset(i, constMQFReadonly).Value = ""
                    End If
                Next j
            Else
                startdesccell.offset(i, constMQFReadonly).Value = ""
            End If
            ' inc
            i = i + 1
        Next cell

        DescTable = startdesccell.Worksheet.Range(startdesccell, startdesccell.offset(i - 1, 2))
        If Not SetXlsParameterValueByName("parameter_mfq_dbdesc_range", DescTable.Address, WORKBOOK) Then System.Diagnostics.Debug.WriteLine("parameter_doc9_dbdesc_range doesnot exist ?!")
        ' delete
        If NameExistsinWorkbook(startdesccell.Worksheet.Parent, "parameter_mqf_structure_db_description_table") Then
            pn = DescTable.Worksheet.Parent.Names("parameter_mqf_structure_db_description_table")
            pn.Delete()
        End If
        ' define
        DescTable.Worksheet.Parent.Names.add( _
                Name:="parameter_mqf_structure_db_description_table", RefersTo:=DescTable)


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
        FileNamePattern = GetDBParameter("parameter_mqfilename_prefix", silent:=True)
        FileNamePattern = FileNamePattern & Globals.ThisAddIn.Application.UserName & "_" & Format(Date.Now(), "yyyy-mm-dd") & "_" & Format(Date.Now(), "hhmm")

        'Open Dialog for Find
        Value = GetDBParameter("parameter_startfoldernode")
        If Value <> "" And FileIO.FileSystem.FileExists(Value) Then
            If Mid(Value, Len(Value), 1) <> "\" Then Value = Value & "\"
            If Mid(Value, 2, 1) = ":" Then
                ChDrive(Mid(Value, 1, 2))
            End If
            Value = Value & GetDBParameter("parameter_mqf_output")
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

    ' ***************************************************************************************************
    '  preProcessXLSMQF -> Read the Excel MQF, create a MQFObject and run the prechecks
    '
    '  MQFWorkbook as Workbook
    '  MQFObject as clsOTDBMessageQueue
    '
    '

    Function preProcessXLSMQF(ByRef MQFWorkbook As Excel.Workbook, _
                              ByRef MQFObject As clsOTDBMessageQueue, _
                              Optional ByRef workerthread As BackgroundWorker = Nothing _
                             ) As Boolean
        Dim aVAlue As Object
        Dim headerstartrow As Integer
        Dim headerids_name As String
        Dim headerids As Range
        Dim Prefix As String
        Dim cell As Range
        Dim DescTable As Range
        Dim datawsname As String
        Dim dataws As Excel.Worksheet
        Dim datefields As String
        'Dim theMQFFormat As MessageFormat
        Dim FieldList() As MQFDBDesc
        Dim startrow As Integer
        Dim foundflag As Boolean
        Dim row As Range
        Dim mfgStatus As New clsMQFStatus
        Dim changeflag As Boolean
        Dim setro_flag As Boolean
        Dim newStatus As New clsMQFStatus
        Dim checkascdates As Boolean

        Dim aXCMD As otXChangeCommandType
        Dim aColumnNo As Long
        Dim theUIDCol As Long
        Dim anObjectName As String
        Dim anID As String
        Dim i, j, k, l As Integer
        Dim n As Long
        Dim mqfDBRange As Range
        Dim listofAttributes As New Collection
        Dim aConfigmember As New clsOTDBXChangeMember
        Dim aMQFRowEntry As New clsOTDBMessageQueueEntry
        Dim aMQFMember As New clsOTDBMessageQueueMember
        Dim aStatus As New clsOTDBDefStatusItem
        'Dim aProgressBar As clsUIProgressBarForm
        Dim maximum As Long
        Dim progress As Long

        ' create the MQFObject
        If MQFObject Is Nothing Then
            MQFObject = New clsOTDBMessageQueue
        End If

        ' cache the MQFWOrkbook
        cacheAllWorkbookNames(MQFWorkbook)


        '**
        '** create a MQFObject
        If Not MQFObject.IsLoaded And Not MQFObject.IsCreated Then
            ' check if we have one
            aVAlue = GetXlsParameterByName(name:="parameter_mqf_tag", workbook:=MQFWorkbook, silent:=True)
            If aVAlue = "" Then
                aVAlue = GetHostProperty("parameter_mqf_tag", host:=MQFWorkbook, silent:=True)
            End If
            ' create
            If aVAlue = "" Then
                aVAlue = MQFWorkbook.Name & " " & CStr(Now)
                If Not MQFObject.Create(aVAlue) Then
                    MQFObject.loadBy(aVAlue)
                End If
            Else
                If Not MQFObject.loadBy(TAG:=aVAlue) Then
                    Call MQFObject.Create(TAG:=aVAlue)
                End If
            End If
            ' save it to properties
            If aVAlue <> "" Then
                Call SetXlsParameterValueByName("parameter_mqf_tag", aVAlue, workbook:=MQFWorkbook, silent:=True)
                Call SetHostProperty("parameter_mqf_tag", aVAlue, host:=MQFWorkbook, silent:=True)
            End If

        End If

        ' get Startrow
        aVAlue = modParameterXLS.GetXlsParameterByName("parameter_mqf_template_headerstartrow", workbook:=MQFWorkbook)
        If Not IsNumeric(aVAlue) Then
            preProcessXLSMQF = False
            'MQFWorkbook.Close (False)
            Exit Function
        End If
        headerstartrow = CInt(aVAlue)
        headerids_name = GetXlsParameterByName(name:="parameter_mqf_headerid_name", workbook:=MQFWorkbook)
        headerids = GetXlsParameterRangeByName(headerids_name, workbook:=MQFWorkbook)
        'parameter_doc9_dbdesc_prefix
        Prefix = GetXlsParameterByName(name:="parameter_mqf_dbdesc_prefix", workbook:=MQFWorkbook)
        datefields = GetDBParameter("parameter_plausibility_fc_asc_dates")

        ' error
        If headerids Is Nothing Then
            Call CoreMessageHandler(showmsgbox:=True, _
                                  message:="The parameter 'parameter_mqf_headerid_name':" & headerids_name & " is not showing a valid range !" _
                                , subname:="modMQF.preProcessXLSMQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)

            preProcessXLSMQF = False
            Exit Function
        End If

        DescTable = GetXlsParameterRangeByName(name:="parameter_mqf_structure_db_description_table", workbook:=MQFWorkbook, silent:=True)
        ' error
        If DescTable Is Nothing Then
            'MsgBox "The parameter 'parameter_doc9_structure_db_description_table' is not showing a valid range !", Buttons:=vbCritical, title:="OnTrack Tooling Error"
            System.Diagnostics.Debug.WriteLine("FATAL ERROR: " & "The parameter 'parameter_doc9_structure_db_description_table' is not showing a valid range !")

            'preProcessXLSMQF = False
            'Exit Function
        End If

        ' set the doc9

        checkascdates = False

        Globals.ThisAddIn.Application.ScreenUpdating = False
        '**** create the MessageFormat
        MQFObject.requestedBy = GetXlsParameterByName(name:="doc9_mqf_requestedby", workbook:=MQFWorkbook, silent:=True)
        MQFObject.requestedByOU = GetXlsParameterByName(name:="doc9_mqf_requestedby_department", workbook:=MQFWorkbook, silent:=True)
        aVAlue = GetXlsParameterByName(name:="doc9_mqf_requested_on", workbook:=MQFWorkbook, silent:=True)
        If Not IsDate(aVAlue) Then
            MQFObject.requestedOn = Date.Now()
        Else
            MQFObject.requestedOn = CDate(aVAlue)
        End If
        MQFObject.description = GetXlsParameterByName(name:="doc9_mqf_title", workbook:=MQFWorkbook, silent:=True)
        MQFObject.COMMENT = GetXlsParameterByName(name:="doc9_mqf_subject", workbook:=MQFWorkbook, silent:=True)
        MQFObject.XCHANGECONFIG = New clsOTDBXChangeConfig
        ' load the config if a name is given
        aVAlue = GetXlsParameterByName(name:="parameter_mqf_xchangeconfigname", workbook:=MQFWorkbook, silent:=True)
        If aVAlue <> "" Then
            If MQFObject.XCHANGECONFIG.LoadBy(configname:=aVAlue) Then
            Else
                MQFObject.XCHANGECONFIG = New clsOTDBXChangeConfig
                MQFObject.XCHANGECONFIG.Create(MQFObject.TAG)
            End If
        Else
            MQFObject.XCHANGECONFIG = New clsOTDBXChangeConfig
            MQFObject.XCHANGECONFIG.Create(MQFObject.TAG)
        End If
        ' index
        i = 0
        k = 0

        '*** HACK
        Call MQFObject.XCHANGECONFIG.AddObjectByName(Name:="tblschedules", XCMD:=otXChangeCommandType.Update)
        Call MQFObject.XCHANGECONFIG.AddObjectByName(Name:="tbldeliverabletargets", XCMD:=otXChangeCommandType.Update)
        Call MQFObject.XCHANGECONFIG.AddObjectByName(Name:="tbldeliverabletracks", XCMD:=otXChangeCommandType.Update)
        Call MQFObject.XCHANGECONFIG.AddObjectByName(Name:="tbldeliverables", XCMD:=otXChangeCommandType.Update)
        Call MQFObject.XCHANGECONFIG.AddObjectByName(Name:="tblparts", XCMD:=otXChangeCommandType.Update)
        Call MQFObject.XCHANGECONFIG.AddObjectByName(Name:="tblconfigs", XCMD:=otXChangeCommandType.Update)

        '** go through headerids
        '**
        Dim aMQFDBDescLookup As New Dictionary(Of String, MQFDBDesc)
        Dim anMQFDBDescEntry As MQFDBDesc
        If getMQFDBDesc(FieldList, WORKBOOK:=MQFWorkbook) Then
            For Each anMQFDBDescEntry In FieldList
                If Not aMQFDBDescLookup.ContainsKey(anMQFDBDescEntry.ID) Then
                    aMQFDBDescLookup.Add(key:=anMQFDBDescEntry.ID, value:=anMQFDBDescEntry)
                End If
            Next
        End If

        For Each cell In headerids
            If cell.Value <> "" And Not IsError(cell.Value) Then
                ' resolve the ID
                anID = CStr(cell.text)
                workerthread.ReportProgress(0, anID)
                ' try to get a MQFDBDesc in the parameters
                If aMQFDBDescLookup.ContainsKey(anID) Then
                    anMQFDBDescEntry = aMQFDBDescLookup.Item(key:=anID)


                    aColumnNo = anMQFDBDescEntry.ColumnNo
                    anObjectName = anMQFDBDescEntry.OBJECTNAME
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

                ' redim
                'ReDim Preserve theMQFFormat.desc(i)
                'theMQFFormat.desc(i).ID = cell.aValue
                ' description
                'theMQFFormat.desc(i).ColumnNo = FieldList(0).ColumnNo

                'default is that READ_ONLY
                'theMQFFormat.desc(i).READ_ONLY = FieldList(0).READ_ONLY
                'theMQFFormat.desc(i).mqfversion = FieldList(0).mqfversion
                'theMQFFormat.desc(i).title = FieldList(0).title
                ' special cells
                If LCase(cell.Value) = "uid" Then    ' this should be the min primary key which is for Doc9 UID
                    theUIDCol = aColumnNo
                ElseIf LCase(cell.Value) = "mqfx2" Then
                    MQFObject.ProcessDateordinal = aColumnNo
                ElseIf LCase(cell.Value) = "mqfx3" Then
                    MQFObject.ProcessLogordinal = aColumnNo
                ElseIf LCase(cell.Value) = "mqfx4" Then
                    MQFObject.ProcessStatusordinal = aColumnNo
                ElseIf LCase(cell.Value) = "mqfxaction" Then
                    MQFObject.Actionordinal = aColumnNo

                End If


                If MQFObject.XCHANGECONFIG.IsLoaded Then
                    Call MQFObject.XCHANGECONFIG.SetordinalForID(anID, aColumnNo)
                    'xcmd:=aXCMD) ' theMQFFormat.desc(i).ColumnNo
                ElseIf MQFObject.XCHANGECONFIG.IsCreated Then
                    ' add Attribute by ID
                    Call MQFObject.XCHANGECONFIG.AddAttributeByID(id:=anID, _
                                                                  ordinal:=CLng(aColumnNo), _
                                                                  objectname:=anObjectName, _
                                                                  xcmd:=aXCMD)    ' theMQFFormat.desc(i).ColumnNo
                Else
                    Call CoreMessageHandler(subname:="MQF.preProcessXLSMQF", _
                                          message:="xChangeConfig is neither created nor loaded")

                End If

                '** set flag for ascending and dates check -> if any fields of the parameter asc_fields is effected
                '**
                'If InStr(datefields, theMQFFormat.desc(i).ID) > 0 Then
                '    checkascdates = True
                'End If
                '** set flag for schedule is touched -> should be parametrized
                '**
                'If LCase(theMQFFormat.desc(i).ID) Like "bp*" Then
                '    checkschedule = True
                'End If
                '** set flag for revision is touched -> should be parametrized
                '**
                'If LCase(theMQFFormat.desc(i).ID) Like "c16" Then
                '    checkrevision = True
                'End If
                '** increment
                i = i + 1

            End If

        Next cell



        '*** error conditions after header processing
        'If checkschedule And Not checkrevision And n > 0 Then
        '    theMessages(n).log = addLog(theMessages(n).log, _
        '                                "ERROR: Revision (c16) has to be included in the MQF if schedules are touched !")
        '    Set theMessages(n).status = New clsMQFStatus
        '    theMessages(n).status.code = constStatusCode_error
        '   theMessages(n).processable = theMessages(n).status.isProcessed
        'End If
        '*** continue feeding
        '***

        ' set the READ_ONLY flag to be determined
        setro_flag = False
        ' add change tag
        'theMQFFormat.processchangetag = MQFWorkbook.name
        ' get datefields
        datefields = GetDBParameter("parameter_plausibility_fc_asc_dates")

        ' get startrow
        startrow = GetXlsParameterByName("parameter_mqf_template_datastartrow", workbook:=MQFWorkbook)
        ' assumption where to start ?!
        datawsname = GetXlsParameterByName("parameter_mqf_templatedata", workbook:=MQFWorkbook, found:=foundflag, silent:=False)
        If Not foundflag Then
            dataws = headerids.Worksheet
        Else
            dataws = MQFWorkbook.Sheets(datawsname)
        End If
        ' the full sheet
        Dim maxrow As Integer
        Dim MaxCol As Integer
        MaxCol = dataws.Cells(startrow, dataws.Columns.Count).End(Excel.XlDirection.xlToLeft).Column
        maxrow = dataws.Cells(dataws.Rows.Count, theUIDCol).End(Excel.XlDirection.xlUp).row
        ' set it
        mqfDBRange = dataws.Range(dataws.Cells(startrow, 1), dataws.Cells(maxrow, MaxCol))
        ' autofilter
        dataws.AutoFilterMode = False

        ' init
        If workerthread Is Nothing Then
            'aProgressBar = New clsUIProgressBarForm
            'Call aProgressBar.initialize(maxrow - startrow, WindowCaption:="preprocessing MQF ...")
            'aProgressBar.showForm()
        Else
            workerthread.ReportProgress(0, "")

            'workerthread.Minimum = 0
            maximum = maxrow - startrow
            'workerthread.Value1 = 0
            'workerthread.Text = "preprocessing MQF...."

        End If
        '**** through all
        '****
        n = 0

        ' get the attributes
        listofAttributes = MQFObject.XCHANGECONFIG.Attributes

        Dim aMQFXLSValueRange As Object = mqfDBRange.Value
        Dim rowno As ULong

        If IsArray(aMQFXLSValueRange) Then
            If CType(aMQFXLSValueRange, Array(,)).Rank = 0 Then
                Console.WriteLine("Rank of MQF Array = 0")
                System.Diagnostics.Debug.Assert(False)

            End If

        Else
            CoreMessageHandler(message:="No Array Table of MQF Data Input", arg1:=mqfDBRange.Address, subname:="preprocessxlsmqf")
            Return False
        End If


        '*********
        '********* step thorugh 2-dimensional array from MQF and build MQFRowEntry objects -> preprocess these
        '*********
        For rowno = LBound(aMQFXLSValueRange, 1) To UBound(aMQFXLSValueRange, 1)

            changeflag = False

            aMQFRowEntry = MQFObject.createEntry(rowno:=rowno)
            '** progress
            If workerthread Is Nothing Then
                'Call aProgressBar.progress(1, Statustext:="preprocessing row #" & rowno)
            Else
                progress += 1
                workerthread.ReportProgress((progress / maximum) * 100, "preprocessing row #" & rowno)
            End If
            '** Application Bar
            Globals.ThisAddIn.Application.StatusBar = " preprocessing from " & MQFWorkbook.Name & " row#" & rowno

            '***** ACTION COMMAND
            '*****
            aVAlue = aMQFXLSValueRange(rowno, MQFObject.Actionordinal)
            If aMQFRowEntry.verifyAction(LCase(Trim(aVAlue))) Then
                aMQFRowEntry.action = LCase(Trim(aVAlue))
                aMQFRowEntry.processable = aMQFRowEntry.isActionProcessable
            End If


            '** already processed -> Process Date
            '**
            aVAlue = aMQFXLSValueRange(rowno, MQFObject.ProcessDateordinal)
            If IsDate(aVAlue) Then
                'theMessages(n).log = addLog(theMessages(n).log, _
                '                            "INFO:In row#" & rowno & " UID #" & theMessages(n).UID & ": message already processed on " & format(aValue, "dd.mm.yyyy") & "")
                'theMessages(n).processable = theMessages(n).processable And True
            End If

            '** already processed -> Status
            '**
            aVAlue = aMQFXLSValueRange(rowno, MQFObject.ProcessStatusordinal)
            'Set newStatus = New clsMQFStatus
            'newStatus.code = aValue
            '**
            'If newStatus.Verify(aValue) Then
            '** if processed
            '   If newStatus.isProcessed Then
            'theMessages(n).log = addLog(theMessages(n).log, _
            '                            "INFO:In row#" & rowno & " uid #" & theMessages(n).UID & " : message already succesfully processed - skipped.")
            'theMessages(n).processable = False
            '   Else
            ' reprocess
            'theMessages(n).log = addLog(theMessages(n).log, _
            '                            "INFO:In row#" & rowno & " uid #" & theMessages(n).UID & " : message not succesfully processed - retry it in next process.")
            'theMessages(n).processable = theMessages(n).processable And True
            '   End If
            ' take the new status if we have no other
            'If theMessages(n).status Is Nothing And newStatus.isProcessed Then
            'Set theMessages(n).status = newStatus
            'End If

            'Else
            ' set it to a status
            'If theMessages(n).status Is Nothing Then
            '    Set theMessages(n).status = New clsMQFStatus
            'End If
            'End If

            ' set the msglog identifiers
            aMQFRowEntry.ContextIdentifier = MQFObject.TAG
            aMQFRowEntry.TupleIdentifier = rowno

            '** phase1 in row : run through all fields of the row to get a full message
            '**
            aColumnNo = 0
            For Each aConfigmember In listofAttributes

                ' get the mapping to the Column
                If IsNumeric(aConfigmember.ordinal.Value) Then
                    aColumnNo = CLng(aConfigmember.ordinal.Value)
                Else
                    aColumnNo = aColumnNo + 1
                End If


                ' get aValue
                aVAlue = aMQFXLSValueRange(rowno, aColumnNo)
                If IsEmpty(aVAlue) Then
                    aVAlue = Nothing
                    'ElseIf  aVAlue = aMQFXLSValueRange(rowno,Columno).HasFormula Then
                    '    aVAlue = row.Cells(1, aColumnNo).text
                ElseIf IsDate(aVAlue) Then
                    aVAlue = CDate(aVAlue)
                ElseIf IsNumeric(aVAlue) Then
                    aVAlue = CDbl(aVAlue)
                ElseIf IsError(aVAlue) Then
                    aVAlue = Nothing    '-> if otRead will be overwritten anyway on otUpdate nothing will happen
                    'theMessages(n).log = addLog(theMessages(n).log, _
                    '                            "ERROR: '" & aValue & "' is computed cell with error in row#" & rowno)
                    'Set theMessages(n).status = New clsMQFStatus
                    'theMessages(n).status.code = constStatusCode_error
                    'theMessages(n).processable = theMessages(n).status.isProcessed
                End If

                '**
                '** store the aValues it
                ' theMessages(n).fieldvalues(i) = aValue
                If Not aVAlue Is Nothing Then
                    ' create a new Member in the RowEntry
                    aMQFMember = aMQFRowEntry.createMember
                    With aMQFMember
                        .xChangeID = aConfigmember.ID
                        .Value = aVAlue
                        .OBJECTNAME = aConfigmember.OBJECTNAME
                        .Entryname = aConfigmember.Entryname
                        .Ordinal.Value = aConfigmember.ordinal.Value
                        .DATATYPE = aConfigmember.ObjectEntryDefinition.Datatype
                    End With
                End If

                ' set the changeflag
                If Trim(aVAlue) <> "" And aConfigmember.xChangeCmd <> otXChangeCommandType.Read And _
                   aMQFRowEntry.processable Then
                    changeflag = changeflag Or True
                End If
                '** set the changeflag
                'If Trim(aValue) <> "" And Not LCase(theMQFFormat.desc(i).ID) Like "mqf*" And _
                '   theMQFFormat.desc(i).READ_ONLY = False And theMessages(n).processable Then
                '    changeflag = changeflag Or True
                'End If

            Next aConfigmember    ' run through fields

            '*** phase 2 in row: next phase -> check on aValues only if all aValues are set in the RowEntry
            '***
            If aMQFRowEntry.processable And changeflag Then
                ' check all fields depending on their aValue context-insensitive
                If Not aMQFRowEntry.runPreCheck Then
                    aMQFRowEntry.processable = False
                    ' status
                    If Not aMQFRowEntry.MSGLOG Is Nothing Then
                        aStatus = aMQFRowEntry.MSGLOG.GetStatus

                    Else
                        System.Diagnostics.Debug.WriteLine("MSGLog is nothign")

                    End If
                End If


            End If

            ' reset the processable flag if no change -> ""
            If Not changeflag And aMQFRowEntry.processable Then
                Call aMQFRowEntry.MSGLOG.AddMsg("301", _
                                                MQFObject.TAG, _
                                                aMQFRowEntry.rowno, _
                                                Nothing, aMQFRowEntry.rowno)
                '    theMessages(n).log = addLog(theMessages(n).log, _
                '                                "INFO:In row#" & rowno & " uid #" & theMessages(n).UID & " : message has no changes - skipped.")
                '    Set theMessages(n).status = New clsMQFStatus
                '    theMessages(n).status.code = constStatusCode_skipped
                '    theMessages(n).processable = theMessages(n).processable And theMessages(n).status.isProcessed
            End If

            ' increase
            n = n + 1
        Next rowno

        ' save the MQF
        If workerthread Is Nothing Then
            'aProgressBar.showStatus("saving MQF in OnTrack ... ")
        Else
            'workerthread.Text = "saving MQF in OnTrack ...."
            workerthread.ReportProgress(100, "saving MQF in OnTrack Database ...")
        End If
        ' Persist
        MQFObject.PERSIST()

        If workerthread Is Nothing Then
            ' aProgressBar.showStatus("saved MQF in OnTrack ... ")
            ' aProgressBar.closeForm()
        Else
            'workerthread.Text = "saved MQF in OnTrack ...."
        End If


        Globals.ThisAddIn.Application.StatusBar = n & " rows preprocesed from" & MQFWorkbook.Name
        'return
        preProcessXLSMQF = True

    End Function


    ' ***************************************************************************************************
    '  add a Field Status to the Message
    '
    '  aStatusCode
    '  aLog


    Public Function getStatus(theMessage As MQFMessage) As clsMQFStatus

        Dim fieldstatus() As Object
        Dim aStatus As New clsMQFStatus
        Dim i As Integer


        If theMessage.status Is Nothing Then
            theMessage.status = aStatus
        End If


        ' if Field Arrays
        If IsArrayInitialized(theMessage.fieldstatus) Then
            ' lookthrough
            For i = 0 To UBound(theMessage.fieldstatus)
                If Not IsEmpty(theMessage.fieldstatus(i)) Then
                    ' if to be approved and approved
                    If theMessage.fieldstatus(i).code = constStatusCode_forapproval And theMessage.isApproved Then
                        System.Diagnostics.Debug.WriteLine("was approved")

                        aStatus.code = constStatusCode_processed_ok    ' maybe is approved
                    Else
                        ' else the weight is ok
                        If aStatus.weight < theMessage.fieldstatus(i).weight Then
                            aStatus = theMessage.fieldstatus(i)
                        End If
                    End If
                End If
            Next i
            If aStatus.weight > theMessage.status.weight Then
                getStatus = aStatus
            Else
                If theMessage.status.code = constStatusCode_forapproval And theMessage.isApproved Then
                    aStatus.code = constStatusCode_processed_ok
                    getStatus = aStatus
                Else
                    getStatus = theMessage.status
                End If
            End If
            Exit Function
        Else
            getStatus = theMessage.status
            Exit Function
        End If

    End Function

    ' ***************************************************************************************************
    '  add a Field Status to the Message
    '
    '  aStatusCode
    '  aLog


    Public Function addFieldStatus(theMessage As MQFMessage, ByVal aFieldindex As Integer, _
                                   ByVal aStatusCode As String, ByVal aLog As String) As Boolean

        Dim fieldstatus() As Object
        Dim fieldlog() As Object
        Dim newStatus As New clsMQFStatus

        ' initialize Arrays
        If Not IsArrayInitialized(theMessage.fieldstatus) Then
            ReDim fieldstatus(UBound(theMessage.fieldvalues))
            theMessage.fieldstatus = fieldstatus
        End If
        ' initialize Arrays
        If Not IsArrayInitialized(theMessage.fieldlog) Then
            ReDim fieldlog(UBound(theMessage.fieldvalues))
            theMessage.fieldlog = fieldlog
        End If

        ' add status
        newStatus.code = aStatusCode
        theMessage.fieldstatus(aFieldindex) = newStatus
        ' add log
        theMessage.fieldlog(aFieldindex) = addLog(theMessage.fieldlog(aFieldindex), aLog)

        addFieldStatus = True

    End Function

    ' ***************************************************************************************************
    '  checkOnMQFAscendingDates check the Date fields if ascending
    '
    '  theMQFFormat as Messageformat
    '  theMessage as Message

    Public Function checkOnMQFAscendingDates(theMQFFormat As MessageFormat, theMessage As MQFMessage) As Boolean
        Dim Value As Object
        Dim MSG As String
        Dim date1, date2 As Object
        Dim dateids() As String
        Dim dateinputids() As String
        Dim dateindex() As Integer

        Dim datefields As String
        Dim i, j, n As Integer


        datefields = GetDBParameter("parameter_plausibility_fc_asc_dates")
        If datefields = "" Then
            checkOnMQFAscendingDates = True
            Exit Function
        End If

        dateinputids = SplitMultiDelims(text:=datefields, DelimChars:=constDelimeter)
        n = 1
        If IsArrayInitialized(dateinputids) Then

            For i = 1 To UBound(dateinputids)
                If InStr(dateinputids(i), constDelimeter) = 0 Then
                    dateinputids(i) = LCase(Trim(dateinputids(i)))
                    ' order in crossreference dateindex
                    For j = 0 To UBound(theMQFFormat.desc)
                        If LCase(theMQFFormat.desc(j).ID) = dateinputids(i) Then
                            ReDim Preserve dateindex(n)
                            ReDim Preserve dateids(n)
                            dateindex(n) = j
                            dateids(n) = theMQFFormat.desc(j).ID
                            n = n + 1
                            Exit For
                        End If
                    Next j
                End If
            Next i
        Else
            checkOnMQFAscendingDates = True
            Exit Function
        End If

        checkOnMQFAscendingDates = True

        '**
        '** now check on the dates ascending
        For i = 1 To UBound(dateindex)
            '** check only on Doc9Fields
            date1 = theMessage.fieldvalues(dateindex(i))

            If IsDate(date1) Then
                If i < UBound(dateindex) Then
                    For j = i + 1 To UBound(dateindex)
                        If IsDate(theMessage.fieldvalues(dateindex(j))) Then
                            date2 = theMessage.fieldvalues(dateindex(j))
                            Exit For
                        Else
                            date2 = Null()
                        End If
                    Next j
                Else
                    Exit For
                End If

                If IsDate(date2) Then

                    '** check the difference in days
                    Value = DateDiff("d", date1, date2)
                    If Value > 0 Then
                        System.Diagnostics.Debug.WriteLine("checking date " & dateids(i) & " with " & dateids(j) & " : " & Value)
                    ElseIf Value = 0 Then
                        MSG = "Warning: for uid #" & theMessage.UID & " date of field " & dateids(i) & "(" & Format(date1, "dd.mm.yyyy") & ")" & _
                              " is the same as date of field " & dateids(j) & "(" & Format(date2, "dd.mm.yyyy") & ")"
                        theMessage.log = addLog(theMessage.log, MSG)
                        theMessage.status = New clsMQFStatus
                        theMessage.status.code = constStatusCode_processed_warnings
                        theMessage.processable = theMessage.status.isProcessed And theMessage.processable
                        If addFieldStatus(theMessage, i, theMessage.status.code, MSG) Then
                        End If
                        checkOnMQFAscendingDates = False
                    Else
                        MSG = "Error: for uid #" & theMessage.UID & " date of field " & dateids(i) & "(" & Format(date1, "dd.mm.yyyy") & ")" & _
                              " is later as date of field " & dateids(j) & "(" & Format(date2, "dd.mm.yyyy") & ") - forecast milestone have to be ascending !"
                        theMessage.log = addLog(theMessage.log, MSG)
                        theMessage.status = New clsMQFStatus
                        theMessage.status.code = constStatusCode_error
                        theMessage.processable = theMessage.status.isProcessed And theMessage.processable
                        If addFieldStatus(theMessage, i, theMessage.status.code, MSG) Then
                        End If
                        checkOnMQFAscendingDates = False
                    End If
                End If

                '** set i to next j
                i = j - 1
            End If
        Next i





    End Function


    '****************************************************************************************************
    ' processXLSMQF
    '

    Function processXLSMQF(ByRef MQFWorkbook As Excel.Workbook, ByRef MQFObject As clsOTDBMessageQueue) As Boolean
        Dim aMQFRowEntry As clsOTDBMessageQueueEntry
        Dim aMapping As New Dictionary(Of Object, Object)
        Dim aMember As clsOTDBMessageQueueMember
        Dim aConfig As clsOTDBXChangeConfig
        Dim aConfigmember As clsOTDBXChangeMember
        'Dim aProgressBar As New clsUIProgressBarForm
        Dim aDeliverable As New Deliverables.Deliverable
        Dim aNewDeliverable As New Deliverables.Deliverable
        Dim aValue As Object
        Dim aWorkspace As String
        Dim aSchedule As New Scheduling.Schedule
        Dim aRefdate As New Date
        Dim aNewUID As Long

        Dim anUID As Long
        Dim aRev As String
        Dim i As Long

        ' init
        'Call aProgressBar.initialize(MQFObject.size, WindowCaption:="processing MQF  ...")
        'aProgressBar.showForm()
        If Not CurrentSession.IsRunning Then
            CurrentSession.StartUp(otAccessRight.ReadUpdateData)
        End If
        ' save
        MQFObject.processedByUsername = CurrentSession.OTdbUser.Username
        MQFObject.processdate = Now

        ' step through the RowEntries

        For Each aMQFRowEntry In MQFObject.Entries

            ' for each Member Check it with the XChangeConfig routines
            If aMQFRowEntry.action = constMQFOperation_CHANGE Then
                Call aMQFRowEntry.runXChange(MAPPING:=aMapping)
                ' get the Result
                'Set aMapping = New Dictionary
                'For Each aMember In aMQFRowEntry.Members
                '    '**
                '    Set aConfigmember = MQFObject.XCHANGECONFIG.AttributeByfieldname(aMember.fieldname, tablename:=aMember.OBJECTNAME)
                '    If aConfigmember.ISXCHANGED Then
                '        If Not aMapping.exists(Key:=aConfigmember.ordinal.value) Then
                '            Call aMapping.add(Key:=aConfigmember.ordinal.value, ITEM:=aMember.Value)
                '        End If
                '    End If

                'Next aMember
                aMQFRowEntry.processedOn = Now

                Call updateRowXlsDoc9(INPUTMAPPING:=aMapping, INPUTXCHANGECONFIG:=MQFObject.XCHANGECONFIG)
                '****
                '**** ADD REVISION
            ElseIf aMQFRowEntry.action = constMQFOperation_REVISION Then
                ' fill the Mapping
                aMapping = New Dictionary(Of Object, Object)
                Call aMQFRowEntry.fillMapping(aMapping)
                ' get UID
                aConfigmember = MQFObject.XCHANGECONFIG.AttributeByID(ID:="uid")
                If Not aConfigmember Is Nothing Then
                    If aConfigmember.IsLoaded Or aConfigmember.IsCreated Then
                        If aMapping.ContainsKey(key:=aConfigmember.ordinal.Value) Then
                            anUID = aMapping.Item(key:=aConfigmember.ordinal.Value)
                            aDeliverable = New Deliverables.Deliverable
                            If aDeliverable.LoadBy(uid:=anUID) Then
                                '** revision ?!
                                aConfigmember = MQFObject.XCHANGECONFIG.AttributeByID(ID:="c16")
                                If Not aConfigmember Is Nothing Then
                                    If aConfigmember.IsLoaded Or aConfigmember.IsCreated Then
                                        If aMapping.ContainsKey(key:=aConfigmember.ordinal.Value) Then
                                            aRev = aMapping.Item(key:=aConfigmember.ordinal.Value)
                                        Else
                                            aRev = ""
                                        End If
                                    Else
                                        aRev = ""
                                    End If
                                Else
                                    aRev = ""
                                End If
                                '**
                                aNewDeliverable = aDeliverable.AddRevision(newRevision:=aRev, persist:=True)
                                If Not aNewDeliverable Is Nothing Then
                                    ' substitute UID
                                    aConfigmember = MQFObject.XCHANGECONFIG.AttributeByID(ID:="uid")
                                    Call aMapping.Remove(key:=aConfigmember.ordinal.Value)
                                    Call aMapping.Add(key:=aConfigmember.ordinal.Value, value:=aNewDeliverable.Uid)
                                    ' substitute REV
                                    aConfigmember = MQFObject.XCHANGECONFIG.AttributeByID(ID:="c16")
                                    If Not aConfigmember Is Nothing Then
                                        If aConfigmember.IsLoaded Or aConfigmember.IsCreated Then
                                            If aMapping.ContainsKey(key:=aConfigmember.ordinal.Value) Then
                                                Call aMapping.Remove(key:=aConfigmember.ordinal.Value)
                                            End If
                                            Call aMapping.Add(key:=aConfigmember.ordinal.Value, value:=aNewDeliverable.Revision)
                                        End If
                                    End If
                                    ' substitute TYPEID or ADD
                                    aConfigmember = MQFObject.XCHANGECONFIG.AttributeByID(ID:="SC14")
                                    If aConfigmember Is Nothing Then
                                        If MQFObject.XCHANGECONFIG.AddAttributeByID(id:="SC14") Then
                                            aConfigmember = MQFObject.XCHANGECONFIG.AttributeByID(ID:="SC14")
                                        End If
                                    End If
                                    If aConfigmember.IsLoaded Or aConfigmember.IsCreated Then
                                        If aMapping.ContainsKey(key:=aConfigmember.ordinal.Value) Then
                                            Call aMapping.Remove(key:=aConfigmember.ordinal.Value)
                                        End If
                                        Dim aTrack As Deliverables.Track
                                        aTrack = aNewDeliverable.GetTrack
                                        If Not aTrack Is Nothing Then
                                            Call aMapping.Add(key:=aConfigmember.ordinal.Value, value:=aTrack.Scheduletype)
                                        End If
                                        'Call aMapping.Add(key:=aConfigmember.ordinal.value, c:=aNewDeliverable.getTrack.SCHEDULETYPE)
                                    End If

                                    '*** runxchange
                                    Call aMQFRowEntry.runXChange(MAPPING:=aMapping)
                                    aMQFRowEntry.processedOn = Now
                                    'how to save new uid ?!
                                    'Call updateRowXlsDoc9(INPUTMAPPING:=aMapping, INPUTXCHANGECONFIG:=MQFObject.XCHANGECONFIG)
                                Else
                                    Call CoreMessageHandler(subname:="MQF.processXLSMQF", message:="AddRevision failed", _
                                                          arg1:=aDeliverable.Uid)
                                End If
                            Else
                                Call CoreMessageHandler(subname:="MQF.processXLSMQF", message:="uid not in mapping", _
                                                      arg1:=anUID)
                            End If
                        Else
                            Call CoreMessageHandler(subname:="MQF.processXLSMQF", message:="load of Deliverable failed", _
                                                  arg1:=aConfigmember.ordinal.Value)
                        End If
                    Else
                        Call CoreMessageHandler(subname:="MQF.processXLSMQF", message:="uid id not in configuration", _
                                              arg1:="uid")
                    End If
                Else
                    Call CoreMessageHandler(subname:="MQF.processXLSMQF", message:="uid id not in configuration", _
                                          arg1:="uid")
                End If

                '****
                '**** ADD-AFTER
                '****
            ElseIf aMQFRowEntry.action = constMQFOperation_ADDAFTER Then
                ' fill the Mapping
                aMapping = New Dictionary(Of Object, Object)
                Call aMQFRowEntry.fillMapping(aMapping)

                ' create
                aDeliverable = New Deliverables.Deliverable
                aDeliverable = aDeliverable.CreateFirstRevision()
                If aDeliverable.IsCreated Then
                    aNewUID = aDeliverable.Uid
                    ' substitute UID
                    aConfigmember = MQFObject.XCHANGECONFIG.AttributeByID(ID:="uid")
                    If Not aConfigmember Is Nothing Then
                        If aConfigmember.IsLoaded Or aConfigmember.IsCreated Then
                            If aMapping.ContainsKey(key:=aConfigmember.ordinal.Value) Then
                                anUID = aMapping.Item(key:=aConfigmember.ordinal.Value)
                                Call aMapping.Remove(key:=aConfigmember.ordinal.Value)
                            Else
                                anUID = -1
                            End If
                        Else
                            If MQFObject.XCHANGECONFIG.AddAttributeByID(id:="uid") Then
                                aConfigmember = MQFObject.XCHANGECONFIG.AttributeByID(ID:="uid")
                            End If
                        End If
                    Else
                        If MQFObject.XCHANGECONFIG.AddAttributeByID(id:="uid") Then
                            aConfigmember = MQFObject.XCHANGECONFIG.AttributeByID(ID:="uid")
                        End If
                    End If

                    Call aMapping.Add(key:=aConfigmember.ordinal.Value, value:=aNewUID)


                    '*** runxchange
                    Call aMQFRowEntry.runXChange(MAPPING:=aMapping)
                    aMQFRowEntry.processedOn = Now
                    '*** TODO : ADD TO OUTLINE
                    System.Diagnostics.Debug.Write("new deliverable added: " & aNewUID & " to be added after uid #" & anUID)
                Else
                    Call CoreMessageHandler(subname:="MQF.processXLSMQF", message:="new deliverable couldn't be created", _
                                          arg1:=anUID, break:=False, messagetype:=otCoreMessageType.ApplicationError)
                End If


                '******
                '****** freeze
            ElseIf aMQFRowEntry.action = constMQFOperation_FREEZE Then
                aMapping = New Dictionary(Of Object, Object)
                Call aMQFRowEntry.fillMapping(aMapping)
                ' get UID
                aConfigmember = MQFObject.XCHANGECONFIG.AttributeByID(ID:="uid")
                If Not aConfigmember Is Nothing Then
                    If aConfigmember.IsLoaded Or aConfigmember.IsCreated Then
                        If aMapping.ContainsKey(key:=aConfigmember.ordinal.Value) Then
                            anUID = aMapping.Item(key:=aConfigmember.ordinal.Value)
                            aDeliverable = New Deliverables.Deliverable
                            If aDeliverable.LoadBy(uid:=anUID) Then
                                If Not aDeliverable.IsDeleted Then
                                    '*** set the workspaceID
                                    aValue = MQFObject.XCHANGECONFIG.GetMemberValue(ID:="WS", mapping:=aMapping)
                                    If IsNull(aValue) Then
                                        aWorkspace = CurrentSession.CurrentWorkspaceID
                                    Else
                                        aWorkspace = CStr(aValue)
                                    End If
                                    '***get the schedule
                                    aSchedule = aDeliverable.GetSchedule(workspaceID:=aWorkspace)
                                    If Not aSchedule Is Nothing Then
                                        If aSchedule.IsLoaded Then
                                            '*** reference date
                                            aRefdate = MQFObject.requestedOn
                                            If aRefdate = ConstNullDate Then
                                                aRefdate = Now
                                            End If
                                            '*** draw baseline
                                            Call aSchedule.DrawBaseline(REFDATE:=aRefdate)
                                        End If
                                    End If
                                End If

                            End If
                        End If
                    End If
                End If
                '****
                '**** Delete Deliverable
            ElseIf aMQFRowEntry.action = constMQFOperation_DELETE Then
                ' fill the Mapping
                aMapping = New Dictionary(Of Object, Object)
                Call aMQFRowEntry.fillMapping(aMapping)
                ' get UID
                aConfigmember = MQFObject.XCHANGECONFIG.AttributeByID(ID:="uid")
                If Not aConfigmember Is Nothing Then
                    If aConfigmember.IsLoaded Or aConfigmember.IsCreated Then
                        If aMapping.ContainsKey(key:=aConfigmember.ordinal.Value) Then
                            anUID = aMapping.Item(key:=aConfigmember.ordinal.Value)
                            aDeliverable = New Deliverables.Deliverable
                            If aDeliverable.LoadBy(uid:=anUID) Then
                                aDeliverable.Delete()

                            End If
                        End If
                    End If
                End If
            End If    ' commands

            i = i + 1
            'Call aProgressBar.progress(1, Statustext:="updating OnTrack Database message #" & i)
        Next

        'aProgressBar.showStatus("saving MQF in OnTrack ... ")
        MQFObject.PERSIST()
        'aProgressBar.showStatus("saved MQF in OnTrack ... ")

        'aProgressBar.closeForm()

        'return
        processXLSMQF = True

    End Function
    ' ***************************************************************************************************
    '  postprocessXLSMQF -> write to the MQF the results
    '

    '

    Function postprocessXLSMQF(ByRef MQFWorkbook As Excel.Workbook, ByRef MQFObject As clsOTDBMessageQueue) As Boolean
        Dim Value As Object
        Dim headerstartrow As Integer
        Dim headerids_name As String
        Dim headerids As Range
        Dim Prefix As String
        Dim cell As Range
        Dim DescTable As Range
        Dim datawsname As String
        Dim dataws As Excel.Worksheet

        'Dim theMQFFormat As MessageFormat
        Dim FieldList() As MQFDBDesc
        'Dim theMessages() As Message
        Dim i As Integer
        Dim n As Long

        Dim startrow As Integer
        Dim foundflag As Boolean
        Dim row As Range
        Dim mfgStatus As New clsMQFStatus
        Dim aStatus As clsMQFStatus


        Dim celllock As Boolean
        Dim MQFWorksheetName As String
        Dim MQFStatus As New clsMQFStatus

        Dim mqfDBRange As Range
        Dim aMQFRowEntry As New clsOTDBMessageQueueEntry



        ' get Startrow
        Value = GetXlsParameterByName("parameter_mqf_template_headerstartrow", workbook:=MQFWorkbook)
        If Not IsNumeric(Value) Then
            postprocessXLSMQF = False
            'MQFWorkbook.Close (False)
            Exit Function
        End If
        headerstartrow = CInt(Value)
        headerids_name = GetXlsParameterByName("parameter_mqf_headerid_name", workbook:=MQFWorkbook)
        headerids = GetXlsParameterRangeByName(headerids_name, workbook:=MQFWorkbook)
        ' error
        If headerids Is Nothing Then
            Call CoreMessageHandler(showmsgbox:=True, _
                                  message:="The parameter 'parameter_mqf_headerid_name':" & headerids_name & " is not showing a valid range !" _
                                , subname:="modMQF.postProcessXLSMQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)

            postprocessXLSMQF = False
            Exit Function
        End If

        'parameter_doc9_dbdesc_prefix
        Prefix = GetXlsParameterByName("parameter_mqf_dbdesc_prefix", workbook:=MQFWorkbook)
        MQFWorksheetName = GetXlsParameterByName("parameter_mqf_templatedata", workbook:=MQFWorkbook, silent:=True)
        If MQFWorksheetName <> "" Then
            If MQFWorkbook.Sheets(MQFWorksheetName).ProtectContents Then MQFWorkbook.Sheets(MQFWorksheetName).Unprotect(constPasswordTemplate)
        End If


        DescTable = GetXlsParameterRangeByName("parameter_mqf_structure_db_description_table", workbook:=MQFWorkbook)
        ' error
        If DescTable Is Nothing Then
            Call CoreMessageHandler(showmsgbox:=True, _
                                 message:="The parameter 'parameter_doc9_structure_db_description_table' is not showing a valid range !" _
                               , subname:="modMQF.postProcessXLSMQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)

            postprocessXLSMQF = False
            Exit Function
        End If

        ' get startrow
        startrow = GetXlsParameterByName("parameter_mqf_template_datastartrow", workbook:=MQFWorkbook)
        datawsname = GetXlsParameterByName("parameter_mqf_templatedata", workbook:=MQFWorkbook, found:=foundflag, silent:=False)
        If Not foundflag Then
            dataws = headerids.Worksheet
        Else
            dataws = MQFWorkbook.Sheets(datawsname)
        End If
        ' the full sheet
        Dim maxrow As Integer
        Dim MaxCol As Integer
        MaxCol = dataws.Cells(startrow, dataws.Columns.Count).End(Excel.XlDirection.xlToLeft).Column
        maxrow = dataws.Cells(dataws.Rows.Count, 1).End(Excel.XlDirection.xlUp).row
        ' set it
        mqfDBRange = dataws.Range(dataws.Cells(startrow, 1), dataws.Cells(maxrow, MaxCol))

        '**** go through message queue
        '****
        n = 1
        For Each row In mqfDBRange.Rows
            ' only if we are really in the same order
            aMQFRowEntry = MQFObject.getEntry(n)

            If Not IsError(row.Cells(1, MQFObject.Actionordinal).Value) Then
                If LCase(aMQFRowEntry.action) = LCase(row.Cells(1, MQFObject.Actionordinal).Value) Then  'And _
                    'aMQFRowEntry. = row.Cells(1, theMQFFormat.UIDCol).Value Then

                    Globals.ThisAddIn.Application.StatusBar = " postprocessing MQF " & MQFWorkbook.Name & " updating messages and stati for row#" & row.row

                    ' timestamp
                    row.Cells(1, MQFObject.ProcessDateordinal) = Format(MQFObject.processdate, "dd.mm.yyyy hh:mm")
                    ' status code
                    'row.Cells(1, MQFObject.ProcessStatusordinal) = aMQFRowEntry.statuscode
                    ' get it
                    'row.Cells(1, MQFObject.ProcessStatusordinal).Value = aStatus.code
                    'row.Cells(1, MQFObject.ProcessStatusordinal).Interior.Color = aStatus.getCodeColor

                    'If aMQFRowEntry.status Is Nothing Then
                    '    theMessages(n).log = theMessages(n).status.name & "-" & theMessages(n).status.description
                    'End If
                    ' logmsg
                    'row.Cells(1, MQFObject.ProcessLogordinal) = aMQFRowEntry.msglog.getSummary()
                    row.Cells(1, MQFObject.ProcessLogordinal).Font.size = 6
                    row.Cells(1, MQFObject.ProcessLogordinal).Font.Bold = False
                    row.Cells(1, MQFObject.ProcessLogordinal).WrapText = True

                Else
                    System.Diagnostics.Debug.WriteLine("Order of postprocess is not matching: ")
                    postprocessXLSMQF = False
                End If
            End If
            ' increase
            n = n + 1
        Next row

        Value = SetXlsParameterValueByName("doc9_mqf_processedBy", MQFObject.processedByUsername, workbook:=MQFWorkbook)
        Value = SetXlsParameterValueByName("doc9_mqf_processedOn", Format(MQFObject.processdate, "dd.mm.yyyy"), workbook:=MQFWorkbook)
        Value = SetXlsParameterValueByName("doc9_mqf_status", MQFObject.statuscode, workbook:=MQFWorkbook)
        ' autofilter
        dataws.Range(dataws.Cells(headerstartrow + 1, 1), _
                     dataws.Cells(headerstartrow + 1, MaxCol)).AutoFilter()

        ' protect again
        MQFWorkbook.Sheets(MQFWorksheetName).Protect(password:=constPasswordTemplate, _
                                                     DrawingObjects:=False, Contents:=True, Scenarios:=False, _
                                                     AllowFormattingCells:=True, AllowFormattingColumns:=True, _
                                                     AllowFormattingRows:=True, AllowInsertingColumns:=True, AllowInsertingRows:=True, _
                                                     AllowDeletingColumns:=True, AllowDeletingRows:=True, _
                                                     AllowFiltering:=True, AllowUsingPivotTables:=True, AllowSorting:=True)

        ' save it
        Globals.ThisAddIn.Application.StatusBar = " MQF " & MQFWorkbook.Name & " was updated after processing"
        MQFWorkbook.Save()

        'return
        postprocessXLSMQF = True

    End Function


    ' ***************************************************************************************************
    '  Subroutine to Locate and Open a Doc9-Document Message Queue in the Globals.ThisAddin.Application
    '  runs through some tests to ensure that it is really an Doc9-MQ
    '  returns the Workbook

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
        FileNamePattern = GetDBParameter("parameter_mqf_filenamepattern", silent:=True)
        'FileNamePattern2 = getXLSParameterByName("parameter_Doc9filename_search_2")
        'For Each wb In Globals.ThisAddin.Application.Globals.ThisAddin.Application.Workbooks
        ' If (InStr(wb.Name, FileNamePattern) > 0) Or (InStr(wb.Name, FileNamePattern2) > 0) Then
        '     MsgBox "It seems that OnTrack as '" & wb.Name & _
        '     "' is alread opened - please close it and run the procedure again !", Buttons:=vbCritical, Title:="OnTrack Tooling Error"
        '     Exit Sub
        ' End If

        'Next

        'Open Dialog for Doc9 Find
        Value = GetDBParameter("parameter_startfoldernode")
        If Value <> "" And FileIO.FileSystem.FileExists(Value) Then

            If Mid(Value, Len(Value), 1) <> "\" Then Value = Value & "\"

            If Mid(Value, 2, 1) = ":" Then
                ChDrive(Mid(Value, 1, 2))
            End If
            Value = Value & GetDBParameter("parameter_mqf_inputqueue")
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


        LocateAndOpenMQF = MQFWorkbook
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

        MQFSheetname = GetXlsParameterByName("parameter_mqf_templatedata", MQFWORKBOOK, silent:=True)
        If MQFSheetname = "" Then
            MQFSheetname = GetXlsParameterByName("parameter_mqf_templatedata", silent:=True)
        End If
        If MQFSheetname = "" Then
            MQFSheetname = GetDBParameter("parameter_mqf_templatedata", silent:=True)
        End If
        'Check if Worksheet there
        ' Check if Parameters Sheet is still there

        If SheetExistsinWorkbook(MQFWORKBOOK, MQFSheetname) Then
            ws = MQFWORKBOOK.Sheets(MQFSheetname)
        Else
            Call CoreMessageHandler(subname:="checkOnMQF", message:="Workbook '" & MQFWORKBOOK.Name & "' is not a valid Message Queue File", arg1:=MQFWORKBOOK.Name, showmsgbox:=SILENT)

            ' Error
            If SILENT = False Then
                Call CoreMessageHandler(showmsgbox:=True, _
                                 message:="TThe Worksheet '" & MQFSheetname & " ' is not found in the Workbook. Is this a valid Doc9 Message Queue Workbook ? " _
                               , subname:="modMQF.checkWorkbookIfMQF", messagetype:=otCoreMessageType.ApplicationError, break:=False)
            End If

            Return False
        End If

        Return True
    End Function


End Module
