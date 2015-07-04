REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** BUSINESS OBJECTs: Calendar Classes for On Track Database Backend Library
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
Imports System.Collections.Generic

Imports OnTrack.Database
Imports OnTrack.Commons
Imports OnTrack.Core

Namespace OnTrack.Calendar

    ''' <summary>
    ''' Enumeration of the Calendar Entry Types
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otCalendarEntryType
        DayEntry = 1
        MonthEntry = 2
        YearEntry = 3
        WeekEntry = 4
        AbsentEntry = 5
        EventEntry = 6
        MilestoneEntry = 7
    End Enum

    ''' <summary>
    ''' Calendar Entry Class
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=CalendarEntry.ConstObjectID, modulename:=ConstModuleCalendar, description:="object to store an calendar entry", _
        usecache:=True, Version:=1)> Public Class CalendarEntry
        Inherits ormBusinessObject
        Implements iormInfusable
        Implements iormRelationalPersistable

        Public Const ConstObjectID = "CalendarEntry"
        '** Schema
        <ormTableAttribute(version:=2, adddeletefieldbehavior:=True, addDomainBehavior:=True, usecache:=True, addsparefields:=True)> _
        Public Const ConstPrimaryTableID As String = "tblCalendarEntries"

        <ormIndex(columnname1:=constFNName, columnname2:=constFNRefID, columnname3:=constFNID, columnname4:=constFNDomainID)> Public Const constINDEXRefID = "refid"
        <ormIndex(columnname1:=constFNName, columnname2:=constFNTimestamp, columnname3:=ConstFNTypeID, columnname4:=constFNDomainID)> Public Const constIndexType = "typeid"
        <ormIndex(columnname1:=constFNName, columnname2:=constFNTimestamp, columnname3:=constFNDomainID, columnname4:=ConstFNTypeID)> Public Const constIndexDomain = "domain"

        '*** keys
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, PrimaryKeyOrdinal:=1, _
            XID:="CAL1", title:="Name", description:="name of calendar")> Public Const constFNName = "cname"
        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=2, _
           XID:="CAL2", title:="EntryNo", description:="entry no in the calendar")> Public Const constFNID = "id"
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=3, _
            useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const constFNDomainID = Domain.ConstFNDomainID

        '** columns
        <ormObjectEntry(Datatype:=otDataType.Timestamp, _
         XID:="CAL4", title:="Timestamp", description:="timestamp entry in the calendar")> Public Const constFNTimestamp = "timestamp"
        <ormObjectEntry(Datatype:=otDataType.Long, _
         XID:="CAL5", title:="Type", description:="entry type in the calendar")> Public Const ConstFNTypeID = "typeid"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=255, _
        XID:="CAL6", title:="Description", description:="entry description in the calendar")> Public Const ConstFNDescription = "desc"
        <ormObjectEntry(Datatype:=otDataType.Long, _
          XID:="CAL8", title:="RefID", description:="entry refID in the calendar")> Public Const constFNRefID = "refid"

        <ormObjectEntry(Datatype:=otDataType.Bool, XID:="cal9", title:="Not Available", description:="not available")> _
        Public Const constFNIsNotAvailable = "notavail"
        <ormObjectEntry(Datatype:=otDataType.Bool, XID:="cal10", title:="Is Important", description:="is important entry (prioritized)")> _
        Public Const constFNIsImportant = "isimp"

        <ormObjectEntry(Datatype:=otDataType.Long, _
          XID:="CAL20", title:="TimeSpan", description:="length in minutes")> Public Const constFNLength = "length"


        '** not mapped

        <ormObjectEntry(Datatype:=otDataType.Long, _
          XID:="CAL31", title:="Week", description:="week of the year")> Public Const constFNNoWeek = "noweek"
        <ormObjectEntry(Datatype:=otDataType.Long, _
         XID:="CAL32", title:="Day", description:="day of the year")> Public Const constFNNoDay = "noday"
        <ormObjectEntry(Datatype:=otDataType.Long, _
         XID:="CAL33", title:="Weekday", description:="number of day in the week")> Public Const constFNweekday = "noweekday"
        <ormObjectEntry(Datatype:=otDataType.Long, _
         XID:="CAL34", title:="Quarter", description:="no of quarter of the year")> Public Const constFNQuarter = "quarter"
        <ormObjectEntry(Datatype:=otDataType.Long, _
         XID:="CAL35", title:="Year", description:="the year")> Public Const constFNYear = "year"
        <ormObjectEntry(Datatype:=otDataType.Long, _
        XID:="CAL36", title:="Month", description:="the month")> Public Const constFNmonth = "month"
        <ormObjectEntry(Datatype:=otDataType.Long, _
        XID:="CAL37", title:="Day", description:="the day")> Public Const constFNDay = "day"
        <ormObjectEntry(Datatype:=otDataType.Time, _
        XID:="CAL38", title:="Time", description:="time")> Public Const constFNTime = "timevalue"
        <ormObjectEntry(Datatype:=otDataType.Date, _
        XID:="CAL39", title:="Date", description:="date")> Public Const constFNDate = "datevalue"
        <ormObjectEntry(Datatype:=otDataType.Text, size:=10, _
        XID:="CAL40", title:="WeekYear", description:="Week and Year representation")> Public Const constFNWeekYear = "weekofyear"


        '** mappings
        <ormObjectEntryMapping(EntryName:=constFNID)> Private _entryid As Long = 0
        <ormObjectEntryMapping(EntryName:=constFNName)> Private _name As String = String.empty
        <ormObjectEntryMapping(EntryName:=constFNTimestamp)> Private _timestamp As Date = constNullDate
        <ormObjectEntryMapping(EntryName:=constFNRefID)> Private _refid As Long = 0
        <ormObjectEntryMapping(EntryName:=constFNLength)> Private _length As Long = 0
        ' fields
        <ormObjectEntryMapping(EntryName:=ConstFNTypeID)> Private _EntryType As otCalendarEntryType
        <ormObjectEntryMapping(EntryName:=constFNIsImportant)> Private s_isImportant As Boolean = False
        <ormObjectEntryMapping(EntryName:=constFNIsNotAvailable)> Private _notAvailable As Boolean = False
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private s_description As String = String.empty


