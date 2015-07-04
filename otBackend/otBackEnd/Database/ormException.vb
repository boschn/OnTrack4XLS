
REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** classes for ORM exception handling
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2013-08-08
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************''' <summary>
Option Explicit On
Imports OnTrack.Core

Namespace OnTrack.Database

    ''' <summary>
    ''' ORMException is an Exception for the ORM LAyer
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ormException
        Inherits Exception

        ''' <summary>
        ''' defines the exception types
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum Types
            None = 0
            NoDataObjectProviderFound
            TypeNotFound
            NoRepositoryAvailable
            SessionNotInitialized
            NoPrimaryDatabaseDriverFound
            NoObjectIDFound
            WrongDataObjectProvider
            NoObjectClassDescription
            NoDatabaseDriver
            WrongDriverType
            WrongRule

        End Enum
        ''' <summary>
        ''' predefined messages
        ''' </summary>
        ''' <remarks></remarks>
        Private _buildinmessages As String() = { _
            "", _
            "A data object provider could not be retrieved for object id '{0}' in domain '{1}'",
            "A data object class for .net type fullname '{0}' was not found in the repository'",
            "No repository in session for domaind '{0}' available",
            "Session is not initialized and not available",
            "No primary database driver found for containerID '{0}'",
            "No data object by object id '{0}' was found in repository",
            "This data object provider '{1}' is not handling data object id '{0}'",
            "No Data Object Class Description '{0}' could be retrieved from repository",
            "No Database Driver could be retrieved for data object id '{0}'",
            "Database Driver for container '{0}' must be '{1}'",
            "Rule type '{1}' expected - rule is of type '{0}' instead"
            }
        Protected _type As ormException.Types
        Protected _InnerException As Exception
        Protected _message As String
        Protected _procedure As String
        Protected _path As String ' Database path
        Protected _arguments As Object()

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="type"></param>
        ''' <param name="message"></param>
        ''' <param name="exception"></param>
        ''' <param name="procedure"></param>
        ''' <param name="path"></param>
        ''' <param name="arguments"></param>
        ''' <remarks></remarks>
        Public Sub New(Optional type As ormException.Types = 0, _
                       Optional message As String = Nothing, _
                       Optional exception As Exception = Nothing, _
                       Optional procedure As String = Nothing, _
                       Optional path As String = Nothing, _
                       Optional arguments As Object() = Nothing)

            If type <> 0 AndAlso String.IsNullOrWhiteSpace(message) Then
                message = String.Format(_buildinmessages(CInt(type)), arguments)
                _type = type
            End If
            If Not String.IsNullOrWhiteSpace(message) Then _message = message
            If Not String.IsNullOrWhiteSpace(procedure) Then _procedure = procedure
            If exception IsNot Nothing Then _InnerException = exception
            If Not String.IsNullOrWhiteSpace(path) Then _path = path
            If arguments IsNot Nothing Then _arguments = arguments
        End Sub

        ''' <summary>
        ''' Gets the type of the Exception
        ''' </summary>
        ''' <value>The path.</value>
        Public ReadOnly Property Type() As ormException.Types
            Get
                Return Me._type
            End Get
        End Property
        ''' <summary>
        ''' Gets the path.
        ''' </summary>
        ''' <value>The path.</value>
        Public ReadOnly Property Path() As String
            Get
                Return Me._path
            End Get
        End Property

        ''' <summary>
        ''' Gets the subname.
        ''' </summary>
        ''' <value>The subname.</value>
        Public ReadOnly Property Procedure() As String
            Get
                Return Me._procedure
            End Get
        End Property

        ''' <summary>
        ''' Gets the message.
        ''' </summary>
        ''' <value>The message.</value>
        Public ReadOnly Property Message() As String
            Get
                Return Me._message
            End Get
        End Property

        ''' <summary>
        ''' Gets the inner exception.
        ''' </summary>
        ''' <value>The inner exception.</value>
        Public ReadOnly Property InnerException() As Exception
            Get
                Return Me._InnerException
            End Get
        End Property
        ''' <summary>
        ''' return the array of object which are the arguments
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Arguments As Object()
            Get
                Return _arguments
            End Get
        End Property
    End Class

    ''' <summary>
    ''' No Connection Excpetion
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ormNoConnectionException
        Inherits ormException
        Public Sub New(Optional message As String = Nothing, Optional exception As Exception = Nothing, Optional subname As String = Nothing, Optional path As String = Nothing)
            MyBase.New(message:=message, exception:=exception, procedure:=subname, path:=path)
        End Sub

    End Class

    ''' <summary>
    ''' Event arguments for Ontrack error Events
    ''' </summary>
    ''' <remarks></remarks>

    Public Class ormErrorEventArgs
        Inherits EventArgs

        Private _error As SessionMessage

        Public Sub New(newError As SessionMessage)
            _error = newError
        End Sub
        ''' <summary>
        ''' Gets the error.
        ''' </summary>
        ''' <value>The error.</value>
        Public ReadOnly Property [Error]() As SessionMessage
            Get
                Return Me._error
            End Get
        End Property

    End Class
End Namespace
