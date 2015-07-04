REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE TOOLING 4 EXCEL
REM ***********
REM *********** EXCEL XCHANGE MANAGER MODULE (Static functions) for On Track Database TOOLING
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
Imports System.ComponentModel
Imports OnTrack.Core
Imports OnTrack.Database
Imports OnTrack.XChange
Imports OnTrack.Commons
Imports OnTrack

'*********
'********* CLASS XLSOTDBDataAreaStore defines a Store of DataAreas per Workbook which is connected to the
'*********                            OnTrack Database
'*********

Public Class XLSDataAreaStore

    Private _Store As Dictionary(Of String, List(Of XLSDataArea))

    Public Sub New()
        _Store = New Dictionary(Of String, List(Of XLSDataArea))

    End Sub

    Public Function maxDataAreas() As UShort
        Return MySettings.Default.Property_Xchange_NoDataAreas

    End Function
    Public Function Attach(ByRef workbook As Workbook) As Boolean

        If IsWorkbookAttached(workbook) Then
            Return False
        Else
            _Store.Add(workbook.Name, loadDataAreas(workbook))
            Return True
        End If
    End Function

    Public Function IsWorkbookAttached(ByRef workbook As Workbook) As Boolean
        Return _Store.ContainsKey(workbook.Name)
    End Function
    Public Function Deattach(ByRef workbook As Workbook) As Boolean

        If IsWorkbookAttached(workbook) Then
            _Store.Remove(workbook.Name)
            Return True
        Else
            Return False
        End If
    End Function

    Public Function addDataArea(ByRef workbook As Workbook, ByRef dataarea As XLSDataArea) As Boolean
        Dim aList As New List(Of XLSDataArea)

        If _Store.ContainsKey(workbook.Name) Then
            aList = _Store.Item(workbook.Name)
            _Store.Remove(key:=workbook.Name)
        End If
        ' add the data area
        If aList.Count = 0 Then
            dataarea.Name = "dataarea"
        Else
            dataarea.Name = "dataarea " & aList.Count
        End If
        aList.Add(dataarea)
        'save
        _Store.Add(key:=workbook.Name, value:=aList)
        Return True
    End Function

    Public Function GetDataAreas(Optional ByRef workbook As Workbook = Nothing, _
    Optional ByVal refresh As Boolean = False) As List(Of XLSDataArea)

        ' workbook
        If workbook Is Nothing Then
            workbook = Globals.ThisAddIn.Application.ActiveWorkbook
        End If

        If _Store.ContainsKey(workbook.Name) And Not refresh Then
            Return _Store.Item(workbook.Name)
        ElseIf _Store.ContainsKey(workbook.Name) = False Then
            If Me.Attach(workbook) Then
                Return _Store.Item(workbook.Name)
            End If
        Else
            Dim aList As List(Of XLSDataArea)
            If _Store.ContainsKey(workbook.Name) Then
                aList = _Store.Item(workbook.Name)
                aList.Clear()
                _Store.Remove(workbook.Name)
            End If
            For Each aDataarea As XLSDataArea In loadDataAreas(workbook)
                aList.Add(aDataarea)
            Next
            _Store.Add(key:=workbook.Name, value:=aList)
            Return aList
        End If
    End Function

    Public Function loadDataAreas(Optional ByRef workbook As Workbook = Nothing) As List(Of XLSDataArea)
        Dim aList As New List(Of XLSDataArea)

        ' check which properties we have
        Dim i As Integer = 1
        Dim foundname, foundaddress, foundheaderid As Boolean
        Dim name, address, headerid, selectionID, xconfigname, prefixreference, transactionID, keyIDs, transactionLogID, statusID, timestampID, extendID As String
        Dim extend As Boolean
        Dim PropertyDefaultName As String = MySettings.Default.Property_XChange_DataArea

        Dim MaxProperty As UShort = maxDataAreas()
        Dim aDataArea As XLSDataArea
        Dim Propertyname As String
        Dim PropertyString As String
        Dim Parameters() As String


        ' workbook
        If workbook Is Nothing Then
            workbook = Globals.ThisAddIn.Application.ActiveWorkbook
        End If

        ' search 10 areas
        For i = 0 To MaxProperty
            If i = 0 Then
                Propertyname = PropertyDefaultName
            Else
                Propertyname = PropertyDefaultName & i
            End If

            PropertyString = GetHostProperty(Propertyname, host:=workbook, silent:=True, found:=foundname)
            If PropertyString Is Nothing Then PropertyString = ""
            Parameters = SplitMultbyChar(PropertyString, ConstDelimiter)
            If IsArrayInitialized(Parameters) Then

                xconfigname = Parameters(0)
                address = Parameters(1)
                headerid = Parameters(2)
                selectionID = Parameters(3)
                If Parameters.Count >= 5 Then
                    name = Parameters(4)
                Else
                    name = ""
                End If
                If Parameters.Count >= 6 Then
                    prefixreference = Parameters(5)
                Else
                    prefixreference = "parameter_" & name & "_"
                End If
                If Parameters.Count >= 7 Then
                    transactionID = Parameters(6)
                Else
                    transactionID = ""
                End If
                If Parameters.Count >= 8 Then
                    transactionLogID = Parameters(7)
                Else
                    transactionLogID = ""
                End If
                If Parameters.Count >= 9 Then
                    timestampID = Parameters(8)
                Else
                    timestampID = ""
                End If
                If Parameters.Count >= 10 Then
                    statusID = Parameters(9)
                Else
                    statusID = ""
                End If
                If Parameters.Count >= 11 Then
                    keyIDs = Parameters(10)
                Else
                    keyIDs = ""
                End If
                If Parameters.Count >= 12 Then
                    extendID = Parameters(11)
                Else
                    extendID = ""
                End If
                '*** add to List
                Try
                    If i = 0 And name = "" Then
                        name = "data area"
                    ElseIf name = "" Then
                        name = "data area " & i
                    End If

                    aDataArea = New XLSDataArea(name)
                    With aDataArea
                        .DataRangeAddress = address
                        If headerid <> "" Then
                            .HeaderIDAddress = headerid
                        End If
                        .SelectionID = selectionID
                        .XConfigName = xconfigname
                        .PrefixReferences = prefixreference
                        .TimestampID = timestampID
                        .StatusID = statusID
                        .TransactionLogID = transactionLogID
                        .KeyIDString = keyIDs
                        If Trim(extendID).Length > 0 Then
                            .ExtendDynamic = True
                        Else
                            .ExtendDynamic = False
                        End If

                    End With


                    ' add to list
                    aList.Add(aDataArea)

                Catch ex As Exception
                    Call CoreMessageHandler(exception:=ex, break:=False)
                End Try


            End If
        Next

        Return aList
    End Function

    Public Function saveDataAreas(Optional ByRef workbook As Workbook = Nothing) As Boolean
        Dim aList As New List(Of XLSDataArea)

        ' check which properties we have
        Dim i As Integer = 1
        Dim foundname, foundaddress, foundheaderid As Boolean
        Dim name, address, headerid As String
        Dim PropertyDefaultName As String = MySettings.Default.Property_XChange_DataArea

        Dim MaxProperty As UShort = maxDataAreas()

        Dim Propertyname As String
        Dim PropertyString As String


        ' workbook
        If workbook Is Nothing Then
            workbook = Globals.ThisAddIn.Application.ActiveWorkbook
        End If

        If Not _Store.ContainsKey(workbook.Name) Then
            Return False
        Else
            aList = _Store.Item(workbook.Name)
        End If

        '** save as properties
        i = 0
        For Each aDataArea As XLSDataArea In aList

            If i = 0 Then
                Propertyname = PropertyDefaultName
                If aDataArea.Name = "" Then
                    aDataArea.Name = "data area"
                End If
            Else
                Propertyname = PropertyDefaultName & i
                If aDataArea.Name = "" Then
                    aDataArea.Name = "data area " & i
                End If
            End If


            ' save to properties of the Excel File
            If aDataArea.DataRangeAddress <> "" Then
                Try
                    PropertyString = Core.ConstDelimiter & aDataArea.XConfigName & _
                    ConstDelimiter & aDataArea.DataRangeAddress & _
                    ConstDelimiter & aDataArea.HeaderIDAddress & _
                    ConstDelimiter & aDataArea.SelectionID & _
                    ConstDelimiter & aDataArea.Name & _
                    ConstDelimiter & aDataArea.PrefixReferences & _
                    ConstDelimiter & aDataArea.TransactionID & _
                    ConstDelimiter & aDataArea.TransactionLogID & _
                    ConstDelimiter & aDataArea.TimestampID & _
                    ConstDelimiter & aDataArea.StatusID & _
                    ConstDelimiter & aDataArea.KeyIDString & _
                    ConstDelimiter & aDataArea.ExtendDynamic.ToString & _
                    ConstDelimiter
                    ' set it
                    SetHostProperty(Propertyname, PropertyString, workbook, silent:=True)

                    ' increase i
                    i += 1
                Catch ex As Exception
                    Call CoreMessageHandler(exception:=ex, break:=False)
                End Try

            End If

        Next
        ' search up to 10
        For j As UShort = i To MaxProperty
            If j = 0 Then
                Propertyname = PropertyDefaultName
            Else
                Propertyname = PropertyDefaultName & j
            End If

            Try
                SetHostProperty(Propertyname, "", workbook, silent:=True)

            Catch ex As Exception

                Call CoreMessageHandler(exception:=ex, break:=False)
            End Try


        Next

        Return True
    End Function
