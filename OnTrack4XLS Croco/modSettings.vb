

REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE TOOLING FOR EXCEL
REM ***********
REM *********** Module to handle Properties to Host Application (Office Document Properties)
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
Imports OnTrack

Imports Microsoft.Office.Interop.Excel

Public Module modSettings



    '************
    '************ PropertyExistsinHost return true if the Name <aName> exists in the Project aProject
    '************

#If ProjectVersion Then
Function HostPropertyExists(ByVal aProject As Project, ByVal aName As String) As Boolean

    On Error GoTo errorhandle

    For Each prop In aProject.CustomDocumentProperties
     Try
            For Each prop In aProject.CustomDocumentProperties
                If prop.Name = aName Then
                    HostPropertyExists = True
                    Exit Function
                End If
            Next prop

            HostPropertyExists = False
            Exit Function
        Catch ex As Exception
            HostPropertyExists = False
            Exit Function
        End Try

End function

#End If


    Function HostPropertyExists(ByVal aWorkbook As Workbook, ByVal aName As String) As Boolean

        Try
            For Each prop As Object In aWorkbook.CustomDocumentProperties
                If prop.Name = aName Then
                    HostPropertyExists = True
                    Exit Function
                End If
            Next prop

            HostPropertyExists = False
            Exit Function
        Catch ex As Exception
            HostPropertyExists = False
            Exit Function
        End Try

    End Function


    '************************************************************************
    ' getPropertyByName : returns Parameter by Name in the order XLS, Project, Worksheet
    '                      Fetches errors like Worksheet is missing or ParameterName not Defined
    ' Optional aHost to look in
    ' Optional found to indicate if parameter exists -> return value
    ' silent if true than do not issue error
    ' returns a object
#If ProjectVersion Then
    Function GetHostProperty(ByVal aName As String, _
                               Optional ByRef aHost As Project.Workbook = Nothing, _
                               Optional ByRef found As Boolean = False, _
                               Optional ByVal silent As Boolean = True) As Object
        Dim namedarea As Object
        Dim prop As Microsoft.Office.Core.DocumentProperty

        Dim Value As Object
        Dim parametername_flag As Boolean

     If IsMissing(aHost) Then
        If Not ourGlobalProject Is Nothing Then
            Set aHost = ourGlobalProject
        Else
            Set aHost = Application.ActiveProject
        End If
    End If
    parametername_flag = HostPropertyExists(aHost, aName)
        If parametername_flag Then
            For Each prop In aHost.CustomDocumentProperties
                If prop.Name = aName Then
                    GetHostProperty = CStr(prop.Value)
                    found = True
                    Exit Function
                End If
            Next prop
            For Each prop In aHost.DocumentProperties
                If prop.Name = aName Then
                    GetHostProperty = CStr(prop.Value)
                    found = True
                    Exit Function
                End If
            Next prop
        Else
            If Not silent Then
                Call OTDBErrorHandler(SHOWMSGBOX:=True, _
                                      message:="The parameter '" & aName & " ' is not found in this HostApplicaiton '" & aHost.Name & "'!", procedure:="modSettings.getPropertyByName")
                'Debug.Print "FATAL ERROR: The parameter '" & aName & "' is not found in this Project !"
            End If
            GetHostProperty = ""
            found = False
            Exit Function
        End If
    end function

