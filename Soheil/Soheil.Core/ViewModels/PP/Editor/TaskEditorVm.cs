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
	public class TaskEditorVm : DependencyObject
	{
		protected DataServices.ProductGroupDataService _productGroupDs;
		protected DataServices.FPCDataService _fpcDs;
		protected DataServices.OperatorDataService _operatorDs;
		protected DataServices.TaskDataService _taskDs;
		protected DataServices.BlockDataService _blockDs;

		public event Action RefreshPPItems;

		public Dal.SoheilEdmContext UOW { get; private set; }
		public TaskEditorVm()
		{
			UOW = new Dal.SoheilEdmContext();
			Reset();
		}
		public void Reset()
		{
			FpcViewer = new Fpc.FpcWindowVm(UOW, true);
			FpcViewer.SelectedStateChanged += FpcViewer_AddNewBlock;
			initializeDataServices();
			initializeCommands();
			SelectedBlock = null;
			SelectedProduct = null;
			BlockList.Clear();
			ShowFpc = true;
		}

		void initializeDataServices()
		{
			_productGroupDs = new DataServices.ProductGroupDataService(UOW);
			_fpcDs = new DataServices.FPCDataService(UOW);
			_operatorDs = new DataServices.OperatorDataService(UOW);
			_taskDs = new DataServices.TaskDataService(UOW);
			_blockDs = new DataServices.BlockDataService(UOW);

			AllProductGroups.Clear();
			var pgList = _productGroupDs.GetActivesRecursive();
			foreach (var pg in pgList)
			{
				AllProductGroups.Add(new ProductGroupVm(pg));
			}
		}
		//Add
		void FpcViewer_AddNewBlock(Fpc.StateVm fpcState)
		{
			var block = BlockList.FirstOrDefault(x => x.StateId == fpcState.Id);
			if (block == null)
			{
				block = new PPEditorBlock(fpcState.Model);
				BlockList.Add(block);
			}
			else
				SelectedBlock = block;
		}
		//Remove
		public void FpcViewer_RemoveBlock(PPEditorBlock block)
		{
			if (SelectedBlock != null && SelectedBlock.StateId == block.StateId)
			{
				SelectedBlock = null;
			}
			BlockList.Remove(block);
		}

		/// <summary>
		/// Gets the bindable <see cref="FpcWindowVm"/> instance in which fpc is being shown
		/// </summary>
		public Fpc.FpcWindowVm FpcViewer
		{
			get { return (Fpc.FpcWindowVm)GetValue(FpcViewerProperty); }
			private set { SetValue(FpcViewerProperty, value); }
		}
		public static readonly DependencyProperty FpcViewerProperty =
			DependencyProperty.Register("FpcViewer", typeof(Fpc.FpcWindowVm), typeof(TaskEditorVm), new UIPropertyMetadata(null));


		#region Blocks
		//BlockList Observable Collection
		public ObservableCollection<PPEditorBlock> BlockList { get { return _blockList; } }
		private ObservableCollection<PPEditorBlock> _blockList = new ObservableCollection<PPEditorBlock>();
		
		/// <summary>
		/// Gets or sets a bindable value to indicate which EditorBlock is currently selected in the TaskEditor
		/// <para>Setting this value will hide fpc and if 1 station is available in state select it</para>
		/// </summary>
		public PPEditorBlock SelectedBlock
		{
			get { return (PPEditorBlock)GetValue(SelectedBlockProperty); }
			set { SetValue(SelectedBlockProperty, value); }
		}
		public static readonly DependencyProperty SelectedBlockProperty =
			DependencyProperty.Register("SelectedBlock", typeof(PPEditorBlock), typeof(TaskEditorVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (TaskEditorVm)d;
				var val = e.NewValue as PPEditorBlock;
				if (val != null)
				{
					//hide fpc
					vm.ShowFpc = false;

					//automatically select the only station
					if (val.StateStation == null && val.State.StateStationList.Count == 1)
					{
						val.SelectedStateStation = val.State.StateStationList.First();
						val.ChangeStationCommand.Execute(null);
					}
				}
			}));
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
			DependencyProperty.Register("SelectedProduct", typeof(ProductVm), typeof(TaskEditorVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (TaskEditorVm)d;
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

		#region Visual
		//Show FpcViewer/TaskEditor
		public bool ShowFpc
		{
			get { return (bool)GetValue(ShowFpcProperty); }
			set { SetValue(ShowFpcProperty, value); }
		}
		public static readonly DependencyProperty ShowFpcProperty =
			DependencyProperty.Register("ShowFpc", typeof(bool), typeof(TaskEditorVm), new UIPropertyMetadata(true));

		//IsVisible Dependency Property
		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsVisibleProperty =
			DependencyProperty.Register("IsVisible", typeof(bool), typeof(TaskEditorVm), new UIPropertyMetadata(false));
		#endregion

		#region Commands
		void initializeCommands()
		{
			SaveCommand = new Commands.Command(o =>
			{
				throw new Exception("not meant to be run yet. reason of disability: possible loss of data due to shared UOW throughout the taskEditor");
				/*if (SelectedBlock == null) return;
				try
				{
					if (SelectedBlock.IsAutoStart)
						SelectedBlock.SetStartToAuto();
					_blockDs.SaveBlock(SelectedBlock.Model);
					if (RefreshPPItems != null) RefreshPPItems();
				}
				catch (Exception exp)
				{
					MessageBox.Show(exp.Message);
				}*/
			});
			ClearAllCommand = new Commands.Command(o =>
			{
				Reset();
			});
			ExitCommand = new Commands.Command(o =>
			{
				IsVisible = false;
			});
			SaveAllCommand = new Commands.Command(o =>
			{
				try
				{
					foreach (var block in BlockList)
					{
						block.Save();
						if (RefreshPPItems != null) RefreshPPItems();
					}
					Reset();
					IsVisible = false;
				}
				catch (Exception exp)
				{
					MessageBox.Show(exp.Message);
				}
			});
			ResetCurrentBlockCommand = new Commands.Command(o =>
			{
				SelectedBlock.Reset();
			});
		}
		//SaveCommand Dependency Property
		public Commands.Command SaveCommand
		{
			get { return (Commands.Command)GetValue(SaveCommandProperty); }
			set { SetValue(SaveCommandProperty, value); }
		}
		public static readonly DependencyProperty SaveCommandProperty =
			DependencyProperty.Register("SaveCommand", typeof(Commands.Command), typeof(TaskEditorVm), new UIPropertyMetadata(null));
		//ClearAllCommand Dependency Property
		public Commands.Command ClearAllCommand
		{
			get { return (Commands.Command)GetValue(ClearAllCommandProperty); }
			set { SetValue(ClearAllCommandProperty, value); }
		}
		public static readonly DependencyProperty ClearAllCommandProperty =
			DependencyProperty.Register("ClearAllCommand", typeof(Commands.Command), typeof(TaskEditorVm), new UIPropertyMetadata(null));
		//ExitCommand Dependency Property
		public Commands.Command ExitCommand
		{
			get { return (Commands.Command)GetValue(ExitCommandProperty); }
			set { SetValue(ExitCommandProperty, value); }
		}
		public static readonly DependencyProperty ExitCommandProperty =
			DependencyProperty.Register("ExitCommand", typeof(Commands.Command), typeof(TaskEditorVm), new UIPropertyMetadata(null));
		//SaveAllCommand Dependency Property
		public Commands.Command SaveAllCommand
		{
			get { return (Commands.Command)GetValue(SaveAllCommandProperty); }
			set { SetValue(SaveAllCommandProperty, value); }
		}
		public static readonly DependencyProperty SaveAllCommandProperty =
			DependencyProperty.Register("SaveAllCommand", typeof(Commands.Command), typeof(TaskEditorVm), new UIPropertyMetadata(null));
		//ResetCurrentBlockCommand Dependency Property
		public Commands.Command ResetCurrentBlockCommand
		{
			get { return (Commands.Command)GetValue(ResetCurrentBlockCommandProperty); }
			set { SetValue(ResetCurrentBlockCommandProperty, value); }
		}
		public static readonly DependencyProperty ResetCurrentBlockCommandProperty =
			DependencyProperty.Register("ResetCurrentBlockCommand", typeof(Commands.Command), typeof(TaskEditorVm), new UIPropertyMetadata(null));
		#endregion

	}
}
