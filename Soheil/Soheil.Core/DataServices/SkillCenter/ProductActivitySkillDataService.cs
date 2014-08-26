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
		Repository<ProductActivitySkill> _productActivitySkillRepository;

		public event EventHandler<ModelAddedEventArgs<ProductActivitySkill>> SpecialSkillAdded;

		public ProductActivitySkillDataService()
			:this(new SoheilEdmContext())
		{

		}
		public ProductActivitySkillDataService(SoheilEdmContext context)
		{
			this.Context = context;
			_productActivitySkillRepository = new Repository<ProductActivitySkill>(Context);
		}

        #region IDataService<SpecialSkill> Members

		public ProductActivitySkill GetSingle(int id)
		{
			ProductActivitySkill entity;

			entity = _productActivitySkillRepository.Single(specialSkill => specialSkill.Id == id);

			return entity;
		}

		public ObservableCollection<ProductActivitySkill> GetAll()
		{
			ObservableCollection<ProductActivitySkill> models;

			IEnumerable<ProductActivitySkill> entityList = _productActivitySkillRepository.GetAll();
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
			ProductActivitySkill entity = _productActivitySkillRepository.Single(specialSkill => specialSkill.Id == model.Id);

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
			if (_productActivitySkillRepository.Exists(specialSkill => specialSkill.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

        #endregion


		public ProductActivitySkill TryFind(int productReworkId, int operatorId, int activityId)
		{
			var model = _productActivitySkillRepository.FirstOrDefault(x =>
				x.ProductRework.Id == productReworkId
				&& x.ActivitySkill.Activity.Id == activityId
				&& x.ActivitySkill.Operator.Id == operatorId);
			return model;
		}

		internal void AddOrUpdateSkill(ViewModels.SkillCenter.ProductReworkActivitySkillVm vm)
		{
			if (vm.Model == null)
			{
				vm.Model = _productActivitySkillRepository.FirstOrDefault(x =>
					x.ProductRework.Id == vm.ProductReworkId
					&& x.ActivitySkill.Operator.Id == vm.OperatorId
					&& x.ActivitySkill.Activity.Id == vm.ActivityId);
				if (vm.Model == null)
				{
					var prModel = new Repository<ProductRework>(Context).Single(x => x.Id == vm.ProductReworkId);
					var asModel = new Repository<ActivitySkill>(Context).Single(x => x.Operator.Id == vm.OperatorId && x.Activity.Id == vm.ActivityId);
					
					//Create ActivitySkill
					if (asModel == null)
					{
						var actv = new Repository<Activity>(Context).Single(x => x.Id == vm.ActivityId);
						var oper = new Repository<Operator>(Context).Single(x => x.Id == vm.OperatorId);
						asModel = new ActivitySkill
						{
							Activity = actv,
							Operator = oper,
							IluoNr = (byte)Soheil.Common.ILUO.NA,
							CreatedDate = DateTime.Now,
							ModifiedDate = DateTime.Now,
							ModifiedBy = LoginInfo.Id
						};
					}

					//Create ProductActivitySkill
					vm.Model = new ProductActivitySkill
					{
						ActivitySkill = asModel,
						ProductRework = prModel,
						CreatedDate = DateTime.Now,
						ModifiedDate = DateTime.Now,
						ModifiedBy = LoginInfo.Id,
					};
				}

				vm.Model.IluoNr = (byte)vm.Data;
				Context.Commit();
			}
		}
	}
}