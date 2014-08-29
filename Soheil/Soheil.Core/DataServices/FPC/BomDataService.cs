using Soheil.Core.Base;
using Soheil.Core.Interfaces;
using Soheil.Model;
using Soheil.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.DataServices
{
	public class BomDataService : DataServiceBase, IDataService<BOM>
	{
		private Repository<BOM> _repository;
		public BomDataService()
			: this (new Dal.SoheilEdmContext())
		{

		}
		public BomDataService(Dal.SoheilEdmContext context)
		{
			Context = context;
			_repository = new Repository<BOM>(context);
		}
		public Model.BOM GetSingle(int id)
		{
			throw new NotImplementedException();
		}

		public System.Collections.ObjectModel.ObservableCollection<Model.BOM> GetAll()
		{
			throw new NotImplementedException();
		}

		public System.Collections.ObjectModel.ObservableCollection<Model.BOM> GetActives()
		{
			throw new NotImplementedException();
		}

		public int AddModel(Model.BOM model)
		{
			throw new NotImplementedException();
		}

		public void UpdateModel(Model.BOM model)
		{
			throw new NotImplementedException();
		}

		public void DeleteModel(Model.BOM model)
		{
			_repository.Delete(model);
		}

		public void AttachModel(Model.BOM model)
		{
			throw new NotImplementedException();
		}
	}
}
