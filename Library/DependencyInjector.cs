using System.Reflection;

namespace Library;

[AttributeUsage(AttributeTargets.Parameter)]
public class UniqueAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Parameter)]
public class SharedAttribute : Attribute
{
    public string Token { get; set; }
    public SharedAttribute(string token)
    {
        Token = token;
    }
}
public class DependencyInjector
{
    private enum ServiceScope
    {
        Shared,
        Unique,
    }
    private record ServiceInfo(Type Type, ServiceScope Scope)
    {
        public bool IsShared => Scope == ServiceScope.Shared;
        public bool IsUnique => Scope == ServiceScope.Unique;
    }
    private record ServiceKey(Type IType, Type? Type, string? Token)
    {
        public override int GetHashCode() => IType.GetHashCode() + Type?.GetHashCode() ?? 0 + Token?.GetHashCode() ?? 0;
    }
    private HashSet<Type> _underConstruction = new();
    private Dictionary<ServiceKey, List<ServiceInfo>> _typesMap = new();
    private Dictionary<ServiceKey, object> _services = new();

    public void AddUnique<T>() where T : class => AddUnique<T, T>();
    public void AddUnique<I, T>() where T : I where I : class => Add<I, T>(ServiceScope.Unique);
    public void AddShared<T>(string? token = null) where T : class => AddShared<T, T>(token);
    public void AddShared<I, T>(string? token = null) where T : I where I : class => Add<I, T>(ServiceScope.Shared, token);
    private void Add<I, T>(ServiceScope scope, string? token = null) where T : I where I : class
    {
        var serviceKey = new ServiceKey(typeof(I), null, token);
        if (_typesMap.ContainsKey(serviceKey))
        {
            _typesMap[serviceKey].Add(new ServiceInfo(typeof(T), scope));
        }
        else
        {
            _typesMap[serviceKey] = new List<ServiceInfo> { new ServiceInfo(typeof(T), scope) };
        }
    }
    public T? GetUnique<T>() where T : class => GetServices<T>(ServiceScope.Unique).FirstOrDefault();
    public T[] GetUniques<T>() where T : class => GetServices<T>(ServiceScope.Unique);
    public T? GetShared<T>(string? token = null) where T : class => GetServices<T>(ServiceScope.Shared, token).FirstOrDefault();
    public T[] GetShares<T>(string? token = null) where T : class => GetServices<T>(ServiceScope.Shared, token);

    private T[] GetServices<T>(ServiceScope? scope = null, string? token = null) where T : class
    {
        return GetServices(typeof(T), ServiceScope.Shared, token) as T[] ?? new T[0];
    }
    private object[] GetServices(Type interfaceType, ServiceScope? scope = null, string? token = null)
    {
        var result = new List<object>();
        var serviceKey = new ServiceKey(interfaceType, null, token);
        if (!_typesMap.TryGetValue(serviceKey, out var infos))
        {
            return new object[0];
        }
        foreach (var info in infos)
        {
            var service = GetService(serviceKey with { Type = info.Type }, info, scope);
            if (service is not null)
            {
                result.Add(service);
            }
        }

        return result.ToArray();
    }

    private object? GetService(ServiceKey serviceKey, ServiceInfo serviceInfo, ServiceScope? scope = null)
    {
        scope ??= serviceInfo.Scope;
        if (serviceInfo.IsShared && scope == ServiceScope.Shared)
        {
            return GetSharedService(serviceKey, serviceInfo);
        }
        else if (scope == ServiceScope.Unique)
        {
            return CreateService(serviceKey, serviceInfo);
        }
        return null;
    }
    private object GetSharedService(ServiceKey serviceKey, ServiceInfo serviceInfo)
    {
        if (_services.TryGetValue(serviceKey, out var obj))
        {
            return obj;
        }
        return CreateAndRegisterService(serviceKey, serviceInfo);
    }
    private object CreateAndRegisterService(ServiceKey serviceKey, ServiceInfo serviceInfo)
    {
        var obj = CreateService(serviceKey, serviceInfo);
        _services[serviceKey] = obj;
        return obj;
    }
    private object CreateService(ServiceKey serviceKey, ServiceInfo serviceInfo)
    {
        var interfaceType = serviceKey.IType;
        if (_underConstruction.Contains(interfaceType))
        {
            throw new InvalidOperationException($"Circular dependency detected in {interfaceType.FullName} constructor");
        }
        _underConstruction.Add(interfaceType);

        var constructor = GetServiceConstructor(serviceKey, serviceInfo);
        List<object> parameters = new();
        foreach (var parameterInfo in constructor.GetParameters())
        {
            var service = GetParameterService(parameterInfo);
            if (service is null)
            {
                throw new InvalidOperationException($"service {parameterInfo.ParameterType.FullName} was not found, check if Scope Attributes match registered service type");
            }
            parameters.Add(service);
        }
        _underConstruction.Remove(interfaceType);
        return constructor.Invoke(parameters.ToArray());
    }
    private ConstructorInfo GetServiceConstructor(ServiceKey serviceKey, ServiceInfo serviceInfo)
    {
        var constructors = serviceInfo.Type.GetConstructors();
        if (constructors.Length != 1)
        {
            throw new InvalidOperationException($"Class {serviceInfo.Type.FullName} should have exactly one constructor");
        }
        return constructors.First();
    }
    private object? GetParameterService(ParameterInfo parameterInfo)
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
            token = sharedAttr.Token;
        }
        var paramType = parameterInfo.ParameterType;
        if (paramType.IsArray)
        {
            paramType = paramType.GetElementType();
            return GetServices(paramType ?? typeof(object), serviceScope, token);
        }
        return GetServices(paramType, serviceScope, token)?.FirstOrDefault();
    }
}