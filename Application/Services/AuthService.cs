using Application.Interfaces;
using Application.Utils;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepo;

        public AuthService(IConfiguration config, IUserRepository userRepo)
        {
            _config = config;
            _userRepo = userRepo;
        }

        public async Task<string> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepo.GetByUsernameAsync(username);
            if (user is null) return string.Empty;

            var hasher = new PasswordHasher<string>();
            var result = hasher.VerifyHashedPassword(username, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed) return string.Empty;

            return JwtTokenGenerator.GenerateToken(user.Username, user.Role, _config);
        }

    }
}