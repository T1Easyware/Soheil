using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class ActivityOperatorsVM : ItemLinkViewModel
    {
        public ActivityOperatorsVM(ActivityVM activity, AccessType access)
            : base(access)
        {
            CurrentActivity = activity;
            ActivityDataService = new ActivityDataService();
            ActivityDataService.OperatorAdded += OnOperatorAdded;
            ActivityDataService.OperatorRemoved += OnOperatorRemoved;
            OperatorDataService = new OperatorDataService();
            ActivityOperatorDataService = new ActivitySkillDataService();

            var selectedVms = new ObservableCollection<ActivityOperatorVM>();
            foreach (var activityOperator in ActivityDataService.GetOperators(activity.Id))
            {
                selectedVms.Add(new ActivityOperatorVM(activityOperator, Access, ActivityOperatorDataService, RelationDirection.Straight));
            }
            SelectedItems = new ListCollectionView(selectedVms);

            var allVms = new ObservableCollection<OperatorVM>();
            foreach (var opr in OperatorDataService.GetActives(SoheilEntityType.Activities, CurrentActivity.Id))
            {
                allVms.Add(new OperatorVM(opr, Access, OperatorDataService));
            }
            AllItems = new ListCollectionView(allVms);

            IncludeCommand = new Command(Include, CanInclude);
            ExcludeCommand = new Command(Exclude, CanExclude);
        }

        public ActivityVM CurrentActivity { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ActivityDataService ActivityDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public OperatorDataService OperatorDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public ActivitySkillDataService ActivityOperatorDataService { get; set; }

        private void OnOperatorRemoved(object sender, ModelRemovedEventArgs e)
        {
            foreach (ActivityOperatorVM item in SelectedItems)
            {
                if (item.Id == e.Id)
                {
                    var model = OperatorDataService.GetSingle(item.OperatorId);
                    var returnedVm = new OperatorVM(model, Access, OperatorDataService);
                    AllItems.AddNewItem(returnedVm);
                    AllItems.CommitNew();
                    SelectedItems.Remove(item);
                    break;
                }
            }
        }

        private void OnOperatorAdded(object sender, ModelAddedEventArgs<ActivitySkill> e)
        {
            var activityOperatorVm = new ActivityOperatorVM(e.NewModel, Access, ActivityOperatorDataService, RelationDirection.Straight);
            SelectedItems.AddNewItem(activityOperatorVm);
            SelectedItems.CommitNew();

            foreach (IEntityItem item in AllItems)
            {
                if (item.Id == activityOperatorVm.OperatorId)
                {
                    AllItems.Remove(item);
                    break;
                }
            }
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(OperatorDataService.GetAll());
        }

        public override void Include(object param)
        {
            ActivityDataService.AddOperator(CurrentActivity.Id, ((IEntityItem) param).Id);
        }

        public override void Exclude(object param)
        {
            ActivityDataService.RemoveOperator(CurrentActivity.Id, ((IEntityItem)param).Id);
        }

        public override void IncludeRange(object param)
        {
            var tempList = new List<ISplitContent>();
            tempList.AddRange(AllItems.Cast<ISplitContent>());
            foreach (ISplitContent item in tempList)
            {
                if (item.IsChecked)
                {
                    ActivityDataService.AddOperator(CurrentActivity.Id, ((IEntityItem)item).Id);
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
                    ActivityDataService.RemoveOperator(CurrentActivity.Id, ((IEntityItem)item).Id);
                }
            }
        }
    }
}