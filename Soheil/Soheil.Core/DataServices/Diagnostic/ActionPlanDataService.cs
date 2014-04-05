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
    public class ActionPlanDataService : DataServiceBase, IDataService<ActionPlan>
    {
        #region IDataService<ActionPlan> Members

        public ActionPlan GetSingle(int id)
        {
            ActionPlan entity;
            using (var context = new SoheilEdmContext())
            {
                var actionPlanRepository = new Repository<ActionPlan>(context);
                entity = actionPlanRepository.Single(actionPlan => actionPlan.Id == id);
            }
            return entity;
        }

        public ObservableCollection<ActionPlan> GetAll()
        {
            ObservableCollection<ActionPlan> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ActionPlan>(context);
                IEnumerable<ActionPlan> entityList =
                    repository.Find(
                        actionPlan => actionPlan.Status != (decimal)Status.Deleted);
                models = new ObservableCollection<ActionPlan>(entityList);
            }
            return models;
        }

        public int AddModel(ActionPlan model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ActionPlan>(context);
                repository.Add(model);
                context.Commit();
                if (ActionPlanAdded != null)
                    ActionPlanAdded(this, new ModelAddedEventArgs<ActionPlan>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(ActionPlan model)
        {
            using (var context = new SoheilEdmContext())
            {
                var actionPlanRepository = new Repository<ActionPlan>(context);
                ActionPlan entity = actionPlanRepository.Single(actionPlan => actionPlan.Id == model.Id);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.CreatedDate = model.CreatedDate;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                context.Commit();
            }
        }

        public void DeleteModel(ActionPlan model)
        {
        }

        public void AttachModel(ActionPlan model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ActionPlan>(context);
                if (repository.Exists(actionPlan => actionPlan.Id == model.Id))
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

        public event EventHandler<ModelAddedEventArgs<ActionPlan>> ActionPlanAdded;
        public event EventHandler<ModelAddedEventArgs<FishboneNode_ActionPlan>> FishboneNodeAdded;
        public event EventHandler<ModelRemovedEventArgs> FishboneNodeRemoved;

        /// <summary>
        /// Gets all active roots as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<ActionPlan> GetActives()
        {
            ObservableCollection<ActionPlan> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ActionPlan>(context);
                IEnumerable<ActionPlan> entityList =
                    repository.Find(
                        actionPlan => actionPlan.Status == (decimal)Status.Active);
                models = new ObservableCollection<ActionPlan>(entityList);
            }
            return models;
        }

        /// <summary>
        /// Gets all active roots as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<ActionPlan> GetActives(SoheilEntityType linkType, int linkId = 0)
        {
            if (linkType == SoheilEntityType.Roots)
            {
                ObservableCollection<ActionPlan> models;
                using (var context = new SoheilEdmContext())
                {
                    var repository = new Repository<ActionPlan>(context);
                    IEnumerable<ActionPlan> entityList =
                        repository.Find(
                            actionPlan => actionPlan.Status == (decimal)Status.Active && actionPlan.FishboneNode_ActionPlan.Count == 0);
                    models = new ObservableCollection<ActionPlan>(entityList);
                }
                return models;
            }
            return GetActives();
        }

        public ObservableCollection<FishboneNode_ActionPlan> GetFishboneNodes(int actionPlanId)
        {
            ObservableCollection<FishboneNode_ActionPlan> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<ActionPlan>(context);
                ActionPlan entity = repository.FirstOrDefault(actionPlan => actionPlan.Id == actionPlanId && actionPlan.Status == (decimal) Status.Active, "FishboneNode_ActionPlan", "FishboneNode_ActionPlan.FishboneNode", "FishboneNode_ActionPlan.ActionPlan");
                models = new ObservableCollection<FishboneNode_ActionPlan>(entity.FishboneNode_ActionPlan);
            }

            return models;
        }

        public void AddFishboneNode(int actionPlanId, int rootId)
        {
            using (var context = new SoheilEdmContext())
            {
                var actionPlanRepository = new Repository<ActionPlan>(context);
                var fishboneRepository = new Repository<FishboneNode>(context);
                ActionPlan currentActionPlan = actionPlanRepository.Single(actionPlan => actionPlan.Id == actionPlanId);
                FishboneNode newFishbone = fishboneRepository.Single(root => root.Id == rootId);
                if (currentActionPlan.FishboneNode_ActionPlan.Any(actionPlanRoot => actionPlanRoot.ActionPlan.Id == actionPlanId && actionPlanRoot.FishboneNode.Id == rootId))
                {
                    return;
                }
                var newFishboneActionPlan = new FishboneNode_ActionPlan { FishboneNode = newFishbone, ActionPlan = currentActionPlan };
                currentActionPlan.FishboneNode_ActionPlan.Add(newFishboneActionPlan);
                context.Commit();
                FishboneNodeAdded(this, new ModelAddedEventArgs<FishboneNode_ActionPlan>(newFishboneActionPlan));
            }
        }

        public void RemoveFishboneNode(int actionPlanId, int rootId)
        {
            using (var context = new SoheilEdmContext())
            {
                var actionPlanRepository = new Repository<ActionPlan>(context);
                var fishboneActionPlanRepository = new Repository<FishboneNode_ActionPlan>(context);
                ActionPlan currentActionPlan = actionPlanRepository.Single(actionPlan => actionPlan.Id == actionPlanId);
                FishboneNode_ActionPlan currentFishboneActionPlan =
                    currentActionPlan.FishboneNode_ActionPlan.First(
                        actionPlanRoot =>
                        actionPlanRoot.ActionPlan.Id == actionPlanId && actionPlanRoot.FishboneNode.Id == rootId);
                fishboneActionPlanRepository.Delete(currentFishboneActionPlan);
                context.Commit();
                FishboneNodeRemoved(this, new ModelRemovedEventArgs(rootId));
            }
        }
    }
}