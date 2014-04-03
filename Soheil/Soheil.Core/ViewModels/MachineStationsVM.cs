using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class MachineStationsVM : ItemLinkViewModel
    {

		public MachineStationsVM(MachineVM machine, AccessType access)
			: base(access)
		{
			CurrentMachine = machine;
			MachineDataService = new MachineDataService();
			MachineDataService.StationAdded += OnStationAdded;
			MachineDataService.StationRemoved += OnStationRemoved;
			StationDataService = new StationDataService();

			var selectedVms = new ObservableCollection<StationMachineVM>();
			foreach (var stationMachine in MachineDataService.GetStations(machine.Id))
			{
				selectedVms.Add(new StationMachineVM(stationMachine, Access, StationMachineDataService, RelationDirection.Reverse));
			}
			SelectedItems = new ListCollectionView(selectedVms);

			var allVms = new ObservableCollection<StationVM>();
			foreach (var station in StationDataService.GetActives()
				.Where(station => !selectedVms.Any(stationMachine => stationMachine.StationId == station.Id)))
			{
				allVms.Add(new StationVM(station, Access, StationDataService));
			}
			AllItems = new ListCollectionView(allVms);

			IncludeCommand = new Command(Include, CanInclude);
			ExcludeCommand = new Command(Exclude, CanExclude);
		}

        public MachineVM CurrentMachine { get; set; }

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
        public StationMachineDataService StationMachineDataService { get; set; }

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
        public StationDataService StationDataService { get; set; }

        private void OnStationRemoved(object sender, ModelRemovedEventArgs e)
        {
            foreach (StationMachineVM item in SelectedItems)
            {
                if (item.Id == e.Id)
                {
                    var model = StationDataService.GetSingle(item.StationId);
                    var returnedVm = new StationVM(model, Access, StationDataService);
                    AllItems.AddNewItem(returnedVm);
                    AllItems.CommitNew();
                    SelectedItems.Remove(item);
                    break;
                }
            }
        }

        private void OnStationAdded(object sender, ModelAddedEventArgs<StationMachine> e)
        {
            var stationMachineVM = new StationMachineVM(e.NewModel, Access, StationMachineDataService, RelationDirection.Reverse);
            SelectedItems.AddNewItem(stationMachineVM);
            SelectedItems.CommitNew();
            foreach (IEntityItem item in AllItems)
            {
                if (item.Id == stationMachineVM.StationId)
                {
                    AllItems.Remove(item);
                    break;
                }
            }
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(StationDataService.GetActives());
        }

        public override void Include(object param)
        {
            MachineDataService.AddStation(CurrentMachine.Id, ((IEntityItem) param).Id);
        }

        public override void Exclude(object param)
        {
            MachineDataService.RemoveStation(CurrentMachine.Id, ((IEntityItem) param).Id);
        }
    }
}