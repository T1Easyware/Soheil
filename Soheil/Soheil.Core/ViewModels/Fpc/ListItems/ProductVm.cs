using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.Fpc.ListItems
{
	public class ProductVm : DependencyObject
	{
		#region Properties and Events
		public event Action<FpcVm> SelectionChanged;

		public int Id { get; set; }
		/// <summary>
		/// Gets or sets a bindable value that indicates Name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(ProductVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates Code
		/// </summary>
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(ProductVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates Color
		/// </summary>
		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(Color), typeof(ProductVm), new UIPropertyMetadata(Colors.Transparent));

		/// <summary>
		/// Gets or sets a bindable collection of Fpcs
		/// </summary>
		public ObservableCollection<FpcVm> Fpcs { get { return _fpcs; } }
		private ObservableCollection<FpcVm> _fpcs = new ObservableCollection<FpcVm>();
		/// <summary>
		/// Gets or sets a bindable value that indicates IsExpanded
		/// </summary>
		public bool IsExpanded
		{
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public static readonly DependencyProperty IsExpandedProperty =
			DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ProductVm), new UIPropertyMetadata(false));
		/// <summary>
		/// Gets or sets a bindable value that indicates SelectedFpc
		/// </summary>
		public FpcVm SelectedFpc
		{
			get { return (FpcVm)GetValue(SelectedFpcProperty); }
			set { SetValue(SelectedFpcProperty, value); }
		}
		public static readonly DependencyProperty SelectedFpcProperty =
			DependencyProperty.Register("SelectedFpc", typeof(FpcVm), typeof(ProductVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (ProductVm)d;
				var val = (FpcVm)e.NewValue;
				if (val == null)
				{
					vm.IsExpanded = false;
					return;
				}
				vm.IsExpanded = true;
				if (vm.SelectionChanged != null)
				{
					vm.SelectionChanged(val);
				}
			}));
		#endregion

		#region Ctor and Init
		public ProductVm(Model.Product model)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
			Color = model.Color;
			foreach (var fpc in model.FPCs)
			{
				var fpcVm = new FpcVm(fpc);
				Fpcs.Add(fpcVm);
			}
		}

		#endregion

		#region Methods
		#endregion

		#region Commands
		/// <summary>
		/// Gets or sets a bindable value that indicates AddCommand
		/// </summary>
		public Commands.Command AddCommand
		{
			get { return (Commands.Command)GetValue(AddCommandProperty); }
			set { SetValue(AddCommandProperty, value); }
		}
		public static readonly DependencyProperty AddCommandProperty =
			DependencyProperty.Register("AddCommand", typeof(Commands.Command), typeof(ProductVm), new UIPropertyMetadata(null));
		#endregion
	}
}
