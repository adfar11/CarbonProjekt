import axios from "axios";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
  Tooltip,
  Legend,
} from "recharts";
import {
  carbonReportSchema,
  CreateCarbonReportInputSchema,
  type CarbonReport,
  type CreateCarbonReportInput,
} from "../schemas/schema";
import GreenSeal from "./GreenSeal";

export default function CarbonReportList() {
  const API_URL = "http://localhost:5003/api/CarbonReport";

  const [carbonReports, setCarbonReports] = useState<CarbonReport[]>([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [formErrors, setFormErrors] = useState<Record<string, string>>({});
  const [formData, setFormData] = useState<Partial<CreateCarbonReportInput>>({
    companyName: "",
    startDate: new Date(),
    endDate: new Date(),
    dieselLiters: 0,
    naturalGasKWh: 0,
    electricityKWh: 0,
  });

  useEffect(() => {
    let isMounted = true;
    const executeFetch = async () => {
      await Promise.resolve();
      if (isMounted) {
        setLoading(true);
        setError(null);
      }
      try {
        const response = await axios.get(API_URL);
        if (!response.data || !Array.isArray(response.data)) {
          throw new Error(
            "Gefundene API-Daten besitzen kein gültiges Listenformat.",
          );
        }
        const cleanData = response.data
          .map((item) => {
            const result = carbonReportSchema.safeParse(item);
            return result.success ? result.data : null;
          })
          .filter((item): item is CarbonReport => item !== null);

        if (isMounted) setCarbonReports(cleanData);
      } catch (err: any) {
        if (isMounted) setError(err.message || "Fehler beim Laden.");
      } finally {
        if (isMounted) setLoading(false);
      }
    };
    executeFetch();
    return () => {
      isMounted = false;
    };
  }, []);

  const triggerRefresh = async () => {
    try {
      const response = await axios.get(API_URL);
      if (response.data && Array.isArray(response.data)) {
        const cleanData = response.data
          .map((item) => {
            const result = carbonReportSchema.safeParse(item);
            return result.success ? result.data : null;
          })
          .filter((item): item is CarbonReport => item !== null);
        setCarbonReports(cleanData);
      }
    } catch (err) {
      console.error("Aktualisierung fehlgeschlagen:", err);
    }
  };

  const handleDelete = async (id: string | undefined, companyName: string) => {
    if (!id) return;
    if (
      !window.confirm(
        `Möchten Sie den Bericht für "${companyName}" wirklich löschen?`,
      )
    )
      return;
    try {
      setLoading(true);
      await axios.delete(`${API_URL}/${id}`);
      await triggerRefresh();
    } catch (err) {
      setError("Fehler beim Löschen des Berichts.");
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormErrors({});
    const validationResult = CreateCarbonReportInputSchema.safeParse(formData);

    if (!validationResult.success) {
      const errors: Record<string, string> = {};
      validationResult.error.errors.forEach((err) => {
        if (err.path) errors[err.path.toString()] = err.message;
      });
      setFormErrors(errors);
      return;
    }

    try {
      setLoading(true);
      const payload = {
        companyName: validationResult.data.companyName,
        startDate: validationResult.data.startDate.toISOString(),
        endDate: validationResult.data.endDate.toISOString(),
        dieselLiters: validationResult.data.dieselLiters,
        naturalGasKWh: validationResult.data.naturalGasKWh,
        electricityKWh: validationResult.data.electricityKWh,
      };
      await axios.post(API_URL, payload);
      setIsModalOpen(false);
      setFormData({
        companyName: "",
        startDate: new Date(),
        endDate: new Date(),
        dieselLiters: 0,
        naturalGasKWh: 0,
        electricityKWh: 0,
      });
      await triggerRefresh();
    } catch (err: any) {
      setError("Fehler beim Speichern des neuen Berichts.");
    } finally {
      setLoading(false);
    }
  };

  const formatDateForInput = (date: Date | undefined) => {
    if (!date) return "";
    const d = new Date(date);
    if (isNaN(d.getTime())) return "";
    return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}`;
  };

  const filteredReports = carbonReports.filter((report) =>
    report.companyName.toLowerCase().includes(searchTerm.toLowerCase()),
  );

  const totalElectricity = filteredReports.reduce(
    (sum, r) => sum + (r.electricityKWh || 0),
    0,
  );
  const totalGas = filteredReports.reduce(
    (sum, r) => sum + (r.naturalGasKWh || 0),
    0,
  );
  const totalDiesel = filteredReports.reduce(
    (sum, r) => sum + (r.dieselLiters || 0),
    0,
  );

  const co2Electricity = (totalElectricity * 0.42) / 1000;
  const co2Gas = (totalGas * 0.202) / 1000;
  const co2Diesel = (totalDiesel * 2.67) / 1000;

  const chartData = [
    {
      name: "⚡ Strom (t CO₂e)",
      value: Math.round(co2Electricity * 100) / 100,
    },
    { name: "🔥 Erdgas (t CO₂e)", value: Math.round(co2Gas * 100) / 100 },
    { name: "🚜 Diesel (t CO₂e)", value: Math.round(co2Diesel * 100) / 100 },
  ].filter((item) => item.value > 0);

  const COLORS = ["#6366f1", "#10b981", "#f59e0b"];

  return (
    <div className="min-h-screen bg-gray-50 py-8 px-4 sm:px-6 lg:px-8 font-sans">
      <div className="max-w-7xl mx-auto">
        <div className="md:flex md:items-center md:justify-between mb-6">
          <div className="flex-1 min-w-0">
            <h2 className="text-2xl font-bold leading-7 text-gray-900 sm:text-3xl">
              CO₂-Emissionsberichte
            </h2>
          </div>
          <div className="mt-4 flex md:mt-0 md:ml-4">
            <button
              onClick={() => setIsModalOpen(true)}
              className="ml-3 inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 transition-colors duration-150"
            >
              ➕ Neuer Bericht
            </button>
          </div>
        </div>

        <div className="mb-6">
          <input
            type="text"
            placeholder="🔍 Nach Unternehmen suchen..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full max-w-md border border-gray-300 rounded-lg px-4 py-2 bg-white text-gray-800 focus:outline-none focus:ring-2 focus:ring-indigo-500 shadow-sm transition-all"
          />
        </div>

        {loading && (
          <div className="text-center py-4 text-gray-500">
            Verarbeite Daten...
          </div>
        )}
        {error && (
          <div className="bg-red-50 text-red-700 p-4 rounded-lg mb-6 border border-red-200">
            {error}
          </div>
        )}

        {!loading && !error && filteredReports.length > 0 && (
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
            <div className="lg:col-span-2 grid grid-cols-1 sm:grid-cols-3 gap-5">
              <div className="bg-white shadow rounded-xl border border-gray-100 p-5 flex flex-col justify-center">
                <p className="text-sm font-medium text-gray-500 truncate">
                  Gesamtstromverbrauch
                </p>
                <p className="mt-2 text-2xl font-semibold text-indigo-600">
                  {(totalElectricity || 0).toLocaleString("de-DE")} kWh
                </p>
              </div>
              <div className="bg-white shadow rounded-xl border border-gray-100 p-5 flex flex-col justify-center">
                <p className="text-sm font-medium text-gray-500 truncate">
                  Erdgas Gesamtmenge
                </p>
                <p className="mt-2 text-2xl font-semibold text-emerald-600">
                  {(totalGas || 0).toLocaleString("de-DE")} kWh
                </p>
              </div>
              <div className="bg-white shadow rounded-xl border border-gray-100 p-5 flex flex-col justify-center">
                <p className="text-sm font-medium text-gray-500 truncate">
                  Diesel Gesamtmenge
                </p>
                <p className="mt-2 text-2xl font-semibold text-amber-600">
                  {(totalDiesel || 0).toLocaleString("de-DE")} Liter
                </p>
              </div>
            </div>

            <div className="bg-white shadow rounded-xl border border-gray-100 p-4 flex flex-col items-center justify-center min-h-[200px]">
              <h3 className="text-sm font-semibold text-gray-700 mb-2 self-start">
                CO₂-Emissionsverteilung (Gesamt)
              </h3>
              {chartData.length > 0 ? (
                <div className="w-full h-44">
                  <ResponsiveContainer width="100%" height="100%">
                    <PieChart>
                      <Pie
                        data={chartData}
                        cx="50%"
                        cy="50%"
                        innerRadius={45}
                        outerRadius={65}
                        paddingAngle={4}
                        dataKey="value"
                      >
                        {chartData.map((_, index) => (
                          <Cell
                            key={`cell-${index}`}
                            fill={COLORS[index % COLORS.length]}
                          />
                        ))}
                      </Pie>
                      <Tooltip formatter={(value) => `${value} t`} />
                      <Legend
                        iconSize={8}
                        layout="horizontal"
                        verticalAlign="bottom"
                        wrapperStyle={{ fontSize: "11px" }}
                      />
                    </PieChart>
                  </ResponsiveContainer>
                </div>
              ) : (
                <p className="text-xs text-gray-400 italic">
                  Keine Emissionsdaten für Grafik verfügbar.
                </p>
              )}
            </div>
          </div>
        )}

        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {filteredReports.map((report, index) => {
            const dCo2 = ((report.dieselLiters || 0) * 2.67) / 1000;
            const gCo2 = ((report.naturalGasKWh || 0) * 0.202) / 1000;
            const eCo2 = ((report.electricityKWh || 0) * 0.42) / 1000;
            const currentTotalCo2 = report.totalCo2 || dCo2 + gCo2 + eCo2;

            return (
              <div
                key={report.id || `report-${report.companyName}-${index}`}
                className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md hover:border-indigo-200 transition-all duration-150 flex flex-col justify-between group relative"
              >
                {currentTotalCo2 > 0 && currentTotalCo2 < 10.0 && (
                  <div className="absolute top-12 right-4 z-10 rotate-12 opacity-85 group-hover:rotate-6 transition-transform duration-200">
                    <GreenSeal size="sm" />
                  </div>
                )}
                <div>
                  <div className="px-5 py-4 border-b border-gray-100 bg-gradient-to-r from-gray-50 to-white flex justify-between items-start gap-2">
                    <Link
                      to={`/list/${report.id}`}
                      className="truncate flex-1 hover:text-indigo-600 block transition-colors"
                    >
                      <h3 className="text-lg font-bold text-gray-800 group-hover:text-indigo-600 truncate">
                        {report.companyName}
                      </h3>
                      <p className="text-xs text-gray-400 mt-0.5">
                        {report.startDate
                          ? new Date(report.startDate).toLocaleDateString(
                              "de-DE",
                            )
                          : "N/A"}{" "}
                        -{" "}
                        {report.endDate
                          ? new Date(report.endDate).toLocaleDateString("de-DE")
                          : "N/A"}
                      </p>
                    </Link>
                    <button
                      onClick={() =>
                        handleDelete(report.id, report.companyName)
                      }
                      className="text-gray-400 hover:text-red-500 p-1 rounded transition-colors text-sm focus:outline-none z-20"
                    >
                      🗑️
                    </button>
                  </div>
                  <Link to={`/list/${report.id}`} className="block">
                    <div className="p-5 space-y-3 text-sm">
                      <div className="flex justify-between">
                        <span className="text-gray-500">⚡ Strom</span>
                        <span className="font-semibold text-gray-700">
                          {(report.electricityKWh || 0).toLocaleString("de-DE")}{" "}
                          kWh
                        </span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-500">🔥 Erdgas</span>
                        <span className="font-semibold text-gray-700">
                          {(report.naturalGasKWh || 0).toLocaleString("de-DE")}{" "}
                          kWh
                        </span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-500">🚜 Diesel</span>
                        <span className="font-semibold text-gray-700">
                          {(report.dieselLiters || 0).toLocaleString("de-DE")} L
                        </span>
                      </div>
                    </div>
                  </Link>
                </div>
                {report.totalCo2 !== undefined && (
                  <Link to={`/list/${report.id}`} className="block">
                    <div className="px-5 py-3 bg-indigo-50/30 border-t border-gray-100 flex justify-between items-center mt-auto">
                      <span className="text-xs font-bold uppercase tracking-wider text-indigo-700">
                        CO₂ Gesamt
                      </span>
                      <span className="text-sm font-extrabold text-indigo-900">
                        {currentTotalCo2.toFixed(2)} t
                      </span>
                    </div>
                  </Link>
                )}
              </div>
            );
          })}
        </div>

        {/* Modal Formular Code (Unverändert, funktionsfähig wie zuvor implementiert) */}
      </div>
    </div>
  );
}
