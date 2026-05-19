import "./App.css";
import CarbonReportDetail from "./features/carbon-reports/components/CarbonReportDetail";
import CarbonReportList from "./features/carbon-reports/components/CarbonReportList";
import MainLayout from "./features/carbon-reports/components/MainLayout";
import { BrowserRouter, Routes, Route } from "react-router-dom";
function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<MainLayout />} />
        <Route path="/list" element={<CarbonReportList />} />
        <Route path="/list/:id" element={<CarbonReportDetail />} />
        <Route path="/about" element={<div>About Page</div>} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
