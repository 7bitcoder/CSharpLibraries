public interface IIterator<T>
{
    public T Current { get; }
    public bool IsValid();
    public IIterator<T> Next();

    public static IIterator<T> operator ++(IIterator<T> it) => it.Next();
    // public static T operator *(IIterator<T> it) => it.Current;
    // public static explicit operator bool(IIterator<T> it) => it.IsValid();
    public static bool operator true(IIterator<T> it) => it.IsValid();
    public static bool operator false(IIterator<T> it) => !it.IsValid();
};

public class ListIterator<T> : IIterator<T>
{
    private int _index = 0;
    private List<T> _list;
    public ListIterator(List<T> list) => _list = list;
    public T Current => _list[_index];
    public bool IsValid() => _index >= 0 && _index < _list.Count;
    public IIterator<T> Next()
    {
        _index++;
        return this;
    }
}

public static class ContainerExtensions
{
    public static IIterator<T> GetIterator<T>(this List<T> list)
    {
        return new ListIterator<T>(list);
    }
}