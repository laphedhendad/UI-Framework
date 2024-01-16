namespace UIFramework
{
    public abstract class UiBinder: IUiBinder
    {
        private bool disposed;

        public abstract void Bind();
        
        protected abstract void Unbind();

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            Unbind();
            disposed = true;
        }
    }
}