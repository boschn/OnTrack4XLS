/**
 *  ONTRACK DATABASE
 *  
 *  DECLARATION
 * 
 * Version: 1.0
 * Created: 2015-04-14
 * Last Change
 * 
 * Change Log
 * 
 * (C) by Boris Schneider, 2015
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using OnTrack.Rulez.eXPressionTree;

namespace OnTrack.Core
{
    /// <summary>
    /// declares a data object meta description
    /// </summary>
    public interface iObjectDefinition
    {
        /// <summary>
        /// gets the object name
        /// </summary>
        String Objectname { get; }

        /// <summary>
        /// gets the System.Type of the object implementation class
        /// </summary>
        System.Type ObjectType { get;  }

       

        /// <summary>
        /// gets the module name space
        /// </summary>
        String Modulename { get; set; }
        /// <summary>
        /// gets the .net class name
        /// </summary>
        String Classname { get; set; }
        /// <summary>
        /// gets the description
        /// </summary>
        String Description { get; set; }
        /// <summary>
        /// gets or sets the Properties of the object
        /// </summary>
        String[] Properties { get; set; }
        /// <summary>
        /// gets or sets the Version of the object
        /// </summary>
        long Version { get; set; }
        /// <summary>
        /// gets or sets the active / enabled flag
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// gets or sets the unique key entry names
        /// </summary>
        String[] Keys { get; set; }
        /// <summary>
        /// returns a List of iObjectEntryDefinitions
        /// </summary>
        IList<iObjectEntryDefinition> iObjectEntryDefinitions { get; }
        /// <summary>
        /// returns the (active) names of the Entries
        /// </summary>
        /// <param name="onlyActive"></param>
        /// <returns></returns>
        IList<String> Entrynames(bool onlyActive = true);
        /// <summary>
        /// returns an EntryDefinition or null
        /// </summary>
        /// <param name="entryname"></param>
        /// <returns></returns>
        iObjectEntryDefinition GetiEntryDefinition(string entryname);
        /// <summary>
        /// returns true if the entry name exists
        /// </summary>
        bool HasEntry(String entryname);
    }

    /// <summary>
/// Data Types for OnTrack Database Fields
/// </summary>
/// <remarks></remarks>

[TypeConverter(typeof(long))]
    public enum otDataType
    {   
            @Void = 0,
            Numeric = 1,
            List = 2,
            Text = 3,
            Runtime = 4,
            Formula = 5,
            Date = 6,
            Long = 7,
            Timestamp = 8,
            Bool = 9,
            Memo = 10,
            Binary = 11,
            Time = 12,
            Money = 13, // not yet implemented
    }   
         
    /// <summary>
    /// Interface for Object Entries
    /// </summary>
    /// <remarks></remarks>
    public interface iObjectEntryDefinition {
        
        /// <summary>
        /// returns true if the Entry is mapped to a class member field
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        bool IsMapped { get; set; }
        /// <summary>
        /// gets the lower range Value
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        long? LowerRangeValue {get;set;}
        /// <summary>
        /// gets the upper range Value
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        long? UpperRangeValue {get;set;}
        /// <summary>
        /// gets the list of possible values
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        List<String> PossibleValues {get;set;}
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        String Description {get;set;}
        /// <summary>
        /// sets or gets the object name of the entry
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        String Objectname {get;set;}
        /// <summary>
        /// returns the name of the entry
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        String Entryname {get;set;}
        /// <summary>
        /// returns the field data type
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        otDataType Datatype {get;set;}
        /// <summary>
        /// returns version
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        long Version {get;set;}
        /// <summary>
        /// returns Title (Column Header)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        String Title {get;set;}
        /// <summary>
        /// sets or gets the default value for the object entry
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        Object DefaultValue {get;set;}
        /// <summary>
        /// set or gets true if the entry value is nullable
        /// </summary>
        bool IsNullable {get;set;}
        /// <summary>
        /// gets or sets the Primary key Ordinal of the Object Entry (if set this is part of a key)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        long  PrimaryKeyOrdinal  {get;set;}
        /// <summary>
        /// returns the inner data type
        /// </summary>
        otDataType? InnerDatatype {get;set;}
        /// <summary>
        /// returns the ordinal
        /// </summary>
        long Ordinal { get; set; }
        /// <summary>
        /// get or sets true if the entry is readonly
        /// </summary>
        bool IsReadonly {get;set;}
        /// <summary>
        /// get or sets true if the entry is active
        /// </summary>
        bool IsActive  {get;set;}
        /// <summary>
        /// gets the ObjectDefinition
        /// </summary>
        /// <returns></returns>
        iObjectDefinition ObjectDefinition {get;}
    }
      
    /// <summary>
    /// declares an data object Engine
    /// </summary>
    public interface iDataObjectEngine
    {
        /// <summary>
        /// gets the unique ID of the Engine
        /// </summary>
        String ID { get; }

        /// <summary>
        /// gets the Repository
        /// </summary>
        iDataObjectRepository Objects { get;}

        /// <summary>
        /// generate a rule
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        bool Generate(IRule rule, out OnTrack.Rulez.ICodeBit result);

        /// <summary>
        /// run a rule by handle
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        bool Run(String id, OnTrack.Rulez.Context context);
    }
    /// <summary>
    /// declares an repository for data objects 
    /// </summary>
    public interface iDataObjectRepository
    {
        /// <summary>
        /// returns a object definition
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        iObjectDefinition GetIObjectDefinition(string id);
        /// <summary>
        /// returns a object definition
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        iObjectDefinition GetIObjectDefinition(Type type);
        /// <summary>
        /// returns the objectname from a type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        String GetObjectname(Type type);
        /// <summary>
        /// returns the objectname from a type name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        String GetObjectname(String typefullname);
        /// <summary>
        /// returns the objectname from a type name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        System.Type GetObjectType(String objectname);
        /// <summary>
        /// returns true if the object handle exists
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        bool HasObjectDefinition(string id);
        bool HasObjectDefinition(System.Type type);
        /// <summary>
        /// returns a list of ObjectDefinitions
        /// </summary>
        IEnumerable<iObjectDefinition> IObjectDefinitions { get; }
        /// <summary>
        /// returns an enumerable of Data Object providers
        /// </summary>
        IEnumerable<iDataObjectProvider> DataObjectProviders { get; }
    }
 
    /// <summary>
    /// describes a data object
    /// </summary>
    public interface iDataObject
    {
        /// <summary>
        /// gets the Object Definition of th data object
        /// </summary>
        iObjectDefinition IObjectDefinition {get;}
        /// <summary>
        /// gets the Primary key of the data object
        /// </summary>
        iKey PrimaryKey {get;}
        /// <summary>
        /// gets the created timestamp
        /// </summary>
        DateTime? CreatedOn {get;}
        /// <summary>
       /// gets the updated on time stamp
       /// </summary>
        DateTime? UpdatedOn {get; }
        /// <summary>
        /// gets the deleted on time stamp
        /// </summary>
        DateTime? DeletedOn {get;}
        /// <summary>
        /// gets the GUID of the object
        /// </summary>
        Guid GUID {get;}
        /// <summary>
        /// gets the object handle of the data object
        /// </summary>
        String ObjectID {get;}
        /// <summary>
        /// gets or sets the change flag of the object
        /// </summary>
        bool IsChanged {get;set;}
        /// <summary>
        /// gets the ChangedOn Timestamp
        /// </summary>
        DateTime? ChangedOn {get;}
        /// <summary>
        /// true if the data object was loaded
        /// </summary>
        bool IsLoaded {get;}
        /// <summary>
        /// true if the data object is created
        /// </summary>
        bool IsCreated {get; }
        /// <summary>
        /// true if the data object was deleted
        /// </summary>
        bool IsDeleted {get;}
        /// <summary>
        /// gets the entry name's value of the data object instance
        /// </summary>
        /// <param name="entryname"></param>
        /// <returns></returns>
        Object GetValue(String entryname);
        /// <summary>
        /// sets the entry name's value of the data object instance
        /// </summary>
        /// <param name="entryname"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool SetValue(String entryname, Object value);
    }
    /// <summary>
    /// describes a key data tuple
    /// </summary>
    public interface iKey : IHashCodeProvider , IComparable 
    {
        /// <summary>
        /// gets or sets the keys / entrynames
        /// </summary>
        String[] Keys { get; set; }
        /// <summary>
        ///  returns the size of the ObjectKey Array
        /// </summary>
        ushort Size {get ;}
        /// <summary>
        ///  Returns the actuals count means if initialised the number of non-nothing members
        /// </summary>
         ushort Count { get; }
        /// <summary>
        /// gets or sets the values
        /// </summary>
          Object[] Values { get; set; }
        /// <summary>
        /// gets or sets the n. the item 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
          Object this[int index] { get; set; }
        /// <summary>
        /// gets or sets the n. the item 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
          Object this[string index] { get; set; }
    }
    /// <summary>
    /// declares a DataObjectProvider
    /// </summary>
    public interface iDataObjectProvider
    {
            /// <summary>
            /// creates a new instance of the data object type
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            iDataObject NewDataObject(System.Type type);
            /// <summary>
            /// returns true if the object handle is handled by the factory
            /// </summary>
            bool HasObjectID (string objectID);
            /// <summary>
            /// returns true if the type is handled by the data object provider
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            bool HasType(Type type);
        /// <summary>
        ///  register an object handle to be handled by the factory
        /// </summary>
        /// <param name="objectID"></param>
        /// <returns></returns>
        bool RegisterObjectID (string objectID);
        /// <summary>
        /// gets the data object repository of this provider
        /// </summary>
        iDataObjectRepository DataObjectRepository { get;  }
        /// <summary>
        /// returns a list of types of data objects handled by this provided
        /// </summary>
        List<System.Type>Types {get;}
        /// <summary>
        /// returns a list of data object ids handled by this data object provider
        /// </summary>
        List<String> ObjectIDs {get;}

        /// <summary>
        /// creates a data object of a object handle with a key
        /// </summary>
        /// <param name="objectid"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        iDataObject Create(string objectid, iKey key);
        /// <summary>
        /// returns an all data object of an object handle
        /// </summary>
        /// <param name="objectid"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable <iDataObject> RetrieveAll(string objectid);
        /// <summary>
        /// returns an all data object of an object handle
        /// </summary>
        /// <param name="objectid"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable<iDataObject> Retrieve(SelectionRule rule);
        /// <summary>
        /// returns an existing data object of an object handle with key
        /// </summary>
        /// <param name="objectid"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        iDataObject Retrieve(string objectid, iKey key);
        /// <summary>
        /// persists an data object with an optional time stamp
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        bool Persist(iDataObject obj,DateTime? timestamp = null);
        /// <summary>
        /// deletes an data object with an optional time stamp
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        bool Delete(iDataObject obj, DateTime? timestamp = null);
        /// <summary>
        /// deletes an data object with an optional time stamp
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        bool UnDelete(iDataObject obj);
        /// <summary>
        /// clones an data object with an optional new key
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        iDataObject Clone(iDataObject obj, iKey key=null);

    }
}
