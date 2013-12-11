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

        #region IDataService<Product> Members

        public Product GetSingle(int id)
        {
            Product entity;
            using (var context = new SoheilEdmContext())
            {
                var productRepository = new Repository<Product>(context);
                entity = productRepository.Single(product => product.Id == id);
            }
            return entity;
        }

        public ObservableCollection<Product> GetAll()
        {
            ObservableCollection<Product> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Product>(context);
                IEnumerable<Product> entityList =
    repository.Find(
        product => product.Status != (decimal)Status.Deleted, "ProductGroup");
                models = new ObservableCollection<Product>(entityList);
            }
            return models;
        }

        public int AddModel(Product model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var groupRepository = new Repository<ProductGroup>(context);
                ProductGroup productGroup = groupRepository.Single(group => group.Id == model.ProductGroup.Id);
                productGroup.Products.Add(model);
                context.Commit();
                if (ProductAdded != null)
                    ProductAdded(this, new ModelAddedEventArgs<Product>(model));
                id = model.Id;
            }
            return id;
        }

        public int AddModel(Product model, int groupId)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var groupRepository = new Repository<ProductGroup>(context);
                ProductGroup productGroup = groupRepository.Single(group => group.Id == groupId);
                model.ProductGroup = productGroup;
                productGroup.Products.Add(model);
                context.Commit();
                if (ProductAdded != null)
                    ProductAdded(this, new ModelAddedEventArgs<Product>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(Product model)
        {
            using (var context = new SoheilEdmContext())
            {
                var productRepository = new Repository<Product>(context);
                var productGroupRepository = new Repository<ProductGroup>(context);
                Product entity = productRepository.Single(product => product.Id == model.Id);
                ProductGroup group =
                    productGroupRepository.Single(productGroup => productGroup.Id == model.ProductGroup.Id);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.CreatedDate = model.CreatedDate;
                entity.Color = model.Color;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                entity.Status = model.Status;

                entity.ProductGroup = group;
                //context.AttachTo(context.CreateObjectSet<Product>().EntitySet.Name, model);
                //context.ObjectStateManager.ChangeObjectState(model, EntityState.Modified);
                context.Commit();
            }
        }

        public void UpdateModel(Product model, int groupId)
        {
            using (var context = new SoheilEdmContext())
            {
                var productRepository = new Repository<Product>(context);
                var productGroupRepository = new Repository<ProductGroup>(context);
                Product entity = productRepository.Single(product => product.Id == model.Id);
                ProductGroup group =
                    productGroupRepository.Single(productGroup => productGroup.Id == groupId);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.CreatedDate = model.CreatedDate;
                entity.Color = model.Color;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                entity.Status = model.Status;

                entity.ProductGroup = group;
                context.Commit();
            }
        }

        public void DeleteModel(Product model)
        {
        }

        public void AttachModel(Product model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Product>(context);
                if (repository.Exists(product => product.Id == model.Id))
                {
                    UpdateModel(model);
                }
                else
                {
                    AddModel(model);
                }
            }
        }

        public void AttachModel(Product model, int groupId)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Product>(context);
                if (repository.Exists(product => product.Id == model.Id))
                {
                    UpdateModel(model, groupId);
                }
                else
                {
                    AddModel(model, groupId);
                }
            }
        }

        #endregion

        public ObservableCollection<Product> GetActives()
        {
            ObservableCollection<Product> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Product>(context);
                IEnumerable<Product> entityList =
                    repository.Find(
                        product => product.Status == (decimal) Status.Active,"ProductGroup");
                models = new ObservableCollection<Product>(entityList);
            }
            return models;
        }
        public ObservableCollection<Product> GetActives(SoheilEntityType linkType)
        {
            if (linkType == SoheilEntityType.Defections)
            {
                ObservableCollection<Product> models;
                using (var context = new SoheilEdmContext())
                {
                    var repository = new Repository<Product>(context);
                    IEnumerable<Product> entityList =
                        repository.Find(
                            product => product.Status == (decimal)Status.Active && product.ProductDefections.Count == 0, "ProductGroup");
                    models = new ObservableCollection<Product>(entityList);
                }
                return models;
            }
            if (linkType == SoheilEntityType.Reworks)
            {
                ObservableCollection<Product> models;
                using (var context = new SoheilEdmContext())
                {
                    var repository = new Repository<Product>(context);
                    IEnumerable<Product> entityList =
                        repository.Find(
                            product => product.Status == (decimal)Status.Active && product.ProductReworks.Count == 0, "ProductGroup");
                    models = new ObservableCollection<Product>(entityList);
                }
                return models;
            }
            return GetActives();
        }

        public ObservableCollection<ProductDefection> GetDefections(int productId)
        {
            ObservableCollection<ProductDefection> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Product>(context);
                Product entity = repository.FirstOrDefault(product => product.Id == productId, "ProductDefection.Defection", "ProductDefection.Product");
                models = new ObservableCollection<ProductDefection>(entity.ProductDefections.Where(item=>item.Defection.Status == (decimal)Status.Active));
            }

            return models;
        }

        public void AddDefection(int productId, int defectionId)
        {
            using (var context = new SoheilEdmContext())
            {
                var productRepository = new Repository<Product>(context);
                var defectionRepository = new Repository<Defection>(context);
                Product currentProduct = productRepository.Single(product => product.Id == productId);
                Defection newDefection = defectionRepository.Single(defection => defection.Id == defectionId);
                if (currentProduct.ProductDefections.Any(productDefection => productDefection.Product.Id == productId && productDefection.Defection.Id == defectionId))
                {
                    return;
                }
                var newProductDefection = new ProductDefection { Defection = newDefection, Product = currentProduct };
                currentProduct.ProductDefections.Add(newProductDefection);
                context.Commit();
                DefectionAdded(this, new ModelAddedEventArgs<ProductDefection>(newProductDefection));
            }
        }

        public void RemoveDefection(int productId, int defectionId)
        {
            using (var context = new SoheilEdmContext())
            {
                var productRepository = new Repository<Product>(context);
                var productDefectionRepository = new Repository<ProductDefection>(context);
                Product currentProduct = productRepository.Single(product => product.Id == productId);
                ProductDefection currentProductDefection =
                    currentProduct.ProductDefections.First(
                        productDefection =>
                        productDefection.Product.Id == productId && productDefection.Id == defectionId);
                int id = currentProductDefection.Id;
                productDefectionRepository.Delete(currentProductDefection);
                context.Commit();
                DefectionRemoved(this, new ModelRemovedEventArgs(id));
            }
        }

        public ObservableCollection<ProductRework> GetReworks(int productId)
        {
            ObservableCollection<ProductRework> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Product>(context);
                Product entity = repository.FirstOrDefault(product => product.Id == productId, "ProductRework.Product", "ProductRework.Rework");
                models = new ObservableCollection<ProductRework>(entity.ProductReworks.Where(item=>item.Rework != null && item.Rework.Status == (decimal)Status.Active));
            }

            return models;
        }

        public void AddRework(int productId, int reworkId, string code, string name, int modifiedBy)
        {
            using (var context = new SoheilEdmContext())
            {
                var productRepository = new Repository<Product>(context);
                var reworkRepository = new Repository<Rework>(context);
                Product currentProduct = productRepository.Single(product => product.Id == productId);
                Rework newRework = reworkRepository.Single(rework => rework.Id == reworkId);
                if (currentProduct.ProductReworks.Any(productRework => productRework.Product.Id == productId && productRework.Rework.Id == reworkId))
                {
                    return;
                }
                var newProductRework = new ProductRework { Rework = newRework, Product = currentProduct, Code = code, Name = name, ModifiedBy = modifiedBy};
                currentProduct.ProductReworks.Add(newProductRework);
                context.Commit();
                ReworkAdded(this, new ModelAddedEventArgs<ProductRework>(newProductRework));
            }
        }

        public void RemoveRework(int productId, int reworkId)
        {
            using (var context = new SoheilEdmContext())
            {
                var productRepository = new Repository<Product>(context);
                var productReworkRepository = new Repository<ProductRework>(context);
                Product currentProduct = productRepository.Single(product => product.Id == productId);
                ProductRework currentProductRework =
                    currentProduct.ProductReworks.First(
                        productRework =>
                        productRework.Product.Id == productId && productRework.Id == reworkId);
                int id = currentProductRework.Id;
                productReworkRepository.Delete(currentProductRework);
                context.Commit();
                ReworkRemoved(this, new ModelRemovedEventArgs(id));
            }
        }

    }
}