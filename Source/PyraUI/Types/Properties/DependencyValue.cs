namespace Pyratron.UI.Types.Properties
{
    public class DependencyValue
    {
        public static readonly object Unset = UnsetValue.Value;

        private object value;

        public DependencyValue(object value)
        {
            this.value = value;
        }

        public object GetValue()
        {
            return value;
        }

        public void SetValue(object value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        private class UnsetValue
        {
            internal static readonly UnsetValue Value;
            static UnsetValue()
            {
                Value = new UnsetValue();
            }

            public override string ToString()
            {
                return "(Not set)";
            }
        }
    }
}