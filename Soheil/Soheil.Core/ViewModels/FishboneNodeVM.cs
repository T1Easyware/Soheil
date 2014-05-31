using System;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class FishboneNodeVM : NodeRelationDetailViewModel
    {
        private readonly FishboneNode _model;
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupVM"/> class initialized with default values.
        /// </summary>
        public FishboneNodeVM(AccessType access, FishboneNodeDataService dataService):base(access)
        {
            InitializeData(dataService);
            _model = new FishboneNode();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVM"/> class from the model.
        /// </summary>
        /// <param name="entity">The model.</param>
        /// <param name="access"></param>
        /// <param name="dataService"></param>
        public FishboneNodeVM(FishboneNode entity, AccessType access, FishboneNodeDataService dataService)
            : base(access)
        {
            _model = entity;
            InitializeData(dataService);
            RootId = entity.Root.Id;
            ParentId = entity.Parent == null ? -1 : entity.Parent.Id;
            Title = entity.Description;
            foreach (var child in entity.Children)
            {
                ChildNodes.Add(new FishboneNodeVM(child, access, DataService));
            }
        }

        private void InitializeData(FishboneNodeDataService dataService)
        {
            DataService = dataService;
            SaveCommand = new Command(Save, CanSave);
        }

        /// <summary>
        /// Gets or sets the activity-operator data service.
        /// </summary>
        /// <value>
        /// The activity-operator data service.
        /// </value>
        public FishboneNodeDataService DataService { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public override int Id
        {
            get { return _model.Id; } 
            set { _model.Id = value; }
        }

        public FishboneNodeType NodeType
        {
            get { return (FishboneNodeType) _model.Type; }
            set { _model.Type = (byte) value; OnPropertyChanged("NodeType");}
        }

        public FishboneNodeType RootType
        {
            get { return (FishboneNodeType)_model.RootType; }
            set { _model.RootType = (byte)value; OnPropertyChanged("RootType"); }
        }

        public string Description
        {
            get { return _model.Description; }
            set { _model.Description = value; OnPropertyChanged("Description"); }
        }

        public string ModifiedBy
        {
            get { return LoginInfo.GetUsername(_model.ModifiedBy); }
        }

        public DateTime ModifiedDate
        {
            get { return _model.ModifiedDate; }
            set { _model.ModifiedDate = value; OnPropertyChanged("ModifiedDate"); }
        }

        public int RootId { get; set; }

        public override void Save(object param)
        {
            Description = Title;
            DataService.UpdateModel(_model);
            OnPropertyChanged("NodeType");
        }

        public override bool CanSave()
        {
            return AllDataValid() && base.CanSave();
        }


    }
}