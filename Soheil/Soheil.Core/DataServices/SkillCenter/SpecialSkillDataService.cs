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
    public class SpecialSkillDataService : DataServiceBase, IDataService<GeneralActivitySkill>
    {
        #region IDataService<SpecialSkill> Members

        public GeneralActivitySkill GetSingle(int id)
        {
            GeneralActivitySkill entity;
            using (var context = new SoheilEdmContext())
            {
                var specialSkillRepository = new Repository<GeneralActivitySkill>(context);
                entity = specialSkillRepository.Single(specialSkill => specialSkill.Id == id);
            }
            return entity;
        }

        public ObservableCollection<GeneralActivitySkill> GetAll()
        {
            ObservableCollection<GeneralActivitySkill> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<GeneralActivitySkill>(context);
                IEnumerable<GeneralActivitySkill> entityList = repository.GetAll();
                models = new ObservableCollection<GeneralActivitySkill>(entityList);
            }
            return models;
        }

        public ObservableCollection<GeneralActivitySkill> GetActives()
        {
            return GetAll();
        }

        public int AddModel(GeneralActivitySkill model)
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

        public void UpdateModel(GeneralActivitySkill model)
        {
            using (var context = new SoheilEdmContext())
            {
                var specialSkillRepository = new Repository<GeneralActivitySkill>(context);
                GeneralActivitySkill entity = specialSkillRepository.Single(specialSkill => specialSkill.Id == model.Id);

               // entity.Reserve1 = model.Reserve1;
            //    entity.Reserve2 = model.Reserve2;
              //  entity.Reserve3 = model.Reserve3;
                entity.ModifiedBy = LoginInfo.Id;
                context.Commit();
            }
        }

        public void DeleteModel(GeneralActivitySkill model)
        {
        }

        public void AttachModel(GeneralActivitySkill model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<GeneralActivitySkill>(context);
                if (repository.Exists(specialSkill => specialSkill.Id == model.Id))
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

        public event EventHandler<ModelAddedEventArgs<GeneralActivitySkill>> SpecialSkillAdded;
    }
}