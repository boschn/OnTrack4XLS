/**
 *  ONTRACK RULEZ ENGINE
 *  
 * rulez repository
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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OnTrack.Core;
using OnTrack.Rulez.eXPressionTree;

namespace OnTrack.Rulez
{
        /// <summary>
        /// declares something which can be run by the engine
        /// </summary>
        public interface ICodeBit
        {
            /// <summary>
            /// the ID of the Bit
            /// </summary>
            string Handle { get; set; }
            /// <summary>
            /// a Helper Tag for the Generator to attach a custom object
            /// </summary>
            Object Tag { get; set; }
            /// <summary>
            /// delegate for the Code
            /// </summary>
            Func<Context, Boolean> Code { get; set; }
        }
        /// <summary>
        /// types of operator
        /// </summary>
        public enum otOperatorType
        {
            Logical,
            Arithmetic,
            Assignement,
        }
        /// <summary>
        /// defines the Operator Token
        /// </summary>
        public class Token
        {
            /// <summary>
            /// static - must be ascending and discrete ! (do not leave one out !!)
            /// </summary>
            public static uint POS=0;
            public static uint AND = 1;
            public static uint ANDALSO = 2;
            public static uint OR = 3;
            public static uint ORELSE = 4;
            public static uint NOT = 5;

            public static uint EQ = 10;
            public static uint NEQ = 11;
            public static uint GT = 12;
            public static uint GE = 13;
            public static uint LT = 14;
            public static uint LE = 15;

            public static uint PLUS = 16;
            public static uint MINUS = 17;
            public static uint MULT = 18;
            public static uint DIV = 19;
            public static uint MOD = 20;
            public static uint CONCAT = 21; // Concat must be the last one for functions to be found

            public static uint BEEP = 22;

            /// <summary>
            /// variable
            /// </summary>
            private uint _token;

            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="value"></param>
            public Token(uint value)
            {
                _token = value;
            }

            /// <summary>
            /// returns the token
            /// </summary>
            public int ToInt { get { return (int) _token; } }
        }
   
    /// <summary>
    /// defines the function
    /// </summary>
    public class @Function
    {

        /// <summary>
        /// get the _BuildInFunctions -> must be in Order of the TokenID
        /// </summary>
        private static Function[] _buildInFunctions = {

                                                  // logical Operations
                                                  new Function(Token.BEEP,  new otDataType  [] {} , otDataType .Bool   ) 
        };

        /// <summary>
        /// inner variables
        /// </summary>
        private Token _token;
        private otDataType[] _signature;
        private otDataType? _returntype;

        /// <summary>
        /// returns a List of BuildInFunctions
        /// </summary>
        /// <returns></returns>
        public static List<Function> BuildInFunctions()
        {
            return _buildInFunctions.ToList();
        }
        /// <summary>
        /// return the Operator Definition
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static OnTrack.Rulez.Function GetFunction(Token token)
        {
            if (token.ToInt < _buildInFunctions.Length) return _buildInFunctions[token.ToInt - Token.CONCAT];
            throw new RulezException(RulezException.Types.OutOfArraySize, arguments: new object[] { token.ToInt, _buildInFunctions.Length });
        }
       
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="arguments"></param>
        /// <param name="priority"></param>
        public Function(Token token, otDataType[] signature, otDataType returnType)
        {
            _token = token;
            _signature  = signature;
            _returntype = returnType;
        }
        public Function(uint tokenID, otDataType[] signature, otDataType returnType)
        {
            _token = new Token(tokenID);
            _signature  = signature;
            _returntype = returnType;
            
        }


        #region "Properties"
        /// <summary>
        /// gets the Token
        /// </summary>
        public Token TokenID { get { return _token; } }

        /// <summary>
        /// gets the signature
        /// </summary>
        public otDataType [] Signature { get { return _signature; } }

        /// <summary>
        /// gets or sets the return type of the operation
        /// </summary>
        public otDataType? ReturnType { get { return _returntype; } set { _returntype = value; } }
       
        #endregion
    }

    /// <summary>
    /// defines the operators
    /// </summary>
    public class Operator
    {

        /// <summary>
        /// get the _BuildInFunctions -> must be in Order of the TokenID
        /// </summary>
        private static Operator[] _buildInOperators = {

                                                  // logical Operations
                                                  new Operator(Token.POS,1,3,otDataType .Bool ,  otOperatorType.Logical  ) ,
                                                  new Operator(Token.AND,2,5,  otDataType .Bool , otOperatorType.Logical ) ,
                                                  new Operator(Token.ANDALSO,2,5 ,  otDataType .Bool, otOperatorType.Logical ) ,
                                                  new Operator(Token.OR,2,6,  otDataType .Bool , otOperatorType.Logical ) ,
                                                  new Operator(Token.ORELSE,2,6,  otDataType .Bool , otOperatorType.Logical ) ,
                                                  new Operator(Token.NOT,1,3, otDataType .Bool, otOperatorType.Logical   ) ,
                                                  new Operator(Token.EQ,2,4,  otDataType .Bool , otOperatorType.Logical ) ,
                                                  new Operator(Token.NEQ,2,4,  otDataType .Bool , otOperatorType.Logical ) ,
                                                  new Operator(Token.GT,2,4,  otDataType .Bool , otOperatorType.Logical ) ,
                                                  new Operator(Token.GE,2,4,  otDataType .Bool, otOperatorType.Logical  ) ,
                                                  new Operator(Token.LT,2,4,  otDataType .Bool , otOperatorType.Logical ) ,
                                                  new Operator(Token.LE,2,4,  otDataType .Bool , otOperatorType.Logical ) ,

                                                  // Arithmetic - null means return type is determined by the operands
                                                  new Operator(Token.PLUS,2,2,  null , otOperatorType.Arithmetic ) ,
                                                  new Operator(Token.MINUS,2,2,  null , otOperatorType.Arithmetic ) ,
                                                  new Operator(Token.MULT,2,1,  null , otOperatorType.Arithmetic ) ,
                                                  new Operator(Token.DIV,2,1,  null , otOperatorType.Arithmetic ) ,
                                                  new Operator(Token.MOD,2,1,  null , otOperatorType.Arithmetic ) ,
                                                  new Operator(Token.CONCAT,2,1,  null , otOperatorType.Arithmetic ) ,
                                               
        };

        /// <summary>
        /// inner variables
        /// </summary>
        private Token _token;
        private UInt16 _arguments;
        private UInt16 _priority;
        private otDataType? _returntype;
        private otOperatorType _type;

        /// <summary>
        /// returns a List of BuildInFunctions
        /// </summary>
        /// <returns></returns>
        public static List<Operator> BuildInOperators()
        {
            return _buildInOperators.ToList();
        }
        /// <summary>
        /// return the Operator Definition
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Operator GetOperator(Token token)
        {
            if (token.ToInt < _buildInOperators.Length) return _buildInOperators[token.ToInt];
            throw new RulezException(RulezException.Types.OutOfArraySize, arguments: new object[] { token.ToInt, _buildInOperators.Length });
        }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="arguments"></param>
        /// <param name="priority"></param>
        public Operator(Token token, UInt16 arguments, UInt16 priority, otDataType? returnType, otOperatorType type)
        {
            _token = token;
            _arguments = arguments;
            _priority = priority;
            _returntype = returnType;
            _type = type;
        }
        public Operator(uint tokenID, UInt16 arguments, UInt16 priority, otDataType? returnType, otOperatorType type)
        {
            _token = new Token(tokenID);
            _arguments = arguments;
            _priority = priority;
            _returntype = returnType;
            _type = type;

        }
        #region "Properties"
        /// <summary>
        /// gets the Token
        /// </summary>
        public Token TokenID { get { return _token; } }

        /// <summary>
        /// gets the Number of Arguments
        /// </summary>
        public UInt16 Arguments { get { return _arguments; } }

        /// <summary>
        /// gets the Priority
        /// </summary>
        public UInt16 Priority { get { return _priority; } }

        /// <summary>
        /// gets or sets the return type of the operation
        /// </summary>
        public otDataType? ReturnType { get { return _returntype; } set { _returntype = value; } }
        /// <summary>
        /// gets the type of operator
        /// </summary>
        public otOperatorType Type { get { return _type; } }
        #endregion
    }
    /// <summary>
    /// a repository for the rulez engine
    /// </summary>
    public class Repository
    {
        private string _id; // ID of the Repository
        // Dictionary of operators
        private Dictionary<Token, Operator> _Operators = new Dictionary<Token, Operator>();
        // Dictionary of functions
        private Dictionary<Token, Function> _Functions = new Dictionary<Token, Function>();
        // Dictionary of the selection rules
        private Dictionary<String, SelectionRule> _selectionrules = new Dictionary<string, SelectionRule>();
        // Stack of dataObject Repositories
        private List<iDataObjectRepository> _dataobjectRepositories = new List<iDataObjectRepository> ();

        // initialize Flag
        private bool _IsInitialized = false;

        /// <summary>
        /// constructor of an engine
        /// </summary>
        public Repository(string id = null)
        {
            if (id == null) _id = new Guid().ToString();
            else _id = id;
        }


        #region "Properties"
        /// <summary>
        /// gets the unique handle of the engine
        /// </summary>
        public string Id { get { return _id; } }

        /// <summary>
        /// gets all the selection rules in the repository
        /// </summary>
        public List<SelectionRule> SelectionRules { get { return _selectionrules.Values.ToList() ; } }
        /// <summary>
        /// gets all selection rule IDs in the repository
        /// </summary>
        public List<String> SelectionRuleIDs { get { return _selectionrules.Keys.ToList (); } }

        /// <summary>
        /// gets all the operators in the repository
        /// </summary>
        public List<Operator> Operators { get { return _Operators.Values.ToList(); } }
        /// <summary>
        /// gets all operator tokens rule IDs in the repository
        /// </summary>
        public List<Token> OperatorTokens { get { return _Operators.Keys.ToList(); } }
        /// <summary>
        /// return true if initialized
        /// </summary>
        public bool IsInitialized { get { return _IsInitialized; } }

        #endregion

        /// <summary>
        /// register the DataObjectEntrySymbol Repository
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public bool RegisterDataObjectRepository(iDataObjectRepository repository)
        {
            _dataobjectRepositories.Add(repository);
            return true;
        }
        /// <summary>
        /// register the DataObjectEntrySymbol Repository
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public bool DeRegisterDataObjectRepository(iDataObjectRepository repository)
        {
            _dataobjectRepositories.Remove(repository);
            return true;
        }
        /// <summary>
        /// lazy initialize
        /// </summary>
        /// <returns></returns>
        private bool Initialize()
        {
            if (_IsInitialized) return false;

            // operator
            foreach (Operator anOperator in Operator.BuildInOperators())
            {
                if (! _Operators.ContainsKey(anOperator.TokenID)) 
                    _Operators.Add(anOperator.TokenID, anOperator);
            }

            // Functions
            foreach (Function aFunction in Function.BuildInFunctions())
            {
                if (!_Functions.ContainsKey(aFunction.TokenID))
                    _Functions.Add(aFunction.TokenID, aFunction);
            }
            _IsInitialized = true;
            return _IsInitialized;
        }
        
        /// <summary>
        /// returns true if the repository has the selection rule
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool HasSelectionRule(string id)
        {
            Initialize();
            return _selectionrules.ContainsKey(id);
        }
        /// <summary>
        /// returns the selectionrule by handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public SelectionRule GetSelectionRule(string id)
        {
            Initialize();
            if (this.HasSelectionRule (id)) return _selectionrules[id];
            throw new KeyNotFoundException(id + " was not found in repository");
        }
        /// <summary>
        /// adds a selection rule to the repository by handle
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool AddSelectionRule(string id, SelectionRule rule)
        {
            Initialize();
            if (this.HasSelectionRule(id)) _selectionrules.Remove(id);
            _selectionrules.Add(id, rule);
            return true;
        }
        /// <summary>
        /// adds a selection rule to the repository by handle
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool RemoveSelectionRule(string id)
        {
            Initialize();
            if (this.HasSelectionRule(id)) return _selectionrules.Remove(id);
            return false;
        }
        /// <summary>
        /// returns true if the repository has the function
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool HasFunction(Token id)
        {
            Initialize();
            return _Functions.ContainsKey(id);
        }
        /// <summary>
        /// returns the function by handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public Function GetFunction(Token id)
        {
            Initialize();
            if (this.HasFunction(id)) return _Functions[id];
            throw new KeyNotFoundException(id + " was not found in repository");
        }
        /// <summary>
        /// adds a function to the repository by handle
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool AddFunction(Function function)
        {
            Initialize();
            if (this.HasFunction(function.TokenID)) _Functions.Remove(function.TokenID);
            _Functions.Add(function.TokenID, function);
            return true;
        }
        /// <summary>
        /// returns true if the repository has the selection rule
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool HasOperator(Token id)
        {
            Initialize();
            return _Operators.ContainsKey(id);
        }
        /// <summary>
        /// returns the selectionrule by handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public Operator GetOperator(Token id)
        {
            Initialize();
            if (this.HasOperator(id)) return _Operators[id];
            throw new KeyNotFoundException(id + " was not found in repository");
        }
        /// <summary>
        /// adds a selection rule to the repository by handle
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool AddOperator(Operator Operator)
        {
            Initialize();
            if (this.HasOperator(Operator.TokenID)) _Operators.Remove(Operator.TokenID);
            _Operators.Add(Operator.TokenID, Operator);
            return true;
        }
        /// <summary>
        /// adds a selection rule to the repository by handle
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool RemoveOperator(Token id)
        {
            Initialize();
            if (this.HasOperator(id)) return _Operators.Remove(id);
            return false;
        }

        /// <summary>
        /// returns true if the repository has the data object definition
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool HasDataObjectDefinition(string id)
        {
            Initialize();
            foreach (iDataObjectRepository aRepository in _dataobjectRepositories)
            {
                if (aRepository.HasObjectDefinition(id)) return true;
            }
            return false;
        }
        /// <summary>
        /// returns the selectionrule by handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public iObjectDefinition GetDataObjectDefinition(String id)
        {
            Initialize();
            foreach (iDataObjectRepository aRepository in _dataobjectRepositories)
            {
                iObjectDefinition aDefinition = aRepository.GetIObjectDefinition(id);
                if (aDefinition != null) return aDefinition;
            }
            throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[] { id, "DataObjectEntrySymbol Repositories" });
        }
    }
}