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
    public partial class BOM
    {
        #region Primitive Properties
    
        public virtual int Id
        {
            get;
            set;
        }
    
        public virtual string Code
        {
            get;
            set;
        }
    
        public virtual string Name
        {
            get;
            set;
        }
    
        public virtual float Quantity
        {
            get;
            set;
        }
    
        public virtual bool IsDefault
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual State State
        {
            get { return _state; }
            set
            {
                if (!ReferenceEquals(_state, value))
                {
                    var previousValue = _state;
                    _state = value;
                    FixupState(previousValue);
                }
            }
        }
        private State _state;
    
        public virtual RawMaterial RawMaterial
        {
            get { return _rawMaterial; }
            set
            {
                if (!ReferenceEquals(_rawMaterial, value))
                {
                    var previousValue = _rawMaterial;
                    _rawMaterial = value;
                    FixupRawMaterial(previousValue);
                }
            }
        }
        private RawMaterial _rawMaterial;

        #endregion

        #region Association Fixup
    
        private void FixupState(State previousValue)
        {
            if (previousValue != null && previousValue.BOMs.Contains(this))
            {
                previousValue.BOMs.Remove(this);
            }
    
            if (State != null)
            {
                if (!State.BOMs.Contains(this))
                {
                    State.BOMs.Add(this);
                }
            }
        }
    
        private void FixupRawMaterial(RawMaterial previousValue)
        {
            if (previousValue != null && previousValue.BOMs.Contains(this))
            {
                previousValue.BOMs.Remove(this);
            }
    
            if (RawMaterial != null)
            {
                if (!RawMaterial.BOMs.Contains(this))
                {
                    RawMaterial.BOMs.Add(this);
                }
            }
        }

        #endregion

    }
}
