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
    public class StationDataService : DataServiceBase, IDataService<Station>
    {
		public event EventHandler<ModelAddedEventArgs<Station>> StationAdded;
		public event EventHandler<ModelAddedEventArgs<StationMachine>> MachineAdded;
		public event EventHandler<ModelRemovedEventArgs> MachineRemoved;
        readonly Repository<Station> _stationRepository;

		public StationDataService()
			: this(new SoheilEdmContext())
		{
		}

		public StationDataService(SoheilEdmContext context)
		{
			Context = context;
			_stationRepository = new Repository<Station>(context);
		}



		#region IDataService<StationVM> Members

        public Station GetSingle(int id)
        {
			return _stationRepository.Single(station => station.Id == id);
        }

		public ObservableCollection<Station> GetAll()
		{
			ObservableCollection<Station> models;
			IEnumerable<Station> entityList =
				_stationRepository.Find(
					station => station.Status != (decimal)Status.Deleted);
			models = new ObservableCollection<Station>(entityList);
			return models;
		}

		/// <summary>
		/// Gets all active station models.
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<Station> GetActives()
		{
			ObservableCollection<Station> models;
			IEnumerable<Station> entityList =
				_stationRepository.Find(station => station.Status == (decimal)Status.Active,
				"StationMachines.Machine.MachineFamily");
			models = new ObservableCollection<Station>(entityList);
			return models;
		}

		public int AddModel(Station model)
		{
			int id;
			_stationRepository.Add(model);
			Context.Commit();
			if (StationAdded != null)
				StationAdded(this, new ModelAddedEventArgs<Station>(model));
			id = model.Id;
			return id;
		}

		public void UpdateModel(Station model)
		{
			Station entity = _stationRepository.Single(station => station.Id == model.Id);

			entity.Code = model.Code;
			entity.Name = model.Name;
			entity.CreatedDate = model.CreatedDate;
			entity.ModifiedBy = LoginInfo.Id;
			entity.ModifiedDate = DateTime.Now;
			entity.Status = model.Status;
			Context.Commit();
		}

        public void DeleteModel(Station model)
        {
        }

		public void AttachModel(Station model)
		{
			if (_stationRepository.Exists(station => station.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

        #endregion


		public ObservableCollection<StationMachine> GetMachines(int stationId)
		{
			ObservableCollection<StationMachine> models;
			Station entity = _stationRepository.FirstOrDefault(station => station.Id == stationId, "StationMachines.Machine", "StationMachines.Station");
			models = new ObservableCollection<StationMachine>(entity.StationMachines.Where(item => item.Machine.Status == (decimal)Status.Active));

			return models;
		}


		public void AddMachine(int stationId, int machineId)
		{
			var machineRepository = new Repository<Machine>(Context);
			Station currentStation = _stationRepository.Single(station => station.Id == stationId);
			Machine newMachine = machineRepository.Single(machine => machine.Id == machineId);
			if (currentStation.StationMachines.Any(stationMachine => stationMachine.Station.Id == stationId && stationMachine.Machine.Id == machineId))
			{
				return;
			}
			var newStationMachine = new StationMachine { Machine = newMachine, Station = currentStation };
			currentStation.StationMachines.Add(newStationMachine);
			Context.Commit();
			MachineAdded(this, new ModelAddedEventArgs<StationMachine>(newStationMachine));
		}

		public void RemoveMachine(int stationId, int machineId)
		{
			var stationMachineRepository = new Repository<StationMachine>(Context);
			Station currentStation = _stationRepository.Single(station => station.Id == stationId);
			StationMachine currentStationMachine =
				currentStation.StationMachines.First(
					stationMachine =>
					stationMachine.Station.Id == stationId && stationMachine.Id == machineId);
			int id = currentStationMachine.Id;
			stationMachineRepository.Delete(currentStationMachine);
			Context.Commit();
			MachineRemoved(this, new ModelRemovedEventArgs(id));
		}


        /// <summary>
        /// Gets all active products as view models.
        /// </summary>
        /// <returns></returns>
		public ObservableCollection<Station> GetActives(SoheilEntityType linkType, int linkId)
		{
			if (linkType == SoheilEntityType.Machines)
			{
				ObservableCollection<Station> models;
				IEnumerable<Station> entityList =
					_stationRepository.Find(
						station => (station.Status == (decimal)Status.Active)
							&& station.StationMachines.All(item=>item.Machine.Id != linkId));
				models = new ObservableCollection<Station>(entityList);
				return models;
			}
			return GetActives();
		}

		internal int GetNextIndex()
		{
			var list = _stationRepository.GetAll();
			if (list.Any())
				return list.Max(x => x.Index) + 1;
			else
				return 0;
		}

		internal IEnumerable<Station> FixAndGetActives()
		{
			var all = GetActives().OrderBy(x => x.Index).ToArray();
			for (int i = 0; i < all.Length; i++)
			{
				all[i].Index = i;
			}

			Context.Commit();
			return all;
		}
	}
}