using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	public class OperatorRowVm : BaseVm
	{
		public OperatorRowVm(Model.Operator model)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
		}

		//Columns Observable Collection
		public ObservableCollection<BaseSkillVm> Cells { get { return _cells; } }
		private ObservableCollection<BaseSkillVm> _cells = new ObservableCollection<BaseSkillVm>();
	}
}
