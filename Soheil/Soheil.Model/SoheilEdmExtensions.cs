using System;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Linq;
using Soheil.Common;
using System.Collections.Generic;
using System.Data.Objects;
using System.Windows.Media;

namespace Soheil.Model
{

	#region Extensions

	public partial class AccessRule
	{
		//public Status RecordStatus
		//{
		//    get { return (Status)Common.Status; }
		//    set { Common.Status = (byte)value; }
		//}
	}

	public partial class ActionPlan
	{
		//public Status RecordStatus
		//{
		//    get { return (Status)Common.Status; }
		//    set { Common.Status = (byte)value; }
		//}
	}

	public partial class ActionPlan_Root
	{
	}

	public partial class Activity
	{
		//public Status RecordStatus
		//{
		//    get { return (Status)Common.Status; }
		//    set { Common.Status = (byte)value; }
		//}
	}

	public partial class ActivityGroup
	{
	}

	public partial class Cause
	{
		//public Status RecordStatus
		//{
		//    get { return (Status)Common.Status; }
		//    set { Common.Status = (byte)value; }
		//}
	}

	public partial class CauseL1
	{
	}

	public partial class CauseL2
	{
	}

	public partial class CauseL3
	{
	}

	public partial class Connector
	{
	}

	public partial class CostCenter
	{
		//public Status RecordStatus
		//{
		//    get { return (Status)Common.Status; }
		//    set { Common.Status = (byte)value; }
		//}
	}

	public partial class Defection
	{
		//public Status RecordStatus
		//{
		//    get { return (Status)Common.Status; }
		//    set { Common.Status = (byte)value; }
		//}
	}

	public partial class Defection_Root
	{
	}

	public partial class DefectionReport
	{
		public float CountEquivalence { get { return LostCount + LostTime * ProcessReport.Process.StateStationActivity.CycleTime; } }
	}

	public partial class FPC
	{
		//public Status RecordStatus
		//{
		//    get { return (Status)Common.Status; }
		//    set { Common.Status = (byte)value; }
		//}

		/// <summary>
		/// Gets the state which starts all the fpc
		/// </summary>
		public State StartingState
		{
			get
			{
				return States.FirstOrDefault(x => x.StateType == Common.StateType.Start);
			}
		}
		public State GetStartingReworkState(int onProductReworkId)
		{
			/*var pr = Product.ProductRework.FirstOrDefault(x => x.Rework == null);
			if (pr.Id == onProductReworkId)
				return StartingState; too slow?*/
			return States.FirstOrDefault(x => 
				x.StateTypeNr == (int)StateType.Mid
				&& x.OnProductRework != null 
				&& x.OnProductRework.Id == onProductReworkId);
		}

		private void addStateToList(State state, List<State> destination)
		{
			foreach (var conn in state.OutConnectors.Where(x => 
				x.EndState.StateType == StateType.Mid))
			{
				if (destination.Any(x => x.Id == conn.EndState.Id)) continue;
				destination.Add(conn.EndState);
				addStateToList(conn.EndState, destination);
			}
		}
		private void addStateToList(State state, int stationId, List<State> destination)
		{
			foreach (var conn in state.OutConnectors.Where(x =>
				x.EndState.StateType == StateType.Mid &&
				x.EndState.StateStations.Any(y => y.Station.Id == stationId)))
			{
				if (destination.Any(x => x.Id == conn.EndState.Id)) continue;
				destination.Add(conn.EndState);
				addStateToList(conn.EndState, stationId, destination);
			}
		}
	}

	public partial class PersonalSkill
	{
	}

	public partial class Machine
	{
		//public Status RecordStatus
		//{
		//    get { return (Status)Common.Status; }
		//    set { Common.Status = (byte)value; }
		//}
	}

	public partial class MachineFamily
	{
	}

	public partial class Operator
	{
		//public Status RecordStatus
		//{
		//    get { return (Status)Common.Status; }
		//    set { Common.Status = (byte)value; }
		//}
	}

