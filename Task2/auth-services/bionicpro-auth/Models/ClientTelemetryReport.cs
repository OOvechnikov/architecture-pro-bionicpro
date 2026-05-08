namespace BionicProAuth.Models
{
	public class ClientTelemetryReport
	{
		public ulong ClientId { get; set; }

		public string FullName { get; set; } = default!;

		public string City { get; set; } = default!;

		public ulong TotalSteps { get; set; }

		public double AvgBattery { get; set; }

		public DateTime LastActivity { get; set; }

		public DateTime CalculatedAt { get; set; }
	}
}