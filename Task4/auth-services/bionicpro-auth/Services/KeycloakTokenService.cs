using BionicProAuth.Models;
using Duende.IdentityModel.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BionicProAuth.Services
{
	public class KeycloakTokenService : ITokenService
	{
		private readonly HttpClient _http;
		private readonly IConfiguration _cfg;

		public KeycloakTokenService(HttpClient http, IConfiguration cfg)
		{
			_http = http;
			_cfg = cfg;
		}

		public async Task<SessionData> ExchangeCodeAsync(string code, string redirectUri)
		{
			Console.WriteLine("ExchangeCodeAsync");
			var token = await _http.RequestAuthorizationCodeTokenAsync(
				new AuthorizationCodeTokenRequest
				{
					Address = TokenEndpoint(),
					ClientId = ClientId(),
					ClientSecret = ClientSecret(),
					Code = code,
					RedirectUri = redirectUri
				});

			if (token.IsError)
				throw new Exception(token.Error);

			return BuildSession(token);
		}

		public async Task<SessionData> RefreshAsync(SessionData session)
		{
			var token = await _http.RequestRefreshTokenAsync(
				new RefreshTokenRequest
				{
					Address = TokenEndpoint(),
					ClientId = ClientId(),
					ClientSecret = ClientSecret(),
					RefreshToken = session.RefreshToken
				});

			return BuildSession(token);
		}

		private SessionData BuildSession(TokenResponse token)
		{
			Console.WriteLine("BuildSession");
			var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.AccessToken);

			return new SessionData
			{
				UserId = jwt.Subject!,
				UserEmail = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value,
				AccessToken = token.AccessToken!,
				RefreshToken = token.RefreshToken!,
				AccessTokenExpiresAt = DateTime.UtcNow.AddSeconds(token.ExpiresIn)
			};
		}

		private string TokenEndpoint()
			=> $"{_cfg["Keycloak:InternalUrl"]}/realms/{_cfg["Keycloak:Realm"]}/protocol/openid-connect/token";

		private string ClientId() => _cfg["Keycloak:ClientId"]!;

		private string ClientSecret() => _cfg["Keycloak:ClientSecret"]!;
	}
}