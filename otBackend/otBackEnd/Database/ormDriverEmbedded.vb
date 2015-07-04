REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** Embbeded Database Driver Declaration
REM *********** 
REM *********** Version: 2.00
REM *********** Created: 2015-02-23
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
Imports System.Reflection
Imports OnTrack.Core
Imports OnTrack.Rulez.eXPressionTree
Imports OnTrack.Rulez


Namespace OnTrack.Database

    ''' <summary>
    ''' Serialiaziations types
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otSerializeFormat
        XML
    End Enum
    ''' <summary>
    ''' Attribute Class for Defining an Embedded Container
    ''' <otSchemaTable(Version:=1)>Const constContainer = "ID"
    ''' Version will be saved into clsOTDBDEfSchemaTable
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Field, AllowMultiple:=False, Inherited:=True)> _
    Public Class ormEmbeddedContainerAttribute
        Inherits ormContainerAttribute
        Implements iormContainerAttribute

        Protected _embeddedInReferenceObjectEntry As String = Nothing ' needed for resolving 
        Protected _serializeAs As otSerializeFormat = 0

        ''' <summary>
        '''  constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
        End Sub
        ''' <summary>
        ''' Gets or sets the container type
        ''' </summary>
        ''' <value>The is active.</value>
        Public Overrides Property Containertype As otContainerType Implements iormContainerAttribute.ContainerType
            Get
                Return otContainerType.EmbeddedObject
            End Get
            Set(value As otContainerType)
                Throw New NotSupportedException(" do not set a container type here")
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the reference object entry which serves as container for this object. Has the form [objectname].[entryname] 
        ''' such as Deliverable.constObjectID & "." & deliverable.constFNUID
        ''' </summary>
        ''' <value>The reference object entry.</value>
        Public Property EmbeddedIn As String
            Get
                Return Me._embeddedInReferenceObjectEntry
            End Get
            Set(value As String)
                Me._embeddedInReferenceObjectEntry = UCase(value)
            End Set
        End Property
        Public ReadOnly Property HasValueEmbeddedIn As Boolean
            Get
                Return Not String.IsNullOrWhiteSpace(_embeddedInReferenceObjectEntry)
            End Get
        End Property
        ''' <summary>
        ''' sepcifies the format to serialize in
        ''' </summary>
        ''' <value>The reference object entry.</value>
        Public Property SerializeAs As otSerializeFormat
            Get
                Return Me._serializeAs
            End Get
            Set(value As otSerializeFormat)
                Me._serializeAs = value
            End Set
        End Property
        Public ReadOnly Property HasValueSerializeAs As Boolean
            Get
                Return _serializeAs <> 0
            End Get
        End Property
        ''' <summary>
        ''' returns the reference entry name 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ReferenceEntryName As String
            Get
                If Not Me.HasValueEmbeddedIn Then Return Nothing
                Dim names = Shuffle.NameSplitter(Me.EmbeddedIn)
                If names.Count > 1 Then
                    Return names(1)
                Else
                    CoreMessageHandler(message:="objectname is missing in reference " & Me.EmbeddedIn, procedure:="ormEmbeddedContainerAttribute.ReferenceEntryName", _
                                       messagetype:=otCoreMessageType.InternalError, argument:=Me.EmbeddedIn)
                    Return Nothing
                End If
            End Get
        End Property
        ''' <summary>
        ''' returns the reference object name 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ReferenceObjectName As String
            Get
                If Not Me.HasValueEmbeddedIn Then Return Nothing
                Dim names = Shuffle.NameSplitter(Me.EmbeddedIn)
                If names.Count > 1 Then
                    Return names(0)
                Else
                    CoreMessageHandler(message:="objectname is missing in reference " & Me.EmbeddedIn, procedure:="ormEmbeddedContainerAttribute.ReferenceEntryName", _
                                       messagetype:=otCoreMessageType.InternalError, argument:=Me.EmbeddedIn)
                    Return Nothing
                End If
            End Get
        End Property
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    <ormDatabaseDriver(Autoinstance:=True, Name:=ConstCPVDriverEmbeddedName, defaultid:=ConstCPVDriverEmbeddedName, description:="Driver for embedded objects")> _
    Public Class EmbeddedDriver
        Implements iormDatabaseDriver

        Private _session As Session
        Private _id As String = ConstCPVDriverEmbeddedName  'instance id -> default value -> must be same as driver name for the attribute setting to be found
        Private _connection As iormConnection
        Private _ContainerSchemaDirectory As New Dictionary(Of String, EmbeddedContainerSchema)
        Private _ContainerStoreDirectory As New Dictionary(Of String, EmbeddedContainerStore)
        Private _lockObject As Object


#Region "Properties"


        ''' <summary>
        ''' gets the parameter container name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DBParameterContainerName As String Implements iormDatabaseDriver.DBParameterContainerName
            Get
                Return ConstCPVDriverEmbeddedName & "_ParameterContainer"
            End Get
        End Property
        ''' <summary>
        ''' gets or sets the session
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <value></value>
        Public Property Session() As Session Implements iormDatabaseDriver.Session
            Get
                Return _session
            End Get
            Set(value As Session)
                _session = value
            End Set
        End Property

        ''' <summary>
        ''' gets the current connection
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public ReadOnly Property CurrentConnection As iormConnection Implements iormDatabaseDriver.CurrentConnection
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' sets or gets the instance id
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ID As String Implements iormDatabaseDriver.ID
            Get
                Return _id
            End Get
            Set(value As String)
                _id = value
            End Set
        End Property

        ''' <summary>
        ''' returns true if the driver is an OnTrack Driver
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsOntrackDriver As Boolean Implements iormDatabaseDriver.IsOnTrackDriver
            Get
                Return False
            End Get
        End Property
        ''' <summary>
        ''' returns true if the driver is an relational driver
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsRelationalDriver As Boolean Implements iormDatabaseDriver.IsRelationalDriver
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' gets or sets the container version for an container id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ContainerVersion(id As String) As Long? Implements iormDatabaseDriver.ContainerVersion
            Get
                Dim aVersion As Object = GetDBParameter(ConstPNBSchemaVersion_ContainerHeader & id, silent:=True)
                Return CULng(aVersion)
            End Get
            Set(value As Long?)
                SetDBParameter(ConstPNBSchemaVersion_ContainerHeader & id, value:=value.ToString, silent:=True)
            End Set
        End Property
#End Region

        ''' <summary>
        ''' Gets the DB parameter.
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateDBParameterContainer(Optional ByRef nativeConnection As Object = Nothing) As Boolean Implements iormDatabaseDriver.CreateDBParameterContainer

            Return True
        End Function

        ''' <summary>
        ''' drops the DB parameter table - given with setup then just the setup related entries
        ''' if then there is no setup related entries at all -> drop the full table
        ''' </summary>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DropDBParameterTable(Optional setupid As String = Nothing, Optional ByRef nativeConnection As Object = Nothing) As Boolean Implements iormDatabaseDriver.DropDBParameterContainer

            Return True
        End Function

        ''' <summary>
        ''' drops the container version
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DropContainerVersion(id As String) As Boolean Implements iormDatabaseDriver.DropContainerVersion
            Return True
        End Function
        ''' <summary>
        ''' convert object value to DB data 
        ''' </summary>
        ''' <param name="invalue"></param>
        ''' <param name="outvalue"></param>
        ''' <param name="targetType"></param>
        ''' <param name="maxsize"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <param name="columnname"></param>
        ''' <param name="isnullable"></param>
        ''' <param name="defaultvalue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Convert2DBData(invalue As Object, ByRef outvalue As Object, targetType As Long, Optional maxsize As Long = 0, Optional ByRef abostrophNecessary As Boolean = False, Optional columnname As String = Nothing, Optional isnullable As Boolean = False, Optional defaultvalue As Object = Nothing) As Boolean Implements iormDatabaseDriver.Convert2DBData
            outvalue = invalue
            Return True
        End Function
        ''' <summary>
        ''' convert DB Data value to Object value
        ''' </summary>
        ''' <param name="invalue"></param>
        ''' <param name="outvalue"></param>
        ''' <param name="sourceType"></param>
        ''' <param name="isnullable"></param>
        ''' <param name="defaultvalue"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Convert2ObjectData(invalue As Object, ByRef outvalue As Object, sourceType As Long, Optional isnullable As Boolean? = Nothing, Optional defaultvalue As Object = Nothing, Optional ByRef abostrophNecessary As Boolean = False) As Boolean Implements iormDatabaseDriver.Convert2ObjectData
            outvalue = invalue
            Return True
        End Function

        ''' <summary>
        ''' returns a persist-able parameter value
        ''' </summary>
        ''' <param name="parametername"></param>
        ''' <param name="nativeConnection"></param>
        ''' <param name="silent"></param>
        ''' <param name="setupID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDBParameter(parametername As String, Optional ByRef nativeConnection As Object = Nothing, Optional silent As Boolean = False, Optional setupID As String = Nothing) As Object Implements iormDatabaseDriver.GetDBParameter
            Return CurrentOTDBDriver.GetDBParameter(Me.DBParameterContainerName & "_" & parametername, silent:=silent, setupID:=setupID)
        End Function
        ''' <summary>
        ''' deletes a persist-able parameter
        ''' </summary>
        ''' <param name="parametername"></param>
        ''' <param name="nativeConnection"></param>
        ''' <param name="silent"></param>
        ''' <param name="setupID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteDBParameter(parametername As String, Optional ByRef nativeConnection As Object = Nothing, Optional silent As Boolean = False, Optional setupID As String = Nothing) As Boolean Implements iormDatabaseDriver.DeleteDBParameter
            Return ot.CurrentOTDBDriver.DeleteDBParameter(Me.DBParameterContainerName & "_" & parametername, silent:=silent, setupID:=setupID)
        End Function
        ''' <summary>
        ''' sets a persist-able database parameter
        ''' </summary>
        ''' <param name="parametername"></param>
        ''' <param name="value"></param>
        ''' <param name="nativeConnection"></param>
        ''' <param name="updateOnly"></param>
        ''' <param name="silent"></param>
        ''' <param name="setupID"></param>
        ''' <param name="description"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetDBParameter(parametername As String, value As Object, Optional ByRef nativeConnection As Object = Nothing, Optional updateOnly As Boolean = False, Optional silent As Boolean = False, Optional setupID As String = Nothing, Optional description As String = Nothing) As Boolean Implements iormDatabaseDriver.SetDBParameter
            Return CurrentOTDBDriver.SetDBParameter(Me.DBParameterContainerName & "_" & parametername, value:=value, silent:=silent, setupID:=setupID)
        End Function

        ''' <summary>
        ''' returns true if the container id exists in the database
        ''' </summary>
        ''' <param name="containerID"></param>
        ''' <param name="connection"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasContainerID(containerID As String, Optional ByRef connection As iormConnection = Nothing, Optional nativeConnection As Object = Nothing) As Boolean Implements iormDatabaseDriver.HasContainerID
            Return ot.GetContainerAttribute(containerid:=containerID) IsNot Nothing
        End Function
        ''' <summary>
        ''' returns (and creates or changes) a native container object
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="createOrAlter"></param>
        ''' <param name="connection"></param>
        ''' <param name="nativeContainerObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetContainerObject(id As String, _
                                           Optional createOrAlter As Boolean = False, _
                                           Optional ByRef connection As iormConnection = Nothing, _
                                           Optional ByRef nativeContainerObject As Object = Nothing) As Object Implements iormDatabaseDriver.GetContainerObject
            Try
                Dim anAttribute As ormEmbeddedContainerAttribute = ot.GetContainerAttribute(containerid:=id)

                If anAttribute.HasValueEmbeddedIn Then
                    Dim embeddedAttribute As ormObjectEntryAttribute = ot.GetObjectEntryAttribute(entryname:=anAttribute.ReferenceEntryName, objectname:=anAttribute.ReferenceObjectName)
                    If embeddedAttribute Is Nothing Then
                        CoreMessageHandler(message:="Embedding object entry attribute was not found", argument:=anAttribute.EmbeddedIn, procedure:="EmbeddedDBDriver.GetContainerObject", messagetype:=otCoreMessageType.InternalError)
                        Return Nothing
                    End If
                    Return embeddedAttribute
                Else
                    CoreMessageHandler(message:="Embedding object entry is not set in attribute", argument:=anAttribute.ID, procedure:="EmbeddedDBDriver.GetContainerObject", messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="EmbeddedDBDriver.GetContainerObject", argument:=id)
                Return Nothing
            End Try


        End Function
        ''' <summary>
        ''' drops the container object from the data base
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DropContainerObject(id As String, Optional ByRef connection As iormConnection = Nothing) As Boolean Implements iormDatabaseDriver.DropContainerObject
            Return False
        End Function

        ''' <summary>
        ''' returns true if the containerID and the entry ID exists in the database
        ''' </summary>
        ''' <param name="containerID"></param>
        ''' <param name="entryID"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasContainerEntryID(containerID As String, entryID As String, Optional ByRef connection As iormConnection = Nothing) As Boolean Implements iormDatabaseDriver.HasContainerEntryID
            Dim anContainerAttribute As ormEmbeddedContainerAttribute = TryCast(Me.GetContainerObject(id:=containerID), ormEmbeddedContainerAttribute)
            If anContainerAttribute IsNot Nothing Then
                Return anContainerAttribute.EntryNames.Contains(entryID.ToUpper)
            End If
            Return False
        End Function
        ''' <summary>
        ''' returns (and creates or changes) a native container entry object
        ''' </summary>
        ''' <param name="nativeContainerObject"></param>
        ''' <param name="containerEntryDefinition"></param>
        ''' <param name="createOrAlter"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetContainerEntryObject(nativeContainerObject As Object, containerEntryDefinition As iormContainerEntryDefinition, Optional createOrAlter As Boolean = False, Optional ByRef connection As iormConnection = Nothing) As Object Implements iormDatabaseDriver.GetContainerEntryObject
            If nativeContainerObject Is Nothing Then
                ot.CoreMessageHandler(message:="native container Object is nothing", messagetype:=otCoreMessageType.InternalError, _
                                     procedure:="EmbeddedDBDriver.GetContainerEntryObject")
                Return Nothing
            ElseIf nativeContainerObject.GetType = GetType(ormEmbeddedContainerAttribute) Then
                Dim anAttribute As ormEmbeddedContainerAttribute = TryCast(nativeContainerObject, ormEmbeddedContainerAttribute)
                Return anAttribute.Entries.Where(Function(x) x.EntryName.ToUpper = containerEntryDefinition.Entryname.ToUpper).ToList

            Else
                ot.CoreMessageHandler(message:="native container Object is not of type ormEmbeddedContainerAttribute", messagetype:=otCoreMessageType.InternalError, _
                                       procedure:="EmbeddedDBDriver.GetContainerEntryObject", argument:=nativeContainerObject.GetType.Name)
                Return Nothing
            End If
        End Function


        ''' <summary>
        ''' returns (and creates or changes) a native foreign key object
        ''' </summary>
        ''' <param name="nativeContainerObject"></param>
        ''' <param name="foreignkeydefinition"></param>
        ''' <param name="createOrAlter"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetForeignKeys(nativeContainerObject As Object, foreignkeydefinition As ormForeignKeyDefinition, Optional createOrAlter As Boolean = False, Optional ByRef connection As iormConnection = Nothing) As IEnumerable(Of Object) Implements iormDatabaseDriver.GetForeignKeys
            Return Nothing
        End Function

        ''' <summary>
        ''' return (and creates or changes) a native index object
        ''' </summary>
        ''' <param name="nativeTable"></param>
        ''' <param name="indexdefinition"></param>
        ''' <param name="forceCreation"></param>
        ''' <param name="createOrAlter"></param>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIndex(ByRef nativeTable As Object, ByRef indexdefinition As ormIndexDefinition, Optional forceCreation As Boolean = False, Optional createOrAlter As Boolean = False, Optional ByRef connection As iormConnection = Nothing) As Object Implements iormDatabaseDriver.GetIndex
            Return Nothing
        End Function

        ''' <summary>
        ''' returns the native database object name
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNativeDBObjectName(id As String) As String Implements iormDatabaseDriver.GetNativeDBObjectName
            Return id
        End Function

        ''' <summary>
        ''' returns the native foreign key name
        ''' </summary>
        ''' <param name="foreignkeyid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNativeForeignKeyName(foreignkeyid As String) As String Implements iormDatabaseDriver.GetNativeForeignKeyName
            Return Nothing
        End Function

        ''' <summary>
        ''' returns the native index name
        ''' </summary>
        ''' <param name="indexid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNativeIndexName(indexid As String) As String Implements iormDatabaseDriver.GetNativeIndexName
            Return Nothing
        End Function

        ''' <summary>
        ''' returns the native target type (as enumeration) for a Ontrack Type
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTargetTypeFor(type As otDataType) As Long Implements iormDatabaseDriver.GetTargetTypeFor
            Return type
        End Function


        ''' <summary>
        ''' returns true if the registration of the connection to the native database was successful
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RegisterConnection(ByRef connection As iormConnection) As Boolean Implements iormDatabaseDriver.RegisterConnection
            _connection = connection
            Return True
        End Function

        ''' <summary>
        ''' returns a container schema object for the container id
        ''' </summary>
        ''' <param name="containerID"></param>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RetrieveContainerSchema(containerID As String, Optional force As Boolean = False) As iormContainerSchema Implements iormDatabaseDriver.RetrieveContainerSchema

            'take existing or make new one
            If _ContainerSchemaDirectory.ContainsKey(containerID.ToUpper) And Not force Then
                Return _ContainerSchemaDirectory.Item(containerID.ToUpper)
            Else
                Dim aNewSchema As iormContainerSchema

                ' delete the existing object
                If _ContainerSchemaDirectory.ContainsKey(containerID.ToUpper) Then
                    aNewSchema = _ContainerSchemaDirectory.Item(containerID.ToUpper)
                    SyncLock aNewSchema
                        If force Or Not aNewSchema.IsInitialized Then aNewSchema.Refresh(force)
                    End SyncLock
                    Return aNewSchema
                End If
                ' assign the Table
                aNewSchema = New EmbeddedContainerSchema(containerID.ToUpper)

                If Not aNewSchema Is Nothing Then
                    SyncLock _lockObject
                        _ContainerSchemaDirectory.Add(key:=containerID.ToUpper, value:=aNewSchema)
                    End SyncLock

                    If Not aNewSchema.IsInitialized Then
                        SyncLock aNewSchema
                            aNewSchema.Refresh(reloadForce:=force)
                        End SyncLock
                    End If
                End If

                ' return
                Return aNewSchema
            End If

        End Function
        ''' <summary>
        ''' returns the container store object for the container id
        ''' </summary>
        ''' <param name="containerid"></param>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RetrieveContainerStore(containerid As String, Optional force As Boolean = False) As iormContainerStore Implements iormDatabaseDriver.RetrieveContainerStore
            'take existing or make new one
            If _ContainerStoreDirectory.ContainsKey(containerid.ToUpper) And Not force Then
                Return _ContainerStoreDirectory.Item(containerid.ToUpper)
            Else
                Dim aNewStore As iormContainerStore

                ' reload the existing object on force
                If _ContainerStoreDirectory.ContainsKey(containerid.ToUpper) Then
                    aNewStore = _ContainerStoreDirectory.Item(containerid.ToUpper)
                    aNewStore.Refresh(force)
                    Return aNewStore
                End If
                ' assign the Table

                aNewStore = New EmbeddedContainerStore(containerid.ToUpper, forceSchemaReload:=force)
                If Not aNewStore Is Nothing Then
                    If Not _ContainerStoreDirectory.ContainsKey(containerid.ToUpper) Then
                        _ContainerStoreDirectory.Add(key:=containerid.ToUpper, value:=aNewStore)
                    End If
                End If
                ' return
                Return aNewStore

            End If
        End Function

        '' <summary>
        ''' Gets the native database name 
        ''' </summary>
        ''' <value>The type.</value>
        Public ReadOnly Property NativeDatabaseName As String Implements iormDatabaseDriver.NativeDatabaseName
            Get
                Return ConstCPVDriverEmbeddedName
            End Get
        End Property

        '' <summary>
        ''' Gets the native database version 
        ''' </summary>
        ''' <value>The type.</value>
        Public ReadOnly Property NativeDatabaseVersion As String Implements iormDatabaseDriver.NativeDatabaseVersion
            Get
                Return ot.GetObjectClassDescription(GetType(EmbeddedDriver)).ObjectAttribute.Version
            End Get
        End Property

        ''' <summary>
        ''' returns the name of the database driver
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Name As String Implements iormDatabaseDriver.Name
            Get
                Return ConstCPVDriverEmbeddedName
            End Get
        End Property


        ''' <summary>
        ''' returns true if the container entry schema of the database is according to the attribute description
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="connection"></param>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function VerifyContainerEntrySchema(definition As iormContainerEntryDefinition, Optional ByRef connection As iormConnection = Nothing, Optional silent As Boolean = False) As Boolean Implements iormDatabaseDriver.VerifyContainerEntrySchema
            Return True
        End Function
        ''' <summary>
        ''' returns true if the container schema in the database is according to the container attribute
        ''' </summary>
        ''' <param name="containerAttribute"></param>
        ''' <param name="connection"></param>
        ''' <param name="nativeConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function VerifyContainerSchema(containerDefinition As iormContainerDefinition, Optional ByRef connection As iormConnection = Nothing, Optional nativeConnection As Object = Nothing) As Boolean Implements iormDatabaseDriver.VerifyContainerSchema
            If containerDefinition.GetType Is GetType(ormEmbeddedContainerAttribute) Then
                If Me.GetContainerObject(containerDefinition.ContainerID) IsNot Nothing Then
                    Return True 'true 
                Else
                    CoreMessageHandler(message:="container attribute is not found", argument:=containerDefinition.ContainerID, _
                                  messagetype:=otCoreMessageType.InternalError, procedure:="EmbeddedDBDriver.VerifyContainerSchema")
                    Return True
                End If
            Else
                CoreMessageHandler(message:="container attribute is not of embedded container attribute", argument:=containerDefinition.GetType.Name, _
                                    messagetype:=otCoreMessageType.InternalError, procedure:="EmbeddedDBDriver.VerifyContainerSchema")
                Return True '
            End If
        End Function

        ''' <summary>
        ''' returns a new visitor object for building expression trees
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIXPTVisitor() As rulez.eXPressionTree.IVisitor Implements iormDatabaseDriver.GetIXPTVisitor
            Return New rulez.eXPressionTree.Visitor(Of String)
        End Function

        ''' <summary>
        ''' run a selection rule and return the result as ormRecords
        ''' </summary>
        ''' <param name="selectionrule"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RunSelectionRule(selectionrule As OnTrack.rulez.eXPressionTree.SelectionRule, context As OnTrack.rulez.Context) As IList(Of ormRecord) Implements iormDatabaseDriver.RetrieveBy
            Throw New NotImplementedException()
        End Function
        ''' <summary>
        ''' prepares a selection rule
        ''' </summary>
        ''' <param name="rule"></param>
        ''' <param name="resultCode"></param>
        ''' <remarks></remarks>
        ''' <returns></returns>
        Public Function PrepareSelection(rule As SelectionRule, ByRef resultCode As ICodeBit) As Boolean Implements iormDatabaseDriver.PrepareSelection
            ' TODO: Implement this method
            Throw New NotImplementedException()
        End Function
    End Class

    ''' <summary>
    ''' describes a container store for embedded objects
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EmbeddedContainerStore
        Implements iormContainerStore

        Private _id As String = "EMBEDDEDDRIVER"
        Private _schema As EmbeddedContainerSchema
        Private _connection As iormConnection

        Private _properties As Dictionary(Of String, Object) 'properties of the store
        Private _hostDataObject As iormDataObject
        Private _cacheTable As New Dictionary(Of ormDatabaseKey, ormRecord)
        Private _isInitialized As Boolean = False

