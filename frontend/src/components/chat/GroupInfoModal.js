import React, { useEffect, useState } from 'react';
import { groupService } from '../../services/groupService';
import { signalRService } from '../../services/signalRService';

export default function GroupInfoModal({ groupId, groupDetails: initialGroupDetails, onClose }) {
  const [groupDetails, setGroupDetails] = useState(initialGroupDetails);
  const [groupMembers, setGroupMembers] = useState(initialGroupDetails?.members || []);
  const [loading, setLoading] = useState(!initialGroupDetails);

  useEffect(() => {
    // Eğer başlangıçta grup detayları varsa, yükleme işlemini atla
    if (initialGroupDetails) {
      setGroupDetails(initialGroupDetails);
      setGroupMembers(initialGroupDetails.members || []);
      setLoading(false);
      return;
    }

    const loadGroupInfo = async () => {
      try {
        setLoading(true);
        
        // SignalR üzerinden grup detaylarını iste
        signalRService.onGroupDetails((data) => {
          console.log('📋 Modal SignalR grup detayları alındı:', data);
          setGroupDetails({
            groupId: data.groupId,
            name: data.groupName,
            createdAt: data.createdDate,
            createdByName: data.createdByName,
            isCurrentUserOwner: data.isCurrentUserOwner,
            isPrivateChat: data.isPrivateChat,
            memberCount: data.memberCount
          });
          setGroupMembers(data.members || []);
          setLoading(false);
        });

        signalRService.onGroupDetailsError((error) => {
          console.error('🚨 Modal SignalR grup detayları hatası:', error);
          // Hata durumunda HTTP API'sine fallback
          loadGroupInfoViaHTTP();
        });

        // SignalR ile grup detaylarını iste
        await signalRService.getGroupDetails(groupId);
        
      } catch (error) {
        console.error('Modal SignalR grup bilgileri yüklenirken hata:', error);
        // SignalR başarısız olursa HTTP API'sine fallback
        loadGroupInfoViaHTTP();
      }
    };

    const loadGroupInfoViaHTTP = async () => {
      try {
        setLoading(true);
        
        // HTTP API fallback
        if (groupService && groupService.getGroupDetails) {
          const details = await groupService.getGroupDetails(groupId);
          setGroupDetails(details);
        }
        
        if (groupService && groupService.getGroupMembers) {
          const members = await groupService.getGroupMembers(groupId);
          setGroupMembers(members);
        }
        
        setLoading(false);
      } catch (error) {
        console.error('HTTP grup bilgileri yüklenirken hata:', error);
        setLoading(false);
      }
    };

    if (groupId && !initialGroupDetails) {
      loadGroupInfo();
    }

    return () => {
      signalRService.listeners['GroupDetails'] = null;
      signalRService.listeners['GroupDetailsError'] = null;
    };
  }, [groupId, initialGroupDetails]);

  const handleBackdropClick = (e) => {
    if (e.target === e.currentTarget) {
      onClose();
    }
  };

  return (
    <div 
      style={{
        position: 'fixed',
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        backgroundColor: 'rgba(0, 0, 0, 0.5)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        zIndex: 1000
      }}
      onClick={handleBackdropClick}
    >
      <div 
        style={{
          backgroundColor: 'white',
          borderRadius: '12px',
          width: '90%',
          maxWidth: '500px',
          maxHeight: '80vh',
          overflowY: 'auto',
          boxShadow: '0 10px 30px rgba(0, 0, 0, 0.3)',
          position: 'relative'
        }}
      >
        <div style={{
          padding: '20px',
          borderBottom: '1px solid #e0e0e0',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between'
        }}>
          <div style={{ display: 'flex', alignItems: 'center' }}>
            <div 
              style={{
                width: '50px',
                height: '50px',
                borderRadius: '50%',
                backgroundColor: '#007bff',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                color: 'white',
                fontWeight: 'bold',
                marginRight: '15px',
                fontSize: '20px'
              }}
            >
              {groupDetails?.name?.charAt(0).toUpperCase() || 'G'}
            </div>
            <div>
              <h2 style={{ 
                margin: 0, 
                fontSize: '20px',
                color: '#2c3e50'
              }}>
                {groupDetails?.name || 'Grup Adı'}
              </h2>
              <p style={{
                margin: '4px 0 0 0',
                fontSize: '14px',
                color: '#6c757d'
              }}>
                {groupMembers.length} üye
              </p>
            </div>
          </div>
          <button
            onClick={onClose}
            style={{
              background: 'none',
              border: 'none',
              fontSize: '24px',
              cursor: 'pointer',
              color: '#6c757d',
              padding: '5px',
              borderRadius: '50%',
              width: '35px',
              height: '35px',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center'
            }}
            onMouseEnter={(e) => e.target.style.backgroundColor = '#f8f9fa'}
            onMouseLeave={(e) => e.target.style.backgroundColor = 'transparent'}
          >
            ×
          </button>
        </div>

        <div style={{ padding: '20px' }}>
          {loading ? (
            <div style={{ 
              textAlign: 'center', 
              padding: '40px',
              color: '#6c757d'
            }}>
              Yükleniyor...
            </div>
          ) : (
            <>
              <div style={{ marginBottom: '25px' }}>
                <h3 style={{ 
                  fontSize: '16px', 
                  marginBottom: '10px',
                  color: '#495057'
                }}>
                  Grup Bilgileri
                </h3>
                <div style={{
                  backgroundColor: '#f8f9fa',
                  padding: '15px',
                  borderRadius: '8px',
                  fontSize: '14px'
                }}>
                  <div style={{ marginBottom: '8px' }}>
                    <strong>Grup Adı:</strong> {groupDetails?.name || 'Belirtilmemiş'}
                  </div>
                  <div>
                    <strong>Oluşturulma:</strong> {groupDetails?.createdAt ? 
                      new Date(groupDetails.createdAt).toLocaleDateString('tr-TR') : 
                      'Bilinmiyor'
                    }
                  </div>
                </div>
              </div>

              {/* Grup Üyeleri */}
              <div>
                <h3 style={{ 
                  fontSize: '16px', 
                  marginBottom: '10px',
                  color: '#495057'
                }}>
                  Grup Üyeleri ({groupMembers.length || 0})
                </h3>
                <div style={{ maxHeight: '200px', overflowY: 'auto' }}>
                  {groupMembers.length > 0 ? (
                    groupMembers.map((member, index) => (
                      <div 
                        key={member.userId || index}
                        style={{
                          display: 'flex',
                          alignItems: 'center',
                          padding: '10px',
                          borderBottom: index < groupMembers.length - 1 ? '1px solid #e9ecef' : 'none'
                        }}
                      >
                        <div 
                          style={{
                            width: '35px',
                            height: '35px',
                            borderRadius: '50%',
                            backgroundColor: '#28a745',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            color: 'white',
                            fontWeight: 'bold',
                            marginRight: '12px',
                            fontSize: '14px'
                          }}
                        >
                          {member.userName?.charAt(0).toUpperCase() || member.name?.charAt(0).toUpperCase() || 'U'}
                        </div>
                        <div>
                          <div style={{ 
                            fontWeight: '500',
                            fontSize: '14px',
                            color: '#2c3e50'
                          }}>
                            {member.userName || member.name || 'İsimsiz Kullanıcı'}
                          </div>
                          <div style={{ 
                            fontSize: '12px',
                            color: '#6c757d'
                          }}>
                            {member.isAdmin ? 'Admin' : 'Üye'}
                          </div>
                        </div>
                      </div>
                    ))
                  ) : (
                    <div style={{
                      textAlign: 'center',
                      padding: '20px',
                      color: '#6c757d',
                      fontSize: '14px'
                    }}>
                      Grup üyeleri yüklenemedi
                    </div>
                  )}
                </div>
              </div>
            </>
          )}
        </div>

        {/* Modal Footer */}
        <div style={{
          padding: '15px 20px',
          borderTop: '1px solid #e0e0e0',
          display: 'flex',
          justifyContent: 'flex-end'
        }}>
          <button
            onClick={onClose}
            style={{
              padding: '8px 20px',
              backgroundColor: '#007bff',
              color: 'white',
              border: 'none',
              borderRadius: '6px',
              cursor: 'pointer',
              fontSize: '14px'
            }}
            onMouseEnter={(e) => e.target.style.backgroundColor = '#0056b3'}
            onMouseLeave={(e) => e.target.style.backgroundColor = '#007bff'}
          >
            Kapat
          </button>
        </div>
      </div>
    </div>
  );
}