End Class


''' <summary>
''' defines a Data Range which is connected to the OnTrack Database
''' </summary>
''' <remarks></remarks>
Public Class XLSDataArea
    Implements INotifyPropertyChanged


    ' Declare the event 
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Private WithEvents sWorkbook As Microsoft.Office.Interop.Excel.Workbook

    ' declare internal variables
    Private sName As String 'Name of the DataArea

    ' data range
    Private sDataRange As Range
    Private sDataRangeAddress As String
    ' header ids
    Private sHeaderIDRange As Range 'HeaderID Range
    Private sHeaderIDAddress As String
    ' keys
    Private sKeyIDString As String
    Private sKeyIDs As String()
    ' special columns
    Private sSelectionID As String 'selection ID
    Private sTransactionID As String ' transaction column
    Private sTimestampID As String ' ID for the update timestamp
    Private sTransactionLogID As String ' ID for the transactionLog
    Private sStatusID As String 'ID for transaction status
    Private sExtendDynamic As Boolean

    ' prefix for the autogen parameters
    Private sPrefixReferences As String

    ' Xconfig    
    Private sXConfig As XChangeConfiguration
    Private sXConfigName As String 'Name of the associated Xconfig

    ' Database Description
    Private sDBDescRange As Range 'Range of the DBDescription
    Private sDBDescRangeAddress As String
    Private DBDescID As Dictionary(Of String, xlsDBDesc) 'Store of ID per xlsDBDesc Entry
    Private DBDescColumn As Dictionary(Of Long, xlsDBDesc) ' Store of Column per xlsDBDesc Entry

    Public Sub New(name As String)
        sName = name
    End Sub
    Public Sub New(name As String, [xConfig] As XChangeConfiguration)
        sName = name
        sXConfig = [xConfig]
    End Sub
    Public Sub New(name As String, workbook As Microsoft.Office.Interop.Excel.Workbook)
        sName = name
        sWorkbook = workbook
    End Sub

