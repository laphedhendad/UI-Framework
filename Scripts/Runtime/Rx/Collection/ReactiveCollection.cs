using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Laphed.Rx
{
    public class ReactiveCollection<T>: Collection<T>, IReactiveCollection<T>
    {
        public event Action OnChanged;
        public event Action<int> OnAdd;
        public event Action<int> OnRemove;
        public event Action<int> OnReplace;
        public event Action OnCleared;
        
        public ReactiveCollection()
        {
        }

        public ReactiveCollection(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            foreach (var item in collection)
            {
                Add(item);
            }
        }

        protected override void ClearItems()
        {
            if (Count == 0) return;
            base.ClearItems();
            OnCleared?.Invoke();
            OnChanged?.Invoke();
        }
        
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            OnAdd?.Invoke(index);
            OnChanged?.Invoke();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            OnRemove?.Invoke(index);
            OnChanged?.Invoke();
        }

        protected override void SetItem(int index, T item)
        {
            if (EqualityComparer<T>.Default.Equals(this[index], item)) return;
            base.SetItem(index, item);
            OnReplace?.Invoke(index);
            OnChanged?.Invoke();
        }
    }
}