
REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE TOOLING 4 EXCEL
REM ***********
REM *********** Global Definitions
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
Imports OnTrack


'****
'**** declare the DatabaseDescription
'****

Public Structure xlsDBDesc

    Public DBName As String

    Public ID As String
    Public TITLE As String
    Public ColumnNo As Long

    Public Fieldtype As xlsDBFieldType    ' constXLSFieldType_*
    Public PARAMETER As String
    Public OTDBRelation As String    ' The mapping for the datafields
    Public OTDBPrimaryKeys As String    ' The mapping for the keys of the otdb

End Structure


' Description Column No in the corresponding Table

Public Enum xlsDBDescColNo
    FieldID = 0
    TITLE = 1
    ColumnNo = 2
    FieldType = 3
    PARAMETER = 4
    OTDBRelation = 5
    OtdbPrimaryKey = 6
End Enum

Public Enum xlsDBFieldType
    numeric = 1
    List
    text
    runtime
    Formula
    datevalue
    Longvalue
    TIMESTAMP
    bool
End Enum

'***
'*** declare the statistic Tuple
'***

Structure StatisticTuple
    ' Dimensions
    Public department As String
    Public week As Integer
    Public year As Integer
    Public carselection As clsLEGACYCartypes

    ' Number of Approvals
    Public noApprovals As Long
    Public noFCApprovals As Long
    Public noBaseLineApprovals As Long
    Public noDelays As Long
    Public noMissedOverDueTarget As Long
    Public EarlyBirdApproval As Long
    Public PendingApproval As Long
    Public noTargets As Long

End Structure

Public Structure FCScheduleType
    Public ID As String
    Public desc As String
    Public AliveStatus As clsFCLFCStatus

    ' Field-IDs to be included in forecast
    Public fc_mandatory() As xlsDBDesc
    Public fc_mandatory_flag As Boolean
    Public fc_forbidden() As xlsDBDesc
    Public fc_forbidden_flag As Boolean
    Public fc_facultative() As xlsDBDesc
    Public fc_facultative_flag As Boolean

    ' field-IDs to be included in IST values
    Public mandatory() As xlsDBDesc
    Public mandatory_flag As Boolean
    Public forbidden() As xlsDBDesc
    Public forbidden_flag As Boolean
    Public facultative() As xlsDBDesc
    Public facultative_flag As Boolean

    'order of the process
    Public fc_milestone_order() As xlsDBDesc
    Public fc_milestone_flag As Boolean
    Public milestone_order() As xlsDBDesc
    Public milestone_flag As Boolean

End Structure

'****
'**** IFM Declare the PairTuple
'****

Public Structure PairTuple
    Public department As Object    'Department Code
    Public assy As Object    ' Assembly
    Public Name As Object    ' Name
    Public Cartypes As clsLEGACYCartypes    ' Cartype
    Public ifcUID As Long    ' Line Number in Interface DB
    Public ICD As String
    Public ICDVer As String

End Structure

'*****
'***** IFM Declare the Interface Tuple 1:1 -> the clsMatrixRow holds n:m
'*****

Structure InterfaceTuple
    Public pair1 As PairTuple
    Public pair2 As PairTuple
    Public status As clsIFStatus
    Public ICD As String
    Public ICDVer As String

    Public IClass As String
End Structure

