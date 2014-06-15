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
    public class StationsVM : GridSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            var viewModels = new ObservableCollection<StationVM>();
            foreach (var model in StationDataService.GetAll())
            {
                viewModels.Add(new StationVM(model, Access, StationDataService));
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
        public StationDataService StationDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="StationsVM"/> class.
        /// </summary>
        public StationsVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            UnitOfWork = new SoheilEdmContext();
            StationDataService = new StationDataService(UnitOfWork);
            StationDataService.StationAdded += OnStationAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Code",0), 
                new ColumnInfo("Name",1), 
                new ColumnInfo("Index","txtStationIndex",2), 
                new ColumnInfo("Status",3) ,
                new ColumnInfo("Mode",4,true) 
            };

            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);
            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (CurrentContent == null || CurrentContent is StationVM)
            {
                StationVM.CreateNew(StationDataService);
            }
        }

        public override void View(object content)
        {
            //else if (content is MachineVM)
            //{
            //    CurrentContent = content as MachineVM;
            //}
        }

        private void OnStationAdded(object sender, ModelAddedEventArgs<Station> e)
        {
            var newStationVm = new StationVM(e.NewModel, Access, StationDataService);
            Items.AddNewItem(newStationVm);
            Items.CommitNew();
            CurrentContent = newStationVm;
            CurrentContent.IsSelected = true;
        }

        #endregion

    }
}