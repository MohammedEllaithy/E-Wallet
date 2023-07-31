namespace WebApi.Models;

using WebApi.Entities;

public class AuthenticateResponse
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public decimal Balance { get; set; }
    public string Token { get; set; }


    public AuthenticateResponse(User user, string token)
    {
        Id = user.Id;
        PhoneNumber = user.PhoneNumber;
        UserName = user.UserName;
        Balance = user.Balance;
        Token = token;
    }
}