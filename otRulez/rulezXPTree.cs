/**
 *  ONTRACK RULEZ ENGINE
 *  
 * eXpression Tree
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OnTrack.Core;

namespace OnTrack.Rulez.eXPressionTree
{
    /// <summary>
    /// base class for all nodes 
    /// </summary>
    public abstract class Node : INode
    {
        protected Engine _engine; // internal engine
        protected otXPTNodeType _nodetype;
       

        /// <summary>
        /// gets the node type
        /// </summary>
        public otXPTNodeType NodeTokenType { get { return _nodetype; } }
        /// <summary>
        /// returns 
        /// </summary>
        public abstract bool HasSubNodes { get; }
       
        /// <summary>
        /// returns the engine
        /// </summary>
        public Engine Engine
        {
            get
            {
                if (_engine == null) return OnTrack.Rules.Engine;
                return _engine;
            }
        }

        /// <summary>
        /// accept the visitor
        /// </summary>
        /// <param name="visitor"></param>
        public bool Accept(IVisitor visitor) { visitor.Visit(this); return true; }

        /// <summary>
        /// returns an IEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            List<INode> aList = new List<INode>();
            aList.Add(this);
            return aList.GetEnumerator ();
        }

        public IEnumerator<INode> GetEnumerator()
        {
            List<INode> aList = new List<INode>();
            aList.Add(this);
            return aList.GetEnumerator();
        }
     }

    /// <summary>
    /// declare a constant node in an AST
    /// </summary>
    public class Literal: Node
    {
        private object _value;
        private otDataType _datatype;

        /// <summary>
        /// constructor
        /// </summary>
        public Literal(object value = null, otDataType? datatype = null): base()
        {
            _nodetype = otXPTNodeType.Literal;
            if (datatype != null && datatype.HasValue ) _datatype = datatype.Value;
            if (value != null) this.Value = value;

            if ((datatype == null) && (value != null))
            {
                throw new NotImplementedException("data type determination by value");
            }


        }
        

        /// <summary>
        /// returns true if node is a leaf
        /// </summary>
        public override bool HasSubNodes { get { return false; } }

        /// <summary>
        /// gets or sets the constant value
        /// </summary>
        public object Value { get { return _value; } set { _value = value; } }
        /// <summary>
        /// returns the datatype of the literal
        /// </summary>
        public otDataType Datatype { get { return _datatype; } }
        /// <summary>
        /// gets or sets the type of the literal
        /// </summary>
        public System.Type Type
        {
            get
            {
                if (_value != null) return _value.GetType();
                else return null;
            }
        }
    }

    /// <summary>
    /// Base class for all tree nodes
    /// </summary>
    public abstract class eXPressionTree : IeXPressionTree
    {
        protected List<INode> _Nodes = new List<INode>();
        protected Engine _engine;
        protected otXPTNodeType _nodetype;

        /// <summary>
        /// return the node type
        /// </summary>
        public otXPTNodeType NodeTokenType { get { return _nodetype; } }
        /// <summary>
        /// return all the leaves
        /// </summary>
        public List<INode> Nodes { get { return _Nodes; } set { _Nodes = value; } }
        /// <summary>
        /// returns true if node is a leaf
        /// </summary>
        public bool HasSubNodes { get { return true; } }
        /// <summary>
        /// returns the engine
        /// </summary>
        public Engine Engine
        {
            get
            {
                if (_engine == null) return OnTrack.Rules.Engine;
                return _engine;
            }
        }

        /// <summary>
        /// accept the visitor
        /// </summary>
        /// <param name="visitor"></param>
        public bool Accept(IVisitor visitor) { visitor.Visit(this); return true; }

        /// <summary>
        /// returns an IEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Nodes.GetEnumerator();
        }

        public IEnumerator<INode> GetEnumerator()
        {
            return _Nodes.GetEnumerator();
        }

        /// <summary>
        /// returns all DataObjectEntry names in the expression tree
        /// </summary>
        /// <returns></returns>
        public List<String> DataObjectEntryNames()
        {
            List<String> aList = new List<string>();
            Visitor<String> aVistor = new Visitor<String>();
            // define a simple handler via lambda
            Visitor<String>.Eventhandler aVisitingHandling 
                = (o, e) => {
                if (e.CurrentNode.GetType() == typeof(DataObjectEntrySymbol))
                    e.Stack.Push((e.CurrentNode as DataObjectEntrySymbol).ID);
               };
            aVistor.VisitingDataObjectSymbol += aVisitingHandling; // register
            aVistor.Visit(this); // run
            // get uniques
            foreach (String aName in aVistor.Stack.ToList<String>())
                if (!aList.Contains(aName)) aList.Add(aName);

            // return
            return aList;
        }

    }

    /// <summary>
    /// defines a rule
    /// </summary>
    public abstract class Rule : eXPressionTree, IRule
    {
        private string _id; // unique ID of the rule
        private otRuleState _state; // state of the rule
        private String _handle; // handle of the rule theCode in the engine
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="handle"></param>
        public Rule( string id = null,  Engine engine = null): base()
        {
            _nodetype = otXPTNodeType.Rule;

            if (id == null) { _id = new Guid().ToString(); }
            else { _id = id; }
            _state = otRuleState.created;
            _engine = engine;
            _handle = new Guid().ToString();
        }

        /// <summary>
        /// sets or gets the handle of the rule
        /// </summary>
        public string ID { get { return _id; } set { _id = value; } }
        /// <summary>
        /// returns the theCode handle
        /// </summary>
        public string Handle { get { return _handle; } }
        /// <summary>
        /// returns the state of the rule
        /// </summary>
        public otRuleState RuleState { get { return _state; } set { _state = value; } }

        /// <summary>
        /// set the state of the rule
        /// </summary>
        /// <param name="newState"></param>
        protected void SetState(otRuleState newState) { _state = newState; }
    }

    /// <summary>
    /// defines a dataobject in a IeXPressionTree object
    /// </summary>
    public class Variable : Node, ISymbol
    {
        private string _id;
        private otDataType _type;
        private IeXPressionTree _scope;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="Type"></param>
        /// <param name="scope"></param>
        public Variable(string id, otDataType Type, IeXPressionTree scope, Engine engine = null): base()
        {
            // default engine
            if (engine == null) engine = OnTrack.Rules.Engine;

            _engine = engine;
            _id = id;
            _type = Type;
            _scope = scope;
            _nodetype = otXPTNodeType.Variable;
        }

        /// <summary>
        /// gets or sets the ID
        /// </summary>
        public string ID { get { return _id; } set { _id = value; } }

        /// <summary>
        /// gets or sets the Type of the variable
        /// </summary>
        public otDataType Type { get { return _type; } set { _type = value; } }

        /// <summary>
        /// sets the scope of the variable
        /// </summary>
        public IeXPressionTree Scope { get { return _scope; } set { _scope = value; } }

        /// <summary>
        /// returns true if node is a leaf
        /// </summary>
        public override bool HasSubNodes { get { return false; } }
    }
    /// <summary>
    /// defines a data object symbol in a IeXPressionTree object
    /// </summary>
    public class DataObjectSymbol : Node, ISymbol
    {
        private IeXPressionTree _scope;
        private iObjectDefinition _objectdefinition;

        /// <summary>
        /// constructor in 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="Type"></param>
        /// <param name="scope"></param>
        public DataObjectSymbol(string id, Engine engine = null)
            : base()
        {
            // default engine
            if (engine == null) engine = OnTrack.Rules.Engine;
            ///
            if (id.Contains('.'))
            {
                string[] names = id.Split('.');
                if (engine.Repository.HasDataObjectDefinition(names[0]))
                {
                    _objectdefinition = engine.Repository.GetDataObjectDefinition(names[0]);
                    if (_objectdefinition == null)
                    {
                        throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[]
                        {
                            names[1],
                            names[0]
                        });
                    }

                }
                else
                { throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[] { names[0], "data object repository" }); }

            }
            else
            {
                throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[] { id, "data object repository" });
            }

            _scope = null;
            _engine = engine;
            _nodetype = otXPTNodeType.DataObjectSymbol;
        }

        /// <summary>
        /// gets or sets the ID
        /// </summary>
        public string ID
        {
            get { return _objectdefinition.Objectname; }
            set
            {   ///
                if (value.Contains('.'))
                {
                    string[] names = value.Split('.');
                    if (_engine.Repository.HasDataObjectDefinition(names[0]))
                    {
                        _objectdefinition = _engine.Repository.GetDataObjectDefinition(names[0]);
                        if (_objectdefinition == null) throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[] { names[1], names[0] });

                    }
                    else
                    { throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[] { names[0], "data object repository" }); }

                }
                else
                {
                    throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[] { value, "data object repository" });
                };
            }
        }

        /// <summary>
        /// returns the ObjectID of the entry
        /// </summary>
        public String ObjectID { get { return _objectdefinition.Objectname; } }
        /// <summary>
        /// returns the IObjectDefinition
        /// </summary>
        public iObjectDefinition Definition { get { return _objectdefinition; } }
        /// <summary>
        /// returns the scope
        /// </summary>
        public IeXPressionTree Scope { get { return _scope; } set { _scope = value; } }
        /// <summary>
        /// returns the engine
        /// </summary>
        public Engine Engine { get { return _engine; } }
        /// <summary>
        /// gets or sets the type of the variable
        /// </summary>
        public Core.otDataType Type { get { return otDataType.Void ;} set{} }
        /// <summary>
        /// returns true if node is a leaf
        /// </summary>
        public override bool HasSubNodes { get { return false; } }
    }
    /// <summary>
    /// defines a local variable in a IeXPressionTree object
    /// </summary>
    public class DataObjectEntrySymbol :  Node, ISymbol 
    {
        private IeXPressionTree _scope;
        private iObjectEntryDefinition _entrydefinition;

        /// <summary>
        /// constructor in 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="Type"></param>
        /// <param name="scope"></param>
        public DataObjectEntrySymbol(string id, Engine engine = null): base()
        {
            // default engine
            if (engine == null) engine = OnTrack.Rules.Engine;
            ///
            if (id.Contains ('.')) {
                string[] names = id.Split ('.');
                if (engine.Repository.HasDataObjectDefinition(names[0]))
                {
                    Core.iObjectDefinition aDefinition = engine.Repository.GetDataObjectDefinition(names[0]);
                    _entrydefinition = aDefinition.GetiEntryDefinition(names[1]);
                    if (_entrydefinition == null)
                    {
                        throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[]
                        {
                            names[1],
                            names[0]
                        });
                    }
                   
                }else
                { throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[] { names[0], "data object repository" }); }

            }else{
                throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[] { id, "data object repository" });
            }

            _scope = null;
            _engine = engine;
            _nodetype = otXPTNodeType.DataObjectSymbol;
        }
        public DataObjectEntrySymbol( string objectid, string entryname,  Engine engine = null): base()
        {
            // default engine
            if (engine == null) engine = OnTrack.Rules.Engine;

                if (engine.Repository.HasDataObjectDefinition(objectid))
                {
                    Core.iObjectDefinition aDefinition = engine.Repository.GetDataObjectDefinition(objectid);
                    _entrydefinition = aDefinition.GetiEntryDefinition(entryname);
                    if (_entrydefinition == null) throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[] { entryname, objectid });
                    else { }
                }
                else
                { throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[] { objectid, "data object repository" }); }


            _scope = null;
            _engine = engine;
            _nodetype = otXPTNodeType.DataObjectSymbol;
        }

        /// <summary>
        /// gets or sets the ID
        /// </summary>
        public string ID
        {
            get { return _entrydefinition .Objectname  + '.' + _entrydefinition .Entryname ; }
            set
            {   ///
                if (value.Contains('.'))
                {
                    string[] names = value.Split('.');
                    if (_engine.Repository.HasDataObjectDefinition(names[0]))
                    {
                        Core.iObjectDefinition aDefinition = _engine.Repository.GetDataObjectDefinition(names[0]);
                        _entrydefinition = aDefinition.GetiEntryDefinition(names[1]);
                        if (_entrydefinition == null) throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[] { names[1], names[0] });

                    }
                    else
                    { throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[] { names[0], "data object repository" }); }

                }
                else
                {
                    throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[] { value, "data object repository" });
                };
            }
        }
        /// <summary>
        /// returns the IObjectDefinition
        /// </summary>
        public iObjectEntryDefinition Definition { get { return _entrydefinition; } }
        /// <summary>
        /// returns the ObjectID of the entry
        /// </summary>
        public String ObjectID { get { return _entrydefinition.Objectname; } }
        /// <summary>
        /// returns the ObjectID of the entry
        /// </summary>
        public String Entryname { get { return _entrydefinition.Entryname; } }
        /// <summary>
        /// gets or sets the Type of the variable
        /// </summary>
        public otDataType  Type { get { return _entrydefinition.Datatype; } set { throw new NotImplementedException(); } }
        /// <summary>
        /// returns the scope
        /// </summary>
        public IeXPressionTree Scope { get { return _scope; } set { _scope = value; } }
        /// <summary>
        /// returns the engine
        /// </summary>
        public Engine Engine { get { return _engine; } }
       

        /// <summary>
        /// returns true if node is a leaf
        /// </summary>
        public override bool HasSubNodes { get { return false; } }
    }
    /// <summary>
    /// function call node
    /// </summary>
    public class FunctionCall: eXPressionTree , IExpression
    {
        protected Token _function; // function Token

        #region "Properties"

        /// <summary>
        /// gets or sets the Operation
        /// </summary>
        public Token TokenID { get { return _function; } set { _function = value; } }

        /// <summary>
        /// gets the Operator definition
        /// </summary>
        public Function Function { get { return OnTrack.Rules.Engine.GetFunction(_function); } }
      
        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="token"></param>
        /// <param name="arguments"></param>
        public FunctionCall(Token token, INode [] arguments) : base()
        {
            // TODO: check the argumetns
            _function = token;
            _Nodes = arguments.ToList();
        }
    }
    /// <summary>
    /// Operation Selection
    /// </summary>
    public class OperationExpression: eXPressionTree , IExpression
    {
        protected Token _op; // operation Token

        #region "Properties"
    
        /// <summary>
        /// gets or sets the Operation
        /// </summary>
        public Token TokenID { get { return _op; } set { _op = value; } }

        /// <summary>
        /// gets the Operator definition
        /// </summary>
        public Operator Operator { get { return OnTrack.Rules.Engine.GetOperator(_op); } }

        /// <summary>
        /// returns the left operand
        /// </summary>
        public INode LeftOperand
        {
            get
            {
                return _Nodes[0];
            }
            set
            {
                if (value != null && ((value.GetType().GetInterfaces().Contains(typeof(INode))
                    || (value.GetType().GetInterfaces().Contains(typeof(IExpression)))
                   )))
                {
                    _Nodes[0] = value;
                }
                else if (value == null) _Nodes[1] = null;
                else throw new RulezException(RulezException.Types.InvalidOperandNodeType, arguments: value);
            }
        }
        /// <summary>
        /// build and return a recursive LogicalExpression Tree from arguments
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private INode BuildExpressionTree(int i)
        {
            // build right-hand a subtree
            if (_Nodes.Count >= i + 1) return new OperationExpression(this.Operator, _Nodes[i], BuildExpressionTree(i + 1));
            // return the single node
            return _Nodes[i];
        }
        /// <summary>
        /// returns the right operand
        /// </summary>
        public INode RightOperand
        {
            get
            {
                if ((_Nodes == null) || (_Nodes.Count == 0)) return null;
                if (_Nodes.Count == 1) return _Nodes[1];
                // create a tree of the rest
                return BuildExpressionTree(1);
            }
            set
            {
                if (value != null && ((value.GetType().GetInterfaces().Contains(typeof(INode))
                    || (value.GetType().GetInterfaces().Contains(typeof(IExpression)))
                   )))
                {
                    _Nodes[1] = value;
                }
                else if (value == null) _Nodes[1] = null;
                else throw new RulezException(RulezException.Types.InvalidOperandNodeType, arguments: value);
            }
        }
        #endregion

         /// <summary>
        /// constructor
        /// </summary>
        /// <param name="op"></param>
        /// <param name="operand"></param>
        public OperationExpression(Token op, INode operand, Engine engine = null): base()
        {
            
            // default engine
            if (engine == null) engine = OnTrack.Rules.Engine;

            if (OnTrack.Rules.Engine.GetOperator(op) == null) throw new RulezException(RulezException.Types.OperatorNotDefined, arguments: new object[] { op.ToString() });
            _op = op;
            if (this.Operator.Arguments != 1) throw new RulezException(RulezException.Types.OperandsNotEqualOperatorDefinition, arguments: new object[] { op.ToString(), this.Operator.Arguments, 1 });
            if (operand != null) _Nodes[0] = operand;
            else throw new RulezException(RulezException.Types.OperandNull, arguments: new object[] { op.ToString(), "" });

            _engine = engine;
            _nodetype = otXPTNodeType.OperationExpression;          
        }
        public OperationExpression( Operator op, INode operand, Engine engine = null): base()
        {
            // default engine
            if (engine == null) engine = OnTrack.Rules.Engine;

            _op = op.TokenID;
            if (op == null) throw new RulezException(RulezException.Types.OperatorNotDefined, arguments: new object[] { "(null)" });
            if (this.Operator.Arguments != 1) throw new RulezException(RulezException.Types.OperandsNotEqualOperatorDefinition, arguments: new object[] { op.TokenID.ToString(), op.Arguments, 1 });
            if (operand != null) _Nodes[0] = operand;
            else throw new RulezException(RulezException.Types.OperandNull, arguments: new object[] { op.TokenID.ToString(), "" });
            _engine = engine;
            _nodetype = otXPTNodeType.OperationExpression;     

        }
        /// <summary>
        /// constructor of an expression
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="leftoperand"></param>
        /// <param name="rightoperand"></param>
        public OperationExpression( Token op, INode leftoperand, INode rightoperand, Engine engine = null): base()
        {
            // default engine
            if (engine == null) engine = OnTrack.Rules.Engine;

            if (OnTrack.Rules.Engine.GetOperator(op) == null ) throw new RulezException(RulezException.Types.OperatorNotDefined,arguments:new object[]{ op.ToString() });
            _op = op;
            if (this.Operator.Arguments != 2) throw new RulezException(RulezException.Types.OperandsNotEqualOperatorDefinition, arguments: new object[] { op.ToString(), this.Operator.Arguments, 2 });
            if (leftoperand != null) _Nodes[0] = leftoperand;
            else throw new RulezException(RulezException.Types.OperandNull, arguments: new object[] { op.ToString(), "left" });

            if (rightoperand != null) _Nodes[1] = rightoperand;
            else throw new RulezException(RulezException.Types.OperandNull, arguments: new object[] { op.ToString(), "right" });
            _engine = engine;
            _nodetype = otXPTNodeType.OperationExpression;     
        }
        public OperationExpression(Operator op, INode leftoperand, INode rightoperand, Engine engine = null)
            : base()
        {
            // default engine
            if (engine == null) engine = OnTrack.Rules.Engine;

            _op = op.TokenID;
            if (op.Arguments != 2) throw new RulezException(RulezException.Types.OperandsNotEqualOperatorDefinition, arguments: new object[] { op.TokenID.ToString(), op.Arguments, 2 });
            if (leftoperand != null) _Nodes[0] = leftoperand;
            else throw new RulezException(RulezException.Types.OperandNull, arguments: new object[] { op.TokenID.ToString(), "left" });

            if (rightoperand != null) _Nodes[1] = rightoperand;
            else throw new RulezException(RulezException.Types.OperandNull, arguments: new object[] { op.TokenID.ToString(), "right" });
            _engine = engine;
            _nodetype = otXPTNodeType.OperationExpression;
        }
    }
    /// <summary>
    /// defines an logical expression
    /// </summary>
    public class LogicalExpression : OperationExpression
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="op"></param>
        /// <param name="operand"></param>
        public LogicalExpression(Token op, INode operand, Engine engine = null) : base( op,  operand,  engine )
        {
            if (this.Operator.Type !=  otOperatorType.Logical )
                throw new RulezException(RulezException.Types.OperatorTypeNotExpected , arguments: new object[] { op.ToString(), "logical" });
            _nodetype = otXPTNodeType.LogicalExpression;          
        }
        public LogicalExpression( Operator op, INode operand, Engine engine = null): base( op,  operand,  engine )
        {
            if (this.Operator.Type !=  otOperatorType.Logical )
                throw new RulezException(RulezException.Types.OperatorTypeNotExpected , arguments: new object[] { op.ToString(), "logical" });
            _nodetype = otXPTNodeType.LogicalExpression;          

        }
        /// <summary>
        /// constructor of an expression
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="leftoperand"></param>
        /// <param name="rightoperand"></param>
        public LogicalExpression( Token op, INode leftoperand, INode rightoperand, Engine engine = null): base( op,  leftoperand, rightoperand, engine )
        {
            if (this.Operator.Type !=  otOperatorType.Logical )
                throw new RulezException(RulezException.Types.OperatorTypeNotExpected , arguments: new object[] { op.ToString(), "logical" });
            _nodetype = otXPTNodeType.LogicalExpression;          
        }
        public LogicalExpression(Operator op, INode leftoperand, INode rightoperand, Engine engine = null)
            : base(op, leftoperand, rightoperand, engine)
        {
            if (this.Operator.Type !=  otOperatorType.Logical )
                throw new RulezException(RulezException.Types.OperatorTypeNotExpected , arguments: new object[] { op.ToString(), "logical" });
            _nodetype = otXPTNodeType.LogicalExpression;          
        }

 #region "Helper"
        /// <summary>
        /// returns an LogicalExpression with AND
        /// </summary>
        /// <param name="leftoperand"></param>
        /// <param name="rightoperand"></param>
        /// <returns></returns>
        static public LogicalExpression AND(INode leftoperand, INode rightoperand)
        {
            return new LogicalExpression(new Token(Token.AND), leftoperand, rightoperand);
        }
        /// <summary>
        /// returns an LogicalExpression with AND
        /// </summary>
        /// <param name="leftoperand"></param>
        /// <param name="rightoperand"></param>
        /// <returns></returns>
         public LogicalExpression AND( INode rightoperand)
        {
            return new LogicalExpression(new Token(Token.AND), this, rightoperand);
        }
        /// <summary>
        /// returns an LogicalExpression with ANDALSO
        /// </summary>
        /// <param name="leftoperand"></param>
        /// <param name="rightoperand"></param>
        /// <returns></returns>
        static public LogicalExpression ANDALSO(INode leftoperand, INode rightoperand)
        {
            return new LogicalExpression(new Token(Token.ANDALSO), leftoperand, rightoperand);
        }
        /// <summary>
        /// returns an LogicalExpression with ANDALSO
        /// </summary>
        /// <param name="leftoperand"></param>
        /// <param name="rightoperand"></param>
        /// <returns></returns>
         public LogicalExpression ANDALSO( INode rightoperand)
        {
            return new LogicalExpression(new Token(Token.ANDALSO), this, rightoperand);
        }
         /// <summary>
         /// returns an LogicalExpression with OR
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         static public LogicalExpression OR(INode leftoperand, INode rightoperand)
         {
             return new LogicalExpression(new Token(Token.OR), leftoperand, rightoperand);
         }
         /// <summary>
         /// returns an LogicalExpression with OR
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         public LogicalExpression OR(INode rightoperand)
         {
             return new LogicalExpression(new Token(Token.OR), this, rightoperand);
         }
         /// <summary>
         /// returns an LogicalExpression with ORELSE
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         static public LogicalExpression ORELSE(INode leftoperand, INode rightoperand)
         {
             return new LogicalExpression(new Token(Token.ORELSE), leftoperand, rightoperand);
         }
         /// <summary>
         /// returns an LogicalExpression with ORELSE
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         public LogicalExpression ORELSE(INode rightoperand)
         {
             return new LogicalExpression(new Token(Token.ORELSE), this, rightoperand);
         }
         /// <summary>
         /// returns an LogicalExpression with ORELSE
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         static public LogicalExpression NOT(INode operand)
         {
             return new LogicalExpression(new Token(Token.NOT), operand);
         }
         /// <summary>
         /// returns an LogicalExpression with EQUAL
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         static public LogicalExpression EQ(INode leftoperand, INode rightoperand)
         {
             return new LogicalExpression(new Token(Token.EQ), leftoperand, rightoperand);
         }
         /// <summary>
         /// returns an LogicalExpression with EQUAL
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         public LogicalExpression EQ(INode rightoperand)
         {
             return new LogicalExpression(new Token(Token.EQ), this, rightoperand);
         }
         /// <summary>
         /// returns an LogicalExpression with NEQUAL
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         static public LogicalExpression NEQ(INode leftoperand, INode rightoperand)
         {
             return new LogicalExpression(new Token(Token.NEQ), leftoperand, rightoperand);
         }
         /// <summary>
         /// returns an LogicalExpression with EQUAL
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         public LogicalExpression NEQ(INode rightoperand)
         {
             return new LogicalExpression(new Token(Token.NEQ), this, rightoperand);
         }
         /// <summary>
         /// returns an LogicalExpression with GREATER THAN
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         static public LogicalExpression GT(INode leftoperand, INode rightoperand)
         {
             return new LogicalExpression(new Token(Token.GT), leftoperand, rightoperand);
         }
         /// <summary>
         /// returns an LogicalExpression with GREATER THAN
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         public LogicalExpression GT(INode rightoperand)
         {
             return new LogicalExpression(new Token(Token.GT), this, rightoperand);
         }
         /// <summary>
         /// returns an LogicalExpression with GREATER EQUAL
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         static public LogicalExpression GE(INode leftoperand, INode rightoperand)
         {
             return new LogicalExpression(new Token(Token.GE), leftoperand, rightoperand);
         }
         /// <summary>
         /// returns an LogicalExpression with GREATER EQUAL
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         public LogicalExpression GE(INode rightoperand)
         {
             return new LogicalExpression(new Token(Token.GE), this, rightoperand);
         }
         /// <summary>
         /// returns an LogicalExpression with GREATER THAN
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         static public LogicalExpression LT(INode leftoperand, INode rightoperand)
         {
             return new LogicalExpression(new Token(Token.LT), leftoperand, rightoperand);
         }
         /// <summary>
         /// returns an LogicalExpression with GREATER THAN
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         public LogicalExpression LT(INode rightoperand)
         {
             return new LogicalExpression(new Token(Token.LT), this, rightoperand);
         }
         /// <summary>
         /// returns an LogicalExpression with GREATER EQUAL
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         static public LogicalExpression LE(INode leftoperand, INode rightoperand)
         {
             return new LogicalExpression(new Token(Token.LE), leftoperand, rightoperand);
         }
         /// <summary>
         /// returns an LogicalExpression with GREATER EQUAL
         /// </summary>
         /// <param name="leftoperand"></param>
         /// <param name="rightoperand"></param>
         /// <returns></returns>
         public LogicalExpression LE(INode rightoperand)
         {
             return new LogicalExpression(new Token(Token.LE), this, rightoperand);
         }
#endregion
      
    }
    /// <summary>
    /// defines a node for holding an result
    /// </summary>
    public class Result : eXPressionTree
    {
        private String _ID;
        private List<String> _objectnames = new List<String> (); // objectnames the result is referring to

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="nodes"></param>
        public Result(string ID, INode node)
        {
            _ID = ID;
            _nodetype = otXPTNodeType.Result;
            _Nodes.Add(node);
        }

        /// <summary>
        /// return the embedded Result INode
        /// </summary>
        public INode Embedded { get { return _Nodes[0]; } }
        /// <summary>
        /// gets or sets the ID of the result node
        /// </summary>
        public String ID { get { return _ID; } set { _ID = value; } }

        /// <summary>
        /// gets the Objectname referenced in the Node
        /// </summary>
        public List<String> Objectnames
        {
            get
            {
                if ((_Nodes == null) || (Nodes.Count == 0)) { _objectnames.Clear(); return _objectnames; }
                // check the tree
                Visitor<String> aVisitor = new Visitor<string>();
               
                aVisitor.VisitingDataObjectSymbol += new Visitor<string>.Eventhandler(VisitorEvent);
                aVisitor.Visit(_Nodes[0]);
                aVisitor.VisitingDataObjectSymbol -= new Visitor<string>.Eventhandler(VisitorEvent);
                
                return _objectnames;
            }
        }
        /// <summary>
        /// VisitorEvent
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void VisitorEvent(object o, VisitorEventArgs<String> e)
        {
            String anObjectname = (e.CurrentNode as DataObjectEntrySymbol).ObjectID;
            // add it
            if (!_objectnames.Contains<String>(anObjectname)) _objectnames.Add(anObjectname);
        }
    }
    /// <summary>
    /// define a list of results
    /// </summary>
    public class ResultList: eXPressionTree, IExpression
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="results"></param>
        public ResultList(params Result[] results)
        {
            _nodetype = otXPTNodeType.ResultList;
            foreach (INode aNode in results) _Nodes.Add(aNode);
        }
        public ResultList(List<Result> results)
        {
            _nodetype = otXPTNodeType.ResultList;
            foreach (INode aNode in results) _Nodes.Add(aNode);
        }
        public ResultList(params INode[] results)
        {
            _nodetype = otXPTNodeType.ResultList;
            foreach (INode aNode in results) _Nodes.Add(aNode);
        }
        public ResultList(List<INode> results)
        {
            _nodetype = otXPTNodeType.ResultList;
            foreach (INode aNode in results) _Nodes.Add(aNode);
        }

        /// <summary>
        /// adds a ResultNode to the result list
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Boolean  Add(INode node)
        {
            if (node.NodeTokenType == otXPTNodeType.ResultList)
            {
                throw new RulezException(RulezException.Types.InvalidOperandNodeType, arguments: new object[] { otXPTNodeType.ResultList .ToString(), otXPTNodeType.Result .ToString() });
                
            }else if (node.NodeTokenType != otXPTNodeType.Result)
            {
                String anID = (_Nodes.Count + 1).ToString();
                _Nodes.Add(new Result(ID:anID,node: node));

            }else if (node.NodeTokenType == otXPTNodeType.Result)
            {
                // check entries
                foreach (Result aNode in _Nodes) if ((node as Result).ID == aNode.ID)
                        throw new RulezException(RulezException.Types.IdExists, arguments: new object[] { (node as Result).ID });
                _Nodes.Add(node);
            }
            return true;
        }

        /// <summary>
        /// return a unique list of used objectnames in the result list
        /// </summary>
        public List<String> Objectnames ()
        {
            List<String> aList = new List<String>();
            foreach (Result aNode in _Nodes)
            {
                foreach (String aName in aNode.Objectnames )
                    if ( !aList.Contains(aName)) aList.Add(aName);
            }
            // return list
            return aList;
        }
    }
    /// <summary>
    /// defines a logical selection expression
    /// </summary>
    public class SelectionRule : Rule
    {
        private Dictionary<string, ISymbol> _parameters = new Dictionary<string, ISymbol>(); // parameters
        private ResultList _result;
        private LogicalExpression  _SelectionExpression;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="handle"></param>
        public SelectionRule(string id = null,  Engine engine = null): base(id, engine)
        {
            _nodetype = otXPTNodeType.SelectionRule;
        }
       

        /// <summary>
        /// gets or sets the result (which is a ResultList)
        /// </summary>
        public ResultList Result { get { return _result; } set { _result = value; } }

        /// <summary>
        /// gets or sets the logical operation
        /// </summary>
        public LogicalExpression  Selection { get { return _SelectionExpression; } set { _SelectionExpression = value; } }

        /// <summary>
        /// gets the list of parameters
        /// </summary>
        public IEnumerable<ISymbol> Parameters { get { return _parameters.Values.ToList(); } }
        
        /// <summary>
        /// returns a List of objectnames retrieved with this selection
        /// </summary>
        /// <returns></returns>
        public List<String> ResultingObjectnames ()
        {
            List<String> aList = new List<String>();

            /// collect unique alle the objectnames in the result nodes
            /// 
            foreach (Result aNode in _result)
            {
                if (aNode.Objectnames.Count != 0) foreach(String aName in aNode.Objectnames) if (!aList.Contains(aName)) aList.Add(aName);
            }

            return aList;
        }
        /// <summary>
        /// Adds a Parameter to the Selection Rule
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ISymbol AddNewParameter(string id, otDataType type)
        {
            if (_parameters.ContainsKey(id))
            {
                throw new RulezException(RulezException.Types.IdExists, arguments: new object[] { id, this.ID });
            }
            Variable aVar = new Variable(id:id, Type:type, scope:this, engine:this.Engine );
             _parameters.Add(aVar.ID,aVar);

             return aVar;
        }

       
    }
}