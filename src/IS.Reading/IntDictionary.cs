using System;
using System.Collections.Generic;

namespace IS.Reading
{
    public class IntDictionary
    {
        private readonly Dictionary<string, int> values 
            = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public int this[string key]
        {
            get => values.TryGetValue(key, out var value) ? value : 0;
            set
            {
                if (value == 0)
                {
                    if (values.ContainsKey(key))
                        values.Remove(key);
                }
                else
                {
                    values[key] = value;
                }
            }
        }

        public int Set(string key, int newValue)
        {
            var oldValue = this[key];
            this[key] = newValue;
            return oldValue;
        }
    }
}
