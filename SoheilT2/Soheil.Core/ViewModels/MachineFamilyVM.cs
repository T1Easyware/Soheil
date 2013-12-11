using System;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class MachineFamilyVM : ItemContentViewModel
    {
        #region Properties
        private MachineFamily _model;

        private ObservableCollection<Machine> _machines;

        public override int Id
        {
            get { return _model.Id; } set{}
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


// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtCodeRequired")]
// ReSharper restore PropertyNotResolved
        public string Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
        }

        public Status Status
        {
            get { return (Status) _model.Status; }
            set { _model.Status = (byte) value; }
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

        public ObservableCollection<Machine> Machines
        {
            get { return _machines; }
            set
            {
                _machines = value;
                OnPropertyChanged("Machines");
            }
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public MachineFamilyDataService MachineFamilyDataService { get; set; }
        #endregion

        #region Method

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineFamilyVM"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public MachineFamilyVM(MachineFamily entity, AccessType access, MachineFamilyDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;

            Machines = new ObservableCollection<Machine>();
            foreach (Machine product in entity.Machines)
            {
                Machines.Add(product);
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="MachineFamilyVM"/> class.
        /// </summary>
        public MachineFamilyVM(AccessType access, MachineFamilyDataService dataService):base(access)
        {
            InitializeData(dataService);
            Machines = new ObservableCollection<Machine>();
        }

        private void InitializeData(MachineFamilyDataService dataService)
        {
            MachineFamilyDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            MachineFamilyDataService.AttachModel(_model);
            _model = MachineFamilyDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }

        #endregion

        #region Static Methods
        public static MachineFamily CreateNew(MachineFamilyDataService dataService)
        {
            int id = dataService.AddModel(new MachineFamily { Name = "جدید", Code = string.Empty, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now });
            return dataService.GetSingle(id);
        }
        #endregion
    }
}