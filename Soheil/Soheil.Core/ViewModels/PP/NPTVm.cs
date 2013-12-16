using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Core.PP;

namespace Soheil.Core.ViewModels.PP
{
	public abstract class NPTVm : PPItemVm
	{
		public NPTVm(TaskCollection parent)
		{
			Parent = parent;
		}
		//Parent Dependency Property
		public Core.PP.TaskCollection Parent
		{
			get { return (Core.PP.TaskCollection)GetValue(ParentProperty); }
			set { SetValue(ParentProperty, value); }
		}
		public static readonly DependencyProperty ParentProperty =
			DependencyProperty.Register("Parent", typeof(Core.PP.TaskCollection), typeof(NPTVm), new UIPropertyMetadata(null));

		//IsEditMode Dependency Property
		public bool IsEditMode
		{
			get { return (bool)GetValue(IsEditModeProperty); }
			set { SetValue(IsEditModeProperty, value); }
		}
		public static readonly DependencyProperty IsEditModeProperty =
			DependencyProperty.Register("IsEditMode", typeof(bool), typeof(NPTVm), new UIPropertyMetadata(false));
	}
}
