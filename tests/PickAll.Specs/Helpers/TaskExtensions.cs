using System.Runtime.CompilerServices;
using System.Threading.Tasks;

static class TaskExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T RunSynchronously<T>(this Task<T> task)
    {
        return task.GetAwaiter().GetResult();
    }
}
