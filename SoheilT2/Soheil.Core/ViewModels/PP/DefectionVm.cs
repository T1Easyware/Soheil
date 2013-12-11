using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class DefectionVm : DependencyObject
	{
		public DefectionVm(Model.ProductDefection model)
		{
			Id = model.Defection.Id;
			ProductDefectionId = model.Id;
			Text = model.Defection.Name;
			IsG2 = model.Defection.IsG2;
		}
		/// <summary>
		/// Defection Id
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// ProductDefection Id
		/// </summary>
		public int ProductDefectionId { get; set; }
		//Text Dependency Property
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(DefectionVm), new UIPropertyMetadata(null));
		//IsG2 Dependency Property
		public bool IsG2
		{
			get { return (bool)GetValue(IsG2Property); }
			set { SetValue(IsG2Property, value); }
		}
		public static readonly DependencyProperty IsG2Property =
			DependencyProperty.Register("IsG2", typeof(bool), typeof(DefectionVm), new UIPropertyMetadata(false));
	}
}
