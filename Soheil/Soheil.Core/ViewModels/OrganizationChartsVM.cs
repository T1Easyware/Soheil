using System.Collections.Generic;
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
    public class OrganizationChartsVM : GridSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            var viewModels = new ObservableCollection<OrganizationChartVM>();
            foreach (var model in OrganizationChartDataService.GetAll())
            {
                viewModels.Add(new OrganizationChartVM(model, Access, OrganizationChartDataService));
            }
            Items = new ListCollectionView(viewModels);

            if (viewModels.Count > 0)
            {
                CurrentContent = (ISplitItemContent)Items.CurrentItem;
                CurrentContent.IsSelected = true;
            }
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public OrganizationChartDataService OrganizationChartDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationChartsVM"/> class.
        /// </summary>
        public OrganizationChartsVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            OrganizationChartDataService = new OrganizationChartDataService();
            OrganizationChartDataService.OrganizationChartAdded += OnOrganizationChartAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Name"), 
                new ColumnInfo("Status") ,
                new ColumnInfo("Mode",true) 
            };

            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);
            ViewCommand = new Command(View, CanView);
            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (CurrentContent == null || CurrentContent is OrganizationChartVM)
            {
                OrganizationChartVM.CreateNew(OrganizationChartDataService);
            }
        }
        public override void View(object content)
        {
            if (content is OrganizationChartVM)
            {
                var orgChartVm = content as OrganizationChartVM;
                CurrentContent = orgChartVm;
            }
        }
        private void OnOrganizationChartAdded(object sender, ModelAddedEventArgs<OrganizationChart> e)
        {
            var newOrganizationChartVm = new OrganizationChartVM(e.NewModel, Access, OrganizationChartDataService);
            Items.AddNewItem(newOrganizationChartVm);
            Items.CommitNew();
            CurrentContent = newOrganizationChartVm;
            CurrentContent.IsSelected = true;
        }

        #endregion

    }
}