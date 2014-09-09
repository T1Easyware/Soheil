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
    public partial class Warehouse
    {
        #region Primitive Properties
    
        public virtual int Id
        {
            get;
            set;
        }
    
        public virtual string Name
        {
            get;
            set;
        }
    
        public virtual string Code
        {
            get;
            set;
        }
    
        public virtual string Location
        {
            get;
            set;
        }
    
        public virtual bool HasWIP
        {
            get;
            set;
        }
    
        public virtual bool HasFinalProduct
        {
            get;
            set;
        }
    
        public virtual bool HasRawMaterial
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
    
        public virtual ICollection<WarehouseTransaction> DestWarehouseTransactions
        {
            get
            {
                if (_destWarehouseTransactions == null)
                {
                    var newCollection = new FixupCollection<WarehouseTransaction>();
                    newCollection.CollectionChanged += FixupDestWarehouseTransactions;
                    _destWarehouseTransactions = newCollection;
                }
                return _destWarehouseTransactions;
            }
            set
            {
                if (!ReferenceEquals(_destWarehouseTransactions, value))
                {
                    var previousValue = _destWarehouseTransactions as FixupCollection<WarehouseTransaction>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupDestWarehouseTransactions;
                    }
                    _destWarehouseTransactions = value;
                    var newValue = value as FixupCollection<WarehouseTransaction>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupDestWarehouseTransactions;
                    }
                }
            }
        }
        private ICollection<WarehouseTransaction> _destWarehouseTransactions;
    
        public virtual ICollection<WarehouseTransaction> SrcWarehouseTransactions
        {
            get
            {
                if (_srcWarehouseTransactions == null)
                {
                    var newCollection = new FixupCollection<WarehouseTransaction>();
                    newCollection.CollectionChanged += FixupSrcWarehouseTransactions;
                    _srcWarehouseTransactions = newCollection;
                }
                return _srcWarehouseTransactions;
            }
            set
            {
                if (!ReferenceEquals(_srcWarehouseTransactions, value))
                {
                    var previousValue = _srcWarehouseTransactions as FixupCollection<WarehouseTransaction>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupSrcWarehouseTransactions;
                    }
                    _srcWarehouseTransactions = value;
                    var newValue = value as FixupCollection<WarehouseTransaction>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupSrcWarehouseTransactions;
                    }
                }
            }
        }
        private ICollection<WarehouseTransaction> _srcWarehouseTransactions;

        #endregion

        #region Association Fixup
    
        private void FixupDestWarehouseTransactions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (WarehouseTransaction item in e.NewItems)
                {
                    item.DestWarehouse = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (WarehouseTransaction item in e.OldItems)
                {
                    if (ReferenceEquals(item.DestWarehouse, this))
                    {
                        item.DestWarehouse = null;
                    }
                }
            }
        }
    
        private void FixupSrcWarehouseTransactions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (WarehouseTransaction item in e.NewItems)
                {
                    item.SrcWarehouse = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (WarehouseTransaction item in e.OldItems)
                {
                    if (ReferenceEquals(item.SrcWarehouse, this))
                    {
                        item.SrcWarehouse = null;
                    }
                }
            }
        }

        #endregion

    }
}
