﻿using Soheil.Core.Base;
using Soheil.Model;

namespace Soheil.Core.ViewModels.InfoViewModels
{
    public class UnitSetInfoVM : ViewModelBase, IInfoViewModel
    {
        private readonly UnitSet _model;

        public int Id
        {
            get { return _model.Id; } 

        }
        public string Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
        }
        public string Name
        {
            get { return _model.Description; }
            set { _model.Description = value; OnPropertyChanged("Name"); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitSetInfoVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        public UnitSetInfoVM(UnitSet entity)
        {
            _model = entity;
        }
    }
}