using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OneApp_minimalApi.Interfaces;

namespace OneApp_minimalApi.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ITokenService> _logger;
    private readonly ILocalVaultService _localVaultService;
    private const string JWTSecretName = "JWT:SECRET";

    public TokenService(IConfiguration configuration, ILogger<ITokenService> logger,
        ILocalVaultService localVaultService)
    {
        _logger = logger;
        _localVaultService = localVaultService;
        _configuration = configuration;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var result = _localVaultService.GetSecret(JWTSecretName);

        string key = result.Result.Data.Value;

        if (result.Result.Data == null)
        {
            _logger.LogWarning($"No key found in LocalVault for {JWTSecretName}");
            return "No key found";
        }

        // Create a symmetric security key using the secret key from the configuration.
        SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _configuration["JWT:ValidIssuer"],
            Audience = _configuration["JWT:ValidAudience"],
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddMinutes(
                Convert.ToDouble(_configuration.GetSection("TokenExpirationMinutes").Value)),
            SigningCredentials = new SigningCredentials
                (authSigningKey, SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        // Create a 32-byte array to hold cryptographically secure random bytes
        var randomNumber = new byte[32];

        // Use a cryptographically secure random number generator 
        // to fill the byte array with random values
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);

        // Convert the random bytes to a base64 encoded string 
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
        // Create a symmetric security key using the secret key from the configuration.
        SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(_configuration[JWTSecretName]));

        // Define the token validation parameters used to validate the token.
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = _configuration["JWT:ValidAudience"],
            ValidIssuer = _configuration["JWT:ValidIssuer"],
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = authSigningKey
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        // Validate the token and extract the claims principal and the security token.
        var principal =
            tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

        // Cast the security token to a JwtSecurityToken for further validation.
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        // Ensure the token is a valid JWT and uses the HmacSha256 signing algorithm.
        // If no throw new SecurityTokenException
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals
                (SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        // return the principal
        return principal;
    }
}