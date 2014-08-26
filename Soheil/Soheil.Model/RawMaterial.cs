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
    public partial class RawMaterial
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
    
        public virtual double Inventory
        {
            get;
            set;
        }
    
        public virtual int SafetyStock
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
    
        public virtual ICollection<WarehouseTransaction> WarehouseTransactions
        {
            get
            {
                if (_warehouseTransactions == null)
                {
                    var newCollection = new FixupCollection<WarehouseTransaction>();
                    newCollection.CollectionChanged += FixupWarehouseTransactions;
                    _warehouseTransactions = newCollection;
                }
                return _warehouseTransactions;
            }
            set
            {
                if (!ReferenceEquals(_warehouseTransactions, value))
                {
                    var previousValue = _warehouseTransactions as FixupCollection<WarehouseTransaction>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupWarehouseTransactions;
                    }
                    _warehouseTransactions = value;
                    var newValue = value as FixupCollection<WarehouseTransaction>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupWarehouseTransactions;
                    }
                }
            }
        }
        private ICollection<WarehouseTransaction> _warehouseTransactions;
    
        public virtual ICollection<RawMaterialUnitGroup> RawMaterialUnitGroups
        {
            get
            {
                if (_rawMaterialUnitGroups == null)
                {
                    var newCollection = new FixupCollection<RawMaterialUnitGroup>();
                    newCollection.CollectionChanged += FixupRawMaterialUnitGroups;
                    _rawMaterialUnitGroups = newCollection;
                }
                return _rawMaterialUnitGroups;
            }
            set
            {
                if (!ReferenceEquals(_rawMaterialUnitGroups, value))
                {
                    var previousValue = _rawMaterialUnitGroups as FixupCollection<RawMaterialUnitGroup>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupRawMaterialUnitGroups;
                    }
                    _rawMaterialUnitGroups = value;
                    var newValue = value as FixupCollection<RawMaterialUnitGroup>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupRawMaterialUnitGroups;
                    }
                }
            }
        }
        private ICollection<RawMaterialUnitGroup> _rawMaterialUnitGroups;

        #endregion

        #region Association Fixup
    
        private void FixupWarehouseTransactions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (WarehouseTransaction item in e.NewItems)
                {
                    item.RawMaterial = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (WarehouseTransaction item in e.OldItems)
                {
                    if (ReferenceEquals(item.RawMaterial, this))
                    {
                        item.RawMaterial = null;
                    }
                }
            }
        }
    
        private void FixupRawMaterialUnitGroups(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (RawMaterialUnitGroup item in e.NewItems)
                {
                    item.RawMaterial = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (RawMaterialUnitGroup item in e.OldItems)
                {
                    if (ReferenceEquals(item.RawMaterial, this))
                    {
                        item.RawMaterial = null;
                    }
                }
            }
        }

        #endregion

    }
}
