namespace Laphed.Rx
{
    public interface IReadonlyReactiveProperty<T>: IReactive
    {
        T Value { get; }
    }
}