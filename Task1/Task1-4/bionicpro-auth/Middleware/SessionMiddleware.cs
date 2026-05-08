using BionicProAuth.Services;

namespace BionicProAuth.Middleware
{
	public class SessionMiddleware
	{
		private readonly RequestDelegate _next;

		public SessionMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(
			HttpContext ctx,
			ISessionStore store,
			ITokenService tokens)
		{
			Console.WriteLine($"SessionMiddleware: Path={ctx.Request.Path}, Cookie={ctx.Request.Cookies["BIONICPRO_SESSION"]}");

			if (ctx.Request.Path.StartsWithSegments("/auth/login") ||
				ctx.Request.Path.StartsWithSegments("/auth/callback") ||
				ctx.Request.Path.StartsWithSegments("/auth/logout"))
			{
				await _next(ctx);
				return;
			}

            Console.WriteLine("new session");

			if (!ctx.Request.Cookies.TryGetValue("BIONICPRO_SESSION", out var sessionId))
			{
				ctx.Response.StatusCode = 401;
				return;
			}

			var session = await store.GetAsync(sessionId);
			if (session == null)
			{
				ctx.Response.StatusCode = 401;
				return;
			}

			var rotated = false;

			if (session.AccessTokenExpiresAt <= DateTime.UtcNow)
			{
				session = await tokens.RefreshAsync(session);
				rotated = true;
			}

			if (rotated)
			{
				var sessionTtlStr = Environment.GetEnvironmentVariable("SESSION_TTL");
				var ttl = string.IsNullOrEmpty(sessionTtlStr)
					? TimeSpan.FromMinutes(60)
					: TimeSpan.Parse(sessionTtlStr);

				var maxAgeStr = Environment.GetEnvironmentVariable("COOKIE_MAX_AGE");
				var maxAge = string.IsNullOrEmpty(maxAgeStr)
					? TimeSpan.FromMinutes(60)
					: TimeSpan.Parse(maxAgeStr);

				var newSessionId = Guid.NewGuid().ToString("N");

				await store.DeleteAsync(sessionId);
				await store.StoreAsync(newSessionId, session, ttl);

				ctx.Response.Cookies.Append(
					"BIONICPRO_SESSION",
					newSessionId,
					new CookieOptions
					{
						HttpOnly = true,
						Secure = false, // true в prod
						SameSite = SameSiteMode.Strict,
						MaxAge = maxAge
					});
			}

			ctx.Items["AccessToken"] = session.AccessToken;
            Console.WriteLine($"session.AccessToken {session.AccessToken}");
			await _next(ctx);
		}
	}
}