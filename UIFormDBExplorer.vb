Imports Telerik.WinControls.UI
Imports System.Drawing
Imports System.Windows.Forms
Imports OnTrack.UI
Imports OnTrack.Database
Imports Telerik.WinControls
Imports OnTrack.Core


''' <summary>
''' Object Explorer - explors the Object Data and Structure and its setting in the OnTrack Enviormennt
''' </summary>
''' <remarks></remarks>
Public Class UIFormDBExplorer

    ''' <summary>
    ''' ModelClass for the TreeView
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ObjectStructureItem


        Public Enum type
            CacheManager
            Database = 1
            Table
            ObjectRepository
            [Module]
            [Object]
            ObjectEntry
            DbParameter
        End Enum

        Private _ID As String = String.Empty
        Private _Nodetype As [type] = type.Module
        Private _Description As String = String.Empty

        Private _Members As New List(Of ObjectStructureItem)
        Private _DataItem As Object

        ''' <summary>
        ''' Gets or sets the members.
        ''' </summary>
        ''' <value>The members.</value>
        Public Property Members() As List(Of ObjectStructureItem)
            Get
                Return Me._Members
            End Get
            Set(value As List(Of ObjectStructureItem))
                Me._Members = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the description.
        ''' </summary>
        ''' <value>The description.</value>
        Public Property Description() As String
            Get
                Return Me._Description
            End Get
            Set(value As String)
                Me._Description = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the nodetype.
        ''' </summary>
        ''' <value>The nodetype.</value>
        Public Property Nodetype() As type
            Get
                Return Me._Nodetype
            End Get
            Set(value As type)
                Me._Nodetype = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the ID.
        ''' </summary>
        ''' <value>The ID.</value>
        Public Property ID() As String
            Get
                Return Me._ID
            End Get
            Set(value As String)
                Me._ID = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the data item.
        ''' </summary>
        ''' <value>The data item.</value>
        Public Property DataItem() As Object
            Get
                Return Me._DataItem
            End Get
            Set(value As Object)
                Me._DataItem = value
            End Set
        End Property

    End Class

    Private _topItems As New List(Of ObjectStructureItem)
    Dim _selectedDomainID As String
    Private WithEvents _otdbsession As Session

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ' nothing on the shadow
        Me.DataGrid = New Global.OnTrack.UI.UIControlDataGridView()
        _otdbsession = ot.CurrentSession

    End Sub

    ''' <summary>
    ''' Build the Object Tree
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub BuildTree()
        '''
        ''' build the cached Entries
        Dim cacheItem As New ObjectStructureItem With {.ID = "Cache Manager", .Description = "Cache", .Nodetype = ObjectStructureItem.type.CacheManager}
        _topItems.Add(cacheItem)

        ' Add Objects
        Dim repositoryCacheItem As New ObjectStructureItem With {.ID = "ObjectCache", .Description = "Cached Objects", .Nodetype = ObjectStructureItem.type.ObjectRepository}
        cacheItem.Members.Add(repositoryCacheItem)
        For Each anObjectDefinition In CurrentSession.Objects.ObjectDefinitions
            Dim newItem As New ObjectStructureItem With {.ID = anObjectDefinition.Objectname, .Description = anObjectDefinition.Description, .Nodetype = ObjectStructureItem.type.Object, _
                                                         .DataItem = anObjectDefinition}
            If anObjectDefinition.UseCache Then repositoryCacheItem.Members.Add(newItem)
        Next
        ' Add Tables
        Dim dbCacheItem As New ObjectStructureItem With {.ID = "TableCache", .Description = "Cached Tables", .Nodetype = ObjectStructureItem.type.Database}
        cacheItem.Members.Add(dbCacheItem)
        If CurrentSession.IsRuntimeRepositoryAvailable Then

            For Each aTable In CType(CurrentSession.Objects, ormObjectRepository).ContainerDefinitions
                Dim newItem As New ObjectStructureItem With {.ID = aTable.ID, .Description = aTable.Description, .Nodetype = ObjectStructureItem.type.Table, .DataItem = aTable}
                If aTable.UseCache Then dbCacheItem.Members.Add(newItem)
            Next

            '''
            ''' build the Database and Table entries
            Dim databaseItem As New ObjectStructureItem With {.ID = CurrentConfigSetName, .Description = "Database", .Nodetype = ObjectStructureItem.type.Database}
            _topItems.Add(databaseItem)
            Dim dbparameterITem As New ObjectStructureItem With {.ID = "Parameters", .Description = "Db parameters", .Nodetype = ObjectStructureItem.type.DbParameter}
            databaseItem.Members.Add(dbparameterITem)
            For Each aTable In ormContainerDefinition.All
                Dim newItem As New ObjectStructureItem With {.ID = aTable.ID, .Description = aTable.Description, .Nodetype = ObjectStructureItem.type.Table, .DataItem = aTable}
                databaseItem.Members.Add(newItem)
            Next
        End If
        ''' 
        ''' build the Business Objects and Modules entries
        Dim repositoryItem As New ObjectStructureItem With {.ID = "Objects Dictionary", .Description = "all object instances per object definition", .Nodetype = ObjectStructureItem.type.ObjectRepository}
        _topItems.Add(repositoryItem)
        For Each aName In ot.InstalledModules
            Dim ModuleItem As New ObjectStructureItem With {.ID = aName, .Description = "Module", .Nodetype = ObjectStructureItem.type.Module}
            repositoryItem.Members.Add(ModuleItem)

            For Each aDescription In ot.GetObjectClassDescriptionsForModule(modulename:=aName)

                Dim anObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=aDescription.ID)
                If anObjectDefinition Is Nothing Then
                    CoreMessageHandler(message:="Object definition could not be retrieved", objectname:=aDescription.ID, argument:=aName, procedure:="UIFormDBExplorer.BuildTree", messagetype:=otCoreMessageType.InternalError)
                End If
                Dim anObjectItem As New ObjectStructureItem With {.ID = aDescription.ObjectAttribute.ID, .Nodetype = ObjectStructureItem.type.Object, _
                                                                  .DataItem = anObjectDefinition}
                With anObjectItem
                    If aDescription.ObjectAttribute.HasValueDescription Then .Description = aDescription.ObjectAttribute.Description
                End With

                ModuleItem.Members.Add(anObjectItem)
            Next
        Next
    End Sub
    ''' <summary>
    ''' OnLoad Event Handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    ''' 
    Public Overloads Sub OnLoad(sender As Object, e As EventArgs) Handles Me.Load

        If ot.RequireAccess(accessRequest:=otAccessRight.ReadOnly) Then

            If CurrentSession.IsRunning Then
                'Me.DomainComboMenu.ToolTipText = tooltiptext
                Dim i, ind As Integer
                For Each aDomain As Commons.Domain In Commons.Domain.All
                    Dim aRadItem As New Telerik.WinControls.UI.RadListDataItem()
                    aRadItem.Text = aDomain.ID
                    aRadItem.Tag = aDomain
                    Me.DomainComboMenu.ComboBoxElement.Items.Add(aRadItem)
                    If aDomain.ID = CurrentSession.CurrentDomainID Then ind = i
                    i += 1
                Next
                Me.DomainComboMenu.AutoSize = True
                Me.DomainComboMenu.ComboBoxElement.DropDownStyle = RadDropDownStyle.DropDownList
                Me.DomainComboMenu.ComboBoxElement.SelectedIndex = ind
                Me.DomainComboMenu.ToolTipText = "Switch to another domain"
                Me.DomainComboMenu.ComboBoxElement.ToolTipText = "Switch to another domain"
                AddHandler Me.DomainComboMenu.ComboBoxElement.SelectedIndexChanged, AddressOf UIFormDBExplorer_DomainButtonClick
            Else
                'Me.DomainComboMenu.Visible = False
                Me.DomainComboMenu.Enabled = False

            End If

            ''' build the tree
            BuildTree()

            With ObjectTree
                .DataSource = _topItems
                .DisplayMember = "ID\ID\ID\ID"
                .ExpandAll()
                .ChildMember = "ObjectStructureItem\Members\Members\Members"
                .Enabled = True

            End With
        Else
            ObjectTree.Text = "Unable to resolve database information"
        End If

    End Sub

    ''' <summary>
    ''' event handler for domain Changed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub UiFormDBExplorer_OnDomainChanged(sender As Object, e As SessionEventArgs) Handles _otdbsession.OnDomainChanged
        Dim i, ind As Integer
        For Each item In DomainComboMenu.ComboBoxElement.Items
            If TryCast(item.Tag, Commons.Domain).ID = CurrentSession.CurrentDomainID Then
                ind = i
            End If
            i += 1
        Next
        DomainComboMenu.ComboBoxElement.SelectedIndex = ind
    End Sub
    ''' <summary>
    ''' Click Event of the Domain Selection
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub UIFormDBExplorer_DomainButtonClick(sender As Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs)
        Dim aNewDomain As Commons.Domain = Me.DomainComboMenu.ComboBoxElement.SelectedItem.Tag

        If aNewDomain IsNot Nothing AndAlso CurrentSession.IsRunning Then

            If aNewDomain.ID.ToUpper <> CurrentSession.CurrentDomainID.ToUpper Then

                DomainComboMenu.ComboBoxElement.ToolTipText = aNewDomain.Description
                'RadMessageBox.SetThemeName(Me.ThemeName)
                Dim ds As Windows.Forms.DialogResult = _
                    RadMessageBox.Show(Me, "Are you sure to switch to Domain '" & aNewDomain.ID & "' ?", "Switch Domain ", Windows.Forms.MessageBoxButtons.YesNo, RadMessageIcon.Question)
                If ds = Windows.Forms.DialogResult.Yes Then
                    _selectedDomainID = aNewDomain.ID

                    CurrentSession.SwitchToDomain(_selectedDomainID)

                End If

            End If
        End If

    End Sub
    ''' <summary>
    ''' Handler for Screen Tip
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ObjecTree_ScreenTipNeeded(ByVal sender As Object, ByVal e As Telerik.WinControls.ScreenTipNeededEventArgs) Handles ObjectTree.ScreenTipNeeded
        Dim node As TreeNodeElement = TryCast(e.Item, TreeNodeElement)
        Dim screentip As New RadOffice2007ScreenTipElement
        Dim size As New Size(120, 70)
        Dim pad As New Padding(2)

        If node IsNot Nothing Then
            'screentip.MainTextLabel.Image = node.ImageElement.Image
            'screentip.MainTextLabel.TextImageRelation = TextImageRelation.ImageBeforeText
            'screentip.MainTextLabel.Padding = pad
            Dim objectEntryItem = TryCast(node.Data.DataBoundItem, ObjectStructureItem)

            screentip.MainTextLabel.Text = "Object Element:" & objectEntryItem.ID.ToString
            screentip.MainTextLabel.Margin = New System.Windows.Forms.Padding(10)
            screentip.CaptionLabel.Padding = pad
            screentip.CaptionLabel.Text = objectEntryItem.Description

            screentip.EnableCustomSize = False
            screentip.AutoSize = True
            screentip.Size = size
            node.ScreenTip = screentip
        End If
    End Sub

    '#Region dataBoundItem
    ''' <summary>
    ''' SelectedNodeChanged Event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ObjectTree_SelectedNodeChanged(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.RadTreeViewEventArgs) Handles ObjectTree.SelectedNodeChanged
        Dim nodeitem As ObjectStructureItem = TryCast(e.Node.DataBoundItem, ObjectStructureItem)
        If nodeitem IsNot Nothing Then

            Select Case nodeitem.Nodetype

                Case ObjectStructureItem.type.Object
                    Me.PageData.Enabled = True
                    'Me.PageObjectProperties.Enabled = True
                    Me.PageData.Item.Visibility = Telerik.WinControls.ElementVisibility.Visible
                    'Me.PageObjectProperties.Item.Visibility = Telerik.WinControls.ElementVisibility.Visible
                    Dim aObjectdefinition As ormObjectDefinition = TryCast(nodeitem.DataItem, ormObjectDefinition)


                    ''' set the Operation menuItems
                    '''
                    Dim aClassdescription As ObjectClassDescription = ot.GetObjectClassDescriptionByID(id:=aObjectdefinition.ID)
                    If aClassdescription IsNot Nothing Then
                        Dim aList As New List(Of ormObjectOperationMethodAttribute)
                        For Each anOperation In aClassdescription.OperationAttributes
                            If anOperation.HasValueUIVisible AndAlso anOperation.UIVisible Then aList.Add(anOperation)
                        Next

                        Dim aTag As ormObjectOperationMethodAttribute
                        For Each aRadItem In Me.Menu.Items
                            aTag = TryCast(aRadItem.Tag, ormObjectOperationMethodAttribute)
                            If aTag IsNot Nothing Then
                                If aTag.ClassDescription.ObjectAttribute.ID <> aObjectdefinition.ID Then
                                    aRadItem.Visibility = ElementVisibility.Collapsed
                                Else
                                    '** remove from list if contained
                                    Dim anOperation As ormObjectOperationMethodAttribute = aList.Where(Function(x) x.TransactionID = aTag.TransactionID).FirstOrDefault
                                    If anOperation IsNot Nothing Then
                                        aList.Remove(anOperation)
                                        aRadItem.Visibility = ElementVisibility.Visible
                                        aRadItem.Enabled = True
                                    End If
                                End If
                            End If
                        Next

                        For Each anOperation In aList
                            If anOperation.HasValueTransactionID Then
                                Dim aRadMenuItem As New RadMenuItem
                                aRadMenuItem.Text = anOperation.Title
                                If anOperation.HasValueDescription Then aRadMenuItem.ToolTipText = anOperation.Description
                                aRadMenuItem.Tag = anOperation
                                AddHandler aRadMenuItem.Click, AddressOf Me.UIFormDBExplorer_OperationMenuOnClick
                                Me.Menu.Items.Add(aRadMenuItem)
                            End If
                        Next
                    End If
                    Me.Refresh()

                    Dim aqry As Global.OnTrack.Database.iormQueriedEnumeration = aObjectdefinition.GetQuery(ormBusinessObject.ConstQRYAll)
                    Dim aModeltable As ormModelTable = New ormModelTable(aqry)
                    With Me.DataGrid
                        .DataSource = aModeltable
                        .Status = Me.StatusLabel
                        .Dock = System.Windows.Forms.DockStyle.Fill
                        .Status = Me.StatusLabel
                        ''' * no grouping
                        .RadGridView.EnableGrouping = False
                        .RadGridView.AllowAddNewRow = True
                    End With

                    Me.PageData.Controls.Add(Me.DataGrid)
                    If Not Me.DataGrid.IsLoaded Then Me.DataGrid.LoadData()
                    Me.RefreshMenu.Tag = Me.DataGrid
                    Me.RefreshMenu.Visibility = ElementVisibility.Visible
                    AddHandler Me.DataGrid.OnStatusTextChanged, AddressOf Me.UIFormDBExplorer_UIControlDataGridViewOnStatusMessage

                Case Else
                    Me.PageData.Enabled = False
                    Me.PageData.Item.Visibility = Telerik.WinControls.ElementVisibility.Hidden
                    'Me.PageObjectProperties.Enabled = False
                    'Me.PageObjectProperties.Item.Visibility = Telerik.WinControls.ElementVisibility.Hidden
                    ''' switch off
                    For Each aRadItem In Me.Menu.Items
                        Dim aTag As ormObjectOperationMethodAttribute = TryCast(aRadItem.Tag, ormObjectOperationMethodAttribute)
                        If aTag IsNot Nothing Then
                            aRadItem.Visibility = ElementVisibility.Collapsed
                        End If
                    Next

                    Me.RefreshMenu.Visibility = ElementVisibility.Collapsed
                    Me.RefreshMenu.Tag = Nothing
            End Select
            Me.Refresh()
        End If
    End Sub

    ''' <summary>
    ''' Event Handler for the Messages of the embedded DataGridView
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub UIFormDBExplorer_UIControlDataGridViewOnStatusMessage(sender As Object, e As UIControlDataGridViewEventArgs)
        Me.StatusStrip.Items.Remove(Me.StatusLabel)
        Me.StatusLabel.Text = e.Msgtext
        Me.StatusLabel.ToolTipText = e.Msgtext
        Me.StatusStrip.Items.Insert(0, Me.StatusLabel)
        Me.Refresh()
    End Sub
    ''' <summary>
    ''' Handler for the Operation Menu Item
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub UIFormDBExplorer_OperationMenuOnClick(sender As Object, e As EventArgs)
        Dim aDatagrid As UIControlDataGridView
        Dim aRadMenuItem As RadMenuItem = TryCast(sender, RadMenuItem)
        If aRadMenuItem IsNot Nothing Then
            Dim anOperation As ormObjectOperationMethodAttribute = TryCast(aRadMenuItem.Tag, ormObjectOperationMethodAttribute)
            For Each aControl In Me.PageData.Controls
                aDatagrid = TryCast(aControl, UIControlDataGridView)
                If aDatagrid IsNot Nothing Then
                    Exit For
                End If
            Next
            If aDatagrid IsNot Nothing Then
                For Each aDataobject As ormBusinessObject In aDatagrid.SelectedDataObjects
                    Dim theParameterEntries As String() = anOperation.ParameterEntries
                    Dim theParameters As Object()
                    Dim returnValueIndex As Integer
                    Dim returnValue As Object ' dummy
                    ReDim theParameters(anOperation.MethodInfo.GetParameters.Count - 1)
                    ''' set the parameters for the delegate
                    'For i = 0 To theParameters.GetUpperBound(0)
                    '    Dim j As Integer = aMethodInfo.GetParameters(i).Position
                    '    If j >= 0 AndAlso j <= theParameters.GetUpperBound(0) Then
                    '        Select Case theParameterEntries(j)
                    '            Case ObjectCompoundEntry.ConstFNEntryName
                    '                theParameters(j) = entryname
                    '            Case ObjectCompoundEntry.ConstFNValues
                    '                theParameters(j) = returnValue
                    '                returnValueIndex = j
                    '            Case Domain.ConstFNDomainID
                    '                theParameters(j) = Me.DomainID
                    '        End Select

                    '    End If
                    'Next
                    'RadMessageBox.SetThemeName(Me.ThemeName)
                    Dim ds As Windows.Forms.DialogResult = _
                        RadMessageBox.Show(Me, "Are you sure to run operation '" & anOperation.Title & "' - '" & anOperation.Description & "' on " & vbLf _
                                                & anOperation.ClassDescription.ObjectAttribute.Title & " (" & Converter.Array2StringList(aDataobject.ObjectPrimaryKeyValues) & ")" _
                                           , "Please check ", Windows.Forms.MessageBoxButtons.YesNo, RadMessageIcon.Question)
                    If ds = Windows.Forms.DialogResult.Yes Then

                        Dim aDelegate = anOperation.ClassDescription.GetOperartionCallerDelegate(anOperation.OperationName)
                        Dim anoldCursor = Me.Cursor
                        Me.Cursor = Cursors.WaitCursor
                        Me.StatusLabel.Text = "operation '" & anOperation.Title & "' is running - please stand by ..."
                        Me.Refresh()

                        Dim result As Object = aDelegate(aDataobject, theParameters)
                        If DirectCast(result, Boolean) = True Then
                            Me.StatusLabel.Text = "operation '" & anOperation.Title & "' run with success"
                            Me.Cursor = anoldCursor
                            Return
                        Else
                            Me.StatusLabel.Text = "operation '" & anOperation.Title & "' failed to run"
                            Call CoreMessageHandler(procedure:="UIFormDBExplorer.UIFormDBExplorer_operationMenuOnclick", messagetype:=otCoreMessageType.InternalError, _
                                          message:="operation failed", _
                                          argument:=anOperation.OperationName, objectname:=aDataobject.ObjectID)
                            Me.Cursor = anoldCursor
                            Return
                        End If
                    End If

                Next
            End If

        End If

    End Sub
    '#endregion


    ''' <summary>
    ''' NodeFormatting
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>

    Private Sub ObjectTree_NodeFormatting(ByVal sender As Object, ByVal e As TreeNodeFormattingEventArgs) Handles ObjectTree.NodeFormatting
        Dim nodeitem As ObjectStructureItem = TryCast(e.Node.DataBoundItem, ObjectStructureItem)

        If nodeitem IsNot Nothing Then
            ''' format the icon
            ''' 
            Select Case nodeitem.Nodetype
                Case ObjectStructureItem.type.CacheManager
                    e.NodeElement.ImageElement.Image = New Bitmap(My.Resources.library, New Size(24, 24))
                    '''
                    e.NodeElement.ItemHeight = 30
                    e.NodeElement.Font = New Font(e.NodeElement.Font, FontStyle.Bold)
                Case ObjectStructureItem.type.Database
                    e.NodeElement.ImageElement.Image = New Bitmap(My.Resources.db, New Size(24, 24))
                    '''
                    e.NodeElement.ItemHeight = 30
                    e.NodeElement.Font = New Font(e.NodeElement.Font, FontStyle.Bold)

                Case ObjectStructureItem.type.Table
                    e.NodeElement.ImageElement.Image = New Bitmap(My.Resources.table, New Size(12, 12))
                    '''
                    e.NodeElement.ItemHeight = 15
                    e.NodeElement.Font = New Font(e.NodeElement.Font, FontStyle.Regular)

                Case ObjectStructureItem.type.ObjectRepository
                    e.NodeElement.ImageElement.Image = New Bitmap(My.Resources.business, New Size(24, 24))
                    '''
                    e.NodeElement.ItemHeight = 30
                    e.NodeElement.Font = New Font(e.NodeElement.Font, FontStyle.Bold)

                Case ObjectStructureItem.type.Module
                    e.NodeElement.ImageElement.Image = New Bitmap(My.Resources.library, New Size(24, 24))
                    '''
                    e.NodeElement.ItemHeight = 30
                    e.NodeElement.Font = New Font(e.NodeElement.Font, FontStyle.Bold)

                Case ObjectStructureItem.type.Object
                    e.NodeElement.ImageElement.Image = New Bitmap(My.Resources.business_contact, New Size(16, 16))
                    '''
                    e.NodeElement.ItemHeight = 18
                    e.NodeElement.Font = New Font(e.NodeElement.Font, FontStyle.Regular)
                Case ObjectStructureItem.type.DbParameter
                    e.NodeElement.ImageElement.Image = New Bitmap(My.Resources.list_bullets, New Size(12, 12))
                    '''
                    e.NodeElement.ItemHeight = 15
                    e.NodeElement.Font = New Font(e.NodeElement.Font, FontStyle.Regular)
                Case ObjectStructureItem.type.ObjectEntry
                Case Else

            End Select


        End If
    End Sub

    ''' <summary>
    ''' Close Button Handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub uiformDBExplorer_CloseClicked(sender As Object, e As EventArgs) Handles CloseButton.Click
        Me.Close()
    End Sub

    ''' <summary>
    ''' Handler for selected Page changed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadPageView_SelectedPageChanged(sender As Object, e As RadPageViewCancelEventArgs) Handles RadPageView.PageRemoving
        e.Cancel = True
    End Sub

    ''' <summary>
    ''' Click Hander for the RefreshButton
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RefreshMenu_Click(sender As Object, e As EventArgs) Handles RefreshMenu.Click
        Dim aDataGrid As UIControlDataGridView = TryCast(Me.DataGrid, UIControlDataGridView)
        If aDataGrid IsNot Nothing AndAlso aDataGrid.Modeltable IsNot Nothing Then
            aDataGrid.Modeltable.Load(refresh:=True)
            aDataGrid.Refresh()
        End If
    End Sub
End Class
