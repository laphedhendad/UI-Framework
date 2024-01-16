using System;
using System.Collections.Generic;

namespace Laphed.Rx.Dictionary
{
    public interface IReadOnlyReactiveDictionary<TKey, TValue>: IEnumerable<KeyValuePair<TKey, TValue>>, IReactive
    {
        int Count { get; }
        TValue this[TKey index] { get; }
        bool ContainsKey(TKey key);
        bool TryGetValue(TKey key, out TValue value);

        event Action<TKey> OnAdd;
        event Action<TKey> OnRemove;
        event Action<TKey> OnReplace;
        event Action OnCleared;
    }
}