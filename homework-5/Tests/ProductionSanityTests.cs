using System.Net;
using System.Net.Http.Json;
using CampusServiceHomework5.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CampusServiceHomework5.Tests;

public class ProductionSanityTests
{
    [Fact]
    public async Task Sanity_CreateAndDeleteCertificateRequest()
    {
        await using WebApplicationFactory<Program> factory = new();

        HttpClient client = factory.CreateClient();

        CertificateRequest request = new()
        {
            Id = 9001,
            StudentId = "SANITY-STUDENT",
            CertificateType = "Тестовая справка"
        };

        // Создание тестовой заявки
        HttpResponseMessage createResponse =
            await client.PostAsJsonAsync(
                "/api/v1/certificate-requests",
                request);

        Assert.Equal(
            HttpStatusCode.OK,
            createResponse.StatusCode);

        // Cleanup — удаление тестовой заявки
        HttpResponseMessage deleteResponse =
            await client.DeleteAsync(
                $"/api/v1/certificate-requests/{request.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);
    }
}
