using System;
using System.Runtime.CompilerServices;

static class Guard
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AgainstNull(string argumentName, object value)
    {
        if (value == null) throw new ArgumentNullException(argumentName,
            $"{argumentName} cannot be null");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AgainstEmptyWhiteSpace(string argumentName, string value)
    {
        if (value.Trim() == string.Empty) throw new ArgumentException(
            $"{argumentName} cannot be empty or contains only white spaces", argumentName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AgainstNegative(string argumentName, int value)
    {
        if (value < 0) throw new ArgumentException(argumentName,
            $"{argumentName} cannot be lesser than zero");
    }
}