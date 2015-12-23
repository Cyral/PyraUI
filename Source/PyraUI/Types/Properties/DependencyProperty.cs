using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace Pyratron.UI.Types.Properties
{
    /// <summary>
    /// A property that inherits its value from the element tree, has change notifications, and associated metadata.
    /// </summary>
    /// <typeparam name="TValue">The type that this property contains. (double, Color, etc.)</typeparam>
    public class DependencyProperty<TValue> : DependencyProperty
    {
        internal DependencyProperty(string name, Type owner, PropertyMetadata metadata,
            ValidateValueCallback validateValueCallback = null, bool attached = false)
            : base(name, owner, typeof(TValue), metadata, validateValueCallback, attached)
        {
        }

        /// <summary>
        /// Overrides the metadata of the closest base type with property metdata. The default value will now be used for
        /// <paramref name="forType" /> and derived classes.
        /// </summary>
        /// <param name="forType">The type for this metadata to apply to.</param>
        /// <param name="defaultValue">The new default value.</param>
        public void OverrideMetadata(Type forType, TValue defaultValue)
        {
            var newMetadata = new PropertyMetadata(OwnerMetadata.Options);
            OverrideMetadata(forType, defaultValue, newMetadata);
        }

        /// <summary>
        /// Overrides the metadata of the closest base type with property metdata. The default value will now be used for
        /// <paramref name="forType" /> and derived classes. Any metadata options specified will be OR'd with the current options.
        /// </summary>
        /// <param name="forType"></param>
        /// <param name="defaultValue"></param>
        /// <param name="metadata">
        /// New metdata options and callbacks. Options will be OR'd with the current options.
        /// CoerceValueCallback will be overridden if specified, and PropertyChangedCallback will be added to the property changed
        /// event.
        /// </param>
        public void OverrideMetadata(Type forType, TValue defaultValue, PropertyMetadata metadata)
        {
            if (metadata == null)
                metadata = new PropertyMetadata(OwnerMetadata.Options);
            else
                metadata.Options = OwnerMetadata.Options | metadata.Options;
            metadata.DefaultValue = defaultValue;
            AddMetadata(forType, metadata);
        }
    }

    /// <summary>
    /// A property that inherits its value from the element tree, has change notifications, and associated metadata.
    /// </summary>
    public class DependencyProperty
    {
        protected static readonly MetadataOption DefaultMetadataOptions = MetadataOption.Inherits;
        public string Name { get; }

        /// <summary>
        /// The type of dependency object this property is applied to.
        /// </summary>
        internal Type ObjectType { get; }

        /// <summary>
        /// The type the value is.
        /// </summary>
        internal Type ValueType { get; }

        /// <summary>
        /// Indicates if this is an attached property.
        /// </summary>
        public bool Attached { get; private set; }

        protected internal PropertyMetadata OwnerMetadata => Metadata[ObjectType];

        /// <summary>
        /// Invoked when the property value changes, if the validation returns false, an ArgumentException will be thrown.
        /// </summary>
        /// <remarks>
        /// Use this when an invalid value that cannot be corrected is specified. (For example, Infinity or NaN on a slider)
        /// </remarks>
        internal ValidateValueCallback ValidateValue { get; set; }

        /// <summary>
        /// Collection of property metadata, as it can be overridden.
        /// </summary>
        protected Dictionary<Type, PropertyMetadata> Metadata { get; }

        private List<Type> metadataCache; // List of sorted metadata types.

        private static Dictionary<Type, List<DependencyProperty>> typeProperties = new Dictionary<Type, List<DependencyProperty>>();

        protected DependencyProperty(string name, Type owner, Type valueType, PropertyMetadata metadata,
            ValidateValueCallback validateValueCallback, bool attached)
        {
            Attached = attached;
            Metadata = new Dictionary<Type, PropertyMetadata>();
            metadataCache = new List<Type>();
            Name = name;
            ObjectType = owner;
            ValueType = valueType;
            Metadata.Add(ObjectType, metadata);
            ValidateValue = validateValueCallback;

            if (!typeProperties.ContainsKey(owner))
                typeProperties[owner] = new List<DependencyProperty>();
            typeProperties[owner].Add(this);
        }

        public static List<DependencyProperty> GetProperties(Type owner)
        {
            var types = typeProperties.Where(type => type.Key == owner || owner.IsSubclassOf(type.Key)).ToList();
            var properties = new List<DependencyProperty>();
            foreach (var type in types)
                properties.AddRange(type.Value);
            return properties;
        }

        public PropertyMetadata GetMetadata(Type forType)
        {
            // Find either the overridden metadata of the specified type, or the metadata type the specified type derives from.
            var nearest = metadataCache.LastOrDefault(type => forType == type || forType.IsSubclassOf(type));
            // If there is no type in the overridden metadata collection, return the metadata of the owner.
            return nearest == null ? OwnerMetadata : Metadata[nearest];
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
            var objType = typeof (TObject);
            if (metadata == null)
                metadata = new PropertyMetadata(DefaultMetadataOptions);
            metadata.DefaultValue = defaultValue;
            return new DependencyProperty<TValue>(name, objType, metadata, validateValueCallback, false);
        }

        /// <summary>
        /// Register a new DependencyProperty.
        /// </summary>
        /// <typeparam name="TObject">The type of DependencyObject the property applies to.</typeparam>
        /// <typeparam name="TValue">The type of value the property is.</typeparam>
        /// <param name="name">The name of this property.</param>
        /// <param name="defaultValue">The default value to be used if no value is defined.</param>
        public static DependencyProperty<TValue> Register<TObject, TValue>(string name,
            TValue defaultValue = default(TValue))
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

        /// <summary>
        /// Register a new attached DependencyProperty.
        /// </summary>
        /// <typeparam name="TObject">The type of DependencyObject the property applies to.</typeparam>
        /// <typeparam name="TValue">The type of value the property is.</typeparam>
        /// <param name="name">The name of this property.</param>
        /// <param name="defaultValue">The default value to be used if no value is defined.</param>
        /// <param name="metadata">Metadata associated with the property.</param>
        /// <param name="validateValueCallback">Callback to validate the property when changed.</param>
        public static DependencyProperty<TValue> RegisterAttached<TObject, TValue>(string name, TValue defaultValue,
            PropertyMetadata metadata, ValidateValueCallback validateValueCallback = null)
            where TObject : DependencyObject
        {
            var objType = typeof(TObject);
            if (metadata == null)
                metadata = new PropertyMetadata(DefaultMetadataOptions);
            metadata.DefaultValue = defaultValue;
            return new DependencyProperty<TValue>(name, objType, metadata, validateValueCallback, true);
        }

        /// <summary>
        /// Register a new attached DependencyProperty.
        /// </summary>
        /// <typeparam name="TObject">The type of DependencyObject the property applies to.</typeparam>
        /// <typeparam name="TValue">The type of value the property is.</typeparam>
        /// <param name="name">The name of this property.</param>
        /// <param name="defaultValue">The default value to be used if no value is defined.</param>
        public static DependencyProperty<TValue> RegisterAttached<TObject, TValue>(string name,
            TValue defaultValue = default(TValue))
            where TObject : DependencyObject
        {
            return RegisterAttached<TObject, TValue>(name, defaultValue, null);
        }

        /// <summary>
        /// Register a new attached DependencyProperty.
        /// </summary>
        /// <typeparam name="TObject">The type of DependencyObject the property applies to.</typeparam>
        /// <typeparam name="TValue">The type of value the property is.</typeparam>
        /// <param name="name">The name of this property.</param>
        /// <param name="metadata">Metadata associated with the property.</param>
        public static DependencyProperty<TValue> RegisterAttached<TObject, TValue>(string name, PropertyMetadata metadata)
            where TObject : DependencyObject
        {
            return RegisterAttached<TObject, TValue>(name, default(TValue), metadata);
        }

        public override string ToString()
        {
            return $"{ObjectType.Name}: {Name}";
        }

        /// <summary>
        /// Add metadata to the metadata collection.
        /// </summary>
        protected void AddMetadata(Type forType, PropertyMetadata metadata)
        {
            // Add or update the metadata for this type.
            Metadata[forType] = metadata;
            metadataCache = new List<Type> {ObjectType}; // The owner of the property is always added first.
            var types = Metadata.Keys.Where(type => type != ObjectType).ToList();

            // For each of the metadata keys, add them to the cache in order of each base class, and then its derived classes.
            while (types.Count > 0)
            {
                var type = types.FirstOrDefault(subtype => types.All(basetype => !subtype.IsSubclassOf(basetype)));
                types.Remove(type);
                metadataCache.Add(type);
            }
        }

        internal bool OnValidateValue(object value)
        {
            if (ValidateValue == null) return true;
            if (!ValidateValue.Invoke(value)) // If validation function returns false, throw an error.
                throw new ArgumentException("Value (" + value + ") is invalid for property: " + Name);
            return true;
        }

        internal void OnValueChanged(DependencyObject sender, object newValue, object oldValue)
        {
            // Call property changed notification on each registered metadata event.
            foreach (var metadata in Metadata)
                metadata.Value.OnValueChanged(sender, newValue, oldValue);
        }

        public delegate bool ValidateValueCallback(object value);
    }
}