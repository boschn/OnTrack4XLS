Imports System.Reflection
Imports OnTrack.Commons
Imports OnTrack.Core

REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** ORM DATA OBJECT CLASSES - The Infusable
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2014-01-31
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************''' <summary>

Namespace OnTrack.Database

    ''' <summary>
    ''' Abstract class for infusable data objects 
    ''' </summary>
    ''' <remarks>
    ''' functional requirements:
    ''' 1. class is able to infuse all persistable members which are described in a classdescription by using the objectdefintion from a ormRecord
    ''' 2. loads also the relations described in the classdescription via a singleton named of type ormrelationmanager
    ''' 3. setting or getting values of object entries dynamically
    ''' 4. raises an validationNeeded event if setting values
    ''' 5. getting or setting values of compounds -> either by relation, by operation
    ''' 6. notifies by event if an entry value is changing
    ''' 7. requests default values by event
    ''' 8. flushes the mapped persistable entries out to a record
    ''' </remarks>

    Public MustInherit Class ormRelationalInfusable
        Inherits ormDataObject
        Implements iormInfusable

        ''' <summary>
        ''' LifeCycle Flags
        ''' </summary>
        ''' <remarks></remarks>
        Protected _isInfused As Boolean = False

        ''' <summary>
        ''' Timestamps
        ''' </summary>
        ''' <remarks></remarks>
        Protected _InfusionTimeStamp As DateTime

        ''' <summary>
        ''' flag for uniqueness check
        ''' </summary>
        ''' <remarks></remarks>
        Protected _UniquenessInStoreWasChecked As Boolean 'true if the check uniqueness function has run 

        ''' <summary>
        ''' relation Manager
        ''' </summary>
        ''' <remarks></remarks>
        Protected WithEvents _relationMgr As ormRelationManager  ' relation manager to manage to objects relations

        ''' <summary>
        ''' Events
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        ''' 
        ''' default values
        Public Event OnDefaultValueNeeded(sender As Object, e As ormDataObjectEntryEventArgs) Implements iormInfusable.OnDefaultValueNeeded
        ''' <summary>
        ''' static infusing
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Shared Event ClassOnInfusing(sender As Object, e As ormDataObjectEventArgs)
        Public Shared Event ClassOnInfused(sender As Object, e As ormDataObjectEventArgs)
        ''' <summary>
        ''' instance infusing
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Event OnInfusing(sender As Object, e As ormDataObjectEventArgs) Implements iormInfusable.OnInfusing
        Public Event OnInfused(sender As Object, e As ormDataObjectEventArgs) Implements iormInfusable.OnInfused
        ''' <summary>
        ''' column entries infused
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Event OnColumnsInfused(sender As Object, e As ormDataObjectEventArgs) Implements iormInfusable.OnColumnsInfused
        ''' <summary>
        ''' infuse the mapped columns
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Shared Event ClassOnColumnMappingInfusing(sender As Object, e As ormDataObjectEventArgs)
        Public Shared Event ClassOnColumnMappingInfused(sender As Object, e As ormDataObjectEventArgs)
        ''' <summary>
        ''' feeding out to a record
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Event OnFeeding(sender As Object, e As ormDataObjectEventArgs) Implements iormInfusable.OnFeeding
        Public Event OnFed(sender As Object, e As ormDataObjectEventArgs) Implements iormInfusable.OnFed
        Public Shared Event ClassOnFeeding(sender As Object, e As ormDataObjectEventArgs)
        Public Shared Event ClassOnFed(sender As Object, e As ormDataObjectEventArgs)

        ''' <summary>
        ''' entry value is changing events
        ''' </summary>
        ''' <remarks></remarks>
        Public Event OnEntryChanged As EventHandler(Of ormDataObjectEntryEventArgs) Implements iormInfusable.OnEntryChanged
        Public Event OnEntryChanging As EventHandler(Of ormDataObjectEntryEventArgs) Implements iormInfusable.OnEntryChanging

        ''' <summary>
        ''' validation requests
        ''' </summary>
        ''' <remarks></remarks>
        Public Event EntryValidationNeeded As EventHandler(Of ormDataObjectEntryValidationEventArgs) Implements iormInfusable.EntryValidationNeeded

        ''' <summary>
        ''' shared event for cascading the relation
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Shared Event ClassOnCascadingRelation(sender As Object, e As ormDataObjectEventArgs)
        Public Shared Event ClassOnCascadedRelation(sender As Object, e As ormDataObjectEventArgs)

        ''' <summary>
        ''' event on loading the relation from the database
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Event OnRelationLoading(sender As Object, e As ormDataObjectEventArgs)
        Public Event OnRelationLoad(sender As Object, e As ormDataObjectEventArgs)

        '* relation Events
        Protected Event OnRelationRetrieveNeeded(sender As Object, e As ormDataObjectRelationEventArgs)
        Protected Event OnRelationCreateNeeded(sender As Object, e As ormDataObjectRelationEventArgs)
        Protected Event OnRelationUpdateNeeded(sender As Object, e As ormDataObjectRelationEventArgs)
        Protected Event OnRelationDeleteNeeded(sender As Object, e As ormDataObjectRelationEventArgs)

#Region "Properties"
        ''' <summary>
        ''' Gets or sets the domain ID.
        ''' </summary>
        ''' <value>The domain ID.</value>
        Public Overridable Property DomainID() As String Implements iormDataObject.DomainID
            Get
                If Me.ObjectHasDomainBehavior Then
                    Return Me._domainID
                Else
                    Return CurrentSession.CurrentDomainID
                End If
            End Get
            Set(value As String)
                SetValue(ConstFNDomainID, value)
            End Set
        End Property
        ''' <summary>
        ''' returns True if the Object is infused
        ''' </summary>
        ''' <value>The PS is created.</value>
        Public ReadOnly Property IsInfused() As Boolean Implements iormInfusable.IsInfused
            Get
                Return _isInfused
            End Get
        End Property
        ''' <summary>
        ''' returns the default value for an Entry of this Object
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectEntryDefaultValue(entryname As String) As Object Implements iormInfusable.ObjectEntryDefaultValue
            Get
                If Me.ObjectDefinition Is Nothing Then
                    Dim anEntryAttribute As iormObjectEntryDefinition = Me.ObjectClassDescription.GetObjectEntryAttribute(entryname)
                    If anEntryAttribute Is Nothing Then Throw New ormException(message:="entry name '" & entryname & "' in object class description '" & Me.ObjectID & "' not found", procedure:="ormInfusable.ObjectEntryDefaultValue")

                    Dim args As ormDataObjectEntryEventArgs = New ormDataObjectEntryEventArgs(object:=Me, entryname:=entryname.ToUpper, value:=anEntryAttribute.DefaultValue)
                    RaiseEvent OnDefaultValueNeeded(Me, args)
                    If args.Result Then
                        Return args.Value
                    Else
                        Return anEntryAttribute.DefaultValue
                    End If
                Else
                    Dim anEntry As iormObjectEntryDefinition = Me.ObjectDefinition.GetEntryDefinition(entryname)
                    If anEntry Is Nothing Then Throw New ormException(message:="entry name '" & entryname & "' in object '" & Me.ObjectID & "' not found", procedure:="ormInfusable.ObjectEntryDefaultValue")

                    Dim args As ormDataObjectEntryEventArgs = New ormDataObjectEntryEventArgs(object:=Me, entryname:=entryname.ToUpper, value:=anEntry.DefaultValue)
                    RaiseEvent OnDefaultValueNeeded(Me, args)
                    If args.Result Then
                        Return args.Value
                    Else
                        Return anEntry.DefaultValue
                    End If
                End If
            End Get
        End Property
        ''' <summary>
        ''' returns the primary key
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property ObjectPrimaryKey As ormDatabaseKey Implements iormDataObject.ObjectPrimaryKey
            Get
                If _primarykey Is Nothing Then
                    _primarykey = New ormDatabaseKey(objectid:=Me.ObjectID)
                    For Each aEntryname In _primarykey.EntryNames
                        _primarykey(aEntryname) = Me.GetValue(aEntryname)
                    Next
                End If
                Return _primarykey
            End Get
        End Property
#End Region
        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub New(Optional runtimeonly As Boolean = False, Optional objectID As String = Nothing)
            MyBase.New(runtimeonly:=runtimeonly, objectID:=objectID)
            _relationMgr = New ormRelationManager(Me)
        End Sub
        ''' <summary>
        ''' clean up with the object
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Finialize()
            MyBase.Finalize()
            _relationMgr = Nothing
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
        Public Function TryGetMember(ByVal binder As System.Dynamic.GetMemberBinder, ByRef result As Object) As Boolean
            If Not Me.IsAlive(throwError:=False) Then Return False
            ' Converting the property name to lowercase
            ' so that property names become case-insensitive.
            Dim name As String = binder.Name

            ' If the property name is found in a dictionary,
            ' set the result parameter to the property value and return true.
            ' Otherwise, return false.

            If Me.ObjectDefinition.HasEntry(name) Then
                result = Me.GetValue(entryname:=name)
                Return True
            End If

            Return False
        End Function
        ''' <summary>
        ''' Dynamic setValue Property
        ''' </summary>
        ''' <param name="binder"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TrySetMember(ByVal binder As System.Dynamic.SetMemberBinder, ByVal value As Object) As Boolean
            If Not Me.IsAlive(throwError:=False) Then Return False

            If Not Me.ObjectDefinition.HasEntry(binder.Name) Then
                Return False
            End If

            Return False
            ' Converting the property name to lowercase
            ' so that property names become case-insensitive.
            Return Me.SetValue(entryname:=binder.Name, value:=value)
        End Function

        ''' <summary>
        ''' returns the value of an object entry of this object either a column entry or a compound
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="member"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GetValue(entryname As String) As Object Implements iormInfusable.GetValue

            Try

                Dim value As Object
                Dim isnullable As Boolean

                '''
                ''' check if object entry is a compound -> branch out
                ''' 
               
                If Me.ObjectDefinition.HasEntry(entryname) AndAlso Me.ObjectDefinition.GetEntryDefinition(entryname).IsActive Then
                    Dim anObjectEntry = Me.ObjectDefinition.GetEntryDefinition(entryname)
                    isnullable = anObjectEntry.IsNullable
                    '''
                    ''' branch out to retrieve compound value
                    ''' 
                    If anObjectEntry.IsCompound Then
                        Return Me.GetCompoundValue(entryname)
                    Else
                        ''' well it is a column although the class doesnot know it ? Might be not enabled
                        ''' 
                        CoreMessageHandler(message:="Object entry is a column entry although not described as one in the class description", argument:=entryname, _
                             objectname:=Me.ObjectID, entryname:=entryname, _
                              messagetype:=otCoreMessageType.InternalError, procedure:="ormInfusable.GetValue")
                        Return Nothing
                    End If

                Else
                    CoreMessageHandler(message:="Object entry does not exist in object definition or is not enabled", argument:=entryname, _
                              objectname:=Me.ObjectID, entryname:=entryname, _
                               messagetype:=otCoreMessageType.InternalError, procedure:="ormInfusable.GetValue")
                End If
               

                '''
                ''' retrieve the fieldinfos of the mapping
                ''' 
                Dim aClassDescription = Me.ObjectClassDescription ' ot.GetObjectClassDescription(Me.GetType)
                If aClassDescription Is Nothing Then
                    CoreMessageHandler(message:=" Object's Class Description could not be retrieved - object not defined ?!", argument:=value, _
                                      objectname:=Me.ObjectID, entryname:=entryname, _
                                       messagetype:=otCoreMessageType.InternalError, procedure:="ormInfusable.GetValue")
                    Return False
                End If

                Dim thefieldinfos = aClassDescription.GetEntryFieldInfos(entryname)
                If thefieldinfos.Count = 0 Then
                    CoreMessageHandler(message:="Warning ! ObjectEntry is not mapped to a class field member or the entry name is not valid", argument:=value, _
                                       objectname:=Me.ObjectID, entryname:=entryname, _
                                        messagetype:=otCoreMessageType.InternalError, procedure:="ormInfusable.GetValue")
                    Return Nothing
                End If

                '''
                ''' search values of the mapped fields
                ''' 
                For Each field In thefieldinfos

                    If Not Reflector.GetFieldValue(field:=field, dataobject:=Me, value:=value) Then
                        CoreMessageHandler(message:="field value ob data object couldnot be retrieved", _
                                            objectname:=Me.ObjectID, procedure:="ormInfusable.getValue", _
                                            messagetype:=otCoreMessageType.InternalError, entryname:=entryname)
                    End If

                Next

                '  the field was not found but the entry
                CoreMessageHandler(message:="Warning ! ObjectEntry is not mapped to class member", _
                                      objectname:=Me.ObjectID, entryname:=entryname, messagetype:=otCoreMessageType.InternalError, procedure:="ormInfusable.GetValue")
                Return value


            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ormInfusable.getvalue", argument:=entryname)
                Return Nothing
            End Try

        End Function
        ''' <summary>
        ''' returns the value of the compound entry name
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetCompoundValue(entryname As String) As Object
            Try
                Dim anObjectEntry = Me.ObjectDefinition.GetEntryDefinition(entryname)
                If Not anObjectEntry.IsCompound Then
                    CoreMessageHandler(message:="Object entry is a not a compound - use GetValue", argument:=entryname, _
                         objectname:=Me.ObjectID, entryname:=entryname, _
                          messagetype:=otCoreMessageType.InternalError, procedure:="ormInfusable.GetCompoundValue")
                    Return Nothing
                End If

                '''
                ''' 1. check if compound is connected with a getter ?!
                ''' 
                Dim aGetterName As String = TryCast(anObjectEntry, ormObjectCompoundEntry).CompoundGetterMethodName
                If aGetterName IsNot Nothing Then

                    ''' branch out to setter method
                    ''' 
                    Dim aOperationAttribute = Me.ObjectClassDescription.GetObjectOperationAttribute(name:=aGetterName)
                    If aOperationAttribute Is Nothing Then
                        CoreMessageHandler(message:="operation id not found in the class description repository", argument:=aGetterName, _
                                           messagetype:=otCoreMessageType.InternalError, objectname:=Me.ObjectID, _
                                           procedure:="DataObjetRelationMGr.GetRelatedObjectsFromOperation")
                        Return Nothing
                    End If

                    ''' check the data on the method to be called
                    ''' 

                    Dim aMethodInfo As MethodInfo = aOperationAttribute.MethodInfo
                    Dim aReturnType As System.Type = aMethodInfo.ReturnType
                    If Not aReturnType.Equals(GetType(Boolean)) Then
                        Call CoreMessageHandler(procedure:="ormInfusable.GetCompoundValue", messagetype:=otCoreMessageType.InternalError, _
                                      message:="getter operation must return a boolean value", _
                                      argument:=aGetterName, objectname:=Me.ObjectID, entryname:=entryname)
                    End If
                    Dim aDelegate As ObjectClassDescription.OperationCallerDelegate = Me.ObjectClassDescription.GetOperartionCallerDelegate(aGetterName)
                    Dim theParameterEntries As String() = aOperationAttribute.ParameterEntries
                    Dim theParameters As Object()
                    Dim returnValueIndex As Integer
                    Dim returnValue As Object ' dummy
                    ReDim theParameters(aMethodInfo.GetParameters.Count - 1)

                    ''' set the parameters for the delegate
                    For i = 0 To theParameters.GetUpperBound(0)
                        Dim j As Integer = aMethodInfo.GetParameters(i).Position
                        If j >= theParameterEntries.GetLowerBound(0) AndAlso j <= theParameterEntries.GetUpperBound(0) _
                            AndAlso theParameterEntries(j) IsNot Nothing Then

                            Select Case theParameterEntries(j)
                                Case ormObjectCompoundEntry.ConstFNEntryName
                                    theParameters(j) = entryname
                                Case ormObjectCompoundEntry.ConstFNValues
                                    theParameters(j) = returnValue
                                    returnValueIndex = j
                                Case Domain.ConstFNDomainID
                                    theParameters(j) = Me.DomainID
                            End Select

                        End If
                    Next

                    ''' call the Operation
                    ''' 
                    Dim result As Object = aDelegate(Me, theParameters)
                    If DirectCast(result, Boolean) = True Then
                        Return theParameters(returnValueIndex)
                    Else
                        Call CoreMessageHandler(procedure:="ormInfusable.GetCompoundValue", messagetype:=otCoreMessageType.InternalError, _
                                      message:="getter operation failed to return a  value", _
                                      argument:=aGetterName, objectname:=Me.ObjectID, entryname:=entryname)
                        Return Nothing
                    End If

                Else
                    '''
                    '''2.  get the relation path and resolve to object
                    ''' 
                    Dim aRelationPath As String() = TryCast(anObjectEntry, ormObjectCompoundEntry).CompoundRelationPath
                    Dim names = aRelationPath(0).Split("."c)
                    Dim aRelationname As String

                    If names.Count > 1 Then
                        aRelationname = names(1)
                    Else
                        aRelationname = names(0)
                    End If


                    ''' request a relation load
                    ''' 
                    If _relationMgr.Status(aRelationname) = ormRelationManager.RelationStatus.Unloaded Then
                        Me.InfuseRelation(aRelationname)
                    End If

                    ''' get the entry which is holding the needed data object
                    ''' 
                    Dim aFieldList As List(Of FieldInfo) = Me.ObjectClassDescription.GetMappedRelation2FieldInfos(relationName:=aRelationname)

                    ''' if last hop
                    ''' 
                    ''' have we reached the last hop ?
                    ''' 

                    Dim searchvalue As Object = Nothing ' by intension (all are selected if nothing)
                    Dim searchvalueentryname As String
                    Dim searchentryname As String
                    ''' if last hop
                    ''' 
                    ''' have we reached the last hop ?
                    ''' 
                    If aRelationPath.Count = 2 Then
                        searchvalue = entryname
                        searchentryname = TryCast(anObjectEntry, ormObjectCompoundEntry).CompoundIDEntryname
                        searchvalueentryname = TryCast(anObjectEntry, ormObjectCompoundEntry).CompoundValueEntryName
                    Else
                        searchvalueentryname = entryname
                        ' do not search anything -> get the objects returned to relation
                        searchentryname = Nothing
                        searchvalue = Nothing
                    End If
                    ''' get the reference data object selected by compoundID - and also load it
                    ''' 
                    Dim theReferenceObjects = _relationMgr.GetObjectsFromContainer(relationname:=aRelationname, entryname:=searchentryname, value:=searchvalue, _
                                                                                   loadRelationIfNotloaded:=True)

                    ''' request the value from there
                    ''' 
                    If theReferenceObjects.Count > 0 Then
                        ' prevent having no value
                        If searchvalueentryname Is Nothing Then searchvalueentryname = entryname
                        Return theReferenceObjects.First.GetValue(searchvalueentryname)
                    ElseIf _relationMgr.Status(aRelationname) = ormRelationManager.RelationStatus.Loaded Then
                        Return Nothing
                    Else
                        Call CoreMessageHandler(procedure:="ormInfusable.GetCompoundValue", messagetype:=otCoreMessageType.InternalError, _
                                       message:="relation couldnot be loaded - no value could be returned to compound", _
                                       argument:=aRelationname, objectname:=Me.ObjectID, entryname:=entryname)
                        Return Nothing
                    End If


                End If




            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ormInfusable.GetCompoundValue", objectname:=Me.ObjectID, argument:=Me.ObjectPrimaryKeyValues, entryname:=entryname)
                Return Nothing
            End Try
        End Function


        ''' <summary>
        ''' sets the value of the compound entry name
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function SetCompoundValue(entryname As String, value As Object) As Boolean
            Try
                Dim oldvalue As Object
                Dim anObjectEntry = Me.ObjectDefinition.GetEntryDefinition(entryname)
                If Not anObjectEntry.IsCompound Then
                    CoreMessageHandler(message:="Object entry is a not a compound - use SetValue", argument:=entryname, _
                         objectname:=Me.ObjectID, entryname:=entryname, _
                          messagetype:=otCoreMessageType.InternalError, procedure:="ormInfusable.SetCompoundValue")
                    Return False
                ElseIf anObjectEntry.IsReadonly Then
                    CoreMessageHandler(message:="Object entry is read-Only - set Value is forbidden", argument:=entryname, _
                         objectname:=Me.ObjectID, entryname:=entryname, _
                          messagetype:=otCoreMessageType.InternalError, procedure:="ormInfusable.SetCompoundValue")
                    Return False
                End If

                '''
                ''' 1. check if compound is connected with a setter ?!
                ''' 
                Dim aSetterName As String = TryCast(anObjectEntry, ormObjectCompoundEntry).CompoundSetterMethodName
                If aSetterName IsNot Nothing AndAlso aSetterName <> String.Empty Then

                    ''' 
                    ''' -> branch out to setter method
                    ''' 
                    Dim aOperationAttribute = Me.ObjectClassDescription.GetObjectOperationAttribute(name:=aSetterName)
                    If aOperationAttribute Is Nothing Then
                        CoreMessageHandler(message:="operation id not found in the class description repository", argument:=aSetterName, _
                                           messagetype:=otCoreMessageType.InternalError,
                                           objectname:=Me.ObjectID, _
                                           procedure:="DataObjetRelationMGr.SetRelatedObjectsFromOperation")
                        Return Nothing
                    End If

                    ''' check the data on the method to be called
                    ''' 

                    Dim aMethodInfo As MethodInfo = aOperationAttribute.MethodInfo
                    Dim aReturnType As System.Type = aMethodInfo.ReturnType
                    If Not aReturnType.Equals(GetType(Boolean)) Then
                        Call CoreMessageHandler(procedure:="ormInfusable.SetCompoundValue", messagetype:=otCoreMessageType.InternalError, _
                                      message:="Setter operation must return a boolean value", _
                                      argument:=aSetterName, objectname:=Me.ObjectID, entryname:=entryname)
                    End If
                    Dim aDelegate As ObjectClassDescription.OperationCallerDelegate = Me.ObjectClassDescription.GetOperartionCallerDelegate(aSetterName)
                    Dim theParameterEntries As String() = aOperationAttribute.ParameterEntries
                    Dim theParameters As Object()
                    ReDim theParameters(aMethodInfo.GetParameters.Count - 1)
                    ''' set the parameters for the delegate
                    For i = 0 To theParameters.GetUpperBound(0)
                        Dim j As Integer = aMethodInfo.GetParameters(i).Position
                        If j >= 0 AndAlso j <= theParameters.GetUpperBound(0) Then
                            Select Case theParameterEntries(j)
                                Case ormObjectCompoundEntry.ConstFNEntryName
                                    theParameters(j) = entryname
                                Case ormObjectCompoundEntry.ConstFNValues
                                    theParameters(j) = value
                                Case Domain.ConstFNDomainID
                                    theParameters(j) = Me.DomainID
                            End Select

                        End If
                    Next

                    ''' Raise the event
                    Dim args As ormDataObjectEntryEventArgs = New ormDataObjectEntryEventArgs(object:=Me, entryname:=entryname, value:=value)
                    RaiseEvent OnEntryChanging(Me, e:=args)
                    If args.Proceed Then
                        ''' call the Operation
                        ''' 
                        Dim result As Object = aDelegate(Me, theParameters)
                        If DirectCast(result, Boolean) = True Then

                            RaiseEvent OnEntryChanged(Me, e:=args)
                            Return args.Proceed
                        Else
                            Call CoreMessageHandler(procedure:="ormInfusable.SetCompoundValue", messagetype:=otCoreMessageType.InternalError, _
                                          message:="setter operation failed", argument:=aSetterName, objectname:=Me.ObjectID, entryname:=entryname)

                            Return Nothing
                        End If
                    Else
                        Return False
                    End If

                Else
                    '''
                    ''' 2. get the relation and travel along it
                    ''' 
                    Dim aRelationPath As String() = TryCast(anObjectEntry, ormObjectCompoundEntry).CompoundRelationPath
                    Dim names = aRelationPath(0).Split("."c)
                    Dim aRelationname As String
                    Dim lastHop As Boolean = False

                    If names.Count > 1 Then
                        aRelationname = names(1)
                    Else
                        aRelationname = names(0)
                    End If

                    ''' request a relation load
                    ''' 
                    If _relationMgr.Status(aRelationname) = ormRelationManager.RelationStatus.Unloaded Then
                        Me.InfuseRelation(aRelationname)
                    End If


                    ''' if last hop
                    ''' 
                    ''' have we reached the last hop ?
                    ''' 


                    Dim searchvalue As Object = Nothing ' by intension (all are selected if nothing)
                    Dim searchvalueentryname As String
                    Dim searchentryname As String
                    ''' if last hop
                    ''' 
                    ''' have we reached the last hop ?
                    ''' 
                    If aRelationPath.Count = 2 Then
                        searchvalue = entryname
                        searchentryname = TryCast(anObjectEntry, ormObjectCompoundEntry).CompoundIDEntryname
                        searchvalueentryname = TryCast(anObjectEntry, ormObjectCompoundEntry).CompoundValueEntryName
                    Else
                        searchvalueentryname = entryname
                        ' do not search anything -> get the objects returned to relation
                        searchentryname = Nothing
                        searchvalue = Nothing
                    End If

                    ''' get the reference data object selected by compoundID and load it 
                    ''' 
                    Dim theReferenceObjects = _relationMgr.GetObjectsFromContainer(relationname:=aRelationname, entryname:=searchentryname, value:=searchvalue, _
                                                                                  loadRelationIfNotloaded:=True)

                    ''' request the value from there
                    ''' 
                    If theReferenceObjects.Count > 0 Then
                        '' prevent having no value
                        If searchvalueentryname Is Nothing Then searchvalueentryname = entryname
                        Dim args As ormDataObjectEntryEventArgs = New ormDataObjectEntryEventArgs(object:=Me, entryname:=searchvalueentryname, value:=value)
                        RaiseEvent OnEntryChanging(Me, e:=args)
                        If args.Proceed Then
                            ''' recursion call to the setvalue of the next object (related one) to resolve the entry
                            ''' 
                            If theReferenceObjects.First.SetValue(searchvalueentryname, value) Then
                                RaiseEvent OnEntryChanged(Me, e:=args)
                                Return args.Proceed
                            End If
                        Else
                            Return False
                        End If

                    ElseIf _relationMgr.Status(aRelationname) = ormRelationManager.RelationStatus.Loaded Then
                        '''
                        ''' create the relation and reload
                        ''' 
                        If _relationMgr.CreateNInfuseRelations(mode:=otInfuseMode.None, relationnames:={aRelationname}.ToList) Then
                            theReferenceObjects = _relationMgr.GetObjectsFromContainer(relationname:=aRelationname, entryname:=searchentryname, value:=searchvalue, _
                                                                                       loadRelationIfNotloaded:=True)
                            ''' request the value from there
                            ''' 
                            If theReferenceObjects.Count > 0 Then

                                ''' recursion call to the setvalue of the next object (related one) to resolve the entry
                                ''' 

                                Dim args As ormDataObjectEntryEventArgs = New ormDataObjectEntryEventArgs(object:=Me, entryname:=searchvalueentryname, value:=value)
                                RaiseEvent OnEntryChanging(Me, e:=args)
                                If args.Proceed Then
                                    ''' recursion call to the setvalue of the next object (related one) to resolve the entry
                                    ''' 
                                    If theReferenceObjects.First.SetValue(searchvalueentryname, value) Then
                                        RaiseEvent OnEntryChanged(Me, e:=args)
                                        Return args.Proceed
                                    End If
                                Else
                                    Return False
                                End If
                            Else
                                Call CoreMessageHandler(procedure:="ormInfusable.SetCompoundValue", messagetype:=otCoreMessageType.InternalWarning, _
                                          message:="compound could not be set - related object create succeeded but retrieve failed ", _
                                          argument:=aRelationname, objectname:=Me.ObjectID, entryname:=entryname)
                                Return True
                            End If

                        Else
                            Call CoreMessageHandler(procedure:="ormInfusable.SetCompoundValue", messagetype:=otCoreMessageType.InternalError, _
                                           message:="compound could not be set - related object could not be created ", _
                                           argument:=aRelationname, objectname:=Me.ObjectID, entryname:=entryname)
                            Return True
                        End If

                    Else
                        Call CoreMessageHandler(procedure:="ormInfusable.SetCompoundValue", messagetype:=otCoreMessageType.InternalError, _
                                       message:="relation could not be loaded - no value could be returned to compound", _
                                       argument:=aRelationname, objectname:=Me.ObjectID, entryname:=entryname)
                        Return False
                    End If

                End If


            Catch ex As Exception


                CoreMessageHandler(exception:=ex, procedure:="ormInfusable.SetCompoundValue")
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' normalize a value and apply EntryProperties
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function NormalizeValue(entryname As String, ByRef value As Object) As Boolean Implements iormInfusable.NormalizeValue
            Dim result As Boolean = False
            Dim outvalue As Object
            Dim isnullable As Boolean = False
            Dim aDatatype As otDataType
            Dim anEntry As iormObjectEntryDefinition
            ''' 
            ''' APPLY THE ENTRY PROPERTIES AND TRANSFORM THE VALUE REQUESTED
            ''' 
            anEntry = Me.ObjectDefinition.GetEntryDefinition(entryname:=entryname)
            If anEntry Is Nothing Then
                CoreMessageHandler(message:="entryname not found in object class repository - value not checked", argument:=value, procedure:="ormInfusable.NormalizeValue", _
                                   objectname:=Me.ObjectID, entryname:=entryname, messagetype:=otCoreMessageType.ApplicationError)
                Return False
            Else
                aDatatype = anEntry.Datatype
            End If

            ''' set value to default value if nothing and not nullable 
            '''  
            If Not anEntry.IsNullable AndAlso value Is Nothing Then
                value = Me.ObjectEntryDefaultValue(entryname:=entryname)
                If value Is Nothing Then value = Core.DataType.GetDefaultValue(anEntry.Datatype)
            End If

            ''' use semy optimized way - object definition is cached / entry has to be looked up 
            '''  
            If Not EntryProperties.Apply(CType(Me.ObjectDefinition, ormDataObject), entryname:=entryname, [in]:=value, out:=outvalue) Then
                CoreMessageHandler(message:="applying object entry properties failed - value not checked", argument:=value, procedure:="ormInfusable.EqualsValue", _
                                   objectname:=Me.ObjectID, entryname:=entryname, messagetype:=otCoreMessageType.ApplicationError)
                Return False
            Else
                value = outvalue
            End If

            Return True
        End Function
        ''' <summary>
        ''' check if the entryname has the same value as supplied
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="member"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EqualsValue(entryname As String, ByVal value As Object) As Boolean Implements iormInfusable.EqualsValue
            Dim result As Boolean = False
            Dim outvalue As Object
            Dim isnullable As Boolean = False
            Dim aDatatype As otDataType

            ''' 
            ''' PHASE I : APPLY THE ENTRY PROPERTIES AND TRANSFORM THE VALUE REQUESTED
            ''' 
            If Not NormalizeValue(entryname, value) Then
                CoreMessageHandler(message:="Warning ! Could not normalize value", argument:=value, objectname:=Me.ObjectID, _
                                    entryname:=entryname, procedure:="ormInfusable.equalsValue")
            End If

            ''' get datatype
            ''' 
           
            Dim anEntry = Me.ObjectDefinition.GetEntryDefinition(entryname:=entryname)

            If anEntry Is Nothing Then
                CoreMessageHandler(message:="entryname not found in object class repository - value not checked", argument:=value, procedure:="ormInfusable.EqualsValue", _
                                   objectname:=Me.ObjectID, entryname:=entryname, messagetype:=otCoreMessageType.ApplicationError)
                Return False
            Else
                aDatatype = anEntry.Datatype
            End If



            '''
            ''' PHASE II: DO EUALIT CHECKING
            ''' 

            Try
                ''' get the existing value
                Dim anExistingValue As Object = Me.GetValue(entryname)
                Dim aConvertedvalue As Object

                ''' doe the checks
                If anExistingValue Is Nothing AndAlso value Is Nothing Then
                    Return True
                ElseIf anExistingValue Is Nothing AndAlso value IsNot Nothing Then
                    Return False
                ElseIf anExistingValue IsNot Nothing AndAlso value Is Nothing Then
                    Return False
                ElseIf anExistingValue IsNot Nothing AndAlso value IsNot Nothing Then

                    If anExistingValue.GetType.IsValueType AndAlso value.GetType.IsValueType Then
                        aConvertedvalue = Convert.ChangeType(value, DataType.GetTypeFor(aDatatype))
                        Return anExistingValue.Equals(aConvertedvalue)
                    ElseIf anExistingValue.GetType Is value.GetType Then
                        Return anExistingValue.Equals(value)
                        ''' special case
                    ElseIf value.GetType Is GetType(String) AndAlso anExistingValue.GetType.IsArray Then
                        Return Core.DataType.ToArray(value).SequenceEqual(anExistingValue)
                        'Return Array.Equals(aConvertedvalue, anExistingValue)
                    ElseIf value.GetType Is GetType(String) AndAlso anExistingValue.GetType.GetInterfaces.Contains(GetType(IList)) Then
                        Return Core.DataType.ToArray(value).ToList.SequenceEqual(anExistingValue)
                        'aConvertedvalue = Converter.String2Array(value).ToList
                        'Return anExistingValue.Equals(aConvertedvalue) ' list compare
                    ElseIf anExistingValue.GetType.IsEnum Then
                        If value.GetType.Equals(GetType(String)) Then
                            '* transform
                            aConvertedvalue = CTypeDynamic([Enum].Parse(anExistingValue.GetType, value, ignoreCase:=True), anExistingValue.GetType)
                        Else
                            aConvertedvalue = CTypeDynamic(value, anExistingValue.GetType)
                        End If
                        Return anExistingValue.Equals(aConvertedvalue)
                    Else
                        Throw New NotImplementedException("checking")
                        Return False
                    End If

                End If

                Return False
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ormInfusable.EqualsValue", argument:=value, entryname:=entryname, objectname:=Me.ObjectID)
                Return False
            End Try

        End Function
        ''' <summary>
        ''' applies object entry properties, validates and sets a value of a entry/member
        ''' the value might be changed during validation
        ''' raises the propertychanged event
        ''' if it is different to its value
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="member"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function SetValue(entryname As String, ByVal value As Object) As Boolean Implements iormInfusable.SetValue
            Dim result As Boolean = False
            Dim isnullable As Boolean = False
            Dim anObjectEntry As iormObjectEntryDefinition
            Dim oldvalue As Object

            ''' 
            ''' PHASE I : APPLY THE ENTRY PROPERTIES AND TRANSFORM THE VALUE REQUESTED
            ''' 
            If Not Me.NormalizeValue(entryname, value) Then
                CoreMessageHandler(message:="Warning ! Could not normalize value", argument:=value, objectname:=Me.ObjectID, _
                                    entryname:=entryname, procedure:="ormInfusable.SetValue")
            End If

            '''
            ''' PHASE II: DO VALIDATION
            ''' 

            Try
                ''' validate the new value -> raise event
                ''' 
                Dim aResult As New ormDataObjectEntryValidationEventArgs(Me, entryname:=entryname, value:=value, domainid:=DomainID)
                RaiseEvent EntryValidationNeeded(Me, aResult)
                Dim aValidateResult As otValidationResultType = aResult.ValidationResult

                '** Validate against the ObjectEntry Rules
                If aValidateResult = otValidationResultType.Succeeded Or aValidateResult = otValidationResultType.FailedButProceed Then

                   

                    ''' decide if compound or columnentry - on compound then branch out
                    ''' 
                    ''' 

                    If Me.ObjectDefinition.HasEntry(entryname) Then
                        anObjectEntry = Me.ObjectDefinition.GetEntryDefinition(entryname)

                        ''' check readonly
                        ''' 
                        If anObjectEntry.IsReadonly Then
                            Return True ' fake it
                        End If
                        '''
                        ''' branch out to set the compound value
                        ''' 
                        If anObjectEntry.IsCompound Then
                            Return SetCompoundValue(entryname, value)
                        Else
                            CoreMessageHandler(message:="Object entry does not exist in object class description as column entry but is also not a compound in the object definition ?!", argument:=entryname, _
                                               objectname:=Me.ObjectID, entryname:=entryname, _
                                               messagetype:=otCoreMessageType.InternalError, procedure:="ormInfusable.SetValue")
                            Return False

                        End If

                    Else
                        CoreMessageHandler(message:="Object entry does not exist in object definition", argument:=entryname, _
                                  objectname:=Me.ObjectID, entryname:=entryname, _
                                   messagetype:=otCoreMessageType.InternalError, procedure:="ormInfusable.SetValue")
                        Return False
                    End If


                    ''' get the fieldinfos of the entry
                    ''' 
                    ''' get the description
                    Dim aClassDescription = Me.ObjectClassDescription 'ot.GetObjectClassDescription(Me.GetType)
                    If aClassDescription Is Nothing Then
                        CoreMessageHandler(message:=" Object's Class Description could not be retrieved - object not defined ?!", argument:=value, _
                                         objectname:=Me.ObjectID, entryname:=entryname, _
                                           messagetype:=otCoreMessageType.InternalError, procedure:="ormInfusable.SetValue")
                        Return False
                    End If
                    Dim afieldinfos = aClassDescription.GetEntryFieldInfos(entryname)
                    If afieldinfos.Count = 0 Then
                        '    ' might be by intention
                        'CoreMessageHandler(message:="Warning ! ObjectEntry is not mapped to a class field member or the entry name is not valid", arg1:=value, _
                        '                   objectname:=Me.ObjectID, entryname:=entryname, _
                        '    '                    messagetype:=otCoreMessageType.InternalError, subname:="ormInfusable.SetValue")
                    End If


                    ''' take nullable
                    '''
                    isnullable = anObjectEntry.IsNullable

                    ''' get old values
                    ''' and set the new values if different
                    ''' 
                    For Each field In afieldinfos
                        oldvalue = Nothing
                        If Not Reflector.GetFieldValue(field:=field, dataobject:=Me, value:=oldvalue) Then
                            CoreMessageHandler(message:="field value of data object could not be retrieved by getvalue", _
                                                objectname:=Me.ObjectID, procedure:="ormInfusable.setValue", _
                                                messagetype:=otCoreMessageType.InternalError, entryname:=entryname)
                            Return False
                        End If

                        '*** if different value
                        If (oldvalue IsNot Nothing AndAlso value Is Nothing AndAlso isnullable) _
                            OrElse (oldvalue Is Nothing AndAlso value IsNot Nothing AndAlso isnullable) _
                            OrElse (value IsNot Nothing AndAlso Not value.Equals(oldvalue)) Then
                            '' raise event
                            Dim args As ormDataObjectEntryEventArgs = New ormDataObjectEntryEventArgs(object:=Me, entryname:=entryname, value:=value)
                            RaiseEvent OnEntryChanging(Me, e:=args)
                            If args.Proceed Then
                                'If args.Result Then value = args.Value possible but should not be done since validation 

                                '' reflector set
                                If Not Reflector.SetFieldValue(field:=field, dataobject:=Me, value:=value) Then
                                    CoreMessageHandler(message:="field value of data object could not be set", _
                                                        objectname:=Me.ObjectID, procedure:="ormInfusable.setValue", _
                                                        messagetype:=otCoreMessageType.InternalError, entryname:=entryname)
                                    Return False
                                End If
                            End If
                            result = args.Proceed
                        ElseIf (Not isnullable AndAlso value Is Nothing) Then
                            CoreMessageHandler(message:="field value is nothing although no nullable allowed", _
                                                    objectname:=Me.ObjectID, procedure:="ormInfusable.setValue", _
                                                    messagetype:=otCoreMessageType.InternalError, entryname:=entryname)
                            Return False
                        Else
                            Return True 'no difference no change but report everything is fine
                        End If

                    Next

                    ''' raise events
                    ''' 
                    If result Then
                        Me.IsChanged = True
                        Dim args As ormDataObjectEntryEventArgs = New ormDataObjectEntryEventArgs(object:=Me, entryname:=entryname, value:=value)
                        RaiseEvent OnEntryChanged(Me, e:=args)
                    End If

                    Return result
                End If

                Return False

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ormInfusable.setvalue", argument:=value, entryname:=entryname, objectname:=Me.ObjectID)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Feed the record belonging to the data object
        ''' </summary>
        ''' <returns>True if successful</returns>
        ''' <remarks></remarks>
        Public Function Feed(Optional record As ormRecord = Nothing) As Boolean Implements iormInfusable.Feed

            Dim classdescriptor As ObjectClassDescription = Me.ObjectClassDescription
            Dim result As Boolean = True

            '** defaultvalue
            If record Is Nothing Then record = Me.Record

            '** Fire Class Event
            Dim ourEventArgs As New ormDataObjectEventArgs(Me, record:=record, key:=Me.ObjectPrimaryKey, usecache:=Me.ObjectUsesCache)
            RaiseEvent ClassOnFeeding(Nothing, ourEventArgs)
            If ourEventArgs.AbortOperation Then
                Return ourEventArgs.Result
            Else
                record = ourEventArgs.Record
            End If
            '** Fire Event
            RaiseEvent OnFeeding(Me, ourEventArgs)
            If ourEventArgs.AbortOperation Then
                Return ourEventArgs.Result
            Else
                record = ourEventArgs.Record
            End If
            Try

                '*** feed each mapped column to record
                '*** if it is in the record

                For Each aColumnName In classdescriptor.MappedContainerEntryNames
                    Dim aFieldList As List(Of FieldInfo) = classdescriptor.GetMappedContainerEntry2FieldInfos(containerEntryName:=aColumnName)
                    For Each aField In aFieldList
                        Dim aMappedAttribute = classdescriptor.GetEntryMappingAttributes(aField.Name)
                        Dim anEntryAttribute = classdescriptor.GetObjectEntryAttribute(aMappedAttribute.EntryName)

                        Dim aValue As Object
                        ''' set the value to the record either if it is bound and the columnname is a member or if it is not bound
                        ''' 
                        If Not record.IsBound OrElse (record.IsBound AndAlso record.HasIndex(aColumnName)) Then
                            If aField.FieldType.IsValueType OrElse aField.FieldType.Equals(GetType(String)) OrElse aField.FieldType.Equals(GetType(Object)) OrElse _
                                aField.FieldType.IsArray OrElse aField.FieldType.GetInterfaces.Contains(GetType(IEnumerable)) Then
                                '** get the value by hook or slooow
                                If Not Reflector.GetFieldValue(field:=aField, dataobject:=Me, value:=aValue) Then
                                    aValue = aField.GetValue(Me)
                                End If

                                '** convert into List
                                If anEntryAttribute.Datatype = otDataType.List Then
                                    If aValue IsNot Nothing Then aValue = Core.DataType.ToString(aValue)

                                    '* 
                                ElseIf aField.FieldType.IsArray OrElse _
                                    (aField.FieldType.GetInterfaces.Contains(GetType(IEnumerable)) AndAlso Not aField.FieldType.Equals(GetType(String))) Then
                                    CoreMessageHandler(message:="field member is an array or list type but object entry attribute is not list - transfered to list presentation", objectname:=Me.ObjectID, containerEntryName:=aColumnName, _
                                                   argument:=aField.Name, entryname:=anEntryAttribute.EntryName, messagetype:=otCoreMessageType.InternalWarning, _
                                                   procedure:="ormInfusable.feedRecord")
                                    aValue = Core.DataType.ToString(aValue)
                                End If
                                '*** set the class internal field
                                record.SetValue(aColumnName, value:=aValue)
                                result = result And True
                            Else
                                CoreMessageHandler(message:="field member is not a value type", objectname:=Me.ObjectID, containerEntryName:=aColumnName, _
                                                    argument:=aField.Name, entryname:=anEntryAttribute.EntryName, messagetype:=otCoreMessageType.InternalError, _
                                                    procedure:="ormInfusable.feedRecord")
                                result = result And False
                            End If

                        End If

                    Next
                Next


                '** Fire Event
                ourEventArgs = New ormDataObjectEventArgs(Me, record:=record, key:=Me.ObjectPrimaryKey, _
                                                          usecache:=Me.ObjectUsesCache)

                ourEventArgs.Result = result
                RaiseEvent OnFed(Nothing, ourEventArgs)
                result = ourEventArgs.Result

                '** Fire Class Event
                ourEventArgs.Result = result
                RaiseEvent ClassOnFed(Nothing, ourEventArgs)
                Return ourEventArgs.Result

            Catch ex As Exception

                Call CoreMessageHandler(procedure:="ormInfusable.FeedRecord", exception:=ex, objectname:=Me.ObjectID)
                Return False

            End Try


        End Function
        ''' <summary>
        ''' feed the record from the field of an data object - use reflection of attribute otfieldname
        ''' </summary>
        ''' <param name="dataobject"></param>
        ''' <param name="record"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function FeedRecordDataObject(ByRef dataobject As iormRelationalPersistable, ByRef record As ormRecord) As Boolean
            Return dataobject.Feed(record:=record)
        End Function
        ''' <summary>
        ''' infuses a data object by a record
        ''' </summary>
        ''' <param name="Record">a fixed ormRecord with the persistence data</param>
        ''' <returns>true if successful</returns>
        ''' <remarks>might be overwritten by class descendants but make sure that you call mybase.infuse</remarks>
        Public Function Infuse(ByRef record As ormRecord, Optional mode? As otInfuseMode = Nothing) As Boolean Implements iormInfusable.Infuse

            '* lazy init
            If Not Me.IsInitialized AndAlso Not Me.Initialize() Then Return False

            Try
                Dim aPrimaryKey As ormDatabaseKey = record.ToPrimaryKey(objectID:=Me.ObjectID, runtimeOnly:=Me.RunTimeOnly)
                '** Fire Event
                Dim ourEventArgs As New ormDataObjectEventArgs(Me, record:=record, key:=aPrimaryKey, usecache:=Me.ObjectUsesCache, infusemode:=mode, _
                                                               runtimeOnly:=Me.RunTimeOnly)

                RaiseEvent OnInfusing(Me, ourEventArgs)
                If ourEventArgs.AbortOperation Then
                    Return ourEventArgs.Proceed
                Else
                    record = ourEventArgs.Record
                End If

                ''' merge the record according
                ''' Me.Record = record
                If _record Is Nothing Then
                    Me._record = record
                Else
                    Me._record.Merge(record)
                End If
                ''' if we have no load nor create state but are infused
                ''' 
                If Not Me.IsLoaded AndAlso Not Me.IsCreated AndAlso (record.IsCreated Or record.IsLoaded) Then
                    _isCreated = record.IsCreated
                    '* set loaded if record was loaded
                    If record.IsLoaded Then
                        'For Each aTableid In record.TableIDs
                        '    Me.Setloaded(aTableid)
                        'Next
                        _UniquenessInStoreWasChecked = True ' loaded is always uniqenuess checked
                    End If
                End If
                '** default mode value
                If Not mode.HasValue Then mode = otInfuseMode.OnDefault

                '*** INFUSE THE COLUMN MAPPED MEMBERS
                Dim aResult As Boolean = InfuseFieldMapping(mode:=mode)

                '*** Fire OnColumnsInfused
                ourEventArgs = New ormDataObjectEventArgs(Me, record:=record, key:=aPrimaryKey, infusemode:=mode, runtimeOnly:=Me.RunTimeOnly, usecache:=Me.ObjectUsesCache)
                RaiseEvent OnColumnsInfused(Me, ourEventArgs)
                If ourEventArgs.AbortOperation Then
                    Return ourEventArgs.Proceed
                End If

                '*** INFUSE THE RELATION MAPPED MEMBERS
                aResult = aResult And InfuseRelationMapped(mode:=mode)

                ''' return if result is false
                If Not aResult Then
                    Call CoreMessageHandler(message:="unable to infuse data object from record", procedure:="ormInfusable.Infuse", _
                                       objectname:=Me.ObjectID, messagetype:=otCoreMessageType.InternalError)
                    Return aResult
                End If

                '** Fire Event
                ourEventArgs = New ormDataObjectEventArgs(Me, record:=record, key:=aPrimaryKey, infusemode:=mode, runtimeOnly:=Me.RunTimeOnly, usecache:=Me.ObjectUsesCache)
                RaiseEvent OnInfused(Me, ourEventArgs)
                If ourEventArgs.AbortOperation Then
                    Return ourEventArgs.Proceed
                Else
                    If ourEventArgs.Result Then record = ourEventArgs.Record
                End If

                ''' final status
                ''' 

                ''' set the primary keys
                _primarykey = aPrimaryKey
                '** set infused status
                _isInfused = True
                _InfusionTimeStamp = DateTime.Now

                Return True

            Catch ex As Exception
                Call CoreMessageHandler(message:="Exception", exception:=ex, procedure:="ormInfusable.Infuse", _
                                        messagetype:=otCoreMessageType.InternalException)
                Return False
            End Try


        End Function
        ''' <summary>
        ''' infuse a data objects object entry column mapped members
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InfuseFieldMapping(mode As otInfuseMode) As Boolean
            '** Fire Event
            Dim ourEventArgs As New ormDataObjectEventArgs(Me, record:=Me.Record, infuseMode:=mode, runtimeOnly:=Me.RunTimeOnly)
            RaiseEvent ClassOnColumnMappingInfusing(Me, ourEventArgs)
            If ourEventArgs.AbortOperation Then
                Return ourEventArgs.Proceed
            ElseIf ourEventArgs.Result Then
                Me.Record = ourEventArgs.Record
            End If
            Dim objectentryname As String

            '*** infuse each mapped column to member
            '*** if it is in the record
            Try


                For Each aContainerEntryname In Me.ObjectClassDescription.MappedContainerEntryNames
                    Dim aFieldList As List(Of FieldInfo) = Me.ObjectClassDescription.GetMappedContainerEntry2FieldInfos(containerEntryName:=aContainerEntryname)

                    For Each aField In aFieldList
                        Dim aMappingAttribute = Me.ObjectClassDescription.GetEntryMappingAttributes(aField.Name)
                        If aMappingAttribute IsNot Nothing AndAlso (mode And aMappingAttribute.InfuseMode) Then
                            objectentryname = aMappingAttribute.EntryName
                            Dim isNull As Boolean
                            Dim aValue As Object

                            If Me.Record.HasIndex(aContainerEntryname) Then
                                'Dim aStopwatch1 As New Diagnostics.Stopwatch
                                'aStopwatch1.Start()
                                '*** set the class internal field
                                aValue = Me.Record.GetValue(aContainerEntryname, isNull:=isNull)

                                ''' check on Default Values on Object level
                                ''' on the OnCreate Infuse
                                If mode = otInfuseMode.OnCreate AndAlso (isNull OrElse aValue Is Nothing) Then
                                    ''' during bootstrapping installation we use just the value from class description
                                    ''' (doesnot matter if runtime or not in this case)
                                    'If CurrentSession.IsBootstrappingInstallationRequested Then

                                    '    ''' only if not nullable we use a default value
                                    '    If Not Me.ObjectClassDescription.GetObjectEntryAttribute(entryname:=objectentryname).IsNullable Then
                                    '        aValue = Me.ObjectClassDescription.GetObjectEntryAttribute(entryname:=objectentryname).DefaultValue
                                    '    End If
                                    'Else
                                    Dim anEntry As iormObjectEntryDefinition = Me.ObjectDefinition.GetEntryDefinition(entryname:=objectentryname)

                                    ''' only if not nullable we use a default value
                                    If anEntry IsNot Nothing Then
                                        aValue = Me.ObjectEntryDefaultValue(anEntry.Entryname)
                                        isNull = False 'reset for the value setting
                                    Else
                                        CoreMessageHandler(message:="object entry not found in object repository", _
                                                            objectname:=Me.ObjectID, procedure:="ormInfusable.InfuseColumnMapping", _
                                                            messagetype:=otCoreMessageType.InternalError, entryname:=objectentryname)

                                    End If
                                    'End If
                                End If

                                'aStopwatch1.Stop()
                                'Debug.WriteLine(">>>>>> GETVALUE:" & aStopwatch1.ElapsedTicks)
                                'Dim aStopwatch2 As New Diagnostics.Stopwatch
                                'aStopwatch2.Start()

                                ''' set the value
                                ''' 
                                If Not isNull AndAlso aValue IsNot Nothing Then
                                    If Not Reflector.SetFieldValue(field:=aField, dataobject:=Me, value:=aValue) Then
                                        CoreMessageHandler(message:="field value ob data object couldnot be set", _
                                                            objectname:=Me.ObjectID, procedure:="ormInfusable.InfuseColumnMapping", _
                                                            messagetype:=otCoreMessageType.InternalError, entryname:=objectentryname)
                                    End If

                                End If

                                'aStopwatch2.Stop()
                                'Debug.WriteLine(">>>>>> SETVALUE:" & aStopwatch2.ElapsedTicks)
                            End If
                        End If
                    Next
                Next


                '** Fire Event OnColumnMappingInfused
                ourEventArgs = New ormDataObjectEventArgs(Me, record:=Me.Record, infuseMode:=mode, runtimeOnly:=Me.RunTimeOnly)
                RaiseEvent ClassOnColumnMappingInfused(Me, ourEventArgs)
                Return ourEventArgs.Proceed

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="ormInfusable.InfuseColumnMapping", exception:=ex, objectname:=Me.ObjectID, _
                                        entryname:=objectentryname)
                Return False

            End Try

        End Function

        ''' <summary>
        ''' infuse a data objects objectentry column mapped members
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Shared Function InfusePrimaryKeys(ByRef dataobject As iormInfusable, _
                                                    ByRef pkarray As Object(), _
                                                  Optional runtimeOnly As Boolean = False) As Boolean
            Dim aList As List(Of String)
            Dim aDescriptor As ObjectClassDescription = dataobject.ObjectClassDescription
            Dim i As UShort = 0
            If aDescriptor Is Nothing Then
                CoreMessageHandler(message:="no object class description found", objectname:=dataobject.ObjectID, procedure:="ormInfusable.InfusePrimaryKeys", _
                                   messagetype:=otCoreMessageType.InternalError)
                Return False
            End If
            If Not runtimeOnly Then
                Dim atablestore As iormRelationalTableStore = ot.GetPrimaryTableStore(aDescriptor.Tablenames.First)
                aList = atablestore.ContainerSchema.PrimaryEntryNames 'take it from the real schema
            Else
                aList = aDescriptor.PrimaryKeyEntryNames.ToList
            End If

            '*** infuse each mapped column to member
            '*** if it is in the record
            Try
                SyncLock dataobject
                    For Each aColumnName In aList
                        Dim aFieldList As List(Of FieldInfo) = aDescriptor.GetMappedContainerEntry2FieldInfos(containerEntryName:=aColumnName)
                        For Each aField In aFieldList
                            Dim aValue As Object = pkarray(i)
                            Reflector.SetFieldValue(field:=aField, dataobject:=dataobject, value:=aValue)
                        Next
                    Next
                End SyncLock

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="ormInfusable.InfusePrimaryKeys", exception:=ex, objectname:=dataobject.ObjectID)
                Return False

            End Try

        End Function
        ''' <summary>
        ''' request to load the relations and infuses the values in the mapped members
        ''' </summary>
        ''' <param name="dataobject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InfuseRelation(id As String, Optional force As Boolean = False) As Boolean Implements iormInfusable.InfuseRelation
            If Not Me.IsInitialized Then
                If Not Me.Initialize Then
                    Return False
                End If
            End If

            Try
                If Not Me.IsAlive(subname:="InfuseRelation") Then Return False
                Dim result As Boolean = InfuseRelationMapped(mode:=otInfuseMode.OnDemand, relationid:=id, force:=force)
                Return result

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ormInfusable.infuseRelation")
                Return False
            End Try
        End Function
        ''' <summary>
        ''' infuse a data object by a record - use reflection and cache. Substitute data object if it is in cache
        ''' </summary>
        ''' <param name="dataobject"></param>
        ''' <param name="record"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function InfuseDataObject(ByRef record As ormRecord, ByRef dataobject As iormInfusable, _
                                                Optional mode? As otInfuseMode = otInfuseMode.OnDefault) As Boolean

            If dataobject Is Nothing Then
                CoreMessageHandler(message:="data object must not be nothing", procedure:="ormInfusable.InfuseDataObject", _
                                   messagetype:=otCoreMessageType.InternalError, _
                                    containerID:=record.ContainerIDS.First)
                Return False
            End If
            If record Is Nothing Then
                CoreMessageHandler(message:="record must not be nothing", procedure:="ormInfusable.InfuseDataObject", _
                                   messagetype:=otCoreMessageType.InternalError, _
                                    containerID:=record.ContainerIDS.First)
                Return False
            End If
            '** extract primary keys
            Dim aPrimaryKey As ormDatabaseKey = record.ToPrimaryKey(objectID:=dataobject.ObjectID, runtimeOnly:=dataobject.RuntimeOnly)
            '** Fire Event
            Dim ourEventArgs As New ormDataObjectEventArgs(dataobject, record:=record, key:=aPrimaryKey, usecache:=dataobject.ObjectUsesCache, infuseMode:=mode)
            RaiseEvent ClassOnInfusing(dataobject, ourEventArgs)
            If ourEventArgs.AbortOperation Then
                If ourEventArgs.Result Then
                    If ourEventArgs.DataObject IsNot Nothing Then dataobject = ourEventArgs.DataObject
                    Return True
                Else
                    Return False
                End If
            End If

            Dim aDescriptor As ObjectClassDescription = dataobject.ObjectClassDescription
            If aDescriptor Is Nothing Then
                CoreMessageHandler(message:="could not retrieve descriptor for business object class from core store", argument:=dataobject.GetType.Name, _
                                    messagetype:=otCoreMessageType.InternalError, procedure:="ormInfusable.createSchema")
                Return False
            End If

            '''
            ''' Infuse the instance
            If Not dataobject.Infuse(record:=record, mode:=mode) Then
                Return False
            End If

            '** Fire Event ClassOnInfused
            ourEventArgs = New ormDataObjectEventArgs(dataobject, record:=record, key:=aPrimaryKey, usecache:=dataobject.ObjectUsesCache, infuseMode:=mode)

            RaiseEvent ClassOnInfused(dataobject, ourEventArgs)
            If ourEventArgs.AbortOperation Then
                If ourEventArgs.Result Then
                    If ourEventArgs.DataObject IsNot Nothing Then dataobject = ourEventArgs.DataObject
                    Return True
                Else
                    Return False
                End If
            End If

            Return ourEventArgs.Proceed

        End Function

        ''' <summary>
        ''' Returns the Status of the Relation
        ''' </summary>
        ''' <param name="relationname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function GetRelationStatus(relationname As String) As ormRelationManager.RelationStatus
            Return _relationMgr.Status(relationname)
        End Function
        ''' <summary>
        ''' infuse the relation mapped Members of a dataobject for a certain mode and fire the events
        ''' </summary>
        ''' <param name="dataobject"></param>
        ''' <param name="classdescriptor"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InfuseRelationMapped(mode As otInfuseMode, Optional relationid As String = Nothing, Optional force As Boolean = False) As Boolean

            Dim anInfusedRelationList As List(Of String)
            ''' we have a relation
            If Not String.IsNullOrWhiteSpace(relationid) Then
                anInfusedRelationList = New List(Of String)
                anInfusedRelationList.Add(relationid)
            End If
            '* Fire Event OnRelationLoading
            Dim ourEventArgs As New ormDataObjectEventArgs(Me, Nothing, relationID:=anInfusedRelationList, infuseMode:=mode, runtimeOnly:=Me.RunTimeOnly)
            ourEventArgs.Proceed = True
            ourEventArgs.Result = True
            RaiseEvent ClassOnCascadingRelation(Me, ourEventArgs)
            If Not ourEventArgs.Proceed Then Return ourEventArgs.Result

            Try

                '*** Raise Event
                Me.RaiseOnRelationLoading(Me, ourEventArgs)
                If Not ourEventArgs.Proceed Then Return ourEventArgs.Result

                '''
                ''' call the relation manager to retrieve and infuse the relations - fille the infused relation list
                ''' 
                _relationMgr.LoadNInfuseRelations(mode:=mode, relationnames:=anInfusedRelationList, force:=force)


                '* Fire Event OnRelationLoading
                ourEventArgs = New ormDataObjectEventArgs(Me, Nothing, , relationID:=anInfusedRelationList, infuseMode:=mode, runtimeOnly:=Me.RunTimeOnly)
                '*** Raise Event
                Me.RaiseOnRelationLoaded(Me, ourEventArgs)
                If Not ourEventArgs.Proceed Then Return False

                '* Fire Event OnRelationLoading
                RaiseEvent ClassOnCascadedRelation(Me, ourEventArgs)
                Return ourEventArgs.Proceed

            Catch ex As Exception
                Call CoreMessageHandler(procedure:="ormInfusable.InfuseRelationMapped", exception:=ex, objectname:=Me.ObjectID)
                Return False

            End Try

        End Function
        ''' <summary>
        ''' cascade the update of relational data
        ''' </summary>
        ''' <param name="dataobject"></param>
        ''' <param name="classdescriptor"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function CascadeRelations(Optional cascadeUpdate As Boolean = False, _
                                          Optional cascadeDelete As Boolean = False, _
                                          Optional ByRef relationnames As List(Of String) = Nothing, _
                                          Optional timestamp As DateTime = constNullDate, _
                                          Optional uniquenesswaschecked As Boolean = True) As Boolean

            If timestamp = constNullDate Then timestamp = DateTime.Now

            '* Fire Event OnRelationLoading
            Dim ourEventArgs As New ormDataObjectEventArgs(Me, Nothing, relationID:=relationnames, timestamp:=timestamp)
            RaiseEvent ClassOnCascadingRelation(Me, ourEventArgs)
            If Not ourEventArgs.Proceed Then Return ourEventArgs.Proceed

            ''' cascade it to the relation manager
            If _relationMgr.CascadeRelations(cascadeUpdate:=cascadeUpdate, cascadeDelete:=cascadeDelete, _
                                           relationnames:=relationnames, timestamp:=timestamp, uniquenesswaschecked:=uniquenesswaschecked) Then



                '* Fire Event OnRelationLoaded
                ourEventArgs = New ormDataObjectEventArgs(Me, Nothing, , relationID:=relationnames)
                RaiseEvent ClassOnCascadedRelation(Me, ourEventArgs)
                Return ourEventArgs.Proceed
            Else
                Return False
            End If

        End Function
        ''' <summary>
        ''' Raise the Instance OnRelationLoading
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Protected Sub RaiseOnRelationLoading(sender As Object, e As ormDataObjectEventArgs)
            RaiseEvent OnRelationLoading(sender, e)
        End Sub
        ''' <summary>
        ''' Raise the Instance OnRelationLoaded
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Protected Sub RaiseOnRelationLoaded(sender As Object, e As ormDataObjectEventArgs)
            RaiseEvent OnRelationLoad(sender, e)
        End Sub
        ''' <summary>
        ''' cascade the OnRelationLoadNeeded from RelationManager
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>

        Protected Sub RaiseOnRelationLoadNeeded(sender As Object, e As ormRelationManager.EventArgs) Handles _relationMgr.OnRelatedObjectsRetrieveRequest
            Dim args As New ormDataObjectRelationEventArgs(e)
            RaiseEvent OnRelationRetrieveNeeded(sender, args)
        End Sub

        ''' <summary>
        ''' cascade the OnRelationLoadNeeded from RelationManager
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>

        Protected Sub RaiseOnRelationCreateNeeded(sender As Object, e As ormRelationManager.EventArgs) Handles _relationMgr.OnRelatedObjectsCreateRequest
            Dim args As New ormDataObjectRelationEventArgs(e)
            RaiseEvent OnRelationCreateNeeded(sender, args)
        End Sub

        '' <summary>
        ''' cascade the OnRelationLoadNeeded from RelationManager
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>

        Protected Sub RaiseOnRelationUpdateNeeded(sender As Object, e As ormRelationManager.EventArgs)
            Dim args As New ormDataObjectRelationEventArgs(e)
            RaiseEvent OnRelationUpdateNeeded(sender, args)
        End Sub

        '' <summary>
        ''' cascade the OnRelationLoadNeeded from RelationManager
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>

        Protected Sub RaiseOnRelationDeleteNeeded(sender As Object, e As ormRelationManager.EventArgs)
            Dim args As New ormDataObjectRelationEventArgs(e)
            RaiseEvent OnRelationDeleteNeeded(sender, args)
        End Sub

    End Class
End Namespace