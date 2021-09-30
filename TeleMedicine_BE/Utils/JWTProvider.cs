using Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BeautyAtHome.Utils
{
    public interface IJwtTokenProvider
    {
        Task<string> GenerateToken(Account accountCreated);
        Task<string> GetPayloadFromToken(string tokenString, string key);

    }
    public class JwtTokenProvider : IJwtTokenProvider
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly IConfiguration _configuration;

        public JwtTokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtSecurityTokenHandler = _jwtSecurityTokenHandler ?? new JwtSecurityTokenHandler();
        }

        public Task<string> GenerateToken(Account accountCreated)
        {
            return Task.Run(() =>
            {
                var symmectricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:Key"]));

                //signing credentials
                var signingCredentials = new SigningCredentials(symmectricSecurityKey, SecurityAlgorithms.HmacSha256);

                var additionalClaims = new List<Claim>
                {
                    new Claim("role", accountCreated.Role.Id.ToString()),
                    new Claim("uid", accountCreated.Id.ToString()),
                    new Claim("email", accountCreated.Email.ToString())
                };

                var token = new JwtSecurityToken(
                        issuer: _configuration["jwt:Issuer"],
                        audience: _configuration["jwt:Audience"],
                        expires: DateTime.Now.AddDays(1),
                        claims: additionalClaims,
                        signingCredentials: signingCredentials
                    );

                //return token
                return _jwtSecurityTokenHandler.WriteToken(token);
            });

        }

        public Task<string> GetPayloadFromToken(string tokenString, string key)
        {
            return Task.Run(() => (string)_jwtSecurityTokenHandler.ReadJwtToken(tokenString).Payload[key]);
        }
    }
}