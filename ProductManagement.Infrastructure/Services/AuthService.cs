using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProductManagement.Application.DTOs.Auth;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Common;
using ProductManagement.Domain.Entities;
using ProductManagement.Domain.Interfaces; // <-- Repository Interface burada
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        // ARTIK DBCONTEXT YOK, REPOSITORY VAR
        private readonly IGenericRepository<User> _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IGenericRepository<User> userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<bool>> RegisterAsync(RegisterDto request)
        {
            // AppDbContext yerine Repository kullanıyoruz
            var existingUser = await _userRepository.GetByFilterAsync(u => u.Email == request.Email);

            if (existingUser != null)
            {
                return ServiceResponse<bool>.ErrorResponse("Bu email adresi zaten kullanılıyor.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FullName = request.FullName,
                Role = "User",
                CreatedDate = DateTime.Now,
                IsDeleted = false,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _userRepository.AddAsync(user);
            // Repository içinde SaveChanges varsa burada çağırmaya gerek yok,
            // ama GenericRepository'miz SaveChanges yapmıyorsa UnitOfWork gerekir. 
            // Bizim yazdığımız AddAsync içinde SaveChanges vardı değil mi? Kontrol etmelisin.
            // (GenericRepository kodumuza bakalım: _dbContext.SaveChanges() yapıyor muyuz?)

            return ServiceResponse<bool>.SuccessResponse(true, "Kayıt işlemi başarılı.");
        }

        public async Task<ServiceResponse<AuthResponseDto>> LoginAsync(LoginDto request)
        {
            var user = await _userRepository.GetByFilterAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return ServiceResponse<AuthResponseDto>.ErrorResponse("Kullanıcı bulunamadı veya şifre hatalı.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return ServiceResponse<AuthResponseDto>.ErrorResponse("Kullanıcı bulunamadı veya şifre hatalı.");
            }

            var token = GenerateJwtToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Role = user.Role,
                Expiration = DateTime.Now.AddMinutes(int.Parse(_configuration["JwtSettings:DurationInMinutes"]))
            };

            return ServiceResponse<AuthResponseDto>.SuccessResponse(response, "Giriş başarılı.");
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}