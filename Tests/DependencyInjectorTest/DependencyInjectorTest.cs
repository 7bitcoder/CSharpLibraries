using Library;

namespace Tests.DependencyInjectorTest;

public class DependencyInjectorTest
{
    public DependencyInjector DI { get; set; } = new DependencyInjector();

    [Fact]
    public void ShouldRegisterSharedService()
    {
        var act = () => DI.AddShared<DefConstructor>();

        act.Should().NotThrow();
    }

    [Fact]
    public void ShouldRegisterInheritedSharedService()
    {
        var act = () => DI.AddShared<ISimpleInherance, SimpleInherance>();

        act.Should().NotThrow();
    }

    [Fact]
    public void ShouldRegisterComplexSharedService()
    {
        var act = () => DI.AddShared<ManySimpleReference>();

        act.Should().NotThrow();
    }

    [Fact]
    public void ShouldRegisterUniqueService()
    {
        var act = () => DI.AddUnique<DefConstructor>();

        act.Should().NotThrow();
    }

    [Fact]
    public void ShouldRegisterInheritedUniqueService()
    {
        var act = () => DI.AddUnique<ISimpleInherance, SimpleInherance>();

        act.Should().NotThrow();
    }

    [Fact]
    public void ShouldRegisterComplexUniqueService()
    {
        var act = () => DI.AddUnique<ManySimpleReference>();

        act.Should().NotThrow();
    }

    [Fact]
    public void ShouldGetNullIfServiceWasNotRegistered()
    {
        var service = DI.GetUnique<DefConstructor>();

        service.Should().BeNull();

    }

    [Fact]
    public void ShouldGetUniqueService()
    {
        DI.AddUnique<DefConstructor>();

        var service = DI.GetUnique<DefConstructor>();
        var service2 = DI.GetUnique<DefConstructor>();

        service.Should().NotBeNull();
        service2.Should().NotBeNull();
        service.Should().NotBe(service2);
    }

    [Fact]
    public void ShouldGetUniqueINheritedService()
    {
        DI.AddUnique<ISimpleInherance, SimpleInherance>();
        DI.AddUnique<Parameter>();

        var service = DI.GetUnique<ISimpleInherance>();
        var service2 = DI.GetUnique<ISimpleInherance>();

        service.Should().NotBeNull();
        service2.Should().NotBeNull();
        service.Should().NotBe(service2);
    }

    [Fact]
    public void ShouldGetSharedService()
    {
        DI.AddShared<DefConstructor>();

        var service = DI.GetShared<DefConstructor>();
        var service2 = DI.GetShared<DefConstructor>();

        service.Should().NotBeNull();
        service2.Should().NotBeNull();
        service.Should().Be(service2);
    }

    [Fact]
    public void ShouldGetSharedInheritedService()
    {
        DI.AddShared<ISimpleInherance, SimpleInherance>();
        DI.AddUnique<Parameter>();

        var service = DI.GetShared<ISimpleInherance>();
        var service2 = DI.GetShared<ISimpleInherance>();

        service.Should().NotBeNull();
        service2.Should().NotBeNull();
        service.Should().Be(service2);
    }

    [Fact]
    public void ShouldGetSharedTokenService()
    {
        DI.AddShared<DefConstructor>();
        DI.AddShared<DefConstructor>("Token");

        var service = DI.GetShared<DefConstructor>("Token");
        var service2 = DI.GetShared<DefConstructor>("Token");
        var service3 = DI.GetShared<DefConstructor>();

        service.Should().NotBeNull();
        service2.Should().NotBeNull();
        service3.Should().NotBeNull();
        service.Should().Be(service2);
        service.Should().NotBe(service3);
    }

    [Fact]
    public void ShouldGetSharedTokenInheritedService()
    {
        DI.AddShared<IDeepInherance, DeepInherance>("Token");
        DI.AddShared<ISimpleInherance, SimpleInherance>();
        DI.AddUnique<Parameter>();

        var service = DI.GetShared<IDeepInherance>("Token");
        var service2 = DI.GetShared<IDeepInherance>("Token");
        var service3 = DI.GetShared<ISimpleInherance>();

        service.Should().NotBeNull();
        service2.Should().NotBeNull();
        service3.Should().NotBeNull();
        service.Should().Be(service2);
        service.Should().NotBe(service3);
    }

    [Fact]
    public void ShouldGetUniqueFromSharedService()
    {
        DI.AddShared<DefConstructor>();
        DI.AddShared<DefConstructor>("Token");

        var service = DI.GetShared<DefConstructor>("Token");
        var service2 = DI.GetShared<DefConstructor>();
        var service3 = DI.GetUnique<DefConstructor>();

        service.Should().NotBeNull();
        service2.Should().NotBeNull();
        service3.Should().NotBeNull();
        service.Should().NotBe(service2);
        service.Should().NotBe(service3);
        service2.Should().NotBe(service3);
    }

