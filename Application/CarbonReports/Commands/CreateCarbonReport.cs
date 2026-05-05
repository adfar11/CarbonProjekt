using System;
using Application.CarbonReports.Queries;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.CarbonReports.Commands;

    public class CreateCarbonReport
    {
        public class Command : IRequest<CarbonReportDto>
    {
        public required CarbonReport CarbonReport{get; set;}
    }

    public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Command, CarbonReportDto>
    {
  /*       public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            context.CarbonReports.Add(request.CarbonReport);
            await context.SaveChangesAsync(cancellationToken);

             return mapper.Map<CarbonReportDto>(request.CarbonReport);
        } */

        async Task<CarbonReportDto> IRequestHandler<Command, CarbonReportDto>.Handle(Command request, CancellationToken cancellationToken)
        {
            context.CarbonReports.Add(request.CarbonReport);
            await context.SaveChangesAsync(cancellationToken);

            return mapper.Map<CarbonReportDto>(request.CarbonReport);
        }
    }
    }
