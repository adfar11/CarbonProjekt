using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.CarbonReports.Commands
{
    public class EditCarbonReport
    {
        public class Command : IRequest
        {
            public required CarbonReport CarbonReport {get; set;}
        }

        public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Command>
        {
            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var carbonReport = await context.CarbonReports.FindAsync([request.CarbonReport.Id], cancellationToken)
                    ?? throw new Exception("Cannot find carbon report");
                
                mapper.Map(request.CarbonReport, carbonReport);
                await context.SaveChangesAsync(cancellationToken);
                
            }
        }
    }
}