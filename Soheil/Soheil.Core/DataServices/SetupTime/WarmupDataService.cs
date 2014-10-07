using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Soheil.Core.Interfaces;
using Soheil.Model;
using Soheil.Dal;

namespace Soheil.Core.DataServices
{
	public class WarmupDataService : IDataService<Warmup>
	{
		public Warmup GetSingle(int id)
		{
			throw new NotImplementedException();
		}
		public Warmup Save(Warmup source, int seconds)
		{
			using (var context = new SoheilEdmContext())
			{
				Warmup model = null;
				var warmupRepos = new Repository<Warmup>(context);

				if (source.Id > 0)
					model = warmupRepos.FirstOrDefault(x => x.Id == source.Id);
				if (model == null)
					model = warmupRepos.FirstOrDefault(x =>
						x.Station.Id == source.Station.Id &&
						x.ProductRework.Id == source.ProductRework.Id,
						"Station",
						"ProductRework");
				if (model == null)
					model = new Warmup
					{
						ProductRework = new Repository<ProductRework>(context).Single(x => x.Id == source.ProductRework.Id),
						Station = new Repository<Station>(context).Single(x => x.Id == source.Station.Id),
					};
				model.Seconds = seconds;
				context.Commit();
				return model;
			}
		}
		public Warmup SmartApply(int stationId, int targetPRId, int seconds)
		{
			using (var context = new SoheilEdmContext())
			{
				Warmup model = new Repository<Warmup>(context).FirstOrDefault(x =>
					x.Station.Id == stationId &&
					x.ProductRework.Id == targetPRId);
				if (model == null)
					model = new Warmup
					{
						Station = new Repository<Station>(context).Single(x => x.Id == stationId),
						ProductRework = new Repository<ProductRework>(context).Single(x => x.Id == targetPRId),
					};
				model.Seconds = seconds;
				context.Commit();
				return model;
			}
		}
		public Warmup SmartFind(int productReworkId, int stationId)
		{
			if (productReworkId == 0 || stationId == 0) return null;
			using (var context = new SoheilEdmContext())
			{
				var warmupRepos = new Repository<Warmup>(context);

				var model = warmupRepos.FirstOrDefault(x => x.Station.Id == stationId && x.ProductRework.Id == productReworkId);
				if (model == null)
				{
					model = new Warmup
						{
							ProductRework = new Repository<ProductRework>(context).FirstOrDefault(x => x.Id == productReworkId),
							Seconds = 0,
							Station = new Repository<Station>(context).FirstOrDefault(x => x.Id == stationId),
						};
					warmupRepos.Add(model);
					context.Commit();
				}
				return model;
			}
		}

		public System.Collections.ObjectModel.ObservableCollection<Warmup> GetAll()
		{
			throw new NotImplementedException();
		}

		public System.Collections.ObjectModel.ObservableCollection<Warmup> GetActives()
		{
			throw new NotImplementedException();
		}

		public int AddModel(Warmup model)
		{
			throw new NotImplementedException();
		}

		public void UpdateModel(Warmup model)
		{
			throw new NotImplementedException();
		}

		public void DeleteModel(Warmup model)
		{
			throw new NotImplementedException();
		}

		public void AttachModel(Warmup model)
		{
			throw new NotImplementedException();
		}

	}
}
