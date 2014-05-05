using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class JobVm : DependencyObject
	{
		public JobVm(Model.Job model)
		{
			if (model == null) throw new Soheil.Common.SoheilException.SoheilExceptionBase("Job model can't be null.", Common.SoheilException.ExceptionLevel.Error);
			Model = model;
			Code = model.Code;
			Deadline = model.Deadline;
			ReleaseDT = model.ReleaseTime;
			Quantity = model.Quantity;
			Description = model.Description;
		}

		public Model.Job Model { get; protected set; }
		public int Id { get { return Model.Id; } }
		public int FpcId { get { return Model.FPC.Id; } }
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(JobVm), new UIPropertyMetadata(null));
		//Deadline Dependency Property
		public DateTime Deadline
		{
			get { return (DateTime)GetValue(DeadlineProperty); }
			set { SetValue(DeadlineProperty, value); }
		}
		public static readonly DependencyProperty DeadlineProperty =
			DependencyProperty.Register("Deadline", typeof(DateTime), typeof(JobVm), new UIPropertyMetadata(DateTime.Now));
		//Release DP
		public DateTime ReleaseDT
		{
			get { return (DateTime)GetValue(ReleaseDTProperty); }
			set { SetValue(ReleaseDTProperty, value); }
		}
		public static readonly DependencyProperty ReleaseDTProperty =
			DependencyProperty.Register("ReleaseDT", typeof(DateTime), typeof(JobVm), new PropertyMetadata(DateTime.Now));
		//Quantity Dependency Property
		public int Quantity
		{
			get { return (int)GetValue(QuantityProperty); }
			set { SetValue(QuantityProperty, value); }
		}
		public static readonly DependencyProperty QuantityProperty =
			DependencyProperty.Register("Quantity", typeof(int), typeof(JobVm), new UIPropertyMetadata(0));
		//Description Dependency Property
		public string Description
		{
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		public static readonly DependencyProperty DescriptionProperty =
			DependencyProperty.Register("Description", typeof(string), typeof(JobVm), new UIPropertyMetadata(null));
	}
}
