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
using TsrmWebApi.Services;
using Newtonsoft.Json.Linq;

namespace TsrmWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketVisitController : ControllerBase
    {
        private readonly IMarketVisitService _tsrmService;
        private readonly ILogger<MarketVisitController> _logger;

        public MarketVisitController(IMarketVisitService tsrmconnService, ILogger<MarketVisitController> logger)
        {
            _tsrmService = tsrmconnService;
            _logger = logger;
        }

       

        [HttpPost("InsertSurveyVisit")]
        public async Task<IActionResult> InsertSurveyVisit([FromBody] MarketVisitRequest model)
        {
            if (model == null || model.Details.Count == 0)
                return BadRequest("Invalid request.");

            await _tsrmService.InsertSurveyVisitAsync(model);
            return Ok(new { message = "Market visit details inserted successfully." });
        }


        [HttpPost("InsertMarketVisit")]
        [Authorize]
        public async Task<IActionResult> InsertMarketVisit([FromBody] SurveyVisitRequest request)
        {
            if (request == null || request.Details == null)
                return BadRequest(new { message = "Request or Details cannot be null." });

            var result = await _tsrmService.InsertMarketVisitAsync(request);

            if (result)
                return Ok(new { message = "Inserted successfully." });
            else
                return StatusCode(500, new { message = "Insertion failed." });
        }
    }
}
