using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Collection<Car> Cars { get; set; } = [];
}