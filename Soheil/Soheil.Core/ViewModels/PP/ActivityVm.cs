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
	/// The simplest implementation of a ViewModel for Activity 
	/// </summary>
	public class ActivityVm : ViewModelBase
	{
		Model.Activity _model;
		/// <summary>
		/// Creates an instance of ActivityVm and initializes it with the given model
		/// <para>parameter can't be null</para>
		/// </summary>
		/// <param name="model">Activity model (no entity reference required)</param>
		public ActivityVm(Model.Activity model)
		{
			_model = model;
		}
		/// <summary>
		/// Gets Id property of the model representing this ViewModel
		/// </summary>
		public int Id { get { return _model.Id; } }
		/// <summary>
		/// Gets Name property of the model representing this ViewModel
		/// </summary>
		public string Name { get { return _model == null ? "" : _model.Name; } }
		/// <summary>
		/// Gets Code property of the model representing this ViewModel
		/// </summary>
		public string Code { get { return _model == null ? "" : _model.Code; } }
	}
}
