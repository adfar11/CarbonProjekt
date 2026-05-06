using Microsoft.EntityFrameworkCore; // Für DbSet
using System.Threading;              // Für CancellationToken
using System.Threading.Tasks; 
using Domain.Entities;

namespace Application.CarbonReports.Interfaces
{
    public interface IApplicationDbContext
    {
      
        DbSet<CarbonReport> CarbonReports { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}