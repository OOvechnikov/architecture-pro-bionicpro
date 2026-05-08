import React, { useEffect, useState } from 'react';
import { checkSession } from './components/auth';
import ReportPage from './components/ReportPage';
import LoginPage from './components/LoginPage';

const App: React.FC = () => {
  const [authorized, setAuthorized] = useState<boolean | null>(null);

  useEffect(() => {
    checkSession().then(ok => {
      setAuthorized(ok);
    });
  }, []);

  if (authorized === null) return <div>Loading...</div>;

  if (!authorized) return <LoginPage />;

  return <ReportPage />;
};

export default App;