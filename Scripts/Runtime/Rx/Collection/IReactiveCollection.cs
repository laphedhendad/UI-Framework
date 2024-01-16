using System.Collections.Generic;

namespace Laphed.Rx
{
    public interface IReactiveCollection<T>: IList<T>, IReadonlyReactiveCollection<T>
    {
        new int Count { get; }
        new T this[int index] { get; set; }
    }
}