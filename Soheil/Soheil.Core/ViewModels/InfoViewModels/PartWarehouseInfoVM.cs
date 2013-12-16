using Soheil.Core.Base;
using Soheil.Model;

namespace Soheil.Core.ViewModels.InfoViewModels
{
    public class PartWarehouseInfoVM : ViewModelBase
    {
        private readonly PartWarehouse _model;
        public PartWarehouse Model{get{return _model;}}

        public int Id
        {
            get { return _model.Id;}

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
        public int Quantity
        {
            get { return _model.Quantity?? 0; }
            set { _model.Quantity = value; OnPropertyChanged("Quantity"); }
        }
        public int OriginalQuantity
        {
            get { return _model.OriginalQuantity?? 0;}
            set { _model.OriginalQuantity = value; OnPropertyChanged("OriginalQuantity"); }
        }

        public double TotalCost
        {
            get { return _model.TotalCost?? 0;}
            set { _model.TotalCost = value; OnPropertyChanged("TotalCost"); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StationInfoVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        public PartWarehouseInfoVM(PartWarehouse entity)
        {
            _model = entity;
        }

    }
}
