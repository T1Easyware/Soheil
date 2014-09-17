using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soheil.Core.ViewModels.Fpc.ListItems
{
	public class FpcVm : DependencyObject
	{
		#region Properties and Events

		public int Id { get; private set; }
		/// <summary>
		/// Gets or sets a bindable value that indicates Name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(FpcVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates Code
		/// </summary>
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(FpcVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates IsDefault
		/// </summary>
		public bool IsDefault
		{
			get { return (bool)GetValue(IsDefaultProperty); }
			set { SetValue(IsDefaultProperty, value); }
		}
		public static readonly DependencyProperty IsDefaultProperty =
			DependencyProperty.Register("IsDefault", typeof(bool), typeof(FpcVm),
			new UIPropertyMetadata(false));

		#endregion

		#region Ctor and Init
		public FpcVm(Model.FPC model)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
			IsDefault = model.IsDefault;
		}
		#endregion

		#region Methods
		#endregion

		#region Commands
		#endregion
	}
}
