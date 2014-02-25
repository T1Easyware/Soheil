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
		/// Returns all NonProductiveTask which are completely or partially inside the given range
		/// <para>NonProductiveTask touching the range from outside are not counted</para>
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public IEnumerable<NonProductiveTask> GetInRange(DateTime startDate, DateTime endDate)
		{
			return _nptRepository.Find(x =>
				(x.StartDateTime < endDate && x.StartDateTime >= startDate)
				||
				(x.EndDateTime <= endDate && x.EndDateTime > startDate)
				||
				(x.StartDateTime <= startDate && x.EndDateTime >= endDate),
				y => y.StartDateTime);
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

		/// <summary>
		/// Returns a value indicating whether or not this npt still exists
		/// </summary>
		/// <param name="vm"></param>
		/// <returns></returns>
		internal bool UpdateViewModel(ViewModels.PP.NPTVm vm)
		{
			var model = _nptRepository.FirstOrDefault(x => x.Id == vm.Id);
			if (model == null) return false;
			vm.StartDateTime = model.StartDateTime;
			vm.DurationSeconds = model.DurationSeconds;
			if (vm is ViewModels.PP.SetupVm && model is Setup)
				updateViewModel(vm as ViewModels.PP.SetupVm, model as Setup);
			else//???
				return false;
			return true;
		}
		private void updateViewModel(ViewModels.PP.SetupVm vm, Setup model)
		{
			vm.ChangeoverId = model.Changeover.Id;
			vm.ChangeoverSeconds = model.Changeover.Seconds;

			vm.WarmupId = model.Warmup.Id;
			vm.WarmupSeconds = model.Warmup.Seconds;

			vm.FromProduct = new ViewModels.PP.ProductReworkVm(model.Changeover.FromProductRework);
			vm.ToProduct = new ViewModels.PP.ProductReworkVm(model.Changeover.ToProductRework);
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
