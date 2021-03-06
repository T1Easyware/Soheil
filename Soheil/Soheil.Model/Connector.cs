//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Soheil.Model
{
    public partial class Connector
    {
        #region Primitive Properties
    
        public virtual int Id
        {
            get;
            set;
        }
    
        public virtual bool HasBuffer
        {
            get { return _hasBuffer; }
            set { _hasBuffer = value; }
        }
        private bool _hasBuffer = false;

        #endregion

        #region Navigation Properties
    
        public virtual State StartState
        {
            get { return _startState; }
            set
            {
                if (!ReferenceEquals(_startState, value))
                {
                    var previousValue = _startState;
                    _startState = value;
                    FixupStartState(previousValue);
                }
            }
        }
        private State _startState;
    
        public virtual State EndState
        {
            get { return _endState; }
            set
            {
                if (!ReferenceEquals(_endState, value))
                {
                    var previousValue = _endState;
                    _endState = value;
                    FixupEndState(previousValue);
                }
            }
        }
        private State _endState;

        #endregion

        #region Association Fixup
    
        private void FixupStartState(State previousValue)
        {
            if (previousValue != null && previousValue.OutConnectors.Contains(this))
            {
                previousValue.OutConnectors.Remove(this);
            }
    
            if (StartState != null)
            {
                if (!StartState.OutConnectors.Contains(this))
                {
                    StartState.OutConnectors.Add(this);
                }
            }
        }
    
        private void FixupEndState(State previousValue)
        {
            if (previousValue != null && previousValue.InConnectors.Contains(this))
            {
                previousValue.InConnectors.Remove(this);
            }
    
            if (EndState != null)
            {
                if (!EndState.InConnectors.Contains(this))
                {
                    EndState.InConnectors.Add(this);
                }
            }
        }

        #endregion

    }
}
