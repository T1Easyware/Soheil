using Soheil.Common.SoheilException;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	public class SkillCenterContentVm : DependencyObject
	{
		public enum TargetMode { General, ProductRework, Product, ProductGroup };

		TargetMode _targetMode;

		public event Action<string> ErrorOccured;

		//can be either productGroup, product, productRework or All(General)
		//SelectedItem Dependency Property
		public BaseTreeItemVm SelectedItem
		{
			get { return (BaseTreeItemVm)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(BaseTreeItemVm), typeof(SkillCenterContentVm), new UIPropertyMetadata(null));


		#region DataServices
		DataServices.OperatorDataService _operatorDataService;
		DataServices.ActivityDataService _aDataService;
		DataServices.ActivityGroupDataService _agDataService;
		DataServices.ActivitySkillDataService _asDataService;
		DataServices.ProductActivitySkillDataService _pasDataService; 
		#endregion

		#region Table
		//Rows Observable Collection
		public ObservableCollection<OperatorRowVm> Rows { get { return _rows; } }
		private ObservableCollection<OperatorRowVm> _rows = new ObservableCollection<OperatorRowVm>();
		//Groups Observable Collection
		public ObservableCollection<ActivityGroupColumnVm> Groups { get { return _groups; } }
		private ObservableCollection<ActivityGroupColumnVm> _groups = new ObservableCollection<ActivityGroupColumnVm>();
		//Columns Observable Collection
		public ObservableCollection<ActivityColumnVm> Columns { get { return _columns; } }
		private ObservableCollection<ActivityColumnVm> _columns = new ObservableCollection<ActivityColumnVm>(); 
		#endregion
		
		#region Ctor and Init
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

			//add column groups
			var groups = _agDataService.GetActives().Select(activityGroupModel => new ActivityGroupColumnVm(activityGroupModel));
			foreach (var grp in groups) Groups.Add(grp);
			//add columns
			var columns = _aDataService.GetActives().Select(activityModel => new ActivityColumnVm(activityModel));
			foreach (var col in columns) Columns.Add(col);
			//add cells
			var rows = _operatorDataService.GetActives().Select(operatorModel => new OperatorRowVm(operatorModel));
			switch (_targetMode)
			{
				case TargetMode.General:
					foreach (var row in rows)
					{

						foreach (var act in columns)
						{
							var activitySkill = _asDataService.FindOrAdd(row.Id, act.Id);
							var skill = new ActivitySkillVm(activitySkill);
							skill.Saved += skill_Saved;
							row.Cells.Add(skill);
						}
						Rows.Add(row);
					}
					break;
				case TargetMode.ProductRework:
					foreach (var row in rows)
					{
						foreach (var act in columns)
						{
							var productActivitySkill = _pasDataService.FindOrAdd(SelectedItem.Id, row.Id, act.Id);
							var skill = new ProductReworkActivitySkillVm(productActivitySkill);
							skill.Saved += skill_Saved;
							row.Cells.Add(skill);
						}
						Rows.Add(row);
					}
					break;
				case TargetMode.Product:
					foreach (var row in rows)
					{
						foreach (var act in columns)
						{
							//var productActivitySkill = _pasDataService.FindOrAdd(row.Id, act.Id);
							var skill = new ProductActivitySkillVm();
							skill.Saved += skill_Saved;
							row.Cells.Add(skill);
						}
						Rows.Add(row);
					}
					break;
				case TargetMode.ProductGroup:
					foreach (var row in rows)
					{
						foreach (var act in columns)
						{
							//var activitySkill = _asDataService.FindOrAdd(row.Id, act.Id);
							var skill = new ProductGroupActivitySkillVm();
							skill.Saved += skill_Saved;
							row.Cells.Add(skill);
						}
						Rows.Add(row);
					}
					break;
				default:
					break;
			}
		}

		void skill_Saved(BaseSkillVm skill)
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
				if (ErrorOccured != null)
					ErrorOccured("Can't Save in this mode.");
			}
		}

		#endregion
	}
}
