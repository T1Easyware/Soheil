using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels.OrganizationCalendar;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
	public class WorkProfilesVM : GridSplitViewModel
	{
        #region Properties
		public override void CreateItems(object param)
		{
			var viewModels = new ObservableCollection<WorkProfileVm>();
			foreach (var model in WorkProfileDataService.GetAll())
			{
				viewModels.Add(new WorkProfileVm(model, Access));
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
		public WorkProfileDataService WorkProfileDataService { get; set; }

        #endregion

        #region Methods

        /// <summary>
		/// Initializes a new instance of the <see cref="WorkProfilesVM"/> class.
        /// </summary>
		public WorkProfilesVM(AccessType access)
            : base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
			WorkProfileDataService = new WorkProfileDataService();
			WorkProfileDataService.WorkProfileAdded += OnWorkProfileAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Name",0, false), 
                new ColumnInfo("NumberOfShiftsInGrid", "txtName",1, true), 
            };

            AddCommand = new Command(Add, CanAdd);
			RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);
			ResetCommand = new Command(Reset, CanReset);
            CreateItems(null);
        }

		public override void Add(object parameter)
		{
			if (CurrentContent == null || CurrentContent is WorkProfileVm)
			{
				WorkProfileVm.CreateNew(WorkProfileDataService);
			}
		}
		public override void View(object content)
		{
			if (content is WorkProfileVm)
			{
				var vm = content as WorkProfileVm;
				CurrentContent = vm;
			}
		}

		public override void Reset(object oldVm)
		{
			//
		}

		private void OnWorkProfileAdded(object sender, ModelAddedEventArgs<WorkProfile> e)
		{
			var newVm = new WorkProfileVm(e.NewModel, Access);
			Items.AddNewItem(newVm);
			Items.CommitNew();
			CurrentContent = newVm;
			CurrentContent.IsSelected = true;
		}


        public override ISplitItemContent CreateClone(ISplitItemContent original)
        {
			var viewModel = original as WorkProfileVm;
			var model = WorkProfileDataService.CloneModelById(viewModel.Id);
			return new WorkProfileVm(model, Access);
        }
        #endregion
	}
}
