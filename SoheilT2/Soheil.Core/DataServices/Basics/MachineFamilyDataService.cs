using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class MachineFamilyDataService : IDataService<MachineFamily>
    {
        #region IDataService<MachineFamily> Members

        public MachineFamily GetSingle(int id)
        {
            MachineFamily entity;
            using (var context = new SoheilEdmContext())
            {
                var machineGroupRepository = new Repository<MachineFamily>(context);
                entity = machineGroupRepository.FirstOrDefault(machineGroup => machineGroup.Id == id);
            }
            return entity;
        }

        public ObservableCollection<MachineFamily> GetAll()
        {
            ObservableCollection<MachineFamily> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<MachineFamily>(context);
                IEnumerable<MachineFamily> entityList = repository.Find(group=> group.Status != (decimal)Status.Deleted,"Machines");
                models = new ObservableCollection<MachineFamily>(entityList);
            }
            return models;
        }

        public ObservableCollection<MachineFamily> GetActives()
        {
            ObservableCollection<MachineFamily> models;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<MachineFamily>(context);
                IEnumerable<MachineFamily> entityList = repository.Find(group => group.Status == (decimal)Status.Active, "Machines");
                models = new ObservableCollection<MachineFamily>(entityList);
            }
            return models;
        }

        public int AddModel(MachineFamily model)
        {
            int id;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<MachineFamily>(context);
                repository.Add(model);
                context.Commit();
                if (MachineFamilyAdded != null)
                    MachineFamilyAdded(this, new ModelAddedEventArgs<MachineFamily>(model));
                id = model.Id;
            }
            return id;
        }

        public void UpdateModel(MachineFamily model)
        {
            using (var context = new SoheilEdmContext())
            {
                var machineGroupRepository = new Repository<MachineFamily>(context);
                MachineFamily entity = machineGroupRepository.Single(machineGroup => machineGroup.Id == model.Id);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.CreatedDate = model.CreatedDate;
                entity.ModifiedBy = LoginInfo.Id;
                entity.ModifiedDate = DateTime.Now;
                context.Commit();
            }
        }

        public void DeleteModel(MachineFamily model)
        {
        }

        public void AttachModel(MachineFamily model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<MachineFamily>(context);
                if (repository.Exists(machineGroup => machineGroup.Id == model.Id))
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

        public event EventHandler<ModelAddedEventArgs<MachineFamily>> MachineFamilyAdded;

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <param name="id">The machine group id.</param>
        /// <returns></returns>
        public MachineFamily GetModel(int id)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<MachineFamily>(context);
                return repository.Single(machine => machine.Id == id);
            }
        }
    }
}