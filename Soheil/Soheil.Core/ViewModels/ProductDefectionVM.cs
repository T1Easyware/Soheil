using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class ProductDefectionVM : ItemRelationDetailViewModel
    {
        private readonly ProductDefection _model;
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public ProductDefectionVM(AccessType access, ProductDefectionDataService dataService, RelationDirection presentationType)
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
        public ProductDefectionVM(ProductDefection entity, AccessType access, ProductDefectionDataService dataService, RelationDirection presentationType):base(access, presentationType)
        {
            InitializeData(dataService);
            _model = entity;
            ProductId = entity.Product.Id;
            DefectionId = entity.Defection.Id;
            ProductName = entity.Product.Name;
            ProductCode = entity.Product.Code;
            DefectionName = entity.Defection.Name;
            DefectionCode = entity.Defection.Code;
        }

        private void InitializeData(ProductDefectionDataService dataService)
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
        public ProductDefectionDataService DataService { get; set; }

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
        public int DefectionId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string DefectionName { get; set; }
        public string DefectionCode { get; set; }

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