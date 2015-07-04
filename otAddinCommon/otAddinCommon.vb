

REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE TOOLING 4 VSTO
REM *********** 
REM *********** Commons Static and other Information
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

Imports OnTrack.Database
Imports OnTrack.Core
Public Module otAddinCommon
    ''' <summary>
    ''' Telerik Theme Name
    ''' </summary>
    ''' <remarks></remarks>
    Public Const ConstCPNUITheme As String = "parameter_ui_telerik_theme"

    ''' <summary>
    ''' Major Version of the 
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <ormChangeLogEntry(Application:=ConstAssemblyName, module:="", version:=2, release:=1, patch:=0, changeimplno:=5, _
     description:="Adapted to be used on otBackend V2R1")> _
  <ormChangeLogEntry(Application:=ConstAssemblyName, module:="", version:=1, release:=1, patch:=2, changeimplno:=4, _
     description:="Rework adding data to the DBExplorer Grids")> _
  <ormChangeLogEntry(Application:=ConstAssemblyName, module:="", version:=1, release:=1, patch:=2, changeimplno:=3, _
     description:="Password Editor for encrypted entry properties")> _
 <ormChangeLogEntry(Application:=ConstAssemblyName, module:="", version:=1, release:=1, patch:=2, changeimplno:=2, _
     description:="Reworked the validation on the UIControlDataGridView. Reworked the Column Size Setting")> _
 <ormChangeLogEntry(Application:=ConstAssemblyName, module:="", version:=1, release:=1, patch:=2, changeimplno:=1, _
     description:="Added the View on Changes in About Box")> _
    Public Const ConstMajorVersion As UInt16 = 2
    ''' <summary>
    ''' minor Version
    ''' </summary>
    ''' <remarks></remarks>
    Public Const ConstMinorVersion As UInt16 = 0

    ''' <summary>
    ''' application name
    ''' </summary>
    ''' <remarks></remarks>
    Public Const ConstAssemblyName = "otUICommon"


    ''' <summary>
    ''' private versions
    ''' </summary>
    ''' <remarks></remarks>
    Private _ApplicationVersion As Version
    Private _ApplicationName As String

    ''' <summary>
    ''' Gets or sets the top application version version.
    ''' </summary>
    ''' <value>The version.</value>
    Public Property ApplicationVersion() As Version
        Get
            If _ApplicationVersion Is Nothing Then
                Dim aVersion As Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version
                _ApplicationVersion = New Version(ot.ConstMajorVersion, ot.ConstMinorVersion, aVersion.Build)
            End If
            Return _ApplicationVersion
        End Get
        Set(value As Version)
            _ApplicationVersion = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the name of the top application.
    ''' </summary>
    ''' <value>The name of the application.</value>
    Public Property ApplicationName() As String
        Get
            If String.IsNullOrWhiteSpace(_ApplicationName) Then
                Return System.Reflection.Assembly.GetExecutingAssembly().GetName().FullName
            End If

            Return _ApplicationName
        End Get
        Set(value As String)
            _ApplicationName = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the version.
    ''' </summary>
    ''' <value>The version.</value>
    Public ReadOnly Property AssemblyVersion() As Version
        Get
            Dim aVersion As Version = System.Reflection.Assembly.GetAssembly(GetType(UIAboutBox)).GetName().Version
            Return New Version(otAddinCommon.ConstMajorVersion, otAddinCommon.ConstMinorVersion, aVersion.Build)
        End Get
    End Property
    ''' <summary>
    ''' gets the application name
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property AssemblyName As String
        Get
            Return ConstAssemblyName
        End Get
    End Property
End Module
