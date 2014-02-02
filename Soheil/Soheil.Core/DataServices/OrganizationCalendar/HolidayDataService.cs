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
    public class HolidayDataService : DataServiceBase, IDataService<Holiday>
	{
        public event EventHandler<ModelAddedEventArgs<Holiday>> HolidayAdded;
        Repository<Holiday> _holidayRepository;

		public HolidayDataService()
			:this(new SoheilEdmContext())
		{
		}
		public HolidayDataService(SoheilEdmContext context)
		{
			this.context = context;
			_holidayRepository = new Repository<Holiday>(context);
		}

        public Holiday GetSingle(int id)
		{
            return _holidayRepository.Single(x => x.Id == id);
		}

        public System.Collections.ObjectModel.ObservableCollection<Holiday> GetAll()
		{
            var list = new System.Collections.ObjectModel.ObservableCollection<Holiday>();
            var items = _holidayRepository.GetAll();
			foreach (var item in items)
			{
				list.Add(item);
			}
			return list;
		}

        public System.Collections.ObjectModel.ObservableCollection<Holiday> GetActives()
		{
			throw new NotImplementedException();
		}

        public int AddModel(Holiday model)
		{
           // model.ModifiedBy = LoginInfo.Id;
           // model.ModifiedDate = DateTime.Now;
           // model.CreatedDate = DateTime.Now;
            _holidayRepository.Add(model);
			context.Commit();

            HolidayAdded(this, new ModelAddedEventArgs<Holiday>(model));
			return model.Id;
		}

        public void UpdateModel(Holiday model)
		{
           // model.ModifiedBy = LoginInfo.Id;
           // model.ModifiedDate = DateTime.Now;
			context.SaveChanges();
		}

        public void DeleteModel(Holiday model)
		{
            _holidayRepository.Delete(model);
			context.Commit();
		}

        public void AttachModel(Holiday model)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// creates and saves a clone of the given model (id) and returns it
		/// </summary>
		/// <param name="id">Id of source model</param>
		/// <returns>clone</returns>
        internal Holiday CloneModelById(int id)
		{
            var model = _holidayRepository.Single(x => x.Id == id);
			var clone = cloneModel(model);
			context.SaveChanges();
			return clone;
		}
		public /*override*/ Holiday Clone(Holiday model)
        {
			var clone = cloneModel(model);
			context.SaveChanges();
			return clone;
        }
		/*
		 * 
		 * This was a fun method:
		 * 
		 * 
		 * public override T Clone<T>(T model)
		{
			var typed_model = (Holiday)Convert.ChangeType(model, typeof(Holiday));
			var typed_clone = cloneModel(typed_model);
			var t_clone = (T)Convert.ChangeType(typed_clone, typeof(T));
			context.SaveChanges();
			return t_clone;
		}*/

        protected Holiday cloneModel(Holiday model)
		{
            var clone = new Holiday();
			clone.Name = model.Name;
			clone.BusinessState = model.BusinessState;
			clone.Date = model.Date;
			clone.IsRecurrent = model.IsRecurrent;
            HolidayAdded(this, new ModelAddedEventArgs<Holiday>(clone));
			return clone;
		}

        internal void Postpone(Holiday model)
		{
			context.PostponeChanges(model);
		}

        internal Holiday GetCurrent()
        {
            return _holidayRepository.FirstOrDefault(x => x.Date == DateTime.Now.Date);
        }

		/// <summary>
		/// Get all holidays overlapping on the specified date
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
        internal List<Holiday> GetAtDate(DateTime date)
        {
			var result = new List<Holiday>();
            var d = date.Date;
			result.AddRange(_holidayRepository.Find(x => 
				x.Date == d)
				.ToList());
			result.AddRange(_holidayRepository.Find(x =>
				x.IsRecurrent &&
				x.Date.Month == d.Month && 
				x.Date.Day == d.Day
				).ToList());
			return result;
		}

		/// <summary>
		/// Gets all BusinessDayStates caused by holidays on the specified date
		/// <para>This does not include DaysOfWeek's state</para>
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		internal List<BusinessDayType> GetBusinessDayStatesAtDate(DateTime date)
		{
			return GetAtDate(date).Select(x=>x.BusinessState).ToList();
		}
    }
}
