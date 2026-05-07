using BionicProAuth.Services;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using ISessionStore = BionicProAuth.Services.ISessionStore;

namespace BionicProAuth.Controllers
{
	// Controllers/AuthController.cs
	[ApiController]
	[Route("auth")]
	public class AuthController : ControllerBase
	{
		private readonly ITokenService _tokens;
		private readonly ISessionStore _sessions;

		public AuthController(ITokenService tokens, ISessionStore sessions, IConfiguration cfg)
		{
			_tokens = tokens;
			_sessions = sessions;
			_cfg = cfg;
		}

		[HttpGet("login")]
		public IActionResult Login()
		{
			Console.WriteLine("login requested");
			var redirectUri = $"{Request.Scheme}://{Request.Host}/auth/callback";

			var url =
				$"{KcUrl}/realms/{Realm}/protocol/openid-connect/auth" +
				$"?client_id={ClientId}" +
				$"&response_type=code" +
				$"&scope=openid" +
				$"&redirect_uri={Uri.EscapeDataString(redirectUri)}";

			return Redirect(url);
		}

		[HttpGet("callback")]
		public async Task<IActionResult> Callback(
			[FromQuery] string code)
		{
			Console.WriteLine("callback requested");
			var sessionTtl = Environment.GetEnvironmentVariable("SESSION_TTL");
			var ttl = string.IsNullOrEmpty(sessionTtl) ? TimeSpan.FromMinutes(60) : TimeSpan.Parse(sessionTtl);

			var maxAgeStr = Environment.GetEnvironmentVariable("COOKIE_MAX_AGE");
			var maxAge = string.IsNullOrEmpty(maxAgeStr) ? TimeSpan.FromMinutes(60) : TimeSpan.Parse(maxAgeStr);

			var redirectUri = $"{Request.Scheme}://{Request.Host}/auth/callback";

			var session = await _tokens.ExchangeCodeAsync(code, redirectUri);

			var sessionId = Guid.NewGuid().ToString("N");

			await _sessions.StoreAsync(sessionId, session, ttl);

			Response.Cookies.Append("BIONICPRO_SESSION", sessionId, CookieOptions(maxAge));

			return Redirect(FrontendUrl);
		}

		[HttpGet("me")]
		public IActionResult Me()
		{
			Console.WriteLine("me requested");
			// если мы тут — значит SessionMiddleware пропустил
			return Ok(new { authenticated = true });
		}

		private static CookieOptions CookieOptions(TimeSpan maxAge) => new()
		{
			HttpOnly = true,
			Secure = false,
			SameSite = SameSiteMode.Strict,
			MaxAge = maxAge
		};

		private string KcUrl => _cfg["Keycloak:Url"];

		private string Realm => _cfg["Keycloak:Realm"];

		private string ClientId => _cfg["Keycloak:ClientId"];

		private string FrontendUrl => _cfg["Frontend.Url"];

		private readonly IConfiguration _cfg;
	}

	public record LoginRequest(string Code, string CodeVerifier, string RedirectUri);
}