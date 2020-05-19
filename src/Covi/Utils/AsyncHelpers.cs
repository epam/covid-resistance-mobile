#pragma warning disable SA1120 // Comments should contain text
//
#pragma warning restore SA1120 // Comments should contain text

using System;

using System.Threading;
using System.Threading.Tasks;

namespace Covi
{
    public static class AsyncHelpers
    {
        private static readonly TaskFactory TaskFactory = new TaskFactory(CancellationToken.None,
            TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func) =>
            TaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        public static void RunSync(Func<Task> func) =>
            TaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
    }
}
