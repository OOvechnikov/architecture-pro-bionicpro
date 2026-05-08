using Octonica.ClickHouseClient;
using ReportService.Models;

namespace ReportService.Services
{
	public class ClickhouseRepository
	{
		public ClickhouseRepository(
			IConfiguration configuration)
		{
			_connectionString = configuration["ClickHouse.ConnectionString"];
		}

		public async Task<ClientTelemetryReport> GetClientReportAsync(string clientId)
		{
			await using var connection = new ClickHouseConnection(_connectionString);
			await connection.OpenAsync();

			var command = connection.CreateCommand();
			command.CommandText = @"
				SELECT 
					client_id,
					full_name,
					city,
					total_steps,
					avg_battery,
					last_activity,
					calculated_at
				FROM dm_clients_telemetry FINAL
				WHERE full_name = @full_name
				";

			var parameter = command.CreateParameter();
			parameter.ParameterName = "full_name";
			parameter.Value = clientId;
			command.Parameters.Add(parameter);

			await using var reader = await command.ExecuteReaderAsync();

			if (!await reader.ReadAsync())
				return new();

			var result = new ClientTelemetryReport
			{
				ClientId = reader.GetFieldValue<ulong>(0),
				FullName = reader.GetString(1),
				City = reader.GetString(2),
				TotalSteps = reader.GetFieldValue<ulong>(3),
				AvgBattery = reader.GetDouble(4),
				LastActivity = reader.GetDateTime(5),
				CalculatedAt = reader.GetDateTime(6)
			};

			return result;
		}

		private readonly string _connectionString;
	}
}