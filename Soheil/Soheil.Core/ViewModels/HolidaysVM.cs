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
	public class HolidaysVM : GridSplitViewModel
	{
        #region Properties
        public static List<BusinessDayType> BusinessStates { get; private set; }

		public override void CreateItems(object param)
		{
            if (BusinessStates == null)
            {
                BusinessStates = new List<BusinessDayType>();
                BusinessStates.AddRange(new List<BusinessDayType>() { 
                    BusinessDayType.Closed, 
                    BusinessDayType.HalfClosed, 
                    BusinessDayType.Open, 
                    BusinessDayType.SpecialDay1, 
                    BusinessDayType.SpecialDay2, 
                    BusinessDayType.SpecialDay3 });
            }

            var viewModels = new ObservableCollection<HolidayVm>();
            foreach (var model in HolidayDataService.GetAll())
			{
                viewModels.Add(new HolidayVm(model, Access, HolidayDataService));
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
        public HolidayDataService HolidayDataService { get; set; }

        #endregion

        #region Methods

        /// <summary>
		/// Initializes a new instance of the <see cref="WorkProfilesVM"/> class.
        /// </summary>
        public HolidaysVM(AccessType access)
            : base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            HolidayDataService = new HolidayDataService();
            HolidayDataService.HolidayAdded += OnHolidayAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Name",0, false), 
                new ColumnInfo("Date",1, false), 
                new ColumnInfo("BusinessState", "txtBusinessDayState",2, true), 
                new ColumnInfo("IsRecurrent", "txtIsRecurrent",3, true), 
            };

            AddCommand = new Command(Add, CanAdd);
			RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);
			ResetCommand = new Command(Reset, CanReset);
            CreateItems(null);
        }

		public override void Add(object parameter)
		{
            if (CurrentContent == null || CurrentContent is HolidayVm)
			{
                HolidayVm.CreateNew(HolidayDataService);
			}
		}
		public override void View(object content)
		{
            if (content is HolidayVm)
			{
                var vm = content as HolidayVm;
				CurrentContent = vm;
			}
		}

		public override void Reset(object oldVm)
		{
			//
		}

        private void OnHolidayAdded(object sender, ModelAddedEventArgs<Holiday> e)
		{
            var newVm = new HolidayVm(e.NewModel, Access, HolidayDataService);
			Items.AddNewItem(newVm);
			Items.CommitNew();
			CurrentContent = newVm;
			CurrentContent.IsSelected = true;
		}


        public override ISplitItemContent CreateClone(ISplitItemContent original)
        {
            var viewModel = original as HolidayVm;
            var model = HolidayDataService.CloneModelById(viewModel.Id);
            return new HolidayVm(model, Access, HolidayDataService);
        }
        #endregion
	}
}
