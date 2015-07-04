
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
Imports OnTrack.UI
Imports OnTrack.Core




Public Class UITelerikMessageBox
    Implements iUINativeFormMessageBox

    Private _shadow As CoreMessageBox

    Private _title As String
    Private _message As String
    Private _result As CoreMessageBox.ResultType
    Private _buttons As CoreMessageBox.ButtonType
    Private _type As CoreMessageBox.MessageType
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
    Public Property Type() As CoreMessageBox.MessageType Implements iUINativeFormMessageBox.Type
        Get
            Return Me._type
        End Get
        Set(value As CoreMessageBox.MessageType)
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
            Case CoreMessageBox.ButtonType.OK
                buttons = MessageBoxButtons.OK
            Case CoreMessageBox.ButtonType.YesNo
                buttons = MessageBoxButtons.YesNo
            Case CoreMessageBox.ButtonType.YesNoCancel
                buttons = MessageBoxButtons.YesNoCancel
            Case CoreMessageBox.ButtonType.OKCancel
                buttons = MessageBoxButtons.OKCancel
            Case Else
                buttons = MessageBoxButtons.OK
        End Select

        Select Case _type
            Case CoreMessageBox.MessageType.Error
                icon = RadMessageIcon.Error
                title &= " ERROR: " & _title
            Case CoreMessageBox.MessageType.Info
                icon = RadMessageIcon.Info
                title &= " INFORMATION: " & _title
            Case CoreMessageBox.MessageType.Question
                icon = RadMessageIcon.Question
                title &= " FEEDBACK REQUIRED: " & _title
            Case CoreMessageBox.MessageType.Warning
                icon = RadMessageIcon.Exclamation
                title &= " WARNING: " & _title
            Case Else
                icon = RadMessageIcon.None
                title &= " : " & _title
        End Select

        '*** CALL THE MESSAGEBOX
        RadMessageBox.SetThemeName(ot.GetConfigProperty(name:=Global.OnTrack.UI.ConstCPNUITheme))
        result = RadMessageBox.Show(text:=Me.Message, caption:=title, buttons:=buttons, icon:=icon)

        '** select on the result
        Select Case result
            Case DialogResult.No
                _result = CoreMessageBox.ResultType.No
            Case DialogResult.Cancel
                _result = CoreMessageBox.ResultType.Cancel
            Case DialogResult.OK
                _result = CoreMessageBox.ResultType.Ok
            Case DialogResult.Yes
                _result = CoreMessageBox.ResultType.Yes
            Case Else
                _result = CoreMessageBox.ResultType.None
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
    Public Property Buttons() As CoreMessageBox.ButtonType Implements iUINativeFormMessageBox.Buttons
        Get
            Return _buttons
        End Get
        Set(value As CoreMessageBox.ButtonType)
            _buttons = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the answer.
    ''' </summary>
    ''' <value>The answer.</value>
    Public Property Result() As CoreMessageBox.ResultType Implements iUINativeFormMessageBox.Result
        Get
            Return _result
        End Get
        Set(value As CoreMessageBox.ResultType)
            _result = value
        End Set
    End Property

End Class

