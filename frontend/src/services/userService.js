const API_URL = 'http://localhost:5500/api';

export const userService = {
  getUserById,
  getCurrentUser,
  updateUser,
  getAllUsers
};

async function getUserById(userId) {
  try {
    const response = await fetch(`${API_URL}/User/${userId}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('chat_token')}`
      }
    });
    
    if (!response.ok) {
      throw new Error('Kullanıcı bilgileri alınamadı');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Kullanıcı bilgileri yüklenirken hata:', error);
    return null;
  }
}

async function getCurrentUser() {
  try {
    const response = await fetch(`${API_URL}/User/me`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('chat_token')}`
      }
    });
    
    if (!response.ok) {
      throw new Error('Kullanıcı bilgileri alınamadı');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Kullanıcı bilgileri yüklenirken hata:', error);
    return null;
  }
}

async function updateUser(userId, userData) {
  const response = await fetch(`${API_URL}/User/${userId}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('chat_token')}`
    },
    body: JSON.stringify(userData)
  });
  
  if (!response.ok) {
    throw new Error('Kullanıcı güncellenemedi');
  }
  
  return await response.json();
}

async function getAllUsers() {
  try {
    const response = await fetch(`${API_URL}/User`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('chat_token')}`
      }
    });
    
    if (!response.ok) {
      throw new Error('Kullanıcılar alınamadı');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Kullanıcılar yüklenirken hata:', error);
    return [];
  }
}
