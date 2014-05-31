using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class TaskEditorHolderVm : DependencyObject
	{
		public event Action TaskCreated;

		public TaskEditorHolderVm()
		{
			CreateNewTaskCommand = new Commands.Command(o =>
			{
				if (TaskCreated != null) TaskCreated();
			});
		}
		//CreateNewTaskCommand Dependency Property
		public Commands.Command CreateNewTaskCommand
		{
			get { return (Commands.Command)GetValue(CreateNewTaskCommandProperty); }
			set { SetValue(CreateNewTaskCommandProperty, value); }
		}
		public static readonly DependencyProperty CreateNewTaskCommandProperty =
			DependencyProperty.Register("CreateNewTaskCommand", typeof(Commands.Command), typeof(TaskEditorHolderVm), new UIPropertyMetadata(null));
	}
}
