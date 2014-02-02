using Soheil.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class ActivityVm : ViewModelBase
	{
		public ActivityVm(Model.Activity model)
		{
			_model = model;
			Name = model.Name;
			Code = model.Code;
		}
		Model.Activity _model;
		public int Id { get { return _model.Id; } }
		public string Name
		{
			get { return _model == null ? "" : _model.Name; }
			set { _model.Name = value; OnPropertyChanged("Name"); }
		}
		public string Code
		{
			get { return _model == null ? "" : _model.Code; }
			set { _model.Code = value; OnPropertyChanged("Code"); }
		}
	}
}
