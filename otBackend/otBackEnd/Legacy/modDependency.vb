

REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** MODULE FOR Dependency static functions
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
Imports System.Diagnostics
Imports System.Linq
Imports System.Diagnostics.Debug
Imports System.ComponentModel
Imports OnTrack
Imports OnTrack.Database
Imports OnTrack.Deliverables
Imports OnTrack.Parts

' ***************************************************************************************************
'   Module for OnTrack Dependencies
'
'   Author: B.Schneider
'   created: 2013-03-09
'
'   change-log:
' ***************************************************************************************************

Namespace OnTrack.Scheduling

    Public Module Dependency

        '*** the clustertypeids in directory, item is a depend clusterids per type id
        '*** the clusterids in directory, item is a dictionary of same ids (found during generation)
        'Private DependClusterIds As New Dictionary
        Private DependClusterTypeIDs As New Dictionary(Of String, Dictionary(Of String, Object))


        '***** initializeCluster
        '*****
        ''' <summary>
        ''' Initialize a cluster
        ''' </summary>
        ''' <param name="FORCE"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InitializedCluster(Optional ByVal force As Boolean = False) As Boolean

            If DependClusterTypeIDs Is Nothing Or FORCE Then
                DependClusterTypeIDs = New Dictionary(Of String, Dictionary(Of String, Object))
            End If
            If DependClusterTypeIDs.Count = 0 Then
                Call DependClusterTypeIDs.Add(key:=ConstDepTypeIDIFC, value:=New Dictionary(Of String, Object))
            End If

            InitializedCluster = True
        End Function

        '***** createclusterID returns a ClusterID for the DependTypeID
        '*****
        ''' <summary>
        ''' create a clusterID
        ''' </summary>
        ''' <param name="aDependTypeId"></param>
        ''' <param name="isDynamic"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateClusterID(aDependTypeId As String, Optional isDynamic As Boolean = False) As String
            Dim aDependClusterIDs As Dictionary(Of String, Object)
            Dim keys As Object
            Dim aKey As String
            Dim i As Integer
            Dim max As Long

            If initializedCluster() Then
                If DependClusterTypeIDs.ContainsKey(key:=aDependTypeId) Then
                    aDependClusterIDs = DependClusterTypeIDs.Item(key:=aDependTypeId)
                Else
                    Call DependClusterTypeIDs.Add(key:=aDependTypeId, value:=New Dictionary(Of String, Object))
                    aDependClusterIDs = DependClusterTypeIDs.Item(key:=aDependTypeId)
                End If

                keys = aDependClusterIDs.Keys
                ' no keys
                If Not IsArrayInitialized(keys) And Not isDynamic Then
                    aKey = "C00001"
                ElseIf Not IsArrayInitialized(keys) And isDynamic Then
                    aKey = "D00001"
                Else
                    For i = LBound(keys) To UBound(keys)
                        If max <= CLng(Mid(keys(i), 2)) Then
                            max = CLng(Mid(keys(i), 2))
                        End If
                    Next i
                    If isDynamic Then
                        aKey = "D" & Format(max + 1, "0#####")
                    Else
                        aKey = "C" & Format(max + 1, "0#####")
                    End If
                End If
                Call aDependClusterIDs.Add(key:=aKey, value:=New Dictionary(Of String, Object))

                createClusterID = aKey
                Exit Function
            End If

            createClusterID = String.empty
        End Function

        '***** markclusterID marks a ClusterID in the Dictionary in the Collection for same
        '*****
        ''' <summary>
        ''' mark a clusterID in the tree
        ''' </summary>
        ''' <param name="aDependTypeId"></param>
        ''' <param name="aClusterID"></param>
        ''' <param name="aSameClusterID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MarkClusterID(aDependTypeId As String, aClusterID As String, aSameClusterID As String) As Boolean
            Dim aDependClusterIDs As Dictionary(Of String, Object)
            Dim aSameKeysColl As Dictionary(Of String, Object)

            Dim aKey As Object
            Dim i As Integer
            Dim max As Long

            If initializedCluster() Then
                If DependClusterTypeIDs.ContainsKey(key:=aDependTypeId) Then
                    aDependClusterIDs = DependClusterTypeIDs.Item(key:=aDependTypeId)
                Else
                    Call DependClusterTypeIDs.Add(key:=aDependTypeId, value:=New Dictionary(Of String, Object))
                    aDependClusterIDs = DependClusterTypeIDs.Item(key:=aDependTypeId)
                End If

                ' look what we have
                For Each aKey In aDependClusterIDs.Keys
                    aSameKeysColl = aDependClusterIDs.Item(key:=aKey)
                    If aSameKeysColl.ContainsKey(key:=aClusterID) And Not aSameKeysColl.ContainsKey(key:=aSameClusterID) Then
                        Call aSameKeysColl.Add(value:=aKey, key:=aSameClusterID)
                    End If
                Next aKey

                'markClusterID = True
                'Exit Function

                If aDependClusterIDs.ContainsKey(key:=aClusterID) Then
                    aSameKeysColl = aDependClusterIDs.Item(key:=aClusterID)
                    If Not aSameKeysColl.ContainsKey(key:=aSameClusterID) Then
                        Call aSameKeysColl.Add(value:=aClusterID, key:=aSameClusterID)
                        System.Diagnostics.Debug.WriteLine(aClusterID & "=" & aSameClusterID)
                    End If
                    markClusterID = True
                    Exit Function
                End If
            End If

            markClusterID = False
        End Function

        '***** saveClusterID -> saves the clusterid as object
        '*****
        ''' <summary>
        ''' save the clusterID
        ''' </summary>
        ''' <param name="aDependTypeId"></param>
        ''' <param name="isDynamic"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveClusterIDs(aDependTypeId As String, Optional isDynamic As Boolean = False) As Boolean
            Dim aCluster As New clsOTDBCluster
            Dim max As Long
            Dim count As Long
            Dim keys As Object
            Dim aKey As Object
            Dim aDependClusterIDs As Dictionary(Of String, Object)

            If DependClusterTypeIDs.ContainsKey(key:=aDependTypeId) Then
                aDependClusterIDs = DependClusterTypeIDs.Item(key:=aDependTypeId)
            Else
                saveClusterIDs = False
                Exit Function
            End If

            keys = aDependClusterIDs.Keys

            For Each aKey In keys

                If Not aCluster.Inject(TYPEID:=aDependTypeId, clusterid:=aKey) Then
                    Call aCluster.create(aDependTypeId, clusterid:=aKey)
                End If

                aCluster.isDynamic = isDynamic
                If aCluster.getSizeMax(count, max) Then
                    aCluster.Persist()
                End If

            Next aKey

            '
            saveClusterIDs = True

        End Function


        '***** markclusterID marks a ClusterID in the Dictionary in the Collection for same
        '*****
        ''' <summary>
        ''' updates same clusterIDs
        ''' </summary>
        ''' <param name="aDependTypeId"></param>
        ''' <param name="isDynamic"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateSameClusterID(aDependTypeId As String, Optional isDynamic As Boolean = False) As Boolean
            Dim aDependClusterIDs As Dictionary(Of String, Object)
            Dim aSameKeysColl As Dictionary(Of String, Object)
            Dim doneDir As New Dictionary(Of String, Object)
            Dim aDepend As New clsOTDBDependency
            Dim keys As Object
            Dim samekey As Object
            Dim aKey As Object
            Dim i As Integer
            Dim max As Long


            If DependClusterTypeIDs.ContainsKey(key:=aDependTypeId) Then
                aDependClusterIDs = DependClusterTypeIDs.Item(key:=aDependTypeId)
            Else
                updateSameClusterID = False
                Exit Function
            End If

            keys = aDependClusterIDs.Keys

            For Each aKey In keys
                If aDependClusterIDs.ContainsKey(key:=aKey) And Not doneDir.ContainsKey(key:=aKey) Then
                    aSameKeysColl = aDependClusterIDs.Item(key:=aKey)
                    For Each samekey In aSameKeysColl.Keys
                        If Not doneDir.ContainsKey(key:=samekey) Then
                            Call aDepend.unionClusters(aDependTypeId, aClusterID:=aKey, aNotherClusterID:=samekey, isDynamic:=isDynamic)
                            Call doneDir.Add(key:=samekey, value:=aKey)

                        End If
                    Next samekey
                End If
            Next aKey


            '
            updateSameClusterID = True
        End Function

        '***** testme
        '*****
        'Public Sub testme()
        '    Dim aDependency As New clsOTDBDependency
        '    Dim aPart As New Part
        '    Dim clusterid As String

        '    Call aPart.Inject("3H03-391025-000")

        '    aDependency = New clsOTDBDependency
        '    If Not aDependency.loadbyDependant(aPart.PartID) Then
        '        aDependency.create(aPart.PartID)
        '        'Else
        '        '    aDependency.delete
        '    End If
        '    clusterid = aDependency.clusterid(ConstDepTypeIDIFC)
        '    'If clusterid = String.empty Then
        '    clusterid = createClusterID(ConstDepTypeIDIFC)
        '    Call aDependency.generateCluster(ConstDepTypeIDIFC, aClusterID:=clusterid, aLevel:=1)
        '    'End If
        '    If aPart.CreateDependencyFromInterfaces(aDependency) Then
        '        aDependency.Persist()
        '    End If
        'End Sub



        '***********
        '*********** Build Dependencies
        '***********
        ''' <summary>
        ''' Build Dependencies for all Parts
        ''' </summary>
        ''' <remarks></remarks>
        Public Function BuildDependencyNet(Optional workerthread As BackgroundWorker = Nothing) As Boolean
            Dim aPartsColl As New List(Of Part)
            Dim aPart As New Part
            'Dim aProgressBar As New clsUIProgressBarForm
            Dim aSenderColl As New Collection
            Dim aReceiverColl As New Collection
            Dim aSenderPart As New Part
            Dim aReceiverPart As New Part
            Dim aDependency As New clsOTDBDependency
            Dim flag As Boolean

            '*** TODO
            If Not CurrentSession.RequireAccessRight(accessRequest:=otAccessRight.ReadUpdateData) Then
                CoreMessageHandler(message:="dependency net not build due to missing rights", _
                                   messagetype:=otCoreMessageType.ApplicationInfo, subname:="Dependency.BuildDependencyNet")
                Return False
            End If

            If workerthread IsNot Nothing Then
                workerthread.ReportProgress(0, "#1 collecting parts")
            End If
            aPartsColl = aPart.all(isDeleted:=False)
            If aPartsColl Is Nothing Or aPartsColl.Count = 0 Then
                workerthread.ReportProgress(100, "#2 no parts collected")
                Return False
            End If

            Dim maximum As Long = aPartsColl.Count
            Dim progress As Long = 1

            ' init
            If workerthread IsNot Nothing Then
                workerthread.ReportProgress(0, "#2 " & maximum & " parts received")
            End If


            For Each aPart In aPartsColl
                'Call aProgressBar.progress(1, Statustext:=aPart.PARTID)
                If workerthread IsNot Nothing Then
                    progress += 1
                    workerthread.ReportProgress((progress / maximum) * 100, "#3 building net progress: " & String.Format("{0:0%}", (progress / maximum)))
                End If

                '** get the dependencies
                '**
                aDependency = New clsOTDBDependency
                If Not aDependency.loadbyDependant(aPart.PartID) Then
                    aDependency.create(aPart.PartID)
                ElseIf aDependency.NoMembers(typeid:=ConstDepTypeIDIFC) > 0 Then
                    aDependency.delete()
                End If
                If aPart.CreateDependencyFromInterfaces(aDependency) Then
                    aDependency.Persist()
                End If

            Next aPart

            If workerthread IsNot Nothing Then
                progress += 1
                workerthread.ReportProgress(100, "#4 building net progress finished ")
            End If

            CoreMessageHandler(message:="dependency net rebuild", messagetype:=otCoreMessageType.ApplicationInfo, subname:="Dependency.BuildDependencyNet")
            Return True
            'aProgressBar.closeForm()
        End Function

        '***********
        '*********** Build Dependency Clusters
        '***********

        Public Sub BuildDependencyCluster()
            Dim aDepColl As New Collection
            Dim aPart As New Part
            'Dim aProgressBar As New clsUIProgressBarForm
            Dim aSenderColl As New Collection
            Dim aReceiverColl As New Collection
            Dim aSenderPart As New Part
            Dim aReceiverPart As New Part
            Dim aDependency As New clsOTDBDependency
            Dim aDepMember As New clsOTDBDependMember
            Dim clusterid As String
            Dim flag As Boolean

            If Not CurrentSession.RequireAccessRight(accessRequest:=otAccessRight.ReadUpdateData) Then
                Exit Sub
            End If

          
            aDepColl = aDepMember.allHeadsByTypeID(ConstDepTypeIDIFC)
            If aDepColl Is Nothing Or aDepColl.Count = 0 Then
                Exit Sub
            End If

            ' init
            Call initializedCluster(FORCE:=True)
            aDependency.clearAllClusters(ConstDepTypeIDIFC)
            'Call aProgressBar.initialize(aDepColl.Count, WindowCaption:="building dependency clusters ...")
            'aProgressBar.showForm()

            For Each aDepMember In aDepColl
                'Call aProgressBar.progress(1, Statustext:=aDepMember.PARTID)

                '** get the dependencies
                '**
                aDependency = New clsOTDBDependency
                If aDependency.loadbyDependant(aDepMember.PARTID) Then
                    clusterid = aDependency.clusterid(ConstDepTypeIDIFC)
                    If clusterid = String.empty Then
                        clusterid = createClusterID(ConstDepTypeIDIFC)
                        Call aDependency.generateCluster(ConstDepTypeIDIFC, aClusterID:=clusterid, aLevel:=1)
                    End If

                    'Call aProgressBar.showStatus(clusterid)

                End If

            Next aDepMember

            ' now set the clusters as the same
            Call updateSameClusterID(ConstDepTypeIDIFC)
            Call saveClusterIDs(ConstDepTypeIDIFC, isDynamic:=False)

            'aProgressBar.closeForm()
        End Sub

        '***********
        '*********** Build isDynamic Dependency Clusters
        '***********
        ''' <summary>
        ''' builds dynamic dependency clusters out of the dependencies net
        ''' </summary>
        ''' <param name="workspaceID"></param>
        ''' <remarks></remarks>
        Public Function BuildDynamicDependencyCluster(Optional ByVal workspaceID As String = String.empty, Optional workerthread As BackgroundWorker = Nothing) As Boolean
            Dim aDepColl As New Collection
            Dim aPart As New Part
            Dim aSenderColl As New Collection
            Dim aReceiverColl As New Collection
            Dim aSenderPart As New Part
            Dim aReceiverPart As New Part
            Dim aDependency As New clsOTDBDependency
            Dim aDepMember As New clsOTDBDependMember
            Dim clusterid As String



            If Not CurrentSession.RequireAccessRight(accessRequest:=otAccessRight.ReadUpdateData) Then
                CoreMessageHandler(message:="dependency net not build due to missing rights", _
                                   messagetype:=otCoreMessageType.ApplicationInfo, subname:="Dependency.BuildDynamicDependencyCluster")
                Return False
            End If

            If workspaceID = String.empty Then
                workspaceID = CurrentSession.CurrentWorkspaceID
            End If

            If workerthread IsNot Nothing Then
                workerthread.ReportProgress(0, "#1 collecting net heads")
            End If

            aDepColl = aDepMember.allHeadsByTypeID(ConstDepTypeIDIFC)
            If aDepColl Is Nothing Or aDepColl.Count = 0 Then
                If workerthread IsNot Nothing Then
                    workerthread.ReportProgress(100, "#2 collected empty net heads")
                End If
                Return False
            End If

            Dim maximum As Long = aDepColl.Count
            Dim progress As Long = 1

            ' init
            If workerthread IsNot Nothing Then
                workerthread.ReportProgress(0, "#2 " & maximum & " net heads received")
            End If

            ' init
            Call InitializedCluster(True)
            Call aDependency.clearAllClusters(ConstDepTypeIDIFC, isDynamic:=True)

            ' init
            If workerthread IsNot Nothing Then
                workerthread.ReportProgress(0, "#3 all clusters cleared")
            End If

            For Each aDepMember In aDepColl
                
                If workerthread IsNot Nothing Then
                    progress += 1
                    workerthread.ReportProgress((progress / maximum) * 100, "#3 building cluster progress: " & String.Format("{0:0%}", (progress / maximum)))
                End If

                '** get the dependencies
                '**
                aDependency = New clsOTDBDependency
                If aDependency.loadbyDependant(aDepMember.PARTID) Then
                    clusterid = aDependency.DynClusterid(ConstDepTypeIDIFC, workspaceID:=workspaceID)
                    If clusterid = String.empty Then
                        clusterid = CreateClusterID(ConstDepTypeIDIFC, isDynamic:=True)
                        Call aDependency.GenerateDynCluster(ConstDepTypeIDIFC, clusterid:=clusterid, level:=1, workspaceID:=workspaceID)
                    End If

                End If

            Next aDepMember

            If workerthread IsNot Nothing Then
                workerthread.ReportProgress(100, "#4 saving clusters ")
            End If

            ' now set the clusters as the same
            Call UpdateSameClusterID(ConstDepTypeIDIFC, isDynamic:=True)
            Call SaveClusterIDs(ConstDepTypeIDIFC, isDynamic:=True)

            If workerthread IsNot Nothing Then
                workerthread.ReportProgress(100, "#5 dynamic clusters built ")
            End If

            CoreMessageHandler(message:="dynamic dependency cluster rebuild", messagetype:=otCoreMessageType.ApplicationInfo, subname:="Dependency.BuildDynamicDependencyCluster")
            Return True

        End Function

        '***********
        '*********** run Dependencies Checks
        '***********
        ''' <summary>
        ''' check the dependiencies for all parts in a workspaceID if they are hold
        ''' and create dependency check objects
        ''' </summary>
        ''' <param name="workspaceID"></param>
        ''' <remarks></remarks>
        Public Function CheckAllDependencies(Optional ByVal workspaceID As String = String.empty, Optional workerthread As BackgroundWorker = Nothing) As Boolean
            Dim aPartsColl As New List(Of Part)
            Dim aPart As New Part
            'Dim aProgressBar As New clsUIProgressBarForm
            Dim aSenderColl As New Collection
            Dim aReceiverColl As New Collection
            Dim aSenderPart As New Part
            Dim aReceiverPart As New Part
            Dim aDependency As New clsOTDBDependency
            Dim flag As Boolean

