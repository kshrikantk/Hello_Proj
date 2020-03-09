using System;

namespace SampleProject
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Class1.HelloWorld("Hello World");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
    }


}
