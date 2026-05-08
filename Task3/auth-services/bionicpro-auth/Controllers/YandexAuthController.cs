using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using BionicProAuth.Services;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Session;
using BionicProAuth.Models;
using ISessionStore = BionicProAuth.Services.ISessionStore;

namespace BionicProAuth.Controllers;

[ApiController]
[Route("auth/yandex")]
public class YandexAuthController : ControllerBase
{
	private readonly YandexOAuthOptions _options;
	private readonly IHttpClientFactory _httpFactory;
	private readonly IUserRepository _users;
	private readonly ISessionStore _sessions;
	private readonly IConfiguration _cfg;

	private string FrontendUrl => _cfg["Frontend.Url"];

	public YandexAuthController(
		IOptions<YandexOAuthOptions> options,
		IHttpClientFactory httpFactory,
		IUserRepository users,
		ISessionStore sessions,
		IConfiguration cfg)
	{
		_options = options.Value;
		_httpFactory = httpFactory;
		_users = users;
		_sessions = sessions;
		_cfg = cfg;
	}

	// 🔷 1. Старт логина
	[HttpGet("login")]
	public IActionResult Login()
	{
		var state = Guid.NewGuid().ToString("N");

		var url =
			"https://oauth.yandex.ru/authorize" +
			$"?response_type=code" +
			$"&client_id={_options.ClientId}" +
			$"&redirect_uri={Uri.EscapeDataString(_options.RedirectUri)}" +
			$"&scope=login:email login:info" +
			$"&state={state}";

		return Redirect(url);
	}

	// 🔷 2. Callback
	[HttpGet("callback")]
	public async Task<IActionResult> Callback(string code)
	{
		var http = _httpFactory.CreateClient();

		// 2.1 Получаем access_token
		var tokenResponse = await http.PostAsync(
			"https://oauth.yandex.ru/token",
			new FormUrlEncodedContent(new Dictionary<string, string>
			{
				["grant_type"] = "authorization_code",
				["code"] = code,
				["client_id"] = _options.ClientId,
				["client_secret"] = _options.ClientSecret
			}));

		if (!tokenResponse.IsSuccessStatusCode)
			return StatusCode(500, "Yandex token error");

		var token = await tokenResponse.Content
			.ReadFromJsonAsync<YandexTokenResponse>();

		// 2.2 Получаем профиль
		Console.WriteLine("2.2 Получаем профиль" + token!.access_token);
		http.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("OAuth", token!.access_token);

		var profile = await http.GetFromJsonAsync<YandexProfile>(
			"https://login.yandex.ru/info");

		if (profile == null)
			return StatusCode(500, "Profile error");

		Console.WriteLine("2.3 Upsert пользователя в БД" + profile.id);
		var user = await _users.FindByYandexIdAsync(profile.id);

		if (user == null)
		{
			user = new User
			{
				Id = Guid.NewGuid(),
				YandexId = profile.id,
				Email = profile.default_email,
				Name = profile.real_name
			};

			await _users.CreateAsync(user);
		}

		// 2.4 Создание серверной сессии
		Console.WriteLine("2.4 Создание серверной сессии " + user.Id.ToString());
		var session = new SessionData
		{
			UserId = user.Id.ToString(),
			UserEmail = user.Email,
			YandexAccessToken = token.access_token,
			AccessTokenExpiresAt = DateTime.UtcNow.AddSeconds(token.expires_in)
		};

		var sessionId = Guid.NewGuid().ToString("N");

		await _sessions.StoreAsync(
			sessionId,
			session,
			TimeSpan.FromHours(1));

		Console.WriteLine("2.5 BIONICPRO_SESSION append ");
		Response.Cookies.Append(
			"BIONICPRO_SESSION",
			sessionId,
			new CookieOptions
			{
				HttpOnly = true,
				Secure = false, // true в проде
				SameSite = SameSiteMode.Strict,
				MaxAge = TimeSpan.FromHours(1)
			});

		return Redirect(FrontendUrl);
	}
}