using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Soheil.Core.DataServices;
using Soheil.Dal;

namespace Soheil.Core.PP.Smart
{
	internal class SmartManager
	{
		internal SmartManager(BlockDataService blockDs, NPTDataService nptDs)
		{
			_blockDataService = blockDs;
			_nptDataService = nptDs;
			SmartJobs = new List<SmartJob>();
			_reserve = new List<KeyValuePair<int, List<SmartRange>>>();
		}

		internal List<SmartJob> SmartJobs { get; private set; }
		protected BlockDataService _blockDataService;
		protected NPTDataService _nptDataService;

		#region Reserve
		/// <summary>
		/// List of <stationId, tasks or npts> where tasks is a list of made-up SmartRanges
		/// </summary>
		private List<KeyValuePair<int, List<SmartRange>>> _reserve;
		
		/// <summary>
		/// Reserves a task in manager (FindNextFreeSpace will consider this task as a reserved area)
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="stateStation"></param>
		/// <param name="job"></param>
		internal void Reserve(DateTime startTime, Model.StateStation stateStation, SmartJob job)
		{
			var reserve = _reserve.FirstOrDefault(x => x.Key == stateStation.Station.Id).Value;
			int durationSeconds = (int)Math.Ceiling(job.Quantity * stateStation.MaxCycleTime);
			var range = SmartRange.NewReserve(startTime, durationSeconds, stateStation);

			//insert to reserve (but maintain its order)
			var insertionPoint = reserve.FirstOrDefault(x => x.StartDT > startTime);
			if (insertionPoint == null) reserve.Add(range);
			else
			{
				int index = reserve.IndexOf(insertionPoint);
				reserve.Insert(index, range);
			}
		}
		/// <summary>
		/// Reserves a setup in manager (FindNextFreeSpace will consider this setup as a reserved area)
		/// </summary>
		/// <param name="newSetup">reserved smartRange from previous step which has newSetup type</param>
		/// <param name="job"></param>
		internal void Reserve(SmartRange newSetup, SmartJob job)
		{
			var reserve = _reserve.FirstOrDefault(x => x.Key == newSetup.StationId).Value;
			var range = SmartRange.NewReserve(newSetup.StartDT, newSetup.DurationSeconds, newSetup.WarmupId, newSetup.ChangeoverId);

			//insert to reserve (but maintain its order)
			var insertionPoint = reserve.FirstOrDefault(x => x.StartDT > newSetup.StartDT);
			if (insertionPoint == null) reserve.Add(range);
			else
			{
				int index = reserve.IndexOf(insertionPoint);
				reserve.Insert(index, range);
			}
		} 
		#endregion

