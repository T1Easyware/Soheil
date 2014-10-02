using System;
using System.ComponentModel;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class ProductPriceVM : ItemContentViewModel
    {
        #region Properties
        private ProductPrice _model;

        public override int Id
        {
            get { return _model.Id; } set{}
            
        }

        public double Value
        {
            get { return _model.Value; }
            set { _model.Value = value; OnPropertyChanged("Value"); }
        }

        public DateTime StartDate
        {
            get { return _model.StartDateTime; }
            set { _model.StartDateTime = value; OnPropertyChanged("StartDate"); }
        }

        public string Text
        {
            get { return Value + " - " + StartDate.ToShortDateString(); }
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ProductPriceDataService ProductPriceDataService { get; set; }

        /// <summary>
        /// Gets or sets the ProductPrice group data service.
        /// </summary>
        /// <value>
        /// The ProductPrice group data service.
        /// </value>

        public Status Status
        {
            get { return (Status) _model.Status; }
            set { _model.Status = (byte) value; }
        }

        public DateTime CreatedDate
        {
            get { return _model.CreatedDate; }
            set { _model.CreatedDate = value;  OnPropertyChanged("CreatedDate"); }
        }

        [ReadOnly(true)]
        public DateTime ModifiedDate
        {
            get { return _model.ModifiedDate; }
            set { _model.ModifiedDate = value; OnPropertyChanged("ModifiedDate"); }
        }

        [ReadOnly(true)]
        public string ModifiedBy
        {
            get {return LoginInfo.GetUsername(_model.ModifiedBy);}
        }


        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductPriceVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public ProductPriceVM(ProductPrice entity, AccessType access, ProductPriceDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
        }
        
        private void InitializeData(ProductPriceDataService dataService)
        {
            ProductPriceDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            ProductPriceDataService.AttachModel(_model);
            _model = ProductPriceDataService.GetSingle(_model.Id);OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");
            Mode = ModificationStatus.Saved;
        }

        public override void Delete(object param)
        {
            _model.Status = (byte) Status.Deleted; ProductPriceDataService.AttachModel(_model);
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }

        #endregion

        #region Static Methods
        public static ProductPrice CreateNew(ProductPriceDataService dataService, int productId)
        {
            int id = dataService.AddModel(new ProductPrice { CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, StartDateTime = DateTime.Now, Value = 0, Status = 1}, productId);
            return dataService.GetSingle(id);
        }

        #endregion
    }
}