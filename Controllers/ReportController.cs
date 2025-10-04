using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TsrmWebApi.IServices;

namespace TsrmWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _tsrmService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportService tsrmconnService, ILogger<ReportController> logger)
        {
            _tsrmService = tsrmconnService;
            _logger = logger;
        }

        [HttpGet("GetReport")]
        [Authorize]
        public async Task<IActionResult> GetTableReport([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int typeid)
        {
            DataTable dt = await _tsrmService.GetVisitReportCondition(from, to,typeid);
            return Ok(dt);
        }

        [HttpGet("exceldownload")]
        [Authorize]
        public async Task<IActionResult> GetExceldownload([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int typeid)
        {
            // Call async service method
            DataTable dt = await _tsrmService.GetVisitReportCondition(from, to, typeid);

            using var wb = new XLWorkbook();
            wb.Worksheets.Add(dt, "VisitReport");

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"VisitReport_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx"
            );
        }
    }
}
