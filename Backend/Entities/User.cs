namespace WebApi.Entities;

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class User : IdentityUser<Guid>
{
    public decimal Balance { get; set; }

    [JsonIgnore]
    public string Password { get; set; }
}