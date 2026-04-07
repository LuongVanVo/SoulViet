using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Commands.RemoveCartItems
{
    public class RemoveCartItemsValidator : AbstractValidator<RemoveCartItemsCommand>
    {
        public RemoveCartItemsValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid User ID format.");

            RuleFor(x => x.CartItemIds)
                .NotEmpty().WithMessage("At least one CartItemId is required.")
                .Must(ids => ids.All(id => Guid.TryParse(id.ToString(), out _)))
                .WithMessage("All Cart Item IDs must be in a valid GUID format.");
        }
    }
}