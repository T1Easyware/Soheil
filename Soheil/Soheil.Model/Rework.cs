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
    public partial class Rework
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
    
        public virtual System.DateTime CreatedDate
        {
            get;
            set;
        }
    
        public virtual System.DateTime ModifiedDate
        {
            get;
            set;
        }
    
        public virtual byte Status
        {
            get { return _status; }
            set { _status = value; }
        }
        private byte _status = 1;
    
        public virtual int ModifiedBy
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual ICollection<ProductRework> ProductReworks
        {
            get
            {
                if (_productReworks == null)
                {
                    var newCollection = new FixupCollection<ProductRework>();
                    newCollection.CollectionChanged += FixupProductReworks;
                    _productReworks = newCollection;
                }
                return _productReworks;
            }
            set
            {
                if (!ReferenceEquals(_productReworks, value))
                {
                    var previousValue = _productReworks as FixupCollection<ProductRework>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupProductReworks;
                    }
                    _productReworks = value;
                    var newValue = value as FixupCollection<ProductRework>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupProductReworks;
                    }
                }
            }
        }
        private ICollection<ProductRework> _productReworks;

        #endregion

        #region Association Fixup
    
        private void FixupProductReworks(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ProductRework item in e.NewItems)
                {
                    item.Rework = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ProductRework item in e.OldItems)
                {
                    if (ReferenceEquals(item.Rework, this))
                    {
                        item.Rework = null;
                    }
                }
            }
        }

        #endregion

    }
}
