
REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK UI MQF DATA MODEL
REM ***********
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2014-04-24
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************''' <summary>

Option Explicit On
Option Strict On

Imports System.Data
Imports OnTrack.Xchange

'************
'************ MQFFeedWizardDataModel is a DataTable Representation of the MessageQueue (file)

Public Class UIMQFDataModel
    Inherits DataTable
    Implements ComponentModel.INotifyPropertyChanged

    Public Const ConstFNRowType As String = "$RowType"
    Public Const ConstFNMessageID As String = "$MessageID"
    Public Const ConstFNTupleID As String = "$TupleID"
    Public Const ConstFNMQFOperation As String = "$MQFOP"
    Public Const ConstFNMQFStatus As String = "$MQFStatus"
    Public Const ConstFNMQFMessages As String = "$MQFMessages"
    Public Const ConstFNMQFTimestamp As String = "$MQFTimestamp"

    Public Enum internalRowtype
        MQMEssage = 1
        XEnvelope = 2
    End Enum

    Private _messagequeue As Xchange.MessageQueue
    Private _isInitialized As Boolean = False

    Public Event PropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs) Implements ComponentModel.INotifyPropertyChanged.PropertyChanged

    ''' <summary>
    '''  constructor
    ''' </summary>
    ''' <param name="messagequeue"></param>
    ''' <remarks></remarks>
    Public Sub New(messagequeue As MessageQueue)
        MyBase.New()

        _messagequeue = messagequeue
        '
        '
        'MQFTable
        '
        Me.TableName = "MQFTable"
    End Sub

#Region "Properties"
    ''' <summary>
    ''' Gets or sets the is initialized.
    ''' </summary>
    ''' <value>The is initialized.</value>
    Public Property IsInitialized() As Boolean
        Get
            Return Me._isInitialized
        End Get
        Private Set(value As Boolean)
            Me._isInitialized = value
        End Set
    End Property

    ''' <summary>
    ''' returns the message queue
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property MessageQueue As MessageQueue
        Get
            MessageQueue = _messagequeue
        End Get
    End Property
