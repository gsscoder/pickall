using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

sealed class ServiceHost
{
    internal ServiceHost(IEnumerable<object> services, Type[] allowed)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (allowed == null) throw new ArgumentNullException(nameof(allowed));

        Services = services;
        Allowed = allowed;
    }

    public ServiceHost(params Type[] allowed)
        : this(Enumerable.Empty<object>(), allowed) { }

    public IEnumerable<object> Services { get; internal set; }

    public Type[] Allowed { get; internal set; }

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

    public ServiceHost Add(Type type, Func<object> factory)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (factory == null) throw new ArgumentNullException(nameof(factory));
        if (!Validate(Allowed, type)) throw new NotSupportedException(ExceptionMessage(Allowed)); 
        var service = factory();
        if (!Validate(Allowed, service.GetType())) throw new NotSupportedException(ExceptionMessage(Allowed));
        if (!service.GetType().Equals(type) && !service.GetType().IsSubclassOf(type)) throw new ArgumentException("Type mismatch", nameof(type));

        return new ServiceHost(Add(factory()).Services, Allowed);
    }

    public ServiceHost Add<T>(Func<T> factory)
    {
        if (factory == null) throw new ArgumentNullException(nameof(factory));
        if (!Validate(Allowed, typeof(T))) throw new NotSupportedException(ExceptionMessage(Allowed));

        return new ServiceHost(Add(factory()).Services, Allowed);
    }

    public ServiceHost Remove(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (!Validate(Allowed, type)) throw new NotSupportedException(ExceptionMessage(Allowed));

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

    public ServiceHost Remove<T>()
    {
        return Remove(typeof(T));
    }

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

    public ServiceHost Map<T>(Func<T, T> func)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));

        return new ServiceHost(impl(), Allowed);
        IEnumerable<object> impl() {
            foreach (var element in Services) {
                if (element.GetType().Equals(typeof(T)) ||
                    element.GetType().IsSubclassOf(typeof(T))) {
                        yield return func((T)element);
                    }
                else {
                    yield return element;
                }
            }
        }
    }

    public ServiceHost Map(Func<object, object> func, Func<object, bool> predicate)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return new ServiceHost(impl(), Allowed);
        IEnumerable<object> impl() {
            foreach (var element in Services) {
                if (predicate(element)) {
                    yield return func(element);
                } else {
                    yield return element;
                }
            }
        }
    }

    public ServiceHost Map<T>(Func<T, T> func, Func<T, bool> predicate)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return new ServiceHost(impl(), Allowed);
        IEnumerable<object> impl() {
            foreach (var element in Services) {
                if ((element.GetType().Equals(typeof(T)) ||
                     element.GetType().IsSubclassOf(typeof(T))) &&
                     predicate((T)element)) {
                    yield return func((T)element);
                } else {
                    yield return element;
                }
            }
        }
    }

    public ServiceHost Configure<T>(Action<T> action)
    {
        return Map<T>(service => {
            action(service);
            return service; });
    }

    public ServiceHost Configure<T>(Action<T> action, Func<T, bool> predicate)
    {
        return Map<T>(service => {
            action(service);
            return service; },
            predicate);
    }

    public ServiceHost Clone()
    {
        return new ServiceHost(Services, Allowed);
    }

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