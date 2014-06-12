using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// One column group in SkillCenter table, representing a single ActivityGroup
	/// </summary>
	public class ActivityGroupColumnVm : BaseVm
	{
		/// <summary>
		/// Creates an instance of ActivityGroupColumnVm with the given model
		/// </summary>
		/// <param name="model">Id, Code and Name of this model are used, as well as number of Activities</param>
		public ActivityGroupColumnVm(Model.ActivityGroup model)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
			Span = model.Activities.Count(x => x.RecordStatus == Common.Status.Active);
		}
		/// <summary>
		/// Gets a bindable value that indicates the number of activities under this group
		/// </summary>
		public int Span
		{
			get { return (int)GetValue(SpanProperty); }
			protected set { SetValue(SpanProperty, value); }
		}
		public static readonly DependencyProperty SpanProperty =
			DependencyProperty.Register("Span", typeof(int), typeof(ActivityGroupColumnVm), new UIPropertyMetadata(1));
	}
}
