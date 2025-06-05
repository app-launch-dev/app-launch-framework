using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace AppLaunch.Services.Data;

public class ApplicationUser: IdentityUser
{
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
}