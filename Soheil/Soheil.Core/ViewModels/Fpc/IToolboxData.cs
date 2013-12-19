using Soheil.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	public interface IToolboxData
	{
		int Id { get; }
		string Name { get; set; }
	}
}
