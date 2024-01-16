using System.Collections.Generic;

namespace Laphed.Rx.Dictionary
{
    public interface IReactiveDictionary<TKey, TValue> : IReadOnlyReactiveDictionary<TKey, TValue>, IDictionary<TKey, TValue>
    {
        new TValue this[TKey index] { get; set; }
    }
}