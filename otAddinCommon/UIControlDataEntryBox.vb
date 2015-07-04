Imports OnTrack.Database
Imports System.ComponentModel
Imports Telerik.WinControls.UI
Imports OnTrack.Core


''' <summary>
''' Control for DataEntry of a Data Object Model property
''' </summary>
''' <remarks></remarks>
Public Class UIControlDataEntryBox
    Inherits Windows.Forms.UserControl
    Implements iUIStatusSender

    ''' <summary>
    ''' Properties
    ''' </summary>
    ''' <remarks></remarks>
    Private _labelsize As Int16 = 20
    Private _entrysize As Int16 = 50
    Private _descriptionsize As Int16 = 3

    '' data sources
    Private WithEvents _modeltable As ormModelTable
    Private WithEvents _dataobject As iormRelationalPersistable

    ''' <summary>
    ''' inner variables
    ''' </summary>
    ''' <remarks></remarks>

    Private _entryelement As Object 'might me a RadTextbox, etc.

    Private WithEvents _currentsession As Session = ot.CurrentSession 'for domain change

    Private _objectdefinition As ormObjectDefinition
    Private _objectentrydefinition As iormObjectEntryDefinition ' objectentry definition to be used
    Private _objectentryname As String = String.Empty
    Private _objectname As String = String.Empty

    Private WithEvents _controller As MVDataObjectController
    Private _buildControl As Boolean = False 'flag to indicate if control needs to be build from data in database

