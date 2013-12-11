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
    public class SpecialSkillDataService : DataServiceBase, IDataService<SpecialSkill>
    {
        #region IDataService<SpecialSkill> Members

        public SpecialSkill GetSingle(int id)
        {
            SpecialSkill entity;
            using (var context = new SoheilEdmContext())
            {
                var specialSkillRepository = new Repository<SpecialSkill>(context);
                entity = specialSkillRepository.Single(specialSkill => specialSkill.Id == id);
            }
            return entity;
        }

        public ObservableCollection<SpecialSkill> GetAll()
        {
            ObservableCollection<SpecialSkill> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<SpecialSkill>(context);
                IEnumerable<SpecialSkill> entityList = repository.GetAll();
                models = new ObservableCollection<SpecialSkill>(entityList);
            }
            return models;
        }

        public ObservableCollection<SpecialSkill> GetActives()
        {
            return GetAll();
        }

        public int AddModel(SpecialSkill model)
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

        public void UpdateModel(SpecialSkill model)
        {
            using (var context = new SoheilEdmContext())
            {
                var specialSkillRepository = new Repository<SpecialSkill>(context);
                SpecialSkill entity = specialSkillRepository.Single(specialSkill => specialSkill.Id == model.Id);

                entity.Reserve1 = model.Reserve1;
                entity.Reserve2 = model.Reserve2;
                entity.Reserve3 = model.Reserve3;
                entity.ModifiedBy = LoginInfo.Id;
                context.Commit();
            }
        }

        public void DeleteModel(SpecialSkill model)
        {
        }

        public void AttachModel(SpecialSkill model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<SpecialSkill>(context);
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

        public event EventHandler<ModelAddedEventArgs<SpecialSkill>> SpecialSkillAdded;
    }
}