    [Fact]
    public void ShouldNotGetSharedFromUniqueService()
    {
        DI.AddUnique<DefConstructor>();

        var service = DI.GetShared<DefConstructor>("Token");
        var service2 = DI.GetShared<DefConstructor>();
        var service3 = DI.GetUnique<DefConstructor>();

        service.Should().BeNull();
        service2.Should().BeNull();
        service3.Should().NotBeNull();
    }

    [Fact]
    public void ShouldFailOnSelfReferencingUniqueService()
    {
        DI.AddUnique<SelfReference>();

        var act = () => DI.GetUnique<SelfReference>();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ShouldFailOnSelfReferencingSharedService()
    {
        DI.AddShared<SelfReference>();

        var act = () => DI.GetShared<SelfReference>();
        var act2 = () => DI.GetUnique<SelfReference>();

        act.Should().Throw<InvalidOperationException>();
        act2.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ShouldFailOnCircularDependency()
    {
        DI.AddShared<CircularDependency1>();
        DI.AddShared<CircularDependency2>();

        var act = () => DI.GetShared<CircularDependency2>();
        var act2 = () => DI.GetUnique<CircularDependency1>();

        act.Should().Throw<InvalidOperationException>();
        act2.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ShouldFailOnUnregisteredParameter()
    {
        DI.AddShared<UnregisteredParameter>();

        var act = () => DI.GetShared<UnregisteredParameter>();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ShouldFailOnMultipleConstructorsService()
    {
        DI.AddShared<MultipleConstructors>();

        var act = () => DI.GetShared<MultipleConstructors>();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ShouldFailAcceptDefaultScopedWithoutApecificAttributes()
    {
        DI.AddShared<ManySimpleReference>();
        DI.AddUnique<DefConstructor>();
        DI.AddShared<SimpleReference>();

        var service = DI.GetShared<ManySimpleReference>();
        var uniqueService = DI.GetUnique<DefConstructor>();
        var sharedService = DI.GetShared<SimpleReference>();

        service.Should().NotBeNull();
        uniqueService.Should().NotBeNull();
        sharedService.Should().NotBeNull();
        service.Def.Should().NotBe(uniqueService);
        service.Simple.Should().Be(sharedService);
    }

    [Fact]
    public void ShouldFailAcceptDefaultScopedWithoutInheranceApecificAttributes()
    {
        DI.AddShared<IDeepInherance, DeepInherance>();
        DI.AddShared<ISimpleInherance, SimpleInherance>();
        DI.AddUnique<Parameter>();

        var service = DI.GetShared<IDeepInherance>();
        var uniqueService = DI.GetUnique<ISimpleInherance>();
        var sharedService = DI.GetShared<Parameter>();

        service.Should().NotBeNull();
        uniqueService.Should().NotBeNull();
        sharedService.Should().BeNull();
        service.Parameter.Should().NotBe(uniqueService);
    }

    [Fact]
    public void ShouldFailAcceptUniqueScopedWithApecificAttributes()
    {
        DI.AddShared<UniqueReference>();
        DI.AddUnique<DefConstructor>();
        DI.AddShared<SimpleReference>();

        var service = DI.GetShared<UniqueReference>();
        var uniqueService = DI.GetUnique<DefConstructor>();
        var sharedService = DI.GetShared<SimpleReference>();

        service.Should().NotBeNull();
        uniqueService.Should().NotBeNull();
        sharedService.Should().NotBeNull();
        service.Def.Should().NotBe(uniqueService);
        service.Simple.Should().NotBe(sharedService);
    }

    [Fact]
    public void ShouldFailAcceptSharedScopedWithApecificAttributes()
    {
        DI.AddShared<SharedWithTokenReference>();
        DI.AddShared<DefConstructor>();
        DI.AddShared<DefConstructor>("token");
        DI.AddShared<SimpleReference>("token");

        var service = DI.GetShared<SharedWithTokenReference>();
        var uniqueService = DI.GetShared<DefConstructor>();
        var sharedService = DI.GetShared<SimpleReference>("token");

        service.Should().NotBeNull();
        uniqueService.Should().NotBeNull();
        sharedService.Should().NotBeNull();
        service.Def.Should().NotBe(uniqueService);
        service.Simple.Should().Be(sharedService);
    }

    [Fact]
    public void ShouldGetArrayTypes()
    {
        DI.AddShared<IMutipleInherance, MutipleInherance2>();
        DI.AddShared<IMutipleInherance, MutipleInherance1>();
        DI.AddShared<MutipleInheranceRef>();
        DI.AddShared<ISimpleInherance, SimpleInherance>();
        DI.AddShared<Parameter>();

        var service = DI.GetShared<MutipleInheranceRef>();
        var uniqueServices = DI.GetShares<IMutipleInherance>();

        service.Should().NotBeNull();
        uniqueServices.Should().NotBeNull();
        service.Parameters.Should().BeEquivalentTo(uniqueServices);
    }

}