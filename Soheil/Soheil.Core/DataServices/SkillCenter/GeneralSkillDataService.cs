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
    public class GeneralSkillDataService : DataServiceBase, IDataService<PersonalSkill>
    {
        #region IDataService<GeneralSkill> Members

        public PersonalSkill GetSingle(int id)
        {
            PersonalSkill entity;
            using (var context = new SoheilEdmContext())
            {
                var generalSkillRepository = new Repository<PersonalSkill>(context);
                entity = generalSkillRepository.Single(generalSkill => generalSkill.Id == id);
            }
            return entity;
        }

        public ObservableCollection<PersonalSkill> GetAll()
        {
            ObservableCollection<PersonalSkill> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<PersonalSkill>(context);
                IEnumerable<PersonalSkill> entityList = repository.GetAll();
                models = new ObservableCollection<PersonalSkill>(entityList);
            }
            return models;
        }

        public ObservableCollection<PersonalSkill> GetActives()
        {
            return GetAll();
        }

        public int AddModel(PersonalSkill model)
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

        public void UpdateModel(PersonalSkill model)
        {
            using (var context = new SoheilEdmContext())
            {
                var generalSkillRepository = new Repository<PersonalSkill>(context);
                PersonalSkill entity = generalSkillRepository.Single(generalSkill => generalSkill.Id == model.Id);

				entity.Operator = model.Operator;
				entity.Education = model.Education;
				entity.Experience = model.Experience;
				entity.ReserveInteger1 = model.ReserveInteger1;
				entity.ReserveText1 = model.ReserveText1;
				entity.ReserveText2 = model.ReserveText2;
                entity.ModifiedBy = LoginInfo.Id;
                context.Commit();
            }
        }

        public void DeleteModel(PersonalSkill model)
        {
        }

        public void AttachModel(PersonalSkill model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<PersonalSkill>(context);
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

        public event EventHandler<ModelAddedEventArgs<PersonalSkill>> GeneralSkillAdded;
    }
}