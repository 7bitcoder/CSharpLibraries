using System.Reflection;

namespace CSharpLibraries.DependencyInjector
{
    [System.AttributeUsage(System.AttributeTargets.Parameter)]
    public class UniqueAttribute : System.Attribute { }

    [System.AttributeUsage(System.AttributeTargets.Parameter)]
    public class SharedAttribute : System.Attribute
    {
        public string token { get; set; }
        public SharedAttribute(string token)
        {
            this.token = token;
        }
    }
    public class DependencyInjector
    {
        private enum ServiceScope
        {
            Shared,
            Unique,
        }
        private class ServiceInfo
        {
            public Type? type { get; init; }
            public ServiceScope scope { get; init; }
            public bool isShared => scope == ServiceScope.Shared;
            public bool isUnique => scope == ServiceScope.Unique;
        }
        private class ServiceKey
        {
            public Type? type { get; set; }
            public string? token { get; set; } = null;

            public override int GetHashCode() => type.GetHashCode() + token?.GetHashCode() ?? 0;
            public override bool Equals(object? obj) => Equals(obj as ServiceKey);
            public bool Equals(ServiceKey? obj) => obj != null && obj.type == this.type && token == this.token;
        }
        private HashSet<Type> underConstruction = new();
        private Dictionary<Type, ServiceInfo> _typesMap = new();
        private Dictionary<ServiceKey, object> _services = new();

        public void AddUnique<T>() where T : class => AddUnique<T, T>();
        public void AddUnique<I, T>() where T : I where I : class => add<I, T>(ServiceScope.Unique);
        public void AddShared<T>() where T : class => AddShared<T, T>();
        public void AddShared<I, T>() where T : I where I : class => add<I, T>(ServiceScope.Shared);
        private void add<I, T>(ServiceScope scope) where T : I where I : class
        {
            _typesMap[typeof(I)] = new ServiceInfo { type = typeof(T), scope = scope };
        }
        public T? getUnique<T>() where T : class => getService(typeof(T), ServiceScope.Unique) as T;
        public T? getShared<T>(string? token = null) where T : class => getService(typeof(T), ServiceScope.Shared, token) as T;
        private object? getService(Type interfaceType, ServiceScope? scope = null, string? token = null)
        {
            if (!_typesMap.TryGetValue(interfaceType, out var info))
            {
                return null;
            }
            scope ??= info.scope;
            if (info.isShared && scope == ServiceScope.Shared)
            {
                return getSharedService(interfaceType, token);
            }
            else if (scope == ServiceScope.Unique)
            {
                return createService(interfaceType);
            }
            return null;
        }
        private object getSharedService(Type interfaceType, string? token = null)
        {
            var key = new ServiceKey { type = interfaceType, token = token };
            if (_services.TryGetValue(key, out var obj))
            {
                return obj;
            }
            return createAndRegisterService(interfaceType, token);
        }
        private object createAndRegisterService(Type interfaceType, string? token = null)
        {
            var key = new ServiceKey { type = interfaceType, token = token };
            var obj = createService(interfaceType);
            _services[key] = obj;
            return obj;
        }
        private object createService(Type interfaceType)
        {
            if (underConstruction.Contains(interfaceType))
            {
                throw new InvalidOperationException($"Circular dependency detected in {interfaceType.FullName} constructor");
            }
            underConstruction.Add(interfaceType);

            var constructor = getServiceConstructor(interfaceType);
            List<object> parameters = new();
            foreach (var parameterInfo in constructor.GetParameters())
            {
                var service = getParameterService(parameterInfo);
                if (service is null)
                {
                    throw new InvalidOperationException($"service {parameterInfo.ParameterType.FullName} was not found, check if Scope Attributes match registered service type");
                }
                parameters.Add(service);
            }
            underConstruction.Remove(interfaceType);
            return constructor.Invoke(parameters.ToArray());
        }
        private ConstructorInfo getServiceConstructor(Type interfaceType)
        {
            var info = _typesMap[interfaceType];
            var constructors = info.type.GetConstructors();
            if (constructors.Length != 1)
            {
                throw new InvalidOperationException($"Class {info.type.FullName} should have exactly one constructor");
            }
            return constructors.First();
        }
        private object? getParameterService(ParameterInfo parameterInfo)
        {
            string? token = null;
            ServiceScope? serviceScope = null;
            if (parameterInfo.GetCustomAttributes(typeof(UniqueAttribute), false).Any())
            {
                serviceScope = ServiceScope.Unique;
            }
            var sharedAttr = parameterInfo.GetCustomAttributes(typeof(SharedAttribute), false).FirstOrDefault() as SharedAttribute;
            if (sharedAttr is not null)
            {
                serviceScope = ServiceScope.Shared;
                token = sharedAttr.token;
            }
            var paramType = parameterInfo.ParameterType;
            return getService(paramType, serviceScope, token);
        }
    }
}