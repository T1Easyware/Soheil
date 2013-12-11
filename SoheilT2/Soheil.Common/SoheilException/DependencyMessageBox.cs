using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Soheil.Common.SoheilException
{
	public class DependencyMessageBox : DependencyObject
	{
		public DependencyMessageBox() { IsEnabled = false; }
		public DependencyMessageBox(
			string message, 
			string caption = "Error", 
			MessageBoxButton buttons = MessageBoxButton.OK,
			ExceptionLevel icon = ExceptionLevel.Error)
		{
			Message = message;
			Caption = caption;
			if (buttons == MessageBoxButton.OK)
			{
				ButtonList.Add(Button.OK(this));
				DefaultButton = ButtonList.First();
			}
			else Buttons = buttons;
			Icon = icon;
			IsEnabled = true;
		}
		public DependencyMessageBox(
			Exception exception,
			string message = "",
			string caption = "Error",
			MessageBoxButton buttons = MessageBoxButton.OK,
			ExceptionLevel icon = ExceptionLevel.Error)
		{
			if (!string.IsNullOrWhiteSpace(message)) Message = message + "\n" + exception.Message;
			else Message = exception.Message;
			if (exception.InnerException != null) Message += ("\n" + exception.InnerException.Message);
			Caption = caption;
			if (buttons == MessageBoxButton.OK)
			{
				ButtonList.Add(Button.OK(this));
				DefaultButton = ButtonList.First();
			}
			else Buttons = buttons;
			Icon = icon;
			IsEnabled = true;
		}
		#region Props
		//IsEnabled Dependency Property
		public bool IsEnabled
		{
			get { return (bool)GetValue(IsEnabledProperty); }
			set { SetValue(IsEnabledProperty, value); }
		}
		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.Register("IsEnabled", typeof(bool), typeof(DependencyMessageBox), new UIPropertyMetadata(false));
		//Message Dependency Property
		public string Message
		{
			get { return (string)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(string), typeof(DependencyMessageBox), new UIPropertyMetadata("Unknown Error."));
		//Caption Dependency Property
		public string Caption
		{
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		public static readonly DependencyProperty CaptionProperty =
			DependencyProperty.Register("Caption", typeof(string), typeof(DependencyMessageBox), new UIPropertyMetadata("Error"));
		//Icon Dependency Property
		public ExceptionLevel Icon
		{
			get { return (ExceptionLevel)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}
		public static readonly DependencyProperty IconProperty =
			DependencyProperty.Register("Icon", typeof(ExceptionLevel), typeof(DependencyMessageBox), new UIPropertyMetadata(ExceptionLevel.None));
		//Buttons Dependency Property
		public MessageBoxButton Buttons
		{
			get { return (MessageBoxButton)GetValue(ButtonsProperty); }
			set { SetValue(ButtonsProperty, value); }
		}
		public static readonly DependencyProperty ButtonsProperty =
			DependencyProperty.Register("Buttons", typeof(MessageBoxButton), typeof(DependencyMessageBox),
			new UIPropertyMetadata(MessageBoxButton.OK, (d, e) =>
			{
				var dmb = (DependencyMessageBox)d;
				switch ((MessageBoxButton)e.NewValue)
				{
					case MessageBoxButton.OK:
						dmb.ButtonList.Add(Button.OK(dmb));
						dmb.DefaultButton = dmb.ButtonList.First();
						break;
					case MessageBoxButton.OKCancel:
						dmb.ButtonList.Add(Button.OK(dmb));
						dmb.ButtonList.Add(Button.Cancel(dmb));
						dmb.DefaultButton = dmb.ButtonList.First();
						break;
					case MessageBoxButton.YesNo:
						dmb.ButtonList.Add(Button.Yes(dmb));
						dmb.ButtonList.Add(Button.No(dmb));
						dmb.DefaultButton = dmb.ButtonList.First();
						break;
					case MessageBoxButton.YesNoCancel:
						dmb.ButtonList.Add(Button.Yes(dmb));
						dmb.ButtonList.Add(Button.No(dmb));
						dmb.ButtonList.Add(Button.Cancel(dmb));
						dmb.DefaultButton = dmb.ButtonList.First();
						break;
					default:
						dmb.ButtonList.Add(new Button(dmb) { Text = "", Result = MessageBoxResult.OK });
						dmb.DefaultButton = dmb.ButtonList.First();
						break;
				}
			}));
		//ButtonList Observable Collection
		private ObservableCollection<Button> _buttonList = new ObservableCollection<Button>();
		public ObservableCollection<Button> ButtonList { get { return _buttonList; } }
		public Button DefaultButton { get; set; }
		//Result Dependency Property
		public MessageBoxResult Result
		{
			get { return (MessageBoxResult)GetValue(ResultProperty); }
			set { SetValue(ResultProperty, value); }
		}
		public static readonly DependencyProperty ResultProperty =
			DependencyProperty.Register("Result", typeof(MessageBoxResult), typeof(DependencyMessageBox), new UIPropertyMetadata(MessageBoxResult.OK)); 
		#endregion



		//				ClickCommand

		public class ClickCommand : ICommand
		{
			Button _button;
			DependencyMessageBox _messageBox;
			public ClickCommand(Button button, DependencyMessageBox messageBox)
			{
				_button = button;
				_messageBox = messageBox;
			}
			public bool CanExecute(object parameter) { return true; }
			public event EventHandler CanExecuteChanged;
			public void Execute(object parameter)
			{
				_messageBox.Result = _button.Result;
				_messageBox.IsEnabled = false;
			}
		}

		//				Button

		public class Button : DependencyObject
		{
			public Button(DependencyMessageBox parent)
			{
				Clicked = new ClickCommand(this, parent);
			}
			//Clicked Dependency Property
			public ClickCommand Clicked
			{
				get { return (ClickCommand)GetValue(ClickedProperty); }
				set { SetValue(ClickedProperty, value); }
			}
			public static readonly DependencyProperty ClickedProperty =
				DependencyProperty.Register("Clicked", typeof(ClickCommand), typeof(Button), new UIPropertyMetadata(null));
			//Text Dependency Property
			public string Text
			{
				get { return (string)GetValue(TextProperty); }
				set { SetValue(TextProperty, value); }
			}
			public static readonly DependencyProperty TextProperty =
				DependencyProperty.Register("Text", typeof(string), typeof(Button), new UIPropertyMetadata("OK"));
			//Result Dependency Property
			public MessageBoxResult Result
			{
				get { return (MessageBoxResult)GetValue(ResultProperty); }
				set { SetValue(ResultProperty, value); }
			}
			public static readonly DependencyProperty ResultProperty =
				DependencyProperty.Register("Result", typeof(MessageBoxResult), typeof(Button), new UIPropertyMetadata(MessageBoxResult.OK));

			internal static Button OK(DependencyMessageBox parent)		{ return new Button(parent) { Text = "OK",		Result = MessageBoxResult.OK }; }
			internal static Button Cancel(DependencyMessageBox parent)	{ return new Button(parent) { Text = "Cancel",	Result = MessageBoxResult.Cancel }; }
			internal static Button Yes(DependencyMessageBox parent)		{ return new Button(parent) { Text = "Yes",		Result = MessageBoxResult.Yes }; }
			internal static Button No(DependencyMessageBox parent)		{ return new Button(parent) { Text = "No",		Result = MessageBoxResult.No }; }
		}
	}
}
