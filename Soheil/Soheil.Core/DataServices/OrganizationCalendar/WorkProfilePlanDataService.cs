using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Model;
using Soheil.Dal;
using Soheil.Common;
using Soheil.Core.Base;
using System.Windows.Media;

namespace Soheil.Core.DataServices
{
	public class WorkProfilePlanDataService : DataServiceBase, IDataService<WorkProfilePlan>
	{
        public event EventHandler<ModelAddedEventArgs<WorkProfilePlan>> WorkProfilePlanAdded;
        Repository<WorkProfilePlan> workProfilePlanRepository;

		public WorkProfilePlanDataService() : this(new SoheilEdmContext())
		{
		}
		public WorkProfilePlanDataService(SoheilEdmContext context)
		{
			this.Context = context;
			workProfilePlanRepository = new Repository<WorkProfilePlan>(context);
		}

        public WorkProfilePlan GetSingle(int id)
		{
            return workProfilePlanRepository.Single(x => x.Id == id,
                "WorkProfile",
                "WorkProfile.WorkDays",
                "WorkProfile.WorkShiftPrototypes",
                "WorkProfile.WorkDays.WorkShifts",
                "WorkProfile.WorkDays.WorkShifts.WorkShiftPrototype",
                "WorkProfile.WorkDays.WorkShifts.WorkBreaks");
		}

        public System.Collections.ObjectModel.ObservableCollection<WorkProfilePlan> GetAll()
		{
            var list = new System.Collections.ObjectModel.ObservableCollection<WorkProfilePlan>();
            var items = workProfilePlanRepository.GetAll(
                "WorkProfile");
			foreach (var item in items)
			{
				list.Add(item);
			}
			return list;
		}

        public System.Collections.ObjectModel.ObservableCollection<WorkProfilePlan> GetActives()
		{
			throw new NotImplementedException();
		}

        public int AddModel(WorkProfilePlan model)
		{
            model.ModifiedBy = LoginInfo.Id;
            model.ModifiedDate = DateTime.Now;
            model.CreatedDate = DateTime.Now;
            workProfilePlanRepository.Add(model);
			Context.Commit();

            WorkProfilePlanAdded(this, new ModelAddedEventArgs<WorkProfilePlan>(model));
			return model.Id;
		}

        public void UpdateModel(WorkProfilePlan model)
		{
            model.ModifiedBy = LoginInfo.Id;
            model.ModifiedDate = DateTime.Now;
			Context.Commit();
		}

        public void DeleteModel(WorkProfilePlan model)
		{
            workProfilePlanRepository.Delete(model);
			Context.Commit();
		}

        public void AttachModel(WorkProfilePlan model)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// creates and saves a clone of the given model (id) and returns it
		/// </summary>
		/// <param name="id">Id of source model</param>
		/// <returns>clone</returns>
        internal WorkProfilePlan CloneModelById(int id)
		{
            var model = workProfilePlanRepository.Single(x => x.Id == id);
			var clone = cloneModel(model);
			Context.Commit();
			return clone;
		}
        public /*override*/ T Clone<T>(T model)
        {
			var typed_model = (WorkProfilePlan)Convert.ChangeType(model, typeof(WorkProfilePlan));
			var typed_clone = cloneModel(typed_model);
			var t_clone = (T)Convert.ChangeType(typed_clone, typeof(T));
            Context.Commit();
			return t_clone;
        }

        protected WorkProfilePlan cloneModel(WorkProfilePlan model)
		{
			var clone = new WorkProfilePlan();
			clone.WorkProfile = model.WorkProfile;
            clone.StartDate = model.StartDate;
            clone.EndDate = model.EndDate;
            clone.Name = model.Name;
            clone.MergingStrategyNr = model.MergingStrategyNr;
            clone.ModifiedBy = LoginInfo.Id;
			clone.ModifiedDate = DateTime.Now;
            model.CreatedDate = DateTime.Now;
            WorkProfilePlanAdded(this, new ModelAddedEventArgs<WorkProfilePlan>(clone));
			return clone;
		}

 
        /// <summary>
        /// <para>Sets the WorkProfile reference of a plan according to the given workProfileId</para>
        /// <para>Model must initiated using this instance of DataService</para>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="workProfileId"></param>
        internal void SetWorkProfile(WorkProfilePlan model, int workProfileId)
        {
            model.WorkProfile = new Repository<WorkProfile>(Context).Single(x => x.Id == workProfileId);
        }

