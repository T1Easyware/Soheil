using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
    public class MachineFamilyDataService : DataServiceBase, IDataService<MachineFamily>
    {
		public event EventHandler<ModelAddedEventArgs<MachineFamily>> MachineFamilyAdded;
		Repository<MachineFamily> _machineFamilyRepository;
		Repository<Machine> _machineRepository;

		public MachineFamilyDataService()
			: this(new SoheilEdmContext())
		{
		}

		public MachineFamilyDataService(SoheilEdmContext context)
		{
			this.context = context;
			_machineFamilyRepository = new Repository<MachineFamily>(context);
			_machineRepository = new Repository<Machine>(context);
		}


        #region IDataService<MachineFamily> Members

        public MachineFamily GetSingle(int id)
        {
			return _machineFamilyRepository.FirstOrDefault(machineGroup => machineGroup.Id == id);
        }

		public ObservableCollection<MachineFamily> GetAll()
		{
			IEnumerable<MachineFamily> entityList = _machineFamilyRepository.Find(group => group.Status != (decimal)Status.Deleted, "Machines");
			return new ObservableCollection<MachineFamily>(entityList);
		}

		public ObservableCollection<MachineFamily> GetActives()
		{
			IEnumerable<MachineFamily> entityList = _machineFamilyRepository.Find(group => group.Status == (decimal)Status.Active, "Machines");
			return new ObservableCollection<MachineFamily>(entityList);
		}

		public int AddModel(MachineFamily model)
		{
			int id;
			_machineFamilyRepository.Add(model);
			context.Commit();
			if (MachineFamilyAdded != null)
				MachineFamilyAdded(this, new ModelAddedEventArgs<MachineFamily>(model));
			id = model.Id;
			return id;
		}

		public void UpdateModel(MachineFamily model)
		{
			MachineFamily entity = _machineFamilyRepository.Single(machineGroup => machineGroup.Id == model.Id);

			entity.Code = model.Code;
			entity.Name = model.Name;
			entity.CreatedDate = model.CreatedDate;
			entity.ModifiedBy = LoginInfo.Id;
			entity.ModifiedDate = DateTime.Now;
			context.Commit();
		}

        public void DeleteModel(MachineFamily model)
        {
        }

		public void AttachModel(MachineFamily model)
		{
			if (_machineFamilyRepository.Exists(machineGroup => machineGroup.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}
        #endregion

		public Machine GetMachine__(int id)/*???*/
		{
			return _machineRepository.Single(x => x.Id == id);
		}
    }
}