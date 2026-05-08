namespace BionicProAuth.Models
{
	// Models/SessionData.cs
	public class SessionData
	{
		public string UserId { get; set; } = default!;

		public string AccessToken { get; set; } = default!;

		public string RefreshToken { get; set; } = default!;

		public DateTime AccessTokenExpiresAt { get; set; }

		public string YandexAccessToken { get; set; } = default!;
	}
}