using DatingApp.Api.DTOs;
using DatingAppUaa.UnitTests.Helpers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DatingAppUaa.UnitTests.Pruebas
{
    public class AccountControllerTests
    {
        private string apiRoute = "api/account";
        private readonly HttpClient _client;
        private HttpResponseMessage httpResponse;
        private string requestUri;
        private string registeredObject;
        private HttpContent httpContent;
        public AccountControllerTests()
        {
            _client = TestHelper.Instance.Client;
        }

        [Theory]
        [InlineData("BadRequest", "lisa", "KnownAs", "Gender", "2000-01-01", "City", "Country", "Password")]
        public async Task registerError(string statusCode, string username, string knownAs, string gender, DateTime dateOfBirth, string city, string country, string password)
        {
            // Arrange
            requestUri = $"{apiRoute}/register";
            var registerDto = new RegisterDto {
                Username = username,
                KnownAs = knownAs,
                Gender = gender,
                DateOfBirth = dateOfBirth,
                City = city,
                Country = country,
                Password = password
            };
            registeredObject = getRegisterObject(registerDto);
            httpContent = getHttpContent(registeredObject);

            // Act
            httpResponse = await _client.PostAsync(requestUri, httpContent);

            // Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("OK", "arturo", "KnownAs", "Gender", "2000-01-01", "City", "Country", "Pa$$w0rd")]
        public async Task registerSuccess(string statusCode, string username, string knownAs, string gender, DateTime dateOfBirth, string city, string country, string password)
        {
            // Arrange
            requestUri = $"{apiRoute}/register";
            var registerDto = new RegisterDto
            {
                Username = username,
                KnownAs = knownAs,
                Gender = gender,
                DateOfBirth = dateOfBirth,
                City = city,
                Country = country,
                Password = password
            };
            registeredObject = getRegisterObject(registerDto);
            httpContent = getHttpContent(registeredObject);

            // Act
            httpResponse = await _client.PostAsync(requestUri, httpContent);

            // Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("Unauthorized", "lisa","Password")]
        public async Task loginError(string statusCode, string username, string password)
        {
            // Arrange
            requestUri = $"{apiRoute}/login";
            var loginDto = new LoginDto
            {
                Username = username,
                Password = password
            };
            registeredObject = getRegisterObject(loginDto);
            httpContent = getHttpContent(registeredObject);

            // Act
            httpResponse = await _client.PostAsync(requestUri, httpContent);

            // Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("OK", "lisa", "Pa$$w0rd")]
        public async Task loginSuccess(string statusCode, string username, string password)
        {
            // Arrange
            requestUri = $"{apiRoute}/login";
            var loginDto = new LoginDto
            {
                Username = username,
                Password = password
            };
            registeredObject = getRegisterObject(loginDto);
            httpContent = getHttpContent(registeredObject);

            // Act
            httpResponse = await _client.PostAsync(requestUri, httpContent);

            // Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        #region Privated methods
        private static string getRegisterObject(RegisterDto registerDto)
        {
            var entityObject = new JObject()
            {
                { nameof(registerDto.Username), registerDto.Username },
                { nameof(registerDto.KnownAs), registerDto.KnownAs },
                { nameof(registerDto.Gender), registerDto.Gender },
                { nameof(registerDto.DateOfBirth), registerDto.DateOfBirth },
                { nameof(registerDto.City), registerDto.City },
                { nameof(registerDto.Country), registerDto.Country },
                { nameof(registerDto.Password), registerDto.Password }
            };

            return entityObject.ToString();
        }

        private static string getRegisterObject(LoginDto loginDto)
        {
            var entityObject = new JObject()
            {
                { nameof(loginDto.Username), loginDto.Username },
                { nameof(loginDto.Password), loginDto.Password }
            };
            return entityObject.ToString();
        }

        private StringContent getHttpContent(string objectToEncode)
        {
            return new StringContent(objectToEncode, Encoding.UTF8, "application/json");
        }

        #endregion
    }
}
