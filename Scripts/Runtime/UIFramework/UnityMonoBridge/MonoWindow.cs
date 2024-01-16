using UnityEngine;

namespace UIFramework
{
    public sealed class MonoWindow: MonoBehaviour, IWindow
    {
        [SerializeField] private MonoBinder binder;
        [SerializeField] private MonoWindow[] subwindows;
        
        private bool isInitialized;

        public void Initialize()
        {
            if (isInitialized) return;
            
            isInitialized = true;
            binder.Bind();
            
            foreach (MonoWindow window in subwindows)
            {
                window.Initialize();
            }
        }

        public void SetSubwindows(MonoWindow[] subwindows) => this.subwindows = subwindows;

        #region Dispose Pattern
        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            Destroy(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                foreach (MonoWindow window in subwindows)
                {
                    window.Dispose();
                }

                subwindows = null;
            }
            
            binder.Dispose();
            binder = null;
            disposed = true;
        }

        private void OnDestroy()
        {
            Dispose(false);
        }
        #endregion
    }
}