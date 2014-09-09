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
    public partial class WarehouseTransaction
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
    
        public virtual byte Flow
        {
            get;
            set;
        }
    
        public virtual byte Type
        {
            get;
            set;
        }
    
        public virtual System.DateTime RecordDateTime
        {
            get;
            set;
        }
    
        public virtual System.DateTime TransactionDateTime
        {
            get;
            set;
        }
    
        public virtual int ModifiedBy
        {
            get;
            set;
        }
    
        public virtual double Quantity
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual Warehouse DestWarehouse
        {
            get { return _destWarehouse; }
            set
            {
                if (!ReferenceEquals(_destWarehouse, value))
                {
                    var previousValue = _destWarehouse;
                    _destWarehouse = value;
                    FixupDestWarehouse(previousValue);
                }
            }
        }
        private Warehouse _destWarehouse;
    
        public virtual WarehouseReceipt WarehouseReceipt
        {
            get { return _warehouseReceipt; }
            set
            {
                if (!ReferenceEquals(_warehouseReceipt, value))
                {
                    var previousValue = _warehouseReceipt;
                    _warehouseReceipt = value;
                    FixupWarehouseReceipt(previousValue);
                }
            }
        }
        private WarehouseReceipt _warehouseReceipt;
    
        public virtual Good Good
        {
            get { return _good; }
            set
            {
                if (!ReferenceEquals(_good, value))
                {
                    var previousValue = _good;
                    _good = value;
                    FixupGood(previousValue);
                }
            }
        }
        private Good _good;
    
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
    
        public virtual TaskReport TaskReport
        {
            get { return _taskReport; }
            set
            {
                if (!ReferenceEquals(_taskReport, value))
                {
                    var previousValue = _taskReport;
                    _taskReport = value;
                    FixupTaskReport(previousValue);
                }
            }
        }
        private TaskReport _taskReport;
    
        public virtual UnitSet UnitSet
        {
            get { return _unitSet; }
            set
            {
                if (!ReferenceEquals(_unitSet, value))
                {
                    var previousValue = _unitSet;
                    _unitSet = value;
                    FixupUnitSet(previousValue);
                }
            }
        }
        private UnitSet _unitSet;
    
        public virtual Warehouse SrcWarehouse
        {
            get { return _srcWarehouse; }
            set
            {
                if (!ReferenceEquals(_srcWarehouse, value))
                {
                    var previousValue = _srcWarehouse;
                    _srcWarehouse = value;
                    FixupSrcWarehouse(previousValue);
                }
            }
        }
        private Warehouse _srcWarehouse;

        #endregion

        #region Association Fixup
    
        private void FixupDestWarehouse(Warehouse previousValue)
        {
            if (previousValue != null && previousValue.DestWarehouseTransactions.Contains(this))
            {
                previousValue.DestWarehouseTransactions.Remove(this);
            }
    
            if (DestWarehouse != null)
            {
                if (!DestWarehouse.DestWarehouseTransactions.Contains(this))
                {
                    DestWarehouse.DestWarehouseTransactions.Add(this);
                }
            }
        }
    
        private void FixupWarehouseReceipt(WarehouseReceipt previousValue)
        {
            if (previousValue != null && previousValue.WarehouseTransactions.Contains(this))
            {
                previousValue.WarehouseTransactions.Remove(this);
            }
    
            if (WarehouseReceipt != null)
            {
                if (!WarehouseReceipt.WarehouseTransactions.Contains(this))
                {
                    WarehouseReceipt.WarehouseTransactions.Add(this);
                }
            }
        }
    
        private void FixupGood(Good previousValue)
        {
            if (previousValue != null && previousValue.WarehouseTransactions.Contains(this))
            {
                previousValue.WarehouseTransactions.Remove(this);
            }
    
            if (Good != null)
            {
                if (!Good.WarehouseTransactions.Contains(this))
                {
                    Good.WarehouseTransactions.Add(this);
                }
            }
        }
    
        private void FixupRawMaterial(RawMaterial previousValue)
        {
            if (previousValue != null && previousValue.WarehouseTransactions.Contains(this))
            {
                previousValue.WarehouseTransactions.Remove(this);
            }
    
            if (RawMaterial != null)
            {
                if (!RawMaterial.WarehouseTransactions.Contains(this))
                {
                    RawMaterial.WarehouseTransactions.Add(this);
                }
            }
        }
    
        private void FixupProductRework(ProductRework previousValue)
        {
            if (previousValue != null && previousValue.WarehouseTransactions.Contains(this))
            {
                previousValue.WarehouseTransactions.Remove(this);
            }
    
            if (ProductRework != null)
            {
                if (!ProductRework.WarehouseTransactions.Contains(this))
                {
                    ProductRework.WarehouseTransactions.Add(this);
                }
            }
        }
    
        private void FixupTaskReport(TaskReport previousValue)
        {
            if (previousValue != null && previousValue.WarehouseTransactions.Contains(this))
            {
                previousValue.WarehouseTransactions.Remove(this);
            }
    
            if (TaskReport != null)
            {
                if (!TaskReport.WarehouseTransactions.Contains(this))
                {
                    TaskReport.WarehouseTransactions.Add(this);
                }
            }
        }
    
        private void FixupUnitSet(UnitSet previousValue)
        {
            if (previousValue != null && previousValue.WarehouseTransactions.Contains(this))
            {
                previousValue.WarehouseTransactions.Remove(this);
            }
    
            if (UnitSet != null)
            {
                if (!UnitSet.WarehouseTransactions.Contains(this))
                {
                    UnitSet.WarehouseTransactions.Add(this);
                }
            }
        }
    
        private void FixupSrcWarehouse(Warehouse previousValue)
        {
            if (previousValue != null && previousValue.SrcWarehouseTransactions.Contains(this))
            {
                previousValue.SrcWarehouseTransactions.Remove(this);
            }
    
            if (SrcWarehouse != null)
            {
                if (!SrcWarehouse.SrcWarehouseTransactions.Contains(this))
                {
                    SrcWarehouse.SrcWarehouseTransactions.Add(this);
                }
            }
        }

        #endregion

    }
}
