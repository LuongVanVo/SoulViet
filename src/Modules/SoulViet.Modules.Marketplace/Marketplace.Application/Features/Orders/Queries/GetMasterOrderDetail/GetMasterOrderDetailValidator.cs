using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetMasterOrderDetail;

public class GetMasterOrderDetailValidator : AbstractValidator<GetMasterOrderDetailQuery>
{
    public GetMasterOrderDetailValidator()
    {
        RuleFor(x => x.MasterOrderId)
            .NotEmpty().WithMessage("MasterOrderId is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("MasterOrderId must be a valid GUID.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("UserId must be a valid GUID.");
    }
}