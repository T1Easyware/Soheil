using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	public class ActivityGroupColumnVm : BaseVm
	{
		public ActivityGroupColumnVm(Model.ActivityGroup model)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
			Span = model.Activities.Count;//???those in the current product or all? all for now.
		}
		//Span Dependency Property
		public int Span
		{
			get { return (int)GetValue(SpanProperty); }
			set { SetValue(SpanProperty, value); }
		}
		public static readonly DependencyProperty SpanProperty =
			DependencyProperty.Register("Span", typeof(int), typeof(ActivityGroupColumnVm), new UIPropertyMetadata(1));
	}
}
