using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.CarbonReports.Queries
{
    public class GetCarbonReportDetails
    {
        public class Query : IRequest<CarbonReport>
        {
            public required string Id {get; set;}
        }

        public class Handler(AppDbContext context) : IRequestHandler<Query, CarbonReport>
        {
            public async Task<CarbonReport> Handle(Query request, CancellationToken cancellationToken)
            {
                var carbonReport = await context.CarbonReports.FindAsync([request.Id], cancellationToken);
                if(carbonReport == null) throw new Exception("Carbon Report not found"); 
                return carbonReport;
            }
        }
    }
}