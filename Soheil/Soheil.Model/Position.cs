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
    public partial class Position
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
    
        public virtual int ModifiedBy
        {
            get;
            set;
        }
    
        public virtual System.DateTime ModifiedDate
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual ICollection<User_Position> User_Positions
        {
            get
            {
                if (_user_Positions == null)
                {
                    var newCollection = new FixupCollection<User_Position>();
                    newCollection.CollectionChanged += FixupUser_Positions;
                    _user_Positions = newCollection;
                }
                return _user_Positions;
            }
            set
            {
                if (!ReferenceEquals(_user_Positions, value))
                {
                    var previousValue = _user_Positions as FixupCollection<User_Position>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupUser_Positions;
                    }
                    _user_Positions = value;
                    var newValue = value as FixupCollection<User_Position>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupUser_Positions;
                    }
                }
            }
        }
        private ICollection<User_Position> _user_Positions;
    
        public virtual ICollection<Position_AccessRule> Position_AccessRules
        {
            get
            {
                if (_position_AccessRules == null)
                {
                    var newCollection = new FixupCollection<Position_AccessRule>();
                    newCollection.CollectionChanged += FixupPosition_AccessRules;
                    _position_AccessRules = newCollection;
                }
                return _position_AccessRules;
            }
            set
            {
                if (!ReferenceEquals(_position_AccessRules, value))
                {
                    var previousValue = _position_AccessRules as FixupCollection<Position_AccessRule>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupPosition_AccessRules;
                    }
                    _position_AccessRules = value;
                    var newValue = value as FixupCollection<Position_AccessRule>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupPosition_AccessRules;
                    }
                }
            }
        }
        private ICollection<Position_AccessRule> _position_AccessRules;
    
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
    
        private void FixupUser_Positions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (User_Position item in e.NewItems)
                {
                    item.Position = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (User_Position item in e.OldItems)
                {
                    if (ReferenceEquals(item.Position, this))
                    {
                        item.Position = null;
                    }
                }
            }
        }
    
        private void FixupPosition_AccessRules(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Position_AccessRule item in e.NewItems)
                {
                    item.Position = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Position_AccessRule item in e.OldItems)
                {
                    if (ReferenceEquals(item.Position, this))
                    {
                        item.Position = null;
                    }
                }
            }
        }
    
        private void FixupOrganizationChart_Positions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (OrganizationChart_Position item in e.NewItems)
                {
                    item.Position = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (OrganizationChart_Position item in e.OldItems)
                {
                    if (ReferenceEquals(item.Position, this))
                    {
                        item.Position = null;
                    }
                }
            }
        }

        #endregion

    }
}
