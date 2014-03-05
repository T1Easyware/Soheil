using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Common.SoheilException
{
	public class EmbeddedException : DependencyObject
	{
		//HasException Dependency Property
		public bool HasException
		{
			get { return (bool)GetValue(HasExceptionProperty); }
			set { SetValue(HasExceptionProperty, value); }
		}
		public static readonly DependencyProperty HasExceptionProperty =
			DependencyProperty.Register("HasException", typeof(bool), typeof(EmbeddedException), new UIPropertyMetadata(false));
		//MainText Dependency Property
		public string MainExceptionText
		{
			get { return (string)GetValue(MainExceptionTextProperty); }
			set { SetValue(MainExceptionTextProperty, value); }
		}
		public static readonly DependencyProperty MainExceptionTextProperty =
			DependencyProperty.Register("MainExceptionText", typeof(string), typeof(EmbeddedException), new UIPropertyMetadata(null));
		//FullText Dependency Property
		public string FullExceptionText
		{
			get { return (string)GetValue(FullExceptionTextProperty); }
			set { SetValue(FullExceptionTextProperty, value); }
		}
		public static readonly DependencyProperty FullExceptionTextProperty =
			DependencyProperty.Register("FullExceptionText", typeof(string), typeof(EmbeddedException), new UIPropertyMetadata(null));

		public void AddEmbeddedException(string text) {
			HasException = true;
			MainExceptionText = text;
			if (!string.IsNullOrWhiteSpace(FullExceptionText))
				FullExceptionText += "\n";
			FullExceptionText += text;
		}
		public void ResetEmbeddedException()
		{
			HasException = false;
			MainExceptionText = string.Empty;
			FullExceptionText = string.Empty;
		}
	}
}
