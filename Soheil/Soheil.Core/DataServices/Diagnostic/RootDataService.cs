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
        private readonly Repository<ProductDefection> _productDefectionRepository;
        private readonly Repository<Root> _rootRepository;
        private readonly Repository<FishboneNode> _fishboneRepository; 
        public RootDataService(SoheilEdmContext context)
        {
            Context = context;
            _fishboneRepository = new Repository<FishboneNode>(context);
            _productDefectionRepository = new Repository<ProductDefection>(context);
            _rootRepository = new Repository<Root>(context);
        }

        #region IDataService<Root> Members

        public Root GetSingle(int id)
        {
                return _rootRepository.Single(root => root.Id == id);
        }

        public ObservableCollection<Root> GetAll()
        {
                IEnumerable<Root> entityList =
                    _rootRepository.Find(
                        root => root.Status != (decimal)Status.Deleted, "ProductDefection", "ProductDefection.Product", "ProductDefection.Defection");
                return new ObservableCollection<Root>(entityList);
        }

        public int AddModel(Root model)
        {
                _rootRepository.Add(model);
                Context.Commit();
                if (RootAdded != null)
                    RootAdded(this, new ModelAddedEventArgs<Root>(model));
                return model.Id;
        }

        public void UpdateModel(Root model)
        {
            model.ModifiedBy = LoginInfo.Id;
            model.ModifiedDate = DateTime.Now;
                Context.Commit();
        }

        public void DeleteModel(Root model)
        {
        }

        public void AttachModel(Root model)
        {
                if (_rootRepository.Exists(root => root.Id == model.Id))
                {
                    UpdateModel(model);
                }
                else
                {
                    AddModel(model);
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
                IEnumerable<Root> entityList =
                    _rootRepository.Find(
                        root => root.Status == (decimal)Status.Active, "ProductDefection", "ProductDefection.Product", "ProductDefection.Defection");
                return  new ObservableCollection<Root>(entityList);
        }

        /// <summary>
        /// Gets all active defections as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Root> GetActives(SoheilEntityType linkType)
        {
            if (linkType == SoheilEntityType.ActionPlans)
            {
                    IEnumerable<Root> entityList =
                        _rootRepository.Find(
                            root => root.Status == (decimal)Status.Active, "ProductDefection", "ProductDefection.Product", "ProductDefection.Defection");
                    return new ObservableCollection<Root>(entityList);
            }
            return GetActives();
        }

        public ObservableCollection<FishboneNode> GetFishboneNodes(int rootId)
        {
                Root entity = _rootRepository.FirstOrDefault(root => root.Id == rootId, "FishboneNodes.Children", "FishboneNodes.Parent");
                return new ObservableCollection<FishboneNode>(entity.FishboneNodes);
        }

        public void AddFishboneNode(int rootId, FishboneNodeType rootType, int parentId, string description, FishboneNodeType nodeType)
        {
                Root currentRoot = _rootRepository.Single(root => root.Id == rootId);
                //if (currentRoot.FishboneNodes.Any(rootDefection => rootDefection.Root.Id == rootId))
                //{
                //    return;
                //}
                var parent = _fishboneRepository.FirstOrDefault(fishbone => fishbone.Id == parentId);
                var newFishboneNode = new FishboneNode { 
                    Root = currentRoot, 
                    RootType = (byte) rootType,
                    Parent = parent,
                    Description = description,
                    Type = (byte) nodeType,
                    ModifiedDate = DateTime.Now};
                currentRoot.FishboneNodes.Add(newFishboneNode);
                Context.Commit();
                FishboneAdded(this, new ModelAddedEventArgs<FishboneNode>(newFishboneNode));
        }

        public void InitializeFishboneRoots(int rootId, int productDefectionId)
        {
                Root currentRoot = _rootRepository.Single(root => root.Id == rootId);
                currentRoot.ProductDefection = _productDefectionRepository.FirstOrDefault(entity => entity.Id == productDefectionId);

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

                Context.Commit();
                FishboneAdded(this, new ModelAddedEventArgs<FishboneNode>(rootNode));
            
        }

        public void RemoveFishboneNode(int rootId, int fishboneId)
        {
                Root currentRoot = _rootRepository.Single(root => root.Id == rootId);
                FishboneNode currentFishbone = currentRoot.FishboneNodes.First(fishboneNode => fishboneNode.Id == fishboneId);
                _fishboneRepository.Delete(currentFishbone);
                Context.Commit();
                FishboneRemoved(this, new ModelRemovedEventArgs(fishboneId));
        }
    }
}