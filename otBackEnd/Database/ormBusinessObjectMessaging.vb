
REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** ORM Business Object Messaging Classes
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2014-01-31
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************''' <summary>
Option Explicit On
Imports System.Collections
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Reflection
Imports OnTrack.Commons
Imports OnTrack.Core

Namespace OnTrack.Database

    ''' <summary>
    ''' ObjectLog for Messages for Business Objects 
    ''' </summary>
    ''' <remarks>
    ''' 
    ''' The ObjectMessageLog is not an Data Object on its own. it is derived from the RelationCollection and
    ''' embedded as relation Member in a data object class
    ''' </remarks>
    Public Class BusinessObjectMessageLog
        Inherits ormRelationCollection(Of BusinessObjectMessage)
        Implements iormLoggable

        ''' <summary>
        ''' Event Args
        ''' </summary>
        ''' <remarks></remarks>
        Public Class EventArgs
            Inherits System.EventArgs

            Private _log As BusinessObjectMessageLog
            Private _objectmessage As BusinessObjectMessage

            Public Sub New(log As BusinessObjectMessageLog, message As BusinessObjectMessage)
                _log = log
                _objectmessage = message
            End Sub

            ''' <summary>
            ''' Gets  the objectmessage log.
            ''' </summary>
            ''' <value>The objectmessage.</value>
            Public ReadOnly Property Log() As BusinessObjectMessageLog
                Get
                    Return Me._log
                End Get
            End Property
            ''' <summary>
            ''' Gets  the objectmessage.
            ''' </summary>
            ''' <value>The objectmessage.</value>
            Public ReadOnly Property Message() As BusinessObjectMessage
                Get
                    Return Me._objectmessage
                End Get
            End Property

        End Class
        ''' <summary>
        ''' Variables
        ''' </summary>
        ''' <remarks></remarks>
        Private _tag As String = String.Empty

        Private _ContextIdentifier As String
        Private _TupleIdentifier As String
        Private _EntitityIdentifier As String

        '''
        Private _MessagesPerStatusType As New SortedDictionary(Of String, List(Of BusinessObjectMessage))


        ''' <summary>
        ''' Events 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Event OnObjectMessageAdded(sender As Object, e As BusinessObjectMessageLog.EventArgs)

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="container"></param>
        ''' <remarks></remarks>

        Public Sub New(Optional container As ormBusinessObject = Nothing, _
                       Optional contextidenifier As String = Nothing, _
                       Optional tupleidentifier As String = Nothing, _
                       Optional entitityidentifier As String = Nothing)

            MyBase.New(container:=container, keyentrynames:={BusinessObjectMessage.ConstFNNo})
            If container IsNot Nothing Then AddHandler container.OnInfused, AddressOf Me.ObjectMessageLog_OnInfused
            If contextidenifier IsNot Nothing Then _ContextIdentifier = contextidenifier
            If tupleidentifier IsNot Nothing Then _TupleIdentifier = tupleidentifier
            If entitityidentifier IsNot Nothing Then _EntitityIdentifier = entitityidentifier

        End Sub

#Region "Properties"

        ''' <summary>
        ''' gets the Tag of the Log
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Tag()
            Get
                Return _tag
            End Get
        End Property

        ''' <summary>
        ''' returns the greatest message no in the log
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property MaxMessageNo()
            Get
                If Me.Keys.Count = 0 Then Return 0
                Return Me.Keys.Max(Function(x) x.Item(0))
            End Get
        End Property

        '***** ContextIdentifier (identifier) sets the context of the message receiver
        '*****
        Public Property ContextIdentifier As String Implements iormLoggable.ContextIdentifier
            Get
                ContextIdentifier = _ContextIdentifier
            End Get
            Set(value As String)
                _ContextIdentifier = value
            End Set
        End Property

        '***** ContextIdentifier (identifier) sets the context of the message receiver
        '*****
        Public Property TupleIdentifier() As String Implements iormLoggable.TupleIdentifier
            Get
                TupleIdentifier = _TupleIdentifier
            End Get
            Set(value As String)
                _TupleIdentifier = value
            End Set
        End Property

        '***** ContextIdentifier (identifier) sets the context of the message receiver
        '*****
        Public Property EntityIdentifier() As String Implements iormLoggable.EntityIdentifier
            Get
                EntityIdentifier = _EntitityIdentifier
            End Get
            Set(value As String)
                _EntitityIdentifier = value
            End Set
        End Property

        ''' <summary>
        ''' Returns myself ?!
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ObjectMessageLog As BusinessObjectMessageLog Implements iormLoggable.BusinessObjectMessageLog
            Get
                Return Me
            End Get
            Set(value As BusinessObjectMessageLog)
                Throw New InvalidOperationException("setting the objectmessage log on a objectmessagelog impossible")
            End Set
        End Property

#End Region

        ''' <summary>
        ''' Clear the ObjectMessagelog from all Messages
        ''' </summary>
        ''' <remarks></remarks>
        Public Overloads Sub Clear()
            '** delete messages
            For Each message In Me
                message.Delete()
            Next
            MyBase.Clear()
            _MessagesPerStatusType.Clear()
        End Sub
        ''' <summary>
        ''' event handler for tag
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ObjectMessageLog_OnInfused(sender As Object, e As ormDataObjectEventArgs)
            _tag = TryCast(_container, ormBusinessObject).ObjectTag
        End Sub

        ''' <summary>
        ''' event handler for adding a message to the log to set the idno
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ObjectMessageLog_OnAdding(sender As Object, e As ormRelationCollection(Of BusinessObjectMessage).EventArgs) Handles MyBase.OnAdding

        End Sub


        ''' <summary>
        ''' retrieves the log and loads all messages for the container object
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Retrieve(msglogtag As String) As iormRelationalCollection(Of BusinessObjectMessage)
            '''
            ''' check if the new Property value is different then old one
            ''' 
            '** build query
            Dim newCollection As ormRelationCollection(Of BusinessObjectMessage) = New ormRelationCollection(Of BusinessObjectMessage)(Nothing, keyentrynames:={BusinessObjectMessage.ConstFNNo})
            'Dim aTag = TryCast(_container, ormDataObject).ObjectTag
            Try
                Dim aStore As iormRelationalTableStore = ot.GetPrimaryTableStore(BusinessObjectMessage.ConstPrimaryTableID) '_container.PrimaryTableStore is the class itself
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(id:="RetrieveObjectMessages", addAllFields:=True)
                If Not aCommand.IsPrepared Then
                    aCommand.Where = "[" & BusinessObjectMessage.ConstFNTag & "] = @tag "
                    aCommand.Where &= " AND [" & BusinessObjectMessage.ConstFNIsDeleted & "] = @deleted "
                    aCommand.OrderBy = "[" & BusinessObjectMessage.ConstFNNo & "] asc"
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@tag", ColumnName:=BusinessObjectMessage.ConstFNTag, tableid:=BusinessObjectMessage.ConstPrimaryTableID))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@deleted", ColumnName:=BusinessObjectMessage.ConstFNIsDeleted, tableid:=BusinessObjectMessage.ConstPrimaryTableID))
                    aCommand.Prepare()
                End If
                aCommand.SetParameterValue(ID:="@tag", value:=msglogtag)
                aCommand.SetParameterValue(ID:="@deleted", value:=False)

                Dim aRecordCollection = aCommand.RunSelect

                For Each aRecord As ormRecord In aRecordCollection
                    Dim aMessage As New BusinessObjectMessage
                    If aMessage.InfuseDataObject(record:=aRecord, dataobject:=aMessage) Then
                        newCollection.Add(item:=aMessage)
                    End If
                Next

                Return newCollection


            Catch ex As Exception

                Call CoreMessageHandler(exception:=ex, procedure:="ObjectMessageLog.Retrieve")
                Return newCollection

            End Try
        End Function

        '*** addMsg adds a Message to the MessageLog with the associated
        '***
        '*** Contextordinal (can be Nothing) as MQF or other ordinal
        '*** Tupleordinal (can be Nothing) as Row or Dataset
        '*** Entity (can be Nothing) per Field or ID

        '***
        '*** looks up the Messages and Parameters from the MessageLogTable
        '*** returns true if successfull

        ''' <summary>
        ''' Add an existing message (basically copy it and add it)
        ''' </summary>
        ''' <param name="message"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Add(message As BusinessObjectMessage) As Boolean
            Return Me.Add(message.MessageTypeID, message.DomainID, _
                          message.ContextIdentifier, message.TupleIdentifier, message.EntityIdentifier, _
                          message.Sender, message.Parameters)
        End Function
        ''' <summary>
        ''' adds a message of the message type uid to the log
        ''' </summary>
        ''' <param name="msguid"></param>
        ''' <param name="ContextIdentifier"></param>
        ''' <param name="TupleIdentifier"></param>
        ''' <param name="EntitityIdentifier"></param>
        ''' <param name="Args"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Add(ByVal typeuid As Long,
                             ByVal domainid As String,
                             ByVal contextidentifier As String, _
                             ByVal tupleIdentifier As String, _
                             ByVal entitityIdentifier As String, _
                             ByVal sender As Object, _
                             ParamArray args() As Object) As Boolean

            Dim runtimeOnly As Boolean = False

            ''' default values
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            If String.IsNullOrWhiteSpace(contextidentifier) Then contextidentifier = Me.ContextIdentifier
            If String.IsNullOrWhiteSpace(tupleIdentifier) Then tupleIdentifier = Me.TupleIdentifier
            If String.IsNullOrWhiteSpace(entitityIdentifier) Then entitityIdentifier = Me.EntityIdentifier

            If _container IsNot Nothing AndAlso _container.GetType.GetInterfaces.Contains(GetType(iormLoggable)) Then
                Dim aLoggable As iormLoggable = DirectCast(_container, iormLoggable)

                If String.IsNullOrWhiteSpace(contextidentifier) Then contextidentifier = aLoggable.ContextIdentifier
                If String.IsNullOrWhiteSpace(tupleIdentifier) Then tupleIdentifier = aLoggable.TupleIdentifier
                If String.IsNullOrWhiteSpace(entitityIdentifier) Then entitityIdentifier = aLoggable.EntityIdentifier
            End If


            ''' 
            ''' get the Message Definition
            Dim aMessageDefinition As ObjectMessageType = ObjectMessageType.Retrieve(uid:=typeuid, domainid:=domainid)
            If aMessageDefinition Is Nothing Then
                Dim anObjectname As String = String.Empty
                If _container IsNot Nothing Then anObjectname = _container.ObjectID
                Dim context As String
                If contextidentifier IsNot Nothing Then context &= contextidentifier
                If tupleIdentifier IsNot Nothing Then context &= tupleIdentifier & ConstDelimiter
                If entitityIdentifier IsNot Nothing Then context &= entitityIdentifier & ConstDelimiter

                CoreMessageHandler(message:="object message type of uid '" & typeuid.ToString & "' could not be retrieved with context '" & context & "'", procedure:="ObjectMessageLog.Add", _
                                   messagetype:=otCoreMessageType.InternalWarning, objectname:=anObjectname, argument:=Me.Tag)
            End If

            If _container Is Nothing Then
                runtimeOnly = True
            Else
                runtimeOnly = _container.RuntimeOnly
            End If


            '''
            ''' create a Message
            ''' 
            Dim anIDNo As Long
            If Me.Size > 0 Then
                anIDNo = Me.MaxMessageNo + 1
            Else
                anIDNo = 1
            End If

            ''' check on tag - set it
            If String.IsNullOrWhiteSpace(Me.Tag) Then
                If _container IsNot Nothing Then _tag = TryCast(_container, ormBusinessObject).ObjectTag
                If String.IsNullOrWhiteSpace(_tag) Then _tag = Guid.NewGuid.ToString
                For Each message In Me
                    message.Tag = _tag
                Next
            End If

            ''' 
            ''' create message
            ''' 
            Dim aMessage As BusinessObjectMessage = BusinessObjectMessage.Create(msglogtag:=Me.Tag, no:=anIDNo, typeuid:=typeuid, _
                                                                 contextIdentifier:=contextidentifier, tupleIdentifier:=tupleIdentifier, entitityIdentifier:=entitityIdentifier, _
                                                                 parameters:=args, runtimeOnly:=runtimeOnly)


            If aMessage IsNot Nothing Then
                If aMessageDefinition IsNot Nothing Then aMessage.IsPersisted = aMessageDefinition.IsPersisted
                aMessage.Username = CurrentSession.CurrentUsername
                aMessage.Sessionid = CurrentSession.SessionID
                '* try to get the sender
                If sender Is Nothing Then sender = _container
                aMessage.Sender = sender
                '* add
                MyBase.Add(item:=aMessage)
                Return True
            End If


            Return False
        End Function

        ''' <summary>
        ''' Handler for the  internal OnAdded Event - raises the Object Added event of the Log
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectMessageLog_OnAdded(sender As Object, e As ormRelationCollection(Of BusinessObjectMessage).EventArgs) Handles Me.OnAdded
            RaiseEvent OnObjectMessageAdded(Me, New BusinessObjectMessageLog.EventArgs(log:=Me, message:=e.Dataobject))

            Dim aMessage As BusinessObjectMessage = e.Dataobject
            Dim aDomainID As String
            If _container IsNot Nothing Then
                If _container.ObjectHasDomainBehavior Then
                    aDomainID = _container.DomainID
                Else
                    aDomainID = CurrentSession.CurrentDomainID
                End If
            Else
                aDomainID = CurrentSession.CurrentDomainID
            End If

            Dim aMessageType As ObjectMessageType = ObjectMessageType.Retrieve(uid:=aMessage.MessageTypeID, domainid:=aDomainID)
            If aMessageType IsNot Nothing Then
                ''' add the message to each status type
                ''' 
                For Each aStatusType As String In aMessageType.StatusTypes
                    Dim aList As New List(Of BusinessObjectMessage)
                    If _MessagesPerStatusType.ContainsKey(aStatusType.ToUpper) Then
                        aList = _MessagesPerStatusType.Item(aStatusType.ToUpper)
                    Else
                        _MessagesPerStatusType.Add(key:=aStatusType.ToUpper, value:=aList)
                    End If
                    ' add it
                    aList.Add(aMessage)
                Next
            End If
        End Sub
        ''' <summary>
        ''' returns a list of messagetexts
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MessageTexts() As List(Of String)
            Dim aList As New List(Of String)

            For Each aMessage In Me
                aList.Add(aMessage.Message)
            Next

            Return aList
        End Function
        ''' <summary>
        ''' returns a one string with all messagetextes
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MessageText() As String
            Dim aMessageText As New Text.StringBuilder

            For Each aMessage As BusinessObjectMessage In Me
                aMessageText.AppendFormat("{0:000000}:", aMessage.MessageTypeID)
                aMessageText.AppendLine(aMessage.Message)
            Next

            Return aMessageText.ToString
        End Function
        ''' <summary>
        ''' Returns the Highest StatusItem - returns nothing if the statusItem is not there
        ''' </summary>
        ''' <param name="statustype"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Function GetHighesStatusItem(Optional ByVal statustype As String = Nothing) As StatusItem
            Dim aList As IList(Of BusinessObjectMessage)
            If statustype IsNot Nothing Then
                If _MessagesPerStatusType.ContainsKey(statustype.ToUpper) Then
                    aList = _MessagesPerStatusType.Item(key:=statustype.ToUpper)
                Else
                    aList = Me.ToList
                End If
                If aList.Count = 0 Then Return Nothing
                Dim highestStatusWeight As Integer = aList.Max(Function(x)
                                                                   Try
                                                                       Dim s As IList(Of StatusItem) = x.HighestStatusItems(statustype:=statustype)
                                                                       If s IsNot Nothing AndAlso s.Count > 0 Then Return s.First(Function(t) t.Weight.HasValue).Weight
                                                                       Return -1
                                                                   Catch ex As Exception
                                                                       Return -1
                                                                   End Try
                                                               End Function)
                If highestStatusWeight = -1 Then Return Nothing

                For Each aMessage In aList
                    Dim aShortList As IEnumerable(Of StatusItem) = aMessage.StatusItems(statustype:=statustype).Where(Function(x) x.Weight = highestStatusWeight).ToList
                    If aShortList.Count > 0 Then
                        Return aShortList.First
                    End If
                Next

            Else
                aList = Me.ToList
                If aList.Count = 0 Then Return Nothing
                Dim highestStatusWeight As Integer = aList.Max(Function(x)
                                                                   Try
                                                                       Dim s As IList(Of StatusItem) = x.HighestStatusItems
                                                                       If s IsNot Nothing AndAlso s.Count > 0 Then Return s.First(Function(t) t.Weight.HasValue).Weight
                                                                       Return -1
                                                                   Catch ex As Exception
                                                                       Return -1
                                                                   End Try

                                                               End Function)
                If highestStatusWeight = -1 Then Return Nothing
                For Each aMessage In aList
                    Dim aShortList As IEnumerable(Of StatusItem) = aMessage.StatusItems().Where(Function(x) x.Weight = highestStatusWeight).ToList
                    If aShortList.Count > 0 Then
                        Return aShortList.First
                    End If
                Next

            End If

            Return Nothing
        End Function
        ''' <summary>
        ''' Returns the Highest StatusItem - returns nothing if the statusItem is not there
        ''' </summary>
        ''' <param name="statustype"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Function GetHighestMessageHighestStatusItem(Optional ByVal statustype As String = Nothing) As StatusItem
            Dim aList As IList(Of BusinessObjectMessage)
            If statustype IsNot Nothing Then
                If _MessagesPerStatusType.ContainsKey(statustype.ToUpper) Then
                    aList = _MessagesPerStatusType.Item(key:=statustype.ToUpper)
                Else
                    aList = Me.ToList
                End If
                If aList.Count = 0 Then Return Nothing
                Dim highestWeight As Integer = aList.Max(Function(x) x.Weight)
                Dim aMessage = aList.Where(Function(x) x.Weight = highestWeight).FirstOrDefault

                If aMessage IsNot Nothing Then
                    Return aMessage.HighestStatusItems(statustype:=statustype).FirstOrDefault
                End If
            Else
                Dim highestWeight As Integer = aList.Max(Function(x) x.Weight)
                Dim aMessage = aList.Where(Function(x) x.Weight = highestWeight).FirstOrDefault

                If aMessage IsNot Nothing Then
                    Return aMessage.HighestStatusItems().FirstOrDefault
                End If

            End If

            Return Nothing
        End Function


        ''' <summary>
        ''' OnRemoved Handler
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>

        Private Sub ObjectMessageLog_OnRemoved(sender As Object, e As Database.ormRelationCollection(Of BusinessObjectMessage).EventArgs) Handles Me.OnRemoved
            '  e.Dataobject.Delete() -> delete Event will remove too and removing doesnot mean deleting !
        End Sub
    End Class


    ''' <summary>
    ''' Message Entries of a Business Object Log 
    ''' </summary>
    ''' <remarks></remarks>

    <ormObject(version:=1, id:=BusinessObjectMessage.ConstObjectID, modulename:=ConstModuleCommons)> Public Class BusinessObjectMessage
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable

        '* schema
        Public Const ConstObjectID = "BusinessObjectMessage"

        ''' <summary>
        ''' Table
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=1)> Public Const ConstPrimaryTableID As String = "tblBusinessObjectMessages"

        ''' <summary>
        ''' Primary Key Entries
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, size:=255, PrimaryKeyOrdinal:=1, _
                         XID:="olog1", title:="Tag", description:="tag to the object message log")> Public Shadows Const ConstFNTag = "MSGLOGTAG"
        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=2, _
                         XID:="olog2", title:="Number", description:="number of the object message")> Public Const ConstFNNo = "IDNO"

        ''' <summary>
        ''' ColumnEntries
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=ObjectMessageType.ConstObjectID & "." & ObjectMessageType.ConstFNUID, _
                         XID:="olog3")> Public Const ConstFNMessageTypeUID = ObjectMessageType.ConstFNUID

        <ormObjectEntry(referenceobjectentry:=ObjectMessageType.ConstObjectID & "." & ObjectMessageType.constFNText, isnullable:=True, _
                         XID:="olog4", title:="Message", description:="the object message")> Public Const ConstFNMessage = "MESSAGE"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, isnullable:=True, _
                         XID:="olog5", title:="ContextID", description:="context of the object message")> Public Const ConstFNContextID = "CONTEXTID"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, isnullable:=True, _
                         XID:="olog6", title:="TupleID", description:="tuple of the object message")> Public Const ConstFNTupleID = "TUPLEID"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, isnullable:=True, _
                         XID:="olog7", title:="EntityID", description:="entity of the object message")> Public Const ConstFNEntityID = "ENTITYID"
        <ormObjectEntry(Datatype:=otDataType.List, isnullable:=True, _
                        XID:="olog8", title:="Parameters", description:="parameters for the message")> Public Const ConstFNParameters = "PARAMETERS"

        <ormObjectEntry(Datatype:=otDataType.Timestamp, isnullable:=True, _
                       XID:="olog9", title:="Timestamp", description:="timestamp of the message")> Public Const ConstFNTimeStamp = "TIMESTAMP"

        <ormObjectEntry(Datatype:=otDataType.Bool, defaultvalue:=False, dbdefaultvalue:="0", _
                       XID:="olog10", title:="Persist", description:="if set than this message will be persisted")> Public Const ConstFNPERSIST = "PERSIST"

        <ormObjectEntry(referenceObjectEntry:=ObjectMessageType.ConstObjectID & "." & ObjectMessageType.constFNArea, isnullable:=True, _
                        XID:="olog11")> Public Const ConstFNArea = "AREA"
        <ormObjectEntry(referenceObjectEntry:=ObjectMessageType.ConstObjectID & "." & ObjectMessageType.constFNWeight, isnullable:=True, _
                       XID:="olog12")> Public Const ConstFNWeight = "WEIGHT"

        <ormObjectEntry(referenceObjectEntry:=User.ConstObjectID & "." & User.ConstFNUsername, isnullable:=True, _
                       XID:="olog13", title:="Username", description:="username of the session")> Public Const ConstFNUsername = "USER"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, isnullable:=True, _
                       XID:="olog14", title:="Session", description:="session in which the error occured")> Public Const ConstFNSessionTAG = "SESSIONTAG"

        <ormObjectEntry(referenceObjectEntry:=SessionMessage.ConstObjectID & "." & SessionMessage.ConstFNID, isnullable:=True, _
                      XID:="olog15", title:="Session Message No", description:="referenced session message no")> Public Const ConstFNSessionMSGNo = "SESSIONMSGNO"

        <ormObjectEntry(referenceObjectEntry:=Workspace.ConstObjectID & "." & Workspace.ConstFNID, isnullable:=True, _
                     XID:="olog16", title:="current Workspace id", description:="current workspace id")> Public Const ConstFNWORKSPACEID = "WORKSPACEID"

        <ormObjectEntry(referenceObjectEntry:=ormObjectDefinition.ConstObjectID & "." & ormObjectDefinition.ConstFNID, isnullable:=True, _
                      XID:="olog21", title:="Objectname", description:="Object name")> Public Const ConstFNObjectname = "Objectname"
        <ormObjectEntry(referenceObjectEntry:=ormObjectFieldEntry.ConstObjectID & "." & ormObjectFieldEntry.ConstFNEntryName, isnullable:=True, _
                      XID:="olog22", title:="Entryname", description:="entry name of the object")> Public Const ConstFNEntryname = "Entryname"

        <ormObjectEntry(datatype:=otDataType.List, size:=255, isnullable:=True, _
                     XID:="olog23", title:="PrimaryKeyValues", description:="values of the primary key of the object")> Public Const ConstFnPkValues = "pkvalues"

        ''' <summary>
        ''' Mappings
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNTag)> Private _tag As String
        <ormObjectEntryMapping(EntryName:=ConstFNNo)> Private _no As Long?
        <ormObjectEntryMapping(EntryName:=ConstFNMessageTypeUID)> Private _typeuid As Long
        <ormObjectEntryMapping(EntryName:=ConstFNMessage)> Private _message As String

        <ormObjectEntryMapping(EntryName:=ConstFNPERSIST)> Private _persistflag As Boolean

        <ormObjectEntryMapping(EntryName:=ConstFNContextID)> Private _ContextID As String
        <ormObjectEntryMapping(EntryName:=ConstFNTupleID)> Private _TupleID As String
        <ormObjectEntryMapping(EntryName:=ConstFNEntityID)> Private _EntitityID As String
        <ormObjectEntryMapping(EntryName:=ConstFNParameters)> Private _Parameters As String()

        <ormObjectEntryMapping(EntryName:=ConstFNArea)> Private _Area As String
        <ormObjectEntryMapping(EntryName:=ConstFNWeight)> Private _Weight As Double?
        <ormObjectEntryMapping(EntryName:=ConstFNTimeStamp)> Private _Timestamp As DateTime?
        <ormObjectEntryMapping(EntryName:=ConstFNUsername)> Private _username As String
        <ormObjectEntryMapping(EntryName:=ConstFNSessionTAG)> Private _sessionid As String
        <ormObjectEntryMapping(EntryName:=ConstFNWORKSPACEID)> Private _workspaceID As String
        <ormObjectEntryMapping(EntryName:=ConstFNSessionMSGNo)> Private _sessionmsgno As Long

        <ormObjectEntryMapping(EntryName:=ConstFNObjectname)> Private _objectname As String
        <ormObjectEntryMapping(EntryName:=ConstFNEntryname)> Private _entryname As String
        <ormObjectEntryMapping(EntryName:=ConstFnPkValues)> Private _objpkvalues As String()


        ''' <summary>
        ''' Relation to ScheduleDefinition
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkObject:=GetType(ObjectMessageType), toprimaryKeys:={ConstFNMessageTypeUID}, _
                     cascadeonCreate:=True, cascadeOnDelete:=False, cascadeOnUpdate:=False)> _
        Public Const ConstRMessageType = "RelMessageType"

        <ormObjectEntryMapping(relationName:=ConstRMessageType, infusemode:=otInfuseMode.OnCreate OrElse otInfuseMode.OnInject OrElse otInfuseMode.OnDemand)> Private _messagetype As New ObjectMessageType

        ''' <summary>
        ''' runtime dynamic members
        ''' </summary>
        ''' <remarks></remarks>
        Private _lock As New Object
        Private _sender As Object


