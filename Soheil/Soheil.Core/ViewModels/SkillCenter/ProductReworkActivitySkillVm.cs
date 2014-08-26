﻿using Soheil.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// One cell in SkillCenter table when in ProductRework state, representing a single <see cref="Soheil.Model.ProductActivitySkill"/>
	/// </summary>
	public class ProductReworkActivitySkillVm : BaseSkillVm
	{
		/// <summary>
		/// Gets the model for this Vm
		/// </summary>
		public Model.ProductActivitySkill Model { get; set; }

		public int ProductReworkId { get; set; }

		/// <summary>
		/// Creates an instance of ProductReworkActivitySkillVm with the given model and initializes the commands
		/// </summary>
		/// <param name="model">Model and its ILUO value are used</param>
		public ProductReworkActivitySkillVm(Model.ProductActivitySkill model, Model.ActivitySkill general, int operatorId, int activityId, int productReworkId)
			: base(operatorId, activityId)
		{
			Data = model == null ? ILUO.NA : model.Iluo;
			GeneralData = general == null ? ILUO.NA : general.Iluo;
			ProductReworkId = productReworkId;
			Model = model;
		}
		//GeneralData Dependency Property
		public ILUO GeneralData
		{
			get { return (ILUO)GetValue(GeneralDataProperty); }
			set { SetValue(GeneralDataProperty, value); }
		}
		public static readonly DependencyProperty GeneralDataProperty =
			DependencyProperty.Register("GeneralData", typeof(ILUO), typeof(ProductReworkActivitySkillVm), new UIPropertyMetadata(ILUO.NA));
	}
}
