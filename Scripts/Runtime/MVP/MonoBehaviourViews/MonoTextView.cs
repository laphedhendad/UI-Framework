namespace Laphed.MVP.MonoViews
{
    public abstract class MonoTextView: MonoView<string>
    {
        protected abstract string Text { set; }
        public override void UpdateView(string value) => Text = value;
    }
}