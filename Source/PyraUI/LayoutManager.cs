using System.Collections.Generic;
using System.Linq;
using Pyratron.UI.Controls;
using Pyratron.UI.Types;

namespace Pyratron.UI
{
    /// <summary>
    /// Manages a queue of elements to arrange and measure.
    /// </summary>
    public class LayoutManager
    {
        private readonly List<Element> arrangeQueue;

        private readonly Manager manager;
        private readonly List<Element> measureQueue;

        public LayoutManager(Manager manager)
        {
            this.manager = manager;

            measureQueue = new List<Element>();
            arrangeQueue = new List<Element>();
        }

        /// <summary>
        /// Measure and arrange any invalidated elements in the queue..
        /// </summary>
        public void UpdateLayout()
        {
            foreach (var element in measureQueue.OrderBy(e => e.Level))
            {
                element.Measure(element.Parent == null || element.PreviousAvailableSize.IsEmpty
                    ? Size.Infinity // Fill by default.
                    : element.PreviousAvailableSize);
            }

            foreach (var element in arrangeQueue.OrderBy(e => e.Level))
            {
                element.Arrange(element.Parent == null || element.PreviousFinalRect.IsEmpty
                    ? new Rectangle(element.DesiredSize)
                    : element.PreviousFinalRect);
            }
        }

        /// <summary>
        /// Add an element to the arrange queue.
        /// </summary>
        public void AddArrange(Element element)
        {
            if (!arrangeQueue.Contains(element))
                arrangeQueue.Add(element);
        }

        /// <summary>
        /// Add an element to the measure queue.
        /// </summary>
        public void AddMeasure(Element element)
        {
            if (!measureQueue.Contains(element))
                measureQueue.Add(element);
        }

        /// <summary>
        /// Remove an element from the arrange queue.
        /// </summary>
        public void RemoveArrange(Element element) => arrangeQueue.Remove(element);

        /// <summary>
        /// Remove an element from the measure queue.
        /// </summary>
        public void RemoveMeasure(Element element) => measureQueue.Remove(element);

    }
}