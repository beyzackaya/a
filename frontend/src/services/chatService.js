import { authservice } from './authservice';

const API_URL = 'http://localhost:5500/api/chatSidebar';

export const chatService = {
  getSidebarGroups
};

async function getSidebarGroups() {
  const token = authservice.getToken();
  const response = await fetch(API_URL, {
    method: 'GET',
    headers: {
      'Authorization': 'Bearer ' + token
    }
  });

  if (!response.ok) {
    throw new Error('Sidebar verileri yüklenemedi.');
  }

  return await response.json();
}