using Microsoft.AspNetCore.Identity;

namespace MyAcademyCQRS.Entities;

public class AppUser : IdentityUser
{
    public string Name { get; set; }
    public string Surname { get; set; }
}
