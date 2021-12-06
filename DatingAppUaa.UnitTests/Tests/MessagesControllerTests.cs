using DatingApp.Api.DTOs;
using DatingAppUaa.UnitTests.Helpers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DatingAppUaa.UnitTests.Pruebas
{
    public class MessagesControllerTests
    {
        private string apiRoute = "api/messages";
        private readonly HttpClient _client;
        private HttpResponseMessage httpResponse;
        private string requestUri;
        private string registeredObject;
        private HttpContent httpContent;
        public MessagesControllerTests()
        {
            _client = TestHelper.Instance.Client;
        }
        [Theory]
        [InlineData("BadRequest", "lois", "Pa$$w0rd", "lois","Hola")]
        public async Task badRequestMessage(string statusCode, string username, string password, string recipientUsername,string content)
        {
            // Arrange
            var user = await LoginHelper.LoginUser(username, password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

            var messageDto = new MessageDto { 
                RecipientUsername= recipientUsername,
                Content = content
            };
            registeredObject = getRegisterObject(messageDto);
            httpContent = getHttpContent(registeredObject);
            requestUri = $"{apiRoute}";

            // Act
            httpResponse = await _client.PostAsync(requestUri, httpContent);
            _client.DefaultRequestHeaders.Authorization = null;
            // Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }
        [Theory]
        [InlineData("NotFound", "lisa", "Pa$$w0rd", "pedritosola", "Hola")]
        public async Task notFoundMessage(string statusCode, string username, string password, string recipientUsername, string content)
        {
            // Arrange
            var user = await LoginHelper.LoginUser(username, password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

            var messageDto = new MessageDto
            {
                RecipientUsername = recipientUsername,
                Content = content
            };
            registeredObject = getRegisterObject(messageDto);
            httpContent = getHttpContent(registeredObject);
            requestUri = $"{apiRoute}";

            // Act
            httpResponse = await _client.PostAsync(requestUri, httpContent);
            _client.DefaultRequestHeaders.Authorization = null;
            // Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }
        [Theory]
        [InlineData("OK", "porter", "Pa$$w0rd", "lisa", "Hola")]
        public async Task successMessage(string statusCode, string username, string password, string recipientUsername, string content)
        {
            // Arrange
            var user = await LoginHelper.LoginUser(username, password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

            var messageDto = new MessageDto
            {
                RecipientUsername = recipientUsername,
                Content = content
            };
            registeredObject = getRegisterObject(messageDto);
            httpContent = getHttpContent(registeredObject);
            requestUri = $"{apiRoute}";

            // Act
            httpResponse = await _client.PostAsync(requestUri, httpContent);
            _client.DefaultRequestHeaders.Authorization = null;
            // Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("OK", "todd", "Pa$$w0rd", "ruthie", "Hola")]
        public async Task getUserMessagesSuccess(string statusCode, string username, string password, string recipientUsername, string content)
        {
            // Arrange
            var user = await LoginHelper.LoginUser(username, password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

            requestUri = $"{apiRoute}";

            // Act
            httpResponse = await _client.GetAsync(requestUri);
            _client.DefaultRequestHeaders.Authorization = null;
            // Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("OK", "skinner", "Pa$$w0rd", "Outbox")]
        public async Task getUserMessagesQuerySuccess(string statusCode, string username, string password, string container)
        {
            // Arrange
            var user = await LoginHelper.LoginUser(username, password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

            requestUri = $"{apiRoute}"+ "?container="+container;

            // Act
            httpResponse = await _client.GetAsync(requestUri);
            _client.DefaultRequestHeaders.Authorization = null;
            // Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("OK", "ruthie", "Pa$$w0rd", "lisa")]
        public async Task messagesThreadSuccess(string statusCode, string username, string password, string user2)
        {
            // Arrange
            var user = await LoginHelper.LoginUser(username, password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

            requestUri = $"{apiRoute}/thread/" + user2;

            // Act
            httpResponse = await _client.GetAsync(requestUri);
            _client.DefaultRequestHeaders.Authorization = null;
            // Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }


        [Theory]
        [InlineData("OK", "davis", "Pa$$w0rd", "karen", "Hola")]
        public async Task deleteMessageSuccess(string statusCode, string username, string password, string recipientUsername, string content)
        {
            // Arrange
            var user = await LoginHelper.LoginUser(username, password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

            var messageDto = new MessageDto
            {
                RecipientUsername = recipientUsername,
                Content = content
            };
            registeredObject = getRegisterObject(messageDto);
            httpContent = getHttpContent(registeredObject);
            requestUri = $"{apiRoute}";
            var result = await _client.PostAsync(requestUri, httpContent);
            var messageJson = await result.Content.ReadAsStringAsync();
            var message = messageJson.Split(',');
            var id = message[0].Split("\"")[2].Split(":")[1];
            requestUri = $"{apiRoute}/"+id;

            // Act
            httpResponse = await _client.DeleteAsync(requestUri);
            _client.DefaultRequestHeaders.Authorization = null;
            user = await LoginHelper.LoginUser(recipientUsername, password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

            // Act
            httpResponse = await _client.DeleteAsync(requestUri);
            _client.DefaultRequestHeaders.Authorization = null;
            // Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("Unauthorized", "mayo", "Pa$$w0rd", "lisa", "Hola", "margo")]
        public async Task deleteUnauthorizedMessage(string statusCode, string username, string password, string recipientUsername, string content,string unauth)
        {
            // Arrange
            var user = await LoginHelper.LoginUser(username, password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

            var messageDto = new MessageDto
            {
                RecipientUsername = recipientUsername,
                Content = content
            };
            registeredObject = getRegisterObject(messageDto);
            httpContent = getHttpContent(registeredObject);
            requestUri = $"{apiRoute}";
            var result = await _client.PostAsync(requestUri, httpContent);
            var messageJson = await result.Content.ReadAsStringAsync();
            _client.DefaultRequestHeaders.Authorization = null;
            var message = messageJson.Split(',');
            var id = message[0].Split("\"")[2].Split(":")[1];
            requestUri = $"{apiRoute}/" + id;

            user = await LoginHelper.LoginUser(unauth, password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

            // Act
            httpResponse = await _client.DeleteAsync(requestUri);
            _client.DefaultRequestHeaders.Authorization = null;
            // Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }


        #region Privated methods
        private static string getRegisterObject(MessageDto message)
        {
            var entityObject = new JObject()
            {
                { nameof(message.RecipientUsername), message.RecipientUsername },
                { nameof(message.Content), message.Content }
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
