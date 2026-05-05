using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.CarbonReports.Queries
{
    public class CarbonReportDto
    {
        public required string Id { get; set; } 
               
        public string CompanyName { get; set; } =string.Empty;
        public required double TotalCo2 { get; set; }
        public required DateTime CreatedAt { get; set; }
        
        // Optional: Ein berechnetes Feld oder Status, das nicht in der DB steht
        public string Summary => $"{CompanyName} verursachte {TotalCo2}kg CO2.";
    }
}