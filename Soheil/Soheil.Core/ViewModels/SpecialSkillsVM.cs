using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class SpecialSkillsVM : GridSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            var viewModels = new ObservableCollection<SpecialSkillVM>();
            foreach (var model in SpecialSkillDataService.GetAll())
            {
                viewModels.Add(new SpecialSkillVM(model, Access, SpecialSkillDataService));
            }
            Items = new ListCollectionView(viewModels);

            if (viewModels.Count > 0)
            {
                CurrentContent = (ISplitItemContent)Items.CurrentItem;
                CurrentContent.IsSelected = true;
            }
        }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public SpecialSkillDataService SpecialSkillDataService { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialSkillsVM"/> class.
        /// </summary>
        public SpecialSkillsVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            SpecialSkillDataService = new SpecialSkillDataService();
            SpecialSkillDataService.SpecialSkillAdded += OnSpecialSkillAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                //new ColumnInfo("Reserve1",0), 
                //new ColumnInfo("Reserve2",1),
                //new ColumnInfo("Reserve3",2),
                new ColumnInfo("Mode",0,true) 
            };

            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);
            CreateItems(null);
        }

        public override void Add(object parameter)
        {
            if (CurrentContent == null || CurrentContent is SpecialSkillVM)
            {
                SpecialSkillVM.CreateNew(SpecialSkillDataService);
            }
        }

        private void OnSpecialSkillAdded(object sender, ModelAddedEventArgs<GeneralActivitySkill> e)
        {
            var newSpecialSkillVm = new SpecialSkillVM(e.NewModel, Access, SpecialSkillDataService);
            Items.AddNewItem(newSpecialSkillVm);
            Items.CommitNew();
            CurrentContent = newSpecialSkillVm;
            CurrentContent.IsSelected = true;
        }

        #endregion

    }
}