#End If

    ''' <summary>
    ''' retrieves an Office Document Property
    ''' </summary>
    ''' <param name="name"></param>
    ''' <param name="host"></param>
    ''' <param name="found"></param>
    ''' <param name="silent"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetHostProperty(ByVal name As String, _
                               Optional ByRef host As Excel.Workbook = Nothing, _
                               Optional ByRef found As Boolean = False, _
                               Optional ByVal silent As Boolean = True) As Object

        Dim parametername_flag As Boolean

        'current Project ?!

        If host Is Nothing Then
            host = Globals.ThisAddIn.Application.ActiveWorkbook
        End If

        If host Is Nothing Then
            Return Nothing
        End If

        parametername_flag = HostPropertyExists(host, name)
        If parametername_flag Then
            For Each prop As Object In host.CustomDocumentProperties
                If prop.Name = name Then
                    GetHostProperty = CStr(prop.Value)
                    found = True
                    Exit Function
                End If
            Next prop
            For Each prop As Object In host.DocumentProperties
                If prop.Name = name Then
                    GetHostProperty = CStr(prop.Value)
                    found = True
                    Exit Function
                End If
            Next prop
        Else
            If Not silent Then
                Call Core.CoreMessageHandler(showmsgbox:=True, messagetype:=Core.otCoreMessageType.ApplicationWarning, _
                                      message:="The parameter '" & name & " ' is not found in this HostApplicaiton '" & host.Name & "'!", procedure:="modSettings.getPropertyByName")
            End If

            found = False
            Return Nothing
        End If

    End Function



    '************************************************************************
    ' setPropertyValueByName : sets the Value of a Parameter on the parameter sheet by the Name supplied
    '
    ' Name: Name of the parameter
    ' Value: Value of the parameter as object
    '
    ' returns a object
#If ProjectVersion Then
     Public Function setHostProperty(ByVal aName As String, _
                                           ByVal value As Object, _
                                           Optional ByRef aHost As Project = Nothing, _
                                           Optional silent As Boolean = True) As Boolean
        Dim parametersheet_flag, parametername_flag As Boolean
        'Dim pn As Name
        Dim flag As Boolean
     Dim prop As Microsoft.Office.Core.DocumentProperty
     'Dim xla As AddIn
        Dim i As Integer

        flag = False

    If IsMissing(aHost) Then
        If Not ourGlobalProject Is Nothing Then
            Set aHost = ourGlobalProject
        Else
            Set aHost = Application.ActiveProject
        End If
    End If

     Try
            ' search in Project if not found on parameter sheet
            If Not parametername_flag Then
                parametername_flag = HostPropertyExists(aHost, aName)
                If parametername_flag Then
                    For Each prop In aHost.CustomDocumentProperties
                        If prop.Name = aName Then
                            prop.Value = value
                            setHostProperty = True
                            Exit Function
                        End If
                    Next prop
                    For Each prop In aHost.DocumentProperties
                        If prop.Name = aName Then
                            prop.Value = value
                            setHostProperty = True
                            Exit Function
                        End If
                    Next prop

                Else
                    aHost.CustomDocumentProperties.add(Name:=aName, LinkToContent:=False, _
                                                       type:=Office.MsoDocProperties.msoPropertyTypeString, Value:=value)

                    setHostProperty = True
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Call OTDBErrorHandler(SHOWMSGBOX:=True, _
                              message:="The parameter '" & aName & " ' cannot be written to " & aName & "!", _
                              procedure:="modSettings.setpropertyValueByName")

            setHostProperty = False
            Exit Function
        End Try

        setHostProperty = True
        Exit Function
    End Function

#End If

    ''' <summary>
    ''' Sets a Office Document Property
    ''' </summary>
    ''' <param name="name"></param>
    ''' <param name="value"></param>
    ''' <param name="host"></param>
    ''' <param name="silent"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SetHostProperty(ByVal name As String, _
                                           ByVal value As Object, _
                                           Optional ByRef host As Excel.Workbook = Nothing, _
                                           Optional silent As Boolean = True) As Boolean
        Dim parametername_flag As Boolean
        
       
        If host Is Nothing Then
            host = Globals.ThisAddIn.Application.ActiveWorkbook
        End If

        Try
            ' search in Project if not found on parameter sheet
            If Not parametername_flag Then
                parametername_flag = HostPropertyExists(host, name)
                If parametername_flag Then
                    For Each prop As Object In host.CustomDocumentProperties
                        If prop.Name = name Then
                            prop.Value = value
                            SetHostProperty = True
                            Exit Function
                        End If
                    Next prop
                    For Each prop As Object In host.DocumentProperties
                        If prop.Name = name Then
                            prop.Value = value
                            SetHostProperty = True
                            Exit Function
                        End If
                    Next prop

                Else
                    host.CustomDocumentProperties.add(Name:=name, LinkToContent:=False, _
                                                       type:=Office.MsoDocProperties.msoPropertyTypeString, Value:=value)

                    SetHostProperty = True
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Core.CoreMessageHandler(showmsgbox:=Not silent, argument:=name, _
                               exception:=ex, procedure:="modSettings.SetHostProperty")

            Return False
        End Try

        SetHostProperty = True
        Exit Function
    End Function


End Module
