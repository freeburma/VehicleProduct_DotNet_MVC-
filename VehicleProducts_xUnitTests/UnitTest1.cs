using Xunit;

namespace VehicleProducts_xUnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Assert.Equal(1, 1); 
        }

        [Fact]
        public void TestFailed()
        {
            Assert.NotEqual(1, 2); 
        }
    }
}