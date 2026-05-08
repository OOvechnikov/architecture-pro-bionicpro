using ReportService.Models;

namespace ReportService.Services;

public class ClientReportService
{
	private readonly ClickhouseRepository _repo;
	private readonly S3ReportCache _cache;

	public ClientReportService(
		ClickhouseRepository repo,
		S3ReportCache cache)
	{
		_repo = repo;
		_cache = cache;
	}

	public async Task<ClientTelemetryReport> GetReportAsync(string clientId)
	{
		// 1 проверяем кэш
		var cached = await _cache.GetAsync(clientId);

		if (cached != null)
		{
			Console.WriteLine($"Report from cache for {clientId}");
			return cached;
		}

		// 2 берем из clickhouse
		Console.WriteLine($"Report from ClickHouse for {clientId}");

		var report = await _repo.GetClientReportAsync(clientId);

		// 3 сохраняем в S3
		await _cache.SaveAsync(clientId, report);

		return report;
	}
}