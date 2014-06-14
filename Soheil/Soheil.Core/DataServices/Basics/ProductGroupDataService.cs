using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
	public class ProductGroupDataService : DataServiceBase, IDataService<ProductGroup>
	{
		public event EventHandler<ModelAddedEventArgs<ProductGroup>> ProductGroupAdded;
		Repository<ProductGroup> _productGroupRepository;

		public ProductGroupDataService()
			: this(new SoheilEdmContext())
		{
		}
		public ProductGroupDataService(SoheilEdmContext context)
		{
			this.context = context;
			_productGroupRepository = new Repository<ProductGroup>(context);
		}

		#region IDataService<ProductGroup> Members

		public ProductGroup GetSingle(int id)
		{
			return _productGroupRepository.Single(productGroup => productGroup.Id == id);
		}

		public ObservableCollection<ProductGroup> GetAll()
		{
			var entityList = _productGroupRepository.Find(group => group.Status != (byte)Status.Deleted);
			return new ObservableCollection<ProductGroup>(entityList);
		}
		public ObservableCollection<ProductGroup> GetActives()
		{
			var entityList = _productGroupRepository.Find(group => 
				(group.Status == (byte)Status.Active) &&
				group.Products.Any(x => 
					(x.Status == (byte)Status.Active) &&
					x.ProductReworks.Any(pr => pr.Status == (byte)Status.Active))
				, "Products.ProductReworks.Rework");
			return new ObservableCollection<ProductGroup>(entityList);
		}

		/// <summary>
		/// <para>Fetches active products with filtered data (Products.ProductRework.Rework)</para>
		/// <para>according to their states and the specified station</para>
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ProductGroup> GetActivesRecursive(int stationId)
		{
			List<ProductGroup> pgCopies = new List<ProductGroup>();
			var repository = new Repository<Product>(context);
			var station = new Repository<Station>(context).Single(x => x.Id == stationId);
			var allProducts = repository.Find(
				x => x.Status == (byte)Status.Active,
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
				foreach (var productRework in product.ProductReworks.Where(x => x.Status == (byte)Status.Active))
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
				//if pg is inactive, skip
				if (product.ProductGroup.Status != (byte)Status.Active) 
					continue;
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

			return pgCopies;
		}

		public int AddModel(ProductGroup model)
		{
			int id;
			_productGroupRepository.Add(model);
			context.Commit();
			if (ProductGroupAdded != null)
				ProductGroupAdded(this, new ModelAddedEventArgs<ProductGroup>(model));
			id = model.Id;
			return id;
		}

		public void UpdateModel(ProductGroup model)
		{
			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;
			context.Commit();
		}

		public void DeleteModel(ProductGroup model)
		{
		}

		public void AttachModel(ProductGroup model)
		{
			if (_productGroupRepository.Exists(productGroup => productGroup.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

		#endregion


		/// <summary>
		/// Gets the model.
		/// </summary>
		/// <param name="id">The product group id.</param>
		/// <returns></returns>
		public ProductGroup GetModel(int id)
		{
			return _productGroupRepository.Single(x => x.Id == id);
		}
	}
}