#If ProjectVersion > 0 Then
    Application.Calculation = pjManual
    Application.ScreenUpdating = False
#End If

            If Not CurrentSession.RequireAccessRight(accessRequest:=otAccessRight.ReadUpdateData) Then
                CoreMessageHandler(message:="dependencies not checked due to missing rights", _
                                  messagetype:=otCoreMessageType.ApplicationInfo, subname:="Dependency.CheckAllDependencies")
                Return False
                Exit Function
            End If

            If workspaceID = String.empty Then
                workspaceID = CurrentSession.CurrentWorkspaceID
            End If

            If workerthread IsNot Nothing Then
                workerthread.ReportProgress(0, "#1 collecting all parts")
            End If

            aPartsColl = aPart.all(isDeleted:=False)
            If aPartsColl Is Nothing Or aPartsColl.Count = 0 Then
                If workerthread IsNot Nothing Then
                    workerthread.ReportProgress(100, "#2 collected no parts")
                End If
                Return False
            End If

            Dim maximum As Long = aPartsColl.Count
            Dim progress As Long = 1

            ' init
            If workerthread IsNot Nothing Then
                workerthread.ReportProgress(0, "#2 " & maximum & " parts received")
            End If
           
            '* go through all parts
            For Each aPart In aPartsColl
                If workerthread IsNot Nothing Then
                    progress += 1
                    workerthread.ReportProgress((progress / maximum) * 100, "#3 checking parts progress: " & String.Format("{0:0%}", (progress / maximum)))
                End If

                '** get the dependencies
                aDependency = New clsOTDBDependency
                If aDependency.loadbyDependant(aPart.PartID) Then
                    If aDependency.RunCheck(ConstDepTypeIDIFC, workspaceID:=workspaceID, autopersist:=True) Then
                    End If
                End If

            Next aPart

            If workerthread IsNot Nothing Then
                workerthread.ReportProgress(100, "#5 all dependencies checked")
            End If

            CoreMessageHandler(message:="dependency checked", messagetype:=otCoreMessageType.ApplicationInfo, _
                               subname:="Dependency.CheckAllDependencies")
            Return True


