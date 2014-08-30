using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Common;

namespace Soheil.Core.ViewModels.MaterialPlanning
{
	public class RequestVm : DependencyObject
	{
		public RequestVm(Model.WarehouseTransaction model)
		{

		}
		public RequestVm()
		{

		}
		/// <summary>
		/// Gets or sets a bindable value that indicates RequestType
		/// </summary>
		public MaterialRequestType RequestType
		{
			get { return (MaterialRequestType)GetValue(RequestTypeProperty); }
			set { SetValue(RequestTypeProperty, value); }
		}
		public static readonly DependencyProperty RequestTypeProperty =
			DependencyProperty.Register("RequestType", typeof(MaterialRequestType), typeof(RequestVm), new PropertyMetadata(MaterialRequestType.NoTransaction));

		/// <summary>
		/// Gets or sets a bindable value that indicates StationName
		/// </summary>
		public string StationName
		{
			get { return (string)GetValue(StationNameProperty); }
			set { SetValue(StationNameProperty, value); }
		}
		public static readonly DependencyProperty StationNameProperty =
			DependencyProperty.Register("StationName", typeof(string), typeof(RequestVm), new PropertyMetadata(""));
		
		/// <summary>
		/// Gets or sets a bindable value that indicates Transaction
		/// </summary>
		public TransactionVm Transaction
		{
			get { return (TransactionVm)GetValue(TransactionProperty); }
			set { SetValue(TransactionProperty, value); }
		}
		public static readonly DependencyProperty TransactionProperty =
			DependencyProperty.Register("Transaction", typeof(TransactionVm), typeof(RequestVm), new PropertyMetadata(null));
	
		/// <summary>
		/// Gets or sets a bindable value that indicates Quantity
		/// </summary>
		public double Quantity
		{
			get { return (double)GetValue(QuantityProperty); }
			set { SetValue(QuantityProperty, value); }
		}
		public static readonly DependencyProperty QuantityProperty =
			DependencyProperty.Register("Quantity", typeof(double), typeof(RequestVm), new PropertyMetadata(0d));

		/// <summary>
		/// Gets or sets a bindable value that indicates UnitCode
		/// </summary>
		public string UnitCode
		{
			get { return (string)GetValue(UnitCodeProperty); }
			set { SetValue(UnitCodeProperty, value); }
		}
		public static readonly DependencyProperty UnitCodeProperty =
			DependencyProperty.Register("UnitCode", typeof(string), typeof(RequestVm), new PropertyMetadata(""));
	}
}
