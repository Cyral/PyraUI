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
        private static readonly TimeSpan second = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Master list of all elements.
        /// </summary>
        public List<Element> Elements { get; } = new List<Element>();

        public Renderer Renderer { get; set; }

        public Skin Skin { get; set; }

        public bool DrawDebug { get; set; }

        /// <summary>
        /// The current FPS.
        /// </summary>
        public int FPS { get; private set; }

        private TimeSpan elapsedTime = TimeSpan.Zero;
        private int totalFrames;

        private string xml;
        private readonly char[] newlineChars = Environment.NewLine.ToCharArray();

        public virtual void Init()
        {
            xml = LoadFromXML(File.ReadAllText("window.xml"), null);
            foreach (var element in Elements)
                element.UpdateLayout();
        }

        public virtual string LoadFromXML(string xml, Element parent)
        {
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
            var t = Type.GetType(typeof(Control).Namespace + '.' + node.Name);
            // Create a new element.
            return (Element) Activator.CreateInstance(t, this);
        }

        public virtual void Load()
        {
            Skin.LoadTextureInternal("button");
        }

        /// <summary>
        /// Draws the UI.
        /// </summary>
        /// <param name="delta">Seconds elapsed since last frame.</param>
        public virtual void Draw(float delta)
        {
            //Elements[0].UpdateLayout();
            Renderer.BeginDraw();
            //Renderer.DrawTexture("button", new Rectangle(50, 50, 150, 150));
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
                element.ActualSizePrevious = element.ActualSize;
            }
            Renderer.DrawString($"FPS: {FPS}\nRendered From XML:\n{xml}", new Point(8, Elements[0].ExtendedArea.Height + 8), Color.Black,
                8, true);
            Renderer.EndDraw();

            // Calculate FPS
            elapsedTime += TimeSpan.FromSeconds(delta);
            if (elapsedTime > second)
            {
                elapsedTime -= second;
                FPS = totalFrames;
                totalFrames = 0;
            }
            totalFrames++;
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
                if (element.ActualSize != element.ActualSizePrevious)
                    element.InvalidateLayout();
                element.Update(delta);
            }
        }

        private void LoadNode(XmlNodeList nodes, Element parent)
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
                            object value;
                            // Convert the attribute to a value.
                            if (propertyDescriptor.PropertyType.IsEnum)
                                value = Enum.Parse(propertyDescriptor.PropertyType, xmlProperty.Value);
                            else if (propertyDescriptor.PropertyType.UnderlyingSystemType == typeof (Thickness))
                                value = (Thickness) xmlProperty.Value;
                            else if (propertyDescriptor.PropertyType.UnderlyingSystemType == typeof(Color))
                                value = (Color)xmlProperty.Value;
                            else if (xmlProperty.Value.Equals("Auto", StringComparison.InvariantCultureIgnoreCase))
                                value = double.PositiveInfinity;
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