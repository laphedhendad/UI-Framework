using System;

namespace Laphed.Rx
{
    public interface IReactive
    {
        event Action OnChanged;
    }
}