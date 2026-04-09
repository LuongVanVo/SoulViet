using FluentValidation;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Commands.CreatePost;

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content cannot be empty.")
            .MaximumLength(5000).WithMessage("Content cannot exceed 5000 characters.");

        RuleForEach(x => x.MediaUrls)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("MediaUrls must contain valid absolute URLs.");

        RuleFor(x => x.MediaUrls)
            .Must(urls => urls == null || urls.Count <= 20)
            .WithMessage("A post can have a maximum of 20 media items.");
    }
}
