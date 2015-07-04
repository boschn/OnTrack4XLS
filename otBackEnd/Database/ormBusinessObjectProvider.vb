
REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** ORM DATA OBJECT FACTORY CLASSES
REM ***********
REM *********** Version: 2.0
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
Imports OnTrack.rulez
Imports OnTrack.Core

Namespace OnTrack.Database

    ''' <summary>
    ''' a singleton Business Object Factory class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ormBusinessObjectProvider
        Inherits ormDataObjectProvider
        Implements iormDataObjectProvider


        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="Session"></param>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Initialize and register all business objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function Initialize(Optional repository As ormObjectRepository = Nothing) As Boolean
            If Not MyBase.Initialize(repository:=repository) Then Return False

            ''' autoregister
            ''' 
            Dim thisAsm As Assembly = Assembly.GetExecutingAssembly()
            Dim aClassList As List(Of Type) = thisAsm.GetTypes().Where(Function(t) _
                                                    ((GetType(ormBusinessObject).IsAssignableFrom(t) AndAlso t.IsClass AndAlso Not t.IsAbstract))).ToList()
            Try
                For Each aClass In aClassList
                    ''' check the class type attributes
                    Dim aDescription As ObjectClassDescription = ot.GetObjectClassDescription(aClass)
                    If aDescription IsNot Nothing Then
                        If aDescription.ObjectAttribute.HasValueID Then Me.RegisterObjectID(aDescription.ObjectAttribute.ID)
                    End If
                Next

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ormBusinessObjectFactory.Initialize")
                Return False
            End Try

            _IsInitialized = True
            Return _IsInitialized
        End Function
        ''' <summary>
        ''' retrieves a business object by primary key
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="type"></param>
        ''' <param name="domainID"></param>
        ''' <param name="forceReload"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Retrieve(key As ormDatabaseKey, _
                                type As Type, _
                                Optional domainID As String = Nothing, _
                                Optional forceReload As Boolean? = Nothing, _
                                Optional runtimeOnly As Boolean? = Nothing) As iormDataObject Implements iormDataObjectProvider.Retrieve

            Dim anObjectID As String = ot.GetObjectClassDescription(type).ObjectAttribute.ID
            '** check on the operation right for this object for the current username (might be that during session startup otdb username is not set)
            If Not runtimeOnly AndAlso Not CurrentSession.IsStartingUp AndAlso Not ot.GetBootStrapObjectClassIDs.Contains(anObjectID) _
                AndAlso Not CurrentSession.ValidateAccessRights(accessrequest:=otAccessRight.ReadOnly, domainid:=domainID, _
                                                                objecttransactions:={anObjectID & "." & ormBusinessObject.ConstOPInject}) Then
                '** request authorizartion
                If Not CurrentSession.RequestUserAccess(accessRequest:=otAccessRight.ReadOnly, domainid:=domainID, _
                                                                            username:=CurrentSession.CurrentUsername, _
                                                                            objecttransactions:={anObjectID & "." & ormBusinessObject.ConstOPInject}) Then
                    Call CoreMessageHandler(message:="data object cannot be retrieved - permission denied to user", _
                                            objectname:=anObjectID, argument:=ormBusinessObject.ConstOPInject, username:=CurrentSession.CurrentUsername, _
                                            procedure:="ormDataObjectFactory.Retrieve", messagetype:=otCoreMessageType.ApplicationError)
                    Return Nothing
                End If
            End If

            ''' retrieve
            ''' 
            Return MyBase.Retrieve(key:=key, type:=type, domainID:=domainID, forceReload:=forceReload, runtimeOnly:=runtimeOnly)
        End Function
        ''' <summary>
        ''' retrieve all business objects regardless of selection criteria
        ''' </summary>
        ''' <param name="type"></param>
        ''' <param name="domainID"></param>
        ''' <param name="forceReload"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RetrieveAll(type As Type, _
                                    Optional key As ormDatabaseKey = Nothing, _
                                    Optional domainID As String = Nothing, _
                                    Optional deleted As Boolean = False, _
                                    Optional forceReload As Boolean? = Nothing, _
                                    Optional runtimeOnly As Boolean? = Nothing) As IEnumerable(Of iormDataObject) Implements iormDataObjectProvider.RetrieveAll
            Dim wherestring As String
            ''' build a wherestring
            If key IsNot Nothing Then
                wherestring = String.Empty
                Dim aDbDriver As iormRelationalDatabaseDriver = TryCast(key.DatabaseDriver, iormRelationalDatabaseDriver)
                If aDbDriver Is Nothing Then
                    CoreMessageHandler(message:="database driver of container is not a relational database driver", messagetype:=otCoreMessageType.InternalError, _
                                        containerID:=key.ContainerID, procedure:="ormBusinessObjectFactory.RetrieveAll")

                Else
                    '''
                    For i As UShort = key.GetLowerBound To key.GetUpperBound
                        If Not String.IsNullOrEmpty(wherestring) Then wherestring &= " AND "
                        Dim aValue As Object
                        Dim aFlag As Boolean
                        aDbDriver.Convert2DBData(invalue:=key.Item(i), outvalue:=aValue, targetType:=key.Datatype(i), abostrophNecessary:=aFlag)
                        If aFlag Then
                            wherestring &= String.Format("[{0}] = '{1}'", key.Item(i), aValue)
                        Else
                            wherestring &= String.Format("[{0}] = {1}", key.Item(i), aValue)
                        End If
                    Next
                End If
            End If

            ''' return the query
            Return Me.RetrieveAllByQuery(type:=type, domainID:=domainID, deleted:=deleted, forceReload:=forceReload, where:=wherestring, runtimeOnly:=runtimeOnly)
        End Function

        ''' <summary>
        ''' retrieve all business objects regardless of selection criteria
        ''' </summary>
        ''' <param name="type"></param>
        ''' <param name="domainID"></param>
        ''' <param name="forceReload"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RetrieveAllByQuery(type As Type, _
                                    Optional domainID As String = Nothing, _
                                    Optional deleted As Boolean = False, _
                                    Optional forceReload As Boolean? = Nothing, _
                                    Optional runtimeOnly As Boolean? = Nothing, _
                                    Optional where As String = Nothing, _
                                    Optional orderby As String = Nothing, _
                                    Optional parameters As List(Of ormSqlCommandParameter) = Nothing) As IEnumerable(Of iormDataObject)
            If Not Me.IsInitialized AndAlso Not Me.Initialize Then
                CoreMessageHandler(message:="could not initiliaze factory", procedure:="ormBusinessObjectFactory.RetrieveAllByQuery", _
                                   argument:=type.FullName, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If
            If Not Me.HasType(type) Then
                CoreMessageHandler(message:="type is not handled by this factory", procedure:="ormBusinessObjectFactory.RetrieveAllByQuery", _
                                   argument:=type.FullName, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If
            Dim theObjectList As New List(Of iormRelationalPersistable)
            Dim aRecordCollection As New List(Of ormRecord)
            Dim aStore As iormRelationalTableStore
            Dim anObject As iormRelationalPersistable = TryCast(Me.NewOrmDataObject(type), iormRelationalPersistable)
            If anObject Is Nothing Then
                CoreMessageHandler(message:="type is not implementing iormRelationalPersistable", procedure:="ormBusinessObjectFactory.Retrieve", _
                                   argument:=type.FullName, messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If
            If String.IsNullOrEmpty(domainID) Then domainID = CurrentSession.CurrentDomainID

            '** is a session running ?!
            If Not CurrentSession.IsRunning AndAlso Not CurrentSession.IsStartingUp Then
                Call CoreMessageHandler(message:="data object cannot be retrieved - start session to database first", _
                                        objectname:=anObject.ObjectID, _
                                        procedure:="ormBusinessObjectFactory.All", messagetype:=otCoreMessageType.ApplicationError)
                Return Nothing
            End If

            '** check on the operation right for this object for the current username (might be that during session startup otdb username is not set)
            If Not CurrentSession.IsStartingUp AndAlso Not ot.GetBootStrapObjectClassIDs.Contains(anObject.ObjectID) _
            AndAlso Not CurrentSession.ValidateAccessRights(accessrequest:=otAccessRight.ReadOnly, domainid:=domainID, _
                                                            objecttransactions:={anObject.ObjectID & "." & ormBusinessObject.ConstOPInject}) Then
                '** request authorizartion
                If Not CurrentSession.RequestUserAccess(accessRequest:=otAccessRight.ReadOnly, domainid:=domainID, _
                                                        username:=CurrentSession.CurrentUsername, _
                                                        objecttransactions:={anObject.ObjectID & "." & ormBusinessObject.ConstOPInject}) Then
                    Call CoreMessageHandler(message:="data object cannot be retrieved - permission denied to user", _
                                            objectname:=anObject.ObjectID, argument:=ormBusinessObject.ConstOPInject, username:=CurrentSession.CurrentUsername, _
                                            procedure:="ormBusinessObjectFactory.Retrieve", messagetype:=otCoreMessageType.ApplicationError)
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
                    If String.IsNullOrEmpty(domainID) Then domainID = CurrentSession.CurrentDomainID
                    ''' add where
                    If Not String.IsNullOrWhiteSpace(where) Then where &= " AND "
                    where &= String.Format(" ([{0}] = @{0} OR [{0}] = @Global{0})", Domain.ConstFNDomainID)
                    ''' add parameters
                    If parameters.Find(Function(x)
                                           Return x.ID.ToUpper = "@" & Domain.ConstFNDomainID.ToUpper
                                       End Function) Is Nothing Then
                        parameters.Add(New ormSqlCommandParameter(id:="@" & Domain.ConstFNDomainID, columnname:=Domain.ConstFNDomainID, _
                                                                  tableid:=anObject.ObjectPrimaryTableID, value:=domainID)
                        )
                    End If
                    If parameters.Find(Function(x)
                                           Return x.ID.ToUpper = "@Global" & Domain.ConstFNDomainID.ToUpper
                                       End Function
                    ) Is Nothing Then
                        parameters.Add(New ormSqlCommandParameter(id:="@Global" & Domain.ConstFNDomainID, columnname:=Domain.ConstFNDomainID, _
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
                aRecordCollection = aStore.GetRecordsBySqlCommand(id:=ConstQryAllObjects, wherestr:=where, orderby:=orderby, parameters:=parameters)
                If aRecordCollection Is Nothing Then
                    CoreMessageHandler(message:="no records returned due to previous errors", procedure:="ormBusinessObjectFactory.AllDataObject", argument:=ConstQryAllObjects, _
                                       objectname:=anObject.ObjectID, containerID:=anObject.ObjectPrimaryTableID, messagetype:=otCoreMessageType.InternalError)
                    Return theObjectList
                End If
                Dim aDomainRecordCollection As New Dictionary(Of String, ormRecord)
                Dim pknames = aStore.ContainerSchema.PrimaryEntryNames
                Dim domainBehavior As Boolean = False

                If anObject.ObjectHasDomainBehavior And domainID <> ConstGlobalDomain Then
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
                            If acolumnname <> Domain.ConstFNDomainID Then pk &= aRecord.GetValue(index:=acolumnname).ToString & ConstDelimiter
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
                    Else
                        ''' just build the list
                        Dim atargetobject As iormRelationalPersistable = TryCast(Me.NewOrmDataObject(type), iormRelationalPersistable)
                        If atargetobject Is Nothing Then
                            CoreMessageHandler(message:="type is not implementing iormRelationalPersistable", procedure:="ormBusinessObjectFactory.Retrieve", _
                                               argument:=type.FullName, messagetype:=otCoreMessageType.InternalError)
                            Return Nothing
                        End If
                        If atargetobject.Infuse(record:=aRecord, mode:=otInfuseMode.OnInject Or otInfuseMode.OnDefault) Then
                            theObjectList.Add(atargetobject)
                        End If
                    End If
                Next

                '** phase II: if on domainbehavior then get the objects out of the active domain entries
                '**
                If domainBehavior Then
                    For Each aRecord In aDomainRecordCollection.Values
                        Dim atargetobject As iormRelationalPersistable = TryCast(Me.NewOrmDataObject(type), iormRelationalPersistable)
                        If atargetobject Is Nothing Then
                            CoreMessageHandler(message:="type is not implementing iormRelationalPersistable", procedure:="ormBusinessObjectFactory.RetrieveAllQuery", _
                                               argument:=type.FullName, messagetype:=otCoreMessageType.InternalError)
                            Return Nothing
                        End If
                        If ormBusinessObject.InfuseDataObject(record:=aRecord, dataobject:=TryCast(atargetobject, iormInfusable), _
                                                          mode:=otInfuseMode.OnInject Or otInfuseMode.OnDefault) Then
                            theObjectList.Add(DirectCast(atargetobject, iormRelationalPersistable))
                        End If
                    Next
                End If

                ''' return the ObjectsList
                Return theObjectList

            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="ormBusinessObjectFactory.RetrieveAllByQuery")
                Return theObjectList
            End Try


        End Function
        
    End Class
End Namespace
