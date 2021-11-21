namespace IS.Reading
{
    public struct VarIncrement
    {
        public string Name { get; }
        public int Value { get; }

        public VarIncrement(string key, int increment)
        {
            Name = key;
            Value = increment;
        }

        public override string ToString() => $"{Name}{Value:+0;-0}";

        public static VarIncrement? Parse(string text)
        {
            var key = text;
            var increment = 0;

            for (var n = 0; n < key.Length; n++)
            {
                var c = key[n];
                if (c == '+')
                    increment++;
                else if (c == '-')
                    increment--;
                else
                {
                    if (n > 0)
                        key = key.Substring(n);
                    break;
                }
            }

            for (var n = 0; n < key.Length; n++)
            {
                var c = key[n];
                if (c == '+' || c == '-')
                {
                    var s = key.Substring(n);
                    if (s == "+")
                        increment++;
                    else if (s == "-")
                        increment--;
                    else if (int.TryParse(s, out var i))
                        increment += i;
                    else
                        return null;
                    key = key.Substring(0, n);
                    break;
                }
            }

            return new VarIncrement(key, increment == 0 ? 1 : increment);
        }

        public const string Pattern = @"(^[\+\-]+[a-z0-9_]+$|^[a-z0-9_]+[\+\-]\d{0,9}$)";
    }
}
