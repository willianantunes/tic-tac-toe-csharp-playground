using System.Collections.Generic;
using Xunit;

namespace tests.Unit
{
    
    
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //Arrange
            string str1 = "Hello";
            string str2 = " ";
            string str3 = "World!";

            var some = new List<int>();
            int a = 1;
            some.Add(a);
            a++;
            

            //Act 
            string result = string.Concat(str1, str2, str3);

            //Assert
            Assert.Equal("Hello World!", result);
        }
    }
}