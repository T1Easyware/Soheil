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
    public partial class Operator
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
    
        public virtual string Name
        {
            get;
            set;
        }
    
        public virtual float Score
        {
            get;
            set;
        }
    
        public virtual System.DateTime ModifiedDate
        {
            get;
            set;
        }
    
        public virtual System.DateTime CreatedDate
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
    
        public virtual bool Sex
        {
            get { return _sex; }
            set { _sex = value; }
        }
        private bool _sex = true;
    
        public virtual int Age
        {
            get { return _age; }
            set { _age = value; }
        }
        private int _age = 1;

        #endregion

        #region Navigation Properties
    
        public virtual ICollection<ProcessOperator> ProcessOperators
        {
            get
            {
                if (_processOperators == null)
                {
                    var newCollection = new FixupCollection<ProcessOperator>();
                    newCollection.CollectionChanged += FixupProcessOperators;
                    _processOperators = newCollection;
                }
                return _processOperators;
            }
            set
            {
                if (!ReferenceEquals(_processOperators, value))
                {
                    var previousValue = _processOperators as FixupCollection<ProcessOperator>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupProcessOperators;
                    }
                    _processOperators = value;
                    var newValue = value as FixupCollection<ProcessOperator>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupProcessOperators;
                    }
                }
            }
        }
        private ICollection<ProcessOperator> _processOperators;
    
        public virtual ICollection<OperatorStoppageReport> OperatorStoppageReports
        {
            get
            {
                if (_operatorStoppageReports == null)
                {
                    var newCollection = new FixupCollection<OperatorStoppageReport>();
                    newCollection.CollectionChanged += FixupOperatorStoppageReports;
                    _operatorStoppageReports = newCollection;
                }
                return _operatorStoppageReports;
            }
            set
            {
                if (!ReferenceEquals(_operatorStoppageReports, value))
                {
                    var previousValue = _operatorStoppageReports as FixupCollection<OperatorStoppageReport>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupOperatorStoppageReports;
                    }
                    _operatorStoppageReports = value;
                    var newValue = value as FixupCollection<OperatorStoppageReport>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupOperatorStoppageReports;
                    }
                }
            }
        }
        private ICollection<OperatorStoppageReport> _operatorStoppageReports;
    
        public virtual ICollection<EducatingOperator> EducatingOperators
        {
            get
            {
                if (_educatingOperators == null)
                {
                    var newCollection = new FixupCollection<EducatingOperator>();
                    newCollection.CollectionChanged += FixupEducatingOperators;
                    _educatingOperators = newCollection;
                }
                return _educatingOperators;
            }
            set
            {
                if (!ReferenceEquals(_educatingOperators, value))
                {
                    var previousValue = _educatingOperators as FixupCollection<EducatingOperator>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupEducatingOperators;
                    }
                    _educatingOperators = value;
                    var newValue = value as FixupCollection<EducatingOperator>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupEducatingOperators;
                    }
                }
            }
        }
        private ICollection<EducatingOperator> _educatingOperators;
    
        public virtual ICollection<Cost> Costs
        {
            get
            {
                if (_costs == null)
                {
                    var newCollection = new FixupCollection<Cost>();
                    newCollection.CollectionChanged += FixupCosts;
                    _costs = newCollection;
                }
                return _costs;
            }
            set
            {
                if (!ReferenceEquals(_costs, value))
                {
                    var previousValue = _costs as FixupCollection<Cost>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCosts;
                    }
                    _costs = value;
                    var newValue = value as FixupCollection<Cost>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCosts;
                    }
                }
            }
        }
        private ICollection<Cost> _costs;
    
        public virtual PersonalSkill PersonalSkill
        {
            get { return _personalSkill; }
            set
            {
                if (!ReferenceEquals(_personalSkill, value))
                {
                    var previousValue = _personalSkill;
                    _personalSkill = value;
                    FixupPersonalSkill(previousValue);
                }
            }
        }
        private PersonalSkill _personalSkill;
    
        public virtual ICollection<OperatorDefectionReport> OperatorDefectionReports
        {
            get
            {
                if (_operatorDefectionReports == null)
                {
                    var newCollection = new FixupCollection<OperatorDefectionReport>();
                    newCollection.CollectionChanged += FixupOperatorDefectionReports;
                    _operatorDefectionReports = newCollection;
                }
                return _operatorDefectionReports;
            }
            set
            {
                if (!ReferenceEquals(_operatorDefectionReports, value))
                {
                    var previousValue = _operatorDefectionReports as FixupCollection<OperatorDefectionReport>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupOperatorDefectionReports;
                    }
                    _operatorDefectionReports = value;
                    var newValue = value as FixupCollection<OperatorDefectionReport>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupOperatorDefectionReports;
                    }
                }
            }
        }
        private ICollection<OperatorDefectionReport> _operatorDefectionReports;
    
        public virtual ICollection<ActivitySkill> ActivitySkills
        {
            get
            {
                if (_activitySkills == null)
                {
                    var newCollection = new FixupCollection<ActivitySkill>();
                    newCollection.CollectionChanged += FixupActivitySkills;
                    _activitySkills = newCollection;
                }
                return _activitySkills;
            }
            set
            {
                if (!ReferenceEquals(_activitySkills, value))
                {
                    var previousValue = _activitySkills as FixupCollection<ActivitySkill>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupActivitySkills;
                    }
                    _activitySkills = value;
                    var newValue = value as FixupCollection<ActivitySkill>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupActivitySkills;
                    }
                }
            }
        }
        private ICollection<ActivitySkill> _activitySkills;

        #endregion

        #region Association Fixup
    
        private void FixupPersonalSkill(PersonalSkill previousValue)
        {
            if (previousValue != null && ReferenceEquals(previousValue.Operator, this))
            {
                previousValue.Operator = null;
            }
    
            if (PersonalSkill != null)
            {
                PersonalSkill.Operator = this;
            }
        }
    
        private void FixupProcessOperators(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ProcessOperator item in e.NewItems)
                {
                    item.Operator = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ProcessOperator item in e.OldItems)
                {
                    if (ReferenceEquals(item.Operator, this))
                    {
                        item.Operator = null;
                    }
                }
            }
        }
    
        private void FixupOperatorStoppageReports(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (OperatorStoppageReport item in e.NewItems)
                {
                    item.Operator = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (OperatorStoppageReport item in e.OldItems)
                {
                    if (ReferenceEquals(item.Operator, this))
                    {
                        item.Operator = null;
                    }
                }
            }
        }
    
        private void FixupEducatingOperators(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (EducatingOperator item in e.NewItems)
                {
                    item.Operator = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (EducatingOperator item in e.OldItems)
                {
                    if (ReferenceEquals(item.Operator, this))
                    {
                        item.Operator = null;
                    }
                }
            }
        }
    
        private void FixupCosts(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Cost item in e.NewItems)
                {
                    item.Operator = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Cost item in e.OldItems)
                {
                    if (ReferenceEquals(item.Operator, this))
                    {
                        item.Operator = null;
                    }
                }
            }
        }
    
        private void FixupOperatorDefectionReports(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (OperatorDefectionReport item in e.NewItems)
                {
                    item.Operator = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (OperatorDefectionReport item in e.OldItems)
                {
                    if (ReferenceEquals(item.Operator, this))
                    {
                        item.Operator = null;
                    }
                }
            }
        }
    
        private void FixupActivitySkills(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ActivitySkill item in e.NewItems)
                {
                    item.Operator = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ActivitySkill item in e.OldItems)
                {
                    if (ReferenceEquals(item.Operator, this))
                    {
                        item.Operator = null;
                    }
                }
            }
        }

        #endregion

    }
}
