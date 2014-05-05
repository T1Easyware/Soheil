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
	public class PPItemCollection : ObservableCollection<StationVm>
	{

		#region Members, props, consts
		public PPTableVm PPTable { get; private set; }

		Dal.SoheilEdmContext _uow;
		DataServices.BlockDataService _blockDataService;

		System.ComponentModel.BackgroundWorker _backgroundWorker;

		DateTime _rangeStart;
		DateTime _rangeEnd;

		private object _lockObject;
		/// <summary>
		/// Number of milliseconds to sleep when loading each item
		/// </summary>
		private const int _workerSleepTime = 40;
		/// <summary>
		/// Number of hours to add to the startRange when loading items
		/// </summary>
		private const int _startHourMargin = -1;
		/// <summary>
		/// Number of hours to add to the endRange when loading items
		/// </summary>
		private const int _endHourMargin = 3; 

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

		#region Events
		public event Action<BlockVm> BlockAdded;
		/// <summary>
		/// Occurs when a block is removed from this collection. parameter is blockId
		/// </summary>
		public event Action<int> BlockRemoved;
		/// <summary>
		/// Occurs when an npt is removed from this collection. parameter is nptId
		/// </summary>
		public event Action<int> NptRemoved;
		#endregion

		#region Ctor, Parallel loading
		/// <summary>
		/// Creates an instance of PPItemCollection
		/// </summary>
		/// <param name="parent"></param>
		public PPItemCollection(PPTableVm parent)
		{
			PPTable = parent;
			ViewMode = PPViewMode.Simple;
			_lockObject = new Object();
		}

		/// <summary>
		/// Reloads all blocks and Npts within the specified range
		/// <para>Any items that partially fall into the range are also included</para>
		/// </summary>
		/// <param name="rangeStart">start of the timeline range</param>
		/// <param name="rangeEnd">end of the timeline range</param>
		public void FetchRange(DateTime rangeStart, DateTime rangeEnd)
		{
			_rangeStart = rangeStart;
			_rangeEnd = rangeEnd;
			Reload();
		}
		/// <summary>
		/// Reloads all visible items by creating a background worker
		/// <para>Timeline range is equal to values specified by last call of FetchRange</para>
		/// </summary>
		/// <remarks>This method uses a background worker to load items asynchronously</remarks>
		public void Reload()
		{
			_uow = new Dal.SoheilEdmContext();
			_blockDataService = new DataServices.BlockDataService(_uow);

			if(_backgroundWorker!=null)
			{
				_backgroundWorker.CancelAsync();
				//return;timers? see _backgroundWorker_Disposed
			}
			_backgroundWorker = new System.ComponentModel.BackgroundWorker
			{
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true,
			};
			_backgroundWorker.DoWork += _backgroundWorker_DoWork;
			_backgroundWorker.ProgressChanged += _backgroundWorker_ProgressChanged;
			_backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
			_backgroundWorker.Disposed += _backgroundWorker_Disposed;
			_backgroundWorker.RunWorkerAsync();
		}

		/// <summary>
		/// BackgroundWorker used in Reload (or FetchRange) method uses this method to load items from database
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			//cancel the worker if needed
			System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;
			if (worker.CancellationPending == true)
			{
				e.Cancel = true;
				return;
			}

			bool acquiredLock = false;
			try
			{
				//enter worker's critical segment
				System.Threading.Monitor.TryEnter(_lockObject, 2000, ref acquiredLock);
				if (acquiredLock)
				{
					//load blocks within the modified range
					var rangeStart = _rangeStart.AddHours(_startHourMargin);
					var rangeEnd = _rangeEnd.AddHours(_endHourMargin);
					var blockIds = new DataServices.BlockDataService(_uow).GetIdsInRange(rangeStart, rangeEnd).ToArray();

					//change the background worker's progress for each block in the range
					foreach (var blockId in blockIds)
					{
						bool err = true;
						while (err)
							try
							{
								System.Threading.Thread.Sleep(_workerSleepTime);
								//load full data
								var data = new BlockFullData(blockId);
								//load vm thru worker
								worker.ReportProgress(1, data);
								err = false;
							}
							catch { }
					}

					//load npt Ids within the modified range
					var nptIds = new DataServices.NPTDataService(_uow).GetIdsInRange(rangeStart, rangeEnd).ToArray();

					//change the background worker's progress for each npt in the range
					foreach (var nptId in nptIds)
					{
						System.Threading.Thread.Sleep(_workerSleepTime);
						worker.ReportProgress(2, nptId);
					}
				}
			}
			finally
			{
				if (acquiredLock)
				{
					System.Threading.Monitor.Exit(_lockObject);
				}
			}
		}

		/// <summary>
		/// BackgroundWorker used in Reload (or FetchRange) method uses this method to add items to Vm
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
		{
			//tries 3 times if error occurs
			int tries = 3;
			while (true)
			{
				try
				{
					//add blocks to Vm within range
					if (e.ProgressPercentage == 1)
					{
						var data = e.UserState as BlockFullData;
						if (BlockFullData.IsNull(data)) return;
						var vm = AddItem(data);
					}
					//add npts to Vm within range
					else if (e.ProgressPercentage == 2)
					{
						var nptId = (int)e.UserState;
						var vm = AddNPT(nptId);
						vm.ViewMode = ViewMode;
					}

					//end if it was successful
					return;
				}
				catch
				{
					//try again if error occured
					if (--tries < 0) return;
				}
			}
		}

		/// <summary>
		/// BackgroundWorker used in Reload (or FetchRange) method uses this method in the end to remove extra items
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void _backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			//Remove outside-the-window items
			if (!e.Cancelled)
			{
				var rangeStart = _rangeStart.AddHours(_startHourMargin);
				var rangeEnd = _rangeEnd.AddHours(_endHourMargin);

				int count = this.Count;
				for (int i = 0; i < count; i++)
				{
					//remove outside-the-window npts
					var removeNptList = this[i].NPTs.Where(x =>
						x.StartDateTime.AddSeconds(x.DurationSeconds) < rangeStart ||
						x.StartDateTime > rangeEnd).ToArray();
					foreach (var npt in removeNptList)
					{
						this[i].NPTs.Remove(npt);
					}
					//remove outside-the-window blocks
					var removeList = this[i].Blocks.Where(x =>
						x.StartDateTime.AddSeconds(x.DurationSeconds) < rangeStart ||
						x.StartDateTime > rangeEnd).ToArray();
					foreach (var block in removeList)
					{
						this[i].Blocks.Remove(block);
					}
				}
			}

			//dispose the worker
			System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;
			worker.Dispose();
		}

		void _backgroundWorker_Disposed(object sender, EventArgs e)
		{
			_backgroundWorker = null;
		}

		#endregion

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
		public BlockVm AddItem(BlockFullData data)
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
		public void RemoveItem(BlockVm vm)
		{
			if (vm == null) return;
			try
			{
				_blockDataService.DeleteModel(vm.Model);
				if (BlockRemoved != null) BlockRemoved(vm.Id);
				this[vm.RowIndex].Blocks.RemoveWhere(x => x.Id == vm.Id);
			}
			catch (Exception ex) { vm.Message.AddEmbeddedException(ex.Message); }
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
		public void RemoveNPT(NPTVm vm)
		{
			try
			{
				if (vm != null && NptRemoved != null) NptRemoved(vm.Id); 
				this[vm.RowIndex].NPTs.RemoveWhere(x => x.Id == vm.Id);
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
			vm.InsertSetupBefore = new Commands.Command(async o =>
			{
				//the following part is async version of "var result = tmp.InsertSetupBeforeTask(Id)"
				var tmp = _blockDataService;
				var id = vm.Id;
				var result = await Task.Run(() => tmp.InsertSetupBeforeBlock(id));

				//in case of error callback with result
				if (result.IsSaved) Reload();
				else vm.InsertSetupBeforeCallback(result);
			});
			vm.DeleteItemCommand = new Commands.Command(o =>
			{
				try { RemoveItem(vm); }
				catch (Exception exp) { vm.Message.AddEmbeddedException(exp.Message); }
			});
		}
		#endregion
	}
}
