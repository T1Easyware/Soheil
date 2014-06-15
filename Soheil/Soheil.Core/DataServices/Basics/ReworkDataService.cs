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
	    readonly Repository<Rework> _reworkRepository;

		public ReworkDataService(SoheilEdmContext context)
		{
			Context = context ?? new SoheilEdmContext();
            _reworkRepository = new Repository<Rework>(Context);
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
			Context.Commit();
			if (ReworkAdded != null)
				ReworkAdded(this, new ModelAddedEventArgs<Rework>(model));
			id = model.Id;
			return id;
		}

		public void UpdateModel(Rework model)
		{
			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;
			Context.Commit();
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
		public ObservableCollection<Rework> GetActives(SoheilEntityType linkType, int linkId)
		{
			if (linkType == SoheilEntityType.Products)
			{
				var entityList = _reworkRepository.Find(rework => (rework.Status == (decimal)Status.Active) && rework.ProductReworks.All(item=>item.Product.Id != linkId));
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
			var productRepository = new Repository<Product>(Context);
			Rework currentRework = _reworkRepository.Single(rework => rework.Id == reworkId);
			Product newProduct = productRepository.Single(product => product.Id == productId);
			if (currentRework.ProductReworks.Any(reworkProduct => reworkProduct.Rework.Id == reworkId && reworkProduct.Product.Id == productId))
			{
				return;
			}
			var newProductRework = new ProductRework { Product = newProduct, Rework = currentRework, Code = code, Name = name, ModifiedBy = modifiedBy };
			currentRework.ProductReworks.Add(newProductRework);
			Context.Commit();
			ProductAdded(this, new ModelAddedEventArgs<ProductRework>(newProductRework));
		}

		public void RemoveProduct(int reworkId, int productId)
		{
			var reworkProductRepository = new Repository<ProductRework>(Context);
            Rework currentRework = _reworkRepository.Single(rework => rework.Id == reworkId);
			ProductRework currentReworkProduct =
				currentRework.ProductReworks.First(
					reworkProduct =>
					reworkProduct.Rework.Id == reworkId && reworkProduct.Id == productId);
			int id = currentReworkProduct.Id;
			reworkProductRepository.Delete(currentReworkProduct);
			Context.Commit();
			ProductRemoved(this, new ModelRemovedEventArgs(id));

		}
	}
}