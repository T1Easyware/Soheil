using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;
using Soheil.Common.SoheilException;

namespace Soheil.Core.ViewModels.PP
{
	public class PPItemVm : EmbeddedException
	{
		protected PPItemVm() { }

		#region Members
		/// <summary>
		/// index of station or other
		/// </summary>
		public int RowIndex { get; protected set; }
		/// <summary>
		/// TaskId or NPTId
		/// </summary>
		public int Id { get; protected set; }

		//StartDateTime Dependency Property
		public DateTime StartDateTime
		{
			get { return (DateTime)GetValue(StartDateTimeProperty); }
			set { SetValue(StartDateTimeProperty, value); }
		}
		public static readonly DependencyProperty StartDateTimeProperty =
			DependencyProperty.Register("StartDateTime", typeof(DateTime), typeof(PPItemVm), new UIPropertyMetadata(DateTime.Now));
		//DurationSeconds Dependency Property
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(PPItemVm), new UIPropertyMetadata(TimeSpan.Zero));
		public int DurationSeconds
		{
			get { return (int)GetValue(DurationSecondsProperty); }
			set { SetValue(DurationSecondsProperty, value); }
		}
		public static readonly DependencyProperty DurationSecondsProperty =
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(PPItemVm),
			new UIPropertyMetadata(0, (d, e) => d.SetValue(DurationProperty, new TimeSpan((int)e.NewValue * TimeSpan.TicksPerSecond))));
		//ViewMode Dependency Property
		public PPTaskViewMode ViewMode
		{
			get { return (PPTaskViewMode)GetValue(ViewModeProperty); }
			set { SetValue(ViewModeProperty, value); }
		}
		public static readonly DependencyProperty ViewModeProperty =
			DependencyProperty.Register("ViewMode", typeof(PPTaskViewMode), typeof(PPItemVm), new UIPropertyMetadata(PPTaskViewMode.Acquiring)); 
		#endregion

		#region Commands
		//EditItemCommand Dependency Property
		public Commands.Command EditItemCommand
		{
			get { return (Commands.Command)GetValue(EditItemCommandProperty); }
			set { SetValue(EditItemCommandProperty, value); }
		}
		public static readonly DependencyProperty EditItemCommandProperty =
			DependencyProperty.Register("EditItemCommand", typeof(Commands.Command), typeof(PPItemVm), new UIPropertyMetadata(null));
		//EditReportCommand Dependency Property
		public Commands.Command EditReportCommand
		{
			get { return (Commands.Command)GetValue(EditReportCommandProperty); }
			set { SetValue(EditReportCommandProperty, value); }
		}
		public static readonly DependencyProperty EditReportCommandProperty =
			DependencyProperty.Register("EditReportCommand", typeof(Commands.Command), typeof(PPItemVm), new UIPropertyMetadata(null));
		//DeleteItemCommand Dependency Property
		public Commands.Command DeleteItemCommand
		{
			get { return (Commands.Command)GetValue(DeleteTaskCommandProperty); }
			set { SetValue(DeleteTaskCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteTaskCommandProperty =
			DependencyProperty.Register("DeleteItemCommand", typeof(Commands.Command), typeof(PPItemVm), new UIPropertyMetadata(null));
		#endregion

		#region Threads and Acquisition
		//Members and Props
		protected Timer _delayAcquisitor;
		protected Thread _acqusitionThread;
		protected static int _acquisitionStartDelay = 100;
		protected static int _acquisitionPeriodicDelay = System.Threading.Timeout.Infinite;//5000???
		protected Object _threadLock;

		//Main Functions
		public void BeginAcquisition()
		{
			try
			{
				ViewMode = PPTaskViewMode.Acquiring;
				_delayAcquisitor = new Timer((s) =>
				{
					try
					{
						Dispatcher.Invoke(() =>
						{
							_acqusitionThread.ForceQuit();
							_acqusitionThread = new Thread(acqusitionThreadStart);
							_acqusitionThread.Priority = ThreadPriority.Lowest;
							_acqusitionThread.Start();
						});
					}
					catch { }
				}, null, _acquisitionStartDelay, _acquisitionPeriodicDelay);
			}
			catch { }
		}
		public void UnloadData()
		{
			ViewMode = PPTaskViewMode.Simple;
			_acqusitionThread.ForceQuit();
			if (_delayAcquisitor != null) _delayAcquisitor.Dispose();
		}

		protected virtual void acqusitionThreadStart() { }
		protected virtual void acqusitionThreadEnd() { }
		#endregion
	}
}
