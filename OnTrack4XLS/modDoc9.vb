

REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE TOOLING 4 EXCEL
REM *********** 
REM *********** LEGACY MODULE FUNCTIONS AND CLASSES DOC9 related 
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

Imports OnTrack.UI
Imports OnTrack.Database
Imports OnTrack.Parts
Imports OnTrack.XChange

Class clsBOM

    '***********************************************************************
    '***** CLASS BoM - interim Bill of Documents for Reading from Doc9XLS
    '*****

    Private S_uniqueKey As String

    Public assy As String
    Public precode As String
    Public toplevelname As String

    Public Name As String
    Public partno As String
    Public UID As Long
    Public sno As Long
    Public level As Long
    Public qty As Double

    Private Members() As clsBOM
    Private NoMembers As Integer

    '** initialize
    Private Sub Class_Initialize()
        S_uniqueKey = ""
        NoMembers = 0
    End Sub

    Public Function getNoMembers() As Integer
        getNoMembers = NoMembers

    End Function
    Public Function getMember(ByVal i As Integer) As clsBOM

        If NoMembers > 0 Then
            getMember = Members(i)
        Else
            getMember = Nothing
        End If
    End Function

    Public Function addMember(aBOM As clsBOM, Optional level As Integer = -1) As Boolean
        Dim i As Integer
        Dim flag As Boolean


        ' Add Member if we are not on the same level
        If NoMembers > 0 Then
            i = UBound(Members)    'breitensuche
            If Members(i).level < level And level >= 0 Then
                flag = Members(i).addMember(aBOM, level)
                addMember = flag
                Exit Function
            Else
            'Debug.Print "-"
            End If
        End If

        ' add it at the end
        NoMembers = NoMembers + 1
        ReDim Preserve Members(NoMembers)
        Members(NoMembers) = New clsBOM
        ' copy
            With Members(NoMembers)
            .assy = aBOM.assy
            .level = aBOM.level
            .Name = aBOM.Name
            .partno = aBOM.partno
            .precode = aBOM.precode
            .sno = aBOM.sno
            .UID = aBOM.UID
            .qty = aBOM.qty
        End With
        addMember = True

    End Function

End Class