#Region "Properties"
        ''' <summary>
        ''' gets the name of the calendar of the entry
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        ReadOnly Property Name() As String
            Get
                Return _name
            End Get
        End Property
        ''' <summary>
        ''' gets the id of the calendar entry
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ID() As Long
            Get
                Return _entryid
            End Get
        End Property
        ''' <summary>
        ''' gets or sets the Entry Type of the calendar
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Type() As otCalendarEntryType
            Get
                Return _EntryType
            End Get
            Set(value As otCalendarEntryType)
                SetValue(ConstFNTypeID, value)
            End Set
        End Property
        ''' <summary>
        ''' Timestamp entry of the calendar
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Timestamp() As Date
            Get
                Return _timestamp
            End Get
            Set(value As Date)
                SetValue(constFNTimestamp, value)
            End Set
        End Property
        ''' <summary>
        ''' returns or sets the date portion of the timestamp
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Datevalue() As Date
            Get
                Datevalue = _timestamp.Date
            End Get
            Set(value As Date)
                Me.Timestamp = New DateTime(year:=value.Year, month:=value.Month, day:=value.Day, _
                                           hour:=_timestamp.Hour, minute:=_timestamp.Minute, [second]:=_timestamp.Second, millisecond:=_timestamp.Millisecond)
                's_timestamp = CDate(Format(value, "dd.mm.yyyy") & " " & Format(CDate(s_timestamp), "hh:mm"))
            End Set
        End Property

        ''' <summary>
        ''' length of an entry
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Length() As Long
            Get
                Return _length
            End Get
            Set(value As Long)
                SetValue(constFNLength, value)
            End Set
        End Property
        ''' <summary>
        ''' returns the Timeportion
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Timevalue() As TimeSpan
            Get
                Return _timestamp.TimeOfDay
            End Get
            Set(value As TimeSpan)
                _timestamp = New DateTime(year:=_timestamp.Year, month:=_timestamp.Month, day:=_timestamp.Day, _
                                          hour:=value.Hours, minute:=value.Minutes, [second]:=value.Seconds, millisecond:=value.Milliseconds)


                Me.IsChanged = True
            End Set
        End Property
        ''' <summary>
        ''' sets or gets the description
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description() As String
            Get
                Return s_description
            End Get
            Set(value As String)
                SetValue(ConstFNDescription, value)
            End Set
        End Property
        ''' <summary>
        ''' gets the week-of-year presentation of the timestamp as string
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Weekofyear() As String
            Get
                Dim myear As Integer
                myear = Me.Year
                If Me.Month = 1 And Me.Week >= 52 Then
                    myear = myear - 1
                End If

                Return CStr(myear) & "-" & Format(DatePart("ww", _timestamp, vbMonday, vbFirstFourDays), "0#")
            End Get

        End Property
        ''' <summary>
        ''' gets the week number
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Week() As UShort
            Get
                Return DatePart("ww", _timestamp, vbMonday, vbFirstFourDays)
            End Get
        End Property
        ''' <summary>
        ''' gets the weekday
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property WeekDay() As DayOfWeek
            Get
                Return DatePart("w", _timestamp)
            End Get
        End Property
        ''' <summary>
        ''' gets the day of year number
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property DayofYear() As UShort
            Get
                Return DatePart("y", _timestamp)
            End Get

        End Property
        ''' <summary>
        ''' gets the Day of month number
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property DayOfMonth() As UShort
            Get
                Return DatePart("d", _timestamp)
            End Get

        End Property
        ''' <summary>
        ''' get the month as number
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Month() As UShort
            Get
                Return DatePart("m", _timestamp)
            End Get

        End Property
        ''' <summary>
        ''' gets the year as number
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Year() As UShort
            Get
                Return DatePart("yyyy", _timestamp)
            End Get

        End Property
        ''' <summary>
        ''' gets the Quarter as number
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Quarter() As UShort
            Get
                Return DatePart("q", _timestamp)
            End Get

        End Property
        ''' <summary>
        ''' gets the hour as number
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Hour() As UShort
            Get
                Return DatePart("h", _timestamp)
            End Get

        End Property
        ''' <summary>
        ''' gets the minutes as number
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Minute() As UShort
            Get
                Return DatePart("m", _timestamp)
            End Get

        End Property
        ''' <summary>
        ''' gets or sets the Important flag
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsImportant() As Boolean
            Get
                Return s_isImportant
            End Get
            Set(value As Boolean)
                SetValue(constFNIsImportant, value)
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the not available flag 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsNotAvailable() As Boolean
            Get
                Return _notAvailable
            End Get
            Set(value As Boolean)
                SetValue(constFNIsNotAvailable, value)
            End Set
        End Property

