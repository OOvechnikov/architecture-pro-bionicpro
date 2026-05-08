using Microsoft.AspNetCore.Mvc;
using ReportService.Services;

namespace ReportService.Controllers
{
	[ApiController]
	[Route("api/reports")]
	public class ReportsController : ControllerBase
	{
		public ReportsController(ClickhouseRepository repository)
		{
			_repository = repository;
		}

		[HttpGet]
		public async Task<IActionResult> GetReportAsync([FromQuery] string clientId)
		{
			Console.WriteLine("get report for " + clientId);
			var report = await _repository.GetClientReportAsync(clientId);

			return Ok(report);
		}

		private readonly ClickhouseRepository _repository;
	}
}