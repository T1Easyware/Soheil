using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Configuration;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class DefectionDataService : DataServiceBase, IDataService<Defection>
    {
        #region IDataService<Defection> Members

        public Defection GetSingle(int id)
        {
                return _defectionRepository.Single(defection => defection.Id == id);
        }

        public ObservableCollection<Defection> GetAll()
        {
                IEnumerable<Defection> entityList =
                    _defectionRepository.Find(
                        defection => defection.Status != (decimal)Status.Deleted);
                return new ObservableCollection<Defection>(entityList);
        }

        public int AddModel(Defection model)
        {
                _defectionRepository.Add(model);
                Context.Commit();
                if (DefectionAdded != null)
                    DefectionAdded(this, new ModelAddedEventArgs<Defection>(model));
                return model.Id;
        }

        public void UpdateModel(Defection model)
        {
            model.ModifiedBy = LoginInfo.Id;
            model.ModifiedDate = DateTime.Now;
                Context.Commit();
        }

        public void DeleteModel(Defection model)
        {
        }

        public void AttachModel(Defection model)
        {
                if (_defectionRepository.Exists(defection => defection.Id == model.Id))
                {
                    UpdateModel(model);
                }
                else
                {
                    AddModel(model);
                }
        }

        #endregion

        public event EventHandler<ModelAddedEventArgs<Defection>> DefectionAdded;
        public event EventHandler<ModelAddedEventArgs<ProductDefection>> ProductAdded;
        public event EventHandler<ModelRemovedEventArgs> ProductRemoved;

        private readonly Repository<Defection> _defectionRepository;
        private readonly Repository<ProductDefection> _defectionProductRepository;
        private readonly Repository<Product> _productRepository; 
        public DefectionDataService(SoheilEdmContext context)
        {
            Context = context;
            _defectionRepository = new Repository<Defection>(context);
            _productRepository = new Repository<Product>(context);
            _defectionProductRepository = new Repository<ProductDefection>(context);
        }
        /// <summary>
        /// Gets all active products as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Defection> GetActives()
        {
                IEnumerable<Defection> entityList =
                    _defectionRepository.Find(
                        defection => defection.Status == (decimal) Status.Active);
                return new ObservableCollection<Defection>(entityList);
        }

        /// <summary>
        /// Gets all active products as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Defection> GetActives(SoheilEntityType linkType, int linkId)
        {
            if (linkType == SoheilEntityType.Products)
            {
                    IEnumerable<Defection> entityList =
                        _defectionRepository.Find(
                            defection => (defection.Status == (decimal)Status.Active) && defection.ProductDefections.All(item=> item.Product.Id != linkId));
                    return new ObservableCollection<Defection>(entityList);
            }
            return GetActives();
        }

        public ObservableCollection<ProductDefection> GetProducts(int defectionId)
        {
                Defection entity = _defectionRepository.FirstOrDefault(defection => defection.Id == defectionId, "ProductDefections.Defection", "ProductDefections.Product");
                return new ObservableCollection<ProductDefection>(entity.ProductDefections.Where(item=>item.Product.Status == (decimal)Status.Active));
        }

        public void AddProduct(int defectionId, int productId)
        {
            Defection currentDefection = _defectionRepository.Single(defection => defection.Id == defectionId);
            Product newProduct = _productRepository.Single(product => product.Id == productId);
            if (
                currentDefection.ProductDefections.Any(
                    defectionProduct =>
                        defectionProduct.Defection.Id == defectionId && defectionProduct.Product.Id == productId))
            {
                return;
            }
            var newProductDefection = new ProductDefection {Defection = currentDefection, Product = newProduct};
            currentDefection.ProductDefections.Add(newProductDefection);
            Context.Commit();
            ProductAdded(this, new ModelAddedEventArgs<ProductDefection>(newProductDefection));
        }

        public void RemoveProduct(int defectionId, int productId)
        {
                Defection currentDefection = _defectionRepository.Single(defection => defection.Id == defectionId);
                ProductDefection currentDefectionProduct =
                    currentDefection.ProductDefections.First(
                        defectionProduct =>
                        defectionProduct.Defection.Id == defectionId && defectionProduct.Id == productId);
                int id = currentDefectionProduct.Id;
                _defectionProductRepository.Delete(currentDefectionProduct);
                Context.Commit();
                ProductRemoved(this, new ModelRemovedEventArgs(id));
        }
    }
}