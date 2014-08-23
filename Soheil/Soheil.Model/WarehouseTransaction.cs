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
    
        public virtual byte TransactionType
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
    
        public virtual Warehouse Warehouse
        {
            get { return _warehouse; }
            set
            {
                if (!ReferenceEquals(_warehouse, value))
                {
                    var previousValue = _warehouse;
                    _warehouse = value;
                    FixupWarehouse(previousValue);
                }
            }
        }
        private Warehouse _warehouse;
    
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

        #endregion

        #region Association Fixup
    
        private void FixupWarehouse(Warehouse previousValue)
        {
            if (previousValue != null && previousValue.WarehouseTransactions.Contains(this))
            {
                previousValue.WarehouseTransactions.Remove(this);
            }
    
            if (Warehouse != null)
            {
                if (!Warehouse.WarehouseTransactions.Contains(this))
                {
                    Warehouse.WarehouseTransactions.Add(this);
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

        #endregion

    }
}
