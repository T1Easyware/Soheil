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
    public class PositionUsersVM : ItemLinkViewModel
    {
        public PositionUsersVM(PositionVM position, AccessType access)
            : base(access)
        {
            CurrentPosition = position;
            PositionDataService = new PositionDataService();
            PositionDataService.UserAdded += OnUserAdded;
            PositionDataService.UserRemoved += OnUserRemoved;
            UserDataService = new UserDataService();

            var selectedVms = new ObservableCollection<UserPositionVM>();
            foreach (var positionUser in PositionDataService.GetUsers(position.Id))
            {
                selectedVms.Add(new UserPositionVM(positionUser, Access, RelationDirection.Reverse));
            }
            SelectedItems = new ListCollectionView(selectedVms);

            var allVms = new ObservableCollection<UserVM>();
            foreach (var user in UserDataService.GetActives(SoheilEntityType.Positions, CurrentPosition.Id))
            {
                allVms.Add(new UserVM(user, Access, UserDataService));
            }
            AllItems = new ListCollectionView(allVms);

            IncludeCommand = new Command(Include, CanInclude);
            ExcludeCommand = new Command(Exclude, CanExclude);
            
        }

        public PositionVM CurrentPosition { get; set; }

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
        public UserDataService UserDataService { get; set; }

        private void OnUserRemoved(object sender, ModelRemovedEventArgs e)
        {
            foreach (UserPositionVM item in SelectedItems)
            {
                if (item.Id == e.Id)
                {
                    var model = UserDataService.GetSingle(item.UserId);
                    var returnedVm = new UserVM(model, Access, UserDataService);
                    AllItems.AddNewItem(returnedVm);
                    AllItems.CommitNew();
                    SelectedItems.Remove(item);
                    break;
                }
            }
        }

        private void OnUserAdded(object sender, ModelAddedEventArgs<User_Position> e)
        {
            var userPositionVm = new UserPositionVM(e.NewModel, Access, RelationDirection.Reverse);
            SelectedItems.AddNewItem(userPositionVm);
            SelectedItems.CommitNew();
            foreach (IEntityItem item in AllItems)
            {
                if (item.Id == userPositionVm.UserId)
                {
                    AllItems.Remove(item);
                    break;
                }
            }
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(UserDataService.GetActives());
        }

        public override void Include(object param)
        {
            PositionDataService.AddUser(CurrentPosition.Id, ((IEntityItem)param).Id);
        }

        public override void Exclude(object param)
        {
            PositionDataService.RemoveUser(CurrentPosition.Id, ((IEntityItem)param).Id);
        }

        public override void IncludeRange(object param)
        {
            var tempList = new List<ISplitContent>();
            tempList.AddRange(AllItems.Cast<ISplitContent>());
            foreach (ISplitContent item in tempList)
            {
                if (item.IsChecked)
                {
                    PositionDataService.AddUser(CurrentPosition.Id, ((IEntityItem)item).Id);
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
                    PositionDataService.RemoveUser(CurrentPosition.Id, ((IEntityItem)item).Id);
                }
            }
        }
    }
}