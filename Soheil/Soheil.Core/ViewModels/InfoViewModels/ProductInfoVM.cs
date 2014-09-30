using Soheil.Core.Base;
using Soheil.Model;

namespace Soheil.Core.ViewModels.InfoViewModels
{
    public class ProductInfoVM : ViewModelBase, IInfoViewModel
    {
        private Product _model;
        public Product Model
        {
            get { return _model; }
            set { _model = value; }
        }
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
        public string Text
        {
            get { return _model.Code + "-" + _model.Name; }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductInfoVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        public ProductInfoVM(Product entity)
        {
            _model = entity;
        }
    }
}
