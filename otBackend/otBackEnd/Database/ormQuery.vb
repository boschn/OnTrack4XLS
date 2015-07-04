REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** queried object enumeration for ORM iormPersistables 
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2014-03-14
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2014
REM ***********************************************************************************************************************************************
Option Explicit On
Imports System.Collections.Generic
Imports System.IO
Imports System.Diagnostics.Debug
Imports OnTrack.Commons
Imports OnTrack.Core

Namespace OnTrack.Database

    ''' <summary>
    ''' Query based parts of the ormDataObject
    ''' </summary>
    ''' <remarks></remarks>
    Partial Public MustInherit Class ormBusinessObject
        ''' <summary>
        ''' Returns a Query Enumeration
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetQuery(name As String) As iormQueriedEnumeration Implements iormQueriable.GetQuery
            If Me.ObjectDefinition.GetType().IsAssignableFrom(GetType(ormObjectDefinition)) Then Return CType(Me.ObjectDefinition, ormObjectDefinition).GetQuery(name:=name)
            Return Nothing
        End Function

        ''' <summary>
        ''' Static Function ALL returns a Collection of all objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function AllDataObject(Of T As {iormRelationalPersistable, New})(Optional ID As String = "All", _
        Optional domainid As String = Nothing,
        Optional where As String = Nothing, _
        Optional orderby As String = Nothing, _
        Optional parameters As List(Of ormSqlCommandParameter) = Nothing, _
        Optional deleted As Boolean = False) _
        As List(Of T)
            Dim theObjectList As New List(Of T)
            Dim aRecordCollection As New List(Of ormRecord)
            Dim aStore As iormRelationalTableStore
            Dim anObject As New T
            If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID

            '** is a session running ?!
            If Not CurrentSession.IsRunning AndAlso Not CurrentSession.IsStartingUp Then
                Call CoreMessageHandler(message:="data object cannot be retrieved - start session to database first", _
                                        objectname:=anObject.ObjectID, _
                                        procedure:="ormDataObject.All", messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If

            '** check on the operation right for this object for the current username (might be that during session startup otdb username is not set)
            If Not CurrentSession.IsStartingUp AndAlso Not ot.GetBootStrapObjectClassIDs.Contains(anObject.ObjectID) _
            AndAlso Not CurrentSession.ValidateAccessRights(accessrequest:=otAccessRight.ReadOnly, domainid:=domainid, _
                                                            objecttransactions:={anObject.ObjectID & "." & ConstOPInject}) Then
                '** request authorizartion
                If Not CurrentSession.RequestUserAccess(accessRequest:=otAccessRight.ReadOnly, domainid:=domainid, _
                                                        username:=CurrentSession.CurrentUsername, _
                                                        objecttransactions:={anObject.ObjectID & "." & ConstOPInject}) Then
                    Call CoreMessageHandler(message:="data object cannot be retrieved - permission denied to user", _
                                            objectname:=anObject.ObjectID, argument:=ConstOPInject, username:=CurrentSession.CurrentUsername, _
                                            procedure:="ormDataObject.Retrieve", messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If
            End If

            Try
                aStore = anObject.ObjectPrimaryTableStore
                If parameters Is Nothing Then
                    parameters = New List(Of ormSqlCommandParameter)
                End If
                ''' build domain behavior and deleteflag
                ''' 
                If anObject.ObjectHasDomainBehavior Then
                    If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
                    ''' add where
                    If Not String.IsNullOrWhiteSpace(where) Then where &= " AND "
                    where &= String.Format(" ([{0}] = @{0} OR [{0}] = @Global{0})", ConstFNDomainID)
                    ''' add parameters
                    If parameters.Find(Function(x)
                                           Return x.ID.ToUpper = "@" & ConstFNDomainID.ToUpper
                                       End Function) Is Nothing Then
                        parameters.Add(New ormSqlCommandParameter(id:="@" & ConstFNDomainID, columnname:=ConstFNDomainID, _
                                                                  tableid:=anObject.ObjectPrimaryTableID, value:=domainid)
                        )
                    End If
                    If parameters.Find(Function(x)
                                           Return x.ID.ToUpper = "@Global" & ConstFNDomainID.ToUpper
                                       End Function
                    ) Is Nothing Then
                        parameters.Add(New ormSqlCommandParameter(id:="@Global" & ConstFNDomainID, columnname:=ConstFNDomainID, _
                                                                  tableid:=anObject.ObjectPrimaryTableID, value:=ConstGlobalDomain)
                        )
                    End If
                End If
                ''' delete 
                ''' 
                If anObject.ObjectHasDeletePerFlagBehavior Then
                    If Not String.IsNullOrWhiteSpace(where) Then where &= " AND "
                    where &= String.Format(" [{0}] = @{0}", ConstFNIsDeleted)
                    If parameters.Find(Function(x)
                                           Return x.ID.ToUpper = "@" & ConstFNIsDeleted.ToUpper
                                       End Function
                    ) Is Nothing Then

                        parameters.Add(New ormSqlCommandParameter(id:="@" & ConstFNIsDeleted, columnname:=ConstFNIsDeleted, tableid:=anObject.ObjectPrimaryTableID, value:=deleted)
                        )
                    End If
                End If

                ''' get the records
                aRecordCollection = aStore.GetRecordsBySqlCommand(id:=ID, wherestr:=where, orderby:=orderby, parameters:=parameters)
                If aRecordCollection Is Nothing Then
                    CoreMessageHandler(message:="no records returned due to previous errors", procedure:="ormDataObject.AllDataObject", argument:=ID, _
                                       objectname:=anObject.ObjectID, containerID:=anObject.ObjectPrimaryTableID, messagetype:=otCoreMessageType.InternalError)
                    Return theObjectList
                End If
                Dim aDomainRecordCollection As New Dictionary(Of String, ormRecord)
                Dim pknames = aStore.ContainerSchema.PrimaryEntryNames
                Dim domainBehavior As Boolean = False

                If anObject.ObjectHasDomainBehavior And domainid <> ConstGlobalDomain Then
                    domainBehavior = True
                End If
                '*** phase I: get all records and store either the currentdomain or the globaldomain if on domain behavior
                '***
                For Each aRecord As ormRecord In aRecordCollection

                    ''' domain behavior and not on global domain
                    ''' 
                    If domainBehavior Then
                        '** build pk key
                        Dim pk As String = String.Empty
                        For Each acolumnname In pknames
                            If acolumnname <> ConstFNDomainID Then pk &= aRecord.GetValue(index:=acolumnname).ToString & ConstDelimiter
                        Next
                        If aDomainRecordCollection.ContainsKey(pk) Then
                            Dim anotherRecord = aDomainRecordCollection.Item(pk)
                            If anotherRecord.GetValue(ConstFNDomainID).ToString = ConstGlobalDomain Then
                                aDomainRecordCollection.Remove(pk)
                                aDomainRecordCollection.Add(key:=pk, value:=aRecord)
                            End If
                        Else
                            aDomainRecordCollection.Add(key:=pk, value:=aRecord)
                        End If
                    Else
                        ''' just build the list
                        Dim atargetobject As New T
                        If InfuseDataObject(record:=aRecord, dataobject:=atargetobject, mode:=otInfuseMode.OnInject Or otInfuseMode.OnDefault) Then
                            theObjectList.Add(atargetobject)
                        End If
                    End If
                Next

                '** phase II: if on domainbehavior then get the objects out of the active domain entries
                '**
                If domainBehavior Then
                    For Each aRecord In aDomainRecordCollection.Values
                        Dim atargetobject As New T
                        If ormBusinessObject.InfuseDataObject(record:=aRecord, dataobject:=TryCast(atargetobject, iormInfusable), _
                                                          mode:=otInfuseMode.OnInject Or otInfuseMode.OnDefault) Then
                            theObjectList.Add(DirectCast(atargetobject, iormRelationalPersistable))
                        End If
                    Next
                End If

                ''' return the ObjectsList
                Return theObjectList

            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="ormDataObject.All(of T)")
                Return theObjectList
            End Try


        End Function
    End Class
    ''' <summary>
    ''' Enumerator for QueryEnumeration
    ''' </summary>
    ''' <remarks></remarks>

    Public Class ormQueriedEnumerator
        Implements IEnumerator
        Implements IDisposable

        Private _queriedEnumeration As iormQueriedEnumeration
        Private _counter As Long = -1

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="qry"></param>
        ''' <remarks></remarks>
        Public Sub New(qry As iormQueriedEnumeration)
            _queriedEnumeration = qry
        End Sub
        ''' <summary>
        ''' returns the Current object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Current As Object Implements IEnumerator.Current
            Get
                If _counter >= 0 And _counter < _queriedEnumeration.Count Then Return _queriedEnumeration.GetObject(_counter)
                ' throw else
                Throw New InvalidOperationException()
            End Get
        End Property
        ''' <summary>
        ''' Move to next object
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            _counter += 1
            Return (_counter < _queriedEnumeration.Count)
            ' throw else
            Throw New InvalidOperationException()
        End Function
        ''' <summary>
        ''' reset
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Reset() Implements IEnumerator.Reset
            _queriedEnumeration.Reset()
            _counter = -1
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

    ''' <summary>
    ''' Enumerator for Queried Data Object Enumeration
    ''' </summary>
    ''' <remarks></remarks>

    Public Class ormQueriedDataObjectEnumerator(Of T As {New, ormBusinessObject})
        Implements IEnumerator
        Implements IDisposable

        Private _queriedEnumeration As ormDataObjectEnumeration(Of T)
        Private _counter As Long = -1

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="qry"></param>
        ''' <remarks></remarks>
        Public Sub New(qry As ormDataObjectEnumeration(Of T))
            _queriedEnumeration = qry
        End Sub
        ''' <summary>
        ''' returns the Current object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Current As Object Implements IEnumerator.Current
            Get
                If _counter >= 0 And _counter < _queriedEnumeration.Count Then Return _queriedEnumeration.GetObject(_counter)
                ' throw else
                Throw New InvalidOperationException()
            End Get
        End Property
        ''' <summary>
        ''' Move to next object
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            _counter += 1
            Return (_counter < _queriedEnumeration.Count)
            ' throw else
            Throw New InvalidOperationException()
        End Function
        ''' <summary>
        ''' reset
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Reset() Implements IEnumerator.Reset
            _queriedEnumeration.Reset()
            _counter = -1
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class


    ''' <summary>
    ''' defines an enumeration of data object of a specific class
    ''' </summary>
    ''' <remarks>
    ''' functional design principles
    ''' 1) this loads all data objects of a certain type -> use linq to select the wanted ones
    ''' 2) retrieved by sql queried are only the primary keys + additional columns -> defered loading of the data object
    ''' 3) objectentrynames are refering to column names to be retrieved in the record (from the sql query) and not from the object
    ''' 4) the record returned and stored are the sql query record -> for visualization before infusing a data object
    ''' 5) each data object has an ordial in the enumeration -> the ordial is kept even if the data object is removed from the list (empty bucket)
    ''' 6) load is expected to be carried out implicit by invoking a property or function whihc is using or processing the result
    ''' 7) the id should be unique for sql query reusage 
    ''' </remarks>

    Public Class ormDataObjectEnumeration(Of T As {New, ormBusinessObject})
        Implements iormQueriedEnumeration

        Private _objectid As String
        Private _objecttype As Type
        Private _isloaded As Boolean = False
        Private _objectCollection As New Dictionary(Of ormDatabaseKey, T) ''' save all keys to data objects retrieved
        Private _orderedlist As New SortedList(Of UInt64, ormDatabaseKey) ''' ordials with keys
        Private _recordCollection As New Dictionary(Of UInt64, ormRecord) ''' dictionary with the records
        Private _id As String
        Private _entrynames As New SortedList(Of UInt16, String) ''' list of entrynames
        Private _selectCommand As ormSqlSelectCommand
        Private _queryAttributeName As String 'if Enumeration Definition is loaded from a build-in Attribute

        ''' <summary>
        '''  events
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Event OnAdded(sender As Object, e As System.EventArgs) Implements iormQueriedEnumeration.OnAdded
        Public Event OnLoaded(sender As Object, e As System.EventArgs) Implements iormQueriedEnumeration.OnLoaded
        Public Event OnLoading(sender As Object, e As System.EventArgs) Implements iormQueriedEnumeration.OnLoading
        Public Event OnRemoved(sender As Object, e As System.EventArgs) Implements iormQueriedEnumeration.OnRemoved

