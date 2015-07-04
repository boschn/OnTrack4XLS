

REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** BUSINESS OBJECTs: Dependency Classes 
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2013-08-08
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************


Option Explicit On

Imports System.Collections
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Diagnostics.Debug
Imports System.Data
Imports System.Data.OleDb

Imports OnTrack
Imports OnTrack.Database
Imports OnTrack.Deliverables
Imports OnTrack.IFM
Imports OnTrack.Parts

Namespace OnTrack.Scheduling

    Public Class clsOTDBDependency
        Inherits ormDataObject

        ' key
        Private s_pnid As String
        Private s_TypeIds As New Collection
        ' list of typeids -> Members of certain types
        Private s_DependMembers_TypeIDs As New Dictionary(Of String, Dictionary(Of Long, clsOTDBDependMember))    ' list of type-dependency-lists

        ' list of DependChecks per typeids -> Members of certain types
        Private s_DependCheck_TypeIDs As New Dictionary(Of String, Dictionary(Of Long, clsOTDBDependCheck))    ' list of type-dependency-lists
        ' components itself per key:=posno, item:=dependfrompartid
        'Private s_dependfrompartids As New Dictionary

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()

        End Sub

#Region "Poperties"

        ReadOnly Property PartID()
            Get
                PARTID = s_pnid
            End Get

        End Property

        ReadOnly Property NoMembers(typeid As String) As Long
            Get
                Dim dependFromList As New Dictionary(Of Long, clsOTDBDependMember)

                ' get the list
                dependFromList = getDependMemberTypeIdList(TYPEID)
                ' return if nothing
                If dependFromList Is Nothing Then
                    NoMembers = 0
                    Exit Property
                End If

                ' No of Components -1 (Head)
                NoMembers = dependFromList.Count - 1
            End Get

        End Property

        ReadOnly Property NoDependChecks(TYPEID As String) As Long
            Get
                Dim dependFromList As New Dictionary(Of Long, clsOTDBDependCheck)

                ' get the list
                dependFromList = getDependCheckTypeIdList(TYPEID)
                ' return if nothing
                If dependFromList Is Nothing Then
                    NoDependChecks = 0
                    Exit Property
                End If

                ' No of Components -1 (Head)
                NoDependChecks = dependFromList.Count - 1
            End Get
        End Property

        '**** typeids
        '****
        ReadOnly Property Typeids() As Collection
            Get
                Typeids = s_TypeIds
            End Get

        End Property
#End Region

        Public Function GetMaxPosNo(TYPEID As String) As Long
            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependMember)
            'Dim keys As List(Of String)
            'Dim i As Integer
            Dim max As Long

            ' get the list
            dependFromList = getDependMemberTypeIdList(TYPEID)
            ' return if nothing
            If dependFromList Is Nothing Then
                getMaxPosNo = 0
                Exit Function
            End If

            ' return
            If NoMembers(TYPEID) >= 0 Then
                For Each key As Long In dependFromList.Keys
                    If key > max Then max = key
                Next key
                getMaxPosNo = max
            Else
                getMaxPosNo = 0
            End If
        End Function
        '*** add a Component by cls OTDB
        '***
        Public Function AddPartID(typeid As String, partid As String) As clsOTDBDependMember
            Dim flag As Boolean
            Dim existEntry As New clsOTDBDependMember
            Dim anEntry As New clsOTDBDependMember
            Dim m As Object
            Dim posno As Long
            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependMember)

            ' empty
            If Not me.isloaded And Not Me.IsCreated Then
                addPartID = Nothing
                Exit Function
            End If

            ' get or add
            dependFromList = createDependMemberTypeIdList(TYPEID)

            ' check Members
            For Each m In dependFromList.Keys
                existEntry = dependFromList.Item(m)
                ' check
                If LCase(existEntry.dependfromPartID) = LCase(PartID) Then
                    addPartID = existEntry
                    Exit Function
                End If
            Next m

            ' create new Member
            anEntry = clsOTDBDependMember.Create(typeid:=typeid, partid:=s_pnid, posno:=posno, dependfromPartID:=partid)
            posno = Me.GetMaxPosNo(TYPEID) + 1
            If anEntry Is Nothing Then
                anEntry = clsOTDBDependMember.Retrieve(typeid:=typeid, partid:=s_pnid, posno:=posno)
            End If
            ' set it
            anEntry.dependfromPartID = PartID
            ' add the component
            If Me.addDependMember(anEntry) Then
                addPartID = anEntry
            Else
                addPartID = Nothing
            End If

        End Function

        '****** creates the TypeIDList of typeid or the existing one
        Private Function CreateDependMemberTypeIdList(TYPEID As String, _
                                                      Optional FORCE As Boolean = False) As Dictionary(Of Long, clsOTDBDependMember)
            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependMember)
            Dim anEntry As New clsOTDBDependMember

            ' empty
            If Not me.isloaded And Not Me.IsCreated Then
                createDependMemberTypeIdList = Nothing
                Exit Function
            End If

            If Not s_TypeIds.Contains(Key:=TYPEID) Then
                s_TypeIds.Add(Item:=TYPEID)
            End If

            ' add the the typeid list dictionary
            If s_DependMembers_TypeIDs.ContainsKey(key:=TYPEID) And Not FORCE Then
                createDependMemberTypeIdList = s_DependMembers_TypeIDs.Item(key:=TYPEID)
                Exit Function
            End If

            ' create new list with header
            dependFromList = New Dictionary(Of Long, clsOTDBDependMember)
            If Not anEntry.Create(typeid:=TYPEID, partid:=Me.PartID, posno:=0, dependfromPartID:=String.empty) Then
                Call anEntry.Retrieve(typeid:=TYPEID, partid:=Me.PartID, posno:=0)
                anEntry.dependfromPartID = String.empty
            End If

            dependFromList.Add(key:=0, value:=anEntry)
            Call exchangeDependMemberTypeIdList(TYPEID, dependFromList)
            createDependMemberTypeIdList = dependFromList
        End Function

        '****** returns the TypeIDList of typeid
        Private Function ExchangeDependMemberTypeIdList(TYPEID As String, _
                                                        dependFromList As Dictionary(Of Long, clsOTDBDependMember)) As Boolean

            ' empty
            If Not me.isloaded And Not Me.IsCreated Then
                exchangeDependMemberTypeIdList = False
                Exit Function
            End If

            ' add the the typeid list dictionary
            If s_DependMembers_TypeIDs.ContainsKey(key:=TYPEID) Then
                Call s_DependMembers_TypeIDs.Remove(key:=TYPEID)
                Call s_DependMembers_TypeIDs.Add(key:=TYPEID, value:=dependFromList)
                exchangeDependMemberTypeIdList = True
                Exit Function
            Else
                Call s_DependMembers_TypeIDs.Add(key:=TYPEID, value:=dependFromList)
                exchangeDependMemberTypeIdList = True
                Exit Function
            End If

            exchangeDependMemberTypeIdList = False
        End Function

        '****** returns the TypeIDList of typeid
        Private Function DeleteDependMemberTypeIdList(TYPEID As String) As Boolean
            ' empty
            If Not me.isloaded And Not Me.IsCreated Then
                deleteDependMemberTypeIdList = False
                Exit Function
            End If

            ' add the the typeid list dictionary
            If s_DependMembers_TypeIDs.ContainsKey(key:=TYPEID) Then
                Call s_DependMembers_TypeIDs.Remove(key:=TYPEID)
                deleteDependMemberTypeIdList = True
                Exit Function
            End If

            ' remove if not in the DependCheck List (otherwise keep it)
            If Not s_DependCheck_TypeIDs.ContainsKey(key:=TYPEID) Then
                s_TypeIds.Remove(Key:=TYPEID)
            End If
            deleteDependMemberTypeIdList = False
        End Function

        '****** returns the TypeIDList of DependMembers typeid
        Private Function GetDependMemberTypeIdList(TYPEID As String) As Dictionary(Of Long, clsOTDBDependMember)
            ' empty
            If Not me.isloaded And Not Me.IsCreated Then
                getDependMemberTypeIdList = Nothing
                Exit Function
            End If

            ' add the the typeid list dictionary
            If s_DependMembers_TypeIDs.ContainsKey(key:=TYPEID) Then
                getDependMemberTypeIdList = s_DependMembers_TypeIDs.Item(key:=TYPEID)
                Exit Function
            End If

            getDependMemberTypeIdList = Nothing
        End Function
        '****** returns the TypeIDList of typeid
        Private Function DeleteDependCheckTypeIdList(TYPEID As String) As Boolean
            ' empty
            If Not me.isloaded And Not Me.IsCreated Then
                deleteDependCheckTypeIdList = False
                Exit Function
            End If

            ' add the the typeid list dictionary
            If s_DependCheck_TypeIDs.ContainsKey(key:=TYPEID) Then
                Call s_DependCheck_TypeIDs.Remove(key:=TYPEID)
                deleteDependCheckTypeIdList = True
                Exit Function
            End If

            deleteDependCheckTypeIdList = False
        End Function
        '****** returns the TypeIDList of typeid
        Private Function ExchangeDependCheckTypeIdList(TYPEID As String, _
                                                       dependFromList As Dictionary(Of Long, clsOTDBDependCheck)) As Boolean

            ' empty
            If Not me.isloaded And Not Me.IsCreated Then
                exchangeDependCheckTypeIdList = False
                Exit Function
            End If

            ' add the the typeid list dictionary
            If s_DependCheck_TypeIDs.ContainsKey(key:=TYPEID) Then
                Call s_DependCheck_TypeIDs.Remove(key:=TYPEID)
                Call s_DependCheck_TypeIDs.Add(key:=TYPEID, value:=dependFromList)
                exchangeDependCheckTypeIdList = True
                Exit Function
            Else
                Call s_DependCheck_TypeIDs.Add(key:=TYPEID, value:=dependFromList)
                exchangeDependCheckTypeIdList = True
                Exit Function
            End If

            exchangeDependCheckTypeIdList = False
        End Function

        '****** returns the TypeIDList of DependChecks
        Private Function GetDependCheckTypeIdList(TYPEID As String) As Dictionary(Of Long, clsOTDBDependCheck)
            ' empty
            If Not me.isloaded And Not Me.IsCreated Then
                getDependCheckTypeIdList = Nothing
                Exit Function
            End If

            ' add the the typeid list dictionary
            If s_DependCheck_TypeIDs.ContainsKey(key:=TYPEID) Then
                getDependCheckTypeIdList = s_DependCheck_TypeIDs.Item(key:=TYPEID)
                Exit Function
            End If

            getDependCheckTypeIdList = Nothing
        End Function
        '****** creates the TypeIDList of typeid or the existing one
        Private Function CreateDependCheckTypeIdList(TYPEID As String, Optional FORCE As Boolean = False) As Dictionary(Of Long, clsOTDBDependCheck)
            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependCheck)
            Dim anEntry As New clsOTDBDependCheck

            ' empty
            If Not me.isloaded And Not Me.IsCreated Then
                createDependCheckTypeIdList = Nothing
                Exit Function
            End If

            ' add the the typeid list dictionary
            If s_DependCheck_TypeIDs.ContainsKey(key:=TYPEID) And Not FORCE Then
                createDependCheckTypeIdList = s_DependCheck_TypeIDs.Item(key:=TYPEID)
                Exit Function
            End If

            ' create new list with header
            dependFromList = New Dictionary(Of Long, clsOTDBDependCheck)
            anEntry = clsOTDBDependCheck.Create(TYPEID:=TYPEID, PARTID:=Me.PartID, POSNO:=0, UID:=0, UPDC:=0, dependfromPartID:=String.empty)
            If anEntry Is Nothing Then
                anEntry = clsOTDBDependCheck.Retrieve(typeid:=TYPEID, partid:=Me.PartID, posno:=0, uid:=0, updc:=0)
                anEntry.dependfromPartID = String.empty
            End If

            dependFromList.Add(key:=0, value:=anEntry)
            Call ExchangeDependCheckTypeIdList(TYPEID, dependFromList)
            createDependCheckTypeIdList = dependFromList
        End Function
        '*** add a DependCheck
        '***
        Public Function AddDependCheckMember(anEntry As clsOTDBDependCheck) As Boolean
            Dim flag As Boolean
            Dim existEntry As New clsOTDBDependCheck
            Dim aHeadEntry As New clsOTDBDependCheck
            Dim m As Object
            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependCheck)

            ' empty
            If (Not me.isloaded And Not Me.IsCreated) Or (Not anEntry.IsLoaded And Not anEntry.IsCreated) Then
                addDependCheckMember = False
                Exit Function
            End If

            ' get or add
            dependFromList = CreateDependCheckTypeIdList(anEntry.TYPEID)

            ' remove and overwrite
            If dependFromList.ContainsKey(key:=anEntry.Posno) Then
                Call dependFromList.Remove(key:=anEntry.Posno)
            End If
            ' add entry
            dependFromList.Add(key:=anEntry.Posno, value:=anEntry)


            ' add the the typeid list dictionary
            If Not s_DependCheck_TypeIDs.ContainsKey(key:=anEntry.TYPEID) Then
                ' add
                Call s_DependCheck_TypeIDs.Add(key:=anEntry.TYPEID, value:=dependFromList)
            Else
                Call ExchangeDependCheckTypeIdList(TYPEID:=anEntry.TYPEID, dependFromList:=dependFromList)
            End If

            ' change head entry
            aHeadEntry = dependFromList.Item(key:=0)
            'aHeadEntry.isNode = True
            '
            addDependCheckMember = True

        End Function
        '*** add a Member by cls OTDB
        '***
        Public Function AddDependMember(anEntry As clsOTDBDependMember) As Boolean
            Dim flag As Boolean
            Dim existEntry As New clsOTDBDependMember
            Dim aHeadEntry As New clsOTDBDependMember
            Dim m As Object
            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependMember)

            ' empty
            If Not me.isloaded And Not Me.IsCreated And Not anEntry.IsLoaded And Not anEntry.IsCreated Then
                addDependMember = False
                Exit Function
            End If

            ' get or add
            dependFromList = CreateDependMemberTypeIdList(anEntry.TypeID)

            ' remove and overwrite
            If dependFromList.ContainsKey(key:=anEntry.PosNo) Then
                Call dependFromList.Remove(key:=anEntry.PosNo)
            End If
            ' add entry
            dependFromList.Add(key:=anEntry.PosNo, value:=anEntry)


            ' add the the typeid list dictionary
            If Not s_DependMembers_TypeIDs.ContainsKey(key:=anEntry.TypeID) Then
                ' add
                Call s_DependMembers_TypeIDs.Add(key:=anEntry.TypeID, value:=dependFromList)
            Else
                Call ExchangeDependMemberTypeIdList(TYPEID:=anEntry.TypeID, dependFromList:=dependFromList)
            End If

            ' change head entry
            aHeadEntry = dependFromList.Item(key:=0)
            aHeadEntry.isNode = True
            '
            addDependMember = True

        End Function
        '*** returns true if the dependency is a leaf (no sub dependencies)
        '***
        Public Function IsLeaf(typeid As String) As Boolean

            Dim flag As Boolean
            Dim anEntry As New clsOTDBDependMember
            Dim m As Object

            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependMember)

            ' empty
            If Not me.isloaded And Not Me.IsCreated Then
                IsLeaf = False
                Exit Function
            End If

            ' get or add
            dependFromList = CreateDependMemberTypeIdList(typeid)

            ' remove and overwrite
            If Not dependFromList.ContainsKey(key:=0) Then
                'Call dependFromList.Remove(key:=anEntry.posno)
                System.Diagnostics.Debug.WriteLine("upps")
            End If

            ' change head entry
            anEntry = dependFromList.Item(key:=0)
            IsLeaf = anEntry.isLeaf

        End Function

        '**** deleteMemberByTypeId
        '****
        Public Function DeleteMemberByTypeId(typeid As String) As Boolean
            Dim anEntry As New clsOTDBDependMember
            Dim aDependCheckColl As New List(Of clsOTDBDependCheck)

            Dim aDependCheck As New clsOTDBDependCheck
            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependMember)
            Dim Key As Object

            ' get or add
            dependFromList = GetDependMemberTypeIdList(TYPEID)
            If dependFromList Is Nothing Then
                DeleteMemberByTypeId = False
                Exit Function
            End If

            ' delete each entry / each list
            For Each Key In dependFromList.Keys
                anEntry = dependFromList.Item(Key)
                aDependCheckColl = clsOTDBDependCheck.AllByDependMember(anEntry)
                For Each aDependCheck In aDependCheckColl
                    Call aDependCheck.Delete()
                Next aDependCheck
                ' delete the Member
                anEntry.Delete()
            Next Key

            ' reset it
            dependFromList = CreateDependMemberTypeIdList(TYPEID, FORCE:=True)
            Call ExchangeDependMemberTypeIdList(TYPEID, dependFromList)

            'me.iscreated = True
            _IsDeleted = True
            Me.Unload()

        End Function
        '**** delete all lists
        '****
        Public Function delete() As Boolean
            Dim m As Object

            If Not Me.IsCreated And Not me.isloaded Then
                delete = False
                Exit Function
            End If

            ' delete each entry / each list and reset !
            For Each m In Me.typeids
                Call Me.DeleteMemberByTypeId(typeid:=CStr(m))
            Next m

            ' reset it

            'me.iscreated = True
            _IsDeleted = True
            Me.Unload()

        End Function

        '**** Posno
        '****
        Public Function Posno(typeid As String) As Object
            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependMember)

            ' get or add
            dependFromList = getDependMemberTypeIdList(TYPEID)
            If dependFromList Is Nothing Then
                Posno = New Collection
                Exit Function
            End If
            ' get the posno
            Posno = dependFromList.Keys


        End Function



        '**** clusterid returns the clusterid for typeid-list
        '****
        Public Function DynClusterid(ByVal atypeid As String, _
                                        Optional workspaceID As String = String.empty) As String

            Dim anEntry As New clsOTDBDependMember
            Dim anDependCheck As New clsOTDBDependCheck

            Dim aCollection As New Collection
            Dim m As Object
            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependMember)

            ' get or add
            dependFromList = getDependMemberTypeIdList(atypeid)
            If dependFromList Is Nothing Then
                DynClusterid = String.empty
                Exit Function
            End If

            If IsMissing(workspaceID) Then
                workspaceID = CStr(CurrentSession.CurrentWorkspaceID)
            Else
                workspaceID = CStr(workspaceID)
            End If

            ' get the headitm
            If dependFromList.ContainsKey(key:=0) Then
                anEntry = dependFromList.Item(key:=0)
                If Not anEntry Is Nothing Then
                    aCollection = anEntry.getDependCheck(workspaceID)
                    If Not aCollection Is Nothing And Not aCollection.Count Then
                        anDependCheck = aCollection.Item(1)
                        DynClusterid = anDependCheck.clusterid
                        Exit Function
                    End If
                End If
            End If

            DynClusterid = String.empty
        End Function

        '**** clusterid returns the clusterid for typeid-list
        '****
        Public Function Clusterid(ByVal atypeid As String) As String
            Dim anEntry As New clsOTDBDependMember
            Dim aCollection As New Collection
            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependMember)

            ' get or add
            dependFromList = getDependMemberTypeIdList(atypeid)
            If dependFromList Is Nothing Then
                clusterid = String.empty
                Exit Function
            End If

            ' get the headitm
            If dependFromList.ContainsKey(key:=0) Then
                anEntry = dependFromList.Item(key:=0)
                If Not anEntry Is Nothing Then
                    clusterid = anEntry.clusterid
                    Exit Function
                End If
            End If

            clusterid = String.empty
        End Function
        '**** DependChecks returns a Collection of Members for typeid-list
        '****
        Public Function DependChecks(typeid As String) As Collection
            Dim anEntry As New clsOTDBDependCheck
            Dim aCollection As New Collection
            Dim m As Object
            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependCheck)

            ' get or add
            dependFromList = getDependCheckTypeIdList(TYPEID)
            If dependFromList Is Nothing Then
                DependChecks = Nothing
                Exit Function
            End If

            ' delete each entry
            For Each m In dependFromList
                If Not IsEmpty(m) Then
                    anEntry = m
                    If anEntry.Posno <> 0 Then aCollection.Add(anEntry)
                End If
            Next m

            DependChecks = aCollection
        End Function
        '**** Members returns a Collection of Members for typeid-list
        '****
        Public Function Members(typeid As String) As Collection
            Dim anEntry As New clsOTDBDependMember
            Dim aCollection As New Collection
            Dim m As Object
            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependMember)

            ' get or add
            dependFromList = getDependMemberTypeIdList(TYPEID)
            If dependFromList Is Nothing Then
                Members = Nothing
                Exit Function
            End If

            ' delete each entry
            For Each m In dependFromList
                If Not IsEmpty(m) Then
                    anEntry = m
                    If anEntry.PosNo <> 0 Then aCollection.Add(anEntry)
                End If
            Next m

            Members = aCollection
        End Function
        '**** infuese the object by a OTDBRecord
        '****
        Public Function Infuse(ByRef aRecord As ormRecord) As Boolean
            ' not implemented
            infuse = False
        End Function

        '**** Inject : load the object by the PrimaryKeys
        '****
        ''' <summary>
        ''' Load by Dependant to by partid
        ''' </summary>
        ''' <param name="partid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadbyDependant(ByVal partid As String) As Boolean
            Dim aStore As iormDataStore
            Dim aRecordCollection As List(Of ormRecord)
            Dim anEntry As New clsOTDBDependMember

            Try
                aStore = GetTableStore(clsOTDBDependMember.ConstPrimaryTableID)
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(id:="loadByDependant", addAllFields:=True)
                If Not aCommand.Prepared Then
                    aCommand.Where = clsOTDBDependMember.ConstPrimaryTableID & ".[" & clsOTDBDependMember.constFNPartID & "] = @partid"
                    aCommand.Where &= " AND " & clsOTDBDependMember.ConstPrimaryTableID & ".[" & clsOTDBDependMember.constFNNoPos & "] = 0"
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@partid", columnname:=clsOTDBDependMember.constFNPartID, tablename:=clsOTDBDependMember.ConstPrimaryTableID))
                    aCommand.Prepare()
                End If

                aCommand.SetParameterValue(ID:="@partid", value:=partid)

                aRecordCollection = aCommand.RunSelect

                For Each aRecord As ormRecord In aRecordCollection
                    ' add the Entry as Component
                    anEntry = New clsOTDBDependMember
                '    If anEntry.Infuse(aRecord) Then
                '        If Not Me.AddDependMember(anEntry) Then
                '        End If
                '    End If
                Next

                If aRecordCollection.Count > 0 Then
                    Return True
                Else
                    Return False
                End If

            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, subname:="clsOTDBDependency.LoadByDependant")
                Return False
            End Try

        End Function

        '**** Inject : load the object by the PrimaryKeys
        '****
        ''' <summary>
        ''' Loads Dependency outgoing from a partid
        ''' </summary>
        ''' <param name="partid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadbyDependingFrom(ByVal partid As String) As Boolean
            Dim aStore As iormDataStore
            Dim aRecordCollection As List(Of ormRecord)
            Dim anEntry As New clsOTDBDependMember

            Try
                aStore = GetTableStore(clsOTDBDependMember.ConstPrimaryTableID)
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(id:="LoadByDependingFrom", addAllFields:=True)
                If Not aCommand.Prepared Then
                    aCommand.Where = clsOTDBDependMember.ConstPrimaryTableID & ".[" & clsOTDBDependMember.constfndepfromid & "] = @partid"
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@partid", columnname:=clsOTDBDependMember.constFNDepFromId, tablename:=clsOTDBDependMember.ConstPrimaryTableID))
                    aCommand.Prepare()
                End If

                aCommand.SetParameterValue(ID:="@partid", value:=partid)

                aRecordCollection = aCommand.RunSelect

                For Each aRecord As ormRecord In aRecordCollection
                    ' add the Entry as Component
                    anEntry = New clsOTDBDependMember
                    'If anEntry.Infuse(aRecord) Then
                    '    If Not Me.AddDependMember(anEntry) Then
                    '    End If
                    'End If
                Next

                If aRecordCollection.Count > 0 Then
                    Return True
                Else
                    Return False
                End If

            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, subname:="clsOTDBDependency.LoadByDependingFrom")
                Return False
            End Try

        End Function

        '**** persistbyTypeId
        '****

        Public Function PersistByTypeID(typeid As String) As Boolean
            Dim anEntry As New clsOTDBDependMember
            Dim headentry As New clsOTDBDependMember
            Dim aLeaf As New clsOTDBDependMember
            Dim maxcarused As Integer
            Dim maxposno As Integer
            Dim status As String

            Dim aTimestamp As Date
            Dim i As Integer
            Dim Key As Object
            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependMember)

            ' get or add
            dependFromList = GetDependMemberTypeIdList(TYPEID)
            If dependFromList Is Nothing Then
                PersistByTypeID = False
                Exit Function
            End If

            ' set Timestamp
            aTimestamp = Now

            ' each entry -> koeff -> Should be moved
            '
            If TYPEID = ConstDepTypeIDIFC Then
                maxcarused = 0
                maxposno = 0
                status = String.empty
                headentry = Nothing
                For Each Key In dependFromList.Keys
                    anEntry = dependFromList.Item(Key)
                    If (anEntry.PosNo = 0 And anEntry.nopos = 0) Or anEntry.isNode Then
                        headentry = anEntry
                    Else
                        If maxcarused <= anEntry.parameter_num2 Then
                            maxcarused = anEntry.parameter_num2
                        End If
                        If maxposno <= anEntry.PosNo Then
                            maxposno = anEntry.PosNo
                        End If
                        '** HACK !
                        If anEntry.parameter_txt1 <> "r2" And status = "r2" Then
                            ' do nothing
                        ElseIf anEntry.parameter_txt1 = "r1" And status <> "r2" Then
                            status = "r2"
                        ElseIf anEntry.parameter_txt1 = "y1" And (status <> "r2" And status <> "y1") Then
                            status = "y1"
                        Else
                            status = anEntry.parameter_txt1
                        End If

                        'create for each leave an entry
                        aLeaf = New clsOTDBDependMember
                        If aLeaf.Create(typeid:=typeid, partid:=anEntry.dependfromPartID, posno:=0) Then
                            aLeaf.category = "leaf"
                            aLeaf.isLeaf = True
                            aLeaf.isNode = False
                            aLeaf.condition = "IFC2"
                            Call aLeaf.Persist()
                        End If
                    End If
                Next Key
                If Not headentry Is Nothing Then
                    headentry.parameter_num1 = maxposno
                    headentry.parameter_num2 = maxcarused
                    headentry.parameter_txt1 = status
                    headentry.nopos = dependFromList.Count - 1
                    headentry.isNode = True
                    headentry.isLeaf = False
                    headentry.condition = "IFC3"
                    headentry.TypeID = TYPEID
                    headentry.category = "head"
                End If
            End If

            ' save each entry
            For Each Key In dependFromList.Keys
                anEntry = dependFromList.Item(Key)
                anEntry.Persist(aTimestamp)
            Next Key

            PersistByTypeID = True

            Exit Function

