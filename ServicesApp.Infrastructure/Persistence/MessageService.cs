using AutoMapper;
using ServicesApp.Domain.DTOs;
using ServicesApp.Domain.DTOs.MessageDTOs;
using ServicesApp.Domain.Enums;

namespace ServicesApp.Infrastructure.Persistence
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        public MessageService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<ChattedUsersDto>> GetChatsForUser(string userId)
        {
            var chattedUsers = await _dbContext.Messages
            .Where(m => m.SenderId == userId)
            .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Select(g => new ChattedUsersDto
            {
                Id = g.FirstOrDefault().ReceiverId,
                FullName = g.OrderByDescending(m => m.CreatedOn).FirstOrDefault().Receiver.FullName,
                Image = g.OrderByDescending(m => m.CreatedOn).FirstOrDefault().Receiver.Image,
                LastMessage = g.OrderByDescending(m => m.CreatedOn).FirstOrDefault().Content,
                LastMessageDate = g.OrderByDescending(m => m.CreatedOn).FirstOrDefault().CreatedOn
            }).Distinct()
            .ToListAsync();
            return chattedUsers;
        }

        public async Task<PagedResultDto<MessageResponseDto>> GetMessages(GetMessagesInputDto getMessagesInputDto)
        {
            // Calculate the number of users to skip based on the page and pageSize
            int messagesToSkip = (getMessagesInputDto.Page - 1) * getMessagesInputDto.PageSize;
            // Retrieve total count of users
            int totalCount = await _dbContext.Messages.Where(u => ((u.SenderId == getMessagesInputDto.SenderId && u.ReceiverId == getMessagesInputDto.ReceiverId) ||
                (u.SenderId == getMessagesInputDto.ReceiverId && u.ReceiverId == getMessagesInputDto.SenderId))).CountAsync();
            int totalPages = totalCount % getMessagesInputDto.PageSize == 0 ? (totalCount / getMessagesInputDto.PageSize) : ((totalCount / getMessagesInputDto.PageSize) + 1);


            var unReadedMessages = await _dbContext.Messages
                .Where(u => ((u.SenderId == getMessagesInputDto.SenderId && u.ReceiverId == getMessagesInputDto.ReceiverId) ||
                (u.SenderId == getMessagesInputDto.ReceiverId && u.ReceiverId == getMessagesInputDto.SenderId)) &&
                !(u.State == MessageState.Readed))
                .OrderByDescending(u => u.CreatedOn)
                .Skip(messagesToSkip)
                .Take(getMessagesInputDto.PageSize)
                .ToListAsync();
            foreach (var msg in unReadedMessages)
            {
                msg.State = MessageState.Readed;
            }
            await _dbContext.SaveChangesAsync();

            var messages = await _dbContext.Messages
                .Where(u => (u.SenderId == getMessagesInputDto.SenderId && u.ReceiverId == getMessagesInputDto.ReceiverId)
                || (u.SenderId == getMessagesInputDto.ReceiverId && u.ReceiverId == getMessagesInputDto.SenderId))
                .OrderByDescending(u => u.CreatedOn)
                .Skip(messagesToSkip)
                .Take(getMessagesInputDto.PageSize)
                .ToListAsync();
            var servicesDto = _mapper.Map<IEnumerable<MessageResponseDto>>(messages);

            var pagedResultDto = new PagedResultDto<MessageResponseDto>
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Page = getMessagesInputDto.Page,
                PageSize = getMessagesInputDto.PageSize,
                Data = servicesDto
            };

            return pagedResultDto;
        }

        public async Task<MessageResponseDto> SaveMessageInDb(string? contentName, CreateMessageDto createMessageDto)
        {
            var message = new Message
            {
                Type = createMessageDto.Type,
                Content = createMessageDto.Content,
                CreatedOn = DateTime.Now,
                State = MessageState.Sent,
                SenderId = createMessageDto.SenderId,
                ReceiverId = createMessageDto.ReceiverId,
                ContentUrl = contentName
            };
            _dbContext.Messages.Add(message);
            var result = await _dbContext.SaveChangesAsync();

            if (result <= 0)
                return null;

            var messageDto = _mapper.Map<MessageResponseDto>(message);

            return messageDto;
        }

        public MessageResponseDto UpdateMessageStatus(int messageID)
        {
            var message = _dbContext.Messages.Find(messageID);
            if (message is not null)
                message.State = MessageState.Delivered;

            var result = _mapper.Map<MessageResponseDto>(message);

            return result;
        }
    }
}
