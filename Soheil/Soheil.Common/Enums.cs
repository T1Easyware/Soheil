using System;
using System.ComponentModel;
using Soheil.Common.Localization;

namespace Soheil.Common
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum Status
    {
        Active = 1,
        Inactive = 2,
        Deleted = 0
    }

	public enum ConfirmationStatus
	{
		New = 1,
		Confirmed = 2,
		Deleted = 0
	}

	[Flags]
	public enum MaintenanceStatus
	{
		//don't change numbers (see edmExtension)

		Inactive = 0,
		
		NotDone = 1,
		Done = 2,

		Early = 4,
		OnTime = 8,
		Late = 16,
	}

	public enum RepairStatus
	{


		//wanna change numbers? 
		//search for #enum# in Core.ViewModels.Reports.PMReportVm.cs


		/// <summary>
		/// Repair Request is disabled
		/// </summary>
		Inactive = 0,
		/// <summary>
		/// Repair Request is reported but not yet delivered
		/// </summary>
		Reported = 1,
		/// <summary>
		/// Repair Request is delivered but not yet completed
		/// </summary>
		NotDone = 2,
		/// <summary>
		/// Repair Request is completed
		/// </summary>
		Done = 3,
	}

    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ModificationStatus
    {
        Unchanged = 0,
        Unsaved = 1,
        Saved = 2
    }

    public enum OperatorRole
    {
        Main = 0,
        Substitude = 1,
        Auxiliary = 2
    }
    public enum PersianOperatorRole
    {
        اصلی = 0,
        جایگزین = 1,
        کمکی = 2
    }

    public enum ILUO
    {
		N = 0,
        I = 1,
        L = 2,
        U = 3,
        O = 4,
		NA = 5,
    }

	/// <summary>
	/// Specifies the tree type of <see cref="BaseTreeItemVm"/> which initializes this instance of <see cref="SkillCenterContentVm"/>
	/// </summary>
	public enum TargetMode { General, ProductRework, Product, ProductGroup };

    [Flags]
    public enum PPFlags
    {
        None = 0,
        Exchange = 1,
        Repair = 2,
        Test = 4,
        Education = 8
    }

    public enum StateType
    {
        Start = 0,
        Mid = 1,
        Final = 2,
        Rework = 3,
        Temp = 4
    }

	public enum Bool3
	{
		True,
		False,
		Undefined
	}

    public enum PersianDayOfWeek
    {
        شنبه = 0,
        یکشنبه = 1,
        دوشنبه = 2,
        سه‎شنبه = 3,
        چهارشنبه = 4,
        پنجشنبه = 5,
        جمعه = 6
    }

    public enum PersianMonth
    {
        فروردین = 1,
        اردیبهشت = 2,
        خرداد = 3,
        تیر = 4,
        مرداد = 5,
        شهریور = 6,
        مهر = 7,
        آبان = 8,
        آذر = 9,
        دی = 10,
        بهمن = 11,
        اسفند = 12
    }

    public enum PersianShortMonth
    {
        فرو = 1,
        ارد = 2,
        خرد = 3,
        تیر = 4,
        مرد = 5,
        شهر = 6,
        مهر = 7,
        آبان = 8,
        آذر = 9,
        دی = 10,
        بهمن = 11,
        اسف = 12
    }

    public enum SoheilEntityType
    {
        None = 0,
        Soheil = 0,
        UsersMenu = 1,
        UserAccessSubMenu = 11,
        Users = 111,
        Positions = 112,
        OrganizationCharts = 113,
        ModulesSubMenu = 12,
        Modules = 121,
		OrganizationCalendar = 13,
		WorkProfiles = 131,
		Holidays = 132,
		WorkProfilePlan = 133,
        DefinitionsMenu = 2,
        ProductsSubMenu = 21,
        Products = 211,
        Reworks = 212,
        DiagnosisSubMenu = 22,
        Defections = 221,
        Roots = 222,
        ActionPlans = 223,
        Causes = 224,
        FpcSubMenu = 23,
        Fpc = 231,
        Stations = 232,
        Machines = 233,
        Activities = 234,
        OperatorsSubMenu = 24,
        Operators = 241,
        GeneralSkills = 242,
        SpecialSkills = 243,
        CostsSubMenu = 25,
        Costs = 251,
        Warehouses = 252,
		SetupTimes = 26,
		SkillCenter = 27,
		SkillCenterForProducts = 271,
		SkillCenterForOperators = 272,
        ControlMenu = 3,
        ProductPlanSubMenu = 31,
		ProductPlanTable = 311,
        PerformanceSubMenu = 32,
		IndicesSubMenu = 33,
		PM = 34,
        ReportsMenu = 4,
        CostReportsSubMenu = 41,
        ActualCostReportsSubMenu = 42,
        OperationReportsSubMenu = 43,
		DailyReport = 44,
		DailyStationPlan = 45,
		PMReport = 46,
        OptionsMenu = 5,
        SettingsSubMenu = 51,
        HelpSubMenu = 52,
        AboutSubMenu = 53,
        StorageMenu = 6,
        WarehouseSubMenu = 61,
        WarehouseTransactions = 62,
        RawMaterialSubMenu = 63,
        UnitsSubMenu = 64,
        UnitSets = 641,
        UnitConversions = 642
    }
	public enum Direction
	{
		Left,
		Up,
		Right,
		Down,
		None
	}
    public enum RelationDirection
    {
        Straight,
        Reverse
    }
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum CostSourceType
    {
        Other,
        Machines,
        Operators,
        Stations,
        Activities,
    }
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum CostType
    {
        All,
        Cash,
        Stock
    }

    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum OEType
    {
        None,
        TimeBased,
        CountBased
    }
    public enum PPDateLevels
    {
        Hours, Days, Month
    }
	public enum PPViewMode
	{
		Acquiring, Simple, Report, Empty
	}
	


	[TypeConverter(typeof(LocalizedEnumConverter))]
    public enum FishboneNodeType
    {
        None = 0,
        Root = 1,
        Man = 2,
        Method = 3,
        Material = 4,
        Machines = 5,
        Maintenance = 6
    }
    [TypeConverter(typeof(LocalizedEnumConverter))]
    [Flags]
    public enum AccessType
    {
        None = 0x0,
        View = 0x1,
        Print = 0x2,
        Update = 0x4,
        Insert = 0x8,
        Full = 0x10,
        All = 0xFF
    }

    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum CauseLevel
    {
        None = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3
    }

    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum IndexType
    {
        None = 0,
        OEE = 1,
        Performance = 2,
        InternalPPM = 3,
        RemainingCapacity = 4
    }
    [Flags]
    public enum IndexFilter
    {
        None = 0x0,
        ByProduct = 0x1,
        ByStation = 0x2,
        ByActivity = 0x4,
        ByOperator = 0x8,
        ByMachine = 0x10,
        ByCauseL1 = 0x20,
        ByCauseL2 = 0x40,
        ByCauseL3 = 0x80
    }

    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum DateTimeIntervals
    {
        None = 0,
        Hourly = 1,
        Shiftly = 2,
        Daily = 3,
        Weekly = 4,
        Monthly = 5,
        Yearly = 6
    }
    public enum SoheilLanguage
    {
        None,
        Farsi,
        English
    }

    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum StockStatus
    {
        Full,
        Used,
        Empty
    }
	//the order is important (order of priority for holidays)
	public enum BusinessDayType
	{
		Closed = 0,
		SpecialDay1 = 1,
		SpecialDay2 = 2,
		SpecialDay3 = 3,
		HalfClosed = 4,
		Open = 5,
		None = 6,
	}
	public enum WorkProfileMergingStrategy
	{
		/// <summary>
		/// Do nothing, let the incomplete shift be
		/// </summary>
		Incomplete = 0,
		/// <summary>
		/// Consider the incomplete shift as an extension to its previous shift
		/// </summary>
		WithLastShift = 1,
		/// <summary>
		/// It's closed, no task there (disable the incomplete shift)
		/// </summary>
		Closed = 2,
	}

	public enum TimeBoxParameter
	{
		Now,
		FirstEmptySpace,
		StartOfHour,
		NextHour,
		PreviousHour,
		AddHalfHour,
		Add5Minutes,
		Add1Minute,
	}

    public enum QualitiveStatus
    {
        None,
        Waste,
        SecondGrade
    }

	public enum NotificationType
	{
		/// <summary>
		/// Late, Error, High priority
		/// </summary>
		Critical,
		/// <summary>
		/// OnTime, Warning, Low priority
		/// </summary>
		Alarm,
		/// <summary>
		/// Early, Information, No priority
		/// </summary>
		Info
	}
}