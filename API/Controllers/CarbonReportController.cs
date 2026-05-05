
using Application.CarbonReports.Commands;
using Application.CarbonReports.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarbonReportController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<CarbonReportDto>> Create(CreateCarbonReport.Command command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<CarbonReportDto>> GetList()
        {
            var results = await mediator.Send(new GetCarbonReportList.Query());
            return Ok(results);
        }
    }
}