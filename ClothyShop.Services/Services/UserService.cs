using ClothyShop.Common.Enums;
using ClothyShop.DAL.Models;
using ClothyShop.DAL.Repositories;

namespace ClothyShop.Business.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly PasswordService _passwordService;

        public UserService(UserRepository userRepository, PasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return null;

            if (!_passwordService.Verify(user.PasswordHash, password))
                return null;

            return user;
        }

        public async Task<bool> RegisterAsync(string email, string password, string role)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                return false;

            var hashedPassword = _passwordService.HashPassword(password);
            var user = new User
            {
                Email = email,
                PasswordHash = hashedPassword,
                Role = role
            };

            await _userRepository.AddAsync(user);
            return true;
        }
    }
}
