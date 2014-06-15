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
    public class UserPositionsVM : ItemLinkViewModel
    {
        public UserPositionsVM(UserVM user, AccessType access)
            : base(access)
        {
            UnitOfWork = new SoheilEdmContext();
            CurrentUser = user;
            UserDataService = new UserDataService(UnitOfWork);
            PositionDataService = new PositionDataService(UnitOfWork);
            AccessRuleDataService = new AccessRuleDataService(UnitOfWork);
            UserDataService.PositionAdded += OnPositionAdded;
            UserDataService.PositionRemoved += OnPositionRemoved;


            var selectedVms = new ObservableCollection<UserPositionVM>();
            foreach (var userPosition in UserDataService.GetPositions(user.Id))
            {
                selectedVms.Add(new UserPositionVM(userPosition, Access, RelationDirection.Straight));
            }
            SelectedItems = new ListCollectionView(selectedVms);

            var allVms = new ObservableCollection<PositionVM>();
            foreach (var position in PositionDataService.GetActives(SoheilEntityType.Users, CurrentUser.Id))
            {
                allVms.Add(new PositionVM(position, Access, PositionDataService));
            }
            AllItems = new ListCollectionView(allVms);

            //AllItems = new ListCollectionView(PositionDataService.GetActives());
            IncludeCommand = new Command(Include, CanInclude);
            ExcludeCommand = new Command(Exclude, CanExclude);
        }

        public UserVM CurrentUser { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public UserDataService UserDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public PositionDataService PositionDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public AccessRuleDataService AccessRuleDataService { get; set; }

        private void OnPositionRemoved(object sender, ModelRemovedEventArgs e)
        {
            foreach (UserPositionVM item in SelectedItems)
            {
                if (item.Id == e.Id)
                {
                    var model = PositionDataService.GetSingle(item.PositionId);
                    var returnedVm = new PositionVM(model, Access, PositionDataService);
                    AllItems.AddNewItem(returnedVm);
                    AllItems.CommitNew();
                    SelectedItems.Remove(item);
                    break;
                }
            }
        }

        private void OnPositionAdded(object sender, ModelAddedEventArgs<User_Position> e)
        {
            var positionUserVm = new UserPositionVM(e.NewModel, Access, RelationDirection.Straight);
            SelectedItems.AddNewItem(positionUserVm);
            SelectedItems.CommitNew();
            foreach (IEntityItem item in AllItems)
            {
                if (item.Id == positionUserVm.PositionId)
                {
                    AllItems.Remove(item);
                    break;
                }
            }
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(PositionDataService.GetActives());
        }


        public override void Include(object param)
        {
            UserDataService.AddPosition(CurrentUser.Id, ((IEntityItem)param).Id);
        }

        public override void Exclude(object param)
        {
            UserDataService.RemovePosition(CurrentUser.Id, ((IEntityItem)param).Id);
        }

        public override void IncludeRange(object param)
        {
            var tempList = new List<ISplitContent>();
            tempList.AddRange(AllItems.Cast<ISplitContent>());
            foreach (ISplitContent item in tempList)
            {
                if (item.IsChecked)
                {
                    UserDataService.AddPosition(CurrentUser.Id, ((IEntityItem)item).Id);
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
                    UserDataService.RemovePosition(CurrentUser.Id, ((IEntityItem)item).Id);
                }
            }
        }
    }
}