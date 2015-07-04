REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** relational helper classes
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2014-04-14
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
Imports System.Reflection
Imports OnTrack.Commons
Imports OnTrack.otrulez
Imports OnTrack.Core

Namespace OnTrack.Database

    ''' <summary>
    '''  Data Object Class is the Persistable data object here we find the relation parts
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Partial Public MustInherit Class ormBusinessObject


        ''' <summary>
        ''' cascade the update of relational data
        ''' </summary>
        ''' <param name="dataobject"></param>
        ''' <param name="classdescriptor"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Private Shared Function CascadeRelation(ByRef dataobject As iormPersistable, ByRef classdescriptor As ObjectClassDescription, _
        '                                              cascadeUpdate As Boolean, cascadeDelete As Boolean, _
        '                                              Optional relationid As String = String.empty, _
        '                                              Optional timestamp As DateTime = constNullDate, _
        '                                              Optional uniquenesswaschecked As Boolean = True) As Boolean

        '    If timestamp = constNullDate Then timestamp = DateTime.Now

        '    '* Fire Event OnRelationLoading
        '    Dim ourEventArgs As New ormDataObjectEventArgs(dataobject, Nothing, relationID:={relationid}.ToList, timestamp:=timestamp)
        '    ourEventArgs.Proceed = True
        '    ourEventArgs.Result = True
        '    RaiseEvent ClassOnCascadingRelation(dataobject, ourEventArgs)
        '    dataobject = ourEventArgs.DataObject
        '    If Not ourEventArgs.Proceed Then Return ourEventArgs.Result


        '    Try
        '        SyncLock dataobject
        '            '***
        '            '*** Fill in the relations
        '            For Each aRelationAttribute In classdescriptor.RelationAttributes

        '                '** run through specific relation condition 
        '                If (relationid = String.empty OrElse relationid.ToLower = aRelationAttribute.Name.ToLower) And _
        '                    ((cascadeUpdate AndAlso cascadeUpdate = aRelationAttribute.CascadeOnUpdate) OrElse _
        '                     (cascadeDelete AndAlso cascadeDelete = aRelationAttribute.CascadeOnDelete)) Then
        '                    '* get the list
        '                    Dim aFieldList As List(Of FieldInfo) = classdescriptor.GetMappedRelationFieldInfos(relationName:=aRelationAttribute.Name)

        '                    For Each aFieldInfo In aFieldList
        '                        Dim aMappingAttribute = classdescriptor.GetEntryMappingAttributes(aFieldInfo.Name)

        '                        '** if direct persistable
        '                        If aFieldInfo.FieldType.GetInterfaces().Contains(GetType(iormPersistable)) Then

        '                            Dim anobject As Object
        '                            '** get value 
        '                            If Not Reflector.GetFieldValue(aFieldInfo, dataobject, anobject) Then
        '                                anobject = aFieldInfo.GetValue(dataobject)
        '                            End If

        '                            Dim ansubdataobject = TryCast(anobject, iormPersistable)
        '                            If ansubdataobject IsNot Nothing Then
        '                                If cascadeUpdate = aRelationAttribute.CascadeOnUpdate Then
        '                                    '** persist
        '                                    ansubdataobject.Persist(timestamp)
        '                                ElseIf cascadeDelete = aRelationAttribute.CascadeOnDelete Then
        '                                    '** persist
        '                                    ansubdataobject.Delete(timestamp)
        '                                End If
        '                            Else
        '                                CoreMessageHandler(message:="mapped field in data object does not implement the iormpersistable", subname:="ormDataObject.CascadeRelation", _
        '                                                   messagetype:=otCoreMessageType.InternalError, objectname:=dataobject.ObjectID, arg1:=aFieldInfo.Name)
        '                                Return False
        '                            End If

        '                            '** get the related objects by query somehow
        '                        Else


        '                            '** and Dicitionary
        '                            If aFieldInfo.FieldType.GetInterfaces.Contains(GetType(IDictionary)) Then
        '                                Dim aDictionary As IDictionary
        '                                '** get values either by hook or by slow getvalue
        '                                If Not Reflector.GetFieldValue(field:=aFieldInfo, dataobject:=dataobject, value:=aDictionary) Then
        '                                    aDictionary = aFieldInfo.GetValue(dataobject)
        '                                End If
        '                                For Each anEntry In aDictionary.Values
        '                                    Dim anSubdataObject As iormPersistable = TryCast(anEntry, iormPersistable)
        '                                    If anSubdataObject IsNot Nothing Then
        '                                        ''' CASCADE UPDATE
        '                                        If cascadeUpdate = aRelationAttribute.CascadeOnUpdate Then
        '                                            '** persist
        '                                            anSubdataObject.Persist(timestamp)

        '                                            ''' CASCADE DELETE
        '                                        ElseIf cascadeDelete = aRelationAttribute.CascadeOnDelete Then
        '                                            '** delete
        '                                            anSubdataObject.Delete(timestamp)
        '                                        End If
        '                                    Else
        '                                        CoreMessageHandler(message:="mapped inner field in dictionary object of type enumerable does not implement the iormpersistable", subname:="ormDataObject.CascadeRelation", _
        '                                                   messagetype:=otCoreMessageType.InternalError, objectname:=dataobject.ObjectID, arg1:=aFieldInfo.Name)
        '                                        Return False
        '                                    End If
        '                                Next

        '                                '** run through the enumerables and try to cascade
        '                            ElseIf aFieldInfo.FieldType.GetInterfaces.Contains(GetType(IEnumerable)) Then
        '                                Dim aList As IEnumerable
        '                                '** get values either by hook or by slow getvalue
        '                                If Not Reflector.GetFieldValue(field:=aFieldInfo, dataobject:=dataobject, value:=aList) Then
        '                                    aList = aFieldInfo.GetValue(dataobject)
        '                                End If
        '                                If aList Is Nothing Then
        '                                    CoreMessageHandler(message:="mapped inner field in container object of type enumerable is not initialized in class", subname:="ormDataObject.CascadeRelation", _
        '                                                       messagetype:=otCoreMessageType.InternalError, objectname:=dataobject.ObjectID, arg1:=aFieldInfo.Name)
        '                                    Return False
        '                                Else
        '                                    For Each anEntry In aList
        '                                        Dim anSubdataObject As iormPersistable
        '                                        If anEntry.GetType.Equals(GetType(KeyValuePair(Of ,))) Then
        '                                            Throw New NotImplementedException
        '                                        Else
        '                                            anSubdataObject = TryCast(anEntry, iormPersistable)
        '                                        End If
        '                                        If anSubdataObject IsNot Nothing Then
        '                                            If cascadeUpdate = aRelationAttribute.CascadeOnUpdate Then
        '                                                '** persist
        '                                                anSubdataObject.Persist(timestamp)
        '                                            ElseIf cascadeDelete = aRelationAttribute.CascadeOnDelete Then
        '                                                '** persist
        '                                                anSubdataObject.Delete(timestamp)
        '                                            End If
        '                                        Else
        '                                            CoreMessageHandler(message:="mapped inner field in container object of type enumerable does not implement the iormpersistable", subname:="ormDataObject.CascadeRelation", _
        '                                                       messagetype:=otCoreMessageType.InternalError, objectname:=dataobject.ObjectID, arg1:=aFieldInfo.Name)
        '                                            Return False
        '                                        End If
        '                                    Next
        '                                End If



        '                            Else
        '                                CoreMessageHandler(message:="generic data handling container object neither of enumerable or dictionary", _
        '                                                    subname:="ormDataObject.CascadeRelation", messagetype:=otCoreMessageType.InternalError)
        '                            End If


        '                        End If

        '                    Next
        '                End If
        '            Next

        '        End SyncLock

        '        '* Fire Event OnRelationLoading
        '        ourEventArgs = New ormDataObjectEventArgs(dataobject, Nothing, , relationID:={relationid}.ToList)
        '        ourEventArgs.Proceed = True
        '        ourEventArgs.Result = True
        '        RaiseEvent ClassOnCascadedRelation(dataobject, ourEventArgs)
        '        dataobject = ourEventArgs.DataObject
        '        Return ourEventArgs.Result
        '    Catch ex As Exception
        '        Call CoreMessageHandler(subname:="ormDataObject.InfuseRelationMapped", exception:=ex, objectname:=dataobject.ObjectID, _
        '                                tablename:=dataobject.primaryTableID)
        '        Return False

        '    End Try

        'End Function


    End Class

    ''' <summary>
    ''' Class to administrate the lifecycle status of a relation in the data object
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ormRelationManager
        Implements IEnumerable(Of ormRelationAttribute)


        ''' <summary>
        ''' Event Handling Arguments
        ''' </summary>
        ''' <remarks></remarks>
        Public Class EventArgs
            Inherits System.EventArgs

            Private _objects As New List(Of iormRelationalPersistable)
            Private _finished As Boolean = False
            Private _relationid As String
            Private _relationattribute As ormRelationAttribute
            Private _fieldinfo As FieldInfo
            Private _dataobject As ormBusinessObject
            Private _infusemode As otInfuseMode
            Private _objectmessagelog As BusinessObjectMessageLog

            ''' <summary>
            ''' constructor
            ''' </summary>
            ''' <param name="objects"></param>
            ''' <param name="proceed"></param>
            ''' <remarks></remarks>

            Public Sub New(relationid As String, _
                           Optional ByRef objects As List(Of iormRelationalPersistable) = Nothing, _
                           Optional ByRef relationAttribute As ormRelationAttribute = Nothing, _
                           Optional ByRef fieldinfo As FieldInfo = Nothing, _
                           Optional ByRef dataobject As ormBusinessObject = Nothing, _
                           Optional infusemode As otInfuseMode = 0)
                _relationid = relationid
                If objects IsNot Nothing Then _objects.AddRange(objects)
                If relationAttribute IsNot Nothing Then _relationattribute = relationAttribute
                If fieldinfo IsNot Nothing Then _fieldinfo = fieldinfo

                If infusemode <> 0 Then _infusemode = infusemode
                If dataobject IsNot Nothing Then
                    _dataobject = dataobject

                End If

            End Sub

