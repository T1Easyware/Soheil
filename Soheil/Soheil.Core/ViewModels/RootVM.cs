using System;
using System.Windows;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class RootVM : ItemContentViewModel
    {
        #region Properties

        private Root _model;
        public Root Model
        {
            get
            {
                return _model;
            }
        }

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
        public RootDataService RootDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ProductDefectionDataService ProductDefectionDataService { get; set; }

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

        public static readonly DependencyProperty FishbonesVMProperty =
            DependencyProperty.Register("FishbonesVM", typeof(RootFishbonesVM), typeof(RootVM), null);

        public RootFishbonesVM FishbonesVM
        {
            get { return (RootFishbonesVM)GetValue(FishbonesVMProperty); }
            set { SetValue(FishbonesVMProperty, value); }
        }

        public FishboneNodeActionPlansVM ActionPlansVM { get; set; }

        public ProductDefectionVM CurrentProductDefection { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public RootVM(AccessType access, RootDataService dataService):base(access)
        {
            InitializeData(dataService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public RootVM(Root entity, AccessType access, RootDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
            if(entity.ProductDefection != null)
                CurrentProductDefection = new ProductDefectionVM(entity.ProductDefection, Access, ProductDefectionDataService, RelationDirection.Straight);
            FishbonesVM = new RootFishbonesVM(this, CurrentProductDefection, Access);
        }

        private void InitializeData(RootDataService dataService)
        {
            RootDataService = dataService;
            ProductDefectionDataService = new ProductDefectionDataService();
            SaveCommand = new Command(Save, CanSave);
        }


        public override void Save(object param)
        {
            RootDataService.AttachModel(_model);
            _model = RootDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }
        public override void Delete(object param)
        {
            _model.Status = (byte)Status.Deleted; RootDataService.AttachModel(_model);
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
                    FishbonesVM = new RootFishbonesVM(this, CurrentProductDefection, Access);
                    CurrentLink = FishbonesVM;
                    break;
                case 1:
                    if(FishbonesVM == null) break;
                    ActionPlansVM = new FishboneNodeActionPlansVM((FishboneNodeVM) FishbonesVM.CurrentNode,Access);
                    CurrentLink = ActionPlansVM;
                    break;
            }
            base.ViewItemLink(param);
        }
        #endregion

        #region Static Methods
        public static Root CreateNew(RootDataService dataService)
        {
            int id = dataService.AddModel(new Root { Name = "جدید", Code = string.Empty, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now });
            return dataService.GetSingle(id);
        }
        #endregion
    }
}