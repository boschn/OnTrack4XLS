Imports Telerik.WinControls
Imports OnTrack.UI
Imports OnTrack.Database
Imports Telerik.WinControls.UI
Imports System.Windows.Forms

Imports OnTrack.Core
Public Class UIFormWorkDeliverablesOLD

    ''' <summary>
    ''' List of Deliverables
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Private WithEvents _deliverableGridView As New UIControlDataGridView
    Private WithEvents _DeliverableLowerDetailPanel As New UIControlDeliverablePanel

    Private WithEvents _controller As New MVDataObjectController ' Form Controller

    Public Const UITxtNotConnected As String = "Not connected to database"

#Region "Properties"
    ''' <summary>
    ''' Gets the controller.
    ''' </summary>
    ''' <value>The controller.</value>
    Public ReadOnly Property Controller() As MVDataObjectController
        Get
            Return _controller
        End Get
    End Property

    ''' <summary>
    ''' List of Deliverables as DataGrid
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property DeliverablesGridView As UI.UIControlDataGridView
        Get
            Return _deliverableGridView
        End Get
    End Property
#End Region

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        'Me.RibbonBar.RibbonBarElement.IconPrimitive.Visibility = ElementVisibility.Hidden
        '_deliverableGridView = New UI.UIControlDataGridView()
        'Me.DeliverablesViewPage.Enabled = False
        'Me.DeliverablesPageView.Enabled = False
    End Sub
    ''' <summary>
    ''' Initialized Event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub UiFormWorkDeliverables_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        ''' Initialize through connection to database
        ''' 
        If ot.RequireAccess(otAccessRight.ReadOnly) Then
            Me.DeliverablesViewPage.Enabled = True
            Me.DeliverablesViewPage.Item.Visibility = Telerik.WinControls.ElementVisibility.Visible
            Dim aObjectdefinition As ormObjectDefinition = ot.CurrentSession.Objects.GetObjectDefinition(Deliverables.Deliverable.ConstObjectID)


            ''' set the Operation menuItems from the database core classes
            '''
            Dim aClassdescription As ObjectClassDescription = ot.GetObjectClassDescriptionByID(id:=Deliverables.Deliverable.ConstObjectID)
            If aClassdescription IsNot Nothing Then
                Dim aList As New List(Of ormObjectOperationMethodAttribute)
                For Each anOperation In aClassdescription.OperationAttributes
                    If anOperation.HasValueUIVisible AndAlso anOperation.UIVisible Then aList.Add(anOperation)
                Next

                Dim aTag As ormObjectOperationMethodAttribute
                For Each aRadItem In Me.DeliverablesGridView.Menuitems
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
                        AddHandler aRadMenuItem.Click, AddressOf Me.UIFormWorkDeliverables_OperationMenuOnClick
                        Me.DeliverablesGridView.Menuitems.Add(aRadMenuItem)
                    End If
                Next
            End If

            ''' bind the Form to a Qry Object
            ''' and create a Modeltable
            Dim aqry As Global.OnTrack.Database.iormQueriedEnumeration = aObjectdefinition.GetQuery(ormBusinessObject.ConstQRYAll)
            Dim aModeltable As ormModelTable = New ormModelTable(aqry)
            If aModeltable IsNot Nothing Then _DeliverableLowerDetailPanel.DataSource = aModeltable

            ''' format GRIDVIEW
            ''' 
            With Me.DeliverablesGridView
                .DataSource = aModeltable
                .Status = Me.StatusLabel
                .Dock = System.Windows.Forms.DockStyle.Fill
                .Status = Me.StatusLabel
            End With
            Me.SplitContainer.Enabled = True
            Me.SplitUpperPanel.Enabled = True
            DeliverablesGridView.controller = Me.Controller

            '' load the modeltable
            If Not DeliverablesGridView.IsLoaded Then DeliverablesGridView.LoadData()
            DeliverablesGridView.AdjustAllColumnSize()

            '* start to put it in PageView -> no pages
            Me.SplitUpperPanel.Controls.Add(Me.DeliverablesGridView)
            Me.DeliverablesPageView.Visible = False
            Me.DeliverablesViewPage.Visible = False
            'Me.DeliverablesPageView.Controls.Add(Me.DeliverablesGridView)

            DeliverablesGridView.Enabled = True
            DeliverablesGridView.RadGridView.CurrentRow = Nothing

            '** lower panel
            _DeliverableLowerDetailPanel.Dock = DockStyle.Fill
            '_DeliverableLowerDetailPanel.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
            _DeliverableLowerDetailPanel.Controller = Me.Controller

            Me.SplitLowerPanel.Controls.Add(_DeliverableLowerDetailPanel)
            Me.SplitLowerPanel.Enabled = False
            Me.SplitLowerPanel.Collapsed = True

            '**
            Me.AcceptStripButton.Enabled = False
            Me.AcceptStripButton.Visibility = ElementVisibility.Hidden

            Me.AbortStripButton.Enabled = False
            Me.AbortStripButton.Visibility = ElementVisibility.Hidden
            Me.Refresh()
        Else
            Me.StatusLabel.Text = UITxtNotConnected
        End If

    End Sub

    ''' <summary>
    ''' Event Handler for DataObject Selection
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub UIFormWorkDeliverables_DataObjectSelected(sender As Object, e As UIControlDataGridViewEventArgs) Handles _deliverableGridView.OnSelectedDataObject
        If e.DataObject IsNot Nothing AndAlso e.DataObject.ObjectID = Deliverables.Deliverable.ConstObjectID Then
            '** lower panel
            Me.SplitLowerPanel.Enabled = True
            Me.SplitLowerPanel.Collapsed = False
            Me.Refresh()
        Else
            '** no lower panel
            Me.SplitLowerPanel.Enabled = False
            Me.SplitLowerPanel.Collapsed = True
            Me.Refresh()
        End If
    End Sub
    ''' <summary>
    ''' Handler for the Operation Menu Item
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub UIFormWorkDeliverables_OperationMenuOnClick(sender As Object, e As EventArgs)
        Dim aDatagrid As UIControlDataGridView
        Dim aRadMenuItem As RadMenuItem = TryCast(sender, RadMenuItem)
        If aRadMenuItem IsNot Nothing Then
            Dim anOperation As ormObjectOperationMethodAttribute = TryCast(aRadMenuItem.Tag, ormObjectOperationMethodAttribute)

            If _deliverableGridView IsNot Nothing Then
                For Each aDataobject As ormBusinessObject In _deliverableGridView.SelectedDataObjects
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
    ''' Close Button Handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub UIFormWorkDeliverables_CloseClicked(sender As Object, e As EventArgs) Handles CloseButton.Click
        Me.Close()
    End Sub

    ''' <summary>
    ''' Click Hander for the RefreshButton
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RefreshMenu_Click(sender As Object, e As EventArgs)

        If _deliverableGridView IsNot Nothing AndAlso _deliverableGridView.Modeltable IsNot Nothing Then
            _deliverableGridView.Modeltable.Load(refresh:=True)
            _deliverableGridView.Refresh()
        End If
    End Sub

    ''' <summary>
    ''' Click the Edit Button Event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub EditButton_Click(sender As Object, e As EventArgs) Handles EditButton.Click
        If Me.Controller IsNot Nothing Then
            Me.Controller.State = MVDataObjectController.CRUDState.Update
        End If
    End Sub

    Private Sub AddNewButton_Click(sender As Object, e As EventArgs) Handles AddNewButton.Click
        If Me.Controller IsNot Nothing Then
            Me.Controller.State = MVDataObjectController.CRUDState.Create
        End If
    End Sub

    Private Sub AcceptButton_Click(sender As Object, e As EventArgs) Handles AcceptButton.Click, AcceptStripButton.Click
        If Me.Controller IsNot Nothing Then
            Me.Controller.State = MVDataObjectController.CRUDState.Read
        End If
    End Sub

    Private Sub DeleteButton_Click(sender As Object, e As EventArgs) Handles DeleteButton.Click
        If Me.Controller IsNot Nothing Then

            ''' from Update abort changes go to read
            If Me.Controller.State = MVDataObjectController.CRUDState.Update Then
                Me.Controller.State = MVDataObjectController.CRUDState.Read
            Else
                ''' delete object
                Me.Controller.State = MVDataObjectController.CRUDState.Delete
            End If

        End If
    End Sub

    Private Sub _controller_OnChangingToCreate(sender As Object, e As MVDataObjectController.EventArgs) Handles _controller.OnChangingToCreate
        Me.AcceptButton.Visibility = ElementVisibility.Visible
        Me.AcceptButton.Enabled = True

        Me.DeleteButton.Visibility = ElementVisibility.Visible
        Me.DeleteButton.Enabled = True
        Me.DeleteButton.ToolTipText = "Abort Changes"

        Me.EditButton.Visibility = ElementVisibility.Hidden
        Me.EditButton.Enabled = False

        Me.AddNewButton.Visibility = ElementVisibility.Hidden
        Me.AddNewButton.Enabled = False

        ''' in Strip
        Me.AcceptStripButton.Visibility = ElementVisibility.Visible
        Me.AcceptStripButton.Enabled = True
        Me.AbortStripButton.Visibility = ElementVisibility.Visible
        Me.AbortStripButton.Enabled = True
    End Sub

    Private Sub _controller_OnChangingToDelete(sender As Object, e As MVDataObjectController.EventArgs) Handles _controller.OnChangingToDelete
        ''' from Update abort changes go to read
        If Me.Controller.State = MVDataObjectController.CRUDState.Update Then
            Me.Controller.State = MVDataObjectController.CRUDState.Read
        Else
            ''' delete object

            Me.AcceptButton.Visibility = ElementVisibility.Hidden
            Me.AcceptButton.Enabled = False

            Me.EditButton.Visibility = ElementVisibility.Hidden
            Me.EditButton.Enabled = False
        End If
    End Sub

    Private Sub _controller_OnChangingToRead(sender As Object, e As MVDataObjectController.EventArgs) Handles _controller.OnChangingToRead
        Me.AcceptButton.Visibility = ElementVisibility.Hidden
        Me.AcceptButton.Enabled = False

        Me.DeleteButton.Visibility = ElementVisibility.Visible
        Me.DeleteButton.Enabled = True
        Me.DeleteButton.ToolTipText = "Delete"

        Me.EditButton.Visibility = ElementVisibility.Visible
        Me.EditButton.Enabled = True

        Me.AddNewButton.Visibility = ElementVisibility.Visible
        Me.AddNewButton.Enabled = True

        ''' in Strip
        Me.AcceptStripButton.Visibility = ElementVisibility.Hidden
        Me.AcceptStripButton.Enabled = False
        Me.AbortStripButton.Visibility = ElementVisibility.Hidden
        Me.AbortStripButton.Enabled = False
    End Sub

    Private Sub _controller_OnChangingToUpdate(sender As Object, e As MVDataObjectController.EventArgs) Handles _controller.OnChangingToUpdate
        Me.AcceptButton.Visibility = ElementVisibility.Visible
        Me.AcceptButton.Enabled = True

        Me.DeleteButton.Visibility = ElementVisibility.Visible
        Me.DeleteButton.Enabled = True
        Me.DeleteButton.ToolTipText = "Abort Changes"

        Me.EditButton.Visibility = ElementVisibility.Hidden
        Me.EditButton.Enabled = False

        Me.AddNewButton.Visibility = ElementVisibility.Hidden
        Me.AddNewButton.Enabled = False

        ''' in Strip
        Me.AcceptStripButton.Visibility = ElementVisibility.Visible
        Me.AcceptStripButton.Enabled = True

        Me.AbortStripButton.Visibility = ElementVisibility.Visible
        Me.AbortStripButton.Enabled = True
        Me.AbortStripButton.ToolTipText = "Abort Changes"
    End Sub

    Private Sub _deliverableGridView_OnStatusTextChanged(sender As Object, e As UIControlDataGridViewEventArgs) Handles _deliverableGridView.OnStatusTextChanged

    End Sub
End Class
