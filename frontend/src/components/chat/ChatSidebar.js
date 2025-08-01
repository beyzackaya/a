import React, { useEffect, useState } from 'react';
import { chatService } from '../../services/chatService';
import NewConversationModal from './NewConversation';
import { signalRService } from '../../services/signalRService';

export default function ChatSidebar({ onGroupSelect, selectedGroup, refreshTrigger }) {
  const [groups, setGroups] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showNewModal, setShowNewModal] = useState(false);

  useEffect(() => {
    loadGroups();
    signalRService.onReceiveMessage(() => {
      loadGroups();
    });
    return () => {
      signalRService.onReceiveMessage(() => {});
    };
  }, []);

  useEffect(() => {
    if (refreshTrigger > -1) {
      loadGroups();
    }
  }, [refreshTrigger]);

  async function loadGroups() {
    setLoading(true);
    setError(null);
    try {
      const data = await chatService.getSidebarGroups();
      console.log('Sidebar verileri yüklendi:', data);
      setGroups(data);
    } catch (error) {
      console.error('Sidebar verileri yüklenemedi.', error);
      setError('Konuşmalar yüklenemedi. Lütfen tekrar deneyin.');
      setGroups([]);
    } finally {
      setLoading(false);
    }
  }

  const handleGroupCreated = async (newGroup) => {
    await loadGroups();
    setShowNewModal(false);
    if (newGroup) {
      onGroupSelect(newGroup);
      loadGroups();
    }
  };

  return (
    <div style={{ width: '30%', borderRight: '1px solid #ccc', overflowY: 'auto' }}>
      <h3>Konuşmalar</h3>
      <button 
        style={{
          margin: '10px',
          padding: '8px 12px',
          backgroundColor: '#007bff',
          color: 'white',
          border: 'none',
          borderRadius: '4px',
          cursor: 'pointer'
        }}
        onClick={() => setShowNewModal(true)}
      >
        + Yeni Konuşma
      </button>
      {loading && <p style={{ color: '#888', padding: '10px' }}>Yükleniyor...</p>}
      {error && <p style={{ color: 'red', padding: '10px' }}>{error}</p>}
      {!loading && !error && groups.length === 0 && (
        <p style={{ color: '#888', padding: '10px' }}>Hiç konuşma bulunamadı.</p>
      )}
      {groups.map(group => (
        <div
          key={group.groupId}
          onClick={() => onGroupSelect(group)}
          style={{
            padding: '10px',
            background: selectedGroup?.groupId === group.groupId ? '#eee' : 'white',
            cursor: 'pointer'
          }}
        >
          <strong>{group.groupName}</strong>
          <p style={{ fontSize: '0.9em', color: '#666' }}>{group.lastMessageContent}</p>
        </div>
      ))}
      {showNewModal && (
        <NewConversationModal
          onClose={() => setShowNewModal(false)}
          onGroupCreated={handleGroupCreated}
        />
      )}
    </div>
  );
}
