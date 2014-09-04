using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using Soheil.Core.ViewModels.PP.Timeline;
using Soheil.Common;
using Soheil.Core.PP;
using Soheil.Common.SoheilException;

namespace Soheil.Core.ViewModels.PP
{
	/// <summary>
	/// An advanced collection of Stations each containing their own Blocks and Npts
	/// <para>This class also provide add/remove/find methods that directly target Blocks and Npts regardless of their station</para>
	/// </summary>
	/// <remarks>Only those items in each station are shown that are within the timeline range</remarks>
	public class PPItemCollection : ObservableCollection<StationVm>, IDisposable
	{
		#region Events
		/// <summary>
		/// Occurs when a new block is added to collection
		/// </summary>
		public event Action<BlockVm> BlockAdded;
		/// <summary>
		/// Occurs when a block is removed from this collection. parameter is blockId
		/// </summary>
		public event Action<int> BlockRemoved;
		/// <summary>
		/// Occurs when a new npt is added to collection
		/// </summary>
		public event Action<NPTVm> NptAdded;
		/// <summary>
		/// Occurs when an npt is removed from this collection. parameter is nptId
		/// </summary>
		public event Action<int> NptRemoved;
		#endregion

		#region Members, props, consts
		public PPTableVm PPTable { get; private set; }
		public PPItemManager Manager { get; private set; }

		/// <summary>
		/// Gets the ViewMode of this Vm or sets it for this Vm and all its blocks
		/// </summary>
		public PPViewMode ViewMode
		{
			get { return _viewMode; }
			set
			{
				_viewMode = value;
				foreach (var station in this)
				{
					foreach (var block in station.Blocks)
					{
						block.ViewMode = value;
					}
				}
			}
		}
		private PPViewMode _viewMode;
		#endregion

		/// <summary>
		/// Creates an instance of PPItemCollection
		/// </summary>
		/// <param name="parent"></param>
		public PPItemCollection(PPTableVm parent)
		{
			PPTable = parent;
			ViewMode = PPViewMode.Simple;
			
			//manager
			Manager = new PPItemManager(parent.Dispatcher);

			Manager.EverythingRemoved += () =>
			{
				foreach (var item in this)
				{
					item.Blocks.Clear();
					item.NPTs.Clear();
				}
			};

			Manager.BlockAddedOrUpdated += item =>
			{
				var station = this[item.Model.StateStation.Station.Index];
				var vm = station.Blocks.FirstOrDefault(x => x.Id == item.Id);
				if(vm == null)
				{
					//add
					vm = AddItem(item);
					vm.ViewMode = ViewMode;
					item.HasVm = true;
				}
				else
				{
					//update
					vm.Reload(item);
				}
				FixStationVSizes(station);
			};
			Manager.BlockRemoved += item =>
			{
				//keep updated with _lastBlockIds
				RemoveItem(item.Model);
				FixStationVSizes(this[item.Model.StateStation.Station.Index]);
			};
			Manager.NptAddedOrUpdated += item =>
			{
				if (item.Model is Model.Setup)//???
				{
					var station = this[(item.Model as Model.Setup).Warmup.Station.Index];
					var vm = station.NPTs.FirstOrDefault(x => x.Id == item.Id);
					if (vm == null)
					{
						//add
						vm = AddNPT(item.Id);
						if (vm != null)
						{
							vm.ViewMode = ViewMode;
							if (NptAdded != null) NptAdded(vm);
						}
					}
					else
					{
						//update
						if (parent.SelectedNPT != null)
						{
							if (parent.SelectedNPT.Id != item.Id)
								vm.Reload(item);
						}
						else vm.Reload(item);
					}
				}
			};
			Manager.NptRemoved += item =>
			{
				RemoveItem(item.Model as Model.Setup);
			};
		}
		private void FixStationVSizes(StationVm station)
		{
			if (station.Blocks.Any())
				station.VCount = station.Blocks.Max(x => x.VIndex) + 1;
			else
				station.VCount = 1;
		}
		public void Dispose()
		{
			if (Manager != null)
				Manager.Dispose();
		}

		#region Task Operations
		/// <summary>
		/// Returns the row of blocks which contains the provided model
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public ObservableCollection<BlockVm> GetRowContaining(Model.Block model)
		{
			return this[model.StateStation.Station.Index].Blocks;
		}

		/// <summary>
		/// Adds a new BlockVm to this collection with the given model and sets all its commands
		/// </summary>
		/// <remarks>
		/// Finds the row of items which this model should be in,
		/// <para>converts the model into VM and adds it to that row</para>
		/// <para>returns the VM</para>
		/// </remarks>
		/// <param name="data">full data fetched from database, UOW of which will be Block's</param>
		/// <returns></returns>
		public BlockVm AddItem(PPItemBlock data)
		{
			try
			{
				//find the container row of blocks
				var container = GetRowContaining(data.Model);
				//find any existing vm with same id
				var currentVm = container.FirstOrDefault(x => x.Id == data.Model.Id);
				//if already exist remove it
				if (currentVm != null) container.Remove(currentVm);

				//create viewmodel for the new block
				var vm = new BlockVm(data, this);
				if (BlockAdded != null) BlockAdded(vm);
				initializeCommands(vm);

				//add the viewmodel to its container
				container.Add(vm);
				return vm;
			}
			catch { return null; }
		}

