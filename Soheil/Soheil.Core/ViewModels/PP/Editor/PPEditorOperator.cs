using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class PPEditorOperator : DependencyObject
	{
		#region Ctor
		/// <summary>
		/// Must be called within an EdmContext
		/// </summary>
		/// <param name="model"></param>
		public PPEditorOperator(Model.Operator model)
		{
			OperatorId = model.Id;
			ProcessOperatorId = -1;
			Name = model.Name;
			Code = model.Code;
		}
		/// <summary>
		/// Must be called within an EdmContext
		/// </summary>
		/// <param name="model"></param>
		public PPEditorOperator(Model.ProcessOperator model)
		{
			OperatorId = model.Operator.Id;
			ProcessOperatorId = model.Id;
			Name = model.Operator.Name;
			Code = model.Operator.Code;
			Role = model.Role;
			IsSelected = true;
		}
		public PPEditorOperator(PPEditorOperator sample)
		{
			OperatorId = sample.OperatorId;
			ProcessOperatorId = sample.ProcessOperatorId;
			Name = sample.Name;
			Code = sample.Code;
			IsSelected = sample.IsSelected;
		} 
		#endregion

		public int OperatorId { get; protected set; }
		public int ProcessOperatorId { get; set; }

		#region DpProps
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(PPEditorOperator), new UIPropertyMetadata(null));
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(PPEditorOperator), new UIPropertyMetadata(null));

		//IsSelected Dependency Property
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(PPEditorOperator),
			new UIPropertyMetadata(false/*, (d, e) => { if (((PPEditorOperator)d).IsSelectedChanged != null) ((PPEditorOperator)d).IsSelectedChanged(); }*/));
		/*		public Action IsSelectedChanged;*/

		//Role Dependency Property
		public OperatorRole Role
		{
			get { return (OperatorRole)GetValue(RoleProperty); }
			set { SetValue(RoleProperty, value); }
		}
		public static readonly DependencyProperty RoleProperty =
			DependencyProperty.Register("Role", typeof(OperatorRole), typeof(PPEditorOperator), new UIPropertyMetadata(OperatorRole.Main)); 
		#endregion
	}
}
