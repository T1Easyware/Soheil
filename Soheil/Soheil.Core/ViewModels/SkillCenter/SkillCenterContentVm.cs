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

		//can be either productGroup, product, productRework or All(General)
		public BaseTreeItemVm SelectedItem { get; protected set; }

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

			var rows = _operatorDataService.GetActives().Select(operatorModel => new OperatorRowVm(operatorModel));
			var columns = _aDataService.GetActives().Select(activityModel => new ActivityColumnVm(activityModel));
			var groups = _agDataService.GetActives().Select(activityGroupModel => new ActivityGroupColumnVm(activityGroupModel));
			foreach (var grp in groups) Groups.Add(grp);
			foreach (var col in columns) Columns.Add(col);
			foreach (var row in rows)
			{
				switch (_targetMode)
				{
					case TargetMode.General:
						foreach (var act in columns)
						{
							var activitySkill = _asDataService.FindOrAdd(row.Id, act.Id);
							row.Cells.Add(new ActivitySkillVm(activitySkill));
						}
						break;
					case TargetMode.ProductRework:
						foreach (var act in columns)
						{
							var productActivitySkill = _pasDataService.FindOrAdd(SelectedItem.Id, row.Id, act.Id);
							row.Cells.Add(new ProductReworkActivitySkillVm(productActivitySkill));
						}
						break;
					case TargetMode.Product:
						foreach (var act in columns)
						{
							//var productActivitySkill = _pasDataService.FindOrAdd(row.Id, act.Id);
							row.Cells.Add(new ProductActivitySkillVm());
						}
						break;
					case TargetMode.ProductGroup:
						foreach (var act in columns)
						{
							//var activitySkill = _asDataService.FindOrAdd(row.Id, act.Id);
							row.Cells.Add(new ProductGroupActivitySkillVm());
						}
						break;
					default:
						break;
				}
				Rows.Add(row);
			}
		} 
		#endregion
	}
}
