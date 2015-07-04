Imports OnTrack.UI
Imports Telerik.WinControls.UI
Imports System.Drawing
Imports System.Windows.Forms
Imports OnTrack.Database
Imports OnTrack.Commons
Imports System.ComponentModel
Imports Telerik.WinControls
Imports OnTrack.Core

''' <summary>
''' DataGridView Control for Objects
''' </summary>
''' <remarks></remarks>
Public Class UIControlDataGridView
    Inherits UserControl
    Implements iUIStatusSender

    ''' <summary>
    ''' Types of rows
    ''' </summary>
    ''' <remarks></remarks>
    Enum RowType
        NotBound
        DataObjectBound
    End Enum
    ''' <summary>
    ''' Types of columns
    ''' </summary>
    ''' <remarks></remarks>
    Enum ColumnType
        NotBound
        ObjectEntry
    End Enum
    ''' <summary>
    ''' data attached to the row tag
    ''' </summary>
    ''' <remarks></remarks>
    Class TagRowData
        Friend Type As RowType = UIControlDataGridView.RowType.NotBound
        Friend ReferenceRowNo As ULong?
        Friend DataObject As ormBusinessObject
    End Class
    ''' <summary>
    ''' data attached to the row tag
    ''' </summary>
    ''' <remarks></remarks>
    Class TagColumnData
        Friend Type As ColumnType = ColumnType.NotBound
        Friend ObjectEntry As iormObjectEntryDefinition
    End Class
    ''' <summary>
    ''' inner variables
    ''' </summary>
    ''' <remarks></remarks>

    Protected Friend WithEvents RadGridView As New RadGridView ''' embedded RadGridView -> new since we have it as friend

    Private WithEvents _modeltable As ormModelTable 'if the model is a model table
    Private WithEvents _queriedEnumeration As iormQueriedEnumeration 'if the model is an queried enumeration
    Private WithEvents _statuslabel As RadLabelElement
    Private WithEvents _ProgressPictureBox As System.Windows.Forms.PictureBox
    Private _pwfieldname As String = String.Empty
    Private WithEvents _menuitems As New RadItemOwnerCollection ' collection of RadMenuItems Menus to be applied on lines
    Private WithEvents _controller As New MVDataObjectController ' Form Controller

    Private _IsDynamicInitialized As Boolean = False
    Private _isInitialized As Boolean = False


    ''' <summary>
    ''' Events
    ''' </summary>
    ''' <remarks></remarks>
    Public Event OnIssueMessage(sender As Object, e As UIStatusMessageEventArgs) Implements iUIStatusSender.OnIssueMessage


    '' Status Text Changed
    Public Event OnStatusTextChanged As EventHandler(Of UIControlDataGridViewEventArgs)
    '' dataobject selectd
    Public Event OnSelectedDataObject As EventHandler(Of UIControlDataGridViewEventArgs)

    ''' <summary>
    ''' Constants
    ''' </summary>
    ''' <remarks></remarks>

    Public Const UITxtSaveUILayout As String = "Save UI Layout"
    Public Const UITxtLoadUILayout As String = "Load UI Layout"
    Public Const UITxtAdjustAllColumns As String = "Adjust all columns in size"
    Public Const UITxtAdjustColumn As String = "Adjust column size"
    Public Const UITxtNoRowAddable As String = "no right to add a object of type "
    Public Const UiTxtNoDataObject As String = "row not a dataobject -refresh view"

#Region "Properties"
    ''' <summary>
    ''' returns the DataObject of a Row no of the model
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private ReadOnly Property DataObject(Optional rowno As ULong? = Nothing) As iormRelationalPersistable
        Get

            If Not rowno.HasValue Then
                Dim aTag = TryCast(RadGridView.CurrentRow.Tag, TagRowData)
                If aTag IsNot Nothing Then
                    rowno = aTag.ReferenceRowNo
                Else
                    If _controller IsNot Nothing Then Return _controller.Dataobject
                End If
            End If
            If _modeltable IsNot Nothing Then
                Return _modeltable.DataObject(rowno)
            ElseIf _queriedEnumeration IsNot Nothing Then
                Return _queriedEnumeration.GetObject(rowno)
            End If
        End Get
    End Property
    ''' <summary>
    ''' return the object definition id of the underlying data object 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property DataObjectID() As String
        Get
            If _modeltable IsNot Nothing Then
                Return _modeltable.DataObjectID
            ElseIf _queriedEnumeration IsNot Nothing Then
                If _queriedEnumeration.AreObjectsEnumerated Then Return _queriedEnumeration.GetObjectDefinition.ID
            End If
            Return Nothing
        End Get
    End Property
    ''' <summary>
    ''' Gets or sets the controller.
    ''' </summary>
    ''' <value>The controller.</value>
    Public Property Controller() As MVDataObjectController
        Get
            Return _controller
        End Get
        Set(value As MVDataObjectController)
            _controller = value
        End Set
    End Property

    ''' <summary>
    ''' context specific menu items to be called on rows
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Menuitems As RadItemOwnerCollection
        Get
            If _menuitems Is Nothing Then _menuitems = New RadItemOwnerCollection
            Return _menuitems
        End Get
    End Property
    ''' <summary>
    ''' Gets or sets the progress picture box.
    ''' </summary>
    ''' <value>The progress picture box.</value>
    Public ReadOnly Property ProgressPictureBox() As PictureBox
        Get
            If _ProgressPictureBox Is Nothing Then
                '
                'ProgressPictureBox
                '
                _ProgressPictureBox = New System.Windows.Forms.PictureBox
                CType(Me._ProgressPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
                With _ProgressPictureBox
                    .Enabled = False
                    .Dock = System.Windows.Forms.DockStyle.Fill
                    .Image = My.Resources.Resources.progress_radar
                    '.Location = New System.Drawing.Point(0, 0)
                    .Name = "ProgressPictureBox"
                    .Size = Me.Size
                    .SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
                    .TabIndex = 0
                    .TabStop = False
                End With
                CType(Me._ProgressPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
            End If
            If Me.Controls.Find("ProgressPictureBox", True).Length = 0 Then Me.Controls.Add(_ProgressPictureBox)
            Return Me._ProgressPictureBox
        End Get

    End Property
    ''' <summary>
    ''' data source to bind to, bind also to embedded data boxes with same object name
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <BrowsableAttribute(True), Category("Data"), Description("data source of this data entry box")> _
    Public Property DataSource() As Object
        Get
            If _modeltable IsNot Nothing Then Return _modeltable
            Return Nothing
        End Get
        Set(value As Object)
            If value.GetType Is GetType(ormModelTable) Then
                _modeltable = value
                If RadGridView IsNot Nothing Then RadGridView.DataSource = _modeltable
            ElseIf value.GetType.GetInterfaces.Contains(GetType(iormQueriedEnumeration)) Then
                '' 
                _queriedEnumeration = value

            End If

        End Set
    End Property
    ''' <summary>
    ''' type of the data source
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <BrowsableAttribute(False), Category("Data"), Description("data source type of this data entry box")> _
    Public ReadOnly Property DataSouceType As Type
        Get
            If Me.DataSource IsNot Nothing Then Return Me.DataSource.GetType
            Return Nothing
        End Get

    End Property
    ''' <summary>
    ''' Gets or sets the selected data object.
    ''' </summary>
    ''' <value>The selected data object.</value>
    Public ReadOnly Property SelectedDataObjects As IList(Of ormBusinessObject)
        Get
            Dim alist As New List(Of ormBusinessObject)
            If RadGridView.SelectionMode = GridViewSelectionMode.FullRowSelect Then
                For Each aRow In RadGridView.SelectedRows
                    Dim aDataObject As ormBusinessObject = Me.DataObject(aRow.Index)
                    If aDataObject IsNot Nothing Then alist.Add(aDataObject)
                Next
            End If
            Return alist
        End Get

    End Property

    ''' <summary>
    ''' Gets or sets the status.
    ''' </summary>
    ''' <value>The status.</value>
    Public Property Status() As RadLabelElement
        Get
            Return Me._statuslabel
        End Get
        Set(value As RadLabelElement)
            Me._statuslabel = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the modeltable.
    ''' </summary>
    ''' <value>The modeltable.</value>
    Public Property Modeltable() As ormModelTable
        Get
            Return Me._modeltable
        End Get
        Set(value As ormModelTable)
            Me._modeltable = value
        End Set
    End Property

    ''' <summary>
    ''' return true if data is loaded into Grid
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsLoaded() As Boolean
        Get
            If _modeltable IsNot Nothing Then
                Return _modeltable.IsLoaded
            Else
                Return False
            End If
        End Get
    End Property

#End Region

    ''' <summary>
    ''' constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New()

    End Sub


    ''' <summary>
    ''' Initialize Components
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub StaticInitializeComponents()

        If _isInitialized Then Return

        ''' create embedded radgridview
        If RadGridView Is Nothing Then RadGridView = New RadGridView()
        Me.SuspendLayout()
        Me.Controls.Clear()

        ''' init the radgridview
        ''' 
        With RadGridView
            .BeginInit()
            .EnableCustomFiltering = False
            .EnableCustomGrouping = False
            .EnableCustomSorting = False
            .EnableHotTracking = False

            .MasterTemplate.ShowHeaderCellButtons = True
            .MasterTemplate.ShowFilteringRow = False
            .MasterTemplate.EnableHierarchyFiltering = True
            .MasterTemplate.EnableCustomFiltering = False

            .EnableFiltering = True
            .MasterTemplate.EnableFiltering = True
            .EnableGrouping = False
            .EnableSorting = True
            .EnableAlternatingRowColor = True
            .AllowAddNewRow = False

            .Dock = DockStyle.Fill
            '.ThemeName = ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
            .AutoSizeColumnsMode = True
            .Name = "RadGridView" & Me.Name
            .EndInit()
        End With

        Me.Controls.Add(RadGridView)
        Me.ResumeLayout(False)
        Me.PerformLayout()

        ''' Menu items
        ''' 
        Dim separator As RadMenuSeparatorItem = New RadMenuSeparatorItem() With {.Name = "Seperator"}
        Me.Menuitems.Add(separator)

        Dim menuItemCS As New RadMenuItem(UITxtAdjustColumn) With {.Name = UITxtAdjustColumn}
        AddHandler menuItemCS.Click, AddressOf OnMenuItemClick_AdjustColumnSize
        Me.Menuitems.Add(menuItemCS)

        Dim menuItemCSa As New RadMenuItem(UITxtAdjustAllColumns) With {.Name = UITxtAdjustAllColumns}
        AddHandler menuItemCSa.Click, AddressOf OnMenuItemClick_AdjustAllColumnSize
        Me.Menuitems.Add(menuItemCSa)

        Dim menuItem1 As New RadMenuItem(UITxtSaveUILayout) With {.Name = UITxtSaveUILayout}
        menuItem1.ForeColor = Color.Red
        AddHandler menuItem1.Click, AddressOf OnMenuItemClick_SaveUILayout
        Me.Menuitems.Add(menuItem1)

        Dim menuItem2 As New RadMenuItem(UITxtLoadUILayout) With {.Name = UITxtLoadUILayout}
        AddHandler menuItem2.Click, AddressOf OnMenuItemClick_LoadUILayout
        Me.Menuitems.Add(menuItem2)

        _isInitialized = True
    End Sub
    ''' <summary>
    ''' Initialize the RadGridView from the queried enumeration
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DynamicInitialize()
        If _IsDynamicInitialized Then Return

        Me.StaticInitializeComponents()

        ''' initialize the grid view
        ''' 
        If _queriedEnumeration IsNot Nothing Then
            Dim aTemplate As New GridViewTemplate
            Dim anEntriesList As New List(Of iormObjectEntryDefinition)

            ''' get the list of the columns by entries
            ''' 
            If _queriedEnumeration.AreObjectsEnumerated Then
                anEntriesList = _queriedEnumeration.GetObjectDefinition.GetOrderedEntries
            End If

            ''' add the columns
            ''' 
            For Each anEntry In anEntriesList
                Dim aColumn = Me.CreateGridViewColumn(anEntry)
                If aColumn IsNot Nothing Then RadGridView.MasterTemplate.Columns.Add(aColumn)
            Next

            _IsDynamicInitialized = True
        End If
    End Sub

    ''' <summary>
    ''' returns the object entry from the current model of the entryname
    ''' </summary>
    ''' <param name="entryname"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetObjectEntry(entryname As String) As iormObjectEntryDefinition
        If _modeltable IsNot Nothing Then
            Return _modeltable.GetObjectEntry(entryname)
        ElseIf _queriedEnumeration IsNot Nothing Then
            Return _queriedEnumeration.GetObjectEntry(entryname)
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' return the formatted column per entrydefinition
    ''' </summary>
    ''' <param name="entrydefinition"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateGridViewColumn(entrydefinition As iormObjectEntryDefinition) As GridViewColumn
        Dim aColumn As GridViewColumn

        ''' define per Datatype
        ''' 

        Select Case entrydefinition.Datatype
            Case otDataType.Bool
                aColumn = New GridViewCheckBoxColumn(fieldName:=entrydefinition.Entryname)
            Case otDataType.Long
                aColumn = New GridViewTextBoxColumn(fieldName:=entrydefinition.Entryname)
                aColumn.TextAlignment = ContentAlignment.MiddleRight
                'Case otDataType.List
            Case otDataType.Text
                aColumn = New GridViewTextBoxColumn(fieldName:=entrydefinition.Entryname)
            Case otDataType.Timestamp
                aColumn = New GridViewDateTimeColumn(fieldName:=entrydefinition.Entryname)
            Case otDataType.Time
                aColumn = New GridViewDateTimeColumn(fieldName:=entrydefinition.Entryname)
            Case otDataType.Date
                aColumn = New GridViewDateTimeColumn(fieldName:=entrydefinition.Entryname)
            Case otDataType.Memo
                aColumn = New GridViewTextBoxColumn(fieldName:=entrydefinition.Entryname)
            Case otDataType.Numeric
            Case Else
                CoreMessageHandler(message:="otDatatype is not defined", argument:=entrydefinition.Datatype, procedure:="UIControlDataGridView.GetGridViewColumn", _
                               messagetype:=otCoreMessageType.InternalError)
                Return Nothing
        End Select



        ''' general formatting
        ''' 
        If aColumn IsNot Nothing Then
            aColumn.HeaderText = entrydefinition.Title
            aColumn.HeaderTextAlignment = ContentAlignment.MiddleCenter
            aColumn.Tag = New TagColumnData With {.Type = ColumnType.ObjectEntry, .ObjectEntry = entrydefinition}
        End If

        Return aColumn
    End Function
    ''' <summary>
    ''' Add a Message to the connected status message
    ''' </summary>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    Private Sub IssueMessage(message As String)
        If _statuslabel IsNot Nothing Then _statuslabel.Text = Date.Now & " : " & message 'for integrated status label
        RaiseEvent OnIssueMessage(Me, New UIStatusMessageEventArgs(message)) 'event for listining status bar
        RaiseEvent OnStatusTextChanged(Me, New UIControlDataGridViewEventArgs(Date.Now & " : " & message))
        Me.Refresh()
    End Sub

#Region "MyEvents"
    ''' <summary>
    ''' Load Event Handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub UiControLDataGridView_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        StaticInitializeComponents()
    End Sub
#End Region
    '''
    ''' Event of the RadGridView
#Region "RadGridViewEvents"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadGridView_Initialized(sender As Object, e As EventArgs) Handles RadGridView.Initialized

    End Sub
    ''' <summary>
    ''' RowSourceNeeded
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadGridView_RowSourceNeeded(sender As Object, e As GridViewRowSourceNeededEventArgs) Handles RadGridView.RowSourceNeeded

    End Sub
    ''' <summary>
    ''' Handle paint Event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadGridView_Paint(sender As Object, e As EventArgs) Handles RadGridView.Paint
        Me.DynamicInitialize()
        HideQRYRowReference()
    End Sub


    ''' <summary>
    ''' Event On Row Adding
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub RadGridView_OnRowAdded(sender As Object, e As GridViewRowEventArgs) Handles RadGridView.UserAddedRow
        Debug.WriteLine(Me.Name & "OnRowAdded")
    End Sub

    ''' <summary>
    ''' Event On Cell Validating
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub UIControlDataGridView_OnCellValidating(sender As Object, e As CellValidatingEventArgs) Handles RadGridView.CellValidating
        If e.ColumnIndex > 0 Then
            If (e.OldValue IsNot Nothing AndAlso e.Value IsNot Nothing AndAlso Not e.Value.Equals(e.OldValue)) _
                OrElse (e.OldValue Is Nothing AndAlso e.Value IsNot Nothing) _
                OrElse (e.OldValue IsNot Nothing AndAlso e.Value Is Nothing) Then


                Dim anObjectEntry As iormObjectEntryDefinition = Me.GetObjectEntry(entryname:=e.Column.FieldName)
                Dim result As otValidationResultType
                Dim msglog As New BusinessObjectMessageLog
                ''' skip if not an objectentry
                ''' 
                If anObjectEntry Is Nothing Then Return
                ''' get the object or create one
                Dim anDataobject As iormRelationalPersistable
                If e.RowIndex >= 0 Then
                    anDataobject = Me.DataObject(e.RowIndex)
                Else
                    Dim anObjectDefinition As ormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=anObjectEntry.Objectname)
                    anDataobject = ot.CurrentSession.DataObjectProvider(objectid:=anObjectDefinition.ID).NewOrmDataObject(type:=anObjectDefinition.ObjectType)
                End If

                If anDataobject Is Nothing Then
                    Me.IssueMessage(UiTxtNoDataObject)
                    result = otValidationResultType.FailedNoProceed

                Else
                    ''' 
                    ''' APPLY THE ENTRY PROPERTIES AND TRANSFORM THE VALUE REQUESTED
                    ''' 
                    Dim outvalue As Object = e.Value ' copy over
                    If Not TryCast(anDataobject, iormInfusable).Normalizevalue(anObjectEntry.Entryname, outvalue) Then
                        CoreMessageHandler(message:="Warning ! Could not normalize value", argument:=outvalue, objectname:=anObjectEntry.Objectname, _
                                            entryname:=anObjectEntry.Entryname, procedure:="UIConTrolDataGridView.CellValidating")

                    End If
                    ''''
                    '''' validate the value
                    ''''
                    result = TryCast(anDataobject, iormValidatable).Validate(anObjectEntry.Entryname, outvalue, msglog)
                End If

                ''' result
                If result = otValidationResultType.FailedNoProceed Then
                    e.Cancel = True ' to cancel
                    If msglog.Count > 0 Then IssueMessage(msglog.MessageText)
                    RadGridView.CurrentCell.BorderBoxStyle = Telerik.WinControls.BorderBoxStyle.SingleBorder
                    RadGridView.CurrentCell.BorderColor = Color.Red
                    RadGridView.CurrentCell.BorderDashStyle = Drawing2D.DashStyle.Solid
                    RadGridView.CurrentCell.BorderThickness = New Padding(0)
                    'Me.CurrentRow.ErrorText = msglog.MessageText

                Else
                    RadGridView.CurrentCell.BorderBoxStyle = Telerik.WinControls.BorderBoxStyle.SingleBorder
                    RadGridView.CurrentCell.BorderColor = Color.LightGreen
                    RadGridView.CurrentCell.BorderDashStyle = Drawing2D.DashStyle.Solid
                    RadGridView.CurrentCell.BorderThickness = New Padding(1)
                End If

            End If

        End If
    End Sub
    ''' <summary>
    ''' Event On Control validated (post validation)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub UIControlDataGridView_OnCellValidated(sender As Object, e As CellValidatedEventArgs) Handles RadGridView.CellValidated
        If sender.Equals(Me) Then
            ' Debug.WriteLine(Me.Name & " OnCellValidated :" & _modeltable.Columns(e.ColumnIndex).ColumnName & " value:" & e.Value)
        End If

    End Sub
    ''' <summary>
    ''' Event On Control validated
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub UIControlDataGridView_OnRowValidating(sender As Object, e As RowValidatingEventArgs) Handles RadGridView.RowValidating
        If e.Row IsNot Nothing AndAlso e.Row.IsModified Then
            Debug.WriteLine(Me.Name & "OnRowValidating")
            e.Cancel = False
        End If
    End Sub
    ''' <summary>
    ''' Event On row validated (post validation)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub UIControlDataGridView_OnRowValidated(sender As Object, e As RowValidatedEventArgs) Handles RadGridView.RowValidated
        If e.Row IsNot Nothing AndAlso e.Row.IsModified Then
            Debug.WriteLine(Me.Name & "OnRowValidated")
        End If
    End Sub
    ''' <summary>
    ''' Event On Row position changed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub OnRowChanged(sender As Object, e As CurrentRowChangedEventArgs) Handles RadGridView.CurrentRowChanged
        If e.CurrentRow IsNot Nothing AndAlso e.CurrentRow.Index >= 0 Then
            Dim aTag As TagRowData = TryCast(e.CurrentRow.Tag, TagRowData)
            If aTag IsNot Nothing Then
                Dim aDataObject As ormBusinessObject
                If aTag.DataObject IsNot Nothing Then
                    aDataObject = aTag.DataObject
                Else
                    aDataObject = TryCast(Me.DataObject(aTag.ReferenceRowNo), ormBusinessObject)
                End If
                ''' notifiy the controller
                If _controller IsNot Nothing Then _controller.Dataobject = aDataObject
                If _modeltable IsNot Nothing Then _modeltable.CurrentRowNo = aTag.ReferenceRowNo
                RaiseEvent OnSelectedDataObject(Me, New UIControlDataGridViewEventArgs(aDataObject))
            End If
        End If
    End Sub

    ''' <summary>
    ''' Event On default values for a new row
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub OnDefaultValueNeeded(sender As Object, e As GridViewRowEventArgs) Handles RadGridView.DefaultValuesNeeded
        For Each aColumn In e.Row.ViewTemplate.Columns
            ''' set the current domain in domain fields
            If aColumn.FieldName = Domain.ConstFNDomainID Then
                If e.Row.Cells.Item(aColumn.Name) IsNot Nothing Then e.Row.Cells.Item(aColumn.Name).Value = CurrentSession.CurrentDomainID
            Else
                Dim anEntry = Me.GetObjectEntry(entryname:=aColumn.FieldName)
                If e.Row.Cells.Item(aColumn.Name) IsNot Nothing AndAlso anEntry IsNot Nothing Then e.Row.Cells.Item(aColumn.Name).Value = anEntry.DefaultValue
            End If
        Next

    End Sub
    ''' <summary>
    ''' Handles the OnContextMenuOpening Event to add or delete context sensitive Events
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub UIControlDataGridView_OnContextMenuOpening(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.ContextMenuOpeningEventArgs) Handles RadGridView.ContextMenuOpening

        ''' Add the Layout Save if we are connected
        ''' 
        If CurrentSession.IsRunning Then

            ''' *** save the Format of the data grid
            ''' 
            If e.ContextMenu.Items.Where(Function(x) x.Name = UITxtSaveUILayout).FirstOrDefault Is Nothing Then
                For Each aMenuitem In Me.Menuitems
                    e.ContextMenu.Items.Add(aMenuitem)
                Next
            End If

        Else
            Dim element1 = e.ContextMenu.Items.Select(Function(x) x.Text = UITxtSaveUILayout)
            If element1 IsNot Nothing Then
                TryCast(element1, RadMenuItem).Enabled = False
            End If
            Dim element2 = e.ContextMenu.Items.Select(Function(x) x.Text = UITxtLoadUILayout)
            If element2 IsNot Nothing Then
                TryCast(element2, RadMenuItem).Enabled = False
            End If
        End If
    End Sub
    ''' <summary>
    ''' MenuItemClick Event handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnMenuItemClick_AdjustAllColumnSize(sender As Object, e As System.EventArgs)
        AdjustAllColumnSize()
    End Sub

    ''' <summary>
    ''' MenuItemClick Event handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnMenuItemClick_AdjustColumnSize(sender As Object, e As System.EventArgs)
        RadGridView.CurrentColumn.BestFit()
    End Sub

    ''' <summary>
    ''' MenuItemClick Event handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub OnMenuItemClick_SaveUILayout(sender As Object, e As System.EventArgs)
        Call StoreGridViewLayout()
    End Sub

    ''' <summary>
    ''' MenuItemClick Event handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub OnMenuItemClick_LoadUILayout(sender As Object, e As System.EventArgs)
        Call RetrieveGridViewLayout()
    End Sub
    ''' <summary>
    ''' Save the Layout Status
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub StoreGridViewLayout()
        Dim aXML As Xml.XmlWriter
        Dim aString As New System.Text.StringBuilder
        Try
            RadGridView.XmlSerializationInfo.DisregardOriginalSerializationVisibility = True
            RadGridView.XmlSerializationInfo.SerializationMetadata.Clear()
            RadGridView.XmlSerializationInfo.SerializationMetadata.Add(GetType(RadGridView), "MasterTemplate", DesignerSerializationVisibilityAttribute.Content)
            RadGridView.XmlSerializationInfo.SerializationMetadata.Add(GetType(GridViewTemplate), "Columns", DesignerSerializationVisibilityAttribute.Content)
            RadGridView.XmlSerializationInfo.SerializationMetadata.Add(GetType(GridViewDataColumn), "UniqueName", DesignerSerializationVisibilityAttribute.Visible)
            RadGridView.XmlSerializationInfo.SerializationMetadata.Add(GetType(GridViewDataColumn), "Width", DesignerSerializationVisibilityAttribute.Visible)

            aXML = Xml.XmlWriter.Create(aString)
            RadGridView.SaveLayout(aXML)
            aXML.Close()

            If CurrentSession.IsRunning Then
                If CurrentSession.RequireAccessRight(otAccessRight.ReadUpdateData) Then
                    CurrentSession.CurrentDomain.SetSetting(id:="UI." & Me.Name & "." & _modeltable.Id, datatype:=otDataType.Text, _
                                                            description:="Setting for the UI ControlDataGridView Element", value:=aString.ToString)
                    If CurrentSession.CurrentDomain.Persist() Then
                        IssueMessage("layout format saved in database domain (" & CurrentSession.CurrentDomainID & ") setting")
                    Else
                        IssueMessage("unable to save layout format - see session log")
                    End If
                Else
                    IssueMessage("unable to save layout format - OnTrack has not granted update rights")
                End If
            Else
                IssueMessage("unable to save layout format - OnTrack session not running")
            End If
        Catch ex As Exception
            CoreMessageHandler(exception:=ex, procedure:="UIControlDataGridView.StoreGridViewLayout")
        End Try

    End Sub
    ''' <summary>
    ''' Save the Layout Status
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub RetrieveGridViewLayout()
        Dim aString As String
        Dim aXML As Xml.XmlReader

        Try

            If CurrentSession.IsRunning Then
                Dim aDomainSetting As DomainSetting = CurrentSession.CurrentDomain.GetSetting(id:="UI." & Me.Name & "." & _modeltable.Id)
                If aDomainSetting IsNot Nothing Then
                    aString = aDomainSetting.value.ToString
                    aXML = Xml.XmlReader.Create(New System.IO.StringReader(aString))
                    RadGridView.LoadLayout(aXML)
                    aXML.Close()
                    IssueMessage("layout loaded from domain setting")
                Else
                    IssueMessage("no layout defined in domain setting")
                End If

            Else
                IssueMessage("unable to load layout format - OnTrack session not running")
            End If

        Catch ex As Exception
            CoreMessageHandler(exception:=ex, procedure:="UIControlDataGridView.RestoreGridViewLayout")
        End Try


    End Sub

    ''' <summary>
    ''' Cell Editor initialized
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub EadGridView_CellEditorInitialized(sender As Object, e As GridViewCellEventArgs) Handles RadGridView.CellEditorInitialized
        Dim dataColumn As GridViewDataColumn = TryCast(e.Column, GridViewDataColumn)

        If dataColumn IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(_pwfieldname) AndAlso dataColumn.FieldName = _pwfieldname Then
            Dim textBoxEditor As RadTextBoxEditor = TryCast(RadGridView.ActiveEditor, RadTextBoxEditor)

            If textBoxEditor IsNot Nothing Then
                Dim editorElement As RadTextBoxEditorElement = TryCast(textBoxEditor.EditorElement, RadTextBoxEditorElement)
                editorElement.PasswordChar = "#"c
            End If
        End If
    End Sub
    ''' <summary>
    ''' CellFormatting Handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadGridView_CellFormatting(sender As Object, e As CellFormattingEventArgs) Handles RadGridView.CellFormatting
        Dim dataColumn As GridViewDataColumn = TryCast(e.CellElement.ColumnInfo, GridViewDataColumn)

        If dataColumn IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(_pwfieldname) AndAlso dataColumn.FieldName = _pwfieldname Then
            If e.CellElement.RowInfo.Cells(dataColumn.Name) IsNot Nothing Then
                Dim value As Object = e.CellElement.RowInfo.Cells(dataColumn.Name).Value
                Dim text As String = [String].Empty
                If value IsNot Nothing Then
                    Dim passwordLen As Integer = Convert.ToString(value).Length
                    text = [String].Join("#", New String(passwordLen - 1) {})
                End If

                e.CellElement.Text = text
            End If

        End If
    End Sub

    ''' <summary>
    ''' event handler to check on Right to Change Object
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub UIControlDataGridView_CellBeginEdit(sender As Object, e As Telerik.WinControls.UI.GridViewCellCancelEventArgs) Handles RadGridView.CellBeginEdit
        If (Me.Controller IsNot Nothing AndAlso _
            (Me.Controller.State <> MVDataObjectController.CRUDState.Update AndAlso Me.Controller.State <> MVDataObjectController.CRUDState.Create)) _
        OrElse (Me.Controller IsNot Nothing AndAlso _
                Not CurrentSession.RequestUserAccess(accessRequest:=otAccessRight.ReadUpdateData, loginOnFailed:=True, _
                    objecttransactions:={Me.DataObjectID & "." & ormBusinessObject.ConstOPPersist})) Then
            IssueMessage("no right to change a object of type " & Me.DataObjectID)
            e.Cancel = True
        End If

    End Sub
    ''' <summary>
    ''' Event On Row Adding
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub OnRowAdding(sender As Object, e As GridViewRowCancelEventArgs) Handles RadGridView.UserAddingRow
        If (Me.Controller IsNot Nothing AndAlso Me.Controller.State <> MVDataObjectController.CRUDState.Create) _
         OrElse (Me.Controller IsNot Nothing AndAlso _
               Not CurrentSession.RequestUserAccess(accessRequest:=otAccessRight.ReadUpdateData, loginOnFailed:=True, _
                   objecttransactions:={Me.DataObjectID & "." & ormBusinessObject.ConstOPCreate})) Then
            IssueMessage(UITxtNoRowAddable & Me.DataObjectID)
            e.Cancel = True
        End If
    End Sub
    ''' <summary>
    ''' Event On Row Deleting
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub OnRowDeleting(sender As Object, e As GridViewRowCancelEventArgs) Handles RadGridView.UserDeletingRow
        If (Me.Controller IsNot Nothing AndAlso Me.Controller.State <> MVDataObjectController.CRUDState.Delete) _
        OrElse (Me.Controller IsNot Nothing AndAlso _
              Not CurrentSession.RequestUserAccess(accessRequest:=otAccessRight.ReadUpdateData, loginOnFailed:=True, _
                  objecttransactions:={Me.DataObjectID & "." & ormBusinessObject.ConstOPDelete})) Then
            IssueMessage("no right to delete a object of type " & Me.DataObjectID)
            e.Cancel = True
        End If
    End Sub
#End Region

    ''' 
    ''' Events for the ModelTabel as data model
#Region "ModelTableEvents"
    ''' <summary>
    ''' Create failed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Modeltable_ObjectOpFailed(sender As Object, e As ormModelTable.EventArgs) Handles _modeltable.ObjectCreateFailed, _modeltable.ObjectDeleteFailed
        If Not String.IsNullOrWhiteSpace(e.Message) Then Me.IssueMessage(e.Message)
    End Sub
    ''' <summary>
    ''' Update failed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Modeltable_ObjectUpdateFailed(sender As Object, e As ormModelTable.EventArgs) Handles _modeltable.ObjectUpdateFailed, _modeltable.ObjectPersistFailed
        If e.Msglog.Count > 0 Then Me.IssueMessage(e.Msglog.MessageText)
    End Sub
    ''' <summary>
    ''' Event handler for operation messages
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub OnMessageFromTable(sender As Object, e As ormModelTable.EventArgs) Handles _modeltable.OperationMessage
        If _statuslabel IsNot Nothing AndAlso e.Message IsNot Nothing Then
            IssueMessage(e.Message)
        End If
    End Sub
#End Region

    '''
    ''' Events of the Controller

#Region "ControllerEvents"

    ''' <summary>
    '''  Event Handler for Controller changing to Edit
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>

    Private Sub _controller_OnChangingToCreate(sender As Object, e As MVDataObjectController.EventArgs) Handles _controller.OnChangingToCreate
        If Not CurrentSession.RequestUserAccess(accessRequest:=otAccessRight.ReadUpdateData, loginOnFailed:=True, _
                                                     objecttransactions:={Me.DataObjectID & "." & ormBusinessObject.ConstOPCreate}) Then
            IssueMessage("no right to change a object of type " & Me.DataObjectID)
            e.AbortNewState = True
        Else
            RadGridView.AllowAddNewRow = True
            Me.Refresh()
        End If
    End Sub

    Private Sub _controller_OnChangingToUpdate(sender As Object, e As MVDataObjectController.EventArgs) Handles _controller.OnChangingToUpdate
        If Not CurrentSession.RequestUserAccess(accessRequest:=otAccessRight.ReadUpdateData, loginOnFailed:=True, _
                                                     objecttransactions:={Me.DataObjectID & "." & ormBusinessObject.ConstOPPersist}) Then
            IssueMessage("no right to change a object of type " & Me.DataObjectID)
            e.AbortNewState = True
        Else
            RadGridView.AllowAddNewRow = False
            Me.Refresh()
        End If
    End Sub

    Private Sub _controller_OnChangingToDelete(sender As Object, e As MVDataObjectController.EventArgs) Handles _controller.OnChangingToDelete
        If Not CurrentSession.RequestUserAccess(accessRequest:=otAccessRight.ReadUpdateData, loginOnFailed:=True, _
                                                     objecttransactions:={Me.DataObjectID & "." & ormBusinessObject.ConstOPDelete}) Then
            IssueMessage("no right to change a object of type " & Me.DataObjectID)
            e.AbortNewState = True
        Else
            RadGridView.AllowAddNewRow = False
            Me.Refresh()
        End If
    End Sub

#End Region

    ''' <summary>
    ''' Adjust all Columns in size
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub AdjustAllColumnSize()
        RadGridView.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None
        RadGridView.MasterTemplate.BestFitColumns(BestFitColumnMode.AllCells)
    End Sub

    ''' <summary>
    ''' Load Data from the ModelTable in the Grid
    ''' </summary>
    ''' <remarks></remarks>
    Private Function LoadDataModelTable() As Boolean
        If RadGridView.DataSource.GetType.Equals(GetType(ormModelTable)) Then
            ''' set time
            Me.Modeltable.Load() '-> loads also the datagridview

            ''' change the column definition if needed here
            ''' 
            For Each aColumn In RadGridView.Columns.ToList
                Dim anEntry As iormObjectEntryDefinition = _modeltable.GetObjectEntry(aColumn.FieldName)

                If anEntry IsNot Nothing Then
                    ''' set the fieldname as reference
                    aColumn.FieldName = anEntry.Entryname
                    aColumn.Tag = New TagColumnData With {.Type = ColumnType.ObjectEntry, .ObjectEntry = anEntry}
                    ''' hack: if enrypted then show masked
                    If anEntry.Properties.Where(Function(x) x.Enum = otObjectEntryProperty.Encrypted).FirstOrDefault IsNot Nothing Then
                        _pwfieldname = anEntry.Entryname
                    End If
                End If
            Next

            '' try to load the UI Layout
            Call RetrieveGridViewLayout()

            '' switch off editing of primary keys
            For Each aRow In RadGridView.Rows
                ''' attach Tag Data
                aRow.Tag = New TagRowData With {.Type = RowType.DataObjectBound, .DataObject = _modeltable.DataObject(rowno:=aRow.Index), .ReferenceRowNo = aRow.Index}
                ''' switch of the key entries names to be readonly
                For Each aCell As GridViewCellInfo In aRow.Cells
                    Dim anEntry As iormObjectEntryDefinition = _modeltable.GetObjectEntry(aCell.ColumnInfo.FieldName)
                    If anEntry IsNot Nothing Then
                        Dim anobjectdefinition As ormObjectDefinition = anEntry.GetObjectDefinition
                        Dim aKeyNameList = anobjectdefinition.PrimaryKeyEntryNames
                        If aKeyNameList.Contains(anEntry.Entryname) Then aCell.ReadOnly = True
                    End If
                Next
            Next

            HideQRYRowReference()
            Return True
        End If

        Return False
    End Function

    ''' <summary>
    ''' Load Data from the queried enumeration
    ''' </summary>
    ''' <remarks></remarks>
    Private Function LoadDataQueriedEnumeration() As Boolean

        ''' if not loaded then load
        ''' 
        If Not _queriedEnumeration.IsLoaded Then
            _queriedEnumeration.Load() '-> load the objects
        End If

        ''' must be loaded now to proceed
        ''' 
        If _queriedEnumeration.IsLoaded Then

            ''' get all the entries of the enumeration and load the cells 
            ''' 
            For i = 0 To _queriedEnumeration.Count - 1
                Dim anObject As iormRelationalPersistable = _queriedEnumeration.GetObject(i)
                Dim aRow As GridViewRowInfo = RadGridView.MasterTemplate.Rows.AddNew
                aRow.Tag = New TagRowData With {.Type = RowType.DataObjectBound, .ReferenceRowNo = i, .DataObject = anObject}
                ''' set the values of the cells from the Object -> Fieldname holds the objectentry name
                For Each aColumn In RadGridView.MasterTemplate.Columns
                    aRow.Cells.Item(aColumn.FieldName).Value = anObject.GetValue(aColumn.FieldName)
                Next
            Next

            Return True
        End If

        Return False
    End Function
    ''' <summary>
    ''' load the data into the control
    ''' </summary>
    ''' <remarks></remarks>
    Public Function LoadData() As Boolean
        Dim awatch As New Stopwatch
        Dim result As Boolean

        ''' show the progress picture box
        Me.ProgressPictureBox.Visible = True
        Me.ProgressPictureBox.BringToFront()
        Me.Refresh()

        ''' initialize
        If Not _IsDynamicInitialized Then Me.DynamicInitialize()

        ''' set time
        awatch.Start()

        '''
        ''' load the data
        ''' 
        If _modeltable IsNot Nothing Then
            result = LoadDataModelTable()
        ElseIf _queriedEnumeration IsNot Nothing Then
            result = LoadDataQueriedEnumeration()
        End If

        ''' stop watch
        awatch.Stop()
        '' status 
        IssueMessage("finished operation in " & awatch.ElapsedMilliseconds & "ms  and " & RadGridView.Rows.Count & " rows loaded")

        ''' switch of progress
        Me.ProgressPictureBox.Visible = False
        RadGridView.BringToFront()
        ''' bring controller to default state
        If Me.Controller IsNot Nothing Then Me.Controller.State = MVDataObjectController.CRUDState.Read
        Me.Refresh()

        Return result
    End Function

    ''' <summary>
    ''' Hide the QryRowReference
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub HideQRYRowReference()
        ''' invisible reference to modeltable objects
        If RadGridView.Columns.Contains(ormModelTable.constQRYRowReference) Then
            If RadGridView.Columns(ormModelTable.constQRYRowReference).IsVisible Then
                RadGridView.Columns(ormModelTable.constQRYRowReference).IsVisible = False
                RadGridView.Refresh()
            End If
        End If
    End Sub


End Class

''' <summary>
''' DataGrid Event Args
''' </summary>
''' <remarks></remarks>
Public Class UIControlDataGridViewEventArgs
    Inherits System.EventArgs

    Private _msgtext As String
    Private _dataobject As ormBusinessObject

    Public Sub New(messagetext As String)
        _msgtext = messagetext
    End Sub
    Public Sub New(dataobject As ormBusinessObject)
        _dataobject = dataobject
    End Sub
    ''' <summary>
    ''' Gets or sets the msgtext.
    ''' </summary>
    ''' <value>The msgtext.</value>
    Public Property Msgtext() As String
        Get
            Return Me._msgtext
        End Get
        Set(value As String)
            Me._msgtext = value
        End Set
    End Property

    Public Property DataObject As ormBusinessObject
        Get
            Return _dataobject
        End Get
        Set(value As ormBusinessObject)
            _dataobject = value
        End Set
    End Property
End Class