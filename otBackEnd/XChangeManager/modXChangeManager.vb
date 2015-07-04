
REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** XChange Manager Module: static database backend functions (independent from Application such as EXCEL)
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
Imports System.Diagnostics.Debug
Imports System.IO

Imports OnTrack
Imports OnTrack.Database

'' CSV Parser Library
Imports LumenWorks.Framework.IO
Imports LumenWorks.Framework.IO.Csv
Imports OnTrack.Core

Namespace OnTrack.XChange

    ''' <summary>
    ''' CSV XChangeManager is a XChange Manager based on .csv files (with headers as XIDs)
    ''' Main Function is FeedInCSV(path)
    ''' </summary>
    ''' <remarks></remarks>
    Public Module CSVXChangeManager

        ''' <summary>
        ''' Feeds in a csv file from the file system - the first headerids must be the key of the first objectnames
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FeedInCSV(path As String, _
                                  Optional delimiterChar As Char = ";"c, _
                                  Optional commentChar As Char = "#"c) As Boolean

            CoreMessageHandler(message:="looking next to csv file '" & System.IO.Path.GetFileName(path) & "' ", _
                                              argument:=path, username:=CurrentSession.CurrentUsername, _
                                              procedure:="CreateDatabase.FeedInitialData", messagetype:=otCoreMessageType.InternalInfo)

            ''' request rights and start session if necessary
            ''' 
            If Not ot.CurrentSession.RequestUserAccess(accessRequest:=otAccessRight.ReadUpdateData) Then
                ot.CoreMessageHandler(message:="operation aborted due to missing ReadUpdate Rights", procedure:="XChangeCSV.FeedinCSV", messagetype:=otCoreMessageType.ApplicationError)
                Return False
            End If
            Dim aCSVReader As OnTrack.IO.CSV.Reader
            Try
                ''' get the path
                ''' 
                If Not System.IO.File.Exists(path) Then
                    ot.CoreMessageHandler(message:="csv file to feed from is not available", argument:=path, procedure:="XChangeCSV.FeedinCSV", messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If
                Dim aStreamReader As System.IO.StreamReader = New StreamReader(path)
                Dim aConfigName As String = System.IO.Path.GetFileName(path) & "-" & DateTime.Now
                aCSVReader = New OnTrack.IO.CSV.Reader(aStreamReader)
                                    ', hasHeaders:=True, _
                                    '  delimiter:=delimiterChar, quote:=Chr(34), escape:="\"c, _
                                    '  comment:=commentChar, trimmingOptions:=Csv.ValueTrimmingOptions.UnquotedOnly)
                'aCSVReader.SkipEmptyLines = True
                'aCSVReader.MissingFieldAction = Csv.MissingFieldAction.ParseError
                'AddHandler aCSVReader.ParseError, AddressOf CSVParseErrorHandler

                '// open the file "data.csv" which is a CSV file with headers
                ' using (CsvReader csv =
                '        new CsvReader(new StreamReader("data.csv"), true))
                '{ 
                '     int fieldCount = csv.FieldCount;

                '    string[]  headers = csv.GetFieldHeaders();
                '                 While (Csv.ReadNextRecord())
                '    { 
                '         for (int i = 0; i < fieldCount; i++)
                '             Console.Write(string.Format("{0} = {1};",
                '                           headers[i], csv[i]));
                '        Console.WriteLine();
                '    } 
                '}  

                ''' get the headers
                Dim headerids As String()
                'ReDim headerids(aCSVReader.FieldCount)
                'headerids = aCSVReader.GetFieldHeaders
                Dim headerstring As String = Core.DataType.ToString(headerids)


                ''' read the object id of the first object -> must be the key
                ''' 
                Dim names As String() = aCSVReader.Header.ToArray
                Dim theObjectEntries As List(Of iormObjectEntryDefinition)
                Dim anObjectDefinition As iormObjectDefinition
                If names.Count > 1 Then
                    theObjectEntries = ot.CurrentSession.Objects.GetEntriesByXID(xid:=names.Last, objectname:=names.First)
                Else
                    theObjectEntries = ot.CurrentSession.Objects.GetEntriesByXID(xid:=names.Last)
                End If
                If theObjectEntries Is Nothing OrElse theObjectEntries.Count = 0 Then
                    '** load object
                    anObjectDefinition = ot.CurrentSession.Objects.GetObjectDefinition(id:=names.First)
                    '** get the entry
                    If anObjectDefinition IsNot Nothing AndAlso anObjectDefinition.HasEntry(entryname:=names.Last) Then
                        theObjectEntries = New List(Of iormObjectEntryDefinition)
                        theObjectEntries.Add(anObjectDefinition.GetEntryDefinition(entryname:=names.Last))
                        '** try to get the Entries by XID Again 
                    ElseIf anObjectDefinition IsNot Nothing Then
                        If names.Count > 1 Then
                            theObjectEntries = ot.CurrentSession.Objects.GetEntriesByXID(xid:=names.Last, objectname:=names.First)
                        Else
                            theObjectEntries = ot.CurrentSession.Objects.GetEntriesByXID(xid:=names.Last)
                        End If
                    End If
                End If
                If theObjectEntries Is Nothing OrElse theObjectEntries.Count = 0 Then
                    ot.CoreMessageHandler(message:="object entry with xid'" & headerids(0) & "' could not be retrieved - aborted", _
                                         argument:=Core.DataType.ToString(headerids), _
                                         procedure:="XChangeCSV.FeedInCSV", messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If

                ''' the object definition in this csv
                If anObjectDefinition Is Nothing Then anObjectDefinition = ot.CurrentSession.Objects.GetObjectDefinition(theObjectEntries.First.Objectname)

                ''' build a xconfiguration
                ''' 
                Dim aXConfig As XChangeConfiguration = XChangeManager.CreateXChangeConfigFromIDs(configname:=aConfigName, objectids:={anObjectDefinition.Objectname}, runtimeOnly:=True, _
                                                                                                 xids:=headerids, xcmd:=otXChangeCommandType.CreateUpdate)
                If aXConfig Is Nothing Then
                    Return False
                Else
                    ot.CoreMessageHandler(message:="read-only xconfiguration '" & aConfigName & "' created with header '" & headerstring & "'", _
                                          argument:=path, _
                                          procedure:="XChangeCSV.FeedInCSV", messagetype:=otCoreMessageType.ApplicationInfo)
                End If
                Dim result As Boolean = True
                Dim aXBag As New XBag(aXConfig)
                Dim aMsgLog As New BusinessObjectMessageLog(contextidenifier:=path)

                ''' read all the records in the csv file
                ''' 
                If aCSVReader.Process() Then
                    Dim i As Long = 1
                    For Each aRow In aCSVReader.Rows
                        Dim aXEnvelope As XEnvelope = aXBag.AddEnvelope(i) ' add the envelope
                        aXEnvelope.TupleIdentifier = i
                        For Each anID In aCSVReader.Header
                            Dim aValue = aRow.Item(anID)
                            If Trim(aValue.ToString) = "$NULL$" Then aValue = Nothing
                            result = result And aXEnvelope.AddSlotByXID(xid:=anID, isHostValue:=True, value:=aValue)

                            If result = False Then
                                CoreMessageHandler(message:="xchange envelope could not be fully set in row #" & i & "  for header id (xid) " & anID, messagetype:=otCoreMessageType.ApplicationError, _
                                               procedure:="CSVXChangeManager.FeedInCSV")
                            End If
                        Next
                        i += 1 '' increment
                    Next
                End If

                ''' xchange it
                ''' 
                If aXBag.RunPreXCheck(msglog:=aMsgLog) Then
                    result = aXBag.RunXChange(msglog:=aMsgLog)
                Else
                    result = False
                End If


                Return result


            Catch ex As Exception
                ot.CoreMessageHandler(exception:=ex, procedure:="CSVXChangeManager.FeedInCSV", argument:=path)
                Return False
            End Try


        End Function
        ''' <summary>
        ''' dumps a xchange configuration result run out to a path or path with filename
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DumpOutToCSV(path As String, xconfig As XChangeConfiguration) As Boolean
            Dim aFilename As String
            Dim aPath As String
            ''' request rights and start session if necessary
            ''' 
            If Not ot.CurrentSession.RequestUserAccess(accessRequest:=otAccessRight.ReadOnly) Then
                ot.CoreMessageHandler(message:="operation aborted due to missing ReadUpdate Rights", procedure:="XChangeCSV.DumpOutToCSV", messagetype:=otCoreMessageType.ApplicationError)
                Return False
            End If

            Try
                ''' get the path
                ''' 
                If System.IO.Path.GetFileName(path) <> String.Empty AndAlso System.IO.Path.GetExtension(path) = String.Empty Then
                    ot.CoreMessageHandler(message:="file to dump to exists has no .csv extension - added", argument:=path, _
                                            procedure:="XChangeCSV.DumpOutToCSV", messagetype:=otCoreMessageType.ApplicationWarning)
                    path &= ".csv"
                End If

                ''' path is a directory -> put it in there
                If System.IO.Directory.Exists(path) Then
                    If path.Last <> System.IO.Path.DirectorySeparatorChar Then path &= System.IO.Path.DirectorySeparatorChar
                    path &= xconfig.Configname & ".csv"
                End If

                ''' path as file exists
                If System.IO.File.Exists(path) Then
                    If System.IO.Path.GetExtension(path).ToUpper <> ".CSV" Then
                        ot.CoreMessageHandler(message:="file to dump to has different ending and exists - operation aborted", _
                                              argument:=path, procedure:="XChangeCSV.DumpOutToCSV", messagetype:=otCoreMessageType.ApplicationError)
                        Return False
                    Else
                        ''' delete the existing file
                        System.IO.File.Delete(path)
                        ot.CoreMessageHandler(message:="csv file to dump to exists - deleted", argument:=path, _
                                              procedure:="XChangeCSV.DumpOutToCSV", messagetype:=otCoreMessageType.ApplicationWarning)
                    End If
                End If
                ''' set the filename and the path
                aFilename = System.IO.Path.GetFileName(path)
                If aFilename = String.Empty Then
                    aFilename = xconfig.Configname & ".csv"
                ElseIf System.IO.Path.GetExtension(aFilename) = String.Empty Then
                    aFilename &= ".csv"
                End If

                aPath = System.IO.Path.GetDirectoryName(path)
                If aPath = String.Empty Then aPath = System.IO.Directory.GetCurrentDirectory()
                ''' directory must exists
                If Not System.IO.Directory.Exists(aPath) Then
                    ot.CoreMessageHandler(message:="path must contain an existing path - operation aborted", _
                                               argument:=path, procedure:="XChangeCSV.DumpOutToCSV", messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                Else
                    Dim writepermission As System.Security.Permissions.FileIOPermission = _
                        New System.Security.Permissions.FileIOPermission(System.Security.Permissions.FileIOPermissionAccess.Write, aPath & aFilename)
                    writepermission.Demand()
                End If

                ''' 
                ''' run the xchange configuration
                ''' 
                Dim result As Boolean = False
                Dim aXBag As New XBag(xconfig)
                Dim aMsgLog As New BusinessObjectMessageLog

                ot.CoreMessageHandler(message:="running xchange configuration for csv dump file '" & aPath & aFilename & "' ... ", argument:=xconfig.Configname, _
                                         procedure:="XChangeCSV.DumpOutToCSV", messagetype:=otCoreMessageType.ApplicationInfo)

                '''
                ''' running
                If aXBag.RunPreXCheck Then
                    result = aXBag.RunXChange
                End If

                ''' dump out
                If result Then
                    Dim aStreamWrite As System.IO.StreamWriter = New StreamWriter(aPath & aFilename)

                    ot.CoreMessageHandler(message:="writing csv dump file '" & aPath & aFilename & "' ... ", argument:=aPath & aFilename, _
                                          procedure:="XChangeCSV.DumpOutToCSV", messagetype:=otCoreMessageType.ApplicationInfo)

                    ''' write the header line
                    ''' 
                    Dim header As String = String.Empty
                    For Each anEntry In xconfig.OrderedXChangeObjectEntries
                        If header <> String.Empty Then header &= ","
                        If anEntry.XID IsNot Nothing AndAlso anEntry.XID <> String.Empty Then
                            header &= anEntry.XID
                        Else
                            header &= anEntry.Objectname & "." & anEntry.ObjectEntryname
                        End If
                    Next
                    aStreamWrite.WriteLine(header)

                    ''' write alle the envelopes
                    ''' 
                    For Each anEnvelope In aXBag
                        Dim aLine As String = String.Empty
                        For Each aSlot In anEnvelope
                            If aLine <> String.Empty Then aLine &= ","
                            aLine &= aSlot.HostValue
                        Next
                        aStreamWrite.WriteLine(aLine)
                    Next

                    ''' close the file
                    aStreamWrite.Close()

                    ot.CoreMessageHandler(message:="csv dump file written", argument:=aPath & aFilename, _
                                           procedure:="XChangeCSV.DumpOutToCSV", messagetype:=otCoreMessageType.ApplicationInfo)
                    Return True
                Else
                    ot.CoreMessageHandler(message:="xchange was not sucessfull - no dump file '" & aPath & aFilename & "' written ", argument:=xconfig.Configname, _
                                        procedure:="XChangeCSV.DumpOutToCSV", messagetype:=otCoreMessageType.ApplicationError)
                    Return False
                End If



            Catch ex As Exception

                ot.CoreMessageHandler(exception:=ex, procedure:="CSVXChangeManager.DumpOutToCSV")
                Return False
            End Try



        End Function
      
    End Module

    Public Module XChangeManager


        ' ***************************************************************************************************
        '   Module for OnTrack DB Exchange Manager (Interface and Mappings)
        '
        '   Author: B.Schneider
        '   created: 2013-04-01
        '
        '   change-log:
        ' ***************************************************************************************************


        '********** createXChangeConfigs
        '**********
        Public Sub createXChangeConfigs()
            Dim aConfig As XChangeConfiguration

            '**** XSTATUS -> Config to eXchange the Status
            '****
            aConfig = CreateXChangeConfigFromObjectDefinition(configname:="xstatus", _
                                                       objectname:="tblDefStatusItems", xcmd:=otXChangeCommandType.Read)
            If aConfig Is Nothing Then
            End If

        End Sub

        ''' <summary>
        ''' routine creates a xchange config from the object definition and adds all 
        ''' </summary>
        ''' <param name="configname"></param>
        ''' <param name="objectname"></param>
        ''' <param name="xcmd"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateXChangeConfigFromObjectDefinition(ByVal configname As String, _
                                                     ByVal objectname As String, _
                                                     ByVal xcmd As otXChangeCommandType) As XChangeConfiguration
            Dim anObjectName As String
            Dim aNewConfig As XChangeConfiguration = XChangeConfiguration.Create(configname:=configname)
            If aNewConfig Is Nothing Then aNewConfig = XChangeConfiguration.Retrieve(configname:=configname)
            If aNewConfig Is Nothing Then
                ot.CoreMessageHandler(message:="xchange configuration couldnot be created nor retrieved", argument:=configname, procedure:="XChangeManager.CreateXChangeConfigFromIDs")
                Return Nothing
            End If
            Dim anObjectDefinition As ormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=objectname)
            Dim i As Long

            '*** load the table definition
            If anObjectDefinition Is Nothing Then
                Call ot.CoreMessageHandler(argument:=objectname, containerID:=objectname, message:=" Could not load ObjectDEFINITION")
                CreateXChangeConfigFromObjectDefinition = Nothing
                Exit Function
            End If

            '****
            '****
            anObjectName = objectname
            If aNewConfig IsNot Nothing Then
                aNewConfig.Delete()
            End If

            ' create config
            aNewConfig = XChangeConfiguration.Create(configname)
            aNewConfig.AddObjectByName(anObjectName)
            i = 1
            '
            For Each aFieldDef As ormAbstractEntryDefinition In anObjectDefinition.GetEntries
                If aFieldDef.XID <> String.Empty Then
                    Call aNewConfig.AddEntryByObjectEntry(objectentry:=aFieldDef, ordinal:=New OnTrack.Database.Ordinal(i), xcmd:=xcmd)
                    i = i + 1
                End If
            Next

            CreateXChangeConfigFromObjectDefinition = aNewConfig
        End Function


        ''' <summary>
        ''' creates a xchange configuration from an array of xids which might also be entry names or in the form
        ''' [OBJECTNAME.][XID] 
        ''' </summary>
        ''' <param name="CONFIGNAME"></param>
        ''' <param name="IDs"></param>
        ''' <param name="XCMD"></param>
        ''' <param name="OBJECTNAMES"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateXChangeConfigFromIDs(ByVal configname As String, _
                                                   ByVal xids As String(), _
                                                   ByVal xcmd As otXChangeCommandType, _
                                                   Optional runtimeOnly As Boolean = False, _
                                                   Optional ByRef objectids As String() = Nothing) As XChangeConfiguration

            Dim aNewConfig As XChangeConfiguration = XChangeConfiguration.Create(configname:=configname, runtimeonly:=runtimeOnly)
            If aNewConfig Is Nothing Then aNewConfig = XChangeConfiguration.Retrieve(configname:=configname, runtimeonly:=runtimeOnly)
            If aNewConfig Is Nothing Then
                ot.CoreMessageHandler(message:="xchange configuration couldnot be created nor retrieved", _
                                      argument:=configname, messagetype:=otCoreMessageType.ApplicationError, procedure:="XChangeManager.CreateXChangeConfigFromIDs")
                Return Nothing
            End If

            Dim i As Long = 0

            ' add Objectnames
            If objectids IsNot Nothing Then
                For i = LBound(objectids) To UBound(objectids)
                    Call aNewConfig.AddObjectByName(name:=CStr(objectids(i)), orderno:=i, xcmd:=xcmd)
                Next i
            End If

            '''
            ''' check all the xids if xid or object entry names in canonical form

            For i = LBound(xids) To UBound(xids)
                ' load ID
                If Not IsEmpty(xids(i)) Then
                    Dim names As String() = Shuffle.NameSplitter(xids(i).ToUpper)
                    Dim anobjectname As String
                    Dim anxid As String = names.Last
                    If names.Count > 1 Then anobjectname = names.First

                    If Not aNewConfig.AddEntryByXID(Xid:=anxid, objectname:=anobjectname, ordinal:=i, isXChanged:=True, xcmd:=xcmd) Then
                        ''' maybe id is not an id
                        ''' 
                        If anobjectname IsNot Nothing AndAlso anobjectname <> String.Empty Then
                            Dim anObjectDefinition As ormObjectDefinition = CurrentSession.Objects.GetObjectDefinition(id:=anobjectname)
                            If anObjectDefinition IsNot Nothing Then
                                Dim anEntry As iormObjectEntryDefinition = CurrentSession.Objects.GetEntryDefinition(entryname:=names.Last, objectname:=anobjectname)
                                If anEntry IsNot Nothing Then
                                    If Not aNewConfig.AddEntryByObjectEntry(objectentry:=anEntry, ordinal:=i, isxchanged:=True, xcmd:=xcmd) Then
                                        ot.CoreMessageHandler(message:="entry couldnot be added to xconfiguration '" & configname & "'", _
                                                              argument:=anEntry.Objectname & "." & anEntry.Entryname, _
                                                              messagetype:=otCoreMessageType.ApplicationError, _
                                                              procedure:="XChangeManager.CreateXChangeConfigFromIDs")
                                    End If
                                Else
                                    ot.CoreMessageHandler(message:="xchange id is not an ontrack object entry name - skipped in xchange configuration '" & configname & "'", _
                                                                  argument:=anEntry.Objectname & "." & names.Last, _
                                                                  messagetype:=otCoreMessageType.ApplicationWarning, _
                                                                  procedure:="XChangeManager.CreateXChangeConfigFromIDs")
                                End If
                            Else
                                ot.CoreMessageHandler(message:="xchange id doesnot contain an ontrack object name - skipped in xchange configuration '" & configname & "'", _
                                                              argument:=xids(i), _
                                                              messagetype:=otCoreMessageType.ApplicationWarning, _
                                                              procedure:="XChangeManager.CreateXChangeConfigFromIDs")
                            End If

                        Else

                            ot.CoreMessageHandler(message:="header id is not an xchange id nor a valid objectname entry in canonical form - skipped in xchange configuration '" & configname & "'", _
                                                              argument:=xids(i), _
                                                              messagetype:=otCoreMessageType.ApplicationWarning, _
                                                              procedure:="XChangeManager.CreateXChangeConfigFromIDs")
                        End If

                    End If


                End If
            Next i

            Return aNewConfig
        End Function

        '******* XChangeWithArray : eXchanges Data according the Config with an 2dimensional array
        '*******
        Public Function XChangeWithArray(ByRef aConfig As XChangeConfiguration, _
                                         ByRef ARRAYDATA() As Object) As Boolean
            Dim i As Long
            Dim rowno As Long

            Dim aMapping As New Dictionary(Of Object, Object)
            Dim listofAttributes As New Collection
            Dim Value As Object
            Dim aConfigmember As New XChangeObjectEntry

            listofAttributes = aConfig.GetObjectEntries

            If listofAttributes.Count = 0 Then
                XChangeWithArray = False
                Exit Function
            End If

            ' is Array initialized ?!
            If Not IsArrayInitialized(ARRAYDATA) Then
                ReDim Preserve ARRAYDATA(listofAttributes.Count)
            End If

            ' fetch the row
            aMapping = New Dictionary(Of Object, Object)
            i = 0
            For Each aConfigmember In listofAttributes

                If aConfigmember.IsXChanged Then
                    If IsNumeric(aConfigmember.Ordinal.Value) Then
                        i = CLng(aConfigmember.Ordinal.Value)
                    Else
                        i = i + 1
                    End If
                    If Not aMapping.ContainsKey(key:=i) Then
                        If i >= LBound(ARRAYDATA) And i <= UBound(ARRAYDATA) Then Call aMapping.Add(key:=i, value:=ARRAYDATA(i)) ' EMPTY possible
                    End If
                End If
            Next aConfigmember

            ' run the XChange with OTDB
            On Error Resume Next
            'Call aConfig.RunXChange(aMapping:=aMapping)

            ' save the row
            i = 0
            For Each aConfigmember In listofAttributes
                If aConfigmember.IsXChanged Then
                    If IsNumeric(aConfigmember.Ordinal.Value) Then
                        i = CLng(aConfigmember.Ordinal.Value)
                    Else
                        i = i + 1
                    End If
                    If aMapping.ContainsKey(key:=i) Then
                        Value = aMapping.Item(key:=i)
                        If i >= LBound(ARRAYDATA) And i <= UBound(ARRAYDATA) Then ARRAYDATA(i) = Value
                    End If
                End If
            Next aConfigmember




        End Function
    End Module

End Namespace

