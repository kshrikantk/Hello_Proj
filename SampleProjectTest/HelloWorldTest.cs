using System;
using SampleProject;
using Xunit;

namespace SampleProjectTest
{
    public class HelloWorldTest
    {
        [Fact]
        public void HelloWorld_InvalidInput_ReturnsInvalidInput()
        {
            string input = null;
            string expectedresult = "Invalid input";

            string actualResult = Class1.HelloWorld(input);

            Assert.Equal(actualResult, expectedresult);
        }

        [Fact]
        public void HelloWorld_ValidInput_ReturnsValidInput()
        {
            string input = "Input";
            string expectedresult = "Valid input";

            string actualResult = Class1.HelloWorld(input);

            Assert.Equal(actualResult, expectedresult);
        }
    }
}
