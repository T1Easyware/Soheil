using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Soheil.Core.Interfaces;

namespace Soheil.Core.ViewModels.Fpc
{
	public class ViewModel : DependencyObject, IViewModel
	{
		//Id Dependency Property
		public int Id
		{
			get { return (int)GetValue(IdProperty); }
			set { SetValue(IdProperty, value); }
		}
		public static readonly DependencyProperty IdProperty =
			DependencyProperty.Register("Id", typeof(int), typeof(ViewModel), new UIPropertyMetadata(-1));

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
	}
}
