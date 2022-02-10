using System.Collections;

namespace IS;

public abstract class TheoryData : IEnumerable<object[]>
{
    readonly List<object[]> data = new List<object[]>();

    protected void AddRow(params object[] values)
    {
        data.Add(values);
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        return data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class TheoryData<T1> : TheoryData
{
    public void Add(T1 p1) => AddRow(p1);
}

public class TheoryData<T1, T2> : TheoryData
{
    public void Add(T1 p1, T2 p2) => AddRow(p1, p2);
}

public class TheoryData<T1, T2, T3> : TheoryData
{
    public void Add(T1 p1, T2 p2, T3 p3) => AddRow(p1, p2, p3);
}
