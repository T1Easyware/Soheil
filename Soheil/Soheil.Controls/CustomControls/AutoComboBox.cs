using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Soheil.Controls.CustomControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Soheil.Controls.CustomControls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Soheil.Controls.CustomControls;assembly=Soheil.Controls.CustomControls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ImageButton/>
    ///
    /// </summary>
    [TemplatePart(Name = "PART_EditableTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    public class AutoComboBox : ComboBox
    {
        private TextBox _textBox;
        private Popup _popup;

        static AutoComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoComboBox), new FrameworkPropertyMetadata(typeof(AutoComboBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textBox = GetTemplateChild("PART_EditableTextBox") as TextBox;
            _popup = GetTemplateChild("PART_Popup") as Popup;


            if (_textBox != null)
            {
                _textBox.TextChanged += delegate
                {
                    _popup.IsOpen = true;
                    Items.Filter += a =>
                    {
                        if (a.ToString().Contains(_textBox.Text))
                        {
                            return true;
                        }
                        return false;
                    };
                };
            }
        }
    }
}
