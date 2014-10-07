using Soheil.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Soheil.Model;
using Soheil.Dal;

namespace Soheil.Core.DataServices
{
	public class ChangeoverDataService : IDataService<Changeover>
	{
		public Changeover GetSingle(int id)
		{
			throw new NotImplementedException();
		}
		public Changeover Save(Changeover source, int value)
		{
			using (var context = new SoheilEdmContext())
			{
				var model = smartFind(source, context);
				if (model == null)
					model = new Changeover
					{
						Station = new Repository<Station>(context).Single(x => x.Id == source.Station.Id),
						FromProductRework = new Repository<ProductRework>(context).Single(x => x.Id == source.FromProductRework.Id),
						ToProductRework = new Repository<ProductRework>(context).Single(x => x.Id == source.ToProductRework.Id),
					};
				model.Seconds = value;
				context.Commit();
				return model;
			}
		}
		private Changeover smartFind(Changeover source, SoheilEdmContext context)
		{
			var changeoverRepos = new Repository<Changeover>(context);
			Changeover model = null;
			if (source.Id > 0)
				model = changeoverRepos.FirstOrDefault(x => x.Id == source.Id);
			if (model == null)
				model = changeoverRepos.FirstOrDefault(x =>
					x.Station.Id == source.Station.Id &&
					x.FromProductRework.Id == source.FromProductRework.Id &&
					x.ToProductRework.Id == source.ToProductRework.Id,
					"Station",
					"FromProductRework",
					"ToProductRework");
			return model;
		}
		public Changeover GetByInfoOrAdd(int stationId, int fromPRId, int toPRId, SoheilEdmContext context)
		{
			var changeoverRepos = new Repository<Changeover>(context);
			var model = changeoverRepos.FirstOrDefault(x => 
				x.Station.Id == stationId &&
				x.FromProductRework.Id == fromPRId &&
				x.ToProductRework.Id == toPRId,
				"Station", 
				"FromProductRework",
				"ToProductRework");
			if (model == null)
			{
				model = new Changeover
					{
						Station = new Repository<Station>(context).Single(x => x.Id == stationId),
						FromProductRework = new Repository<ProductRework>(context).Single(x => x.Id == fromPRId),
						ToProductRework = new Repository<ProductRework>(context).Single(x => x.Id == toPRId),
					};
				context.Commit();
			}
			return model;
		}
		public Changeover SmartApply(int stationId, int fromPRId, int toPRId, int value)
		{
			using (var context = new SoheilEdmContext())
			{
				var model = GetByInfoOrAdd(stationId, fromPRId, toPRId, context);
				if (model == null)
					model = new Changeover
					{
						Station = new Repository<Station>(context).Single(x => x.Id == stationId),
						FromProductRework = new Repository<ProductRework>(context).Single(x => x.Id == fromPRId),
						ToProductRework = new Repository<ProductRework>(context).Single(x => x.Id == toPRId),
					};
				model.Seconds = value;
				context.Commit();
				return model;
			}
		}
		public Changeover SmartFind(int fromProductReworkId, int toProductReworkId, int stationId)
		{
			if (fromProductReworkId == 0 || toProductReworkId == 0 || stationId == 0) return null;
			using (var context = new SoheilEdmContext())
			{
				var changeoverRepos = new Repository<Changeover>(context);

				var model = changeoverRepos.FirstOrDefault(x => x.Station.Id == stationId
					&& x.FromProductRework.Id == fromProductReworkId
					&& x.ToProductRework.Id == toProductReworkId);
				if (model == null)
				{
					model = new Changeover
					{
						FromProductRework = new Repository<ProductRework>(context).FirstOrDefault(x => x.Id == fromProductReworkId),
						ToProductRework = new Repository<ProductRework>(context).FirstOrDefault(x => x.Id == toProductReworkId),
						Station = new Repository<Station>(context).FirstOrDefault(x=>x.Id == stationId),
						Seconds = 0
					};
					changeoverRepos.Add(model);
					context.Commit();
				}
				return model;
			}
		}

		public System.Collections.ObjectModel.ObservableCollection<Changeover> GetAll()
		{
			throw new NotImplementedException();
		}

		public System.Collections.ObjectModel.ObservableCollection<Changeover> GetActives()
		{
			throw new NotImplementedException();
		}

		public int AddModel(Changeover model)
		{
			throw new NotImplementedException();
		}

		public void UpdateModel(Changeover model)
		{
			throw new NotImplementedException();
		}

		public void DeleteModel(Changeover model)
		{
			throw new NotImplementedException();
		}

		public void AttachModel(Changeover model)
		{
			throw new NotImplementedException();
		}


	}
}
