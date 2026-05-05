
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

        [HttpGet("{id}")]
        public async Task<ActionResult<CarbonReportDto>> GetDetails(Guid id)
        {
            var carbonReport = mediator.Send(new GetCarbonReportDetails.Query {Id = id});
            if(carbonReport == null ) throw new Exception("carbon report not found");
            return Ok(carbonReport);
        }
    }
}