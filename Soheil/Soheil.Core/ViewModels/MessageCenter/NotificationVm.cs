using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Common;

namespace Soheil.Core.ViewModels.MessageCenter
{
	public class NotificationVm : DependencyObject
	{
		/// <summary>
		/// Gets or sets a bindable value that indicates Message
		/// </summary>
		public string Message
		{
			get { return (string)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(string), typeof(NotificationVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates MoreInfo
		/// </summary>
		public string MoreInfo
		{
			get { return (string)GetValue(MoreInfoProperty); }
			set { SetValue(MoreInfoProperty, value); }
		}
		public static readonly DependencyProperty MoreInfoProperty =
			DependencyProperty.Register("MoreInfo", typeof(string), typeof(NotificationVm), new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable value that indicates NotificationType
		/// </summary>
		public NotificationType NotificationType
		{
			get { return (NotificationType)GetValue(NotificationTypeProperty); }
			set { SetValue(NotificationTypeProperty, value); }
		}
		public static readonly DependencyProperty NotificationTypeProperty =
			DependencyProperty.Register("NotificationType", typeof(NotificationType), typeof(NotificationVm), new PropertyMetadata(NotificationType.Info));
		/// <summary>
		/// Gets or sets a bindable value that indicates OpenCommand
		/// </summary>
		public Commands.Command OpenCommand
		{
			get { return (Commands.Command)GetValue(OpenCommandProperty); }
			set { SetValue(OpenCommandProperty, value); }
		}
		public static readonly DependencyProperty OpenCommandProperty =
			DependencyProperty.Register("OpenCommand", typeof(Commands.Command), typeof(NotificationVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates IsSeparator
		/// </summary>
		public bool IsSeparator
		{
			get { return (bool)GetValue(IsSeparatorProperty); }
			set { SetValue(IsSeparatorProperty, value); }
		}
		public static readonly DependencyProperty IsSeparatorProperty =
			DependencyProperty.Register("IsSeparator", typeof(bool), typeof(NotificationVm), new PropertyMetadata(false));

	}
}
