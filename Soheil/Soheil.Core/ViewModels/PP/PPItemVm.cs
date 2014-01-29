using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;
using Soheil.Common.SoheilException;
using Soheil.Core.Base;

namespace Soheil.Core.ViewModels.PP
{
	public abstract class PPItemVm : ViewModelBase
	{
		protected PPItemVm() { Message = new EmbeddedException(); _threadLock = new object(); }

		//Message Dependency Property
		public EmbeddedException Message
		{
			get { return (EmbeddedException)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(EmbeddedException), typeof(PPItemVm), new UIPropertyMetadata(null));

		#region Members
		/// <summary>
		/// Vertical Index of Item within its container 
		/// <para>Could be station or SSA...</para>
		/// <para>does not apply for task</para>
		/// </summary>
		public int RowIndex { get; protected set; }
		/// <summary>
		/// Id of Model associated with this VM (This field must be overriden)
		/// <para>Could be TaskId, NPTId, ProcessReportId...</para>
		/// </summary>
		public abstract int Id { get; }

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
		public PPViewMode ViewMode
		{
			get { return (PPViewMode)GetValue(ViewModeProperty); }
			set { SetValue(ViewModeProperty, value); }
		}
		public static readonly DependencyProperty ViewModeProperty =
			DependencyProperty.Register("ViewMode", typeof(PPViewMode), typeof(PPItemVm), new UIPropertyMetadata(PPViewMode.Acquiring)); 
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
		protected int _tries;
		protected static int _MAX_TRIES = 10;

		//Main Functions
		/// <summary>
		/// When Items are loaded their event calls this method
		/// </summary>
		public void BeginAcquisition()
		{
			try
			{
				ViewMode = PPViewMode.Acquiring;
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
		/// <summary>
		/// When Items are unloaded their event calls this method
		/// </summary>
		public void UnloadData()
		{
			ViewMode = PPViewMode.Simple;
			_acqusitionThread.ForceQuit();
			if (_delayAcquisitor != null) _delayAcquisitor.Dispose();
		}

		protected virtual void acqusitionThreadStart() { }
		protected virtual void acqusitionThreadEnd() { }
		#endregion
	}
}
