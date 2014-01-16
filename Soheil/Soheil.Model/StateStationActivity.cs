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
    public partial class StateStationActivity
    {
        #region Primitive Properties
    
        public virtual int Id
        {
            get;
            set;
        }
    
        public virtual float CycleTime
        {
            get;
            set;
        }
    
        public virtual float ManHour
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual StateStation StateStation
        {
            get { return _stateStation; }
            set
            {
                if (!ReferenceEquals(_stateStation, value))
                {
                    var previousValue = _stateStation;
                    _stateStation = value;
                    FixupStateStation(previousValue);
                }
            }
        }
        private StateStation _stateStation;
    
        public virtual ICollection<StateStationActivityMachine> StateStationActivityMachines
        {
            get
            {
                if (_stateStationActivityMachines == null)
                {
                    var newCollection = new FixupCollection<StateStationActivityMachine>();
                    newCollection.CollectionChanged += FixupStateStationActivityMachines;
                    _stateStationActivityMachines = newCollection;
                }
                return _stateStationActivityMachines;
            }
            set
            {
                if (!ReferenceEquals(_stateStationActivityMachines, value))
                {
                    var previousValue = _stateStationActivityMachines as FixupCollection<StateStationActivityMachine>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupStateStationActivityMachines;
                    }
                    _stateStationActivityMachines = value;
                    var newValue = value as FixupCollection<StateStationActivityMachine>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupStateStationActivityMachines;
                    }
                }
            }
        }
        private ICollection<StateStationActivityMachine> _stateStationActivityMachines;
    
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
    
        public virtual ICollection<Process> Processes
        {
            get
            {
                if (_processes == null)
                {
                    var newCollection = new FixupCollection<Process>();
                    newCollection.CollectionChanged += FixupProcesses;
                    _processes = newCollection;
                }
                return _processes;
            }
            set
            {
                if (!ReferenceEquals(_processes, value))
                {
                    var previousValue = _processes as FixupCollection<Process>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupProcesses;
                    }
                    _processes = value;
                    var newValue = value as FixupCollection<Process>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupProcesses;
                    }
                }
            }
        }
        private ICollection<Process> _processes;
    
        public virtual ICollection<UniqueActivitySkill> UniqueActivitySkills
        {
            get
            {
                if (_uniqueActivitySkills == null)
                {
                    var newCollection = new FixupCollection<UniqueActivitySkill>();
                    newCollection.CollectionChanged += FixupUniqueActivitySkills;
                    _uniqueActivitySkills = newCollection;
                }
                return _uniqueActivitySkills;
            }
            set
            {
                if (!ReferenceEquals(_uniqueActivitySkills, value))
                {
                    var previousValue = _uniqueActivitySkills as FixupCollection<UniqueActivitySkill>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupUniqueActivitySkills;
                    }
                    _uniqueActivitySkills = value;
                    var newValue = value as FixupCollection<UniqueActivitySkill>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupUniqueActivitySkills;
                    }
                }
            }
        }
        private ICollection<UniqueActivitySkill> _uniqueActivitySkills;

        #endregion

        #region Association Fixup
    
        private void FixupStateStation(StateStation previousValue)
        {
            if (previousValue != null && previousValue.StateStationActivities.Contains(this))
            {
                previousValue.StateStationActivities.Remove(this);
            }
    
            if (StateStation != null)
            {
                if (!StateStation.StateStationActivities.Contains(this))
                {
                    StateStation.StateStationActivities.Add(this);
                }
            }
        }
    
        private void FixupActivity(Activity previousValue)
        {
            if (previousValue != null && previousValue.StateStationActivities.Contains(this))
            {
                previousValue.StateStationActivities.Remove(this);
            }
    
            if (Activity != null)
            {
                if (!Activity.StateStationActivities.Contains(this))
                {
                    Activity.StateStationActivities.Add(this);
                }
            }
        }
    
        private void FixupStateStationActivityMachines(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (StateStationActivityMachine item in e.NewItems)
                {
                    item.StateStationActivity = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (StateStationActivityMachine item in e.OldItems)
                {
                    if (ReferenceEquals(item.StateStationActivity, this))
                    {
                        item.StateStationActivity = null;
                    }
                }
            }
        }
    
        private void FixupProcesses(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Process item in e.NewItems)
                {
                    item.StateStationActivity = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Process item in e.OldItems)
                {
                    if (ReferenceEquals(item.StateStationActivity, this))
                    {
                        item.StateStationActivity = null;
                    }
                }
            }
        }
    
        private void FixupUniqueActivitySkills(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (UniqueActivitySkill item in e.NewItems)
                {
                    item.StateStationActivity = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (UniqueActivitySkill item in e.OldItems)
                {
                    if (ReferenceEquals(item.StateStationActivity, this))
                    {
                        item.StateStationActivity = null;
                    }
                }
            }
        }

        #endregion
	}
}