errorhandle:

            PersistByTypeID = False

        End Function

        ''' <summary>
        ''' Persists the dependency object
        ''' </summary>
        ''' <param name="timestamp"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Persist(Optional timestamp As Date = ot.ConstNullDate) As Boolean
            Dim atypeid As Object

            '**
            If Not Me.IsCreated And Not me.isloaded Then
                Persist = False
                Exit Function
            End If

            ' delete each entry
            For Each atypeid In Me.typeids
                Me.persistByTypeID(atypeid)
            Next atypeid

            Return True

        End Function

        '**** create : create a new Object with primary keys
        '****
        ''' <summary>
        ''' Create a Dependency persistable
        ''' </summary>
        ''' <param name="pnid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Create(ByVal pnid As String) As clsOTDBDependCheck
            Return Create(typeid:=String.empty, pnid:=pnid, Posno:=0)
        End Function

        '**** runCheck runs through the DependCheck run
        '****
        Public Function RunCheck(ByVal typeid As String, _
                                 Optional ByVal workspaceID As String = String.empty, _
                                 Optional ByVal autopersist As Boolean = False) As Boolean
            Dim anEntry As New clsOTDBDependMember
            Dim headentry As New clsOTDBDependMember
            Dim maxcarused As Integer
            Dim maxposno As Integer
            Dim status As String

            Dim aTimestamp As Date
            Dim i As Integer
            Dim Key As Object
            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependMember)
            Dim aDependCheck As New clsOTDBDependCheck
            Dim aDepCheckColl As New Collection
            Dim dependCheckFromlist As New Dictionary(Of Long, clsOTDBDependCheck)



            ' get or add
            dependFromList = GetDependMemberTypeIdList(typeid)
            If dependFromList Is Nothing Then
                RunCheck = False
                Exit Function
            End If

            ' set Timestamp
            aTimestamp = Now

            ' each entry -> koeff -> Should be moved
            '
            If typeid = ConstDepTypeIDIFC Then
                maxcarused = 0
                maxposno = 0
                status = String.empty
                headentry = Nothing
                For Each Key In dependFromList.Keys
                    anEntry = dependFromList.Item(Key)
                    If anEntry.PosNo = 0 Then
                        headentry = anEntry
                    Else
                        'run the check
                        aDependCheck = New clsOTDBDependCheck
                        If aDependCheck.run(anEntry, workspaceID, autopersist:=False) Then

                            ' Persist
                            If autopersist And aDependCheck.IsCreated Then aDependCheck.Persist(aTimestamp)
                            ' get status upgrades
                            If status = OTDBConst_DependStatus_r3 Then
                                ' do nothing
                            ElseIf status = OTDBConst_DependStatus_r2 And _
                                   aDependCheck.status = OTDBConst_DependStatus_r3 Then
                                status = aDependCheck.status
                            ElseIf status = OTDBConst_DependStatus_r1 And ( _
                                   aDependCheck.status = OTDBConst_DependStatus_r3 Or aDependCheck.status = OTDBConst_DependStatus_r2) Then
                                status = aDependCheck.status
                            ElseIf status = OTDBConst_DependStatus_y1 And ( _
                                   aDependCheck.status = OTDBConst_DependStatus_r3 Or aDependCheck.status = OTDBConst_DependStatus_r2 Or aDependCheck.status = OTDBConst_DependStatus_r1 _
                                   ) Then
                                status = aDependCheck.status
                            ElseIf Not LCase(status) Like "r*" And Not LCase(status) Like "y*" Then
                                status = aDependCheck.status
                            End If
                            ' add Collection
                            Call Me.AddDependCheckMember(aDependCheck)
                            If Not aDependCheck.status Like "g*" And (aDependCheck.IsCreated Or aDependCheck.IsLoaded) Then
                                Call aDepCheckColl.Add(aDependCheck)
                            End If
                        End If
                    End If
                Next Key
            End If

            ' generate TopLevel Status
            aDependCheck = New clsOTDBDependCheck
            If autopersist Then
                If Not aDependCheck.Retrieve(typeid:=headentry.TypeID, partid:=headentry.PartID, _
                                           posno:=headentry.PosNo, uid:=0, updc:=0) Then
                    Call aDependCheck.create(TYPEID:=headentry.TypeID, PARTID:=headentry.PartID, _
                                             POSNO:=headentry.PosNo, UID:=0, UPDC:=0)
                End If
                aDependCheck.status = status
                aDependCheck.Persist()

            End If

            ' save it to local list
            dependCheckFromlist = GetDependCheckTypeIdList(typeid)
            If Not dependCheckFromlist Is Nothing Then
                If dependCheckFromlist.ContainsKey(key:=0) Then
                    aDependCheck = dependCheckFromlist.Item(key:=0)
                    If Not aDependCheck Is Nothing Then
                        aDependCheck.status = status
                    End If
                Else
                    aDependCheck.status = String.empty
                End If
            End If

            RunCheck = True

            Exit Function
        End Function
        ''' <summary>
        ''' retrieves the Status of the dependency structure
        ''' </summary>
        ''' <param name="typeid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Status(typeid As String) As String
            Dim aDependCheck As New clsOTDBDependCheck
            Dim dependCheckFromlist As Dictionary(Of Long, clsOTDBDependCheck)

            dependCheckFromlist = GetDependCheckTypeIdList(TYPEID)
            If Not dependCheckFromlist Is Nothing Then
                aDependCheck = dependCheckFromlist.Item(key:=0)
                Status = aDependCheck.status
                Exit Function
            End If

            Status = String.empty
        End Function

        '**** lastStatus
        '****

        Public Function GetlastStatus(TYPEID As String) As String
            Dim aDependCheck As New clsOTDBDependCheck

            ' get TopLevel Status
            If aDependCheck.Retrieve(typeid:=TYPEID, partid:=Me.PartID, _
                                   posno:=0, uid:=0, updc:=0) Then

                GetlastStatus = aDependCheck.status
                Exit Function
            End If

            getlastStatus = String.empty
        End Function

        '**** unionClusters
        '****
        Public Function UnionClusters(ByVal atypeid As String, _
                                      ByVal aClusterID As String, _
                                      ByVal aNotherClusterID As String, _
                                      Optional isDynamic As Boolean = False) As Boolean
            Dim aTable As iormDataStore
            Dim anEntry As New clsOTDBDependMember
            Dim anDepCheck As New clsOTDBDependCheck

            aTable = GetTableStore(anEntry.primaryTableID)

            If isDynamic Then
                unionClusters = aTable.RunSqlStatement("update " & anDepCheck.primaryTableID & " set clusterid = '" & aClusterID & "' where clusterid = '" & aNotherClusterID & "'")
            Else
                unionClusters = aTable.RunSqlStatement("update " & anEntry.primaryTableID & " set clusterid = '" & aClusterID & "' where clusterid = '" & aNotherClusterID & "'")
            End If

        End Function

        '**** clearAllClusters
        '****
        Public Function ClearAllClusters(ByVal atypeid As String, _
                                         Optional ByVal aClusterID As String = String.empty, _
                                         Optional isDynamic As Boolean = False) As Boolean
            Dim aTable As iormDataStore
            Dim anEntry As New clsOTDBDependMember
            Dim cmdstr As String
            Dim anDepCheck As New clsOTDBDependCheck

            aTable = GetTableStore(anEntry.primaryTableID)
            If isDynamic Then
                cmdstr = "update " & anDepCheck.primaryTableID & " set clusterid = '', clusterlevel=0 "
            Else
                cmdstr = "update " & anEntry.primaryTableID & " set clusterid = '', clusterlevel=0 "
            End If

            If aClusterID <> String.empty Then
                cmdstr = cmdstr & " where clusterid = '" & aClusterID & "'"
            End If
            clearAllClusters = aTable.RunSqlStatement(cmdstr)

        End Function
        '**** generateCluster runs through the DependCheck run
        '****
        Public Function GenerateCluster(ByVal atypeid As String, _
                                        ByVal aClusterID As String, ByVal aLevel As Long) As Boolean
            Dim anEntry As New clsOTDBDependMember
            Dim anSubHead As New clsOTDBDependMember
            Dim aTimestamp As Date
            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependMember)
            Dim aDependCheck As New clsOTDBDependCheck
            Dim aDependency As New clsOTDBDependency
            Dim Key As Object
            Dim keys As Object

            ' get or add
            dependFromList = GetDependMemberTypeIdList(atypeid)
            If dependFromList Is Nothing Then
                generateCluster = False
                Exit Function
            End If

            ' set Timestamp
            aTimestamp = Now
            keys = dependFromList.Keys
            ' go thorugh each
            For Each Key In keys
                ' better reload
                If anEntry.Retrieve(atypeid, Me.PartID, posno:=Key) Then
                    System.Diagnostics.Debug.WriteLine(aClusterID, aLevel, Me.PartID & " -> " & anEntry.dependfromPartID)
                    'Set anEntry = dependFromList.Item(key)
                    ' check the head -> we have been here already !
                    If (anEntry.PosNo = 0 And anEntry.nopos <> 0) Or anEntry.isNode Then
                        ' do nothing
                        If anEntry.clusterid = String.empty Then
                            anEntry.clusterid = aClusterID
                            anEntry.clusterlevel = aLevel
                            Call anEntry.Persist()    '-> persist
                        ElseIf anEntry.clusterid = aClusterID Then
                            ' what to do on short cycle ?!
                            'If anEntry.clusterlevel < aLevel Then
                            '    anEntry.clusterlevel = aLevel
                            '    Call anEntry.persist
                            'End If
                            generateCluster = True
                            Exit Function
                        ElseIf anEntry.clusterid <> aClusterID Then
                            Call MarkClusterID(atypeid, anEntry.clusterid, aClusterID)

                            If anEntry.clusterlevel < aLevel Then
                                anEntry.clusterlevel = aLevel
                                Call anEntry.Persist()
                            End If
                            generateCluster = True
                            Exit Function
                        End If
                        ' member entry
                        '
                    Else
                        ' has the Entry a cluster ?!
                        If anEntry.clusterid = String.empty Then
                            anEntry.clusterid = aClusterID
                            anEntry.clusterlevel = aLevel
                            Call anEntry.Persist()    '-> persist
                            'if the Entry has Children mark these too
                            If aDependency.LoadbyDependant(anEntry.dependfromPartID) Then
                                ' more than a headentry
                                If aDependency.NoMembers(atypeid) > 0 Then
                                    'Debug.Print aClusterID, aLevel, Me.partid & " -> " & anEntry.dependfromPartID
                                    GenerateCluster = aDependency.GenerateCluster(atypeid:=atypeid, aClusterID:=aClusterID, aLevel:=aLevel + 1)
                                Else
                                    ' a leaf
                                    If anSubHead.Retrieve(atypeid, partid:=anEntry.dependfromPartID, posno:=0) Then
                                        If anSubHead.clusterid = String.empty Then
                                            anSubHead.clusterid = aClusterID
                                            anSubHead.clusterlevel = aLevel
                                            Call anSubHead.Persist()    '-> persist
                                        ElseIf anSubHead.clusterid <> aClusterID Then
                                            Call MarkClusterID(atypeid, anSubHead.clusterid, aClusterID)

                                        End If
                                        If anSubHead.clusterlevel < aLevel Then
                                            anSubHead.clusterlevel = aLevel
                                            Call anSubHead.Persist()
                                        End If

                                    End If
                                End If
                            Else
                                ' no children -> fine
                            End If


                        ElseIf anEntry.clusterid <> aClusterID Then
                            ' mark it as same cluster
                            Call MarkClusterID(atypeid, anEntry.clusterid, aClusterID)
                            If anSubHead.Retrieve(atypeid, partid:=anEntry.dependfromPartID, posno:=0) Then
                                If anSubHead.clusterid <> aClusterID Then
                                    Call MarkClusterID(atypeid, anSubHead.clusterid, aClusterID)
                                End If
                                If anSubHead.clusterlevel < aLevel Then
                                    anSubHead.clusterlevel = aLevel
                                    Call anSubHead.Persist()
                                End If
                            End If

                        ElseIf anEntry.clusterid = aClusterID Then
                            ' do nothing we have been here already
                        End If

                    End If    ' head or entry

                End If    'loaded
            Next Key

            '
            generateCluster = True

            Exit Function
        End Function


        '**** generateDynCluster runs through the DependCheck run
        '****
        Public Function GenerateDynCluster(ByVal typeid As String, ByVal clusterid As String, ByVal level As Long, Optional workspaceID As String = String.empty) As Boolean
            Dim anEntry As New clsOTDBDependMember
            Dim anSubHead As New clsOTDBDependMember
            Dim aTimestamp As Date
            Dim dependFromList As New Dictionary(Of Long, clsOTDBDependMember)
            Dim aDependCheck As New clsOTDBDependCheck
            Dim aSubDependCheck As New clsOTDBDependCheck
            Dim aDCColl As New Collection
            Dim aDependency As New clsOTDBDependency
            Dim Key As Object
            Dim keys As Object

            ' get or add
            dependFromList = GetDependMemberTypeIdList(typeid)
            If dependFromList Is Nothing Then
                GenerateDynCluster = False
                Exit Function
            End If
            ' workspaceID
            If IsMissing(workspaceID) Then
                workspaceID = CStr(CurrentSession.CurrentWorkspaceID)
            Else
                workspaceID = CStr(workspaceID)
            End If

            ' set Timestamp
            aTimestamp = Now
            keys = dependFromList.Keys
            ' go thorugh each
            For Each Key In keys
                ' better reload
                If anEntry.Retrieve(typeid:=typeid, partid:=Me.PartID, posno:=Key) Then
                    ' Get the Dependency Check
                    aDCColl = anEntry.GetDependCheck(workspaceID)
                    ' run or check
                    If aDCColl Is Nothing Or aDCColl.Count = 0 Then
                        If Me.RunCheck(typeid, workspaceID:=workspaceID, autopersist:=True) Then
                            aDCColl = anEntry.GetDependCheck(workspaceID)
                            If aDCColl Is Nothing Or aDCColl.Count = 0 Then
                                GenerateDynCluster = False
                                Exit Function
                            End If
                        Else
                            GenerateDynCluster = False
                            Exit Function
                        End If
                    End If
                    ' get the DependCheckItem
                    aDependCheck = aDCColl.Item(1)

                    System.Diagnostics.Debug.WriteLine(aDependCheck.status, clusterid, level, Me.PartID & " -> " & anEntry.dependfromPartID)

                    '**
                    '** check it even if we have not a green status -> red edges leading to green nodes are ok (the green nodes)
                    '**

                    '** check the head -> we have been here already !
                    '**
                    If (anEntry.PosNo = 0 And anEntry.nopos <> 0) Or anEntry.isNode Then
                        ' do nothing
                        If aDependCheck.clusterid = String.empty Then
                            aDependCheck.clusterid = clusterid
                            aDependCheck.clusterlevel = level
                            Call aDependCheck.Persist()    '-> persist
                        ElseIf aDependCheck.clusterid = clusterid Then
                            ' what to do on short cycle ?!
                            'If anEntry.clusterlevel < aLevel Then
                            '    anEntry.clusterlevel = aLevel
                            '    Call anEntry.persist
                            'End If
                            GenerateDynCluster = True
                            Exit Function
                        ElseIf aDependCheck.clusterid <> clusterid Then
                            Call MarkClusterID(typeid, aDependCheck.clusterid, clusterid)
                            If aDependCheck.clusterlevel < level Then
                                aDependCheck.clusterlevel = level
                                Call aDependCheck.Persist()
                            End If
                            GenerateDynCluster = True
                            Exit Function
                        End If

                        '** member entry -> only if not green
                        '**
                    ElseIf Not aDependCheck.status Like "g*" Then

                        ' has the Entry a cluster ?!
                        If aDependCheck.clusterid = String.empty Then
                            aDependCheck.clusterid = clusterid
                            aDependCheck.clusterlevel = level
                            Call aDependCheck.Persist()    '-> persist
                            'if the Entry has Children mark these too
                            If aDependency.LoadbyDependant(anEntry.dependfromPartID) Then
                                ' more than a headentry
                                If aDependency.NoMembers(typeid) > 0 Then
                                    'Debug.Print aClusterID, aLevel, Me.partid & " -> " & anEntry.dependfromPartID
                                    GenerateDynCluster = aDependency.GenerateDynCluster(typeid:=typeid, clusterid:=clusterid, level:=level + 1, workspaceID:=workspaceID)
                                Else
                                    '*a leaf
                                    '*
                                    If anSubHead.Retrieve(typeid, partid:=anEntry.dependfromPartID, posno:=0) Then
                                        ' create or get
                                        aDCColl = anSubHead.GetDependCheck(workspaceID)
                                        If aDCColl Is Nothing Or aDCColl.Count = 0 Then
                                            aSubDependCheck = New clsOTDBDependCheck
                                            Call aSubDependCheck.create(TYPEID:=typeid, PARTID:=anEntry.dependfromPartID, POSNO:=0, UID:=0, UPDC:=0)
                                        Else
                                            aSubDependCheck = aDCColl.Item(1)
                                        End If
                                        'check
                                        If aSubDependCheck.clusterid = String.empty Then
                                            aSubDependCheck.clusterid = clusterid
                                            aSubDependCheck.clusterlevel = level
                                            Call aSubDependCheck.Persist()    '-> persist
                                        ElseIf aSubDependCheck.clusterid <> clusterid Then
                                            Call MarkClusterID(typeid, anSubHead.clusterid, clusterid)
                                            '  Debug.Print "*"
                                        End If
                                        If aSubDependCheck.clusterlevel < level Then
                                            aSubDependCheck.clusterlevel = level
                                            Call aSubDependCheck.Persist()
                                        End If

                                    End If
                                End If
                            Else
                                ' no children -> fine
                            End If    ' node or leaf of member


                        ElseIf aDependCheck.clusterid <> clusterid Then
                            ' mark it as same cluster
                            Call MarkClusterID(typeid, anEntry.clusterid, clusterid)
                            If anSubHead.Retrieve(typeid, partid:=anEntry.dependfromPartID, posno:=0) Then
                                aDCColl = anSubHead.GetDependCheck(workspaceID)
                                If aDCColl Is Nothing Or aDCColl.Count = 0 Then
                                    aSubDependCheck = New clsOTDBDependCheck
                                    Call aSubDependCheck.create(TYPEID:=typeid, PARTID:=anEntry.dependfromPartID, POSNO:=0, UID:=0, UPDC:=0)
                                Else
                                    aSubDependCheck = aDCColl.Item(1)
                                End If

                                If aSubDependCheck.clusterid <> clusterid Then
                                    Call MarkClusterID(typeid, aSubDependCheck.clusterid, clusterid)
                                End If
                                If aSubDependCheck.clusterlevel < level Then
                                    aSubDependCheck.clusterlevel = level
                                    Call aSubDependCheck.Persist()
                                End If

                            End If
                        ElseIf aDependCheck.clusterid = clusterid Then
                            ' do nothing we have been here already
                        End If    ' types


                    End If    'head or entry

                End If    ' load of entry
            Next Key

            '
            GenerateDynCluster = True

            Exit Function
        End Function


    End Class

    '************************************************************************************
    '***** CLASS clsOTDBDependMember is a helper for the Dependend Parts
    '*****
    '*****

    Public Class clsOTDBDependMember
        Inherits ormDataObject
        Implements iormInfusable
        Implements iormPersistable

        Public Const ConstPrimaryTableID = "tblPartDepends"

        Public Const constFNPartID = "pnid"
        Public Const constFNPosno = "posno"
        Public Const constFNNoPos = "nopos"
        Public Const constFNDepFromId = "depfromid"
        Public Const constFNTypeid = "typeid"

        Private s_partID As String    ' Assy ID
        Private s_dependfrompartid As String    ' Component ID
        Private s_posno As Long
        Private s_condition As String
        Private s_typeid As String
        Private s_category As String
        Private s_nopos As Long
        Private s_parameter_txt1 As String
        Private s_parameter_txt2 As String
        Private s_parameter_txt3 As String
        Private s_parameter_num1 As Double
        Private s_parameter_num2 As Double
        Private s_parameter_num3 As Double
        Private s_parameter_date1 As Date
        Private s_parameter_date2 As Date
        Private s_parameter_date3 As Date
        Private s_parameter_flag1 As Boolean
        Private s_parameter_flag2 As Boolean
        Private s_parameter_flag3 As Boolean

        Private s_clusterid As String
        Private s_clusterlevel As Long

        Private s_isleaf As Boolean
        Private s_isnode As Boolean

