using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
	public class ProductDataService : DataServiceBase, IDataService<Product>
	{
		public event EventHandler<ModelAddedEventArgs<Product>> ProductAdded;
		public event EventHandler<ModelAddedEventArgs<ProductDefection>> DefectionAdded;
		public event EventHandler<ModelRemovedEventArgs> DefectionRemoved;
		public event EventHandler<ModelAddedEventArgs<ProductRework>> ReworkAdded;
		public event EventHandler<ModelRemovedEventArgs> ReworkRemoved;
	    readonly Repository<Product> _productRepository;

		public ProductDataService()
			: this(new SoheilEdmContext())
		{ }
		public ProductDataService(SoheilEdmContext context)
		{
			Context = context ?? new SoheilEdmContext();
            _productRepository = new Repository<Product>(Context);

			//along with this event a default FPC and PR is added
			ProductAdded += (sender, e) =>
			{
				new Repository<FPC>(context).Add(
					new FPC
					{
						Name = "*",
						Code = "*",
						CreatedDate = DateTime.Now,
						ModifiedDate = DateTime.Now,
						ModifiedBy = LoginInfo.Id,
						Product = e.NewModel,
						IsDefault = true,
					});
				new Repository<ProductRework>(context).Add(
					new ProductRework
					{
						Name = "*",
						Code = "*",
						Rework = null,
						Product = e.NewModel,
						ModifiedBy = LoginInfo.Id,
					});
			};
		}

		#region IDataService<Product> Members

		public Product GetSingle(int id)
		{
			return _productRepository.Single(product => product.Id == id);
		}

		public ObservableCollection<Product> GetAll()
		{
			var entityList = _productRepository.Find(product => product.Status != (decimal)Status.Deleted, "ProductGroup");
			return new ObservableCollection<Product>(entityList);
		}

		public int AddModel(Product model)
		{
			int id;
			var groupRepository = new Repository<ProductGroup>(Context);
			ProductGroup productGroup = groupRepository.Single(group => group.Id == model.ProductGroup.Id);
			productGroup.Products.Add(model);
			Context.Commit();
			if (ProductAdded != null)
				ProductAdded(this, new ModelAddedEventArgs<Product>(model));
			id = model.Id;
			return id;
		}

		public int AddModel(Product model, int groupId)
		{
			int id;
			var groupRepository = new Repository<ProductGroup>(Context);
			ProductGroup productGroup = groupRepository.Single(group => group.Id == groupId);
			model.ProductGroup = productGroup;
			productGroup.Products.Add(model);
			Context.Commit();
			if (ProductAdded != null)
				ProductAdded(this, new ModelAddedEventArgs<Product>(model));
			id = model.Id;
			return id;
		}

		public void UpdateModel(Product model)
		{
			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;

			//if default FPC and PR exist, update their Name & Code
			createDefaultFPCAndProductRework(model);
			
			Context.Commit();
		}

		public void UpdateModel(Product model, int groupId)
		{
			var productGroupRepository = new Repository<ProductGroup>(Context);
			ProductGroup group =
				productGroupRepository.Single(productGroup => productGroup.Id == groupId);

			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;

			model.ProductGroup = group;

			//if default FPC and PR exist, update their Name & Code
			createDefaultFPCAndProductRework(model);

			Context.Commit();
		}

		public void DeleteModel(Product model)
		{
		}

		public void AttachModel(Product model)
		{
			if (_productRepository.Exists(product => product.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

		public void AttachModel(Product model, int groupId)
		{
			if (_productRepository.Exists(product => product.Id == model.Id))
			{
				UpdateModel(model, groupId);
			}
			else
			{
				AddModel(model, groupId);
			}
		}

		#endregion

		void createDefaultFPCAndProductRework(Product model)
		{
			if (model.FPCs.Count == 1)
			{
				var def = model.FPCs.First();
				if (def.Name == "*" && def.Code == "*")
				{
					def.Name = model.Name;
					def.Code = model.Code;
				}
			}
			if (model.ProductReworks.Count == 1)
			{
				var def = model.ProductReworks.First();
				if (def.Name == "*" && def.Code == "*")
				{
					def.Name = model.Name;
					def.Code = model.Code;
				}
			} 
		}

		public ObservableCollection<Product> GetActives()
		{
			var entityList = _productRepository.Find(product => product.Status == (decimal)Status.Active, "ProductGroup");
			return new ObservableCollection<Product>(entityList);
		}
		public ObservableCollection<Product> GetActives(SoheilEntityType linkType, int linkId)
		{
			if (linkType == SoheilEntityType.Defections)
			{
				var entityList = _productRepository.Find(product => (product.Status == (decimal)Status.Active) && !product.ProductDefections.Any(item => item.Defection.Id == linkId), "ProductGroup");
				return new ObservableCollection<Product>(entityList);
			}
			if (linkType == SoheilEntityType.Reworks)
			{
				var entityList = _productRepository.Find(product => (product.Status == (decimal)Status.Active) && !product.ProductReworks.Any(item=> item.Rework.Id == linkId), "ProductGroup");
				return new ObservableCollection<Product>(entityList);
			}
			return GetActives();
		}

		public ObservableCollection<ProductDefection> GetDefections(int productId)
		{
			Product entity = _productRepository.FirstOrDefault(product => product.Id == productId, "ProductDefections.Defection", "ProductDefections.Product");
			return new ObservableCollection<ProductDefection>(entity.ProductDefections.Where(item => item.Defection.Status == (decimal)Status.Active));
		}

		public void AddDefection(int productId, int defectionId)
		{

			Product currentProduct = _productRepository.Single(product => product.Id == productId);
			if (currentProduct.ProductDefections.Any(productDefection =>
				productDefection.Product.Id == productId
				&& productDefection.Defection.Id == defectionId))
				return;

			var defectionRepository = new Repository<Defection>(Context);
			Defection newDefection = defectionRepository.Single(defection => defection.Id == defectionId);

			var newProductDefection = new ProductDefection { Defection = newDefection, Product = currentProduct };
			currentProduct.ProductDefections.Add(newProductDefection);
			Context.Commit();
			DefectionAdded(this, new ModelAddedEventArgs<ProductDefection>(newProductDefection));
		}

		public void RemoveDefection(int productId, int defectionId)
		{
			var productDefectionRepository = new Repository<ProductDefection>(Context);
			Product currentProduct = _productRepository.Single(product => product.Id == productId);
			ProductDefection currentProductDefection =
				currentProduct.ProductDefections.First(
					productDefection =>
					productDefection.Product.Id == productId && productDefection.Id == defectionId);
			int id = currentProductDefection.Id;
			productDefectionRepository.Delete(currentProductDefection);
			Context.Commit();
			DefectionRemoved(this, new ModelRemovedEventArgs(id));
		}

		public ObservableCollection<ProductRework> GetReworks(int productId)
		{
			Product entity = _productRepository.FirstOrDefault(product => product.Id == productId, "ProductReworks.Product", "ProductReworks.Rework");
			return new ObservableCollection<ProductRework>(entity.ProductReworks.Where(item => item.Rework != null && item.Rework.Status == (decimal)Status.Active));
		}

		public void AddRework(int productId, int reworkId, string code, string name, int modifiedBy)
		{
			var reworkRepository = new Repository<Rework>(Context);
			Product currentProduct = _productRepository.Single(product => product.Id == productId);
			Rework newRework = reworkRepository.Single(rework => rework.Id == reworkId);
			if (currentProduct.ProductReworks.Any(productRework => productRework.Product.Id == productId && productRework.Rework!=null && productRework.Rework.Id == reworkId))
			{
				return;
			}
			var newProductRework = new ProductRework { Rework = newRework, Product = currentProduct, Code = code, Name = name, ModifiedBy = modifiedBy };
			currentProduct.ProductReworks.Add(newProductRework);
			Context.Commit();
			ReworkAdded(this, new ModelAddedEventArgs<ProductRework>(newProductRework));
		}

		public void RemoveRework(int productId, int reworkId)
		{
			var productReworkRepository = new Repository<ProductRework>(Context);
			Product currentProduct = _productRepository.Single(product => product.Id == productId);
			ProductRework currentProductRework =
				currentProduct.ProductReworks.FirstOrDefault(
					productRework =>
					productRework.Product.Id == productId && productRework.Id == reworkId);
			if (currentProductRework == null)
				return;//??? because currentProduct.ProductReworks.FirstOrDefault
			//and that happens when you can't delete it
			
			int id = currentProductRework.Id;
			productReworkRepository.Delete(currentProductRework);

			//correct states
			var stateRepository = new Repository<State>(Context);
			var connectorRepository = new Repository<Connector>(Context);
			int reworkStateTypeNr = (int)StateType.Rework;
			if (stateRepository.Exists(x => x.OnProductRework.Id == id && x.StateTypeNr != reworkStateTypeNr))
				return;
				//???throw new Soheil.Common.SoheilException.SoheilExceptionBase("Can't delete rework because of FPC", Common.SoheilException.ExceptionLevel.Error);

			var states = stateRepository.Find(x => x.OnProductRework.Id == id && x.StateTypeNr == reworkStateTypeNr);
			foreach (var state in states.ToArray())
			{
				foreach (var conn in state.InConnectors.ToArray())
					connectorRepository.Delete(conn);
				foreach (var conn in state.OutConnectors.ToArray())
					connectorRepository.Delete(conn);
				stateRepository.Delete(state);
			}
			Context.Commit();
			ReworkRemoved(this, new ModelRemovedEventArgs(id));
		}
	}
}