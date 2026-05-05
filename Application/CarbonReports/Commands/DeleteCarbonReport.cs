using System;
using Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CarbonReports.Commands
{
    public class DeleteCarbonReport
    {
        public class Command : IRequest
        {
            public required string Id {get; set;}
        }

        public class Handler(AppDbContext  context) : IRequestHandler<Command>
        {
            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var carbonReport = await context.CarbonReports.FindAsync([request.Id], cancellationToken)
                    ?? throw new Exception("Cannot find carbon report");

                context.Remove(carbonReport);
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}