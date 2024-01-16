using System;

namespace UIFramework
{
    public class Window: IWindow
    {
        private IUiBinder binder;
        private IWindow[] subwindows;
        private bool isInitialized;

        public Window(IUiBinder binder)
        {
            this.binder = binder;
            subwindows = Array.Empty<IWindow>();
        }
        
        public Window(IUiBinder binder, IWindow[] subwindows): this(binder)
        {
            this.subwindows = subwindows;
        }
        
        public void Initialize()
        {
            if (isInitialized) return;
            
            isInitialized = true;
            binder.Bind();
            
            foreach (IWindow window in subwindows)
            {
                window.Initialize();
            }
        }

        #region Dispose Pattern
        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                foreach (IWindow window in subwindows)
                {
                    window.Dispose();
                }

                subwindows = null;
            }
            
            binder.Dispose();
            binder = null;
            disposed = true;
        }

        ~Window()
        {
            Dispose(false);
        }
        #endregion
    }
}