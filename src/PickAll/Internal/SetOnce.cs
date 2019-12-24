//Use project level define(s) when referencing with Paket.
//#define CSX_SETONCE_INTERNAL // Uncomment this to set SetOnce<T> accessibility to internal.
#define CSX_SETONCE_ONLY_UNSAFE // Un comment to remove thread-safe implementation.

using System;

namespace CSharpx
{
    /// <summary>
    /// Wraps a value that can be set only once.
    /// </summary>
    #if !CSX_SETONCE_INTERNAL
    public
    #endif
    struct SetOnce<T>
    {
        private bool _set;
        private T _value;

        /// <summary>
        /// Inner wrapped value.
        /// </summary>
        public T Value
        {
            get
            {
                if (_set) {
                    return _value;
                }
                else {
                    throw new InvalidOperationException($"Value not set");
                }
            }
            set
            {
                if (_set) {
                    throw new InvalidOperationException($"Value can be set only once");
                }
                else {
                    _value = value;
                    _set = true;
                }
            }
        }

        public static implicit operator T(SetOnce<T> instance) => instance.Value;
    }

#if !CSX_SETONCE_ONLY_UNSAFE
    /// <summary>
    /// Wraps a value that can be set only once. Thread-safe implementation.
    /// </summary>
    #if !CSX_SETONCE_INTERNAL
    public
    #endif
    class SafeSetOnce<T>
    {
        private readonly object _syncRoot = new object();
        private bool _set;
        private T _value;

        /// <summary>
        /// Inner wrapped value.
        /// </summary>
        public T Value
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_set) {
                        return _value;
                    }
                    else {
                        throw new InvalidOperationException($"Value not set");
                    }
                } 
            }
            set
            {
                lock (_syncRoot)
                {
                    if (_set) {
                        throw new InvalidOperationException($"Value can be set only once");
                    }
                    else {
                        _value = value;
                        _set = true;
                    }
                }
            }
        }

        public static implicit operator T(SafeSetOnce<T> instance) => instance.Value;
    }
#endif
}