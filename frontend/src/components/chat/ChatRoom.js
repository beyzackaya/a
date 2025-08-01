import React, { useEffect, useRef, useState } from 'react';
import { messageService } from '../../services/messageService';
import { signalRService } from '../../services/signalRService';
import { authservice } from '../../services/authservice';
import GroupInfoModal from './GroupInfoModal';

export default function ChatRoom({ group,onMessageSent }) {
  const [messages, setMessages] = useState([]);
  const [newMsg, setNewMsg] = useState('');
  const [currentUserId, setCurrentUserId] = useState(null);
  const [showGroupDetails, setShowGroupDetails] = useState(false);
  const [groupDetails, setGroupDetails] = useState(null);
  const messagesEndRef = useRef(null);


  useEffect(() => {
    const getUserId = () => {
      const token = authservice.getToken();
      if (token) {
        try {
          const payload = JSON.parse(atob(token.split('.')[1]));
          return payload.sub || payload.userId || payload.nameid;
        } catch (error) {
          console.error('Token decode hatası:', error);
        }
      }
      return null;
    };

    setCurrentUserId(getUserId());
  }, []);
    useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  useEffect(() => {
    if (group) {
      messageService.getMessages(group.groupId).then(setMessages);
     // signalRService.joinGroup(group.groupId);

      signalRService.onReceiveMessage(message => {
        console.log('Gelen mesaj objesi:', message); 
        console.log('SenderName:', message.senderName); 
        console.log('SenderId:', message.senderId);
        
        if (message.groupId === group.groupId) {
          setMessages(prev => [...prev, message]);
        }
      });

      const loadGroupDetails = () => {
        signalRService.onGroupDetails((data) => {
          console.log('ChatRoom grup detayları alındı:', data);
          setGroupDetails({
            groupId: data.groupId,
            name: data.groupName,
            createdAt: data.createdDate,
            createdByName: data.createdByName,
            isCurrentUserOwner: data.isCurrentUserOwner,
            isPrivateChat: data.isPrivateChat,
            memberCount: data.memberCount,
            members: data.members || []
          });
        });

        signalRService.onGroupDetailsError((error) => {
          console.error('ChatRoom grup detayları hatası:', error);
        });

        signalRService.getGroupDetails(group.groupId);
      };

      loadGroupDetails();

      return () => {
        //signalRService.leaveGroup(group.groupId);
        signalRService.listeners['GroupDetails'] = null;
        signalRService.listeners['GroupDetailsError'] = null;
      };
    }
  }, [group]);

  const handleSend = () => {
    if (newMsg.trim()) {
      signalRService.sendMessage(group.groupId, newMsg);
      setNewMsg('');
      if (onMessageSent) {
        onMessageSent();
      }
      
      setTimeout(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
      }, 100);
    }
  };

  if (!group) return <div style={{ flex: 1, padding: '20px' }}>Grup seçin...</div>;

  return (
    <div style={{ flex: 1, display: 'flex', flexDirection: 'column' }}>
      <div 
        style={{ 
          padding: '15px 20px',
          borderBottom: '1px solid #e0e0e0',
          backgroundColor: '#f8f9fa',
          cursor: 'pointer',
          display: 'flex',
          alignItems: 'center',
          transition: 'background-color 0.2s ease'
        }}
        onClick={() => setShowGroupDetails(true)}
        onMouseEnter={(e) => e.target.style.backgroundColor = '#e9ecef'}
        onMouseLeave={(e) => e.target.style.backgroundColor = '#f8f9fa'}
      >
        <div style={{ display: 'flex', alignItems: 'center', flex: 1 }}>
          <div 
            style={{
              width: '45px',
              height: '45px',
              borderRadius: '50%',
              backgroundColor: '#25d366',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              color: 'white',
              fontWeight: 'bold',
              marginRight: '15px',
              fontSize: '18px',
              flexShrink: 0
            }}
          >
            {(groupDetails?.name || group.name)?.charAt(0).toUpperCase() || 'G'}
          </div>
          
          <div style={{ flex: 1, minWidth: 0 }}>
            <div style={{ 
              fontWeight: '600', 
              fontSize: '16px',
              color: '#111b21',
              marginBottom: '2px',
              overflow: 'hidden',
              textOverflow: 'ellipsis',
              whiteSpace: 'nowrap'
            }}>
              {groupDetails?.name || group.name || 'Grup Adı'}
            </div>
            <div style={{ 
              fontSize: '13px', 
              color: '#667781',
              overflow: 'hidden',
              textOverflow: 'ellipsis',
              whiteSpace: 'nowrap'
            }}>
              {groupDetails?.isPrivateChat ? 
                'Son görülme zamanı' : 
                `${groupDetails?.memberCount || 0} üye`}
            </div>
          </div>
        </div>
        
        
      </div>

      <div style={{ flex: 1, overflowY: 'auto', padding: '10px' }}>
        {messages.map(msg => {
          const isCurrentUser = msg.isMine || 
                               (msg.senderId?.toString() === currentUserId?.toString());
          
          console.log('Mesaj debug:', {
            messageId: msg.id,
            isMine: msg.isMine,
            senderId: msg.senderId,
            currentUserId: currentUserId,
            isCurrentUser: isCurrentUser
          });
          
          return (
            <div 
              key={msg.id || msg.messageId} 
              style={{
                display: 'flex',
                justifyContent: isCurrentUser ? 'flex-end' : 'flex-start',
                marginBottom: '10px',
                width: '100%'
              }}
            >
              <div
                style={{
                  maxWidth: '70%',
                  padding: '8px 12px',
                  borderRadius: '12px',
                  backgroundColor: isCurrentUser ? '#007bff' : '#f1f1f1',
                  color: isCurrentUser ? 'white' : 'black',
                  boxShadow: '0 1px 2px rgba(0,0,0,0.1)'
                }}
              >
                {!isCurrentUser && (
                  <div style={{ 
                    fontSize: '12px', 
                    fontWeight: 'bold', 
                    marginBottom: '4px',
                    opacity: 0.8
                  }}>
                    {msg.senderName}
                  </div>
                )}
                <div>{msg.content}</div>
                <div style={{ 
                  fontSize: '10px', 
                  opacity: 0.7, 
                  marginTop: '4px',
                  textAlign: isCurrentUser ? 'right' : 'left'
                }}>
                  {new Date(msg.createdAt || msg.sentAt).toLocaleTimeString('tr-TR', {
                    hour: '2-digit',
                    minute: '2-digit'
                  })}
                </div>
              </div>
            </div>
          );
        })}
        <div ref={messagesEndRef} />
      </div>
      <div style={{ padding: '10px', borderTop: '1px solid #ccc' }}>
        <input
          value={newMsg}
          onChange={e => setNewMsg(e.target.value)}
          onKeyDown={e => e.key === 'Enter' && handleSend()}
          placeholder="Mesaj yaz..."
          style={{ 
            width: '80%', 
            padding: '8px', 
            border: '1px solid #ccc', 
            borderRadius: '4px' 
          }}
        />
        <button 
          onClick={handleSend}
          style={{
            marginLeft: '10px',
            padding: '8px 16px',
            backgroundColor: '#007bff',
            color: 'white',
            border: 'none',
            borderRadius: '4px',
            cursor: 'pointer'
          }}
        >
          Gönder
        </button>

      </div>

      {showGroupDetails && (
        <GroupInfoModal 
          groupId={group.groupId}
          groupDetails={groupDetails}
          onClose={() => setShowGroupDetails(false)} 
        />
      )}
    </div>
  );
}