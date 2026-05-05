using System;
using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.CarbonReports.Commands;

    public class CreateCarbonReport
    {
        public class Commad : IRequest<string>
    {
        public required CarbonReport CarbonReport{get; set;}
    }

    public class Handler(AppDbContext context) : IRequestHandler<Commad, string>
    {
        public async Task<string> Handle(Commad request, CancellationToken cancellationToken)
        {
            context.CarbonReports.Add(request.CarbonReport);
            await context.SaveChangesAsync(cancellationToken);

            return request.CarbonReport.Id;
        }
    }
    }
