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
		Repository<Connector> connectorRepository;
		Repository<State> stateRepository;
		public ConnectorDataService(SoheilEdmContext context)
		{
			this.context = context;
			connectorRepository = new Repository<Connector>(context);
			stateRepository = new Repository<State>(context);
		}

		public IEnumerable<Connector> GetByFpcId(int fpcId)
		{
			return connectorRepository
					.Find(x => x.StartState.FPC.Id == fpcId, "StartState", "EndState")
					.ToList();
		}


		public Connector GetSingle(int id)
		{
			throw new NotImplementedException();
		}

		public System.Collections.ObjectModel.ObservableCollection<Connector> GetAll()
		{
			throw new NotImplementedException();
		}

		public System.Collections.ObjectModel.ObservableCollection<Connector> GetActives()
		{
			throw new NotImplementedException();
		}

		public int AddModel(Connector model)
		{
			var entity = new Connector
			{
				StartState = stateRepository.FirstOrDefault(x => x.Id == model.StartState.Id),
				EndState = stateRepository.FirstOrDefault(x => x.Id == model.EndState.Id),
			};
			connectorRepository.Add(entity);
			context.Commit();
			return entity.Id;
		}

		public void UpdateModel(Connector model)
		{
			throw new NotImplementedException();
		}

		public void DeleteModel(Connector model)
		{
			connectorRepository.Delete(model);
		}

		public void AttachModel(Connector model)
		{
			throw new NotImplementedException();
		}
	}
}
