using System;
using System.Runtime.InteropServices;

namespace SDL
{
    public abstract class Resource<ExceptionType> : IDisposable
    where ExceptionType : Exception, new()
    {
        public const string SDLLib = SDLContext.SDLLib;
        public const string TTFLib = SDLContext.TTFLib;

        internal IntPtr handle { get; private set; }

        public bool Disposed { get; private set; }
        public event EventHandler Disposing;

        private bool free = true;

        protected Resource(IntPtr resource, bool managed = true, bool safe = true)
        {
            free = managed;
            if(safe) EnsureSafe(resource);
            handle = resource;
            Disposed = false;
        }

        ~Resource() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;

            if (disposing) {
                EventHandler handler = Disposing;
                if (handler != null) handler(this, new EventArgs());
            }

            if (free) Free(handle);

            Disposed = true;
        }

        protected abstract void Free(IntPtr handle);

        protected static void EnsureSafe(int status)
        {
            if (status != 0) throw new ExceptionType();
        }

        protected static void EnsureSafe(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) throw new ExceptionType();
        }
    }
}
