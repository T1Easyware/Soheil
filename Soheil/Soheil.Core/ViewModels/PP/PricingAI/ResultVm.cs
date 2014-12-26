using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.PP.PricingAI
{
	public class ResultVm : DependencyObject
	{
		public ResultVm(string product, string[] str, int id)
		{
			Name = product;
			Id = id;
			for (int i = 0; i < 4; i++)
			{
				Price.Add(Convert.ToInt32(str[i + 1]));
			}
			for (int i = 0; i < 4; i++)
			{
				Production.Add(Convert.ToInt32(str[i + 5]));
			}
			for (int i = 0; i < 4; i++)
			{
				Sales.Add(Convert.ToInt32(str[i + 9]));
			}
			HoldingCost = Convert.ToInt32(str[13]);
			ProductionCost = Convert.ToInt32(str[14]);
			PenaltyCost = Convert.ToInt32(str[15]);
			Revenue = Convert.ToInt32(str[16]);
			Profit = Revenue - HoldingCost - ProductionCost - PenaltyCost;
		}
		public int Id { get; set; }
		/// <summary>
		/// Gets or sets a bindable value that indicates HoldingCost
		/// </summary>
		public int HoldingCost
		{
			get { return (int)GetValue(HoldingCostProperty); }
			set { SetValue(HoldingCostProperty, value); }
		}
		public static readonly DependencyProperty HoldingCostProperty =
			DependencyProperty.Register("HoldingCost", typeof(int), typeof(ResultVm), new PropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value that indicates ProductionCost
		/// </summary>
		public int ProductionCost
		{
			get { return (int)GetValue(ProductionCostProperty); }
			set { SetValue(ProductionCostProperty, value); }
		}
		public static readonly DependencyProperty ProductionCostProperty =
			DependencyProperty.Register("ProductionCost", typeof(int), typeof(ResultVm), new PropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value that indicates PenaltyCost
		/// </summary>
		public int PenaltyCost
		{
			get { return (int)GetValue(PenaltyCostProperty); }
			set { SetValue(PenaltyCostProperty, value); }
		}
		public static readonly DependencyProperty PenaltyCostProperty =
			DependencyProperty.Register("PenaltyCost", typeof(int), typeof(ResultVm), new PropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value that indicates Revenue
		/// </summary>
		public int Revenue
		{
			get { return (int)GetValue(RevenueProperty); }
			set { SetValue(RevenueProperty, value); }
		}
		public static readonly DependencyProperty RevenueProperty =
			DependencyProperty.Register("Revenue", typeof(int), typeof(ResultVm), new PropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value that indicates Profit
		/// </summary>
		public int Profit
		{
			get { return (int)GetValue(ProfitProperty); }
			set { SetValue(ProfitProperty, value); }
		}
		public static readonly DependencyProperty ProfitProperty =
			DependencyProperty.Register("Profit", typeof(int), typeof(ResultVm), new PropertyMetadata(0));

		/// <summary>
		/// Gets or sets a bindable value that indicates Name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(ResultVm), new PropertyMetadata(""));
		/// <summary>
		/// Gets or sets a bindable value that indicates Selected
		/// </summary>
		public bool Selected
		{
			get { return (bool)GetValue(SelectedProperty); }
			set { SetValue(SelectedProperty, value); }
		}
		public static readonly DependencyProperty SelectedProperty =
			DependencyProperty.Register("Selected", typeof(bool), typeof(ResultVm), new PropertyMetadata(true));

		/// <summary>
		/// Gets or sets a bindable collection that indicates Production
		/// </summary>
		public ObservableCollection<int> Production { get { return _production; } }
		private ObservableCollection<int> _production = new ObservableCollection<int>();
		/// <summary>
		/// Gets or sets a bindable collection that indicates Price
		/// </summary>
		public ObservableCollection<int> Price { get { return _price; } }
		private ObservableCollection<int> _price = new ObservableCollection<int>();
		/// <summary>
		/// Gets or sets a bindable collection that indicates Sales
		/// </summary>
		public ObservableCollection<int> Sales { get { return _sales; } }
		private ObservableCollection<int> _sales = new ObservableCollection<int>();
	}
}
