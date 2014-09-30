using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels;

namespace Soheil.Views
{
    /// <summary>
    /// Interaction logic for SoheilSplitView.xaml
    /// </summary>
    public partial class SoheilSplitView : INotifyPropertyChanged
    {
        public SoheilSplitView(ISplitList viewModel, List<Tuple<string, AccessType>> accessList)
        {
            InitializeComponent();
            ViewModel = viewModel;
            AccessList = accessList;
            RefreshViewModelTemplate();
            ContentUpdated(null, null);
        }

        private ISplitList _viewMode;
        public ISplitList ViewModel
        {
            get
            {
                return _viewMode;
            }
            set
            {
                _viewMode = value;
                OnPropertyChanged("ViewModel");
            }
        }

        public List<Tuple<string, AccessType>> AccessList { get; set; }
        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ContentUpdated(object sender, DataTransferEventArgs e)
        {
            if (ViewModel == null) return;

            if (ViewModel.CurrentContent == null)
            {
                var tmpl = (DataTemplate)FindResource("EmptyContentTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }

            else if (ViewModel.CurrentContent is AccessRuleVM)
            {
                var tmpl = (DataTemplate)FindResource("AccessRuleTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
			else if (ViewModel.CurrentContent is Soheil.Core.ViewModels.OrganizationCalendar.WorkProfileVm)
			{
				var tmpl = (DataTemplate)FindResource("WorkProfileTemplate");
				_itemContentView.ContentTemplate = tmpl;
			}
			else if (ViewModel.CurrentContent is Soheil.Core.ViewModels.OrganizationCalendar.HolidayVm)
			{
				var tmpl = (DataTemplate)FindResource("HolidayTemplate");
				_itemContentView.ContentTemplate = tmpl;
			}
			else if (ViewModel.CurrentContent is Soheil.Core.ViewModels.OrganizationCalendar.WorkProfilePlanVm)
			{
				var tmpl = (DataTemplate)FindResource("WorkProfilePlanTemplate");
				_itemContentView.ContentTemplate = tmpl;
			}
			else if (ViewModel.CurrentContent is CauseVM)
            {
                var tmpl = (DataTemplate)FindResource("CauseTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is ProductGroupVM)
            {
                var tmpl = (DataTemplate)FindResource("ProductGroupTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is ProductVM)
            {
                var tmpl = (DataTemplate)FindResource("ProductTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is ReworkVM)
            {
                var tmpl = (DataTemplate)FindResource("ReworkTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is StationVM)
            {
                var tmpl = (DataTemplate)FindResource("StationTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is MachineVM)
            {
                var tmpl = (DataTemplate)FindResource("MachineTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is MachineFamilyVM)
            {
                var tmpl = (DataTemplate)FindResource("MachineFamilyTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is ActivityVM)
            {
                var tmpl = (DataTemplate)FindResource("ActivityTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is ActivityGroupVM)
            {
                var tmpl = (DataTemplate)FindResource("ActivityGroupTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is DefectionVM)
            {
                var tmpl = (DataTemplate)FindResource("DefectionTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is GeneralSkillVM)
            {
                var tmpl = (DataTemplate)FindResource("GeneralSkillTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is SpecialSkillVM)
            {
                var tmpl = (DataTemplate)FindResource("SpecialSkillTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }

            else if (ViewModel.CurrentContent is OperatorVM)
            {
                var tmpl = (DataTemplate)FindResource("OperatorTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }

            else if (ViewModel.CurrentContent is UserVM)
            {
                var tmpl = (DataTemplate)FindResource("UserTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }

            else if (ViewModel.CurrentContent is PositionVM)
            {
                var tmpl = (DataTemplate)FindResource("PositionTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is OrganizationChartVM)
            {
                var tmpl = (DataTemplate)FindResource("OrganizationChartTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is FpcVm)
            {
                var tmpl = (DataTemplate)FindResource("FpcTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is CostVM)
            {
                var tmpl = (DataTemplate)FindResource("CostTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is CostCenterVM)
            {
                var tmpl = (DataTemplate)FindResource("CostCenterTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is PartWarehouseVM)
            {
                var tmpl = (DataTemplate)FindResource("PartWarehouseTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is PartWarehouseGroupVM)
            {
                var tmpl = (DataTemplate)FindResource("PartWarehouseGroupTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is RootVM)
            {
                var tmpl = (DataTemplate)FindResource("RootTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is ActionPlanVM)
            {
                var tmpl = (DataTemplate)FindResource("ActionPlanTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is WarehouseVM)
            {
                var tmpl = (DataTemplate)FindResource("WarehouseTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is WarehouseReceiptVM)
            {
                var content = (WarehouseReceiptVM) ViewModel.CurrentContent;
                if (content.Type == WarehouseReceiptType.Storage)
                {
                    var tmpl = (DataTemplate)FindResource("WarehouseStorageTemplate");
                    _itemContentView.ContentTemplate = tmpl;
                }
                else if (content.Type == WarehouseReceiptType.Discharge)
                {
                    var tmpl = (DataTemplate)FindResource("WarehouseDischargeTemplate");
                    _itemContentView.ContentTemplate = tmpl;
                }
                else
                {
                    var tmpl = (DataTemplate)FindResource("WarehouseTransferTemplate");
                    _itemContentView.ContentTemplate = tmpl;
                }
            }
            else if (ViewModel.CurrentContent is RawMaterialVM)
            {
                var tmpl = (DataTemplate)FindResource("RawMaterialTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is UnitSetVM)
            {
                var tmpl = (DataTemplate)FindResource("UnitSetTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }
            else if (ViewModel.CurrentContent is UnitGroupVM)
            {
                var tmpl = (DataTemplate)FindResource("UnitGroupTemplate");
                _itemContentView.ContentTemplate = tmpl;
            }

        }

        private void ListUpdated(object sender, RoutedEventArgs e)
        {
            string code = Convert.ToString(((Control)e.Source).Tag);
            var accessItem = AccessList.FirstOrDefault(item => item.Item1 == code);
            var access = accessItem == null ? AccessType.None : accessItem.Item2;
            var type = ((SoheilEntityType)Convert.ToInt32(((Control)sender).Tag));
			//!@#$
			switch (type)
            {
                case SoheilEntityType.None:
                    break;
                case SoheilEntityType.UsersMenu:
                    break;
                case SoheilEntityType.UserAccessSubMenu:
                    break;
                case SoheilEntityType.Users:
                    ViewModel = new UsersVM(access);
                    break;
                case SoheilEntityType.Positions:
                    ViewModel = new PositionsVM(access);
                    break;
                case SoheilEntityType.OrganizationCharts:
                    ViewModel = new OrganizationChartsVM(access);
                    break;
                case SoheilEntityType.ModulesSubMenu:
                    break;
                case SoheilEntityType.Modules:
                    ViewModel = new AccessRulesVM(access);
                    break;
				case SoheilEntityType.WorkProfiles:
					ViewModel = new WorkProfilesVM(access);
					break;
				case SoheilEntityType.Holidays:
					ViewModel = new HolidaysVM(access);
					break;
				case SoheilEntityType.WorkProfilePlan:
					ViewModel = new WorkProfilePlansVM(access);
					break;
                case SoheilEntityType.DefinitionsMenu:
                    break;
                case SoheilEntityType.ProductsSubMenu:
                    break;
                case SoheilEntityType.Products:
                    ViewModel = new ProductsVM(access);
                    break;
                case SoheilEntityType.Reworks:
                    ViewModel = new ReworksVM(access);
                    break;
                case SoheilEntityType.DiagnosisSubMenu:
                    break;
                case SoheilEntityType.Defections:
                    ViewModel = new DefectionsVM(access);
                    break;
                case SoheilEntityType.Roots:
                    ViewModel = new RootsVM(access);
                    break;
                case SoheilEntityType.ActionPlans:
                    ViewModel = new ActionPlansVM(access);
                    break;
                case SoheilEntityType.Causes:
                    ViewModel = new CausesVM(access);
                    break;
                case SoheilEntityType.FpcSubMenu:
                    break;
				//case SoheilEntityType.Fpc:
				//	ViewModel = new FpcsVm(access);
				//	break;
                case SoheilEntityType.Stations:
                    ViewModel = new StationsVM(access);
                    break;
                case SoheilEntityType.Machines:
                    ViewModel = new MachinesVM(access);
                    break;
                case SoheilEntityType.Activities:
                    ViewModel = new ActivitiesVM(access);
                    break;
                case SoheilEntityType.OperatorsSubMenu:
                    break;
                case SoheilEntityType.Operators:
                    ViewModel = new OperatorsVM(access);
                    break;
                case SoheilEntityType.GeneralSkills:
                    ViewModel = new GeneralSkillsVM(access);
                    break;
                case SoheilEntityType.SpecialSkills:
                    ViewModel = new SpecialSkillsVM(access);
                    break;
                case SoheilEntityType.CostsSubMenu:
                    break;
                case SoheilEntityType.Costs:
                    ViewModel = new CostsVM(access);
                    break;
                case SoheilEntityType.Warehouses:
                    ViewModel = new WarehousesVM(access);
                    break;
                case SoheilEntityType.WarehouseReceiptSubMenu:
                    break;
                case SoheilEntityType.WarehouseStorageReceipt:
                    ViewModel = new WarehouseReceiptsVM(access, WarehouseReceiptType.Storage, WarehouseTransactionType.RawMaterial);
                    break;
                case SoheilEntityType.WarehouseDischargeReceipt:
                    ViewModel = new WarehouseReceiptsVM(access, WarehouseReceiptType.Discharge, WarehouseTransactionType.Product);
                    break;
                case SoheilEntityType.RawMaterialSubMenu:
                    ViewModel = new RawMaterialsVM(access);
                    break;
                case SoheilEntityType.UnitSets:
                    ViewModel = new UnitSetsVM(access);
                    break;
                case SoheilEntityType.ControlMenu:
                    break;
                case SoheilEntityType.ProductPlanSubMenu:
                    break;
                case SoheilEntityType.PerformanceSubMenu:
                    break;
                case SoheilEntityType.IndicesSubMenu:
                    break;
                case SoheilEntityType.OptionsMenu:
                    break;
                case SoheilEntityType.SettingsSubMenu:
                    break;
                case SoheilEntityType.HelpSubMenu:
                    break;
                case SoheilEntityType.AboutSubMenu:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            RefreshViewModelTemplate();
        }
        private void RefreshViewModelTemplate()
        {
            if (ViewModel is ITreeSplitList)
            {
                var tmpl = (DataTemplate)FindResource("TreeViewListTemplate");
                _listPanel.ContentTemplate = tmpl;
            }
            else
            {
                var tmpl = (DataTemplate)FindResource("DataGridListTemplate");
                _listPanel.ContentTemplate = tmpl;
            }
            OnPropertyChanged("ViewModel"); 
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (ViewModel == null)
                return;

            if (ViewModel.ColumnHeaders == null)
                return;

            var column = ViewModel.ColumnHeaders.FirstOrDefault(col => col.Name == e.Column.Header.ToString());
            if (column == null)
            {
                e.Cancel = true;
            }
            else if (column.Name == "Color")
            {
                e.Column.IsReadOnly = column.ReadOnly;
                e.Column.Width = 50;
                var templateColumn = new DataGridTemplateColumn
                    {
                        //Header = Common.Properties.Resources.ResourceManager.GetString(column.Header),
                        Header = column.Name,
                        CellTemplate = (DataTemplate)Resources["ColorCellTemplate"]
                    };
                e.Column = templateColumn;
            }
            else
            {
                //e.Column.Header = Common.Properties.Resources.ResourceManager.GetString(column.Header);
                e.Column.Header = column.Name;
                e.Column.IsReadOnly = column.ReadOnly;
                if (column.Name == "Mode")
                {
                    e.Column.Width = 100;
                }
            }
        }
        private void OnAutoGeneratedColumns(object sender, EventArgs e)
        {
            if (ViewModel.ColumnHeaders == null)
            {
                return;
            }
            var grid = (DataGrid)sender;
            foreach (var item in grid.Columns)
            {
                var headerInfo = ViewModel.ColumnHeaders.FirstOrDefault(info => info.Name == item.Header.ToString());
                if (headerInfo == null) return;
                item.Header = Common.Properties.Resources.ResourceManager.GetString(headerInfo.Header);
                item.DisplayIndex = headerInfo.Index;
            }
        }

        private bool _cellEditingMode;
        private bool _isSaving;
        private ISplitItemContent _currentContent;

        public static readonly DependencyProperty CurrentIndexProperty =
            DependencyProperty.Register("CurrentIndex", typeof (int), typeof (SoheilSplitView), new PropertyMetadata(default(int)));

        public int CurrentIndex
        {
            get { return (int) GetValue(CurrentIndexProperty); }
            set { SetValue(CurrentIndexProperty, value); }
        }

        void GridPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var grid = (DataGrid)sender;
            _currentContent = (ISplitItemContent)grid.CurrentItem;
            if (e.Key == Key.F2)
            {
                if (_cellEditingMode)
                {
                    grid.CommitEdit(DataGridEditingUnit.Cell, true);
                    _cellEditingMode = false;
                }
                else
                {
                    grid.BeginEdit();
                    _cellEditingMode = true;
                }
            }
            else if (!_cellEditingMode && e.Key == Key.Left && Equals(grid.CurrentCell.Column, grid.Columns[grid.Columns.Count - 1]))
            {
                e.Handled = true;
            }
            else if (!_cellEditingMode && e.Key == Key.Right && Equals(grid.CurrentCell.Column, grid.Columns[0]))
            {
                e.Handled = true;
            }
            else if (!_cellEditingMode && e.Key == Key.Up && Equals(grid.SelectedIndex, 0))
            {
                e.Handled = true;
            }
            else if (!_cellEditingMode && e.Key == Key.Down && Equals(grid.SelectedIndex, grid.Items.Count - 1))
            {
                e.Handled = true;
            }
            else if (_cellEditingMode && e.Key == Key.Left || e.Key == Key.Right)
            {
                 grid.CommitEdit(DataGridEditingUnit.Cell, true);
                _cellEditingMode = false;
            }
            else if (e.Key == Key.Enter)
            {
                grid.CommitEdit(DataGridEditingUnit.Row, true);

                ViewModel.CurrentContent = (ISplitContent) grid.CurrentItem;
                _currentContent = (ISplitItemContent)ViewModel.CurrentContent;


                if (_currentContent.CanSave()) 
                    _currentContent.Save(null);
                else
                    _currentContent.Mode = ModificationStatus.Unsaved;

                if (_currentContent.SelectedGroupVM != null)
                {
                    if (grid.SelectedIndex == grid.Items.Count - 1)
                    {
                        ViewModel.Add(_currentContent.SelectedGroupVM);
                    }
                    else
                    {
                        var nextItem = (ISplitItemContent)grid.Items[grid.SelectedIndex + 1];
                        if (nextItem.SelectedGroupVM.Id != _currentContent.SelectedGroupVM.Id)
                        {
                            ViewModel.Add(_currentContent.SelectedGroupVM);
                        }
                    }
                }
                else if (grid.SelectedIndex == grid.Items.Count - 1)
                {
                    ViewModel.Add(null); 
                }
                _isSaving = true;
            }
            else if (!_cellEditingMode)
            {
                if (e.Key == Key.D && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                {
                    ViewModel.CreateClone(_currentContent);
                }
                else if (e.Key == Key.Delete)
                {
                    if (ViewModel.CurrentContent.IsDeleting)
                    {
                        ((IEntityObject)ViewModel.Items.CurrentItem).Delete(null);
                    }
                    else
                    {
                        e.Handled = true;
                        ViewModel.CurrentContent.IsDeleting = true;
                    }
                }
                else if (e.Key == Key.Escape)
                {
                    ViewModel.CurrentContent.IsDeleting = false;
                }
            }
        }

        private void TreePreviewKeyDown(object sender, KeyEventArgs e)
        {
            var tree = (TreeView)sender;
            _currentContent = (ISplitNodeContent)tree.SelectedItem;
            var treeVm = (ITreeSplitList) ViewModel;
            var root = treeVm.RootNode;
            if (e.Key == Key.Delete)
            {
                if (ViewModel.CurrentContent.IsDeleting)
                {
                    ((IEntityObject)ViewModel.CurrentContent).Delete(null);
                    TreeSplitViewModel.Remove(_currentContent.Id,root);
                }
                else
                {
                    e.Handled = true;
                    ViewModel.CurrentContent.IsDeleting = true;
                }
            }
            else if (e.Key == Key.Escape)
            {
                ViewModel.CurrentContent.IsDeleting = false;
            }
        }

        private void DataGridOnPreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            _cellEditingMode = true;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e != null && e.RemovedItems.Count > 0)
            {
                var item = e.RemovedItems[0] as ISplitContent;
                if (item != null)
                    item.IsDeleting = false;
            }

            if (!_isSaving)
                return;

            _isSaving = false;
            var grid = (DataGrid)sender;
            grid.Focus();
            if(CurrentIndex > 0)
                grid.CurrentCell = new DataGridCellInfo(grid.Items[CurrentIndex],grid.Columns[0]);
        }

		private void skillCenterLoaded(object sender, RoutedEventArgs e)
		{

		}

        private void OnTreeLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var tree = (TreeView)sender;
            _currentContent = (ISplitNodeContent)tree.SelectedItem;
            if (_currentContent == null) return;
            _currentContent.IsDeleting = false;
        }

        #region Warehouse Transactions

        public static readonly DependencyProperty TransactionIndexProperty = DependencyProperty.Register(
            "TransactionIndex", typeof (int), typeof (SoheilSplitView), new PropertyMetadata(default(int)));

        public int TransactionIndex
        {
            get { return (int) GetValue(TransactionIndexProperty); }
            set { SetValue(TransactionIndexProperty, value); }
        }

        private WarehouseTransactionVM _currentTransaction;
        private void OnAutoGeneratingTransactionColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnAutoGeneratedTransactionColumn(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TransGridPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var grid = (DataGrid)sender;
            _currentTransaction = (WarehouseTransactionVM)grid.CurrentItem;
            if (e.Key == Key.F2)
            {
                if (_cellEditingMode)
                {
                    grid.CommitEdit(DataGridEditingUnit.Cell, true);
                    _cellEditingMode = false;
                }
                else
                {
                    grid.BeginEdit();
                    _cellEditingMode = true;
                }
            }
            else if (!_cellEditingMode && e.Key == Key.Left && Equals(grid.CurrentCell.Column, grid.Columns[grid.Columns.Count - 1]))
            {
                e.Handled = true;
            }
            else if (!_cellEditingMode && e.Key == Key.Right && Equals(grid.CurrentCell.Column, grid.Columns[0]))
            {
                e.Handled = true;
            }
            else if (!_cellEditingMode && e.Key == Key.Up && Equals(grid.SelectedIndex, 0))
            {
                e.Handled = true;
            }
            else if (!_cellEditingMode && e.Key == Key.Down && Equals(grid.SelectedIndex, grid.Items.Count - 1))
            {
                e.Handled = true;
            }
            else if (_cellEditingMode && e.Key == Key.Left || e.Key == Key.Right)
            {
                grid.CommitEdit(DataGridEditingUnit.Cell, true);
                _cellEditingMode = false;
            }
            else if (e.Key == Key.Enter)
            {
                if (Equals(grid.CurrentCell.Column, grid.Columns[grid.Columns.Count - 1]))
                {
                    grid.CommitEdit(DataGridEditingUnit.Row, true);
                    if (_currentTransaction.CanSave())
                    {
                        _currentTransaction.Save(null);
                        if (grid.Items.IndexOf(grid.CurrentItem) == grid.Items.Count - 1)
                            ((WarehouseReceiptVM)ViewModel.CurrentContent).AddBlankTransaction();
                    }
                    else
                        _currentTransaction.Mode = ModificationStatus.Unsaved;
                }
                _isSaving = true;
                _cellEditingMode = false;
            }
            else if (!_cellEditingMode)
            {
                if (e.Key == Key.Delete)
                {
                    if (_currentTransaction.Transported || !_currentTransaction.IsReadOnly)
                    {
                        e.Handled = true;
                    }
                    else if (_currentTransaction.IsDeleting)
                    {
                        e.Handled = true;
                        ((WarehouseReceiptVM)ViewModel.CurrentContent).CurrentTransaction.Delete(null);
                    }
                    else
                    {
                        e.Handled = true;
                        _currentTransaction.IsDeleting = true;
                    }
                }
                else if (e.Key == Key.Escape)
                {
                    _currentTransaction.IsDeleting = false;
                }
            }
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9
                || e.Key >= Key.A && e.Key <= Key.Z)
            {
                grid.BeginEdit();
                _cellEditingMode = true;
            }
        }

        private void TransGridOnPreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TransGridOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}