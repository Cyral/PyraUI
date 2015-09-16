using System;

namespace Pyratron.UI.Types
{
    /// <summary>
    /// Represents a dimension of an element. (width or height).
    /// </summary>
    public class Dimension
    {
        /// <summary>
        /// Signifies that the element's width will be automatic (not specified explicitly).
        /// </summary>
        public bool Auto { get; set; }

        /// <summary>
        /// The minimum value.
        /// </summary>
        public int Min
        {
            get { return min; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Minimum must be greater than zero.");
                if (value != min)
                {
                    if (target < min && value < min)
                    {
                        min = value;
                        Value = Math.Max(target, min);
                    }
                    else
                        min = value;
                    if (Value < min)
                        Value = min;
                }
            }
        }

        /// <summary>
        /// The maximum value.
        /// </summary>
        public int Max
        {
            get { return max; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Maximum must be greater than zero.");
                if (value != max)
                {
                    if (target > max && value > max)
                    {
                        max = value;
                        Value = Math.Min(target, max);
                    }
                    else
                        max = value;
                    if (Value < max)
                        Value = max;
                }
            }
        }

        public int Value
        {
            get { return value; }
            set
            {
                // Check bounds.
                target = value;
                this.value = Auto ? value : Math.Max(min, Math.Min(value, max));
            }
        }

        private int value, min, max, target;

        public Dimension(int value, bool auto = true)
        {
            Auto = auto;
            Value = Min = Max =  value;
        }

        public Dimension(int value, int min, int max, bool auto = true)
        {
            Min = min;
            Max = max;
            Auto = auto;
            Value = value;
        }

        public static implicit operator Dimension(int value) => new Dimension(value);
        public static implicit operator int(Dimension dimension) => dimension.Value;
        public static explicit operator Dimension(string value) => new Dimension(int.Parse(value));
    }
}