#Region "Property"

        ''' <summary>
        ''' returns the loaded relational collection as list
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property AsList As List(Of T)
            Get
                If _objectCollection IsNot Nothing Then Return _objectCollection.Values.ToList
                Return Nothing
            End Get
        End Property
        ''' <summary>
        ''' return ID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ID As String Implements iormQueriedEnumeration.ID
            Get
                Return _id
            End Get
        End Property
        ''' <summary>
        ''' return the object entry names - check if column entry and existing
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ObjectEntryNames As IList(Of String) Implements iormQueriedEnumeration.ObjectEntryNames
            Get
                Return _entrynames.Values
            End Get
            Set(value As IList(Of String))
                Dim aList As New SortedList(Of UInt16, String)
                Dim ordinal As UInt16 = 0
                For Each aName In value
                    If CurrentSession.Objects.GetObjectDefinition(id:=_objectid).HasEntry(entryname:=aName) Then
                        If CurrentSession.Objects.GetObjectDefinition(id:=_objectid).GetEntryDefinition(entryname:=aName).IsContainer Then
                            aList.Add(ordinal, aName.ToUpper)
                            ordinal += 1
                        Else
                            CoreMessageHandler(message:="name '" & aName & "' is not a column entry name for object type '" & _objectid & "'", _
                                           messagetype:=otCoreMessageType.InternalError, procedure:="ormDataObjectEnumeration.ObjectEntryNames#Set")
                        End If

                    Else
                        CoreMessageHandler(message:="name '" & aName & "' is not a valid object entry name for object type '" & _objectid & "'", _
                                            messagetype:=otCoreMessageType.InternalError, procedure:="ormDataObjectEnumeration.ObjectEntryNames#Set")
                    End If
                Next
                _entrynames = aList
            End Set
        End Property
        ''' <summary>
        ''' return the ObjectDefinition of the returned data object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectDefinition As ormObjectDefinition
            Get
                Return CurrentSession.Objects.GetObjectDefinition(id:=_objectid)
            End Get
        End Property
        ''' <summary>
        ''' gets thhe AreObjectEnumerated Flag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AreObjectsEnumerated As Object Implements iormQueriedEnumeration.AreObjectsEnumerated
            Get
                Return True
            End Get
            Set(value As Object)
                Throw New NotImplementedException("setting ormDataObjectEnumeration.AreObjectsEnumerated not permitted")
            End Set
        End Property
        ''' <summary>
        ''' set the Build-In QueryAttribute Name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property QueryAttribute As String
            Get
                Return _queryAttributeName
            End Get
            Set(value As String)
                If Not _isloaded Then _queryAttributeName = value
            End Set
        End Property
        ''' <summary>
        ''' return true if the Enumeration is loaded
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsLoaded As Boolean Implements iormQueriedEnumeration.IsLoaded
            Get
                Return _isloaded
            End Get
        End Property
        ''' <summary>
        ''' returns true if the  data object defininition has Domain behavior
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectHasDomainBehavior As Boolean
            Get
                Return Me.ObjectDefinition.HasDomainBehavior
            End Get
        End Property
        ''' <summary>
        ''' returns true if the  data object defininition has Domain behavior
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectHasDeleteBehavior As Boolean
            Get
                Return Me.ObjectDefinition.HasDeleteFieldBehavior
            End Get
        End Property
        ''' <summary>
        ''' returns all Records
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Records As IList(Of ormRecord)
            Get
                Return _recordCollection.Values.ToList
            End Get
        End Property
