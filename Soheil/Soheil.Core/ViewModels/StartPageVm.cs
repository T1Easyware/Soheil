using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Soheil.Core.Interfaces;
using Soheil.Common;

namespace Soheil.Core.ViewModels
{
	public class StartPageVm : DependencyObject, ISingularList
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public StartPageVm(AccessType access)
		{
			
		}
	}
}
