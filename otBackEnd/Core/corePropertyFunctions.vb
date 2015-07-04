
REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** CORE PropertyFunctions Classes for On Track Database Backend Library
REM *********** A Property function is a property with parameters in the form "PROP(ARG1, ARG2, ... )" or "PROP"
REM *********** which will be translated from String to a data structure with enumeration and vice versa
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2014-01-06
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2014
REM ***********************************************************************************************************************************************''' <summary>

Imports System.Collections
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Data
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Diagnostics.Debug
Imports System.Attribute
Imports System.IO
Imports System.Text.RegularExpressions

Imports OnTrack.UI
Imports System.Reflection
Imports OnTrack.Core

Namespace OnTrack.Database


    ''' <summary>
    ''' PropertyFunction base Property Class
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class AbstractPropertyFunction(Of T)


        Protected _property As T
        Protected _arguments As Object()

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="property"></param>
        ''' <remarks></remarks>
        Public Sub New([property] As T)
            _property = [property]
        End Sub

        ''' <summary>
        ''' Constructor with arguments
        ''' </summary>
        ''' <param name="property"></param>
        ''' <param name="arguments"></param>
        ''' <remarks></remarks>
        Public Sub New([property] As T, ByVal ParamArray arguments() As Object)
            _property = [property]
            _arguments = arguments
        End Sub
        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="propertystring"></param>
        ''' <remarks></remarks>
        Public Sub New(propertystring As String)
            Dim aName As String
            Dim arguments As String()
            Try
                '** extract arguments
                If propertystring.Contains("(") Then
                    aName = propertystring.Substring(0, propertystring.IndexOf("("c)).ToUpper 'length
                    Dim i = propertystring.LastIndexOf(")"c)
                    If i > 0 Then
                        arguments = propertystring.Substring(propertystring.IndexOf("("c) + 1, i - 1 - propertystring.IndexOf("("c)).Split(","c)
                    Else
                        arguments = propertystring.Substring(propertystring.IndexOf("("c) + 1).Split(","c)
                    End If

                    Dim aList As New List(Of String)
                    For Each arg In arguments
                        aList.Add(arg.ToUpper.Trim)
                    Next
                    _arguments = aList.ToArray
                Else
                    aName = propertystring
                End If
                _property = ToEnum(aName)
                Return
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="AbstractPropertyFunction", argument:=GetType(T).FullName & ":" & propertystring)
            End Try

        End Sub
        ''' <summary>
        ''' set the enumeration
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property [Enum] As T
            Get
                Return _property
            End Get

        End Property
        ''' <summary>
        ''' set or gets the arguments
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Arguments As Object()
            Get
                Return _arguments
            End Get

        End Property

        ''' <summary>
        ''' String representation of this Property
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToString() As String
            Dim aString As String = MyClass.ToString(_property).ToUpper
            If _arguments IsNot Nothing AndAlso _arguments.Count > 0 Then
                aString &= "("
                For i = 0 To _arguments.Count - 1
                    If i > 0 Then aString &= ","
                    aString &= _arguments(i)
                Next
                aString &= ")"
            End If
            Return aString
        End Function
        ''' <summary>
        ''' retuns the enumeration of a string presentation
        ''' </summary>
        ''' <param name="property"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ToEnum(ByVal [property] As String) As T
            Dim fieldinfo() As FieldInfo = GetType(T).GetFields

            ' Loop over the fields.
            For Each field As FieldInfo In fieldinfo
                ' See if this is a literal value
                ' (set at compile time).
                If field.IsLiteral Then
                    If [property].ToUpper = field.Name.ToUpper Then
                        Return CType(field.GetValue(Nothing), T)
                    Else
                        ' List it.
                        For Each attribute In field.GetCustomAttributes(True)
                            If attribute.GetType.Equals(GetType(DescriptionAttribute)) Then
                                If [property].ToUpper = DirectCast(attribute, DescriptionAttribute).Description.ToUpper Then
                                    Return CType(field.GetValue(Nothing), T)
                                End If
                            End If
                        Next
                    End If


                End If
            Next field

            '** throw error
            Throw New Exception(message:="enumeration of " & GetType(T).Name & " has not the defined '" & [property] & "'")


        End Function
        ''' <summary>
        ''' validates the property string against the enumeration T
        ''' </summary>
        ''' <param name="property"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Validate(Of T)(ByVal [property] As String) As Boolean
            Dim fieldinfo() As FieldInfo = GetType(T).GetFields

            ' Loop over the fields.
            For Each field As FieldInfo In fieldinfo
                ' See if this is a literal value
                ' (set at compile time).
                If field.IsLiteral Then
                    If [property].ToUpper = field.Name.ToUpper Then
                        Return True
                    Else
                        ' List it.
                        For Each attribute In field.GetCustomAttributes(True)
                            If attribute.GetType.Equals(GetType(DescriptionAttribute)) Then
                                If [property].ToUpper = DirectCast(attribute, DescriptionAttribute).Description.ToUpper Then
                                    Return True
                                End If
                            End If
                        Next
                    End If


                End If
            Next field
            Return False
        End Function
        ''' <summary>
        ''' returns the string presentation of the enum 
        ''' </summary>
        ''' <param name="enumconstant"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ToString(ByVal enumconstant As T) As String
            Dim fi As Reflection.FieldInfo = enumconstant.GetType().GetField(enumconstant.ToString())
            Dim aattr() As DescriptionAttribute = DirectCast(fi.GetCustomAttributes(GetType(DescriptionAttribute), False), DescriptionAttribute())
            If aattr.Length > 0 Then
                Return aattr(0).Description
            Else
                Return enumconstant.ToString()
            End If
        End Function
    End Class

    ''' <summary>
    ''' ObjectPermission Rule Property
    ''' 
    ''' </summary>
    ''' <remarks> 
    ''' Validation Rules like 
    ''' 1) OTDBACCESS( DBACCESSRIGHT, FALSE|TRUE, FALSE|TRUE) which checks if the user has the DB Access right, then return ARG2 
    ''' and end the permission checking if third argument is true
    ''' 2) GROUP( [GROUPNAME] FALSE|TRUE, FALSE|TRUE) which checks if the user is in the group by name, then return ARG2 
    ''' and end the permission checking if third argument is true
    ''' 3) USER ( [USERNAME], FALSE|TRUE, FALSE|TRUE) which checks if the user is the username, then return ARG2 
    ''' and end the permission checking if third argument is true
    ''' </remarks>
    Public Class ObjectPermissionRuleProperty
        Inherits AbstractPropertyFunction(Of otObjectPermissionRuleProperty)
        Public Const DBAccess = "OTDBACCESS"
        Public Const Group = "GROUP"
        Public Const UserID = "USER"

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="propertystring"></param>
        ''' <remarks></remarks>
        Public Sub New(propertystring As String)
            MyBase.New(propertystring:=propertystring)
            If Not Validate(Me) Then
                CoreMessageHandler(message:="Argument value is not valid", argument:=propertystring, procedure:="ObjectPermissionRuleProperty.New", _
                                    messagetype:=otCoreMessageType.InternalError)
            End If
        End Sub
        ''' <summary>
        ''' returns True if ExitOnTrue Flag is set
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ExitOnTrue
            Get
                If Not Validate() Then Return False

                Select Case _property
                    Case otObjectPermissionRuleProperty.DBAccess
                        Return CBool(_arguments(1))
                    Case otObjectPermissionRuleProperty.Group, otObjectPermissionRuleProperty.User
                        Return CBool(_arguments(0))
                    Case Else
                        Return True
                End Select
            End Get
        End Property
        ''' <summary>
        ''' returns True if ExitOnTrue Flag is set
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ExitOnFalse
            Get
                If Not Validate() Then Return False

                Select Case _property
                    Case otObjectPermissionRuleProperty.DBAccess
                        Return CBool(_arguments(2))
                    Case otObjectPermissionRuleProperty.Group, otObjectPermissionRuleProperty.User
                        Return CBool(_arguments(1))
                    Case Else
                        Return True
                End Select
            End Get
        End Property
        ''' <summary>
        ''' validates the property
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Validate() As Boolean
            Return Validate(Me)
        End Function
        ''' <summary>
        ''' validates the property
        ''' </summary>
        ''' <param name="property"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Validate([property] As ObjectPermissionRuleProperty) As Boolean
            Try
                Select Case [property].Enum
                    Case otObjectPermissionRuleProperty.DBAccess
                        If [property].Arguments.Count = 3 Then
                            Dim accessright As AccessRightProperty = New AccessRightProperty([property].Arguments(0).ToString)
                            If Not CBool([property].Arguments(1)) Then
                                CoreMessageHandler(message:="second argument must be a bool ", argument:=[property].ToString, _
                                               procedure:="ObjectpermissionRuleProperty.Validate", messagetype:=otCoreMessageType.InternalError)
                                Return False
                            End If
                            If Not CBool([property].Arguments(2)) Then
                                CoreMessageHandler(message:="third argument must be a bool ", argument:=[property].ToString, _
                                               procedure:="ObjectpermissionRuleProperty.Validate", messagetype:=otCoreMessageType.InternalError)
                                Return False
                            End If
                            Return True
                        Else
                            CoreMessageHandler(message:="Number of arguments wrong (should be 3)", argument:=[property].ToString, _
                                               procedure:="ObjectpermissionRuleProperty.Validate", messagetype:=otCoreMessageType.InternalError)
                            Return False
                        End If
                    Case otObjectPermissionRuleProperty.Group, otObjectPermissionRuleProperty.User
                        If [property].Arguments.Count = 1 Then
                            If Not CBool([property].Arguments(0)) Then
                                CoreMessageHandler(message:="first argument must be a bool ", argument:=[property].ToString, _
                                               procedure:="ObjectpermissionRuleProperty.Validate", messagetype:=otCoreMessageType.InternalError)
                                Return False
                            End If
                            If Not CBool([property].Arguments(1)) Then
                                CoreMessageHandler(message:="second argument must be a bool ", argument:=[property].ToString, _
                                               procedure:="ObjectpermissionRuleProperty.Validate", messagetype:=otCoreMessageType.InternalError)
                                Return False
                            End If
                            Return True
                        Else
                            CoreMessageHandler(message:="Number of arguments wrong (should be one)", argument:=[property].ToString, _
                                               procedure:="ObjectpermissionRuleProperty.Validate", messagetype:=otCoreMessageType.InternalError)
                            Return False
                        End If
                    Case Else
                        Return True
                End Select
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectpermissionRuleProperty.Validate")
                Return False
            End Try

        End Function
        ''' <summary>
        ''' returns the enumeration value
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToEnum() As otObjectPermissionRuleProperty
            Return AbstractPropertyFunction(Of otObjectPermissionRuleProperty).ToEnum(_property)
        End Function
    End Class
    ''' <summary>
    ''' Enumeration of the validation properties
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otObjectPermissionRuleProperty
        <Description(ObjectPermissionRuleProperty.DBAccess)> DBAccess
        <Description(ObjectPermissionRuleProperty.Group)> Group
        <Description(ObjectPermissionRuleProperty.UserID)> User
    End Enum

    ''' <summary>
    ''' ForeignKey Property
    ''' 
    ''' </summary>
    ''' <remarks> 
    ''' Validation Rules like 
    ''' 1) ONDELETE( CASCADE | RESTRICT | DEFAULT | NULL | NOOP ) which checks if the user has the DB Access right, then return ARG2 
    ''' and end the permission checking if third argument is true
    ''' 2) ONUPDATE (CASCADE | RESTRICT | DEFAULT | NULL | NOOP ) which checks if the user is in the group by name, then return ARG2 
    ''' and end the permission checking if third argument is true
    ''' </remarks>
    Public Class ForeignKeyProperty
        Inherits AbstractPropertyFunction(Of otForeignKeyProperty)
        Public Const OnUpdate = "ONUPDATE"
        Public Const OnDelete = "ONDELETE"
        Public Const PrimaryTableLink = "PRIMARYTABLELINK"

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="propertystring"></param>
        ''' <remarks></remarks>
        Public Sub New(propertystring As String)
            MyBase.New(propertystring:=propertystring)
            If Not Validate(Me) Then
                CoreMessageHandler(message:="Argument value is not valid", argument:=propertystring, procedure:="ObjectPermissionRuleProperty.New", _
                                    messagetype:=otCoreMessageType.InternalError)
            End If
        End Sub

        ''' <summary>
        ''' returns the ForeignKey Action Property
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ActionProperty() As ForeignKeyActionProperty
            Return New ForeignKeyActionProperty(Me.Arguments(0).ToString)
        End Function
        ''' <summary>
        ''' returns the Foreign Key Action enumeration
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Action() As otForeignKeyAction
            Return New ForeignKeyActionProperty(Me.Arguments(0).ToString).ToEnum
        End Function
        ''' <summary>
        ''' validates the property
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Validate() As Boolean
            Return Validate(Me)
        End Function
        ''' <summary>
        ''' validates the property
        ''' </summary>
        ''' <param name="property"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Validate([property] As ForeignKeyProperty) As Boolean
            Try
                Select Case [property].Enum
                    Case otForeignKeyProperty.OnUpdate, otForeignKeyProperty.OnDelete
                        If [property].Arguments.Count = 1 Then
                            If Not ForeignKeyActionProperty.Validate([property].Arguments(0).ToString) Then
                                CoreMessageHandler(message:="argument must be of otForeignKeyAction ", argument:=[property].ToString, _
                                               procedure:="ForeignKeyProperty.Validate", messagetype:=otCoreMessageType.InternalError)
                                Return False
                            End If
                            Return True
                        Else
                            CoreMessageHandler(message:="Number of arguments wrong (should be 1)", argument:=[property].ToString, _
                                               procedure:="ForeignKeyProperty.Validate", messagetype:=otCoreMessageType.InternalError)
                            Return False
                        End If
                    Case otForeignKeyProperty.PrimaryTableLink
                        Return True
                    Case Else
                        Return False
                End Select
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ForeignKeyProperty.Validate")
                Return False
            End Try

        End Function
        ''' <summary>
        ''' returns the enumeration value
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function ToEnum() As otForeignKeyProperty
            Return AbstractPropertyFunction(Of otForeignKeyProperty).ToEnum(_property)
        End Function
    End Class
    ''' <summary>
    ''' Enumeration of the validation properties
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otForeignKeyProperty
        <Description(ForeignKeyProperty.OnUpdate)> OnUpdate = 1
        <Description(ForeignKeyProperty.OnDelete)> OnDelete
        <Description(ForeignKeyProperty.PrimaryTableLink)> PrimaryTableLink 'Link a secondary table to a primary
    End Enum
    ''' <summary>
    ''' ObjectPermission Rule Property
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ForeignKeyActionProperty
        Inherits AbstractPropertyFunction(Of otForeignKeyAction)

        Public Const Cascade = "CASCADE"
        Public Const NOOP = "NOOP"
        Public Const Restrict = "RESTRICT"
        Public Const SetDefault = "DEFAULT"
        Public Const SetNull = "NULL"

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="propertystring"></param>
        ''' <remarks></remarks>
        Public Sub New(propertystring As String)
            MyBase.New(propertystring:=propertystring)
        End Sub

        ''' <summary>
        ''' Validate the string before a Property is created
        ''' </summary>
        ''' <param name="propertystring"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Validate(propertystring As String) As Boolean
            Return AbstractPropertyFunction(Of ForeignKeyActionProperty).Validate(Of otForeignKeyAction)(propertystring)
        End Function

        ''' <summary>
        ''' returns the enumeration value
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToEnum() As otForeignKeyAction
            Return AbstractPropertyFunction(Of otForeignKeyAction).ToEnum(_property)
        End Function

    End Class
    ''' <summary>
    ''' Enumeration for Access Rights to the database
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otForeignKeyAction
        <Description(ForeignKeyActionProperty.Cascade)> Cascade = 0
        <Description(ForeignKeyActionProperty.NOOP)> Noop
        <Description(ForeignKeyActionProperty.Restrict)> Restrict
        <Description(ForeignKeyActionProperty.SetNull)> SetNull
        <Description(ForeignKeyActionProperty.SetDefault)> SetDefault
    End Enum


    ''' <summary>
    ''' ObjectEntry (Field) Property Class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ObjectEntryProperty
        Inherits AbstractPropertyFunction(Of otObjectEntryProperty)
        Public Const Upper = "UPPER"
        Public Const Lower = "LOWER"
        Public Const Trim = "TRIM"
        Public Const Capitalize = "CAPITALIZE"
        Public Const Keyword = "KEYWORD"
        Public Const Encrypted = "ENCRYPTED"
        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="propertystring"></param>
        ''' <remarks></remarks>
        Public Sub New(propertystring As String)
            MyBase.New(propertystring:=propertystring)
        End Sub
        ''' <summary>
        ''' Apply the Property function to a value
        ''' </summary>
        ''' <param name="in"></param>
        ''' <param name="out"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Apply(ByVal [in] As String(), ByRef [out] As String()) As Boolean
            If [in] Is Nothing Then Return True
            For i = 0 To [in].Count - 1
                Me.Apply([in]:=[in](i), out:=out(i))
            Next
            Return True
        End Function
        ''' <summary>
        ''' Apply the Property function to a value
        ''' </summary>
        ''' <param name="in"></param>
        ''' <param name="out"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Apply(ByVal [in] As Object, ByRef [out] As Object) As Boolean
            If [in] Is Nothing Then
                [out] = [in]
                Return True
            End If
            Select Case _property
                Case otObjectEntryProperty.Lower
                    [out] = [in].ToString.ToLower
                    Return True
                Case otObjectEntryProperty.Upper
                    [out] = [in].ToString.ToUpper
                    Return True
                Case otObjectEntryProperty.Trim
                    [out] = [in].ToString.Trim
                    Return True
                Case otObjectEntryProperty.Keyword
                    [out] = [in].ToString.Trim.ToUpper
                    Return True
                Case otObjectEntryProperty.Capitalize
                    [out] = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase([in].ToString)
                    Return True
                Case otObjectEntryProperty.Encrypted
                    [out] = [in].ToString.Trim
                    Return True
                Case Else
                    CoreMessageHandler(message:="Property function is not implemented", argument:=_property.ToString, messagetype:=otCoreMessageType.InternalError, _
                                       procedure:="ObjectEntryProperty.Apply")
                    Return False
            End Select
        End Function
        ''' <summary>
        ''' returns the enumeration value
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToEnum() As otObjectEntryProperty
            Return AbstractPropertyFunction(Of otObjectEntryProperty).ToEnum(_property)
        End Function
    End Class
    ''' <summary>
    ''' Enumeration of the validation properties
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otObjectEntryProperty
        <Description(ObjectEntryProperty.Upper)> Upper
        <Description(ObjectEntryProperty.Lower)> Lower
        <Description(ObjectEntryProperty.Trim)> Trim
        <Description(ObjectEntryProperty.Capitalize)> Capitalize
        <Description(ObjectEntryProperty.Keyword)> Keyword
        <Description(ObjectEntryProperty.Encrypted)> Encrypted
    End Enum


    ''' <summary>
    ''' ObjectEntry Validation Property Class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class LookupProperty
        Inherits AbstractPropertyFunction(Of otLookupProperty)
        Public Const UseAttributeReference = "USEREFERENCE"
        Public Const UseAttributeValues = "USEATTRIBUTEVALUES"
        Public Const UseForeignKey = "USEFOREIGNKEY"
        Public Const UseObjectEntry = "USEOBJECT"
        Public Const UseVALUELIST = "USEVALUELIST"
        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="propertystring"></param>
        ''' <remarks></remarks>
        Public Sub New(propertystring As String)
            MyBase.New(propertystring:=propertystring)
            If Not Validate(Me) Then
                CoreMessageHandler(message:="Argument value is not valid", argument:=propertystring, procedure:="LookupProperty.New", _
                                    messagetype:=otCoreMessageType.InternalError)
            End If
        End Sub
        ''' <summary>
        ''' validates the property
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Validate() As Boolean
            Return Validate(Me)
        End Function
        ''' <summary>
        ''' validates the property
        ''' </summary>
        ''' <param name="property"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Validate([property] As LookupProperty) As Boolean
            Try
                Select Case [property].Enum
                    Case otLookupProperty.UseForeignKey
                        '''
                        ''' optional arguemnt of foreign key
                        ''' 
                        If [property].Arguments IsNot Nothing AndAlso [property].Arguments.Count > 1 Then
                            CoreMessageHandler(message:="first argument is optional foreign key name - more arguments specified as required ", argument:=[property].ToString, _
                                           procedure:="LookupProperty.Validate", messagetype:=otCoreMessageType.InternalError)
                            Return False
                        End If

                    Case otLookupProperty.UseObjectEntry
                        If [property].Arguments IsNot Nothing AndAlso [property].Arguments.Count = 1 Then
                            If [property].Arguments(0) = Nothing OrElse [property].Arguments(0) = String.Empty Then
                                CoreMessageHandler(message:="first argument must be a object name ", argument:=[property].ToString, _
                                               procedure:="LookupProperty.Validate", messagetype:=otCoreMessageType.InternalError)
                                Return False
                            End If
                        Else
                            CoreMessageHandler(message:="Number of arguments wrong (should be one)", argument:=[property].ToString, _
                                               procedure:="LookupProperty.Validate", messagetype:=otCoreMessageType.InternalError)
                            Return False
                        End If
                    Case otLookupProperty.UseValueList
                        If [property].Arguments IsNot Nothing AndAlso [property].Arguments.Count = 1 Then
                            If [property].Arguments(0) = Nothing OrElse [property].Arguments(0) = String.Empty Then
                                CoreMessageHandler(message:="first argument must be a value list id ", argument:=[property].ToString, _
                                               procedure:="LookupProperty.Validate", messagetype:=otCoreMessageType.InternalError)
                                Return False
                            End If
                        Else
                            CoreMessageHandler(message:="Number of arguments wrong (should be one)", argument:=[property].ToString, _
                                               procedure:="LookupProperty.Validate", messagetype:=otCoreMessageType.InternalError)
                            Return False
                        End If
                    Case otLookupProperty.UseAttributeValues, otLookupProperty.UseAttributeReference
                        If [property].Arguments IsNot Nothing AndAlso [property].Arguments.Count > 0 Then
                            CoreMessageHandler(message:="Number of arguments wrong (should be none)", argument:=[property].ToString, _
                                               procedure:="LookupProperty.Validate", messagetype:=otCoreMessageType.InternalError)
                            Return False
                        End If
                    Case Else
                        Return True
                End Select

                Return True
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="LookupProperty.Validate")
                Return False
            End Try

        End Function

        ''' <summary>
        ''' returns a unique list of values of a object entry
        ''' </summary>
        ''' <param name="objectentryattribute"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UniqueForeignKeyValues(objectentryname As String, objectname As String, Optional foreignkeyname As String = Nothing, Optional allkeys As Boolean = False) As IList(Of Object)
            Dim anObjectname As String = objectname
            Dim anEntyname As String

            Dim names As String() = Shuffle.NameSplitter(objectentryname.ToUpper)
            If names.Length > 1 Then
                anObjectname = names(0)
                anEntyname = names(1)
            Else
                anEntyname = names(0)
            End If
            If objectname Is Nothing Then
                CoreMessageHandler(message:="ObjectEntryname has no objectname", argument:=objectentryname, _
                                  procedure:="LookupProperty.UniqueRowValues", messagetype:=otCoreMessageType.InternalError)
                Return New List(Of Object)
            End If
            Dim aclassdescription As ObjectClassDescription = ot.GetObjectClassDescriptionByID(anObjectname)
            If aclassdescription Is Nothing Then
                CoreMessageHandler(message:="Objectname is not in class repository", argument:=objectentryname, _
                                  procedure:="LookupProperty.UniqueRowValues", messagetype:=otCoreMessageType.InternalError)
                Return New List(Of Object)
            End If
            Dim anObjectEntryAttribute = aclassdescription.GetObjectEntryAttribute(entryname:=anEntyname)
            If anObjectEntryAttribute Is Nothing Then
                CoreMessageHandler(message:="ObjectEntry is not in class repository", argument:=objectentryname, _
                                 procedure:="LookupProperty.UniqueRowValues", messagetype:=otCoreMessageType.InternalError)
                Return New List(Of Object)
            End If
            If Not anObjectEntryAttribute.HasValueContainerID Then
                CoreMessageHandler(message:="ObjectEntryAttribute has no Tablename", argument:=anObjectEntryAttribute.ObjectName & "." & anObjectEntryAttribute.EntryName, _
                                   procedure:="LookupProperty.UniqueRowValues", messagetype:=otCoreMessageType.InternalError)
                Return New List(Of Object)
            End If
            Dim anObjectDefinition As ormObjectDefinition = ot.CurrentSession.Objects.GetObjectDefinition(id:=anObjectname)
            If anObjectDefinition Is Nothing Then
                CoreMessageHandler(message:="ObjectDefinition not found", argument:=anObjectname, _
                                   procedure:="LookupProperty.UniqueRowValues", messagetype:=otCoreMessageType.InternalError)
                Return New List(Of Object)
            End If
            Dim aTablename As String = anObjectEntryAttribute.ContainerID
            Dim aColumnname As String = anObjectEntryAttribute.ContainerEntryName
            Dim aTable As ormContainerDefinition = anObjectDefinition.GetContainer(aTablename)
            Dim aForeignKey As ormForeignKeyDefinition
            Dim found As Boolean = False
            Dim indexEntry As Integer
            Dim i As Integer = 0

            ''' search the foreign key with the referenc column
            ''' 
            For Each aForeignKey In aTable.ForeignKeys
                If foreignkeyname Is Nothing OrElse (foreignkeyname IsNot Nothing AndAlso aForeignKey.Id = foreignkeyname.ToUpper) Then
                    Dim columns As IList(Of String) = aForeignKey.ColumnNames.ToList
                    indexEntry = 0
                    For Each aName In columns
                        Dim cname = Shuffle.NameSplitter(aName)
                        If cname(1) = aColumnname Then
                            found = True
                            Exit For
                        End If
                        indexEntry += 1
                    Next
                End If
            Next

            If Not found Then
                CoreMessageHandler(message:="no foreign key definition in table '" & aTablename & "' with columnname '" & aColumnname & "' found", argument:=anObjectname, _
                                  procedure:="LookupProperty.UniqueRowValues", messagetype:=otCoreMessageType.InternalError)
                Return New List(Of Object)
            End If

            Dim aForeignKeyReferences As List(Of String) = aForeignKey.ForeignKeyReferences.ToList
            Dim aDomainFieldname As String = Commons.Domain.ConstFNDomainID
            Dim aCommand As ormSqlSelectCommand
            Dim theForeignKeyTables As List(Of String) = aForeignKey.ForeignKeyReferenceTables
            Dim hasDomainBehavior As Boolean = True

            '''
            ''' set the domain behavior from the referenced tables -> for mixed domain behavior this gets interesting (not implemented)
            ''' 
            For Each atableid In theForeignKeyTables
                hasDomainBehavior = hasDomainBehavior And CurrentSession.Objects.GetContainerDefinition(atableid).HasDomainBehavior
            Next
            '''
            '''
            Try
                If theForeignKeyTables.Count = 1 Then
                    Dim aStore As iormRelationalTableStore = ot.GetPrimaryTableStore(tableid:=theForeignKeyTables.First)
                    aCommand = aStore.CreateSqlSelectCommand(id:=aColumnname & "_FKValues_" & aForeignKey.Id, addAllFields:=False)
                Else
                    If ot.CurrentOTDBDriver.GetType.GetInterfaces.Contains(GetType(iormRelationalDatabaseDriver)) Then
                        aCommand = CType(ot.CurrentOTDBDriver, iormRelationalDatabaseDriver).CreateSqlSelectCommand(id:=aTablename & "_" & aColumnname & "_FKValues_" & aForeignKey.Id)
                    Else
                        Call CoreMessageHandler(message:="current database driver is not a relational driver - no sql possible to build lookup foreign key values", _
                                                procedure:="LookupProperty.UniqueRowValues", messagetype:=otCoreMessageType.InternalError)
                        Return New List(Of Object)
                    End If

                End If

                If Not aCommand.IsPrepared Then

                    ''' build for multiple tables
                    ''' 
                    If theForeignKeyTables.Count > 1 Then
                        For Each atableid In theForeignKeyTables
                            aCommand.AddTable(atableid, addAllFields:=False)
                        Next
                    End If
                    ''' build select
                    aCommand.select = "DISTINCT "

                    If Not allkeys Then
                        '' take the referenceing key entry
                        Dim colnames As String() = Shuffle.NameSplitter(aForeignKeyReferences.ElementAt(indexEntry))
                        aCommand.select &= colnames(0) & ".[" & colnames(1) & "] "
                    Else
                        i = 0
                        For Each aName In aForeignKeyReferences
                            Dim colnames As String() = Shuffle.NameSplitter(aName)
                            If i > 1 Then aCommand.select &= ","
                            aCommand.select &= colnames(0) & ".[" & colnames(1) & "] "
                            i += 1
                        Next
                    End If



                    ''' build where
                    '''  
                    For Each atableid In theForeignKeyTables
                        If hasDomainBehavior Then aCommand.select &= "," & atableid & ".[" & aDomainFieldname & "]"
                        aCommand.Where = atableid & ".[" & ConstFNIsDeleted & "] = @" & atableid & "Deleted "
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@" & atableid & "Deleted ", notColumn:=True, datatype:=otDataType.Bool))
                        If hasDomainBehavior Then
                            aCommand.Where &= " AND (" & atableid & ".[" & aDomainFieldname & "] = @" & atableid & "domainID OR " & atableid & ".[" & aDomainFieldname & "] = @globalID)"
                            aCommand.AddParameter(New ormSqlCommandParameter(ID:="@" & atableid & "domainID", notColumn:=True, datatype:=otDataType.Text))
                        End If

                    Next
                    If hasDomainBehavior Then aCommand.AddParameter(New ormSqlCommandParameter(ID:="@globalID", notColumn:=True, datatype:=otDataType.Text))
                    aCommand.Prepare()
                End If

                For Each atableid In theForeignKeyTables
                    aCommand.SetParameterValue(ID:="@" & atableid & "Deleted", value:=False)
                    If hasDomainBehavior Then aCommand.SetParameterValue(ID:="@" & atableid & "domainID", value:=CurrentSession.CurrentDomainID)
                Next

                If hasDomainBehavior Then aCommand.SetParameterValue(ID:="@globalID", value:=ConstGlobalDomain)

                Dim aRecordCollection As List(Of ormRecord) = aCommand.RunSelect
                Dim DomainValues As New Dictionary(Of Object, String)

                For Each aRecord As ormRecord In aRecordCollection
                    Dim aDomainValue As String
                    If hasDomainBehavior Then
                        aDomainValue = aRecord.GetValue(2).ToString
                    Else
                        aDomainValue = CurrentSession.CurrentDomainID
                    End If

                    Dim aValue As Object = aRecord.GetValue(1)
                    If aValue IsNot Nothing Then
                        If DomainValues.ContainsKey(aValue) Then
                            If DomainValues.Item(aValue) = ConstGlobalDomain Then
                                aDomainValue.Remove(aValue)
                                DomainValues.Add(key:=aValue, value:=aDomainValue)
                            End If
                        Else
                            DomainValues.Add(key:=aValue, value:=aDomainValue)
                        End If
                    End If

                Next

                Return DomainValues.Keys.ToList


            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="LookupProperty.UniqueRowValues")
                Return New List(Of Object)

            End Try

        End Function

        ''' <summary>
        ''' returns a unique list of values of a object entry
        ''' </summary>
        ''' <param name="objectentryattribute"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UniqueObjectColumnEntryValues(objectentryname As String, Optional objectname As String = Nothing) As IList(Of Object)
            Dim anObjectname As String = objectname
            Dim anEntyname As String

            Dim names As String() = Shuffle.NameSplitter(objectentryname.ToUpper)
            If names.Length > 1 Then
                anObjectname = names(0)
                anEntyname = names(1)
            Else
                anEntyname = names(0)
            End If
            If objectname Is Nothing Then
                CoreMessageHandler(message:="ObjectEntryname has no objectname", argument:=objectentryname, _
                                  procedure:="LookupProperty.UniqueRowValues", messagetype:=otCoreMessageType.InternalError)
                Return New List(Of Object)
            End If
            Dim aclassdescription As ObjectClassDescription = ot.GetObjectClassDescriptionByID(anObjectname)
            If aclassdescription Is Nothing Then
                CoreMessageHandler(message:="Objectname is not in class repository", argument:=objectentryname, _
                                  procedure:="LookupProperty.UniqueRowValues", messagetype:=otCoreMessageType.InternalError)
                Return New List(Of Object)
            End If
            Dim anObjectEntryAttribute As ormObjectEntryAttribute = aclassdescription.GetObjectEntryAttribute(entryname:=anEntyname)
            If anObjectEntryAttribute Is Nothing Then
                CoreMessageHandler(message:="ObjectEntry is not in class repository", argument:=objectentryname, _
                                 procedure:="LookupProperty.UniqueRowValues", messagetype:=otCoreMessageType.InternalError)
                Return New List(Of Object)
            End If
            If Not anObjectEntryAttribute.HasValueContainerID Then
                CoreMessageHandler(message:="ObjectEntryAttribute has no Tablename", argument:=anObjectEntryAttribute.ObjectName & "." & anObjectEntryAttribute.EntryName, _
                                   procedure:="LookupProperty.UniqueRowValues", messagetype:=otCoreMessageType.InternalError)
                Return New List(Of Object)
            End If
            Dim anObjectDefinition As ormObjectDefinition = ot.CurrentSession.Objects.GetObjectDefinition(id:=anObjectname)
            If anObjectDefinition Is Nothing Then
                CoreMessageHandler(message:="ObjectDefinition not found", argument:=anObjectname, _
                                   procedure:="LookupProperty.UniqueRowValues", messagetype:=otCoreMessageType.InternalError)
                Return New List(Of Object)
            End If
            Dim aTablename As String = anObjectEntryAttribute.ContainerID
            Dim aColumnname As String = anObjectEntryAttribute.ContainerEntryName
            Dim aDomainFieldname As String = Commons.Domain.ConstFNDomainID

            '''
            '''
            Try
                Dim aStore As iormRelationalTableStore = ot.GetPrimaryTableStore(tableid:=aTablename)
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(id:=aColumnname & "_Values", addAllFields:=False)
                If Not aCommand.IsPrepared Then
                    aCommand.select = "DISTINCT [" & aColumnname & "]"
                    If anObjectDefinition.HasDomainBehavior Then aCommand.select &= ", [" & aDomainFieldname & "]"
                    aCommand.Where = ConstFNIsDeleted & " = @deleted "
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@deleted", ColumnName:=ConstFNIsDeleted, tableid:=aTablename))
                    If anObjectDefinition.HasDomainBehavior Then
                        aCommand.Where &= " AND ([" & aDomainFieldname & "] = @domainID OR [" & aDomainFieldname & "] = @globalID)"
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@domainID", ColumnName:=aDomainFieldname, tableid:=aTablename))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@globalID", ColumnName:=aDomainFieldname, tableid:=aTablename))
                    End If

                    aCommand.Prepare()
                End If

                aCommand.SetParameterValue(ID:="@deleted", value:=False)
                If anObjectDefinition.HasDomainBehavior Then
                    aCommand.SetParameterValue(ID:="@domainID", value:=CurrentSession.CurrentDomainID)
                    aCommand.SetParameterValue(ID:="@globalID", value:=ConstGlobalDomain)
                End If


                Dim aRecordCollection As List(Of ormRecord) = aCommand.RunSelect
                Dim DomainValues As New Dictionary(Of Object, String)

                For Each aRecord As ormRecord In aRecordCollection
                    Dim aDomainValue As String
                    If anObjectDefinition.HasDomainBehavior Then
                        aDomainValue = aRecord.GetValue(2).ToString
                    Else
                        aDomainValue = CurrentSession.CurrentDomainID
                    End If

                    Dim aValue As Object = aRecord.GetValue(1)
                    If aValue IsNot Nothing Then
                        If DomainValues.ContainsKey(aValue) Then
                            If DomainValues.Item(aValue) = ConstGlobalDomain Then
                                aDomainValue.Remove(aValue)
                                DomainValues.Add(key:=aValue, value:=aDomainValue)
                            End If
                        Else
                            DomainValues.Add(key:=aValue, value:=aDomainValue)
                        End If
                    End If
                Next

                Return DomainValues.Keys.ToList


            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="LookupProperty.UniqueRowValues")
                Return New List(Of Object)

            End Try

        End Function
        ''' <summary>
        ''' Apply the Property function to a value
        ''' </summary>
        ''' <param name="in"></param>
        ''' <param name="out"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetValues(entry As iormObjectEntryDefinition) As List(Of Object)
            Dim aList As New List(Of Object)

            Select Case _property

                Case otLookupProperty.UseAttributeReference
                    '''
                    ''' use the object reference
                    ''' 
                    Dim aclassdescription As ObjectClassDescription = ot.GetObjectClassDescriptionByID(entry.Objectname)
                    If aclassdescription Is Nothing Then
                        CoreMessageHandler(message:="Objectname is not in class repository", argument:=entry.Objectname, _
                                          procedure:="LookupProperty.GetValues", messagetype:=otCoreMessageType.InternalError)
                        Return New List(Of Object)
                    End If
                    Dim anObjectEntryAttribute As ormObjectEntryAttribute = aclassdescription.GetObjectEntryAttribute(entryname:=entry.Entryname)
                    If anObjectEntryAttribute Is Nothing Then
                        CoreMessageHandler(message:="ObjectEntry is not in class repository", argument:=entry.Objectname & "." & entry.Entryname, _
                                         procedure:="LookupProperty.GetValues", messagetype:=otCoreMessageType.InternalError)
                        Return New List(Of Object)
                    End If
                    If anObjectEntryAttribute.HasValueReferenceObjectEntry Then
                        Dim names As String() = Shuffle.NameSplitter(anObjectEntryAttribute.ReferenceObjectEntry)
                        Dim aReference As ormObjectEntryAttribute = ot.GetObjectClassDescriptionByID(id:=names(0)).GetObjectEntryAttribute(entryname:=names(1))
                        Return UniqueObjectColumnEntryValues(objectentryname:=aReference.EntryName, objectname:=aReference.ObjectName)
                    Else
                        CoreMessageHandler(message:="ObjectEntry has no references to lookup", argument:=entry.Objectname & "." & entry.Entryname, _
                                         procedure:="LookupProperty.GetValues", messagetype:=otCoreMessageType.InternalError)
                        Return New List(Of Object)
                    End If



                Case otLookupProperty.UseAttributeValues
                    '''
                    ''' User the Posiible Value Attribute Entry 
                    '''
                    If entry.PossibleValues IsNot Nothing Then
                        For Each aValue In entry.PossibleValues
                            aList.Add(aValue)
                        Next
                    End If

                    Return aList


                Case otLookupProperty.UseForeignKey

                    '''
                    ''' use the foreign keys
                    ''' 
                    Return Me.UniqueForeignKeyValues(objectentryname:=entry.Entryname, objectname:=entry.Objectname)

                Case otLookupProperty.UseObjectEntry
                    '''
                    ''' use the object entry in the argument
                    ''' 
                    If Arguments.Count = 0 OrElse String.IsNullOrWhiteSpace(Arguments(0).ToString) Then
                        CoreMessageHandler(message:="UseObjectEntry Attribute für ObjectEntry has missing reference object entry argument", argument:=entry.Objectname & "." & entry.Entryname, _
                                       procedure:="LookupProperty.GetValues", messagetype:=otCoreMessageType.InternalError)
                        Return New List(Of Object)
                    End If
                    Dim names As String() = Shuffle.NameSplitter(Arguments(0).ToString)
                    If names.Count < 2 OrElse String.IsNullOrWhiteSpace(Arguments(0).ToString) Then
                        CoreMessageHandler(message:="UseObjectEntry Attribute für ObjectEntry has malformatted reference object entry argument '" & Arguments(0).ToString & "'", argument:=entry.Objectname & "." & entry.Entryname, _
                                       procedure:="LookupProperty.GetValues", messagetype:=otCoreMessageType.InternalError)
                        Return New List(Of Object)
                    End If
                    Dim aclassdescription As ObjectClassDescription = ot.GetObjectClassDescriptionByID(names(0))
                    If aclassdescription Is Nothing Then
                        CoreMessageHandler(message:="Objectname is not in class repository", argument:=names(0), _
                                          procedure:="LookupProperty.GetValues", messagetype:=otCoreMessageType.InternalError)
                        Return New List(Of Object)
                    End If
                    Dim anObjectEntryAttribute As iormObjectEntryDefinition = aclassdescription.GetObjectEntryAttribute(entryname:=names(1))
                    If anObjectEntryAttribute Is Nothing Then
                        CoreMessageHandler(message:="ObjectEntry is not in class repository", argument:=Arguments(0).ToString, _
                                         procedure:="LookupProperty.GetValues", messagetype:=otCoreMessageType.InternalError)
                        Return New List(Of Object)
                    End If
                    Return UniqueObjectColumnEntryValues(objectentryname:=names(1), objectname:=names(0))

                Case otLookupProperty.UseValueList
                    ''' Value List 
                    ''' 
                    Dim aValueList As Commons.ValueList = CurrentSession.ValueList(name:=Me._arguments(0).ToString)
                    If aValueList IsNot Nothing Then
                        aList = aValueList.Values
                        Return aList
                    Else
                        CoreMessageHandler(message:="list could not be retrieved", argument:=Me._arguments(0).ToString, messagetype:=otCoreMessageType.ApplicationError, _
                                     procedure:="LookupProperty.GetList")
                        Return aList
                    End If
                Case Else
                    CoreMessageHandler(message:="Property function is not implemented", argument:=_property.ToString, messagetype:=otCoreMessageType.InternalError, _
                                       procedure:="LookupProperty.GetList")
                    Return aList
            End Select
        End Function
        ''' <summary>
        ''' returns the enumeration value
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToEnum() As otLookupProperty
            Return AbstractPropertyFunction(Of otLookupProperty).ToEnum(_property)
        End Function
    End Class
    ''' <summary>
    ''' Enumeration of the validation properties
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otLookupProperty
        <Description(LookupProperty.UseAttributeReference)> UseAttributeReference = 1
        <Description(LookupProperty.UseForeignKey)> UseForeignKey
        <Description(LookupProperty.UseObjectEntry)> UseObjectEntry
        <Description(LookupProperty.UseAttributeValues)> UseAttributeValues
        <Description(LookupProperty.UseVALUELIST)> UseValueList

    End Enum


    ''' <summary>
    ''' Render Property Class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RenderProperty
        Inherits AbstractPropertyFunction(Of otRenderProperty)
        Public Const PASSWORD = "PASSWORD"

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="propertystring"></param>
        ''' <remarks></remarks>
        Public Sub New(propertystring As String)
            MyBase.New(propertystring:=propertystring)
        End Sub

        ''' <summary>
        ''' returns the enumeration value
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToEnum() As otRenderProperty
            Return AbstractPropertyFunction(Of otRenderProperty).ToEnum(_property)
        End Function
    End Class
    ''' <summary>
    ''' Enumeration of the validation properties
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otRenderProperty
        <Description(RenderProperty.PASSWORD)> Password

    End Enum
    '*************************************************************************************
    '*************************************************************************************
    ''' <summary>
    ''' ObjectPermission Rule Property
    ''' </summary>
    ''' <remarks></remarks>
    Public Class AccessRightProperty
        Inherits AbstractPropertyFunction(Of otAccessRight)
        '*** ACCESS RIGHTS CONSTANTS
        Public Const ConstARReadonly = "READONLY"
        Public Const ConstARReadUpdate = "READUPDATE"
        Public Const ConstARAlter = "ALTERSCHEMA"
        Public Const ConstARProhibited = "PROHIBITED"

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="propertystring"></param>
        ''' <remarks></remarks>
        Public Sub New(propertystring As String)
            MyBase.New(propertystring:=propertystring)
        End Sub

        Public Sub New([enum] As otAccessRight)
            MyBase.New(property:=[enum])
        End Sub

        ''' <summary>
        ''' returns the enumeration value
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToEnum() As otAccessRight
            Return AbstractPropertyFunction(Of otAccessRight).ToEnum(_property)
        End Function
        ''' <summary>
        ''' Returns a List of Higher Access Rights then the one selected
        ''' </summary>
        ''' <param name="accessrequest"></param>
        ''' <param name="domain" >Domain to validate for</param>
        ''' <param name="Objects" >list of Obejectnames to validate in the domain</param>
        ''' <returns>eturns false if reverification of User is needed or true if currentAccessLevel includes this new request Level</returns>
        ''' <remarks></remarks>

        Public Shared Function GetHigherAccessRequests(ByVal accessrequest As otAccessRight) As List(Of String)

            Dim aResult As New List(Of String)

            If accessrequest = otAccessRight.AlterSchema Then
                aResult.Add(otAccessRight.AlterSchema.ToString)
            End If

            If accessrequest = otAccessRight.ReadUpdateData Then
                aResult.Add(otAccessRight.AlterSchema.ToString)
                aResult.Add(otAccessRight.ReadUpdateData.ToString)
            End If

            If accessrequest = otAccessRight.ReadOnly Then
                aResult.Add(otAccessRight.AlterSchema.ToString)
                aResult.Add(otAccessRight.ReadUpdateData.ToString)
                aResult.Add(otAccessRight.ReadOnly.ToString)
            End If

            Return aResult
        End Function
        ''' <summary>
        ''' shared version of coverrights of who to cover
        ''' </summary>
        ''' <param name="who"></param>
        ''' <param name="covers"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CoverRights(who As AccessRightProperty, covers As AccessRightProperty)
            Return who.CoverRights(covers)
        End Function
        ''' <summary>
        ''' returns true if the accessrightproperty (as request) is covered by this access right
        ''' </summary>
        ''' <param name="accessrightpropery"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CoverRights([accessrightpropery] As AccessRightProperty) As Boolean
            Return CoverRights([accessrightpropery].[Enum])
        End Function
        ''' <summary>
        ''' cover rights and what to cover
        ''' </summary>
        ''' <param name="rights"></param>
        ''' <param name="covers"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CoverRights(rights As otAccessRight, covers As otAccessRight) As Boolean

            If rights = covers Then
                Return True
            ElseIf covers = otAccessRight.[ReadOnly] And (rights = otAccessRight.ReadUpdateData Or rights = otAccessRight.AlterSchema) Then
                Return True
            ElseIf covers = otAccessRight.ReadUpdateData And rights = otAccessRight.AlterSchema Then
                Return True
                ' will never be reached !
            ElseIf covers = otAccessRight.AlterSchema And rights = otAccessRight.AlterSchema Then
                Return True
            Else
                Return False
            End If

        End Function
        ''' <summary>
        ''' returns true if the accessrequest  is covered by this access right
        ''' </summary>
        ''' <param name="accessrightpropery"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CoverRights(accessrequest As otAccessRight) As Boolean
            Return CoverRights(rights:=Me.[Enum], covers:=accessrequest)
        End Function
    End Class
    ''' <summary>
    ''' Enumeration for Access Rights to the database
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otAccessRight
        <Description(AccessRightProperty.ConstARProhibited)> Prohibited = 0
        <Description(AccessRightProperty.ConstARReadonly)> [ReadOnly] = 1
        <Description(AccessRightProperty.ConstARReadUpdate)> ReadUpdateData = 2
        <Description(AccessRightProperty.ConstARAlter)> AlterSchema = 4
    End Enum


End Namespace