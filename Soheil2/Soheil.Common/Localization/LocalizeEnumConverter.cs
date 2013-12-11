using System;

namespace Soheil.Common.Localization
{
    /// <summary>
    /// Defines a type converter for enum types defined in this project
    /// </summary>
    class LocalizedEnumConverter : ResourceEnumConverter
    {
        /// <summary>
        /// Create a new instance of the converter using translations from the given resource manager
        /// </summary>
        /// <param name="type"></param>
        public LocalizedEnumConverter(Type type)
            : base(type, Properties.Resources.ResourceManager)
        {
        }
    }
}
