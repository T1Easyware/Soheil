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
    public partial class Block
    {
        #region Primitive Properties
    
        public virtual int Id
        {
            get;
            set;
        }
    
        public virtual int DurationSeconds
        {
            get;
            set;
        }
    
        public virtual System.DateTime StartDateTime
        {
            get;
            set;
        }
    
        public virtual string Code
        {
            get;
            set;
        }
    
        public virtual int BlockTargetPoint
        {
            get;
            set;
        }
    
        public virtual System.DateTime EndDateTime
        {
            get;
            set;
        }
    
        public virtual int ModifiedBy
        {
            get;
            set;
        }
    
        private byte PPFlagsNr
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
    
        public virtual ICollection<Task> Tasks
        {
            get
            {
                if (_tasks == null)
                {
                    var newCollection = new FixupCollection<Task>();
                    newCollection.CollectionChanged += FixupTasks;
                    _tasks = newCollection;
                }
                return _tasks;
            }
            set
            {
                if (!ReferenceEquals(_tasks, value))
                {
                    var previousValue = _tasks as FixupCollection<Task>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTasks;
                    }
                    _tasks = value;
                    var newValue = value as FixupCollection<Task>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTasks;
                    }
                }
            }
        }
        private ICollection<Task> _tasks;
    
        public virtual Job Job
        {
            get { return _job; }
            set
            {
                if (!ReferenceEquals(_job, value))
                {
                    var previousValue = _job;
                    _job = value;
                    FixupJob(previousValue);
                }
            }
        }
        private Job _job;
    
        public virtual Education Education
        {
            get { return _education; }
            set
            {
                if (!ReferenceEquals(_education, value))
                {
                    var previousValue = _education;
                    _education = value;
                    FixupEducation(previousValue);
                }
            }
        }
        private Education _education;
    
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

        #endregion

        #region Association Fixup
    
        private void FixupJob(Job previousValue)
        {
            if (previousValue != null && previousValue.Blocks.Contains(this))
            {
                previousValue.Blocks.Remove(this);
            }
    
            if (Job != null)
            {
                if (!Job.Blocks.Contains(this))
                {
                    Job.Blocks.Add(this);
                }
            }
        }
    
        private void FixupEducation(Education previousValue)
        {
            if (previousValue != null && previousValue.Blocks.Contains(this))
            {
                previousValue.Blocks.Remove(this);
            }
    
            if (Education != null)
            {
                if (!Education.Blocks.Contains(this))
                {
                    Education.Blocks.Add(this);
                }
            }
        }
    
        private void FixupStateStation(StateStation previousValue)
        {
            if (previousValue != null && previousValue.Blocks.Contains(this))
            {
                previousValue.Blocks.Remove(this);
            }
    
            if (StateStation != null)
            {
                if (!StateStation.Blocks.Contains(this))
                {
                    StateStation.Blocks.Add(this);
                }
            }
        }
    
        private void FixupTasks(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Task item in e.NewItems)
                {
                    item.Block = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Task item in e.OldItems)
                {
                    if (ReferenceEquals(item.Block, this))
                    {
                        item.Block = null;
                    }
                }
            }
        }

        #endregion

    }
}
