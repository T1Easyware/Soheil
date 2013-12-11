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
    public class ReworkDataService : IDataService<Rework>
    {
        #region IDataService<Rework> Members

        public Rework GetSingle(int id)
        {
            Rework entity;
            using (var context = new SoheilEdmContext())
            {
                var reworkRepository = new Repository<Rework>(context);
                entity = reworkRepository.Single(rework => rework.Id == id);
            }
            return entity;
        }

        public ObservableCollection<Rework> GetAll()
        {
            ObservableCollection<Rework> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Rework>(context);
                IEnumerable<Rework> entityList = repository.GetAll();
                models = new ObservableCollection<Rework>(entityList);
            }
            return models;
        }

        public int AddModel(Rework model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Rework>(context);
                repository.Add(model);
                context.Commit();
                if (ReworkAdded != null)
                    ReworkAdded(this, new ModelAddedEventArgs<Rework>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(Rework model)
        {
            using (var context = new SoheilEdmContext())
            {
                var reworkRepository = new Repository<Rework>(context);
                Rework entity = reworkRepository.Single(rework => rework.Id == model.Id);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.CreatedDate = model.CreatedDate;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                entity.Status = model.Status;
                context.Commit();
            }
        }

        public void DeleteModel(Rework model)
        {
        }

        public void AttachModel(Rework model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Rework>(context);
                if (repository.Exists(rework => rework.Id == model.Id))
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

        public event EventHandler<ModelAddedEventArgs<Rework>> ReworkAdded;
        public event EventHandler<ModelAddedEventArgs<ProductRework>> ProductAdded;
        public event EventHandler<ModelRemovedEventArgs> ProductRemoved;

        /// <summary>
        /// Gets all active products as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Rework> GetActives()
        {
            ObservableCollection<Rework> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Rework>(context);
                IEnumerable<Rework> entityList =
                    repository.Find(
                        rework => rework.Status == (decimal)Status.Active);
                models = new ObservableCollection<Rework>(entityList);
            }
            return models;
        }

        /// <summary>
        /// Gets all active products as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Rework> GetActives(SoheilEntityType linkType)
        {
            if (linkType == SoheilEntityType.Products)
            {
                ObservableCollection<Rework> models;
                using (var context = new SoheilEdmContext())
                {
                    var repository = new Repository<Rework>(context);
                    IEnumerable<Rework> entityList =
                        repository.Find(
                            rework => rework.Status == (decimal)Status.Active && rework.ProductReworks.Count == 0);
                    models = new ObservableCollection<Rework>(entityList);
                }
                return models;
            }
            return GetActives();
        }

        public ObservableCollection<ProductRework> GetProducts(int reworkId)
        {
            ObservableCollection<ProductRework> viewModel;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Rework>(context);
                Rework entity = repository.FirstOrDefault(rework => rework.Id == reworkId, "ProductRework.Product", "ProductRework.Rework");
                viewModel = new ObservableCollection<ProductRework>(entity.ProductReworks);
            }

            return viewModel;
        }

        public void AddProduct(int reworkId, int productId, string code, string name, int modifiedBy)
        {
            using (var context = new SoheilEdmContext())
            {
                var reworkRepository = new Repository<Rework>(context);
                var productRepository = new Repository<Product>(context);
                Rework currentRework = reworkRepository.Single(rework => rework.Id == reworkId);
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
        }

        public void RemoveProduct(int reworkId, int productId)
        {
            using (var context = new SoheilEdmContext())
            {
                var reworkRepository = new Repository<Rework>(context);
                var reworkProductRepository = new Repository<ProductRework>(context);
                Rework currentRework = reworkRepository.Single(rework => rework.Id == reworkId);
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
}