		/// <summary>
		/// Removes a blockVm from this collection based on its Id
		/// </summary>
		/// <param name="vm">vm of block to remove (Id and RowIndex are used)</param>
		public void RemoveItem(BlockVm vm, int id = -1)
		{
			if (vm == null) return;
			if (id == -1) id = vm.Id;
			try
			{
				//remove from vm
				this[vm.RowIndex].Blocks.RemoveWhere(x => x.Id == id);

				//deselect selected block
				if (BlockRemoved != null) BlockRemoved(id);
			}
			catch (Exception ex) { vm.Message.AddEmbeddedException(ex.Message); }
		}
		public void RemoveItem(Model.Block model)
		{
			//remove from vm
			this[model.StateStation.Station.Index].Blocks.RemoveWhere(x => x.Id == model.Id);

			//deselect selected block
			if (BlockRemoved != null) BlockRemoved(model.Id);
		}

		#endregion

		#region NPT Operations
		/// <summary>
		/// Adds a new NPTVm to this collection with the given model
		/// </summary>
		/// <remarks>
		/// Finds the row of items which this model should be in,
		/// <para>converts the model into VM and adds it to that row</para>
		/// <para>returns the VM</para>
		/// </remarks>
		/// <param name="model"></param>
		/// <returns></returns>
		public NPTVm AddNPT(int nptId)
		{
			try
			{
				//if (model is Model.Setup)
				//{
				//create a new SetupVm
				var vm = new SetupVm(nptId, this);
				//find the container row of npts that this setup belongs to
				var container = this[vm.RowIndex].NPTs;
				//find any existing setup vm that matches the Id
				var currentVm = container.FirstOrDefault(x => x.Id == nptId);
				//if setup vm already exists remove it
				if (currentVm != null) container.Remove(currentVm);
				//add it to container
				container.Add(vm);
				return vm;
				//}
				//else throw new NotImplementedException();//???
			}
			catch { return null; }
		}

		/// <summary>
		/// Removes a nptVm from this collection based on its Id
		/// </summary>
		/// <param name="vm">vm of npt to remove (Id and RowIndex are used)</param>
		public void RemoveItem(NPTVm vm)
		{
			try
			{
				//remove from vm
				this[vm.RowIndex].NPTs.RemoveWhere(x => x.Id == vm.Id);

				//deselect selected npt
				if (vm != null && NptRemoved != null) NptRemoved(vm.Id);
			}
			catch { }
		}
		public void RemoveItem(Model.Setup model)
		{
			try
			{
				//remove from vm
				this[model.Warmup.Station.Index].NPTs.RemoveWhere(x => x.Id == model.Id);

				//deselect selected npt
				if (model != null && NptRemoved != null) NptRemoved(model.Id);
			}
			catch { }
		}
		#endregion

		#region Commands
		/// <summary>
		/// Initializes BlockVm commands of vm that can be assigned in this class
		/// </summary>
		/// <param name="vm"></param>
		private void initializeCommands(BlockVm vm)
		{
			/*vm.InsertSetup = new Commands.Command(o =>
			{
			});*/
			vm.InsertSetupBefore = new Commands.Command(o =>
			{
				var id = vm.Id;
				var result = new DataServices.BlockDataService().InsertSetupBeforeBlock(id);

				//in case of error callback with result
				if (result.IsSaved) Manager.ForceReload();
				else vm.InsertSetupBeforeCallback(result);
			});
			/*vm.InsertSetupBetween = new Commands.Command(o =>
			{
				var id = vm.Id;
				var before = ((BlockVm)o).Id;
				var result = new DataServices.BlockDataService().InsertSetupBetweenBlocks(before, id);

				//in case of error callback with result
				if (result.IsSaved) Manager.ForceReload();
				else vm.InsertSetupBeforeCallback(result);
			}); */
			vm.DeleteItemCommand = new Commands.Command(o =>
			{
				lock (Manager)
				{
					try
					{
						int id = vm.Model.Id;
						new DataServices.BlockDataService().DeleteModel(vm.Model);
						RemoveItem(vm, id);
						Manager.ForceReload();
					}
					catch (Exception exp) { vm.Message.AddEmbeddedException(exp.Message); }
				}
			});
			vm.DeleteBlockWithReportsCommand = new Commands.Command(o =>
			{
				lock (Manager)
				{
					try
					{
						int id = vm.Model.Id;
						new DataServices.BlockDataService().DeleteModelRecursive(vm.Model);
						RemoveItem(vm, id);
						Manager.ForceReload();
					}
					catch (Exception exp) { vm.Message.AddEmbeddedException(exp.Message); }
				}
			});
		}
		#endregion
	}
}
