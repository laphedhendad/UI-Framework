using System;
using System.Collections.Generic;

namespace Laphed.Rx
{
    public class ReactiveProperty<T>: IReactiveProperty<T>
    {
        public event Action OnChanged;
        private T currentValue;

        public T Value
        {
            get => currentValue;
            set
            {
                if (EqualityComparer<T>.Default.Equals(value, currentValue)) return;
                currentValue = value;
                OnChanged?.Invoke();
            }
        }
    }
}