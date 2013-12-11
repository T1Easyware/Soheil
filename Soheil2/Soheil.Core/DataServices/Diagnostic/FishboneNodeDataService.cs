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
    public class FishboneNodeDataService : IDataService<FishboneNode>
    {
        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public FishboneNode GetSingle(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a list of view models representing all records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<FishboneNode> GetAll()
        {
            ObservableCollection<FishboneNode> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<FishboneNode>(context);
                IEnumerable<FishboneNode> entityList = repository.GetAll("FishboneNode_ActionPlans", "Root", "Parent","Children");
                models = new ObservableCollection<FishboneNode>(entityList);
            }
            return models;
        }

        /// <summary>
        /// Gets a list of view models representing currently active records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<FishboneNode> GetActives()
        {
            return GetAll();
        }

        public int AddModel(FishboneNode model)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateModel(FishboneNode model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<FishboneNode>(context);
                FishboneNode entity = repository.Single(actionPlanFishboneNode => actionPlanFishboneNode.Id == model.Id);
                entity.Description = model.Description;
                context.Commit();
                if (ModelUpdated != null) ModelUpdated(this, new ModelAddedEventArgs<FishboneNode>(entity));
            }
        }

        public void DeleteModel(FishboneNode model)
        {
            throw new System.NotImplementedException();
        }

        public void AttachModel(FishboneNode model)
        {
            throw new System.NotImplementedException();
        }

        public event EventHandler<ModelAddedEventArgs<FishboneNode>> ModelUpdated;
        public event EventHandler<ModelAddedEventArgs<FishboneNode>> FishboneNodeAdded;
        public event EventHandler<ModelAddedEventArgs<FishboneNode_ActionPlan>> ActionPlanAdded;
        public event EventHandler<ModelRemovedEventArgs> ActionPlanRemoved;


        public ObservableCollection<FishboneNode_ActionPlan> GetActionPlans(int fishboneId)
        {
            ObservableCollection<FishboneNode_ActionPlan> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<FishboneNode>(context);
                FishboneNode entity = repository.FirstOrDefault(fishbone => fishbone.Id == fishboneId, "FishboneNode_ActionPlans.FishboneNode", "FishboneNode_ActionPlans.ActionPlan");
                models = new ObservableCollection<FishboneNode_ActionPlan>(entity.FishboneNode_ActionPlans.Where(item=>item.ActionPlan.Status ==(decimal)Status.Active));
            }

            return models;
        }

        public void AddActionPlan(int fishboneId, int actionPlanId)
        {
            using (var context = new SoheilEdmContext())
            {
                var fishboneRepository = new Repository<FishboneNode>(context);
                var actionPlanRepository = new Repository<ActionPlan>(context);
                FishboneNode currentFishboneNode = fishboneRepository.Single(fishbone => fishbone.Id == fishboneId);
                ActionPlan newActionPlan = actionPlanRepository.Single(actionPlan => actionPlan.Id == actionPlanId);
                if (currentFishboneNode.FishboneNode_ActionPlans.Any(fishboneActionPlan => fishboneActionPlan.FishboneNode.Id == fishboneId && fishboneActionPlan.ActionPlan.Id == actionPlanId))
                {
                    return;
                }
                var newActionPlanFishboneNode = new FishboneNode_ActionPlan { FishboneNode = currentFishboneNode, ActionPlan = newActionPlan };
                currentFishboneNode.FishboneNode_ActionPlans.Add(newActionPlanFishboneNode);
                context.Commit();
                ActionPlanAdded(this, new ModelAddedEventArgs<FishboneNode_ActionPlan>(newActionPlanFishboneNode));
            }
        }

        public void RemoveActionPlan(int fishboneId, int actionPlanId)
        {
            using (var context = new SoheilEdmContext())
            {
                var fishboneRepository = new Repository<FishboneNode>(context);
                var fishboneActionPlanRepository = new Repository<FishboneNode_ActionPlan>(context);
                FishboneNode currentFishboneNode = fishboneRepository.Single(fishbone => fishbone.Id == fishboneId);
                FishboneNode_ActionPlan currentFishboneNodeActionPlan =
                    currentFishboneNode.FishboneNode_ActionPlans.First(
                        fishboneActionPlan =>
                        fishboneActionPlan.FishboneNode.Id == fishboneId && fishboneActionPlan.ActionPlan.Id == actionPlanId);
                fishboneActionPlanRepository.Delete(currentFishboneNodeActionPlan);
                context.Commit();
                ActionPlanRemoved(this, new ModelRemovedEventArgs(actionPlanId));
            }
        }
    }
}