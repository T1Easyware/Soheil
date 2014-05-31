using System;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class DefectionVM : ItemContentViewModel
    {
        #region Properties

        private Defection _model;

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
        public DefectionDataService DefectionDataService { get; set; }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtCodeRequired")]
// ReSharper restore PropertyNotResolved
        public string Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
        }

		public bool IsG2
		{
			get { return _model.IsG2; }
			set { _model.IsG2 = value; OnPropertyChanged("IsG2"); }
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

        public DefectionProductsVM ProductsVM { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public DefectionVM(AccessType access, DefectionDataService dataService):base(access)
        {
            InitializeData(dataService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public DefectionVM(Defection entity, AccessType access, DefectionDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
        }

        private void InitializeData(DefectionDataService dataService)
        {
            DefectionDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }


        public override void Save(object param)
        {
            DefectionDataService.AttachModel(_model);
            _model = DefectionDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }

        public override void Delete(object param)
        {
            _model.Status = (byte) Status.Deleted; DefectionDataService.AttachModel(_model);
        }

        public override void ViewItemLink(object param)
        {
            var relationIndex = Convert.ToInt32(param);
            switch (relationIndex)
            {
                case 0:
                    ProductsVM = new DefectionProductsVM(this, Access);
                    CurrentLink = ProductsVM;
                    break;
                case 1:
                    break;
            }
            base.ViewItemLink(param);
        }
        #endregion

        #region Static Methods
        public static Defection CreateNew(DefectionDataService dataService)
        {
            int id = dataService.AddModel(new Defection { Name = "جدید", Code = string.Empty, IsG2=false, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now });
            return dataService.GetSingle(id);
        }
        #endregion
    }
}