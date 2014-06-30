using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Soheil.Core.Index;
using Soheil.Common;

namespace Soheil.Core.ViewModels.Index
{
	public class OeeMachineVm : DependencyObject
	{
		public Model.Machine Model { get; set; }
		public event Action Selected;
		public OeeMachineVm(Model.Machine model)
		{
			Model = model;
			Name = model.Name;
			Code = model.Code;
			_data = new List<OeeRecord>();

			SelectCommand = new Commands.Command(o =>
			{
				if (Selected != null) Selected();
			});
		}

		/// <summary>
		/// View calls this method to load its index infromation from database
		/// </summary>
		public void Load(OeeType ot)
		{
			Timeline.Clear();
			var dt = DateTime.Now.GetNorooz();
			switch (ot.Interval)
			{
				case OeeType.OeeInterval.Monthly:
					dt = dt.AddDays(ot.MonthStart - 1);
					int months = (int)DateTime.Now.GetPersianMonth();
					for (int i = 0; i < months; i++)
					{
						var vm = new OeeMachineDetailVm();
						vm.Selected += () =>
						{
							if (SelectedDetail != null)
								SelectedDetail.IsSelected = false;
							SelectedDetail = vm;
						};
						Timeline.Add(vm);

						//load data
						var start = dt;
						if (i == months - 1)
						{
							dt = DateTime.Now;
						}
						else
						{
							dt = dt.AddDays(dt.GetPersianMonthDays());
						}
						var end = dt;

						var data = new OeeRecord(Model.Id, start, end);
						data.TimeRange = start.GetPersianMonth().ToString();
						_data.Add(data);
						vm.Load(data);
					}
					break;
				case OeeType.OeeInterval.Weekly:
					dt = dt.AddDays(-(int)dt.GetPersianDayOfWeek());
					int weeks = DateTime.Now.GetPersianWeekOfYear() + 2;
					for (int i = 0; i < weeks; i++)
					{
						var vm = new OeeMachineDetailVm();
						vm.Selected += () =>
						{
							if (SelectedDetail != null)
								SelectedDetail.IsSelected = false;
							SelectedDetail = vm;
						};
						Timeline.Add(vm);

						//load data
						var start = dt;
						if (i == weeks - 1)
						{
							dt = DateTime.Now;
						}
						else
						{
							dt = dt.AddDays(7);
						}
						var end = dt;

						var data = new OeeRecord(Model.Id, start, end);
						data.TimeRange = string.Format("از {0} الی {1}", start.ToPersianDateString(), end.ToPersianDateString());
						_data.Add(data);
						vm.Load(data);
					}
					break;
				case OeeType.OeeInterval.Daily:
					dt = dt.Add(ot.DayStart);
					int days = DateTime.Now.GetPersianDayOfYear();
					for (int i = 0; i < days; i++)
					{
						var vm = new OeeMachineDetailVm();
						vm.Selected += () =>
						{
							if (SelectedDetail != null)
								SelectedDetail.IsSelected = false;
							SelectedDetail = vm;
						};
						Timeline.Add(vm);

						//load data
						var start = dt;
						if (i == days - 1)
						{
							dt = DateTime.Now;
						}
						else
						{
							dt = dt.AddDays(1);
						}
						var end = dt;

						var data = new OeeRecord(Model.Id, start, end);
						data.TimeRange = start.ToPersianCompactDateString();
						_data.Add(data);
						vm.Load(data);
					}
					break;
				default:
					break;
			}


			if (Timeline.Any()) Timeline.LastOrDefault().SelectCommand.Execute();
		}

		#region Machine props as an item from list
		/// <summary>
		/// Get or sets the bindable name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(OeeMachineVm), new PropertyMetadata(""));
		/// <summary>
		/// Gets or sets a bindable value that indicates Code
		/// </summary>
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(OeeMachineVm), new PropertyMetadata(""));

		/// <summary>
		/// Gets or sets a bindable command to Select a machine
		/// </summary>
		public Commands.Command SelectCommand
		{
			get { return (Commands.Command)GetValue(SelectCommandProperty); }
			set { SetValue(SelectCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectCommandProperty =
			DependencyProperty.Register("SelectCommand", typeof(Commands.Command), typeof(OeeMachineVm), new PropertyMetadata(null)); 
		#endregion


		#region Timeline and data
		List<OeeRecord> _data;

		public ObservableCollection<OeeMachineDetailVm> Timeline { get { return _timeline; } }
		private ObservableCollection<OeeMachineDetailVm> _timeline = new ObservableCollection<OeeMachineDetailVm>();

		/// <summary>
		/// Gets or sets a bindable value that indicates SelectedDetail
		/// </summary>
		public OeeMachineDetailVm SelectedDetail
		{
			get { return (OeeMachineDetailVm)GetValue(SelectedDetailProperty); }
			set { SetValue(SelectedDetailProperty, value); }
		}
		public static readonly DependencyProperty SelectedDetailProperty =
			DependencyProperty.Register("SelectedDetail", typeof(OeeMachineDetailVm), typeof(OeeMachineVm), new PropertyMetadata(null));

		#endregion
	}
}
