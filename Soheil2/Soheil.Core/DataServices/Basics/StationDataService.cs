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
    public class StationDataService : IDataService<Station>
    {
		public IEnumerable<Station> GetAllWithStationMachines()
		{
			List<Station> models;
			using (var context = new SoheilEdmContext())
			{
				var repository = new Repository<Station>(context);
				IEnumerable<Station> entityList = repository.Find(station=> station.Status != (decimal)Status.Deleted,"StationMachines", "StationMachines.Machine");
				models = new List<Station>(entityList);
			}
			return models;
		}


		#region IDataService<StationVM> Members

        public Station GetSingle(int id)
        {
            Station entity;
            using (var context = new SoheilEdmContext())
            {
                var stationRepository = new Repository<Station>(context);
                entity = stationRepository.Single(station => station.Id == id);
        }
            return entity;
        }

		public ObservableCollection<Station> GetAll()
		{
            ObservableCollection<Station> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Station>(context);
                IEnumerable<Station> entityList =
                    repository.Find(
                        station => station.Status != (decimal)Status.Deleted);
                models = new ObservableCollection<Station>(entityList);
            }
            return models;
		}

        public int AddModel(Station model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Station>(context);
                repository.Add(model);
                context.Commit();
                if (StationAdded != null)
                    StationAdded(this, new ModelAddedEventArgs<Station>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(Station model)
        {
            using (var context = new SoheilEdmContext())
            {
                var stationRepository = new Repository<Station>(context);
                Station entity = stationRepository.Single(station => station.Id == model.Id);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.CreatedDate = model.CreatedDate;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                entity.Status = model.Status;
                context.Commit();
            }
        }

        public void DeleteModel(Station model)
        {
        }

        public void AttachModel(Station model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Station>(context);
                if (repository.Exists(station => station.Id == model.Id))
                {
                    UpdateModel(model);
                }
                else
                {
                    AddModel(model);
                }
            }
        }

        #endregion

        public event EventHandler<ModelAddedEventArgs<Station>> StationAdded;
        public event EventHandler<ModelAddedEventArgs<StationMachine>> MachineAdded;
        public event EventHandler<ModelRemovedEventArgs> MachineRemoved;

        public ObservableCollection<StationMachine> GetMachines(int stationId)
        {
            ObservableCollection<StationMachine> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Station>(context);
                Station entity = repository.FirstOrDefault(station => station.Id == stationId, "StationMachines.Machine", "StationMachines.Station");
                models = new ObservableCollection<StationMachine>(entity.StationMachines.Where(item=>item.Machine.Status == (decimal)Status.Active));
            }

            return models;
        }

        public void AddMachine(int stationId, int machineId)
        {
            using (var context = new SoheilEdmContext())
            {
                var stationRepository = new Repository<Station>(context);
                var machineRepository = new Repository<Machine>(context);
                Station currentStation = stationRepository.Single(station => station.Id == stationId);
                Machine newMachine = machineRepository.Single(machine => machine.Id == machineId);
                if (currentStation.StationMachines.Any(stationMachine => stationMachine.Station.Id == stationId && stationMachine.Machine.Id == machineId))
                {
                    return;
                }
                var newStationMachine = new StationMachine { Machine = newMachine, Station = currentStation };
                currentStation.StationMachines.Add(newStationMachine);
                context.Commit();
                MachineAdded(this, new ModelAddedEventArgs<StationMachine>(newStationMachine));
            }
        }

        public void RemoveMachine(int stationId, int machineId)
        {
            using (var context = new SoheilEdmContext())
            {
                var stationRepository = new Repository<Station>(context);
                var stationMachineRepository = new Repository<StationMachine>(context);
                Station currentStation = stationRepository.Single(station => station.Id == stationId);
                StationMachine currentStationMachine =
                    currentStation.StationMachines.First(
                        stationMachine =>
                        stationMachine.Station.Id == stationId && stationMachine.Id == machineId);
                int id = currentStationMachine.Id;
                stationMachineRepository.Delete(currentStationMachine);
                context.Commit();
                MachineRemoved(this, new ModelRemovedEventArgs(id));
            }
        }

        /// <summary>
        /// Gets all active products as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Station> GetActives()
        {
            ObservableCollection<Station> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Station>(context);
                IEnumerable<Station> entityList =
                    repository.Find(
                        station => station.Status == (decimal)Status.Active);
                models = new ObservableCollection<Station>(entityList);
            }
            return models;
        }

        /// <summary>
        /// Gets all active products as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Station> GetActives(SoheilEntityType linkType)
        {
            if (linkType == SoheilEntityType.Machines)
            {
                ObservableCollection<Station> models;
                using (var context = new SoheilEdmContext())
                {
                    var repository = new Repository<Station>(context);
                    IEnumerable<Station> entityList =
                        repository.Find(
                            station => station.Status == (decimal)Status.Active && station.StationMachines.Count == 0);
                    models = new ObservableCollection<Station>(entityList);
                }
                return models;
            }
            return GetActives();
        }
    }
}