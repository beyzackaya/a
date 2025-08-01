using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using ChatApp.Application.Interfaces;
using System.Security.Claims;

namespace ChatApp.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IGroupUserService _groupUserService;
        private readonly IUserOnlineStatusService _userOnlineStatusService;
        private readonly IGroupService _groupService;
        private readonly IMessageReadStatusService _messageReadStatusService;
        private readonly IUserService _userService;
        private static readonly Dictionary<string, int> _connectedUsers = new();

        public ChatHub(
            IMessageService messageService,
            IGroupUserService groupUserService,
            IUserOnlineStatusService userOnlineStatusService,
            IGroupService groupService,
            IMessageReadStatusService messageReadStatusService,
            IUserService userService) 
        {
            _messageService = messageService;
            _groupUserService = groupUserService;
            _userOnlineStatusService = userOnlineStatusService;
            _groupService = groupService;
            _messageReadStatusService = messageReadStatusService;
            _userService = userService;
        }

        // online 
        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                _connectedUsers[Context.ConnectionId] = userId.Value;
                await _userOnlineStatusService.SetOnlineStatusAsync(userId.Value, true);

                var userGroups = await _groupUserService.GetGroupsbyUserIdAsync(userId.Value);
                foreach (var group in userGroups)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Group_{group.GroupId}");
                    await Clients.Group($"Group_{group.GroupId}").SendAsync("UserJoined", new
                    {
                        UserId = userId.Value,
                        GroupId = group.GroupId
                    });
                }
            }

            await base.OnConnectedAsync();
        }

        //offline
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connectedUsers.TryGetValue(Context.ConnectionId, out var userId))
            {
                _connectedUsers.Remove(Context.ConnectionId);
                await _userOnlineStatusService.SetOnlineStatusAsync(userId, false);

                var userGroups = await _groupUserService.GetGroupsbyUserIdAsync(userId);
                foreach (var group in userGroups)
                {
                    await Clients.Group($"Group_{group.GroupId}").SendAsync("UserLeft", new
                    {
                        UserId = userId,
                        GroupId = group.GroupId
                    });
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
        public async Task GetMessages(int groupId)
{
    var userId = GetUserId();
    if (!userId.HasValue) return;

    var messages = await _messageService.GetMessagesByGroupIdAsync(groupId,userId.Value);

    await Clients.Caller.SendAsync("Messages", messages);
}

        public async Task SendMessage(int groupId, string content)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return;

            var createdMessage = await _messageService.CreateMessageAsync(userId.Value, groupId, content);

            var user = await _userService.GetUserByIdAsync(userId.Value);

            var groupUsers = await _groupUserService.GetUsersByGroupIdAsync(groupId);

            foreach (var groupUser in groupUsers)
            {
                var connectionIds = _connectedUsers.Where(x => x.Value == groupUser.UserId).Select(x => x.Key);

                foreach (var connectionId in connectionIds)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveMessage", new
                    {
                        Id = createdMessage.MessageId,
                        Content = createdMessage.Content,
                        SenderId = createdMessage.SenderId,
                        SenderName = user?.UserName ?? "Bilinmeyen",
                        GroupId = createdMessage.GroupId,
                        CreatedAt = createdMessage.SentAt,
                        IsRead = false,
                        IsMine = groupUser.UserId == userId.Value // Her kullanıcı için ayrı hesapla
                    });
                }
            }
            
        }

        public async Task JoinGroup(int groupId)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return;

            await Groups.AddToGroupAsync(Context.ConnectionId, $"Group_{groupId}");
            await Clients.Group($"Group_{groupId}").SendAsync("UserJoined", new { UserId = userId.Value, GroupId = groupId });
        }

        // public async Task (int groupId)
        // {
        //     var userId = GetUserId();
        //     if (!userId.HasValue) return;

        //     await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Group_{groupId}");
        //     await Clients.Group($"Group_{groupId}").SendAsync("UserLeft", new { UserId = userId.Value, GroupId = groupId });
        // }

        public async Task MarkMessageAsRead(int messageId)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return;

            await _messageReadStatusService.SetReadStatusAsync(messageId, true);
            await Clients.All.SendAsync("MessageRead", new { MessageId = messageId, UserId = userId.Value });
        }

        public async Task GetOnlineUsers(int groupId)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return;

            var groupUsers = await _groupUserService.GetUsersByGroupIdAsync(groupId);
            var onlineUserIds = new List<int>();

            foreach (var groupUser in groupUsers)
            {
                var status = await _userOnlineStatusService.GetLatestStatusAsync(groupUser.UserId);
                if (status?.IsOnline == true)
                {
                    onlineUserIds.Add(groupUser.UserId);
                }
            }

            await Clients.Caller.SendAsync("OnlineUsers", new { GroupId = groupId, UserIds = onlineUserIds });
        }

        public async Task GetGroupDetails(int groupId)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return;

            try
            {
                var group = await _groupService.GetGroupByIdAsync(groupId);
                if (group == null)
                {
                    await Clients.Caller.SendAsync("GroupDetailsError", new { GroupId = groupId, Error = "Grup bulunamadı" });
                    return;
                }

                var groupUsers = await _groupService.GetGroupUsersAsync(groupId);
                var members = new List<object>();

                foreach (var groupUser in groupUsers)
                {
                    var user = await _userService.GetUserByIdAsync(groupUser.UserId);
                    if (user != null)
                    {
                        var onlineStatus = await _userOnlineStatusService.GetLatestStatusAsync(user.UserId);
                        members.Add(new
                        {
                            UserId = user.UserId,
                            UserName = user.UserName,
                            DisplayName = $"{user.Name} {user.LastName}",
                            IsOwner = user.UserId == group.CreatedByUserId,
                            IsOnline = onlineStatus?.IsOnline ?? false
                        });
                    }
                }

                var createdByUser = await _userService.GetUserByIdAsync(group.CreatedByUserId);
                string createdByName = createdByUser != null ? $"{createdByUser.Name} {createdByUser.LastName}" : "Bilinmeyen";

                var groupDetails = new 
                { 
                    GroupId = group.GroupId,
                    GroupName = group.GroupName,
                    CreatedByName = createdByName,
                    CreatedDate = group.CreatedAt,
                    IsCurrentUserOwner = group.CreatedByUserId == userId.Value,
                    IsPrivateChat = members.Count == 2,
                    Members = members,
                    MemberCount = members.Count
                };

                await Clients.Caller.SendAsync("GroupDetails", groupDetails);
            }
            catch (Exception)
            {
                await Clients.Caller.SendAsync("GroupDetailsError", new { GroupId = groupId, Error = "Grup detayları alınırken hata oluştu" });
            }
        }

        private int? GetUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : (int?)null;
        }
    }
}