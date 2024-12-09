using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FeatureToggle.Application.DTOs;
using FeatureToggle.Domain.ConfigurationModels;
using FeatureToggle.Domain.Entity.FeatureManagementSchema;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FeatureToggle.Application.Requests.Queries.Login
{
    public class GetAuthTokenQueryHandler(UserManager<User> userManager, IOptionsMonitor<Authentication> optionsMonitor) : IRequestHandler<GetAuthTokenQuery, LoginResponseDTO>
    {
        private readonly UserManager<User> _userManager = userManager;  
        private readonly IOptionsMonitor<Authentication> _optionsMonitor = optionsMonitor;

        public async Task<LoginResponseDTO> Handle(GetAuthTokenQuery request, CancellationToken cancellationToken)
        {
            User? user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
            {
                string secretKey = _optionsMonitor.CurrentValue.JWTSecret;
                SymmetricSecurityKey signInKey = new(Encoding.UTF8.GetBytes(secretKey));
                SecurityTokenDescriptor tokenDescriptor = new()
                {
                    Subject = new ClaimsIdentity(
                    [
                        new Claim("UserID",user.Id.ToString()),
                        new Claim("IsAdmin",user.IsAdmin.ToString())
                    ]),
                    Expires = DateTime.UtcNow.AddHours(_optionsMonitor.CurrentValue.ExpiryTime),
                    SigningCredentials = new SigningCredentials(signInKey,SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _optionsMonitor.CurrentValue.Issuer,
                    Audience = _optionsMonitor.CurrentValue.Audience,
                };
                JwtSecurityTokenHandler tokenHandler = new();
                SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
                string token = tokenHandler.WriteToken(securityToken);

                return new LoginResponseDTO { Token = token };
            }
            else
            {
                return new LoginResponseDTO { ErrorMessage = "Incorrect Username or Password" };
            }
        }
    }

    
}
