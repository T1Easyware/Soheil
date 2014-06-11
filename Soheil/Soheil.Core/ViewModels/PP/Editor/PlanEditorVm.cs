using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.PP.Editor
{
	/// <summary>
	/// ViewModel for PlanEditor
	/// <para>Does not have a UnitOfWork</para>
	/// </summary>
	public class PlanEditorVm : DependencyObject
	{
		public event Action RefreshPPItems;

		/// <summary>
		/// Creates an empty PlanEditor with its default values and reloads products
		/// </summary>
		public PlanEditorVm()
		{
			Reset();
		}

		/// <summary>
		/// Resets all information about this PlanEditor and reloads products
		/// </summary>
		public void Reset()
		{
			SelectedBlock = null;
			SelectedProduct = null;
			BlockList.Clear();

			//initializeDataServices
			var _productGroupDs = new DataServices.ProductGroupDataService();

			ProductGroups.Clear();
			var pgList = _productGroupDs.GetActives();
			foreach (var pg in pgList)
			{
				ProductGroups.Add(new ProductGroupVm(pg));
			}

			initializeCommands();
		}

		/// <summary>
		/// Changes BlockList or SelectedBlock
		/// </summary>
		/// <param name="stateVm"></param>
		void FpcViewer_SelectedStateChanged(Fpc.StateVm stateVm)
		{
			var block = BlockList.FirstOrDefault(x => x.StateId == stateVm.Id);
			if (block == null)
			{
				//add to list
				block = new BlockEditorVm(stateVm.Model);
				block.BlockAdded += b =>
				{
					if (RefreshPPItems != null) RefreshPPItems();
				};
				BlockList.Add(block);
			}
			else
				//exists, select it (double click kind of thing)
				SelectedBlock = block;
		}

		/// <summary>
		/// Removes a block from BlockList
		/// </summary>
		/// <param name="block"></param>
		public void RemoveBlock(BlockEditorVm block)
		{
			if (SelectedBlock != null && SelectedBlock.StateId == block.StateId)
			{
				SelectedBlock = null;
			}
			BlockList.Remove(block);
		}

		#region Visual Properties
		/// <summary>
		/// Gets or sets a bindable value that indicates whether the PlanEditor is visible
		/// </summary>
		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsVisibleProperty =
			DependencyProperty.Register("IsVisible", typeof(bool), typeof(PlanEditorVm), new UIPropertyMetadata(false));


		/// <summary>
		/// Gets or sets a bindable value for FpcWindow viewer (state selector)
		/// </summary>
		public Fpc.FpcWindowVm FpcViewer
		{
			get { return (Fpc.FpcWindowVm)GetValue(FpcViewerProperty); }
			set { SetValue(FpcViewerProperty, value); }
		}
		public static readonly DependencyProperty FpcViewerProperty =
			DependencyProperty.Register("FpcViewer", typeof(Fpc.FpcWindowVm), typeof(PlanEditorVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets or seta a bindable value that indicates whether the Machines are visible in Processes
		/// </summary>
		public bool ShowMachines
		{
			get { return (bool)GetValue(ShowMachinesProperty); }
			set { SetValue(ShowMachinesProperty, value); }
		}
		public static readonly DependencyProperty ShowMachinesProperty =
			DependencyProperty.Register("ShowMachines", typeof(bool), typeof(PlanEditorVm), new UIPropertyMetadata(true));

		/// <summary>
		/// Gets or sets a bindable value that indicates whether the Operators are visible in Processes
		/// </summary>
		public bool ShowOperators
		{
			get { return (bool)GetValue(ShowOperatorsProperty); }
			set { SetValue(ShowOperatorsProperty, value); }
		}
		public static readonly DependencyProperty ShowOperatorsProperty =
			DependencyProperty.Register("ShowOperators", typeof(bool), typeof(PlanEditorVm), new UIPropertyMetadata(true));

		#endregion

		#region Products(groups) and Blocks(states)
		/// <summary>
		/// Gets a bindable collection for product groups (and products)
		/// </summary>
		public ObservableCollection<ProductGroupVm> ProductGroups { get { return _productGroups; } }
		private ObservableCollection<ProductGroupVm> _productGroups = new ObservableCollection<ProductGroupVm>();

		/// <summary>
		/// Gets or sets a bindable value for Selected Product inside ProductGroups
		/// <para>Changing the value updates FpcViewer</para>
		/// </summary>
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
					//hide BlockEditor
					d.SetValue(SelectedBlockProperty, null);

					//create new fpc vm
					vm.FpcViewer = new Fpc.FpcWindowVm(true);
					vm.FpcViewer.ChangeFpcByProductId(val.Id);

					//change BlockList or SelectedBlock upon SelectedStateChanged event
					vm.FpcViewer.SelectedStateChanged += vm.FpcViewer_SelectedStateChanged;
				}
			}));


		/// <summary>
		/// Gets a bindable collection of Blocks
		/// </summary>
		public ObservableCollection<BlockEditorVm> BlockList { get { return _blockList; } }
		private ObservableCollection<BlockEditorVm> _blockList = new ObservableCollection<BlockEditorVm>();

		/// <summary>
		/// Gets or sets a bindable value to indicate which EditorBlock is currently selected in the TaskEditor
		/// <para>Changing this value to a valid Block will hide fpc</para>
		/// <para>If only one station is available in state, selects it</para>
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
					//hide FpcViewer
					d.SetValue(SelectedProductProperty, null);

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
					SelectedBlock.Message.AddEmbeddedException(exp);
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
				foreach (var block in BlockList)
				{
					try
					{
						block.Save();
						if (RefreshPPItems != null) RefreshPPItems();
					}
					catch (Exception exp)
					{
						block.Message.AddEmbeddedException(exp);
						return;
					}
				}
				Reset();
				IsVisible = false;
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
		#endregion

	}
}