/**
 *  ONTRACK RULEZ ENGINE
 *  
 * Abstract Syntax Tree Visitor
 * 
 * Version: 1.0
 * Created: 2015-05-14
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
    /// Event Args for Visitor 
    /// </summary>
    public class VisitorEventArgs<T> : EventArgs
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="currentNode"></param>
        public VisitorEventArgs (INode currentNode = null, Stack<T> stack = null)
        {
            if (currentNode != null) this.CurrentNode = currentNode;
            if (stack != null) this.Stack = stack;

        }
        /// <summary>
        /// Stack to store results
        /// </summary>
        public Stack<T> Stack { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public INode CurrentNode { get; set; }
    }

    /// <summary>
    /// vistor pattern class for the rulez xpression tree.
    /// make the stack of the visitor
    /// </summary>
    public class Visitor<T> : IVisitor
    {
        // declare events
        public delegate void Eventhandler(object o, VisitorEventArgs<T> e);
        public event Eventhandler VisitingExpression;
        public event Eventhandler VisitedExpression;

        public event Eventhandler VisitingSelectionRule;
        public event Eventhandler VisitedSelectionRule;

        public event Eventhandler VisitingLogicalExpression;
        public event Eventhandler VisitedLogicalExpression;

        public event Eventhandler VisitingOperationExpression;
        public event Eventhandler VisitedOperationExpression;

        public event Eventhandler VisitingFunctionCall;
        public event Eventhandler VisitedFunctionCall;

        /// <summary>
        /// end nodes
        /// </summary>
        public event Eventhandler VisitingNode;
        public event Eventhandler VisitingLiteral;
        public event Eventhandler VisitingVariable;
        public event Eventhandler VisitingDataObjectSymbol;
        //
        private Stack<T> _stack = new Stack <T>();
        private object _result;

        /// <summary>
        /// return the Result of a run
        /// </summary>
        public Object Result { get { return _result; } }
        /// <summary>
        /// returns the stack of the visitor
        /// </summary>
        public Stack<T> @Stack { get { return _stack; } }

        /// <summary>
        /// visit LogicalExpression
        /// </summary>
        /// <param name="expression"></param>
        public void Visit(SelectionRule rule)
        {
            VisitorEventArgs<T> args = new VisitorEventArgs<T>(currentNode: rule, stack: _stack);
            // call the event
            VisitingSelectionRule(this, args);

            // visit subnodes from left to right
            Visit(rule.Result );
            Visit(rule.Selection);

            // call the event
            VisitedSelectionRule(this, args);
        }

        /// <summary>
        /// visit LogicalExpression
        /// </summary>
        /// <param name="expression"></param>
        public void Visit(LogicalExpression expression)
        {
            VisitorEventArgs<T> args = new VisitorEventArgs<T>(currentNode: expression, stack: _stack);
            // call the event
            VisitingLogicalExpression(this, args);

            // visit subnodes from left to right
            if (expression.Operator.Arguments >= 1) Visit(expression.LeftOperand);
            if (expression.Operator.Arguments >= 2) Visit(expression.RightOperand);

            // call the event
            VisitedLogicalExpression(this, args);
        }
        /// <summary>
        /// visit operation Selection
        /// </summary>
        /// <param name="expression"></param>
        public void Visit(OperationExpression expression)
        {
            VisitorEventArgs<T> args = new VisitorEventArgs<T>(currentNode: expression, stack: _stack);
            // call the event
            VisitingOperationExpression(this, args);

            // visit subnodes from left to right
            if (expression.Operator.Arguments >= 1) Visit(expression.LeftOperand);
            if (expression.Operator.Arguments >= 2) Visit(expression.RightOperand);

            // call the event
            VisitedOperationExpression(this, args);
        }
        /// <summary>
        /// visit operation Selection
        /// </summary>
        /// <param name="expression"></param>
        public void Visit(FunctionCall call)
        {
            VisitorEventArgs<T> args = new VisitorEventArgs<T>(currentNode: call, stack: _stack);
            // call the event
            VisitingFunctionCall(this, args);

            // visit subnodes 
            foreach (INode node in call.Nodes )
            {
                 Visit (node);
            }

            // call the event
            VisitedFunctionCall(this, args);
        }
        /// <summary>
        ///  visit the expression
        /// </summary>
        /// <param name="expression"></param>
        public void Visit(IExpression expression)
        {
            switch (expression.NodeTokenType)
            {
                    // check the expression nodes
                case otXPTNodeType.OperationExpression :
                    Visit((OperationExpression )expression);
                    break;
                case otXPTNodeType.LogicalExpression:
                    Visit((LogicalExpression)expression);
                    break;
                case otXPTNodeType.FunctionCall :
                    Visit((FunctionCall)expression);
                    break;
                default:
                    throw new RulezException(RulezException.Types.InvalidOperandNodeType, Tag: "Visit");
            };
        }
        /// <summary>
        /// visit the literal
        /// </summary>
        /// <param name="literal"></param>
        public void Visit(Literal literal)
        {
            VisitorEventArgs<T> args = new VisitorEventArgs<T>(currentNode: literal, stack: _stack);
            VisitingLiteral(this, args);
        }
        /// <summary>
        /// visit the variable
        /// </summary>
        /// <param name="literal"></param>
        public void Visit(Variable  variable)
        {
            VisitorEventArgs<T> args = new VisitorEventArgs<T>(currentNode: variable, stack: _stack);
            VisitingVariable(this, args);
        }
        /// <summary>
        /// visit the variable
        /// </summary>
        /// <param name="literal"></param>
        public void Visit(DataObjectEntrySymbol symbol)
        {
            VisitorEventArgs<T> args = new VisitorEventArgs<T>(currentNode: symbol, stack: _stack);
            VisitingDataObjectSymbol(this, args);
        }
       /// <summary>
       /// visit a generic node
       /// </summary>
       /// <param name="node"></param>
        public void Visit(INode node)
        {
            switch (node.NodeTokenType )
            {
                    // check on the leaves
                case otXPTNodeType.Literal :
                     Visit((Literal)node);
                    break;
                case otXPTNodeType .Variable :
                    Visit((Variable)node);
                    break;
                case otXPTNodeType .DataObjectSymbol:
                    Visit((DataObjectEntrySymbol)node);
                    break;

                    // if not then check other possibilities
                default :
                    // if this is implementing an IExpression
                    if (node.GetType().GetInterfaces().Contains(typeof(IExpression))) Visit((IExpression)node);
                    // else throw exception
                    throw new RulezException ( RulezException.Types.InvalidOperandNodeType, Tag:"Visit");
            };
        }
    }
}