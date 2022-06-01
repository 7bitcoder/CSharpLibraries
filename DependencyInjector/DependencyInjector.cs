namespace CSharpLibraries.DependencyInjector
{
    public class DependencyInjector
    {
        private HashSet<Type> underConstruction = new();
        private Dictionary<Type, Type> _typesMap = new();
        private Dictionary<Type, object> _registeredObjects = new();

        public void AddSingeleton<T>()
            where T : class
        {
            _typesMap[typeof(T)] = typeof(T);
        }

        public void AddSingeleton<I, T>()
            where T : I
            where I : class
        {
            _typesMap[typeof(I)] = typeof(T);
        }

        public T? get<T>() where T : class
        {
            return get(typeof(T)) as T;
        }

        public void build()
        {
            foreach (var interfaceType in _typesMap.Keys)
            {
                registerObject(interfaceType);
            }
        }

        private object registerObject(Type interfaceType)
        {
            if (underConstruction.Contains(interfaceType))
            {
                throw new InvalidOperationException($"Circular dependency detected in {interfaceType.FullName} constructor");
            }
            underConstruction.Add(interfaceType);
            var obj = createObject(interfaceType);
            _registeredObjects[interfaceType] = obj;
            underConstruction.Remove(interfaceType);
            return obj;
        }

        private object createObject(Type interfaceType)
        {
            var type = _typesMap[interfaceType];
            List<object> parameters = new();
            var constructors = type.GetConstructors();
            if (constructors.Length != 1)
            {
                throw new InvalidOperationException($"Class {type.FullName} should have exactly one constructor");
            }
            var constructor = constructors.First();
            var parametersInfo = constructor.GetParameters();

            foreach (var parameterInfo in parametersInfo)
            {
                var paramType = parameterInfo.ParameterType;
                parameters.Add(getParameter(paramType));
            }
            return constructor.Invoke(parameters.ToArray());
        }

        private object getParameter(Type parameterType)
        {
            var obj = get(parameterType);
            if (obj is not null)
            {
                return obj;
            }
            else
            {
                return registerObject(parameterType);
            }
        }

        private object? get(Type interfaceType)
        {
            if (_registeredObjects.TryGetValue(interfaceType, out var obj))
            {
                return obj;
            }
            return null;
        }
    }
}