Module modDoc9

    ' ***************************************************************************************************
    '   Module for doc9 db functions
    '
    '   Author: B.Schneider
    '   created: 2012-07-13
    '
    '   change-log:
    ' ***************************************************************************************************

    Public dbdoc9structure As Range    ' global cache of dbdoc9structure range

    '
    ' Field definitions

    ' ***************************************************************************************************
    '  Function to Get and Set the current Doc9 Database Range
    '
    '
    '  returns the Range as Range or Empty Value if no freeze !


    Function GetdbDoc9Range() As Range

        Dim nullRange As Range

        Dim flag As Boolean
        Dim ws As Worksheet
        Dim namedarea As Name
        Dim namedrange As Range
        Dim startrow As Integer
        Dim parameter_ws As Worksheet

        Dim Value As Object

        Dim dbDoc9Range As Range
        Dim dbdoc9HRange As Range

        ' return cache
        If Not dbdoc9structure Is Nothing Then
            getdbDoc9Range = dbdoc9structure
            Exit Function
        End If

        REM
        REM Run some Checks
        REM

        ' Check if sheet in tool matrix is there
        flag = False
        If Not GlobalDoc9isSet() Then
            Call SetGlobalDoc9(Globals.ThisAddIn.Application.ActiveWorkbook)

        End If
        ourSMBDoc9 = GetGlobalDoc9()
        ' Sheet there ?
        If SheetExistsinWorkbook(ourSMBDoc9, constDoc9StructureSheetName) Then
            ws = ourSMBDoc9.Sheets(constDoc9StructureSheetName)
            Value = modParameterXLS.GetXlsParameterByName("parameter_doc9_database_range")
            flag = True
        Else
            Call CoreMessageHandler(showmsgbox:=True, break:=False, subname:="modDoc9.getDBdDoc9Range", _
                                   message:="Abort: The Worksheet '" & constDoc9StructureSheetName & " ' is not found in the Workbook " & ourSMBDoc9.Name & "!", _
                                   messagetype:=otCoreMessageType.ApplicationError)
            getdbDoc9Range = nullRange
            Exit Function
        End If

        'parametersheet
        ' Sheet there ?
        If SheetExistsinWorkbook(ourSMBDoc9, constParameterSheetName) Then
            parameter_ws = ourSMBDoc9.Sheets(constParameterSheetName)
        Else
            Call CoreMessageHandler(showmsgbox:=True, break:=False, subname:="modDoc9.getDBdDoc9Range", _
                                  message:="Abort: The Worksheet '" & constDoc9StructureSheetName & " ' is not found in the Workbook " & ourSMBDoc9.Name & "!", _
                                  messagetype:=otCoreMessageType.ApplicationError)
            getdbDoc9Range = nullRange
            Exit Function
        End If

        ' search replica database
        flag = False
        ' get startrow
        startrow = modParameterXLS.GetXlsParameterByName("parameter_doc9db_startrow")
        ' the full sheet
        Dim maxrow As Integer
        Dim MaxCol As Integer

        MaxCol = ws.Cells(startrow, ws.Columns.Count).End(Excel.XlDirection.xlToLeft).Column
        maxrow = ws.Cells(ws.Rows.Count, 1).End(Excel.XlDirection.xlUp).row

        ' no header in range
        dbDoc9Range = ws.Range(ws.Cells(startrow, 1), ws.Cells(maxrow, MaxCol))
        dbdoc9HRange = ws.Range(ws.Cells(startrow - 1, 1), ws.Cells(maxrow, MaxCol))

        ' store values
        If (parameter_ws.ProtectContents And Not ourSMBDoc9.MultiUserEditing) _
        Or Not parameter_ws.ProtectContents Then
            ' save parameter
            Value = modParameterXLS.SetXlsParameterValueByName("parameter_doc9_database_name", constdbdoc9structureName)
            Value = modParameterXLS.SetXlsParameterValueByName("parameter_doc9H_database_name", constHdbdoc9structureName)
            Value = modParameterXLS.SetXlsParameterValueByName("parameter_doc9_database_range", dbDoc9Range.Address)
            Value = modParameterXLS.SetXlsParameterValueByName("parameter_doc9H_database_range", dbdoc9HRange.Address)
        End If

        ourSMBDoc9.Names.Add(Name:=constdbdoc9structureName, RefersTo:=dbDoc9Range)
        ourSMBDoc9.Names.Add(Name:=constHdbdoc9structureName, RefersTo:=dbdoc9HRange)

        'If dbDoc9Range.AutoFilter Then
        dbDoc9Range.Worksheet.AutoFilterMode = False
        'End If

        dbdoc9structure = dbDoc9Range
        getdbDoc9Range = dbDoc9Range
    End Function

    '**********
    '********** get Selection in Range
    '**********
    '********** returns Nothing if nothing is selected
    Public Function getSelectionAsRange(Optional silent As Boolean = False, _
    Optional selectionField As String = "x1") As Range
        Dim Value As Object
        Dim dbRange As Range
        Dim selection, selectioncol, selected, selectfield As Range
        Dim uid_column As Long
        Dim msgboxrs As clsCoreUIMessageBox.ResultType

        ' Get Selection
        dbRange = getdbDoc9Range()
        If dbRange Is Nothing Then
            getSelectionAsRange = Nothing
            Exit Function
        End If
        ' Blend in all Columns
        dbRange.EntireColumn.Hidden = False

        ' selection
        uid_column = getXLSHeaderIDColumn(selectionField)
        'Set selectioncol = dbDoc9Range.Worksheet.Range(dbDoc9Range.Cells(1, value), dbDoc9Range.Cells(dbDoc9Range.Rows.count, value))
        ' Any values selection
        'selection.Find what:="*", LookIn:=xlValues
        'Set selection = FindAll(selectioncol, "*", LookIn:=xlValues)

        ' selection is empty
        'If selection Is Nothing Then
        '        value = getXLSHeaderIDColumn("x2")
        '        Set selectioncol = dbDoc9Range.Worksheet.Range(dbDoc9Range.Cells(1, value), dbDoc9Range.Cells(dbDoc9Range.Rows.count, value))
        '        Set selection = selectioncol

        'End If
        '*** selection
        ' selection
        uid_column = getXLSHeaderIDColumn(selectionField)
        selected = Nothing
        selectioncol = dbRange.Worksheet.Range(dbRange.Cells(1, uid_column), dbRange.Cells(dbRange.Rows.Count, uid_column))
        ' selection
        ' select manually the uids
        For Each selectfield In selectioncol.Cells
            If Not IsEmpty(selectfield.Value) Then
                If selected Is Nothing Then
                    selected = selectfield
                Else
                    selected = Globals.ThisAddIn.Application.Union(selected, selectfield)
                End If
            End If
        Next selectfield
        '* nothing selected
        If selected Is Nothing Then
            With New clsCoreUIMessageBox
                .Title = "ARE YOU SURE ?"
                .Message = "ATTENTION !" & vbLf & "No data rows have been selected in the SELECTION Column of the Database. Should ALL rows be written to the Message Queue File ?"
                .buttons = clsCoreUIMessageBox.ButtonType.YesNo
                .Show()
                msgboxrs = .result
            End With


            If msgboxrs <> clsCoreUIMessageBox.ResultType.Yes Then
                getSelectionAsRange = Nothing
                Exit Function
            Else
                selected = selectioncol.Cells
            End If
        End If
        '
        getSelectionAsRange = selected
    End Function

    '********************************************************************************************************************
    '    getXLSHeaderIDColumn is a Helper function to get the Column No by Header ID from the Parameter in 'A' Notation
    '
    ' ParameterName is the Name of the Column Parameter
    ' db is the Range of the Database to reference
    '
    ' return the Column or -1 if there is none
    '
    Function getXLSHeaderIDColumn(ByVal headerid As String) As Integer
        Dim Value As String
        Dim flag As Boolean
        Dim pn As Name
        Dim Prefix As String

        ' Doc9
        If ourSMBDoc9 Is Nothing Then
            ourSMBDoc9 = Globals.ThisAddin.Application.ActiveWorkbook
        End If

        '** search
        Prefix = modParameterXLS.getXLSParameterByName("parameter_doc9_dbdesc_prefix")
        If NameExistsinWorkbook(ourSMBDoc9, Prefix & headerid) Then
            pn = ourSMBDoc9.Names(Prefix & headerid)
            flag = True
        Else
            '** update if not found
            Call updateXlSDBDescTable()
            If NameExistsinWorkbook(ourSMBDoc9, Prefix & headerid) Then
                pn = ourSMBDoc9.Names(Prefix & headerid)
                flag = True
            End If
        End If

        '** errror handling
        If Not flag Then
            Call CoreMessageHandler(SHOWMSGBOX:=True, break:=False, SUBNAME:="modDoc9.getXLSHeaderIDColumn", _
                                message:="The column with header-id " & headerid & " is not found in this workbook.", _
                                messagetype:=otCoreMessageType.ApplicationError)

            getXLSHeaderIDColumn = -1
            Exit Function
        Else
            getXLSHeaderIDColumn = CDec(pn.RefersToRange.Value)
            Exit Function
        End If

    End Function

    '********
    '******** getDBDescIDs() returns the IDs (Header-IDs) as String()
    '********
    '********

    Public Function getDBDescIDs() As Object()
        Dim headerids As Range
        Dim headerids_name As String
        Dim DescTable As Range
        Dim cell As Range
        Dim i As Integer
        Dim startdesccell As Range
        Dim Prefix As String
        Dim idList() As Object


        headerids_name = modParameterXLS.getXLSParameterByName("parameter_doc9_headerid_name")
        headerids = getXLSParameterRangeByName(headerids_name)
        'parameter_doc9_dbdesc_prefix
        Prefix = modParameterXLS.getXLSParameterByName("parameter_doc9_dbdesc_prefix")

        ' error
        If headerids Is Nothing Then
            Call CoreMessageHandler(SHOWMSGBOX:=True, break:=False, SUBNAME:="modDoc9.getDBDescIDs", _
                               message:="The parameter 'parameter_doc9_headerid_name':" & headerids_name & " is not showing a valid range !", _
                               messagetype:=otCoreMessageType.ApplicationError)

            Exit Function
        End If

        DescTable = getXLSParameterRangeByName("parameter_doc9_structure_db_description_table")
        ' error
        If DescTable Is Nothing Then
            Call CoreMessageHandler(SHOWMSGBOX:=True, break:=False, SUBNAME:="modDoc9.getDBDescIDs", _
                              message:="The parameter 'parameter_doc9_structure_db_description_table' is not showing a valid range !", _
                              messagetype:=otCoreMessageType.ApplicationError)
            Exit Function
        End If

        ' upper right corner
        'Set startdesccell = desctable(1, 1)

        ' run through the headerids
        i = 0
        For Each cell In headerids
            If Trim(cell.Value) <> "" Then
                ReDim Preserve idList(i)
                idList(i) = Trim(cell.Value)
                i = i + 1
            End If
        Next cell

        getDBDescIDs = idList
    End Function

    '********
    '******** getDBDesc() returns the xlsDBDesc of the Doc9 Database or search for aName
    '********
    '********

    Public Function getDBDesc(ByRef DBFieldList As xlsDBDesc(), _
    Optional ByVal aName As String = "", _
    Optional aWorkbook As Excel.Workbook = Nothing) As Boolean
        Dim headerids As Range
        Dim headerids_name As String
        Dim DescTable As Range
        Dim row As Range
        Dim i As Integer
        Dim startdesccell As Range
        Dim Prefix As String
        'Dim DBFieldList() As xlsDBDesc

        If IsMissing(aWorkbook) Then
            aWorkbook = GetGlobalDoc9()
        End If

        headerids_name = getXLSParameterByName(NAME:="parameter_doc9_headerid_name", WORKBOOK:=aWorkbook)
        headerids = getXLSParameterRangeByName(headerids_name, WORKBOOK:=aWorkbook)
        'parameter_doc9_dbdesc_prefix
        Prefix = getXLSParameterByName(NAME:="parameter_doc9_dbdesc_prefix", WORKBOOK:=aWorkbook)

        ' error
        If headerids Is Nothing Then
            Call CoreMessageHandler(SHOWMSGBOX:=True, break:=False, SUBNAME:="modDoc9.getDBDesc", _
                              message:="The parameter 'parameter_doc9_headerid_name':" & headerids_name & " is not showing a valid range !", _
                              messagetype:=otCoreMessageType.ApplicationError)

            getDBDesc = False
            Exit Function
        End If

        DescTable = getXLSParameterRangeByName("parameter_doc9_structure_db_description_table", WORKBOOK:=aWorkbook)
        ' error
        If DescTable Is Nothing Then
            Call CoreMessageHandler(SHOWMSGBOX:=True, break:=False, SUBNAME:="modDoc9.getDBDesc", _
                              message:="The parameter 'parameter_doc9_structure_db_description_table' is not showing a valid range !", _
                              messagetype:=otCoreMessageType.ApplicationError)
            getDBDesc = False
            Exit Function
        End If

        ' upper right corner
        'Set startdesccell = desctable(1, 1)

        ' run through the rows
        i = -1
        For Each row In DescTable.Rows
            If Trim(row.Cells(1, xlsDBDescColNo.FieldID + 1)) Like aName Or aName = "" Then
                i = i + 1
                ReDim Preserve DBFieldList(i)

                DBFieldList(i).DBName = "Doc9StructureDB"
                DBFieldList(i).ID = Trim(row.Cells(1, xlsDBDescColNo.FieldID + 1))
                ' Title
                DBFieldList(i).TITLE = Trim(row.Cells(1, xlsDBDescColNo.TITLE + 1))
                ' ColumnNo
                DBFieldList(i).ColumnNo = CInt(row.Cells(1, xlsDBDescColNo.ColumnNo + 1))
                'type
                ' convert it to constants
                Select Case LCase(Trim(row.Cells(1, xlsDBDescColNo.FieldType + 1)))
                    Case "numeric"
                        DBFieldList(i).Fieldtype = xlsDBFieldType.numeric
                    Case "List"
                        DBFieldList(i).Fieldtype = xlsDBFieldType.List
                    Case "text"
                        DBFieldList(i).Fieldtype = xlsDBFieldType.text
                    Case "Runtime"
                        DBFieldList(i).Fieldtype = xlsDBFieldType.runtime
                    Case "formula"
                        DBFieldList(i).Fieldtype = xlsDBFieldType.Formula
                    Case "date"
                        DBFieldList(i).Fieldtype = xlsDBFieldType.datevalue
                    Case "long"
                        DBFieldList(i).Fieldtype = xlsDBFieldType.Longvalue
                    Case "boolean"
                        DBFieldList(i).Fieldtype = xlsDBFieldType.bool
                End Select
                ' Parameter
                DBFieldList(i).PARAMETER = Trim(row.Cells(1, xlsDBDescColNo.PARAMETER + 1))
                ' OTDB Relation
                DBFieldList(i).OTDBRelation = Trim(row.Cells(1, xlsDBDescColNo.OTDBRelation + 1))
                ' Primary Keys Relation
                DBFieldList(i).OTDBPrimaryKeys = Trim(row.Cells(1, xlsDBDescColNo.OtdbPrimaryKey + 1))
            End If
        Next row

        If i >= 0 Then
            getDBDesc = True
        Else
            getDBDesc = False
        End If

    End Function

    '********
    '******** update the doc9DB Structure Description Table
    '********
    '********

    Public Sub updateXlSDBDescTable(Optional aWorkbook As Excel.Workbook = Nothing)
        Dim headerids As Range
        Dim headerids_name As String
        Dim DescTable As Range
        Dim cell As Range
        Dim i, j As Integer
        Dim startdesccell As Range
        Dim Prefix As String
        Dim protect_flag As Boolean
        Dim pn As Name
        Dim fields() As xlsDBDesc
        Dim found As Boolean
        Dim Value As Object

        '**
        headerids_name = getXLSParameterByName("parameter_doc9_headerid_name")
        headerids = getXLSParameterRangeByName(headerids_name)
        'parameter_doc9_dbdesc_prefix
        Prefix = getXLSParameterByName("parameter_doc9_dbdesc_prefix")

        ' error
        If headerids Is Nothing Then
            Call CoreMessageHandler(SHOWMSGBOX:=True, break:=False, SUBNAME:="modDoc9.updateXLSDBDescTable", _
                              message:="The parameter 'parameter_doc9_headerid_name':" & headerids_name & " is not showing a valid range !", _
                              messagetype:=otCoreMessageType.ApplicationError)
            Exit Sub
        End If

        DescTable = getXLSParameterRangeByName("parameter_doc9_structure_db_description_table")
        ' error
        If DescTable Is Nothing Then
            Call CoreMessageHandler(SHOWMSGBOX:=True, break:=False, SUBNAME:="modDoc9.updateXLSDBDescTable", _
                              message:="The parameter 'parameter_doc9_structure_db_description_table' is not showing a valid range !", _
                              messagetype:=otCoreMessageType.ApplicationError)
            Exit Sub
        End If

        ' save DBFieldList
        If Not getDBDesc(fields) Then
            'Debug.Print "could not read field definitions while update DBDescTable"
        End If

        ' upper right corner
        startdesccell = DescTable(1, 1)

        'protect
        If startdesccell.Worksheet.ProtectContents Then
            startdesccell.Worksheet.Unprotect(Password:=constPasswordParameters)
            protect_flag = True
        End If

        'Public Const xlsDBDescColNo.FieldID = 0
        'Public Const xlsDBDescColNo.Title = 1
        'Public Const xlsDBDescColNo.columnNo = 2
        'Public Const xlsDBDescColNo.FieldType = 3
        'Public Const xlsDBDescColNo.Parameter = 4

        ' run through the headerids
        i = 0
        For Each cell In headerids
            ' existing row in DBFieldList ? set to j
            'If Not Fields() Is Nothing Then
            found = False
            ' search it
            For j = 0 To UBound(fields)
                If fields(j).ID = cell.Value Then
                    found = True
                    Exit For
                End If
            Next j
            'End If
            ' insert new row at end of table
            If i >= DescTable.Rows.count Then
                startdesccell.offset(i, 0).EntireRow.Insert()
            End If
            ' Header ID
            startdesccell.offset(i, xlsDBDescColNo.FieldID).Value = cell.Value
            If Trim(cell.Value) = "" Then
                startdesccell.offset(i, 0).Interior.Color = constErrorBackground
            Else
                startdesccell.offset(i, 0).Interior.Color = startdesccell.offset(0, 0).Interior.Color
            End If
            ' Description
            startdesccell.offset(i, xlsDBDescColNo.TITLE).Value = cell.offset(2, 0).Value
            ' Column
            startdesccell.offset(i, xlsDBDescColNo.ColumnNo).Value = i + 1
            ' name
            If NameExistsinWorkbook(startdesccell.Worksheet.Parent, Prefix & cell.Value) Then
                pn = DescTable.Worksheet.Parent.Names(Prefix & cell.Value)
                pn.Delete()
            End If
            DescTable.Worksheet.Parent.Names.add( _
            Name:=Prefix & cell.Value, RefersTo:=startdesccell.offset(i, 2))
            ' restore saved values
            If found Then
                ' convert it to constants
                Select Case fields(j).Fieldtype
                    Case xlsDBFieldType.numeric
                        startdesccell.Offset(i, xlsDBDescColNo.FieldType).Value = "numeric"
                    Case xlsDBFieldType.List
                        startdesccell.Offset(i, xlsDBDescColNo.FieldType).Value = "List"
                    Case xlsDBFieldType.text
                        startdesccell.Offset(i, xlsDBDescColNo.FieldType).Value = "text"
                    Case xlsDBFieldType.runtime
                        startdesccell.offset(i, xlsDBDescColNo.FieldType).Value = "runtime"
                    Case xlsDBFieldType.Formula
                        startdesccell.offset(i, xlsDBDescColNo.FieldType).Value = "formula"
                    Case xlsDBFieldType.datevalue
                        startdesccell.offset(i, xlsDBDescColNo.FieldType).Value = "date"
                    Case xlsDBFieldType.Longvalue
                        startdesccell.offset(i, xlsDBDescColNo.FieldType).Value = "long"

                End Select

                ' Parameter
                startdesccell.offset(i, xlsDBDescColNo.PARAMETER).Value = fields(j).PARAMETER
                ' OTDB Relation
                startdesccell.offset(i, xlsDBDescColNo.OTDBRelation).Value = fields(j).OTDBRelation
                ' OTDB Primary Relation
                startdesccell.offset(i, xlsDBDescColNo.OtdbPrimaryKey).Value = fields(j).OTDBPrimaryKeys
            End If
            ' inc
            i = i + 1
        Next cell

        'protect
        If protect_flag Then
            startdesccell.Worksheet.Protect(Password:=constPasswordParameters)
        End If

        DescTable = startdesccell.Worksheet.Range(startdesccell, startdesccell.offset(i - 1, 2))
        'If Not setParameterValueByName("parameter_doc9_dbdesc_range", DescTable.Address) Then Debug.Print "parameter_doc9_dbdesc_range doesnot exist ?!"
        ' delete
        If NameExistsinWorkbook(startdesccell.Worksheet.Parent, "parameter_doc9_structure_db_description_table") Then
            pn = DescTable.Worksheet.Parent.Names("parameter_doc9_structure_db_description_table")
            pn.Delete()
        End If
        DescTable.Worksheet.Parent.Names.add( _
        Name:="parameter_doc9_structure_db_description_table", RefersTo:=DescTable)


        Value = getXLSParameterByName("parameter_doc9_StructureDatabaseVersion")
        If IsNumeric(Value) Then
            Value = Value + 1
            Call setXLSParameterValueByName("parameter_doc9_StructureDatabaseVersion", Value)
        End If

        Globals.ThisAddin.Application.StatusBar = " Doc9 Structure Database Description in version " & Value & " updated"

        '* OTDB update
        'If updateOTDBSchema(True) Then
        'Value = getDBParameter("parameter_doc9_StructureDatabaseVersion", silent:=True)
        'Globals.ThisAddin.Application.StatusBar = " OnTrack Database version " & Value & " meta structure updated"
        'End If
    End Sub

    Public Sub copyDoc9Format(source As Object, TARGET As Range, Optional changeformat As Boolean = False)
        ' interior
        If Not changeformat Then
            TARGET.Interior.Color = source.Interior.Color
        End If
        ' font
        With TARGET.Font
            .FontStyle = source.Font.FontStyle
            .Bold = source.Font.Bold
            .Italic = source.Font.Italic
            .Color = source.Font.Color
            .size = source.Font.size
            .Strikethrough = source.Font.Strikethrough
            .Subscript = source.Font.Subscript
            .Superscript = source.Font.Superscript
        End With
        '** Borders
        With TARGET.Borders(XlBordersIndex.xlEdgeRight)
            .Weight = source.Borders(XlBordersIndex.xlEdgeRight).weight
            .LineStyle = source.Borders(XlBordersIndex.xlEdgeRight).LineStyle
            .Color = source.Borders(XlBordersIndex.xlEdgeRight).Color
        End With
        With TARGET.Borders(XlBordersIndex.xlEdgeLeft)
            .Weight = source.Borders(XlBordersIndex.xlEdgeLeft).weight
            .LineStyle = source.Borders(XlBordersIndex.xlEdgeLeft).LineStyle
            .Color = source.Borders(XlBordersIndex.xlEdgeLeft).Color
        End With
        With TARGET.Borders(XlBordersIndex.xlEdgeTop)
            .Weight = source.Borders(XlBordersIndex.xlEdgeTop).weight
            .LineStyle = source.Borders(XlBordersIndex.xlEdgeTop).LineStyle
            .Color = source.Borders(XlBordersIndex.xlEdgeTop).Color
        End With
        With TARGET.Borders(XlBordersIndex.xlEdgeBottom)
            .Weight = source.Borders(XlBordersIndex.xlEdgeBottom).weight
            .LineStyle = source.Borders(XlBordersIndex.xlEdgeBottom).LineStyle
            .Color = source.Borders(XlBordersIndex.xlEdgeBottom).Color
        End With

    End Sub

    '***** createDoc9 Config 
    '*****                      Creates a special XConfig (Dynmaic) for the Doc9 by Hand and saves it
    Public Function createDoc9XConfig(Optional ByVal XCMD As otXChangeCommandType = otXChangeCommandType.Read) As Boolean
        Dim aXChangeConfig As New clsOTDBXChangeConfig

        If Not aXChangeConfig.create(MySettings.Default.DefaultDoc9ConfigNameDynamic) Then
            aXChangeConfig.Inject(MySettings.Default.DefaultDoc9ConfigNameDynamic)
            aXChangeConfig.delete()

        End If

        Call aXChangeConfig.addObjectByName("tblschedules", XCMD:=XCMD)
        Call aXChangeConfig.addObjectByName("tbldeliverabletargets", XCMD:=XCMD)
        Call aXChangeConfig.addObjectByName("tbldeliverabletracks", XCMD:=XCMD)
        Call aXChangeConfig.addObjectByName("tbldeliverables", XCMD:=XCMD)
        Call aXChangeConfig.addObjectByName("tblparts", XCMD:=XCMD)
        Call aXChangeConfig.AddObjectByName("tblconfigs", XCMD:=XCMD)
        Call aXChangeConfig.AddObjectByName("ctblDeliverableObeyas", XCMD:=XCMD)
        Call aXChangeConfig.AddObjectByName("ctblDeliverableExpeditingStatus", XCMD:=XCMD)
        Call aXChangeConfig.AddObjectByName("tblDeliverableWorkstationCodes", XCMD:=XCMD)
        Call aXChangeConfig.AddObjectByName("tblxoutlineitems", XCMD:=XCMD)

        aXChangeConfig.AllowDynamicAttributes = True

        createDoc9XConfig = aXChangeConfig.Persist()
    End Function

    '***** createDoc9 Config 
    '*****                      Creates a special XConfig (Dynmaic) for the Doc9 by Hand and saves it
    Public Function createExpediterXConfig(Optional ByVal XCMD As otXChangeCommandType = otXChangeCommandType.Read) As Boolean
        Dim aXChangeConfig As New clsOTDBXChangeConfig

        If Not aXChangeConfig.Create(MySettings.Default.DefaultExpediterConfigNameDynamic) Then
            aXChangeConfig.Inject(MySettings.Default.DefaultExpediterConfigNameDynamic)
            aXChangeConfig.Delete()

        End If

        Call aXChangeConfig.AddObjectByName("ctblDeliverableExpeditingStatus", XCMD:=XCMD)

        aXChangeConfig.AllowDynamicAttributes = True

        Return aXChangeConfig.Persist()
    End Function
    Private GlobalDoc9XChangeConfig As clsOTDBXChangeConfig

    '***** getXlsDoc9XConfig: Returns the used Doc9/18 XChangeConfiguration
    '*****
    Public Function getXlsDoc9Xconfig(Optional ByVal XCMD As otXChangeCommandType = 0) As clsOTDBXChangeConfig
        Dim headeridRange As Range
        Dim headerids As Object
        Dim IDs() As String
        Dim dataArray As Object
        Dim dataRange As Range
        Dim i As Integer
        Dim isReadOnly As Boolean
        Dim j As Integer
        Dim aXChangeConfig As New clsOTDBXChangeConfig

        ' do it only we we donot have to reset
        If Not GlobalDoc9XChangeConfig Is Nothing And XCMD = 0 Then
            If GlobalDoc9XChangeConfig.IsCreated Or GlobalDoc9XChangeConfig.IsLoaded Then
                getXlsDoc9Xconfig = GlobalDoc9XChangeConfig
                Exit Function
            End If
        End If

        '****
        '**** create the Config for this Doc9

        headeridRange = getXLSParameterRangeByName( _
        NAME:="doc9_headerid", WORKBOOK:=Globals.ThisAddIn.Application.ActiveWorkbook)

        dataRange = getXLSParameterRangeByName( _
        NAME:="dbdoc9structure", WORKBOOK:=Globals.ThisAddIn.Application.ActiveWorkbook)


        '**
        Call aXChangeConfig.create("Doc18Config")
        Call aXChangeConfig.addObjectByName("tblschedules", XCMD:=XCMD)
        Call aXChangeConfig.addObjectByName("tbldeliverabletargets", XCMD:=XCMD)
        Call aXChangeConfig.addObjectByName("tbldeliverabletracks", XCMD:=XCMD)
        Call aXChangeConfig.addObjectByName("tbldeliverables", XCMD:=XCMD)
        Call aXChangeConfig.AddObjectByName("tblparts", XCMD:=XCMD)
        Call aXChangeConfig.AddObjectByName("tblconfigs", XCMD:=XCMD)
        Call aXChangeConfig.AddObjectByName("tbldeliverableworkstationcodes", XCMD:=XCMD)
        Call aXChangeConfig.AddObjectByName("ctbldeliverableobeyas", XCMD:=XCMD)
        isReadOnly = False

        If Not headeridRange Is Nothing And Not dataRange Is Nothing Then
            ' convert to single array
            headerids = headeridRange.Value
            'ReDim ids(LBound(headerids, 2) To UBound(headerids, 2))
            j = 0
            For i = LBound(headerids, 2) To UBound(headerids, 2)
                If LCase(headerids(1, i)) <> "" Then
                    j = j + 1
                    ReDim Preserve IDs(0 To j)
                    IDs(j) = headerids(1, i)
                    'Debug.Print(IDs(j), i)

                    If LCase(headerids(1, i)) = "uid" Then
                        isReadOnly = True
                    Else
                        isReadOnly = False
                    End If

                    'If isReadOnly Then Debug.Assert False

                    Call aXChangeConfig.AddAttributeByID(id:=IDs(j), ordinal:=i, _
                                                         xcmd:=XCMD, readonly:=isReadOnly)

                End If
            Next i

        End If

        '**



        GlobalDoc9XChangeConfig = aXChangeConfig
        getXlsDoc9Xconfig = GlobalDoc9XChangeConfig

    End Function

    '********
    '******** updateRowXlsDoc9: updates a line per uid by with a array of columns and corresponding values in
    '********                INPUTMAPPING

    Public Function updateRowXlsDoc9(ByRef INPUTMAPPING As Dictionary(Of Object, Object), _
    ByRef INPUTXCHANGECONFIG As clsOTDBXChangeConfig, _
    Optional ByVal workspaceID As String = "") _
    As Boolean


        'Dim aProgressBar As New clsUIProgressBarForm
        Dim aXChangeConfig As New clsOTDBXChangeConfig
        Dim aMQFXChangeMember As New clsOTDBXChangeMember
        Dim aXChangeMember As New clsOTDBXChangeMember
        Dim m As Object

        Dim doc9DB As Range
        Dim UIDCol, col As Long
        Dim UID As Long
        Dim row As Long
        Dim maxrow As Long
        Dim searchcell As Range
        Dim searcharea As Range
        Dim found As Range
        Dim findrange As Range
        Dim offset As Integer
        Dim i As Integer
        Dim aVAlue As Object
        Dim aNewValue As Object
        Dim valuechanged_flag As Boolean
        Dim rowvaluechanged_flag As Boolean
        Dim formatwarning, formatChange As Range
        'Dim aMsgLog As clsStatusMsg


        aXChangeConfig = getXlsDoc9Xconfig()
        If aXChangeConfig Is Nothing Then
            updateRowXlsDoc9 = False
            Exit Function
        End If


        ' Get Database
        doc9DB = getdbDoc9Range()
        If doc9DB Is Nothing Then
            'Debug.Print "Fatal:Doc9DB could not be set ?!"
            updateRowXlsDoc9 = False
            Exit Function
        End If

        Globals.ThisAddIn.Application.EnableEvents = False

        ' Get Parameters
        UIDCol = getXLSHeaderIDColumn(constDoc9DB_UID)
        If UIDCol = -1 Then
            'Debug.Print "Fatal:UID Column is in DOC9 missing ?!"
            updateRowXlsDoc9 = False
            Exit Function
        End If

        aVAlue = INPUTXCHANGECONFIG.getMemberValue("uid", MAPPING:=INPUTMAPPING)
        If IsNumeric(aVAlue) Then
            UID = CLng(aVAlue)
        Else
            'Debug.Assert False
        End If


        formatwarning = getXLSParameterRangeByName("parameter_doc9_format_warning")
        formatChange = getXLSParameterRangeByName("parameter_doc9_format_change")
        'aMsgLog = New clsStatusMsg
        '
        offset = getXLSParameterByName("parameter_doc9db_startrow")
        ' Search
        maxrow = doc9DB.Rows.count
        searchcell = doc9DB.Cells(1, UIDCol)
        searcharea = doc9DB.Worksheet.Range(searchcell, doc9DB.Worksheet.Cells(maxrow, UIDCol))
        found = FindAll(searcharea, UID, Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlWhole, XlSearchOrder.xlByColumns, False)

        ' go through all found rows
        If Not found Is Nothing Then
            For Each findrange In found
                ' row no
                row = findrange.row - offset + 1  'Cells (1,1) is the one
                UID = findrange.Cells(1, UIDCol).Value

                rowvaluechanged_flag = False

                For Each m In INPUTXCHANGECONFIG.Attributes
                    aMQFXChangeMember = m
                    aXChangeMember = aXChangeConfig.AttributeByID(ID:=aMQFXChangeMember.ID, objectname:=aMQFXChangeMember.Objectname)

                    If Not aXChangeMember Is Nothing Then
                        If aXChangeMember.IsCreated Or aXChangeMember.IsLoaded And aXChangeMember.ISXCHANGED Then
                            ' current value of cell
                            col = aXChangeMember.ordinal.Value
                            aVAlue = findrange.Cells(1, col).Value
                            If INPUTMAPPING.ContainsKey(aMQFXChangeMember.ordinal.Value) Then
                                aNewValue = INPUTMAPPING.Item(aMQFXChangeMember.ordinal.Value)

                                If Not IsNull(aNewValue) And Not IsEmpty(aNewValue) And aNewValue <> aVAlue And aNewValue <> "" Then
                                    rowvaluechanged_flag = True
                                    '** convert
                                    If (aXChangeMember.ObjectEntryDefinition.Datatype = otFieldDataType.[Date] Or _
                                    aXChangeMember.ObjectEntryDefinition.Datatype = otFieldDataType.Timestamp) And _
                                    IsDate(aNewValue) Then
                                        findrange.Cells(1, col).value = CDate(aNewValue)
                                    ElseIf aXChangeMember.ObjectEntryDefinition.Datatype = otFieldDataType.[Long] And _
                                    IsNumeric(aNewValue) Then
                                        findrange.Cells(1, col).value = CLng(aNewValue)
                                    ElseIf aXChangeMember.ObjectEntryDefinition.Datatype = otFieldDataType.Numeric And _
                                    IsNumeric(aNewValue) Then
                                        findrange.Cells(1, col).value = CDbl(aNewValue)
                                    ElseIf IsEmpty(aNewValue) Then
                                        findrange.Cells(1, col).value = "-"
                                    Else
                                        findrange.Cells(1, col).value = CStr(aNewValue)
                                    End If
                                    'findrange.Cells(1, col).Value = aNewValue
                                    '* format
                                    Call copyDoc9Format(formatChange, findrange.Cells(1, col), True)
                                End If
                            End If
                        End If
                    End If

                    If rowvaluechanged_flag Then
                        Globals.ThisAddIn.Application.StatusBar = "Update Doc#9 Database Row#" & row
                    End If
                Next m
            Next findrange
        End If

        Globals.ThisAddIn.Application.EnableEvents = True
        updateRowXlsDoc9 = True
    End Function

    '**********  ReplicateWithOTDB: Replicate Doc9 Data to OTDB
    '**********

    Public Function ReplicateWithOTDB(Optional ByVal fullReplication As Boolean = False, _
    Optional ByVal selection As Range = Nothing, _
    Optional ByVal silent As Boolean = True, _
    Optional ByVal workspaceID As String = "") As Boolean
        Dim Value As Object
        Dim dbDoc9Range As Range
        Dim selectioncol As Range
        Dim msgboxrsl As Object
        Dim dbdesc() As xlsDBDesc
        Dim fieldname As String
        Dim i As Integer
        Dim headerids As Range
        Dim headerids_name As String
        Dim row As Range
        Dim aRange As Range
        Dim j As Integer
        Dim MaxCol As Integer
        Dim startdatarow As Integer
        Dim Prefix As String

        Dim UIDCol As Integer
        Dim UID As Long
        Dim pn As String
        Dim acell As Range
        Dim n As Integer
        Dim addresses As String
        Dim TimeStampCol As Integer
        Dim ChangeCol, updcCol, logCol As Integer


        Dim UIDColRange As Range

        Dim doc9Values() As Object
        'Dim aFieldMsglog As New clsStatusMsg
        'Dim aRowMsglog As New clsStatusMsg
        Dim aTimestamp As Date

        Dim otdbcn As Object 'ADODB.Connection
        Dim parts_i, docs_i, doctargets_i, wbs_i, schedule_i, docschedule_i, _
        cartypes_i, tasks_i, mqfprocess_i, curschedule_i As Integer

        ' cartypes
        Dim NoCartypes As Integer
        Dim CT_col() As Integer
        Dim CT() As Integer

        ' Bill of Drawings
        Dim BODTopNodes() As clsBOM
        Dim BODTopLevels() As Integer
        Dim BODTopNames() As String
        Dim Level_Col As Integer
        Dim level As Integer
        Dim NoNodes As Integer    ' Number Of Nodes
        Dim NodeI As Integer  ' current Node Index

        Dim Sno_Col As Integer
        Dim Precode_Col As Integer
        Dim sno As Long
        Dim precode As String
        Dim partno_col As Integer
        Dim name_col As Integer
        Dim partno As String
        Dim Name As String

        Dim found As Boolean
        Dim build_structure As Boolean
        Dim newBodMember As clsBOM

        'Dim aProgressBar As New clsUIProgressBarForm
        Dim aXChangeConfig As New clsOTDBXChangeConfig
        Dim aXChangeMember As New clsOTDBXChangeMember
        Dim m As Object
        Dim aMapping As New Dictionary(Of Object, Object)
        Dim col As Integer
        Dim aVAlue As Object
        Dim flag As Boolean
        Dim aNewValue As Object
        Dim formatChange, formatwarning As Range
        Dim rowvaluechanged_flag As Boolean

        ' get config
        aXChangeConfig = getXlsDoc9Xconfig(XCMD:=otXChangeCommandType.Read)
        If aXChangeConfig Is Nothing Then
            ReplicateWithOTDB = False
            Exit Function
        End If

        ' Get Selection
        dbDoc9Range = getdbDoc9Range()
        If dbDoc9Range Is Nothing Then
            ReplicateWithOTDB = False
            Exit Function
        End If

        ' Blend in all Columns
        dbDoc9Range.EntireColumn.Hidden = False
        ' field columns
        UIDCol = getXLSHeaderIDColumn("uid")
        UIDColRange = dbDoc9Range.Worksheet.Range(dbDoc9Range.Worksheet.Cells(dbDoc9Range.Rows(1).row, UIDCol), _
                                                  dbDoc9Range.Worksheet.Cells(dbDoc9Range.Rows.Count, UIDCol))

        ' Full Replication
        If fullReplication Then
            selection = UIDColRange
            build_structure = True
        End If

        ' selection
        If IsMissing(selection) Or selection Is Nothing Then
            selection = getSelectionAsRange(selectionField:="x1", silent:=silent)
        End If
        ' selection is empty
        If selection Is Nothing Then
            ReplicateWithOTDB = False
            Exit Function
        End If

        '**** init
        'Call aProgressBar.initialize(selection.Rows.Count, "updating Excel sheet ....")
        'Call aProgressBar.showForm()

        '****** parameters
        ' get the Database Description
        If Not getDBDesc(dbdesc) Then
            ReplicateWithOTDB = False
            Exit Function
        Else
            ' dimension to columnno
            ReDim doc9Values(0 To (UBound(dbdesc)))
        End If

        TimeStampCol = getXLSHeaderIDColumn("x4")
        ChangeCol = getXLSHeaderIDColumn("x2")
        updcCol = getXLSHeaderIDColumn("bs3")
        logCol = getXLSHeaderIDColumn("a3")
        aTimestamp = Now
        Precode_Col = getXLSHeaderIDColumn("c3")
        Sno_Col = getXLSHeaderIDColumn("c1")
        Level_Col = getXLSHeaderIDColumn("c2")
        name_col = getXLSHeaderIDColumn("c6")
        partno_col = getXLSHeaderIDColumn("c10")
        'parameter_doc9_dbdesc_prefix
        ' cartypes
        NoCartypes = 26
        ReDim Preserve CT_col(NoCartypes)
        ReDim Preserve CT(NoCartypes)
        For i = 1 To NoCartypes
            CT_col(i) = getXLSHeaderIDColumn("ct" & i)
        Next i

        '**
        '** Build Header
        '**
        Value = getXLSParameterByName("parameter_doc9db_startrow")
        startdatarow = CInt(Value)
        headerids_name = getXLSParameterByName("parameter_doc9_headerid_name")
        headerids = getXLSParameterRangeByName(headerids_name)
        'parameter_doc9_dbdesc_prefix
        Prefix = getXLSParameterByName("parameter_doc9_dbdesc_prefix")
        formatwarning = getXLSParameterRangeByName("parameter_doc9_format_warning")
        formatChange = getXLSParameterRangeByName("parameter_doc9_format_change")

        ' error
        If headerids Is Nothing Then
            If Not silent Then
                Call CoreMessageHandler(SHOWMSGBOX:=True, break:=False, SUBNAME:="modDoc9.replicateWITHOTDB", _
                             message:="The parameter 'parameter_doc9_headerid_name':" & headerids_name & " is not showing a valid range !", _
                             messagetype:=otCoreMessageType.ApplicationError)
            End If

            ReplicateWithOTDB = False
            Exit Function
        End If

        Globals.ThisAddIn.Application.ScreenUpdating = False
        Globals.ThisAddIn.Application.EnableEvents = False

        '**** Build the Xref for the Tables we are updating

        ' start data row
        MaxCol = i
        i = 0

        '*** run through each row
        '*** in selection
        For Each acell In selection.Rows
            row = acell.EntireRow
            '** progress
            'Call aProgressBar.progress(1, "updating row #" & row.Row)
            ' get certain values
            If Not IsNumeric(row.Cells(1, UIDCol).Value) Then
                'Debug.Print "UID in row#" & row.row & " of value : " & row.Cells(1, UIDCol).Value & " is not numeric"
                ReplicateWithOTDB = False
                UID = -1
            Else
                UID = CLng(row.Cells(1, UIDCol).Value)
            End If

            '***
            '*** Exchange with OnTrack
            '***
            '**** INPUT
            aMapping = New Dictionary(Of Object, Object)
            ' put the UID to read on
            Call aMapping.Add(key:=UIDCol, value:=UID)

            If (1 <> 1) Then
                For Each m In aXChangeConfig.Attributes
                    aXChangeMember = m
                    If Not aXChangeMember Is Nothing Then
                        If (aXChangeMember.IsCreated Or aXChangeMember.IsLoaded) And aXChangeMember.ISXCHANGED Then
                            ' current value of cell
                            col = aXChangeMember.ordinal.Value
                            aVAlue = row.Cells(1, col).Value
                            If aMapping.ContainsKey(aXChangeMember.ordinal.Value) Then
                                Call aMapping.Remove(key:=aXChangeMember.ordinal.Value)
                            End If
                            Call aMapping.Add(key:=aXChangeMember.ordinal.Value, value:=aVAlue)
                        End If
                    End If
                Next m
            End If

            '*** run XCHANGE
            flag = aXChangeConfig.runXChange(aMapping)
            '*** OUTPUT
            For Each m In aXChangeConfig.Attributes
                aXChangeMember = m
                If Not aXChangeMember Is Nothing Then
                    aNewValue = aMapping.Item(aXChangeMember.ordinal.Value)
                    If (aXChangeMember.IsCreated Or aXChangeMember.IsLoaded) _
                    And aXChangeMember.ISXCHANGED And Not aXChangeMember.isReadOnly Then
                        ' current value of cell
                        col = aXChangeMember.ordinal.Value
                        aVAlue = row.Cells(1, col).Value

                        If aMapping.ContainsKey(aXChangeMember.ordinal.Value) Then

                            'If aXChangeMember.ordinal.value = 55 Then Debug.Assert False
                            If row.Cells(1, col).HasFormula Or _
                            (Not IsNull(aNewValue) And CStr(aNewValue) <> CStr(aVAlue)) Then
                                rowvaluechanged_flag = True
                                '* change dependent on type
                                If (aXChangeMember.ObjectEntryDefinition.Datatype = otFieldDataType.[Date] Or _
                                aXChangeMember.ObjectEntryDefinition.Datatype = otFieldDataType.Timestamp) And _
                                IsDate(aNewValue) Then
                                    row.Cells(1, col).value = CDate(aNewValue)
                                ElseIf aXChangeMember.ObjectEntryDefinition.Datatype = otFieldDataType.[Long] And _
                                IsNumeric(aNewValue) Then
                                    row.Cells(1, col).value = CLng(aNewValue)
                                ElseIf aXChangeMember.ObjectEntryDefinition.Datatype = otFieldDataType.Numeric And _
                                IsNumeric(aNewValue) Then
                                    row.Cells(1, col).value = CDbl(aNewValue)
                                ElseIf IsEmpty(aNewValue) Then
                                    row.Cells(1, col).value = "-"
                                Else
                                    row.Cells(1, col).value = CStr(aNewValue)
                                End If


                                'row.Cells(1, col).Value = aNewValue
                                '* format
                                Call copyDoc9Format(formatChange, row.Cells(1, col), True)
                            End If
                        End If
                    End If
                End If

                If rowvaluechanged_flag Then
                    Globals.ThisAddIn.Application.StatusBar = "Update Doc#9 Database Row#" & row.Row
                End If
            Next m
            '***
            '*** BOM
            '***

            ' For each Cartype usage build the Topnode
            If build_structure = True And level >= 0 Then
                For j = 1 To NoCartypes
                    ' get the TopLevel Nodes if used
                    If CT(j) > 0 Then
                        found = False
                        For NodeI = 1 To NoNodes
                            If BODTopNames(NodeI) = "H" & j Then
                                found = True
                                Exit For
                            End If
                        Next NodeI
                        ' extend the top nodes
                        If Not found Then
                            NoNodes = NoNodes + 1
                            NodeI = NoNodes
                            ReDim Preserve BODTopNodes(NoNodes)
                            ReDim Preserve BODTopNames(NoNodes)
                            ReDim Preserve BODTopLevels(NoNodes)
                            ' initialize
                            BODTopNames(NodeI) = "H" & j
                            BODTopLevels(NodeI) = -1    ' special level -1
                            ' create Artificial Top Item -> skipped
                            BODTopNodes(NodeI) = New clsBOM
                            With BODTopNodes(NodeI)
                                .UID = -1
                                .sno = -1
                                .level = -1
                                .precode = "H" & j
                                .partno = ""
                                .toplevelname = BODTopNames(NodeI)
                                .Name = "H" & j
                                .qty = 0
                            End With
                        End If


                        ' add this drawing to the BOD Structure
                        newBodMember = New clsBOM
                        With newBodMember
                            .UID = UID
                            .sno = sno
                            .level = level
                            .precode = precode
                            .toplevelname = BODTopNames(NodeI)
                            .partno = partno
                            .Name = Name
                            .qty = CT(j)
                        End With
                        ' add level max
                        If level >= 0 Then
                            BODTopLevels(NodeI) = level
                        End If

                        If BODTopNodes(NodeI) Is Nothing Then
                            BODTopNodes(NodeI) = newBodMember
                        Else
                            'Debug.Assert uid Mod 500
                            If Not BODTopNodes(NodeI).addMember(newBodMember, level) Then
                                ' error
                            End If
                        End If
                    End If    ' entry in cartype
                Next j    ' Cartypes
            End If

            'Update

            Globals.ThisAddIn.Application.StatusBar = " Updating OTDB on UID " & UID & " in row #" & row.Row


            row.Cells(1, ChangeCol).Value = ""
            row.Cells(1, TimeStampCol).Value = Format(aTimestamp, "dd.mm.yyyy hh:mm:ss")

        Next acell

        '*** close
        'Call aProgressBar.closeForm()

        '***
        '*** save the structure recursivley
        If build_structure = True Then
            For NodeI = 1 To NoNodes
                ' all members
                For i = 1 To BODTopNodes(NodeI).getNoMembers
                    newBodMember = BODTopNodes(NodeI).getMember(i)
                    If Not saveBOMStructure(newBodMember) Then
                    End If
                Next i
            Next NodeI
        End If

        '***
        '*** save parameters
        '***
        Globals.ThisAddIn.Application.EnableEvents = True
        ReplicateWithOTDB = True

    End Function
    '***recursive save of the BOM structure from XLS to OTDB
    '***
    Public Function saveBOMStructure(aBOD As clsBOM) As Boolean
        Dim i As Integer
        Dim aMember As clsBOM
        Dim aOTDBBOM As New clsOTDBBOM

        ' save if we have members
        ' all members
        If aBOD.getNoMembers = 0 Then
            saveBOMStructure = False
            Exit Function
        Else
            ' get this BOM -> Delete
            If aOTDBBOM.Inject(aBOD.partno) Then
                Call aOTDBBOM.delete()    ' delete the BOM ?!
            Else
                Call aOTDBBOM.create(pnid:=aBOD.partno)
            End If
            '
            Globals.ThisAddIn.Application.StatusBar = " building / updating BOM of " & aBOD.partno & " at level " & aBOD.level
            ' step down for each member
            For i = 1 To aBOD.getNoMembers
                aMember = aBOD.getMember(i)
                ' add new
                Call aOTDBBOM.addPartID(aPNID:=aMember.partno, aQty:=aMember.qty)
                ' further
                If aMember.getNoMembers > 0 Then
                    Call saveBOMStructure(aMember)
                End If
            Next i

            ' persist
            Call aOTDBBOM.persist()
        End If
    End Function
    '********
    '******** updateDoc9LineFromOTDB: updates a line per uid by with a array of columns and corresponding values in
    '********                INPUTMAPPING

    Public Function updateDoc9LineFromOTDB(ByRef INPUTMAPPING As Dictionary(Of Object, Object), _
    ByRef INPUTXCHANGECONFIG As clsOTDBXChangeConfig, _
    Optional ByVal workspaceID As String = "") _
    As Boolean


        'Dim aProgressBar As New clsUIProgressBarForm
        Dim aXChangeConfig As New clsOTDBXChangeConfig
        Dim aMQFXChangeMember As New clsOTDBXChangeMember
        Dim aXChangeMember As New clsOTDBXChangeMember
        Dim m As Object

        Dim doc9DB As Range
        Dim UIDCol, col As Long
        Dim UID As Long
        Dim row As Long
        Dim maxrow As Long
        Dim searchcell As Range
        Dim searcharea As Range
        Dim found As Range
        Dim findrange As Range
        Dim offset As Integer
        Dim i As Integer
        Dim aVAlue As Object
        Dim aNewValue As Object
        Dim valuechanged_flag As Boolean
        Dim rowvaluechanged_flag As Boolean
        Dim formatwarning, formatChange As Range
        'Dim aMsgLog As clsStatusMsg

        ' XChangeConfig
        aXChangeConfig = getXlsDoc9Xconfig()
        If aXChangeConfig Is Nothing Then
            updateDoc9LineFromOTDB = False
            Exit Function
        End If


        ' Get Database
        doc9DB = getdbDoc9Range()
        If doc9DB Is Nothing Then
            'Debug.Print "Fatal:Doc9DB could not be set ?!"
            updateDoc9LineFromOTDB = False
            Exit Function
        End If

        Globals.ThisAddIn.Application.EnableEvents = False

        ' Get Parameters
        UIDCol = getXLSHeaderIDColumn(constDoc9DB_UID)
        If UIDCol = -1 Then
            'Debug.Print "Fatal:UID Column is in DOC9 missing ?!"
            updateDoc9LineFromOTDB = False
            Exit Function
        End If


        formatwarning = getXLSParameterRangeByName("parameter_doc9_format_warning")
        formatChange = getXLSParameterRangeByName("parameter_doc9_format_change")
        'aMsgLog = New clsStatusMsg
        '
        offset = getXLSParameterByName("parameter_doc9db_startrow")
        ' Search
        maxrow = doc9DB.Rows.count
        searchcell = doc9DB.Cells(1, UIDCol)
        searcharea = doc9DB.Worksheet.Range(searchcell, doc9DB.Worksheet.Cells(maxrow, UIDCol))
        found = FindAll(searcharea, UID, XlFindLookIn.xlValues, XlLookAt.xlWhole, XlSearchOrder.xlByColumns, False)

        ' go through all found rows
        If Not found Is Nothing Then
            For Each findrange In found
                ' row no
                row = findrange.row - offset + 1  'Cells (1,1) is the one
                UID = findrange.Cells(1, UIDCol).Value

                rowvaluechanged_flag = False

                For Each m In INPUTXCHANGECONFIG.Attributes
                    aMQFXChangeMember = m
                    aXChangeMember = aXChangeConfig.AttributeByID(ID:=aMQFXChangeMember.ID, objectname:=aMQFXChangeMember.Objectname)

                    If Not aXChangeMember Is Nothing Then
                        If (aXChangeMember.IsCreated Or aXChangeMember.IsLoaded) _
                        And aXChangeMember.ISXCHANGED Then
                            ' current value of cell
                            col = aXChangeMember.ordinal.Value.Value
                            aVAlue = findrange.Cells(1, col).Value
                            If INPUTMAPPING.ContainsKey(aMQFXChangeMember.ordinal.Value.Value) Then
                                aNewValue = INPUTMAPPING.Item(aMQFXChangeMember.ordinal.Value.Value)
                                'If aNewValue = "" Then aNewValue = "-"

                                If Not IsNull(aNewValue) And Not IsEmpty(aNewValue) And aNewValue <> aVAlue And aNewValue <> "" Then
                                    rowvaluechanged_flag = True
                                    '** convert
                                    If (aMQFXChangeMember.ObjectEntryDefinition.Datatype = otFieldDataType.[Date] Or _
                                    aMQFXChangeMember.ObjectEntryDefinition.Datatype = otFieldDataType.Timestamp) And _
                                    IsDate(aNewValue) Then
                                        findrange.Cells(1, col).value = CDate(aNewValue)
                                    ElseIf aMQFXChangeMember.ObjectEntryDefinition.Datatype = otFieldDataType.[Long] And _
                                    IsNumeric(aNewValue) Then
                                        findrange.Cells(1, col).value = CLng(aNewValue)
                                    ElseIf aMQFXChangeMember.ObjectEntryDefinition.Datatype = otFieldDataType.Numeric And _
                                    IsNumeric(aNewValue) Then
                                        findrange.Cells(1, col).value = CDbl(aNewValue)
                                    ElseIf IsEmpty(aNewValue) Then
                                        findrange.Cells(1, col).value = "-"
                                    Else
                                        findrange.Cells(1, col).value = CStr(aNewValue)
                                    End If
                                    'findrange.Cells(1, col).Value = aNewValue
                                    '* format
                                    Call copyDoc9Format(formatChange, findrange.Cells(1, col), True)
                                End If
                            End If
                        End If
                    End If

                    If rowvaluechanged_flag Then
                        Globals.ThisAddIn.Application.StatusBar = "Update Doc#9 Database Row#" & row
                    End If
                Next m
            Next findrange
        End If

        Globals.ThisAddIn.Application.EnableEvents = True
        updateDoc9LineFromOTDB = True
    End Function

End Module
