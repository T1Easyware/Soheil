using System;
using System.Collections.ObjectModel;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
	public class StationMachineDataService : DataServiceBase, IDataService<StationMachine>
	{
		public event EventHandler<ModelAddedEventArgs<StationMachine>> ModelUpdated;
		Repository<StationMachine> _stationMachineRepository;

		public StationMachineDataService()
			: this(new SoheilEdmContext())
		{
		}
		public StationMachineDataService(SoheilEdmContext context)
		{
			this.context = context;
			_stationMachineRepository = new Repository<StationMachine>(context);
		}

		/// <summary>
		/// Gets a single view model.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		public StationMachine GetSingle(int id)
		{
			return _stationMachineRepository.Single(productDefection => productDefection.Id == id);
		}

		/// <summary>
		/// Gets a list of view models representing all records of the entity.
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<StationMachine> GetAll()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Gets a list of view models representing currently active records of the entity.
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<StationMachine> GetActives()
		{
			throw new NotImplementedException();
		}

		public int AddModel(StationMachine model)
		{
			throw new System.NotImplementedException();
		}

		public void UpdateModel(StationMachine model)
		{
			StationMachine entity = _stationMachineRepository.Single(productDefection => productDefection.Id == model.Id);
			entity.RecordStatus = model.RecordStatus;
			context.Commit();
			if (ModelUpdated != null) ModelUpdated(this, new ModelAddedEventArgs<StationMachine>(entity));
		}

		public void DeleteModel(StationMachine model)
		{
			throw new System.NotImplementedException();
		}

		public void AttachModel(StationMachine model)
		{
			throw new System.NotImplementedException();
		}
	}
}