#Region "Properties"
    ''' <summary>
    ''' Gets or sets the PS workbook.
    ''' </summary>
    ''' <value>The PS workbook.</value>
    <DisplayName("Excel Workbook")> _
    <Category("Data Area")> _
    <Description("Name of the ExcelWorkbook")> _
    Public Property WorkbookName() As String
        Get
            If sWorkbook Is Nothing Then
                Return ""
            Else
                Return Me.sWorkbook.Name
            End If

        End Get
        Set(value As String)
            Try
                Me.sWorkbook = Globals.ThisAddIn.Application.Workbooks.Item(value)
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, break:=False, procedure:="XLSDataarea.workbookname")
            End Try
            ' Call OnPropertyChanged whenever the property is updated
            OnPropertyChanged("WorkbookName")
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the prefix references.
    ''' </summary>
    ''' <value>The prefix references.</value>
    <DisplayName("prefix of references")> _
    <Category("Data Configuration")> _
    <Description("Prefix of autogenerated Excel sheets parameter reference names to the column numnber")> _
    Public Property PrefixReferences() As String
        Get
            Return Me.sPrefixReferences
        End Get
        Set(value As String)
            Me.sPrefixReferences = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the name of the PSX config.
    ''' </summary>
    ''' <value>The name of the PSX config.</value>
    ''' 
    <DisplayName("XChangeConfigName")> _
    <Category("Data Configuration")> _
    <Description("Name of the associated XChange Configuration")> _
    Public Property XConfigName() As String
        Get
            Return Me.sXConfigName
        End Get
        Set(value As String)
            Me.sXConfigName = value
            ' Call OnPropertyChanged whenever the property is updated
            OnPropertyChanged("XConfigName")
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the PS extend dynamic.
    ''' </summary>
    ''' <value>The PS extend dynamic.</value>
    ''' 
    <DisplayName("Allow Automatic Extend")> _
    <Category("Data Configuration")> _
    <Description("Size of data area is driven by database if enabled. Or by key columns if disabled.")> _
    Public Property ExtendDynamic() As Boolean
        Get
            Return Me.sExtendDynamic
        End Get
        Set(value As Boolean)
            Me.sExtendDynamic = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the name of the PSX config.
    ''' </summary>
    ''' <value>The name of the PSX config.</value>
    ''' 
    <DisplayName("XChangeConfig")> _
    <Category("Data Configuration")> _
    <Browsable(False)> _
    <Description("The associated XChange Configuration")> _
    Public Property XConfig() As XChangeConfiguration
        Get
            'load
            If sXConfig Is Nothing And sXConfigName <> "" Then
                sXConfig = XChangeConfiguration.Retrieve(sXConfigName)
                Return sXConfig

            ElseIf Not sXConfig Is Nothing AndAlso (sXConfig.IsLoaded Or sXConfig.IsCreated) Then
                Return Me.sXConfig
            Else
                Return Nothing
            End If
        End Get
        Set(value As XChangeConfiguration)
            Me.sXConfig = value
            Me.sXConfigName = value.Configname
            ' Call OnPropertyChanged whenever the property is updated
            'OnPropertyChanged("XConfig") -> donot fire
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the PS selection ID.
    ''' </summary>
    ''' <value>The PS selection ID.</value>
    <DisplayName("SelectionID")> _
    <Category("Data Range")> _
    <Description("Name of the Header ID for the selection column. Can be left blank if not existing in data area.")> _
    Public Property SelectionID() As String
        Get
            Return Me.sSelectionID
        End Get
        Set(value As String)
            Me.sSelectionID = value
            ' Call OnPropertyChanged whenever the property is updated
            OnPropertyChanged("SelectionID")
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the PS header ID.
    ''' </summary>
    ''' <value>The PS header ID.</value>
    <DisplayName("HeaderIds Range")> _
    <Category("Data Range")> _
    <Browsable(False)> _
    <Description("Address or named Range for the Header IDs of data area")> _
    Public Property HeaderIDRange() As Range
        Get
            Return Me.sHeaderIDRange
        End Get
        Set(value As Range)
            Me.sHeaderIDRange = value
            ' Call OnPropertyChanged whenever the property is updated
            OnPropertyChanged("HeaderIDRange")
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the PS header ID.
    ''' </summary>
    ''' <value>The PS header ID.</value>
    <DisplayName("HeaderIds Range Address")> _
    <Category("Data Range")> _
    <Description("Address or named Range for the Header IDs of data area")> _
    Public Property HeaderIDAddress() As String
        Get
            Return sHeaderIDAddress
        End Get
        Set(value As String)
            Try
                If Not GetXlsParameterRangeByName(value, workbook:=sWorkbook, silent:=True) Is Nothing Then
                    Me.HeaderIDRange = GetXlsParameterRangeByName(value, workbook:=sWorkbook, silent:=True)
                    sHeaderIDAddress = value
                ElseIf sWorkbook Is Nothing Then
                    Dim aSheet As Excel.Worksheet
                    aSheet = Globals.ThisAddIn.Application.ActiveWorkbook.ActiveSheet
                    If value.Contains("!") Then
                        Me.HeaderIDRange = Globals.ThisAddIn.Application.Range(value)
                    Else
                        Me.HeaderIDRange = aSheet.Range(value)
                    End If
                    sHeaderIDAddress = "'[" & Me.HeaderIDRange.Worksheet.Parent.Name & "]" & Me.HeaderIDRange.Worksheet.Name & "'!" & Me.HeaderIDRange.Address
                Else
                    Dim aSheet As Excel.Worksheet
                    aSheet = sWorkbook.ActiveSheet
                    If value.Contains("!") Then
                        Me.HeaderIDRange = Globals.ThisAddIn.Application.Range(value)
                    Else
                        Me.HeaderIDRange = aSheet.Range(value)
                    End If

                    sHeaderIDAddress = "'[" & sWorkbook.Name & "]" & Me.HeaderIDRange.Worksheet.Name & "'!" & Me.HeaderIDRange.Address
                End If
                ' Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("HeaderIDAddress")

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, break:=False, procedure:="XLSDataarea.HeaderIDAddress")
            End Try

        End Set
    End Property

    ''' <summary>
    ''' Gets the name of the PS.
    ''' </summary>
    ''' <value>The name of the PS.</value>
    <DisplayName("Data Area Identifier")> _
    <Category("Data Area")> _
    <Description("Identiefier of the data area in the workbook")> _
    Public Property Name() As String
        Get
            Return Me.sName

        End Get
        Set(ByVal value As String)
            sName = value
            ' Call OnPropertyChanged whenever the property is updated
            OnPropertyChanged("Name")
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the PS data range.
    ''' </summary>
    ''' <value>The PS data range.</value>
    <DisplayName("Data Area Range")> _
    <Category("Data Range")> _
    <Browsable(False)> _
    <Description("Address or named range of the data area in the workbook")> _
    Public Property DataRange() As Range
        Get
            Return Me.sDataRange
        End Get
        Set(value As Range)
            Me.sDataRange = value
            sWorkbook = value.Worksheet.Parent
            ' Call OnPropertyChanged whenever the property is updated
            OnPropertyChanged("DataRange")
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the datarange address
    ''' </summary>
    ''' <value>address as string</value>
    <DisplayName("Data Area Range Address")> _
    <Category("Data Range")> _
    <Description("Address or named range of the data area in the workbook")> _
    Public Property DataRangeAddress() As String
        Get

            Return sDataRangeAddress

        End Get
        Set(value As String)
            Try
                If Not GetXlsParameterRangeByName(value, workbook:=sWorkbook, silent:=True) Is Nothing Then
                    Me.DataRange = GetXlsParameterRangeByName(value, workbook:=sWorkbook, silent:=True)
                    sDataRangeAddress = value
                ElseIf sWorkbook Is Nothing Then
                    Dim aSheet As Excel.Worksheet
                    aSheet = Globals.ThisAddIn.Application.ActiveWorkbook.ActiveSheet
                    If value.Contains("!") Then
                        Me.DataRange = Globals.ThisAddIn.Application.Range(value)
                    Else
                        Me.DataRange = aSheet.Range(value)
                    End If
                    sDataRangeAddress = "'[" & Me.DataRange.Worksheet.Parent.Name & "]" & Me.DataRange.Worksheet.Name & "'!" & Me.DataRange.Address
                Else
                    Dim aSheet As Excel.Worksheet
                    aSheet = sWorkbook.ActiveSheet
                    If value.Contains("!") Then
                        Me.DataRange = Globals.ThisAddIn.Application.Range(value)
                    Else
                        Me.DataRange = aSheet.Range(value)
                    End If

                    sDataRangeAddress = "'[" & sWorkbook.Name & "]" & Me.DataRange.Worksheet.Name & "'!" & Me.DataRange.Address
                End If
                ' Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("DataRangeAddress")
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, break:=False, procedure:="XLSDataarea.DataRangeAddress")
            End Try

        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the PS key I ds.
    ''' </summary>
    ''' <value>The PS key I ds.</value>
    <DisplayName("key header IDs")> _
    <Category("Data Range")> _
    <Browsable(False)> _
    <Description("key header IDs as array")> _
    Public Property KeyIDs() As String()
        Get
            sKeyIDs = sKeyIDString.Split(",")
            Return Me.sKeyIDs
        End Get
        Set(value As String())

            sKeyIDString = ""
            For Each s As String In value
                sKeyIDString = sKeyIDString & "," & Trim(s)
            Next

            Me.sKeyIDs = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the PS key ID string.
    ''' </summary>
    ''' <value>The PS key ID string.</value>
    <DisplayName("key header IDs")> _
    <Category("Data Range")> _
    <Browsable(True)> _
    <Description("key header IDs as list with ','")> _
    Public Property KeyIDString() As String
        Get
            Return Me.sKeyIDString
        End Get
        Set(value As String)
            Me.sKeyIDString = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the PS status ID.
    ''' </summary>
    ''' <value>The PS status ID.</value>
    <DisplayName("status header ID")> _
    <Category("Data Range")> _
    <Browsable(True)> _
    <Description("status header ID")> _
    Public Property StatusID() As String
        Get
            Return Me.sStatusID
        End Get
        Set(value As String)
            Me.sStatusID = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the PS transaction log ID.
    ''' </summary>
    ''' <value>The PS transaction log ID.</value>
    <DisplayName("transaction log header ID")> _
    <Category("Data Range")> _
    <Browsable(True)> _
    <Description("transaction log header ID to store the messages for the transaction in dataarea")> _
    Public Property TransactionLogID() As String
        Get
            Return Me.sTransactionLogID
        End Get
        Set(value As String)
            Me.sTransactionLogID = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the PS timestamp ID.
    ''' </summary>
    ''' <value>The PS timestamp ID.</value>
    <DisplayName("timestamp header ID")> _
    <Category("Data Range")> _
    <Browsable(True)> _
    <Description("timestamp header ID to store the last OnTrack transaction date")> _
    Public Property TimestampID() As String
        Get
            Return Me.sTimestampID
        End Get
        Set(value As String)
            Me.sTimestampID = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the PS transaction ID.
    ''' </summary>
    ''' <value>The PS transaction ID.</value>
    <DisplayName("transaction header ID")> _
    <Category("Data Range")> _
    <Browsable(True)> _
    <Description("transaction header ID to identify the command / transaction of the data row")> _
    Public Property TransactionID() As String
        Get
            Return Me.sTransactionID
        End Get
        Set(value As String)
            Me.sTransactionID = value
        End Set
    End Property

#End Region

    '****** getHeaderIDColumn returns the Column number of ther header id (overstepps "" etc)
    '******                           or zero (if not found)

    Function GetHeaderIDColumn(ByVal headerid As String) As UShort
        Dim i As UShort = 1

        If Me.HeaderIDRange Is Nothing Then
            Call CoreMessageHandler(message:="header range of data area is not set", procedure:="xlsdataarea.getxlsheaderidcolumn")
            Return 0
        End If

        '**
        Dim aList As Object(,) = Me.HeaderIDRange.Value
        ' i holds the column
        For Each aValue As Object In aList
            If LCase(aValue) = LCase(headerid) Then
                Return i
            End If
            i += 1
        Next

        Return 0
    End Function

    ''' <summary>
    ''' returns the selection from the selection column as range
    ''' </summary>
    ''' <param name="silent"></param>
    ''' <param name="selectionHeaderID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <remarks></remarks>
    Public Function GetSelectionAsRange(Optional silent As Boolean = False, _
                                        Optional selectionHeaderID As String = "") As Range


        Dim selectioncol, selected As Range
        Dim column As Long
        Dim msgboxrsl As Object

        ' set default
        If selectionHeaderID = "" Then
            selectionHeaderID = Me.SelectionID
        End If

        ' selection
        column = GetHeaderIDColumn(selectionHeaderID)
        If column = 0 OrElse Me.DataRange Is Nothing Then
            Return Nothing
        End If

        selected = Nothing
        selectioncol = Me.DataRange.Worksheet.Range(Me.DataRange.Cells(1, column), Me.DataRange.Cells(Me.DataRange.Rows.Count, column))
        Dim selectionfields(,) As Object = selectioncol.Value
        Dim i As ULong = 1

        ' select
        For Each selectfield As Object In selectionfields

            If Not selectfield Is Nothing Then

                If selected Is Nothing Then
                    selected = Me.DataRange.Cells(i, column)
                Else
                    selected = Globals.ThisAddIn.Application.Union(selected, Me.DataRange.Cells(i, column))
                End If
            End If

            i += 1
        Next

        '* nothing selected
        If selected Is Nothing Then
            Telerik.WinControls.RadMessageBox.SetThemeName("TelerikMetroBlue")
            Dim aresult As System.Windows.Forms.DialogResult = Telerik.WinControls.RadMessageBox.Show(text:="ATTENTION !" & vbLf & "No data rows have been selected in the SELECTION Column of the Database. Should ALL rows be selected ?", _
                                                          caption:="Please confirm", _
                                                          icon:=Telerik.WinControls.RadMessageIcon.Question, _
                                                          buttons:=System.Windows.Forms.MessageBoxButtons.YesNo, _
                                                          defaultButton:=System.Windows.Forms.MessageBoxDefaultButton.Button2)
            If aresult = System.Windows.Forms.DialogResult.Yes OrElse aresult = System.Windows.Forms.DialogResult.OK Then
                selected = selectioncol.Cells
            Else
                Return Nothing
            End If
        End If

        Return selected
    End Function

    '****** AddHeaderIDRange2Config
    '******
    Public Function AddHeaderIDs2XConfig(ByVal XCMD As otXChangeCommandType) As Boolean
        Dim aList As New List(Of String)

        ''' collect all the cell values for the header ids in the header id range of excel
        For Each cell As Excel.Range In Me.HeaderIDRange.Cells
            If Not Globals.ThisAddIn.Application.WorksheetFunction.IsError(cell) AndAlso Not cell.Value Is Nothing Then
                aList.Add(cell.Value.ToString.ToUpper)
            End If
        Next

        ''' add these to the headerids
        If aList.Count > 0 Then
            Return Me.AddHeaderIDs2XConfig(aList, XCMD)
        End If

        Return False
    End Function

    Private Function AddHeaderIDs2XConfig(ByRef headerids As List(Of String), ByVal XCMD As otXChangeCommandType) As Boolean
        Dim aXConfig As XChangeConfiguration = Me.XConfig
        Dim isReadonly As Boolean = False
        Dim i As Long = 1


        If aXConfig Is Nothing OrElse Not (aXConfig.IsLoaded Or aXConfig.IsCreated) Then

            Call CoreMessageHandler(message:="couldnot load default XConfig " & XConfigName, _
                                    procedure:="AddHeaderIDs2XConfig")
            Return False

        End If

        ' add the IDs
        For Each id As String In headerids

            '***
            '*** HACK 
            '***
            If Trim(id) <> "" Then
                Call aXConfig.AddEntryByXID(Xid:=Trim(id), ordinal:=i, xcmd:=XCMD, readonly:=isReadonly, isXChanged:=True)
            End If
            i += 1
        Next

        Return True
    End Function
