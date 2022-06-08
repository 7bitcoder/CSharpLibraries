using CSharpLibraries.DependencyInjector;
using CSharpLibraries;
// See https://aka.ms/new-console-template for more information

var di = new DependencyInjector();

di.AddShared<ExampleClassB>();
di.AddShared<ExampleClassA>();

var objB = di.getShared<ExampleClassB>();
var objA = objB.obj;
var objAA = di.getShared<ExampleClassA>();
var eq = objA == objAA;
var objAtok = di.getShared<ExampleClassA>("mytoken");
var eqtok = objAtok == objA;
var objun = di.getUnique<ExampleClassA>();
var eqUn = objun == objAA;
var eqTOKen = objAtok == di.getShared<ExampleClassA>("mytoken");
var eqUNI = objun == di.getUnique<ExampleClassA>();
int gg = 1;

