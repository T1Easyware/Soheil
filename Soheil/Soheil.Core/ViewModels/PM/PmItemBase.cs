using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PM
{
	public class PmItemBase : DependencyObject
	{
		/// <summary>
		/// Gets or sets a bindable value that indicates Name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(PmItemBase), new PropertyMetadata(""));
		/// <summary>
		/// Gets or sets a bindable value that indicates Code
		/// </summary>
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(PmItemBase), new PropertyMetadata(""));
		/// <summary>
		/// Gets or sets a bindable value that indicates Description
		/// </summary>
		public string Description
		{
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		public static readonly DependencyProperty DescriptionProperty =
			DependencyProperty.Register("Description", typeof(string), typeof(PmItemBase), new PropertyMetadata(""));
		/// <summary>
		/// Gets or sets a bindable value that indicates Status
		/// </summary>
		public Status Status
		{
			get { return (Status)GetValue(StatusProperty); }
			set { SetValue(StatusProperty, value); }
		}
		public static readonly DependencyProperty StatusProperty =
			DependencyProperty.Register("Status", typeof(Status), typeof(PmItemBase), new PropertyMetadata(Status.Active));



		/// <summary>
		/// Gets or sets a bindable value that indicates SelectCommand
		/// </summary>
		public Commands.Command SelectCommand
		{
			get { return (Commands.Command)GetValue(SelectCommandProperty); }
			set { SetValue(SelectCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectCommandProperty =
			DependencyProperty.Register("SelectCommand", typeof(Commands.Command), typeof(PmItemBase), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates DeleteCommand
		/// </summary>
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(PmItemBase), new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable command that adds this item to collection of a previous page
		/// </summary>
		public Commands.Command UseCommand
		{
			get { return (Commands.Command)GetValue(UseCommandProperty); }
			set { SetValue(UseCommandProperty, value); }
		}
		public static readonly DependencyProperty UseCommandProperty =
			DependencyProperty.Register("UseCommand", typeof(Commands.Command), typeof(PmItemBase), new PropertyMetadata(null));
	}
}
