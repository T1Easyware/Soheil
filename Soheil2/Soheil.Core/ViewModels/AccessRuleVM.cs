using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class AccessRuleVM : NodeContentViewModel
    {
        #region Properties

        private AccessRule _model;

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
            get { return Common.Properties.Resources.ResourceManager.GetString(_model.Name); }
            set { _model.Name = value; OnPropertyChanged("Name"); }
        }


        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public AccessRuleDataService AccessRuleDataService { get; set; }

        public string Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
        }

        public AccessRulePositionsVM PositionsVM { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public AccessRuleVM(AccessType access, AccessRuleDataService dataService):base(access)
        {
            InitializeData(dataService);
            _model = new AccessRule();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="dataService"></param>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        public AccessRuleVM(AccessRuleDataService dataService, AccessRule entity, AccessType access)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
            ParentId = entity.Parent == null? 0: entity.Parent.Id;
            Title = Name;
            foreach (var child in entity.Children)
            {
                ChildNodes.Add(new AccessRuleVM(dataService, child, Access));
            }
        }

        private void InitializeData(AccessRuleDataService dataService)
        {
            AccessRuleDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
            ChildNodes = new ObservableCollection<IEntityNode>();
        }

        public override void Save(object param)
        {
            AccessRuleDataService.AttachModel(_model);
            _model = AccessRuleDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }

        #endregion

        #region Static Method
        public static AccessRule CreateNew(AccessRuleDataService dataService)
        {
            int id = dataService.AddModel(new AccessRule { Name = "جدید", Code = string.Empty });
            return dataService.GetSingle(id);
        }
        #endregion
    }
}