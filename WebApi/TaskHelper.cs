using System.Threading.Tasks;

namespace WebApi
{
    public static class TaskHelper
    {
        public static void FireAndForget(this Task task)
        {
            Task.Run(async () => await task).ConfigureAwait(false);
        }
    }
}