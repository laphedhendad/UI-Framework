using System;

namespace UIFramework
{
    public interface IUiBinder: IDisposable
    {
        void Bind();
    }
}