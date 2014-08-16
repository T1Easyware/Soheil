using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Common
{
	public class TwoStrings : DependencyObject
	{
		public TwoStrings(string string1, string string2)
		{
			SetValue(String1Property, string1);
			String2 = string2;
		}
		public string String1 { get { return (string)GetValue(String2Property); } }
		public string String2
		{
			get { return (string)GetValue(String2Property); }
			set { SetValue(String2Property, value); }
		}
		public static readonly DependencyProperty String1Property = DependencyProperty.Register("String1", typeof(string), typeof(TwoStrings), new UIPropertyMetadata(""));
		public static readonly DependencyProperty String2Property = DependencyProperty.Register("String2", typeof(string), typeof(TwoStrings), new UIPropertyMetadata(""));
	}
}
