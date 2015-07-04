Option Explicit On

REM ***********************************************************************************************************************************************''' <summary>
REM *********** ON TRACK DATABASE BACKEND LIBRARY
REM ***********
REM *********** PROJECT BUSINESS OBJECT DEFINITION Classes 
REM ***********
REM *********** Version: X.YY
REM *********** Created: 2014-08-29
REM *********** Last Change:
REM ***********
REM *********** Change Log:
REM ***********
REM *********** (C) by Boris Schneider 2014
REM ***********************************************************************************************************************************************''' <summary>

Imports System.Diagnostics.Debug
Imports System.Text.RegularExpressions
Imports OnTrack.Database
Imports OnTrack.Commons
Imports OnTrack.ObjectProperties
Imports OnTrack.Core

Namespace OnTrack.Deliverables

    ''' <summary>
    ''' Definition class for projects
    ''' </summary>
    ''' <remarks>
    ''' Design Principles:
    ''' 
    ''' 1) This is a subclass of the deliverable class
    ''' 2) The class serializes in 2 Tables which are linked with a foreign key but have different primary keys. 
    ''' 3) Therefore the project has 2 primary keys: the deliverable UID and the ID of the project
    ''' </remarks>
    ''' 
    <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleDeliverables, Version:=2, Release:=0, patch:=0, changeimplno:=3, _
            description:="Introducing Project as deliverables")> _
    <ormObject(id:=Project.ConstObjectID, description:="definition of a project", _
        modulename:=ConstModuleDeliverables, Version:=1, useCache:=True, adddeletefieldbehavior:=True, addDomainBehavior:=False)> _
    Public Class Project
        Inherits Deliverable

        Public Shadows Const ConstObjectID = "Project"

        ''' <summary>
        ''' Define a second Table
        ''' </summary>
        ''' <remarks></remarks>
        <ormChangeLogEntry(Application:=ConstAssemblyName, Module:=ConstModuleRepository, Version:=ConstOTDBSchemaVersion, Release:=0, patch:=0, changeimplno:=2, _
           description:="added secondary table " & ConstProjectTableID)> _
        <ormTableAttribute(version:=1, usecache:=True)> Public Const ConstProjectTableID = "TBLPROJECTS"

        '** indexes
        <ormIndex(columnName1:=ConstFNDLVUID, columnname2:=constFNID, columnname3:=ConstFNIsDeleted, tableid:=ConstProjectTableID)> _
        Public Const ConstIndexDeliverables = "INDDELIVERABLES"

        ''' <summary>
        ''' keys
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(Datatype:=otDataType.Text, size:=20, PrimaryKeyOrdinal:=1, ContainerID:=ConstProjectTableID, _
            dbdefaultvalue:="", _
            properties:={ObjectEntryProperty.Keyword}, validationPropertystrings:={ObjectValidationProperty.NotEmpty}, _
           title:="ID", description:="id of the project", XID:="PRJ1")> Public Const ConstFNID = "ID"


        ''' <summary>
        ''' Fields
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        ''' <summary>
        ''' Link the secondary Table to the primary via foreign key
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=Deliverable.ConstObjectID & "." & Deliverable.ConstFNDLVUID, ContainerID:=ConstProjectTableID, _
            title:="UID", description:="uid of the deliverable of this project", XID:="PRJ2", isnullable:=True, _
            foreignkeyProperties:={ForeignKeyProperty.PrimaryTableLink}, useforeignkey:=otForeignKeyImplementation.NativeDatabase)> _
        Public Const ConstFNDLVUID = "DLVUID"

        <ormObjectEntry(Datatype:=otDataType.Text, isnullable:=True, ContainerID:=ConstProjectTableID, _
         title:="Description", description:="short description of the project", XID:="PRJ3")> Public Const constFNDescription = "DESC"

        <ormObjectEntry(Datatype:=otDataType.Memo, isnullable:=True, ContainerID:=ConstProjectTableID, _
            title:="comment", description:="long description of the project", XID:="PRJ10")> Public Const constFNComment = "CMT"

        ''' <summary>
        ''' Domain
        ''' </summary>
        ''' <remarks></remarks>
        <ormObjectEntry(referenceObjectEntry:=Domain.ConstObjectID & "." & Domain.ConstFNDomainID, ContainerID:=ConstProjectTableID, _
            useforeignkey:=otForeignKeyImplementation.NativeDatabase, _
            foreignkeyProperties:={ForeignKeyProperty.OnDelete & "(" & ForeignKeyActionProperty.SetDefault & ")", _
                                    ForeignKeyProperty.OnUpdate & "(" & ForeignKeyActionProperty.Cascade & ")"})> _
        Public Const ConstFNDomainID = Domain.ConstFNDomainID

        '*** Mapping
        <ormObjectEntryMapping(EntryName:=constFNID)> Private _id As String = String.Empty
        <ormObjectEntryMapping(EntryName:=ConstFNDLVUID)> Private _dlvuid As Long?
        <ormObjectEntryMapping(EntryName:=constFNDescription)> Private _description As String
        <ormObjectEntryMapping(EntryName:=constFNComment)> Private _comment As String

    End Class
End Namespace