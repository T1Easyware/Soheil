using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class CauseVm : DependencyObject
	{
		public CauseVm(Model.Cause model)
		{
			Id = model.Id;
			Text = model.Name;
			Code = string.Format("{0:D2}", model.Code);
			ChildrenModels = new List<Model.Cause>();
			ChildrenModels.AddRange(model.Children);
		}
		/// <summary>
		/// Cause Id
		/// </summary>
		public int Id { get; set; }
		public List<Model.Cause> ChildrenModels { get; set; }

		//Text Dependency Property
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(CauseVm), new UIPropertyMetadata(null));
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(CauseVm), new UIPropertyMetadata(null));
	}
}
