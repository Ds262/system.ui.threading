using System;
using System.Theading;
namespace System.UIThreading{
public class UIProgress<T> : IDisposable,IProgress<T>
    {
        private bool disposedValue;
        private IProgress<T> v;
        private T Value;
        private EventWaitHandle handle = new EventWaitHandle(false, EventResetMode.AutoReset);
        public UIProgress(Action<T> action)
            {
            v = new Progress<T>(action);
            ThreadPool.QueueUserWorkItem((r) => {while (true)
                {
                    handle.WaitOne();if (v != null)
                    {
                        v.Report(Value);
                    }
                }
            });
        }
        public void Report(T val)
        {
            Value = val;
            handle.Set();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    v = null;
                    handle = null;
        // TODO: dispose managed state (managed objects)
                }
        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
            disposedValue = true;
            }
        }
        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~UIProgress()
        {// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }
        public void Dispose()
        {// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
     public static class UIExtensions
    {
        public static SynchronizationContext Synchronization { get; set; } = SynchronizationContext.Current;

        public static UIProgress<Action> GetInvoke(this FrameworkElement element)
        {
            UIProgress<Action> a=null;
            Synchronization.Send((r) =>{ a= new UIProgress<Action>((d) => d()); },null);
            return a;
        }
        public static UIProgress<T> GetProgress<T>(this FrameworkElement element, Action<T> action)
        {
            UIProgress<T> a=null;
            Synchronization.Send((r) => { a = new UIProgress<T>(d => action(d)); }, null);

            return a;
            
        }
    }
}
