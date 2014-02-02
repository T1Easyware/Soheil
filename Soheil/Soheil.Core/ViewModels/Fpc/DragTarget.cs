using Soheil.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	/// <summary>
	/// connector, state, toolbox item are DragTarget
	/// </summary>
	public abstract class DragTarget : ViewModelBase
	{
		//Location(Margin) Dependency Property
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

		public abstract int Id { get; }
	}
}
