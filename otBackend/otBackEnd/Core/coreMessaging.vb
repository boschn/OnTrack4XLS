REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** CORE Messaging Classes
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2015-05-06
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2015
REM ***********************************************************************************************************************************************''' <summary>
Option Explicit On
Imports System.Collections
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Diagnostics.Debug
Imports System.Text.RegularExpressions
Imports System.Collections.Concurrent

Imports System.IO
Imports System.Threading

Imports OnTrack.Database
Imports OnTrack.rulez
Imports System.Reflection
Imports OnTrack.Commons

Namespace OnTrack.Core



    ''' <summary>
    ''' describes a persistable Session Log Message
    ''' </summary>
    ''' <remarks></remarks>

    <ormObject(id:=SessionMessage.ConstObjectID, description:="message generated during an OnTrack session", modulename:=ConstModuleCommons, Version:=1)> _
    Public Class SessionMessage
        Inherits ormBusinessObject
        Implements iormRelationalPersistable
        Implements iormInfusable
        Implements iormCloneable
        Implements ICloneable

        '*** CONST Schema
        Public Const ConstObjectID = "SessionMessage"
        '** Table
        <ormTableAttribute(Version:=5)> Public Const ConstPrimaryTableID = "tblSessionLogMessages"

        ''' <summary>
        ''' primary keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, _
                         title:="Session", Description:="sessiontag", PrimaryKeyOrdinal:=1)> Public Const ConstFNTag As String = "tag"

        <ormObjectEntry(Datatype:=otDataType.Long, _
                         title:="no", Description:="number of entry", PrimaryKeyOrdinal:=2)> Public Const ConstFNno As String = "no"

        ''' <summary>
        ''' column definitions
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, isnullable:=True, _
                         title:="Message ID", Description:="id of the message")> Public Const ConstFNID As String = "id"

        <ormObjectEntry(Datatype:=otDataType.Memo, isnullable:=True, _
                         title:="Message", Description:="message text")> Public Const ConstFNmessage As String = "message"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, isnullable:=True, _
                         title:="Routine", Description:="routine name")> Public Const ConstFNsubname As String = "subname"

        <ormObjectEntry(Datatype:=otDataType.Timestamp, isnullable:=True, _
                         title:="Timestamp", Description:="timestamp of entry")> Public Const ConstFNtimestamp As String = "timestamp"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, isnullable:=True, _
                         title:="Object", Description:="object name")> Public Const ConstFNObjectname As String = "object"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, isnullable:=True, _
                         title:="ObjectEntry", Description:="object entry")> Public Const ConstFNObjectentry As String = "objectentry"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, isnullable:=True, _
                         title:="Table", Description:="tablename")> Public Const ConstFNtablename As String = "table"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=100, isnullable:=True, _
                         title:="Column", Description:="columnname in the table")> Public Const ConstFNColumn As String = "column"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=255, isnullable:=True, _
                         title:="Argument", Description:="argument of the message")> Public Const ConstFNarg As String = "arg"

        <ormObjectEntry(Datatype:=otDataType.Long, isnullable:=True, _
                         title:="message type id", Description:="id of the message type")> Public Const ConstFNtype As String = "typeid"

        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, isnullable:=True, title:="Username of the session", Description:="name of the user for this session")> _
        Public Const ConstFNUsername As String = "username"

        <ormObjectEntry(Datatype:=otDataType.Memo, isnullable:=True, title:="stack trace", Description:="caller stack trace")> _
        Public Const ConstFNStack As String = "stack"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, _
         useforeignkey:=otForeignKeyImplementation.None, isnullable:=True)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        <ormObjectEntry(Datatype:=otDataType.Text, size:=255, isnullable:=True, _
                        title:="tag", Description:="object tag values")> Public Const ConstFNObjectTag As String = "OBJECTTAG"

        ' fields
        <ormObjectEntryMapping(EntryName:=ConstFNTag)> Private _tag As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNID)> Private _id As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNno)> Private _entryno As Long = 0
        <ormObjectEntryMapping(EntryName:=ConstFNmessage)> Private _Message As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNsubname)> Private _Subname As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNtimestamp)> Private _Timestamp As Date = constNullDate
        <ormObjectEntryMapping(EntryName:=ConstFNObjectname)> Private _Objectname As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNObjectentry)> Private _Entryname As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNtablename)> Private _Tablename As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNColumn)> Private _Columnname As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNtype)> Private _ErrorType As otCoreMessageType
        <ormObjectEntryMapping(EntryName:=ConstFNUsername)> Private _Username As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNStack)> Private _StackTrace As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNarg)> Private _Arguments As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNDomainID)> Private _domainid As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNObjectTag)> Private _objecttag As String = String.Empty

        '** dynamic
        Private _processed As Boolean = False
        Private _Exception As Exception

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            Call MyBase.New()
            _ErrorType = otCoreMessageType.ApplicationInfo
            _Timestamp = DateTime.Now()
        End Sub

#Region "Properties"


        ''' <summary>
        ''' Gets or sets the domainid.
        ''' </summary>
        ''' <value>The domainid.</value>
        Public Overloads Property Domainid As String
            Get
                Return Me._domainid
            End Get
            Set(value As String)
                Me._domainid = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the objecttag.
        ''' </summary>
        ''' <value>The objecttag.</value>
        Public Property Objecttag As String
            Get
                Return Me._objecttag
            End Get
            Set(value As String)
                Me._objecttag = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the stack trace.
        ''' </summary>
        ''' <value>The stack trace.</value>
        Public Property StackTrace As String
            Get
                Return Me._StackTrace
            End Get
            Set(value As String)
                Me._StackTrace = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the username.
        ''' </summary>
        ''' <value>The username.</value>
        Public Property ID As String
            Get
                Return Me._id
            End Get
            Set(value As String)
                Me._id = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the username.
        ''' </summary>
        ''' <value>The username.</value>
        Public Property Username() As String
            Get
                Return Me._Username
            End Get
            Set(value As String)
                Me._Username = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the tag.
        ''' </summary>
        ''' <value>The tag.</value>
        Public Property Tag() As String
            Get
                Return Me._tag
            End Get
            Set(value As String)
                _tag = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the processed.
        ''' </summary>
        ''' <value>The processed.</value>
        Public Property Processed() As Boolean
            Get
                Return Me._processed
            End Get
            Set(value As Boolean)
                Me._processed = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the entryno.
        ''' </summary>
        ''' <value>The entryno.</value>
        Public Property Entryno() As Long
            Get
                Return Me._entryno
            End Get
            Set(value As Long)
                Me._entryno = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the name of the entry.
        ''' </summary>
        ''' <value>The name of the entry.</value>
        Public Property Columnname() As String
            Get
                Return Me._Columnname
            End Get
            Set(value As String)
                Me._Columnname = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the type of the error.
        ''' </summary>
        ''' <value>The type of the error.</value>
        Public Property messagetype() As otCoreMessageType
            Get
                Return Me._ErrorType
            End Get
            Set(value As otCoreMessageType)
                Me._ErrorType = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the tablename.
        ''' </summary>
        ''' <value>The tablename.</value>
        Public Property Tablename() As String
            Get
                Return Me._Tablename
            End Get
            Set(value As String)
                Me._Tablename = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the name of the object.
        ''' </summary>
        ''' <value>The name of the entry.</value>
        Public Property Objectname() As String
            Get
                Return Me._Objectname
            End Get
            Set(value As String)
                Me._Objectname = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the name of the object entry.
        ''' </summary>
        ''' <value>The name of the entry.</value>
        Public Property ObjectEntry() As String
            Get
                Return Me._Entryname
            End Get
            Set(value As String)
                Me._Entryname = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the timestamp.
        ''' </summary>
        ''' <value>The timestamp.</value>
        Public Property Timestamp() As DateTime
            Get
                Return Me._Timestamp
            End Get
            Set(value As DateTime)
                Me._Timestamp = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the subname.
        ''' </summary>
        ''' <value>The subname.</value>
        Public Property Subname() As String
            Get
                Return Me._Subname
            End Get
            Set(value As String)
                Me._Subname = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the arguments.
        ''' </summary>
        ''' <value>The arguments.</value>
        Public Property Arguments() As String
            Get
                Return Me._Arguments
            End Get
            Set(value As String)
                Me._Arguments = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the exception.
        ''' </summary>
        ''' <value>The exception.</value>
        Public Property Exception() As Exception
            Get
                Return Me._Exception
            End Get
            Set(value As Exception)
                Me._Exception = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the message.
        ''' </summary>
        ''' <value>The message.</value>
        Public Property Message() As String
            Get
                Return Me._Message
            End Get
            Set(value As String)
                Me._Message = value
            End Set
        End Property
#End Region



        ''' <summary>
        ''' create a persistable Error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateDataObject(ByVal sessiontag As String, ByVal entryno As Long) As SessionMessage
            Dim primarykey() As Object = {sessiontag, entryno}
            ' create
            Return ormBusinessObject.CreateDataObject(Of SessionMessage)(primarykey, checkUnique:=False, runtimeOnly:=True)
        End Function


        ''' <summary>
        ''' load and infuse the object by primary key
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="entryname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Retrieve(ByVal sessiontag As String, ByVal entryno As Long) As SessionMessage
            Dim primarykey() As Object = {sessiontag, entryno}
            Return ormBusinessObject.RetrieveDataObject(Of SessionMessage)(pkArray:=primarykey)
        End Function



        ''' <summary>
        ''' clone the error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Clone() As Object Implements System.ICloneable.Clone
            Dim aClone As New SessionMessage
            With aClone
                If Me.Tag IsNot Nothing Then .Tag = Me.Tag.Clone
                If Me.ID IsNot Nothing Then .ID = Me.ID.Clone
                .Exception = Me.Exception
                If Me.Username IsNot Nothing Then .Username = Me.Username.Clone
                .Entryno = Me.Entryno
                If Me.Tablename IsNot Nothing Then .Tablename = Me.Tablename.Clone
                If Me.Columnname IsNot Nothing Then .Columnname = Me.Columnname.Clone
                If Me.Message IsNot Nothing Then .Message = Me.Message.Clone
                .messagetype = Me.messagetype
                .Timestamp = Me.Timestamp
                .StackTrace = Me.StackTrace
                If Me.Objectname IsNot Nothing Then .Objectname = Me.Objectname.Clone
                If Me.ObjectEntry IsNot Nothing Then .ObjectEntry = Me.ObjectEntry.Clone
                If Me.Objecttag IsNot Nothing Then .Objecttag = Me.Objecttag.Clone
            End With

            Return aClone
        End Function
    End Class

    ''' <summary>
    ''' Event Arguments for Request Bootstrapping Installation
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SessionBootstrapEventArgs
        Inherits EventArgs

        Private _install As Boolean = False
        Private _askbefore As Boolean = True
        Private _modules As String()
        Private _installationResult As Boolean = False

        Public Sub New(install As Boolean, modules As String(), Optional AskBefore As Boolean = True)
            _install = install
            _modules = modules
            _askbefore = AskBefore
        End Sub

        Public ReadOnly Property Install As Boolean
            Get
                Return _install
            End Get
        End Property
        Public ReadOnly Property AskBefore As Boolean
            Get
                Return _askbefore
            End Get
        End Property
        Public ReadOnly Property Modules As String()
            Get
                Return _modules
            End Get
        End Property
        Public Property InstallationResult As Boolean
            Get
                Return _installationResult
            End Get
            Set(value As Boolean)
                _installationResult = value
            End Set
        End Property
    End Class



    ''' <summary>
    ''' Describes an not persistable Log of Messages. Can be persisted by SessionLogMessages
    ''' </summary>
    ''' <remarks></remarks>

    Public Class SessionMessageLog
        Implements IEnumerable
        Implements ICloneable

        Public Event onErrorRaised As EventHandler(Of ormErrorEventArgs)
        Public Event onLogClear As EventHandler(Of ormErrorEventArgs)
        '*** log
        Private _log As New SortedList(Of Long, SessionMessage)
        Private _queue As New ConcurrentQueue(Of SessionMessage)
        Private _maxEntry As Long = 0
        Private _tag As String
        Private _lockObject As New Object ' lock object instead of me

        Public Sub New(tag As String)
            _tag = tag
        End Sub
        ''' <summary>
        ''' Gets the tag.
        ''' </summary>
        ''' <value>The tag.</value>
        Public ReadOnly Property Tag() As String
            Get
                Return Me._tag
            End Get
        End Property

        ''' <summary>
        ''' Returns an enumerator that iterates through a collection.
        ''' </summary>
        ''' <returns>
        ''' An <see cref="T:System.Collections.IEnumerator" /> object that can be
        ''' used to iterate through the collection.
        ''' </returns>
        Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Dim anEnumerator As IEnumerator
            SyncLock _lockObject
                Dim aList As List(Of SessionMessage) = _log.Values.ToList
                anEnumerator = aList.GetEnumerator
            End SyncLock
            Return anEnumerator
        End Function

        Public Function Clone() As Object Implements System.ICloneable.Clone
            Dim m As New System.IO.MemoryStream()
            Dim f As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
            f.Serialize(m, Me)
            m.Seek(0, System.IO.SeekOrigin.Begin)
            Return f.Deserialize(m)
        End Function
        ''' <summary>
        ''' Clears the error log from all messages
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Clear()
            RaiseEvent onLogClear(Me, New ormErrorEventArgs(Nothing))
            _log.Clear()
            '_queue = Nothing leave it for flush
            Return True
        End Function
        ''' <summary>
        ''' Persist the Messages
        ''' </summary>
        ''' <param name="timestamp"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Persist(Optional timestamp As Date = ot.constNullDate) As Boolean
            '** we have a session
            If CurrentSession.IsRunning Then
                '*** only if the table is there
                If CurrentSession.OTDBDriver.GetContainerObject(SessionMessage.ConstPrimaryTableID) Is Nothing Then
                    Return False
                End If

                SyncLock _lockObject
                    For Each anError As SessionMessage In _log.Values
                        If Not anError.Processed And anError.IsAlive Then
                            anError.Persist()
                            anError.Processed = True ' do not again
                        End If
                    Next
                End SyncLock

            End If

            Return False
        End Function
        ''' <summary>
        ''' Add an otdb error object to the log
        ''' </summary>
        ''' <param name="otdberror"></param>
        ''' <remarks></remarks>
        Public Sub Enqueue(otdberror As SessionMessage)
            Dim aClone As SessionMessage = otdberror.Clone
            Try
                ' add
                SyncLock _lockObject

                    If aClone.Timestamp = Nothing Then
                        aClone.Timestamp = DateTime.Now()
                    End If

                    aClone.Tag = Me.Tag
                    aClone.Entryno = _maxEntry + 1

                    _queue.Enqueue(aClone)
                    _log.Add(key:=aClone.Entryno, value:=aClone)
                    _maxEntry += 1

                End SyncLock

                RaiseEvent onErrorRaised(Me, New ormErrorEventArgs(aClone))

            Catch ex As Exception
                Debug.WriteLine("{0} Exception raised in SessionMessageLog.Enqueue", Date.Now)
                Debug.WriteLine("{0}", ex.Message)
                Debug.WriteLine("{0}", ex.StackTrace)
            End Try

        End Sub
        ''' <summary>
        ''' returns the size of the log
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Size() As Long
            SyncLock _lockObject
                Return _log.Count
            End SyncLock
        End Function
        ''' <summary>
        ''' try to get the first Error from log
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function PeekFirst() As SessionMessage
            Dim anError As SessionMessage
            SyncLock _lockObject
                If _queue.TryPeek(anError) Then
                    Return anError
                Else
                    Return Nothing
                End If
            End SyncLock
        End Function
        ''' <summary>
        ''' try to get the most recent error from log without removing
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function PeekLast() As SessionMessage
            Dim anError As SessionMessage
            SyncLock _lockObject
                If _queue.Count >= 1 Then
                    Return _queue.ToArray.Last
                Else
                    Return Nothing
                End If
            End SyncLock
        End Function
        ''' <summary>
        ''' remove and returns the first error in the error log 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Retain() As SessionMessage
            Dim anError As SessionMessage
            SyncLock _lockObject
                If _queue.TryDequeue([anError]) Then
                    Return anError
                Else
                    Return Nothing
                End If
            End SyncLock
        End Function

    End Class



    ''' <summary>
    ''' OntrackChangeLog for Changes in the OnTrack Modules and Classes 
    ''' </summary>
    ''' <remarks>
    ''' 
    ''' The OntrackChangeLog is not an Data Object on its own. it is derived from the RelationCollection and
    ''' embedded as relation Member in a data object class
    ''' </remarks>
    Public Class OnTrackChangeLog
        Inherits ormRelationCollection(Of OnTrackChangeLogEntry)

        ''' <summary>
        ''' Version presentation class
        ''' </summary>
        ''' <remarks></remarks>
        Public Class Versioning
            Implements IComparable
            Implements IHashCodeProvider


            Private _version As Long
            Private _release As Long
            Private _patch As Long

            ''' <summary>
            ''' constructor
            ''' </summary>
            ''' <param name="version"></param>
            ''' <param name="release"></param>
            ''' <param name="patch"></param>
            ''' <remarks></remarks>
            Public Sub New(version As Long, release As Long, patch As Long)
                _version = version
                _release = release
                _patch = patch
            End Sub
            ''' <summary>
            ''' Gets or sets the version.
            ''' </summary>
            ''' <value>The version.</value>
            Public Property Version() As Long
                Get
                    Return Me._version
                End Get
                Set(value As Long)
                    Me._version = value
                End Set
            End Property
            ''' <summary>
            ''' Gets or sets the release.
            ''' </summary>
            ''' <value>The release.</value>
            Public Property Release() As Long
                Get
                    Return Me._release
                End Get
                Set(value As Long)
                    Me._release = value
                End Set
            End Property
            ''' <summary>
            ''' Gets or sets the patch.
            ''' </summary>
            ''' <value>The patch.</value>
            Public Property Patch() As Long
                Get
                    Return Me._patch
                End Get
                Set(value As Long)
                    Me._patch = value
                End Set
            End Property

            ''' <summary>
            ''' Comparer
            ''' </summary>
            ''' <param name="obj"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
                Dim aVersion As Versioning = TryCast(obj, Versioning)
                If aVersion Is Nothing Then Return -1

                If aVersion.Version = Me.Version AndAlso aVersion.Release = Me.Release AndAlso aVersion.Patch = Me.Patch Then
                    Return 0
                ElseIf aVersion.Version >= Me.Version AndAlso aVersion.Release >= Me.Release AndAlso aVersion.Patch >= Me.Patch Then
                    Return 1
                Else
                    Return 0
                End If
            End Function

            ''' <summary>
            ''' returns hashcode
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function GetHascode() As Integer
                Return Me.GetHashCode(Me)
            End Function
            ''' <summary>
            ''' returns hashcode
            ''' </summary>
            ''' <param name="obj"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function GetHashCode(o As Object) As Integer Implements IHashCodeProvider.GetHashCode
                Dim aVersion As Versioning = TryCast(o, Versioning)
                If aVersion Is Nothing Then Return o.GetHashCode
                Return aVersion.Version Xor aVersion.Release Xor aVersion.Patch
            End Function

            ''' <summary>
            ''' Returns the Versioning String
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function ToString() As String
                Return String.Format("V{0}.R{1}.P{2}", Me.Version, Me.Release, Me.Patch)
            End Function

        End Class

        Private _ApplicationVersion As New Dictionary(Of String, Versioning)

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="container"></param>
        ''' <remarks></remarks>

        Public Sub New()
            MyBase.New(container:=Nothing, keyentrynames:={OnTrackChangeLogEntry.ConstFNApplication, OnTrackChangeLogEntry.ConstFNModule, _
                                                           OnTrackChangeLogEntry.ConstFNVersion, OnTrackChangeLogEntry.ConstFNRelease, _
                                                           OnTrackChangeLogEntry.ConstFNPatch, OnTrackChangeLogEntry.ConstFNImplNo})

        End Sub

#Region "Properties"
        ''' <summary>
        ''' Returns the Maximal Version or with optional application the version of the application (or nothing)
        ''' </summary>
        ''' <param name="application"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Version(Optional application = Nothing) As String
            Get
                If application Is Nothing Then
                    Dim maxVersion As Versioning = New Versioning(0, 0, 0)
                    For Each aVersion In _ApplicationVersion.Values
                        If aVersion.CompareTo(maxVersion) > 1 Then maxVersion = aVersion
                    Next
                    Return maxVersion.ToString
                Else
                    If _ApplicationVersion.ContainsKey(key:=application.toupper) Then Return _ApplicationVersion.Item(key:=application.toupper).ToString
                    Return Nothing
                End If
            End Get
        End Property


#End Region

        ''' <summary>
        ''' Initialize the Changelog by searching the assembly
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Refresh(Optional type As System.Type = Nothing) As Boolean
            Dim thisAsm As Assembly
            If type Is Nothing Then
                thisAsm = Assembly.GetExecutingAssembly
            Else
                thisAsm = Assembly.GetAssembly(type:=type)
            End If

            ''' 
            ''' Look into the Modules
            ''' 
            For Each aModule As [Module] In thisAsm.GetModules.ToList
                For Each anAttribute As System.Attribute In aModule.GetCustomAttributes(False)
                    ''' ChangeLog Attribute
                    ''' 
                    If anAttribute.GetType().Equals(GetType(ormChangeLogEntry)) Then
                        Dim aChangeLogAttribute = DirectCast(anAttribute, ormChangeLogEntry)
                        Me.Add(aChangeLogAttribute)
                    End If
                Next

                ''' look into fields
                ''' 
                For Each aField As FieldInfo In aModule.GetFields
                    For Each anAttribute As System.Attribute In aField.GetCustomAttributes(False)
                        ''' ChangeLog Attribute
                        ''' 
                        If anAttribute.GetType().Equals(GetType(ormChangeLogEntry)) Then
                            Dim aChangeLogAttribute = DirectCast(anAttribute, ormChangeLogEntry)
                            Me.Add(aChangeLogAttribute)
                        End If
                    Next
                Next

                ''' look into subs
                ''' 
                For Each aMethod As MethodInfo In aModule.GetMethods
                    For Each anAttribute As System.Attribute In aMethod.GetCustomAttributes(False)
                        ''' ChangeLog Attribute
                        ''' 
                        If anAttribute.GetType().Equals(GetType(ormChangeLogEntry)) Then
                            Dim aChangeLogAttribute = DirectCast(anAttribute, ormChangeLogEntry)
                            Me.Add(aChangeLogAttribute)
                        End If
                    Next
                Next
            Next

            ''' 
            ''' Look into the Types and Classes
            ''' 
            For Each aClass As Type In thisAsm.GetTypes.Where(Function(t) t.IsClass).ToList
                For Each anAttribute As System.Attribute In aClass.GetCustomAttributes(False)
                    ''' ChangeLog Attribute
                    ''' 
                    If anAttribute.GetType().Equals(GetType(ormChangeLogEntry)) Then
                        Dim aChangeLogAttribute = DirectCast(anAttribute, ormChangeLogEntry)
                        Me.Add(aChangeLogAttribute)
                    End If
                Next

                ''' look into fields
                ''' 
                For Each aField As FieldInfo In aClass.GetFields
                    For Each anAttribute As System.Attribute In aField.GetCustomAttributes(False)
                        ''' ChangeLog Attribute
                        ''' 
                        If anAttribute.GetType().Equals(GetType(ormChangeLogEntry)) Then
                            Dim aChangeLogAttribute = DirectCast(anAttribute, ormChangeLogEntry)
                            Me.Add(aChangeLogAttribute)
                        End If
                    Next
                Next

                ''' look into subs
                ''' 
                For Each aMethod As MethodInfo In aClass.GetMethods
                    For Each anAttribute As System.Attribute In aMethod.GetCustomAttributes(False)
                        ''' ChangeLog Attribute
                        ''' 
                        If anAttribute.GetType().Equals(GetType(ormChangeLogEntry)) Then
                            Dim aChangeLogAttribute = DirectCast(anAttribute, ormChangeLogEntry)
                            Me.Add(aChangeLogAttribute)
                        End If
                    Next
                Next
            Next

            Return True
        End Function
        ''' <summary>
        ''' Clear the OnTrackChangeLog from all Entries
        ''' </summary>
        ''' <remarks></remarks>
        Public Overloads Sub Clear()
            '** delete Entries
            For Each changeEntry In Me
                changeEntry.Delete()
            Next
            MyBase.Clear()

        End Sub

        ''' <summary>
        ''' Add an ChangeLogEntry
        ''' </summary>
        ''' <param name="entry"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Add(entry As OnTrackChangeLogEntry) As Boolean
            '''
            '''
            If Me.ContainsKey(key:={entry.Application, entry.Module, entry.Version, entry.Release, entry.Patch, entry.ChangeImplementationNo}) Then
                CoreMessageHandler(message:="change log entry already in change log", argument:=Converter.Array2StringList({entry.Application, entry.Module, entry.Version, entry.Release, entry.Patch, entry.ChangeImplementationNo}), _
                                   messagetype:=otCoreMessageType.InternalWarning, procedure:="OnTrackChangeLog.Add")
            End If

            ''' add the max version to the Application Version
            ''' 
            If _ApplicationVersion.ContainsKey(key:=entry.Application.ToUpper) Then
                Dim aVersion As Versioning = _ApplicationVersion.Item(key:=entry.Application.ToUpper)
                Dim newVersion As Versioning = New Versioning(entry.Version, entry.Release, entry.Patch)
                If aVersion.CompareTo(newVersion) > 1 Then
                    _ApplicationVersion.Remove(key:=entry.Application.ToUpper)
                    _ApplicationVersion.Add(key:=entry.Application.ToUpper, value:=newVersion)
                End If
            Else
                _ApplicationVersion.Add(key:=entry.Application.ToUpper, value:=New Versioning(entry.Version, entry.Release, entry.Patch))
            End If

            ''' add the entry to list
            MyBase.Add(entry)
        End Function
        ''' <summary>
        ''' Add ormAttribute ormChangeLogEntry
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Add(attribute As ormChangeLogEntry) As Boolean
            Dim anEntry As OnTrackChangeLogEntry
            If ot.IsInitialized AndAlso CurrentSession.IsRunning Then
                anEntry = OnTrackChangeLogEntry.Create(application:=attribute.Application, [module]:=attribute.Module, _
                                                       version:=attribute.Version, release:=attribute.Release, patch:=attribute.Patch, _
                                                       changeimplno:=attribute.Changeimplno)

                If anEntry IsNot Nothing Then
                    With anEntry
                        .Description = attribute.Description
                        .ChangerequestID = attribute.ChangeID
                        .Releasedate = attribute.Releasedate
                    End With
                    Return Me.Add(anEntry)
                Else
                    CoreMessageHandler(message:="could not create change log entry - already in change log ?!", argument:=Converter.Array2StringList({attribute.Application, attribute.Module, attribute.Version, attribute.Release, attribute.Patch, attribute.Changeimplno}), _
                                                      messagetype:=otCoreMessageType.InternalWarning, procedure:="OnTrackChangeLog.AddAttribute")
                End If
            Else
                anEntry = New OnTrackChangeLogEntry(application:=attribute.Application, [module]:=attribute.Module, _
                                                    version:=attribute.Version, release:=attribute.Release, _
                                                    patch:=attribute.Patch, changeimplno:=attribute.Changeimplno, description:=attribute.Description _
                                                    )
                Return Me.Add(anEntry)
            End If



            Return False
        End Function

        ''' <summary>
        ''' retrieves the log and loads all messages for the container object
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Retrieve() As iormRelationalCollection(Of OnTrackChangeLogEntry)
            '''
            ''' check if the new Property value is different then old one
            ''' 
            '** build query
            Dim newCollection As ormRelationCollection(Of OnTrackChangeLogEntry) = New ormRelationCollection(Of OnTrackChangeLogEntry) _
                                                                           (Nothing, keyentrynames:={OnTrackChangeLogEntry.ConstFNApplication, _
                                                                                                     OnTrackChangeLogEntry.ConstFNModule, _
                                                                                                       OnTrackChangeLogEntry.ConstFNVersion, OnTrackChangeLogEntry.ConstFNRelease, _
                                                                                                       OnTrackChangeLogEntry.ConstFNPatch, OnTrackChangeLogEntry.ConstFNImplNo})

            Try
                Dim aStore As iormRelationalTableStore = ot.GetPrimaryTableStore(OnTrackChangeLogEntry.ConstPrimaryTableID) '_container.PrimaryTableStore is the class itself
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(id:="RetrieveChangeLogEntry", addAllFields:=True)
                If Not aCommand.IsPrepared Then
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@deleted", ColumnName:=BusinessObjectMessage.ConstFNIsDeleted, tableid:=BusinessObjectMessage.ConstPrimaryTableID))
                    aCommand.Prepare()
                End If
                aCommand.SetParameterValue(ID:="@deleted", value:=False)

                Dim aRecordCollection = aCommand.RunSelect

                For Each aRecord As ormRecord In aRecordCollection
                    Dim anEntry As New OnTrackChangeLogEntry
                    If anEntry.InfuseDataObject(record:=aRecord, dataobject:=anEntry) Then
                        newCollection.Add(item:=anEntry)
                    End If
                Next

                Return newCollection


            Catch ex As Exception

                Call CoreMessageHandler(exception:=ex, procedure:="OnTrackChangeLog.Retrieve")
                Return newCollection

            End Try
        End Function

    End Class



End Namespace
