namespace IS.Reading.Conditions
{

    public struct Condition : ICondition
    {
        public static Condition Empty { get; } = new();

        public string[] VariableNames { get; }
        public ConditionType Operator { get; }
        public int Value { get; }
        public int Value2 { get; }

        public Condition(string[] names, ConditionType op, int value, int value2)
        {
            VariableNames = names;
            Operator = op;
            Value = value;
            Value2 = value2;
        }

        public bool EvaluateFor(int value)
        {
            return Operator switch
            {
                ConditionType.Defined => value > 0,
                ConditionType.Undefined => value <= 0,
                ConditionType.EqualTo => value == Value,
                ConditionType.NotEqualTo => value != Value,
                ConditionType.EqualOrLessThan => value <= Value,
                ConditionType.LessThan => value < Value,
                ConditionType.EqualOrGreaterThan => value >= Value,
                ConditionType.GreaterThan => value > Value,
                ConditionType.Between => value >= Value && value <= Value2,
                ConditionType.NotBetween => value < Value || value > Value2,
                _ => value == Value,
            };
        }

        public override string ToString()
        {
            var joinedNames = string.Join(",", VariableNames);
            return Operator switch
            {
                ConditionType.Defined => $"{joinedNames}[1:]",
                ConditionType.Undefined => $"{joinedNames}[:0]",
                ConditionType.EqualTo => $"{joinedNames}[{Value}]",
                ConditionType.NotEqualTo => $"!{joinedNames}[{Value}]",
                ConditionType.EqualOrLessThan => $"{joinedNames}[:{Value}]",
                ConditionType.LessThan => $"!{joinedNames}[{Value}:]",
                ConditionType.EqualOrGreaterThan => $"{joinedNames}[{Value}:]",
                ConditionType.GreaterThan => $"!{joinedNames}[:{Value}]",
                ConditionType.Between => $"{joinedNames}[{Value}:{Value2}]",
                ConditionType.NotBetween => $"!{joinedNames}[{Value}:{Value2}]",
                _ => $"{joinedNames}[{Value}]",
            };
        }

        public bool Evaluate(IVariableDictionary variables)
        {
            if (VariableNames.Length == 1)
                return EvaluateFor(variables.Get(VariableNames[0]));

            var mustHaveDefined = Operator == ConditionType.Defined;

            foreach (var name in VariableNames)
            {
                var value = variables.Get(name);
                if (value != 0)
                    return mustHaveDefined;
            }

            return !mustHaveDefined;
        }
    }
}
