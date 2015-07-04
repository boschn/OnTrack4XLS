


Imports OnTrack.Database

Namespace OnTrack.IFM


    Public Class clsOTDBInterface
        Inherits ormDataObject

        '************************************************************************************
        '***** CLASS clsOTDBInterface is the object for a OTDBRecord (which is the datastore)
        '*****
        '*****
        Const ourTablename As String = "tblinterfaces"

        Private s_uid As Long
        Private s_icdid As String
        Private s_icdrev As String
        Private s_assy1 As String
        Private s_dept1 As String
        Private s_desc1 As String
        Private s_assy2 As String
        Private s_dept2 As String
        Private s_desc2 As String
        Private s_cartypes As New clsLEGACYCartypes
        Private s_status As String
        Private s_class As String
        Private s_changedOn As Date

        '** initialize
        Public Sub New()
            Call MyBase.New(ourTablename)
        End Sub

        '*** init
        Public Function initialize() As Boolean
            initialize = MyBase.Initialize()

        End Function

        ReadOnly Property UID() As Long
            Get
                UID = s_uid
            End Get
        End Property

        ReadOnly Property icdid() As String
            Get
                icdid = s_icdid
            End Get
        End Property
        Public Property icdrev() As String
            Get
                icdrev = s_icdrev
            End Get
            Set(value As String)
                If value <> s_icdrev Then
                    s_icdrev = value
                    Me.IsChanged = True
                End If
            End Set
        End Property


        Public Property dept1() As String
            Get
                dept1 = s_dept1
            End Get
            Set(value As String)
                If value <> dept1 Then
                    s_dept1 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        Public Property assy1() As String
            Get
                assy1 = s_assy1
            End Get
            Set(value As String)
                If value <> s_assy1 Then
                    s_assy1 = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        Public Property desc1() As String
            Get
                desc1 = s_desc1
            End Get
            Set(value As String)
                s_desc1 = value
                Me.IsChanged = True
            End Set
        End Property


        Public Property dept2() As String
            Get
                dept2 = s_dept2
            End Get
            Set(value As String)
                s_dept2 = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property assy2() As String
            Get
                assy2 = s_assy2
            End Get
            Set(value As String)
                s_assy2 = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property desc2() As String
            Get
                desc2 = s_desc2
            End Get
            Set(value As String)
                s_desc2 = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property Cartypes() As clsLEGACYCartypes
            Get
                Cartypes = s_cartypes
            End Get
            Set(value As clsLEGACYCartypes)
                s_cartypes = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property IFClass() As String
            Get
                IFClass = s_class
            End Get
            Set(value As String)
                If s_class <> value Then
                    s_class = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        Public Property status() As String
            Get
                status = s_status
            End Get
            Set(value As String)
                If value <> s_status Then
                    s_status = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        '**** infuese the object by a OTDBRecord
        '****
        Public Function infuse(ByRef aRecord As ormRecord) As Boolean
            '            Dim i As Integer
            '            Dim aCartypes As New clsCartypes
            '            Dim fieldname As String
            '            Dim flag As Boolean
            '            Dim Value As Object

            '            '* init
            '            If Not Me.IsInitialized Then
            '                If Not initialize() Then
            '                    infuse = False
            '                    Exit Function
            '                End If
            '            End If

            '            If aRecord.TableID <> ourTablename Then
            '                infuse = False
            '                Exit Function
            '            End If

            '            On Error GoTo errorhandle

            '            Me.Record = aRecord
            '            s_uid = CLng(aRecord.GetValue("uid"))
            '            s_icdid = CStr(aRecord.GetValue("icdid"))
            '            s_icdrev = CStr(aRecord.GetValue("icdrev"))
            '            s_dept1 = CStr(aRecord.GetValue("dept1"))
            '            s_assy1 = CStr(aRecord.GetValue("assy1"))
            '            s_desc1 = CStr(aRecord.GetValue("desc1"))

            '            s_dept2 = CStr(aRecord.GetValue("dept2"))
            '            s_assy2 = CStr(aRecord.GetValue("assy2"))
            '            s_desc2 = CStr(aRecord.GetValue("desc2"))

            '            s_class = CStr(aRecord.GetValue("class"))
            '            s_status = CStr(aRecord.GetValue("status"))
            '            If IsDate(aRecord.GetValue("chgdt")) Then s_changedOn = CDate(aRecord.GetValue("chgdt"))

            '            ' set cartypes
            '            For i = 1 To aCartypes.getNoCars
            '                fieldname = "h" & Format(i, "0#")
            '                flag = False
            '                Value = aRecord.GetValue(fieldname)
            '                If TypeName(Value) = "Boolean" Then
            '                    If Value = True Then
            '                        flag = CBool(aRecord.GetValue(fieldname))
            '                    Else
            '                        flag = False
            '                    End If
            '                ElseIf Not IsEmpty(Value) And Not Value = String.empty Then
            '                    flag = True
            '                End If

            '                If flag Then Call aCartypes.addCartypeByIndex(i)

            '            Next i
            '            s_cartypes = aCartypes

            '            _updatedOn = CDate(aRecord.GetValue(ConstFNUpdatedOn))

            '            infuse = MyBase.Infuse(aRecord)
            '            me.isloaded = infuse
            '            Exit Function

            'errorhandle:
            '            infuse = False


        End Function


        '**** getICD as clsOTDBICD
        '****
        Public Function getICD() As clsOTDBICD

            If Me.isloaded Then
                Dim anewICD As New clsOTDBICD
                If anewICD.Inject(Me.icdid, Me.icdrev) Then
                    getICD = anewICD
                    Exit Function
                End If
            End If

            '
            getICD = Nothing
            Exit Function
        End Function


        '**** getAssyisSender : returns true if the Assy # pairno is the sender
        '****
        Public Function getAssyisSender(pairno As Integer) As Boolean
            Dim Sender() As String
            Dim Receiver() As String
            Dim i As Integer
            Dim assycode As String
            Dim Value As String

            If Me.isloaded Then
                If pairno = 1 Then
                    assycode = Me.assy1
                ElseIf pairno = 2 Then
                    assycode = Me.assy2
                Else
                    'error
                    System.Diagnostics.Debug.WriteLine("clsOTDBInterface.getAssyisSender: " & pairno & " is not 1 oder 2 for the pairno")
                    Exit Function
                End If
                ' get ICD
                Dim anewICD As New clsOTDBICD
                anewICD = Me.getICD()
                If Not anewICD Is Nothing And anewICD.IsLoaded Then
                    Receiver = anewICD.receiver_assycode()
                    Sender = anewICD.sender_assycode()
                    ' if we have a sender
                    If IsArrayInitialized(Sender) Then
                        ' search the sender
                        For i = LBound(Sender) To UBound(Sender)
                            If Sender(i) = assycode Then
                                getAssyisSender = True
                                Exit Function
                            End If
                        Next i
                    ElseIf IsArrayInitialized(Receiver) Then
                        For i = LBound(Receiver) To UBound(Receiver)
                            If Receiver(i) = assycode Then
                                getAssyisSender = False
                                Exit Function
                            End If
                        Next i
                    Else
                        ' there is no sender or receiver
                        ' determine by ICD No.

                        If LCase(Me.dept1) = LCase(Mid(Me.icdid, 8, 1)) And pairno = 1 Then
                            getAssyisSender = True
                            Exit Function
                        ElseIf LCase(Me.dept2) = LCase(Mid(Me.icdid, 8, 1)) And pairno = 2 Then
                            getAssyisSender = True
                            Exit Function
                        Else
                            getAssyisSender = True
                        End If

                    End If    'sender exists
                Else
                    ' determine by ICD No.
                    If LCase(Me.dept1) = LCase(Mid(Me.icdid, 8, 1)) And pairno = 1 Then
                        getAssyisSender = True
                        Exit Function
                    ElseIf LCase(Me.dept2) = LCase(Mid(Me.icdid, 8, 1)) And pairno = 2 Then
                        getAssyisSender = True
                        Exit Function
                    Else
                        getAssyisSender = True
                    End If
                End If    ' ICD is nothing

            End If

            '
            getAssyisSender = False
            Exit Function

        End Function
        '**** Inject : load the object by the PrimaryKeys
        '****
        Public Function Inject(UID As Long) As Boolean
            Dim aTable As iormDataStore
            Dim pkarry() As Object
            Dim aRecord As ormRecord

            '* init
            If Not Me.IsInitialized Then
                If Not initialize() Then
                    Inject = False
                    Exit Function
                End If
            End If
            ' set the primaryKey
            ReDim pkarry(1)
            pkarry(0) = UID


            'aTable = GetTableStore(Me.Record.TableIDs)
            aRecord = aTable.GetRecordByPrimaryKey(pkarry)

            If aRecord Is Nothing Then
                Me.Unload()
                Inject = Me.IsLoaded
                Exit Function
            Else
                Me.Record = aRecord
                'me.isloaded = Me.infuse(Me.Record)
                Inject = Me.IsLoaded
                Exit Function
            End If


        End Function

        '****** allByAssyCode: "static" function to return a collection of clsOTDBInterfaces
        '******
        '****** expects assycode to be in the form xx.yy.zz
        '****** selectCartypes is regarded as "OR" cartypes


        Public Function allByAssyCode(ByVal assycode As String, ByRef selectCartypes As clsLEGACYCartypes) As Collection
            Dim aCollection As New Collection
            Dim aRecordCollection As List(Of ormRecord)
            Dim aTable As iormDataStore
            Dim aRecord As ormRecord
            Dim wherestr As String
            Dim i As Integer
            Dim flag As Boolean
            Dim anInterface As clsOTDBInterface

            ' create the where-clause

            wherestr = "(assy1 = '" & assycode & "' or assy2 = '" & assycode & "') and ("
            For i = 1 To selectCartypes.getNoCars

                If selectCartypes.getCar(i) Then
                    If flag Then
                        wherestr = wherestr & " or "
                    End If
                    wherestr = wherestr & "h" & Format(i, "0#") & "="
                    wherestr = wherestr & "true"

                    flag = True
                Else
                    'wherestr = wherestr & "false"
                End If

            Next i
            If flag Then
                wherestr = wherestr & ")"
            Else
                System.Diagnostics.Debug.WriteLine("clsOTDBInterface.allByAssyCode: selectCartypes has no cartypes to select on")
                Call CoreMessageHandler(message:="selectCartypes has no cartypes to select on" _
                                                        , arg1:=Me.UID & " " & assycode & " on " & selectCartypes.show, subname:="cl,sOTDBInterface.allByAssyCode" _
                                                                                                                                 , break:=False)
                GoTo error_handler
            End If

            'Debug.Print wherestr

            On Error GoTo error_handler

            ' aTable = GetTableStore(Me.Record.TableIDs)
            aRecordCollection = aTable.GetRecordsBySql(wherestr, silent:=True)

            If aRecordCollection Is Nothing Then
                Me.Unload()
                allByAssyCode = Nothing
                Exit Function
            Else
                For Each aRecord In aRecordCollection
                    anInterface = New clsOTDBInterface
                    If anInterface.infuse(aRecord) Then
                        aCollection.Add(Item:=anInterface)
                    End If
                Next aRecord
                allByAssyCode = aCollection
                Exit Function
            End If

