
REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** TIMEINTERVAL HELPER CLASS
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
Imports OnTrack.Core

Public Class clsHELPERTimeInterval
    '**************************************************************************************************
    '******* class TimeInterval defines a time with start and end

    Private s_start As Date
    Private s_end As Date
    Public isActEnd As Boolean
    Public isActStart As Boolean

    Public startcmt As String
    Public endcmt As String

    Public Property startdate() As Date
        Get
            startdate = s_start
        End Get
        Set(value As Date)
            s_start = value
        End Set
    End Property

    Public Property enddate() As Date
        Get
            enddate = s_end
        End Get
        Set(value As Date)
            s_end = value
        End Set
    End Property


    '**** relativeTo returns -1 if the Start / End is past the refdate or 1 if Start / End is in future or 0 start < refdate < end
    Public Function relativeTo(aRefDate As Date) As otIntervalRelativeType

        Dim aVAlue As Long
        Dim result As otIntervalRelativeType

        ' in the past
        If DateDiff("d", Me.startdate, aRefDate) > 0 And DateDiff("d", Me.enddate, aRefDate) > 0 Then
            result = otIntervalRelativeType.IntervalLeft
            ' in the future
        ElseIf DateDiff("d", Me.startdate, aRefDate) < 0 And DateDiff("d", Me.enddate, aRefDate) < 0 Then
            result = otIntervalRelativeType.IntervalRight
        ElseIf DateDiff("d", Me.startdate, aRefDate) < 0 And DateDiff("d", Me.enddate, aRefDate) > 0 Then
            result = otIntervalRelativeType.IntervalMiddle
        ElseIf DateDiff("d", Me.startdate, aRefDate) > 0 And DateDiff("d", Me.enddate, aRefDate) < 0 Then
            result = otIntervalRelativeType.IntervalMiddle
        Else    ' not valid
            result = otIntervalRelativeType.IntervalInvalid
        End If

        relativeTo = result

    End Function

    Public Function isvalid() As Boolean

        If s_start <> ot.constNullDate And s_end <> ot.constNullDate And _
           DateDiff("d", s_start, s_end) >= 0 Then
            isvalid = True
        Else
            isvalid = False
        End If
    End Function
    Public Sub New()
        s_start = ot.constNullDate
        s_end = ot.constNullDate
    End Sub
    Public Function span() As Long
        If Me.isvalid Then
            span = DateDiff("d", Me.startdate, Me.enddate)
        End If
    End Function
    '******* calculates the overlapping time between 2 TimeIntervals
    Public Function overlapp(anotherTI As clsHELPERTimeInterval) As Long
        Dim result As Long

        ' mystart > otherend
        If DateDiff("d", Me.startdate, anotherTI.enddate) <= 0 Then
            result = DateDiff("d", Me.startdate, anotherTI.enddate)
            ' otherend < mystart
        ElseIf DateDiff("d", anotherTI.startdate, Me.enddate) <= 0 Then
            result = DateDiff("d", anotherTI.startdate, Me.enddate)
            ' mystart < otherstart and myend < otherend -> left
        ElseIf DateDiff("d", Me.startdate, anotherTI.startdate) >= 0 _
               And DateDiff("d", Me.enddate, anotherTI.enddate) >= 0 Then
            result = DateDiff("d", anotherTI.startdate, Me.enddate)
            ' otherstart < mystart and otherend < myend -> right
        ElseIf DateDiff("d", anotherTI.startdate, Me.startdate) >= 0 _
               And DateDiff("d", anotherTI.enddate, Me.enddate) >= 0 Then
            result = DateDiff("d", Me.startdate, anotherTI.enddate)
            ' otherstart < mystart and myend > otherend
        ElseIf DateDiff("d", anotherTI.startdate, Me.startdate) >= 0 _
               And DateDiff("d", Me.enddate, anotherTI.enddate) >= 0 Then
            result = DateDiff("d", Me.startdate, Me.enddate)
            ' otherstart > mystart and myend < otherend
        ElseIf DateDiff("d", anotherTI.startdate, Me.startdate) <= 0 _
               And DateDiff("d", Me.enddate, anotherTI.enddate) <= 0 Then
            result = DateDiff("d", anotherTI.startdate, anotherTI.enddate)
        Else
            System.Diagnostics.Debug.Assert(False)
        End If

        overlapp = result
    End Function

End Class
