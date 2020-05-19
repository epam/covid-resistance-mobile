#pragma warning disable SA1633 // File should have header
using System;
#pragma warning restore SA1633 // File should have header

namespace Covi
{
    public static class WeakObservableExtensions
    {
        /// <summary>
        /// Makes a weak subscription of <paramref name="observer"/> to the <paramref name="observable"/>.
        /// </summary>
        /// <typeparam name="T">Payload type.</typeparam>
        /// <param name="observable">Observable.</param>
        /// <param name="observer">Observer.</param>
        /// <returns>Observable with weak reference.</returns>
        public static IDisposable WeakSubscribe<T>(
            this IObservable<T> observable, IObserver<T> observer)
        {
            return new WeakSubscription<T>(observable, observer);
        }

        private class WeakSubscription<T> : IDisposable, IObserver<T>
        {
            private readonly WeakReference<IObserver<T>> _observerWeakReference;
            private readonly IDisposable _subscription;
            private bool _disposed;

            public WeakSubscription(IObservable<T> observable, IObserver<T> observer)
            {
                _observerWeakReference = new WeakReference<IObserver<T>>(observer);
                _subscription = observable.Subscribe(this);
            }

            void IObserver<T>.OnCompleted()
            {
                if (_observerWeakReference.TryGetTarget(out var observer))
                {
                    observer.OnCompleted();
                }
                else
                {
                    Dispose();
                }
            }

            void IObserver<T>.OnError(Exception error)
            {
                if (_observerWeakReference.TryGetTarget(out var observer))
                {
                    observer.OnError(error);
                }
                else
                {
                    Dispose();
                }
            }

            void IObserver<T>.OnNext(T value)
            {
                if (_observerWeakReference.TryGetTarget(out var observer))
                {
                    observer.OnNext(value);
                }
                else
                {
                    Dispose();
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        _subscription.Dispose();
                    }

                    _disposed = true;
                }
            }
        }
    }
}