#Region "Properties"


        ReadOnly Property PartID() As String
            Get
                PARTID = s_partID
            End Get

        End Property
        ReadOnly Property PosNo() As Long
            Get
                PosNo = s_posno
            End Get
        End Property


        Public Property nopos() As Long
            Get
                nopos = s_nopos
            End Get
            Set(value As Long)
                If value <> s_nopos Then
                    s_nopos = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        Public Property isNode() As Boolean
            Get
                isNode = s_isnode
            End Get
            Set(value As Boolean)
                If value <> s_isnode Then
                    s_isnode = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        Public Property isLeaf() As Boolean
            Get
                isLeaf = s_isleaf
            End Get
            Set(value As Boolean)
                If value <> s_isleaf Then
                    s_isleaf = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        Public Property dependfromPartID() As String
            Get
                dependfromPartID = s_dependfrompartid
            End Get
            Set(value As String)
                If s_dependfrompartid <> value Then
                    s_dependfrompartid = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        Public Property condition() As String
            Get
                condition = s_condition
            End Get
            Set(value As String)
                If s_condition <> value Then
                    s_condition = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        Public Property category() As String
            Get
                category = s_category
            End Get
            Set(value As String)
                If value <> s_category Then
                    s_category = value
                    Me.IsChanged = True
                End If
            End Set
        End Property


        Public Property TypeID() As String
            Get
                TypeID = s_typeid
            End Get
            Set(value As String)
                s_typeid = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property clusterid() As String
            Get
                clusterid = s_clusterid
            End Get
            Set(value As String)
                s_clusterid = value
                Me.IsChanged = True
            End Set
        End Property


        Public Property clusterlevel() As Long
            Get
                clusterlevel = s_clusterlevel
            End Get
            Set(value As Long)
                If value <> s_clusterlevel Then
                    s_clusterlevel = value
                    Me.IsChanged = True
                End If
            End Set
        End Property


        Public Property parameter_num1() As Double
            Get
                parameter_num1 = s_parameter_num1
            End Get
            Set(value As Double)
                If s_parameter_num1 <> value Then
                    s_parameter_num1 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_num2() As Double
            Get
                parameter_num2 = s_parameter_num2
            End Get
            Set(value As Double)
                If s_parameter_num2 <> value Then
                    s_parameter_num2 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_num3() As Double
            Get
                parameter_num3 = s_parameter_num3
            End Get
            Set(value As Double)
                If s_parameter_num3 <> value Then
                    s_parameter_num3 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_date1() As Date
            Get
                parameter_date1 = s_parameter_date1
            End Get
            Set(value As Date)
                If s_parameter_date1 <> value Then
                    s_parameter_date1 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_date2() As Date
            Get
                parameter_date2 = s_parameter_date2
            End Get
            Set(value As Date)
                If s_parameter_date2 <> value Then
                    s_parameter_date2 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_date3() As Date
            Get
                parameter_date3 = s_parameter_date3
            End Get
            Set(value As Date)
                s_parameter_date3 = value
                Me.IsChanged = True
            End Set
        End Property
        Public Property parameter_flag1() As Boolean
            Get
                parameter_flag1 = s_parameter_flag1
            End Get
            Set(value As Boolean)
                If s_parameter_flag1 <> value Then
                    s_parameter_flag1 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_flag3() As Boolean
            Get
                parameter_flag3 = s_parameter_flag3
            End Get
            Set(value As Boolean)
                If s_parameter_flag3 <> value Then
                    s_parameter_flag3 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_flag2() As Boolean
            Get
                parameter_flag2 = s_parameter_flag2
            End Get
            Set(value As Boolean)
                If s_parameter_flag2 <> value Then
                    s_parameter_flag2 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_txt1() As String
            Get
                parameter_txt1 = s_parameter_txt1
            End Get
            Set(value As String)
                If s_parameter_txt1 <> value Then
                    s_parameter_txt1 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_txt2() As String
            Get
                parameter_txt2 = s_parameter_txt2
            End Get
            Set(value As String)
                If s_parameter_txt2 <> value Then
                    s_parameter_txt2 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_txt3() As String
            Get
                parameter_txt3 = s_parameter_txt3
            End Get
            Set(value As String)
                If s_parameter_txt3 <> value Then
                    s_parameter_txt3 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
