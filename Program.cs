using CSharpLibraries.DependencyInjector;
using CSharpLibraries;
// See https://aka.ms/new-console-template for more information

var di = new DependencyInjector();

di.AddSingeleton<ExampleClassB>();
di.AddSingeleton<ExampleClassA>();

di.build();

var objB = di.get<ExampleClassB>();
var objA = objB.obj;