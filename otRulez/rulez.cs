/**
 *  ONTRACK RULEZ ENGINE
 *  
 * rulez static core
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

namespace OnTrack
{
    /// <summary>
    /// static definitions
    /// </summary>
    static public class Rules
    {
        // skeleton engine
        static private OnTrack.Rulez.Engine _engine ;

        /// <summary>
        /// gets the Engine
        /// </summary>
        public static OnTrack.Rulez.Engine Engine { 
            get 
            {
                // lazy initialization
                if (_engine == null) _engine = new Rulez.Engine();
                return _engine; 
            } 
        }
    }
}
