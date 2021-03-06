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
    public partial class Education : NonProductiveTask
    {
        #region Primitive Properties
    
        public virtual System.DateTimeOffset StartOffset
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual ICollection<EducatingOperator> EducatingOperators
        {
            get
            {
                if (_educatingOperators == null)
                {
                    var newCollection = new FixupCollection<EducatingOperator>();
                    newCollection.CollectionChanged += FixupEducatingOperators;
                    _educatingOperators = newCollection;
                }
                return _educatingOperators;
            }
            set
            {
                if (!ReferenceEquals(_educatingOperators, value))
                {
                    var previousValue = _educatingOperators as FixupCollection<EducatingOperator>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupEducatingOperators;
                    }
                    _educatingOperators = value;
                    var newValue = value as FixupCollection<EducatingOperator>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupEducatingOperators;
                    }
                }
            }
        }
        private ICollection<EducatingOperator> _educatingOperators;
    
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
    
        private void FixupEducatingOperators(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (EducatingOperator item in e.NewItems)
                {
                    item.Education = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (EducatingOperator item in e.OldItems)
                {
                    if (ReferenceEquals(item.Education, this))
                    {
                        item.Education = null;
                    }
                }
            }
        }
    
        private void FixupBlocks(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Block item in e.NewItems)
                {
                    item.Education = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Block item in e.OldItems)
                {
                    if (ReferenceEquals(item.Education, this))
                    {
                        item.Education = null;
                    }
                }
            }
        }

        #endregion

    }
}