	public partial class GeneralActivitySkill
	{
		public ILUO Iluo
		{
			get { return (ILUO)IluoNr; }
			set { IluoNr = (byte)value; }
		}
	}
	public partial class UniqueActivitySkill
	{
		public ILUO Iluo
		{
			get { return (ILUO)IluoNr; }
			set { IluoNr = (byte)value; }
		}
	}

	public partial class Operator_DefectionReport
	{
	}

	public partial class Operator_StoppageReport
	{
	}

	/*public partial class PPSquare
	{
		public PPFlags PPFlags
		{
			get { return (PPFlags) PPFlagsNr; }
			set { PPFlagsNr = (byte) value; }
		}
	}

	public partial class PPSubsquare
	{
		public PPFlags PPFlags
		{
			get { return (PPFlags) PPFlagsNr; }
			set { PPFlagsNr = (byte) value; }
		}
	}

	public partial class PPSubsquare_Activity
	{
	}*/

	public partial class ProcessOperator
	{
		public OperatorRole Role
		{
			get { return (OperatorRole)RoleNr; }
			set { RoleNr = (byte)value; }
		}
	}

	public partial class PPSubsquareActivityReport
	{
	}

	public partial class TaskReport
	{
		public ConfirmationStatus ConfirmationStatus
		{
			get { return (ConfirmationStatus)Status; }
			set { Status = (byte)value; }
		}
	}

	public partial class Product
	{
		public Status RecordStatus
		{
			get { return (Status)Status; }
			set { Status = (byte)value; }
		}
		public System.Windows.Media.Color Color
		{
			get
			{
				return System.Windows.Media.Color.FromArgb(
					(byte)(ColorNumber >> 24 == 0 ? 255 : ColorNumber >> 24),
					(byte)((ColorNumber >> 16) & 0xFF),
					(byte)((ColorNumber >> 8) & 0xFF),
					(byte)(ColorNumber & 0xFF));
			}
			set { ColorNumber = (((((value.A << 8) + value.R) << 8) + value.G) << 8) + value.B; }
		}

		/// <summary>
		/// Gets the only ProductRework of this Product with Rework = null
		/// </summary>
		public ProductRework MainProductRework { get { return ProductReworks.First(x => x.Rework == null); } }

		public FPC DefaultFpc { get { return FPCs.First(x => x.IsDefault); } }
	}

	public partial class ProductDefection
	{
	}

	public partial class ProductRework
	{
		//public Status RecordStatus
		//{
		//    get { return (Status)Common.Status; }
		//    set { Common.Status = (byte)value; }
		//}
	}

	public partial class Rework
	{
		//public Status RecordStatus
		//{
		//    get { return (Status)Common.Status; }
		//    set { Common.Status = (byte)value; }
		//}
	}

	public partial class Root
	{
		//public Status RecordStatus
		//{
		//    get { return (Status)Common.Status; }
		//    set { Common.Status = (byte)value; }
		//}
	}

	public partial class GeneralActivitySkill
	{
	}

	public partial class State
	{
		public StateType StateType
		{
			get { return (StateType)StateTypeNr; }
			set { StateTypeNr = (byte)value; }
		}
		public Bool3 IsReworkState
		{
			get
			{
				return StateTypeNr != (int)StateType.Mid 
					? Bool3.Undefined :
					(OnProductRework == null || OnProductRework.Rework == null) 
					? Bool3.False : Bool3.True;
			}
		}
	}


	public partial class StateStation
	{
		public float MaxCycleTime
		{
			get { return StateStationActivities.Max(x => x.CycleTime); }
		}
	}

	public partial class Station
	{
		//public Status RecordStatus
		//{
		//    get { return (Status)Common.Status; }
		//    set { Common.Status = (byte)value; }
		//}
	}

	public partial class StationMachine
	{
	}

	public partial class StoppageReport
	{
		public float CountEquivalence { get { return LostCount + LostTime * ProcessReport.Process.StateStationActivity.CycleTime; } }
	}

