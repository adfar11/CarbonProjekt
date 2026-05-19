import type { CarbonReport } from "../features/carbon-reports/schemas/schema";
import { Ca } from "zod/v4/locales";
import CarbonReportList from "../features/carbon-reports/components/CarbonReportList";
export default function Main({
  carbonReports,
}: {
  carbonReports: CarbonReport[];
}) {
  console.log(carbonReports);

  return (
    <div>
      <h1>Carbon Reports</h1>
      <CarbonReportList />
    </div>
  );
}
