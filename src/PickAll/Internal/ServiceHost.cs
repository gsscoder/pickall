//#define CATALOQ_INTERNAL // Uncomment or define at build time to set accessibility to internal.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>Models a service host.</summary>
#if !CATALOQ_INTERNAL
public
#endif
class ServiceHost
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

    /// <summary>The sequence of managed services instances.</summary>
    public IEnumerable<object> Services
    {
        get;
        private set;
    }

#if !DEBUG
    Type[] Allowed { get; set; }
#else
    public Type[] Allowed { get; set; }
#endif 

    /// <summary>Returns a new <c>ServiceHost</c> instance with the given service added.</summary>
    public ServiceHost Add<T>(T newService)
    {
        if (newService == null) throw new ArgumentNullException(nameof(newService));
        if (!Validate(Allowed, newService.GetType())) throw new NotSupportedException(ExceptionMessage(Allowed));

        return new ServiceHost(impl(), Allowed);
        IEnumerable<object> impl() {
            foreach (var service in Services) {
                yield return service;
            }
            yield return newService;
        }
    }

    /// <summary>Returns a new <c>ServiceHost</c> instance with the given service added and initialized
    /// by a <c>factory</c> function. Creation is delegated to allow type checking. The given type can
    /// be a a base type of the instance created by the factory function.</summary>
    public ServiceHost Add(Type type, Func<object> factory)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (factory == null) throw new ArgumentNullException(nameof(factory));
        if (!Validate(Allowed, type)) throw new NotSupportedException(ExceptionMessage(Allowed)); 
        var service = factory();
        if (!Validate(Allowed, service.GetType()))
            throw new NotSupportedException(ExceptionMessage(Allowed));
        if (!service.GetType().Equals(type) && !service.GetType().IsSubclassOf(type))
            throw new ArgumentException("Type mismatch", nameof(type));

        return new ServiceHost(Add(factory()).Services, Allowed);
    }

    /// <summary>Returns a new <c>ServiceHost</c> instance with the given service added and initialized
    /// by a <c>factory</c> function. Creation is delegated to allow type checking.</summary>
    public ServiceHost Add<T>(Func<T> factory)
    {
        if (factory == null) throw new ArgumentNullException(nameof(factory));
        if (!Validate(Allowed, typeof(T))) throw new NotSupportedException(ExceptionMessage(Allowed));

        return new ServiceHost(Add(factory()).Services, Allowed);
    }

    /// <summary>Returns a new <c>ServiceHost</c> instance with given the service removed.</summary>
    public ServiceHost Remove(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        if (!Validate(Allowed, type))
            throw new NotSupportedException(ExceptionMessage(Allowed));

        return new ServiceHost(impl(), Allowed);
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

    /// <summary>Returns a new <c>ServiceHost</c> instance with the given service removed.</summary>
    public ServiceHost Remove<T>()
    {
        return Remove(typeof(T));
    }

    /// <summary>Returns a new <c>ServiceHost</c> instance with all services the inherit from a
    /// given type removed.</summary>
    public ServiceHost RemoveAll<T>()
    {
        if (!Validate(Allowed, typeof(T))) throw new NotSupportedException(ExceptionMessage(Allowed));

        return new ServiceHost(impl(), Allowed);
        IEnumerable<object> impl() {
            foreach (var element in Services) {
                if (!element.GetType().IsSubclassOf(typeof(T))) {
                    yield return element;
                }
            }
        }
    }

    /// <summary>Returns a new <c>ServiceHost</c> instance with mapping a given function on each
    /// service.</summary>
    public ServiceHost Map(Func<object, object> func)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));

        return new ServiceHost(impl(), Allowed);
        IEnumerable<object> impl() {
            foreach (var element in Services) {
                yield return func(element);
            }
        }
    }

    /// <summary>Clones a <c>ServiceHost</c> instance.</summary>
    public ServiceHost Clone()
    {
        return new ServiceHost(Services, Allowed);
    }
    
    // Validates a service against allowed types.
    static bool Validate(Type[] allowed, Type type)
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
    static string ExceptionMessage(Type[] allowed, bool exactly = false)
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