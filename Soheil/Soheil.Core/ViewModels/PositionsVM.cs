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
    public class PositionsVM : GridSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            var viewModels = new ObservableCollection<PositionVM>();
            foreach (var model in PositionDataService.GetAll())
            {
                viewModels.Add(new PositionVM(model, Access, PositionDataService));
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
        public PositionDataService PositionDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionsVM"/> class.
        /// </summary>
        public PositionsVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            UnitOfWork = new SoheilEdmContext();
            PositionDataService = new PositionDataService(UnitOfWork);
            PositionDataService.PositionAdded += OnPositionAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Name",0), 
                new ColumnInfo("Status",1) ,
                new ColumnInfo("Mode",2,true) 
            };

            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);
            ViewCommand = new Command(View, CanView);
            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (CurrentContent == null || CurrentContent is PositionVM)
            {
                PositionVM.CreateNew(PositionDataService);
            }
        }
        public override void View(object content)
        {
            if (content is PositionVM)
            {
                var positionVm = content as PositionVM;
                CurrentContent = positionVm;
            }
        }
        private void OnPositionAdded(object sender, ModelAddedEventArgs<Position> e)
        {
            var newPositionVm = new PositionVM(e.NewModel, Access, PositionDataService);
            Items.AddNewItem(newPositionVm);
            Items.CommitNew();
            CurrentContent = newPositionVm;
            CurrentContent.IsSelected = true;
        }

        #endregion

    }
}