using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Generator;

namespace WebApi
{
    public static class TaskHelper
    {
        public static void FireAndForget(this Task task)
        {
            Task.Run(async () => await task).ConfigureAwait(false);
        }

        public static int FireAndHandleLater(this Task task, 
            Action<Task> onDone = null, 
            Action<Task, AggregateException> onFault = null, 
            Action<Task> onCancel = null, 
            Action<Task> onOther = null)
        {            
            Task.Run(async () => await task).ContinueWith(executedTask =>
            {
                if (executedTask.IsCompleted)
                {
                    if (executedTask.IsCompleted && !executedTask.IsFaulted && !executedTask.IsCanceled)
                    {
                        onDone?.Invoke(task);
                        Console.WriteLine("Finished task, yay!");
                    }

                    if (executedTask.IsCompleted && executedTask.IsFaulted)
                    {
                        onFault?.Invoke(task, executedTask.Exception);
                        Console.WriteLine("Failed task, oh noes.");
                    }

                    if (executedTask.IsCompleted && executedTask.IsCanceled)
                    {
                        onCancel?.Invoke(task);
                        Console.WriteLine("Someone pushed stop...");
                    }
                }
                else
                {
                    onOther?.Invoke(task);
                }
            }).ConfigureAwait(false);
            return task.Id;
        }
    }
}