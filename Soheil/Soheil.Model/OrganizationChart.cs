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
    public partial class OrganizationChart
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
    
        public virtual byte Status
        {
            get { return _status; }
            set { _status = value; }
        }
        private byte _status = 1;
    
        public virtual System.DateTime ModifiedDate
        {
            get;
            set;
        }
    
        public virtual int ModifiedBy
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual ICollection<OrganizationChart_Position> OrganizationChart_Positions
        {
            get
            {
                if (_organizationChart_Positions == null)
                {
                    var newCollection = new FixupCollection<OrganizationChart_Position>();
                    newCollection.CollectionChanged += FixupOrganizationChart_Positions;
                    _organizationChart_Positions = newCollection;
                }
                return _organizationChart_Positions;
            }
            set
            {
                if (!ReferenceEquals(_organizationChart_Positions, value))
                {
                    var previousValue = _organizationChart_Positions as FixupCollection<OrganizationChart_Position>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupOrganizationChart_Positions;
                    }
                    _organizationChart_Positions = value;
                    var newValue = value as FixupCollection<OrganizationChart_Position>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupOrganizationChart_Positions;
                    }
                }
            }
        }
        private ICollection<OrganizationChart_Position> _organizationChart_Positions;

        #endregion

        #region Association Fixup
    
        private void FixupOrganizationChart_Positions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (OrganizationChart_Position item in e.NewItems)
                {
                    item.OrganizationChart = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (OrganizationChart_Position item in e.OldItems)
                {
                    if (ReferenceEquals(item.OrganizationChart, this))
                    {
                        item.OrganizationChart = null;
                    }
                }
            }
        }

        #endregion

    }
}
