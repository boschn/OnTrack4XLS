
REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** CORE HELPER Classes for On Track Database Backend Library
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

Imports System.Reflection
Imports System.ComponentModel
Imports OnTrack.Commons
Imports OnTrack.Database


Namespace OnTrack.Core

    Public Class Shuffle
        '' <summary>
        ''' splits a Ontrack Canonical name of the form [head] '.' | '|' [tail] in head and tail
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function NameSplitter(name As String, Optional ByRef head As String = Nothing, Optional ByRef tail As String = Nothing) As String()
            Dim names As String() = [name].ToUpper.Split(CChar(ConstDelimiter), "."c)
            If names.Count = 1 Then
                head = names(0)
                tail = Nothing
                Return names
            Else
                Dim chain(1) As String
                chain(0) = names(0)
                Dim i As Integer = name.IndexOf("."c)
                Dim j As Integer = name.IndexOf(CChar(ConstDelimiter))
                If i >= 0 AndAlso j >= 0 Then
                    chain(1) = name.ToUpper.Substring(Math.Min(i, j) + 1)
                ElseIf i >= 0 AndAlso j < 0 Then
                    chain(1) = name.ToUpper.Substring(i + 1)
                ElseIf i < 0 AndAlso j >= 0 Then
                    chain(1) = name.ToUpper.Substring(j + 1)
                End If

                head = chain(0)
                tail = chain(1)
                Return chain
            End If
        End Function
        ''' <summary>
        ''' substitutes in a primary key array (of a table) the domainid with the current domainid
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <param name="pkarray"></param>
        ''' <param name="domainid"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SubstituteDomainIDinTablePrimaryKey(ByRef primarykey As ormDatabaseKey, domainid As String, _
                                                                    Optional substitueOnlyNothingDomain As Boolean = True, _
                                                                    Optional runtimeOnly As Boolean = False) As Boolean
            Dim domindex As Integer = -1
            Dim containerID As String = primarykey.ContainerID

            ''' beware of startup and installation
            ''' here the Substitute doesnot work and doesnot make any sense
            ''' 
            If Not runtimeOnly AndAlso Not CurrentSession.IsBootstrappingInstallationRequested AndAlso Not CurrentSession.IsStartingUp AndAlso Not CurrentSession.IsDomainSwitching Then
                Dim aTabledefinition As ormContainerDefinition = CurrentSession.Objects.GetContainerDefinition(id:=containerID)
                If aTabledefinition Is Nothing Then
                    CoreMessageHandler(message:="table definition could not be retrieved", procedure:="Shuffle.SubstituteDomainIDinPKArray", _
                                    argument:=domainid, containerID:=containerID, containerEntryName:=DomainSetting.ConstFNDomainID, messagetype:=otCoreMessageType.InternalError)
                    Return False
                ElseIf Not aTabledefinition.HasDomainBehavior Then
                    ' this might also be called if we donot have domain behavior 
                    'CoreMessageHandler(message:="table definition shows no domainhebahvior -> check it", subname:="Shuffle.SubstituteDomainIDinPKArray", _
                    '                arg1:=domainid, tablename:=tablename, columnname:=DomainSetting.ConstFNDomainID, messagetype:=otCoreMessageType.InternalError)
                    Return True
                End If
                ''' get schema
                Dim aSchema = ot.CurrentOTDBDriver.RetrieveContainerSchema(containerID)

                ''' check if the domain id is part of the primary key
                ''' 
                domindex = aSchema.GetDomainIDPKOrdinal
                If domindex > 0 Then
                    If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID
                    ''' check if the count of the arrays match
                    If primarykey.Count = aSchema.NoPrimaryEntries Then
                        ' set only if nothing is set
                        If primarykey(domindex - 1) Is Nothing OrElse String.IsNullOrWhiteSpace(primarykey(domindex - 1)) Then
                            primarykey(domindex - 1) = UCase(domainid) ' set the domainid
                            ' replace all values if flag is set 
                        ElseIf Not substitueOnlyNothingDomain AndAlso primarykey(domindex - 1) <> UCase(domainid) Then
                            primarykey(domindex - 1) = UCase(domainid)
                        End If
                    Else
                        ''' extend the primary key
                        'ReDim Preserve primarykey(aSchema.NoPrimaryKeyFields - 1)
                        primarykey(domindex - 1) = UCase(domainid) ' set domainid
                    End If
                ElseIf aTabledefinition.HasDomainBehavior Then
                    CoreMessageHandler(message:="domainID is not in primary key although domain behavior is set", procedure:="Shuffle.SubstituteDomainIDinPKArray", _
                                       argument:=domainid, containerID:=containerID, containerEntryName:=Domain.ConstFNDomainID, messagetype:=otCoreMessageType.InternalError)
                End If

                ''' check if nothing is in key
                ''' 
                For i = 0 To primarykey.GetUpperBound(0)
                    If primarykey(i) Is Nothing Then
                        Dim acolumnname As String = aTabledefinition.GetPrimaryKeyColumnNames.ElementAt(i)
                        CoreMessageHandler(message:="part of primary key is nothing", procedure:="Shuffle.SubstituteDomainIDinPKArray", _
                             argument:=i, containerID:=containerID, containerEntryName:=acolumnname, messagetype:=otCoreMessageType.InternalWarning)

                    End If
                Next

                ''' return successful
                ''' 
                Return True
            Else
                ''' do the same but use the attributes since we are bootstrapping or starting up
                ''' 
                Dim aContainerAttribute As iormContainerAttribute = ot.GetContainerAttribute(containerID)
                If aContainerAttribute Is Nothing Then
                    CoreMessageHandler(message:="table attribute could not be retrieved", procedure:="Shuffle.SubstituteDomainIDinPKArray", _
                                    argument:=domainid, containerID:=containerID, containerEntryName:=Domain.ConstFNDomainID, messagetype:=otCoreMessageType.InternalError)
                    Return False
                ElseIf (aContainerAttribute.HasValueAddDomainBehavior AndAlso aContainerAttribute.HasDomainBehavior) Then
                    Dim keynames As String() = aContainerAttribute.PrimaryEntryNames
                    domindex = Array.FindIndex(keynames, Function(s) s.ToLower = Domain.ConstFNDomainID.ToLower)
                    If domindex >= 0 Then
                        If String.IsNullOrEmpty(domainid) Then domainid = CurrentSession.CurrentDomainID

                        If primarykey.Count = keynames.Count Then
                            ' set only if nothing is set
                            If primarykey(domindex) Is Nothing OrElse String.IsNullOrWhiteSpace(primarykey(domindex)) Then
                                primarykey(domindex) = UCase(domainid)
                            ElseIf primarykey(domindex) <> UCase(domainid) Then
                                primarykey(domindex) = UCase(domainid)
                            End If
                        Else
                            'ReDim Preserve primarykey(keynames.Count)
                            primarykey(domindex) = UCase(domainid)
                        End If
                    Else
                        CoreMessageHandler(message:="domainID is not in primary key although domain behavior is set", procedure:="ormDataObject.SubstituteDomainIDinPKArray", _
                                     argument:=domainid, containerID:=containerID, containerEntryName:=Domain.ConstFNDomainID, messagetype:=otCoreMessageType.InternalError)
                        Return False
                    End If
                Else
                    Return True
                End If

                Return True
            End If
            Return True
        End Function
        ''' <summary>
        ''' helper routine to check and fix the primary key on length, datatype and domain substitution
        ''' </summary>
        ''' <param name="pkarray"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ChecknFixPimaryKey(objectid As String, ByRef primarykey As ormDatabaseKey, domainid As String, _
                                                  Optional substitueOnlyNothingDomain As Boolean = True, _
                                                  Optional runtimeOnly As Boolean = False) As Boolean
            Dim aPrimaryTableid As String

            Dim aDescription = ot.GetObjectClassDescriptionByID(id:=objectid)
            If aDescription IsNot Nothing Then aPrimaryTableid = aDescription.PrimaryContainerID

            ''' bring it to length
            'If primarykey Is Nothing OrElse primarykey.Size = 0 OrElse aDescription.PrimaryKeyEntryNames.Count <> primarykey.Size Then
            'ReDim Preserve primarykey(aDescription.PrimaryKeyEntryNames.Count - 1)
            'End If

            ''' Substitute the DomainID
            '''
            SubstituteDomainIDinTablePrimaryKey(primarykey:=primarykey, substitueOnlyNothingDomain:=substitueOnlyNothingDomain, domainid:=domainid, runtimeOnly:=runtimeOnly)

            ''' convert the  key fields
            ''' 
            Dim i As UShort = 0
            For Each aPKName In aDescription.PrimaryKeyEntryNames
                Dim aMappingList = aDescription.GetMappedContainerEntry2FieldInfos(containerEntryName:=aPKName, containerID:=primarykey.ContainerID)

                If aMappingList IsNot Nothing Then
                    For Each aMapping In aMappingList
                        If primarykey(i) Is Nothing Then
                            'do nothing since the event handler to generate a key might be called by an event
                            '
                            'CoreMessageHandler(message:="part of primary key must not be nothing", arg1:=pkarray(i), _
                            '                   objectname:=aDescription.Name, messagetype:=otCoreMessageType.InternalError, _
                            '                   subname:="Shuffle.SubstituteDomainIDInPrimaryKey")
                            'Return False
                        ElseIf Not primarykey(i).GetType.Equals(aMapping.FieldType) Then
                            Dim avalue = primarykey(i)
                            Try
                                primarykey(i) = CTypeDynamic(avalue, aMapping.FieldType)
                            Catch ex As Exception
                                CoreMessageHandler(exception:=ex, argument:=primarykey(i), procedure:="Shuffle.SubstituteDomainIDInPrimaryKey")
                                Return False
                            End Try

                        End If

                    Next
                End If

                ''' increase
                ''' 
                i += 1
            Next
        End Function

    End Class

    ''' <summary>
    ''' Converter Class for ORM Data
    ''' </summary>
    ''' <remarks></remarks>
    'Public Class Converter

    ''' <summary>
    ''' translates an hex integer to argb presentation integer RGB(FF,00,00) = FF but integer = FF0000
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Shared Function Int2ARGB(value As Long) As Long
    '    Dim red, green, blue As Long
    '    blue = value And &HFF&
    '    green = value \ &H100& And &HFF&
    '    red = value \ &H10000 And &HFF&
    '    Return blue * Math.Pow(255, 2) + green * 255 + red
    'End Function

    '''' <summary>
    '''' returns a color value in rgb to system.drawing.color
    '''' </summary>
    '''' <param name="value"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Shared Function RGB2Color(value As Long) As System.Drawing.Color
    '    Dim red, green, blue As Long
    '    red = value And &HFF&
    '    green = value \ &H100& And &HFF&
    '    blue = value \ &H10000 And &HFF&
    '    Return System.Drawing.Color.FromArgb(red:=red, green:=green, blue:=blue)
    'End Function

    '''' <summary>
    '''' returns a color value to hexadecimal (bgr of rgb) 
    '''' </summary>
    '''' <param name="value"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Shared Function Color2RGB(color As System.Drawing.Color) As Long
    '    Dim red, green, blue As Long
    '    blue = color.B
    '    green = color.G
    '    red = color.R
    '    Return blue * Math.Pow(255, 2) + green * 255 + red
    'End Function
    ''' <summary>
    ''' Converts String to Array
    ''' </summary>
    ''' <param name="input"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Shared Function otString2Array(input As String) As String()
    '    otString2Array = SplitMultbyChar(text:=input, DelimChar:=ConstDelimiter)
    '    If Not IsArrayInitialized(otString2Array) Then
    '        Return New String() {}
    '    Else
    '        Return otString2Array
    '    End If
    'End Function
    ''' <summary>
    ''' Converts Array to String in otdb Array representation
    ''' </summary>
    ''' <param name="input"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Shared Function Array2otString(input() As Object) As String
    '    Dim i As Integer
    '    If IsArrayInitialized(input) Then
    '        Dim aStrValue As String = String.Empty
    '        For i = LBound(input) To UBound(input)
    '            If i = LBound(input) Then
    '                aStrValue = ConstDelimiter & CStr(input(i)) & ConstDelimiter
    '            Else
    '                aStrValue = aStrValue & CStr(input(i)) & ConstDelimiter
    '            End If
    '        Next i
    '        Return aStrValue
    '    Else
    '        Return String.Empty
    '    End If
    'End Function
    ''' <summary>
    ''' Converts Array to String in list representation
    ''' </summary>
    ''' <param name="input"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Shared Function Array2StringList(input() As Object, Optional delimiter As Char = ","c) As String
    '    Dim i As Integer
    '    If IsArrayInitialized(input) Then
    '        Dim aStrValue As String = String.Empty
    '        For i = LBound(input) To UBound(input)
    '            If i = LBound(input) Then
    '                aStrValue = CStr(input(i))
    '            Else
    '                aStrValue &= delimiter & CStr(input(i))
    '            End If
    '        Next i
    '        Return aStrValue
    '    Else
    '        Return String.Empty
    '    End If
    'End Function
    ''' <summary>
    ''' Converts iEnumerable to String in otdb Array representation
    ''' </summary>
    ''' <param name="input"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Shared Function Enumerable2otString(input As IEnumerable) As String
    '    Dim aStrValue As String = String.Empty
    '    If input Is Nothing Then Return String.Empty
    '    For Each anElement In input
    '        Dim s As String
    '        If anElement Is Nothing Then
    '            s = String.Empty
    '        Else
    '            s = anElement.ToString
    '        End If


    '        If aStrValue = String.Empty Then
    '            aStrValue = ConstDelimiter & s & ConstDelimiter
    '        Else
    '            aStrValue &= s & ConstDelimiter
    '        End If
    '    Next
    '    Return aStrValue
    'End Function
    ''' <summary>
    ''' Converts iEnumerable to String in list representation
    ''' </summary>
    ''' <param name="input"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Shared Function Enumerable2StringList(input As IEnumerable, Optional delimiter As Char = ","c) As String
    '    Dim aStrValue As String = String.Empty
    '    If input Is Nothing Then Return String.Empty
    '    For Each anElement In input
    '        Dim s As String
    '        If anElement Is Nothing Then
    '            s = String.Empty
    '        Else
    '            s = anElement.ToString
    '        End If


    '        If aStrValue = String.Empty Then
    '            aStrValue = s
    '        Else
    '            aStrValue &= delimiter & s
    '        End If
    '    Next
    '    Return aStrValue
    'End Function
    ''' <summary>
    ''' converts a object  to an object of OnTrack DB Type.
    ''' sets the flag failed if the output is an assumption
    ''' </summary>
    ''' <param name="input"></param>
    ''' <param name="datatype"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Shared Function Object2otObject(input As Object, datatype As otDataType, Optional isnullable As Boolean = False, Optional ByRef failed As Boolean = False) As Object


    '    Try

    '        ''' check if the type is of nullable
    '        If input IsNot Nothing AndAlso Reflector.IsNullable(input.GetType) Then
    '            input = CTypeDynamic(input, Nullable.GetUnderlyingType(input.GetType))

    '        End If

    '        ''' reflect is nullable
    '        If input Is Nothing AndAlso isnullable Then
    '            failed = False
    '            Return Nothing
    '        End If

    '        Select Case datatype
    '            Case otDataType.Bool
    '                If input Is Nothing AndAlso Not isnullable Then
    '                    failed = True
    '                    Return False
    '                ElseIf input Is Nothing AndAlso Not isnullable Then
    '                    failed = False
    '                    Return Nothing
    '                ElseIf input.GetType Is GetType(Boolean) Then
    '                    failed = False
    '                    Return input
    '                ElseIf IsNumeric(input) Then
    '                    If CDbl(input) = 0 Then
    '                        Return False
    '                    ElseIf CDbl(input) = 1 Then
    '                        failed = False
    '                        Return True
    '                    ElseIf CDbl(input) > 0 Then
    '                        failed = True
    '                        Return True
    '                    Else
    '                        failed = True
    '                        Return False
    '                    End If
    '                ElseIf String.IsNullOrWhiteSpace(input) Then
    '                    failed = True
    '                    Return False
    '                ElseIf input.Trim.ToUpper = "TRUE" OrElse input.Trim.ToUpper = "YES" Then
    '                    failed = False
    '                    Return True
    '                ElseIf input.Trim.ToUpper = "FALSE" OrElse input.Trim.ToUpper = "NO" Then
    '                    failed = False
    '                    Return False
    '                Else
    '                    failed = True
    '                    Return CBool(input)
    '                End If

    '            Case otDataType.Long
    '                If input Is Nothing Then
    '                    failed = True
    '                    Return CLng(0)
    '                ElseIf input.GetType Is GetType(Long) OrElse input.GetType Is GetType(ULong) OrElse input.GetType Is GetType(UShort) OrElse input.GetType Is GetType(Short) Then
    '                    failed = False
    '                    Return CLng(input)
    '                ElseIf IsNumeric(input) AndAlso Math.Floor(CDbl(input)) = Math.Ceiling(CDbl(input)) Then
    '                    failed = False
    '                    Return CLng(input)
    '                ElseIf IsNumeric(input) Then
    '                    failed = True
    '                    Return CLng(input)
    '                Else
    '                    failed = True
    '                    Return CLng(0)
    '                End If

    '            Case otDataType.Numeric
    '                If input Is Nothing Then
    '                    failed = True
    '                    Return CDbl(0)
    '                ElseIf input.GetType Is GetType(Double) Then
    '                    failed = False
    '                    Return CDbl(input)
    '                ElseIf IsNumeric(input) Then
    '                    failed = False
    '                    Return CDbl(input)
    '                Else
    '                    failed = True
    '                    Return CDbl(0)
    '                End If
    '            Case otDataType.List
    '                If input Is Nothing Then
    '                    failed = True
    '                ElseIf input.GetType.IsArray Then
    '                    failed = False
    '                    Return Core.DataType.ToString(input)
    '                ElseIf Not input.ToString.Contains(ConstDelimiter) Then
    '                    failed = False
    '                    Return ConstDelimiter & input.ToString & ConstDelimiter
    '                Else
    '                    failed = False
    '                    If input.ToString.First <> ConstDelimiter Then input = ConstDelimiter & input.ToString
    '                    If input.ToString.Last <> ConstDelimiter Then input = input.ToString & ConstDelimiter
    '                    Return input.ToString
    '                End If

    '            Case otDataType.Memo, otDataType.Text
    '                If input Is Nothing Then
    '                    failed = True
    '                    Return String.Empty
    '                Else
    '                    failed = False
    '                    Return input.ToString
    '                End If

    '            Case otDataType.Date, otDataType.Timestamp
    '                If input Is Nothing OrElse Not IsDate(input) Then
    '                    failed = True
    '                    Return constNullDate
    '                ElseIf input.GetType Is GetType(Date) OrElse input.GetType Is GetType(DateTime) Then
    '                    failed = False
    '                    Return CDate(input)
    '                Else
    '                    failed = False
    '                    Return CDate(input)
    '                End If

    '            Case otDataType.Time
    '                If input Is Nothing OrElse Not IsDate(input) Then
    '                    failed = True
    '                    Return ConstNullTime
    '                ElseIf input.GetType Is GetType(Date) OrElse input.GetType Is GetType(DateTime) Then
    '                    failed = False
    '                    Return CDate(input)
    '                Else
    '                    failed = False
    '                    Return CDate(input)
    '                End If

    '            Case Else
    '                CoreMessageHandler(message:="Datatype is not implemented in this routine", procedure:="Converter:object2otObject", argument:=datatype, _
    '                                    messagetype:=otCoreMessageType.InternalError)
    '                failed = True
    '                Return Nothing
    '        End Select

    '    Catch ex As Exception
    '        CoreMessageHandler(exception:=ex, procedure:="Converter.Object2OTObject")
    '        failed = True
    '        Return Nothing
    '    End Try
    'End Function

    '''' <summary>
    '''' return a timestamp in the localTime
    '''' </summary>
    '''' <param name="datevalue"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Shared Function DateTime2LocaleDateTimeString(datevalue As DateTime) As String
    '    Dim formattimestamp As String = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern & " " & System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern
    '    Return Format(datevalue, formattimestamp)
    'End Function

    '''' <summary>
    '''' return a date in the date localTime
    '''' </summary>
    '''' <param name="datevalue"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Shared Function DateTime2UniversalDateTimeString(datevalue As DateTime) As String
    '    Dim formattimestamp As String = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.UniversalSortableDateTimePattern
    '    Return Format(datevalue, formattimestamp)
    'End Function
    '''' <summary>
    '''' return a date in the date localTime
    '''' </summary>
    '''' <param name="datevalue"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Shared Function Date2LocaleShortDateString(datevalue As Date) As String
    '    Dim formattimestamp As String = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern
    '    Return Format(datevalue, formattimestamp)
    'End Function
    '''' <summary>
    '''' return a date in the date localTime
    '''' </summary>
    '''' <param name="datevalue"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Shared Function Time2LocaleShortTimeString(timevalue As DateTime) As String
    '    Dim formattimestamp As String = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern
    '    Return Format(timevalue, formattimestamp)
    'End Function

    'End Class

    ''' <summary>
    ''' Reflector Class for reflecting ORM Attributes
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Reflector

        ''' <summary>
        ''' returns true if the type implements a generic interface of interfacetype
        ''' </summary>
        ''' <param name="type"></param>
        ''' <param name="interfacetype"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function TypeImplementsGenericInterface(ByVal [type] As Type, ByVal interfacetype As Type) As Boolean
            '            if (myType.IsInterface && myType.IsGenericType &&  myType.GetGenericTypeDefinition () == typeof (IList<>)) 
            '                   return myType.GetGenericArguments ()[0] ; 

            '           foreach (var i in myType.GetInterfaces ())
            '                               if (i.IsGenericType && i.GetGenericTypeDefinition () == typeof (IList<>))
            '                                   return i.GetGenericArguments ()[0] ;

            For Each anInterface In type.GetInterfaces
                If anInterface.IsGenericType AndAlso anInterface.GetGenericTypeDefinition.Equals(interfacetype) Then
                    Return True
                End If
            Next

            Return False
        End Function


        ''' <summary>
        ''' returns true if the type is nullable or string (which is also nullable)
        ''' </summary>
        ''' <param name="myType"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function IsNullableTypeOrString(ByVal [type] As Type) As Boolean
            Return ([type] Is GetType(String)) OrElse ([type].IsGenericType) AndAlso ([type].GetGenericTypeDefinition() Is GetType(Nullable(Of )))
        End Function

        ''' <summary>
        ''' returns true if the type is nullable
        ''' </summary>
        ''' <param name="myType"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function IsNullable(ByVal [type] As Type) As Boolean
            Return ([type].IsGenericType) AndAlso ([type].GetGenericTypeDefinition() Is GetType(Nullable(Of )))
        End Function
        ''' <summary>
        ''' create a IList from a Type
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateInstanceOfIlist(type As System.Type) As IList
            Dim aGenricType As System.Type = GetType(List(Of )).MakeGenericType(type)
            Dim aListInstance As Object = Activator.CreateInstance(aGenricType)
            Return aListInstance
        End Function

        ''' <summary>
        ''' returns ORM Attributes out of a Type
        ''' </summary>
        ''' <param name="ormType"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetAttributes(ormType As Type) As List(Of System.Attribute)
            Dim aFieldList As System.Reflection.FieldInfo()
            Dim anAttributeList As New List(Of System.Attribute)

            Try
                '***
                '*** collect all the attributes first
                '***
                aFieldList = ormType.GetFields(Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic Or _
                                                  Reflection.BindingFlags.Public Or Reflection.BindingFlags.Static Or _
                                                  Reflection.BindingFlags.FlattenHierarchy)
                '** look into each  Type (Fields)
                For Each aFieldInfo As System.Reflection.FieldInfo In aFieldList

                    If aFieldInfo.MemberType = Reflection.MemberTypes.Field Then
                        '** Attributes
                        For Each anAttribute As System.Attribute In Attribute.GetCustomAttributes(aFieldInfo)
                            '** TABLE
                            If anAttribute.GetType().Equals(GetType(ormTableAttribute)) Then
                                '* set the tablename
                                DirectCast(anAttribute, ormTableAttribute).TableID = aFieldInfo.GetValue(Nothing).ToString
                                anAttributeList.Add(anAttribute)
                                '** FIELD COLUMN
                            ElseIf anAttribute.GetType().Equals(GetType(iormObjectEntryDefinition)) Then
                                '* set the cloumn name
                                DirectCast(anAttribute, ormObjectEntryAttribute).ContainerEntryName = aFieldInfo.GetValue(Nothing).ToString

                                anAttributeList.Add(anAttribute)
                                '** INDEX
                            ElseIf anAttribute.GetType().Equals(GetType(ormIndexAttribute)) Then
                                '* set the index name
                                DirectCast(anAttribute, ormIndexAttribute).IndexName = aFieldInfo.GetValue(Nothing).ToString

                                anAttributeList.Add(anAttribute)
                            End If
                        Next
                    End If
                Next

                Return anAttributeList

            Catch ex As Exception

                Call CoreMessageHandler(procedure:="Reflector.GetAttribute", exception:=ex)
                Return anAttributeList

            End Try


        End Function



        ''' <summary>
        ''' returns ORM Attributes out of a Type
        ''' </summary>
        ''' <param name="ormType"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetColumnAttribute(ormType As Type, columnName As String) As System.Attribute
            Dim aFieldList As System.Reflection.FieldInfo()

            Try
                '***
                '*** collect all the attributes first
                '***
                aFieldList = ormType.GetFields(Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic Or _
                                                  Reflection.BindingFlags.Public Or Reflection.BindingFlags.Static Or _
                                                  Reflection.BindingFlags.FlattenHierarchy)
                '** look into each  Type (Fields)
                For Each aFieldInfo As System.Reflection.FieldInfo In aFieldList

                    If aFieldInfo.MemberType = Reflection.MemberTypes.Field Then
                        '** Attributes
                        For Each anAttribute As System.Attribute In Attribute.GetCustomAttributes(aFieldInfo)
                            ''' Column
                            If anAttribute.GetType().Equals(GetType(iormObjectEntryDefinition)) Then
                                If aFieldInfo.GetValue(Nothing).ToString.ToUpper = columnName.ToUpper Then
                                    '* set the column name
                                    DirectCast(anAttribute, ormObjectEntryAttribute).ContainerEntryName = aFieldInfo.GetValue(Nothing).ToString

                                    Return anAttribute
                                End If
                            End If
                        Next
                    End If
                Next

                Return Nothing

            Catch ex As Exception

                Call CoreMessageHandler(procedure:="Reflector.GetColumnAttribute", exception:=ex)
                Return Nothing

            End Try


        End Function


        ''' <summary>
        ''' returns ORM Attributes out of a Type
        ''' </summary>
        ''' <param name="ormType"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetIndexAttribute(ormType As Type, indexName As String) As System.Attribute
            Dim aFieldList As System.Reflection.FieldInfo()

            Try
                '***
                '*** collect all the attributes first
                '***
                aFieldList = ormType.GetFields(Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic Or _
                                                  Reflection.BindingFlags.Public Or Reflection.BindingFlags.Static Or _
                                                  Reflection.BindingFlags.FlattenHierarchy)
                '** look into each  Type (Fields)
                For Each aFieldInfo As System.Reflection.FieldInfo In aFieldList

                    If aFieldInfo.MemberType = Reflection.MemberTypes.Field Then
                        '** Attributes
                        For Each anAttribute As System.Attribute In Attribute.GetCustomAttributes(aFieldInfo)
                            ''' Index
                            If anAttribute.GetType().Equals(GetType(ormIndexAttribute)) Then
                                If aFieldInfo.GetValue(Nothing).ToString.ToUpper = indexName.ToUpper Then
                                    '* set the index name
                                    DirectCast(anAttribute, ormIndexAttribute).IndexName = aFieldInfo.GetValue(Nothing).ToString

                                    Return anAttribute
                                End If
                            End If
                        Next
                    End If
                Next

                Return Nothing

            Catch ex As Exception

                Call CoreMessageHandler(procedure:="Reflector.GetIndexAttribute", exception:=ex)
                Return Nothing

            End Try

        End Function
        ''' <summary>
        ''' retrieves a list of related objects from a relation attribute for a object class described by a classdescriptor
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="classdescriptor"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetContainerEntryValues(dataobject As iormRelationalPersistable, Optional entrynames As String() = Nothing) As List(Of Object)
            Dim aDescriptor As ObjectClassDescription = ot.GetObjectClassDescription(dataobject.GetType)
            Dim aList As New List(Of Object)
            If aDescriptor Is Nothing Then
                CoreMessageHandler(message:="Class Description not found for data object", argument:=dataobject.GetType.Name, _
                                   procedure:="Reflector.Getvalues", messagetype:=otCoreMessageType.InternalError)
                Return aList
            End If
            If entrynames Is Nothing Then
                entrynames = aDescriptor.Entrynames.ToArray
            End If

            '*** get the values in the order of the entrynames
            For Each anEntryname In entrynames
                Dim anObjectEntry = aDescriptor.GetObjectEntryAttribute(entryname:=anEntryname)
                If anObjectEntry IsNot Nothing AndAlso anObjectEntry.HasValueContainerEntryName AndAlso anObjectEntry.HasValueContainerID Then
                    Dim aFieldlist = aDescriptor.GetMappedContainerEntry2FieldInfos(containerEntryName:=anObjectEntry.ContainerEntryName, _
                                                                           containerID:=anObjectEntry.ContainerID)
                    If aFieldlist IsNot Nothing AndAlso aFieldlist.Count > 0 Then
                        Dim aValue As Object
                        '** get value by hook or slooow
                        If Not Reflector.GetFieldValue(aFieldlist.First, dataobject, aValue) Then
                            aValue = aFieldlist.First.GetValue(dataobject)
                        End If

                        aList.Add(aValue)
                    ElseIf aFieldlist Is Nothing OrElse aFieldlist.Count = 0 Then
                        CoreMessageHandler(message:="Object Entry not mapped to a FieldMember of the class ", _
                                       argument:=dataobject.GetType.Name, entryname:=anEntryname, objectname:=dataobject.ObjectID, _
                                       procedure:="Reflector.Getvalues", messagetype:=otCoreMessageType.InternalWarning)
                    Else
                        CoreMessageHandler(message:="Object Entry mapped to multiple FieldMember of the class - first one taken ", _
                                       argument:=dataobject.GetType.Name, entryname:=anEntryname, objectname:=dataobject.ObjectID, _
                                       procedure:="Reflector.Getvalues", messagetype:=otCoreMessageType.InternalWarning)
                    End If

                ElseIf anObjectEntry Is Nothing Then
                    CoreMessageHandler(message:="Object Entry not found in Class Description ", _
                                       argument:=dataobject.GetType.Name, entryname:=anEntryname, objectname:=dataobject.ObjectID, _
                                       procedure:="Reflector.Getvalues", messagetype:=otCoreMessageType.InternalError)
                ElseIf Not anObjectEntry.HasValueContainerEntryName OrElse Not anObjectEntry.HasValueContainerID Then
                    CoreMessageHandler(message:="Class Description Object Entry has no tablename or columnname ", _
                                       argument:=dataobject.GetType.Name, entryname:=anEntryname, objectname:=dataobject.ObjectID, _
                                       procedure:="Reflector.Getvalues", messagetype:=otCoreMessageType.InternalError)
                End If
            Next

            Return aList
        End Function


        ''' <summary>
        ''' set the member field value with conversion of a dataobject
        ''' </summary>
        ''' <param name="field"></param>
        ''' <param name="dataobject"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SetFieldValue(field As FieldInfo, dataobject As iormRelationalPersistable, value As Object) As Boolean

            Try
                Dim converter As TypeConverter = TypeDescriptor.GetConverter(field.FieldType)
                Dim aClassDescription = dataobject.ObjectClassDescription 'ot.GetObjectClassDescription(dataobject.GetType)
                If aClassDescription Is Nothing Then
                    CoreMessageHandler(message:="class description of object could not be retrieved", objectname:=dataobject.ObjectID, argument:=field.Name, _
                                       procedure:="Reflector.SetFieldValue", messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If
                Dim aSetter = aClassDescription.GetFieldMemberSetterDelegate(field.Name)
                If aSetter Is Nothing Then
                    CoreMessageHandler(message:="setter delegate of object could not be retrieved - field.setvalue will be used", objectname:=dataobject.ObjectID, argument:=field.Name, _
                                       procedure:="Reflector.SetFieldValue", messagetype:=otCoreMessageType.InternalError)
                End If

                ''' if we have a null somehow
                ''' 
                If DBNull.Value.Equals(value) AndAlso Reflector.IsNullableTypeOrString(field.FieldType) Then
                    value = Nothing
                End If

                ''' determine the targettype
                Dim targettype As System.Type
                If Reflector.IsNullable(field.FieldType) Then
                    targettype = Nullable.GetUnderlyingType(field.FieldType)
                Else
                    targettype = field.FieldType
                End If

                ' do nothing leave the value
                If value Is Nothing Then
                    If aSetter IsNot Nothing Then
                        aSetter(dataobject, value)
                    Else
                        field.SetValue(dataobject, value)
                    End If
                ElseIf targettype.IsArray Then
                    Dim aStringArray As String()
                    If value IsNot Nothing AndAlso value.GetType.IsArray Then
                        aStringArray = value
                    Else
                        aStringArray = Core.DataType.ToArray(value)
                    End If

                    If targettype.GetElementType Is GetType(Long) Then
                        Dim aLongArray As Long()
                        Dim i As Integer
                        ReDim aLongArray(aStringArray.Length - 1)
                        For Each aValue In aStringArray
                            aLongArray(i) = CLng(aValue)
                            i += 1
                        Next
                        If aSetter IsNot Nothing Then
                            aSetter(dataobject, aLongArray)
                        Else
                            field.SetValue(dataobject, aLongArray)
                        End If
                    ElseIf targettype.GetElementType Is GetType(String) Then
                        '' no need to transfer
                        If aSetter IsNot Nothing Then
                            aSetter(dataobject, aStringArray)
                        Else
                            field.SetValue(dataobject, aStringArray)
                        End If

                    Else
                        Dim anArray As Object = Array.CreateInstance(targettype.GetElementType, aStringArray.Length)
                        Dim i As Integer
                        For Each aValue In aStringArray
                            anArray(i) = CTypeDynamic(aValue, targettype.GetElementType)
                            i += 1
                        Next
                        If aSetter IsNot Nothing Then
                            aSetter(dataobject, anArray)
                        Else
                            field.SetValue(dataobject, anArray)
                        End If
                    End If

                    '''
                    ''' setter for all types of list interfaces
                    ''' 
                ElseIf targettype.GetInterfaces.Contains(GetType(IList)) Then
                    Dim anArray As String()
                    Dim aList As Object
                    If value.GetType.IsArray Then
                        anArray = value
                        If anArray.Count = 0 Then
                            aList = Reflector.CreateInstanceOfIlist(targettype.GetGenericArguments.First)
                        Else
                            aList = anArray.ToList
                        End If
                    ElseIf value.GetType.GetInterfaces.Contains(GetType(IList)) Then
                        ''' make sure that the inner type of the list 
                        ''' are casted as well before we pass it
                        Dim innertype As System.Type = value.GetType.GetGenericArguments.First
                        aList = Reflector.CreateInstanceOfIlist(targettype.GetGenericArguments.First)
                        For i = 0 To DirectCast(value, IList).Count - 1
                            '' try to cast
                            Dim item As Object = CTypeDynamic(DirectCast(value, IList).Item(i), innertype)
                            TryCast(aList, IList).Add(item)
                        Next

                    ElseIf value.GetType.Equals(GetType(String)) Then
                        anArray = Core.DataType.ToArray(value)
                        If anArray.Count = 0 Then
                            aList = New List(Of String) 'HACK ! this should be of generic type of the field
                        Else
                            aList = anArray.ToList
                        End If

                    Else
                        CoreMessageHandler(message:="Type is not convertable to ILIST", procedure:="Reflector.SetFieldValue", messagetype:=otCoreMessageType.InternalError, _
                                           entryname:=field.Name, containerID:=dataobject.ObjectPrimaryTableID, _
                                           argument:=field.Name)

                    End If

                    ''' set the value
                    ''' 
                    If aSetter IsNot Nothing Then
                        aSetter(dataobject, aList)
                    Else
                        field.SetValue(dataobject, aList)
                    End If

                ElseIf value Is Nothing OrElse targettype.Equals(value.GetType) Then
                    If aSetter IsNot Nothing Then
                        aSetter(dataobject, value)
                    Else
                        field.SetValue(dataobject, value)
                    End If

                ElseIf targettype.IsEnum Then
                    Dim anewValue As Object
                    If value.GetType.Equals(GetType(String)) Then
                        '* transform
                        anewValue = CTypeDynamic([Enum].Parse(field.FieldType, value, ignoreCase:=True), field.FieldType)
                    Else
                        anewValue = CTypeDynamic(value, field.FieldType)
                    End If

                    If aSetter IsNot Nothing Then
                        aSetter(dataobject, anewValue)
                    Else
                        field.SetValue(dataobject, anewValue)
                    End If
                ElseIf converter.CanConvertFrom(value.GetType) Then
                    Dim anewvalue As Object = converter.ConvertFrom(value)
                    If aSetter IsNot Nothing Then
                        aSetter(dataobject, anewvalue)
                    Else
                        field.SetValue(dataobject, anewvalue)
                    End If
                ElseIf targettype.Equals(GetType(Long)) AndAlso IsNumeric(value) Then
                    If aSetter IsNot Nothing Then
                        aSetter(dataobject, CLng(value))
                    Else
                        field.SetValue(dataobject, CLng(value))
                    End If
                ElseIf targettype.Equals(GetType(Boolean)) Then
                    If aSetter IsNot Nothing Then
                        aSetter(dataobject, CBool(value))
                    Else
                        field.SetValue(dataobject, CBool(value))
                    End If

                ElseIf targettype.Equals(GetType(String)) Then
                    If aSetter IsNot Nothing Then
                        aSetter(dataobject, CStr(value))
                    Else
                        field.SetValue(dataobject, CStr(value))
                    End If
                    field.SetValue(dataobject, CStr(value))
                ElseIf targettype.Equals(GetType(Integer)) AndAlso IsNumeric(value) Then
                    If aSetter IsNot Nothing Then
                        aSetter(dataobject, CInt(value))
                    Else
                        field.SetValue(dataobject, CInt(value))
                    End If

                ElseIf targettype.Equals(GetType(UInteger)) AndAlso IsNumeric(value) _
                    AndAlso value >= UInteger.MinValue AndAlso value <= UInteger.MaxValue Then
                    If aSetter IsNot Nothing Then
                        aSetter(dataobject, CUInt(value))
                    Else
                        field.SetValue(dataobject, CUInt(value))
                    End If
                ElseIf targettype.Equals(GetType(UShort)) And IsNumeric(value) _
                    AndAlso value >= UShort.MinValue AndAlso value <= UShort.MaxValue Then
                    If aSetter IsNot Nothing Then
                        aSetter(dataobject, CUShort(value))
                    Else
                        field.SetValue(dataobject, CUShort(value))
                    End If
                ElseIf targettype.Equals(GetType(ULong)) And IsNumeric(value) _
                     AndAlso value >= ULong.MinValue AndAlso value <= ULong.MaxValue Then
                    If aSetter IsNot Nothing Then
                        aSetter(dataobject, CULng(value))
                    Else
                        field.SetValue(dataobject, CULng(value))
                    End If

                ElseIf targettype.Equals(GetType(Double)) And IsNumeric(value) _
                    AndAlso value >= Double.MinValue AndAlso value <= Double.MaxValue Then
                    If aSetter IsNot Nothing Then
                        aSetter(dataobject, CDbl(value))
                    Else
                        field.SetValue(dataobject, CDbl(value))
                    End If
                ElseIf targettype.Equals(GetType(Decimal)) And IsNumeric(value) _
                  AndAlso value >= Decimal.MinValue AndAlso value <= Decimal.MaxValue Then
                    If aSetter IsNot Nothing Then
                        aSetter(dataobject, CDec(value))
                    Else
                        field.SetValue(dataobject, CDec(value))
                    End If
                ElseIf targettype.Equals(GetType(Object)) Then
                    If aSetter IsNot Nothing Then
                        aSetter(dataobject, value)
                    Else
                        field.SetValue(dataobject, value)
                    End If
                Else
                    Call CoreMessageHandler(procedure:="ormDataObject.infuse", message:="cannot convert record value type to field type", _
                                           entryname:=field.Name, containerID:=dataobject.ObjectPrimaryTableID, _
                                           argument:=field.Name, messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If
                'End SyncLock

                Return True

            Catch ex As Exception

                CoreMessageHandler(exception:=ex, procedure:="Reflector.SetFieldValue", argument:=value, entryname:=field.Name, objectname:=dataobject.ObjectID)
                Return False
            End Try


        End Function
        ''' <summary>
        ''' set the member field value with conversion of a dataobject
        ''' </summary>
        ''' <param name="field"></param>
        ''' <param name="dataobject"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetFieldValue(field As FieldInfo, dataobject As iormRelationalPersistable, ByRef value As Object) As Boolean

            Try
                'Dim converter As TypeConverter = TypeDescriptor.GetConverter(field.FieldType)
                Dim aClassDescription = dataobject.ObjectClassDescription 'ot.GetObjectClassDescription(dataobject.GetType)
                If aClassDescription Is Nothing Then
                    CoreMessageHandler(message:="class description of object could not be retrieved", objectname:=dataobject.ObjectID, argument:=field.Name, _
                                       procedure:="Reflector.GetFieldValue", messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If
                Dim aGetter = aClassDescription.GetFieldMemberGetterDelegate(field.Name)
                If aGetter Is Nothing Then
                    CoreMessageHandler(message:="setter delegate of object could not be retrieved", objectname:=dataobject.ObjectID, argument:=field.Name, _
                                      procedure:="Reflector.GetFieldValue", messagetype:=otCoreMessageType.InternalError)
                    Return False
                End If

                value = aGetter(dataobject)

                Return True

            Catch ex As Exception

                CoreMessageHandler(exception:=ex, procedure:="Reflector.GetFieldValue", argument:=value, entryname:=field.Name, objectname:=dataobject.ObjectID)
                Return False
            End Try


        End Function
    End Class

End Namespace
