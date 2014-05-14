using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Soheil.Common;
using Soheil.Common.Localization;
using Soheil.Controls.CustomControls;
using Soheil.Core;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels;
using Soheil.Core.ViewModels.Index;
using Soheil.Core.ViewModels.Reports;
using Soheil.Views;

namespace Soheil
{
    /// <summary>
    /// Interaction logic for MainWindowTemplate.xaml
    /// </summary>
    public partial class MainWindow
    {
        private int _newTabNumber;
        private readonly UserDataService _userDataService;
        private readonly AccessRuleDataService _accessRuleDataService;

        public static readonly DependencyProperty LoginHeaderProperty =
            DependencyProperty.Register("LoginHeader", typeof (string), typeof (MainWindow), new PropertyMetadata("▼ Login"));

        public string LoginHeader
        {
            get { return (string) GetValue(LoginHeaderProperty); }
            set { SetValue(LoginHeaderProperty, value); }
        }

        public string Username { get; set; }
        public static readonly DependencyProperty AccessListProperty =
            DependencyProperty.Register("AccessList", typeof (List<Tuple<string,AccessType>>), typeof (MainWindow), null);

        public List<Tuple<string, AccessType>> AccessList
        {
            get { return (List<Tuple<string, AccessType>>)GetValue(AccessListProperty); }
            set { SetValue(AccessListProperty, value); }
        }

        public Command LoginCommand { get; set; }

        public MainWindow()
        {
            _accessRuleDataService = new AccessRuleDataService();
            _userDataService = new UserDataService();

            var culture = CultureInfo.GetCultureInfo("fa-IR");

            Dispatcher.Thread.CurrentCulture = culture;
            Dispatcher.Thread.CurrentUICulture = culture;

            LocalizationManager.UpdateValues();
            
            InitializeComponent();
            AccessList = new List<Tuple<string, AccessType>>();
            LoginCommand = new Command(Login);
            _newTabNumber = 1;

            // temp
            Login(null);
            //.

			Closing += (s, e) => Soheil.Core.PP.PPItemManager.Abort();
        }

        public ISplitList SplitList { get; set; }
        public ISingularList SingularList { get; set; }

