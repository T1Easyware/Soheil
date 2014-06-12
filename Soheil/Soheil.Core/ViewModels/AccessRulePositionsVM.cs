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
    public class AccessRulePositionsVM : ItemLinkViewModel
    {
        public AccessRulePositionsVM(AccessRuleVM accessRule, AccessType access)
            : base(access)
        {
            CurrentAccessRule = accessRule;
            AccessRuleDataService = new AccessRuleDataService();
            AccessRuleDataService.PositionAdded += OnPositionAdded;
            AccessRuleDataService.PositionRemoved += OnPositionRemoved;
            PositionDataService = new PositionDataService();

            var selectedVms = new ObservableCollection<PositionAccessRuleVM>();
            foreach (var accessRulePosition in AccessRuleDataService.GetPositions(accessRule.Id))
            {
                selectedVms.Add(new PositionAccessRuleVM(accessRulePosition, Access, RelationDirection.Reverse));
            }
            SelectedItems = new ListCollectionView(selectedVms);

            var allVms = new ObservableCollection<PositionVM>();
            foreach (var position in PositionDataService.GetActives())
            {
                allVms.Add(new PositionVM(position, Access,PositionDataService));
            }
            AllItems = new ListCollectionView(allVms);

            IncludeCommand = new Command(Include,CanInclude);
            ExcludeCommand = new Command(Exclude,CanExclude);
        }

        public AccessRuleVM CurrentAccessRule { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public AccessRuleDataService AccessRuleDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public PositionDataService PositionDataService { get; set; }

        private void OnPositionRemoved(object sender, ModelRemovedEventArgs e)
        {
            foreach (PositionAccessRuleVM item in SelectedItems)
            {
                if (item.Id == e.Id)
                {
                    SelectedItems.Remove(item);
                    break;
                }
            }
        }

        private void OnPositionAdded(object sender, ModelAddedEventArgs<Position_AccessRule> e)
        {
            var positionAccessRuleVm = new PositionAccessRuleVM(e.NewModel, Access, RelationDirection.Reverse);
            SelectedItems.AddNewItem(positionAccessRuleVm);
            SelectedItems.CommitNew();
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(PositionDataService.GetActives());
        }

        public override void Include(object param)
        {
            AccessRuleDataService.AddPosition(CurrentAccessRule.Id, ((IEntityItem)param).Id);
        }

        public override void Exclude(object param)
        {
            AccessRuleDataService.RemovePosition(CurrentAccessRule.Id, ((IEntityItem)param).Id);
        }

        public override void IncludeRange(object param)
        {
            var tempList = new List<ISplitContent>();
            tempList.AddRange(AllItems.Cast<ISplitContent>());
            foreach (ISplitContent item in tempList)
            {
                if (item.IsChecked)
                {
                    AccessRuleDataService.AddPosition(CurrentAccessRule.Id, ((IEntityItem)item).Id);
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
                    AccessRuleDataService.RemovePosition(CurrentAccessRule.Id, ((IEntityItem)item).Id);
                }
            }
        }
    }
}