#Region "Events"
    ' Create the OnPropertyChanged method to raise the event 
    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
#End Region

End Class

''' <summary>
''' defines a XBag as Excel Converter function
''' </summary>
''' <remarks></remarks>

Public Class ExcelXBag
    Inherits XBag

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="xConfig"></param>
    ''' <remarks></remarks>
    Public Sub New(xConfig As XChangeConfiguration)
        MyBase.New(xConfig)
    End Sub

    ''' <summary>
    ''' Converts to Hostvalue
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    ''' <remarks></remarks>
    Sub Convert2Hostvalue(ByVal sender As Object, ByVal args As ConvertRequestEventArgs) Handles MyBase.ConvertRequest2HostValue

        If Not args.result Then

            '** special values
            '**

            '*** if we already know it is null Reset to '-'
            If args.IsNull Then

                ' HACK ! Here we should define which IDs/Slot react with which value
                Select Case args.Datatype

                    Case otDataType.Bool
                        If args.Dbvalue = True Then
                            args.Hostvalue = args.Dbvalue
                            args.result = True
                        End If

                    Case Else
                        args.Hostvalue = constMQFClearFieldChar
                        args.result = True
                        Return

                End Select

            ElseIf args.IsEmpty Then
                '** Do not Change Value at all
                args.Hostvalue = Nothing
                args.IsEmpty = True
                args.result = True
                Return
            Else

                '*** Take the Default Routine
                args.result = XSlot.DefaultConvert2HostValue(datatype:=args.Datatype, hostvalue:=args.Hostvalue, dbvalue:=args.Dbvalue, _
                                                                       dbValueIsEmpty:=args.DbValueIsEmpty, dbValueIsNull:=args.DbValueisNull, _
                                                                       hostValueIsEmpty:=args.HostValueisEmpty, hostValueIsNull:=args.HostValueisNull, _
                                                                       msglog:=args.Msglog)
                '** was converted to null
                '*** Reset to '-'
                If args.HostValueisNull Then

                    ' HACK ! Here we should define which IDs/Slot react with which value
                    Select Case args.Datatype

                        Case otDataType.Bool
                            If args.Dbvalue = True Then
                                args.Hostvalue = args.Dbvalue
                                args.result = True
                            End If

                        Case Else
                            args.Hostvalue = constMQFClearFieldChar
                            args.result = True
                            Return

                    End Select
                End If
                Return
            End If


        End If


    End Sub

    ''' <summary>
    ''' Converts to Hostvalue
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    ''' <remarks></remarks>
    Sub Convert2DBvalue(ByVal sender As Object, ByVal args As ConvertRequestEventArgs) Handles MyBase.ConvertRequest2DBValue

        If Not args.result Then

            '** special values
            '*** Reset to '-'
            If CStr(args.Hostvalue) = constMQFClearFieldChar OrElse args.IsNull Then
                args.DbValueisNull = True
                ' HACK ! Here we should define which IDs/Slot react with which value
                Select Case args.Datatype
                    Case otDataType.Date, otDataType.Timestamp
                        args.Dbvalue = Nothing
                        args.result = True
                        Return
                    Case otDataType.Time
                        args.Dbvalue = Nothing
                        args.result = True
                        Return
                    Case otDataType.Long
                        args.Dbvalue = Nothing
                        args.result = True
                        Return
                    Case otDataType.Numeric
                        args.Dbvalue = Nothing
                        args.result = True
                        Return
                    Case otDataType.Bool
                        args.Dbvalue = Nothing
                        args.result = True
                        Return
                    Case otDataType.Text, otDataType.Memo, otDataType.List
                        args.Dbvalue = Nothing
                        args.result = True
                        Return
                    Case Else
                        CoreMessageHandler(message:="cannot determine default converter for '-'", messagetype:=otCoreMessageType.InternalError, procedure:="ExcelXBag.ConvertRequest2DBValue")
                        args.result = False
                        args.Dbvalue = Nothing
                        Return
                End Select

            ElseIf String.IsNullOrEmpty(args.Hostvalue) Then
                '** Do not Change Value at all
                args.DbValueIsEmpty = True
                args.result = True
                Return
            Else
                '*** Take the Default Routine
                args.result = XSlot.DefaultConvert2DBValue(datatype:=args.Datatype, hostvalue:=args.Hostvalue, dbvalue:=args.Dbvalue, _
                                                                      dbValueIsEmpty:=args.DbValueIsEmpty, dbValueIsNull:=args.DbValueisNull, _
                                                                       hostValueIsEmpty:=args.HostValueisEmpty, hostValueIsNull:=args.HostValueisNull, _
                                                                       msglog:=args.Msglog)
                Return
            End If


        End If


    End Sub
End Class
'*****************************************************************************************
'****** Module for simple Replication between OTDB and a Table in a ExcelSheet

Module XLSXChangeMgr

    Private _DataAreaStore As New XLSDataAreaStore
    Public WorkbookBeforeCloseEvent As AppEvents_WorkbookBeforeCloseEventHandler

    '***** DataStore attach
    Public Function AttachWorkbook(ByRef workbook As Workbook) As Boolean

        'Add an event handler for the WorkbookBeforeClose Event of the
        'Application object.
        WorkbookBeforeCloseEvent = New AppEvents_WorkbookBeforeCloseEventHandler(AddressOf BeforeBookClose)
        AddHandler workbook.Application.WorkbookBeforeClose, WorkbookBeforeCloseEvent

        Return _DataAreaStore.Attach(workbook)
    End Function
    '***** DataStore de-attach
    Public Function DeattachWorkbook(ByRef workbook As Workbook) As Boolean
        Try
            RemoveHandler workbook.Application.WorkbookBeforeClose, WorkbookBeforeCloseEvent
        Catch ex As Exception

        End Try

        Return _DataAreaStore.Deattach(workbook)
    End Function
    '***** datastore get DataAreas
    Public Function getDataAreas(ByRef workbook As Workbook, Optional ByVal refresh As Boolean = False) As List(Of XLSDataArea)
        If Not _DataAreaStore.IsWorkbookAttached(workbook) Then
            AttachWorkbook(workbook)
        End If
        Return _DataAreaStore.GetDataAreas(workbook, refresh)
    End Function
    '***** datastore max DataArea constant
    Public Function maxDataAreas() As UShort
        Return _DataAreaStore.maxDataAreas
    End Function
    Public Function addDataArea(ByRef workbook As Workbook, ByRef dataarea As XLSDataArea) As Boolean
        If Not _DataAreaStore.IsWorkbookAttached(workbook) Then
            AttachWorkbook(workbook)
        End If
        Return _DataAreaStore.addDataArea(workbook, dataarea)
    End Function
    Public Function saveDataAreas(ByRef workbook As Workbook) As Boolean
        If Not _DataAreaStore.IsWorkbookAttached(workbook) Then
            AttachWorkbook(workbook)
        End If
        Return _DataAreaStore.saveDataAreas(workbook)
    End Function

    Private Sub BeforeBookClose(ByVal Wb As Excel.Workbook, ByRef Cancel As Boolean)
        'This is called when you choose to close the workbook in Excel.
        'The event handlers are removed, and then the workbook is closed 
        'without saving changes.
        _DataAreaStore.Deattach(Wb)
        RemoveHandler Wb.Application.WorkbookBeforeClose, WorkbookBeforeCloseEvent
        Wb.Saved = True 'Set the dirty flag to true so there is no prompt to save.
    End Sub

    ''' <summary>
    ''' replicate OnTrack Database with the excel data area
    ''' </summary>
    ''' <param name="dataarea"></param>
    ''' <param name="xcmd"></param>
    ''' <param name="fullReplication"></param>
    ''' <param name="silent"></param>
    ''' <param name="workspaceID"></param>
    ''' <param name="workerthread"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReplicateDataArea(dataarea As XLSDataArea, _
                                        Optional xcmd As otXChangeCommandType = otXChangeCommandType.Read, _
                                        Optional ByVal fullReplication As Boolean = False, _
                                        Optional ByVal silent As Boolean = True, _
                                        Optional ByVal workspaceID As String = Nothing, _
                                        Optional ByVal domainid As String = Nothing, _
                                        Optional ByRef workerthread As BackgroundWorker = Nothing) As Boolean


        Dim aSelection As Excel.Range
        Dim aValue, aNewValue As Object
        Dim progress As Long = 0
        Dim maximum As ULong = 0
        Dim column As Long
        Dim xchangeresult As Boolean
        Dim previousDomainid As String

        Dim aLogColumn As Long?
        Dim aStatusColumn As Long?
        Dim aTimestampColumn As Long?

        Try

            If xcmd = otXChangeCommandType.Read Then
                If Not CurrentSession.RequireAccessRight(accessRequest:=otAccessRight.[ReadOnly], domainID:=domainid) Then
                    Call CoreMessageHandler(message:="Access right READONLY could not be granted to this user", procedure:="replicate", showmsgbox:=True, _
                                            messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If
            Else
                If Not CurrentSession.RequireAccessRight(accessRequest:=otAccessRight.ReadUpdateData, domainID:=domainid) Then
                    Call CoreMessageHandler(message:="Access right READ UPDATE could not be granted to this user", procedure:="replicate", showmsgbox:=True, _
                                            messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If
            End If

            ''' step out in edit mode
            ''' 
            If modXLSHelper.IsEditing() Then
                Globals.ThisAddIn.Application.SendKeys("{Enter}")
                If modXLSHelper.IsEditing() Then
                    Call CoreMessageHandler(message:="the cell in [" & Globals.ThisAddIn.Application.ActiveWorkbook.Name & "!" & _
                                            CType(Globals.ThisAddIn.Application.ActiveSheet, Excel.Worksheet).Name & "]" & _
                                             CType(Globals.ThisAddIn.Application.ActiveCell, Excel.Range).Address.ToString & " is being edited " & vbLf & " - please leave cell before starting operation", _
                                             procedure:="replicate", showmsgbox:=True, messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If
            End If


            '*** check on Domain -> switch if necessary
            '***
            If String.IsNullOrWhiteSpace(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim aDomain As Domain = Domain.Retrieve(id:=domainid)
            If aDomain Is Nothing Then
                Call CoreMessageHandler(message:="domain by id '" & domainid & "' is not defined", procedure:="replicate", _
                                        showmsgbox:=True, _
                                        messagetype:=otCoreMessageType.ApplicationError)
                Return False
            Else
                If CurrentSession.CurrentDomainID <> domainid Then
                    previousDomainid = CurrentSession.CurrentDomainID
                    CurrentSession.SwitchToDomain(domainid)
                End If
            End If

            '*** check on workspaceID
            '***
            If String.IsNullOrWhiteSpace(workspaceID) Then workspaceID = CurrentSession.CurrentWorkspaceID
            Dim aWorkspace As Workspace = Workspace.Retrieve(id:=workspaceID)

            If aWorkspace Is Nothing Then
                Call CoreMessageHandler(message:="workspaceID '" & workspaceID & "' is not defined", procedure:="replicate", _
                                        showmsgbox:=True, _
                                        messagetype:=otCoreMessageType.ApplicationError)
                '' switch back
                If previousDomainid IsNot Nothing AndAlso previousDomainid <> CurrentSession.CurrentDomainID Then
                    CurrentSession.SwitchToDomain(previousDomainid)
                End If
                Return False
            End If


            'progress
            If Not workerthread Is Nothing Then
                workerthread.ReportProgress(0, "#1 checking data area")
            End If

            '** xconfig 
            If dataarea.XConfigName = "" Then
                Call CoreMessageHandler(message:="xconfig name must not be empty", _
                                        messagetype:=otCoreMessageType.ApplicationError, _
                                        procedure:="xlsChangeMgr.replicate")
                '' switch back
                If previousDomainid IsNot Nothing AndAlso previousDomainid <> CurrentSession.CurrentDomainID Then
                    CurrentSession.SwitchToDomain(previousDomainid)
                End If
                Return False
            ElseIf dataarea.XConfig Is Nothing Then
                Call CoreMessageHandler(message:="could not load xchange config by name '" & dataarea.XConfigName & "'", _
                                        messagetype:=otCoreMessageType.ApplicationError, _
                                        procedure:="replicate")
                '' switch back
                If previousDomainid IsNot Nothing AndAlso previousDomainid <> CurrentSession.CurrentDomainID Then
                    CurrentSession.SwitchToDomain(previousDomainid)
                End If
                Return False
            Else
                ''' refresh the objects in the domain
                ''' make sure they are loaded in the repository
                dataarea.XConfig.RefreshObjects(domainid)
            End If

            '** datarange
            If dataarea.DataRange Is Nothing Then
                If dataarea.DataRangeAddress IsNot Nothing AndAlso dataarea.DataRangeAddress <> "" Then
                    Try
                        dataarea.DataRange = Globals.ThisAddIn.Application.Range(dataarea.DataRangeAddress)
                    Catch ex As Exception
                        Call CoreMessageHandler(message:="data range with address " & dataarea.DataRangeAddress & " couldn't be found", _
                                                messagetype:=otCoreMessageType.ApplicationError, procedure:="XLSXchangeMGr.replicate")
                        '' switch back
                        If previousDomainid IsNot Nothing AndAlso previousDomainid <> CurrentSession.CurrentDomainID Then
                            CurrentSession.SwitchToDomain(previousDomainid)
                        End If
                        Return False
                    End Try
                Else
                    Call CoreMessageHandler(message:="data range in data area " & dataarea.Name & "' has no address ", _
                                               messagetype:=otCoreMessageType.ApplicationError, procedure:="XLSXchangeMGr.replicate")
                    '' switch back
                    If previousDomainid IsNot Nothing AndAlso previousDomainid <> CurrentSession.CurrentDomainID Then
                        CurrentSession.SwitchToDomain(previousDomainid)
                    End If
                    Return False
                End If
            End If


            '** headerids
            '**
            If dataarea.HeaderIDRange Is Nothing Then
                If Not String.IsNullOrWhiteSpace(dataarea.HeaderIDAddress) Then
                    Try
                        dataarea.HeaderIDRange = Globals.ThisAddIn.Application.Range(dataarea.HeaderIDAddress)
                    Catch ex As Exception
                        Call CoreMessageHandler(message:="header id range with address " & dataarea.HeaderIDAddress & " couldn't be found", messagetype:=otCoreMessageType.ApplicationError, procedure:="XLSXChangeMgr.replicate")
                        '' switch back
                        If previousDomainid IsNot Nothing AndAlso previousDomainid <> CurrentSession.CurrentDomainID Then
                            CurrentSession.SwitchToDomain(previousDomainid)
                        End If
                        Return False
                    End Try
                End If
            End If

            '*** TODO:here the ordinal should be rearranged to the headerids
            '*** 

            '*** get the dynamic  IDs from the header area
            If dataarea.XConfig.AllowDynamicEntries Then
                If Not dataarea.AddHeaderIDs2XConfig(XCMD:=xcmd) Then
                    Call CoreMessageHandler(message:="header id range with address " & dataarea.HeaderIDAddress _
                                            & " couldnot be added to xconfig with name '" & dataarea.XConfigName & "'", _
                                            messagetype:=otCoreMessageType.ApplicationError, procedure:="xlsXChangeMgr.replicate")
                    '' switch back
                    If previousDomainid IsNot Nothing AndAlso previousDomainid <> CurrentSession.CurrentDomainID Then
                        CurrentSession.SwitchToDomain(previousDomainid)
                    End If
                    Return False
                End If
            End If

            '*** define the keys
            Dim keyordinals As New List(Of Ordinal)
            For Each headerID As String In dataarea.KeyIDs
                Dim value As Object = dataarea.GetHeaderIDColumn(headerID)
                Dim anordinal As New Ordinal(value)
                keyordinals.Add(anordinal)
            Next

            '*** get the LogColumn
            If Not String.IsNullOrWhiteSpace(dataarea.TransactionLogID) Then
                aLogColumn = dataarea.GetHeaderIDColumn(dataarea.TransactionLogID)
            End If
            '*** get the Timestamp Column
            If Not String.IsNullOrWhiteSpace(dataarea.TimestampID) Then
                aTimestampColumn = dataarea.GetHeaderIDColumn(dataarea.TimestampID)
            End If
            '*** get the Status Column
            If Not String.IsNullOrWhiteSpace(dataarea.StatusID) Then
                aStatusColumn = dataarea.GetHeaderIDColumn(dataarea.StatusID)
            End If

            '*** check on selection
            If Not fullReplication Then
                If String.IsNullOrWhiteSpace(dataarea.SelectionID) Then
                    Call CoreMessageHandler(message:="no selection header id provided although partial replication called", procedure:="replicate", messagetype:=otCoreMessageType.ApplicationError)
                    '' switch back
                    If previousDomainid IsNot Nothing AndAlso previousDomainid <> CurrentSession.CurrentDomainID Then
                        CurrentSession.SwitchToDomain(previousDomainid)
                    End If
                    Return False
                Else
                    If dataarea.HeaderIDRange.Find(What:=dataarea.SelectionID, MatchCase:=False) Is Nothing Then
                        Call CoreMessageHandler(message:="selection header id '" & dataarea.SelectionID & _
                                                "' could not be found in header id range " & dataarea.HeaderIDAddress, _
                                                messagetype:=otCoreMessageType.ApplicationError, procedure:="XLSXChangeMgr.replicate")
                        '' switch back
                        If previousDomainid IsNot Nothing AndAlso previousDomainid <> CurrentSession.CurrentDomainID Then
                            CurrentSession.SwitchToDomain(previousDomainid)
                        End If
                        Return False
                    End If
                    ''' get selection
                    ''' get the selection as range
                    ''' 
                    aSelection = dataarea.GetSelectionAsRange(silent:=silent)
                End If
            Else
                '** select the full selection
                aSelection = dataarea.DataRange.Worksheet.Range(dataarea.DataRange.Worksheet.Cells(dataarea.DataRange.Rows(1).row, CInt(keyordinals(0).Value)), _
                                                                dataarea.DataRange.Worksheet.Cells(dataarea.DataRange.Rows(1).row + dataarea.DataRange.Rows.Count - 1, CInt(keyordinals(0).Value)))

                Diagnostics.Debug.WriteLine(aSelection.Rows.Count & " rows selected in " & aSelection.Address & " of " & dataarea.DataRange.Address & " (" & dataarea.DataRangeAddress & ")")
                Globals.ThisAddIn.Application.StatusBar = aSelection.Rows.Count & " rows selected in " & aSelection.Address & " of " & dataarea.DataRange.Address & " (" & dataarea.DataRangeAddress & ")"

                ''' confirm the inbound-full replication
                If xcmd = otXChangeCommandType.CreateUpdate Then
                    Telerik.WinControls.RadMessageBox.SetThemeName("TelerikMetroBlue")
                    Dim aresult As System.Windows.Forms.DialogResult = _
                        Telerik.WinControls.RadMessageBox.Show(text:="ATTENTION !" & vbLf & "Are you sure to replicate all rows from excel to database - overwrite or create new data objects ?", _
                                                                  caption:="Please confirm", _
                                                                  icon:=Telerik.WinControls.RadMessageIcon.Question, _
                                                                  buttons:=System.Windows.Forms.MessageBoxButtons.YesNo, _
                                                                  defaultButton:=System.Windows.Forms.MessageBoxDefaultButton.Button2)
                    If aresult <> System.Windows.Forms.DialogResult.Yes AndAlso aresult <> System.Windows.Forms.DialogResult.OK Then
                        Return False
                    End If
                End If

            End If

            If aSelection Is Nothing Then
                Call CoreMessageHandler(message:="No selection could be found", messagetype:=otCoreMessageType.ApplicationError, _
                                        procedure:="XLSXChangeMgr.replicate")
                '' switch back
                If previousDomainid IsNot Nothing AndAlso previousDomainid <> CurrentSession.CurrentDomainID Then
                    CurrentSession.SwitchToDomain(previousDomainid)
                End If
                Return False
            End If

            Globals.ThisAddIn.Application.ScreenUpdating = False
            Globals.ThisAddIn.Application.EnableEvents = False

            '*** save the Attributes
            Dim aXBag As New ExcelXBag(dataarea.XConfig)
            Dim aXEnvelope As XEnvelope = aXBag.AddEnvelope(1) ' only one Envelope -> reuse
            Dim aMsgLog As New BusinessObjectMessageLog(Nothing)
            Dim anEntryList As List(Of XChangeObjectEntry) = dataarea.XConfig.GetObjectEntries
            '** put the level in the mapping
            Dim anAttributeLevel As XChangeObjectEntry = dataarea.XConfig.GetEntryByXID(XID:="OTLIV4")
            Dim theDomainXID As String = CurrentSession.Objects.GetObjectDefinition(id:=Domain.ConstObjectID).GetEntryDefinition(entryname:=Domain.ConstFNDomainID).XID

            '*** operate on the outline -> makes only sense on a Read !
            '***
            '***
            If fullReplication AndAlso xcmd = otXChangeCommandType.Read AndAlso dataarea.XConfig.Outline IsNot Nothing Then

                '*** get the outline enumeration -> dynmaic
                If workerthread IsNot Nothing Then workerthread.ReportProgress(0, "#2 clean up outline")
                ''' set the domain for the outline
                dataarea.XConfig.Outline.DomainID = domainid
                ''' clean up
                dataarea.XConfig.Outline.CleanUpRevision()
                ' the row
                Dim i As Long = 0

                '*** get the outline enumeration -> dynmaic
                If Not workerthread Is Nothing Then workerthread.ReportProgress(0, "#3 generating outline")

                Dim outLineList As List(Of XOutlineItem) = dataarea.XConfig.Outline.ToList
                If maximum = 0 Then maximum = outLineList.Count

                '** workerthread progress
                If Not workerthread Is Nothing Then workerthread.ReportProgress(0, "#3 outline generated")

                '*** step through all lines in the outline 
                '*** 
                For Each item As XOutlineItem In outLineList
                    i += 1

                    Dim aRow As Excel.Range = dataarea.DataRange.Rows(i)
                    '** progress
                    If Not workerthread Is Nothing Then
                        progress += 1
                        workerthread.ReportProgress((progress / maximum) * 100, "#4 replicated progress: " & String.Format("{0:0%}", (progress / maximum)))
                    End If

                    '** put keys in map
                    aXEnvelope.Clear()
                    aMsgLog = aXEnvelope.MessageLog
                    aMsgLog.Clear()
                    '* set context
                    aXEnvelope.ContextIdentifier = dataarea.WorkbookName & ":" & dataarea.Name
                    aXEnvelope.TupleIdentifier = aRow.Address.ToString

                    '* add the domain
                    aXEnvelope.AddSlotByXID(xid:=theDomainXID, value:=domainid, isHostValue:=True, extendXConfig:=True)
                    ' add the Level Attribute
                    If anAttributeLevel IsNot Nothing Then
                        aXEnvelope.AddSlotbyXEntry(entry:=anAttributeLevel, value:=item.Level, isHostValue:=True)
                    End If

                    '** add the ordinals to the envelope
                    For Each key As XOutlineItem.OutlineKey In item.keys
                        aXEnvelope.AddSlotByXID(xid:=key.ID, value:=key.Value, isHostValue:=True)
                    Next

                    ''' run XCHANGE
                    '''
                    Dim result As Boolean = aXEnvelope.RunXPreCheck(msglog:=aMsgLog)
                    Dim aStatusItem As Commons.StatusItem = aMsgLog.GetHighesStatusItem(ConstStatusType_XEnvelope)
                    If aStatusItem IsNot Nothing AndAlso aStatusItem.Aborting Then
                        xchangeresult = False
                    Else
                        result = aXEnvelope.RunXChange(msglog:=aMsgLog)
                        aStatusItem = aMsgLog.GetHighesStatusItem(ConstStatusType_XEnvelope)
                        If aStatusItem IsNot Nothing AndAlso aStatusItem.Aborting Then
                            xchangeresult = False
                        Else
                            xchangeresult = result
                        End If
                    End If

                    If xchangeresult Then
                        ''' 
                        ''' update output
                        ''' 
                        For Each aSlot As XSlot In aXEnvelope
                            If aSlot.IsXChanged And Not aSlot.IsEmpty AndAlso aSlot.Ordinal IsNot Nothing Then
                                aNewValue = aSlot.HostValue
                                column = CLng(aSlot.Ordinal.Value)
                                If column > 0 And column < 65536 Then
                                    If CType(aRow.Cells(1, column), Range).HasFormula Then
                                        aValue = CType(aRow.Cells(1, column), Range).Text
                                    Else
                                        aValue = CType(aRow.Cells(1, column), Range).Value
                                    End If
                                    '*substitute
                                    If aRow.Cells(1, column).HasFormula OrElse (Not IsNull(aNewValue) AndAlso CStr(aNewValue) <> CStr(aValue)) Then
                                        aRow.Cells(1, column).Value = aNewValue
                                    End If
                                End If
                            End If
                        Next

                        ''' update additional status fields
                        ''' 
                        If aTimestampColumn.HasValue Then
                            aRow.Cells(1, aTimestampColumn.Value).Value = Converter.DateTime2LocaleDateTimeString(aXEnvelope.ProcessedTimestamp)
                        End If
                        If aStatusColumn.HasValue Then
                            Dim aStatus As Commons.StatusItem = aMsgLog.GetHighesStatusItem(statustype:=ConstStatusType_XEnvelope)
                            If aStatus IsNot Nothing Then
                                aRow.Cells(1, aStatusColumn.Value).Value = aStatus.Code
                                If aStatus.FormatBGColor IsNot Nothing Then aRow.Cells(1, aStatusColumn.Value).Interior.Color = aStatus.FormatBGColor
                                If aStatus.FormatFGColor IsNot Nothing Then CType(aRow.Cells(1, aStatusColumn.Value), Excel.Range).Font.Color = aStatus.FormatFGColor
                            End If
                        End If
                        If aLogColumn.HasValue Then
                            Dim height As Object = CType(aRow.Cells(1, aLogColumn.Value), Range).RowHeight
                            Dim Text As String = aMsgLog.MessageText
                            If Text IsNot Nothing Then
                                With CType(aRow.Cells(1, aLogColumn.Value), Range)
                                    .Value = Text
                                    .Font.Size = 6
                                    .Font.Bold = False
                                    .WrapText = True
                                End With
                                aRow.RowHeight = height
                            End If
                        End If
                        'Update
                        Globals.ThisAddIn.Application.StatusBar = " Updating data area " & dataarea.Name & " in row #" & aRow.Row
                    End If
                Next

                ''' delete surplus rows ?
                ''' 
                Diagnostics.Debug.WriteLine("old size of data area " & aSelection.Rows.Count & " rows <-> new size " & i & " rows")
                Globals.ThisAddIn.Application.StatusBar = "old size of data area " & aSelection.Rows.Count & " rows <-> new size " & i & " rows"
                If aSelection.Rows.Count > i Then
                    Dim m As UShort = dataarea.DataRange.Rows.Count - i
                    Diagnostics.Debug.WriteLine("deleting " & m & " row(s)")
                    Globals.ThisAddIn.Application.StatusBar = "deleting " & m & " row(s)"
                    For n As UShort = 1 To m
                        Diagnostics.Debug.WriteLine("deleting row " & CType(dataarea.DataRange.Rows(i + 1), Range).Address)
                        Globals.ThisAddIn.Application.StatusBar = "deleting row " & CType(dataarea.DataRange.Rows(i + 1), Range).Address
                        Dim lastRow As Range = dataarea.DataRange.Rows(i + 1) 'take the next
                        lastRow.Delete(Shift:=Excel.XlDeleteShiftDirection.xlShiftUp)
                    Next
                End If


                '''
                ''' set the dataarea range and reset it if necessary
                ''' 

                Dim aDataRange As Range = dataarea.DataRange.Worksheet.Range( _
                    dataarea.DataRange.Worksheet.Cells(dataarea.DataRange.Rows(1).row, 1), _
                    dataarea.DataRange.Worksheet.Cells(dataarea.DataRange.Rows(1).row + i - 1, _
                                                       CType(dataarea.HeaderIDRange.Columns(dataarea.HeaderIDRange.Columns.Count), Range).Column))
                ''' 
                ''' change the named range
                If NameExistsinWorkbook(aDataRange.Worksheet.Parent, dataarea.DataRangeAddress) Then
                    SetXlsParameterRangeByName(dataarea.DataRangeAddress, aDataRange, workbook:=aDataRange.Worksheet.Parent, silent:=True)
                End If

                If NameExistsinWorkbook(aDataRange.Worksheet.Parent, "otdb_parameter_xlsdb_database_range") Then
                    SetXlsParameterValueByName("otdb_parameter_xlsdb_database_range", aDataRange.Address, workbook:=aDataRange.Worksheet.Parent, silent:=True)
                End If

                '''
                ''' set the dataarea range with header and reset it if necessary
                ''' 

                aDataRange = dataarea.DataRange.Worksheet.Range( _
                    dataarea.DataRange.Worksheet.Cells(dataarea.DataRange.Rows(1).row - 1, 1), _
                    dataarea.DataRange.Worksheet.Cells(dataarea.DataRange.Rows(1).row + i - 1, _
                                                       CType(dataarea.HeaderIDRange.Columns(dataarea.HeaderIDRange.Columns.Count), Range).Column))

                If NameExistsinWorkbook(aDataRange.Worksheet.Parent, "otdb_parameter_xlsdbH_database_range") Then
                    SetXlsParameterValueByName("otdb_parameter_xlsdbH_database_range", aDataRange.Address, workbook:=aDataRange.Worksheet.Parent, silent:=True)
                End If


            Else

                ''' count - aselection.rows.count is not getting the real number of rows bac
                maximum = 0
                For Each aRow As Range In aSelection.Rows
                    maximum += 1
                Next

                ''' hack
                ''' 
                For Each aXObject As XChangeObject In dataarea.XConfig.XChangeobjects.ToList
                    aXObject.XChangeCmd = xcmd
                Next

                For Each aCell As Excel.Range In aSelection.Rows
                    Dim aRow As Excel.Range = aCell.EntireRow
                    '** progress
                    If Not workerthread Is Nothing Then
                        progress += 1
                        workerthread.ReportProgress((progress / maximum) * 100, "#4 replicated progress: " & String.Format("{0:0%}", (progress / maximum)))
                    End If

                    '** put keys in map
                    aXEnvelope = aXBag.AddEnvelope(key:=progress)
                    ''' use the MessageLog of the Envelope
                    aMsgLog = aXEnvelope.MessageLog


                    ''' add the context identifier
                    ''' 
                    aMsgLog.ContextIdentifier = dataarea.WorkbookName & ":" & dataarea.Name
                    aMsgLog.TupleIdentifier = aRow.Address.ToString
                    aXEnvelope.ContextIdentifier = dataarea.WorkbookName & ":" & dataarea.Name
                    aXEnvelope.TupleIdentifier = aRow.Address.ToString



                    '** Add Values only if not Read -> updated
                    If (xcmd <> otXChangeCommandType.Read) Then
                        '* add the domain
                        aXEnvelope.AddSlotByXID(xid:=theDomainXID, value:=domainid, isHostValue:=True, extendXConfig:=True)
                        '* all entries
                        For Each anEntry As XChangeObjectEntry In anEntryList
                            If anEntry.IsXChanged Then
                                Dim aColumn As Long = CLng(anEntry.Ordinal.Value)
                                If aColumn > 0 And aColumn < 65536 Then
                                    If CType(aRow.Cells(1, aColumn), Range).HasFormula Then
                                        aValue = CType(aRow.Cells(1, aColumn), Range).Text
                                    Else
                                        aValue = CType(aRow.Cells(1, aColumn), Range).Value
                                    End If
                                    aXEnvelope.AddSlotByXID(xid:=anEntry.XID, value:=aValue, isHostValue:=True)
                                End If
                            End If
                        Next
                    Else
                        '* add the domain
                        aXEnvelope.AddSlotByXID(xid:=theDomainXID, value:=domainid, isHostValue:=True, extendXConfig:=True)

                        '*** only the key
                        For Each ordinal As Object In keyordinals
                            Dim aColumn As Long = CLng(ordinal)
                            If aColumn > 0 And aColumn < 65536 Then
                                If CType(aRow.Cells(1, aColumn), Range).HasFormula Then
                                    aValue = CType(aRow.Cells(1, aColumn), Range).Text
                                Else
                                    aValue = CType(aRow.Cells(1, aColumn), Range).Value
                                End If

                                If Not Globals.ThisAddIn.Application.WorksheetFunction.IsError(aValue) Then
                                    aXEnvelope.SetSlotValue(ordinal:=ordinal, value:=aValue)
                                Else
                                    Call CoreMessageHandler(showmsgbox:=True, message:="key for data area '" & dataarea.Name & "' could not be retrieved from column " & ordinal.ToString, _
                                                            break:=False, procedure:="modxlsXchangeMgr.Replicate")
                                End If
                            End If

                        Next
                    End If


                    ''' run XCHANGE
                    '''
                    Dim result As Boolean = aXEnvelope.RunXPreCheck(msglog:=aMsgLog)
                    Dim aStatusItem As Commons.StatusItem = aMsgLog.GetHighesStatusItem(ConstStatusType_XEnvelope)
                    If aStatusItem IsNot Nothing AndAlso aStatusItem.Aborting Then
                        xchangeresult = False
                    Else
                        result = aXEnvelope.RunXChange(msglog:=aMsgLog)
                        aStatusItem = aMsgLog.GetHighesStatusItem(ConstStatusType_XEnvelope)
                        If aStatusItem IsNot Nothing AndAlso aStatusItem.Aborting Then
                            xchangeresult = False
                        Else
                            xchangeresult = result
                        End If
                    End If


                    If xchangeresult Then
                        '''
                        ''' update row in excel with the output
                        ''' 
                        For Each aSlot As XSlot In aXEnvelope
                            If aSlot.IsXChanged And Not aSlot.IsEmpty Then
                                aNewValue = aSlot.HostValue
                                column = aSlot.Ordinal.Value
                                If column > 0 AndAlso column < 65536 Then
                                    If CType(aRow.Cells(1, column), Range).HasFormula Then
                                        aValue = CType(aRow.Cells(1, column), Range).Text
                                    Else
                                        aValue = CType(aRow.Cells(1, column), Range).Value
                                    End If

                                    If aRow.Cells(1, column).HasFormula OrElse (Not IsNull(aNewValue) And CStr(aNewValue) <> CStr(aValue)) Then
                                        ' update
                                        aRow.Cells(1, column).Value = aNewValue

                                    End If
                                End If
                            End If
                        Next

                        'Update
                        Globals.ThisAddIn.Application.StatusBar = " Updating data area " & dataarea.Name & " in row #" & aRow.Row
                    End If
                    'Update
                    Globals.ThisAddIn.Application.StatusBar = " Updated data area " & dataarea.Name & " rows " & aRow.Row


                    ''' update additional status fields
                    ''' 
                    If aTimestampColumn.HasValue Then
                        If xchangeresult Then
                            aRow.Cells(1, aTimestampColumn.Value).Value = Converter.DateTime2LocaleDateTimeString(aXEnvelope.ProcessedTimestamp)
                        Else
                            aRow.Cells(1, aTimestampColumn.Value).Value = Converter.DateTime2LocaleDateTimeString(DateTime.Now)
                        End If

                    End If
                    If aStatusColumn.HasValue Then
                        Dim aStatus As Commons.StatusItem = aMsgLog.GetHighesStatusItem(statustype:=ConstStatusType_XEnvelope)
                        If aStatus IsNot Nothing Then
                            aRow.Cells(1, aStatusColumn.Value).Value = aStatus.Code
                            If aStatus.FormatBGColor IsNot Nothing Then aRow.Cells(1, aStatusColumn.Value).Interior.Color = aStatus.FormatBGColor
                            If aStatus.FormatFGColor IsNot Nothing Then CType(aRow.Cells(1, aStatusColumn.Value), Excel.Range).Font.Color = aStatus.FormatFGColor
                        Else
                            aRow.Cells(1, aStatusColumn.Value).Value = "n/a"
                        End If
                    End If
                    If aLogColumn.HasValue Then
                        Dim height As Object = CType(aRow.Cells(1, aLogColumn.Value), Range).RowHeight
                        Dim Text As String = aMsgLog.MessageText
                        If Text Is Nothing And Not xchangeresult Then
                            Text = "UID not found - invalid ???"
                        End If
                        With CType(aRow.Cells(1, aLogColumn.Value), Range)
                            .Value = Text
                            .Font.Size = 6
                            .Font.Bold = False
                            .WrapText = True
                        End With
                        aRow.RowHeight = height
                    End If
                Next aCell
            End If

            '' switch back
            If previousDomainid IsNot Nothing AndAlso previousDomainid <> CurrentSession.CurrentDomainID Then
                CurrentSession.SwitchToDomain(previousDomainid)
            End If

            Globals.ThisAddIn.Application.EnableEvents = True
            Globals.ThisAddIn.Application.ScreenUpdating = True

            Return True


        Catch ex As Exception
            CoreMessageHandler(showmsgbox:=True, exception:=ex, procedure:="XLSXChangeMgr.Replicate")
            '' switch back
            If previousDomainid IsNot Nothing AndAlso previousDomainid <> CurrentSession.CurrentDomainID Then
                CurrentSession.SwitchToDomain(previousDomainid)
            End If

            Globals.ThisAddIn.Application.EnableEvents = True
            Globals.ThisAddIn.Application.ScreenUpdating = True

            Return False
        End Try
    End Function
End Module
