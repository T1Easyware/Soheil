using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
	public class CauseDataService : RecursiveDataServiceBase, IDataService<Cause>
	{
		Repository<Cause> _causeRepository;

		public CauseDataService()
			: this(new SoheilEdmContext())
		{
		}
		public CauseDataService(SoheilEdmContext context)
		{
			this.context = context;
			_causeRepository = new Repository<Cause>(context);
		}

		#region IDataService<Cause> Members

		public Cause GetSingle(int id)
		{
			Cause entity;
			entity = _causeRepository.FirstOrDefault(cause => cause.Id == id, "Parent", "Children");
			return entity;
		}

		public ObservableCollection<Cause> GetAll()
		{
			ObservableCollection<Cause> models;
			IEnumerable<Cause> entityList = _causeRepository.Find(cause => cause.Status != (decimal)Status.Deleted);
			models = new ObservableCollection<Cause>(entityList);
			return models;
		}

		public ObservableCollection<Cause> GetActives()
		{
			ObservableCollection<Cause> models;
			IEnumerable<Cause> entityList = _causeRepository.Find(cause => cause.Status == (decimal)Status.Active, "Children");
			models = new ObservableCollection<Cause>(entityList);
			return models;
		}

		public Cause GetRoot()
		{
			return _causeRepository.FirstOrDefault(x => x.Level == 0, "Children.Children.Children");
		}

		public int AddModel(Cause model)
		{
			int id;
			_causeRepository.Add(model);
			context.Commit();
			if (CauseAdded != null)
				CauseAdded(this, new ModelAddedEventArgs<Cause>(model));
			id = model.Id;
			return id;
		}

		public int AddModel(Cause model, int parentId)
		{
			int id;
			var parent = _causeRepository.FirstOrDefault(cause => cause.Id == parentId);
			model.Parent = parent;
			parent.Children.Add(model);
			context.Commit();
			if (CauseAdded != null)
				CauseAdded(this, new ModelAddedEventArgs<Cause>(model));
			id = model.Id;
			return id;
		}

		public void UpdateModel(Cause model)
		{
			Cause entity = _causeRepository.Single(cause => cause.Id == model.Id);

			entity.Code = model.Code;
			entity.Name = model.Name;
		    entity.Status = model.Status;
			context.Commit();
		}

		public void DeleteModel(Cause model)
		{
		}

		public void AttachModel(Cause model)
		{
			if (_causeRepository.Exists(cause => cause.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

		#endregion

		public event EventHandler<ModelAddedEventArgs<Cause>> CauseAdded;

		#region Overrides of RecursiveDataServiceBase

		public override ObservableCollection<IEntityNode> GetChildren(int id)
		{
			//var allNodes = GetAll();
			//return GetChildrenNodes(allNodes,GetSingle(id));
			return null;
		}
		#endregion
	}
}