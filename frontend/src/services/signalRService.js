import * as signalR from '@microsoft/signalr';
import { authservice,isTokenExpired } from './authservice';


class SignalRService {
  connection = null;
  listeners = {};
  intervalId = null;
  logoutWarningShown = false; 
  async startConnection() {
    const token = authservice.getToken();
    if (!token || authservice.isTokenExpired(token)) {
      this.handleLogout();
      return;
    }
    if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {  
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5500/chatHub', {
        accessTokenFactory: () => authservice.getToken(),
      })
      .withAutomaticReconnect()
      .build();

    this.connection.on('ReceiveMessage', (msg) => this.emit('ReceiveMessage', msg));
    this.connection.on('GroupDetails', (data) => this.emit('GroupDetails', data));
    this.connection.on('GroupDetailsError', (error) => this.emit('GroupDetailsError', error));
    this.connection.on('CreateGroup', (group) => this.emit('CreateGroup', group));

    this.connection.on('AuthenticationError', (data) => {
      this.handleTokenExpired();
    });

    try {
      await this.connection.start();
            this.intervalId = setInterval(() => {
                const token = authservice.getToken();
                if (isTokenExpired(token)) {
                    this.handleLogout();
                }
            }, 15000);    } catch (err) {

      if (err.message && err.message.includes('401')) {
        this.handleTokenExpired();
      }
    }
  }

  // startTokenMonitoring() {
  //   if (this.intervalId) {
  //     clearInterval(this.intervalId);
  //   }

  //   this.intervalId = setInterval(() => {
  //     // Fonksiyon varlığını kontrol et
  //     if (authservice && typeof authservice.isTokenExpired === 'function') {
  //       if (authservice.isTokenExpired()) {
  //         this.handleTokenExpired();
  //       }
  //     } else {
  //       // Fallback - manual token kontrolü
  //       const token = localStorage.getItem('chat_token');
  //       if (token) {
  //         try {
  //           const payload = JSON.parse(atob(token.split('.')[1]));
  //           const currentTime = Date.now() / 1000;
  //           if (payload.exp < currentTime) {
  //             this.handleTokenExpired();
  //           }
  //         } catch (error) {
  //           this.handleTokenExpired();
  //         }
  //       }
  //     }
  //   }, 30000);
  // }


  async stopConnection() {
    if (this.connection && this.connection.state !== signalR.HubConnectionState.Disconnected) {
        await this.connection.stop();
    }   
    if (this.intervalId) {
      clearInterval(this.intervalId);
      this.intervalId = null;
    }
  }

  isConnected() {
    return this.connection && this.connection.state === signalR.HubConnectionState.Connected;
  }

  async sendMessage(groupId, content) {
    if (!this.isConnected()) {
      return;
    }
    try {
      await this.connection.invoke('SendMessage', groupId, content);

    } catch (err) {
      if (err.message && err.message.includes('401')) {
        this.handleTokenExpired();
      }

    }
  }

  // async joinGroup(groupId,users) {
  //   if (!this.isConnected()) {
  //     return;
  //   }
  //   try {
  //     await this.connection.invoke('JoinGroup', groupId, users);
  //   } catch (err) {
  //     if (err.message && err.message.includes('401')) {
  //       this.handleTokenExpired();
  //     }
  //   }
  // }

  // async leaveGroup(groupId) {
  //   if (!this.isConnected()) {
  //     return;
  //   }
  //   try {
  //     await this.connection.invoke('LeaveGroup', groupId);
  //   } catch (err) {
  //   }
  // }

  async getGroupDetails(groupId) {
    if (!this.isConnected()) {
      return;
    }
    try {
      await this.connection.invoke('GetGroupDetails', groupId);
    } catch (err) {
      if (err.message && err.message.includes('401')) {
        this.handleTokenExpired();
      }
      throw err;
    }
  }

  onReceiveMessage(callback) {
    this.on('ReceiveMessage', callback);
  }

  onGroupDetails(callback) {
    this.on('GroupDetails', callback);
  }

  onGroupDetailsError(callback) {
    this.on('GroupDetailsError', callback);
  }
  onCreateGroup(callback) {
    this.on('CreateGroup', callback);
  }
  

  on(event, cb) {
    this.listeners[event] = cb;
  }

  emit(event, data) {
    if (this.listeners[event]) this.listeners[event](data);
  }
    handleLogout() {
        if (this.connection) {
            this.connection.stop();
        }
        clearInterval(this.intervalId);
        localStorage.removeItem("chat_token");
        window.location.href = "";
    }}

export const signalRService = new SignalRService();