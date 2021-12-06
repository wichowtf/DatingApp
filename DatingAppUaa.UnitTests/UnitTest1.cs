using DatingApp.Api.Controllers;
using DatingApp.Api.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using Xunit;

namespace DatingAppUaa.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            /// AAA - Arrange Act Assert
            Assert.True(1 == 1);
        }

        [Theory]
        [InlineData("parametro1", "parametro2", 21)]
        public void Test2(string param1,string param2, int param3)
        {
            // Arrange
            string cadena = param1 + " " + param2;

            // Act
            int length = cadena.Length;

            // Assert
            Assert.Equal(param3, length);
        }
    }
}
