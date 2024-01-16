using System;
using System.Collections.Generic;

namespace Laphed.Rx
{
    public interface IReadonlyReactiveCollection<T>: IEnumerable<T>, IReactive
    {
        int Count { get; }
        T this[int index] { get; }
        event Action<int> OnAdd;
        event Action<int> OnRemove;
        event Action<int> OnReplace;
        event Action OnCleared;
    }
}