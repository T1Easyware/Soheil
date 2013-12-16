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
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Model;

namespace Soheil.Core.ViewModels.OrganizationCalendar
{
	public class WorkProfileVm : ItemContentViewModel
	{
		#region Properties

        private Model.WorkProfile _model;

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

		public int NumberOfShifts
		{
			get { return _model.WorkShiftPrototypes.Count; }
		}

		#region Create/Modify Properties
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
		#endregion

		#region Number of shifts
		//NumberOfShiftsModifier Dependency Property
		public int NumberOfShiftsModifier
		{
			get { return (int)GetValue(NumberOfShiftsProperty); }
			set { SetValue(NumberOfShiftsProperty, value); }
		}
		public static readonly DependencyProperty NumberOfShiftsProperty =
			DependencyProperty.Register("NumberOfShiftsModifier", typeof(int), typeof(WorkProfileVm),
			new UIPropertyMetadata(1, (d, e) =>
			{
				var vm = (WorkProfileVm)d;
				var val = (int)e.NewValue;
				vm.correctShifts(val);
			}, (d, v) =>
			{
				if ((int)v < 1) return 1;
				if ((int)v > 5) return 5;
				return v;
			}));
		private void correctShifts(int value)
		{
			//recorrect indices
			var arr = ShiftPrototypes.OrderBy(x => x.Index).ToArray();
			for (byte i = 0; i < arr.Length; i++)
				arr[i].Index = i;

			//add protos
			while (value > _model.WorkShiftPrototypes.Count)
			{
				//add a new proto until the desired number of protos are reached
				//each added item have its index and other props properly propped
				var protoModel = WorkShiftPrototype.CreateDefault(_model);
				_model.WorkShiftPrototypes.Add(protoModel);
				ShiftPrototypes.Add(new WorkShiftPrototypeVm(protoModel, ShiftColors));
			}

			//correct WorkShifts
			foreach (var day in _model.WorkDays)
			{
				var workDayVm = WorkDays.First(x => x.Id == day.Id);

				//remove extra shifts
				var workShiftVms = workDayVm.Shifts.OrderBy(x => x.Prototype.Index).ToArray();
				for (int i = value; i < workShiftVms.Length; i++)
				{
					var shift = day.WorkShifts.First(x => x.WorkShiftPrototype.Index == i);
					day.WorkShifts.Remove(shift);
					workDayVm.Shifts.Remove(workShiftVms[i]);
				}

				//add new shifts
				var protoArray = _model.WorkShiftPrototypes.OrderBy(x => x.Index).ToArray();
				for (int i = value; i < protoArray.Length; i++)
				{
					if (day.WorkShifts.Any(x => x.WorkShiftPrototype.Index == i)) continue;
					var shift = WorkShift.CreateDefault(day, protoArray[i]);
					day.WorkShifts.Add(shift);
					workDayVm.Shifts.Add(new WorkShiftVm(shift, ShiftPrototypes.First(x => x.Index == i)));
				}
			}

			//remove protos
			while (value < _model.WorkShiftPrototypes.Count)
			{
				//add a new proto until the desired number of protos are reached
				//each added item have its index and other props properly propped
				var protoModel = _model.WorkShiftPrototypes.OrderBy(x => x.Index).Last();
				_model.WorkShiftPrototypes.Remove(protoModel);
				ShiftPrototypes.RemoveWhere(x => x.Index == protoModel.Index);
			}
		}
		//IsPrototypeChangeAskVisible Dependency Property
		public bool IsPrototypeChangeAskVisible
		{
			get { return (bool)GetValue(IsPrototypeChangeAskVisibleProperty); }
			set { SetValue(IsPrototypeChangeAskVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsPrototypeChangeAskVisibleProperty =
			DependencyProperty.Register("IsPrototypeChangeAskVisible", typeof(bool), typeof(WorkProfileVm), new UIPropertyMetadata(false));
		//AskForDecreaseNumberOfShifts Dependency Property
		public Command AskForDecreaseNumberOfShifts
		{
			get { return (Command)GetValue(AskForDecreaseNumberOfShiftsProperty); }
			set { SetValue(AskForDecreaseNumberOfShiftsProperty, value); }
		}
		public static readonly DependencyProperty AskForDecreaseNumberOfShiftsProperty =
			DependencyProperty.Register("AskForDecreaseNumberOfShifts", typeof(Command), typeof(WorkProfileVm), new UIPropertyMetadata(null));
		//DecreaseNumberOfShifts Dependency Property
		public Command DecreaseNumberOfShifts
		{
			get { return (Command)GetValue(DecreaseNumberOfShiftsProperty); }
			set { SetValue(DecreaseNumberOfShiftsProperty, value); }
		}
		public static readonly DependencyProperty DecreaseNumberOfShiftsProperty =
			DependencyProperty.Register("DecreaseNumberOfShifts", typeof(Command), typeof(WorkProfileVm), new UIPropertyMetadata(null));
		//IncreaseNumberOfShifts Dependency Property
		public Command IncreaseNumberOfShifts
		{
			get { return (Command)GetValue(IncreaseNumberOfShiftsProperty); }
			set { SetValue(IncreaseNumberOfShiftsProperty, value); }
		}
		public static readonly DependencyProperty IncreaseNumberOfShiftsProperty =
			DependencyProperty.Register("IncreaseNumberOfShifts", typeof(Command), typeof(WorkProfileVm), new UIPropertyMetadata(null)); 
		#endregion

		//ShiftPrototypes Observable Collection
		private ObservableCollection<WorkShiftPrototypeVm> _shiftPrototypes = new ObservableCollection<WorkShiftPrototypeVm>();
		public ObservableCollection<WorkShiftPrototypeVm> ShiftPrototypes { get { return _shiftPrototypes; } }

		//ShiftColors Observable Collection
		private ObservableCollection<ShiftColorVm> _shiftColors = new ObservableCollection<ShiftColorVm>();
		public ObservableCollection<ShiftColorVm> ShiftColors { get { return _shiftColors; } }

		#region Days
		/// <summary>
		/// Types of days (count=6) this is for shifts and etc...
		/// </summary>
		private ObservableCollection<WorkDayVm> _workDays = new ObservableCollection<WorkDayVm>();
		public ObservableCollection<WorkDayVm> WorkDays { get { return _workDays; } }
		/// <summary>
		/// <para>The whole week (count=7) </para>
		/// <para>this is for week start and for businessState of each day</para>
		/// <para>always starts from saturday</para>
		/// </summary>
		private ObservableCollection<DayOfWeekVm> _week = new ObservableCollection<DayOfWeekVm>();
		public ObservableCollection<DayOfWeekVm> Week { get { return _week; } }

		void dayOfWeek_BusinessStateChanged(object sender, Soheil.Core.ViewModels.OrganizationCalendar.DayOfWeekVm.PropertyChangedEventArgs e)
		{
			_model.SetBusinessState(e.Index, e.State);
		}
		void dayOfWeek_WeekStartChanged(object sender, Soheil.Core.ViewModels.OrganizationCalendar.DayOfWeekVm.PropertyChangedEventArgs e)
		{
			if (e.IsWeekStart)
			{
				_model.WeekStartNr = (byte)e.Index;
				for (int i = 0; i < 7; i++)
				{
					if (i != (byte)e.Index)
						Week[i].IsWeekStart = false;
				}
			}
			else
			{
				//cancel if no other day is weekstart
				if (_model.WeekStartNr == (byte)e.Index)
					Week[(byte)e.Index].IsWeekStart = true;
			}
		} 
		#endregion

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
        /// Initializes a new instance of the <see cref="WorkProfileVm"/> class initialized with default values.
        /// </summary>
        public WorkProfileVm(AccessType access, WorkProfileDataService dataService)
            : base(access)
        {
			_model = WorkProfile.CreateDefault();
			InitializeData(dataService);
        }

	    /// <summary>
        /// Initializes a new instance of the <see cref="WorkProfileVm"/> class from the model.
	    /// </summary>
	    /// <param name="entity">The model.</param>
	    /// <param name="access"></param>
	    /// <param name="dataService"></param>
		public WorkProfileVm(WorkProfile entity, AccessType access, WorkProfileDataService dataService)
            : base(access)
        {
			_model = entity;
			InitializeData(dataService);
        }

		private void InitializeData(WorkProfileDataService dataService)
        {
			WorkProfileDataService = dataService;
            SaveCommand = new Command(Save, CanSave);

			AskForDecreaseNumberOfShifts = new Command(o => IsPrototypeChangeAskVisible = true);
			DecreaseNumberOfShifts = new Command(o => { IsPrototypeChangeAskVisible = false; NumberOfShiftsModifier--; });
			IncreaseNumberOfShifts = new Command(o => NumberOfShiftsModifier++);

			ShiftColors.Add(new ShiftColorVm { Color = DefaultColors.Shift.Day });
			ShiftColors.Add(new ShiftColorVm { Color = DefaultColors.Shift.Evening });
			ShiftColors.Add(new ShiftColorVm { Color = DefaultColors.Shift.Night });
			ShiftColors.Add(new ShiftColorVm { Color = DefaultColors.Shift.Reserve1 });
			ShiftColors.Add(new ShiftColorVm { Color = DefaultColors.Shift.Reserve2 });
			ShiftColors.Add(new ShiftColorVm { Color = DefaultColors.Shift.Reserve3 });

			//fill vm with _model data
			Load();
		}



        public override void Save(object param)
        {
			WorkProfileDataService.UpdateModel(_model);
			OnPropertyChanged("ModifiedBy");
			OnPropertyChanged("ModifiedDate");
			Mode = ModificationStatus.Saved;
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }
        #endregion

		#region Static Methods
		public static WorkProfile CreateNew(WorkProfileDataService dataService)
		{
			int id = dataService.AddModel(WorkProfile.CreateDefault());
			return dataService.GetSingle(id);
		}
		#endregion

		internal void Load()
		{
			foreach (var proto in _model.WorkShiftPrototypes)
			{
				ShiftPrototypes.Add(new WorkShiftPrototypeVm(proto, ShiftColors));
			}
			foreach (var workDay in _model.WorkDays)
			{
				WorkDays.Add(new WorkDayVm(workDay, ShiftPrototypes));
			}
			for (int i = 0; i < 7; i++)
			{
				var stateEnum = _model.GetBusinessState(i);
				var dow = new DayOfWeekVm(i, WorkDays.First(x => x.BusinessState == stateEnum));
				dow.State = stateEnum;
				dow.IsWeekStart = (_model.WeekStartNr == i);
				//set the listeners
				dow.BusinessStateChanged += dayOfWeek_BusinessStateChanged;
				dow.WeekStartChanged += dayOfWeek_WeekStartChanged;
				Week.Add(dow);
			}
			correctShifts(_model.WorkShiftPrototypes.Count);
		}
		internal void Reset()
		{
			WorkProfileDataService.Postpone(_model);
		}
	}
}
