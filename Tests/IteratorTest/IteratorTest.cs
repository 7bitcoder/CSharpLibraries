using Library;

namespace Tests.DependencyInjectorTest;

public class IteratorTest
{
    [Fact]
    public void ShouldIterateOverList()
    {
        var result = new List<int>();
        var list = new List<int> { 1, 2, 3, 4 };
        for (var it = list.GetIterator(); it; it++)
        {
            result.Add(it.Current);
        }

        result.Should().BeEquivalentTo(new[] { 1, 2, 3, 4 });
    }
}