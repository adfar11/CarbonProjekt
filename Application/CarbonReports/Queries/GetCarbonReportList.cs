using System;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.CarbonReports.Queries
{
    public class GetCarbonReportList
    {
        //public class Query : IRequest<List<CarbonReport>>{};
        public class Query : IRequest<List<CarbonReport>>{};

        public class Handler(AppDbContext context) : IRequestHandler<Query, List<CarbonReport>>
        {
            public async Task<List<CarbonReport>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await context.CarbonReports.ToListAsync(cancellationToken);
            }
        }
    }
}