        private void HandleAddTabAndSelect(object sender, RoutedEventArgs e)
        {
            string code = Convert.ToString(((Control)e.Source).Tag);
            var accessItem = AccessList.FirstOrDefault(item => item.Item1 == code);
            var access = accessItem == null ? AccessType.None : accessItem.Item2;
            var type = (SoheilEntityType)Convert.ToInt32(((Control)e.Source).Tag);

            foreach (ChromeTabItem item in chrometabs.Items)
            {
                if (item.Tag != null && (SoheilEntityType)item.Tag == type)
                {
                    chrometabs.SelectedItem = item;
                    return;
                }
            }

			//!@#$
            switch (type)
            {
                case SoheilEntityType.None:
                    break;
                case SoheilEntityType.UsersMenu:
                    break;
                case SoheilEntityType.UserAccessSubMenu:
                    SplitList = new UsersVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.Users:
                    SplitList = new UsersVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.Positions:
                    SplitList = new PositionsVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.OrganizationCharts:
                    SplitList = new OrganizationChartsVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.ModulesSubMenu:
                    SplitList = new AccessRulesVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.Modules:
                    SplitList = new AccessRulesVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
				case SoheilEntityType.OrganizationCalendar:
                    SplitList = new WorkProfilesVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
					break;
				case SoheilEntityType.WorkProfiles:
                    SplitList = new WorkProfilesVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
					break;
				case SoheilEntityType.Holidays:
					SplitList = new HolidaysVM(access);
					chrometabs.AddTab(CreateSplitTab(type), true);
					break;
				case SoheilEntityType.WorkProfilePlan:
					SplitList = new WorkProfilePlansVM(access);
					chrometabs.AddTab(CreateSplitTab(type), true);
					break;
				case SoheilEntityType.SkillCenter:
					SingularList = new Soheil.Core.ViewModels.SkillCenter.SkillCenterVm(access);
					chrometabs.AddTab(CreateSingularTab(type), true);
					break;
                case SoheilEntityType.DefinitionsMenu:
                    break;
                case SoheilEntityType.ProductsSubMenu:
                    SplitList = new ProductsVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.Products:
                    SplitList = new ProductsVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.Reworks:
                    SplitList = new ReworksVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.DiagnosisSubMenu:
                    SplitList = new DefectionsVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.Defections:
                    SplitList = new DefectionsVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.Roots:
                    SplitList = new RootsVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.ActionPlans:
                    SplitList = new ActionPlansVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.Causes:
                    //SplitList = new (access);
                    break;
                case SoheilEntityType.FpcSubMenu:
                    SplitList = new FpcsVm(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.Fpc:
                    SplitList = new FpcsVm(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.Stations:
                    SplitList = new StationsVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.Machines:
                    SplitList = new MachinesVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.Activities:
                    SplitList = new ActivitiesVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
				case SoheilEntityType.SetupTimes:
					SingularList = new Core.ViewModels.SetupTime.SetupTimeTableVm(access);
					chrometabs.AddTab(CreateSingularTab(type), true);
					break;
                case SoheilEntityType.OperatorsSubMenu:
                    SplitList = new OperatorsVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.Operators:
                    SplitList = new OperatorsVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.GeneralSkills:
                    SplitList = new GeneralSkillsVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.SpecialSkills:
                    SplitList = new SpecialSkillsVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.CostsSubMenu:
                    SplitList = new CostsVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.Costs:
                    SplitList = new CostsVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.Warehouses:
                    SplitList = new PartWarehousesVM(access);
                    chrometabs.AddTab(CreateSplitTab(type), true);
                    break;
                case SoheilEntityType.ControlMenu:
                    break;
                case SoheilEntityType.ProductPlanSubMenu:
				case SoheilEntityType.ProductPlanTable:
					SingularList = new Core.ViewModels.PP.PPTableVm(access);
                    chrometabs.AddTab(CreateSingularTab(type), true);
                    break;
                case SoheilEntityType.PerformanceSubMenu:
                    break;
                case SoheilEntityType.IndicesSubMenu:
                    SingularList = new IndicesVm(access);
                    chrometabs.AddTab(CreateSingularTab(type), true);
                    break;
                case SoheilEntityType.ReportsMenu:
                    break;
                case SoheilEntityType.CostReportsSubMenu:
                    SingularList = new CostReportsVm(access);
                    chrometabs.AddTab(CreateSingularTab(type), true);
                    break;
                case SoheilEntityType.ActualCostReportsSubMenu:
                    SingularList = new ActualCostReportsVm(access);
                    chrometabs.AddTab(CreateSingularTab(type), true);
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
        }

        private object CreateSplitTab(SoheilEntityType type)
        {
            object itemToAdd = new SoheilSplitView(SplitList, AccessList);

            Interlocked.Increment(ref _newTabNumber);

            itemToAdd = new ChromeTabItem
                            {
                                Header = GetTabHeader(type),
                                Content = itemToAdd,
                                Tag = type
                            };

            return itemToAdd;
        }
        private object CreateSingularTab(SoheilEntityType type)
        {
			Cursor openhand = new System.Windows.Input.Cursor(System.IO.Path.Combine(Environment.CurrentDirectory, "Images\\openhand.cur"));
			Cursor closehand = new System.Windows.Input.Cursor(System.IO.Path.Combine(Environment.CurrentDirectory, "Images\\closehand.cur"));
			object itemToAdd = new SoheilSingularView(SingularList, AccessList, openhand, closehand);

            Interlocked.Increment(ref _newTabNumber);

            itemToAdd = new ChromeTabItem
            {
                Header = GetTabHeader(type),
                Content = itemToAdd,
                Tag = type
            };

            return itemToAdd;
        }

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
        private void HandleRemoveTab(object sender, RoutedEventArgs e)
// ReSharper restore UnusedParameter.Local
// ReSharper restore UnusedMember.Local
        {
            chrometabs.RemoveTab(chrometabs.SelectedItem);
        }

        private void GoToPageExecuteHandler(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void GoToPageCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SoheilButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender == null || !(sender is Button)) return;
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void QuickAccessMenuChecked(object sender, RoutedEventArgs e)
        {
            var tag = Convert.ToInt32(((ToggleButton)sender).Tag);
            var tmpl = new DataTemplate();
            switch (tag)
            {
                case 1:
                    btnControls.IsChecked = false;
                    btnDefinitions.IsChecked = false;
                    btnOptions.IsChecked = false;
                    btnReports.IsChecked = false;
                    tmpl = (DataTemplate)FindResource("usersTemplate");
                    break;
                case 2:
                    btnControls.IsChecked = false;
                    btnUsers.IsChecked = false;
                    btnOptions.IsChecked = false;
                    btnReports.IsChecked = false;
                    tmpl = (DataTemplate)FindResource("definitionsTemplate");
                    break;
                case 3:
                    btnUsers.IsChecked = false;
                    btnDefinitions.IsChecked = false;
                    btnOptions.IsChecked = false;
                    btnReports.IsChecked = false;
                    tmpl = (DataTemplate)FindResource("controlsTemplate");
                    break;
                case 4:
                    btnUsers.IsChecked = false;
                    btnDefinitions.IsChecked = false;
                    btnControls.IsChecked = false;
                    btnOptions.IsChecked = false;
                    tmpl = (DataTemplate)FindResource("reportsTemplate");
                    break;
                case 5:
                    btnUsers.IsChecked = false;
                    btnDefinitions.IsChecked = false;
                    btnControls.IsChecked = false;
                    btnReports.IsChecked = false;
                    tmpl = (DataTemplate)FindResource("optionsTemplate");
                    break;
            }
            quickAccessSubMenu.ContentTemplate = tmpl;
        }

        private void MenuItemChecked(object sender, RoutedEventArgs e)
        {
            var clr = ((FrameworkElement)sender).Tag as string;
            if (clr != null)
            {
                var culture = CultureInfo.GetCultureInfo(clr);

                Dispatcher.Thread.CurrentCulture = culture;
                Dispatcher.Thread.CurrentUICulture = culture;
            }

            LocalizationManager.UpdateValues();

            if (clr == "en-US")
            {
                faMenu.IsChecked = false;
                Resources["flowDirection"] = FlowDirection.LeftToRight;
            }
            else
            {
                try { enMenu.IsChecked = false; }
// ReSharper disable EmptyGeneralCatchClause
                catch { }
// ReSharper restore EmptyGeneralCatchClause
                Resources["flowDirection"] = FlowDirection.RightToLeft;
            }
        }

		/// <summary>
		/// Returns the header tab of a SoheilEntityType for showing in MDI tabs
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
        private string GetTabHeader(SoheilEntityType type)
        {
            switch (type)
            {
                case SoheilEntityType.None:
                    return string.Empty;
                case SoheilEntityType.UsersMenu:
                    return Common.Properties.Resources.txtUsersAccess;
                case SoheilEntityType.UserAccessSubMenu:
                    return Common.Properties.Resources.txtUsersAccess;
                case SoheilEntityType.Users:
                    return Common.Properties.Resources.txtUsers;
                case SoheilEntityType.Positions:
                    return Common.Properties.Resources.txtPositions;
				case SoheilEntityType.OrganizationCharts:
					return Common.Properties.Resources.txtOrgCharts;
				case SoheilEntityType.WorkProfiles:
					return Common.Properties.Resources.txtWorkProfiles;
				case SoheilEntityType.WorkProfilePlan:
					return Common.Properties.Resources.txtWorkProfilePlan;
				case SoheilEntityType.OrganizationCalendar:
					return Common.Properties.Resources.txtOrganizationCalendar;
				case SoheilEntityType.ModulesSubMenu:
                    return Common.Properties.Resources.txtModules;
                case SoheilEntityType.Modules:
                    return Common.Properties.Resources.txtModules;
                case SoheilEntityType.DefinitionsMenu:
                    return Common.Properties.Resources.txtDefinitions;
                case SoheilEntityType.ProductsSubMenu:
                    return Common.Properties.Resources.txtProducts;
                case SoheilEntityType.Products:
                    return Common.Properties.Resources.txtProducts;
                case SoheilEntityType.Reworks:
                    return Common.Properties.Resources.txtReworks;
                case SoheilEntityType.DiagnosisSubMenu:
                    return Common.Properties.Resources.txtDiagnosis;
                case SoheilEntityType.Defections:
                    return Common.Properties.Resources.txtDefections;
                case SoheilEntityType.Roots:
                    return Common.Properties.Resources.txtRoots;
                case SoheilEntityType.ActionPlans:
                    return Common.Properties.Resources.txtActionPlans;
                case SoheilEntityType.Causes:
                    return Common.Properties.Resources.txtCauses;
                case SoheilEntityType.FpcSubMenu:
                    return Common.Properties.Resources.txtFPC;
                case SoheilEntityType.Fpc:
                    return Common.Properties.Resources.txtFPC;
                case SoheilEntityType.Stations:
                    return Common.Properties.Resources.txtStations;
                case SoheilEntityType.Machines:
                    return Common.Properties.Resources.txtMachines;
                case SoheilEntityType.Activities:
                    return Common.Properties.Resources.txtActivities;
                case SoheilEntityType.OperatorsSubMenu:
                    return Common.Properties.Resources.txtOperators;
                case SoheilEntityType.Operators:
                    return Common.Properties.Resources.txtOperators;
                case SoheilEntityType.GeneralSkills:
                    return Common.Properties.Resources.txtGenSkills;
                case SoheilEntityType.SpecialSkills:
                    return Common.Properties.Resources.txtSpeSkills;
                case SoheilEntityType.CostsSubMenu:
                    return Common.Properties.Resources.txtCosts;
                case SoheilEntityType.Costs:
                    return Common.Properties.Resources.txtCosts;
                case SoheilEntityType.Warehouses:
                    return Common.Properties.Resources.txtPartWarehouses;
                case SoheilEntityType.ControlMenu:
                    return Common.Properties.Resources.txtControl;
                case SoheilEntityType.ProductPlanSubMenu:
                    return Common.Properties.Resources.txtProductPlan;
                case SoheilEntityType.PerformanceSubMenu:
                    return Common.Properties.Resources.txtPerformance;
                case SoheilEntityType.IndicesSubMenu:
                    return Common.Properties.Resources.txtIndices;
                case SoheilEntityType.CostReportsSubMenu:
                    return Common.Properties.Resources.txtCostReports;
                case SoheilEntityType.ActualCostReportsSubMenu:
                    return Common.Properties.Resources.txtActualCostReports;
                case SoheilEntityType.SettingsSubMenu:
                    return Common.Properties.Resources.txtSettings;
                case SoheilEntityType.HelpSubMenu:
                    return Common.Properties.Resources.txtHelp;
                case SoheilEntityType.AboutSubMenu:
                    return Common.Properties.Resources.txtAbout;
                default:
                    return string.Empty;
            }
        }

        public void Login(object param)
        {
            //var userInfo = _accessRuleDataService.VerifyLogin(Username, ((PasswordBox)param).Password);
            var userInfo = _accessRuleDataService.VerifyLogin("Admin", "fromdust");
            LoginInfo.DataService = _userDataService;
            if (userInfo.Item1 >=0)
            {
                AccessList = _accessRuleDataService.GetAccessOfUser(userInfo.Item1);
                LoginHeader = userInfo.Item2;
                LoginInfo.Id = userInfo.Item1;
                LoginInfo.Title = Username;
                LoginInfo.Access = AccessList;
            }
            else
            {
                AccessList = new List<Tuple<string, AccessType>>();
                LoginHeader = "Login Failed";
                LoginInfo.Id = -1;
                LoginInfo.Title = string.Empty;
                LoginInfo.Access = AccessList;
            }
        }
    }
}