using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
	public class NPTDataService : DataServiceBase, IDataService<NonProductiveTask>
	{
		Repository<NonProductiveTask> _nptRepository;

		public NPTDataService()
			: this(new SoheilEdmContext())
		{

		}
		public NPTDataService(SoheilEdmContext context)
		{
			this.context = context;
			_nptRepository = new Repository<NonProductiveTask>(context);
		}

		#region IDataService
		public NonProductiveTask GetSingle(int id)
		{
				var repository = new Repository<NonProductiveTask>(context);
				return repository.FirstOrDefault(x => x.Id == id);
		}

		public System.Collections.ObjectModel.ObservableCollection<NonProductiveTask> GetAll()
		{
			throw new NotImplementedException();
		}

		public System.Collections.ObjectModel.ObservableCollection<NonProductiveTask> GetActives()
		{
			throw new NotImplementedException();
		}

		public int AddModel(NonProductiveTask model)
		{
				var repository = new Repository<NonProductiveTask>(context);
				repository.Add(model);
				context.SaveChanges();
			return 1;
		}

		public void UpdateModel(NonProductiveTask model)
		{
				var repository = new Repository<NonProductiveTask>(context);
				var entity = repository.FirstOrDefault(x => x.Id == model.Id);
				if (entity == null) AddModel(model);
				else repository.Add(model);
				context.SaveChanges();
		}

		public void DeleteModel(NonProductiveTask model)
		{
				var repository = new Repository<NonProductiveTask>(context);
				var entity = repository.FirstOrDefault(x => x.Id == model.Id);
				if (entity != null) repository.Delete(entity);
				context.SaveChanges();
		}
		public void DeleteModel(int id)
		{
				var repository = new Repository<NonProductiveTask>(context);
				var entity = repository.FirstOrDefault(x => x.Id == id);
				if (entity != null) repository.Delete(entity);
				context.SaveChanges();
		}

		public void AttachModel(NonProductiveTask model)
		{
			throw new NotImplementedException();
		}
		#endregion

		/// <summary>
		/// Returns all NonProductiveTask Ids which are completely or partially inside the given range
		/// <para>NonProductiveTask touching the range from outside are not counted</para>
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public IEnumerable<int> GetIdsInRange(DateTime startDate, DateTime endDate)
		{
			return _nptRepository.Find(x =>
				(x.StartDateTime < endDate && x.StartDateTime >= startDate)
				||
				(x.EndDateTime <= endDate && x.EndDateTime > startDate)
				||
				(x.StartDateTime <= startDate && x.EndDateTime >= endDate))
				.OrderBy(y => y.StartDateTime).Select(x => x.Id);
		}
		/// <summary>
		/// Returns all NonProductiveTask which are completely or partially inside the given range
		/// <para>NonProductiveTask touching the range from outside are not counted</para>
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public IEnumerable<NonProductiveTask> GetInRange(DateTime startDate, DateTime endDate, int StationId)
		{
			var list = _nptRepository.Find(x =>
				(x.StartDateTime < endDate && x.StartDateTime >= startDate)
				||
				(x.EndDateTime <= endDate && x.EndDateTime > startDate)
				||
				(x.StartDateTime <= startDate && x.EndDateTime >= endDate));
			//???
			return list.OfType<Setup>().Where(x => x.Warmup.Station.Id == StationId).OrderBy(x => x.StartDateTime);
		}

		public IEnumerable<NonProductiveTask> GetInRange(DateTime startDate, int stationId)
		{
			var entityList = new List<NonProductiveTask>();
			entityList.AddRange(_nptRepository.OfType<Setup>(
				"Warmup", "Warmup.Station", "Warmup.ProductRework",
				"Changeover", "Changeover.FromProductRework", "Changeover.ToProductRework", "Changeover.Station")
				.Where(x => x.EndDateTime >= startDate && x.Warmup.Station.Id == stationId)
				.OrderBy(y => y.StartDateTime));//???
			return entityList;
		}

		internal int AddModel(PP.Smart.SmartRange setup)
		{
			try
			{
				return AddModel(new Setup
				{
					Changeover = new Repository<Changeover>(context).FirstOrDefault(x => x.Id == setup.ChangeoverId),
					Warmup = new Repository<Warmup>(context).FirstOrDefault(x => x.Id == setup.WarmupId),
					StartDateTime = setup.StartDT,
					DurationSeconds = setup.DurationSeconds,
					EndDateTime = setup.EndDT,
					Description = "[Auto Insert]",
				});
			}
			catch { return -1; }
		}
	}
}
