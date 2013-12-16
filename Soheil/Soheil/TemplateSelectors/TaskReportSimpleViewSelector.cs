using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Soheil.Core.ViewModels.PP;

namespace Soheil.TemplateSelectors
{
	public class TaskReportSimpleViewSelector : DataTemplateSelector
	{
		public DataTemplate EmptyTaskReportTemplate { get; set; }
		public DataTemplate TaskReportTemplate { get; set; }
		public DataTemplate TaskReportHolderTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if(item is TaskReportHolderVm)
				return TaskReportHolderTemplate;
			if (item is TaskReportVm)
			{
				if ((item as TaskReportVm).ProducedG1 == 0)
					return EmptyTaskReportTemplate;
				return TaskReportTemplate;
			}
			return new DataTemplate();
		}
	}
}
