using System.Collections.ObjectModel;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class ActionPlanFishbonesVM : ItemLinkViewModel
    {

        public ActionPlanFishbonesVM(ActionPlanVM actionPlan, AccessType access)
            : base(access)
        {
            CurrentActionPlan = actionPlan;
            ActionPlanDataService = new ActionPlanDataService();
            ActionPlanDataService.FishboneNodeAdded += OnFishboneNodeAdded;
            ActionPlanDataService.FishboneNodeRemoved += OnFishboneNodeRemoved;
            FishboneActionPlanDataService = new FishboneActionPlanDataService();
            FishboneNodeDataService=new FishboneNodeDataService();

            var selectedVms = new ObservableCollection<ActionPlanFishboneVM>();
            foreach (var fishboneNodeActionPlan in ActionPlanDataService.GetFishboneNodes(actionPlan.Id))
            {
                selectedVms.Add(new ActionPlanFishboneVM(fishboneNodeActionPlan,access,RelationDirection.Straight));
            }
            SelectedItems = new ListCollectionView(selectedVms);

            var allVms = new ObservableCollection<FishboneNodeVM>();
            foreach (var fishboneNode in FishboneNodeDataService.GetActives())
            {
                allVms.Add(new FishboneNodeVM(fishboneNode, Access, FishboneNodeDataService));
            }
            AllItems = new ListCollectionView(allVms);

            IncludeCommand = new Command(Include, CanInclude);
            ExcludeCommand = new Command(Exclude, CanExclude);
        }

        public ActionPlanVM CurrentActionPlan { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ActionPlanDataService ActionPlanDataService { get; set; }

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
        public FishboneActionPlanDataService FishboneActionPlanDataService { get; set; }

        private void OnFishboneNodeRemoved(object sender, ModelRemovedEventArgs e)
        {
            foreach (ProductVM item in SelectedItems)
            {
                if (item.Id == e.Id)
                {
                    SelectedItems.Remove(item);
                    break;
                }
            }
        }

        private void OnFishboneNodeAdded(object sender, ModelAddedEventArgs<FishboneNode_ActionPlan> e)
        {
            var productVm = new ActionPlanFishboneVM(e.NewModel, Access, RelationDirection.Straight);
            SelectedItems.AddNewItem(productVm);
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(FishboneActionPlanDataService.GetActives());
        }

        public override void Include(object param)
        {
            ActionPlanDataService.AddFishboneNode(CurrentActionPlan.Id, ((IEntityItem) param).Id);
        }

        public override void Exclude(object param)
        {
            ActionPlanDataService.RemoveFishboneNode(CurrentActionPlan.Id, ((IEntityItem) param).Id);
        }
    }
}