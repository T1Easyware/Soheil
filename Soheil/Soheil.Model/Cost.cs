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
    public partial class Cost
    {
        #region Primitive Properties
    
        public virtual int Id
        {
            get;
            set;
        }
    
        public virtual Nullable<double> CostValue
        {
            get;
            set;
        }
    
        public virtual System.DateTime Date
        {
            get;
            set;
        }
    
        public virtual string Description
        {
            get;
            set;
        }
    
        public virtual Nullable<int> Quantity
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
    
        public virtual byte CostType
        {
            get { return _costType; }
            set { _costType = value; }
        }
        private byte _costType = 1;

        #endregion

        #region Navigation Properties
    
        public virtual Machine Machine
        {
            get { return _machine; }
            set
            {
                if (!ReferenceEquals(_machine, value))
                {
                    var previousValue = _machine;
                    _machine = value;
                    FixupMachine(previousValue);
                }
            }
        }
        private Machine _machine;
    
        public virtual CostCenter CostCenter
        {
            get { return _costCenter; }
            set
            {
                if (!ReferenceEquals(_costCenter, value))
                {
                    var previousValue = _costCenter;
                    _costCenter = value;
                    FixupCostCenter(previousValue);
                }
            }
        }
        private CostCenter _costCenter;
    
        public virtual PartWarehouse PartWarehouse
        {
            get { return _partWarehouse; }
            set
            {
                if (!ReferenceEquals(_partWarehouse, value))
                {
                    var previousValue = _partWarehouse;
                    _partWarehouse = value;
                    FixupPartWarehouse(previousValue);
                }
            }
        }
        private PartWarehouse _partWarehouse;
    
        public virtual Activity Activity
        {
            get { return _activity; }
            set
            {
                if (!ReferenceEquals(_activity, value))
                {
                    var previousValue = _activity;
                    _activity = value;
                    FixupActivity(previousValue);
                }
            }
        }
        private Activity _activity;
    
        public virtual Operator Operator
        {
            get { return _operator; }
            set
            {
                if (!ReferenceEquals(_operator, value))
                {
                    var previousValue = _operator;
                    _operator = value;
                    FixupOperator(previousValue);
                }
            }
        }
        private Operator _operator;
    
        public virtual Station Station
        {
            get { return _station; }
            set
            {
                if (!ReferenceEquals(_station, value))
                {
                    var previousValue = _station;
                    _station = value;
                    FixupStation(previousValue);
                }
            }
        }
        private Station _station;

        #endregion

        #region Association Fixup
    
        private void FixupMachine(Machine previousValue)
        {
            if (previousValue != null && previousValue.Costs.Contains(this))
            {
                previousValue.Costs.Remove(this);
            }
    
            if (Machine != null)
            {
                if (!Machine.Costs.Contains(this))
                {
                    Machine.Costs.Add(this);
                }
            }
        }
    
        private void FixupCostCenter(CostCenter previousValue)
        {
            if (previousValue != null && previousValue.Costs.Contains(this))
            {
                previousValue.Costs.Remove(this);
            }
    
            if (CostCenter != null)
            {
                if (!CostCenter.Costs.Contains(this))
                {
                    CostCenter.Costs.Add(this);
                }
            }
        }
    
        private void FixupPartWarehouse(PartWarehouse previousValue)
        {
            if (previousValue != null && ReferenceEquals(previousValue.Cost, this))
            {
                previousValue.Cost = null;
            }
    
            if (PartWarehouse != null)
            {
                PartWarehouse.Cost = this;
            }
        }
    
        private void FixupActivity(Activity previousValue)
        {
            if (previousValue != null && previousValue.Costs.Contains(this))
            {
                previousValue.Costs.Remove(this);
            }
    
            if (Activity != null)
            {
                if (!Activity.Costs.Contains(this))
                {
                    Activity.Costs.Add(this);
                }
            }
        }
    
        private void FixupOperator(Operator previousValue)
        {
            if (previousValue != null && previousValue.Costs.Contains(this))
            {
                previousValue.Costs.Remove(this);
            }
    
            if (Operator != null)
            {
                if (!Operator.Costs.Contains(this))
                {
                    Operator.Costs.Add(this);
                }
            }
        }
    
        private void FixupStation(Station previousValue)
        {
            if (previousValue != null && previousValue.Costs.Contains(this))
            {
                previousValue.Costs.Remove(this);
            }
    
            if (Station != null)
            {
                if (!Station.Costs.Contains(this))
                {
                    Station.Costs.Add(this);
                }
            }
        }

        #endregion

    }
}
