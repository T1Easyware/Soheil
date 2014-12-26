using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Core.PP.PricingAI;
using Soheil.Core.Commands;

namespace Soheil.Core.ViewModels.PP.PricingAI
{
	public class PriceVm : DependencyObject
	{
		public PriceVm(Price data, ProductPeriodVm ppVm)
		{
			Period = ppVm;
			Fee = data.Fee;
			MaxDemand = data.MaxDemand;
			MinDemand = data.MinDemand;
			MoveUpCommand = new Command(o => Period.MoveUp(this), () => Period.CanMoveUp(this));
			MoveDownCommand = new Command(o => Period.MoveDown(this), () => Period.CanMoveDown(this));
		}
		public ProductPeriodVm Period { get; set; }
		/// <summary>
		/// Gets or sets a bindable value that indicates Fee
		/// </summary>
		public int Fee
		{
			get { return (int)GetValue(FeeProperty); }
			set { SetValue(FeeProperty, value); }
		}
		public static readonly DependencyProperty FeeProperty =
			DependencyProperty.Register("Fee", typeof(int), typeof(PriceVm), new PropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value that indicates MinDemand
		/// </summary>
		public int MinDemand
		{
			get { return (int)GetValue(MinDemandProperty); }
			set { SetValue(MinDemandProperty, value); }
		}
		public static readonly DependencyProperty MinDemandProperty =
			DependencyProperty.Register("MinDemand", typeof(int), typeof(PriceVm), new PropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value that indicates MaxDemand
		/// </summary>
		public int MaxDemand
		{
			get { return (int)GetValue(MaxDemandProperty); }
			set { SetValue(MaxDemandProperty, value); }
		}
		public static readonly DependencyProperty MaxDemandProperty =
			DependencyProperty.Register("MaxDemand", typeof(int), typeof(PriceVm), new PropertyMetadata(0));

		/// <summary>
		/// Gets or sets a bindable value that indicates MoveUpCommand
		/// </summary>
		public Command MoveUpCommand
		{
			get { return (Command)GetValue(MoveUpCommandProperty); }
			set { SetValue(MoveUpCommandProperty, value); }
		}
		public static readonly DependencyProperty MoveUpCommandProperty =
			DependencyProperty.Register("MoveUpCommand", typeof(Command), typeof(PriceVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates MoveDownCommand
		/// </summary>
		public Command MoveDownCommand
		{
			get { return (Command)GetValue(MoveDownCommandProperty); }
			set { SetValue(MoveDownCommandProperty, value); }
		}
		public static readonly DependencyProperty MoveDownCommandProperty =
			DependencyProperty.Register("MoveDownCommand", typeof(Command), typeof(PriceVm), new PropertyMetadata(null));

	}
}
