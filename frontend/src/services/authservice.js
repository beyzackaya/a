const API_URL = 'http://localhost:5500/api';

export const authservice = {
  login,
  //register,
  getToken,
  setToken,
  logout,
  getUserId,
  isTokenExpired
};

function login(userName, password) {
  return fetch(`${API_URL}/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ userName, password })
  })
    .then(handleResponse)
    .then(data => {
      if (data.token) {
        localStorage.setItem('chat_token', data.token);
      }
      return data;
    });
}

// function register(userData) {
//   return fetch(`${API_URL}/register`, {
//     method: 'POST',
//     headers: { 'Content-Type': 'application/json' },
//     body: JSON.stringify(userData)
//   }).then(handleResponse);
// }

function getToken() {
  return localStorage.getItem('chat_token');
}

function setToken(token) {
  localStorage.setItem('chat_token', token);
}

function logout() {
  localStorage.removeItem('chat_token');
  // clearTokenTimer();
}

function getUserId() {
  const token = getToken();
  if (token) {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.sub || payload.userId || payload.nameid || payload.id || null;
  }
  return null;
}
export function isTokenExpired(token) {
    if (!token) return true;

    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const exp = payload.exp;
        const currentTime = Math.floor(Date.now() / 1000);
        return exp < currentTime;
    } catch (e) {
        console.error("Token parse hatası", e);
        return true;
    }
}

// function isAuthenticated() {
//   const token = getToken();
//   return token && !isTokenExpired();
// }

// function isTokenExpired() {
//   const token = getToken();
//   if (!token) return true;

//   try {
//     const payload = JSON.parse(atob(token.split('.')[1]));
//     const currentTime = Date.now() / 1000;
//     return payload.exp < currentTime;
//   } catch (error) {
//     console.error('Token decode hatası:', error);
//     return true;
//   }
// }

// function getTokenExpirationTime() {
//   const token = getToken();
//   if (!token) return null;

//   try {
//     const payload = JSON.parse(atob(token.split('.')[1]));
//     return payload.exp * 1000; // milisaniye cinsinden
//   } catch (error) {
//     console.error('Token decode hatası:', error);
//     return null;
//   }
// }
  
//   // SignalR bağlantısını kapat
//   if (window.signalRService) {
//     window.signalRService.disconnect();
//   }
  
//   alert('Oturum süreniz dolmuş. Tekrar giriş yapmanız gerekiyor.');
//   window.location.href = '/login';
// }

// function startTokenTimer() {
//   const expirationTime = getTokenExpirationTime();
//   if (!expirationTime) return;

//   const currentTime = Date.now();
//   const timeUntilExpiry = expirationTime - currentTime;

//   if (timeUntilExpiry <= 0) {
//     autoLogout();
//     return;
//   }

//   console.log(`⏰ Token ${Math.round(timeUntilExpiry / 1000)} saniye sonra expire olacak`);

//   // Timer'ları global değişkenlerde sakla
//   window.tokenExpiryTimer = setTimeout(() => {
//     autoLogout();
//   }, timeUntilExpiry);
// }

// function clearTokenTimer() {
//   if (window.tokenExpiryTimer) {
//     clearTimeout(window.tokenExpiryTimer);
//     window.tokenExpiryTimer = null;
//   }
// }

// async function refreshToken() {
//   try {
//     const response = await fetch(`${API_URL}/auth/refresh`, {
//       method: 'POST',
//       headers: {
//         'Content-Type': 'application/json',
//         'Authorization': `Bearer ${getToken()}`
//       }
//     });

//     if (response.ok) {
//       const data = await response.json();
//       setToken(data.token);
//       startTokenTimer(); // Yeni token için timer başlat
//       return data;
//     } else {
//       throw new Error('Token yenilenemedi');
//     }
//   } catch (error) {
//     console.error('Token yenileme hatası:', error);
//     autoLogout();
//     throw error;
//   }
// }

function handleResponse(res) {
  if (!res.ok) {
    return res.text().then(text => {
      try {
        const errorData = JSON.parse(text);
        if (errorData.expired) {
          logout();
        }
      } catch (e) {
      }
      throw new Error(text || 'Bir hata oluştu');
    });
  }
  return res.json();
}