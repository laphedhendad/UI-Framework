using System;
using UnityEngine;

namespace Laphed.MVP.MonoViews
{
    public abstract class MonoView<T>: MonoBehaviour, IView<T>
    {
        public event Action OnDispose;
        public abstract void UpdateView(T value);
        protected virtual void OnDestroy() => OnDispose?.Invoke();
    }
}