namespace Pyratron.UI.Types.Properties
{
    /// <summary>
    /// Defines behavior of a dependency property.
    /// </summary>
    public class PropertyMetadata
    {
        public MetadataOption Options { get; private set; }

        /// <summary>
        /// The default value to be used if no defined values are found in the tree.
        /// </summary>
        internal object DefaultValue { get; set; }

        /// <summary>
        /// Invoked when the property value changes.
        /// </summary>
        public PropertyChangedCallback PropertyChanged { get; set; }

        /// <summary>
        /// Invoked when the property value changes, giving the opportunity to correct (coerce) the value. (Such as keeping it
        /// within a range.)
        /// </summary>
        /// <remarks>
        /// Use this when an invalid value can be corrected. (For example, a value below the minimum value on a slider)
        /// </remarks>
        public CoerceValueCallback CoerceValue { get; set; }


        /// <summary>
        /// Indicates if the element should be invalidated and request a measure pass if the property is changed.
        /// </summary>
        public bool AffectsMeasure
        {
            get { return Options.HasFlag(MetadataOption.AffectsMeasure); }
            set { SetFlag(MetadataOption.AffectsMeasure, value); }
        }

        /// <summary>
        /// Indicates if the element should be invalidated and request an arrange pass if the property is changed.
        /// </summary>
        public bool AffectsArrange
        {
            get { return Options.HasFlag(MetadataOption.AffectsArrange); }
            set { SetFlag(MetadataOption.AffectsArrange, value); }
        }

        /// <summary>
        /// Indicates if the element's parent should be invalidated and request a measure pass if the property is changed.
        /// </summary>
        public bool AffectsParentMeasure
        {
            get { return Options.HasFlag(MetadataOption.AffectsParentMeasure); }
            set { SetFlag(MetadataOption.AffectsParentMeasure, value); }
        }

        /// <summary>
        /// Indicates if the element's parent should be invalidated and request an arrange pass if the property is changed.
        /// </summary>
        public bool AffectsParentArrange
        {
            get { return Options.HasFlag(MetadataOption.AffectsParentArrange); }
            set { SetFlag(MetadataOption.AffectsParentArrange, value); }
        }

        /// <summary>
        /// Indicates the property will inherit the property of other parent elements if it is not defined.
        /// </summary>
        public bool Inherits
        {
            get { return Options.HasFlag(MetadataOption.Inherits); }
            set { SetFlag(MetadataOption.Inherits, value); }
        }

        public PropertyMetadata(MetadataOption options, PropertyChangedCallback propertyChangedCallback = null,
            CoerceValueCallback coerceValueCallback = null)
        {
            Options = options;
            PropertyChanged = propertyChangedCallback;
            CoerceValue = coerceValueCallback;
        }

        public PropertyMetadata(MetadataOption options, CoerceValueCallback coerceValueCallback)
            : this(options, null, coerceValueCallback)
        {
        }

        public static implicit operator PropertyMetadata(MetadataOption option)
        {
            return new PropertyMetadata(option);
        }

        internal object OnCoerceValue(object value) => CoerceValue == null ? value : CoerceValue.Invoke(value);


        internal void OnValueChanged(object newValue, object oldValue) => PropertyChanged?.Invoke(newValue, oldValue);


        private void SetFlag(MetadataOption option, bool value)
        {
            if (value)
                Options |= option;
            else
                Options &= ~option;
        }

        public delegate object CoerceValueCallback(object value);

        public delegate object PropertyChangedCallback(object newValue, object oldValue);
    }
}