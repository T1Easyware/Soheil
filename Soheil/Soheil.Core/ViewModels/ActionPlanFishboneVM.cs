using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class ActionPlanFishboneVM : ItemRelationDetailViewModel
    {
        private readonly FishboneNode_ActionPlan _model;
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public ActionPlanFishboneVM(AccessType access, RelationDirection presentationType):base(access, presentationType)
        {
            InitializeData();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="presentationType"></param>
        public ActionPlanFishboneVM(FishboneNode_ActionPlan entity, AccessType access,RelationDirection presentationType)
            : base(access, presentationType)
        {
            InitializeData();
            _model = entity;
            ActionPlanId = entity.ActionPlan.Id;
            RootId = entity.FishboneNode.Id;
            FishboneNodeId = entity.FishboneNode.Id;
            ActionPlanCode = entity.ActionPlan.Code;
            ActionPlanName = entity.ActionPlan.Name;
            FishboneNodeName = entity.FishboneNode.Description;
        }

        private void InitializeData()
        {
            DataService = new FishboneActionPlanDataService();
            SaveCommand = new Command(Save, CanSave);
        }


        /// <summary>
        /// Gets or sets the activity-operator data service.
        /// </summary>
        /// <value>
        /// The activity-operator data service.
        /// </value>
        public FishboneActionPlanDataService DataService { get; set; }

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


        public int ActionPlanId { get; set; }
        public int RootId { get; set; }
        public int FishboneNodeId { get; set; }
        public string ActionPlanCode { get; set; }
        public string ActionPlanName { get; set; }
        public string FishboneNodeName { get; set; }

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