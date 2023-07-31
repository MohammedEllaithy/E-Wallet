using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using WebApi.Appsettings;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly UserSettings _userSettings;

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IOptions<UserSettings> userSettings, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userSettings = userSettings.Value;
        }

        public async Task<User> RegisterUserAsync(RegisterRequest request)
        {
            // Check if the user with the provided email already exists
            var existingUser = await _userRepository.GetUserByPhoneAsync(request.PhoneNumber);
            if (existingUser != null)
            {
                throw new Exception("User with this Phone already exists.");
            }

            // Hash the password before saving it in the database
            var hashedPassword = _passwordHasher.HashPassword(null, request.Password);

            // Create a new User entity with the provided details
            var newUser = new User
            {
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                Balance = _userSettings.Balance,
            // Set other properties as needed
            Password = hashedPassword
            };

            // Save the new user in the database
            await _userRepository.AddUserAsync(newUser);

            return newUser;
        }

        public async Task<AuthenticateResponse> AuthenticateUserAsync(LoginRequest request)
        {
            // Get the user by their email
            var user = await _userRepository.GetUserByPhoneAsync(request.PhoneNumber);
            if (user == null)
            {
                throw new Exception("Invalid email or password.");
            }

            // Verify the provided password against the user's hashed password
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(null, user.Password, request.Password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                throw new Exception("Invalid email or password.");
            }
            var token = _jwtTokenGenerator.GenerateToken(user.Id,user.UserName);
            return new AuthenticateResponse(user, token);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }


        public async Task TransferBalance(string senderMobile, string recipientMobile, decimal amount)
        {
            var sender = await _userRepository.GetUserByPhoneAsync(senderMobile);
            var recipient = await _userRepository.GetUserByPhoneAsync(recipientMobile);

            if (sender == null || recipient == null)
            {
                throw new Exception("Invalid sender or recipient mobile.");
            }

            if (sender.Balance <= amount)
            {
                throw new Exception("Insufficient balance for the transfer.");
            }

            // Perform the balance transfer operation.
            sender.Balance -= amount;
            recipient.Balance += amount;

            await _userRepository.UpdateUser(sender);
            await _userRepository.UpdateUser(recipient);
        }
    }

}
