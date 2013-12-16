using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Editor
{
	/// <summary>
	/// This is the whole Editor
	/// </summary>
	public class PPJobEditorVm : DependencyObject
	{
		protected DataServices.ProductGroupDataService _productGroupDs;
		protected DataServices.FPCDataService _fpcDs;

		public PPJobEditorVm(Action<IList<PPEditorJob>> jobsSaved)
		{
			JobsSaved = jobsSaved;
			_productGroupDs = new DataServices.ProductGroupDataService();
			_fpcDs = new DataServices.FPCDataService();
			var pgList = _productGroupDs.GetActivesRecursive();
			foreach (var pg in pgList)
			{
				AllProductGroups.Add(new ProductGroupVm(pg));
			}
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
		//Save
		internal Action<IList<PPEditorJob>> JobsSaved;//PPTableVm handles this event
		public void SaveJob(PPEditorJob job)//View calls this function
		{
			if (JobsSaved != null) JobsSaved(new List<PPEditorJob> { job });
		}
		public void SaveAllJobs()//View calls this function
		{
			if (JobsSaved != null) JobsSaved(JobList.Where(x => x.Quantity > 0).ToList());
		}
		//Clear
		public void Reset()
		{
			JobList.Clear();
		}
		public void DeleteJob(PPEditorJob job)
		{
			JobList.Remove(job);
			//???
		}
		#endregion

		#region Product etc
		//AllProductGroups Observable Collection
		private ObservableCollection<ProductGroupVm> _allProductGroups = new ObservableCollection<ProductGroupVm>();
		public ObservableCollection<ProductGroupVm> AllProductGroups { get { return _allProductGroups; } }
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
			DependencyProperty.Register("IsVisible", typeof(bool), typeof(PPJobEditorVm), new UIPropertyMetadata(false));
	}
}
