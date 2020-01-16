//#define CATALOQ_INTERNAL // Uncomment or define at build time to set accessibility to internal.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>Represents a service host.</summary>
#if !CATALOQ_INTERNAL
public
#endif
abstract class ServiceHost
{
    internal ServiceHost(IEnumerable<object> services, Type[] allowed)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (allowed == null) throw new ArgumentNullException(nameof(allowed));

        Services = services;
        Allowed = allowed;
    }

    /// <summary>Initializes a new <c>ServiceHost</c> instance, optionally defining allowed base
    /// types.</summary>
    public ServiceHost(params Type[] allowed) : this(Enumerable.Empty<object>(), allowed)
    {
    }

    /// <summary>Builds a new default service host instance with allowed types.</summary>
    public static ServiceHost DefaultHost(params Type[] allowed)
    {
        return new DefaultServiceHost(allowed);
    }

    /// <summary>Builds a new thread-safe service host instance with allowed types.</summary>
    public static ServiceHost BlockingHost(params Type[] allowed)
    {
        return new BlockingServiceHost(allowed);
    }

    /// <summary>The sequence of managed services instances.</summary>
    public virtual IEnumerable<object> Services { get; internal set; }

    /// <summary>Types and/or base types allowed as services.</summary>
    public virtual Type[] Allowed { get; internal set; }

    /// <summary>Returns a new <c>ServiceHost</c> instance with the given service added.</summary>
    public abstract ServiceHost Add<T>(T newService);

    /// <summary>Returns a new <c>ServiceHost</c> instance with the given service added and initialized
    /// by a <c>factory</c> function. Creation is delegated to allow type checking. The given type can
    /// be a a base type of the instance created by the factory function.</summary>
    public abstract ServiceHost Add(Type type, Func<object> factory);

    /// <summary>Returns a new <c>ServiceHost</c> instance with the given service added and initialized
    /// by a <c>factory</c> function. Creation is delegated to allow type checking.</summary>
    public abstract ServiceHost Add<T>(Func<T> factory);

    /// <summary>Returns a new <c>ServiceHost</c> instance with given the service removed.</summary>
    public abstract ServiceHost Remove(Type type);

    /// <summary>Returns a new <c>ServiceHost</c> instance with the given service removed.</summary>
    public abstract ServiceHost Remove<T>();

    /// <summary>Returns a new <c>ServiceHost</c> instance with all services the inherit from a
    /// given type removed.</summary>
    public abstract ServiceHost RemoveAll<T>();

    /// <summary>Returns a new <c>ServiceHost</c> instance with mapping a given function on each
    /// service.</summary>
    public abstract ServiceHost Map(Func<object, object> func);

    /// <summary>Clones a <c>ServiceHost</c> instance.</summary>
    public abstract ServiceHost Clone();

    // Validates a service against allowed types.
    protected static bool Validate(Type[] allowed, Type type)
    {
        if (allowed.Length > 0) {
            foreach (var element in allowed) {
                if (type.Equals(element) ||
                    type.IsSubclassOf(element)) {
                    return true;
                }
            }
            return false;
        }
        return true;
    }

    // Formats a meaningful exception message for validation fail.
    protected static string ExceptionMessage(Type[] allowed, bool exactly = false)
    {
        var builder = new StringBuilder(29 + allowed.Length * 10);
        builder.Append("Type must be ");
        if (!exactly) {
            builder.Append("or inherit from ");
        }
        for (var i = 0; i < allowed.Length; i++) {
            builder.Append(allowed[i].Name);
            if (i == allowed.Length - 2) {
                builder.Append(" or ");  
            }
            else if (i != allowed.Length - 1) {
                builder.Append(", ");
            }
        }
        return builder.ToString();
    }
}

/// <summary>Provides default service host implementation.</summary>
#if !CATALOQ_INTERNAL
public
#endif
class DefaultServiceHost : ServiceHost
{
    internal DefaultServiceHost(IEnumerable<object> services, Type[] allowed) : base(services, allowed)
    {
    }

    public DefaultServiceHost(params Type[] allowed) : base(Enumerable.Empty<object>(), allowed) 
    {
    }

