using Soheil.Core.Base;
using Soheil.Model;

namespace Soheil.Core.ViewModels.InfoViewModels
{
    public class StationInfoVM : ViewModelBase, IInfoViewModel
    {
        private readonly Station _model;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="StationInfoVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        public StationInfoVM(Station entity)
        {
            _model = entity;
        }
    }
}
