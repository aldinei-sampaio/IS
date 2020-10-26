using System.Text.RegularExpressions;

namespace IS.Reading.Conditions
{
    public static class ConditionParser
    {
        // Padrões suportados:
        // - variavel
        // - variavel[]
        // - variavel[0]
        // - variavel[0:]
        // - variavel[0:1]
        // - variavel[:1]
        // - var1,var2
        // - Os números dentro dos colchetes podem ser negativos (precedidos por "-")
        // - A condição toda pode ser negada incluindo um ! no início do texto
        private const string RegexPattern1 = @"^(?<var>[A-Za-z0-9_]+(\,[A-Za-z0-9_]+)*)$";
        private const string RegexPattern2 = @"^(?<var>[A-Za-z0-9_]+)(\[(?<min>\-?\d{0,9})?(?<max>\:\-?\d{0,9})?\])?$";

        public static Condition? Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            var negate = text.StartsWith("!");
            if (negate)
                text = text.Substring(1);

            var match = Regex.Match(text, RegexPattern1);
            if (match.Success)
                return GetCondition(negate, match.Groups["var"].Value);

            match = Regex.Match(text, RegexPattern2);

            if (!match.Success)
                return null;

            return GetCondition(
                negate,
                match.Groups["var"].Value,
                match.Groups["min"].Value,
                match.Groups["max"].Value
            );
        }

        private static Condition GetCondition(bool negate, string text)
        {
            var names = text.Split(',');
            var op = negate ? ConditionType.EqualTo : ConditionType.Defined;
            return new Condition(names, op, 0, 0);
        }

        private static Condition? GetCondition(bool negate, string text, string min, string max)
        {
            var names = new[] { text };
            ConditionType op;
            var value = 0;
            var value2 = 0;

            if (min.Length == 0)
            {
                if (max.Length == 0)
                {
                    op = negate ? ConditionType.EqualTo : ConditionType.Defined;
                }
                else
                {
                    op = negate ? ConditionType.GreaterThan : ConditionType.EqualOrLessThan;
                    if (!int.TryParse(max, out value))
                        return null;
                }
            }
            else
            {
                if (max.Length == 0)
                {
                    op = negate ? ConditionType.LessThan : ConditionType.EqualOrGreaterThan;
                    if (!int.TryParse(min, out value))
                        return null;
                }
                else
                {
                    op = negate ? ConditionType.NotBetween : ConditionType.Between;
                    if (!int.TryParse(min, out value) || !int.TryParse(max, out value2))
                        return null;
                }
            }

            return new Condition(names, op, value, value2);
        }

    }
}
