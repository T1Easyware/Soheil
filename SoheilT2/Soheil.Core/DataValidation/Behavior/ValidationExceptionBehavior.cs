using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Soheil.Core.DataValidation.Behavior
{
    /// <summary>
    /// A simple behavior that can transfer the number of validation error with exceptions
    /// to a ViewModel which supports the INotifyValidationException interface
    /// </summary>
    public class ValidationExceptionBehavior : Behavior<FrameworkElement>
    {
        private int _validationExceptionCount;

        protected override void OnAttached()
        {
            AssociatedObject.AddHandler(Validation.ErrorEvent,
                                        new EventHandler<ValidationErrorEventArgs>(OnValidationError));
        }

        private void OnValidationError(object sender, ValidationErrorEventArgs e)
        {
            // we want to count only the validation error with an exception
            // other error are handled by using the attribute on the properties
            if (e.Error.Exception == null)
            {
                return;
            }

            if (e.Action == ValidationErrorEventAction.Added)
            {
                _validationExceptionCount++;
            }
            else
            {
                _validationExceptionCount--;
            }

            if (AssociatedObject.DataContext is IValidationExceptionHandler)
            {
                // transfer the information back to the viewmodel
                var viewModel = (IValidationExceptionHandler) AssociatedObject.DataContext;
                viewModel.ValidationExceptionsChanged(_validationExceptionCount);
            }
        }
    }
}