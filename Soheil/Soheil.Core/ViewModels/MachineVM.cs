using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class MachineVM : ItemContentViewModel
    {
        #region Properties

        private Machine _model;

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

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public MachineDataService MachineDataService { get; set; }

        /// <summary>
        /// Gets or sets the machine group data service.
        /// </summary>
        /// <value>
        /// The machine group data service.
        /// </value>
        public MachineFamilyDataService GroupDataService { get; set; }

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
            set { _model.Status = (byte)value; }
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


        public MachineStationsVM StationsVM { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        /// <param name="groupDataService"></param>
        public MachineVM(Machine entity, AccessType access, MachineDataService dataService, MachineFamilyDataService groupDataService)
            : base(access)
        {
            InitializeData(dataService, groupDataService);
            _model = entity;
            Groups = new ListCollectionView(new ObservableCollection<ProductGroupVM>());

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="groupItems">The group view models.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        /// <param name="groupDataService"></param>
        public MachineVM(Machine entity, ListCollectionView groupItems, AccessType access, MachineDataService dataService, MachineFamilyDataService groupDataService)
            : base(access)
        {
            InitializeData(dataService, groupDataService);
            _model = entity;
            Groups = groupItems;
            foreach (MachineFamilyVM groupVm in groupItems)
            {
                if (groupVm.Id == entity.MachineFamily.Id)
                {
                    SelectedGroupVM = groupVm;
                    break;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineVM"/> class initialized by default values.
        /// </summary>
        public MachineVM(int groupId, ObservableCollection<MachineFamilyVM> machineGroupVms, AccessType access, MachineDataService dataService, MachineFamilyDataService groupDataService)
            : base(access)
        {
            InitializeData(dataService, groupDataService);
            Groups = new ListCollectionView(machineGroupVms);
            foreach (MachineFamilyVM groupVm in machineGroupVms)
            {
                if (groupVm.Id == groupId)
                {
                    SelectedGroupVM = groupVm;
                    break;
                }
            }
        }

        private void InitializeData(MachineDataService dataService, MachineFamilyDataService groupDataService)
        {
            MachineDataService = dataService;
            GroupDataService = groupDataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            MachineDataService.AttachModel(_model,SelectedGroupVM.Id);
            _model = MachineDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }
        public override void Delete(object param)
        {
            _model.Status = (byte)Status.Deleted; MachineDataService.AttachModel(_model);
        }
        public override void ViewItemLink(object param)
        {
            var relationIndex = Convert.ToInt32(param);
            switch (relationIndex)
            {
                case 0:
                    StationsVM = new MachineStationsVM(this, Access);
                    CurrentLink = StationsVM;
                    break;
                case 1:
                    break;
            }
            base.ViewItemLink(param);
        }
        #endregion

        #region Static Methods
        public static Machine CreateNew(MachineDataService dataService, int groupId)
        {
            int id = dataService.AddModel(new Machine { Code = string.Empty, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, ModifiedBy = 0, Name = " جدید" , Status = 1}, groupId);
            return dataService.GetSingle(id);
        }
        #endregion
    }
}