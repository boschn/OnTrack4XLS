REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** UI Model Controller
REM *********** 
REM *********** Version: 2.00
REM *********** Created: 2015-02-13
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2015
REM ***********************************************************************************************************************************************
Option Explicit On
Imports System.Collections
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Diagnostics.Debug
Imports OnTrack.Database

Namespace OnTrack.UI

    ''' <summary>
    ''' a small controller in a MVC setup
    ''' </summary>
    ''' <remarks>
    ''' functional design principles
    ''' </remarks>
    Public Class MVController
        ''' <summary>
        ''' Controller Event Args
        ''' </summary>
        ''' <remarks></remarks>
        Public Class EventArgs
            Inherits System.EventArgs

            ''' <summary>
            ''' Constructor
            ''' </summary>
            ''' <remarks></remarks>
            Public Sub New()
            End Sub

        End Class
    End Class


    ''' <summary>
    ''' a controller in a MVC envirorment - based on data object handling
    ''' </summary>
    ''' <remarks>
    ''' functional design principles
    ''' 1. enable state operations e.g CRUD on a current data object
    ''' 2. hold a data object and able to change it and inform the event receivers
    ''' 3. Raise Events to inform Handlers about the state change, handlers might cancel the change
    ''' 4. raise event about changing to new data object
    ''' </remarks>
    Public Class MVDataObjectController
        Inherits MVController
        ''' <summary>
        ''' Controller Event Args
        ''' </summary>
        ''' <remarks></remarks>
        Public Class EventArgs
            Inherits MVController.EventArgs

            Private _DataObject As iormRelationalPersistable
            Private _State As CRUDState
            Private _abortNewState As Boolean = False
            ''' <summary>
            ''' Constructor
            ''' </summary>
            ''' <remarks></remarks>
            Public Sub New(Optional dataobject As iormRelationalPersistable = Nothing, Optional state As CRUDState = Nothing)
                MyBase.New()
                _DataObject = dataobject
                _State = state
            End Sub

            ''' <summary>
            ''' Gets or sets the new state of the abort.
            ''' </summary>
            ''' <value>The new state of the abort.</value>
            Public Property AbortNewState() As Boolean
                Get
                    Return _abortNewState
                End Get
                Set(value As Boolean)
                    _abortNewState = value
                End Set
            End Property

            ''' <summary>
            ''' Gets the state.
            ''' </summary>
            ''' <value>The state.</value>
            Public ReadOnly Property State() As CRUDState
                Get
                    Return _State
                End Get
            End Property

            ''' <summary>
            ''' Gets the data object.
            ''' </summary>
            ''' <value>The data object.</value>
            Public ReadOnly Property DataObject() As iormRelationalPersistable
                Get
                    Return _DataObject
                End Get
            End Property

        End Class
        ''' <summary>
        ''' Enum State as State Definition of the Controller
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum CRUDState As UShort
            Read
            Create
            Update
            Delete
        End Enum

        Private _state As CRUDState = CRUDState.Read
        Private _dataobject As iormRelationalPersistable

        ''' <summary>
        ''' Events of change states
        ''' </summary>
        ''' <remarks></remarks>
        Public Event OnChangingToRead As EventHandler(Of MVDataObjectController.EventArgs)
        Public Event OnChangingToCreate As EventHandler(Of MVDataObjectController.EventArgs)
        Public Event OnChangingToUpdate As EventHandler(Of MVDataObjectController.EventArgs)
        Public Event OnChangingToDelete As EventHandler(Of MVDataObjectController.EventArgs)

        Public Event OnChangingDataObject As EventHandler(Of MVDataObjectController.EventArgs)

        ''' <summary>
        ''' Gets or sets the state.
        ''' </summary>
        ''' <value>The state.</value>
        Public Property State As CRUDState
            Get
                Return _state
            End Get
            Set(value As CRUDState)
                Dim eventargs As New MVDataObjectController.EventArgs(dataobject:=Me.Dataobject, state:=value)

                ''' raise event
                Select Case value
                    Case CRUDState.Read
                        RaiseEvent OnChangingToRead(Me, eventargs)
                    Case CRUDState.Create
                        RaiseEvent OnChangingToCreate(Me, eventargs)
                    Case CRUDState.Update
                        RaiseEvent OnChangingToUpdate(Me, eventargs)
                    Case CRUDState.Delete
                        RaiseEvent OnChangingToDelete(Me, eventargs)
                End Select

                If Not eventargs.AbortNewState Then
                    _state = value
                End If
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the dataobject.
        ''' </summary>
        ''' <value>The dataobject.</value>
        Public Property Dataobject() As iormRelationalPersistable
            Get
                Return _dataobject
            End Get
            Set(value As iormRelationalPersistable)
                _dataobject = value
                RaiseEvent OnChangingDataObject(Me, New MVDataObjectController.EventArgs(dataobject:=_dataobject, state:=Me.State))
            End Set
        End Property

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(Optional dataobject As iormRelationalPersistable = Nothing, Optional state As CRUDState = CRUDState.Read)
            _dataobject = dataobject
            _state = state
        End Sub
    End Class
    ''' <summary>
    ''' a controller in a MVC envirorment - iterating and holding a list of data objects
    ''' </summary>
    ''' <remarks>
    ''' functional design principles
    ''' 1. enable state operations e.g CRUD on a current data object
    ''' 2. hold a current data object out of a list of data object and able to change the current one and inform the event receivers
    ''' 3. Raise Events to inform Handlers about the state change, handlers might cancel the change
    ''' 4. raise event about changing to new data object
    ''' 5. load the list on demand
    ''' </remarks>
    Public Class MVDataObjectListController
        Inherits MVDataObjectController

        ''' <summary>
        ''' Controller Event Args
        ''' </summary>
        ''' <remarks></remarks>
        Public Class EventArgs
            Inherits MVDataObjectController.EventArgs


        End Class

        Private WithEvents _qryenumeration As iormQueriedEnumeration 'the list
        Private WithEvents _enumerator As ormQueriedEnumerator

        ''' events
        ''' 
        Public Event OnLoading As EventHandler(Of MVDataObjectListController.EventArgs)
        Public Event OnLoaded As EventHandler(Of MVDataObjectListController.EventArgs)
        Public Event OnAdding As EventHandler(Of MVDataObjectListController.EventArgs)
        Public Event OnRemoving As EventHandler(Of MVDataObjectListController.EventArgs)


