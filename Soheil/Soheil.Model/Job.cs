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
    public partial class Job
    {
        #region Primitive Properties
    
        public virtual int Id
        {
            get;
            set;
        }
    
        public virtual System.DateTime Deadline
        {
            get;
            set;
        }
    
        public virtual float Weight
        {
            get;
            set;
        }
    
        public virtual int Quantity
        {
            get;
            set;
        }
    
        public virtual string Description
        {
            get;
            set;
        }
    
        public virtual string Code
        {
            get;
            set;
        }
    
        public virtual System.DateTime ReleaseTime
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual ProductRework ProductRework
        {
            get { return _productRework; }
            set
            {
                if (!ReferenceEquals(_productRework, value))
                {
                    var previousValue = _productRework;
                    _productRework = value;
                    FixupProductRework(previousValue);
                }
            }
        }
        private ProductRework _productRework;
    
        public virtual FPC FPC
        {
            get { return _fPC; }
            set
            {
                if (!ReferenceEquals(_fPC, value))
                {
                    var previousValue = _fPC;
                    _fPC = value;
                    FixupFPC(previousValue);
                }
            }
        }
        private FPC _fPC;
    
        public virtual ICollection<Block> Blocks
        {
            get
            {
                if (_blocks == null)
                {
                    var newCollection = new FixupCollection<Block>();
                    newCollection.CollectionChanged += FixupBlocks;
                    _blocks = newCollection;
                }
                return _blocks;
            }
            set
            {
                if (!ReferenceEquals(_blocks, value))
                {
                    var previousValue = _blocks as FixupCollection<Block>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupBlocks;
                    }
                    _blocks = value;
                    var newValue = value as FixupCollection<Block>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupBlocks;
                    }
                }
            }
        }
        private ICollection<Block> _blocks;

        #endregion

        #region Association Fixup
    
        private void FixupProductRework(ProductRework previousValue)
        {
            if (previousValue != null && previousValue.Jobs.Contains(this))
            {
                previousValue.Jobs.Remove(this);
            }
    
            if (ProductRework != null)
            {
                if (!ProductRework.Jobs.Contains(this))
                {
                    ProductRework.Jobs.Add(this);
                }
            }
        }
    
        private void FixupFPC(FPC previousValue)
        {
            if (previousValue != null && previousValue.Jobs.Contains(this))
            {
                previousValue.Jobs.Remove(this);
            }
    
            if (FPC != null)
            {
                if (!FPC.Jobs.Contains(this))
                {
                    FPC.Jobs.Add(this);
                }
            }
        }
    
        private void FixupBlocks(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Block item in e.NewItems)
                {
                    item.Job = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Block item in e.OldItems)
                {
                    if (ReferenceEquals(item.Job, this))
                    {
                        item.Job = null;
                    }
                }
            }
        }

        #endregion

    }
}
