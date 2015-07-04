Imports OnTrack.Database
Imports System.Windows.Forms
Imports Telerik.WinControls.UI
Imports Telerik.WinControls
Imports OnTrack.Core

Public Class UIFormWorkDeliverables
    Implements iUIStatusSender


    ''' <summary>
    ''' List of Deliverables
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Friend WithEvents DeliverablesViewPage As New RadPageViewPage
    Friend WithEvents DeliverablesGridView As New UIControlDataGridView
    Friend WithEvents DeliverableDetailPanel As New UIControlDeliverablePanel

    Private WithEvents _controller As MVDataObjectListController ' Form Controller

    Public Const UITxtNotConnected As String = "Not connected to database"


    Public Event OnIssueMessage(sender As Object, e As UIStatusMessageEventArgs) Implements iUIStatusSender.OnIssueMessage

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


#End Region

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

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

            ''' set the underlying query and create a controller on it
            Dim aObjectdefinition As ormObjectDefinition = ot.CurrentSession.Objects.GetObjectDefinition(Deliverables.Deliverable.ConstObjectID)
            'Dim aQuery As Global.OnTrack.Database.iormQueriedEnumeration = aObjectdefinition.GetQuery(ormDataObject.ConstQRYAll)
            Dim aQuery As New ormDataObjectEnumeration(Of Deliverables.Deliverable)
            _controller = New MVDataObjectListController(aQuery) ' create the controller

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

            Dim aModeltable As ormModelTable = New ormModelTable(aQuery) 'bin the modeltable to the query to hold the result
            'bind the Modeltable as datasource to the detail panel -> should be bound dynamically by the control on change of the current object
            If aModeltable IsNot Nothing Then DeliverableDetailPanel.DataSource = aModeltable
            '** register the gridview to the status strip
            Me.StatusStrip.RegisterControls(DeliverablesGridView)

            '*** disable leftplanel
            Me.WorkPanel.LeftPanel.Enabled = False
            Me.WorkPanel.LeftPanel.Visible = False
            Me.WorkPanel.LeftPanel.Collapsed = True
            Me.WorkPanel.RightPanel.Enabled = True
            Me.WorkPanel.RightPanel.Visible = True
            Me.WorkPanel.RightPanel.Collapsed = False

            '*** upper right Pannel
            '**
            Me.WorkPanel.UpperRightPanel.Enabled = True
            Me.WorkPanel.UpperRightPanel.Visible = True

            With DeliverablesGridView
                .DataSource = aQuery 'bind the grid to the query

                .Dock = System.Windows.Forms.DockStyle.Fill
                .Controller = Me.Controller
                '' load the modeltable
                If Not .IsLoaded Then .LoadData()

                .AdjustAllColumnSize()
                .Enabled = True
                .RadGridView.CurrentRow = Nothing

            End With


            '* start to put it in PageView -> no pages
            Me.DeliverablesViewPage.Visible = False
            Me.DeliverablesViewPage.Enabled = False
            'Me.DeliverablesViewPage.Item.Visibility = Telerik.WinControls.ElementVisibility.Visible

            '** add GridView to UpperRightPanel 
            Me.WorkPanel.UpperRightPanel.Controls.Add(Me.DeliverablesGridView)

            '** lower panel
            DeliverableDetailPanel.Dock = DockStyle.Fill
            'DeliverableDetailPanel.ThemeName =  ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme)
            DeliverableDetailPanel.Controller = Me.Controller

            Me.WorkPanel.LowerRightPanel.Controls.Add(DeliverableDetailPanel)
            Me.WorkPanel.LowerRightPanel.Enabled = False
            Me.WorkPanel.LowerRightPanel.Collapsed = True

            '** Status Strip
            Me.StatusStrip.Controller = Me.Controller
            Me.StatusStrip.RegisterControls(DeliverableDetailPanel) '*** IMPORTANT -> registers all Controls to issue messages to strip

            Me.Refresh()
        Else
            Me.StatusStrip.IssueMessage(UITxtNotConnected)
        End If

    End Sub

    ''' <summary>
    ''' Event Handler for DataObject Selection
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub UIFormWorkDeliverables_DataObjectSelected(sender As Object, e As UIControlDataGridViewEventArgs) Handles DeliverablesGridView.OnSelectedDataObject
        If e.DataObject IsNot Nothing AndAlso e.DataObject.ObjectID = Deliverables.Deliverable.ConstObjectID Then
            '** lower panel
            Me.WorkPanel.LowerRightPanel.Enabled = True
            Me.WorkPanel.LowerRightPanel.Collapsed = False
            Me.Refresh()
        Else
            '** no lower panel
            Me.WorkPanel.LowerRightPanel.Enabled = False
            Me.WorkPanel.LowerRightPanel.Collapsed = True
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

            If DeliverablesGridView IsNot Nothing Then
                For Each aDataobject As ormBusinessObject In DeliverablesGridView.SelectedDataObjects
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
                        Me.StatusStrip.IssueMessage("operation '" & anOperation.Title & "' is running - please stand by ...")
                        Me.Refresh()

                        Dim result As Object = aDelegate(aDataobject, theParameters)
                        If DirectCast(result, Boolean) = True Then
                            Me.IssueMessage("operation '" & anOperation.Title & "' run with success")
                            Me.Cursor = anoldCursor
                            Return
                        Else
                            Me.IssueMessage("operation '" & anOperation.Title & "' failed to run")
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

    ''' <summary>
    ''' Issue a Message
    ''' </summary>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    Public Sub IssueMessage(message As String)
        RaiseEvent OnIssueMessage(Me, New UIStatusMessageEventArgs(message:=message))
    End Sub


    ''' <summary>
    ''' Click Hander for the RefreshButton
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RefreshMenu_Click(sender As Object, e As EventArgs)

        If DeliverablesGridView IsNot Nothing AndAlso DeliverablesGridView.Modeltable IsNot Nothing Then
            DeliverablesGridView.Modeltable.Load(refresh:=True)
            DeliverablesGridView.Refresh()
        End If
    End Sub

    ''' <summary>
    ''' Click the Edit Button Event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub EditButton_Click(sender As Object, e As EventArgs) 'Handles EditButton.Click
        If Me.Controller IsNot Nothing Then
            Me.Controller.State = MVDataObjectController.CRUDState.Update
        End If
    End Sub

    Private Sub AddNewButton_Click(sender As Object, e As EventArgs) Handles AddNewButton.Click
        If Me.Controller IsNot Nothing Then
            Me.Controller.State = MVDataObjectController.CRUDState.Create
        End If
    End Sub

    Private Sub AcceptButton_Click(sender As Object, e As EventArgs) 'Handles AcceptButton.Click
        If Me.Controller IsNot Nothing Then
            Me.Controller.State = MVDataObjectController.CRUDState.Read
        End If
    End Sub

    Private Sub DeleteButton_Click(sender As Object, e As EventArgs) 'Handles DeleteButton.Click
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


End Class
