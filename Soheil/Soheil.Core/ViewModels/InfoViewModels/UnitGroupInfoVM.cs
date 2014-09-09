using System.Collections.ObjectModel;
using Soheil.Core.Base;
using Soheil.Model;

namespace Soheil.Core.ViewModels.InfoViewModels
{
    public class UnitGroupInfoVM : ViewModelBase, IInfoViewModel
    {
        private UnitGroup _model;
        public UnitGroup Model
        {
            get { return _model; }
            set { _model = value; }
        }
        public int Id
        {
            get { return _model.Id; } 

        }
        public string Code { get; set; }
        public string Name
        {
            get { return _model.Name; }
            set { _model.Name = value; OnPropertyChanged("Name"); }
        }

        public ObservableCollection<UnitSetInfoVM> UnitSets { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitSetInfoVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        public UnitGroupInfoVM(UnitGroup entity)
        {
            _model = entity;
            UnitSets = new ObservableCollection<UnitSetInfoVM>();
                foreach (var unitSet in entity.UnitSets)
                {
                    UnitSets.Add(new UnitSetInfoVM(unitSet));
                }
        }
    }
}
