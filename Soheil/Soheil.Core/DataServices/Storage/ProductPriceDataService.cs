using System;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;
using System.Collections.Generic;

namespace Soheil.Core.DataServices
{
    public class ProductPriceDataService : DataServiceBase, IDataService<ProductPrice>
    {
		public event EventHandler<ModelAddedEventArgs<ProductPrice>> ProductPriceAdded;
        readonly Repository<ProductPrice> _productPriceRepository;

		public ProductPriceDataService()
			: this(new SoheilEdmContext())
		{
		}

		public ProductPriceDataService(SoheilEdmContext context)
		{
			Context = context;
			_productPriceRepository = new Repository<ProductPrice>(context);
		}



		#region IDataService<ProductPriceVM> Members

        public ProductPrice GetSingle(int id)
        {
			return _productPriceRepository.Single(ProductPrice => ProductPrice.Id == id);
        }

		public ObservableCollection<ProductPrice> GetAll()
		{
            var entityList = _productPriceRepository.Find(price => price.Status != (decimal)Status.Deleted );
            return new ObservableCollection<ProductPrice>(entityList);
		}

		/// <summary>
		/// Gets all active ProductPrice models.
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<ProductPrice> GetActives()
		{
            var entityList = _productPriceRepository.Find(price => price.Status == (byte)Status.Active);
            return new ObservableCollection<ProductPrice>(entityList);
		}

        public ObservableCollection<ProductPrice> GetProductActivePrices(int productId)
        {
            var entityList = _productPriceRepository.Find(price => price.Product != null && price.Product.Id == productId && price.Status == (byte)Status.Active);
            return new ObservableCollection<ProductPrice>(entityList);
        }

		public int AddModel(ProductPrice model)
		{
		    _productPriceRepository.Add(model);
			Context.Commit();
			if (ProductPriceAdded != null)
				ProductPriceAdded(this, new ModelAddedEventArgs<ProductPrice>(model));
			int id = model.Id;
			return id;
		}

        public int AddModel(ProductPrice model, int groupId)
        {
            int id;
            var groupRepository = new Repository<Product>(Context);
            Product product = groupRepository.Single(group => group.Id == groupId);
            model.Product = product;
            product.ProductPrices.Add(model);
            Context.Commit();
            if (ProductPriceAdded != null)
                ProductPriceAdded(this, new ModelAddedEventArgs<ProductPrice>(model));
            id = model.Id;
            return id;
        }
		public void UpdateModel(ProductPrice model)
		{
            model.ModifiedBy = LoginInfo.Id;
            model.ModifiedDate = DateTime.Now;
            Context.Commit();
		}

        public void DeleteModel(ProductPrice model)
        {
        }

		public void AttachModel(ProductPrice model)
		{
			if (_productPriceRepository.Exists(ProductPrice => ProductPrice.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

        #endregion

	}
}