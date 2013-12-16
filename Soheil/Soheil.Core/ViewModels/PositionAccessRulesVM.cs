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
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class PositionAccessRulesVM : NodeLinkViewModel
    {
        public PositionAccessRulesVM(PositionVM position, AccessType access):base(access)
        {
            CurrentPosition = position;
            PositionDataService = new PositionDataService();
            PositionDataService.AccessRuleChanged += OnAccessRuleChanged;
            AccessRuleDataService = new AccessRuleDataService();
            PositionAccessRuleDataService = new PositionAccessRuleDataService();

            RootNode = new PositionAccessNodeVM(Access) { Title = string.Empty, Id = -1, ParentId = -2 };

            var selectedVms = new ObservableCollection<PositionAccessNodeVM>();
            foreach (var accessRule in AccessRuleDataService.GetActives())
            {
                selectedVms.Add(new PositionAccessNodeVM(accessRule.Id, position.Id, AccessRuleDataService, PositionAccessRuleDataService, Access));
            }

            var allVms = new ObservableCollection<AccessRuleVM>();
            foreach (var accessRule in AccessRuleDataService.GetActives())
            {
                allVms.Add(new AccessRuleVM(AccessRuleDataService, accessRule, Access));
            }
            AllItems = new ListCollectionView(allVms);

            IncludeCommand = new Command(Include, CanInclude);
            ExcludeTreeCommand = new Command(ExcludeTree, CanExcludeTree);

            foreach (PositionAccessNodeVM item in selectedVms)
            {
                if (item.ParentId == RootNode.Id)
                {
                    RootNode.ChildNodes.Add(item);
                    break;
                }
            }

            CurrentNode = RootNode;
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
        public AccessRuleDataService AccessRuleDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public PositionAccessRuleDataService PositionAccessRuleDataService { get; set; }

        public static readonly DependencyProperty AccessToSubItemsProperty =
            DependencyProperty.Register("AccessToSubItems", typeof (bool), typeof (PositionAccessRulesVM), new PropertyMetadata(true));

        public bool AccessToSubItems
        {
            get { return (bool) GetValue(AccessToSubItemsProperty); }
            set { SetValue(AccessToSubItemsProperty, value); }
        }

        private void OnAccessRuleChanged(object sender, ModelAddedEventArgs<Position_AccessRule> e)
        {
            var positionAccessRule = new PositionAccessRuleVM(e.NewModel, Access, RelationDirection.Straight);
            var newNode = FindNode(RootNode, positionAccessRule.AccessRuleId) as PositionAccessNodeVM;
            if (newNode != null)
                newNode.PositionAccess = positionAccessRule.Type;

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
                PositionDataService.AddRemoveAccessRule(CurrentPosition.Id, CurrentNode.Id, (AccessType)param);
            }
        }

        public override void ExcludeTree(object param)
        {
            var relationIdList = new List<Tuple<int, int>>();
            FindRelationIdList(CurrentNode, relationIdList);
            foreach (Tuple<int, int> tuple in relationIdList)
            {
                PositionDataService.AddRemoveAccessRule(tuple.Item1, tuple.Item2, (AccessType)param);
            }
            PositionDataService.AddRemoveAccessRule(CurrentPosition.Id, CurrentNode.Id, (AccessType) param);
        }

        private void FindRelationIdList(IEntityNode parentNode, List<Tuple<int, int>> relationIdList)
        {
            foreach (IEntityNode node in parentNode.ChildNodes)
            {
                if (node.ChildNodes.Count > 0)
                {
                    relationIdList.Add(new Tuple<int, int>(CurrentPosition.Id, node.Id));
                    FindRelationIdList(node, relationIdList);
                }
                else
                {
                    relationIdList.Add(new Tuple<int, int>(CurrentPosition.Id, node.Id));
                }
            }
        }

        public void RemoveAccess(IList nodeCollection, int id)
        {
            foreach (PositionAccessNodeVM node in nodeCollection)
            {
                if (node.Id == id)
                {
                    node.PositionAccess = AccessType.None;
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