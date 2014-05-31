using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP
{
	/// <summary>
	/// ViewModel for Job List in PPTable
	/// </summary>
	public class JobListVm : DependencyObject
	{
		/// <summary>
		/// Occurs when a job is selected
		/// </summary>
		public event Action<JobListItemVm> JobSelected;

		/// <summary>
		/// Creates an instance of JobListVm and sets the date range
		/// </summary>
		/// <param name="jobDataService">data service to use in this vm</param>
		public JobListVm()
		{
			initializeCommands();

			//reset dates
			var dt = DateTime.Now.Date;
			dt = dt.AddDays(1 - dt.GetPersianDayOfMonth());
			StartDate = dt;
			EndDate = dt.AddDays(dt.GetPersianMonthDays());
		}
		/// <summary>
		/// Reloads all jobs within current date range
		/// </summary>
		void reloadJobs()
		{
			using (var uow = new Dal.SoheilEdmContext())
			{
				var jobs = new DataServices.JobDataService(uow).GetInRange(StartDate, EndDate, ByDefinition);
				Jobs.Clear();
				foreach (var job in jobs)
				{
					var jobItemVm = new JobListItemVm(job);
					Jobs.Add(jobItemVm);
					jobItemVm.JobSelected += id => { if (JobSelected != null) JobSelected(jobItemVm); };
				}
			}
		}

		/// <summary>
		/// Gets a bindable collection of Job items
		/// </summary>
		public ObservableCollection<JobListItemVm> Jobs { get { return _jobs; } }
		private ObservableCollection<JobListItemVm> _jobs = new ObservableCollection<JobListItemVm>();
		
		/// <summary>
		/// Gets or sets the selected job in items
		/// </summary>
		public JobListItemVm SelectedJob
		{
			get { return (JobListItemVm)GetValue(SelectedJobProperty); }
			set { SetValue(SelectedJobProperty, value); }
		}
		public static readonly DependencyProperty SelectedJobProperty =
			DependencyProperty.Register("SelectedJob", typeof(JobListItemVm), typeof(JobListVm), new UIPropertyMetadata(null, (d, e) => {
				var val = e.NewValue as JobListItemVm;
				if (val != null) val.SelectCommand.Execute(null);
			}));

		/// <summary>
		/// Gets or sets the bindable start date
		/// </summary>
		public DateTime StartDate
		{
			get { return (DateTime)GetValue(StartDateProperty); }
			set { SetValue(StartDateProperty, value); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(JobListVm), new UIPropertyMetadata(DateTime.Now));
		/// <summary>
		/// Gets or sets the bindable end date
		/// </summary>
		public DateTime EndDate
		{
			get { return (DateTime)GetValue(EndDateProperty); }
			set { SetValue(EndDateProperty, value); }
		}
		public static readonly DependencyProperty EndDateProperty =
			DependencyProperty.Register("EndDate", typeof(DateTime), typeof(JobListVm), new UIPropertyMetadata(DateTime.Now));

		/// <summary>
		/// Gets or sets a bindable value that indicates whether load job by their definition values
		/// </summary>
		/// <remarks>definition values are Release date time and Deadline, 
		/// if this value is set to true Definition values are in effect.
		/// which means the job is shown if it is partially or completely is in the given range.
		/// if this value is set to false Blocks range will be in effect.
		/// which means the job is shown only if any of its Blocks are (partially or completely) in the given range</remarks>
		public bool ByDefinition
		{
			get { return (bool)GetValue(ByDefinitionProperty); }
			set { SetValue(ByDefinitionProperty, value); }
		}
		public static readonly DependencyProperty ByDefinitionProperty =
			DependencyProperty.Register("ByDefinition", typeof(bool), typeof(JobListVm), new UIPropertyMetadata(true));

		/// <summary>
		/// Gets or sets a value that indicates whether the job list is visible
		/// <para>Reloads jobs if set to true, deselect the selected job if set to false</para>
		/// </summary>
		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsVisibleProperty =
			DependencyProperty.Register("IsVisible", typeof(bool), typeof(JobListVm), new UIPropertyMetadata(false, (d, e) =>
			{
				var vm = d as JobListVm;
				if ((bool)e.NewValue)
				{
					vm.reloadJobs();
				}
				else
				{
					if (vm.JobSelected != null) vm.JobSelected(null);//deselect job
				}
			}));

		#region Commands
		void initializeCommands()
		{
			SearchCommand = new Commands.Command(o => reloadJobs());
		}
		/// <summary>
		/// Gets a bindable command that reload jobs in the specified range
		/// </summary>
		public Commands.Command SearchCommand
		{
			get { return (Commands.Command)GetValue(SearchCommandProperty); }
			protected set { SetValue(SearchCommandProperty, value); }
		}
		public static readonly DependencyProperty SearchCommandProperty =
			DependencyProperty.Register("SearchCommand", typeof(Commands.Command), typeof(JobListVm), new UIPropertyMetadata(null));
		#endregion
	}
}
