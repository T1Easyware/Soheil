﻿using System;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class ReworkVM : ItemContentViewModel
    {
        #region Properties

        private Rework _model;

        public override int Id
        {
            get { return _model.Id; } set{}
            
        }

        public override string SearchItem {get {return Code + Name;} set{} }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtNameRequired")]
// ReSharper restore PropertyNotResolved
        public string Name
        {
            get { return _model.Name; }
            set { _model.Name = value; OnPropertyChanged("Name"); }
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ReworkDataService ReworkDataService { get; set; }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtCodeRequired")]
// ReSharper restore PropertyNotResolved
        public string Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
        }

        public Status Status
        {
            get { return (Status) _model.Status; }
            set { _model.Status = (byte)value; }
        }

        public DateTime CreatedDate
        {
            get { return _model.CreatedDate; }
            set { _model.CreatedDate = value; OnPropertyChanged("CreatedDate"); }
        }

        public DateTime ModifiedDate
        {
            get { return _model.ModifiedDate; }
            set { _model.ModifiedDate = value; OnPropertyChanged("ModifiedDate"); }
        }

        public string ModifiedBy
        {
            get { return LoginInfo.GetUsername(_model.ModifiedBy); }
            
        }

        public ReworkProductsVM ProductsVM { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public ReworkVM(AccessType access, ReworkDataService dataService):base(access)
        {
            InitializeData(dataService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public ReworkVM(Rework entity, AccessType access, ReworkDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
        }

        private void InitializeData(ReworkDataService dataService)
        {
            ReworkDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            ReworkDataService.AttachModel(_model);
            _model = ReworkDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }
        public override void Delete(object param)
        {
            _model.Status = (byte)Status.Deleted; ReworkDataService.AttachModel(_model);
        }
        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }

        public override void ViewItemLink(object param)
        {
            var relationIndex = Convert.ToInt32(param);
            switch (relationIndex)
            {
                case 0:
                    ProductsVM = new ReworkProductsVM(this, Access);
                    CurrentLink = ProductsVM;
                    break;
                case 1:
                    break;
            }
            base.ViewItemLink(param);
        }
        #endregion

        #region Static Methods
        public static Rework CreateNew(ReworkDataService dataService)
        {
            int id = dataService.AddModel(new Rework { Name = "جدید", Code = string.Empty, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = (byte)Status.Active });
            return dataService.GetSingle(id);
        }
        #endregion
    }
}