error_handler:

            allByAssyCode = Nothing
            Exit Function
        End Function


    End Class



    Public Class clsOTDBICD
        Inherits ormDataObject

        '************************************************************************************
        '***** CLASS clsOTDBICD is the object for a OTDBRecord (which is the datastore)
        '*****
        '*****
        Const ourTablename As String = "tblicd"

        Private s_icdid As String
        Private s_icdrev As String

        Private s_sassy() As String
        Private s_sdept As String
        Private s_seditor As String
        Private s_sresp As String

        Private s_rassy() As String
        Private s_rdept As String
        Private s_receiver As String

        Private s_desc As String
        Private s_class As String

        Private s_cartypes As New clsLEGACYCartypes
        Private s_status As String
        Private s_statdt As Date
        Private s_statuscmt As String

        Private s_changedOn As Date
        Private s_duedt As Date

        '** initialize
        Public Sub New()
            Call MyBase.New(ourTablename)
        End Sub
        '*** init
        Public Function initialize() As Boolean
            initialize = MyBase.Initialize()

        End Function

#Region "********* Properties ************ "
        ReadOnly Property icdid() As String
            Get
                icdid = s_icdid
            End Get

        End Property
        ReadOnly Property icdrev() As String
            Get
                icdrev = s_icdrev
            End Get

        End Property

        Public Property sender_dept() As String
            Get
                sender_dept = s_sdept
            End Get
            Set(value As String)
                If value <> s_sdept Then
                    s_sdept = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        Public Property sender_assycode() As String()
            Get
                sender_assycode = s_sassy
            End Get
            Set(value As String())
                s_sassy = value
                Me.IsChanged = True


            End Set
        End Property

        Public Property sender_responsible() As String
            Get
                sender_responsible = s_sresp
            End Get
            Set(value As String)
                If s_sresp <> value Then
                    s_sresp = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        Public Property sender_editor() As String
            Get
                sender_editor = s_seditor
            End Get
            Set(value As String)
                If value <> s_seditor Then
                    s_seditor = value
                    Me.IsChanged = True

                End If
            End Set
        End Property

        Public Property desc() As String
            Get
                desc = s_desc
            End Get
            Set(value As String)
                s_desc = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property receiver_dept() As String
            Get
                receiver_dept = s_rdept
            End Get
            Set(value As String)
                If value <> s_rdept Then
                    s_rdept = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        Public Property receiver_assycode() As String()
            Get
                receiver_assycode = s_rassy
            End Get
            Set(value As String())
                s_rassy = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property Receiver() As String
            Get
                Receiver = s_receiver
            End Get
            Set(value As String)
                s_receiver = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property Cartypes() As clsLEGACYCartypes
            Get
                Cartypes = s_cartypes

            End Get
            Set(value As clsLEGACYCartypes)
                s_cartypes = value
                Me.IsChanged = True
            End Set
        End Property


        Public Property IFClass() As String
            Get
                IFClass = s_class
            End Get
            Set(value As String)
                s_class = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property status() As String
            Get
                status = s_status
            End Get
            Set(value As String)
                s_status = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property status_comment() As String
            Get
                status_comment = s_statuscmt

            End Get
            Set(value As String)
                s_statuscmt = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property changedOn() As Date
            Get
                changedOn = s_changedOn
            End Get
            Set(value As Date)
                If value <> s_changedOn Then
                    s_changedOn = value
                    Me.IsChanged = True
                End If
            End Set
        End Property

        Public Property statusFrom() As Date
            Get
                statusFrom = s_statdt
            End Get
            Set(value As Date)
                s_statdt = value
                Me.IsChanged = True
            End Set
        End Property

        Public Property dueOn() As Date
            Get
                dueOn = s_duedt
            End Get
            Set(value As Date)
                If value <> s_duedt Then
                    s_duedt = value
                    Me.IsChanged = True
                End If
            End Set
        End Property