#Region "Properties"
        ''' <summary>
        ''' Gets or sets the list - the queried enumeration object.
        ''' </summary>
        ''' <value>The list.</value>
        Public ReadOnly Property QueriedEnumeration() As iormQueriedEnumeration
            Get
                Return _qryenumeration
            End Get
        End Property
        ''' <summary>
        ''' Gets the enumerator as ormQueriedEnumerator
        ''' </summary>
        ''' <value>The enumerator.</value>
        Public ReadOnly Property Enumerator() As ormQueriedEnumerator
            Get
                Return _enumerator
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the list - the queried enumeration object.
        ''' </summary>
        ''' <value>The list.</value>
        Public ReadOnly Property IsLoaded() As Boolean
            Get
                If Me.QueriedEnumeration IsNot Nothing Then
                    Return Me.QueriedEnumeration.IsLoaded
                Else
                    Return False
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the current dataobject.
        ''' </summary>
        ''' <value>The dataobject.</value>
        Public Property Dataobject() As iormRelationalPersistable
            Get
                Return Me.Dataobject
            End Get
            Set(value As iormRelationalPersistable)
                MyBase.Dataobject = value
            End Set
        End Property

#End Region

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(enumeration As iormQueriedEnumeration)
            MyBase.New()
            _qryenumeration = enumeration
        End Sub
        ''' <summary>
        ''' load the qry enumeration
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Load() As Boolean
            If Me.QueriedEnumeration IsNot Nothing AndAlso Not Me.IsLoaded Then
                If Me.QueriedEnumeration.Load() Then
                    _enumerator = Me.QueriedEnumeration.GetEnumerator
                    Return True
                End If
            End If
            Return False
        End Function

        ''' <summary>
        ''' raise OnAdding Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub _qryenumeration_OnAdding(sender As Object, e As System.EventArgs) Handles _qryenumeration.OnAdded
            RaiseEvent OnAdding(Me, New MVDataObjectListController.EventArgs())
        End Sub

        ''' <summary>
        ''' raise OnLoading Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub _qryenumeration_OnLoading(sender As Object, e As System.EventArgs) Handles _qryenumeration.OnLoading
            RaiseEvent OnLoading(Me, New MVDataObjectListController.EventArgs())
        End Sub
        ''' <summary>
        ''' raise OnLoaded Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub _qryenumeration_OnLoaded(sender As Object, e As System.EventArgs) Handles _qryenumeration.OnLoaded
            RaiseEvent OnLoaded(Me, New MVDataObjectListController.EventArgs())
        End Sub

        ''' <summary>
        ''' RaiseOnRemoveEvent
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub _qryenumeration_OnRemoving(sender As Object, e As System.EventArgs) Handles _qryenumeration.OnRemoved
            RaiseEvent OnRemoving(Me, New MVDataObjectListController.EventArgs())
        End Sub
    End Class

End Namespace
