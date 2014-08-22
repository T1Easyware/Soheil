using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class RawMaterialUnitGroupVM : ItemRelationDetailViewModel
    {
        private readonly RawMaterialUnitGroup _model;
        /// <summary>
        /// Initializes a new instance of the <see cref="RawMaterialGroupVM"/> class initialized with default values.
        /// </summary>
        public RawMaterialUnitGroupVM(AccessType access, RawMaterialUnitGroupDataService dataService, RelationDirection presentationType)
            : base(access, presentationType)
        {
            InitializeData(dataService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawMaterialVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        /// <param name="presentationType"></param>
        public RawMaterialUnitGroupVM(RawMaterialUnitGroup entity, AccessType access, RawMaterialUnitGroupDataService dataService, RelationDirection presentationType):base(access, presentationType)
        {
            InitializeData(dataService);
            _model = entity;
            RawMaterialId = entity.RawMaterial.Id;
            UnitGroupId = entity.UnitGroup.Id;
            RawMaterialName = entity.RawMaterial.Name;
            RawMaterialCode = entity.RawMaterial.Code;
            UnitGroupName = entity.UnitGroup.Name;
        }

        private void InitializeData(RawMaterialUnitGroupDataService dataService)
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
        public RawMaterialUnitGroupDataService DataService { get; set; }

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


        public int RawMaterialId { get; set; }
        public int UnitGroupId { get; set; }
        public string RawMaterialName { get; set; }
        public string RawMaterialCode { get; set; }
        public string UnitGroupName { get; set; }
        public string UnitGroupCode { get; set; }

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