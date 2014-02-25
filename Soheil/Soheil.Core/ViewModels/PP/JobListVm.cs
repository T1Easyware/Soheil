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
	public class JobListVm : DependencyObject
	{
		public event Action<JobListItemVm> JobSelected;
		DataServices.JobDataService _jobDataService;

		public JobListVm(DataServices.JobDataService jobDataService)
		{
			_jobDataService = jobDataService;
			initializeCommands();

			//reset dates
			var dt = DateTime.Now.Date;
			dt = dt.AddDays(1 - dt.GetPersianDayOfMonth());
			StartDate = dt;
			EndDate = dt.AddDays(dt.GetPersianMonthDays());
		}
		void reloadJobs()
		{
			var jobs = _jobDataService.GetInRange(StartDate, EndDate, ByDefinition);
			Jobs.Clear();
			foreach (var job in jobs)
			{
				var jobItemVm = new JobListItemVm(job);
				Jobs.Add(jobItemVm);
				jobItemVm.JobSelected += id => { if (JobSelected != null) JobSelected(jobItemVm); };
			}
		}

		//Jobs Observable Collection
		public ObservableCollection<JobListItemVm> Jobs { get { return _jobs; } }
		private ObservableCollection<JobListItemVm> _jobs = new ObservableCollection<JobListItemVm>();
		//SelectedJob Dependency Property
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

		//StartDate Dependency Property
		public DateTime StartDate
		{
			get { return (DateTime)GetValue(StartDateProperty); }
			set { SetValue(StartDateProperty, value); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(JobListVm), new UIPropertyMetadata(DateTime.Now));
		//EndDate Dependency Property
		public DateTime EndDate
		{
			get { return (DateTime)GetValue(EndDateProperty); }
			set { SetValue(EndDateProperty, value); }
		}
		public static readonly DependencyProperty EndDateProperty =
			DependencyProperty.Register("EndDate", typeof(DateTime), typeof(JobListVm), new UIPropertyMetadata(DateTime.Now));

		//ByDefinition Dependency Property
		public bool ByDefinition
		{
			get { return (bool)GetValue(ByDefinitionProperty); }
			set { SetValue(ByDefinitionProperty, value); }
		}
		public static readonly DependencyProperty ByDefinitionProperty =
			DependencyProperty.Register("ByDefinition", typeof(bool), typeof(JobListVm), new UIPropertyMetadata(true));

		//IsVisible Dependency Property
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
		//SearchCommand Dependency Property
		public Commands.Command SearchCommand
		{
			get { return (Commands.Command)GetValue(SearchCommandProperty); }
			set { SetValue(SearchCommandProperty, value); }
		}
		public static readonly DependencyProperty SearchCommandProperty =
			DependencyProperty.Register("SearchCommand", typeof(Commands.Command), typeof(JobListVm), new UIPropertyMetadata(null));
		#endregion
	}
}
