using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class StationMachinesVM : ItemLinkViewModel
    {
        public StationMachinesVM(StationVM station, AccessType access):base(access)
        {
            UnitOfWork = new SoheilEdmContext();
            CurrentStation = station;
            StationDataService = new StationDataService(UnitOfWork);
            StationDataService.MachineAdded += OnMachineAdded;
            StationDataService.MachineRemoved += OnMachineRemoved;
            MachineDataService = new MachineDataService(UnitOfWork);
            StationMachineDataService = new StationMachineDataService(UnitOfWork);
            MachineFamilyDataService = new MachineFamilyDataService(UnitOfWork);

            var selectedVms = new ObservableCollection<StationMachineVM>();
            foreach (var stationMachine in StationDataService.GetMachines(station.Id))
            {
                selectedVms.Add(new StationMachineVM(stationMachine, Access, StationMachineDataService, RelationDirection.Straight));
            }
            SelectedItems = new ListCollectionView(selectedVms);

            var allVms = new ObservableCollection<MachineVM>();
            foreach (var machine in MachineDataService.GetActives()
				.Where(machine => !selectedVms.Any(stationMachine => stationMachine.MachineId == machine.Id)))
            {
                allVms.Add(new MachineVM(machine, Access, MachineDataService, MachineFamilyDataService));
            }
            AllItems = new ListCollectionView(allVms);

            IncludeCommand = new Command(Include, CanInclude);
            ExcludeCommand = new Command(Exclude, CanExclude);
			IncludeAllCommand = new Command(o =>
			{
				foreach (var vm in allVms.ToArray())
				{
					StationDataService.AddMachine(CurrentStation.Id, ((IEntityItem)vm).Id);
				}
			}, () => allVms.Any());
        }

        public StationVM CurrentStation { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public StationDataService StationDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public MachineDataService MachineDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public MachineFamilyDataService MachineFamilyDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public StationMachineDataService StationMachineDataService { get; set; }

        private void OnMachineRemoved(object sender, ModelRemovedEventArgs e)
        {
            foreach (StationMachineVM  item in SelectedItems)
            {
                if (item.Id == e.Id)
                {
                    var model = MachineDataService.GetSingle(item.MachineId);
                    var returnedVm = new MachineVM(model, Access, MachineDataService, MachineFamilyDataService);
                    AllItems.AddNewItem(returnedVm);
                    AllItems.CommitNew();
                    SelectedItems.Remove(item);
                    break;
                }
            }
        }

        private void OnMachineAdded(object sender, ModelAddedEventArgs<StationMachine> e)
        {
            var stationMachineVM = new StationMachineVM(e.NewModel, Access, StationMachineDataService, RelationDirection.Straight);
            SelectedItems.AddNewItem(stationMachineVM);
            SelectedItems.CommitNew();
            foreach (IEntityItem item in AllItems)
            {
                if (item.Id == stationMachineVM.MachineId)
                {
                    AllItems.Remove(item);
                    break;
                }
            }
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(MachineDataService.GetActives());
        }

        public override void Include(object param)
        {
            StationDataService.AddMachine(CurrentStation.Id, ((IEntityItem)param).Id);
        }

        public override void Exclude(object param)
        {
            StationDataService.RemoveMachine(CurrentStation.Id, ((IEntityItem) param).Id);
        }

        public override void IncludeRange(object param)
        {
            var tempList = new List<ISplitContent>();
            tempList.AddRange(AllItems.Cast<ISplitContent>());
            foreach (ISplitContent item in tempList)
            {
                if (item.IsChecked)
                {
                    StationDataService.AddMachine(CurrentStation.Id, ((IEntityItem)item).Id);
                }
            }
        }

        public override void ExcludeRange(object param)
        {
            var tempList = new List<ISplitDetail>();
            tempList.AddRange(SelectedItems.Cast<ISplitDetail>());
            foreach (ISplitDetail item in tempList)
            {
                if (item.IsChecked)
                {
                    StationDataService.RemoveMachine(CurrentStation.Id, ((IEntityItem)item).Id);
                }
            }
        }

        public Command IncludeAllCommand { get; set; }
    }
}