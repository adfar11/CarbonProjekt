using System;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.CarbonReports.Queries
{
    public class GetCarbonReportList
    {
        //public class Query : IRequest<List<CarbonReport>>{};
        public class Query : IRequest<List<CarbonReportDto>>{};

        public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Query, List<CarbonReportDto>>
        {
            /*      public async Task<List<CarbonReport>> Handle(Query request, CancellationToken cancellationToken)
                 {
                     return await context.CarbonReports.ToListAsync(cancellationToken);
                 } */
            public async Task<List<CarbonReportDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var reports = await context.CarbonReports.ToListAsync(cancellationToken);
                return mapper.Map<List<CarbonReportDto>>(reports);
            }
        }
    }
}