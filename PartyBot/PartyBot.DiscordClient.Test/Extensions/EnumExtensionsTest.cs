using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartyBot.DiscordClient.Extensions;

namespace PartyBot.DiscordClient.Test.Extensions
{
    [TestClass]
    public class EnumExtensionsTest
    {
        [TestMethod]
        public void EnumExtensionsTest_GetDescription()
        {
            // Arrange
            var testValue = TestEnum.Foo;


            // Act
            var result = testValue.GetDescription();

            // Assert
            Assert.AreEqual("Foooo", result);
        }

        private enum TestEnum
        {
            [System.ComponentModel.Description("Foooo")]
            Foo,
            Bar
        }
    }
}