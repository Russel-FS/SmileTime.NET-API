using AutoMapper;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.src.Aplication.DTOs.chat;

namespace SmileTimeNET_API.src.Aplication.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Mapeo de Usuario
            CreateMap<ApplicationUser, UserDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName ?? string.Empty))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar ?? string.Empty));

            CreateMap<UserDTO, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId ?? string.Empty))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName ?? string.Empty))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar ?? string.Empty));

            // Mapeo de Message 
            CreateMap<Message, MessageDTO>()
                .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.MessageType, opt => opt.MapFrom(src => src.MessageType))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => src.ModifiedAt))
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments))
                .ForMember(dest => dest.MessageStatuses, opt => opt.MapFrom(src => src.MessageStatuses));

            CreateMap<MessageDTO, Message>()
                .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId ?? string.Empty))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content ?? string.Empty))
                .ForMember(dest => dest.MessageType, opt => opt.MapFrom(src => src.MessageType ?? "text"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => src.ModifiedAt))
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

            CreateMap<Attachment, AttachmentDTO>();
            CreateMap<AttachmentDTO, Attachment>();

            CreateMap<MessageStatus, MessageStatusDTO>();
            CreateMap<MessageStatusDTO, MessageStatus>();

            // Mapeo de Conversation
            CreateMap<Conversation, ConversationDto>()
              .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants));

            CreateMap<ConversationDto, Conversation>()
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants.Select(p => new ConversationParticipant
                {
                    UserId = p.UserId,
                    ConversationId = p.ConversationId ?? 0,
                    IsAdmin = p.IsAdmin ?? false,
                    JoinedAt = p.JoinedAt ?? DateTime.UtcNow,
                    LeftAt = p.LeftAt,
                    LastRead = DateTime.UtcNow
                })));

            CreateMap<ConversationParticipant, UserDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User!.UserName))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User!.Avatar))
                .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.IsAdmin))
                .ForMember(dest => dest.JoinedAt, opt => opt.MapFrom(src => src.JoinedAt))
                .ForMember(dest => dest.LeftAt, opt => opt.MapFrom(src => src.LeftAt))
                .ForMember(dest => dest.ConversationId, opt => opt.MapFrom(src => src.ConversationId));

        }
    }
}