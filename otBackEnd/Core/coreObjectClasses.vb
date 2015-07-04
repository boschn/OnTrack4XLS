Option Explicit On

REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** CLASS Repository for On Track Database Backend Library
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2014-01-08
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************''' <summary>

Imports System.Collections
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Diagnostics.Debug
Imports System.Text.RegularExpressions
Imports System.Collections.Concurrent

Imports System.IO
Imports System.Threading

Imports OnTrack
Imports OnTrack.Database
Imports System.Reflection
Imports System.Reflection.Emit
Imports System.Linq.Expressions
Imports OnTrack.Core

Namespace OnTrack


    ''' <summary>
    ''' store for attribute information in the dataobject classes - relies in the CORE
    ''' </summary>
    ''' <remarks></remarks>

    Public Class ObjectClassRepository
        Implements iormDataObjectRepository

        ''' <summary>
        ''' Event Args for Object Class Repository
        ''' </summary>
        ''' <remarks></remarks>
        Public Class EventArgs
            Inherits System.EventArgs

            Private _id As String
            Private _description As ObjectClassDescription

            Public Sub New(objectname As String, description As ObjectClassDescription)
                _id = objectname
                _description = description
            End Sub

            ''' <summary>
            ''' Gets the object class description.
            ''' </summary>
            ''' <value>The objectdefinition.</value>
            Public ReadOnly Property Description() As ObjectClassDescription
                Get
                    Return Me._description
                End Get
            End Property

            ''' <summary>
            ''' Gets the objectname.
            ''' </summary>
            ''' <value>The objectname.</value>
            Public ReadOnly Property Objectname() As String
                Get
                    Return Me._id
                End Get
            End Property

        End Class

        Private _isInitialized As Boolean = False
        Private _lock As New Object
        Private _BootStrapSchemaCheckSum As ULong

        '** stores
        Private _CreateInstanceDelegateStore As New Dictionary(Of String, ObjectClassDescription.CreateInstanceDelegate) ' Class Name and Delegate for Instance Creator
        Private _DescriptionsByClass As New Dictionary(Of String, ObjectClassDescription) 'name of classes with id
        Private _DescriptionsByID As New Dictionary(Of String, ObjectClassDescription) 'name of classes with id
        Private _Container2ObjectClassStore As New Dictionary(Of String, List(Of Type)) 'name of tables to types
        Private _BootstrapObjectClasses As New List(Of Type)
        Private _ClassDescriptorPerModule As New Dictionary(Of String, List(Of ObjectClassDescription))
        Private _ContainerAttributesStore As New Dictionary(Of String, iormContainerAttribute)

        Public Event OnObjectClassDescriptionLoaded(sender As Object, e As ObjectClassRepository.EventArgs)

        ''' <summary>
        ''' constructor of the object class repository
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()

        End Sub

#Region "Properties"


        ''' <summary>
        ''' returns the count for the class description store (all classes in store)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Count As ULong
            Get
                Me.Initialize()
                Return _DescriptionsByClass.Count
            End Get
        End Property
        ''' <summary>
        ''' returns an IEnumerable of all ObjectClassDescriptions
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectClassDescriptions As IEnumerable(Of ObjectClassDescription)
            Get
                Me.Initialize()
                Return _DescriptionsByClass.Values
            End Get
        End Property
        ''' <summary>
        ''' gets the Checksum of the ObjectClassRepository for Bootstrapping classes 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Property BootstrapSchemaChecksum As ULong
            Get
                Return _BootStrapSchemaCheckSum
            End Get
            Private Set(value As ULong)
                _BootStrapSchemaCheckSum = value
            End Set
        End Property
#End Region

        ''' <summary>
        ''' Add oder modify a table attribute 
        ''' </summary>
        ''' <param name="tableattribute"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AlterContainerAttribute(ByRef containerAttribute As iormContainerAttribute, Optional fieldinfo As FieldInfo = Nothing) As Boolean
            Dim aContainerAttribute As iormContainerAttribute
            Dim afieldvalue As String
            Dim aContainerID As String

            If fieldinfo IsNot Nothing Then
                afieldvalue = fieldinfo.GetValue(Nothing).ToString.ToUpper
            End If

            '***
            If containerAttribute.HasValueContainerID Then
                aContainerID = containerAttribute.ContainerID
            ElseIf fieldinfo IsNot Nothing Then
                aContainerID = afieldvalue
            ElseIf containerAttribute.HasValueID Then
                aContainerID = containerAttribute.ID
            Else
                CoreMessageHandler(message:="cannot determine container name", procedure:="ObjectClassrepository.AlterContainerAttribute", _
                                   messagetype:=otCoreMessageType.InternalError)
                Return False
            End If

            If _ContainerAttributesStore.ContainsKey(aContainerID) Then
                aContainerAttribute = _ContainerAttributesStore.Item(aContainerID)
                '** default values
                With aContainerAttribute
                    '**
                    If Not .HasValueContainerID Then .ContainerID = aContainerID
                    '** version
                    If containerAttribute.HasValueVersion Then
                        If Not .HasValueVersion Then
                            .Version = containerAttribute.Version
                        ElseIf .Version < containerAttribute.Version Then
                            .Version = containerAttribute.Version
                        End If
                    End If

                    '** copy
                    '** true overrules
                    If (.HasValueAddDomainBehavior AndAlso Not .HasDomainBehavior AndAlso containerAttribute.HasValueAddDomainBehavior) _
                        OrElse (Not .HasValueAddDomainBehavior AndAlso containerAttribute.HasValueAddDomainBehavior) Then
                        .HasDomainBehavior = containerAttribute.HasDomainBehavior
                    End If
                    If (.HasValueDeleteFieldBehavior AndAlso Not .HasDeleteFieldBehavior AndAlso containerAttribute.HasValueDeleteFieldBehavior) _
                       OrElse (Not .HasValueDeleteFieldBehavior AndAlso containerAttribute.HasValueDeleteFieldBehavior) Then
                        .HasDeleteFieldBehavior = containerAttribute.HasDeleteFieldBehavior
                    End If
                    If (.HasValueSpareFields AndAlso Not .HasValueSpareFields AndAlso containerAttribute.HasValueSpareFields) _
                      OrElse (Not .HasValueSpareFields AndAlso containerAttribute.HasValueSpareFields) Then
                        .HasSpareFields = containerAttribute.HasSpareFields
                    End If
                    If (.HasValueUseCache AndAlso Not .UseCache AndAlso containerAttribute.HasValueUseCache) _
                     OrElse (Not .HasValueUseCache AndAlso containerAttribute.HasValueUseCache) Then
                        .UseCache = containerAttribute.UseCache
                    End If
                    '** other
                    If Not .HasValueDescription AndAlso containerAttribute.HasValueDescription Then
                        .Description = containerAttribute.Description
                    End If
                    If Not .HasValuePrimaryKey AndAlso containerAttribute.HasValuePrimaryKey Then
                        .PrimaryKey = containerAttribute.PrimaryKey
                    End If
                    If Not .HasValueID AndAlso containerAttribute.HasValueID Then
                        .ID = containerAttribute.ID
                    End If

                    '** import foreign keys
                    For Each afk In containerAttribute.ForeignkeyAttributes
                        If Not .HasForeignKey(afk.ID) Then
                            .AddForeignKey(afk)
                        End If
                    Next
                    '** import columns
                    For Each acol In containerAttribute.Entries
                        If Not .HasEntry(acol.EntryName) Then
                            .AddEntry(acol)
                        End If
                    Next
                    '** import indices
                    For Each anindex In containerAttribute.IndexAttributes
                        If Not .HasIndex(anindex.IndexName) Then
                            .AddIndex(anindex)
                        End If
                    Next
                End With
                '** overwrite
                containerAttribute = aContainerAttribute
            Else
                '** take the new one
                With containerAttribute
                    '**
                    .ContainerID = aContainerID
                    '** version
                    If Not .HasValueVersion Then .Version = 1
                End With
                _ContainerAttributesStore.Add(key:=containerAttribute.ContainerID.ToUpper, value:=containerAttribute)
            End If

        End Function
        ''' <summary>
        ''' returns the names of the bootstrapping tables
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetBootStrapObjectClassDescriptions() As List(Of ObjectClassDescription)
            Me.Initialize()
            Dim aList = New List(Of ObjectClassDescription)
            For Each aClasstype In _BootstrapObjectClasses
                Dim anObjectDescription As ObjectClassDescription = Me.GetObjectClassDescription(aClasstype)
                If anObjectDescription IsNot Nothing Then
                    If Not aList.Contains(anObjectDescription) Then aList.Add(anObjectDescription)
                Else
                    CoreMessageHandler(message:="Object Description not found for bootstrapping classes", objectname:=aClasstype.Name, _
                                       procedure:="objectClassRepository.GetBootStrapObjectClassDescriptions", messagetype:=otCoreMessageType.InternalError)
                End If
            Next
            Return aList
        End Function
        ''' <summary>
        ''' returns the names of the bootstrapping tables
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetBootStrapContainerIDs() As List(Of String)
            Me.Initialize()
            Dim aList = New List(Of String)
            For Each aClasstype In _BootstrapObjectClasses
                Dim anObjectDescription As ObjectClassDescription = Me.GetObjectClassDescription(aClasstype)
                If anObjectDescription IsNot Nothing Then
                    For Each aName In anObjectDescription.Tablenames
                        If Not aList.Contains(aName.ToUpper) Then aList.Add(aName.ToUpper)
                    Next
                Else
                    CoreMessageHandler(message:="Object Description not found for bootstrapping classes", objectname:=aClasstype.Name, _
                                       procedure:="objectClassRepository.getBootStrapTablesNames", messagetype:=otCoreMessageType.InternalError)
                End If
            Next
            Return aList
        End Function
        ''' <summary>
        ''' returns the ObjectClass Type for an object class name
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateInstance(type As System.Type) As iormDataObject
            Try
                If Not _CreateInstanceDelegateStore.ContainsKey(key:=type.FullName.ToUpper) Then
                    CoreMessageHandler(message:="type is not found in the instance creator store of class descriptions", _
                                       argument:=type.FullName, procedure:="ObjectClassRepository.CreateInstance", messagetype:=otCoreMessageType.InternalError)
                    Return Nothing
                End If

                Dim aDelegate As ObjectClassDescription.CreateInstanceDelegate = _CreateInstanceDelegateStore.Item(key:=type.FullName.ToUpper)
                Dim anObject As iormDataObject = aDelegate()
                Return anObject
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectClassRepository.CreateInstance", argument:=type.FullName)
                Return Nothing
            End Try
        End Function
        ''' <summary>
        ''' returns the ObjectClass Type for an object class name
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectClassType(objectname As String) As System.Type Implements iormDataObjectRepository.GetObjectType
            If _DescriptionsByID.ContainsKey(key:=objectname.ToUpper) Then
                Return _DescriptionsByID.Item(key:=objectname.ToUpper).Type
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' returns the ObjectClass Description
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectClassDescription([type] As Type) As ObjectClassDescription
            Return GetObjectClassDescription([type].FullName)
        End Function
        ''' <summary>
        ''' returns the ObjectClassDescription for a ObjectDescription Class by name
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectClassDescription(typename As String) As ObjectClassDescription
            Me.Initialize()

            If _DescriptionsByClass.ContainsKey(key:=typename.ToUpper) Then
                Return _DescriptionsByClass.Item(key:=typename.ToUpper)
            Else
                '' was ist not a fullname ?!
                Dim aType As System.Type = Assembly.GetExecutingAssembly.GetType(name:=typename, throwOnError:=False, ignoreCase:=True)
                If aType IsNot Nothing AndAlso _DescriptionsByClass.ContainsKey(key:=aType.FullName.ToUpper) Then
                    Return _DescriptionsByClass.Item(key:=aType.FullName.ToUpper)
                Else
                    Return Nothing
                End If
            End If

        End Function
        ''' <summary>
        ''' returns an iObjectDefinition 
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectDefinition(id As String) As iObjectDefinition Implements IDataObjectRepository.GetIObjectDefinition
            Return TryCast(Me.GetObjectClassDescription(id).ObjectAttribute, iObjectDefinition)
        End Function
        ''' <summary>
        ''' returns an iObjectDefinition 
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectDefinition(type As System.Type) As iObjectDefinition Implements IDataObjectRepository.GetIObjectDefinition
            Return TryCast(Me.GetObjectClassDescription(type.FullName).ObjectAttribute, iObjectDefinition)
        End Function
        ''' <summary>
        ''' returns the ObjectClassDescription for a ObjectDescription Class by name
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectClassDescriptionByID(id As String) As ObjectClassDescription
            Me.Initialize()

            If _DescriptionsByID.ContainsKey(key:=id.ToUpper) Then
                Return _DescriptionsByID.Item(key:=id.ToUpper)
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' return the object id of a type
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectID(type As System.Type) As String Implements iormDataObjectRepository.GetObjectname
            Return GetObjectID(type.FullName)
        End Function
        ''' <summary>
        ''' returns the object id for a type fullname
        ''' </summary>
        ''' <param name="fullname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectID(fullname As String) As String Implements iormDataObjectRepository.GetObjectname
            If _DescriptionsByClass.ContainsKey(fullname) Then
                Return _DescriptionsByClass.Item(fullname).ObjectAttribute.Objectname
            Else
                Throw New ormException(ormException.Types.TypeNotFound, arguments:={fullname})
            End If
        End Function
        ''' <summary>
        ''' returns an Ienumerable of all database driver attributes of the executing assembly
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDBDriverAttributes() As IEnumerable(Of ormDatabaseDriverAttribute)
            Dim anEnumerable As New List(Of ormDatabaseDriverAttribute)
            Dim thisAsm As Assembly = Assembly.GetExecutingAssembly()
            Dim aDriverClassList As List(Of Type) = thisAsm.GetTypes().Where(Function(t) _
                                                    ((t.GetInterfaces.Contains(GetType(iormDatabaseDriver)) AndAlso t.IsClass AndAlso Not t.IsAbstract))).ToList()
            Try
                For Each aClass In aDriverClassList
                    ''' check the class type attributes
                    '''
                    For Each anAttribute As System.Attribute In aClass.GetCustomAttributes(False)
                        ''' Object Attribute
                        ''' 
                        If anAttribute.GetType().Equals(GetType(ormDatabaseDriverAttribute)) Then
                            Dim aDriverAttribute = DirectCast(anAttribute, ormDatabaseDriverAttribute)
                            aDriverAttribute.Type = aClass
                            anEnumerable.Add(aDriverAttribute)
                        End If
                    Next

                Next

                Return anEnumerable
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectClassRepository.GetDBDriverAttributes")
                Return anEnumerable
            End Try

        End Function
        ''' <summary>
        ''' returns a container definition
        ''' </summary>
        ''' <param name="containerID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetContainerDefinition(containerID As String) As iormContainerDefinition Implements iormDataObjectRepository.GetContainerDefinition
            Return GetContainerAttribute(containerID)
        End Function

        ''' <summary>
        ''' returns the container attribute for a container name
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetContainerAttribute(containerID As String) As iormContainerAttribute
            Me.Initialize()

            If _ContainerAttributesStore.ContainsKey(key:=containerID.ToUpper) Then
                Return _ContainerAttributesStore(key:=containerID.ToUpper)
            Else
                Return Nothing
            End If

        End Function
        ''' <summary>
        ''' returns a list of all container attributes 
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ContainerAttributes() As List(Of iormContainerAttribute)
            Get
                Me.Initialize()
                Return _ContainerAttributesStore.Values.ToList
            End Get
        End Property

        ''' <summary>
        ''' returns a list of all table attributes
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTableAttributes() As List(Of ormTableAttribute)
            Dim aList As New List(Of ormTableAttribute)
            For Each anAttribute In Me.ContainerAttributes
                If anAttribute.GetType Is GetType(ormTableAttribute) Then
                    aList.Add(anAttribute)
                End If
            Next
            Return aList
        End Function

        ''' <summary>
        ''' gets a iormObjectEntryDefinition
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectEntry(entryname As String, Optional objectname As String = Nothing, Optional runtimeOnly? As Boolean = Nothing) As iormObjectEntryDefinition Implements iormDataObjectRepository.GetEntryDefinition
            Return Me.GetObjectEntryAttribute(entryname:=entryname, objectname:=objectname)
        End Function
        ''' <summary>
        ''' returns all iormObjectEntryDefinitions of a objectname
        ''' </summary>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectEntries(objectname As String) As List(Of iormObjectEntryDefinition) Implements iormDataObjectRepository.GetObjectEntries
            Me.Initialize()
            If _DescriptionsByID.ContainsKey(key:=objectname.ToUpper) Then
                Dim aList = _DescriptionsByID.Item(key:=objectname.ToUpper).ObjectEntryAttributes
                Dim anewList As List(Of iormObjectEntryDefinition)
                For Each anEntry In aList
                    anewList.Add(anEntry)
                Next
                Return anewList
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' returns all iormObjectEntryDefinitions of an xid
        ''' </summary>
        ''' <param name="objectname"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectEntriesByXID(xid As String, Optional objectname As String = Nothing) As IList(Of iormObjectEntryDefinition) Implements iormDataObjectRepository.GetEntriesByXID
            Me.Initialize()
            Dim aList As New List(Of iormObjectEntryDefinition)
            If String.IsNullOrEmpty(objectname) Then
                For Each aDescription In _DescriptionsByID.Values
                    aList.AddRange(aDescription.ObjectEntryAttributes.Where(Function(x) x.HasValueXID AndAlso x.XID = xid))
                Next
                Return aList
            ElseIf _DescriptionsByID.ContainsKey(xid) Then
                Dim aDescription As ObjectClassDescription = _DescriptionsByID(key:=objectname.ToUpper)
                aList.AddRange(aDescription.ObjectEntryAttributes.Where(Function(x) x.HasValueXID AndAlso x.XID = xid))
                Return aList
            Else
                Return aList
            End If

        End Function
        ''' <summary>
        ''' gets a ormObjectEntryAttribute for an entryname
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectEntryAttribute(entryname As String, Optional objectname As String = Nothing) As ormObjectEntryAttribute
            Me.Initialize()
            If String.IsNullOrEmpty(objectname) Then objectname = Shuffle.NameSplitter(entryname).First

            If Not String.IsNullOrEmpty(objectname) AndAlso _DescriptionsByID.ContainsKey(key:=objectname.ToUpper) Then
                Return _DescriptionsByID.Item(key:=objectname.ToUpper).GetObjectEntryAttribute(entryname:=entryname)
            Else
                Return Nothing
            End If

        End Function
        ''' <summary>
        ''' gets a ormObjectEntryAttribute for an entryname
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasObjectEntryAttribute(entryname As String, objectname As String) As Boolean Implements iormDataObjectRepository.HasObjectEntry
            Me.Initialize()
            If _DescriptionsByID.ContainsKey(key:=objectname.ToUpper) Then
                Return _DescriptionsByID.Item(key:=objectname.ToUpper).HasObjectEntryAttribute(entryname:=entryname)
            Else
                Return Nothing
            End If

        End Function

        ''' <summary>
        ''' substitute referenced properties in the reference
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SubstituteReferencedContainerEntryProperties(ByRef attribute As iormContainerEntryAttribute) As Boolean
            '*** REFERENCE OBJECT ENTRY
            If attribute.HasValueReferenceObjectEntry Then
                Dim refObjectName As String = String.Empty
                Dim refObjectEntry As String = String.Empty
                Dim names = Shuffle.NameSplitter(attribute.ReferenceObjectEntry)
                If names.Count > 1 Then
                    refObjectName = names(0)
                    refObjectEntry = names(1)
                Else
                    CoreMessageHandler(message:="objectname is missing in reference " & attribute.ReferenceObjectEntry, procedure:="ObjectClassRepository.GetReferenceTableColumn", _
                                       messagetype:=otCoreMessageType.InternalError, argument:=attribute.ReferenceObjectEntry, containerEntryName:=attribute.EntryName, containerID:=attribute.ContainerID)
                    Return False
                End If

                ' will not take 
                Dim anReferenceAttribute As ormObjectEntryAttribute = _
                    Me.GetObjectEntryAttribute(entryname:=refObjectEntry, objectname:=refObjectName)

                If anReferenceAttribute IsNot Nothing Then
                    With anReferenceAttribute
                        If .HasValueID And Not attribute.HasValueID Then attribute.ID = .ID '-> should be set by the const value
                        If .HasValueContainerID And Not attribute.HasValueContainerID Then attribute.ContainerID = .ContainerID
                        If .HasValueContainerEntryName And Not attribute.HasValueContainerEntryName Then attribute.EntryName = .ContainerEntryName
                        If .HasValueRelation And Not attribute.HasValueRelation Then attribute.Relation = .Relation
                        If .HasValueIsNullable And Not attribute.HasValueIsNullable Then attribute.IsNullable = .IsNullable
                        If .HasValueIsUnique And Not attribute.HasValueIsUnique Then attribute.IsUnique = .IsUnique
                        If .HasValueDataType And Not attribute.HasValueDataType Then attribute.DataType = .Datatype
                        'If .HasValueInnerDataType And Not attribute.HasValueInnerDataType Then attribute.InnerDataType = .InnerDatatype
                        If .HasValueSize And Not attribute.HasValueSize Then attribute.Size = .Size
                        If .HasValueDescription And Not attribute.HasValueDescription Then attribute.Description = .Description
                        If .HasValueDBDefaultValue And Not attribute.HasValueDBDefaultValue Then attribute.DBDefaultValue = .DBDefaultValue
                        If .HasValueVersion And Not attribute.HasValueVersion Then attribute.Version = .Version

                        If .HasValueUseForeignKey And Not attribute.HasValueUseForeignKey Then attribute.UseForeignKey = .UseForeignKey
                        If .HasValueForeignKeyReferences And Not attribute.HasValueForeignKeyReferences Then attribute.ForeignKeyReferences = .ForeignKeyReferences
                        If .HasValueForeignKeyProperties And Not attribute.HasValueForeignKeyProperties Then attribute.ForeignKeyProperties = .ForeignKeyProperties
                    End With

                Else
                    CoreMessageHandler(message:="referenceObjectEntry  object id '" & refObjectName & "' and column name '" & refObjectEntry & "' not found for column schema", _
                                       containerEntryName:=attribute.EntryName, containerID:=attribute.ContainerID, procedure:="ObjectClassRepository.GetReferenceTableColumn", messagetype:=otCoreMessageType.InternalError)
                End If
                Return True
            Else
                Return False
            End If

        End Function
        ''' <summary>
        ''' substitute referenced properties in the reference
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SubstituteReferencedObjectEntry(ByRef attribute As ormObjectEntryAttribute) As Boolean
            '*** REFERENCE OBJECT ENTRY
            If attribute.HasValueReferenceObjectEntry Then
                Dim refObjectName As String = String.Empty
                Dim refObjectEntry As String = String.Empty
                Dim names = Shuffle.NameSplitter(attribute.ReferenceObjectEntry)
                If names.Count > 1 Then
                    refObjectName = names(0)
                    refObjectEntry = names(1)
                Else
                    refObjectEntry = attribute.ReferenceObjectEntry
                    refObjectName = attribute.ObjectName
                End If

                ' will not take 
                Dim anReferenceAttribute As ormObjectEntryAttribute = _
                    Me.GetObjectEntryAttribute(entryname:=refObjectEntry, objectname:=refObjectName)

                If anReferenceAttribute IsNot Nothing Then
                    With anReferenceAttribute
                        '** read table column elements and then the object references
                        If SubstituteReferencedContainerEntryProperties(attribute:=attribute) Then
                            If .HasValueEntryType And Not attribute.HasValueEntryType Then attribute.EntryType = .EntryType
                            If .HasValueTitle And Not attribute.HasValueTitle Then attribute.Title = .Title
                            If .HasValueCategory And Not attribute.HasValueCategory Then attribute.Category = .Category
                            If .HasValueDescription And Not attribute.HasValueDescription Then attribute.Description = .Description
                            If .HasValueInnerDataType And Not attribute.HasValueInnerDataType Then attribute.InnerDatatype = .InnerDatatype
                            If .HasValueXID And Not attribute.HasValueXID Then attribute.XID = .XID
                            If .HasValueAliases And Not attribute.HasValueAliases Then attribute.Aliases = .Aliases
                            If .HasValueObjectEntryProperties And Not attribute.HasValueObjectEntryProperties Then attribute.Properties = .Properties
                            If .HasValueVersion And Not attribute.HasValueVersion Then attribute.Version = .Version
                            If .hasValueIsSpareField And Not attribute.hasValueIsSpareField Then attribute.SpareFieldTag = .SpareFieldTag

                            If .HasValueIsRendering And Not attribute.HasValueIsRendering Then attribute.IsRendering = .IsRendering
                            If .HasValueRenderProperties And Not attribute.HasValueRenderProperties Then attribute.RenderProperties = .RenderProperties
                            If .HasValueRenderRegExpMatch And Not attribute.HasValueRenderRegExpMatch Then attribute.RenderRegExpMatch = .RenderRegExpMatch
                            If .HasValueRenderRegExpPattern And Not attribute.HasValueRenderRegExpPattern Then attribute.RenderRegExpPattern = .RenderRegExpPattern

                            If .HasValueValidate And Not attribute.HasValueValidate Then attribute.Validate = .Validate
                            If .HasValueLowerRange And Not attribute.HasValueLowerRange Then attribute.LowerRange = .LowerRange
                            If .HasValueUpperRange And Not attribute.HasValueUpperRange Then attribute.UpperRange = .UpperRange
                            If .HasValueValidationProperties And Not attribute.HasValueValidationProperties Then attribute.ValidationProperties = .ValidationProperties
                            If .HasValueLookupCondition And Not attribute.HasValueLookupCondition Then attribute.LookupCondition = .LookupCondition
                            If .HasValuePossibleValues And Not attribute.HasValuePossibleValues Then attribute.PossibleValues = .PossibleValues
                        End If

                    End With

                Else
                    CoreMessageHandler(message:="referenceObjectEntry  object id '" & refObjectName & "' and column name '" & refObjectEntry & "' not found for column schema", _
                                       entryname:=attribute.EntryName, objectname:=attribute.ObjectName, procedure:="ObjectClassRepository.SubstituteReferencedObjectEntry", messagetype:=otCoreMessageType.InternalError)
                End If
                Return True
            Else
                Return False
            End If

        End Function
        ''' <summary>
        ''' returns the IContainerEntry Definition
        ''' </summary>
        ''' <param name="entryname"></param>
        ''' <param name="containerid"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIContainerEntryDefinition(entryname As String, Optional containerid As String = Nothing, Optional runtimeOnly As Boolean? = Nothing) As iormContainerEntryDefinition Implements iormDataObjectRepository.GetContainerEntry
            Return Me.GetContainerEntryAttribute(entryname, containerid)
        End Function
        ''' <summary>
        ''' returns the schemaColumnAttribute for a given columnname and tablename
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetContainerEntryAttribute(entryname As String, Optional containerid As String = Nothing) As ormContainerEntryAttribute
            Me.Initialize()
            Dim anEntryname As String = String.Empty
            Dim aContainerID As String = String.Empty
            Dim names() As String = Shuffle.NameSplitter(entryname)
            Dim anAttribute As ormContainerEntryAttribute



            '** split the names
            If Not String.IsNullOrWhiteSpace(containerid) And names.Count = 1 Then
                anEntryname = entryname.ToUpper
                aContainerID = containerid.ToUpper
            ElseIf names.Count > 1 AndAlso String.IsNullOrWhiteSpace(containerid) Then
                aContainerID = names(0)
                anEntryname = names(1)
            Else
                CoreMessageHandler(message:="more than one container in the description but no container id specified in the entry name or as argument", _
                                   messagetype:=otCoreMessageType.InternalError, procedure:="ObjectClassDescription.GetContainerEntryAttribute", _
                                   argument:=names, containerID:=containerid, containerEntryName:=entryname)
                Return Nothing
            End If

            '** return
            If _ContainerAttributesStore.ContainsKey(key:=containerid.ToUpper) Then
                anAttribute = _ContainerAttributesStore.Item(key:=aContainerID).GetEntry(anEntryname)
                '*** substitute references
                SubstituteReferencedContainerEntryProperties(attribute:=anAttribute)
                '** return
                Return anAttribute

            Else
                Return Nothing
            End If

        End Function
        ''' <summary>
        ''' gets a list of ObjectClassDescriptions per tablename or empty if none
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectClassDescriptionsByContainer(containerID As String, Optional onlyenabled As Boolean = True) As List(Of ObjectClassDescription)
            Me.Initialize()
            Dim alist As New List(Of ObjectClassDescription)
            If Not _ContainerAttributesStore.ContainsKey(containerID.ToUpper) Then Return alist
            If onlyenabled AndAlso Not _ContainerAttributesStore.Item(containerID.ToUpper).Enabled Then Return alist

            If _Container2ObjectClassStore.ContainsKey(containerID.ToUpper) Then
                For Each aObjectType In _Container2ObjectClassStore.Item(containerID.ToUpper)
                    alist.Add(GetObjectClassDescription(aObjectType))
                Next
            End If
            Return alist
        End Function
        ''' <summary>
        ''' returns true if the objectDefinition exists
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasObjectDefinition(id As String) As Boolean Implements IDataObjectRepository.HasObjectDefinition
            Me.Initialize()
            If _DescriptionsByID.ContainsKey(key:=id.ToUpper) Then Return True
            Return False
        End Function
        ''' <summary>
        ''' returns true if the objectDefinition exists
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasObjectDefinition(type As System.Type) As Boolean Implements IDataObjectRepository.HasObjectDefinition
            Me.Initialize()
            If _DescriptionsByClass.ContainsKey(key:=type.FullName) Then Return True
            Return False
        End Function

        ''' <summary>
        ''' returns an iormobjectdefinition or nothing
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="runtimeOnly"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectDefinition(id As String, Optional runtimeOnly As Boolean? = Nothing) As iormObjectDefinition Implements iormDataObjectRepository.GetObjectDefinition
            If Me.HasObjectDefinition(id:=id) Then Return Me.GetObjectClassDescriptionByID(id:=id).ObjectAttribute
            Return Nothing
        End Function

        ''' <summary>
        ''' returns a list of iObjectDefinitions
        ''' </summary>
        ''' <param name="modulename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IObjectDefinitions As IEnumerable(Of iObjectDefinition) Implements IDataObjectRepository.IObjectDefinitions
            Get
                Me.Initialize()
                Dim aList As New List(Of iObjectDefinition)
                For Each anEntry In _DescriptionsByID.Values
                    aList.Add(anEntry)
                Next
                Return aList
            End Get

        End Property
        ''' <summary>
        ''' returns a list of iormObjectDefinitions
        ''' </summary>
        ''' <param name="modulename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectDefinitions As IEnumerable(Of iormObjectDefinition) Implements iormDataObjectRepository.ObjectDefinitions
            Get
                Me.Initialize()
                Dim aList As New List(Of iObjectDefinition)
                For Each anEntry In _DescriptionsByID.Values
                    aList.Add(anEntry)
                Next
                Return aList
            End Get

        End Property
        ''' <summary>
        ''' gets the list of data object providers
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DataObjectProviders As IEnumerable(Of iDataObjectProvider) Implements IDataObjectRepository.DataObjectProviders
            Get
                Return New List(Of iDataObjectProvider) 'none available
            End Get
        End Property
        Public Function GetDataObjectProvider(type As System.Type) As iormDataObjectProvider Implements iormDataObjectRepository.GetDataObjectProvider
            Throw New NotImplementedException("DataObjectProviders are not implemented in ObjectClassRepository")
        End Function
        Public Function GetDataObjectProvider(objectid As String) As iormDataObjectProvider Implements iormDataObjectRepository.GetDataObjectProvider
            Throw New NotImplementedException("DataObjectProviders are not implemented in ObjectClassRepository")
        End Function
        ''' <summary>
        ''' returns a list of ObjectClassDescriptions per module name
        ''' </summary>
        ''' <param name="modulename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectClassDescriptions(modulename As String) As List(Of ObjectClassDescription)
            Me.Initialize()
            If _ClassDescriptorPerModule.ContainsKey(key:=modulename.ToUpper) Then
                Return _ClassDescriptorPerModule.Item(key:=modulename.ToUpper)
            Else
                Return New List(Of ObjectClassDescription)
            End If
        End Function

        ''' <summary>
        ''' returns a list of all Modulenames
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetModulenames() As List(Of String)
            Me.Initialize()
            Return _ClassDescriptorPerModule.Keys.ToList
        End Function
        ''' <summary>
        ''' gets a list of object classes which are using a container for persistence
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectClassesForContainer(containerID As String, Optional onlyenabled As Boolean = True) As List(Of Type)
            Me.Initialize()
            If Not _ContainerAttributesStore.ContainsKey(containerID.ToUpper) Then Return New List(Of Type)
            If onlyenabled AndAlso Not _ContainerAttributesStore.Item(containerID.ToUpper).Enabled Then Return New List(Of Type)

            If _Container2ObjectClassStore.ContainsKey(key:=containerID.ToUpper) Then
                Return _Container2ObjectClassStore.Item(key:=containerID.ToUpper)
            Else
                Return New List(Of Type)
            End If
        End Function

        ''' <summary>
        ''' register a CacheManager at the ObjectClassRepository
        ''' </summary>
        ''' <param name="cache"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RegisterCacheManager(cache As iObjectCacheManager) As Boolean
            AddHandler OnObjectClassDescriptionLoaded, AddressOf cache.OnObjectClassDescriptionLoaded
        End Function
        ''' <summary>
        ''' Initialize the Repository
        ''' </summary>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Initialize(Optional force As Boolean = False) As Boolean

            If IsInitialized Or Not force Then Return True
            Dim aFieldList As System.Reflection.FieldInfo()

            '*** select all the dataobjects implementors
            ''' register all data objects which have a direct orm mapping
            ''' implementation of the interface iormpersistable

            Dim thisAsm As Assembly = Assembly.GetExecutingAssembly()
            Dim adataObjectClassLists As List(Of Type) = thisAsm.GetTypes().Where(Function(t) _
                                                         ((t.GetInterfaces.Contains(GetType(iormDataObject)) AndAlso t.IsClass AndAlso Not t.IsAbstract))).ToList()
            _BootStrapSchemaCheckSum = 0

            '*** go through the classes in the assembly
            '***
            For Each aClass In adataObjectClassLists
                '* add it to _classes
                If _DescriptionsByClass.ContainsKey(aClass.FullName.ToUpper) Then _DescriptionsByClass.Remove(key:=aClass.FullName.ToUpper)
                Dim anewDescription As New ObjectClassDescription(aClass, Me)

                ''' check the class type attributes -> Object attribute first
                '''
                For Each anAttribute As System.Attribute In aClass.GetCustomAttributes(False)
                    ''' Object Attribute
                    ''' 
                    If anAttribute.GetType().Equals(GetType(ormObjectAttribute)) Then
                        Dim anObjectAttribute = DirectCast(anAttribute, ormObjectAttribute)
                        anewDescription.ObjectAttribute = anObjectAttribute
                        '** bootstrapping classes ??
                        If anObjectAttribute.HasValueIsBootstap Then
                            If anObjectAttribute.IsBootstrap Then
                                If Not _BootstrapObjectClasses.Contains(aClass) Then
                                    _BootstrapObjectClasses.Add(aClass)
                                End If
                            End If

                        Else
                            anObjectAttribute.IsBootstrap = False ' default
                        End If
                        ''' remove ObjectID
                        If _DescriptionsByID.ContainsKey(key:=anObjectAttribute.ID) Then
                            _DescriptionsByID.Remove(key:=anObjectAttribute.ID)
                        End If
                        ''' Add both
                        _DescriptionsByID.Add(key:=anObjectAttribute.ID, value:=anewDescription)
                        _DescriptionsByClass.Add(key:=aClass.FullName.ToUpper, value:=anewDescription)
                    End If
                Next

                ''' no object attribute -> description ?!
                If Not _DescriptionsByClass.ContainsKey(key:=aClass.FullName.ToUpper) Then
                    Call CoreMessageHandler(procedure:="ObjectClassRepository.Initialize", _
                                                          message:="WARNING! CLASS '" & aClass.FullName & "' implements IORMDATAOBJECT BUT HAS NO <OrmAttribute> -> DATA OBJECT CLASS NOT STORED In REPOSITORY", _
                                                          messagetype:=otCoreMessageType.InternalWarning, argument:=aClass.FullName)
                End If

                ''' create the InstanceCreator
                ''' 

                Dim aCreator As ObjectClassDescription.CreateInstanceDelegate = _
                    ObjectClassDescription.CreateILGCreateInstanceDelegate(aClass.GetConstructor(Type.EmptyTypes), GetType(ObjectClassDescription.CreateInstanceDelegate))
                If _CreateInstanceDelegateStore.ContainsKey(key:=aClass.FullName.ToUpper) Then
                    _CreateInstanceDelegateStore.Remove(key:=aClass.FullName.ToUpper)
                End If
                _CreateInstanceDelegateStore.Add(key:=aClass.FullName.ToUpper, value:=aCreator)

                ''' get the Fieldlist especially collect the constants
                ''' 

                aFieldList = aClass.GetFields(Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Public Or _
                                              Reflection.BindingFlags.Static Or BindingFlags.FlattenHierarchy)

                '** look into each Const Type (Fields)
                For Each aFieldInfo As System.Reflection.FieldInfo In aFieldList
                    If aFieldInfo.MemberType = Reflection.MemberTypes.Field Then
                        Dim aFieldValue As String = String.Empty
                        Dim aName As String
                        Dim aContainerID As String

                        '** split the container id if static value
                        If aFieldInfo.IsStatic Then
                            If aFieldInfo.GetValue(Nothing) IsNot Nothing Then
                                aFieldValue = aFieldInfo.GetValue(Nothing).ToString.ToUpper
                            End If

                            '* split
                            '* beware a container attribute would be lost
                            Dim names As String() = Shuffle.NameSplitter(aFieldValue)
                            If names.Count > 1 Then
                                aContainerID = names(0)
                                aName = names(1)
                            Else
                                aContainerID = String.Empty
                                aName = aFieldValue
                            End If
                        Else
                            aContainerID = String.Empty
                            aName = String.Empty
                        End If

                        '** Attributes
                        For Each anAttribute As System.Attribute In Attribute.GetCustomAttributes(aFieldInfo)

                            ''' check for containers first to get them all before we process the
                            ''' objects in details
                            If anAttribute.GetType().GetInterfaces.Contains(GetType(iormContainerAttribute)) Then
                                Dim alist As List(Of Type)

                                ''' do we have the same const variable name herited from other classes ?
                                ''' take then only the local / const variable with attributes from the herited class (overwriting)

                                Dim localfield As FieldInfo = aClass.GetField(name:=aFieldInfo.Name, bindingAttr:=Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Public Or _
                                              Reflection.BindingFlags.Static)
                                If localfield Is Nothing OrElse (localfield IsNot Nothing AndAlso aFieldInfo.DeclaringType.Equals(localfield.ReflectedType)) Then

                                    If CType(anAttribute, iormContainerAttribute).HasValueContainerID Then
                                        aContainerID = CType(anAttribute, iormContainerAttribute).ContainerID
                                    Else
                                        aContainerID = aFieldInfo.GetValue(Nothing).ToString.ToUpper
                                        CType(anAttribute, iormContainerAttribute).ContainerID = aContainerID
                                    End If


                                    '** Type Definition
                                    If _Container2ObjectClassStore.ContainsKey(aContainerID) Then
                                        alist = _Container2ObjectClassStore.Item(aContainerID)
                                    Else
                                        alist = New List(Of Type)
                                        _Container2ObjectClassStore.Add(key:=aContainerID, value:=alist)
                                    End If
                                    If Not alist.Contains(item:=aClass) Then
                                        alist.Add(aClass)
                                        ' set also the primary table id of the object with the first
                                        If Not anewDescription.ObjectAttribute.HasValuePrimaryContainerID Then
                                            anewDescription.ObjectAttribute.PrimaryContainerID = aContainerID
                                        End If

                                    End If

                                    '*** Calculate the Checksum from the Tableversions in the Bootstrapclasses
                                    If _BootstrapObjectClasses.Contains(aClass) Then
                                        If Not CType(anAttribute, iormContainerAttribute).HasValueVersion Then
                                            DirectCast(anAttribute, iormContainerAttribute).Version = 1
                                        End If
                                        Dim i = _BootstrapObjectClasses.IndexOf(aClass)
                                        _BootStrapSchemaCheckSum += DirectCast(anAttribute, iormContainerAttribute).Version * Math.Pow(10, i)
                                    End If

                                    '*** add to global tableattribute store
                                    Me.AlterContainerAttribute(anAttribute, fieldinfo:=aFieldInfo)
                                End If

                                '*** Object Attribute
                                ''' check for Object Attributes bound to constants in the class
                                ''' 
                            ElseIf anAttribute.GetType().Equals(GetType(ormObjectAttribute)) Then
                                Dim anObjectAttribute = DirectCast(anAttribute, ormObjectAttribute)
                                If anObjectAttribute.HasValueIsBootstap Then
                                    If anObjectAttribute.IsBootstrap Then
                                        If Not _BootstrapObjectClasses.Contains(aClass) Then
                                            _BootstrapObjectClasses.Add(aClass)
                                        End If
                                    End If
                                Else
                                    anObjectAttribute.IsBootstrap = False ' default
                                End If
                                ''' remove ObjectID
                                If _DescriptionsByID.ContainsKey(key:=anObjectAttribute.ID) Then
                                    Call CoreMessageHandler(procedure:="ObjectClassRepository.Initialize", _
                                                            message:="WARNING! ID '" & anObjectAttribute.ID & "' OF DATA OBJECT CLASS ALREADY STORED In REPOSITORY", _
                                                            messagetype:=otCoreMessageType.InternalWarning, argument:=anObjectAttribute.ClassName)
                                    _DescriptionsByID.Remove(key:=anObjectAttribute.ID)
                                End If
                                ''' Add both
                                _DescriptionsByID.Add(key:=anObjectAttribute.ID, value:=anewDescription)
                                _DescriptionsByClass.Add(key:=aClass.FullName.ToUpper, value:=anewDescription)

                                ''' check object entries register to make reference checks available
                                '''
                            ElseIf anAttribute.GetType().GetInterfaces.Contains(GetType(iormObjectEntryDefinition)) Then
                                Dim anObjectEntryAttribute = DirectCast(anAttribute, iormObjectEntryDefinition)
                                Dim aDescription As ObjectClassDescription = _DescriptionsByClass.Item(key:=aClass.FullName.ToUpper)
                                If aDescription IsNot Nothing Then
                                    ''' save with nothing under the name
                                    ''' -> will be replaced later on
                                    If Not aDescription.SaveObjectEntryAttribute(attribute:=Nothing, entryname:=aName, containerID:=aContainerID, overridesExisting:=True) Then
                                        Call CoreMessageHandler(procedure:="ObjectClassRepository.Initialize", _
                                                               message:="WARNING! OBJECT ENTRY ATTRIBUTE" & aName & "' COULD NOT BE  STORED In CLASS REPOSITORY", _
                                                               messagetype:=otCoreMessageType.InternalWarning, argument:=aClass.FullName)
                                    End If
                                Else
                                    Call CoreMessageHandler(procedure:="ObjectClassRepository.Initialize", _
                                                               message:="WARNING! DESCRIPTION OF CLASS " & aName & "' COULD NOT BE FOUND IN CLASS REPOSITORY", _
                                                               messagetype:=otCoreMessageType.InternalWarning, argument:=aClass.FullName)
                                End If
                            End If

                        Next
                    End If
                Next

                ''' if we donot have a defined container
                ''' 
                If Not anewDescription.ObjectAttribute.HasValuePrimaryContainerID Then
                    Call CoreMessageHandler(procedure:="ObjectClassRepository.Initialize", _
                                                            message:="ERROR! Object with ID '" & anewDescription.ObjectAttribute.ID & "' OF DATA OBJECT CLASS HAS NOT A PRIMARY CONTAINER", _
                                                            messagetype:=otCoreMessageType.InternalError, argument:=anewDescription.ObjectAttribute.ClassName)
                End If
            Next

            '***
            '*** go through all classes
            '*** and get the attributes to look into
            '*** 

            Try
                For Each aClassDescription In _DescriptionsByClass.Values.ToList
                    If aClassDescription.IsInitialized OrElse aClassDescription.Initialize() Then
                        '*** sort per module
                        If aClassDescription.ObjectAttribute.HasValueModulename Then
                            Dim aName As String = aClassDescription.ObjectAttribute.Modulename.ToUpper
                            Dim aList = New List(Of ObjectClassDescription)
                            If Not _ClassDescriptorPerModule.ContainsKey(key:=aName) Then
                                _ClassDescriptorPerModule.Add(key:=aName, value:=aList)
                            Else
                                aList = _ClassDescriptorPerModule.Item(key:=aName)
                            End If
                            aList.Add(aClassDescription)
                        End If
                    Else
                        '** remove from store if initialiazing failed
                        _DescriptionsByClass.Remove(key:=aClassDescription.Name.ToUpper)
                    End If
                Next

                _isInitialized = True
                Return True
            Catch ex As Exception

                Call CoreMessageHandler(procedure:="ObjectClassRepository.Initialize", exception:=ex)

            End Try

        End Function
    End Class

    ''' <summary>
    '''  class to hold per Class the orM Attributes and FieldInfo for Mapping and Relation
    ''' </summary>
    ''' <remarks></remarks>

    Public Class ObjectClassDescription

        ''' <summary>
        ''' Delegates
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Delegate Function CreateInstanceDelegate() As iormDataObject
        Public Delegate Function OperationCallerDelegate(dataobject As Object, parameters As Object()) As Object
        Public Delegate Function MappingGetterDelegate(dataobject As Object) As Object

        ''' <summary>
        ''' internal Store
        ''' </summary>
        ''' <remarks></remarks>
        Private _Type As Type
        Private _ObjectAttribute As ormObjectAttribute

        Private _ContainerAttributes As New Dictionary(Of String, iormContainerAttribute) 'name of table to Attribute

        Private _ObjectEntryAttributes As New Dictionary(Of String, ormObjectEntryAttribute) 'name of object entry to Attribute
        Private _ObjectTransactionAttributes As New Dictionary(Of String, ormObjectTransactionAttribute) 'name of object entry to Attribute
        Private _ObjectOperationAttributes As New Dictionary(Of String, ormObjectOperationMethodAttribute) 'name of object entry to Attribute
        Private _ObjectOperationAttributesByTag As New Dictionary(Of String, ormObjectOperationMethodAttribute) 'name of object entry to Attribute

        Private _OperationCallerDelegates As New Dictionary(Of String, OperationCallerDelegate) ' dictionary of columns to mappings field to getter delegates
        Private _ObjectEntriesPerContainer As New Dictionary(Of String, Dictionary(Of String, ormObjectEntryAttribute)) ' dictionary of tables to dictionary of columns

        Private _ContainerMappings As New Dictionary(Of String, Dictionary(Of String, List(Of FieldInfo))) ' dictionary of container to dictionary of fieldmappings
        Private _ContainerEntryMappings As New Dictionary(Of String, List(Of FieldInfo)) ' dictionary of container entries to mappings
        Private _MappingSetterDelegates As New Dictionary(Of String, Action(Of iormInfusable, Object)) ' dictionary of field to setter delegates
        Private _MappingGetterDelegates As New Dictionary(Of String, MappingGetterDelegate) ' dictionary of columns to mappings field to getter delegates

        Private _ContainerIndices As New Dictionary(Of String, Dictionary(Of String, ormIndexAttribute)) ' dictionary of containers to dictionary of indices
        Private _Indices As New Dictionary(Of String, ormIndexAttribute) ' dictionary of columns to mappings

        Private _ContainerRelationMappings As New Dictionary(Of String, Dictionary(Of String, List(Of FieldInfo))) ' dictionary of tables to dictionary of relation mappings
        Private _RelationEntryMapping As New Dictionary(Of String, List(Of FieldInfo)) ' dictionary of relations to mappings
        Private _ContainerRelations As New Dictionary(Of String, Dictionary(Of String, ormRelationAttribute)) ' dictionary of tables to dictionary of relation
        Private _Relations As New Dictionary(Of String, ormRelationAttribute) ' dictionary of relations 

        Private _DataOperationHooks As New Dictionary(Of String, RuntimeMethodHandle)
        Private _EntryMappings As New Dictionary(Of String, ormObjectEntryMapping)

        Private _ForeignKeys As New Dictionary(Of String, Dictionary(Of String, ormForeignKeyAttribute)) 'dictionary of tables and foreign keys by ids
        Private _QueryAttributes As New Dictionary(Of String, ormObjectQueryAttribute) 'dictionary of queries and definitions

        Private _isInitalized As Boolean = False
        Private _lock As New Object

        '' caches
        Private _cachedMappedContainerEntryNames As List(Of String) = Nothing
        Private _cachedContainerEntryNames As List(Of String) = Nothing
        Private _cachedEntrynames As List(Of String) = Nothing
        Private _cachedQuerynames As List(Of String) = Nothing
        Private _cachedTablenames As List(Of String) = Nothing
        Private _cachedContainerIDs As List(Of String) = Nothing
        Private _cachedRelationNames As List(Of String) = Nothing

        '** backreference
        Private _repository As ObjectClassRepository
        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="class"></param>
        ''' <remarks></remarks>
        Public Sub New([class] As Type, repository As ObjectClassRepository)
            _Type = [class]
            _repository = repository
        End Sub

        ''' <summary>
        ''' Gets or sets the object attribute.
        ''' </summary>
        ''' <value>The object attribute.</value>
        Public Property ObjectAttribute() As ormObjectAttribute
            Get
                Return Me._ObjectAttribute
            End Get
            Set(value As ormObjectAttribute)
                Me._ObjectAttribute = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the primary table ID.
        ''' </summary>
        ''' <value>The primary table ID.</value>
        Public Property PrimaryContainerID() As String
            Get
                Return _ObjectAttribute.PrimaryContainerID
            End Get
            Private Set(value As String)
                _ObjectAttribute.PrimaryContainerID = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the object attribute.
        ''' </summary>
        ''' <value>The object attribute.</value>
        Public ReadOnly Property PrimaryKeyEntryNames() As String()
            Get
                If _ObjectAttribute IsNot Nothing Then Return Me._ObjectAttribute.PrimaryKeyEntryNames
                Return {}
            End Get

        End Property
        ''' <summary>
        ''' returns true if the class description is initialized
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsInitialized As Boolean
            Get
                Return _isInitalized
            End Get
        End Property
        ''' <summary>
        ''' returns the ID of the ObjectClassDescription (the constObjectID)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ID As String
            Get
                If _ObjectAttribute IsNot Nothing Then Return _ObjectAttribute.ID
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the type.
        ''' </summary>
        ''' <value>The type.</value>
        Public Property [Type]() As Type
            Get
                Return Me._Type
            End Get
            Set(value As Type)
                Me._Type = value
            End Set
        End Property

        ''' <summary>
        ''' Name of the Class
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Name As String
            Get
                Return _Type.Name
            End Get
        End Property
        ''' <summary>
        ''' gets a List of all container ids
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ContainerIDs As List(Of String)
            Get
                If _cachedContainerIDs Is Nothing Then
                    Dim theNames As New List(Of String)
                    Dim aList = _ContainerAttributes.Values.Where(Function(x) x.Enabled = True) ' only the enabled
                    If aList IsNot Nothing Then
                        theNames = aList.Select(Function(x) x.ContainerID).ToList ' get the remaining keynames
                    End If
                    _cachedContainerIDs = theNames
                End If
                Return _cachedContainerIDs
            End Get
        End Property

        ''' <summary>
        ''' gets a List of all table names
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Tablenames As List(Of String)
            Get
                If _cachedTablenames Is Nothing Then
                    Dim theNames As New List(Of String)
                    Dim aList As List(Of iormContainerAttribute) = _ContainerAttributes.Values.Where(Function(x) x.Enabled = True AndAlso x.GetType Is GetType(ormTableAttribute)).ToList ' only the enabled
                    If aList IsNot Nothing Then
                        theNames = aList.Select(Function(x) x.ContainerID).ToList ' get the remaining keynames
                    End If
                    _cachedTablenames = theNames
                End If
                Return _cachedTablenames
            End Get
        End Property
        ''' <summary>
        ''' gets a List of all queries
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Querynames As List(Of String)
            Get
                If _cachedQuerynames Is Nothing Then
                    Dim theNames As New List(Of String)
                    Dim aList = _QueryAttributes.Where(Function(x) x.Value.Enabled = True) ' only the enabled
                    If aList IsNot Nothing Then
                        theNames = aList.Select(Function(x) x.Key).ToList ' get the remaining keynames
                    End If
                    _cachedQuerynames = theNames
                End If
                Return _cachedQuerynames
            End Get
        End Property
        ''' <summary>
        ''' gets a List of all object entry names
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Entrynames As List(Of String)
            Get
                If _cachedEntrynames Is Nothing Then
                    Dim aList As New List(Of String)
                    For Each anAttribute In _ObjectEntryAttributes.Values.Where(Function(x) x.Enabled = True)
                        If anAttribute.Enabled Then
                            If anAttribute.HasValueEntryName AndAlso Not aList.Contains(anAttribute.EntryName) Then aList.Add(anAttribute.EntryName)
                        End If
                    Next
                    _cachedEntrynames = aList
                End If
                Return _cachedEntrynames
            End Get
        End Property
        ''' <summary>
        ''' gets a List of all enabled container entry names
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ContainerEntryNames As List(Of String)
            Get
                If _cachedContainerEntryNames Is Nothing Then
                    Dim aList As New List(Of String)
                    For Each perTable In _ObjectEntriesPerContainer
                        If _ContainerAttributes.Item(perTable.Key).Enabled Then
                            Dim entriesperTables = _ObjectEntriesPerContainer.Item(key:=perTable.Key)
                            For Each anEntry In entriesperTables.Values
                                If anEntry.Enabled Then aList.Add(item:=anEntry.ContainerEntryName)
                            Next
                        End If
                    Next
                    _cachedContainerEntryNames = aList
                End If

                Return _cachedContainerEntryNames
            End Get
        End Property
        ''' <summary>
        ''' gets a List of all active object transactions
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TransactionAttributes As List(Of ormObjectTransactionAttribute)
            Get
                Return _ObjectTransactionAttributes.Values.Where(Function(x) x.Enabled = True).ToList ' only the enabled
            End Get
        End Property
        ''' <summary>
        ''' gets a List of all object operations
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property OperationAttributes As List(Of ormObjectOperationMethodAttribute)
            Get
                Return _ObjectOperationAttributes.Values.ToList
            End Get
        End Property
        ''' <summary>
        ''' gets a List of all objectattributes attributes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ObjectEntryAttributes As List(Of ormObjectEntryAttribute)
            Get
                Dim aList As New List(Of ormObjectEntryAttribute)
                For Each anAttribute In _ObjectEntryAttributes.Values.Where(Function(x) x.Enabled = True)
                    _repository.SubstituteReferencedObjectEntry(attribute:=anAttribute)
                    SubstituteDefaultValues(attribute:=anAttribute)
                    aList.Add(anAttribute)
                Next
                Return aList
            End Get
        End Property
        ''' <summary>
        ''' gets a List of all primary key column attributes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property PrimaryEntryAttributes As List(Of ormObjectEntryAttribute)
            Get
                Dim aList As New List(Of ormObjectEntryAttribute)
                For Each anAttribute In _ObjectEntryAttributes.Values.Where(Function(x) x.Enabled = True And Me.PrimaryKeyEntryNames.Contains(x.ContainerEntryName))
                    _repository.SubstituteReferencedObjectEntry(attribute:=anAttribute)
                    SubstituteDefaultValues(attribute:=anAttribute)
                    aList.Add(anAttribute)
                Next
                Return aList
            End Get
        End Property
        ''' <summary>
        ''' gets a List of all mapped Container Entry Names
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property MappedContainerEntryNames As List(Of String)
            Get
                If _cachedMappedContainerEntryNames Is Nothing Then
                    Dim aList As New List(Of String)
                    For Each aContainerAttribute In _ContainerAttributes.Values.Where(Function(x) x.Enabled = True)
                        Dim theColumns = _ContainerAttributes.Item(key:=aContainerAttribute.ContainerID).Entries.Where(Function(x) x.Enabled = True).Select(Function(x) x.EntryName)
                        Dim aDir As Dictionary(Of String, List(Of FieldInfo)) = _ContainerMappings.Item(key:=aContainerAttribute.ContainerID)
                        For Each anEntryName In aDir.Keys
                            If theColumns.Contains(anEntryName) Then aList.Add(item:=aContainerAttribute.ContainerID & "." & anEntryName)
                        Next
                    Next
                    _cachedMappedContainerEntryNames = aList
                End If

                Return _cachedMappedContainerEntryNames
            End Get
        End Property
        ''' <summary>
        ''' gets a List of all active relation names
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property RelationNames As List(Of String)
            Get
                If _cachedRelationNames Is Nothing Then
                    Dim aList As New List(Of String)
                    For Each aRelation In _Relations.Values.Where(Function(x) x.Enabled = True)
                        Dim names As String() = Shuffle.NameSplitter(aRelation.Name)
                        Dim aName As String
                        If names.Count > 1 Then
                            aName = names(1)
                        Else
                            aName = names(0)
                        End If

                        If Not aList.Contains(aName) Then aList.Add(aName)
                    Next
                    _cachedRelationNames = aList
                End If

                Return _cachedRelationNames
            End Get
        End Property
        ''' <summary>
        ''' gets a List of all index attributes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IndexAttributes As List(Of ormIndexAttribute)
            Get
                Dim aList As New List(Of ormIndexAttribute)
                For Each aTablename In _ObjectEntriesPerContainer.Keys
                    If _ContainerAttributes.ContainsKey(aTablename) AndAlso _ContainerAttributes.Item(aTablename).Enabled Then
                        Dim aList2 As List(Of ormIndexAttribute) = _ContainerIndices.Item(key:=aTablename).Values.Where(Function(x) x.Enabled = True).ToList
                        aList.AddRange(aList2)
                    End If
                Next
                Return aList
            End Get
        End Property
        ''' <summary>
        ''' gets a List of all relation Attributes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property RelationAttributes As List(Of ormRelationAttribute)
            Get
                Return _Relations.Values.Where(Function(x) x.Enabled = True).ToList
            End Get
        End Property
        ''' <summary>
        ''' gets a List of all container Attributes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ContainerAttributes As List(Of iormContainerAttribute)
            Get
                Return _ContainerAttributes.Values.Where(Function(x) x.Enabled = True).ToList
            End Get
        End Property
        ''' <summary>
        ''' gets a List of all table Attributes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TableAttributes As List(Of ormTableAttribute)
            Get
                Dim aList As New List(Of ormTableAttribute)
                For Each anTableAttribute In _ContainerAttributes.Values.Where(Function(x) x.Enabled = True)
                    If anTableAttribute.GetType Is GetType(ormTableAttribute) Then
                        aList.Add(TryCast(anTableAttribute, ormTableAttribute))
                    End If
                Next
                Return aList
            End Get
        End Property

        ''' <summary>
        ''' returns the SchemaTableAttribute for a table name
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetContainerAttribute(containerID As String, Optional OnlyEnabled As Boolean = True) As iormContainerAttribute
            If _ContainerAttributes.ContainsKey(key:=containerID) Then
                Dim anAttribute = _ContainerAttributes.Item(containerID)
                If OnlyEnabled Then
                    If anAttribute.Enabled Then
                        Return anAttribute
                    Else
                        Return Nothing
                    End If
                Else
                    Return anAttribute
                End If

            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' returns the SchemaTableAttribute for a table name
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTableAttribute(tableid As String, Optional OnlyEnabled As Boolean = True) As ormTableAttribute
            If _ContainerAttributes.ContainsKey(key:=tableid) Then
                Dim anAttribute As ormTableAttribute = TryCast(_ContainerAttributes.Item(tableid), ormTableAttribute)
                If OnlyEnabled Then
                    If anAttribute.Enabled Then
                        Return anAttribute
                    Else
                        Return Nothing
                    End If
                Else
                    Return anAttribute
                End If

            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' returns a MethodInfo for Dataoperation Hooks
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMethodInfoHook(name As String) As RuntimeMethodHandle
            If _DataOperationHooks.ContainsKey(key:=name.ToUpper) Then
                Return _DataOperationHooks.Item(key:=name.ToUpper)
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' ToString Function
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Return Me.Name
        End Function

        ''' <summary>
        ''' returns the object transaction attribute 
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectTransactionAttribute(name As String, Optional onlyEnabled As Boolean = True) As ormObjectTransactionAttribute
            Dim anEntryname As String = String.Empty
            Dim anObjectname As String = String.Empty
            Dim names() As String = Shuffle.NameSplitter(name)

            '** split the names
            If names.Count > 1 Then
                anObjectname = names(0)
                If anObjectname <> _ObjectAttribute.ID Then
                    'CoreMessageHandler(message:="object name of Object is not equal with entry name", arg1:=anObjectname, entryname:=name, _
                    '                   subname:="ObjectClassDescription.GetObjecTransactionAttribute", messagetype:=otCoreMessageType.InternalWarning)
                    anEntryname = name.ToUpper
                    anObjectname = _ObjectAttribute.ID
                Else
                    anEntryname = names(1)
                End If

            Else
                anEntryname = name.ToUpper
            End If

            '** return

            If _ObjectTransactionAttributes.ContainsKey(key:=anEntryname) Then
                Dim anAttribute As ormObjectTransactionAttribute = _ObjectTransactionAttributes.Item(key:=anEntryname)
                If onlyEnabled Then
                    If anAttribute.Enabled Then
                        Return anAttribute
                    Else
                        Return Nothing
                    End If
                Else
                    Return anAttribute
                End If

            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' returns the object operation attribute 
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectOperationAttributeByTag(tag As String) As ormObjectOperationMethodAttribute
            '** return
            If _ObjectOperationAttributesByTag.ContainsKey(key:=tag) Then
                Dim anAttribute As ormObjectOperationMethodAttribute = _ObjectOperationAttributesByTag.Item(key:=tag)
                Return anAttribute
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' returns the object operation attribute 
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectOperationAttribute(name As String) As ormObjectOperationMethodAttribute
            Dim anEntryname As String = String.Empty
            Dim anObjectname As String = String.Empty
            Dim names() As String = Shuffle.NameSplitter(name)

            '** split the names
            If names.Count > 1 Then
                anObjectname = names(0)
                If anObjectname <> _ObjectAttribute.ID Then
                    'CoreMessageHandler(message:="object name of Object is not equal with entry name", arg1:=anObjectname, entryname:=name, _
                    '                   subname:="ObjectClassDescription.GetObjecTransactionAttribute", messagetype:=otCoreMessageType.InternalWarning)
                    anEntryname = name.ToUpper
                    anObjectname = _ObjectAttribute.ID
                Else
                    anEntryname = names(1)

                End If
            Else
                anEntryname = name.ToUpper
            End If

            '** return

            If _ObjectOperationAttributes.ContainsKey(key:=anEntryname) Then
                Dim anAttribute As ormObjectOperationMethodAttribute = _ObjectOperationAttributes.Item(key:=anEntryname)
                Return anAttribute
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' substitute the default values for object entry attributes
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function SubstituteDefaultValues(attribute As ormObjectEntryAttribute) As Boolean

            ''' check if we have a value otherwise take these as default
            If Not attribute.HasValueIsReadonly Then attribute.IsReadOnly = False
            If Not attribute.HasValueIsNullable Then attribute.IsNullable = False
            If Not attribute.HasValueIsUnique Then attribute.IsUnique = False
            If Not attribute.HasValueIsActive Then attribute.IsActive = True

            Return True
        End Function
        ''' <summary>
        ''' returns True if the ObjectEntry exists
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function HasObjectEntryAttribute(entryname As String, Optional onlyenabled As Boolean = True) As Boolean
            Dim anEntryname As String = String.Empty
            Dim anObjectname As String = String.Empty
            Dim names() As String = Shuffle.NameSplitter(entryname)

            '** split the names
            If names.Count > 1 Then
                anObjectname = names(0)
                If anObjectname <> _ObjectAttribute.ID Then
                    'CoreMessageHandler(message:="object name of Object is not equal with entry name", arg1:=anObjectname, entryname:=entryname, _
                    '                   subname:="ObjectClassDescription.HasObjectEntryAttribute", messagetype:=otCoreMessageType.InternalWarning)
                    anEntryname = entryname.ToUpper
                    anObjectname = _ObjectAttribute.ID
                Else
                    anEntryname = names(1)
                End If

            Else
                anEntryname = entryname.ToUpper
            End If

            '** return

            Return _ObjectEntryAttributes.ContainsKey(key:=anEntryname)
        End Function
        ''' <summary>
        ''' returns the ormObjectEntryAttribute for a given entryname
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetObjectEntryAttribute(entryname As String, Optional onlyenabled As Boolean = True) As ormObjectEntryAttribute
            Dim anEntryname As String = String.Empty
            Dim anObjectname As String = String.Empty
            Dim names() As String = Shuffle.NameSplitter(entryname)

            '** split the names
            If names.Count > 1 Then
                anObjectname = names(0)
                If anObjectname <> _ObjectAttribute.ID Then
                    CoreMessageHandler(message:="object name of Object is not equal with entry name", argument:=anObjectname, entryname:=entryname, _
                                       procedure:="ObjectClassDescription.GetObjectEntryAttribute", messagetype:=otCoreMessageType.InternalWarning)
                    anEntryname = entryname.ToUpper
                    anObjectname = _ObjectAttribute.ID
                Else
                    anEntryname = names(1)
                End If

            Else
                anEntryname = entryname.ToUpper
            End If

            '** return

            If _ObjectEntryAttributes.ContainsKey(key:=anEntryname) Then
                Dim anAttribute As ormObjectEntryAttribute = _ObjectEntryAttributes.Item(key:=anEntryname)
                If anAttribute Is Nothing OrElse (onlyenabled AndAlso Not anAttribute.Enabled) Then Return Nothing

                '' substitute entries
                _repository.SubstituteReferencedObjectEntry(attribute:=anAttribute)
                '' set default values on non-set 
                Me.SubstituteDefaultValues(attribute:=anAttribute)
                ''return final
                Return anAttribute
            Else
                Return Nothing
            End If

        End Function
        ''' <summary>
        ''' returns a relation attribute by name (tablename is obsolete)
        ''' </summary>
        ''' <param name="relationname"></param>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetRelationAttribute(relationname As String, Optional onlyenabled As Boolean = False) As ormRelationAttribute
            Dim aRelationName As String = String.Empty
            Dim names() As String = Shuffle.NameSplitter(relationname)

            '** split the names
            If names.Count > 1 Then
                aRelationName = names(1)
            Else
                aRelationName = relationname.ToUpper
                'If _TableAttributes.Count > 1 Then
                '    CoreMessageHandler(message:="more than one tables in the description but no table name specified in the relation name or as argument", _
                '                        messagetype:=otCoreMessageType.InternalWarning, subname:="ObjectClassDescription.GetRelationAttribute", _
                '                        arg1:=relationname)
                'End If
            End If

            '** return
            If _Relations.ContainsKey(key:=aRelationName) Then
                Dim anattribute As ormRelationAttribute = _Relations.Item(key:=aRelationName)
                If onlyenabled AndAlso Not anattribute.Enabled Then Return Nothing
                Return anattribute
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' returns a relation attribute by name (tablename is obsolete)
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetQueryAttribute(name As String, Optional onlyenabled As Boolean = True) As ormObjectQueryAttribute
            Dim aQueryname As String = String.Empty
            Dim names() As String = Shuffle.NameSplitter(name)

            '** split the names
            If names.Count > 1 Then
                aQueryname = names(1)
            Else
                aQueryname = name.ToUpper
            End If

            '** return
            If _QueryAttributes.ContainsKey(key:=aQueryname) Then
                Dim anattribute As ormObjectQueryAttribute = _QueryAttributes.Item(key:=name.ToUpper)
                If onlyenabled AndAlso Not anattribute.Enabled Then Return Nothing
                Return anattribute
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' gets a List of all index attributes for a tablename
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIndexAttributes(containerID As String, Optional onlyenabled As Boolean = True) As List(Of ormIndexAttribute)
            If Not onlyenabled Then Return _ContainerIndices.Item(key:=containerID).Values.ToList
            Return _ContainerIndices.Item(key:=containerID).Values.Where(Function(x) x.Enabled = True).ToList
        End Function
        ''' <summary>
        ''' gets the mapping attribute for a member name (of class)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEntryMappingAttributes(membername As String, Optional onlyenabled As Boolean = True) As ormObjectEntryMapping
            If _EntryMappings.ContainsKey(key:=membername) Then
                Dim anAttribute As ormObjectEntryMapping = _EntryMappings.Item(key:=membername)
                If onlyenabled AndAlso Not anAttribute.Enabled Then Return Nothing
                Return anAttribute
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' gets the setter delegate for the member field
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFieldMemberSetterDelegate(membername As String) As Action(Of ormBusinessObject, Object)
            If _MappingSetterDelegates.ContainsKey(membername) Then
                Return _MappingSetterDelegates.Item(key:=membername)
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' gets the getter delegate for the member field
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFieldMemberGetterDelegate(membername As String) As MappingGetterDelegate
            If _MappingGetterDelegates.ContainsKey(membername) Then
                Return _MappingGetterDelegates.Item(key:=membername)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' retrieves the Operation Caller Delegate for an operation
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetOperartionCallerDelegate(operationname As String) As OperationCallerDelegate
            If _OperationCallerDelegates.ContainsKey(operationname.ToUpper) Then
                Return _OperationCallerDelegates.Item(key:=operationname.ToUpper)
            Else
                Return Nothing
            End If
        End Function
        ''' <summary>
        ''' returns the mapped FieldInfos for a given columnname and tablename
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMappedContainerEntry2FieldInfos(containerEntryName As String, _
                                                  Optional containerID As String = Nothing, _
                                                  Optional onlyenabled As Boolean = True) As List(Of FieldInfo)
            Dim aContainerEntryName As String = String.Empty
            Dim aContainerID As String = String.Empty
            If containerEntryName Is Nothing Then
                CoreMessageHandler(message:="function called with nothing as container entry name ", procedure:="ObjectClassDescription.GetMappedColumnFieldInfos", argument:=Me.ObjectAttribute.ID, _
                                   messagetype:=otCoreMessageType.InternalError)
                Return New List(Of FieldInfo)
            End If
            Dim names() As String = Shuffle.NameSplitter(containerEntryName)

            '** split the names
            If names.Count > 1 Then
                If String.IsNullOrWhiteSpace(containerID) Then
                    aContainerID = names(0)
                Else
                    aContainerID = containerID.ToUpper
                End If
                aContainerEntryName = names(1)
            Else
                aContainerEntryName = containerEntryName.ToUpper
                aContainerID = Me.PrimaryContainerID
                If _ContainerAttributes.Count > 1 Then
                    CoreMessageHandler(message:="more than one tables in the description but no table name specified in the column name or as argument", _
                                       messagetype:=otCoreMessageType.InternalWarning, procedure:="ObjectClassDescription.GetMappedColumnFieldInfos", _
                                       argument:=containerEntryName)
                End If
            End If

            '** return
            If _ContainerMappings.ContainsKey(key:=aContainerID) Then
                ''' check on the enabled table
                If onlyenabled Then
                    If Not _ContainerAttributes.ContainsKey(aContainerID) OrElse Not _ContainerAttributes.Item(key:=aContainerID).Enabled Then
                        Return New List(Of FieldInfo)
                    End If

                End If
                If _ContainerMappings.Item(key:=aContainerID).ContainsKey(key:=aContainerEntryName) Then

                    Return _ContainerMappings.Item(key:=aContainerID).Item(key:=aContainerEntryName)
                Else
                    Return New List(Of FieldInfo)
                End If
            Else
                Return New List(Of FieldInfo)
            End If

        End Function
        ''' <summary>
        ''' returns the mapped FieldInfos for a given entryname
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEntryFieldInfos(entryname As String, Optional onlyenabled As Boolean = True) As List(Of FieldInfo)
            Dim anObjectEntry = Me.GetObjectEntryAttribute(entryname:=entryname)
            If anObjectEntry Is Nothing OrElse (onlyenabled AndAlso Not anObjectEntry.Enabled) Then
                Return New List(Of FieldInfo)
            End If

            Dim aFieldname As String = anObjectEntry.ContainerEntryName
            Dim aTablename As String = anObjectEntry.ContainerID


            '** return
            If _ContainerMappings.ContainsKey(key:=aTablename) Then
                If _ContainerMappings.Item(key:=aTablename).ContainsKey(key:=aFieldname) Then
                    Return _ContainerMappings.Item(key:=aTablename).Item(key:=aFieldname)
                Else
                    Return New List(Of FieldInfo)
                End If
            Else
                Return New List(Of FieldInfo)
            End If

        End Function
        ''' <summary>
        ''' returns the FieldInfos for a given relation and tablename
        ''' </summary>
        ''' <param name="columnname"></param>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMappedRelation2FieldInfos(relationName As String, _
                                                    Optional tableid As String = Nothing, _
                                                    Optional onlyenabled As Boolean = True) As List(Of FieldInfo)
            Dim aRelationName As String = String.Empty
            Dim atableid As String = String.Empty
            Dim names() As String = Shuffle.NameSplitter(relationName)

            '** split the names
            If names.Count > 1 Then
                If String.IsNullOrWhiteSpace(tableid) Then
                    atableid = names(0)
                Else
                    atableid = tableid.ToUpper
                End If
                aRelationName = names(1)
            Else
                aRelationName = relationName.ToUpper
                atableid = Me.PrimaryContainerID
                If _ContainerAttributes.Count > 1 Then
                    CoreMessageHandler(message:="more than one tables in the description but no table name specified in the column name or as argument", _
                                       messagetype:=otCoreMessageType.InternalWarning, procedure:="ObjectClassDescription.GetMappedRelationFieldInfos", _
                                       argument:=relationName)
                End If
            End If


            '** return
            If _ContainerRelationMappings.ContainsKey(key:=atableid) Then
                If _ContainerRelationMappings.Item(key:=atableid).ContainsKey(key:=aRelationName) Then
                    Return _ContainerRelationMappings.Item(key:=atableid).Item(key:=aRelationName)
                Else
                    Return New List(Of FieldInfo)
                End If
            Else
                Return New List(Of FieldInfo)
            End If

        End Function

        ''' <summary>
        ''' gets a List of all column names for a given Table name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetColumnNames(tableid As String, Optional onlyenabled As Boolean = True) As IList(Of String)
            If onlyenabled Then
                '' check the table and the object entries per table
                If Not _ContainerAttributes.ContainsKey(tableid.ToUpper) OrElse Not _ContainerAttributes.Item(tableid.ToUpper).Enabled _
                   OrElse Not _ObjectEntriesPerContainer.ContainsKey(key:=tableid.ToUpper) Then
                    Return New List(Of String)
                End If

                Return _ObjectEntriesPerContainer.Item(tableid.ToUpper).Values.Where(Function(x) x.Enabled = True)

            ElseIf _ObjectEntriesPerContainer.ContainsKey(key:=tableid.ToUpper) Then

                Return _ObjectEntriesPerContainer.Item(key:=tableid.ToUpper).Keys.ToList
            End If

            Return New List(Of String)
        End Function
        ''' <summary>
        ''' intializie a embedded container
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="containerID"></param>
        ''' <param name="overridesExisting"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InitializeEmbeddedContainerAttribute(attribute As Attribute, containerID As String, overridesExisting As Boolean) As Boolean
            Dim aContainerAttribute As ormEmbeddedContainerAttribute = TryCast(attribute, ormEmbeddedContainerAttribute)
            Try

                If aContainerAttribute IsNot Nothing AndAlso _
                    InitializeContainerAttribute(attribute:=attribute, containerID:=containerID, overridesExisting:=overridesExisting) Then
                    If Not aContainerAttribute.HasValueEmbeddedIn Then aContainerAttribute.EmbeddedIn = containerID
                    '*** check REFERENCE OBJECT ENTRY
                    If aContainerAttribute.HasValueEmbeddedIn Then
                        Dim refObjectName As String = String.Empty
                        Dim refObjectEntry As String = String.Empty
                        Dim names = Shuffle.NameSplitter(aContainerAttribute.EmbeddedIn)
                        If names.Count > 1 Then
                            refObjectName = names(0)
                            refObjectEntry = names(1)
                        Else
                            refObjectEntry = aContainerAttribute.EmbeddedIn
                            refObjectName = Me.ObjectAttribute.ID
                        End If

                        ' will not take if embedded object entry does not exist
                        If Not _repository.HasObjectEntryAttribute(entryname:=refObjectEntry, objectname:=refObjectName) Then
                            CoreMessageHandler(message:="reference to object entry could not be resolved", procedure:="ObjectClassDescription.InitializeEmbeddedContainerAttribute", messagetype:=otCoreMessageType.InternalError, containerID:=containerID)
                            Return False
                        End If


                        ''' initialize the reference -> the keys of the reference must also become keys of the Embedded Container
                        ''' 
                        Dim aReferenceDescription As ObjectClassDescription = _repository.GetObjectClassDescriptionByID(id:=refObjectName)
                        If aReferenceDescription IsNot Nothing AndAlso (aReferenceDescription.IsInitialized OrElse aReferenceDescription.Initialize) Then
                            ''' not implemented
                            ''' 

                        End If
                    End If
                End If

                Return True

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectClassdescription.InitializeEmbeddedContainerAttribute", argument:=containerID, messagetype:=otCoreMessageType.InternalException)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' initialize a container attribute to the Description
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InitializeContainerAttribute(attribute As Attribute, containerID As String, overridesExisting As Boolean) As Boolean
            Dim aContainerAttribute As iormContainerAttribute = DirectCast(attribute, iormContainerAttribute)
            Try

                '** Tables
                If _ContainerAttributes.ContainsKey(key:=containerID) And overridesExisting Then
                    _ContainerAttributes.Remove(key:=containerID)
                ElseIf _ContainerAttributes.ContainsKey(key:=containerID) And Not overridesExisting Then
                    Return True '* do nothing since we have a ClassOverrides tableattribute
                End If

                '** default values
                With aContainerAttribute
                    If Not .HasValueID Then .ID = containerID.ToUpper
                    '** make sure that the tableid is the value provided in the tableid - the const value of the fieldinfo
                    '** otherwise the automatic setting of the default primary table will not work -> see initialize of ObjectClassRegistery
                    If Not .HasValueContainerID Then .ContainerID = containerID.ToUpper
                    '** version
                    If Not .HasValueVersion Then .Version = 1
                    '** set the link
                    If _ObjectAttribute IsNot Nothing Then .ID = _ObjectAttribute.ID
                    If Not .HasValuePrimaryKey Then .PrimaryKey = ot.ConstDefaultPrimaryKeyname
                    If Not .HasValuePrimaryDatabaseDriverID Then .PrimaryDatabaseDriverID = ot.ConstDefaultPrimaryDBDriver
                End With



                '** check the container attribute from global store
                '** merge the values there
                '** table name must be set
                _repository.AlterContainerAttribute(aContainerAttribute)


                '** add it
                _ContainerAttributes.Add(key:=aContainerAttribute.ContainerID, value:=aContainerAttribute)
                '** to the object attributes
                If _ObjectAttribute.ContainerIDs Is Nothing OrElse _ObjectAttribute.ContainerIDs.Count = 0 Then
                    _ObjectAttribute.ContainerIDs = {aContainerAttribute.ContainerID}
                    If Not _ObjectAttribute.HasValuePrimaryContainerID Then _ObjectAttribute.PrimaryContainerID = aContainerAttribute.ContainerID
                Else
                    ReDim Preserve _ObjectAttribute.ContainerIDs(_ObjectAttribute.ContainerIDs.GetUpperBound(0) + 1)
                    _ObjectAttribute.ContainerIDs(_ObjectAttribute.ContainerIDs.GetUpperBound(0)) = aContainerAttribute.ContainerID
                End If

                '** Add Columns per Container
                If _ObjectEntriesPerContainer.ContainsKey(key:=aContainerAttribute.ContainerID) Then _ObjectEntriesPerContainer.Remove(key:=aContainerAttribute.ContainerID)
                _ObjectEntriesPerContainer.Add(key:=aContainerAttribute.ContainerID, value:=New Dictionary(Of String, ormObjectEntryAttribute))
                '** Mappings per Container
                If _ContainerMappings.ContainsKey(key:=aContainerAttribute.ContainerID) Then _ContainerMappings.Remove(key:=aContainerAttribute.ContainerID)
                _ContainerMappings.Add(key:=aContainerAttribute.ContainerID, value:=New Dictionary(Of String, List(Of FieldInfo)))
                '** Indices per Container
                If _ContainerIndices.ContainsKey(key:=aContainerAttribute.ContainerID) Then _ContainerIndices.Remove(key:=aContainerAttribute.ContainerID)
                _ContainerIndices.Add(key:=aContainerAttribute.ContainerID, value:=New Dictionary(Of String, ormIndexAttribute))
                '** Relations per Container
                If _ContainerRelationMappings.ContainsKey(key:=aContainerAttribute.ContainerID) Then _ContainerRelationMappings.Remove(key:=aContainerAttribute.ContainerID)
                _ContainerRelationMappings.Add(key:=aContainerAttribute.ContainerID, value:=New Dictionary(Of String, List(Of FieldInfo)))
                '** Relations per Container
                If _ContainerRelations.ContainsKey(key:=aContainerAttribute.ContainerID) Then _ContainerRelations.Remove(key:=aContainerAttribute.ContainerID)
                _ContainerRelations.Add(key:=aContainerAttribute.ContainerID, value:=New Dictionary(Of String, ormRelationAttribute))

                Return True
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectClassDescription.InitializeContainerAttribute")
                Return False
            End Try

        End Function
        ''' <summary>
        ''' initialize a table attribute to the Description
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InitializeQueryAttribute(attribute As Attribute, queryname As String, value As String, overridesExisting As Boolean) As Boolean
            Dim aQueryAttribute As ormObjectQueryAttribute = DirectCast(attribute, ormObjectQueryAttribute)
            Try
                If String.IsNullOrWhiteSpace(queryname) Then queryname = value.ToUpper

                '** Tables
                If _QueryAttributes.ContainsKey(key:=queryname) And overridesExisting Then
                    _QueryAttributes.Remove(key:=queryname)
                ElseIf _QueryAttributes.ContainsKey(key:=queryname) And Not overridesExisting Then
                    Return True '* do nothing since we have a ClassOverrides attribute
                End If


                '** default values
                With aQueryAttribute
                    .ID = queryname.ToUpper
                    ' Entry names
                    If Not .HasValueEntrynames Then .AddAllFields = True
                    '** version
                    If Not .HasValueVersion Then .Version = 1
                End With

                '** add it
                _QueryAttributes.Add(key:=aQueryAttribute.ID, value:=aQueryAttribute)

                Return True
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectClassDescription.InitializeQueryAttribute")
                Return False
            End Try

        End Function

        ''' <summary>
        ''' save the object entry attribute in the global and local stores
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="name"></param>
        ''' <param name="containerID"></param>
        ''' <param name="fieldvalue"></param>
        ''' <param name="overridesExisting"></param>
        ''' <returns>True if saved successful</returns>
        ''' <remarks></remarks>
        Public Function SaveObjectEntryAttribute(attribute As ormObjectEntryAttribute, entryname As String, containerID As String, overridesExisting As Boolean) As Boolean
            Dim anObjectEntryName As String = String.Empty
            Dim globalContainerAttributes As iormContainerAttribute
            Dim result As Boolean = True

            If Not entryname.Contains(".") AndAlso Not entryname.Contains(ConstDelimiter) Then
                anObjectEntryName = _ObjectAttribute.ID.ToUpper & "." & entryname.ToUpper
            End If

            '* save local in the object class description
            If Not _ObjectEntryAttributes.ContainsKey(key:=entryname) Then
                _ObjectEntryAttributes.Add(key:=entryname, value:=attribute)
            ElseIf Not overridesExisting Then
                Dim anAttribute As Attribute = _ObjectEntryAttributes.Item(entryname)
                If anAttribute Is Nothing Then
                    _ObjectEntryAttributes.Remove(key:=entryname) ' if not enabled still please remove the entry
                    _ObjectEntryAttributes.Add(key:=entryname, value:=attribute)
                    result = True
                Else
                    result = False
                End If
            ElseIf overridesExisting Then
                _ObjectEntryAttributes.Remove(key:=entryname) ' if not enabled still please remove the entry
                _ObjectEntryAttributes.Add(key:=entryname, value:=attribute)
            End If


            '** save in object description per Container as well as in global Container Store
            '** of the repository
            If Not String.IsNullOrEmpty(containerID) AndAlso _ObjectEntriesPerContainer.ContainsKey(key:=containerID) Then
                Dim aDictionary As Dictionary(Of String, ormObjectEntryAttribute) = _ObjectEntriesPerContainer.Item(key:=containerID)
                If aDictionary IsNot Nothing Then
                    If Not aDictionary.ContainsKey(key:=anObjectEntryName) Then
                        aDictionary.Add(key:=anObjectEntryName, value:=attribute)
                        globalContainerAttributes = _repository.GetContainerAttribute(containerID)
                        If globalContainerAttributes IsNot Nothing Then

                            If Not globalContainerAttributes.HasEntry(attribute.ContainerEntryName) Then
                                globalContainerAttributes.AddEntry(attribute)
                            Else
                                globalContainerAttributes.UpdateEntry(attribute)
                            End If
                        Else
                            CoreMessageHandler(message:="container attribute was not defined in global container attribute store", argument:=entryname, messagetype:=otCoreMessageType.InternalError, _
                                               procedure:="ObjectClassDescription.SaveObjectEntryAttribute", containerID:=containerID, objectname:=_Type.Name)
                            Return False

                        End If
                    ElseIf Not overridesExisting Then
                        Dim anAttribute As Attribute = aDictionary.Item(anObjectEntryName)
                        If anAttribute Is Nothing Then
                            '*** override
                            aDictionary.Remove(key:=anObjectEntryName) '* through out
                            aDictionary.Add(key:=anObjectEntryName, value:=attribute) '* add new
                            globalContainerAttributes = _repository.GetContainerAttribute(containerID)
                            If globalContainerAttributes IsNot Nothing Then
                                If globalContainerAttributes.GetEntry(attribute.ContainerEntryName) Is Nothing Then
                                    globalContainerAttributes.AddEntry(attribute)
                                End If
                            Else
                                CoreMessageHandler(message:="container attribute was not defined in global container attribute store", argument:=entryname, messagetype:=otCoreMessageType.InternalError, _
                                                   procedure:="ObjectClassDescription.SaveObjectEntryAttribute", containerID:=containerID, objectname:=_Type.Name)
                                Return False
                            End If

                        Else
                            'Return  later
                            result = False
                        End If

                    ElseIf overridesExisting Then
                        '*** override
                        aDictionary.Remove(key:=anObjectEntryName) '* through out
                        aDictionary.Add(key:=anObjectEntryName, value:=attribute) '* add new
                        globalContainerAttributes = _repository.GetContainerAttribute(containerID)
                        If globalContainerAttributes IsNot Nothing Then
                            If globalContainerAttributes.GetEntry(attribute.ContainerEntryName) Is Nothing Then
                                globalContainerAttributes.AddEntry(attribute)
                            End If
                        Else
                            CoreMessageHandler(message:="container attribute was not defined in global container attribute store", argument:=entryname, messagetype:=otCoreMessageType.InternalError, _
                                               procedure:="ObjectClassDescription.SaveObjectEntryAttribute", containerID:=containerID, objectname:=_Type.Name)
                            Return False

                        End If
                    Else
                        CoreMessageHandler(message:="object entry exists in container more than once", argument:=entryname, messagetype:=otCoreMessageType.InternalError, _
                                           procedure:="ObjectClassDescription.SaveObjectEntryAttribute", containerID:=containerID, objectname:=_Type.Name)
                        Return False
                    End If

                Else
                    CoreMessageHandler(message:="containerID does not exist in container store of ObjectClassRepository", argument:=containerID, messagetype:=otCoreMessageType.InternalError, _
                                       procedure:="ObjectClassDescription.SaveObjectEntryAttribute", objectname:=_Type.Name)
                    Return False
                End If
            End If

            Return result
        End Function

        ''' <summary>
        ''' Initialize a ObjectEntry Attribute
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="name"></param>
        ''' <param name="tablename"></param>
        ''' <param name="fieldvalue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InitializeObjectEntryAttribute(attribute As Attribute, entryname As String, containerID As String, fieldvalue As String, overridesExisting As Boolean) As Boolean
            Dim anObjectEntryName As String = String.Empty
            Dim globalContainerAttributes As iormContainerAttribute

            Try

                '* set the column name
                Dim anObjectEntryAttribute As ormObjectEntryAttribute = TryCast(attribute, ormObjectEntryAttribute)

                If String.IsNullOrWhiteSpace(entryname) Then entryname = fieldvalue
                '** default
                If String.IsNullOrWhiteSpace(containerID) Then
                    If Not anObjectEntryAttribute.HasValueContainerID Then
                        If _ContainerAttributes.Count = 0 Then
                            CoreMessageHandler(message:="Object Entry Attribute was not assigned to a table - no tables seem to be defined in the class", _
                                               argument:=_Type.Name, entryname:=fieldvalue, messagetype:=otCoreMessageType.InternalError, _
                                               procedure:="ObjectClassDescription.InitializeObjectEntryAttribute", objectname:=_Type.Name)
                            Return False
                        End If
                        containerID = Me.PrimaryContainerID
                    Else
                        containerID = anObjectEntryAttribute.ContainerID
                    End If
                End If
                ' retrieve the global container Attributes
                globalContainerAttributes = _repository.GetContainerAttribute(containerID:=containerID)
                ' reset the attributes 
                If Not anObjectEntryAttribute.HasValueID Then anObjectEntryAttribute.ID = entryname.ToUpper
                If Not anObjectEntryAttribute.HasValueContainerEntryName Then anObjectEntryAttribute.ContainerEntryName = entryname.ToUpper
                If Not anObjectEntryAttribute.HasValueContainerID Then anObjectEntryAttribute.ContainerID = containerID.ToUpper
                If Not anObjectEntryAttribute.HasValueObjectName Then anObjectEntryAttribute.ObjectName = _ObjectAttribute.ID.ToUpper
                If Not anObjectEntryAttribute.HasValueEntryName Then anObjectEntryAttribute.EntryName = entryname.ToUpper
                If Not anObjectEntryAttribute.HasValueVersion Then anObjectEntryAttribute.Version = 1

                ' if we set an default value here - we cannot reference anymore :-(
                ' only possible for values which cannot be referenced !!
                ' put it in substitutedefaultvalues routine
                'If Not anObjectEntryAttribute.HasValueIsReadonly Then anObjectEntryAttribute.IsReadOnly = False
                'If Not anObjectEntryAttribute.HasValueIsNullable Then anObjectEntryAttribute.IsNullable = False
                'If Not anObjectEntryAttribute.HasValueIsUnique Then anObjectEntryAttribute.IsUnique = False

                If Not anObjectEntryAttribute.hasValuePosOrdinal Then
                    anObjectEntryAttribute.Posordinal = _ObjectEntryAttributes.Count + 1
                End If

                ''' check the referenced object entries
                ''' 
                If anObjectEntryAttribute.HasValueReferenceObjectEntry Then
                    Dim refObjectName As String = String.Empty
                    Dim refObjectEntry As String = String.Empty
                    Dim names = Shuffle.NameSplitter(anObjectEntryAttribute.ReferenceObjectEntry)
                    If names.Count > 1 Then
                        refObjectName = names(0)
                        refObjectEntry = names(1)
                    Else
                        refObjectEntry = anObjectEntryAttribute.ReferenceObjectEntry
                        refObjectName = Me.ObjectAttribute.ID
                    End If

                    ' will not take 
                    If Not _repository.HasObjectEntryAttribute(entryname:=refObjectEntry, objectname:=refObjectName) Then
                        CoreMessageHandler(message:="reference to object entry could not be resolved", procedure:="ObjectClassDescription.InitializeObjectEntryAttribute", messagetype:=otCoreMessageType.InternalError, containerID:=containerID)
                    End If
                End If


                '* save local in the object class description
                If overridesExisting And Not SaveObjectEntryAttribute(attribute:=anObjectEntryAttribute, entryname:=entryname, containerID:=containerID, overridesExisting:=overridesExisting) Then
                    CoreMessageHandler(message:="object entry attribute could not be stored in repository", entryname:=entryname, objectname:=_ObjectAttribute.ID, _
                                        procedure:="ObjectClassDescription.InitializeObjectEntryAttribute", messagetype:=otCoreMessageType.InternalError)
                End If

                ''' if not enabled delete the entry if we have one
                ''' doe it here so we could also do bookkeeping on deleting everything
                ''' BEWARE: Entries are stored under their FIELD VALUE = NAME not under the FIELD NAME (which are overwritten in the class)
                ''' 
                'If Not anObjectEntryAttribute.Enabled Then
                '    If overridesExisting Then
                '        If _ObjectEntryAttributes.ContainsKey(key:=name) Then _ObjectEntryAttributes.Remove(key:=name)
                '    End If
                '    Return True
                'End If

                '** create a foreign key attribute and store it with the global table
                '** use the reference object entry as foreign key reference
                If anObjectEntryAttribute.HasValueUseForeignKey AndAlso anObjectEntryAttribute.UseForeignKey <> otForeignKeyImplementation.None Then
                    If anObjectEntryAttribute.UseForeignKey <> otForeignKeyImplementation.None And _
                        Not anObjectEntryAttribute.HasValueForeignKeyReferences And anObjectEntryAttribute.HasValueReferenceObjectEntry Then
                        ' foreign key reference is the the reference object entry
                        anObjectEntryAttribute.ForeignKeyReferences = {anObjectEntryAttribute.ReferenceObjectEntry}

                    ElseIf anObjectEntryAttribute.UseForeignKey <> otForeignKeyImplementation.None And _
                        Not anObjectEntryAttribute.HasValueForeignKeyReferences And Not anObjectEntryAttribute.HasValueReferenceObjectEntry Then
                        CoreMessageHandler(message:="For using foreign keys either the foreign key reference or the reference object entry is set", _
                                               argument:=_Type.Name, entryname:=fieldvalue, messagetype:=otCoreMessageType.InternalWarning, _
                                               procedure:="ObjectClassDescription.InitializeObjectEntryAttribute", objectname:=_Type.Name)
                    End If

                    '*** create and add
                    If globalContainerAttributes IsNot Nothing Then
                        Dim newForeignKey As New ormForeignKeyAttribute
                        With newForeignKey
                            .ID = "FK_" & globalContainerAttributes.ContainerID & "_" & anObjectEntryAttribute.ContainerEntryName
                            '** use the reference
                            If anObjectEntryAttribute.HasValueForeignKeyReferences Then
                                .ForeignKeyReferences = anObjectEntryAttribute.ForeignKeyReferences
                            Else
                                CoreMessageHandler(message:="For using foreign keys either the foreign key reference or the reference object entry must be set", _
                                              argument:=_Type.Name, entryname:=fieldvalue, messagetype:=otCoreMessageType.InternalWarning, _
                                              procedure:="ObjectClassDescription.InitializeObjectEntryAttribute", objectname:=_Type.Name)
                            End If

                            .Entrynames = {anObjectEntryAttribute.ObjectName & "." & anObjectEntryAttribute.ContainerEntryName}

                            If anObjectEntryAttribute.HasValueForeignKeyProperties Then
                                .ForeignKeyProperties = anObjectEntryAttribute.ForeignKeyProperties
                            Else
                                ' CoreMessageHandler(message:="For using foreign keys the foreign key property should be set", _
                                '             arg1:=_Type.Name, entryname:=fieldvalue, messagetype:=otCoreMessageType.InternalWarning, _
                                '            subname:="ObjectClassDescription.InitializeObjectEntryAttribute", objectname:=_Type.Name)

                                ''' add defaults
                                Dim alist As New List(Of String)
                                alist.Add(ForeignKeyProperty.OnDelete & "(" & ForeignKeyActionProperty.Cascade & ")")
                                alist.Add(ForeignKeyProperty.OnUpdate & "(" & ForeignKeyActionProperty.Cascade & ")")
                                .ForeignKeyProperties = alist.ToArray
                            End If
                            If anObjectEntryAttribute.HasValueUseForeignKey Then
                                .UseForeignKey = anObjectEntryAttribute.UseForeignKey
                            Else
                                .UseForeignKey = otForeignKeyImplementation.None
                            End If

                            .Description = "created out of object entry " & anObjectEntryAttribute.ObjectName & "." & anObjectEntryAttribute.EntryName
                            .Version = anObjectEntryAttribute.Version
                            .ObjectID = anObjectEntryAttribute.ObjectName
                            .TableID = anObjectEntryAttribute.ContainerID

                        End With

                        '** check if the foreign key is a primarylink
                        '** add the default properties of OnDelete/OnUpdate
                        If anObjectEntryAttribute.HasValueForeignKeyProperties AndAlso _
                            Array.Exists(Of ForeignKeyProperty)(anObjectEntryAttribute.ForeignKeyProperty, Function(x) (x.Enum = otForeignKeyProperty.PrimaryTableLink)) Then
                            ''' add also the OnDelete / OnUpdate if not existing
                            ''' 
                            If Not Array.Exists(Of ForeignKeyProperty)(newForeignKey.ForeignKeyProperty, Function(x) (x.Enum = otForeignKeyProperty.OnDelete)) Then
                                Dim alist As List(Of String) = newForeignKey.ForeignKeyProperties.ToList
                                alist.Add(ForeignKeyProperty.OnDelete & "(" & ForeignKeyActionProperty.Cascade & ")")
                                newForeignKey.ForeignKeyProperties = alist.ToArray
                            End If
                            If Not Array.Exists(Of ForeignKeyProperty)(newForeignKey.ForeignKeyProperty, Function(x) (x.Enum = otForeignKeyProperty.OnUpdate)) Then
                                Dim alist As List(Of String) = newForeignKey.ForeignKeyProperties.ToList
                                alist.Add(ForeignKeyProperty.OnUpdate & "(" & ForeignKeyActionProperty.Cascade & ")")
                                newForeignKey.ForeignKeyProperties = alist.ToArray
                            End If
                        End If

                        '** add the foreign key
                        If Not globalContainerAttributes.HasForeignKey(newForeignKey.ID) Then
                            globalContainerAttributes.AddForeignKey(newForeignKey)
                        Else
                            CoreMessageHandler(message:="foreign key with ID '" & newForeignKey.ID & "' already exists in table attribute", _
                                              argument:=newForeignKey.ID, containerID:=globalContainerAttributes.ContainerID, entryname:=fieldvalue, _
                                              messagetype:=otCoreMessageType.InternalWarning, _
                                              procedure:="ObjectClassDescription.InitializeObjectEntryAttribute", objectname:=_Type.Name)
                        End If
                        Dim containerWise As New Dictionary(Of String, ormForeignKeyAttribute)
                        If _ForeignKeys.ContainsKey(key:=containerID) Then
                            containerWise = _ForeignKeys.Item(key:=containerID)
                        Else
                            _ForeignKeys.Add(key:=containerID, value:=containerWise)
                        End If
                        If Not containerWise.ContainsKey(key:=newForeignKey.ID) Then
                            containerWise.Add(key:=newForeignKey.ID, value:=newForeignKey)
                        Else
                            CoreMessageHandler(message:="foreign key with ID '" & newForeignKey.ID & "' already exists in object class attribute", _
                                           argument:=newForeignKey.ID, containerID:=globalContainerAttributes.ContainerID, entryname:=fieldvalue, _
                                           messagetype:=otCoreMessageType.InternalWarning, _
                                           procedure:="ObjectClassDescription.InitializeObjectEntryAttribute", objectname:=_Type.Name)

                        End If
                    End If
                End If

                ''' store the Primary Key also with the Object as Object Primary
                ''' 
                If anObjectEntryAttribute.HasValuePrimaryKeyOrdinal AndAlso _ContainerAttributes.Count > 0 Then

                    '** set the primary table of the object
                    If String.IsNullOrWhiteSpace(Me.PrimaryContainerID) Then Me.PrimaryContainerID = anObjectEntryAttribute.ContainerID

                    '* extend the objects primary key if primarytableid is addressed
                    If Me.PrimaryContainerID = anObjectEntryAttribute.ContainerID Then
                        If Not _ObjectAttribute.HasValuePrimaryKeys Then
                            _ObjectAttribute.PrimaryKeyEntryNames = {entryname.ToUpper}
                        Else
                            If _ObjectAttribute.PrimaryKeyEntryNames.GetUpperBound(0) < anObjectEntryAttribute.PrimaryKeyOrdinal - 1 Then
                                ReDim Preserve _ObjectAttribute.PrimaryKeyEntryNames(anObjectEntryAttribute.PrimaryKeyOrdinal - 1)
                            End If
                            _ObjectAttribute.PrimaryKeyEntryNames(anObjectEntryAttribute.PrimaryKeyOrdinal - 1) = entryname.ToUpper
                        End If
                    End If

                    '** extend the primary key column names of the container attribute. container primary key must be at least the object primary ke<y
                    Dim aContainerAttribute = Me.GetContainerAttribute(anObjectEntryAttribute.ContainerID)
                    ''' add the primarykey - extend if necessary - check if already in there
                    If aContainerAttribute IsNot Nothing Then
                        If aContainerAttribute.PrimaryEntryNames Is Nothing Then
                            aContainerAttribute.PrimaryEntryNames = {anObjectEntryAttribute.ContainerEntryName}
                        ElseIf Not aContainerAttribute.PrimaryEntryNames.Contains(anObjectEntryAttribute.ContainerEntryName) Then
                            Dim pknames As String() = aContainerAttribute.PrimaryEntryNames
                            '+ extend
                            If aContainerAttribute.PrimaryEntryNames.GetUpperBound(0) < anObjectEntryAttribute.PrimaryKeyOrdinal - 1 Then
                                ReDim Preserve pknames(anObjectEntryAttribute.PrimaryKeyOrdinal - 1)
                            End If
                            pknames(anObjectEntryAttribute.PrimaryKeyOrdinal - 1) = anObjectEntryAttribute.ContainerEntryName
                            '* set
                            aContainerAttribute.PrimaryEntryNames = pknames
                        End If
                    Else
                        CoreMessageHandler(message:="ATTENTION ! Container attribute is not defined in Object Description.", _
                                      objectname:=_ObjectAttribute.ID, containerID:=containerID, messagetype:=otCoreMessageType.InternalError, _
                                      procedure:="ObjectClassDescription.InitializeObjectEntryAttribute")
                    End If

                ElseIf anObjectEntryAttribute.HasValuePrimaryKeyOrdinal AndAlso _ContainerAttributes.Count = 0 Then
                    CoreMessageHandler(message:="ATTENTION ! Primary keys for Object Attributes are not defined - no container are used", _
                                       objectname:=_ObjectAttribute.ID, containerID:=containerID, messagetype:=otCoreMessageType.InternalError, _
                                       procedure:="ObjectClassDescription.InitializeObjectEntryAttribute")
                End If

                ''' return
                Return True

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectClassDescription.InitializeObjectEntryAttribute")
                Return False
            End Try

        End Function
        ''' <summary>
        ''' Initialize an ObjectEntry Mapping
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="tablename"></param>
        ''' <param name="value"></param>
        ''' <param name="fieldinfo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InitializeEntryMapping(attribute As Attribute, containerID As String, value As String, fieldinfo As FieldInfo, ClassOverrides As Boolean) As Boolean
            Try
                '* set the cloumn name
                Dim aMappingAttribute As ormObjectEntryMapping = DirectCast(attribute, ormObjectEntryMapping)
                '** default -> Table/Column mapping
                If Not aMappingAttribute.HasValueEntryName And Not aMappingAttribute.HasValueRelationName Then
                    CoreMessageHandler(message:="Entry Mapping Attribute was neither assigned to a data entry definition nor a relation definition", _
                                       argument:=_Type.Name, entryname:=value, messagetype:=otCoreMessageType.InternalWarning, _
                                       procedure:="ObjectClassDescription.InitializeEntryMapping", objectname:=_Type.Name)

                End If

                '** default -> Table/Column mapping
                If Not aMappingAttribute.HasValueContainerID Then
                    containerID = Me.PrimaryContainerID
                    If _ContainerAttributes.Count > 1 Then
                        ' CoreMessageHandler(message:="Column Attribute was not assigned to a table although multiple tables are defined in class - implicit assignment to '" & containerID & "'", _
                        '                   argument:=_Type.Name, entryname:=value, messagetype:=otCoreMessageType.InternalWarning, objectname:=_Type.Name, _
                        '                  procedure:="ObjectClassDescription.InitializeEntryMapping")
                    End If
                Else
                    containerID = aMappingAttribute.ContainerID
                End If
                ' reset the attributes 
                aMappingAttribute.ContainerID = containerID
                '** set the default columnname
                If aMappingAttribute.HasValueEntryName And Not aMappingAttribute.HasValueContainerEntryName Then
                    If _ObjectEntryAttributes.ContainsKey(key:=aMappingAttribute.EntryName) Then
                        Dim anObjectEntry = _ObjectEntryAttributes.Item(key:=aMappingAttribute.EntryName)
                        If anObjectEntry IsNot Nothing Then
                            aMappingAttribute.ContainerEntryName = anObjectEntry.ContainerEntryName
                        Else
                            CoreMessageHandler(message:="Object Entry was not initialized", _
                                           argument:=_Type.Name, entryname:=aMappingAttribute.EntryName, messagetype:=otCoreMessageType.InternalError, _
                                           procedure:="ObjectClassDescription.InitializeEntryMapping", objectname:=_Type.Name)
                        End If

                    Else
                        CoreMessageHandler(message:="Object Entry was not found", _
                                           argument:=_Type.Name, entryname:=aMappingAttribute.EntryName, messagetype:=otCoreMessageType.InternalError, _
                                           procedure:="ObjectClassDescription.InitializeEntryMapping", objectname:=_Type.Name)
                    End If

                End If

                '** save

                Dim aTablewiseDictionary As IDictionary
                Dim aGlobalDictionary As IDictionary
                Dim anID As String
                Dim aTablewiseID As String

                '***
                '*** ENTRY SETTING
                If aMappingAttribute.HasValueEntryName Then
                    aTablewiseDictionary = _ContainerMappings.Item(key:=containerID)
                    aGlobalDictionary = _ContainerEntryMappings
                    anID = aMappingAttribute.EntryName
                    aTablewiseID = aMappingAttribute.ContainerEntryName

                    If aTablewiseDictionary Is Nothing Then
                        CoreMessageHandler(message:="_tablecolumnsMappings   does not exist", containerID:=containerID, argument:=aMappingAttribute.ID, _
                                           messagetype:=otCoreMessageType.InternalError, _
                                           procedure:="ObjectClassDescription.InitializeEntryMapping", objectname:=_Type.Name)
                        Return False
                    End If

                    If Not _ObjectEntryAttributes.ContainsKey(key:=anID) Then
                        CoreMessageHandler(message:="the to be mapped entry attribute does not exist", containerID:=containerID, _
                                           argument:=aMappingAttribute.ID, _
                                          messagetype:=otCoreMessageType.InternalError, _
                                          procedure:="ObjectClassDescription.InitializeEntryMapping", objectname:=_Type.Name)
                    Else
                        aMappingAttribute.Enabled = _ObjectEntryAttributes.Item(key:=anID).Enabled
                    End If
                    '***
                    '*** RELATION SETTING
                ElseIf aMappingAttribute.HasValueRelationName Then
                    aTablewiseDictionary = _ContainerRelationMappings.Item(key:=containerID)
                    aGlobalDictionary = _RelationEntryMapping
                    anID = aMappingAttribute.RelationName
                    aTablewiseID = anID

                    If aTablewiseDictionary Is Nothing Then
                        CoreMessageHandler(message:="_tablerelationMappings or  does not exist", containerID:=containerID, argument:=aMappingAttribute.ID, _
                                           messagetype:=otCoreMessageType.InternalError, _
                                           procedure:="ObjectClassDescription.InitializeEntryMapping", objectname:=_Type.Name)
                        Return False
                    End If

                    If Not _Relations.ContainsKey(key:=anID) Then
                        CoreMessageHandler(message:="the to be mapped entry attribute does not exist", containerID:=containerID, _
                                           argument:=aMappingAttribute.ID, _
                                          messagetype:=otCoreMessageType.InternalError, _
                                          procedure:="ObjectClassDescription.InitializeEntryMapping", objectname:=_Type.Name)
                    Else
                        aMappingAttribute.Enabled = _Relations.Item(key:=anID).Enabled
                    End If
                Else
                    CoreMessageHandler(message:="EntryMapping Attribute has no link to object entries nor relation", argument:=aMappingAttribute.ID, _
                                       messagetype:=otCoreMessageType.InternalError, _
                                       procedure:="ObjectClassDescription.InitializeEntryMapping", objectname:=_Type.Name)
                    Return False
                End If



                '** add the fieldinfo to the global list for per Mapping.ID (which is the entryname or the relationname)
                Dim aList As List(Of FieldInfo)
                If aGlobalDictionary.Contains(key:=anID) Then
                    aList = aGlobalDictionary.Item(key:=anID)
                Else
                    aList = New List(Of FieldInfo)
                    aGlobalDictionary.Add(key:=anID, value:=aList)
                End If
                If aList.Find(Function(x)
                                  Return x.Name = fieldinfo.Name
                              End Function) Is Nothing Then
                    aList.Add(fieldinfo)
                End If

                '** add the fieldinfo to the list for per Mapping.ID (which is the entryname or the relationname)
                aList = New List(Of FieldInfo)
                If aTablewiseDictionary.Contains(key:=aTablewiseID) Then
                    aList = aTablewiseDictionary.Item(key:=aTablewiseID)
                Else
                    aList = New List(Of FieldInfo)
                    aTablewiseDictionary.Add(key:=aTablewiseID, value:=aList)
                End If
                If aList.Find(Function(x)
                                  Return x.Name = fieldinfo.Name
                              End Function) Is Nothing Then
                    aList.Add(fieldinfo)
                End If

                '** defaults
                If aMappingAttribute.HasValueRelationName Then
                    If Not aMappingAttribute.HasValueInfuseMode Then aMappingAttribute.InfuseMode = otInfuseMode.OnInject Or otInfuseMode.OnDemand
                ElseIf aMappingAttribute.HasValueEntryName Or aMappingAttribute.HasValueContainerEntryName Then
                    If Not aMappingAttribute.HasValueInfuseMode Then aMappingAttribute.InfuseMode = otInfuseMode.Always
                End If

                '** store the MappingAttribute under the fieldinfo name
                If Not _EntryMappings.ContainsKey(key:=fieldinfo.Name) Then
                    _EntryMappings.Add(key:=fieldinfo.Name, value:=aMappingAttribute)
                ElseIf ClassOverrides Then
                    Return True '* do nothing
                ElseIf Not ClassOverrides Then
                    _EntryMappings.Remove(key:=fieldinfo.Name)
                    _EntryMappings.Add(key:=fieldinfo.Name, value:=aMappingAttribute)
                Else
                    CoreMessageHandler(message:="Warning ! Field Member already associated with EntryMapping", argument:=fieldinfo.Name, _
                                       objectname:=_Type.Name, messagetype:=otCoreMessageType.InternalWarning, procedure:="ObjectClassDescription.InitializeEntryMapping")
                End If

                '*** create the setter
                If Not _MappingSetterDelegates.ContainsKey(key:=fieldinfo.Name) Then
                    Dim setter As Action(Of iormInfusable, Object) = CreateILGSetterDelegate(Of iormInfusable, Object)(_Type, fieldinfo)
                    _MappingSetterDelegates.Add(key:=fieldinfo.Name, value:=setter)
                End If
                '*** create the getter
                If Not _MappingGetterDelegates.ContainsKey(key:=fieldinfo.Name) Then
                    Dim getter = CreateILGGetterDelegate(Of Object, Object)(_Type, fieldinfo)
                    _MappingGetterDelegates.Add(key:=fieldinfo.Name, value:=getter)
                End If

                Return True
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectClassDescription.InitializeEntryMapping")
                Return False
            End Try
        End Function
        ''' <summary>
        ''' Initialize a Relation Attribute to the Description
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="name"></param>
        ''' <param name="tablename"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InitializeRelationAttribute(attribute As Attribute, name As String, containerID As String, value As String, overridesExisting As Boolean) As Boolean
            Try

                '* set the cloumn name
                Dim aRelationAttribute As ormRelationAttribute = DirectCast(attribute, ormRelationAttribute)
                If String.IsNullOrWhiteSpace(name) Then name = value
                If String.IsNullOrWhiteSpace(containerID) Then
                    '** default
                    If Not aRelationAttribute.HasValueContainerID Then
                        containerID = Me.PrimaryContainerID
                        If _ContainerAttributes.Count > 1 Then
                            'CoreMessageHandler(message:="Relation Attribute was not assigned to a table although multiple tables are defined in class - implicit assignment to '" & containerid & "'", _
                            '                  argument:=_Type.Name, entryname:=value, messagetype:=otCoreMessageType.InternalWarning, procedure:="ObjectClassDescription.initializeRelationAttribute")
                        End If
                    Else
                        containerID = aRelationAttribute.ContainerID
                    End If
                End If
                ' reset the attributes 
                name = name.ToUpper

                aRelationAttribute.Name = name
                aRelationAttribute.ContainerID = containerID
                '* save to global
                If Not _Relations.ContainsKey(key:=name) Then
                    _Relations.Add(key:=name, value:=aRelationAttribute)
                ElseIf Not overridesExisting Then
                ElseIf overridesExisting Then
                    _Relations.Remove(key:=name)
                    _Relations.Add(key:=name, value:=aRelationAttribute)
                End If

                '** save to tablewise
                Dim aDictionary = _ContainerRelations.Item(key:=containerID)
                If aDictionary IsNot Nothing Then
                    If Not aDictionary.ContainsKey(key:=name) Then
                        aDictionary.Add(key:=name, value:=aRelationAttribute)
                    ElseIf Not overridesExisting Then
                        Return True '
                    ElseIf overridesExisting Then
                        aDictionary.Remove(key:=name)
                        aDictionary.Add(key:=name, value:=aRelationAttribute)
                    End If

                Else
                    CoreMessageHandler(message:="_tablerelations does not exist", argument:=containerID, messagetype:=otCoreMessageType.InternalError, procedure:="ObjectClassDescription.initializeRelationAttribute")
                End If
                '** linkobject
                If Not aRelationAttribute.HasValueLinkedObject Then
                    CoreMessageHandler(message:="Relation Attribute has not defined a linked object type", objectname:=_Type.Name, _
                                       argument:=name, messagetype:=otCoreMessageType.InternalError, procedure:="ObjectClassDescription.initializeRelationAttribute")
                Else
                    Dim atype As System.Type = aRelationAttribute.LinkObject
                    If atype.IsAbstract Then
                        CoreMessageHandler(message:="Relation Attribute with a linked object type which is abstract (mustinherit) is not supported", objectname:=_Type.Name, _
                                       argument:=name, messagetype:=otCoreMessageType.InternalError, procedure:="ObjectClassDescription.initializeRelationAttribute")
                    End If

                End If

                If Not aRelationAttribute.HasValueLinkJOin AndAlso _
                Not (aRelationAttribute.HasValueFromEntries OrElse aRelationAttribute.HasValueToEntries) AndAlso _
                Not aRelationAttribute.HasValueToPrimarykeys Then
                    ' more possibilitues now e.g events or operation
                    'CoreMessageHandler(message:="Relation Attribute has not defined a link join or a matching entries or a target primary keys  - how to link ?", _
                    '                   objectname:=_Type.Name, _
                    '                   arg1:=name, messagetype:=otCoreMessageType.InternalError, subname:="ObjectClassDescription.initializeRelationAttribute")
                End If
                If aRelationAttribute.HasValueFromEntries AndAlso aRelationAttribute.HasValueToEntries Then
                    If aRelationAttribute.ToEntries.Count > aRelationAttribute.FromEntries.Count Then
                        CoreMessageHandler(message:="relation attribute has nor mot ToEntries than FromEntries set", _
                                           argument:=name, objectname:=_Type.Name, _
                                           procedure:="ObjectClassDescription.initializeRelationAttribute", messagetype:=otCoreMessageType.InternalError)
                    End If
                End If

                '** defaults
                If Not aRelationAttribute.HasValueCascadeOnCreate Then aRelationAttribute.CascadeOnCreate = False
                If Not aRelationAttribute.HasValueCascadeOnDelete Then aRelationAttribute.CascadeOnDelete = False
                If Not aRelationAttribute.HasValueCascadeOnUpdate Then aRelationAttribute.CascadeOnUpdate = False


                Return True

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectClassDescription.InitializeRelationAttribute")
                Return False
            End Try
        End Function
        ''' <summary>
        ''' Initialize a Relation Attribute to the Description
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="name"></param>
        ''' <param name="tablename"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InitializeForeignKeyAttribute(attribute As Attribute, name As String, containerID As String, value As String, overridesExisting As Boolean) As Boolean
            Try

                '* set the cloumn name
                Dim aForeignKeyAttribute As ormForeignKeyAttribute = DirectCast(attribute, ormForeignKeyAttribute)
                If String.IsNullOrWhiteSpace(name) Then name = value

                '** default
                If String.IsNullOrWhiteSpace(containerID) Then
                    If Not aForeignKeyAttribute.HasValueTableID Then
                        containerID = Me.PrimaryContainerID
                        If _ContainerAttributes.Count > 1 Then
                            'CoreMessageHandler(message:="Foreign Key Attribute was not assigned to a table although multiple tables are defined in class - implicit assignment to '" & containerID & "'", _
                            '                   argument:=_Type.Name, entryname:=value, messagetype:=otCoreMessageType.InternalWarning, _
                            '                   procedure:="ObjectClassDescription.InitializeForeignKeyAttribute")
                        End If
                    Else
                        containerID = aForeignKeyAttribute.TableID
                    End If
                End If

                ' reset the attributes 
                name = containerID & "_" & name.ToUpper
                aForeignKeyAttribute.ID = name
                If Not aForeignKeyAttribute.HasValueTableID Then aForeignKeyAttribute.TableID = containerID
                If _ObjectAttribute.HasValueID Then aForeignKeyAttribute.ObjectID = _ObjectAttribute.ID

                '** save to table wise dictionary
                Dim aDictionary As New Dictionary(Of String, ormForeignKeyAttribute)
                If _ForeignKeys.ContainsKey(containerID) Then
                    aDictionary = _ForeignKeys.Item(key:=containerID)
                Else
                    _ForeignKeys.Add(key:=containerID, value:=aDictionary)
                End If

                If Not aDictionary.ContainsKey(key:=name) Then
                    aDictionary.Add(key:=name, value:=aForeignKeyAttribute)
                ElseIf Not overridesExisting Then
                    Return True '
                ElseIf overridesExisting Then
                    aDictionary.Remove(key:=name)
                    aDictionary.Add(key:=name, value:=aForeignKeyAttribute)
                End If

                '** save the table attribute
                If _ContainerAttributes.ContainsKey(containerID) Then
                    Dim aContainerAttribute = _ContainerAttributes.Item(containerID)
                    If Not aContainerAttribute.HasForeignKey(name) Then
                        aContainerAttribute.AddForeignKey(aForeignKeyAttribute)
                    End If
                Else
                    CoreMessageHandler(message:="container attribute was not defined in global container attribute store", argument:=name, _
                                       messagetype:=otCoreMessageType.InternalError, _
                                       procedure:="ObjectClassDescription.InitializeForeignKeyAttribute", containerID:=containerID, objectname:=_Type.Name)

                End If

                '** save to global table attribute
                Dim globalContainerAttributes = _repository.GetContainerAttribute(containerID)
                If globalContainerAttributes IsNot Nothing Then
                    If Not globalContainerAttributes.HasForeignKey(name) Then
                        globalContainerAttributes.AddForeignKey(aForeignKeyAttribute)
                    End If
                Else
                    CoreMessageHandler(message:="table attribute was not defined in global table attribute store", argument:=name, _
                                       messagetype:=otCoreMessageType.InternalError, _
                                       procedure:="ObjectClassDescription.InitializeForeignKeyAttribute", containerID:=containerID, objectname:=_Type.Name)

                End If


                '*** check the entrynames references
                '***
                If Not aForeignKeyAttribute.HasValueEntrynames Then
                    CoreMessageHandler(message:="entrynames must be defined in foreign key attribute", objectname:=_Type.Name, _
                                       argument:=name, messagetype:=otCoreMessageType.InternalError, procedure:="ObjectClassDescription.InitializeForeignKeyAttribute")
                Else
                    For i = 0 To aForeignKeyAttribute.Entrynames.Count - 1
                        Dim areference As String = aForeignKeyAttribute.Entrynames(i)
                        Dim objectname As String
                        Dim entryname As String

                        If areference.Contains("."c) OrElse areference.Contains(ConstDelimiter) Then
                            Dim names = Shuffle.NameSplitter(areference)
                            objectname = names(0)
                            entryname = names(1)
                            If objectname.ToUpper <> aForeignKeyAttribute.ObjectID Then
                                CoreMessageHandler(message:="entrynames " & aForeignKeyAttribute.Entrynames.ToString & " in foreign key attribute must be defined for the object", objectname:=_Type.Name, _
                                      argument:=objectname, messagetype:=otCoreMessageType.InternalError, procedure:="ObjectClassDescription.InitializeForeignKeyAttribute")
                            End If
                        Else
                            '** add the objectname
                            objectname = aForeignKeyAttribute.ObjectID
                            entryname = areference
                            aForeignKeyAttribute.Entrynames(i) = objectname.ToUpper & "." & entryname.ToUpper
                        End If

                        '** reference cannot be checked at this time
                        '**
                        'Dim anentry As ormObjectEntryAttribute = _repository.GetObjectEntryAttribute(entryname:=entryname, objectname:=objectname)
                        'If anentry Is Nothing Then
                        '    CoreMessageHandler(message:="entry reference object entry is not found the repository: '" & areference & "'", _
                        '             arg1:=name, objectname:=objectname, entryname:=entryname, _
                        '             messagetype:=otCoreMessageType.InternalError, subname:="ObjectClassDescription.InitializeForeignKeyAttribute")
                        'End If
                    Next
                End If


                '*** check the foreign key references
                '***
                If Not aForeignKeyAttribute.HasValueForeignKeyReferences Then
                    CoreMessageHandler(message:="foreign key references must be defined in foreign key attribute", objectname:=_Type.Name, _
                                       argument:=name, messagetype:=otCoreMessageType.InternalError, procedure:="ObjectClassDescription.InitializeForeignKeyAttribute")
                Else
                    For Each areference In aForeignKeyAttribute.ForeignKeyReferences
                        If Not areference.Contains("."c) AndAlso Not areference.Contains(ConstDelimiter) Then
                            CoreMessageHandler(message:="foreign key references must be [objectname].[entryname] in the foreign key attribute and not: '" & areference & "'", objectname:=_Type.Name, _
                                      argument:=name, messagetype:=otCoreMessageType.InternalError, procedure:="ObjectClassDescription.InitializeForeignKeyAttribute")
                        Else
                            Dim names = Shuffle.NameSplitter(areference)
                            Dim objectname = names(0)
                            Dim entryname = names(1)
                            '** reference cannot be checked this time
                            '**
                            'Dim anentry As ormObjectEntryAttribute = _repository.GetObjectEntryAttribute(entryname:=entryname, objectname:=objectname)
                            'If anentry Is Nothing Then
                            '    CoreMessageHandler(message:="foreign key reference object entry is not found the repository: '" & areference & "'", objectname:=_Type.Name, _
                            '             arg1:=name, messagetype:=otCoreMessageType.InternalError, subname:="ObjectClassDescription.InitializeForeignKeyAttribute")
                            'Else
                            'If Not anentry.HasValueTableName Then
                            '    CoreMessageHandler(message:="foreign key reference object entry has no tablename defined : '" & areference & "'", objectname:=_Type.Name, _
                            '         arg1:=name, messagetype:=otCoreMessageType.InternalError, subname:="ObjectClassDescription.InitializeForeignKeyAttribute")
                            'Else
                            '    globaleTableAttributes = _repository.GetTableAttribute(anentry.Tablename)
                            '    If globaleTableAttributes IsNot Nothing Then
                            '        If Not globaleTableAttributes.HasColumn(anentry.ColumnName) Then
                            '            CoreMessageHandler(message:="In foreign key attribute the foreign key reference column was not defined in table", arg1:=name, _
                            '                               tablename:=anentry.Tablename, columnname:=anentry.ColumnName, _
                            '                               objectname:=objectname, entryname:=entryname, _
                            '                                messagetype:=otCoreMessageType.InternalError, _
                            '                                subname:="ObjectClassDescription.InitializeForeignKeyAttribute")
                            '        End If
                            '    Else
                            '        CoreMessageHandler(message:="In foreign key attribute the table was not defined in global table attribute store", arg1:=name, _
                            '                           messagetype:=otCoreMessageType.InternalError, _
                            '                             tablename:=anentry.Tablename, columnname:=anentry.ColumnName, _
                            '                              objectname:=objectname, entryname:=entryname, _
                            '                           subname:="ObjectClassDescription.InitializeForeignKeyAttribute")

                            '    End If
                            'End If
                            'End If
                        End If

                    Next
                End If

                '*** check number of entries
                If aForeignKeyAttribute.HasValueForeignKeyReferences AndAlso aForeignKeyAttribute.HasValueEntrynames Then
                    If aForeignKeyAttribute.ForeignKeyReferences.Count <> aForeignKeyAttribute.Entrynames.Count Then
                        CoreMessageHandler(message:="foreign key references must be the same number as entry names", objectname:=_Type.Name, _
                                           argument:=name, messagetype:=otCoreMessageType.InternalError, procedure:="ObjectClassDescription.InitializeForeignKeyAttribute")
                    End If
                End If

                '** defaults
                If Not aForeignKeyAttribute.HasValueVersion Then aForeignKeyAttribute.Version = 1
                If Not aForeignKeyAttribute.HasValueUseForeignKey Then
                    aForeignKeyAttribute.UseForeignKey = otForeignKeyImplementation.None
                    CoreMessageHandler(message:="In foreign key attribute the use foreign key is not set - set to none", argument:=name, _
                                                      messagetype:=otCoreMessageType.InternalWarning, _
                                                      containerID:=containerID, objectname:=_Type.Name, _
                                                      procedure:="ObjectClassDescription.InitializeForeignKeyAttribute")
                End If

                If Not aForeignKeyAttribute.HasValueForeignKeyProperties Then
                    'CoreMessageHandler(message:="In foreign key attribute the properties are not set - set to default", argument:=name, _
                    '                                  messagetype:=otCoreMessageType.InternalWarning, _
                    '                                  containerID:=containerID, objectname:=_Type.Name, _
                    '                                  procedure:="ObjectClassDescription.InitializeForeignKeyAttribute")
                    aForeignKeyAttribute.ForeignKeyProperties = {ForeignKeyProperty.OnUpdate & "(" & ForeignKeyActionProperty.Cascade & ")", _
                                                                 ForeignKeyProperty.OnDelete & "(" & ForeignKeyActionProperty.Cascade & ")"}
                End If


                Return True

            Catch ex As Exception

                CoreMessageHandler(exception:=ex, procedure:="ObjectClassDescription.InitializeForeignKeyAttribute")
                Return False
            End Try
        End Function
        ''' <summary>
        ''' Initialize a Transaction Attribute to the Description
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="name"></param>
        ''' <param name="tablename"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InitializeTransactionAttribute(attribute As Attribute, objectname As String, name As String, value As String, _
                                                      overridesExisting As Boolean) As Boolean
            Try

                '* set the  name
                Dim aTransactionAttribute As ormObjectTransactionAttribute = DirectCast(attribute, ormObjectTransactionAttribute)
                If String.IsNullOrWhiteSpace(name) Then name = value
                If String.IsNullOrWhiteSpace(objectname) Then objectname = _ObjectAttribute.ID
                ' reset the attributes 
                name = name.ToUpper
                '** default
                aTransactionAttribute.TransactionName = name
                If Not aTransactionAttribute.HasValueDefaultAllowPermission Then aTransactionAttribute.DefaultAllowPermission = True
                If Not aTransactionAttribute.HasValueID Then aTransactionAttribute.ID = name
                If Not aTransactionAttribute.HasValueVersion Then aTransactionAttribute.Version = 1
                '* save to global
                If Not _ObjectTransactionAttributes.ContainsKey(key:=name) Then
                    _ObjectTransactionAttributes.Add(key:=name, value:=aTransactionAttribute)
                ElseIf Not overridesExisting Then
                ElseIf overridesExisting Then
                    _ObjectTransactionAttributes.Remove(key:=name)
                    _ObjectTransactionAttributes.Add(key:=name, value:=aTransactionAttribute)
                End If

                '** validate rules
                If aTransactionAttribute.HasValuePermissionRules Then
                    For Each Rule In aTransactionAttribute.PermissionRules
                        Dim aProp As ObjectPermissionRuleProperty = New ObjectPermissionRuleProperty(Rule)
                        If Not aProp.Validate Then
                            CoreMessageHandler(message:="property rule did not validate", argument:=name & "[" & Rule & "]", objectname:=_ObjectAttribute.ID, _
                                               procedure:="ObjectClassDescription.InitializeTransactionAttribute", messagetype:=otCoreMessageType.InternalError)
                        End If
                    Next
                End If
                Return True

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectClassDescription.InitializeTransactionAttribute")
                Return False
            End Try
        End Function
        ''' <summary>
        ''' Initialize the index Attribute
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="name"></param>
        ''' <param name="tablename"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InitializeIndexAttribute(attribute As Attribute, name As String, containerID As String, value As String, overridesExisting As Boolean) As Boolean
            Try

                '* set the cloumn name
                Dim anIndexAttribute As ormIndexAttribute = DirectCast(attribute, ormIndexAttribute)
                If String.IsNullOrWhiteSpace(name) Then name = value.ToUpper
                '** default
                If String.IsNullOrWhiteSpace(containerID) Then
                    If Not anIndexAttribute.HasValueTableID Then
                        containerID = Me.PrimaryContainerID
                        If _ContainerAttributes.Count > 1 Then
                            'CoreMessageHandler(message:="Index Attribute was not assigned to a table although multiple tables are defined in class - implicit assignment to '" & containerid & "'", _
                            '                  argument:=_Type.Name, entryname:=value, messagetype:=otCoreMessageType.InternalWarning, procedure:="ObjectClassDescription.Refresh")
                        End If
                    Else
                        containerID = anIndexAttribute.TableID
                    End If
                End If

                ' reset the attributes 
                anIndexAttribute.IndexName = name
                If Not anIndexAttribute.HasValueTableID Then anIndexAttribute.TableID = containerID
                '* save to global
                If Not _Indices.ContainsKey(key:=name) Then
                    _Indices.Add(key:=name, value:=anIndexAttribute)
                ElseIf Not overridesExisting Then
                ElseIf overridesExisting Then
                    _Indices.Remove(key:=name)
                    _Indices.Add(key:=name, value:=anIndexAttribute)
                End If

                '** save
                Dim aDictionary = _ContainerIndices.Item(key:=containerID)
                If aDictionary IsNot Nothing Then
                    If Not aDictionary.ContainsKey(key:=name) Then
                        aDictionary.Add(key:=name, value:=anIndexAttribute)
                    ElseIf Not overridesExisting Then
                        Return True '** do nothing with the ClassOverrides one
                    ElseIf overridesExisting Then
                        aDictionary.Remove(key:=name)
                        aDictionary.Add(key:=name, value:=anIndexAttribute) '** overwrite the non-ClassOverrides
                    End If

                Else
                    CoreMessageHandler(message:="_tableindex does not exist", argument:=containerID, messagetype:=otCoreMessageType.InternalError, procedure:="ObjectClassDescription.Refresh")
                End If

                '** save to global table attribute
                Dim globalContainerAttributes = _repository.GetContainerAttribute(containerID)
                If globalContainerAttributes IsNot Nothing Then
                    If Not globalContainerAttributes.HasIndex(name) Then
                        globalContainerAttributes.AddIndex(anIndexAttribute)
                    End If
                Else
                    CoreMessageHandler(message:="container attribute was not defined in global container attribute store", argument:=name, _
                                       messagetype:=otCoreMessageType.InternalError, _
                                       procedure:="ObjectClassDescription.InitializeIndexAttribute", containerID:=containerID, objectname:=_Type.Name)

                End If

                Return True

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectClassDescription.InitializeIndexAttribute")
                Return False
            End Try
        End Function
        ''' <summary>
        ''' Initialize the ObjectAttribute by a const field member of the class
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="fieldinfo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InitializeObjectAttributeByField(attribute As Attribute, fieldinfo As FieldInfo) As Boolean
            Try
                If _ObjectAttribute Is Nothing Then
                    _ObjectAttribute = attribute
                Else
                    With DirectCast(attribute, ormObjectAttribute)
                        If .HasValueDomainBehavior Then _ObjectAttribute.AddDomainBehavior = .AddDomainBehavior
                        If .HasValueClassname Then _ObjectAttribute.ClassName = .ClassName
                        If .HasValueDeleteFieldBehavior Then _ObjectAttribute.AddDeleteFieldBehavior = .AddDeleteFieldBehavior
                        If .HasValueDescription Then _ObjectAttribute.Description = .Description
                        If .HasValueDomainBehavior Then _ObjectAttribute.AddDomainBehavior = .AddDomainBehavior
                        If .HasValueID Then _ObjectAttribute.ID = .ID
                        If .HasValueIsActive Then _ObjectAttribute.IsActive = .IsActive
                        If .HasValueModulename Then _ObjectAttribute.Modulename = .Modulename
                        If .HasValueSpareFieldsBehavior Then _ObjectAttribute.AddSpareFieldsBehavior = .AddSpareFieldsBehavior
                        If .HasValuePrimaryKeys Then _ObjectAttribute.PrimaryKeyEntryNames = .PrimaryKeyEntryNames
                        .ObjectClassDescription = Me ' backlink
                    End With
                End If

                '** defaults
                If Not _ObjectAttribute.HasValueClassname Then
                    _ObjectAttribute.ClassName = _Type.FullName
                End If
                If _ObjectAttribute.ID Is Nothing OrElse _ObjectAttribute.ID = String.Empty Then
                    _ObjectAttribute.ID = fieldinfo.GetValue(Nothing).ToString.ToUpper
                End If
                If _ObjectAttribute.Modulename Is Nothing OrElse _ObjectAttribute.Modulename = String.Empty Then
                    _ObjectAttribute.Modulename = _Type.Namespace.ToUpper
                End If
                If _ObjectAttribute.Description Is Nothing OrElse _ObjectAttribute.Description = String.Empty Then
                    _ObjectAttribute.Description = String.Empty
                End If
                ''' backlink
                _ObjectAttribute.ObjectClassDescription = Me ' backlink
                Return True
            Catch ex As Exception
                CoreMessageHandler(procedure:="ObjectClassDescription.InitializeFieldObjectEntryAttribute", exception:=ex)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Initialize a Transaction Attribute to the Description
        ''' </summary>
        ''' <param name="attribute"></param>
        ''' <param name="name"></param>
        ''' <param name="tablename"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InitializeOperationAttribute(attribute As Attribute, methodinfo As MethodInfo, _
                                                      overridesExisting As Boolean) As Boolean
            Try

                '* set the  name
                Dim anOperationAttribute As ormObjectOperationMethodAttribute = DirectCast(attribute, ormObjectOperationMethodAttribute)

                '** default
                anOperationAttribute.ID = methodinfo.Name.ToUpper
                anOperationAttribute.ClassDescription = Me

                If Not anOperationAttribute.HasValueOperationName Then anOperationAttribute.OperationName = methodinfo.Name.ToUpper
                anOperationAttribute.OperationName = anOperationAttribute.OperationName.ToUpper 'always to upper
                If Not anOperationAttribute.HasValueVersion Then anOperationAttribute.Version = 1
                anOperationAttribute.MethodInfo = methodinfo

                ''' check parameters
                If anOperationAttribute.HasValueParameterEntries Then
                    If anOperationAttribute.ParameterEntries.Count <> methodinfo.GetParameters.Count Then
                        CoreMessageHandler(message:="operation parameter count differs from method's parameter count", procedure:="ObjectClassDescription.InitializeOperationAttribute", _
                                     messagetype:=otCoreMessageType.InternalWarning, argument:=methodinfo.Name)
                    End If
                End If

                ''' check return parameters only if used in relation !
                ''' 
                If Me._Relations.Where(Function(x) (x.Value.HasValueCreateOperationID AndAlso x.Value.CreateOperation.ToUpper = Me.Name.ToUpper) OrElse (x.Value.HasValueRetrieveOperationID AndAlso x.Value.RetrieveOperation.ToUpper = Me.Name.ToUpper)).Count > 0 Then

                    Dim result As Boolean = False
                    Dim rtype As System.Type = methodinfo.ReturnType

                    If rtype.Equals(GetType(iormRelationalPersistable)) OrElse rtype.GetInterfaces.Contains(GetType(iormRelationalPersistable)) Then
                        result = True
                    ElseIf rtype.IsInterface AndAlso rtype.IsGenericType AndAlso _
                        (rtype.GetGenericTypeDefinition.Equals(GetType(IList(Of ))) OrElse rtype.GetGenericTypeDefinition.Equals(GetType(IEnumerable(Of ))) _
                         OrElse rtype.GetGenericTypeDefinition.Equals(GetType(iormRelationalCollection(Of )))
                            ) Then
                        result = True
                    ElseIf rtype.GetInterfaces.Contains(GetType(IList(Of ))) OrElse rtype.GetInterfaces.Contains(GetType(IEnumerable(Of ))) _
                        OrElse rtype.GetInterfaces.Contains(GetType(iormRelationalCollection(Of ))) Then
                        If rtype.GetGenericArguments(1).GetInterfaces.Equals(GetType(iormRelationalPersistable)) Then
                            result = True
                        Else
                            CoreMessageHandler(message:="generic return type is not of iormpersistable", procedure:="ObjectClassDescription.InitializeOperationAttribute", _
                                          messagetype:=otCoreMessageType.InternalError, argument:=methodinfo.Name)
                        End If
                    Else
                        CoreMessageHandler(message:="return type is not of iormpersistable or array, list, iormrelationalcollection nor dictionary", procedure:="ObjectClassDescription.InitializeOperationAttribute", _
                                                     messagetype:=otCoreMessageType.InternalError, argument:=methodinfo.Name)
                    End If
                End If

                '* generate the caller and save it
                Dim OperationDelegate = CreateILGMethodInvoker(methodinfo)

                If _OperationCallerDelegates.ContainsKey(anOperationAttribute.OperationName) Then
                    _OperationCallerDelegates.Remove(anOperationAttribute.OperationName)
                End If
                _OperationCallerDelegates.Add(key:=anOperationAttribute.OperationName, value:=OperationDelegate)

                '** save to description
                If Not _ObjectOperationAttributes.ContainsKey(key:=anOperationAttribute.OperationName) Then
                    _ObjectOperationAttributes.Add(key:=anOperationAttribute.OperationName, value:=anOperationAttribute)
                ElseIf Not overridesExisting Then
                ElseIf overridesExisting Then
                    _ObjectOperationAttributes.Remove(key:=anOperationAttribute.OperationName)
                    _ObjectOperationAttributes.Add(key:=anOperationAttribute.OperationName, value:=anOperationAttribute)
                End If

                '** store under Tag
                If anOperationAttribute.HasValueTag Then
                    If Not _ObjectOperationAttributesByTag.ContainsKey(key:=anOperationAttribute.Tag) Then
                        _ObjectOperationAttributesByTag.Add(key:=anOperationAttribute.Tag, value:=anOperationAttribute)
                    ElseIf Not overridesExisting Then
                    ElseIf overridesExisting Then
                        _ObjectOperationAttributesByTag.Remove(key:=anOperationAttribute.Tag)
                        _ObjectOperationAttributesByTag.Add(key:=anOperationAttribute.Tag, value:=anOperationAttribute)
                    End If
                End If

                Return True

            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectClassDescription.InitializeOperationAttribute")
                Return False
            End Try
        End Function
        ''' <summary>
        ''' set the hook for the generic Retrieve
        ''' </summary>
        ''' <param name="methodinfo"></param>
        ''' <returns>True if the hook was set</returns>
        ''' <remarks></remarks>
        'Private Function InitializeMethodRetrieveHook(methodinfo As MethodInfo) As Boolean
        '    '*
        '    If Not methodinfo.IsGenericMethodDefinition Then
        '        CoreMessageHandler(message:="retrieve is not a generic method in class", procedure:="ObjectClassDescription.InitializeMethodRetrieveHook", _
        '                           messagetype:=otCoreMessageType.InternalError, objectname:=methodinfo.GetBaseDefinition.Name)
        '        Return False
        '    End If

        '    Dim ahandle = methodinfo.MethodHandle
        '    Dim genericMethod = methodinfo.MakeGenericMethod({_Type})
        '    Dim parameters = genericMethod.GetParameters
        '    Dim retrieveParameters As ParameterInfo() = {}

        '    '     // compare the method parameters
        '    'if (parameters.Length == parameterTypes.Length) {
        '    '  for (int i = 0; i < parameters.Length; i++) {
        '    '    if (parameters[i].ParameterType != parameterTypes[i]) {
        '    '      continue; // this is not the method we're looking for
        '    '    }
        '    '  }

        '    If parameters.Count = 5 Then
        '        If _DataOperationHooks.ContainsKey(key:=ConstMTRetrieve) Then
        '            _DataOperationHooks.Remove(key:=ConstMTRetrieve)
        '        End If
        '        _DataOperationHooks.Add(key:=ConstMTRetrieve, value:=genericMethod.MethodHandle)
        '        Return True
        '    End If
        '    Return False
        'End Function
        ''' <summary>
        ''' Initialize the right CreateDataObject Function
        ''' </summary>
        ''' <param name="methodinfo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        'Private Function InitializeMethodCreateHook(methodinfo As MethodInfo) As Boolean
        '    '*
        '    If Not methodinfo.IsGenericMethodDefinition Then
        '        CoreMessageHandler(message:="CreateDataObject is not a generic method in class", procedure:="ObjectClassDescription.InitializeMethodCreateHook", _
        '                           messagetype:=otCoreMessageType.InternalError, objectname:=methodinfo.GetBaseDefinition.Name)
        '        Return False
        '    End If
        '    Dim genericMethod As MethodInfo = methodinfo.MakeGenericMethod({_Type})
        '    Dim parameters = genericMethod.GetParameters
        '    Dim retrieveParameters As ParameterInfo() = {}
        '    Dim found As Boolean = False

        '    '     // compare the method parameters
        '    'if (parameters.Length == parameterTypes.Length) {
        '    '  for (int i = 0; i < parameters.Length; i++) {
        '    '    if (parameters[i].ParameterType != parameterTypes[i]) {
        '    '      continue; // this is not the method we're looking for
        '    '    }
        '    '  }

        '    If parameters.Count = 4 Then

        '        For i = 0 To parameters.Length - 1
        '            ' And parameters(i).ParameterType.IsArray doesnot work ?!
        '            If parameters(i).ParameterType.Name.ToUpper = "Object[]&".ToUpper Then
        '                found = True
        '                Exit For
        '            End If
        '        Next

        '        If Not found Then Return False

        '        '*** save
        '        If _DataOperationHooks.ContainsKey(key:=ConstMTCreateDataObject) Then
        '            _DataOperationHooks.Remove(key:=ConstMTCreateDataObject)
        '        End If
        '        _DataOperationHooks.Add(key:=ConstMTCreateDataObject, value:=genericMethod.MethodHandle)
        '        Return True
        '    End If
        '    Return False
        'End Function
        ''' <summary>
        ''' refresh all the loaded information
        ''' </summary>
        ''' <param name="force"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Initialize(Optional force As Boolean = False) As Boolean

            If Me.IsInitialized AndAlso Not force Then Return False

            '** reset
            _ContainerEntryMappings.Clear()
            '_ContainerAttributes.Clear() -> preset by Initialize of ObjectClassRepository
            _Indices.Clear()
            _RelationEntryMapping.Clear()
            _ContainerMappings.Clear()
            _ContainerIndices.Clear()
            _ObjectEntriesPerContainer.Clear()
            _ContainerRelationMappings.Clear()
            _Relations.Clear()
            '_ObjectEntryAttributes.Clear() -> preset by Initialize of ObjectClassRepository
            '_ObjectAttribute = Nothing -> preset by Initialize of ObjectClassRepository
            _DataOperationHooks.Clear()
            _EntryMappings.Clear()
            _ObjectTransactionAttributes.Clear()
            _ForeignKeys.Clear()
            _QueryAttributes.Clear()
            _ObjectOperationAttributes.Clear()
            _ObjectOperationAttributesByTag.Clear()

            '***
            '*** collect all the attributes first
            '***
            Dim aFieldList As System.Reflection.FieldInfo()
            Dim aName As String
            Dim aContainerID As String
            Dim aValue As String

            Try

                SyncLock _lock

                    '** save the ObjectAttribute
                    For Each anAttribute As System.Attribute In Attribute.GetCustomAttributes(Me.Type)
                        If anAttribute.GetType().Equals(GetType(ormObjectAttribute)) Then
                            _ObjectAttribute = anAttribute
                            '** defaults
                            If Not _ObjectAttribute.HasValueClassname Then _ObjectAttribute.ClassName = Me.Type.FullName
                            If Not _ObjectAttribute.HasValueID Then _ObjectAttribute.ID = Me.Type.Name
                            If Not _ObjectAttribute.HasValueModulename Then _ObjectAttribute.Modulename = Me.Type.Namespace
                            If Not _ObjectAttribute.HasValueDescription Then _ObjectAttribute.Description = String.Empty
                            If Not _ObjectAttribute.HasValueUseCache Then _ObjectAttribute.UseCache = False
                            If Not _ObjectAttribute.HasValueIsBootstap Then _ObjectAttribute.IsBootstrap = False
                            If Not _ObjectAttribute.HasValueIsActive Then _ObjectAttribute.IsActive = True
                            If Not _ObjectAttribute.HasValueTitle Then _ObjectAttribute.Title = Me.Type.Name
                            If Not _ObjectAttribute.HasValueVersion Then _ObjectAttribute.Version = 1

                        End If
                    Next


                    If _ObjectAttribute Is Nothing Then
                        CoreMessageHandler(message:="Class has no attribute - not added to repository", argument:=Me.Type.Name, procedure:="ObjectClassDescription.initialize", _
                                           messagetype:=otCoreMessageType.InternalError, objectname:=Me.Type.Name)
                        Return False
                    End If
                    '*** get the Attributes in the fields
                    '***
                    aFieldList = Me.Type.GetFields(Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Public Or _
                    Reflection.BindingFlags.Static Or Reflection.BindingFlags.Instance Or _
                    Reflection.BindingFlags.FlattenHierarchy)



                    '** look into each Const Type (Fields) to check for tablenames first !
                    '**
                    Dim overridesFlag As Boolean = False

                    For Each aFieldInfo As System.Reflection.FieldInfo In aFieldList
                        If aFieldInfo.IsStatic AndAlso aFieldInfo.MemberType = Reflection.MemberTypes.Field Then
                            '** is this the declaring class ?! -> Do  override then
                            If aFieldInfo.DeclaringType = Me.Type Then
                                overridesFlag = True
                            Else
                                overridesFlag = False
                            End If
                            '** Attributes
                            For Each anAttribute As System.Attribute In Attribute.GetCustomAttributes(aFieldInfo)
                                '** Object Attribute as Const bound
                                If anAttribute.GetType().Equals(GetType(ormObjectAttribute)) Then
                                    InitializeObjectAttributeByField(attribute:=anAttribute, fieldinfo:=aFieldInfo)

                                    '*** Container ATTRIBUTES
                                ElseIf anAttribute.GetType().GetInterfaces.Contains(GetType(iormContainerAttribute)) Then
                                    ''' do we have the same const variable name herited from other classes ?
                                    ''' take then only the local / const variable with attributes from the herited class (overwriting)

                                    Dim localfield As FieldInfo = Me.Type.GetField(name:=aFieldInfo.Name, bindingAttr:=Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Public Or _
                                                  Reflection.BindingFlags.Static)
                                    If localfield Is Nothing OrElse (localfield IsNot Nothing AndAlso aFieldInfo.DeclaringType.Equals(localfield.ReflectedType)) Then
                                        If Not CType(anAttribute, iormContainerAttribute).HasValueContainerID Then aContainerID = aFieldInfo.GetValue(Nothing).ToString.ToUpper
                                        If CType(anAttribute, iormContainerAttribute).Enabled Then

                                            ''' container type
                                            ''' 
                                            If CType(anAttribute, iormContainerAttribute).ContainerType = otContainerType.EmbeddedObject Then
                                                InitializeEmbeddedContainerAttribute(attribute:=anAttribute, containerID:=aContainerID, overridesExisting:=overridesFlag)
                                            Else
                                                InitializeContainerAttribute(attribute:=anAttribute, containerID:=aContainerID, overridesExisting:=overridesFlag)
                                            End If

                                        End If
                                    End If
                                End If

                            Next
                        End If
                    Next


                    '**
                    '** look up the definitions
                    '**
                    '*** get the Attributes in the fields
                    '***
                    aFieldList = Me.Type.GetFields(Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Public Or _
                                                 Reflection.BindingFlags.Static Or Reflection.BindingFlags.FlattenHierarchy)

                    For Each aFieldInfo As System.Reflection.FieldInfo In aFieldList

                        If aFieldInfo.MemberType = Reflection.MemberTypes.Field Then
                            '* see if this class is the declaring one
                            If aFieldInfo.DeclaringType = Me.Type Then
                                overridesFlag = True
                                '*** if this class is a derived one - override an existing one
                            Else 'If aFieldInfo.ReflectedType = Me.Type Then
                                overridesFlag = False
                            End If

                            '** Attributes
                            For Each anAttribute As System.Attribute In Attribute.GetCustomAttributes(aFieldInfo)
                                '** split the container id if static value
                                aValue = String.Empty
                                If aFieldInfo.IsStatic Then
                                    '*
                                    If aFieldInfo.GetValue(Nothing) IsNot Nothing Then aValue = aFieldInfo.GetValue(Nothing).ToString.ToUpper
                                    '* split
                                    '* beware a container attribute would be lost
                                    Dim names As String() = Shuffle.NameSplitter(aValue)
                                    If names.Count > 1 Then
                                        aContainerID = names(0)
                                        aName = names(1)
                                    Else
                                        aContainerID = String.Empty
                                        aName = String.Empty
                                    End If
                                Else
                                    aContainerID = String.Empty
                                    aName = String.Empty
                                End If

                                '** Object Entry Column
                                '** 
                                If aFieldInfo.IsStatic AndAlso anAttribute.GetType().GetInterfaces.Contains(GetType(iormObjectEntryDefinition)) Then
                                    InitializeObjectEntryAttribute(attribute:=anAttribute, entryname:=aName, containerID:=aContainerID, fieldvalue:=aValue, _
                                                              overridesExisting:=overridesFlag)
                                    '** Foreign Keys
                                ElseIf aFieldInfo.IsStatic AndAlso anAttribute.GetType().Equals(GetType(ormForeignKeyAttribute)) Then
                                    InitializeForeignKeyAttribute(attribute:=anAttribute, name:=aName, containerID:=aContainerID, value:=aValue, overridesExisting:=overridesFlag)

                                    '** INDEX
                                ElseIf aFieldInfo.IsStatic AndAlso anAttribute.GetType().Equals(GetType(ormIndexAttribute)) Then
                                    InitializeIndexAttribute(attribute:=anAttribute, name:=aName, containerID:=aContainerID, value:=aValue, overridesExisting:=overridesFlag)

                                    '** Relation
                                ElseIf aFieldInfo.IsStatic AndAlso anAttribute.GetType().Equals(GetType(ormRelationAttribute)) Then
                                    InitializeRelationAttribute(attribute:=anAttribute, name:=aName, containerID:=aContainerID, value:=aValue, overridesExisting:=overridesFlag)

                                    '** Transaction
                                ElseIf aFieldInfo.IsStatic AndAlso anAttribute.GetType().Equals(GetType(ormObjectTransactionAttribute)) Then
                                    InitializeTransactionAttribute(attribute:=anAttribute, objectname:=aContainerID, name:=aName, value:=aValue, overridesExisting:=overridesFlag)

                                    '** Queries
                                ElseIf aFieldInfo.IsStatic AndAlso anAttribute.GetType().Equals(GetType(ormObjectQueryAttribute)) Then
                                    InitializeQueryAttribute(attribute:=anAttribute, queryname:=aName, value:=aValue, overridesExisting:=overridesFlag)

                                End If
                            Next
                        End If
                    Next

                    '*** get the Attributes in the mapping fields
                    '***
                    aFieldList = Me.Type.GetFields(Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Public Or _
                                    Reflection.BindingFlags.Instance Or Reflection.BindingFlags.FlattenHierarchy)
                    '**
                    '** lookup the mappings from the definitions
                    '**
                    For Each aFieldInfo As System.Reflection.FieldInfo In aFieldList

                        If aFieldInfo.MemberType = Reflection.MemberTypes.Field Then
                            '* see if ClassOverrides from higher classes
                            If aFieldInfo.DeclaringType = Me.Type Then
                                overridesFlag = False
                            ElseIf aFieldInfo.ReflectedType = Me.Type Then
                                overridesFlag = True
                            End If

                            '** Attributes
                            For Each anAttribute As System.Attribute In Attribute.GetCustomAttributes(aFieldInfo)
                                '** split the tablename if static value
                                aValue = String.Empty
                                If aFieldInfo.IsStatic Then
                                    If aFieldInfo.GetValue(Nothing) IsNot Nothing Then
                                        aValue = aFieldInfo.GetValue(Nothing).ToString.ToUpper
                                    End If

                                    '* split
                                    '* beware a tableattribute would be lost
                                    Dim names As String() = Shuffle.NameSplitter(aValue)
                                    If names.Count > 1 Then
                                        aContainerID = names(0)
                                        aName = names(1)
                                    Else
                                        aContainerID = String.Empty
                                        aName = aValue
                                    End If
                                Else
                                    aContainerID = String.Empty
                                    aName = String.Empty
                                End If

                                '** ENTRY MAPPING -> instance
                                '**
                                If anAttribute.GetType().Equals(GetType(ormObjectEntryMapping)) Then
                                    InitializeEntryMapping(attribute:=anAttribute, containerID:=aContainerID, fieldinfo:=aFieldInfo, value:=aValue, ClassOverrides:=overridesFlag)
                                End If

                            Next
                        End If
                    Next

                    '** get some of the methods hooks
                    Dim theMethods = Me.Type.GetMethods(bindingAttr:=BindingFlags.FlattenHierarchy Or BindingFlags.Public Or BindingFlags.NonPublic Or _
                    BindingFlags.Static Or BindingFlags.Instance)
                    For Each aMethodInfo In theMethods

                        '* see if this class is the declaring one
                        If aMethodInfo.DeclaringType = Me.Type Then
                            overridesFlag = True
                            '*** if this class is a derived one - override an existing one
                        Else
                            overridesFlag = False
                        End If


                        ''' LEGACY SPECIAL HOOKS TO RETRIEVE / CREATEDATAOBJECT 
                        'If aMethodInfo.Name.ToUpper = ConstMTRetrieve AndAlso aMethodInfo.IsGenericMethodDefinition Then
                        '    InitializeMethodRetrieveHook(methodinfo:=aMethodInfo)
                        'ElseIf aMethodInfo.Name.ToUpper = ConstMTCreateDataObject AndAlso aMethodInfo.IsGenericMethodDefinition Then
                        '    InitializeMethodCreateHook(methodinfo:=aMethodInfo)
                        'End If

                        '** Attributes
                        For Each anAttribute As System.Attribute In Attribute.GetCustomAttributes(aMethodInfo)
                            If anAttribute.GetType().Equals(GetType(ormObjectOperationMethodAttribute)) Then
                                InitializeOperationAttribute(attribute:=anAttribute, methodinfo:=aMethodInfo, overridesExisting:=overridesFlag)
                            End If
                        Next
                    Next

                End SyncLock


                _isInitalized = True
                Return True
            Catch ex As Exception
                Call CoreMessageHandler(procedure:="ObjectClassRepository.Initialize", exception:=ex)
                _isInitalized = False
                Return False
            End Try

        End Function

        ''' <summary>
        ''' generates an ILG Method Invoker from a method info
        ''' </summary>
        ''' <param name="methodInfo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function CreateILGMethodInvoker(methodInfo As MethodInfo) As OperationCallerDelegate
            Dim dynamicMethod As New DynamicMethod(String.Empty, GetType(Object), New Type() {GetType(Object), GetType(Object())}, methodInfo.DeclaringType.[Module])
            Dim il As ILGenerator = dynamicMethod.GetILGenerator()
            Dim ps As ParameterInfo() = methodInfo.GetParameters()
            Dim paramTypes As Type() = New Type(ps.Length - 1) {}

            For i As Integer = 0 To paramTypes.Length - 1
                If ps(i).ParameterType.IsByRef Then
                    paramTypes(i) = ps(i).ParameterType.GetElementType()
                Else
                    paramTypes(i) = ps(i).ParameterType
                End If
            Next

            Dim locals As LocalBuilder() = New LocalBuilder(paramTypes.Length - 1) {}

            For i As Integer = 0 To paramTypes.Length - 1
                locals(i) = il.DeclareLocal(paramTypes(i), True)
            Next
            For i As Integer = 0 To paramTypes.Length - 1
                il.Emit(OpCodes.Ldarg_1)
                EmitFastInt(il, i)
                il.Emit(OpCodes.Ldelem_Ref)
                EmitCastToReference(il, paramTypes(i))
                il.Emit(OpCodes.Stloc, locals(i))
            Next
            If Not methodInfo.IsStatic Then
                il.Emit(OpCodes.Ldarg_0)
            End If
            For i As Integer = 0 To paramTypes.Length - 1
                If ps(i).ParameterType.IsByRef Then
                    il.Emit(OpCodes.Ldloca_S, locals(i))
                Else
                    il.Emit(OpCodes.Ldloc, locals(i))
                End If
            Next
            If methodInfo.IsStatic Then
                il.EmitCall(OpCodes.[Call], methodInfo, Nothing)
            Else
                il.EmitCall(OpCodes.Callvirt, methodInfo, Nothing)
            End If
            If methodInfo.ReturnType = GetType(System.Void) Then
                il.Emit(OpCodes.Ldnull)
            Else
                EmitBoxIfNeeded(il, methodInfo.ReturnType)
            End If

            For i As Integer = 0 To paramTypes.Length - 1
                If ps(i).ParameterType.IsByRef Then
                    il.Emit(OpCodes.Ldarg_1)
                    EmitFastInt(il, i)
                    il.Emit(OpCodes.Ldloc, locals(i))
                    If locals(i).LocalType.IsValueType Then
                        il.Emit(OpCodes.Box, locals(i).LocalType)
                    End If
                    il.Emit(OpCodes.Stelem_Ref)
                End If
            Next

            il.Emit(OpCodes.Ret)
            Dim invoder As OperationCallerDelegate = DirectCast(dynamicMethod.CreateDelegate(GetType(OperationCallerDelegate)), OperationCallerDelegate)
            Return invoder
        End Function


        Private Shared Sub EmitCastToReference(il As ILGenerator, type As System.Type)
            If type.IsValueType Then
                il.Emit(OpCodes.Unbox_Any, type)
            Else
                il.Emit(OpCodes.Castclass, type)
            End If
        End Sub
        Private Shared Sub EmitBoxIfNeeded(il As ILGenerator, type As System.Type)
            If type.IsValueType Then
                il.Emit(OpCodes.Box, type)
            End If
        End Sub

        Private Shared Sub EmitFastInt(il As ILGenerator, value As Integer)
            Select Case value
                Case -1
                    il.Emit(OpCodes.Ldc_I4_M1)
                    Return
                Case 0
                    il.Emit(OpCodes.Ldc_I4_0)
                    Return
                Case 1
                    il.Emit(OpCodes.Ldc_I4_1)
                    Return
                Case 2
                    il.Emit(OpCodes.Ldc_I4_2)
                    Return
                Case 3
                    il.Emit(OpCodes.Ldc_I4_3)
                    Return
                Case 4
                    il.Emit(OpCodes.Ldc_I4_4)
                    Return
                Case 5
                    il.Emit(OpCodes.Ldc_I4_5)
                    Return
                Case 6
                    il.Emit(OpCodes.Ldc_I4_6)
                    Return
                Case 7
                    il.Emit(OpCodes.Ldc_I4_7)
                    Return
                Case 8
                    il.Emit(OpCodes.Ldc_I4_8)
                    Return
            End Select

            If value > -129 AndAlso value < 128 Then
                il.Emit(OpCodes.Ldc_I4_S, Convert.ToSByte(value))
            Else
                il.Emit(OpCodes.Ldc_I4, value)
            End If
        End Sub

        '
        Public Shared Function CreateILGCreateInstanceDelegate(constructor As ConstructorInfo, delegateType As Type) As CreateInstanceDelegate
            If constructor Is Nothing Then
                Throw New ArgumentNullException("constructor")
            End If
            If delegateType Is Nothing Then
                Throw New ArgumentNullException("delegateType")
            End If

            ' Validate the delegate return type
            Dim delMethod As MethodInfo = delegateType.GetMethod("Invoke")
            'If delMethod.ReturnType <> constructor.DeclaringType Then
            '       Throw New InvalidOperationException("The return type of the delegate must match the constructors declaring type")
            'End If

            ' Validate the signatures
            Dim delParams As ParameterInfo() = delMethod.GetParameters()
            Dim constructorParam As ParameterInfo() = constructor.GetParameters()
            If delParams.Length <> constructorParam.Length Then
                Throw New InvalidOperationException("The delegate signature does not match that of the constructor")
            End If
            For i As Integer = 0 To delParams.Length - 1
                ' Probably other things we should check ??
                If delParams(i).ParameterType <> constructorParam(i).ParameterType OrElse delParams(i).IsOut Then
                    Throw New InvalidOperationException("The delegate signature does not match that of the constructor")
                End If
            Next
            ' Create the dynamic method
            Dim method As New DynamicMethod(String.Format("{0}__{1}", constructor.DeclaringType.Name, Guid.NewGuid().ToString().Replace("-", String.Empty)), constructor.DeclaringType, Array.ConvertAll(Of ParameterInfo, Type)(constructorParam, Function(p) p.ParameterType), True)

            ' Create the il
            Dim gen As ILGenerator = method.GetILGenerator()
            For i As Integer = 0 To constructorParam.Length - 1
                If i < 4 Then
                    Select Case i
                        Case 0
                            gen.Emit(OpCodes.Ldarg_0)
                            Exit Select
                        Case 1
                            gen.Emit(OpCodes.Ldarg_1)
                            Exit Select
                        Case 2
                            gen.Emit(OpCodes.Ldarg_2)
                            Exit Select
                        Case 3
                            gen.Emit(OpCodes.Ldarg_3)
                            Exit Select
                    End Select
                Else
                    gen.Emit(OpCodes.Ldarg_S, i)
                End If
            Next
            gen.Emit(OpCodes.Newobj, constructor)
            gen.Emit(OpCodes.Ret)

            ' Return the delegate :)
            Return DirectCast(method.CreateDelegate(delegateType), CreateInstanceDelegate)

        End Function


        ''' <summary>
        ''' Searches an instanceType constructor with delegateType-matching signature and constructs delegate of delegateType creating new instance of instanceType.
        ''' Instance is casted to delegateTypes's return type. 
        ''' Delegate's return type must be assignable from instanceType.
        ''' </summary>
        ''' <param name="delegateType">Type of delegate, with constructor-corresponding signature to be constructed.</param>
        ''' <param name="instanceType">Type of instance to be constructed.</param>
        ''' <returns>Delegate of delegateType wich constructs instance of instanceType by calling corresponding instanceType constructor.</returns>
        Public Shared Function CreateLambdaInstance(delegateType As Type, instanceType As Type) As [Delegate]

            If Not GetType([Delegate]).IsAssignableFrom(delegateType) Then
                Throw New ArgumentException([String].Format("{0} is not a Delegate type.", delegateType.FullName), "delegateType")
            End If

            Dim invoke = delegateType.GetMethod("Invoke")
            Dim parameterTypes = invoke.GetParameters().[Select](Function(pi) pi.ParameterType).ToArray()
            Dim resultType = invoke.ReturnType
            If Not resultType.IsAssignableFrom(instanceType) Then
                Throw New ArgumentException([String].Format("Delegate's return type ({0}) is not assignable from {1}.", resultType.FullName, instanceType.FullName))
            End If

            Dim ctor = instanceType.GetConstructor(BindingFlags.Instance Or BindingFlags.[Public] Or BindingFlags.NonPublic, Nothing, parameterTypes, Nothing)
            If ctor Is Nothing Then
                Throw New ArgumentException("Can't find constructor with delegate's signature", "instanceType")
            End If

            Dim parapeters = parameterTypes.[Select](Function(x) Expression.Parameter(x)).ToArray()

            Dim newExpression = Expression.Lambda(delegateType, Expression.Convert(Expression.[New](ctor, parapeters), resultType), parapeters)
            Dim [delegate] = newExpression.Compile()
            Return [delegate]
        End Function
        ''' <summary>
        ''' create Instance
        ''' </summary>
        ''' <typeparam name="TDelegate"></typeparam>
        ''' <param name="instanceType"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Dim newList = Constructor.Compile(Of Func(Of Integer, IList(Of [String])))(GetType(List(Of [String])))
        ''' Dim list = newList(100)
        ''' </remarks>
        Public Shared Function CreateLambdaInstance(Of TDelegate)(instanceType As Type) As TDelegate
            Return DirectCast(DirectCast(CreateLambdaInstance(GetType(TDelegate), instanceType), Object), TDelegate)
        End Function
        ''' <summary>
        ''' Creates a IL GET VALUE
        ''' </summary>
        ''' <typeparam name="T">Type of the class of the setter variable</typeparam>
        ''' <typeparam name="TValue">Type of the value</typeparam>
        ''' <param name="field">fieldinfo </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function CreateILGGetterDelegate(Of T, TValue)(tclass As Type, field As FieldInfo) As MappingGetterDelegate
            Try
                Dim m As New DynamicMethod("getter", GetType(TValue), New Type() {GetType(T)}, tclass)
                Dim cg As ILGenerator = m.GetILGenerator()

                ' Push the current value of the id field onto the 
                ' evaluation stack. It's an instance field, so load the
                ' instance  before accessing the field.
                cg.Emit(OpCodes.Ldarg_0)
                cg.Emit(OpCodes.Castclass, field.DeclaringType) 'cast the parameter of type object to the type containing the field

                cg.Emit(OpCodes.Ldfld, field)
                If field.FieldType.IsValueType Then
                    cg.Emit(OpCodes.Box, field.FieldType) 'box the value type, so you will have an object on the stack
                End If

                ' return
                cg.Emit(OpCodes.Ret)


                Return DirectCast(m.CreateDelegate(GetType(MappingGetterDelegate)), MappingGetterDelegate)
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectClassDescription.CreateILGetterDelegate")
                Return Nothing
            End Try

        End Function
        ''' <summary>
        ''' Creates a IL SET VALUE
        ''' </summary>
        ''' <typeparam name="T">Type of the class of the setter variable</typeparam>
        ''' <typeparam name="TValue">Type of the value</typeparam>
        ''' <param name="field">fieldinfo </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function CreateILGSetterDelegate(Of T, TValue)(tclass As Type, field As FieldInfo) As Action(Of T, TValue)
            Try
                Dim m As New DynamicMethod("setter", GetType(System.Void), New Type() {GetType(T), GetType(TValue)}, tclass)
                Dim cg As ILGenerator = m.GetILGenerator()

                ' Load the instance , load the new value 
                ' of id, and store the new field value. 
                cg.Emit(OpCodes.Ldarg_0)
                cg.Emit(OpCodes.Castclass, field.DeclaringType) ' cast the parameter of type object to the type containing the field

                cg.Emit(OpCodes.Ldarg_1)
                If field.FieldType.IsValueType Then
                    cg.Emit(OpCodes.Unbox_Any, field.FieldType) ' unbox the value parameter to the value-type
                Else
                    cg.Emit(OpCodes.Castclass, field.FieldType) 'cast the value on the stack to the field type
                End If


                cg.Emit(OpCodes.Stfld, field)

                ' return
                cg.Emit(OpCodes.Ret)


                Return DirectCast(m.CreateDelegate(GetType(Action(Of T, TValue))), Action(Of T, TValue))
            Catch ex As Exception
                CoreMessageHandler(exception:=ex, procedure:="ObjectClassDescription.CreateILGSetterDelegate")
                Return Nothing
            End Try

        End Function

        'Private Shared Function CreateExpressionSetter(of T)(field As FieldInfo) As Object
        '    Dim targetExp As ParameterExpression = Expression.Parameter(GetType(T), "target")
        '    Dim valueExp As ParameterExpression = Expression.Parameter(GetType(String), "value")

        '    ' Expression.Property can be used here as well
        '    Dim fieldExp As MemberExpression = Expression.Field(targetExp, field)
        '    Dim assignExp As BinaryExpression = Expression.Assign(fieldExp, valueExp)

        '    Dim setter = Expression.Lambda(Of Action(Of T, String))(assignExp, targetExp, valueExp).Compile()

        '    setter(subject, "new value")
        'End Function


        'Private Shared Sub Main()
        '    Dim f As FieldInfo = GetType(MyObject).GetField("MyField")

        '    Dim setter As Action(Of MyObject, Integer) = CreateILGSetterDelegate(Of MyObject, Integer)(f)

        '    Dim obj = New MyObject()
        '    obj.MyField = 10

        '    setter(obj, 42)

        '    Console.WriteLine(obj.MyField)
        '    Console.ReadLine()
        'End Sub
    End Class

End Namespace