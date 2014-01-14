using Soheil.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class CauseVm : ViewModelBase
	{
		public CauseVm(Model.Cause model)
		{
			_model = model;
			ChildrenModels = new List<Model.Cause>();
			ChildrenModels.AddRange(model.Children);
		}
		/// <summary>
		/// Cause Id
		/// </summary>
		public List<Model.Cause> ChildrenModels { get; set; }

		Model.Cause _model;
		public int Id { get { return _model.Id; } }
		public string Text
		{
			get { return _model == null ? "?" : _model.Name; }
			set { _model.Name = value; OnPropertyChanged("Text"); }
		}
		public string Code
		{
			get { return _model == null ? "??" : string.Format("{0:D2}", _model.Code); }
		}
	}
}
