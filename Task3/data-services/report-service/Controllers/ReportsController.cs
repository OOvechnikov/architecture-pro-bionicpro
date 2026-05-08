using Microsoft.AspNetCore.Mvc;
using ReportService.Services;

namespace ReportService.Controllers
{
	[ApiController]
	[Route("api/reports")]
	public class ReportsController : ControllerBase
	{
		public ReportsController(ClientReportService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<IActionResult> GetReportAsync([FromQuery] string clientId)
		{
			var report = await _service.GetReportAsync(clientId);
			return Ok(report);
		}

		private readonly ClientReportService _service;
	}
}