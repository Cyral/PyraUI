using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using Pyratron.UI.Controls;
using Pyratron.UI.Types;

namespace Pyratron.UI
{
    public abstract class Manager
    {
        /// <summary>
        /// Master list of all elements.
        /// </summary>
        public List<Element> Elements { get; } = new List<Element>();

        public Renderer Renderer { get; set; }

        public Skin Skin { get; set; }

        public bool DrawDebug { get; set; }

        public virtual void Init()
        {
            LoadFromXML(File.ReadAllText("window.xml"), null);
        }

        public virtual void LoadFromXML(string xml, Element parent)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var nodes = doc.ChildNodes;
            LoadNode(nodes, parent);
        }

        /// <summary>
        /// Create an instance of an element by its name.
        /// </summary>
        public Element CreateControlInstance(XmlNode node)
        {
            if (node.Name.StartsWith("#text"))
                return new Label(this) {Text = node.Value};
            var t = Type.GetType(typeof (Control).Namespace + '.' + node.Name);
            return (Element) Activator.CreateInstance(t, this);
        }

        public virtual void Load()
        {
            Skin.LoadTextureInternal("button");
            Skin.LoadFontInternal("default");
        }

        /// <summary>
        /// Draws the UI.
        /// </summary>
        /// <param name="delta">Seconds elapsed since last frame.</param>
        public virtual void Draw(float delta)
        {
            Elements[0].Arrange();
            Renderer.BeginDraw();
            //Renderer.DrawTexture("button", new Rectangle(50, 50, 150, 150));
            //Renderer.DrawString("Hello! Welcome to PyraUI.", new Point(400, 400));

            // Render all top level elements. (Those with no parent).
            for (var i = 0; i < Elements.Count; i++)
            {
                var element = Elements[i];
                var visual = element as Visual;
                // If the element is a visual, render it.
                if (element.Parent == null && visual != null)
                {
                    visual.Draw(delta);
                }
            }
            Renderer.EndDraw();
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        /// <param name="delta">Seconds elapsed since last frame.</param>
        public virtual void Update(float delta)
        {
            for (var i = 0; i < Elements.Count; i++)
            {
                var element = Elements[i];
                element.Update(delta);
            }
        }

        private void LoadNode(XmlNodeList nodes, Element parent)
        {
            foreach (XmlNode node in nodes)
            {
                var control = CreateControlInstance(node);
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
                            object value;
                            if (propertyDescriptor.PropertyType.IsEnum)
                                value = Enum.Parse(propertyDescriptor.PropertyType, xmlProperty.Value);
                            else if (propertyDescriptor.PropertyType.UnderlyingSystemType == typeof (Dimension))
                                value = (Dimension) xmlProperty.Value;
                            else if (propertyDescriptor.PropertyType.UnderlyingSystemType == typeof (Thickness))
                                value = (Thickness) xmlProperty.Value;
                            else
                                value = Convert.ChangeType(xmlProperty.Value, propertyDescriptor.PropertyType);
                            propertyDescriptor.SetValue(control, value);
                        }
                    }
                }

                // Add element to parent or set as root element.
                if (parent == null)
                    Elements.Add(control);
                else
                    parent.Add(control);
                LoadNode(node.ChildNodes, control);
            }
        }
    }
}