#End Region
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            Call MyBase.New(ConstPrimaryTableID)
            s_nopos = 0
        End Sub

        ''' <summary>
        ''' Infuses a DependMember by record
        ''' </summary>
        ''' <param name="aRecord"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Public Overrides Function Infuse(ByRef record As ormRecord) As Boolean


        '    Try
        '        s_partID = CStr(record.GetValue(constFNPartID))
        '        s_dependfrompartid = CStr(record.GetValue("depfromid"))
        '        s_posno = CLng(record.GetValue("posno"))
        '        s_condition = CStr(record.GetValue("cond"))
        '        s_typeid = CStr(record.GetValue("typeid"))
        '        s_category = CStr(record.GetValue("cat"))
        '        If Not IsNull(record.GetValue("clusterid")) Then
        '            s_clusterid = CStr(record.GetValue("clusterid"))
        '        Else
        '            s_clusterid = String.empty
        '        End If
        '        If Not IsNull(record.GetValue("clusterlevel")) Then
        '            s_clusterlevel = CLng(record.GetValue("clusterlevel"))
        '        Else
        '            s_clusterlevel = 0
        '        End If
        '        If Not IsNull(record.GetValue("nopos")) Then
        '            s_nopos = CLng(record.GetValue("nopos"))
        '        Else
        '            s_nopos = 0
        '        End If
        '        If Not IsNull(record.GetValue("isleaf")) Then
        '            s_isleaf = CBool(record.GetValue("isleaf"))
        '        Else
        '            s_nopos = False
        '        End If
        '        If Not IsNull(record.GetValue("isnode")) Then
        '            s_isnode = CBool(record.GetValue("isnode"))
        '        Else
        '            s_nopos = False
        '        End If
        '        s_parameter_txt1 = CStr(record.GetValue("param_txt1"))
        '        s_parameter_txt2 = CStr(record.GetValue("param_txt2"))
        '        s_parameter_txt3 = CStr(record.GetValue("param_txt3"))
        '        s_parameter_num1 = CDbl(record.GetValue("param_num1"))
        '        s_parameter_num2 = CDbl(record.GetValue("param_num2"))
        '        s_parameter_num3 = CDbl(record.GetValue("param_num3"))
        '        s_parameter_date1 = CDate(record.GetValue("param_date1"))
        '        s_parameter_date2 = CDate(record.GetValue("param_date2"))
        '        s_parameter_date3 = CDate(record.GetValue("param_date3"))
        '        s_parameter_flag1 = CBool(record.GetValue("param_flag1"))
        '        s_parameter_flag2 = CBool(record.GetValue("param_flag2"))
        '        s_parameter_flag3 = CBool(record.GetValue("param_flag3"))

        '        Return MyBase.Infuse(record)


        '    Catch ex As Exception
        '        CoreMessageHandler(exception:=ex, subname:="clsOTDBDEpendMember.Infuse")
        '        Return False
        '    End Try


        'End Function

        '**** Inject : load the object by the PrimaryKeys
        '****
        ''' <summary>
        ''' Loads a Depend Member by Primary Key
        ''' </summary>
        ''' <param name="TYPEID"></param>
        ''' <param name="PARTID"></param>
        ''' <param name="POSNO"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal typeid As String, ByVal partid As String, ByVal posno As Long) As clsOTDBDependMember
            Dim pkarry() As Object = {typeid, partid, posno}
            Return ormDataObject.Retrieve(Of clsOTDBDependMember)(pkArray:=pkarry)
        End Function

        '**** allHeadsByTypeID returns all Dependency Heads by TypeID
        '****
        ''' <summary>
        ''' retrieves a collection of head members by dependency typeid
        ''' </summary>
        ''' <param name="typeid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function allHeadsByTypeID(ByVal typeid As String) As Collection
            Dim aCollection As New Collection
            Dim aRecordCollection As List(Of ormRecord)
            Dim aTable As iormDataStore
            Dim aRecord As ormRecord
            Dim wherestr As String
            Dim i As Integer
            Dim innerjoin As String
            Dim orderby As String
            Dim aNewDepend As clsOTDBDependMember
            Dim aDir As New Dictionary(Of String, clsOTDBDependMember)


            ' wherestr
            wherestr = " posno = 0 and nopos <> 0 and typeid = '" & TYPEID & "'"
            ' orderby
            orderby = " param_num1 desc, param_num2 desc "
            ' inner join
            innerjoin = String.empty
            'Debug.Print wherestr

            On Error GoTo error_handler

            aTable = GetTableStore(ConstPrimaryTableID)
            aRecordCollection = aTable.GetRecordsBySql(wherestr:=wherestr, orderby:=orderby, innerjoin:=innerjoin, silent:=True)

            If aRecordCollection Is Nothing Then
                Me.Unload()
                allHeadsByTypeID = Nothing
                Exit Function
            Else
                For Each aRecord In aRecordCollection
                    aNewDepend = New clsOTDBDependMember
                    'If aNewDepend.Infuse(aRecord) Then
                    '    If Not aDir.ContainsKey(aNewDepend.PartID) Then
                    '        aCollection.Add(Item:=aNewDepend)
                    '        aDir.Add(key:=aNewDepend.PartID, value:=aNewDepend)
                    '    End If
                    'End If
                Next aRecord
                allHeadsByTypeID = aCollection
                Exit Function
            End If

