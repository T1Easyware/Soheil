using System;
using System.Windows;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class OrganizationChartVM : ItemContentViewModel
    {
        #region Properties

        private OrganizationChart _model;
        public override int Id
        {
            get { return _model.Id; } set{}
            
        }

        public override string SearchItem {get {return Name;} set{} }

// ReSharper disable PropertyNotResolved
        [LocalizedRequired(ErrorMessageResourceName = @"txtNameRequired")]
// ReSharper restore PropertyNotResolved
        public string Name
        {
            get { return _model.Name; }
            set { _model.Name = value; OnPropertyChanged("Name"); }
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public OrganizationChartDataService OrganizationChartDataService { get; set; }

        public DateTime ModifiedDate
        {
            get { return _model.ModifiedDate; }
            set { _model.ModifiedDate = value; OnPropertyChanged("ModifiedDate"); }
        }

        public string ModifiedBy
        {
            get { return LoginInfo.GetUsername(_model.ModifiedBy); }
            
        }

        public Status Status
        {
            get { return (Status) _model.Status; }
            set { _model.Status = (byte)value; }
        }

        public static readonly DependencyProperty PositionsVMProperty =
            DependencyProperty.Register("PositionsVM", typeof(OrganizationChartPositionsVM), typeof(OrganizationChartVM), null);

        public OrganizationChartPositionsVM PositionsVM
        {
            get { return (OrganizationChartPositionsVM)GetValue(PositionsVMProperty); }
            set { SetValue(PositionsVMProperty, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public OrganizationChartVM(AccessType access, OrganizationChartDataService dataService):base(access)
        {
            InitializeData(dataService);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public OrganizationChartVM(OrganizationChart entity, AccessType access, OrganizationChartDataService dataService)
            : base(access)
        {
            InitializeData(dataService);
            _model = entity;
            
            {
                PositionsVM = new OrganizationChartPositionsVM(this, Access);
            }
        }


        private void InitializeData(OrganizationChartDataService dataService)
        {
            OrganizationChartDataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }

        public override void Save(object param)
        {
            OrganizationChartDataService.AttachModel(_model);
            _model = OrganizationChartDataService.GetSingle(_model.Id); OnPropertyChanged("ModifiedBy");OnPropertyChanged("ModifiedDate");Mode = ModificationStatus.Saved;
        }
        public override void Delete(object param)
        {
            _model.Status = (byte)Status.Deleted; OrganizationChartDataService.AttachModel(_model);
        }
        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }
        public override void ViewItemLink(object param)
        {
            var relationIndex = Convert.ToInt32(param);
            switch (relationIndex)
            {
                case 0:
                    PositionsVM = new OrganizationChartPositionsVM(this, Access);
                    CurrentLink = PositionsVM;
                    break;
                case 1:
                    break;
            }
            base.ViewItemLink(param);
        }
        #endregion

        #region Static Methods
        public static OrganizationChart CreateNew(OrganizationChartDataService dataService)
        {
            int id = dataService.AddModel(new OrganizationChart { Name = "جدید", ModifiedDate = DateTime.Now, Status = (byte) Status.Active});
            return dataService.GetSingle(id);
        }
        #endregion
    }
}