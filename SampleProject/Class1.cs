using System;

namespace SampleProject
{
    public class Class1
    {
        public static string HelloWorld(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                Console.WriteLine(input);
                Console.ReadLine();
                return "Valid input";
            }

            return "Invalid input";
        }
    }
}
