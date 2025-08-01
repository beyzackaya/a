import React, { useEffect, useState } from 'react';
import ChatSidebar from '../components/chat/ChatSidebar'; 
import ChatRoom from '../components/chat/ChatRoom';
import { signalRService } from '../services/signalRService';

export default function ChatPage() {
  const [selectedGroup, setSelectedGroup] = useState(null);
  const [refreshSidebar,setRefreshSidebar] = useState(0);

  useEffect(() => {
    signalRService.startConnection().catch(console.error);
    
    return () => {
      signalRService.stopConnection();
    };
  }, []);
    const handleMessageSent = () => {
    setRefreshSidebar(prev => prev + 1);
  };

  return (
     <div style={{ display: 'flex', height: '100vh' }}>
      <ChatSidebar 
        onGroupSelect={setSelectedGroup} 
        selectedGroup={selectedGroup}
        refreshTrigger={refreshSidebar}
      />
      <ChatRoom 
        group={selectedGroup} 
        onMessageSent={handleMessageSent}
      />
    </div>
  );
}