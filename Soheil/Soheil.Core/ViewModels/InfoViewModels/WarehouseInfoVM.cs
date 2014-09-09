using System;
using System.Collections.Generic;
using Soheil.Core.Base;
using Soheil.Model;

namespace Soheil.Core.ViewModels.InfoViewModels
{
    public class WarehouseInfoVM : ViewModelBase, IInfoViewModel
    {
        private Warehouse _model;

        public int Id
        {
            get { return _model.Id; } 

        }
        public string Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code");  }
        }
        public string Name
        {
            get { return _model.Name; }
            set { _model.Name = value; OnPropertyChanged("Name");  }
        }

        public string Text
        {
            get { return _model.Code + "-" + _model.Name; }
        }

        public Warehouse Model
        {
            get { return _model; }
            set { _model = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseInfoVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        public WarehouseInfoVM(Warehouse entity)
        {
            _model = entity;
        }
    }
}