    public override ServiceHost Add<T>(T newService)
    {
        if (newService == null) throw new ArgumentNullException(nameof(newService));
        if (!Validate(Allowed, newService.GetType())) throw new NotSupportedException(ExceptionMessage(Allowed));

        return new DefaultServiceHost(impl(), Allowed);
        IEnumerable<object> impl() {
            foreach (var service in Services) {
                yield return service;
            }
            yield return newService;
        }
    }

    public override ServiceHost Add(Type type, Func<object> factory)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (factory == null) throw new ArgumentNullException(nameof(factory));
        if (!Validate(Allowed, type)) throw new NotSupportedException(ExceptionMessage(Allowed)); 
        var service = factory();
        if (!Validate(Allowed, service.GetType()))
            throw new NotSupportedException(ExceptionMessage(Allowed));
        if (!service.GetType().Equals(type) && !service.GetType().IsSubclassOf(type))
            throw new ArgumentException("Type mismatch", nameof(type));

        return new DefaultServiceHost(Add(factory()).Services, Allowed);
    }

    public override ServiceHost Add<T>(Func<T> factory)
    {
        if (factory == null) throw new ArgumentNullException(nameof(factory));
        if (!Validate(Allowed, typeof(T))) throw new NotSupportedException(ExceptionMessage(Allowed));

        return new DefaultServiceHost(Add(factory()).Services, Allowed);
    }

    public override ServiceHost Remove(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        if (!Validate(Allowed, type))
            throw new NotSupportedException(ExceptionMessage(Allowed));

        return new DefaultServiceHost(impl(), Allowed);
        IEnumerable<object> impl() {
            bool removed = false;
            foreach (var element in Services) {
                if (!element.GetType().Equals(type)) {
                    yield return element;
                }
                else {
                    if (!removed) {
                        removed = true;
                    }
                    else {
                        yield return element;
                    }
                }
            }
        }
    }

    public override ServiceHost Remove<T>()
    {
        return Remove(typeof(T));
    }


    public override ServiceHost RemoveAll<T>()
    {
        if (!Validate(Allowed, typeof(T))) throw new NotSupportedException(ExceptionMessage(Allowed));

        return new DefaultServiceHost(impl(), Allowed);
        IEnumerable<object> impl() {
            foreach (var element in Services) {
                if (!element.GetType().IsSubclassOf(typeof(T))) {
                    yield return element;
                }
            }
        }
    }

    public override ServiceHost Map(Func<object, object> func)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));

        return new DefaultServiceHost(impl(), Allowed);
        IEnumerable<object> impl() {
            foreach (var element in Services) {
                yield return func(element);
            }
        }
    }

    public override ServiceHost Clone()
    {
        return new DefaultServiceHost(Services, Allowed);
    }
}

/// <summary>Provides thread-safe service host implementation.</summary>
#if !CATALOQ_INTERNAL
public
#endif
class BlockingServiceHost : DefaultServiceHost
{
    internal BlockingServiceHost(IEnumerable<object> services, Type[] allowed) : base(services, allowed)
    {
    }

    public BlockingServiceHost(params Type[] allowed) : base(allowed)
    {
    }

    public override IEnumerable<object> Services
    {
        get { lock(this) { return base.Services;  } }
        internal set { lock(this) { base.Services = value; } }
    }

    public override Type[] Allowed
    {
        get { lock(this) { return base.Allowed;  } }
        internal set { lock(this) { base.Allowed = value; } }
    }

    public override ServiceHost Add<T>(T newService)
    {
        lock (this) { return base.Add(newService); }
    }

    public override ServiceHost Add(Type type, Func<object> factory)
    {
        lock (this) { return base.Add(type, factory); }
    }

    public override ServiceHost Add<T>(Func<T> factory)
    {
        lock (this) { return base.Add(factory); }
    }

    public override ServiceHost Remove(Type type)
    {
        lock (this) { return base.Remove(type); }
    }

    public override ServiceHost Remove<T>()
    {
        lock (this) { return base.Remove<T>(); }
    }

    public override ServiceHost RemoveAll<T>()
    {
        lock (this) { return base.RemoveAll<T>(); }
    }

    public override ServiceHost Map(Func<object, object> func)
    {
        lock (this) { return base.Map(func); }
    }

    public override ServiceHost Clone()
    {
        lock (this) { return base.Clone(); }
    }
}