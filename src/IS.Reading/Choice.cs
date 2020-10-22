namespace IS.Reading
{
    public struct Choice
    {
        public string Caption { get; }
        public string? Condition { get; }
        public Choice(string caption, string condition)
        {
            Caption = caption;
            Condition = condition;
        }
    }
}
