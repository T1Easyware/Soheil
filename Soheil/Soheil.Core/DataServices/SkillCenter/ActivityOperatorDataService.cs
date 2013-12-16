using System;
using System.Collections.ObjectModel;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class ActivityOperatorDataService : IDataService<OperatorActivity>
    {
        public event EventHandler<ModelAddedEventArgs<OperatorActivity>> ModelUpdated;
        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public OperatorActivity GetSingle(int id)
        {
            OperatorActivity entity;
            using (var context = new SoheilEdmContext())
            {
                var operatorRepository = new Repository<OperatorActivity>(context);
                entity = operatorRepository.FirstOrDefault(model => model.Id == id, "Operator","Activity");
            }
            return entity;
        }

        /// <summary>
        /// Gets a list of view models representing all records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<OperatorActivity> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a list of view models representing currently active records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<OperatorActivity> GetActives()
        {
            throw new NotImplementedException();
        }

        public int AddModel(OperatorActivity model)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateModel(OperatorActivity model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<OperatorActivity>(context);
                OperatorActivity entity = repository.Single(operatorActivity => operatorActivity.Id == model.Id);

                entity.IluoNr = model.IluoNr;

                context.Commit();
                if (ModelUpdated != null) ModelUpdated(this, new ModelAddedEventArgs<OperatorActivity>(entity));
            }
        }

        public void DeleteModel(OperatorActivity model)
        {
            throw new System.NotImplementedException();
        }

        public void AttachModel(OperatorActivity model)
        {
            throw new System.NotImplementedException();
        }
    }
}