error_handler:

            allHeadsByTypeID = Nothing
            Exit Function
        End Function


        ''' <summary>
        '''  create static persistency schema
        ''' </summary>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateSchema(Optional silent As Boolean = True) As Boolean

            '            Dim aFieldDesc As New ormFieldDescription
            '            Dim PrimaryColumnNames As New Collection
            '            Dim aTable As New ObjectDefinition
            '            Dim UsedKeyColumnNames As New Collection
            '            Dim ClusterColumnNames As New Collection

            '            aFieldDesc.Relation = New String() {}
            '            aFieldDesc.Size = 0
            '            aFieldDesc.Parameter = String.empty
            '            aFieldDesc.Tablename = ConstPrimaryTableID

            '            With aTable
            '                .Create(ConstPrimaryTableID)
            '                .Delete()


            '                ' typeid
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "type of dependencies"
            '                aFieldDesc.ColumnName = constfntypeid
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                PrimaryColumnNames.Add(aFieldDesc.ColumnName)
            '                ClusterColumnNames.Add(aFieldDesc.ColumnName)

            '                'component id
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "dependend from part-id"
            '                aFieldDesc.ColumnName = constFNDepFromId
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                UsedKeyColumnNames.Add(aFieldDesc.ColumnName)

            '                aFieldDesc.Title = "part-id"
            '                aFieldDesc.ColumnName = constFNPartID
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                PrimaryColumnNames.Add(aFieldDesc.ColumnName)
            '                UsedKeyColumnNames.Add(aFieldDesc.ColumnName)

            '                'Position
            '                aFieldDesc.Datatype = otFieldDataType.[Long]
            '                aFieldDesc.Title = "posno"
            '                aFieldDesc.ColumnName = "posno"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                PrimaryColumnNames.Add(aFieldDesc.ColumnName)

            '                'no Position
            '                aFieldDesc.Datatype = otFieldDataType.[Long]
            '                aFieldDesc.Title = "number of positions "
            '                aFieldDesc.ColumnName = "nopos"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' condition
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "condition"
            '                aFieldDesc.ColumnName = "cond"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' categorie
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "categorie"
            '                aFieldDesc.ColumnName = "cat"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_txt 1
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "parameter_txt 1 of condition"
            '                aFieldDesc.ColumnName = "param_txt1"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_txt 2
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "parameter_txt 2 of condition"
            '                aFieldDesc.ColumnName = "param_txt2"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                ' parameter_txt 2
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "parameter_txt 3 of condition"
            '                aFieldDesc.ColumnName = "param_txt3"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_num 1
            '                aFieldDesc.Datatype = otFieldDataType.Numeric
            '                aFieldDesc.Title = "parameter numeric 1 of condition"
            '                aFieldDesc.ColumnName = "param_num1"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_num 2
            '                aFieldDesc.Datatype = otFieldDataType.Numeric
            '                aFieldDesc.Title = "parameter numeric 2 of condition"
            '                aFieldDesc.ColumnName = "param_num2"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_num 2
            '                aFieldDesc.Datatype = otFieldDataType.Numeric
            '                aFieldDesc.Title = "parameter numeric 3 of condition"
            '                aFieldDesc.ColumnName = "param_num3"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_date 1
            '                aFieldDesc.Datatype = otFieldDataType.[Date]
            '                aFieldDesc.Title = "parameter date 1 of condition"
            '                aFieldDesc.ColumnName = "param_date1"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_date 2
            '                aFieldDesc.Datatype = otFieldDataType.[Date]
            '                aFieldDesc.Title = "parameter date 2 of condition"
            '                aFieldDesc.ColumnName = "param_date2"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_date 3
            '                aFieldDesc.Datatype = otFieldDataType.[Date]
            '                aFieldDesc.Title = "parameter date 3 of condition"
            '                aFieldDesc.ColumnName = "param_date3"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_flag 1
            '                aFieldDesc.Datatype = otFieldDataType.Bool
            '                aFieldDesc.Title = "parameter flag 1 of condition"
            '                aFieldDesc.ColumnName = "param_flag1"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_flag 2
            '                aFieldDesc.Datatype = otFieldDataType.Bool
            '                aFieldDesc.Title = "parameter flag 2 of condition"
            '                aFieldDesc.ColumnName = "param_flag2"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_flag 3
            '                aFieldDesc.Datatype = otFieldDataType.Bool
            '                aFieldDesc.Title = "parameter flag 3 of condition"
            '                aFieldDesc.ColumnName = "param_flag3"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' cluster
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "cluster id"
            '                aFieldDesc.ColumnName = "clusterid"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                ClusterColumnNames.Add(aFieldDesc.ColumnName)

            '                ' clusterlevel
            '                aFieldDesc.Datatype = otFieldDataType.[Long]
            '                aFieldDesc.Title = "cluster level"
            '                aFieldDesc.ColumnName = "clusterlevel"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                ClusterColumnNames.Add(aFieldDesc.ColumnName)
            '                ClusterColumnNames.Add(constFNPartID)

            '                ' isLeaf
            '                aFieldDesc.Datatype = otFieldDataType.Bool
            '                aFieldDesc.Title = "is entry a leaf"
            '                aFieldDesc.ColumnName = "isleaf"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' isNode
            '                aFieldDesc.Datatype = otFieldDataType.Bool
            '                aFieldDesc.Title = "is entry a node"
            '                aFieldDesc.ColumnName = "isnode"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                '***
            '                '*** TIMESTAMP
            '                '****
            '                aFieldDesc.Datatype = otFieldDataType.Timestamp
            '                aFieldDesc.Title = "last Update"
            '                aFieldDesc.ColumnName = ConstFNUpdatedOn
            '                aFieldDesc.ID = String.empty
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                aFieldDesc.Datatype = otFieldDataType.Timestamp
            '                aFieldDesc.Title = "creation Date"
            '                aFieldDesc.ColumnName = ConstFNCreatedOn
            '                aFieldDesc.ID = String.empty
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                ' Index
            '                Call .AddIndex("PrimaryKey", PrimaryColumnNames, isprimarykey:=True)
            '                Call .AddIndex("UsedByKey", UsedKeyColumnNames, isprimarykey:=False)
            '                Call .AddIndex("cluster", ClusterColumnNames, isprimarykey:=False)


            '                ' persist
            '                .Persist()
            '                ' change the database
            '                .CreateObjectSchema()
            '            End With

            '            ' Handle the error
            '            createSchema = True
            '            Exit Function

            '            ' Handle the error
            'error_handle:
            '            Call CoreMessageHandler(subname:="clsOTDBDependMember.createSchema", tablename:=ConstPrimaryTableID)
            '            createSchema = False
        End Function

        ''' <summary>
        ''' persist to the datastore
        ''' </summary>
        ''' <param name="timestamp"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Overloads Function Persist(Optional timestamp As Date = ot.ConstNullDate) As Boolean

            Try
                'On Error GoTo errorhandle
                Call Me.Record.SetValue(constFNPartID, s_partID)
                Call Me.Record.SetValue("posno", s_posno)
                Call Me.Record.SetValue("isleaf", s_isleaf)
                Call Me.Record.SetValue("isnode", s_isnode)
                Call Me.Record.SetValue("nopos", s_nopos)
                Call Me.Record.SetValue("depfromid", s_dependfrompartid)
                Call Me.Record.SetValue("cond", s_condition)
                Call Me.Record.SetValue("cat", s_category)
                Call Me.Record.SetValue("typeid", s_typeid)
                Call Me.Record.SetValue("clusterid", s_clusterid)
                Call Me.Record.SetValue("clusterlevel", s_clusterlevel)

                Call Me.Record.SetValue("param_txt1", s_parameter_txt1)
                Call Me.Record.SetValue("param_txt2", s_parameter_txt2)
                Call Me.Record.SetValue("param_txt3", s_parameter_txt3)
                Call Me.Record.SetValue("param_date1", s_parameter_date1)
                Call Me.Record.SetValue("param_date2", s_parameter_date2)
                Call Me.Record.SetValue("param_date3", s_parameter_date3)
                Call Me.Record.SetValue("param_num1", s_parameter_num1)
                Call Me.Record.SetValue("param_num2", s_parameter_num2)
                Call Me.Record.SetValue("param_num3", s_parameter_num3)
                Call Me.Record.SetValue("param_flag1", s_parameter_flag1)
                Call Me.Record.SetValue("param_flag2", s_parameter_flag2)
                Call Me.Record.SetValue("param_flag3", s_parameter_flag3)

                Return MyBase.Persist(timestamp)
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, subname:="clsOTDBDependMember.Persist")
                Return False
            End Try

        End Function

        '**** create : create a new Object with primary keys
        '****
        ''' <summary>
        ''' Create a persistence object
        ''' </summary>
        ''' <param name="TYPEID"></param>
        ''' <param name="PARTID"></param>
        ''' <param name="POSNO"></param>
        ''' <param name="dependfromPartID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(ByVal typeid As String, ByVal partid As String, ByVal posno As Long, Optional ByVal dependfromPartID As String = String.empty) As clsOTDBDependMember
            Dim pkarray() As Object = {typeid, partid, posno}
            Return ormDataObject.CreateDataObject(Of clsOTDBDependMember)(pkArray:=pkarray, checkUnique:=True)
        End Function

        '*********** getDependCheck get the latest DependCheck of Type
        '***********
        ''' <summary>
        ''' get the latest DependCheck of Type
        ''' </summary>
        ''' <param name="workspaceID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDependCheck(Optional workspaceID As String = String.empty) As Collection
            Dim aDependCheck As New clsOTDBDependCheck
            Dim aCollection

            ' return
            If Not me.isloaded And Not Me.IsCreated Then
                GetDependCheck = Nothing
                Exit Function
            End If

            aCollection = aDependCheck.AllByDependMember(Me, workspaceID:=workspaceID)
            If Not aCollection Is Nothing Then
                GetDependCheck = aCollection
            Else
                GetDependCheck = Nothing
            End If


        End Function

    End Class
    '************************************************************************************
    '***** CLASS clsOTDBDependMember is a helper for the Dependend Parts
    '*****
    '*****
    Public Class clsOTDBDependCheck
        Inherits ormDataObject
        Implements iormPersistable
        Implements iormInfusable

        Public Const ConstPrimaryTableID = "tblDependChecks"

        ' fields
        Public Const constFNPartid = "pnid"
        Public Const constFNPosno = "posno"
        Public Const constFNSUpdc = "supdc"
        Public Const constFNdepfromid = "depfromid"
        Public Const constFNtypeid = "typeid"
        Public Const constFNClusterID = "clusterid"
        Public Const constFNClusterLevel = "clusterlevel"

        'fields
        Private s_partID As String = String.empty  ' Assy ID
        Private s_typeid As String = String.empty
        Private s_posno As Long
        Private s_suid As Long    'deliverable UID
        Private s_supdc As Long    'deliverable UID
        Private s_depsuid As Long    'deliverable UID
        Private s_depsupdc As Long    'deliverable UID

        'fields
        Private s_status As String = String.empty

        Private s_dependfrompartid As String = String.empty  ' Component ID
        Private s_condition As String = String.empty
        Private s_comment As String = String.empty
        Private s_msgno As String = String.empty

        Private s_parameter_txt1 As String = String.empty
        Private s_parameter_txt2 As String = String.empty
        Private s_parameter_txt3 As String = String.empty
        Private s_parameter_num1 As Double
        Private s_parameter_num2 As Double
        Private s_parameter_num3 As Double
        Private s_parameter_date1 As Date = ConstNullDate
        Private s_parameter_date2 As Date = ConstNullDate
        Private s_parameter_date3 As Date = ConstNullDate
        Private s_parameter_flag1 As Boolean
        Private s_parameter_flag2 As Boolean
        Private s_parameter_flag3 As Boolean

        Private s_clusterid As String
        Private s_clusterlevel As Long


#Region "Properties"


        ReadOnly Property PartID() As String
            Get
                PARTID = s_partID
            End Get
        End Property
        ReadOnly Property ScheduleUID() As Long
            Get
                scheduleUID = s_suid
            End Get

        End Property
        ReadOnly Property ScheduleUPDC() As Long
            Get
                scheduleUPDC = s_supdc
            End Get
        End Property
        ReadOnly Property Posno() As Long
            Get
                posno = s_posno
            End Get
        End Property

        Public Property DepScheduleUPDC() As Long
            Get
                DepScheduleUPDC = s_depsupdc
            End Get
            Set(value As Long)
                If value <> s_depsupdc Then
                    s_depsupdc = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property DepScheduleUID() As Long
            Get
                DepScheduleUID = s_depsuid
            End Get
            Set(value As Long)
                If value <> s_depsuid Then
                    s_depsuid = value
                    Me.IsChanged = True
                End If
            End Set
        End Property


        Public Property dependfromPartID() As String
            Get
                dependfromPartID = s_dependfrompartid
            End Get
            Set(value As String)
                s_dependfrompartid = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property status() As String
            Get
                status = s_status
            End Get
            Set(value As String)
                s_status = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property condition() As String
            Get
                condition = s_condition
            End Get
            Set(value As String)
                s_condition = value
                Me.IsChanged = True
            End Set
        End Property


        Public Property TYPEID() As String
            Get
                TYPEID = s_typeid
            End Get
            Set(value As String)
                s_typeid = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property msgno() As String
            Get
                msgno = s_msgno
            End Get
            Set(value As String)
                s_msgno = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property comment() As String
            Get
                comment = s_comment
            End Get
            Set(value As String)
                s_comment = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property clusterid() As String
            Get
                clusterid = s_clusterid
            End Get
            Set(value As String)
                s_clusterid = value
                Me.IsChanged = True
            End Set
        End Property


        Public Property clusterlevel() As Long
            Get
                clusterlevel = s_clusterlevel
            End Get
            Set(value As Long)
                If value <> s_clusterlevel Then
                    s_clusterlevel = value
                    Me.IsChanged = True
                End If
            End Set
        End Property


        Public Property parameter_num1() As Double
            Get
                parameter_num1 = s_parameter_num1
            End Get
            Set(value As Double)
                If s_parameter_num1 <> value Then
                    s_parameter_num1 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_num2() As Double
            Get
                parameter_num2 = s_parameter_num2
            End Get
            Set(value As Double)
                If s_parameter_num2 <> value Then
                    s_parameter_num2 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_num3() As Double
            Get
                parameter_num3 = s_parameter_num3
            End Get
            Set(value As Double)
                If s_parameter_num3 <> value Then
                    s_parameter_num3 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_date1() As Date
            Get
                parameter_date1 = s_parameter_date1
            End Get
            Set(value As Date)
                If s_parameter_date1 <> value Then
                    s_parameter_date1 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_date2() As Date
            Get
                parameter_date2 = s_parameter_date2
            End Get
            Set(value As Date)
                If s_parameter_date2 <> value Then
                    s_parameter_date2 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_date3() As Date
            Get
                parameter_date3 = s_parameter_date3
            End Get
            Set(value As Date)
                s_parameter_date3 = value
                Me.IsChanged = True
            End Set
        End Property
        Public Property parameter_flag1() As Boolean
            Get
                parameter_flag1 = s_parameter_flag1
            End Get
            Set(value As Boolean)
                If s_parameter_flag1 <> value Then
                    s_parameter_flag1 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_flag3() As Boolean
            Get
                parameter_flag3 = s_parameter_flag3
            End Get
            Set(value As Boolean)
                If s_parameter_flag3 <> value Then
                    s_parameter_flag3 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_flag2() As Boolean
            Get
                parameter_flag2 = s_parameter_flag2
            End Get
            Set(value As Boolean)
                If s_parameter_flag2 <> value Then
                    s_parameter_flag2 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_txt1() As String
            Get
                parameter_txt1 = s_parameter_txt1
            End Get
            Set(value As String)
                If s_parameter_txt1 <> value Then
                    s_parameter_txt1 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_txt2() As String
            Get
                parameter_txt2 = s_parameter_txt2
            End Get
            Set(value As String)
                If s_parameter_txt2 <> value Then
                    s_parameter_txt2 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
        Public Property parameter_txt3() As String
            Get
                parameter_txt3 = s_parameter_txt3
            End Get
            Set(value As String)
                If s_parameter_txt3 <> value Then
                    s_parameter_txt3 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
