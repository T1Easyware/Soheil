using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// A <see cref="BaseTreeItemVm"/> that represents the product mode in skill center
	/// <para>When an instance of this type is selected in the skill center tree, product activity skills of operators will be in effect</para>
	/// </summary>
	/// <remarks>
	/// <para>The values that are shown in cells of the table will be a single ILUO value of all underlying product reworks of the product represented by this Vm</para>
	/// <para>If the underlying values are not the same a specific symbol will be shown</para>
	/// <para>Changing the ILUO value in cells of the table will cause an overwrite to all underlying product reworks of the product represented by this Vm</para>
	/// </remarks>
	public class ProductVm : BaseTreeItemVm
	{
		/// <summary>
		/// Creates an instance of ProductVm with the given model and fills it with its underlying ProductReworks
		/// </summary>
		/// <param name="model">Id, Name, Code, Color and underlying ProductReworks of the model are used</param>
		public ProductVm(Model.Product model)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
			Color = model.Color;
			foreach (var pr in model.ProductReworks)
			{
				AddChild(new ProductReworkVm(pr));
			}
			InitializeCommands();
		}
		
		/// <summary>
		/// Gets a bindable value for Color of the product
		/// </summary>
		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(Color), typeof(ProductVm), new UIPropertyMetadata(Colors.White));
	}
}
