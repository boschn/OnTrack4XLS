REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** Data Object Behavior Declaration
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
Imports OnTrack
Imports OnTrack.Core
Imports OnTrack.rulez.eXPressionTree


Namespace OnTrack.Database

    ''' <summary>
    ''' describing a singleton DataObject Factory which retrieves or creates data objects of a certain type
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormDataObjectProvider
        Inherits iDataObjectProvider
        ''' <summary>
        ''' returns a new instance of a data object
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function NewOrmDataObject(type As Type) As iormDataObject
        ''' <summary>
        ''' sets or gets the ObjectRepository this Factory belongs to
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Repository As ormObjectRepository
        ''' <summary>
        ''' Events
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnCreating(sender As Object, e As ormDataObjectEventArgs)
        Event OnCreated(sender As Object, e As ormDataObjectEventArgs)
        Event OnRetrieving(sender As Object, e As ormDataObjectEventArgs)
        Event OnRetrieved(sender As Object, e As ormDataObjectEventArgs)
        Event OnOverloaded(sender As Object, e As ormDataObjectOverloadedEventArgs)

        ''' <summary>
        ''' create a persistable dataobject
        ''' </summary>
        ''' <param name="pkArray"></param>
        ''' <param name="checkUnique"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Create(primarykey As ormDatabaseKey, type As System.Type, _
                        Optional domainID As String = Nothing, _
                        Optional checkUnique As Boolean? = Nothing, _
                        Optional runTimeonly As Boolean? = Nothing) As iormDataObject


        ''' <summary>
        ''' create the dataobject as persistable object in the data store
        ''' </summary>
        ''' <param name="record"></param>
        ''' <param name="domainID"></param>
        ''' <param name="checkUnique"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Create(ByRef record As ormRecord, type As System.Type, _
                       Optional domainID As String = Nothing, _
                       Optional checkUnique As Boolean? = Nothing, _
                       Optional runtimeOnly? As Boolean = Nothing) As iormDataObject

        ''' <summary>
        ''' retrieves a data object from the persistence store
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="type"></param>
        ''' <param name="domainID"></param>
        ''' <param name="forceReload"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Retrieve(primarykey As ormDatabaseKey, type As System.Type, _
                                                            Optional domainID As String = Nothing, _
                                                            Optional forceReload As Boolean? = Nothing, _
                                                            Optional runtimeOnly As Boolean? = Nothing) As iormDataObject

        ''' <summary>
        ''' returns a inenumerable of all data objects of this type
        ''' </summary>
        ''' <param name="type"></param>
        ''' <param name="domainID"></param>
        ''' <param name="forceReload"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RetrieveAll(type As System.Type,
                              Optional key As ormDatabaseKey = Nothing, _
                              Optional domainID As String = Nothing, _
                              Optional deleted As Boolean = False, _
                              Optional forceReload As Boolean? = Nothing, _
                              Optional runtimeOnly As Boolean? = Nothing) As IEnumerable(Of iormDataObject)

        ''' <summary>
        ''' retrieve object by relation
        ''' </summary>
        ''' <param name="arelationAttribute"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RetrieveByRelation(arelationAttribute As ormRelationAttribute, sourceobject As iormDataObject) As List(Of iormDataObject)

        ''' <summary>
        ''' prepare aSelectionRule
        ''' </summary>
        ''' <param name="rule"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function PrepareSelection(rule As SelectionRule, ByRef resultCode As rulez.ICodeBit) As Boolean

    End Interface
    ''' <summary>
    ''' interface describes a queriable object class
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormQueriable

        ''' <summary>
        ''' returns a queried Enumeration by name
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetQuery(name As String) As iormQueriedEnumeration

    End Interface

    ''' <summary>
    ''' Interface for objects which are validatable by entry or total
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormValidatable

        ''' <summary>
        ''' raise an OnEntryValidating Event
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="msglog"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RaiseOnEntryValidatingEvent(entryname As String, msglog As BusinessObjectMessageLog) As otValidationResultType
        ''' <summary>
        ''' Raise an OnEntryValidated Event
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="msglog"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RaiseOnEntryValidatedEvent(entryname As String, msglog As BusinessObjectMessageLog) As otValidationResultType

        ''' <summary>
        ''' raise an OnValidated Event
        ''' </summary>
        ''' <param name="msglog"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RaiseOnValidatedEvent(msglog As BusinessObjectMessageLog) As otValidationResultType



        ''' <summary>
        ''' Event on Object Instance Level for Validation (before Validation)
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnValidating(sender As Object, e As ormDataObjectEventArgs)
        ''' <summary>
        ''' Event on Object Instance Level for Validation (after Validation)
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnValidated(sender As Object, e As ormDataObjectEventArgs)

        ''' <summary>
        ''' Triggered if an Entry of a object was validated
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnEntryValidated(sender As Object, e As ormDataObjectEntryEventArgs)

        ''' <summary>
        ''' triggered if an entry of a object needs validation
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnEntryValidating(sender As Object, e As ormDataObjectEntryEventArgs)

        ''' <summary>
        ''' validates the Business Object as total
        ''' </summary>
        ''' <returns>True if validated and OK</returns>
        ''' <remarks></remarks>
        Function Validate(Optional msglog As BusinessObjectMessageLog = Nothing) As otValidationResultType

        ''' <summary>
        ''' validates a named object entry of the object
        ''' </summary>
        ''' <param name="enryname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Validate(entryname As String, value As Object, Optional msglog As BusinessObjectMessageLog = Nothing) As otValidationResultType

        ''' <summary>
        ''' raise the validating event and returns the result 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RaiseOnValidatingEvent(msglog As BusinessObjectMessageLog) As otValidationResultType


    End Interface

    ''' <summary>
    ''' describes an abstract data object
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormDataObject
        Inherits iDataObject

        ''' <summary>
        ''' get the persistable object definition
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectDefinition As iormObjectDefinition

        ''' <summary>
        ''' returns the object class description of the data object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectClassDescription As OnTrack.ObjectClassDescription

        ''' <summary>
        ''' gets the primary Key
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectPrimaryKey As ormDatabaseKey

        ''' <summary>
        ''' gets or sets the Domain ID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property DomainID As String

        ''' <summary>
        ''' gets the primary Container ID of the data object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectPrimaryContainerID As String

        ''' <summary>
        ''' returns the primary database driver of the data object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectPrimaryDatabaseDriver As iormDatabaseDriver
        ''' <summary>
        ''' returns the primary container store for the data object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectPrimaryContainerStore As iormContainerStore
        ''' <summary>
        ''' returns the stack of database drivers for for a data object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectDatabaseDrivers As Stack(Of iormDatabaseDriver)

        ''' <summary>
        ''' returns an Array of Container IDs
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectContainerIDs As String()

        ''' <summary>
        ''' check on the live status - if created or loaded / infused
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function DetermineLifeStatus() As Boolean


        ''' <summary>
        ''' returns the values of the primary keys
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectPrimaryKeyValues As Object()

        ''' <summary>
        ''' returns true if the object instanced is cached
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectUsesCache As Boolean

        ''' <summary>
        ''' return true if the object has the domain behavior
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectHasDomainBehavior As Boolean

        ''' <summary>
        ''' returns true if the object has the delete per flag behavior
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectHasDeletePerFlagBehavior As Boolean

        ''' <summary>
        ''' returns True if the persistable is only a runtime object and not persistable before not switched to runtimeOff
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property RuntimeOnly As Boolean

        ''' <summary>
        ''' Initialize the data object
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Initialize(Optional RuntimeOnly As Boolean = False) As Boolean

        ''' <summary>
        ''' returns the version by attribute of the persistance objects
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="dataobject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetObjectClassVersion(dataobject As iormDataObject, Optional name As String = Nothing) As Long

        ''' <summary>
        ''' retruns true or throws error if the dataobject is alive (created, retrieved, infused)
        ''' </summary>
        ''' <param name="subname"></param>
        ''' <param name="throwError"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function IsAlive(Optional subname As String = Nothing, Optional throwError As Boolean = True) As Boolean

        ''' <summary>
        ''' gets or sets the record of the data object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Record As ormRecord

        ''' <summary>
        ''' True if data object is initialized and working
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property IsInitialized As Boolean

    End Interface

    ''' <summary>
    ''' describes a general persistable data object
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormPersistable
        Inherits iormInfusable

        Event OnValidationNeeded(persistableDataObject As iormPersistable, validationEventArgs As ormDataObjectValidationEventArgs)

        Function UnDelete() As Boolean

        ''' <summary>
        ''' Events
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnInjecting(sender As Object, e As ormDataObjectEventArgs)
        Event OnInjected(sender As Object, e As ormDataObjectEventArgs)

        Event OnPersisting(sender As Object, e As ormDataObjectEventArgs)
        Event OnPersisted(sender As Object, e As ormDataObjectEventArgs)
        Event OnUnDeleting(sender As Object, e As ormDataObjectEventArgs)
        Event OnUnDeleted(sender As Object, e As ormDataObjectEventArgs)
        Event OnDeleting(sender As Object, e As ormDataObjectEventArgs)
        Event OnDeleted(sender As Object, e As ormDataObjectEventArgs)
        Event OnCreating(sender As Object, e As ormDataObjectEventArgs)
        Event OnCreated(sender As Object, e As ormDataObjectEventArgs)
        ''' <summary>
        ''' triggered if the create operation needs default values
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnCreateDefaultValuesNeeded(sender As Object, e As ormDataObjectEventArgs)

        ''' <summary>
        ''' create a persistable dataobject
        ''' </summary>
        ''' <param name="pkArray"></param>
        ''' <param name="checkUnique"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Create(primarykey As ormDatabaseKey, _
                        Optional domainID As String = Nothing, _
                        Optional checkUnique As Boolean? = Nothing, _
                        Optional runTimeonly As Boolean? = Nothing) As Boolean


        ''' <summary>
        ''' create the dataobject as persistable object in the data store
        ''' </summary>
        ''' <param name="record"></param>
        ''' <param name="domainID"></param>
        ''' <param name="checkUnique"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Create(ByRef record As ormRecord, _
                       Optional domainID As String = Nothing, _
                       Optional checkUnique As Boolean? = Nothing, _
                       Optional runtimeOnly? As Boolean = Nothing) As Boolean

        ''' <summary>
        ''' Perists the object in the datastore
        ''' </summary>
        ''' <param name="timestamp"></param>
        ''' <param name="doFeedRecord"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Persist(Optional timestamp As DateTime? = Nothing, Optional doFeedRecord As Boolean = True) As Boolean

        ''' <summary>
        ''' deletes a persistable object in the datastore
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Delete(Optional timestamp As DateTime? = Nothing) As Boolean

        ''' <summary>
        ''' load and infuse the dataobject by primary key
        ''' </summary>
        ''' <param name="pkArray"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Inject(ByRef key As ormDatabaseKey, _
                        Optional domainid As String = Nothing, _
                         Optional dbdriver As iormDatabaseDriver = Nothing, _
                        Optional loadDeleted As Boolean = False) As Boolean
    End Interface
    ''' <summary>
    ''' interface describes a persistable relational data object
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormRelationalPersistable
        Inherits iormPersistable

        ''' <summary>
        ''' returns the table ids
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectTableIDs As String()

        ''' <summary>
        ''' returns the Objects database Driver
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectPrimaryRelationalDatabaseDriver As iormRelationalDatabaseDriver
        ''' <summary>
        ''' Tablestore associated with this data object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectPrimaryTableStore As iormRelationalTableStore

        ''' <summary>
        ''' TableID associated with this data object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectPrimaryTableID As String


    End Interface

    ''' <summary>
    ''' interface infusable if an Object can be infused by a record
    ''' </summary>
    ''' <remarks></remarks>

    Public Interface iormInfusable
        Inherits iormDataObject

        ''' <summary>
        ''' return true if entry of the value of the entryname and the value are the same
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function EqualsValue(entryname As String, value As Object) As Boolean

        ''' <summary>
        ''' returns true if the data object is infused
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property IsInfused As Boolean

        ''' <summary>
        ''' triggers if a column is infused
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnColumnsInfused(sender As Object, e As ormDataObjectEventArgs)

        ''' <summary>
        ''' normalize the value to the standards
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function NormalizeValue(entryname As String, ByRef value As Object) As Boolean

        ''' <summary>
        ''' triggered if a default value is needed
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnDefaultValueNeeded(sender As Object, e As ormDataObjectEntryEventArgs)

        ''' <summary>
        ''' returns the entries' default value
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ObjectEntryDefaultValue(entryname As String) As Object

        ''' <summary>
        ''' requests a relation by id to be infused, force if it was loaded and infused before
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function InfuseRelation(id As String, Optional force As Boolean = False) As Boolean

        ''' <summary>
        ''' OnInfusing event triggers before infusing a data object
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnInfusing(sender As Object, e As ormDataObjectEventArgs)
        ''' <summary>
        ''' OnInfused event triggers after infusing a data object
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnInfused(sender As Object, e As ormDataObjectEventArgs)



        ''' <summary>
        ''' raised when about to feed
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnFeeding(sender As Object, e As ormDataObjectEventArgs)

        ''' <summary>
        ''' raised when fed
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnFed(sender As Object, e As ormDataObjectEventArgs)

        ''' <summary>
        ''' Infuse the object with data from the record
        ''' </summary>
        ''' <param name="record">record </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Infuse(ByRef record As ormRecord, Optional mode? As otInfuseMode = Nothing) As Boolean

        ''' <summary>
        ''' flushs the records persistable values out to a record
        ''' </summary>
        ''' <param name="record"></param>
        ''' <param name="domainID"></param>
        ''' <param name="checkUnique"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Feed(Optional record As ormRecord = Nothing) As Boolean

        ''' <summary>
        ''' raise the validation needed event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event EntryValidationNeeded As EventHandler(Of ormDataObjectEntryValidationEventArgs)

        ''' <summary>
        ''' Changing an Entry Value
        ''' </summary>
        ''' <remarks></remarks>
        Event OnEntryChanging As EventHandler(Of ormDataObjectEntryEventArgs)
        Event OnEntryChanged As EventHandler(Of ormDataObjectEntryEventArgs)
    End Interface
    ''' <summary>
    ''' interface cloneable if an object can be cloned
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks></remarks>

    Public Interface iormCloneable(Of T As {iormRelationalPersistable, iormInfusable, New})
        ''' <summary>
        ''' clone the object with the new primary key
        ''' </summary>
        ''' <param name="pkarray">primary key array</param>
        ''' <returns>the new cloned object or nothing</returns>
        ''' <remarks></remarks>
        Function Clone(pkarray() As Object, Optional runtimeOnly As Boolean? = Nothing) As T
    End Interface
    ''' <summary>
    ''' interface cloneable if an object can be cloned
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks></remarks>

    Public Interface iormCloneable

        Function Clone(newpkarray As Object(), Optional runtimeOnly As Boolean? = Nothing) As Object

        ''' <summary>
        ''' OnCloning Event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Event OnCloning(sender As Object, e As ormDataObjectCloneEventArgs)
        Event OnCloned(sender As Object, e As ormDataObjectCloneEventArgs)

    End Interface


    ''' <summary>
    ''' interface for having an Compound 
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iotHasCompounds
        ''' <summary>
        ''' adds compounds slots of an instance (out of the envelope) to the envelope
        ''' </summary>
        ''' <param name="envelope"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function AddSlotCompounds(ByRef envelope As XChange.XEnvelope) As Boolean

    End Interface




    ''' <summary>
    ''' Interface for objects which are loggable - have a object message log
    ''' </summary>
    ''' <remarks></remarks>

    Public Interface iormLoggable


        ''' <summary>
        ''' sets or gets the context identifier for the message in the context
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property ContextIdentifier As String
        ''' <summary>
        ''' sets or gets the tuple identifier for the message in the context
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property TupleIdentifier As String
        ''' <summary>
        ''' sets or gets the entity identifier for the message in the context
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property EntityIdentifier As String

        ''' <summary>
        ''' returns the ObjectMessageLog
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property BusinessObjectMessageLog As BusinessObjectMessageLog

    End Interface
End Namespace