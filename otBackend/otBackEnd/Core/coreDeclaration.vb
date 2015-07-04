REM ***********************************************************************************************************************************************
REM *********** CORE CLASSES DEFINITIONS (Enumerations, Interfaces, Types) for On Track Database Backend Library
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

Imports System.Collections
Imports System.ComponentModel
Imports OnTrack
Imports OnTrack.Database
Imports System.Reflection

Namespace OnTrack.Core

    ''' <summary>
    ''' LinkTypes for be used in linking objects types
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Enum otLinkType
        One2One = 1
    End Enum
    ''' <summary>
    ''' Structure to Use to Validate UserInformation
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure UserValidation
        Public ValidEntry As Boolean

        Public Username As String
        Public Password As String
        Public IsProhibited As Boolean
        Public IsAnonymous As Boolean
        Public HasNoRights As Boolean
        Public HasReadRights As Boolean
        Public HasUpdateRights As Boolean
        Public HasAlterSchemaRights As Boolean
    End Structure

    '************************************************************************************
    '**** INTERFACE iOTDBForm defines a Wrapper for a Form UI for the Core to use
    '****           
    '****

    Public Interface iOTDBUIAbstractForm

    End Interface

    ''' <summary>
    ''' Message types of the On Track Database Core
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otCoreMessageType
        InternalError = 1
        InternalWarning = 2
        InternalException = 3
        InternalInfo = 7
        ApplicationError = 4
        ApplicationWarning = 5
        ApplicationInfo = 6
        ApplicationException = 8
    End Enum

    ' Enum ofRelativeToInterval

    Public Enum otIntervalRelativeType
        IntervalRight = -1
        IntervalMiddle = 0
        IntervalLeft = 1
        IntervalInvalid = -2

    End Enum

    'LogMessageTypes

    Public Enum otObjectMessageType
        [Error] = 1
        Info = 3
        Attention = 2
        Warning = 4
    End Enum


End Namespace


