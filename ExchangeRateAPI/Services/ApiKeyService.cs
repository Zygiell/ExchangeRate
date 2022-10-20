using ExchangeRateAPI.Auth;
using ExchangeRateAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExchangeRateAPI.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly ExchangeDbContext _dbContext;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly ILogger<ApiKeyService> _logger;

        public ApiKeyService(ExchangeDbContext dbContext, AuthenticationSettings authenticationSettings,
            ILogger<ApiKeyService> logger)
        {
            _dbContext = dbContext;
            _authenticationSettings = authenticationSettings;
            _logger = logger;
        }

        public async Task<string> GenerateApiKeyAsync()
        {
            _logger.LogWarning("Generating new Api Key.");
            // Every time someone generate api key, method also check if there are any expired keys in db, and removes them
            var expiredKeys = await _dbContext.ApiKeys.Where(x => x.ExpireDate < DateTime.Now).ToListAsync();
            if (expiredKeys.Any())
            {
                _logger.LogWarning("Removing expired ApiKeys from data base");
                _dbContext.RemoveRange(expiredKeys);
                await _dbContext.SaveChangesAsync();
            }

            var apiKeyEntity = new ApiKey
            {
                Id = Guid.NewGuid(),
                ExpireDate = DateTime.Now.AddDays(15),
            };
            await _dbContext.ApiKeys.AddAsync(apiKeyEntity);

            await _dbContext.SaveChangesAsync();

            var subject = _dbContext.ApiKeys.FirstOrDefaultAsync(x => x.Id == apiKeyEntity.Id);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, subject.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = apiKeyEntity.ExpireDate;

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }
    }
}