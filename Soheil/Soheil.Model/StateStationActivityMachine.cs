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
    public partial class StateStationActivityMachine
    {
        #region Primitive Properties
    
        public virtual int Id
        {
            get;
            set;
        }
    
        public virtual bool IsFixed
        {
            get { return _isFixed; }
            set { _isFixed = value; }
        }
        private bool _isFixed = true;

        #endregion

        #region Navigation Properties
    
        public virtual StateStationActivity StateStationActivity
        {
            get { return _stateStationActivity; }
            set
            {
                if (!ReferenceEquals(_stateStationActivity, value))
                {
                    var previousValue = _stateStationActivity;
                    _stateStationActivity = value;
                    FixupStateStationActivity(previousValue);
                }
            }
        }
        private StateStationActivity _stateStationActivity;
    
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
    
        public virtual ICollection<SelectedMachine> SelectedMachines
        {
            get
            {
                if (_selectedMachines == null)
                {
                    var newCollection = new FixupCollection<SelectedMachine>();
                    newCollection.CollectionChanged += FixupSelectedMachines;
                    _selectedMachines = newCollection;
                }
                return _selectedMachines;
            }
            set
            {
                if (!ReferenceEquals(_selectedMachines, value))
                {
                    var previousValue = _selectedMachines as FixupCollection<SelectedMachine>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupSelectedMachines;
                    }
                    _selectedMachines = value;
                    var newValue = value as FixupCollection<SelectedMachine>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupSelectedMachines;
                    }
                }
            }
        }
        private ICollection<SelectedMachine> _selectedMachines;
    
        public virtual ICollection<UniqueActivitySkill> UniqueActivitySkills
        {
            get
            {
                if (_uniqueActivitySkills == null)
                {
                    _uniqueActivitySkills = new FixupCollection<UniqueActivitySkill>();
                }
                return _uniqueActivitySkills;
            }
            set
            {
                _uniqueActivitySkills = value;
            }
        }
        private ICollection<UniqueActivitySkill> _uniqueActivitySkills;

        #endregion

        #region Association Fixup
    
        private void FixupStateStationActivity(StateStationActivity previousValue)
        {
            if (previousValue != null && previousValue.StateStationActivityMachines.Contains(this))
            {
                previousValue.StateStationActivityMachines.Remove(this);
            }
    
            if (StateStationActivity != null)
            {
                if (!StateStationActivity.StateStationActivityMachines.Contains(this))
                {
                    StateStationActivity.StateStationActivityMachines.Add(this);
                }
            }
        }
    
        private void FixupMachine(Machine previousValue)
        {
            if (previousValue != null && previousValue.StateStationActivityMachines.Contains(this))
            {
                previousValue.StateStationActivityMachines.Remove(this);
            }
    
            if (Machine != null)
            {
                if (!Machine.StateStationActivityMachines.Contains(this))
                {
                    Machine.StateStationActivityMachines.Add(this);
                }
            }
        }
    
        private void FixupSelectedMachines(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SelectedMachine item in e.NewItems)
                {
                    item.StateStationActivityMachine = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SelectedMachine item in e.OldItems)
                {
                    if (ReferenceEquals(item.StateStationActivityMachine, this))
                    {
                        item.StateStationActivityMachine = null;
                    }
                }
            }
        }

        #endregion

    }
}
