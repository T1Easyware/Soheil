using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Soheil.Common;
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
        }

        private void ListUpdated(object sender, RoutedEventArgs e)
        {
            string code = Convert.ToString(((Control)e.Source).Tag);
            var accessItem = AccessList.FirstOrDefault(item => item.Item1 == code);
            var access = accessItem == null ? AccessType.None : accessItem.Item2;
            var type = ((SoheilEntityType)Convert.ToInt32(((Control)sender).Tag));
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
					ViewModel = new WorkProfilesVM(access);//---=-====-=-==-=----=-=-=-=-=-=-=-=----=-=-=
					break;
				case SoheilEntityType.WorkProfilePlan:
					ViewModel = new WorkProfilesVM(access);//-=----=-=-==-====-=-==-=----=-=-=-=-=-=-=-=-
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
                case SoheilEntityType.Fpc:
                    ViewModel = new FpcsVm(access);
                    break;
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
                    ViewModel = new PartWarehousesVM(access);
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
            grid.CurrentCell = new DataGridCellInfo(grid.Items[CurrentIndex],grid.Columns[0]);
        }

		#region WorkProfile Events
		private void workShiftLineMouseMove(object sender, MouseEventArgs e)
		{
			var line = (FrameworkElement)sender;
			var x = e.GetPosition(line).X;
			var timetip = line.Tag as FrameworkElement;
			showTimetip(timetip, x);
		}
		private void showTimetip(FrameworkElement timetip, double x)
		{
			if (timetip != null)
			{
				timetip.Margin = new Thickness(x, 0, 0, 0);
				int seconds = SoheilFunctions.RoundFiveMinutes((int)x * 60 + SoheilConstants.EDITOR_START_SECONDS);
				timetip.Tag = SoheilFunctions.GetWorkShiftTime(seconds);
				var fadeout = timetip.Resources["fadeout"] as System.Windows.Media.Animation.Storyboard;
				fadeout.Stop();
				timetip.Opacity = 1;
				fadeout.Begin();
			}
		}
		private double _onThumbStartX;
		private double getDeltaOnLine(object sender)
		{
			var thumb = sender as FrameworkElement;
			if (thumb != null)
			{
				var line = thumb.Tag as FrameworkElement;
				if (line != null)
				{
					return Mouse.GetPosition(line).X;
				}
			}
			return double.NaN;
		}
		private FrameworkElement updateTimetip(object sender, System.Windows.Visibility visiblility)
		{
			var thumb = sender as FrameworkElement;
			if (thumb == null) return null;
			var timebelow = thumb.GetNextVisual();
			if (timebelow != null)
				timebelow.Visibility = visiblility;
			return timebelow;
		}
		private void showTimetipForLine(object sender, int seconds)
		{
			showTimetip(
				(sender as FrameworkElement).Tag as FrameworkElement, 
				(SoheilFunctions.RoundFiveMinutes(seconds) - SoheilConstants.EDITOR_START_SECONDS) / 60);
		}
		private void shiftStartDragStart(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			updateTimetip(sender, System.Windows.Visibility.Visible);
			var thumb = sender as FrameworkElement;
			if (thumb == null) return;
			_onThumbStartX = Mouse.GetPosition(thumb).X;
		}

		private void shiftStartDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			var onLineX = getDeltaOnLine(sender);
			var shift = sender.GetDataContext<Soheil.Core.ViewModels.OrganizationCalendar.WorkShiftVm>();
			if (shift!=null && !double.IsNaN(onLineX))
				shift.StartSeconds =
					SoheilConstants.EDITOR_START_SECONDS + (int)((onLineX - _onThumbStartX) * 60);
		}
		private void breakStartDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			var onLineX = getDeltaOnLine(sender);
			var wbreak = sender.GetDataContext<Soheil.Core.ViewModels.OrganizationCalendar.WorkBreakVm>();
			if (wbreak != null && !double.IsNaN(onLineX))
				wbreak.StartSeconds =
					SoheilConstants.EDITOR_START_SECONDS + (int)((onLineX - _onThumbStartX) * 60);
		}

		private void shiftStartDragEnd(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			updateTimetip(sender, System.Windows.Visibility.Collapsed);
		}

		private void shiftEndDragStart(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			updateTimetip(sender, System.Windows.Visibility.Visible);
			var thumb = sender as FrameworkElement;
			if (thumb == null) return;
			_onThumbStartX = thumb.ActualWidth - Mouse.GetPosition(thumb).X;
		}

		private void shiftEndDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			var onLineX = getDeltaOnLine(sender);
			var shift = sender.GetDataContext<Soheil.Core.ViewModels.OrganizationCalendar.WorkShiftVm>();
			if (shift != null && !double.IsNaN(onLineX))
				shift.EndSeconds =
					SoheilConstants.EDITOR_START_SECONDS + (int)((onLineX + _onThumbStartX) * 60);
		}
		private void breakEndDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			var onLineX = getDeltaOnLine(sender);
			var wbreak = sender.GetDataContext<Soheil.Core.ViewModels.OrganizationCalendar.WorkBreakVm>();
			if (wbreak != null && !double.IsNaN(onLineX))
				wbreak.EndSeconds =
					SoheilConstants.EDITOR_START_SECONDS + (int)((onLineX + _onThumbStartX) * 60);
		}

		private void shiftEndDragEnd(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			updateTimetip(sender, System.Windows.Visibility.Collapsed);
		}

		Soheil.Core.ViewModels.OrganizationCalendar.WorkBreakVm _currentDrawingBreak;
		private void shiftLineDragStart(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			var thumb = sender as FrameworkElement;
			var day = sender.GetDataContext<Soheil.Core.ViewModels.OrganizationCalendar.WorkDayVm>();
			if (thumb == null || day == null) return;

			int seconds = (int)(e.HorizontalOffset * 60) + SoheilConstants.EDITOR_START_SECONDS;
			_currentDrawingBreak = null;
			Soheil.Core.ViewModels.OrganizationCalendar.WorkShiftVm currentDrawingShift = null;
			foreach (var shift in day.Shifts)
			{
				if (seconds >= shift.StartSeconds && seconds <= shift.EndSeconds)
					currentDrawingShift = shift;
			}

			if (currentDrawingShift == null) MessageBox.Show("ساعت استراحت بایستی داخل شیفت افزوده شود");
			else
			{
				_currentDrawingBreak = currentDrawingShift.AddTemporaryBreak(seconds);
				_onThumbStartX = Mouse.GetPosition(thumb).X;

				//showTimetipForLine(sender, seconds);
			}
		}
		private void shiftLineDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			var thumb = sender as FrameworkElement;
			if (thumb == null || _currentDrawingBreak == null) return;

			int oldSeconds = (int)(_onThumbStartX * 60) + SoheilConstants.EDITOR_START_SECONDS;
			int newSeconds = (int)(Mouse.GetPosition(thumb).X * 60) + SoheilConstants.EDITOR_START_SECONDS;
			if (newSeconds > oldSeconds)
			{
				_currentDrawingBreak.StartSeconds = oldSeconds;
				_currentDrawingBreak.EndSeconds = newSeconds;
			}
			else
			{
				_currentDrawingBreak.StartSeconds = newSeconds;
				_currentDrawingBreak.EndSeconds = oldSeconds;
			}
			showTimetipForLine(sender, newSeconds);
		}
		private void shiftLineDragEnd(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
		}

		#endregion

		private void skillCenterLoaded(object sender, RoutedEventArgs e)
		{

		}
	}
}