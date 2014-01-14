using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class ActivityOperatorVM : ItemRelationDetailViewModel
    {
        private readonly GeneralActivitySkill _model;
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public ActivityOperatorVM(AccessType access, ActivityOperatorDataService dataService, RelationDirection presentationType):base(access, presentationType)
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
        public ActivityOperatorVM(GeneralActivitySkill entity, AccessType access, ActivityOperatorDataService dataService, RelationDirection presentationType)
            : base(access, presentationType)
        {
            InitializeData(dataService);
            _model = entity;
            ActivityId = entity.Activity.Id;
            OperatorId = entity.Operator.Id;
            ActivityName = entity.Activity.Name;
            ActivityCode = entity.Activity.Code;
            OperatorName = entity.Operator.Name;
            OperatorCode = entity.Operator.Code;
            SkillPoint = (ILUO)entity.IluoNr;
        }

        private void InitializeData(ActivityOperatorDataService dataService)
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
        public ActivityOperatorDataService DataService { get; set; }

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

        /// <summary>
        /// Gets or sets the skill point.
        /// </summary>
        /// <value>
        /// The skill point.
        /// </value>
        public ILUO SkillPoint
        {
            get { return (ILUO)_model.IluoNr; }
            set { _model.IluoNr = (byte)value; OnPropertyChanged("SkillPoint"); }
        }

        public int ActivityId { get; set; }
        public int OperatorId { get; set; }
        public string ActivityName { get; set; }
        public string ActivityCode { get; set; }
        public string OperatorName { get; set; }
        public string OperatorCode { get; set; }

        public override void Save(object param)
        {
            DataService.UpdateModel(_model);
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }
    }
}