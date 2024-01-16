using System;
using System.Collections;
using System.Collections.Generic;

namespace Laphed.Rx.Dictionary
{
    [Serializable]
    public class ReactiveDictionary<TKey, TValue> : IReactiveDictionary<TKey, TValue>, IDictionary
    {
        readonly Dictionary<TKey, TValue> inner;

        public event Action<TKey> OnAdd;
        public event Action<TKey> OnRemove;
        public event Action<TKey> OnReplace;
        public event Action OnCleared;
        public event Action OnChanged;

        public ReactiveDictionary()
        {
            inner = new Dictionary<TKey, TValue>();
        }

        public ReactiveDictionary(IEqualityComparer<TKey> comparer)
        {
            inner = new Dictionary<TKey, TValue>(comparer);
        }

        public ReactiveDictionary(Dictionary<TKey, TValue> innerDictionary)
        {
            inner = innerDictionary;
        }

        public TValue this[TKey key]
        {
            get => inner[key];
            set
            {
                if (TryGetValue(key, out var oldValue))
                {
                    inner[key] = value;
                    OnReplace?.Invoke(key);
                    return;
                }
                
                inner[key] = value;
                OnAdd?.Invoke(key);
                OnChanged?.Invoke();
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => inner.Keys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => inner.Values;

        ICollection IDictionary.Values => inner.Values;
        ICollection IDictionary.Keys => inner.Keys;


        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index) => ((IDictionary)inner).CopyTo(array, index);

        public int Count => inner.Count;
        public bool IsSynchronized => ((IDictionary)inner).IsSynchronized;
        public object SyncRoot => ((ICollection)inner).SyncRoot;
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((IDictionary)inner).IsReadOnly;

        public object this[object key]
        {
            get => this[(TKey)key];
            set => this[(TKey)key] = (TValue)value;
        }

        public void Add(TKey key, TValue value)
        {
            inner.Add(key, value);
            OnAdd?.Invoke(key);
            OnChanged?.Invoke();
        }

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public void Add(object key, object value) => Add((TKey)key, (TValue)value);

        public void Clear()
        {
            if (inner.Count == 0) return;
            
            inner.Clear();
            OnCleared?.Invoke();
            OnChanged?.Invoke();
        }

        public bool Contains(object key) => inner.ContainsKey((TKey)key);

        IDictionaryEnumerator IDictionary.GetEnumerator() => inner.GetEnumerator();

        public void Remove(object key) => Remove((TKey)key);

        public bool IsFixedSize => ((IDictionary)inner).IsFixedSize;

        bool IDictionary.IsReadOnly => ((IDictionary)inner).IsReadOnly;

        public bool Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)inner).Contains(item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)inner).CopyTo(array, arrayIndex);

        public bool Remove(TKey key)
        {
            if (!inner.TryGetValue(key, out var oldValue)) return false;
            var isSuccessRemove = inner.Remove(key);
            if (!isSuccessRemove) return false;
            OnRemove?.Invoke(key);
            OnChanged?.Invoke();
            return true;
        }

        public bool ContainsKey(TKey key) => inner.ContainsKey(key);
        
        public bool TryGetValue(TKey key, out TValue value) => inner.TryGetValue(key, out value);
        
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => inner.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();
    }
}
