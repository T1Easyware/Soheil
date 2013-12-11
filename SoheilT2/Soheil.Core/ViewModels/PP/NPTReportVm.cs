using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class NPTReportVm : DependencyObject
	{
		public NPTReportVm(NPTVm parent)
		{
			Parent = parent;
			DurationSeconds = parent.DurationSeconds;
			EndDateTime = parent.StartDateTime.AddSeconds(parent.DurationSeconds);
			
			AddCommand = new Commands.Command(o =>
			{
				var model = new Model.NonProductiveTaskReport();
				model.ReportDurationSeconds = DurationSeconds;
				IsSelected = false;
			});
			FocusCommand = new Commands.Command(o =>
			{
				IsSelected = true;
			});
			CancelCommand = new Commands.Command(o =>
			{
				IsSelected = false;
			});
		}

		public NPTVm Parent { get; set; }
		
		#region Commands
		//AddCommand Dependency Property
		public Commands.Command AddCommand
		{
			get { return (Commands.Command)GetValue(AddCommandProperty); }
			set { SetValue(AddCommandProperty, value); }
		}
		public static readonly DependencyProperty AddCommandProperty =
			DependencyProperty.Register("AddCommand", typeof(Commands.Command), typeof(TaskReportHolderVm), new UIPropertyMetadata(null));
		//FocusCommand Dependency Property
		public Commands.Command FocusCommand
		{
			get { return (Commands.Command)GetValue(FocusCommandProperty); }
			set { SetValue(FocusCommandProperty, value); }
		}
		public static readonly DependencyProperty FocusCommandProperty =
			DependencyProperty.Register("FocusCommand", typeof(Commands.Command), typeof(TaskReportVm), new UIPropertyMetadata(null));
		//CancelCommand Dependency Property
		public Commands.Command CancelCommand
		{
			get { return (Commands.Command)GetValue(CancelCommandProperty); }
			set { SetValue(CancelCommandProperty, value); }
		}
		public static readonly DependencyProperty CancelCommandProperty =
			DependencyProperty.Register("CancelCommand", typeof(Commands.Command), typeof(TaskReportHolderVm), new UIPropertyMetadata(null));
		#endregion

		#region Start/End/Duration
		//DurationSeconds Dependency Property
		public int DurationSeconds
		{
			get { return (int)GetValue(DurationSecondsProperty); }
			set { SetValue(DurationSecondsProperty, value); }
		}
		public static readonly DependencyProperty DurationSecondsProperty =
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(TaskReportBaseVm),
			new PropertyMetadata(0, (d, e) =>
			{
				var vm = d as TaskReportHolderVm;
				if (vm != null)
				{
					if (!vm.ByEndDate)
						vm.EndDateTime = vm.StartDateTime.AddSeconds((int)e.NewValue);
				}
				d.SetValue(DurationProperty, new TimeSpan((int)e.NewValue * TimeSpan.TicksPerSecond));
			}));
		//Duration Dependency Property
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(TaskReportBaseVm), new UIPropertyMetadata(TimeSpan.Zero));

		//EndDate Dependency Property
		public DateTime EndDate
		{
			get { return (DateTime)GetValue(EndDateProperty); }
			set { SetValue(EndDateProperty, value); }
		}
		public static readonly DependencyProperty EndDateProperty =
			DependencyProperty.Register("EndDate", typeof(DateTime), typeof(NPTReportVm),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = d as NPTReportVm;
				if (vm != null)
				{
					if (vm.ByEndDate)
						vm.DurationSeconds = (int)((DateTime)e.NewValue).Add(vm.EndTime).Subtract(vm.Parent.StartDateTime).TotalSeconds;
				}
			}));
		//EndTime Dependency Property
		public TimeSpan EndTime
		{
			get { return (TimeSpan)GetValue(EndTimeProperty); }
			set { SetValue(EndTimeProperty, value); }
		}
		public static readonly DependencyProperty EndTimeProperty =
			DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(NPTReportVm),
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = d as NPTReportVm;
				if (vm != null)
				{
					if (vm.ByEndDate)
						vm.DurationSeconds = (int)vm.EndDate.Add((TimeSpan)e.NewValue).Subtract(vm.Parent.StartDateTime).TotalSeconds;
				}
			}));
		public DateTime EndDateTime
		{
			get { return EndDate.Add(EndTime); }
			set { EndDate = value.Date; EndTime = value.TimeOfDay; }
		}
		#endregion

		#region Other PropDp
		//ByEndDate Dependency Property
		public bool ByEndDate
		{
			get { return (bool)GetValue(ByEndDateProperty); }
			set { SetValue(ByEndDateProperty, value); }
		}
		public static readonly DependencyProperty ByEndDateProperty =
			DependencyProperty.Register("ByEndDate", typeof(bool), typeof(NPTReportVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (NPTReportVm)d;
				if ((bool)e.NewValue)
					vm.DurationSeconds = (int)vm.EndDateTime.Subtract(vm.Parent.StartDateTime).TotalSeconds;
				else
				{
					var endDt = vm.Parent.StartDateTime.AddSeconds(vm.DurationSeconds);
					vm.EndDate = endDt.Date;
					vm.EndTime = endDt.TimeOfDay;
				}
			}));
		//IsSelected Dependency Property
		/// <summary>
		/// Also Sets CurrentNPTReportBuilder in PPTableVm
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(NPTReportVm),
			new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = (NPTReportVm)d;
				if ((bool)e.NewValue)
					vm.Parent.Parent.Parent.CurrentNPTReportBuilder = vm;
				else
					vm.Parent.Parent.Parent.CurrentNPTReportBuilder = null;
			}));
		//Offset Dependency Property
		public Point Offset
		{
			get { return (Point)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		public static readonly DependencyProperty OffsetProperty =
			DependencyProperty.Register("Offset", typeof(Point), typeof(NPTReportVm), new UIPropertyMetadata(new Point()));
		#endregion
	}
}
