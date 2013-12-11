using System;
using System.Collections.ObjectModel;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class StationMachineDataService : IDataService<StationMachine>
    {
        public event EventHandler<ModelAddedEventArgs<StationMachine>> ModelUpdated;
        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public StationMachine GetSingle(int id)
        {
            StationMachine entity;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<StationMachine>(context);
                entity = repository.Single(productDefection => productDefection.Id == id);
            }
            return entity;
        }

        /// <summary>
        /// Gets a list of view models representing all records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<StationMachine> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a list of view models representing currently active records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<StationMachine> GetActives()
        {
            throw new NotImplementedException();
        }

        public int AddModel(StationMachine model)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateModel(StationMachine model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<StationMachine>(context);
                StationMachine entity = repository.Single(productDefection => productDefection.Id == model.Id);

                context.Commit();
                if (ModelUpdated != null) ModelUpdated(this, new ModelAddedEventArgs<StationMachine>(entity));
            }
        }

        public void DeleteModel(StationMachine model)
        {
            throw new System.NotImplementedException();
        }

        public void AttachModel(StationMachine model)
        {
            throw new System.NotImplementedException();
        }
    }
}