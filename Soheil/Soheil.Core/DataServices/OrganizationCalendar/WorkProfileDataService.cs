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

namespace Soheil.Core.DataServices
{
	public class WorkProfileDataService : DataServiceBase, IDataService<WorkProfile>
	{
		public event EventHandler<ModelAddedEventArgs<WorkProfile>> WorkProfileAdded;
		Repository<WorkProfile> workProfileRepository;

		public WorkProfileDataService() : this(new SoheilEdmContext())
		{
		}
		public WorkProfileDataService(SoheilEdmContext context)
		{
			this.context = context;
			workProfileRepository = new Repository<WorkProfile>(context);
		}

		public WorkProfile GetSingle(int id)
		{
			return new Repository<WorkProfile>(context).Single(x => x.Id == id,
				"WorkDays",
				"WorkShiftPrototypes",
				"WorkDays.WorkShifts",
				"WorkDays.WorkShifts.WorkShiftPrototype",
				"WorkDays.WorkShifts.WorkBreaks");
		}

		public System.Collections.ObjectModel.ObservableCollection<WorkProfile> GetAll()
		{
			var list = new System.Collections.ObjectModel.ObservableCollection<WorkProfile>();
			var items = workProfileRepository.GetAll(
				"WorkDays",
				"WorkShiftPrototypes",
				"WorkDays.WorkShifts",
				"WorkDays.WorkShifts.WorkShiftPrototype",
				"WorkDays.WorkShifts.WorkBreaks");
			foreach (var item in items)
			{
				list.Add(item);
			}
			return list;
		}

		public System.Collections.ObjectModel.ObservableCollection<WorkProfile> GetActives()
		{
			throw new NotImplementedException();
		}

		public int AddModel(WorkProfile model)
		{
			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;
			model.CreatedDate = DateTime.Now;
			workProfileRepository.Add(model);
			context.Commit();

			WorkProfileAdded(this, new ModelAddedEventArgs<WorkProfile>(model));
			return model.Id;
		}

		public void UpdateModel(WorkProfile model)
		{
			context.SaveChanges();
			//var workProfileRepository = new Repository<WorkProfile>(context);
			//WorkProfile entity = workProfileRepository.Single(x => x.Id == model.Id);
			/*entity.Name = model.Name;
			entity.Name = model.Name;
			entity.WeekStartNr = model.WeekStartNr;
			entity.SaturdayBusinessStateNr = model.SaturdayBusinessStateNr;
			entity.SundayBusinessStateNr = model.SundayBusinessStateNr;
			entity.MondayBusinessStateNr = model.MondayBusinessStateNr;
			entity.TuesdayBusinessStateNr = model.TuesdayBusinessStateNr;
			entity.WednesdayBusinessStateNr = model.WednesdayBusinessStateNr;
			entity.ThursdayBusinessStateNr = model.ThursdayBusinessStateNr;
			entity.FridayBusinessStateNr = model.FridayBusinessStateNr;
			entity.ModifiedBy = LoginInfo.Id;
			entity.CreatedDate = DateTime.Now;
			entity.ModifiedDate = DateTime.Now;
			foreach (var protoModel in model.WorkShiftPrototypes.ToArray())
			{
				entity.WorkShiftPrototypes.Add(new WorkShiftPrototype
				{
					WorkProfile = entity,
					Index = protoModel.Index,
					ColorNr = protoModel.ColorNr,
					Name = protoModel.Name,
				});
			}
			foreach (var workDayModel in model.WorkDays.ToArray())
			{
				var workDayEntity = new WorkDay();
				workDayEntity.WorkProfile = entity;
				workDayEntity.Name = workDayModel.Name;
				workDayEntity.ColorNr = workDayModel.ColorNr;
				workDayEntity.BusinessStateNr = workDayModel.BusinessStateNr;
				foreach (var workShiftModel in workDayModel.WorkShifts.ToArray())
				{
					var workShiftEntity = new WorkShift
					{
						WorkDay = workDayModel,
						WorkShiftPrototype = entity.WorkShiftPrototypes.First(x => x.Index == workShiftModel.WorkShiftPrototype.Index),
						StartSeconds = workShiftModel.StartSeconds,
						EndSeconds = workShiftModel.EndSeconds,
					};
					foreach (var workBreakModel in workShiftModel.WorkBreaks.ToArray())
					{
						var workBreakEntity = new WorkBreak();
						workBreakEntity.WorkShift = workShiftModel;
						workBreakEntity.StartSeconds = workBreakModel.StartSeconds;
						workBreakEntity.EndSeconds = workBreakModel.EndSeconds;
						workShiftEntity.WorkBreaks.Add(workBreakEntity);
					}
					workDayEntity.WorkShifts.Add(workShiftEntity);
				}
				entity.WorkDays.Add(workDayEntity);
			}
			WorkProfileAdded(this, new ModelAddedEventArgs<WorkProfile>(entity));*/
		}

