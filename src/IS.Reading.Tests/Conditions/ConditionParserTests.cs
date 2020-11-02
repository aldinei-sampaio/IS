using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IS.Reading.Conditions
{
    public class ConditionParserTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("ab-cde")]
        [InlineData("abcde[")]
        [InlineData("ab cde")]
        [InlineData("abcde[ ]")]
        [InlineData("abcde[1: 2]")]
        [InlineData("abcde[ 1:2]")]
        [InlineData("abcde[1 :2]")]
        [InlineData("abcde [1:2]")]
        [InlineData("abcde[1:2] ")]
        [InlineData(" abcde[1:2]")]
        [InlineData("[0]")]
        [InlineData("[0:]")]
        [InlineData("[0:1]")]
        [InlineData("[:1]")]
        [InlineData("abc[2147483647]")]
        [InlineData("abc[1000000000:]")]
        [InlineData("abc[:1234567890]")]
        [InlineData("abc[2147483647:2147483647]")]
        public void EmptyOrInvalid(string expression)
        {
            ConditionParser.Parse(expression).Should().BeNull();
        }

        [Theory]
        [InlineData("a", "a", 0, 0, ConditionType.Defined)]
        [InlineData("!a", "a", 0, 0, ConditionType.Undefined)]
        [InlineData("rage", "rage", 0, 0, ConditionType.Defined)]
        [InlineData("rage[]", "rage", 0, 0, ConditionType.Defined)]
        [InlineData("!rage[]", "rage", 0, 0, ConditionType.Undefined)]
        [InlineData("rage[0]", "rage", 0, 0, ConditionType.EqualTo)]
        [InlineData("!rage[0]", "rage", 0, 0, ConditionType.NotEqualTo)]
        [InlineData("rage[-5]", "rage", -5, 0, ConditionType.EqualTo)]
        [InlineData("!rage[-5]", "rage", -5, 0, ConditionType.NotEqualTo)]
        [InlineData("rage[999999999]", "rage", 999999999, 0, ConditionType.EqualTo)]
        [InlineData("rage[-999999999]", "rage", -999999999, 0, ConditionType.EqualTo)]
        [InlineData("rage[8:]", "rage", 8, 0, ConditionType.EqualOrGreaterThan)]
        [InlineData("!rage[8:]", "rage", 8, 0, ConditionType.LessThan)]
        [InlineData("rage[-5:]", "rage", -5, 0, ConditionType.EqualOrGreaterThan)]
        [InlineData("!rage[-5:]", "rage", -5, 0, ConditionType.LessThan)]
        [InlineData("rage[999999999:]", "rage", 999999999, 0, ConditionType.EqualOrGreaterThan)]
        [InlineData("rage[-999999999:]", "rage", -999999999, 0, ConditionType.EqualOrGreaterThan)]
        [InlineData("rage[:13]", "rage", 13, 0, ConditionType.EqualOrLessThan)]
        [InlineData("!rage[:13]", "rage", 13, 0, ConditionType.GreaterThan)]
        [InlineData("rage[:-24]", "rage", -24, 0, ConditionType.EqualOrLessThan)]
        [InlineData("!rage[:-24]", "rage", -24, 0, ConditionType.GreaterThan)]
        [InlineData("rage[:999999999]", "rage", 999999999, 0, ConditionType.EqualOrLessThan)]
        [InlineData("rage[:-999999999]", "rage", -999999999, 0, ConditionType.EqualOrLessThan)]
        [InlineData("rage[0:1]", "rage", 0, 1, ConditionType.Between)]
        [InlineData("fury[-1:2]", "fury", -1, 2, ConditionType.Between)]
        [InlineData("rage[-2:-1]", "rage", -2, -1, ConditionType.Between)]
        [InlineData("rage[1:-1]", "rage", 1, -1, ConditionType.Between)]
        [InlineData("!rage[0:1]", "rage", 0, 1, ConditionType.NotBetween)]
        [InlineData("rage[999999999:999999999]", "rage", 999999999, 999999999, ConditionType.Between)]
        [InlineData("rage[-999999999:999999999]", "rage", -999999999, 999999999, ConditionType.Between)]
        [InlineData("rage[999999999:-999999999]", "rage", 999999999, -999999999, ConditionType.Between)]
        [InlineData("rage[-999999999:-999999999]", "rage", -999999999, -999999999, ConditionType.Between)]
        public void Parse(string expression, string varname, int v1, int v2, ConditionType op)
        {
            var condition = ConditionParser.Parse(expression);
            condition.Should().NotBeNull();
            condition.Value.VariableNames.Should().BeEquivalentTo(varname);
            condition.Value.Value.Should().Be(v1);
            condition.Value.Value2.Should().Be(v2);
            condition.Value.Operator.Should().Be(op);
        }

        [Fact]
        public void MultipleVariables1()
        {
            var condition = ConditionParser.Parse("rage,fury");
            condition.Should().NotBeNull();
            condition.Value.VariableNames.Should().BeEquivalentTo("rage", "fury");
            condition.Value.Operator.Should().Be(ConditionType.Defined);
            condition.Value.Value.Should().Be(0);
            condition.Value.Value2.Should().Be(0);
        }

        [Fact]
        public void MultipleVariables2()
        {
            var condition = ConditionParser.Parse("!a,b,c");
            condition.Should().NotBeNull();
            condition.Value.VariableNames.Should().BeEquivalentTo("a", "b", "c");
            condition.Value.Operator.Should().Be(ConditionType.Undefined);
            condition.Value.Value.Should().Be(0);
            condition.Value.Value2.Should().Be(0);
        }
    }
}
