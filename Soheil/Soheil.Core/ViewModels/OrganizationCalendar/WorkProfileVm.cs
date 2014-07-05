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
	/// <summary>
	/// ViewModel for a <see cref="Soheil.Model.WorkProfile"/>
	/// </summary>
	public class WorkProfileVm : ItemContentViewModel
	{
		public WorkProfileDataService WorkProfileDataService { get; private set; }
		public Dal.SoheilEdmContext UOW { get; private set; }

        private Model.WorkProfile _model;

		#region Ctor, init

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkProfileVm"/> class initialized with default values.
		/// </summary>
		public WorkProfileVm(AccessType access)
			: this(null, access)
		{
			//_model filled in InitializeData()
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WorkProfileVm"/> class from the model.
		/// </summary>
		/// <param name="entity">The model.</param>
		/// <param name="access"></param>
		/// <param name="dataService"></param>
		public WorkProfileVm(WorkProfile entity, AccessType access)
			: base(access)
		{
			IsReadonly = (access < AccessType.Update);
			_model = entity;
			InitializeData();
		}

		/// <summary>
		/// Initializes all commands and Shift colors and, at last, Loads data
		/// </summary>
		/// <param name="dataService"></param>
		private void InitializeData()
		{
			UOW = new Dal.SoheilEdmContext();
			WorkProfileDataService = new WorkProfileDataService(UOW);
			if (_model == null)
			{
				_model = WorkProfile.CreateDefault();
				WorkProfileDataService.AttachModel(_model);
			}
			else
				_model = WorkProfileDataService.GetSingle(_model.Id);


			SaveCommand = new Command(Save, CanSave);

			AskForIncreaseNumberOfShifts = new Command(o => IsPrototypeChangeAskVisible = true);
			IncreaseNumberOfShifts = new Command(o => { IsPrototypeChangeAskVisible = false; NumberOfShiftsModifier++; });

			ShiftColors.Add(new ShiftColorVm { Color = DefaultColors.Shift.Day });
			ShiftColors.Add(new ShiftColorVm { Color = DefaultColors.Shift.Evening });
			ShiftColors.Add(new ShiftColorVm { Color = DefaultColors.Shift.Night });
			ShiftColors.Add(new ShiftColorVm { Color = DefaultColors.Shift.Reserve1 });
			ShiftColors.Add(new ShiftColorVm { Color = DefaultColors.Shift.Reserve2 });
			ShiftColors.Add(new ShiftColorVm { Color = DefaultColors.Shift.Reserve3 });

			//fill vm with _model data
			Load();

			ShiftPrototypes.CollectionChanged += (s, e) =>
			{
				if (e.NewItems != null)
					foreach (WorkShiftPrototypeVm proto in e.NewItems)
					{
						_model.WorkShiftPrototypes.Add(proto.Model);
					}
				if (e.OldItems != null)
					foreach (WorkShiftPrototypeVm proto in e.OldItems)
					{
						_model.WorkShiftPrototypes.Remove(proto.Model);
					}
			};

		}

		/// <summary>
		/// Corrects Shifts and Prototypes info to match the desired count
		/// </summary>
		/// <param name="desiredCount">Target number of shifts in this profile</param>
		private void correctShifts(int desiredCount)
		{
			//changing ShiftPrototypes makes ShiftPrototype.Index akin to errors
			//-> Correct Indices
			var arr = ShiftPrototypes.OrderBy(x => x.Index).ToArray();
			for (byte i = 0; i < arr.Length; i++)
				arr[i].Index = i;

			//add or remove shift prototypes to/from model and viewModel
			//-> correct shiftPrototypes
			while (desiredCount != _model.WorkShiftPrototypes.Count)
			{
				if (desiredCount > _model.WorkShiftPrototypes.Count)
				{
					//add a new proto until the desired number of protos are reached
					//each added item have its index and other props properly propped!
					//-> add ShiftPrototype
					var protoModel = WorkShiftPrototype.CreateDefault(_model);
					var protoVm = new WorkShiftPrototypeVm(protoModel, ShiftColors);
					_model.WorkShiftPrototypes.Add(protoModel);
					ShiftPrototypes.Add(protoVm);

					//After adding to shiftPrototypes, WorkShifts of each WorkDay have to be corrected too
					//-> add WorkShifts
					foreach (var day in _model.WorkDays)
					{
						//find workDayVm from day model
						var workDayVm = WorkDays.First(x => x.Id == day.Id);

						//add new shifts to workDay vm and workDay model
						if (day.WorkShifts.Any(x => x.WorkShiftPrototype.Index == protoModel.Index)) continue;
						var shift = WorkShift.CreateDefault(day, protoModel);
						day.WorkShifts.Add(shift);
						workDayVm.Shifts.Add(new WorkShiftVm(shift, protoVm));
					}
				}
				else
				{
					//remove extra shift prototypes from model and viewModel
					//remove a new proto until the desired number of protos are reached
					//-> remove ShiftPrototype
					var protoModel = _model.WorkShiftPrototypes.OrderBy(x => x.Index).Last();
					_model.WorkShiftPrototypes.Remove(protoModel);
					ShiftPrototypes.RemoveWhere(x => x.Index == protoModel.Index);

					//After correcting shiftPrototypes, WorkShifts of each WorkDay have to be corrected too
					//-> remove WorkShifts
					foreach (var day in _model.WorkDays)
					{
						//find workDayVm from day model
						var workDayVm = WorkDays.First(x => x.Id == day.Id);

						//remove extra shifts from workDay vm and workDay model
						var shift = day.WorkShifts.First(x => x.WorkShiftPrototype.Index == protoModel.Index);
						day.WorkShifts.Remove(shift);
						workDayVm.Shifts.Remove(workDayVm.Shifts.OrderBy(x => x.Prototype.Index).Last());
					}
				}
			}
		}
		#endregion

		#region Methods (Load,Save,Reset)

		/// <summary>
		/// Loads everything for this work profile
		/// </summary>
		internal void Load()
		{
			foreach (var proto in _model.WorkShiftPrototypes)
			{
				ShiftPrototypes.Add(new WorkShiftPrototypeVm(proto,  ShiftColors));
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
				dow.BusinessStateChanged += (dayIndex, state) => _model.SetBusinessState(dayIndex, state);
				dow.WeekStartChanged += (dayIndex, isWeekStart) =>
				{
					if (isWeekStart)
					{
						_model.WeekStartNr = (byte)dayIndex;
						for (int d = 0; d < 7; d++)
						{
							if (d != (byte)dayIndex)
								Week[d].IsWeekStart = false;
						}
					}
					else
					{
						//cancel if no other day is weekstart
						if (_model.WeekStartNr == (byte)dayIndex)
							Week[(byte)dayIndex].IsWeekStart = true;
					}
				};
				Week.Add(dow);
			}
			correctShifts(_model.WorkShiftPrototypes.Count);
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
			return (Access >= AccessType.Update) && AllDataValid() && base.CanSave();
		}
		#endregion

		//props

		#region Common basic props (Name,Id,Search...)
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

		/// <summary>
		/// Gets an observable collection for openness state of days (count=6) this is for shifts and etc...
		/// </summary>
		public ObservableCollection<WorkDayVm> WorkDays { get { return _workDays; } }
		private ObservableCollection<WorkDayVm> _workDays = new ObservableCollection<WorkDayVm>();
		/// <summary>
		/// <para>Gets an observable collection for the whole week (count=7) </para>
		/// <para>used for week start and for businessState of each day</para>
		/// <para>the collection always starts from saturday</para>
		/// </summary>
		public ObservableCollection<DayOfWeekVm> Week { get { return _week; } }
		private ObservableCollection<DayOfWeekVm> _week = new ObservableCollection<DayOfWeekVm>();

		/// <summary>
		/// Gets an observable collection of Shift prototypes to add or remove or modify
		/// </summary>
		public ObservableCollection<WorkShiftPrototypeVm> ShiftPrototypes { get { return _shiftPrototypes; } }
		private ObservableCollection<WorkShiftPrototypeVm> _shiftPrototypes = new ObservableCollection<WorkShiftPrototypeVm>();

		/// <summary>
		/// Gets an observable collection of Shift Colors for comboboxes
		/// </summary>
		public ObservableCollection<ShiftColorVm> ShiftColors { get { return _shiftColors; } }
		private ObservableCollection<ShiftColorVm> _shiftColors = new ObservableCollection<ShiftColorVm>();

		//IsReadonly Dependency Property
		public bool IsReadonly
		{
			get { return (bool)GetValue(IsReadonlyProperty); }
			set { SetValue(IsReadonlyProperty, value); }
		}
		public static readonly DependencyProperty IsReadonlyProperty =
			DependencyProperty.Register("IsReadonly", typeof(bool), typeof(WorkProfileVm), new UIPropertyMetadata(false));

		#region Shifts props and commands
		/// <summary>
		/// Gets the number of shifts from a valid model
		/// </summary>
		public int NumberOfShifts
		{
			get { return _model.WorkShiftPrototypes.Count; }
		}

		/// <summary>
		/// Gets or Sets the number of shifts and correct shifts afterwards
		/// </summary>
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

		/// <summary>
		/// Gets or Sets a value indicating an unconfirmed change in prototype (number of shifts)
		/// <para>if a change occurs, user sees a message asking to confirm changes</para>
		/// </summary>
		public bool IsPrototypeChangeAskVisible
		{
			get { return (bool)GetValue(IsPrototypeChangeAskVisibleProperty); }
			set { SetValue(IsPrototypeChangeAskVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsPrototypeChangeAskVisibleProperty =
			DependencyProperty.Register("IsPrototypeChangeAskVisible", typeof(bool), typeof(WorkProfileVm), new UIPropertyMetadata(false));
		/// <summary>
		/// Asks user to confirm increasing the number of shifts by 1
		/// </summary>
		public Command AskForIncreaseNumberOfShifts
		{
			get { return (Command)GetValue(AskForIncreaseNumberOfShiftssProperty); }
			set { SetValue(AskForIncreaseNumberOfShiftssProperty, value); }
		}
		public static readonly DependencyProperty AskForIncreaseNumberOfShiftssProperty =
			DependencyProperty.Register("AskForIncreaseNumberOfShifts", typeof(Command), typeof(WorkProfileVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Instantly increases the number of shifts by 1 (adds 1 extra shift by correcting shifts afterwards)
		/// </summary>
		public Command IncreaseNumberOfShifts
		{
			get { return (Command)GetValue(IncreaseNumberOfShiftsProperty); }
			set { SetValue(IncreaseNumberOfShiftsProperty, value); }
		}
		public static readonly DependencyProperty IncreaseNumberOfShiftsProperty =
			DependencyProperty.Register("IncreaseNumberOfShifts", typeof(Command), typeof(WorkProfileVm), new UIPropertyMetadata(null));
		#endregion


		#region Static Methods (Create New)
		public static WorkProfile CreateNew(WorkProfileDataService dataService)
		{
			int id = dataService.AddModel(WorkProfile.CreateDefault());
			return dataService.GetSingle(id);
		}
		#endregion
	}
}
