// =========================================================================
// Copyright 2020 EPAM Systems, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// =========================================================================

using System;
using System.Threading.Tasks;

namespace Covi
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Fires the <see cref="Task" /> and safely forget.
        /// </summary>
        /// <param name="task">The task.</param>
        public static void FireAndForget(this Task task)
        {
            task.FireAndForget(null);
        }

        /// <summary>
        /// Fires the <see cref="Task"/> and safely forget and in case of exception call <paramref name="onError"/> handler if any.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="onError">The on error action handler.</param>
        public static async void FireAndForget(this Task task, Action<Exception> onError)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
        }

        /// <summary>
        /// Waits for the other task's result in case if the first one didn't completed in <paramref name="timeoutInMilliseconds"/> milliseconds.
        /// </summary>
        /// <typeparam name="T">Type of the result.</typeparam>
        /// <param name="task">The task to initially wait for.</param>
        /// <param name="defaultValue">The default value which should be returned in case of elapsed <paramref name="timeoutInMilliseconds"/>.</param>
        /// <param name="timeoutInMilliseconds">Time to wait until <paramref name="task"/> completion.</param>
        /// <returns>Result of the either of tasks.</returns>
        public static async Task<T> ReturnInTimeoutAsync<T>(this Task<T> task, T defaultValue, int timeoutInMilliseconds = 200)
        {
            var timeoutTask = Task.Delay(timeoutInMilliseconds);

            await Task.WhenAny(task, timeoutTask);

            if (timeoutTask.IsCompleted)
            {
                return defaultValue;
            }

            return await task;
        }

        /// <summary>
        /// Waits for the other task's result in case if the first one didn't completed in <paramref name="timeout"/> milliseconds.
        /// </summary>
        /// <typeparam name="T">Type of the result.</typeparam>
        /// <param name="task">The task to initially wait for.</param>
        /// <param name="defaultValue">The default value which should be returned in case of elapsed <paramref name="timeout"/>.</param>
        /// <param name="timeout">Time to wait until <paramref name="task"/> completion.</param>
        /// <returns>Result of the either of tasks.</returns>
        public static async Task<T> ReturnInTimeoutAsync<T>(this Task<T> task, T defaultValue, TimeSpan timeout)
        {
            var timeoutTask = Task.Delay(timeout);

            await Task.WhenAny(task, timeoutTask);

            if (timeoutTask.IsCompleted)
            {
                return defaultValue;
            }

            return await task;
        }

        /// <summary>
        /// Waits for the other task in case if the first one didn't completed in <paramref name="timeoutInMilliseconds"/> milliseconds.
        /// </summary>
        /// <param name="task">The task to initially wait for.</param>
        /// <param name="otherTask">The task to wait in case of elapsed <paramref name="timeoutInMilliseconds"/>.</param>
        /// <param name="timeoutInMilliseconds">Time to wait until <paramref name="task"/> completion.</param>
        /// <returns>The task to await on.</returns>
        public static async Task RunOtherInTimeoutAsync(this Task task, Func<Task> otherTask, int timeoutInMilliseconds = 200)
        {
            var timeoutTask = Task.Delay(timeoutInMilliseconds);

            await Task.WhenAny(task, timeoutTask);

            if (timeoutTask.IsCompleted)
            {
                await otherTask();
            }
        }

        /// <summary>
        /// Waits for the other task's result in case if the first one didn't completed in <paramref name="timeoutInMilliseconds"/> milliseconds.
        /// </summary>
        /// <typeparam name="T">Type of the result.</typeparam>
        /// <param name="task">The task to initially wait for.</param>
        /// <param name="otherTask">The task to wait in case of elapsed <paramref name="timeoutInMilliseconds"/>.</param>
        /// <param name="timeoutInMilliseconds">Time to wait until <paramref name="task"/> completion.</param>
        /// <returns>Result of the either of tasks.</returns>
        public static async Task<T> RunOtherInTimeoutAsync<T>(this Task<T> task, Func<Task<T>> otherTask, int timeoutInMilliseconds = 200)
        {
            var timeoutTask = Task.Delay(timeoutInMilliseconds);

            await Task.WhenAny(task, timeoutTask);

            if (timeoutTask.IsCompleted)
            {
                return await otherTask();
            }

            return await task;
        }
    }
}
