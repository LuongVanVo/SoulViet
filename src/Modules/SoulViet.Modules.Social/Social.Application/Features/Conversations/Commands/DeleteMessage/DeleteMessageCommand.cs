using MediatR;

namespace SoulViet.Modules.Social.Social.Application.Features.Conversations.Commands.DeleteMessage
{
    public class DeleteMessageCommand : IRequest
    {
        public Guid MessageId { get; set; }
        public Guid UserId { get; set; }
    }
}
