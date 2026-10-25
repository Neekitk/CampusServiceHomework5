namespace CampusServiceHomework5.Models;

public class CertificateRequest
{
    public int Id { get; set; }

    public string StudentId { get; set; } = string.Empty;

    public string CertificateType { get; set; } = string.Empty;

    public string Status { get; set; } = "Created";

    public int? StatusCode { get; set; }
}