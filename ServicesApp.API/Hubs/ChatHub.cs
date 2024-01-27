using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using ServicesApp.Domain.DTOs.MessageDTOs;
using ServicesApp.Domain.Enums;
using ServicesApp.Infrastructure.Persistence;

namespace ServicesApp.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ChatHub(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task NotifyUserWithNewMessage(string createMessageDto)
        {
            //save message in database
            //var result = await _messageService.SaveMessageInDb(createMessageDto);
            //if (result is not null)
            //Console.WriteLine(createMessageDto);
            await Clients.OthersInGroup(createMessageDto).SendAsync("receiveNewMessage", createMessageDto);

            //if (result is not null)
            //{
            //    await Clients.User(createMessageDto.ReceiverId).SendAsync("receiveNewMessage", result);
            //    await Clients.Caller.SendAsync("ReceiveMessage", result);
            //}
        }
        public async Task MarkAsRead(int messageId)
        {
            // Update the message state to "Read" in the database
            var message = await _dbContext.Messages.FindAsync(messageId);
            if (message is not null)
            {
                message.State = MessageState.Readed;
                await _dbContext.SaveChangesAsync();
            }
            var messageDto = _mapper.Map<MessageResponseDto>(message);
            // Notify the sender about the state change to "Read"
            await Clients.Caller.SendAsync("UpdateMessageState", messageDto);
        }
        public async Task SubscribeUserToChat(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

    }
}
