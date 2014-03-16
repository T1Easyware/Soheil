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

namespace Soheil.Core.ViewModels.PP
{
	public class PPItemCollection : ObservableCollection<PPStationVm>
	{
		DateTime _rangeStart;
		DateTime _rangeEnd;
		public PPTableVm PPTable { get; private set; }
		public DataServices.NPTDataService NPTDataService { get { return PPTable.NPTDataService; } }
		public DataServices.BlockDataService BlockDataService { get { return PPTable.BlockDataService; } }
		public DataServices.JobDataService JobDataService { get { return PPTable.JobDataService; } }

		System.ComponentModel.BackgroundWorker _backgroundWorker;

		public PPItemCollection(PPTableVm parent)
		{
			PPTable = parent;
			ViewMode = PPViewMode.Simple;
		}

		private PPViewMode _viewMode;
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

		#region Parallel loading
		public void FetchRange(DateTime rangeStart, DateTime rangeEnd)
		{
			_rangeStart = rangeStart;
			_rangeEnd = rangeEnd;
			Reload();
		}
		private static readonly object _LOCK = new object();
		/// <summary>
		/// Reloads all visible items by creating a background worker
		/// </summary>
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
					var rangeStart = _rangeStart.AddHours(-1);
					var rangeEnd = _rangeEnd.AddHours(3);
					var blockModels = BlockDataService.GetInRange(rangeStart, rangeEnd).ToArray();
					foreach (var block in blockModels)
					{
						bool err = true;
						while (err)
							try
							{
								System.Threading.Thread.Sleep(50);
								//load data
								var data = new BlockFullData(BlockDataService, block.Id);
								//load vm thru worker
								worker.ReportProgress(1, data);
								err = false;
							}
							catch { }
					}
					var nptModels = NPTDataService.GetInRange(rangeStart, rangeEnd).ToArray();
					foreach (var npt in nptModels)
					{
						System.Threading.Thread.Sleep(50);
						worker.ReportProgress(2, npt);
					}
				}
				finally
				{
					System.Threading.Monitor.Exit(_LOCK);
				}
			}
		}
		private void _backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
		{
			System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;
			//try
			{
				//add inside-the-window blocks
				if (e.ProgressPercentage == 1)
				{
					var data = e.UserState as BlockFullData;
					if (BlockFullData.IsNull(data)) return;

					var vm = AddItem(data.Model);
					vm.Reload(data);
				}
				//add inside-the-window npts
				else if (e.ProgressPercentage == 2)
				{
					var npt = e.UserState as Model.NonProductiveTask;
					var vm = AddNPT(npt);

					//fill the vm from npt model
					if (!NPTDataService.UpdateViewModel(vm))
						RemoveNPT(vm);
					else
						vm.ViewMode = ViewMode;
				}
			}
			//catch { }
		}
		void _backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;
			//Remove outside-the-window items
			if (!e.Cancelled)
			{
				int count = this.Count;
				for (int i = 0; i < count; i++)
				{
					//remove outside-the-window npts
					var removeNptList = this[i].NPTs.Where(x =>
						x.StartDateTime.AddSeconds(x.DurationSeconds) < _rangeStart ||
						x.StartDateTime > _rangeEnd).ToArray();
					foreach (var npt in removeNptList)
					{
						this[i].NPTs.Remove(npt);
					}
					//remove outside-the-window blocks
					var removeList = this[i].Blocks.Where(x =>
						x.StartDateTime.AddSeconds(x.DurationSeconds) < _rangeStart ||
						x.StartDateTime > _rangeEnd).ToArray();
					foreach (var block in removeList)
					{
						this[i].Blocks.Remove(block);
					}
				}
			}
			worker.Dispose();
		}

		void _backgroundWorker_Disposed(object sender, EventArgs e)
		{
			_backgroundWorker = null;
		}

		#endregion

		#region Ease of use
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
		#endregion

		#region Task Operations
		/// <summary>
		/// Slow (not recommended)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
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
		/// VERY Slow (not recommended)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
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
		/// Finds the row of items which this model should be in,
		/// <para>converts the model into VM and adds it to that row</para>
		/// <para>returns the VM</para>
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public BlockVm AddItem(Model.Block model)
		{
			try
			{
				var container = GetRowContaining(model);
				var currentVm = container.FirstOrDefault(x => x.Id == model.Id);
				if (currentVm != null) container.Remove(currentVm);
				var vm = new BlockVm(model, this, model.StateStation.Station.Index);
				vm.AddBlockToEditorStarted += editorBlockVm =>
				{
					PPTable.TaskEditor.BlockList.Add(editorBlockVm);
					PPTable.TaskEditor.SelectedBlock = PPTable.TaskEditor.BlockList.Last();
				};
				vm.EditBlockStarted += editorBlockVm =>
				{
					PPTable.TaskEditor.Reset();
					PPTable.TaskEditor.IsVisible = true;
					PPTable.JobEditor.IsVisible = false;
					PPTable.TaskEditor.BlockList.Add(editorBlockVm);
					PPTable.TaskEditor.SelectedBlock = PPTable.TaskEditor.BlockList.Last();
				};
				vm.AddJobToEditorStarted += jobVm =>
				{
					PPTable.JobEditor.Append(jobVm);
				};
				vm.EditJobStarted += jobVm =>
				{
					PPTable.TaskEditor.IsVisible = false;
					PPTable.JobEditor.IsVisible = true;
					PPTable.JobEditor.Reset();
					PPTable.JobEditor.Append(jobVm);
				};
				vm.EditReportStarted += blockVm =>
				{
					blockVm.BlockReport = new BlockReportVm(blockVm);
					PPTable.SelectedBlock = blockVm;
				};
				vm.DeleteBlockStarted += blockVm =>
				{
					PPTable.BlockDataService.DeleteModelById(blockVm.Id);
					RemoveItem(blockVm);
				};
				vm.DeleteJobStarted += jobVm =>
				{
					PPTable.JobDataService.DeleteModel(jobVm.Id);
					PPTable.RemoveBlocks(jobVm);
				};
				//vm.InsertSetupStarted += async (blockVm, callback) =>
				//{

				//};
				container.Add(vm);
				return vm;
			}
			catch { return null; }
		}
		public void RemoveItem(Model.Block model)
		{
			try
			{
				if (PPTable.SelectedBlock != null && model != null)
					if (PPTable.SelectedBlock.Id == model.Id)
						PPTable.SelectedBlock = null;
				GetRowContaining(model).RemoveWhere(x => x.Id == model.Id);
			}
			catch { }
		}
		public void RemoveItem(BlockVm vm)
		{
			try
			{
				if (PPTable.SelectedBlock == vm)
					PPTable.SelectedBlock = null;
				this[vm.RowIndex].Blocks.RemoveWhere(x => x.Id == vm.Id);
			}
			catch { }
		}
		public BlockVm FindItem(Model.Block model)
		{
			try
			{
				return GetRowContaining(model).FirstOrDefault(x => x.Id == model.Id);
			}
			catch { return null; }
		} 
		#endregion

		#region NPT Operations
		/// <summary>
		/// Slow (not recommended)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public NPTVm FindNPTById(int id)
		{
			foreach (var row in this)
			{
				var npt = row.NPTs.FirstOrDefault(y => y.Id == id);
				if (npt != null) return npt;
			}
			return null;
		}
		public NPTVm FindNPT(int id, int stationIndex)
		{
			return this[stationIndex].NPTs.FirstOrDefault(y => y.Id == id);
		}
		/// Finds the row of items which this model should be in,
		/// <para>converts the model into VM and adds it to that row</para>
		/// <para>returns the VM</para>
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public NPTVm AddNPT(Model.NonProductiveTask model)
		{
			try
			{
				if (model is Model.Setup)
				{
					var setupModel = model as Model.Setup;
					var container = this[setupModel.Warmup.Station.Index].NPTs;
					var currentVm = container.FirstOrDefault(x => x.Id == model.Id);
					if (currentVm != null) container.Remove(currentVm);
					var vm = new SetupVm(setupModel, this);
					container.Add(vm);
					return vm;
				}
				else throw new NotImplementedException();//???
			}
			catch { return null; }
		}
		public void RemoveNPT(Model.NonProductiveTask model)
		{
			try
			{
				if (PPTable.SelectedNPT != null && model != null)
					if (PPTable.SelectedNPT.Id == model.Id) 
						PPTable.SelectedNPT = null;
				if (model is Model.Setup)
				{
					var setupModel = model as Model.Setup;
					this[setupModel.Warmup.Station.Index].NPTs.RemoveWhere(x => x.Id == setupModel.Id);
				}
				else throw new NotImplementedException();//???
			}
			catch { }
		}
		public void RemoveNPT(NPTVm vm)
		{
			try
			{
				if (PPTable.SelectedNPT == vm) PPTable.SelectedNPT = null;
				this[vm.RowIndex].NPTs.RemoveWhere(x => x.Id == vm.Id);
			}
			catch { }
		}
		#endregion
	}
}