#Region "Properties"


            ''' <summary>
            ''' Gets or sets the mode.
            ''' </summary>
            ''' <value>The mode.</value>
            Public Property InfuseMode() As otInfuseMode
                Get
                    Return Me._infusemode
                End Get
                Set(value As otInfuseMode)
                    Me._infusemode = value
                End Set
            End Property

            ''' <summary>
            ''' Gets or sets the dataobject.
            ''' </summary>
            ''' <value>The dataobject.</value>
            Public Property Dataobject() As ormBusinessObject
                Get
                    Return Me._dataobject
                End Get
                Set(value As ormBusinessObject)
                    Me._dataobject = value
                End Set
            End Property

            ''' <summary>
            ''' Gets or sets the relationattribute.
            ''' </summary>
            ''' <value>The relationattribute.</value>
            Public ReadOnly Property RelationAttribute() As ormRelationAttribute
                Get
                    Return Me._relationattribute
                End Get
            End Property

            ''' <summary>
            ''' Gets or sets the fieldinfo.
            ''' </summary>
            ''' <value>The fieldinfo.</value>
            Public ReadOnly Property FieldInfo() As FieldInfo
                Get
                    Return Me._fieldinfo
                End Get

            End Property

            ''' <summary>
            ''' Gets or sets the finished - do not proceed.
            ''' </summary>
            ''' <value>The proceed.</value>
            Public Property Finished() As Boolean
                Get
                    Return Me._finished
                End Get
                Set(value As Boolean)
                    Me._finished = value
                End Set
            End Property

            ''' <summary>
            ''' Gets the objects.
            ''' </summary>
            ''' <value>The objects.</value>
            Public ReadOnly Property Objects() As List(Of iormRelationalPersistable)
                Get
                    Return Me._objects
                End Get
            End Property
