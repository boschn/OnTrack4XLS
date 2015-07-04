/**
 *  ONTRACK RULEZ ENGINE
 *  
 * rulez engine
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
    /// an engine for running rulez
    /// </summary>
    public class Engine
    {
        private Repository _repository; // the repository
        private string _id; // handle of the engine
        private Context _context;
        private Dictionary<String, ICodeBit> _Code; // Code Dicitionary
        private List<iDataObjectEngine> _dataobjectEngines; // DataObject Engines for running data object against against
        /// <summary>
        /// constructor of an engine
        /// </summary>
        public Engine (string id = null)
        {
            if (id == null) _id = System.Environment.MachineName  + "_" + DateTime.Now.ToString();
            else _id = id;
            _repository = new Repository ();
            _context = new Context(this);
            _dataobjectEngines = new List<iDataObjectEngine>();
            _Code = new Dictionary<string, ICodeBit>();
        }

        ///
        /// Properties
        /// 
        #region "Properties"

        /// <summary>
        /// gets the unique handle of the engine
        /// </summary>
        public string Id { get { return _id; } }

        /// <summary>
        /// returns the Repository of the Engine
        /// </summary>
        public Repository Repository { get { return _repository; } }

        #endregion

        /// <summary>
        /// Add a data object engine
        /// </summary>
        /// <param name="engine"></param>
        /// <returns></returns>
        public bool AddDataEngine(iDataObjectEngine engine)
        {
            Boolean result = false;

            if  (_dataobjectEngines.Where (x => x.ID == engine.ID).FirstOrDefault () == null)
                throw new RulezException(RulezException.Types.IdExists, arguments: new object[] { engine.ID , "DataEngines"});

            _dataobjectEngines.Add(engine);
            result &= _repository .RegisterDataObjectRepository (engine.Objects );
            return result;
        }
        /// <summary>
        /// Add a data object engine
        /// </summary>
        /// <param name="engine"></param>
        /// <returns></returns>
        public bool RemoveDataEngine(String id)
        {
            Boolean result = false;
            iDataObjectEngine aDataEngine = _dataobjectEngines.Where(x => x.ID == id).FirstOrDefault();

            if (aDataEngine != null)
                throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[] { id, "DataEngines" });


            result &= _dataobjectEngines.Remove(aDataEngine);
            result &= _repository.DeRegisterDataObjectRepository(aDataEngine.Objects);
            return result;
        }
        /// <summary>
        /// gets the ICodeBit of an handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private ICodeBit GetCode(string handle)
        {
            if (_Code.ContainsKey(handle)) return _Code[handle];
            return null;
        }
        /// <summary>
        /// adds or replaces a codebit
        /// </summary>
        /// <param name="theCode"></param>
        /// <returns></returns>
        private bool AddCode(ICodeBit code)
        {
            if (_Code.ContainsKey(code.Handle)) _Code.Remove(code.Handle);
             _Code.Add(key: code.Handle, value: code);
             return true;
        }
        /// <summary>
        /// returns a selection rule from the repository or creates a new one and returns this
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public SelectionRule GetSelectionRule(string id = null)
        {
            if (_repository .HasSelectionRule (id)) return _repository .GetSelectionRule (id);
            SelectionRule aRule = new SelectionRule(id);
            _repository.AddSelectionRule(aRule.ID, aRule);
            return aRule;
        }

        /// <summary>
        /// gets the Operator definition for the Token ID
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public Operator GetOperator (Token id)
        {
            if (_repository.HasOperator (id)) return _repository .GetOperator (id);
            return null;
        }
        /// <summary>
        /// gets the Operator definition for the Token ID
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public Function GetFunction(Token id)
        {
            if (_repository.HasFunction(id)) return _repository.GetFunction(id);
            return null;
        }
        /// <summary>
        /// gets the Operator definition for the Token ID
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public iObjectDefinition GetDataObjectDefinition(string id)
        {
            if (_repository.HasDataObjectDefinition(id)) return _repository.GetDataObjectDefinition(id);
            return null;
        }

        /// <summary>
        /// Generate from a rule the intermediate Code and store it
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool Generate(eXPressionTree.Rule  rule)
        {
            ICodeBit code=null;
            bool result;
            try
            {
                switch (rule.NodeTokenType)
                {
                    // selection rule
                    case otXPTNodeType.SelectionRule:
                        result= Generate((rule as SelectionRule), out code);
                        break;
                    // no theCode
                    default:
                        throw new RulezException(RulezException.Types.InvalidNodeType, arguments: new object[] { rule.NodeTokenType.ToString(), "IRULE" });
                }

                // if successfull
                if (result) {
                    rule.RuleState = otRuleState.generatedCode ;
                    // save the theCode
                    if (!String.IsNullOrEmpty(code.Handle)) code.Handle = rule.Handle;

                    if (!String.IsNullOrEmpty(code.Handle)) AddCode(code);
                    else throw new RulezException(RulezException.Types.HandleNotDefined, arguments: new object[] { rule.ID});
                }
                return result;
               
            } catch (Exception ex )
              {
                  throw new RulezException(RulezException.Types.GenerateFailed, inner: ex);
               }
        }

        /// <summary>
        /// generate theCode for a selection rule
        /// </summary>
        /// <param name="selection"></param>
        /// <returns></returns>
        public bool Generate(SelectionRule selection, out ICodeBit code)
        {
            try
            {
                bool result = false;
                code = null;

                // check if the object to which data engine
                foreach (iDataObjectEngine anEngine in _dataobjectEngines.Reverse <iDataObjectEngine > () )
                {
                    foreach (String aName in selection.ResultingObjectnames () ) result &= anEngine.Objects.HasObjectDefinition(id: aName);
                    if (result) {
                        return anEngine.Generate((selection as eXPressionTree.Rule), out code);
                    }
                }

                // failure
                if (!result)
                {
                    String theNames = DataType.ToString(selection.ResultingObjectnames());
                    throw new RulezException(RulezException.Types.NoDataEngineAvailable, arguments: new object[] { theNames });
                }
                
                return false;
                


            }
            catch (Exception ex)
            {
                throw new RulezException(RulezException.Types.GenerateFailed, inner: ex);
            }
        }
        /// <summary>
        /// run a selection rule and return an ienumerable of IDataObjects
        /// </summary>
        /// <param name="ruleid"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable <iDataObject > RunSelectionRule (string ruleid, params object[] parameters)
        {
            SelectionRule aRule = this.GetSelectionRule(id: ruleid);
            // search the rule
            if (aRule == null)
                throw new RulezException(RulezException.Types.IdNotFound, arguments: new object[] { ruleid, "SelectionRule" });
            // not the required number of arguments
            if (parameters.Length != aRule.Parameters.Count())
                throw new RulezException (RulezException.Types.InvalidNumberOfArguments, arguments: new object[] {"SelectionRule", ruleid, aRule.Parameters .Count(), parameters.Length});
            // get the Codebit
            ICodeBit theCode = this.GetCode(aRule.Handle);
            if (theCode == null) throw new RulezException (RulezException.Types.HandleNotDefined , arguments: new object[] {aRule.ID});
            if (theCode.Code == null) throw new RulezException (RulezException.Types.InvalidCode, arguments: new object[]{aRule.ID, aRule.Handle});
            // push the arguments
            _context.PushParameters(parameters);
            try
            {
                // run the theCode
                if (theCode.Code(_context) == false) return null;
                // pop result
                IEnumerable<iDataObject> result = (_context.Pop() as IEnumerable<iDataObject>);
                return result;
            }
            catch (RulezException ex)
            {
                throw new RulezException(RulezException.Types.RunFailed, inner: ex, message: ex.Message);
            }
            catch (Exception ex)
            {
                throw new RulezException(RulezException.Types.RunFailed, inner: ex);
            }
        }
    }

    /// <summary>
    /// runtime Context for storing variables etc.
    /// </summary>
    public class Context
    {
        private Engine _engine ; // reference engine
        private Dictionary <String, object> _heap = new Dictionary<string,object> () ;
        private Stack<Object> _stack = new Stack<Object>();

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="engine"></param>
        public Context(Engine engine = null)
        {
            if (_engine != null) _engine = engine;
            else _engine = Rules.Engine;
        }

        /// <summary>
        /// gets the Stack of the Context
        /// </summary>
        public Stack<Object> Stack  { get {return _stack;} }
        /// <summary>
        /// pop from stack
        /// </summary>
        /// <returns></returns>
        public Object Pop ()
        {
            if (_stack.Count > 0) return _stack.Pop();
            throw new RulezException(RulezException.Types.StackUnderFlow, arguments: new Object[] { 1, _stack.Count });
        }
        /// <summary>
        /// pops no arguments from the stack as an array
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public Object[] PopParameters (uint no)
        {
            if (no > _stack.Count)
            {
                Object[] arr = {};
                Array.Resize<object>(ref arr, (int)no);
                for (uint  i=no;i>0;i--) arr[i-1] = _stack.Pop();
                return arr;
            }else throw new RulezException (RulezException.Types.StackUnderFlow, arguments: new Object[] {no, _stack.Count});
            
        }
        /// <summary>
        /// push an array on the stack - item by item
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public void PushParameters (object[] parameters)
        {
            if (parameters==null) return;
            for (Int16 i = 0; i < parameters.Length;i++ ) _stack.Push (parameters[i]);
        }
        /// <summary>
        /// push element on stack
        /// </summary>
        /// <param name="item"></param>
        public void Push(object item)
        {
            _stack.Push(item);
        }
        /// <summary>
        /// returns true if the heap has the handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool  HasItem(string id) {
            return _heap.ContainsKey(id);
        }
        /// <summary>
        /// returns true if item was added 
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool AddItem(string id, object value)
        {
            if (this.HasItem(id)) return false;
            _heap.Add(id, value);
            return true;
        }
        /// <summary>
        /// returns true if item was added 
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public Object Item(string id)
        {
            if (!this.HasItem(id)) return null;
           return _heap[id];
        }
        /// <summary>
        /// returns true if item was replaced 
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool ReplaceItem(string id, object value)
        {
            if (!this.HasItem(id)) return false;
            this.RemoveItem(id);
            _heap.Add(id, value);
            return true;
        }
        /// <summary>
        /// returns true if the item by handle was removed
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool RemoveItem(string id)
        {
            if (this.HasItem(id)) return false;
            _heap.Remove(id);
            return true;
        }

        /// <summary>
        /// return the item names
        /// </summary>
        public List<String> Itemnames { get { return _heap.Keys.ToList(); } }
        /// <summary>
        /// return the item values
        /// </summary>
        public List<Object> Itemvalues { get { return _heap.Values.ToList(); } }
    }

    /// <summary>
    /// theCode bit is an executable object to run a rule
    /// </summary>
    public class CodeBit : ICodeBit
    {
        protected string _handle;
        protected object _tag;
        protected Func<Context, Boolean> _code;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="handle"></param>
        public CodeBit (string handle=null)
        {
            if (handle == null) _handle = new Guid().ToString();
        }
        /// <summary>
        /// gets the unique handle of the codebit
        /// </summary>
        public string Handle { get { return _handle; } set { _handle = value; } }

        /// <summary>
        /// gets and set an arbitary object for the theCode generator
        /// </summary>
        public object Tag { get { return _tag; } set { _tag = value; } }
       
        /// <summary>
        /// gets and sets the executable theCode
        /// </summary>
        public Func<Context, Boolean> Code { get { return _code; } set { _code = value; } }
        
    }
}