using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class FishboneNodeActionPlansVM : ItemLinkViewModel
    {

        public FishboneNodeActionPlansVM(FishboneNodeVM defection, AccessType access)
            : base(access)
        {
            UnitOfWork = new SoheilEdmContext();
            CurrentFishboneNode = defection;
            FishboneNodeDataService = new FishboneNodeDataService(UnitOfWork);
            FishboneNodeDataService.ActionPlanAdded += OnActionPlanAdded;
            FishboneNodeDataService.ActionPlanRemoved += OnActionPlanRemoved;
            ActionPlanDataService = new ActionPlanDataService(UnitOfWork);

            var selectedVms = new ObservableCollection<ActionPlanFishboneVM>();
            foreach (var productFishboneNode in FishboneNodeDataService.GetActionPlans(defection.Id))
            {
                selectedVms.Add(new ActionPlanFishboneVM(productFishboneNode, Access, RelationDirection.Reverse));
            }
            SelectedItems = new ListCollectionView(selectedVms);

            var allVms = new ObservableCollection<ActionPlanVM>();
            foreach (var actionPlan in ActionPlanDataService.GetActives())
            {
                allVms.Add(new ActionPlanVM(actionPlan, Access, ActionPlanDataService));
            }
            AllItems = new ListCollectionView(allVms);

            IncludeCommand = new Command(Include, CanInclude);
            ExcludeCommand = new Command(Exclude, CanExclude);
        }

        public FishboneNodeVM CurrentFishboneNode { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public FishboneNodeDataService FishboneNodeDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ActionPlanDataService ActionPlanDataService { get; set; }

        private void OnActionPlanRemoved(object sender, ModelRemovedEventArgs e)
        {
            foreach (ActionPlanFishboneVM item in SelectedItems)
            {
                if (item.Id == e.Id)
                {
                    SelectedItems.Remove(item);
                    break;
                }
            }
        }

        private void OnActionPlanAdded(object sender, ModelAddedEventArgs<FishboneNode_ActionPlan> e)
        {
            var productVm = new ActionPlanFishboneVM(e.NewModel, Access , RelationDirection.Reverse);
            SelectedItems.AddNewItem(productVm);
            SelectedItems.CommitNew();
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(ActionPlanDataService.GetActives());
        }

        public override void Include(object param)
        {
            FishboneNodeDataService.AddActionPlan(CurrentFishboneNode.Id, ((IEntityItem) param).Id);
        }

        public override void Exclude(object param)
        {
            FishboneNodeDataService.RemoveActionPlan(CurrentFishboneNode.Id, ((IEntityItem) param).Id);
        }

        public override void IncludeRange(object param)
        {
            var tempList = new List<ISplitContent>();
            tempList.AddRange(AllItems.Cast<ISplitContent>());
            foreach (ISplitContent item in tempList)
            {
                if (item.IsChecked)
                {
                    FishboneNodeDataService.AddActionPlan(CurrentFishboneNode.Id, ((IEntityItem)item).Id);
                }
            }
        }

        public override void ExcludeRange(object param)
        {
            var tempList = new List<ISplitDetail>();
            tempList.AddRange(SelectedItems.Cast<ISplitDetail>());
            foreach (ISplitDetail item in tempList)
            {
                if (item.IsChecked)
                {
                    FishboneNodeDataService.RemoveActionPlan(CurrentFishboneNode.Id, ((IEntityItem)item).Id);
                }
            }
        }
    }
}