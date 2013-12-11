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
    public partial class WorkShift
    {
        #region Primitive Properties
    
        public virtual int Id
        {
            get;
            set;
        }
    
        public virtual int StartSeconds
        {
            get { return _startSeconds; }
            set { _startSeconds = value; }
        }
        private int _startSeconds = 0;
    
        public virtual int EndSeconds
        {
            get { return _endSeconds; }
            set { _endSeconds = value; }
        }
        private int _endSeconds = 0;

        #endregion

        #region Navigation Properties
    
        public virtual ICollection<WorkBreak> WorkBreaks
        {
            get
            {
                if (_workBreaks == null)
                {
                    var newCollection = new FixupCollection<WorkBreak>();
                    newCollection.CollectionChanged += FixupWorkBreaks;
                    _workBreaks = newCollection;
                }
                return _workBreaks;
            }
            set
            {
                if (!ReferenceEquals(_workBreaks, value))
                {
                    var previousValue = _workBreaks as FixupCollection<WorkBreak>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupWorkBreaks;
                    }
                    _workBreaks = value;
                    var newValue = value as FixupCollection<WorkBreak>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupWorkBreaks;
                    }
                }
            }
        }
        private ICollection<WorkBreak> _workBreaks;
    
        public virtual WorkDay WorkDay
        {
            get { return _workDay; }
            set
            {
                if (!ReferenceEquals(_workDay, value))
                {
                    var previousValue = _workDay;
                    _workDay = value;
                    FixupWorkDay(previousValue);
                }
            }
        }
        private WorkDay _workDay;
    
        public virtual WorkShiftPrototype WorkShiftPrototype
        {
            get { return _workShiftPrototype; }
            set
            {
                if (!ReferenceEquals(_workShiftPrototype, value))
                {
                    var previousValue = _workShiftPrototype;
                    _workShiftPrototype = value;
                    FixupWorkShiftPrototype(previousValue);
                }
            }
        }
        private WorkShiftPrototype _workShiftPrototype;

        #endregion

        #region Association Fixup
    
        private void FixupWorkDay(WorkDay previousValue)
        {
            if (previousValue != null && previousValue.WorkShifts.Contains(this))
            {
                previousValue.WorkShifts.Remove(this);
            }
    
            if (WorkDay != null)
            {
                if (!WorkDay.WorkShifts.Contains(this))
                {
                    WorkDay.WorkShifts.Add(this);
                }
            }
        }
    
        private void FixupWorkShiftPrototype(WorkShiftPrototype previousValue)
        {
            if (previousValue != null && previousValue.WorkShifts.Contains(this))
            {
                previousValue.WorkShifts.Remove(this);
            }
    
            if (WorkShiftPrototype != null)
            {
                if (!WorkShiftPrototype.WorkShifts.Contains(this))
                {
                    WorkShiftPrototype.WorkShifts.Add(this);
                }
            }
        }
    
        private void FixupWorkBreaks(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (WorkBreak item in e.NewItems)
                {
                    item.WorkShift = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (WorkBreak item in e.OldItems)
                {
                    if (ReferenceEquals(item.WorkShift, this))
                    {
                        item.WorkShift = null;
                    }
                }
            }
        }

        #endregion

    }
}
