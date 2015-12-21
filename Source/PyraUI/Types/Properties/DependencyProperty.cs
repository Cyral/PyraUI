using System;
using System.Collections.Generic;

namespace Pyratron.UI.Types.Properties
{
    public class DependencyProperty<TValue> : DependencyProperty
    {
        internal DependencyProperty(string name, Type owner, PropertyMetadata metadata,
            ValidateValueCallback validateValueCallback = null)
            : base(name, owner, metadata, validateValueCallback)
        {
        }
    }

    public class DependencyProperty
    {
        public string Name { get; }

        /// <summary>
        /// The type of dependency object this property is applied to.
        /// </summary>
        internal Type ObjectType { get; }

        public PropertyMetadata Metadata
        {
            get { return metadata[ObjectType]; }
        }

        /// <summary>
        /// Invoked when the property value changes, if the validation returns false, an ArgumentException will be thrown.
        /// </summary>
        /// <remarks>
        /// Use this when an invalid value that cannot be corrected is specified. (For example, Infinity or NaN on a slider)
        /// </remarks>
        public ValidateValueCallback ValidateValue { get; set; }

        /// <summary>
        /// Collection of property metadata, as it can be overridden.
        /// </summary>
        private readonly Dictionary<Type, PropertyMetadata> metadata;

        protected DependencyProperty(string name, Type owner, PropertyMetadata metadata,
            ValidateValueCallback validateValueCallback)
        {
            this.metadata = new Dictionary<Type, PropertyMetadata>();
            Name = name;
            ObjectType = owner;
            this.metadata.Add(ObjectType, metadata);
            ValidateValue = validateValueCallback;
        }

        /// <summary>
        /// Register a new DependencyProperty.
        /// </summary>
        /// <typeparam name="TObject">The type of DependencyObject the property applies to.</typeparam>
        /// <typeparam name="TValue">The type of value the property is.</typeparam>
        /// <param name="name">The name of this property.</param>
        /// <param name="defaultValue">The default value to be used if no value is defined.</param>
        /// <param name="metadata">Metadata associated with the property.</param>
        /// <param name="validateValueCallback">Callback to validate the property when changed.</param>
        public static DependencyProperty<TValue> Register<TObject, TValue>(string name, TValue defaultValue,
            PropertyMetadata metadata, ValidateValueCallback validateValueCallback = null)
            where TObject : DependencyObject
        {
            if (metadata == null)
                metadata = new PropertyMetadata(MetadataOption.Inherits);
            metadata.DefaultValue = defaultValue;
            return new DependencyProperty<TValue>(name, typeof (TObject), metadata, validateValueCallback);
        }

        /// <summary>
        /// Register a new DependencyProperty.
        /// </summary>
        /// <typeparam name="TObject">The type of DependencyObject the property applies to.</typeparam>
        /// <typeparam name="TValue">The type of value the property is.</typeparam>
        /// <param name="name">The name of this property.</param>
        /// <param name="defaultValue">The default value to be used if no value is defined.</param>
        public static DependencyProperty<TValue> Register<TObject, TValue>(string name, TValue defaultValue = default(TValue))
            where TObject : DependencyObject
        {
            return Register<TObject, TValue>(name, defaultValue, null);
        }

        /// <summary>
        /// Register a new DependencyProperty.
        /// </summary>
        /// <typeparam name="TObject">The type of DependencyObject the property applies to.</typeparam>
        /// <typeparam name="TValue">The type of value the property is.</typeparam>
        /// <param name="name">The name of this property.</param>
        /// <param name="metadata">Metadata associated with the property.</param>
        public static DependencyProperty<TValue> Register<TObject, TValue>(string name, PropertyMetadata metadata)
            where TObject : DependencyObject
        {
            return Register<TObject, TValue>(name, default(TValue), metadata);
        }

        public override string ToString()
        {
            return $"{ObjectType.Name}: {Name}";
        }

        internal bool OnValidateValue(object value)
        {
            if (ValidateValue == null) return true;
            if (ValidateValue.Invoke(value))
                throw new ArgumentException("Value (" + value + ") is invalid for property: " + Name);
            return true;
        }

        public delegate bool ValidateValueCallback(object value);
    }
}