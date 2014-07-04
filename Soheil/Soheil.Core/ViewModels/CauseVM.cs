using System;
using System.Collections.ObjectModel;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class CauseVM : NodeContentViewModel
    {
        #region Properties

        private Cause _model;

        public override int Id
        {
            get { return _model.Id; }
            set { }
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
        public CauseDataService CauseDataService { get; set; }

        // ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtCodeRequired")]
        // ReSharper restore PropertyNotResolved
        public byte Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
        }

        public CauseLevel Level
        {
            get { return (CauseLevel)_model.Level; }
            set { _model.Level = (byte)value; }
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
        public CauseVM(AccessType access, CauseDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = new Cause();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public CauseVM(Cause entity, AccessType access, CauseDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
            ParentId = entity.Parent == null ? 0 : entity.Parent.Id;
            Title = Name;
            foreach (var child in entity.Children)
            {
                if (child.Status != (decimal) Status.Deleted)
                    ChildNodes.Add(new CauseVM(child, Access,dataService));
            }
        }

        private void InitializeData(CauseDataService dataService)
        {
            CauseDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
            ChildNodes = new ObservableCollection<IEntityNode>();
        }

		public override void Save(object param)
		{
			var cobranches = CauseDataService.GetActives().Where(x => x != _model && x.Parent == _model.Parent);
			var duplicate = cobranches.FirstOrDefault(x => x.Code == Code);
			if (duplicate != null)
			{
				Code = (byte)(cobranches.Max(x => x.Code) + 1);
				System.Windows.MessageBox.Show("تمام زیرشاخه های هر علت توقف، بایستی کد منحصر به فرد داشته باشند", duplicate.Name, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
			}
			if (_model.Code > 99)
			{
				Code = 99;
				System.Windows.MessageBox.Show("کد بایستی بین 0 و 99 باشد", Name, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
			}
			CauseDataService.AttachModel(_model);
			_model = CauseDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy"); OnPropertyChanged("ModifiedDate"); Mode = ModificationStatus.Saved;
		}

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }

        public override void Delete(object param)
        {
            _model.Status = (byte) Status.Deleted; CauseDataService.AttachModel(_model);
        }

        #endregion

        #region Static Method
        public static Cause CreateNew(CauseDataService dataService, int parentId, CauseLevel level)
        {
            int id = dataService.AddModel(new Cause { Name = "جدید", Code = 0, Description = string.Empty,
                CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, 
                Level = (byte) level, Status = (byte) Status.Active}, parentId);
            return dataService.GetSingle(id);
        }
        #endregion
    }
}