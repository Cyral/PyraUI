using System;
using System.Linq;

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

        public Control(Manager manager) : base(manager)
        {

        }
    }
}