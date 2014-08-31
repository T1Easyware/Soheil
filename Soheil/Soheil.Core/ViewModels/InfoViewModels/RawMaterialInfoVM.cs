using System.Collections.ObjectModel;
using System.Windows;
using Soheil.Core.Base;
using Soheil.Model;

namespace Soheil.Core.ViewModels.InfoViewModels
{
    public class RawMaterialInfoVM : ViewModelBase, IInfoViewModel
    {
        private readonly RawMaterial _model;

        public int Id
        {
            get { return _model.Id; } 

        }
        public string Code
        {
            get { return _model.Code; }
            set { _model.Code = value; OnPropertyChanged("Code"); }
        }
        public string Name
        {
            get { return _model.Name; }
            set { _model.Name = value; OnPropertyChanged("Name"); }
        }

        public ObservableCollection<UnitSetInfoVM> UnitSets { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawMaterialInfoVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
		public RawMaterialInfoVM(RawMaterial entity)
		{
			_model = entity;
			UnitSets = new ObservableCollection<UnitSetInfoVM>();
			if (entity.UnitGroup != null)
				foreach (var unitSet in entity.UnitGroup.UnitSets)
				{
					UnitSets.Add(new UnitSetInfoVM(unitSet));
				}
		}
    }
}
