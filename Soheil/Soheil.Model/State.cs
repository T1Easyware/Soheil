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
    public partial class State
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
    
        public virtual byte StateTypeNr
        {
            get;
            set;
        }
    
        public virtual float X
        {
            get;
            set;
        }
    
        public virtual float Y
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual ICollection<StateStation> StateStations
        {
            get
            {
                if (_stateStations == null)
                {
                    var newCollection = new FixupCollection<StateStation>();
                    newCollection.CollectionChanged += FixupStateStations;
                    _stateStations = newCollection;
                }
                return _stateStations;
            }
            set
            {
                if (!ReferenceEquals(_stateStations, value))
                {
                    var previousValue = _stateStations as FixupCollection<StateStation>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupStateStations;
                    }
                    _stateStations = value;
                    var newValue = value as FixupCollection<StateStation>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupStateStations;
                    }
                }
            }
        }
        private ICollection<StateStation> _stateStations;
    
        public virtual ICollection<Connector> OutConnectors
        {
            get
            {
                if (_outConnectors == null)
                {
                    var newCollection = new FixupCollection<Connector>();
                    newCollection.CollectionChanged += FixupOutConnectors;
                    _outConnectors = newCollection;
                }
                return _outConnectors;
            }
            set
            {
                if (!ReferenceEquals(_outConnectors, value))
                {
                    var previousValue = _outConnectors as FixupCollection<Connector>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupOutConnectors;
                    }
                    _outConnectors = value;
                    var newValue = value as FixupCollection<Connector>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupOutConnectors;
                    }
                }
            }
        }
        private ICollection<Connector> _outConnectors;
    
        public virtual ICollection<Connector> InConnectors
        {
            get
            {
                if (_inConnectors == null)
                {
                    var newCollection = new FixupCollection<Connector>();
                    newCollection.CollectionChanged += FixupInConnectors;
                    _inConnectors = newCollection;
                }
                return _inConnectors;
            }
            set
            {
                if (!ReferenceEquals(_inConnectors, value))
                {
                    var previousValue = _inConnectors as FixupCollection<Connector>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupInConnectors;
                    }
                    _inConnectors = value;
                    var newValue = value as FixupCollection<Connector>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupInConnectors;
                    }
                }
            }
        }
        private ICollection<Connector> _inConnectors;
    
        public virtual FPC FPC
        {
            get { return _fPC; }
            set
            {
                if (!ReferenceEquals(_fPC, value))
                {
                    var previousValue = _fPC;
                    _fPC = value;
                    FixupFPC(previousValue);
                }
            }
        }
        private FPC _fPC;
    
        public virtual ProductRework OnProductRework
        {
            get { return _onProductRework; }
            set
            {
                if (!ReferenceEquals(_onProductRework, value))
                {
                    var previousValue = _onProductRework;
                    _onProductRework = value;
                    FixupOnProductRework(previousValue);
                }
            }
        }
        private ProductRework _onProductRework;

        #endregion

        #region Association Fixup
    
        private void FixupFPC(FPC previousValue)
        {
            if (previousValue != null && previousValue.States.Contains(this))
            {
                previousValue.States.Remove(this);
            }
    
            if (FPC != null)
            {
                if (!FPC.States.Contains(this))
                {
                    FPC.States.Add(this);
                }
            }
        }
    
        private void FixupOnProductRework(ProductRework previousValue)
        {
            if (previousValue != null && previousValue.States.Contains(this))
            {
                previousValue.States.Remove(this);
            }
    
            if (OnProductRework != null)
            {
                if (!OnProductRework.States.Contains(this))
                {
                    OnProductRework.States.Add(this);
                }
            }
        }
    
        private void FixupStateStations(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (StateStation item in e.NewItems)
                {
                    item.State = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (StateStation item in e.OldItems)
                {
                    if (ReferenceEquals(item.State, this))
                    {
                        item.State = null;
                    }
                }
            }
        }
    
        private void FixupOutConnectors(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Connector item in e.NewItems)
                {
                    item.StartState = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Connector item in e.OldItems)
                {
                    if (ReferenceEquals(item.StartState, this))
                    {
                        item.StartState = null;
                    }
                }
            }
        }
    
        private void FixupInConnectors(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Connector item in e.NewItems)
                {
                    item.EndState = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Connector item in e.OldItems)
                {
                    if (ReferenceEquals(item.EndState, this))
                    {
                        item.EndState = null;
                    }
                }
            }
        }

        #endregion

    }
}