#End Region
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            'me.record.tablename = ourTableName
            Call MyBase.New(ConstPrimaryTableID)

        End Sub
        ''' <summary>
        ''' Initialize
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Initialize() As Boolean

            SerializeWithHostApplication = isDefaultSerializeAtHostApplication(ConstPrimaryTableID)
            Return MyBase.Initialize()
        End Function

        '''' <summary>
        '''' Infuse the data object by a record
        '''' </summary>
        '''' <param name="aRecord"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Overrides Function Infuse(ByRef record As ormRecord) As Boolean

        '    '* init
        '    If Not Me.IsInitialized Then
        '        If Not Me.Initialize() Then
        '            Infuse = False
        '            Exit Function
        '        End If
        '    End If

        '    '*** overload it from the Application Container
        '    '***
        '    If Me.SerializeWithHostApplication Then
        '        If overloadFromHostApplication(Record) Then
        '            Me.LoadedFromHost = True
        '        End If
        '    End If

        '    Try
        '        s_partID = CStr(record.GetValue(constFNPartid))
        '        s_dependfrompartid = CStr(record.GetValue("depfromid"))
        '        s_posno = CLng(record.GetValue("posno"))
        '        s_suid = CLng(record.GetValue("suid"))
        '        s_supdc = CLng(record.GetValue("supdc"))
        '        s_depsuid = CLng(record.GetValue("depsuid"))
        '        s_depsupdc = CLng(record.GetValue("depsupdc"))
        '        s_condition = CStr(record.GetValue("cond"))
        '        s_typeid = CStr(record.GetValue("typeid"))
        '        s_status = CStr(record.GetValue("status"))
        '        s_msgno = CStr(record.GetValue("msgno"))
        '        s_comment = CStr(record.GetValue("cmt"))
        '        If Not IsNull(record.GetValue("clusterid")) Then
        '            s_clusterid = CStr(record.GetValue("clusterid"))
        '        Else
        '            s_clusterid = String.empty
        '        End If
        '        If Not IsNull(record.GetValue("clusterlevel")) Then
        '            s_clusterlevel = CLng(record.GetValue("clusterlevel"))
        '        Else
        '            s_clusterlevel = 0
        '        End If
        '        s_parameter_txt1 = CStr(record.GetValue("param_txt1"))
        '        s_parameter_txt2 = CStr(record.GetValue("param_txt2"))
        '        s_parameter_txt3 = CStr(record.GetValue("param_txt3"))
        '        s_parameter_num1 = CDbl(record.GetValue("param_num1"))
        '        s_parameter_num2 = CDbl(record.GetValue("param_num2"))
        '        s_parameter_num3 = CDbl(record.GetValue("param_num3"))
        '        s_parameter_date1 = CDate(record.GetValue("param_date1"))
        '        s_parameter_date2 = CDate(record.GetValue("param_date2"))
        '        s_parameter_date3 = CDate(record.GetValue("param_date3"))
        '        s_parameter_flag1 = CBool(record.GetValue("param_flag1"))
        '        s_parameter_flag2 = CBool(record.GetValue("param_flag2"))
        '        s_parameter_flag3 = CBool(record.GetValue("param_flag3"))


        '        Return MyBase.Infuse(record)

        '    Catch ex As Exception
        '        CoreMessageHandler(exception:=ex, subname:="clsOTDBDependCheck.Infuse")
        '        Return False
        '    End Try


        'End Function

        ''' <summary>
        ''' load a DependCheck by primary key
        ''' </summary>
        ''' <param name="typeid"></param>
        ''' <param name="partid"></param>
        ''' <param name="posno"></param>
        ''' <param name="uid"></param>
        ''' <param name="VERSION"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal typeid As String, ByVal partid As String, ByVal posno As Long, ByVal uid As Long, ByVal updc As Long) As clsOTDBDependCheck
            Dim pkarry() As Object = {typeid, partid, posno, uid, updc}
            Return ormDataObject.Retrieve(Of clsOTDBDependCheck)(pkArray:=pkarry)
        End Function

        '********** all Head by ClusterID
        '**********
        ''' <summary>
        ''' retrieve just the DependCheckHeads by ClusterID
        ''' </summary>
        ''' <param name="typeid"></param>
        ''' <param name="clusterid"></param>
        ''' <param name="workspaceID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function AllHeadByClusterID(typeid As String, clusterid As String, Optional ByVal workspaceID As String = String.empty) As List(Of clsOTDBDependCheck)
            Dim aCollection As New List(Of clsOTDBDependCheck)
            Dim aRecordCollection As List(Of ormRecord)
            Dim aStore As iormDataStore

            Try
                aStore = GetTableStore(ConstPrimaryTableID)
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(id:="AllHeadByClusterID", addAllFields:=True)
                If Not aCommand.Prepared Then
                    aCommand.Where = ConstPrimaryTableID & ".[" & constFNdepfromid & "] = '' AND " & ConstPrimaryTableID & ".[" & constFNtypeid & "] =@typeid"
                    aCommand.Where &= " AND " & ConstPrimaryTableID & ".[" & constFNClusterID & "] = @clusterid"
                    aCommand.OrderBy = ConstPrimaryTableID & ".[" & constFNClusterLevel & "] asc"
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@typeid", ColumnName:=ConstFNtypeid, tablename:=ConstPrimaryTableID))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@clusterid", ColumnName:=ConstFNClusterID, tablename:=ConstPrimaryTableID))
                    aCommand.Prepare()
                End If
                aCommand.SetParameterValue(ID:="@typeid", value:=typeid)
                aCommand.SetParameterValue(ID:="@clusterid", value:=clusterid)

                aRecordCollection = aCommand.RunSelect

                For Each aRecord As ormRecord In aRecordCollection
                    Dim aDependCheck As New clsOTDBDependCheck
                    'If aDependCheck.Infuse(aRecord) Then
                    '    aCollection.Add(Item:=aDependCheck)
                    'End If
                Next

                Return aCollection


            Catch ex As Exception

                Call CoreMessageHandler(exception:=ex, subname:="clsOtdbDependCheck.AllHeadByClusterID")
                Return aCollection

            End Try

        End Function

        '********** all by DEpendMember
        '**********
        ''' <summary>
        ''' return all depend checks for a depend member
        ''' </summary>
        ''' <param name="aDependMember"></param>
        ''' <param name="workspaceID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function AllByDependMember(dependMember As clsOTDBDependMember, Optional ByVal workspaceID As String = String.empty) As List(Of clsOTDBDependCheck)

            Dim aCollection As New List(Of clsOTDBDependCheck)
            Dim aRecordCollection As List(Of ormRecord)
            Dim aStore As iormDataStore


            Try
                aStore = GetTableStore(ConstPrimaryTableID)
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(id:="AllByDependMember", addAllFields:=True)
                If Not aCommand.Prepared Then
                    aCommand.Where = ConstPrimaryTableID & ".[" & constFNPartid & "] = @partid AND " & ConstPrimaryTableID & ".[" & constFNtypeid & "] =@typeid"
                    aCommand.Where &= " AND " & ConstPrimaryTableID & ".[" & constFNPosno & "] = @posno"
                    aCommand.OrderBy = ConstPrimaryTableID & ".[" & constFNSUpdc & "] desc"
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@partid", ColumnName:=ConstFNPartid, tablename:=ConstPrimaryTableID))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@typeid", ColumnName:=ConstFNtypeid, tablename:=ConstPrimaryTableID))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@posno", ColumnName:=ConstFNPosno, tablename:=ConstPrimaryTableID))
                    aCommand.Prepare()
                End If
                aCommand.SetParameterValue(ID:="@partid", value:=dependMember.PartID)
                aCommand.SetParameterValue(ID:="@typeid", value:=dependMember.TypeID)
                aCommand.SetParameterValue(ID:="@posno", value:=dependMember.PosNo)

                aRecordCollection = aCommand.RunSelect

                For Each aRecord As ormRecord In aRecordCollection
                    Dim aDependCheck As New clsOTDBDependCheck
                    'If aDependCheck.Infuse(aRecord) Then
                    '    aCollection.Add(item:=aDependCheck)
                    'End If
                Next

                Return aCollection


            Catch ex As Exception

                Call CoreMessageHandler(exception:=ex, subname:="clsOtdbDependCheck.AllByDependMember")
                Return aCollection

            End Try

        End Function
        ''' <summary>
        ''' create persistency schema
        ''' </summary>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateSchema(Optional silent As Boolean = True) As Boolean


            '            Dim PrimaryColumnNames As New Collection
            '            Dim UsedKeyColumnNames As New Collection
            '            Dim ClusterColumnNames As New Collection
            '            Dim aFieldDesc As New ormFieldDescription
            '            Dim aTable As New ObjectDefinition


            '            aFieldDesc.ID = String.empty
            '            aFieldDesc.Parameter = String.empty
            '            aFieldDesc.Relation = New String() {}
            '            aFieldDesc.Aliases = New String() {}
            '            aFieldDesc.Tablename = ConstPrimaryTableID

            '            With aTable
            '                .Create(ConstPrimaryTableID)
            '                .Delete()
            '                ' typeid
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "type of dependencies"
            '                aFieldDesc.ColumnName = "typeid"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                PrimaryColumnNames.Add(aFieldDesc.ColumnName)

            '                'component id
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "dependend from part-id"
            '                aFieldDesc.ColumnName = "depfromid"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                UsedKeyColumnNames.Add(aFieldDesc.ColumnName)

            '                aFieldDesc.Title = "part-id"
            '                aFieldDesc.ColumnName = constFNPartid
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                PrimaryColumnNames.Add(aFieldDesc.ColumnName)
            '                UsedKeyColumnNames.Add(aFieldDesc.ColumnName)

            '                'Position
            '                aFieldDesc.Datatype = otFieldDataType.[Long]
            '                aFieldDesc.Title = "posno"
            '                aFieldDesc.ColumnName = "posno"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                PrimaryColumnNames.Add(aFieldDesc.ColumnName)

            '                'Position
            '                aFieldDesc.Datatype = otFieldDataType.[Long]
            '                aFieldDesc.Title = "uid of schedule"
            '                aFieldDesc.ColumnName = "suid"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                PrimaryColumnNames.Add(aFieldDesc.ColumnName)

            '                'Position
            '                aFieldDesc.Datatype = otFieldDataType.[Long]
            '                aFieldDesc.Title = "schedule update counter"
            '                aFieldDesc.ColumnName = "supdc"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                PrimaryColumnNames.Add(aFieldDesc.ColumnName)

            '                'Position
            '                aFieldDesc.Datatype = otFieldDataType.[Long]
            '                aFieldDesc.Title = "uid of dependant schedule"
            '                aFieldDesc.ColumnName = "depsuid"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                'PrimaryColumnNames.add aFieldDesc.Name

            '                'Position
            '                aFieldDesc.Datatype = otFieldDataType.[Long]
            '                aFieldDesc.Title = "dependant schedule update counter"
            '                aFieldDesc.ColumnName = "depsupdc"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                'PrimaryColumnNames.add aFieldDesc.Name

            '                ' status
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "status"
            '                aFieldDesc.ColumnName = "status"
            '                aFieldDesc.Size = 20
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)


            '                ' condition
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "condition"
            '                aFieldDesc.ColumnName = "cond"
            '                aFieldDesc.Size = 0
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' msg
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "msgno"
            '                aFieldDesc.ColumnName = "msgno"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' cmt
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "comments"
            '                aFieldDesc.ColumnName = "cmt"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                ' parameter_txt 1
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "parameter_txt 1 of condition"
            '                aFieldDesc.ColumnName = "param_txt1"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_txt 2
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "parameter_txt 2 of condition"
            '                aFieldDesc.ColumnName = "param_txt2"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_txt 2
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "parameter_txt 3 of condition"
            '                aFieldDesc.ColumnName = "param_txt3"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_num 1
            '                aFieldDesc.Datatype = otFieldDataType.Numeric
            '                aFieldDesc.Title = "parameter numeric 1 of condition"
            '                aFieldDesc.ColumnName = "param_num1"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_num 2
            '                aFieldDesc.Datatype = otFieldDataType.Numeric
            '                aFieldDesc.Title = "parameter numeric 2 of condition"
            '                aFieldDesc.ColumnName = "param_num2"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                ' parameter_num 2
            '                aFieldDesc.Datatype = otFieldDataType.Numeric
            '                aFieldDesc.Title = "parameter numeric 3 of condition"
            '                aFieldDesc.ColumnName = "param_num3"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_date 1
            '                aFieldDesc.Datatype = otFieldDataType.[Date]
            '                aFieldDesc.Title = "parameter date 1 of condition"
            '                aFieldDesc.ColumnName = "param_date1"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_date 2
            '                aFieldDesc.Datatype = otFieldDataType.[Date]
            '                aFieldDesc.Title = "parameter date 2 of condition"
            '                aFieldDesc.ColumnName = "param_date2"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                ' parameter_date 3
            '                aFieldDesc.Datatype = otFieldDataType.[Date]
            '                aFieldDesc.Title = "parameter date 3 of condition"
            '                aFieldDesc.ColumnName = "param_date3"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                ' parameter_flag 1
            '                aFieldDesc.Datatype = otFieldDataType.Bool
            '                aFieldDesc.Title = "parameter flag 1 of condition"
            '                aFieldDesc.ColumnName = "param_flag1"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' parameter_flag 2
            '                aFieldDesc.Datatype = otFieldDataType.Bool
            '                aFieldDesc.Title = "parameter flag 2 of condition"
            '                aFieldDesc.ColumnName = "param_flag2"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                ' parameter_flag 3
            '                aFieldDesc.Datatype = otFieldDataType.Bool
            '                aFieldDesc.Title = "parameter flag 3 of condition"
            '                aFieldDesc.ColumnName = "param_flag3"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' cluster
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "cluster id"
            '                aFieldDesc.ColumnName = "clusterid"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                ClusterColumnNames.Add("clusterid")

            '                ' clusterlevel
            '                aFieldDesc.Datatype = otFieldDataType.[Long]
            '                aFieldDesc.Title = "cluster level"
            '                aFieldDesc.ColumnName = "clusterlevel"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                ClusterColumnNames.Add("clusterlevel")
            '                ClusterColumnNames.Add(constFNPartid)

            '                '***
            '                '*** TIMESTAMP
            '                '****
            '                aFieldDesc.Datatype = otFieldDataType.Timestamp
            '                aFieldDesc.Title = "last Update"
            '                aFieldDesc.ColumnName = ConstFNUpdatedOn
            '                aFieldDesc.ID = String.empty
            '                aFieldDesc.Aliases = New String() {}
            '                aFieldDesc.Relation = New String() {}
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                aFieldDesc.Datatype = otFieldDataType.Timestamp
            '                aFieldDesc.Title = "creation Date"
            '                aFieldDesc.ColumnName = ConstFNCreatedOn
            '                aFieldDesc.ID = String.empty
            '                aFieldDesc.Aliases = New String() {}
            '                aFieldDesc.Relation = New String() {}
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                ' Index
            '                Call .AddIndex("PrimaryKey", PrimaryColumnNames, isprimarykey:=True)
            '                Call .AddIndex("UsedByKey", UsedKeyColumnNames, isprimarykey:=False)
            '                Call .AddIndex("cluster", ClusterColumnNames, isprimarykey:=False)

            '                ' persist
            '                .Persist()
            '                ' change the database
            '                .CreateObjectSchema()
            '            End With

            '            '
            '            createSchema = True
            '            Exit Function

            '            ' Handle the error
            'error_handle:
            '            Call CoreMessageHandler(subname:="clsOTDBDependCheck.createSchema", tablename:=ConstPrimaryTableID)
            '            createSchema = False
        End Function

        ''' <summary>
        ''' Persist the object to the datastore
        ''' </summary>
        ''' <param name="timestamp"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Persist(Optional timestamp As Date = ot.ConstNullDate) As Boolean

            '* init
            If Not Me.IsInitialized Then
                If Not Me.Initialize() Then
                    Persist = False
                    Exit Function
                End If
            End If
            Try
                Call Me.Record.SetValue(constFNPartid, s_partID)
                Call Me.Record.SetValue("posno", s_posno)
                Call Me.Record.SetValue("suid", s_suid)
                Call Me.Record.SetValue("supdc", s_supdc)
                Call Me.Record.SetValue("depsuid", s_depsuid)
                Call Me.Record.SetValue("depsupdc", s_depsupdc)
                Call Me.Record.SetValue("status", s_status)
                Call Me.Record.SetValue("depfromid", s_dependfrompartid)
                Call Me.Record.SetValue("cond", s_condition)
                Call Me.Record.SetValue("cmt", s_comment)
                Call Me.Record.SetValue("msgno", s_msgno)
                Call Me.Record.SetValue("typeid", s_typeid)
                Call Me.Record.SetValue("clusterid", s_clusterid)
                Call Me.Record.SetValue("clusterlevel", s_clusterlevel)
                Call Me.Record.SetValue("param_txt1", s_parameter_txt1)
                Call Me.Record.SetValue("param_txt2", s_parameter_txt2)
                Call Me.Record.SetValue("param_txt3", s_parameter_txt3)
                Call Me.Record.SetValue("param_date1", s_parameter_date1)
                Call Me.Record.SetValue("param_date2", s_parameter_date2)
                Call Me.Record.SetValue("param_date3", s_parameter_date3)
                Call Me.Record.SetValue("param_num1", s_parameter_num1)
                Call Me.Record.SetValue("param_num2", s_parameter_num2)
                Call Me.Record.SetValue("param_num3", s_parameter_num3)
                Call Me.Record.SetValue("param_flag1", s_parameter_flag1)
                Call Me.Record.SetValue("param_flag2", s_parameter_flag2)
                Call Me.Record.SetValue("param_flag3", s_parameter_flag3)

                ' overwrite
                If Me.SerializeWithHostApplication And Not isOverloadingSuspended() Then
                    If overwriteToHostApplication(Me.Record) Then
                        Me.SavedToHost = True
                    End If
                Else
                    Return MyBase.Persist(timestamp)
                End If
                Return True

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, subname:="clsOTDBDependCheck.Persist")
                Return False
            End Try


