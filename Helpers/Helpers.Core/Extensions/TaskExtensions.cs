namespace Helpers.Core.Extensions;

public static class TaskExtensions
{
    /// <summary>
    ///     Synonim for Task.WhenAll(...) call
    /// </summary>
    public static Task<TResult[]> WhenAll<TResult>(this IEnumerable<Task<TResult>> tasks)
    {
        return Task.WhenAll(tasks);
    }

    /// <summary>
    ///     Synonim for Task.WhenAll(...) call
    /// </summary>
    public static Task WhenAll(this IEnumerable<Task> tasks)
    {
        return Task.WhenAll(tasks);
    }

    /// <summary>
    ///     When task ends with exception (IsFailed == true), returns null as a result
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="task">The task.</param>
    /// <param name="requestErrorCondition">The request error condition.</param>
    /// <returns>Null fallback continuation</returns>
    public static Task<TResult> WithNullFallback<TResult>(this Task<TResult> task,
        Predicate<Exception> requestErrorCondition = null) where TResult : class
    {
        return task.WithFallback((TResult)null, requestErrorCondition);
    }

    /// <summary>
    ///     When task ends with exception (IsFailed == true), returns default instance as a result (0 for integers, new List
    ///     &lt;T&gt;() for List&lt;T&gt; and etc)
    /// </summary>
    public static Task<TResult> WithDefaultFallback<TResult>(this Task<TResult> task,
        Predicate<Exception> requestErrorCondition = null)
        where TResult : new()
    {
        var defaultValue = Activator.CreateInstance<TResult>();
        return task.WithFallback(defaultValue, requestErrorCondition);
    }

    /// <summary>
    ///     When task ends with exception (IsFailed == true), returns provided default value generator call as a result
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="task">The task.</param>
    /// <param name="fallbackFunc">The fallback function.</param>
    /// <param name="requestErrorCondition">The request error condition.</param>
    /// <returns></returns>
    public static Task<TResult> WithFallback<TResult>(this Task<TResult> task, Func<Exception, TResult> fallbackFunc,
        Predicate<Exception> requestErrorCondition = null)
    {
        return task.WithFallback(ex => Task.FromResult(fallbackFunc(ex)), requestErrorCondition);
    }

    /// <summary>
    ///     When task ends with exception (IsFailed == true), returns provided default value generator call as a result
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="task">The task.</param>
    /// <param name="fallbackFunc">The fallback function.</param>
    /// <param name="requestErrorCondition">The request error condition.</param>
    /// <returns></returns>
    public static Task<TResult> WithFallback<TResult>(this Task<TResult> task, Func<Task<TResult>> fallbackFunc,
        Predicate<Exception> requestErrorCondition = null)
    {
        return task.WithFallback(ex => fallbackFunc(), requestErrorCondition);
    }

    /// <summary>
    ///     When task ends with exception (IsFailed == true), returns provided default value generator call as a result
    /// </summary>
    public static Task<TResult> WithFallback<TResult>(this Task<TResult> task,
        Predicate<Exception> requestErrorCondition, Func<Task<TResult>> fallbackFunc)
    {
        return task.WithFallback(ex => fallbackFunc(), requestErrorCondition);
    }

    /// <summary>
    ///     When task ends with exception (IsFailed == true), returns provided default value generator call as a result
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="task">The task.</param>
    /// <param name="fallbackFunc">The fallback function.</param>
    /// <param name="requestErrorCondition">The request error condition.</param>
    /// <returns></returns>
    public static Task<TResult> WithFallback<TResult>(this Task<TResult> task,
        Func<Exception, Task<TResult>> fallbackFunc, Predicate<Exception> requestErrorCondition = null)
    {
        return task.ContinueWith(t =>
        {
            if (t.IsFaulted || t.IsCanceled)
            {
                var error = t.IsCanceled ? new OperationCanceledException() : t.Exception.InnerException;
                while (error is AggregateException)
                    error = error.InnerException;
                if (requestErrorCondition == null || requestErrorCondition.Invoke(error))
                    return fallbackFunc(error);

                throw error;
            }

            return Task.FromResult(t.Result);
        }).Unwrap();
    }