		/// <summary>
		/// checks if the total duration of the sequence is less than free space or not
		/// </summary>
		/// <param name="sequence"></param>
		/// <param name="freeSpaceSeconds"></param>
		/// <returns></returns>
		private bool validateSequence(IEnumerable<SmartRange> sequence, int freeSpaceSeconds)
		{
			return sequence.Where(x => x.Type != SmartRange.RangeType.DeleteSetup).Sum(x => x.DurationSeconds) <=
				freeSpaceSeconds + sequence.Where(x => x.Type == SmartRange.RangeType.DeleteSetup).Sum(x => x.DurationSeconds);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="stationId"></param>
		/// <param name="productReworkId"></param>
		/// <param name="startFrom"></param>
		/// <param name="durationOftask"></param>
		/// <param name="snapToLast">indicates if setups after an auto task should snap to its next task(true) or to the auto task(false)</param>
		/// <returns></returns>
		internal List<SmartRange> FindNextFreeSpace(int stationId, int productReworkId, DateTime startFrom, int durationOftask, Model.Block excludeModel = null)
		{
			//Find all tasks in database, which end after startFrom
			var inRangeItems = _blockDataService.GetInRange(startFrom, stationId, excludeModel)
				.Select(x => SmartRange.ExistingBlock(x)).ToList();
			var inRangeNPTs = _nptDataService.GetInRange(startFrom, stationId)
				.Select(x => SmartRange.ExistingSetup(x))
				.Where(x => x != null).ToList();
			//find the task and setup in database, which is just before startFrom
			var previousItem = _blockDataService.FindPreviousBlock(stationId, startFrom);
			var previousTask = SmartRange.ExistingBlock(previousItem.Item1);
			var previousSetup = SmartRange.ExistingSetup(previousItem.Item2);

			//create a row for specified station if needed
			if (!_reserve.Any(x => x.Key == stationId))
				_reserve.Add(new KeyValuePair<int, List<SmartRange>>(stationId, new List<SmartRange>()));
			var reserve = _reserve.First(x => x.Key == stationId).Value.Where(y => y.EndDT >= startFrom).ToList();

			PolynomialUnion(inRangeItems, inRangeNPTs);
			PolynomialUnion(inRangeItems, reserve);
			//*all are saved in inRangeItems*

			TimeSpan minFreeSpace = TimeSpan.FromSeconds(durationOftask);


			#region findSequenceOfSpotsBetween
			// Find the sequence of tasks and setups needed to be added or removed after the specified time
			// according to items starting before start and items starting before end
			// it's supposed that in the range (start..end) there is no other items
			var findSequenceOfSpotsBetween = new Func<DateTime, DateTime?, List<SmartRange>>((start,end) =>
			{
				var previousTaskInRange = inRangeItems.Where(x => x.Type == SmartRange.RangeType.Task || x.Type == SmartRange.RangeType.NewTask)
					.LastOrDefault(x => x.StartDT <= start);
				var previousSetupInRange = inRangeItems.Where(x => x.Type == SmartRange.RangeType.Setup || x.Type == SmartRange.RangeType.NewSetup)
					.LastOrDefault(x => x.StartDT <= start);
				if (previousTaskInRange != null)
					previousTask = previousTaskInRange;
				if (previousSetupInRange != null)
					previousSetup = previousSetupInRange;

				//nothing before this
				if (previousTask == null)
				{
					return new List<SmartRange> { 
						SmartRange.NewTask(start, durationOftask, stationId, productReworkId) 
					};
				}
				//product before this
				else
				{
					//no setup yet
					if (previousSetup == null)
					{
						//if the task ends after start
						if (previousTask.EndDT > start)
							start = previousTask.EndDT;

						//same products
						if (previousTask.ProductReworkId == productReworkId)
							return new List<SmartRange> { 
								SmartRange.NewTask(start, durationOftask, stationId, productReworkId) 
							};
						//different products
						else
						{
							var warmup = new DataServices.WarmupDataService().SmartFind(productReworkId, stationId);
							var changeover = new DataServices.ChangeoverDataService().SmartFind(previousTask.ProductReworkId, productReworkId, stationId);
							var smartSetup = SmartRange.NewSetup(start, warmup, changeover, stationId);
							return new List<SmartRange> { 
								smartSetup,
								SmartRange.NewTask(start.AddSeconds(smartSetup.DurationSeconds), durationOftask, stationId, productReworkId) 
							};
						}
					}
					//a setup after previous task and before this
					else
					{
						//same products
						if (previousTask.ProductReworkId == productReworkId)
						{
							//if the task ends after start (because setup is gonna be deleted)
							if (previousTask.EndDT > start)
								start = previousTask.EndDT;

							return new List<SmartRange> { 
								SmartRange.NewDeleteSetup(previousSetup),
								SmartRange.NewTask(start, durationOftask, stationId, productReworkId) 
							};
						}
						//if setup is ok due to its from_productRework and to_productRework
						else if (previousSetup.FromProductReworkId == previousTask.ProductReworkId
							&& previousSetup.ProductReworkId == productReworkId)
						{
							//if the setup ends after start
							if (previousSetup.EndDT > start)
								start = previousSetup.EndDT;

							return new List<SmartRange> { 
								SmartRange.NewTask(start, durationOftask, stationId, productReworkId) 
							};
						}
						//if setup is wrong due to its from_pdorcut_rorewk or to_productRework
						else
						{
							//if the task ends after start (because setup is gonna be deleted)
							if (previousTask.EndDT > start)
								start = previousTask.EndDT;

							var warmup = new DataServices.WarmupDataService().SmartFind(productReworkId, stationId);
							var changeover = new DataServices.ChangeoverDataService().SmartFind(
								previousTask.ProductReworkId,
								productReworkId, stationId);
							var smartSetup = SmartRange.NewSetup(start, warmup, changeover, stationId);
							return new List<SmartRange> { 
								SmartRange.NewDeleteSetup(previousSetup),
								smartSetup,
								SmartRange.NewTask(start.AddSeconds(smartSetup.DurationSeconds), durationOftask, stationId, productReworkId) 
							};
						}
					}
				}
			});
			#endregion


			#region what's this?
			// read the placement expressions left to right
			// second line [ ] is the range on which findSequenceOfSpotsBetween is called
			// first line is before adding, next lines are afterwards
			// 1111,2222,... are tasks
			// | is startFrom point
			// --- is empty space
			// TTTT is new task
			// x is setuptime (if needed)
			#endregion

			//--|---------
			//   [
			//--|xTTTT----
			//if no task: use startFrom as start of sequence finding
			if (inRangeItems.Count() == 0)
				return findSequenceOfSpotsBetween(startFrom, null);

			//if 1 task:
			else if (inRangeItems.Count() == 1)
			{
				var only = inRangeItems.First();

				//if the only task starts before startFrom but ends after it: use the end of it as start of sequence finding
				// 11|11---------
				//      [
				// 11|11xTTTT----
				if (only.StartDT <= startFrom)
					return findSequenceOfSpotsBetween(only.EndDT, null);
				else
				{
					//if the only task, starts after startFrom and enough space between startFrom and only task: 
					//					use startFrom as start of sequence finding and also add a reverse setup (from T to 1)
					// --|--------1111
					//    [      ]
					// --|xTTTT--x1111 snapToLast
					// --|xTTTTx--1111 snapToFirst
					//check if after-task setup is needed
					var warmup = only.ProductReworkId == productReworkId ?
						null : new DataServices.WarmupDataService().SmartFind(only.ProductReworkId, stationId);
					var changeover = only.ProductReworkId == productReworkId ?
						null : new DataServices.ChangeoverDataService().SmartFind(productReworkId, only.ProductReworkId, stationId);
					int reverseSetupDelay = only.ProductReworkId == productReworkId ?
						0 : warmup.Seconds + changeover.Seconds;
					
					var seq = findSequenceOfSpotsBetween(startFrom, only.StartDT);
					if(only.ProductReworkId != productReworkId)
						seq.Add(SmartRange.NewSetup(only.StartDT.AddSeconds(-reverseSetupDelay), warmup, changeover, stationId));//???

					if(validateSequence(seq, (int)only.StartDT.Subtract(startFrom).TotalSeconds)) 
						return seq;
					//if not enough space between startFrom and the only task: use the end of it as start of sequence finding
					// --|--1111-------
					// --|--1111xTTTT--
					else
						return findSequenceOfSpotsBetween(only.EndDT, null);
				}
			}
			//if more than 1 task:
			else
			{
				var first = inRangeItems.First();

				//if the first task, starts after startFrom and enough space between startFrom and first task:
				//					use startFrom as start of sequence finding and also add a reverse setup (from T to 1)
				// --|--------1111
				// --|xTTTT--x1111 snapToLast
				// --|xTTTTx--1111 snapToFirst
				if (first.StartDT > startFrom)
				{
					//check if after-task setup is needed
					var warmup = first.ProductReworkId == productReworkId ?
						null : new DataServices.WarmupDataService().SmartFind(first.ProductReworkId, stationId);
					var changeover = first.ProductReworkId == productReworkId ?
						null : new DataServices.ChangeoverDataService().SmartFind(productReworkId, first.ProductReworkId, stationId);
					int reverseSetupDelay = first.ProductReworkId == productReworkId ?
						0 : warmup.Seconds + changeover.Seconds;

					var seq = findSequenceOfSpotsBetween(startFrom, first.StartDT);
					if (first.ProductReworkId != productReworkId)
						seq.Add(SmartRange.NewSetup(first.StartDT.AddSeconds(-reverseSetupDelay), warmup, changeover, stationId));//???
					
					if (validateSequence(seq, (int)first.StartDT.Subtract(startFrom).TotalSeconds))
						return seq;
				}
				// after task prev and before inRangeTask
				// --|--prev--------inRangeTask
				// --|--prevxTTTT--xinRangeTask snapToLast
				// --|--prevxTTTTx--inRangeTask snapToFirst
				//for any of above ifs: 'else'
				{
					SmartRange prev = null;
					foreach (var inRangeTask in inRangeItems)
					{
						if (prev != null)
						{
							//if enough space between i & i+1: use end of i as start of sequence finding
							if (inRangeTask.StartDT.Subtract(prev.EndDT) >= minFreeSpace)
							{
								//check if after-task setup is needed
								var warmup = inRangeTask.ProductReworkId == productReworkId ?
									null : new DataServices.WarmupDataService().SmartFind(inRangeTask.ProductReworkId, stationId);
								var changeover = inRangeTask.ProductReworkId == productReworkId ?
									null : new DataServices.ChangeoverDataService().SmartFind(productReworkId, inRangeTask.ProductReworkId, stationId);
								int reverseSetupDelay = inRangeTask.ProductReworkId == productReworkId ?
									0 : warmup.Seconds + changeover.Seconds;

								var seq = findSequenceOfSpotsBetween(prev.EndDT, inRangeTask.StartDT);
								if(inRangeTask.ProductReworkId != productReworkId)
								seq.Add(SmartRange.NewSetup(inRangeTask.StartDT.AddSeconds(-reverseSetupDelay), warmup, changeover, stationId));//???

								if (validateSequence(seq, (int)inRangeTask.StartDT.Subtract(prev.EndDT).TotalSeconds))
									return seq;
							}
						}
						prev = inRangeTask;
					}
					//if not enough space between tasks: use end of last task as start of sequence finding
					// --|--last-------
					// --|--lastxTTTT--
					return findSequenceOfSpotsBetween(inRangeItems.Last().EndDT, null);
				}
			}
		}

		/// <summary>
		/// <para>Inserts reserve list into data and maintains its order.</para>
		/// <para>Both data and reserve MUST be sorted</para>
		/// </summary>
		/// <param name="data">tasks fetched from database</param>
		/// <param name="reserve">reserved tasks</param>
		/// <returns></returns>
		internal void PolynomialUnion(List<SmartRange> data, List<SmartRange> reserve)
		{
			List<SmartRange> dest = new List<SmartRange>();
			int p = 0;
			int q = 0;
			for (int i = 0; i < data.Count + reserve.Count; i++)
			{
				if (data.Count == p)
					dest.Add(reserve[q++]);
				else if (reserve.Count == q)
					dest.Add(data[p++]);
				else if (data[p].StartDT <= reserve[q].StartDT)
					dest.Add(data[p++]);
				else
					dest.Add(reserve[q++]);
			}
			data.Clear();
			data.AddRange(dest);
		}


		/// <summary>
		/// Saves all new setup entries in given list and returns true if all added successfully
		/// </summary>
		/// <param name="data">list of smart ranges creates by a smart manager</param>
		/// <returns>true if all added successfully</returns>
		internal bool SaveSetups(IEnumerable<SmartRange> data)
		{
			bool allOk = true;
			foreach (var setup in data.Where(x=>x.Type == SmartRange.RangeType.NewSetup))
			{
				if (_nptDataService.AddModel(setup) <= 0) allOk = false;
			}
			return allOk;
		}
	}
}