#End Region

    ''' <summary>
    ''' initialize the DataModel
    ''' </summary>
    ''' <param name="force"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Initialize(Optional force As Boolean = False) As Boolean
        If Me.IsInitialized AndAlso Not force Then Return True

        Dim aXConfig As Xchange.XChangeConfiguration = _messagequeue.XChangeConfig

        Dim aMQMessageIDColumn As DataColumn = New DataColumn(columnName:=ConstFNMessageID, dataType:=GetType(Long))
        aMQMessageIDColumn.ReadOnly = True
        aMQMessageIDColumn.Caption = ""
        'aMQFRowTypeColumn.Caption = anEntry.ObjectEntryDefinition.Title
        Me.Columns.Add(aMQMessageIDColumn)
        Me.PrimaryKey = {aMQMessageIDColumn}

        Dim aMQFRowTypeColumn As DataColumn = New DataColumn(columnName:=ConstFNRowType, dataType:=GetType(internalRowtype))
        'aMQFRowTypeColumn.ReadOnly = anEntry.IsReadOnly
        'aMQFRowTypeColumn.Caption = anEntry.ObjectEntryDefinition.Title
        aMQFRowTypeColumn.Caption = ""
        Me.Columns.Add(aMQFRowTypeColumn)

        Dim aMQFTupleIDColumn As DataColumn = New DataColumn(columnName:=ConstFNTupleID, dataType:=GetType(String))
        'aMQFRowTypeColumn.ReadOnly = anEntry.IsReadOnly
        'aMQFRowTypeColumn.Caption = anEntry.ObjectEntryDefinition.Title
        aMQFTupleIDColumn.Caption = ""
        Me.Columns.Add(aMQFTupleIDColumn)

        ''' add the Operation
        Dim aMQFOperationColumn As DataColumn = New DataColumn(columnName:=ConstFNMQFOperation, dataType:=GetType(String))
        'amqfStatusColumn.ReadOnly = True
        aMQFOperationColumn.Caption = "Operation"
        Me.Columns.Add(aMQFOperationColumn)

        ''' add the exchange entries
        For Each aSlotid As String In _messagequeue.UsedSlotIDs
            Dim aList As IList(Of IXChangeConfigEntry) = _messagequeue.XChangeConfig.GetEntriesByMappingOrdinal(New Database.Ordinal(aSlotid))
            If aList IsNot Nothing And aList.Count > 0 Then
                Dim anEntry As IXChangeConfigEntry = aList.First
                If anEntry.IsXChanged Then
                    'Dim aType As System.Type = ot.GetDatatypeMappingOf(anEntry.ObjectEntryDefinition.Datatype)
                    Dim aColumn As DataColumn = New DataColumn(columnName:=aSlotid, dataType:=GetType(String)) 'everything in the table is a string as it is in excel
                    aColumn.ReadOnly = anEntry.IsReadOnly
                    aColumn.Caption = anEntry.ObjectEntryDefinition.XID & ":" & anEntry.ObjectEntryDefinition.Title
                    Me.Columns.Add(aColumn)
                End If
            End If
        Next

        ''' add the Status
        Dim amqfStatusColumn As DataColumn = New DataColumn(columnName:=ConstFNMQFStatus, dataType:=GetType(String))
        amqfStatusColumn.ReadOnly = True
        amqfStatusColumn.Caption = "Status"
        Me.Columns.Add(amqfStatusColumn)

        ''' add the Messages
        Dim aMQFMessagesColumn As DataColumn = New DataColumn(columnName:=ConstFNMQFMessages, dataType:=GetType(String))
        aMQFMessagesColumn.ReadOnly = True
        aMQFMessagesColumn.Caption = "Messages"
        Me.Columns.Add(aMQFMessagesColumn)

        ''' add the timestamp
        Dim aMQFTimestampColumn As DataColumn = New DataColumn(columnName:=ConstFNMQFTimestamp, dataType:=GetType(DateTime))
        aMQFTimestampColumn.ReadOnly = True
        aMQFTimestampColumn.Caption = "Timestamp"
        Me.Columns.Add(aMQFTimestampColumn)


        Me.IsInitialized = True
        Return True
    End Function

    ''' <summary>
    ''' Returns the StatusItem of the messageno
    ''' </summary>
    ''' <param name="messageno"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetStatusItemOf(messageno As Long) As Commons.StatusItem
        If _messagequeue Is Nothing Then Return Nothing

        Dim aList As IList(Of MQMessage) = CType(_messagequeue.Messages.Where(Function(x) x.IDNO = messageno), Global.System.Collections.Generic.IList(Of Global.OnTrack.Xchange.MQMessage))
        Return aList.First.Statusitem
    End Function
    ''' <summary>
    ''' load data from the mqf into the data model
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadData() As Boolean
        If Not Me.IsInitialized AndAlso Not Me.Initialize Then Return False

        For Each aMessage As MQMessage In _messagequeue.Messages
            Dim aRow As DataRow = Me.NewRow
            For Each aColumn As DataColumn In Me.Columns
                Select Case aColumn.ColumnName
                    Case ConstFNMessageID
                        aRow.Item(aColumn.ColumnName) = aMessage.IDNO
                    Case ConstFNRowType
                        aRow.Item(aColumn.ColumnName) = internalRowtype.MQMEssage
                    Case ConstFNMQFOperation
                        aRow.Item(aColumn.ColumnName) = aMessage.Action
                    Case ConstFNMQFMessages
                        'aRow.Item(aColumn.ColumnName) =  aMessage.ObjectMessageLog.GetAllMessageTexts
                    Case ConstFNMQFStatus
                        aRow.Item(aColumn.ColumnName) = aMessage.Statuscode
                    Case ConstFNMQFTimestamp
                        'aRow.Item(aColumn.ColumnName) = aMessage.ChangeTimeStamp
                    Case ConstFNTupleID
                        aRow.Item(aColumn.ColumnName) = aMessage.TupleIdentifier
                    Case Else
                        ''' check if the slot exists and add it
                        If IsNumeric(aColumn.ColumnName) AndAlso aMessage.Slots.ContainsKey(aColumn.ColumnName) Then
                            If aMessage.Slots.Item(aColumn.ColumnName).Value IsNot Nothing Then
                                aRow.Item(aColumn.ColumnName) = aMessage.Slots.Item(aColumn.ColumnName).Value.ToString
                            Else
                                aRow.Item(aColumn.ColumnName) = ""
                            End If

                        End If
                End Select
            Next
            AddHandler aMessage.PropertyChanged, AddressOf Me.OnPropertyChanged
            AddHandler aMessage.OnSlotValueChanged, AddressOf Me.OnSlotValueChanged
            AddHandler aMessage.OnPreChecked, AddressOf Me.OnMessagePrechecked
            AddHandler aMessage.OnProcessed, AddressOf Me.OnMessageProcessed
            Me.Rows.Add(aRow)
        Next

        Return True

    End Function

    ''' <summary>
    ''' Event Handler for the Message Prechecked Event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub OnMessagePrechecked(sender As Object, e As Xchange.MQMessage.EventArgs)
        Dim msgid As Long = e.Mqmessage.IDNO
        Dim aRow As DataRow() = Me.Select(ConstFNMessageID & "=" & msgid.ToString)
        If aRow IsNot Nothing AndAlso aRow.Count > 0 Then

            Me.Columns.Item(ConstFNMQFStatus).ReadOnly = False
            aRow(0).Item(ConstFNMQFStatus) = e.Mqmessage.Statuscode
            RaiseEvent PropertyChanged(aRow, New System.ComponentModel.PropertyChangedEventArgs(ConstFNMQFStatus))
            Me.Columns.Item(ConstFNMQFStatus).ReadOnly = True

            If e.Mqmessage.PrecheckedOn IsNot Nothing Then
                Me.Columns.Item(ConstFNMQFTimestamp).ReadOnly = False
                aRow(0).Item(ConstFNMQFTimestamp) = e.Mqmessage.PrecheckedOn
                RaiseEvent PropertyChanged(aRow, New System.ComponentModel.PropertyChangedEventArgs(ConstFNMQFTimestamp))
                Me.Columns.Item(ConstFNMQFTimestamp).ReadOnly = True
            End If

            Dim aMessageBlock As New StringBuilder

            For Each aMessage As Database.BusinessObjectMessage In e.Mqmessage.ObjectMessageLog
                aMessageBlock.AppendFormat("{0:000000}:", aMessage.MessageTypeID)
                aMessageBlock.AppendLine(aMessage.Message)
            Next
            Me.Columns.Item(ConstFNMQFMessages).ReadOnly = False
            aRow(0).Item(ConstFNMQFMessages) = aMessageBlock.ToString
            RaiseEvent PropertyChanged(aRow, New System.ComponentModel.PropertyChangedEventArgs(ConstFNMQFMessages))
            Me.Columns.Item(ConstFNMQFMessages).ReadOnly = True

            'aRow(0).Item(ConstFNMQFStatus) = TryCast(sender, MQMessage).Statuscode
            ' RaiseEvent PropertyChanged(aRow, New System.ComponentModel.PropertyChangedEventArgs(e.PropertyName))
            'aRow(0).Item(ConstFNMQFStatus) = TryCast(sender, MQMessage).Statuscode
            ' RaiseEvent PropertyChanged(aRow, New System.ComponentModel.PropertyChangedEventArgs(e.PropertyName))
        End If

    End Sub


    ''' <summary>
    ''' Event Handler for the Message Processed Event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub OnMessageProcessed(sender As Object, e As Xchange.MQMessage.EventArgs)
        Dim msgid As Long = e.Mqmessage.IDNO
        Dim aRow As DataRow() = Me.Select(ConstFNMessageID & "=" & msgid.ToString)
        If aRow IsNot Nothing AndAlso aRow.Count > 0 Then

            Me.Columns.Item(ConstFNMQFStatus).ReadOnly = False
            aRow(0).Item(ConstFNMQFStatus) = e.Mqmessage.Statuscode
            RaiseEvent PropertyChanged(aRow, New System.ComponentModel.PropertyChangedEventArgs(ConstFNMQFStatus))
            Me.Columns.Item(ConstFNMQFStatus).ReadOnly = True

            If e.Mqmessage.ProcessedOn IsNot Nothing Then
                Me.Columns.Item(ConstFNMQFTimestamp).ReadOnly = False
                aRow(0).Item(ConstFNMQFTimestamp) = e.Mqmessage.ProcessedOn
                RaiseEvent PropertyChanged(aRow, New System.ComponentModel.PropertyChangedEventArgs(ConstFNMQFTimestamp))
                Me.Columns.Item(ConstFNMQFTimestamp).ReadOnly = True
            End If

            Dim aMessageBlock As New StringBuilder

            For Each aMessage As Database.BusinessObjectMessage In e.Mqmessage.ObjectMessageLog
                aMessageBlock.AppendFormat("{0:000000}:", aMessage.MessageTypeID)
                aMessageBlock.AppendLine(aMessage.Message)
            Next
            Me.Columns.Item(ConstFNMQFMessages).ReadOnly = False
            aRow(0).Item(ConstFNMQFMessages) = aMessageBlock.ToString
            RaiseEvent PropertyChanged(aRow, New System.ComponentModel.PropertyChangedEventArgs(ConstFNMQFMessages))
            Me.Columns.Item(ConstFNMQFMessages).ReadOnly = True

            'aRow(0).Item(ConstFNMQFStatus) = TryCast(sender, MQMessage).Statuscode
            ' RaiseEvent PropertyChanged(aRow, New System.ComponentModel.PropertyChangedEventArgs(e.PropertyName))
            'aRow(0).Item(ConstFNMQFStatus) = TryCast(sender, MQMessage).Statuscode
            ' RaiseEvent PropertyChanged(aRow, New System.ComponentModel.PropertyChangedEventArgs(e.PropertyName))
        End If

    End Sub
    ''' <summary>
    ''' event handler if property changed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub OnPropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs)

        If sender.GetType Is GetType(MQMessage) Then
            Dim msgid As Long = TryCast(sender, MQMessage).IDNO
            Dim aRow As DataRow() = Me.Select(ConstFNMessageID & "=" & msgid.ToString)
            If aRow IsNot Nothing AndAlso aRow.Count > 0 Then

                Select Case e.PropertyName
                    Case MQMessage.ConstFNProcStatus
                        aRow(0).Table.Columns.Item(ConstFNMQFStatus).ReadOnly = False
                        aRow(0).Item(ConstFNMQFStatus) = TryCast(sender, MQMessage).Statuscode
                        aRow(0).Table.Columns.Item(ConstFNMQFStatus).ReadOnly = True
                        RaiseEvent PropertyChanged(aRow, New System.ComponentModel.PropertyChangedEventArgs(e.PropertyName))
                    Case MQMessage.ConstFNPROCSTAMP
                        aRow(0).Table.Columns.Item(ConstFNMQFTimestamp).ReadOnly = False
                        If TryCast(sender, MQMessage).ProcessedOn IsNot Nothing Then
                            aRow(0).Item(ConstFNMQFTimestamp) = TryCast(sender, MQMessage).ProcessedOn
                        Else
                            aRow(0).Item(ConstFNMQFTimestamp) = DBNull.Value
                        End If

                        aRow(0).Table.Columns.Item(ConstFNMQFTimestamp).ReadOnly = True
                        RaiseEvent PropertyChanged(aRow, New System.ComponentModel.PropertyChangedEventArgs(e.PropertyName))
                    Case MQMessage.ConstFNProcessed
                        'aRow(0).Item(ConstFNMQFStatus) = TryCast(sender, MQMessage).Statuscode
                        ' RaiseEvent PropertyChanged(aRow, New System.ComponentModel.PropertyChangedEventArgs(e.PropertyName))
                    Case MQMessage.ConstFNProcessable
                        'aRow(0).Item(ConstFNMQFStatus) = TryCast(sender, MQMessage).Statuscode
                        ' RaiseEvent PropertyChanged(aRow, New System.ComponentModel.PropertyChangedEventArgs(e.PropertyName))
                End Select

            End If

        End If
    End Sub

    ''' <summary>
    ''' event handler if a slot value changed from the associated message queue message
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub OnSlotValueChanged(sender As Object, e As XSlot.EventArgs)

        If sender.GetType Is GetType(MQMessage) Then
            Dim msgid As Long = TryCast(sender, MQMessage).IDNO
            Dim aRow As DataRow() = Me.Select(ConstFNMessageID & "=" & msgid.ToString)
            If aRow IsNot Nothing AndAlso aRow.Count > 0 Then
                Dim anEntryname As String = e.XSlot.XChangeEntry.ObjectEntryname
                If aRow(0).Table.Columns.Contains(e.XSlot.XChangeEntry.Ordinal.ToString) Then
                    If Not aRow(0).Table.Columns(e.XSlot.XChangeEntry.Ordinal.ToString).ReadOnly Then
                        aRow(0).Item(e.XSlot.XChangeEntry.Ordinal.ToString) = e.XSlot.HostValue
                        RaiseEvent PropertyChanged(aRow, New System.ComponentModel.PropertyChangedEventArgs(e.XSlot.XChangeEntry.Ordinal.ToString))
                    End If
                End If
            End If
        End If
    End Sub
End Class