#End Region
        ''' <summary>
        ''' Constructor - provide an optional unique id for the query (reuse)
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(Optional id As String = Nothing)
            Dim aDescription = ot.GetObjectClassDescription(GetType(T))
            If aDescription Is Nothing Then
                Throw New ormException(message:="The supplied type '" & GetType(T).Name & "' has not been found in the Class Repository ")
            Else
                _objectid = aDescription.ObjectAttribute.ID
                _objecttype = GetType(T)
                Dim aList As New SortedList(Of UInt16, String)
                Dim i As Integer = 1
                For Each anEntry In CurrentSession.Objects.GetObjectDefinition(id:=_objectid).PrimaryKeyEntryNames
                    aList.Add(i, anEntry)
                    i += 1
                Next
                _entrynames = aList 'set entrynames to the primary keys
                _id = "ormDataObjectEnumeration_" & _objectid & "_" & id
            End If

        End Sub

        ''' <summary>
        ''' Add an Object to the enumeration
        ''' </summary>
        ''' <param name="dataobject"></param>
        ''' <param name="no"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddObject(dataobject As iormRelationalPersistable, Optional ByRef no As ULong? = Nothing) As Boolean Implements iormQueriedEnumeration.AddObject
            If Not Me.IsLoaded AndAlso Not Me.Load() Then
                CoreMessageHandler(message:="failed to run query", procedure:="ormDataObjectEnumeration.AddObject", messagetype:=otCoreMessageType.InternalError)
                Return False
            End If
            If dataobject.GetType() Is GetType(T) Then
                Dim aKey As ormDatabaseKey = dataobject.ObjectPrimaryKey
                If Not _objectCollection.ContainsKey(key:=aKey) Then
                    Dim ordial As Long = 0
                    If _orderedlist.Count > 0 Then ordial = _orderedlist.Keys.Max + 1
                    _orderedlist.Add(key:=ordial, value:=aKey)
                    _objectCollection.Add(key:=aKey, value:=dataobject)
                    _recordCollection.Add(key:=ordial, value:=dataobject.Record)
                    RaiseEvent OnAdded(Me, New System.EventArgs())
                    Return True
                End If

                Return False
            Else
                CoreMessageHandler(message:="data object is of wrong type ('" & dataobject.GetType().Name & "') must be of type '" & GetType(T).Name & "'", _
                                   messagetype:=otCoreMessageType.InternalError, procedure:="ormDataObjectEnumeration.AddObject")
                Return False
            End If
        End Function

        ''' <summary>
        ''' returns the number of elements
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Count As ULong Implements iormQueriedEnumeration.Count
            Get
                If Not Me.IsLoaded AndAlso Not Me.Load() Then
                    CoreMessageHandler(message:="failed to run query", procedure:="ormDataObjectEnumeration.Count", messagetype:=otCoreMessageType.InternalError)
                    Return 0
                End If
                If _orderedlist.Count > 0 Then Return _orderedlist.Keys.Max + 1
                Return 0
            End Get
        End Property
        ''' <summary>
        ''' returns the zero-based number of object
        ''' </summary>
        ''' <param name="no"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObject(no As ULong) As iormDataObject Implements iormQueriedEnumeration.GetObject
            If Not Me.IsLoaded AndAlso Not Me.Load() Then
                CoreMessageHandler(message:="failed to run query", procedure:="ormDataObjectEnumeration.GetObject", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If

            If no >= 0 And no < Me.Count Then
                Dim aKey As ormDatabaseKey = _orderedlist.Item(no)
                Dim aObject As T = _objectCollection.Item(key:=aKey)
                If aObject Is Nothing Then
                    aObject = ormBusinessObject.RetrieveDataObject(Of T)(key:=aKey)
                    If aObject IsNot Nothing Then
                        ''' add object
                        _objectCollection.Remove(key:=aKey)
                        _objectCollection.Add(key:=aKey, value:=aObject)
                        Return aObject
                    End If
                Else
                    Return aObject
                End If
                Return Nothing
            Else
                Throw New IndexOutOfRangeException("index " & no & " must be between 0 and " & Me.Count)
            End If
        End Function
        ''' <summary>
        ''' returns the class definition of the data object
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectClassDescription() As ObjectClassDescription Implements iormQueriedEnumeration.GetObjectClassDescription
            Return ot.GetObjectClassDescription(GetType(T))
        End Function
        ''' <summary>
        ''' returns the objectdefinition
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectDefinition() As ormObjectDefinition Implements iormQueriedEnumeration.GetObjectDefinition
            Return ot.CurrentSession.Objects.GetObjectDefinition(id:=_objectid)
        End Function
        ''' <summary>
        ''' return ObjectEntries Enumeration of the defined columns 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectEntries() As IList(Of iormObjectEntryDefinition) Implements iormQueriedEnumeration.GetObjectEntries
            Dim newList As New List(Of iormObjectEntryDefinition)
            For Each anEntryname In _entrynames
                If CurrentSession.Objects.GetObjectDefinition(id:=_objectid).HasEntry(anEntryname.Value) Then
                    Dim anObjectEntry As iormObjectEntryDefinition = CurrentSession.Objects.GetObjectDefinition(id:=_objectid).GetEntryDefinition(anEntryname.Value)
                    If anObjectEntry IsNot Nothing AndAlso anObjectEntry.IsActive Then newList.Add(anObjectEntry)
                Else
                    CoreMessageHandler(message:="entry name '" & anEntryname.Value & "' not valid for data object '" & _objectid & "'", _
                                       messagetype:=otCoreMessageType.InternalError, procedure:="ormDataObjectEnumeration.GetObjectEntries")
                End If
            Next
            Return newList.OrderBy(Function(X) X.Ordinal).ToList
        End Function

        ''' <summary>
        ''' returns object entry by name - returns nothing if not in enumeration definition
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectEntry(name As String) As iormObjectEntryDefinition Implements iormQueriedEnumeration.GetObjectEntry
            If _entrynames.Values.Contains(name.ToUpper) Then
                Return CurrentSession.Objects.GetObjectDefinition(id:=_objectid).GetEntryDefinition(entryname:=name)
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' get the record of the zero-based index
        ''' </summary>
        ''' <param name="no"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetRecord(no As ULong) As ormRecord Implements iormQueriedEnumeration.GetRecord
            If Not Me.IsLoaded AndAlso Not Me.Load() Then
                CoreMessageHandler(message:="failed to run query", procedure:="ormDataObjectEnumeration.GetRecord", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If
            If _recordCollection.ContainsKey(no) Then Return _recordCollection.Item(no)
            Return Nothing
        End Function

        ''' <summary>
        ''' get parameter value 
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetValue(name As String, ByRef value As Object) As Boolean Implements iormQueriedEnumeration.GetValue
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' build the to be used select command from a QryAttribute
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function BuildSelectCommand(Optional domainid As String = Nothing) As ormSqlSelectCommand
            Dim aDescription As ObjectClassDescription = ot.GetObjectClassDescriptionByID(_objectid)
            If aDescription Is Nothing Then
                CoreMessageHandler(message:="class description for class of" & _objecttype.FullName & " could not be retrieved", _
                                   procedure:="ormDataObjectEnumeration.BuildSelectCommand", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If
            Dim aTargetType As System.Type = aDescription.Type

            If String.IsNullOrWhiteSpace(domainid) Then domainid = CurrentSession.CurrentDomainID
            Dim primaryTableID As String = aDescription.PrimaryContainerID  ' First Tablename if multiple
            Dim aCurrentRelationalDBDriver As iormRelationalDatabaseDriver = TryCast(ot.CurrentSession.OTDBDriver, iormRelationalDatabaseDriver)
            If aCurrentRelationalDBDriver Is Nothing Then
                CoreMessageHandler(message:="current db driver of the session is not a relational driver", _
                                   procedure:="ormDataObjectEnumeration.BuildSelectCommand", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If
            '***
            Try

                '** get a Store
                Dim aStore As iormRelationalTableStore = aCurrentRelationalDBDriver.GetTableStore(primaryTableID)
                If aStore Is Nothing Then
                    CoreMessageHandler(message:="store of table '" & primaryTableID & "' is not in the primary driver of the session", _
                                   procedure:="ormDataObjectEnumeration.BuildSelectCommand", messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If
                Dim aSelectCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(id:=_id, addAllFields:=False)
                Dim where As String = String.Empty
                Dim [select] As String = String.Empty
                Dim orderby As String = String.Empty

                If Not aSelectCommand.IsPrepared Then

                    ' build the key part
                    For i = 0 To aDescription.PrimaryKeyEntryNames.Count - 1
                        [select] &= "[" & CurrentOTDBDriver.GetNativeDBObjectName(primaryTableID) & "].[" & aDescription.PrimaryKeyEntryNames(i).ToUpper & "]"
                        If i > 0 Then [select] &= " , "
                    Next
                    ''' extend by the entrynames
                    For Each aName In _entrynames.Values
                        If Not aDescription.PrimaryKeyEntryNames.Contains(aName) Then
                            If aName.Contains(".") Then
                                [select] &= ",[ " & aName & "]"
                            Else
                                [select] &= ", [" & CurrentOTDBDriver.GetNativeDBObjectName(primaryTableID) & "].[" & aName & "]"
                            End If
                        End If
                    Next

                    '** additional behavior
                    ''' build domain behavior and deleteflag
                    ''' 
                    If Me.ObjectHasDomainBehavior Then
                        ''' add domainid
                        ''' 
                        If Not aDescription.PrimaryKeyEntryNames.Contains(Domain.ConstFNDomainID.ToUpper) AndAlso _
                            Not _entrynames.Values.Contains(Domain.ConstFNDomainID.ToUpper) Then
                            [select] &= ", [" & CurrentOTDBDriver.GetNativeDBObjectName(primaryTableID) & "].[" & Domain.ConstFNDomainID.ToUpper & "]"
                        End If


                        ''' add where
                        If Not String.IsNullOrWhiteSpace(where) Then where &= " AND "
                        where &= String.Format(" ([{0}] = @{0} OR [{0}] = @Global{0})", Domain.ConstFNDomainID)
                        ''' add parameters
                        If aSelectCommand.Parameters.Find(Function(x)
                                                              Return x.ID.ToUpper = "@" & Domain.ConstFNDomainID.ToUpper
                                                          End Function) Is Nothing Then
                            aSelectCommand.AddParameter(New ormSqlCommandParameter(id:="@" & Domain.ConstFNDomainID, columnname:=Domain.ConstFNDomainID, _
                                                                                   tableid:=primaryTableID, value:=domainid)
                            )
                        End If
                        If aSelectCommand.Parameters.Find(Function(x)
                                                              Return x.ID.ToUpper = "@Global" & Domain.ConstFNDomainID.ToUpper
                                                          End Function
                        ) Is Nothing Then
                            aSelectCommand.AddParameter(New ormSqlCommandParameter(id:="@Global" & Domain.ConstFNDomainID, columnname:=Domain.ConstFNDomainID, _
                                                                                   tableid:=primaryTableID, value:=ConstGlobalDomain)
                            )
                        End If
                    End If
                    ''' delete 
                    ''' 
                    If Me.ObjectHasDeleteBehavior Then
                        ''' add deleteflag
                        If Not aDescription.PrimaryKeyEntryNames.Contains(ormBusinessObject.ConstFNIsDeleted.ToUpper) AndAlso _
                          Not _entrynames.Values.Contains(ormBusinessObject.ConstFNIsDeleted.ToUpper) Then
                            [select] &= ", [" & CurrentOTDBDriver.GetNativeDBObjectName(primaryTableID) & "].[" & ormBusinessObject.ConstFNIsDeleted.ToUpper & "]"
                        End If

                        '' add
                        If Not String.IsNullOrWhiteSpace(where) Then where &= " AND "
                        where &= String.Format(" [{0}] = @{0}", ConstFNIsDeleted)
                        If aSelectCommand.Parameters.Find(Function(x)
                                                              Return x.ID.ToUpper = "@" & ConstFNIsDeleted.ToUpper
                                                          End Function
                        ) Is Nothing Then

                            aSelectCommand.AddParameter(New ormSqlCommandParameter(id:="@" & ConstFNIsDeleted, columnname:=ConstFNIsDeleted, tableid:=primaryTableID, _
                                                                                   value:=False)
                            )
                        End If
                    End If
                    '' set sql parameters
                    aSelectCommand.Where = where
                    aSelectCommand.select = [select]
                    aSelectCommand.OrderBy = orderby
                End If

                'return finally
                Return aSelectCommand

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, _
                                    objectname:=_objectid, _
                                     procedure:="ormDataObjectEnumeration.BuildSelectCommand")
                Return Nothing
            End Try



        End Function
        ''' <summary>
        ''' build the to be used select command from a QryAttribute
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function BuildSelectCommandFromQryAttribute(Optional domainid As String = Nothing) As ormSqlSelectCommand
            Dim aDescription = ot.GetObjectClassDescriptionByID(_objectid)
            Dim aQryAttribute As ormObjectQueryAttribute = aDescription.GetQueryAttribute(name:=_id)
            Dim primaryTableID As String = aDescription.PrimaryContainerID
            Dim where As String
            Dim orderby As String
            Dim fieldnames As New List(Of String)
            Dim addallfields As Boolean


            Try
                If aQryAttribute Is Nothing Then
                    Call CoreMessageHandler(message:="query attribute could not be retrieved", _
                                            objectname:=_objectid, _
                                            procedure:="ormDataObjectEnumeration.BuildSelectCommandFromQryAttribute", messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If

                If aQryAttribute.HasValueWhere Then
                    where = aQryAttribute.Where
                Else
                    where = String.Empty
                End If
                If aQryAttribute.HasValueOrderBy Then
                    orderby = aQryAttribute.Orderby
                Else
                    orderby = String.Empty
                End If
                If aQryAttribute.HasValueAddAllFields Then addallfields = aQryAttribute.AddAllFields
                If aQryAttribute.HasValueEntrynames Then
                    Call CoreMessageHandler(message:="retrieving entry names not yet implemented", _
                                            objectname:=_objectid, argument:=_id, _
                                            procedure:="ormDataObjectEnumeration.BuildSelectCommandFromQryAttribute", messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If
                '** get the store for the primary table 
                Dim aStore = TryCast(CurrentOTDBDriver.RetrieveContainerStore(containerid:=Me.ObjectDefinition.PrimaryContainerID), iormRelationalTableStore)
                If aStore Is Nothing Then
                    Call CoreMessageHandler(message:="table store cannot be retrieved", _
                                            objectname:=_objectid, containerID:=aDescription.PrimaryContainerID, _
                                            procedure:="ormDataObjectEnumeration.GetQuery", messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If

                ''' get the Select-Command
                Dim aSelectCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(_id)

                ''' add tables
                ''' 
                aSelectCommand.AddTable(primaryTableID, addAllFields:=addallfields)

                ''' build domain behavior and deleteflag
                ''' 
                If Me.ObjectHasDomainBehavior Then
                    ''' add where
                    If Not String.IsNullOrWhiteSpace(where) Then where &= " AND "
                    where &= String.Format(" ([{0}] = @{0} OR [{0}] = @Global{0})", Domain.ConstFNDomainID)
                    ''' add parameters
                    If aSelectCommand.Parameters.Find(Function(x)
                                                          Return x.ID.ToUpper = "@" & Domain.ConstFNDomainID.ToUpper
                                                      End Function) Is Nothing Then
                        aSelectCommand.AddParameter(New ormSqlCommandParameter(id:="@" & Domain.ConstFNDomainID, columnname:=Domain.ConstFNDomainID, _
                                                                               tableid:=primaryTableID, value:=domainid)
                        )
                    End If
                    If aSelectCommand.Parameters.Find(Function(x)
                                                          Return x.ID.ToUpper = "@Global" & Domain.ConstFNDomainID.ToUpper
                                                      End Function
                    ) Is Nothing Then
                        aSelectCommand.AddParameter(New ormSqlCommandParameter(id:="@Global" & Domain.ConstFNDomainID, columnname:=Domain.ConstFNDomainID, _
                                                                               tableid:=primaryTableID, value:=ConstGlobalDomain)
                        )
                    End If
                End If
                ''' delete 
                ''' 
                If Me.ObjectHasDeleteBehavior Then
                    If Not String.IsNullOrWhiteSpace(where) Then where &= " AND "
                    where &= String.Format(" [{0}] = @{0}", ConstFNIsDeleted)
                    If aSelectCommand.Parameters.Find(Function(x)
                                                          Return x.ID.ToUpper = "@" & ConstFNIsDeleted.ToUpper
                                                      End Function
                    ) Is Nothing Then

                        aSelectCommand.AddParameter(New ormSqlCommandParameter(id:="@" & ConstFNIsDeleted, columnname:=ConstFNIsDeleted, tableid:=primaryTableID, _
                                                                               value:=False)
                        )
                    End If
                End If

                ''' set the parameters
                aSelectCommand.Where = where
                aSelectCommand.OrderBy = orderby

                Return aSelectCommand

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ormDataObjectEnumeration.BuildSelectCommandFromQryAttribute")
                Return Nothing
            End Try
        End Function
        ''' <summary>
        ''' Load all data objects
        ''' </summary>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Function Load(Optional domainid As String = Nothing) As Boolean Implements iormQueriedEnumeration.Load
            If Not Me.IsLoaded Then

                '** DOMAIN ID
                If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID

                '** check on the operation right for this object for the current username (might be that during session startup otdb username is not set)
                If Not CurrentSession.IsStartingUp AndAlso Not ot.GetBootStrapObjectClassIDs.Contains(_objectid) _
                AndAlso Not CurrentSession.ValidateAccessRights(accessrequest:=otAccessRight.ReadOnly, domainid:=domainid, _
                                                                objecttransactions:={_objectid & "." & OnTrack.Database.ormBusinessObject.ConstOPInject}) Then
                    '** request authorizartion
                    If Not CurrentSession.RequestUserAccess(accessRequest:=otAccessRight.ReadOnly, domainid:=domainid, _
                                                            username:=CurrentSession.CurrentUsername, _
                                                            objecttransactions:={_objectid & "." & OnTrack.Database.ormBusinessObject.ConstOPInject}) Then
                        Call CoreMessageHandler(message:="data object cannot be retrieved - permission denied to user", _
                                                objectname:=_objectid, argument:=OnTrack.Database.ormBusinessObject.ConstOPInject, username:=CurrentSession.CurrentUsername, _
                                                procedure:="ormDataObjectEnumeration.GetQuery", messagetype:=otCoreMessageType.ApplicationError)
                        Return Nothing
                    End If
                End If

                '''
                ''' build the select command
                ''' 
                If _selectCommand Is Nothing OrElse Not _selectCommand.IsPrepared Then
                    ''' build it
                    If Not String.IsNullOrWhiteSpace(_queryAttributeName) Then
                        _selectCommand = Me.BuildSelectCommandFromQryAttribute(domainid:=domainid)
                        If _selectCommand Is Nothing Then
                            CoreMessageHandler(message:="select command couldnot be build from attribute '" & _queryAttributeName & "'", _
                                                procedure:="ormDataObjectEnumeration.Load")
                            Return False
                        End If
                    Else
                        _selectCommand = Me.BuildSelectCommand(domainid:=domainid)
                        If _selectCommand Is Nothing Then
                            CoreMessageHandler(message:="select command could not be build ", _
                                                procedure:="ormDataObjectEnumeration.Load")
                            Return False
                        End If
                    End If

                    If Not _selectCommand.Prepare Then
                        CoreMessageHandler(message:="could not prepare select command for object type '" & _objectid & "'", _
                                            messagetype:=otCoreMessageType.InternalError, objectname:=_objectid, argument:=_id)
                        Return False
                    End If
                End If

                '** parameters
                If _selectCommand.Parameters.Find(Function(x)
                                                      Return x.ID.ToUpper = "@" & Domain.ConstFNDomainID.ToUpper
                                                  End Function) IsNot Nothing Then
                    _selectCommand.SetParameterValue(ID:="@" & Domain.ConstFNDomainID, value:=domainid)
                End If
                If _selectCommand.Parameters.Find(Function(x)
                                                      Return x.ID.ToUpper = "@Global" & Domain.ConstFNDomainID.ToUpper
                                                  End Function
                ) IsNot Nothing Then
                    _selectCommand.SetParameterValue(ID:="@Global" & Domain.ConstFNDomainID.ToUpper, value:=ConstGlobalDomain)
                End If
                If _selectCommand.Parameters.Find(Function(x)
                                                      Return x.ID.ToUpper = "@" & ConstFNIsDeleted.ToUpper
                                                  End Function
                   ) IsNot Nothing Then
                    _selectCommand.SetParameterValue(ID:="@" & ConstFNIsDeleted.ToUpper, value:=False)
                End If

                ''' 
                ''' run
                ''' 
                RaiseEvent OnLoading(Me, New System.EventArgs())

                Dim aRecordCollection As List(Of ormRecord) = _selectCommand.RunSelect
                Dim ordial As Long = 0

                For Each aRecord As ormRecord In aRecordCollection
                    ''' build a primary key
                    Dim aPrimaryKey As New ormDatabaseKey(objectid:=_objectid)
                    For Each acolumnname In aPrimaryKey.EntryNames
                        aPrimaryKey.Item(acolumnname) = aRecord.GetValue(index:=acolumnname)
                    Next

                    ''' check if the global domain is already in the keylist
                    If Me.ObjectHasDomainBehavior And domainid <> ConstGlobalDomain Then
                        Dim aGlobalKey As ormDatabaseKey = aPrimaryKey.Clone
                        aGlobalKey.SubstituteDomainID(domainid:=ConstGlobalDomain)
                        If _objectCollection.ContainsKey(aGlobalKey) Then
                            _objectCollection.Remove(aGlobalKey)
                        End If
                    End If

                    '** add
                    If Not _objectCollection.ContainsKey(key:=aPrimaryKey) Then
                        If _orderedlist.ContainsKey(key:=ordial) Then _orderedlist.Remove(key:=ordial)
                        _orderedlist.Add(key:=ordial, value:=aPrimaryKey)
                        _objectCollection.Add(key:=aPrimaryKey, value:=Nothing)
                        _recordCollection.Add(key:=ordial, value:=aRecord)
                        ordial = _orderedlist.Keys.Max + 1
                    End If


                Next

                _isloaded = True
                RaiseEvent OnLoaded(Me, New System.EventArgs())
                Return True
            End If
        End Function

        ''' <summary>
        ''' remove Data Object from the Enumeration
        ''' </summary>
        ''' <param name="no"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RemoveObject(no As ULong) As Boolean Implements iormQueriedEnumeration.RemoveObject
            If Not Me.IsLoaded AndAlso Not Me.Load() Then
                CoreMessageHandler(message:="failed to run query", procedure:="ormDataObjectEnumeration.RemoveObject", messagetype:=otCoreMessageType.InternalError)
                Return False
            End If
            If no >= 0 AndAlso no < Me.Count Then
                If _orderedlist.ContainsKey(no) Then
                    Dim aKey = _orderedlist.Item(no)
                    If _objectCollection.ContainsKey(key:=aKey) Then _objectCollection.Remove(key:=aKey)
                    If _recordCollection.ContainsKey(no) Then _recordCollection.Remove(no)
                    _orderedlist.Remove(no)
                    RaiseEvent OnRemoved(Me, New System.EventArgs())
                    Return True
                End If
            End If
            Return False
        End Function
        ''' <summary>
        ''' reset the Enumeration
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Reset() As Boolean Implements iormQueriedEnumeration.Reset
            ''' reset the collection by removing
            ''' 
            _orderedlist.Clear()
            _objectCollection.Clear()
            _recordCollection.Clear()
            _isloaded = False ' unload

        End Function
        ''' <summary>
        ''' set parameter value
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetValue(name As String, value As Object) As Boolean Implements iormQueriedEnumeration.SetValue
            Throw New NotImplementedException
        End Function

        Public Function GetEnumerator() As IEnumerator(Of iormRelationalPersistable) Implements IEnumerable(Of iormRelationalPersistable).GetEnumerator
            Return New ormQueriedDataObjectEnumerator(Of T)(Me)
        End Function

        Public Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Return New ormQueriedDataObjectEnumerator(Of T)(Me)
        End Function
    End Class
    ''' <summary>
    ''' a queried enumeration object runs a query and build a enumeration of iormpersistable objects
    ''' implementation is based on a sql query which can retrieve data objects
    ''' </summary>
    ''' <remarks></remarks>

    Public Class ormSQLQueriedEnumeration
        Implements iormQueriedEnumeration

        Private _id As String
        Private _objecttype As System.Type
        Private _objectid As String
        Private _otherobjectids As New List(Of String)
        Private _objectentrienamess As New List(Of String)
        Private _objectentriesOrdinal As New Dictionary(Of UShort, String) ' dictionary of Ordinal to ObjectEntryname
        Private _select As ormSqlSelectCommand
        Private _parametervalues As New Dictionary(Of String, Object)

        Private _runTimestamp As DateTime
        Private _run As Boolean = False
        Private _records As New List(Of ormRecord)

        ''' <summary>
        '''  Parameters
        ''' </summary>
        ''' <remarks></remarks>
        Private _steps As UShort = 0
        Private _domainid As String = String.Empty
        Private _deleted As Boolean?
        Private _isObjectEnumerated = True

        Private _qrystopwatch As New Stopwatch
        Private _qryStart As DateTime
        Private _qryEnd As DateTime
        Private _qrycount As ULong

        Public Event OnLoading As iormQueriedEnumeration.OnLoadingEventHandler Implements iormQueriedEnumeration.OnLoading
        Public Event OnLoaded As iormQueriedEnumeration.OnLoadedEventHandler Implements iormQueriedEnumeration.OnLoaded
        Public Event OnAdded As iormQueriedEnumeration.OnAddedEventHandler Implements iormQueriedEnumeration.OnAdded
        Public Event OnRemoved As iormQueriedEnumeration.OnRemovedEventHandler Implements iormQueriedEnumeration.OnRemoved

        ''' <summary>
        ''' constructors
        ''' </summary>
        ''' <remarks>
        ''' set domainid if bound to a domain otherwise currentdomain
        ''' </remarks>
        Public Sub New(type As System.Type, _
        Optional id As String = Nothing, _
        Optional domainID As String = Nothing,
        Optional where As String = Nothing, _
        Optional orderby As String = Nothing, _
        Optional tablenames As String() = Nothing, _
        Optional parameters As List(Of ormSqlCommandParameter) = Nothing, _
        Optional deleted As Boolean? = Nothing)

            ''' check the id
            ''' 
            If Not String.IsNullOrWhiteSpace(id) Then
                _id = id
            Else
                _id = Guid.NewGuid.ToString
            End If

            ''' create a sql select command
            ''' 
            _select = New ormSqlSelectCommand(id)
            If String.IsNullOrEmpty(domainID) Then domainID = ConstGlobalDomain
            Me.Domainid = domainID
            Me.Where = where
            Me.Orderby = orderby
            If parameters IsNot Nothing Then Me.Parameters = parameters
            If deleted.HasValue Then Me.Deleted = deleted

            ''' set the resulted object type
            ''' 
            _isObjectEnumerated = SetObjectType(type)

            ''' Check Tablenames
            If tablenames IsNot Nothing AndAlso CheckTablenames(tablenames) Then
                Throw New ormException("instance creation error for " & _objecttype.Name & " for tables " & tablenames.ToArray.ToString)
            End If
        End Sub

        Public Sub New(type As System.Type, command As ormSqlSelectCommand, Optional id As String = Nothing)
            ''' check the id
            ''' 
            If Not String.IsNullOrWhiteSpace(id) Then
                _id = id
            Else
                _id = Guid.NewGuid.ToString
            End If

            ''' set the resulted object type
            ''' 
            _isObjectEnumerated = SetObjectType(type)

            ''' Check tablename
            ''' 
            If CheckTablenames(command.TableIDs) Then
                Throw New ormException("instance creation error for " & _objecttype.Name & " for tables " & command.TableIDs.ToArray.ToString)
            End If
            _select = command
        End Sub

#Region "Properties"


        ''' <summary>
        ''' gets or sets all object entry names of the query
        ''' </summary>
        ''' <param name="ordered"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ObjectEntryNames As IList(Of String) Implements iormQueriedEnumeration.ObjectEntryNames
            Get
                Return _objectentriesOrdinal.Values
            End Get
            Set(value As IList(Of String))
                _objectentriesOrdinal.Clear()
                _objectentrienamess.Clear()
                Dim i = 1
                For Each aName In value
                    If Not _objectentrienamess.Contains(aName) Then
                        _objectentriesOrdinal.Add(i, aName)
                        _objectentrienamess.Add(aName)
                        i += 1
                    Else
                        CoreMessageHandler(message:="entry name is not in query (" & _id & ") results entry names", argument:=aName, procedure:="ormQueriedSQLEnumeration.EntryOrder", messagetype:=otCoreMessageType.InternalError)
                    End If
                Next
            End Set
        End Property

        ''' <summary>
        ''' returns the elapsed timespan in milliseconds for the query to fetch all records
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property QryElapsedMilliseconds As Long
            Get
                If _run Then Return _qrystopwatch.ElapsedMilliseconds
                Return 0
            End Get
        End Property
        ''' <summary>
        ''' Gets the qrycount.
        ''' </summary>
        ''' <value>The qrycount.</value>
        Public ReadOnly Property Qrycount() As ULong
            Get
                Return Me._qrycount
            End Get
        End Property

        ''' <summary>
        ''' Gets the qry end.
        ''' </summary>
        ''' <value>The qry end.</value>
        Public ReadOnly Property QryEnd() As DateTime
            Get
                Return Me._qryEnd
            End Get
        End Property

        ''' <summary>
        ''' Gets the qry start.
        ''' </summary>
        ''' <value>The qry start.</value>
        Public ReadOnly Property QryStart() As DateTime
            Get
                Return Me._qryStart
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the is objects enumerated flag - true if objects are going to be returned otherwise ormRecord could be returned
        ''' </summary>
        ''' <value>The is object enumerated.</value>
        Public Property AreObjectsEnumerated() As Object Implements iormQueriedEnumeration.AreObjectsEnumerated
            Get
                Return Me._isObjectEnumerated
            End Get
            Private Set(value As Object)
                Me._isObjectEnumerated = value
            End Set
        End Property

        ''' <summary>
        ''' returns the size of the result list
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Count As ULong Implements iormQueriedEnumeration.Count
            Get
                If Not _run Then
                    If Not Me.Load() Then
                        CoreMessageHandler(message:="failed to run query", procedure:="ormQueriedSQLEnumeration.GetObject", messagetype:=otCoreMessageType.InternalError)
                        Return Nothing
                    End If
                End If
                Return _records.Count
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the steps.
        ''' </summary>
        ''' <value>The steps.</value>
        Public Property Steps() As UShort
            Get
                Return Me._steps
            End Get
            Set(value As UShort)
                Me._steps = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the deleted flag.
        ''' </summary>
        ''' <value>The deleted.</value>
        Public Property Deleted() As Boolean
            Get
                Return Me._deleted
            End Get
            Set(value As Boolean)
                Me._deleted = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the parameters as list of ormSQLCommandParameter.
        ''' </summary>
        ''' <value>The parameters.</value>
        Public Property Parameters() As List(Of ormSqlCommandParameter)
            Get
                Return Me._select.Parameters
            End Get
            Set(value As List(Of ormSqlCommandParameter))
                For Each aP In value
                    If _select.Parameters.Find(Function(x)
                                                   Return x.ID = aP.ID
                                               End Function) Is Nothing Then
                        _select.AddParameter(aP)
                    End If
                Next
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the tableids of the query.
        ''' </summary>
        ''' <value>The parameters.</value>
        Public Property Tablenames() As List(Of String)
            Get
                Return Me._select.TableIDs
            End Get
            Set(value As List(Of String))
                If value IsNot Nothing AndAlso CheckTablenames(value) Then
                    Throw New ormException("instance creation error for " & _objecttype.Name & " for tables " & value.ToArray.ToString)
                End If
                For Each aTablename In value
                    If _select.TableIDs.Contains(aTablename.ToUpper) Then
                        _select.AddTable(aTablename, addAllFields:=True)
                    End If
                Next
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the orderby part of the sql command.
        ''' </summary>
        ''' <value>The orderby.</value>
        Public Property Orderby() As String
            Get
                Return _select.OrderBy
            End Get
            Set(value As String)
                _select.OrderBy = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the where part of the sql command
        ''' </summary>
        ''' <value>The where.</value>
        Public Property Where() As String
            Get
                Return _select.Where
            End Get
            Set(value As String)
                _select.Where = Where
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the static domainid in this query.
        ''' </summary>
        ''' <value>The domainid.</value>
        Public Property Domainid() As String
            Get
                Return Me._domainid
            End Get
            Set(value As String)
                Me._domainid = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the id of this queried enumeration.
        ''' </summary>
        ''' <value>The id.</value>
        Public ReadOnly Property Id() As String Implements iormQueriedEnumeration.ID
            Get
                Return Me._id
            End Get
        End Property

        ''' <summary>
        ''' true if the query has run and a result is loaded
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property IsLoaded() As Boolean Implements iormQueriedEnumeration.IsLoaded
            Get
                Return Me._run
            End Get
        End Property
        ''' <summary>
        ''' Gets the run timestamp.
        ''' </summary>
        ''' <value>The run timestamp.</value>
        Public ReadOnly Property RunTimestamp() As DateTime
            Get
                Return Me._runTimestamp
            End Get
        End Property

#End Region


        ''' <summary>
        ''' check the tablenames
        ''' </summary>
        ''' <param name="tablenames"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CheckTablenames(tablenames As IEnumerable(Of String)) As Boolean
            Dim found As Boolean = False

            If _objecttype Is Nothing Then Return True

            ''' check the tablename
            ''' 
            If tablenames IsNot Nothing Then
                ''' check each tablename
                For Each tablename In tablenames
                    Dim theDescriptions = ot.GetObjectClassDescriptionByContainer(tablename)
                    If theDescriptions Is Nothing Then
                        CoreMessageHandler(message:="The supplied QueriedEnumeration type '" & _objecttype.Name & "' has no class description for table '" & tablename & "'", procedure:="ormQueriedSQLEnumeration.CheckTablename", _
                                           messagetype:=otCoreMessageType.InternalError)
                        Return False
                    Else
                        For Each aDescription In theDescriptions
                            If Not _otherobjectids.Contains(aDescription.ObjectAttribute.ID) Then _otherobjectids.Add(aDescription.ObjectAttribute.ID)
                        Next
                    End If
                Next
                ''' conclude
                ''' 
                If Not _otherobjectids.Contains(_objectid.ToUpper) Then
                    CoreMessageHandler(message:="The supplied QueriedEnumeration type '" & _objecttype.Name & "' does not use the table '" & tablenames.ToString & "'", procedure:="ormQueriedSQLEnumeration.CheckTablename", _
                                       messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If
            End If
        End Function

        ''' <summary>
        ''' set the Object Type and objectclass description depending if the type implements iorpersistable
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function SetObjectType(type As System.Type) As Boolean
            ''' Check Type
            ''' 
            If type.GetInterface(name:=GetType(iormRelationalPersistable).Name) IsNot Nothing Then
                Dim aDescription = ot.GetObjectClassDescription(type)
                If aDescription Is Nothing Then
                    Throw New ormException(message:="The supplied type '" & type.Name & "' has not been found in the Class Repository ")
                Else
                    _objectid = aDescription.ObjectAttribute.ID
                    _objecttype = type
                    Dim aList As New List(Of String)
                    For Each anEntry In Me.GetObjectEntries
                        If anEntry.IsMapped Then aList.Add(anEntry.Entryname)
                    Next
                    Me.ObjectEntryNames = aList
                    Return True
                End If
            Else
                Throw New ormException(message:="The supplied type '" & type.Name & "' is not implementing " & GetType(iormRelationalPersistable).Name)
            End If
        End Function
        ''' <summary>
        ''' returns the primary ClassDescription
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectClassDescription() As ObjectClassDescription Implements iormQueriedEnumeration.GetObjectClassDescription
            Return ot.GetObjectClassDescriptionByID(_objectid)
        End Function
        ''' <summary>
        ''' returns the primary Object Definition
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectDefinition() As ormObjectDefinition Implements iormQueriedEnumeration.GetObjectDefinition
            Return CurrentSession.Objects.GetObjectDefinition(_objectid)
        End Function
        ''' <summary>
        ''' returns a list of iobject entries returned by this Queried Enumeration
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectEntries() As IList(Of iormObjectEntryDefinition) Implements iormQueriedEnumeration.GetObjectEntries
            Dim anObjectDefinition As ormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=_objectid)
            Return anObjectDefinition.GetOrderedEntries
        End Function
        ''' <summary>
        ''' returns a list of iobject entries returned by this Queried Enumeration
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectEntry(name As String) As iormObjectEntryDefinition Implements iormQueriedEnumeration.GetObjectEntry
            Dim anObjectDefinition As ormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=_objectid)
            Return anObjectDefinition.GetEntry(entryname:=name)
        End Function

        ''' <summary>
        ''' sets the value of query parameter
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetValue(name As String, value As Object) As Boolean Implements iormQueriedEnumeration.SetValue
            If _parametervalues.ContainsKey(name) Then
                Return False
            Else
                _parametervalues.Add(name, value)
            End If
        End Function
        ''' <summary>
        ''' gets the value of a query parameter
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetValue(name As String, ByRef value As Object) As Boolean Implements iormQueriedEnumeration.GetValue
            If _parametervalues.ContainsKey(name) Then
                value = _parametervalues.Item(key:=name)
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' resets the result but not the query itself
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Reset() As Boolean Implements iormQueriedEnumeration.Reset
            If _run Then
                _run = False
                _records.Clear()
                _runTimestamp = Nothing
                _parametervalues.Clear()
                Return True
            End If
            Return False
        End Function
        ''' <summary>
        ''' returns an infused object out of the zero-based number or results
        ''' </summary>
        ''' <param name="no"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObject(no As ULong) As iormDataObject Implements iormQueriedEnumeration.GetObject
            If Not _run Then
                If Not Me.Load() Then
                    CoreMessageHandler(message:="failed to run query", procedure:="ormQueriedSQLEnumeration.GetObject", messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If
            End If
            If _run Then
                If no < _records.Count Then
                    Dim newObject As iormRelationalPersistable = TryCast(ot.CurrentSession.DataObjectProvider(_objecttype).NewOrmDataObject(_objecttype), iormRelationalPersistable)
                    If ormBusinessObject.InfuseDataObject(_records.ElementAt(no), dataobject:=newObject, mode:=otInfuseMode.OnInject) Then
                        Return newObject
                    End If
                End If
            End If
            Return Nothing
        End Function
        ''' <summary>
        ''' adds a database object to the results of the query
        ''' </summary>
        ''' <param name="no"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AddObject(dataobject As iormRelationalPersistable, Optional ByRef no As ULong? = Nothing) As Boolean Implements iormQueriedEnumeration.AddObject
            If Not _run Then
                If Not Me.Load() Then
                    CoreMessageHandler(message:="failed to run query", procedure:="ormQueriedSQLEnumeration.GetObject", messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If
            End If
            If _run Then

                _records.Add(dataobject.Record)
                If no.HasValue Then no = _records.Count - 1
                RaiseEvent OnAdded(Me, New EventArgs())
            End If
            Return True
        End Function
        ''' <summary>
        ''' remove the data object at position in the query result
        ''' </summary>
        ''' <param name="no"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RemoveObject(no As ULong) As Boolean Implements iormQueriedEnumeration.RemoveObject
            If Not _run Then
                If Not Me.Load() Then
                    CoreMessageHandler(message:="failed to run query", procedure:="ormQueriedSQLEnumeration.RemoveObject", messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If
            End If
            If _run Then
                ' remove it
                If no >= 0 And no < _records.Count Then

                    _records.RemoveAt(no)
                    RaiseEvent OnRemoved(Me, New EventArgs())
                End If

            End If
            Return Nothing
        End Function
        ''' <summary>
        ''' returns the zero-based ormRecord
        ''' </summary>
        ''' <param name="no"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetRecord(no As ULong) As ormRecord Implements iormQueriedEnumeration.GetRecord
            If Not _run Then
                If Not Me.Load() Then
                    CoreMessageHandler(message:="failed to run query", procedure:="ormQueriedSQLEnumeration.GetRecord", messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If
            End If
            If _run Then
                If no < _records.Count Then Return _records.ElementAt(no)
            End If
            Return Nothing
        End Function
        ''' <summary>
        ''' run the query
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Load(Optional domainid As String = Nothing) As Boolean Implements iormQueriedEnumeration.Load

            ''' prepare
            ''' 
            If Not _select.IsPrepared Then
                If Not _select.Prepare Then
                    CoreMessageHandler(message:="sql select command couldnot be prepared", procedure:="ormQueriedSQLEnumeration.Run", messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If
            End If

            ''' raise event
            RaiseEvent OnLoading(Me, New EventArgs())

            If _select.IsPrepared Then
                ''' instance just for some settings
                ''' should be reworked
                Dim anObjectDefinition As ormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(_objectid)
                Dim hasDomainBehavior As Boolean = False
                If anObjectDefinition IsNot Nothing Then
                    hasDomainBehavior = anObjectDefinition.HasDomainBehavior
                End If
                If hasDomainBehavior Then
                    If String.IsNullOrEmpty(domainid) Then domainid = Me.Domainid
                    If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
                    ''' set domain parameter
                    Dim aDomainIDParameter = _select.Parameters.Find(Function(x)
                                                                         Return x.ID.ToUpper = "@" & Domain.ConstFNDomainID.ToUpper
                                                                     End Function)
                    If aDomainIDParameter IsNot Nothing Then
                        If _parametervalues.ContainsKey(key:="@" & Domain.ConstFNDomainID.ToUpper) Then
                            _parametervalues.Remove(key:="@" & Domain.ConstFNDomainID.ToUpper)
                        End If
                        _parametervalues.Add(key:="@" & Domain.ConstFNDomainID.ToUpper, value:=domainid)
                        aDomainIDParameter.Value = domainid
                    End If
                End If

                ''' run the statement
                ''' 
                _qryStart = DateTime.Now
                _qrystopwatch.Start()
                Dim aRecordCollection = _select.RunSelect(parametervalues:=_parametervalues)
                If aRecordCollection Is Nothing Then
                    CoreMessageHandler(message:="no records returned due to previous errors", procedure:="ormQueriedSQLEnumeration.Run", argument:=Me.Id, _
                                       objectname:=_objectid, containerID:=_select.TableIDs.ToString, messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If
                _qryEnd = DateTime.Now
                _qrystopwatch.Stop()
                _qrycount = aRecordCollection.Count
                Call CoreMessageHandler(message:="query " & Me.Id & " run on " & Format(QryStart, "yyyy-mm-dd hh:mm:ss") & " for " & _
                                        _qrystopwatch.ElapsedMilliseconds & " ms and returned " & _qrycount & " records", _
                                        messagetype:=otCoreMessageType.InternalInfo, procedure:="ormQueriedSQLEnumeration.Run")

                If hasDomainBehavior And domainid <> ConstGlobalDomain Then

                    Dim aDomainRecordCollection As New Dictionary(Of String, ormRecord)
                    Dim pknames = CurrentSession.OTDBDriver.RetrieveContainerSchema(_select.TableIDs.First).PrimaryEntryNames
                    '*** get all records and store either the currentdomain or the globaldomain if on domain behavior
                    '***
                    For Each aRecord As ormRecord In aRecordCollection

                        '** build pk key
                        Dim pk As String = String.Empty
                        For Each acolumnname In pknames
                            If acolumnname <> Commons.Domain.ConstFNDomainID Then pk &= aRecord.GetValue(index:=acolumnname).ToString & ConstDelimiter
                        Next

                        If aDomainRecordCollection.ContainsKey(pk) Then
                            Dim anotherRecord = aDomainRecordCollection.Item(pk)
                            If anotherRecord.GetValue(Domain.ConstFNDomainID).ToString = ConstGlobalDomain Then
                                aDomainRecordCollection.Remove(pk)
                                aDomainRecordCollection.Add(key:=pk, value:=aRecord)
                            End If
                        Else
                            aDomainRecordCollection.Add(key:=pk, value:=aRecord)
                        End If
                    Next

                    ''' set the result
                    _records = aDomainRecordCollection.Values.ToList
                Else
                    ''' set the result
                    _records = aRecordCollection
                End If

                _run = True
                _runTimestamp = DateTime.Now
            Else
                _run = False
                _runTimestamp = DateTime.Now
            End If

            ''' raise event
            RaiseEvent OnLoaded(Me, New EventArgs())
            Return _run
        End Function
        ''' <summary>
        ''' returns a Enumerator over the QueriedEnumeration
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEnumerator() As IEnumerator Implements iormQueriedEnumeration.GetEnumerator
            Return New ormQueriedEnumerator(Me)
        End Function
        ''' <summary>
        ''' Returns an enumerator that iterates through the collection.
        ''' </summary>
        ''' <returns>
        ''' A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can
        ''' be used to iterate through the collection.
        ''' </returns>
        Public Function GetEnumerator1() As IEnumerator(Of iormRelationalPersistable) Implements IEnumerable(Of iormRelationalPersistable).GetEnumerator
            Return New ormQueriedEnumerator(Me)
        End Function
    End Class
End Namespace
