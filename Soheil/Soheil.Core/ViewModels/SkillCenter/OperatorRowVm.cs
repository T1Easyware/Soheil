using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// One row in SkillCenter table, representing a single Operator
	/// </summary>
	public class OperatorRowVm : BaseVm
	{
		/// <summary>
		/// Creates an instance of this Vm with the given model
		/// </summary>
		/// <param name="model">Id, Code and Name of this model are used</param>
		public OperatorRowVm(Model.Operator model)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
		}

		/// <summary>
		/// Gets a collection of <see cref="BaseSkillVm"/> representing the skills of this operator in each activity
		/// </summary>
		public ObservableCollection<BaseSkillVm> Cells { get { return _cells; } }
		private ObservableCollection<BaseSkillVm> _cells = new ObservableCollection<BaseSkillVm>();
	}
}
