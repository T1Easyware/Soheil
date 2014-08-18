using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Common
{
	public class ShiftVm : DependencyObject
	{
		public ShiftVm(string shiftCode, string supervisorName)
		{
			SetValue(ShiftCodeProperty, shiftCode);
			Supervisor = supervisorName;
		}
		public string ShiftCode { get { return (string)GetValue(ShiftCodeProperty); } }
		public string Supervisor
		{
			get { return (string)GetValue(SupervisorProperty); }
			set { SetValue(SupervisorProperty, value); }
		}
		public static readonly DependencyProperty ShiftCodeProperty = DependencyProperty.Register("ShiftCode", typeof(string), typeof(ShiftVm), new UIPropertyMetadata(""));
		public static readonly DependencyProperty SupervisorProperty = DependencyProperty.Register("Supervisor", typeof(string), typeof(ShiftVm), new UIPropertyMetadata(""));
	}
}
