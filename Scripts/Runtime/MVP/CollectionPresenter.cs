using Laphed.Rx;

namespace Laphed.MVP
{
    public abstract class CollectionPresenter<TModelData, TViewData>: ReactivePresenterBase<TViewData, IReadonlyReactiveCollection<TModelData>>
    {
        protected CollectionPresenter(IView<TViewData> view): base(view)
        {
        }

        public override void SubscribeModel(IReadonlyReactiveCollection<TModelData> model)
        {
            base.SubscribeModel(model);
            UpdateView(model);
        }

        protected override void HandleModelUpdate() => UpdateView(model);
        protected abstract void UpdateView(IReadonlyReactiveCollection<TModelData> model);
    }
    
    public class CollectionPresenter<TData>: CollectionPresenter<TData, IReadonlyReactiveCollection<TData>>
    {
        protected CollectionPresenter(IView<IReadonlyReactiveCollection<TData>> view): base(view)
        {
        }

        protected override void UpdateView(IReadonlyReactiveCollection<TData> model) => view.UpdateView(model);
    }
}