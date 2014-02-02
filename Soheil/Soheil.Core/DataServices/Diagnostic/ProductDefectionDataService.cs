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
    public class ProductDefectionDataService : IDataService<ProductDefection>
    {
        public event EventHandler<ModelAddedEventArgs<ProductDefection>> ModelUpdated;
        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ProductDefection GetSingle(int id)
        {
            ProductDefection entity;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ProductDefection>(context);
                entity = repository.Single(productDefection => productDefection.Id == id);
            }
            return entity;
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
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ProductDefection>(context);
                ProductDefection entity = repository.Single(productDefection => productDefection.Id == model.Id);

                context.Commit();
                if (ModelUpdated != null) ModelUpdated(this, new ModelAddedEventArgs<ProductDefection>(entity));
            }
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
			ProductDefection[] models;
			using (var context = new SoheilEdmContext())
			{
				var repository = new Repository<ProductDefection>(context);
				models = repository.Find(
					x => x.Product.Id == productId && x.Defection.Status == (byte)Status.Active,
					"Defection").ToArray();
			}
			return models;
		}
	}
}