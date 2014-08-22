using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices.PM
{
	public class PartDataService : DataServiceBase, IDataService<Part>
    {
		readonly Repository<MachinePart> _machinePartRepository;
		readonly Repository<Part> _partRepository;
		readonly Repository<Machine> _machineRepository;
	    readonly Repository<MachineFamily> _machineFamilyRepository;

		public PartDataService()
			: this(new SoheilEdmContext())
		{

		}
		public PartDataService(SoheilEdmContext context)
		{
			Context = context ?? new SoheilEdmContext();
			_machinePartRepository = new Repository<MachinePart>(Context);
			_partRepository = new Repository<Part>(Context);
			_machineRepository = new Repository<Machine>(Context);
            _machineFamilyRepository = new Repository<MachineFamily>(Context);
		}
	
        #region IDataService<Machine> Members

		public Part GetSingle(int id)
        {
			return _partRepository.FirstOrDefault(x => x.Id == id);
        }

		public ObservableCollection<Part> GetAll()
        {
			IEnumerable<Part> entityList = _partRepository.Find(x => x.Status != (decimal)Status.Deleted);
			return new ObservableCollection<Part>(entityList);
        }

		public int AddModel(Part model)
		{
			int id;
			_partRepository.Add(model);
			Context.Commit();
			id = model.Id;
			return id;
		}

		public void UpdateModel(Part model)
        {
			model.ModifiedBy = LoginInfo.Id;
			model.ModifiedDate = DateTime.Now;

			Context.Commit();
        }

		/// <summary>
		/// Changes the status or physically delete the model
		/// </summary>
		/// <param name="model"></param>
        public void DeleteModel(Part model)
        {
			if (model.MachineParts.Any())
			{
				model.Status = (byte)Status.Deleted;
				model.ModifiedDate = DateTime.Now;
			}
			else
			{
				_partRepository.Delete(model);
			}
			Context.Commit();
        }

		public void AttachModel(Part model)
		{
			if (_partRepository.Exists(mp => mp.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
			Context.Commit();
        }

        #endregion

        /// <summary>
        /// Gets all active machines as view models.
        /// </summary>
        /// <returns></returns>
		public ObservableCollection<Part> GetActives()
		{
			var entityList = _partRepository.Find(x => x.Status == (decimal)Status.Active);
			return new ObservableCollection<Part>(entityList);
		}
    }
}