#End Region

        '**** infuese the object by a OTDBRecord
        '****
        Public Function infuse(ByRef aRecord As ormRecord) As Boolean
            '            Dim i As Integer
            '            Dim aCartypes As New clsCartypes
            '            Dim fieldname As String
            '            Dim flag As Boolean
            '            Dim Value As Object
            '            '* init
            '            If Not Me.IsInitialized Then
            '                If Not initialize() Then
            '                    Return False
            '                    Exit Function
            '                End If
            '            End If


            '            On Error GoTo errorhandle

            '            Me.Record = aRecord

            '            s_icdid = CStr(aRecord.GetValue("icdid"))
            '            s_icdrev = CStr(aRecord.GetValue("rev"))
            '            s_sdept = CStr(aRecord.GetValue("sdept"))
            '            s_sassy = Split(CStr(aRecord.GetValue("sassy")), ",")
            '            s_seditor = CStr(aRecord.GetValue("seditor"))
            '            s_sresp = CStr(aRecord.GetValue("sresp"))

            '            s_desc = CStr(aRecord.GetValue("desc"))

            '            s_rdept = CStr(aRecord.GetValue("rdept"))
            '            s_rassy = Split(CStr(aRecord.GetValue("rassy")), ",")
            '            s_receiver = CStr(aRecord.GetValue("receiver"))

            '            s_class = CStr(aRecord.GetValue("class"))
            '            s_status = CStr(aRecord.GetValue("status"))
            '            s_statuscmt = CStr(aRecord.GetValue("statuscmt"))
            '            If IsDate(aRecord.GetValue("statdt")) Then s_statdt = CDate(aRecord.GetValue("statdt"))
            '            If IsDate(aRecord.GetValue("duedt")) Then s_duedt = CDate(aRecord.GetValue("duedt"))

            '            ' set cartypes
            '            For i = 1 To aCartypes.getNoCars
            '                fieldname = "h" & Format(i, "0#")
            '                Value = aRecord.GetValue(fieldname)
            '                If TypeName(Value) = "boolean" Then
            '                    If Value = True Then
            '                        flag = CBool(aRecord.GetValue(fieldname))
            '                    End If
            '                ElseIf Not IsEmpty(Value) And Not Value = String.empty Then
            '                    flag = True
            '                End If

            '                If flag Then Call aCartypes.addCartypeByIndex(i)
            '            Next i
            '            s_cartypes = aCartypes

            '            _updatedOn = CDate(aRecord.GetValue(ConstFNUpdatedOn))

            '            infuse = MyBase.Infuse(aRecord)
            '            me.isloaded = infuse
            '            Exit Function

            'errorhandle:
            '            infuse = False


        End Function

        '**** Inject : load the object by the PrimaryKeys
        '****
        Public Function Inject(icdid As String, Optional icdrev As String = String.empty) As Boolean
            Dim aTable As iormDataStore
            Dim pkarry() As Object
            Dim aRecord As ormRecord

            ' set the primaryKey
            If IsMissing(icdrev) Then
                ReDim pkarry(1)
                pkarry(0) = icdid
            Else
                ReDim pkarry(2)
                pkarry(0) = icdid
                pkarry(1) = icdrev
            End If


            'aTable = GetTableStore(Me.Record.TableIDs)
            aRecord = aTable.GetRecordByPrimaryKey(pkarry)

            If aRecord Is Nothing Then
                Me.Unload()
                Inject = Me.IsLoaded
                Exit Function
            Else
                Me.Record = aRecord
                'me.isloaded = Me.infuse(Me.Record)
                Inject = Me.IsLoaded
                Exit Function
            End If


        End Function
    End Class
End Namespace
