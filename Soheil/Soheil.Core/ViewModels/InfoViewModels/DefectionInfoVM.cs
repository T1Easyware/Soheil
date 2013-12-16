using Soheil.Core.Base;
using Soheil.Model;

namespace Soheil.Core.ViewModels.InfoViewModels
{
    public class DefectionInfoVM : ViewModelBase, IInfoViewModel
    {
        private readonly Defection _model;

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
        /// Initializes a new instance of the <see cref="DefectionInfoVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        public DefectionInfoVM(Defection entity)
        {
            _model = entity;
        }
    }
}
