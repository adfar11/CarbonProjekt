namespace Domain.Entities;

public class CarbonReport
{
         // Primärschlüssel für die Datenbank (Persistence Schicht nutzt diesen später)
        //public int Id { get; set; } public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Id {get; set;} =  Guid.NewGuid().ToString();
        // Basis-Informationen
        public string CompanyName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Eingabewerte (Rohdaten für die Berechnung)
        public double DieselLiters { get; set; }
        public double NaturalGasKWh { get; set; }
        public double ElectricityKWh { get; set; }

        // Berechnete Ergebnisse (in kg CO2e)
        // Diese werden in der Application-Schicht befüllt
        public double Co2Scope1 { get; set; }
        public double Co2Scope2 { get; set; }
        
        // Logik innerhalb der Domain: Berechneter Gesamt-Fußabdruck
        public double TotalCo2 => Co2Scope1 + Co2Scope2;

        // Metadaten
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}