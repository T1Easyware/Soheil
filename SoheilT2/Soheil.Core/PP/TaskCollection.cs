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
	public class TaskCollection : ObservableCollection<PPStationVm>
	{
		DateTime _rangeStart;
		DateTime _rangeEnd;
		public PPTableVm Parent { get; private set; }
		System.Windows.Threading.Dispatcher _dispatcher;
		public DataServices.NPTDataService NPTDataService { get { return Parent.NPTDataService; } }
		public DataServices.TaskDataService TaskDataService { get { return Parent.TaskDataService; } }
		public DataServices.JobDataService JobDataService { get { return Parent.JobDataService; } }
		System.Threading.Thread _thread;
		Object _threadLock;

		public TaskCollection(PPTableVm parent)
		{
			Parent = parent;
			ViewMode = PPTaskViewMode.Simple;
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
		public ObservableCollection<PPTaskVm> this[Model.Task model]
		{
			get { return this[model.StateStation.Station.Index].Tasks; }
		}
		public ObservableCollection<NPTVm> this[Model.NonProductiveTask model]
		{
			get
			{
				if (model is Model.Setup)
					return this[(model as Model.Setup).Warmup.Station.Index].NPTs;
				else
					throw new NotImplementedException();//???
			}
		}

		#region Task Operations
		/// <summary>
		/// Slow (not recommended)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public PPTaskVm FindTaskById(int id)
		{
			foreach (var row in this)
			{
				var task = row.Tasks.FirstOrDefault(y => y.Id == id);
				if (task != null) return task;
			}
			return null;
		}
		public void AddTask(Model.Task model)
		{
			try
			{
				var container = this[model];
				var currentVm = container.FirstOrDefault(x => x.Id == model.Id);
				if (currentVm != null) container.Remove(currentVm);
				container.Add(new PPTaskVm(model, this, model.StateStation.Station.Index));
			}
			catch { }
		}
		public void RemoveTask(Model.Task model)
		{
			try
			{
				if (Parent.SelectedTask != null && model != null)
					if (Parent.SelectedTask.Id == model.Id)
						Parent.SelectedTask = null;
				this[model].RemoveWhere(x => x.Id == model.Id);
			}
			catch { }
		}
		public void RemoveTask(PPTaskVm vm)
		{
			try
			{
				if (Parent.SelectedTask == vm)
					Parent.SelectedTask = null;
				this[vm.RowIndex].Tasks.RemoveWhere(x => x.Id == vm.Id);
			}
			catch { }
		}
		public PPTaskVm FindTask(Model.Task model)
		{
			try
			{
				return this[model].FirstOrDefault(x => x.Id == model.Id);
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
				if (Parent.SelectedNPT != null && model != null)
					if (Parent.SelectedNPT.Id == model.Id) 
						Parent.SelectedNPT = null;
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
				if (Parent.SelectedNPT == vm) Parent.SelectedNPT = null;
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
				using (var context = new Dal.SoheilEdmContext())
				{
					var rangeStart = _rangeStart.AddHours(-1);
					var rangeEnd = _rangeEnd.AddHours(3);
					var taskModels = TaskDataService.GetInRange(rangeStart, rangeEnd, context).ToList();
					var nptModels = NPTDataService.GetInRange(rangeStart, rangeEnd, context).ToList();

					//add inside-the-box tasks
					foreach (var model in taskModels)
					{
						//if (!this[model].Any(x => x.Id == model.Id))
							_dispatcher.InvokeInBackground(() => AddTask(model));
					}
					//add inside-the-box npts
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
							//remove outside-the-box npts
							var removeNptList = this[I].NPTs.Where(x =>
								x.StartDateTime.AddSeconds(x.DurationSeconds) < paramRangeStart ||
								x.StartDateTime > paramRangeEnd).ToArray();
							foreach (var npt in removeNptList)
							{
								this[I].NPTs.Remove(npt);
							}
							//remove outside-the-box tasks
							var removeList = this[I].Tasks.Where(x =>
								x.StartDateTime.AddSeconds(x.DurationSeconds) < paramRangeStart ||
								x.StartDateTime > paramRangeEnd).ToArray();
							foreach (var task in removeList)
							{
								this[I].Tasks.Remove(task);
							}
						}, i, rangeStart, rangeEnd);
					}
				}
			}
			catch { }
		}

		private PPTaskViewMode _viewMode;
		public PPTaskViewMode ViewMode
		{
			get { return _viewMode; }
			set
			{
				_viewMode = value;
				foreach (var station in this)
				{
					foreach (var task in station.Tasks)
					{
						task.ViewMode = value;
					}
				}
			}
		}
	}
}
