using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;

using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class OrganizationChartPositionVM : NodeRelationDetailViewModel
    {
        private readonly OrganizationChart_Position _model;

        public override int Id
        {
            get { return _model.Id; } 
            set { _model.Id = value;}
        }
                /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public OrganizationChartPositionVM(AccessType access, OrganizationChartPositionDataService dataService):base(access)
        {
            InitializeData(dataService);
            _model = new OrganizationChart_Position();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public OrganizationChartPositionVM(OrganizationChart_Position entity, AccessType access, OrganizationChartPositionDataService dataService):base(access)
        {
            _model = entity;
            InitializeData(dataService);
            OrganizationChartId = entity.OrganizationChart.Id;
            PositionId = entity.Position.Id;
            ParentId = entity.Parent == null? -1: entity.Parent.Id;
            Title = entity.Position.Name;
            foreach (var child in entity.Children)
            {
                ChildNodes.Add(new OrganizationChartPositionVM(child, Access,dataService));
            }
            
        }

        private void InitializeData(OrganizationChartPositionDataService dataService)
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
        public OrganizationChartPositionDataService DataService { get; set; }

        public int OrganizationChartId { get; set; }
        public int PositionId { get; set; }

    }
}