using System;

namespace Laphed.MVP
{
    public interface IView<T>
    {
        event Action OnDispose;
        void UpdateView(T value);
    }
}