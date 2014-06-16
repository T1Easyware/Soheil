using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
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
    public class UserAccessRulesVM : NodeLinkViewModel
    {
        public UserAccessRulesVM(UserVM user, AccessType access):base(access)
        {
            UnitOfWork = new SoheilEdmContext();
            CurrentUser = user;
            UserDataService = new UserDataService(UnitOfWork);
            UserDataService.AccessRuleChanged += OnAccessRuleChanged;
            AccessRuleDataService = new AccessRuleDataService(UnitOfWork);
            UserAccessRuleDataService = new UserAccessRuleDataService(UnitOfWork);
            PositionAccessRuleDataService = new PositionAccessRuleDataService(UnitOfWork);

            RootNode = new UserAccessNodeVM(Access) { Title = string.Empty, Id = -1, ParentId = -2 };

            var selectedVms = new ObservableCollection<UserAccessNodeVM>();

            var ruleAccessList = AccessRuleDataService.GetPositionsAccessOfUser(user.Id);

            foreach (var accessRule in AccessRuleDataService.GetActives())
            {
                selectedVms.Add(new UserAccessNodeVM(accessRule.Id, user.Id, AccessRuleDataService, UserAccessRuleDataService, ruleAccessList, Access));
            }

            var allVms = new ObservableCollection<AccessRuleVM>();
            foreach (var accessRule in AccessRuleDataService.GetActives())
            {
                allVms.Add(new AccessRuleVM(AccessRuleDataService, accessRule, Access));
            }
            AllItems = new ListCollectionView(allVms);

            IncludeCommand = new Command(Include, CanInclude);
            ExcludeTreeCommand = new Command(ExcludeTree, CanExcludeTree);

            foreach (UserAccessNodeVM item in selectedVms)
            {
                if (item.ParentId == RootNode.Id)
                {
                    RootNode.ChildNodes.Add(item);
                    break;
                }
            }

            CurrentNode = RootNode;
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
        public AccessRuleDataService AccessRuleDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public UserAccessRuleDataService UserAccessRuleDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public PositionAccessRuleDataService PositionAccessRuleDataService { get; set; }

        public static readonly DependencyProperty AccessToSubItemsProperty =
            DependencyProperty.Register("AccessToSubItems", typeof(bool), typeof(UserAccessRulesVM), new PropertyMetadata(true));

        public bool AccessToSubItems
        {
            get { return (bool)GetValue(AccessToSubItemsProperty); }
            set { SetValue(AccessToSubItemsProperty, value); }
        }

        private void OnAccessRuleChanged(object sender, ModelAddedEventArgs<User_AccessRule> e)
        {
            var userAccessRule = new UserAccessRuleVM(e.NewModel, Access, RelationDirection.Straight);
            var newNode = FindNode(RootNode, userAccessRule.AccessRuleId) as UserAccessNodeVM;
            if (newNode != null)
                newNode.UserAccess = userAccessRule.Type;

            if (CurrentNode == RootNode)
            {
                CurrentNode = newNode;
            }
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(AccessRuleDataService.GetActives());
        }


        public override void Exclude(object param)
        {
            throw new NotImplementedException();
        }

        public override void Include(object param)
        {
            if (AccessToSubItems)
            {
                ExcludeTree(param);
            }
            else
            {
                UserDataService.AddRemoveAccessRule(CurrentUser.Id, CurrentNode.Id, (AccessType)param);
            }
        }

        public override void ExcludeTree(object param)
        {
            var relationIdList = new List<Tuple<int, int>>();
            FindRelationIdList(CurrentNode, relationIdList);
            foreach (Tuple<int, int> tuple in relationIdList)
            {
                UserDataService.AddRemoveAccessRule(tuple.Item1, tuple.Item2, (AccessType)param);
            }
            UserDataService.AddRemoveAccessRule(CurrentUser.Id, CurrentNode.Id, (AccessType)param);
        }

        private void FindRelationIdList(IEntityNode parentNode, List<Tuple<int, int>> relationIdList)
        {
            foreach (IEntityNode node in parentNode.ChildNodes)
            {
                if (node.ChildNodes.Count > 0)
                {
                    relationIdList.Add(new Tuple<int, int>(CurrentUser.Id, node.Id));
                    FindRelationIdList(node, relationIdList);
                }
                else
                {
                    relationIdList.Add(new Tuple<int, int>(CurrentUser.Id, node.Id));
                }
            }
        }

        public void RemoveAccess(IList nodeCollection, int id)
        {
            foreach (UserAccessNodeVM node in nodeCollection)
            {
                if (node.Id == id)
                {
                    node.UserAccess = AccessType.None;
                    return;
                }
                if (node.ChildNodes.Count > 0)
                {
                    RemoveAccess(node.ChildNodes, id);
                }
            }
        }
    }
}