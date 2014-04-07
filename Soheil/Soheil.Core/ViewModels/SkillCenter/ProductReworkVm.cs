using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// A <see cref="BaseTreeItemVm"/> that represents the product rework mode in skill center
	/// <para>When an instance of this type is selected in the skill center tree, product activity skills of operators will be in effect</para>
	/// </summary>
	public class ProductReworkVm : BaseTreeItemVm
	{
		/// <summary>
		/// Creates an instance of this Vm with the given model
		/// </summary>
		/// <param name="model">Id, Name, Code and Rework of this model are used</param>
		/// <remarks>If rework is null, Name will be Main and Code will be model's Code
		/// <para>Otherwise Name and Code will be set to Rework's</para></remarks>
		public ProductReworkVm(Model.ProductRework model)
		{
			Id = model.Id;
			if(model.Rework == null)
			{
				IsMainProduct = true;
				Name = "تولید اصلی";
				Code = model.Code;
			}
			else
			{
				IsMainProduct = false;
				Name = model.Rework.Name;
				Code = model.Rework.Code;
			}
			InitializeCommands();
		}

		/// <summary>
		/// Gets a bindable value that indicates if this Vm presents a main product rework (with rework = null)
		/// </summary>
		public bool IsMainProduct
		{
			get { return (bool)GetValue(IsMainProductProperty); }
			protected set { SetValue(IsMainProductProperty, value); }
		}
		public static readonly DependencyProperty IsMainProductProperty =
			DependencyProperty.Register("IsMainProduct", typeof(bool), typeof(ProductReworkVm), new UIPropertyMetadata(true));
	}
}
