using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Common.SoheilException
{
	public class SoheilExceptionBase : Exception
	{
		static string[] fa = new string[] { string.Empty, "توجه", "اخطار", "خطا", "توجه" };
		static string[] en = new string[] { string.Empty, "Attention", "Warning", "Error", "Attention" };

		public SoheilExceptionBase(string message, ExceptionLevel level, string caption = "")
			: base(message)
		{
			Level = level;
			_captionInfo = caption;
		}
		public ExceptionLevel Level { get; set; }
		string _captionInfo;

		public string Caption { get { return string.Format("{0} - {1}", _captionInfo, /*???*/fa[(int)Level]); } }
	}
}
