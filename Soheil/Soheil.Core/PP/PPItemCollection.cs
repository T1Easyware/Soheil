using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Soheil.Core.ViewModels.PP;
using Soheil.Common;
using System.Data;
using System.Data.Entity;

namespace Soheil.Core.PP
{
	public class PPItemCollection : ObservableCollection<PPStationVm>
	{
		DateTime _rangeStart;
		DateTime _rangeEnd;
		public PPTableVm PPTable { get; private set; }
		System.Windows.Threading.Dispatcher _dispatcher;
		public DataServices.NPTDataService NPTDataService { get { return PPTable.NPTDataService; } }
		public DataServices.BlockDataService BlockDataService { get { return PPTable.BlockDataService; } }
		public DataServices.JobDataService JobDataService { get { return PPTable.JobDataService; } }
		System.Threading.Thread _thread;
		Object _threadLock;

		public PPItemCollection(PPTableVm parent)
		{
			PPTable = parent;
			ViewMode = PPViewMode.Simple;
			_dispatcher = parent.Dispatcher;
			_thread = new System.Threading.Thread(addRangeThreadFunc);
			_threadLock = new Object();
		}


		public void FetchRange(DateTime rangeStart, DateTime rangeEnd)
		{
			_rangeStart = rangeStart;
			_rangeEnd = rangeEnd;
			threadStarter();
		}
		public void Reload()
		{
			threadStarter();
		}


		//Ease of use
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
		public void AddItem(Model.Block model)
		{
			try
			{
				var container = GetRowContaining(model);
				var currentVm = container.FirstOrDefault(x => x.Id == model.Id);
				if (currentVm != null) container.Remove(currentVm);
				container.Add(new BlockVm(model, PPTable, model.StateStation.Station.Index));
			}
			catch { }
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
		public void AddNPT(Model.NonProductiveTask model)
		{
			try
			{
				if (model is Model.Setup)
				{
					var setupModel = model as Model.Setup;
					var container = this[setupModel.Warmup.Station.Index].NPTs;
					var currentVm = container.FirstOrDefault(x => x.Id == model.Id);
					if (currentVm != null) container.Remove(currentVm);
					container.Add(new SetupVm(setupModel, this));
				}
				else throw new NotImplementedException();//???
			}
			catch { }
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


		void threadStarter()
		{
			lock (_threadLock)
			{
				if (_thread.IsAlive)
					_thread.Abort();
				while (_thread.IsAlive) ;
				_thread = new System.Threading.Thread(addRangeThreadFunc);
				_thread.Start();
			}
		}
		void addRangeThreadFunc()
		{
			try
			{
					var rangeStart = _rangeStart.AddHours(-1);
					var rangeEnd = _rangeEnd.AddHours(3);
					var blockModels = BlockDataService.GetInRange(rangeStart, rangeEnd).ToList();
					var nptModels = NPTDataService.GetInRange(rangeStart, rangeEnd).ToList();

					//add inside-the-window blocks
					foreach (var model in blockModels)
					{
						//if (!this[model].Any(x => x.Id == model.Id))
							_dispatcher.InvokeInBackground(() => AddItem(model));
					}
					//add inside-the-window npts
					foreach (var model in nptModels)
					{
						//if (!this[model].Any(x => x.Id == model.Id))
							_dispatcher.InvokeInBackground(() => AddNPT(model));
					}

					int count = this.Count;
					for (int i = 0; i < count; i++)
					{
						_dispatcher.BeginInBackground((I, paramRangeStart, paramRangeEnd) =>
						{
							//remove outside-the-window npts
							var removeNptList = this[I].NPTs.Where(x =>
								x.StartDateTime.AddSeconds(x.DurationSeconds) < paramRangeStart ||
								x.StartDateTime > paramRangeEnd).ToArray();
							foreach (var npt in removeNptList)
							{
								this[I].NPTs.Remove(npt);
							}
							//remove outside-the-window blocks
							var removeList = this[I].Blocks.Where(x =>
								x.StartDateTime.AddSeconds(x.DurationSeconds) < paramRangeStart ||
								x.StartDateTime > paramRangeEnd).ToArray();
							foreach (var block in removeList)
							{
								this[I].Blocks.Remove(block);
							}
						}, i, rangeStart, rangeEnd);
					}
			}
			catch { }
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
	}
}
