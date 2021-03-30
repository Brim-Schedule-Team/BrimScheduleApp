﻿using System;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BrimSchedule.API.Services;
using BrimSchedule.API.Utility;
using BrimSchedule.Application.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using NUnit.Framework;

namespace BrimSchedule.Tests.API.Services
{
	[TestFixture]
	public class ExceptionHandlerTests
	{
		private Mock<HttpResponse> _responseMock;
		private Mock<ILoggingManager> _loggerMock;
		private MemoryStream _actualResponseStream;

		[SetUp]
		public void InitializeTest()
		{
			_actualResponseStream = new MemoryStream();
			_loggerMock = new Mock<ILoggingManager>();

			_responseMock = new Mock<HttpResponse>();
			_responseMock
				.Setup(r => r.Body.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);
			_responseMock.SetupGet(r => r.BodyWriter).Returns(PipeWriter.Create(_actualResponseStream));
		}

		[Test]
		public async Task HandleGlobalException_ShouldHandleGenericException_WithDefaultStatusCode_AndDefaultErrorMessage()
		{
			var exception = new InvalidOperationException("Test exception message");
			var httpContextMock = MockHttpContext(exception);
			const int expectedStatusCode = ExceptionHandler.DefaultErrorStatusCode;
			const string expectedErrorMessage = ExceptionHandler.DefaultErrorMessage;

			await ExceptionHandler.HandleGlobalExceptionAsync(httpContextMock);

			AssertResponse(expectedStatusCode, expectedErrorMessage, exception);
		}

		[Test]
		public async Task HandleGlobalException_ShouldHandleUserFriendlyException_WithDefaultStatusCode_AndCustomErrorMessage()
		{
			const string expectedErrorMessage = "Test exception message";
			var exception = new UserFriendlyException(expectedErrorMessage);
			var httpContextMock = MockHttpContext(exception);
			const int expectedStatusCode = ExceptionHandler.DefaultErrorStatusCode;

			await ExceptionHandler.HandleGlobalExceptionAsync(httpContextMock);

			AssertResponse(expectedStatusCode, expectedErrorMessage, exception);
		}

		[Test]
		public async Task HandleGlobalException_ShouldHandleUserFriendlyException_WithCustomStatusCode_AndCustomErrorMessage()
		{
			const string expectedErrorMessage = "Test exception message";
			const int expectedStatusCode = StatusCodes.Status401Unauthorized;
			var exception = new UserFriendlyException(expectedErrorMessage, null, expectedStatusCode);
			var httpContextMock = MockHttpContext(exception);

			await ExceptionHandler.HandleGlobalExceptionAsync(httpContextMock);

			AssertResponse(expectedStatusCode, expectedErrorMessage, exception);
		}

		[Test]
		public async Task HandleGlobalException_ShouldSerializeOriginalException_ForDevelopmentMode()
		{
			const string innerExceptionMessage = "Inner exception test message";
			var innerException = new InvalidOperationException(innerExceptionMessage);

			const string expectedErrorMessage = "Test exception message";
			const int expectedStatusCode = StatusCodes.Status401Unauthorized;
			var exception = new UserFriendlyException(expectedErrorMessage, innerException, expectedStatusCode);

			var httpContextMock = MockHttpContext(exception);

			await ExceptionHandler.HandleGlobalExceptionAsync(httpContextMock, true);

			AssertResponse(expectedStatusCode, expectedErrorMessage, exception);
			AssertResponseContainsText(innerExceptionMessage);
		}

		private HttpContext MockHttpContext(Exception exception)
		{
			var exceptionHandlerFeatureMock = new Mock<IExceptionHandlerPathFeature>();
			exceptionHandlerFeatureMock.SetupGet(e => e.Error).Returns(exception);

			var featureCollectionMock = new Mock<IFeatureCollection>();
			featureCollectionMock.Setup(c => c.Get<IExceptionHandlerPathFeature>())
				.Returns(exceptionHandlerFeatureMock.Object);

			var requestServicesMock = new Mock<IServiceProvider>();
			requestServicesMock.Setup(r => r.GetService(typeof(ILoggingManager))).Returns(_loggerMock.Object);

			var httpContextMock = new Mock<HttpContext>();
			httpContextMock.SetupGet(c => c.Features).Returns(featureCollectionMock.Object);
			httpContextMock.SetupGet(c => c.RequestServices).Returns(requestServicesMock.Object);
			httpContextMock.SetupGet(c => c.Response).Returns(_responseMock.Object);

			return httpContextMock.Object;
		}

		private void AssertResponse(int statusCode, string errorMessage, Exception exception)
		{
			_responseMock.VerifySet(r => r.StatusCode = statusCode);
			_responseMock.VerifySet(r => r.ContentType = ExceptionHandler.DefaultErrorContentType);

			var actualResponseContent = GetResponseContent();
			var deserializedResponse = JsonSerializer.Deserialize<EndpointError>(actualResponseContent);
			deserializedResponse?.ErrorMessage.Should().Be(errorMessage);

			_loggerMock.Verify(l => l.Error(actualResponseContent, exception), Times.Once);
		}

		private void AssertResponseContainsText(string text)
		{
			var actualResponseContent = GetResponseContent();
			actualResponseContent.Contains(text, StringComparison.Ordinal).Should().BeTrue();
		}

		private string GetResponseContent()
		{
			return Encoding.UTF8.GetString(_actualResponseStream.ToArray());
		}
	}
}
