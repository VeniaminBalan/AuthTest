namespace AuthTest.Controllers;

public class CarRequestForUpdate
{
    public required string Id { get; set; }
    public string? Model { get; set; }
    public string? PhotoUrl { get; set; }
}