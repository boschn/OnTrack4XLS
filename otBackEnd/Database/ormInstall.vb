REM ***********************************************************************************************************************************************
REM *********** CREATE SCHEMA DATABASE MODULE for On Track Database Backend Library
REM ***********
REM *********** Version: X.YY
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************
Imports OnTrack.Scheduling
Imports OnTrack.Deliverables
Imports OnTrack.Parts
Imports OnTrack.Configurables
Imports OnTrack.XChange
Imports OnTrack.Calendar
Imports OnTrack.Commons
Imports OnTrack.Core

Namespace OnTrack.Database

    Public Module Installation

        ''' <summary>
        ''' creates the schema and persist for a list of objects
        ''' </summary>
        ''' <param name="objects"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CreateAndPersist(objects As IEnumerable(Of String), Optional force As Boolean = False) As Boolean
            Dim theObjects As New List(Of ormObjectDefinition)
            Dim result As Boolean = True

            For Each anObjectID In objects
                ''' from repository needed -> either build-in (ObjectClasses) or loaded
                Dim anObjectDefinition = ot.CurrentSession.Objects.GetObjectDefinition(id:=anObjectID, runtimeOnly:=CurrentSession.IsBootstrappingInstallationRequested)
                If anObjectDefinition IsNot Nothing Then
                    theObjects.Add(anObjectDefinition)
                End If
            Next

            '*** create all the schema for the objects
            For Each anobjectdefinition In theObjects
                result = result And anobjectdefinition.CreateObjectSchema(silent:=True)
                If result Then
                    Call ot.CoreMessageHandler(procedure:="createDatabase.CreateAndPersist", _
                                                           message:="Schema for  Object " & anobjectdefinition.ID & " updated or created to version " & anobjectdefinition.Version & ". Tables created or updated:" & Core.DataType.ToString(anobjectdefinition.Tablenames), _
                                                           messagetype:=otCoreMessageType.ApplicationInfo, _
                                                           objectname:=anobjectdefinition.ID, noOtdbAvailable:=True)
                Else
                    Call ot.CoreMessageHandler(procedure:="createDatabase.CreateAndPersist", showmsgbox:=True, _
                                                             message:="Schema for  Object " & anobjectdefinition.ID & " could not be updated nor created ! - Contact your administrator ", _
                                                             messagetype:=otCoreMessageType.InternalError, _
                                                             noOtdbAvailable:=True, objectname:=anobjectdefinition.ID)
                    Return result
                End If
            Next

            '** persist the objectdefinition
            For Each anobjectdefinition In theObjects
                '** switch off RuntimeMode
                If Not anobjectdefinition.SwitchRuntimeOff() Then
                    Call ot.CoreMessageHandler(procedure:="createDatabase.CreateAndPersist", showmsgbox:=True, _
                                                           message:="Runtime for  Object " & anobjectdefinition.ID & " could not be switched off ! - Contact your administrator ", _
                                                           messagetype:=otCoreMessageType.InternalError, _
                                                          noOtdbAvailable:=True, objectname:=anobjectdefinition.ID)
                    Return result
                End If
                result = result And anobjectdefinition.Persist()
                If result Then
                    Call ot.CoreMessageHandler(procedure:="createDatabase.CreateAndPersist", _
                                                           message:="Schema for  Object " & anobjectdefinition.ID & " persisted.", _
                                                           messagetype:=otCoreMessageType.ApplicationInfo, _
                                                           objectname:=anobjectdefinition.ID, noOtdbAvailable:=True)
                Else
                    Call ot.CoreMessageHandler(procedure:="createDatabase.CreateAndPersist", showmsgbox:=True, _
                                                             message:="Schema for  Object " & anobjectdefinition.ID & " could not be peristed ! - Contact your administrator ", _
                                                             messagetype:=otCoreMessageType.InternalError, _
                                                            noOtdbAvailable:=True, objectname:=anobjectdefinition.ID)
                    Return result
                End If
            Next

            Return result
        End Function
        ''' <summary>
        ''' Creates or updates all the Database Schema for all objects or a subset
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CreateDatabase(Optional modules As IEnumerable(Of String) = Nothing, Optional force As Boolean = False, Optional setupid As String = Nothing)

            Dim aNativeConnection = CurrentOTDBDriver.CurrentConnection.NativeConnection
            Dim repersistnecessary As Boolean = False
            Dim result As Boolean = True
            If String.IsNullOrWhiteSpace(setupid) Then setupid = ot.CurrentSetupID

            '** verify database bootstrap in detail to check if bootstrap is needed
            If Not CurrentSession.IsBootstrappingInstallationRequested Then
                CurrentOTDBDriver.VerifyOnTrackDatabase(install:=False, modules:=Nothing, verifySchema:=True) 'this will not ask to install but check on bootstrapping necessary
            End If
            '** create the db table
            result = result And CurrentOTDBDriver.CreateDBParameterContainer(nativeConnection:=aNativeConnection)

            '*** get the current schema version
            Dim schemaversion = CurrentOTDBDriver.GetDBParameter(parametername:=ConstPNBSchemaVersion, setupID:=setupid, silent:=True)
            If schemaversion Is Nothing OrElse Not IsNumeric(schemaversion) Then
                Call CoreMessageHandler(message:="No schema version for database available - assuming first time installation", messagetype:=otCoreMessageType.InternalInfo, _
                                               procedure:="Installation.createDatabase")
            ElseIf Convert.ToUInt64(schemaversion) < ot.SchemaVersion Then
                Call CoreMessageHandler(message:="Schema version for database available - assuming upgrade installation", messagetype:=otCoreMessageType.InternalInfo, _
                                               procedure:="Installation.createDatabase", argument:=schemaversion)
            ElseIf Convert.ToUInt64(schemaversion) > ot.SchemaVersion Then
                Call CoreMessageHandler(message:="Schema version for database available but higher ( " & schemaversion & " ) - downgrading ?!", messagetype:=otCoreMessageType.InternalInfo, _
                                               procedure:="Installation.createDatabase", argument:=ot.SchemaVersion)
            Else
                Call CoreMessageHandler(message:="Schema version for database available - assuming repair installation", messagetype:=otCoreMessageType.InternalInfo, _
                                               procedure:="Installation.createDatabase", argument:=schemaversion)
            End If

            '** create the bootstrapping 
            '**
            Dim descriptions = ot.GetBootStrapObjectClassDescriptions
            Dim objectids As New List(Of String)

            For Each description In descriptions
                Dim addflag As Boolean = False
                ''' create in the current db driver
                For Each aContainerID In description.ContainerIDs
                    Dim aVersion As Long? = CurrentOTDBDriver.ContainerVersion(aContainerID)
                    If aVersion Is Nothing OrElse Not CurrentOTDBDriver.HasContainerID(aContainerID) Then
                        addflag = True
                    ElseIf aVersion > description.GetContainerAttribute(aContainerID).Version Then
                        CoreMessageHandler(message:="WARNING ! Version of Bootstrapping Table in database is higher ( " & aVersion & ") than in class description ( " & description.GetContainerAttribute(aContainerID).Version & "). Downgrading ?!", messagetype:=otCoreMessageType.InternalWarning, _
                                            procedure:="Installation.createDatabase", containerID:=aContainerID, objectname:=description.ID, argument:=description.GetContainerAttribute(aContainerID).Version)
                    ElseIf force OrElse aVersion < description.GetContainerAttribute(aContainerID).Version Then
                        addflag = True
                    End If
                Next

                '** add it
                If addflag Then
                    objectids.Add(description.ID)
                End If
            Next

            '*** create it
            If objectids.Count > 0 Then
                result = result And CreateAndPersist(objectids, force:=force)
                repersistnecessary = True
            Else
                result = result And True
            End If

            '** Create SuperUser
            If Not CurrentSession.OTDBDriver.HasAdminUserValidation Then
                result = result And CurrentOTDBDriver.CreateDBUserDefTable(nativeConnection:=aNativeConnection)
                If result Then
                    Call CoreMessageHandler(message:="Administrator account created ", _
                                            messagetype:=otCoreMessageType.InternalInfo, _
                                            procedure:="Installation.createDatabase", break:=False, noOtdbAvailable:=True)

                Else
                    Call CoreMessageHandler(message:="Administrator Account could not be created - Please see your system administrator.", messagetype:=otCoreMessageType.InternalInfo, _
                                                procedure:="Installation.createDatabase_CoreData", _
                                                break:=False, showmsgbox:=True, noOtdbAvailable:=True)
                    Return
                End If
            End If

            '*** create global domain
            If CurrentOTDBDriver.CreateGlobalDomain(nativeConnection:=aNativeConnection) Then
                Call CoreMessageHandler(message:="global domain created", argument:=ConstGlobalDomain, messagetype:=otCoreMessageType.InternalInfo, _
                                                procedure:="Installation.createDatabase")
            End If

            '*** set objects to load
            Call CurrentOTDBDriver.SetDBParameter(ConstPNObjectsLoad, _
                                                         ScheduleEdition.ConstObjectID & ", " & _
                                                         ScheduleMilestone.ConstObjectID & ", " & _
                                                         Deliverable.ConstObjectID, setupID:=setupid, silent:=True)
            '*** bootstrap checksum
            CurrentOTDBDriver.SetDBParameter(ConstPNBootStrapSchemaChecksum, value:=ot.GetBootStrapSchemaChecksum, setupID:=setupid, silent:=True)

            '**** Create the core objects first
            '****
            If modules.Contains(ConstModuleCommons.ToUpper) Then
                descriptions = ot.GetObjectClassDescriptionsForModule(ConstModuleCommons)
                objectids = New List(Of String)

                For Each description In descriptions
                    Dim addflag As Boolean = False

                    For Each aContainerID In description.ContainerIDs
                        Dim aVersion As Long? = CurrentOTDBDriver.ContainerVersion(aContainerID)
                        If aVersion Is Nothing OrElse Not CurrentOTDBDriver.HasContainerID(aContainerID) Then
                            addflag = True
                        ElseIf aVersion > description.GetContainerAttribute(aContainerID).Version Then
                            CoreMessageHandler(message:="WARNING ! Version of Container in database is higher ( " & aVersion & ") than in class description ( " & description.GetContainerAttribute(aContainerID).Version & "). Downgrading ?!", messagetype:=otCoreMessageType.InternalWarning, _
                                                procedure:="Installation.createDatabase", containerID:=aContainerID, objectname:=description.ID, argument:=description.GetContainerAttribute(aContainerID).Version)
                        ElseIf force OrElse aVersion < description.GetContainerAttribute(aContainerID).Version Then
                            addflag = True
                        End If
                    Next

                    '** add it
                    If (repersistnecessary OrElse addflag) AndAlso Not ot.GetBootStrapObjectClassIDs.Contains(description.ID) Then
                        objectids.Add(description.ID)
                    End If
                Next

                '*** create it
                If objectids.Count > 0 Then
                    result = result And CreateAndPersist(objectids, force:=force)
                Else
                    result = result And True
                End If
            End If

            '**** Create the other modules
            '****
            For Each modulename In modules
                If modulename <> ConstModuleCommons Then
                    descriptions = ot.GetObjectClassDescriptionsForModule(modulename)
                    objectids = New List(Of String)

                    For Each description In descriptions
                        Dim addflag As Boolean = False

                        ''' check other containers in the specific database drivers
                        ''' 
                        For Each aContainerID In description.ContainerIDs
                            For Each aDriver As iormDatabaseDriver In CurrentSession.GetDatabaseDrivers(aContainerID)
                                Dim aVersion As Long? = aDriver.ContainerVersion(aContainerID)
                                If Not aVersion.HasValue OrElse Not aDriver.HasContainerID(aContainerID) Then
                                    addflag = True
                                ElseIf aVersion > description.GetContainerAttribute(aContainerID).Version Then
                                    CoreMessageHandler(message:="WARNING ! Version of Table in database is higher ( " & aVersion & ") than in class description ( " & description.GetContainerAttribute(aContainerID).Version & "). Downgrading ?!", messagetype:=otCoreMessageType.InternalWarning, _
                                                        procedure:="Installation.createDatabase", containerID:=aContainerID, objectname:=description.ID, argument:=description.GetContainerAttribute(aContainerID).Version)
                                ElseIf force OrElse aVersion < description.GetContainerAttribute(aContainerID).Version Then
                                    addflag = True
                                End If
                            Next
                        Next

                        '** add it
                        If (repersistnecessary OrElse addflag) AndAlso Not ot.GetBootStrapObjectClassIDs.Contains(description.ID) Then
                            objectids.Add(description.ID)
                        End If

                    Next

                    '*** create it
                    If objectids.Count > 0 Then
                        result = result And CreateAndPersist(objectids, force:=force)
                    Else
                        result = result And True
                    End If
                End If
            Next

            '*** create all foreign keys
            '***

            For Each aTable In DirectCast(CurrentSession.Objects, ormObjectRepository).ContainerDefinitions
                If aTable.AlterSchemaForeignRelations() Then
                    Call ot.CoreMessageHandler(procedure:="Installation.createDatabase", _
                                                      message:="foreign keys created for table " & aTable.ID, _
                                                      containerID:=aTable.ID, _
                                                      messagetype:=otCoreMessageType.ApplicationInfo)
                Else
                    Call ot.CoreMessageHandler(procedure:="Installation.createDatabase", _
                                                     message:="Error while creating foreign keys for table " & aTable.ID, _
                                                     containerID:=aTable.ID, _
                                                     messagetype:=otCoreMessageType.InternalError)
                End If
            Next

            '*** set the current schema version
            CurrentOTDBDriver.SetDBParameter(parametername:=ConstPNBSchemaInstallationDate, value:=Now.ToString, setupID:=setupid, silent:=True)
            CurrentOTDBDriver.SetDBParameter(parametername:=ConstPNBackendVersion, value:=ot.AssemblyVersion.ToString, setupID:=setupid, silent:=True)
            CurrentOTDBDriver.SetDBParameter(parametername:=ConstPNBSchemaVersion, value:=ot.SchemaVersion, setupID:=setupid, silent:=True)
            Dim aSchemaChange As OnTrackChangeLogEntry = New OnTrackChangeLogEntry(application:=ConstAssemblyName, [module]:=ConstPNBSchemaVersion, _
                                                                                   version:=ot.SchemaVersion, release:=0, patch:=0, changeimplno:=0, description:="installed schema")

            ot.OnTrackChangeLog.Add(aSchemaChange)

            '*** request end of bootstrap
            '***
            If Not CurrentSession.RequestEndofBootstrap() Then
                Call ot.CoreMessageHandler(showmsgbox:=True, procedure:="Installation.createDatabase", _
                                                       message:="failed to create tables for object repository - abort the installation", _
                                                       messagetype:=otCoreMessageType.InternalError)
                Return
            End If

            '*** start a session
            Dim sessionrunning As Boolean = CurrentSession.IsRunning
            Dim sessionstarted As Boolean = False
            Dim sessionaborted As Boolean = False

            '** if not global domain shutdown
            If (sessionrunning AndAlso ot.CurrentSession.CurrentDomainID <> ConstGlobalDomain) Then
                Call ot.CoreMessageHandler(showmsgbox:=True, procedure:="Installation.createDatabase", _
                                                       message:="shutting down current session since it is not in the global domain", _
                                                       messagetype:=otCoreMessageType.InternalInfo)
                CurrentSession.ShutDown(force:=True)
                sessionrunning = False
            End If
            '** no session runnnig -> startup
            If Not sessionrunning Then
                ''' if we have to abort the starting up
                If CurrentSession.IsStartingUp Then sessionaborted = CurrentSession.RequestToAbortStartingUp()
                sessionstarted = CurrentSession.StartUp(otAccessRight.AlterSchema, domainID:=ConstGlobalDomain, messagetext:="Please start up a Session to setup initial data")

            End If

            '***
            '*** Initialize Data
            If sessionrunning OrElse sessionstarted Then

                ''' Change Log Data
                ''' 
                If Not SaveChangeLog() Then
                    Call ot.CoreMessageHandler(showmsgbox:=True, procedure:="Installation.createDatabase", _
                                                          message:="failed to write change log data", _
                                                          messagetype:=otCoreMessageType.InternalError)
                    Return
                Else
                    ot.CoreMessageHandler(showmsgbox:=False, procedure:="Installation.createDatabase", _
                                                          message:="change log data persisted", _
                                                          messagetype:=otCoreMessageType.InternalInfo)
                End If

                ''' Core Data
                ''' 
                If Not InitialCoreData() Then
                    Call ot.CoreMessageHandler(showmsgbox:=True, procedure:="Installation.createDatabase", _
                                                          message:="failed to import initial data", _
                                                          messagetype:=otCoreMessageType.InternalError)
                    Return
                Else
                    ot.CoreMessageHandler(showmsgbox:=False, procedure:="Installation.createDatabase", _
                                                          message:="initial data for " & setupid & " imported ", _
                                                          messagetype:=otCoreMessageType.InternalInfo)
                End If

                ''' Initialize Data
                ''' 
                If Not InitializeData(setupid) Then
                    Call ot.CoreMessageHandler(showmsgbox:=True, procedure:="Installation.createDatabase", _
                                                          message:="failed to write initial core data - core might not be working correctly", _
                                                          messagetype:=otCoreMessageType.InternalError)
                    Return
                Else
                    ot.CoreMessageHandler(showmsgbox:=False, procedure:="Installation.createDatabase", _
                                                          message:="core objects with data instanced and persisted", _
                                                          messagetype:=otCoreMessageType.InternalInfo)
                End If
            End If

            ''' 
            '''shutdown a session
            If CurrentSession.IsRunning AndAlso sessionstarted Then
                CurrentSession.ShutDown(force:=True)
            End If
            If sessionaborted Then
                Call ot.CoreMessageHandler(showmsgbox:=True, procedure:="Installation.createDatabase", _
                                                             message:="The session which triggered the install routines was aborted during setup. Please reconnect again !", _
                                                             messagetype:=otCoreMessageType.InternalInfo)
            End If
        End Sub

        ''' <summary>
        ''' Drop Database
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DropDatabase() As Boolean
            '** check rights
            If ot.CurrentConnection.IsConnected OrElse CurrentOTDBDriver.HasAdminUserValidation() Then
                If Not ot.CurrentConnection.VerifyUserAccess(accessRequest:=otAccessRight.AlterSchema, useLoginWindow:=True, messagetext:="Please enter an administrator account to drop database") Then
                    CoreMessageHandler(message:="User access for dropping the the database schema could NOT be granted - operation aborted", messagetype:=otCoreMessageType.InternalInfo, _
                                        procedure:="adonetDBDriver.InstallOnTrackDatabase", showmsgbox:=True)
                    Return False
                End If
            End If
            With New UI.CoreMessageBox
                '* Message Heaxder
                .Title = "CAUTION - PLEASE CONFIRM CRITICAL OPERATION"
                .type = UI.CoreMessageBox.MessageType.Warning

                '* Message
                .Message = "Please confirm that you really want to drop the database >" & CurrentSetupID & "< using configuration >" & CurrentConfigSetName & "< and therefore ALL DATA WILL BE LOST !" & vbLf & _
                    " Make sure you have a database backup at hand."
                .buttons = UI.CoreMessageBox.ButtonType.YesNo
                .Show()
                If .result <> UI.CoreMessageBox.ResultType.Yes AndAlso .result <> UI.CoreMessageBox.ResultType.Ok Then
                    Return False
                End If
            End With

            ''' shut down if connected
            ''' 
            If (CurrentSession.IsRunning) Then
                Call ot.CoreMessageHandler(showmsgbox:=True, procedure:="Installation.DropDatabase", _
                                                       message:="shutting down current session - dropping database", _
                                                       messagetype:=otCoreMessageType.InternalInfo)
                CurrentSession.ShutDown(force:=True)
            End If
            '''
            ''' run through all tables
            ''' 
            Dim droppedContainer As New List(Of String)
            Dim theDependenciesList As New Dictionary(Of String, List(Of String))
            Dim allContainers As List(Of iormContainerAttribute) = ot.GetContainerAttributes
            Dim deleted As Long = 0

            ''' check all tables -> build net
            ''' 
            For Each aContainerAttribute In allContainers.ToList
                Dim delete As Boolean = False
                Dim driverresult As Boolean = False

                '' for each driver
                For Each aDriver As iormDatabaseDriver In CurrentSession.GetDatabaseDrivers(aContainerAttribute.ContainerID)

                    ''' check if foreign key references 
                    If Not aDriver.HasContainerID(aContainerAttribute.ContainerID) Then
                        ''' delete also the DB Parameter -> do not go over the table object
                        ''' since we are not connected
                        aDriver.DropContainerVersion(aContainerAttribute.ContainerID)
                    Else
                        driverresult = True 'one driver has the container
                    End If
                Next

                ''' check dependencies
                If Not driverresult Then
                    '** if container does not exist in any container then remove from allContainer
                    allContainers.Remove(aContainerAttribute)

                    ''' else if exists and has foreign keys
                ElseIf aContainerAttribute.ForeignkeyAttributes.Count <> 0 Then

                    For Each aforeignkey In aContainerAttribute.ForeignkeyAttributes
                        ''' foreign key is used in database as native
                        '''
                        If aforeignkey.HasValueForeignKeyReferences AndAlso aforeignkey.HasValueUseForeignKey _
                            AndAlso aforeignkey.UseForeignKey = otForeignKeyImplementation.NativeDatabase Then

                            ''' build dependendency
                            '''
                            For Each aReference In aforeignkey.ForeignKeyReferences
                                Dim names As String() = Shuffle.NameSplitter(aReference)
                                If names.Count > 1 Then

                                    ''' check referenced object entry on its table
                                    '''
                                    Dim anRefObjectEntry As ormObjectEntryAttribute = ot.GetObjectEntryAttribute(objectname:=names(0), entryname:=names(1))
                                    '' for each driver
                                    For Each aDriver As iormDatabaseDriver In CurrentSession.GetDatabaseDrivers(anRefObjectEntry.ContainerID)
                                        If anRefObjectEntry IsNot Nothing AndAlso anRefObjectEntry.HasValueContainerID _
                                            AndAlso aDriver.HasContainerID(anRefObjectEntry.ContainerID) Then

                                            Dim aDependendFromList As List(Of String)
                                            ''' add the referenced table to this table as depended
                                            '''
                                            If theDependenciesList.ContainsKey(anRefObjectEntry.ContainerID.ToUpper) Then
                                                aDependendFromList = theDependenciesList.Item(anRefObjectEntry.ContainerID.ToUpper)
                                            Else
                                                aDependendFromList = New List(Of String)
                                                theDependenciesList.Add(anRefObjectEntry.ContainerID.ToUpper, aDependendFromList)
                                            End If
                                            '* add this container id to be dependend from
                                            If Not aDependendFromList.Contains(aContainerAttribute.ContainerID.ToUpper) Then
                                                aDependendFromList.Add(aContainerAttribute.ContainerID.ToUpper)
                                            End If
                                        End If
                                    Next

                                End If

                            Next
                        End If
                    Next
                End If
            Next


            ''' do endless iterations
            ''' 
            Dim allContainerCount As Integer = allContainers.Count
            Do
                deleted = 0

                ''' check all containers
                For Each aContainerAttribute In allContainers.ToList
                    Dim driverresult As Boolean = False

                    ''' check if foreign key references 
                    If Not theDependenciesList.ContainsKey(aContainerAttribute.ContainerID.ToUpper) Then
                        '** Drop Container
                        '' for each driver
                        For Each aDriver As iormDatabaseDriver In CurrentSession.GetDatabaseDrivers(aContainerAttribute.ContainerID)
                            '' drop from each driver database
                            If aDriver.DropContainerObject(aContainerAttribute.ContainerID) Then
                                CoreMessageHandler(message:="Container  " & aContainerAttribute.ContainerID.ToUpper & " dropped in database driver '" & aDriver.Name & "' with ID '" & aDriver.ID & "'", containerID:=aContainerAttribute.ContainerID.ToUpper, _
                                               messagetype:=otCoreMessageType.ApplicationInfo, procedure:="Installation.DropDatabase")
                                driverresult = True
                            End If
                        Next

                        ''' increase no. deletes in this round
                        deleted += 1
                        '** if table doesnot exist then remove from alltables
                        allContainers.Remove(aContainerAttribute)
                        '* add it to dropped tables
                        droppedContainer.Add(aContainerAttribute.ContainerID.ToUpper)

                        '' delete references
                        Dim aRemoveDepend As New List(Of String)
                        For Each aDependList In theDependenciesList
                            If aDependList.Value.Contains(aContainerAttribute.ContainerID.ToUpper) Then
                                aDependList.Value.Remove(aContainerAttribute.ContainerID.ToUpper)
                            End If
                            '** add to dependency remove list
                            If aDependList.Value.Count = 0 Then
                                aRemoveDepend.Add(aDependList.Key)
                            End If
                        Next
                        '* remove from the list the tables which have no dependencies anymore
                        For Each aName In aRemoveDepend
                            theDependenciesList.Remove(aName)
                        Next
                    End If


                Next


                ''' end condition
            Loop While deleted <> 0 AndAlso allContainers.Count > 0


            ''' drop the setup
            '''
            CurrentOTDBDriver.DropDBParameterContainer(setupid:=CurrentSetupID)

            With New UI.CoreMessageBox
                '* Message Header
                .Title = "DATABASE >" & CurrentSetupID & "< using configuration >" & CurrentConfigSetName & "< DROP RESULT"
                .type = UI.CoreMessageBox.MessageType.Info

                '* Message
                If droppedContainer.Count = allContainerCount Then
                    .Message = droppedContainer.Count & " tables of the database have been dropped." & vbLf & _
                        "Database was deleted."
                Else
                    .Message = droppedContainer.Count & " tables from a total of " & allContainerCount & " in the database have been dropped." & vbLf & _
                       "Database was not completely deleted."
                End If

                .buttons = UI.CoreMessageBox.ButtonType.OK
                .Show()

            End With

            '* return
            If droppedContainer.Count = allContainers.Count Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Initialize Data by Importing them from the file system
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InitializeData(setupid As String, Optional searchpath As String = Nothing) As Boolean

            If Not ot.CurrentSession.RequireAccessRight(otAccessRight.AlterSchema) Then
                Call ot.CoreMessageHandler(message:="Access right could not be set to AlterSchema", procedure:="Installation.InitializeData", _
                                             messagetype:=otCoreMessageType.ApplicationInfo, break:=False)
                Return False
            End If
            ''' Initialize calendar
            ''' 
            Dim fromDate As Date = CDate(My.MySettings.Default.InitializeCalendarFrom)
            Dim ToDate As Date = CDate(My.MySettings.Default.InitializeCalendarTo)
            '***
            Dim valueFrom As Object = CurrentOTDBDriver.GetDBParameter(ConstPNCalendarInitializedFrom, setupID:=setupid, silent:=True)
            Dim valueTo As Object = CurrentOTDBDriver.GetDBParameter(ConstPNCalendarInitializedto, setupID:=setupid, silent:=True)

            ''' check if calendar already initialized
            ''' 
            If valueFrom Is Nothing OrElse valueTo Is Nothing _
                OrElse (IsDate(valueFrom) AndAlso CDate(valueFrom) <> fromDate) OrElse (IsDate(valueTo) AndAlso CDate(valueTo) <> ToDate) Then
                ''' initialize if date is not there
                ''' 
                If Not InitializeCalendar(calendarname:=CurrentSession.DefaultCalendarName, fromDate:=fromDate, toDate:=ToDate) Then
                    Call ot.CoreMessageHandler(showmsgbox:=True, procedure:="Installation.createDatabase", _
                                                              message:="failed to write initial calendar data - calendar might not be working correctly", _
                                                              messagetype:=otCoreMessageType.InternalError)
                Else
                    ot.CoreMessageHandler(showmsgbox:=False, procedure:="Installation.createDatabase", _
                                                         message:="calendar from " & fromDate & " until " & ToDate & " instanced and persisted", _
                                                         messagetype:=otCoreMessageType.InternalInfo)
                    CurrentOTDBDriver.SetDBParameter(ConstPNCalendarInitializedFrom, setupID:=setupid, value:=Format(fromDate, "yyyy-MM-dd"))
                    CurrentOTDBDriver.SetDBParameter(ConstPNCalendarInitializedto, setupID:=setupid, value:=Format(ToDate, "yyyy-MM-dd"))
                End If
            End If


            ''' import the initial data from DEFAULT directory
            ''' 
            Dim DefaultFolder As String = ConstInitialDataDefaultFolder
            Dim uri As System.Uri = New System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)
            ''' select the direct path 
            If String.IsNullOrWhiteSpace(searchpath) Then
                searchpath = My.Application.Info.DirectoryPath & "\Resources\" & DefaultFolder
            End If


            ''' try to feed from My.Application path, then from Executing Assembly Path 
            ''' 
            If Not String.IsNullOrWhiteSpace(DefaultFolder) AndAlso System.IO.Directory.Exists(searchpath) Then
                ot.CoreMessageHandler(message:="importing initial default data ...", argument:=searchpath, procedure:="Installation.createDatabase", messagetype:=otCoreMessageType.InternalInfo)
                FeedInInitialData(searchpath)
            ElseIf System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(uri.LocalPath) & "\Resources\" & DefaultFolder) Then
                searchpath = System.IO.Path.GetDirectoryName(uri.LocalPath) & "\Resources\" & DefaultFolder
                ot.CoreMessageHandler(message:="importing initial default data ...", argument:=searchpath, procedure:="Installation.createDatabase", messagetype:=otCoreMessageType.InternalInfo)
                FeedInInitialData(searchpath)
            Else
                ot.CoreMessageHandler(message:="initial default data not in default folder or default forlder does not exist", argument:=searchpath, procedure:="Installation.InitializeData", messagetype:=otCoreMessageType.InternalInfo)
            End If

            ''' Import the SETUP Specific Implementation under the name of the SETUPID
            ''' 
            searchpath = My.Application.Info.DirectoryPath & "\Resources\" & ConstInitialDataFolder & "\" & setupid

            If Not String.IsNullOrWhiteSpace(DefaultFolder) AndAlso System.IO.Directory.Exists(searchpath) Then
                ot.CoreMessageHandler(message:="importing initial setup data ...", argument:=searchpath, procedure:="Installation.createDatabase", messagetype:=otCoreMessageType.InternalInfo)
                FeedInInitialData(searchpath)
            ElseIf System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(uri.LocalPath) & "\Resources\" & setupid) Then
                searchpath = System.IO.Path.GetDirectoryName(uri.LocalPath) & "\Resources\" & setupid
                ot.CoreMessageHandler(message:="importing initial setup data ...", argument:=searchpath, procedure:="Installation.createDatabase", messagetype:=otCoreMessageType.InternalInfo)
                FeedInInitialData(searchpath)
            Else
                ot.CoreMessageHandler(message:="initial setup data not in setup folder or setup folder does not exist", argument:=searchpath, procedure:="Installation.InitializeData", messagetype:=otCoreMessageType.InternalInfo)
            End If


            Return True
        End Function
        ''' <summary>
        ''' Initialize the Calendar
        ''' </summary>
        ''' <remarks></remarks>
        Public Function InitializeCalendar(calendarname As String, fromDate As Date, toDate As Date) As Boolean

            ot.CoreMessageHandler(showmsgbox:=False, procedure:="Installation.createDatabase", _
                                                     message:="creating calendar from " & fromDate & " until " & toDate & " - please stand by ...", _
                                                     messagetype:=otCoreMessageType.ApplicationInfo)
            ''' generate the days
            CalendarEntry.GenerateDays(fromdate:=fromDate, untildate:=toDate, name:=calendarname)

            Dim acalentry As CalendarEntry
            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    ' additional
                    .Datevalue = CDate("29.03.2013")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "Karfreitag (Eastern)"
                    .Persist()
                End With
            End If

            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("01.04.2013")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "EasterMonday (Eastern)"
                    .Persist()
                End With
            End If
            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("09.05.2013")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "Christi Himmelfahrt"
                    .Persist()
                End With
            End If

            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("10.05.2013")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "Christi Himmelfahrt Brückentag"
                    .Persist()
                End With
            End If
            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("20.05.2013")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "Pfingsten"
                    .Persist()
                End With
            End If
            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("31.10.2013")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "Reformationstag (Sachsen)"
                End With
            End If
            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("20.11.2013")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "Buß- und Bettag (Sachsen)"
                    .Persist()
                End With
            End If
            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("18.04.2014")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "Karfreitag (Eastern)"
                    .Persist()
                End With
            End If
            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("01.04.2014")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "EasterMonday (Eastern)"
                    .Persist()
                End With
            End If
            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("29.05.2013")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "Christi Himmelfahrt"
                    .Persist()
                End With
            End If
            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("20.05.2014")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "Pfingsten"
                    .Persist()
                End With
            End If
            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("31.10.2014")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "Reformationstag (Sachsen)"
                    .Persist()
                End With
            End If
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("19.11.2014")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "Buß- und Bettag (Sachsen)"
                    .Persist()
                End With
            End If
            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("03.04.2015")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "Karfreitag (Eastern)"
                    .Persist()
                End With
            End If
            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("06.04.2015")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "EasterMonday (Eastern)"
                    .Persist()
                End With
            End If
            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("14.05.2015")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "Christi Himmelfahrt"
                    .Persist()
                End With
            End If
            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("25.05.2015")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "Pfingsten"
                    .Persist()
                End With
            End If
            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("31.10.2015")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "Reformationstag (Sachsen)"
                    .Persist()
                End With
            End If
            acalentry = CalendarEntry.Create()
            If acalentry IsNot Nothing Then
                With acalentry
                    .Datevalue = CDate("18.11.2015")
                    .Type = otCalendarEntryType.DayEntry
                    .IsNotAvailable = True
                    .Description = "Buß- und Bettag (Sachsen)"
                    .Persist()

                End With
            End If

            Call ot.CoreMessageHandler(showmsgbox:=False, procedure:="Installation.createDatabase_CoreData", containerID:=CalendarEntry.ConstPrimaryTableID, _
                                         message:="Calendar until 31.12.2016 created", messagetype:=otCoreMessageType.ApplicationInfo)

            Return True
        End Function
        ''' <summary>
        ''' save the ontrack change log to the database
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveChangeLog() As Boolean

            ''' save all change log entries to the database
            ''' 
            For Each anEntry In ot.OnTrackChangeLog
                If Not anEntry.IsAlive(throwError:=False) Then
                    anEntry.Create() 'bring to alive
                End If
                If anEntry.RunTimeOnly Then anEntry.SwitchRuntimeOff() ' switch runtime off to make persistable
                anEntry.Persist()
            Next

            Return True
        End Function
        ''' <summary>
        ''' feeds the initial data from a path
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function FeedInInitialData(path As String) As Boolean

            If Not System.IO.Directory.Exists(path) Then
                CoreMessageHandler(message:="path does not exist in file system", argument:=path, procedure:="createDatabase.FeedInitialData")
            Else

                CoreMessageHandler(message:="checking directory '" & path & "'", _
                                                  argument:=path, username:=CurrentSession.CurrentUsername, _
                                                  procedure:="CreateDatabase.FeedInitialData", messagetype:=otCoreMessageType.InternalInfo)
            End If

            ''' try to feed in each File in the filepath
            For Each anEntry In System.IO.Directory.EnumerateFileSystemEntries(path)
                If System.IO.Directory.Exists(anEntry) Then
                    FeedInInitialData(anEntry)
                Else
                    ''' feed in the csv file if it is one
                    ''' 
                    If System.IO.Path.GetExtension(anEntry).ToUpper = ".CSV" Then
                        If CSVXChangeManager.FeedInCSV(anEntry) Then
                            CoreMessageHandler(message:="csv file '" & System.IO.Path.GetFileName(anEntry) & "' imported", _
                                               argument:=path, username:=CurrentSession.CurrentUsername, _
                                               procedure:="CreateDatabase.FeedInitialData", messagetype:=otCoreMessageType.InternalInfo)
                        End If
                    End If
                End If

            Next

            Return True
        End Function
        ''' <summary>
        '''  Initial Core Data
        ''' </summary>
        ''' <remarks></remarks>
        Private Function InitialCoreData() As Boolean

            '**** default domain settings
            Dim aDomain = Domain.Retrieve(id:=ConstGlobalDomain)
            If aDomain IsNot Nothing Then
                '*** set the Domain Settings
                '***
                aDomain.SetSetting(id:=Session.ConstCPDependencySynchroMinOverlap, datatype:=otDataType.Long, value:=7)
                aDomain.SetSetting(id:=Session.ConstCPDefaultWorkspace, datatype:=otDataType.Text, value:="@")
                aDomain.SetSetting(id:=Session.ConstCPDefaultCalendarName, datatype:=otDataType.Text, value:="default")
                aDomain.SetSetting(id:=Session.ConstCPDefaultTodayLatency, datatype:=otDataType.Long, value:=-14)
                aDomain.SetSetting(id:=Session.ConstCDefaultScheduleTypeID, datatype:=otDataType.Text, value:=String.Empty)
                aDomain.SetSetting(id:=Session.ConstCPDefaultDeliverableTypeID, datatype:=otDataType.Text, value:=String.Empty)
                aDomain.Persist()
            End If

            '*** Project Base workspaceID
            Dim aWorkspace = Workspace.Create("@")
            If aWorkspace IsNot Nothing Then
                aWorkspace.Description = "base workspaceID"
                aWorkspace.IsBasespace = True
                aWorkspace.FCRelyingOn = New String() {"@"}
                aWorkspace.ACTRelyingOn = New String() {"@"}
                aWorkspace.AccesslistIDs = New String() {}
                aWorkspace.HasActuals = True
                aWorkspace.MinScheduleUPDC = 1
                aWorkspace.MaxScheduleUPDC = 999
                aWorkspace.MinTargetUPDC = 1
                aWorkspace.MaxTargetUPDC = 999
                aWorkspace.Persist()

                Call ot.CoreMessageHandler(showmsgbox:=False, procedure:="Installation.InitialCoreData", _
                                             message:="base workspaceID @ created", messagetype:=otCoreMessageType.ApplicationInfo, containerID:=aWorkspace.ObjectPrimaryTableID)
            End If

            '*** Create Group
            Dim aGroup As Group = Group.Create(groupname:="admin")
            If aGroup IsNot Nothing Then
                aGroup.Description = "Administratio group"
                aGroup.HasAlterSchemaRights = True
                aGroup.HasReadRights = True
                aGroup.HasUpdateRights = True
                aGroup.HasNoRights = False
                If aGroup.Persist() Then
                    Call ot.CoreMessageHandler(showmsgbox:=False, procedure:="Installation.InitialCoreData", objectname:=Group.ConstObjectID, _
                                                message:="Group Admin created", messagetype:=otCoreMessageType.ApplicationInfo)
                End If

            End If
            '*** Create Group
            aGroup = Group.Create(groupname:="readers")
            If aGroup IsNot Nothing Then
                aGroup.Description = "anonymous group"
                aGroup.HasAlterSchemaRights = False
                aGroup.HasReadRights = True
                aGroup.HasUpdateRights = False
                aGroup.HasNoRights = False
                If aGroup.Persist() Then
                    Call ot.CoreMessageHandler(showmsgbox:=False, procedure:="Installation.InitialCoreData", objectname:=Group.ConstObjectID, _
                                                message:="Group Readers created", messagetype:=otCoreMessageType.ApplicationInfo)
                End If

            End If
            '*** Create Default Users
            '***
            Dim anUser As User = User.Create(username:="admin")
            If anUser Is Nothing Then anUser = User.Retrieve(username:="admin")
            If anUser IsNot Nothing Then
                anUser.Description = "Administrator"
                anUser.DefaultWorkspaceID = "@"
                anUser.DefaultDomainID = ConstGlobalDomain
                anUser.GroupNames = {"admin"}
                anUser.Password = "axs2ontrack"
                anUser.HasAlterSchemaRights = True
                anUser.HasNoRights = False
                anUser.HasReadRights = True
                anUser.HasUpdateRights = True
                anUser.IsAnonymous = False
                anUser.Persist()
                Call ot.CoreMessageHandler(showmsgbox:=False, procedure:="Installation.createDatabase_CoreData", containerID:=anUser.ObjectPrimaryTableID, _
                                             message:="User Admin created", messagetype:=otCoreMessageType.ApplicationInfo)
            End If
            anUser = User.Create(username:="boschnei")
            If anUser IsNot Nothing Then
                anUser.Description = "Boris Schneider"
                anUser.GroupNames = {"admin"}
                anUser.DefaultWorkspaceID = "@"
                anUser.DefaultDomainID = ConstGlobalDomain
                anUser.Password = "zulu4Hart"
                anUser.HasAlterSchemaRights = True
                anUser.HasNoRights = False
                anUser.HasReadRights = True
                anUser.HasUpdateRights = True
                anUser.IsAnonymous = False
                anUser.PersonName = "Boris Schneider"
                anUser.Persist()
            End If
            anUser = User.Create(username:="anonymous")
            If anUser IsNot Nothing Then
                anUser.Description = "anonymous"
                anUser.GroupNames = {"readers"}
                anUser.DefaultWorkspaceID = "@"
                anUser.DefaultDomainID = ConstGlobalDomain
                anUser.Password = Nothing
                anUser.HasAlterSchemaRights = False
                anUser.HasNoRights = False
                anUser.HasReadRights = True
                anUser.HasUpdateRights = False
                anUser.IsAnonymous = True
                anUser.PersonName = Nothing
                anUser.Persist()
                Call ot.CoreMessageHandler(showmsgbox:=False, procedure:="Installation.createDatabase_CoreData", containerID:=anUser.ObjectPrimaryTableID, _
                                             message:="User anonymous for read created", messagetype:=otCoreMessageType.ApplicationInfo)
            End If


            Return True
        End Function
    End Module
End Namespace