' ***************************************************************************************************
'   Module for Overloading a OTDBRecordset with Data out of the local Data container
'
'   Author: B.Schneider
'   created: 2013-03-09
'
'   change-log:
' ***************************************************************************************************
Option Explicit On

Imports OnTrack
Imports OnTrack.Database
Imports OnTrack.Deliverables
Imports OnTrack.Scheduling

Public Module modHostData

    Private s_OverloadingRegistry As New Dictionary(Of String, Boolean)
    Private s_suspendOverloading As Boolean

    '************* isOverloadingSuspended
    '*************
    Public Function isOverloadingSuspended() As Boolean
        isOverloadingSuspended = s_suspendOverloading

    End Function

    '************ suspendOverloading
    '************
    Public Function SuspendOverloading(ByVal ONOFF As Boolean) As Boolean
        s_suspendOverloading = ONOFF
        SuspendOverloading = isOverloadingSuspended()
    End Function
    '************* registerDefaultObjects registers all Default Objects to be in runtime
    '*************
    Public Function registerDefaultObjects() As Boolean
        Dim aSchedule As New ScheduleEdition
        'Dim aDependCheck As New clsOTDBDependCheck

        Call registerHostApplicationFor(aSchedule.ObjectPrimaryTableID, True)
        'Call registerHostApplicationFor(aDependCheck.primaryTableID, True)

        registerDefaultObjects = True
    End Function

    '************* unregisterDefaultObjects unregisters all Default Objects to be in runtime
    '*************
    Public Function unregisterDefaultObjects() As Boolean
        Dim aSchedule As New ScheduleEdition
        ' Dim aDependCheck As New clsOTDBDependCheck

        Call unregisterHostApplicationFor(aSchedule.ObjectPrimaryTableID)
        'Call unregisterHostApplicationFor(aDependCheck.primaryTableID)

        unregisterDefaultObjects = True
    End Function

    '************* isregistered a tabletag to be overloaded / overwritten to the local application
    '*************
    Public Function isRegisteredAtHostApplication(ByVal atabletag As String) As Boolean

        If s_OverloadingRegistry.ContainsKey(key:=atabletag) And Not isOverloadingSuspended() Then
            isRegisteredAtHostApplication = True
            Exit Function
        End If

        isRegisteredAtHostApplication = False
    End Function
    '************* isDefaultSerialize a tabletag to be overloaded / overwritten to the local application
    '*************
    Public Function isDefaultSerializeAtHostApplication(ByVal atabletag As String) As Boolean

        If s_OverloadingRegistry.ContainsKey(key:=atabletag) Then
            isDefaultSerializeAtHostApplication = s_OverloadingRegistry.Item(key:=atabletag)
            Exit Function
        End If

        isDefaultSerializeAtHostApplication = False
    End Function


    '************* register a tabletag to be overloaded / overwritten to the local application
    '************* saves also a default value if new objects of that tag should serialize AllObjectSerialize

    Public Function registerHostApplicationFor(ByVal atabletag As String, Optional ByVal AllObjectSerialize As Boolean = True) As Boolean

        If Not s_OverloadingRegistry.ContainsKey(key:=atabletag) Then
            Call s_OverloadingRegistry.Add(key:=atabletag, value:=AllObjectSerialize)
            registerHostApplicationFor = True
            Exit Function
        End If

        registerHostApplicationFor = False
    End Function

    '************* unregister a tabletag to be overloaded / overwritten to the local application
    '*************
    Public Function unregisterHostApplicationFor(ByVal atabletag As String) As Boolean

        If s_OverloadingRegistry.ContainsKey(key:=atabletag) Then
            Call s_OverloadingRegistry.Remove(key:=atabletag)
            unregisterHostApplicationFor = True
            Exit Function
        End If

        unregisterHostApplicationFor = False
    End Function
    '************* overload aRecord with data from the local Application data container
    '*************

    Public Function overloadFromHostApplication(ByRef aRecord As ormRecord) As Boolean
        Dim aSchedule As New ScheduleEdition
        'Dim aDependCheck As New clsOTDBDependCheck

        ' if not registered
        'If Not isRegisteredAtHostApplication(aRecord.TableIDs) Then
        '           overloadFromHostApplication = False
        'Exit Function
        'End If

'        Select Case LCase(aRecord.TableIDs)

'            Case LCase(aSchedule.PrimaryTableID)
'#If ExcelVersion <> String.empty Then
'                ' write it to
'                'Debug.Print "excel"
'#End If
'#If projectVersion Then
'        If Not overloadScheduleFromMSP(aRecord) Then
'        End If
'#End If
'            Case LCase(aDependCheck.PrimaryTableID)

'#If projectVersion Then
'        If Not overloadScheduleFromMSP(aRecord) Then
'        End If
'#End If
'                ' load it from
'        End Select

        overloadFromHostApplication = True
    End Function

    '************* overload aRecord with data from the local Application data container
    '*************

    Public Function overwriteToHostApplication(ByRef aRecord As ormRecord) As Boolean
        Dim aSchedule As New ScheduleEdition
        'Dim aDependCheck As New clsOTDBDependCheck

        ' if not registered
'        If Not isRegisteredAtHostApplication(aRecord.TableIDs) Then
'            overwriteToHostApplication = False
'            Exit Function
'        End If


'        Select Case LCase(aRecord.TableIDs)
'            Case LCase(aSchedule.primaryTableID)
'#If ExcelVersion <> String.empty Then
'                ' write it to
'                'Debug.Print "excel"
'#End If
'#If ProjectVersion Then

'        If Not overwriteScheduleToMSP(aRecord) Then
'        End If
'#End If
'            Case LCase(aDependCheck.primaryTableID)

'#If projectVersion Then
'        If Not overwriteDependCheckToMSP(aRecord) Then
'        End If
'#End If
'        End Select

        overwriteToHostApplication = True
    End Function
End Module