#Region "Properties"
        ''' <summary>
        ''' returns true if linq is available
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsLinqAvailable As Boolean Implements iormContainerStore.IsLinqAvailable
            Get
                Return False
            End Get
        End Property
        ''' <summary>
        ''' returns true if initialized
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsInitialized As Boolean
            Get
                Return _isInitialized
            End Get
            Private Set(value As Boolean)
                _isInitialized = value
            End Set
        End Property
        ''' <summary>
        ''' gets or sets the connection of the store
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Connection As iormConnection Implements iormContainerStore.Connection
            Get
                Return _connection
            End Get
            Set(value As iormConnection)
                _connection = value
            End Set
        End Property

        ''' <summary>
        ''' gets or sets the containerID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ContainerID As String Implements iormContainerStore.ContainerID
            Get
                Return _id
            End Get
            Set(value As String)
                _id = value
            End Set
        End Property

        ''' <summary>
        ''' gets or sets the schema of the container
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ContainerSchema As iormContainerSchema Implements iormContainerStore.ContainerSchema
            Get
                Return _schema
            End Get
            Set(value As iormContainerSchema)
                _schema = value
            End Set
        End Property
        Public ReadOnly Property NativeDBObjectname As String Implements iormContainerStore.NativeDBObjectname
            Get
                Return _id
            End Get
        End Property
