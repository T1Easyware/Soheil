using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Soheil.Common;
using Soheil.Common.Annotations;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
	public class ProductReworkDataService : DataServiceBase, IDataService<ProductRework>
	{
		public event EventHandler<ModelAddedEventArgs<ProductRework>> ModelUpdated;
	    readonly Repository<ProductRework> _productReworkRepository;

		public IEnumerable<ProductRework> GetWhere(Expression<Func<ProductRework, bool>> where)
		{
			return new List<ProductRework>(_productReworkRepository.Find(where));
		}

		public ProductReworkDataService(SoheilEdmContext context)
		{
			this.Context = context ?? new SoheilEdmContext();
            _productReworkRepository = new Repository<ProductRework>(Context);
		}

		/// <summary>
		/// Gets a single view model.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		public ProductRework GetSingle(int id)
		{
			throw new System.NotImplementedException();
		}
		public ProductRework GetMainForProduct(int pid)
		{
			return _productReworkRepository.Single(x => x.Product.Id == pid && x.Rework == null);
		}

		/// <summary>
		/// Gets a list of view models representing all records of the entity.
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<ProductRework> GetAll()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Gets a list of view models representing currently active records of the entity.
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<ProductRework> GetActives()
		{
            var entityList = _productReworkRepository.Find(price => price.Status == (byte)Status.Active);
            return new ObservableCollection<ProductRework>(entityList);
		}

		public int AddModel(ProductRework model)
		{
			_productReworkRepository.Add(model);
			return model.Id;
		}

		public void UpdateModel(ProductRework model)
		{
			model.ModifiedBy = LoginInfo.Id;
			Context.Commit();
			if (ModelUpdated != null) ModelUpdated(this, new ModelAddedEventArgs<ProductRework>(model));
		}

		public void DeleteModel(ProductRework model)
		{
			throw new System.NotImplementedException();
		}

		public void AttachModel(ProductRework model)
		{
			throw new System.NotImplementedException();
		}

        public bool HasStateLink(int id)
	    {
	        bool hasLink = false;
	        using (var context = new SoheilEdmContext())
	        {
                var stateRepository = new Repository<State>(context);
	            if (stateRepository.Exists(item => item.OnProductRework != null && item.OnProductRework.Id == id)) 
                    hasLink = true;
	        }
	        return hasLink;
	    }
	}
}