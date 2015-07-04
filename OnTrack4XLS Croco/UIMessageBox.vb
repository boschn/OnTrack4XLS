
REM ***********************************************************************************************************************************************
REM *********** Telerik RAD MessageBox Version for OTDB
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

Imports System.Windows.Forms
Imports Telerik.WinControls
Imports OnTrack

Public Class UITelerikMessageBox
    Implements iUINativeFormMessageBox

    Private _shadow As clsCoreUIMessageBox

    Private _title As String
    Private _message As String
    Private _result As clsCoreUIMessageBox.ResultType
    Private _buttons As clsCoreUIMessageBox.ButtonType
    Private _type As clsCoreUIMessageBox.MessageType
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' _messagebox = New MessageBox

    End Sub

    ''' <summary>
    ''' Gets or sets the type.
    ''' </summary>
    ''' <value>The type.</value>
    Public Property Type() As clsCoreUIMessageBox.MessageType Implements iUINativeFormMessageBox.Type
        Get
            Return Me._type
        End Get
        Set(value As clsCoreUIMessageBox.MessageType)
            Me._type = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the title.
    ''' </summary>
    ''' <value>The title.</value>
    Public Property Title() As String Implements iUINativeFormMessageBox.Title
        Get
            Return _title
        End Get
        Set(value As String)
            _title = value
        End Set
    End Property

    ''' <summary>
    ''' Connect with OTDB counterpart
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' Property OTDBParent As iOTDBAbstractUIForm ' for call back to the OTDB UI Form
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <value></value>
    Public Property OtdbShadow() As iOTDBUIAbstractForm Implements iUINativeForm.OtdbShadow
        Get
            Return _shadow
        End Get
        Set(value As iOTDBUIAbstractForm)
            _shadow = value
        End Set

    End Property

    ''' <summary>
    ''' Show the Form
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    ''' Property OTDBParent As iOTDBAbstractUIForm ' for call back to the OTDB UI Form
    ''' <remarks></remarks>
    ''' <returns></returns>
    Public Function ShowOTDBForm() As Object Implements iUINativeForm.ShowOTDBForm
        Dim result As DialogResult
        Dim buttons As MessageBoxButtons
        Dim icon As MessageBoxIcon
        Dim title As String = "OnTrack Database"
        Dim defaultbutton As MessageBoxDefaultButton

        Select Case _buttons
            Case clsCoreUIMessageBox.ButtonType.OK
                buttons = MessageBoxButtons.OK
            Case clsCoreUIMessageBox.ButtonType.YesNo
                buttons = MessageBoxButtons.YesNo
            Case clsCoreUIMessageBox.ButtonType.YesNoCancel
                buttons = MessageBoxButtons.YesNoCancel
            Case clsCoreUIMessageBox.ButtonType.OKCancel
                buttons = MessageBoxButtons.OKCancel
            Case Else
                buttons = MessageBoxButtons.OK
        End Select

        Select Case _type
            Case clsCoreUIMessageBox.MessageType.Error
                icon = RadMessageIcon.Error
                title &= " ERROR: " & _title
            Case clsCoreUIMessageBox.MessageType.Info
                icon = RadMessageIcon.Info
                title &= " INFORMATION: " & _title
            Case clsCoreUIMessageBox.MessageType.Question
                icon = RadMessageIcon.Question
                title &= " FEEDBACK REQUIRED: " & _title
            Case clsCoreUIMessageBox.MessageType.Warning
                icon = RadMessageIcon.Exclamation
                title &= " WARNING: " & _title
            Case Else
                icon = RadMessageIcon.None
                title &= " : " & _title
        End Select

        '*** CALL THE MESSAGEBOX
        RadMessageBox.SetThemeName("TelerikMetroBlue")
        result = RadMessageBox.Show(text:=Me.Message, caption:=title, buttons:=buttons, icon:=icon)

        '** select on the result
        Select Case result
            Case DialogResult.No
                _result = clsCoreUIMessageBox.ResultType.No
            Case DialogResult.Cancel
                _result = clsCoreUIMessageBox.ResultType.Cancel
            Case DialogResult.OK
                _result = clsCoreUIMessageBox.ResultType.Ok
            Case DialogResult.Yes
                _result = clsCoreUIMessageBox.ResultType.Yes
            Case Else
                _result = clsCoreUIMessageBox.ResultType.None
        End Select

    End Function

    ''' <summary>
    ''' Close the Form
    ''' </summary>
    ''' <remarks></remarks>
    ''' <returns></returns>
    Public Function CloseOTDBForm() As Object Implements iUINativeForm.CloseOTDBForm
        Return False
    End Function

    ''' <summary>
    ''' Refresh the Form
    ''' </summary>
    ''' <remarks></remarks>
    ''' <returns></returns>
    Public Function RefreshOTDBForm() As Object Implements iUINativeForm.RefreshOTDBForm
        Return False
    End Function

    ''' <summary>
    ''' Gets or sets the message.
    ''' </summary>
    ''' <value>The message.</value>
    Public Property Message() As String Implements iUINativeFormMessageBox.Message
        Get
            Return _message
        End Get
        Set(value As String)
            _message = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the buttons.
    ''' </summary>
    ''' <value>The buttons.</value>
    Public Property Buttons() As clsCoreUIMessageBox.ButtonType Implements iUINativeFormMessageBox.Buttons
        Get
            Return _buttons
        End Get
        Set(value As clsCoreUIMessageBox.ButtonType)
            _buttons = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the answer.
    ''' </summary>
    ''' <value>The answer.</value>
    Public Property Result() As clsCoreUIMessageBox.ResultType Implements iUINativeFormMessageBox.Result
        Get
            Return _result
        End Get
        Set(value As clsCoreUIMessageBox.ResultType)
            _result = value
        End Set
    End Property

End Class