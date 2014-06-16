using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class OrganizationChartPositionsVM : NodeLinkViewModel
    {
        public OrganizationChartPositionsVM(OrganizationChartVM organizationChart, AccessType access):base(access)
        {
            UnitOfWork = new SoheilEdmContext();
            CurrentOrganizationChart = organizationChart;
            OrganizationChartDataService = new OrganizationChartDataService(UnitOfWork);
            OrganizationChartDataService.PositionAdded += OnPositionAdded;
            OrganizationChartDataService.PositionRemoved += OnPositionRemoved;
            PositionDataService = new PositionDataService(UnitOfWork);
            OrganizationChartPositionDataService = new OrganizationChartPositionDataService(UnitOfWork);

            RootNode = new OrganizationChartPositionVM(Access, OrganizationChartPositionDataService) { Title = string.Empty, Id = -1, ParentId = -2 };

            var selectedVms = new ObservableCollection<OrganizationChartPositionVM>();
            foreach (var organizationChartPosition in OrganizationChartDataService.GetPositions(organizationChart.Id))
            {
                selectedVms.Add(new OrganizationChartPositionVM(organizationChartPosition, Access, OrganizationChartPositionDataService));
            }

            var allVms = new ObservableCollection<PositionVM>();
            foreach (var position in PositionDataService.GetActives(SoheilEntityType.OrganizationCharts, CurrentOrganizationChart.Id))
            {
                allVms.Add(new PositionVM(position, Access, PositionDataService));
            }
            AllItems = new ListCollectionView(allVms);

            IncludeCommand = new Command(Include, CanInclude);
            ExcludeTreeCommand = new Command(ExcludeTree, CanExcludeTree);


            foreach (OrganizationChartPositionVM item in selectedVms)
            {
                if (item.ParentId == RootNode.Id)
                {
                    RootNode.ChildNodes.Add(item);
                    break;
                }
            }

            CurrentNode = RootNode;
        }

        public OrganizationChartVM CurrentOrganizationChart { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public OrganizationChartDataService OrganizationChartDataService { get; set; }

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
        public OrganizationChartPositionDataService OrganizationChartPositionDataService { get; set; }

        private void OnPositionRemoved(object sender, ModelRemovedEventArgs e)
        {
            var removedNode = FindNode(RootNode, e.Id);
            int parentId = removedNode.ParentId;
            RemoveNode(RootNode.ChildNodes, removedNode.Id);
            CurrentNode = FindNode(RootNode, parentId);
        }

        private void OnPositionAdded(object sender, ModelAddedEventArgs<OrganizationChart_Position> e)
        {
            var positionOrganizationChartVm = new OrganizationChartPositionVM(e.NewModel, Access, OrganizationChartPositionDataService);
            CurrentNode.ChildNodes.Add(positionOrganizationChartVm);

            if (CurrentNode == RootNode)
            {
                CurrentNode = positionOrganizationChartVm;
            }
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(PositionDataService.GetActives());
        }

        public override void Exclude(object param)
        {
            throw new NotImplementedException();
        }

        public override void Include(object organizationChartId)
        {
            var param = (int[])organizationChartId;
            OrganizationChartDataService.AddPosition(CurrentOrganizationChart.Id, param[0], param[1]);
        }

        public override void ExcludeTree(object positionOrganizationChartVm)
        {
            CurrentNode = (IEntityNode) positionOrganizationChartVm;
            var relationIdList = new List<Tuple<int, int>>();
            FindRelationIdList(CurrentNode, relationIdList);
            foreach (Tuple<int, int> tuple in relationIdList)
            {
                OrganizationChartDataService.RemovePosition(tuple.Item1, tuple.Item2);
            }
            OrganizationChartDataService.RemovePosition(CurrentOrganizationChart.Id, ((OrganizationChartPositionVM)CurrentNode).PositionId);
        }

        private void FindRelationIdList(IEntityNode parentNode, List<Tuple<int,int>> relationIdList)
        {
            foreach (IEntityNode node in parentNode.ChildNodes)
            {
                if (node.ChildNodes.Count > 0)
                {
                    FindRelationIdList(node, relationIdList);
                }
                else
                {
                    relationIdList.Add(new Tuple<int, int>(CurrentOrganizationChart.Id, ((OrganizationChartPositionVM)node).PositionId));
                }
            }
        }
    }
}