using Laphed.Rx.Dictionary;

namespace Laphed.MVP
{
    public abstract class DictionaryPresenter<TKey, TValue, TViewData>: ReactivePresenterBase<TViewData, IReadOnlyReactiveDictionary<TKey, TValue>>
    {
        protected DictionaryPresenter(IView<TViewData> view) : base(view)
        {
        }

        public override void SubscribeModel(IReadOnlyReactiveDictionary<TKey, TValue> model)
        {
            base.SubscribeModel(model);
            UpdateView(model);
        }

        protected override void HandleModelUpdate() => UpdateView(model);

        protected abstract void UpdateView(IReadOnlyReactiveDictionary<TKey, TValue> model);
    }
    
    public class DictionaryPresenter<TKey, TValue>: DictionaryPresenter<TKey, TValue, IReadOnlyReactiveDictionary<TKey, TValue>>
    {
        protected DictionaryPresenter(IView<IReadOnlyReactiveDictionary<TKey, TValue>> view) : base(view)
        {
        }

        protected override void HandleModelUpdate() => UpdateView(model);

        protected override void UpdateView(IReadOnlyReactiveDictionary<TKey, TValue> model) => view.UpdateView(model);
    }
}