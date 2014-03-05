using Soheil.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.SkillCenter
{
	public abstract class BaseSkillVm : DependencyObject
	{
		public event Action<BaseSkillVm> Saved;

		protected BaseSkillVm()
		{
			initializeCommands();
		}

		//Data Dependency Property
		public ILUO Data
		{
			get { return (ILUO)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register("Data", typeof(ILUO), typeof(BaseSkillVm), new UIPropertyMetadata(ILUO.N));

		void initializeCommands()
		{
			ChangeCommand = new Commands.Command(value =>
			{
				var previousData = Data;
				if (value == null) Data = ILUO.N;
				if (value is ILUO) Data = (ILUO)value;
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
				if (previousData != Data && Saved != null) Saved(this);
			});
		}
		//ChangeCommand Dependency Property
		public Commands.Command ChangeCommand
		{
			get { return (Commands.Command)GetValue(ChangeCommandProperty); }
			set { SetValue(ChangeCommandProperty, value); }
		}
		public static readonly DependencyProperty ChangeCommandProperty =
			DependencyProperty.Register("ChangeCommand", typeof(Commands.Command), typeof(BaseSkillVm), new UIPropertyMetadata(null));
	}
}
