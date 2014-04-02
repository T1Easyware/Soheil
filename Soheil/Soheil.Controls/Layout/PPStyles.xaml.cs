using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Soheil.Common;
using System.Windows.Input;
using Soheil.Common.SoheilException;

namespace Soheil.Controls.Layout
{
	public partial class PPStyles
	{
		private void balloonClicked(object sender, MouseButtonEventArgs e)
		{
			var vm = sender.GetDataContext<EmbeddedException>();
			if (vm != null) vm.ResetEmbeddedException();
		}
	}
}
