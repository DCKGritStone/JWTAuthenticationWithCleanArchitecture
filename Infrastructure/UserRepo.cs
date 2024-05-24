using Domain.Dto;
using Domain.Entities;
using Domain.Interface;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure
{
    public class UserRepo : IUser
    {
        private readonly AppDbContext dbContext;
        private readonly IConfiguration configuration;

        public UserRepo(AppDbContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
        }
        public async Task<LoginResponse> LoginUserAsync(LoginDto loginDto)
        {
            var getUser = await FindUserByEmail(loginDto.Email);
            if (getUser == null) { return new LoginResponse(false, "User not found, sorry"); }

            bool checkPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, getUser.Password);
            if (checkPassword)
            {
                return new LoginResponse(true, "Login successful", GenerateJWTToken(getUser));
            }

            return new LoginResponse(false, "Invalid credentials");
        }

        private string GenerateJWTToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(5),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<ApplicationUser> FindUserByEmail(string email) =>
            await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        public async Task<RegistrationResponse> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            var getUser = await FindUserByEmail(registerUserDto.Email);

            if (getUser != null) { return new RegistrationResponse(false, "User already exist"); }

            dbContext.Users.Add(new ApplicationUser()
            {
                Name = registerUserDto.Name,
                Email = registerUserDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password)
            });
            await dbContext.SaveChangesAsync();
            return new RegistrationResponse(true, "Registration completed");
        }
    }
}
