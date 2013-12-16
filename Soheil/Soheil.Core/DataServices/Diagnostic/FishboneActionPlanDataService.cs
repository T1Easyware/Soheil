using System;
using System.Collections.ObjectModel;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class FishboneActionPlanDataService : IDataService<FishboneNode_ActionPlan>
    {
        public event EventHandler<ModelAddedEventArgs<FishboneNode_ActionPlan>> ModelUpdated;
        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public FishboneNode_ActionPlan GetSingle(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a list of view models representing all records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<FishboneNode_ActionPlan> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a list of view models representing currently active records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<FishboneNode_ActionPlan> GetActives()
        {
            throw new NotImplementedException();
        }

        public int AddModel(FishboneNode_ActionPlan model)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateModel(FishboneNode_ActionPlan model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<FishboneNode_ActionPlan>(context);
                FishboneNode_ActionPlan entity = repository.Single(productDefection => productDefection.Id == model.Id);

                context.Commit();
                if (ModelUpdated != null) ModelUpdated(this, new ModelAddedEventArgs<FishboneNode_ActionPlan>(entity));
            }
        }

        public void DeleteModel(FishboneNode_ActionPlan model)
        {
            throw new System.NotImplementedException();
        }

        public void AttachModel(FishboneNode_ActionPlan model)
        {
            throw new System.NotImplementedException();
        }
    }
}