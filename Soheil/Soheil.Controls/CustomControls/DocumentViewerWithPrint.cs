using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Controls.CustomControls
{
	public class DocumentViewerWithPrint : System.Windows.Controls.DocumentViewer
	{
		public event Action<DocumentViewerWithPrint> PreviewPrint;
		protected override void OnPrintCommand()
		{
			if (PreviewPrint != null) PreviewPrint(this);
			base.OnPrintCommand();
		}
	}
}
