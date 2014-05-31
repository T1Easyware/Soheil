using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Commands;
using Soheil.Core.DataValidation.Behavior;
using Soheil.Core.Interfaces;

namespace Soheil.Core.Base
{
    public abstract class EntityObjectBase : ViewModelBase, IEntityObject, IDataErrorInfo,
                                                 IValidationExceptionHandler
    {
        #region Implementation of DataValidations

        private readonly Dictionary<string, Func<EntityObjectBase, object>> _propertyGetters;
        private readonly Dictionary<string, ValidationAttribute[]> _validators;
        private int _validationExceptionCount;

        protected EntityObjectBase(AccessType access)
        {
            _validators = GetType()
                .GetProperties()
                .Where(p => GetValidations(p).Length != 0)
                .ToDictionary(p => p.Name, GetValidations);

            _propertyGetters = GetType()
                .GetProperties()
                .Where(p => GetValidations(p).Length != 0)
                .ToDictionary(p => p.Name, GetValueGetter);

            Access = access;
        }

        /// <summary>
        /// Gets the number of properties which have a validation attribute and are currently valid
        /// </summary>
        public int ValidPropertiesCount
        {
            get
            {
                IEnumerable<KeyValuePair<string, ValidationAttribute[]>> query = from validator in _validators
                                                                                 where
                                                                                     validator.Value.All(
                                                                                         attribute =>
                                                                                         attribute.IsValid(
                                                                                             _propertyGetters[
                                                                                                 validator.Key](this)))
                                                                                 select validator;

                int count = query.Count() - _validationExceptionCount;
                return count;
            }
        }

        /// <summary>
        /// Gets the number of properties which have a validation attribute
        /// </summary>
        public int TotalPropertiesWithValidationCount
        {
            get { return _validators.Count(); }
        }

        #region IDataErrorInfo Members

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        public string this[string propertyName]
        {
            get
            {
                if (_propertyGetters.ContainsKey(propertyName))
                {
                    object propertyValue = _propertyGetters[propertyName](this);
                    string[] errorMessages = _validators[propertyName]
                        .Where(v => !v.IsValid(propertyValue))
                        .Select(v => v.ErrorMessage).ToArray();

                    return string.Join(Environment.NewLine, errorMessages);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        public string Error
        {
            get
            {
                IEnumerable<string> errors = from validator in _validators
                                             from attribute in validator.Value
                                             where !attribute.IsValid(_propertyGetters[validator.Key](this))
                                             select attribute.ErrorMessage;

                return string.Join(Environment.NewLine, errors.ToArray());
            }
        }

        #endregion

        #region IValidationExceptionHandler Members

        public void ValidationExceptionsChanged(int count)
        {
            _validationExceptionCount = count;
            OnPropertyChanged("ValidPropertiesCount");
        }

        #endregion

        public bool AllDataValid()
        {
            if (_validationExceptionCount != 0)
                return false;
            return
                _validators.All(
                    validator =>
                    !validator.Value.Any(attribute => !attribute.IsValid(_propertyGetters[validator.Key](this))));
        }

        private ValidationAttribute[] GetValidations(PropertyInfo property)
        {
            return (ValidationAttribute[]) property.GetCustomAttributes(typeof (ValidationAttribute), true);
        }

        private Func<EntityObjectBase, object> GetValueGetter(PropertyInfo property)
        {
            return viewmodel => property.GetValue(viewmodel, null);
        }

        #endregion

        public AccessType Access { get; set; }

        public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.Register("SaveCommand", typeof(Command), typeof(EntityObjectBase), new PropertyMetadata(default(Command)));

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(Command), typeof(EntityObjectBase), new PropertyMetadata(default(Command)));

        public Command SaveCommand
        {
            get { return (Command)GetValue(SaveCommandProperty); }
            set { SetValue(SaveCommandProperty, value); }
        }

        public virtual void Save(object param)
        {
        }
        public virtual bool CanSave()
        {
            return (Access & AccessType.Update) == AccessType.Update;
        }

        public Command DeleteCommand
        {
            get { return (Command)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        public virtual void Delete(object param)
        {
        }

        public virtual bool CanDelete()
        {
            return (Access & AccessType.Update) == AccessType.Update;
        }

    }
}
