using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using Pyratron.UI.Controls;
using Pyratron.UI.Markup.Converters;

namespace Pyratron.UI.Markup
{
    public class MarkupLoader
    {
        private readonly List<IMarkdownConverter> converters = new List<IMarkdownConverter>();

        private readonly Manager manager;
        private readonly char[] newlineChars = Environment.NewLine.ToCharArray();

        internal MarkupLoader(Manager manager)
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
        public string LoadFromXAML(string xml, Element parent)
        {
            if (string.IsNullOrEmpty(xml))
                throw new ArgumentException(nameof(xml), "XAML must not be empty.");
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var nodes = doc.ChildNodes;
            LoadNode(nodes, parent);
            return xml;
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
                var control = CreateControlInstance(node, parent);
                if (control == null) continue;

                var props = TypeDescriptor.GetProperties(control.GetType());

                // Set attributes.
                if (node.Attributes != null)
                {
                    foreach (XmlAttribute xmlProperty in node.Attributes)
                    {
                        var propertyName = xmlProperty.Name;
                        var propertyDescriptor = props[propertyName];

                        if (propertyDescriptor != null)
                        {
                            object value = null;
                            // Convert the attribute to a value using a converter or Convert.ChangeType.
                            var found = false;
                            for (var i = 0; i < converters.Count; i++)
                            {
                                var converter = converters[i];
                                if (converter.CanConvert(propertyDescriptor.PropertyType))
                                {
                                    value = converter.Convert(propertyDescriptor.PropertyType, xmlProperty.Value);
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                                value = Convert.ChangeType(xmlProperty.Value, propertyDescriptor.PropertyType);
                            propertyDescriptor.SetValue(control, value);
                        }
                    }
                }

                // Add element to parent or set as root element.
                if (parent == null)
                    manager.Elements.Add(control);
                else
                    parent.Add(control);
                LoadNode(node.ChildNodes, control);
            }
        }
    }
}