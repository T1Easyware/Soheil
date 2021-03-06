﻿using Soheil.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// One cell in SkillCenter table, representing an <see cref="ActivitySkillVm"/>, <see cref="ProductGroupActivitySkillVm"/>, <see cref="ProductActivitySkillVm"/> or <see cref="ProductReworkActivitySkillVm"/>
	/// </summary>
	public abstract class BaseSkillVm : DependencyObject
	{
		/// <summary>
		/// Occurs when user changes the ILUO value of this Vm (when ChangeCommand is fired)
		/// </summary>
		public event Action<BaseSkillVm> IluoChanged;

		/// <summary>
		/// Initializes an instance of BaseSkillVm (initializes the commands)
		/// </summary>
		protected BaseSkillVm()
		{
			initializeCommands();
		}

		/// <summary>
		/// Gets or sets the bindable ILUO value of this Vm
		/// </summary>
		public ILUO Data
		{
			get { return (ILUO)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register("Data", typeof(ILUO), typeof(BaseSkillVm), new UIPropertyMetadata(ILUO.N));

		/// <summary>
		/// Initializes the command(s)
		/// </summary>
		void initializeCommands()
		{
			ChangeCommand = new Commands.Command(value =>
			{
				//memorize the previous data
				var previousData = Data;

				//finds the new value for ILUO in any way possible

				//command parameter is null
				if (value == null) Data = ILUO.N;

				//command parameter is enum
				if (value is ILUO) Data = (ILUO)value;

				//command parameter is string
				else if (value is string)
				{
					if (string.IsNullOrWhiteSpace((string)value)) Data = ILUO.N;
					var c = ((string)value).ToUpper()[0];
					switch (c)
					{
						case 'N': Data = ILUO.N; break;
						case 'I': Data = ILUO.I; break;
						case 'L': Data = ILUO.L; break;
						case 'U': Data = ILUO.U; break;
						case 'O': Data = ILUO.O; break;
						default: Data = ILUO.N; break;
					}
				}

				//if any change to Data occurs fire the IluoChanged event
				if (previousData != Data && IluoChanged != null)
					IluoChanged(this);
			});
		}
		
		/// <summary>
		/// Gets a bindable command that indicates when Data is changed
		/// </summary>
		public Commands.Command ChangeCommand
		{
			get { return (Commands.Command)GetValue(ChangeCommandProperty); }
			protected set { SetValue(ChangeCommandProperty, value); }
		}
		public static readonly DependencyProperty ChangeCommandProperty =
			DependencyProperty.Register("ChangeCommand", typeof(Commands.Command), typeof(BaseSkillVm), new UIPropertyMetadata(null));
	}
}
