using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class PlanEditorVm : DependencyObject
	{
		public event Action RefreshPPItems;

		public PlanEditorVm()
		{
			Reset();
		}
		public void Reset()
		{
			SelectedBlock = null;
			SelectedProduct = null;
			BlockList.Clear();
			Message = new Common.SoheilException.EmbeddedException();

			//initializeDataServices
			var _productGroupDs = new DataServices.ProductGroupDataService();

			ProductGroups.Clear();
			var pgList = _productGroupDs.GetActivesRecursive();
			foreach (var pg in pgList)
			{
				ProductGroups.Add(new ProductGroupVm(pg));
			}

			initializeCommands();
		}


		#region Visual Properties
		//IsVisible Dependency Property
		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsVisibleProperty =
			DependencyProperty.Register("IsVisible", typeof(bool), typeof(PlanEditorVm), new UIPropertyMetadata(false));


		//FpcViewer Dependency Property
		public Fpc.FpcWindowVm FpcViewer
		{
			get { return (Fpc.FpcWindowVm)GetValue(FpcViewerProperty); }
			set { SetValue(FpcViewerProperty, value); }
		}
		public static readonly DependencyProperty FpcViewerProperty =
			DependencyProperty.Register("FpcViewer", typeof(Fpc.FpcWindowVm), typeof(PlanEditorVm), new UIPropertyMetadata(null));


		//Message Dependency Property
		public Soheil.Common.SoheilException.EmbeddedException Message
		{
			get { return (Soheil.Common.SoheilException.EmbeddedException)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(Soheil.Common.SoheilException.EmbeddedException), typeof(PlanEditorVm), new UIPropertyMetadata(null));
		#endregion

		#region Products(groups) and Blocks(states)
		//ProductGroups Observable Collection
		public ObservableCollection<ProductGroupVm> ProductGroups { get { return _productGroups; } }
		private ObservableCollection<ProductGroupVm> _productGroups = new ObservableCollection<ProductGroupVm>();

		//SelectedProduct Dependency Property
		public ProductVm SelectedProduct
		{
			get { return (ProductVm)GetValue(SelectedProductProperty); }
			set { SetValue(SelectedProductProperty, value); }
		}
		public static readonly DependencyProperty SelectedProductProperty =
			DependencyProperty.Register("SelectedProduct", typeof(ProductVm), typeof(PlanEditorVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (PlanEditorVm)d;
				var val = (ProductVm)e.NewValue;

				if (val == null)
					vm.FpcViewer = null;
				else
				{
					//create new fpc vm
					vm.FpcViewer = new Fpc.FpcWindowVm(true);
					vm.FpcViewer.ChangeFpcByProductId(val.Id);

					//change BlockList or SelectedBlock upon SelectedStateChanged event
					vm.FpcViewer.SelectedStateChanged += stateVm =>
					{
						var block = vm.BlockList.FirstOrDefault(x => x.StateId == stateVm.Id);
						if (block == null)
						{
							//add to list
							block = new BlockEditorVm(stateVm.Model);
							vm.BlockList.Add(block);
						}
						else
							//exists, select it (double click kind of thing)
							vm.SelectedBlock = block;
					};
				}
			}));

		public void RemoveBlock(BlockEditorVm block)
		{
			if (SelectedBlock != null && SelectedBlock.StateId == block.StateId)
			{
				SelectedBlock = null;
			}
			BlockList.Remove(block);
		}

		//BlockList Observable Collection
		public ObservableCollection<BlockEditorVm> BlockList { get { return _blockList; } }
		private ObservableCollection<BlockEditorVm> _blockList = new ObservableCollection<BlockEditorVm>();

		/// <summary>
		/// Gets or sets a bindable value to indicate which EditorBlock is currently selected in the TaskEditor
		/// <para>Setting this value will hide fpc and if 1 station is available in state select it</para>
		/// </summary>
		public BlockEditorVm SelectedBlock
		{
			get { return (BlockEditorVm)GetValue(SelectedBlockProperty); }
			set { SetValue(SelectedBlockProperty, value); }
		}
		public static readonly DependencyProperty SelectedBlockProperty =
			DependencyProperty.Register("SelectedBlock", typeof(BlockEditorVm), typeof(PlanEditorVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (PlanEditorVm)d;
				var val = e.NewValue as BlockEditorVm;
				if (val != null)
				{
					//hide fpc
					vm.SelectedProduct = null;

					//automatically select the only station
					if (val.StateStation == null && val.State.StateStationList.Count == 1)
					{
						val.SelectedStateStation = val.State.StateStationList.First();
						val.ChangeStationCommand.Execute(null);
					}
				}
			}));
		#endregion


		#region Commands
		void initializeCommands()
		{
			SaveCommand = new Commands.Command(o =>
			{
				if (SelectedBlock == null) return;
				try
				{
					SelectedBlock.Save();
					if (RefreshPPItems != null) RefreshPPItems();
				}
				catch (Exception exp)
				{
					Message.AddEmbeddedException(exp);
				}
			});
			ClearAllCommand = new Commands.Command(o =>
			{
				Reset();
			});
			ExitCommand = new Commands.Command(o =>
			{
				IsVisible = false;
			});
			SaveAllAndExitCommand = new Commands.Command(o =>
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
					Message.AddEmbeddedException(exp);
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
			DependencyProperty.Register("SaveCommand", typeof(Commands.Command), typeof(PlanEditorVm), new UIPropertyMetadata(null));
		//ClearAllCommand Dependency Property
		public Commands.Command ClearAllCommand
		{
			get { return (Commands.Command)GetValue(ClearAllCommandProperty); }
			set { SetValue(ClearAllCommandProperty, value); }
		}
		public static readonly DependencyProperty ClearAllCommandProperty =
			DependencyProperty.Register("ClearAllCommand", typeof(Commands.Command), typeof(PlanEditorVm), new UIPropertyMetadata(null));
		//ExitCommand Dependency Property
		public Commands.Command ExitCommand
		{
			get { return (Commands.Command)GetValue(ExitCommandProperty); }
			set { SetValue(ExitCommandProperty, value); }
		}
		public static readonly DependencyProperty ExitCommandProperty =
			DependencyProperty.Register("ExitCommand", typeof(Commands.Command), typeof(PlanEditorVm), new UIPropertyMetadata(null));
		//SaveAllAndExitCommand Dependency Property
		public Commands.Command SaveAllAndExitCommand
		{
			get { return (Commands.Command)GetValue(SaveAllAndExitCommandProperty); }
			set { SetValue(SaveAllAndExitCommandProperty, value); }
		}
		public static readonly DependencyProperty SaveAllAndExitCommandProperty =
			DependencyProperty.Register("SaveAllAndExitCommand", typeof(Commands.Command), typeof(PlanEditorVm), new UIPropertyMetadata(null));
		//ResetCurrentBlockCommand Dependency Property
		public Commands.Command ResetCurrentBlockCommand
		{
			get { return (Commands.Command)GetValue(ResetCurrentBlockCommandProperty); }
			set { SetValue(ResetCurrentBlockCommandProperty, value); }
		}
		public static readonly DependencyProperty ResetCurrentBlockCommandProperty =
			DependencyProperty.Register("ResetCurrentBlockCommand", typeof(Commands.Command), typeof(PlanEditorVm), new UIPropertyMetadata(null));
		#endregion

	}
}