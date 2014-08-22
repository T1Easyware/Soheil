using System;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.Base;

namespace Soheil.Core.DataServices
{
    public class UnitConversionDataService : DataServiceBase, IDataService<UnitConversion>
    {
		public event EventHandler<ModelAddedEventArgs<UnitConversion>> UnitConversionAdded;
        readonly Repository<UnitConversion> _unitConversionRepository;

		public UnitConversionDataService()
			: this(new SoheilEdmContext())
		{
		}

		public UnitConversionDataService(SoheilEdmContext context)
		{
			Context = context;
			_unitConversionRepository = new Repository<UnitConversion>(context);
		}



		#region IDataService<UnitConversionVM> Members

        public UnitConversion GetSingle(int id)
        {
			return _unitConversionRepository.Single(unitConversion => unitConversion.Id == id);
        }

		public ObservableCollection<UnitConversion> GetAll()
		{
            var entityList = _unitConversionRepository.Find(activity => activity.Status != (decimal)Status.Deleted );
            return new ObservableCollection<UnitConversion>(entityList);
		}

		/// <summary>
		/// Gets all active UnitConversion models.
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<UnitConversion> GetActives()
		{
            var entityList = _unitConversionRepository.Find(activity => activity.Status == (byte)Status.Active);
            return new ObservableCollection<UnitConversion>(entityList);
		}

		public int AddModel(UnitConversion model)
		{
		    _unitConversionRepository.Add(model);
			Context.Commit();
			if (UnitConversionAdded != null)
				UnitConversionAdded(this, new ModelAddedEventArgs<UnitConversion>(model));
			int id = model.Id;
			return id;
		}

		public void UpdateModel(UnitConversion model)
		{
            model.ModifiedBy = LoginInfo.Id;
            Context.Commit();
		}

        public void DeleteModel(UnitConversion model)
        {
        }

		public void AttachModel(UnitConversion model)
		{
			if (_unitConversionRepository.Exists(unitConversion => unitConversion.Id == model.Id))
			{
				UpdateModel(model);
			}
			else
			{
				AddModel(model);
			}
		}

        #endregion
        
	}
}