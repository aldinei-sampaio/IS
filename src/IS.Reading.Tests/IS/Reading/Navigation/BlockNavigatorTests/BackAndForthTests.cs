﻿namespace IS.Reading.Navigation.BlockNavigatorTests;

public class BackAndForthTests
{
    [Fact]
    public async void SingleNode()
    {
        var tester = new BackAndForthTester();
        tester.AddNode("normal", "reversed");

        for (var n = 1; n <= 3; n++)
        {
            await tester.MoveAsync(true, "normal", null);
            await tester.MoveAsync(false, "reversed", null);
        }
    }

    [Fact]
    public async void SingleNode_BackAndForth()
    {
        var tester = new BackAndForthTester();
        tester.AddNode("normal", "reversed");

        for (var n = 1; n <= 3; n++)
        {
            await tester.MoveAsync(true, "normal");
            await tester.MoveAsync(false, "reversed", null);
            await tester.MoveAsync(true, "normal", null);

            await tester.MoveAsync(false, "reversed");
            await tester.MoveAsync(true, "normal", null);
            await tester.MoveAsync(false, "reversed", null);
        }
    }

    [Fact]
    public async void TwoNodes()
    {
        var tester = new BackAndForthTester();
        tester.AddNode("n1", "r1");
        tester.AddNode("n2", "r2");

        for (var n = 1; n <= 3; n++)
        {
            await tester.MoveAsync(true, "n1", "n2", null);
            await tester.MoveAsync(false, "r2", "r1", null);
        }
    }

    [Fact]
    public async void TwoNodes_BackAndForth()
    {
        var tester = new BackAndForthTester();
        tester.AddNode("n1", "r1");
        tester.AddNode("n2", "r2");

        for (var n = 1; n <= 3; n++)
        {
            await tester.MoveAsync(true, "n1");
            await tester.MoveAsync(false, "r1", null);
            await tester.MoveAsync(true, "n1", "n2");
            await tester.MoveAsync(false, "r2", "r1", null);
            await tester.MoveAsync(true, "n1", "n2", null);

            await tester.MoveAsync(false, "r2");
            await tester.MoveAsync(true, "n2", null);
            await tester.MoveAsync(false, "r2", "r1");
            await tester.MoveAsync(true, "n1", "n2", null);
            await tester.MoveAsync(false, "r2", "r1", null);
        }
    }

    [Fact]
    public async void ThreeNodes()
    {
        var tester = new BackAndForthTester();
        tester.AddNode("n1", "r1");
        tester.AddNode("n2", "r2");
        tester.AddNode("n3", "r3");

        for (var n = 1; n <= 3; n++)
        {
            await tester.MoveAsync(true, "n1", "n2", "n3", null);
            await tester.MoveAsync(false, "r3", "r2", "r1", null);
        }
    }

    [Fact]
    public async void ThreeNodes_BackAndForth()
    {
        var tester = new BackAndForthTester();
        tester.AddNode("n1", "r1");
        tester.AddNode("n2", "r2");
        tester.AddNode("n3", "r3");

        for (var n = 1; n <= 3; n++)
        {
            await tester.MoveAsync(true, "n1");
            await tester.MoveAsync(false, "r1", null);
            await tester.MoveAsync(true, "n1", "n2");
            await tester.MoveAsync(false, "r2", "r1", null);
            await tester.MoveAsync(true, "n1", "n2", "n3");
            await tester.MoveAsync(false, "r3", "r2", "r1", null);
            await tester.MoveAsync(true, "n1", "n2", "n3", null);

            await tester.MoveAsync(false, "r3");
            await tester.MoveAsync(true, "n3", null);
            await tester.MoveAsync(false, "r3", "r2");
            await tester.MoveAsync(true, "n2", "n3", null);
            await tester.MoveAsync(false, "r3", "r2", "r1");
            await tester.MoveAsync(true, "n1", "n2", "n3", null);
            await tester.MoveAsync(false, "r3", "r2", "r1", null);
        }
    }
}
