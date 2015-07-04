REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** ORM DATA STRUCTURE CLASSES
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


Imports System.Collections
Imports System.ComponentModel
Imports System.Collections.Generic

Imports System.Reflection
Imports OnTrack.Core

Namespace OnTrack.Database

    ''' <summary>
    ''' OrdinalType identifies the data type of the ordinal
    ''' </summary>
    ''' <remarks></remarks>

    Public Enum OrdinalType
        longType
        stringType
    End Enum
    ''' <summary>
    ''' ordinal class describes values as ordinal values (ordering)
    ''' </summary>
    ''' <remarks></remarks>

    Public Class Ordinal
        Implements IEqualityComparer(Of Ordinal)
        Implements IConvertible
        Implements IComparable(Of Ordinal)
        Implements IComparer(Of Ordinal)

        Private _ordinalvalue As Object
        Private _ordinalType As OrdinalType

        Public Sub New(ByVal value As Object)
            ' return depending on the type

            If TypeOf value Is Long Or TypeOf value Is Integer Or TypeOf value Is UShort _
            Or TypeOf value Is Short Or TypeOf value Is UInteger Or TypeOf value Is ULong Then
                _ordinalType = OrdinalType.longType
                _ordinalvalue = CLng(value)
            ElseIf IsNumeric(value) Then
                _ordinalType = OrdinalType.longType
                _ordinalvalue = CLng(value)
            ElseIf TypeOf value Is Ordinal Then
                _ordinalType = CType(value, Ordinal).Type
                _ordinalvalue = CType(value, Ordinal).Value

            ElseIf value IsNot Nothing AndAlso value.ToString Then
                _ordinalType = OrdinalType.stringType
                _ordinalvalue = String.Copy(value.ToString)
            Else
                Throw New Exception("value is not casteable to a XMAPordinalType")

            End If

        End Sub
        Public Sub New(ByVal value As Object, ByVal type As OrdinalType)
            _ordinalType = type
            Me.Value = value
        End Sub
        Public Sub New(ByVal type As OrdinalType)
            _ordinalType = type
            _ordinalvalue = Nothing
        End Sub

        Public Function ToString() As String
            Return _ordinalvalue.ToString
        End Function
        ''' <summary>
        ''' Equalses the specified x.
        ''' </summary>
        ''' <param name="x">The x.</param>
        ''' <param name="y">The y.</param>
        ''' <returns></returns>
        Public Function [Equals](x As Ordinal, y As Ordinal) As Boolean Implements IEqualityComparer(Of Ordinal).[Equals]
            Select Case x._ordinalType
                Case OrdinalType.longType
                    Return x.Value.Equals(y.Value)
                Case OrdinalType.stringType
                    If String.Compare(x.Value, y.Value, False) = 0 Then
                        Return True
                    Else
                        Return False
                    End If
            End Select

            Return x.Value = y.Value
        End Function
        ''' <summary>
        ''' Compares two objects and returns a value indicating whether one is less
        ''' than, equal to, or greater than the other.
        ''' </summary>
        ''' <param name="x">The first object to compare.</param>
        ''' <param name="y">The second object to compare.</param>
        ''' <exception cref="T:System.ArgumentException">Neither <paramref name="x" /> nor
        ''' <paramref name="y" /> implements the <see cref="T:System.IComparable" /> interface.-or-
        ''' <paramref name="x" /> and <paramref name="y" /> are of different types and neither
        ''' one can handle comparisons with the other. </exception>
        ''' <returns>
        ''' A signed integer that indicates the relative values of <paramref name="x" />
        ''' and <paramref name="y" />, as shown in the following table.Value Meaning Less
        ''' than zero <paramref name="x" /> is less than <paramref name="y" />. Zero <paramref name="x" />
        ''' equals <paramref name="y" />. Greater than zero <paramref name="x" /> is greater
        ''' than <paramref name="y" />.
        ''' </returns>
        Public Function [Compare](x As Ordinal, y As Ordinal) As Integer Implements IComparer(Of Ordinal).[Compare]

            '** depend on the type
            Select Case x.Type
                Case OrdinalType.longType
                    ' try to compare numeric
                    If IsNumeric(y.Value) Then
                        If Me.Value > CLng(y.Value) Then
                            Return 1
                        ElseIf Me.Value < CLng(y.Value) Then
                            Return -1
                        Else
                            Return 0

                        End If
                    Else
                        Return -1
                    End If
                Case OrdinalType.stringType
                    Return String.Compare(y.Value, y.Value.ToString)

            End Select
        End Function
        ''' <summary>
        ''' Compares to.
        ''' </summary>
        ''' <param name="other">The other.</param>
        ''' <returns></returns>
        Public Function CompareTo(other As Ordinal) As Integer Implements IComparable(Of Ordinal).CompareTo
            Return Compare(Me, other)

        End Function

        ''' <summary>
        ''' Gets the hash code.
        ''' </summary>
        ''' <param name="obj">The obj.</param>
        ''' <returns></returns>
        Public Function GetHashCode(obj As Ordinal) As Integer Implements IEqualityComparer(Of Ordinal).GetHashCode
            Return _ordinalvalue.GetHashCode
        End Function
        ''' <summary>
        ''' Value of the ordinal
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Value As Object
            Get
                Select Case Me.Type
                    Case OrdinalType.longType
                        Return CLng(_ordinalvalue)
                    Case OrdinalType.stringType
                        Return CStr(_ordinalvalue)
                End Select
                Return Nothing
            End Get
            Set(value As Object)
                Select Case Me.Type
                    Case OrdinalType.longType
                        _ordinalvalue = CLng(value)
                    Case OrdinalType.stringType
                        _ordinalvalue = CStr(value)
                End Select

                _ordinalvalue = value
            End Set

        End Property
        ''' <summary>
        ''' Datatype of the ordinal
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Type As OrdinalType
            Get
                Return _ordinalType
            End Get
        End Property
        ''' <summary>
        ''' gets the Typecode of the ordinal
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTypeCode() As TypeCode Implements IConvertible.GetTypeCode
            If _ordinalType = OrdinalType.longType Then
                Return TypeCode.UInt64
            ElseIf _ordinalType = OrdinalType.stringType Then
                Return TypeCode.String
            Else
                Return TypeCode.Object
            End If

        End Function

        Public Function ToBoolean(provider As IFormatProvider) As Boolean Implements IConvertible.ToBoolean
            Return _ordinalvalue <> Nothing
        End Function

        Public Function ToByte(provider As IFormatProvider) As Byte Implements IConvertible.ToByte
            Return Convert.ToByte(_ordinalvalue)
        End Function

        Public Function ToChar(provider As IFormatProvider) As Char Implements IConvertible.ToChar
            Return Convert.ToChar(_ordinalvalue)
        End Function

        Public Function ToDateTime(provider As IFormatProvider) As Date Implements IConvertible.ToDateTime

        End Function

        Public Function ToDecimal(provider As IFormatProvider) As Decimal Implements IConvertible.ToDecimal
            Return Convert.ToDecimal(_ordinalvalue)
        End Function

        Public Function ToDouble(provider As IFormatProvider) As Double Implements IConvertible.ToDouble
            Return Convert.ToDouble(_ordinalvalue)
        End Function

        Public Function ToInt16(provider As IFormatProvider) As Short Implements IConvertible.ToInt16
            Return Convert.ToInt16(_ordinalvalue)
        End Function

        Public Function ToInt32(provider As IFormatProvider) As Integer Implements IConvertible.ToInt32
            Return Convert.ToInt32(_ordinalvalue)
        End Function

        Public Function ToInt64(provider As IFormatProvider) As Long Implements IConvertible.ToInt64
            Return Convert.ToInt64(_ordinalvalue)
        End Function

        Public Function ToSByte(provider As IFormatProvider) As SByte Implements IConvertible.ToSByte
            Return Convert.ToSByte(_ordinalvalue)
        End Function

        Public Function ToSingle(provider As IFormatProvider) As Single Implements IConvertible.ToSingle
            Return Convert.ToSingle(_ordinalvalue)
        End Function

        Public Function ToString(provider As IFormatProvider) As String Implements IConvertible.ToString
            Return Convert.ToString(_ordinalvalue)
        End Function

        Public Function ToType(conversionType As Type, provider As IFormatProvider) As Object Implements IConvertible.ToType
            ' DirectCast(_ordinalvalue, conversionType)
        End Function

        Public Function ToUInt16(provider As IFormatProvider) As UShort Implements IConvertible.ToUInt16
            Return Convert.ToUInt16(_ordinalvalue)
        End Function

        Public Function ToUInt32(provider As IFormatProvider) As UInteger Implements IConvertible.ToUInt32
            Return Convert.ToUInt32(_ordinalvalue)
        End Function

        Public Function ToUInt64(provider As IFormatProvider) As ULong Implements IConvertible.ToUInt64
            Return Convert.ToUInt64(_ordinalvalue)
        End Function

        Public Shared Operator =(x As Ordinal, y As Ordinal) As Boolean
            Return x.Value = y.Value
        End Operator
        Public Shared Operator <(x As Ordinal, y As Ordinal) As Boolean
            Return x.Value < y.Value
        End Operator
        Public Shared Operator >(x As Ordinal, y As Ordinal) As Boolean
            Return x.Value > y.Value
        End Operator
        Public Shared Operator <>(x As Ordinal, y As Ordinal) As Boolean
            Return x.Value <> y.Value
        End Operator
        Public Shared Operator +(x As Ordinal, y As Ordinal) As Boolean
            Return x.Value + y.Value
        End Operator

        Function ToUInt64() As Integer
            If IsNumeric(_ordinalvalue) Then Return CLng(_ordinalvalue)
            Throw New NotImplementedException
        End Function
        ''' <summary>
        ''' compares this to an ordinal
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Equals(value As Ordinal) As Boolean
            Return Me.Compare(Me, value) = 0
        End Function

    End Class

    ''' <summary>
    ''' Enumerator for QueryEnumeration
    ''' </summary>
    ''' <remarks></remarks>

    Public Class ormRelationCollectionEnumerator(Of T As {iormInfusable, iormPersistable})
        Implements IEnumerator(Of T)

        Private _collection As ormRelationCollection(Of T)
        Private _counter As Integer
        Private _keyvalues As IList
        Public Sub New(collection As ormRelationCollection(Of T))
            _collection = collection
            _keyvalues = _collection.Keys
            _counter = -1
        End Sub
        Public ReadOnly Property Current As T Implements IEnumerator(Of T).Current
            Get
                If _counter >= 0 And _counter < _keyvalues.Count Then Return _collection.Item(key:=_keyvalues.Item(_counter))
                ' throw else
                Throw New InvalidOperationException()
            End Get
        End Property

        Public Function MoveNext() As Boolean Implements IEnumerator(Of T).MoveNext
            _counter += 1
            Return (_counter < _keyvalues.Count)
            ' throw else
            Throw New InvalidOperationException()
        End Function

        Public Sub Reset() Implements IEnumerator(Of T).Reset
            _counter = 0
        End Sub

        ''' <summary>
        ''' Gets the current element in the collection.
        ''' </summary>
        ''' <returns>The current element in the collection.</returns>
        ''' <value></value>
        Public ReadOnly Property CurrentE() As Object Implements IEnumerator.Current
            Get
                If _counter >= 0 And _counter < _keyvalues.Count Then Return _collection.Item(key:=_keyvalues.Item(_counter))
                ' throw else
                Throw New InvalidOperationException()
            End Get
        End Property
        ''' <summary>
        ''' Performs application-defined tasks associated with freeing, releasing,
        ''' or resetting unmanaged resources.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            _collection = Nothing
            _keyvalues = Nothing
        End Sub


    End Class

    ''' <summary>
    '''  Interface
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks></remarks>
    Public Interface iormRelationalCollection(Of T)
        Inherits ICollection(Of T)

        Property Item(key As Object) As T
        Property Item(keys As Object()) As T

        Property item(key As DataValueTuple) As T


        Function ContainsKey(keys As Object()) As Boolean
        Function ContainsKey(key As Object) As Boolean

        Function ContainsKey(key As DataValueTuple) As Boolean


        Function GetKeyValues(item As T) As DataValueTuple

        ReadOnly Property KeyNames() As String()
    End Interface

    ''' <summary>
    ''' describes an RelationCollection which can add new iormpersistables by key
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks></remarks>

    Public Class ormRelationNewableCollection(Of T As {New, iormInfusable, iormRelationalPersistable})
        Inherits ormRelationCollection(Of T)

        ''' <summary>
        ''' Event Args 
        ''' </summary>
        ''' <remarks></remarks>
        Public Class EventArgs
            Inherits ormRelationCollection(Of T).EventArgs

            Dim _keys As Object()
            Public Sub New(ByRef dataobject As T)
                MyBase.New(dataobject)
            End Sub
            ''' <summary>
            ''' Gets or sets the keys.
            ''' </summary>
            ''' <value>The keys.</value>
            Public Property Keys() As Object()
                Get
                    Return Me._keys
                End Get
                Set(value As Object())
                    Me._keys = value
                End Set
            End Property

        End Class

        ''' <summary>
        ''' Events
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Event RequestKeys(sender As Object, e As ormRelationCollection(Of T).EventArgs)
        Public Event OnNew(sender As Object, e As ormRelationCollection(Of T).EventArgs)


        ''' <summary>
        ''' constructor with the container object (of iormpersistable) 
        ''' and keyentrynames of T
        ''' </summary>
        ''' <param name="containerobject"></param>
        ''' <param name="keynames"></param>
        ''' <remarks></remarks>
        Public Sub New(container As iormRelationalPersistable, keyentrynames As String())
            MyBase.New(container:=container, keyentrynames:=keyentrynames)
        End Sub

        ''' <summary>
        ''' create a new item already stored in this collection
        ''' </summary>
        ''' <param name="keys"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddCreate(Optional keys As Object() = Nothing, _
                                  Optional domainid As String = Nothing, _
                                  Optional checkUnique As Boolean? = True, _
                                  Optional runtimeOnly As Boolean? = Nothing) As T
            Dim anItem As T = CTypeDynamic(ot.CurrentSession.DataObjectProvider(type:=GetType(T), domainid:=domainid).NewOrmDataObject(GetType(T)), GetType(T))

            '''
            ''' raise event if no keys supplied
            If keys Is Nothing Then
                Dim e As ormRelationNewableCollection(Of T).EventArgs = New ormRelationNewableCollection(Of T).EventArgs(anItem)
                RaiseEvent RequestKeys(Me, e)
                keys = e.Keys

                If keys Is Nothing Then
                    CoreMessageHandler(message:="no keys retrieved by event RequestKey", procedure:="ormRelationNewableCollection.AddCreate", messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If
            End If

            ' set the values in the object
            For i = 0 To _keyentries.Count - 1
                keys(i) = anItem.SetValue(_keyentries(i), keys(i))
            Next i

            Dim args = New ormRelationNewableCollection(Of T).EventArgs(anItem)
            RaiseEvent OnNew(Me, args)
            If args.Cancel Then Return Nothing

            Dim arecord As New ormRecord
            If args.Dataobject.Feed(arecord) Then
                anItem = ormBusinessObject.CreateDataObject(Of T)(arecord, domainID:=domainid, checkUnique:=checkUnique, runtimeOnly:=runtimeOnly)
                If anItem IsNot Nothing Then
                    Me.Add(anItem)
                    Return anItem
                Else
                    anItem = CTypeDynamic(Of T)(ormBusinessObject.RetrieveDataObject(pkArray:=keys, type:=GetType(T), domainID:=domainid, runtimeOnly:=runtimeOnly))
                    If anItem.IsDeleted Then
                        CoreMessageHandler("adding create a deleted dataobject - use undelete instead", dataobject:=anItem, _
                                            procedure:="ormRelationNewableCollection.AddCreate", messagetype:=otCoreMessageType.ApplicationError)
                        anItem = Nothing
                    End If
                End If
            End If
            Return Nothing
        End Function
    End Class

    ''' <summary>
    ''' Implementation of an Relational Collection
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks></remarks>

    Public Class ormRelationCollection(Of T As {iormInfusable, iormPersistable})
        Implements iormRelationalCollection(Of T)

        ''' <summary>
        ''' Event Arguments
        ''' </summary>
        ''' <remarks></remarks>
        Public Class EventArgs
            Inherits CancelEventArgs

            Private _dataobject As T

            Public Sub New(ByRef dataobject As T)
                _dataobject = dataobject
            End Sub


            ''' <summary>
            ''' Gets or sets the dataobject.
            ''' </summary>
            ''' <value>The dataobject.</value>
            Public Property Dataobject() As T
                Get
                    Return Me._dataobject
                End Get
                Set(value As T)
                    Me._dataobject = value
                End Set
            End Property

        End Class

        Private _dictionary As New SortedDictionary(Of DataValueTuple, iormRelationalPersistable)
        Protected WithEvents _container As iormRelationalPersistable

        Protected _keyentries As String()

        Public Event OnAdding(sender As Object, e As ormRelationCollection(Of T).EventArgs)
        Public Event OnAdded(sender As Object, e As ormRelationCollection(Of T).EventArgs)

        Public Event OnRemoving(sender As Object, e As ormRelationCollection(Of T).EventArgs)
        Public Event OnRemoved(sender As Object, e As ormRelationCollection(Of T).EventArgs)

        ''' <summary>
        ''' constructor with the container object (of iormpersistable) 
        ''' and keyentrynames of T
        ''' </summary>
        ''' <param name="containerobject"></param>
        ''' <param name="keynames"></param>
        ''' <remarks></remarks>
        Public Sub New(container As iormRelationalPersistable, keyentrynames As String())
            If container IsNot Nothing Then _container = container
            _keyentries = keyentrynames
        End Sub
        ''' <summary>
        ''' get the size of the collection
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Size As ULong
            Get
                Return _dictionary.LongCount
            End Get
        End Property
        ''' <summary>
        ''' gets the list of keys in the collection
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Keys As IList(Of DataValueTuple)
            Get
                Return _dictionary.Keys.ToList
            End Get

        End Property
        ''' <summary>
        ''' returns the entry names for the keys in the collection
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property KeyNames() As String() Implements iormRelationalCollection(Of T).KeyNames
            Get
                Return _keyentries
            End Get
        End Property

        ''' <summary>
        ''' extract the key values of the item (keyentries)
        ''' </summary>
        ''' <param name="item"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetKeyValues(item As T) As DataValueTuple Implements iormRelationalCollection(Of T).GetKeyValues

            Dim keys As New DataValueTuple(_keyentries.Count)
            For i = 0 To _keyentries.GetUpperBound(0)
                keys.Item(i) = item.GetValue(_keyentries(i))
            Next i
            Return keys
        End Function

        ''' <summary>
        ''' add an item to the collection - notifies container
        ''' </summary>
        ''' <param name="item"></param>
        ''' <remarks></remarks>
        Public Sub Add(item As T) Implements ICollection(Of T).Add
            Dim args = New ormRelationCollection(Of T).EventArgs(item)
            RaiseEvent OnAdding(Me, args)
            If args.Cancel Then Return
            If item Is Nothing Then
                Throw New InvalidOperationException("nothing cannot be added")
            End If
            ''' get the keys
            Dim keys = Me.GetKeyValues(item)

            '' no error if we are already in this collection
            If Not Me.ContainsKey(keys) Then
                ''' add the handler for the delete event
                AddHandler item.OnDeleting, AddressOf IormPersistable_OnDelete
                ''' add to the dictionary
                _dictionary.Add(key:=keys, value:=item)
                ''' raise the event
                RaiseEvent OnAdded(Me, args)
            End If

        End Sub

        ''' <summary>
        ''' handler for the OnDeleting Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub IormPersistable_OnDelete(sender As Object, e As ormDataObjectEventArgs)
            Dim anItem As iormRelationalPersistable = e.DataObject
            Me.Remove(anItem)
        End Sub
        ''' <summary>
        ''' clear the Collection - is not a remove with handler
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Clear() Implements ICollection(Of T).Clear
            _dictionary.Clear()
        End Sub
        ''' <summary>
        ''' returns true if the key is in the collection
        ''' </summary>
        ''' <param name="keys"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ContainsKey(keys As Object()) As Boolean Implements iormRelationalCollection(Of T).ContainsKey
            Dim aKey As New DataValueTuple(keys.GetUpperBound(0) + 1)
            aKey.Values = keys
            Return _dictionary.ContainsKey(key:=aKey)
        End Function
        ''' <summary>
        ''' returns true if the key is in the collection
        ''' </summary>
        ''' <param name="keys"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ContainsKey(keys As DataValueTuple) As Boolean Implements iormRelationalCollection(Of T).ContainsKey
            Return _dictionary.ContainsKey(key:=keys)
        End Function

        ''' <summary>
        ''' returns true if the key is in the collection
        ''' </summary>
        ''' <param name="keys"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ContainsKey(key As Object) As Boolean Implements iormRelationalCollection(Of T).ContainsKey
            If key.GetType.IsArray AndAlso key.GetType.GetArrayRank = 1 Then
                Dim akey As New DataValueTuple(UBound(key) + 1)
                Dim i As UShort = 0
                For Each aValue In key
                    akey.Values(i) = aValue
                    i += 1
                Next

                Return _dictionary.ContainsKey(key:=akey)
            Else
                Dim aKey As New DataValueTuple(1)
                aKey.Values = {key}
                Return _dictionary.ContainsKey(key:=aKey)
            End If


        End Function
        ''' <summary>
        ''' returns true if the item is in the collection. based on same keys
        ''' </summary>
        ''' <param name="item"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Contains(item As T) As Boolean Implements ICollection(Of T).Contains
            Dim keys = Me.GetKeyValues(item)
            Return ContainsKey(keys)
        End Function
        ''' <summary>
        ''' copy out to an array
        ''' </summary>
        ''' <param name="array"></param>
        ''' <param name="arrayIndex"></param>
        ''' <remarks></remarks>
        Public Sub CopyTo(array() As T, arrayIndex As Integer) Implements ICollection(Of T).CopyTo
            Dim anArray = _dictionary.Values.ToArray
            For i = arrayIndex To anArray.GetUpperBound(0)
                array(i) = anArray(i)
            Next

        End Sub
        ''' <summary>
        ''' count the number of items in the collection
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Count As Integer Implements ICollection(Of T).Count
            Get
                Return _dictionary.Count
            End Get
        End Property
        ''' <summary>
        ''' return true if readonly
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of T).IsReadOnly
            Get
                Return False
            End Get
        End Property
        ''' <summary>
        ''' remove an item from the collection - the delete handler of the container will be called 
        ''' which might lead to an delete of the item itself
        ''' </summary>
        ''' <param name="item"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Remove(item As T) As Boolean Implements ICollection(Of T).Remove
            Dim args = New ormRelationCollection(Of T).EventArgs(item)
            RaiseEvent OnRemoving(Me, args)

            Dim keys = Me.GetKeyValues(item)
            Dim result = _dictionary.Remove(key:=keys)

            RaiseEvent OnRemoved(Me, args)
            Return result
        End Function
        ''' <summary>
        ''' gets an item by keys
        ''' </summary>
        ''' <param name="keys"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Item(keys As Object()) As T Implements iormRelationalCollection(Of T).Item
            Get
                Dim aKey As New DataValueTuple(keys.GetUpperBound(0) + 1)
                aKey.Values = keys
                Return Me.Item(aKey)
            End Get
            Set(value As T)
                Dim aKey As New DataValueTuple(keys.GetUpperBound(0) + 1)
                aKey.Values = keys
                Me.Item(aKey) = value
            End Set
        End Property
        ''' <summary>
        ''' gets an item by keys
        ''' </summary>
        ''' <param name="keys"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Item(key As DataValueTuple) As T Implements iormRelationalCollection(Of T).Item
            Get
                If ContainsKey(key) Then Return _dictionary.Item(key:=key)
            End Get
            Set(value As T)
                If Not ContainsKey(key) Then _dictionary.Add(key:=key, value:=value)
            End Set
        End Property

        ''' <summary>
        ''' gets an item by keys
        ''' </summary>
        ''' <param name="keys"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Item(key As Object) As T Implements iormRelationalCollection(Of T).Item
            Get
                ' strange we cannot overload
                If key.GetType.Equals(GetType(DataValueTuple)) Then
                    Return _dictionary.Item(key:=key)

                ElseIf key.GetType.IsArray AndAlso key.GetType.GetArrayRank = 1 Then
                    Dim akey As New DataValueTuple(UBound(key) + 1)
                    Dim i As UShort = 0
                    For Each aValue In key
                        akey.Values(i) = aValue
                        i += 1
                    Next

                    Return CType(_dictionary.Item(key:=akey), T)
                Else
                    Dim aKey As New DataValueTuple(1)
                    aKey.Values = {key}
                    Return CType(_dictionary.Item(key:=aKey), T)
                End If

            End Get
            Set(value As T)
                ' strange we cannot overload
                If key.GetType.Equals(GetType(DataValueTuple)) Then
                    _dictionary.Add(key:=CType(key, DataValueTuple), value:=value)

                ElseIf key.GetType.IsArray And key.GetType.GetArrayRank = 1 Then
                    Dim akey As New DataValueTuple(UBound(key) + 1)
                    Dim i As UShort = 0
                    For Each aValue In key
                        akey.Values(i) = aValue
                        i += 1
                    Next

                    _dictionary.Add(key:=akey, value:=value)
                Else
                    Dim aKey As New DataValueTuple(1)
                    aKey.Values = {key}
                    Me.Item(aKey) = value
                End If

            End Set
        End Property
        ''' <summary>
        ''' returns an enumerator
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Return New ormRelationCollectionEnumerator(Of T)(Me)
        End Function

        Public Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Return New ormRelationCollectionEnumerator(Of T)(Me)
        End Function
    End Class



    ''' <summary>
    ''' class for a Property Store with weighted properties for multiple property sets
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ComplexPropertyStore


        ''' <summary>
        ''' Event Arguments
        ''' </summary>
        ''' <remarks></remarks>
        Public Class EventArgs
            Inherits System.EventArgs

            Private _propertyname As String
            Private _setname As String
            Private _weight As Nullable(Of UShort)
            Private _value As Object

            Sub New(Optional propertyname As String = Nothing, Optional setname As String = Nothing, Optional weight As Nullable(Of UShort) = Nothing, Optional value As Object = Nothing)
                If propertyname IsNot Nothing Then _propertyname = propertyname
                If setname IsNot Nothing Then _setname = setname
                If weight.HasValue Then _weight = weight
                If value IsNot Nothing Then value = _value
            End Sub


            ''' <summary>
            ''' Gets the value.
            ''' </summary>
            ''' <value>The value.</value>
            Public ReadOnly Property Value() As Object
                Get
                    Return Me._value
                End Get
            End Property

            ''' <summary>
            ''' Gets the weight.
            ''' </summary>
            ''' <value>The weight.</value>
            Public ReadOnly Property Weight() As UShort?
                Get
                    Return Me._weight
                End Get
            End Property

            ''' <summary>
            ''' Gets the setname.
            ''' </summary>
            ''' <value>The setname.</value>
            Public ReadOnly Property Setname() As String
                Get
                    Return Me._setname
                End Get
            End Property

            ''' <summary>
            ''' Gets the propertyname.
            ''' </summary>
            ''' <value>The propertyname.</value>
            Public ReadOnly Property Propertyname() As String
                Get
                    Return Me._propertyname
                End Get
            End Property

        End Class

        ''' <summary>
        '''  Sequenze of sets
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum Sequence
            Primary = 0
            Secondary = 1
        End Enum

        ''' <summary>
        ''' main data structure a set by name consists of different properties with weights for the values
        ''' </summary>
        ''' <remarks></remarks>
        Private _sets As New Dictionary(Of String, Dictionary(Of String, SortedList(Of UShort, Object)))

        Private _currentset As String
        Private _defaultset As String = String.Empty

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="defaultsetname"></param>
        ''' <remarks></remarks>
        Sub New(defaultsetname As String)
            _defaultset = defaultsetname
        End Sub
        ''' <summary>
        ''' Gets or sets the currentset.
        ''' </summary>
        ''' <value>The currentset.</value>
        Public Property CurrentSet() As String
            Get
                Return Me._currentset
            End Get
            Set(value As String)
                If Me.HasSet(value) Then
                    Me._currentset = value
                    RaiseEvent OnCurrentSetChanged(Me, New ComplexPropertyStore.EventArgs(setname:=value))
                Else
                    Throw New IndexOutOfRangeException(message:="set name '" & value & "' does not exist in the store")
                End If

            End Set
        End Property
        ''' <summary>
        ''' Event OnPropertyChange
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Event OnPropertyChanged(sender As Object, e As ComplexPropertyStore.EventArgs)
        Public Event OnCurrentSetChanged(sender As Object, e As ComplexPropertyStore.EventArgs)
        ''' <summary>
        ''' returns the config set for a setname with a driversequence
        ''' </summary>
        ''' <param name="setname"></param>
        ''' <param name="driverseq"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSet(setname As String, Optional sequence As Sequence = Sequence.Primary) As Dictionary(Of String, SortedList(Of UShort, Object))
            If HasConfigSetName(setname, sequence) Then
                Return _sets.Item(key:=setname & ":" & sequence)
            End If
        End Function
        ''' <summary>
        ''' returns the config set for a setname with a driversequence
        ''' </summary>
        ''' <param name="setname"></param>
        ''' <param name="driverseq"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasProperty(name As String, Optional setname As String = Nothing, Optional sequence As Sequence = Sequence.Primary) As Boolean
            If setname Is Nothing Then
                setname = _currentset
            End If
            If setname Is Nothing Then
                setname = _defaultset
            End If
            If HasSet(setname, sequence) Then
                Dim aset = GetSet(setname:=setname, sequence:=sequence)
                Return aset.ContainsKey(key:=name)
            End If
            Return False
        End Function

        ''' <summary>
        ''' sets a Property to the TableStore
        ''' </summary>
        ''' <param name="Name">Name of the Property</param>
        ''' <param name="Object">ObjectValue</param>
        ''' <returns>returns True if succesfull</returns>
        ''' <remarks></remarks>
        Public Function SetProperty(ByVal name As String, ByVal value As Object, _
                                    Optional ByVal weight As UShort = 0,
                                    Optional setname As String = Nothing, _
                                    Optional sequence As Sequence = Sequence.Primary) As Boolean

            Dim aWeightedList As SortedList(Of UShort, Object)
            Dim aSet As Dictionary(Of String, SortedList(Of UShort, Object))
            If String.IsNullOrWhiteSpace(setname) Then
                setname = _defaultset
            End If

            If HasConfigSetName(setname, sequence) Then
                aSet = GetSet(setname, sequence:=sequence)
            Else
                aSet = New Dictionary(Of String, SortedList(Of UShort, Object))
                _sets.Add(key:=setname & ":" & sequence, value:=aSet)
            End If

            If aSet.ContainsKey(name) Then
                aWeightedList = aSet.Item(name)
                ' weight missing
                If weight = 0 Then
                    weight = aWeightedList.Keys.Max + 1
                End If
                ' retrieve
                If aWeightedList.ContainsKey(weight) Then
                    aWeightedList.Remove(weight)

                End If
                aWeightedList.Add(weight, value)
            Else
                aWeightedList = New SortedList(Of UShort, Object)
                '* get weight
                If weight = 0 Then
                    weight = 1
                End If
                aWeightedList.Add(weight, value)
                aSet.Add(name, aWeightedList)
            End If

            RaiseEvent OnPropertyChanged(Me, New ComplexPropertyStore.EventArgs(propertyname:=name, setname:=setname, weight:=weight, value:=value))
            Return True
        End Function
        ''' <summary>
        ''' Gets the Property of a config set. if setname is ommitted then check currentconfigset and the global one
        ''' </summary>
        ''' <param name="name">name of property</param>
        ''' <returns>object of the property</returns>
        ''' <remarks></remarks>
        Public Function GetProperty(ByVal name As String, Optional weight As UShort = 0, _
        Optional setname As String = Nothing, _
        Optional sequence As Sequence = Sequence.Primary) As Object

            Dim aConfigSet As Dictionary(Of String, SortedList(Of UShort, Object))
            If String.IsNullOrWhiteSpace(setname) Then
                setname = _currentset
            End If
            '* test
            If Not String.IsNullOrWhiteSpace(setname) AndAlso HasProperty(name, setname:=setname, sequence:=sequence) Then
                aConfigSet = GetSet(setname, sequence)
            ElseIf Not String.IsNullOrWhiteSpace(setname) AndAlso HasProperty(name, setname:=setname) Then
                aConfigSet = GetSet(setname)
            ElseIf String.IsNullOrWhiteSpace(setname) AndAlso _currentset IsNot Nothing AndAlso HasProperty(name, setname:=_currentset, sequence:=sequence) Then
                setname = _currentset
                aConfigSet = GetSet(setname, sequence)
            ElseIf String.IsNullOrWhiteSpace(setname) AndAlso _defaultset IsNot Nothing AndAlso HasProperty(name, setname:=_defaultset) Then
                setname = _defaultset
                aConfigSet = GetSet(setname)
            Else
                Return Nothing
            End If
            ' retrieve
            Dim aWeightedList As SortedList(Of UShort, Object)
            If aConfigSet.ContainsKey(name) Then
                aWeightedList = aConfigSet.Item(name)
                If aWeightedList.ContainsKey(weight) Then
                    Return aWeightedList.Item(weight)
                ElseIf weight = 0 Then
                    Return aWeightedList.Last.Value
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' returns a list of selectable config set names without global
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ConfigSetNamesToSelect As List(Of String)
            Get
                Return ot.ConfigSetNames.FindAll(Function(x) x <> ConstGlobalConfigSetName)
            End Get
        End Property
        ''' <summary>
        ''' returns a list of ConfigSetnames
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SetNames As List(Of String)
            Get
                Dim aList As New List(Of String)

                For Each name In _sets.Keys
                    If name.Contains(":") Then
                        name = name.Substring(0, name.IndexOf(":"))
                    End If
                    If Not aList.Contains(name) Then aList.Add(name)
                Next
                Return aList
            End Get
        End Property

        ''' <summary>
        ''' returns true if the config-set name exists 
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasSet(ByVal setname As String, Optional sequence As Sequence = Sequence.Primary) As Boolean
            If _sets.ContainsKey(setname & ":" & sequence) Then
                Return True
            Else
                Return False
            End If
        End Function

    End Class


    ''' <summary>
    ''' a DataValue Tuple holds a dynamic number of values(objects)
    ''' </summary>
    ''' <remarks>
    ''' SwRS Design Principle
    ''' 1. a Tuple can hold a dynamic number of objects
    ''' 2. the Entries can be addressed by number or by unique name
    ''' 3. The Entrynames are passed as array reference
    ''' 4. The values are cloned
    ''' </remarks>
    Public Class DataValueTuple
        Implements iKey
        Implements IQueryable


        ''' <summary>
        ''' Values 
        ''' </summary>
        ''' <remarks></remarks>
        Protected _Values() As Object

        ''' <summary>
        ''' Entrynames should be kept as reference - these are facultative
        ''' </summary>
        ''' <remarks></remarks>
        Protected _EntryNames() As String

        Private _lockobject As New Object ''' internal lock object

        ''' <summary>
        ''' constructor of an keyentry - creates an objectkey for number of keys (1..)
        ''' </summary>
        ''' <param name="registeryentry"></param>
        ''' <remarks></remarks>

        Public Sub New(size As UShort)
            ReDim _Values(size - 1)
        End Sub
        ''' <summary>
        ''' constructor with values
        ''' </summary>
        ''' <param name="values"></param>
        ''' <remarks></remarks>
        Public Sub New(values() As Object, Optional ByRef entrynames() As String = Nothing)
            _Values = values.Clone 'take a copy
            _EntryNames = entrynames 'keep reference
        End Sub
        Public Sub New(head As Object, tail() As Object, Optional ByRef entrynames() As String = Nothing)
            ReDim _Values(tail.GetUpperBound(0) + 1)
            If head IsNot Nothing Then _Values(0) = head.clone
            Array.ConstrainedCopy(tail.Clone, 0, _Values, 1, tail.Length)
            _EntryNames = entrynames 'keep reference
        End Sub
        ''' <summary>
        ''' Returns the actuals count means if initialised the number of non-nothing members
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads ReadOnly Property Count As UShort Implements iKey.Count
            Get
                ' count 
                If _Values.Count > 0 Then Return Array.FindAll(_Values, Function(x) x IsNot Nothing).Count
                Return 0
            End Get
        End Property
        ''' <summary>
        ''' returns the size of the ObjectKey Array
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Size As UShort Implements iKey.Size
            Get
                Return _Values.GetUpperBound(0) + 1
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the keys.
        ''' </summary>
        ''' <value>The keys.</value>
        Public Overridable Property Values() As Object() Implements iKey.Values
            Get
                Return Me._Values
            End Get
            Set(value As Object())
                If value.GetUpperBound(0) <> _Values.GetUpperBound(0) Then Throw New Exception("keys of this type have different bound")
                ReDim Preserve _Values(value.GetUpperBound(0))
                Me._Values = value
            End Set
        End Property
        ''' <summary>
        ''' return hashcode
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GetHashCode() As Integer
            If Me.Size = 0 Then Return 0

            Dim hashvalue As Integer = 0
            For i = _Values.GetLowerBound(0) To _Values.GetUpperBound(0)
                If _Values(i) Is Nothing Then
                    hashvalue = hashvalue Xor 0
                Else
                    hashvalue = hashvalue Xor _Values(i).GetHashCode()
                End If
            Next
            Return hashvalue
        End Function
        ''' <summary>
        ''' returns a hash value for the keys
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GetHashCode(o As Object) As Integer Implements iKey.GetHashCode
            If o IsNot Nothing Then Return o.GetHashCode
            Return Me.GetHashCode
        End Function
        ''' <summary>
        ''' Equal routine of 2 data tuples
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Equals(obj As Object) As Boolean
            Try
                Dim aKey As DataValueTuple = TryCast(obj, DataValueTuple)
                If aKey Is Nothing Then
                    Return False
                Else
                    If (aKey.Values Is Nothing AndAlso _Values IsNot Nothing) OrElse _
                        (aKey.Values IsNot Nothing AndAlso _Values Is Nothing) Then
                        Return False
                    End If
                    If (aKey.Values Is Nothing AndAlso _Values Is Nothing) Then
                        Return True
                    End If

                    If aKey.Count <> Me.Count Then Return False
                    For i As UShort = 0 To CUShort(aKey.Values.Count - 1)
                        If aKey(i) Is Nothing AndAlso Me(CUShort(i)) Is Nothing Then
                            Return True
                        ElseIf aKey(i) Is Nothing OrElse Me(CUShort(i)) Is Nothing Then
                            Return False
                        ElseIf aKey(i).GetType.Equals(Me(CUShort(i)).GetType) Then
                            If Not aKey(CUShort(i)).Equals(Me(CUShort(i))) Then Return False
                        Else
                            Try
                                Dim avalue = CTypeDynamic(aKey(i), Me(CUShort(i)).GetType)
                                If Not Me(i).Equals(avalue) Then Return False
                            Catch ex As Exception
                                Return False
                            End Try
                        End If

                    Next
                    Return True
                End If

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectKeyArray.Equals")
                Return False
            End Try
        End Function

        ''' <summary>
        ''' gets or sets the value in the data tuple by index as numeric zero bound value
        ''' </summary>
        ''' <param name="index"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Default Public Overridable Property Item(index As Integer) As Object Implements iKey.Item
            Get
                If index >= _Values.GetLowerBound(0) AndAlso index <= _Values.GetUpperBound(0) Then
                    Return _Values(index)
                Else
                    Throw New ormException(message:="DataValueTuple: index " & index & " out of bound")
                End If
            End Get
            Set(value As Object)
                If index >= _Values.GetLowerBound(0) AndAlso index <= _Values.GetUpperBound(0) Then
                    _Values(index) = value
                Else
                    Throw New ormException(message:="DataValueTuple: index " & index & "  out of bound")
                End If

            End Set
        End Property
        ''' <summary>
        '''gets or sets the value in the data tuple by index as key name
        ''' </summary>
        ''' <param name="index"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Default Public Overridable Property Item(index As String) As Object Implements iKey.Item
            Get
                Dim i As Int64
                If _EntryNames.Count > 0 Then
                    i = Array.IndexOf(_EntryNames, index)
                ElseIf IsNumeric(index) Then
                    i = Convert.ToInt64(index)
                Else
                    Throw New ormException(message:="DataValueTuple: index by string '" & index & "' is not applicable")
                End If

                If i >= _Values.GetLowerBound(0) AndAlso i <= _Values.GetUpperBound(0) Then
                    Return _Values(i)
                Else
                    Throw New ormException(message:="DataValueTuple: index " & index & " (" & i & ") out of bound")
                End If

            End Get
            Set(value As Object)
                Dim i As Integer = Array.FindIndex(Of Object)(_EntryNames, Function(x) x IsNot Nothing AndAlso x = index)
                Me(i) = value
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the key names
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Property Keys As String() Implements iKey.Keys
            Get
                Return _EntryNames
            End Get
            Set(value As String())
                _EntryNames = value
                ''' todo: check length of array + redim
            End Set
        End Property
        ''' <summary>
        ''' get an enumerator
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return _Values.ToList
        End Function

        Public ReadOnly Property ElementType As Type Implements IQueryable.ElementType
            Get

            End Get
        End Property

        Public ReadOnly Property Expression As Expressions.Expression Implements IQueryable.Expression
            Get

            End Get
        End Property

        Public ReadOnly Property Provider As IQueryProvider Implements IQueryable.Provider
            Get

            End Get
        End Property

        ''' <summary>
        ''' toString
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            If _Values IsNot Nothing Then
                Dim s As String = "["
                For i = 0 To _Values.Count - 1
                    If s <> "[" Then s &= ","
                    s &= _Values(i).ToString
                Next
                Return s & "]"
            Else
                Return "[]"
            End If

        End Function
        ''' <summary>
        ''' compare
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
            Try
                Dim aKey As DataValueTuple = TryCast(obj, DataValueTuple)
                If aKey Is Nothing Then
                    Return False
                Else
                    If (aKey.Values Is Nothing AndAlso _Values IsNot Nothing) Then
                        Return 1
                    ElseIf (aKey.Values IsNot Nothing AndAlso _Values Is Nothing) Then
                        Return -1
                    End If
                    If (aKey.Values Is Nothing AndAlso _Values Is Nothing) Then
                        Return 0
                    End If

                    If aKey.Values.Count <> _Values.Count Then Return False
                    Dim result As Integer = 0
                    For i = 0 To aKey.Values.Count - 1
                        If Not aKey(i).Equals(Me(i)) Then
                            '' compare them if we can
                            If (aKey.GetType.GetInterfaces.Contains(GetType(IComparable))) AndAlso (_Values(i).GetType.GetInterfaces.Contains(GetType(IComparable))) Then
                                Return TryCast(_Values(i), IComparable).CompareTo(TryCast(aKey(i), IComparable))
                            Else
                                Return _Values(i).ToString.CompareTo(aKey(i).ToString)
                            End If
                        End If
                    Next
                    Return result
                End If

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectKeyArray.Equals")
                Return False
            End Try
        End Function
    End Class

    ''' <summary>
    ''' a data value tuple describeing a database key by referenceing to a database key object
    ''' </summary>
    ''' <remarks>
    ''' SwRS Design Principle
    ''' 
    ''' The first data datum from the data value tuple contains a canonical keyid, if keyid is not nothing.
    ''' the keyid is either the primary key name (or nothing) orelse the name of an index of the table
    ''' 
    ''' </remarks>
    Public Class ormDatabaseKey
        Inherits DataValueTuple
        Implements ICloneable
        Implements IEquatable(Of ormDatabaseKey)

        ''' <summary>
        ''' Comparer Class for Dictioneries
        ''' </summary>
        ''' <remarks></remarks>
        Public Class Comparer
            Implements IEqualityComparer(Of ormDatabaseKey)
            Sub New()

            End Sub
            ''' <summary>
            ''' Equalses the specified x.
            ''' </summary>
            ''' <param name="x">The x.</param>
            ''' <param name="y">The y.</param>
            ''' <returns></returns>
            Public Overloads Function [Equals](x As ormDatabaseKey, y As ormDatabaseKey) As Boolean Implements IEqualityComparer(Of ormDatabaseKey).[Equals]
                Return x.Equals(y)
            End Function

            ''' <summary>
            ''' Gets the hash code.
            ''' </summary>
            ''' <param name="obj">The obj.</param>
            ''' <returns></returns>
            Public Overloads Function GetHashCode(obj As ormDatabaseKey) As Integer Implements IEqualityComparer(Of ormDatabaseKey).GetHashCode
                Return obj.GetHashCode
            End Function

        End Class

        Private _containerID As String
        Private _keyid As String 'id of the key
        Private _objectid As String ' object id of the key if assigned to an object
        Private _isUnique As Boolean

        ''' <summary>
        ''' create a primary key withvalues
        ''' </summary>
        ''' <param name="keyvalues"></param>
        ''' <remarks></remarks>
        Public Sub New(keyvalues() As Object)
            MyBase.New(keyvalues)
        End Sub
        ''' <summary>
        ''' create a key with reference to a containerID, keyvalues (optional the keyid - if nothing -> primary key)
        ''' </summary>
        ''' <param name="typeid"></param>
        ''' <param name="keyvalues"></param>
        ''' <remarks></remarks>
        Public Sub New(objectid As String, keyvalues() As Object, Optional containerID As String = Nothing, Optional keyid As Object = Nothing)
            MyBase.New(head:=keyid, tail:=keyvalues)
            SetKeyid(objectid:=objectid, containerID:=containerID, keyid:=keyid)
        End Sub
        ''' <summary>
        ''' create a key with reference to a containerID, keyvalues (optional the keyid - if nothing -> primary key)
        ''' </summary>
        ''' <param name="typeid"></param>
        ''' <param name="keyvalues"></param>
        ''' <remarks></remarks>
        Public Sub New(containerID As String, keyvalues() As Object, Optional keyid As Object = Nothing)
            MyBase.New(head:=keyid, tail:=keyvalues)
            SetKeyid(containerID:=containerID, keyid:=keyid)
        End Sub
        ''' <summary>
        ''' create a key with reference to a tableid, keyvalues (optional the keyid - if nothing -> primary key)
        ''' </summary>
        ''' <param name="typeid"></param>
        ''' <param name="keyvalues"></param>
        ''' <remarks></remarks>
        Public Sub New(objectid As String, Optional containerID As String = Nothing, Optional keyid As String = Nothing)
            MyBase.New({})
            SetKeyid(objectid:=objectid, containerID:=containerID, keyid:=keyid)
        End Sub


        ''' <summary>
        ''' Gets the objectid.
        ''' </summary>
        ''' <value>The objectid.</value>
        Public ReadOnly Property Objectid As String
            Get
                Return Me._objectid
            End Get
        End Property

        ''' <summary>
        ''' Gets the tableid for the key.
        ''' </summary>
        ''' <value>The tableid.</value>
        Public ReadOnly Property ContainerID As String
            Get
                Return Me._containerID
            End Get
        End Property
        ''' <summary>
        ''' Gets the id for the key.
        ''' </summary>
        ''' <value>The tableid.</value>
        Public ReadOnly Property KeyID As String
            Get
                Return Me._keyid
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the unique flag.
        ''' </summary>
        ''' <value>The is unique.</value>
        Public Property IsUnique As Boolean
            Get
                Return Me._isUnique
            End Get
            Set(value As Boolean)
                Me._isUnique = value
            End Set
        End Property
        ''' <summary>
        ''' returns the Names of the keys
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property EntryNames As String()
            Get
                Return _EntryNames
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the keys.
        ''' </summary>
        ''' <value>The keys.</value>
        Public Overrides Property Values() As Object()
            Get
                If _keyid Is Nothing Then Return Me._Values
                Return _Values.Skip(1).ToArray
            End Get
            Set(value As Object())
                'If value.GetUpperBound(0) <> _registery.NoKeys - 1 Then Throw New Exception("keys of this type have different bound")
                '* different on typeid or without
                If _keyid Is Nothing Then
                    Me._Values = value
                Else
                    Dim i As UShort = Math.Min(_Values.Length - 1, value.Length) ' leave out keyid (first)
                    Array.ConstrainedCopy(value, 0, _Values, 1, i)
                End If
            End Set
        End Property
        ''' <summary>
        ''' gets a primary database driver
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DatabaseDriver As iormDatabaseDriver
            Get
                Return ot.CurrentSession.GetPrimaryDatabaseDriver(containerID:=_containerID)
            End Get
        End Property
        ''' <summary>
        ''' returns the Ontrack datatype of the value
        ''' </summary>
        ''' <param name="index"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Datatype(index As Integer) As otDataType
            Get
                If _containerID IsNot Nothing AndAlso index >= Me.GetLowerBound(0) AndAlso index <= Me.GetUpperBound(0) Then
                    If ot.CurrentSession.IsRunning Then
                        Dim aContainerDefinition = CurrentSession.Objects.GetContainerDefinition(id:=_containerID)
                        If aContainerDefinition IsNot Nothing AndAlso aContainerDefinition.HasEntry(Me.EntryNames(index)) Then Return aContainerDefinition.Entries(Me.EntryNames(index)).Datatype
                        CoreMessageHandler(message:="entry name is not part of the container id", _
                                            containerEntryName:=Me.EntryNames(index), containerID:=_containerID, procedure:="ormDatabaseKey.Datatype", messagetype:=otCoreMessageType.InternalError)
                        Return 0
                    Else
                        Dim aContainerEntryDefinition = ot.ObjectClassRepository.GetContainerEntryAttribute(entryname:=Me.EntryNames(index), containerID:=_containerID)
                        If aContainerEntryDefinition IsNot Nothing Then Return aContainerEntryDefinition.DataType
                        CoreMessageHandler(message:="entry name is not part of the container id", _
                                            containerEntryName:=Me.EntryNames(index), containerID:=_containerID, procedure:="ormDatabaseKey.Datatype", messagetype:=otCoreMessageType.InternalError)
                        Return 0
                    End If
                ElseIf _containerID Is Nothing Then
                    CoreMessageHandler(message:=" container id is not set", _
                                      procedure:="ormDatabaseKey.Datatype", messagetype:=otCoreMessageType.InternalError)
                    Return 0
                Else
                    Throw New ormException(message:="ormDatabaseKey: index " & index & " out of bound")
                End If
            End Get
        End Property
        ''' <summary>
        ''' returns the Ontrack datatype of the value
        ''' </summary>
        ''' <param name="index"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Datatype(index As String) As otDataType
            Get
                If _containerID IsNot Nothing Then
                    If ot.CurrentSession.IsRunning Then
                        Dim aContainerDefinition = CurrentSession.Objects.GetContainerDefinition(ID:=_containerID)
                        If aContainerDefinition IsNot Nothing AndAlso aContainerDefinition.HasEntry(index) Then Return aContainerDefinition.Entries(index).Datatype
                        CoreMessageHandler(message:="entry name is not part of the container id", _
                                            containerEntryName:=index, containerID:=_containerID, procedure:="ormDatabaseKey.Datatype", messagetype:=otCoreMessageType.InternalError)
                        Return 0
                    Else
                        Dim aContainerEntryDefinition = ot.ObjectClassRepository.GetContainerEntryAttribute(entryname:=index, containerID:=_containerID)
                        If aContainerEntryDefinition IsNot Nothing Then Return aContainerEntryDefinition.DataType
                        CoreMessageHandler(message:="entry name is not part of the container id", _
                                            containerEntryName:=index, containerID:=_containerID, procedure:="ormDatabaseKey.Datatype", messagetype:=otCoreMessageType.InternalError)
                        Return 0
                    End If
                ElseIf _containerID Is Nothing Then
                    CoreMessageHandler(message:=" container id is not set", _
                                      procedure:="ormDatabaseKey.Datatype", messagetype:=otCoreMessageType.InternalError)
                    Return 0
                Else
                    Throw New ormException(message:="ormDatabaseKey: index " & index & " out of bound")
                End If
            End Get
        End Property
        ''' <summary>
        ''' gets or sets the item in an key
        ''' </summary>
        ''' <param name="index"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Default Public Overrides Property Item(index As Integer) As Object
            Get
                If _keyid IsNot Nothing AndAlso index >= Me.GetLowerBound(0) AndAlso index <= Me.GetUpperBound(0) Then
                    Return MyBase.Item(index + 1)
                ElseIf _keyid Is Nothing AndAlso index >= Me.GetLowerBound(0) AndAlso index <= Me.GetUpperBound(0) Then
                    Return MyBase.Item(index)
                Else
                    Throw New ormException(message:="ormDatabaseKey: index " & index & " out of bound")
                End If
            End Get
            Set(value As Object)
                If _keyid IsNot Nothing AndAlso index >= Me.GetLowerBound(0) AndAlso index <= Me.GetUpperBound(0) Then
                    MyBase.Item(index + 1) = value
                ElseIf _keyid Is Nothing AndAlso index >= Me.GetLowerBound(0) AndAlso index <= Me.GetUpperBound(0) Then
                    MyBase.Item(index) = value
                Else
                    Throw New ormException(message:="ormDatabaseKey: index " & index & "  out of bound")
                End If

            End Set
        End Property
        ''' <summary>
        ''' get an enumerator
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GetEnumerator() As IEnumerator
            Return Me.Values.ToList
        End Function
        ''' <summary>
        ''' Returns the size of the key 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads ReadOnly Property Size As UShort
            Get
                ' count if  keyid is applied (all entries -> first id not in entrynames)
                If _keyid IsNot Nothing Then Return _EntryNames.Count
                Return _Values.Count
            End Get
        End Property
        ''' <summary>
        ''' Returns the actuals count means if initialised the number of non-nothing members
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads ReadOnly Property Count As UShort
            Get
                ' count if not keyid is applied (all entires)
                If _Values.Count > 0 And _keyid Is Nothing Then Return Array.FindAll(_Values, Function(x) x IsNot Nothing).Count
                ' count if keyid is applied (leave the first out)
                If _Values.Count > 0 And _keyid IsNot Nothing Then Return Array.FindAll(_Values, Function(x) x IsNot Nothing).Count - 1
                Return 0
            End Get
        End Property
        ''' <summary>
        ''' Simulate the Upper Bound
        ''' </summary>
        ''' <param name="i"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GetUpperBound(Optional i As Integer = 0) As Short
            Get
                Return Me.Size - 1
            End Get
        End Property
        ''' <summary>
        ''' Simulate the lower Bound
        ''' </summary>
        ''' <param name="i"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GetLowerBound(Optional i As Integer = 0) As Short
            Get
                Return 0
            End Get
        End Property
        ''' <summary>
        ''' returns the index of the domain id if the key is bound to a table
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDomainIDOrdinal() As Short
            If _EntryNames.Count > 0 Then Return Array.FindIndex(_EntryNames, Function(x) x.ToUpper = Commons.Domain.ConstFNDomainID)
            Return -1
        End Function
        ''' <summary>
        ''' sets the reference keyid for this key
        ''' </summary>
        ''' <param name="keyid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function SetKeyid(Optional containerID As String = Nothing, Optional keyid As String = Nothing, Optional objectid As String = Nothing) As Boolean
            _objectid = objectid.ToUpper

            ''' this routine is based on the class descriptions not on the objectdefinition since 
            ''' the information about keys are derived from indices and tables primary key which should be tightly
            ''' linked with the code and are therefore obsolete
            ''' 

            '** if we have just the objectid
            If containerID Is Nothing AndAlso Not String.IsNullOrWhiteSpace(objectid) Then
                Dim anObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=objectid)
                If anObjectDefinition IsNot Nothing Then
                    containerID = anObjectDefinition.PrimaryContainerID
                Else
                    CoreMessageHandler(message:="object class description for '" & objectid & "' could not be retrieved from store", _
                                      procedure:="ormDatabaseKey.setKeyiD", messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If

            End If


            Dim aContainerDefinition = CurrentSession.Objects.GetContainerDefinition(id:=containerID)
            If aContainerDefinition IsNot Nothing AndAlso (keyid Is Nothing OrElse keyid.ToUpper = aContainerDefinition.PrimaryKey.ToUpper) Then
                _EntryNames = aContainerDefinition.PrimaryEntryNames
                ReDim Preserve _Values(_EntryNames.GetUpperBound(0) + 1) ' plus keyid
                _containerID = containerID.ToUpper
                _keyid = containerID.ToUpper & "." & aContainerDefinition.PrimaryKey.ToUpper
                _Values(0) = _keyid
                _isUnique = True

            ElseIf aContainerDefinition IsNot Nothing AndAlso keyid IsNot Nothing Then

                Dim anIndexAttribute As ormIndexAttribute = aContainerDefinition.GetIndex(indexname:=keyid.ToUpper)
                If anIndexAttribute Is Nothing Then
                    CoreMessageHandler(message:="keyid (indexid) '" & keyid & "' could not be retrieved from table attribute '" & containerID & "'", _
                                       procedure:="ormDatabaseKey.setKeyiD", messagetype:=otCoreMessageType.InternalError)
                    Return False
                Else
                    _containerID = containerID.ToUpper
                    _keyid = containerID.ToUpper & "." & keyid.ToUpper
                    _EntryNames = anIndexAttribute.ColumnNames
                    ReDim Preserve _Values(_EntryNames.GetUpperBound(0) + 1) 'plus keyid
                    _isUnique = anIndexAttribute.IsUnique
                    _Values(0) = _keyid
                End If
            Else
                If String.IsNullOrWhiteSpace(containerID) Then containerID = Nothing
                If String.IsNullOrWhiteSpace(keyid) Then keyid = Nothing
                CoreMessageHandler(message:="tableid or keyid couldnot be retrieved from Attributes", argument:=containerID & "." & keyid, procedure:="ormDatabaseKey.setKeyiD", messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            Return True
        End Function
        ''' <summary>
        ''' substitutes in a primary key array (of a table) the domainid with the current domainid
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="pkarray"></param>
        ''' <param name="domainid"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SubstituteDomainID(domainid As String, _
                                            Optional substituteOnlyNothingDomain As Boolean = True, _
                                            Optional runtimeOnly As Boolean = False) As Boolean
            Dim domindex As Integer = -1

            ''' beware of startup and installation
            ''' here the Substitute doesnot work and doesnot make any sense
            ''' 
            'If Not runtimeOnly AndAlso ot.CurrentSession.IsRuntimeRepositoryAvailable Then
            Dim aContainerDef As ormContainerDefinition = CurrentSession.Objects.GetContainerDefinition(id:=Me.ContainerID)
            If aContainerDef Is Nothing Then
                CoreMessageHandler(message:="container definition could not be retrieved", procedure:="ormDatabaseKey.SubstituteDomainID", _
                                argument:=domainid, containerID:=Me.ContainerID, containerEntryName:=Commons.DomainSetting.ConstFNDomainID, messagetype:=otCoreMessageType.InternalError)
                Return False
            ElseIf Not aContainerDef.HasDomainBehavior Then
                ' this might also be called if we donot have domain behavior 
                'CoreMessageHandler(message:="table definition shows no domainhebahvior -> check it", subname:="ormDatabaseKey.SubstituteDomainID", _
                '                arg1:=domainid, tablename:=tablename, columnname:=DomainSetting.ConstFNDomainID, messagetype:=otCoreMessageType.InternalError)
                Return True
            End If

            ''' check if the domain id is part of the primary key
            ''' 
            domindex = Me.GetDomainIDOrdinal
            If domindex >= 0 Then
                If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
                Me(domindex) = UCase(domainid) ' set domainid
            ElseIf aContainerDef.HasDomainBehavior Then
                CoreMessageHandler(message:="domainID is not in primary key although domain behavior is set", procedure:="ormDatabaseKey.SubstituteDomainID", _
                                   argument:=domainid, containerID:=Me.ContainerID, containerEntryName:=Commons.Domain.ConstFNDomainID, messagetype:=otCoreMessageType.InternalError)
            End If

            ''' check if nothing is in key
            ''' 
            For i = 0 To Me.GetUpperBound(0)
                If Me(i) Is Nothing Then
                    If Me.ContainerID IsNot Nothing Then
                        CoreMessageHandler(message:="part of key is nothing", procedure:="ormDatabaseKey.SubstituteDomainID", _
                             argument:=i, containerID:=Me.ContainerID, containerEntryName:=Me.EntryNames(i), messagetype:=otCoreMessageType.InternalWarning)
                    Else
                        CoreMessageHandler(message:="part of key is nothing", procedure:="ormDatabaseKey.SubstituteDomainID", _
                            argument:=i, containerID:=Me.ContainerID, messagetype:=otCoreMessageType.InternalWarning)
                    End If
                End If
            Next

            ''' return successful
            ''' 
            Return True
            'Else
            '    ''' do the same but use the attributes since we are bootstrapping or starting up
            '    ''' 
            '    Dim aContainerAttribute As iormContainerAttribute = ot.GetContainerAttribute(Me.ContainerID)
            '    If aContainerAttribute Is Nothing Then
            '        CoreMessageHandler(message:="table attribute could not be retrieved", procedure:="ormDatabaseKey.SubstituteDomainID", _
            '                        argument:=domainid, containerID:=Me.ContainerID, containerEntryName:=Commons.Domain.ConstFNDomainID, messagetype:=otCoreMessageType.InternalError)
            '        Return False
            '    ElseIf (aContainerAttribute.HasValueAddDomainBehavior AndAlso aContainerAttribute.AddDomainBehavior) Then
            '        Dim keynames As String() = aContainerAttribute.PrimaryEntryNames
            '        domindex = Array.FindIndex(Me.EntryNames, Function(s) s.ToLower = Commons.Domain.ConstFNDomainID.ToLower)
            '        If domindex >= 0 Then
            '            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID

            '            If Me.Size = keynames.Count Then
            '                ' set only if nothing is set
            '                If Me(domindex) Is Nothing OrElse String.IsNullOrWhiteSpace(Me(domindex)) Then
            '                    Me(domindex) = UCase(domainid)
            '                ElseIf Me(domindex) <> UCase(domainid) Then
            '                    Me(domindex) = UCase(domainid)
            '                End If
            '            Else
            '                'ReDim Preserve primarykey(keynames.Count)
            '                Me(domindex) = UCase(domainid)
            '            End If
            '        Else
            '            CoreMessageHandler(message:="domainID is not in primary key although domain behavior is set", procedure:="ormDataObject.SubstituteDomainIDinPKArray", _
            '                         argument:=domainid, containerID:=Me.ContainerID, containerEntryName:=Commons.Domain.ConstFNDomainID, messagetype:=otCoreMessageType.InternalError)
            '            Return False
            '        End If
            '    Else
            '        Return True
            '    End If

            '    Return True
            'End If
            Return True
        End Function
        ''' <summary>
        ''' helper routine to check and fix the primary key on length, datatype and domain substitution
        ''' </summary>
        ''' <param name="pkarray"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ChecknFix(domainid As String, _
                                    Optional substitueOnlyNothingDomain As Boolean = True, _
                                    Optional runtimeOnly As Boolean = False) As Boolean

            If Me.Objectid Is Nothing Then Return False

            Dim aDescription = ot.GetObjectClassDescriptionByID(id:=Me.Objectid)

            ''' Substitute the DomainID
            '''
            SubstituteDomainID(substituteOnlyNothingDomain:=substitueOnlyNothingDomain, domainid:=domainid, runtimeOnly:=runtimeOnly)

            ''' convert the  key fields
            ''' 
            Dim i As UShort = 0
            For Each aColumnname In Me.EntryNames
                Dim aMappingList As IEnumerable(Of FieldInfo) = aDescription.GetMappedContainerEntry2FieldInfos(containerEntryName:=aColumnname, containerID:=Me.ContainerID)

                If aMappingList IsNot Nothing Then
                    For Each aMapping In aMappingList
                        If Me(i) Is Nothing Then
                            'do nothing since the event handler to generate a key might be called by an event
                            '
                            'CoreMessageHandler(message:="part of primary key must not be nothing", arg1:=pkarray(i), _
                            '                   objectname:=aDescription.Name, messagetype:=otCoreMessageType.InternalError, _
                            '                   subname:="ormDatabaseKey.SubstituteDomainID)
                            'Return False
                        ElseIf Not Me(i).GetType.Equals(aMapping.FieldType) Then
                            Dim avalue = Me(i)
                            Try
                                Me(i) = CTypeDynamic(avalue, aMapping.FieldType)
                            Catch ex As Exception
                                CoreMessageHandler(exception:=ex, argument:=Me(i), procedure:="ormDatabaseKey.SubstituteDomainID")
                                Return False
                            End Try

                        End If

                    Next
                End If

                ''' increase
                ''' 
                i += 1
            Next
            Return True
        End Function

        ''' <summary>
        ''' clone this key
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Clone() As Object Implements ICloneable.Clone
            Return New ormDatabaseKey(objectid:=_objectid, keyid:=_keyid, containerID:=_containerID, keyvalues:=_Values)
        End Function

        ''' <summary>
        ''' Equal routine of 2 keys
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Equals(obj As Object) As Boolean
            Dim aKey As ormDatabaseKey = TryCast(obj, ormDatabaseKey)
            If aKey Is Nothing Then Return False
            Return Me.Equals(other:=aKey)
        End Function

        ''' <summary>
        ''' Equalses the specified other.
        ''' </summary>
        ''' <param name="other">The other.</param>
        ''' <returns></returns>
        Public Overloads Function [Equals](other As ormDatabaseKey) As Boolean Implements IEquatable(Of ormDatabaseKey).[Equals]
            If other.KeyID = Me.KeyID Then
                Return MyBase.Equals(other) ' test the values
            End If
            Return False
        End Function
        ''' <summary>
        ''' return the hashcode
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetHashCode() As Integer
            Return MyBase.GetHashCode
        End Function

        ''' <summary>
        ''' compare
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function CompareTo(obj As Object) As Integer
            Dim aKey As ormDatabaseKey = TryCast(obj, ormDatabaseKey)
            If aKey Is Nothing Then Return False
            If aKey.KeyID = Me.KeyID Then
                Return MyBase.CompareTo(aKey) ' test the values
            End If
            Return Me.KeyID.CompareTo(aKey.KeyID)
        End Function
        ''' <summary>
        ''' to string function
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Dim aString As New System.Text.StringBuilder("[<")

            If _keyid IsNot Nothing Then
                aString.Append(KeyID)
            Else
                aString.Append("-")
            End If
            aString.Append(">")

            Dim first As Boolean = True
            For Each aValue In Me.Values
                If Not first Then
                    aString.Append(",")
                Else
                    first = False
                End If
                If aValue IsNot Nothing Then
                    aString.Append(aValue.ToString)
                Else
                    aString.Append("NULL")
                End If

            Next
            aString.Append("]")
            Return aString.ToString
        End Function

        ''' <summary>
        ''' add the key to a record
        ''' </summary>
        ''' <param name="pkArray"></param>
        ''' <remarks></remarks>
        Public Function ToRecord(ByRef record As ormRecord, objectclassdescription As ObjectClassDescription,
                                                Optional domainid As String = Nothing, _
                                                Optional runtimeOnly As Boolean = False) As Boolean
            ''' get list of column names
            ''' 
            Dim aList As List(Of String)
            Dim i As UShort = 0
            If String.IsNullOrEmpty(domainid) Then domainid = ConstGlobalDomain
            If [objectclassdescription] IsNot Nothing AndAlso [objectclassdescription].PrimaryKeyEntryNames.Count > 0 Then
                aList = [objectclassdescription].PrimaryKeyEntryNames.ToList
            Else
                CoreMessageHandler(message:="no object class description found", objectname:=Me.Objectid, procedure:="ormDatabaseKey.ToRecord", _
                                   messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            ''' lookup the list of primary keys
            ''' 
            For Each anEntryName In aList
                If (record.IsBound AndAlso record.HasIndex(anEntryName)) OrElse Not record.IsBound Then
                    If anEntryName IsNot Nothing Then
                        If anEntryName.ToUpper <> Commons.Domain.ConstFNDomainID Then
                            record.SetValue(anEntryName, Me(i))
                        Else
                            If Me(i) Is Nothing OrElse Me(i) = String.Empty Then
                                record.SetValue(anEntryName, domainid)
                            Else
                                record.SetValue(anEntryName, Me(i))
                            End If
                        End If

                    End If
                Else
                    CoreMessageHandler(message:="record index not found", objectname:=Me.Objectid, procedure:="ormDatabaseKey.ToRecord", _
                                       entryname:=anEntryName, messagetype:=otCoreMessageType.InternalError)
                End If
                i = i + 1
            Next

            Return True
        End Function
    End Class


    ''' <summary>
    ''' represents a record data tuple for to be stored and retrieved in a data store
    ''' </summary>
    ''' <remarks>
    ''' Design Principles
    ''' 
    ''' 1. An ormRecord can be bound to one or multiple containers -> bound mode and fixed number of entries or columns
    '''    1.1 in Bound mode the re record is always set to all the entries of the container / columns of the table
    '''    1.2 In Bound mode the record should also know if it is created or loaded or changed
    ''' 2. An ormRecord can also be set individual by entry name -> unbound dynamic
    ''' 3. An ormRecord is splittup in entrynames in the form [table].[columnname]
    '''    3.1 it can be adressed either by entry name or by number 
    ''' 4. Keep the orginal values
    '''</remarks>
    Public Class ormRecord
        Inherits Dynamic.DynamicObject

        Private _FixEntries As Boolean = False
        Private _isBound As Boolean = False
        Private _ContainerStores As iormContainerStore() = {}
        Private _DbDriver As iormDatabaseDriver = Nothing
        Private _entrynames() As String = {}
        Private _Values() As Object = {}
        Private _OriginalValues() As Object = {}
        Private _isCreated As Boolean = False
        Private _isUnknown As Boolean = True
        Private _isLoaded As Boolean = False
        Private _isChanged As Boolean = False
        Private _ContainerIds As String() = {}
        Private _upperRangeofContainer As ULong() = {}
        Private _isnullable As Boolean() = {}

        '** initialize
        Public Sub New()

        End Sub

        Public Sub New(ByVal containerID As String, _
                       Optional dbdriver As iormDatabaseDriver = Nothing, _
                       Optional fillDefaultValues As Boolean = False, _
                       Optional runtimeOnly As Boolean = False)
            _DbDriver = dbdriver
            ReDim _ContainerIds(0)
            _ContainerIds(0) = containerID
            If Not runtimeOnly Then
                Me.SetContainer(containerID, forceReload:=False, dbdriver:=dbdriver, fillDefaultValues:=fillDefaultValues)
                _FixEntries = True
            End If
        End Sub

        Public Sub New(ByVal containerIDs As String(), _
                       Optional dbdriver As iormRelationalDatabaseDriver = Nothing, _
                       Optional fillDefaultValues As Boolean = False, _
                       Optional runtimeOnly As Boolean = False)
            _DbDriver = dbdriver
            _ContainerIds = containerIDs
            If Not runtimeOnly Then
                Me.SetContainers(containerIDs, forceReload:=False, dbdriver:=dbdriver, fillDefaultValues:=fillDefaultValues)
                _FixEntries = True
            End If
        End Sub

        Public Sub Finalize()
            _DbDriver = Nothing
            _ContainerStores = Nothing
            _Values = Nothing
            _OriginalValues = Nothing
        End Sub

        ' If you try to get a value of a property that is
        ' not defined in the class, this method is called.
        ''' <summary>
        ''' dynamic getValue Property
        ''' </summary>
        ''' <param name="binder"></param>
        ''' <param name="result"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function TryGetMember(
            ByVal binder As System.Dynamic.GetMemberBinder,
            ByRef result As Object) As Boolean

            ' Converting the property name to lowercase
            ' so that property names become case-insensitive.
            Dim name As String = binder.Name

            ' If the property name is found in a dictionary,
            ' set the result parameter to the property value and return true.
            ' Otherwise, return false.
            Dim flag As Boolean
            result = Me.GetValue(index:=name, notFound:=flag)
            Return flag
        End Function
        ''' <summary>
        ''' Dynamic setValue Property
        ''' </summary>
        ''' <param name="binder"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function TrySetMember(
            ByVal binder As System.Dynamic.SetMemberBinder,
            ByVal value As Object) As Boolean

            ' Converting the property name to lowercase
            ' so that property names become case-insensitive.
            Return Me.SetValue(index:=binder.Name, value:=value)

        End Function
        ''' <summary>
        ''' Gets the is table set.
        ''' </summary>
        ''' <value>The is table set.</value>
        Public ReadOnly Property IsBound() As Boolean
            Get
                Return Me._isBound
            End Get
        End Property

        ''' <summary>
        ''' set if this record is a new Record in the databse
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsCreated As Boolean
            Get
                Return _isCreated
            End Get
            Protected Friend Set(value As Boolean)

                If value Then
                    _isCreated = True
                    _isLoaded = False
                    _isUnknown = False
                End If
            End Set
        End Property
        ''' <summary>
        ''' set if the record state is unkown if new or load
        ''' </summary>
        ''' <value>The is unknown.</value>
        Public Property IsUnknown() As Boolean
            Get
                Return Me._isUnknown
            End Get
            Set(value As Boolean)
                Me._isUnknown = value
                If value Then
                    Me.IsCreated = False
                    Me.IsLoaded = False
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the is changed.
        ''' </summary>
        ''' <value>The is changed.</value>
        Public Property IsChanged() As Boolean
            Get
                Return Me._isChanged
            End Get
            Protected Friend Set(value As Boolean)
                Me._isChanged = value
            End Set
        End Property
        ''' <summary>
        ''' set if record is loaded
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsLoaded As Boolean
            Get
                Return _isLoaded
            End Get
            Protected Friend Set(value As Boolean)
                If value Then
                    _isCreated = False
                    _isLoaded = True
                    _isUnknown = False
                End If
            End Set
        End Property
        ''' <summary>
        ''' returns true if record is alive
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Alive As Boolean
            Get
                If _FixEntries Then
                    Return _isBound
                Else
                    Return True
                End If

            End Get
        End Property
        ''' <summary>
        ''' returns Length of Record
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Length As Integer
            Get
                Length = UBound(_Values)
            End Get
        End Property
        ''' <summary>
        '''  the TableID to the Record
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ContainerIDS As String()
            Get
                Return _ContainerIds
            End Get
            Private Set(value As String())
                If Not _isBound Then
                    _ContainerIds = value
                Else
                    CoreMessageHandler(message:="containerids cannot be assigned after binding a record", procedure:="ormRecord.containerids")
                    Throw New ormException(message:="containerids cannot be assigned after binding a record")
                End If
            End Set
        End Property

        ''' <summary>
        ''' returns the container store for the container id if bound
        ''' </summary>
        ''' <param name="tableid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RetrieveContainerStore(containerID As String) As iormContainerStore
            If _isBound Then
                Dim i As Integer = Array.IndexOf(_ContainerIds, containerID.ToUpper)
                If i >= 0 Then Return _ContainerStores(i)
                Return Nothing
            Else
                Return Nothing
            End If

        End Function
        ''' <summary>
        ''' returns the containerstores as array or nothing
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ContainerStores As iormContainerStore()
            Get
                If Alive Then
                    Return _ContainerStores
                Else
                    Return Nothing
                End If
            End Get

        End Property

        ''' <summary>
        ''' returns the values
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Values As List(Of Object)
            Get
                Return _Values.ToList
            End Get
        End Property
        ''' <summary>
        ''' returns the values
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend ReadOnly Property ValuesArray As Object()
            Get
                Return _Values
            End Get
        End Property

        ''' <summary>
        ''' Merge Values of an record in own record
        ''' </summary>
        ''' <param name="record"></param>
        ''' <returns>True if successfull </returns>
        ''' <remarks></remarks>
        Public Function Merge(record As ormRecord) As Boolean
            Dim result As Boolean = True
            ' take all values
            For Each key In record.Entrynames
                If (Me.IsBound AndAlso Me.HasIndex(key)) OrElse Not Me.IsBound Then result = result And Me.SetValue(key, record.GetValue(key))
            Next
            ' take over also the status if we have none
            If Not Me.IsLoaded AndAlso Not Me.IsCreated AndAlso (record.IsCreated OrElse record.IsLoaded) Then Me.IsLoaded = record.IsLoaded

            Return result
        End Function
        ''' <summary>
        ''' checkStatus if loaded or created by checking if Record exists in Table. Sets the isChanged / isLoaded Property
        ''' </summary>
        ''' <returns>true if successfully checked</returns>
        ''' <remarks></remarks>
        Public Function CheckStatus(Optional ByRef status As Boolean() = Nothing) As Boolean
            Dim aLoad As Boolean = False
            Dim aCreate As Boolean = False

            '** not loaded and not created but alive ?!
            If Not Me.IsLoaded AndAlso Not Me.IsCreated AndAlso Alive Then

                ReDim status(_ContainerIds.Length - 1)
                For n = 0 To _ContainerStores.Length - 1
                    Dim pkarr() As Object
                    Dim i, index As Integer
                    Dim value As Object

                    Dim aRecord As ormRecord
                    Try
                        ReDim pkarr(0 To _ContainerStores(n).ContainerSchema.NoPrimaryEntries - 1)
                        For i = 1 To _ContainerStores(n).ContainerSchema.NoPrimaryEntries
                            index = _ContainerStores(n).ContainerSchema.GetOrdinalOfPrimaryEntry(i)
                            value = Me.GetValue(index)
                            pkarr(i - 1) = value
                        Next i
                        ' delete
                        aRecord = _ContainerStores(n).GetRecordByPrimaryKey(pkarr)
                        status(n) = aRecord IsNot Nothing

                        If aRecord Is Nothing Then
                            aCreate = True
                        Else
                            aLoad = True
                        End If
                    Catch ex As Exception
                        Call CoreMessageHandler(exception:=ex, message:="Exception", messagetype:=otCoreMessageType.InternalException, _
                                              procedure:="ormRecord.checkStatus")
                        Return False
                    End Try
                Next

                If aLoad And Not aCreate Then
                    Me.IsLoaded = True
                ElseIf aCreate And Not aLoad Then
                    Me.IsCreated = True
                Else
                    Me.IsUnknown = True
                    'not determinable
                End If

            End If


            Return True
        End Function

        ''' <summary>
        ''' sets the default value to an index
        ''' </summary>
        ''' <param name="index"></param>
        ''' <param name="tableid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDefaultValue(index As Object) As Object
            Dim i As Integer
            ''' only on bound
            ''' 
            If Not Me.Alive Or Not Me.IsBound Then
                Return Nothing
            End If

            If IsNumeric(index) Then
                i = CInt(index) - 1
            Else
                i = ZeroBasedIndexOf(index)
                If i < 0 Then
                    Return Nothing
                End If
            End If

            ' prevent overflow
            If Not (i > 0 And i <= _Values.Count) Then
                Return Nothing
            End If

            '* set the default values
            '* do not allow recursion on objectentrydefinition table itself
            '* since this is not included 

            Dim names As String() = Shuffle.NameSplitter(index.ToString)
            Dim n As Integer = Array.IndexOf(_ContainerIds, names(0))
            If n >= 0 Then
                Return _ContainerStores(n).ContainerSchema.GetDefaultValue(i)
            Else
                Return Nothing
            End If

        End Function

        ''' <summary>
        ''' set the container of this records and bind it to it
        ''' </summary>
        ''' <param name="tableID"></param>
        ''' <param name="dbdriver"></param>
        ''' <param name="tablestore"></param>
        ''' <param name="forceReload"></param>
        ''' <param name="fillDefaultValues"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Function SetContainer(ByVal containerID As String, _
                                 Optional dbdriver As iormDatabaseDriver = Nothing, _
                                 Optional forceReload As Boolean = False, _
                                 Optional fillDefaultValues As Boolean = False) As Boolean
            Return Me.SetContainers(containerIDs:={containerID}, dbdriver:=dbdriver, forceReload:=forceReload, fillDefaultValues:=fillDefaultValues)
        End Function


        ''' <summary>
        ''' set the tables of this record and bind it to them !
        ''' </summary>
        ''' <param name="TableID">Name of the Table</param>
        ''' <param name="ForceReload">Forece to reaassign</param>
        ''' <returns>True if ssuccessfull</returns>
        ''' <remarks></remarks>
        ''' 
        Public Function SetContainers(ByVal containerIDs() As String, _
                                 Optional dbdriver As iormDatabaseDriver = Nothing, _
                                 Optional forceReload As Boolean = False, _
                                 Optional fillDefaultValues As Boolean = False) As Boolean

            If Not _isBound Or forceReload Then

                ReDim _ContainerStores(containerIDs.Length - 1)
                ReDim _upperRangeofContainer(containerIDs.Length - 1)
                Dim totalsize As ULong = 0

                ''' PHASE I: get the tstores
                '''
                For I = 0 To _ContainerStores.Length - 1
                    If dbdriver Is Nothing Then dbdriver = CurrentSession.GetPrimaryDatabaseDriver(containerID:=containerIDs(I))
                    ' get the store
                    _ContainerStores(I) = dbdriver.RetrieveContainerStore(containerIDs(I))

                    If _ContainerStores(I) Is Nothing OrElse _ContainerStores(I).ContainerSchema Is Nothing _
                        OrElse Not _ContainerStores(I).ContainerSchema.IsInitialized Then

                        CoreMessageHandler(message:="record cannot be bound to container - store cannot be initialized", argument:=containerIDs(I), _
                                           procedure:="ormRecord.setContainers")
                        Return False
                    Else
                        '' set the upper ranges in the record
                        _upperRangeofContainer(I) = _ContainerStores(I).ContainerSchema.NoEntries - 1
                        totalsize += _upperRangeofContainer(I)
                    End If

                Next I

                ''' PHASE II : resize the internals
                ''' 
                '*** redim else and set the default values
                ReDim Preserve _Values(totalsize)
                ReDim Preserve _OriginalValues(totalsize)
                ReDim Preserve _isnullable(totalsize)
                'ReDim Preserve _entrynames(totalsize) ' not here we rely on _entrynames to see if we are used before binding
                _ContainerIds = containerIDs

                ''' set the values and entries
                _isBound = True
                _FixEntries = True

                ' get the number of fields
                If totalsize > 0 Then

                    '*** if there have been entries before or was set to another table
                    '*** preserve as much as possible
                    If _entrynames.GetUpperBound(0) > 0 Then

                        Dim newValues(totalsize) As Object
                        Dim newOrigValues(totalsize) As Object
                        Dim newEntrynames(totalsize) As String

                        For I = 0 To _ContainerStores.Length - 1
                            Dim aTablename As String = _ContainerStores(I).ContainerID.ToUpper
                            '** re-sort 
                            For j = 1 To _ContainerStores(I).ContainerSchema.NoEntries

                                ''' calculate new index
                                Dim index As UShort = 0
                                If I > 0 Then index = _upperRangeofContainer(I - 1)
                                index += j - 1
                                Dim aFieldname As String = _ContainerStores(I).ContainerSchema.GetEntryName(j).ToUpper
                                Dim aCanonicalName As String = aTablename & "." & aFieldname
                                newEntrynames(index) = aCanonicalName
                                ''' fill the nullable
                                _isnullable(index) = _ContainerStores(I).ContainerSchema.GetNullable(j)
                                '' get old index 
                                Dim oldindex As Integer = Array.FindIndex(_entrynames, Function(x) x IsNot Nothing AndAlso (x.ToUpper = aFieldname OrElse x.ToUpper = aCanonicalName))
                                If oldindex >= _Values.GetLowerBound(0) And oldindex <= _Values.GetUpperBound(0) Then
                                    newValues(index) = _Values(oldindex)
                                    newOrigValues(index) = _Values(oldindex)
                                Else
                                    ' can be - default value ? CoreMessageHandler(message:="index not found", subname:="ormRecord.SetTables", messagetype:=otCoreMessageType.InternalError)
                                End If

                            Next
                        Next

                        '** change over
                        _Values = newValues
                        _OriginalValues = newOrigValues
                        _entrynames = newEntrynames
                    Else
                        ReDim Preserve _entrynames(totalsize)
                        ''' set the entry names and initial values
                        ''' for each table
                        For I = 0 To _ContainerStores.Length - 1
                            For j = 1 To _ContainerStores(I).ContainerSchema.NoEntries
                                ''' calculate index
                                Dim index As UShort = 0
                                If I > 0 Then index = _upperRangeofContainer(I - 1)
                                index += j - 1
                                ''' set fieldname
                                _entrynames(index) = _ContainerStores(I).ContainerID.ToUpper & "." & _ContainerStores(I).ContainerSchema.GetEntryName(j).ToUpper
                                ''' fill the nullable
                                _isnullable(index) = _ContainerStores(I).ContainerSchema.GetNullable(j)
                                ''' fill default from tablestore
                                If fillDefaultValues Then
                                    If Not _ContainerStores(I).ContainerSchema.GetNullable(j) Then
                                        _Values(index) = Me.GetDefaultValue(j)
                                    Else
                                        _Values(index) = Nothing
                                    End If
                                End If
                                ''' set the orginal values with default values
                                _OriginalValues(index) = _Values(index)

                            Next
                        Next

                    End If

                    Return _isBound

                Else
                    Call CoreMessageHandler(message:="container store or container schema is not initialized", procedure:="ormRecord.setContainers", _
                                          messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If

                Return False
            Else
                Return True 'already set
            End If
        End Function
        ''' <summary>
        ''' persists the Record in the Database
        ''' </summary>
        ''' <param name="aTimestamp">Optional TimeStamp for using the persist</param>
        ''' <returns>true if successfull</returns>
        ''' <remarks></remarks>
        Public Function Persist(Optional ByVal timestamp As Date = ot.constNullDate) As Boolean
            Dim result As Boolean = True
            Dim aStatus As Boolean()
            '** try to set the table
            If Not _isBound And _ContainerIds.Length <> 0 Then
                Me.SetContainers(containerIDs:=_ContainerIds)
            End If
            '** only on success
            If _isBound Then
                If timestamp = constNullDate Then timestamp = Date.Now
                '' check for status
                If Not Me.IsCreated AndAlso Not Me.IsLoaded Then CheckStatus(aStatus)
                '* persist in each store
                For i = 0 To _ContainerStores.Length - 1
                    result = result And _ContainerStores(i).PersistRecord(Me, timestamp:=timestamp)
                Next i
                '* result
                If result Then
                    Me.IsLoaded = True
                    Me.IsCreated = False
                    Me.IsChanged = False
                    Return True
                End If
            Else
                CoreMessageHandler(message:="unbound record cannot be persisted", messagetype:=otCoreMessageType.InternalError, procedure:="ormRecord.Persist")
                Return False
            End If

            Return False
        End Function

        ''' <summary>
        ''' Deletes the Record in all stores
        ''' </summary>
        ''' <returns>true if successfull</returns>
        ''' <remarks></remarks>

        Public Function Delete() As Boolean
            Dim pkarr() As Object
            Dim i, index As Integer
            Dim result As Boolean = True

            If _isBound Then
                For n = 0 To _ContainerStores.Length - 1
                    ReDim pkarr(0 To _ContainerStores(n).ContainerSchema.NoPrimaryEntries - 1)
                    For i = 0 To _ContainerStores(n).ContainerSchema.NoPrimaryEntries - 1
                        ''' get index
                        If n > 0 Then
                            index = _upperRangeofContainer(n - 1)
                        Else
                            index = 0
                        End If
                        index += _ContainerStores(n).ContainerSchema.GetOrdinalOfPrimaryEntry(i + 1)
                        If Me.HasIndex(index) Then
                            pkarr(i) = Me.GetValue(index)
                        Else
                            CoreMessageHandler(message:="part of primary key for store is not in record", containerEntryName:=index, _
                                               containerID:=_ContainerStores(n).ContainerID, procedure:="ormRecord.Delete", messagetype:=otCoreMessageType.InternalError)
                        End If

                    Next i
                    ' delete
                    result = result And _ContainerStores(n).DeleteRecordByPrimaryKey(pkarr)
                Next
                Return result
            Else
                Call CoreMessageHandler(procedure:="ormRecord.delete", message:="Record not bound to a store", _
                                      messagetype:=otCoreMessageType.InternalError)
                Return False
            End If
            Return False
        End Function
        ''' <summary>
        ''' returns true if the record has the index either numerical (1..) or by name
        ''' a tablename in form [tablename].[columnname] will be stripped of and checked too 
        ''' </summary>
        ''' <param name="anIndex"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasIndex(index As Object) As Boolean
            If IsNumeric(index) Then
                Dim i = CInt(index) - 1
                If i >= LBound(_Values) And i <= UBound(_Values) Then
                    Return True
                Else
                    Return False
                End If
            Else
                If ZeroBasedIndexOf(index) >= 0 Then Return True
            End If

        End Function

        ''' <summary>
        ''' returns a list of Entry names
        ''' </summary>
        ''' <returns>List(of String)</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Entrynames() As List(Of String)
            Get
                ' no table ?!
                If Not Me.Alive Then
                    Return New List(Of String)
                ElseIf _isBound And _entrynames.Length = 0 Then
                    Dim aList As New List(Of String)
                    For n = 0 To _ContainerStores.Length - 1
                        aList.AddRange(_ContainerStores(n).ContainerSchema.EntryNames)
                    Next
                Else
                    Entrynames = _entrynames.ToList
                End If
            End Get
        End Property

        ''' <summary>
        ''' gets the index of an entryname 0 ... n !!
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Function ZeroBasedIndexOf(entryname As String) As Integer
            entryname = entryname.ToUpper
            Dim i As Integer = Array.IndexOf(_entrynames, entryname.ToUpper)
            If i < 0 Then
                Dim names As String() = Shuffle.NameSplitter(entryname)
                If names.Count > 1 Then
                    If _isBound Then
                        i = 0
                        Dim n As Integer = Array.IndexOf(_ContainerIds, names(0))
                        If n < 0 Then Return -1
                        If n > 0 Then i = _upperRangeofContainer(n - 1)
                        i += _ContainerStores(n).ContainerSchema.GetEntryOrdinal(names(1)) - 1
                        Return i
                    Else
                        Dim acolumnname As String = Shuffle.NameSplitter(entryname).Last
                        Return Array.FindIndex(_entrynames, Function(x) x IsNot Nothing AndAlso (x.ToUpper = entryname OrElse x = acolumnname OrElse entryname = Shuffle.NameSplitter(x).Last))
                    End If
                Else

                    Return Array.FindIndex(_entrynames, Function(x) x IsNot Nothing AndAlso (x.ToUpper = entryname.ToUpper OrElse entryname.ToUpper = Shuffle.NameSplitter(x).Last))
                End If

            Else
                Return i 'if found or not bound
            End If

        End Function
        ''' <summary>
        ''' returns True if Value of anIndex is Changed
        ''' </summary>
        ''' <param name="anIndex">index in Number 1..n or fieldname</param>
        ''' <returns>True on Change</returns>
        ''' <remarks></remarks>
        Public Function IsValueChanged(ByVal index As Object) As Boolean
            Dim i As Integer

            ' no table ?!
            If Not _isBound Then
                Call CoreMessageHandler(procedure:="ormRecord.isValueChanged", argument:=index, message:="record is not bound to container")
                Return False
            End If

            If IsNumeric(index) Then
                i = CInt(index) - 1
            Else
                i = ZeroBasedIndexOf(index)
                If i < 0 Then Return False
            End If
            ' set the value
            If (i) >= LBound(_Values) And (i) <= UBound(_Values) Then
                If (Not _OriginalValues(i) Is Nothing AndAlso Not _OriginalValues(i).Equals(_Values(i)) _
                    OrElse IsCreated) Then
                    Return True
                Else
                    _isChanged = _isChanged And False
                    Return False
                End If

            Else

                Call CoreMessageHandler(message:="Index of " & index & " is out of bound ", _
                                      procedure:="ormRecord.isIndexChangedValue", argument:=index, _
                                      messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

        End Function
        ''' <summary>
        ''' sets the record to an array
        ''' </summary>
        ''' <param name="array"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function [Set](ByRef [array] As Object(), Optional ByRef names As Object() = Nothing) As Boolean
            ' no table ?!
            If Not Me.Alive Then
                Return False
            End If
            '** fixed ?!
            Try
                If _Values.GetUpperBound(0) > 0 Then
                    If [array].GetUpperBound(0) <> _Values.GetUpperBound(0) Then
                        CoreMessageHandler(message:="input array has different upper bound than the set values array", argument:=[array].GetUpperBound(0), _
                                            messagetype:=otCoreMessageType.InternalError)
                        Return False
                    Else
                        _OriginalValues = _Values.Clone
                        _Values = [array].Clone
                        If Not names Is Nothing Then
                            _entrynames = names.Clone
                        End If
                        Return True
                    End If
                Else
                    ReDim _Values([array].Length)
                    ReDim _OriginalValues([array].Length)
                    _Values = [array].Clone
                    If Not names Is Nothing Then
                        _entrynames = names.Clone
                    End If
                End If

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ormRecord.Set")
                Return False
            End Try



        End Function


        ''' <summary>
        ''' set the Value of an Entry of the Record
        ''' </summary>
        ''' <param name="anIndex">Index as No 1...n or name or [tablename].[columnname]</param>
        ''' <param name="anValue">value</param>
        ''' <param name="FORCE"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetValue(ByVal index As Object, ByVal value As Object, Optional ByVal force As Boolean = False) As Boolean
            Dim i As Integer

            Try
                ' no table ?!
                If Not Me.Alive And Not force Then
                    SetValue = False
                    Exit Function
                End If
                '*
                If DBNull.Value.Equals(value) Then
                    value = Nothing
                End If

                If IsNumeric(index) Then
                    i = CLng(index) - 1
                    If i > _entrynames.GetUpperBound(0) OrElse i < 0 Then
                        CoreMessageHandler(message:="index is out of range 0.." & _entrynames.GetUpperBound(0), argument:=i, _
                                            messagetype:=otCoreMessageType.InternalError, procedure:="ormRecord.SetValue")
                        Return False  'wrong table
                    End If

                Else
                    i = ZeroBasedIndexOf(index)
                    If i < 0 And _isBound Then
                        CoreMessageHandler(message:="column name was not found as index in record", argument:=index, _
                                            messagetype:=otCoreMessageType.InternalError, procedure:="ormRecord.SetValue")
                        Return False  'wrong table
                    End If

                End If
                '*** else dynamic extend

                '** extend if not found
                If i < 0 Then
                    i = _entrynames.GetUpperBound(0) + 1

                    ReDim Preserve _entrynames(i)
                    ReDim Preserve _Values(i)
                    ReDim Preserve _OriginalValues(i)
                    ReDim Preserve _isnullable(i)

                    If index.ToString.Contains("."c) OrElse index.ToString.Contains(ConstDelimiter) Then
                        _entrynames(i) = index.ToString.ToUpper
                    ElseIf _ContainerIds.Count = 1 Then
                        _entrynames(i) = _ContainerIds(0) & "." & index.ToString.ToUpper
                    Else
                        _entrynames(i) = index.ToString.ToUpper
                    End If

                    _isnullable(i) = True
                End If

                '''' set the value
                '''

                If (i) >= LBound(_Values) And (i) <= UBound(_Values) Then
                    ' save old value
                    _OriginalValues(i) = _Values(i)
                    ' condition to accept nothing
                    If (value Is Nothing AndAlso _isnullable(i)) Then
                        _Values(i) = Nothing
                    ElseIf value Is Nothing AndAlso _isnullable(i) AndAlso Reflector.IsNullableTypeOrString(value) Then
                        _Values(i) = Nothing
                    ElseIf value Is Nothing And Not _isnullable(i) Then
                        _Values(i) = GetDefaultValue(i)
                    Else
                        If (value.GetType.GetInterfaces.Contains(GetType(ICloneable))) Then
                            _Values(i) = value.clone
                        Else
                            _Values(i) = value
                        End If
                    End If

                    If _OriginalValues(i) Is Nothing Then
                        _isChanged = False
                    ElseIf (Not _OriginalValues(i) Is Nothing And Not _Values(i) Is Nothing) _
                        AndAlso ((_OriginalValues(i).GetType().Equals(_Values(i)) AndAlso _OriginalValues(i) <> _Values(i))) _
                        OrElse (Not _OriginalValues(i).GetType().Equals(_Values(i))) Then
                        _isChanged = True
                    ElseIf (Not _OriginalValues(i) Is Nothing And _Values(i) Is Nothing) Then
                        _isChanged = True
                    End If
                Else

                    Call CoreMessageHandler(message:="Index of " & index & " is out of bound of", _
                                          procedure:="ormRecord.setValue", argument:=value, entryname:=index, messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If

                Return True


            Catch ex As Exception
                Call CoreMessageHandler(procedure:="ormRecord.setValue", exception:=ex)
                Return False
            End Try


        End Function
        ''' <summary>
        ''' returns True if the indexed entry in the record is null or doesnot exist
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsNull(index As Object) As Boolean
            Dim nullvalue As Boolean
            Dim notfound As Boolean
            If Not Me.HasIndex(index:=index) Then Return False
            Dim avalue As Object = Me.GetValue(index:=index, isNull:=nullvalue, notFound:=notfound)
            Return nullvalue
        End Function
        ''' <summary>
        ''' gets the Value of an Entry of the Record
        ''' </summary>
        ''' <param name="anIndex">Index 1...n or name of the Field</param>
        ''' <returns>the value as object or Null of not found</returns>
        ''' <remarks></remarks>
        Public Function GetValue(index As Object, Optional ByRef isNull As Boolean = False, Optional ByRef notFound As Boolean = False) As Object
            Dim i As Long

            Try

                ' no table ?!
                If Not Me.Alive Then
                    GetValue = False
                    Exit Function
                End If


                If IsNumeric(index) Then
                    i = CLng(index) - 1
                Else
                    i = ZeroBasedIndexOf(index)
                    If i < 0 Then
                        'CoreMessageHandler(message:="column name could not be found", arg1:=index, _
                        '                    messagetype:=otCoreMessageType.InternalWarning, subname:="ormRecord.GetValue")
                        notFound = True
                        Return Nothing  'wrong table
                    End If
                End If


                ''' Get the value
                ''' 
                If (i) >= LBound(_Values) And (i) <= UBound(_Values) Then
                    If DBNull.Value.Equals(_Values(i)) OrElse (_isnullable(i) = True AndAlso _Values(i) Is Nothing) Then
                        isNull = True
                        Return Nothing
                    Else
                        isNull = False
                        Return _Values(i)
                    End If
                Else
                    Call CoreMessageHandler(message:="Index of " & index & " is out of bound of tablestore or doesnot exist in record '", _
                                          procedure:="ormRecord.getValue", entryname:=index, messagetype:=otCoreMessageType.InternalError)
                    notFound = True
                    Return Nothing
                End If

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="ormRecord.getValue", exception:=ex)
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' extract out of a record a Primary Key array
        ''' </summary>
        ''' <param name="record"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToPrimaryKey(objectID As String,
                                     Optional runtimeOnly As Boolean = False) As ormDatabaseKey
            Dim thePrimaryKeyEntryNames As String()
            Dim pkarray As Object()
            Dim anObjectDescription As ObjectClassDescription = ot.GetObjectClassDescriptionByID(objectID)
            '*** extract the primary keys from record
            'If Not CurrentSession.IsRuntimeRepositoryAvailable Then
            '    If anObjectDescription IsNot Nothing Then
            '        thePrimaryKeyEntryNames = anObjectDescription.PrimaryKeyEntryNames
            '    Else
            '        CoreMessageHandler(message:="ObjectDescriptor not found", objectname:=objectID, argument:=objectID, _
            '                            procedure:="ormRecord.ToPrimaryKey", messagetype:=otCoreMessageType.InternalError)
            '        Return Nothing
            '    End If
            '    '* extract
            '    thePrimaryKeyEntryNames = anObjectDescription.PrimaryKeyEntryNames
            '    ReDim pkarray(thePrimaryKeyEntryNames.Length - 1)
            '    Dim i As UShort = 0
            '    For Each anEntry In anObjectDescription.PrimaryEntryAttributes
            '        If Me.HasIndex(anEntry.ContainerEntryName) Then
            '            pkarray(i) = Me.GetValue(index:=anEntry.ContainerEntryName)
            '            i += 1
            '        End If
            '    Next
            'Else
                Dim anObjectDefinition = CurrentSession.Objects.GetObjectDefinition(objectID)
                '* keynames of the object
                thePrimaryKeyEntryNames = anObjectDefinition.PrimaryKeyEntryNames
                If thePrimaryKeyEntryNames.Count = 0 Then
                    CoreMessageHandler(message:="objectdefinition has not primary keys", objectname:=anObjectDefinition.Objectname, _
                                   procedure:="ormRecord.ToPrimaryKey", messagetype:=otCoreMessageType.InternalWarning)
                    Return Nothing
                End If
                '* extract
                ReDim pkarray(thePrimaryKeyEntryNames.Length - 1)
                Dim i As UShort = 0
                For Each anEntry In anObjectDefinition.GetKeyEntries
                    If Me.HasIndex(DirectCast(anEntry, ormObjectFieldEntry).ContainerEntryName) Then
                        pkarray(i) = Me.GetValue(index:=DirectCast(anEntry, ormObjectFieldEntry).ContainerEntryName)
                        i += 1
                    End If
                Next

            'End If

            Return New ormDatabaseKey(objectid:=objectID, keyvalues:=pkarray)
        End Function
    End Class


End Namespace