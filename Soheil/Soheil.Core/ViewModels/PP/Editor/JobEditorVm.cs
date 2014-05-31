using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using S = System;
namespace Soheil.Core.ViewModels.PP.Editor
{
	/// <summary>
	/// This is the whole Editor
	/// </summary>
	public class JobEditorVm : DependencyObject
	{
		DataServices.ProductGroupDataService _productGroupDs;
		DataServices.FPCDataService _fpcDs;
		DataServices.JobDataService _jobDs;
		Dal.SoheilEdmContext _uow;
		public event Action RefreshPPTable;
		const int x = 0;
		public JobEditorVm()
		{
			initializeDataServices();
			initializeCommands();

			//load products
			var pgList = _productGroupDs.GetActivesRecursive();
			foreach (var pg in pgList)
			{
				var pgvm = new JobProductGroupVm(pg, _jobDs);
				AllProductGroups.Add(pgvm);
			}

			//event handler for DeleteJobCommand
			JobList.CollectionChanged += (s, e) =>
			{
				if(e.NewItems != null)
					foreach (var item in e.NewItems.OfType<PPEditorJob>())
					{
						item.JobDeleted += job => JobList.Remove(job);
						item.RefreshPPTable += () => { if (RefreshPPTable != null) RefreshPPTable(); };
					}
			};
		}

		void initializeDataServices()
		{
			_uow = new Dal.SoheilEdmContext();
			_productGroupDs = new DataServices.ProductGroupDataService(_uow);
			_fpcDs = new DataServices.FPCDataService(_uow);
			_jobDs = new DataServices.JobDataService(_uow);
		}

		#region Interactions
		//Add
		/*void FpcViewer_AddNewJob(Fpc.StateVm fpcState)
		{
			var ppStateListItem = PPStateList.FirstOrDefault(x => x.StateId == fpcState.Id);
			if (ppStateListItem == null)
			{
				ppStateListItem = new PPEditorState(fpcState);
				PPStateList.Add(ppStateListItem);
			}
		}
		//Remove
		public void FpcViewer_RemovePPState(PPEditorState ppState)
		{
			if (SelectedState != null && SelectedState.StateId == ppState.StateId) SelectedState = null;
			PPStateList.Remove(ppState);
		}*/
		//Clear
		public void Reset()
		{
			JobList.Clear();
		}
		#endregion

		#region Product etc
		//All Job ProductGroups Observable Collection
		private ObservableCollection<JobProductGroupVm> _allProductGroups = new ObservableCollection<JobProductGroupVm>();
		public ObservableCollection<JobProductGroupVm> AllProductGroups { get { return _allProductGroups; } }
		//JobList Observable Collection
		private ObservableCollection<PPEditorJob> _jobList = new ObservableCollection<PPEditorJob>();
		public ObservableCollection<PPEditorJob> JobList { get { return _jobList; } }
		#endregion

		//IsVisible Dependency Property
		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsVisibleProperty =
			DependencyProperty.Register("IsVisible", typeof(bool), typeof(JobEditorVm), new UIPropertyMetadata(false));

		#region Commands
		void initializeCommands()
		{
			SaveAllCommand = new Commands.Command(o =>
			{
				foreach (var job in JobList.Where(x => x.Quantity > 0))
				{
					job.SaveCommand.Execute(false);//false: job won't refresh pptable
				}
				Reset();
				IsVisible = false;
				if (RefreshPPTable != null)
					RefreshPPTable();
			});
			ClearAllCommand = new Commands.Command(o => Reset());
			ExitCommand = new Commands.Command(o => IsVisible = false);
		}

		//SaveAllCommand Dependency Property
		public Commands.Command SaveAllCommand
		{
			get { return (Commands.Command)GetValue(SaveAllCommandProperty); }
			set { SetValue(SaveAllCommandProperty, value); }
		}
		public static readonly DependencyProperty SaveAllCommandProperty =
			DependencyProperty.Register("SaveAllCommand", typeof(Commands.Command), typeof(JobEditorVm), new UIPropertyMetadata(null));
		//ClearAllCommand Dependency Property
		public Commands.Command ClearAllCommand
		{
			get { return (Commands.Command)GetValue(ClearAllCommandProperty); }
			set { SetValue(ClearAllCommandProperty, value); }
		}
		public static readonly DependencyProperty ClearAllCommandProperty =
			DependencyProperty.Register("ClearAllCommand", typeof(Commands.Command), typeof(JobEditorVm), new UIPropertyMetadata(null));
		//ExitCommand Dependency Property
		public Commands.Command ExitCommand
		{
			get { return (Commands.Command)GetValue(ExitCommandProperty); }
			set { SetValue(ExitCommandProperty, value); }
		}
		public static readonly DependencyProperty ExitCommandProperty =
			DependencyProperty.Register("ExitCommand", typeof(Commands.Command), typeof(JobEditorVm), new UIPropertyMetadata(null)); 
		#endregion

		internal void Append(JobVm Job)
		{
			var job = new Editor.PPEditorJob(Job.Model, _jobDs);
			job.RefreshPPTable += () => { if (RefreshPPTable != null) RefreshPPTable(); };
            JobList.Add(job);
		}
	}
}
