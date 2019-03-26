using System;
using System.Threading.Tasks;
using Gtk;
using Action = System.Action;

namespace MarcusW.SharpUtils.Gtk
{
    public static class Async
    {
        /// <summary>
        /// Executes the <paramref name="action"/> in the Gtk GUI-Thread and waits for it's completion.
        /// </summary>
        /// <param name="action">The action to execute in the GUI-Thread</param>
        public static Task RunInGtkThreadAsync(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var tcs = new TaskCompletionSource<object>();
            Application.Invoke(delegate {
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                    return;
                }

                tcs.SetResult(null);
            });
            return tcs.Task;
        }

        /// <summary>
        /// Executes the <paramref name="action"/> in the Gtk GUI-Thread and waits for it's completion.
        /// </summary>
        /// <param name="action">The action to execute in the GUI-Thread</param>
        public static Task<TResult> RunInGtkThreadAsync<TResult>(Func<TResult> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var tcs = new TaskCompletionSource<TResult>();
            Application.Invoke(delegate {
                TResult result;
                try
                {
                    result = action.Invoke();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                    return;
                }

                tcs.SetResult(result);
            });
            return tcs.Task;
        }

        /// <summary>
        /// Executes the <paramref name="asyncAction"/> in the Gtk GUI-Thread and waits for it's completion.
        /// </summary>
        /// <param name="asyncAction">The action to execute in the GUI-Thread</param>
        public static Task RunInGtkThreadAsync(Func<Task> asyncAction)
        {
            if (asyncAction == null)
                throw new ArgumentNullException(nameof(asyncAction));

            var tcs = new TaskCompletionSource<object>();
            Application.Invoke(async delegate {
                try
                {
                    await asyncAction.Invoke().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                    return;
                }

                tcs.SetResult(null);
            });
            return tcs.Task;
        }

        /// <summary>
        /// Executes the <paramref name="asyncAction"/> in the Gtk GUI-Thread and waits for it's completion.
        /// </summary>
        /// <param name="asyncAction">The action to execute in the GUI-Thread</param>
        public static Task<TResult> RunInGtkThreadAsync<TResult>(Func<Task<TResult>> asyncAction)
        {
            if (asyncAction == null)
                throw new ArgumentNullException(nameof(asyncAction));

            var tcs = new TaskCompletionSource<TResult>();
            Application.Invoke(async delegate {
                TResult result;
                try
                {
                    result = await asyncAction.Invoke().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                    return;
                }

                tcs.SetResult(result);
            });
            return tcs.Task;
        }
    }
}
