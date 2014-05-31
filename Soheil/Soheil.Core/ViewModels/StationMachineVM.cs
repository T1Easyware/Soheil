using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class StationMachineVM : ItemRelationDetailViewModel
    {
        private readonly StationMachine _model;
        /// <summary>
        /// Initializes a new instance of the class initialized with default values.
        /// </summary>
        public StationMachineVM(AccessType access, StationMachineDataService dataService, RelationDirection presentationType):base(access, presentationType)
        {
            InitializeData(dataService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StationVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="presentationType"></param>
        public StationMachineVM(StationMachine entity, AccessType access, StationMachineDataService dataService, RelationDirection presentationType)
            : base(access,presentationType)
        {
            InitializeData(dataService);
            _model = entity;
            StationId = entity.Station.Id;
            MachineId = entity.Machine.Id;
			//Status = entity.RecordStatus;
            StationName = entity.Station.Name;
            StationCode = entity.Station.Code;
            MachineName = entity.Machine.Name;
            MachineCode = entity.Machine.Code;
        }

        private void InitializeData(StationMachineDataService dataService)
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
        public StationMachineDataService DataService { get; set; }

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
		/// Gets or sets the record status and Updates the database entity if possible.
		/// </summary>
		/// <value>
		/// The status of this object in database.
		/// </value>
		public Status Status
		{
			get { return _model.RecordStatus; }
			set { _model.RecordStatus = value; OnPropertyChanged("Status"); if (CanSave()) Save(null); }
		}

        public int StationId { get; set; }
        public int MachineId { get; set; }
        public string StationName { get; set; }
        public string StationCode { get; set; }
        public string MachineName { get; set; }
        public string MachineCode { get; set; }

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