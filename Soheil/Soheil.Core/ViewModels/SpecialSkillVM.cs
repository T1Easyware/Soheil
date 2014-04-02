using System.Windows;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class SpecialSkillVM : ItemContentViewModel
    {
        #region Properties

		private ProductActivitySkill _model;

        public override int Id
        {
            get { return _model.Id; } 
            set { _model.Id = value; OnPropertyChanged("Id"); }
        }

        public override string SearchItem {get {return "Reserve1 + Reserve2 + Reserve3???";} set{} }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ProductActivitySkillDataService SpecialSkillDataService { get; set; }

        /// <summary>
        /// Gets or sets the operator ViewModel.
        /// </summary>
        /// <value>
        /// The operator ViewModel.
        /// </value>
        public OperatorVM OperatorVM { get; set; }

        /* public string Reserve1
         {
             get { return _model.Reserve1; }
             set { _model.Reserve1 = value; OnPropertyChanged("Reserve1"); }
         }

         public string Reserve2
         {
             get { return _model.Reserve2; }
             set { _model.Reserve2 = value; OnPropertyChanged("Reserve2"); }
         }

         public string Reserve3
         {
             get { return _model.Reserve3; }
             set { _model.Reserve3 = value; OnPropertyChanged("Reserve3"); }
         }???*/

        public string ModifiedBy
        {
            get { return LoginInfo.GetUsername(_model.ModifiedBy); }
            
        }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public SpecialSkillVM(AccessType access, ProductActivitySkillDataService dataService):base(access)
        {
			_model = new ProductActivitySkill();
            InitializeData(dataService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        public SpecialSkillVM(ProductActivitySkill entity, AccessType access, ProductActivitySkillDataService dataService)
            : base(access)
        {
            _model = entity;
            InitializeData(dataService);
            Id = entity.Id;
            /*Reserve1 = entity.Reserve1;
            Reserve2 = entity.Reserve2;
            Reserve3 = entity.Reserve3;???*/
            //OperatorVM = new OperatorVM(entity.Operator);
        }

        private void InitializeData(ProductActivitySkillDataService dataService)
        {
            SpecialSkillDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            SpecialSkillDataService.AttachModel(_model);
            _model = SpecialSkillDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }

        #endregion

        #region Static Methods
		public static ProductActivitySkill CreateNew(ProductActivitySkillDataService dataService)
        {
			int id = dataService.AddModel(new ProductActivitySkill { /*???*/});
            return dataService.GetSingle(id);
        }
        #endregion
    }
}