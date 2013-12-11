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
    public class GeneralSkillDataService : DataServiceBase, IDataService<GeneralSkill>
    {
        #region IDataService<GeneralSkill> Members

        public GeneralSkill GetSingle(int id)
        {
            GeneralSkill entity;
            using (var context = new SoheilEdmContext())
            {
                var generalSkillRepository = new Repository<GeneralSkill>(context);
                entity = generalSkillRepository.Single(generalSkill => generalSkill.Id == id);
            }
            return entity;
        }

        public ObservableCollection<GeneralSkill> GetAll()
        {
            ObservableCollection<GeneralSkill> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<GeneralSkill>(context);
                IEnumerable<GeneralSkill> entityList = repository.GetAll();
                models = new ObservableCollection<GeneralSkill>(entityList);
            }
            return models;
        }

        public ObservableCollection<GeneralSkill> GetActives()
        {
            return GetAll();
        }

        public int AddModel(GeneralSkill model)
        {
            //int id;
            //using (var context = new SoheilEdmContext())
            //{
            //    var repository = new Repository<GeneralSkill>(context);
            //    repository.Add(model);
            //    context.Commit();
            //    if (GeneralSkillAdded != null)
            //        GeneralSkillAdded(this, new ModelAddedEventArgs<GeneralSkill>(model));
            //    id = model.Id;
            //}
            //return id;
            return -1;
        }

        public void UpdateModel(GeneralSkill model)
        {
            using (var context = new SoheilEdmContext())
            {
                var generalSkillRepository = new Repository<GeneralSkill>(context);
                GeneralSkill entity = generalSkillRepository.Single(generalSkill => generalSkill.Id == model.Id);

                entity.Education = model.Education;
                entity.PhysicalState = model.PhysicalState;
                entity.Reserve1 = model.Reserve1;
                entity.Reserve2 = model.Reserve2;
                entity.Reserve3 = model.Reserve3;
                entity.ModifiedBy = LoginInfo.Id;
                context.Commit();
            }
        }

        public void DeleteModel(GeneralSkill model)
        {
        }

        public void AttachModel(GeneralSkill model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<GeneralSkill>(context);
                if (repository.Exists(generalSkill => generalSkill.Id == model.Id))
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

        public event EventHandler<ModelAddedEventArgs<GeneralSkill>> GeneralSkillAdded;
    }
}