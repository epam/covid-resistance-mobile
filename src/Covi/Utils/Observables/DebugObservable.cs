#pragma warning disable SA1633 // File should have header
// no license
#pragma warning restore SA1633 // File should have header

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Covi
{
    public static class DebugObservable
    {
        /// <summary>
        /// Dumps all observable events into console.
        /// </summary>
        /// <typeparam name="T">Observable payload type.</typeparam>
        /// <param name="source">Source observable.</param>
        /// <param name="tag">Tag to be used in logs.</param>
        /// <returns>Observable.</returns>
        public static IObservable<T> Debug<T>(this IObservable<T> source, string tag = null)
        {
            var id = !string.IsNullOrEmpty(tag) ? tag : source.GetHashCode().ToString();
            Console.WriteLine($"{id}: Observable created on threadId: {System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()}.");

            return Observable.Create<T>(obs =>
                                        {
                                            Console.WriteLine(
                                                $"{id}: DebugObservable subscribed on threadId: {System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()}.");

                                            var subscription = source
                                                               .Do(
                                                                   value =>
                                                                       Console.WriteLine(
                                                                       $"{id}: OnNext({value?.ToString() ?? "<null>"}) on ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()}."),
                                                                   ex =>
                                                                       Console.WriteLine(
                                                                       $"{id}: OnError({ex}) on ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()}."),
                                                                   () =>
                                                                       Console.WriteLine(
                                                                       $"{id}: OnCompleted on ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()}."))
                                                               .Subscribe(obs);
                                            var dumpDisposable = Disposable.Create(() =>
                                                                                       Console.WriteLine(
                                                                                           $"{id}: Disposed on ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()}."));
                                            return new CompositeDisposable(subscription, dumpDisposable);
                                        });
        }
    }
}
