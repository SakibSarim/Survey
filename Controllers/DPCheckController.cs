using TsrmWebApi.Models.PresentationModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TsrmWebApi.IServices;
using TsrmWebApi.Models.DataModels;
using System.Text.Json;
using System.Data;
using ClosedXML.Excel;

namespace TsrmWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DPCheckController : ControllerBase
    {

        private readonly IDPCheckService _tsrmService;
        private readonly ILogger<DPCheckController> _logger;

        public DPCheckController(IDPCheckService tsrmconnService, ILogger<DPCheckController> logger)
        {
            _tsrmService = tsrmconnService;
            _logger = logger;
        }

        [HttpGet("GetDPCheckDropdown")]
        [Authorize]
        public async Task<ActionResult<ResponseDefault>> GetDataLoad(int loginId, int typeId, decimal questionid, decimal selectedid)
        {
            try
            {
                var assetTypes = await _tsrmService.GetDataLoadDPCheck(loginId, typeId, questionid, selectedid);

                if (assetTypes == null)
                {
                   
                    return NotFound(new { HttpStatusCode = HttpStatusCode.OK, Success = false, Message = "No data found." });
                }

                
                return Ok(new { HttpStatusCode = HttpStatusCode.OK, Success = true, Data = assetTypes });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ecom types");

              
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Success = false, Message = "An error occurred while fetching ecom types." });
            }
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitSurvey([FromBody] SurveyAnswerDto input)
        {
            try
            {
                var result = await _tsrmService.SubmitSurvey(input);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpGet("responses")]
        [Authorize]
        public async Task<ActionResult<List<QuestionResponseJson>>> GetResponses([FromQuery] int loginId, [FromQuery] int typeId,[FromQuery] int questionId,[FromQuery] int sectionId)
        {
            var result = await _tsrmService.GetQuestionResponsesAsync(loginId, typeId, questionId, sectionId);
            if (result == null || result.Count == 0)
            {
                return NotFound("No data found");
            }
            return Ok(result);
        }

        [HttpPost("insertdppoint")]
        [Authorize]
        public async Task<IActionResult> Insert([FromBody] DistributorVisitRequestCheck request)
        {
            if (request == null || request.Details == null)
                return BadRequest("Invalid request data.");

            // Serialize dictionary back to JSON string
            string detailsJson = JsonSerializer.Serialize(request.Details);

            try
            {
                await _tsrmService.InsertVisitAsync(request.Enroll, detailsJson);
                return Ok(new { message = "Insert successful" });
            }
            catch (Exception ex)
            {
                // Log error as needed
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("GetVisitReport")]
        [Authorize]
        public async Task<IActionResult> GetVisitReport([FromQuery] DateTime fromDate,[FromQuery] DateTime toDate, [FromQuery]  decimal enroll,[FromQuery] int typeId)
        {
            try
            {
                var data = await _tsrmService.GetVisitReportAsync(fromDate, toDate, enroll, typeId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetVisitJson")]
        public async Task<IActionResult> GetVisitJson(int enrollId, string fromDate, string toDate)
        {
            if (!DateTime.TryParse(fromDate, out var fromDt) || !DateTime.TryParse(toDate, out var toDt))
                return BadRequest("Invalid date format.");

            var jsonResult = await _tsrmService.GetVisitJsonAsync(enrollId, fromDt, toDt);

            // Return raw JSON
            return Content(jsonResult, "application/json");
        }


        [HttpGet("GetConditionalReport")]
        [Authorize]
        public async Task<IActionResult> GetJson([FromQuery] DateTime from, [FromQuery] DateTime to,decimal? zoneid, decimal? divisionid, decimal? regionid , decimal? teritoryid)
        {
            DataTable dt = await _tsrmService.GetVisitReportConditionAsync(from, to, zoneid, divisionid, regionid, teritoryid);
            return Ok(dt);
        }

        [HttpGet("excel")]
        [Authorize]
        public async Task<IActionResult> GetExcel([FromQuery] DateTime from, [FromQuery] DateTime to, decimal? zoneid, decimal? divisionid, decimal? regionid, decimal? teritoryid)
        {
            // Call async service method
            DataTable dt = await _tsrmService.GetVisitReportConditionAsync(from, to, zoneid, divisionid, regionid, teritoryid);

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


        [HttpGet("GetReport")]
        [Authorize]
        public async Task<IActionResult> GetTableReport([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            DataTable dt = await _tsrmService.GetVisitReportCondition(from, to);
            return Ok(dt);
        }

        [HttpGet("exceldownload")]
        [Authorize]
        public async Task<IActionResult> GetExceldownload([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            // Call async service method
            DataTable dt = await _tsrmService.GetVisitReportCondition(from, to);

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
