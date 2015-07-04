
REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** XChangeManager Classes Runtime Structures 
REM ***********
REM *********** Version: X.YY
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************
Option Explicit On
Imports System.Diagnostics.Debug
Imports System.Collections.Specialized

Imports OnTrack
Imports OnTrack.Database
Imports OnTrack.Scheduling
Imports OnTrack.Deliverables
Imports OnTrack.Parts
Imports OnTrack.Configurables
Imports OnTrack.XChange.ConvertRequestEventArgs
Imports OnTrack.Core


Namespace OnTrack.XChange

    ''' <summary>
    ''' Arguments for the ConvertRequest and Result Arguments
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ConvertRequestEventArgs
        Inherits EventArgs

        Public Enum convertValueType
            Hostvalue
            DBValue
        End Enum

        Private _valuetype As convertValueType
        Private _hostvalue As Object = Nothing
        Private _dbvalue As Object = Nothing
        Private _HostValueisNull As Boolean = False
        Private _HostValueisEmpty As Boolean = False
        Private _dbValueisNull As Boolean = False
        Private _dbValueIsEmpty As Boolean = False
        Private _datatype As otDataType = 0

        ' result
        Private _result As Boolean = False
        Private _msglog As BusinessObjectMessageLog

        Public Sub New(datatype As otDataType, valuetype As convertValueType, value As Object,
                       Optional isnull As Boolean = False, Optional isempty As Boolean = False, Optional msglog As BusinessObjectMessageLog = Nothing)
            _datatype = datatype
            _valuetype = valuetype
            Me.Value = value
            Me.IsEmpty = isempty
            Me.IsNull = isnull

            _msglog = msglog
        End Sub

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the is null.
        ''' </summary>
        ''' <value>The is null.</value>
        Public Property IsNull() As Boolean
            Get
                If _valuetype = convertValueType.Hostvalue Then
                    Return Me._HostValueisNull
                Else
                    Return Me._dbValueisNull
                End If
            End Get
            Set(value As Boolean)
                Me._HostValueisNull = value
                If _valuetype = convertValueType.Hostvalue Then
                    Me._HostValueisNull = value
                Else
                    Me._dbValueisNull = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the is empty.
        ''' </summary>
        ''' <value>The is empty.</value>
        Public Property IsEmpty() As Boolean
            Get
                If _valuetype = convertValueType.Hostvalue Then
                    Return Me._HostValueisEmpty
                Else
                    Return Me._dbValueIsEmpty
                End If
            End Get
            Set(value As Boolean)
                Me._HostValueisNull = value
                If _valuetype = convertValueType.Hostvalue Then
                    Me._HostValueisEmpty = value
                Else
                    Me._dbValueIsEmpty = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the datatype.
        ''' </summary>
        ''' <value>The datatype.</value>
        Public Property Datatype() As otDataType
            Get
                Return Me._datatype
            End Get
            Set(value As otDataType)
                Me._datatype = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the msglog.
        ''' </summary>
        ''' <value>The msglog.</value>
        Public Property Msglog() As BusinessObjectMessageLog
            Get
                Return Me._msglog
            End Get
            Set(value As BusinessObjectMessageLog)
                Me._msglog = value
            End Set
        End Property


        ''' <summary>
        ''' Gets or sets the convert succeeded.
        ''' </summary>
        ''' <value>The convert succeeded.</value>
        Public Property result() As Boolean
            Get
                Return Me._result
            End Get
            Set(value As Boolean)
                Me._result = value
            End Set
        End Property
        ''' <summary>
        ''' returns the value to be converted
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Value As Object
            Get
                If _valuetype = convertValueType.DBValue Then
                    Return _dbvalue
                Else
                    Return _hostvalue
                End If
            End Get
            Set(value As Object)
                If _valuetype = convertValueType.DBValue Then
                    _dbvalue = value
                    _hostvalue = Nothing
                Else
                    _dbvalue = Nothing
                    _hostvalue = value
                End If
            End Set
        End Property
        ''' <summary>
        ''' returns the converted value 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConvertedValue As Object
            Get
                If _valuetype = convertValueType.Hostvalue Then
                    Return _dbvalue
                Else
                    Return _hostvalue
                End If
            End Get
            Set(value As Object)
                If _valuetype = convertValueType.Hostvalue Then
                    _dbvalue = value
                    _hostvalue = Nothing
                Else
                    _dbvalue = Nothing
                    _hostvalue = value
                End If
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the dbvalue.
        ''' </summary>
        ''' <value>The dbvalue.</value>
        Public Property Dbvalue() As Object
            Get
                Return Me._dbvalue
            End Get
            Set(value As Object)
                Me._dbvalue = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the hostvalue.
        ''' </summary>
        ''' <value>The hostvalue.</value>
        Public Property Hostvalue() As Object
            Get
                Return Me._hostvalue
            End Get
            Set(value As Object)
                Me._hostvalue = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the host valueis null.
        ''' </summary>
        ''' <value>The host valueis null.</value>
        Public Property HostValueisNull() As Boolean
            Get
                Return Me._HostValueisNull
            End Get
            Set(value As Boolean)
                Me._HostValueisNull = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the host valueis empty.
        ''' </summary>
        ''' <value>The host valueis empty.</value>
        Public Property HostValueisEmpty() As Boolean
            Get
                Return Me._HostValueisEmpty
            End Get
            Set(value As Boolean)
                Me._HostValueisEmpty = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the db valueis null.
        ''' </summary>
        ''' <value>The db valueis null.</value>
        Public Property DbValueisNull() As Boolean
            Get
                Return Me._dbValueisNull
            End Get
            Set(value As Boolean)
                Me._dbValueisNull = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the db value is empty.
        ''' </summary>
        ''' <value>The db value is empty.</value>
        Public Property DbValueIsEmpty() As Boolean
            Get
                Return Me._dbValueIsEmpty
            End Get
            Set(value As Boolean)
                Me._dbValueIsEmpty = value
            End Set
        End Property

#End Region
    End Class

    ''' <summary>
    ''' XBag is an arbitary runtime and non persistable XChange Data Object which constists of different XEnvelopes ordered by
    ''' ordinals.
    ''' An XBag an Default persistable XChangeConfig
    ''' </summary>
    ''' <remarks>
    ''' design principles:
    ''' 
    ''' the XBag consists of XEnvelopes which are the record or line by line based elements
    ''' Processing the XBag means processing all the envelopes in the bag
    ''' 
    ''' prechecking is necessary to process the elements
    ''' 
    ''' ''' for transformation of data values of the slots especially in the case of special values of the exchanging sub system
    ''' the events 
    ''' 
    ''' ConvertRequest2HostValue
    ''' ConvertRequest2DBValue 
    ''' 
    ''' can be used. These are propagated to the XBAG level
    ''' 
    ''' </remarks>
    Public Class XBag
        Implements IEnumerable(Of XEnvelope)

        '* default Config we are looking over
        Private _XChangeDefaultConfig As XChangeConfiguration
        Private _XCmd As otXChangeCommandType = 0

        '* real Attributes used after prepared
        Private _usedAttributes As New Dictionary(Of String, IXChangeConfigEntry)
        Private _usedObjects As New Dictionary(Of String, IXChangeConfigEntry)

        '** all the member envelopes
        Private WithEvents _defaultEnvelope As New XEnvelope(Me)
        Private WithEvents _envelopes As New SortedDictionary(Of Ordinal, XEnvelope)

        '** flags

        Private _isPrepared As Boolean = False

        Private _PreparedOn As Date?

        Private _IsPrechecked As Boolean = False
        Private _PrecheckedOk As Boolean = False
        Private _PrecheckTimestamp As Date?
        Private _isProcessed As Boolean = False
        Private _XChangedOK As Boolean = False
        Private _ProcessedTimestamp As Date?

        Private _contextid As String
        Private _tuppelID As String
        Private _entityID As String
        Private WithEvents _msglog As BusinessObjectMessageLog

        '** events for convert values
        Public Event ConvertRequest2HostValue As EventHandler(Of ConvertRequestEventArgs)
        Public Event ConvertRequest2DBValue As EventHandler(Of ConvertRequestEventArgs)

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="xchangeDefaultConfig"></param>
        ''' <remarks></remarks>
        Public Sub New(xchangeDefaultConfig As XChangeConfiguration)
            _XChangeDefaultConfig = xchangeDefaultConfig

        End Sub


#Region "Properties"

        ''' <summary>
        ''' returns the messagelog associated with this XBag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property MessageLog As BusinessObjectMessageLog
            Get
                If _msglog Is Nothing Then _msglog = New BusinessObjectMessageLog(contextidenifier:=Me.ContextIdentifier, tupleidentifier:=Me.TupleIdentifier, entitityidentifier:=Me.EntityIdentifier)
                Return _msglog
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the contextid.
        ''' </summary>
        ''' <value>The contextid.</value>
        Public Property ContextIdentifier() As String
            Get
                Return Me._contextid
            End Get
            Set(value As String)
                Me._contextid = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the TupleIdentifier.
        ''' </summary>
        ''' <value>The contextid.</value>
        Public Property TupleIdentifier() As String
            Get
                Return Me._tuppelID
            End Get
            Set(value As String)
                Me._tuppelID = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the TupleIdentifier.
        ''' </summary>
        ''' <value>The contextid.</value>
        Public Property EntityIdentifier() As String
            Get
                Return Me._entityID
            End Get
            Set(value As String)
                Me._entityID = value
            End Set
        End Property
        ''' <summary>
        ''' Gets the default envelope.
        ''' </summary>
        ''' <value>The default envelope.</value>
        Public ReadOnly Property DefaultEnvelope() As XEnvelope
            Get
                Return Me._defaultEnvelope
            End Get
        End Property

        Public ReadOnly Property IsPrechecked As Boolean
            Get
                Return _IsPrechecked
            End Get
        End Property
        Public ReadOnly Property PrecheckedOk As Boolean
            Get
                Return _PrecheckedOk
            End Get
        End Property
        Public ReadOnly Property PrecheckTimestamp As Date
            Get
                Return _PrecheckTimestamp
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the top CMD.
        ''' </summary>
        ''' <value>The top CMD.</value>
        Public Property XChangeCommand() As otXChangeCommandType
            Get
                Return Me._XCmd
            End Get
            Set(value As otXChangeCommandType)
                Me._XCmd = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the prepared on.
        ''' </summary>
        ''' <value>The prepared on.</value>
        Public Property PreparedOn() As DateTime
            Get
                Return Me._PreparedOn
            End Get
            Private Set(value As DateTime)
                Me._PreparedOn = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the processed on.
        ''' </summary>
        ''' <value>The processed on.</value>
        Public Property ProcessedOn() As DateTime
            Get
                Return Me._ProcessedTimestamp
            End Get
            Private Set(value As DateTime)
                Me._ProcessedTimestamp = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the is prepared.
        ''' </summary>
        ''' <value>The is prepared.</value>
        Public Property IsPrepared() As Boolean
            Get
                Return _isPrepared
            End Get
            Private Set(value As Boolean)
                _isPrepared = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the is processed.
        ''' </summary>
        ''' <value>The is processed.</value>
        Public Property IsProcessed() As Boolean
            Get
                Return Me._isProcessed
            End Get
            Private Set(value As Boolean)
                Me._isProcessed = value
            End Set
        End Property
        ''' <summary>
        ''' returns true if the successfully processed
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ProcessedOK As Boolean
            Get
                Return _XChangedOK
            End Get
        End Property
        ''' <summary>
        ''' Gets the xchangeconfig.
        ''' </summary>
        ''' <value>The xchangeconfig.</value>
        Public ReadOnly Property XChangeDefaultConfig() As XChangeConfiguration
            Get
                Return Me._XChangeDefaultConfig
            End Get
        End Property

#End Region

