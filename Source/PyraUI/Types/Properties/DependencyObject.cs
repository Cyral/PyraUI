using System.Collections.Generic;

namespace Pyratron.UI.Types.Properties
{
    /// <summary>
    /// An object that has dependency properties.
    /// </summary>
    public class DependencyObject
    {
        /// <summary>
        /// The parent of this dependency property, which is used to resolve values if a property is not defined for this object.
        /// </summary>
        protected DependencyObject DependencyParent { get; set; }

        /// <summary>
        /// Collection of defined property values.
        /// </summary>
        private readonly Dictionary<DependencyProperty, DependencyValue> values;

        public DependencyObject()
        {
            values = new Dictionary<DependencyProperty, DependencyValue>();
        }

        public void ClearValue(DependencyProperty property)
        {
            values.Remove(property);
        }

        /// <summary>
        /// Set the value of the property.
        /// </summary>
        public void SetValue(DependencyProperty property, object value)
        {
            var type = GetType();
            var metadata = property.GetMetadata(type);

            if (values.ContainsKey(property)) // Update value
            {
                var old = values[property].GetValue();
                if (value != old)
                {
                    if (property.OnValidateValue(value))
                    {
                        value = metadata.OnCoerceValue(value);
                        values[property].SetValue(value);
                        property.OnValueChanged(this, value, old);
                        OnPropertyChanged(property, value, old);
                    }
                }

            }
            else // Create new
            {
                if (property.OnValidateValue(value))
                {
                    value = metadata.OnCoerceValue(value);
                    values[property] = new DependencyValue(value);
                    property.OnValueChanged(this, value, DependencyValue.Unset);
                    OnPropertyChanged(property, value, DependencyValue.Unset);
                }
            }
        }

        private object GetValueCore(DependencyProperty property)
        {
            var type = GetType();
            var metadata = property.GetMetadata(type);

            // If the value exists in the dictionary, use it, otherwise search for it in the tree.
            if (values.ContainsKey(property))
                return values[property].GetValue();
            if (metadata.Inherits && DependencyParent != null)
            {
                // The value didn't exist in the dictionary, but if the metadata is marked as inheritable, try and find the value further up the tree.
                // If the metadata is different, meaning it was overridden by a more specific type, then return the default value.
                var parentMetadata = property.GetMetadata(DependencyParent.GetType());
                if (parentMetadata == metadata)
                    return DependencyParent.GetValueCore(property);
                return parentMetadata.DefaultValue;
            }
            return metadata.DefaultValue;
        }

        /// <summary>
        /// Returns the current effective value of the property.
        /// </summary>
        public TValue GetValue<TValue>(DependencyProperty<TValue> property)
        {
            return (TValue)GetValueCore(property);
        }

        protected virtual void OnPropertyChanged(DependencyProperty property, object newValue, object oldValue)
        {
            // Classes deriving from DependencyObject can implement behavior when a property changes. (Such as invalidating the measure/arrange)
        }
    }
}