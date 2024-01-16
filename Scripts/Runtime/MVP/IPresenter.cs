using System;

namespace Laphed.MVP
{
    public interface IPresenter<TModel>: IDisposable
    {
        void SubscribeModel(TModel model);
    }
}