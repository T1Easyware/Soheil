using System;
using System.ComponentModel.DataAnnotations;

namespace Soheil.Common
{
    [AttributeUsage(AttributeTargets.Property |
     AttributeTargets.Field, AllowMultiple = false)]
    public class LocalizedRequired : ValidationAttribute
    {
        private const string DefaultErrorMessage = "data is required";

        public LocalizedRequired()
            : base(DefaultErrorMessage)
        {
        }

        public override bool IsValid(object value)
        {
            ErrorMessage = Localization.LocalizationManager.DefaultResourceManager.GetString(ErrorMessageResourceName);
            if (value == null || String.IsNullOrWhiteSpace(value.ToString()))
                return false;
            return true;
        }
    }
}
