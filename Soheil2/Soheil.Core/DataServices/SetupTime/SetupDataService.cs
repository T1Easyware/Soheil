using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Soheil.Core.Interfaces;
using Soheil.Model;
using Soheil.Dal;

namespace Soheil.Core.DataServices
{
	public class SetupDataService : IDataService<Setup>
	{
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
			throw new NotImplementedException();
		}

		public void UpdateModel(Setup model)
		{
			throw new NotImplementedException();
		}

		public void DeleteModel(Setup model)
		{
			throw new NotImplementedException();
		}

		public void AttachModel(Setup model)
		{
			throw new NotImplementedException();
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
