using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	/// <summary>
	/// ViewModel for Product Defection used in Process reports
	/// </summary>
	public class DefectionVm : DependencyObject
	{
		/// <summary>
		/// Creates an instance of DefectionVm with the given ProductDefection model
		/// </summary>
		/// <param name="model">model can't be null (or its Defection)</param>
		public DefectionVm(Model.ProductDefection model)
		{
			Id = model.Defection.Id;
			ProductDefectionId = model.Id;
			Text = model.Defection.Name;
			IsG2 = model.Defection.IsG2;
		}
		/// <summary>
		/// Gets Defection Id
		/// </summary>
		public int Id { get; protected set; }
		/// <summary>
		/// Gets ProductDefection Id
		/// </summary>
		public int ProductDefectionId { get; protected set; }
		/// <summary>
		/// Gets a bindable value for Defection's Name
		/// </summary>
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			protected set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(DefectionVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets a bindable value that indicates if the Defection of this Vm represents a Grade 2
		/// <para>If false, the the defection is typical defection</para>
		/// </summary>
		public bool IsG2
		{
			get { return (bool)GetValue(IsG2Property); }
			protected set { SetValue(IsG2Property, value); }
		}
		public static readonly DependencyProperty IsG2Property =
			DependencyProperty.Register("IsG2", typeof(bool), typeof(DefectionVm), new UIPropertyMetadata(false));
	}
}