errorhandle:

            Persist = False

        End Function

        '**** create : create a new Object with primary keys
        '****
        Public Shared Function Create(ByVal TYPEID As String, _
                               ByVal PARTID As String, _
                               ByVal POSNO As Long, _
                               ByVal UID As Long, _
                               ByVal UPDC As Long, _
                               Optional ByVal dependfromPartID As String = String.empty) As clsOTDBDependCheck
            Dim pkarray() As Object = {TYPEID, PARTID, POSNO, UID, UPDC}
            Return ormDataObject.CreateDataObject(Of clsOTDBDependCheck)(pkArray:=pkarray, checkUnique:=True)
        End Function


        '**************** run specific check IFC1
        '****************
        Private Function runIFC1(DEPENDMEMBER As clsOTDBDependMember, _
                             DELIVERABLE As Deliverable, _
                             PART As Part, _
                             SCHEDULE As ScheduleEdition, _
                             Optional workspaceID As String = String.empty) As Boolean
            Dim aDependPart As New Part
            Dim aDependDeliv As New Deliverable
            Dim aDependDelivColl As New List(Of Deliverable)
            Dim aDependSchedule As New ScheduleEdition
            Dim UID As Long
            Dim anInterface As New clsOTDBInterface

            Dim ourTimeI As New clsHELPERTimeInterval
            Dim DependTimeI As New clsHELPERTimeInterval
            Dim overlapp As Long
            Dim maxoverlapp As Long

            maxoverlapp = 0
            Me.status = String.empty

            If IsMissing(workspaceID) Then
                workspaceID = CurrentSession.CurrentWorkspaceID
            Else
                workspaceID = CStr(workspaceID)
            End If

            'do we have a schedule open ??
            If String.isnullorempty(sCHEDULE.Typeid) Then
                Me.status = OTDBConst_DependStatus_g1
                Me.msgno = Me.msgno & ":0001"
                Me.comment = "schedule (" & SCHEDULE.Updc & " for " & aDependPart.PartID & " with deliverable " & aDependDeliv.Uid & " is of type none -> fine"
                runIFC1 = True
                Exit Function
                ' pdm ?!
            ElseIf LCase(SCHEDULE.Typeid) = "pdm" Then
                Me.status = OTDBConst_DependStatus_g1
                Me.msgno = Me.msgno & ":0002"
                Me.comment = "schedule (" & SCHEDULE.Updc & " for " & aDependPart.PartID & " with deliverable " & aDependDeliv.Uid & " is of type pdm -> fine"
                runIFC1 = True
                Exit Function
            End If



            ' check the interface first
            If Not anInterface.Inject(DEPENDMEMBER.parameter_num1) Then
                Me.status = OTDBConst_DependStatus_r2
                Me.msgno = Me.msgno & ":0003"
                Me.comment = "no interface for uid " & anInterface.UID & " could be loaded ->  no status to reflect"
                runIFC1 = True
                Exit Function
            Else
                ' exit if interface is not red
                If Not anInterface.status Like "r*" Then
                    Me.status = OTDBConst_DependStatus_g2
                    Me.msgno = Me.msgno & ":0004"
                    Me.comment = "interface with uid " & anInterface.UID & " is of status '" & anInterface.status & "' and therefore not reflected "
                    runIFC1 = True
                    Exit Function
                End If
            End If

            'is the schedule open no posibilty to handshake ??
            If Not SCHEDULE.IsMilestoneValueMissing("bp8") Or Not SCHEDULE.IsMilestoneValueMissing("bp4") Or SCHEDULE.IsFinished Then
                Me.status = OTDBConst_DependStatus_y1
                Me.msgno = "0005"
                Me.comment = Me.msgno & ":schedule (" & SCHEDULE.Updc & " for " & aDependPart.PartID & " with deliverable " & aDependDeliv.Uid & " is past bp4 -> interfaces of uid# " & DEPENDMEMBER.parameter_num1 & " are open - revision to come ?! IFM Gate defined ?!"
                runIFC1 = True
                Exit Function
            End If
            ' check on Synchro
            ourTimeI = SCHEDULE.GetTimeInterval("Development")
            If Not ourTimeI.isvalid Then
                Me.status = OTDBConst_DependStatus_r3
                Me.msgno = "0007"
                Me.comment = Me.msgno & ":no valid milestones (" & ourTimeI.startcmt & ";" & ourTimeI.endcmt & _
                             ") for development could be found in " & _
                             Me.PARTID & " with schedule " & _
                             Me.scheduleUPDC & " -> correct first"
                runIFC1 = True
                Exit Function
            End If
            If ourTimeI.relativeTo(DateAdd("d", -CurrentSession.TodayLatency, Date.Now())) = otIntervalRelativeType.IntervalLeft Then
                Me.msgno = "0008"
                Me.status = OTDBConst_DependStatus_r3
                Me.comment = Me.msgno & ":milestones (" & ourTimeI.startcmt & ";" & ourTimeI.endcmt & ") for development " & _
                             Me.PARTID & " with schedule " & _
                             Me.scheduleUPDC & "are in the past -> correct first"
                runIFC1 = True
                Exit Function
            End If
            If ourTimeI.relativeTo(DateAdd("d", -CurrentSession.TodayLatency, Date.Now())) = otIntervalRelativeType.IntervalMiddle Then
                ' Me.msgno = "0009"
                ' Me.Status = OTDBConst_DependStatus_y1
                ' Me.comment = Me.msgno & ":milestones (" & ourTimeI.startcmt & ";" & ourTimeI.endcmt & ") for development " & 
                '              Me.PartID & " with schedule " & 
                '              me.scheduleUPDC & " has one milestone in the past -> reduced overlapping time"
                'runIFC1 = True
                'Exit Function
            End If
            ' get a the other Schedule to compare
            aDependPart = Parts.Part.Retrieve(DEPENDMEMBER.dependfromPartID)
            If aDependPart IsNot Nothing AndAlso Not aDependPart.IsDeleted Then
                aDependDelivColl = aDependPart.GetDeliverables

                '** go through each delivarble of the other member (multiple deliverables per part)
                '** following deliverables will be not checked !
                '**
                For Each aDependDeliv In aDependDelivColl
                    ' get
                    aDependSchedule = aDependDeliv.GetWorkScheduleEdition(workspaceID)
                    If Not aDependSchedule Is Nothing And aDependSchedule.IsLoaded Then
                        ' store the Dependant UID in Parameter #3
                        Me.DepScheduleUID = aDependSchedule.Uid
                        Me.DepScheduleUPDC = aDependSchedule.Updc

                        ' if we are depending on a finished
                        If aDependSchedule.IsFinished Or Not aDependSchedule.IsMilestoneValueMissing("bp8") Then
                            Me.status = OTDBConst_DependStatus_y1
                            Me.msgno = "0006"
                            Me.comment = Me.msgno & ":depending from " & aDependPart.PartID & _
                                         " with deliverable " & aDependDeliv.Uid & " has finished schedule but open interface (#" & anInterface.UID & ") ???!"
                            runIFC1 = True
                            Exit Function
                        End If

                        ' check the dependant

                        DependTimeI = aDependSchedule.GetTimeInterval("Development")

                        If Not DependTimeI.isvalid Then
                            Me.status = OTDBConst_DependStatus_r2
                            Me.msgno = "0010"
                            Me.comment = Me.msgno & ": ifc uid# " & anInterface.UID & " results in part with no valid milestones (" & ourTimeI.startcmt & "," & ourTimeI.endcmt & _
                                         ") for development could be found in depend " & _
                                         Me.PartID & " with schedule " & _
                                         Me.ScheduleUPDC & " -> correct first"
                            '
                            runIFC1 = True
                            Exit Function
                        Else
                            Me.parameter_date1 = DependTimeI.startdate
                            Me.parameter_txt1 = DependTimeI.startcmt
                            Me.parameter_flag1 = DependTimeI.isActStart
                            Me.parameter_date2 = DependTimeI.enddate
                            Me.parameter_txt2 = DependTimeI.endcmt
                            Me.parameter_flag2 = DependTimeI.isActEnd
                        End If
                        If DependTimeI.relativeTo(DateAdd("d", -CurrentSession.TodayLatency, Date.Now())) = otIntervalRelativeType.IntervalLeft Then
                            If Not DependTimeI.isActEnd Then
                                Me.status = OTDBConst_DependStatus_r2
                                Me.msgno = "0011"
                                Me.comment = Me.msgno & ": ifc uid# " & anInterface.UID & " results in part milestones (" & ourTimeI.startcmt & "," & ourTimeI.endcmt & ") for development of depend " & _
                                             Me.PartID & " with schedule " & _
                                             Me.ScheduleUPDC & "are in the past -> correct first"
                                runIFC1 = True
                                Exit Function
                            Else
                                Me.status = OTDBConst_DependStatus_y1
                                Me.msgno = "0015"
                                Me.comment = Me.msgno & ":schedule #" & SCHEDULE.Updc & " for depend " & aDependPart.PartID & " with deliverable " & aDependDeliv.Uid & _
                                             " is past actual FAP of " & Format(aDependSchedule.GetMilestoneValue("bp4"), "dd.mm.yyyy") & " -> interfaces of uid# " & anInterface.UID & " are open - revision to come ?! IFM Gate defined ?!"
                                runIFC1 = True
                                Exit Function
                            End If
                        End If
                        If DependTimeI.relativeTo(DateAdd("d", CurrentSession.TodayLatency, Date.Now())) = otIntervalRelativeType.IntervalMiddle Then
                            'Me.Status = OTDBConst_DependStatus_y1
                            'Me.msgno = "0012"
                            'Me.comment = "milestones (" & ourTimeI.startcmt & "," & ourTimeI.endcmt & ") for development of depend" & 
                            '             Me.PartID & " with schedule " & 
                            '             me.scheduleUPDC & " has one milestone in the past -> reduced overlapping time"
                            'runIFC1 = True
                            'Exit Function
                        End If
                        ' save the depend


                        ' check the overlapping if valid -> if not and we are still here than a msgno should have been issued
                        If ourTimeI.isvalid And DependTimeI.isvalid Then
                            ' calc overlapp
                            overlapp = ourTimeI.overlapp(DependTimeI)

                            Me.parameter_num1 = overlapp
                            If overlapp < CurrentSession.DependencySynchroMinOverlap Then
                                Me.status = OTDBConst_DependStatus_r1
                                Me.msgno = "0013"
                                Me.comment = Me.msgno & ": (ifc uid#" & anInterface.UID & ") overlapping time between " & aDependPart.PartID & _
                                             " with deliverable " & aDependDeliv.Uid & " and " & Me.PartID & " with schedule " & _
                                             Me.ScheduleUPDC & " has less overlapping of " & overlapp & " which is less then 7 days -> not sync"
                                runIFC1 = True
                                Exit Function
                            Else
                            End If

                            If overlapp >= maxoverlapp Then
                                maxoverlapp = overlapp
                            End If
                        End If
                    End If
                Next aDependDeliv
            End If

            If Me.status = String.empty Then
                Me.status = OTDBConst_DependStatus_g1
                Me.msgno = "0014"
                Me.comment = Me.msgno & ":synchro check on ifc uid# " & anInterface.UID & " between " & aDependPart.PartID & _
                             " with deliverable " & aDependDeliv.Uid & " and " & Me.PARTID & " (schedule #" & _
                             Me.scheduleUPDC & " has suceeded with max overlapp of " & maxoverlapp
            End If
            runIFC1 = True

        End Function

        '**************** run check
        '****************
        Public Function run(DEPENDMEMBER As clsOTDBDependMember, _
                            Optional workspaceID As String = String.empty, _
                            Optional ByVal autopersist As Boolean = False) As Boolean
            'Dim aDependMember As New clsOTDBDependMember
            Dim aDelivColl As New List(Of Deliverable)
            Dim aPart As New Part
            Dim aDeliverable As New Deliverable
            Dim aSchedule As New ScheduleEdition

            If Not DEPENDMEMBER.IsCreated And Not DEPENDMEMBER.IsLoaded Then
                run = False
                Exit Function
            End If

            If IsMissing(workspaceID) Then
                workspaceID = CurrentSession.CurrentWorkspaceID
            End If

            ' get a Schedule
            aPart = Parts.Part.Retrieve(DEPENDMEMBER.PartID)
            If aPart IsNot Nothing AndAlso Not aPart.IsDeleted Then
                aDelivColl = aPart.GetDeliverables
                If aDelivColl Is Nothing Then
                    Call CoreMessageHandler(message:="no deliverables for part", arg1:=DEPENDMEMBER.PartID, break:=False)

                    run = False
                    Exit Function
                End If
                If aDelivColl.Count = 0 Then
                    Call CoreMessageHandler(message:="no deliverables for part", arg1:=DEPENDMEMBER.PartID, break:=False)

                    run = False
                    Exit Function
                End If
                ' go through each delivarble
                For Each aDeliverable In aDelivColl
                    ' get
                    aSchedule = aDeliverable.GetWorkScheduleEdition(workspaceID)
                    If Not aSchedule Is Nothing And aSchedule.IsLoaded Then
                        ' set it all -> create through the backdoor
                        ' we donot want to save if we are in a host enviorement
                        s_typeid = DEPENDMEMBER.TypeID
                        s_partID = DEPENDMEMBER.PartID
                        s_posno = DEPENDMEMBER.PosNo
                        s_suid = aSchedule.Uid
                        s_supdc = aSchedule.Updc
                        Me.condition = DEPENDMEMBER.condition
                        Me.dependfromPartID = DEPENDMEMBER.dependfromPartID
                        'me.iscreated = True
                        '*** run specific tests on the CONDITION
                        Select Case DEPENDMEMBER.condition
                            Case "IFC1"
                                run = runIFC1(DEPENDMEMBER, aDeliverable, aPart, aSchedule, workspaceID:=workspaceID)
                                If run And autopersist Then
                                    'me.iscreated = True
                                    Me.Persist()
                                End If
                            Case "IFC2"
                                System.Diagnostics.Debug.WriteLine("leaf IFC2 not yet implemented")
                            Case "IFC3"
                                System.Diagnostics.Debug.WriteLine("head IFC3 not yet implemented")
                            Case Else
                                System.Diagnostics.Debug.WriteLine("not recognised")
                                run = False
                                Exit Function
                        End Select

                    End If
                Next aDeliverable
            End If

            ' exit true
            run = True
            Exit Function

        End Function


    End Class

    Public Class clsOTDBCluster
        Inherits ormDataObject
        '************************************************************************************
        '***** CLASS clsOTDBCluster is a helper for the Dependend Parts
        '*****
        '*****

        Const ourTableName = "tblClusters"

        Private s_typeid As String       ' condition type
        Private s_clusterid As String    ' cluster id

        'fields
        Private s_clustertype As String
        Private s_status As String
        Private s_maxlevel As Long
        Private s_size As Long



        '*** init
        Public Function initialize() As Boolean
            initialize = MyBase.Initialize()
        End Function

