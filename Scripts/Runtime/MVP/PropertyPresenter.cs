using Laphed.Rx;

namespace Laphed.MVP
{
    public abstract class PropertyPresenter<TModelData, TViewData>: ReactivePresenterBase<TViewData, IReactiveProperty<TModelData>>
    {
        protected PropertyPresenter(IView<TViewData> view): base(view)
        {
        }

        public override void SubscribeModel(IReactiveProperty<TModelData> model)
        {
            base.SubscribeModel(model);
            UpdateView(model.Value);
        }

        protected override void HandleModelUpdate() => UpdateView(model.Value);

        protected abstract void UpdateView(TModelData value);
    }

    public class PropertyPresenter<TData>: PropertyPresenter<TData, TData>
    {
        public PropertyPresenter(IView<TData> view) : base(view)
        {
        }

        protected override void UpdateView(TData value) => view.UpdateView(value);
    }
}