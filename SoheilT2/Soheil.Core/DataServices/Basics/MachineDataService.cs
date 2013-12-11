using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class MachineDataService : IDataService<Machine>
    {
		public Machine GetSingleWithFamily(int id)
		{
			Machine entity;
			using (var context = new SoheilEdmContext())
			{
				var machineRepository = new Repository<Machine>(context);
				entity = machineRepository.FirstOrDefault(machine => machine.Id == id, "MachineFamily");
			}
			return entity;
		}

		public bool IsMachineIsUsedByDefault(int machineId, int stateStationActivityId)
		{
			using (var context = new SoheilEdmContext())
			{
				var ssaRepository = new Repository<StateStationActivity>(context);
				return ssaRepository.First(x => x.Id == stateStationActivityId)
					.StateStationActivityMachines.First(y => y.Machine.Id == machineId).IsFixed;
			}
		}


        #region IDataService<Machine> Members

        public Machine GetSingle(int id)
        {
            Machine entity;
            using (var context = new SoheilEdmContext())
            {
                var machineRepository = new Repository<Machine>(context);
                entity = machineRepository.FirstOrDefault(machine => machine.Id == id);
            }
            return entity;
        }

        public ObservableCollection<Machine> GetAll()
        {
            ObservableCollection<Machine> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Machine>(context);
                IEnumerable<Machine> entityList = repository.Find(machine=> machine.Status != (decimal)Status.Deleted, "MachineFamily");
                models = new ObservableCollection<Machine>(entityList);
            }
            return models;
        }

        public int AddModel(Machine model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var groupRepository = new Repository<MachineFamily>(context);
                MachineFamily machineGroup = groupRepository.Single(group => group.Id == model.MachineFamily.Id);
                machineGroup.Machines.Add(model);
                context.Commit();
                if (MachineAdded != null)
                    MachineAdded(this, new ModelAddedEventArgs<Machine>(model));
                id = model.Id;
            }
            return id;
        }

        public int AddModel(Machine model, int groupId)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var groupRepository = new Repository<MachineFamily>(context);
                MachineFamily machineGroup = groupRepository.Single(group => group.Id == groupId);
                model.MachineFamily = machineGroup;
                machineGroup.Machines.Add(model);
                context.Commit();
                if (MachineAdded != null)
                    MachineAdded(this, new ModelAddedEventArgs<Machine>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(Machine model)
        {
            using (var context = new SoheilEdmContext())
            {
                var machineRepository = new Repository<Machine>(context);
                var machineGroupRepository = new Repository<MachineFamily>(context);
                Machine entity = machineRepository.Single(machine => machine.Id == model.Id);
                MachineFamily group =
                    machineGroupRepository.Single(machineGroup => machineGroup.Id == model.MachineFamily.Id);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.CreatedDate = model.CreatedDate;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                entity.Status = model.Status;

                entity.MachineFamily = group;

                context.Commit();
            }
        }
        public void UpdateModel(Machine model, int groupId)
        {
            using (var context = new SoheilEdmContext())
            {
                var machineRepository = new Repository<Machine>(context);
                var machineGroupRepository = new Repository<MachineFamily>(context);
                Machine entity = machineRepository.Single(machine => machine.Id == model.Id);
                MachineFamily group =
                    machineGroupRepository.Single(machineGroup => machineGroup.Id == groupId);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.CreatedDate = model.CreatedDate;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                entity.Status = model.Status;

                entity.MachineFamily = group;

                context.Commit();
            }
        }

        public void DeleteModel(Machine model)
        {
        }

        public void AttachModel(Machine model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Machine>(context);
                if (repository.Exists(machine => machine.Id == model.Id))
                {
                    UpdateModel(model);
                }
                else
                {
                    AddModel(model);
                }
            }
        }

        public void AttachModel(Machine model, int groupId)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Machine>(context);
                if (repository.Exists(machine => machine.Id == model.Id))
                {
                    UpdateModel(model, groupId);
                }
                else
                {
                    AddModel(model, groupId);
                }
            }
        }

        #endregion

        public event EventHandler<ModelAddedEventArgs<Machine>> MachineAdded;
        public event EventHandler<ModelAddedEventArgs<StationMachine>> StationAdded;
        public event EventHandler<ModelRemovedEventArgs> StationRemoved;

        public ObservableCollection<StationMachine> GetStations(int machineId)
        {
            ObservableCollection<StationMachine> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Machine>(context);
                Machine entity = repository.FirstOrDefault(machine => machine.Id == machineId, "StationMachines.Station", "StationMachines.Machine");
                models = new ObservableCollection<StationMachine>(entity.StationMachines.Where(item=>item.Station.Status == (decimal)Status.Active));
            }

            return models;
        }

        public void AddStation(int machineId, int stationId)
        {
            using (var context = new SoheilEdmContext())
            {
                var machineRepository = new Repository<Machine>(context);
                var stationRepository = new Repository<Station>(context);
                Machine currentMachine = machineRepository.Single(machine => machine.Id == machineId);
                Station newStation = stationRepository.Single(station => station.Id == stationId);
                if (currentMachine.StationMachines.Any(machineStation => machineStation.Machine.Id == machineId && machineStation.Station.Id == stationId))
                {
                    return;
                }
                var newMachineStation = new StationMachine { Station = newStation, Machine = currentMachine };
                currentMachine.StationMachines.Add(newMachineStation);
                context.Commit();
                StationAdded(this, new ModelAddedEventArgs<StationMachine>(newMachineStation));
            }
        }

        public void RemoveStation(int machineId, int stationId)
        {
            using (var context = new SoheilEdmContext())
            {
                var machineRepository = new Repository<Machine>(context);
                var machineStationRepository = new Repository<StationMachine>(context);
                Machine currentMachine = machineRepository.Single(machine => machine.Id == machineId);
                StationMachine currentMachineStation =
                    currentMachine.StationMachines.First(
                        machineStation =>
                        machineStation.Machine.Id == machineId && machineStation.Id == stationId);
                int id = currentMachineStation.Id;
                machineStationRepository.Delete(currentMachineStation);
                context.Commit();
                StationRemoved(this, new ModelRemovedEventArgs(id));
            }
        }

        /// <summary>
        /// Gets all active machines as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Machine> GetActives()
        {
            ObservableCollection<Machine> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Machine>(context);
                IEnumerable<Machine> entityList =
                    repository.Find(
                        machine => machine.Status == (decimal)Status.Active,"MachineFamily");
                models = new ObservableCollection<Machine>(entityList);
            }
            return models;
        }

        /// <summary>
        /// Gets all active machines as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Machine> GetActives(SoheilEntityType linkType)
        {
            if (linkType == SoheilEntityType.Stations)
            {
                ObservableCollection<Machine> models;
                using (var context = new SoheilEdmContext())
                {
                    var repository = new Repository<Machine>(context);
                    IEnumerable<Machine> entityList =
                        repository.Find(
                            machine => machine.Status == (decimal)Status.Active && machine.StationMachines.Count == 0, "MachineFamily");
                    models = new ObservableCollection<Machine>(entityList);
                }
                return models;
            }
            return GetActives();
        }

		public IEnumerable<Machine> GetActives(int stateStationActivityId, params string[] includePath)
		{
			var models = new List<Machine>();
			using (var context = new SoheilEdmContext())
			{
				var repository = new Repository<StateStationActivityMachine>(context);
				var ssamList = repository.Find(ssam =>
					ssam.StateStationActivity.Id == stateStationActivityId &&
					ssam.Machine.Status == (decimal)Status.Active, includePath);
			    models.AddRange(ssamList.Select(ssam => ssam.Machine));
			}
			return models;
		}
    }
}