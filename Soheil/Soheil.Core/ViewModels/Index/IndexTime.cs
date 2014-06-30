using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.Index
{
	public class IndexTime : DependencyObject
	{
		public event Action<IndexTime> Selected;

		/// <summary>
		/// Creates a ViewModel for an index parameter in which the duration is the given number of hours
		/// </summary>
		/// <param name="hours">Number of hours applied for the index parameter</param>
		/// <param name="total">Total number of hours for the index</param>
		public IndexTime(double hours, double total, string text = null)
		{
			Hours = hours;
			Perc = 100 * hours / total;
			Text = text;
			SelectCommand = new Commands.Command(o =>
			{
				//for detailed indices
				if (!string.IsNullOrWhiteSpace(Text)) { if (Selected != null) Selected(this); }

					//for normal indices
				else if (ShowSubItems)
				{
					ShowSubItems = false;
				}
				else
				{
					ShowSubItems = true;
					//load
					if (Selected != null) Selected(this);
				}
			});

		}
		/// <summary>
		/// Gets or sets a bindable value that indicates total Hours of current object
		/// </summary>
		public double Hours
		{
			get { return (double)GetValue(HoursProperty); }
			set { SetValue(HoursProperty, value); }
		}
		public static readonly DependencyProperty HoursProperty =
			DependencyProperty.Register("Hours", typeof(double), typeof(IndexTime), new PropertyMetadata(0d));
		/// <summary>
		/// Gets or sets a bindable value that indicates Percentage of current object
		/// </summary>
		public double Perc
		{
			get { return (double)GetValue(PercProperty); }
			set { SetValue(PercProperty, value); }
		}
		public static readonly DependencyProperty PercProperty =
			DependencyProperty.Register("Perc", typeof(double), typeof(IndexTime), new PropertyMetadata(0d));
		/// <summary>
		/// Gets or sets a bindable value that indicates Text
		/// </summary>
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(IndexTime), new PropertyMetadata(""));

		/// <summary>
		/// Gets a bindable collection of sub items for this item
		/// </summary>
		public ObservableCollection<IndexTime> SubItems { get { return _subItems; } }
		private ObservableCollection<IndexTime> _subItems = new ObservableCollection<IndexTime>();

		/// <summary>
		/// Gets or sets a bindable value that indicates ShowSubItems
		/// </summary>
		public bool ShowSubItems
		{
			get { return (bool)GetValue(ShowSubItemsProperty); }
			set { SetValue(ShowSubItemsProperty, value); }
		}
		public static readonly DependencyProperty ShowSubItemsProperty =
			DependencyProperty.Register("ShowSubItems", typeof(bool), typeof(IndexTime),
			new PropertyMetadata(false));

		/// <summary>
		/// Gets or sets a bindable value that indicates CurrentItem
		/// </summary>
		public IndexTime CurrentItem
		{
			get { return (IndexTime)GetValue(CurrentItemProperty); }
			set { SetValue(CurrentItemProperty, value); }
		}
		public static readonly DependencyProperty CurrentItemProperty =
			DependencyProperty.Register("CurrentItem", typeof(IndexTime), typeof(IndexTime), new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable command that handles the selection of this bar in OEE
		/// </summary>
		public Commands.Command SelectCommand
		{
			get { return (Commands.Command)GetValue(SelectCommandProperty); }
			set { SetValue(SelectCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectCommandProperty =
			DependencyProperty.Register("SelectCommand", typeof(Commands.Command), typeof(IndexTime), new PropertyMetadata(null));

	}
}
