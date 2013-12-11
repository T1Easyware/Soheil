using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class ProductReworkDataService : IDataService<ProductRework>
    {
        public event EventHandler<ModelAddedEventArgs<ProductRework>> ModelUpdated;

		public IEnumerable<ProductRework> GetWhere(Expression<Func<ProductRework, bool>> where)
		{
			using (var context = new SoheilEdmContext())
			{
				var models = new Repository<ProductRework>(context).Find(where);
				return new List<ProductRework>(models);
			}
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
			using (var context = new Dal.SoheilEdmContext())
			{
				return new Repository<ProductRework>(context).Single(x => x.Product.Id == pid && x.Rework == null);
			}
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
            throw new NotImplementedException();
        }

        public int AddModel(ProductRework model)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateModel(ProductRework model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ProductRework>(context);
                ProductRework entity = repository.Single(productRework => productRework.Id == model.Id);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.ModifiedBy = LoginInfo.Id;
                context.Commit();
                if (ModelUpdated != null) ModelUpdated(this, new ModelAddedEventArgs<ProductRework>(entity));
            }
        }

        public void DeleteModel(ProductRework model)
        {
            throw new System.NotImplementedException();
        }

        public void AttachModel(ProductRework model)
        {
            throw new System.NotImplementedException();
        }


	}
}