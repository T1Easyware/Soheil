using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.PP.Smart
{
	public class SmartRange
	{
		public enum RangeType { Empty, NewTask, Task, NewSetup, DeleteSetup, Setup, Forbidden }
		public RangeType Type { get; protected set; }

		public DateTime StartDT { get; protected set; }

		public DateTime EndDT { get { return StartDT.AddSeconds(DurationSeconds); } }

		public int DurationSeconds { get; protected set; }
		public int StationId { get; protected set; }
		/// <summary>
		/// <para>On_ProductRework in Task</para>
		/// <para>To_ProductRework in Changeover</para>
		/// <para>ProductRework in Warmup</para>
		/// </summary>
		public int ProductReworkId { get; protected set; }
		public int FromProductReworkId { get; protected set; }

		public int SetupId { get; protected set; }
		public int WarmupId { get; protected set; }
		public int ChangeoverId { get; protected set; }


		public static SmartRange NewEmptySpot(DateTime start)
		{
			return new SmartRange
			{
				StartDT = start,
				DurationSeconds = 0,
				Type = RangeType.Empty,
			};
		}
		public static SmartRange ExistingBlock(Model.Block model)
		{
			if (model == null) return null;
			return new SmartRange
			{
				StartDT = model.StartDateTime,
				DurationSeconds = model.DurationSeconds,
				StationId = model.StateStation.Station.Id,
				ProductReworkId = model.StateStation.State.OnProductRework.Id,
				Type = RangeType.Task,
			};
		}
		public static SmartRange ExistingSetup(Model.NonProductiveTask model)
		{
			var setup = model as Model.Setup;
			if (setup == null) return null;
			if (setup.Warmup == null || setup.Changeover == null) return null;
			var sr = new SmartRange
			{
				StartDT = model.StartDateTime,
				DurationSeconds = model.DurationSeconds,
				Type = RangeType.Setup,
				SetupId = model.Id,
				StationId = setup.Warmup.Station.Id,
				ProductReworkId = setup.Warmup.ProductRework.Id,
				FromProductReworkId = setup.Changeover.FromProductRework.Id,
				WarmupId = setup.Warmup.Id,
				ChangeoverId = setup.Changeover.Id,
			};
			return sr;
		}
		public static SmartRange NewSetup(DateTime start, Model.Warmup warmup, Model.Changeover changeover, int stationId)
		{
			var totalSeconds = warmup.Seconds + changeover.Seconds;
			if (warmup == null || changeover == null) return null;
			return new SmartRange
			{
				StartDT = start,
				DurationSeconds = totalSeconds,
				StationId = stationId,
				WarmupId = warmup.Id,
				ChangeoverId = changeover.Id,
				Type = RangeType.NewSetup,
			};
		}

		public static SmartRange NewDeleteSetup(SmartRange sr)
		{
			return new SmartRange
			{
				Type = RangeType.DeleteSetup,
				SetupId = sr.SetupId,
				StartDT = sr.StartDT,
				StationId = sr.StationId,
				DurationSeconds = sr.DurationSeconds,
			};
		}
		public static SmartRange NewTask(DateTime start, int durationSeconds, int stationId, int productReworkId)
		{
			return new SmartRange
			{
				StartDT = start,
				StationId = stationId,
				ProductReworkId = productReworkId,
				DurationSeconds = durationSeconds,
				Type = RangeType.NewTask,
			};
		}
		protected SmartRange()
		{

		}
		internal static SmartRange NewForbidden(DateTime startTime, int durationSeconds)
		{
			return new SmartRange
			{
				Type = RangeType.Forbidden,
				StartDT = startTime,
				DurationSeconds = durationSeconds,
			};
		}
		internal static SmartRange NewReserve(DateTime startTime, int durationSeconds, Model.StateStation stateStation)
		{
			return new SmartRange
			{
				Type = RangeType.Task,
				StartDT = startTime,
				DurationSeconds = durationSeconds,
				StationId = stateStation.Station.Id,
				ProductReworkId = stateStation.State.OnProductRework.Id,
			};
		}
		internal static SmartRange NewReserve(DateTime startTime, int durationSeconds, int warmupId, int changeoverId)
		{
			return new SmartRange
			{
				StartDT = startTime,
				DurationSeconds = durationSeconds,
				WarmupId = warmupId,
				ChangeoverId = changeoverId,
				Type = RangeType.Setup,
			};
		}
	}
}
