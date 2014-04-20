using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Core.Base;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels.Fpc;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
	public class ConnectorDataService : DataServiceBase, IDataService<Connector>
	{
		Repository<Connector> _connectorRepository;
		FPCDataService _parentDataService;

		internal ConnectorDataService(SoheilEdmContext context, FPCDataService parentDataService)
		{
			this.context = context;
			_connectorRepository = new Repository<Connector>(context);
			_parentDataService = parentDataService;
		}

		public IEnumerable<Connector> GetByFpcId(int fpcId)
		{
			return _connectorRepository
					.Find(x => x.StartState.FPC.Id == fpcId, "StartState", "EndState")
					.ToList();
		}
		/// <summary>
		/// No Save changes... just add
		/// </summary>
		/// <param name="startStateId"></param>
		/// <param name="endStateId"></param>
		internal void AddConnector(int startStateId, int endStateId)
		{
			var startStateModel = _parentDataService.stateDataService.GetSingle(startStateId);
			var endStateModel = _parentDataService.stateDataService.GetSingle(endStateId);
			var connectorModel = new Soheil.Model.Connector
			{
				StartState = startStateModel,
				EndState = endStateModel
			};
			_connectorRepository.Add(connectorModel);
			context.Commit();
		}

		#region IDataService

		public Connector GetSingle(int id)
		{
			return _connectorRepository.FirstOrDefault(x => x.Id == id);
		}

		public System.Collections.ObjectModel.ObservableCollection<Connector> GetAll()
		{
			return new System.Collections.ObjectModel.ObservableCollection<Connector>(_connectorRepository.GetAll());
		}

		public System.Collections.ObjectModel.ObservableCollection<Connector> GetActives()
		{
			throw new NotImplementedException();
		}

		public int AddModel(Connector model)
		{
			var entity = new Connector
			{
				StartState = _parentDataService.stateDataService.GetSingle(model.StartState.Id),
				EndState = _parentDataService.stateDataService.GetSingle(model.EndState.Id),
			};
			_connectorRepository.Add(entity);
			context.Commit();
			return entity.Id;
		}

		public void UpdateModel(Connector model)
		{
			throw new NotImplementedException();
		}

		public void DeleteModel(Connector model)
		{
			_connectorRepository.Delete(model);
			context.Commit();
		}

		public void AttachModel(Connector model)
		{
			throw new NotImplementedException();
		} 
		#endregion

	}
}
