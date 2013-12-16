using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Common.SoheilException
{
	public class RoutedException : SoheilExceptionBase
	{
		public RoutedException(string message, ExceptionLevel level, Object target)
			: base(message, level)
		{
			Target = target;
		}
		public Object Target { get; set; }
	}
}