Module modGlobals

    ' ***************************************************************************************************
    '   Module for public Defintion of parameters, Types, Constants
    '
    '   Author: B.Schneider
    '   created: 2012-02-14
    '
    '   change-log:
    ' ***************************************************************************************************


    Public Const constMSGType_ERROR = 1
    Public Const constMSGType_WARNING = 2
    Public Const constMSGType_INFO = 3
    Public Const constMSGType_Attention = 4

   



    '***** Constants
    '*****
    '*****

    Public Const constMaxNoCartypes As UShort = 24
    Public Const constDoc9ToolingParameterName As String = "parameter_doc9_tooling_xla_name"  ' CHANGE IF YOU CHANGE NAME OF THE XLA !
    Public Const constDoc9ToolingPatchLevel = 1
    Public Const constParameterSheetName = "Parameters"
    Public Const constDoc9StructureSheetName = "StructureDatabase"
    Public Const constdbdoc9structureName = "dbdoc9structure"
    Public Const constHdbdoc9structureName = "dbHdoc9structure"

    Public Const constDBColumnPrefix = "parameter_doc9_structure_dbdesc_"

    Public Const constdbrevisionLogName = "DBRevisionLog"

    Public Const constLockedBackground = 15853019
    Public Const constProcessBackground = 16771295
    Public Const constNormalBackground = &H80000005
    Public Const constHighlightBackground = &HC0FFFF
    Public Const constErrorBackground = &HC0&

    Public Const constDelimeter = " ,;/|" & vbLf

    Public Const constPasswordTemplate = "NasiGoreng"
    Public Const constPasswordParameters = "englischerrasen"

    Public Const constDoc9DB_UID = "uid"

    Public Const constfclfcStatus_PROCESSED_NO_ERRORS = "written to the doc9 database"
    Public Const constfclfcStatus_PROCESSED_WITH_ERRORS = "written and corrected to the doc9 database - see log"

    Public Const constDBStatisticName = "dbstatistic"
    '******
    '****** publics
    '******
    '******


    Public parameter_status_Doc9_table As Range
    Public parameter_status_Doc9_x_table As Range

    Public ourSMBDoc9 As Workbook

    '******* MQF Status Table CACHE -> clsMQFStatus
    '*******
    Public parameter_status_mqf_message_table As Range
    Public mqf_status_table(,) As Object
    Public mqf_status_table_maxcol As Integer
    Public mqf_status_table_maxrow As Integer

    ' const no of columns in the table to hold status parameter
    '-> look also in clsMQFStatus
    Public Const const_parameter_mqf_status_col_code = 1
    Public Const const_parameter_mqf_status_col_name = 2
    Public Const const_parameter_mqf_status_col_processed = 3
    Public Const const_parameter_mqf_status_col_weight = 4
    Public Const const_parameter_mqf_status_col_desc = 5
    Public Const const_parameter_mqf_status_col_format = 6    ' additional

    ' define const for stati
    Public Const constStatusCode_skipped = "s1"
    Public Const constStatusCode_error = "r1"
    Public Const constStatusCode_processed_ok = "g1"
    Public Const constStatusCode_processed_warnings = "y1"
    Public Const constStatusCode_processed_infos = "g2"
    Public Const constStatusCode_forapproval = "y2"
    Public Const constStatusCode_approvalrejected = "r2"

    ' Forecast LifeCycle Status Table CACHE -> clsMQFStatus
    Public parameter_status_FCLFC_table As Range
    Public fclfc_status_table(,) As Object
    Public fclfc_status_table_maxcol As Integer
    Public fclfc_status_table_maxrow As Integer
    ' const no of columns in the table to hold status parameter
    '-> look also in clsMQFStatus
    Public Const const_parameter_fclfc_status_col_code = 1
    Public Const const_parameter_fclfc_status_col_name = 2
    Public Const const_parameter_fclfc_status_col_alive = 3
    Public Const const_parameter_fclfc_status_col_weight = 4
    Public Const const_parameter_fclfc_status_col_desc = 5
    Public Const const_parameter_fclfc_status_col_kpi = 6    ' additional

    Public Const constFCLFCStatusCode_error_missing = "r2"
    Public Const constFCLFCStatusCode_error_outdated = "r1"
    Public Const constFCLFCStatusCode_error_notascending = "r3"
    Public Const constFCLFCStatusCode_error_value_notvalid = "r4"
    Public Const constFCLFCStatusCode_warning = "y1"
    Public Const constFCLFCStatusCode_alive = "g1"
    Public Const constFCLFCStatusCode_over = "g2"
    Public Const constFCLFCStatusCode_na = "na"

    ' Process Status Table CACHE -> clsProcessStatus
    Public parameter_status_Process_table As Range
    Public Process_status_table(,) As Object
    Public Process_status_table_maxcol As Integer
    Public Process_status_table_maxrow As Integer

    ' const no of columns in the table to hold status parameter
    '-> look also in clsprocessStatus
    Public Const const_parameter_process_Status_col_code = 1
    Public Const const_parameter_process_Status_col_name = 2
    Public Const const_parameter_process_Status_col_finished = 3
    Public Const const_parameter_process_Status_col_weight = 4
    Public Const const_parameter_process_Status_col_risk = 5
    Public Const const_parameter_process_Status_col_desc = 6
    Public Const const_parameter_process_Status_col_kpi = 7    ' additional

    ' Process Status Table CACHE -> clsProcessStatus
    Public parameter_status_DMU_table As Range
    Public DMU_status_table(,) As Object
    Public DMU_status_table_maxcol As Integer
    Public DMU_status_table_maxrow As Integer

    ' const no of columns in the table to hold status parameter
    '-> look also in clsprocessStatus
    Public Const const_parameter_DMU_Status_col_code = 1
    Public Const const_parameter_DMU_Status_col_name = 2
    Public Const const_parameter_DMU_Status_col_approved = 3
    Public Const const_parameter_DMU_Status_col_weight = 4
    'Public Const const_parameter_DMU_Status_col_risk = 5
    Public Const const_parameter_DMU_Status_col_desc = 5
    Public Const const_parameter_DMU_Status_col_kpi = 6    ' additional

    ' Process Status Table CACHE -> clsProcessStatus
    Public parameter_status_FEM_table As Range
    Public FEM_status_table(,) As Object
    Public FEM_status_table_maxcol As Integer
    Public FEM_status_table_maxrow As Integer

    ' const no of columns in the table to hold status parameter
    '-> look also in clsprocessStatus
    Public Const const_parameter_FEM_Status_col_code = 1
    Public Const const_parameter_FEM_Status_col_name = 2
    Public Const const_parameter_FEM_Status_col_approved = 3
    Public Const const_parameter_FEM_Status_col_weight = 4
    'Public Const const_parameter_DMU_Status_col_risk = 5
    Public Const const_parameter_FEM_Status_col_desc = 5
    Public Const const_parameter_FEM_Status_col_kpi = 6    ' additional

    ' IFC Status Table CACHE -> clsIFStatus
    Public parameter_status_ifc_table As Range
    Public parameter_status_icd_table As Range
    Public parameter_status_icd_x_table As Range

    Public ifc_status_table(,) As Object
    Public ifc_status_table_maxcol As Integer
    Public ifc_status_table_maxrow As Integer

    '****** const no of columns in the table to hold status parameter
    '****** -> look also in clsIFStatus
    '******

    Public Const const_parameter_IF_status_col_code = 1
    Public Const const_parameter_IF_status_col_name = 2
    Public Const const_parameter_IF_status_col_oldcode = 3
    Public Const const_parameter_IF_status_col_weight = 4
    Public Const const_parameter_IF_status_col_freeze = 5
    Public Const const_parameter_IF_status_col_desc = 6
    Public Const const_parameter_IF_status_col_kpicode = 7
    Public Const const_parameter_IF_status_col_format = 8    ' additional

    '**********
    '********** publics for UFDoc9MQFAdmin

    Public UFDoc9MQFADMIN_theMQFFormat As modXLSMessageQueueFile.XLSMQFStructure
    Public UFDoc9MQFADMIN_theMQFMessages() As modXLSMessageQueueFile.MQFMessage
    Public UFDoc9MQFADMIN_theMQFWorkbook As Excel.Workbook

End Module
