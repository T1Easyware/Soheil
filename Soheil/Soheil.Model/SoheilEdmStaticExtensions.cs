using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Soheil.Common;

namespace Soheil.Model
{
	public partial class Rework
	{
	}

	public partial class WorkProfile
	{
		public static WorkProfile CreateDefault()
		{
			var workProfile = new WorkProfile
			{
				Name = "[New]",
				WeekStartNr = 0,
				SaturdayBusinessStateNr = 0,
				SundayBusinessStateNr = 0,
				MondayBusinessStateNr = 0,
				TuesdayBusinessStateNr = 0,
				WednesdayBusinessStateNr = 0,
				ThursdayBusinessStateNr = 1,
				FridayBusinessStateNr = 2,
			};
			var proto = WorkShiftPrototype.CreateDefault(workProfile);
			workProfile.WorkShiftPrototypes.Add(proto);
			workProfile.WorkDays.Add(WorkDay.CreateDefault(workProfile, proto, BusinessDayType.Open));
			workProfile.WorkDays.Add(WorkDay.CreateDefault(workProfile, proto, BusinessDayType.HalfClosed));
			workProfile.WorkDays.Add(WorkDay.CreateDefault(workProfile, proto, BusinessDayType.Closed));
			workProfile.WorkDays.Add(WorkDay.CreateDefault(workProfile, proto, BusinessDayType.SpecialDay1));
			workProfile.WorkDays.Add(WorkDay.CreateDefault(workProfile, proto, BusinessDayType.SpecialDay2));
			workProfile.WorkDays.Add(WorkDay.CreateDefault(workProfile, proto, BusinessDayType.SpecialDay3));
			return workProfile;
		}
	}
	public partial class WorkDay
	{
		/// <summary>
		/// Creates a default WorkDay with current WorkProfile and its default WorkShiftPrototype
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="proto"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public static WorkDay CreateDefault(WorkProfile parent, WorkShiftPrototype proto, BusinessDayType state)
		{
			var model = new WorkDay();
			model.WorkShifts.Add(WorkShift.CreateDefault(model, proto));
			model.Name = GetNameByNr((int)state);
			model.Color = GetColorByNr((int)state);
			model.BusinessState = state;
			model.WorkProfile = parent;
			return model;
		}

		public static Color GetColorByNr(int stateNr)
		{
			return new Color[] { 
				DefaultColors.Day.Closed,
				DefaultColors.Day.SpecialDay1, DefaultColors.Day.SpecialDay2, DefaultColors.Day.SpecialDay3 ,
				DefaultColors.Day.HalfClosed, 
				DefaultColors.Day.Open
			}[stateNr % 6];
		}
		public static string GetNameByNr(int stateNr) { return new string[] { 
			"تعطیل", 
			"رزرو1", "رزرو2", "رزرو3" ,
			"نیمه تعطیل", 
			"باز"
		}[stateNr % 6].ToString(); }
	}
	public partial class WorkShiftPrototype
	{
		public static WorkShiftPrototype CreateDefault(WorkProfile parent)
		{
			var model = new WorkShiftPrototype();
			if (parent.WorkShiftPrototypes.Any())
			{
				var last = parent.WorkShiftPrototypes.Last();
				model.Index = (byte)(last.Index + 1);
			}
			else model.Index = 0;
			model.Name = GetNameByIndex(model.Index);
			model.Color = GetColorByIndex(model.Index);
			model.WorkProfile = parent;
			return model;
		}
		public static Color GetColorByIndex(int idx)
		{
			return new Color[] { DefaultColors.Shift.Day, DefaultColors.Shift.Evening, DefaultColors.Shift.Night,
			DefaultColors.Shift.Reserve1, DefaultColors.Shift.Reserve2, DefaultColors.Shift.Reserve3 }[idx % 6];
		}
		public static string GetNameByIndex(int idx) { return "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[idx % 26].ToString(); }
	}
	public partial class WorkShift
	{
		public static WorkShift CreateDefault(WorkDay parent, WorkShiftPrototype proto)
		{
			var model = new WorkShift();
			if (parent.WorkShifts.Any())
			{
				var last = parent.WorkShifts.Last();
				model.StartSeconds = last.EndSeconds;
				model.EndSeconds = last.EndSeconds + 3600;
				if (model.EndSeconds > SoheilConstants.EDITOR_END_SECONDS) throw new Exception("قادر به افزودن شیفت نمی باشد. زمان کافی در روز وجود ندارد");
			}
			else
			{
				model.StartSeconds = SoheilConstants.EDITOR_START_SECONDS;
				model.EndSeconds = SoheilConstants.EDITOR_START_SECONDS + 3600;
			}
			model.WorkDay = parent;
			model._workShiftPrototype = proto;
			return model;
		}
	}
	public partial class Holiday
	{
		public static Holiday CreateDefault()
		{
			return new Holiday
			{
				Name = "تعطیلی رسمی",
				Date = DateTime.Now.Date,
				BusinessState = BusinessDayType.Closed,
				IsRecurrent = true,
			};
		}
	}
	public partial class WorkProfilePlan
	{
		//public static WorkProfilePlan CreateDefault(WorkProfilePlan last)
		//{
		//	return new WorkProfilePlan
		//	{
		//		Name = "*",
		//		StartDate = last == null ? DateTime.Now.Date : last.EndDate,
		//		EndDate = last == null ? DateTime.Now.Date.AddDays(1) : last.EndDate.AddDays(1),
		//		MergingStrategy = WorkProfileMergingStrategy.Closed,
		//		WorkProfile = null,
		//	};
		//}
	}

}
