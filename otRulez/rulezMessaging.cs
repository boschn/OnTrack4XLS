/**
 *  ONTRACK RULEZ ENGINE
 *  
 * rulez messaging and exceptions
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

namespace OnTrack.Rulez
{
    /// <summary>
    /// defines the exception
    /// </summary>
    public class RulezException : Exception
    {
        /// <summary>
        /// RulezException Types
        /// </summary>
        public enum Types
        {
            None = 0,
            NullArgument = 1,
            InvalidOperandNodeType,
            OutOfArraySize,
            OperatorNotDefined,
            OperandsNotEqualOperatorDefinition,
            OperandNull,
            IdNotFound,
            IdExists,
            OperatorTypeNotExpected,
            ValueNotConvertible,
            InvalidNodeType,
            GenerateFailed,
            RunFailed,
            NoDataEngineAvailable,
            StackUnderFlow,
            StackOverFlow,
            HandleNotDefined,
            InvalidNumberOfArguments,
            InvalidCode,
        }

        private static string[] _messages = {
                                         // None
                                         String.Empty,
                                         // NullArgument
                                         "invalid null argument",
                                         // InvalidOperandNodeType
                                         "invalid type of operand '{0}' - should be implementing INode or IExpression",
                                         // OutOfArraySize
                                         "index '{0}' greater than array size of '{1}'",
                                         // OperatorNotDefined
                                         "operator '{0}' is not defined",
                                         // OperandsNotEqualOperatorDefinition
                                         "for operator '{0}' are '{1}' operands necessary - '{2}' are supplied",
                                         // OperandsNotEqualOperatorDefinition
                                         "for operator '{0}' {1} operand must not be null",
                                         // IdNotFound
                                         "handle '{0}' was not found in context '{1}'",
                                         // IdExits
                                         "handle '{0}' is already defined in context '{1}'",
                                         // OperatorType not Expected
                                         "operator '{0}' is not of expected type {1}",
                                         // ValueNotConvertible
                                         "value '{0}' is not convertible to {1}",
                                          // InvalidNodeType
                                         "invalid type of node '{0}' - expected is {1}",
                                         // GenerateFailed
                                         "Generating rule theCode failed - see inner exception", 
                                         // RunFailed
                                         "Running a rule failed - see inner exception",
                                         // NoDataEngineAvailable
                                         "No data engine for object names '{0}' available",
                                         // Stack Underflow
                                         "Context Stack Underflow Error - no of elements on Stack {0} but {1} elements to be popped off",
                                         // Stack Overflow
                                         "Context Stack Overflow Error",
                                         // Handle not defined
                                         "Code handle for rule '{0}' is not existing in the engine or is null",
                                         // Invalid Number of Arguments
                                         "Rule '{1}' of type '{0}' is expecting {2} arguments - {3} supplied",
                                         // Invalid Code
                                         "Code of handle '{1}' of Rule '{0}' is invalid"
                                         };
        /// <summary>
        /// variables
        /// </summary>
        private Types _id;
        private String _message;
        private String _category;
        private String _Tag;
        private Exception _innerException;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="message"></param>
        public RulezException(Types id = 0, String message = null, String category = null, String Tag = null, Exception inner = null,  params object[] arguments)
        {
            _id = id;
            _category = category;
            _Tag = Tag;
            if (message == null) _message = message;
            else _message = _messages[(int)id];
            string.Format(_message, arguments);
            _innerException = inner;
        }

        /// <summary>
        /// gets the message string of the exception
        /// </summary>
        public String Message { get { return _message; } }

        /// <summary>
        /// gets the exception handle
        /// </summary>
        public Types ID { get { return _id; } }


        /// <summary>
        /// gets the category of the exception
        /// </summary>
        public String Category { get { return _category; } }


        /// <summary>
        /// gets the Tag string of the exception
        /// </summary>
        public String Tag { get { return _Tag; } }
    }
}
