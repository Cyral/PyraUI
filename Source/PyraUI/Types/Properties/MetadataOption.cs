using System;

namespace Pyratron.UI.Types.Properties
{
    /// <summary>
    /// Behavior associated with property metadata.
    /// </summary>
    [Flags]
    public enum MetadataOption
    {
        /// <summary>
        /// No options enabled.
        /// </summary>
        None = 0,
        /// <summary>
        /// The element should be invalidated and request a measure pass if the property is changed.
        /// </summary>
        AffectsMeasure = 1,
        /// <summary>
        /// The element should be invalidated and request an arrange pass if the property is changed.
        /// </summary>
        AffectsArrange = 2,
        /// <summary>
        /// The element's parent should be invalidated and request a measure pass if the property is changed.
        /// </summary>
        AffectsParentArrange = 4,
        /// <summary>
        /// The element should be invalidated and request an arrange pass if the property is changed.
        /// </summary>
        AffectsParentMeasure = 8,
        /// <summary>
        /// This property will inherit the property of other parent elements if it is not defined.
        /// </summary>
        Inherits = 16,
        /// <summary>
        /// All options enabled.
        /// </summary>
        All = AffectsMeasure | AffectsArrange | AffectsParentMeasure | AffectsParentArrange | Inherits,
        /// <summary>
        /// This element will not inherit values from its parents.
        /// </summary>
        IgnoreInheritance = Inherits & ~Inherits,
    }
}