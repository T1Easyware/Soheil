using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
	public class ReworkDataService : DataServiceBase, IDataService<Rework>
	{
		public event EventHandler<ModelAddedEventArgs<Rework>> ReworkAdded;
		public event EventHandler<ModelAddedEventArgs<ProductRework>> ProductAdded;
		public event EventHandler<ModelRemovedEventArgs> ProductRemoved;
		Repository<Rework> _reworkRepository;

		public ReworkDataService()
			:this(new SoheilEdmContext())
		{
		}
		public ReworkDataService(SoheilEdmContext context)
		{
			this.context = context;
			_reworkRepository = new Repository<Rework>(context);
		}

		#region IDataService<Rework> Members

		public Rework GetSingle(int id)
		{
			return _reworkRepository.Single(rework => rework.Id == id);
		}

		public ObservableCollection<Rework> GetAll()
		{
			IEnumerable<Rework> entityList = _reworkRepository.GetAll();
			return new ObservableCollection<Rework>(entityList);
		}

		public int AddModel(Rework model)
		{
			int id;
			_reworkRepository.Add(model);
			context.Commit();
			if (ReworkAdded != null)
				ReworkAdded(this, new ModelAddedEventArgs<Rework>(model));
			id = model.Id;
			return id;
		}

		public void UpdateModel(Rework model)
		{
			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;
			context.Commit();
		}

		public void DeleteModel(Rework model)
		{
		}

		public void AttachModel(Rework model)
		{
			if (_reworkRepository.Exists(rework => rework.Id == model.Id))
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
		/// Gets all active products as view models.
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<Rework> GetActives()
		{
			var entityList = _reworkRepository.Find(rework => rework.Status == (decimal)Status.Active);
			return new ObservableCollection<Rework>(entityList);
		}

		/// <summary>
		/// Gets all active products as view models.
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<Rework> GetActives(SoheilEntityType linkType)
		{
			if (linkType == SoheilEntityType.Products)
			{
				var entityList = _reworkRepository.Find(rework => rework.Status == (decimal)Status.Active && rework.ProductReworks.Count == 0);
				return new ObservableCollection<Rework>(entityList);
			}
			return GetActives();
		}

		public ObservableCollection<ProductRework> GetProducts(int reworkId)
		{
			Rework entity = _reworkRepository.FirstOrDefault(rework => rework.Id == reworkId, "ProductReworks.Product", "ProductReworks.Rework");
			return new ObservableCollection<ProductRework>(entity.ProductReworks);
		}

		public void AddProduct(int reworkId, int productId, string code, string name, int modifiedBy)
		{
			var productRepository = new Repository<Product>(context);
			Rework currentRework = _reworkRepository.Single(rework => rework.Id == reworkId);
			Product newProduct = productRepository.Single(product => product.Id == productId);
			if (currentRework.ProductReworks.Any(reworkProduct => reworkProduct.Rework.Id == reworkId && reworkProduct.Product.Id == productId))
			{
				return;
			}
			var newProductRework = new ProductRework { Product = newProduct, Rework = currentRework, Code = code, Name = name, ModifiedBy = modifiedBy };
			currentRework.ProductReworks.Add(newProductRework);
			context.Commit();
			ProductAdded(this, new ModelAddedEventArgs<ProductRework>(newProductRework));
		}

		public void RemoveProduct(int reworkId, int productId)
		{
			var reworkProductRepository = new Repository<ProductRework>(context);
			Rework currentRework = _reworkRepository.Single(rework => rework.Id == reworkId);
			ProductRework currentReworkProduct =
				currentRework.ProductReworks.First(
					reworkProduct =>
					reworkProduct.Rework.Id == reworkId && reworkProduct.Id == productId);
			int id = currentReworkProduct.Id;
			reworkProductRepository.Delete(currentReworkProduct);
			context.Commit();
			ProductRemoved(this, new ModelRemovedEventArgs(id));
		}
	}
}