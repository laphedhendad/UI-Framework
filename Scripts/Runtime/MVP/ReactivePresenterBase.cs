using Laphed.Rx;

namespace Laphed.MVP
{
    public abstract class ReactivePresenterBase<TViewData, TReactive>: PresenterBase<TReactive, TViewData> where TReactive: IReactive
    {
        protected ReactivePresenterBase(IView<TViewData> view): base(view)
        {
            view.OnDispose += Dispose;
        }
        
        public override void SubscribeModel(TReactive model)
        {
            if (model == null) return;
            this.model = model;
            model.OnChanged += HandleModelUpdate;
        }

        protected override void UnsubscribeModel()
        {
            if (model == null) return;
            model.OnChanged -= HandleModelUpdate;
        }
    }
}