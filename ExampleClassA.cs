namespace CSharpLibraries
{
    public class ExampleClassA
    {
        public int id = 100;
        public ExampleClassA()
        {
            id = 101;
        }

        public ExampleClassA(int gg)
        {
            id = 101;
        }
    }

    public class ExampleClassB
    {
        public int id = 100;
        public ExampleClassA obj;
        public ExampleClassB(ExampleClassA exampleClass)
        {
            id = 101;
            obj = exampleClass;
        }
    }
}