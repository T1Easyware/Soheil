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
		public RequestVm(Core.Reports.MaterialPlanStation station)
		{
			Quantity = station.Quantity;
			StationName = station.Station.Name;
			UnitCode = station.Bom.UnitSet == null ? "عدد" : station.Bom.UnitSet.Code;
		}

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

		/// <summary>
		/// Gets or sets a bindable value that indicates CreateTransactionCommand
		/// </summary>
		public Commands.Command CreateTransactionCommand
		{
			get { return (Commands.Command)GetValue(CreateTransactionCommandProperty); }
			set { SetValue(CreateTransactionCommandProperty, value); }
		}
		public static readonly DependencyProperty CreateTransactionCommandProperty =
			DependencyProperty.Register("CreateTransactionCommand", typeof(Commands.Command), typeof(RequestVm), new UIPropertyMetadata(null));
	}
}