#Region "properties"

        ''' <summary>
        ''' Gets or sets the persistflag.
        ''' </summary>
        ''' <value>The persistflag.</value>
        Public Property IsPersisted() As Boolean
            Get
                Return Me._persistflag
            End Get
            Set(value As Boolean)
                Me._persistflag = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the sender.
        ''' </summary>
        ''' <value>The sender.</value>
        Public Property Sender() As Object
            Get
                Return Me._sender
            End Get
            Set(value As Object)
                If value IsNot Nothing Then
                    Dim apersistable As iormRelationalPersistable = TryCast(value, iormRelationalPersistable)
                    If apersistable IsNot Nothing Then
                        Me.Objectname = apersistable.ObjectID
                        Dim aList As New List(Of String)
                        For Each aValue As Object In apersistable.ObjectPrimaryKeyValues
                            aList.Add(CStr(aValue))
                        Next
                        Me.LoggableKeyValues = aList.ToArray
                    Else
                        Me.Objectname = value.GetType.FullName
                    End If
                End If
                _sender = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the username.
        ''' </summary>
        ''' <value>The username.</value>
        Public Property Username() As String
            Get
                Return Me._username
            End Get
            Set(value As String)
                SetValue(ConstFNUsername, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the primary key values of the loggable sender object .
        ''' </summary>
        ''' <value>The objpkvalues.</value>
        Public Property LoggableKeyValues() As String()
            Get
                Return Me._objpkvalues
            End Get
            Set(value As String())
                SetValue(ConstFnPkValues, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the entryname.
        ''' </summary>
        ''' <value>The entryname.</value>
        Public Property Entryname() As String
            Get
                Return Me._entryname
            End Get
            Set(value As String)
                SetValue(ConstFNEntityID, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the objectname.
        ''' </summary>
        ''' <value>The objectname.</value>
        Public Property Objectname() As String
            Get
                Return Me._objectname
            End Get
            Set(value As String)
                SetValue(ConstFNObjectname, value)
            End Set
        End Property

        ''' <summary>
        ''' returns true if data object has primary keys and is alive
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property IsDataObject As Boolean
            Get
                If _tag IsNot Nothing AndAlso _tag <> String.Empty AndAlso _no.HasValue AndAlso _no > 0 AndAlso Me.IsAlive(throwError:=False) Then
                    Return True
                End If
                Return False
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the workspace ID.
        ''' </summary>
        ''' <value>The workspace ID.</value>
        Public Property WorkspaceID() As String
            Get
                Return Me._workspaceID
            End Get
            Set(value As String)
                SetValue(ConstFNWORKSPACEID, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the sessionid.
        ''' </summary>
        ''' <value>The sessionid.</value>
        Public Property Sessionid() As String
            Get
                Return Me._sessionid
            End Get
            Set(value As String)
                SetValue(ConstFNSessionTAG, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the sessionmsgno.
        ''' </summary>
        ''' <value>The sessionmsgno.</value>
        Public Property SessionMessageNo() As Long
            Get
                Return Me._sessionmsgno
            End Get
            Set(value As Long)
                SetValue(ConstFNSessionMSGNo, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the weight.
        ''' </summary>
        ''' <value>The weight.</value>
        Public Property Weight() As Double?
            Get
                Return Me._Weight
            End Get
            Set(value As Double?)
                Me._Weight = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the area.
        ''' </summary>
        ''' <value>The area.</value>
        Public Property Area() As String
            Get
                Return Me._Area
            End Get
            Set(value As String)
                SetValue(ConstFNArea, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the parameters.
        ''' </summary>
        ''' <value>The parameters.</value>
        Public Property Parameters() As String()
            Get
                Return Me._Parameters
            End Get
            Set(value As String())
                SetValue(ConstFNParameters, value)
            End Set
        End Property

        ''' <summary>
        ''' gets or sets the tag of the log message
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Tag() As String
            Get
                Return _tag
            End Get
            Set(value As String)
                SetValue(ConstFNTag, value)
            End Set
        End Property

        ''' <summary>
        ''' gets the index number
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property No() As Long
            Get
                Return _no
            End Get
            Set(value As Long)
                If Not Me.IsDataObject Then
                    SetValue(ConstFNNo, value)
                End If
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the message type uid
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MessageTypeID() As Long
            Get
                Return _typeuid
            End Get
            Set(avalue As Long)
                SetValue(ConstFNMessageTypeUID, avalue)
            End Set
        End Property

        ''' <summary>
        ''' sets or gets the messagetext
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Message As String
            Get
                Return _message
            End Get
            Private Set(value As String)
                SetValue(ConstFNMessage, value)
            End Set
        End Property

        ''' <summary>
        ''' gets the Message type object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property MessageType As ObjectMessageType
            Get
                If Me.GetRelationStatus(ConstRMessageType) = ormRelationManager.RelationStatus.Unloaded Then InfuseRelation(ConstRMessageType)
                Return _messagetype
            End Get

        End Property

        ''' <summary>
        ''' returns the highest Status Item
        ''' </summary>
        ''' <param name="domainid"></param>
        ''' <param name="statustype"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HighestStatusItems(Optional domainid As String = Nothing, Optional statustype As String = Nothing) As IList(Of StatusItem)
            Get
                If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
                Dim aShortlist As IEnumerable(Of StatusItem) = Me.StatusItems(domainid:=domainid, statustype:=statustype)
                If aShortlist Is Nothing OrElse aShortlist.Count = 0 Then Return New List(Of StatusItem)
                Dim highest As Integer = aShortlist.Max(Function(x) x.Weight)
                aShortlist = aShortlist.Where(Function(x) x.Weight = highest)
                Return aShortlist.ToList
            End Get
        End Property
        ''' <summary>
        ''' returns the status items associated with this message
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property StatusItems(Optional domainid As String = Nothing, Optional statustype As String = Nothing) As IList(Of Commons.StatusItem)
            Get
                If Me.MessageType IsNot Nothing Then
                    If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
                    Return Me.MessageType.StatusItems(domainid:=domainid, statustype:=statustype)
                End If
                Return New List(Of StatusItem)
            End Get
        End Property
        ''' <summary>
        ''' gets or sets the context identifier
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ContextIdentifier As Object
            Get
                Return _ContextID
            End Get
            Set(value As Object)
                SetValue(ConstFNContextID, value)
            End Set
        End Property

        ''' <summary>
        ''' sets or gets the data tupple identifier
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TupleIdentifier As Object
            Get
                Return _TupleID
            End Get
            Set(avalue As Object)
                SetValue(ConstFNTupleID, value:=avalue)
            End Set
        End Property

        ''' <summary>
        ''' gets or sets the entitity identifier
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property EntityIdentifier As Object
            Get
                Return _EntitityID
            End Get
            Set(value As Object)
                SetValue(ConstFNEntityID, value)
            End Set
        End Property
#End Region



        ''' <summary>
        ''' loads and infuses a message log member
        ''' </summary>
        ''' <param name="msglogtag"></param>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Retrieve(ByVal msglogtag As String, ByVal ID As Long) As BusinessObjectMessage
            Dim primarykey() As Object = {msglogtag.ToUpper, ID}
            Return ormBusinessObject.RetrieveDataObject(Of BusinessObjectMessage)(primarykey)
        End Function


        ''' <summary>
        ''' Create a persistable Message Log Member by primary key
        ''' </summary>
        ''' <param name="msglogtag"></param>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal typeuid As Long, _
                                      Optional ByVal msglogtag As String = Nothing, _
                                      Optional ByVal no As Long? = Nothing, _
                                      Optional ByVal contextIdentifier As String = Nothing, _
                                      Optional ByVal tupleIdentifier As String = Nothing, _
                                      Optional ByVal entitityIdentifier As String = Nothing, _
                                      Optional parameters As Object() = Nothing,
                                      Optional ByVal domainid As String = Nothing, _
                                      Optional checkUnique As Boolean = False, _
                                      Optional runtimeOnly As Boolean = True) As BusinessObjectMessage
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim aRecord As New ormRecord
            With aRecord
                If msglogtag IsNot Nothing Then .SetValue(ConstFNTag, msglogtag.ToUpper)
                .SetValue(ConstFNMessageTypeUID, typeuid)
                If no.HasValue Then .SetValue(ConstFNNo, no.Value)
                .SetValue(ConstFNDomainID, domainid)
                .SetValue(ConstFNContextID, contextIdentifier)
                .SetValue(ConstFNTupleID, tupleIdentifier)
                .SetValue(ConstFNEntityID, entitityIdentifier)

                If parameters IsNot Nothing Then .SetValue(ConstFNParameters, Core.DataType.ToString(parameters))
            End With
            '''
            ''' create a not alive ObjectMessage
            ''' 
            If msglogtag Is Nothing OrElse Not no.HasValue Then
                Dim anObjectMessage As BusinessObjectMessage = ot.CurrentSession.DataObjectProvider(objectid:=BusinessObjectMessage.ConstObjectID).NewOrmDataObject(GetType(BusinessObjectMessage))
                anObjectMessage.Feed(aRecord)
                Return anObjectMessage
            Else
                ''' create a normal ObjectMessage which is alive
                Return ormBusinessObject.CreateDataObject(Of BusinessObjectMessage)(aRecord, checkUnique:=checkUnique, runtimeOnly:=runtimeOnly)
            End If

        End Function

        ''' <summary>
        ''' handles the default value needed event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectMessage_OnDefaultValuesNeeded(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnCreateDefaultValuesNeeded

            ''' defaults
            If Not e.Record.HasIndex(ConstFNSessionTAG) OrElse e.Record.GetValue(ConstFNSessionTAG) Is Nothing Then e.Record.SetValue(ConstFNSessionTAG, CurrentSession.SessionID)
            If Not e.Record.HasIndex(ConstFNUsername) OrElse e.Record.GetValue(ConstFNUsername) Is Nothing Then e.Record.SetValue(ConstFNUsername, CurrentSession.CurrentUsername)
            If Not e.Record.HasIndex(ConstFNDomainID) OrElse e.Record.GetValue(ConstFNDomainID) Is Nothing Then e.Record.SetValue(ConstFNDomainID, CurrentSession.CurrentDomainID)
            If Not e.Record.HasIndex(ConstFNWORKSPACEID) OrElse e.Record.GetValue(ConstFNWORKSPACEID) Is Nothing Then e.Record.SetValue(ConstFNWORKSPACEID, CurrentSession.CurrentWorkspaceID)
            If Not e.Record.HasIndex(ConstFNTimeStamp) OrElse e.Record.GetValue(ConstFNTimeStamp) Is Nothing Then e.Record.SetValue(ConstFNTimeStamp, Date.Now)

        End Sub

        Private Function FormatMessage(messagetext As String) As String
            Dim aBuilder As Text.StringBuilder
            Dim aMessageDefinition As ObjectMessageType = Me.MessageType
            If aMessageDefinition IsNot Nothing Then

                ''' set the values from the definition
                If messagetext IsNot Nothing Then
                    aBuilder = New Text.StringBuilder(messagetext)
                Else
                    aBuilder = New Text.StringBuilder(aMessageDefinition.Message)
                End If

                Me.Weight = aMessageDefinition.Weight
                Me.Area = aMessageDefinition.Area
                If Me.Sessionid Is Nothing Then Me.Sessionid = CurrentSession.SessionID

                ''' replace
                ''' 
                If Me.TupleIdentifier IsNot Nothing Then
                    aBuilder.Replace("%uid%", Me.TupleIdentifier)
                    aBuilder.Replace("%Tupleid%", Me.TupleIdentifier)
                    aBuilder.Replace("%Tupleidentifier%", Me.TupleIdentifier)
                End If
                If Me.ContextIdentifier IsNot Nothing Then
                    aBuilder.Replace("%contextid%", ContextIdentifier)
                    aBuilder.Replace("%Contextidentifier%", ContextIdentifier)
                End If
                If Me.EntityIdentifier IsNot Nothing Then
                    aBuilder.Replace("%entitiyid%", EntityIdentifier)
                    aBuilder.Replace("%Entitiyidentifier%", EntityIdentifier)
                    aBuilder.Replace("%ids%", EntityIdentifier)
                End If

                'aMember.message = Replace(aMember.message, "%rowno%", aRowNo)
                aBuilder.Replace("%type%", aMessageDefinition.Type.ToString.ToUpper)
                aBuilder.Replace("%errno%", Strings.Format(aMessageDefinition.ID, "00000"))
                Dim formattimestamp As String = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern & " " & System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern

                '*
                For i = LBound(Me.Parameters) To UBound(Me.Parameters)
                    Dim aValue As Object = Me.Parameters(i)
                    If IsDate(aValue) Then
                        aValue = Format(CDate(aValue), formattimestamp)
                    End If
                    aBuilder.Replace("%" & i + 1 & "%", CStr(aValue))
                Next i
            Else
                aBuilder.AppendFormat("> Message type {0} not found.", Me.MessageTypeID)
                aBuilder.AppendLine()
                aBuilder.AppendFormat("> ContextIdentifier: '{1}', TupleIdentifier: '{0}', EntityIdentifier: {2}", Me.TupleIdentifier, Me.ContextIdentifier, Me.EntityIdentifier)
                aBuilder.AppendLine()

                For i = LBound(Me.Parameters) To UBound(Me.Parameters)
                    aBuilder.AppendFormat("> Message Parameter #{0}: '{1}'", i, Me.Parameters(i))
                    aBuilder.AppendLine()
                Next i

            End If

            Return aBuilder.ToString
        End Function

        ''' <summary>
        ''' Infused Handler to set some stuff
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectMessage_OnInfused(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnInfused
            Me.Message = FormatMessage(DirectCast(e.DataObject, BusinessObjectMessage)._message)
        End Sub

        ''' <summary>
        ''' On deleted Handler
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ObjectMessage_OnRelationRetrieveNeeded(sender As Object, e As ormDataObjectRelationEventArgs) Handles Me.OnRelationRetrieveNeeded

        End Sub
    End Class
End Namespace