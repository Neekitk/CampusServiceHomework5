using System.Diagnostics;
using CampusServiceHomework5.Models;
using Microsoft.AspNetCore.Mvc;

namespace CampusServiceHomework5.Controllers;

[ApiController]
[Route("api/v1/certificate-requests")]
public class CertificateRequestsController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateRequest(
        [FromBody] CertificateRequest request)
    {
        Activity.Current?.SetTag(
            "jira.issue",
            "CAMPUS-107");

        Activity.Current?.SetTag(
            "campus.request_id",
            request.Id);

        Activity.Current?.SetTag(
            "campus.certificate_type",
            request.CertificateType);

        request.Status = "Created";
        request.StatusCode = 1;

        return Ok(request);
    }
}