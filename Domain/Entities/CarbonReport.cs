namespace Domain.Entities;

public class CarbonReport 
{
         // Primärschlüssel für die Datenbank (Persistence Schicht nutzt diesen später)
        //public int Id { get; set; } public string Id { get; set; } = Guid.NewGuid().ToString();
        public Guid Id {get; set;} = Guid.NewGuid();
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
        public double Co2Scope1 { get; private set; }
        public double Co2Scope2 { get; private set; }
        
        // Logik innerhalb der Domain: Berechneter Gesamt-Fußabdruck
        public double TotalCo2 => Co2Scope1 + Co2Scope2;

        // Metadaten
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

                // Zusätzliches Feld für die Dokumentation:
        // Wer hat den Bericht erstellt?
        public string? CreatedBy { get; set; }


         // Die zentrale Methode für deine Berechnungen
        public void CalculateEmissions()
        {
        // Faktoren (können später auch übergeben werden)
            const double dieselFactor = 2.67;
            const double gasFactor = 0.202;
            const double electricityFactor = 0.420;

            this.Co2Scope1 = (this.DieselLiters * dieselFactor) + (this.NaturalGasKWh * gasFactor);
            this.Co2Scope2 = this.ElectricityKWh * electricityFactor;
        }
}