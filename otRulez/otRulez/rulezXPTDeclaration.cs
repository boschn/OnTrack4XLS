/**
 *  ONTRACK RULEZ ENGINE
 *  
 * Abstract Syntax Tree Declaration
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
using System.Threading.Tasks;


namespace OnTrack.Rulez.eXPressionTree
{
    /// <summary>
    /// state of the Rule
    /// </summary>
    public enum otRuleState
    {
        created = 1,
        updated = 2,
        generatedCode = 4
    }
    /// <summary>
    /// token type of the Node
    /// </summary>
    public enum otXPTNodeType
    {
        Literal,
        Variable,
        Operand,
        Operation,
        LogicalExpression,
        OperationExpression,
        FunctionCall,
        DataObjectSymbol,
        Rule,
        SelectionRule,
        Result,
        ResultList
    }

    /// <summary>
    /// defines a tree visitor
    /// </summary>
    public interface IVisitor
    {
        /// <summary>
        /// returns the whatever result
        /// </summary>
        Object Result { get; }
        /// <summary>
        /// generic visit to a node
        /// </summary>
        /// <param name="node"></param>
        void Visit(INode node);
    }
    /// <summary>
    /// defines a node of the AST
    /// </summary>
    public interface INode : IEnumerable <INode>
    {
        /// <summary>
        /// gets the type of the node
        /// </summary>
        otXPTNodeType  NodeTokenType { get; }

        /// <summary>
        /// returns true if the node is a leaf
        /// </summary>
        bool HasSubNodes { get; }

        /// <summary>
        /// returns the engine
        /// </summary>
        Engine Engine { get; }

        /// <summary>
        /// accepts a visitor
        /// </summary>
        /// <param name="visitor"></param>
        bool Accept(IVisitor visitor);
    }
    /// <summary>
    /// describes an abstract syntax tree
    /// </summary>
    public interface IeXPressionTree: INode
    {
        /// <summary>
        /// gets and sets the list of nodes
        /// </summary>
        List<INode> Nodes { get; set; }
    }
    /// <summary>
    /// describes an abstract Selection 
    /// </summary>
    public interface IExpression: IeXPressionTree
    {
        /// <summary>
        /// gets or sets the operation of the LogicalExpression
        /// </summary>
        //Function Operator { get; }

        /// <summary>
        /// gets or sets the left Operand
        /// </summary>
        //INode LeftOperand { get; set; }

        /// <summary>
        /// gets or sets the right operand
        /// </summary>
        //INode RightOperand { get; set; }

       
        
    }
    /// <summary>
    /// describes a rule
    /// </summary>
    public interface IRule : IeXPressionTree
    {
        /// <summary>
        /// returns the ID of the rule
        /// </summary>
        String ID { get; set; }

        /// <summary>
        /// returns the state of the rule
        /// </summary>
        otRuleState RuleState { get; }
    }
    /// <summary>
    /// function calls
    /// </summary>
    public interface IFunction: IeXPressionTree
    {
        /// <summary>
        /// gets or sets the ID of the variable
        /// </summary>
        String ID { get; set; }
        /// <summary>
        /// gets or sets the type of the variable
        /// </summary>
        Core.otDataType Type { get; set; }
    }
    /// <summary>
    /// describes a expression tree symbol 
    /// </summary>
    public interface ISymbol : INode
    {
        /// <summary>
        /// gets or sets the ID of the variable
        /// </summary>
        String ID { get; set; }
        /// <summary>
        /// gets or sets the type of the variable
        /// </summary>
        Core.otDataType Type { get; set; }
        /// <summary>
        /// defines the IeXPressionTree scope of the symbol
        /// </summary>
        IeXPressionTree Scope { get; set; }
    }
}
