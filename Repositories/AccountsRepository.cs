using LibApp.Data;
using LibApp.Dtos;
using LibApp.Models;
using LibApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace LibApp.Repositories {
    public class AccountsRepository : IAccountRepository {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Customer> _passwordHasher;
        private readonly AuthenticationSettings _authentication;

        public AccountsRepository(ApplicationDbContext context,
            IPasswordHasher<Customer> passwordHasher,
            AuthenticationSettings authentication) {
            _context = context;
            _passwordHasher = passwordHasher;
            _authentication = authentication;
        }

        public void RegisterUser(RegisterUserDTO userDto) {
            var user = new Customer() {
                Email = userDto.Email,
                Name = userDto.Name,
                RoleId = userDto.RoleId,
                HasNewsletterSubscribed = userDto.HasNewsletterSubscribed,
                MembershipTypeId = userDto.MembershipTypeId,
                Birthdate = userDto.Birthdate
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, userDto.Password);

            _context.Customers.Add(user);
            _context.SaveChanges();
        }

        public string GenerateJwt(LoginUserDTO loginDto) {
            var user = _context.Customers
                .Include(u => u.Role)
                .Include(u => u.MembershipType)
                .FirstOrDefault(u => u.Email == loginDto.Email);

            if (user == null ||
                _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password) == PasswordVerificationResult.Failed)
                throw new BadHttpRequestException("Invalid username or password");

            var claims = new List<Claim> {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Role, user.Role.Name),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authentication.JwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authentication.JwtExpireDays);

            var token = new JwtSecurityToken(
                _authentication.JwtIssuer,
                _authentication.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}