    /// <summary>
    ///     Executes provided handler on error occured
    /// </summary>
    public static Task<TResult> OnError<TResult>(this Task<TResult> task, Predicate<Exception> requestErrorCondition,
        Func<Exception, Task> errorHandlerFunc)
    {
        return task.ContinueWith(async t =>
        {
            if (t.IsFaulted || t.IsCanceled)
            {
                var error = t.IsCanceled ? new OperationCanceledException() : t.Exception.InnerException;

                while (error is AggregateException)
                    error = error.InnerException;

                if (requestErrorCondition == null || requestErrorCondition.Invoke(error)) await errorHandlerFunc(error);

                throw error;
            }

            return t.Result;
        }).Unwrap();
    }


    /// <summary>
    ///     When task ends with exception (IsFailed == true), returns provided default value as a result
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="task">The task.</param>
    /// <param name="fallback">The fallback.</param>
    /// <param name="requestErrorCondition">The request error condition.</param>
    /// <returns>
    ///     Task with fallback value in case of faiture
    /// </returns>
    public static Task<TResult> WithFallback<TResult>(this Task<TResult> task, TResult fallback,
        Predicate<Exception> requestErrorCondition = null)
    {
        return task.WithFallback(ex => fallback, requestErrorCondition);
    }

    /// <summary>
    ///     When task ends with exception (IsFailed == true), finishes task silently
    /// </summary>
    public static Task WithSilentFallback(this Task task, Predicate<Exception> requestErrorCondition = null,
        Action<Exception> action = null)
    {
        return task.AsTaskWithResult().WithFallback(ex =>
        {
            action?.Invoke(ex);
            return Task.CompletedTask;
        }, requestErrorCondition);
    }

    /// <summary>
    ///     When task ends with exception (IsFailed == true), finishes task silently
    /// </summary>
    public static Task WithSilentFallback(this Task task, Predicate<Exception> requestErrorCondition,
        Func<Exception, Task> action)
    {
        return task.AsTaskWithResult().WithFallback(async ex =>
        {
            if (action != null)
                await action(ex);

            return null;
        }, requestErrorCondition);
    }

    /// <summary>
    ///     Applies provided selector to task result. If task failes, exception will be thrown
    /// </summary>
    /// <typeparam name="TTaskResult">The type of the task result.</typeparam>
    /// <typeparam name="TReturnResult">The type of the return result.</typeparam>
    /// <param name="task">The task.</param>
    /// <param name="selector">The selector.</param>
    /// <returns>Task with selected result</returns>
    public static Task<TReturnResult> WithResult<TTaskResult, TReturnResult>(this Task<TTaskResult> task,
        Func<TTaskResult, TReturnResult> selector)
    {
        return task.ContinueWith(t => selector(t.Result));
    }


    /// <summary>
    ///     Applies provided async selector to task result. If task failes, exception will be thrown
    /// </summary>
    /// <typeparam name="TTaskResult">The type of the task result.</typeparam>
    /// <typeparam name="TReturnResult">The type of the return result.</typeparam>
    /// <param name="task">The task.</param>
    /// <param name="selector">The asyncronious selector.</param>
    /// <returns>Task with selected result</returns>
    public static Task<TReturnResult> WithResult<TTaskResult, TReturnResult>(this Task<TTaskResult> task,
        Func<TTaskResult, Task<TReturnResult>> selector)
    {
        return task.ContinueWith(async t => await selector(t.Result)).Unwrap();
    }

    /// <summary>
    ///     Performs provided action  provided selector to task result. If task failes, exception will be thrown
    /// </summary>
    public static Task<TTaskResult> WithResult<TTaskResult>(this Task<TTaskResult> task, Action<TTaskResult> action)
    {
        return task.ContinueWith(t =>
        {
            action(t.Result);
            return t.Result;
        });
    }

    /// <summary>
    ///     Represents parameterless task as task with result (result will be new object())
    /// </summary>
    public static Task<object> AsTaskWithResult(this Task task)
    {
        return Task.FromResult(new object()).ContinueWith(async t =>
        {
            await task;
            return t.Result;
        }).Unwrap();
    }
}