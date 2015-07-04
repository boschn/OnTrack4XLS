
REM ***********************************************************************************************************************************************
REM *********** LEGACY OBJECT: clsCartypes for Cartypes selection
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2013-08-08
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** TO DO Log:
REM ***********             - change to clsConfigurable
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************

Option Explicit On
Imports System.Diagnostics
Imports System.Linq
Imports System.Diagnostics.Debug

Imports OTDB
Imports OnTrack


Public Class clsLEGACYCartypes
    '***********************************************************************
    '***** CLASS Cartypes is a representation class of cartypes collection
    '*****


    'Const maxCartypes = 24

    Private sCartypes(26) As Boolean
    Private sCartypesAmount(26) As Integer

    '** initialize -> no car types
    Public Sub New()
        Dim i As Integer

        For i = 1 To UBound(Cartypes)
            sCartypes(i) = False
            sCartypesAmount(i) = 0
        Next i

    End Sub
    '** get the slots as Boolean
    ReadOnly Property Cartypes() As Boolean()
        Get
            Cartypes = sCartypes
        End Get

    End Property


    '** Add a Cartype By Name "H01" ... "H24"
    Public Function addCartypeByName(Value As String) As Boolean

        addCartypeByName = Me.addCartypeByIndex(Mid(Value, 2, 2))

    End Function
    '** add a Cartype by Index 1,..,24
    Public Function addCartypeByIndex(ByVal Value As Integer) As Boolean

        If Value > 0 And Value <= UBound(sCartypes) Then
            sCartypes(Value - 1) = True
            addCartypeByIndex = True
            Exit Function
        End If

        addCartypeByIndex = False
    End Function
    '** add the Cartype with amount
    Public Function addCartypeAmountByIndex(ByVal Value As Integer, ByVal amount As Integer) As Boolean

        If Value > 0 And Value <= UBound(sCartypesAmount) Then
            sCartypesAmount(Value - 1) = amount
            If amount > 0 Then addCartypeAmountByIndex = addCartypeByIndex(Value)
            Exit Function
        End If

        addCartypeAmountByIndex = False
    End Function
    '** get Car Status by Index
    Public Function getCarAmount(ByVal Value As Integer) As UShort

        If Value > 0 And Value <= UBound(sCartypesAmount) Then
            getCarAmount = sCartypesAmount(Value - 1)
            Exit Function
        End If

        getCarAmount = False
    End Function
    '** get Car Status by Index
    Public Function getCar(ByVal Value As Integer) As Boolean

        If Value > 0 And Value <= UBound(sCartypes) Then
            getCar = sCartypes(Value - 1)
            Exit Function
        End If

        getCar = False
    End Function
    '** Helper function show_CarTypes(cartypes() as boolean) -> Generates string of cartypes
    Function show() As String

        Dim i As Integer
        Dim result As String
        '**
        result = String.empty
        For i = 0 To UBound(Me.Cartypes) - 1
            If sCartypes(i) = True Then
                result = result & "X"
            Else
                result = result & "."
            End If
            '* Blank
            If (((i + 1) Mod 5) = 0) Then
                result = result & " "
            End If
        Next i

        show = result
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property printout() As String
        Get
            printout = show()
        End Get
    End Property
    '** Helper function show_CarTypes(cartypes() as boolean) -> Generates string of cartypes
    Function nousedCars() As Integer
        Dim result As Integer
        Dim i As Integer
        '**
        result = 0
        For i = 0 To UBound(Me.Cartypes) - 1
            If sCartypes(i) = True Then
                result = result + 1
            End If
        Next i
        nousedCars = result
    End Function
    '** Helper function description -> Generates H01H10 out of it
    Function description() As String

        Dim i As Integer
        Dim result As String
        '**
        result = String.empty
        For i = 0 To UBound(Me.Cartypes) - 1
            If sCartypes(i) = True Then
                result = result & "H" & Format(i + 1, "00")
            Else

            End If
        Next i

        description = result
    End Function
    '** get True representation
    Function getTrueChar() As String
        Return "X"
    End Function
    '** get False representation
    Function getFalseChar() As String
        Return "."
    End Function

    Function getNoCars() As Integer
        getNoCars = UBound(sCartypes)
    End Function

End Class
