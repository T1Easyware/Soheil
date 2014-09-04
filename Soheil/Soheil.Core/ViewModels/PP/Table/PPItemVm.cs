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
	/// <summary>
	/// An abstract class suitable for any kind of Item that can (directly/indirectly) reside inside PPTable
	/// <para>
	/// <example>Example: BlockVm, TaskVm, NPTVm</example>
	/// </para>
	/// </summary>
	public abstract class PPItemVm : ViewModelBase
	{
		public event Action<PPViewMode> ViewModeChanged;
		public event Action<DateTime> StartDateTimeChanged;
		public event Action<int> DurationSecondsChanged;
		protected PPItemVm() { Message = new EmbeddedException(); }

		/// <summary>
		/// Gets or sets the bindable error message
		/// </summary>
		public EmbeddedException Message
		{
			get { return (EmbeddedException)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(EmbeddedException), typeof(PPItemVm), new UIPropertyMetadata(null));

		#region Members

		/// <summary>
		/// Gets the unit of work
		/// </summary>
		public Dal.SoheilEdmContext UOW { get; protected set; }

		/// <summary>
		/// Vertical Index of Item within its container 
		/// <para>Could be station or SSA...</para>
		/// <para>does not apply for task</para>
		/// </summary>
		public int RowIndex { get; set; }
		/// <summary>
		/// Id of Model associated with this VM (This field must be overriden)
		/// <para>Could be TaskId, NPTId, ProcessReportId...</para>
		/// </summary>
		public abstract int Id { get; }

		/// <summary>
		/// Gets or sets the bindable Start dateTime
		/// </summary>
		public DateTime StartDateTime
		{
			get { return (DateTime)GetValue(StartDateTimeProperty); }
			set { SetValue(StartDateTimeProperty, value); }
		}
		public static readonly DependencyProperty StartDateTimeProperty =
			DependencyProperty.Register("StartDateTime", typeof(DateTime), typeof(PPItemVm),
			new UIPropertyMetadata(DateTime.Now, (d, e) => 
			{ if (((PPItemVm)d).StartDateTimeChanged != null) ((PPItemVm)d).StartDateTimeChanged((DateTime)e.NewValue); }));
		/// <summary>
		/// Gets or sets the bindable duration seconds
		/// </summary>
		public int DurationSeconds
		{
			get { return (int)GetValue(DurationSecondsProperty); }
			set { SetValue(DurationSecondsProperty, value); }
		}
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(PPItemVm), new UIPropertyMetadata(TimeSpan.Zero));
		public static readonly DependencyProperty DurationSecondsProperty =
			DependencyProperty.Register("DurationSeconds", typeof(int), typeof(PPItemVm),
			new UIPropertyMetadata(0, (d, e) =>{
				d.SetValue(DurationProperty, TimeSpan.FromSeconds((int)e.NewValue));
				if (((PPItemVm)d).DurationSecondsChanged != null) ((PPItemVm)d).DurationSecondsChanged((int)e.NewValue);
			}));
		/// <summary>
		/// Gets or sets the bindable view mode
		/// </summary>
		public PPViewMode ViewMode
		{
			get { return (PPViewMode)GetValue(ViewModeProperty); }
			set { SetValue(ViewModeProperty, value); }
		}
		public static readonly DependencyProperty ViewModeProperty =
			DependencyProperty.Register("ViewMode", typeof(PPViewMode), typeof(PPItemVm),
			new UIPropertyMetadata(PPViewMode.Acquiring, (d, e) => { 
				if (((PPItemVm)d).ViewModeChanged != null) 
					((PPItemVm)d).ViewModeChanged((PPViewMode)e.NewValue);
				var vm = d as BlockVm;
				if (vm != null)
					if (vm.BlockReport != null)
						vm.BlockReport.CorrectTransactions();
			})); 
		#endregion

		#region Commands
		/// <summary>
		/// Gets or sets a bindable command that edits the report
		/// </summary>
		public Commands.Command EditItemCommand
		{
			get { return (Commands.Command)GetValue(EditItemCommandProperty); }
			set { SetValue(EditItemCommandProperty, value); }
		}
		public static readonly DependencyProperty EditItemCommandProperty =
			DependencyProperty.Register("EditItemCommand", typeof(Commands.Command), typeof(PPItemVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command that reloads all related reports
		/// </summary>
		public Commands.Command EditReportCommand
		{
			get { return (Commands.Command)GetValue(EditReportCommandProperty); }
			set { SetValue(EditReportCommandProperty, value); }
		}
		public static readonly DependencyProperty EditReportCommandProperty =
			DependencyProperty.Register("EditReportCommand", typeof(Commands.Command), typeof(PPItemVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command that deletes the report
		/// </summary>
		public Commands.Command DeleteItemCommand
		{
			get { return (Commands.Command)GetValue(DeleteTaskCommandProperty); }
			set { SetValue(DeleteTaskCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteTaskCommandProperty =
			DependencyProperty.Register("DeleteItemCommand", typeof(Commands.Command), typeof(PPItemVm), new UIPropertyMetadata(null));


		#endregion
	}
}
