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
    public class DefectionDataService : DataServiceBase, IDataService<Defection>
    {
        #region IDataService<Defection> Members

        public Defection GetSingle(int id)
        {
            Defection entity;
            using (var context = new SoheilEdmContext())
            {
                var defectionRepository = new Repository<Defection>(context);
                entity = defectionRepository.Single(defection => defection.Id == id);
            }
            return entity;
        }

        public ObservableCollection<Defection> GetAll()
        {
            ObservableCollection<Defection> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Defection>(context);
                IEnumerable<Defection> entityList =
                    repository.Find(
                        defection => defection.Status != (decimal)Status.Deleted);
                models = new ObservableCollection<Defection>(entityList);
            }
            return models;
        }

        public int AddModel(Defection model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Defection>(context);
                repository.Add(model);
                context.Commit();
                if (DefectionAdded != null)
                    DefectionAdded(this, new ModelAddedEventArgs<Defection>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(Defection model)
        {
            using (var context = new SoheilEdmContext())
            {
                var defectionRepository = new Repository<Defection>(context);
                Defection entity = defectionRepository.Single(defection => defection.Id == model.Id);

                entity.Code = model.Code;
				entity.Name = model.Name;
				entity.IsG2 = model.IsG2;
                entity.CreatedDate = model.CreatedDate;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                context.Commit();
            }
        }

        public void DeleteModel(Defection model)
        {
        }

        public void AttachModel(Defection model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Defection>(context);
                if (repository.Exists(defection => defection.Id == model.Id))
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

        public event EventHandler<ModelAddedEventArgs<Defection>> DefectionAdded;
        public event EventHandler<ModelAddedEventArgs<ProductDefection>> ProductAdded;
        public event EventHandler<ModelRemovedEventArgs> ProductRemoved;

        /// <summary>
        /// Gets all active products as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Defection> GetActives()
        {
            ObservableCollection<Defection> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Defection>(context);
                IEnumerable<Defection> entityList =
                    repository.Find(
                        defection => defection.Status == (decimal) Status.Active);
                models = new ObservableCollection<Defection>(entityList);
            }
            return models;
        }

        /// <summary>
        /// Gets all active products as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Defection> GetActives(SoheilEntityType linkType, int linkId)
        {
            if (linkType == SoheilEntityType.Products)
            {
                ObservableCollection<Defection> models;
                using (var context = new SoheilEdmContext())
                {
                    var repository = new Repository<Defection>(context);
                    IEnumerable<Defection> entityList =
                        repository.Find(
                            defection => defection.Status == (decimal)Status.Active && defection.ProductDefections.All(item=> item.Product.Id != linkId));
                    models = new ObservableCollection<Defection>(entityList);
                }
                return models;
            }
            return GetActives();
        }

        public ObservableCollection<ProductDefection> GetProducts(int defectionId)
        {
            ObservableCollection<ProductDefection> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Defection>(context);
                Defection entity = repository.FirstOrDefault(defection => defection.Id == defectionId, "ProductDefections.Defection", "ProductDefections.Product");
                models = new ObservableCollection<ProductDefection>(entity.ProductDefections.Where(item=>item.Product.Status == (decimal)Status.Active));
            }

            return models;
        }

        public void AddProduct(int defectionId, int productId)
        {
            using (var context = new SoheilEdmContext())
            {
                var defectionRepository = new Repository<Defection>(context);
                var productRepository = new Repository<Product>(context);
                Defection currentDefection = defectionRepository.Single(defection => defection.Id == defectionId);
                Product newProduct = productRepository.Single(product => product.Id == productId);
                if (currentDefection.ProductDefections.Any(defectionProduct => defectionProduct.Defection.Id == defectionId && defectionProduct.Product.Id == productId))
                {
                    return;
                }
                var newProductDefection = new ProductDefection { Defection = currentDefection, Product = newProduct };
                currentDefection.ProductDefections.Add(newProductDefection);
                context.Commit();
                ProductAdded(this, new ModelAddedEventArgs<ProductDefection>(newProductDefection));
            }
        }

        public void RemoveProduct(int defectionId, int productId)
        {
            using (var context = new SoheilEdmContext())
            {
                var defectionRepository = new Repository<Defection>(context);
                var defectionProductRepository = new Repository<ProductDefection>(context);
                Defection currentDefection = defectionRepository.Single(defection => defection.Id == defectionId);
                ProductDefection currentDefectionProduct =
                    currentDefection.ProductDefections.First(
                        defectionProduct =>
                        defectionProduct.Defection.Id == defectionId && defectionProduct.Id == productId);
                int id = currentDefectionProduct.Id;
                defectionProductRepository.Delete(currentDefectionProduct);
                context.Commit();
                ProductRemoved(this, new ModelRemovedEventArgs(id));
            }
        }
    }
}