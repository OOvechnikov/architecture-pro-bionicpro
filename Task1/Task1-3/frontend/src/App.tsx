import React, { useEffect, useState } from 'react';
import { checkSession, login } from './components/auth';
import ReportPage from './components/ReportPage';

const App: React.FC = () => {
  const [ready, setReady] = useState(false);

  useEffect(() => {
    checkSession().then(ok => {
      if (!ok) login();
      else setReady(true);
    });
  }, []);

  if (!ready) return <div>Loading...</div>;

  return <ReportPage />;
};

export default App;