		public void DeleteModel(WorkProfile model)
		{
			var newModel = workProfileRepository.Single(cost => cost.Id == model.Id);
			var protoRepos = new Repository<WorkShiftPrototype>(context);
			var dayRepos = new Repository<WorkDay>(context);
			var shiftRepos = new Repository<WorkShift>(context);
			var breakRepos = new Repository<WorkBreak>(context);

			//delete old items
			foreach (var day in newModel.WorkDays.ToArray())
			{
				foreach (var shift in day.WorkShifts.ToArray())
				{
					foreach (var wbreak in shift.WorkBreaks.ToArray())
					{
						breakRepos.Delete(wbreak);
					}
					shift.WorkBreaks.Clear();
					shiftRepos.Delete(shift);
				}
				day.WorkShifts.Clear();
				dayRepos.Delete(day);
			}
			newModel.WorkDays.Clear();
			foreach (var proto in newModel.WorkShiftPrototypes.ToArray())
			{
				proto.WorkShifts.Clear();
				protoRepos.Delete(proto);
			}
			newModel.WorkShiftPrototypes.Clear();

			workProfileRepository.Delete(newModel);
			context.Commit();
		}

		public void AttachModel(WorkProfile model)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// creates and saves a clone of the given model (id) and returns it
		/// </summary>
		/// <param name="id">Id of source model</param>
		/// <returns>clone</returns>
		internal WorkProfile CloneModelById(int id)
		{
			var workProfileRepository = new Repository<WorkProfile>(context);
			var model = workProfileRepository.Single(x => x.Id == id,
				"WorkDays",
				"WorkShiftPrototypes",
				"WorkDays.WorkShifts",
				"WorkDays.WorkShifts.WorkShiftPrototype",
				"WorkDays.WorkShifts.WorkBreaks");
			var clone = cloneModel(model);
			context.SaveChanges();
			return clone;
		}
		public /*override*/ WorkProfile Clone(WorkProfile model)
		{
			var clone = cloneModel(model);
			context.SaveChanges();
			return clone;
		}

		protected WorkProfile cloneModel(WorkProfile model)
		{
			var clone = new WorkProfile();
			clone.Name = model.Name;
			clone.WeekStartNr = model.WeekStartNr;
			clone.SaturdayBusinessStateNr = model.SaturdayBusinessStateNr;
			clone.SundayBusinessStateNr = model.SundayBusinessStateNr;
			clone.MondayBusinessStateNr = model.MondayBusinessStateNr;
			clone.TuesdayBusinessStateNr = model.TuesdayBusinessStateNr;
			clone.WednesdayBusinessStateNr = model.WednesdayBusinessStateNr;
			clone.ThursdayBusinessStateNr = model.ThursdayBusinessStateNr;
			clone.FridayBusinessStateNr = model.FridayBusinessStateNr;
			clone.ModifiedBy = LoginInfo.Id;
			clone.CreatedDate = DateTime.Now;
			clone.ModifiedDate = DateTime.Now;
			foreach (var protoModel in model.WorkShiftPrototypes.ToArray())
			{
				clone.WorkShiftPrototypes.Add(new WorkShiftPrototype
				{
					WorkProfile = clone,
					Index = protoModel.Index,
					ColorNr = protoModel.ColorNr,
					Name = protoModel.Name,
				});
			}
			foreach (var workDayModel in model.WorkDays.ToArray())
			{
				var workDayClone = new WorkDay();
				workDayClone.WorkProfile = clone;
				workDayClone.Name = workDayModel.Name;
				workDayClone.ColorNr = workDayModel.ColorNr;
				workDayClone.BusinessStateNr = workDayModel.BusinessStateNr;
				foreach (var workShiftModel in workDayModel.WorkShifts.ToArray())
				{
					var workShiftClone = new WorkShift
					{
						WorkDay = workDayModel,
						WorkShiftPrototype = clone.WorkShiftPrototypes.First(x => x.Index == workShiftModel.WorkShiftPrototype.Index),
						StartSeconds = workShiftModel.StartSeconds,
						EndSeconds = workShiftModel.EndSeconds,
					};
					foreach (var workBreakModel in workShiftModel.WorkBreaks.ToArray())
					{
						var workBreakClone = new WorkBreak();
						workBreakClone.WorkShift = workShiftModel;
						workBreakClone.StartSeconds = workBreakModel.StartSeconds;
						workBreakClone.EndSeconds = workBreakModel.EndSeconds;
						workShiftClone.WorkBreaks.Add(workBreakClone);
					}
					workDayClone.WorkShifts.Add(workShiftClone);
				}
				clone.WorkDays.Add(workDayClone);
			}
			if (WorkProfileAdded != null)
				WorkProfileAdded(this, new ModelAddedEventArgs<WorkProfile>(clone));
			return clone;
		}

		internal void Postpone(WorkProfile model)
		{
			context.PostponeChanges(model);
			foreach (var proto in model.WorkShiftPrototypes)
			{
				context.PostponeChanges(proto);
				foreach (var shift in proto.WorkShifts)
				{
					context.PostponeChanges(shift);
					foreach (var wbreak in shift.WorkBreaks)
					{
						context.PostponeChanges(wbreak);
					}
				}
			}
			foreach (var day in model.WorkDays)
			{
				context.PostponeChanges(day);
			}
		}
	}
}
