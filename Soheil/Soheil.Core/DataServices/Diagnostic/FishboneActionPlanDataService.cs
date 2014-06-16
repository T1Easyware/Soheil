using System;
using System.Collections.ObjectModel;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class FishboneActionPlanDataService : DataServiceBase, IDataService<FishboneNode_ActionPlan>
    {
        public event EventHandler<ModelAddedEventArgs<FishboneNode_ActionPlan>> ModelUpdated;
        private readonly Repository<FishboneNode_ActionPlan> _fishboneActionplanRepository;
        public FishboneActionPlanDataService(SoheilEdmContext context)
        {
            Context = context;
            _fishboneActionplanRepository = new Repository<FishboneNode_ActionPlan>(Context);
        }
        public FishboneActionPlanDataService()
        {
            Context = new SoheilEdmContext();
            _fishboneActionplanRepository = new Repository<FishboneNode_ActionPlan>(Context);
        }
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
                FishboneNode_ActionPlan entity = _fishboneActionplanRepository.Single(productDefection => productDefection.Id == model.Id);

                Context.Commit();
                if (ModelUpdated != null) ModelUpdated(this, new ModelAddedEventArgs<FishboneNode_ActionPlan>(entity));
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