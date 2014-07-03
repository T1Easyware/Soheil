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
		/// Specifies the tree type of <see cref="BaseTreeItemVm"/> which initializes this instance of <see cref="SkillCenterContentVm"/>
		/// </summary>
		public enum TargetMode { General, ProductRework, Product, ProductGroup };
		
		/// <summary>
		/// Specifies the content mode of current Vm
		/// </summary>
		TargetMode _targetMode;

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
		DataServices.OperatorDataService _operatorDataService;
		/// <summary>
		/// Instance of <see cref="DataServices.ActivityDataService"/> initialized with local UOW
		/// </summary>
		DataServices.ActivityDataService _aDataService;
		/// <summary>
		/// Instance of <see cref="DataServices.ActivityGroupDataService"/> initialized with local UOW
		/// </summary>
		DataServices.ActivityGroupDataService _agDataService;
		/// <summary>
		/// Instance of <see cref="DataServices.ActivitySkillDataService"/> initialized with local UOW
		/// </summary>
		DataServices.ActivitySkillDataService _asDataService;
		/// <summary>
		/// Instance of <see cref="DataServices.ProductActivitySkillDataService"/> initialized with local UOW
		/// </summary>
		DataServices.ProductActivitySkillDataService _pasDataService; 
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
		
		#region Ctor and Init
		/// <summary>
		/// Instantiates an initializes this Vm with the given <see cref="BaseTreeItemVm"/> node
		/// </summary>
		/// <param name="node">A tree item that initializes the mode and data of this Vm</param>
		public SkillCenterContentVm(BaseTreeItemVm node)
		{
			SelectedItem = node;
			if (node is GeneralVm)
			{
				_targetMode = TargetMode.General;
			}
			else if (node is ProductGroupVm)
			{
				_targetMode = TargetMode.ProductGroup;
			}
			else if (node is ProductVm)
			{
				_targetMode = TargetMode.Product;
			}
			else if (node is ProductReworkVm)
			{
				_targetMode = TargetMode.ProductRework;
			}
			initializeData();
		}

		/// <summary>
		/// Initializes UOW, data services, rows and columns of this Vm
		/// <para>Fills the cells of the table according to type of the initializer node</para>
		/// </summary>
		void initializeData()
		{
			//Init DataServices

			var uow = new Soheil.Dal.SoheilEdmContext();
			_operatorDataService = new DataServices.OperatorDataService(uow);
			_aDataService = new DataServices.ActivityDataService(uow);
			_agDataService = new DataServices.ActivityGroupDataService(uow);
			if (_targetMode == TargetMode.General)
				_asDataService = new DataServices.ActivitySkillDataService(uow);
			else
				_pasDataService = new DataServices.ProductActivitySkillDataService(uow);

			//Init Data

			var a_g = _aDataService.GetActives().GroupBy(x => x.ActivityGroup);
			foreach (var activityGroup in a_g)
			{
				Groups.Add(new ActivityGroupColumnVm(activityGroup.Key));
				foreach (var activity in activityGroup)
				{
					Columns.Add(new ActivityColumnVm(activity));
				}
			}

			//add cells
			var rows = _operatorDataService.GetActives().Select(operatorModel => 
				new OperatorRowVm(operatorModel));

			switch (_targetMode)
			{
				case TargetMode.General:

					//General mode
					foreach (var row in rows)
					{
						foreach (var act in Columns)
						{
							var activitySkill = _asDataService.FindOrAdd(row.Id, act.Id);
							var skill = new ActivitySkillVm(activitySkill);
							skill.IluoChanged += saveSkillToDatabase;
							row.Cells.Add(skill);
						}
						Rows.Add(row);
					}
					break;
				case TargetMode.ProductRework:

					//ProductRework mode
					foreach (var row in rows)
					{
						foreach (var act in Columns)
						{
							var productActivitySkill = _pasDataService.FindOrAdd(SelectedItem.Id, row.Id, act.Id);
							var skill = new ProductReworkActivitySkillVm(productActivitySkill);
							skill.IluoChanged += saveSkillToDatabase;
							row.Cells.Add(skill);
						}
						Rows.Add(row);
					}
					break;
				case TargetMode.Product:

					//Product mode
					foreach (var row in rows)
					{
						foreach (var act in Columns)
						{
							//var productActivitySkill = _pasDataService.FindOrAdd(row.Id, act.Id);
							var skill = new ProductActivitySkillVm();
							skill.IluoChanged += saveSkillToDatabase;
							row.Cells.Add(skill);
						}
						Rows.Add(row);
					}
					break;
				case TargetMode.ProductGroup:

					//ProductGroup mode
					foreach (var row in rows)
					{
						foreach (var act in Columns)
						{
							//var activitySkill = _asDataService.FindOrAdd(row.Id, act.Id);
							var skill = new ProductGroupActivitySkillVm();
							skill.IluoChanged += saveSkillToDatabase;
							row.Cells.Add(skill);
						}
						Rows.Add(row);
					}
					break;
				default:
					break;
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
				(skill as ActivitySkillVm).Model.Iluo = skill.Data;
				_asDataService.UpdateModel((skill as ActivitySkillVm).Model);
			}
			else if (skill is ProductReworkActivitySkillVm)
			{
				(skill as ProductReworkActivitySkillVm).Model.Iluo = skill.Data;
				_pasDataService.UpdateModel((skill as ProductReworkActivitySkillVm).Model);
			}
			else
			{
				//Modes: Product & ProductGroup are not supported yet???
				if (ErrorOccured != null)
					ErrorOccured("Can't Save in this mode.");
			}
		}

		#endregion
	}
}
