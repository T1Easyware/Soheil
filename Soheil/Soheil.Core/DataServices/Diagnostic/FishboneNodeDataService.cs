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
    public class FishboneNodeDataService : DataServiceBase, IDataService<FishboneNode>
    {

        private readonly Repository<ActionPlan> _actionPlanRepository;
        private readonly Repository<FishboneNode_ActionPlan> _fishboneActionplanRepository;
        private readonly Repository<FishboneNode> _fishboneRepository;

        public FishboneNodeDataService(SoheilEdmContext context)
        {
            Context = context;
            _actionPlanRepository = new Repository<ActionPlan>(context);
            _fishboneActionplanRepository = new Repository<FishboneNode_ActionPlan>(context);
            _fishboneRepository = new Repository<FishboneNode>(context);
        }
        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public FishboneNode GetSingle(int id)
        {
            return _fishboneRepository.Single(node => node.Id == id);
        }

        /// <summary>
        /// Gets a list of view models representing all records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<FishboneNode> GetAll()
        {

                IEnumerable<FishboneNode> entityList = _fishboneRepository.GetAll("FishboneNode_ActionPlans", "Root", "Parent","Children");
                return new ObservableCollection<FishboneNode>(entityList);
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
                FishboneNode entity = _fishboneRepository.Single(actionPlanFishboneNode => actionPlanFishboneNode.Id == model.Id);
                entity.Description = model.Description;
                Context.Commit();
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
                FishboneNode entity = _fishboneRepository.FirstOrDefault(fishbone => fishbone.Id == fishboneId, "FishboneNode_ActionPlans.FishboneNode", "FishboneNode_ActionPlans.ActionPlan");
                return new ObservableCollection<FishboneNode_ActionPlan>(entity.FishboneNode_ActionPlans.Where(item=>item.ActionPlan.Status ==(decimal)Status.Active));
        }

        public void AddActionPlan(int fishboneId, int actionPlanId)
        {
                FishboneNode currentFishboneNode = _fishboneRepository.Single(fishbone => fishbone.Id == fishboneId);
                ActionPlan newActionPlan = _actionPlanRepository.Single(actionPlan => actionPlan.Id == actionPlanId);
                if (currentFishboneNode.FishboneNode_ActionPlans.Any(fishboneActionPlan => fishboneActionPlan.FishboneNode.Id == fishboneId && fishboneActionPlan.ActionPlan.Id == actionPlanId))
                {
                    return;
                }
                var newActionPlanFishboneNode = new FishboneNode_ActionPlan { FishboneNode = currentFishboneNode, ActionPlan = newActionPlan };
                currentFishboneNode.FishboneNode_ActionPlans.Add(newActionPlanFishboneNode);
                Context.Commit();
                ActionPlanAdded(this, new ModelAddedEventArgs<FishboneNode_ActionPlan>(newActionPlanFishboneNode));
            
        }

        public void RemoveActionPlan(int fishboneId, int actionPlanId)
        {
                FishboneNode currentFishboneNode = _fishboneRepository.Single(fishbone => fishbone.Id == fishboneId);
                FishboneNode_ActionPlan currentFishboneNodeActionPlan =
                    currentFishboneNode.FishboneNode_ActionPlans.First(
                        fishboneActionPlan =>
                        fishboneActionPlan.FishboneNode.Id == fishboneId && fishboneActionPlan.ActionPlan.Id == actionPlanId);
                _fishboneActionplanRepository.Delete(currentFishboneNodeActionPlan);
                Context.Commit();
                ActionPlanRemoved(this, new ModelRemovedEventArgs(actionPlanId));
        }
    }
}