using System;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
	public class FpcVm: ItemContentViewModel
    {
        #region Properties

        private FPC _model;
		/// <summary>
		/// Gets the model represented by this vm
		/// </summary>
		public Model.FPC Model { get { return _model; } }

		public int ID;
        public override int Id
        {
			get { return _model.Id; }
			set { ID = value; }
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
        public FPCDataService FPCDataService { get; set; }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtCodeRequired")]
// ReSharper restore PropertyNotResolved
        public string Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
        }

// ReSharper disable PropertyNotResolved
		[LocalizedRequired(ErrorMessageResourceName = @"txtIsDefaultRequired")]
// ReSharper restore PropertyNotResolved
        public bool IsDefault
        {
            get { return _model.IsDefault; }
            set { _model.IsDefault = value; OnPropertyChanged("IsDefault"); }
        }

		public Status Status
		{
			get { return (Status)_model.Status; }
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

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public FpcVm(AccessType access, FPCDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
        }

	    /// <summary>
	    /// Initializes a new instance of the <see cref="FpcVm"/> class from the model.
	    /// </summary>
	    /// <param name="entity">The model.</param>
	    /// <param name="access"></param>
	    /// <param name="dataService"></param>
        public FpcVm(FPC entity, AccessType access, FPCDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
        }

	    /// <summary>
	    /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
	    /// </summary>
	    /// <param name="entity">The model.</param>
	    /// <param name="groupItems">The group view models.</param>
	    /// <param name="access"></param>
	    /// <param name="dataService"></param>
        public FpcVm(FPC entity, ListCollectionView groupItems, AccessType access, FPCDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
            Groups = groupItems;
            foreach (ProductVM groupVm in groupItems)
            {
                if (groupVm.Id == entity.Product.Id)
                {
                    SelectedGroupVM = groupVm;
                    break;
                }
            }
        }

        private void InitializeData(FPCDataService dataService)
        {
            FPCDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
			FPCDataService.AttachModel(_model);
			OnPropertyChanged("ModifiedBy"); OnPropertyChanged("ModifiedDate"); Mode = ModificationStatus.Saved;
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }

	    public override void Delete(object param)
	    {
	        _model.Status = (byte) Status.Deleted; FPCDataService.AttachModel(_model);
	    }

	    #endregion

        #region Static Methods
        public static FPC CreateNew(FPCDataService dataService, int groupId)
        {
            int id = dataService.AddModel(new FPC { Name = "*", Code = "*", CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now }, groupId);
            return dataService.GetSingle(id);
        }
        #endregion



	}
}
