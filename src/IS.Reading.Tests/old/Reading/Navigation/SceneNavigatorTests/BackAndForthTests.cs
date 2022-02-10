namespace IS.Reading.Navigation.SceneNavigatorTests;

public class BackAndForthTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Empty(bool forward)
    {
        var tester = new BackAndForthTester(forward);
        await tester.MoveAsync(forward, null);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task NoPause(bool forward)
    {
        var tester = new BackAndForthTester(forward);
        tester.AddDummy();
        tester.AddDummy();
        tester.AddDummy();
        await tester.MoveAsync(forward, null);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task SinglePause(bool forward)
    {
        var tester = new BackAndForthTester(forward);
        tester.AddPause("s1");
        await tester.MoveAsync(forward, "s1", null);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task SinglePauseBetweenDummies(bool forward)
    {
        var tester = new BackAndForthTester(forward);
        tester.AddDummy();
        tester.AddPause("s1");
        tester.AddDummy();
        await tester.MoveAsync(forward, "s1", null);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TwoPauses(bool forward)
    {
        var tester = new BackAndForthTester(forward);
        tester.AddDummy();
        tester.AddPause("s1");
        tester.AddDummy();
        tester.AddPause("s2");
        tester.AddDummy();
        await tester.MoveAsync(forward, "s1", "s2", null);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ThreePauses(bool forward)
    {
        var tester = new BackAndForthTester(forward);
        tester.AddDummy();
        tester.AddDummy();
        tester.AddDummy();
        tester.AddPause("s1");
        tester.AddPause("s2");
        tester.AddDummy();
        tester.AddDummy();
        tester.AddDummy();
        tester.AddDummy();
        tester.AddDummy();
        tester.AddDummy();
        tester.AddDummy();
        tester.AddDummy();
        tester.AddDummy();
        tester.AddPause("s3");
        await tester.MoveAsync(forward, "s1", "s2", "s3", null);
    }

}
