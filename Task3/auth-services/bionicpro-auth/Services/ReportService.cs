using BionicProAuth.Models;

namespace BionicProAuth.Services
{
	public class ReportService
	{
		public ReportService(
			HttpClient http,
			IConfiguration cfg)
		{
			_http = http;
			_cfg = cfg;
		}

		public async Task<ClientTelemetryReport> GetUserReportAsync(string userId)
		{
			var uri = $"{ReportsUrl}/api/reports?clientId={userId}";
			Console.WriteLine($"Get report for {uri}");
			var report = await _http.GetFromJsonAsync<ClientTelemetryReport>(uri);
			return report;
		}

		private string ReportsUrl => _cfg["Reports.Url"];

		private readonly IConfiguration _cfg;
		private readonly HttpClient _http;
	}
}