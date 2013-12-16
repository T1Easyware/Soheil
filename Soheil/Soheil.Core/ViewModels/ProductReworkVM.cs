using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class ProductReworkVM : ItemRelationDetailViewModel
    {
        private readonly ProductRework _model;

        public string Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
        }

        public string Name
        {
            get { return _model.Name; }
            set { _model.Name = value; OnPropertyChanged("Name"); }
        }

        public string ModifiedBy
        {
            get { return LoginInfo.GetUsername(_model.ModifiedBy); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public ProductReworkVM(AccessType access, ProductReworkDataService dataService, RelationDirection presentationType)
            : base(access, presentationType)
        {
            InitializeData(dataService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        /// <param name="presentationType"></param>
        public ProductReworkVM(ProductRework entity, AccessType access, ProductReworkDataService dataService, RelationDirection presentationType):base(access, presentationType)
        {
            InitializeData(dataService);
            _model = entity;
            ProductId = entity.Product.Id;
            ReworkId = entity.Rework.Id;
            ProductName = entity.Product.Name;
            ProductCode = entity.Product.Code;
            ReworkName = entity.Rework.Name;
            ReworkCode = entity.Rework.Code;
        }

        private void InitializeData(ProductReworkDataService dataService)
        {
            DataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }

        /// <summary>
        /// Gets or sets the activity-operator data service.
        /// </summary>
        /// <value>
        /// The activity-operator data service.
        /// </value>
        public ProductReworkDataService DataService { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public override int Id
        {
            get { return _model.Id; } set{}
        }


        public int ProductId { get; set; }
        public int ReworkId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ReworkName { get; set; }
        public string ReworkCode { get; set; }

        public override void Save(object param)
        {
            DataService.UpdateModel(_model);
            OnPropertyChanged("SkillPoint");
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }
    }
}