		public void AddDefaultWorkProfilePlanIfNeeded()
		{
			if (GetCurrent() == null)
			{
				AddModel(CreateDefault());
			}
		}

		public WorkProfilePlan CreateDefault()
		{
			var last = GetLast();
			var wpds = new Repository<WorkProfile>(Context);
			var lastwp = wpds.LastOrDefault(x => true, x => x.ModifiedDate);
			if (lastwp == null)
			{
				lastwp = WorkProfile.CreateDefault();
				wpds.Add(lastwp);
			}

			return new WorkProfilePlan
			{
				Name = "*",
				StartDate = last == null ? DateTime.Now.Date : last.EndDate,
				EndDate = last == null ? DateTime.Now.Date.AddDays(1) : last.EndDate.AddDays(1),
				MergingStrategy = WorkProfileMergingStrategy.Closed,
				WorkProfile = lastwp,
			};
		}

		public override void Dispose()
		{
			Context.Dispose();
			base.Dispose();
		}

		#region Search
		/// <summary>
		/// Returns the latest plan (due to its end date)
		/// </summary>
		/// <returns></returns>
		internal WorkProfilePlan GetLast()
		{
			return workProfilePlanRepository.LastOrDefault(x => true, x => x.EndDate);
		}
		/// <summary>
		/// <para>Returns the current plan</para>
		/// <para>If more than one plan includes Now, return the latest modified plan</para>
		/// </summary>
		/// <returns></returns>
		internal WorkProfilePlan GetCurrent()
		{
			return workProfilePlanRepository.LastOrDefault(x =>
				x.StartDate >= DateTime.Now && x.EndDate <= DateTime.Now,
				x => x.ModifiedDate);
		}

		internal WorkProfilePlan GetCurrentAt(DateTime dateTime)
		{
			return workProfilePlanRepository.LastOrDefault(x =>
				x.StartDate >= dateTime && x.EndDate <= dateTime,
				x => x.ModifiedDate);
		}
		/// <summary>
		/// Gets the starting DateTime of the first Shift of the given Date
		/// </summary>
		/// <param name="dateTime">target Date</param>
		/// <returns>dateTime.Date.Add([FirstShift in the date].StartSeconds)</returns>
		internal DateTime GetShiftStartAt(DateTime dateTime)
		{
			dateTime = dateTime.Date;
			var wpp = workProfilePlanRepository.LastOrDefault(x =>
				x.StartDate >= dateTime && x.EndDate <= dateTime,
				x => x.ModifiedDate);
			var bizState = GetEffectiveBizState(dateTime, wpp);
			if (wpp == null)
				return dateTime;
			if (wpp.WorkProfile == null)
				return dateTime;
			if (!wpp.WorkProfile.WorkDays.Any())
				return dateTime;
			var day = wpp.WorkProfile.WorkDays.First(x => x.BusinessState == bizState);
			if (day == null)
				return dateTime;
			//add seconds (start of first shift) to dateTime (given date)
			return dateTime.AddSeconds(day.WorkShifts.Min(x=>x.StartSeconds));
		}

		internal List<WorkProfilePlan> GetInRange(DateTime startDate, DateTime endDate)
		{
			var plans = workProfilePlanRepository.Find(x => !(x.EndDate < startDate || x.StartDate > endDate),
				"WorkProfile.WorkDays.WorkShifts.WorkBreaks",
				"WorkProfile.WorkDays.WorkShifts.WorkShiftPrototype",
				"WorkProfile.WorkShiftPrototypes.WorkShifts.WorkBreaks"
				);
			return plans.ToList();
		}

