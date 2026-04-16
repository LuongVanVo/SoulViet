using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Commands.UpdatePostComment
{
    public class UpdatePostCommentCommandValidator : AbstractValidator<UpdatePostCommentCommand>
    {
        public UpdatePostCommentCommandValidator() {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required.");
        }
    }
}