#Region "Administration functions"

        Public Function Ordinals() As System.Collections.Generic.SortedDictionary(Of Ordinal, XEnvelope).KeyCollection
            Return _envelopes.Keys
        End Function
        '**** check functions if exists
        Public Function ContainsKey(ByVal key As Ordinal) As Boolean
            Return Me.Hasordinal(key)
        End Function
        Public Function ContainsKey(ByVal key As Long) As Boolean
            Return Me.Hasordinal(New Ordinal(key))
        End Function
        Public Function ContainsKey(ByVal key As String) As Boolean
            Return Me.Hasordinal(New Ordinal(key))
        End Function
        Public Function Hasordinal(ByVal ordinal As Ordinal) As Boolean
            Return _envelopes.ContainsKey(ordinal)
        End Function

        '***** remove 
        Public Function RemoveEnvelope(ByVal key As Long) As Boolean
            Me.RemoveEnvelope(New Ordinal(key))
        End Function
        Public Function RemoveEnvelope(ByVal key As String) As Boolean
            Me.RemoveEnvelope(New Ordinal(key))
        End Function
        Public Function RemoveEnvelope(ByVal ordinal As Ordinal) As Boolean
            If Me.Hasordinal(ordinal) Then
                Dim envelope = _envelopes.Item(key:=ordinal)
                '** add handlers
                RemoveHandler envelope.ConvertRequest2HostValue, AddressOf Me.OnRequestConvert2HostValue
                RemoveHandler envelope.ConvertRequestDBValue, AddressOf Me.OnRequestConvert2DBValue
                _envelopes.Remove(ordinal)
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' clear all entries remove all envelopes
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Clear() As Boolean
            _defaultEnvelope.Clear()
            For Each ordinal In _envelopes.Keys
                RemoveEnvelope(ordinal:=ordinal)
            Next
            _envelopes.Clear()
            If _envelopes.Count > 0 Then Return False
            Return True
        End Function
        '***** function to add an Entry
        ''' <summary>
        ''' adds an envelope to the bag by key
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="envelope"></param>
        ''' <param name="removeIfExists"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddEnvelope(ByVal key As Long, Optional ByVal envelope As XEnvelope = Nothing, Optional removeIfExists As Boolean = True) As XEnvelope
            Return Me.AddEnvelope(ordinal:=New Ordinal(key), envelope:=envelope, removeIfExists:=removeIfExists)
        End Function
        ''' <summary>
        ''' adds an envelope to the bag by key
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="envelope"></param>
        ''' <param name="removeIfExists"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddEnvelope(ByVal key As String, Optional ByVal envelope As XEnvelope = Nothing, Optional removeIfExists As Boolean = True) As XEnvelope
            Return Me.AddEnvelope(ordinal:=New Ordinal(key), envelope:=envelope, removeIfExists:=removeIfExists)
        End Function
        ''' <summary>
        ''' adds an envelope to the bag by ordinal
        ''' </summary>
        ''' <param name="ordinal"></param>
        ''' <param name="envelope"></param>
        ''' <param name="removeIfExists"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddEnvelope(ByVal ordinal As Ordinal, Optional ByVal envelope As XEnvelope = Nothing, Optional removeIfExists As Boolean = False) As XEnvelope
            If Me.Hasordinal(ordinal) Then
                'TODO: Differentiate if the value is coming from which object -> donot overwrite with wrong information
                If removeIfExists Then
                    Me.RemoveEnvelope(ordinal)
                Else
                    Return Nothing
                End If
            End If
            If envelope Is Nothing Then
                envelope = New XEnvelope(Me)
            End If
            '** add handlers -> done in new of XEnvelope
            'AddHandler envelope.ConvertRequest2HostValue, AddressOf Me.OnRequestConvert2HostValue
            'AddHandler envelope.ConvertRequestDBValue, AddressOf Me.OnRequestConvert2DBValue
            'add it
            _envelopes.Add(ordinal, value:=envelope)
            envelope.ContextIdentifier = Me.ContextIdentifier
            Return envelope
        End Function

        '***** replace
        ''' <summary>
        ''' replaces or adds an envelope against another with same key
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="envelope"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReplaceEnvelope(ByVal key As Long, ByVal envelope As XEnvelope) As XEnvelope
            Return Me.AddEnvelope(ordinal:=New Ordinal(key), envelope:=envelope, removeIfExists:=True)
        End Function
        ''' <summary>
        ''' replaces or adds an envelope against another with same key
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="envelope"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReplaceEnvelope(ByVal key As String, ByVal envelope As XEnvelope) As XEnvelope
            Return Me.AddEnvelope(ordinal:=New Ordinal(key), envelope:=envelope, removeIfExists:=True)
        End Function
        ''' <summary>
        ''' replaces or adds an envelope against another with same ordinal
        ''' </summary>
        ''' <param name="ordinal"></param>
        ''' <param name="envelope"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReplaceEnvelope(ByVal ordinal As Ordinal, ByVal envelope As XEnvelope) As XEnvelope
            Return Me.AddEnvelope(ordinal:=ordinal, envelope:=envelope, removeIfExists:=True)
        End Function
        ''' <summary>
        ''' returns an Envelope by key
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Item(ByVal key As Object) As XEnvelope
            If TypeOf key Is Ordinal Then
                Dim ordinal As Ordinal = DirectCast(key, Ordinal)
                Return Me.GetEnvelope(ordinal:=ordinal)
            ElseIf IsNumeric(key) Then
                Return Me.GetEnvelope(key:=CLng(key))
            Else
                Return Me.GetEnvelope(key:=key.ToString)
            End If

        End Function
        ''' <summary>
        ''' returns an Envelope by key
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEnvelope(ByVal key As Long) As XEnvelope
            Return Me.GetEnvelope(ordinal:=New Ordinal(key))
        End Function
        ''' <summary>
        '''  returns an Envelope by key
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEnvelope(ByVal key As String) As XEnvelope
            Return Me.GetEnvelope(ordinal:=New Ordinal(key))
        End Function
        ''' <summary>
        '''  returns an Envelope by ordinal
        ''' </summary>
        ''' <param name="ordinal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEnvelope(ByVal ordinal As Ordinal) As XEnvelope
            If _envelopes.ContainsKey(key:=ordinal) Then
                Return _envelopes.Item(key:=ordinal)
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' gets an enumarator over the envelopes
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEnumerator() As IEnumerator(Of XEnvelope) Implements IEnumerable(Of XEnvelope).GetEnumerator
            _envelopes.ToList.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            _envelopes.ToList.GetEnumerator()
        End Function
#End Region

        ''' <summary>
        ''' Event handler for the Slots OnRequestConvert2Hostvalue
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnRequestConvert2HostValue(ByVal sender As Object, ByVal e As ConvertRequestEventArgs) Handles _defaultEnvelope.ConvertRequest2HostValue
            RaiseEvent ConvertRequest2HostValue(sender, e) ' cascade
        End Sub
        ''' <summary>
        ''' EventHandler for the Slots OnRequestConvert2DBValue
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnRequestConvert2DBValue(ByVal sender As Object, ByVal e As ConvertRequestEventArgs) Handles _defaultEnvelope.ConvertRequestDBValue
            RaiseEvent ConvertRequest2DBValue(sender, e) ' cascade
        End Sub
        ''' <summary>
        ''' Prepares the XBag for the Operations to run on it
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Prepare(Optional force As Boolean = False) As Boolean
            If Me.IsPrepared And Not force Then
                Return True
            End If

            If _XCmd = 0 Then
                _XCmd = _XChangeDefaultConfig.GetHighestXCmd()
            End If


            _isPrepared = True
            _PreparedOn = Date.Now
            Return True
        End Function


        ''' <summary>
        ''' Runs the XChange PreCheck
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RunPreXCheck(Optional msglog As BusinessObjectMessageLog = Nothing, _
                                     Optional ByRef workerthread As ComponentModel.BackgroundWorker = Nothing) As Boolean

            RunPreXCheck = True

            ' Exchange all Envelopes
            For Each anEnvelope In _envelopes.Values
                RunPreXCheck = RunPreXCheck And anEnvelope.RunXPreCheck(msglog:=msglog)
            Next

            _IsPrechecked = True
            _PrecheckedOk = RunPreXCheck
            _PrecheckTimestamp = Date.Now

            Return RunPreXCheck
        End Function
        ''' <summary>
        ''' Runs the XChange
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RunXChange(Optional msglog As BusinessObjectMessageLog = Nothing, _
                                   Optional ByRef workerthread As ComponentModel.BackgroundWorker = Nothing) As Boolean

            RunXChange = True

            ' Exchange all Envelopes
            For Each anEnvelope In _envelopes.Values
                RunXChange = RunXChange And anEnvelope.RunXChange(msglog:=msglog)
            Next

            _XChangedOK = RunXChange
            _isProcessed = True
            _ProcessedTimestamp = Date.Now
            Return RunXChange
        End Function
    End Class

    ''' <summary>
    ''' a XSlot represents a Slot in an XEnvelope
    ''' </summary>
    ''' <remarks>
    ''' Design principles:
    ''' 
    ''' the xslot is a non-persistable data container of an envelope to which it belongs. Basically the xslot is also assigned to one or many otdb
    ''' object entries but is ordered by the ordering ordinal (which is a column or something the like).
    ''' 
    ''' an xslot holds the data in the host value presentation which is the value presentation of the exchanging sub system.
    ''' the data will be transformed to the otdb datatype by the property dbvalue which is used to store the data value in OnTrack
    ''' 
    ''' setting a hostvalue or a dbvalue should also be combined with setting the special values
    ''' isEmpty which means that the slot has not value at all and is by intention an empty slot which cannot be used to store data
    ''' IsNull which means the slot has the NOTHING  or NULL value by intention and can be used to set also the database value to null
    ''' 
    ''' for transformation of data values especially in the case of special values of the exchanging sub system
    ''' the events 
    ''' 
    ''' ConvertRequest2HostValue
    ''' ConvertRequest2DBValue 
    ''' 
    ''' can be used. These are propagated to the XBAG level
    ''' 
    ''' the Event OnSlotValueChanged can be used to be informed if the slot value changes
    ''' 
    ''' </remarks>

    Public Class XSlot

        ''' <summary>
        '''  Event Argument Class
        ''' </summary>
        ''' <remarks></remarks>
        Public Class EventArgs
            Inherits System.EventArgs

            Private _slot As XSlot

            Public Sub New(slot As XSlot)
                _slot = slot
            End Sub

            ''' <summary>
            ''' Gets the xchang config slot.
            ''' </summary>
            ''' <value>The slot.</value>
            Public ReadOnly Property XSlot() As XSlot
                Get
                    Return Me._slot
                End Get
            End Property

        End Class


        Private _envelope As XEnvelope
        Private _xentry As XChangeObjectEntry
        Private _explicitDatatype As otDataType

        Private _ordinal As Ordinal

        Private _hostvalue As Object = Nothing
        Private _isEmpty As Boolean = False
        Private _isNull As Boolean = False
        Private _isPrechecked As Boolean = False
        Private _isPrecheckedOk As Boolean = False


        Private _msglog As BusinessObjectMessageLog
        Private _contextid As String
        Private _tuppelID As String
        Private _entityID As String


        '** events for convert values
        Public Event ConvertRequest2HostValue As EventHandler(Of ConvertRequestEventArgs)
        Public Event ConvertRequest2DBValue As EventHandler(Of ConvertRequestEventArgs)

        Public Event OnSlotValueChanged As EventHandler(Of XSlot.EventArgs)

        ''' <summary>
        ''' constructor for slot with envelope reference and attribute
        ''' </summary>
        ''' <param name="xenvelope"></param>
        ''' <param name="attribute"></param>
        ''' <remarks></remarks>
        Public Sub New(xenvelope As XEnvelope, entry As XChangeObjectEntry)
            _envelope = xenvelope
            _xentry = entry
            _ordinal = entry.Ordinal
            _hostvalue = Nothing
            _isEmpty = True
            _isNull = True
            _explicitDatatype = 0 'read from attribute
            AddHandler Me.ConvertRequest2HostValue, AddressOf xenvelope.OnRequestConvert2HostValue
            AddHandler Me.ConvertRequest2DBValue, AddressOf xenvelope.OnRequestConvert2DBValue
            AddHandler Me.OnSlotValueChanged, AddressOf xenvelope.XEnvelope_OnSlotValueChanged
        End Sub
        ''' <summary>
        ''' constructor for slot with envelope reference and attribute and hostvalue
        ''' </summary>
        ''' <param name="xenvelope"></param>
        ''' <param name="attribute"></param>
        ''' <remarks></remarks>
        Public Sub New(xenvelope As XEnvelope, entry As XChangeObjectEntry, hostvalue As Object, Optional isEmpty As Boolean = False, Optional isNull As Boolean = False)
            _envelope = xenvelope
            _xentry = entry
            _ordinal = entry.Ordinal
            _hostvalue = hostvalue
            _isEmpty = isEmpty
            _isNull = isNull
            _explicitDatatype = 0 'read from attribute
            AddHandler Me.ConvertRequest2HostValue, AddressOf xenvelope.OnRequestConvert2HostValue
            AddHandler Me.ConvertRequest2DBValue, AddressOf xenvelope.OnRequestConvert2DBValue
        End Sub

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the contextid.
        ''' </summary>
        ''' <value>The contextid.</value>
        Public Property ContextIdentifier() As String
            Get
                Return Me._contextid
            End Get
            Set(value As String)
                Me._contextid = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the TupleIdentifier.
        ''' </summary>
        ''' <value>The contextid.</value>
        Public Property TupleIdentifier() As String
            Get
                Return Me._tuppelID
            End Get
            Set(value As String)
                Me._tuppelID = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the TupleIdentifier.
        ''' </summary>
        ''' <value>The contextid.</value>
        Public Property EntityIdentifier() As String
            Get
                Return Me._entityID
            End Get
            Set(value As String)
                Me._entityID = value
            End Set
        End Property
        ''' <summary>
        ''' gets the pre checked result - only valid if ISPrechecked is true
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsPrecheckedOk As Boolean
            Get
                Return _isPrecheckedOk
            End Get
            Private Set(ByVal value As Boolean)
                _isPrecheckedOk = value
            End Set
        End Property
        ''' <summary>
        ''' returns True if Slot is supposed to be XChanged
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsXChanged As Boolean
            Get
                If _xentry IsNot Nothing Then
                    Return Not Me.IsEmpty And Me.XChangeEntry.IsXChanged And Not Me.XChangeEntry.IsReadOnly
                Else
                    Return Not Me.IsEmpty
                End If
            End Get
        End Property
        ''' <summary>
        ''' gets the IsPrechecked flag if pre check has Run
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsPrechecked As Boolean
            Private Set(value As Boolean)
                _isPrechecked = value
            End Set
            Get
                Return _isPrechecked
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the ordinal.
        ''' </summary>
        ''' <value>The ordinal.</value>
        Public Property Ordinal() As Ordinal
            Get
                Return Me._ordinal
            End Get
            Private Set(value As Ordinal)
                Me._ordinal = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the is null.
        ''' </summary>
        ''' <value>The is null.</value>
        Public Property IsNull() As Boolean
            Get
                Return Me._isNull Or IsDBNull(_hostvalue)
            End Get
            Set(value As Boolean)
                Me._isNull = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the is empty.
        ''' </summary>
        ''' <value>The is empty.</value>
        Public Property IsEmpty() As Boolean
            Get
                Return Me._isEmpty
            End Get
            Set(value As Boolean)
                Me._isEmpty = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the host value.
        ''' </summary>
        ''' <value>The value.</value>
        Public Property HostValue() As Object
            Get
                Return Me._hostvalue
            End Get
            Set(value As Object)
                If (value Is Nothing AndAlso _hostvalue IsNot Nothing) OrElse (value IsNot Nothing AndAlso _hostvalue Is Nothing) _
                    OrElse (value IsNot Nothing AndAlso _hostvalue IsNot Nothing AndAlso Not _hostvalue.Equals(value)) Then
                    Me._hostvalue = value
                    ' Me.IsEmpty = False ' HACK ! should raise event -> not working since assigning nothing is !
                    If value IsNot Nothing Then
                        Me.IsNull = False
                        Me.IsEmpty = False
                    End If

                    RaiseEvent OnSlotValueChanged(Me, New XSlot.EventArgs(Me))
                End If
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the datatype of the slot -cannot be set if this is bound to a column entry
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Datatype As otDataType
            Get
                If _xentry IsNot Nothing And _explicitDatatype = 0 Then
                    Return _xentry.[ObjectEntryDefinition].Datatype
                ElseIf _explicitDatatype <> 0 Then
                    Return _explicitDatatype
                Else
                    CoreMessageHandler(message:="Attribute or Datatype not set in slot", messagetype:=otCoreMessageType.InternalError, procedure:="XSlot.Datatype")
                    Return 0
                End If
            End Get
            Set(value As otDataType)
                If _xentry Is Nothing Then
                    _explicitDatatype = value
                Else
                    'CoreMessageHandler(message:="explicit datatype cannot be set if attribute was specified", messagetype:=otCoreMessageType.InternalWarning, subname:="XSlot.Datatype")
                    _explicitDatatype = value
                End If
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the Database value.
        ''' </summary>
        ''' <value>The value.</value>
        Public Property DBValue() As Object
            Get
                Dim isNull As Boolean = False
                Dim isEmpty As Boolean = False
                Dim outvalue As Object = _hostvalue
                Dim anArgs As New ConvertRequestEventArgs(Datatype:=Me.Datatype, valuetype:=ConvertRequestEventArgs.convertValueType.Hostvalue,
                                                          value:=_hostvalue, isempty:=Me.IsEmpty, isnull:=Me.IsNull)
                '** raise the event if we have a special eventhandler
                RaiseEvent ConvertRequest2DBValue(sender:=Me, e:=anArgs)
                If anArgs.result Then
                    Me.IsEmpty = anArgs.HostValueisEmpty
                    Me.IsNull = anArgs.HostValueisNull
                    Return anArgs.Dbvalue
                Else
                    If DefaultConvert2DBValue(datatype:=Me.Datatype, dbvalue:=outvalue, hostvalue:=_hostvalue, _
                                                dbValueIsEmpty:=isEmpty, dbValueIsNull:=isNull, hostValueIsEmpty:=_isEmpty, hostValueIsNull:=_isNull, _
                                                msglog:=Me.MessageLog) Then
                        Return outvalue

                    Else
                        ''' TODO: How to comunnicate back that value couldnot be converted ?!
                        Return DBNull.Value
                    End If
                End If

            End Get
            Set(value As Object)
                Dim isNull As Boolean = value Is Nothing
                Dim isEmpty As Boolean = False
                Dim outvalue As Object = Nothing
                Dim anArgs As New ConvertRequestEventArgs(Datatype:=Me.Datatype, valuetype:=ConvertRequestEventArgs.convertValueType.DBValue,
                                                          value:=value, isnull:=isNull, isempty:=isEmpty)

                RaiseEvent ConvertRequest2HostValue(sender:=Me, e:=anArgs)
                '** try to convert by event
                If anArgs.result Then
                    _hostvalue = anArgs.Hostvalue
                    Me.IsEmpty = anArgs.HostValueisEmpty
                    Me.IsNull = anArgs.HostValueisNull
                    RaiseEvent OnSlotValueChanged(Me, New XSlot.EventArgs(Me))
                Else
                    '** convert by function
                    If DefaultConvert2HostValue(datatype:=Me.Datatype, dbvalue:=value, hostvalue:=outvalue, _
                                                dbValueIsEmpty:=Me.IsEmpty, dbValueIsNull:=Me.IsNull, hostValueIsEmpty:=isEmpty, hostValueIsNull:=isNull, _
                                                msglog:=Me.MessageLog) Then
                        _hostvalue = outvalue
                        RaiseEvent OnSlotValueChanged(Me, New XSlot.EventArgs(Me))
                    End If
                End If

            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the XChange Entry.
        ''' </summary>
        ''' <value>The XchangeObjectEntry.</value>
        Public Property XChangeEntry() As XChangeObjectEntry
            Get
                Return Me._xentry
            End Get
            Set(value As XChangeObjectEntry)
                Me._xentry = value
            End Set
        End Property
        ''' <summary>
        ''' returns the messagelog associated with this xEnvelope
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property MessageLog As BusinessObjectMessageLog
            Get
                If _msglog Is Nothing Then _msglog = New BusinessObjectMessageLog(contextidenifier:=Me.ContextIdentifier, tupleidentifier:=Me.TupleIdentifier, entitityidentifier:=Me.EntityIdentifier)
                Return _msglog
            End Get
        End Property
#End Region

        ''' <summary>
        ''' convert a value according an objectentry from dbvalue to hostvalue
        ''' </summary>
        ''' <param name="slot"></param>
        ''' <param name="dbvalue"></param>
        ''' <param name="hostvalue"></param>
        ''' <param name="msglog"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Shared Function DefaultConvert2HostValue(ByVal datatype As otDataType,
                                                 ByRef hostvalue As Object, ByVal dbvalue As Object,
                                                Optional ByRef hostValueIsNull As Boolean = False, Optional ByRef hostValueIsEmpty As Boolean = False,
                                                Optional ByVal dbValueIsNull As Boolean = False, Optional ByVal dbValueIsEmpty As Boolean = False,
                                                Optional ByRef msglog As BusinessObjectMessageLog = Nothing) As Boolean


            '*** transfer
            '****

            hostValueIsEmpty = False
            hostValueIsNull = False

            Select Case datatype
                Case otDataType.[Long]
                    If dbValueIsNull Then
                        hostvalue = CLng(0) ' HACK ! Should be Default Null Value
                        hostValueIsNull = True
                        Return True
                    ElseIf dbValueIsEmpty Then
                        hostvalue = Nothing
                        hostValueIsEmpty = True
                        Return True
                    ElseIf IsNumeric(dbvalue) Then
                        hostvalue = CLng(dbvalue)
                        Return True
                    Else
                        Call CoreMessageHandler(procedure:="DefaultConvert2HostValue.convertValue2Hostvalue",
                                              message:="OTDB data '" & dbvalue & "' is not convertible to long",
                                              argument:=dbvalue)
                        hostValueIsEmpty = True
                        Return False
                    End If
                Case otDataType.Numeric
                    If dbValueIsNull Then
                        hostvalue = CDbl(0) ' HACK ! Should be Default Null Value
                        hostValueIsNull = True
                        Return True
                    ElseIf dbValueIsEmpty Then
                        hostvalue = Nothing
                        hostValueIsEmpty = True
                        Return True
                    ElseIf IsNumeric(dbvalue) Then
                        hostvalue = CDbl(dbvalue)
                        Return True
                    Else
                        Call CoreMessageHandler(procedure:="DefaultConvert2HostValue.convertValue2Hostvalue",
                                              message:="OTDB data '" & dbvalue & "' is not convertible to double",
                                              argument:=dbvalue)
                        hostvalue = Nothing
                        hostValueIsEmpty = True
                        Return False
                    End If


                Case otDataType.Text, otDataType.Memo
                    hostvalue = CStr(dbvalue)
                    Return True
                Case otDataType.List
                    hostvalue = Core.DataType.ToString(dbvalue)
                    Return True
                Case otDataType.Runtime
                    Call CoreMessageHandler(procedure:="DefaultConvert2HostValue.convertValue2Hostvalue",
                                            message:="OTDB data '" & dbvalue & "' is not convertible to runtime",
                                            argument:=dbvalue)
                    hostvalue = Nothing
                    hostValueIsEmpty = True
                    Return False

                Case otDataType.Formula
                    Call CoreMessageHandler(procedure:="DefaultConvert2HostValue.convertValue2Hostvalue",
                                            message:="OTDB data '" & dbvalue & "' is not convertible to formula",
                                            argument:=dbvalue)
                    hostvalue = Nothing
                    hostValueIsEmpty = True
                    Return False

                Case otDataType.[Date], otDataType.Time, otDataType.Timestamp
                    If dbValueIsNull OrElse IsDBNull(dbvalue) OrElse dbvalue = constNullDate OrElse dbvalue = ConstNullTime Then
                        'If datatype = otDataType.Time Then
                        '    hostvalue = ConstNullTime ' HACK ! Should be Default Null Value
                        'Else
                        '    hostvalue = constNullDate
                        'End If
                        hostvalue = Nothing
                        hostValueIsNull = True
                        Return True
                    ElseIf dbValueIsEmpty Then
                        hostvalue = Nothing
                        hostValueIsEmpty = True
                        Return True
                    ElseIf IsDate(dbvalue) Then
                        hostvalue = dbvalue
                        Return True
                    Else
                        Call CoreMessageHandler(procedure:="DefaultConvert2HostValue.convertValue2Hostvalue",
                                              message:="OTDB data '" & dbvalue & "' is not convertible to date, time, timestamp",
                                              argument:=dbvalue)
                        hostvalue = Nothing
                        hostValueIsEmpty = True
                        Return False
                    End If

                Case otDataType.Bool
                    hostvalue = dbvalue
                    Return True
                Case otDataType.Binary
                    hostvalue = dbvalue
                    Return True
                Case Else
                    Call CoreMessageHandler(procedure:="XSlot.convert2HostValue",
                                           message:="type has no converter",
                                           argument:=hostvalue)
                    hostvalue = Nothing
                    hostValueIsEmpty = True
                    Return False
            End Select

        End Function



        ''' <summary>
        ''' Default Convert to DBValue without any specials
        ''' </summary>
        ''' <param name="slot"></param>
        ''' <param name="hostvalue"></param>
        ''' <param name="dbvalue"></param>
        ''' <param name="msglog"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DefaultConvert2DBValue(ByVal datatype As otDataType,
                                                ByVal hostvalue As Object, ByRef dbvalue As Object,
                                                Optional hostValueIsNull As Boolean = False, Optional hostValueIsEmpty As Boolean = False,
                                                Optional ByRef dbValueIsNull As Boolean = False, Optional ByRef dbValueIsEmpty As Boolean = False,
                                                Optional ByRef msglog As BusinessObjectMessageLog = Nothing) As Boolean
            ' set msglog
            If msglog Is Nothing Then
                msglog = New BusinessObjectMessageLog
            End If

            '*** transfer
            '****
            ' default
            dbValueIsEmpty = False
            dbValueIsNull = True

            Select Case datatype

                Case otDataType.Numeric, otDataType.[Long]
                    If hostvalue Is Nothing OrElse hostValueIsNull Then
                        dbvalue = Nothing
                        dbValueIsNull = True
                        Return True
                    ElseIf IsNumeric(hostvalue) Then
                        If datatype = otDataType.Numeric Then
                            dbvalue = CDbl(hostvalue)    ' simply keep it
                            Return True
                        Else
                            dbvalue = CLng(hostvalue)
                            Return True
                        End If
                    Else
                        ' ERROR
                        If msglog IsNot Nothing Then
                            '1201;@;VALIDATOR;object entry validation for '%1%.%2% (XID %5%) failed. Value'%4%' couldnot be converted to data type '%3%';Provide a correct value;90;Error;false;|R1|;|XCHANGEENVELOPE|
                            msglog.Add(1201, CurrentSession.CurrentDomainID, Nothing, Nothing, Nothing, Nothing, _
                                       String.Empty, String.Empty, datatype.ToString, hostvalue.ToString, String.Empty)
                        End If
                        CoreMessageHandler(message:="value is not convertible to numeric or long", argument:=hostvalue,
                                           procedure:="Xslot.DefaultConvert2DBValue", messagetype:=otCoreMessageType.ApplicationError)
                        dbvalue = Nothing
                        dbValueIsEmpty = True
                        Return False
                    End If


                Case otDataType.Text, otDataType.List, otDataType.Memo

                    If hostvalue Is Nothing Then
                        dbvalue = Nothing
                        dbValueIsNull = True
                        Return True
                    ElseIf True Then
                        dbvalue = CStr(hostvalue)
                        Return True
                    Else
                        ' ERROR
                        ' ERROR
                        If msglog IsNot Nothing Then
                            '1201;@;VALIDATOR;object entry validation for '%1%.%2% (XID %5%) failed. Value'%4%' couldnot be converted to data type '%3%';Provide a correct value;90;Error;false;|R1|;|XCHANGEENVELOPE|
                            msglog.Add(1201, CurrentSession.CurrentDomainID, Nothing, Nothing, Nothing, Nothing, _
                                       String.Empty, String.Empty, datatype.ToString, hostvalue.ToString, String.Empty)
                        End If
                        CoreMessageHandler(message:="value is not convertible to string", procedure:="Xslot.DefaultConvert2DBValue",
                                            messagetype:=otCoreMessageType.ApplicationError)
                        dbvalue = Nothing
                        dbValueIsEmpty = True
                        Return False
                    End If

                Case otDataType.Runtime
                    Call CoreMessageHandler(procedure:="XSlot.convert2DBValue",
                                          message:="OTDB data " & hostvalue & " is not convertible from/to runtime",
                                           argument:=hostvalue)

                    dbvalue = DBNull.Value
                    Return False

                Case otDataType.Formula
                    Call CoreMessageHandler(procedure:="XSlot.convert2DBValue", argument:=hostvalue.ToString,
                                          message:="OTDB data " & hostvalue & " is not convertible from/to formula")

                    dbvalue = Nothing
                    dbValueIsEmpty = True
                    Return False

                Case otDataType.[Date], otDataType.Time, otDataType.Timestamp
                    If hostvalue Is Nothing OrElse hostValueIsNull = True Then
                        dbvalue = Nothing
                        dbValueIsNull = True
                        Return True
                    ElseIf IsDate(hostvalue) Then
                        dbvalue = CDate(hostvalue)
                        Return True
                    Else
                        If msglog IsNot Nothing Then
                            '1201;@;VALIDATOR;object entry validation for '%1%.%2% (XID %5%) failed. Value'%4%' couldnot be converted to data type '%3%';Provide a correct value;90;Error;false;|R1|;|XCHANGEENVELOPE|
                            msglog.Add(1201, CurrentSession.CurrentDomainID, Nothing, Nothing, Nothing, Nothing, _
                                       String.Empty, String.Empty, datatype.ToString, hostvalue.ToString, String.Empty)
                        End If
                        Call CoreMessageHandler(procedure:="XSlot.convert2DBValue",
                                              message:="OTDB data '" & hostvalue & "' is not convertible to Date",
                                              argument:=hostvalue)

                        dbvalue = Nothing
                        dbValueIsEmpty = True
                        Return False
                    End If

                Case otDataType.Bool
                    If hostvalue Is Nothing OrElse hostValueIsNull = True Then
                        dbvalue = Nothing
                        dbValueIsNull = True
                        Return True
                    ElseIf TypeOf (hostvalue) Is Boolean Then
                        dbvalue = hostvalue
                        Return True
                    ElseIf IsNumeric(hostvalue) Then
                        If hostvalue = 0 Then
                            dbvalue = False
                        Else
                            dbvalue = True
                        End If
                        Return True
                    ElseIf hostvalue.ToString.ToUpper = "TRUE" Then
                        dbvalue = True
                        Return True
                    ElseIf hostvalue.ToString.ToUpper = "FALSE" Then
                        dbvalue = False
                        Return True
                    ElseIf String.IsNullOrWhiteSpace(hostvalue.ToString) Then
                        dbvalue = False
                        Return True
                    ElseIf Not String.IsNullOrWhiteSpace(hostvalue.ToString) Then
                        dbvalue = True
                        Return True
                    Else
                        If msglog IsNot Nothing Then
                            ' 1201;@;VALIDATOR;object entry validation for '%1%.%2% (XID %5%) failed. Value'%4%' couldnot be converted to data type '%3%';Provide a correct value;90;Error;false;|R1|;|XCHANGEENVELOPE|
                            msglog.Add(1201, CurrentSession.CurrentDomainID, Nothing, Nothing, Nothing, Nothing, _
                                       String.Empty, String.Empty, datatype.ToString, hostvalue.ToString, String.Empty)
                        End If
                        Call CoreMessageHandler(procedure:="XSlot.convert2DBValue",
                                            message:="OTDB data '" & hostvalue & "' is not convertible to boolean",
                                            argument:=hostvalue)

                        dbvalue = True
                        dbValueIsEmpty = True
                        Return False
                    End If

                Case otDataType.Binary
                    dbvalue = hostvalue
                    Return True
                Case Else
                    Call CoreMessageHandler(procedure:="XSlot.convert2DBValue", message:="type has no converter", messagetype:=otCoreMessageType.InternalError, _
                                            argument:=hostvalue)
                    dbvalue = Nothing
                    dbValueIsEmpty = True
                    Return False
            End Select

        End Function

    End Class

    ''' <summary>
    ''' XChange Envelope is a Member of a Bag and Contains Pairs of ordinal, XSlot
    ''' </summary>
    ''' <remarks>
    ''' Design principles:
    ''' an envelope is an arbitary and non persistable xchange class which is used to put together all related entries of otdb dataobjects in xslots
    ''' the data exchange is done in 2 steps: 
    ''' 
    ''' 1. Prechecking and validating the slots
    ''' 2. Exchanging and persisting the changes
    ''' 
    ''' the envelope constist of xslots which can be adressed by ordinals (of the exchange structure e.g columns), 
    ''' otdb objectnames and entrynames or otdb entry exchange ids or otdb exchange aliases.
    ''' 
    ''' prechecking is necessary to process the elements !
    ''' 
    ''' An envelope has a xcmd whch is a operation to be carried out. the xcmd is assigned to the different otdb dataobjects (xobjects)
    ''' and xslots.
    ''' 
    ''' Events:
    ''' for transformation of data values of the slots especially in the case of special values of the exchanging sub system
    ''' the events 
    ''' 
    ''' ConvertRequest2HostValue
    ''' ConvertRequest2DBValue 
    ''' 
    ''' can be used. These are propagated to the XBAG level
    ''' </remarks>
    Public Class XEnvelope
        Implements IEnumerable(Of XSlot)

        Private _xbag As XBag
        Private _xchangeconfig As XChangeConfiguration

        Private _IsPrechecked As Boolean = False
        Private _PrecheckedOk As Boolean = False
        Private _PrecheckTimestamp As DateTime?

        Private _IsXChanged As Boolean = False
        Private _XChangedOK As Boolean = False
        Private _XChangedTimestamp As DateTime?

        Private _slots As New SortedDictionary(Of Ordinal, XSlot) 'the map
        Private WithEvents _msglog As New BusinessObjectMessageLog

        ''' <summary>
        ''' track messagelog as objectmessagelog to track the events of this additional messagelog and propagate
        ''' </summary>
        ''' <remarks></remarks>
        Private WithEvents _trackmessagelog As BusinessObjectMessageLog
        ''' <summary>
        '''  for the object message log envirorment
        ''' </summary>
        ''' <remarks></remarks>
        Private _contextid As String
        Private _tuppelID As String
        Private _entityID As String

        ''' <summary>
        ''' Validator for Validating Prechecks
        ''' </summary>
        ''' <remarks></remarks>
        Private _validator As New Database.ObjectValidator


        '** events for convert values
        Public Event ConvertRequest2HostValue As EventHandler(Of ConvertRequestEventArgs)
        Public Event ConvertRequestDBValue As EventHandler(Of ConvertRequestEventArgs)

        Public Event OnSlotValueChanged As EventHandler(Of XSlot.EventArgs)

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="xbag"></param>
        ''' <remarks></remarks>
        Public Sub New(xbag As XBag)
            _xbag = xbag
            _xchangeconfig = xbag.XChangeDefaultConfig
            '** add handlers
            AddHandler Me.ConvertRequest2HostValue, AddressOf xbag.OnRequestConvert2HostValue
            AddHandler Me.ConvertRequestDBValue, AddressOf xbag.OnRequestConvert2DBValue
        End Sub

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the contextid.
        ''' </summary>
        ''' <value>The contextid.</value>
        Public Property ContextIdentifier() As String
            Get
                Return Me._contextid
            End Get
            Set(value As String)
                Me._contextid = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the TupleIdentifier.
        ''' </summary>
        ''' <value>The contextid.</value>
        Public Property TupleIdentifier() As String
            Get
                Return Me._tuppelID
            End Get
            Set(value As String)
                Me._tuppelID = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the TupleIdentifier.
        ''' </summary>
        ''' <value>The contextid.</value>
        Public Property EntityIdentifier() As String
            Get
                Return Me._entityID
            End Get
            Set(value As String)
                Me._entityID = value
            End Set
        End Property
        ''' <summary>
        ''' get the prechecked flag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsPrechecked As Boolean
            Get
                Return _IsPrechecked
            End Get
            Private Set(ByVal value As Boolean)
                _IsPrechecked = value
            End Set
        End Property

        ''' <summary>
        ''' gets the timestamp for the precheck
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PrecheckTimestamp As DateTime?
            Get
                Return _PrecheckTimestamp
            End Get
            Private Set(value As DateTime?)
                _PrecheckTimestamp = value
            End Set
        End Property

        ''' <summary>
        ''' returns true if the envelope was xchanged / processed
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsProcessed As Boolean
            Get
                Return _IsXChanged
            End Get
            Set(ByVal value As Boolean)
                _IsXChanged = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the processed date.
        ''' </summary>
        ''' <value>The processed date.</value>
        Public Property ProcessedTimestamp As DateTime?
            Get
                Return Me._XChangedTimestamp
            End Get
            Set(value As DateTime?)
                _XChangedTimestamp = value
            End Set
        End Property

        ''' <summary>
        ''' returns the messagelog associated with this xEnvelope
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property MessageLog As BusinessObjectMessageLog
            Get
                If _msglog Is Nothing Then _msglog = New BusinessObjectMessageLog(contextidenifier:=Me.ContextIdentifier, tupleidentifier:=Me.TupleIdentifier, entitityidentifier:=Me.EntityIdentifier)
                Return _msglog
            End Get
        End Property
        ''' <summary>
        ''' Gets the xchangeconfig.
        ''' </summary>
        ''' <value>The xchangeconfig.</value>
        Public ReadOnly Property Xchangeconfig() As XChangeConfiguration
            Get
                Return Me._xchangeconfig
            End Get
        End Property
