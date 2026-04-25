using FluentValidation;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Commands.UpdatePost;

public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
{
    public UpdatePostCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Post Id cannot be empty.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content cannot be empty.")
            .MaximumLength(5000).WithMessage("Content cannot exceed 5000 characters.");

        RuleForEach(x => x.Media)
            .Must(m => Uri.TryCreate(m.Url, UriKind.Absolute, out _))
            .WithMessage("Each media item must have a valid absolute URL.");

        RuleFor(x => x.Media)
            .Must(media => media == null || media.Count <= 20)
            .WithMessage("A post can have a maximum of 20 media items.");
    }
}
