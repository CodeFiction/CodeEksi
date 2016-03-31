using System.Diagnostics;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static Task RunWithErrorHandling(Func<Task> function)
        {
            return Task.Run(function).ContinueWith(t =>
            {
                if (t.IsCanceled || !t.IsFaulted || t.Exception == null) return;
                var innerException = t.Exception.Flatten().InnerExceptions.FirstOrDefault();
                Trace.WriteLine((innerException ?? t.Exception).ToString());
            });
        }
    }
}
