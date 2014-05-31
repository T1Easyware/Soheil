using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class ProductVM : ItemContentViewModel
    {
        #region Properties
        private Product _model;

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
        public ProductDataService ProductDataService { get; set; }

        /// <summary>
        /// Gets or sets the product group data service.
        /// </summary>
        /// <value>
        /// The product group data service.
        /// </value>
        public ProductGroupDataService GroupDataService { get; set; }

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
            set { _model.Status = (byte) value; }
        }

        public DateTime CreatedDate
        {
            get { return _model.CreatedDate; }
            set { _model.CreatedDate = value;  OnPropertyChanged("CreatedDate"); }
        }

        public Color Color
        {
            get { return _model.Color; }
            set { _model.Color = value; OnPropertyChanged("Color"); }
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


        public ProductDefectionsVM DefectionsVM { get; set; }
        public ProductReworksVM ReworksVM { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="groupItems">The group view models.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        /// <param name="groupDataService"></param>
        public ProductVM(Product entity, ListCollectionView groupItems, AccessType access, ProductDataService dataService, ProductGroupDataService groupDataService)
            : base(access)
        {
            InitializeData(dataService, groupDataService);
            _model = entity;
            Groups = groupItems;
            foreach (ProductGroupVM groupVm in groupItems)
            {
                if (groupVm.Id == entity.ProductGroup.Id)
                {
                    SelectedGroupVM = groupVm;
                    break;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        /// <param name="groupDataService"></param>
        public ProductVM(Product entity, AccessType access, ProductDataService dataService, ProductGroupDataService groupDataService)
            : base(access)
        {
            _model = entity;
            InitializeData(dataService, groupDataService);
            Groups = new ListCollectionView(new ObservableCollection<ProductGroupVM>());
        }

        private void InitializeData(ProductDataService dataService, ProductGroupDataService groupDataService)
        {
            ProductDataService = dataService;
            GroupDataService = groupDataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            ProductDataService.AttachModel(_model,SelectedGroupVM.Id);
            _model = ProductDataService.GetSingle(_model.Id);OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");
            Mode = ModificationStatus.Saved;
        }

        public override void Delete(object param)
        {
            _model.Status = (byte) Status.Deleted; ProductDataService.AttachModel(_model, SelectedGroupVM.Id);
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
                    DefectionsVM = new ProductDefectionsVM(this, Access);
                    CurrentLink = DefectionsVM;
                    break;
                case 1:
                    ReworksVM = new ProductReworksVM(this, Access);
                    CurrentLink = ReworksVM;
                    break;
            }
            base.ViewItemLink(param);
        }
        #endregion

        #region Static Methods
        public static Product CreateNew(ProductDataService dataService, int groupId)
        {
            var color = PickRandomColor();
            int id = dataService.AddModel(new Product { Name = "جدید", Code = string.Empty, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now , Color = color}, groupId);
            return dataService.GetSingle(id);
        }
        private static Color PickRandomColor()
        {
            var rnd = new Random();
            Type brushesType = typeof(Colors);
            PropertyInfo[] properties = brushesType.GetProperties();
            int random = rnd.Next(properties.Length);
            var result = (Color)properties[random].GetValue(null, null);

            return result;
        }

        #endregion
    }
}