#End Region

        Public Function DeltaYear(newDate As Date) As Long
            DeltaYear = DateDiff("y", _timestamp, newDate)
        End Function
        Public Function DeltaMonth(newDate As Date) As Long
            DeltaMonth = DateDiff("m", _timestamp, newDate)
        End Function
        Public Function DeltaWeek(newDate As Date) As Long
            DeltaWeek = DateDiff("ww", _timestamp, newDate)
        End Function

        Public Function DeltaDay(ByVal newDate As Date, _
                                Optional ByVal considerAvailibilty As Boolean = True, _
                                Optional ByVal calendarname As String = Nothing) As Long
            Dim anEntry As New CalendarEntry
            Dim currDate As Date
            Dim delta As Long
            Dim exitflag As Boolean
            ' delta
            delta = 0
            DeltaDay = DateDiff("d", _timestamp, newDate)
            '
            If considerAvailibilty Then
                currDate = _timestamp
                If DeltaDay < 0 Then
                    delta = -AvailableDays(fromdate:=newDate, untildate:=_timestamp, name:=calendarname)
                ElseIf DeltaDay > 0 Then
                    delta = AvailableDays(fromdate:=_timestamp, untildate:=newDate, name:=calendarname)
                Else : delta = 0
                End If
                ' if the new date is not available
                'If Not anEntry.isAvailableOn(newDate, name:=calendarname) Then
                '    If deltaDay < 0 And delta <> 0 Then
                '        delta = delta + 1
                '    ElseIf deltaDay > 0 And delta <> 0 Then
                '        delta = delta - 1
                '    End If
                'End If
                DeltaDay = delta
                Exit Function
            End If

        End Function
        Public Function DeltaHour(newDate As Date) As Long
            DeltaHour = DateDiff("h", _timestamp, newDate)
        End Function
        Public Function DeltaMinute(newDate As Date) As Long
            DeltaMinute = DateDiff("m", _timestamp, newDate)
        End Function

        Public Function AddYear(aVAlue As Integer) As Date
            AddYear = DateAdd("y", aVAlue, _timestamp)
        End Function
        Public Function AddMonth(aVAlue As Integer) As Date
            AddMonth = DateAdd("m", aVAlue, _timestamp)
        End Function
        Public Function AddWeek(aVAlue As Integer) As Date
            AddWeek = DateAdd("ww", aVAlue, _timestamp)
        End Function
        Public Function AddDay(ByVal aVAlue As Integer, _
        Optional ByVal considerAvailibilty As Boolean = True, _
        Optional ByVal calendarname As String = Nothing) As Date
            Dim anEntry As New CalendarEntry
            Dim currDate As Date
            Dim newDate As Date
            Dim delta As Long
            Dim exitflag As Boolean
            ' delta
            AddDay = DateAdd("d", aVAlue, _timestamp)
            '
            If considerAvailibilty Then
                currDate = _timestamp
                AddDay = Me.NextAvailableDate(currDate, aVAlue, calendarname)
                Exit Function
            End If

        End Function
        Public Function AddHour(aVAlue As Integer) As Date
            AddHour = DateAdd("h", aVAlue, _timestamp)
        End Function
        Public Function AddMinute(aVAlue As Integer) As Date
            AddMinute = DateAdd("m", aVAlue, _timestamp)
        End Function

        Public Function IncYear(aVAlue As Integer) As Date
            Me.Timestamp = Me.AddYear(aVAlue)
            IncYear = Me.Timestamp
        End Function
        Public Function IncMonth(aVAlue As Integer) As Date
            Me.Timestamp = Me.AddMonth(aVAlue)
            IncMonth = Me.Timestamp
        End Function
        Public Function IncDay(aVAlue As Integer) As Date
            Me.Timestamp = Me.AddDay(aVAlue)
            IncDay = Me.Timestamp
        End Function
        Public Function IncWeek(aVAlue As Integer) As Date
            Me.Timestamp = Me.AddWeek(aVAlue)
            IncWeek = Me.Timestamp
        End Function
        Public Function IncHour(aVAlue As Integer) As Date
            Me.Timestamp = Me.AddHour(aVAlue)
            IncHour = Me.Timestamp
        End Function
        Public Function IncMinute(aVAlue As Integer) As Date
            Me.Timestamp = Me.AddMinute(aVAlue)
            IncMinute = Me.Timestamp
        End Function

        ''' <summary>
        ''' Event Handler for record Fed
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub OnRecordFed(sender As Object, e As ormDataObjectEventArgs) Handles Me.OnFed
            Try
                If e.Record.HasIndex(constFNYear) Then e.Record.SetValue(constFNYear, Me.Year)
                If e.Record.HasIndex(constFNmonth) Then e.Record.SetValue(constFNmonth, Me.Month)
                If e.Record.HasIndex(constFNDay) Then e.Record.SetValue(constFNDay, Me.DayOfMonth)
                If e.Record.HasIndex(constFNNoWeek) Then e.Record.SetValue(constFNNoWeek, Me.Week)
                If e.Record.HasIndex(constFNNoDay) Then Call e.Record.SetValue(constFNNoDay, Me.DayofYear)
                If e.Record.HasIndex(constFNQuarter) Then Call e.Record.SetValue(constFNQuarter, Me.Quarter)
                If e.Record.HasIndex(constFNDate) Then Call e.Record.SetValue(constFNDate, Me.Datevalue)
                If e.Record.HasIndex(constFNTime) Then Call e.Record.SetValue(constFNTime, Me.Timevalue)
                If e.Record.HasIndex(constFNweekday) Then Call e.Record.SetValue(constFNweekday, Me.WeekDay)
                If e.Record.HasIndex(constFNWeekYear) Then Call e.Record.SetValue(constFNWeekYear, Me.Weekofyear)
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="CalendarEntry.OnRecordFed")
            End Try

        End Sub

        ''' <summary>
        ''' loads and infuses the object
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(ByVal name As String, ByVal ID As Long, Optional domainid As String = Nothing) As CalendarEntry
            Dim primarykey() As Object = {name, ID, domainID}
            Return RetrieveDataObject(Of CalendarEntry)(pkArray:=primarykey, domainID:=domainID)
        End Function


        ''' <summary>
        ''' Return a Collection of all Calendar Entries
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function All() As List(Of CalendarEntry)
            Return ormBusinessObject.AllDataObject(Of CalendarEntry)()
        End Function

        ''' <summary>
        ''' Returns the number of available days between two dates
        ''' </summary>
        ''' <param name="fromdate"></param>
        ''' <param name="untildate"></param>
        ''' <param name="name">default calendar</param>
        ''' <returns>days in long</returns>
        ''' <remarks></remarks>
        Public Shared Function AvailableDays(ByVal fromdate As Date, ByVal untildate As Date, _
                                             Optional ByVal name As String = Nothing, _
                                             Optional domainid As String = Nothing) As Long

            '* default parameters
            If name = String.Empty Then
                name = CurrentSession.DefaultCalendarName
            End If

            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID

            '** run sqlstatement
            Try
                Dim aStore = ot.GetPrimaryTableStore(ConstPrimaryTableID)
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand(id:="availabledays", addAllFields:=False, addMe:=True)
                If Not aCommand.IsPrepared Then
                    aCommand.select = "count(id)"
                    aCommand.Where = String.Format("[{0}]=@cname and [{1}] > @date1 and [{1}] <@date2 and [{2}] <> @notavail and [{3}]=@typeID ", _
                    {constFNName, constFNTimestamp, constFNIsNotAvailable, ConstFNTypeID, constFNDomainID})
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@cname", datatype:=otDataType.Text, notColumn:=True))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@date1", datatype:=otDataType.Date, notColumn:=True))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@date2", datatype:=otDataType.Date, notColumn:=True))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@notavail", datatype:=otDataType.Bool, notColumn:=True))
                    aCommand.AddParameter(New ormSqlCommandParameter(ID:="@typeid", datatype:=otDataType.[Long], notColumn:=True))
                    'aCommand.AddParameter(New ormSqlCommandParameter(ID:="@domainid", datatype:=otDataType.Text, notColumn:=True))
                    'aCommand.AddParameter(New ormSqlCommandParameter(ID:="@globalDomain", datatype:=otFieldDataType.Text, notColumn:=True))

                    aCommand.Prepare()
                End If

                '** values
                aCommand.SetParameterValue(ID:="@cname", value:=name)
                aCommand.SetParameterValue(ID:="@date1", value:=fromdate)
                aCommand.SetParameterValue(ID:="@date2", value:=untildate)
                aCommand.SetParameterValue(ID:="@notavail", value:=True)
                aCommand.SetParameterValue(ID:="@typeid", value:=otCalendarEntryType.DayEntry)
                'aCommand.SetParameterValue(ID:="@domainid", value:=domainid)
                'aCommand.SetParameterValue(ID:="@globalDomain", value:=ConstGlobalDomain)

                Dim resultRecords As List(Of ormRecord) = aCommand.RunSelect

                If resultRecords.Count > 0 Then
                    If Not IsNull(resultRecords.Item(0).GetValue(1)) And IsNumeric(resultRecords.Item(0).GetValue(1)) Then
                        AvailableDays = CLng(resultRecords.Item(0).GetValue(1)) + 1
                    Else
                        AvailableDays = 0
                    End If
                Else
                    AvailableDays = 0
                End If

                Return AvailableDays

            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="CalendarEntry.AvailableDays")
                Return -1
            End Try

        End Function
        ''' <summary>
        ''' returnss the next available date from a date in no of  days
        ''' </summary>
        ''' <param name="fromdate">From Date</param>
        ''' <param name="noDays">number of days</param>
        ''' <param name="Name">default calendar</param>
        ''' <returns>next date</returns>
        ''' <remarks></remarks>
        Public Shared Function NextAvailableDate(ByVal fromdate As Date, ByVal noDays As Integer, _
                                                 Optional ByVal name As String = Nothing, _
                                                 Optional domainid As String = Nothing) As Date

            '** default values
            If String.IsNullOrWhiteSpace(name) Then name = CurrentSession.DefaultCalendarName
            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID

            '**
            Try
                Dim aStore = GetPrimaryTableStore(ConstPrimaryTableID)
                Dim aCommand As ormSqlSelectCommand
                If noDays < 0 Then
                    aCommand = aStore.CreateSqlSelectCommand(id:="nextavailabledate-neg", addAllFields:=False, addMe:=False)
                    If Not aCommand.IsPrepared Then
                        aCommand.select = "[" & constFNTimestamp & "]"
                        aCommand.Where = String.Format("[{0}]=@cname and [{1}] < @date1  and [{2}] <> @avail and [{3}]=@typeID and [{4}]=@domainID", _
                            {constFNName, constFNTimestamp, constFNIsNotAvailable, ConstFNTypeID, constFNDomainID})

                        aCommand.OrderBy = "[" & constFNTimestamp & "] desc"
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@cname", columnname:=constFNName, tableid:=ConstPrimaryTableID))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@date1", datatype:=otDataType.Date, notColumn:=True))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@avail", datatype:=otDataType.Bool, notColumn:=True))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@typeid", datatype:=otDataType.[Long], notColumn:=True))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@domainid", datatype:=otDataType.Text, notColumn:=True))

                        aCommand.Prepare()
                    End If

                Else
                    aCommand = aStore.CreateSqlSelectCommand(id:="nextavailabledate-pos", addAllFields:=False, addMe:=False)
                    If Not aCommand.IsPrepared Then

                        aCommand.select = "[" & constFNTimestamp & "]"
                        aCommand.Where = String.Format("[{0}]=@cname and [{1}] > @date1  and [{2}] <> @avail and [{3}]=@typeID and [{4}]=@domainID", _
                           {constFNName, constFNTimestamp, constFNIsNotAvailable, ConstFNTypeID, constFNDomainID})

                        aCommand.OrderBy = "[" & constFNTimestamp & "] asc"

                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@cname", columnname:=constFNName, tableid:=ConstPrimaryTableID))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@date1", datatype:=otDataType.Date, notColumn:=True))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@avail", datatype:=otDataType.Bool, notColumn:=True))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@typeid", datatype:=otDataType.[Long], notColumn:=True))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@domainid", datatype:=otDataType.Text, notColumn:=True))

                        aCommand.Prepare()

                    End If
                End If

                '** values
                aCommand.SetParameterValue(ID:="@cnamd", value:=name)
                aCommand.SetParameterValue(ID:="@date1", value:=fromdate)
                aCommand.SetParameterValue(ID:="@avail", value:=True)
                aCommand.SetParameterValue(ID:="@typeid", value:=otCalendarEntryType.DayEntry)
                aCommand.SetParameterValue(ID:="@domainid", value:=domainid)

                Dim resultRecords As List(Of ormRecord) = aCommand.RunSelect

                If resultRecords.Count > noDays Then
                    NextAvailableDate = resultRecords.Item(noDays - 1).GetValue(1)
                Else
                    Call CoreMessageHandler(procedure:="CalendarEntry.nextavailableDate", message:="requested no of days is behind calendar end - regenerate calendar", _
                                           messagetype:=otCoreMessageType.ApplicationError, argument:=noDays)
                    NextAvailableDate = resultRecords.Last.GetValue(1)
                End If

                Return NextAvailableDate

            Catch ex As Exception
                Call CoreMessageHandler(exception:=ex, procedure:="CalendarEntry.nextAvailableDate")
                Return constNullDate
            End Try



        End Function

        '****** isAvailable looks for otDayEntries showing availibility
        '******
        ''' <summary>
        ''' isAvailable looks for otDayEntries showing availibility
        ''' </summary>
        ''' <param name="refdate"></param>
        ''' <param name="Name">default calendar</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function IsAvailableOn(ByVal refdate As Date, Optional ByVal name As String = Nothing, Optional domainid As String = Nothing) As Boolean
            Dim aCollection As New List(Of CalendarEntry)
            Dim anEntry As New CalendarEntry

            If String.IsNullOrWhiteSpace(name) Then name = CurrentSession.DefaultCalendarName
            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID

            aCollection = AllByDate(name:=name, refDate:=refdate, domainID:=domainid)
            If aCollection Is Nothing Or aCollection.Count = 0 Then
                IsAvailableOn = True
                Exit Function
            End If

            For Each anEntry In aCollection
                If anEntry.Type = otCalendarEntryType.DayEntry Or anEntry.Type = otCalendarEntryType.AbsentEntry Then
                    If anEntry.IsNotAvailable Then
                        IsAvailableOn = False
                        Exit Function
                    End If
                End If
            Next anEntry

            IsAvailableOn = True
        End Function

        ''' <summary>
        ''' Returns True if the Calendar has the referenced date as a valid date
        ''' </summary>
        ''' <param name="refDate"></param>
        ''' <param name="name"></param>
        ''' <param name="domainID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function HasDate(ByVal refDate As Date, Optional ByVal name As String = Nothing, Optional domainid As String = Nothing) As Boolean
            If String.IsNullOrWhiteSpace(name) Then name = CurrentSession.DefaultCalendarName
            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID


            Try
                Dim aStore As iormRelationalTableStore = GetPrimaryTableStore(ConstPrimaryTableID)
                Dim cached = aStore.GetProperty(ormTableStore.ConstTPNCacheProperty)
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand("AllByDate", addAllFields:=False)

                '** prepare the command if necessary
                If Not aCommand.IsPrepared Then

                    aCommand.AddTable(ConstPrimaryTableID, addAllFields:=True)
                    aCommand.select = constFNName
                    '** Depends on the server
                    If aCommand.DatabaseDriver.Name = ConstCPVDriverSQLServer And cached Is Nothing Then
                        aCommand.Where = String.Format(" [{0}] = @cname and CONVERT(nvarchar, [{1}], 104) = @datestr and [{2}] = @domainID", _
                                                       {constFNName, constFNTimestamp, constFNDomainID})
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@cname", datatype:=otDataType.Text, notColumn:=True))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@datestr", datatype:=otDataType.Text, notColumn:=True))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@domainID", datatype:=otDataType.Text, notColumn:=True))

                    ElseIf aCommand.DatabaseDriver.Name = ConstCPVDriverOleDB And cached Is Nothing Then
                        aCommand.Where = String.Format(" [{0}] = @cname and [{1}] = @date and [{2}]=@domainID", _
                        {constFNName, constFNTimestamp, constFNDomainID})
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@cname", datatype:=otDataType.Text, notColumn:=True))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@date", datatype:=otDataType.Date, notColumn:=True))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@domainID", datatype:=otDataType.Text, notColumn:=True))

                        ''' just cached against DataTable
                    ElseIf cached IsNot Nothing Then
                        aCommand.Where = String.Format(" [{0}] = @cname and [{1}] = @date and [{2}]=@domainID", _
                       {constFNName, constFNTimestamp, constFNDomainID})
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@cname", datatype:=otDataType.Text, notColumn:=True))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@date", datatype:=otDataType.Date, notColumn:=True))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@domainID", datatype:=otDataType.Text, notColumn:=True))
                    Else

                        CoreMessageHandler(message:="database driver name not recognized for SQL Statement", messagetype:=otCoreMessageType.InternalError, _
                                           procedure:="CalendarEntry.HasDate", containerID:=ConstPrimaryTableID, argument:=aCommand.DatabaseDriver.Name)
                        Return False
                    End If


                    aCommand.Prepare()
                End If

                ' set Parameter
                aCommand.SetParameterValue("@cname", name)
                If aCommand.DatabaseDriver.Name = ConstCPVDriverSQLServer And cached Is Nothing Then
                    aCommand.SetParameterValue("@datestr", Format("dd.MM.yyyy", refDate))
                Else
                    aCommand.SetParameterValue("@date", refDate)
                End If
                aCommand.SetParameterValue("@domainID", domainid)

                '** run the Command
                Dim theRecords As List(Of ormRecord) = aCommand.RunSelect

                If theRecords.Count >= 0 Then
                    Return True
                End If
                Return False
            Catch ex As Exception
                Call CoreMessageHandler(procedure:="CalendarEntry.HasDate", exception:=ex, messagetype:=otCoreMessageType.InternalError)
                Return False
            End Try

        End Function


        ''' <summary>
        ''' returns all calendar entries by refence date
        ''' </summary>
        ''' <param name="refDate"></param>
        ''' <param name="name"></param>
        ''' <returns>a collection of objects</returns>
        ''' <remarks></remarks>
        Public Shared Function AllByDate(ByVal refDate As Date, Optional ByVal name As String = Nothing, Optional domainid As String = Nothing) As List(Of CalendarEntry)
            Dim aCollection As New List(Of CalendarEntry)
            Dim aStore As iormRelationalTableStore

            '** defaults
            If String.IsNullOrWhiteSpace(name) Then name = CurrentSession.DefaultCalendarName
            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID


            Try
                aStore = GetPrimaryTableStore(ConstPrimaryTableID)
                Dim cached = aStore.GetProperty(ormTableStore.ConstTPNCacheProperty)
                Dim aCommand As ormSqlSelectCommand = aStore.CreateSqlSelectCommand("AllByDate")

                '** prepare the command if necessary
                If Not aCommand.IsPrepared Then

                    aCommand.AddTable(ConstPrimaryTableID, addAllFields:=True)
                    '** Depends on the server
                    If aCommand.DatabaseDriver.Name = ConstCPVDriverSQLServer And cached Is Nothing Then
                        aCommand.Where = String.Format(" [{0}] = @cname and CONVERT(nvarchar, [{1}], 104) = @datestr and [{2}] = @domainID", _
                                                       {constFNName, constFNTimestamp, constFNDomainID})
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@cname", datatype:=otDataType.Text, columnname:=constFNName))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@datestr", datatype:=otDataType.Text, notColumn:=True))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@domainID", datatype:=otDataType.Text, notColumn:=True))

                    ElseIf aCommand.DatabaseDriver.Name = ConstCPVDriverOleDB And cached Is Nothing Then
                        aCommand.Where = String.Format(" [{0}] = @cname and [{1}] = @date and [{2}]=@domainID", _
                        {constFNName, constFNTimestamp, constFNDomainID})
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@cname", datatype:=otDataType.Text, columnname:=constFNName))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@date", datatype:=otDataType.Date, notColumn:=True))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@domainID", datatype:=otDataType.Text, notColumn:=True))

                        ''' just cached against DataTable
                    ElseIf cached IsNot Nothing Then
                        aCommand.Where = String.Format(" [{0}] = @cname and [{1}] = @date and [{2}]=@domainID", _
                       {constFNName, constFNTimestamp, constFNDomainID})
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@cname", datatype:=otDataType.Text, columnname:=constFNName))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@date", datatype:=otDataType.Date, notColumn:=True))
                        aCommand.AddParameter(New ormSqlCommandParameter(ID:="@domainID", datatype:=otDataType.Text, notColumn:=True))
                    Else

                        CoreMessageHandler(message:="Database Driver Name not recognized for SQL Statement", messagetype:=otCoreMessageType.InternalError, _
                                           procedure:="CalendarEntry.AllByDate", containerID:=ConstPrimaryTableID, argument:=aCommand.DatabaseDriver.Name)
                        Return aCollection
                    End If


                    aCommand.Prepare()
                End If

                ' set Parameter
                aCommand.SetParameterValue("@cname", name)
                If aCommand.DatabaseDriver.Name = ConstCPVDriverSQLServer And cached Is Nothing Then
                    aCommand.SetParameterValue("@datestr", Format("dd.MM.yyyy", refDate))
                Else
                    aCommand.SetParameterValue("@date", refDate)
                End If
                aCommand.SetParameterValue("@domainID", domainid)

                '** run the Command
                Dim theRecords As List(Of ormRecord) = aCommand.RunSelect

                If theRecords.Count >= 0 Then
                    For Each aRecord As ormRecord In theRecords
                        Dim aNewObject As New CalendarEntry
                        If InfuseDataObject(record:=aRecord, dataobject:=aNewObject) Then
                            aCollection.Add(item:=aNewObject)
                        End If
                    Next aRecord

                End If
                Return aCollection
            Catch ex As Exception
                Call CoreMessageHandler(procedure:="CalendarEntry.AllByDate", exception:=ex, messagetype:=otCoreMessageType.InternalError)
                Return aCollection
            End Try

        End Function

        ''' <summary>
        ''' Initialize the calendar with dates from a date until a date
        ''' </summary>
        ''' <param name="fromdate">from date to initalize</param>
        ''' <param name="untildate">to date </param>
        ''' <param name="name">name of the calendar (optional)</param>
        ''' <returns>True if successfull</returns>
        ''' <remarks></remarks>
        Public Shared Function GenerateDays(ByVal fromdate As Date, ByVal untildate As Date, Optional ByVal name As String = Nothing, Optional domainid As String = Nothing) As Boolean
            Dim aCollection As New List(Of CalendarEntry)
            Dim currDate As Date
            Dim anEntry As New CalendarEntry

            ' calendar name
            If String.IsNullOrWhiteSpace(name) Then name = CurrentSession.DefaultCalendarName
            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID

            ' start
            currDate = fromdate
            Do While currDate <= untildate

                'exists ?
                aCollection = CalendarEntry.AllByDate(refDate:=currDate, name:=name, domainid:=domainid)
                If aCollection Is Nothing OrElse aCollection.Count = 0 Then
                    anEntry = CalendarEntry.Create(name:=name, domainID:=domainid)
                    If anEntry IsNot Nothing Then
                        With anEntry
                            .Datevalue = currDate
                            .IsNotAvailable = False
                            .Description = "working day"
                            ' weekend
                            If .WeekDay = vbSaturday Or .WeekDay = vbSunday Then
                                .IsNotAvailable = True
                                .Description = "weekend"
                            Else
                                .IsNotAvailable = False
                            End If
                            ' new year
                            If .Month = 1 And (.DayOfMonth = 1) Then
                                .IsNotAvailable = True
                                .Description = "new year"
                            ElseIf .Month = 10 And .DayOfMonth = 3 Then
                                .IsNotAvailable = True
                                .Description = "reunifcation day in germany"
                            ElseIf .Month = 5 And .DayOfMonth = 1 Then
                                .IsNotAvailable = True
                                .Description = "labor day in germany"
                            ElseIf .Month = 11 And .DayOfMonth = 1 Then
                                .IsNotAvailable = True
                                .Description = "allerseelen in germany"
                                ' christmas
                            ElseIf .Month = 12 And (.DayOfMonth = 24 Or .DayOfMonth = 26 Or .DayOfMonth = 25) Then
                                .IsNotAvailable = True
                                .Description = "christmas"
                            End If
                            .Type = otCalendarEntryType.DayEntry
                            .Persist()
                        End With
                    End If
                End If

                ' inc
                currDate = DateAdd("d", 1, currDate)
            Loop

            Return True

        End Function

        ''' <summary>
        ''' Creates an persistable calendar entry
        ''' </summary>
        ''' <param name="name">name of calendar</param>
        ''' <returns>True if successfull</returns>
        ''' <remarks></remarks>
        Public Shared Function Create(Optional ByVal name As String = Nothing, Optional entryid As Long = 0, Optional domainid As String = Nothing) As CalendarEntry
            Dim primarykey() As Object = {name, entryid, domainid}

            '** default values
            If String.IsNullOrWhiteSpace(name) Then name = CurrentSession.DefaultCalendarName
            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID

            '** create the key
            If entryid = 0 Then
                Dim pkarray() As Object = {name, Nothing, Nothing}
                If Not ot.GetPrimaryTableStore(ConstPrimaryTableID).CreateUniquePkValue(pkarray) Then
                    Call CoreMessageHandler(message:="unique key could not be created", procedure:="CalendarEntry.Create", argument:=name, _
                                                containerID:=ConstPrimaryTableID, messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If
                primarykey = {name, pkarray(1), domainid}
            End If
            Return CreateDataObject(Of CalendarEntry)(pkArray:=primarykey, domainID:=domainid)
        End Function

    End Class
End Namespace