using System;
using System.Collections.Generic;

namespace IS.Reading
{
    public class StringDictionary
    {
        private readonly Dictionary<string, string> values
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string this[string key]
        {
            get
            {
                if (values.TryGetValue(key, out var value))
                    return value;
                return string.Empty;
            }
            set => values[key] = value;
        }

        public string Set(string key, string newValue)
        {
            var oldValue = this[key];
            this[key] = newValue;
            return oldValue;
        }
    }
}
