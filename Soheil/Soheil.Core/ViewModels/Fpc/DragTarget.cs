using Soheil.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	/// <summary>
	/// Abstract view model for draggable objects such as : <see cref="ConnectorVm"/>, <see cref="StateVm"/>, <see cref="ToolboxItemVm"/>
	/// </summary>
	public abstract class DragTarget : ViewModelBase
	{
		/// <summary>
		/// Gets or sets a bindable value that indicates the Margin of this view model in its container
		/// </summary>
		public Vector Location
		{
			get
			{
				var margin = (Thickness)GetValue(LocationProperty);
				return new Vector(margin.Left, margin.Top);
			}
			set { SetValue(LocationProperty, new Thickness(value.X, value.Y, 0, 0)); }
		}
		public static readonly DependencyProperty LocationProperty =
			DependencyProperty.Register("Location", typeof(Thickness), typeof(DragTarget), 
			new UIPropertyMetadata(new Thickness(0), StateVm.AnyPropertyChangedCallback));

		/// <summary>
		/// Gets the Id of the model associated with the derived class
		/// <para>Must be overriden</para>
		/// </summary>
		public abstract int Id { get; }
	}
}
