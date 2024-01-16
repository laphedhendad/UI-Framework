using Laphed.Rx;

namespace Laphed.MVP
{
    public abstract class PresenterBase<TReactive, TViewData>: IPresenter<TReactive> where TReactive: IReactive
    {
        protected IView<TViewData> view;
        protected TReactive model;

        protected PresenterBase(IView<TViewData> view)
        {
            this.view = view;
            view.OnDispose += Dispose;
        }

        public abstract void SubscribeModel(TReactive model);

        protected abstract void HandleModelUpdate();

        public virtual void Dispose()
        {
            view.OnDispose -= Dispose;
            UnsubscribeModel();
        }

        protected abstract void UnsubscribeModel();
    }
}