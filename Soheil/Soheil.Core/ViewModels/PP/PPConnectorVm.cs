using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP
{
	public class PPConnectorVm : PPItemVm
	{
		public PPConnectorVm()
		{

		}

		/// <summary>
		/// Gets or sets a bindable value that indicates FromProduct
		/// </summary>
		public ProductVm FromProduct
		{
			get { return (ProductVm)GetValue(FromProductProperty); }
			set { SetValue(FromProductProperty, value); }
		}
		public static readonly DependencyProperty FromProductProperty =
			DependencyProperty.Register("FromProduct", typeof(ProductVm), typeof(PPConnectorVm), new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable value that indicates ToProduct
		/// </summary>
		public ProductVm ToProduct
		{
			get { return (ProductVm)GetValue(ToProductProperty); }
			set { SetValue(ToProductProperty, value); }
		}
		public static readonly DependencyProperty ToProductProperty =
			DependencyProperty.Register("ToProduct", typeof(ProductVm), typeof(PPConnectorVm), new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable value that indicates Direction
		/// </summary>
		public Thickness Direction
		{
			get { return (Thickness)GetValue(DirectionProperty); }
			set { SetValue(DirectionProperty, value); }
		}
		public static readonly DependencyProperty DirectionProperty =
			DependencyProperty.Register("Direction", typeof(Thickness), typeof(PPConnectorVm), new PropertyMetadata(new Thickness()));

		/// <summary>
		/// Gets or sets a bindable value that indicates Height
		/// </summary>
		public double Height
		{
			get { return (double)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}
		public static readonly DependencyProperty HeightProperty =
			DependencyProperty.Register("Height", typeof(double), typeof(PPConnectorVm), new PropertyMetadata(0d));

		/// <summary>
		/// Gets or sets a bindable value that indicates Width
		/// </summary>
		public double Width
		{
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		public static readonly DependencyProperty WidthProperty =
			DependencyProperty.Register("Width", typeof(double), typeof(PPConnectorVm), new PropertyMetadata(0d));

		//propdpchange
	}
}
