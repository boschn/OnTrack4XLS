
REM ***********************************************************************************************************************************************
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** BUSINESS OBJECTs: CONFIGURABLES Classes 
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2013-08-08
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** TO DO Log:
REM ***********             -
REM ***********
REM *********** (C) by Boris Schneider 2013
REM ***********************************************************************************************************************************************

Option Explicit On
Imports System.Diagnostics
Imports System.Linq
Imports System.Diagnostics.Debug

Imports OnTrack
Imports OnTrack.Core
Imports OnTrack.Database
Imports OnTrack.ObjectProperties
Imports OnTrack.Scheduling
Imports OnTrack.Deliverables
Imports OnTrack.Commons

Namespace OnTrack.Configurables
    ''' <summary>
    ''' Enumeration and other definitions
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum otConfigConditionRuleType
        FindConfigSet = 1
    End Enum

    ''' <summary>
    ''' class to define a configuration which is able to dynamically associated other business objects by conditions
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=Configuration.ConstObjectID, version:=1, adddomainbehavior:=True, adddeletefieldbehavior:=True, usecache:=True, _
        modulename:=ConstModuleConfiguration, Title:="Configuration", description:="definition of a configuration")> _
    Public Class Configuration
        Inherits ormBusinessObject

        Public Const ConstObjectID = "Configuration"

        ''' <summary>
        ''' Table Definition
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=1, usecache:=True)> Public Const ConstPrimaryTableID = "TBLDEFCONFIGURATION"

        ''' <summary>
        ''' primary keys
        ''' </summary>
        ''' <remarks></remarks>
        ''' 

        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=1, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty},
            XID:="CNF1", title:="Configuration UID", description:="UID of the configuration")> Public Const ConstFNConfigUID = "UID"


        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=2 _
       , useforeignkey:=otForeignKeyImplementation.NativeDatabase, defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        ''' <summary>
        ''' fields
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, size:=50, _
            properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty},
            XID:="CNF2", title:="Configuration ID", description:="ID of the configuration")> Public Const constFNConfigID = "ID"


        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
          XID:="CNF3", title:="Description", description:="description of the configuration")> Public Const ConstFNDescription = "DESC"

        <ormObjectEntry(Datatype:=otDataType.List, isnullable:=True, _
          XID:="CNF4", title:="Properties", description:="properties of the configuration")> Public Const ConstFNProperties = "PROPERTIES"

        <ormObjectEntry(Datatype:=otDataType.List, isnullable:=True, _
             properties:={ObjectEntryProperty.Keyword}, validationPropertyStrings:={ObjectValidationProperty.NotEmpty}, _
             defaultvalue:={Deliverable.ConstObjectID}, _
             values:={Deliverable.ConstObjectID, Parts.Part.ConstObjectID}, _
             XID:="CNF5", title:="Business Objects", description:="applicable business objects for this configuration")> _
        Public Const ConstFNObjects = "OBJECTS"

        ''' <summary>
        ''' Mappings
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntryMapping(EntryName:=ConstFNConfigUID)> Private _uid As Long
        <ormObjectEntryMapping(EntryName:=constFNConfigID)> Private _id As String = String.empty
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _description As String = String.empty
        <ormObjectEntryMapping(EntryName:=ConstFNProperties)> Private _properties As String()
        <ormObjectEntryMapping(EntryName:=ConstFNObjects)> Private _objects As String()

        ''' <summary>
        ''' Relations
        ''' </summary>
        ''' <remarks></remarks>
        <ormRelation(linkobject:=GetType(ConfigItemSelector), cascadeOnDelete:=True, cascadeOnUpdate:=True, _
            fromEntries:={constFNConfigID}, toEntries:={ConfigItemSelector.ConstFNConfiguID})> Public Const ConstREntities = "CONFIGCONDITION"

        <ormObjectEntryMapping(RelationName:=ConstREntities, infuseMode:=otInfuseMode.OnInject Or otInfuseMode.OnDemand, _
            keyentries:={ConfigItemSelector.ConstFNIDNO})> Private WithEvents _conditionCollection As New ormRelationCollection(Of ConfigItemSelector)(Me, {ConfigItemSelector.ConstFNIDNO})

#Region "Properties"

        '' <summary>
        ''' Gets or sets the properties.
        ''' </summary>
        ''' <value>The properties.</value>
        Public Property Objects() As String()
            Get
                Return Me._objects
            End Get
            Set(value As String())
                SetValue(ConstFNObjects, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the properties.
        ''' </summary>
        ''' <value>The properties.</value>
        Public Property Properties() As String()
            Get
                Return Me._properties
            End Get
            Set(value As String())
                SetValue(ConstFNProperties, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the description.
        ''' </summary>
        ''' <value>The description.</value>
        Public Property Description() As String
            Get
                Return Me._description
            End Get
            Set(value As String)
                SetValue(ConstFNDescription, value)
            End Set
        End Property

        ''' <summary>
        ''' gets the UID of the configuration
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property UID() As Long

            Get
                Return _uid
            End Get
        End Property

        ''' <summary>
        ''' gets or sets the ID of the configuration
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ID() As String

            Get
                Return _id
            End Get
            Set(value As String)
                SetValue(constFNConfigID, value)
            End Set
        End Property

        ''' <summary>
        ''' returns the Entities of this Section
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Conditions As ormRelationCollection(Of ConfigItemSelector)
            Get
                Return _conditionCollection
            End Get
        End Property

#End Region
        ''' <summary>
        ''' retrieve  the configuration from store
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(uid As Long, Optional domainid As String = Nothing)
            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID
            Return ormBusinessObject.RetrieveDataObject(Of Configuration)(pkArray:={uid, domainid.ToUpper}, domainID:=domainid)
        End Function


        ''' <summary>
        ''' handler for onCreating Event - generates unique primary key values
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Shadows Sub OnCreating(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnCreating
            Dim uid As Long? = e.Record.GetValue(ConstFNConfigUID)
            Dim primarykey As Object() = {uid}

            If Not uid.HasValue OrElse uid = 0 Then
                uid = Nothing

                If e.DataObject.ObjectPrimaryContainerStore.CreateUniquePkValue(pkArray:=primarykey) Then
                    e.Record.SetValue(ConstFNConfigUID, primarykey(0))
                    e.Result = True
                    e.Proceed = True
                Else
                    CoreMessageHandler(message:="primary keys could not be created ?!", procedure:="Configuration.OnCreate", messagetype:=otCoreMessageType.InternalError)
                End If
            End If
        End Sub
        ''' <summary>
        ''' creates a persistable configuration
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(id As String, Optional uid As Long = 0, Optional domainid As String = Nothing)
            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID
            Dim aRecord As New ormRecord
            With aRecord
                .SetValue(constFNConfigID, id)
                .SetValue(ConstFNConfigUID, uid)
                .SetValue(ConstFNDomainID, domainid)
            End With
            Return ormBusinessObject.CreateDataObject(Of Configuration)(aRecord, domainID:=domainid, checkUnique:=True)
        End Function



    End Class

    ''' <summary>
    ''' class to define a configuration condition which enables the configuration to retrieve associated business objects
    ''' </summary>
    ''' <remarks></remarks>
    <ormObject(id:=ConfigItemSelector.ConstObjectID, version:=1, adddomainbehavior:=True, adddeletefieldbehavior:=True, usecache:=True, _
        modulename:=ConstModuleConfiguration, Title:="Configuration Condition", description:="definition of a configuration condition")> _
    Public Class ConfigItemSelector
        Inherits ormBusinessObject

        Public Shadows Const ConstObjectID = "ConfigItemSelector"

        ''' <summary>
        ''' Table Definition
        ''' </summary>
        ''' <remarks></remarks>
        <ormTableAttribute(version:=1, usecache:=True)> Public Shadows Const ConstPrimaryTableID = "tblDefConfigSConditions"

        ''' <summary>
        ''' primary keys
        ''' </summary>
        ''' <remarks></remarks>

        <ormObjectEntry(referenceObjectEntry:=Configuration.ConstObjectID & "." & Configuration.ConstFNConfigUID, PrimaryKeyOrdinal:=1 _
         , defaultvalue:=ConstGlobalDomain)> Public Const ConstFNConfiguID = Configuration.ConstFNConfigUID

        <ormObjectEntry(Datatype:=otDataType.Long, PrimaryKeyOrdinal:=2, _
         XID:="CCOND2", title:="ID", description:="ID of the configuration condition")> Public Const ConstFNIDNO = "IDNO"

        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, PrimaryKeyOrdinal:=3 _
         , useforeignkey:=otForeignKeyImplementation.None, defaultvalue:=ConstGlobalDomain)> Public Const ConstFNDomainID = Domain.ConstFNDomainID

        '*** foreign key
        <ormForeignKey(entrynames:={ConstFNConfiguID, ConstFNDomainID}, _
            foreignkeyreferences:={Configuration.ConstObjectID & "." & Configuration.ConstFNConfigUID, Configuration.ConstObjectID & "." & Configuration.ConstFNDomainID}, _
            useforeignkey:=otForeignKeyImplementation.NativeDatabase)> Public Const constFKConditions = "FK_ConfigConditions_Configs"

        ''' <summary>
        ''' other fields
        ''' </summary>
        ''' <remarks></remarks>
        ''' 

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, defaultvalue:=otConfigConditionRuleType.FindConfigSet, _
         XID:="CCOND3", title:="RuleType", description:="rule type of the configuration condition")> Public Const ConstFNRuletype = "ruletype"

        <ormObjectEntry(Datatype:=otDataType.Long, defaultvalue:=10, dbdefaultvalue:="10", _
         XID:="CCOND4", title:="Ordinal", description:="ordinal of the configuration condition")> Public Const ConstFNOrdinal = "Ordinal"

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, _
          XID:="CCOND5", title:="Description", description:="description of the configuration condition")> Public Const ConstFNDescription = "DESC"

        <ormObjectEntry(Datatype:=otDataType.List, isnullable:=True, _
          XID:="CCOND6", title:="Properties", description:="properties of the configuration condition")> Public Const ConstFNProperties = "PROPERTIES"


        ''' <summary>
        ''' Mappings
        ''' </summary>
        ''' <remarks></remarks>

        <ormObjectEntryMapping(EntryName:=ConstFNConfiguID)> Private _configID As Long
        <ormObjectEntryMapping(entryname:=ConstFNIDNO)> Private _id As Long
        <ormObjectEntryMapping(EntryName:=ConstFNRuletype)> Private _ruletype As otConfigConditionRuleType
        <ormObjectEntryMapping(EntryName:=ConstFNDescription)> Private _description As String
        <ormObjectEntryMapping(EntryName:=ConstFNProperties)> Private _properties As String()


#Region "Properties"
        ''' <summary>
        ''' Gets or sets the ruletype.
        ''' </summary>
        ''' <value>The ruletype.</value>
        Public Property Ruletype() As otConfigConditionRuleType
            Get
                Return Me._ruletype
            End Get
            Set(value As otConfigConditionRuleType)
                SetValue(ConstFNRuletype, value)
            End Set
        End Property

        ''' <summary>
        ''' Gets the entity ID.
        ''' </summary>
        ''' <value>The entity.</value>
        Public ReadOnly Property ID() As Long
            Get
                Return Me._id
            End Get
        End Property

        ''' <summary>
        ''' returns the ID of the section
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ConfigurationID As Long
            Get
                Return _configID
            End Get
        End Property

        ''' <summary>
        ''' gets or sets the description (nothing)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Description As String
            Get
                Return _description
            End Get
            Set(value As String)
                SetValue(ConstFNDescription, Description)
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the properties.
        ''' </summary>
        ''' <value>The properties.</value>
        Public Property Properties() As String()
            Get
                Return Me._properties
            End Get
            Set(value As String())
                Me._properties = value
            End Set
        End Property


#End Region

        ''' <summary>
        ''' Handles OnCreating and Relation to ConfigSection
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ConfigCondition_OnCreating(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnCreating
            Dim my As ConfigItemSelector = TryCast(e.DataObject, ConfigItemSelector)

            If my IsNot Nothing Then
                Dim configuid As Long? = e.Record.GetValue(ConstFNConfiguID)
                If configuid Is Nothing Then
                    CoreMessageHandler(message:="section does not exist", procedure:="ConfigEntity.OnCreating", _
                                       messagetype:=otCoreMessageType.ApplicationError, _
                                       argument:=my.ConfigurationID)
                    e.AbortOperation = True
                    Return
                End If
                Dim mySection As Configuration = Configuration.Retrieve(uid:=configuid)
                If mySection Is Nothing Then
                    CoreMessageHandler(message:="section does not exist", procedure:="ConfigEntity.OnCreating", _
                                       messagetype:=otCoreMessageType.ApplicationError, _
                                       argument:=my.ConfigurationID)
                    e.AbortOperation = True
                    Return
                End If

                Dim idno As Long? = e.Record.GetValue(ConstFNIDNO)
                If Not idno.HasValue OrElse idno = 0 Then
                    Dim primarykey As Object() = {configuid, idno}
                    If e.DataObject.ObjectPrimaryContainerStore.CreateUniquePkValue(pkArray:=primarykey) Then
                        e.Record.SetValue(ConstFNIDNO, primarykey(1))
                        e.Result = True
                        e.Proceed = True
                    Else
                        CoreMessageHandler(message:="primary keys could not be created ?!", procedure:="ConfigCondition.OnCreate", _
                                           messagetype:=otCoreMessageType.InternalError)
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Handles OnCreating and Relation to ConfigSection
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub ConfigCondition_OnInfused(sender As Object, e As ormDataObjectEventArgs) Handles MyBase.OnInfused
            Dim my As ConfigItemSelector = TryCast(e.DataObject, ConfigItemSelector)

            If my IsNot Nothing Then
                Dim myConfiguration As Configuration = Configuration.Retrieve(uid:=my.ConfigurationID)
                If myConfiguration Is Nothing Then
                    CoreMessageHandler(message:="section does not exist", procedure:="ConfigEntity.OnCreating", _
                                       messagetype:=otCoreMessageType.ApplicationError, _
                                       argument:=my.ConfigurationID)
                    e.AbortOperation = True
                    Return
                Else
                    myConfiguration.Conditions.Add(my)
                End If
            End If
        End Sub
        ''' <summary>
        ''' create a persistable ConfigEntity
        ''' </summary>
        ''' <param name="Section"></param>
        ''' <param name="Entity"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Create(configid As Long, Optional id As Long = 0, Optional domainid As String = Nothing) As ConfigItemSelector
            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey As Object() = {configid, id, domainid}
            Return ormBusinessObject.CreateDataObject(Of ConfigItemSelector)(pkArray:=primarykey, domainID:=domainid, checkUnique:=True)
        End Function

        ''' <summary>
        ''' create a persistable ConfigEntity
        ''' </summary>
        ''' <param name="Section"></param>
        ''' <param name="Entity"></param>
        ''' <param name="domainid"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Retrieve(configid As Long, id As Long, Optional domainid As String = Nothing) As ConfigItemSelector
            If String.IsnullorEmpty(domainID) Then domainid = CurrentSession.CurrentDomainID
            Dim primarykey As Object() = {configid, id, domainid}
            Return ormBusinessObject.RetrieveDataObject(Of ConfigItemSelector)(pkArray:=primarykey)
        End Function
    End Class

End Namespace
