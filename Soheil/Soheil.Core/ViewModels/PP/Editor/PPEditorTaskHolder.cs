using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class PPEditorTaskHolder : DependencyObject
	{
		public PPEditorTaskHolder(PPEditorBlock parent)
		{
			CreateNewTaskCommand = new Commands.Command(o =>
			{
				try
				{
					parent.InsertTask();
				}
				catch (Soheil.Common.SoheilException.RoutedException ex)
				{
					parent.Message.AddEmbeddedException(ex.Message);
				}
			});
		}
		//CreateNewTaskCommand Dependency Property
		public Commands.Command CreateNewTaskCommand
		{
			get { return (Commands.Command)GetValue(CreateNewTaskCommandProperty); }
			set { SetValue(CreateNewTaskCommandProperty, value); }
		}
		public static readonly DependencyProperty CreateNewTaskCommandProperty =
			DependencyProperty.Register("CreateNewTaskCommand", typeof(Commands.Command), typeof(PPEditorTaskHolder), new UIPropertyMetadata(null));
	}
}
