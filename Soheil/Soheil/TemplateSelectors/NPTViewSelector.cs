using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Soheil.Core.ViewModels.PP;

namespace Soheil.TemplateSelectors
{
	public class NPTViewSelector : DataTemplateSelector
	{
		public DataTemplate SetupTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if(item is SetupVm)
				return SetupTemplate;
			return new DataTemplate();
		}
	}
}
