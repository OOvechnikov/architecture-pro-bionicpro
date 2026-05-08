import React, { useState } from 'react';

interface ReportData {
  clientId: number;
  fullName: string;
  city: string;
  totalSteps: number;
  avgBattery: number;
  lastActivity: string;
  calculatedAt: string;
}

const ReportPage: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [reportData, setReportData] = useState<ReportData | null>(null);
  const API_URL = process.env.REACT_APP_API_URL;

  const downloadReport = async () => {
    try {
      setLoading(true);
      setError(null);
      setReportData(null); // сбрасываем предыдущие данные при новом запросе

      const response = await fetch(`${API_URL}/api/v1/reports/telemetry`, {
        credentials: 'include'
      });

      if (!response.ok) {
        throw new Error('Failed to download report');
      }

      // Парсим JSON-ответ
      const data: ReportData = await response.json();
      setReportData(data);

    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100">
      <div className="p-8 bg-white rounded-lg shadow-md max-w-2xl w-full">
        <h1 className="text-2xl font-bold mb-6">Usage Reports</h1>

        <button
          onClick={downloadReport}
          disabled={loading}
          className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {loading ? 'Generating Report...' : 'Download Report'}
        </button>

        {error && (
          <div className="mt-4 p-4 bg-red-100 text-red-700 rounded">
            {error}
          </div>
        )}

        {reportData && (
          <div className="mt-6 p-6 bg-green-50 rounded-lg border border-green-200">
            <h2 className="text-xl font-semibold mb-4 text-green-800">Report Data</h2>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <div><span className="font-medium">Client ID:</span> {reportData.clientId}</div>
                <div><span className="font-medium">Full Name:</span> {reportData.fullName}</div>
                <div><span className="font-medium">City:</span> {reportData.city}</div>
              </div>
              <div className="space-y-2">
                <div><span className="font-medium">Total Steps:</span> {reportData.totalSteps}</div>
                <div><span className="font-medium">Average Battery:</span> {reportData.avgBattery.toFixed(2)}%</div>
                <div><span className="font-medium">Last Activity:</span> {new Date(reportData.lastActivity).toLocaleString()}</div>
                <div><span className="font-medium">Calculated At:</span> {new Date(reportData.calculatedAt).toLocaleString()}</div>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default ReportPage;