#If ProjectVersion > 0 Then
    'Application.Calculation = pjManual
    Application.ScreenUpdating = True
#End If

        End Function

        '***********
        '*********** check dependencies on part
        '***********
        ''' <summary>
        ''' check dependencies for a part
        ''' </summary>
        ''' <param name="partID"></param>
        ''' <param name="dependency"></param>
        ''' <param name="workspaceID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CheckDependenciesFor(ByVal partID As String, _
                                             ByRef dependency As clsOTDBDependency, _
                                             Optional workspaceID As String = String.empty) As Boolean

            Dim aPart As Part = Part.Retrieve(partID)

            Dim aSenderColl As New Collection
            Dim aReceiverColl As New Collection
            Dim aSenderPart As New Part
            Dim aReceiverPart As New Part
            ' Dim aDependency As New clsOTDBDependency
            Dim flag As Boolean

            ' load the part
            If aPart Is Nothing Then
                CheckDependenciesFor = False
                Exit Function
            End If

            If IsMissing(workspaceID) Then
                workspaceID = CurrentSession.CurrentWorkspaceID
            Else
                workspaceID = CStr(workspaceID)
            End If

            '** get the dependencies
            '**
            dependency = New clsOTDBDependency
            If dependency.loadbyDependant(aPart.PartID) Then
                If dependency.runCheck(ConstDepTypeIDIFC, workspaceID, autopersist:=False) Then
                End If
            End If

            CheckDependenciesFor = True
        End Function


    End Module

End Namespace