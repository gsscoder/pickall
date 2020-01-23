using System.Threading.Tasks;

static class TaskExtensions
{
    public static T RunSynchronously<T>(this Task<T> task)
    {
        return task.GetAwaiter().GetResult();
    }
}