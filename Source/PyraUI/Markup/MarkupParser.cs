using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using Pyratron.UI.Controls;
using Pyratron.UI.Markup.Converters;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Markup
{
    /// <summary>
    /// Parses XAML into a visual tree.
    /// </summary>
    public class MarkupParser
    {
        private readonly List<IMarkdownConverter> converters = new List<IMarkdownConverter>();

        private readonly Manager manager;
        private readonly char[] newlineChars = Environment.NewLine.ToCharArray();

        internal MarkupParser(Manager manager)
        {
            this.manager = manager;

            // Converters.
            converters.Add(new DoubleMarkdownConverter());
            converters.Add(new EnumMarkdownConverter());
            converters.Add(new ThicknessMarkdownConverter());
            converters.Add(new ColorMarkdownConverter());
            converters.Add(new BrushMarkdownConverter());
        }

        /// <summary>
        /// Loads a visual tree from the specified XAML, which will be placed under the parent. Leave null for no parent.
        /// </summary>
        public void LoadFromXAML(string xml, Element parent)
        {
            if (string.IsNullOrEmpty(xml))
                throw new ArgumentException(nameof(xml), "XAML must not be empty.");
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var nodes = doc.ChildNodes;
            LoadNode(nodes, parent);
        }

        /// <summary>
        /// Create an instance of an element by its name.
        /// </summary>
        public Element CreateControlInstance(XmlNode node, Element parent)
        {
            // Add content (inline text) from the XML to a parent element.
            if (parent != null && node.Name.StartsWith("#text") && node.ParentNode != null)
                parent.AddContent(node.Value.Trim().TrimStart(newlineChars).TrimEnd(newlineChars));
            if (node.Name.StartsWith("#")) return null;
            var t = Type.GetType(typeof (Control).Namespace + '.' + node.Name);
            // Create a new element.
            return (Element) Activator.CreateInstance(t, manager);
        }

        private void LoadNode(IEnumerable nodes, Element parent)
        {
            foreach (XmlNode node in nodes)
            {
                // Create an instance of the control.
                var control = CreateControlInstance(node, parent);
                if (control == null) continue;
                control.LogicalParent = parent;
                control.Parent = parent;

                if (node.Attributes != null)
                {
                    // Get all the dependency properties (and those inherited) of the control.
                    var props = DependencyProperty.GetProperties(control.GetType());
                    List<DependencyProperty> parentProps = null;

                    // For each xml property, try and find a dependency property to set.
                    foreach (XmlAttribute xmlProperty in node.Attributes)
                    {
                        var propertyName = xmlProperty.Name;

                        var prop = props.FirstOrDefault(p => p.Name == propertyName);
                        if (prop != null)
                        {
                            if (prop.Attached)
                                throw new InvalidOperationException("Attached property cannot be set on the parent.");
                            // Convert the string value to the dependency property type.
                            var value = ConvertValue(xmlProperty.Value, prop.ValueType);
                            control.SetValue(prop, value);
                        }
                        // Attached properties.
                        else if (propertyName.Contains(".") && parent != null)
                        {
                            if (parentProps == null) // Get parent properties only when needed and only once.
                                parentProps = DependencyProperty.GetProperties(parent.GetType());
                            var parts = propertyName.Split('.');
                            if (parts.Length == 2)
                            {
                                var parentName = parts[0];
                                propertyName = parts[1];
                                var parentType = parent.GetType();

                                if (parentType.Name == parentName)
                                {
                                    prop = parentProps.FirstOrDefault(p => p.Attached && p.Name == propertyName);
                                    if (prop != null)
                                    {
                                        var value = ConvertValue(xmlProperty.Value, prop.ValueType);
                                        control.SetValue(prop, value);
                                    }

                                }
                                else
                                    throw new InvalidOperationException("No parent found matching " + parentName);
                            }
                        }
                    }
                }

                // Add element to parent or set as root element.
                if (parent == null)
                    manager.AddRootElement(control);
                else
                    parent.Add(control);
                LoadNode(node.ChildNodes, control);
            }
        }

        private object ConvertValue(string unconverted, Type type)
        {
            object value = null;
            // Convert the attribute to a value using a converter or Convert.ChangeType.
            var found = false;
            for (var i = 0; i < converters.Count; i++)
            {
                var converter = converters[i];
                if (converter.CanConvert(type))
                {
                    value = converter.Convert(type, unconverted);
                    found = true;
                    break;
                }
            }
            if (!found)
                value = Convert.ChangeType(unconverted, type);
            return value;
        }
    }
}