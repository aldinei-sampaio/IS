namespace IS.Reading.Parsing.ConditionParsers;

public enum WordType
{
    None,
    Invalid,
    OpenParenthesys,
    CloseParenthesys,
    Identifier,
    String,
    Number,
    Not,
    And,
    Or,
    Equals,
    Different,
    GreaterThan,
    EqualOrGreaterThan,
    LowerThan,
    EqualOrLowerThan,
    In,
    Between,
    Comma,
    Is,
    Null
}

