using FluentValidation;
using SoulViet.Modules.Social.Social.Application.Common.Pagination;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Queries.GetCommentReplies;

public class GetCommentRepliesQueryValidator : AbstractValidator<GetCommentRepliesQuery>
{
    public GetCommentRepliesQueryValidator()
    {
        RuleFor(v => v.CommentId)
            .NotEmpty().WithMessage("CommentId is required.");

        RuleFor(v => v.First)
            .InclusiveBetween(1, 50).WithMessage("First must be between 1 and 50.");

        RuleFor(v => v.SortBy)
            .Must(s => s == "newest" || s == "oldest" || s == "top")
            .WithMessage("SortBy must be either 'newest', 'oldest', or 'top'.");

        RuleFor(v => v.After)
            .Must(BeValidCursor)
            .When(v => !string.IsNullOrEmpty(v.After))
            .WithMessage("Invalid or malformed cursor string.");
    }

    private bool BeValidCursor(string? cursor)
    {
        var decoded = CursorHelper.Decode(cursor);
        return decoded.HasValue;
    }
}
