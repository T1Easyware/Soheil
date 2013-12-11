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
        #region IDataService<Cause> Members

        public Cause GetSingle(int id)
        {
            Cause entity;
            using (var context = new SoheilEdmContext())
            {
                var causeRepository = new Repository<Cause>(context);
                entity = causeRepository.FirstOrDefault(cause => cause.Id == id, "Parent", "Children");
            }
            return entity;
        }

        public ObservableCollection<Cause> GetAll()
        {
            ObservableCollection<Cause> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Cause>(context);
                IEnumerable<Cause> entityList = repository.Find(cause=> cause.Status != (decimal)Status.Deleted, "Children");
                models = new ObservableCollection<Cause>(entityList);
            }
            return models;
        }

        public ObservableCollection<Cause> GetActives()
        {
            ObservableCollection<Cause> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Cause>(context);
                IEnumerable<Cause> entityList = repository.Find(cause => cause.Status == (decimal)Status.Active, "Children");
                models = new ObservableCollection<Cause>(entityList);
            }
            return models;
        }

        public int AddModel(Cause model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Cause>(context);
                repository.Add(model);
                context.Commit();
                if (CauseAdded != null)
                    CauseAdded(this, new ModelAddedEventArgs<Cause>(model));
                id = model.Id;
            }
            return id;
        }

        public int AddModel(Cause model, int parentId)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Cause>(context);
                var parent = repository.FirstOrDefault(cause => cause.Id == parentId);
                model.Parent = parent;
                parent.Children.Add(model);
                context.Commit();
                if (CauseAdded != null)
                    CauseAdded(this, new ModelAddedEventArgs<Cause>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(Cause model)
        {
            using (var context = new SoheilEdmContext())
            {
                var causeRepository = new Repository<Cause>(context);
                Cause entity = causeRepository.Single(cause => cause.Id == model.Id);

                entity.Code = model.Code;
                entity.Name = model.Name;
                context.Commit();
            }
        }

        public void DeleteModel(Cause model)
        {
        }

        public void AttachModel(Cause model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Cause>(context);
                if (repository.Exists(cause => cause.Id == model.Id))
                {
                    UpdateModel(model);
                }
                else
                {
                    AddModel(model);
                }
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