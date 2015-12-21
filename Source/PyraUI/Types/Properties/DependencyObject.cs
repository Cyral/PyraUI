using System.Collections.Generic;

namespace Pyratron.UI.Types.Properties
{
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
            if (values.ContainsKey(property)) // Update value
            {
                var old = values[property].GetValue();
                if (value != old)
                {
                    if (property.OnValidateValue(value))
                    {
                        value = property.Metadata.OnCoerceValue(value);
                        values[property].SetValue(value);
                        property.Metadata.OnValueChanged(value, old);
                        OnPropertyChanged(property, value, old);
                    }
                }

            }
            else // Create new
            {
                if (property.OnValidateValue(value))
                {
                    value = property.Metadata.OnCoerceValue(value);
                    values[property] = new DependencyValue(value);
                    property.Metadata.OnValueChanged(value, DependencyValue.Unset);
                    OnPropertyChanged(property, value, DependencyValue.Unset);
                }
            }
        }

        private object GetValueCore(DependencyProperty property)
        {
            // If the value exists in the dictionary, use it, otherwise search for it in the tree.
            if (values.ContainsKey(property))
                return values[property].GetValue();
            if (property.Metadata.Inherits && DependencyParent != null)
                return DependencyParent.GetValueCore(property);
            return property.Metadata.DefaultValue;
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
            
        }
    }
}