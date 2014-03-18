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
	public class PPItemCollection : ObservableCollection<PPStationVm>
	{

		#region Members, props, consts
		public PPTableVm PPTable { get; private set; }
		System.ComponentModel.BackgroundWorker _backgroundWorker;

		DateTime _rangeStart;
		DateTime _rangeEnd;

		//DataServices
		public DataServices.NPTDataService NPTDataService { get { return PPTable.NPTDataService; } }
		public DataServices.BlockDataService BlockDataService { get { return PPTable.BlockDataService; } }
		public DataServices.JobDataService JobDataService { get { return PPTable.JobDataService; } }

		private static readonly object _LOCK = new object();
		/// <summary>
		/// Number of milliseconds to sleep when loading each item
		/// </summary>
		private const int _workerSleepTime = 50;
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
		/// <summary>
		/// Occurs when a block is removed from this collection. parameter is blockId
		/// </summary>
		public event Action<int> BlockRemoved;
		/// <summary>
		/// Occurs when an npt is removed from this collection. parameter is nptId
		/// </summary>
		public event Action<int> NptRemoved;
		/// <summary>
		/// Occurs when a job is removed from this collection. parameter is jobVm
		/// </summary>
		public event Action<PPJobVm> JobRemoved;
		/// <summary>
		/// Occurs when an npt is removed from this collection. parameter is nptId
		/// </summary>
		public event Action<Editor.PPEditorBlock> TaskEditorUpdated;
		/// <summary>
		/// Occurs when an npt is removed from this collection. parameter is nptId
		/// </summary>
		public event Action TaskEditorReset;
		/// <summary>
		/// Occurs when an npt is removed from this collection. parameter is nptId
		/// </summary>
		public event Action<PPJobVm> JobEditorUpdated;
		/// <summary>
		/// Occurs when an npt is removed from this collection. parameter is nptId
		/// </summary>
		public event Action JobEditorReset;
		/// <summary>
		/// Occurs when an npt is removed from this collection. parameter is nptId
		/// </summary>
		public event Action<BlockVm> EditBlockReportStarted;
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

			//enter worker's critical segment
			if (System.Threading.Monitor.TryEnter(_LOCK, 2000))
			{
				try
				{
					//load blocks within the modified range
					var rangeStart = _rangeStart.AddHours(_startHourMargin);
					var rangeEnd = _rangeEnd.AddHours(_endHourMargin);
					var blockIds = BlockDataService.GetIdsInRange(rangeStart, rangeEnd).ToArray();

					//change the background worker's progress for each block in the range
					foreach (var blockId in blockIds)
					{
						bool err = true;
						while (err)
							try
							{
								System.Threading.Thread.Sleep(_workerSleepTime);
								//load full data
								var data = new BlockFullData(BlockDataService, blockId);
								//load vm thru worker
								worker.ReportProgress(1, data);
								err = false;
							}
							catch { }
					}

					//load npts within the modified range
					var nptModels = NPTDataService.GetInRange(rangeStart, rangeEnd).ToArray();

					//change the background worker's progress for each npt in the range
					foreach (var npt in nptModels)
					{
						System.Threading.Thread.Sleep(_workerSleepTime);
						worker.ReportProgress(2, npt);
					}
				}
				finally
				{
					System.Threading.Monitor.Exit(_LOCK);
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

						var vm = AddItem(data.Model);
						vm.Reload(data);
					}
					//add npts to Vm within range
					else if (e.ProgressPercentage == 2)
					{
						var npt = e.UserState as Model.NonProductiveTask;
						var vm = AddNPT(npt);

						//fill the vm from npt model
						NPTDataService.UpdateViewModel(vm);
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
		/// Finds an existing BlockVm in this collection
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public BlockVm FindItem(Model.Block model)
		{
			try
			{
				return GetRowContaining(model).FirstOrDefault(x => x.Id == model.Id);
			}
			catch { return null; }
		} 
		/// <summary>
		/// Searches in all stations for an existing BlockVm
		/// <para>Slow - not recommended for bottlenecks, use FindItem instead</para>
		/// </summary>
		/// <param name="id">Id of Block to find</param>
		/// <returns>ViewModel of block from this collection</returns>
		public BlockVm FindBlockById(int id)
		{
			foreach (var row in this)
			{
				var block = row.Blocks.FirstOrDefault(y => y.Id == id);
				if (block != null) return block;
			}
			return null;
		}
		/// <summary>
		/// Searches in all stations for existing instances of <see cref="BlockVm"/> that are part of the given job
		/// <para>Slow - not recommended for bottlenecks, use FindItem instead</para>
		/// </summary>
		/// <param name="id">Id of a job</param>
		/// <returns>collection of BlockVms from this collection</returns>
		public IEnumerable<BlockVm> FindBlocksByJobId(int id)
		{
			List<BlockVm> blocks = new List<BlockVm>();
			foreach (var row in this)
			{
				blocks.AddRange(row.Blocks.Where(y => y.Id == id));
			}
			return blocks;
		}
		/// <summary>
		/// Adds a new BlockVm to this collection with the given model and sets all its commands
		/// </summary>
		/// <remarks>
		/// Finds the row of items which this model should be in,
		/// <para>converts the model into VM and adds it to that row</para>
		/// <para>returns the VM</para>
		/// </remarks>
		/// <param name="model"></param>
		/// <returns></returns>
		public BlockVm AddItem(Model.Block model)
		{
			try
			{
				//find the container row of blocks
				var container = GetRowContaining(model);
				//find any existing vm with same id
				var currentVm = container.FirstOrDefault(x => x.Id == model.Id);
				//if already exist remove it
				if (currentVm != null) container.Remove(currentVm);

				//create viewmodel for the new block
				var vm = new BlockVm(model, this, model.StateStation.Station.Index);

				#region block commands
				vm.ReloadBlockCommand = new Commands.Command(o =>
				{
					var data = new Soheil.Core.PP.BlockFullData(BlockDataService, vm.Id);
					vm.Reload(data);

					//check for selected things

					//check if the SelectedJobId in PPTable is the same as this Job
					if (vm.Job != null)
					{
						if (PPTable.SelectedJobId == vm.Job.Id)
						{
							vm.IsJobSelected = true;
						}
					}
					//check if the SelectedBlock in PPTable is the same as this block
					if (PPTable.SelectedBlock == null)
					{
						ViewMode = PPViewMode.Simple;
					}
					else ViewMode = (PPTable.SelectedBlock.Id == vm.Id) ? PPViewMode.Report : PPViewMode.Simple;
				});
				vm.AddBlockToEditorCommand = new Commands.Command(o =>
				{
					try { if (TaskEditorUpdated != null) TaskEditorUpdated(new Editor.PPEditorBlock(vm.Model)); }
					catch (Exception exp) { vm.Message.AddEmbeddedException(exp.Message); }
				}, () => vm.Model != null);
				vm.EditItemCommand = new Commands.Command(o =>
				{
					try
					{
						if (TaskEditorReset != null) TaskEditorReset();
						if (TaskEditorUpdated != null) TaskEditorUpdated(new Editor.PPEditorBlock(vm.Model));
					}
					catch (Exception exp) { vm.Message.AddEmbeddedException(exp.Message); }
				});
				vm.AddJobToEditorCommand = new Commands.Command(o =>
				{
					try { if (JobEditorUpdated != null) JobEditorUpdated(vm.Job); }
					catch (Exception exp) { vm.Message.AddEmbeddedException(exp.Message); }
				}, () =>
				{
					if (vm.Job == null) return false;
					if (vm.Job.Id == 0) return false;
					return true;
				});
				vm.EditJobCommand = new Commands.Command(o =>
				{
					try
					{
						if (JobEditorReset != null) JobEditorReset();
						if (JobEditorUpdated != null) JobEditorUpdated(vm.Job);
					}
					catch (Exception exp) { vm.Message.AddEmbeddedException(exp.Message); }
				}, () =>
				{
					if (vm.Job == null) return false;
					if (vm.Job.Id == 0) return false;
					return true;
				});
				vm.EditReportCommand = new Commands.Command(blockVm =>
				{
					try { if (EditBlockReportStarted != null) EditBlockReportStarted(vm); }
					catch (Exception exp) { vm.Message.AddEmbeddedException(exp.Message); }
				});
				vm.DeleteItemCommand = new Commands.Command(o =>
				{
					try { RemoveItem(vm); }
					catch (Exception exp) { vm.Message.AddEmbeddedException(exp.Message); }
				});
				vm.DeleteJobCommand = new Commands.Command(o =>
				{
					try { if (JobRemoved != null) JobRemoved(vm.Job); }
					catch (RoutedException exp)
					{
						if (exp.Target is PPTaskVm)
							(exp.Target as PPTaskVm).Message.AddEmbeddedException(exp.Message);
						else //if(exp.Target is BlockVm)
							vm.Message.AddEmbeddedException(exp.Message);
					}
					catch (Exception exp) { vm.Message.AddEmbeddedException(exp.Message); }
				}, () => { return vm.Job != null; });
				vm.InsertSetupBefore = new Commands.Command(async o =>
				{
					//the following part is async version of "var result = tmp.InsertSetupBeforeTask(Id)"
					var tmp = BlockDataService;
					var id = vm.Id;
					var result = await Task.Run(() => tmp.InsertSetupBeforeBlock(id));

					//in case of error callback with result
					if (result.IsSaved) Reload();
					else vm.InsertSetupBeforeCallback(result);
				}); 
				#endregion

				//add the viewmodel to its container
				container.Add(vm);
				return vm;
			}
			catch { return null; }
		}
		/// <summary>
		/// Removes a blockVm from this collection based on its Id
		/// </summary>
		/// <param name="model">model of block to remove (Id and Station are used)</param>
		public void RemoveItem(Model.Block model)
		{
			try
			{
				GetRowContaining(model).RemoveWhere(x => x.Id == model.Id);
			}
			catch { }
		}
		/// <summary>
		/// Removes a blockVm from this collection based on its Id
		/// </summary>
		/// <param name="vm">vm of block to remove (Id and RowIndex are used)</param>
		public void RemoveItem(BlockVm vm)
		{
			try
			{
				if (vm != null && BlockRemoved != null) BlockRemoved(vm.Id);
				this[vm.RowIndex].Blocks.RemoveWhere(x => x.Id == vm.Id);
			}
			catch { }
		}

		#endregion

		#region NPT Operations
		/// <summary>
		/// Returns the row of npts which contains the provided model
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public ObservableCollection<NPTVm> GetRowContaining(Model.NonProductiveTask model)
		{
			if (model is Model.Setup)
				return this[(model as Model.Setup).Warmup.Station.Index].NPTs;
			else
				throw new NotImplementedException();//???
		}
		/// <summary>
		/// Finds an existing NPTVm in this collection
		/// </summary>
		/// <param name="id">id of npt</param>
		/// <param name="stationIndex">index of station</param>
		/// <returns></returns>
		public NPTVm FindNPT(int id, int stationIndex)
		{
			return this[stationIndex].NPTs.FirstOrDefault(y => y.Id == id);
		}
		/// <summary>
		/// Searches in all stations for an existing NPTVm
		/// <para>Slow - not recommended for bottlenecks, use FindItem instead</para>
		/// </summary>
		/// <param name="id">Id of NPT to find</param>
		/// <returns>ViewModel of npt from this collection</returns>
		public NPTVm FindNPTById(int id)
		{
			foreach (var row in this)
			{
				var npt = row.NPTs.FirstOrDefault(y => y.Id == id);
				if (npt != null) return npt;
			}
			return null;
		}
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
		public NPTVm AddNPT(Model.NonProductiveTask model)
		{
			try
			{
				if (model is Model.Setup)
				{
					var setupModel = model as Model.Setup;
					//find the container row of npts that this setup belongs to
					var container = this[setupModel.Warmup.Station.Index].NPTs;
					//find any existing setup vm that matches the Id
					var currentVm = container.FirstOrDefault(x => x.Id == model.Id);
					//if setup vm already exists remove it
					if (currentVm != null) container.Remove(currentVm);
					//create a new SetupVm
					var vm = new SetupVm(setupModel, this);
					//add it to container
					container.Add(vm);
					return vm;
				}
				else throw new NotImplementedException();//???
			}
			catch { return null; }
		}
		/// <summary>
		/// Removes a NptVm from this collection based on its Id
		/// </summary>
		/// <param name="model">model of block to remove (Id and Station are used)</param>
		public void RemoveNPT(Model.NonProductiveTask model)
		{
			try
			{
				if (model != null && NptRemoved != null) NptRemoved(model.Id);
				if (model is Model.Setup)
				{
					var setupModel = model as Model.Setup;
					this[setupModel.Warmup.Station.Index].NPTs.RemoveWhere(x => x.Id == setupModel.Id);
				}
				else throw new NotImplementedException();//???
			}
			catch { }
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
	}
}
