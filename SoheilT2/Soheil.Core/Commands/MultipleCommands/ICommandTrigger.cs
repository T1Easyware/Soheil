using System.Windows;

namespace Soheil.Core.Commands.MultipleCommands
{
	public interface ICommandTrigger
	{
		void Initialize(FrameworkElement source);
	}
}
