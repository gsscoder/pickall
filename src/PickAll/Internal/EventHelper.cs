using System;
using System.Runtime.CompilerServices;

static class EventHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RaiseEvent(object sender, EventHandler handler, EventArgs args, bool enabled)
    {
        if (enabled && handler != null) {
            handler(sender, args);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RaiseEvent<T>(object sender, EventHandler<T> handler, Func<T> args, bool enabled)
        where T : EventArgs
    {
        if (enabled && handler != null) {
            handler(sender, args());
        }
    }
}
