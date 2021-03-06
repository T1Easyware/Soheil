﻿using System.Collections.Generic;
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
    public class AccessRuleUsersVM : ItemLinkViewModel
    {
        public AccessRuleUsersVM(AccessRuleVM accessRule, AccessType access)
            : base(access)
        {
            UnitOfWork = new SoheilEdmContext();
            CurrentAccessRule = accessRule;
            AccessRuleDataService = new AccessRuleDataService(UnitOfWork);
            AccessRuleDataService.UserAdded += OnUserAdded;
            AccessRuleDataService.UserRemoved += OnUserRemoved;
            UserDataService = new UserDataService(UnitOfWork);

            var selectedVms = new ObservableCollection<UserAccessRuleVM>();
            foreach (var accessRuleUser in AccessRuleDataService.GetUsers(accessRule.Id))
            {
                selectedVms.Add(new UserAccessRuleVM(accessRuleUser, Access, RelationDirection.Reverse));
            }
            SelectedItems = new ListCollectionView(selectedVms);

            var allVms = new ObservableCollection<UserVM>();
            foreach (var user in UserDataService.GetActives())
            {
                allVms.Add(new UserVM(user, Access, UserDataService));
            }
            AllItems = new ListCollectionView(allVms);

            IncludeCommand = new Command(Include,CanInclude);
            ExcludeCommand = new Command(Exclude, CanExclude);
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
        public UserDataService UserDataService { get; set; }

        private void OnUserRemoved(object sender, ModelRemovedEventArgs e)
        {
            foreach (UserAccessRuleVM item in SelectedItems)
            {
                if (item.Id == e.Id)
                {
                    SelectedItems.Remove(item);
                    break;
                }
            }
        }

        private void OnUserAdded(object sender, ModelAddedEventArgs<User_AccessRule> e)
        {
            var userAccessRuleVm = new UserAccessRuleVM(e.NewModel, Access, RelationDirection.Reverse);
            SelectedItems.AddNewItem(userAccessRuleVm);
            SelectedItems.CommitNew();
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(UserDataService.GetActives());
        }

        public override void Include(object param)
        {
            AccessRuleDataService.AddUser(CurrentAccessRule.Id, ((IEntityItem)param).Id);
        }

        public override void Exclude(object param)
        {
            AccessRuleDataService.RemoveUser(CurrentAccessRule.Id, ((IEntityItem)param).Id);
        }

        public override void IncludeRange(object param)
        {
            var tempList = new List<ISplitContent>();
            tempList.AddRange(AllItems.Cast<ISplitContent>());
            foreach (ISplitContent item in tempList)
            {
                if (item.IsChecked)
                {
                    AccessRuleDataService.AddUser(CurrentAccessRule.Id, ((IEntityItem)item).Id);
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
                    AccessRuleDataService.AddUser(CurrentAccessRule.Id, ((IEntityItem)item).Id);
                }
            }
        }
    }
}