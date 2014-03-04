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
    public class OperatorActivitiesVM : ItemLinkViewModel
    {
        public OperatorActivitiesVM(OperatorVM opr, AccessType access) : base(access)
        {
            CurrentOperator = opr;
            OperatorDataService = new OperatorDataService();
            OperatorDataService.ActivityAdded += OnActivityAdded;
            OperatorDataService.ActivityRemoved += OnActivityRemoved;
            ActivityDataService = new ActivityDataService();
            ActivityOperatorDataService = new ActivitySkillDataService();
            ActivityGroupDataService = new ActivityGroupDataService();

            var selectedVms = new ObservableCollection<ActivityOperatorVM>();
            foreach (var generalActivitySkill in OperatorDataService.GetActivities(opr.Id))
            {
                selectedVms.Add(new ActivityOperatorVM(generalActivitySkill, Access, ActivityOperatorDataService, RelationDirection.Reverse));
            }
            SelectedItems = new ListCollectionView(selectedVms);

            var allVms = new ObservableCollection<ActivityVM>();
            foreach (var activity in ActivityDataService.GetActives())
            {
                allVms.Add(new ActivityVM(activity, Access, ActivityDataService, ActivityGroupDataService));
            }
            AllItems = new ListCollectionView(allVms);

            IncludeCommand = new Command(Include, CanInclude);
            ExcludeCommand = new Command(Exclude, CanExclude);
            
        }

        public OperatorVM CurrentOperator { get; set; }

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
        public ActivityDataService ActivityDataService { get; set; }

        /// <summary>
        /// Gets or sets the activity-operator data service.
        /// </summary>
        /// <value>
        /// The activity-operator data service.
        /// </value>
        public ActivitySkillDataService ActivityOperatorDataService { get; set; }

        /// <summary>
        /// Gets or sets the activity-operator data service.
        /// </summary>
        /// <value>
        /// The activity-operator data service.
        /// </value>
        public ActivityGroupDataService ActivityGroupDataService { get; set; }

        private void OnActivityRemoved(object sender, ModelRemovedEventArgs e)
        {
            foreach (ActivityOperatorVM item in SelectedItems)
            {
                if (item.Id == e.Id)
                {
                    SelectedItems.Remove(item);
                    break;
                }
            }
        }

        private void OnActivityAdded(object sender, ModelAddedEventArgs<ActivitySkill> e)
        {
            var activityOperatorVm = new ActivityOperatorVM(e.NewModel, Access, ActivityOperatorDataService, RelationDirection.Reverse);
            SelectedItems.AddNewItem(activityOperatorVm);
            SelectedItems.CommitNew();
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(ActivityDataService.GetActives());
        }

        public override void Include(object param)
        {
            OperatorDataService.AddActivity(CurrentOperator.Id, ((IEntityItem) param).Id);
        }

        public override void Exclude(object param)
        {
            OperatorDataService.RemoveActivity(CurrentOperator.Id, ((IEntityItem)param).Id);
        }
    }
}