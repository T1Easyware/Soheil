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
	/// A Simple Vm of Activity to be used in PPTable
	/// </summary>
	public class ActivityVm : ViewModelBase
	{
		/// <summary>
		/// Gets the activity model
		/// </summary>
		public Model.Activity Model { get; protected set; }
		/// <summary>
		/// Creates an instance of ActivityVm and initializes it with the given model
		/// <para>parameter can't be null</para>
		/// </summary>
		/// <param name="model">Activity model (no entity reference required)</param>
		public ActivityVm(Model.Activity model)
		{
			Model = model;
		}
		/// <summary>
		/// Gets Id property of the model representing this ViewModel
		/// </summary>
		public int Id { get { return Model.Id; } }
		/// <summary>
		/// Gets Name property of the model representing this ViewModel
		/// </summary>
		public string Name { get { return Model == null ? "" : Model.Name; } }
		/// <summary>
		/// Gets Code property of the model representing this ViewModel
		/// </summary>
		public string Code { get { return Model == null ? "" : Model.Code; } }
	}
}
