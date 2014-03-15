using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// Basic abstract class for row, column, column group or tree items of skill center
	/// </summary>
	/// <remarks>
	/// Derived types are <see cref="OperatorRowVm"/>, <see cref="ActivityGroupColumnVm"/>, <see cref="ActivityColumnVm"/> and <see cref="BaseTreeItemVm"/>
	/// </remarks>
	public abstract class BaseVm : DependencyObject
	{
		/// <summary>
		/// Gets a value that indicates the Id of model associated with this Vm
		/// <para>It can be either OperatorId, ActivityGroupId, ActivityId, ProductGroupId, ProductId, ProductReworkId or -100 based on the derived type</para>
		/// </summary>
		/// <remarks>Set this value through constructors of derived type</remarks>
		public int Id { get; protected set; }

		/// <summary>
		/// Gets a bindable value that indicates the Name of model associated with this Vm
		/// </summary>
		/// <remarks>Set this bindable value through constructors of derived type</remarks>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			protected set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(BaseVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets a bindable value that indicates the Code of model associated with this Vm
		/// </summary>
		/// <remarks>Set this bindable value through constructors of derived type</remarks>
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			protected set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(BaseVm), new UIPropertyMetadata(null));
	}
}