#Region "Properties"

    ''' <summary>
    ''' Gets or sets the controller.
    ''' </summary>
    ''' <value>The controller.</value>
    ''' 
    <BrowsableAttribute(True), Category("Data"), Description("MVC Controller")> _
    Public Property Controller() As MVDataObjectController
        Get
            Return _controller
        End Get
        Set(value As MVDataObjectController)
            _controller = value
            _buildControl = True 'rebuild control due to controller state
        End Set
    End Property
    ''' <summary>
    ''' gets the entry element which might be flexible
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <BrowsableAttribute(False)> Public ReadOnly Property EntryElement As Object
        Get
            Return _entryelement
        End Get
    End Property

    ''' <summary>
    ''' data source to bind to - must be to the same objectname as this controls object name (or nothing)
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
            If value Is Nothing Then
                _modeltable = Nothing
            ElseIf value.GetType Is GetType(ormModelTable) Then
                If Me.ObjectName Is Nothing OrElse CType(value, ormModelTable).DataObjectID = Me.ObjectName Then
                    If Me.ObjectName Is Nothing Then Me.ObjectName = CType(value, ormModelTable).DataObjectID
                    _modeltable = value
                    AddHandler _modeltable.OnCurrentRowChanged, AddressOf UIConTrolDataEntryBox_CurrentDataObjectChanged
                Else
                    ''' error condition
                End If
            ElseIf value.GetType.GetInterfaces.Where(Function(x) x.Name = GetType(iormRelationalPersistable).Name).Count > 0 Then
                _dataobject = value
                Me.Text = _dataobject.GetValue(ObjectEntryName)
                '** disconnect model table
                If _modeltable IsNot Nothing Then
                    RemoveHandler _modeltable.OnCurrentRowChanged, AddressOf UIConTrolDataEntryBox_CurrentDataObjectChanged
                    _modeltable = Nothing
                End If
            Else
                Throw New System.NotSupportedException("not a supported type")
            End If

        End Set
    End Property

    ''' <summary>
    ''' type of the data souce
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
    ''' Gets or sets the descriptionsize.
    ''' </summary>
    ''' <value>The descriptionsize.</value>
    ''' 
    <BrowsableAttribute(True), Category("Appearance"), Description("size in em (letter count) of the description")> _
    Public Property Descriptionsize() As Short
        Get
            Return _descriptionsize
        End Get
        Set(value As Short)
            _descriptionsize = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the entrysize.
    ''' </summary>
    ''' <value>The entrysize.</value>
    <BrowsableAttribute(True), Category("Appearance"), Description("size in em (letter count) of the entry")> _
    Public Property Entrysize() As Short
        Get
            Return _entrysize
        End Get
        Set(value As Short)
            _entrysize = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the labelsize.
    ''' </summary>
    ''' <value>The labelsize.</value>
    <BrowsableAttribute(True), Category("Appearance"), Description("size in em (letter count) of the label")> _
    Public Property Labelsize() As Short
        Get
            Return _labelsize
        End Get
        Set(value As Short)
            _labelsize = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the build control flag.
    ''' </summary>
    ''' <value>The build control.</value>
    <BrowsableAttribute(False)> _
    Protected Friend Property BuildControl As Boolean
        Get
            Return _buildControl
        End Get
        Private Set(value As Boolean)
            _buildControl = value
        End Set
    End Property

    ''' <summary>
    ''' object name
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <BrowsableAttribute(True), Category("OnTrack"), Description("name of the OnTrack Object (of the entry) - can be canonical form")> _
    Public Property ObjectName As String
        Get
            Return _objectname
        End Get
        Set(value As String)
            If String.Compare(_objectname, value, True) <> 0 Then
                _objectname = value
                Me.BuildControl = True
            End If
        End Set
    End Property
    ''' <summary>
    ''' sets or gets the object entry name either just the entry or the object and entry in canonical form [objectname].[entryname]
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <BrowsableAttribute(True), Category("OnTrack"), Description("name of the OnTrack ObjectEntry - can be canonical form")> _
    Public Property ObjectEntryName As String
        Get
            Return _objectentryname
        End Get
        Set(value As String)
            If String.Compare(_objectentryname, value, True) <> 0 Then
                Dim names() As String = Shuffle.NameSplitter(value)

                '** split the names
                If names.Count > 1 Then
                    _objectname = names(0).ToUpper
                    _objectentryname = names(1).ToUpper
                    Me.BuildControl = True
                Else
                    _objectentryname = value.ToUpper
                    _objectentrydefinition = Nothing
                    Me.BuildControl = True
                End If
            End If

        End Set
    End Property
    ''' <summary>
    ''' set the text presentation of the entry element
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <BrowsableAttribute(False)> _
    Public Property Text As String
        Get
            If Me.EntryElement IsNot Nothing Then Return Me.EntryElement.text
            Return Me.Textbox.Text
        End Get
        Set(value As String)
            If Me.EntryElement IsNot Nothing Then Me.EntryElement.text = value
            Me.Textbox.Text = value
        End Set
    End Property
    ''' <summary>
    ''' Object Entry Definition
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <BrowsableAttribute(False)> _
    Public ReadOnly Property ObjectEntryDefinition As iormObjectEntryDefinition
        Get
            If _objectentrydefinition Is Nothing Then
                If ot.RequireAccess(accessRequest:=otAccessRight.ReadOnly) Then
                    _objectdefinition = ot.CurrentSession.Objects.GetObjectDefinition(id:=_objectname)
                    If _objectdefinition IsNot Nothing Then
                        _objectentrydefinition = _objectdefinition.GetEntry(entryname:=_objectentryname)
                        Me.BuildControl = True

                        If _objectentrydefinition Is Nothing Then
                            ''' todo:error condition
                        End If
                    Else
                        _objectentrydefinition = Nothing

                    End If
                Else
                    _objectdefinition = Nothing
                    _objectentrydefinition = Nothing
                End If
            End If

            Return _objectentrydefinition
        End Get
    End Property
#End Region

    ''' <summary>
    ''' Event Handler for Domain Change to reload Definition
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub UiControlDataEntryBox_onDomainChange(sender As Object, e As EventArgs) Handles _currentsession.OnDomainChanged
        _objectentrydefinition = Nothing 'reset
        Me.BuildControl = True
        Me.Refresh()
    End Sub

    ''' <summary>
    ''' Repaint Event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub UIConTrolDataEntryBox_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Me.BuildControl AndAlso Me.Enabled Then
            DynamicInitialize()
        End If
    End Sub

    ''' <summary>
    ''' Handle the CurrentDataObjectChanged from a Model Table
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub UIConTrolDataEntryBox_CurrentDataObjectChanged(sender As Object, e As UI.ormModelTable.EventArgs) ' Handles _modeltable.OnCurrentRowChanged -> manual
        If Me.BuildControl AndAlso Me.Enabled Then
            DynamicInitialize()
        End If
        If e.Object IsNot Nothing AndAlso e.Object.ObjectDefinition.Objectname = Me.ObjectName AndAlso e.Object.ObjectDefinition.HasEntry(Me.ObjectEntryName) Then
            Me.Text = e.Object.GetValue(entryname:=Me.ObjectEntryName)
        Else
            Me.EntryElement.text = String.Empty
        End If
    End Sub

    ''' <summary>
    ''' Refresh
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Refresh()
        If Me.BuildControl Then
            DynamicInitialize()
        End If
        MyBase.Refresh()
    End Sub
    ''' <summary>
    ''' Dynamic Initialize the DataEntry Box depending 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DynamicInitialize()
        Dim aDataObjectDataType As OnTrack.Core.otDataType
        Dim aDataObjectDefaultValue As String = String.Empty


        ''' retrieve the definitions
        '''
        If ot.RequireAccess(accessRequest:=otAccessRight.ReadOnly) AndAlso Me.ObjectEntryDefinition IsNot Nothing Then
            Me.Label.Text = Me.ObjectEntryDefinition.Title
            aDataObjectDataType = Me.ObjectEntryDefinition.Datatype
            If Not String.IsNullOrEmpty(Me.ObjectEntryDefinition.DefaultValue) Then aDataObjectDefaultValue = Me.ObjectEntryDefinition.DefaultValue.ToString
            Me.EntryDescription.Text = ""
            '** set as done
            Me.BuildControl = False
            '' get the value from the binding
            Select Case Me.DataSouceType
                Case GetType(ormModelTable)
                    _dataobject = _modeltable.DataObject
               
            End Select
        Else
            If Not String.IsNullOrWhiteSpace(Me.ObjectName) AndAlso Not String.IsNullOrWhiteSpace(Me.ObjectEntryName) Then
                Dim anObjectClass As ObjectClassDescription = ot.GetObjectClassDescriptionByID(id:=Me.ObjectName)
                Dim anObjectEntryAttribute = anObjectClass.GetObjectEntryAttribute(entryname:=Me.ObjectEntryName)
                If anObjectEntryAttribute IsNot Nothing Then
                    Me.Label.Text = anObjectEntryAttribute.Title
                    aDataObjectDataType = anObjectEntryAttribute.Datatype
                    If anObjectEntryAttribute.HasValueDefaultValue Then aDataObjectDefaultValue = anObjectEntryAttribute.DefaultValue
                End If
                '** set as done
                Me.BuildControl = False
            End If
        End If

        ''' format
        '''
        Dim g As System.Drawing.Graphics = Me.CreateGraphics
        Dim StringSize As New Drawing.SizeF
        Dim xpos As Integer = 1

        ''' Assign the EntryElement
        ''' 
        If _entryelement Is Nothing Then

            ''' select the approbiate Element
            Select Case aDataObjectDataType
                Case otDataType.Text
                    _entryelement = New RadTextBox
                Case otDataType.Bool
                    _entryelement = New RadCheckBox
                Case Else
                    _entryelement = New RadTextBox
            End Select


            '** rebuild the control
            Me.Controls.Clear()
            If Me.Labelsize > 0 Then Me.Controls.Add(Me.Label)
            Me.Controls.Add(Me.EntryElement)
            If Me.Descriptionsize > 0 Then Me.Controls.Add(Me.EntryDescription)
        End If


        ''' calculate the length of label
        ''' 
        If Me.Labelsize > 0 Then
            StringSize = g.MeasureString(String.Empty.PadRight(Me.Labelsize, "A"), Me.Label.Font)
            With Me.Label
                .AutoSize = False
                .Width = Math.Abs(StringSize.Width + 1)
                .Height = Math.Abs(StringSize.Height + 1)
                .Dock = Windows.Forms.DockStyle.None
                .Anchor = Windows.Forms.AnchorStyles.Left Or Windows.Forms.AnchorStyles.Top Or Windows.Forms.AnchorStyles.Bottom
                .Location = New Drawing.Point(x:=xpos, y:=1)
            End With
            '** increase xpos
            xpos = Me.Label.Size.Width + 2
        End If

        ''' format EntryElement
        ''' 
        If Me.EntryElement IsNot Nothing Then
            Me.Textbox.Enabled = False

            With Me.EntryElement
                .autosize = False
                .font = Me.Textbox.Font
                StringSize = g.MeasureString(String.Empty.PadRight(Me.Entrysize, "Q"), .Font)
                .Width = Math.Abs(StringSize.Width) + 1
                .Height = Math.Abs(StringSize.Height) + 4
                .dock = Windows.Forms.DockStyle.None
                .Anchor = Windows.Forms.AnchorStyles.Left Or Windows.Forms.AnchorStyles.Top Or Windows.Forms.AnchorStyles.Bottom
                .Location = New Drawing.Point(x:=xpos, y:=1)
                .text = Me.Textbox.Text
                If _controller IsNot Nothing AndAlso _controller.State = MVDataObjectController.CRUDState.Read Then
                    .enabled = False
                Else
                    .enabled = True
                End If
            End With
            ' increase xpos
            xpos += Me.EntryElement.size.width + 2
        End If

        ''' add DescriptionLabel
        If Me.Descriptionsize > 0 Then
            StringSize = g.MeasureString(String.Empty.PadRight(Me.Descriptionsize, "A"), Me.EntryDescription.Font)
            With Me.EntryDescription
                .AutoSize = False
                .Width = Math.Abs(StringSize.Width) + 1
                .Height = Math.Abs(StringSize.Height) + 1
                .Dock = Windows.Forms.DockStyle.None
                .Anchor = Windows.Forms.AnchorStyles.Left Or Windows.Forms.AnchorStyles.Top Or Windows.Forms.AnchorStyles.Bottom
                .Text = String.Empty
                .Location = New Drawing.Point(x:=xpos, y:=1)
            End With
            xpos += Me.EntryDescription.Size.Width + 2
        End If

        ''' check me
        ''' 

        Me.BuildControl = False ' to avoid stack overflow in refresh

        With Me

            .Width = xpos
            .Height = Me.EntryElement.height + 4
            .PerformLayout()
            .Refresh()
        End With

    End Sub
    ''' <summary>
    ''' constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    
    ''' <summary>
    ''' Controllers Event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub _controller_OnChangingToCreate(sender As Object, e As MVDataObjectController.EventArgs) Handles _controller.OnChangingToCreate, _controller.OnChangingToUpdate
        If Me.EntryElement IsNot Nothing Then
            Me.EntryElement.enabled = True
        End If
    End Sub

    Private Sub _controller_OnChangingToRead(sender As Object, e As MVDataObjectController.EventArgs) Handles _controller.OnChangingToRead
        If Me.EntryElement IsNot Nothing Then
            Me.EntryElement.enabled = False
        End If
    End Sub

    Private Sub _controller_OnChangingDataObject(sender As Object, e As MVDataObjectController.EventArgs) Handles _controller.OnChangingDataObject
       
        If e.DataObject IsNot Nothing AndAlso e.DataObject.ObjectDefinition.Objectname = Me.ObjectName _
            AndAlso e.DataObject.ObjectDefinition.HasEntry(Me.ObjectEntryName) Then
            Me.DataSource = e.DataObject
        Else
            Me.Text = String.Empty
        End If
    End Sub


    Public Event OnIssueMessage(sender As Object, e As UIStatusMessageEventArgs) Implements iUIStatusSender.OnIssueMessage
End Class