#End Region

#Region "Administrative Function"


        Public ReadOnly Property Ordinals() As System.Collections.Generic.SortedDictionary(Of Ordinal, XSlot).KeyCollection
            Get
                Return _slots.Keys
            End Get
        End Property
        ''' <summary>
        ''' sets an interim Messagelog to track 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Property trackMessageLog As BusinessObjectMessageLog
            Get
                Return _trackmessagelog
            End Get
            Set(value As BusinessObjectMessageLog)
                If value IsNot Nothing Then
                    _trackmessagelog = value
                Else
                    _trackmessagelog = Nothing
                End If

            End Set
        End Property

        '**** check functions if exists
        Public Function ContainsOrdinal(ByVal [ordinal] As Ordinal) As Boolean
            Return _slots.ContainsKey(ordinal)
        End Function
        Public Function ContainsOrdinal(ByVal [ordinal] As Long) As Boolean
            Return Me.ContainsOrdinal(New Ordinal([ordinal]))
        End Function
        Public Function ContainsOrdinal(ByVal [ordinal] As String) As Boolean
            Return Me.ContainsOrdinal(New Ordinal([ordinal]))
        End Function
        ''' <summary>
        ''' returns true if in the XConfig a Slot is available for the entryname
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasConfigObjectEntryname(ByVal entryname As String, Optional objectname As String = Nothing) As Boolean
            If _xchangeconfig Is Nothing Then
                Call CoreMessageHandler(message:="XChangeConfig is not set within XMAP", procedure:="XEnvelope.HasConfigObjectEntryname")
                Return False
            End If

            Dim aXChangeMember = _xchangeconfig.GetEntryByObjectEntryName(entryname:=entryname, objectname:=objectname)

            If aXChangeMember Is Nothing Then
                Return False
            Else
                Return True
            End If
        End Function
        ''' <summary>
        ''' returns true if in the XConfig a Slot is available for the XChange ID
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasConfigXID(ByVal xid As String, Optional objectname As String = Nothing) As Boolean

            If _xchangeconfig Is Nothing Then
                Call CoreMessageHandler(message:="XChangeConfig is not set within XMAP", procedure:="XEnvelope.getvaluebyID")
                Return Nothing
            End If

            Dim anEntry = _xchangeconfig.GetEntryByXID(XID:=xid, objectname:=objectname)
            If anEntry Is Nothing Then
                Return False
            Else
                Return True
            End If
        End Function
        ''' <summary>
        ''' remove the slot by ordinal
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RemoveSlot(ByVal key As Long) As Boolean
            Me.RemoveSlot(New Ordinal(key))
        End Function
        ''' <summary>
        ''' remove the slot by ordinal
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RemoveSlot(ByVal key As String) As Boolean
            Me.RemoveSlot(New Ordinal(key))
        End Function
        ''' <summary>
        ''' remove the slot by ordinal
        ''' </summary>
        ''' <param name="ordinal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RemoveSlot(ByVal ordinal As Ordinal) As Boolean
            If Me.ContainsOrdinal(ordinal) Then
                RemoveHandler _slots.Item(ordinal).ConvertRequest2DBValue, AddressOf Me.OnRequestConvert2DBValue
                RemoveHandler _slots.Item(ordinal).ConvertRequest2HostValue, AddressOf Me.OnRequestConvert2HostValue
                RemoveHandler _slots.Item(ordinal).OnSlotValueChanged, AddressOf Me.XEnvelope_OnSlotValueChanged
                _slots.Remove(ordinal)
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' clear the Envelope from all slots
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Clear() As Boolean
            Dim aordinalList = _slots.Keys.ToList
            For Each anordinal In aordinalList
                RemoveSlot(anordinal)
            Next
            _slots.Clear()
            If _slots.Count > 0 Then Return False
            Return True
        End Function
        ''' <summary>
        ''' sets the value of an existing slot given by ordinal
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="value"></param>
        ''' <param name="isHostValue"></param>
        ''' <param name="overwrite"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetSlotValue(ByVal key As Long, ByVal value As Object, _
                                     Optional ByVal isHostValue As Boolean = True, _
                                     Optional overwrite As Boolean = False, _
                                      Optional ValueIsNull As Boolean = False, _
                                            Optional SlotIsEmpty As Boolean = False) As Boolean
            Return Me.SetSlotValue(ordinal:=New Ordinal(key), value:=value, isHostValue:=isHostValue, overwrite:=overwrite, ValueIsNull:=ValueIsNull, SlotIsEmpty:=SlotIsEmpty)
        End Function
        ''' <summary>
        ''' sets the value of an existing slot given by ordinal
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="value"></param>
        ''' <param name="isHostValue"></param>
        ''' <param name="overwrite"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetSlotValue(ByVal key As String, ByVal value As Object, _
                                     Optional ByVal isHostValue As Boolean = True, _
                                     Optional overwrite As Boolean = False, _
                                     Optional valueisNull As Boolean = False, _
                                     Optional SlotIsEmpty As Boolean = False) As Boolean
            Return Me.SetSlotValue(ordinal:=New Ordinal(key), value:=value, isHostValue:=isHostValue, overwrite:=overwrite, ValueIsNull:=valueisNull, SlotIsEmpty:=SlotIsEmpty)
        End Function
        ''' <summary>
        ''' set the value of an existing slot given by ordinal
        ''' </summary>
        ''' <param name="ordinal"></param>
        ''' <param name="value"></param>
        ''' <param name="isHostValue"></param>
        ''' <param name="overwrite"></param>
        ''' <returns>returns true if successfull</returns>
        ''' <remarks></remarks>
        Public Function SetSlotValue(ByVal ordinal As Ordinal, ByVal value As Object,
                                     Optional ByVal isHostValue As Boolean = True,
                                     Optional overwrite As Boolean = False, _
                                      Optional ValueIsNull As Boolean = False, _
                                     Optional SlotIsEmpty As Boolean = False) As Boolean
            ' Add slot if the ordinal is in the config
            ' take the first Attribute which has the ordinal
            If Not Me.ContainsOrdinal(ordinal) Then
                Dim theEntryList = Me.Xchangeconfig.GetEntriesByMappingOrdinal(ordinal:=ordinal)
                Dim anXEntry As XChangeObjectEntry = Nothing
                For Each anEntry In theEntryList
                    If anEntry.IsObjectEntry Then
                        anXEntry = TryCast(anEntry, XChangeObjectEntry)
                        If anXEntry IsNot Nothing Then
                            Exit For
                        End If
                    End If
                Next
                If anXEntry IsNot Nothing Then
                    Me.AddSlot(slot:=New XSlot(xenvelope:=Me, entry:=anXEntry, hostvalue:=Nothing, isEmpty:=True))
                    overwrite = True
                End If
            End If
            ' try again
            If Me.ContainsOrdinal(ordinal) Then
                Dim aSlot = _slots.Item(key:=ordinal)
                '* reset the value if meant to be empty
                If SlotIsEmpty Then
                    value = Nothing
                End If
                If aSlot.IsEmpty Or aSlot.IsNull Or overwrite Then
                    If isHostValue Then
                        aSlot.HostValue = value
                    Else
                        aSlot.DBValue = value
                    End If
                    aSlot.IsEmpty = SlotIsEmpty
                    aSlot.IsNull = ValueIsNull
                End If

            End If

        End Function

        ''' <summary>
        ''' returns a Slot by mapping ordinal
        ''' </summary>
        ''' <param name="ordinal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSlot(ByRef ordinal As Ordinal) As XSlot
            If Me.ContainsOrdinal(ordinal) Then
                'TODO: Differentiate if the value is coming from which object -> donot overwrite with wrong information
                Return _slots.Item(key:=ordinal)
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' returns a List of Slot of a certain ObjectName
        ''' </summary>
        ''' <param name="ordinal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSlotByObject(ByRef objectname As String) As List(Of XSlot)
            Dim aList As New List(Of XSlot)

            If Me.Xchangeconfig Is Nothing Then
                Return aList
            End If
            For Each anAttribute In Me.Xchangeconfig.GetEntriesByObjectName(objectname:=objectname)
                If Me.HasSlotByObjectEntryName(entryname:=anAttribute.ObjectEntryname, objectname:=objectname) Then
                    aList.Add(Me.GetSlot(ordinal:=anAttribute.Ordinal))
                End If
            Next
            Return aList
        End Function
        ''' <summary>
        ''' Add a Slot by ordinal
        ''' </summary>
        ''' <param name="slot"></param>
        ''' <param name="replaceSlotIfExists"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddSlot(ByRef slot As XSlot, Optional replaceSlotIfExists As Boolean = False) As Boolean
            If Me.ContainsOrdinal(slot.Ordinal) Then
                'TODO: Differentiate if the value is coming from which object -> donot overwrite with wrong information
                If replaceSlotIfExists Then
                    Me.RemoveSlot(slot.Ordinal)
                Else
                    Return False
                End If
            End If

            'add our EventHandler for ConvertRequests -> done in new of Slot
            'AddHandler slot.ConvertRequest2HostValue, AddressOf Me.OnRequestConvert2HostValue
            'AddHandler slot.ConvertRequest2DBValue, AddressOf Me.OnRequestConvert2DBValue
            AddHandler slot.MessageLog.OnObjectMessageAdded, AddressOf Me.XEnvelope_OnXSlotMessage

            ' add the slot
            _slots.Add(slot.Ordinal, value:=slot)
            Return True
        End Function
        '*****
        ''' <summary>
        ''' set a slot by ID Reference. get the ordinal from the id and set the value
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="value"></param>
        ''' <param name="isHostValue"></param>
        ''' <param name="objectname"></param>
        ''' <param name="replaceSlotIfExists"></param>
        '''  <param name="extendXConfig"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddSlotByXID(ByVal xid As String, ByVal value As Object,
                                    Optional ByVal isHostValue As Boolean = True,
                                    Optional objectname As String = Nothing,
                                    Optional replaceSlotIfExists As Boolean = False,
                                    Optional extendXConfig As Boolean = False, _
                                    Optional ValueIsNull As Boolean = False, _
                                            Optional SlotIsEmpty As Boolean = False, _
                                             Optional isXchanged As Boolean = True, _
                                            Optional isReadonly As Boolean = False, _
                                            Optional xcmd As otXChangeCommandType = Nothing) As Boolean

            If _xchangeconfig Is Nothing Then
                Call CoreMessageHandler(message:="XChangeConfig is not set within XMAP", procedure:="XEnvelope.AddByID")
                Return False
            End If

            Dim anEntry = _xchangeconfig.GetEntryByXID(XID:=xid, objectname:=objectname)
            If anEntry IsNot Nothing Then
                Return Me.AddSlotbyXEntry(entry:=anEntry, value:=value, isHostValue:=isHostValue, SlotIsEmpty:=SlotIsEmpty, ValueIsNull:=ValueIsNull, _
                                          replaceSlotIfexists:=replaceSlotIfExists)
            ElseIf extendXConfig Then
                _xchangeconfig.AddEntryByXID(Xid:=xid, objectname:=objectname, [readonly]:=isReadonly, isXChanged:=isXchanged, xcmd:=xcmd)
                anEntry = _xchangeconfig.GetEntryByXID(XID:=xid, objectname:=objectname)

                If anEntry IsNot Nothing Then
                    Return Me.AddSlotbyXEntry(entry:=anEntry, value:=value, isHostValue:=isHostValue, SlotIsEmpty:=SlotIsEmpty, ValueIsNull:=ValueIsNull, _
                                          replaceSlotIfexists:=replaceSlotIfExists)
                End If
            End If

            Return False

        End Function
        ''' <summary>
        ''' Add a Slot by entryname
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="value"></param>
        ''' <param name="isHostValue"></param>
        ''' <param name="objectname"></param>
        ''' <param name="overwriteValue"></param>
        ''' <param name="extendXConfig"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddSlotByObjectEntryName(ByVal entryname As String, ByVal value As Object,
                                           Optional ByVal isHostValue As Boolean = True,
                                            Optional objectname As String = Nothing,
                                            Optional overwriteValue As Boolean = False,
                                            Optional extendXConfig As Boolean = False, _
                                            Optional ValueIsNull As Boolean = False, _
                                            Optional SlotIsEmpty As Boolean = False, _
                                            Optional isXchanged As Boolean = True, _
                                            Optional isReadonly As Boolean = False, _
                                            Optional xcmd As otXChangeCommandType = Nothing) As Boolean

            If _xchangeconfig Is Nothing Then
                Call CoreMessageHandler(message:="XChangeConfig is not set within XMAP", procedure:="XEnvelope.AddByFieldname")
                Return False
            End If

            Dim anEntry = _xchangeconfig.GetEntryByObjectEntryName(entryname:=entryname, objectname:=objectname)
            If anEntry Is Nothing And extendXConfig Then
                _xchangeconfig.AddEntryByObjectEntry(entryname:=entryname, objectname:=objectname, isXChanged:=isXchanged, [readonly]:=isReadonly, _
                                                   xcmd:=xcmd)
                anEntry = _xchangeconfig.GetEntryByObjectEntryName(entryname:=entryname, objectname:=objectname)
            End If

            If anEntry IsNot Nothing Then
                Return Me.AddSlotbyXEntry(entry:=anEntry, value:=value, isHostValue:=isHostValue, overwriteValue:=overwriteValue, _
                                             ValueIsNull:=ValueIsNull, SlotIsEmpty:=SlotIsEmpty)
            Else
                Return False
            End If

        End Function
        ''' <summary>
        ''' Add a slot by a configMember definition
        ''' </summary>
        ''' <param name="configmember"></param>
        ''' <param name="value"></param>
        ''' <param name="isHostValue"></param>
        ''' <param name="objectname"></param>
        ''' <param name="overwriteValue"></param>
        ''' <param name="removeSlotIfExists"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddSlotbyXEntry(ByRef entry As IXChangeConfigEntry, ByVal value As Object,
                                        Optional ByVal isHostValue As Boolean = True,
                                        Optional objectname As String = Nothing,
                                        Optional overwriteValue As Boolean = False,
                                        Optional replaceSlotIfexists As Boolean = False, _
                                        Optional ValueIsNull As Boolean = False, _
                                        Optional SlotIsEmpty As Boolean = False) As Boolean

            If _xchangeconfig Is Nothing Then
                Call CoreMessageHandler(message:="XChangeConfig is not set within XMAP", procedure:="XEnvelope.AddSlotbyXEntry")
                Return False
            End If

            If Not entry Is Nothing AndAlso (entry.IsLoaded Or entry.IsCreated) Then
                If Me.ContainsOrdinal(ordinal:=entry.Ordinal) And Not replaceSlotIfexists Then
                    If overwriteValue Then
                        Dim aSlot As XSlot = _slots.Item(key:=entry.Ordinal)
                        aSlot.IsEmpty = SlotIsEmpty
                        aSlot.IsNull = ValueIsNull
                        '* set value later than isNull to make sure that dbvalue transformation works
                        If isHostValue Then
                            aSlot.HostValue = value
                        Else
                            aSlot.DBValue = value
                        End If
                        Return True
                    End If
                    Return False
                Else
                    Dim aNewSlot As XSlot = New XSlot(Me, entry:=entry)
                    aNewSlot.ContextIdentifier = Me.ContextIdentifier
                    aNewSlot.TupleIdentifier = Me.TupleIdentifier
                    aNewSlot.EntityIdentifier = entry.Ordinal.Value.ToString
                    aNewSlot.IsEmpty = SlotIsEmpty
                    aNewSlot.IsNull = ValueIsNull
                    '* set value later than isNull to make sure that dbvalue transformation works
                    If isHostValue Then
                        aNewSlot.HostValue = value
                    Else
                        aNewSlot.DBValue = value
                    End If
                    Return Me.AddSlot(slot:=aNewSlot, replaceSlotIfExists:=replaceSlotIfexists)
                End If
            End If
        End Function
        ''' <summary>
        ''' returns the Slot's value by ID
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="objectname"></param>
        ''' <param name="asHostValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSlotValueByXID(ByVal xid As String, Optional objectname As String = Nothing, Optional asHostValue As Boolean = True) As Object

            If _xchangeconfig Is Nothing Then
                Call CoreMessageHandler(message:="XChangeConfig is not set within XMAP", procedure:="XEnvelope.GetSlotValueByXID")
                Return Nothing
            End If

            Dim aXChangeMember = _xchangeconfig.GetEntryByXID(XID:=xid, objectname:=objectname)
            If aXChangeMember IsNot Nothing Then
                Return Me.GetSlotValueByXEntry(aXChangeMember)
            Else
                CoreMessageHandler(message:="XChangeConfig '" & Me.Xchangeconfig.Configname & "' does not include the id", argument:=xid, messagetype:=otCoreMessageType.ApplicationWarning, procedure:="XEnvelope.GetSlotValueByID")
                Return Nothing
            End If

        End Function
        ''' <summary>
        ''' return true if there is a slot by ID
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasSlotByXID(ByVal xid As String, Optional objectname As String = Nothing) As Object

            If _xchangeconfig Is Nothing Then
                Call CoreMessageHandler(message:="XChangeConfig is not set within XMAP", procedure:="XEnvelope.HasSlotByXID")
                Return Nothing
            End If

            Dim aXChangeMember = _xchangeconfig.GetEntryByXID(XID:=xid, objectname:=objectname)
            If aXChangeMember IsNot Nothing Then
                Return Me.HasSlotByXEntry(aXChangeMember)
            Else
                Return False
            End If

        End Function
        ''' <summary>
        ''' returns the slot's value by entryname
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="objectname"></param>
        ''' <param name="asHostValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSlotValueByObjectEntryName(ByVal entryname As String, Optional objectname As String = Nothing, Optional asHostValue As Boolean = True) As Object

            If _xchangeconfig Is Nothing Then
                Call CoreMessageHandler(message:="XChangeConfig is not set within XMAP", procedure:="XEnvelope.GetSlotValueByObjectEntryName")
                Return Nothing
            End If

            Dim aXChangeMember As XChangeObjectEntry = _xchangeconfig.GetEntryByObjectEntryName(entryname:=entryname, objectname:=objectname)
            If aXChangeMember IsNot Nothing Then
                Return Me.GetSlotValueByXEntry(aXChangeMember)
            Else
                CoreMessageHandler(message:="xconfiguration '" & Me.Xchangeconfig.Configname & "' does not include entryname", entryname:=entryname, objectname:=objectname, _
                                   messagetype:=otCoreMessageType.ApplicationWarning, procedure:="Xenvelope.GetSlotValueByObjectEntryName")
                Return Nothing
            End If

        End Function
        ''' <summary>
        ''' returns true if there is a slot by entryname
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasSlotByObjectEntryName(ByVal entryname As String, Optional objectname As String = Nothing) As Boolean

            If _xchangeconfig Is Nothing Then
                Call CoreMessageHandler(message:="XChangeConfig is not set within XMAP", procedure:="XEnvelope.HasSlotByObjectEntryName")
                Return Nothing
            End If
            Dim aXChangeMember As XChangeObjectEntry = _xchangeconfig.GetEntryByObjectEntryName(entryname:=entryname, objectname:=objectname)
            If aXChangeMember IsNot Nothing Then
                Return Me.HasSlotByXEntry(aXChangeMember)
            Else
                Return False

            End If

        End Function

        ''' <summary>
        ''' returns the slot's value by attribute
        ''' </summary>
        ''' <param name="xchangemember"></param>
        ''' <param name="asHostValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSlotValueByXEntry(ByRef entry As XChangeObjectEntry, Optional asHostValue As Boolean = True) As Object

            If _xchangeconfig Is Nothing Then
                Call CoreMessageHandler(message:="XChangeConfig is not set within XMAP", procedure:="XEnvelope.GetSlotValueByXEntry")
                Return Nothing
            End If

            If Not entry Is Nothing AndAlso (entry.IsLoaded Or entry.IsCreated) Then
                Return Me.GetSlotValue(ordinal:=New Ordinal(entry.Ordinal), asHostvalue:=asHostValue)
            Else
                Call CoreMessageHandler(message:="entry is nothing", messagetype:=otCoreMessageType.InternalWarning, procedure:="XEnvelope.GetSlotValueByEntry")
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' returns True if there is a slot by XConfig Member by XChangemember
        ''' </summary>
        ''' <param name="xchangemember"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasSlotByXEntry(ByRef objectentry As XChangeObjectEntry) As Boolean

            If _xchangeconfig Is Nothing Then
                Call CoreMessageHandler(message:="XChangeConfig is not set within XMAP", procedure:="XEnvelope.HasSlotByXEntry")
                Return False
            End If

            If objectentry IsNot Nothing AndAlso (objectentry.IsLoaded Or objectentry.IsCreated) Then
                If _slots.ContainsKey(key:=objectentry.Ordinal) Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        End Function
        ''' <summary>
        ''' returns the Attribute of a slot by entryname and objectname
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEntryByObjectEntryname(ByVal entryname As String, Optional objectname As String = Nothing) As XChangeObjectEntry

            If _xchangeconfig Is Nothing Then
                Call CoreMessageHandler(message:="XChangeConfig is not set within XMAP", procedure:="XEnvelope.GetEntryByObjectEntryname")
                Return Nothing
            End If

            Return _xchangeconfig.GetEntryByObjectEntryName(entryname:=entryname, objectname:=objectname)
        End Function
        ''' <summary>
        ''' returns the Entry of a slot by xid and objectname
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEntryByXID(ByVal XID As String, Optional objectname As String = Nothing) As XChangeObjectEntry

            If _xchangeconfig Is Nothing Then
                Call CoreMessageHandler(message:="XChangeConfig is not set within XMAP", procedure:="XEnvelope.GetEntryByXID")
                Return Nothing
            End If

            Return _xchangeconfig.GetEntryByXID(XID:=XID, objectname:=objectname)
        End Function

        ''' <summary>
        ''' returns the Slotsvalue as hostvalue or dbvalue by ordinal key
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="asHostvalue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSlotValue(ByVal key As Long, Optional ByVal asHostValue As Boolean = False) As Object
            Return Me.GetSlotValue(ordinal:=New Ordinal(key), asHostvalue:=asHostValue)
        End Function
        ''' <summary>
        ''' returns the Slotsvalue as hostvalue or dbvalue by ordinal key
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="asHostvalue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSlotValue(ByVal key As String, Optional ByVal asHostValue As Boolean = False) As Object
            Return Me.GetSlotValue(ordinal:=New Ordinal(key), asHostvalue:=asHostValue)
        End Function
        ''' <summary>
        ''' returns the Slotsvalue as hostvalue or dbvalue by ordinal
        ''' </summary>
        ''' <param name="ordinal"></param>
        ''' <param name="asHostvalue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSlotValue(ByVal ordinal As Ordinal, Optional asHostvalue As Boolean = True) As Object
            If _slots.ContainsKey(key:=ordinal) Then
                Dim aSlot = _slots.Item(key:=ordinal)
                If asHostvalue Then
                    Return aSlot.HostValue
                Else
                    Return aSlot.DBValue
                End If
            Else
                Return Nothing
            End If
        End Function
        '*** enumerators -> get values
        Public Function GetEnumerator() As IEnumerator(Of XSlot) Implements IEnumerable(Of XSlot).GetEnumerator
            Return _slots.Values.GetEnumerator
        End Function
        Public Function GetEnumerator1() As Collections.IEnumerator Implements Collections.IEnumerable.GetEnumerator
            Return _slots.Values.GetEnumerator
        End Function
#End Region


        ''' <summary>
        ''' Eventhandler for the Slots OnSlotValueChanged
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub XEnvelope_OnSlotValueChanged(ByVal sender As Object, ByVal e As XSlot.EventArgs)
            RaiseEvent OnSlotValueChanged(Me, e) ' cascade
        End Sub
        ''' <summary>
        ''' Eventhandler for the Slots OnRequestConvert2Hostvalue
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnRequestConvert2HostValue(ByVal sender As Object, ByVal e As ConvertRequestEventArgs)
            RaiseEvent ConvertRequest2HostValue(sender, e) ' cascade
        End Sub
        ''' <summary>
        ''' EventHandler for the Slots OnRequestConvert2DBValue
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnRequestConvert2DBValue(ByVal sender As Object, ByVal e As ConvertRequestEventArgs)
            RaiseEvent ConvertRequestDBValue(sender, e) ' cascade
        End Sub

        ''' <summary>
        ''' returns the Object XCommand
        ''' </summary>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectXCmd(ByVal objectname As String) As otXChangeCommandType
            Dim anObject As XChangeObject = Me.Xchangeconfig.GetObjectByName(objectname:=objectname)
            If anObject IsNot Nothing Then
                Return anObject.XChangeCmd
            Else
                Return 0
            End If
        End Function
        ''' <summary>
        ''' run XChange Precheck on the Envelope
        ''' </summary>
        ''' <param name="aMapping"></param>
        ''' <param name="MSGLOG"></param>
        ''' <param name="SUSPENDOVERLOAD"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RunXPreCheck(Optional msglog As BusinessObjectMessageLog = Nothing,
                                     Optional ByVal suspendoverload As Boolean = True) As Boolean
            Dim result As Boolean = True

            ' set msglog
            If msglog Is Nothing Then msglog = Me.MessageLog


            ' suspend Overloading
            If suspendoverload Then Call SuspendOverloading(True)


            '* go through each object
            For Each anConfigObject As XChangeObject In _xbag.XChangeDefaultConfig.ObjectsByOrderNo

                ' Obsolete interface and way to call (should be by retrieving the object than calling):
                '--------------------------------------------------------------------------------------
                '** check through reflection
                'Dim anObjectType As System.Type = ot.GetObjectClassType(anConfigObject.Objectname)
                '
                'If anObjectType IsNot Nothing AndAlso _
                '    anObjectType.GetInterface(GetType(iotXChangeable).FullName) IsNot Nothing Then
                '    Dim aXChangeable As iotXChangeable = ot.CreateDataObjectInstance(anObjectType)
                '    flag = aXChangeable.RunXPreCheck(Me, msglog)
                'Else
                ' default
                result = result And RunDefaultPreCheck(anConfigObject, msglog)
                'End If

            Next

            ' suspend Overloading
            If suspendoverload Then Call SuspendOverloading(False)

            _PrecheckTimestamp = Date.Now
            Me.IsPrechecked = result
            Me.PrecheckTimestamp = Date.Now
            Return result
        End Function

        ''' <summary>
        ''' run XChange for this Envelope
        ''' </summary>
        ''' <param name="msglog"></param>
        ''' <param name="suspendoverload"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RunXChange(Optional msglog As BusinessObjectMessageLog = Nothing,
                                   Optional ByVal suspendoverload As Boolean = True) As Boolean
            Dim result As Boolean = True

            ' set msglog
            If msglog Is Nothing Then msglog = Me.MessageLog

            ' suspend Overloading
            If suspendoverload Then Call SuspendOverloading(True)

            If Me.ProcessedTimestamp Is Nothing OrElse Me.ProcessedTimestamp = constNullDate Then
                _XChangedTimestamp = Date.Now
            End If

            '* go through each object
            For Each anConfigObject As XChangeObject In Me.Xchangeconfig.ObjectsByOrderNo

                ' OBSOLETE INTERFACE:
                '--------------------
                '** check through reflection
                'Dim anObjectType As System.Type = ot.GetObjectClassType(anConfigObject.Objectname)
                'If anObjectType IsNot Nothing AndAlso _
                '    anObjectType.GetInterface(GetType(iotXChangeable).FullName) IsNot Nothing Then

                '    Dim aXChangeable As iotXChangeable = ot.CreateDataObjectInstance(anObjectType)
                'result = result And aXChangeable.RunXChange(Me)
                'Else
                ' default
                result = result And RunDefaultXChange(anConfigObject, msglog)
                'End If
            Next

            ' suspend Overloading
            If suspendoverload Then Call SuspendOverloading(False)

            Me.ProcessedTimestamp = Date.Now
            Me.IsProcessed = result
            Return result
        End Function


        ''' <summary>
        ''' Run the Default Precheck for an exchange object in this envelope
        ''' </summary>
        ''' <param name="xobject"></param>
        ''' <param name="msglog"></param>
        ''' <param name="nocompounds"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function RunDefaultPreCheck(ByRef xobject As XChangeObject,
                                           Optional ByRef msglog As BusinessObjectMessageLog = Nothing) As Boolean
            Dim pkarry() As Object
            Dim aValue As Object

            ' set msglog
            If msglog Is Nothing Then msglog = Me.MessageLog

            '*** build the primary key array
            If xobject.XObjectDefinition.GetNoKeys = 0 Then
                If msglog IsNot Nothing Then msglog.Add(1009, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, _
                           Me.EntityIdentifier, Me, xobject.Configname, xobject.XObjectDefinition.ID, xobject.XChangeCmd.ToString)
                Return False
            Else
                ReDim pkarry(xobject.XObjectDefinition.GetNoKeys - 1)
            End If

            '**** fill the primary key structure
            Dim i As UShort = 0
            For Each aPKEntry In xobject.XObjectDefinition.GetKeyEntries
                aValue = Me.GetSlotValueByObjectEntryName(entryname:=aPKEntry.Entryname, objectname:=aPKEntry.Objectname, asHostValue:=False)
                If aValue IsNot Nothing Then
                    '** convert from DB to Host
                    pkarry(i) = aValue
                    i += 1
                Else
                    If msglog IsNot Nothing Then msglog.Add(1002, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, _
                          Me.EntityIdentifier, Me, xobject.Configname, xobject.XObjectDefinition.ID, aPKEntry.Entryname)
                    Return False
                End If

            Next

            ''' check if we need a object and how to get it
            ''' then run the command

            Dim anObject As iormRelationalPersistable
            Select Case xobject.XChangeCmd
                Case otXChangeCommandType.CreateUpdate
                    anObject = ormBusinessObject.RetrieveDataObject(pkarry, xobject.XObjectDefinition.ObjectType)
                    If anObject Is Nothing Then
                        ''' no object which could be retrieved - a new one will be created
                        If msglog IsNot Nothing Then msglog.Add(1004, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, _
                          Me.EntityIdentifier, Me, xobject.Configname, xobject.XObjectDefinition.ID, Converter.Array2StringList(pkarry))
                    End If
                Case otXChangeCommandType.Delete, otXChangeCommandType.Duplicate, otXChangeCommandType.Read, otXChangeCommandType.Update
                    '*** read the data
                    '***
                    anObject = ormBusinessObject.RetrieveDataObject(pkarry, xobject.XObjectDefinition.ObjectType)
                    If anObject Is Nothing Then
                        ''' no object which could be retrieved for such a operation
                        If msglog IsNot Nothing Then msglog.Add(1003, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, _
                          Me.EntityIdentifier, Me, xobject.Configname, xobject.XObjectDefinition.ID, Converter.Array2StringList(pkarry))
                    End If
                Case Else
                    ''' no command ?!
                    If msglog IsNot Nothing Then msglog.Add(1005, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, _
                      Me.EntityIdentifier, Me, xobject.Configname, xobject.XObjectDefinition.ID, xobject.XChangeCmd.ToString)
                    CoreMessageHandler(message:="XCMD is not implemented for XConfig " & xobject.Configname, procedure:="Xenvelope.RunDefaultXChange", argument:=xobject.XChangeCmd, messagetype:=otCoreMessageType.InternalError)
                    Return False
            End Select

            '** run it with the object or without

            Return Me.RunDefaultPreCheck(anObject, xobject:=xobject, msglog:=msglog)

        End Function
        ''' <summary>
        ''' Run the default precheck on a given data object, this envelope and xconfiguration
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="dataobject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function RunDefaultPreCheck(ByRef dataobject As iormRelationalPersistable, xobject As XChangeObject, _
                                           Optional msglog As BusinessObjectMessageLog = Nothing) As Boolean
            Dim aValue As Object

            '** no object is given
            If dataobject Is Nothing OrElse Not dataobject.IsAlive(throwError:=False) Then
                dataobject = ot.CurrentSession.DataObjectProvider(xobject.XObjectDefinition.ObjectType).NewOrmDataObject(xobject.XObjectDefinition.ObjectType)
            End If

            ' set msglog
            If msglog Is Nothing Then msglog = Me.MessageLog


            '** run the read first -> to fill the envelope anyway what is happening afterwards
            '** 

            For Each anObjectEntry In dataobject.ObjectDefinition.GetEntries()
                '** only if alive (was not created above)
                If dataobject.IsAlive(throwError:=False) AndAlso _
                    Me.HasConfigObjectEntryname(entryname:=anObjectEntry.Entryname, objectname:=xobject.Objectname) Then
                    '* get the value and add it -> will be replaced as well !
                    aValue = dataobject.GetValue(anObjectEntry.Entryname)
                    ' add it to the slot even if it's nothing -> default must be converted through the
                    ' slot
                    ' add the slot but donot extend the XConfig - donot overwrite existing values
                    Me.AddSlotByObjectEntryName(entryname:=anObjectEntry.Entryname, _
                                                objectname:=xobject.Objectname, _
                                                value:=aValue, _
                                                isHostValue:=False,
                                                overwriteValue:=False, _
                                                extendXConfig:=False)

                End If
            Next


            '*** run the commands
            '***
            Select Case xobject.XChangeCmd


                '*** delete
                '***
                Case otXChangeCommandType.Delete
                    If dataobject Is Nothing OrElse Not dataobject.IsAlive(throwError:=False) Then
                        ''' dataobject doesnot exist delete is absolete
                        msglog.Add(1008, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, _
                                   Me.EntityIdentifier, Me, xobject.Configname, xobject.XObjectDefinition.ID, _
                                   Converter.Array2StringList(xobject.XObjectDefinition.Keys), xobject.XChangeCmd.ToString)
                        Return False
                    Else
                        ''' dataobject exists everthing is fine
                        msglog.Add(1090, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, _
                                   Me.EntityIdentifier, Me, xobject.Configname, xobject.XObjectDefinition.ID, xobject.XChangeCmd.ToString)
                        Return True
                    End If
                    '**** add or update
                    '****
                Case otXChangeCommandType.Update, otXChangeCommandType.CreateUpdate
                    '*** evaluate even 
                    '***
                    Dim evalflag As Boolean = True
                    For Each anXEntry In Me.Xchangeconfig.GetEntriesByObjectName(objectname:=xobject.Objectname)

                        ''' check on Update if xchanged and not readonly
                        ''' check on CreateUpdate if not readonly (empty must be provided
                        ''' 
                        If (anXEntry.XChangeCmd = otXChangeCommandType.Update AndAlso anXEntry.IsXChanged AndAlso Not anXEntry.IsReadOnly) _
                            OrElse (anXEntry.XChangeCmd = otXChangeCommandType.CreateUpdate AndAlso Not anXEntry.IsReadOnly) Then

                            Dim aSlot = Me.GetSlot(ordinal:=anXEntry.Ordinal)
                            If aSlot IsNot Nothing Then
                                '* get Value from Slot DBNULL ist the hack-return
                                aValue = aSlot.DBValue
                                Dim outvalue As Object
                                If Not IsDBNull(aValue) AndAlso dataobject.ObjectDefinition.HasEntry(anXEntry.ObjectEntryname) Then



                                    ''' if creating then add the default value if left empty
                                    ''' 
                                    If aSlot.IsEmpty AndAlso anXEntry.XChangeCmd = otXChangeCommandType.CreateUpdate Then
                                        aSlot.IsEmpty = False
                                        If Not anXEntry.ObjectEntryDefinition.IsNullable Then
                                            aValue = TryCast(dataobject, iormInfusable).ObjectEntryDefaultValue(anXEntry.ObjectEntryname)
                                            If aValue Is Nothing Then
                                                aSlot.IsNull = True
                                            Else
                                                aSlot.IsNull = False
                                            End If
                                            aSlot.DBValue = aValue
                                        Else
                                            aSlot.DBValue = Nothing
                                            aValue = Nothing
                                        End If
                                    End If

                                    ''' do not check empty slots normaly - these are by intention free
                                    ''' 
                                    If aSlot.IsEmpty Then
                                        evalflag = evalflag Or True
                                    Else
                                        ''' 
                                        ''' PHASE I : APPLY THE ENTRY PROPERTIES AND TRANSFORM THE VALUE REQUESTED
                                        ''' 
                                        outvalue = aValue ' copy over
                                        If Not TryCast(dataobject, iormInfusable).Normalizevalue(anXEntry.ObjectEntryname, outvalue) Then
                                            CoreMessageHandler(message:="Warning ! Could not normalize value", argument:=outvalue, objectname:=anXEntry.Objectname, _
                                                                entryname:=anXEntry.ObjectEntryname, procedure:="Xenvelope.RunDefaultPrecheck")
                                        End If

                                        ''' Validate the OutValue
                                        ''' 
                                        Dim result As otValidationResultType = _
                                            TryCast(dataobject, iormValidatable).Validate(anXEntry.ObjectEntryname, outvalue, msglog)

                                        If result = otValidationResultType.Succeeded Then
                                            evalflag = evalflag Or True
                                        Else
                                            evalflag = evalflag Or False
                                        End If
                                    End If

                                Else

                                End If
                            End If
                        End If
                    Next
                    If evalflag Then
                        ''' add message if nothing else there
                        If msglog.Count = 0 Then
                            '1091;@;XCHANGE;all object entry evaluations were successfull - xchange command '%3%' can be run for object id '%2%' in xchange configuration '%1%';;10;Info;false;|G1|;|XCHANGEENVELOPE|
                            msglog.Add(1091, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, Me.EntityIdentifier, Me, _
                                       xobject.Configname, xobject.XObjectDefinition.ID, xobject.XChangeCmd.ToString)
                        Else
                            Dim aStatusitem = msglog.GetHighesStatusItem(statustype:=ot.ConstStatusType_XEnvelope)
                            If aStatusitem Is Nothing OrElse (aStatusitem IsNot Nothing AndAlso aStatusitem.Code.ToUpper Like "G%") Then
                                '1091;@;XCHANGE;all object entry evaluations were successfull - xchange command '%3%' can be run for object id '%2%' in xchange configuration '%1%';;10;Info;false;|G1|;|XCHANGEENVELOPE|
                                msglog.Add(1091, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, Me.EntityIdentifier, Me, _
                                           xobject.Configname, xobject.XObjectDefinition.ID, xobject.XChangeCmd.ToString)
                            End If
                        End If

                        Return True
                    Else
                        Return False
                    End If


                    '*** duplicate
                    '***
                Case otXChangeCommandType.Duplicate
                    If dataobject Is Nothing OrElse Not dataobject.IsAlive(throwError:=False) Then
                        ''' dataobject doesnot exist duplicate not possible
                        msglog.Add(1007, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, _
                                   Me.EntityIdentifier, Me, xobject.Configname, xobject.XObjectDefinition.ID, _
                                   Converter.Array2StringList(xobject.XObjectDefinition.Keys), xobject.XChangeCmd.ToString)
                        Return False
                    Else
                        ''' add message if nothing else there
                        If msglog.Count = 0 Then
                            '1091;@;XCHANGE;all object entry evaluations were successfull - xchange command '%3%' can be run for object id '%2%' in xchange configuration '%1%';;10;Info;false;|G1|;|XCHANGEENVELOPE|
                            msglog.Add(1091, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, Me.EntityIdentifier, Me, _
                                       xobject.Configname, xobject.XObjectDefinition.ID, xobject.XChangeCmd.ToString)
                        Else
                            Dim aStatusitem = msglog.GetHighesStatusItem(statustype:=ot.ConstStatusType_XEnvelope)
                            If aStatusitem Is Nothing OrElse (aStatusitem IsNot Nothing AndAlso aStatusitem.Code.ToUpper Like "G%") Then
                                '1091;@;XCHANGE;all object entry evaluations were successfull - xchange command '%3%' can be run for object id '%2%' in xchange configuration '%1%';;10;Info;false;|G1|;|XCHANGEENVELOPE|
                                msglog.Add(1091, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, Me.EntityIdentifier, Me, _
                                           xobject.Configname, xobject.XObjectDefinition.ID, xobject.XChangeCmd.ToString)
                            End If
                        End If

                        Return True
                    End If

                    '***
                    '*** just read and return
                Case otXChangeCommandType.Read
                    If dataobject Is Nothing OrElse Not dataobject.IsAlive(throwError:=False) Then
                        ''' dataobject doesnot exist read not possible
                        msglog.Add(1007, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, _
                                   Me.EntityIdentifier, Me, xobject.Configname, xobject.XObjectDefinition.ID, _
                                   Converter.Array2StringList(xobject.XObjectDefinition.Keys), xobject.XChangeCmd.ToString)
                        Return False
                    Else
                        ''' add message if nothing else there
                        If msglog.Count = 0 Then
                            '1091;@;XCHANGE;all object entry evaluations were successfull - xchange command '%3%' can be run for object id '%2%' in xchange configuration '%1%';;10;Info;false;|G1|;|XCHANGEENVELOPE|
                            msglog.Add(1091, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, Me.EntityIdentifier, Me, _
                                       xobject.Configname, xobject.XObjectDefinition.ID, xobject.XChangeCmd.ToString)
                        Else
                            Dim aStatusitem = msglog.GetHighesStatusItem(statustype:=ot.ConstStatusType_XEnvelope)
                            If aStatusitem Is Nothing OrElse (aStatusitem IsNot Nothing AndAlso aStatusitem.Code.ToUpper Like "G%") Then
                                '1091;@;XCHANGE;all object entry evaluations were successfull - xchange command '%3%' can be run for object id '%2%' in xchange configuration '%1%';;10;Info;false;|G1|;|XCHANGEENVELOPE|
                                msglog.Add(1091, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, Me.EntityIdentifier, Me, _
                                           xobject.Configname, xobject.XObjectDefinition.ID, xobject.XChangeCmd.ToString)
                            End If
                        End If


                        Return True

                    End If

                    '**** no command ?!
                Case Else
                    ''' no command ?!
                    msglog.Add(1005, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, _
                      Me.EntityIdentifier, Me, xobject.Configname, xobject.XObjectDefinition.ID, xobject.XChangeCmd)
                    Call CoreMessageHandler(message:="XChangeCmd for this object is not known :" & xobject.Objectname,
                                      argument:=xobject.XChangeCmd, objectname:=xobject.Objectname, messagetype:=otCoreMessageType.ApplicationError,
                                      procedure:="XEnvelope.runXChangeCMD")
                    Return False
            End Select


        End Function
        ''' <summary>
        ''' Run the default xchange on a given and alive dataobject derived from an xchange object
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="dataobject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function RunDefaultXChange(ByRef dataobject As iormRelationalPersistable,
                                          Optional xobject As XChangeObject = Nothing, _
                                          Optional ByRef msglog As BusinessObjectMessageLog = Nothing) As Boolean
            Dim aValue As Object

            '* get the config
            If xobject Is Nothing Then
                xobject = Me.Xchangeconfig.GetObjectByName(objectname:=dataobject.ObjectID)
            End If

            ' set msglog
            If msglog Is Nothing Then msglog = Me.MessageLog


            '** no object is given
            If dataobject Is Nothing OrElse Not dataobject.IsAlive(throwError:=False) Then
                CoreMessageHandler(message:="dataobject must be alive to xchange it from an Envelope", procedure:="XEnvelope.RunXChangeCMD", _
                                   messagetype:=otCoreMessageType.InternalError, objectname:=dataobject.ObjectID)
                Return False
            End If

            '** run the read first -> to fill the envelope anyway what is happening afterwards
            '** 

            For Each anObjectEntry In dataobject.ObjectDefinition.GetEntries()
                If Me.HasConfigObjectEntryname(entryname:=anObjectEntry.Entryname, objectname:=xobject.Objectname) Then
                    '* get the value and add it -> will be replaced as well !
                    aValue = dataobject.GetValue(anObjectEntry.Entryname)
                    ' add it to the slot even if it's nothing -> default must be converted through the
                    ' slot
                    ' add the slot but donot extend the XConfig - donot overwrite existing values
                    Me.AddSlotByObjectEntryName(entryname:=anObjectEntry.Entryname, _
                                                objectname:=xobject.Objectname, _
                                                value:=aValue, _
                                                isHostValue:=False,
                                                overwriteValue:=False, _
                                                extendXConfig:=False)

                End If
            Next


            '*** run the commands
            '***
            Select Case xobject.XChangeCmd


                '*** delete
                '***
                Case otXChangeCommandType.Delete
                    '** add own handler to catch messages
                    AddHandler DirectCast(dataobject, iormLoggable).BusinessObjectMessageLog.OnObjectMessageAdded, AddressOf XEnvelope_OnObjectMessageAdded
                    Me.trackMessageLog = msglog
                    Dim result As Boolean = dataobject.Delete()
                    '1094;@;XCHANGE;xchange command '%4' run for object of type '%2%' with primary key '%3%';;10;Info;false;|G1|;|XCHANGEENVELOPE|
                    If result Then msglog.Add(1094, Nothing, Nothing, Nothing, Nothing, Me, _
                              Me.Xchangeconfig.Configname, xobject.Objectname, Converter.Array2StringList(dataobject.ObjectPrimaryKeyValues), xobject.XChangeCmd.ToString)
                    RemoveHandler DirectCast(dataobject, iormLoggable).BusinessObjectMessageLog.OnObjectMessageAdded, AddressOf XEnvelope_OnObjectMessageAdded
                    Me.trackMessageLog = Nothing
                    Return result
                    '**** add or update
                    '****
                Case otXChangeCommandType.Update, otXChangeCommandType.CreateUpdate
                    '*** set values of object
                    '***
                    Dim persistflag As Boolean = False
                    For Each anXEntry In Me.Xchangeconfig.GetEntriesByObjectName(objectname:=xobject.Objectname)

                        If anXEntry.IsXChanged AndAlso Not anXEntry.IsReadOnly Then
                            If (anXEntry.XChangeCmd = otXChangeCommandType.Update Or anXEntry.XChangeCmd = otXChangeCommandType.CreateUpdate) Then
                                Dim aSlot = Me.GetSlot(ordinal:=anXEntry.Ordinal)
                                ''' only if not empty
                                If aSlot IsNot Nothing AndAlso Not aSlot.IsEmpty Then
                                    '* get Value from Slot
                                    aValue = aSlot.DBValue
                                    If Not IsDBNull(aValue) AndAlso dataobject.ObjectDefinition.HasEntry(anXEntry.ObjectEntryname) _
                                        AndAlso Not dataobject.ObjectDefinition.GetEntryDefinition(anXEntry.ObjectEntryname).IsReadonly Then
                                        If Not dataobject.EqualsValue(entryname:=anXEntry.ObjectEntryname, value:=aValue) Then
                                            persistflag = persistflag Or dataobject.SetValue(entryname:=anXEntry.ObjectEntryname, value:=aValue)
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Next
                    If persistflag Then
                        '** add own handler to catch messages
                        AddHandler DirectCast(dataobject, iormLoggable).BusinessObjectMessageLog.OnObjectMessageAdded, AddressOf XEnvelope_OnObjectMessageAdded
                        Me.trackMessageLog = msglog
                        '** run persist
                        Dim result As Boolean = dataobject.Persist()
                        If result Then
                            ''' re-read the read-only (well hack .. should be the computed entries
                            For Each anObjectEntry In dataobject.ObjectDefinition.GetEntries()
                                If Me.HasConfigObjectEntryname(entryname:=anObjectEntry.Entryname, objectname:=xobject.Objectname) Then
                                    If anObjectEntry.IsReadonly Then
                                        '* get the value and add it -> will be replaced as well !
                                        aValue = dataobject.GetValue(anObjectEntry.Entryname)
                                        If Not Me.AddSlotByObjectEntryName(entryname:=anObjectEntry.Entryname, _
                                                                    objectname:=xobject.Objectname, _
                                                                    value:=aValue, _
                                                                    isHostValue:=False,
                                                                    overwriteValue:=True, _
                                                                    extendXConfig:=True) Then

                                        End If
                                    End If

                                End If
                            Next
                        End If
                        '1092;@;XCHANGE;object of type '%2%' with primary key '%3%' in xchange configuration '%1%' updated;;10;Info;false;|G1|;|XCHANGEENVELOPE|
                        If result Then msglog.Add(1092, Nothing, Nothing, Nothing, Nothing, Me, _
                                  Me.Xchangeconfig.Configname, xobject.Objectname, Converter.Array2StringList(dataobject.ObjectPrimaryKeyValues))
                        RemoveHandler DirectCast(dataobject, iormLoggable).BusinessObjectMessageLog.OnObjectMessageAdded, AddressOf XEnvelope_OnObjectMessageAdded
                        Me.trackMessageLog = Nothing
                        Return result
                    Else
                        '1093;@;XCHANGE;object of type '%2%' with primary key '%3%' in xchange configuration '%1%' has no changes - not updated;;10;Info;false;|G2|;|XCHANGEENVELOPE|
                        msglog.Add(1093, Nothing, Nothing, Nothing, Nothing, Me, _
                                   Me.Xchangeconfig.Configname, xobject.Objectname, Converter.Array2StringList(dataobject.ObjectPrimaryKeyValues))
                        Return True ' even if not persisted the operation is successfull
                    End If


                    '*** duplicate
                    '***
                Case otXChangeCommandType.Duplicate
                    'dataobject.clone().persist
                    Throw New NotImplementedException
                    '***
                    '*** just read and return
                Case otXChangeCommandType.Read
                    If dataobject IsNot Nothing Then
                        '1094;@;XCHANGE;xchange command '%4' run for object of type '%2%' with primary key '%3%';;10;Info;false;|G1|;|XCHANGEENVELOPE|
                        msglog.Add(1094, Nothing, Nothing, Nothing, Nothing, Me, _
                                  Me.Xchangeconfig.Configname, xobject.Objectname, Converter.Array2StringList(dataobject.ObjectPrimaryKeyValues), xobject.XChangeCmd.ToString)

                        '** just return successfull
                        Return True
                    Else
                        '1007;@;XCHANGE;object of type '%2%' with primary key '%3%' from xchange configuration '%1%'  doesnot exists in the database - operation '%4%' aborted;;90;Error;false;|G2|;|XCHANGEENVELOPE|
                        msglog.Add(1007, Nothing, Nothing, Nothing, Nothing, Me, _
                                  Me.Xchangeconfig.Configname, xobject.Objectname, Converter.Array2StringList(dataobject.ObjectPrimaryKeyValues), xobject.XChangeCmd.ToString)

                        '** just return successfull
                        Return False

                    End If

                    '**** no command ?!
                Case Else
                    Call CoreMessageHandler(message:="XChangeCmd for this object is not known :" & xobject.Objectname,
                                      argument:=xobject.XChangeCmd, objectname:=xobject.Objectname, messagetype:=otCoreMessageType.ApplicationError,
                                      procedure:="XEnvelope.runXChangeCMD")
                    Return False
            End Select


        End Function

        ''' <summary>
        ''' Event Handler for ObjectMessageLogs propagate
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub XEnvelope_OnObjectMessageAdded(sender As Object, e As BusinessObjectMessageLog.EventArgs)

            Dim msglog As BusinessObjectMessageLog

            If Me.trackMessageLog IsNot Nothing Then
                msglog = Me.trackMessageLog
            Else
                msglog = Me.MessageLog
            End If

            '** if concerning ?!
            If e.Message.StatusItems(statustype:=ConstStatusType_XEnvelope).Count > 0 OrElse _
               e.Message.StatusItems(statustype:=ConstStatusType_ObjectValidation).Count > 0 OrElse _
               e.Message.StatusItems(statustype:=ConstStatusType_ObjectEntryValidation).Count > 0 OrElse _
                e.Message.StatusItems(statustype:=ConstStatusType_MQMessage).Count > 0 OrElse _
                  e.Message.StatusItems(statustype:=ConstStatusType_MQF).Count > 0 Then
                '** add it
                msglog.Add(e.Message)
            End If
        End Sub
        ''' <summary>
        ''' Run the Default XChange for an xchange object
        ''' </summary>
        ''' <param name="xobject"></param>
        ''' <param name="msglog"></param>
        ''' <param name="nocompounds"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function RunDefaultXChange(ByRef xobject As XChangeObject,
                                          Optional ByRef msglog As BusinessObjectMessageLog = Nothing) As Boolean
            Dim pkarry() As Object
            Dim aValue As Object
            Dim aDomainID As String


            ' set msglog
            If msglog Is Nothing Then msglog = Me.MessageLog

            '*** build the primary key array
            If xobject.XObjectDefinition.GetNoKeys = 0 Then
                ''' error 1009 no primary keys in object definition
                If msglog IsNot Nothing Then msglog.Add(1009, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, _
                     Me.EntityIdentifier, Me, xobject.Configname, xobject.XObjectDefinition.ID, xobject.XChangeCmd.ToString)

                Call CoreMessageHandler(message:="primary key of table is Nothing in xchange config:" & xobject.Configname,
                                      argument:=xobject.Objectname, messagetype:=otCoreMessageType.InternalError, procedure:="XEnvelope.runDefaultXChange4Object")
                Return False
            Else
                ReDim pkarry(xobject.XObjectDefinition.GetNoKeys - 1)
            End If

            '**** fill the primary key structure
            Dim i As UShort = 0
            For Each aPKEntry In xobject.XObjectDefinition.GetKeyEntries
                aValue = Me.GetSlotValueByObjectEntryName(entryname:=aPKEntry.Entryname, objectname:=aPKEntry.Objectname, asHostValue:=False)
                If aValue IsNot Nothing Then
                    '** convert from DB to Host
                    pkarry(i) = aValue
                    i += 1
                Else
                    ''' error 1002 value of primary key is not there
                    If msglog IsNot Nothing Then msglog.Add(1002, CurrentSession.CurrentDomainID, Me.ContextIdentifier, Me.TupleIdentifier, _
                         Me.EntityIdentifier, Me, xobject.Configname, xobject.XObjectDefinition.ID, aPKEntry.Entryname)

                    Call CoreMessageHandler(message:="value of primary key is not in configuration or envelope :" & xobject.Configname,
                                     argument:=xobject.Objectname, entryname:=aPKEntry.Entryname, messagetype:=otCoreMessageType.ApplicationError,
                                     procedure:="XEnvelope.runDefaultXChange4Object")
                    Return False
                End If

            Next

            '''
            ''' recover the domain id
            '''  
            If xobject.Objectname.ToUpper <> Commons.Domain.ConstObjectID.ToUpper Then
                Dim anXID As String = CurrentSession.Objects.GetObjectDefinition(id:=Commons.Domain.ConstObjectID).GetEntryDefinition(entryname:=Commons.Domain.ConstFNDomainID).XID
                aValue = Me.GetSlotValueByXID(xid:=anXID, asHostValue:=False)
                If Not String.IsNullOrWhiteSpace(aValue) Then
                    If Commons.Domain.Retrieve(id:=aValue) IsNot Nothing Then
                        aDomainID = aValue
                    Else
                        aDomainID = CurrentSession.CurrentDomainID
                    End If
                Else
                    aDomainID = CurrentSession.CurrentDomainID
                End If
            End If

            ''' check if we need a object and how to get it
            ''' then run the command

            Dim anObject As iormRelationalPersistable
            Select Case xobject.XChangeCmd
                Case otXChangeCommandType.CreateUpdate
                    ''' try to create with primary key
                    ''' 
                    anObject = ormBusinessObject.CreateDataObject(New ormDatabaseKey(objectid:=xobject.XObjectDefinition.ID, keyvalues:=pkarry), xobject.XObjectDefinition.ObjectType, domainID:=aDomainID)

                    ''' retrieve the Object 
                    ''' 
                    If anObject Is Nothing Then
                        anObject = ormBusinessObject.RetrieveDataObject(pkarry, xobject.XObjectDefinition.ObjectType)
                    End If
                Case otXChangeCommandType.Delete, otXChangeCommandType.Duplicate, otXChangeCommandType.Read, otXChangeCommandType.Update
                    '*** read the data
                    '***
                    anObject = ormBusinessObject.RetrieveDataObject(pkarry, xobject.XObjectDefinition.ObjectType)
                Case Else
                    CoreMessageHandler(message:="XCMD is not implemented for XConfig " & xobject.Configname, procedure:="Xenvelope.RunDefaultXChange", argument:=xobject.XChangeCmd, messagetype:=otCoreMessageType.InternalError)
                    Return False
            End Select

            '** run it with the object
            If anObject IsNot Nothing Then
                Return Me.RunDefaultXChange(anObject, xobject:=xobject, msglog:=msglog)
            Else
                CoreMessageHandler(message:="OnTrack DataObject could not be retrieved nor created: " & xobject.Objectname, _
                                   procedure:="Xenvelope.RunDefaultXChange", argument:=Converter.Array2StringList(pkarry), messagetype:=otCoreMessageType.InternalError)

                Return False
            End If

        End Function


        ''' <summary>
        ''' Handler for OnObjectMessage Added Event on one of the slots
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub XEnvelope_OnXSlotMessage(sender As Object, e As BusinessObjectMessageLog.EventArgs)
            If sender.GetType Is GetType(XSlot) Then
            End If
            If _msglog IsNot Nothing Then _msglog.Add(e.Message)
        End Sub



    End Class

End Namespace