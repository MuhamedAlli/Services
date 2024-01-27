using ServicesApp.Domain.DTOs;
using ServicesApp.Domain.DTOs.MessageDTOs;

namespace ServicesApp.Application.Interfaces
{
    public interface IMessageService
    {
        public Task<MessageResponseDto> SaveMessageInDb(string? contentName, CreateMessageDto createMessageDto);
        public Task<List<ChattedUsersDto>> GetChatsForUser(string userId);
        public Task<PagedResultDto<MessageResponseDto>> GetMessages(GetMessagesInputDto getMessagesInputDto);
        public MessageResponseDto UpdateMessageStatus(int messageID);

    }
}
