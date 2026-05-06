using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.CarbonReports.Interfaces;
using MediatR;

namespace Application.CarbonReports.Queries;

public record GetCarbonReportPdfQuery(Guid Id) : IRequest<byte[]>;
public class GetCarbonReportPdf(IApplicationDbContext context, IPdfService pdfService) 
    : IRequestHandler<GetCarbonReportPdfQuery, byte[]>
    {
    public async Task<byte[]> Handle(GetCarbonReportPdfQuery request, CancellationToken cancellationToken)
    {
        var report = await context.CarbonReports.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (report == null) throw new Exception($"Bericht {request.Id} wurde nicht gefunden.");

        var pdf = pdfService.GenerateCarbonReportPdf(report);
        return pdf;
    }

    
}
