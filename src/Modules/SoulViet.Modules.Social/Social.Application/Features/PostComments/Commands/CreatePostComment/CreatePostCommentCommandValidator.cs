using FluentValidation;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Commands.CreatePostComment
{
    public class CreatePostCommentCommandValidator : AbstractValidator<CreatePostCommentCommand>
    {
        public CreatePostCommentCommandValidator()
        {
            RuleFor(x => x.PostId)
                .NotEmpty().WithMessage("PostId is required.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Comment content does not allow empty.");
        }
    }
}