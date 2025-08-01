import { authservice } from './authservice';

const API_URL = 'http://localhost:5500/api/groups';

export const messageService = {
  getMessages
};

async function getMessages(groupId) {
  const token = authservice.getToken();
  const response = await fetch(`${API_URL}/${groupId}/messages`, {
    method: 'GET',
    headers: {
      'Authorization': 'Bearer ' + token
    }
  });

  if (!response.ok) {
    throw new Error('Mesajlar alınamadı.');
  }

  return await response.json();
}