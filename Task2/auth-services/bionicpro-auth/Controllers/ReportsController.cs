using BionicProAuth.Services;
using Microsoft.AspNetCore.Mvc;

namespace BionicProAuth.Controllers
{
	[ApiController]
	[Route("api/v1/reports")]
	public class ReportsController : ControllerBase
	{
		public ReportsController(
			ReportService reportService)
		{
			_reportService = reportService;
		}

		[HttpGet("telemetry")]
		public async Task<IActionResult> GetTelemetryReportAsync()
		{
			var email = HttpContext.Items["UserEmail"];

			string userId = email?.ToString()?.Split("@")?.First();

			if (userId == null)
			{
				return BadRequest($"User email is empty for {email}");
			}

			var report = await _reportService.GetUserReportAsync(userId);

			return Ok(report);
		}

		[HttpGet("user")]
		public async Task<IActionResult> GetUserInfoAsync()
		{
			var session = HttpContext.Items["Session"];

			if (session == null)
			{
				return Unauthorized($"Session not stored");
			}

			return Ok(session);
		}

		private readonly ReportService _reportService;
	}
}