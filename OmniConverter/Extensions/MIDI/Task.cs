using System;

namespace OmniConverter
{
    public abstract class OmniTask : IDisposable
    {
        protected bool _disposed = false;

        public abstract double Progress { get; }
        public abstract double Remaining { get; }
        public abstract double Length { get; }
        public abstract double Processed { get; }
        public abstract void RefreshInfo();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);
    }
}