#End Region

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="containerid"></param>
        ''' <param name="forceSchemaReload"></param>
        ''' <remarks></remarks>

        Public Sub New(containerid As String, Optional forceSchemaReload As Boolean = False)
            _id = containerid
            If _schema IsNot Nothing AndAlso forceSchemaReload Then _schema.Refresh()
        End Sub
        ''' <summary>
        ''' converts data to container data representation
        ''' </summary>
        ''' <param name="invalue"></param>
        ''' <param name="outvalue"></param>
        ''' <param name="targetType"></param>
        ''' <param name="maxsize"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <param name="columnname"></param>
        ''' <param name="isnullable"></param>
        ''' <param name="defaultvalue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Convert2ContainerData(invalue As Object, ByRef outvalue As Object, targetType As Long, Optional maxsize As Long = 0, Optional ByRef abostrophNecessary As Boolean = False, Optional columnname As String = Nothing, Optional isnullable As Boolean? = Nothing, Optional defaultvalue As Object = Nothing) As Boolean Implements iormContainerStore.Convert2ContainerData
            outvalue = invalue
            Return True
        End Function
        ''' <summary>
        '''  converts data to container data representation
        ''' </summary>
        ''' <param name="index"></param>
        ''' <param name="invalue"></param>
        ''' <param name="outvalue"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <param name="isnullable"></param>
        ''' <param name="defaultvalue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Convert2ContainerData(index As Object, invalue As Object, ByRef outvalue As Object, Optional ByRef abostrophNecessary As Boolean = False, Optional isnullable As Boolean? = Nothing, Optional defaultvalue As Object = Nothing) As Boolean Implements iormContainerStore.Convert2ContainerData
            outvalue = invalue
            Return True
        End Function
        ''' <summary>
        '''  converts data to object data representation
        ''' </summary>
        ''' <param name="index"></param>
        ''' <param name="invalue"></param>
        ''' <param name="outvalue"></param>
        ''' <param name="isnullable"></param>
        ''' <param name="defaultvalue"></param>
        ''' <param name="abostrophNecessary"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Convert2ObjectData(index As Object, invalue As Object, ByRef outvalue As Object, Optional isnullable As Boolean? = Nothing, Optional defaultvalue As Object = Nothing, Optional ByRef abostrophNecessary As Boolean = False) As Boolean Implements iormContainerStore.Convert2ObjectData
            outvalue = invalue
            Return True
        End Function



        ''' <summary>
        ''' gets the value of an store property
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetProperty(name As String) As Object Implements iormContainerStore.GetProperty
            If _properties.ContainsKey(name.ToUpper) Then Return _properties.Item(name.ToUpper)
            Return Nothing
        End Function
        ''' <summary>
        ''' returns true if the store has the property
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasProperty(name As String) As Boolean Implements iormContainerStore.HasProperty
            If _properties.ContainsKey(name.ToUpper) Then Return True
            Return False
        End Function
        ''' <summary>
        ''' sets a property of the store
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetProperty(name As String, value As Object) As Boolean Implements iormContainerStore.SetProperty
            If _properties.ContainsKey(name.ToUpper) Then _properties.Remove(name.ToUpper)
            _properties.Add(key:=name.ToUpper, value:=value)
            Return True
        End Function

        ''' <summary>
        ''' creates an unique primary key value
        ''' </summary>
        ''' <param name="pkArray"></param>
        ''' <param name="tag"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateUniquePkValue(ByRef pkArray() As Object, Optional tag As String = Nothing) As Boolean Implements iormContainerStore.CreateUniquePkValue
            If Not Me.IsInitialized AndAlso Not Refresh() Then
                CoreMessageHandler(message:="Could not initialize and refresh embedded object '" & Me.ContainerID & "'", messagetype:=otCoreMessageType.InternalError, _
                                    procedure:="EmbbededContainerStore.CreateUniquePkValue")
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' delete a record
        ''' </summary>
        ''' <param name="pkArray"></param>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteRecordByPrimaryKey(ByRef pkArray() As Object, Optional silent As Boolean = False) As Boolean Implements iormContainerStore.DeleteRecordByPrimaryKey
            If Not Me.IsInitialized AndAlso Not Refresh() Then
                CoreMessageHandler(message:="Could not initialize and refresh embedded object '" & Me.ContainerID & "'", messagetype:=otCoreMessageType.InternalError, _
                                    procedure:="EmbbededContainerStore.DeleteRecordByPrimaryKey")
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' returns a record by primary key
        ''' </summary>
        ''' <param name="pkArray"></param>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetRecordByPrimaryKey(ByRef pkArray() As Object, Optional silent As Boolean = False) As ormRecord Implements iormContainerStore.GetRecordByPrimaryKey
            If Not Me.IsInitialized AndAlso Not Refresh() Then
                CoreMessageHandler(message:="Could not initialize and refresh embedded object '" & Me.ContainerID & "'", messagetype:=otCoreMessageType.InternalError, _
                                    procedure:="EmbbededContainerStore.GetRecordByPrimaryKey")
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' returns a record by index
        ''' </summary>
        ''' <param name="indexname"></param>
        ''' <param name="keyArray"></param>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetRecordsByIndex(indexname As String, ByRef keyArray() As Object, Optional silent As Boolean = False) As List(Of ormRecord) Implements iormContainerStore.GetRecordsByIndex
            If Not Me.IsInitialized AndAlso Not Refresh() Then
                CoreMessageHandler(message:="Could not initialize and refresh embedded object '" & Me.ContainerID & "'", messagetype:=otCoreMessageType.InternalError, _
                                    procedure:="EmbbededContainerStore.GetRecordsByIndex")
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' infuse a record from an internal rowObject
        ''' </summary>
        ''' <param name="newRecord"></param>
        ''' <param name="rowObject"></param>
        ''' <param name="silent"></param>
        ''' <param name="createNewRecord"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InfuseRecord(ByRef newRecord As ormRecord, ByRef rowObject As Object, Optional silent As Boolean = False, Optional createNewRecord As Boolean = False) As Boolean Implements iormContainerStore.InfuseRecord
            If Not Me.IsInitialized AndAlso Not Refresh() Then
                CoreMessageHandler(message:="Could not initialize and refresh embedded object '" & Me.ContainerID & "'", messagetype:=otCoreMessageType.InternalError, _
                                    procedure:="EmbbededContainerStore.InfuseRecord")
                Return Nothing
            End If
        End Function


        ''' <summary>
        ''' persist record
        ''' </summary>
        ''' <param name="record"></param>
        ''' <param name="timestamp"></param>
        ''' <param name="silent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Function PersistRecord(ByRef record As ormRecord, Optional timestamp As Date = #1/1/1900#, Optional silent As Boolean = False) As Boolean Implements iormContainerStore.PersistRecord
            If Not Me.IsInitialized AndAlso Not Refresh() Then
                CoreMessageHandler(message:="Could not initialize and refresh embedded object '" & Me.ContainerID & "'", messagetype:=otCoreMessageType.InternalError, _
                                    procedure:="EmbbededContainerStore.PersistRecord")
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' refresh the container store
        ''' </summary>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Refresh(Optional force As Boolean = False) As Boolean Implements iormContainerStore.Refresh

        End Function

        ''' <summary>
        ''' returns records by selection rule
        ''' </summary>
        ''' <param name="rule"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GetRecordsBySelectionRule(rule As rulez.eXPressionTree.SelectionRule) As IEnumerable(Of ormRecord) Implements iormContainerStore.GetRecordsBySelectionRule
            If Not Me.IsInitialized AndAlso Not Refresh() Then
                CoreMessageHandler(message:="Could not initialize and refresh embedded object '" & Me.ContainerID & "'", messagetype:=otCoreMessageType.InternalError, _
                                    procedure:="EmbbededContainerStore.GetRecordsBySelectionRule")
                Return Nothing
            End If
        End Function
    End Class

    ''' <summary>
    ''' describes a container schema for embedded objects
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EmbeddedContainerSchema
        Implements iormContainerSchema

        Private _id As String

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="containerid"></param>
        ''' <param name="forceSchemaReload"></param>
        ''' <remarks></remarks>

        Public Sub New(containerid As String)
            _id = containerid
        End Sub


        ''' <summary>
        ''' gets the container id
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ContainerID As String Implements iormContainerSchema.ContainerID
            Get
                Return _id
            End Get
        End Property

        ''' <summary>
        ''' returns the entry names of the schema
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property EntryNames As List(Of String) Implements iormContainerSchema.EntryNames
            Get
                Dim anContainerAttribute As ormEmbeddedContainerAttribute = TryCast(ot.GetContainerAttribute(containerid:=Me.ContainerID), ormEmbeddedContainerAttribute)
                If anContainerAttribute IsNot Nothing Then
                    Return anContainerAttribute.EntryNames
                End If
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' returns the default values
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDefaultValue(index As Object) As Object Implements iormContainerSchema.GetDefaultValue
            Dim anContainerAttribute As ormEmbeddedContainerAttribute = TryCast(ot.GetContainerAttribute(containerid:=Me.ContainerID), ormEmbeddedContainerAttribute)
            If anContainerAttribute IsNot Nothing Then
                If IsNumeric(index) Then
                    Dim anEntry = anContainerAttribute.EntryAttributes.ElementAt(CUInt(index))
                    If anEntry Is Nothing Then Return Nothing
                    Return anEntry.DBDefaultValue
                Else
                    Dim anEntry = anContainerAttribute.EntryAttributes.Where(Function(x) x.EntryName.ToUpper = index.toupper).FirstOrDefault
                    If anEntry Is Nothing Then Return Nothing
                    Return anEntry.DBDefaultValue
                End If

            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' returns the ordinal of the domainID (entry)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDomainIDPKOrdinal() As Integer Implements iormContainerSchema.GetDomainIDPKOrdinal
            Dim anContainerAttribute As ormEmbeddedContainerAttribute = TryCast(ot.GetContainerAttribute(containerid:=Me.ContainerID), ormEmbeddedContainerAttribute)
            If anContainerAttribute IsNot Nothing Then
                Return Array.IndexOf(anContainerAttribute.PrimaryEntryNames, Commons.Domain.ConstFNDomainID)
            End If
            Return -1
        End Function

        ''' <summary>
        ''' get entry name of ordinal
        ''' </summary>
        ''' <param name="i"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEntryName(i As Integer) As String Implements iormContainerSchema.GetEntryName
            Dim anContainerAttribute As ormEmbeddedContainerAttribute = TryCast(ot.GetContainerAttribute(containerid:=Me.ContainerID), ormEmbeddedContainerAttribute)
            If anContainerAttribute IsNot Nothing Then
                Return anContainerAttribute.EntryNames.ElementAtOrDefault(i)
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' returns the ordinal of an entry
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEntryOrdinal(index As Object) As Integer Implements iormContainerSchema.GetEntryOrdinal
            Dim anContainerAttribute As ormEmbeddedContainerAttribute = TryCast(ot.GetContainerAttribute(containerid:=Me.ContainerID), ormEmbeddedContainerAttribute)
            If anContainerAttribute IsNot Nothing Then
                If Not IsNumeric(index) Then Return Array.IndexOf(anContainerAttribute.EntryNames, CStr(index))
                Return CUInt(index)
            End If
            Return -1
        End Function

        ''' <summary>
        ''' returns the fields of an index
        ''' </summary>
        ''' <param name="indexname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIndex(indexname As String) As ArrayList Implements iormContainerSchema.GetIndex
            Return Nothing
        End Function

        ''' <summary>
        ''' returns true if the index of entry is nullable
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNullable(index As Object) As Boolean Implements iormContainerSchema.GetNullable
            Dim anContainerAttribute As ormEmbeddedContainerAttribute = TryCast(ot.GetContainerAttribute(containerid:=Me.ContainerID), ormEmbeddedContainerAttribute)
            If anContainerAttribute IsNot Nothing Then
                If IsNumeric(index) Then
                    Dim anEntry = anContainerAttribute.EntryAttributes.ElementAt(CUInt(index))
                    If anEntry Is Nothing Then Return False
                    Return anEntry.IsNullable
                Else
                    Dim anEntry = anContainerAttribute.EntryAttributes.Where(Function(x) x.EntryName.ToUpper = index.toupper).FirstOrDefault
                    If anEntry Is Nothing Then Return Nothing
                    Return anEntry.IsNullable
                End If

            End If
            Return Nothing
        End Function
        ''' <summary>
        ''' returns the ordinal of the primary key
        ''' </summary>
        ''' <param name="i"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetOrdinalOfPrimaryEntry(i As UShort) As Integer Implements iormContainerSchema.GetOrdinalOfPrimaryEntry
            Return Me.GetEntryOrdinal(Me.GetPrimaryEntryNames(i))
        End Function

        ''' <summary>
        ''' returns the entry name of the primary ordinal
        ''' </summary>
        ''' <param name="i"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPrimaryEntryNames(i As UShort) As String Implements iormContainerSchema.GetPrimaryEntryNames
            Dim anContainerAttribute As ormEmbeddedContainerAttribute = TryCast(ot.GetContainerAttribute(containerid:=Me.ContainerID), ormEmbeddedContainerAttribute)
            If anContainerAttribute IsNot Nothing Then
                Return anContainerAttribute.PrimaryEntryNames.ElementAt(i)
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' returns true if entry name has a default value
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasDefaultValue(index As Object) As Boolean Implements iormContainerSchema.HasDefaultValue
            Dim anContainerAttribute As ormEmbeddedContainerAttribute = TryCast(ot.GetContainerAttribute(containerid:=Me.ContainerID), ormEmbeddedContainerAttribute)
            If anContainerAttribute IsNot Nothing Then
                If IsNumeric(index) Then
                    Dim anEntry = anContainerAttribute.EntryAttributes.ElementAt(CUInt(index))
                    If anEntry Is Nothing Then Return Nothing
                    Return anEntry.HasValueDBDefaultValue
                Else
                    Dim anEntry = anContainerAttribute.EntryAttributes.Where(Function(x) x.EntryName.ToUpper = index.toupper).FirstOrDefault
                    If anEntry Is Nothing Then Return Nothing
                    Return anEntry.HasValueDBDefaultValue
                End If

            End If
            Return False
        End Function
        ''' <summary>
        ''' returns true if the schema has the entry name
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasEntryName(name As String) As Boolean Implements iormContainerSchema.HasEntryName
            Dim anContainerAttribute As ormEmbeddedContainerAttribute = TryCast(ot.GetContainerAttribute(containerid:=Me.ContainerID), ormEmbeddedContainerAttribute)
            If anContainerAttribute IsNot Nothing Then
                Return anContainerAttribute.EntryNames.Contains(name.ToUpper)
            End If
            Return False
        End Function
        ''' <summary>
        ''' returns true if the schema has the named index
        ''' </summary>
        ''' <param name="indexname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasIndex(indexname As String) As Boolean Implements iormContainerSchema.HasIndex
            Return False
        End Function
        ''' <summary>
        ''' returns true if the schema has the entryname as primary key
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasPrimaryEntryName(ByRef name As String) As Boolean Implements iormContainerSchema.HasPrimaryEntryName
            Dim anContainerAttribute As ormEmbeddedContainerAttribute = TryCast(ot.GetContainerAttribute(containerid:=Me.ContainerID), ormEmbeddedContainerAttribute)
            If anContainerAttribute IsNot Nothing Then
                Return anContainerAttribute.PrimaryEntryNames.Contains(name.ToUpper)
            End If
            Return False
        End Function
        ''' <summary>
        ''' returns the list of indices
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Indices As List(Of String) Implements iormContainerSchema.Indices
            Get
                Return Nothing
            End Get
        End Property
        ''' <summary>
        ''' returns true if schema is initialized
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsInitialized As Boolean Implements iormContainerSchema.IsInitialized
            Get
                Return True
            End Get
        End Property
        ''' <summary>
        ''' returns the native container name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NativeDBContainerName As String Implements iormContainerSchema.NativeDBContainerName
            Get
                Return _id
            End Get
        End Property
        ''' <summary>
        ''' returns the number of entry names
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NoEntries As Integer Implements iormContainerSchema.NoEntries
            Get
                Dim anContainerAttribute As ormEmbeddedContainerAttribute = TryCast(ot.GetContainerAttribute(containerid:=Me.ContainerID), ormEmbeddedContainerAttribute)
                If anContainerAttribute IsNot Nothing Then
                    Return anContainerAttribute.EntryNames.Count
                End If
                Return 0
            End Get
        End Property
        ''' <summary>
        ''' returns the number of primary key entry names
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function NoPrimaryEntries() As Integer Implements iormContainerSchema.NoPrimaryEntries
            Dim anContainerAttribute As ormEmbeddedContainerAttribute = TryCast(ot.GetContainerAttribute(containerid:=Me.ContainerID), ormEmbeddedContainerAttribute)
            If anContainerAttribute IsNot Nothing Then
                Return anContainerAttribute.PrimaryEntryNames.Count
            End If
            Return 0
        End Function
        ''' <summary>
        ''' returns a list of primary key entry names
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property PrimaryEntryNames As List(Of String) Implements iormContainerSchema.PrimaryEntryNames
            Get
                Dim anContainerAttribute As ormEmbeddedContainerAttribute = TryCast(ot.GetContainerAttribute(containerid:=Me.ContainerID), ormEmbeddedContainerAttribute)
                If anContainerAttribute IsNot Nothing Then
                    Return anContainerAttribute.PrimaryEntryNames.ToList
                End If
                Return Nothing
            End Get
        End Property
        ''' <summary>
        ''' returns the index name of the primary key
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property PrimaryKeyIndexName As String Implements iormContainerSchema.PrimaryKeyIndexName
            Get
                Return "PRIMARYKEY"
            End Get
        End Property
        ''' <summary>
        ''' refresh the schema
        ''' </summary>
        ''' <param name="reloadForce"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Refresh(Optional reloadForce As Boolean = False) As Boolean Implements iormContainerSchema.Refresh
            Return True
        End Function
    End Class

    ''' <summary>
    ''' describes an EmbeddedObject
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface iormEmbeddedObject
        Inherits iormPersistable
        ''' empty - used only to mark the object classes to be registered with the embedded object factory
    End Interface
    ''' <summary>
    ''' a singleton embedded Object Factory class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ormEmbeddedObjectProvider
        Inherits ormDataObjectProvider
        Implements iormDataObjectProvider


        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="Session"></param>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Initialize and register all business objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function Initialize(Optional repository As ormObjectRepository = Nothing) As Boolean
            If Not MyBase.Initialize(repository:=repository) Then Return False

            ''' autoregister
            ''' 
            Dim thisAsm As Assembly = Assembly.GetExecutingAssembly()
            Dim aClassList As List(Of Type) = thisAsm.GetTypes().Where(Function(t) _
                                                    ((t.GetInterfaces.Contains(GetType(iormEmbeddedObject)) AndAlso t.IsClass AndAlso Not t.IsAbstract))).ToList()
            Try
                For Each aClass In aClassList
                    ''' check the class type attributes
                    Dim aDescription As ObjectClassDescription = ot.GetObjectClassDescription(aClass)
                    If aDescription IsNot Nothing Then
                        If aDescription.ObjectAttribute.HasValueID Then Me.RegisterObjectID(aDescription.ObjectAttribute.ID)
                    End If
                Next

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ormBusinessObjectFactory.Initialize")
                Return False
            End Try

            _IsInitialized = True
            Return _IsInitialized
        End Function
        ''' <summary>
        ''' retrieve all business objects regardless of selection criteria
        ''' </summary>
        ''' <param name="type"></param>
        ''' <param name="domainID"></param>
        ''' <param name="forceReload"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RetrieveAll(type As Type, _
                                    Optional key As ormDatabaseKey = Nothing, _
                                    Optional domainID As String = Nothing, _
                                    Optional deleted As Boolean = False, _
                                    Optional forceReload As Boolean? = Nothing, _
                                    Optional runtimeOnly As Boolean? = Nothing) As IEnumerable(Of iormDataObject) Implements iormDataObjectProvider.RetrieveAll


            ''' return the query
            ' Return Me.RetrieveAllByQuery(type:=type, domainID:=domainID, deleted:=deleted, forceReload:=forceReload, where:=wherestring, runtimeOnly:=runtimeOnly)
        End Function
        

    End Class
End Namespace

