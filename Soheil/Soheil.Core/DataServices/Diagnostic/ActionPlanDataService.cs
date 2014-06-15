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
                return _actionPlanRepository.Single(actionPlan => actionPlan.Id == id);
        }

        public ObservableCollection<ActionPlan> GetAll()
        {
                IEnumerable<ActionPlan> entityList =
                    _actionPlanRepository.Find(
                        actionPlan => actionPlan.Status != (decimal)Status.Deleted);
                return new ObservableCollection<ActionPlan>(entityList);
        }

        public int AddModel(ActionPlan model)
        {
                _actionPlanRepository.Add(model);
                Context.Commit();
                if (ActionPlanAdded != null)
                    ActionPlanAdded(this, new ModelAddedEventArgs<ActionPlan>(model));
                return model.Id;
        }

        public void UpdateModel(ActionPlan model)
        {
            model.ModifiedBy = LoginInfo.Id;
            model.ModifiedDate = DateTime.Now;
                Context.Commit();
        }

        public void DeleteModel(ActionPlan model)
        {
        }

        public void AttachModel(ActionPlan model)
        {
                if (_actionPlanRepository.Exists(actionPlan => actionPlan.Id == model.Id))
                {
                    UpdateModel(model);
                }
                else
                {
                    AddModel(model);
                }
        }

        #endregion

        public event EventHandler<ModelAddedEventArgs<ActionPlan>> ActionPlanAdded;
        public event EventHandler<ModelAddedEventArgs<FishboneNode_ActionPlan>> FishboneNodeAdded;
        public event EventHandler<ModelRemovedEventArgs> FishboneNodeRemoved;

        private readonly Repository<ActionPlan> _actionPlanRepository;
        private readonly Repository<FishboneNode_ActionPlan> _fishboneActionplanRepository;
        private readonly Repository<FishboneNode> _fishboneRepository;
        public ActionPlanDataService(SoheilEdmContext context)
        {
            Context = context;
            _actionPlanRepository = new Repository<ActionPlan>(context);
            _fishboneActionplanRepository = new Repository<FishboneNode_ActionPlan>(context);
            _fishboneRepository = new Repository<FishboneNode>(context);
        }

        /// <summary>
        /// Gets all active roots as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<ActionPlan> GetActives()
        {
                IEnumerable<ActionPlan> entityList =
                    _actionPlanRepository.Find(
                        actionPlan => actionPlan.Status == (decimal)Status.Active);
                return new ObservableCollection<ActionPlan>(entityList);
        }

        /// <summary>
        /// Gets all active roots as view models.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<ActionPlan> GetActives(SoheilEntityType linkType, int linkId = 0)
        {
            if (linkType == SoheilEntityType.Roots)
            {
                    IEnumerable<ActionPlan> entityList =
                        _actionPlanRepository.Find(
                            actionPlan => actionPlan.Status == (decimal)Status.Active && actionPlan.FishboneNode_ActionPlan.Count == 0);
                    return new ObservableCollection<ActionPlan>(entityList);
            }
            return GetActives();
        }

        public ObservableCollection<FishboneNode_ActionPlan> GetFishboneNodes(int actionPlanId)
        {
                ActionPlan entity = _actionPlanRepository.FirstOrDefault(actionPlan => actionPlan.Id == actionPlanId && actionPlan.Status == (decimal) Status.Active, "FishboneNode_ActionPlan", "FishboneNode_ActionPlan.FishboneNode", "FishboneNode_ActionPlan.ActionPlan");
                return new ObservableCollection<FishboneNode_ActionPlan>(entity.FishboneNode_ActionPlan);
        }

        public void AddFishboneNode(int actionPlanId, int rootId)
        {
            ActionPlan currentActionPlan = _actionPlanRepository.Single(actionPlan => actionPlan.Id == actionPlanId);
            FishboneNode newFishbone = _fishboneRepository.Single(root => root.Id == rootId);
            if (
                currentActionPlan.FishboneNode_ActionPlan.Any(
                    actionPlanRoot =>
                        actionPlanRoot.ActionPlan.Id == actionPlanId && actionPlanRoot.FishboneNode.Id == rootId))
            {
                return;
            }
            var newFishboneActionPlan = new FishboneNode_ActionPlan
            {
                FishboneNode = newFishbone,
                ActionPlan = currentActionPlan
            };
            currentActionPlan.FishboneNode_ActionPlan.Add(newFishboneActionPlan);
            Context.Commit();
            FishboneNodeAdded(this, new ModelAddedEventArgs<FishboneNode_ActionPlan>(newFishboneActionPlan));
        }

        public void RemoveFishboneNode(int actionPlanId, int fishboneActionplanId)
        {
                ActionPlan currentActionPlan = _actionPlanRepository.Single(actionPlan => actionPlan.Id == actionPlanId);
                //FishboneNode_ActionPlan currentFishboneActionPlan =
                //    currentActionPlan.FishboneNode_ActionPlan.First(
                //        actionPlanRoot =>
                //        actionPlanRoot.ActionPlan.Id == actionPlanId && actionPlanRoot.FishboneNode.Id == rootId);
                FishboneNode_ActionPlan currentFishboneActionPlan =
        currentActionPlan.FishboneNode_ActionPlan.First(
            nodeActionPlan =>
            nodeActionPlan.Id == fishboneActionplanId);
                _fishboneActionplanRepository.Delete(currentFishboneActionPlan);
                Context.Commit();
                FishboneNodeRemoved(this, new ModelRemovedEventArgs(fishboneActionplanId));
        }
    }
}