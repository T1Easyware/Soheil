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
    public class PositionOrganizationChartsVM : ItemLinkViewModel
    {
        public PositionOrganizationChartsVM(PositionVM position, AccessType access)
            : base(access)
        {
            UnitOfWork = new SoheilEdmContext();
            CurrentPosition = position;
            PositionDataService = new PositionDataService(UnitOfWork);
            PositionDataService.OrganizationChartAdded += OnOrganizationChartAdded;
            PositionDataService.OrganizationChartRemoved += OnOrganizationChartRemoved;
            OrganizationChartDataService = new OrganizationChartDataService(UnitOfWork);
            OrganizationChartPositionDataService = new OrganizationChartPositionDataService(UnitOfWork);

            var selectedVms = new ObservableCollection<OrganizationChartPositionVM>();
            foreach (var positionOrganizationChart in PositionDataService.GetOrganizationCharts(position.Id))
            {
                selectedVms.Add(new OrganizationChartPositionVM(positionOrganizationChart, Access, OrganizationChartPositionDataService));
            }
            SelectedItems = new ListCollectionView(selectedVms);

            var allVms = new ObservableCollection<OrganizationChartVM>();
            foreach (var organizationChart in OrganizationChartDataService.GetActives())
            {
                allVms.Add(new OrganizationChartVM(organizationChart, Access, OrganizationChartDataService));
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
        public OrganizationChartDataService OrganizationChartDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public OrganizationChartPositionDataService OrganizationChartPositionDataService { get; set; }

        private void OnOrganizationChartRemoved(object sender, ModelRemovedEventArgs e)
        {
            foreach (OrganizationChartPositionVM item in SelectedItems)
            {
                if (item.Id == e.Id)
                {
                    SelectedItems.Remove(item);
                    break;
                }
            }
        }

        private void OnOrganizationChartAdded(object sender, ModelAddedEventArgs<OrganizationChart_Position> e)
        {
            var organizationChartPositionVm = new OrganizationChartPositionVM(e.NewModel, Access, OrganizationChartPositionDataService);
            SelectedItems.AddNewItem(organizationChartPositionVm);
            SelectedItems.CommitNew();
        }

        public override void IncludeRange(object param)
        {
            throw new System.NotImplementedException();
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(OrganizationChartDataService.GetActives());
        }

        public override void Include(object param)
        {
            PositionDataService.AddOrganizationChart(CurrentPosition.Id, ((IEntityItem)param).Id);
        }

        public override void ExcludeRange(object param)
        {
            throw new System.NotImplementedException();
        }

        public override void Exclude(object param)
        {
            PositionDataService.RemoveOrganizationChart(CurrentPosition.Id,  ((IEntityItem)param).Id);
        }
    }
}