	public partial class Task
	{
		public DateTimeOffset Duration
		{
			get { return new DateTimeOffset(DurationSeconds * 1000, new TimeSpan(0)); }
			set { DurationSeconds = (int)(value.Ticks / 1000); }
		}
		public bool IsRework
		{
			get
			{
				return Block.StateStation.State.OnProductRework.Rework != null;
			}
		}
	}
	public partial class TaskReport
	{

	}

	public partial class User
	{
		//public Status RecordStatus
		//{
		//    get { return (Status)Common.Status; }
		//    set { Common.Status = (byte)value; }
		//}
	}

	public partial class User_AccessRule
	{
	}

	public partial class User_UserGroup
	{
	}

	public partial class UserGroup
	{
		//public Status RecordStatus
		//{
		//    get { return (Status)Common.Status; }
		//    set { Common.Status = (byte)value; }
		//}
	}

	public partial class UserGroup_AccessRule
	{
	}

	public partial class WorkProfile
	{
		/// <summary>
		/// sets business state of a day by its index
		/// </summary>
		/// <param name="dayIndex">Saturday = 0</param>
		public void SetBusinessState(int dayIndex, BusinessDayType state)
		{
			switch (dayIndex)
			{
				case 0: SaturdayBusinessStateNr = (byte)state; break;
				case 1: SundayBusinessStateNr = (byte)state; break;
				case 2: MondayBusinessStateNr = (byte)state; break;
				case 3: TuesdayBusinessStateNr = (byte)state; break;
				case 4: WednesdayBusinessStateNr = (byte)state; break;
				case 5: ThursdayBusinessStateNr = (byte)state; break;
				case 6: FridayBusinessStateNr = (byte)state; break;
				default:
					break;
			}
		}
		/// <summary>
		/// gets business state of a day by its index
		/// </summary>
		/// <param name="dayIndex">Saturday = 0</param>
		public BusinessDayType GetBusinessState(int dayIndex)
		{
			switch (dayIndex)
			{
				case 0: return (BusinessDayType)SaturdayBusinessStateNr;
				case 1: return (BusinessDayType)SundayBusinessStateNr;
				case 2: return (BusinessDayType)MondayBusinessStateNr;
				case 3: return (BusinessDayType)TuesdayBusinessStateNr;
				case 4: return (BusinessDayType)WednesdayBusinessStateNr;
				case 5: return (BusinessDayType)ThursdayBusinessStateNr;
				case 6: return (BusinessDayType)FridayBusinessStateNr;
				default: return BusinessDayType.Closed;
			}
		}
	}
	public partial class WorkDay
	{
		public BusinessDayType BusinessState
		{
			get { return (BusinessDayType)BusinessStateNr; }
			set { BusinessStateNr = (byte)value; }
		}
		public Color Color
		{
			get
			{
				return System.Windows.Media.Color.FromArgb(
					(byte)(ColorNr >> 24 == 0 ? 255 : ColorNr >> 24),
					(byte)((ColorNr >> 16) & 0xFF),
					(byte)((ColorNr >> 8) & 0xFF),
					(byte)(ColorNr & 0xFF));
			}
			set { ColorNr = (((((value.A << 8) + value.R) << 8) + value.G) << 8) + value.B; }
		}
	}
	public partial class WorkShiftPrototype
	{
		public Color Color
		{
			get
			{
				return System.Windows.Media.Color.FromArgb(
					(byte)(ColorNr >> 24 == 0 ? 255 : ColorNr >> 24),
					(byte)((ColorNr >> 16) & 0xFF),
					(byte)((ColorNr >> 8) & 0xFF),
					(byte)(ColorNr & 0xFF));
			}
			set { ColorNr = (((((value.A << 8) + value.R) << 8) + value.G) << 8) + value.B; }
		}
	}
	public partial class Holiday
	{
		public BusinessDayType BusinessState
		{
			get { return (BusinessDayType)BusinessStateNr; }
			set { BusinessStateNr = (byte)value; }
		}
	}
	public partial class WorkProfilePlan
	{
		public WorkProfileMergingStrategy MergingStrategy
		{
			get { return (WorkProfileMergingStrategy)MergingStrategyNr; }
			set { MergingStrategyNr = (byte)value; }
		}
	}

	#endregion
}