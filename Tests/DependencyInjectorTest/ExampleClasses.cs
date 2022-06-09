using Library;

namespace Tests.DependencyInjectorTest;

internal class DefConstructor
{
}

internal class SimpleReference
{
    public DefConstructor Def { get; }

    public SimpleReference(DefConstructor def)
    {
        Def = def;
    }
}

internal class ManySimpleReference
{
    public DefConstructor Def { get; }
    public SimpleReference Simple { get; }

    public ManySimpleReference(DefConstructor def, SimpleReference simple)
    {
        Def = def;
        Simple = simple;
    }
}

internal class UniqueReference
{
    public DefConstructor Def { get; }
    public SimpleReference Simple { get; }

    public UniqueReference([Unique] DefConstructor def, [Unique] SimpleReference simple)
    {
        Def = def;
        Simple = simple;
    }
}

internal class SharedWithTokenReference
{
    public DefConstructor Def { get; }
    public SimpleReference Simple { get; }

    public SharedWithTokenReference([Shared("token")] DefConstructor def, [Shared("token")] SimpleReference simple)
    {
        Def = def;
        Simple = simple;
    }
}

internal class SelfReference
{
    public SelfReference Self { get; }

    public SelfReference(SelfReference self)
    {
        Self = self;
    }
}

internal class CircularDependency1
{
    public CircularDependency2 Circular { get; }

    public CircularDependency1(CircularDependency2 circular)
    {
        Circular = circular;
    }
}

internal class CircularDependency2
{
    public CircularDependency1 Circular { get; }

    public CircularDependency2(CircularDependency1 circular)
    {
        Circular = circular;
    }
}

internal class Parameter
{
}

internal class UnregisteredParameter
{
    public Parameter Parameter { get; }

    public UnregisteredParameter(Parameter parameter)
    {
        Parameter = parameter;
    }
}

internal class MultipleConstructors
{
    public Parameter Parameter { get; }

    public MultipleConstructors(Parameter parameter)
    {
        Parameter = parameter;
    }

    public MultipleConstructors(Parameter parameter, Parameter parameter2)
    {
        Parameter = parameter;
    }
}

internal interface ISimpleInherance
{

}

internal class SimpleInherance : ISimpleInherance
{
    public Parameter Parameter { get; }

    public SimpleInherance(Parameter parameter)
    {
        Parameter = parameter;
    }
}


internal interface IDeepInherance
{
    public ISimpleInherance Parameter { get; }

}

internal class DeepInherance : IDeepInherance
{
    public ISimpleInherance Parameter { get; }

    public DeepInherance(ISimpleInherance parameter)
    {
        Parameter = parameter;
    }
}

internal interface IMutipleInherance
{

}

internal class MutipleInherance1 : IMutipleInherance
{
    public ISimpleInherance Parameter { get; }

    public MutipleInherance1(ISimpleInherance parameter)
    {
        Parameter = parameter;
    }
}

internal class MutipleInherance2 : IMutipleInherance
{
    public ISimpleInherance Parameter { get; }

    public MutipleInherance2(ISimpleInherance parameter)
    {
        Parameter = parameter;
    }
}


internal class MutipleInheranceRef
{
    public IMutipleInherance[] Parameters { get; }

    public MutipleInheranceRef(IEnumerable<IMutipleInherance> parameters)
    {
        Parameters = parameters.ToArray();
    }
}