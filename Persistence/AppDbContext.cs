using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence;
public class AppDbContext(DbContextOptions options) :  DbContext(options)
{
    public required  DbSet<CarbonReport> CarbonReports {get; set;}
}
