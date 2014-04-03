using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
	public class MachineDataService : DataServiceBase, IDataService<Machine>
    {
		public event EventHandler<ModelAddedEventArgs<Machine>> MachineAdded;
		public event EventHandler<ModelAddedEventArgs<StationMachine>> StationAdded;
		public event EventHandler<ModelRemovedEventArgs> StationRemoved;
		Repository<Machine> _machineRepository;
		Repository<MachineFamily> _machineFamilyRepository;

		public MachineDataService()
			: this(new SoheilEdmContext())
		{

		}
		public MachineDataService(SoheilEdmContext context)
		{
			this.context = context;
			_machineRepository = new Repository<Machine>(context);
			_machineFamilyRepository = new Repository<MachineFamily>(context);
		}
	
		public Machine GetSingleWithFamily(int id)
		{
			return _machineRepository.FirstOrDefault(machine => machine.Id == id, "MachineFamily");
		}

		public bool IsMachineIsUsedByDefault(int machineId, int stateStationActivityId)
		{
			var ssaRepository = new Repository<StateStationActivity>(context);
			return ssaRepository.First(x => x.Id == stateStationActivityId)
				.StateStationActivityMachines.First(y => y.Machine.Id == machineId).IsFixed;
		}


        #region IDataService<Machine> Members

        public Machine GetSingle(int id)
        {
			return _machineRepository.FirstOrDefault(machine => machine.Id == id);
        }

        public ObservableCollection<Machine> GetAll()
        {
			IEnumerable<Machine> entityList = _machineRepository.Find(machine => machine.Status != (decimal)Status.Deleted, "MachineFamily");
			return new ObservableCollection<Machine>(entityList);
        }

		public int AddModel(Machine model)
		{
			int id;
			var machineGroup = _machineFamilyRepository.Single(group => group.Id == model.MachineFamily.Id);
			machineGroup.Machines.Add(model);
			context.Commit();
			if (MachineAdded != null)
				MachineAdded(this, new ModelAddedEventArgs<Machine>(model));
			id = model.Id;
			return id;
		}

        public int AddModel(Machine model, int groupId)
		{
			int id;
			var machineGroup = _machineFamilyRepository.Single(group => group.Id == groupId);
			model.MachineFamily = machineGroup;
			machineGroup.Machines.Add(model);
			context.Commit();
			if (MachineAdded != null)
				MachineAdded(this, new ModelAddedEventArgs<Machine>(model));
			id = model.Id;
			return id;
        }

        public void UpdateModel(Machine model)
        {
			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;

			context.Commit();
        }
		public void UpdateModel(Machine model, int groupId)
		{
			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;

			model.MachineFamily = _machineFamilyRepository.Single(machineGroup => machineGroup.Id == groupId);
			context.Commit();
		}

        public void DeleteModel(Machine model)
        {
        }

        public void AttachModel(Machine model)
		{
			if (_machineRepository.Exists(machine => machine.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
        }

		public void AttachModel(Machine model, int groupId)
		{
			if (_machineRepository.Exists(machine => machine.Id == model.Id))
			{
				UpdateModel(model, groupId);
			}
			else
			{
				AddModel(model, groupId);
			}
		}

        #endregion

        public ObservableCollection<StationMachine> GetStations(int machineId)
        {
			Machine entity = _machineRepository.FirstOrDefault(machine => machine.Id == machineId, "StationMachines.Station", "StationMachines.Machine");
			return new ObservableCollection<StationMachine>(entity.StationMachines.Where(item => item.Station.Status == (decimal)Status.Active));
        }

        public void AddStation(int machineId, int stationId)
        {
			var currentMachine = _machineRepository.Single(machine => machine.Id == machineId);
			if (currentMachine.StationMachines.Any(machineStation => 
				machineStation.Machine.Id == machineId 
				&& machineStation.Station.Id == stationId))
				return;

			var newStation = new Repository<Station>(context).Single(station => station.Id == stationId);
			var newMachineStation = new StationMachine { Station = newStation, Machine = currentMachine };
			currentMachine.StationMachines.Add(newMachineStation);
			context.Commit();
			StationAdded(this, new ModelAddedEventArgs<StationMachine>(newMachineStation));
        }

        public void RemoveStation(int machineId, int stationId)
        {
                var currentMachine = _machineRepository.Single(machine => machine.Id == machineId);
                var currentMachineStation = currentMachine.StationMachines.First(machineStation =>
					machineStation.Machine.Id == machineId 
					&& machineStation.Id == stationId);

                int id = currentMachineStation.Id;
				new Repository<StationMachine>(context).Delete(currentMachineStation);
                context.Commit();
                StationRemoved(this, new ModelRemovedEventArgs(id));
        }

        /// <summary>
        /// Gets all active machines as view models.
        /// </summary>
        /// <returns></returns>
		public ObservableCollection<Machine> GetActives()
		{
			var entityList = _machineRepository.Find(machine => machine.Status == (decimal)Status.Active, "MachineFamily");
			return new ObservableCollection<Machine>(entityList);
		}

		public IEnumerable<Machine> GetActives(int stateStationActivityId, params string[] includePath)
		{
			var models = new List<Machine>();
			var ssamList = new Repository<StateStationActivityMachine>(context).Find(ssam =>
				ssam.StateStationActivity.Id == stateStationActivityId &&
				ssam.Machine.Status == (decimal)Status.Active, includePath);
			models.AddRange(ssamList.Select(ssam => ssam.Machine));
			return models;
		}
    }
}