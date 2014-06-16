using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class ProductDefectionDataService :DataServiceBase, IDataService<ProductDefection>
    {
        public event EventHandler<ModelAddedEventArgs<ProductDefection>> ModelUpdated;
        private readonly Repository<ProductDefection> _productDefectionRepository;

        public ProductDefectionDataService(SoheilEdmContext context)
        {
            Context = context;
            _productDefectionRepository = new Repository<ProductDefection>(context);
        }
        public ProductDefectionDataService()
        {
            Context = new SoheilEdmContext();
            _productDefectionRepository = new Repository<ProductDefection>(Context);
        }
        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ProductDefection GetSingle(int id)
        {
               return _productDefectionRepository.Single(productDefection => productDefection.Id == id);
        }

        /// <summary>
        /// Gets a list of view models representing all records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<ProductDefection> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a list of view models representing currently active records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<ProductDefection> GetActives()
        {
            throw new NotImplementedException();
        }

        public int AddModel(ProductDefection model)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateModel(ProductDefection model)
        {
            Context.Commit();
            if (ModelUpdated != null) ModelUpdated(this, new ModelAddedEventArgs<ProductDefection>(model));
        }

        public void DeleteModel(ProductDefection model)
        {
            throw new System.NotImplementedException();
        }

        public void AttachModel(ProductDefection model)
        {
            throw new System.NotImplementedException();
        }

		public ProductDefection[] GetActivesForProduct(int productId)
		{
				return _productDefectionRepository.Find(
					x => x.Product.Id == productId && x.Defection.Status == (byte)Status.Active,
					"Defection").ToArray();
		}
	}
}