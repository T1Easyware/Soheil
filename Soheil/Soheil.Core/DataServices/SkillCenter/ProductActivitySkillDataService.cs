using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class ProductActivitySkillDataService : DataServiceBase, IDataService<ProductActivitySkill>
    {
		public ProductActivitySkillDataService()
			:this(new SoheilEdmContext())
		{

		}
		public ProductActivitySkillDataService(SoheilEdmContext context)
		{
			this.Context = context;
		}

        #region IDataService<SpecialSkill> Members

		public ProductActivitySkill GetSingle(int id)
        {
			ProductActivitySkill entity;

				var specialSkillRepository = new Repository<ProductActivitySkill>(Context);
                entity = specialSkillRepository.Single(specialSkill => specialSkill.Id == id);

            return entity;
        }

		public ObservableCollection<ProductActivitySkill> GetAll()
        {
			ObservableCollection<ProductActivitySkill> models;

				var repository = new Repository<ProductActivitySkill>(Context);
				IEnumerable<ProductActivitySkill> entityList = repository.GetAll();
				models = new ObservableCollection<ProductActivitySkill>(entityList);

            return models;
        }

		public ObservableCollection<ProductActivitySkill> GetActives()
        {
            return GetAll();
        }

		public int AddModel(ProductActivitySkill model)
        {
            //int id;
            //using (var context = new SoheilEdmContext())
            //{
            //    var repository = new Repository<SpecialSkill>(context);
            //    repository.Add(model);
            //    context.Commit();
            //    if (SpecialSkillAdded != null)
            //        SpecialSkillAdded(this, new ModelAddedEventArgs<SpecialSkill>(model));
            //    id = model.Id;
            //}
            //return id;
            return -1;
        }

		public void UpdateModel(ProductActivitySkill model)
        {

				var specialSkillRepository = new Repository<ProductActivitySkill>(Context);
				ProductActivitySkill entity = specialSkillRepository.Single(specialSkill => specialSkill.Id == model.Id);

               // entity.Reserve1 = model.Reserve1;
            //    entity.Reserve2 = model.Reserve2;
              //  entity.Reserve3 = model.Reserve3;
                entity.ModifiedBy = LoginInfo.Id;
                Context.Commit();

        }

		public void DeleteModel(ProductActivitySkill model)
        {
        }

		public void AttachModel(ProductActivitySkill model)
        {

				var repository = new Repository<ProductActivitySkill>(Context);
                if (repository.Exists(specialSkill => specialSkill.Id == model.Id))
                {
                    UpdateModel(model);
                }
                else
                {
                    AddModel(model);
                }

        }

        #endregion

		public event EventHandler<ModelAddedEventArgs<ProductActivitySkill>> SpecialSkillAdded;

		public ProductActivitySkill FindOrAdd(int productReworkId, int operatorId, int activityId)
		{
			var asModel = new ActivitySkillDataService(Context).FindOrAdd(operatorId, activityId);
			var pasRepository = new Repository<ProductActivitySkill>(Context);
			var model = pasRepository.FirstOrDefault(x =>
				x.ProductRework.Id == productReworkId
				&& x.ActivitySkill.Id == asModel.Id, "ActivitySkill.Operator", "ActivitySkill.Activity");
			if (model == null)
			{
				var prModel = new Repository<ProductRework>(Context).Single(x => x.Id == productReworkId);
				model = new ProductActivitySkill
				{
					ActivitySkill = asModel,
					ProductRework = prModel,
					CreatedDate = DateTime.Now,
					ModifiedDate = DateTime.Now,
					ModifiedBy = LoginInfo.Id,
					IluoNr = 0,
				};
				Context.Commit();
			}
			return model;
		}
    }
}