import React from 'react';
import { loginWithKeycloak, loginWithYandex } from './auth';

const LoginPage: React.FC = () => {
  return (
    <div style={{ textAlign: 'center', marginTop: '100px' }}>
      <h2>Войти в систему</h2>

      <button onClick={loginWithKeycloak} style={{ margin: '10px' }}>
        Войти через Keycloak
      </button>

      <button onClick={loginWithYandex} style={{ margin: '10px' }}>
        Войти через Яндекс
      </button>
    </div>
  );
};

export default LoginPage;