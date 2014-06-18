using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Soheil.Core.Interfaces;
using Soheil.Model;
using Soheil.Dal;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
	public class SetupDataService : DataServiceBase, IDataService<Setup>
	{
		public SetupDataService()
			: this(new Dal.SoheilEdmContext())
		{

		}
		public SetupDataService(Dal.SoheilEdmContext context)
		{
			this.Context = context;
		}
		public Setup GetSingle(int id)
		{
			using (var context = new SoheilEdmContext())
			{
				return new Repository<NonProductiveTask>(context).OfType<Setup>().FirstOrDefault(x => x.Id == id);
			}
		}

		public System.Collections.ObjectModel.ObservableCollection<Setup> GetAll()
		{
			throw new NotImplementedException();
		}

		public System.Collections.ObjectModel.ObservableCollection<Setup> GetActives()
		{
			throw new NotImplementedException();
		}

		public int AddModel(Setup model)
		{
			new Repository<NonProductiveTask>(Context).Add(model);
			Context.Commit();
			return model.Id;
		}

		public void UpdateModel(Setup model)
		{
			throw new NotImplementedException();
		}


		/// <summary>
		/// No commit
		/// </summary>
		/// <param name="model"></param>
		public void DeleteModel(Setup model)
		{
			new Repository<NonProductiveTask>(Context).Delete(model);
		}

		public void AttachModel(Setup model)
		{
			throw new NotImplementedException();
		}

		internal Setup AddModel(int stationId, int from, int to, DateTime start)
		{
			var repos = new Repository<NonProductiveTask>(Context);
			var warmupRepos = new Repository<Warmup>(Context);
			var changeoverRepos = new Repository<Changeover>(Context);
			var changeover = changeoverRepos.FirstOrDefault(x => x.FromProductRework.Id == from && x.ToProductRework.Id == to && x.Station.Id == stationId);
			var warmup = warmupRepos.FirstOrDefault(x => x.ProductRework.Id == to && x.Station.Id == stationId);
			var duration = (changeover == null ? 0 : changeover.Seconds) + (warmup == null ? 0 : warmup.Seconds);
			var entity = new Setup
			{
				Changeover = changeover,
				Warmup = warmup,
				StartDateTime = start,
				EndDateTime = start.AddSeconds(duration),
				DurationSeconds = duration,
				Description = "Auto Generated",
			};
			repos.Add(entity);
			return entity;
		}
		internal void AddModelBySmart(PP.Smart.SmartRange item, SoheilEdmContext context)
		{
			var repos = new Repository<NonProductiveTask>(context);
			var warmupRepos = new Repository<Warmup>(context);
			var changeoverRepos = new Repository<Changeover>(context);
			var entity = new Setup
			{
				Changeover = changeoverRepos.FirstOrDefault(x => x.Id == item.ChangeoverId),
				Warmup = warmupRepos.FirstOrDefault(x => x.Id == item.WarmupId),
				StartDateTime = item.StartDT,
				EndDateTime = item.EndDT,
				DurationSeconds = item.DurationSeconds,
				Description = "Auto Generated",
			};
			repos.Add(entity);
		}

		internal bool DeleteModelById(int setupId, SoheilEdmContext context)
		{
			var repos = new Repository<NonProductiveTask>(context);
			var model = repos.FirstOrDefault(x => x.Id == setupId);
			if (model != null)
			{
				repos.Delete(model);
				return true;
			}
			else return false;
		}

	}
}
