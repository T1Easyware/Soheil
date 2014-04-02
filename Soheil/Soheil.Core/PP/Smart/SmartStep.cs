using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Soheil.Model;

namespace Soheil.Core.PP.Smart
{
	internal class SmartStep
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="job"></param>
		/// <param name="currentState">Required some of its Relations loaded</param>
		/// <param name="parentStep"></param>
		internal SmartStep(SmartJob job, State currentState, SmartStep parentStep)
		{
			_job = job;
			State = currentState;
			ParentSteps = new List<SmartStep>();
			if (parentStep != null)
				if (parentStep.State.StateType == Common.StateType.Mid)
					ParentSteps.Add(parentStep);
		}

		internal void MakeTheBestFit()
		{
			ActualReleaseTime = DateTime.Now;//best release time for current step
			BestStateStation = null;//best stateStation for current step
	
			foreach (var ss in State.StateStations)
			{
				//Find earliest possible start time
				var earliestPossibleReleaseTime = _job.ReleaseTime;
				if (earliestPossibleReleaseTime < DateTime.Now) earliestPossibleReleaseTime = DateTime.Now;
				DurationSeconds = Math.Ceiling(_job.Quantity * ss.MaxCycleTime);
				foreach (var parent in ParentSteps)
				{
					var laggedRT = parent.ActualReleaseTime;
					if (_job.LagSeconds != 0)
						laggedRT = laggedRT.AddSeconds(_job.LagSeconds);
					else if (_job.LagCount != 0)
						laggedRT = laggedRT.AddSeconds((int)Math.Ceiling(_job.LagCount * ss.MaxCycleTime));
					if (earliestPossibleReleaseTime < laggedRT) earliestPossibleReleaseTime = laggedRT;
				}

				//find free space
				var seq = _job.Manager.FindNextFreeSpace(
					ss.Station.Id, ss.State.OnProductRework.Id, earliestPossibleReleaseTime, (int)DurationSeconds);

				//find the newly added task
				var taskseq = seq.FirstOrDefault(x => x.Type == SmartRange.RangeType.NewTask);

				//Check for deadline
				if (taskseq.StartDT.AddSeconds(DurationSeconds) > _job.Deadline) continue;

				//Set the best fit
				if (BestStateStation == null)
				{
					ActualReleaseTime = taskseq.StartDT;
					BestStateStation = ss;
					ChosenSequence = seq;
				}
				else if (ActualReleaseTime > taskseq.StartDT)
				{
					ActualReleaseTime = taskseq.StartDT;
					BestStateStation = ss;
					ChosenSequence = seq;
				}
			}

			if (BestStateStation == null)
				throw new Exception(string.Format(
					"Cannot fit a Job. Not enough free space on timeline.\nJob Code = {0}\nQuantity = {1}", _job.Code, _job.Quantity));

			//reserve the space
			_job.Manager.Reserve(ActualReleaseTime, BestStateStation, _job);
			foreach (var newSetup in ChosenSequence.Where(x => x.Type == SmartRange.RangeType.NewSetup))
				_job.Manager.Reserve(newSetup, _job);
		}

		SmartJob _job;
		internal DateTime ActualReleaseTime { get; private set; }//Starting Time
		internal double DurationSeconds { get; set; }
		internal State State;//{ get; private set; }
		internal StateStation BestStateStation;//{ get; private set; }
		internal List<SmartRange> ChosenSequence;//{ get; private set; }
		internal bool HasStateStation { get { return State.StateStations.Any(); } }
		internal List<SmartStep> ParentSteps;
	}
}