#Region "******************** Properties ******************"
        Public Property status() As String
            Get
                status = s_status
            End Get
            Set(value As String)
                If value <> s_status Then
                    s_status = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        ReadOnly Property typeid() As String
            Get
                typeid = s_typeid
            End Get

        End Property
        'Public Property Let typeid(aValue As String)
        '    s_typeid = aValue
        '    me.isChanged = True
        'End Property

        ReadOnly Property clusterid() As String
            Get
                clusterid = s_clusterid
            End Get

        End Property


        Public Property maxlevel() As Long
            Get
                maxlevel = s_maxlevel
            End Get
            Set(value As Long)
                If value <> s_maxlevel Then
                    s_maxlevel = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        Public Property size() As Long
            Get
                size = s_size
            End Get
            Set(value As Long)
                If s_size <> value Then
                    s_size = value
                    Me.IsChanged = True
                End If
            End Set
        End Property


        Public Property clustertype() As String
            Get
                clustertype = s_clustertype
            End Get
            Set(value As String)
                If value <> s_clustertype Then
                    s_clustertype = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        Public Property isDynamic() As Boolean
            Get
                If s_clustertype = "dynamic" Then
                    isDynamic = True
                Else
                    isDynamic = False
                End If
            End Get
            Set(value As Boolean)
                If value Then
                    s_clustertype = "dynamic"
                Else
                    s_clustertype = "static"
                End If
                Me.IsChanged = True
            End Set
        End Property

#End Region

        '****** all: "static" function to return a collection of parts by key
        '******
        Public Function all(Optional isDynamic = False) As Collection
            Dim aCollection As New Collection
            Dim aRecordCollection As List(Of ormRecord)
            Dim aTable As iormDataStore
            Dim Key() As Object
            Dim aRecord As ormRecord
            Dim wherestr As String
            Dim orderby As String
            Dim innerjoin As String

            ' set the primaryKey
            'ReDim Key(1 To 1) As object
            'Key(1) = uid

            On Error GoTo error_handler

            aTable = GetTableStore(ourTableName)

            wherestr = " ctype = 'dynamic'"    ' and size > 1"
            orderby = " size desc "
            ' select

            aRecordCollection = aTable.GetRecordsBySql(wherestr, orderby:=orderby)

            If aRecordCollection Is Nothing Then
                Me.Unload()
                all = Nothing
                Exit Function
            Else
                For Each aRecord In aRecordCollection
                    Dim aNewPart As New clsOTDBCluster
                    aNewPart = New clsOTDBCluster
                    If aNewPart.infuse(aRecord) Then
                        aCollection.Add(Item:=aNewPart)
                    End If
                Next aRecord
                all = aCollection
                Exit Function
            End If

error_handler:

            all = Nothing
            Exit Function
        End Function

        '** initialize

        Public Sub New()
            'me.record.tablename = ourTableName
            MyBase.New(ourTableName)

        End Sub

        '**** infuese the object by a OTDBRecord
        '****
        Public Function infuse(ByRef aRecord As ormRecord) As Boolean
            '* lazy init
            If Not Me.IsInitialized Then
                If Not Me.initialize() Then
                    infuse = False
                    Exit Function
                End If
            End If
            On Error GoTo errorhandle

            Me.Record = aRecord


            s_typeid = CStr(aRecord.GetValue("typeid"))
            s_status = CStr(aRecord.GetValue("status"))

            s_clusterid = CStr(aRecord.GetValue("clusterid"))
            s_clustertype = CStr(aRecord.GetValue("ctype"))
            If Not IsNull(aRecord.GetValue("clusterlevel")) Then
                s_maxlevel = CLng(aRecord.GetValue("clusterlevel"))
            Else
                s_maxlevel = 0
            End If
            s_size = CLng(aRecord.GetValue("size"))
            _updatedOn = CDate(aRecord.GetValue(ConstFNUpdatedOn))
            _createdOn = CDate(aRecord.GetValue(ConstFNCreatedOn))

            infuse = True
            'me.isloaded = True
            Exit Function

errorhandle:
            infuse = False


        End Function

        '**** Inject : load the object by the PrimaryKeys
        '****
        Public Function Inject(ByVal TYPEID As String, ByVal clusterid As String) As Boolean
            Dim aTable As iormDataStore
            Dim pkarry(2) As Object
            Dim aRecord As ormRecord

            '* lazy init
            If Not Me.IsInitialized Then
                If Not Me.initialize() Then
                    Inject = False
                    Exit Function
                End If
            End If

            ' set the primaryKey
            pkarry(1) = TYPEID
            pkarry(2) = clusterid

            aTable = GetTableStore(ourTableName)
            aRecord = aTable.GetRecordByPrimaryKey(pkarry)

            If aRecord Is Nothing Then
                Me.Unload()
                Inject = Me.IsLoaded
                Exit Function
            Else
                Me.Record = aRecord
                'me.isloaded = Me.infuse(Me.Record)
                Inject = Me.IsLoaded
                Exit Function
            End If

error_handler:
            Inject = True
            Exit Function
        End Function

        '********** static createSchema
        '**********
        Public Function createSchema(Optional silent As Boolean = True) As Boolean

            '            Dim aFieldDesc As New ormFieldDescription
            '            Dim PrimaryColumnNames As New Collection
            '            Dim aTable As New ObjectDefinition

            '            With aTable
            '                .Create(ourTableName)
            '                .Delete()

            '                aFieldDesc.Tablename = ourTableName
            '                aFieldDesc.ID = String.empty
            '                aFieldDesc.Parameter = String.empty

            '                '***
            '                '*** Fields
            '                '****

            '                ' typeid
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "type of dependencies"
            '                aFieldDesc.ColumnName = "typeid"
            '                aFieldDesc.ID = "DT1"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                PrimaryColumnNames.Add(aFieldDesc.ColumnName)

            '                'cluster id
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "cluster id"
            '                aFieldDesc.ColumnName = "clusterid"
            '                aFieldDesc.ID = "CL1"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                PrimaryColumnNames.Add(aFieldDesc.ColumnName)

            '                'clustertype
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "cluster type"
            '                aFieldDesc.ColumnName = "ctype"
            '                aFieldDesc.ID = "CL2"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                ' status
            '                aFieldDesc.Datatype = otFieldDataType.Text
            '                aFieldDesc.Title = "status"
            '                aFieldDesc.ColumnName = "status"
            '                aFieldDesc.ID = "CL3"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                ' clusterlevel
            '                aFieldDesc.Datatype = otFieldDataType.[Long]
            '                aFieldDesc.Title = "max cluster level"
            '                aFieldDesc.ColumnName = "clusterlevel"
            '                aFieldDesc.ID = "CL4"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                aFieldDesc.Datatype = otFieldDataType.[Long]
            '                aFieldDesc.Title = "size size"
            '                aFieldDesc.ColumnName = "size"
            '                aFieldDesc.ID = "CL5"
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                '***
            '                '*** TIMESTAMP
            '                '****
            '                aFieldDesc.Datatype = otFieldDataType.Timestamp
            '                aFieldDesc.Title = "last Update"
            '                aFieldDesc.ColumnName = ConstFNUpdatedOn
            '                aFieldDesc.ID = String.empty
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)

            '                aFieldDesc.Datatype = otFieldDataType.Timestamp
            '                aFieldDesc.Title = "creation Date"
            '                aFieldDesc.ColumnName = ConstFNCreatedOn
            '                aFieldDesc.ID = String.empty
            '                Call .AddFieldDesc(fielddesc:=aFieldDesc)
            '                ' Index
            '                Call .AddIndex("PrimaryKey", PrimaryColumnNames, isprimarykey:=True)


            '                ' persist
            '                .Persist()
            '                ' change the database
            '                .CreateObjectSchema()
            '            End With
            '            ' reset the Table description
            '            If Not Me.Record.SetTable(ourTableName, forceReload:=True) Then
            '                Call CoreMessageHandler(subname:="clsDependency.createSchema", tablename:=ourTableName, _
            '                                      message:="Error while setTable in createSchema")
            '            End If

            '            '
            '            createSchema = True
            '            Exit Function


            '            ' Handle the error
            'error_handle:
            '            Call CoreMessageHandler(subname:="clsOTDBBOM.createSchema", tablename:=ourTableName)
            '            createSchema = False
        End Function

        '**** persist
        '****


        Public Function Persist(Optional timestamp As Date = ot.ConstNullDate) As Boolean

            '* lazy init
            If Not Me.IsInitialized Then
                If Not Me.initialize() Then
                    Persist = False
                    Exit Function
                End If
            End If

            'On Error GoTo errorhandle
            Call Me.Record.SetValue("status", s_status)

            Call Me.Record.SetValue("typeid", s_typeid)
            Call Me.Record.SetValue("clusterid", s_clusterid)
            Call Me.Record.SetValue("clusterlevel", s_maxlevel)
            Call Me.Record.SetValue("ctype", s_clustertype)
            Call Me.Record.SetValue("size", s_size)
            'Call me.record.setValue(OTDBConst_UpdateOn, (Date & " " & Time)) not necessary

            Persist = Me.Record.Persist(timestamp)

            Exit Function

errorhandle:

            Persist = False

        End Function

        '**** getSizeMax
        '****
        Public Function getSizeMax(ByRef size As Long, ByRef max As Long) As Boolean
            Dim otdbcn As ADODB.Connection
            Dim rst As ADODB.Recordset
            Dim tableid As String
            Dim cmdstr As String
            Dim mysize As Long, mymax As Long

            Dim i As Integer
            Dim j As Integer
            Dim aDependCheck As New clsOTDBDependCheck
            Dim aDependMember As New clsOTDBDependMember


            If Me.IsLoaded And Not Me.IsCreated Then
                getSizeMax = False
                Exit Function
            End If

            ' Connection
            '*** TO DO
            'otdbcn = ADOConnection
            If otdbcn Is Nothing Then
                getSizeMax = False
                Exit Function
            End If

            On Error GoTo error_handle
            rst = New ADODB.Recordset

            ' get
            If Me.isDynamic Then
                tableid = aDependCheck.PrimaryTableID
            Else
                tableid = aDependMember.PrimaryTableID
            End If
            ''' TODO
            ''' 
            cmdstr = "SELECT count(*), max(clusterlevel) from " & aDependCheck.DatabaseDriver.GetNativeTableName(tableid) & " where clusterid='" & Me.clusterid & "' and typeid='" & Me.typeid & "'"

            rst.Open(cmdstr, otdbcn, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic)
            If Not rst.EOF Then
                'For i = LBound(dims) To UBound(dims)
                If Not IsNull(rst.Fields(0).Value) And IsNumeric(rst.Fields(0).Value) Then
                    mysize = CLng(rst.Fields(0).Value)
                Else
                    mysize = 0
                End If

                If Not IsNull(rst.Fields(1).Value) And IsNumeric(rst.Fields(1).Value) Then
                    mymax = CLng(rst.Fields(1).Value)
                Else
                    mymax = 0
                End If
                getSizeMax = True

            Else
                getSizeMax = False
            End If

            ' close
            rst.Close()
            '
            Me.maxlevel = mymax
            Me.size = mysize
            size = Me.size
            max = Me.maxlevel
            '*


            Exit Function

            ' Handle the error
error_handle:
            Call CoreMessageHandler(showmsgbox:=False, subname:="OTDB_Doc9.getSizeMax")
            getSizeMax = False
        End Function
        '**** create : create a new Object with primary keys
        '****
        Public Function create(ByVal TYPEID As String, ByVal clusterid As String) As Boolean
            Dim aTable As iormDataStore
            Dim pkarry(2) As Object
            Dim aRecord As ormRecord

            If IsLoaded Then
                create = False
                Exit Function
            End If

            ' Check
            ' set the primaryKey
            pkarry(0) = TYPEID
            pkarry(1) = clusterid
            aTable = GetTableStore(ourTableName)
            aRecord = aTable.GetRecordByPrimaryKey(pkarry)

            If Not aRecord Is Nothing Then
                create = False
                'Call OTDBErrorHandler(tablename:=ourTableName, entryname:="partid, posno", _
                'subname:="clsOTDBBOMMember.create", message:=" double key as should be unique", arg1:=partid & posno)
                Exit Function
            End If

            ' set the primaryKey
            s_typeid = TYPEID
            s_clusterid = clusterid

            'me.iscreated = True
            create = Me.IsCreated

        End Function


    End Class

End Namespace