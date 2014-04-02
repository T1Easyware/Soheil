using Soheil.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	/// <summary>
	/// Cause ViewModel for showing in PPTable's reports
	/// </summary>
	public class CauseVm : ViewModelBase
	{
		Model.Cause _model;
		/// <summary>
		/// Creates an instance of CauseVm with the given model
		/// </summary>
		/// <remarks>CODE OF CAUSE MUST BE 1 or 2 DIGITS</remarks>
		/// <param name="model">model or its children can't be null</param>
		public CauseVm(Model.Cause model)
		{
			_model = model;
			ChildrenModels = new List<Model.Cause>();
			ChildrenModels.AddRange(model.Children);
		}
		/// <summary>
		/// Gets the sublevel collection of this Cause
		/// </summary>
		public List<Model.Cause> ChildrenModels { get; protected set; }

		/// <summary>
		/// Gets the Id of model
		/// </summary>
		public int Id { get { return _model.Id; } }

		/// <summary>
		/// Gets the Name of this cause
		/// </summary>
		public string Text
		{
			get { return _model == null ? "?" : _model.Name; }
			set { _model.Name = value; OnPropertyChanged("Text"); }
		}
		/// <summary>
		/// Gets the Code of this cause represented in two digits
		/// </summary>
		public string Code
		{
			get { return _model == null ? "??" : string.Format("{0:D2}", _model.Code); }
		}
	}
}
