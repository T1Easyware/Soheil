using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Model;
using Soheil.Core.DataServices;
using Soheil.Core.Commands;

namespace Soheil.Core.ViewModels.OrganizationCalendar
{
    public class HolidayVm : ItemContentViewModel
    {
        #region Properties

        private Holiday _model;

        public override int Id
        {
            get { return _model.Id; }
            set { }
        }
        public override string SearchItem { get { return Name; } set { } }

        [LocalizedRequired(ErrorMessageResourceName = @"txtNameRequired")]
        public string Name
        {
            get { return _model.Name; }
            set { _model.Name = value; OnPropertyChanged("Name"); }
        }

        [LocalizedRequired(ErrorMessageResourceName = @"txtDateRequired")]
        public DateTime Date
        {
            get { return _model.Date; }
            set { _model.Date = value; OnPropertyChanged("Date"); }
        }

		[LocalizedRequired(ErrorMessageResourceName = @"txtEntryRequired")]
		public BusinessDayType BusinessState
		{
			get { return _model.BusinessState; }
			set { _model.BusinessState = value; OnPropertyChanged("BusinessState"); }
		}

		public bool IsRecurrent
		{
			get { return _model.IsRecurrent; }
			set { _model.IsRecurrent = value; OnPropertyChanged("IsRecurrent"); }
		}

		public ObservableCollection<BusinessDayType> DayStates { get { return _dayStates; } }
		private ObservableCollection<BusinessDayType> _dayStates = new ObservableCollection<BusinessDayType>();
		public BusinessDayType SelectedState
		{
			get { return (BusinessDayType)GetValue(SelectedStateProperty); }
			set { SetValue(SelectedStateProperty, value); }
		}
		public static readonly DependencyProperty SelectedStateProperty =
			DependencyProperty.Register("SelectedState", typeof(BusinessDayType), typeof(HolidayVm),
			new UIPropertyMetadata(BusinessDayType.None, (d, e) =>
			{
				var vm = (HolidayVm)d;
				var val = (BusinessDayType)e.NewValue;
				vm.BusinessState = val;
			}));

      /*  #region Create/Modify Properties
        public DateTime CreatedDate
        {
            get { return _model.CreatedDate; }
            set { _model.CreatedDate = value; OnPropertyChanged("CreatedDate"); }
        }

        public DateTime ModifiedDate
        {
            get { return _model.ModifiedDate; }
            set { _model.ModifiedDate = value; OnPropertyChanged("ModifiedDate"); }
        }

        public string ModifiedBy
        {
            get { return LoginInfo.GetUsername(_model.ModifiedBy); }
        }
        #endregion*/

        public HolidayDataService HolidayDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayVm"/> class initialized with default values.
        /// </summary>
        public HolidayVm(AccessType access, HolidayDataService dataService)
            : base(access)
        {
            _model = Holiday.CreateDefault();
            InitializeData(dataService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayVm"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public HolidayVm(Holiday entity, AccessType access, HolidayDataService dataService)
            : base(access)
        {
            _model = entity;
            InitializeData(dataService);
        }

        private void InitializeData(HolidayDataService dataService)
        {
            HolidayDataService = dataService;
            SaveCommand = new Command(Save, CanSave);

			//combo box issues
			DayStates.Add(BusinessDayType.Open);
			DayStates.Add(BusinessDayType.Closed);
			DayStates.Add(BusinessDayType.HalfClosed);
			DayStates.Add(BusinessDayType.SpecialDay1);
			DayStates.Add(BusinessDayType.SpecialDay2);
			DayStates.Add(BusinessDayType.SpecialDay3);
			SelectedState = DayStates.FirstOrDefault(x => x == BusinessState);
        }

        public override void Save(object param)
        {
            HolidayDataService.UpdateModel(_model);
            OnPropertyChanged("ModifiedBy");
            OnPropertyChanged("ModifiedDate");
            Mode = ModificationStatus.Saved;
        }

        public override bool CanSave()
        {
			return (Access >= AccessType.Update) && AllDataValid() && base.CanSave();
        }


        #endregion

        #region Static Methods
        public static Holiday CreateNew(HolidayDataService dataService)
        {
            int id = dataService.AddModel(Holiday.CreateDefault());
            return dataService.GetSingle(id);
        }
        #endregion
    }
}
