using System;
using System.Linq;

namespace Pyratron.UI.Controls
{
    /// <summary>
    /// A visual UI control that can be interacted with.
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