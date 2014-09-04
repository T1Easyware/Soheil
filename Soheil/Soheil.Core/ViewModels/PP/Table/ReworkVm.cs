using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.PP
{
	public class ReworkVm : DependencyObject
	{
		public ReworkVm(Model.Rework model)
		{
			if (model == null)
			{
				Id = -1;
				Name = "تولید عادی";
				IsRework = false;
			}
			else
			{
				Id = model.Id;
				Name = model.Name;
				Code = model.Code;
				IsRework = true;
			}
		}
		public int Id { get; protected set; }
		public bool IsRework { get; protected set; }
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(ReworkVm), new UIPropertyMetadata(null));
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(ReworkVm), new UIPropertyMetadata(null));
	}
}
