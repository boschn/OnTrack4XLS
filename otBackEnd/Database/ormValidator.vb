
REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** ORM DATA OBJECT VALIDATOR CLASSES
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2014-01-31
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************''' <summary>

Imports System.Collections
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Text.RegularExpressions
Imports System.Reflection
Imports OnTrack.Core

Namespace OnTrack.Database
    ''' <summary>
    ''' ObjectEntry Validation Property Class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ObjectValidationProperty
        Inherits AbstractPropertyFunction(Of otObjectValidationProperty)
        Public Const Unique = "UNIQUE"
        Public Const NotEmpty = "NOTEMPTY"
        Public Const UseLookup = "USELOOKUP"
        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="propertystring"></param>
        ''' <remarks></remarks>
        Public Sub New(propertystring As String)
            MyBase.New(propertystring:=propertystring)
        End Sub
        ''' <summary>
        ''' Apply the Property function to a list
        ''' </summary>
        ''' <param name="in"></param>
        ''' <param name="out"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ApplyList(ByVal [in] As Object(), objectentrydefinition As iormObjectEntryDefinition, msglog As BusinessObjectMessageLog) As otValidationResultType
            Dim result As otValidationResultType = otValidationResultType.Succeeded
            If [in] Is Nothing Then Return True
            For i = 0 To [in].Count - 1
                Dim r As otValidationResultType = Me.Apply([in]:=[in](i), objectentrydefinition:=objectentrydefinition, msglog:=msglog)
                If result <= r Then
                    result = r
                End If
            Next
            Return result
        End Function
        ''' <summary>
        ''' Apply the Property function to a value
        ''' </summary>
        ''' <param name="in"></param>
        ''' <param name="out"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Apply(ByVal [in] As Object, objectentrydefinition As iormObjectEntryDefinition, msglog As BusinessObjectMessageLog) As otValidationResultType

            Try
                ''' check on empty arrays or list
                ''' 
                If _property = otObjectValidationProperty.NotEmpty Then
                    ''' if isNullable than Empty is not regarded on NOTHING / NULL
                    ''' 
                    If ([in] Is Nothing AndAlso objectentrydefinition.IsNullable) Then
                        Return otValidationResultType.Succeeded

                        ''' not allowed
                    ElseIf ([in] Is Nothing AndAlso Not objectentrydefinition.IsNullable) Then
                        '1102;@;VALIDATOR;object entry validation for '%1%.%2% (XID %3%) failed. Null or empty value is not allowed.;Provide a correct value;90;Error;false;|R1|R1|;|ENTRYVALIDATOR|XCHANGEENVELOPE|
                        msglog.Add(1102, Nothing, Nothing, Nothing, Nothing, Nothing, _
                              objectentrydefinition.Objectname, objectentrydefinition.Entryname, objectentrydefinition.XID)
                        Return otValidationResultType.FailedNoProceed

                        ''' here in is something -> if string not whitechar
                    ElseIf [in].GetType Is GetType(String) AndAlso Not String.IsNullOrWhiteSpace([in].ToString) Then
                        Return otValidationResultType.Succeeded

                    ElseIf [in].GetType Is GetType(String) AndAlso String.IsNullOrWhiteSpace([in].ToString) Then
                        '1102;@;VALIDATOR;object entry validation for '%1%.%2% (XID %3%) failed. Null or empty value is not allowed.;Provide a correct value;90;Error;false;|R1|R1|;|ENTRYVALIDATOR|XCHANGEENVELOPE|
                        msglog.Add(1102, Nothing, Nothing, Nothing, Nothing, Nothing, _
                              objectentrydefinition.Objectname, objectentrydefinition.Entryname, objectentrydefinition.XID)
                        Return otValidationResultType.FailedNoProceed

                        ''' not allowed
                        ''' 
                    ElseIf Not [in].GetType.IsArray AndAlso Not String.IsNullOrWhiteSpace([in].ToString) Then
                        Return otValidationResultType.Succeeded

                    ElseIf Not [in].GetType.IsArray AndAlso String.IsNullOrWhiteSpace([in].ToString) Then
                        '1102;@;VALIDATOR;object entry validation for '%1%.%2% (XID %3%) failed. Null or empty value is not allowed.;Provide a correct value;90;Error;false;|R1|R1|;|ENTRYVALIDATOR|XCHANGEENVELOPE|
                        msglog.Add(1102, Nothing, Nothing, Nothing, Nothing, Nothing, _
                              objectentrydefinition.Objectname, objectentrydefinition.Entryname, objectentrydefinition.XID)
                        Return otValidationResultType.FailedNoProceed

                    ElseIf Not objectentrydefinition.IsNullable AndAlso [in].GetType.IsArray AndAlso [in].length = 0 Then
                        '1102;@;VALIDATOR;object entry validation for '%1%.%2% (XID %3%) failed. Null or empty value is not allowed.;Provide a correct value;90;Error;false;|R1|R1|;|ENTRYVALIDATOR|XCHANGEENVELOPE|
                        msglog.Add(1102, Nothing, Nothing, Nothing, Nothing, Nothing, _
                              objectentrydefinition.Objectname, objectentrydefinition.Entryname, objectentrydefinition.XID)
                        Return otValidationResultType.FailedNoProceed

                    ElseIf objectentrydefinition.IsNullable AndAlso [in].GetType.IsArray AndAlso [in].length = 0 Then
                        Return otValidationResultType.Succeeded

                    ElseIf [in].GetType.IsArray AndAlso [in].length > 0 Then
                        Return ApplyList([in], objectentrydefinition:=objectentrydefinition, msglog:=msglog)

                    ElseIf Not [in].GetType Is GetType(String) AndAlso ([in].GetType Is GetType(List(Of ))) Then
                        If Not objectentrydefinition.IsNullable AndAlso TryCast([in], IList).Count = 0 Then
                            '1102;@;VALIDATOR;object entry validation for '%1%.%2% (XID %3%) failed. Null or empty value is not allowed.;Provide a correct value;90;Error;false;|R1|R1|;|ENTRYVALIDATOR|XCHANGEENVELOPE|
                            msglog.Add(1102, Nothing, Nothing, Nothing, Nothing, Nothing, _
                                  objectentrydefinition.Objectname, objectentrydefinition.Entryname, objectentrydefinition.XID)
                            Return otValidationResultType.FailedNoProceed
                        ElseIf objectentrydefinition.IsNullable AndAlso TryCast([in], IList).Count = 0 Then
                            Return otValidationResultType.Succeeded

                        Else
                            Return ApplyList([in].toArray, objectentrydefinition:=objectentrydefinition, msglog:=msglog)
                        End If

                    ElseIf Not [in].GetType Is GetType(String) AndAlso ([in].GetType.GetInterfaces.Contains(GetType(IEnumerable))) Then
                        Dim anArray As Object()
                        For Each anObject In TryCast([in], IEnumerable)
                            ReDim Preserve anArray(UBound(anArray) + 1)
                            anArray(UBound(anArray)) = anObject
                        Next
                        If UBound(anArray) > 0 Then
                            Return ApplyList(anArray, objectentrydefinition:=objectentrydefinition, msglog:=msglog)
                        Else
                            '1102;@;VALIDATOR;object entry validation for '%1%.%2% (XID %3%) failed. Null or empty value is not allowed.;Provide a correct value;90;Error;false;|R1|R1|;|ENTRYVALIDATOR|XCHANGEENVELOPE|
                            msglog.Add(1102, Nothing, Nothing, Nothing, Nothing, Nothing, _
                                  objectentrydefinition.Objectname, objectentrydefinition.Entryname, objectentrydefinition.XID)
                            Return otValidationResultType.FailedNoProceed
                        End If
                    End If
                End If

                '''
                ''' check if we are processing a list -> branch out
                ''' 

                If [in] Is Nothing OrElse [in].GetType.IsValueType OrElse [in].GetType Is GetType(String) Then
                    '' do nothing -> continue below

                ElseIf [in].GetType.IsArray AndAlso [in].length > 0 Then
                    ''' array
                    Return ApplyList([in], objectentrydefinition:=objectentrydefinition, msglog:=msglog)

                ElseIf Not [in].GetType Is GetType(String) AndAlso ([in].GetType Is GetType(List(Of ))) Then
                    ''' list
                    Return ApplyList([in].toArray, objectentrydefinition:=objectentrydefinition, msglog:=msglog)

                ElseIf Not [in].GetType Is GetType(String) AndAlso ([in].GetType.GetInterfaces.Contains(GetType(IEnumerable))) Then
                    ''' enumerable
                    Dim anArray As Object()
                    For Each anObject In TryCast([in], IEnumerable)
                        ReDim Preserve anArray(UBound(anArray) + 1)
                        anArray(UBound(anArray)) = anObject
                    Next
                    Return ApplyList(anArray, objectentrydefinition:=objectentrydefinition, msglog:=msglog)
                End If

                '''
                ''' check the properties
                ''' 
                Select Case _property
                    Case otObjectValidationProperty.Unique
                        Return True
                    Case otObjectValidationProperty.NotEmpty
                        ''' should be already checked above
                        '''
                        If (Not objectentrydefinition.IsNullable AndAlso [in] Is Nothing) OrElse ([in] IsNot Nothing AndAlso String.IsNullOrEmpty([in].ToString)) Then
                            '1102;@;VALIDATOR;object entry validation for '%1%.%2% (XID %3%) failed. Null or empty value is not allowed.;Provide a correct value;90;Error;false;|R1|R1|;|ENTRYVALIDATOR|XCHANGEENVELOPE|
                            msglog.Add(1102, Nothing, Nothing, Nothing, Nothing, Nothing, _
                                  objectentrydefinition.Objectname, objectentrydefinition.Entryname, objectentrydefinition.XID)
                            Return otValidationResultType.FailedNoProceed
                        End If
                    Case otObjectValidationProperty.UseLookup
                        If objectentrydefinition.LookupProperties.Count = 0 Then
                            Return otValidationResultType.Succeeded
                        End If

                        ''' nothing is allowed
                        If [in] Is Nothing And objectentrydefinition.IsNullable Then
                            Return otValidationResultType.Succeeded
                        End If

                        Dim aLookupList As String = String.Empty
                        ''' check all lookup properties
                        For Each aProperty In objectentrydefinition.LookupProperties
                            If aLookupList <> String.Empty Then aLookupList &= ","
                            aLookupList &= aProperty.ToString
                            If aProperty.Enum = otLookupProperty.UseAttributeValues Then
                                aLookupList &= " of [" & Core.DataType.ToString(objectentrydefinition.PossibleValues) & "]"
                            ElseIf aProperty.Enum = otLookupProperty.UseAttributeReference Then

                            End If
                            Dim aList As IList(Of Object) = aProperty.GetValues(objectentrydefinition)
                            If aList.Contains([in]) Then Return otValidationResultType.Succeeded
                        Next

                        'object entry validation for '%1%.%2% (XID %5%) failed. Value '%4%' is not found in lookup condition '%3%'
                        msglog.Add(1105, Nothing, Nothing, Nothing, Nothing, Nothing, _
                                   objectentrydefinition.Objectname, objectentrydefinition.Entryname, aLookupList, CStr([in]), _
                                   objectentrydefinition.XID)
                        Return otValidationResultType.FailedNoProceed


                    Case Else
                        CoreMessageHandler(message:="Property function is not implemented", argument:=_property.ToString, messagetype:=otCoreMessageType.InternalError, _
                                           procedure:="ObjectValidationProperty.Apply")
                        ''' return success
                        Return otValidationResultType.Succeeded
                End Select

                Return otValidationResultType.Succeeded
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectValidationProperty.Apply")
                Return otValidationResultType.Succeeded
            End Try

        End Function
        ''' <summary>
        ''' returns the enumeration value
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToEnum() As otObjectValidationProperty
            Return AbstractPropertyFunction(Of otObjectValidationProperty).ToEnum(_property)
        End Function
    End Class
    ''' <summary>
    ''' Enumeration of the validation properties
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otObjectValidationProperty
        <Description(ObjectValidationProperty.Unique)> Unique = 1
        <Description(ObjectValidationProperty.NotEmpty)> NotEmpty
        <Description(ObjectValidationProperty.UseLookup)> UseLookup
    End Enum


    ''' <summary>
    ''' type of validation results
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otValidationResultType
        Succeeded = 0
        SuccceededButRemark = 2
        WarningProceed = 4
        FailedButProceed = 6
        FailedNoProceed = 8
    End Enum

    ''' <summary>
    ''' Validation parts of the ormDataObject Class
    ''' </summary>
    ''' <remarks></remarks>

    Partial Public MustInherit Class ormBusinessObject

        ''' <summary>
        ''' Raise the Validating Event for this object
        ''' </summary>
        ''' <param name="msglog"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RaiseOnEntryValidatingEvent(entryname As String, msglog As BusinessObjectMessageLog) As otValidationResultType Implements iormValidatable.RaiseOnEntryValidatingEvent
            Dim args As ormDataObjectEntryValidationEventArgs = New ormDataObjectEntryValidationEventArgs(object:=Me, entryname:=entryname, msglog:=msglog, timestamp:=Date.Now)

            RaiseEvent OnEntryValidating(Me, args)
            Return args.Result
        End Function

        ''' <summary>
        ''' Raise the Validated Event for this object
        ''' </summary>
        ''' <param name="msglog"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RaiseOnEntryValidatedEvent(entryname As String, msglog As BusinessObjectMessageLog) As otValidationResultType Implements iormValidatable.RaiseOnEntryValidatedEvent
            Dim args As ormDataObjectEntryValidationEventArgs = New ormDataObjectEntryValidationEventArgs(object:=Me, entryname:=entryname, msglog:=msglog, timestamp:=Date.Now)

            RaiseEvent OnEntryValidated(Me, args)
            Return args.Result
        End Function
        ''' <summary>
        ''' Raise the Validating Event for this object
        ''' </summary>
        ''' <param name="msglog"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RaiseOnValidatingEvent(msglog As BusinessObjectMessageLog) As otValidationResultType Implements iormValidatable.RaiseOnValidatingEvent
            Dim args As ormDataObjectValidationEventArgs = New ormDataObjectValidationEventArgs(object:=Me, msglog:=msglog, timestamp:=Date.Now)

            RaiseEvent OnValidating(Me, args)
            Return args.Result
        End Function

        ''' <summary>
        ''' Raise the Validated Event for this object
        ''' </summary>
        ''' <param name="msglog"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RaiseOnValidatedEvent(msglog As BusinessObjectMessageLog) As otValidationResultType Implements iormValidatable.RaiseOnValidatedEvent
            Dim args As ormDataObjectValidationEventArgs = New ormDataObjectValidationEventArgs(object:=Me, msglog:=msglog, timestamp:=Date.Now)

            RaiseEvent OnValidated(Me, args)
            Return args.Result
        End Function
        ''' <summary>
        ''' validates the Business Object as total
        ''' </summary>
        ''' <remarks></remarks>
        ''' <returns>True if validated and OK</returns>
        Public Function Validate(Optional msglog As BusinessObjectMessageLog = Nothing) As otValidationResultType Implements iormValidatable.Validate
            If msglog Is Nothing Then msglog = Me.ObjectMessageLog
            Dim args As New ormDataObjectValidationEventArgs(object:=Me, timestamp:=Date.Now)
            Dim result As otValidationResultType
            '''
            ''' STEP 1 Raise the pre validate event
            ''' 
            RaiseEvent OnValidating(Me, args)
            If args.ValidationResult = otValidationResultType.FailedNoProceed Then Return args.ValidationResult

            ''' STEP 1a raise the event for all compound object relations if loaded
            ''' 
            If Not CurrentSession.IsInstallationRunning AndAlso Not CurrentSession.IsBootstrappingInstallationRequested Then
                args.ValidationResult = Me.RaiseValidatingCompound(msglog:=msglog)
                If args.ValidationResult = otValidationResultType.FailedNoProceed Then Return args.ValidationResult
            End If

            ''' 
            ''' Validate all the Entries against current value
            ''' 
            Dim aDescription As ObjectClassDescription = ot.GetObjectClassDescriptionByID(Me.ObjectID)
            For Each anEntryname In Me.ObjectDefinition.Entrynames
                If aDescription.MappedContainerEntryNames.Contains(anEntryname) Then
                    result = Me.Validate(entryname:=anEntryname, value:=GetValue(entryname:=anEntryname), msglog:=msglog)
                    If result = otValidationResultType.FailedNoProceed Then Return result
                End If
            Next

            ''' 
            ''' STEP 3 Raise the validated Event
            ''' 

            ''' STEP 3a raise the event for all compound object relations if loaded
            ''' 
            If Not CurrentSession.IsInstallationRunning AndAlso Not CurrentSession.IsBootstrappingInstallationRequested Then
                args.ValidationResult = Me.RaiseValidatedCompound(msglog:=msglog)
                If args.ValidationResult = otValidationResultType.FailedNoProceed Then Return args.ValidationResult
            End If


            ''' raise the validated event on this object
            ''' 
            RaiseEvent OnValidated(Me, args)
            If args.ValidationResult = otValidationResultType.FailedNoProceed Then Return args.ValidationResult

            Return args.ValidationResult
        End Function

        ''' <summary>
        ''' raises a validating event for the compound object
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function RaiseValidatingCompound(Optional msglog As BusinessObjectMessageLog = Nothing) As otValidationResultType
            Try
                '** do not allow during installation
                If CurrentSession.IsBootstrappingInstallationRequested OrElse CurrentSession.IsInstallationRunning Then
                    Return otValidationResultType.Succeeded
                End If

                Dim aRelationlist As New List(Of String)
                If msglog Is Nothing Then msglog = Me.ObjectMessageLog
                '''
                ''' Phase 1: get the loaded relations of the compound objects
                ''' 
                For Each anEntry As iormObjectEntryDefinition In CType(Me.ObjectDefinition, ormObjectDefinition).GetCompoundEntries
                    Dim anCompoundEntry As ormObjectCompoundEntry = TryCast(anEntry, ormObjectCompoundEntry)
                    Dim aRelationPath As String() = anCompoundEntry.CompoundRelationPath
                    Dim names = aRelationPath(0).Split("."c)
                    Dim aRelationname As String

                    If names.Count > 1 Then
                        aRelationname = names(1)
                    Else
                        aRelationname = names(0)
                    End If


                    ''' check on loaded only
                    ''' 
                    If _relationMgr.Contains(aRelationname) AndAlso _relationMgr.Status(aRelationname) = ormRelationManager.RelationStatus.Loaded Then
                        If Not aRelationlist.Contains(aRelationname) Then aRelationlist.Add(aRelationname)
                    End If
                Next

                '''
                ''' Phase 2: Get the Object and raise the Event and return
                ''' 

                For Each aRelationname In aRelationlist

                    ''' get the entry which is holding the needed data object
                    ''' 
                    Dim aFieldList As List(Of FieldInfo) = Me.ObjectClassDescription.GetMappedRelation2FieldInfos(relationName:=aRelationname)
                    Dim searchvalue As Object = Nothing ' by intension (all are selected if nothing)


                    ''' get the reference data object selected by compoundID - and also load it
                    ''' 
                    Dim theReferenceObjects = _relationMgr.GetObjectsFromContainer(relationname:=aRelationname, loadRelationIfNotloaded:=False)

                    ''' request the value from there
                    ''' 
                    If theReferenceObjects.Count > 0 Then
                        Dim aValidatable As iormValidatable = TryCast(theReferenceObjects.First, iormValidatable)
                        If aValidatable IsNot Nothing Then
                            Return aValidatable.RaiseOnValidatingEvent(msglog:=msglog)
                        Else
                            Return otValidationResultType.Succeeded
                        End If
                    ElseIf _relationMgr.Status(aRelationname) = ormRelationManager.RelationStatus.Loaded Then
                        ''' if loaded and nothing ?! -> will be a new object -> succeeded we cannot validate basically 
                        ''' 
                        ''' relation parameter createifnotretrieved should be used in these cases
                        ''' 
                        CoreMessageHandler(message:="compound object relation load return nothing - object will be created ", argument:=aRelationname, objectname:=Me.ObjectID, _
                                           messagetype:=otCoreMessageType.ApplicationWarning, procedure:="ormDataObject.RaiseValidatingCompound")
                        Return otValidationResultType.Succeeded
                    Else
                        ''' not loaded - couldnot load
                        ''' 
                        CoreMessageHandler(message:="compound object relation could not load", argument:=aRelationname, objectname:=Me.ObjectID, _
                                           messagetype:=otCoreMessageType.ApplicationError, procedure:="ormDataObject.RaiseValidatingCompound")
                        Return otValidationResultType.FailedNoProceed
                    End If


                Next


                Return otValidationResultType.Succeeded
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ormDataObject.RaiseValidatingCompound")
                Return otValidationResultType.FailedNoProceed
            End Try
        End Function
        ''' <summary>
        ''' raises a validated event for the compound object
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function RaiseValidatedCompound(Optional msglog As BusinessObjectMessageLog = Nothing) As otValidationResultType
            Try
                Dim aRelationlist As New List(Of String)

                If msglog Is Nothing Then msglog = Me.ObjectMessageLog

                '''
                ''' Phase 1: get the loaded relations of the compound objects
                ''' 
                For Each anEntry As iormObjectEntryDefinition In CType(Me.ObjectDefinition, ormObjectDefinition).GetCompoundEntries
                    Dim anCompoundEntry As ormObjectCompoundEntry = TryCast(anEntry, ormObjectCompoundEntry)
                    Dim aRelationPath As String() = anCompoundEntry.CompoundRelationPath
                    Dim names = aRelationPath(0).Split("."c)
                    Dim aRelationname As String

                    If names.Count > 1 Then
                        aRelationname = names(1)
                    Else
                        aRelationname = names(0)
                    End If


                    ''' check on loaded only
                    ''' 
                    If _relationMgr.Contains(aRelationname) AndAlso _relationMgr.Status(aRelationname) = ormRelationManager.RelationStatus.Loaded Then
                        If Not aRelationlist.Contains(aRelationname) Then aRelationlist.Add(aRelationname)
                    End If
                Next

                '''
                ''' Phase 2: Get the Object and raise the Event and return
                ''' 

                For Each aRelationname In aRelationlist

                    ''' get the entry which is holding the needed data object
                    ''' 
                    Dim aFieldList As List(Of FieldInfo) = Me.ObjectClassDescription.GetMappedRelation2FieldInfos(relationName:=aRelationname)
                    Dim searchvalue As Object = Nothing ' by intension (all are selected if nothing)


                    ''' get the reference data object selected by compoundID - and also load it
                    ''' 
                    Dim theReferenceObjects = _relationMgr.GetObjectsFromContainer(relationname:=aRelationname, loadRelationIfNotloaded:=False)

                    ''' request the value from there
                    ''' 
                    If theReferenceObjects.Count > 0 Then
                        Dim aValidatable As iormValidatable = TryCast(theReferenceObjects.First, iormValidatable)
                        If aValidatable IsNot Nothing Then
                            Return aValidatable.RaiseOnValidatedEvent(msglog:=msglog)
                        Else
                            Return otValidationResultType.Succeeded
                        End If
                    ElseIf _relationMgr.Status(aRelationname) = ormRelationManager.RelationStatus.Loaded Then
                        ''' if loaded and nothing ?! -> will be a new object -> succeeded we cannot validate basically 
                        ''' 
                        ''' relation parameter createifnotretrieved should be used in these cases
                        ''' 
                        CoreMessageHandler(message:="compound object relation load return nothing - object will be created ", argument:=aRelationname, objectname:=Me.ObjectID, _
                                           messagetype:=otCoreMessageType.ApplicationWarning, procedure:="ormDataObject.RaiseValidatedCompound")
                        Return otValidationResultType.Succeeded
                    Else
                        ''' not loaded - couldnot load
                        ''' 
                        CoreMessageHandler(message:="compound object relation could not load", argument:=aRelationname, objectname:=Me.ObjectID, _
                                           messagetype:=otCoreMessageType.ApplicationError, procedure:="ormDataObject.RaiseValidatedCompound")
                        Return otValidationResultType.FailedNoProceed
                    End If


                Next


            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ormDataObject.RaiseValidatedCompound")
                Return Nothing
            End Try
        End Function
        ''' <summary>
        ''' Validates a Compound
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ValidateCompoundValue(entryname As String, value As Object, Optional msglog As BusinessObjectMessageLog = Nothing) As otValidationResultType
            Try



                Dim anObjectEntry = Me.ObjectDefinition.GetEntryDefinition(entryname)
                If Not anObjectEntry.IsCompound Then
                    CoreMessageHandler(message:="Object entry is a not a compound - use Validate", argument:=entryname, _
                         objectname:=Me.ObjectID, entryname:=entryname, _
                          messagetype:=otCoreMessageType.InternalError, procedure:="ormDataObject.ValidateCompoundValue")
                    Return Nothing
                End If

                '''
                ''' 1. check if compound is connected with a getter ?!
                ''' 
                Dim aValidatorName As String = TryCast(anObjectEntry, ormObjectCompoundEntry).CompoundValidatorMethodName
                If aValidatorName IsNot Nothing Then

                    ''' branch out to setter method
                    ''' 
                    Dim aOperationAttribute = Me.ObjectClassDescription.GetObjectOperationAttribute(name:=aValidatorName)
                    If aOperationAttribute Is Nothing Then
                        CoreMessageHandler(message:="operation id not found in the class description repository", argument:=aValidatorName, _
                                           messagetype:=otCoreMessageType.InternalError, objectname:=Me.ObjectID, _
                                           procedure:="DataObjetRelationMGr.ValidateCompoundValue")
                        Return Nothing
                    End If

                    ''' check the data on the method to be called
                    ''' 

                    Dim aMethodInfo As MethodInfo = aOperationAttribute.MethodInfo
                    Dim aReturnType As System.Type = aMethodInfo.ReturnType
                    If Not aReturnType.Equals(GetType(otValidationResultType)) Then
                        Call CoreMessageHandler(procedure:="ormDataObject.ValidateCompoundValue", messagetype:=otCoreMessageType.InternalError, _
                                      message:="validator operation must return a otValidationResultType value", _
                                      argument:=aValidatorName, objectname:=Me.ObjectID, entryname:=entryname)
                    End If
                    Dim aDelegate As ObjectClassDescription.OperationCallerDelegate = Me.ObjectClassDescription.GetOperartionCallerDelegate(aValidatorName)
                    Dim theParameterEntries As String() = aOperationAttribute.ParameterEntries
                    Dim theParameters As Object()
                    Dim returnValueIndex As Integer
                    Dim returnValue As Object ' dummy
                    ReDim theParameters(aMethodInfo.GetParameters.Count - 1)
                    ''' set the parameters for the delegate
                    For i = 0 To theParameters.GetUpperBound(0)
                        Dim j As Integer = aMethodInfo.GetParameters(i).Position
                        If j >= 0 AndAlso j <= theParameters.GetUpperBound(0) Then
                            Select Case theParameterEntries(j)
                                Case ormObjectCompoundEntry.ConstFNEntryName
                                    theParameters(j) = entryname
                                Case ormObjectCompoundEntry.ConstFNValues
                                    theParameters(j) = returnValue
                                    returnValueIndex = j
                            End Select

                        End If
                    Next

                    ''' call the Operation
                    ''' 
                    Dim result As Object = aDelegate(Me, theParameters)
                    If result IsNot Nothing Then
                        Return result
                    Else
                        Call CoreMessageHandler(procedure:="ormDataObject.ValidateCompoundValue", messagetype:=otCoreMessageType.InternalError, _
                                      message:="getter operation failed to return a  value", _
                                      argument:=aValidatorName, objectname:=Me.ObjectID, entryname:=entryname)
                        Return Nothing
                    End If

                Else

                    '''
                    ''' 1. Validate the compound in the current
                    ''' 

                    Dim result As otValidationResultType = ObjectValidator.Validate(newvalue:=value, objectentrydefinition:=anObjectEntry, msglog:=msglog)

                    ''' return here if not alive (pre-create validate)
                    If Not Me.IsAlive("ValidateCompoundValue", throwError:=False) Then
                        Return result
                    End If
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

                    ''' return also if the relationname is the ObjectID -> means last item
                    If aRelationname = Me.ObjectID Then Return result

                    ''' request a relation load -> only if alive
                    ''' 
                    If _relationMgr.Status(aRelationname) = ormRelationManager.RelationStatus.Unloaded Then
                        Me.InfuseRelation(aRelationname)
                    End If

                    ''' get the entry which is holding the needed data object
                    ''' 
                    Dim aFieldList As List(Of FieldInfo) = Me.ObjectClassDescription.GetMappedRelation2FieldInfos(relationName:=aRelationname)
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
                    Dim theReferenceObjects As New List(Of iormRelationalPersistable)


                    theReferenceObjects = _relationMgr.GetObjectsFromContainer(relationname:=aRelationname, entryname:=searchentryname, value:=searchvalue, _
                                                                                                       loadRelationIfNotloaded:=True)

                    ''' request the value from there
                    ''' 
                    If theReferenceObjects.Count > 0 Then
                        ''' prevent having no value
                        If searchvalueentryname Is Nothing Then searchvalueentryname = entryname
                        Dim aValidatable As iormValidatable = TryCast(theReferenceObjects.First, iormValidatable)
                        If aValidatable IsNot Nothing Then
                            Return aValidatable.Validate(entryname:=searchvalueentryname, value:=value, msglog:=msglog)
                        Else
                            Return otValidationResultType.Succeeded
                        End If
                    ElseIf _relationMgr.Status(aRelationname) = ormRelationManager.RelationStatus.Loaded Then
                        ''' if loaded and nothing ?! -> will be a new object -> succeeded we cannot validate basically 
                        ''' 
                        ''' relation parameter createifnotretrieved should be used in these cases
                        ''' 
                        CoreMessageHandler(message:="compound object relation load return nothing - object will be created ", entryname:=entryname, argument:=aRelationname, objectname:=Me.ObjectID, _
                                           messagetype:=otCoreMessageType.ApplicationWarning, procedure:="ormDataObject.ValidateCompoundValue")
                        Return otValidationResultType.Succeeded
                    Else
                        ''' not loaded - couldnot load
                        ''' 
                        CoreMessageHandler(message:="compound object relation could not load", entryname:=entryname, argument:=aRelationname, objectname:=Me.ObjectID, _
                                           messagetype:=otCoreMessageType.ApplicationError, procedure:="ormDataObject.ValidateCompoundValue")
                        Return otValidationResultType.FailedNoProceed
                    End If


                End If


            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ormDataObject.ValidateCompoundValue")
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' validates a named object entry of the object
        ''' </summary>
        ''' <param name="enryname"></param>
        ''' <remarks></remarks>
        ''' <returns></returns>
        Public Function Validate(entryname As String, ByVal value As Object, Optional msglog As BusinessObjectMessageLog = Nothing) As otValidationResultType Implements iormValidatable.Validate
            Dim result As otValidationResultType

            ''' how to validate during bootstrapping or session starting
            If CurrentSession.IsBootstrappingInstallationRequested OrElse CurrentSession.IsStartingUp Then
                '' while doing it different
                result = otValidationResultType.Succeeded
            Else
                If msglog Is Nothing Then msglog = Me.ObjectMessageLog
                Dim anObjectEntry As iormObjectEntryDefinition = Me.ObjectDefinition.GetEntryDefinition(entryname:=entryname)

                ''' 3 Step Validation process
                ''' 

                Dim args As New ormDataObjectEntryValidationEventArgs(object:=Me, entryname:=entryname, value:=value, msglog:=msglog, timestamp:=Date.Now)

                '''
                ''' STEP 1 RAISE THE VALIDATING ENTRY EVENT BEFORE WE PROCESS
                '''
                RaiseEvent OnEntryValidating(Me, args)
                If args.ValidationResult = otValidationResultType.FailedNoProceed Then Return args.ValidationResult
                If args.Result Then value = args.Value

                '''
                '''  STEP 2 Validate the entry against INTERNAL RULES
                ''' 

                result = ObjectValidator.Validate(Me.ObjectDefinition.GetEntryDefinition(entryname), newvalue:=value, msglog:=msglog)
                If result = otValidationResultType.FailedNoProceed Then Return result

                ''' STEP 3 VALIDATE VIA ENTRY VALIDATED EVENT (Post Validating)
                ''' 
                RaiseEvent OnEntryValidated(Me, args)
                result = args.ValidationResult

                ''' 
                ''' check if we are validating a compound
                ''' 
                If anObjectEntry.IsCompound Then
                    '''
                    ''' branch out to validateCompoung
                    ''' 
                    result = Me.ValidateCompoundValue(entryname:=entryname, value:=value, msglog:=msglog)
                End If

                Return result
            End If
            Return result
        End Function

    End Class


    ''' <summary>
    ''' Class for Object (Entry) Validation
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ObjectValidator

        ''' <summary>
        ''' Event Argument Class
        ''' </summary>
        ''' <remarks></remarks>
        Public Class EventArgs
            Inherits System.EventArgs

        End Class

        Private Shared _validate As otValidationResultType


        ''' <summary>
        ''' Events
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Event OnValidationEntyFailed(sender As Object, e As ObjectValidator.EventArgs)


        ''' <summary>
        ''' validate an individual entry (contextfree)
        ''' </summary>
        ''' <param name="objectentrydefinition"></param>
        ''' <param name="newvalue"></param>
        ''' <param name="oldvalue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Validate(objectentrydefinition As iormObjectEntryDefinition, ByVal newvalue As Object, _
                                             Optional ByRef msglog As BusinessObjectMessageLog = Nothing) As otValidationResultType

            Dim result As otValidationResultType = otValidationResultType.Succeeded

            If objectentrydefinition Is Nothing Then
                CoreMessageHandler(message:="object entry definition is nothing - validate aborted", messagetype:=otCoreMessageType.InternalError, _
                                   procedure:="ObjectValidator.ValidateEntry")
                Return otValidationResultType.FailedNoProceed
            End If
            Try

                'If msglog Is Nothing Then msglog = New ObjectMessageLog()

                '''
                ''' 1. Datatype : try to convert
                ''' 
                Dim failedflag As Boolean = Not Core.DataType.Is(newvalue, datatype:=objectentrydefinition.Datatype)

                If failedflag And msglog IsNot Nothing Then
                    If newvalue IsNot Nothing Then
                        msglog.Add(1101, Nothing, Nothing, Nothing, Nothing, Nothing, _
                                   objectentrydefinition.Objectname, objectentrydefinition.Entryname, objectentrydefinition.Datatype.ToString, newvalue, objectentrydefinition.XID)
                    ElseIf Not objectentrydefinition.IsNullable AndAlso newvalue Is Nothing Then
                        '1102;@;VALIDATOR;object entry validation for '%1%.%2% (XID %3%) failed. Null or empty value is not allowed.;Provide a correct value;90;Error;false;|R1|R1|;|ENTRYVALIDATOR|XCHANGEENVELOPE|
                        msglog.Add(1102, Nothing, Nothing, Nothing, Nothing, Nothing, _
                             objectentrydefinition.Objectname, objectentrydefinition.Entryname, objectentrydefinition.XID)
                    End If
                    '* return
                    Return otValidationResultType.FailedNoProceed

                ElseIf failedflag Then
                    Return otValidationResultType.FailedNoProceed
                End If

                '''
                ''' finish validating if not validating
                ''' 
                If Not objectentrydefinition.IsValidating Then
                    Return otValidationResultType.Succeeded
                End If

                '''
                ''' 2. Check on Boundaries
                ''' 
                If objectentrydefinition.Datatype = otDataType.Long OrElse objectentrydefinition.Datatype = otDataType.Numeric Then
                    If objectentrydefinition.LowerRangeValue.HasValue AndAlso objectentrydefinition.LowerRangeValue > CLng(newvalue) Then
                        msglog.Add(1103, Nothing, Nothing, Nothing, Nothing, Nothing, _
                        objectentrydefinition.Objectname, objectentrydefinition.Entryname, objectentrydefinition.LowerRangeValue, newvalue, objectentrydefinition.XID)
                        Return otValidationResultType.FailedNoProceed
                    End If
                    If objectentrydefinition.UpperRangeValue.HasValue AndAlso objectentrydefinition.UpperRangeValue < CLng(newvalue) Then
                        msglog.Add(1104, Nothing, Nothing, Nothing, Nothing, Nothing, _
                        objectentrydefinition.Objectname, objectentrydefinition.Entryname, objectentrydefinition.UpperRangeValue, newvalue, objectentrydefinition.XID)
                        Return otValidationResultType.FailedNoProceed
                    End If
                End If

                '''
                ''' 3. Apply the Validation Property Function on the Value
                ''' 

                For Each aProperty In objectentrydefinition.ValidationProperties
                    Dim r As otValidationResultType = aProperty.Apply(newvalue, objectentrydefinition, msglog)
                    If r = otValidationResultType.FailedNoProceed Then
                        Return r
                    ElseIf r > result Then
                        result = r
                    End If
                Next

                '''
                ''' 4. Apply RegExpression Matching
                ''' 
                If Not String.IsNullOrWhiteSpace(objectentrydefinition.ValidateRegExpression) Then
                    Dim aRegexObj As Regex = New Regex(objectentrydefinition.ValidateRegExpression)
                    If Not aRegexObj.IsMatch(newvalue.ToString) Then
                        '1106;@;VALIDATOR;object entry validation for '%1%.%2% (XID %5%) failed. Value '%4%' is not matching against regular expression '%3%'.;Provide a correct value;90;Error;false;|R1|R1|;|ENTRYVALIDATOR|XCHANGEENVELOPE|
                        msglog.Add(1106, Nothing, Nothing, Nothing, Nothing, Nothing, _
                                   objectentrydefinition.Objectname, objectentrydefinition.Entryname, objectentrydefinition.ValidateRegExpression, newvalue, objectentrydefinition.XID)
                        result = otValidationResultType.FailedNoProceed
                    End If
                End If

                Return result


            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectValidator.ValidateEntry")
                Return otValidationResultType.FailedNoProceed
            End Try
        End Function


    End Class
    ''' <summary>
    ''' Class for Object Entry Properties
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EntryProperties

        ''' <summary>
        ''' apply the object entry properties
        ''' </summary>
        ''' <param name="objectDefinition"></param>
        ''' <param name="entryname"></param>
        ''' <param name="in"></param>
        ''' <param name="out"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Apply(objectentrydefinition As iormObjectEntryDefinition, ByVal [in] As Object, ByRef out As Object) As Boolean

            If objectentrydefinition Is Nothing Then
                CoreMessageHandler(message:="entry of object definition is nothing", _
                                    procedure:="EntryProperties.Apply", messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            Try
                Dim theProperties As IEnumerable(Of ObjectEntryProperty) = objectentrydefinition.Properties
                If theProperties Is Nothing OrElse theProperties.Count = 0 Then
                    out = [in]
                    Return True
                End If

                ''' apply
                ''' 
                '*** return result
                Return EntryProperties.Apply(properties:=theProperties, [in]:=[in], out:=out)

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="EntryProperties.Apply")
                Return False
            End Try
        End Function
        ''' <summary>
        ''' apply the object entry properties
        ''' </summary>
        ''' <param name="objectDefinition"></param>
        ''' <param name="entryname"></param>
        ''' <param name="in"></param>
        ''' <param name="out"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Apply(objectDefinition As ormObjectDefinition, entryname As String, ByVal [in] As Object, ByRef out As Object) As Boolean
            Try
                Dim theProperties As IEnumerable(Of ObjectEntryProperty)
                Dim objectid As String = objectDefinition.ID


                ''' apply
                ''' 
                If Not objectDefinition.HasEntry(entryname) Then
                    CoreMessageHandler(message:="entry of object definition could not be found", objectname:=objectid, entryname:=entryname, _
                                        procedure:="EntryProperties.Apply", messagetype:=otCoreMessageType.InternalError)
                    Return False
                Else
                    theProperties = objectDefinition.GetEntry(entryname).Properties
                    '*** return result
                    Return EntryProperties.Apply(properties:=theProperties, [in]:=[in], out:=out)
                End If
               
                Return False
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="EntryProperties.Apply")
                Return False
            End Try
        End Function
        ''' <summary>
        ''' Apply the ObjectEntryProperties to a value
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="in"></param>
        ''' <param name="out"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Apply(objectid As String, entryname As String, ByVal [in] As Object, ByRef out As Object) As Boolean
            Try
                Dim theProperties As IEnumerable(Of ObjectEntryProperty)
                'Dim anObjectClassDescription As ObjectClassDescription
                ''' retrieve the properties
                ''' 
                'If Not CurrentSession.IsBootstrappingInstallationRequested AndAlso _
                '    Not CurrentSession.IsStartingUp AndAlso ot.GetBootStrapObjectClassIDs.Contains(objectid) Then

                '    Dim anObjectDefinition As iormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=objectid)
                '    If anObjectDefinition.HasEntry(entryname:=entryname) Then
                '        theProperties = anObjectDefinition.GetEntryDefinition(entryname).Properties
                '    End If


                'Else
                '    anObjectClassDescription = ot.GetObjectClassDescriptionByID(objectid)

                '    If anObjectClassDescription.GetObjectEntryAttribute(entryname:=entryname) IsNot Nothing Then
                '        If anObjectClassDescription.GetObjectEntryAttribute(entryname:=entryname).HasValueObjectEntryProperties Then
                '            theProperties = anObjectClassDescription.GetObjectEntryAttribute(entryname:=entryname).ObjectEntryProperties
                '            If theProperties Is Nothing Then
                '                out = [in]
                '                Return True
                '            End If

                '        Else
                '            out = [in]
                '            Return True
                '        End If

                '    Else
                '        CoreMessageHandler(message:="entry of object definition could not be found", objectname:=objectid, entryname:=entryname, _
                '                            procedure:="EntryProperties.Apply", messagetype:=otCoreMessageType.InternalError)
                '        Return False
                '    End If
                'End If

                ''' get
                ''' 
                Dim anObjectDefinition As iormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=objectid)
                If anObjectDefinition.HasEntry(entryname:=entryname) Then
                    theProperties = anObjectDefinition.GetEntryDefinition(entryname).Properties
                    ''' apply
                    ''' 
                    Return EntryProperties.Apply(properties:=theProperties, [in]:=[in], out:=out)
                End If
               
                '*** return result
                Return True

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="EntryProperties.Apply")
                Return False
            End Try
        End Function

        ''' <summary>
        ''' apply the object entry properties to an in value and retrieve a out value
        ''' </summary>
        ''' <param name="properties"></param>
        ''' <param name="in"></param>
        ''' <param name="out"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Apply(properties As IEnumerable(Of ObjectEntryProperty), ByVal [in] As Object, ByRef out As Object) As Boolean
            Try
                ''' return
                If properties Is Nothing OrElse properties.Count = 0 Then
                    out = [in]
                    Return True
                End If

                ''' Apply all the Entry Properties
                ''' 
                Dim result As Boolean = True
                Dim outvalue As Object
                Dim inarr() As String 'might be a problem
                Dim outarr() As String
                If IsArray([in]) Then
                    inarr = [in]
                    ReDim outarr(inarr.Count - 1)
                End If

                If properties IsNot Nothing Then
                    For Each aProperty In properties
                        If IsArray([in]) Then
                            result = result And aProperty.Apply([in]:=inarr, out:=outarr)
                            If result Then inarr = outarr ' change the in - it is no reference by
                        Else
                            result = result And aProperty.Apply([in]:=[in], out:=outvalue)
                            If result Then [in] = outvalue ' change the in to reflect changes
                        End If

                    Next
                Else
                    CoreMessageHandler(message:="ObjectEntryProperty is nothing", procedure:="EntryProperties.Apply", messagetype:=otCoreMessageType.InternalError)

                End If

                ' set the final out value

                If result And Not IsArray([in]) Then
                    '** if we have a value
                    If outvalue IsNot Nothing Then
                        out = outvalue
                    Else
                        '** may be since result is true from the beginning 
                        '** no property might be applied
                        out = [in]
                    End If

                Else
                    '** if we have a value
                    If outvalue IsNot Nothing Then
                        out = outarr
                    Else
                        '** may be since result is true from the beginning 
                        '** no property might be applied
                        out = [in]
                    End If

                End If

                '*** return result
                Return result
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="EntryProperties.Apply")
                Return False
            End Try

        End Function
    End Class

End Namespace