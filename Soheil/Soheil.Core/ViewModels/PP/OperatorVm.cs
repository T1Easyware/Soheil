using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP
{
	public abstract class OperatorVm : DependencyObject
	{
		internal Model.Operator OperatorModel { get; private set; }
		public int OperatorId { get { return OperatorModel.Id; } }

		#region Ctor
		/// <summary>
		/// Use this constructor to create an operator outside a process
		/// </summary>
		/// <param name="model"></param>
		protected OperatorVm(Model.Operator model, Model.StateStationActivity ssa)
		{
			OperatorModel = model;
			Name = model.Name;
			Code = model.Code;

			//find special skill
			var productRework = ssa.StateStation.State.OnProductRework;
			var specialSkill = productRework.ProductActivitySkills.FirstOrDefault(skill => skill.ActivitySkill.Operator.Id == model.Id);

			//find general skill
			var activity = ssa.Activity;
			var generalSkill = activity.ActivitySkills.FirstOrDefault(skill => skill.Operator.Id == model.Id);

			//set skill propdps
			if (generalSkill == null)
			{
				GeneralSkill = ILUO.N;
			}
			else
			{
				GeneralSkill = generalSkill.Iluo;
			}
			if (specialSkill == null || specialSkill.Iluo == ILUO.N)
			{
				SpecialSkill = ILUO.N;
				EffectiveSkill = GeneralSkill;
			}
			else
			{
				SpecialSkill = specialSkill.Iluo;
				EffectiveSkill = specialSkill.Iluo;
			}
		}
		/// <summary>
		/// Use this constructor to create an operator inside a process
		/// </summary>
		/// <param name="model"></param>
		protected OperatorVm(Model.ProcessOperator model)
			:this(model.Operator, model.Process.StateStationActivity)
		{
			Role = model.Role;
		}
		public OperatorVm(Model.Operator model)
		{
			OperatorModel = model;
			Name = model.Name;
			Code = model.Code;
		}
		#endregion



		#region DpProps
		/// <summary>
		/// Gets a bindable value for operator name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			private set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(OperatorVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets a bindable value for operator code
		/// </summary>
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			private set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(OperatorVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable value for operator role in a process
		/// </summary>
		public OperatorRole Role
		{
			get { return (OperatorRole)GetValue(RoleProperty); }
			set { SetValue(RoleProperty, value); }
		}
		public static readonly DependencyProperty RoleProperty =
			DependencyProperty.Register("Role", typeof(OperatorRole), typeof(OperatorVm), new UIPropertyMetadata(OperatorRole.Main));

		/// <summary>
		/// Gets a bindable value for General skill level of this operator in the specified activity
		/// </summary>
		public ILUO GeneralSkill
		{
			get { return (ILUO)GetValue(GeneralSkillProperty); }
			private set { SetValue(GeneralSkillProperty, value); }
		}
		public static readonly DependencyProperty GeneralSkillProperty =
			DependencyProperty.Register("GeneralSkill", typeof(ILUO), typeof(OperatorVm), new UIPropertyMetadata(ILUO.N));

		/// <summary>
		/// Gets a bindable value for Special skill level of this operator in the specified activity in the specified product rework
		/// </summary>
		public ILUO SpecialSkill
		{
			get { return (ILUO)GetValue(SpecialSkillProperty); }
			private set { SetValue(SpecialSkillProperty, value); }
		}
		public static readonly DependencyProperty SpecialSkillProperty =
			DependencyProperty.Register("SpecialSkill", typeof(ILUO), typeof(OperatorVm),
			new UIPropertyMetadata(ILUO.N));

		/// <summary>
		/// Gets a bindable value for effective skill level of this operator in the specified activity in the specified product rework
		/// <para>The value would be equal to Special skill if higher than N, otherwise equal to General skill</para>
		/// </summary>
		public ILUO EffectiveSkill
		{
			get { return (ILUO)GetValue(EffectiveSkillProperty); }
			private set { SetValue(EffectiveSkillProperty, value); }
		}
		public static readonly DependencyProperty EffectiveSkillProperty =
			DependencyProperty.Register("EffectiveSkill", typeof(ILUO), typeof(OperatorVm), new UIPropertyMetadata(ILUO.N));
		#endregion
	}
}
