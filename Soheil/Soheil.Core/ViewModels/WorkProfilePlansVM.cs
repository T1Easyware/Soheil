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
	public class WorkProfilePlansVM : GridSplitViewModel
	{
        #region Properties
		public override void CreateItems(object param)
		{
			var viewModels = new ObservableCollection<WorkProfilePlanVm>();
			foreach (var model in WorkProfilePlanDataService.GetAll())
			{
				viewModels.Add(new WorkProfilePlanVm(model, Access, WorkProfilePlanDataService));
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
        public WorkProfilePlanDataService WorkProfilePlanDataService { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkProfilePlansVM"/> class.
        /// </summary>
        public WorkProfilePlansVM(AccessType access)
            : base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            WorkProfilePlanDataService = new WorkProfilePlanDataService();
            WorkProfilePlanDataService.WorkProfilePlanAdded += OnWorkProfilePlanAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Name",0, false), 
                new ColumnInfo("StartDate",1, false), 
                new ColumnInfo("EndDate",2, false), 
                new ColumnInfo("SelectedProfile.Name", "txtWorkProfile",3, true), 
            };

            AddCommand = new Command(Add, CanAdd);
			RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);
			ResetCommand = new Command(Reset, CanReset);
            CreateItems(null);
        }

		public override void Add(object parameter)
		{
			if (CurrentContent == null || CurrentContent is WorkProfilePlanVm)
			{
                WorkProfilePlanVm.CreateNew(WorkProfilePlanDataService);
			}
		}
		public override void View(object content)
		{
			if (content is WorkProfilePlanVm)
			{
				var vm = content as WorkProfilePlanVm;
				CurrentContent = vm;
			}
		}

		public override void Reset(object oldVm)
		{
			var vm = oldVm as WorkProfilePlanVm;
			if (vm != null)
			{
				vm.Reset();
			}
		}

        private void OnWorkProfilePlanAdded(object sender, ModelAddedEventArgs<WorkProfilePlan> e)
		{
			var newVm = new WorkProfilePlanVm(e.NewModel, Access, WorkProfilePlanDataService);
			Items.AddNewItem(newVm);
			Items.CommitNew();
			CurrentContent = newVm;
			CurrentContent.IsSelected = true;
		}


        public override ISplitItemContent CreateClone(ISplitItemContent original)
        {
            var viewModel = original as WorkProfilePlanVm;
            var model = WorkProfilePlanDataService.CloneModelById(viewModel.Id);
            return new WorkProfilePlanVm(model, Access, WorkProfilePlanDataService);
        }
        #endregion
	}
}
