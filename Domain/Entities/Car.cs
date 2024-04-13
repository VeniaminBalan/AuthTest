namespace Domain.Entities;

public class Car
{
    public required string Id { get; set; }
    public required string Model { get; set; }
    public required string PhotoUrl { get; set; }
    public required User Owner { get; set; }

    public void UpdateCar(string? model, string? photoUrl)
    {
        if(model is not null)
            Model = model;
        
        if(photoUrl is not null)
            PhotoUrl = photoUrl;
    }
}