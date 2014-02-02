using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace Soheil.Core.PP
{
	public class TaskManager
	{
		/*List<Model.Task> _tasks;
		TaskCollection _taskCollection;
		Dispatcher _dispatcher;
		DataServices.TaskDataService _taskDataService;
		System.Threading.Thread _thread;
		DateTime _rangeStart;
		DateTime _rangeEnd;

		public TaskManager(TaskCollection taskCollection, Dispatcher dispatcher, DataServices.TaskDataService taskDataService)
		{
			_taskCollection = taskCollection;
			_dispatcher = dispatcher;
			_taskDataService = taskDataService;
			_tasks = new List<Model.Task>();
			_thread = new System.Threading.Thread(addRangeThreadFunc);
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

		void threadStarter()
		{
			if (_thread.IsAlive)
				_thread.Abort();

			_thread.Start();

		}
		void addRangeThreadFunc()
		{
			using (var context = new Dal.SoheilEdmContext())
			{
				var rangeStart = _rangeStart.AddHours(-1);
				var rangeEnd = _rangeEnd.AddHours(3);

				var models = _taskDataService.GetInRange(rangeStart, rangeEnd, context).ToList();
				{
					//add inside-the-box tasks
					foreach (var model in models)
					{
						if (!_tasks.Any(x => x.Id == model.Id))
						{
							_tasks.Add(model);
							_dispatcher.Invoke(() =>
							{
							});
						}
					}
					//remove outside-the-box tasks
					foreach (var task in this.Where(x =>
						x.StartDateTime.AddSeconds(x.DurationSeconds) < rangeStart &&
						x.StartDateTime.AddSeconds(x.DurationSeconds) > rangeEnd))
					{
						this.Remove(task);
					}
				}
			}
		}*/
	}
}
