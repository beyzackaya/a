const API_URL = 'http://localhost:5500/api';

export const groupService = {
  getGroupDetails,
  getGroupMembers,
  createGroup,
  updateGroup,
  deleteGroup
};

async function getGroupDetails(groupId) {
  try {
    const response = await fetch(`${API_URL}/chat/group/${groupId}/details`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('chat_token')}`
      }
    });
    
    if (!response.ok) {
      throw new Error('Grup detayları alınamadı');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Grup detayları yüklenirken hata:', error);

    return {
      groupId: groupId,
      name: `Grup ${groupId}`,
      createdAt: new Date().toISOString(),
      isPrivateChat: false
    };
  }
}

async function getGroupMembers(groupId) {
  try {
    const response = await fetch(`${API_URL}/chat/groups/${groupId}/members`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('chat_token')}`
      }
    });
    
    if (!response.ok) {
      throw new Error('Grup üyeleri alınamadı');
    }
    
    return await response.json();
  } catch (error) {
    console.error('Grup üyeleri yüklenirken hata:', error);
    return [
      {
        userId: 1,
        userName: 'Admin Kullanıcı',
        name: 'Admin',
        isAdmin: true
      }
    ];
  }
}

async function createGroup(groupData) {
  const response = await fetch('http://localhost:5500/api/Group/create-group', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('chat_token')}`
    },
    body: JSON.stringify(groupData)
  });
  
  if (!response.ok) {
    throw new Error('Grup oluşturulamadı');
  }
  
  return await response.json();
}

async function updateGroup(groupId, groupData) {
  const response = await fetch(`${API_URL}/groups/${groupId}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('chat_token')}`
    },
    body: JSON.stringify(groupData)
  });
  
  if (!response.ok) {
    throw new Error('Grup güncellenemedi');
  }
  
  return await response.json();
}

async function deleteGroup(groupId) {
  const response = await fetch(`${API_URL}/groups/${groupId}`, {
    method: 'DELETE',
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('chat_token')}`
    }
  });
  
  if (!response.ok) {
    throw new Error('Grup silinemedi');
  }
  
  return true;
}
