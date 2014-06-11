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
		public NPTVm(PPItemCollection parent)
			: base()
		{
			Parent = parent;
			StartDateTimeChanged += newVal =>
			{
				StartDate = newVal.Date;
				StartTime = newVal.TimeOfDay;
			};
			initializeCommands();
		}

		public abstract void Reload(PPItemNpt item);

		//StartDate Dependency Property
		public DateTime StartDate
		{
			get { return (DateTime)GetValue(StartDateProperty); }
			set { SetValue(StartDateProperty, value); }
		}
		public static readonly DependencyProperty StartDateProperty =
			DependencyProperty.Register("StartDate", typeof(DateTime), typeof(NPTVm),
			new UIPropertyMetadata(DateTime.Now, (d, e) =>
			{
				var vm = d as NPTVm;
				var val = (DateTime)e.NewValue;
				vm.StartDateTime = val.Add(vm.StartTime);
			}));
		//StartTime Dependency Property
		public TimeSpan StartTime
		{
			get { return (TimeSpan)GetValue(StartTimeProperty); }
			set { SetValue(StartTimeProperty, value); }
		}
		public static readonly DependencyProperty StartTimeProperty =
			DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(NPTVm), 
			new UIPropertyMetadata(TimeSpan.Zero, (d, e) =>
			{
				var vm = d as NPTVm;
				var val = (TimeSpan)e.NewValue;
				vm.StartDateTime = vm.StartDate.Add(val);
			}));
		//VIndex Dependency Property
		public int VIndex
		{
			get { return (int)GetValue(VIndexProperty); }
			set { SetValue(VIndexProperty, value); }
		}
		public static readonly DependencyProperty VIndexProperty =
			DependencyProperty.Register("VIndex", typeof(int), typeof(NPTVm), new UIPropertyMetadata(0));

		//Parent Dependency Property
		public PPItemCollection Parent
		{
			get { return (PPItemCollection)GetValue(ParentProperty); }
			set { SetValue(ParentProperty, value); }
		}
		public static readonly DependencyProperty ParentProperty =
			DependencyProperty.Register("Parent", typeof(PPItemCollection), typeof(NPTVm), new UIPropertyMetadata(null));

		//IsEditMode Dependency Property
		public bool IsEditMode
		{
			get { return (bool)GetValue(IsEditModeProperty); }
			set { SetValue(IsEditModeProperty, value); }
		}
		public static readonly DependencyProperty IsEditModeProperty =
			DependencyProperty.Register("IsEditMode", typeof(bool), typeof(NPTVm), new UIPropertyMetadata(false));

		#region Commands
		protected virtual void initializeCommands()
		{
		}

		//SaveCommand Dependency Property
		public Commands.Command SaveCommand
		{
			get { return (Commands.Command)GetValue(SaveCommandProperty); }
			set { SetValue(SaveCommandProperty, value); }
		}
		public static readonly DependencyProperty SaveCommandProperty =
			DependencyProperty.Register("SaveCommand", typeof(Commands.Command), typeof(NPTVm), new UIPropertyMetadata(null)); 
		#endregion

	}
}
