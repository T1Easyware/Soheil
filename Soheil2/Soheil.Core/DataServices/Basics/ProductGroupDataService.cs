using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class ProductGroupDataService : IDataService<ProductGroup>
    {
        #region IDataService<ProductGroup> Members

        public ProductGroup GetSingle(int id)
        {
            ProductGroup entity;
            using (var context = new SoheilEdmContext())
            {
                var productGroupRepository = new Repository<ProductGroup>(context);
                entity = productGroupRepository.Single(productGroup => productGroup.Id == id);
            }
            return entity;
        }

        public ObservableCollection<ProductGroup> GetAll()
        {
            ObservableCollection<ProductGroup> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ProductGroup>(context);
                IEnumerable<ProductGroup> entityList = repository.Find(group=>group.Status != (decimal)Status.Deleted);
                models = new ObservableCollection<ProductGroup>(entityList);
            }
            return models;
        }

		public ObservableCollection<ProductGroup> GetActives()
		{
            ObservableCollection<ProductGroup> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ProductGroup>(context);
                IEnumerable<ProductGroup> entityList = repository.Find(group => group.Status == (decimal)Status.Active);
                models = new ObservableCollection<ProductGroup>(entityList);
            }
            return models;
		}

		/// <summary>
		/// Fetches active products with full data (Products.ProductRework.Rework)
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<ProductGroup> GetActivesRecursive()
		{
			ObservableCollection<ProductGroup> models;
			using (var context = new SoheilEdmContext())
			{
				var repository = new Repository<ProductGroup>(context);
				IEnumerable<ProductGroup> entityList = repository.GetAll("Products", "Products.ProductReworks", "Products.ProductReworks.Rework");
				models = new ObservableCollection<ProductGroup>(entityList);
			}
			return models;
		}
		/// <summary>
		/// <para>Fetches active products with filtered data (Products.ProductRework.Rework)</para>
		/// <para>according to their states and the specified station</para>
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ProductGroup> GetActivesRecursive(int stationId)
		{
			List<ProductGroup> pgCopies = new List<ProductGroup>();
			using (var context = new SoheilEdmContext())
			{
				var repository = new Repository<Product>(context);
				var station = new Repository<Station>(context).Single(x => x.Id == stationId);
				var allProducts = repository.GetAll(
					"ProductGroup",
					"ProductReworks", "ProductReworks.Rework",
					"ProductReworks.Warmups", "ProductReworks.Warmups.Station", 
					"FPCs", "FPCs.States", "FPCs.States.StateStations",
					"FPCs.States.OnProductRework", "FPCs.States.OnProductRework.Rework",
					"FPCs.States.OutConnectors", "FPCs.States.OutConnectors.EndState").ToList();
				
				//make a copy of all products
				foreach (var product in allProducts)
				{
					var productCopy = new Product
					{
						Id = product.Id,
						Name = product.Name,
						Code = product.Code,
						Color = product.Color,
					};

					//find default fpc
					var fpc = product.FPCs.FirstOrDefault(x => x.IsDefault);
					if (fpc == null) continue;

					#region Rework
					//for all PRs
					foreach (var productRework in product.ProductReworks)
					{
						//find states that matches current PR and station
						var states = (productRework.Rework == null) ?
							fpc.States.Where(x => x.IsReworkState == Bool3.False
								&& x.StateStations.Any(y => y.Station.Id == station.Id)) :
							fpc.States.Where(x => x.IsReworkState == Bool3.True
								&& x.StateStations.Any(y => y.Station.Id == station.Id));
						//add a copy of PR to productCopy
						if (states.Any())
						{
							var pr = new ProductRework
								{
									Id = productRework.Id,
									Name = productRework.Name,
									Code = productRework.Code,
									Product = productCopy,
									Rework = productRework.Rework,
								};
							//add warmup
							var wu = productRework.Warmups.FirstOrDefault(x => x.Station.Id == station.Id);
							if (wu == null) wu = new Warmup
							{
								Id = -1,
								Station = station,
								ProductRework = productRework,
								Seconds = 0,
							};
							pr.Warmups.Add(wu);
							productCopy.ProductReworks.Add(pr);
						}
					}
					if (!productCopy.ProductReworks.Any()) continue;
					#endregion

					#region Product And Group
					//if empty pg, skip
					if (!product.ProductGroup.Products.Any(x => !x.ProductReworks.Any())) continue;

					var pgCopy = pgCopies.FirstOrDefault(x => x.Id == product.ProductGroup.Id);
					if (pgCopy == null)
					{
						pgCopy = new ProductGroup
						{
							Id = product.ProductGroup.Id,
							Name = product.ProductGroup.Name,
							Code = product.ProductGroup.Code,
						};
						pgCopies.Add(pgCopy);
					}
					pgCopy.Products.Add(productCopy); 
					#endregion
				}

			}
			return pgCopies;
		}

        public int AddModel(ProductGroup model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ProductGroup>(context);
                repository.Add(model);
                context.Commit();
                if (ProductGroupAdded != null)
                    ProductGroupAdded(this, new ModelAddedEventArgs<ProductGroup>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(ProductGroup model)
        {
            using (var context = new SoheilEdmContext())
            {
                var productGroupRepository = new Repository<ProductGroup>(context);
                ProductGroup entity = productGroupRepository.Single(productGroup => productGroup.Id == model.Id);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.CreatedDate = model.CreatedDate;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                context.Commit();
            }
        }

        public void DeleteModel(ProductGroup model)
        {
        }

        public void AttachModel(ProductGroup model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ProductGroup>(context);
                if (repository.Exists(productGroup => productGroup.Id == model.Id))
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

        public event EventHandler<ModelAddedEventArgs<ProductGroup>> ProductGroupAdded;

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <param name="id">The product group id.</param>
        /// <returns></returns>
        public ProductGroup GetModel(int id)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ProductGroup>(context);
                return repository.Single(product => product.Id == id);
            }
        }
    }
}