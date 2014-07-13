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
	public class MachinePartDataService : DataServiceBase, IDataService<MachinePart>
    {
		readonly Repository<MachinePart> _machinePartRepository;
		readonly Repository<Part> _partRepository;
		readonly Repository<Machine> _machineRepository;
	    readonly Repository<MachineFamily> _machineFamilyRepository;

		public MachinePartDataService()
			: this(new SoheilEdmContext())
		{

		}
		public MachinePartDataService(SoheilEdmContext context)
		{
			Context = context ?? new SoheilEdmContext();
			_machinePartRepository = new Repository<MachinePart>(Context);
			_partRepository = new Repository<Part>(Context);
			_machineRepository = new Repository<Machine>(Context);
            _machineFamilyRepository = new Repository<MachineFamily>(Context);
		}
	
        #region IDataService<Machine> Members

		public MachinePart GetSingle(int id)
        {
			return _machinePartRepository.FirstOrDefault(x => x.Id == id);
        }

		public ObservableCollection<MachinePart> GetAll()
        {
			IEnumerable<MachinePart> entityList = _machinePartRepository.Find(machine => machine.Status != (decimal)Status.Deleted, "MachineFamily");
			return new ObservableCollection<MachinePart>(entityList);
        }

		public int AddModel(MachinePart model)
		{
			int id;
			var machine = _machineRepository.Single(x => x.Id == model.Machine.Id);
			machine.MachineParts.Add(model);
			Context.Commit();
			id = model.Id;
			return id;
		}

		public int AddModel(Machine machine, Part part)
		{
			int id;
			machine = _machineRepository.Single(x => x.Id == machine.Id);
			if(part != null)
				part = _partRepository.Single(x => x.Id == part.Id);
			var mp = new MachinePart
			{
				Machine = machine,
				Part = part,
				IsMachine = part == null,
				Name = part == null ? machine.Name : part.Name,
				Code = part == null ? machine.Code : part.Code,
				Description = part == null ? "" : part.Description,
			};
			_machinePartRepository.Add(mp);
			Context.Commit();
			id = mp.Id;
			return id;
        }

		public void UpdateModel(MachinePart model)
        {
			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;

			Context.Commit();
        }

        public void DeleteModel(MachinePart model)
        {
        }

		public void AttachModel(MachinePart model)
		{
			if (_machinePartRepository.Exists(mp => mp.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
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
			/*var currentMachine = _machineRepository.Single(machine => machine.Id == machineId);
			if (currentMachine.StationMachines.Any(machineStation => 
				machineStation.Machine.Id == machineId 
				&& machineStation.Station.Id == stationId))
				return;

			var newStation = new Repository<Station>(Context).Single(station => station.Id == stationId);
			var newMachineStation = new StationMachine { Station = newStation, Machine = currentMachine };
			currentMachine.StationMachines.Add(newMachineStation);
			Context.Commit();
			StationAdded(this, new ModelAddedEventArgs<StationMachine>(newMachineStation));*/
        }

        public void RemoveStation(int machineId, int stationId)
        {
               /* var currentMachine = _machineRepository.Single(machine => machine.Id == machineId);
                var currentMachineStation = currentMachine.StationMachines.First(machineStation =>
					machineStation.Machine.Id == machineId 
					&& machineStation.Id == stationId);

                int id = currentMachineStation.Id;
				new Repository<StationMachine>(Context).Delete(currentMachineStation);
                Context.Commit();
                StationRemoved(this, new ModelRemovedEventArgs(id));*/
        }

        /// <summary>
        /// Gets all active machines as view models.
        /// </summary>
        /// <returns></returns>
		public ObservableCollection<MachinePart> GetActives()
		{
			var entityList = _machinePartRepository.Find(mp => mp.Status == (decimal)Status.Active, "Machine", "Part");
			return new ObservableCollection<MachinePart>(entityList);
		}

        /// <summary>
        /// Gets all active machines as view models.
        /// </summary>
        /// <returns></returns>
		public ObservableCollection<MachinePart> GetActives(SoheilEntityType linkType, int linkId)
        {
            if (linkType == SoheilEntityType.Stations)
            {
				var entityList = _machinePartRepository.Find(mp => mp.Status == (decimal)Status.Active);
				return new ObservableCollection<MachinePart>(entityList);
            }
            return GetActives();
        }
    }
}