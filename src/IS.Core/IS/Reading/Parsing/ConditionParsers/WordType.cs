namespace IS.Reading.Parsing.ConditionParsers;

public enum WordType
{
    None,
    Invalid,
    OpenParenthesys,
    CloseParenthesys,
    Variable,
    String,
    Number,
    Not,
    And,
    Or,
    Equals,
    NotEqualsTo,
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

