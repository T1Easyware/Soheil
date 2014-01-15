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
    public class GeneralSkillsVM : GridSplitViewModel
    {
        #region Properties
        public override void CreateItems(object param)
        {
            var viewModels = new ObservableCollection<GeneralSkillVM>();
            foreach (var model in GeneralSkillDataService.GetAll())
            {
                viewModels.Add(new GeneralSkillVM(model, Access, GeneralSkillDataService));
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
        public GeneralSkillDataService GeneralSkillDataService { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralSkillsVM"/> class.
        /// </summary>
        public GeneralSkillsVM(AccessType access):base(access)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            GeneralSkillDataService = new GeneralSkillDataService();
            GeneralSkillDataService.GeneralSkillAdded += OnGeneralSkillAdded;

            ColumnHeaders = new List<ColumnInfo> 
            { 
                new ColumnInfo("Education"), 
                new ColumnInfo("Reserve1"), 
                new ColumnInfo("Reserve2") ,
                new ColumnInfo("Mode",true) 
            };

            AddCommand = new Command(Add, CanAdd);RefreshCommand = new Command(CreateItems);
            AddGroupCommand = new Command(Add, CanAddGroup);
            CreateItems(null);

        }

        public override void Add(object parameter)
        {
            if (CurrentContent == null || CurrentContent is GeneralSkillVM)
            {
                GeneralSkillVM.CreateNew(GeneralSkillDataService);
            }
        }

        private void OnGeneralSkillAdded(object sender, ModelAddedEventArgs<PersonalSkill> e)
        {
            var newGeneralSkillVm = new GeneralSkillVM(e.NewModel, Access, GeneralSkillDataService);
            Items.AddNewItem(newGeneralSkillVm);
            Items.CommitNew();
            CurrentContent = newGeneralSkillVm;
            CurrentContent.IsSelected = true;
        }

        #endregion


    }
}