#End Region


        End Class
        ''' <summary>
        ''' status enumeration of the relation
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum RelationStatus
            Unloaded = 0
            Loaded = 1
        End Enum

        Private WithEvents _dataobject As ormBusinessObject 'link to the data object
        Private _relationStatus As RelationStatus() 'status of the relation in order of ObjectClassDescription.RelationAttributes
        Private _isInitialized As Boolean = False
        Private _objectmessagelog As BusinessObjectMessageLog

        Public Event OnRelatedObjectsRetrieveRequest(sender As Object, e As ormRelationManager.EventArgs)
        Public Event OnRelatedObjectsCreateRequest(sender As Object, e As ormRelationManager.EventArgs)

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="objectid"></param>
        ''' <remarks></remarks>
        Public Sub New(dataobject As ormRelationalInfusable)
            _dataobject = dataobject

        End Sub


        ''' <summary>
        ''' Event Handler for the Runtime Switch off Event - check which relations we regards as loaded
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub DataObjectRelationMGr_OnRuntimeSwitchOff(sender As Object, e As ormDataObjectEventArgs) Handles _dataobject.OnSwitchRuntimeOff

            For Each aRelationName In Me.Relationnames
                Dim aFieldList As List(Of FieldInfo) = _dataobject.ObjectClassDescription.GetMappedRelation2FieldInfos(relationName:=aRelationName)

                For Each aFieldInfo In aFieldList
                    Dim aMappingAttribute = _dataobject.ObjectClassDescription.GetEntryMappingAttributes(aFieldInfo.Name)
                    Dim theObjects = Me.GetObjectsFromContainer(aRelationName)

                    If theObjects.Count > 0 Then
                        Me.Status(aRelationName) = RelationStatus.Loaded
                    End If
                Next
            Next

        End Sub
        ''' <summary>
        ''' initialize 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Initialize() As Boolean
            Try
                If _isInitialized Then Return _isInitialized

                Dim relationnames = _dataobject.ObjectClassDescription.RelationNames
                ReDim _relationStatus(relationnames.Count - 1)
                For i = 0 To _relationStatus.GetUpperBound(0)
                    _relationStatus(i) = RelationStatus.Unloaded
                Next

                '** objectmessagelog -> recursion since this is also called on infuse the log relation
                '** late bound instead
                'If _objectmessagelog Is Nothing Then
                '    _objectmessagelog = _dataobject.ObjectMessageLog
                'End If

                _isInitialized = True
                Return _isInitialized
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="DataObjectRelationMgr.Initialize", objectname:=_dataobject.ObjectID, messagetype:=otCoreMessageType.InternalError)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' returns a ObjectMessageLog
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectMessageLog As BusinessObjectMessageLog
            Get
                '** objectmessagelog
                If _objectmessagelog Is Nothing Then
                    _objectmessagelog = _dataobject.ObjectMessageLog
                End If
                Return _objectmessagelog
            End Get
        End Property

        ''' <summary>
        ''' gets the Relation Names of this data object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Relationnames As List(Of String)
            Get
                Return _dataobject.ObjectClassDescription.RelationNames
            End Get
        End Property
        ''' <summary>
        ''' returns true if the relation is loaded otherwise false
        ''' </summary>
        ''' <param name="relationname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Status(i As UShort) As RelationStatus
            Get
                If Not _isInitialized AndAlso Not Initialize() Then
                    CoreMessageHandler(message:="could not initialize DataObjectRelationMgr", objectname:=_dataobject.ObjectID, _
                                       procedure:="DataObjectRelationMgr.Get_Status", messagetype:=otCoreMessageType.InternalError)
                    Return 0
                End If

                Try
                    If i > _relationStatus.GetUpperBound(0) OrElse i < 0 Then
                        CoreMessageHandler(message:="relation found in relation names of class description out of bound of initialized relation set ?!", _
                                         argument:=i, procedure:="DataObjectRelationMgr.Get_Status", objectname:=_dataobject.ObjectID, _
                                         messagetype:=otCoreMessageType.InternalError)
                        Return 0
                    End If

                    '''
                    Return _relationStatus(i)
                Catch ex As Exception
                    CoreMessageHandler(exception:=ex, procedure:="DataObjectRelationMgr.Get_Status", objectname:=_dataobject.ObjectID, messagetype:=otCoreMessageType.InternalError)
                    Return 0
                End Try
            End Get
            Private Set(value As RelationStatus)
                If Not _isInitialized AndAlso Not Initialize() Then
                    CoreMessageHandler(message:="could not initialize DataObjectRelationMgr", objectname:=_dataobject.ObjectID, _
                                       procedure:="DataObjectRelationMgr.Set_Status", messagetype:=otCoreMessageType.InternalError)
                End If

                Try
                    If i > _relationStatus.GetUpperBound(0) Then
                        CoreMessageHandler(message:="relation found in relation names of class description out of bound of initialized relation set ?!", _
                                         argument:=i, procedure:="DataObjectRelationMgr.Set_Status", objectname:=_dataobject.ObjectID, _
                                         messagetype:=otCoreMessageType.InternalError)

                    End If

                    '''
                    _relationStatus(i) = value
                Catch ex As Exception
                    CoreMessageHandler(exception:=ex, procedure:="DataObjectRelationMgr.Set_Status", objectname:=_dataobject.ObjectID, messagetype:=otCoreMessageType.InternalError)
                End Try
            End Set
        End Property

        ''' <summary>
        ''' returns true if the relation is loaded otherwise false
        ''' </summary>
        ''' <param name="relationname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Status(relationname As String) As RelationStatus
            Get
                If Not _isInitialized AndAlso Not Initialize() Then
                    CoreMessageHandler(message:="could not initialize DataObjectRelationMgr", objectname:=_dataobject.ObjectID, _
                                       procedure:="DataObjectRelationMgr.Get_Status", messagetype:=otCoreMessageType.InternalError)
                    Return 0
                End If

                Try
                    Dim i = _dataobject.ObjectClassDescription.RelationNames.IndexOf(relationname.ToUpper)
                    If i < 0 Then
                        CoreMessageHandler(message:="relation not found in relation names of class description", _
                                           argument:=relationname, procedure:="DataObjectRelationMgr.Get_Status", objectname:=_dataobject.ObjectID, _
                                           messagetype:=otCoreMessageType.InternalError)
                        Return 0
                    End If

                    '''
                    Return Status(Convert.ToUInt16(i))
                Catch ex As Exception
                    CoreMessageHandler(exception:=ex, procedure:="DataObjectRelationMgr.Get_Status", objectname:=_dataobject.ObjectID, messagetype:=otCoreMessageType.InternalError)
                    Return 0
                End Try
            End Get
            Private Set(value As RelationStatus)
                If Not _isInitialized AndAlso Not Initialize() Then
                    CoreMessageHandler(message:="could not initialize DataObjectRelationMgr", objectname:=_dataobject.ObjectID, _
                                       procedure:="DataObjectRelationMgr.Set_Status", messagetype:=otCoreMessageType.InternalError)
                End If

                Try
                    Dim i = _dataobject.ObjectClassDescription.RelationNames.IndexOf(relationname.ToUpper)
                    If i < 0 Then
                        CoreMessageHandler(message:="relation not found in relation names of class description", _
                                           argument:=relationname, procedure:="DataObjectRelationMgr.Set_Status", objectname:=_dataobject.ObjectID, _
                                           messagetype:=otCoreMessageType.InternalError)
                    End If

                    '''
                    Status(Convert.ToUInt16(i)) = value
                Catch ex As Exception
                    CoreMessageHandler(exception:=ex, procedure:="DataObjectRelationMgr.Set_Status", objectname:=_dataobject.ObjectID, messagetype:=otCoreMessageType.InternalError)
                End Try
            End Set
        End Property

        ''' <summary>
        ''' returns true if the relationname is in the relation manager
        ''' </summary>
        ''' <param name="relationname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Contains(relationname As String) As Boolean
            Return Me.Relationnames.Contains(relationname)
        End Function
        ''' <summary>
        ''' selects dataobject from a relation mapped entry : optional if an entryname exist: select the data objects having the entryname containing the value
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectsFromContainer(relationname As String, _
                                                   Optional entryname As String = Nothing, _
                                                   Optional loadRelationIfNotloaded As Boolean = False, _
                                                   Optional value As Object = Nothing) As List(Of iormRelationalPersistable)

            If Not _isInitialized AndAlso Not Initialize() Then
                CoreMessageHandler(message:="could not initialize DataObjectRelationMgr", objectname:=_dataobject.ObjectID, _
                                   procedure:="DataObjectRelationMgr.Get_Status", messagetype:=otCoreMessageType.InternalError)
                Return New List(Of iormRelationalPersistable)
            End If

            Try


                Dim aFieldList As List(Of FieldInfo) = _dataobject.ObjectClassDescription.GetMappedRelation2FieldInfos(relationName:=relationname)
                Dim aList As New List(Of iormRelationalPersistable) ' results

                ''' check if relation is loaded
                ''' infuse it if necessary
                ''' 

                If Me.Status(relationname) = RelationStatus.Unloaded Then
                    If loadRelationIfNotloaded Then
                        If TryCast(_dataobject, iormInfusable).InfuseRelation(relationname) Then
                            Call CoreMessageHandler(procedure:="DataObjectRelationMgr.SelectObjectsFromContainer", _
                                                           message:="could not infuse relation into data object", _
                                                           argument:=relationname, objectname:=_dataobject.ObjectID, entryname:=entryname, containerID:=_dataobject.ObjectPrimaryTableID)
                            Return New List(Of iormRelationalPersistable)
                        End If

                        'Else -> still fetch objects from Container ! there might be somethong
                        '    Return New List(Of iormPersistable)
                    End If
                End If
                '''
                ''' go through all mapped fields
                For Each aFieldinfo In aFieldList
                    'Dim aMappingAttribute = _dataobject.ObjectClassDescription.GetEntryMappingAttributes(aFieldinfo.Name)

                    ''' check if the container holds only one type
                    If aFieldinfo.FieldType.GetInterfaces.Contains(GetType(iormRelationalPersistable)) Then
                        Dim aContainer As iormRelationalPersistable

                        If Not Reflector.GetFieldValue(field:=aFieldinfo, dataobject:=_dataobject, value:=aContainer) Then
                            Call CoreMessageHandler(procedure:="DataObjectRelationMgr.SelectObjectsFromContainer", _
                                                   message:="could not object mapped entry", _
                                                   argument:=aFieldinfo.Name, _
                                                   objectname:=_dataobject.ObjectID, _
                                                  entryname:=entryname, _
                                                   containerID:=_dataobject.ObjectPrimaryTableID)

                        End If

                        ''' add it or leave it
                        If aContainer IsNot Nothing AndAlso _
                            (entryname Is Nothing OrElse _
                             (aContainer.ObjectDefinition.HasEntry(entryname) AndAlso (value Is Nothing OrElse aContainer.GetValue(entryname).Equals(value)))) Then aList.Add(aContainer)


                        ''' check on arrays
                        ''' 
                    ElseIf aFieldinfo.FieldType.IsArray Then
                        Dim aContainer As iormRelationalPersistable()
                        If Not Reflector.GetFieldValue(field:=aFieldinfo, dataobject:=_dataobject, value:=aContainer) Then
                            Call CoreMessageHandler(procedure:="DataObjectRelationMgr.SelectObjectsFromContainer", _
                                                   message:="could not object mapped entry", _
                                                   argument:=aFieldinfo.Name, _
                                                   objectname:=_dataobject.ObjectID, _
                                                  entryname:=entryname, _
                                                   containerID:=_dataobject.ObjectPrimaryTableID)

                        End If
                        If aContainer IsNot Nothing Then
                            '' return the search condition
                            For Each anObject In aContainer.ToList
                                If (entryname Is Nothing OrElse _
                                    (anObject.ObjectDefinition.HasEntry(entryname) AndAlso _
                                        (value Is Nothing OrElse anObject.GetValue(entryname).Equals(value)))) Then
                                    aList.Add(anObject)
                                End If
                            Next
                        End If

                        '*** Lists
                    ElseIf aFieldinfo.FieldType.GetInterfaces.Contains(GetType(IList)) Then
                        Dim aContainer As IList
                        '** setfieldvalue by hook or slooow
                        If Not Reflector.GetFieldValue(field:=aFieldinfo, dataobject:=_dataobject, value:=aContainer) Then
                            Call CoreMessageHandler(procedure:="DataObjectRelationMgr.SelectObjectsFromContainer", _
                                                    message:="could not object mapped entry", _
                                                    argument:=aFieldinfo.Name, objectname:=_dataobject.ObjectID, _
                                                   entryname:=entryname, containerID:=_dataobject.ObjectPrimaryTableID)
                        End If
                        If aContainer IsNot Nothing Then
                            '' return the search condition
                            For Each anObject In aContainer
                                If (entryname Is Nothing OrElse _
                                    (anObject.ObjectDefinition.HasEntry(entryname) AndAlso _
                                        (value Is Nothing OrElse anObject.GetValue(entryname).Equals(value)))) Then
                                    aList.Add(anObject)
                                End If
                            Next
                        End If
                        '*** Dictionary
                    ElseIf aFieldinfo.FieldType.GetInterfaces.Contains(GetType(IDictionary)) Then

                        Dim aContainer As IDictionary '= aFieldinfo.GetValue(_dataobject)
                        Reflector.GetFieldValue(field:=aFieldinfo, dataobject:=_dataobject, value:=aContainer)

                        '** select
                        If aContainer IsNot Nothing Then
                            '' return the search condition
                            For Each anObject In aContainer.Values
                                If (entryname Is Nothing OrElse _
                                    (anObject.ObjectDefinition.HasEntry(entryname) AndAlso _
                                        (value Is Nothing OrElse anObject.GetValue(entryname).Equals(value)))) Then
                                    aList.Add(anObject)
                                End If
                            Next
                        End If
                        '*** relationCollection
                    ElseIf Reflector.TypeImplementsGenericInterface(aFieldinfo.FieldType, GetType(iormRelationalCollection(Of ))) Then
                        Dim aGenericContainer As Object
                        If Not Reflector.GetFieldValue(field:=aFieldinfo, dataobject:=_dataobject, value:=aGenericContainer) Then
                            Call CoreMessageHandler(procedure:="DataObjectRelationMgr.SelectObjectsFromContainer", _
                                            message:="iormRelationalCollection must not be nothing", _
                                            argument:=aFieldinfo.Name, objectname:=_dataobject.ObjectID, entryname:=entryname, containerID:=_dataobject.ObjectPrimaryTableID)
                        End If

                        If aGenericContainer IsNot Nothing Then
                            Dim aContainerType As Type = GetType(iormRelationalCollection(Of )).MakeGenericType(aFieldinfo.FieldType.GetGenericTypeDefinition)
                            'Dim aContainer As iormRelationalCollection(Of iormPersistable) = aGenericContainer -> through cast exception :-(

                            ''' use the index of the container to select if this is the key !
                            ''' 
                            Dim keynames As String() = aGenericContainer.Keynames
                            If entryname IsNot Nothing AndAlso keynames.Length = 1 AndAlso Array.Exists(keynames, Function(x) x = entryname) Then
                                If value IsNot Nothing Then
                                    Dim anObject As iormRelationalPersistable = aGenericContainer.Item(value)
                                    If anObject IsNot Nothing Then aList.Add(anObject)
                                Else
                                    aList.AddRange(aGenericContainer.ToList)
                                End If

                            Else
                                ''' or select conventionally
                                ''' 
                                For Each anObject In aGenericContainer
                                    If (entryname Is Nothing OrElse _
                                     (anObject.ObjectDefinition.HasEntry(entryname) AndAlso _
                                         (value Is Nothing OrElse anObject.GetValue(entryname).Equals(value)))) Then
                                        aList.Add(anObject)
                                    End If
                                Next
                            End If
                        End If

                    End If

                Next


                Return aList
            Catch ex As Exception
                Call CoreMessageHandler(procedure:="DataObjectRelationMgr.SelectObjectsFromContainer", exception:=ex, _
                                        argument:=value, objectname:=_dataobject.ObjectID, entryname:=entryname, containerID:=_dataobject.ObjectPrimaryTableID)
                Return New List(Of iormRelationalPersistable)
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="relationname"></param>
        ''' <param name="mode"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InfuseRelatedObjectIntoContainer(relationname As String, mode As otInfuseMode, objects As List(Of iormDataObject)) As Boolean
            If Not _isInitialized AndAlso Not Initialize() Then
                CoreMessageHandler(message:="could not initialize DataObjectRelationMgr", objectname:=_dataobject.ObjectID, _
                                   procedure:="DataObjectRelationMgr.InfuseRelatedObjectIntoContainer", messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            Try

                Dim aFieldList As List(Of FieldInfo) = _dataobject.ObjectClassDescription.GetMappedRelation2FieldInfos(relationName:=relationname)

                For Each aFieldInfo In aFieldList
                    Dim aMappingAttribute = _dataobject.ObjectClassDescription.GetEntryMappingAttributes(aFieldInfo.Name)

                    ''' check if the container holds only one type
                    If aFieldInfo.FieldType.GetInterfaces.Contains(GetType(iormRelationalPersistable)) Then
                        '** setfieldvalue by hook or slooow
                        If Not Reflector.SetFieldValue(field:=aFieldInfo, dataobject:=_dataobject, value:=objects.First) Then
                            Call CoreMessageHandler(procedure:="DataObjectRelationMgr.InfuseRelatedObjectIntoContainer", _
                                                   message:="could not object mapped entry", _
                                                   argument:=aFieldInfo.Name, objectname:=_dataobject.ObjectID, entryname:=aMappingAttribute.EntryName, containerID:=_dataobject.ObjectPrimaryTableID)
                            Return False
                        End If
                        Return True
                        ''' Arrays
                    ElseIf aFieldInfo.FieldType.IsArray Then
                        '** setfieldvalue by hook or slooow
                        If Not Reflector.SetFieldValue(field:=aFieldInfo, dataobject:=_dataobject, value:=objects.ToArray) Then
                            Call CoreMessageHandler(procedure:="DataObjectRelationMgr.InfuseRelatedObjectIntoContainer", _
                                                   message:="could not object mapped entry", _
                                                   argument:=aFieldInfo.Name, objectname:=_dataobject.ObjectID, entryname:=aMappingAttribute.EntryName, containerID:=_dataobject.ObjectPrimaryTableID)
                            Return False
                        End If
                        Return True
                        '*** Lists
                    ElseIf aFieldInfo.FieldType.GetInterfaces.Contains(GetType(IList)) Then
                        '** setfieldvalue by hook or slooow
                        If Not Reflector.SetFieldValue(field:=aFieldInfo, dataobject:=_dataobject, value:=objects) Then
                            Call CoreMessageHandler(procedure:="DataObjectRelationMgr.InfuseRelatedObjectIntoContainer", _
                                                    message:="could not object mapped entry", _
                                                    argument:=aFieldInfo.Name, objectname:=_dataobject.ObjectID, entryname:=aMappingAttribute.EntryName, containerID:=_dataobject.ObjectPrimaryTableID)
                            Return False
                        End If

                        Return True
                        '*** Dictionary
                    ElseIf aFieldInfo.FieldType.GetInterfaces.Contains(GetType(IDictionary)) Then
                        Dim aDictionary As IDictionary '= aFieldinfo.GetValue(_dataobject)
                        Reflector.GetFieldValue(field:=aFieldInfo, dataobject:=_dataobject, value:=aDictionary)
                        Dim typedef As Type() = aFieldInfo.FieldType.GetGenericArguments()

                        '** create
                        If aDictionary Is Nothing Then
                            If aFieldInfo.FieldType.IsGenericType Then
                                Dim specifictype = aFieldInfo.FieldType.MakeGenericType(typedef)
                                aDictionary = Activator.CreateInstance(specifictype)
                            Else
                                aDictionary = Activator.CreateInstance(aFieldInfo.FieldType)
                            End If

                            '** setfieldvalue by hook or slooow
                            If Not Reflector.SetFieldValue(field:=aFieldInfo, dataobject:=_dataobject, value:=aDictionary) Then
                                Call CoreMessageHandler(procedure:="DataObjectRelationMgr.InfuseRelatedObjectIntoContainer", _
                                        message:="could not object mapped entry", _
                                        argument:=aFieldInfo.Name, objectname:=_dataobject.ObjectID, entryname:=aMappingAttribute.EntryName, containerID:=_dataobject.ObjectPrimaryTableID)

                                Return False
                            End If
                        End If

                        '** assign
                        For Each anObject In objects
                            If typedef(0) = GetType(String) Then
                                Dim aKey As String = String.Empty
                                For i = 0 To aMappingAttribute.KeyEntries.Count - 1
                                    If i > 0 Then aKey &= ConstDelimiter
                                    aKey &= anObject.Record.GetValue(aMappingAttribute.KeyEntries(i)).ToString
                                Next
                                If Not aDictionary.Contains(key:=aKey) Then
                                    aDictionary.Add(key:=aKey, value:=anObject)
                                Else
                                    CoreMessageHandler(message:="for relation '" & relationname & "' :key in dictionary member '" & aFieldInfo.Name & "' already exists", _
                                                       messagetype:=otCoreMessageType.InternalWarning, _
                                                       objectname:=_dataobject.ObjectID, containerID:=_dataobject.ObjectPrimaryTableID, _
                                                       argument:=aKey, procedure:="DataObjectRelationMgr.InfuseRelatedObjectIntoContainer")
                                End If


                            ElseIf typedef(0).Equals(GetType(Int64)) And IsNumeric(anObject.Record.GetValue(aMappingAttribute.KeyEntries(0))) Then
                                Dim aKey As Long = CLng(anObject.Record.GetValue(aMappingAttribute.KeyEntries(0)))
                                If Not aDictionary.Contains(key:=aKey) Then
                                    aDictionary.Add(key:=aKey, value:=anObject)
                                Else
                                    CoreMessageHandler(message:="for relation '" & relationname & "' :key in dictionary member '" & aFieldInfo.Name & "' already exists", _
                                                       messagetype:=otCoreMessageType.InternalWarning, _
                                                       objectname:=_dataobject.ObjectID, containerID:=_dataobject.ObjectPrimaryTableID, _
                                                       argument:=aKey, procedure:="DataObjectRelationMgr.InfuseRelatedObjectIntoContainer")
                                End If
                            Else
                                Call CoreMessageHandler(procedure:="DataObjectRelationMgr.InfuseRelatedObjectIntoContainer", message:="cannot convert key to dicitionary from List of iormpersistables", _
                                                        objectname:=_dataobject.ObjectID, containerID:=_dataobject.ObjectPrimaryTableID)
                            End If
                        Next

                        Return True
                        '*** relationCollection
                    ElseIf Reflector.TypeImplementsGenericInterface(aFieldInfo.FieldType, GetType(iormRelationalCollection(Of ))) Then
                        Dim aCollection As Object
                        If Not Reflector.GetFieldValue(field:=aFieldInfo, dataobject:=_dataobject, value:=aCollection) Then
                            Call CoreMessageHandler(procedure:="DataObjectRelationMgr.InfuseRelatedObjectIntoContainer", _
                                            message:="iormRelationalCollection must not be nothing", _
                                            argument:=aFieldInfo.Name, objectname:=_dataobject.ObjectID, entryname:=aMappingAttribute.EntryName, containerID:=_dataobject.ObjectPrimaryTableID)
                            Return False
                        End If

                        '** assign
                        For Each anObject In objects
                            aCollection.Add(anObject)
                        Next
                        Return True
                    End If

                Next

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="DataObjectRelationMgr.InfuseRelatedObjectIntoContainer", exception:=ex, _
                                      argument:=relationname, objectname:=_dataobject.ObjectID, containerID:=_dataobject.ObjectPrimaryTableID)
                Return False

            End Try

        End Function
        ''' <summary>
        ''' returns dataobjects from a Container-Data Structure
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadNInfuseRelations(mode As otInfuseMode, Optional ByRef relationnames As List(Of String) = Nothing, Optional force As Boolean = False) As Boolean

            If Not _isInitialized AndAlso Not Initialize() Then
                CoreMessageHandler(message:="could not initialize DataObjectRelationMgr", objectname:=_dataobject.ObjectID, _
                                   procedure:="DataObjectRelationMgr.LoadRelations", messagetype:=otCoreMessageType.InternalError)
                Return False
            End If
            Try
                Dim relationLoaded As Boolean
                '''
                ''' go through all relations
                ''' 
                If relationnames Is Nothing Then relationnames = _dataobject.ObjectClassDescription.RelationNames

                For Each relationname In relationnames
                    relationLoaded = False
                    Dim aRelationAttribute = _dataobject.ObjectClassDescription.GetRelationAttribute(relationname:=relationname)
                    Dim aList As New List(Of iormDataObject)
                    '''
                    ''' run if it was not loaded before or force
                    ''' 
                    'If aRelationAttribute IsNot Nothing AndAlso (Me.Status(relationname) = RelationStatus.Unloaded OrElse force) Then
                    If aRelationAttribute IsNot Nothing Then
                        Dim aFieldList As List(Of FieldInfo) = _dataobject.ObjectClassDescription.GetMappedRelation2FieldInfos(relationName:=aRelationAttribute.Name)

                        For Each aFieldInfo In aFieldList
                            Dim aMappingAttribute = _dataobject.ObjectClassDescription.GetEntryMappingAttributes(aFieldInfo.Name)

                            '''
                            ''' check on the right mode
                            ''' 
                            If (mode And aMappingAttribute.InfuseMode) Then
                                relationLoaded = True
                                Dim startCount = aList.Count ' for checking if we have found anything

                                ''' raise event
                                ''' 
                                Dim theEventargs As New ormRelationManager.EventArgs(relationname, relationAttribute:=aRelationAttribute, _
                                                                                        dataobject:=_dataobject, fieldinfo:=aFieldInfo, infusemode:=mode)
                                RaiseEvent OnRelatedObjectsRetrieveRequest(Me, theEventargs)
                                If theEventargs.Objects.Count > 0 Then aList.AddRange(theEventargs.Objects)

                                '''
                                ''' check if the Event das brought back all events
                                If theEventargs.Finished Then
                                    ''' do nothing

                                    '''
                                    ''' 2. check on an Operation to call first
                                ElseIf aRelationAttribute.HasValueRetrieveOperationID Then
                                    aList.AddRange(Me.GetRelatedObjectsByOperation(relationname:=relationname, operationname:=aRelationAttribute.RetrieveOperation))

                                ElseIf aRelationAttribute.HasValueToPrimarykeys Then
                                    '''
                                    ''' 3. get the related Object by Retrieving
                                    ''' 
                                    Dim anObject As iormDataObject = Me.GetRelatedObjectByRetrieve(relationname:=relationname)
                                    If anObject IsNot Nothing Then aList.Add(anObject)
                                Else
                                    '''
                                    ''' 4. get the related Objects by Query from the data store
                                    ''' 
                                    aList.AddRange(Me.GetRelatedObjects(relationname:=relationname))
                                End If

                                '''
                                ''' have we received anything ?!
                                ''' 
                                If startCount - aList.Count = 0 Then
                                    '''
                                    ''' create related objects -> use own infuse
                                    ''' 
                                    If aRelationAttribute.HasValueCreateObjectIfNotRetrieved AndAlso aRelationAttribute.CreateObjectIfNotRetrieved Then
                                        '' create and infuse just this relation
                                        CreateNInfuseRelations(mode:=mode, relationnames:={aRelationAttribute.Name}.ToList)
                                    End If
                                End If
                            End If
                        Next
                    End If



                    '''
                    ''' infuse the dataobject mapped containers with the List
                    ''' 
                    If relationLoaded AndAlso aList.Count > 0 Then
                        If Not InfuseRelatedObjectIntoContainer(relationname, mode, aList) Then
                            CoreMessageHandler(message:="failed to infuse relation container objects in data object", argument:=relationname, objectname:=_dataobject.ObjectID, _
                                                procedure:="DataObjectRelationMgr.LoadRelation", messagetype:=otCoreMessageType.InternalError)
                            Return False
                        End If
                    End If

                    ''' set the status
                    If relationLoaded Then Me.Status(relationname) = RelationStatus.Loaded
                Next

                ''' return
                Return True

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, argument:=relationnames.ToString, objectname:=_dataobject.ObjectID, _
                                     procedure:="DataObjectRelationMgr.LoadRelation", messagetype:=otCoreMessageType.InternalError)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' returns dataobjects from a Container-Data Structure
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateNInfuseRelations(mode As otInfuseMode, _
                                               Optional ByRef relationnames As List(Of String) = Nothing, _
                                               Optional force As Boolean = False) As Boolean

            If Not _isInitialized AndAlso Not Initialize() Then
                CoreMessageHandler(message:="could not initialize DataObjectRelationMgr", objectname:=_dataobject.ObjectID, _
                                   procedure:="DataObjectRelationMgr.CreateRelations", messagetype:=otCoreMessageType.InternalError)
                Return False
            End If
            Try
                Dim relationLoaded As Boolean

                '''
                ''' go through all relations
                ''' 
                If relationnames Is Nothing Then relationnames = _dataobject.ObjectClassDescription.RelationNames

                For Each relationname In relationnames
                    Dim aRelationAttribute = _dataobject.ObjectClassDescription.GetRelationAttribute(relationname:=relationname)
                    Dim aList As New List(Of iormDataObject)
                    '''
                    ''' check if loaded before or force -> what to to with outdated relations ?!
                    ''' 
                    ''' If aRelationAttribute IsNot Nothing AndAlso (Me.Status(relationname) = RelationStatus.Unloaded OrElse force) Then
                    If aRelationAttribute IsNot Nothing Then

                        Dim aFieldList As List(Of FieldInfo) = _dataobject.ObjectClassDescription.GetMappedRelation2FieldInfos(relationName:=aRelationAttribute.Name)

                        For Each aFieldInfo In aFieldList
                            Dim aMappingAttribute = _dataobject.ObjectClassDescription.GetEntryMappingAttributes(aFieldInfo.Name)
                            relationLoaded = True
                            '''
                            ''' no check on the right mode
                            ''' 


                            ''' raise event
                            ''' 
                            Dim theEventargs As New ormRelationManager.EventArgs(relationname, relationAttribute:=aRelationAttribute, _
                                                                                    dataobject:=_dataobject, fieldinfo:=aFieldInfo, infusemode:=mode)
                            RaiseEvent OnRelatedObjectsCreateRequest(Me, theEventargs)
                            If theEventargs.Objects.Count > 0 Then aList.AddRange(theEventargs.Objects)

                            '''
                            ''' check if the Event das brought back all events
                            If theEventargs.Finished Then

                                ''' do nothing


                                '''
                                ''' 2. check on an Operation to call first
                            ElseIf aRelationAttribute.HasValueCreateOperationID Then
                                ' not implemented
                                aList.AddRange(Me.GetRelatedObjectsByOperation(relationname:=relationname, operationname:=aRelationAttribute.CreateOperation))

                            ElseIf aRelationAttribute.HasValueToPrimarykeys Then
                                Dim anObject = Me.GetRelatedObjectByCreate(relationname:=relationname)
                                '** setfieldvalue by hook or slooow
                                aList.Add(anObject)

                            Else
                                '''
                                ''' 4. get the related Objects by Query from the data store
                                ''' 
                                ''' not implemented !
                                ''' 
                                ''' aList.AddRange(Me.GetRelatedObjectsByQuery(relationname:=relationname))
                            End If
                        Next
                    End If

                    '''
                    ''' infuse the dataobject mapped containers with the List
                    ''' 
                    If relationLoaded AndAlso aList.Count > 0 Then
                        If Not InfuseRelatedObjectIntoContainer(relationname, mode, aList) Then
                            CoreMessageHandler(message:="failed to infuse relation container objects in data object", argument:=relationname, objectname:=_dataobject.ObjectID, _
                                                procedure:="DataObjectRelationMgr.CreateRelations", messagetype:=otCoreMessageType.InternalError)
                            Return False
                        Else

                        End If
                    End If

                    ''' set the status anyway
                    If relationLoaded Then Me.Status(relationname) = RelationStatus.Loaded
                Next

                ''' return
                Return True

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, argument:=relationnames.ToString, objectname:=_dataobject.ObjectID, _
                                     procedure:="DataObjectRelationMgr.CreateRelations", messagetype:=otCoreMessageType.InternalError)
                Return False
            End Try

        End Function
        ''' <summary>
        ''' returns dataobjects from a Container-Data Structure
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CascadeRelations(Optional cascadeUpdate As Boolean = False,
                                         Optional cascadeDelete As Boolean = False, _
                                         Optional timestamp As DateTime = constNullDate, _
                                         Optional uniquenesswaschecked As Boolean = True,
                                         Optional ByRef relationnames As List(Of String) = Nothing) As Boolean

            If Not _isInitialized AndAlso Not Initialize() Then
                CoreMessageHandler(message:="could not initialize DataObjectRelationMgr", objectname:=_dataobject.ObjectID, _
                                   procedure:="DataObjectRelationMgr.CascadeRelations", messagetype:=otCoreMessageType.InternalError)
                Return False
            End If
            Try
                '''
                ''' go through all relations
                ''' 
                If relationnames Is Nothing Then relationnames = _dataobject.ObjectClassDescription.RelationNames
                Dim result As Boolean = True

                For Each relationname In relationnames
                    Dim aRelationAttribute = _dataobject.ObjectClassDescription.GetRelationAttribute(relationname:=relationname)


                    '''
                    ''' check  if this relation needs to be cascaded
                    If aRelationAttribute IsNot Nothing AndAlso _
                        ((cascadeUpdate AndAlso cascadeUpdate = aRelationAttribute.CascadeOnUpdate) OrElse _
                        (cascadeDelete AndAlso cascadeDelete = aRelationAttribute.CascadeOnDelete)) Then

                        Dim theObjectsList = Me.GetObjectsFromContainer(relationname, loadRelationIfNotloaded:=False)
                        For Each anObject In theObjectsList

                            ''' Cascade Update
                            If cascadeUpdate AndAlso cascadeUpdate = aRelationAttribute.CascadeOnUpdate Then

                                ''' listen to the messages
                                AddHandler TryCast(anObject, iormLoggable).BusinessObjectMessageLog.OnObjectMessageAdded, AddressOf Me.DataObject_OnObjectMessageAdded
                                ''' here persist

                                If Not anObject.RuntimeOnly AndAlso Not anObject.Persist(timestamp:=timestamp) Then
                                    CoreMessageHandler("object could not persist", dataobject:=anObject, messagetype:=otCoreMessageType.InternalError, _
                                                       procedure:="DataObjectRelationMgr.CascadeRelation")
                                    result = result And False
                                ElseIf anObject.RuntimeOnly Then
                                    CoreMessageHandler("object on RuntimeOnly could not persist", dataobject:=anObject, messagetype:=otCoreMessageType.InternalWarning, _
                                                      procedure:="DataObjectRelationMgr.CascadeRelation")
                                    result = result And False
                                Else
                                    result = result And True
                                End If

                                ''' stop listing to the messages
                                RemoveHandler TryCast(anObject, iormLoggable).BusinessObjectMessageLog.OnObjectMessageAdded, AddressOf Me.DataObject_OnObjectMessageAdded

                                ''' if we are not loaded with check on uniqueness
                                ''' and cascade the relation updates
                                ''' we need to make sure that all older relations are deleted
                                If Not uniquenesswaschecked Then
                                    Me.DeleteRelatedObjects(relationname, timestamp:=timestamp)
                                End If

                            End If
                            ''' Cascade Delete
                            If cascadeDelete AndAlso cascadeDelete = aRelationAttribute.CascadeOnDelete Then
                                result = result And anObject.Delete(timestamp:=timestamp)
                            End If

                        Next
                    End If

                Next

                ''' return
                Return result

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, argument:=relationnames.ToString, objectname:=_dataobject.ObjectID, _
                                     procedure:="DataObjectRelationMgr.CascadeRelations", messagetype:=otCoreMessageType.InternalError)
                Return False
            End Try

        End Function
        ''' <summary>
        ''' Event Handler for ObjectMessageLogs propagate
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub DataObject_OnObjectMessageAdded(sender As Object, e As BusinessObjectMessageLog.EventArgs)

            '** if concerning ?!
            If e.Message.StatusItems(statustype:=ConstStatusType_ObjectValidation).Count > 0 OrElse _
                e.Message.StatusItems(statustype:=ConstStatusType_ObjectEntryValidation).Count > 0 Then
                '** add it
                Me.ObjectMessageLog.Add(e.Message)
            End If
        End Sub
        ''' <summary>
        ''' create a  related objects from a relation attribute for a object class described by a classdescriptor
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="classdescriptor"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetRelatedObjectByCreate(relationname As String) As iormDataObject
            Dim aRelationAttribute As ormRelationAttribute = _dataobject.ObjectClassDescription.GetRelationAttribute(relationname)
            If aRelationAttribute Is Nothing Then
                CoreMessageHandler(message:="relation was not found in classdescription", _
                                   argument:=relationname, objectname:=_dataobject.ObjectID, _
                                    procedure:="DataObjectRelationMgr.GetObjectByCreate", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If
            Dim theKeyvalues As New List(Of Object)
            Dim keyentries As String()
            Dim runtimeOnly As Boolean = _dataobject.RunTimeOnly

            '** get the keys althoug determining if TOEntries are by Primarykey is a bit obsolete
            If aRelationAttribute.HasValueToPrimarykeys Then
                keyentries = aRelationAttribute.ToPrimaryKeys
            Else
                CoreMessageHandler(message:="relation attribute has no ToPrimarykeys set - unable to create", _
                                    argument:=aRelationAttribute.Name, objectname:=_dataobject.ObjectID, _
                                     procedure:="DataObjectRelationMgr.GetObjectByCreate", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If

            Try
                Dim aTargetObjectDescriptor As ObjectClassDescription = ot.GetObjectClassDescription(aRelationAttribute.LinkObject)
                theKeyvalues = Reflector.GetContainerEntryValues(dataobject:=_dataobject, entrynames:=keyentries)
                Dim aFactory As iormDataObjectProvider = ot.CurrentSession.DataObjectProvider(objectid:=aTargetObjectDescriptor.ObjectAttribute.ID)
                'Dim aDelegate As ObjectClassDescription.OperationCallerDelegate = aTargetObjectDescriptor.GetOperartionCallerDelegate(operationname:=anOperationAttribute.OperationName)

                If aFactory IsNot Nothing Then
                    Dim aPrimaryKey As ormDatabaseKey = New ormDatabaseKey(objectid:=aTargetObjectDescriptor.ObjectAttribute.ID, keyvalues:=theKeyvalues.ToArray)
                    Dim anobject As iormDataObject = aFactory.Create(primarykey:=aPrimaryKey, type:=aTargetObjectDescriptor.Type, runTimeonly:=runtimeOnly)
                    Return anobject
                Else
                    CoreMessageHandler(message:="No factory found in current session for object id '" & aTargetObjectDescriptor.ObjectAttribute.ID & "'", _
                                        messagetype:=otCoreMessageType.InternalError, procedure:="ormRelationalManager.GetRelatedObjectByCreate")
                    Return Nothing
                End If

                ''' OBSOLETE CODE
                ''' 

                'Dim anOperationAttribute As ormObjectOperationMethodAttribute = _
                '    aTargetObjectDescriptor.GetObjectOperationAttributeByTag(tag:=ObjectClassDescription.ConstMTCreateDataObject)
                'Dim aPrimaryKey As ormDatabaseKey

                'If anOperationAttribute IsNot Nothing Then
                '    '''
                '    ''' new code - fast
                '    ''' 
                '    theKeyvalues = Reflector.GetContainerEntryValues(dataobject:=_dataobject, entrynames:=keyentries)
                '    Dim aFactory As iormDataObjectFactory = ot.CurrentSession.DataObjectFactory(objectid:=aTargetObjectDescriptor.ObjectAttribute.ID)
                '    Dim aDelegate As ObjectClassDescription.OperationCallerDelegate = aTargetObjectDescriptor.GetOperartionCallerDelegate(operationname:=anOperationAttribute.OperationName)
                '    aPrimaryKey = New ormDatabaseKey(objectid:=aTargetObjectDescriptor.ObjectAttribute.ID, keyvalues:=theKeyvalues.ToArray)
                '    Dim anobject As iormInfusable = ot.CurrentSession.DataObjectFactory(objectid:=aTargetObjectDescriptor.ObjectAttribute.ID).Create()

                '    '** relate also in the runtime !
                '    Dim anObject As iormRelationalPersistable = aDelegate(Nothing, {aPrimaryKey, aTargetObjectDescriptor.Type, Nothing, Nothing, runtimeOnly})
                '    Return anObject

                'Else
                '    '''
                '    ''' old code - slow
                '    ''' 


                '    Dim aTargetType As System.Type = aTargetObjectDescriptor.Type
                '    theKeyvalues = Reflector.GetContainerEntryValues(dataobject:=_dataobject, entrynames:=keyentries)
                '    aPrimaryKey = New ormDatabaseKey(objectid:=aTargetObjectDescriptor.ObjectAttribute.ID, keyvalues:=theKeyvalues.ToArray)
                '    Dim createMethod = ot.GetMethodInfo(aTargetType, ObjectClassDescription.ConstMTCreateDataObject)

                '    If createMethod IsNot Nothing Then
                '        '** if creating then do also with the new data object in the runtime
                '        Dim anObject As iormRelationalPersistable = createMethod.Invoke(Nothing, {aPrimaryKey, Nothing, Nothing, runtimeOnly})
                '        Return anObject
                '    Else
                '        CoreMessageHandler(message:="the CREATE method was not found on this object class", messagetype:=otCoreMessageType.InternalError, _
                '                            objectname:=aTargetType.Name, procedure:="DataObjectRelationMgr.GetObjectByCreate")
                '        Return Nothing
                '    End If
                'End If

                '*** return
                Return Nothing

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, _
                                    argument:=aRelationAttribute.Name, objectname:=_dataobject.ObjectID, _
                                     procedure:="DataObjectRelationMgr.GetRelatedObjectByCreate")
                Return Nothing
            End Try

        End Function
        ''' <summary>
        ''' get the related objects from a call to an operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetRelatedObjectsByOperation(relationname As String, operationname As String) As List(Of iormDataObject)
            Dim aList As New List(Of iormDataObject)
            If Not _isInitialized AndAlso Not Initialize() Then
                CoreMessageHandler(message:="could not initialize DataObjectRelationMgr", objectname:=_dataobject.ObjectID, _
                                   procedure:="DataObjectRelationMgr.GetRelatedObjectsByOperation", messagetype:=otCoreMessageType.InternalError)
                Return New List(Of iormDataObject)
            End If
            Try
                Dim aRelationAttribute = _dataobject.ObjectClassDescription.GetRelationAttribute(relationname)
                If aRelationAttribute Is Nothing Then Return aList

                Dim aOperationAttribute = _dataobject.ObjectClassDescription.GetObjectOperationAttribute(name:=operationname)
                If aOperationAttribute Is Nothing Then
                    CoreMessageHandler(message:="operation id not found in the class description repository", argument:=operationname, messagetype:=otCoreMessageType.InternalError, _
                                       procedure:="DataObjetRelationMGr.GetRelatedObjectsFromOperation")
                    Return aList
                End If

                ''' check the data on the method to be called
                ''' 
                Dim fromEntries As String()
                If aRelationAttribute.HasValueFromEntries Then
                    fromEntries = aRelationAttribute.FromEntries
                Else
                    ' not an error might be a warning
                    'CoreMessageHandler(message:="to call an operation the relation attribute needs to define from entries to match parameter entries", arg1:=relationname, messagetype:=otCoreMessageType.InternalError, _
                    '                  subname:="DataObjetRelationMGr.GetRelatedObjectsFromOperation")
                    'Return aList
                End If
                Dim theEntryValues As Object() = Reflector.GetContainerEntryValues(dataobject:=_dataobject, entrynames:=fromEntries).ToArray
                Dim aMethodInfo As MethodInfo = aOperationAttribute.MethodInfo
                Dim aReturnType As System.Type = aMethodInfo.ReturnType
                Dim aDelegate As ObjectClassDescription.OperationCallerDelegate = _dataobject.ObjectClassDescription.GetOperartionCallerDelegate(operationname)
                Dim theParameters As Object()
                ReDim theParameters(aMethodInfo.GetParameters.Count - 1)
                ''' set the parameters for the delegate
                For i = 0 To theParameters.GetUpperBound(0)
                    Dim j As Integer = aMethodInfo.GetParameters(i).Position
                    If j >= 0 AndAlso j <= theParameters.GetUpperBound(0) Then
                        theParameters(j) = theEntryValues(j)
                    End If
                Next

                ''' call the Operation
                ''' 
                Dim result As Object = aDelegate(_dataobject, theParameters)

                ''' check if the container holds only one type
                If aReturnType.GetInterfaces.Contains(GetType(iormDataObject)) Then
                    Dim anObject As iormDataObject = TryCast(result, iormDataObject)
                    If anObject IsNot Nothing Then aList.Add(anObject)

                    ''' check on arrays
                    ''' 
                ElseIf aReturnType.IsArray Then
                    Dim theObjects As iormDataObject() = TryCast(result, iormDataObject())
                    If theObjects IsNot Nothing Then
                        aList.AddRange(theObjects)
                    End If

                    '*** Lists
                ElseIf aReturnType.GetInterfaces.Contains(GetType(IList)) Then
                    Dim aContainer As IList = result
                    If aContainer IsNot Nothing Then
                        '' return the search condition
                        For Each anObject In aContainer
                            aList.Add(TryCast(anObject, iormDataObject))
                        Next
                    End If
                    '*** Dictionary
                ElseIf aReturnType.GetInterfaces.Contains(GetType(IDictionary)) Then

                    Dim aContainer As IDictionary = result
                    '** select
                    If aContainer Is Nothing Then
                        '' return the search condition
                        For Each anObject In aContainer.Values
                            aList.Add(TryCast(anObject, iormDataObject))
                        Next
                    End If
                    '*** relationCollection
                ElseIf Reflector.TypeImplementsGenericInterface(aReturnType, GetType(iormRelationalCollection(Of ))) Then
                    Dim aContainer As iormRelationalCollection(Of iormDataObject) = result
                    aList.AddRange(aContainer.ToList)
                End If


                Return aList
            Catch ex As Exception
                Call CoreMessageHandler(procedure:="DataObjectRelationMgr.GetRelatedObjectsByOperation ", exception:=ex, _
                                        argument:=relationname, objectname:=_dataobject.ObjectID)
                Return New List(Of iormDataObject)
            End Try
        End Function
        ''' <summary>
        ''' retrieves a list of related objects from a relation attribute for a object class described by a classdescriptor
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="dataobject"></param>
        ''' <param name="classdescriptor"></param>
        ''' <param name="dbdriver"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetRelatedObjects(relationname As String) As List(Of iormDataObject)

            If Not _isInitialized AndAlso Not Initialize() Then
                CoreMessageHandler(message:="could not initialize DataObjectRelationMgr", objectname:=_dataobject.ObjectID, _
                                   procedure:="DataObjectRelationMgr.GetRelatedObjectsByQuery", messagetype:=otCoreMessageType.InternalError)
                Return New List(Of iormDataObject)
            End If
            Dim arelationAttribute As ormRelationAttribute = _dataobject.ObjectClassDescription.GetRelationAttribute(relationname)
            ''' return the related object
            Dim aFactory As iormDataObjectProvider = ot.CurrentSession.DataObjectProvider(arelationAttribute.LinkObject)
            If aFactory IsNot Nothing Then Return aFactory.RetrieveByRelation(arelationAttribute, sourceobject:=_dataobject)

            CoreMessageHandler(message:="no factory available for '" & arelationAttribute.LinkObject.FullName & "' - is this a dataobject ?!", _
                                procedure:="ormRelationManager.GetRelatedObjects")

            Return New List(Of iormDataObject)

            'Dim theKeyvalues As New List(Of Object)
            'Dim theObjectList As New List(Of iormDataObject)


            '''' work on the query
            '''' 
            'Dim domainBehavior As Boolean
            'Dim deletebehavior As Boolean
            'Dim FNDomainID As String = Domain.ConstFNDomainID
            'Dim FNDeleted As String = ConstFNIsDeleted
            'Dim domainID As String = CurrentSession.CurrentDomainID
            'Dim fromTablename As String = _dataobject.ObjectClassDescription.PrimaryContainerID
            'Dim ToTableID = aTargetObjectDescriptor.PrimaryContainerID  ' First Tablename if multiple

            ''** get the keys althoug determining if TOEntries are by Primarykey is a bit obsolete
            'If Not arelationAttribute.HasValueFromEntries OrElse Not arelationAttribute.HasValueToEntries Then
            '    CoreMessageHandler(message:="relation attribute has nor fromEntries or ToEntries set", _
            '                        argument:=arelationAttribute.Name, objectname:=_dataobject.ObjectID, _
            '                         procedure:="DataObjectRelationMgr.GetRelatedObjects", messagetype:=otCoreMessageType.InternalError)
            '    Return theObjectList
            'ElseIf arelationAttribute.ToEntries.Count > arelationAttribute.FromEntries.Count Then
            '    CoreMessageHandler(message:="relation attribute has nor mot ToEntries than FromEntries set", _
            '                        argument:=arelationAttribute.Name, objectname:=_dataobject.ObjectID, _
            '                         procedure:="DataObjectRelationMgr.GetRelatedObjects", messagetype:=otCoreMessageType.InternalError)
            '    Return theObjectList

            'End If

            'If Not aTargetType.GetInterfaces.Contains(GetType(iormRelationalPersistable)) And Not aTargetType.GetInterfaces.Contains(GetType(iormInfusable)) Then
            '    CoreMessageHandler(message:="target type has neither iormperistable nor iorminfusable interface", _
            '                       argument:=arelationAttribute.Name, objectname:=_dataobject.ObjectID, _
            '                        procedure:="DataObjectRelationMgr.GetRelatedObjects", messagetype:=otCoreMessageType.InternalError)
            '    Return theObjectList
            'End If
            ''***
            'Try

            '    theKeyvalues = Reflector.GetContainerEntryValues(dataobject:=_dataobject, entrynames:=arelationAttribute.FromEntries)
            '    Dim wherekey As String = String.Empty

            '    '** get a Store
            '    Dim aStore As iormRelationalTableStore = CType(dbdriver, iormRelationalDatabaseDriver).GetTableStore(ToTableID)
            '    Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(id:="allbyRelation" & arelationAttribute.Name, addAllFields:=True)
            '    If Not aCommand.IsPrepared Then
            '        ' build the key part
            '        For i = 0 To arelationAttribute.ToEntries.Count - 1
            '            If i > 0 Then wherekey &= " AND "
            '            '** if where is run against select of datatable the tablename is creating an error
            '            wherekey &= "[" & arelationAttribute.ToEntries(i) & "] = @" & arelationAttribute.ToEntries(i)
            '        Next
            '        aCommand.Where = wherekey
            '        If arelationAttribute.HasValueLinkJOin Then
            '            aCommand.Where &= " " & arelationAttribute.LinkJoin
            '        End If
            '        '** additional behavior
            '        If deletebehavior Then aCommand.Where &= " AND " & FNDeleted & " = @deleted "
            '        If domainBehavior Then aCommand.Where &= " AND ([" & FNDomainID & "] = @domainID OR [" & FNDomainID & "] = @globalID)"

            '        '** parameters
            '        For i = 0 To arelationAttribute.ToEntries.Count - 1
            '            aCommand.AddParameter(New ormSqlCommandParameter(ID:="@" & arelationAttribute.ToEntries(i), columnname:=arelationAttribute.ToEntries(i), _
            '                                                             tableid:=ToTableID))
            '        Next
            '        If deletebehavior Then aCommand.AddParameter(New ormSqlCommandParameter(ID:="@deleted", ColumnName:=FNDeleted, tableid:=ToTableID))
            '        If domainBehavior Then aCommand.AddParameter(New ormSqlCommandParameter(ID:="@domainID", ColumnName:=FNDomainID, tableid:=ToTableID))
            '        If domainBehavior Then aCommand.AddParameter(New ormSqlCommandParameter(ID:="@globalID", ColumnName:=FNDomainID, tableid:=ToTableID))
            '        aCommand.Prepare()
            '    End If
            '    '** parameters
            '    For i = 0 To arelationAttribute.ToEntries.Count - 1
            '        aCommand.SetParameterValue(ID:="@" & arelationAttribute.ToEntries(i), value:=theKeyvalues(i))
            '    Next
            '    '** set the values
            '    If aCommand.HasParameter(ID:="@deleted") Then aCommand.SetParameterValue(ID:="@deleted", value:=False)
            '    If aCommand.HasParameter(ID:="@domainID") Then aCommand.SetParameterValue(ID:="@domainID", value:=domainID)
            '    If aCommand.HasParameter(ID:="@globalID") Then aCommand.SetParameterValue(ID:="@globalID", value:=ConstGlobalDomain)




            'Catch ex As Exception
            '    CoreMessageHandler(exception:=ex, _
            '                        argument:=relationname, objectname:=_dataobject.ObjectID, _
            '                         procedure:="DataObjectRelationMgr.GetRelatedObjects")
            '    Return theObjectList
            'End Try




        End Function
        ''' <summary>
        ''' deletes related objects from a relation attribute for a object class described by a classdescriptor
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="dataobject"></param>
        ''' <param name="classdescriptor"></param>
        ''' <param name="dbdriver"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteRelatedObjects(relationname As String, _
                                             Optional dbdriver As iormRelationalDatabaseDriver = Nothing, _
                                             Optional timestamp As DateTime? = Nothing) As List(Of iormRelationalPersistable)
            If Not _isInitialized AndAlso Not Initialize() Then
                CoreMessageHandler(message:="could not initialize DataObjectRelationMgr", objectname:=_dataobject.ObjectID, _
                                   procedure:="DataObjectRelationMgr.DeleteRelatedObjects", messagetype:=otCoreMessageType.InternalError)
                Return New List(Of iormRelationalPersistable)
            End If

            Dim theKeyvalues As New List(Of Object)
            Dim theObjectList As New List(Of iormRelationalPersistable)
            If dbdriver Is Nothing Then dbdriver = _dataobject.ObjectPrimaryRelationalDatabaseDriver
            If dbdriver Is Nothing Then dbdriver = CurrentOTDBDriver
            Dim aRelationAttribute As ormRelationAttribute = _dataobject.ObjectClassDescription.GetRelationAttribute(relationname)
            Dim aTargetObjectDescriptor As ObjectClassDescription = ot.GetObjectClassDescription(aRelationAttribute.LinkObject)
            Dim aTargetType As System.Type = aTargetObjectDescriptor.Type

            Dim domainBehavior As Boolean
            Dim deletebehavior As Boolean
            Dim FNDomainID As String = Domain.ConstFNDomainID
            Dim FNDeleted As String = ConstFNIsDeleted
            Dim domainID As String = CurrentSession.CurrentDomainID
            Dim fromTablename As String = _dataobject.ObjectClassDescription.Tablenames.First
            Dim toTablename = aTargetObjectDescriptor.Tablenames.First ' First Tablename if multiple


            '** get the keys althoug determining if TOEntries are by Primarykey is a bit obsolete
            If Not aRelationAttribute.HasValueFromEntries OrElse Not aRelationAttribute.HasValueToEntries Then
                CoreMessageHandler(message:="relation attribute has nor fromEntries or ToEntries set", _
                                    argument:=aRelationAttribute.Name, objectname:=_dataobject.ObjectID, _
                                     procedure:="DataObjectRelationMgr.DeleteRelatedObjects", messagetype:=otCoreMessageType.InternalError)
                Return theObjectList
            ElseIf aRelationAttribute.ToEntries.Count > aRelationAttribute.FromEntries.Count Then
                CoreMessageHandler(message:="relation attribute has nor mot ToEntries than FromEntries set", _
                                    argument:=aRelationAttribute.Name, objectname:=_dataobject.ObjectID, _
                                     procedure:="DataObjectRelationMgr.DeleteRelatedObjects", messagetype:=otCoreMessageType.InternalError)
                Return theObjectList

            End If

            If Not aTargetType.GetInterfaces.Contains(GetType(iormRelationalPersistable)) And Not aTargetType.GetInterfaces.Contains(GetType(iormInfusable)) Then
                CoreMessageHandler(message:="target type has neither iormperistable nor iorminfusable interface", _
                                   argument:=aRelationAttribute.Name, objectname:=_dataobject.ObjectID, _
                                    procedure:="DataObjectRelationMgr.DeleteRelatedObjects", messagetype:=otCoreMessageType.InternalError)
                Return theObjectList
            End If
            '***
            Try
                '** return if we are bootstrapping
                'If CurrentSession.IsBootstrappingInstallationRequested Then
                '    CoreMessageHandler(message:="query for relations not possible during bootstrapping installation", _
                '                        arg1:=attribute.Name, objectname:=dataobject.ObjectID, _
                '                         subname:="Reflector.DeleteRelatedObjects", messagetype:=otCoreMessageType.InternalWarning)
                '    Return theObjectList

                '    '** avoid loops during startup
                'ElseIf CurrentSession.IsStartingUp AndAlso ot.GetBootStrapObjectClassIDs.Contains(aTargetObjectDescriptor.ID) Then
                '    Dim anObjectClassdDescription = ot.GetObjectClassDescriptionByID(id:=aTargetObjectDescriptor.ID)
                '    domainBehavior = anObjectClassdDescription.ObjectAttribute.AddDomainBehavior
                '    deletebehavior = anObjectClassdDescription.ObjectAttribute.AddDeleteFieldBehavior

                '    '** normal way
                'Else
                '    Dim anObjectDefinition As ObjectDefinition = ot.CurrentSession.Objects.GetObjectDefinition(id:=)aTargetObjectDescriptor.ID)
                '    domainBehavior = anObjectDefinition.HasDomainBehavior
                '    deletebehavior = anObjectDefinition.HasDeleteFieldBehavior
                'End If
                theKeyvalues = Reflector.GetContainerEntryValues(dataobject:=_dataobject, entrynames:=aRelationAttribute.FromEntries)
                Dim wherekey As String = String.Empty

                '** get a Store
                Dim aStore As iormRelationalTableStore = dbdriver.GetTableStore(toTablename)
                Dim aCommand As ormSqlCommand = aStore.CreateSqlCommand(id:="DeleteAllbyRelation_" & aRelationAttribute.Name)
                If Not aCommand.IsPrepared Then
                    aCommand.DatabaseDriver = dbdriver
                    Dim aSqlText = String.Format("DELETE FROM {0} WHERE ", toTablename)
                    aCommand.AddTable(toTablename) ' add it manually for recaching

                    ' build the key part
                    For i = 0 To aRelationAttribute.ToEntries.Count - 1
                        If i > 0 Then aSqlText &= " AND "
                        '** if where is run against select of datatable the tablename is creating an error
                        aSqlText &= "[" & aRelationAttribute.ToEntries(i) & "] = @" & aRelationAttribute.ToEntries(i)
                    Next

                    If aRelationAttribute.HasValueLinkJOin Then
                        aSqlText &= " " & aRelationAttribute.LinkJoin
                    End If
                    '** additional behavior
                    If timestamp.HasValue Then aSqlText &= " AND [" & ConstFNUpdatedOn & "] < @" & ConstFNUpdatedOn
                    'If deletebehavior Then aSqlText &= " AND " & FNDeleted & " = @deleted "
                    'If domainBehavior Then aSqlText &= " AND ([" & FNDomainID & "] = @domainID OR [" & FNDomainID & "] = @globalID)"

                    '** parameters
                    For i = 0 To aRelationAttribute.ToEntries.Count - 1
                        Dim anEntryAttribute As iormObjectEntryDefinition = _dataobject.ObjectClassDescription.GetObjectEntryAttribute(entryname:=aRelationAttribute.ToEntries(i))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@" & aRelationAttribute.ToEntries(i), datatype:=anEntryAttribute.Datatype, notColumn:=True))
                    Next
                    If timestamp.HasValue Then aCommand.AddParameter(New ormSqlCommandParameter(ID:="@" & ConstFNUpdatedOn, datatype:=otDataType.Timestamp, notColumn:=True))
                    'If deletebehavior Then aCommand.AddParameter(New ormSqlCommandParameter(ID:="@deleted", ColumnName:=FNDeleted, tablename:=toTablename))
                    'If domainBehavior Then aCommand.AddParameter(New ormSqlCommandParameter(ID:="@domainID", ColumnName:=FNDomainID, tablename:=toTablename))
                    'If domainBehavior Then aCommand.AddParameter(New ormSqlCommandParameter(ID:="@globalID", ColumnName:=FNDomainID, tablename:=toTablename))
                    aCommand.CustomerSqlStatement = aSqlText
                    aCommand.Prepare()
                End If
                '** parameters
                For i = 0 To aRelationAttribute.ToEntries.Count - 1
                    aCommand.SetParameterValue(ID:="@" & aRelationAttribute.ToEntries(i), value:=theKeyvalues(i))
                Next
                '** set the values
                If timestamp.HasValue Then aCommand.SetParameterValue(ID:="@" & ConstFNUpdatedOn, value:=timestamp)
                'If aCommand.HasParameter(ID:="@deleted") Then aCommand.SetParameterValue(ID:="@deleted", value:=False)
                'If aCommand.HasParameter(ID:="@domainID") Then aCommand.SetParameterValue(ID:="@domainID", value:=domainID)
                'If aCommand.HasParameter(ID:="@globalID") Then aCommand.SetParameterValue(ID:="@globalID", value:=ConstGlobalDomain)


                ' Infuse
                'If Not aCommand.Run() Then
                '   CoreMessageHandler(message:="command failed to run", subname:="DataObjectRelationMgr.DeleteRelatedObjects", messagetype:=otCoreMessageType.InternalError, _
                '                  arg1:=aCommand.SqlText)
                'End If

                'return finally
                Return theObjectList


            Catch ex As Exception
                CoreMessageHandler(exception:=ex, _
                                    argument:=aRelationAttribute.Name, objectname:=_dataobject.ObjectID, _
                                     procedure:="DataObjectRelationMgr.DeleteRelatedObjects")
                Return theObjectList
            End Try




        End Function
        ''' <summary>
        ''' retrieves a  related objects from a relation attribute for a object class described by a classdescriptor
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="classdescriptor"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetRelatedObjectByRetrieve(relationname As String) As iormDataObject
            Dim theKeyvalues As New List(Of Object)
            Dim keyentries As String()
            Dim runtimeOnly As Boolean = _dataobject.RunTimeOnly
            Dim aKey As ormDatabaseKey
            Dim aRelationAttribute As ormRelationAttribute = _dataobject.ObjectClassDescription.GetRelationAttribute(relationname:=relationname)
            If aRelationAttribute Is Nothing Then
                CoreMessageHandler(message:="relation was not found in class description", _
                                    argument:=relationname, objectname:=_dataobject.ObjectID, _
                                     procedure:="DataObjectRelationMgr.GetObjectByRetrieve", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If
            '** get the keys althoug determining if TOEntries are by Primarykey is a bit obsolete
            If aRelationAttribute.HasValueToPrimarykeys Then
                keyentries = aRelationAttribute.ToPrimaryKeys
            ElseIf Not aRelationAttribute.HasValueFromEntries And aRelationAttribute.HasValueToEntries Then
                keyentries = aRelationAttribute.ToEntries
            ElseIf aRelationAttribute.HasValueFromEntries Then
                keyentries = aRelationAttribute.FromEntries
            Else
                CoreMessageHandler(message:="relation attribute has nor fromEntries or ToEntries set", _
                                    argument:=aRelationAttribute.Name, objectname:=_dataobject.ObjectID, _
                                     procedure:="DataObjectRelationMgr.GetObjectByRetrieve", messagetype:=otCoreMessageType.InternalError)
                Return Nothing
            End If

            Try
                Dim aTargetObjectDescriptor As ObjectClassDescription = ot.GetObjectClassDescription(aRelationAttribute.LinkObject)
                theKeyvalues = Reflector.GetContainerEntryValues(dataobject:=_dataobject, entrynames:=keyentries)
                Dim aFactory As iormDataObjectProvider = ot.CurrentSession.DataObjectProvider(objectid:=aTargetObjectDescriptor.ObjectAttribute.ID)
                'Dim aDelegate As ObjectClassDescription.OperationCallerDelegate = aTargetObjectDescriptor.GetOperartionCallerDelegate(operationname:=anOperationAttribute.OperationName)

                If aFactory IsNot Nothing Then
                    Dim aPrimaryKey As ormDatabaseKey = New ormDatabaseKey(objectid:=aTargetObjectDescriptor.ObjectAttribute.ID, keyvalues:=theKeyvalues.ToArray)
                    Dim anobject As iormDataObject = aFactory.Retrieve(primarykey:=aPrimaryKey, type:=aTargetObjectDescriptor.Type, runtimeOnly:=runtimeOnly)
                    Return anobject
                Else
                    CoreMessageHandler(message:="No factory found in current session for object id '" & aTargetObjectDescriptor.ObjectAttribute.ID & "'", _
                                        messagetype:=otCoreMessageType.InternalError, procedure:="ormRelationalManager.GetRelatedObjectByRetrieve")
                    Return Nothing
                End If

                ''' OBSOLETE CODE
                ''' 
                'Dim anOperationAttribute As ormObjectOperationMethodAttribute = _
                '    aTargetObjectDescriptor.GetObjectOperationAttributeByTag(tag:=ObjectClassDescription.ConstMTRetrieve)

                'If anOperationAttribute IsNot Nothing Then
                '    '''
                '    ''' new code - fast
                '    ''' 
                '    theKeyvalues = Reflector.GetContainerEntryValues(dataobject:=_dataobject, entrynames:=keyentries)
                '    ' get a key by Primary Key
                '    aKey = New ormDatabaseKey(objectid:=aTargetObjectDescriptor.ObjectAttribute.ID, keyvalues:=theKeyvalues.ToArray)

                '    ''' if we have nothing we could not get all the values from the object
                '    ''' in some cases this is ok
                '    If theKeyvalues.Contains(Nothing) Then
                '        'CoreMessageHandler(message:="primary key contains nothing - could not retrieved from object", messagetype:=otCoreMessageType.InternalWarning, _
                '        '                    objectname:=_dataobject.ObjectID, subname:="DataObjectRelationMgr.GetObjectByRetrieve")
                '        Return Nothing
                '    End If
                '    Dim aDelegate As ObjectClassDescription.OperationCallerDelegate = aTargetObjectDescriptor.GetOperartionCallerDelegate(operationname:=anOperationAttribute.OperationName)
                '    '** relate also in the runtime !
                '    Dim anObject As iormRelationalPersistable = aDelegate(Nothing, {aKey, aTargetObjectDescriptor.Type, Nothing, Nothing, Nothing, runtimeOnly})
                '    Return anObject

                'Else
                '    '''
                '    ''' old code - slow
                '    ''' 

                '    Dim aTargetType As System.Type = aTargetObjectDescriptor.Type
                '    theKeyvalues = Reflector.GetContainerEntryValues(dataobject:=_dataobject, entrynames:=keyentries)
                '    aKey = New ormDatabaseKey(objectid:=aTargetObjectDescriptor.ObjectAttribute.ID, keyvalues:=theKeyvalues.ToArray)



                '    '** full primary key

                '    Dim retrieveMethod = ot.GetMethodInfo(aTargetType, ObjectClassDescription.ConstMTRetrieve)
                '    If retrieveMethod IsNot Nothing Then
                '        '** relate also in the runtime !
                '        Dim anObject As iormRelationalPersistable = retrieveMethod.Invoke(Nothing, {aKey, Nothing, Nothing, Nothing, runtimeOnly})
                '        Return anObject
                '    Else
                '        CoreMessageHandler(message:="the RETRIEVE method was not found on this object class", messagetype:=otCoreMessageType.InternalError, _
                '                            objectname:=aTargetType.Name, procedure:="DataObjectRelationMgr.GetObjectByRetrieve")
                '        Return Nothing
                '    End If
                'End If



                '*** return
                Return Nothing

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, _
                                    argument:=aRelationAttribute.Name, objectname:=_dataobject.ObjectID, _
                                     procedure:="DataObjectRelationMgr.GetRelatedObjectByRetrieve")
                Return Nothing
            End Try




        End Function

        ''' <summary>
        ''' Returns an enumerator of ormRelationAttributes that iterates through a collection.
        ''' </summary>
        ''' <returns>
        ''' An <see cref="T:System.Collections.IEnumerator" /> object that can be
        ''' used to iterate through the collection.
        ''' </returns>
        Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            If Not _isInitialized AndAlso Not Initialize() Then
                Throw New ormException(message:="could not initialize DataObjectRelationMgr", procedure:="DataObjectRelationMgr.GetEnumerator")
            End If
            Return _dataobject.ObjectClassDescription.RelationAttributes.GetEnumerator
        End Function

        ''' Returns an enumerator that iterates through the collection.
        ''' </summary>
        ''' <returns>
        ''' A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can
        ''' be used to iterate through the collection.
        ''' </returns>
        Public Function GetEnumerator1() As IEnumerator(Of ormRelationAttribute) Implements IEnumerable(Of ormRelationAttribute).GetEnumerator
            If Not _isInitialized AndAlso Not Initialize() Then
                Throw New ormException(message:="could not initialize DataObjectRelationMgr", procedure:="DataObjectRelationMgr.GetEnumerator1")
            End If
            Return _dataobject.ObjectClassDescription.RelationAttributes.GetEnumerator
        End Function
    End Class

End Namespace

