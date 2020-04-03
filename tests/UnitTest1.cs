using Xunit;

namespace tests
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

            //Act 
            string result = string.Concat(str1, str2, str3);

            //Assert
            Assert.Equal("Hello World!", result);
        }
    }
}