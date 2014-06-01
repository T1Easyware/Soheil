using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Report
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
			DependencyProperty.Register("AddCommand", typeof(Commands.Command), typeof(NPTReportVm), new UIPropertyMetadata(null));
		//FocusCommand Dependency Property
		public Commands.Command FocusCommand
		{
			get { return (Commands.Command)GetValue(FocusCommandProperty); }
			set { SetValue(FocusCommandProperty, value); }
		}
		public static readonly DependencyProperty FocusCommandProperty =
			DependencyProperty.Register("FocusCommand", typeof(Commands.Command), typeof(NPTReportVm), new UIPropertyMetadata(null));
		//CancelCommand Dependency Property
		public Commands.Command CancelCommand
		{
			get { return (Commands.Command)GetValue(CancelCommandProperty); }
			set { SetValue(CancelCommandProperty, value); }
		}
		public static readonly DependencyProperty CancelCommandProperty =
			DependencyProperty.Register("CancelCommand", typeof(Commands.Command), typeof(NPTReportVm), new UIPropertyMetadata(null));
		#endregion

		#region Start/End/Duration
		//DurationSeconds Dependency Property
		public int DurationSeconds
		{
			get { return (int)GetValue(DurationSecondsProperty); }
			set { SetValue(DurationSecondsProperty, value); }
		}
		public static readonly DependencyProperty DurationSecondsProperty =
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(NPTReportVm),
			new PropertyMetadata(0, (d, e) =>
			{
				var vm = d as NPTReportVm;
			}));
		//Duration Dependency Property
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(NPTReportVm), new UIPropertyMetadata(TimeSpan.Zero));

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
			}));
		public DateTime EndDateTime
		{
			get { return EndDate.Add(EndTime); }
			set { EndDate = value.Date; EndTime = value.TimeOfDay; }
		}
		#endregion

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
					vm.Parent.Parent.PPTable.CurrentNPTReportBuilder = vm;
				else
					vm.Parent.Parent.PPTable.CurrentNPTReportBuilder = null;
			}));
	}
}
