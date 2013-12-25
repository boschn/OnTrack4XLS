
REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE TOOLING 4 EXCEL
REM *********** 
REM *********** MODULE FUNCTIONS to handle Parameters in the Application / Host Envirorement
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
Imports Microsoft.Office.Tools.Excel
Imports Microsoft.Office.Interop.Excel
Imports OnTrack

Module modParameterXLS
    ' ***************************************************************************************************
    '   Module to handle Parameter to Host Globals.ThisAddin.Application (EXCEL)
    '
    '   Author: B.Schneider
    '   created: 2013-03-22
    '
    '   change-log:
    ' ***************************************************************************************************



    '************************************************************************
    ' getParameterByName : returns Parameter by Name in the order XLS, Workbook, Worksheet
    '                      Fetches errors like Worksheet is missing or ParameterName not Defined
    '
    ' returns a Range

    ''' <summary>
    ''' retrieves a named Range Area by name from a workbook or the current workbook
    ''' or nothing if not found
    ''' </summary>
    ''' <param name="name"></param>
    ''' <param name="workbook"></param>
    ''' <param name="silent"></param>
    ''' <param name="found"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetXlsParameterRangeByName(ByVal name As String, _
                                     Optional ByRef workbook As Excel.Workbook = Nothing, _
                                     Optional silent As Boolean = False, _
                                     Optional ByRef found As Boolean = False) As Range
        Dim aParameterSheetFlag, aParameterNameFlag As Boolean
        Dim ws As Excel.Worksheet
        Dim wb As Excel.Workbook
        Dim pn As Name

        Try


            ' get or set the global doc9
            If Not workbook Is Nothing Then
                wb = workbook
            End If
            If wb Is Nothing Then
                If Globals.ThisAddIn.Application.ActiveWorkbook Is Nothing Then
                    Return Nothing
                Else
                    wb = Globals.ThisAddIn.Application.ActiveWorkbook
                End If
            End If

            ' Check if Parameters Sheet is still there
            aParameterSheetFlag = False
            If SheetExistsinWorkbook(wb, constParameterSheetName) Then
                ws = wb.Sheets(constParameterSheetName)
                aParameterSheetFlag = True
            Else
                If Not silent Then
                    Call CoreMessageHandler(message:="The Worksheet " & constParameterSheetName & " is not found in this Workbook '" & wb.Name & "' !", _
                                                messagetype:=otCoreMessageType.ApplicationError, showmsgbox:=True, subname:="modParameterXLS.getXLSParameterRangebyName")
                End If
            End If

            ' Check if Parameter is known in the parameter worksheet (if existing)
            If aParameterSheetFlag Then
                pn = NameinWorksheet(ws, name)

                If Not pn Is Nothing Then
                    aParameterNameFlag = True
                Else
                    aParameterNameFlag = False
                End If
            End If

            ' search in workbook if not found on parameter sheet
            If Not aParameterNameFlag Then

                aParameterNameFlag = NameExistsinWorkbook(wb, name)
                If aParameterNameFlag Then
                    pn = NameInWorkbook(wb, name)
                Else
                    ' final check on Globals.ThisAddin.Application
                    If NameExistsinApplication(name) Then
                        pn = NameinApplication(name)
                        If pn.Parent.Name = wb.Name Then
                            aParameterNameFlag = True
                        End If
                    End If
                    If Not aParameterNameFlag Then
                        If Not silent Then
                            Call CoreMessageHandler(message:="The parameter '" & name & " ' is not found in this Workbook '" & wb.Name & "'!", _
                                         messagetype:=otCoreMessageType.ApplicationError, showmsgbox:=True, subname:="modParameterXLS.getXLSParameterRangebyName")

                        End If
                        If Not IsMissing(found) Then found = False
                        GetXlsParameterRangeByName = Nothing
                        Exit Function
                    End If
                End If
            End If


            If Not IsError(pn.RefersToRange) Then
                GetXlsParameterRangeByName = pn.RefersToRange
                If Not IsMissing(found) Then found = True
            Else
                GetXlsParameterRangeByName = Nothing
                If Not IsMissing(found) Then found = False
            End If

            Exit Function

        Catch ex As Exception
            Call CoreMessageHandler(exception:=ex, _
                                         messagetype:=otCoreMessageType.ApplicationError, subname:="modParameterXLS.getXLSParameterRangebyName")

            Return Nothing
        End Try


    End Function



    '************************************************************************
    ' getParameterByName : returns Parameter by Name in the order XLS, Workbook, Worksheet
    '                      Fetches errors like Worksheet is missing or ParameterName not Defined
    ' Optional aWorkbook to look in
    ' Optional found to indicate if parameter exists -> return value
    ' silent if true than do not issue error
    ' returns a object
    ''' <summary>
    ''' returns Parameter by Name in the order XLS, Workbook, Worksheet as Object (value)
    ''' </summary>
    ''' <param name="name"></param>
    ''' <param name="workbook"></param>
    ''' <param name="found"></param>
    ''' <param name="silent"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetXlsParameterByName(ByVal name As String, _
                                Optional ByRef workbook As Excel.Workbook = Nothing, _
                                Optional ByRef found As Boolean = False, _
                                Optional ByVal silent As Boolean = False) As Object


        Dim namedarea As Range = GetXlsParameterRangeByName(name:=name, workbook:=workbook, found:=found, silent:=silent)

        '** Found ?!
        If namedarea Is Nothing Then
            Return Nothing
        Else
            Return namedarea.Value
        End If

    End Function

    '************************************************************************
    ' setParameterValueByName : sets the Value of a Parameter on the parameter sheet by the Name supplied
    '
    ' Name: Name of the parameter
    ' Value: Value of the parameter as object
    '
    ' returns a object
    ''' <summary>
    ''' sets the Value of a Parameter on the parameter sheet by the Name supplied
    ''' </summary>
    ''' <param name="name"></param>
    ''' <param name="value"></param>
    ''' <param name="workbook"></param>
    ''' <param name="silent"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Function SetXlsParameterValueByName(ByVal name As String, _
                                     ByRef value As Object, _
                                     Optional ByRef workbook As Excel.Workbook = Nothing, _
                                     Optional silent As Boolean = True, _
                                     Optional passwordParameter As String = "") As Boolean
        Dim aParameterSheetFlag = False, aParameterNameFlag As Boolean
        Dim ws As Excel.Worksheet
        Dim pn As Name
        Dim wb As Excel.Workbook

        Try
            ' now we hope to have it in the Active Workbook
            ' get or set the global doc9
            If Not workbook Is Nothing Then
                wb = workbook
            Else
                wb = Globals.ThisAddIn.Application.ActiveWorkbook
            End If

            ' Check if Parameters Sheet is still there
            If SheetExistsinWorkbook(wb, constParameterSheetName) Then
                ws = wb.Sheets(constParameterSheetName)
                aParameterSheetFlag = True
            Else
                If Not silent Then
                    Call CoreMessageHandler(message:="The Worksheet " & constParameterSheetName & " is not found in this Workbook '" & wb.Name & "' !", _
                                                   messagetype:=otCoreMessageType.ApplicationError, showmsgbox:=True, subname:="modParameterXLS.SetXLSParameterValuebyName")

                End If
            End If

            ' Check if Parameter is known in the parameter worksheet (if existing)
            If aParameterSheetFlag Then
                pn = NameinWorksheet(ws, name)
                If Not pn Is Nothing Then
                    aParameterNameFlag = True
                End If
            End If

            ' search in workbook if not found on parameter sheet
            If Not aParameterNameFlag Then
                aParameterNameFlag = NameExistsinWorkbook(wb, name)
                If aParameterNameFlag Then
                    pn = NameInWorkbook(wb, name)
                Else
                    If Not silent Then
                        Call CoreMessageHandler(message:="The parameter '" & name & " ' is not found in this Workbook '" & wb.Name & "'!", _
                                                 messagetype:=otCoreMessageType.ApplicationError, showmsgbox:=True, subname:="modParameterXLS.SetXLSParameterValuebyName")

                    End If
                    Return False
                End If
            End If

            'Set Value
            ws = pn.RefersToRange.Worksheet
            ' protected
            If ws.ProtectContents Then
                ws.Activate()
                ws.Select()    'Ungroup
                ws.Unprotect(passwordParameter)
                ws.Range(name).Value = value
                ws.Protect(passwordParameter)
            Else
                ws.Range(name).Value = value
            End If

            Return True

        Catch ex As Exception

            Call CoreMessageHandler(message:="The parameter '" & name & " ' cannot be written to " & wb.Name & "!", _
                                        subname:="modParameterXLS.SetXlsParameterValueByName", arg1:=name, showmsgbox:=silent)
            Call CoreMessageHandler(exception:=ex, messagetype:=otCoreMessageType.ApplicationException, _
                                         subname:="modParameterXLS.SetXlsParameterValueByName", arg1:=name)
            Return False
        End Try

    End Function


End Module
