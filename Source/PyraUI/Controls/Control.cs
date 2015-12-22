using System;
using System.Linq;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Controls
{
    /// <summary>
    /// An element that uses a ControlTemplate to define its appearence.
    /// </summary>
    public class Control : Visual
    {
        /// <summary>
        /// Indicates if the control is enabled 
        /// </summary>
        public bool IsEnabled { get; set; }

        public ContentPresenter Presenter { get; set; }

        /// <summary>
        /// The area inside the border.
        /// </summary>
        /// <remarks>
        /// Padding is not calculated in any way with the arrange/measure process. Instead, the padding is applied to a controls ContentPresenter.
        /// </remarks>
        public Thickness Padding
        {
            get { return GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly DependencyProperty<Thickness> PaddingProperty =
            DependencyProperty.Register<Element, Thickness>(nameof(Padding),
                MetadataOption.IgnoreInheritance | MetadataOption.AffectsMeasure | MetadataOption.AffectsArrange);


        public Control(Manager manager) : base(manager)
        {
            Presenter = new ContentPresenter(manager) {Margin = Padding};
            base.Add(Presenter);
        }

        protected override void OnPropertyChanged(DependencyProperty property, object newValue, object oldValue)
        {
            if (property == PaddingProperty)
            {
                Presenter.Margin = (Thickness) newValue;
            }
            base.OnPropertyChanged(property, newValue, oldValue);
        }

        public override void Add(Element element)
        {
            element.LogicalParent = LogicalParent;
            Presenter.Add(element);
        }

        public override void Remove(Element element, bool dispose = true)
        {
            Presenter.Remove(element, dispose);
        }

        /// <summary>
        /// Add an element directly and not to the presenter.
        /// </summary>
        internal virtual void AddDirect(Element element)
        {
            base.Add(element);
        }
        /// <summary>
        /// Removes an element directly and not from the presenter.
        /// </summary>
        internal virtual void RemoveDirect(Element element)
        {
            base.Remove(element, false);
        }

        public override void AddContent(string content)
        {
            Presenter.AddContent(content);
        }
    }
}