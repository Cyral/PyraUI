using System;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Controls
{
    public class Label : Control
    {
        public static readonly DependencyProperty<string> TextProperty =
            DependencyProperty.Register<Element, string>(nameof(Text), "Text", new PropertyMetadata(
                MetadataOption.IgnoreInheritance | MetadataOption.AffectsMeasure | MetadataOption.AffectsArrange));

        public static readonly DependencyProperty<Alignment> TextAlignmentProperty =
            DependencyProperty.Register<Element, Alignment>(nameof(TextAlignment), Alignment.Center,
                new PropertyMetadata(
                    MetadataOption.IgnoreInheritance));

        /// <summary>
        /// The label's text.
        /// </summary>
        public string Text
        {
            get { return GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private TextBlock textBlock;

        /// <summary>
        /// Alignment of label text.
        /// </summary>
        public Alignment TextAlignment
        {
            get { return GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        static Label()
        {
            HorizontalAlignmentProperty.OverrideMetadata(typeof (Label), HorizontalAlignment.Center);
            VerticalAlignmentProperty.OverrideMetadata(typeof(Label), VerticalAlignment.Center);
        }

        public Label(Manager manager, string text) : this(manager)
        {
            textBlock = new TextBlock(manager);
            Presenter.Add(textBlock);
            Text = text;
        }

        protected override void OnPropertyChanged(DependencyProperty property, object newValue, object oldValue)
        {
            if (property == TextProperty)
            {
                textBlock.Text = (string) newValue;
            }
            else if (property == TextAlignmentProperty)
            {
                textBlock.TextAlignment = (Alignment) newValue;
            }
            base.OnPropertyChanged(property, newValue, oldValue);
        }

        public Label(Manager manager) : base(manager)
        {
         
        }

        public override void AddContent(string content)
        {
            Text = content;
        }
    }
}