import React, { useEffect, useState } from 'react';
import { groupService } from '../../services/groupService';
import { authservice } from '../../services/authservice';
import { userService } from '../../services/userService';

export default function NewConversationModal({ onClose, onGroupCreated }) {
  const [allUsers, setAllUsers] = useState([]);
  const [selectedUsers, setSelectedUsers] = useState([]);
  const [groupName, setGroupName] = useState('');
  const [currentUserId, setCurrentUserId] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [searchResults, setSearchResults] = useState([]);
  const [searchLoading, setSearchLoading] = useState(false);

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const users = await userService.getAllUsers();
        setAllUsers(users);
      } catch (err) {
        console.error('Kullanıcılar alınamadı:', err);
      }
    };

    const token = authservice.getToken();
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        setCurrentUserId(payload.sub || payload.userId || payload.nameid);
      } catch (err) {
        console.error('Token decode hatası:', err);
        authservice.logout();

      }
    }

    fetchUsers();
  }, []);

  useEffect(() => {
    let timeout;
    if (searchTerm.length > 0) {
      setSearchLoading(true);
      timeout = setTimeout(async () => {
        try {
          const token = authservice.getToken();
          const response = await fetch(`http://localhost:5500/api/chat/search-users?searchTerm=${encodeURIComponent(searchTerm)}`, {
            method: 'GET',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': 'Bearer ' + token
            }
          });
          if (response.ok) {
            const users = await response.json();
            setSearchResults(users.filter(u => !selectedUsers.includes(u.userId) && u.userId !== parseInt(currentUserId)));
          } else {
            setSearchResults([]);
          }
        } catch (error) {
          setSearchResults([]);
        } finally {
          setSearchLoading(false);
        }
      }, 300);
    } else {
      setSearchResults([]);
    }
    return () => clearTimeout(timeout);
  }, [searchTerm, selectedUsers, currentUserId]);

  const toggleUserSelection = (user) => {
    setSelectedUsers(prev =>
      prev.includes(user.userId)
        ? prev.filter(id => id !== user.userId)
        : [...prev, user.userId]
    );
    setAllUsers(prev => {
      if (!prev.find(u => u.userId === user.userId)) {
        return [...prev, user];
      }
      return prev;
    });
    setSearchTerm('');
    setSearchResults([]);
  };

  // ...existing code...
const handleCreate = async () => {
  if (selectedUsers.length === 0) return;

  try {
    const currentUserIdInt = parseInt(currentUserId);
    const isPrivate = selectedUsers.length === 1;
    let payload;

    if (isPrivate) {
      const targetUserId = selectedUsers[0];
      payload = {
        userIds: [currentUserIdInt, targetUserId],
        groupName: `Private_${currentUserIdInt}_${targetUserId}`,
        isPrivate: true
      };
    } else {
      if (!groupName.trim()) {
        alert("Grup adı gerekli!");
        return;
      }
      payload = {
        userIds: [currentUserIdInt, ...selectedUsers],
        groupName: groupName.trim(),
        isPrivate: false
      };
    }

    console.log('Grup oluşturma payload:', payload);
    const group = await groupService.createGroup(payload);
    onGroupCreated(group);
    onClose();
  } catch (err) {
    console.error('Grup oluşturulamadı:', err);
    alert('Grup oluşturulamadı: ' + err.message);
  }
};

  return (
    <div style={{
      position: 'fixed', top: 0, left: 0, right: 0, bottom: 0,
      backgroundColor: 'rgba(0,0,0,0.5)', display: 'flex', justifyContent: 'center', alignItems: 'center',
      zIndex: 1000
    }}>
      <div style={{ backgroundColor: 'white', padding: 20, borderRadius: 8, width: '400px' }}>
        <span style={{ float: 'right', cursor: 'pointer', fontSize: 22 }} onClick={onClose}>&times;</span>
        <h3>Yeni Konuşma Başlat</h3>
        <div style={{ marginBottom: 10 }}>
          <input
            type="text"
            placeholder="Kullanıcı adı veya isim ara..."
            value={searchTerm}
            onChange={e => setSearchTerm(e.target.value)}
            style={{ width: '100%', padding: 8, marginBottom: 5 }}
          />
          {searchLoading && <div style={{ color: '#888', fontSize: 12 }}>Aranıyor...</div>}
          {searchResults.length > 0 && (
            <div style={{ maxHeight: 120, overflowY: 'auto', border: '1px solid #eee', background: '#fafafa', marginBottom: 5 }}>
              {searchResults.map(user => (
                <div
                  key={user.userId}
                  style={{ padding: '6px 10px', cursor: 'pointer', borderBottom: '1px solid #eee' }}
                  onClick={() => toggleUserSelection(user)}
                >
                  {user.userName} ({user.name} {user.lastName})
                </div>
              ))}
            </div>
          )}
        </div>

        <div className="selected-users" style={{ marginBottom: 10 }}>
          <strong>Seçilen Kullanıcılar:</strong>
          <div style={{ display: 'flex', flexWrap: 'wrap', gap: 5, marginTop: 5 }}>
            {selectedUsers.map(userId => {
              const user = allUsers.find(u => u.userId === userId);
              if (!user) return null;
              return (
                <span key={userId} style={{ background: '#e3e3e3', borderRadius: 12, padding: '4px 10px', fontSize: 13 }}>
                  {user.userName}
                  <span style={{ marginLeft: 6, cursor: 'pointer', color: '#d00' }} onClick={() => setSelectedUsers(selectedUsers.filter(id => id !== userId))}>&times;</span>
                </span>
              );
            })}
          </div>
        </div>

        {selectedUsers.length > 1 && (
          <div style={{ marginBottom: 10 }}>
            <input
              type="text"
              placeholder="Grup adı girin..."
              value={groupName}
              onChange={e => setGroupName(e.target.value)}
              style={{ width: '100%', padding: 8 }}
            />
          </div>
        )}

        <button
          onClick={handleCreate}
          disabled={selectedUsers.length === 0 || (selectedUsers.length > 1 && !groupName)}
          style={{ width: '100%', padding: '10px', background: '#007bff', color: 'white', border: 'none', borderRadius: 4, fontWeight: 'bold', cursor: selectedUsers.length === 0 ? 'not-allowed' : 'pointer' }}
        >
          Konuşma Oluştur
        </button>
      </div>
    </div>
  );
}