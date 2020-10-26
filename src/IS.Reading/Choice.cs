namespace IS.Reading
{
    public struct Choice
    {
        public string Text { get; }
        public string Tip { get; }
        public string Value { get; }
        public ICondition? Condition { get; }
        public Choice(string value, string text, string tip, ICondition? condition)
        {
            Value = value;
            Text = text;
            Tip = tip;
            Condition = condition;
        }
    }
}
