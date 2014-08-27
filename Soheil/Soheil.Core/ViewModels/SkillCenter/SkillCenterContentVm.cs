using Soheil.Common;
using Soheil.Common.SoheilException;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// ViewModel for Content of <see cref="SkillCenterVm"/> when a <see cref="BaseTreeItemVm"/> is selected
	/// </summary>
	public class SkillCenterContentVm : DependencyObject
	{
		/// <summary>
		/// Specifies the content mode of current Vm
		/// </summary>
		public TargetMode Mode;

		/// <summary>
		/// Occures when an error occures in this Vm (parameter contains the error message)
		/// </summary>
		public event Action<string> ErrorOccured;

		/// <summary>
		/// Gets an bindable instance of <see cref="ProductGroupVm"/>, <see cref="ProductVm"/>, <see cref="ProductReworkVm"/> or <see cref="GeneralVm"/> that initialized this Vm
		/// </summary>
		public BaseTreeItemVm SelectedItem
		{
			get { return (BaseTreeItemVm)GetValue(SelectedItemProperty); }
			protected set { SetValue(SelectedItemProperty, value); }
		}
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(BaseTreeItemVm), typeof(SkillCenterContentVm), new UIPropertyMetadata(null));


		#region DataServices
		/// <summary>
		/// Instance of <see cref="DataServices.OperatorDataService"/> initialized with local UOW
		/// </summary>
		public DataServices.OperatorDataService OperatorDataService;
		/// <summary>
		/// Instance of <see cref="DataServices.ActivityDataService"/> initialized with local UOW
		/// </summary>
		public DataServices.ActivityDataService ActivityDataService;
		/// <summary>
		/// Instance of <see cref="DataServices.ActivityGroupDataService"/> initialized with local UOW
		/// </summary>
		public DataServices.ActivityGroupDataService ActivityGroupDataService;
		/// <summary>
		/// Instance of <see cref="DataServices.ActivitySkillDataService"/> initialized with local UOW
		/// </summary>
		public DataServices.ActivitySkillDataService ActivitySkillDataService;
		/// <summary>
		/// Instance of <see cref="DataServices.ProductActivitySkillDataService"/> initialized with local UOW
		/// </summary>
		public DataServices.ProductActivitySkillDataService ProductActivitySkillDataService; 
		#endregion

		#region Table
		/// <summary>
		/// Gets a collection of <see cref="OperatorRowVm"/>s (all operators)
		/// </summary>
		public ObservableCollection<OperatorRowVm> Rows { get { return _rows; } }
		private ObservableCollection<OperatorRowVm> _rows = new ObservableCollection<OperatorRowVm>();
		
		/// <summary>
		/// Gets a collection of <see cref="ActivityGroupColumnVm"/>, each for one column group in the skill center table
		/// </summary>
		public ObservableCollection<ActivityGroupColumnVm> Groups { get { return _groups; } }
		private ObservableCollection<ActivityGroupColumnVm> _groups = new ObservableCollection<ActivityGroupColumnVm>();
	
		/// <summary>
		/// Gets a collection of <see cref="ActivityColumnVm"/>, each for one column in the skill center table
		/// </summary>
		public ObservableCollection<ActivityColumnVm> Columns { get { return _columns; } }
		private ObservableCollection<ActivityColumnVm> _columns = new ObservableCollection<ActivityColumnVm>(); 
		#endregion
		
		#region Init
		/// <summary>
		/// Initializes UOW and data services of this Vm with the given <see cref="BaseTreeItemVm"/> node
		/// </summary>
		public void InitializeDataService()
		{
			//Init DataServices
			var uow = new Soheil.Dal.SoheilEdmContext();
			OperatorDataService = new DataServices.OperatorDataService(uow);
			ActivityDataService = new DataServices.ActivityDataService(uow);
			ActivityGroupDataService = new DataServices.ActivityGroupDataService(uow);
			ActivitySkillDataService = new DataServices.ActivitySkillDataService(uow);
			ProductActivitySkillDataService = new DataServices.ProductActivitySkillDataService(uow);
		}
		/// <summary>
		/// Initializes rows and columns of this Vm with the given <see cref="BaseTreeItemVm"/> node
		/// <para>Fills the cells of the table according to type of the initializer node</para>
		/// </summary>
		/// <param name="node">A tree item that initializes the mode and data of this Vm</param>
		public void Initialize(BaseTreeItemVm node)
		{
			if (node is GeneralVm)
			{
				Mode = TargetMode.General;
			}
			else if (node is ProductGroupVm)
			{
				Mode = TargetMode.ProductGroup;
			}
			else if (node is ProductVm)
			{
				Mode = TargetMode.Product;
			}
			else if (node is ProductReworkVm)
			{
				Mode = TargetMode.ProductRework;
			}
			SelectedItem = node;


			//Init Data
			Groups.Clear();
			Columns.Clear();
			Rows.Clear();

			//add columns
			var a_g = ActivityDataService.GetActives().GroupBy(x => x.ActivityGroup);
			foreach (var activityGroup in a_g)
			{
				Groups.Add(new ActivityGroupColumnVm(activityGroup.Key));
				foreach (var activity in activityGroup)
				{
					Columns.Add(new ActivityColumnVm(activity));
				}
			}

			//add rows
			var rows = OperatorDataService.GetActives().Select(operatorModel =>
				new OperatorRowVm(operatorModel));

			//add cells
			foreach (var row in rows)
			{
				foreach (var act in Columns)
				{
					BaseSkillVm skill;
					switch (Mode)
					{
						case TargetMode.General:
							skill = new ActivitySkillVm(row.Id, act.Id);
							break;
						case TargetMode.ProductRework:
							skill = new ProductReworkActivitySkillVm(row.Id, act.Id, SelectedItem.Id);
							break;
						case TargetMode.Product:
							return;
						case TargetMode.ProductGroup:
							return;
						default:
							return;
					}
					skill.IluoChanged += saveSkillToDatabase;
					row.Cells.Add(skill);
				}
				Rows.Add(row);
			}
		}
		//General mode
		public void InitializeData(IEnumerable<Model.ActivitySkill> skills)
		{
			if (Mode == TargetMode.General)
			{
				foreach (var skill in skills)
				{
					var row = Rows.FirstOrDefault(x => x.Id == skill.Operator.Id);
					if (row == null) return;
					var cell = row.Cells.FirstOrDefault(x => x.ActivityId == skill.Activity.Id) as ActivitySkillVm;
					if (cell == null) return;
					cell.Update(skill);
				}
			}
			else
			{
				foreach (var gen in skills)
				{
					var row = Rows.FirstOrDefault(x => x.Id == gen.Operator.Id);
					if (row == null) return;
					var cell = row.Cells.FirstOrDefault(x => x.ActivityId == gen.Activity.Id) as ProductReworkActivitySkillVm;
					if (cell == null) return;
					cell.Update(gen);
				}
			}
		}
		//ProductRework mode
		public void InitializeData(IEnumerable<Model.ProductActivitySkill> skills)
		{
			foreach (var skill in skills)
			{
				var row = Rows.FirstOrDefault(x => x.Id == skill.ActivitySkill.Operator.Id);
				if (row == null) return;
				var cell = row.Cells.FirstOrDefault(x => x.ActivityId == skill.ActivitySkill.Activity.Id) as ProductReworkActivitySkillVm;
				if (cell == null) return;
				cell.Update(skill);
			}
		}

		/// <summary>
		/// Event handler for IluoChanged event on each instance of <see cref="BaseSkillVm"/>
		/// </summary>
		/// <param name="skill">target skill that will be saved</param>
		void saveSkillToDatabase(BaseSkillVm skill)
		{
			if (skill == null)
			{
				if (ErrorOccured!= null)
					ErrorOccured("Can't Save. refresh the page.");
			}
			else if (skill is ActivitySkillVm)
			{
				ActivitySkillDataService.AddOrUpdateSkill(skill as ActivitySkillVm);
			}
			else if (skill is ProductReworkActivitySkillVm)
			{
				ProductActivitySkillDataService.AddOrUpdateSkill(skill as ProductReworkActivitySkillVm);
			}
			else
			{
				//Modes: Product & ProductGroup are not supported yet???
				if (ErrorOccured != null)
					ErrorOccured("Save is not supported in this mode.");
			}
		}

		#endregion
	}
}
