using System;
using System.Collections.ObjectModel;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class ActivityOperatorDataService : IDataService<GeneralActivitySkill>
    {
        public event EventHandler<ModelAddedEventArgs<GeneralActivitySkill>> ModelUpdated;
        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public GeneralActivitySkill GetSingle(int id)
        {
            GeneralActivitySkill entity;
            using (var context = new SoheilEdmContext())
            {
                var operatorRepository = new Repository<GeneralActivitySkill>(context);
                entity = operatorRepository.FirstOrDefault(model => model.Id == id, "Operator","Activity");
            }
            return entity;
        }

        /// <summary>
        /// Gets a list of view models representing all records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<GeneralActivitySkill> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a list of view models representing currently active records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<GeneralActivitySkill> GetActives()
        {
            throw new NotImplementedException();
        }

        public int AddModel(GeneralActivitySkill model)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateModel(GeneralActivitySkill model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<GeneralActivitySkill>(context);
                GeneralActivitySkill entity = repository.Single(generalActivitySkill => generalActivitySkill.Id == model.Id);

                entity.IluoNr = model.IluoNr;

                context.Commit();
                if (ModelUpdated != null) ModelUpdated(this, new ModelAddedEventArgs<GeneralActivitySkill>(entity));
            }
        }

        public void DeleteModel(GeneralActivitySkill model)
        {
            throw new System.NotImplementedException();
        }

        public void AttachModel(GeneralActivitySkill model)
        {
            throw new System.NotImplementedException();
        }
    }
}