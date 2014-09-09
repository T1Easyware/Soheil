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
    public class RawMaterialsVM : GridSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            var viewModels = new ObservableCollection<RawMaterialVM>();
            foreach (var model in RawMaterialDataService.GetAll())
            {
                viewModels.Add(new RawMaterialVM(model, Access, RawMaterialDataService, UnitGroupDataService));
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
        public RawMaterialDataService RawMaterialDataService { get; set; }
        public UnitGroupDataService UnitGroupDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="RawMaterialsVM"/> class.
        /// </summary>
        public RawMaterialsVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            UnitOfWork = new SoheilEdmContext();
            RawMaterialDataService = new RawMaterialDataService(UnitOfWork);
            RawMaterialDataService.RawMaterialAdded += OnRawMaterialAdded;
            UnitGroupDataService = new UnitGroupDataService(UnitOfWork);

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Code",0), 
                new ColumnInfo("Name",1), 
                new ColumnInfo("Status",2) ,
                new ColumnInfo("Mode",3,true) 
            };

            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);
            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (CurrentContent == null || CurrentContent is RawMaterialVM)
            {
                RawMaterialVM.CreateNew(RawMaterialDataService);
            }
        }

        public override void View(object content)
        {
            //else if (content is MachineVM)
            //{
            //    CurrentContent = content as MachineVM;
            //}
        }

        private void OnRawMaterialAdded(object sender, ModelAddedEventArgs<RawMaterial> e)
        {
            var newRawMaterialVm = new RawMaterialVM(e.NewModel, Access, RawMaterialDataService, UnitGroupDataService);
            Items.AddNewItem(newRawMaterialVm);
            Items.CommitNew();
            CurrentContent = newRawMaterialVm;
            CurrentContent.IsSelected = true;
        }

        #endregion

    }
}