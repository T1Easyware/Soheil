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
	public class PPTaskEditorVm : DependencyObject
	{
		protected DataServices.ProductGroupDataService _productGroupDs;
		protected DataServices.FPCDataService _fpcDs;
		protected DataServices.OperatorDataService _operatorDs;

		public PPTaskEditorVm(Action<IList<PPEditorStation>> tasksSaved)
		{
			TasksSaved = tasksSaved;
			_productGroupDs = new DataServices.ProductGroupDataService();
			_fpcDs = new DataServices.FPCDataService();
			var pgList = _productGroupDs.GetActivesRecursive();
			foreach (var pg in pgList)
			{
				AllProductGroups.Add(new ProductGroupVm(pg));
			}
			_operatorDs = new DataServices.OperatorDataService();

			FpcViewer = new Fpc.FpcWindowVm();
			FpcViewer.SelectState += FpcViewer_AddNewTask;
		}

		#region Interactions
		//Add
		void FpcViewer_AddNewTask(Fpc.StateVm fpcState)
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
		}
		//Save
		internal Action<IList<PPEditorStation>> TasksSaved;//PPTableVm handles this event
		protected void SaveTasks(List<PPEditorStation> tasks)//View calls this function
		{
			if (TasksSaved != null) TasksSaved(tasks);
		}
		public void SaveSelectedStateStationAsTask()
		{
			var tasks = new List<PPEditorStation>();
			tasks.Add(SelectedState.StationList[SelectedState.FocusedStationTabIndex]);
			SaveTasks(tasks);
		}
		public void SaveAllAsTasks()
		{
			var tasks = new List<PPEditorStation>();
			foreach (var state in PPStateList.Where(x => x.HasUnsavedChanges))
			{
				foreach (var station in state.StationList.Where(x => x.HasUnsavedChanges))
				{
					tasks.Add(station);
				}
			}
			SaveTasks(tasks);
		}
		//Clear
		public void Reset()
		{
			SelectedState = null;
			SelectedProduct = null;
			PPStateList.Clear();
			ShowFpc = true;
		}
		#endregion

		#region State etc
		//PPStateList Observable Collection
		private ObservableCollection<PPEditorState> _ppStateList = new ObservableCollection<PPEditorState>();
		public ObservableCollection<PPEditorState> PPStateList { get { return _ppStateList; } }
		//SelectedState Dependency Property
		public PPEditorState SelectedState
		{
			get { return (PPEditorState)GetValue(SelectedStateProperty); }
			set { SetValue(SelectedStateProperty, value); }
		}
		public static readonly DependencyProperty SelectedStateProperty =
			DependencyProperty.Register("SelectedState", typeof(PPEditorState), typeof(PPTaskEditorVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (PPTaskEditorVm)d;
				if (e.NewValue != null)
					vm.ShowFpc = false;
			}));
		//ShowFpc Dependency Property
		public bool ShowFpc
		{
			get { return (bool)GetValue(ShowFpcProperty); }
			set { SetValue(ShowFpcProperty, value); }
		}
		public static readonly DependencyProperty ShowFpcProperty =
			DependencyProperty.Register("ShowFpc", typeof(bool), typeof(PPTaskEditorVm), new UIPropertyMetadata(true));
		#endregion

		#region Product etc
		//AllProductGroups Observable Collection
		private ObservableCollection<ProductGroupVm> _allProductGroups = new ObservableCollection<ProductGroupVm>();
		public ObservableCollection<ProductGroupVm> AllProductGroups { get { return _allProductGroups; } }

		//SelectedProduct Dependency Property
		public ProductVm SelectedProduct
		{
			get { return (ProductVm)GetValue(SelectedProductProperty); }
			set { SetValue(SelectedProductProperty, value); }
		}
		public static readonly DependencyProperty SelectedProductProperty =
			DependencyProperty.Register("SelectedProduct", typeof(ProductVm), typeof(PPTaskEditorVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (PPTaskEditorVm)d;
				var val = (ProductVm)e.NewValue;

				//reset fpc if product is null
				if (val == null) 
					vm.FpcViewer.ResetFPC(clearModel: true);
				
				else
				{
					//get active fpc
					var fpcModel = vm._fpcDs.GetActiveForProduct(val.Id);

					//reset fpc if fpc is null
					if (fpcModel == null)
						vm.FpcViewer.ResetFPC(clearModel: true);

						//update the viewer
					else
						vm.FpcViewer.ChangeFpc(fpcModel.Id);
				}
			}));
		#endregion

		//IsVisible Dependency Property
		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsVisibleProperty =
			DependencyProperty.Register("IsVisible", typeof(bool), typeof(PPTaskEditorVm), new UIPropertyMetadata(false));
		//FpcViewer Dependency Property
		public Fpc.FpcWindowVm FpcViewer
		{
			get { return (Fpc.FpcWindowVm)GetValue(FpcViewerProperty); }
			private set { SetValue(FpcViewerProperty, value); }
		}
		public static readonly DependencyProperty FpcViewerProperty =
			DependencyProperty.Register("FpcViewer", typeof(Fpc.FpcWindowVm), typeof(PPTaskEditorVm),
			new UIPropertyMetadata(null));
	}
}
