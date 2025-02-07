using AppointmentBooking.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace AppointmentBooking.Tests.Middleware
{
    /// <summary>
    /// Unit tests for the ExceptionHandlingMiddleware class
    /// </summary>
    public class ExceptionHandlingMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _nextMock;
        private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
        private readonly ExceptionHandlingMiddleware _middleware;

        public ExceptionHandlingMiddlewareTests()
        {
            // Initialize mocks and create middleware instance for testing
            _nextMock = new Mock<RequestDelegate>();
            _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            _middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task InvokeAsync_HandlesArgumentException_ReturnsBadRequest()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream(); // Use MemoryStream to capture response

            // Setup middleware to throw ArgumentException
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(new ArgumentException("Invalid argument"));

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var response = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var responseObject = JsonSerializer.Deserialize<JsonElement>(response);

            // Verify status code and error message
            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
            Assert.Equal("Invalid argument", responseObject.GetProperty("message").GetString());
        }

        [Fact]
        public async Task InvokeAsync_HandlesGenericException_ReturnsInternalServerError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Setup middleware to throw generic Exception
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(new Exception("An error occurred"));

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var response = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var responseObject = JsonSerializer.Deserialize<JsonElement>(response);

            // Verify 500 status code and error message
            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
            Assert.Equal("An error occurred", responseObject.GetProperty("message").GetString());
        }

        [Fact]
        public async Task InvokeAsync_LogsException()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var exception = new Exception("An error occurred");

            // Setup middleware to throw test exception
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            // Verify that the logger was called with LogLevel.Error and the correct exception
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.Is<Exception>(ex => ex == exception),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }
    }
}