		/// <summary>
		/// Extracts the specified date's BusinessDayType from a plan and all available Holidays
		/// <para>Priority of states are as follows: Closed, Special1, Special2, Special3, HalfClosed, Open</para>
		/// </summary>
		/// <param name="date">date of which BusinessDayType is returned</param>
		/// <param name="plan">instance of WorkProfilePlan to search in</param>
		/// <returns></returns>
		internal BusinessDayType GetEffectiveBizState(DateTime date, WorkProfilePlan plan)
		{
			var states = new HolidayDataService(Context).GetBusinessDayStatesAtDate(date);

			if (plan == null) return BusinessDayType.None;
			states.Add(plan.WorkProfile.GetBusinessState((int)date.GetPersianDayOfWeek()));
			return (BusinessDayType)states.Min();
		}


		/// <summary>
		/// Returns every day of the given range [start,end)
		/// <para>where value of each day is its effective BusinessDayStates considering Holidays</para>
		/// <para>Priority of states are as follows: Closed, Special1, Special2, Special3, HalfClosed, Open</para>
		/// </summary>
		/// <param name="startDate">Included start date</param>
		/// <param name="endDate">Excluded end date</param>
		/// <returns>an array of BusinessDayTypes, each element for one day</returns>
		internal List<BusinessDayType> GetBusinessDayStatesInRange(DateTime startDate, DateTime endDate)
		{
			var plans = GetInRange(startDate, endDate);
			List<BusinessDayType> list = new List<BusinessDayType>();
			for (DateTime date = startDate; date < endDate; date = date.AddDays(1))
			{
				var plan = plans.OrderBy(x => x.ModifiedDate).LastOrDefault(x => x.StartDate <= date && date < x.EndDate);
				list.Add(GetEffectiveBizState(date, plan));
			}
			return list;
		}        

		/// <summary>
        /// Returns every day of the given range [start,end)
		/// <para>where value of each day is its effective color considering Holidays</para>
		/// <para>Priority of states are as follows: Closed, Special1, Special2, Special3, HalfClosed, Open</para>
        /// </summary>
        /// <param name="startDate">Included start date</param>
        /// <param name="endDate">Excluded end date</param>
        /// <returns>an array of colors, each element for one day</returns>
		internal List<Color> GetBusinessDayColorsInRange(DateTime startDate, DateTime endDate)
		{
			var plans = GetInRange(startDate, endDate).OrderByDescending(x => x.ModifiedDate);
			List<Color> list = new List<Color>();
			for (DateTime date = startDate; date < endDate; date = date.AddDays(1))
			{
				var plan = plans.FirstOrDefault(x => x.StartDate <= date && date < x.EndDate);
				list.Add(WorkDay.GetColorByNr((int)GetEffectiveBizState(date, plan)));
			}
			return list;
		}
		
		/// <summary>
		/// Gets a list of active profile Shifts with their actual day start in the given time range
		/// </summary>
		/// <param name="startDate">this date&Time is considered</param>
		/// <param name="endDate">this date&Time is considered</param>
		/// <returns>Returns a list of Tuple&lt;<see cref="Soheil.Model.WorkShift"/>, DateTime&gt;</returns>
		public List<Tuple<WorkShift, DateTime>> GetShiftsInRange(DateTime startDate, DateTime endDate)
		{
			List<Tuple<WorkShift, DateTime>> list = new List<Tuple<WorkShift, DateTime>>();
			var plans = GetInRange(startDate, endDate).OrderByDescending(x => x.ModifiedDate);
			
			for (DateTime date = startDate; date < endDate; date = date.AddDays(1))
			{
				var plan = plans.FirstOrDefault(x => x.StartDate <= date && date < x.EndDate);

				var bizState = GetEffectiveBizState(date, plan);
				if (bizState != BusinessDayType.None)
				{
					var day = plan.WorkProfile.WorkDays.First(x => x.BusinessState == bizState);
					foreach (var shift in day.WorkShifts)
					{
						list.Add(new Tuple<WorkShift, DateTime>(shift, date));
					}
				}
			}
			return list;
		}

		#endregion	

	}
}
