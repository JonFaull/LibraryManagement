using System.Net;
using System.Net.Http.Json;
using LibraryMgmt.Common;
using LibraryMgmt;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;

namespace LibraryMgmt.Tests.Controllers
{
    public class GlobalExceptionHandlerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public GlobalExceptionHandlerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GlobalExceptionHandler_ReturnsStandardErrorResponse_OnUnhandledException()
        {
            var response = await _client.GetAsync("/api/ExceptionTest/Throw");

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

            var result = await response.Content.ReadFromJsonAsync<OperationalResult<object>>();

            result.Should().NotBeNull();
            result!.Success.Should().BeFalse();
            result.Message.Should().Be("An unexpected error occurred.");
            result.Code.Should().Be(ErrorCode.InternalServerError);
        }
    }
}
