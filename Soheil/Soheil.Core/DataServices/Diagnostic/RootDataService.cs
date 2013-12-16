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
    public class RootDataService : DataServiceBase, IDataService<Root>
    {
        #region IDataService<Root> Members

        public Root GetSingle(int id)
        {
            Root entity;
            using (var context = new SoheilEdmContext())
            {
                var rootRepository = new Repository<Root>(context);
                entity = rootRepository.Single(root => root.Id == id);
            }
            return entity;
        }

        public ObservableCollection<Root> GetAll()
        {
            ObservableCollection<Root> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Root>(context);
                IEnumerable<Root> entityList =
                    repository.Find(
                        root => root.Status != (decimal)Status.Deleted, "ProductDefection", "ProductDefection.Product", "ProductDefection.Defection");
                models = new ObservableCollection<Root>(entityList);
            }
            return models;
        }

        public int AddModel(Root model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Root>(context);
                repository.Add(model);
                context.Commit();
                if (RootAdded != null)
                    RootAdded(this, new ModelAddedEventArgs<Root>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(Root model)
        {
            using (var context = new SoheilEdmContext())
            {
                var rootRepository = new Repository<Root>(context);
                Root entity = rootRepository.Single(root => root.Id == model.Id);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.CreatedDate = model.CreatedDate;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                context.Commit();
            }
        }

        public void DeleteModel(Root model)
        {
        }

        public void AttachModel(Root model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Root>(context);
                if (repository.Exists(root => root.Id == model.Id))
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

        public event EventHandler<ModelAddedEventArgs<Root>> RootAdded;
        public event EventHandler<ModelAddedEventArgs<FishboneNode>> FishboneAdded;
        public event EventHandler<ModelRemovedEventArgs> FishboneRemoved;

        /// <summary>
        /// Gets all active defections as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Root> GetActives()
        {
            ObservableCollection<Root> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Root>(context);
                IEnumerable<Root> entityList =
                    repository.Find(
                        root => root.Status == (decimal)Status.Active, "ProductDefection", "ProductDefection.Product", "ProductDefection.Defection");
                models = new ObservableCollection<Root>(entityList);
            }
            return models;
        }

        /// <summary>
        /// Gets all active defections as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Root> GetActives(SoheilEntityType linkType)
        {
            if (linkType == SoheilEntityType.ActionPlans)
            {
                ObservableCollection<Root> models;
                using (var context = new SoheilEdmContext())
                {
                    var repository = new Repository<Root>(context);
                    IEnumerable<Root> entityList =
                        repository.Find(
                            root => root.Status == (decimal)Status.Active && root.FishboneNodes.Count == 0, "ProductDefection", "ProductDefection.Product", "ProductDefection.Defection");
                    models = new ObservableCollection<Root>(entityList);
                }
                return models;
            }
            return GetActives();
        }

        public ObservableCollection<FishboneNode> GetFishboneNodes(int rootId)
        {
            ObservableCollection<FishboneNode> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Root>(context);
                Root entity = repository.FirstOrDefault(root => root.Id == rootId, "FishboneNodes.Children", "FishboneNodes.Parent");
                models = new ObservableCollection<FishboneNode>(entity.FishboneNodes);
            }

            return models;
        }

        public void AddFishboneNode(int rootId, FishboneNodeType rootType, int parentId, string description, FishboneNodeType nodeType)
        {
            using (var context = new SoheilEdmContext())
            {
                var rootRepository = new Repository<Root>(context);
                var fishboneRepository = new Repository<FishboneNode>(context);
                Root currentRoot = rootRepository.Single(root => root.Id == rootId);
                //if (currentRoot.FishboneNodes.Any(rootDefection => rootDefection.Root.Id == rootId))
                //{
                //    return;
                //}
                var parent = fishboneRepository.FirstOrDefault(fishbone => fishbone.Id == parentId);
                var newFishboneNode = new FishboneNode { 
                    Root = currentRoot, 
                    RootType = (byte) rootType,
                    Parent = parent,
                    Description = description,
                    Type = (byte) nodeType,
                    ModifiedDate = DateTime.Now};
                currentRoot.FishboneNodes.Add(newFishboneNode);
                context.Commit();
                FishboneAdded(this, new ModelAddedEventArgs<FishboneNode>(newFishboneNode));
            }
        }

        public void InitializeFishboneRoots(int rootId, int productDefectionId)
        {
            using (var context = new SoheilEdmContext())
            {
                var rootRepository = new Repository<Root>(context);
                var productDefectionRepository = new Repository<ProductDefection>(context);
                Root currentRoot = rootRepository.Single(root => root.Id == rootId);
                currentRoot.ProductDefection = productDefectionRepository.FirstOrDefault(entity => entity.Id == productDefectionId);

                if (currentRoot.FishboneNodes.Any(rootDefection => rootDefection.Root.Id == rootId))
                {
                    return;
                }

                
                var rootNode = new FishboneNode
                {
                    Root = currentRoot,
                    RootType = (byte)FishboneNodeType.Root,
                    Parent = null,
                    Description = "",
                    Type = (byte)FishboneNodeType.Root,
                    ModifiedDate = DateTime.Now
                };
                currentRoot.FishboneNodes.Add(rootNode);

                var manNode = new FishboneNode
                {
                    Root = currentRoot,
                    RootType = (byte)FishboneNodeType.Man,
                    Parent = rootNode,
                    Description = Common.Properties.Resources.txtMan,
                    Type = (byte)FishboneNodeType.Man,
                    ModifiedDate = DateTime.Now
                };
                rootNode.Children.Add(manNode);

                var methodNode = new FishboneNode
                {
                    Root = currentRoot,
                    RootType = (byte)FishboneNodeType.Method,
                    Parent = rootNode,
                    Description = Common.Properties.Resources.txtMethod,
                    Type = (byte)FishboneNodeType.Method,
                    ModifiedDate = DateTime.Now
                };
                rootNode.Children.Add(methodNode);

                var materialNode = new FishboneNode
                {
                    Root = currentRoot,
                    RootType = (byte)FishboneNodeType.Material,
                    Parent = rootNode,
                    Description = Common.Properties.Resources.txtMaterial,
                    Type = (byte)FishboneNodeType.Material,
                    ModifiedDate = DateTime.Now
                };
                rootNode.Children.Add(materialNode);

                var machinesNode = new FishboneNode
                {
                    Root = currentRoot,
                    RootType = (byte)FishboneNodeType.Machines,
                    Parent = rootNode,
                    Description = Common.Properties.Resources.txtMachine,
                    Type = (byte)FishboneNodeType.Machines,
                    ModifiedDate = DateTime.Now
                };
                rootNode.Children.Add(machinesNode);

                var maintenanceNode = new FishboneNode
                {
                    Root = currentRoot,
                    RootType = (byte)FishboneNodeType.Maintenance,
                    Parent = rootNode,
                    Description = Common.Properties.Resources.txtMaintenance,
                    Type = (byte)FishboneNodeType.Maintenance,
                    ModifiedDate = DateTime.Now
                };
                rootNode.Children.Add(maintenanceNode);

                context.Commit();
                FishboneAdded(this, new ModelAddedEventArgs<FishboneNode>(rootNode));
            }
        }

        public void RemoveFishboneNode(int rootId, int fishboneId)
        {
            using (var context = new SoheilEdmContext())
            {
                var rootRepository = new Repository<Root>(context);
                var fishboneRepository = new Repository<FishboneNode>(context);
                Root currentRoot = rootRepository.Single(root => root.Id == rootId);
                FishboneNode currentFishbone = currentRoot.FishboneNodes.First(fishboneNode => fishboneNode.Id == fishboneId);
                fishboneRepository.Delete(currentFishbone);
                context.Commit();
                FishboneRemoved(this, new ModelRemovedEventArgs(fishboneId));
            }
        }
    }
}