using Soheil.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	/// <summary>
	/// Interface for toolbox item or group, which contains Id and Name
	/// </summary>
	public interface IToolboxData
	{
		int Id { get; }
		string Name { get; set; }
	}
}
